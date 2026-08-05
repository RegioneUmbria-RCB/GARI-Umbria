Imports System.Transactions

Imports Newtonsoft.Json

Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModello
Imports System.Data.Entity

Public Class G2GPratiche_R

    Dim hash_Pratiche As Hashtable
    Dim hash_Pratiche_Reverse As Hashtable

    Public Sub New()
        hash_Pratiche = New Hashtable
        hash_Pratiche_Reverse = New Hashtable
    End Sub


    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="LetturaCodificheInOrigine">passare true se la lettura avviene in origine</param>
    ''' <param name="objOpzioniImportImpresa"></param>
    ''' <param name="piva"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    Public Function LeggiPerGias2Gias(
        ByVal LetturaCodificheInOrigine As Boolean,
        ByVal objOpzioniImportImpresa As clsImpresa,
        ByVal piva As String,
        ByVal to_piva As String,
        ByVal PivaSuperUser_Destinazione As String,
        ByVal PivaSuperUser_Origine As String,
        ByRef objParametri As AgronicaCoreParametri
    ) As G2G_Pratiche


        Dim NomeRoutine As String = "G2GlocalDal.Analisi_Testata_R.LeggiPerGias2Gias()"



        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
        Dim psu As String = objParametri.PivaSuperUser
        GiasContext.Database.CommandTimeout = 3600
        Dim oConfigurazione As G2G_Configurazione_FiltriReq_Pratiche
        If objOpzioniImportImpresa.configurazione_pratiche <> "" Then
            oConfigurazione = JsonConvert.DeserializeObject(Of G2G_Configurazione_FiltriReq_Pratiche)(objOpzioniImportImpresa.configurazione_pratiche)
        Else
            oConfigurazione = New G2G_Configurazione_FiltriReq_Pratiche
            oConfigurazione.listaServiziWWorkflow = New List(Of G2G_Configurazione_FiltriReq_Pratiche_Servizi)
            oConfigurazione.BloccaOperazioni = New Blocca_Operazioni_Pratiche
        End If

        'lista di appoggio per verificare i servizi ammessi
        Dim listaServiziAmmessi As List(Of Integer) = (
            From ll In oConfigurazione.listaServiziWWorkflow
            Select ll.Servizio_Cod
           ).ToList()

        Dim rval As New G2G_Pratiche

        'quanto sicuramente non assegnato ...
        With rval
            .From_Piva = piva
            .From_PivaSuperUser = objParametri.PivaSuperUser
            .G2G_Pratiche_Recode_insert = New List(Of G2G_Recode_Pratiche)
            .G2G_Pratiche_Recode_update = New List(Of G2G_Recode_Pratiche)
            .G2G_Pratiche_Stati_Recode_insert = New List(Of G2G_Recode_Pratiche_Stati)
            .G2G_Pratiche_Stati_Recode_update = New List(Of G2G_Recode_Pratiche_Stati)
            .To_Piva = to_piva
            .Recode = New G2G_Recode
        End With


        'questa lettura avviene solo in origine al momento
        If LetturaCodificheInOrigine Then
            'assegnazioni, pratiche
            rval.pratiche_insert = (
            From p In GiasContext.Pratiche
            Where p.Piva = piva _
                AndAlso p.Validita_Inizio >= objOpzioniImportImpresa.ValiditaInizio_pratiche _
                AndAlso (oConfigurazione.listaServiziWWorkflow.Count = 0 OrElse listaServiziAmmessi.Contains(p.Servizio_Cod)) _
                AndAlso (
                    LetturaCodificheInOrigine AndAlso Not GiasContext.G2G_Recode_Pratiche.Any(Function(g) g.From_PivaSuperUser = p.Piva_SuperUser AndAlso g.To_PivaSuperUser = PivaSuperUser_Destinazione AndAlso g.From_Pratica_Cod = p.Pratica_Cod) _
                    OrElse Not LetturaCodificheInOrigine AndAlso Not GiasContext.G2G_Recode_Pratiche.Any(Function(g) g.To_PivaSuperUser = p.Piva_SuperUser AndAlso g.From_PivaSuperUser = PivaSuperUser_Destinazione AndAlso g.To_Pratica_Cod = p.Pratica_Cod)
                )
            Select p).ToList()

            rval.pratiche_update = (
            From p In GiasContext.Pratiche
            Join g2g In GiasContext.G2G_Recode_Pratiche
                On p.Pratica_Cod Equals g2g.From_Pratica_Cod _
                And p.Piva_SuperUser Equals g2g.From_PivaSuperUser
            Where p.Piva = piva _
                AndAlso g2g.datainvio < p.Data_Modifica _
                AndAlso g2g.To_PivaSuperUser = PivaSuperUser_Destinazione
            Select p
            ).ToList()
        Else
            rval.pratiche_insert = New List(Of Pratiche)
            rval.pratiche_update = New List(Of Pratiche)
        End If

        'questa lettura avviene solo in origine al momento
        If LetturaCodificheInOrigine Then
            rval.G2G_Pratiche_Recode_delete = (
            From r In GiasContext.G2G_Recode_Pratiche
            Where r.From_PivaSuperUser = psu _
                AndAlso Not GiasContext.Pratiche.Any(Function(p) p.Pratica_Cod = r.From_Pratica_Cod AndAlso p.Piva_SuperUser = r.From_PivaSuperUser AndAlso r.To_PivaSuperUser = PivaSuperUser_Destinazione)
            Select r).ToList()
        Else
            rval.G2G_Pratiche_Recode_delete = New List(Of G2G_Recode_Pratiche)
        End If


        '----------------------
        'assegnazioni, stati
        Dim pratiche_stati_insert_appoggio As List(Of Pratiche_Stati) = (
            From p In GiasContext.Pratiche
            Join s In GiasContext.Pratiche_Stati On
                p.Piva_SuperUser Equals s.Piva_SuperUser And
                p.Pratica_Cod Equals s.Pratica_Cod
            Where p.Piva = piva _
                AndAlso p.Validita_Inizio >= objOpzioniImportImpresa.ValiditaInizio_pratiche _
                AndAlso (oConfigurazione.listaServiziWWorkflow.Count = 0 OrElse listaServiziAmmessi.Contains(p.Servizio_Cod)) _
                AndAlso (
                    LetturaCodificheInOrigine AndAlso Not GiasContext.G2G_Recode_Pratiche_Stati.Any(Function(g) g.From_PivaSuperUser = p.Piva_SuperUser AndAlso g.To_PivaSuperUser = PivaSuperUser_Destinazione AndAlso g.From_PassaggioDiStato_Cod = s.PassaggioDiStato_cod) _
                    OrElse Not LetturaCodificheInOrigine AndAlso Not GiasContext.G2G_Recode_Pratiche_Stati.Any(Function(g) g.To_PivaSuperUser = p.Piva_SuperUser AndAlso g.From_PivaSuperUser = PivaSuperUser_Origine AndAlso g.To_PassaggioDiStato_Cod = s.PassaggioDiStato_cod)
                )
            Select s).ToList()


        'rielaboro la lettura sql se necessario..
        Dim listaVerificaContains As New List(Of String)
        Dim listaVerificaStatoAttuale As New List(Of String)

        Dim listaServiziSenzaStatiEsclusi As List(Of Integer) = (
            From ls In oConfigurazione.listaServiziWWorkflow
            Where ls.PassaggiDiStatoEsclusi.Count = 0
            Select ls.Servizio_Cod).ToList()


        G2GUtility.RecuparaListe(oConfigurazione, listaVerificaContains, listaVerificaStatoAttuale)

        Dim list_PraticheG2G As New List(Of Integer)
        For Each p In rval.pratiche_insert
            list_PraticheG2G.Add(p.Pratica_Cod)
        Next

        If LetturaCodificheInOrigine Then
            list_PraticheG2G.AddRange((From g2gpr In GiasContext.G2G_Recode_Pratiche
                                       Join pr In GiasContext.Pratiche On g2gpr.From_Pratica_Cod Equals pr.Pratica_Cod
                                       Where pr.Piva = piva _
                                       AndAlso g2gpr.To_PivaSuperUser = PivaSuperUser_Destinazione Select pr.Pratica_Cod).ToList)
        Else
            list_PraticheG2G.AddRange((From g2gpr In GiasContext.G2G_Recode_Pratiche
                                       Join pr In GiasContext.Pratiche On g2gpr.To_Pratica_Cod Equals pr.Pratica_Cod
                                       Where pr.Piva = piva _
                                           AndAlso g2gpr.From_PivaSuperUser = PivaSuperUser_Origine Select pr.Pratica_Cod).ToList)
        End If

        'immetto nella lista degli insert solo quelli che non coincidono con una chiave da escludere
        rval.pratiche_stati_insert = New List(Of Pratiche_Stati)
        For Each g In pratiche_stati_insert_appoggio

            Dim prat = (From ps In GiasContext.Pratiche_Stati Where ps.Pratica_Cod = g.Pratica_Cod AndAlso ps.PassaggioDiStato_cod = g.PassaggioDiStato_cod).FirstOrDefault
            If prat IsNot Nothing AndAlso list_PraticheG2G.Contains(prat.Pratica_Cod) Then
                If listaServiziSenzaStatiEsclusi.Contains(g.Servizio_Cod) Then
                    rval.pratiche_stati_insert.Add(g)
                Else
                    Dim k As String = g.Servizio_Cod & G2GUtility.SeparatoreChiave & g.stato_origine_cod & G2GUtility.SeparatoreChiave & g.Stato_Cod

                    If Not listaVerificaContains.Contains(k) OrElse LetturaCodificheInOrigine Then
                        rval.pratiche_stati_insert.Add(g)
                    End If
                End If
            End If
        Next
        'fine lista insert nuovi stati



        'questa lettura avviene solo in origine al momento
        If LetturaCodificheInOrigine Then

            rval.pratiche_stati_update = (
             From p In GiasContext.Pratiche
             Join s In GiasContext.Pratiche_Stati On
                p.Piva_SuperUser Equals s.Piva_SuperUser And
                p.Pratica_Cod Equals s.Pratica_Cod
             Join g2g In GiasContext.G2G_Recode_Pratiche_Stati
                On s.PassaggioDiStato_cod Equals g2g.From_PassaggioDiStato_Cod _
                And s.Piva_SuperUser Equals g2g.From_PivaSuperUser
             Where p.Piva = piva _
                AndAlso g2g.datainvio < s.Data_Modifica _
                 AndAlso g2g.To_PivaSuperUser = PivaSuperUser_Destinazione
             Select s
            ).ToList()
        Else
            rval.pratiche_stati_update = New List(Of Pratiche_Stati)
        End If

        'questa lettura avviene solo in origine al momento 
        'dal momento che l'archivio può comportarsi come destinazione imposto anche filtro per super-user
        If LetturaCodificheInOrigine Then
            rval.G2G_Pratiche_Stati_Recode_delete = (
            From r In GiasContext.G2G_Recode_Pratiche_Stati
            Where r.From_PivaSuperUser = psu _
                AndAlso r.To_PivaSuperUser = PivaSuperUser_Destinazione _
            AndAlso Not GiasContext.Pratiche_Stati.Any(Function(p) p.PassaggioDiStato_cod = r.From_PassaggioDiStato_Cod AndAlso p.Piva_SuperUser = r.From_PivaSuperUser)
            Select r).ToList()
        Else
            rval.G2G_Pratiche_Stati_Recode_delete = New List(Of G2G_Recode_Pratiche_Stati)
        End If

        '----------------------
        'assegnazioni, stato attuale
        Dim pratiche_stati_attuali_insert_appoggio As List(Of Pratiche_Stati_Attuali) = (
            From p In GiasContext.Pratiche
            Join s In GiasContext.Pratiche_Stati_Attuali On
                p.Piva_SuperUser Equals s.Piva_SuperUser And
                p.Pratica_Cod Equals s.Pratica_Cod
            Where p.Piva = piva _
                AndAlso p.Validita_Inizio >= objOpzioniImportImpresa.ValiditaInizio_pratiche _
                AndAlso (oConfigurazione.listaServiziWWorkflow.Count = 0 OrElse listaServiziAmmessi.Contains(p.Servizio_Cod)) _
                AndAlso (
                    LetturaCodificheInOrigine AndAlso Not GiasContext.G2G_Recode_Pratiche.Any(Function(g) g.From_PivaSuperUser = p.Piva_SuperUser AndAlso g.To_PivaSuperUser = PivaSuperUser_Destinazione AndAlso g.From_Pratica_Cod = p.Pratica_Cod) _
                    OrElse Not LetturaCodificheInOrigine AndAlso Not GiasContext.G2G_Recode_Pratiche.Any(Function(g) g.To_PivaSuperUser = p.Piva_SuperUser AndAlso g.From_PivaSuperUser = PivaSuperUser_Origine AndAlso g.To_Pratica_Cod = p.Pratica_Cod)
                )
            Select s).ToList()


        'immetto nella lista dello stato attuale solo quelli che non coincidono con una pratica del servizio da escludere
        rval.pratiche_stati_attuali_insert = New List(Of Pratiche_Stati_Attuali)
        For Each statoAttuale In pratiche_stati_attuali_insert_appoggio

            'verifico se la pratica afferisce al servizio, se afferisce al servizio allora la metto in lista
            Dim p1 As Pratiche = (
                From pInsert In GiasContext.Pratiche
                Where pInsert.Pratica_Cod = statoAttuale.Pratica_Cod
                ).FirstOrDefault

            'se non ci sono verifiche su stati esclusi aggiungo.
            If p1 IsNot Nothing AndAlso listaServiziSenzaStatiEsclusi.Contains(p1.Servizio_Cod) Then
                rval.pratiche_stati_attuali_insert.Add(statoAttuale)
            Else

                For Each curListaVerificaStatoAttuale In listaVerificaStatoAttuale

                    'verifico lo stato, se è diverso significa che non rientra negli stati esclusi e passo a verifica successiva
                    Dim servizioStato As String() = curListaVerificaStatoAttuale.Split(G2GUtility.SeparatoreChiave)
                    Dim servizioVerifica As String = servizioStato(0)
                    Dim statoVerifica As String = servizioStato(1)

                    If statoAttuale.Stato_Cod <> statoVerifica Then

                        If p1 IsNot Nothing AndAlso p1.Servizio_Cod = servizioVerifica Then
                            If Not rval.pratiche_stati_attuali_insert.Any(Function(g) p1.Pratica_Cod = g.Pratica_Cod) Then
                                rval.pratiche_stati_attuali_insert.Add(statoAttuale)
                            End If
                        End If

                    End If
                Next

            End If



        Next
        'fine lista pratiche_stati_attuali_insert_appoggio

        ' VAnni: 25/6/2019: TODO: capire se è sufficiente basarsi sulla data di modifica della pratica ricodificata
        'questa lettura avviene solo in origine al momento


        Dim pratiche_stati_attuali_update_Appoggio As List(Of Pratiche_Stati_Attuali)

        rval.pratiche_stati_attuali_update = New List(Of Pratiche_Stati_Attuali)

        If LetturaCodificheInOrigine Then

            pratiche_stati_attuali_update_Appoggio = (
            From p In GiasContext.Pratiche
            Join s In GiasContext.Pratiche_Stati_Attuali On
                p.Piva_SuperUser Equals s.Piva_SuperUser And
                p.Pratica_Cod Equals s.Pratica_Cod
            Join g2g In GiasContext.G2G_Recode_Pratiche
                On p.Pratica_Cod Equals g2g.From_Pratica_Cod _
                And p.Piva_SuperUser Equals g2g.From_PivaSuperUser
            Where p.Piva = piva _
                AndAlso g2g.datainvio < s.Data_Modifica _
                AndAlso g2g.To_PivaSuperUser = PivaSuperUser_Destinazione
            Select s
            ).ToList()


        Else

            pratiche_stati_attuali_update_Appoggio = (
                From p In GiasContext.Pratiche
                Join s In GiasContext.Pratiche_Stati_Attuali On
                    p.Piva_SuperUser Equals s.Piva_SuperUser And
                    p.Pratica_Cod Equals s.Pratica_Cod
                Where p.Piva = piva
                Select s
                ).ToList()

        End If


        For Each g As Pratiche_Stati_Attuali In pratiche_stati_attuali_update_Appoggio

            'solo su pratiche il cui servizio è ammesso
            Dim p As Pratiche = (
                From p1 In GiasContext.Pratiche
                Where p1.Pratica_Cod = g.Pratica_Cod _
                    AndAlso (oConfigurazione.listaServiziWWorkflow.Count = 0 OrElse listaServiziAmmessi.Contains(p1.Servizio_Cod)) _
                    AndAlso p1.Piva_SuperUser = g.Piva_SuperUser
                Select p1).FirstOrDefault

            'se trovata la pratica
            If p IsNot Nothing Then
                Dim k As String =
                                p.Servizio_Cod & G2GUtility.SeparatoreChiave & g.Stato_Cod

                If Not listaVerificaStatoAttuale.Contains(k) Then
                    rval.pratiche_stati_attuali_update.Add(g)
                End If
            End If

        Next



        ' VAnni: 25/6/2019: è sufficiente verificare se è stata eliminata la pratica, poichè lo stato attuale non è mai eliminabile se non eliminando la pratica
        'questa lettura avviene solo in origine al momento
        If LetturaCodificheInOrigine Then

            rval.pratiche_stati_attuali_delete = (
            From r In GiasContext.G2G_Recode_Pratiche
            Join s In GiasContext.Pratiche_Stati_Attuali On
                r.From_PivaSuperUser Equals s.Piva_SuperUser And
                r.From_Pratica_Cod Equals s.Pratica_Cod
            Where r.From_PivaSuperUser = psu _
                AndAlso r.To_PivaSuperUser = PivaSuperUser_Destinazione _
                AndAlso Not GiasContext.Pratiche.Any(Function(p) p.Pratica_Cod = r.From_Pratica_Cod AndAlso PivaSuperUser_Destinazione = r.To_PivaSuperUser AndAlso p.Piva_SuperUser = r.From_PivaSuperUser)
            Select s).ToList()

        Else
            rval.pratiche_stati_attuali_delete = New List(Of Pratiche_Stati_Attuali)
        End If



        If oConfigurazione.BloccaOperazioni IsNot Nothing AndAlso oConfigurazione.BloccaOperazioni.BloccaOperazioni Then

            For Each r In rval.pratiche_insert
                r.Blocco_Flag = oConfigurazione.BloccaOperazioni.Blocco_Flag
                r.Blocco_Data = DateTime.Now
                r.Blocco_Username = objParametri.UsernameOperazione

                'r.ChangeTracker.State = ObjectState.Modified
                GiasContext.Pratiche.Attach(r)
                GiasContext.Entry(r).State = EntityState.Modified
                GiasContext.SaveChanges()

            Next

            For Each r In rval.pratiche_update
                r.Blocco_Flag = oConfigurazione.BloccaOperazioni.Blocco_Flag
                r.Blocco_Data = DateTime.Now
                r.Blocco_Username = objParametri.UsernameOperazione

                'r.ChangeTracker.State = ObjectState.Modified
                GiasContext.Pratiche.Attach(r)
                GiasContext.Entry(r).State = EntityState.Modified
                GiasContext.SaveChanges()

            Next

        End If


        Return rval

    End Function

    Public Function LeggiPerGias2GiasReverse(
        ByVal LetturaCodificheInOrigine As Boolean,
        ByVal objOpzioniImportImpresa As clsImpresa,
        ByVal piva As String,
        ByVal to_piva As String,
        ByVal PivaSuperUser_Destinazione As String,
        ByRef objParametri As AgronicaCoreParametri
    ) As G2G_Pratiche_Reverse


        Dim NomeRoutine As String = "G2GlocalDal.Analisi_Testata_R.LeggiPerGias2GiasReverse()"



        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
        Dim psu As String = objParametri.PivaSuperUser

        Dim oConfigurazione As G2G_Configurazione_FiltriReq_Pratiche
        If objOpzioniImportImpresa.configurazione_pratiche <> "" Then
            oConfigurazione = JsonConvert.DeserializeObject(Of G2G_Configurazione_FiltriReq_Pratiche)(objOpzioniImportImpresa.configurazione_pratiche)
        Else
            oConfigurazione = New G2G_Configurazione_FiltriReq_Pratiche
            oConfigurazione.listaServiziWWorkflow = New List(Of G2G_Configurazione_FiltriReq_Pratiche_Servizi)
            oConfigurazione.BloccaOperazioni = New Blocca_Operazioni_Pratiche
        End If
        GiasContext.Database.CommandTimeout = 3600
        'lista di appoggio per verificare i servizi ammessi
        Dim listaServiziAmmessi As List(Of Integer) = (
            From ll In oConfigurazione.listaServiziWWorkflow
            Select ll.Servizio_Cod
           ).ToList()

        Dim rval As New G2G_Pratiche_Reverse

        'quanto sicuramente non assegnato ...
        With rval
            .From_Piva = piva
            .From_PivaSuperUser = objParametri.PivaSuperUser
            .G2G_Pratiche_Recode_insert = New List(Of G2G_Recode_Pratiche)
            .G2G_Pratiche_Recode_update = New List(Of G2G_Recode_Pratiche)
            .G2G_Pratiche_Stati_Recode_insert = New List(Of G2G_Recode_Pratiche_Stati)
            .G2G_Pratiche_Stati_Recode_update = New List(Of G2G_Recode_Pratiche_Stati)
            .To_Piva = to_piva
            .Recode = New G2G_Recode
        End With


        'questa lettura avviene solo in origine al momento
        If LetturaCodificheInOrigine Then
            'assegnazioni, pratiche
            rval.pratiche_insert = (
            From p In GiasContext.Pratiche
            Where p.Piva = piva _
                AndAlso p.Validita_Inizio >= objOpzioniImportImpresa.ValiditaInizio_pratiche _
                AndAlso (oConfigurazione.listaServiziWWorkflow.Count = 0 OrElse listaServiziAmmessi.Contains(p.Servizio_Cod)) _
                AndAlso (
                    LetturaCodificheInOrigine AndAlso Not GiasContext.G2G_Recode_Pratiche.Any(Function(g) g.To_PivaSuperUser = p.Piva_SuperUser AndAlso g.From_PivaSuperUser = PivaSuperUser_Destinazione AndAlso g.To_Pratica_Cod = p.Pratica_Cod) _
                    OrElse Not LetturaCodificheInOrigine AndAlso Not GiasContext.G2G_Recode_Pratiche.Any(Function(g) g.From_PivaSuperUser = p.Piva_SuperUser AndAlso g.To_PivaSuperUser = PivaSuperUser_Destinazione AndAlso g.From_Pratica_Cod = p.Pratica_Cod)
                )
            Select p).ToList()

            rval.pratiche_update = (
            From p In GiasContext.Pratiche
            Join g2g In GiasContext.G2G_Recode_Pratiche
                On p.Pratica_Cod Equals g2g.To_Pratica_Cod _
                And p.Piva_SuperUser Equals g2g.To_PivaSuperUser
            Where p.Piva = piva _
                AndAlso g2g.datainvio < p.Data_Modifica _
                AndAlso g2g.From_PivaSuperUser = PivaSuperUser_Destinazione
            Select p
            ).ToList()
        Else
            rval.pratiche_insert = New List(Of Pratiche)
            rval.pratiche_update = New List(Of Pratiche)
        End If

        'questa lettura avviene solo in origine al momento
        If LetturaCodificheInOrigine Then
            rval.G2G_Pratiche_Recode_delete = (
            From r In GiasContext.G2G_Recode_Pratiche
            Where r.To_PivaSuperUser = psu _
                AndAlso Not GiasContext.Pratiche.Any(Function(p) p.Pratica_Cod = r.To_Pratica_Cod AndAlso p.Piva_SuperUser = r.To_PivaSuperUser AndAlso PivaSuperUser_Destinazione = r.From_PivaSuperUser)
            Select r).ToList()
        Else
            rval.G2G_Pratiche_Recode_delete = New List(Of G2G_Recode_Pratiche)
        End If


        '----------------------
        'assegnazioni, stati
        Dim pratiche_stati_insert_appoggio As List(Of Pratiche_Stati) = (
            From p In GiasContext.Pratiche
            Join s In GiasContext.Pratiche_Stati On
                p.Piva_SuperUser Equals s.Piva_SuperUser And
                p.Pratica_Cod Equals s.Pratica_Cod
            Where p.Piva = piva _
                AndAlso p.Validita_Inizio >= objOpzioniImportImpresa.ValiditaInizio_pratiche _
                AndAlso (oConfigurazione.listaServiziWWorkflow.Count = 0 OrElse listaServiziAmmessi.Contains(p.Servizio_Cod)) _
                AndAlso (
                    LetturaCodificheInOrigine AndAlso Not GiasContext.G2G_Recode_Pratiche_Stati.Any(Function(g) g.To_PivaSuperUser = p.Piva_SuperUser AndAlso g.From_PivaSuperUser = PivaSuperUser_Destinazione AndAlso g.To_PassaggioDiStato_Cod = s.PassaggioDiStato_cod) _
                    OrElse Not LetturaCodificheInOrigine AndAlso Not GiasContext.G2G_Recode_Pratiche_Stati.Any(Function(g) g.From_PivaSuperUser = p.Piva_SuperUser AndAlso g.To_PivaSuperUser = PivaSuperUser_Destinazione AndAlso g.From_PassaggioDiStato_Cod = s.PassaggioDiStato_cod)
                )
            Select s).ToList()


        'rielaboro la lettura sql se necessario..
        Dim listaVerificaContains As New List(Of String)
        Dim listaVerificaStatoAttuale As New List(Of String)

        Dim listaServiziSenzaStatiEsclusi As List(Of Integer) = (
            From ls In oConfigurazione.listaServiziWWorkflow
            Where ls.PassaggiDiStatoEsclusi.Count = 0
            Select ls.Servizio_Cod).ToList()


        G2GUtility.RecuparaListe(oConfigurazione, listaVerificaContains, listaVerificaStatoAttuale)

        'immetto nella lista degli insert solo quelli che non coincidono con una chiave da escludere
        rval.pratiche_stati_insert = New List(Of Pratiche_Stati)
        For Each g In pratiche_stati_insert_appoggio

            If listaServiziSenzaStatiEsclusi.Contains(g.Servizio_Cod) Then
                rval.pratiche_stati_insert.Add(g)
            Else
                Dim k As String =
                                g.Servizio_Cod & G2GUtility.SeparatoreChiave & g.stato_origine_cod & G2GUtility.SeparatoreChiave & g.Stato_Cod

                If Not listaVerificaContains.Contains(k) Then
                    rval.pratiche_stati_insert.Add(g)
                End If
            End If
        Next
        'fine lista insert nuovi stati



        'questa lettura avviene solo in origine al momento
        If LetturaCodificheInOrigine Then

            rval.pratiche_stati_update = (
                From p In GiasContext.Pratiche
                Join s In GiasContext.Pratiche_Stati On
                    p.Piva_SuperUser Equals s.Piva_SuperUser And
                    p.Pratica_Cod Equals s.Pratica_Cod
                Join recodePratiche In GiasContext.G2G_Recode_Pratiche On
                    p.Piva_SuperUser Equals recodePratiche.To_PivaSuperUser And
                    p.Pratica_Cod Equals recodePratiche.To_Pratica_Cod
                Join recodeStati In GiasContext.G2G_Recode_Pratiche_Stati On
                    s.PassaggioDiStato_cod Equals recodeStati.To_PassaggioDiStato_Cod And
                    s.Piva_SuperUser Equals recodeStati.To_PivaSuperUser
                Where p.Piva = piva AndAlso
                    recodePratiche.From_PivaSuperUser = PivaSuperUser_Destinazione AndAlso
                    recodeStati.datainvio < s.Data_Modifica AndAlso
                    recodeStati.From_PivaSuperUser = PivaSuperUser_Destinazione
                Select s
            ).ToList()
        Else
            rval.pratiche_stati_update = New List(Of Pratiche_Stati)
        End If

        'questa lettura avviene solo in origine al momento 
        'dal momento che l'archivio può comportarsi come destinazione imposto anche filtro per super-user
        If LetturaCodificheInOrigine Then
            rval.G2G_Pratiche_Stati_Recode_delete = (
            From r In GiasContext.G2G_Recode_Pratiche_Stati
            Where r.To_PivaSuperUser = psu _
            AndAlso Not GiasContext.Pratiche_Stati.Any(Function(p) p.PassaggioDiStato_cod = r.To_PassaggioDiStato_Cod AndAlso p.Piva_SuperUser = r.To_PivaSuperUser AndAlso PivaSuperUser_Destinazione = r.From_PivaSuperUser)
            Select r).ToList()
        Else
            rval.G2G_Pratiche_Stati_Recode_delete = New List(Of G2G_Recode_Pratiche_Stati)
        End If

        '----------------------
        'assegnazioni, stato attuale
        Dim pratiche_stati_attuali_insert_appoggio As List(Of Pratiche_Stati_Attuali) = (
            From p In GiasContext.Pratiche
            Join s In GiasContext.Pratiche_Stati_Attuali On
                p.Piva_SuperUser Equals s.Piva_SuperUser And
                p.Pratica_Cod Equals s.Pratica_Cod
            Where p.Piva = piva _
                AndAlso p.Validita_Inizio >= objOpzioniImportImpresa.ValiditaInizio_pratiche _
                AndAlso (oConfigurazione.listaServiziWWorkflow.Count = 0 OrElse listaServiziAmmessi.Contains(p.Servizio_Cod)) _
                AndAlso (
                    LetturaCodificheInOrigine AndAlso Not GiasContext.G2G_Recode_Pratiche.Any(Function(g) g.To_PivaSuperUser = p.Piva_SuperUser AndAlso g.From_PivaSuperUser = PivaSuperUser_Destinazione AndAlso g.To_Pratica_Cod = p.Pratica_Cod) _
                    OrElse Not LetturaCodificheInOrigine AndAlso Not GiasContext.G2G_Recode_Pratiche.Any(Function(g) g.From_PivaSuperUser = p.Piva_SuperUser AndAlso g.To_PivaSuperUser = PivaSuperUser_Destinazione AndAlso g.From_Pratica_Cod = p.Pratica_Cod)
                )
            Select s).ToList()


        'immetto nella lista dello stato attuale solo quelli che non coincidono con una pratica del servizio da escludere
        rval.pratiche_stati_attuali_insert = New List(Of Pratiche_Stati_Attuali)
        For Each statoAttuale In pratiche_stati_attuali_insert_appoggio

            'verifico se la pratica afferisce al servizio, se afferisce al servizio allora la metto in lista
            Dim p1 As Pratiche = (
                From pInsert In GiasContext.Pratiche
                Where pInsert.Pratica_Cod = statoAttuale.Pratica_Cod
                ).FirstOrDefault

            'se non ci sono verifiche su stati esclusi aggiungo.
            If p1 IsNot Nothing AndAlso listaServiziSenzaStatiEsclusi.Contains(p1.Servizio_Cod) Then
                rval.pratiche_stati_attuali_insert.Add(statoAttuale)
            Else

                For Each curListaVerificaStatoAttuale In listaVerificaStatoAttuale

                    'verifico lo stato, se è diverso significa che non rientra negli stati esclusi e passo a verifica successiva
                    Dim servizioStato As String() = curListaVerificaStatoAttuale.Split(G2GUtility.SeparatoreChiave)
                    Dim servizioVerifica As String = servizioStato(0)
                    Dim statoVerifica As String = servizioStato(1)

                    If statoAttuale.Stato_Cod <> statoVerifica Then

                        If p1 IsNot Nothing AndAlso p1.Servizio_Cod = servizioVerifica Then
                            If Not rval.pratiche_stati_attuali_insert.Any(Function(g) p1.Pratica_Cod = g.Pratica_Cod) Then
                                rval.pratiche_stati_attuali_insert.Add(statoAttuale)
                            End If
                        End If

                    End If
                Next

            End If



        Next
        'fine lista pratiche_stati_attuali_insert_appoggio

        ' VAnni: 25/6/2019: TODO: capire se è sufficiente basarsi sulla data di modifica della pratica ricodificata
        'questa lettura avviene solo in origine al momento


        Dim pratiche_stati_attuali_update_Appoggio As List(Of Pratiche_Stati_Attuali)

        rval.pratiche_stati_attuali_update = New List(Of Pratiche_Stati_Attuali)

        If LetturaCodificheInOrigine Then

            pratiche_stati_attuali_update_Appoggio = (
            From p In GiasContext.Pratiche
            Join s In GiasContext.Pratiche_Stati_Attuali On
                p.Piva_SuperUser Equals s.Piva_SuperUser And
                p.Pratica_Cod Equals s.Pratica_Cod
            Join g2g In GiasContext.G2G_Recode_Pratiche
                On p.Pratica_Cod Equals g2g.To_Pratica_Cod _
                And p.Piva_SuperUser Equals g2g.To_PivaSuperUser
            Where p.Piva = piva _
                AndAlso g2g.datainvio < s.Data_Modifica _
                AndAlso g2g.From_PivaSuperUser = PivaSuperUser_Destinazione
            Select s
            ).ToList()


        Else

            pratiche_stati_attuali_update_Appoggio = (
                From p In GiasContext.Pratiche
                Join s In GiasContext.Pratiche_Stati_Attuali On
                    p.Piva_SuperUser Equals s.Piva_SuperUser And
                    p.Pratica_Cod Equals s.Pratica_Cod
                Where p.Piva = piva
                Select s
                ).ToList()

        End If


        For Each g As Pratiche_Stati_Attuali In pratiche_stati_attuali_update_Appoggio

            'solo su pratiche il cui servizio è ammesso
            Dim p As Pratiche = (
                From p1 In GiasContext.Pratiche
                Where p1.Pratica_Cod = g.Pratica_Cod _
                      AndAlso (oConfigurazione.listaServiziWWorkflow.Count = 0 OrElse listaServiziAmmessi.Contains(p1.Servizio_Cod)) _
                      AndAlso p1.Piva_SuperUser = g.Piva_SuperUser
                Select p1).FirstOrDefault

            'se trovata la pratica
            If p IsNot Nothing Then
                Dim k As String =
                                p.Servizio_Cod & G2GUtility.SeparatoreChiave & g.Stato_Cod

                If Not listaVerificaStatoAttuale.Contains(k) Then
                    rval.pratiche_stati_attuali_update.Add(g)
                End If
            End If

        Next



        ' VAnni: 25/6/2019: è sufficiente verificare se è stata eliminata la pratica, poiché lo stato attuale non è mai eliminabile se non eliminando la pratica
        'questa lettura avviene solo in origine al momento
        If LetturaCodificheInOrigine Then

            rval.pratiche_stati_attuali_delete = (
            From r In GiasContext.G2G_Recode_Pratiche
            Join s In GiasContext.Pratiche_Stati_Attuali On
                r.To_PivaSuperUser Equals s.Piva_SuperUser And
                r.To_Pratica_Cod Equals s.Pratica_Cod
            Where r.To_PivaSuperUser = psu _
                AndAlso Not GiasContext.Pratiche.Any(Function(p) p.Pratica_Cod = r.To_Pratica_Cod AndAlso p.Piva_SuperUser = r.To_PivaSuperUser AndAlso PivaSuperUser_Destinazione = r.From_PivaSuperUser)
            Select s).ToList()

        Else
            rval.pratiche_stati_attuali_delete = New List(Of Pratiche_Stati_Attuali)
        End If



        If oConfigurazione.BloccaOperazioni IsNot Nothing AndAlso oConfigurazione.BloccaOperazioni.BloccaOperazioni Then

            For Each r In rval.pratiche_insert
                r.Blocco_Flag = oConfigurazione.BloccaOperazioni.Blocco_Flag
                r.Blocco_Data = DateTime.Now
                r.Blocco_Username = objParametri.UsernameOperazione

                'r.ChangeTracker.State = ObjectState.Modified
                GiasContext.Pratiche.Attach(r)
                GiasContext.Entry(r).State = EntityState.Modified
                GiasContext.SaveChanges()

            Next

            For Each r In rval.pratiche_update
                r.Blocco_Flag = oConfigurazione.BloccaOperazioni.Blocco_Flag
                r.Blocco_Data = DateTime.Now
                r.Blocco_Username = objParametri.UsernameOperazione

                'r.ChangeTracker.State = ObjectState.Modified
                GiasContext.Pratiche.Attach(r)
                GiasContext.Entry(r).State = EntityState.Modified
                GiasContext.SaveChanges()

            Next

        End If


        Return rval

    End Function

    Public Function ricodifica_Pratica_Cod(GiasContext As Gias_DeveloperServer_Entities, Origine_Piva_SuperUser As String, From_Pratica_Cod As String) As String
        Dim newStrPratiche As String = ""
        If Not String.IsNullOrEmpty(From_Pratica_Cod) AndAlso From_Pratica_Cod <> "0" Then
            Dim chiave_From = Origine_Piva_SuperUser & "_" & From_Pratica_Cod
            If Not hash_Pratiche.Contains(chiave_From) Then
                Dim arrayPratiche As String() = From_Pratica_Cod.Split({"|"}, StringSplitOptions.RemoveEmptyEntries)
                For Each pratica As String In arrayPratiche
                    Dim pratica_dest = (From rr In GiasContext.G2G_Recode_Pratiche Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_Pratica_Cod = pratica Select rr.To_Pratica_Cod).FirstOrDefault()
                    If pratica_dest <> 0 Then
                        newStrPratiche &= "|" & pratica_dest
                    End If
                Next
                hash_Pratiche.Add(chiave_From, newStrPratiche)
            Else
                Return hash_Pratiche.Item(chiave_From)
            End If
        Else
            newStrPratiche = "0"
        End If

        Return newStrPratiche
    End Function

    Public Function ricodifica_Pratica_CodReverse(GiasContext As Gias_DeveloperServer_Entities, Origine_Piva_SuperUser As String, From_Pratica_Cod1 As String) As String
        Dim newStrPratiche As String = ""
        If Not String.IsNullOrEmpty(From_Pratica_Cod1) AndAlso From_Pratica_Cod1 <> "0" Then
            Dim chiave_From = Origine_Piva_SuperUser & "_" & From_Pratica_Cod1
            If Not hash_Pratiche_Reverse.Contains(chiave_From) Then
                Dim arrayPratiche As String() = From_Pratica_Cod1.Split({"|"}, StringSplitOptions.RemoveEmptyEntries)
                For Each pratica As String In arrayPratiche
                    Dim pratica_dest = (From rr In GiasContext.G2G_Recode_Pratiche Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_Pratica_Cod = pratica Select rr.From_Pratica_Cod).FirstOrDefault()
                    If pratica_dest <> 0 Then
                        newStrPratiche &= "|" & pratica_dest
                    End If
                Next
                hash_Pratiche_Reverse.Add(chiave_From, newStrPratiche)
            Else
                Return hash_Pratiche_Reverse.Item(chiave_From)
            End If
        Else
            newStrPratiche = "0"
        End If

        Return newStrPratiche
    End Function

End Class


Public Class G2GPratiche_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi_Pratiche_G2G_Recode(ByRef g2g As G2G_Pratiche, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GPratiche_W.Scrivi_Pratiche_G2G_Recode()"
        Dim messaggioErrore As String = ""

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Dim transactionOptions As New TransactionOptions()
        transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
        transactionOptions.Timeout = G2GUtility.GetTransactionTimeout(objParametri)

        Try

            Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    If g2g.G2G_Pratiche_Recode_insert IsNot Nothing Then
                        For Each recode As G2G_Recode_Pratiche In g2g.G2G_Pratiche_Recode_insert
                            GiasContext.G2G_Recode_Pratiche.Add(recode)
                            GiasContext.SaveChanges()
                        Next
                    End If

                    If g2g.G2G_Pratiche_Recode_update IsNot Nothing Then
                        For Each recode As G2G_Recode_Pratiche In g2g.G2G_Pratiche_Recode_update
                            GiasContext.G2G_Recode_Pratiche.Attach(recode)
                            GiasContext.Entry(recode).State = EntityState.Modified
                        Next
                    End If

                    If g2g.G2G_Pratiche_Recode_delete IsNot Nothing Then
                        For Each recode As G2G_Recode_Pratiche In g2g.G2G_Pratiche_Recode_delete
                            GiasContext.G2G_Recode_Pratiche.Attach(recode)
                            GiasContext.G2G_Recode_Pratiche.Remove(recode)
                        Next
                    End If

                    If g2g.G2G_Pratiche_Stati_Recode_insert IsNot Nothing Then
                        For Each recode As G2G_Recode_Pratiche_Stati In g2g.G2G_Pratiche_Stati_Recode_insert
                            GiasContext.G2G_Recode_Pratiche_Stati.Add(recode)
                            GiasContext.SaveChanges()
                        Next
                    End If

                    If g2g.G2G_Pratiche_Stati_Recode_update IsNot Nothing Then
                        For Each recode As G2G_Recode_Pratiche_Stati In g2g.G2G_Pratiche_Stati_Recode_update
                            GiasContext.G2G_Recode_Pratiche_Stati.Attach(recode)
                            GiasContext.Entry(recode).State = EntityState.Modified
                        Next
                    End If

                    If g2g.G2G_Pratiche_Stati_Recode_delete IsNot Nothing Then
                        For Each recode As G2G_Recode_Pratiche_Stati In g2g.G2G_Pratiche_Stati_Recode_delete
                            GiasContext.G2G_Recode_Pratiche_Stati.Attach(recode)
                            GiasContext.G2G_Recode_Pratiche_Stati.Remove(recode)
                        Next
                    End If

                    If g2g.Recode IsNot Nothing Then

                        If g2g.Recode.G2GRecodePraticheStatiToInsert IsNot Nothing Then
                            For Each recode As G2G_Recode_Pratiche_Stati In g2g.Recode.G2GRecodePraticheStatiToInsert
                                GiasContext.G2G_Recode_Pratiche_Stati.Add(recode)
                                GiasContext.SaveChanges()
                            Next
                        End If

                    End If




                    ' COMIT Effettivo
                    scope.Complete()

                End Using
            End Using

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore, ex)

        End Try

        Return messaggioErrore

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="LetturaCodificheInOrigine">passare true se la lettura avviene in origine</param>
    ''' <param name="Origine_Piva_SuperUser"></param>
    ''' <param name="Destinazione_Piva_SuperUser"></param>
    ''' <param name="g2g"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    Public Function Scrivi_Pratiche_G2G(
        ByVal LetturaCodificheInOrigine As Boolean,
        Origine_Piva_SuperUser As String,
        Destinazione_Piva_SuperUser As String,
        ByRef g2g As G2G_Pratiche,
        ByRef objParametri As AgronicaCoreParametri
    ) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GPratiche_W.Scrivi_Pratiche_G2G()"
        Dim messaggioErrore As String = ""

        Dim username = objParametri.UsernameOperazione
        Dim piva = g2g.To_Piva
        Dim data = Date.Now

        Dim objSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Dim transactionOptions As New TransactionOptions()
        transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
        transactionOptions.Timeout = G2GUtility.GetTransactionTimeout(objParametri)

        Try

            Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    Dim idSeq As Integer = 0
                    g2g.Recode.G2GRecodePraticheToInsert = New List(Of G2G_Recode_Pratiche)
                    For Each m As Pratiche In g2g.pratiche_insert

                        'Richiedo un nuovo id sequenza
                        idSeq = objSequenze.NuovoId_Tabella_EF(GiasContext, "Pratiche", 0, 2000000000, objParametri)

                        Dim pratiche = Gias_EF_Utility.CopyEntity(GiasContext, m, Nothing, username, data)
                        pratiche.Piva = piva
                        pratiche.Pratica_Cod = idSeq
                        pratiche.Piva_SuperUser = Destinazione_Piva_SuperUser
                        GiasContext.Pratiche.Add(pratiche)

                        Dim recode =
                            New G2G_Recode_Pratiche With {
                                .From_PivaSuperUser = Origine_Piva_SuperUser,
                                .To_PivaSuperUser = Destinazione_Piva_SuperUser,
                                .From_Pratica_Cod = m.Pratica_Cod,
                                .To_Pratica_Cod = idSeq,
                                .Username_Creazione = username,
                                .Username_Modifica = username,
                                .Data_Creazione = Now(),
                                .Data_Modifica = Now(),
                                .Validita_Inizio = AGRODATAINIZIO,
                                .Validita_Fine = AGRODATAFINE,
                                .inviato = "0",
                                .datainvio = Now()
                            }
                        g2g.G2G_Pratiche_Recode_insert.Add(recode)
                        g2g.Recode.G2GRecodePraticheToInsert.Add(recode)
                        GiasContext.G2G_Recode_Pratiche.Add(recode)

                        GiasContext.SaveChanges()

                    Next


                    g2g.Recode.G2GRecodePraticheStatiToInsert = New List(Of G2G_Recode_Pratiche_Stati)
                    For Each s As Pratiche_Stati In g2g.pratiche_stati_insert


                        'Richiedo un nuovo id sequenza
                        idSeq = objSequenze.NuovoId_Tabella_EF(GiasContext, "passaggiodistato_cod", 0, 2000000000, objParametri)
                        Dim recodePratica As G2G_Recode_Pratiche

                        'se la lettura avviene con i codici origine allora l'oggetto pratiche_Stati contiene codici "origine", 
                        'altrimenti contiene codici che provengono dalla "destinazione"..
                        'le p.iva super-user vengono invece chiamate già correttamente come parametri della funzione.
                        If LetturaCodificheInOrigine Then
                            recodePratica = (From rr In GiasContext.G2G_Recode_Pratiche Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_Pratica_Cod = s.Pratica_Cod).FirstOrDefault()
                        Else
                            recodePratica = (From rr In GiasContext.G2G_Recode_Pratiche Where rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.To_Pratica_Cod = s.Pratica_Cod).FirstOrDefault()
                        End If

                        Dim praticheStati = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)

                        praticheStati.Piva_SuperUser = Destinazione_Piva_SuperUser

                        If LetturaCodificheInOrigine Then
                            praticheStati.Pratica_Cod = recodePratica.To_Pratica_Cod
                        Else
                            praticheStati.Pratica_Cod = recodePratica.From_Pratica_Cod
                        End If

                        praticheStati.PassaggioDiStato_cod = idSeq

                        GiasContext.Pratiche_Stati.Add(praticheStati)

                        Dim fromPassaggioDiStatoCod As Integer = s.PassaggioDiStato_cod
                        Dim toPassaggioDiStatoCod As Integer = idSeq

                        Dim recode As G2G_Recode_Pratiche_Stati

                        If LetturaCodificheInOrigine Then

                            recode = New G2G_Recode_Pratiche_Stati With {
                                .From_PivaSuperUser = Origine_Piva_SuperUser,
                                .To_PivaSuperUser = Destinazione_Piva_SuperUser,
                                .From_PassaggioDiStato_Cod = fromPassaggioDiStatoCod,
                                .To_PassaggioDiStato_Cod = toPassaggioDiStatoCod,
                                .Username_Creazione = username,
                                .Username_Modifica = username,
                                .Data_Creazione = Now(),
                                .Data_Modifica = Now(),
                                .Validita_Inizio = AGRODATAINIZIO,
                                .Validita_Fine = AGRODATAFINE,
                                .inviato = "0",
                                .datainvio = Now()
                            }

                        Else

                            recode = New G2G_Recode_Pratiche_Stati With {
                                .From_PivaSuperUser = Destinazione_Piva_SuperUser,
                                .To_PivaSuperUser = Origine_Piva_SuperUser,
                                .From_PassaggioDiStato_Cod = toPassaggioDiStatoCod,
                                .To_PassaggioDiStato_Cod = fromPassaggioDiStatoCod,
                                .Username_Creazione = username,
                                .Username_Modifica = username,
                                .Data_Creazione = Now(),
                                .Data_Modifica = Now(),
                                .Validita_Inizio = AGRODATAINIZIO,
                                .Validita_Fine = AGRODATAFINE,
                                .inviato = "0",
                                .datainvio = Now()}

                        End If

                        g2g.G2G_Pratiche_Stati_Recode_insert.Add(recode)
                        g2g.Recode.G2GRecodePraticheStatiToInsert.Add(recode)
                        GiasContext.G2G_Recode_Pratiche_Stati.Add(recode)


                        'Grilli 12/09/2019: In caso di PULL, blocco le operazioni d'agenda dalla notte dei tempi fino alla data in cui l'utente ha messo lo stato "Compilazione alla data completata e verificata" (2007)
                        If Not LetturaCodificheInOrigine Then
                            If s.Stato_Cod = enum_WWorflow_WAnagraficaStati.Quaderno_Campagna_Verifica_in_corso AndAlso
                               s.stato_origine_cod = enum_WWorflow_WAnagraficaStati.Quaderno_Campagna_Compilazione_Alla_Data_Completata_e_Verificata Then

                                Dim dataInizioBlocco As DateTime = AGRODATAINIZIO

                                'Cerco la data dalla quale devo finire di sbloccare:
                                Dim dataFineBlocco As DateTime = (From p1 In GiasContext.Pratiche_Stati Where p1.Pratica_Cod = praticheStati.Pratica_Cod AndAlso p1.Stato_Cod = CInt(enum_WWorflow_WAnagraficaStati.Quaderno_Campagna_Compilazione_Alla_Data_Completata_e_Verificata) Select p1).OrderByDescending(Function(x) x.Validita_Inizio).Select(Function(x) x.Validita_Inizio).First

                                'Se la data è successiva a quella della pratica
                                Dim prat As Pratiche = (From p1 In GiasContext.Pratiche Where p1.Pratica_Cod = praticheStati.Pratica_Cod).FirstOrDefault
                                If Not IsNothing(prat) AndAlso IsDate(prat.Validita_Fine) AndAlso CDate(prat.Validita_Fine) <> AGRODATAFINE AndAlso dataFineBlocco > CDate(prat.Validita_Fine) Then
                                    dataFineBlocco = CDate(prat.Validita_Fine)
                                End If

                                'Cerco la piva
                                'Dim pivaPerBlocco As String = (From px In GiasContext.Pratiche Where px.Pratica_Cod = praticheStati.Pratica_Cod AndAlso px.Servizio_Cod = s.Servizio_Cod Select (px.Piva)).First


                                Select Case s.Servizio_Cod
                                    Case enum_Servizi.Quaderno_Campagna_Caa

                                        'Blocco l'agenda
                                        Dim ag_2Upd As List(Of Agenda) = (From x In GiasContext.Agenda Where x.PIVA = piva AndAlso x.Blocco_Flag = 0 AndAlso x.Validita_Inizio >= dataInizioBlocco AndAlso x.Validita_Inizio <= dataFineBlocco).ToList()

                                        For Each ag As Agenda In ag_2Upd
                                            ag.Blocco_Flag = 1
                                            ag.Blocco_Data = s.Data_Modifica
                                            ag.Blocco_Username = s.Username_Modifica

                                            GiasContext.Agenda.Attach(ag)
                                            GiasContext.Entry(ag).State = EntityState.Modified
                                        Next
                                    Case enum_Servizi.QuadernoCampagnaBio

                                        'Blocco l'agenda
                                        Dim ag_2Upd As List(Of Agenda) = (From x In GiasContext.Agenda Where x.PIVA = piva AndAlso x.Blocco_Flag = 0 AndAlso x.Validita_Inizio >= dataInizioBlocco AndAlso x.Validita_Inizio <= dataFineBlocco).ToList()

                                        For Each ag As Agenda In ag_2Upd
                                            ag.Blocco_Flag = 1
                                            ag.Blocco_Data = s.Data_Modifica
                                            ag.Blocco_Username = s.Username_Modifica

                                            GiasContext.Agenda.Attach(ag)
                                            GiasContext.Entry(ag).State = EntityState.Modified
                                        Next

                                    Case enum_Servizi.PianoConcimazione

                                        'Blocco le testate del Piano Concimazione
                                        Dim pct_2Upd As List(Of PianoConcimazione_Testata) = (From pt In GiasContext.PianoConcimazione_Testata
                                                                                              Join pd In GiasContext.PianoConcimazione_Dettagli
                                                                                                  On pt.PC_Testata_Cod Equals pd.PC_Testata_Cod
                                                                                              Where pd.PC_Dettagli_PIVA = piva AndAlso pt.Blocco_Flag = 0 _
                                                                                                  AndAlso pt.Validita_Fine >= dataInizioBlocco AndAlso pt.Validita_Inizio <= dataFineBlocco
                                                                                              Select pt).ToList()

                                        For Each pct As PianoConcimazione_Testata In pct_2Upd
                                            pct.Blocco_Flag = 1
                                            pct.Blocco_Data = s.Data_Modifica
                                            pct.Blocco_Username = s.Username_Modifica

                                            GiasContext.PianoConcimazione_Testata.Attach(pct)
                                            GiasContext.Entry(pct).State = EntityState.Modified
                                        Next

                                        'Blocco le ricette 
                                        Dim ric_2Upd As List(Of Ricette) = (From r In GiasContext.Ricette
                                                                            Join pt In GiasContext.PianoConcimazione_Testata
                                                                                On pt.PC_Testata_Cod Equals r.Programmazione_Cod
                                                                            Join pd In GiasContext.PianoConcimazione_Dettagli
                                                                                On pt.PC_Testata_Cod Equals pd.PC_Testata_Cod
                                                                            Where pd.PC_Dettagli_PIVA = piva AndAlso r.Blocco_Flag = 0 AndAlso r.Tipo_Ricetta = CInt(enum_TipoRicetta.PianoDistribuzioneConcimi) _
                                                                                AndAlso pt.Validita_Fine >= dataInizioBlocco AndAlso pt.Validita_Inizio <= dataFineBlocco
                                                                            Select r).ToList()

                                        For Each ric As Ricette In ric_2Upd
                                            ric.Blocco_Flag = 1
                                            ric.Blocco_Data = s.Data_Modifica
                                            ric.Blocco_Username = s.Username_Modifica

                                            GiasContext.Ricette.Attach(ric)
                                            GiasContext.Entry(ric).State = EntityState.Modified
                                        Next

                                    Case enum_Servizi.PUA

                                        'Blocco le testate del PUA
                                        Dim pt_2Upd As List(Of PUA_Testata) = (From x In GiasContext.PUA_Testata Where x.Piva = piva AndAlso x.Blocco_Flag = 0 AndAlso x.Validita_Inizio >= dataInizioBlocco AndAlso x.Validita_Inizio <= dataFineBlocco).ToList()

                                        For Each pt As PUA_Testata In pt_2Upd
                                            pt.Blocco_Flag = 1
                                            pt.Blocco_Data = s.Data_Modifica
                                            pt.Blocco_Username = s.Username_Modifica

                                            GiasContext.PUA_Testata.Attach(pt)
                                            GiasContext.Entry(pt).State = EntityState.Modified
                                        Next

                                        'Blocco le ricette 
                                        Dim ric_2Upd As List(Of Ricette) = (From r In GiasContext.Ricette
                                                                            Join pt In GiasContext.PUA_Testata
                                                                                On pt.PUA_Cod Equals r.Programmazione_Cod
                                                                            Where pt.Piva = piva AndAlso r.Blocco_Flag = 0 AndAlso r.Tipo_Ricetta = CInt(enum_TipoRicetta.PianoDistribuzionePua) _
                                                                                AndAlso pt.Validita_Inizio >= dataInizioBlocco AndAlso pt.Validita_Inizio <= dataFineBlocco
                                                                            Select r).ToList()

                                        For Each ric As Ricette In ric_2Upd
                                            ric.Blocco_Flag = 1
                                            ric.Blocco_Data = s.Data_Modifica
                                            ric.Blocco_Username = s.Username_Modifica

                                            GiasContext.Ricette.Attach(ric)
                                            GiasContext.Entry(ric).State = EntityState.Modified
                                        Next

                                End Select

                            End If

                        End If

                        GiasContext.SaveChanges()

                    Next

                    For Each s As Pratiche_Stati_Attuali In g2g.pratiche_stati_attuali_insert

                        'Richiedo un nuovo id sequenza                                    
                        Dim recodePratica = (From rr In GiasContext.G2G_Recode_Pratiche Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_Pratica_Cod = s.Pratica_Cod).FirstOrDefault()
                        Dim praticheStatoAttuale = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)
                        praticheStatoAttuale.Pratica_Cod = recodePratica.To_Pratica_Cod
                        praticheStatoAttuale.Piva_SuperUser = Destinazione_Piva_SuperUser
                        GiasContext.Pratiche_Stati_Attuali.Add(praticheStatoAttuale)
                        GiasContext.SaveChanges()

                    Next

                    g2g.Recode.G2GRecodePraticheToUpdate = New List(Of G2G_Recode_Pratiche)
                    For Each m As Pratiche In g2g.pratiche_update

                        Dim recode = (From rr In GiasContext.G2G_Recode_Pratiche Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_Pratica_Cod = m.Pratica_Cod).FirstOrDefault()
                        recode.Username_Modifica = username
                        recode.Data_Modifica = data
                        recode.datainvio = data
                        g2g.G2G_Pratiche_Recode_update.Add(recode)
                        g2g.Recode.G2GRecodePraticheToUpdate.Add(recode)
                        GiasContext.G2G_Recode_Pratiche.Attach(recode)
                        GiasContext.Entry(recode).State = EntityState.Modified

                        Dim pratiche = (From mm In GiasContext.Pratiche Where mm.Pratica_Cod = recode.To_Pratica_Cod).FirstOrDefault()
                        pratiche = Gias_EF_Utility.CopyEntity(GiasContext, m, pratiche, username, data)
                        pratiche.Piva_SuperUser = Destinazione_Piva_SuperUser
                        pratiche.Piva = piva
                        pratiche.Pratica_Cod = recode.To_Pratica_Cod
                        GiasContext.Pratiche.Attach(pratiche)
                        GiasContext.Entry(pratiche).State = EntityState.Modified

                        GiasContext.SaveChanges()

                    Next

                    g2g.Recode.G2GRecodePraticheStatiToUpdate = New List(Of G2G_Recode_Pratiche_Stati)
                    For Each m As Pratiche_Stati In g2g.pratiche_stati_update


                        Dim recode = (From rr In GiasContext.G2G_Recode_Pratiche_Stati Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PassaggioDiStato_Cod = m.PassaggioDiStato_cod).FirstOrDefault()
                        recode.Username_Modifica = username
                        recode.Data_Modifica = data
                        recode.datainvio = data
                        g2g.G2G_Pratiche_Stati_Recode_update.Add(recode)
                        g2g.Recode.G2GRecodePraticheStatiToUpdate.Add(recode)
                        GiasContext.G2G_Recode_Pratiche_Stati.Attach(recode)
                        GiasContext.Entry(recode).State = EntityState.Modified

                        Dim praticheStati = (From mm In GiasContext.Pratiche_Stati Where mm.PassaggioDiStato_cod = recode.To_PassaggioDiStato_Cod).FirstOrDefault()
                        Dim lPraticaCod As Integer = praticheStati.Pratica_Cod
                        Dim lpassaggioDiStatoCod As Integer = praticheStati.PassaggioDiStato_cod
                        praticheStati = Gias_EF_Utility.CopyEntity(GiasContext, m, praticheStati, username, data)
                        praticheStati.Piva_SuperUser = Destinazione_Piva_SuperUser
                        praticheStati.PassaggioDiStato_cod = lpassaggioDiStatoCod
                        praticheStati.Pratica_Cod = lPraticaCod

                        GiasContext.Pratiche_Stati.Attach(praticheStati)
                        GiasContext.Entry(praticheStati).State = EntityState.Modified
                        GiasContext.SaveChanges()

                    Next


                    For Each m As Pratiche_Stati_Attuali In g2g.pratiche_stati_attuali_update

                        'leggo in virtù del parametro (piva super user è ok, poiché passata "invertita")
                        Dim recode As G2G_Recode_Pratiche
                        If LetturaCodificheInOrigine Then
                            'sono sulla destinazione, lo scopo è leggere un recode destinazione partendo da codici origine
                            recode = (From rr In GiasContext.G2G_Recode_Pratiche Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_Pratica_Cod = m.Pratica_Cod).FirstOrDefault()
                        Else
                            'sono in origine, lo scopo è leggere un recode di origine partendo da destinazione
                            recode = (From rr In GiasContext.G2G_Recode_Pratiche Where rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.To_Pratica_Cod = m.Pratica_Cod).FirstOrDefault()
                        End If

                        If recode Is Nothing Then
                            Throw New Exception("Errore, non esiste la decodifica di Pratica_Cod = " & m.Pratica_Cod & " in G2G_Recode_Pratiche")
                        End If

                        Dim praticheStatiAttuali As Pratiche_Stati_Attuali
                        If LetturaCodificheInOrigine Then
                            'sono in destinazione, lo scopo è leggere lo stato attuale partendo dalle chiavi destinazione.
                            praticheStatiAttuali = (From mm In GiasContext.Pratiche_Stati_Attuali Where mm.Piva_SuperUser = Destinazione_Piva_SuperUser AndAlso mm.Pratica_Cod = recode.To_Pratica_Cod).FirstOrDefault()
                        Else
                            'sono in origine, lo scopo è leggere lo stato attuale partendo dalle chiavi origine
                            praticheStatiAttuali = (From mm In GiasContext.Pratiche_Stati_Attuali Where mm.Piva_SuperUser = Destinazione_Piva_SuperUser AndAlso mm.Pratica_Cod = recode.From_Pratica_Cod).FirstOrDefault()
                        End If

                        If praticheStatiAttuali Is Nothing Then
                            Throw New Exception("Errore, non esiste riferimento a Pratica_Cod = " & If(LetturaCodificheInOrigine, recode.To_Pratica_Cod, recode.From_Pratica_Cod) & " in Pratiche_Stati_Attuali")
                        End If

                        'cancella e riscrive per ovviare al fatto stat_cod che si trova in chiave primaria
                        GiasContext.Pratiche_Stati_Attuali.Remove(praticheStatiAttuali)
                        GiasContext.SaveChanges()

                        Dim lPraticaCod As Integer = praticheStatiAttuali.Pratica_Cod
                        praticheStatiAttuali = Gias_EF_Utility.CopyEntity(GiasContext, m, praticheStatiAttuali, username, data)
                        praticheStatiAttuali.Piva_SuperUser = Destinazione_Piva_SuperUser
                        praticheStatiAttuali.Pratica_Cod = lPraticaCod

                        GiasContext.Pratiche_Stati_Attuali.Add(praticheStatiAttuali)
                        GiasContext.SaveChanges()

                    Next

                    'cancellazioni, da gestire ... 
                    Dim list_G2G_Recode_Pratiche_Stati_Delete As New List(Of G2G_Recode_Pratiche_Stati)
                    For Each r As G2G_Recode_Pratiche_Stati In g2g.G2G_Pratiche_Stati_Recode_delete

                        'cancella stato
                        Dim stato = (From s In GiasContext.Pratiche_Stati
                                     Join p In GiasContext.Pratiche On s.Pratica_Cod Equals p.Pratica_Cod
                                     Where p.Piva_SuperUser = Destinazione_Piva_SuperUser _
                                         AndAlso p.Piva = piva _
                                         AndAlso s.PassaggioDiStato_cod = r.To_PassaggioDiStato_Cod
                                     Select s).FirstOrDefault()

                        If stato IsNot Nothing Then

                            GiasContext.Pratiche_Stati.Attach(stato)
                            GiasContext.Pratiche_Stati.Remove(stato)


                            ' cancella recode stato
                            Dim recode = (From rr In GiasContext.G2G_Recode_Pratiche_Stati
                                          Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso
                                                rr.From_PassaggioDiStato_Cod = r.From_PassaggioDiStato_Cod).FirstOrDefault()

                            If recode IsNot Nothing Then

                                GiasContext.G2G_Recode_Pratiche_Stati.Attach(recode)
                                GiasContext.G2G_Recode_Pratiche_Stati.Remove(recode)

                                'g2g.G2G_Pratiche_Stati_Recode_delete.Add(recode)
                                list_G2G_Recode_Pratiche_Stati_Delete.Add(recode)
                            End If

                            GiasContext.SaveChanges()
                        End If

                    Next

                    g2g.G2G_Pratiche_Stati_Recode_delete = list_G2G_Recode_Pratiche_Stati_Delete
                    g2g.Recode.G2GRecodePraticheStatiToDelete = list_G2G_Recode_Pratiche_Stati_Delete

                    Dim list_G2G_Recode_Pratiche_Delete As New List(Of G2G_Recode_Pratiche)
                    For Each r As G2G_Recode_Pratiche In g2g.G2G_Pratiche_Recode_delete

                        'cancella pratica
                        Dim pratica = (From m In GiasContext.Pratiche
                                       Where m.Piva_SuperUser = Destinazione_Piva_SuperUser _
                                           AndAlso m.Pratica_Cod = r.To_Pratica_Cod _
                                           AndAlso m.Piva = piva).FirstOrDefault()

                        If pratica IsNot Nothing Then

                            GiasContext.Pratiche.Attach(pratica)
                            GiasContext.Pratiche.Remove(pratica)

                            ' cancella Pratiche_Stati_Attuali
                            Dim pratica_stato_attuale = (From psa In GiasContext.Pratiche_Stati_Attuali Where psa.Pratica_Cod = pratica.Pratica_Cod).FirstOrDefault

                            If pratica_stato_attuale IsNot Nothing Then
                                GiasContext.Pratiche_Stati_Attuali.Attach(pratica_stato_attuale)
                                GiasContext.Pratiche_Stati_Attuali.Remove(pratica_stato_attuale)
                            End If

                            ' cancella G2G_Recode_Pratiche
                            Dim recode = (From rr In GiasContext.G2G_Recode_Pratiche
                                          Where rr.From_PivaSuperUser = Origine_Piva_SuperUser _
                                                  AndAlso rr.From_Pratica_Cod = r.From_Pratica_Cod).FirstOrDefault()

                            If recode IsNot Nothing Then
                                GiasContext.G2G_Recode_Pratiche.Attach(recode)
                                GiasContext.G2G_Recode_Pratiche.Remove(recode)

                                list_G2G_Recode_Pratiche_Delete.Add(recode)
                                'g2g.G2G_Pratiche_Recode_delete.Add(recode)
                            End If


                            GiasContext.SaveChanges()
                        End If

                    Next
                    g2g.G2G_Pratiche_Recode_delete = list_G2G_Recode_Pratiche_Delete
                    g2g.Recode.G2GRecodePraticheToDelete = list_G2G_Recode_Pratiche_Delete
                    ' COMIT Effettivo
                    scope.Complete()

                End Using
            End Using

        Catch ex As Exception

            messaggioErrore = ex.Message
            g2g.Recode.MessaggioErrore = messaggioErrore
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            'Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore, ex)

        End Try

        Return g2g.Recode

    End Function

    Public Function Scrivi_Pratiche_G2GReverse(
        ByVal LetturaCodificheInOrigine As Boolean,
        Origine_Piva_SuperUser As String,
        Destinazione_Piva_SuperUser As String,
        ByRef g2g As G2G_Pratiche_Reverse,
        ByRef objParametri As AgronicaCoreParametri
    ) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GPratiche_W.Scrivi_Pratiche_G2GReverse()"
        Dim messaggioErrore As String = ""

        Dim username = objParametri.UsernameOperazione
        Dim piva = g2g.To_Piva
        Dim data = Date.Now

        Dim objSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Dim transactionOptions As New TransactionOptions()
        transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
        transactionOptions.Timeout = G2GUtility.GetTransactionTimeout(objParametri)

        g2g.Recode.G2GRecodePraticheToInsert = New List(Of G2G_Recode_Pratiche)
        g2g.Recode.G2GRecodePraticheToUpdate = New List(Of G2G_Recode_Pratiche)
        g2g.Recode.G2GRecodePraticheToDelete = New List(Of G2G_Recode_Pratiche)
        g2g.Recode.G2GRecodePraticheStatiToInsert = New List(Of G2G_Recode_Pratiche_Stati)
        g2g.Recode.G2GRecodePraticheStatiToUpdate = New List(Of G2G_Recode_Pratiche_Stati)
        g2g.Recode.G2GRecodePraticheStatiToDelete = New List(Of G2G_Recode_Pratiche_Stati)

        Try

            Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    Dim idSeq As Integer = 0

                    For Each m As Pratiche In g2g.pratiche_insert

                        'Richiedo un nuovo id sequenza
                        idSeq = objSequenze.NuovoId_Tabella_EF(GiasContext, "Pratiche", 0, 2000000000, objParametri)

                        Dim pratiche = Gias_EF_Utility.CopyEntity(GiasContext, m, Nothing, username, data)
                        pratiche.Piva = piva
                        pratiche.Pratica_Cod = idSeq
                        pratiche.Piva_SuperUser = Destinazione_Piva_SuperUser
                        GiasContext.Pratiche.Add(pratiche)

                        Dim recode =
                            New G2G_Recode_Pratiche With {
                                .From_PivaSuperUser = Destinazione_Piva_SuperUser,
                                .To_PivaSuperUser = Origine_Piva_SuperUser,
                                .From_Pratica_Cod = idSeq,
                                .To_Pratica_Cod = m.Pratica_Cod,
                                .Username_Creazione = username,
                                .Username_Modifica = username,
                                .Data_Creazione = Now(),
                                .Data_Modifica = Now(),
                                .Validita_Inizio = AGRODATAINIZIO,
                                .Validita_Fine = AGRODATAFINE,
                                .inviato = "0",
                                .datainvio = Now()
                            }
                        g2g.Recode.G2GRecodePraticheToInsert.Add(recode)
                        GiasContext.G2G_Recode_Pratiche.Add(recode)

                        GiasContext.SaveChanges()

                    Next

                    For Each s As Pratiche_Stati In g2g.pratiche_stati_insert


                        'Richiedo un nuovo id sequenza
                        idSeq = objSequenze.NuovoId_Tabella_EF(GiasContext, "passaggiodistato_cod", 0, 2000000000, objParametri)
                        Dim recodePratica As G2G_Recode_Pratiche

                        'se la lettura avviene con i codici origine allora l'oggetto pratiche_Stati contiene codici "origine", 
                        'altrimenti contiene codici che provengono dalla "destinazione"..
                        'le p.iva super-user vengono invece chiamate già correttamente come parametri della funzione.
                        If LetturaCodificheInOrigine Then
                            recodePratica = (From rr In GiasContext.G2G_Recode_Pratiche Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_Pratica_Cod = s.Pratica_Cod).FirstOrDefault()
                        Else
                            recodePratica = (From rr In GiasContext.G2G_Recode_Pratiche Where rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_Pratica_Cod = s.Pratica_Cod).FirstOrDefault()
                        End If

                        Dim praticheStati = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)

                        praticheStati.Piva_SuperUser = Destinazione_Piva_SuperUser

                        If LetturaCodificheInOrigine Then
                            praticheStati.Pratica_Cod = recodePratica.From_Pratica_Cod
                        Else
                            praticheStati.Pratica_Cod = recodePratica.To_Pratica_Cod
                        End If

                        praticheStati.PassaggioDiStato_cod = idSeq

                        GiasContext.Pratiche_Stati.Add(praticheStati)

                        Dim fromPassaggioDiStatoCod As Integer = s.PassaggioDiStato_cod
                        Dim toPassaggioDiStatoCod As Integer = idSeq

                        Dim recode =
                            New G2G_Recode_Pratiche_Stati With {
                                .From_PivaSuperUser = Destinazione_Piva_SuperUser,
                                .To_PivaSuperUser = Origine_Piva_SuperUser,
                                .From_PassaggioDiStato_Cod = toPassaggioDiStatoCod,
                                .To_PassaggioDiStato_Cod = fromPassaggioDiStatoCod,
                                .Username_Creazione = username,
                                .Username_Modifica = username,
                                .Data_Creazione = Now(),
                                .Data_Modifica = Now(),
                                .Validita_Inizio = AGRODATAINIZIO,
                                .Validita_Fine = AGRODATAFINE,
                                .inviato = "0",
                                .datainvio = Now()
                            }
                        g2g.Recode.G2GRecodePraticheStatiToInsert.Add(recode)
                        GiasContext.G2G_Recode_Pratiche_Stati.Add(recode)

                        GiasContext.SaveChanges()

                    Next

                    For Each s As Pratiche_Stati_Attuali In g2g.pratiche_stati_attuali_insert

                        'Richiedo un nuovo id sequenza                                    
                        Dim recodePratica = (From rr In GiasContext.G2G_Recode_Pratiche Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_Pratica_Cod = s.Pratica_Cod).FirstOrDefault()
                        Dim praticheStatoAttuale = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)
                        praticheStatoAttuale.Pratica_Cod = recodePratica.From_Pratica_Cod
                        praticheStatoAttuale.Piva_SuperUser = Destinazione_Piva_SuperUser
                        GiasContext.Pratiche_Stati_Attuali.Add(praticheStatoAttuale)
                        GiasContext.SaveChanges()

                    Next

                    For Each m As Pratiche In g2g.pratiche_update

                        Dim recode = (From rr In GiasContext.G2G_Recode_Pratiche Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_Pratica_Cod = m.Pratica_Cod).FirstOrDefault()
                        recode.Username_Modifica = username
                        recode.Data_Modifica = data
                        recode.datainvio = data
                        g2g.Recode.G2GRecodePraticheToUpdate.Add(recode)
                        GiasContext.G2G_Recode_Pratiche.Attach(recode)
                        GiasContext.Entry(recode).State = EntityState.Modified

                        Dim pratiche = (From mm In GiasContext.Pratiche Where mm.Pratica_Cod = recode.From_Pratica_Cod).FirstOrDefault()
                        pratiche = Gias_EF_Utility.CopyEntity(GiasContext, m, pratiche, username, data)
                        pratiche.Piva_SuperUser = Destinazione_Piva_SuperUser
                        pratiche.Piva = piva
                        pratiche.Pratica_Cod = recode.From_Pratica_Cod
                        GiasContext.Pratiche.Attach(pratiche)
                        GiasContext.Entry(pratiche).State = EntityState.Modified

                        GiasContext.SaveChanges()

                    Next

                    For Each m As Pratiche_Stati In g2g.pratiche_stati_update


                        Dim recode = (From rr In GiasContext.G2G_Recode_Pratiche_Stati Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PassaggioDiStato_Cod = m.PassaggioDiStato_cod).FirstOrDefault()
                        recode.Username_Modifica = username
                        recode.Data_Modifica = data
                        recode.datainvio = data
                        g2g.Recode.G2GRecodePraticheStatiToUpdate.Add(recode)
                        GiasContext.G2G_Recode_Pratiche_Stati.Attach(recode)
                        GiasContext.Entry(recode).State = EntityState.Modified

                        Dim praticheStati = (From mm In GiasContext.Pratiche_Stati Where mm.PassaggioDiStato_cod = recode.From_PassaggioDiStato_Cod).FirstOrDefault()
                        Dim lPraticaCod As Integer = praticheStati.Pratica_Cod
                        Dim lpassaggioDiStatoCod As Integer = praticheStati.PassaggioDiStato_cod
                        praticheStati = Gias_EF_Utility.CopyEntity(GiasContext, m, praticheStati, username, data)
                        praticheStati.Piva_SuperUser = Destinazione_Piva_SuperUser
                        praticheStati.PassaggioDiStato_cod = lpassaggioDiStatoCod
                        praticheStati.Pratica_Cod = lPraticaCod

                        GiasContext.Pratiche_Stati.Attach(praticheStati)
                        GiasContext.Entry(praticheStati).State = EntityState.Modified
                        GiasContext.SaveChanges()

                    Next


                    For Each m As Pratiche_Stati_Attuali In g2g.pratiche_stati_attuali_update

                        'leggo in virtù del parametro (piva super user è ok, poichè passata "invertita")
                        Dim recode As G2G_Recode_Pratiche
                        If LetturaCodificheInOrigine Then
                            'sono sulla destinazione, lo scopo è leggere un recode destinazione partendo da codici origine
                            recode = (From rr In GiasContext.G2G_Recode_Pratiche Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_Pratica_Cod = m.Pratica_Cod).FirstOrDefault()
                        Else
                            'sono in origine, lo scopo è leggere un recode di origine partendo da destinazione
                            recode = (From rr In GiasContext.G2G_Recode_Pratiche Where rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_Pratica_Cod = m.Pratica_Cod).FirstOrDefault()
                        End If

                        If recode Is Nothing Then
                            Throw New Exception("Errore, non esiste la decodifica di Pratica_Cod = " & m.Pratica_Cod & " in G2G_Recode_Pratiche")
                        End If

                        Dim praticheStatiAttuali As Pratiche_Stati_Attuali
                        If LetturaCodificheInOrigine Then
                            'sono in destinazione, lo scopo è leggere lo stato attuale partendo dalle chiavi destinazione.
                            praticheStatiAttuali = (From mm In GiasContext.Pratiche_Stati_Attuali Where mm.Piva_SuperUser = recode.From_PivaSuperUser AndAlso mm.Pratica_Cod = recode.From_Pratica_Cod).FirstOrDefault()
                        Else
                            'sono in origine, lo scopo è leggere lo stato attuale partendo dalle chiavi origine
                            praticheStatiAttuali = (From mm In GiasContext.Pratiche_Stati_Attuali Where mm.Piva_SuperUser = recode.To_PivaSuperUser AndAlso mm.Pratica_Cod = recode.To_Pratica_Cod).FirstOrDefault()
                        End If

                        If praticheStatiAttuali Is Nothing Then
                            Throw New Exception("Errore, non esiste riferimento a Pratica_Cod = " & If(LetturaCodificheInOrigine, recode.To_Pratica_Cod, recode.From_Pratica_Cod) & " in Pratiche_Stati_Attuali")
                        End If

                        'cancella e riscrive per ovviare al fatto stat_cod che si trova in chiave primaria
                        GiasContext.Pratiche_Stati_Attuali.Remove(praticheStatiAttuali)
                        GiasContext.SaveChanges()

                        Dim lPraticaCod As Integer = praticheStatiAttuali.Pratica_Cod
                        Dim lPraticaPivaSuperUser As String = praticheStatiAttuali.Piva_SuperUser
                        praticheStatiAttuali = Gias_EF_Utility.CopyEntity(GiasContext, m, praticheStatiAttuali, username, data)
                        praticheStatiAttuali.Piva_SuperUser = lPraticaPivaSuperUser
                        praticheStatiAttuali.Pratica_Cod = lPraticaCod

                        GiasContext.Pratiche_Stati_Attuali.Add(praticheStatiAttuali)
                        GiasContext.SaveChanges()

                    Next

                    'cancellazioni, da gestire ... 
                    Dim list_G2G_Recode_Pratiche_Stati_Delete As New List(Of G2G_Recode_Pratiche_Stati)

                    If g2g.G2G_Pratiche_Stati_Recode_delete IsNot Nothing AndAlso g2g.G2G_Pratiche_Stati_Recode_delete.Count > 0 Then

                        Dim list_From_PassaggioDiStato_Cod As List(Of Integer) = (From el In g2g.G2G_Pratiche_Stati_Recode_delete Select el.From_PassaggioDiStato_Cod).ToList
                        Dim stati As List(Of Pratiche_Stati)

                        Dim pratiche_stati_impresa As List(Of Pratiche_Stati) = (From s In GiasContext.Pratiche_Stati
                                    Join p In GiasContext.Pratiche On s.Pratica_Cod Equals p.Pratica_Cod
                                    Where p.Piva_SuperUser = Origine_Piva_SuperUser AndAlso
                                          p.Piva = piva
                                    Select s).ToList

                        stati = pratiche_stati_impresa.FindAll(Function(elem) list_From_PassaggioDiStato_Cod.Contains(elem.PassaggioDiStato_cod))

                        For Each stato As Pratiche_Stati In stati

                            'cancella stato
                            'Dim stato = (From s In GiasContext.Pratiche_Stati
                            '             Join p In GiasContext.Pratiche On s.Pratica_Cod Equals p.Pratica_Cod
                            '             Where p.Piva_SuperUser = Origine_Piva_SuperUser AndAlso
                            '                   p.Piva = piva AndAlso
                            '                   s.PassaggioDiStato_cod = r.From_PassaggioDiStato_Cod
                            '             Select s).FirstOrDefault()

                            'If stato IsNot Nothing Then

                            GiasContext.Pratiche_Stati.Attach(stato)
                                GiasContext.Pratiche_Stati.Remove(stato)


                            ' cancella recode stato
                            Dim recode = (From rr In GiasContext.G2G_Recode_Pratiche_Stati
                                          Where rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso
                                                rr.From_PassaggioDiStato_Cod = stato.PassaggioDiStato_cod).FirstOrDefault()

                            If recode IsNot Nothing Then

                                    GiasContext.G2G_Recode_Pratiche_Stati.Attach(recode)
                                    GiasContext.G2G_Recode_Pratiche_Stati.Remove(recode)

                                    'g2g.G2G_Pratiche_Stati_Recode_delete.Add(recode)
                                    list_G2G_Recode_Pratiche_Stati_Delete.Add(recode)
                                End If

                                GiasContext.SaveChanges()
                            'End If

                        Next

                    End If

                    'g2g.G2G_Pratiche_Stati_Recode_delete = list_G2G_Recode_Pratiche_Stati_Delete
                    g2g.Recode.G2GRecodePraticheStatiToDelete = list_G2G_Recode_Pratiche_Stati_Delete
                    Dim list_G2G_Recode_Pratiche_Delete As New List(Of G2G_Recode_Pratiche)

                    If g2g.G2G_Pratiche_Recode_delete IsNot Nothing AndAlso g2g.G2G_Pratiche_Recode_delete.Count > 0 Then

                        Dim list_From_Pratica_Cod As List(Of Integer) = (From el In g2g.G2G_Pratiche_Recode_delete Select el.From_Pratica_Cod).ToList

                        Dim pratiche As List(Of Pratiche) = (From m In GiasContext.Pratiche
                                                             Where m.Piva_SuperUser = Origine_Piva_SuperUser _
                                                               AndAlso list_From_Pratica_Cod.Contains(m.Pratica_Cod) _
                                                               AndAlso m.Piva = piva).ToList()

                        For Each pratica As Pratiche In pratiche

                            ''cancella pratica
                            'Dim pratica = (From m In GiasContext.Pratiche
                            '               Where m.Piva_SuperUser = Origine_Piva_SuperUser _
                            '                   AndAlso m.Pratica_Cod = r.From_Pratica_Cod _
                            '                   AndAlso m.Piva = piva).FirstOrDefault()

                            'If pratica IsNot Nothing Then

                            GiasContext.Pratiche.Attach(pratica)
                            GiasContext.Pratiche.Remove(pratica)

                            ' cancella Pratiche_Stati_Attuali
                            Dim pratica_stato_attuale = (From psa In GiasContext.Pratiche_Stati_Attuali Where psa.Pratica_Cod = pratica.Pratica_Cod).FirstOrDefault

                            If pratica_stato_attuale IsNot Nothing Then
                                GiasContext.Pratiche_Stati_Attuali.Attach(pratica_stato_attuale)
                                GiasContext.Pratiche_Stati_Attuali.Remove(pratica_stato_attuale)
                            End If

                            ' cancella G2G_Recode_Pratiche
                            Dim recode = (From rr In GiasContext.G2G_Recode_Pratiche
                                          Where rr.To_PivaSuperUser = Origine_Piva_SuperUser _
                                                  AndAlso rr.From_Pratica_Cod = pratica.Pratica_Cod).FirstOrDefault()

                            If recode IsNot Nothing Then
                                GiasContext.G2G_Recode_Pratiche.Attach(recode)
                                GiasContext.G2G_Recode_Pratiche.Remove(recode)

                                list_G2G_Recode_Pratiche_Delete.Add(recode)
                                'g2g.G2G_Pratiche_Recode_delete.Add(recode)
                            End If


                            GiasContext.SaveChanges()
                            'End If

                        Next

                    End If

                    'g2g.G2G_Pratiche_Recode_delete = list_G2G_Recode_Pratiche_Delete
                    g2g.Recode.G2GRecodePraticheToDelete = list_G2G_Recode_Pratiche_Delete
                    ' COMIT Effettivo
                    scope.Complete()

                End Using
            End Using

        Catch ex As Exception

            messaggioErrore = ex.Message
            g2g.Recode.MessaggioErrore = messaggioErrore
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            'Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore, ex)

        End Try

        Return g2g.Recode

    End Function

End Class
