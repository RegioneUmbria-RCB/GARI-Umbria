

Imports System.Web.UI.WebControls
Imports System.Xml
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports System.Web.Script.Serialization
Imports AgronicaCoreDataProvider
Imports Newtonsoft.Json.Linq


Public Class Meteo


    Public Shared Function Carica_Dati_Sazione_WS_StringaWS(
        ByVal TestoMessaggioErrore As String,
        ByVal ASG_Utente_Username_Crypt As String,
        ByVal ASG_Utente_Password_Crypt As String,
        ByVal objMeteo As AgronicaCoreWebService.Meteo,
        ByVal Data_Da As Date,
        ByVal Data_A As Date,
        ByVal tipoSorgente As String,
        ByVal sorgente As String,
        ByRef MessaggioErrore As String,
        ByRef flag_tabella_leggi As String
    ) As String

        'crea xml x ws
        Dim XML_Parametri As XmlElement
        Dim Str_XML_Parametri As String = ""
        Dim Doorkey As String = "Y4h8u3B5w2"
        Dim Security_Token As String = ""
        Dim Flag_Dettagli As String = "L" 'L=info minime; H=dettagli aggiuntivi
        Dim Flag_Aggrega As String = "G" ' G=giornaliero; H= orario; I=intervallo        
        Dim Numero_Stazione As Integer = 0
        Dim Numero_Quadrante As Integer = -1

        Dim SogliaMM As Decimal = 0
        Dim Ora As String = "23"

        Dim Flag_Spazio As String = "S"

        Try


            If tipoSorgente = enum_Meteo_Tiposorgente.Gias_RER Then
                Numero_Stazione = sorgente
                sorgente = ""
            Else
                If tipoSorgente = enum_Meteo_Tiposorgente.Gias_RER_Quadranti Then
                    Numero_Stazione = -1
                    Numero_Quadrante = sorgente
                    Flag_Spazio = "Q"
                    sorgente = ""
                Else
                    Numero_Stazione = -1
                    flag_tabella_leggi = "M"
                End If

            End If

            XML_Parametri = objMeteo.Crea_XMLParametri_DatiPrecipitazioni(
                Nothing,
                Doorkey,
                ASG_Utente_Username_Crypt,
                ASG_Utente_Password_Crypt,
                Security_Token,
                Flag_Dettagli,
                Flag_Aggrega,
                Flag_Spazio,
                SogliaMM,
                Ora,
                Numero_Quadrante,
                Numero_Stazione,
                0,
                0,
                Data_Da,
                Data_A,
                NomeStazione:=sorgente,
                flag_tabella_leggi:=flag_tabella_leggi
             )

            Str_XML_Parametri = XML_Parametri.OuterXml

            Str_XML_Parametri = objMeteo.Cripta_XML_Parametri(Str_XML_Parametri)


        Catch ex As Exception
            MessaggioErrore = TestoMessaggioErrore &
            vbCrLf + ex.Message
            Return ""
        End Try


        Return Str_XML_Parametri
    End Function

#If False Then
    Public Function DatiMeteo_Elabora(ByVal Str_XML_Parametri_Meteo As String, ByVal Str_XML_Parametri As String, ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim hlpHttp As New Http
        Dim rval As String = ""
        Dim url As String = ""

        urlMeteo_Leggi(objParametri_Server, url)
        url &= "/DatiMeteo_Calcola"

        Dim jss As New JavaScriptSerializer
        Dim s As String = "{ ""InputData_Meteo"": """ & Str_XML_Parametri_Meteo & """, ""Params"": """ & Str_XML_Parametri & """  }"

        rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "", TimeOutRichiesta:=600)

        Return rval

    End Function

    Public Function DatiMeteo_Elabora2(ByVal Str_XML_Parametri As String, ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim hlpHttp As New Http
        Dim rval As String = ""
        Dim url As String = ""

        urlMeteo_Leggi(objParametri_Server, url)
        url &= "/DatiMeteo_Calcola2"

        Dim jss As New JavaScriptSerializer
        Dim s As String = "{ ""xmlParams"": """ & Str_XML_Parametri & """  }"

        rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "", TimeOutRichiesta:=600)

        Return rval

    End Function

#End If

    Public Function ModelliPrevisionaliRaggruppamenti_Lista(ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim hlpHttp As New Http
        Dim rval As String = ""
        Dim url As String = ""

        urlMeteo_Leggi(objParametri_Server, url)
        url &= "/ModelliPrevisionaliRaggruppamenti_Elenco"


        Dim jss As New JavaScriptSerializer
        'byval ModelliPrevisionali_ID As Integer, byval ModelliPrevisionali_InputData_Meteo As String, byval ModelliPrevisionali_InputData As String
        Dim s As String = "{}"

        rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "")


        Return rval

    End Function
    Public Function ModelliPrevisionali_Lista(ByVal veg_cod As Integer, ByVal av_cod As Integer, ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim hlpHttp As New Http
        Dim rval As String = ""
        Dim url As String = ""

        urlMeteo_Leggi(objParametri_Server, url)
        url &= "/ModelliPrevisionali_Elenco"


        Dim jss As New JavaScriptSerializer
        'byval ModelliPrevisionali_ID As Integer, byval ModelliPrevisionali_InputData_Meteo As String, byval ModelliPrevisionali_InputData As String
        Dim s As String = "{ ""veg_Cod"": " & veg_cod & ", ""av_Cod"": " & av_cod & " }"

        rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "")


        Return rval

    End Function
    Public Function ModelliPrevisionali_Elenco_Algoritmi(ByVal modello As Integer, ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim hlpHttp As New Http
        Dim rval As String = ""
        Dim url As String = ""

        urlMeteo_Leggi(objParametri_Server, url)
        url &= "/ModelliPrevisionali_Elenco_Algoritmi"


        Dim jss As New JavaScriptSerializer
        'byval ModelliPrevisionali_ID As Integer, byval ModelliPrevisionali_InputData_Meteo As String, byval ModelliPrevisionali_InputData As String
        Dim s As String = "{ ""modello"": " & modello & " }"

        rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "")

        Return rval

    End Function

    Public Function SpeciexModelli_Inizializza(ByVal piva As String, ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String
        Dim hlpHttp As New Http
        Dim rval As String = ""
        Dim url As String = ""

        urlMeteo_Leggi(objParametri_Server, url)
        url &= "/SpeciexModelli_Inizializza"

        Dim Str_XML_Parametri As String = "<PARAMETRI piva_superuser = """ & objParametri_Server.PivaSuperUser & """ piva = """ & piva & """></PARAMETRI>"
        Str_XML_Parametri = Cripta_XML_Parametri(Str_XML_Parametri)

        Dim s As String = "{ ""Parametri"": """ & Str_XML_Parametri & """ }"

        rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "")

        Return rval

    End Function

    Public Function AvModAlg_Inizializza(ByVal piva As String, ByVal veg_cod As Integer, ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim url As String = ""
        urlMeteo_Leggi(objParametri_Server, url)
        url &= "/AvModAlg_InizializzaConParametri"

        Dim Str_XML_Parametri As String = "<PARAMETRI piva_superuser = """ & objParametri_Server.PivaSuperUser & """ piva = """ & piva & """ veg_cod= """ & veg_cod & """ ></PARAMETRI>"
        Str_XML_Parametri = Cripta_XML_Parametri(Str_XML_Parametri)

        Dim jss As New JavaScriptSerializer
        'byval ModelliPrevisionali_ID As Integer, byval ModelliPrevisionali_InputData_Meteo As String, byval ModelliPrevisionali_InputData As String
        Dim s As String = "{ ""Parametri"": """ & Str_XML_Parametri & """ }"

        Dim hlpHttp As New Http
        Dim risp = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "")

        Dim objRisp = JObject.Parse(risp)

        Return objRisp("AvModAlg_InizializzaConParametriResult").ToString
    End Function

    Public Function ModelliPrevisionali_Elenco_Avversita(ByVal veg_cod As Integer, ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim hlpHttp As New Http
        Dim rval As String = ""
        Dim url As String = ""

        urlMeteo_Leggi(objParametri_Server, url)
        url &= "/ModelliPrevisionali_Elenco_Avversita"


        Dim jss As New JavaScriptSerializer
        'byval ModelliPrevisionali_ID As Integer, byval ModelliPrevisionali_InputData_Meteo As String, byval ModelliPrevisionali_InputData As String
        Dim s As String = "{ ""veg_Cod"": " & veg_cod & " }"

        rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "")


        Return rval

    End Function
    Public Function ModelliPrevisionali_Elabora(ByVal Str_XML_Parametri_Meteo As String, ByVal Str_XML_Parametri_Modello As String, ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim hlpHttp As New Http
        Dim rval As String = ""
        Dim url As String = ""

        urlMeteo_Leggi(objParametri_Server, url)
        url &= "/ModelliPrevisionali_Calcola"

        Str_XML_Parametri_Modello = Cripta_XML_Parametri(Str_XML_Parametri_Modello)

        'Dim jss As New JavaScriptSerializer
        'byval ModelliPrevisionali_ID As Integer, byval ModelliPrevisionali_InputData_Meteo As String, byval ModelliPrevisionali_InputData As String
        Dim s As String = "{ ""ModelliPrevisionali_InputData_Meteo"": """ & Str_XML_Parametri_Meteo & """, ""ModelliPrevisionali_InputData"": """ & Str_XML_Parametri_Modello & """  }"

        rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "", TimeOutRichiesta:=600)

        Return rval

    End Function

    Public Function ModelliPrevisionali_ElaboraIndicatori(ByVal Parametri_XML As String, ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim url As String = ""
        urlMeteo_Leggi(objParametri_Server, url)
        url &= "/ModelliPrevisionali_CalcolaIndicatori"

        Dim req As String = Cripta_XML_Parametri(Parametri_XML)

        Dim hlpHttp As New Http
        Dim result As String = hlpHttp.chiamaWS("{ ""Parametri_Input"": """ & req & """}", Nothing, url, "application/json", "POST", "application/json", "", TimeOutRichiesta:=600)

        Return result

    End Function

    Private Shared Sub urlMeteo_Leggi(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, ByRef url As String)

        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim DTConfigSiti As DataTable

        DTConfigSiti = objConfigSiti.Leggi(0, "GiasOnline_WS_Meteo_Meteo", "", "", objParametri_Server)
        If Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 AndAlso LCase(DTConfigSiti.Rows(0).Item("Valore")) <> "" Then
            url = DTConfigSiti.Rows(0).Item("Valore")
            url = url.Replace("Meteo.asmx", "ModelliPrevisionaliMeteo.svc")
        End If
    End Sub


    '################################################################################
    Public Function Verifica_Attivazione_WS_DatiMeteo(ByVal Timeout As Integer,
                                                        ByVal Username_Crypt As String,
                                                        ByVal Password_Crypt As String,
                                                        ByVal objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        'chiama una funzione del WS per vedere se risponde

        'Dim pippo As Boolean
        'pippo = True
        ''pippo = False
        'Return pippo

        Dim ObjDownloadWsMeteo As New WS_Meteo.Meteo
        Dim objWs As AgronicaCoreVarieDAL.ConnessioneWS
        Dim objMeteo As New Meteo
        Dim XML_Parametri As XmlElement
        Dim Str_XML_Parametri As String = ""
        Dim Doorkey As String = "Y4h8u3B5w2"
        Dim Security_Token As String = ""
        Dim Flag_Risposta As Boolean = True
        Dim Somma As Integer
        Dim strErr As String

        Dim N_1 As Integer = 1
        Dim N_2 As Integer = 2

        If Timeout <> 0 Then
            ObjDownloadWsMeteo.Timeout = Timeout
        End If

        objWs = New AgronicaCoreVarieDAL.ConnessioneWS

        objWs.NewWS(ObjDownloadWsMeteo,
                    Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Meteo.Meteo").ToString(),
                    objParametri_Utenti)

        Str_XML_Parametri = objMeteo.Crypt_XMLParametri_VerificaCollegamento(Doorkey,
                                                                            Username_Crypt,
                                                                            Password_Crypt,
                                                                            Security_Token)

        Somma = ObjDownloadWsMeteo.VerificaCollegamento(Str_XML_Parametri,
                                                        N_1,
                                                        N_2,
                                                        strErr)

        If strErr <> "" Then
            Flag_Risposta = False
        Else
            If (N_1 + N_2) = Somma Then
                'tutto ok
            Else
                Flag_Risposta = False
            End If
        End If

        Return Flag_Risposta

    End Function



    '##########################################################################################
    'Crea l'xml da passare al ws del meteo per la lettura dei dati delle precipitazioni
    Public Function Crea_XMLParametri_DatiPrecipitazioni(ByRef XmlDoc As XmlDocument,
                                        ByVal Doorkey As String,
                                        ByVal Username As String,
                                        ByVal Password As String,
                                        ByVal Security_Token As String,
                                        ByVal Flag_Dettagli As String,
                                        ByVal Flag_Aggrega As String,
                                        ByVal Flag_Spazio As String,
                                        ByVal Soglia As Decimal,
                                        ByVal OraRilievo As String,
                                        ByVal Numero_Quadrante As Integer,
                                        ByVal Numero_Stazione As Integer,
                                        ByVal Coordinata_X As Integer,
                                        ByVal Coordinata_Y As Integer,
                                        ByVal Data_DA As Date,
                                        ByVal Data_A As Date,
                                        Optional ByVal NomeStazione As String = "",
                                        Optional ByVal flag_tabella_leggi As String = "H") As XmlElement

        Dim XML_Parametri As XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        XML_Parametri = XmlDoc.CreateElement("PARAMETRI")

        'Imposto gli attributi
        XML_Parametri.SetAttribute(LCase("Doorkey"), Doorkey)
        XML_Parametri.SetAttribute(LCase("USR"), Username)
        XML_Parametri.SetAttribute(LCase("pwd"), Password)
        XML_Parametri.SetAttribute(LCase("tkn"), Security_Token)
        XML_Parametri.SetAttribute(LCase("Flag_Dettagli"), Flag_Dettagli)
        XML_Parametri.SetAttribute(LCase("Flag_Aggrega"), Flag_Aggrega)
        XML_Parametri.SetAttribute(LCase("Flag_Spazio"), Flag_Spazio)
        XML_Parametri.SetAttribute(LCase("Soglia"), CStr(Soglia).Replace(".", ","))
        XML_Parametri.SetAttribute(LCase("OraRilievo"), CStr(OraRilievo))
        XML_Parametri.SetAttribute(LCase("idq"), CStr(Numero_Quadrante))
        XML_Parametri.SetAttribute(LCase("ids"), CStr(Numero_Stazione))
        XML_Parametri.SetAttribute(LCase("idx"), CStr(Coordinata_X))
        XML_Parametri.SetAttribute(LCase("idy"), CStr(Coordinata_Y))
        XML_Parametri.SetAttribute(LCase("data1"), CStr(Data_DA))
        XML_Parametri.SetAttribute(LCase("data2"), CStr(Data_A))
        XML_Parametri.SetAttribute(LCase("nSt"), CStr(NomeStazione))
        XML_Parametri.SetAttribute(LCase("flag_tabella_leggi"), CStr(flag_tabella_leggi))

        'Restituisco in uscita 
        Return XML_Parametri

    End Function

    '##########################################################################################
    'Crea l'xml da passare al ws del meteo per la verifica del collegamento
    Public Function Crea_XMLParametri_VerificaCollegamento(ByRef XmlDoc As XmlDocument,
                                                            ByVal Doorkey As String,
                                                            ByVal Username As String,
                                                            ByVal Password As String,
                                                            ByVal Security_Token As String) As XmlElement

        Dim XML_Parametri As XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        XML_Parametri = XmlDoc.CreateElement("PARAMETRI")

        'Imposto gli attributi
        XML_Parametri.SetAttribute(LCase("Doorkey"), Doorkey)
        XML_Parametri.SetAttribute(LCase("USR"), Username)
        XML_Parametri.SetAttribute(LCase("pwd"), Password)
        XML_Parametri.SetAttribute(LCase("tkn"), Security_Token)

        'Restituisco in uscita 
        Return XML_Parametri

    End Function

    '##########################################################################################
    'cripta l'xml da passare al ws del meteo per la verifica del collegamento
    Public Function Crypt_XMLParametri_VerificaCollegamento(ByVal Doorkey As String,
                                                            ByVal Username As String,
                                                            ByVal Password As String,
                                                            ByVal Security_Token As String) As String

        Dim Str_XML_Parametri As String = ""
        Dim XML_Parametri As XmlElement

        XML_Parametri = Crea_XMLParametri_VerificaCollegamento(Nothing,
                                                                Doorkey,
                                                                Username,
                                                                Password,
                                                                Security_Token)

        Str_XML_Parametri = XML_Parametri.OuterXml

        Str_XML_Parametri = Cripta_XML_Parametri(Str_XML_Parametri)

        Return Str_XML_Parametri

    End Function


    '##########################################################################################
    'Legge l'xml restituito dal ws del meteo
    Public Sub Leggi_XML_Risultato(ByVal Str_XML_Risultato As String,
                                    ByRef Numero_Quadrante As Integer,
                                    ByRef Numero_Nodi_Dato As Integer,
                                    ByRef Codice_Errore As Integer,
                                    ByRef Messaggio_Errore As String,
                                    ByRef XMLs_NodiDato As XmlNodeList)

        Dim XmlDoc As New XmlDocument
        Dim XML_Risultato As XmlElement

        'Carico la stringa nel documento XML
        XmlDoc.LoadXml(Str_XML_Risultato)

        XML_Risultato = XmlDoc.SelectSingleNode("RISULTATO")

        Codice_Errore = XML_Risultato.GetAttribute("errcod")
        Messaggio_Errore = XML_Risultato.GetAttribute("errmsg")

        If Codice_Errore = 0 Then

            Numero_Quadrante = XML_Risultato.GetAttribute("idq")
            Numero_Nodi_Dato = XML_Risultato.GetAttribute("num")

            XMLs_NodiDato = XmlDoc.GetElementsByTagName("DATO")

        End If


    End Sub

    '##########################################################################################
    Public Function Chiama_WS_Meteo_DatiPrecipitazioni(ByVal Str_XML_Parametri As String) As String

        Dim WS_Meteo As New WS_Meteo.Meteo
        Dim objAgroWebConfig As New AgroWebConfig
        WS_Meteo.Url = objAgroWebConfig.GiasOnline_WS_Meteo_Meteo

        Return WS_Meteo.DatiPrecipitazioni(Str_XML_Parametri)


    End Function

    Public Function Chiama_WS_Meteo_DatiCompleti(ByVal Str_XML_Parametri As String) As String

        Dim WS_Meteo As New WS_Meteo.Meteo
        Dim objAgroWebConfig As New AgroWebConfig
        WS_Meteo.Url = objAgroWebConfig.GiasOnline_WS_Meteo_Meteo

        Return WS_Meteo.DatiCompleti(Str_XML_Parametri)


    End Function

    Public Function Chiama_WS_Meteo_DatiPrecipitazioniMetos(ByVal Str_XML_Parametri As String) As String

        Dim WS_Meteo As New WS_Meteo.Meteo
        Dim objAgroWebConfig As New AgroWebConfig
        WS_Meteo.Url = objAgroWebConfig.GiasOnline_WS_Meteo_Meteo

        Return WS_Meteo.DatiPrecipitazioniMetos(Str_XML_Parametri)


    End Function


    '##########################################################################################
    'Cripta la stringa xml dei parameri utilizzando la stessa codifica del ws dei fitofarmaci (modulo AgroWS)
    Public Function Cripta_XML_Parametri(ByVal Str_XML_Parametri As String) As String

        Dim Str_XML_Parametri_Crypt As String

        Str_XML_Parametri_Crypt = New AgronicaCoreWebService.AgroWs().AWS_Codifica_P(Str_XML_Parametri)

        Return Str_XML_Parametri_Crypt

    End Function

    Public Sub Chiama_WS_Meteo_CaricaCombo_StazioniMeteoConDistanza(
        ByRef Controllo As ListControl,
        ByVal Piva_SuperUser As String,
        Long_Centro As Decimal, Lat_Centro As Decimal,
        ByVal PrimaRiga_Flag As Boolean,
        ByVal PrimaRiga_Text As String,
        ByVal PrimaRiga_Value As String,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByVal Utente_Username_client_GIAS As String,
        ByVal PivaVisibilita_ClientGiasImpostata As String,
        ByVal Username As String,
        ByVal Password As String,
        ByVal Doorkey As String,
        ByVal tiposorgente As enum_Meteo_Tiposorgente,
        ByRef DT_stazioni As DataTable,
        ByVal objParametri_Server As AgronicaCoreParametri)

        ' Dim StringaConnessione As String = "Provider=SQLOLEDB;Server=*****;Initial Catalog=AgronicaMeteoSuite;User Id=*****;Password=*****;"
        '= enum_Meteo_Tiposorgente.RetiPartner,
        '= Nothing
        Try
            Dim WS_Meteo As New WS_Meteo.Meteo
            Dim objAgroWebConfig As New AgroWebConfig
            Dim XML_Parametri As XmlElement
            Dim Str_XML_Parametri As String = ""

            XML_Parametri = Crea_XMLParametri_DatiListaStazioniConDistanza(
                Nothing,
                Doorkey,
                Utente_Username_client_GIAS,
                PivaVisibilita_ClientGiasImpostata,
                Username,
                Password,
                Piva_SuperUser,
                Long_Centro,
                Lat_Centro,
                tiposorgente,
                objParametri_Server
            )

            Str_XML_Parametri = XML_Parametri.OuterXml

            Str_XML_Parametri = Cripta_XML_Parametri(Str_XML_Parametri)

            WS_Meteo.Url = objAgroWebConfig.GiasOnline_WS_Meteo_Meteo

            Dim strElenco As String = WS_Meteo.Metos_Stazioni_Appoggio_Leggi_Con_Distanza(Str_XML_Parametri)

            DT_stazioni = elencoStazioniXMLtoDatatable(strElenco)




            'Pulisco il controllo
            Controllo.Items.Clear()

            If PrimaRiga_Flag = True Then
                Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
            End If


            If Not DT_stazioni Is Nothing AndAlso DT_stazioni.Rows.Count > 0 Then

                'l'ordinamento (NULL LAST) viene dalla query
                Dim txt As String
                For i = 0 To DT_stazioni.Rows.Count - 1

                    txt = DT_stazioni.Rows(i).Item("Descrizione") & " (Distanza "
                    If IsDBNull(DT_stazioni.Rows(i).Item("Distanza")) OrElse CInt(DT_stazioni.Rows(i).Item("Distanza")) = CInt("-1") Then
                        txt &= "non disponibile"
                    Else
                        txt &= CInt(CInt(DT_stazioni.Rows(i).Item("Distanza")) / 1000) & " km"
                    End If
                    txt &= ")"
                    txt &= IIf(CDate(DT_stazioni.Rows(i).Item("data_ultimo_agg")) > AGRODATAINIZIO, " (agg. al " & CStr(CDate(DT_stazioni.Rows(i).Item("data_ultimo_agg"))) & ")", "")
                    txt &= IIf(String.IsNullOrEmpty(DT_stazioni.Rows(i).Item("Stazione_Cod_Fornitore").ToString), "", " - " & DT_stazioni.Rows(i).Item("Stazione_Cod_Fornitore"))

                    Controllo.Items.Add(New ListItem() With {
                        .Text = txt,
                        .Value = DT_stazioni.Rows(i).Item("nome")
                    })

                Next




                'For i = 0 To DT_stazioni.Rows.Count - 1

                '    If Not IsDBNull(DT_stazioni.Rows(i).Item("Distanza")) AndAlso CInt(DT_stazioni.Rows(i).Item("Distanza")) <> CInt("-1") Then
                '        'descrizione

                '        Controllo.Items.Add(New ListItem() With {
                '            .Text = DT_stazioni.Rows(i).Item("Descrizione") &
                '                " (" & CInt(CInt(DT_stazioni.Rows(i).Item("Distanza")) / 1000) & " km)" &
                '                IIf(CDate(DT_stazioni.Rows(i).Item("data_ultimo_agg")) > AGRODATAINIZIO, " (agg. al " & CStr(CDate(DT_stazioni.Rows(i).Item("data_ultimo_agg"))) & ")", "") &
                '                IIf(String.IsNullOrEmpty(DT_stazioni.Rows(i).Item("Stazione_Cod_Fornitore").ToString), "", " - " & DT_stazioni.Rows(i).Item("Stazione_Cod_Fornitore")),
                '            .Value = DT_stazioni.Rows(i).Item("nome")
                '        })
                '    End If

                'Next

                'For i = 0 To DT_stazioni.Rows.Count - 1

                '    If IsDBNull(DT_stazioni.Rows(i).Item("Distanza")) Or CInt(DT_stazioni.Rows(i).Item("Distanza")) = CInt("-1") Then
                '        Controllo.Items.Add(New ListItem() With {
                '            .Text = DT_stazioni.Rows(i).Item("Descrizione") &
                '                " (Non Indicata)" &
                '                IIf(CDate(DT_stazioni.Rows(i).Item("data_ultimo_agg")) > AGRODATAINIZIO, " (agg. al " & CStr(CDate(DT_stazioni.Rows(i).Item("data_ultimo_agg"))) & ")", "") &
                '                IIf(String.IsNullOrEmpty(DT_stazioni.Rows(i).Item("Stazione_Cod_Fornitore").ToString), "", " - " & DT_stazioni.Rows(i).Item("Stazione_Cod_Fornitore")),
                '            .Value = DT_stazioni.Rows(i).Item("nome")
                '         })
                '    End If

                'Next


            End If

        Catch ex As Exception

        End Try



    End Sub


    Public Function Chiama_WS_Meteo_CaricaCombo_StazioniMeteoConDistanza2(
        ByVal Piva_SuperUser As String,
        ByVal Long_Centro As Decimal,
        ByVal Lat_Centro As Decimal,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByVal Utente_Username_client_GIAS As String,
        ByVal PivaVisibilita_ClientGiasImpostata As String,
        ByVal Username As String,
        ByVal Password As String,
        ByVal Doorkey As String,
        ByVal tiposorgente As enum_Meteo_Tiposorgente,
        ByVal objParametri_Server As AgronicaCoreParametri) As DataTable

        ' Dim StringaConnessione As String = "Provider=SQLOLEDB;Server=laurezio\SQL2012;Initial Catalog=AgronicaMeteoSuite;User Id=agronauta;Password=*****;"
        '= enum_Meteo_Tiposorgente.RetiPartner,
        '= Nothing

        Dim DT_Stazioni As DataTable = Nothing

        Try
            Dim WS_Meteo As New WS_Meteo.Meteo
            Dim objAgroWebConfig As New AgroWebConfig
            Dim XML_Parametri As XmlElement
            Dim Str_XML_Parametri As String = ""

            XML_Parametri = Crea_XMLParametri_DatiListaStazioniConDistanza(
                Nothing,
                Doorkey,
                Utente_Username_client_GIAS,
                PivaVisibilita_ClientGiasImpostata,
                Username,
                Password,
                Piva_SuperUser,
                Long_Centro,
                Lat_Centro,
                tiposorgente,
                objParametri_Server
            )

            Str_XML_Parametri = XML_Parametri.OuterXml

            Str_XML_Parametri = Cripta_XML_Parametri(Str_XML_Parametri)

            WS_Meteo.Url = objAgroWebConfig.GiasOnline_WS_Meteo_Meteo

            Dim strElenco As String = WS_Meteo.Metos_Stazioni_Appoggio_Leggi_Con_Distanza(Str_XML_Parametri)

            DT_Stazioni = elencoStazioniXMLtoDatatable(strElenco)

        Catch ex As Exception

        End Try

        Return DT_Stazioni

    End Function

    Public Function Crea_XMLParametri_DatiListaStazioniConDistanza(ByRef XmlDoc As XmlDocument,
            ByVal Doorkey As String,
            ByVal Utente_Username_Client_gias As String,
            ByVal PivaVisibilita_ClientGiasImpostata As String,
            ByVal Username As String,
            ByVal Password As String,
            ByVal Piva_SuperUser As String,
            ByVal Long_Centro As Decimal,
            ByVal Lat_Centro As Decimal,
            ByVal tiposorgente As enum_Meteo_Tiposorgente,
            ByVal objParametri_Server As AgronicaCoreParametri
            ) As XmlElement

        '= enum_Meteo_Tiposorgente.RetiPartner
        Dim XML_Parametri As XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        XML_Parametri = XmlDoc.CreateElement("PARAMETRI")

        'Imposto gli attributi
        XML_Parametri.SetAttribute(LCase("Doorkey"), Doorkey)
        XML_Parametri.SetAttribute(LCase("USR"), Username)
        XML_Parametri.SetAttribute(LCase("pwd"), Password)

        Dim Utente_Username_Client_gias_Cript As String = ""
        Try
            Utente_Username_Client_gias_Cript = AgronicaCoreDataProvider.Sicurezza.Stringa_Codifica_LANCompatibile(Utente_Username_Client_gias, AgroKey_EncoderDecoder)
        Catch ex As Exception

        End Try
        XML_Parametri.SetAttribute(LCase("ucg"), Utente_Username_Client_gias_Cript)

        Dim PivaVisibilita_ClientGias As String = ""

        If String.IsNullOrEmpty(PivaVisibilita_ClientGiasImpostata) Then


            Dim dtPiva As DataTable
            Dim xLetturaVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
            dtPiva = xLetturaVisibilita.Leggi(1, "", "", objParametri_Server)

            PivaVisibilita_ClientGias =
                String.Join(",", (From pp In dtPiva.AsEnumerable Select CStr(pp("piva"))).ToArray())

        Else
            PivaVisibilita_ClientGias = PivaVisibilita_ClientGiasImpostata

        End If

        XML_Parametri.SetAttribute(LCase("piva"), PivaVisibilita_ClientGias)


        XML_Parametri.SetAttribute(LCase("tkn"), "")

        XML_Parametri.SetAttribute(LCase("Piva_SuperUser"), Piva_SuperUser)
        XML_Parametri.SetAttribute(LCase("longitudine"), CStr(Long_Centro))
        XML_Parametri.SetAttribute(LCase("latitudine"), CStr(Lat_Centro))
        XML_Parametri.SetAttribute(LCase("tiposorgente"), CStr(tiposorgente))

        'Restituisco in uscita 
        Return XML_Parametri

    End Function

    Private Function elencoStazioniXMLtoDatatable(Str_XML_Risultato As String) As DataTable

        Dim dt_stazioni As New DataTable
        Dim XmlDoc As New XmlDocument
        Dim XML_Risultato As XmlElement

        'Carico la stringa nel documento XML
        XmlDoc.LoadXml(Str_XML_Risultato)

        XML_Risultato = XmlDoc.SelectSingleNode("RISULTATO")

        Dim Codice_Errore As String = ""
        Codice_Errore = XML_Risultato.GetAttribute("errcod")
        Dim Messaggio_Errore = XML_Risultato.GetAttribute("errmsg")

        If Codice_Errore = "" Or Codice_Errore = "0" Then

            Dim XMLs_NodiDato As XmlNodeList

            XMLs_NodiDato = XmlDoc.GetElementsByTagName("DATO")

            Dim XML_Dato As XmlElement
            Dim nome, descrizione As String
            Dim distanza, f_longitude, f_latitude As Decimal
            Dim data_ultimo_agg As Date
            Dim stazione_cod_fornitore As String = ""

            'Leggi i nodi dato e Carica il datagrid
            If Not IsNothing(XMLs_NodiDato) AndAlso XMLs_NodiDato.Count > 0 Then

                Try


                    Dim Dr_Stazioni As DataRow

                    '----- Definisco la struttura del DataTable
                    dt_stazioni.Columns.Add(New DataColumn("nome", GetType(String)))
                    dt_stazioni.Columns.Add(New DataColumn("descrizione", GetType(String)))
                    dt_stazioni.Columns.Add(New DataColumn("distanza", GetType(Decimal)))
                    dt_stazioni.Columns.Add(New DataColumn("f_longitude", GetType(Decimal)))
                    dt_stazioni.Columns.Add(New DataColumn("f_latitude", GetType(Decimal)))
                    dt_stazioni.Columns.Add(New DataColumn("data_ultimo_agg", GetType(Date)))
                    dt_stazioni.Columns.Add(New DataColumn("stazione_cod_fornitore", GetType(String)))

                    '-------------------------------------------------------------------

                    For i = 0 To XMLs_NodiDato.Count - 1



                        XML_Dato = XMLs_NodiDato.Item(i)

                        nome = XML_Dato.GetAttribute("nome")
                        descrizione = XML_Dato.GetAttribute("descrizione")
                        distanza = XML_Dato.GetAttribute("distanza")
                        f_longitude = XML_Dato.GetAttribute("f_longitude")
                        f_latitude = XML_Dato.GetAttribute("f_latitude")
                        data_ultimo_agg = CDate(XML_Dato.GetAttribute("data_ultimo_agg"))

                        Dr_Stazioni = dt_stazioni.NewRow


                        Dr_Stazioni.Item("nome") = nome
                        Dr_Stazioni.Item("descrizione") = descrizione
                        Dr_Stazioni.Item("distanza") = distanza
                        Dr_Stazioni.Item("f_longitude") = f_longitude
                        Dr_Stazioni.Item("f_latitude") = f_latitude
                        Dr_Stazioni.Item("data_ultimo_agg") = data_ultimo_agg

                        If XML_Dato.HasAttribute("stazione_cod_fornitore") Then
                            stazione_cod_fornitore = XML_Dato.GetAttribute("stazione_cod_fornitore")
                        End If
                        Dr_Stazioni.Item("stazione_cod_fornitore") = stazione_cod_fornitore


                        'Associo alla tabella la nuova riga creata
                        dt_stazioni.Rows.Add(Dr_Stazioni)

                    Next

                    Return dt_stazioni

                Catch ex As Exception
                    'messaggioErrore = 
                    Return Nothing
                End Try

            Else
                'messaggioErrore 
            End If


        End If
        Return Nothing
    End Function

    Public Function MaxIntervalloDatiMeteo(ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim hlpHttp As New Http
        Dim rval As String = ""
        Dim url As String = ""

        urlMeteo_Leggi(objParametri_Server, url)
        url &= "/ModelliPrevisionali_IntervalloDatiMeteo"

        rval = hlpHttp.chiamaWS("", Nothing, url, "application/json", "POST", "application/json", "")

        Return rval

    End Function

End Class
