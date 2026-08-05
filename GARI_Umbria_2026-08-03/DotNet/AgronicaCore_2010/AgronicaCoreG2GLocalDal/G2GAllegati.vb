Imports System.Transactions

Imports Newtonsoft.Json

Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModello
Imports Newtonsoft.Json.Linq
Imports System.Data.Entity

Public Class G2GAllegati_R

    Public Function LeggiPerGias2Gias(
        ByVal objOpzioniImportImpresa As clsImpresa,
        ByVal piva As String,
        ByVal Piva_Destinazione As String,
        ByVal PivaSuperUser_Destinazione As String,
        ByRef objParametri As AgronicaCoreParametri
    ) As G2G_Allegati

        Dim NomeRoutine As String = "G2GlocalDal.G2GAllegati_R.LeggiPerGias2Gias()"

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
        GiasContext.Database.CommandTimeout = 3600
        Dim oConfigurazione As G2G_Configurazione_FiltriReq_Allegati
        If objOpzioniImportImpresa.configurazione_Allegati <> "" Then
            oConfigurazione = JsonConvert.DeserializeObject(Of G2G_Configurazione_FiltriReq_Allegati)(objOpzioniImportImpresa.configurazione_Allegati)
        Else
            oConfigurazione = New G2G_Configurazione_FiltriReq_Allegati
        End If
        If oConfigurazione.listaAllegati_Documenti_CatCod Is Nothing Then
            oConfigurazione.listaAllegati_Documenti_CatCod = New List(Of Integer)
        End If
        If oConfigurazione.listaAlert_TipoEntita_Cod Is Nothing Then
            oConfigurazione.listaAlert_TipoEntita_Cod = New List(Of Integer)
        End If

        Dim rval As New G2G_Allegati
        rval.alert_tipo_entita_cod = oConfigurazione.listaAlert_TipoEntita_Cod

        'quanto sicuramente non assegnato ...
        With rval
            .From_Piva = piva
            .From_PivaSuperUser = objParametri.PivaSuperUser
            .G2G_Allegati_Recode_insert = New List(Of G2G_Recode_Allegati)
            .G2G_Allegati_Recode_update = New List(Of G2G_Recode_Allegati)
            .G2G_Allegati_Entita_Recode_insert = New List(Of G2G_Recode_Allegati_Entita)
            .G2G_Allegati_Entita_Recode_update = New List(Of G2G_Recode_Allegati_Entita)
            .allegati_delete = New List(Of Allegati_Documenti)
            .allegati_entita_delete = New List(Of Allegati_EntitaxDocumenti)
            .To_Piva = Piva_Destinazione
        End With

        'assegnazioni, pratiche
        rval.allegati_insert = (
            From p In GiasContext.Allegati_Documenti
            Where p.Allegati_Documenti_Piva = piva _
                AndAlso p.Validita_Inizio >= objOpzioniImportImpresa.ValiditaInizio_allegati _
                AndAlso (oConfigurazione.listaAllegati_Documenti_CatCod.Count = 0 OrElse oConfigurazione.listaAllegati_Documenti_CatCod.Contains(p.Allegati_Documenti_CatCod)) _
                AndAlso Not GiasContext.G2G_Recode_Allegati.Any(Function(g) g.From_PivaSuperUser = p.Allegati_Documenti_SuperUser AndAlso g.To_PivaSuperUser = PivaSuperUser_Destinazione AndAlso g.From_Allegati_Documenti_Cod = p.Allegati_Documenti_Cod)
            Select p).ToList()

        Dim listaAllegatiInsert As List(Of Integer) = (From a In rval.allegati_insert Select a.Allegati_Documenti_Cod).ToList()

        rval.alert_entita_insert = (
            From e In GiasContext.Alert_Entita
            Where oConfigurazione.listaAlert_TipoEntita_Cod.Contains(e.TipoEntita_Cod) AndAlso listaAllegatiInsert.Contains(e.Allegati_Documenti_Cod) Select e).ToList()

        rval.alert_elenco_insert = (
            From s In GiasContext.Alert_Elenco
            Join e In GiasContext.Alert_Entita
                On s.PivaSuperUser Equals e.PivaSuperUser And s.ID_Alert_Entita Equals e.ID_Alert_Entita
            Where oConfigurazione.listaAlert_TipoEntita_Cod.Contains(e.TipoEntita_Cod) AndAlso listaAllegatiInsert.Contains(e.Allegati_Documenti_Cod) Select s).ToList()

        rval.allegati_update = (
            From p In GiasContext.Allegati_Documenti
            Join g2g In GiasContext.G2G_Recode_Allegati
                On p.Allegati_Documenti_Cod Equals g2g.From_Allegati_Documenti_Cod _
                And p.Allegati_Documenti_SuperUser Equals g2g.From_PivaSuperUser
            Where p.Allegati_Documenti_Piva = piva _
                AndAlso g2g.Data_Modifica < p.Data_Modifica
            Select p
            ).ToList()

        Dim listaAllegatiUpdate As List(Of Integer) = (From a In rval.allegati_update Select a.Allegati_Documenti_Cod).ToList()

        rval.alert_entita_update = (
            From e In GiasContext.Alert_Entita
            Where oConfigurazione.listaAlert_TipoEntita_Cod.Contains(e.TipoEntita_Cod) AndAlso listaAllegatiUpdate.Contains(e.Allegati_Documenti_Cod) Select e).ToList()

        rval.alert_elenco_update = (
            From s In GiasContext.Alert_Elenco
            Join e In GiasContext.Alert_Entita
                On s.PivaSuperUser Equals e.PivaSuperUser And s.ID_Alert_Entita Equals e.ID_Alert_Entita
            Where oConfigurazione.listaAlert_TipoEntita_Cod.Contains(e.TipoEntita_Cod) AndAlso listaAllegatiUpdate.Contains(e.Allegati_Documenti_Cod) Select s).ToList()

        rval.G2G_Allegati_Recode_delete = (
            From r In GiasContext.G2G_Recode_Allegati
            Where Not GiasContext.Allegati_Documenti.Any(Function(p) p.Allegati_Documenti_Cod = r.From_Allegati_Documenti_Cod AndAlso p.Allegati_Documenti_SuperUser = r.From_PivaSuperUser) _
                AndAlso r.To_PivaSuperUser = PivaSuperUser_Destinazione
            Select r).ToList()

        '----------------------
        'assegnazioni, stati
        rval.allegati_entita_insert = (
            From p In GiasContext.Allegati_Documenti
            Join s In GiasContext.Allegati_EntitaxDocumenti On
                p.Allegati_Documenti_SuperUser Equals s.Allegati_Documenti_SuperUser And
                p.Allegati_Documenti_Cod Equals s.Allegati_Documenti_Cod
            Where p.Allegati_Documenti_Piva = piva _
                AndAlso p.Validita_Inizio >= objOpzioniImportImpresa.ValiditaInizio_allegati _
                AndAlso (oConfigurazione.listaAllegati_Documenti_CatCod.Count = 0 OrElse oConfigurazione.listaAllegati_Documenti_CatCod.Contains(p.Allegati_Documenti_CatCod)) _
                AndAlso Not GiasContext.G2G_Recode_Allegati_Entita.Any(Function(g) g.From_PivaSuperUser = p.Allegati_Documenti_SuperUser AndAlso g.To_PivaSuperUser = PivaSuperUser_Destinazione AndAlso g.From_ID = s.ID)
            Select s).ToList()


        rval.allegati_entita_update = (
             From p In GiasContext.Allegati_Documenti
             Join s In GiasContext.Allegati_EntitaxDocumenti On
                p.Allegati_Documenti_SuperUser Equals s.Allegati_Documenti_SuperUser And
                p.Allegati_Documenti_Cod Equals s.Allegati_Documenti_Cod
             Join g2g In GiasContext.G2G_Recode_Allegati_Entita
                On s.ID Equals g2g.From_ID _
                And s.Allegati_Documenti_SuperUser Equals g2g.From_PivaSuperUser
             Where p.Allegati_Documenti_Piva = piva _
                AndAlso g2g.Data_Modifica < s.Data_Modifica
             Select s
            ).ToList()


        rval.G2G_Allegati_Entita_Recode_delete = (
            From r In GiasContext.G2G_Recode_Allegati_Entita
            Where Not GiasContext.Allegati_EntitaxDocumenti.Any(Function(p) p.ID = r.From_ID AndAlso p.Allegati_Documenti_SuperUser = r.From_PivaSuperUser) _
                AndAlso r.To_PivaSuperUser = PivaSuperUser_Destinazione
            Select r).ToList()

        Return rval

    End Function

    Public Function LeggiPerGias2GiasReverse(
        ByVal objOpzioniImportImpresa As clsImpresa,
        ByVal piva As String,
        ByVal Piva_Destinazione As String,
        ByVal PivaSuperUser_Destinazione As String,
        ByRef objParametri As AgronicaCoreParametri
    ) As G2G_Allegati_Reverse

        Dim NomeRoutine As String = "G2GlocalDal.G2GAllegati_R.LeggiPerGias2GiasReverse()"

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
        GiasContext.Database.CommandTimeout = 3600
        Dim oConfigurazione As G2G_Configurazione_FiltriReq_Allegati
        If objOpzioniImportImpresa.configurazione_Allegati <> "" Then
            oConfigurazione = JsonConvert.DeserializeObject(Of G2G_Configurazione_FiltriReq_Allegati)(objOpzioniImportImpresa.configurazione_Allegati)
        Else
            oConfigurazione = New G2G_Configurazione_FiltriReq_Allegati
        End If
        If oConfigurazione.listaAllegati_Documenti_CatCod Is Nothing Then
            oConfigurazione.listaAllegati_Documenti_CatCod = New List(Of Integer)
        End If
        If oConfigurazione.listaAlert_TipoEntita_Cod Is Nothing Then
            oConfigurazione.listaAlert_TipoEntita_Cod = New List(Of Integer)
        End If

        Dim rval As New G2G_Allegati_Reverse
        rval.alert_tipo_entita_cod = oConfigurazione.listaAlert_TipoEntita_Cod

        'quanto sicuramente non assegnato ...
        With rval
            .From_Piva = piva
            .From_PivaSuperUser = objParametri.PivaSuperUser
            .G2G_Allegati_Recode_insert = New List(Of G2G_Recode_Allegati)
            .G2G_Allegati_Recode_update = New List(Of G2G_Recode_Allegati)
            .G2G_Allegati_Entita_Recode_insert = New List(Of G2G_Recode_Allegati_Entita)
            .G2G_Allegati_Entita_Recode_update = New List(Of G2G_Recode_Allegati_Entita)
            .allegati_delete = New List(Of Allegati_Documenti)
            .allegati_entita_delete = New List(Of Allegati_EntitaxDocumenti)
            .To_Piva = Piva_Destinazione
        End With

        'assegnazioni, pratiche
        rval.allegati_insert = (
            From p In GiasContext.Allegati_Documenti
            Where p.Allegati_Documenti_Piva = piva _
                AndAlso p.Validita_Inizio >= objOpzioniImportImpresa.ValiditaInizio_allegati _
                AndAlso (oConfigurazione.listaAllegati_Documenti_CatCod.Count = 0 OrElse oConfigurazione.listaAllegati_Documenti_CatCod.Contains(p.Allegati_Documenti_CatCod)) _
                AndAlso Not GiasContext.G2G_Recode_Allegati.Any(Function(g) g.To_PivaSuperUser = p.Allegati_Documenti_SuperUser AndAlso g.From_PivaSuperUser = PivaSuperUser_Destinazione AndAlso g.To_Allegati_Documenti_Cod = p.Allegati_Documenti_Cod)
            Select p).ToList()

        Dim listaAllegatiInsert As List(Of Integer) = (From a In rval.allegati_insert Select a.Allegati_Documenti_Cod).ToList()

        rval.alert_entita_insert = (
            From e In GiasContext.Alert_Entita
            Where oConfigurazione.listaAlert_TipoEntita_Cod.Contains(e.TipoEntita_Cod) AndAlso listaAllegatiInsert.Contains(e.Allegati_Documenti_Cod) Select e).ToList()

        rval.alert_elenco_insert = (
            From s In GiasContext.Alert_Elenco
            Join e In GiasContext.Alert_Entita
                On s.PivaSuperUser Equals e.PivaSuperUser And s.ID_Alert_Entita Equals e.ID_Alert_Entita
            Where oConfigurazione.listaAlert_TipoEntita_Cod.Contains(e.TipoEntita_Cod) AndAlso listaAllegatiInsert.Contains(e.Allegati_Documenti_Cod) Select s).ToList()

        rval.allegati_update = (
            From p In GiasContext.Allegati_Documenti
            Join g2g In GiasContext.G2G_Recode_Allegati
                On p.Allegati_Documenti_Cod Equals g2g.To_Allegati_Documenti_Cod _
                And p.Allegati_Documenti_SuperUser Equals g2g.To_PivaSuperUser
            Where p.Allegati_Documenti_Piva = piva _
                AndAlso g2g.From_PivaSuperUser = PivaSuperUser_Destinazione _
                AndAlso g2g.Data_Modifica < p.Data_Modifica
            Select p
            ).ToList()

        Dim listaAllegatiUpdate As List(Of Integer) = (From a In rval.allegati_update Select a.Allegati_Documenti_Cod).ToList()

        rval.alert_entita_update = (
            From e In GiasContext.Alert_Entita
            Where oConfigurazione.listaAlert_TipoEntita_Cod.Contains(e.TipoEntita_Cod) AndAlso listaAllegatiUpdate.Contains(e.Allegati_Documenti_Cod) Select e).ToList()

        rval.alert_elenco_update = (
            From s In GiasContext.Alert_Elenco
            Join e In GiasContext.Alert_Entita
                On s.PivaSuperUser Equals e.PivaSuperUser And s.ID_Alert_Entita Equals e.ID_Alert_Entita
            Where oConfigurazione.listaAlert_TipoEntita_Cod.Contains(e.TipoEntita_Cod) AndAlso listaAllegatiUpdate.Contains(e.Allegati_Documenti_Cod) Select s).ToList()

        rval.G2G_Allegati_Recode_delete = (
            From r In GiasContext.G2G_Recode_Allegati
            Where Not GiasContext.Allegati_Documenti.Any(Function(p) p.Allegati_Documenti_Cod = r.To_Allegati_Documenti_Cod _
                AndAlso p.Allegati_Documenti_SuperUser = r.To_PivaSuperUser) _
                AndAlso r.From_PivaSuperUser = PivaSuperUser_Destinazione
            Select r).ToList()

        '----------------------
        'assegnazioni, stati
        rval.allegati_entita_insert = (
            From p In GiasContext.Allegati_Documenti
            Join s In GiasContext.Allegati_EntitaxDocumenti On
                p.Allegati_Documenti_SuperUser Equals s.Allegati_Documenti_SuperUser And
                p.Allegati_Documenti_Cod Equals s.Allegati_Documenti_Cod
            Where p.Allegati_Documenti_Piva = piva _
                AndAlso p.Validita_Inizio >= objOpzioniImportImpresa.ValiditaInizio_allegati _
                AndAlso (oConfigurazione.listaAllegati_Documenti_CatCod.Count = 0 OrElse oConfigurazione.listaAllegati_Documenti_CatCod.Contains(p.Allegati_Documenti_CatCod)) _
                AndAlso Not GiasContext.G2G_Recode_Allegati_Entita.Any(Function(g) g.To_PivaSuperUser = p.Allegati_Documenti_SuperUser AndAlso g.From_PivaSuperUser = PivaSuperUser_Destinazione AndAlso g.To_ID = s.ID)
            Select s).ToList()


        rval.allegati_entita_update = (
             From p In GiasContext.Allegati_Documenti
             Join s In GiasContext.Allegati_EntitaxDocumenti On
                p.Allegati_Documenti_SuperUser Equals s.Allegati_Documenti_SuperUser And
                p.Allegati_Documenti_Cod Equals s.Allegati_Documenti_Cod
             Join g2g In GiasContext.G2G_Recode_Allegati_Entita
                On s.ID Equals g2g.From_ID _
                And s.Allegati_Documenti_SuperUser Equals g2g.To_PivaSuperUser
             Where p.Allegati_Documenti_Piva = piva _
                AndAlso g2g.From_PivaSuperUser = PivaSuperUser_Destinazione _
                AndAlso g2g.Data_Modifica < s.Data_Modifica
             Select s
            ).ToList()


        rval.G2G_Allegati_Entita_Recode_delete = (
            From r In GiasContext.G2G_Recode_Allegati_Entita
            Where Not GiasContext.Allegati_EntitaxDocumenti.Any(Function(p) p.ID = r.To_ID AndAlso p.Allegati_Documenti_SuperUser = r.To_PivaSuperUser) _
                AndAlso r.From_PivaSuperUser = PivaSuperUser_Destinazione
            Select r).ToList()

        Return rval

    End Function

End Class


Public Class G2GAllegati_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi_Allegati_G2G(Origine_Piva_SuperUser As String, Destinazione_Piva_SuperUser As String, ByRef g2g As G2G_Allegati, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDal.G2GAllegati_W.Scrivi_Allegati_G2G()"
        Dim messaggioErrore As String = ""
        g2g.Recode = New G2G_Recode

        Dim username = objParametri.UsernameOperazione
        Dim piva = g2g.To_Piva
        Dim data = Date.Now
        Dim listaAlert_TipoEntita_Cod = g2g.alert_tipo_entita_cod
        Dim gestione_alert_entita As Boolean = listaAlert_TipoEntita_Cod.Count > 0

        Dim objSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try

            Dim transactionOptions As New TransactionOptions()
            transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            transactionOptions.Timeout = G2GUtility.GetTransactionTimeout(objParametri)

            Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    For Each m As Allegati_Documenti In g2g.allegati_insert

                        Dim allegati = Gias_EF_Utility.CopyEntity(GiasContext, m, Nothing, username, data)
                        allegati.Allegati_Documenti_Piva = piva
                        'allegati.Allegati_Documenti_Cod = generato automaticamente
                        allegati.Allegati_Documenti_SuperUser = Destinazione_Piva_SuperUser
                        GiasContext.Allegati_Documenti.Add(allegati)
                        GiasContext.SaveChanges() 'Da lasciare perché la chiave è un autoincrementale e non viene assegnato finhcé non viene scritto. Ci serve per il recode.

                        If gestione_alert_entita Then

                            ' inserisco allegati contatto
                            Dim alert_entita_insert = (From d In g2g.alert_entita_insert Where d.Allegati_Documenti_Cod = m.Allegati_Documenti_Cod).ToList()

                            For Each e As Alert_Entita In alert_entita_insert

                                ' nuovo alert entita
                                Dim idAlert = objSequenze.NuovoId_Tabella("Alert_Entita", 0, 2000000000, objParametri, UtilizzaTransazione:=False)
                                Dim entita = Gias_EF_Utility.CopyEntity(GiasContext, e, Nothing, username, data)
                                entita.PivaSuperUser = Destinazione_Piva_SuperUser
                                entita.Piva = allegati.Allegati_Documenti_Piva
                                entita.ID_Alert_Entita = idAlert
                                entita.Allegati_Documenti_Cod = allegati.Allegati_Documenti_Cod

                                If entita.Analisi_Testata_Cod <> 0 Then
                                    Dim entita_old = entita.Analisi_Testata_Cod
                                    Dim objAnalisi = (From g2ga In GiasContext.G2G_Recode_Analisi_Testata
                                                      Where g2ga.From_Analisi_Testata_Cod = entita_old _
                                                                      And g2ga.From_PivaSuperUser = Origine_Piva_SuperUser _
                                                                      And g2ga.To_PivaSuperUser = Destinazione_Piva_SuperUser).FirstOrDefault

                                    If objAnalisi IsNot Nothing Then
                                        entita.Analisi_Testata_Cod = objAnalisi.To_Analisi_Testata_Cod
                                    Else
                                        Throw New Exception("Analisi Testata Cod " & CStr(entita.Analisi_Testata_Cod) & " non trovata in Alert_Entita ")
                                    End If

                                End If


                                GiasContext.Alert_Entita.Add(entita)

                                ' nuovo alert elenco
                                Dim scadenze = (From s In g2g.alert_elenco_insert Where s.ID_Alert_Entita = e.ID_Alert_Entita).ToList()
                                For Each s As Alert_Elenco In scadenze
                                    Dim idElenco = objSequenze.NuovoId_Tabella("Alert_Elenco", 0, 2000000000, objParametri, UtilizzaTransazione:=False)
                                    Dim scadenza = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)
                                    scadenza.PivaSuperUser = Destinazione_Piva_SuperUser
                                    scadenza.ID_Alert_Entita = entita.ID_Alert_Entita
                                    scadenza.ID_Elenco = idElenco
                                    GiasContext.Alert_Elenco.Add(scadenza)
                                Next

                            Next

                        End If

                        Dim recode =
                            New G2G_Recode_Allegati With {
                                .From_PivaSuperUser = Origine_Piva_SuperUser,
                                .To_PivaSuperUser = Destinazione_Piva_SuperUser,
                                .From_Allegati_Documenti_Cod = m.Allegati_Documenti_Cod,
                                .To_Allegati_Documenti_Cod = allegati.Allegati_Documenti_Cod,
                                .Username_Creazione = username,
                                .Username_Modifica = username,
                                .Data_Creazione = Now(),
                                .Data_Modifica = Now(),
                                .Validita_Inizio = AGRODATAINIZIO,
                                .Validita_Fine = AGRODATAFINE,
                                .inviato = "0",
                                .datainvio = Now()
                            }
                        g2g.G2G_Allegati_Recode_insert.Add(recode)
                        GiasContext.G2G_Recode_Allegati.Add(recode)

                        'GiasContext.SaveChanges()

                    Next

                    GiasContext.SaveChanges()

                    Dim list_entita_insert As New List(Of Allegati_EntitaxDocumenti)

                    For Each s As Allegati_EntitaxDocumenti In g2g.allegati_entita_insert

                        Dim recodeAllegato = (From rr In GiasContext.G2G_Recode_Allegati Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_Allegati_Documenti_Cod = s.Allegati_Documenti_Cod).FirstOrDefault()
                        Dim allegati_entita = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)

                        allegati_entita.Allegati_Documenti_Cod = recodeAllegato.To_Allegati_Documenti_Cod
                        allegati_entita.Allegati_Documenti_SuperUser = Destinazione_Piva_SuperUser
                        allegati_entita.Piva = piva

                        If allegati_entita.Programmazione_Cod <> 0 Then
                            Dim programmazione_testata = (From rr In GiasContext.G2G_Recode_Programmazione_Testata Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_Piva = s.Piva AndAlso rr.From_Programmazione_Cod = allegati_entita.Programmazione_Cod).FirstOrDefault()
                            If programmazione_testata IsNot Nothing Then
                                allegati_entita.Programmazione_Cod = programmazione_testata.To_Programmazione_Cod
                            Else
                                allegati_entita.Programmazione_Cod = 0
                            End If
                        End If

                        If allegati_entita.Programmazione_Entita_Cod <> 0 Then
                            Dim programmazione_entita = (From rr In GiasContext.G2G_Recode_Programmazione_Entita Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_Piva = s.Piva AndAlso rr.From_Programmazione_Entita_Cod = allegati_entita.Programmazione_Entita_Cod).FirstOrDefault()
                            If programmazione_entita IsNot Nothing Then
                                allegati_entita.Programmazione_Entita_Cod = programmazione_entita.To_Programmazione_Entita_Cod
                            Else
                                allegati_entita.Programmazione_Entita_Cod = 0
                            End If
                        End If

                        GiasContext.Allegati_EntitaxDocumenti.Add(allegati_entita)
                        list_entita_insert.Add(allegati_entita)
                        'GiasContext.SaveChanges() 'Da lasciare perché la chiave è un autoincrementale e non viene assegnato finhcé non viene scritto. Ci serve per il recode.

                        'GiasContext.SaveChanges()

                    Next

                    If list_entita_insert.Count = g2g.allegati_entita_insert.Count Then
                        GiasContext.SaveChanges()

                        For i = 0 To list_entita_insert.Count - 1

                            Dim recode =
                            New G2G_Recode_Allegati_Entita With {
                                .From_PivaSuperUser = Origine_Piva_SuperUser,
                                .To_PivaSuperUser = Destinazione_Piva_SuperUser,
                                .From_ID = g2g.allegati_entita_insert(i).ID,
                                .To_ID = list_entita_insert(i).ID,
                                .Username_Creazione = username,
                                .Username_Modifica = username,
                                .Data_Creazione = Now(),
                                .Data_Modifica = Now(),
                                .Validita_Inizio = AGRODATAINIZIO,
                                .Validita_Fine = AGRODATAFINE,
                                .inviato = "0",
                                .datainvio = Now()
                            }
                            g2g.G2G_Allegati_Entita_Recode_insert.Add(recode)
                            GiasContext.G2G_Recode_Allegati_Entita.Add(recode)

                        Next

                        GiasContext.SaveChanges()

                    End If



                    For Each m As Allegati_Documenti In g2g.allegati_update

                        Dim recode = (From rr In GiasContext.G2G_Recode_Allegati Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_Allegati_Documenti_Cod = m.Allegati_Documenti_Cod).FirstOrDefault()
                        recode.Username_Modifica = username
                        recode.Data_Modifica = data
                        recode.datainvio = data
                        g2g.G2G_Allegati_Recode_update.Add(recode)
                        GiasContext.G2G_Recode_Allegati.Attach(recode)
                        GiasContext.Entry(recode).State = EntityState.Modified

                        Dim allegati = (From mm In GiasContext.Allegati_Documenti Where mm.Allegati_Documenti_Cod = recode.To_Allegati_Documenti_Cod).FirstOrDefault()
                        allegati = Gias_EF_Utility.CopyEntity(GiasContext, m, allegati, username, data)
                        allegati.Allegati_Documenti_SuperUser = Destinazione_Piva_SuperUser
                        allegati.Allegati_Documenti_Piva = piva
                        allegati.Allegati_Documenti_Cod = recode.To_Allegati_Documenti_Cod
                        GiasContext.Allegati_Documenti.Attach(allegati)
                        GiasContext.Entry(allegati).State = EntityState.Modified

                        If gestione_alert_entita Then

                            ' cancello allegati contatto
                            Dim alert_entita = (From e In GiasContext.Alert_Entita Where e.PivaSuperUser = Destinazione_Piva_SuperUser AndAlso e.Piva = allegati.Allegati_Documenti_Piva And e.Allegati_Documenti_Cod = allegati.Allegati_Documenti_Cod And listaAlert_TipoEntita_Cod.Contains(e.TipoEntita_Cod) Select e).ToList()

                            For Each e As Alert_Entita In alert_entita
                                Dim scadenze = (From s In GiasContext.Alert_Elenco Where s.PivaSuperUser = e.PivaSuperUser AndAlso s.ID_Alert_Entita = e.ID_Alert_Entita Select s).ToList()
                                e.Piva = piva
                                For Each scadenza As Alert_Elenco In scadenze
                                    GiasContext.Alert_Elenco.Attach(scadenza)
                                    GiasContext.Alert_Elenco.Remove(scadenza)
                                Next
                                GiasContext.Alert_Entita.Attach(e)
                                GiasContext.Alert_Entita.Remove(e)
                            Next

                            ' inserisco allegati contatto
                            Dim alert_entita_update = (From d In g2g.alert_entita_update Where d.Allegati_Documenti_Cod = m.Allegati_Documenti_Cod).ToList()

                            For Each e As Alert_Entita In alert_entita_update

                                ' nuovo alert entita
                                Dim idAlert = objSequenze.NuovoId_Tabella("Alert_Entita", 0, 2000000000, objParametri, UtilizzaTransazione:=False)
                                'Dim idAlert = G2GUtility.NuovoId_Tabella(GiasContext, "Alert_Entita", 0, 2000000000, objParametri)
                                Dim entita = Gias_EF_Utility.CopyEntity(GiasContext, e, Nothing, username, data)
                                entita.PivaSuperUser = Destinazione_Piva_SuperUser
                                entita.Piva = allegati.Allegati_Documenti_Piva
                                entita.ID_Alert_Entita = idAlert
                                entita.Allegati_Documenti_Cod = allegati.Allegati_Documenti_Cod
                                entita.Piva = piva
                                GiasContext.Alert_Entita.Add(entita)

                                ' nuovo alert elenco
                                Dim scadenze = (From s In g2g.alert_elenco_update Where s.ID_Alert_Entita = e.ID_Alert_Entita).ToList()
                                For Each s As Alert_Elenco In scadenze
                                    'Dim idElenco = G2GUtility.NuovoId_Tabella(GiasContext, "Alert_Elenco", 0, 2000000000, objParametri)
                                    Dim idElenco = objSequenze.NuovoId_Tabella("Alert_Elenco", 0, 2000000000, objParametri, UtilizzaTransazione:=False)
                                    Dim scadenza = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)
                                    scadenza.PivaSuperUser = Destinazione_Piva_SuperUser
                                    scadenza.ID_Alert_Entita = entita.ID_Alert_Entita
                                    scadenza.ID_Elenco = idElenco
                                    GiasContext.Alert_Elenco.Add(scadenza)
                                Next

                            Next

                        End If

                        'GiasContext.SaveChanges()

                    Next

                    GiasContext.SaveChanges()

                    For Each m As Allegati_EntitaxDocumenti In g2g.allegati_entita_update


                        Dim recode = (From rr In GiasContext.G2G_Recode_Allegati_Entita Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_ID = m.ID).FirstOrDefault()
                        recode.Username_Modifica = username
                        recode.Data_Modifica = data
                        recode.datainvio = data
                        g2g.G2G_Allegati_Entita_Recode_update.Add(recode)
                        GiasContext.G2G_Recode_Allegati_Entita.Attach(recode)
                        GiasContext.Entry(recode).State = EntityState.Modified

                        Dim allegati_entita = (From mm In GiasContext.Allegati_EntitaxDocumenti Where mm.ID = recode.To_ID).FirstOrDefault()
                        Dim lID As Integer = allegati_entita.ID
                        Dim lAllegato_Documento_Cod As Integer = allegati_entita.Allegati_Documenti_Cod
                        allegati_entita = Gias_EF_Utility.CopyEntity(GiasContext, m, allegati_entita, username, data)
                        allegati_entita.Allegati_Documenti_SuperUser = Destinazione_Piva_SuperUser
                        allegati_entita.ID = lID
                        allegati_entita.Allegati_Documenti_Cod = lAllegato_Documento_Cod
                        allegati_entita.Piva = piva

                        If m.Programmazione_Cod <> 0 Then
                            Dim programmazione_testata = (From rr In GiasContext.G2G_Recode_Programmazione_Testata Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_Piva = piva AndAlso rr.From_Programmazione_Cod = m.Programmazione_Cod).FirstOrDefault()
                            If programmazione_testata IsNot Nothing Then
                                m.Programmazione_Cod = programmazione_testata.To_Programmazione_Cod
                            Else
                                m.Programmazione_Cod = 0
                            End If
                        End If

                        If m.Programmazione_Entita_Cod <> 0 Then
                            Dim programmazione_entita = (From rr In GiasContext.G2G_Recode_Programmazione_Entita Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_Piva = piva AndAlso rr.From_Programmazione_Entita_Cod = m.Programmazione_Entita_Cod).FirstOrDefault()
                            If programmazione_entita IsNot Nothing Then
                                m.Programmazione_Entita_Cod = programmazione_entita.To_Programmazione_Entita_Cod
                            Else
                                m.Programmazione_Entita_Cod = 0
                            End If
                        End If

                        GiasContext.Allegati_EntitaxDocumenti.Attach(allegati_entita)
                        GiasContext.Entry(allegati_entita).State = EntityState.Modified
                        'GiasContext.SaveChanges()

                    Next

                    GiasContext.SaveChanges()

                    For Each r As G2G_Recode_Allegati_Entita In g2g.G2G_Allegati_Entita_Recode_delete

                        ''cancella entita
                        Dim allegati_entita = (From m In GiasContext.Allegati_EntitaxDocumenti Where m.Allegati_Documenti_SuperUser = Destinazione_Piva_SuperUser AndAlso m.ID = r.To_ID).FirstOrDefault()
                        If allegati_entita IsNot Nothing Then
                            GiasContext.Allegati_EntitaxDocumenti.Attach(allegati_entita)
                            GiasContext.Allegati_EntitaxDocumenti.Remove(allegati_entita)
                        End If

                        '' cancella recode entita
                        Dim recode = (From rr In GiasContext.G2G_Recode_Allegati_Entita Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_ID = r.From_ID).FirstOrDefault()
                        If recode IsNot Nothing Then
                            GiasContext.G2G_Recode_Allegati_Entita.Attach(recode)
                            GiasContext.G2G_Recode_Allegati_Entita.Remove(recode)
                        End If
                        'GiasContext.SaveChanges()

                    Next

                    GiasContext.SaveChanges()

                    For Each r As G2G_Recode_Allegati In g2g.G2G_Allegati_Recode_delete
                        ''cancella entita
                        Dim allegati = (From m In GiasContext.Allegati_Documenti Where m.Allegati_Documenti_SuperUser = Destinazione_Piva_SuperUser AndAlso m.Allegati_Documenti_Cod = r.To_Allegati_Documenti_Cod).FirstOrDefault()

                        If allegati IsNot Nothing Then

                            If gestione_alert_entita Then

                                ' cancello allegati contatto
                                Dim alert_entita = (From e In GiasContext.Alert_Entita Where e.PivaSuperUser = Destinazione_Piva_SuperUser AndAlso e.Piva = allegati.Allegati_Documenti_Piva AndAlso e.Allegati_Documenti_Cod = allegati.Allegati_Documenti_Cod AndAlso listaAlert_TipoEntita_Cod.Contains(e.TipoEntita_Cod) Select e).ToList()

                                For Each e As Alert_Entita In alert_entita
                                    Dim scadenze = (From s In GiasContext.Alert_Elenco Where s.PivaSuperUser = e.PivaSuperUser AndAlso s.ID_Alert_Entita = e.ID_Alert_Entita Select s).ToList()
                                    For Each scadenza As Alert_Elenco In scadenze
                                        GiasContext.Alert_Elenco.Attach(scadenza)
                                        GiasContext.Alert_Elenco.Remove(scadenza)
                                    Next
                                    GiasContext.Alert_Entita.Attach(e)
                                    GiasContext.Alert_Entita.Remove(e)
                                Next

                            End If

                            GiasContext.Allegati_Documenti.Attach(allegati)
                            GiasContext.Allegati_Documenti.Remove(allegati)
                        End If

                        '' cancella recode entita
                        Dim recode = (From rr In GiasContext.G2G_Recode_Allegati Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_Allegati_Documenti_Cod = r.From_Allegati_Documenti_Cod).FirstOrDefault()
                        If recode IsNot Nothing Then
                            GiasContext.G2G_Recode_Allegati.Attach(recode)
                            GiasContext.G2G_Recode_Allegati.Remove(recode)
                        End If
                        'GiasContext.SaveChanges()

                    Next

                    GiasContext.SaveChanges()

                    ' COMIT Effettivo
                    scope.Complete()

                    g2g.Recode.G2GRecodeAllegatiToInsert = g2g.G2G_Allegati_Recode_insert
                    g2g.Recode.G2GRecodeAllegatiToUpdate = g2g.G2G_Allegati_Recode_update
                    g2g.Recode.G2GRecodeAllegatiToDelete = g2g.G2G_Allegati_Recode_delete

                    g2g.Recode.G2GRecodeAllegati_EntitaToInsert = g2g.G2G_Allegati_Entita_Recode_insert
                    g2g.Recode.G2GRecodeAllegati_EntitaToUpdate = g2g.G2G_Allegati_Entita_Recode_update
                    g2g.Recode.G2GRecodeAllegati_EntitaToDelete = g2g.G2G_Allegati_Entita_Recode_delete


                End Using

            End Using

        Catch ex As Exception

            messaggioErrore = ex.Message
            messaggioErrore &= If(IsNothing(ex.InnerException), "", vbCrLf & ex.InnerException.Message)
            g2g.Recode.MessaggioErrore = messaggioErrore
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            'Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore, ex)

        End Try

        Return g2g.Recode

    End Function

    Public Function Scrivi_Allegati_G2GReverse(Origine_Piva_SuperUser As String, Destinazione_Piva_SuperUser As String, ByRef g2g As G2G_Allegati_Reverse, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDal.G2GAllegati_W.Scrivi_Allegati_G2GReverse()"
        Dim messaggioErrore As String = ""
        g2g.Recode = New G2G_Recode

        Dim username = objParametri.UsernameOperazione
        Dim piva = g2g.To_Piva
        Dim data = Date.Now
        Dim listaAlert_TipoEntita_Cod = g2g.alert_tipo_entita_cod
        Dim gestione_alert_entita As Boolean = listaAlert_TipoEntita_Cod.Count > 0

        Dim objSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try

            Dim transactionOptions As New TransactionOptions()
            transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            transactionOptions.Timeout = G2GUtility.GetTransactionTimeout(objParametri)

            Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    For Each m As Allegati_Documenti In g2g.allegati_insert

                        Dim allegati = Gias_EF_Utility.CopyEntity(GiasContext, m, Nothing, username, data)
                        allegati.Allegati_Documenti_Piva = piva
                        'allegati.Allegati_Documenti_Cod = generato automaticamente
                        allegati.Allegati_Documenti_SuperUser = Destinazione_Piva_SuperUser
                        GiasContext.Allegati_Documenti.Add(allegati)
                        GiasContext.SaveChanges() 'Da lasciare perché la chiave è un autoincrementale e non viene assegnato finhcé non viene scritto. Ci serve per il recode.

                        If gestione_alert_entita Then

                            ' inserisco allegati contatto
                            Dim alert_entita_insert = (From d In g2g.alert_entita_insert Where d.Allegati_Documenti_Cod = m.Allegati_Documenti_Cod).ToList()

                            For Each e As Alert_Entita In alert_entita_insert

                                ' nuovo alert entita
                                Dim idAlert = objSequenze.NuovoId_Tabella("Alert_Entita", 0, 2000000000, objParametri, UtilizzaTransazione:=False)
                                Dim entita = Gias_EF_Utility.CopyEntity(GiasContext, e, Nothing, username, data)
                                entita.PivaSuperUser = Destinazione_Piva_SuperUser
                                entita.Piva = allegati.Allegati_Documenti_Piva
                                entita.ID_Alert_Entita = idAlert
                                entita.Allegati_Documenti_Cod = allegati.Allegati_Documenti_Cod

                                If entita.Analisi_Testata_Cod <> 0 Then
                                    Dim entita_old = entita.Analisi_Testata_Cod
                                    Dim objAnalisi = (From g2ga In GiasContext.G2G_Recode_Analisi_Testata
                                                      Where g2ga.From_Analisi_Testata_Cod = entita_old _
                                                                      And g2ga.To_PivaSuperUser = Origine_Piva_SuperUser _
                                                                      And g2ga.From_PivaSuperUser = Destinazione_Piva_SuperUser).FirstOrDefault

                                    If objAnalisi IsNot Nothing Then
                                        entita.Analisi_Testata_Cod = objAnalisi.From_Analisi_Testata_Cod
                                    Else
                                        Throw New Exception("Analisi Testata Cod " & CStr(entita.Analisi_Testata_Cod) & " non trovata in Alert_Entita ")
                                    End If

                                End If


                                GiasContext.Alert_Entita.Add(entita)

                                ' nuovo alert elenco
                                Dim scadenze = (From s In g2g.alert_elenco_insert Where s.ID_Alert_Entita = e.ID_Alert_Entita).ToList()
                                For Each s As Alert_Elenco In scadenze
                                    Dim idElenco = objSequenze.NuovoId_Tabella("Alert_Elenco", 0, 2000000000, objParametri, UtilizzaTransazione:=False)
                                    Dim scadenza = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)
                                    scadenza.PivaSuperUser = Destinazione_Piva_SuperUser
                                    scadenza.ID_Alert_Entita = entita.ID_Alert_Entita
                                    scadenza.ID_Elenco = idElenco
                                    GiasContext.Alert_Elenco.Add(scadenza)
                                Next

                            Next

                        End If

                        Dim recode =
                            New G2G_Recode_Allegati With {
                                .From_PivaSuperUser = Destinazione_Piva_SuperUser,
                                .To_PivaSuperUser = Origine_Piva_SuperUser,
                                .From_Allegati_Documenti_Cod = allegati.Allegati_Documenti_Cod,
                                .To_Allegati_Documenti_Cod = m.Allegati_Documenti_Cod,
                                .Username_Creazione = username,
                                .Username_Modifica = username,
                                .Data_Creazione = Now(),
                                .Data_Modifica = Now(),
                                .Validita_Inizio = AGRODATAINIZIO,
                                .Validita_Fine = AGRODATAFINE,
                                .inviato = "0",
                                .datainvio = Now()
                            }
                        g2g.G2G_Allegati_Recode_insert.Add(recode)
                        GiasContext.G2G_Recode_Allegati.Add(recode)

                        'GiasContext.SaveChanges()

                    Next

                    GiasContext.SaveChanges()

                    Dim list_entita_insert As New List(Of Allegati_EntitaxDocumenti)

                    For Each s As Allegati_EntitaxDocumenti In g2g.allegati_entita_insert

                        Dim recodeAllegato = (From rr In GiasContext.G2G_Recode_Allegati Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.To_Allegati_Documenti_Cod = s.Allegati_Documenti_Cod).FirstOrDefault()
                        Dim allegati_entita = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)

                        allegati_entita.Allegati_Documenti_Cod = recodeAllegato.From_Allegati_Documenti_Cod
                        allegati_entita.Allegati_Documenti_SuperUser = Destinazione_Piva_SuperUser
                        allegati_entita.Piva = piva

                        If allegati_entita.Programmazione_Cod <> 0 Then
                            Dim programmazione_testata = (From rr In GiasContext.G2G_Recode_Programmazione_Testata Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.To_Piva = s.Piva AndAlso rr.To_Programmazione_Cod = allegati_entita.Programmazione_Cod).FirstOrDefault()
                            If programmazione_testata IsNot Nothing Then
                                allegati_entita.Programmazione_Cod = programmazione_testata.From_Programmazione_Cod
                            Else
                                allegati_entita.Programmazione_Cod = 0
                            End If
                        End If

                        If allegati_entita.Programmazione_Entita_Cod <> 0 Then
                            Dim programmazione_entita = (From rr In GiasContext.G2G_Recode_Programmazione_Entita Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.To_Piva = s.Piva AndAlso rr.To_Programmazione_Entita_Cod = allegati_entita.Programmazione_Entita_Cod).FirstOrDefault()
                            If programmazione_entita IsNot Nothing Then
                                allegati_entita.Programmazione_Entita_Cod = programmazione_entita.From_Programmazione_Entita_Cod
                            Else
                                allegati_entita.Programmazione_Entita_Cod = 0
                            End If
                        End If

                        GiasContext.Allegati_EntitaxDocumenti.Add(allegati_entita)
                        list_entita_insert.Add(allegati_entita)
                        'GiasContext.SaveChanges() 'Da lasciare perché la chiave è un autoincrementale e non viene assegnato finhcé non viene scritto. Ci serve per il recode.

                        'GiasContext.SaveChanges()

                    Next

                    If list_entita_insert.Count = g2g.allegati_entita_insert.Count Then
                        GiasContext.SaveChanges()

                        For i = 0 To list_entita_insert.Count - 1

                            Dim recode =
                            New G2G_Recode_Allegati_Entita With {
                                .From_PivaSuperUser = Destinazione_Piva_SuperUser,
                                .To_PivaSuperUser = Origine_Piva_SuperUser,
                                .From_ID = list_entita_insert(i).ID,
                                .To_ID = g2g.allegati_entita_insert(i).ID,
                                .Username_Creazione = username,
                                .Username_Modifica = username,
                                .Data_Creazione = Now(),
                                .Data_Modifica = Now(),
                                .Validita_Inizio = AGRODATAINIZIO,
                                .Validita_Fine = AGRODATAFINE,
                                .inviato = "0",
                                .datainvio = Now()
                            }
                            g2g.G2G_Allegati_Entita_Recode_insert.Add(recode)
                            GiasContext.G2G_Recode_Allegati_Entita.Add(recode)

                        Next

                        GiasContext.SaveChanges()

                    End If



                    For Each m As Allegati_Documenti In g2g.allegati_update

                        Dim recode = (From rr In GiasContext.G2G_Recode_Allegati Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.To_Allegati_Documenti_Cod = m.Allegati_Documenti_Cod).FirstOrDefault()
                        recode.Username_Modifica = username
                        recode.Data_Modifica = data
                        recode.datainvio = data
                        g2g.G2G_Allegati_Recode_update.Add(recode)
                        'GiasContext.G2G_Recode_Allegati.Attach(recode)
                        GiasContext.Entry(recode).State = EntityState.Modified

                        Dim allegati = (From mm In GiasContext.Allegati_Documenti Where mm.Allegati_Documenti_Cod = recode.From_Allegati_Documenti_Cod).FirstOrDefault()
                        allegati = Gias_EF_Utility.CopyEntity(GiasContext, m, allegati, username, data)
                        allegati.Allegati_Documenti_SuperUser = Destinazione_Piva_SuperUser
                        allegati.Allegati_Documenti_Piva = piva
                        allegati.Allegati_Documenti_Cod = recode.From_Allegati_Documenti_Cod
                        'GiasContext.Allegati_Documenti.Attach(allegati)
                        GiasContext.Entry(allegati).State = EntityState.Modified

                        If gestione_alert_entita Then

                            ' cancello allegati contatto
                            Dim alert_entita = (From e In GiasContext.Alert_Entita Where e.PivaSuperUser = Destinazione_Piva_SuperUser AndAlso e.Piva = allegati.Allegati_Documenti_Piva And e.Allegati_Documenti_Cod = allegati.Allegati_Documenti_Cod And listaAlert_TipoEntita_Cod.Contains(e.TipoEntita_Cod) Select e).ToList()

                            For Each e As Alert_Entita In alert_entita
                                Dim scadenze = (From s In GiasContext.Alert_Elenco Where s.PivaSuperUser = e.PivaSuperUser AndAlso s.ID_Alert_Entita = e.ID_Alert_Entita Select s).ToList()
                                e.Piva = piva
                                For Each scadenza As Alert_Elenco In scadenze
                                    'GiasContext.Alert_Elenco.Attach(scadenza)
                                    GiasContext.Alert_Elenco.Remove(scadenza)
                                Next
                                'GiasContext.Alert_Entita.Attach(e)
                                GiasContext.Alert_Entita.Remove(e)
                            Next

                            ' inserisco allegati contatto
                            Dim alert_entita_update = (From d In g2g.alert_entita_update Where d.Allegati_Documenti_Cod = m.Allegati_Documenti_Cod).ToList()

                            For Each e As Alert_Entita In alert_entita_update

                                ' nuovo alert entita
                                Dim idAlert = objSequenze.NuovoId_Tabella("Alert_Entita", 0, 2000000000, objParametri, UtilizzaTransazione:=False)
                                'Dim idAlert = G2GUtility.NuovoId_Tabella(GiasContext, "Alert_Entita", 0, 2000000000, objParametri)
                                Dim entita = Gias_EF_Utility.CopyEntity(GiasContext, e, Nothing, username, data)
                                entita.PivaSuperUser = Destinazione_Piva_SuperUser
                                entita.Piva = allegati.Allegati_Documenti_Piva
                                entita.ID_Alert_Entita = idAlert
                                entita.Allegati_Documenti_Cod = allegati.Allegati_Documenti_Cod
                                entita.Piva = piva
                                GiasContext.Alert_Entita.Add(entita)

                                ' nuovo alert elenco
                                Dim scadenze = (From s In g2g.alert_elenco_update Where s.ID_Alert_Entita = e.ID_Alert_Entita).ToList()
                                For Each s As Alert_Elenco In scadenze
                                    'Dim idElenco = G2GUtility.NuovoId_Tabella(GiasContext, "Alert_Elenco", 0, 2000000000, objParametri)
                                    Dim idElenco = objSequenze.NuovoId_Tabella("Alert_Elenco", 0, 2000000000, objParametri, UtilizzaTransazione:=False)
                                    Dim scadenza = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)
                                    scadenza.PivaSuperUser = Destinazione_Piva_SuperUser
                                    scadenza.ID_Alert_Entita = entita.ID_Alert_Entita
                                    scadenza.ID_Elenco = idElenco
                                    GiasContext.Alert_Elenco.Add(scadenza)
                                Next

                            Next

                        End If

                        'GiasContext.SaveChanges()

                    Next

                    GiasContext.SaveChanges()

                    For Each m As Allegati_EntitaxDocumenti In g2g.allegati_entita_update


                        Dim recode = (From rr In GiasContext.G2G_Recode_Allegati_Entita Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.To_ID = m.ID).FirstOrDefault()
                        recode.Username_Modifica = username
                        recode.Data_Modifica = data
                        recode.datainvio = data
                        g2g.G2G_Allegati_Entita_Recode_update.Add(recode)
                        'GiasContext.G2G_Recode_Allegati_Entita.Attach(recode)
                        GiasContext.Entry(recode).State = EntityState.Modified

                        Dim allegati_entita = (From mm In GiasContext.Allegati_EntitaxDocumenti Where mm.ID = recode.From_ID).FirstOrDefault()
                        Dim lID As Integer = allegati_entita.ID
                        Dim lAllegato_Documento_Cod As Integer = allegati_entita.Allegati_Documenti_Cod
                        allegati_entita = Gias_EF_Utility.CopyEntity(GiasContext, m, allegati_entita, username, data)
                        allegati_entita.Allegati_Documenti_SuperUser = Destinazione_Piva_SuperUser
                        allegati_entita.ID = lID
                        allegati_entita.Allegati_Documenti_Cod = lAllegato_Documento_Cod
                        allegati_entita.Piva = piva

                        If m.Programmazione_Cod <> 0 Then
                            Dim programmazione_testata = (From rr In GiasContext.G2G_Recode_Programmazione_Testata Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.To_Piva = piva AndAlso rr.To_Programmazione_Cod = m.Programmazione_Cod).FirstOrDefault()
                            If programmazione_testata IsNot Nothing Then
                                m.Programmazione_Cod = programmazione_testata.From_Programmazione_Cod
                            Else
                                m.Programmazione_Cod = 0
                            End If
                        End If

                        If m.Programmazione_Entita_Cod <> 0 Then
                            Dim programmazione_entita = (From rr In GiasContext.G2G_Recode_Programmazione_Entita Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.To_Piva = piva AndAlso rr.To_Programmazione_Entita_Cod = m.Programmazione_Entita_Cod).FirstOrDefault()
                            If programmazione_entita IsNot Nothing Then
                                m.Programmazione_Entita_Cod = programmazione_entita.From_Programmazione_Entita_Cod
                            Else
                                m.Programmazione_Entita_Cod = 0
                            End If
                        End If

                        'GiasContext.Allegati_EntitaxDocumenti.Attach(allegati_entita)
                        GiasContext.Entry(allegati_entita).State = EntityState.Modified
                        'GiasContext.SaveChanges()

                    Next

                    GiasContext.SaveChanges()

                    For Each r As G2G_Recode_Allegati_Entita In g2g.G2G_Allegati_Entita_Recode_delete

                        ''cancella entita
                        Dim allegati_entita = (From m In GiasContext.Allegati_EntitaxDocumenti Where m.Allegati_Documenti_SuperUser = Destinazione_Piva_SuperUser AndAlso m.ID = r.From_ID).FirstOrDefault()
                        'GiasContext.Allegati_EntitaxDocumenti.Attach(allegati_entita)
                        GiasContext.Allegati_EntitaxDocumenti.Remove(allegati_entita)

                        '' cancella recode entita
                        Dim recode = (From rr In GiasContext.G2G_Recode_Allegati_Entita Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.To_ID = r.To_ID).FirstOrDefault()
                        'GiasContext.G2G_Recode_Allegati_Entita.Attach(recode)
                        GiasContext.G2G_Recode_Allegati_Entita.Remove(recode)
                        'GiasContext.SaveChanges()

                    Next

                    GiasContext.SaveChanges()

                    For Each r As G2G_Recode_Allegati In g2g.G2G_Allegati_Recode_delete
                        ''cancella entita
                        Dim allegati = (From m In GiasContext.Allegati_Documenti Where m.Allegati_Documenti_SuperUser = Destinazione_Piva_SuperUser AndAlso m.Allegati_Documenti_Cod = r.From_Allegati_Documenti_Cod).FirstOrDefault()

                        If allegati IsNot Nothing Then

                            If gestione_alert_entita Then

                                ' cancello allegati contatto
                                Dim alert_entita = (From e In GiasContext.Alert_Entita Where e.PivaSuperUser = Destinazione_Piva_SuperUser AndAlso e.Piva = allegati.Allegati_Documenti_Piva And e.Allegati_Documenti_Cod = allegati.Allegati_Documenti_Cod And listaAlert_TipoEntita_Cod.Contains(e.TipoEntita_Cod) Select e).ToList()

                                For Each e As Alert_Entita In alert_entita
                                    Dim scadenze = (From s In GiasContext.Alert_Elenco Where s.PivaSuperUser = e.PivaSuperUser AndAlso s.ID_Alert_Entita = e.ID_Alert_Entita Select s).ToList()
                                    For Each scadenza As Alert_Elenco In scadenze
                                        'GiasContext.Alert_Elenco.Attach(scadenza)
                                        GiasContext.Alert_Elenco.Remove(scadenza)
                                    Next
                                    'GiasContext.Alert_Entita.Attach(e)
                                    GiasContext.Alert_Entita.Remove(e)
                                Next

                            End If

                            'GiasContext.Allegati_Documenti.Attach(allegati)
                            GiasContext.Allegati_Documenti.Remove(allegati)
                        End If

                        '' cancella recode entita
                        Dim recode = (From rr In GiasContext.G2G_Recode_Allegati Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.To_Allegati_Documenti_Cod = r.To_Allegati_Documenti_Cod).FirstOrDefault()
                        If recode IsNot Nothing Then
                            'GiasContext.G2G_Recode_Allegati.Attach(recode)
                            GiasContext.G2G_Recode_Allegati.Remove(recode)
                        End If
                        'GiasContext.SaveChanges()

                    Next

                    GiasContext.SaveChanges()

                    ' COMIT Effettivo
                    scope.Complete()

                    g2g.Recode.G2GRecodeAllegatiToInsert = g2g.G2G_Allegati_Recode_insert
                    g2g.Recode.G2GRecodeAllegatiToUpdate = g2g.G2G_Allegati_Recode_update
                    g2g.Recode.G2GRecodeAllegatiToDelete = g2g.G2G_Allegati_Recode_delete

                    g2g.Recode.G2GRecodeAllegati_EntitaToInsert = g2g.G2G_Allegati_Entita_Recode_insert
                    g2g.Recode.G2GRecodeAllegati_EntitaToUpdate = g2g.G2G_Allegati_Entita_Recode_update
                    g2g.Recode.G2GRecodeAllegati_EntitaToDelete = g2g.G2G_Allegati_Entita_Recode_delete


                End Using

            End Using

        Catch ex As Exception

            messaggioErrore = ex.Message
            g2g.Recode.MessaggioErrore = messaggioErrore & If(IsNothing(ex.InnerException), "", vbCrLf & ex.InnerException.Message)
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            'Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore, ex)

        End Try

        Return g2g.Recode

    End Function

End Class
