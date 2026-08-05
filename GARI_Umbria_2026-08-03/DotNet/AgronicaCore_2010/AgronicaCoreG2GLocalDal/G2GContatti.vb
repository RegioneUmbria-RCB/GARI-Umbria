Imports System.Transactions
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModello
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports System.Data.Entity.Core.Objects
Imports System.Data.Entity

Public Class G2GContatti_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Nuovo_Contatti_G2G(ByVal From_PivaSuperUser As String, ByVal To_PivaSuperUser As String, ByVal From_Piva As String, ByVal To_Piva As String) As G2G_Contatti

        Dim g2g As New G2G_Contatti
        With g2g
            .From_PivaSuperUser = From_PivaSuperUser
            .To_PivaSuperUser = To_PivaSuperUser
            .From_Piva = From_Piva
            .To_Piva = To_Piva
            .ContattiToInsert = New List(Of G2G_Contatto)
            .ContattiToUpdate = New List(Of G2G_Contatto)
            .ContattiToDelete = New List(Of G2G_Contatto)
            .Recode = G2GUtility.Nuovo_G2G_Recode()
            .GestioneAllegati = False
        End With

        Return g2g

    End Function

    Public Function Nuovo_Contatti_G2GReverse(ByVal From_PivaSuperUser As String, ByVal To_PivaSuperUser As String, ByVal From_Piva As String, ByVal To_Piva As String) As G2G_Contatti_Reverse

        Dim g2g As New G2G_Contatti_Reverse
        With g2g
            .From_PivaSuperUser = From_PivaSuperUser
            .To_PivaSuperUser = To_PivaSuperUser
            .From_Piva = From_Piva
            .To_Piva = To_Piva
            .ContattiToInsert = New List(Of G2G_Contatto)
            .ContattiToUpdate = New List(Of G2G_Contatto)
            .ContattiToDelete = New List(Of G2G_Contatto)
            .Recode = G2GUtility.Nuovo_G2G_Recode()
            .GestioneAllegati = False
        End With

        Return g2g

    End Function

    Public Function Leggi_Contatti_Allegati_G2G(ByRef GiasContext As Gias_DeveloperServer_Entities, ByVal contatto As Contatti) As G2G_Contatto_Allegati

        Dim allegati As New G2G_Contatto_Allegati
        GiasContext.Database.CommandTimeout = 3600
        allegati.Entita = (From e In GiasContext.Alert_Entita 
                            Where e.Piva = contatto.Piva AndAlso
                                  e.Cod_Contatto = contatto.Cod_Contatto Select e).ToList()

        allegati.Documenti = (From e In GiasContext.Alert_Entita
                              Join d In GiasContext.Allegati_Documenti On e.PivaSuperUser Equals d.Allegati_Documenti_SuperUser And e.Piva Equals d.Allegati_Documenti_Piva And e.Allegati_Documenti_Cod Equals d.Allegati_Documenti_Cod
                              Where e.Piva = contatto.Piva AndAlso
                                    e.Cod_Contatto = contatto.Cod_Contatto Select d).ToList()

        allegati.Scadenze = (From e In GiasContext.Alert_Entita
                             Join s In GiasContext.Alert_Elenco On e.PivaSuperUser Equals s.PivaSuperUser And e.ID_Alert_Entita Equals s.ID_Alert_Entita
                             Where e.Piva = contatto.Piva AndAlso
                                   e.Cod_Contatto = contatto.Cod_Contatto Select s).ToList()

        Return allegati

    End Function

    Public Function Leggi_Contatti_G2G(ByVal Piva As String, ByVal Flag_Pubblico As Boolean, ByVal listaImprese As List(Of String), ByRef listaImpreseIndirizzi As List(Of String), ByVal listaCodRapporto As List(Of Integer), ByVal dataValidita As Date, ByRef g2g As G2G_Contatti, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GContatti_R.Leggi_Contatti_G2G()"
        Dim From_PivaSuperUser = g2g.From_PivaSuperUser
        Dim To_PivaSuperUser = g2g.To_PivaSuperUser

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
            GiasContext.Database.CommandTimeout = 3600
            ' Valutare se aggiungere nella join tra contatti e risorse umane and il sa_cod (r.Sa_Cod Equals c.Sa_Cod)
            Dim listaContatti As New List(Of Integer?)
            Dim listaCausali As New List(Of String)({"6800", "6850", "6851", "8100"})

            If dataValidita > AGRODATAINIZIO Then

                ' contatti movimentati
                listaContatti = (
                    From d In GiasContext.Movimenti_dettagli
                    Join m In GiasContext.Movimenti On d.Id_Agenda Equals m.Id_Agenda And d.PIVA Equals m.PIVA And d.Id_Mov Equals m.Id_Mov
                    Where listaCausali.Contains(m.Cau_Mov) AndAlso d.Elem_Cod = 0 AndAlso d.Mat_Cod <> 0 AndAlso
                        m.Data_Movimento >= dataValidita AndAlso (listaImprese.Count = 0 OrElse listaImprese.Contains(m.PIVA))
                    Select d.Mat_Cod).Distinct().ToList()

                ' contatti imputati diversamente
                Dim listaContattiAltro = (
                    From m In GiasContext.Movimenti
                    Where (m.Cod_RisUm <> 0 OrElse m.Cod_Destinazione <> 0 OrElse m.Cod_Vettore <> 0 OrElse m.Cod_RisUm_Aggiuntivo <> 0 OrElse m.Cod_RisUm_Altro) AndAlso
                        m.Data_Movimento >= dataValidita AndAlso (listaImprese.Count = 0 OrElse listaImprese.Contains(m.PIVA))
                    Select m).ToList()

                For Each c In listaContattiAltro
                    Dim cod_risum As Integer = 0
                    If c.Cod_RisUm <> 0 Then
                        cod_risum = c.Cod_RisUm
                    ElseIf c.Cod_Destinazione <> 0 Then
                        cod_risum = c.Cod_Destinazione
                    ElseIf c.Cod_Vettore <> 0 Then
                        cod_risum = c.Cod_Vettore
                    ElseIf c.Cod_RisUm_Aggiuntivo <> 0 Then
                        cod_risum = c.Cod_RisUm_Aggiuntivo
                    ElseIf c.Cod_RisUm_Altro <> 0 Then
                        cod_risum = c.Cod_RisUm_Altro
                    End If
                    If cod_risum <> 0 AndAlso Not listaContatti.Contains(cod_risum) Then
                        listaContatti.Add(cod_risum)
                    End If
                Next

                ' lista imprese indirizzi
                If Flag_Pubblico Then
                    listaImpreseIndirizzi = (
                        From m In GiasContext.Movimenti
                        Join i In GiasContext.ImpresexIndirizzi On m.Cod_IndirizzoRisUm Equals i.cod_indirizzo
                        Where m.Cod_IndirizzoRisUm <> 0 AndAlso m.Data_Movimento >= dataValidita AndAlso (listaImprese.Count = 0 OrElse listaImprese.Contains(m.PIVA))
                        Select (i.PIVA)).Distinct().ToList()
                End If

            End If

            Dim listaRecodeContatti = (
                From g In GiasContext.G2G_Recode_Contatti
                Join r In GiasContext.Risorse_Umane On g.From_Cod_RisUm Equals r.Cod_RisUm
                Join c In GiasContext.Contatti On c.Piva Equals r.Piva And c.Cod_Contatto Equals r.Cod_Contatto
                Where g.From_PivaSuperUser = From_PivaSuperUser AndAlso g.To_PivaSuperUser = To_PivaSuperUser AndAlso
                    If(Flag_Pubblico, c.Sa_Cod = -1, c.Sa_Cod <> -1 AndAlso c.Piva = Piva)
                Select c.Piva & "_" & c.Cod_Contatto).Distinct().ToList()

            Dim listaContattiInsert = (
                From c In GiasContext.Contatti
                Where If(Flag_Pubblico, c.Sa_Cod = -1, c.Sa_Cod <> -1 AndAlso c.Piva = Piva) AndAlso Not listaRecodeContatti.Contains(c.Piva & "_" & c.Cod_Contatto)
                Select c).ToList()

            For Each contatto In listaContattiInsert

                Dim risorse = (
                    From r In GiasContext.Risorse_Umane Where r.Piva = contatto.Piva AndAlso r.Cod_Contatto = contatto.Cod_Contatto AndAlso
                        (listaCodRapporto.Count = 0 OrElse listaCodRapporto.Contains(r.Cod_Rapporto)) AndAlso
                        (dataValidita = AGRODATAINIZIO OrElse listaContatti.Contains(r.Cod_RisUm))
                    Select r).ToList()

                If risorse.Count > 0 Then

                    Dim listaRisorse = (From r In risorse Select r.Cod_RisUm).ToList()
                    Dim codici = (From c In GiasContext.Contatti_Codici Where c.PIVA = contatto.Piva AndAlso c.Cod_Contatto = contatto.Cod_Contatto Select c).ToList()
                    Dim indirizzi = (From ci In GiasContext.ContattiXIndirizzi Join i In GiasContext.Indirizzi On ci.Cod_Indirizzo Equals i.cod_indirizzo Where ci.Piva = contatto.Piva AndAlso ci.Cod_Contatto = contatto.Cod_Contatto Select New G2G_Indirizzo With {.Indirizzo = i, .Tipo_Indirizzo = ci.Tipo_Indirizzo}).ToList()
                    Dim rubrica = (From cr In GiasContext.ContattiXRubrica Join r In GiasContext.Rubrica On cr.Cod_Rubrica Equals r.cod_rubrica Where cr.Piva = contatto.Piva AndAlso cr.Cod_Contatto = contatto.Cod_Contatto Select r).ToList()
                    Dim costi = (From c In GiasContext.Prodotti_Costi Where c.Piva = contatto.Piva AndAlso listaRisorse.Contains(c.Mat_Cod) AndAlso c.Elem_Cod = 0 AndAlso c.Id_Budget = 0 Select c).ToList()
                    Dim allegati = If(g2g.GestioneAllegati, Leggi_Contatti_Allegati_G2G(GiasContext, contatto), Nothing)

                    ' forzo il centro aziendale contatto con quello destinazione
                    If Not Flag_Pubblico AndAlso contatto.Sa_Cod <> 0 Then
                        Dim recode = (
                            From rr In GiasContext.G2G_Recode_Imprese
                            Where rr.From_PivaSuperUser = From_PivaSuperUser AndAlso rr.To_PivaSuperUser = To_PivaSuperUser AndAlso
                            rr.FROM_Piva = contatto.Piva AndAlso rr.FROM_SaCod = contatto.Sa_Cod).FirstOrDefault()
                        contatto.Sa_Cod = If(recode Is Nothing, 0, recode.TO_SaCod)
                    End If

                    g2g.ContattiToInsert.Add(New G2G_Contatto With {.Contatto = contatto, .RisorseUmane = risorse, .Codici = codici, .Indirizzi = indirizzi, .Rubrica = rubrica, .Costi = costi, .Allegati = allegati})

                End If
            Next

            Dim listaContattiUpdate = (
                From g In GiasContext.G2G_Recode_Contatti
                Join r In GiasContext.Risorse_Umane On g.From_Cod_RisUm Equals r.Cod_RisUm
                Join c In GiasContext.Contatti On c.Piva Equals r.Piva And c.Cod_Contatto Equals r.Cod_Contatto
                Where g.From_PivaSuperUser = From_PivaSuperUser AndAlso g.To_PivaSuperUser = To_PivaSuperUser AndAlso
                    If(Flag_Pubblico, c.Sa_Cod = -1, c.Sa_Cod <> -1 AndAlso c.Piva = Piva) AndAlso g.datainvio < c.Data_Modifica
                Select c).Distinct().ToList()

            For Each contatto In listaContattiUpdate

                Dim risorse = (
                    From r In GiasContext.Risorse_Umane Where r.Piva = contatto.Piva AndAlso r.Cod_Contatto = contatto.Cod_Contatto AndAlso
                        (listaCodRapporto.Count = 0 OrElse listaCodRapporto.Contains(r.Cod_Rapporto)) AndAlso
                        (dataValidita = AGRODATAINIZIO OrElse listaContatti.Contains(r.Cod_RisUm))
                    Select r).ToList()

                If risorse.Count > 0 Then

                    Dim listaRisorse = (From r In risorse Select r.Cod_RisUm).ToList()
                    Dim codici = (From c In GiasContext.Contatti_Codici Where c.PIVA = contatto.Piva AndAlso c.Cod_Contatto = contatto.Cod_Contatto Select c).ToList()
                    Dim indirizzi = (From ci In GiasContext.ContattiXIndirizzi Join i In GiasContext.Indirizzi On ci.Cod_Indirizzo Equals i.cod_indirizzo Where ci.Piva = contatto.Piva AndAlso ci.Cod_Contatto = contatto.Cod_Contatto Select New G2G_Indirizzo With {.Indirizzo = i, .Tipo_Indirizzo = ci.Tipo_Indirizzo}).ToList()
                    Dim rubrica = (From cr In GiasContext.ContattiXRubrica Join r In GiasContext.Rubrica On cr.Cod_Rubrica Equals r.cod_rubrica Where cr.Piva = contatto.Piva AndAlso cr.Cod_Contatto = contatto.Cod_Contatto Select r).ToList()
                    Dim costi = (From c In GiasContext.Prodotti_Costi Where c.Piva = contatto.Piva AndAlso listaRisorse.Contains(c.Mat_Cod) AndAlso c.Elem_Cod = 0 AndAlso c.Id_Budget = 0 Select c).ToList()
                    Dim allegati = If(g2g.GestioneAllegati, Leggi_Contatti_Allegati_G2G(GiasContext, contatto), Nothing)

                    ' forzo il centro aziendale contatto con quello destinazione
                    If Not Flag_Pubblico AndAlso contatto.Sa_Cod <> 0 Then
                        Dim recode = (
                            From rr In GiasContext.G2G_Recode_Imprese
                            Where rr.From_PivaSuperUser = From_PivaSuperUser AndAlso
                                rr.To_PivaSuperUser = To_PivaSuperUser AndAlso
                                rr.FROM_Piva = contatto.Piva AndAlso
                                rr.FROM_SaCod = contatto.Sa_Cod).FirstOrDefault()
                        contatto.Sa_Cod = If(recode Is Nothing, 0, recode.TO_SaCod)
                    End If

                    g2g.ContattiToUpdate.Add(New G2G_Contatto With {.Contatto = contatto, .RisorseUmane = risorse, .Codici = codici, .Indirizzi = indirizzi, .Rubrica = rubrica, .Costi = costi, .Allegati = allegati})

                End If

            Next

            If Not Flag_Pubblico Then
                g2g.Recode.G2GRecodeContattiToDelete = (
                    From g In GiasContext.G2G_Recode_Contatti
                    Where g.From_PivaSuperUser = From_PivaSuperUser AndAlso g.To_PivaSuperUser = To_PivaSuperUser AndAlso
                        Not GiasContext.Risorse_Umane.Any(Function(r) r.Cod_RisUm = g.From_Cod_RisUm)
                    Select g).ToList()
            End If

        End Using

        Return g2g.ContattiToInsert.Count > 0 OrElse g2g.ContattiToUpdate.Count > 0 OrElse g2g.Recode.G2GRecodeContattiToDelete.Count > 0

    End Function


    Public Function Leggi_Contatti_G2GReverse(ByVal Piva As String, ByVal Flag_Pubblico As Boolean, ByVal listaImprese As List(Of String), ByRef listaImpreseIndirizzi As List(Of String), ByVal listaCodRapporto As List(Of Integer), ByVal dataValidita As Date, ByRef g2g As G2G_Contatti_Reverse, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GContatti_R.Leggi_Contatti_G2GReverse()"
        Dim From_PivaSuperUser = g2g.From_PivaSuperUser
        Dim To_PivaSuperUser = g2g.To_PivaSuperUser

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
            GiasContext.DataBase.CommandTimeout = 3600
            ' Valutare se aggiungere nella join tra contatti e risorse umane and il sa_cod (r.Sa_Cod Equals c.Sa_Cod)
            Dim listaContatti As New List(Of Integer?)
            Dim listaCausali As New List(Of String)({"6800", "6850", "6851", "8100"})

            If dataValidita > AGRODATAINIZIO Then

                ' contatti movimentati
                listaContatti = (
                    From d In GiasContext.Movimenti_dettagli
                    Join m In GiasContext.Movimenti On d.Id_Agenda Equals m.Id_Agenda And d.PIVA Equals m.PIVA And d.Id_Mov Equals m.Id_Mov
                    Where listaCausali.Contains(m.Cau_Mov) AndAlso d.Elem_Cod = 0 AndAlso d.Mat_Cod <> 0 AndAlso
                        m.Data_Movimento >= dataValidita AndAlso (listaImprese.Count = 0 OrElse listaImprese.Contains(m.PIVA))
                    Select d.Mat_Cod).Distinct().ToList()

                ' contatti imputati diversamente
                Dim listaContattiAltro = (
                    From m In GiasContext.Movimenti
                    Where (m.Cod_RisUm <> 0 OrElse m.Cod_Destinazione <> 0 OrElse m.Cod_Vettore <> 0 OrElse m.Cod_RisUm_Aggiuntivo <> 0 OrElse m.Cod_RisUm_Altro) AndAlso
                        m.Data_Movimento >= dataValidita AndAlso (listaImprese.Count = 0 OrElse listaImprese.Contains(m.PIVA))
                    Select m).ToList()

                For Each c In listaContattiAltro
                    Dim cod_risum As Integer = 0
                    If c.Cod_RisUm <> 0 Then
                        cod_risum = c.Cod_RisUm
                    ElseIf c.Cod_Destinazione <> 0 Then
                        cod_risum = c.Cod_Destinazione
                    ElseIf c.Cod_Vettore <> 0 Then
                        cod_risum = c.Cod_Vettore
                    ElseIf c.Cod_RisUm_Aggiuntivo <> 0 Then
                        cod_risum = c.Cod_RisUm_Aggiuntivo
                    ElseIf c.Cod_RisUm_Altro <> 0 Then
                        cod_risum = c.Cod_RisUm_Altro
                    End If
                    If cod_risum <> 0 AndAlso Not listaContatti.Contains(cod_risum) Then
                        listaContatti.Add(cod_risum)
                    End If
                Next

                ' lista imprese indirizzi
                If Flag_Pubblico Then
                    listaImpreseIndirizzi = (
                        From m In GiasContext.Movimenti
                        Join i In GiasContext.ImpresexIndirizzi On m.Cod_IndirizzoRisUm Equals i.cod_indirizzo
                        Where m.Cod_IndirizzoRisUm <> 0 AndAlso m.Data_Movimento >= dataValidita AndAlso (listaImprese.Count = 0 OrElse listaImprese.Contains(m.PIVA))
                        Select (i.PIVA)).Distinct().ToList()
                End If

            End If

            Dim listaRecodeContatti = (
                From g In GiasContext.G2G_Recode_Contatti
                Join r In GiasContext.Risorse_Umane On g.To_Cod_Risum Equals r.Cod_RisUm
                Join c In GiasContext.Contatti On c.Piva Equals r.Piva And c.Cod_Contatto Equals r.Cod_Contatto
                Where g.To_PivaSuperUser = From_PivaSuperUser AndAlso g.From_PivaSuperUser = To_PivaSuperUser AndAlso
                    If(Flag_Pubblico, c.Sa_Cod = -1, c.Sa_Cod <> -1 AndAlso c.Piva = Piva)
                Select c.Piva & "_" & c.Cod_Contatto).Distinct().ToList()

            Dim listaContattiInsert = (
                From c In GiasContext.Contatti
                Where If(Flag_Pubblico, c.Sa_Cod = -1, c.Sa_Cod <> -1 AndAlso c.Piva = Piva) AndAlso Not listaRecodeContatti.Contains(c.Piva & "_" & c.Cod_Contatto)
                Select c).ToList()

            For Each contatto In listaContattiInsert

                Dim risorse = (
                    From r In GiasContext.Risorse_Umane Where r.Piva = contatto.Piva AndAlso r.Cod_Contatto = contatto.Cod_Contatto AndAlso
                        (listaCodRapporto.Count = 0 OrElse listaCodRapporto.Contains(r.Cod_Rapporto)) AndAlso
                        (dataValidita = AGRODATAINIZIO OrElse listaContatti.Contains(r.Cod_RisUm))
                    Select r).ToList()

                If risorse.Count > 0 Then

                    Dim listaRisorse = (From r In risorse Select r.Cod_RisUm).ToList()
                    Dim codici = (From c In GiasContext.Contatti_Codici Where c.PIVA = contatto.Piva AndAlso c.Cod_Contatto = contatto.Cod_Contatto Select c).ToList()
                    Dim indirizzi = (From ci In GiasContext.ContattiXIndirizzi Join i In GiasContext.Indirizzi On ci.Cod_Indirizzo Equals i.cod_indirizzo Where ci.Piva = contatto.Piva AndAlso ci.Cod_Contatto = contatto.Cod_Contatto Select New G2G_Indirizzo With {.Indirizzo = i, .Tipo_Indirizzo = ci.Tipo_Indirizzo}).ToList()
                    Dim rubrica = (From cr In GiasContext.ContattiXRubrica Join r In GiasContext.Rubrica On cr.Cod_Rubrica Equals r.cod_rubrica Where cr.Piva = contatto.Piva AndAlso cr.Cod_Contatto = contatto.Cod_Contatto Select r).ToList()
                    Dim costi = (From c In GiasContext.Prodotti_Costi Where c.Piva = contatto.Piva AndAlso listaRisorse.Contains(c.Mat_Cod) AndAlso c.Elem_Cod = 0 AndAlso c.Id_Budget = 0 Select c).ToList()
                    Dim allegati = If(g2g.GestioneAllegati, Leggi_Contatti_Allegati_G2G(GiasContext, contatto), Nothing)

                    ' forzo il centro aziendale contatto con quello destinazione
                    If Not Flag_Pubblico AndAlso contatto.Sa_Cod <> 0 Then
                        Dim recode = (
                            From rr In GiasContext.G2G_Recode_Imprese
                            Where rr.To_PivaSuperUser = From_PivaSuperUser AndAlso rr.From_PivaSuperUser = To_PivaSuperUser AndAlso
                            rr.FROM_Piva = contatto.Piva AndAlso rr.TO_SaCod = contatto.Sa_Cod).FirstOrDefault()
                        contatto.Sa_Cod = If(recode Is Nothing, 0, recode.FROM_SaCod)
                    End If

                    g2g.ContattiToInsert.Add(New G2G_Contatto With {.Contatto = contatto, .RisorseUmane = risorse, .Codici = codici, .Indirizzi = indirizzi, .Rubrica = rubrica, .Costi = costi, .Allegati = allegati})

                End If
            Next

            Dim listaContattiUpdate = (
                From g In GiasContext.G2G_Recode_Contatti
                Join r In GiasContext.Risorse_Umane On g.To_Cod_Risum Equals r.Cod_RisUm
                Join c In GiasContext.Contatti On c.Piva Equals r.Piva And c.Cod_Contatto Equals r.Cod_Contatto
                Where g.To_PivaSuperUser = From_PivaSuperUser AndAlso g.From_PivaSuperUser = To_PivaSuperUser AndAlso
                    If(Flag_Pubblico, c.Sa_Cod = -1, c.Sa_Cod <> -1 AndAlso c.Piva = Piva) AndAlso g.datainvio < c.Data_Modifica
                Select c).Distinct().ToList()

            For Each contatto In listaContattiUpdate

                Dim risorse = (
                    From r In GiasContext.Risorse_Umane Where r.Piva = contatto.Piva AndAlso r.Cod_Contatto = contatto.Cod_Contatto AndAlso
                        (listaCodRapporto.Count = 0 OrElse listaCodRapporto.Contains(r.Cod_Rapporto)) AndAlso
                        (dataValidita = AGRODATAINIZIO OrElse listaContatti.Contains(r.Cod_RisUm))
                    Select r).ToList()

                If risorse.Count > 0 Then

                    Dim listaRisorse = (From r In risorse Select r.Cod_RisUm).ToList()
                    Dim codici = (From c In GiasContext.Contatti_Codici Where c.PIVA = contatto.Piva AndAlso c.Cod_Contatto = contatto.Cod_Contatto Select c).ToList()
                    Dim indirizzi = (From ci In GiasContext.ContattiXIndirizzi Join i In GiasContext.Indirizzi On ci.Cod_Indirizzo Equals i.cod_indirizzo Where ci.Piva = contatto.Piva AndAlso ci.Cod_Contatto = contatto.Cod_Contatto Select New G2G_Indirizzo With {.Indirizzo = i, .Tipo_Indirizzo = ci.Tipo_Indirizzo}).ToList()
                    Dim rubrica = (From cr In GiasContext.ContattiXRubrica Join r In GiasContext.Rubrica On cr.Cod_Rubrica Equals r.cod_rubrica Where cr.Piva = contatto.Piva AndAlso cr.Cod_Contatto = contatto.Cod_Contatto Select r).ToList()
                    Dim costi = (From c In GiasContext.Prodotti_Costi Where c.Piva = contatto.Piva AndAlso listaRisorse.Contains(c.Mat_Cod) AndAlso c.Elem_Cod = 0 AndAlso c.Id_Budget = 0 Select c).ToList()
                    Dim allegati = If(g2g.GestioneAllegati, Leggi_Contatti_Allegati_G2G(GiasContext, contatto), Nothing)

                    ' forzo il centro aziendale contatto con quello destinazione
                    If Not Flag_Pubblico AndAlso contatto.Sa_Cod <> 0 Then
                        Dim recode = (
                            From rr In GiasContext.G2G_Recode_Imprese
                            Where rr.To_PivaSuperUser = From_PivaSuperUser AndAlso
                                rr.From_PivaSuperUser = To_PivaSuperUser AndAlso
                                rr.FROM_Piva = contatto.Piva AndAlso
                                rr.TO_SaCod = contatto.Sa_Cod).FirstOrDefault()
                        contatto.Sa_Cod = If(recode Is Nothing, 0, recode.FROM_SaCod)
                    End If

                    g2g.ContattiToUpdate.Add(New G2G_Contatto With {.Contatto = contatto, .RisorseUmane = risorse, .Codici = codici, .Indirizzi = indirizzi, .Rubrica = rubrica, .Costi = costi, .Allegati = allegati})

                End If

            Next

            If Not Flag_Pubblico Then
                g2g.Recode.G2GRecodeContattiToDelete = (
                    From g In GiasContext.G2G_Recode_Contatti
                    Where g.To_PivaSuperUser = From_PivaSuperUser AndAlso g.From_PivaSuperUser = To_PivaSuperUser AndAlso
                        Not GiasContext.Risorse_Umane.Any(Function(r) r.Cod_RisUm = g.To_Cod_Risum)
                    Select g).ToList()
            End If

        End Using

        Return g2g.ContattiToInsert.Count > 0 OrElse g2g.ContattiToUpdate.Count > 0 OrElse g2g.Recode.G2GRecodeContattiToDelete.Count > 0

    End Function

End Class

Public Class G2GContatti_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi_Contatti_G2G(ByRef g2g As G2G_Contatti, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GParcoMacchine_W.Scrivi_Contatti_G2G()"
        Dim messaggioErrore As String = ""

        If G2GUtility.transactionScope Then

            Dim transactionOptions As New TransactionOptions()
            transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            transactionOptions.Timeout = G2GUtility.GetTransactionTimeout(objParametri)
            Dim g2gRecode As G2G_Recode = g2g.Recode

            Try

                Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)
                    g2gRecode = Scrivi_Contatti(g2g, objParametri)
                    scope.Complete()
                End Using

            Catch ex As Exception

                messaggioErrore = ex.Message
                g2gRecode.MessaggioErrore = messaggioErrore
                Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)

            End Try

            Return g2gRecode

        End If

        Return Scrivi_Contatti(g2g, objParametri)

    End Function

    Public Function Scrivi_Contatti_G2GReverse(ByRef g2g As G2G_Contatti_Reverse, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GParcoMacchine_W.Scrivi_Contatti_G2G()"
        Dim messaggioErrore As String = ""

        If G2GUtility.transactionScope Then

            Dim transactionOptions As New TransactionOptions()
            transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            transactionOptions.Timeout = G2GUtility.GetTransactionTimeout(objParametri)
            Dim g2gRecode As G2G_Recode = g2g.Recode

            Try

                Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)
                    g2gRecode = Scrivi_ContattiReverse(g2g, objParametri)
                    scope.Complete()
                End Using

            Catch ex As Exception

                messaggioErrore = ex.Message
                g2gRecode.MessaggioErrore = messaggioErrore
                Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)

            End Try

            Return g2gRecode

        End If

        Return Scrivi_ContattiReverse(g2g, objParametri)

    End Function

    Public Function Scrivi_Contatti(ByRef g2g As G2G_Contatti, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GContatti_W.Scrivi_Contatti()"
        Dim messaggioErrore As String = ""

        Dim From_PivaSuperUser = g2g.From_PivaSuperUser
        Dim To_PivaSuperUser = g2g.To_PivaSuperUser
        Dim From_Piva = g2g.From_Piva
        Dim To_Piva = g2g.To_Piva
        Dim username = objParametri.UsernameOperazione
        Dim data = Date.Now

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try

            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                ' CONTATTI DA CANCELLARE (solo privati)
                If g2g.Recode.G2GRecodeContattiToDelete.Count > 0 Then

                    Dim index As Integer = 0
                    Dim listaContattiDaCancellare = New List(Of String)

                    ' cancella risorse contatti
                    For Each r As G2G_Recode_Contatti In g2g.Recode.G2GRecodeContattiToDelete

                        Dim risorsa = (From rr In GiasContext.Risorse_Umane Where rr.Cod_RisUm = r.To_Cod_Risum).FirstOrDefault()

                        ' cancella contatto privato azienda
                        If risorsa IsNot Nothing AndAlso risorsa.Piva = To_Piva AndAlso risorsa.Sa_Cod <> -1 Then

                            'cancella risorsa umana
                            listaContattiDaCancellare.Add(risorsa.Cod_Contatto)
                            GiasContext.Risorse_Umane.Attach(risorsa)
                            GiasContext.Risorse_Umane.Remove(risorsa)
                            'GiasContext.AttachTo("Risorse_Umane", risorsa)
                            'GiasContext.DeleteObject(risorsa)

                            ' cancella recode contatto
                            Dim recode = (From rr In GiasContext.G2G_Recode_Contatti Where rr.From_PivaSuperUser = From_PivaSuperUser AndAlso rr.To_PivaSuperUser = To_PivaSuperUser AndAlso rr.From_Cod_RisUm = r.From_Cod_RisUm).FirstOrDefault()
                            If recode IsNot Nothing Then
                                GiasContext.G2G_Recode_Contatti.Attach(recode)
                                GiasContext.G2G_Recode_Contatti.Remove(recode)
                            End If

                        End If

                        G2GUtility.SaveChanges(GiasContext, index)

                    Next

                    GiasContext.SaveChanges()

                    If listaContattiDaCancellare.Count > 0 Then

                        ' cancella contatti che non hanno risorse attaccate
                        For Each cod_contatto In listaContattiDaCancellare.Distinct()
                            Dim risorse = (From r In GiasContext.Risorse_Umane Where r.Piva = To_Piva AndAlso r.Cod_Contatto = cod_contatto).ToList()
                            If risorse.Count = 0 Then
                                Dim contatto = (From c In GiasContext.Contatti Where c.Piva = To_Piva AndAlso c.Cod_Contatto = cod_contatto).FirstOrDefault()
                                If contatto IsNot Nothing Then

                                    ' cancello allegati contatto
                                    If g2g.GestioneAllegati Then
                                        Dim alert_entita = (From e In GiasContext.Alert_Entita Where e.PivaSuperUser = To_PivaSuperUser AndAlso e.Piva = contatto.Piva AndAlso e.Cod_Contatto = contatto.Cod_Contatto Select e).ToList()
                                        For Each e As Alert_Entita In alert_entita
                                            Dim scadenze = (From s In GiasContext.Alert_Elenco Where s.PivaSuperUser = e.PivaSuperUser AndAlso s.ID_Alert_Entita = e.ID_Alert_Entita Select s).ToList()
                                            For Each scadenza As Alert_Elenco In scadenze
                                                GiasContext.Alert_Elenco.Attach(scadenza)
                                                GiasContext.Alert_Elenco.Remove(scadenza)
                                            Next
                                            Dim documento = (From d In GiasContext.Allegati_Documenti Where d.Allegati_Documenti_SuperUser = e.PivaSuperUser AndAlso d.Allegati_Documenti_Piva = e.Piva AndAlso d.Allegati_Documenti_Cod = e.Allegati_Documenti_Cod Select d).FirstOrDefault()
                                            If documento IsNot Nothing Then
                                                GiasContext.Allegati_Documenti.Attach(documento)
                                                GiasContext.Allegati_Documenti.Remove(documento)
                                            End If
                                            GiasContext.Alert_Entita.Attach(e)
                                            GiasContext.Alert_Entita.Remove(e)
                                        Next
                                    End If

                                    'TODO: cancellare record nelle altre tabelle collegate al contatto
                                    GiasContext.Contatti.Attach(contatto)
                                    GiasContext.Contatti.Remove(contatto)

                                End If
                            End If
                        Next

                        GiasContext.SaveChanges()

                    End If

                End If

                ' CONTATTI DA INSERIRE
                If g2g.ContattiToInsert.Count > 0 Then

                    Dim index As Integer = 0

                    For Each c As G2G_Contatto In g2g.ContattiToInsert

                        Dim contatto As Contatti = Nothing
                        Dim contatto_pubblico As Boolean = String.IsNullOrEmpty(To_Piva) OrElse c.Contatto.Sa_Cod = -1
                        Dim contatto_piva = If(contatto_pubblico, c.Contatto.Piva, To_Piva)

                        ' se il contatto è già presente allora salto l'inserimento
                        contatto = (From cc In GiasContext.Contatti Where cc.Piva = contatto_piva AndAlso cc.Cod_Contatto = c.Contatto.Cod_Contatto).FirstOrDefault()

                        If contatto IsNot Nothing Then

                            ' creo mappature con risorse esistenti
                            For Each r As Risorse_Umane In c.RisorseUmane

                                Dim risorsa = (From rr In GiasContext.Risorse_Umane Where rr.Piva = contatto.Piva AndAlso rr.Cod_Contatto = contatto.Cod_Contatto AndAlso rr.Cod_Rapporto = r.Cod_Rapporto Select rr).FirstOrDefault

                                If risorsa IsNot Nothing Then
                                    Dim recode = New G2G_Recode_Contatti With {
                                        .From_PivaSuperUser = From_PivaSuperUser,
                                        .To_PivaSuperUser = To_PivaSuperUser,
                                        .From_Cod_RisUm = r.Cod_RisUm,
                                        .To_Cod_Risum = risorsa.Cod_RisUm,
                                        .Username_Creazione = username,
                                        .Username_Modifica = username,
                                        .Data_Creazione = Now(),
                                        .Data_Modifica = Now(),
                                        .Validita_Inizio = AGRODATAINIZIO,
                                        .Validita_Fine = AGRODATAFINE,
                                        .inviato = "0",
                                        .datainvio = Now()
                                    }
                                    g2g.Recode.G2GRecodeContattiToInsert.Add(recode)
                                    GiasContext.G2G_Recode_Contatti.Add(recode)
                                End If

                            Next

                            Continue For

                        End If

                        ' nuovo contatto
                        contatto = Gias_EF_Utility.CopyEntity(GiasContext, c.Contatto, contatto, username, data)
                        contatto.Piva = contatto_piva
                        GiasContext.Contatti.Add(contatto)

                        ' inserisco codici contatto
                        For Each cc As Contatti_Codici In c.Codici
                            Dim codice = Gias_EF_Utility.CopyEntity(GiasContext, cc, Nothing, username, data)
                            codice.PIVA = contatto.Piva
                            GiasContext.Contatti_Codici.Add(codice)
                        Next

                        ' inserisco indirizzi contatto
                        For Each i As G2G_Indirizzo In c.Indirizzi

                            ' nuovo indirizzo
                            Dim idSeq = G2GUtility.NuovoId_Tabella(GiasContext, "Indirizzi", 0, 2000000000, objParametri)
                            Dim indirizzo = Gias_EF_Utility.CopyEntity(GiasContext, i.Indirizzo, Nothing, username, data)
                            indirizzo.cod_indirizzo = idSeq
                            GiasContext.Indirizzi.Add(indirizzo)

                            ' nuovo indirizzo contatto
                            Dim cxi = New ContattiXIndirizzi With {
                                .Piva = contatto.Piva,
                                .Sa_Cod = contatto.Sa_Cod,
                                .Cod_Contatto = contatto.Cod_Contatto,
                                .Cod_Indirizzo = indirizzo.cod_indirizzo,
                                .Tipo_Indirizzo = i.Tipo_Indirizzo,
                                .Inviato = 0,
                                .Data_Creazione = data,
                                .Data_Modifica = data,
                                .Validita_Inizio = AGRODATAINIZIO,
                                .Validita_Fine = AGRODATAFINE,
                                .Username_Creazione = username,
                                .Username_Modifica = username
                            }
                            GiasContext.ContattiXIndirizzi.Add(cxi)

                            ' nuovo recode indirizzo
                            Dim recode =
                                New G2G_Recode_Indirizzi With {
                                    .From_PivaSuperUser = From_PivaSuperUser,
                                    .To_PivaSuperUser = To_PivaSuperUser,
                                    .From_Cod_Indirizzo = i.Indirizzo.cod_indirizzo,
                                    .To_Cod_Indirizzo = idSeq,
                                    .Username_Creazione = username,
                                    .Username_Modifica = username,
                                    .Data_Creazione = Now(),
                                    .Data_Modifica = Now(),
                                    .Validita_Inizio = AGRODATAINIZIO,
                                    .Validita_Fine = AGRODATAFINE,
                                    .inviato = "0",
                                    .datainvio = Now()
                                }
                            g2g.Recode.G2GRecodeIndirizziToInsert.Add(recode)
                            GiasContext.G2G_Recode_Indirizzi.Add(recode)

                        Next

                        ' inserisco rubrica contatto
                        For Each cr As Rubrica In c.Rubrica

                            ' nuova rubrica
                            Dim idSeq = G2GUtility.NuovoId_Tabella(GiasContext, "Rubrica", 0, 2000000000, objParametri)
                            Dim rubrica = Gias_EF_Utility.CopyEntity(GiasContext, cr, Nothing, username, data)
                            rubrica.cod_rubrica = idSeq
                            GiasContext.Rubrica.Add(rubrica)

                            ' nuova rubrica contatto
                            Dim cxr = New ContattiXRubrica With {
                                .Piva = contatto.Piva,
                                .Sa_Cod = contatto.Sa_Cod,
                                .Cod_Contatto = contatto.Cod_Contatto,
                                .Cod_Rubrica = rubrica.cod_rubrica,
                                .Inviato = 0,
                                .Data_Creazione = data,
                                .Data_Modifica = data,
                                .Validita_Inizio = AGRODATAINIZIO,
                                .Validita_Fine = AGRODATAFINE,
                                .Username_Creazione = username,
                                .Username_Modifica = username
                            }
                            GiasContext.ContattiXRubrica.Add(cxr)

                        Next

                        If g2g.GestioneAllegati Then

                            ' inserisco allegati contatto
                            For Each e As Alert_Entita In c.Allegati.Entita

                                ' nuovo allegato documento
                                Dim doc_from = (From d In c.Allegati.Documenti Where d.Allegati_Documenti_Cod = e.Allegati_Documenti_Cod).FirstOrDefault()

                                If doc_from IsNot Nothing Then

                                    Dim documento = Gias_EF_Utility.CopyEntity(GiasContext, doc_from, Nothing, username, data)
                                    documento.Allegati_Documenti_SuperUser = To_PivaSuperUser
                                    documento.Allegati_Documenti_Piva = contatto.Piva
                                    GiasContext.Allegati_Documenti.Add(documento)
                                    GiasContext.SaveChanges()

                                    ' nuovo alert entita
                                    Dim idAlert = G2GUtility.NuovoId_Tabella(GiasContext, "Alert_Entita", 0, 2000000000, objParametri)
                                    Dim entita = Gias_EF_Utility.CopyEntity(GiasContext, e, Nothing, username, data)
                                    entita.PivaSuperUser = To_PivaSuperUser
                                    entita.Piva = contatto.Piva
                                    entita.ID_Alert_Entita = idAlert
                                    entita.Allegati_Documenti_Cod = documento.Allegati_Documenti_Cod
                                    GiasContext.Alert_Entita.Add(entita)

                                    ' nuovo alert elenco
                                    Dim scadenze = (From s In c.Allegati.Scadenze Where s.ID_Alert_Entita = e.ID_Alert_Entita).ToList()
                                    For Each s As Alert_Elenco In scadenze
                                        Dim idElenco = G2GUtility.NuovoId_Tabella(GiasContext, "Alert_Elenco", 0, 2000000000, objParametri)
                                        Dim scadenza = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)
                                        scadenza.PivaSuperUser = To_PivaSuperUser
                                        scadenza.ID_Alert_Entita = entita.ID_Alert_Entita
                                        scadenza.ID_Elenco = idElenco
                                        GiasContext.Alert_Elenco.Add(scadenza)
                                    Next

                                End If

                            Next

                        End If

                        ' inserisco risorse umane contatto
                        For Each r As Risorse_Umane In c.RisorseUmane

                            ' nuova risorsa numana
                            Dim idSeq = G2GUtility.NuovoId_Tabella(GiasContext, "Risorse_Umane", 0, 2000000000, objParametri)
                            Dim risorsa = Gias_EF_Utility.CopyEntity(GiasContext, r, Nothing, username, data)
                            risorsa.Piva = contatto.Piva
                            risorsa.Cod_RisUm = idSeq
                            GiasContext.Risorse_Umane.Add(risorsa)

                            ' inserisce costi contatto
                            For Each pc As Prodotti_Costi In c.Costi
                                If pc.Mat_Cod = r.Cod_RisUm Then
                                    Dim costo = Gias_EF_Utility.CopyEntity(GiasContext, pc, Nothing, username, data)
                                    costo.Piva = To_Piva
                                    costo.Mat_Cod = idSeq
                                    GiasContext.Prodotti_Costi.Add(costo)
                                End If
                            Next

                            ' nuovo recode contatto
                            Dim recode =
                                New G2G_Recode_Contatti With {
                                    .From_PivaSuperUser = From_PivaSuperUser,
                                    .To_PivaSuperUser = To_PivaSuperUser,
                                    .From_Cod_RisUm = r.Cod_RisUm,
                                    .To_Cod_Risum = idSeq,
                                    .Username_Creazione = username,
                                    .Username_Modifica = username,
                                    .Data_Creazione = Now(),
                                    .Data_Modifica = Now(),
                                    .Validita_Inizio = AGRODATAINIZIO,
                                    .Validita_Fine = AGRODATAFINE,
                                    .inviato = "0",
                                    .datainvio = Now()
                                }
                            g2g.Recode.G2GRecodeContattiToInsert.Add(recode)
                            GiasContext.G2G_Recode_Contatti.Add(recode)

                        Next

                        G2GUtility.SaveChanges(GiasContext, index)

                    Next

                    GiasContext.SaveChanges()

                End If

                ' CONTATTI DA MODIFICARE
                If g2g.ContattiToUpdate.Count > 0 Then

                    Dim index As Integer = 0

                    For Each c As G2G_Contatto In g2g.ContattiToUpdate

                        Dim contatto_pubblico As Boolean = String.IsNullOrEmpty(To_Piva) OrElse c.Contatto.Sa_Cod = -1
                        Dim contatto_piva = If(contatto_pubblico, c.Contatto.Piva, To_Piva)

                        Dim contatto = (From cc In GiasContext.Contatti Where cc.Piva = contatto_piva AndAlso cc.Cod_Contatto = c.Contatto.Cod_Contatto).FirstOrDefault()
                        Dim risorse = (From r In GiasContext.Risorse_Umane Where r.Piva = contatto.Piva AndAlso r.Cod_Contatto = contatto.Cod_Contatto Select r).ToList()

                        ' modifica contatto
                        contatto = Gias_EF_Utility.CopyEntity(GiasContext, c.Contatto, contatto, username, data)
                        contatto.Piva = contatto_piva
                        GiasContext.Contatti.Attach(contatto)
                        GiasContext.Entry(contatto).State = EntityState.Modified
                        'GiasContext.ObjectStateManager.ChangeObjectState(contatto, EntityState.Modified)

                        Dim listaCodiciOK = New List(Of Integer)
                        Dim listaCodici = (From cc In GiasContext.Contatti_Codici Where cc.PIVA = contatto.Piva AndAlso cc.Cod_Contatto = contatto.Cod_Contatto Select cc).ToList()

                        ' inserisco / modifico codici contatto
                        For Each cc As Contatti_Codici In c.Codici
                            Dim codice = (From ccc In listaCodici Where ccc.Id_cod = cc.Id_cod Select ccc).FirstOrDefault()
                            If codice Is Nothing Then
                                codice = Gias_EF_Utility.CopyEntity(GiasContext, cc, Nothing, username, data)
                                codice.PIVA = contatto.Piva
                                GiasContext.Contatti_Codici.Add(codice)
                            Else
                                codice = Gias_EF_Utility.CopyEntity(GiasContext, cc, codice, username, data)
                                codice.PIVA = contatto.Piva
                                GiasContext.Contatti_Codici.Attach(codice)
                                GiasContext.Entry(codice).State = EntityState.Modified
                            End If
                            listaCodiciOK.Add(cc.Id_cod)
                        Next

                        ' cancello codici non presenti
                        Dim codiciKO = (From ccc In listaCodici Where Not listaCodiciOK.Contains(ccc.Id_cod) Select ccc).ToList()
                        For Each codice In codiciKO
                            GiasContext.Contatti_Codici.Attach(codice)
                            GiasContext.Contatti_Codici.Remove(codice)
                        Next

                        ' cancello indirizzi contatto
                        Dim contattixindirizzi = (From ci In GiasContext.ContattiXIndirizzi Where ci.Piva = contatto.Piva AndAlso ci.Cod_Contatto = contatto.Cod_Contatto Select ci).ToList()
                        For Each cxi As ContattiXIndirizzi In contattixindirizzi
                            GiasContext.ContattiXIndirizzi.Attach(cxi)
                            GiasContext.ContattiXIndirizzi.Remove(cxi)
                        Next

                        ' inserisco / modifico indirizzi contatto
                        For Each i As G2G_Indirizzo In c.Indirizzi

                            Dim recode = (From rr In GiasContext.G2G_Recode_Indirizzi Where rr.From_PivaSuperUser = From_PivaSuperUser AndAlso rr.To_PivaSuperUser = To_PivaSuperUser AndAlso rr.From_Cod_Indirizzo = i.Indirizzo.cod_indirizzo).FirstOrDefault()
                            Dim indirizzo = If(recode Is Nothing, Nothing, (From ii In GiasContext.Indirizzi Where ii.cod_indirizzo = recode.To_Cod_Indirizzo Select ii).FirstOrDefault())

                            If indirizzo Is Nothing Then

                                ' nuovo indirizzo
                                Dim idSeq = G2GUtility.NuovoId_Tabella(GiasContext, "Indirizzi", 0, 2000000000, objParametri)
                                indirizzo = Gias_EF_Utility.CopyEntity(GiasContext, i.Indirizzo, Nothing, username, data)
                                indirizzo.cod_indirizzo = idSeq
                                GiasContext.Indirizzi.Add(indirizzo)

                                ' nuovo recode indirizzi
                                Dim ri =
                                    New G2G_Recode_Indirizzi With {
                                        .From_PivaSuperUser = From_PivaSuperUser,
                                        .To_PivaSuperUser = To_PivaSuperUser,
                                        .From_Cod_Indirizzo = i.Indirizzo.cod_indirizzo,
                                        .To_Cod_Indirizzo = indirizzo.cod_indirizzo,
                                        .Username_Creazione = username,
                                        .Username_Modifica = username,
                                        .Data_Creazione = Now(),
                                        .Data_Modifica = Now(),
                                        .Validita_Inizio = AGRODATAINIZIO,
                                        .Validita_Fine = AGRODATAFINE,
                                        .inviato = "0",
                                        .datainvio = Now()
                                    }
                                g2g.Recode.G2GRecodeIndirizziToInsert.Add(ri)
                                GiasContext.G2G_Recode_Indirizzi.Add(ri)

                            Else

                                ' modifica indirizzo
                                indirizzo = Gias_EF_Utility.CopyEntity(GiasContext, i.Indirizzo, indirizzo, username, data)
                                indirizzo.cod_indirizzo = recode.To_Cod_Indirizzo
                                GiasContext.Indirizzi.Attach(indirizzo)
                                GiasContext.Entry(indirizzo).State = EntityState.Modified

                                ' modifica recode indirizzo
                                recode.Username_Modifica = username
                                recode.Data_Modifica = data
                                recode.datainvio = data
                                g2g.Recode.G2GRecodeIndirizziToUpdate.Add(recode)
                                GiasContext.G2G_Recode_Indirizzi.Attach(recode)
                                GiasContext.Entry(recode).State = EntityState.Modified

                            End If

                            ' nuovo indirizzo contatto
                            Dim cxi = New ContattiXIndirizzi With {
                                .Piva = contatto.Piva,
                                .Sa_Cod = contatto.Sa_Cod,
                                .Cod_Contatto = contatto.Cod_Contatto,
                                .Cod_Indirizzo = indirizzo.cod_indirizzo,
                                .Tipo_Indirizzo = i.Tipo_Indirizzo,
                                .Inviato = 0,
                                .Data_Creazione = data,
                                .Data_Modifica = data,
                                .Validita_Inizio = AGRODATAINIZIO,
                                .Validita_Fine = AGRODATAFINE,
                                .Username_Creazione = username,
                                .Username_Modifica = username
                            }
                            GiasContext.ContattiXIndirizzi.Add(cxi)

                        Next

                        ' cancello rubrica contatto
                        Dim contattixrubrica = (From cr In GiasContext.ContattiXRubrica Where cr.Piva = contatto.Piva AndAlso cr.Cod_Contatto = contatto.Cod_Contatto Select cr).ToList()
                        For Each cxr As ContattiXRubrica In contattixrubrica
                            Dim rubrica = (From r In GiasContext.Rubrica Where r.cod_rubrica = cxr.Cod_Rubrica Select r).FirstOrDefault()
                            GiasContext.ContattiXRubrica.Attach(cxr)
                            GiasContext.ContattiXRubrica.Remove(cxr)

                            If rubrica IsNot Nothing Then
                                GiasContext.Rubrica.Attach(rubrica)
                                GiasContext.Rubrica.Remove(rubrica)
                            End If
                        Next

                        ' inserisco rubrica contatto
                        For Each cr As Rubrica In c.Rubrica

                            Dim idSeq = G2GUtility.NuovoId_Tabella(GiasContext, "Rubrica", 0, 2000000000, objParametri)
                            Dim rubrica = Gias_EF_Utility.CopyEntity(GiasContext, cr, Nothing, username, data)
                            rubrica.cod_rubrica = idSeq
                            GiasContext.Rubrica.Add(rubrica)

                            Dim cxr = New ContattiXRubrica With {
                                .Piva = contatto.Piva,
                                .Sa_Cod = contatto.Sa_Cod,
                                .Cod_Contatto = contatto.Cod_Contatto,
                                .Cod_Rubrica = rubrica.cod_rubrica,
                                .Inviato = 0,
                                .Data_Creazione = data,
                                .Data_Modifica = data,
                                .Validita_Inizio = AGRODATAINIZIO,
                                .Validita_Fine = AGRODATAFINE,
                                .Username_Creazione = username,
                                .Username_Modifica = username
                            }
                            GiasContext.ContattiXRubrica.Add(cxr)

                        Next

                        If g2g.GestioneAllegati Then

                            ' cancello allegati contatto
                            Dim alert_entita = (From e In GiasContext.Alert_Entita Where e.PivaSuperUser = To_PivaSuperUser AndAlso e.Piva = contatto.Piva AndAlso e.Cod_Contatto = contatto.Cod_Contatto Select e).ToList()
                            For Each e As Alert_Entita In alert_entita

                                Dim scadenze = (From s In GiasContext.Alert_Elenco Where s.PivaSuperUser = e.PivaSuperUser AndAlso s.ID_Alert_Entita = e.ID_Alert_Entita Select s).ToList()
                                For Each scadenza As Alert_Elenco In scadenze
                                    GiasContext.Alert_Elenco.Attach(scadenza)
                                    GiasContext.Alert_Elenco.Remove(scadenza)
                                Next

                                Dim documento = (From d In GiasContext.Allegati_Documenti Where d.Allegati_Documenti_SuperUser = e.PivaSuperUser AndAlso d.Allegati_Documenti_Piva = e.Piva AndAlso d.Allegati_Documenti_Cod = e.Allegati_Documenti_Cod Select d).FirstOrDefault()
                                If documento IsNot Nothing Then
                                    GiasContext.Allegati_Documenti.Attach(documento)
                                    GiasContext.Allegati_Documenti.Remove(documento)
                                End If

                                GiasContext.Alert_Entita.Attach(e)
                                GiasContext.Alert_Entita.Remove(e)

                            Next

                            ' inserisco allegati contatto
                            For Each e As Alert_Entita In c.Allegati.Entita

                                ' nuovo allegato documento
                                Dim doc_from = (From d In c.Allegati.Documenti Where d.Allegati_Documenti_Cod = e.Allegati_Documenti_Cod).FirstOrDefault()

                                If doc_from IsNot Nothing Then

                                    Dim documento = Gias_EF_Utility.CopyEntity(GiasContext, doc_from, Nothing, username, data)
                                    documento.Allegati_Documenti_SuperUser = To_PivaSuperUser
                                    documento.Allegati_Documenti_Piva = contatto.Piva
                                    GiasContext.Allegati_Documenti.Add(documento)
                                    GiasContext.SaveChanges()

                                    ' nuovo alert entita
                                    Dim idAlert = G2GUtility.NuovoId_Tabella(GiasContext, "Alert_Entita", 0, 2000000000, objParametri)
                                    Dim entita = Gias_EF_Utility.CopyEntity(GiasContext, e, Nothing, username, data)
                                    entita.PivaSuperUser = To_PivaSuperUser
                                    entita.Piva = contatto.Piva
                                    entita.ID_Alert_Entita = idAlert
                                    entita.Allegati_Documenti_Cod = documento.Allegati_Documenti_Cod
                                    GiasContext.Alert_Entita.Add(entita)

                                    ' nuovo alert elenco
                                    Dim scadenze = (From s In c.Allegati.Scadenze Where s.ID_Alert_Entita = e.ID_Alert_Entita).ToList()
                                    For Each s As Alert_Elenco In scadenze
                                        Dim idElenco = G2GUtility.NuovoId_Tabella(GiasContext, "Alert_Elenco", 0, 2000000000, objParametri)
                                        Dim scadenza = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)
                                        scadenza.PivaSuperUser = To_PivaSuperUser
                                        scadenza.ID_Alert_Entita = entita.ID_Alert_Entita
                                        scadenza.ID_Elenco = idElenco
                                        GiasContext.Alert_Elenco.Add(scadenza)
                                    Next

                                End If

                            Next

                        End If

                        ' modifico risorse umane contatto
                        Dim listaRisorseOK = New List(Of Integer)
                        For Each r As Risorse_Umane In c.RisorseUmane

                            Dim recode = (From rr In GiasContext.G2G_Recode_Contatti Where rr.From_PivaSuperUser = From_PivaSuperUser AndAlso rr.To_PivaSuperUser = To_PivaSuperUser AndAlso rr.From_Cod_RisUm = r.Cod_RisUm).FirstOrDefault()
                            Dim risorsa = If(recode Is Nothing, Nothing, (From rr In GiasContext.Risorse_Umane Where rr.Cod_RisUm = recode.To_Cod_Risum).FirstOrDefault())

                            ' creo la risorsa se non è presente 
                            If risorsa Is Nothing Then

                                ' nuova risorsa umana contatto
                                Dim idSeq = G2GUtility.NuovoId_Tabella(GiasContext, "Risorse_Umane", 0, 2000000000, objParametri)
                                risorsa = Gias_EF_Utility.CopyEntity(GiasContext, r, Nothing, username, data)
                                risorsa.Piva = contatto_piva
                                risorsa.Cod_RisUm = idSeq
                                GiasContext.Risorse_Umane.Add(risorsa)

                                ' inserisce costi contatto
                                For Each pc As Prodotti_Costi In c.Costi
                                    If pc.Mat_Cod = r.Cod_RisUm Then
                                        Dim costo = Gias_EF_Utility.CopyEntity(GiasContext, pc, Nothing, username, data)
                                        costo.Piva = contatto_piva
                                        costo.Mat_Cod = idSeq
                                        GiasContext.Prodotti_Costi.Add(costo)
                                    End If
                                Next

                                ' nuovo recode contatto
                                recode =
                                    New G2G_Recode_Contatti With {
                                        .From_PivaSuperUser = From_PivaSuperUser,
                                        .To_PivaSuperUser = To_PivaSuperUser,
                                        .From_Cod_RisUm = r.Cod_RisUm,
                                        .To_Cod_Risum = idSeq,
                                        .Username_Creazione = username,
                                        .Username_Modifica = username,
                                        .Data_Creazione = Now(),
                                        .Data_Modifica = Now(),
                                        .Validita_Inizio = AGRODATAINIZIO,
                                        .Validita_Fine = AGRODATAFINE,
                                        .inviato = "0",
                                        .datainvio = Now()
                                    }
                                g2g.Recode.G2GRecodeContattiToInsert.Add(recode)
                                GiasContext.G2G_Recode_Contatti.Add(recode)

                            Else

                                If Not listaRisorseOK.Contains(risorsa.Cod_RisUm) Then

                                    ' modifica risorsa contatto
                                    risorsa = Gias_EF_Utility.CopyEntity(GiasContext, r, risorsa, username, data)
                                    risorsa.Piva = contatto_piva
                                    risorsa.Cod_RisUm = recode.To_Cod_Risum
                                    GiasContext.Risorse_Umane.Attach(risorsa)
                                    GiasContext.Entry(risorsa).State = EntityState.Modified

                                    ' cancella costi contatto
                                    Dim costi = (From pc In GiasContext.Prodotti_Costi Where pc.Piva = risorsa.Piva And pc.Mat_Cod = risorsa.Cod_RisUm And pc.Elem_Cod = 0 And pc.Id_Budget = 0 Select pc).ToList()
                                    For Each pc As Prodotti_Costi In costi
                                        GiasContext.Prodotti_Costi.Attach(pc)
                                        GiasContext.Prodotti_Costi.Remove(pc)
                                    Next

                                    ' inserisce costi contatto
                                    For Each pc As Prodotti_Costi In c.Costi
                                        If pc.Mat_Cod = r.Cod_RisUm Then
                                            Dim costo = Gias_EF_Utility.CopyEntity(GiasContext, pc, Nothing, username, data)
                                            costo.Piva = contatto_piva
                                            costo.Mat_Cod = recode.To_Cod_Risum
                                            GiasContext.Prodotti_Costi.Add(costo)
                                        End If
                                    Next

                                End If

                                ' modifica recode contatto
                                recode.Username_Modifica = username
                                recode.Data_Modifica = data
                                recode.datainvio = data
                                g2g.Recode.G2GRecodeContattiToUpdate.Add(recode)
                                GiasContext.G2G_Recode_Contatti.Attach(recode)
                                GiasContext.Entry(recode).State = EntityState.Modified

                            End If

                            listaRisorseOK.Add(risorsa.Cod_RisUm)

                        Next

                        ' cancello risorse contatto eliminate
                        For Each risorsa In risorse

                            If Not listaRisorseOK.Contains(risorsa.Cod_RisUm) Then

                                ' cancella costi contatto
                                Dim costi = (From pc In GiasContext.Prodotti_Costi Where pc.Piva = risorsa.Piva And pc.Mat_Cod = risorsa.Cod_RisUm And pc.Elem_Cod = 0 And pc.Id_Budget = 0 Select pc).ToList()
                                For Each pc As Prodotti_Costi In costi
                                    GiasContext.Prodotti_Costi.Attach(pc)
                                    GiasContext.Prodotti_Costi.Remove(pc)
                                Next

                                ' cancella risorsa contatto
                                GiasContext.Risorse_Umane.Attach(risorsa)
                                GiasContext.Risorse_Umane.Remove(risorsa)

                                ' cancella recode contatto
                                Dim recode = (From rr In GiasContext.G2G_Recode_Contatti Where rr.To_PivaSuperUser = To_PivaSuperUser AndAlso rr.To_Cod_Risum = risorsa.Cod_RisUm).FirstOrDefault()
                                If recode IsNot Nothing Then
                                    g2g.Recode.G2GRecodeContattiToDelete.Add(recode)
                                    GiasContext.G2G_Recode_Contatti.Attach(recode)
                                    GiasContext.G2G_Recode_Contatti.Remove(recode)
                                End If
                            End If

                        Next

                        G2GUtility.SaveChanges(GiasContext, index)

                    Next

                    GiasContext.SaveChanges()

                End If

                ' log importazione
                g2g.Recode.LogRecode = g2g.ContattiToInsert.Count & " nuovi, " & g2g.ContattiToUpdate.Count & " modificati, " & g2g.Recode.G2GRecodeContattiToDelete.Count & " cancellati"

            End Using


        Catch ex As Exception

            messaggioErrore = ex.Message & If(IsNothing(ex.InnerException), "", vbCrLf & ex.InnerException.Message)
            g2g.Recode.MessaggioErrore = messaggioErrore
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)

        End Try


        Return g2g.Recode

    End Function

    Public Function Scrivi_ContattiReverse(ByRef g2g As G2G_Contatti_Reverse, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GContatti_W.Scrivi_ContattiReverse()"
        Dim messaggioErrore As String = ""

        Dim From_PivaSuperUser = g2g.From_PivaSuperUser
        Dim To_PivaSuperUser = g2g.To_PivaSuperUser
        Dim From_Piva = g2g.From_Piva
        Dim To_Piva = g2g.To_Piva
        Dim username = objParametri.UsernameOperazione
        Dim data = Date.Now

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try

            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                ' CONTATTI DA CANCELLARE (solo privati)
                If g2g.Recode.G2GRecodeContattiToDelete.Count > 0 Then

                    Dim index As Integer = 0
                    Dim listaContattiDaCancellare = New List(Of String)

                    ' cancella risorse contatti
                    For Each r As G2G_Recode_Contatti In g2g.Recode.G2GRecodeContattiToDelete

                        Dim risorsa = (From rr In GiasContext.Risorse_Umane Where rr.Cod_RisUm = r.From_Cod_RisUm).FirstOrDefault()

                        ' cancella contatto privato azienda
                        If risorsa IsNot Nothing AndAlso risorsa.Piva = To_Piva AndAlso risorsa.Sa_Cod <> -1 Then

                            'cancella risorsa umana
                            listaContattiDaCancellare.Add(risorsa.Cod_Contatto)
                            GiasContext.Risorse_Umane.Attach(risorsa)
                            GiasContext.Risorse_Umane.Remove(risorsa)

                            ' cancella recode contatto
                            Dim recode = (From rr In GiasContext.G2G_Recode_Contatti Where rr.To_PivaSuperUser = From_PivaSuperUser AndAlso rr.From_PivaSuperUser = To_PivaSuperUser AndAlso rr.To_Cod_Risum = r.From_Cod_RisUm).FirstOrDefault()
                            If recode IsNot Nothing Then
                                GiasContext.G2G_Recode_Contatti.Attach(recode)
                                GiasContext.G2G_Recode_Contatti.Remove(recode)
                            End If

                        End If

                        G2GUtility.SaveChanges(GiasContext, index)

                    Next

                    GiasContext.SaveChanges()

                    If listaContattiDaCancellare.Count > 0 Then

                        ' cancella contatti che non hanno risorse attaccate
                        For Each cod_contatto In listaContattiDaCancellare.Distinct()
                            Dim risorse = (From r In GiasContext.Risorse_Umane Where r.Piva = To_Piva AndAlso r.Cod_Contatto = cod_contatto).ToList()
                            If risorse.Count = 0 Then
                                Dim contatto = (From c In GiasContext.Contatti Where c.Piva = To_Piva AndAlso c.Cod_Contatto = cod_contatto).FirstOrDefault()
                                If contatto IsNot Nothing Then

                                    ' cancello allegati contatto
                                    If g2g.GestioneAllegati Then
                                        Dim alert_entita = (From e In GiasContext.Alert_Entita Where e.PivaSuperUser = To_PivaSuperUser AndAlso e.Piva = contatto.Piva AndAlso e.Cod_Contatto = contatto.Cod_Contatto Select e).ToList()
                                        For Each e As Alert_Entita In alert_entita
                                            Dim scadenze = (From s In GiasContext.Alert_Elenco Where s.PivaSuperUser = e.PivaSuperUser AndAlso s.ID_Alert_Entita = e.ID_Alert_Entita Select s).ToList()
                                            For Each scadenza As Alert_Elenco In scadenze
                                                GiasContext.Alert_Elenco.Attach(scadenza)
                                                GiasContext.Alert_Elenco.Remove(scadenza)
                                            Next
                                            Dim documento = (From d In GiasContext.Allegati_Documenti Where d.Allegati_Documenti_SuperUser = e.PivaSuperUser AndAlso d.Allegati_Documenti_Piva = e.Piva And d.Allegati_Documenti_Cod = e.Allegati_Documenti_Cod Select d).FirstOrDefault()
                                            If documento IsNot Nothing Then
                                                GiasContext.Allegati_Documenti.Attach(documento)
                                                GiasContext.Allegati_Documenti.Remove(documento)
                                            End If
                                            GiasContext.Alert_Entita.Attach(e)
                                            GiasContext.Alert_Entita.Remove(e)
                                        Next
                                    End If

                                    'TODO: cancellare record nelle altre tabelle collegate al contatto
                                    GiasContext.Contatti.Attach(contatto)
                                    GiasContext.Contatti.Remove(contatto)

                                End If
                            End If
                        Next

                        GiasContext.SaveChanges()

                    End If

                End If

                ' CONTATTI DA INSERIRE
                If g2g.ContattiToInsert.Count > 0 Then

                    Dim index As Integer = 0

                    For Each c As G2G_Contatto In g2g.ContattiToInsert

                        Dim contatto As Contatti = Nothing
                        Dim contatto_pubblico As Boolean = String.IsNullOrEmpty(To_Piva) OrElse c.Contatto.Sa_Cod = -1
                        Dim contatto_piva = If(contatto_pubblico, c.Contatto.Piva, To_Piva)

                        ' se il contatto è già presente allora salto l'inserimento
                        contatto = (From cc In GiasContext.Contatti Where cc.Piva = contatto_piva AndAlso cc.Cod_Contatto = c.Contatto.Cod_Contatto).FirstOrDefault()

                        If contatto IsNot Nothing Then

                            ' creo mappature con risorse esistenti
                            For Each r As Risorse_Umane In c.RisorseUmane

                                Dim risorsa = (From rr In GiasContext.Risorse_Umane Where rr.Piva = contatto.Piva AndAlso rr.Cod_Contatto = contatto.Cod_Contatto AndAlso rr.Cod_Rapporto = r.Cod_Rapporto Select rr).FirstOrDefault

                                If risorsa IsNot Nothing Then
                                    Dim recode = New G2G_Recode_Contatti With {
                                        .From_PivaSuperUser = To_PivaSuperUser,
                                        .To_PivaSuperUser = From_PivaSuperUser,
                                        .From_Cod_RisUm = risorsa.Cod_RisUm,
                                        .To_Cod_Risum = r.Cod_RisUm,
                                        .Username_Creazione = username,
                                        .Username_Modifica = username,
                                        .Data_Creazione = Now(),
                                        .Data_Modifica = Now(),
                                        .Validita_Inizio = AGRODATAINIZIO,
                                        .Validita_Fine = AGRODATAFINE,
                                        .inviato = "0",
                                        .datainvio = Now()
                                    }
                                    g2g.Recode.G2GRecodeContattiToInsert.Add(recode)
                                    GiasContext.G2G_Recode_Contatti.Add(recode)
                                End If

                            Next

                            Continue For

                        End If

                        ' nuovo contatto
                        contatto = Gias_EF_Utility.CopyEntity(GiasContext, c.Contatto, contatto, username, data)
                        contatto.Piva = contatto_piva
                        GiasContext.Contatti.Add(contatto)

                        ' inserisco codici contatto
                        For Each cc As Contatti_Codici In c.Codici
                            Dim codice = Gias_EF_Utility.CopyEntity(GiasContext, cc, Nothing, username, data)
                            codice.PIVA = contatto.Piva
                            GiasContext.Contatti_Codici.Add(codice)
                        Next

                        ' inserisco indirizzi contatto
                        For Each i As G2G_Indirizzo In c.Indirizzi

                            ' nuovo indirizzo
                            Dim idSeq = G2GUtility.NuovoId_Tabella(GiasContext, "Indirizzi", 0, 2000000000, objParametri)
                            Dim indirizzo = Gias_EF_Utility.CopyEntity(GiasContext, i.Indirizzo, Nothing, username, data)
                            indirizzo.cod_indirizzo = idSeq
                            GiasContext.Indirizzi.Add(indirizzo)

                            ' nuovo indirizzo contatto
                            Dim cxi = New ContattiXIndirizzi With {
                                .Piva = contatto.Piva,
                                .Sa_Cod = contatto.Sa_Cod,
                                .Cod_Contatto = contatto.Cod_Contatto,
                                .Cod_Indirizzo = indirizzo.cod_indirizzo,
                                .Tipo_Indirizzo = i.Tipo_Indirizzo,
                                .Inviato = 0,
                                .Data_Creazione = data,
                                .Data_Modifica = data,
                                .Validita_Inizio = AGRODATAINIZIO,
                                .Validita_Fine = AGRODATAFINE,
                                .Username_Creazione = username,
                                .Username_Modifica = username
                            }
                            GiasContext.ContattiXIndirizzi.Add(cxi)

                            ' nuovo recode indirizzo
                            Dim recode =
                                New G2G_Recode_Indirizzi With {
                                    .From_PivaSuperUser = To_PivaSuperUser,
                                    .To_PivaSuperUser = From_PivaSuperUser,
                                    .From_Cod_Indirizzo = idSeq,
                                    .To_Cod_Indirizzo = i.Indirizzo.cod_indirizzo,
                                    .Username_Creazione = username,
                                    .Username_Modifica = username,
                                    .Data_Creazione = Now(),
                                    .Data_Modifica = Now(),
                                    .Validita_Inizio = AGRODATAINIZIO,
                                    .Validita_Fine = AGRODATAFINE,
                                    .inviato = "0",
                                    .datainvio = Now()
                                }
                            g2g.Recode.G2GRecodeIndirizziToInsert.Add(recode)
                            GiasContext.G2G_Recode_Indirizzi.Add(recode)

                        Next

                        ' inserisco rubrica contatto
                        For Each cr As Rubrica In c.Rubrica

                            ' nuova rubrica
                            Dim idSeq = G2GUtility.NuovoId_Tabella(GiasContext, "Rubrica", 0, 2000000000, objParametri)
                            Dim rubrica = Gias_EF_Utility.CopyEntity(GiasContext, cr, Nothing, username, data)
                            rubrica.cod_rubrica = idSeq
                            GiasContext.Rubrica.Add(rubrica)

                            ' nuova rubrica contatto
                            Dim cxr = New ContattiXRubrica With {
                                .Piva = contatto.Piva,
                                .Sa_Cod = contatto.Sa_Cod,
                                .Cod_Contatto = contatto.Cod_Contatto,
                                .Cod_Rubrica = rubrica.cod_rubrica,
                                .Inviato = 0,
                                .Data_Creazione = data,
                                .Data_Modifica = data,
                                .Validita_Inizio = AGRODATAINIZIO,
                                .Validita_Fine = AGRODATAFINE,
                                .Username_Creazione = username,
                                .Username_Modifica = username
                            }
                            GiasContext.ContattiXRubrica.Add(cxr)

                        Next

                        If g2g.GestioneAllegati Then

                            ' inserisco allegati contatto
                            For Each e As Alert_Entita In c.Allegati.Entita

                                ' nuovo allegato documento
                                Dim doc_from = (From d In c.Allegati.Documenti Where d.Allegati_Documenti_Cod = e.Allegati_Documenti_Cod).FirstOrDefault()

                                If doc_from IsNot Nothing Then

                                    Dim documento = Gias_EF_Utility.CopyEntity(GiasContext, doc_from, Nothing, username, data)
                                    documento.Allegati_Documenti_SuperUser = To_PivaSuperUser
                                    documento.Allegati_Documenti_Piva = contatto.Piva
                                    GiasContext.Allegati_Documenti.Add(documento)
                                    GiasContext.SaveChanges()

                                    ' nuovo alert entita
                                    Dim idAlert = G2GUtility.NuovoId_Tabella(GiasContext, "Alert_Entita", 0, 2000000000, objParametri)
                                    Dim entita = Gias_EF_Utility.CopyEntity(GiasContext, e, Nothing, username, data)
                                    entita.PivaSuperUser = To_PivaSuperUser
                                    entita.Piva = contatto.Piva
                                    entita.ID_Alert_Entita = idAlert
                                    entita.Allegati_Documenti_Cod = documento.Allegati_Documenti_Cod
                                    GiasContext.Alert_Entita.Add(entita)

                                    ' nuovo alert elenco
                                    Dim scadenze = (From s In c.Allegati.Scadenze Where s.ID_Alert_Entita = e.ID_Alert_Entita).ToList()
                                    For Each s As Alert_Elenco In scadenze
                                        Dim idElenco = G2GUtility.NuovoId_Tabella(GiasContext, "Alert_Elenco", 0, 2000000000, objParametri)
                                        Dim scadenza = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)
                                        scadenza.PivaSuperUser = To_PivaSuperUser
                                        scadenza.ID_Alert_Entita = entita.ID_Alert_Entita
                                        scadenza.ID_Elenco = idElenco
                                        GiasContext.Alert_Elenco.Add(scadenza)
                                    Next

                                End If

                            Next

                        End If

                        ' inserisco risorse umane contatto
                        For Each r As Risorse_Umane In c.RisorseUmane

                            ' nuova risorsa numana
                            Dim idSeq = G2GUtility.NuovoId_Tabella(GiasContext, "Risorse_Umane", 0, 2000000000, objParametri)
                            Dim risorsa = Gias_EF_Utility.CopyEntity(GiasContext, r, Nothing, username, data)
                            risorsa.Piva = contatto.Piva
                            risorsa.Cod_RisUm = idSeq
                            GiasContext.Risorse_Umane.Add(risorsa)

                            ' inserisce costi contatto
                            For Each pc As Prodotti_Costi In c.Costi
                                If pc.Mat_Cod = r.Cod_RisUm Then
                                    Dim costo = Gias_EF_Utility.CopyEntity(GiasContext, pc, Nothing, username, data)
                                    costo.Piva = To_Piva
                                    costo.Mat_Cod = idSeq
                                    GiasContext.Prodotti_Costi.Add(costo)
                                End If
                            Next

                            ' nuovo recode contatto
                            Dim recode =
                                New G2G_Recode_Contatti With {
                                    .From_PivaSuperUser = To_PivaSuperUser,
                                    .To_PivaSuperUser = From_PivaSuperUser,
                                    .From_Cod_RisUm = idSeq,
                                    .To_Cod_Risum = r.Cod_RisUm,
                                    .Username_Creazione = username,
                                    .Username_Modifica = username,
                                    .Data_Creazione = Now(),
                                    .Data_Modifica = Now(),
                                    .Validita_Inizio = AGRODATAINIZIO,
                                    .Validita_Fine = AGRODATAFINE,
                                    .inviato = "0",
                                    .datainvio = Now()
                                }
                            g2g.Recode.G2GRecodeContattiToInsert.Add(recode)
                            GiasContext.G2G_Recode_Contatti.Add(recode)

                        Next

                        G2GUtility.SaveChanges(GiasContext, index)

                    Next

                    GiasContext.SaveChanges()

                End If

                ' CONTATTI DA MODIFICARE
                If g2g.ContattiToUpdate.Count > 0 Then

                    Dim index As Integer = 0

                    For Each c As G2G_Contatto In g2g.ContattiToUpdate

                        Dim contatto_pubblico As Boolean = String.IsNullOrEmpty(To_Piva) OrElse c.Contatto.Sa_Cod = -1
                        Dim contatto_piva = If(contatto_pubblico, c.Contatto.Piva, To_Piva)

                        Dim contatto = (From cc In GiasContext.Contatti Where cc.Piva = contatto_piva AndAlso cc.Cod_Contatto = c.Contatto.Cod_Contatto).FirstOrDefault()
                        Dim risorse = (From r In GiasContext.Risorse_Umane Where r.Piva = contatto.Piva AndAlso r.Cod_Contatto = contatto.Cod_Contatto Select r).ToList()

                        ' modifica contatto
                        contatto = Gias_EF_Utility.CopyEntity(GiasContext, c.Contatto, contatto, username, data)
                        contatto.Piva = contatto_piva
                        GiasContext.Contatti.Attach(contatto)
                        GiasContext.Entry(contatto).State = EntityState.Modified

                        Dim listaCodiciOK = New List(Of Integer)
                        Dim listaCodici = (From cc In GiasContext.Contatti_Codici Where cc.PIVA = contatto.Piva AndAlso cc.Cod_Contatto = contatto.Cod_Contatto Select cc).ToList()

                        ' inserisco / modifico codici contatto
                        For Each cc As Contatti_Codici In c.Codici
                            Dim codice = (From ccc In listaCodici Where ccc.Id_cod = cc.Id_cod Select ccc).FirstOrDefault()
                            If codice Is Nothing Then
                                codice = Gias_EF_Utility.CopyEntity(GiasContext, cc, Nothing, username, data)
                                codice.PIVA = contatto.Piva
                                GiasContext.Contatti_Codici.Add(codice)
                            Else
                                codice = Gias_EF_Utility.CopyEntity(GiasContext, cc, codice, username, data)
                                codice.PIVA = contatto.Piva
                                GiasContext.Contatti_Codici.Attach(codice)
                                GiasContext.Entry(codice).State = EntityState.Modified
                            End If
                            listaCodiciOK.Add(cc.Id_cod)
                        Next

                        ' cancello codici non presenti
                        Dim codiciKO = (From ccc In listaCodici Where Not listaCodiciOK.Contains(ccc.Id_cod) Select ccc).ToList()
                        For Each codice In codiciKO
                            GiasContext.Contatti_Codici.Attach(codice)
                            GiasContext.Contatti_Codici.Remove(codice)
                        Next

                        ' cancello indirizzi contatto
                        Dim contattixindirizzi = (From ci In GiasContext.ContattiXIndirizzi Where ci.Piva = contatto.Piva AndAlso ci.Cod_Contatto = contatto.Cod_Contatto Select ci).ToList()
                        For Each cxi As ContattiXIndirizzi In contattixindirizzi
                            GiasContext.ContattiXIndirizzi.Attach(cxi)
                            GiasContext.ContattiXIndirizzi.Remove(cxi)
                        Next

                        ' inserisco / modifico indirizzi contatto
                        For Each i As G2G_Indirizzo In c.Indirizzi

                            Dim recode = (From rr In GiasContext.G2G_Recode_Indirizzi Where rr.To_PivaSuperUser = From_PivaSuperUser AndAlso rr.From_PivaSuperUser = To_PivaSuperUser AndAlso rr.To_Cod_Indirizzo = i.Indirizzo.cod_indirizzo).FirstOrDefault()
                            Dim indirizzo = If(recode Is Nothing, Nothing, (From ii In GiasContext.Indirizzi Where ii.cod_indirizzo = recode.To_Cod_Indirizzo Select ii).FirstOrDefault())

                            If indirizzo Is Nothing Then

                                ' nuovo indirizzo
                                Dim idSeq = G2GUtility.NuovoId_Tabella(GiasContext, "Indirizzi", 0, 2000000000, objParametri)
                                indirizzo = Gias_EF_Utility.CopyEntity(GiasContext, i.Indirizzo, Nothing, username, data)
                                indirizzo.cod_indirizzo = idSeq
                                GiasContext.Indirizzi.Add(indirizzo)

                                ' nuovo recode indirizzi
                                Dim ri =
                                    New G2G_Recode_Indirizzi With {
                                        .From_PivaSuperUser = To_PivaSuperUser,
                                        .To_PivaSuperUser = From_PivaSuperUser,
                                        .From_Cod_Indirizzo = indirizzo.cod_indirizzo,
                                        .To_Cod_Indirizzo = i.Indirizzo.cod_indirizzo,
                                        .Username_Creazione = username,
                                        .Username_Modifica = username,
                                        .Data_Creazione = Now(),
                                        .Data_Modifica = Now(),
                                        .Validita_Inizio = AGRODATAINIZIO,
                                        .Validita_Fine = AGRODATAFINE,
                                        .inviato = "0",
                                        .datainvio = Now()
                                    }
                                g2g.Recode.G2GRecodeIndirizziToInsert.Add(ri)
                                GiasContext.G2G_Recode_Indirizzi.Add(ri)

                            Else

                                ' modifica indirizzo
                                indirizzo = Gias_EF_Utility.CopyEntity(GiasContext, i.Indirizzo, indirizzo, username, data)
                                indirizzo.cod_indirizzo = recode.To_Cod_Indirizzo
                                GiasContext.Indirizzi.Attach(indirizzo)
                                GiasContext.Entry(indirizzo).State = EntityState.Modified

                                ' modifica recode indirizzo
                                recode.Username_Modifica = username
                                recode.Data_Modifica = data
                                recode.datainvio = data
                                g2g.Recode.G2GRecodeIndirizziToUpdate.Add(recode)
                                GiasContext.G2G_Recode_Indirizzi.Attach(recode)
                                GiasContext.Entry(recode).State = EntityState.Modified

                            End If

                            ' nuovo indirizzo contatto
                            Dim cxi = New ContattiXIndirizzi With {
                                .Piva = contatto.Piva,
                                .Sa_Cod = contatto.Sa_Cod,
                                .Cod_Contatto = contatto.Cod_Contatto,
                                .Cod_Indirizzo = indirizzo.cod_indirizzo,
                                .Tipo_Indirizzo = i.Tipo_Indirizzo,
                                .Inviato = 0,
                                .Data_Creazione = data,
                                .Data_Modifica = data,
                                .Validita_Inizio = AGRODATAINIZIO,
                                .Validita_Fine = AGRODATAFINE,
                                .Username_Creazione = username,
                                .Username_Modifica = username
                            }
                            GiasContext.ContattiXIndirizzi.Add(cxi)

                        Next

                        ' cancello rubrica contatto
                        Dim contattixrubrica = (From cr In GiasContext.ContattiXRubrica Where cr.Piva = contatto.Piva AndAlso cr.Cod_Contatto = contatto.Cod_Contatto Select cr).ToList()
                        For Each cxr As ContattiXRubrica In contattixrubrica
                            Dim rubrica = (From r In GiasContext.Rubrica Where r.cod_rubrica = cxr.Cod_Rubrica Select r).FirstOrDefault()
                            GiasContext.ContattiXRubrica.Attach(cxr)
                            GiasContext.ContattiXRubrica.Remove(cxr)

                            If rubrica IsNot Nothing Then
                                GiasContext.Rubrica.Attach(rubrica)
                                GiasContext.Rubrica.Remove(rubrica)
                            End If
                        Next

                        ' inserisco rubrica contatto
                        For Each cr As Rubrica In c.Rubrica

                            Dim idSeq = G2GUtility.NuovoId_Tabella(GiasContext, "Rubrica", 0, 2000000000, objParametri)
                            Dim rubrica = Gias_EF_Utility.CopyEntity(GiasContext, cr, Nothing, username, data)
                            rubrica.cod_rubrica = idSeq
                            GiasContext.Rubrica.Add(rubrica)

                            Dim cxr = New ContattiXRubrica With {
                                .Piva = contatto.Piva,
                                .Sa_Cod = contatto.Sa_Cod,
                                .Cod_Contatto = contatto.Cod_Contatto,
                                .Cod_Rubrica = rubrica.cod_rubrica,
                                .Inviato = 0,
                                .Data_Creazione = data,
                                .Data_Modifica = data,
                                .Validita_Inizio = AGRODATAINIZIO,
                                .Validita_Fine = AGRODATAFINE,
                                .Username_Creazione = username,
                                .Username_Modifica = username
                            }
                            GiasContext.ContattiXRubrica.Add(cxr)

                        Next

                        If g2g.GestioneAllegati Then

                            ' cancello allegati contatto
                            Dim alert_entita = (From e In GiasContext.Alert_Entita Where e.PivaSuperUser = To_PivaSuperUser AndAlso e.Piva = contatto.Piva AndAlso e.Cod_Contatto = contatto.Cod_Contatto Select e).ToList()
                            For Each e As Alert_Entita In alert_entita

                                Dim scadenze = (From s In GiasContext.Alert_Elenco Where s.PivaSuperUser = e.PivaSuperUser AndAlso s.ID_Alert_Entita = e.ID_Alert_Entita Select s).ToList()
                                For Each scadenza As Alert_Elenco In scadenze
                                    GiasContext.Alert_Elenco.Attach(scadenza)
                                    GiasContext.Alert_Elenco.Remove(scadenza)
                                Next

                                Dim documento = (From d In GiasContext.Allegati_Documenti Where d.Allegati_Documenti_SuperUser = e.PivaSuperUser AndAlso d.Allegati_Documenti_Piva = e.Piva AndAlso d.Allegati_Documenti_Cod = e.Allegati_Documenti_Cod Select d).FirstOrDefault()
                                If documento IsNot Nothing Then
                                    GiasContext.Allegati_Documenti.Attach(documento)
                                    GiasContext.Allegati_Documenti.Remove(documento)
                                End If

                                GiasContext.Alert_Entita.Attach(e)
                                GiasContext.Alert_Entita.Remove(e)

                            Next

                            ' inserisco allegati contatto
                            For Each e As Alert_Entita In c.Allegati.Entita

                                ' nuovo allegato documento
                                Dim doc_from = (From d In c.Allegati.Documenti Where d.Allegati_Documenti_Cod = e.Allegati_Documenti_Cod).FirstOrDefault()

                                If doc_from IsNot Nothing Then

                                    Dim documento = Gias_EF_Utility.CopyEntity(GiasContext, doc_from, Nothing, username, data)
                                    documento.Allegati_Documenti_SuperUser = To_PivaSuperUser
                                    documento.Allegati_Documenti_Piva = contatto.Piva
                                    GiasContext.Allegati_Documenti.Add(documento)
                                    GiasContext.SaveChanges()

                                    ' nuovo alert entita
                                    Dim idAlert = G2GUtility.NuovoId_Tabella(GiasContext, "Alert_Entita", 0, 2000000000, objParametri)
                                    Dim entita = Gias_EF_Utility.CopyEntity(GiasContext, e, Nothing, username, data)
                                    entita.PivaSuperUser = To_PivaSuperUser
                                    entita.Piva = contatto.Piva
                                    entita.ID_Alert_Entita = idAlert
                                    entita.Allegati_Documenti_Cod = documento.Allegati_Documenti_Cod
                                    GiasContext.Alert_Entita.Add(entita)

                                    ' nuovo alert elenco
                                    Dim scadenze = (From s In c.Allegati.Scadenze Where s.ID_Alert_Entita = e.ID_Alert_Entita).ToList()
                                    For Each s As Alert_Elenco In scadenze
                                        Dim idElenco = G2GUtility.NuovoId_Tabella(GiasContext, "Alert_Elenco", 0, 2000000000, objParametri)
                                        Dim scadenza = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)
                                        scadenza.PivaSuperUser = To_PivaSuperUser
                                        scadenza.ID_Alert_Entita = entita.ID_Alert_Entita
                                        scadenza.ID_Elenco = idElenco
                                        GiasContext.Alert_Elenco.Add(scadenza)
                                    Next

                                End If

                            Next

                        End If

                        ' modifico risorse umane contatto
                        Dim listaRisorseOK = New List(Of Integer)
                        For Each r As Risorse_Umane In c.RisorseUmane

                            Dim recode = (From rr In GiasContext.G2G_Recode_Contatti Where rr.To_PivaSuperUser = From_PivaSuperUser AndAlso rr.From_PivaSuperUser = To_PivaSuperUser AndAlso rr.To_Cod_Risum = r.Cod_RisUm).FirstOrDefault()
                            Dim risorsa = If(recode Is Nothing, Nothing, (From rr In GiasContext.Risorse_Umane Where rr.Cod_RisUm = recode.From_Cod_RisUm).FirstOrDefault())

                            ' creo la risorsa se non è presente 
                            If risorsa Is Nothing Then

                                ' nuova risorsa umana contatto
                                Dim idSeq = G2GUtility.NuovoId_Tabella(GiasContext, "Risorse_Umane", 0, 2000000000, objParametri)
                                risorsa = Gias_EF_Utility.CopyEntity(GiasContext, r, Nothing, username, data)
                                risorsa.Piva = contatto_piva
                                risorsa.Cod_RisUm = idSeq
                                GiasContext.Risorse_Umane.Add(risorsa)

                                ' inserisce costi contatto
                                For Each pc As Prodotti_Costi In c.Costi
                                    If pc.Mat_Cod = r.Cod_RisUm Then
                                        Dim costo = Gias_EF_Utility.CopyEntity(GiasContext, pc, Nothing, username, data)
                                        costo.Piva = contatto_piva
                                        costo.Mat_Cod = idSeq
                                        GiasContext.Prodotti_Costi.Add(costo)
                                    End If
                                Next

                                ' nuovo recode contatto
                                recode =
                                    New G2G_Recode_Contatti With {
                                        .From_PivaSuperUser = To_PivaSuperUser,
                                        .To_PivaSuperUser = From_PivaSuperUser,
                                        .From_Cod_RisUm = idSeq,
                                        .To_Cod_Risum = r.Cod_RisUm,
                                        .Username_Creazione = username,
                                        .Username_Modifica = username,
                                        .Data_Creazione = Now(),
                                        .Data_Modifica = Now(),
                                        .Validita_Inizio = AGRODATAINIZIO,
                                        .Validita_Fine = AGRODATAFINE,
                                        .inviato = "0",
                                        .datainvio = Now()
                                    }
                                g2g.Recode.G2GRecodeContattiToInsert.Add(recode)
                                GiasContext.G2G_Recode_Contatti.Add(recode)

                            Else

                                If Not listaRisorseOK.Contains(risorsa.Cod_RisUm) Then

                                    ' modifica risorsa contatto
                                    risorsa = Gias_EF_Utility.CopyEntity(GiasContext, r, risorsa, username, data)
                                    risorsa.Piva = contatto_piva
                                    risorsa.Cod_RisUm = recode.From_Cod_RisUm
                                    GiasContext.Risorse_Umane.Attach(risorsa)
                                    GiasContext.Entry(risorsa).State = EntityState.Modified

                                    ' cancella costi contatto
                                    Dim costi = (From pc In GiasContext.Prodotti_Costi Where pc.Piva = risorsa.Piva And pc.Mat_Cod = risorsa.Cod_RisUm And pc.Elem_Cod = 0 And pc.Id_Budget = 0 Select pc).ToList()
                                    For Each pc As Prodotti_Costi In costi
                                        GiasContext.Prodotti_Costi.Attach(pc)
                                        GiasContext.Prodotti_Costi.Remove(pc)
                                    Next

                                    ' inserisce costi contatto
                                    For Each pc As Prodotti_Costi In c.Costi
                                        If pc.Mat_Cod = r.Cod_RisUm Then
                                            Dim costo = Gias_EF_Utility.CopyEntity(GiasContext, pc, Nothing, username, data)
                                            costo.Piva = contatto_piva
                                            costo.Mat_Cod = recode.From_Cod_RisUm
                                            GiasContext.Prodotti_Costi.Add(costo)
                                        End If
                                    Next

                                End If

                                ' modifica recode contatto
                                recode.Username_Modifica = username
                                recode.Data_Modifica = data
                                recode.datainvio = data
                                g2g.Recode.G2GRecodeContattiToUpdate.Add(recode)
                                GiasContext.G2G_Recode_Contatti.Attach(recode)
                                GiasContext.Entry(recode).State = EntityState.Modified

                            End If

                            listaRisorseOK.Add(risorsa.Cod_RisUm)

                        Next

                        ' cancello risorse contatto eliminate
                        For Each risorsa In risorse

                            If Not listaRisorseOK.Contains(risorsa.Cod_RisUm) Then

                                ' cancella costi contatto
                                Dim costi = (From pc In GiasContext.Prodotti_Costi Where pc.Piva = risorsa.Piva And pc.Mat_Cod = risorsa.Cod_RisUm And pc.Elem_Cod = 0 And pc.Id_Budget = 0 Select pc).ToList()
                                For Each pc As Prodotti_Costi In costi
                                    GiasContext.Prodotti_Costi.Attach(pc)
                                    GiasContext.Prodotti_Costi.Remove(pc)
                                Next

                                ' cancella risorsa contatto
                                GiasContext.Risorse_Umane.Attach(risorsa)
                                GiasContext.Risorse_Umane.Remove(risorsa)

                                ' cancella recode contatto
                                Dim recode = (From rr In GiasContext.G2G_Recode_Contatti Where rr.To_PivaSuperUser = To_PivaSuperUser AndAlso rr.To_Cod_Risum = risorsa.Cod_RisUm).FirstOrDefault()
                                If recode IsNot Nothing Then
                                    g2g.Recode.G2GRecodeContattiToDelete.Add(recode)
                                    GiasContext.G2G_Recode_Contatti.Attach(recode)
                                    GiasContext.G2G_Recode_Contatti.Remove(recode)
                                End If
                            End If

                        Next

                        G2GUtility.SaveChanges(GiasContext, index)

                    Next

                    GiasContext.SaveChanges()

                End If

                ' log importazione
                g2g.Recode.LogRecode = g2g.ContattiToInsert.Count & " nuovi, " & g2g.ContattiToUpdate.Count & " modificati, " & g2g.Recode.G2GRecodeContattiToDelete.Count & " cancellati"

            End Using


        Catch ex As Exception

            messaggioErrore = ex.Message & If(IsNothing(ex.InnerException), "", vbCrLf & ex.InnerException.Message)
            g2g.Recode.MessaggioErrore = messaggioErrore
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)

        End Try


        Return g2g.Recode

    End Function

End Class