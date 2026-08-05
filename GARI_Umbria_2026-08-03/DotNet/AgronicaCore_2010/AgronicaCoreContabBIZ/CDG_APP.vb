Imports System.Text
Imports AgronicaCoreContabDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class CDG_APP
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function LeggiCentroAziendale(ByRef Piva As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer
        Dim objCentro As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
        Dim dt = objCentro.Leggi(Piva, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri)
        Return dt.Rows(0).Item("sa_cod")
    End Function

    Public Function LeggiPivaOreIndirette(ByVal Piva As String, ByRef dtConfig As DataTable) As String
        For Each config In dtConfig.Rows
            If Piva = config.Item("Piva_From") Then
                Return config.Item("Piva_To")
            End If
        Next
        Return ""
    End Function

    Public Sub LeggiImputazioneOreIndirette(ByRef Piva As String, ByRef Sa_Cod As Integer, ByRef Id_Attivita As Integer, ByRef Id_Imputazione As Integer, ByRef dtConfig As DataTable)
        For Each config In dtConfig.Rows
            If Piva = config.Item("Piva_From") Then
                If Sa_Cod = 0 OrElse Sa_Cod = config.Item("Sa_Cod_From") OrElse config.Item("Sa_Cod_From") = 0 Then
                    Piva = config.Item("Piva_To")
                    Sa_Cod = config.Item("Sa_Cod_To")
                    If config.Item("Id_Attivita") <> 0 Then
                        Id_Attivita = config.Item("Id_Attivita")
                    End If
                    Id_Imputazione = config.Item("Id_Imputazione")
                    Exit For
                End If
            End If
        Next
    End Sub

    Public Function LeggiConfigRibaltamentoOre(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim dtConfigSiti As DataTable = objConfigSiti.Leggi(0, "RibaltamentoOreIndirette", "", "", objParametri)

        Dim dtConfig As New DataTable
        dtConfig.Columns.Add(New DataColumn("Piva_From", GetType(String)))
        dtConfig.Columns.Add(New DataColumn("Sa_Cod_From", GetType(Integer)))
        dtConfig.Columns.Add(New DataColumn("Piva_To", GetType(String)))
        dtConfig.Columns.Add(New DataColumn("Sa_Cod_To", GetType(Integer)))
        dtConfig.Columns.Add(New DataColumn("Id_Imputazione", GetType(Integer)))
        dtConfig.Columns.Add(New DataColumn("Id_Attivita", GetType(Integer)))

        If Not IsNothing(dtConfigSiti) AndAlso dtConfigSiti.Rows.Count > 0 Then
            Dim configurazione = dtConfigSiti.Rows(0)("valore").ToString
            Dim config = configurazione.Split("|")
            If config.Count Mod 3 = 0 Then
                Dim objCentro As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
                Dim numRows = config.Count \ 3
                Dim tokens As String()
                For i As Integer = 1 To numRows

                    Dim index As Integer = (i - 1) * 3
                    tokens = config(0 + index).Split("_")
                    Dim Piva_From = tokens(0)
                    Dim Sa_Cod_From = If(tokens.Count > 1, tokens(1), 0)

                    tokens = config(1 + index).Split("_")
                    Dim Piva_To = tokens(0)
                    Dim Sa_Cod_To = If(tokens.Count > 1, tokens(1), 0)

                    tokens = config(2 + index).Split("_")
                    Dim Id_Imputazione = tokens(0)
                    Dim Id_Attivita = If(tokens.Count > 1, tokens(1), 0)

                    Dim drConfig As DataRow = dtConfig.NewRow
                    drConfig.Item("Piva_From") = Piva_From
                    drConfig.Item("Sa_Cod_From") = Sa_Cod_From
                    drConfig.Item("Piva_To") = Piva_To
                    drConfig.Item("Sa_Cod_To") = Sa_Cod_To
                    drConfig.Item("Id_Imputazione") = Id_Imputazione
                    drConfig.Item("Id_Attivita") = Id_Attivita
                    dtConfig.Rows.Add(drConfig)

                Next
            End If
        End If

        Return dtConfig
    End Function

    Public Function LeggiRiferimentiCDGInterventiAPP(ByVal Id_Ricetta As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "LeggiRiferimentiCDGInterventiAPP()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            '---------------------------------------------
            Stb.Append("SELECT a.id as id_cdg_riferimento, c.id as id_cdg_movimento ")
            'Stb.Append("SELECT a.id as id_cdg, a.Id_Cdg_Generale_Rif, b.Ricetta_Cod, b.Ricetta_Operazione_Cod, b.Ricetta_Operazione_Cod_RIF, c.id, c.piva, c.Sa_Cod, c.Appezza, c.Id_Reg ")
            Stb.Append("FROM APP_Riferimenti_Interventi_Cdg a ")
            Stb.Append("INNER JOIN APP_Ricette_Operazioni b On a.Ricetta_Cod = b.Ricetta_Cod And a.Ricetta_Operazione_Cod= b.Ricetta_Operazione_Cod And substring(a.id,0,charindex('|',a.id)) = substring(b.id,0,charindex('|',b.id)) ")
            Stb.Append("INNER JOIN APP_CDG_Movimenti c On a.Id_Cdg_Generale_Rif = c.Id_Cdg_Generale And substring(a.id,0,charindex('|',a.id)) = substring(c.id,0,charindex('|',c.id)) ")
            Stb.Append("WHERE b.id='" & Agro_SQL_SaveText(Id_Ricetta) & "' ")
            Stb.Append("ORDER BY c.id")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return dt

    End Function

    Public Function InserisciMovimentiCDG(ByVal Id As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "InserisciMovimentiCDG()"
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            Stb.Append("INSERT INTO APP_CDG_Movimenti (id,Piva_Superuser,Piva,Id_CDG_Movimenti,Id_Cdg_Generale,Data_Ora_Inizio,Data_Ora_Fine,Qta,Mac_Cod,Cod_RisUm,NrBadge,Id_Attivita,Id_Imputazione,inviato,datainvio,Data_Creazione,Data_Modifica,Username_Creazione,Username_Modifica,Validita_Inizio,Validita_Fine,Sa_Cod,Appezza,Id_Reg,Progetto_Cod) ")
            Stb.Append("SELECT c.id+'|'+cast(b.ricetta_destinazione_cod as varchar) as id,c.Piva_Superuser,c.Piva,c.Id_CDG_Movimenti,c.Id_Cdg_Generale,c.Data_Ora_Inizio,c.Data_Ora_Fine,c.Qta,c.Mac_Cod,c.Cod_RisUm,c.NrBadge,c.Id_Attivita,c.Id_Imputazione,c.inviato,c.datainvio,c.Data_Creazione,c.Data_Modifica,c.Username_Creazione,c.Username_Modifica,c.Validita_Inizio,c.Validita_Fine,c.Sa_Cod,b.Appezza,b.Id_Reg,c.Progetto_Cod ")
            Stb.Append("FROM APP_Riferimenti_Interventi_Cdg a INNER JOIN ( ")
            Stb.Append("  SELECT a.id, c.ricetta_destinazione_cod, c.piva, c.Sa_Cod, c.Appezza, c.Id_Reg ")
            Stb.Append("  FROM APP_Riferimenti_Interventi_Cdg a ")
            Stb.Append("  INNER JOIN APP_Ricette_Operazioni b On a.Ricetta_Cod = b.Ricetta_Cod And a.Ricetta_Operazione_Cod= b.Ricetta_Operazione_Cod And substring(a.id,0,charindex('|',a.id)) = substring(b.id,0,charindex('|',b.id)) ")
            Stb.Append("  INNER JOIN APP_Ricette_Destinazioni c On b.Ricetta_Cod = c.Ricetta_Cod And b.Ricetta_Operazione_Cod= c.Ricetta_Operazione_Cod And substring(b.id,0,charindex('|',b.id)) = substring(c.id,0,charindex('|',c.id)) ")
            Stb.Append("  WHERE tipo_destinazione=0 And b.id='" & Agro_SQL_SaveText(Id) & "') b on a.id=b.id ")
            Stb.Append("INNER JOIN APP_CDG_Movimenti c On a.Id_Cdg_Generale_Rif = c.Id_Cdg_Generale And substring(a.id,0,charindex('|',a.id)) = substring(c.id,0,charindex('|',c.id))")

            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function CancellaMovimentoCDG(ByVal Id As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "CancellaMovimentoCDG()"
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            Stb.Append("DELETE FROM APP_CDG_Movimenti ")
            Stb.Append("WHERE id='" & Agro_SQL_SaveText(Id) & "'")
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function CancellaRiferimentoCDG(ByVal Id As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "CancellaRiferimentoCDG()"
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            Stb.Append("DELETE FROM APP_Riferimenti_Interventi_Cdg ")
            Stb.Append("WHERE id='" & Agro_SQL_SaveText(Id) & "'")
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    ' Converte movimenti e rimuove riferimento all'intervento cancellato
    Public Function ConvertiMovimentiCDGInterventiAPP(ByVal Id_Ricetta As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim msgErr As New StringBuilder
        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False
        Dim esitoFinale As Boolean = False

        If Not String.IsNullOrEmpty(Id_Ricetta) Then

            Dim dtMovimentiCDG = LeggiRiferimentiCDGInterventiAPP(Id_Ricetta, objParametri)

            If dtMovimentiCDG.Rows.Count > 0 Then

                Try

                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    'Apro la connessione al DB
                    AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, FlagTransazioneLocale, objParametri)
                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

                    Dim id_cdg_riferimento As String = dtMovimentiCDG.Rows(0).Item("id_cdg_riferimento")
                    InserisciMovimentiCDG(Id_Ricetta, objParametri)
                    For Each dr As DataRow In dtMovimentiCDG.Rows
                        Dim id_cdg_movimento As String = dr.Item("id_cdg_movimento")
                        CancellaMovimentoCDG(id_cdg_movimento, objParametri)
                    Next
                    CancellaRiferimentoCDG(id_cdg_riferimento, objParametri)

                    esitoFinale = True

                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    'Chiudo la connessione al DB
                    AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)
                    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

                Catch ex As Exception

                    'Faccio il rollback della transazione
                    If Not objParametri.objTransazione Is Nothing Then
                        AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
                    End If

                Finally

                    AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)

                End Try

            End If

        End If

        Return esitoFinale

    End Function

    Public Function LeggiCDGDaTabelleAPP(
        ByVal Piva As String,
        ByVal Id_Agenda As Integer,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
        Optional ByVal guid As String = ""
    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "LeggiCDGDaTabelleAPP()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            '---------------------------------------------
            Stb.Length = 0

            Stb.Append("SELECT a.Id, a.Piva, a.Data_Inserimento, a.Importato_Data, a.Importato_Errore, a.Note, ")
            Stb.Append("b.cod_risum, b.mac_cod, b.id_attivita, b.Data_Ora_Inizio, b.Data_Ora_Fine, ")
            Stb.Append("b.id_imputazione, b.sa_cod, b.appezza, b.id_reg, b.progetto_cod, ")
            Stb.Append("d.[Desc] as attivita, d.Attivita_Extra_Campagna, e.sup_imp, f.Cul_Des, g.Veg_Des, ")
            Stb.Append("h.cod_rapporto, h.qualifica_cod, i.nome, i.cognome, j.Qualifica_Des, ")
            Stb.Append("k.Mac_Des, k.sa_cod as mac_sa_cod, k.tipo as mac_tipo, rr.Ricetta_Numero, ")
            Stb.Append("r.id as Id_Ricetta, r.Ricetta_Operazione_Cod, r.Ricetta_Operazione_Cod_RIF, ")
            Stb.Append("ra.Id_Agenda, ag.des_lib, ag.Lav_Cod, ag.validita_inizio as data_movimento ")
            Stb.Append("FROM APP_CDG_Generale a ")
            Stb.Append("inner join APP_CDG_Movimenti b on a.Id_Cdg_Generale = b.Id_Cdg_Generale and substring(a.Id,0,charindex('|',a.id)) = substring(b.Id, 0, charindex('|',b.id)) ")
            Stb.Append("left join APP_Riferimenti_Interventi_Cdg c On a.Id_Cdg_Generale = c.Id_Cdg_Generale_Rif And substring(a.Id,0,charindex('|',a.id)) = substring(c.Id,0,charindex('|',c.id)) ")
            Stb.Append("Left Join APP_Ricette_Operazioni r On c.Ricetta_Cod = r.Ricetta_Cod And c.Ricetta_Operazione_Cod= r.Ricetta_Operazione_Cod And substring(a.Id, 0, charindex('|',a.id)) = substring(r.Id,0,charindex('|',r.id)) ")
            Stb.Append("left join Ricette_Operazioni ro On r.ID = ro.APP_Ricetta_Operazione_ID ")
            Stb.Append("left join Ricette rr On ro.Ricetta_Cod = rr.Ricetta_Cod ")
            Stb.Append("left join ricettexagenda ra On ro.Ricetta_Cod = ra.Ricetta_Cod And ro.Ricetta_Operazione_Cod = ra.Ricetta_Operazione_Cod ")
            Stb.Append("left join agenda ag On ra.Id_Agenda = ag.Id_Agenda and a.Piva = ag.Piva ")
            Stb.Append("left Join attivita d on b.id_attivita = d.ID_Attivita ")
            Stb.Append("left Join reg_impianti e on b.piva = e.piva And b.sa_cod = e.sa_cod And b.appezza = e.appezza And b.id_reg = e.ID_REG ")
            Stb.Append("left Join cultivar f on e.cul_cod = f.cul_cod left join specievegetali g on f.veg_cod = g.veg_cod ")
            Stb.Append("left join risorse_umane h on b.Cod_RisUm = h.Cod_RisUm left join contatti i on h.Cod_Contatto = i.cod_contatto ")
            Stb.Append("left join qualifiche j on h.Qualifica_Cod = j.Qualifica_Cod left join parco_macchine k on b.mac_cod = k.mac_cod ")
            Stb.Append("WHERE a.Importato_Data is null and (c.id is null or (r.Importato_Data is not null and ro.Ricetta_Operazione_Cod is not null)) ") ' ra.Id_Agenda is not null

            ' filtro su piva azienda
            If Not String.IsNullOrEmpty(Piva) Then
                Stb.Append(" AND a.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            ' filtro su id app
            If Not String.IsNullOrEmpty(guid) Then
                Stb.Append(" AND a.ID LIKE '%" & Agro_SQL_SaveText(guid) & "%' ")
            End If

            ' filtro per attivita collegate a interventi
            If Id_Agenda <> 0 Then
                Stb.Append("and ra.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")
            End If

            Stb.Append("order by a.Id, a.Data_Inserimento, b.Cod_RisUm, b.Mac_Cod, b.Data_Ora_Inizio, b.Data_Ora_Fine")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return dt

    End Function

    Public Function ImportaCDG(ByVal id As String, ByVal Piva As String, ByVal Id_Agenda As Integer, ByVal Id_Agenda_CDG As Integer, ByVal Lav_Cod As Integer, ByVal Descrizione As String, ByVal Data_Inserimento As Date, ByVal Sup_Tot As Decimal,
                               ByRef dtManodopera As DataTable, ByRef dtMacchine As DataTable, ByRef dtProgetti As DataTable, ByRef dtImpianti As DataTable, ByVal impianti_CDG As String, ByVal dati_CDG As String,
                               ByRef msgErr As StringBuilder, ByVal objParametri_Server As AgronicaCoreParametri, Optional ByRef riferimento As String = "") As Boolean
        Try

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            Dim CDG_BIZ_W As New CDG_BIZ_W

            Dim residuoSpalmabile As Decimal = 100
            Dim valoreMaggiore As Decimal = 0
            If Sup_Tot > 0 Then
                For Each dr As DataRow In dtImpianti.Rows
                    dr.Item("Valore") = Math.Round(dr.Item("Valore") / Sup_Tot * 100, 2)
                    If valoreMaggiore < dr.Item("Valore") Then
                        valoreMaggiore = dr.Item("Valore")
                    End If
                    residuoSpalmabile -= dr.Item("Valore")
                Next
            End If
            If residuoSpalmabile > 0 Then
                For Each dr As DataRow In dtImpianti.Rows
                    If dr.Item("Valore") = valoreMaggiore Then
                        dr.Item("Valore") += residuoSpalmabile
                        Exit For
                    End If
                Next
            End If

            Dim manodopera = JsonConvert.SerializeObject(dtManodopera, Newtonsoft.Json.Formatting.None, serializerSettings)
            Dim macchine = JsonConvert.SerializeObject(dtMacchine, Newtonsoft.Json.Formatting.None, serializerSettings)
            Dim progetti = JsonConvert.SerializeObject(dtProgetti, Newtonsoft.Json.Formatting.None, serializerSettings)
            Dim impianti = ""
            Dim eredita = ""

            If Not String.IsNullOrEmpty(dati_CDG) Then
                Dim dati = JObject.Parse(dati_CDG)

                If Not IsNothing(dati("kendo_Eredita")) Then
                    eredita = dati("kendo_Eredita").ToString
                End If

                impianti = dati("kendo_Impianti_Dettagli").ToString

            ElseIf String.IsNullOrEmpty(impianti_CDG) Then
                impianti = JsonConvert.SerializeObject(dtImpianti, Newtonsoft.Json.Formatting.None, serializerSettings)
            Else
                impianti = impianti_CDG
            End If

            Dim Id_Agenda_CDG_Return = CDG_BIZ_W.AggiornaCDG(Piva, Id_Agenda, Id_Agenda_CDG, Lav_Cod,
                    Descrizione, Data_Inserimento, IIf(Id_Agenda = 0, 1, 0), 0, 0, 0, 0, 0, 1, True, 0,
                    manodopera, "", "", "",
                    macchine, "", "", "", "", "",
                    eredita, "",
                    impianti, "",
                    progetti, "", "", "", "", "", "", "",
                    objParametri_Server)

            If Id_Agenda_CDG_Return = 0 Then
                MarcaCDGImportataAPP(id, "Problema sul salvataggio dati", objParametri_Server)
            Else
                MarcaCDGImportataAPP(id, "", objParametri_Server)
                riferimento = CStr(Id_Agenda_CDG_Return)
            End If

        Catch ex As Exception

            'Scrivo che non sono state importate
            MarcaCDGImportataAPP(id, ex.Message, objParametri_Server)
            msgErr.Append(ex.Message & "<br>")
            Return False

        End Try

        Return True

    End Function

    Private Sub InitDataTable(ByRef dtManodopera As DataTable, ByRef dtMacchine As DataTable, ByRef dtImpianti As DataTable, ByRef dtProgetti As DataTable)

        ' Manodopera
        dtManodopera.Columns.Add(New DataColumn("Id_CDG", GetType(Integer)))
        dtManodopera.Columns.Add(New DataColumn("Cod_Rapporto", GetType(Integer)))
        dtManodopera.Columns.Add(New DataColumn("Cod_Risum", GetType(Integer)))
        dtManodopera.Columns.Add(New DataColumn("Prodotto_Des", GetType(String)))
        dtManodopera.Columns.Add(New DataColumn("Qualifica_Cod", GetType(Integer)))
        dtManodopera.Columns.Add(New DataColumn("ID_Attivita", GetType(Integer)))
        dtManodopera.Columns.Add(New DataColumn("Ora_Inizio", GetType(Date)))
        dtManodopera.Columns.Add(New DataColumn("Ora_Fine", GetType(Date)))
        'dtManodopera.Columns.Add(New DataColumn("Ora", GetType(Date)))
        dtManodopera.Columns.Add(New DataColumn("Qta", GetType(Decimal)))
        dtManodopera.Columns.Add(New DataColumn("Tariffa_Cod", GetType(Integer)))
        dtManodopera.Columns.Add(New DataColumn("Prezzo_Unitario", GetType(Decimal)))
        dtManodopera.Columns.Add(New DataColumn("Prezzo_Totale", GetType(Decimal)))
        dtManodopera.Columns.Add(New DataColumn("Udm_Cod", GetType(Integer)))
        dtManodopera.Columns.Add(New DataColumn("OrigineApp", GetType(Integer)))
        dtManodopera.Columns.Add(New DataColumn("APP_CDG_Generale_ID", GetType(String)))

        ' Macchine
        dtMacchine.Columns.Add(New DataColumn("Id_CDG", GetType(Integer)))
        dtMacchine.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
        dtMacchine.Columns.Add(New DataColumn("Tipo", GetType(Integer)))
        dtMacchine.Columns.Add(New DataColumn("Mac_Cod", GetType(Integer)))
        dtMacchine.Columns.Add(New DataColumn("Prodotto_Des", GetType(String)))
        dtMacchine.Columns.Add(New DataColumn("ID_Attivita", GetType(Integer)))
        dtMacchine.Columns.Add(New DataColumn("Ora_Inizio", GetType(Date)))
        dtMacchine.Columns.Add(New DataColumn("Ora_Fine", GetType(Date)))
        'dtMacchine.Columns.Add(New DataColumn("Ora", GetType(Date)))
        dtMacchine.Columns.Add(New DataColumn("Qta", GetType(Decimal)))
        dtMacchine.Columns.Add(New DataColumn("Udm_Cod", GetType(Integer)))
        dtMacchine.Columns.Add(New DataColumn("OrigineApp", GetType(Integer)))
        dtMacchine.Columns.Add(New DataColumn("APP_CDG_Generale_ID", GetType(String)))

        ' Impianti
        dtImpianti.Columns.Add(New DataColumn("Piva", GetType(String)))
        dtImpianti.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
        dtImpianti.Columns.Add(New DataColumn("Appezza", GetType(Integer)))
        dtImpianti.Columns.Add(New DataColumn("Id_Destinazione", GetType(Integer)))
        dtImpianti.Columns.Add(New DataColumn("Progetto_Cod", GetType(Integer)))
        dtImpianti.Columns.Add(New DataColumn("Valore", GetType(Decimal)))

        ' Progetti
        dtProgetti.Columns.Add(New DataColumn("Imputazione_Cod", GetType(Integer)))
        dtProgetti.Columns.Add(New DataColumn("Valore", GetType(Decimal)))

    End Sub

    Private Sub SetOraInizioFine(ByRef dr As DataRow, ByRef data_inizio As DateTime, ByRef data_fine As DateTime)
        dr.Item("Ora_Inizio") = Format(data_inizio.ToUniversalTime, "dd/MM/yyyy HH:mm")
        If data_fine = AGRODATAFINE Then
            dr.Item("Ora_Fine") = Format(New DateTime(data_inizio.Year, data_inizio.Month, data_inizio.Day, 23, 59, 0, 0).ToUniversalTime, "dd/MM/yyyy HH:mm")
        Else
            dr.Item("Ora_Fine") = Format(data_fine.ToUniversalTime, "dd/MM/yyyy HH:mm")
        End If
        dr.Item("Qta") = DateDiff(DateInterval.Minute, dr.Item("Ora_Inizio"), dr.Item("Ora_Fine")) / 60
        ' dr.Item("Ora") = 0
    End Sub

    Private Sub AddRigaManodopera(ByRef dr As DataRow, ByRef dtManodopera As DataTable, ByRef dtTariffe As DataTable)
        Dim drManodopera As DataRow = dtManodopera.NewRow
        drManodopera.Item("Id_CDG") = 0
        drManodopera.Item("OrigineApp") = 1
        drManodopera.Item("APP_CDG_Generale_ID") = dr.Item("id")
        drManodopera.Item("Cod_Rapporto") = dr.Item("Cod_Rapporto")
        drManodopera.Item("Cod_RisUm") = dr.Item("Cod_RisUm")
        drManodopera.Item("Prodotto_Des") = dr.Item("Cognome") & " " & dr.Item("Nome")
        drManodopera.Item("Qualifica_Cod") = dr.Item("Qualifica_Cod")
        drManodopera.Item("ID_Attivita") = dr.Item("Id_Attivita")
        SetOraInizioFine(drManodopera, dr.Item("Data_Ora_Inizio"), dr.Item("Data_Ora_Fine"))
        drManodopera.Item("Udm_Cod") = 141
        drManodopera.Item("Tariffa_Cod") = 0
        drManodopera.Item("Prezzo_Unitario") = 0
        drManodopera.Item("Prezzo_Totale") = 0
        ' prendo la prima tariffa relativa alla qualifica
        For Each dr2 As DataRow In dtTariffe.Rows
            If dr2.Item("Qualifica_Cod") = dr.Item("Qualifica_Cod") Then
                drManodopera.Item("Tariffa_Cod") = dr2.Item("Tariffa_Cod")
                drManodopera.Item("Prezzo_Unitario") = dr2.Item("Valore")
                drManodopera.Item("Prezzo_Totale") = drManodopera.Item("Qta") * dr2.Item("Valore")
                Exit For
            End If
        Next
        dtManodopera.Rows.Add(drManodopera)
    End Sub

    Private Sub AddRigaMacchine(ByRef dr As DataRow, ByRef dtMacchine As DataTable)
        Dim drMacchine As DataRow = dtMacchine.NewRow
        drMacchine.Item("Id_CDG") = 0
        drMacchine.Item("OrigineApp") = 1
        drMacchine.Item("APP_CDG_Generale_ID") = dr.Item("id")
        drMacchine.Item("Sa_Cod") = dr.Item("Mac_Sa_Cod")
        drMacchine.Item("Tipo") = dr.Item("Mac_Tipo")
        drMacchine.Item("Mac_Cod") = dr.Item("Mac_Cod")
        drMacchine.Item("Prodotto_Des") = dr.Item("Mac_Des")
        drMacchine.Item("ID_Attivita") = dr.Item("Id_Attivita")
        SetOraInizioFine(drMacchine, dr.Item("Data_Ora_Inizio"), dr.Item("Data_Ora_Fine"))
        drMacchine.Item("Udm_Cod") = 141
        dtMacchine.Rows.Add(drMacchine)
    End Sub

    Private Sub AddRigaImpianti(ByRef dr As DataRow,
                                ByRef dtImpianti As DataTable,
                                ByVal data_movimento As Date,
                                ByVal attPoliennale As Boolean,
                                ByRef objParametri As AgronicaCoreParametri)

        Dim drImpianti As DataRow

        If Not attPoliennale Then

            ' se non c'è la distinta prendo quello valida per la data movimento
            Dim Progetto_Cod = dr.Item("Progetto_Cod")
            If Progetto_Cod = 0 Then
                Dim objProg As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
                Progetto_Cod = objProg.CodProgetto_from_DataValidita(dr.Item("Piva"), dr.Item("Sa_Cod"), dr.Item("Appezza"), dr.Item("Id_Reg"), data_movimento, objParametri)
            End If

            drImpianti = dtImpianti.NewRow
            drImpianti.Item("Piva") = dr.Item("Piva")
            drImpianti.Item("Sa_Cod") = dr.Item("Sa_Cod")
            drImpianti.Item("Appezza") = dr.Item("Appezza")
            drImpianti.Item("Id_Destinazione") = dr.Item("Id_Reg")
            drImpianti.Item("Progetto_Cod") = Progetto_Cod
            drImpianti.Item("Valore") = dr.Item("Sup_Imp")
            dtImpianti.Rows.Add(drImpianti)

        Else

            Dim leggi_distinta As New Impresa_Progetti_R
            Dim distinteSuCuiSpalmare = leggi_distinta.Trova_Distinte_Attive(dr.Item("Piva"),
                                              dr.Item("Sa_Cod"),
                                              dr.Item("Appezza"),
                                              dr.Item("Id_Reg"),
                                              data_movimento, objParametri)
            Dim residuo As Decimal = dr.Item("Sup_Imp")
            If distinteSuCuiSpalmare.Rows.Count > 0 Then
                For i As Integer = 0 To distinteSuCuiSpalmare.Rows.Count - 1
                    drImpianti = dtImpianti.NewRow
                    drImpianti.Item("Piva") = dr.Item("Piva")
                    drImpianti.Item("Sa_Cod") = dr.Item("Sa_Cod")
                    drImpianti.Item("Appezza") = dr.Item("Appezza")
                    drImpianti.Item("Id_Destinazione") = dr.Item("Id_Reg")
                    drImpianti.Item("Progetto_Cod") = distinteSuCuiSpalmare.Rows(i).Item("Progetto_Cod")
                    drImpianti.Item("Valore") = dr.Item("Sup_Imp") / distinteSuCuiSpalmare.Rows.Count
                    residuo = residuo - drImpianti.Item("Valore")
                    dtImpianti.Rows.Add(drImpianti)
                Next
                If residuo > 0 Then
                    dtImpianti.Rows(0).Item("Valore") = dtImpianti.Rows(0).Item("Valore") + residuo
                End If
            End If

        End If

    End Sub

    Public Function ImportaCDGDaTabelleAPP(ByVal Piva_CDG As String,
                                           ByVal Id_Agenda As Integer,
                                           ByRef msgFinale As StringBuilder,
                                           ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                           ByVal objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                           Optional ByVal guid As String = "",
                                           Optional ByRef riferimento As String = ""
                                           ) As Boolean

        '===========================================================================================================================
        '===========================================================================================================================
        '  ATTENZIONE A CAMBIARE LA GESTIONE DEL MESSAGGIO DI ERRORE PERCHE' QUANDO QUESTA FUNZIONE VIENE CHIAMATA DAL BOMARDINO
        '  VIENE TESTATO IL FATTO CHE IL MESSAGGIO SIA A "" PER CAPIRE CHE NON C'ERANO ATTIVITA' DA IMPORTARE DA APP
        '===========================================================================================================================
        '===========================================================================================================================

        Dim msgErr As New StringBuilder
        Dim esitoFinale As Boolean = False

        Try

            Dim id As String = ""
            Dim Piva As String = ""
            Dim Descrizione As String = ""
            Dim Des_Impianti As Boolean = True
            Dim Data_Inserimento As Date
            Dim Data_Ora_Inizio As Date
            Dim Cod_RisUm As Integer = 0
            Dim Mac_Cod As Integer = 0
            Dim Sup_Tot As Decimal = 0
            Dim Lav_Cod As Integer = 0
            Dim Id_Agenda_CDG As Integer = 0
            Dim dati_CDG As String = ""
            Dim impianti As String = ""

            Dim dtAttivita = LeggiCDGDaTabelleAPP(Piva_CDG, Id_Agenda, objParametri_Server, guid)

            If dtAttivita.Rows.Count > 0 Then

                Dim dtManodopera As New DataTable
                Dim dtMacchine As New DataTable
                Dim dtImpianti As New DataTable
                Dim dtProgetti As New DataTable

                InitDataTable(dtManodopera, dtMacchine, dtImpianti, dtProgetti)

                Dim objQualifichexTariffe As New AgronicaCoreContabDAL.QualificheXTariffe_R
                Dim dtTariffe = objQualifichexTariffe.Leggi("", 1, 0, 0, "", "", objParametri_Server)

                Dim attivitaDaImportare As Integer = 0
                Dim attivitaImportate As Integer = 0
                Dim attivitaNonImportate As Integer = 0
                Dim listaCodiciRicette As New StringBuilder
                Dim importaAttivita As Boolean = False

                'Dim dtTestate As DataTable = dtAttivita.DefaultView.ToTable(True, "Id", "Piva", "Data_Inserimento", "Id_Ricetta", "Id_Agenda", "attivita", "des_lib", "Lav_Cod", "Data_Movimento")

                For Each dr As DataRow In dtAttivita.Rows

                    ' Verifico se l'attività è poliennale
                    Dim leggi_Attivita As New Attivita_R
                    Dim attPoliennale = False
                    Dim dtAtt As New DataTable
                    If dr.Item("Id_Attivita") <> 0 Then
                        dtAtt = leggi_Attivita.Leggi(dr.Item("Id_Attivita"), "Attivita_Poliannuale = 1", "", objParametri_Server)
                    End If
                    If dtAtt.Rows.Count = 1 Then
                        attPoliennale = True
                    End If

                    If dr.Item("id") <> id Then

                        attivitaDaImportare += 1

                        If importaAttivita AndAlso ImportaCDG(id, Piva, Id_Agenda, Id_Agenda_CDG, Lav_Cod, Descrizione, Data_Inserimento, Sup_Tot, dtManodopera, dtMacchine, dtProgetti, dtImpianti, impianti, dati_CDG, msgErr, objParametri_Server, riferimento) Then
                            attivitaImportate += 1
                        End If

                        importaAttivita = IsDBNull(dr.Item("Id_Ricetta")) Or Not IsDBNull(dr.Item("Id_Agenda"))

                        ' salto le attività collegate a ricette non ribaltate in qdc
                        If Not importaAttivita Then
                            If dr.Item("id") <> id Then
                                id = dr.Item("id")
                                If (listaCodiciRicette.Length > 0) Then
                                    listaCodiciRicette.Append(", ")
                                End If
                                listaCodiciRicette.Append(dr.Item("Ricetta_Numero"))
                                attivitaNonImportate += 1
                            End If
                            Continue For
                        End If

                        id = dr.Item("id")
                        Piva = dr.Item("Piva")
                        Data_Inserimento = CDate(dr.Item("Data_Ora_Inizio")).Date
                        Id_Agenda = IIf(IsDBNull(dr.Item("Id_Agenda")), 0, dr.Item("Id_Agenda"))
                        Id_Agenda_CDG = 0
                        Descrizione = dr.Item("attivita")
                        If Not IsDBNull(dr.Item("Note")) AndAlso Not String.IsNullOrEmpty(dr.Item("Note")) Then
                            Descrizione = Left(Descrizione & " - " & dr.Item("Note"), 3000)
                        End If
                        Des_Impianti = True
                        Cod_RisUm = 0
                        Mac_Cod = 0
                        Sup_Tot = 0
                        Lav_Cod = 0
                        dati_CDG = ""
                        impianti = ""
                        dtManodopera.Clear()
                        dtMacchine.Clear()
                        dtImpianti.Clear()
                        dtProgetti.Clear()

                        If Id_Agenda <> 0 Then

                            Dim Data_Movimento = dr.Item("Data_Movimento")
                            Descrizione = dr.Item("Ricetta_Numero") & " " & dr.Item("des_lib")
                            Lav_Cod = dr.Item("Lav_Cod")

                            ' Chiamata Bombardino per scrivere lo scarico prodotti
                            Dim leggi As New CDG_DAL_R
                            Dim scrivi_BIZ As New CDG_BIZ_W
                            Dim rBombardino = scrivi_BIZ.AllineaCostiDaCampagna_Bombardino(Piva, Id_Agenda, Data_Movimento, True, objParametri_Server, objParametri_Utenti)

                            ' Ricavo l'Id agenda CDG
                            Dim dt_agenda_rif = leggi.Leggi_Agenda_Riferimento(Piva, Id_Agenda, 0, True, objParametri_Server)
                            If dt_agenda_rif.Rows.Count > 0 Then
                                Id_Agenda_CDG = dt_agenda_rif.Rows(0).Item("Id_Agenda_Rif")
                            End If

                            ' se esiste azienda cdg recupero i dati relativi
                            'If Id_Agenda_CDG <> 0 Then
                            dati_CDG = leggi.Leggi_CDG(Piva, Id_Agenda, 0, Id_Agenda_CDG, Data_Movimento, 0, 0, "", 0, objParametri_Server, True)
                            'Else
                            'impianti = leggi.Leggi_Impianti_Dettagli(Piva, Id_Agenda, Data_Movimento, False, 0, True, attPoliennale, objParametri_Server)
                            ' End If

                        End If

                    End If

                    ' aggiunge righe manodopera / macchine
                    If dr.Item("Data_Ora_Inizio") <> Data_Ora_Inizio OrElse dr.Item("Cod_RisUm") <> Cod_RisUm OrElse dr.Item("Mac_Cod") <> Mac_Cod Then
                        Des_Impianti = Cod_RisUm = 0 And Mac_Cod = 0
                        Data_Ora_Inizio = dr.Item("Data_Ora_Inizio")
                        Cod_RisUm = dr.Item("Cod_RisUm")
                        Mac_Cod = dr.Item("Mac_Cod")
                        If Cod_RisUm <> 0 Then
                            AddRigaManodopera(dr, dtManodopera, dtTariffe)
                        ElseIf Mac_Cod <> 0 Then
                            AddRigaMacchine(dr, dtMacchine)
                        End If
                        dtImpianti.Clear()
                        dtProgetti.Clear()
                        Sup_Tot = 0
                    End If

                    ' aggiunge riga impianti (in caso di attività e distinte poliennali più di una)
                    If dr.Item("Id_Reg") <> 0 Then
                        'Qui siamo nel caso in cui sono state inserite sull'APP attività non legate al QdC
                        Sup_Tot += dr.Item("Sup_Imp")
                        AddRigaImpianti(dr, dtImpianti, Data_Inserimento, attPoliennale, objParametri_Server)
                        If Id_Agenda = 0 AndAlso Des_Impianti AndAlso Not IsDBNull(dr.Item("Veg_Des")) Then
                            Dim Des_Impianto = " (" & dr.Item("Veg_Des") & " [" & dr.Item("Cul_Des") & "])"
                            If Descrizione.IndexOf(Des_Impianto) = -1 Then
                                Descrizione &= Des_Impianto
                            End If
                        End If
                    ElseIf dr.Item("Id_Imputazione") <> 0 Then
                        Dim drProgetti As DataRow
                        drProgetti = dtProgetti.NewRow
                        drProgetti.Item("Imputazione_Cod") = dr.Item("Id_Imputazione")
                        drProgetti.Item("Valore") = 100
                        dtProgetti.Rows.Add(drProgetti)
                    End If

                Next

                If importaAttivita AndAlso ImportaCDG(id, Piva, Id_Agenda, Id_Agenda_CDG, Lav_Cod, Descrizione, Data_Inserimento, Sup_Tot, dtManodopera, dtMacchine, dtProgetti, dtImpianti, impianti, dati_CDG, msgErr, objParametri_Server, riferimento) Then
                    attivitaImportate += 1
                End If

                msgFinale.Append("Sono state importate correttamente " & attivitaImportate & " attività su " & attivitaDaImportare)

                If attivitaNonImportate > 0 Then
                    msgFinale.Append("<br><br>" & attivitaNonImportate & " attività non importate perchè manca il passaggio da Brogliaccio a QDCA:<br>" & listaCodiciRicette.ToString)
                End If

                If msgErr.Length > 0 Then
                    msgFinale.Append("<br><br>ERRORI:<br>" & msgErr.ToString)
                End If

            End If

            esitoFinale = True

        Catch ex As Exception

            esitoFinale = False
            msgFinale.Append(Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True))

        End Try

        Return esitoFinale

    End Function

    Public Function MarcaCDGImportataAPP(
        ByVal ID As String,
        ByVal Errori As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "MarcaCDGImportataAPP()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            Stb.Length = 0

            Dim xErr As String = ""
            If Errori <> "" Then
                Dim xlenErr As Integer = 0
                If Errori.Length > 1999 Then
                    xlenErr = 1999
                Else
                    xlenErr = Errori.Length - 1
                End If
                xErr = Errori.Substring(0, xlenErr)
            End If

            Stb.Append(" UPDATE APP_CDG_Generale ")
            Stb.Append(" SET ")
            Stb.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            Stb.Append("         ,Data_Modifica= " & Agro_SQL_SaveDateTime(Date.Now) & " ")
            Stb.Append("         ,Importato_Data= " & Agro_SQL_SaveDateTime(Date.Now) & " ")
            Stb.Append("         ,Importato_Errore = '" & Agro_SQL_SaveText(xErr) & "' ")
            Stb.Append(" WHERE   ID = '" & Agro_SQL_SaveText(ID) & "' ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function LeggiOreCDG(
        ByVal Piva As String,
        ByVal Cod_Risum As Integer,
        ByVal Mac_Cod As Integer,
        ByVal Data_Inizio As DateTime,
        ByVal Data_Fine As DateTime,
        ByVal Inviate As Integer,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "LeggiCDGOrigineAPP()"
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            '---------------------------------------------
            Stb.Append("select a.piva, id_cdg, a.id_agenda, a.cod_risum, a.mac_cod, descrizione, OrigineApp, c.des_lib, ")
            Stb.Append("cast(a.data_inserimento As Date) as data_inserimento, data_ora_inizio, data_ora_fine, qta, prezzo_unitario, valore_totale ")
            Stb.Append("from cdg_testata a ")
            Stb.Append("inner join(select distinct piva, Cod_Risum, Mac_Cod, cast(data_inserimento As Date) as data_inserimento ")
            Stb.Append("from cdg_testata where cdg_testata.Budget = 0 And OrigineApp IN (1,2,4) and (cod_risum<>0 or mac_cod<>0)) b ")
            Stb.Append("on a.piva = b.piva and a.cod_risum=b.cod_risum and a.mac_cod=b.mac_cod and cast(a.data_inserimento As Date) = b.data_inserimento ")
            Stb.Append("inner join agenda c on a.piva = c.piva and a.id_agenda = c.id_agenda ")
            Stb.Append("where b.data_inserimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")
            Stb.Append("and b.data_inserimento <= " & Agro_SQL_SaveDate(Data_Fine) & " And a.Budget = 0 ")

            ' filtro su piva azienda
            If Not String.IsNullOrEmpty(Piva) Then
                Stb.Append(" and a.piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Cod_Risum <> 0 Then
                Stb.Append(" and a.cod_risum = " & Agro_SQL_SaveNum(Cod_Risum) & " ")
            End If

            If Mac_Cod <> 0 Then
                Stb.Append(" and a.mac_cod = " & Agro_SQL_SaveNum(Mac_Cod) & " ")
            End If


            If Inviate = 1 Then
                ' considera ore app anche quelle già inviate
                Stb.Append(" and a.OrigineApp IN (1,2,4) ")
            Else
                ' considera ore app anche quelle già ricongiunte
                Stb.Append(" and a.OrigineApp IN (1,2) ")
                ' considera anche ore inserite su web non inviate
                ' Stb.Append(" and a.OrigineApp IN (0,1,2) ")
            End If

            Stb.Append("order by a.piva, a.cod_risum, a.mac_cod, data_ora_inizio, data_ora_fine")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return dt

    End Function

    Public Function RicongiungiOreCDG(ByVal Piva As String,
                                        ByVal Cod_Persona As Integer,
                                        ByVal Cod_Macchina As Integer,
                                        ByVal Data_Inizio As DateTime,
                                        ByVal Data_Fine As DateTime,
                                        ByVal Inviate As Integer,
                                        ByRef msgFinale As StringBuilder,
                                        ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As Boolean

        Dim msgErr As New StringBuilder
        Dim esitoFinale As Boolean = False

        Try

            Dim dtOre = LeggiOreCDG(Piva, Cod_Persona, Cod_Macchina, Data_Inizio, Data_Fine, Inviate, objParametri_Server)

            If dtOre.Rows.Count > 0 Then

                Dim id_cdg As Integer = 0
                Dim id_agenda_cdg As Integer = 0
                Dim Data_Inserimento As Date
                Dim Cod_RisUm As Integer = 0
                Dim Mac_Cod As Integer = 0

                Dim dtOreRisorse As New DataTable
                dtOreRisorse.Columns.Add(New DataColumn("Piva", GetType(String)))
                dtOreRisorse.Columns.Add(New DataColumn("Id_CDG", GetType(Integer)))
                dtOreRisorse.Columns.Add(New DataColumn("Id_Agenda", GetType(Integer)))
                dtOreRisorse.Columns.Add(New DataColumn("Des_Lib", GetType(String)))
                dtOreRisorse.Columns.Add(New DataColumn("OrigineApp", GetType(Integer)))
                dtOreRisorse.Columns.Add(New DataColumn("Cod_Risum", GetType(Integer)))
                dtOreRisorse.Columns.Add(New DataColumn("Mac_Cod", GetType(Integer)))
                dtOreRisorse.Columns.Add(New DataColumn("Data_Inserimento", GetType(Date)))
                dtOreRisorse.Columns.Add(New DataColumn("Ora_Inizio", GetType(Date)))
                dtOreRisorse.Columns.Add(New DataColumn("Ora_Fine", GetType(Date)))
                dtOreRisorse.Columns.Add(New DataColumn("Qta", GetType(Decimal)))
                dtOreRisorse.Columns.Add(New DataColumn("Prezzo_Unitario", GetType(Decimal)))
                dtOreRisorse.Columns.Add(New DataColumn("Valore_Totale", GetType(Decimal)))
                dtOreRisorse.Columns.Add(New DataColumn("Aggiorna", GetType(Integer)))

                Dim oreDaRicongiungere As Integer = 0
                Dim oreRicongiunte As Integer = 0
                Dim ricongiungiOre As Boolean = False

                For Each dr As DataRow In dtOre.Rows

                    If dr.Item("data_inserimento") <> Data_Inserimento OrElse dr.Item("Cod_RisUm") <> Cod_RisUm OrElse dr.Item("Mac_Cod") <> Mac_Cod Then

                        oreDaRicongiungere += 1

                        If ricongiungiOre AndAlso RicongiungiOreRisorse(dtOreRisorse) Then
                            If AggiornaOreCDG(dtOreRisorse, objParametri_Server) Then
                                oreRicongiunte += 1
                            End If
                        End If

                        ricongiungiOre = True

                        Piva = dr.Item("Piva")
                        id_cdg = dr.Item("id_cdg")
                        id_agenda_cdg = dr.Item("id_agenda")
                        Data_Inserimento = dr.Item("Data_Inserimento")
                        Cod_RisUm = dr.Item("Cod_RisUm")
                        Mac_Cod = dr.Item("Mac_Cod")

                        dtOreRisorse.Clear()

                    End If

                    Dim drOreRisorse As DataRow = dtOreRisorse.NewRow
                    drOreRisorse.Item("Piva") = dr.Item("piva")
                    drOreRisorse.Item("Id_CDG") = dr.Item("id_cdg")
                    drOreRisorse.Item("Id_Agenda") = dr.Item("id_agenda")
                    drOreRisorse.Item("Des_Lib") = dr.Item("des_lib")
                    drOreRisorse.Item("OrigineApp") = dr.Item("OrigineApp")
                    drOreRisorse.Item("Cod_RisUm") = dr.Item("Cod_RisUm")
                    drOreRisorse.Item("Mac_Cod") = dr.Item("Mac_Cod")
                    drOreRisorse.Item("Data_Inserimento") = dr.Item("Data_Inserimento")
                    drOreRisorse.Item("Ora_Inizio") = dr.Item("Data_Ora_Inizio")
                    drOreRisorse.Item("Ora_Fine") = dr.Item("Data_Ora_Fine")
                    drOreRisorse.Item("Qta") = dr.Item("qta")
                    drOreRisorse.Item("Prezzo_Unitario") = dr.Item("prezzo_unitario")
                    drOreRisorse.Item("Valore_Totale") = dr.Item("valore_totale")
                    drOreRisorse.Item("Aggiorna") = 0
                    dtOreRisorse.Rows.Add(drOreRisorse)

                Next

                If ricongiungiOre AndAlso RicongiungiOreRisorse(dtOreRisorse) Then
                    If AggiornaOreCDG(dtOreRisorse, objParametri_Server) Then
                        oreRicongiunte += 1
                    End If
                End If

                If oreRicongiunte > 0 Then
                    msgFinale.Append("Sono state ricongiunte correttamente " & oreRicongiunte & " ore su " & oreDaRicongiungere)
                End If

                If msgErr.Length > 0 Then
                    msgFinale.Append("<br><br>ERRORI:<br>" & msgErr.ToString)
                End If

            End If

            esitoFinale = True

        Catch ex As Exception

            esitoFinale = False
            msgFinale.Append(Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True))

        End Try

        Return esitoFinale

    End Function

    ' Algoritmo per ricongiungimento ore
    Public Function RicongiungiOreRisorse(ByRef dtOre As DataTable) As Boolean

        Dim next_ora_inizio As DateTime = AGRODATAFINE
        Dim last_ora_fine As DateTime = AGRODATAINIZIO
        Dim aggiorna_ore As Boolean = False

        For i As Integer = 0 To dtOre.Rows.Count - 1

            Dim ora_inizio = dtOre.Rows(i).Item("Ora_Inizio")
            Dim ora_fine = dtOre.Rows(i).Item("Ora_Fine")
            Dim aggiorna As Integer = 0

            If i < dtOre.Rows.Count - 1 Then
                next_ora_inizio = dtOre.Rows(i + 1).Item("Ora_Inizio")
                If ora_fine > next_ora_inizio Then
                    dtOre.Rows(i).Item("Ora_Fine") = next_ora_inizio
                    aggiorna = 1
                End If
            End If

            If ora_inizio < last_ora_fine And ora_fine > last_ora_fine And last_ora_fine < next_ora_inizio Then
                dtOre.Rows(i).Item("Ora_Fine") = last_ora_fine
                aggiorna = 1
            End If

            If Format(ora_fine, "HH:mm") <> "23:59" Then
                last_ora_fine = ora_fine
            End If

            If aggiorna = 1 Then
                dtOre.Rows(i).Item("Qta") = Math.Round(DateDiff(DateInterval.Minute, dtOre.Rows(i).Item("Ora_Inizio"), dtOre.Rows(i).Item("Ora_Fine")) / 60, 4)
                dtOre.Rows(i).Item("Valore_Totale") = dtOre.Rows(i).Item("Qta") * dtOre.Rows(i).Item("Prezzo_Unitario")
            End If

            If dtOre.Rows(i).Item("OrigineApp") = 1 Then
                dtOre.Rows(i).Item("OrigineApp") = 2
                If aggiorna = 0 Then
                    aggiorna = 2
                End If
            End If

            If aggiorna <> 0 Then
                dtOre.Rows(i).Item("Aggiorna") = aggiorna
                aggiorna_ore = True
            End If

        Next

        Return aggiorna_ore

    End Function

    ' Salvo le righe CDG che sono state modificate
    Public Function AggiornaOreCDG(ByRef dtOre As DataTable, ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False
        Dim esitoFinale As Boolean = False

        Try

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Apro la connessione al DB
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, FlagTransazioneLocale, objParametri_Server)
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            Dim AgronicaLogAgenda As New AgronicaCoreContabDAL.AgronicaLogAgenda_W

            For Each dr As DataRow In dtOre.Rows

                If dr.Item("Aggiorna") <> 0 Then

                    AggiornaCDGTestata(
                        dr.Item("Piva"),
                        dr.Item("Id_CDG"),
                        dr.Item("Ora_Inizio"),
                        dr.Item("Ora_Fine"),
                        dr.Item("Qta"),
                        dr.Item("Valore_Totale"),
                        dr.Item("OrigineApp"),
                        objParametri_Server)

                    ' aggiorno l'agenda solo se sono cambiate le ore
                    If dr.Item("Aggiorna") = 1 Then

                        AggiornaCDGAgenda(
                        dr.Item("Piva"),
                        dr.Item("Id_Agenda"),
                        objParametri_Server)

                        AgronicaLogAgenda.Scrivi(dr.Item("Data_Inserimento"),
                            enum_TipoOperazioneDB.Modifica,
                            dr.Item("Des_Lib"),
                            dr.Item("Id_Agenda"),
                            dr.Item("Piva"),
                            0,
                            4500,
                            5,
                            objParametri_Server)

                    End If

                End If

            Next

            esitoFinale = True

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Chiudo la connessione al DB
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        Catch ex As Exception

            'Faccio il rollback della transazione
            If Not objParametri_Server.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If

        Finally

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

        End Try

        Return esitoFinale

    End Function

    Public Function AggiornaCDGTestata(
        ByRef Piva As String,
        ByRef Id_CDG As Integer,
        ByVal Data_Ora_Inizio As DateTime,
        ByVal Data_Ora_Fine As DateTime,
        ByVal Qta As Decimal,
        ByVal Valore_Totale As Decimal,
        ByRef OrigineApp As Integer,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "AggiornaCDGTestata()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            Stb.Append(" UPDATE CDG_Testata ")
            Stb.Append(" SET ")
            Stb.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            Stb.Append("         ,Data_Modifica= " & Agro_SQL_SaveDateTime(Date.Now) & " ")
            'Stb.Append("         ,Data_Ora_Inizio= " & Agro_SQL_SaveDateTime(Data_Ora_Inizio) & " ")
            Stb.Append("         ,Data_Ora_Fine= " & Agro_SQL_SaveDateTime(Data_Ora_Fine) & " ")
            Stb.Append("         ,Qta = " & Agro_SQL_SaveNum(Qta) & " ")
            Stb.Append("         ,Valore_Totale = " & Agro_SQL_SaveNum(Valore_Totale) & " ")
            Stb.Append("         ,OrigineApp = " & Agro_SQL_SaveNum(OrigineApp) & " ")
            Stb.Append(" WHERE   Piva = '" & Agro_SQL_SaveText(Piva) & "' And Budget = 0 ")
            Stb.Append(" AND Id_CDG = " & Agro_SQL_SaveNum(Id_CDG))

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function AggiornaCDGAgenda(
        ByRef Piva As String,
        ByRef Id_Agenda As Integer,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "AggiornaCDGAgenda()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            Stb.Append(" UPDATE Agenda ")
            Stb.Append(" SET ")
            Stb.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            Stb.Append("         ,Data_Modifica= " & Agro_SQL_SaveDateTime(Date.Now) & " ")
            Stb.Append(" WHERE   Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            Stb.Append(" AND Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda))
            Stb.Append(" AND Lav_Cod = 4500")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

End Class
