Imports System.ComponentModel
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports AgronicaCoreAnagrafeBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreMetaSchemaDAL
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class DPI
    Inherits System.Web.Services.WebService

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboDPI_Regioni(objP_server As String, objP_utenti As String) As RispostaStandard

        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            '' VAnni: 6/9/2017: ovvio alla carenza di sessione..
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

            '' VAnni: 6/9/2017: fine carenza sessione...
            objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(Now.Date, Now.Date)

            Dim x As New AgronicaCoreDpiBIZ.CaricaListControl
            Dim ddl As New DropDownList
            x.Disciplinari_Elenco(ddl, False, "", "", Session,
                       objParametri_Server, objParametri_Utenti,
                       0, 0, 0, 0, False, False, False,
                       New AgronicaCoreGestioneRichieste.AgroWebConfig() With {.Flag_DisciplinarePrivato = False, .Flag_DisciplinareAttivo = True})

            objParametri_Server.ResettaFinestra()

            r.RispostaStringa = AgronicaCoreUtility.CaricaListControl.itemsCollection_AsJsonArray(ddl.Items, "value", "text", Now.Year & "|")
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function CaricaDettagliDisciplinare(objP_server As String, objP_utenti As String, disciplinare As String) As RispostaStandard

        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            '' VAnni: 6/9/2017: ovvio alla carenza di sessione..
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

            '' VAnni: 6/9/2017: fine carenza sessione...

            objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(Now.Date, Now.Date)

            Dim objAgroWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig
            Dim agroWs As String = objAgroWeb.GiasOnline_WS_Disciplinari_AgroWS_Disciplinari

            Dim ObjDownloadWs As New AgronicaCoreDpiBIZ.WS_Disciplinari.AgroWS_Disciplinari
            Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS
            objWs.NewWS(ObjDownloadWs, agroWs, objParametri_Utenti)

            'Richiamo il disciplinare pubblico
            Dim strErr As String = ""
            Dim Dati As String = ObjDownloadWs.Leggi_Disciplinari2(CInt(disciplinare.Split("/")(0)),
                                    0, 0, 0, 0, CostantiPersonalizzate.AGRODATAINIZIO, CostantiPersonalizzate.AGRODATAFINE,
                                    CStr(Session("ASG_Utente_Username_Crypt").ToString),
                                    CStr(Session("ASG_Utente_Password_Crypt").ToString),
                                    strErr)

            If strErr = "" Then
                If Not IsNothing(Dati) AndAlso Dati.ToLower.IndexOf("errore") < 0 Then
                    Dim XmlDocumento As New System.Xml.XmlDocument
                    XmlDocumento.LoadXml(Dati)
                    Dim XmlNodo As System.Xml.XmlNodeList = XmlDocumento.GetElementsByTagName("Record")
                    If Not XmlNodo Is Nothing Then
                        Dati = XmlNodo(0).OuterXml()
                        'For Each XmlElemento As System.Xml.XmlElement In XmlNodo
                        'PUA_Regolamento_Cod = CInt(XmlElemento.GetAttribute("pua_regolamento_cod"))
                        'Session("DpiFertPredefinitoUtente") = PUA_Regolamento_Cod
                        'Next
                    End If
                End If
            End If

            objParametri_Server.ResettaFinestra()

            r.RispostaStringa = Dati
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboDPI(objP_server As String, objP_utenti As String, veg_cod As String, data As String, flag_disciplinareprivato As Boolean) As RispostaStandard

        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            '' VAnni: 6/9/2017: ovvio alla carenza di sessione..
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

            '' VAnni: 6/9/2017: fine carenza sessione...
            If IsDate(data) Then
                objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(CDate(data), CDate(data))
            Else
                objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(Now.Date, Now.Date)
            End If

            Dim x As New AgronicaCoreDpiBIZ.CaricaListControl
            Dim ddl As New DropDownList
            x.Disciplinari_Elenco(ddl, False, "", "", Session,
                                  objParametri_Server, objParametri_Utenti,
                                   0,
                                  veg_cod, 0, 0,
                                  True, True, False,
                       New AgronicaCoreGestioneRichieste.AgroWebConfig() With {.Flag_DisciplinarePrivato = flag_disciplinareprivato, .Flag_DisciplinareAttivo = True}, True)

            objParametri_Server.ResettaFinestra()

            r.RispostaStringa = AgronicaCoreUtility.CaricaListControl.itemsCollection_AsJsonArray(ddl.Items, "value", "text")
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboDPI_Ente(objP_server As String, objP_utenti As String) As RispostaStandard

        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            '' VAnni: 6/9/2017: ovvio alla carenza di sessione..
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

            '' VAnni: 6/9/2017: fine carenza sessione...
            objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(Now.Date, Now.Date)

            Dim x As New AgronicaCoreDpiBIZ.CaricaListControl
            Dim ddl As New DropDownList
            x.Disciplinari_Elenco_Ente(ddl, False, "", "", Session,
                       objParametri_Server, objParametri_Utenti,
                       0, 0, 0, 0, False, False, False,
                       New AgronicaCoreGestioneRichieste.AgroWebConfig() With {.Flag_DisciplinarePrivato = True, .Flag_DisciplinareAttivo = True})

            objParametri_Server.ResettaFinestra()

            r.RispostaStringa = AgronicaCoreUtility.CaricaListControl.itemsCollection_AsJsonArray(ddl.Items, "value", "text", Now.Year & "|")
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function


    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboDisciplinare_Ente_NG(InData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Metaschema.LeggiDisciplinari)) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.Disciplinare))
        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.Disciplinare))

        If InData.objP.objP_server = "" OrElse InData.objP.objP_utenti = "" Then
            r.Errore = If(InData.objP.objP_server = "", "objP_server non valorizzato", "objP_utenti non valorizzato")
            Return r
        End If

        Try
            Dim disciplinari As New List(Of AgronicaCoreModelsSTD.metaschema.Disciplinare)
            Dim risp As RispostaStandard = CaricaComboDPI_Ente(InData.objP.objP_server, InData.objP.objP_utenti)
            Dim jArr As JArray = JsonConvert.DeserializeObject(risp.RispostaStringa)

            For i As Integer = 0 To jArr.Count - 1
                Dim value = jArr(i)("value").ToString
                Dim text = jArr(i)("text").ToString
                disciplinari.Add(New AgronicaCoreModelsSTD.metaschema.Disciplinare(value) With {.descrizione = text})
            Next

            r.RispostaStringa = disciplinari
            r.RispostaOK = True
        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            End Try

        Return r
    End Function


    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboDPI_ConTipoRegolamento(objP_server As String, objP_utenti As String, veg_cod As String, data As String, flag_disciplinareprivato As Boolean) As RispostaStandard

        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim callWS As Boolean = True
            Dim cache As Cache
            If HttpContext.Current.Cache IsNot Nothing Then
                cache = HttpContext.Current.Cache
            End If

            Dim key As String = "CaricaComboDPI_ConTipoRegolamento_" + veg_cod + "-" + data + "-" + CStr(flag_disciplinareprivato) + "-" + objParametri_Server.PivaSuperUser

            If cache IsNot Nothing And cache.Item(key) IsNot Nothing Then
                r.RispostaStringa = cache.Item(key)
                r.RispostaOK = True
                Return r
            End If

            '' VAnni: 6/9/2017: ovvio alla carenza di sessione..
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

            '' VAnni: 6/9/2017: fine carenza sessione...
            If IsDate(data) Then
                objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(CDate(data), CDate(data))
            Else
                objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(New Date(1900, 1, 1), New Date(2100, 12, 31))
            End If

            Dim x As New AgronicaCoreDpiBIZ.CaricaListControl
            Dim ddl As New DropDownList
            x.Disciplinari_Elenco_TuttigliElemInChiave(ddl, False, "", "", Session,
                                  objParametri_Server, objParametri_Utenti,
                                   0,
                                  veg_cod, 0, 0,
                                  True, True, False,
                       New AgronicaCoreGestioneRichieste.AgroWebConfig() With {.Flag_DisciplinarePrivato = flag_disciplinareprivato, .Flag_DisciplinareAttivo = True}, True)

            objParametri_Server.ResettaFinestra()

            Dim result = AgronicaCoreUtility.CaricaListControl.itemsCollection_AsJsonArray(ddl.Items, "value", "text")

            If cache IsNot Nothing Then
                cache.Item(key) = result
            End If

            r.RispostaStringa = result
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboIAF(objP_server As String, objP_utenti As String, data As String, flag_disciplinareprivato As Boolean, veg_cod As String, disciplinare_cod As String) As RispostaStandard

        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            '' VAnni: 6/9/2017: ovvio alla carenza di sessione..
            Dim xLeggiConfigurazioneSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

            Dim GiasOnline_WS_Disciplinari_AgroWS_Disciplinari As String = xLeggiConfigurazioneSiti.Leggi_Valore(0, "GiasOnline_WS_Disciplinari_AgroWS_Disciplinari", "", "", objParametri_Server)
            HttpContext.Current.Session("WS_DPI") = GiasOnline_WS_Disciplinari_AgroWS_Disciplinari

            Dim GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci As String = xLeggiConfigurazioneSiti.Leggi_Valore(0, "GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci", "", "", objParametri_Server)
            HttpContext.Current.Session("WS_FITO") = GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci


            Session("ASG_SuperUser_CodFiscale") = objParametri_Server.PivaSuperUser
            Session("ASG_Utente_Username_Crypt") = Sicurezza.Stringa_Codifica_LANCompatibile(objParametri_Server.SuperUserUsername, CostantiPersonalizzate.AgroKey_EncoderDecoder)

            Dim xletturautente As New AgronicaCoreUtentiDAL.Utenti_Read
            Dim pass As String = xletturautente.Password_From_UserName(objParametri_Server.SuperUserUsername, objParametri_Utenti)
            Session("ASG_Utente_Password_Crypt") = Sicurezza.Stringa_Codifica_LANCompatibile(pass, AgroKey_EncoderDecoder)

            '' VAnni: 6/9/2017: fine carenza sessione...
            If IsDate(data) Then
                objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(CDate(data), CDate(data))
            Else
                objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(AGRODATAINIZIO, AGRODATAFINE)
            End If

            Dim x As New AgronicaCoreDpiBIZ.CaricaListControl
            Dim ddl As New DropDownList
            x.IAF_Elenco(ddl, False, "", "", Session,
                         objParametri_Server, objParametri_Utenti, disciplinare_cod, veg_cod,
                       New AgronicaCoreGestioneRichieste.AgroWebConfig() With {.Flag_DisciplinarePrivato = flag_disciplinareprivato, .Flag_DisciplinareAttivo = True})

            objParametri_Server.ResettaFinestra()

            r.RispostaStringa = AgronicaCoreUtility.CaricaListControl.itemsCollection_AsJsonArray(ddl.Items, "value", "text")


            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function CaricaTipiConduzione(objP_server As String, objP_utenti As String, piva As String) As RispostaStandard

        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim JArrayListaOp As New JArray()

            'Estraggo i pacchetti acquistati
            Dim leggiServizioStatoQDC As New AgronicaCoreProfilazioneBIZ.Pratiche_R
            Dim xRisp As RispostaStandard = leggiServizioStatoQDC.leggiServiziStati(enum_WWorflow.Servizi_Agronica_2017, piva, 0, "", objParametri_Server, objParametri_Utenti)

            If xRisp.RispostaOK Then
                Dim descrizioneServizioStatoQDCArray As JArray = JArray.Parse(xRisp.RispostaStringa)
                Dim hashConduzioni As New Hashtable
                If descrizioneServizioStatoQDCArray.Count > 0 Then
                    For Each itemDescrizioneServizioStatoQDC As JObject In descrizioneServizioStatoQDCArray
                        Select Case CInt(itemDescrizioneServizioStatoQDC("ServizioCod"))
                            Case enum_Servizi.QStandard
                                If Not hashConduzioni.Contains("Convenzionale") Then
                                    JArrayListaOp.Add(New JObject(New JProperty("text", "Convenzionale"), New JProperty("value", "0")))
                                    hashConduzioni.Add("Convenzionale", "")
                                End If
                            Case enum_Servizi.QPlus
                                If Not hashConduzioni.Contains("Convenzionale") Then
                                    JArrayListaOp.Add(New JObject(New JProperty("text", "Convenzionale"), New JProperty("value", "0")))
                                    hashConduzioni.Add("Convenzionale", "")
                                End If
                                If Not hashConduzioni.Contains("Integrata") Then
                                    JArrayListaOp.Add(New JObject(New JProperty("text", "Integrata"), New JProperty("value", "1")))
                                    hashConduzioni.Add("Integrata", "")
                                End If
                            Case enum_Servizi.QBio
                                If Not hashConduzioni.Contains("Convenzionale") Then
                                    JArrayListaOp.Add(New JObject(New JProperty("text", "Convenzionale"), New JProperty("value", "0")))
                                    hashConduzioni.Add("Convenzionale", "")
                                End If
                                If Not hashConduzioni.Contains("Integrata") Then
                                    JArrayListaOp.Add(New JObject(New JProperty("text", "Integrata"), New JProperty("value", "1")))
                                    hashConduzioni.Add("Integrata", "")
                                End If
                                If Not hashConduzioni.Contains("Biologico") Then
                                    JArrayListaOp.Add(New JObject(New JProperty("text", "Biologico"), New JProperty("value", "2")))
                                    hashConduzioni.Add("Biologico", "")
                                End If
                        End Select
                    Next

                Else

                    'Controllo se è un utente CAA (l'impostazione non esiste o è vuota)
                    Dim ui_R As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
                    Dim res As String = ui_R.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.UTENTE_Attiva_Configurazione_Pratica, objParametri_Utenti)


                    If res = "" OrElse res = "0" Then

                        JArrayListaOp.Add(New JObject(New JProperty("text", "Convenzionale"), New JProperty("value", "0")))
                        JArrayListaOp.Add(New JObject(New JProperty("text", "Integrata"), New JProperty("value", "1")))
                        JArrayListaOp.Add(New JObject(New JProperty("text", "Biologico"), New JProperty("value", "2")))

                    End If

                End If

                r.RispostaStringa = JsonConvert.SerializeObject(JArrayListaOp, Formatting.None)
                r.RispostaOK = True

            End If

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboDisciplinare(objP_server As String, objP_utenti As String, veg_cod As String, data As String, flag_disciplinareprivato As Boolean, reg_cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim callWS As Boolean = True
            Dim cache As Cache
            If HttpContext.Current.Cache IsNot Nothing Then
                cache = HttpContext.Current.Cache
            End If

            Dim key As String = "CaricaComboDisciplinare_" + veg_cod + "-" + data + "-" + CStr(flag_disciplinareprivato) + "-" + objParametri_Server.PivaSuperUser

            If cache IsNot Nothing And cache.Item(key) IsNot Nothing Then
                r.RispostaStringa = cache.Item(key)
                r.RispostaOK = True
                Return r
            End If

            '' VAnni: 6/9/2017: ovvio alla carenza di sessione..
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

            '' VAnni: 6/9/2017: fine carenza sessione...
            If IsDate(data) Then
                'TODO: Giulia fix data stringa
                objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(CDate(data), AGRODATAFINE)
            Else
                objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(AGRODATAINIZIO, AGRODATAFINE)
            End If

            Dim x As New AgronicaCoreDpiBIZ.CaricaListControl
            Dim ddl As New DropDownList
            x.Disciplinari_Elenco_TuttigliElemInChiave(ddl, False, "", "", Session,
                                  objParametri_Server, objParametri_Utenti,
                                   reg_cod,
                                  veg_cod, 0, 0,
                                  True, True, False,
                       New AgronicaCoreGestioneRichieste.AgroWebConfig() With {.Flag_DisciplinarePrivato = flag_disciplinareprivato, .Flag_DisciplinareAttivo = True}, True)

            objParametri_Server.ResettaFinestra()

            Dim result = AgronicaCoreUtility.CaricaListControl.itemsCollection_AsJsonArray(ddl.Items, "value", "text")

            If cache IsNot Nothing Then
                cache.Item(key) = result
            End If

            r.RispostaStringa = result
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboDisciplinare_Modello(InData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Metaschema.LeggiDisciplinari)) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.Disciplinare))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.Disciplinare))

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If InData.objP.objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim callWS As Boolean = True
            Dim cache As Cache
            If HttpContext.Current.Cache IsNot Nothing Then
                cache = HttpContext.Current.Cache
            End If

            Dim key As String = "CaricaComboDisciplinare_Modello_" & InData.InData.specie.codice & "-" & CStr(InData.InData.data) & "-" & CStr(InData.InData.privato) + "-" + objParametri_Server.PivaSuperUser



            Dim dt = AgronicaCoreWebService.Disciplinari_WS.Disciplinari_Elenco_TuttigliElemInChiave(objParametri_Super_Server,
                                                                        objParametri_Server,
                                                                        objParametri_Utenti,
                                                                        0,
                                                                         InData.InData.specie.codice, 0, 0, True, False, True)

            If (InData.InData.validita IsNot Nothing)
                dt = dt.AsEnumerable.
                    Where(Function(row) new IntervalloTemporale(row("Validita_Inizio"), row("Validita_Fine")).
                        overlaps(InData.InData.validita)
                    ).CopyToDataTable
            End If

            Dim listItems = (From row In dt.Rows Select New AgronicaCoreModelsSTD.metaschema.Disciplinare(row("codRegolamento") & "/" & row("PubblicoPrivato") & "/" & row("PUA_Regolamento_Cod") & "/" & row("id_tr")) With {
                                                    .descrizione = row("nomeEsteso"),
                                                    .disciplinarePubblicoPrivato = row("PubblicoPrivato"),
                                                    .idTr = row("id_tr"),
                                                    .regolamentoConcimazione = New AgronicaCoreModelsSTD.metaschema.RegolamentoConcimazione(row("PUA_Regolamento_Cod"))
                                                    }).ToList

            If cache IsNot Nothing Then
                cache.Item(key) = listItems
            End If

            If InData.InData.IncludiBiologico Or InData.InData.IncludiNessunDisciplinare Then
                Dim objControlli As New AgronicaControlli_2010.STD_Disciplinari
                Dim disciplinariDefault = objControlli.GetDisciplinariDefault()
                If InData.InData.IncludiBiologico Then
                    listItems.Insert(0, disciplinariDefault.disciplinareBio)
                End If
                If InData.InData.IncludiNessunDisciplinare Then
                    listItems.Insert(0, disciplinariDefault.disciplinareNessuno)
                End If
            End If


            r.RispostaStringa = listItems
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboIAF_Modello(InData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Metaschema.LeggiIAF)) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.ImpegniAggiuntiviFacoltativi))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.ImpegniAggiuntiviFacoltativi))

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

            '' VAnni: 6/9/2017: ovvio alla carenza di sessione..
            Dim xLeggiConfigurazioneSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

            Dim GiasOnline_WS_Disciplinari_AgroWS_Disciplinari As String = xLeggiConfigurazioneSiti.Leggi_Valore(0, "GiasOnline_WS_Disciplinari_AgroWS_Disciplinari", "", "", objParametri_Server)
            HttpContext.Current.Session("WS_DPI") = GiasOnline_WS_Disciplinari_AgroWS_Disciplinari

            Dim GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci As String = xLeggiConfigurazioneSiti.Leggi_Valore(0, "GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci", "", "", objParametri_Server)
            HttpContext.Current.Session("WS_FITO") = GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci


            Session("ASG_SuperUser_CodFiscale") = objParametri_Server.PivaSuperUser
            Session("ASG_Utente_Username_Crypt") = Sicurezza.Stringa_Codifica_LANCompatibile(objParametri_Server.SuperUserUsername, CostantiPersonalizzate.AgroKey_EncoderDecoder)

            Dim xletturautente As New AgronicaCoreUtentiDAL.Utenti_Read
            Dim pass As String = xletturautente.Password_From_UserName(objParametri_Server.SuperUserUsername, objParametri_Utenti)
            Session("ASG_Utente_Password_Crypt") = Sicurezza.Stringa_Codifica_LANCompatibile(pass, AgroKey_EncoderDecoder)

            '' VAnni: 6/9/2017: fine carenza sessione...
            If InData.InData.data <> AGRODATAINIZIO Then
                objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(CDate(InData.InData.data), CDate(InData.InData.data))
            Else
                objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(AGRODATAINIZIO, AGRODATAFINE)
            End If

            Dim disciplinare_cod = InData.InData.disciplinare.codice.Split("/")(0)

            Dim x As New AgronicaCoreDpiBIZ.CaricaListControl
            Dim ddl As New DropDownList
            x.IAF_Elenco(ddl, False, "", "", Session,
                         objParametri_Server, objParametri_Utenti, disciplinare_cod, InData.InData.specie.codice,
                       New AgronicaCoreGestioneRichieste.AgroWebConfig() With {
                       .Flag_DisciplinarePrivato = InData.InData.privato,
                       .Flag_DisciplinareAttivo = True
                       })

            objParametri_Server.ResettaFinestra()

            Dim listItems = (From item As ListItem In ddl.Items Select New AgronicaCoreModelsSTD.metaschema.ImpegniAggiuntiviFacoltativi(item.Value) With {
                .descrizione = item.Text
            }).ToList

            r.RispostaStringa = listItems
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function


    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Disciplinari_Testata_conRegolamentoConcimazione(InData As Object) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.Disciplinare))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.Disciplinare))

        Try

            Dim datiRequest As String = JsonConvert.SerializeObject(InData)
            Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Metaschema.LeggiDisciplinari))(datiRequest)
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)
            Dim objParametri_DPI As AgronicaCoreDTOStd.InData.Metaschema.LeggiDisciplinari = objRequest.InData

            If objParametri_Server Is Nothing Then
                r.Errore = "objP_server non valorizzato"
                Return r
            End If

            If objParametri_Utenti Is Nothing Then
                r.Errore = "objP_utenti non valorizzato"
                Return r
            End If

            If objParametri_Super_Server Is Nothing Then
                r.Errore = "objP_super_server non valorizzato"
                Return r
            End If

            Dim lav_cod_list As Integer() = {}

            If Not IsNothing(objParametri_DPI.lavorazioni) AndAlso objParametri_DPI.lavorazioni.Length > 0 Then
                lav_cod_list = Array.ConvertAll(objParametri_DPI.lavorazioni, Function(s) Convert.ToInt32(s.primaryKey.codice))
            End If

            Dim objDPI As New AgronicaControlli_2010.STD_Disciplinari
            Dim disciplinariList = objDPI.Leggi_Disciplinari_Testata_conRegolamentoConcimazione(0,
                                                                    objParametri_DPI.specie.codice,
                                                                    0,
                                                                    lav_cod_list,
                                                                    objParametri_DPI.data,
                                                                    objParametri_Super_Server,
                                                                    objParametri_Server,
                                                                    objParametri_Utenti)

            If objParametri_DPI.leggiPianoNutrizionale Then
                disciplinariList.AddRange(objDPI.Leggi_Disciplinari_Testata_PianoNutrizionale(0,
                                                                                                 objParametri_DPI.specie.codice,
                                                                                                 objParametri_DPI.data,
                                                                                                 objParametri_Super_Server,
                                                                                                 objParametri_Server,
                                                                                                 objParametri_Utenti))
            End If

            r.RispostaStringa = disciplinariList
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Disciplinari_Testata_DirettivaNitrati(InData As Object) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.Disciplinare))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.Disciplinare))

        Try

            Dim datiRequest As String = JsonConvert.SerializeObject(InData)
            Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Metaschema.LeggiDisciplinari))(datiRequest)
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)
            Dim objParametri_DPI As AgronicaCoreDTOStd.InData.Metaschema.LeggiDisciplinari = objRequest.InData

            If objParametri_Server Is Nothing Then
                r.Errore = "objP_server non valorizzato"
                Return r
            End If

            If objParametri_Utenti Is Nothing Then
                r.Errore = "objP_utenti non valorizzato"
                Return r
            End If

            If objParametri_Super_Server Is Nothing Then
                r.Errore = "objP_super_server non valorizzato"
                Return r
            End If

            Dim lav_cod_list As Integer() = Array.ConvertAll(objParametri_DPI.lavorazioni, Function(s) Convert.ToInt32(s.primaryKey.codice))

            Dim objDPI As New AgronicaControlli_2010.STD_Disciplinari
            Dim disciplinariList = objDPI.Leggi_Disciplinari_Testata_DirettivaNitrati(0,
                                                                                      lav_cod_list,
                                                                    objParametri_DPI.data,
                                                                    objParametri_Super_Server,
                                                                    objParametri_Server,
                                                                    objParametri_Utenti)

            r.RispostaStringa = disciplinariList
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboDPI_Nome_e_codRegolamento(objP_server As String, objP_utenti As String,
                                                         ByVal seleziona_Nome_e_codRegolamento As Boolean) As RispostaStandard

        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            '' VAnni: 6/9/2017: ovvio alla carenza di sessione..
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

            '' Vanni: 6/9/2017: fine carenza sessione...
            objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(Now.Date, Now.Date)

            Dim x As New AgronicaCoreDpiBIZ.CaricaListControl
            Dim ddl As New DropDownList
            x.Disciplinari_Elenco_Ente(ddl, False, "", "", Session,
                       objParametri_Server, objParametri_Utenti,
                       0, 0, 0, 0, False, False, False,
                       New AgronicaCoreGestioneRichieste.AgroWebConfig() With {.Flag_DisciplinarePrivato = True, .Flag_DisciplinareAttivo = True},
                       IncludiPUA_Regolamento_Cod:=True,
                       seleziona_Nome_e_codRegolamento:=True)

            objParametri_Server.ResettaFinestra()

            r.RispostaStringa = AgronicaCoreUtility.CaricaListControl.itemsCollection_AsJsonArray(ddl.Items, "value", "text") ', Now.Year & "|")
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboVincoli_Modello(InData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Metaschema.LeggiVincoli)) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.Vincolo))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.Vincolo))

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If InData.objP.objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim callWS As Boolean = True
            Dim cache As Cache
            If HttpContext.Current.Cache IsNot Nothing Then
                cache = HttpContext.Current.Cache
            End If

            'Dim key As String = "CaricaComboVincoli_Modello" & InData.InData.specie.codice & "-" & CStr(InData.InData.data) & "-" & CStr(InData.InData.privato) + "-" + objParametri_Server.PivaSuperUser



            Dim listItems = AgronicaCoreWebService.Vincoli_WS.LeggiVincoli(InData.InData, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)

            'If cache IsNot Nothing Then
            '    cache.Item(key) = listItems
            'End If

            r.RispostaStringa = listItems
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboVincoliOrdered_Modello(InData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Metaschema.LeggiVincoli)) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.Vincolo))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.Vincolo))

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If InData.objP.objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim callWS As Boolean = True
            Dim cache As Cache
            If HttpContext.Current.Cache IsNot Nothing Then
                cache = HttpContext.Current.Cache
            End If

            'Dim key As String = "CaricaComboVincoli_Modello" & InData.InData.specie.codice & "-" & CStr(InData.InData.data) & "-" & CStr(InData.InData.privato) + "-" + objParametri_Server.PivaSuperUser



            Dim listItems = AgronicaCoreWebService.Vincoli_WS.LeggiVincoliOrdered(InData.InData, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)

            'If cache IsNot Nothing Then
            '    cache.Item(key) = listItems
            'End If

            r.RispostaStringa = listItems
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

End Class