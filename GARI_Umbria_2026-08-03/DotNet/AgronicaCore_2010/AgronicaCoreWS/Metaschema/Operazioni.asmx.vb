Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreMetaSchemaDAL
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Imports AgronicaCoreDTOStd
Imports AgronicaCoreDTOStd.InData.Metaschema

Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider

Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreModelsSTD
Imports System
Imports AgronicaCoreModelsSTD.exceptions

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class Operazioni
    Inherits System.Web.Services.WebService

#Region "CARICA COMBO LAVORAZIONI"
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboLavorazioni(objP_server As String, objP_utenti As String, FiltraImpostazioniUtente As Boolean, Tipo_GruppoOperazioni As String) As RispostaStandard
        Return CaricaComboLavorazioni_ConFiltro(objP_server, objP_utenti, FiltraImpostazioniUtente, Tipo_GruppoOperazioni, "")
    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboLavorazioni_NG(ByVal InData As CoreWS_Generic(Of CaricaComboLavorazioni)) As RispostaStandard
        Return CaricaComboLavorazioni_ConFiltro(InData.objP.objP_server, InData.objP.objP_utenti, InData.InData.FiltraImpostazioniUtente, InData.InData.Tipo_GruppoOperazioni, "")
    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboLavorazioni_ConFiltro_NG(ByVal InData As CoreWS_Generic(Of CaricaComboLavorazioni_ConFiltro)) As RispostaStandard

        Dim r As New RispostaStandard

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If InData.InData.Tipo_GruppoOperazioni = "" Then
            InData.InData.Tipo_GruppoOperazioni = "'C','E','V','P','Z'"
        End If

        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim leggiLingua As New AgronicaCoreUtentiDAL.Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)


            Dim objUtente As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim dt_FiltroUtente As DataTable = objUtente.LeggiQryParametrica(TipiEnumerativi.enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_LAVORAZIONI,
                                                           1, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                           "", "", "", objParametri_Utenti)

            Dim str_filtroUtente As String = ""
            If dt_FiltroUtente.Rows.Count > 0 AndAlso InData.InData.FiltraImpostazioniUtente Then
                Dim f As New List(Of String)
                For Each drF In dt_FiltroUtente.Rows
                    If Not IsDBNull(dt_FiltroUtente.Rows(0).Item("ID_0")) AndAlso dt_FiltroUtente.Rows(0).Item("ID_0").ToString() <> "" Then
                        f.Add(drF.Item("ID_0"))
                    End If
                Next
                str_filtroUtente = " AND Operazioni.Lav_Cod IN (" & String.Join(", ", f) & ") "
            End If

            'Dim FiltroAggiuntivo As String = STR_OP_NON_GESTITE + "AND ( GruppoOperazioni.Tipo IN ('C','Z','P','E')) "

            ' VAnni: 20/4/2018: il parametro filtro al momento non viene incluso nella query perchè viene sempre passato come stringa vuota,
            ' e quindi onde evitare il problema della sql injection segnalata da Leonardo Sistemi lo rimuovo dal filtro aggiuntivo..
            Dim FiltroAggiuntivo As String = CostantiPersonalizzate.STR_OP_NON_GESTITE & " " & InData.InData.Filtro & " AND GruppoOperazioni.Tipo IN (" & InData.InData.Tipo_GruppoOperazioni & ") " & str_filtroUtente
            'Dim FiltroAggiuntivo As String = CostantiPersonalizzate.STR_OP_NON_GESTITE & " AND GruppoOperazioni.Tipo IN (" & Tipo_GruppoOperazioni & ") " & str_filtroUtente

            ' Dim Ordinamento As String = " GruppoOperazioni.Tipo, GruppoOperazioni.GRU_COD, Lav_Des "
            Dim Ordinamento As String = " Operazioni.Lav_Des "

            Dim objOperazioniLeggi As New AgronicaCoreMetaSchemaDAL.Operazioni_R
            Dim DTOperazioni As DataTable = objOperazioniLeggi.Leggi(0, 0, 0, "", 0, "", "", False, False, False, False,
                                                                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                                FiltroAggiuntivo, Ordinamento, objParametri_Server)


            'Prelevo i preferiti
            Dim ui_R As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read()
            Dim dtPref As DataTable = ui_R.Leggi_Utente_Poi_SuperUser(TipiEnumerativi.enum_Impostazioni_Utenti.UTENTE_OPERAZIONI_QDC_PREFERITE, 1, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            Dim lavs As String = ""
            If Not IsNothing(dtPref) AndAlso dtPref.Rows.Count > 0 Then
                lavs = dtPref.Rows(0).Item("Impostazione_Valore_1")
            End If
            Dim lavstr() As String = lavs.Split("|")

            Dim JArrayListaOp As New JArray()
            'JArrayListaOp.Add(New JObject(New JProperty("lav_cod", ""), New JProperty("lav_des", "Nessuna Selezione")))
            For Each dr In DTOperazioni.Rows
                JArrayListaOp.Add(New JObject(New JProperty("lav_cod", dr.Item("LAV_COD")), New JProperty("lav_des", dr.Item("LAV_DES")), New JProperty("gru_cod", dr.Item("GRU_COD")), New JProperty("gru_des", dr.Item("GRU_DES")), New JProperty("preferito", If(lavstr.Contains(dr.Item("LAV_COD")), 1, 0))))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(JArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = Gias.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboLavorazioni_ConFiltro(objP_server As String, objP_utenti As String, FiltraImpostazioniUtente As Boolean, Tipo_GruppoOperazioni As String, Filtro As String) As RispostaStandard

        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If Tipo_GruppoOperazioni = "" Then
            Tipo_GruppoOperazioni = "'C','E','V','P','Z'"
        End If

        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim leggiLingua As New AgronicaCoreUtentiDAL.Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            Dim objUtente As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim dt_FiltroUtente As DataTable = objUtente.LeggiQryParametrica(TipiEnumerativi.enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_LAVORAZIONI,
                                                           1, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                           "", "", "", objParametri_Utenti)

            Dim str_filtroUtente As String = ""
            If dt_FiltroUtente.Rows.Count > 0 Then
                Dim f As New List(Of String)
                For Each drF In dt_FiltroUtente.Rows
                    If Not IsDBNull(dt_FiltroUtente.Rows(0).Item("ID_0")) AndAlso dt_FiltroUtente.Rows(0).Item("ID_0").ToString() <> "" Then
                        f.Add(drF.Item("ID_0"))
                    End If
                Next
                str_filtroUtente = " AND Operazioni.Lav_Cod IN (" & String.Join(", ", f) & ") "
            End If

            'Dim FiltroAggiuntivo As String = STR_OP_NON_GESTITE + "AND ( GruppoOperazioni.Tipo IN ('C','Z','P','E')) "

            ' VAnni: 20/4/2018: il parametro filtro al momento non viene incluso nella query perchè viene sempre passato come stringa vuota,
            ' e quindi onde evitare il problema della sql injection segnalata da Leonardo Sistemi lo rimuovo dal filtro aggiuntivo..
            Dim FiltroAggiuntivo As String = CostantiPersonalizzate.STR_OP_NON_GESTITE & " " & Filtro & " AND GruppoOperazioni.Tipo IN (" & Tipo_GruppoOperazioni & ") " & str_filtroUtente & " AND" & CostantiPersonalizzate.STR_OP_NON_GESTITE_BS
            'Dim FiltroAggiuntivo As String = CostantiPersonalizzate.STR_OP_NON_GESTITE & " AND GruppoOperazioni.Tipo IN (" & Tipo_GruppoOperazioni & ") " & str_filtroUtente

            ' Dim Ordinamento As String = " GruppoOperazioni.Tipo, GruppoOperazioni.GRU_COD, Lav_Des "
            Dim Ordinamento As String = " Operazioni.Lav_Des "

            Dim objOperazioniLeggi As New AgronicaCoreMetaSchemaDAL.Operazioni_R
            Dim DTOperazioni As DataTable = objOperazioniLeggi.Leggi(0, 0, 0, "", 0, "", "", False, False, False, False,
                                                                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                                FiltroAggiuntivo, Ordinamento, objParametri_Server)


            'Prelevo i preferiti
            Dim ui_R As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read()
            Dim dtPref As DataTable = ui_R.Leggi_Utente_Poi_SuperUser(TipiEnumerativi.enum_Impostazioni_Utenti.UTENTE_OPERAZIONI_QDC_PREFERITE, 1, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            Dim lavs As String = ""
            If Not IsNothing(dtPref) AndAlso dtPref.Rows.Count > 0 Then
                lavs = dtPref.Rows(0).Item("Impostazione_Valore_1")
            End If
            Dim lavstr() As String = lavs.Split("|")

            Dim JArrayListaOp As New JArray()
            'JArrayListaOp.Add(New JObject(New JProperty("lav_cod", ""), New JProperty("lav_des", "Nessuna Selezione")))
            For Each dr In DTOperazioni.Rows
                JArrayListaOp.Add(New JObject(New JProperty("lav_cod", dr.Item("LAV_COD")), New JProperty("lav_des", dr.Item("LAV_DES")), New JProperty("gru_cod", dr.Item("GRU_COD")), New JProperty("gru_des", dr.Item("GRU_DES")), New JProperty("preferito", If(lavstr.Contains(dr.Item("LAV_COD")), 1, 0))))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(JArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = Gias.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboLavorazioni_PerRicette(objP_server As String, objP_utenti As String, FiltraImpostazioniUtente As Boolean, Tipo_GruppoOperazioni As String) As RispostaStandard
        Return CaricaComboLavorazioni_ConFiltro_PerRicette(objP_server, objP_utenti, FiltraImpostazioniUtente, Tipo_GruppoOperazioni, "")
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboLavorazioni_PerRicette_NG(ByVal InData As CoreWS_Generic(Of CaricaComboLavorazioni)) As RispostaStandard
        Return CaricaComboLavorazioni_ConFiltro_PerRicette(InData.objP.objP_server, InData.objP.objP_utenti, InData.InData.FiltraImpostazioniUtente, InData.InData.Tipo_GruppoOperazioni, "")
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboLavorazioni_ConFiltro_PerRicette(objP_server As String, objP_utenti As String, FiltraImpostazioniUtente As Boolean, Tipo_GruppoOperazioni As String, Filtro As String) As RispostaStandard

        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If Tipo_GruppoOperazioni = "" Then
            Tipo_GruppoOperazioni = "'C','E','V','P'"
        End If

        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim leggiLingua As New AgronicaCoreUtentiDAL.Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            Dim objUtente As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim dt_FiltroUtente As DataTable = objUtente.LeggiQryParametrica(TipiEnumerativi.enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_LAVORAZIONI,
                                                           1, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                           "", "", "", objParametri_Utenti)

            Dim str_filtroUtente As String = ""
            If dt_FiltroUtente.Rows.Count > 0 Then
                Dim f As New List(Of String)
                For Each drF In dt_FiltroUtente.Rows
                    If Not IsDBNull(dt_FiltroUtente.Rows(0).Item("ID_0")) AndAlso dt_FiltroUtente.Rows(0).Item("ID_0").ToString() <> "" Then
                        f.Add(drF.Item("ID_0"))
                    End If
                Next
                str_filtroUtente = " AND Operazioni.Lav_Cod IN (" & String.Join(", ", f) & ") "
            End If

            'Se il flag IrrigazioneBS è true allora si possono creare delle nuove 'Ricette di Irrigazione'(perchè vengono gestiti dalla nuova pagina),
            'altrimenti se è false oppure la riga è mancante in Configurazione Siti allora escludo l'Irrigazione, perchè la gestione vecchia non permetteva
            'la creazione di nuove ricette.
            Dim STRINGA_OPERAZIONI_RICETTABILI As String = String.Empty

            Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim dtConfigSiti As DataTable = objConfigSiti.Leggi(0, "IrrigazioneBS", "", "", objParametri_Server)

            If Not IsNothing(dtConfigSiti) AndAlso dtConfigSiti.Rows.Count > 0 AndAlso LCase(dtConfigSiti.Rows(0).Item("Valore")) = "true" Then
                STRINGA_OPERAZIONI_RICETTABILI = CostantiPersonalizzate.STR_OP_RICETTABILI
            Else
                Dim StrOperazioniRicettabili As String = CostantiPersonalizzate.STR_OP_RICETTABILI
                Dim ArrOperazioniRicettabili() As String = StrOperazioniRicettabili.Split(",")
                Dim ListOperazioniRicettabili As List(Of String) = ArrOperazioniRicettabili.ToList()

                ListOperazioniRicettabili = ListOperazioniRicettabili.Where(Function(value) value <> LAVCOD_IRRIGAZIONE).ToList

                STRINGA_OPERAZIONI_RICETTABILI = String.Join(",", ListOperazioniRicettabili)
            End If

            Dim str_filtroRicette As String = " AND Operazioni.lav_cod IN (" & STRINGA_OPERAZIONI_RICETTABILI & ")"

            'Dim FiltroAggiuntivo As String = STR_OP_NON_GESTITE + "AND ( GruppoOperazioni.Tipo IN ('C','Z','P','E')) "

            ' VAnni: 20/4/2018: il parametro filtro al momento non viene incluso nella query perchè viene sempre passato come stringa vuota,
            ' e quindi onde evitare il problema della sql injection segnalata da Leonardo Sistemi lo rimuovo dal filtro aggiuntivo..
            Dim FiltroAggiuntivo As String = CostantiPersonalizzate.STR_OP_NON_GESTITE & " " & Filtro & " AND GruppoOperazioni.Tipo IN (" & Tipo_GruppoOperazioni & ") " & str_filtroUtente & str_filtroRicette
            'Dim FiltroAggiuntivo As String = CostantiPersonalizzate.STR_OP_NON_GESTITE & " AND GruppoOperazioni.Tipo IN (" & Tipo_GruppoOperazioni & ") " & str_filtroUtente

            ' Dim Ordinamento As String = " GruppoOperazioni.Tipo, GruppoOperazioni.GRU_COD, Lav_Des "
            Dim Ordinamento As String = " Operazioni.Lav_Des "

            Dim objOperazioniLeggi As New AgronicaCoreMetaSchemaDAL.Operazioni_R
            Dim DTOperazioni As DataTable = objOperazioniLeggi.Leggi(0, 0, 0, "", 0, "", "", False, False, False, False,
                                                                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                                FiltroAggiuntivo, Ordinamento, objParametri_Server)


            'Prelevo i preferiti
            Dim ui_R As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read()
            Dim dtPref As DataTable = ui_R.Leggi_Utente_Poi_SuperUser(TipiEnumerativi.enum_Impostazioni_Utenti.UTENTE_OPERAZIONI_QDC_PREFERITE, 1, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            Dim lavs As String = ""
            If Not IsNothing(dtPref) AndAlso dtPref.Rows.Count > 0 Then
                lavs = dtPref.Rows(0).Item("Impostazione_Valore_1")
            End If
            Dim lavstr() As String = lavs.Split("|")

            Dim JArrayListaOp As New JArray()
            'JArrayListaOp.Add(New JObject(New JProperty("lav_cod", ""), New JProperty("lav_des", "Nessuna Selezione")))
            For Each dr In DTOperazioni.Rows
                JArrayListaOp.Add(New JObject(New JProperty("lav_cod", dr.Item("LAV_COD")), New JProperty("lav_des", "Ricetta di " & dr.Item("LAV_DES")), New JProperty("gru_cod", dr.Item("GRU_COD")), New JProperty("gru_des", dr.Item("GRU_DES")), New JProperty("preferito", If(lavstr.Contains(dr.Item("LAV_COD")), 1, 0))))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(JArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = Gias.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboLavorazioni_ConFiltro_PerRicette_NG(ByVal InData As CoreWS_Generic(Of CaricaComboLavorazioni_ConFiltro)) As RispostaStandard

        Dim r As New RispostaStandard

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If InData.InData.Tipo_GruppoOperazioni = "" Then
            InData.InData.Tipo_GruppoOperazioni = "'C','E','V','P'"
        End If

        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim leggiLingua As New AgronicaCoreUtentiDAL.Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            Dim objUtente As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim dt_FiltroUtente As DataTable = objUtente.LeggiQryParametrica(TipiEnumerativi.enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_LAVORAZIONI,
                                                           1, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                           "", "", "", objParametri_Utenti)

            Dim str_filtroUtente As String = ""
            If dt_FiltroUtente.Rows.Count > 0 Then
                Dim f As New List(Of String)
                For Each drF In dt_FiltroUtente.Rows
                    If Not IsDBNull(dt_FiltroUtente.Rows(0).Item("ID_0")) AndAlso dt_FiltroUtente.Rows(0).Item("ID_0").ToString() <> "" Then
                        f.Add(drF.Item("ID_0"))
                    End If
                Next
                str_filtroUtente = " AND Operazioni.Lav_Cod IN (" & String.Join(", ", f) & ") "
            End If

            'Se il flag IrrigazioneBS è true allora si possono creare delle nuove 'Ricette di Irrigazione'(perchè vengono gestiti dalla nuova pagina),
            'altrimenti se è false oppure la riga è mancante in Configurazione Siti allora escludo l'Irrigazione, perchè la gestione vecchia non permetteva
            'la creazione di nuove ricette.
            Dim STRINGA_OPERAZIONI_RICETTABILI As String = String.Empty

            Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim dtConfigSiti As DataTable = objConfigSiti.Leggi(0, "IrrigazioneBS", "", "", objParametri_Server)

            If Not IsNothing(dtConfigSiti) AndAlso dtConfigSiti.Rows.Count > 0 AndAlso LCase(dtConfigSiti.Rows(0).Item("Valore")) = "true" Then
                STRINGA_OPERAZIONI_RICETTABILI = CostantiPersonalizzate.STR_OP_RICETTABILI
            Else
                Dim StrOperazioniRicettabili As String = CostantiPersonalizzate.STR_OP_RICETTABILI
                Dim ArrOperazioniRicettabili() As String = StrOperazioniRicettabili.Split(",")
                Dim ListOperazioniRicettabili As List(Of String) = ArrOperazioniRicettabili.ToList()

                ListOperazioniRicettabili = ListOperazioniRicettabili.Where(Function(value) value <> LAVCOD_IRRIGAZIONE).ToList

                STRINGA_OPERAZIONI_RICETTABILI = String.Join(",", ListOperazioniRicettabili)
            End If

            Dim str_filtroRicette As String = " AND Operazioni.lav_cod IN (" & STRINGA_OPERAZIONI_RICETTABILI & ")"

            'Dim FiltroAggiuntivo As String = STR_OP_NON_GESTITE + "AND ( GruppoOperazioni.Tipo IN ('C','Z','P','E')) "

            ' VAnni: 20/4/2018: il parametro filtro al momento non viene incluso nella query perchè viene sempre passato come stringa vuota,
            ' e quindi onde evitare il problema della sql injection segnalata da Leonardo Sistemi lo rimuovo dal filtro aggiuntivo..
            Dim FiltroAggiuntivo As String = CostantiPersonalizzate.STR_OP_NON_GESTITE & " " & InData.InData.Filtro & " AND GruppoOperazioni.Tipo IN (" & InData.InData.Tipo_GruppoOperazioni & ") " & str_filtroUtente & str_filtroRicette
            'Dim FiltroAggiuntivo As String = CostantiPersonalizzate.STR_OP_NON_GESTITE & " AND GruppoOperazioni.Tipo IN (" & Tipo_GruppoOperazioni & ") " & str_filtroUtente

            ' Dim Ordinamento As String = " GruppoOperazioni.Tipo, GruppoOperazioni.GRU_COD, Lav_Des "
            Dim Ordinamento As String = " Operazioni.Lav_Des "

            Dim objOperazioniLeggi As New AgronicaCoreMetaSchemaDAL.Operazioni_R
            Dim DTOperazioni As DataTable = objOperazioniLeggi.Leggi(0, 0, 0, "", 0, "", "", False, False, False, False,
                                                                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                                FiltroAggiuntivo, Ordinamento, objParametri_Server)


            'Prelevo i preferiti
            Dim ui_R As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read()
            Dim dtPref As DataTable = ui_R.Leggi_Utente_Poi_SuperUser(TipiEnumerativi.enum_Impostazioni_Utenti.UTENTE_OPERAZIONI_QDC_PREFERITE, 1, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            Dim lavs As String = ""
            If Not IsNothing(dtPref) AndAlso dtPref.Rows.Count > 0 Then
                lavs = dtPref.Rows(0).Item("Impostazione_Valore_1")
            End If
            Dim lavstr() As String = lavs.Split("|")

            Dim JArrayListaOp As New JArray()
            'JArrayListaOp.Add(New JObject(New JProperty("lav_cod", ""), New JProperty("lav_des", "Nessuna Selezione")))
            For Each dr In DTOperazioni.Rows
                JArrayListaOp.Add(New JObject(New JProperty("lav_cod", dr.Item("LAV_COD")), New JProperty("lav_des", "Ricetta di " & dr.Item("LAV_DES")), New JProperty("gru_cod", dr.Item("GRU_COD")), New JProperty("gru_des", dr.Item("GRU_DES")), New JProperty("preferito", If(lavstr.Contains(dr.Item("LAV_COD")), 1, 0))))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(JArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = Gias.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboLavorazioni_PerBrogliaccio(objP_server As String, objP_utenti As String, FiltraImpostazioniUtente As Boolean, Tipo_GruppoOperazioni As String) As RispostaStandard
        Return CaricaComboLavorazioni_ConFiltro_PerBrogliaccio(objP_server, objP_utenti, FiltraImpostazioniUtente, Tipo_GruppoOperazioni, "")
    End Function


    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboLavorazioni_PerBrogliaccio_NG(ByVal InData As CoreWS_Generic(Of CaricaComboLavorazioni)) As RispostaStandard
        Return CaricaComboLavorazioni_ConFiltro_PerBrogliaccio(InData.objP.objP_server, InData.objP.objP_utenti, InData.InData.FiltraImpostazioniUtente, InData.InData.Tipo_GruppoOperazioni, "")
    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboLavorazioni_ConFiltro_PerBrogliaccio_NG(ByVal InData As CoreWS_Generic(Of CaricaComboLavorazioni_ConFiltro)) As RispostaStandard

        Dim r As New RispostaStandard

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If InData.InData.Tipo_GruppoOperazioni = "" Then
            InData.InData.Tipo_GruppoOperazioni = "'C','E','V','P'"
        End If

        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim leggiLingua As New AgronicaCoreUtentiDAL.Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            Dim objUtente As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim dt_FiltroUtente As DataTable = objUtente.LeggiQryParametrica(TipiEnumerativi.enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_LAVORAZIONI,
                                                           1, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                           "", "", "", objParametri_Utenti)

            Dim str_filtroUtente As String = ""
            If dt_FiltroUtente.Rows.Count > 0 Then
                Dim f As New List(Of String)
                For Each drF In dt_FiltroUtente.Rows
                    If Not IsDBNull(dt_FiltroUtente.Rows(0).Item("ID_0")) AndAlso dt_FiltroUtente.Rows(0).Item("ID_0").ToString() <> "" Then
                        f.Add(drF.Item("ID_0"))
                    End If
                Next
                str_filtroUtente = " AND Operazioni.Lav_Cod IN (" & String.Join(", ", f) & ") "
            End If

            'Se il flag IrrigazioneBS è true allora si possono creare dei nuovi 'Brogliacci di Irrigazione'(perchè vengono gestiti dalla nuova pagina),
            'altrimenti se è false oppure la riga è mancante in Configurazione Siti allora escludo l'Irrigazione, perchè la gestione vecchia non permetteva
            'la creazione di nuovi brogliacci.
            Dim STRINGA_OPERAZIONI_RICETTABILI As String = String.Empty

            Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim dtConfigSiti As DataTable = objConfigSiti.Leggi(0, "IrrigazioneBS", "", "", objParametri_Server)

            If Not IsNothing(dtConfigSiti) AndAlso dtConfigSiti.Rows.Count > 0 AndAlso LCase(dtConfigSiti.Rows(0).Item("Valore")) = "true" Then
                STRINGA_OPERAZIONI_RICETTABILI = CostantiPersonalizzate.STR_OP_RICETTABILI
            Else
                Dim StrOperazioniRicettabili As String = CostantiPersonalizzate.STR_OP_RICETTABILI
                Dim ArrOperazioniRicettabili() As String = StrOperazioniRicettabili.Split(",")
                Dim ListOperazioniRicettabili As List(Of String) = ArrOperazioniRicettabili.ToList()

                ListOperazioniRicettabili = ListOperazioniRicettabili.Where(Function(value) value <> LAVCOD_IRRIGAZIONE).ToList

                STRINGA_OPERAZIONI_RICETTABILI = String.Join(",", ListOperazioniRicettabili)
            End If

            Dim str_filtroRicette As String = " AND Operazioni.lav_cod IN (" & STRINGA_OPERAZIONI_RICETTABILI & ")"

            'Dim FiltroAggiuntivo As String = STR_OP_NON_GESTITE + "AND ( GruppoOperazioni.Tipo IN ('C','Z','P','E')) "

            ' VAnni: 20/4/2018: il parametro filtro al momento non viene incluso nella query perchè viene sempre passato come stringa vuota,
            ' e quindi onde evitare il problema della sql injection segnalata da Leonardo Sistemi lo rimuovo dal filtro aggiuntivo..
            Dim FiltroAggiuntivo As String = CostantiPersonalizzate.STR_OP_NON_GESTITE & " " & InData.InData.Filtro & " AND GruppoOperazioni.Tipo IN (" & InData.InData.Tipo_GruppoOperazioni & ") " & str_filtroUtente & str_filtroRicette
            'Dim FiltroAggiuntivo As String = CostantiPersonalizzate.STR_OP_NON_GESTITE & " AND GruppoOperazioni.Tipo IN (" & Tipo_GruppoOperazioni & ") " & str_filtroUtente

            ' Dim Ordinamento As String = " GruppoOperazioni.Tipo, GruppoOperazioni.GRU_COD, Lav_Des "
            Dim Ordinamento As String = " Operazioni.Lav_Des "

            Dim objOperazioniLeggi As New AgronicaCoreMetaSchemaDAL.Operazioni_R
            Dim DTOperazioni As DataTable = objOperazioniLeggi.Leggi(0, 0, 0, "", 0, "", "", False, False, False, False,
                                                                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                                FiltroAggiuntivo, Ordinamento, objParametri_Server)


            'Prelevo i preferiti
            Dim ui_R As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read()
            Dim dtPref As DataTable = ui_R.Leggi_Utente_Poi_SuperUser(TipiEnumerativi.enum_Impostazioni_Utenti.UTENTE_OPERAZIONI_QDC_PREFERITE, 1, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            Dim lavs As String = ""
            If Not IsNothing(dtPref) AndAlso dtPref.Rows.Count > 0 Then
                lavs = dtPref.Rows(0).Item("Impostazione_Valore_1")
            End If
            Dim lavstr() As String = lavs.Split("|")

            Dim JArrayListaOp As New JArray()
            'JArrayListaOp.Add(New JObject(New JProperty("lav_cod", ""), New JProperty("lav_des", "Nessuna Selezione")))
            For Each dr In DTOperazioni.Rows
                JArrayListaOp.Add(New JObject(New JProperty("lav_cod", dr.Item("LAV_COD")), New JProperty("lav_des", "Brogliaccio di " & dr.Item("LAV_DES")), New JProperty("gru_cod", dr.Item("GRU_COD")), New JProperty("gru_des", dr.Item("GRU_DES")), New JProperty("preferito", If(lavstr.Contains(dr.Item("LAV_COD")), 1, 0))))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(JArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = Gias.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboLavorazioni_ConFiltro_PerBrogliaccio(objP_server As String, objP_utenti As String, FiltraImpostazioniUtente As Boolean, Tipo_GruppoOperazioni As String, Filtro As String) As RispostaStandard

        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If Tipo_GruppoOperazioni = "" Then
            Tipo_GruppoOperazioni = "'C','E','V','P'"
        End If

        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim leggiLingua As New AgronicaCoreUtentiDAL.Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            Dim objUtente As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim dt_FiltroUtente As DataTable = objUtente.LeggiQryParametrica(TipiEnumerativi.enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_LAVORAZIONI,
                                                           1, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                           "", "", "", objParametri_Utenti)

            Dim str_filtroUtente As String = ""
            If dt_FiltroUtente.Rows.Count > 0 Then
                Dim f As New List(Of String)
                For Each drF In dt_FiltroUtente.Rows
                    If Not IsDBNull(dt_FiltroUtente.Rows(0).Item("ID_0")) AndAlso dt_FiltroUtente.Rows(0).Item("ID_0").ToString() <> "" Then
                        f.Add(drF.Item("ID_0"))
                    End If
                Next
                str_filtroUtente = " AND Operazioni.Lav_Cod IN (" & String.Join(", ", f) & ") "
            End If

            Dim objConfig_Siti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

            Dim STRINGA_OPERAZIONI_RICETTABILI As String = String.Join(",", objConfig_Siti.Ottiene_Lav_Cod_Ricettabili(objParametri_Server, isFromNG:=False))

            Dim str_filtroRicette As String = " AND Operazioni.lav_cod IN (" & STRINGA_OPERAZIONI_RICETTABILI & ")"

            'Dim FiltroAggiuntivo As String = STR_OP_NON_GESTITE + "AND ( GruppoOperazioni.Tipo IN ('C','Z','P','E')) "

            ' VAnni: 20/4/2018: il parametro filtro al momento non viene incluso nella query perchè viene sempre passato come stringa vuota,
            ' e quindi onde evitare il problema della sql injection segnalata da Leonardo Sistemi lo rimuovo dal filtro aggiuntivo..
            Dim FiltroAggiuntivo As String = CostantiPersonalizzate.STR_OP_NON_GESTITE & " " & Filtro & " AND GruppoOperazioni.Tipo IN (" & Tipo_GruppoOperazioni & ") " & str_filtroUtente & str_filtroRicette
            'Dim FiltroAggiuntivo As String = CostantiPersonalizzate.STR_OP_NON_GESTITE & " AND GruppoOperazioni.Tipo IN (" & Tipo_GruppoOperazioni & ") " & str_filtroUtente

            ' Dim Ordinamento As String = " GruppoOperazioni.Tipo, GruppoOperazioni.GRU_COD, Lav_Des "
            Dim Ordinamento As String = " Operazioni.Lav_Des "

            Dim objOperazioniLeggi As New AgronicaCoreMetaSchemaDAL.Operazioni_R
            Dim DTOperazioni As DataTable = objOperazioniLeggi.Leggi(0, 0, 0, "", 0, "", "", False, False, False, False,
                                                                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                                FiltroAggiuntivo, Ordinamento, objParametri_Server)


            'Prelevo i preferiti
            Dim ui_R As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read()
            Dim dtPref As DataTable = ui_R.Leggi_Utente_Poi_SuperUser(TipiEnumerativi.enum_Impostazioni_Utenti.UTENTE_OPERAZIONI_QDC_PREFERITE, 1, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            Dim lavs As String = ""
            If Not IsNothing(dtPref) AndAlso dtPref.Rows.Count > 0 Then
                lavs = dtPref.Rows(0).Item("Impostazione_Valore_1")
            End If
            Dim lavstr() As String = lavs.Split("|")

            Dim JArrayListaOp As New JArray()
            'JArrayListaOp.Add(New JObject(New JProperty("lav_cod", ""), New JProperty("lav_des", "Nessuna Selezione")))
            For Each dr In DTOperazioni.Rows
                JArrayListaOp.Add(New JObject(New JProperty("lav_cod", dr.Item("LAV_COD")), New JProperty("lav_des", "Brogliaccio di " & dr.Item("LAV_DES")), New JProperty("gru_cod", dr.Item("GRU_COD")), New JProperty("gru_des", dr.Item("GRU_DES")), New JProperty("preferito", If(lavstr.Contains(dr.Item("LAV_COD")), 1, 0))))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(JArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = Gias.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboLavorazioni_PerZoo(objP_server As String, objP_utenti As String, FiltraImpostazioniUtente As Boolean, Tipo_GruppoOperazioni As String) As RispostaStandard
        Return CaricaComboLavorazioni_ConFiltro_PerZoo(objP_server, objP_utenti, FiltraImpostazioniUtente, Tipo_GruppoOperazioni, "")
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboLavorazioni_ConFiltro_PerZoo(objP_server As String, objP_utenti As String, FiltraImpostazioniUtente As Boolean, Tipo_GruppoOperazioni As String, Filtro As String) As RispostaStandard
        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If Tipo_GruppoOperazioni = "" Then
            Tipo_GruppoOperazioni = "'Z'"
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim leggiLingua As New AgronicaCoreUtentiDAL.Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            Dim objUtente As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim dt_FiltroUtente As DataTable = objUtente.LeggiQryParametrica(enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_LAVORAZIONI,
                                                                             1, enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                                             "", "", "", objParametri_Utenti)

            Dim str_filtroUtente As String = ""
            If dt_FiltroUtente.Rows.Count > 0 Then
                Dim f As New List(Of String)
                For Each drF In dt_FiltroUtente.Rows
                    If Not IsDBNull(dt_FiltroUtente.Rows(0).Item("ID_0")) AndAlso dt_FiltroUtente.Rows(0).Item("ID_0").ToString() <> "" Then
                        f.Add(drF.Item("ID_0"))
                    End If
                Next
                str_filtroUtente = " AND Operazioni.Lav_Cod IN (" & String.Join(", ", f) & ") "
            End If

            'Dim STRINGA_OPERAZIONI_RICETTABILI As String = Operazioni_Ricettabili(objParametri_Server)
            'Dim str_filtroRicette As String = " AND Operazioni.lav_cod IN (" & STRINGA_OPERAZIONI_RICETTABILI & ")"
            'Dim FiltroAggiuntivo As String = STR_OP_NON_GESTITE + "AND ( GruppoOperazioni.Tipo IN ('C','Z','P','E')) "

            ' VAnni: 20/4/2018: il parametro filtro al momento non viene incluso nella query perchè viene sempre passato come stringa vuota,
            ' e quindi onde evitare il problema della sql injection segnalata da Leonardo Sistemi lo rimuovo dal filtro aggiuntivo..
            'Dim FiltroAggiuntivo As String = CostantiPersonalizzate.STR_OP_NON_GESTITE & " " & Filtro & " AND GruppoOperazioni.Tipo IN (" & Tipo_GruppoOperazioni & ") " & str_filtroUtente & str_filtroRicette
            'Dim FiltroAggiuntivo As String = CostantiPersonalizzate.STR_OP_NON_GESTITE & " AND GruppoOperazioni.Tipo IN (" & Tipo_GruppoOperazioni & ") " & str_filtroUtente

            Dim FiltroAggiuntivo As String = " Operazioni.lav_cod IN (3000,3001,3002,3003,3004,3020,3023,3030,3033,3034,3035,3036,3037)"
            ' Dim Ordinamento As String = " GruppoOperazioni.Tipo, GruppoOperazioni.GRU_COD, Lav_Des "
            Dim Ordinamento As String = " Operazioni.Lav_Des "

            Dim objOperazioniLeggi As New AgronicaCoreMetaSchemaDAL.Operazioni_R
            Dim DTOperazioni As DataTable = objOperazioniLeggi.Leggi(0, 0, 0, "", 0, "", "", False, False, False, False,
                                                                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                                FiltroAggiuntivo, Ordinamento, objParametri_Server)

            'Prelevo i preferiti
            Dim ui_R As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read()
            Dim dtPref As DataTable = ui_R.Leggi_Utente_Poi_SuperUser(TipiEnumerativi.enum_Impostazioni_Utenti.UTENTE_OPERAZIONI_ZOO_PREFERITE, 1, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            Dim lavs As String = ""
            If Not IsNothing(dtPref) AndAlso dtPref.Rows.Count > 0 Then
                lavs = dtPref.Rows(0).Item("Impostazione_Valore_1")
            End If
            Dim lavstr() As String = lavs.Split("|")

            Dim JArrayListaOp As New JArray()
            'JArrayListaOp.Add(New JObject(New JProperty("lav_cod", ""), New JProperty("lav_des", "Nessuna Selezione")))
            For Each dr In DTOperazioni.Rows
                JArrayListaOp.Add(New JObject(New JProperty("lav_cod", dr.Item("LAV_COD")), New JProperty("lav_des", dr.Item("LAV_DES")), New JProperty("gru_cod", dr.Item("GRU_COD")), New JProperty("gru_des", dr.Item("GRU_DES")), New JProperty("preferito", If(lavstr.Contains(dr.Item("LAV_COD")), 1, 0))))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(JArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = Gias.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboLavorazioni_Model(InData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Metaschema.LeggiOperazioni)) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.attivita.Lavorazione))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.attivita.Lavorazione))

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim leggiLingua As New AgronicaCoreUtentiDAL.Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            Dim Tipo_GruppoOperazioni As String = ""

            If InData.InData.gruppiOperazioni Is Nothing Then
                Tipo_GruppoOperazioni = "'C','E','V','P','Z'"
            Else
                If InData.InData.gruppiOperazioni.Length = 0 Then
                    Tipo_GruppoOperazioni = "'C','E','V','P','Z'"
                Else
                    For Each gruppoOp In InData.InData.gruppiOperazioni
                        Tipo_GruppoOperazioni &= "'" & gruppoOp & "',"
                    Next
                    Tipo_GruppoOperazioni = Tipo_GruppoOperazioni.Substring(0, Tipo_GruppoOperazioni.Length - 1)
                End If
            End If

            Dim objUtente As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim dt_FiltroUtente As DataTable = objUtente.LeggiQryParametrica(TipiEnumerativi.enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_LAVORAZIONI,
                                                           1, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                           "", "", "", objParametri_Utenti)

            Dim str_filtroUtente As String = ""
            If dt_FiltroUtente.Rows.Count > 0 Then
                Dim f As New List(Of String)
                For Each drF In dt_FiltroUtente.Rows
                    If Not IsDBNull(dt_FiltroUtente.Rows(0).Item("ID_0")) AndAlso dt_FiltroUtente.Rows(0).Item("ID_0").ToString() <> "" Then
                        f.Add(drF.Item("ID_0"))
                    End If
                Next
                str_filtroUtente = " AND Operazioni.Lav_Cod IN (" & String.Join(", ", f) & ") "
            End If

            'Dim FiltroAggiuntivo As String = STR_OP_NON_GESTITE + "AND ( GruppoOperazioni.Tipo IN ('C','Z','P','E')) "

            ' VAnni: 20/4/2018: il parametro filtro al momento non viene incluso nella query perchè viene sempre passato come stringa vuota,
            ' e quindi onde evitare il problema della sql injection segnalata da Leonardo Sistemi lo rimuovo dal filtro aggiuntivo..
            Dim FiltroAggiuntivo As String = CostantiPersonalizzate.STR_OP_NON_GESTITE & " AND GruppoOperazioni.Tipo IN (" & Tipo_GruppoOperazioni & ") " & str_filtroUtente
            'Dim FiltroAggiuntivo As String = CostantiPersonalizzate.STR_OP_NON_GESTITE & " AND GruppoOperazioni.Tipo IN (" & Tipo_GruppoOperazioni & ") " & str_filtroUtente

            ' Dim Ordinamento As String = " GruppoOperazioni.Tipo, GruppoOperazioni.GRU_COD, Lav_Des "
            Dim Ordinamento As String = " Operazioni.Lav_Des "

            Dim objOperazioniLeggi As New AgronicaCoreMetaSchemaDAL.Operazioni_R
            Dim DTOperazioni As DataTable = objOperazioniLeggi.Leggi(0, 0, 0, "", 0, "", "", False, False, False, False,
                                                                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                                FiltroAggiuntivo, Ordinamento, objParametri_Server)


            'Prelevo i preferiti
            Dim ui_R As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read()
            Dim dtPref As DataTable = ui_R.Leggi_Utente_Poi_SuperUser(TipiEnumerativi.enum_Impostazioni_Utenti.UTENTE_OPERAZIONI_QDC_PREFERITE, 1, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            Dim lavs As String = ""
            If Not IsNothing(dtPref) AndAlso dtPref.Rows.Count > 0 Then
                lavs = dtPref.Rows(0).Item("Impostazione_Valore_1")
            End If
            Dim lavstr() As String = lavs.Split("|")

            Dim OperazioniList As New List(Of AgronicaCoreModelsSTD.attivita.Lavorazione)
            For Each dr In DTOperazioni.Rows
                Dim lav = New AgronicaCoreModelsSTD.attivita.Lavorazione(dr.Item("LAV_COD"), dr.Item("LAV_DES"))
                OperazioniList.Add(lav)
            Next

            r.RispostaStringa = OperazioniList
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = Gias.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r

    End Function
#End Region

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Operazioni_APP(objP_super_server As String, objP_server As String, objP_utenti As String) As RispostaStandard
        Dim filtro As CaricaComboLavorazioni_ConFiltro = New CaricaComboLavorazioni_ConFiltro
        filtro.FiltraImpostazioniUtente = True
        filtro.Tipo_GruppoOperazioni = "'C','V'"
        filtro.Filtro = ""
        Dim objP = New CoreWS_GenericObjP(objP_super_server, objP_server, objP_utenti)

        Dim request As CoreWS_Generic(Of CaricaComboLavorazioni_ConFiltro) = New CoreWS_Generic(Of CaricaComboLavorazioni_ConFiltro)(objP, filtro)
        Return CaricaComboLavorazioni_ConFiltro_NG(request)
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_GruppoOperazioni_NG(ByVal InData As CoreWS_Generic(Of Leggi_GruppoOperazioni)) As RispostaStandard
        Dim r As New RispostaStandard

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim leggiLingua As New AgronicaCoreUtentiDAL.Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            Dim objGruppoOperazioniLeggi As New AgronicaCoreMetaSchemaDAL.GruppoOperazioni_R
            Dim DTOperazioni As DataTable = objGruppoOperazioniLeggi.Leggi(0, "", 0, "", True, True, True, True,
                                                                                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                                "", "", objParametri_Server)

            If InData.InData.Flag_OpColturali Then
                Dim DTOperazioniFlag As DataTable = objGruppoOperazioniLeggi.Leggi(0, "", 0, "", True, False, False, False,
                                                                                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                                "", "", objParametri_Server)
                DTOperazioni.Merge(DTOperazioniFlag)
            End If

            If InData.InData.Flag_OpZoo Then
                Dim DTOperazioniFlag As DataTable = objGruppoOperazioniLeggi.Leggi(0, "", 0, "", False, True, False, False,
                                                                                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                                "", "", objParametri_Server)
                DTOperazioni.Merge(DTOperazioniFlag)
            End If

            If InData.InData.Flag_OpMacchine Then
                Dim DTOperazioniFlag As DataTable = objGruppoOperazioniLeggi.Leggi(0, "", 0, "", False, False, True, False,
                                                                                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                                "", "", objParametri_Server)
                DTOperazioni.Merge(DTOperazioniFlag)
            End If

            If InData.InData.Flag_OpContabili Then
                Dim DTOperazioniFlag As DataTable = objGruppoOperazioniLeggi.Leggi(0, "", 0, "", False, False, False, True,
                                                                                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                                "", "", objParametri_Server)
                DTOperazioni.Merge(DTOperazioniFlag)
            End If

            Dim JArrayListaOp As New JArray()
            'JArrayListaOp.Add(New JObject(New JProperty("lav_cod", ""), New JProperty("lav_des", "Nessuna Selezione")))
            For Each dr In DTOperazioni.Rows
                Dim tipo As String
                Select Case dr("Tipo")
                    Case "C"
                        tipo = "Colturali"
                    Case "E"
                        tipo = "Altre"
                    Case "Z"
                        tipo = "Zoo"
                    Case "P"
                        tipo = "Macchine"
                    Case "V"
                        tipo = "Audit"
                    Case Else
                        tipo = "Altre"
                End Select

                JArrayListaOp.Add(New JObject(New JProperty("gru_cod", dr.Item("GRU_COD")),
                                                      New JProperty("gru_des", dr.Item("GRU_DES")),
                                                      New JProperty("tipo", tipo)))

            Next

            Dim res = JsonConvert.SerializeObject(JArrayListaOp, Formatting.None)
            r.RispostaStringa = res
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = Gias.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Operazioni_Combinazioni_APP(objP_super_server As String, objP_server As String, objP_utenti As String) As RispostaStandard

        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)


            Dim objOperazione As New Operazioni_Combinazioni_R
            Dim dtOperazioniCombinazioni = objOperazione.Leggi(objParametri_Server)

            Dim listaOperazioniCombinazioni As New JArray()
            For Each dr In dtOperazioniCombinazioni.Rows
                listaOperazioniCombinazioni.Add(
                    New JObject(
                        New JProperty("codice", dr.Item("ID")),
                        New JProperty("lavCod1", dr.Item("Lav_Cod1")),
                        New JProperty("lavCod2", dr.Item("Lav_Cod2")),
                        New JProperty("lavCod3", dr.Item("Lav_Cod3")),
                        New JProperty("lavCod4", dr.Item("Lav_Cod4")),
                        New JProperty("lavCod5", dr.Item("Lav_Cod5"))
                    )
                )
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(listaOperazioniCombinazioni, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = Gias.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_GruppiOperazioni_FiltroImpostazioni(objP_super_server As String, objP_server As String, objP_utenti As String) As RispostaStandard
        Dim r As New RispostaStandard
        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim leggiLingua As New AgronicaCoreUtentiDAL.Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)


            Dim objGruppoOperazioniLeggi As New GruppoOperazioni_R
            Dim xFiltro = " GOper.GRU_COD in (1, 2, 3, 4, 6, 10) "
            Dim gruppi As List(Of baseClass.BaseCodeDescr) = objGruppoOperazioniLeggi.Leggi(
                0, Tipo:="", 0, Cerca_GruDes:="",
                False, False, False, False,
                enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                xFiltro, xOrderBy:="", objParametri_Server
            ).AsEnumerable.
              Select(Function(dr) New baseClass.BaseCodeDescr(dr("gru_cod"), dr("gru_des"))).
              ToList

            r.RispostaStringa = JsonConvert.SerializeObject(gruppi)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = Gias.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_GruppoOperazioni(ByVal objP_server As String,
                                           ByVal Flag_OpColturali As Boolean,
                                           ByVal Flag_OpZoo As Boolean,
                                           ByVal Flag_OpMacchine As Boolean,
                                           ByVal Flag_OpContabili As Boolean) As RispostaStandard
        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim leggiLingua As New Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            Dim objGruppoOperazioniLeggi As New AgronicaCoreMetaSchemaDAL.GruppoOperazioni_R
            Dim DTOperazioni As DataTable = objGruppoOperazioniLeggi.Leggi(0, "", 0, "", True, True, True, True,
                                                                                   AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                                   "", "", objParametri_Server)

            If Flag_OpColturali Then
                Dim DTOperazioniFlag As DataTable = objGruppoOperazioniLeggi.Leggi(0, "", 0, "", True, False, False, False,
                                                                                   AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                                   "", "", objParametri_Server)
                DTOperazioni.Merge(DTOperazioniFlag)
            End If

            If Flag_OpZoo Then
                Dim DTOperazioniFlag As DataTable = objGruppoOperazioniLeggi.Leggi(0, "", 0, "", False, True, False, False,
                                                                                   AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                                   "", "", objParametri_Server)
                DTOperazioni.Merge(DTOperazioniFlag)
            End If

            If Flag_OpMacchine Then
                Dim DTOperazioniFlag As DataTable = objGruppoOperazioniLeggi.Leggi(0, "", 0, "", False, False, True, False,
                                                                                   AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                                   "", "", objParametri_Server)
                DTOperazioni.Merge(DTOperazioniFlag)
            End If

            If Flag_OpContabili Then
                Dim DTOperazioniFlag As DataTable = objGruppoOperazioniLeggi.Leggi(0, "", 0, "", False, False, False, True,
                                                                                   AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                                   "", "", objParametri_Server)
                DTOperazioni.Merge(DTOperazioniFlag)
            End If

            Dim JArrayListaOp As New JArray()
            'JArrayListaOp.Add(New JObject(New JProperty("lav_cod", ""), New JProperty("lav_des", "Nessuna Selezione")))
            For Each dr In DTOperazioni.Rows
                Dim tipo As String
                Select Case dr("Tipo")
                    Case "C"
                        tipo = "Colturali"
                    Case "E"
                        tipo = "Altre"
                    Case "Z"
                        tipo = "Zoo"
                    Case "P"
                        tipo = "Macchine"
                    Case "V"
                        tipo = "Audit"
                    Case Else
                        tipo = "Altre"
                End Select

                JArrayListaOp.Add(New JObject(New JProperty("gru_cod", dr.Item("GRU_COD")),
                                                      New JProperty("gru_des", dr.Item("GRU_DES")),
                                                      New JProperty("tipo", tipo)))
            Next

            Dim res = JsonConvert.SerializeObject(JArrayListaOp, Formatting.None)
            r.RispostaStringa = res
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = Gias.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function SalvaOperazioniPreferite(InData As CoreWS_Generic(Of SalvaOperazioniPreferite)) As RispostaStandard

        Dim r As New RispostaStandard

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim leggiLingua As New AgronicaCoreUtentiDAL.Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            Dim objUtente_R As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim objUtente_W As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_W

            Dim lista_Lav_Cod = InData.InData.lista_Lav_Cod
            If lista_Lav_Cod.Length = 0 Then 'non dovrebbe mai entrare qui..
                Throw New GiasException("Deve essere seleizonata almeno unb'operazione")
            End If

            Dim strPreferiti As String = ""
            Dim READ_operazioniPreferite() As String = Nothing 'Array di lav_cod già nei preferiti, da confrontare con i nuovi elementi salvati
            Dim UPDATE_operazioniPreferite As New List(Of String)  'Array di lav_cod con le nuove Operazioni Preferite, che non sono già presenti nell'array sopra

            Dim _new As Boolean = True 'true IF esiste una riga in impostazioni utenti ELSE false

            Dim dtOperazioniPreferite As DataTable

            If (InData.InData.tipoPreferiti = 1) Then
                dtOperazioniPreferite = objUtente_R.Leggi(enum_Impostazioni_Utenti.UTENTE_OPERAZIONI_ZOO_PREFERITE,
                                                                     1,
                                                                     AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                     "", "",
                                                                     objParametri_Utenti)
            Else
                dtOperazioniPreferite = objUtente_R.Leggi(enum_Impostazioni_Utenti.UTENTE_OPERAZIONI_QDC_PREFERITE,
                                                                     1,
                                                                     AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                     "", "",
                                                                     objParametri_Utenti)
            End If


            If dtOperazioniPreferite.Rows.Count > 0 Then
                _new = False
                strPreferiti = dtOperazioniPreferite.Rows(0).Item("Impostazione_Valore_1")
                READ_operazioniPreferite = strPreferiti.Split("|")
            End If

            Select Case _new
                Case True 'se non ho nessuna riga nelle impostazioni utenti, scrivo il record
                    strPreferiti = String.Join("|", lista_Lav_Cod)
                    If (InData.InData.tipoPreferiti = 1) Then
                        objUtente_W.Scrivi(enum_Impostazioni_Utenti.UTENTE_OPERAZIONI_ZOO_PREFERITE, strPreferiti,
                                            "", "", "",
                                            AGRODATAINIZIO, AGRODATAFINE,
                                            objParametri_Utenti)
                    Else
                        objUtente_W.Scrivi(enum_Impostazioni_Utenti.UTENTE_OPERAZIONI_QDC_PREFERITE, strPreferiti,
                                           "", "", "",
                                           AGRODATAINIZIO, AGRODATAFINE,
                                           objParametri_Utenti)
                    End If
                Case False 'se ho già una riga nelle impostazioni utenti, confronto i due array (operazioni preferite già esistenti con le nuove)
                    For Each operazione In lista_Lav_Cod
                        'se le operazioni salvate da interfaccia NON sono già tra i preferiti, le aggiungo all'array UPDATE
                        If Not READ_operazioniPreferite.Contains(operazione) Then
                            UPDATE_operazioniPreferite.Add(operazione)
                        End If
                    Next

                    'se sto eseguendo un aggiornaento completo vado a sostituire completamente le operazioni preferite
                    If InData.InData.full_update Then
                        strPreferiti = ""
                        UPDATE_operazioniPreferite = lista_Lav_Cod.Select(Of String)(Function(n) n.ToString()).ToList()
                    End If

                    'Concateno i preferiti già esistenti con quelli appena aggiunti SE effettivmanete ci sono nuovi elementi
                    If UPDATE_operazioniPreferite.Count > 0 Then
                        If strPreferiti <> "" Then
                            strPreferiti += "|"
                        End If
                        strPreferiti = strPreferiti + String.Join("|", UPDATE_operazioniPreferite)
                        If (InData.InData.tipoPreferiti = 1) Then
                            objUtente_W.Modifica(enum_Impostazioni_Utenti.UTENTE_OPERAZIONI_ZOO_PREFERITE, strPreferiti,
                                                 "", "", "",
                                                 AGRODATAINIZIO, AGRODATAFINE,
                                                 objParametri_Utenti)
                        Else
                            objUtente_W.Modifica(enum_Impostazioni_Utenti.UTENTE_OPERAZIONI_QDC_PREFERITE, strPreferiti,
                                                 "", "", "",
                                                 AGRODATAINIZIO, AGRODATAFINE,
                                                 objParametri_Utenti)
                        End If

                    End If

            End Select


            r.RispostaStringa = strPreferiti
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = Gias.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Operazioni_Modello(InData As CoreWS_Generic(Of LeggiOperazioni)) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.attivita.Lavorazione))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.attivita.Lavorazione))

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Dim Visualizza_Solo_Operazioni_Preferite As Boolean = InData.InData.Visualizza_Solo_Operazioni_Preferite

        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim leggiLingua As New AgronicaCoreUtentiDAL.Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            Dim listOperazioni As List(Of AgronicaCoreModelsSTD.attivita.Lavorazione)

            Dim Flag_Agenda As Boolean = False

            Dim Flag_Ricetta As Boolean = False

            Dim Flag_Brogliaccio As Boolean = False

            Select Case InData.InData.tipo_Attivita
                Case AgronicaCoreModelsSTD.attivita.Attivita.Tipo_Attivita.QuadernoDiCampagna
                    Flag_Agenda = True
                Case AgronicaCoreModelsSTD.attivita.Attivita.Tipo_Attivita.Ricetta
                    If InData.InData.tipo_Ricetta = AgronicaCoreModelsSTD.attivita.Attivita.Tipo_Ricetta.Standard_Destinazioni OrElse
                        InData.InData.tipo_Ricetta = AgronicaCoreModelsSTD.attivita.Attivita.Tipo_Ricetta.PianoDistribuzionePua Then

                        If InData.InData.stato = AgronicaCoreModelsSTD.attivita.Attivita.Stati.Da_Eseguire Then
                            Flag_Ricetta = True
                        ElseIf InData.InData.stato = AgronicaCoreModelsSTD.attivita.Attivita.Stati.Eseguita Then
                            Flag_Brogliaccio = True
                        End If

                    End If
            End Select


            'IsNothing(lista_Lav_Cod) se siamo entrati nella pagina e non abbiamo selezionato un'operazione, eseguo la lettura per caricarle tutte
            ' altrimenti se esiste almeno un elemento nella lista, cerco tutte le combinazioni possibili 
            Dim lista_Lav_Cod = InData.InData.lista_Lav_Cod
            If IsNothing(lista_Lav_Cod) OrElse lista_Lav_Cod.Length = 0 Then

                Dim Tipo_GruppoOperazioni As String = String.Empty

                If Flag_Agenda Then
                    Tipo_GruppoOperazioni = "'C'"
                ElseIf Flag_Ricetta OrElse Flag_Brogliaccio Then
                    Tipo_GruppoOperazioni = "'C','E','V','P'"
                End If

                Dim objCom As New AgronicaCoreMetaSchemaBIZ.Operazioni
                listOperazioni = objCom.Leggi(InData.InData.FiltraImpostazioniUtente, Tipo_GruppoOperazioni, objParametri_Server, objParametri_Utenti)

                If Flag_Ricetta OrElse Flag_Brogliaccio Then
                    filtraOperazioniRicettabili(listOperazioni, objParametri_Server)
                End If

                If Visualizza_Solo_Operazioni_Preferite Then
                    filtraOperazioniPreferite(listOperazioni, objParametri_Utenti)
                End If

                filtraOperazioniGestite(listOperazioni)
            Else


                'Se il filtro utente ritorna un risultato, mi costruisco una stringa di LAV_COD da passare come filtro 
                Dim str_filtro_Lav_Cod As String = ""

                If InData.InData.FiltraImpostazioniUtente Then
                    'Utilizzo il filtro utente per leggere solo le combinazioni possibili con operazioni che l'utente vuole visualizzare 
                    Dim objUtente As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
                    Dim dt_FiltroImpostazioniUtente As DataTable = objUtente.LeggiQryParametrica(enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_LAVORAZIONI,
                                                                                                 1, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                                                                 "", "", "", objParametri_Utenti)

                    If dt_FiltroImpostazioniUtente.Rows.Count > 0 Then
                        Dim lista_Filtri_Lav_Cod As New List(Of String)
                        For Each drF In dt_FiltroImpostazioniUtente.Rows
                            If Not IsDBNull(dt_FiltroImpostazioniUtente.Rows(0).Item("ID_0")) AndAlso dt_FiltroImpostazioniUtente.Rows(0).Item("ID_0").ToString() <> "" Then
                                lista_Filtri_Lav_Cod.Add(drF.Item("ID_0"))
                            End If
                        Next
                        'Aggiungo anche lo 0, devo contare anche i casi in cui in cui le combinazioni sono composte da solo 2 lav_cod
                        str_filtro_Lav_Cod = "0, " & String.Join(", ", lista_Filtri_Lav_Cod)
                    End If
                End If


                Dim objOperazione As New Operazioni_Combinazioni_R
                listOperazioni = objOperazione.Leggi(lista_Lav_Cod, str_filtro_Lav_Cod, "", objParametri_Server)

                If Flag_Ricetta OrElse Flag_Brogliaccio Then
                    filtraOperazioniRicettabili(listOperazioni, objParametri_Server)
                End If

                If Visualizza_Solo_Operazioni_Preferite Then
                    filtraOperazioniPreferite(listOperazioni, objParametri_Utenti)
                End If

                'filtraOperazioniconAttivitaCompatibili(lista_Lav_Cod, listOperazioni, objParametri_Server)

                filtraOperazioniGestite(listOperazioni)

            End If

            r.RispostaStringa = listOperazioni
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = Gias.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r

    End Function


#Region "Utility Operazioni"

    Private Shared Sub filtraOperazioniRicettabili(ByRef listaOperazioni As List(Of AgronicaCoreModelsSTD.attivita.Lavorazione),
                                                 objParametri_Server As AgronicaCoreParametri)

        If Not IsNothing(listaOperazioni) AndAlso listaOperazioni.Count > 0 Then

            Dim dummyRicettabili As New List(Of AgronicaCoreModelsSTD.attivita.Lavorazione)

            Dim objConfig_Siti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

            Dim operazioniRicettabili As List(Of String) = objConfig_Siti.Ottiene_Lav_Cod_Ricettabili(objParametri_Server, isFromNG:=True)

            If Not IsNothing(operazioniRicettabili) AndAlso operazioniRicettabili.Count > 0 Then
                For Each operazione In listaOperazioni
                    For Each ricettabile In operazioniRicettabili
                        If operazione.primaryKey.codice = ricettabile AndAlso dummyRicettabili.Where(Function(d) d.primaryKey.codice = ricettabile).Count = 0 Then
                            dummyRicettabili.Add(operazione)
                        End If
                    Next
                Next
            End If

            listaOperazioni = dummyRicettabili

        End If

    End Sub

    Private Shared Sub filtraOperazioniPreferite(ByRef listaOperazioni As List(Of AgronicaCoreModelsSTD.attivita.Lavorazione),
                                                 objParametri_Utenti As AgronicaCoreParametri)

        Dim objUtente As New Utenti_Impostazioni_Read
        Dim lav_cod As String = objUtente.LeggiValoreImpostazioneScalare(enum_Impostazioni_Utenti.UTENTE_OPERAZIONI_QDC_PREFERITE, objParametri_Utenti.UtenteUsername, objParametri_Utenti)

        Dim dummyPreferiti As New List(Of AgronicaCoreModelsSTD.attivita.Lavorazione)
        If Not String.IsNullOrEmpty(lav_cod) Then
            Dim operazioniPreferite() As String = lav_cod.Split("|")

            For Each operazione In listaOperazioni
                For Each preferito In operazioniPreferite
                    'Non mostro le operazioni preferite che sono state salvate due volte
                    If operazione.primaryKey.codice = preferito AndAlso dummyPreferiti.Where(Function(d) d.primaryKey.codice = preferito).Count = 0 Then
                        dummyPreferiti.Add(operazione)
                    End If
                Next
            Next
        End If

        listaOperazioni = dummyPreferiti

    End Sub

    Private Shared Sub filtraOperazioniGestite(ByRef listaOperazioni As List(Of AgronicaCoreModelsSTD.attivita.Lavorazione))

        If Not IsNothing(listaOperazioni) AndAlso listaOperazioni.Count > 0 Then

            Dim dummyOpGestite As New List(Of AgronicaCoreModelsSTD.attivita.Lavorazione)

            Dim List_Op_Non_Gestite = CostantiPersonalizzate.STR_OP_NON_GESTITE_NG.Split(",").ToList()

            If Not IsNothing(List_Op_Non_Gestite) AndAlso List_Op_Non_Gestite.Count > 0 Then
                For Each operazione In listaOperazioni
                    If Not List_Op_Non_Gestite.Contains(operazione.primaryKey.codice.ToString()) AndAlso dummyOpGestite.Where(Function(d) d.primaryKey.codice = operazione.primaryKey.codice).Count = 0 Then
                        dummyOpGestite.Add(operazione)
                    End If
                Next
            End If

            listaOperazioni = dummyOpGestite

        End If

    End Sub

    Private Shared Sub filtraOperazioniconAttivitaCompatibili(ByVal lista_lav_Cod_scelti As Integer(),
                                                                ByRef listaOperazioni As List(Of AgronicaCoreModelsSTD.attivita.Lavorazione),
                                                                objParametri_Server As AgronicaCoreParametri)

        'Blocco le operazioni che hanno delle attivita annuali/poliennali diverse
        'TODO Chiedere se va bene

        Dim objAttivitaXOperazioni As New AgronicaCoreContabDAL.AttivitaXOperazioni_R

        Dim dt_AttivitaXOperazioni As DataTable = objAttivitaXOperazioni.Leggi(0, 0, "", "", objParametri_Server, xSelezioneVariabile:=enumSelezioneVariabile.Selezione_TabellaDatiMinimi)

        Dim lav_cod_da_escludere As New List(Of Integer)

        If Not IsNothing(dt_AttivitaXOperazioni) AndAlso dt_AttivitaXOperazioni.Rows.Count > 0 Then

            Dim dt_AttivitaXOperazioni_Operazioni_Scelte = dt_AttivitaXOperazioni.Select.Where(Function(r) lista_lav_Cod_scelti.Contains(r("Lav_Cod"))).CopyToDataTable()

            If Not IsNothing(dt_AttivitaXOperazioni_Operazioni_Scelte) AndAlso dt_AttivitaXOperazioni_Operazioni_Scelte.Rows.Count > 0 Then

                Dim dt_AttivitaXOperazioni_Operazioni_Compatibili = dt_AttivitaXOperazioni.Select.Where(Function(r) Not lista_lav_Cod_scelti.Contains(r("Lav_Cod"))).CopyToDataTable()

                If Not IsNothing(dt_AttivitaXOperazioni_Operazioni_Compatibili) AndAlso dt_AttivitaXOperazioni_Operazioni_Compatibili.Rows.Count > 0 Then

                    For Each r_Op_Scelte In dt_AttivitaXOperazioni_Operazioni_Scelte.Rows
                        Dim Dt = dt_AttivitaXOperazioni_Operazioni_Compatibili.Select.Where(Function(f) CInt(f("Attivita_Poliannuale")) <> CInt(r_Op_Scelte("Attivita_Poliannuale"))).CopyToDataTable()

                        If Not IsNothing(Dt) AndAlso Dt.Rows.Count > 0 Then
                            For Each r In Dt.Rows
                                If Not lav_cod_da_escludere.Contains(CInt(r("Lav_Cod"))) Then
                                    lav_cod_da_escludere.Add(r("Lav_Cod"))
                                End If
                            Next
                        End If
                    Next

                End If

            End If
        End If

        If lav_cod_da_escludere.Count > 0 Then
            listaOperazioni = listaOperazioni.Where(Function(o) Not lav_cod_da_escludere.Contains(CInt(o.primaryKey.codice))).ToList()
        End If

    End Sub

    <Obsolete("NON USATA")>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function changeVisualizzaOperazioni(InData As CoreWS_Generic(Of LeggiOperazioni)) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.attivita.Lavorazione))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.attivita.Lavorazione))

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Dim Visualizza_Solo_Operazioni_Preferite As Boolean = InData.InData.Visualizza_Solo_Operazioni_Preferite

        Try


            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)
            Dim cache As Cache
            If HttpContext.Current.Cache IsNot Nothing Then
                cache = HttpContext.Current.Cache
            End If

            Dim key = "OperazioniModello"
            Dim listOperazioni As New List(Of AgronicaCoreModelsSTD.attivita.Lavorazione)
            Dim operazioni As New List(Of AgronicaCoreModelsSTD.attivita.Lavorazione)

            Dim objCom As New AgronicaCoreMetaSchemaBIZ.Operazioni
            listOperazioni = objCom.Leggi(True, "'C'", objParametri_Server, objParametri_Utenti)

            Dim FiltroAggiuntivo As String = ""

            If Visualizza_Solo_Operazioni_Preferite Then
                'Utilizzo il filtro utente per leggere solo le operazioni che l'utente vuole visualizzare 
                Dim objUtente As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
                Dim dt_FiltroImpostazioniUtente As DataTable = objUtente.Leggi_Utente_Poi_SuperUser(enum_Impostazioni_Utenti.UTENTE_OPERAZIONI_QDC_PREFERITE,
                                                                                                    1, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                                    "", "",
                                                                                                    objParametri_Utenti)

                If dt_FiltroImpostazioniUtente.Rows.Count > 0 Then
                    Dim lavs As String = dt_FiltroImpostazioniUtente.Rows(0).Item("Impostazione_Valore_1")
                    Dim lavstr() As String = lavs.Split("|")

                    FiltroAggiuntivo = " Operazioni.Lav_Cod IN (" & Agro_SQL_Save_Clausola_IN(String.Join(",", lavstr), False) & ") "
                End If



                Dim objOperazioniLeggi As New AgronicaCoreMetaSchemaDAL.Operazioni_R
                Dim DTOperazioni As DataTable = objOperazioniLeggi.Leggi(0, 0, 0, "",
                                                                         0, "", "",
                                                                         False, False,
                                                                         False, False,
                                                                         enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                                         FiltroAggiuntivo, "",
                                                                         objParametri_Server)

                listOperazioni = (From row As DataRow In DTOperazioni.Rows
                                  Select New AgronicaCoreModelsSTD.attivita.Lavorazione(row("LAV_COD"), row("LAV_DES"))).ToList

            End If


            r.RispostaStringa = listOperazioni
            r.RispostaOK = True


        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = Gias.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r

    End Function
#End Region

End Class