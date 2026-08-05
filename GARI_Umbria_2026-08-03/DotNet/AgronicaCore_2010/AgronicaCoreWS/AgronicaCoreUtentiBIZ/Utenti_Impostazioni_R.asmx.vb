Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreUtility
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class Utenti_Impostazioni_R
    Inherits System.Web.Services.WebService


    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function AnnataAgraria(ByVal DataRiferimento As String, ByVal objP_Utenti As String) As rispostaStandard(Of AnnataAgraria)
        Dim r As New rispostaStandard(Of AnnataAgraria)

        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_Utenti)

        Try

            'Inserire il codice QUI..

            Dim d1 As DateTime
            Dim d2 As DateTime

            Dim lAnn As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            lAnn.AnnataAgraria(DataRiferimento, d1, d2, objParametri_Utenti)

            r.RispostaOK = True
            r.RispostaStringa = New AnnataAgraria(d1, d2)


        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Get_AnnataAgraria(objP_utenti As String) As RispostaStandard

        Dim r As New RispostaStandard()

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

        Try

            'Leggo il dato
            Dim ui_R As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim dt As DataTable = ui_R.Leggi(enum_Impostazioni_Utenti.UTENTE_InizioFineAnnataAgraria, 1, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)

            If dt.Rows.Count = 0 Then
                r.RispostaStringa = "01013112"
            Else
                r.RispostaStringa = dt.Rows(0).Item("Impostazione_Valore_1")
            End If

            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Get_ImpostazioniDisciplinari(objP_server As String, objP_utenti As String, piva As String) As RispostaStandard

        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If piva = "" Then
            r.Errore = "piva non valorizzato"
            Return r
        End If

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

        Try

            Dim rd As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
            Dim val As String = rd.Leggi_Codice_from_Imprese_Codici(piva, enum_CodiciAnagrafe.Disciplinare_Aziendale_Default, objParametri_Server)

            If Not val.Contains("e:") Then
                'vecchia modalità
                Dim xLeggiConfigurazioneSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

                Dim GiasOnline_WS_Disciplinari_AgroWS_Disciplinari As String = xLeggiConfigurazioneSiti.Leggi_Valore(0, "GiasOnline_WS_Disciplinari_AgroWS_Disciplinari", "", "", objParametri_Server)
                HttpContext.Current.Session("WS_DPI") = GiasOnline_WS_Disciplinari_AgroWS_Disciplinari

                Dim GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci As String = xLeggiConfigurazioneSiti.Leggi_Valore(0, "GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci", "", "", objParametri_Server)
                HttpContext.Current.Session("WS_FITO") = GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci


                Session("ASG_SuperUser_CodFiscale") = objParametri_Server.PivaSuperUser
                Session("ASG_Utente_Username_Crypt") = Sicurezza.Stringa_Codifica_LANCompatibile(objParametri_Server.SuperUserUsername, CostantiPersonalizzate.AgroKey_EncoderDecoder)

                Dim xletturautente As New AgronicaCoreUtentiDAL.Utenti_Read
                Dim pass As String = xletturautente.Password_From_UserName(objParametri_Server.SuperUserUsername, objParametri_Utenti)
                Session("ASG_Utente_Password_Crypt") = Sicurezza.Stringa_Codifica_LANCompatibile(pass, CostantiPersonalizzate.AgroKey_EncoderDecoder)

                Dim x As New AgronicaCoreDpiBIZ.CaricaListControl
                Dim disciplinare As String = val
                If disciplinare.Split("/").Length > 1 Then
                    Dim ente_cod As String = ""
                    Dim fp As String = ""

                    x.Trova_Ente_Disciplinare(disciplinare, False, "", "",
                                                       Session, objParametri_Server, objParametri_Utenti,
                                                       0, 0, 0, 0, True, True, False,
                                                       New AgronicaCoreGestioneRichieste.AgroWebConfig() With {.Flag_DisciplinarePrivato = True, .Flag_DisciplinareAttivo = True},
                                                       ente_cod,
                                                       fp)

                    val = "e:" & ente_cod & "/" & "fp:" & fp

                End If

            End If

            r.RispostaOK = True
            r.RispostaStringa = val

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Get_OperazioniPreferite(objP_server As String, objP_utenti As String) As RispostaStandard

        Return Get_OperazioniPreferite_PerGruppoOperazioni(objP_server, objP_utenti, "")

    End Function


    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Get_OperazioniPreferite_PerGruppoOperazioni(objP_server As String, objP_utenti As String, gruppoOperazioni As String) As RispostaStandard

        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_Server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_Utenti non valorizzato"
            Return r
        End If

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

        Try

            Dim leggiLingua As New AgronicaCoreUtentiDAL.Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            Dim ui_R As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read()
            Dim dtPref As DataTable = ui_R.Leggi_Utente_Poi_SuperUser(enum_Impostazioni_Utenti.UTENTE_OPERAZIONI_QDC_PREFERITE, 1, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)

            Dim jArrayLista As New JArray()

            If dtPref.Rows.Count > 0 AndAlso Not IsDBNull(dtPref.Rows(0).Item("Impostazione_Valore_1")) AndAlso dtPref.Rows(0).Item("Impostazione_Valore_1") <> "" Then

                Dim xfiltroAggiuntivo As String = STR_OP_NON_GESTITE_BS

                Dim dtOperaz As DataTable = New AgronicaCoreMetaSchemaDAL.Operazioni_R().Leggi(0, 0, 0, "", 0, "", "", False, False, False, False,
                                                                                            AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                                                            xfiltroAggiuntivo, "", objParametri_Server)
                Dim lavs As String = dtPref.Rows(0).Item("Impostazione_Valore_1")
                Dim lavstr() As String = lavs.Split("|")

                For Each lavcod As String In lavstr
                    If IsNumeric(lavcod) Then
                        Dim tipo As String = (From riga As DataRow In dtOperaz.Rows Where riga.Item("lav_cod") = lavcod Select riga.Item("tipo")).FirstOrDefault
                        Dim des As String = (From riga As DataRow In dtOperaz.Rows Where riga.Item("lav_cod") = lavcod Select riga.Item("lav_des")).FirstOrDefault

                        If Not String.IsNullOrEmpty(tipo) AndAlso Not String.IsNullOrEmpty(des) Then
                            If gruppoOperazioni = "" OrElse gruppoOperazioni = tipo Then
                                jArrayLista.Add(New JObject(New JProperty("lav_cod", CInt(lavcod)), New JProperty("lav_des", des)))
                            End If
                        End If
                    End If
                Next

            End If

            r.RispostaOK = True
            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            r.RispostaStringa = JsonConvert.SerializeObject(jArrayLista, Formatting.None)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Get_OperazioniPreferite_PerRicette(objP_server As String, objP_utenti As String) As RispostaStandard

        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_Server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_Utenti non valorizzato"
            Return r
        End If

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

        Try

            Dim leggiLingua As New AgronicaCoreUtentiDAL.Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            Dim ui_R As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read()
            Dim dtPref As DataTable = ui_R.Leggi_Utente_Poi_SuperUser(enum_Impostazioni_Utenti.UTENTE_OPERAZIONI_QDC_PREFERITE, 1, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)

            Dim jArrayLista As New JArray()

            If dtPref.Rows.Count > 0 AndAlso Not IsDBNull(dtPref.Rows(0).Item("Impostazione_Valore_1")) AndAlso dtPref.Rows(0).Item("Impostazione_Valore_1") <> "" Then

                Dim Filtro As String = " operazioni.lav_cod IN (" & CostantiPersonalizzate.STR_OP_RICETTABILI & ")"

                Dim dtOperaz As DataTable = New AgronicaCoreMetaSchemaDAL.Operazioni_R().Leggi(0, 0, 0, "", 0, "", "", False, False, False, False,
                                                                                            AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                                                            Filtro, "", objParametri_Server)
                Dim listaOperazTutte As New Hashtable
                For Each dr As DataRow In dtOperaz.Rows
                    listaOperazTutte.Add(dr("LAV_COD"), dr("LAV_DES"))
                Next

                Dim lavs As String = dtPref.Rows(0).Item("Impostazione_Valore_1")
                Dim lavstr() As String = lavs.Split("|")

                For Each lavcod As String In lavstr
                    If IsNumeric(lavcod) AndAlso listaOperazTutte.Contains(CInt(lavcod)) Then
                        jArrayLista.Add(New JObject(New JProperty("lav_cod", CInt(lavcod)), New JProperty("lav_des", "Ricetta di " & listaOperazTutte(CInt(lavcod)))))
                    End If
                Next

            End If

            r.RispostaOK = True
            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            r.RispostaStringa = JsonConvert.SerializeObject(jArrayLista, Formatting.None)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Get_OperazioniPreferite_PerBrogliaccio(objP_server As String, objP_utenti As String) As RispostaStandard

        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_Server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_Utenti non valorizzato"
            Return r
        End If

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

        Try

            Dim leggiLingua As New AgronicaCoreUtentiDAL.Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            Dim ui_R As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read()
            Dim dtPref As DataTable = ui_R.Leggi_Utente_Poi_SuperUser(enum_Impostazioni_Utenti.UTENTE_OPERAZIONI_QDC_PREFERITE, 1, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)

            Dim jArrayLista As New JArray()

            If dtPref.Rows.Count > 0 AndAlso Not IsDBNull(dtPref.Rows(0).Item("Impostazione_Valore_1")) AndAlso dtPref.Rows(0).Item("Impostazione_Valore_1") <> "" Then

                Dim Filtro As String = " operazioni.lav_cod IN (" & CostantiPersonalizzate.STR_OP_RICETTABILI & ")"
                'LAVCOD_SEMINA & "," & LAVCOD_SOVESCIO & "," & LAVCOD_TRAPIANTO & "," & LAVCOD_RILIEVO_AVVERSITA_CAMPO & ")"

                Dim dtOperaz As DataTable = New AgronicaCoreMetaSchemaDAL.Operazioni_R().Leggi(0, 0, 0, "", 0, "", "", False, False, False, False,
                                                                                            AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                                                            Filtro, "", objParametri_Server)
                Dim listaOperazTutte As New Hashtable
                For Each dr As DataRow In dtOperaz.Rows
                    listaOperazTutte.Add(dr("LAV_COD"), dr("LAV_DES"))
                Next

                Dim lavs As String = dtPref.Rows(0).Item("Impostazione_Valore_1")
                Dim lavstr() As String = lavs.Split("|")

                For Each lavcod As String In lavstr
                    If IsNumeric(lavcod) AndAlso listaOperazTutte.Contains(CInt(lavcod)) Then
                        jArrayLista.Add(New JObject(New JProperty("lav_cod", CInt(lavcod)), New JProperty("lav_des", "Brogliaccio di " & listaOperazTutte(CInt(lavcod)))))
                    End If
                Next

            End If

            r.RispostaOK = True
            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            r.RispostaStringa = JsonConvert.SerializeObject(jArrayLista, Formatting.None)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Get_OperazioniPreferite_PerZoo(objP_server As String, objP_utenti As String) As RispostaStandard
        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_Server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_Utenti non valorizzato"
            Return r
        End If

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

        Try
            Dim leggiLingua As New AgronicaCoreUtentiDAL.Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            Dim ui_R As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read()
            Dim dtPref As DataTable = ui_R.Leggi_Utente_Poi_SuperUser(enum_Impostazioni_Utenti.UTENTE_OPERAZIONI_ZOO_PREFERITE, 1, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)

            Dim jArrayLista As New JArray()

            If dtPref.Rows.Count > 0 AndAlso Not IsDBNull(dtPref.Rows(0).Item("Impostazione_Valore_1")) AndAlso dtPref.Rows(0).Item("Impostazione_Valore_1") <> "" Then
                Dim Filtro As String = " operazioni.lav_cod IN (3000,3001,3002,3003,3004,3020,3023,3030,3033,3034,3035,3036,3037)"
                'LAVCOD_NASCITA_ANIMALI & "," & LAVCOD_INCREMENTO_CONSISTENZE_ZOO & "," & LAVCOD_DECREMENTO_CONSISTENZE_ZOO & "," & LAVCOD_MORTE_ANIMALI & "," & LAVCOD_MACELLAZIONE_ANIMALI & "," & LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MANUALE_FORAGGI & "," & LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MECCANICA_MANGIMI & "," & LAVCOD_SPOSTAMENTI_ZOO & "," & LAVCOD_PESATURA_ANIMALI & "," & LAVCOD_ACQUISTO_ANIMALI & "," & LAVCOD_VENDITA_ANIMALI & "," & LAVCOD_ALTRE_LAVORAZIONI_ZOO & " & "," & LAVCOD_TRASFERIMENTO_ANIMALI & ")"

                Dim dtOperaz As DataTable = New AgronicaCoreMetaSchemaDAL.Operazioni_R().Leggi(0, 0, 0, "", 0, "", "",
                                                                                               False, False, False, False,
                                                                                               AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                                                               Filtro, "", objParametri_Server)
                Dim listaOperazTutte As New Hashtable
                For Each dr As DataRow In dtOperaz.Rows
                    listaOperazTutte.Add(dr("LAV_COD"), dr("LAV_DES"))
                Next

                Dim lavs As String = dtPref.Rows(0).Item("Impostazione_Valore_1")
                Dim lavstr() As String = lavs.Split("|")

                For Each lavcod As String In lavstr
                    If IsNumeric(lavcod) AndAlso listaOperazTutte.Contains(CInt(lavcod)) Then
                        jArrayLista.Add(New JObject(New JProperty("lav_cod", CInt(lavcod)), New JProperty("lav_des", listaOperazTutte(CInt(lavcod)))))
                    End If
                Next
            End If

            r.RispostaOK = True
            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            r.RispostaStringa = JsonConvert.SerializeObject(jArrayLista, Formatting.None)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Get_Impostazioni(objP_Utenti As String, impostazione_cod As Integer) As RispostaStandard

        Return Get_ImpostazioniByUsername_1Utente_o_2SuperUser(objP_Utenti, impostazione_cod, 1)

    End Function


    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Get_ImpostazioniByUsername_1Utente_o_2SuperUser(objP_Utenti As String, impostazione_cod As Integer, Username_1Utente_o_2SuperUser As Integer) As RispostaStandard

        Dim r As New RispostaStandard()

        If objP_Utenti = "" Then
            r.Errore = "objP_Utenti non valorizzato"
            Return r
        End If

        If Not IsNumeric(impostazione_cod) OrElse impostazione_cod = 0 Then
            r.Errore = "impostazione_cod non valorizzato"
            Return r
        End If

        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_Utenti)

        Try

            Dim ui_R As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read()
            Dim dt As DataTable = ui_R.Leggi(impostazione_cod, Username_1Utente_o_2SuperUser, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)

            Dim val As String = "notSet"
            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                val = dt.Rows(0).Item("Impostazione_Valore_1")
            End If

            r.RispostaOK = True
            r.RispostaStringa = val

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Get_Impostazioni_APP(ByVal objP_server As String, ByVal objP_utenti As String) As RispostaStandard

        Dim r As New RispostaStandard()

        Dim dtImpostazioni As New DataTable
        dtImpostazioni.Columns.Add(New DataColumn("Impostazione_Cod", GetType(Integer)))
        dtImpostazioni.Columns.Add(New DataColumn("Impostazione_Valore_1", GetType(String)))

        If objP_utenti = "" Then
            r.Errore = "objP_Utenti non valorizzato"
            Return r
        End If

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

        Try

            Dim listaFiltroAggiuntivo As New List(Of String)

            ' leggo impostazioni superuser

            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_NUOVA_RICETTA)
            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_NUOVO_INTERVENTO)
            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_INTERVENTI_DA_FARE)
            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_SCARICO_ORE)
            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.SUPERUSER_UTILIZZO_ORARI_INIZIO_FINE)
            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_MAX_AZIENDE)
            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_GIACENZE_APP)
            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_ENTRATAUSCITA)
            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_LAMIAPOSIZIONE)
            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_VISITE)
            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_FREQUENZARILIEVO_MINUTI)
            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_FREQUENZASINCRO_MINUTI)
            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_LOTTI)
            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_IMPOSTAZIONI)
            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_DOCUMENTI)
            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_RILIEVI)
            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_GIS)
            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_InCab)

            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.SUPERUSER_COD_PERSON_RILIEVO_FASI_FENOLOGICHE)
            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.SUPERUSER_COD_PERSON_RILIEVO_AVVERSITA)
            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.SUPERUSER_COD_PERSON_RILIEVO_ERBE_INFESTANTI)
            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.SUPERUSER_COD_PERSON_RILIEVO_INDICI_MATURITA)
            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.SUPERUSER_COD_PERSON_RILIEVO_INDICI_RESE_RACCOLTA)
            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.SUPERUSER_COD_PERSON_RILIEVO_DANNI_ALLA_RACCOLTA)
            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.SUPERUSER_GESTIONE_MAGAZZINO_ABILITATA)
            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.SUPERUSER_COD_ALGORITMO_COSTI_ACCESSORI)
            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.Raccolta_Con_Carico_Magazzino)
            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.SUPERUSER_NR_ORE_VISITA)
            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.SUPERUSER_ZOO_IN_VISITA)
            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.SUPERUSER_STATO_VISITA)
            listaFiltroAggiuntivo.Add(enum_Impostazioni_Utenti.SUPERUSER_ModalitaDemetra)

            Dim stbFiltro As New StringBuilder

            stbFiltro.Append(" Impostazione_Cod IN (")
            stbFiltro.Append(String.Join(",", listaFiltroAggiuntivo))
            stbFiltro.Append(")")

            Dim xFiltroAggiuntivo = stbFiltro.ToString

            Dim ui_R As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read()
            Dim dt As DataTable = ui_R.Leggi(0, 2, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, xFiltroAggiuntivo, "", objParametri_Utenti)

            For Each dr As DataRow In dt.Rows
                Dim drImpo = dtImpostazioni.NewRow
                drImpo.Item("Impostazione_Cod") = dr.Item("Impostazione_Cod")
                drImpo.Item("Impostazione_Valore_1") = dr.Item("Impostazione_Valore_1")
                dtImpostazioni.Rows.Add(drImpo)
            Next

            ' leggo impostazioni utente
            Dim listaImpostazioniUtente As New List(Of String)
            listaImpostazioniUtente.Add(enum_Impostazioni_Utenti.UTENTE_COD_RACCOLTA_TIPO)
            listaImpostazioniUtente.Add(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_SE_SENZA_MAGAZZINO)
            listaImpostazioniUtente.Add(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_SE_SUPERA_GIACENZE)
            listaImpostazioniUtente.Add(enum_Impostazioni_Utenti.UTENTE_COD_AZIENDA_PREDEFINITA_ALL_AVVIO)
            xFiltroAggiuntivo = " Impostazione_Cod IN (" + String.Join(",", listaImpostazioniUtente) + ")"

            Dim dt2 As DataTable = ui_R.Leggi(0, 1, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, xFiltroAggiuntivo, "", objParametri_Utenti)

            For Each dr As DataRow In dt2.Rows
                Dim drImpo = dtImpostazioni.NewRow
                drImpo.Item("Impostazione_Cod") = dr.Item("Impostazione_Cod")
                drImpo.Item("Impostazione_Valore_1") = dr.Item("Impostazione_Valore_1")
                dtImpostazioni.Rows.Add(drImpo)
            Next

            ' leggo impostazioni utente/superuser
            ui_R.Leggi_Impostazione_APP(enum_Impostazioni_Utenti.UTENTE_GIASAPP_LAVORAZIONI_IN_CAMPO, dtImpostazioni, objParametri_Utenti)
            ui_R.Leggi_Impostazione_APP(enum_Impostazioni_Utenti.UTENTE_GIASAPP_GESTIONE_POSIZIONE, dtImpostazioni, objParametri_Utenti)

            ' legge permessi app utente e forza le impostazioni
            Dim leggiPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            Dim gestionePermessi As Boolean = ui_R.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_Permessi, objParametri_Utenti, 2) <> "0"
            Dim permessiGiasAPP As Boolean = leggiPermessi.Controlla_Permessi_Utente(objParametri_Utenti.UtenteUsername, enum_Id_Servizio.GiasOnline, enum_Security_Attivita.GiasAPP_Permessi, enum_Security_Operazione.Lettura, Date.Now, "", objParametri_Utenti)

            ' se è attiva la gestione permessi gestisco il blocco permessi e forzo le impostazioni app
            Dim gestionePermessiGiasAPP As String = If(gestionePermessi And Not permessiGiasAPP, "0", "1")

            If gestionePermessi OrElse permessiGiasAPP Then

                Dim dtPermessi = leggiPermessi.Leggi_Permessi_APP(objParametri_Utenti.UtenteUsername, Date.Now, objParametri_Utenti, gestionePermessiGiasAPP)

                For Each permesso In dtPermessi.Rows
                    Dim impostazione = dtImpostazioni.Select("Impostazione_Cod=" & permesso.Item("Impostazione_Cod"))
                    If impostazione.Length = 0 Then
                        Dim drImpo = dtImpostazioni.NewRow
                        drImpo.Item("Impostazione_Cod") = permesso.Item("Impostazione_Cod")
                        drImpo.Item("Impostazione_Valore_1") = permesso.Item("Impostazione_Valore_1")
                        dtImpostazioni.Rows.Add(drImpo)
                    Else
                        impostazione(0).Item("Impostazione_Valore_1") = permesso.Item("Impostazione_Valore_1")
                    End If
                Next

                ' aggiungo blocco permessi + eventuali permessi extra
                Dim drImpostazioni = dtImpostazioni.NewRow
                drImpostazioni.Item("Impostazione_Cod") = enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_Permessi
                drImpostazioni.Item("Impostazione_Valore_1") = gestionePermessiGiasAPP
                dtImpostazioni.Rows.Add(drImpostazioni)

                ' verifico configurazione e permesso per operazioni agenda NG
                Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                Dim dtConfigSitiNG As DataTable = objConfigSiti.Leggi(0, "OperazioniAgendaNG", "", "", objParametri_Server)
                Dim operazioniAgendaNG As Boolean = Not IsNothing(dtConfigSitiNG) AndAlso dtConfigSitiNG.Rows.Count > 0 AndAlso LCase(dtConfigSitiNG.Rows(0).Item("Valore")) = "true"
                Dim permessoAgendaNG As Boolean = leggiPermessi.Controlla_Permessi_Utente(objParametri_Utenti.UtenteUsername, enum_Id_Servizio.GiasOnline, enum_Security_Attivita.Agenda_AccessoMenu_NG, enum_Security_Operazione.Lettura, Date.Now, "", objParametri_Utenti)

                ' aggiungo impostazione per configurazioni app
                drImpostazioni = dtImpostazioni.NewRow
                drImpostazioni.Item("Impostazione_Cod") = 0
                drImpostazioni.Item("Impostazione_Valore_1") = If(operazioniAgendaNG, If(permessoAgendaNG, "2", "1"), "0")
                dtImpostazioni.Rows.Add(drImpostazioni)

            End If

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(dtImpostazioni, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function


    '###############################################################################################
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiOpzioni_DocContabili(ByVal piva As String, ByVal objP_server As String, ByVal objP_utenti As String) As RispostaStandard

        Dim r As New RispostaStandard

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try

            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametriUtenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim obj As New AgronicaCoreUtentiBIZ.Utenti_Impostazioni_R
            Dim impDict As Dictionary(Of String, Object) = obj.LeggiOpzioni_DocContabili(piva, objParametriServer, objParametriUtenti)

            r.RispostaStringa = JsonConvert.SerializeObject(impDict, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function


    '###############################################################################################
    <WebMethod(EnableSession:=True)> _
    <Script.Services.ScriptMethod()> _
    Public Function Get_PersonalizzazioniGrigliaKendo(ByVal objP_utenti As String, _
                                                      ByVal pagina As String, _
                                                      ByVal nomeDiv As String
                                                      ) As rispostaStandard

        Dim r As New rispostaStandard()

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If pagina = "" Then
            r.Errore = "pagina non valorizzato"
            Return r
        End If

        If nomeDiv = "" Then
            r.Errore = "nomeDiv non valorizzato"
            Return r
        End If

        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

        'Leggo le impostazioni attuali
        Dim objUtentiImpostazioni_R As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

        'Il filtro mi aiuta ad evitare di ciclare tutte le personalizzazioni se sono certo non ci sia nulla che mi interessi
        Dim sbtfiltroAggiuntivoParameters As New StringBuilder


        sbtfiltroAggiuntivoParameters.AppendLine("  Declare @Pagina varchar(max) ")
        sbtfiltroAggiuntivoParameters.AppendLine("  Declare @NomeDiv varchar(max) ")
        sbtfiltroAggiuntivoParameters.AppendLine("  ")
        sbtfiltroAggiuntivoParameters.AppendLine("  Set @Pagina = '" & AgronicaCoreDataProvider.UtilityProvider.Agro_SQL_SaveText(pagina) & "' ")
        sbtfiltroAggiuntivoParameters.AppendLine("  Set @NomeDiv = '" & AgronicaCoreDataProvider.UtilityProvider.Agro_SQL_SaveText(nomeDiv) & "' ")


        Dim filtroAggiuntivoParameters As String = sbtfiltroAggiuntivoParameters.ToString()

        Dim filtroAggiuntivo As String = " Impostazione_Valore_1 LIKE '%' + @pagina + '%' AND Impostazione_Valore_1 LIKE '%' + @NomeDiv + '%' "
        Dim DT_personalizTutte As DataTable = objUtentiImpostazioni_R.LeggiQryParametrica(
                                                TipiEnumerativi.enum_Impostazioni_Utenti.UTENTE_COD_PERSONALIZZAZIONI_GRIGLIE_KENDO, 1,
                                                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, filtroAggiuntivoParameters, filtroAggiuntivo, "", objParametri_Utenti)

        If DT_personalizTutte.Rows.Count > 0 Then
            Dim personalizTutte As String = DT_personalizTutte.Rows(0).Item("Impostazione_Valore_1")

            'Converto in array json le impostazioni (o altrimenti creo un nuovo array)
            Dim JArrayPersonalizTutte As JArray = JArray.Parse(personalizTutte)

            'Controllo se esiste già un elemento con quella personalizzazione
            For Each obj As JObject In JArrayPersonalizTutte
                If obj("pagina").ToString().ToLower() = pagina.ToLower() AndAlso obj("nomeDiv").ToString().ToLower() = nomeDiv.ToLower() Then
                    r.RispostaStringa = JsonConvert.SerializeObject(obj, Formatting.None) 'Ricopio la nuova personalizzazione
                    Exit For
                End If
            Next
        End If

        'Se non ho trovato nulla, ritorno stringa vuota... (default)
        r.RispostaOK = True

        Return r
    End Function


    '###############################################################################################
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Set_PersonalizzazioniGrigliaKendo(ByVal objP_utenti As String,
                                                      ByVal personalizzazione As String
                                                      ) As rispostaStandard

        Dim r As New rispostaStandard()

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If personalizzazione = "" Then
            r.Errore = "personalizzazione non valorizzato"
            Return r
        End If

        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)
        Dim JobjectPersonalizzazione As JObject = JObject.Parse(personalizzazione)

        'Leggo le impostazioni attuali
        Dim objUtentiImpostazioni_R As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim DT_personalizTutte_old As DataTable = objUtentiImpostazioni_R.Leggi(
                                                TipiEnumerativi.enum_Impostazioni_Utenti.UTENTE_COD_PERSONALIZZAZIONI_GRIGLIE_KENDO, 1,
                                                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)

        Dim personalizTutte_old As String = If(DT_personalizTutte_old.Rows.Count = 0, "", DT_personalizTutte_old.Rows(0).Item("Impostazione_Valore_1"))
        'Modifico le impostazioni attuali

        'Converto in array json le impostazioni (o altrimenti creo un nuovo array)
        Dim JArrayPersonalizTutte As JArray = If(personalizTutte_old <> "", JArray.Parse(personalizTutte_old), New JArray())

        ''Controllo se esiste già un elemento con quella personalizzazione
        'For Each obj As JObject In JArrayPersonalizTutte
        '    If obj("pagina").ToString().ToLower() = JobjectPersonalizzazione("pagina").ToString().ToLower() AndAlso obj("nomeDiv").ToString().ToLower() = JobjectPersonalizzazione("nomeDiv").ToString().ToLower() Then
        '        JArrayPersonalizTutte.Remove(obj)
        '        'obj = JobjectPersonalizzazione.DeepClone() 'Ricopio la nuova personalizzazione
        '        Exit For
        '    End If
        'Next

        'Controllo se esiste già un elemento con quella personalizzazione
        Dim indexToDelete As Integer = -1
        For i As Integer = 0 To JArrayPersonalizTutte.Count - 1
            Dim obj = JArrayPersonalizTutte.Item(i)
            If obj("pagina").ToString().ToLower() = JobjectPersonalizzazione("pagina").ToString().ToLower() AndAlso obj("nomeDiv").ToString().ToLower() = JobjectPersonalizzazione("nomeDiv").ToString().ToLower() Then
                indexToDelete = i
                'JArrayPersonalizTutte.Remove(obj)
                'obj = JobjectPersonalizzazione.DeepClone() 'Ricopio la nuova personalizzazione
                Exit For
            End If
        Next

        If indexToDelete <> -1 Then
            JArrayPersonalizTutte.RemoveAt(indexToDelete)
        End If

        'aggiungo all'array la personalizzazione
        JArrayPersonalizTutte.Add(JobjectPersonalizzazione)

        'Converto in stringa l'array json
        Dim personalizTutte_new As String = JsonConvert.SerializeObject(JArrayPersonalizTutte, Formatting.None)

        'Salvo le nuove impostazioni
        Dim objUtentiImpostazioni_W As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_W
        Dim res As Boolean = False

        If DT_personalizTutte_old.Rows.Count = 0 Then
            'Array nuovo
            res = objUtentiImpostazioni_W.Scrivi(
                                TipiEnumerativi.enum_Impostazioni_Utenti.UTENTE_COD_PERSONALIZZAZIONI_GRIGLIE_KENDO,
                                personalizTutte_new, "", "", "", CostantiPersonalizzate.AGRODATAINIZIO,
                                CostantiPersonalizzate.AGRODATAFINE, objParametri_Utenti)

        Else
            'Array preesistente
            res = objUtentiImpostazioni_W.Modifica(
                    TipiEnumerativi.enum_Impostazioni_Utenti.UTENTE_COD_PERSONALIZZAZIONI_GRIGLIE_KENDO,
                    personalizTutte_new, "", "", "", CostantiPersonalizzate.AGRODATAINIZIO,
                    CostantiPersonalizzate.AGRODATAFINE, objParametri_Utenti)
        End If

        If res = True Then
            r.RispostaOK = True
        Else
            r.Errore = "errore durante il salvataggio"
        End If

        Return r

    End Function

    '###############################################################################################
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Del_PersonalizzazioniGrigliaKendo(ByVal objP_utenti As String,
                                                      ByVal pagina As String,
                                                      ByVal nomeDiv As String
                                                      ) As RispostaStandard

        Dim r As New RispostaStandard()

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If pagina = "" Then
            r.Errore = "pagina non valorizzato"
            Return r
        End If

        If nomeDiv = "" Then
            r.Errore = "pagina non valorizzato"
            Return r
        End If

        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

        'Leggo le impostazioni attuali
        Dim objUtentiImpostazioni_R As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim DT_personalizTutte_old As DataTable = objUtentiImpostazioni_R.Leggi(
                                                TipiEnumerativi.enum_Impostazioni_Utenti.UTENTE_COD_PERSONALIZZAZIONI_GRIGLIE_KENDO, 1,
                                                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)

        Dim personalizTutte_old As String = If(DT_personalizTutte_old.Rows.Count = 0, "", DT_personalizTutte_old.Rows(0).Item("Impostazione_Valore_1"))
        'Modifico le impostazioni attuali

        'Converto in array json le impostazioni (o altrimenti creo un nuovo array)
        Dim JArrayPersonalizTutte As JArray = If(personalizTutte_old <> "", JArray.Parse(personalizTutte_old), New JArray())

        'Controllo se esiste già un elemento con quella personalizzazione
        Dim indexToDelete As Integer = -1
        For i As Integer = 0 To JArrayPersonalizTutte.Count - 1
            Dim obj = JArrayPersonalizTutte.Item(i)
            If obj("pagina").ToString().ToLower() = pagina.ToLower() AndAlso obj("nomeDiv").ToString().ToLower() = nomeDiv.ToLower() Then
                indexToDelete = i
                'JArrayPersonalizTutte.Remove(obj)
                'obj = JobjectPersonalizzazione.DeepClone() 'Ricopio la nuova personalizzazione
                Exit For
            End If
        Next

        Dim res As Boolean = False
        If indexToDelete <> -1 Then
            JArrayPersonalizTutte.RemoveAt(indexToDelete)

            'Converto in stringa l'array json
            Dim personalizTutte_new As String = JsonConvert.SerializeObject(JArrayPersonalizTutte, Formatting.None)

            'Salvo le nuove impostazioni
            Dim objUtentiImpostazioni_W As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_W

            If DT_personalizTutte_old.Rows.Count = 0 Then
                'Array nuovo
                res = objUtentiImpostazioni_W.Scrivi(
                                TipiEnumerativi.enum_Impostazioni_Utenti.UTENTE_COD_PERSONALIZZAZIONI_GRIGLIE_KENDO,
                                personalizTutte_new, "", "", "", CostantiPersonalizzate.AGRODATAINIZIO,
                                CostantiPersonalizzate.AGRODATAFINE, objParametri_Utenti)

            Else
                'Array preesistente
                res = objUtentiImpostazioni_W.Modifica(
                    TipiEnumerativi.enum_Impostazioni_Utenti.UTENTE_COD_PERSONALIZZAZIONI_GRIGLIE_KENDO,
                    personalizTutte_new, "", "", "", CostantiPersonalizzate.AGRODATAINIZIO,
                    CostantiPersonalizzate.AGRODATAFINE, objParametri_Utenti)
            End If

        Else
            res = True
        End If

        If res = True Then
            r.RispostaOK = True
        Else
            r.Errore = "errore durante il salvataggio"
        End If

        Return r

    End Function

End Class



Public Class AnnataAgraria

    Public Sub New()
    End Sub

    Public Sub New(ByVal d1 As DateTime, ByVal d2 As DateTime)
        DataInizio = d1
        DataFine = d2
    End Sub

    Public DataInizio As Date
    Public DataFine As Date

End Class