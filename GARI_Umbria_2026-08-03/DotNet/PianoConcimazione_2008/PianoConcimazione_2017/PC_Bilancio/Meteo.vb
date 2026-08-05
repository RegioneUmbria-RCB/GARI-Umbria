Imports System.Xml
Imports AgronicaCoreGestioneRichieste

Public Class Meteo

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
                    ConfigurationManager.AppSettings("GiasOnline.WS_Meteo.Meteo").ToString(),
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
                                        ByVal Soglia As Double,
                                        ByVal OraRilievo As String,
                                        ByVal Numero_Quadrante As Integer,
                                        ByVal Numero_Stazione As Integer,
                                        ByVal Coordinata_X As Integer,
                                        ByVal Coordinata_Y As Integer,
                                        ByVal Data_DA As Date,
                                        ByVal Data_A As Date) As XmlElement

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
        Dim objAgroWebConfig As New AgroWebConfig()
        WS_Meteo.Url = objAgroWebConfig.GiasOnline_WS_Meteo_Meteo

        Return WS_Meteo.DatiPrecipitazioni(Str_XML_Parametri)


    End Function


    '##########################################################################################
    'Cripta la stringa xml dei parameri utilizzando la stessa codifica del ws dei fitofarmaci (modulo AgroWS)
    Public Function Cripta_XML_Parametri(ByVal Str_XML_Parametri As String) As String

        Dim Str_XML_Parametri_Crypt As String

        Str_XML_Parametri_Crypt = New AgronicaCoreWebService.AgroWs().AWS_Codifica_P(Str_XML_Parametri)

        Return Str_XML_Parametri_Crypt

    End Function



End Class


