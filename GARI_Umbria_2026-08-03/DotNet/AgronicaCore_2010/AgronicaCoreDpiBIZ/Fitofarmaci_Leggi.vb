Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreMetaSchemaDAL
Imports AgronicaCoreDataProvider.DataProviderExtensions



Public Class Fitofarmaci_Leggi
    Inherits AgronicaCoreDataProvider.DataProvider


    '###############################################################################################
    Public Enum enum_AWS_Xml
        Xml_CODIFICA = 1
        Xml_DECODIFICA = 2
    End Enum

    '###############################################################################
    'WS DEI FITOFARMACI
    'Ci mette una vita! Ci vorrebbe una funzione sul webservice che leggesse direttamente
    'il primo record delle dosi di quel formulato, prendesse l'unit di misura
    'e restituisse la sua conversione in litri o kg
    Public Sub Recupera_DoseEtichetta_da_FrCod(ByRef objServer As System.Web.HttpServerUtility, _
                                                ByRef objSession As System.Web.SessionState.HttpSessionState, _
                                                ByRef objPage As System.Web.UI.Page, _
                                                ByRef strErr As String, _
                                                ByRef Dose_Min As Decimal, _
                                                ByRef Dose_Max As Decimal, _
                                                ByRef Udm_Cod As Integer, _
                                                ByRef Udm_Des As String, _
                                                ByVal Fr_Cod As Integer)


        '-----------------------------------------------------------------------------------
        '--- DOSI REMOTE
        '-----------------------------------------------------------------------------------

        Dim XmlDoc As System.Xml.XmlDocument
        Dim XML_Credenziali As System.Xml.XmlElement
        Dim StrCredenziali As String
        Dim StrParametri As String

        'Dim XmlRisultati As System.Xml.XmlElement
        'Dim XmlFormulato As System.Xml.XmlElement
        'Dim XmlDatiDosi As System.Xml.XmlElement
        Dim XmlDose As System.Xml.XmlElement
        'Dim XmlsDosi As System.Xml.XmlNodeList

        Dim Parametri As String
        Dim Risultati As String
        Dim y As Integer
        'Dim i, j As Integer

        Dim objAgroWS As New AgronicaCoreWebService.AgroWs

        Try

            Dim ObjDownloadWs As WS_Fitofarmaci.AgroWS_Fitofarmaci
            Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
            objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(objSession("ASG_objParametri_Utenti"))
            ObjDownloadWs = New WS_Fitofarmaci.AgroWS_Fitofarmaci
            Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS

            Dim indirizzo_ws_fitofarmaci As String
            If Not IsNothing(Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Fitofarmaci.AgroWS_Fitofarmaci")) Then
                indirizzo_ws_fitofarmaci = Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Fitofarmaci.AgroWS_Fitofarmaci").ToString()
            Else
                'lo carico dagli objwebconfig
                Dim objagrowebconfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
                indirizzo_ws_fitofarmaci = objagrowebconfig.GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci
            End If


            objWs.NewWS(ObjDownloadWs, _
                            indirizzo_ws_fitofarmaci, _
                            objParametri_Utenti)


            XmlDoc = New System.Xml.XmlDocument

            objAgroWS.AgroWS_XML__Credenziali(objAgroWS.enum_AWS_Xml.Xml_CODIFICA, _
                                    StrCredenziali, _
                                    objAgroWS.enum_AWS_Schede.AWS_Fitofarmaci_Formulato_Completo, _
                                    objAgroWS.AgroWS_DoorKey(objAgroWS.enum_AWS_Schede.AWS_Fitofarmaci_Formulato_Completo), _
                                    objSession("ASG_ProgressivoGIAS"), _
                                    objSession("ASG_SuperUser_Username").ToString, _
                                    objSession("ASG_SuperUser_Password").ToString)

            XmlDoc.LoadXml(StrCredenziali)

            XML_Credenziali = XmlDoc.SelectSingleNode("CREDENZIALI")

            objAgroWS.AgroWS_XML_Parametri_Formulato_Completo(objAgroWS.enum_AWS_Xml.Xml_CODIFICA, _
                                                    StrParametri, _
                                                    Fr_Cod, _
                                                    strErr)

            If strErr = "" Then

                XML_Credenziali.InnerXml = StrParametri

                Parametri = XmlDoc.OuterXml

                Parametri = objAgroWS.AWS_Codifica_P(Parametri)

                '-----------------------------------------------------------------------------------
                'Chiamata al WebService dei Fitofarmaci
                Risultati = ObjDownloadWs.Formulato_Completo(Parametri)

                If Not Risultati Is Nothing Then

                    Risultati = objAgroWS.AWS_Decodifica_R(Risultati)
                    Risultati = Risultati.Replace(">", ">" & vbCrLf)

                    '-----------------------------------------------------------------------------------
                    'Spacchetto l'XML 

                    XmlDoc.LoadXml(Risultati)

                    'XmlRisultati = XmlDoc.SelectSingleNode("RISULTATI")

                    'XmlFormulato = XmlRisultati.SelectSingleNode("FORMULATO")

                    '-----------------------------------------------------------------------------------
                    '----- DOSI 

                    'XmlDatiDosi = XmlAvversita.SelectSingleNode( "DATIDOSI")

                    'XmlsDosi = XmlDatiDosi.GetElementsByTagName("DOSE")

                    XmlDose = XmlDoc.SelectSingleNode("//DOSE")

                    If Not IsNothing(XmlDose) Then

                        'For y = 0 To XmlsDosi.Count - 1

                        'XmlDose = XmlsDosi.Item(y)

                        'Recupero le informazioni
                        'For_Veg_Av_Dos_Cod_A = CInt(XmlDose.GetAttribute(LCase("For_Veg_Av_Dos_Cod_A")))
                        'For_Veg_Av_Dos_Cod_I = CInt(XmlDose.GetAttribute(LCase("For_Veg_Av_Dos_Cod_I")))

                        If CDbl(XmlDose.GetAttribute(LCase("Dose_Min_A"))) < 0 Then
                            Dose_Min = CDbl(XmlDose.GetAttribute(LCase("Dose_Min_I")))
                        Else
                            Dose_Min = CDbl(XmlDose.GetAttribute(LCase("Dose_Min_A")))
                        End If

                        If CDbl(XmlDose.GetAttribute(LCase("Dose_Max_A"))) < 0 Then
                            Dose_Max = CDbl(XmlDose.GetAttribute(LCase("Dose_Max_I")))
                        Else
                            Dose_Max = CDbl(XmlDose.GetAttribute(LCase("Dose_Max_A")))
                        End If

                        If CInt(XmlDose.GetAttribute(LCase("Udm_Cod_A"))) < 0 Then
                            Udm_Cod = CInt(XmlDose.GetAttribute(LCase("Udm_Cod_I")))
                        Else
                            Udm_Cod = CInt(XmlDose.GetAttribute(LCase("Udm_Cod_A")))
                        End If

                        If CStr(XmlDose.GetAttribute(LCase("Udm_Des_A"))) = "-1" Then
                            Udm_Des = CStr(XmlDose.GetAttribute(LCase("Udm_Des_I")))
                        Else
                            Udm_Des = CStr(XmlDose.GetAttribute(LCase("Udm_Des_A")))
                        End If

                        'Note_A = CStr(XmlDose.GetAttribute(LCase("Note_A")))
                        'Note_I = CStr(XmlDose.GetAttribute(LCase("Note_I")))

                        'Next

                    End If

                End If

            End If


        Catch ex As Exception

            strErr = ex.Message

        End Try


    End Sub


    '###############################################################################
    Public Function Formulati_SpecieVegetali_Infestanti_Dosi(ByVal Fr_Cod As Integer,
                                                            ByVal Veg_Cod As Integer,
                                                            ByVal Av_Cod As Integer,
                                                            ByVal Av_Gru As Integer,
                                                            ByVal grfi_cod As Integer,
                                                            ByVal Data As Date,
                                                            ByRef objSession As System.Web.SessionState.HttpSessionState) As DataTable

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XML_Credenziali As System.Xml.XmlElement
        Dim StrCredenziali As String
        Dim StrParametri As String
        Dim strErr As String
        Dim Parametri As String
        Dim Risultati As String
        Dim Testo As String
        Dim i As Integer
        Dim Flag_WS_Fitofarmaci_Remoto As Boolean

        Dim objAgroWS As New AgronicaCoreWebService.AgroWs

        Dim Dt_Dose As New DataTable

        Dim ObjDownloadWs As WS_Fitofarmaci.AgroWS_Fitofarmaci

        If Not IsNothing(Configuration.ConfigurationManager.AppSettings("Flag_WS_Fitofarmaci_Remoto")) Then
            Testo = Configuration.ConfigurationManager.AppSettings("Flag_WS_Fitofarmaci_Remoto").ToString
            If Testo = "" Then Testo = "true"
        Else
            Testo = "true"
        End If

        Flag_WS_Fitofarmaci_Remoto = CBool(Testo)

        '-----------------------------------------------------------------------------------
        '--- DOSI LOCALI
        '-----------------------------------------------------------------------------------
        If Flag_WS_Fitofarmaci_Remoto = False Then

        Else

            '-----------------------------------------------------------------------------------
            '--- DOSI REMOTE
            '-----------------------------------------------------------------------------------

            Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
            objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(objSession("ASG_objParametri_Utenti"))

            ObjDownloadWs = New WS_Fitofarmaci.AgroWS_Fitofarmaci
            Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS

            Dim indirizzo_ws_fitofarmaci As String
            If Not IsNothing(Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Fitofarmaci.AgroWS_Fitofarmaci")) Then
                indirizzo_ws_fitofarmaci = Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Fitofarmaci.AgroWS_Fitofarmaci").ToString()
            Else
                'lo carico dagli objwebconfig
                Dim objagrowebconfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
                indirizzo_ws_fitofarmaci = objagrowebconfig.GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci
            End If

            objWs.NewWS(ObjDownloadWs,
                            indirizzo_ws_fitofarmaci,
                            objParametri_Utenti)


            objAgroWS.AgroWS_XML__Credenziali(objAgroWS.enum_AWS_Xml.Xml_CODIFICA,
                            StrCredenziali,
                            objAgroWS.enum_AWS_Schede.AWS_Fitofarmaci_Formulati_SpecieVegetali_Infestanti_Dosi,
                            objAgroWS.AgroWS_DoorKey(objAgroWS.enum_AWS_Schede.AWS_Fitofarmaci_Formulati_SpecieVegetali_Infestanti_Dosi),
                            objSession("ASG_ProgressivoGIAS"),
                            objSession("ASG_SuperUser_Username").ToString,
                            objSession("ASG_SuperUser_Password").ToString)

            XmlDoc.LoadXml(StrCredenziali)

            XML_Credenziali = XmlDoc.SelectSingleNode("CREDENZIALI")

            objAgroWS.AgroWS_XML_Parametri_Formulati_SpecieVegetali_Infestanti_Dosi(objAgroWS.enum_AWS_Xml.Xml_CODIFICA,
                                                                         StrParametri,
                                                                         Fr_Cod.ToString,
                                                                         Veg_Cod.ToString,
                                                                         Av_Cod.ToString,
                                                                         Av_Gru.ToString,
                                                                         grfi_cod.ToString,
                                                                         "0", "0", "",
                                                                         Data,
                                                                         "0",
                                                                          strErr)

            XML_Credenziali.InnerXml = StrParametri

            Parametri = XmlDoc.OuterXml

            Parametri = objAgroWS.AWS_Codifica_P(Parametri)

            Risultati = ObjDownloadWs.Formulati_SpecieVegetali_Infestanti_Dosi(Parametri)

            Risultati = objAgroWS.AWS_Decodifica_R(Risultati)
            Risultati = Risultati.Replace(">", ">" & vbCrLf)

            objAgroWS.AgroWS_XML_Risultati_Formulati_SpecieVegetali_Infestanti_Dosi(objAgroWS.enum_AWS_Xml.Xml_DECODIFICA,
                                                                        Risultati,
                                                                        Dt_Dose,
                                                                        strErr)
            Return Dt_Dose

        End If

    End Function







    '###############################################################################
    Public Function Formulati_SpecieVegetali_Avversita_Dosi(ByVal Fr_Cod As Integer,
                                                            ByVal Veg_Cod As Integer,
                                                            ByVal Av_Cod As Integer,
                                                            ByVal Av_Gru As Integer,
                                                            ByVal grfi_cod As Integer,
                                                            ByVal Data As Date,
                                                            ByRef objSession As System.Web.SessionState.HttpSessionState) As DataTable

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XML_Credenziali As System.Xml.XmlElement
        Dim StrCredenziali As String
        Dim StrParametri As String
        Dim strErr As String
        Dim Parametri As String
        Dim Risultati As String
        Dim Testo As String
        Dim i As Integer
        Dim Flag_WS_Fitofarmaci_Remoto As Boolean

        Dim objAgroWS As New AgronicaCoreWebService.AgroWs

        Dim Dt_Dose As New DataTable

        Dim ObjDownloadWs As WS_Fitofarmaci.AgroWS_Fitofarmaci

        If Not IsNothing(Configuration.ConfigurationManager.AppSettings("Flag_WS_Fitofarmaci_Remoto")) Then
            Testo = Configuration.ConfigurationManager.AppSettings("Flag_WS_Fitofarmaci_Remoto").ToString
            If Testo = "" Then Testo = "true"
        Else
            Testo = "true"
        End If

        Flag_WS_Fitofarmaci_Remoto = CBool(Testo)

        '-----------------------------------------------------------------------------------
        '--- DOSI LOCALI
        '-----------------------------------------------------------------------------------
        If Flag_WS_Fitofarmaci_Remoto = False Then

        Else

            '-----------------------------------------------------------------------------------
            '--- DOSI REMOTE
            '-----------------------------------------------------------------------------------

            Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
            objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(objSession("ASG_objParametri_Utenti"))

            ObjDownloadWs = New WS_Fitofarmaci.AgroWS_Fitofarmaci
            Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS

            Dim indirizzo_ws_fitofarmaci As String
            If Not IsNothing(Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Fitofarmaci.AgroWS_Fitofarmaci")) Then
                indirizzo_ws_fitofarmaci = Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Fitofarmaci.AgroWS_Fitofarmaci").ToString()
            Else
                'lo carico dagli objwebconfig
                Dim objagrowebconfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
                indirizzo_ws_fitofarmaci = objagrowebconfig.GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci
            End If

            objWs.NewWS(ObjDownloadWs,
                            indirizzo_ws_fitofarmaci,
                            objParametri_Utenti)


            objAgroWS.AgroWS_XML__Credenziali(objAgroWS.enum_AWS_Xml.Xml_CODIFICA,
                            StrCredenziali,
                            objAgroWS.enum_AWS_Schede.AWS_Fitofarmaci_Formulati_SpecieVegetali_Avversita_Dosi,
                            objAgroWS.AgroWS_DoorKey(objAgroWS.enum_AWS_Schede.AWS_Fitofarmaci_Formulati_SpecieVegetali_Avversita_Dosi),
                            objSession("ASG_ProgressivoGIAS"),
                            objSession("ASG_SuperUser_Username").ToString,
                            objSession("ASG_SuperUser_Password").ToString)

            XmlDoc.LoadXml(StrCredenziali)

            XML_Credenziali = XmlDoc.SelectSingleNode("CREDENZIALI")

            objAgroWS.AgroWS_XML_Parametri_Formulati_SpecieVegetali_Avversita_Dosi(objAgroWS.enum_AWS_Xml.Xml_CODIFICA,
                                                                         StrParametri,
                                                                         Fr_Cod.ToString,
                                                                         Veg_Cod.ToString,
                                                                         Av_Cod.ToString,
                                                                         Av_Gru.ToString,
                                                                         grfi_cod.ToString,
                                                                         "0", "0", "",
                                                                         Data,
                                                                         "0",
                                                                          strErr)

            XML_Credenziali.InnerXml = StrParametri

            Parametri = XmlDoc.OuterXml

            Parametri = objAgroWS.AWS_Codifica_P(Parametri)

            Risultati = ObjDownloadWs.Formulati_SpecieVegetali_Avversita_Dosi(Parametri)

            Risultati = objAgroWS.AWS_Decodifica_R(Risultati)
            Risultati = Risultati.Replace(">", ">" & vbCrLf)

            objAgroWS.AgroWS_XML_Risultati_Formulati_SpecieVegetali_Avversita_Dosi(objAgroWS.enum_AWS_Xml.Xml_DECODIFICA,
                                                                        Risultati,
                                                                        Dt_Dose,
                                                                        strErr)
            Return Dt_Dose

        End If

    End Function




    ''###############################################################################
    ''aggiungo il testo ricerca
    'Public Function Leggi_Infestanti_Da_Formulati_SpecieVegetali_Infestanti_3(ByVal Fr_Cod As Integer, _
    '                                                                        ByVal Veg_Cod As Integer, _
    '                                                                        ByVal Av_Cod As Integer, _
    '                                                                        ByVal Av_Gru As Integer, _
    '                                                                        ByVal Solo_Registrati As Integer, _
    '                                                                        ByVal Testo_Ricerca As String, _
    '                                                                        ByRef objSession As System.Web.SessionState.HttpSessionState) As DataTable

    '    Dim XmlDoc As New System.Xml.XmlDocument
    '    Dim XML_Credenziali As System.Xml.XmlElement
    '    Dim StrCredenziali As String
    '    Dim StrParametri As String
    '    Dim strErr As String
    '    Dim Parametri As String
    '    Dim Risultati As String
    '    Dim Testo As String
    '    Dim i As Integer
    '    Dim Flag_WS_Fitofarmaci_Remoto As Boolean

    '    Dim objAgroWS As New AgronicaCoreWebService.AgroWs

    '    Dim Dt_Avversita As New DataTable

    '    Dim ObjDownloadWs As WS_Fitofarmaci.AgroWS_Fitofarmaci

    '    If Not IsNothing(System.Configuration.ConfigurationSettings.AppSettings("Flag_WS_Fitofarmaci_Remoto")) Then
    '        Testo = System.Configuration.ConfigurationSettings.AppSettings("Flag_WS_Fitofarmaci_Remoto").ToString
    '        If Testo = "" Then Testo = "true"
    '    Else
    '        Testo = "true"
    '    End If

    '    Flag_WS_Fitofarmaci_Remoto = CBool(Testo)

    '    '-----------------------------------------------------------------------------------
    '    '--- DOSI LOCALI
    '    '-----------------------------------------------------------------------------------
    '    If Flag_WS_Fitofarmaci_Remoto = False Then

    '    Else

    '        '-----------------------------------------------------------------------------------
    '        '--- DOSI REMOTE
    '        '-----------------------------------------------------------------------------------

    '        Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
    '        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(objSession("ASG_objParametri_Utenti"))

    '        ObjDownloadWs = New WS_Fitofarmaci.AgroWS_Fitofarmaci
    '        Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS


    '        Dim indirizzo_ws_fitofarmaci As String
    '        'indirizzo_ws_fitofarmaci = System.Configuration.ConfigurationSettings.AppSettings("GiasOnline.WS_Fitofarmaci.AgroWS_Fitofarmaci").ToString()
    '        If Not IsNothing(System.Configuration.ConfigurationSettings.AppSettings("GiasOnline.WS_Fitofarmaci.AgroWS_Fitofarmaci")) Then
    '            indirizzo_ws_fitofarmaci = System.Configuration.ConfigurationSettings.AppSettings("GiasOnline.WS_Fitofarmaci.AgroWS_Fitofarmaci").ToString()
    '        Else
    '            'lo carico dagli objwebconfig
    '            Dim objagrowebconfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
    '            indirizzo_ws_fitofarmaci = objagrowebconfig.GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci
    '        End If


    '        objWs.NewWS(ObjDownloadWs, _
    '                        indirizzo_ws_fitofarmaci, _
    '                        objParametri_Utenti)

    '        objAgroWS.AgroWS_XML__Credenziali(objAgroWS.enum_AWS_Xml.Xml_CODIFICA, _
    '                                StrCredenziali, _
    '                                objAgroWS.enum_AWS_Schede.AWS_Fitofarmaci_Formulati_SpecieVegetali_Infestanti, _
    '                                objAgroWS.AgroWS_DoorKey(objAgroWS.enum_AWS_Schede.AWS_Fitofarmaci_Formulati_SpecieVegetali_Infestanti), _
    '                                objSession("ASG_ProgressivoGIAS"), _
    '                                objSession("ASG_SuperUser_Username").ToString, _
    '                                objSession("ASG_SuperUser_Password").ToString)

    '        XmlDoc.LoadXml(StrCredenziali)

    '        XML_Credenziali = XmlDoc.SelectSingleNode("CREDENZIALI")

    '        objAgroWS.AgroWS_XML_Parametri_Formulati_SpecieVegetali_Infestanti_3(objAgroWS.enum_AWS_Xml.Xml_CODIFICA, _
    '                                                                     StrParametri, _
    '                                                                     Fr_Cod.ToString, _
    '                                                                     Veg_Cod.ToString, _
    '                                                                     "0", _
    '                                                                     Av_Cod.ToString, _
    '                                                                     Av_Gru.ToString, _
    '                                                                     "0", _
    '                                                                     Solo_Registrati, _
    '                                                                     Testo_Ricerca, _
    '                                                                     "", _
    '                                                                     strErr)

    '        XML_Credenziali.InnerXml = StrParametri

    '        Parametri = XmlDoc.OuterXml

    '        Parametri = objAgroWS.AWS_Codifica_P(Parametri)

    '        Risultati = ObjDownloadWs.Leggi_Infestanti_Da_Formulati_SpecieVegetali_Infestanti_3(Parametri)

    '        Risultati = objAgroWS.AWS_Decodifica_R(Risultati)
    '        Risultati = Risultati.Replace(">", ">" & vbCrLf)

    '        objAgroWS.AgroWS_XML_Risultati_Leggi_Avversita_Da_Formulati_SpecieVegetali_Avversita(objAgroWS.enum_AWS_Xml.Xml_DECODIFICA, _
    '                                                                Risultati, _
    '                                                                Dt_Avversita, _
    '                                                                strErr)
    '        Return Dt_Avversita

    '    End If

    'End Function

    '###############################################################################
    'aggiungo la data
    Public Function Leggi_Infestanti_Da_Formulati_SpecieVegetali_Infestanti_4(ByVal Fr_Cod As Integer,
                                                                            ByVal Veg_Cod As Integer,
                                                                            ByVal Av_Cod As Integer,
                                                                            ByVal Av_Gru As Integer,
                                                                            ByVal Solo_Registrati As Integer,
                                                                            ByVal Testo_Ricerca As String,
                                                                            ByVal Data As Date,
                                                                            ByRef objSession As System.Web.SessionState.HttpSessionState) As DataTable

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XML_Credenziali As System.Xml.XmlElement
        Dim StrCredenziali As String
        Dim StrParametri As String
        Dim strErr As String
        Dim Parametri As String
        Dim Risultati As String
        Dim Testo As String
        Dim i As Integer
        Dim Flag_WS_Fitofarmaci_Remoto As Boolean

        Dim objAgroWS As New AgronicaCoreWebService.AgroWs

        Dim Dt_Avversita As New DataTable

        Dim ObjDownloadWs As WS_Fitofarmaci.AgroWS_Fitofarmaci

        If Not IsNothing(Configuration.ConfigurationManager.AppSettings("Flag_WS_Fitofarmaci_Remoto")) Then
            Testo = Configuration.ConfigurationManager.AppSettings("Flag_WS_Fitofarmaci_Remoto").ToString
            If Testo = "" Then Testo = "true"
        Else
            Testo = "true"
        End If

        Flag_WS_Fitofarmaci_Remoto = CBool(Testo)

        '-----------------------------------------------------------------------------------
        '--- DOSI LOCALI
        '-----------------------------------------------------------------------------------
        If Flag_WS_Fitofarmaci_Remoto = False Then

        Else

            '-----------------------------------------------------------------------------------
            '--- DOSI REMOTE
            '-----------------------------------------------------------------------------------

            Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
            objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(objSession("ASG_objParametri_Utenti"))

            ObjDownloadWs = New WS_Fitofarmaci.AgroWS_Fitofarmaci
            Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS


            Dim indirizzo_ws_fitofarmaci As String
            If Not IsNothing(Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Fitofarmaci.AgroWS_Fitofarmaci")) Then
                indirizzo_ws_fitofarmaci = Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Fitofarmaci.AgroWS_Fitofarmaci").ToString()
            Else
                'lo carico dagli objwebconfig
                Dim objagrowebconfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
                indirizzo_ws_fitofarmaci = objagrowebconfig.GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci
            End If


            objWs.NewWS(ObjDownloadWs,
                            indirizzo_ws_fitofarmaci,
                            objParametri_Utenti)

            objAgroWS.AgroWS_XML__Credenziali(objAgroWS.enum_AWS_Xml.Xml_CODIFICA,
                                    StrCredenziali,
                                    objAgroWS.enum_AWS_Schede.AWS_Fitofarmaci_Formulati_SpecieVegetali_Infestanti,
                                    objAgroWS.AgroWS_DoorKey(objAgroWS.enum_AWS_Schede.AWS_Fitofarmaci_Formulati_SpecieVegetali_Infestanti),
                                    objSession("ASG_ProgressivoGIAS"),
                                    objSession("ASG_SuperUser_Username").ToString,
                                    objSession("ASG_SuperUser_Password").ToString)

            XmlDoc.LoadXml(StrCredenziali)

            XML_Credenziali = XmlDoc.SelectSingleNode("CREDENZIALI")

            objAgroWS.AgroWS_XML_Parametri_Formulati_SpecieVegetali_Infestanti_3(objAgroWS.enum_AWS_Xml.Xml_CODIFICA,
                                                                         StrParametri,
                                                                         Fr_Cod.ToString,
                                                                         Veg_Cod.ToString,
                                                                         "0",
                                                                         Av_Cod.ToString,
                                                                         Av_Gru.ToString,
                                                                         "0",
                                                                         Solo_Registrati,
                                                                         Testo_Ricerca,
                                                                         Data.ToShortDateString,
                                                                         "", strErr)

            XML_Credenziali.InnerXml = StrParametri

            Parametri = XmlDoc.OuterXml

            Parametri = objAgroWS.AWS_Codifica_P(Parametri)

            Risultati = ObjDownloadWs.Leggi_Infestanti_Da_Formulati_SpecieVegetali_Infestanti_3(Parametri)

            Risultati = objAgroWS.AWS_Decodifica_R(Risultati)
            Risultati = Risultati.Replace(">", ">" & vbCrLf)

            objAgroWS.AgroWS_XML_Risultati_Leggi_Avversita_Da_Formulati_SpecieVegetali_Avversita(objAgroWS.enum_AWS_Xml.Xml_DECODIFICA,
                                                                    Risultati,
                                                                    Dt_Avversita,
                                                                    strErr)
            Return Dt_Avversita

        End If

    End Function

    ''###############################################################################
    ''aggiungo il testo ricerca
    'Public Function Leggi_Avversita_Da_Formulati_SpecieVegetali_Avversita_3(ByVal Fr_Cod As Integer, _
    '                                                                        ByVal Veg_Cod As Integer, _
    '                                                                        ByVal Av_Cod As Integer, _
    '                                                                        ByVal Av_Gru As Integer, _
    '                                                                        ByVal Solo_Registrati As Integer, _
    '                                                                        ByVal Testo_Ricerca As String, _
    '                                                                        ByRef objSession As System.Web.SessionState.HttpSessionState) As DataTable

    '    Dim XmlDoc As New System.Xml.XmlDocument
    '    Dim XML_Credenziali As System.Xml.XmlElement
    '    Dim StrCredenziali As String
    '    Dim StrParametri As String
    '    Dim strErr As String
    '    Dim Parametri As String
    '    Dim Risultati As String
    '    Dim Testo As String
    '    Dim i As Integer
    '    Dim Flag_WS_Fitofarmaci_Remoto As Boolean

    '    Dim objAgroWS As New AgronicaCoreWebService.AgroWs

    '    Dim Dt_Avversita As New DataTable

    '    Dim ObjDownloadWs As WS_Fitofarmaci.AgroWS_Fitofarmaci



    '    If Not IsNothing(System.Configuration.ConfigurationSettings.AppSettings("Flag_WS_Fitofarmaci_Remoto")) Then
    '        Testo = System.Configuration.ConfigurationSettings.AppSettings("Flag_WS_Fitofarmaci_Remoto").ToString
    '        If Testo = "" Then Testo = "true"
    '    Else
    '        Testo = "true"
    '    End If

    '    Flag_WS_Fitofarmaci_Remoto = CBool(Testo)

    '    '-----------------------------------------------------------------------------------
    '    '--- DOSI LOCALI
    '    '-----------------------------------------------------------------------------------
    '    If Flag_WS_Fitofarmaci_Remoto = False Then

    '    Else

    '        '-----------------------------------------------------------------------------------
    '        '--- DOSI REMOTE
    '        '-----------------------------------------------------------------------------------

    '        Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
    '        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(objSession("ASG_objParametri_Utenti"))

    '        ObjDownloadWs = New WS_Fitofarmaci.AgroWS_Fitofarmaci

    '        Dim indirizzo_ws_fitofarmaci As String
    '        'indirizzo_ws_fitofarmaci = System.Configuration.ConfigurationSettings.AppSettings("GiasOnline.WS_Fitofarmaci.AgroWS_Fitofarmaci").ToString()
    '        If Not IsNothing(System.Configuration.ConfigurationSettings.AppSettings("GiasOnline.WS_Fitofarmaci.AgroWS_Fitofarmaci")) Then
    '            indirizzo_ws_fitofarmaci = System.Configuration.ConfigurationSettings.AppSettings("GiasOnline.WS_Fitofarmaci.AgroWS_Fitofarmaci").ToString()
    '        Else
    '            'lo carico dagli objwebconfig
    '            Dim objagrowebconfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
    '            indirizzo_ws_fitofarmaci = objagrowebconfig.GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci
    '        End If


    '        Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS
    '        objWs.NewWS(ObjDownloadWs, _
    '                        indirizzo_ws_fitofarmaci, _
    '                        objParametri_Utenti)

    '        objAgroWS.AgroWS_XML__Credenziali(objAgroWS.enum_AWS_Xml.Xml_CODIFICA, _
    '                                StrCredenziali, _
    '                                objAgroWS.enum_AWS_Schede.AWS_Fitofarmaci_Formulati_SpecieVegetali_Avversita, _
    '                                objAgroWS.AgroWS_DoorKey(objAgroWS.enum_AWS_Schede.AWS_Fitofarmaci_Formulati_SpecieVegetali_Avversita), _
    '                                objSession("ASG_ProgressivoGIAS"), _
    '                                objSession("ASG_SuperUser_Username").ToString, _
    '                                objSession("ASG_SuperUser_Password").ToString)

    '        XmlDoc.LoadXml(StrCredenziali)

    '        XML_Credenziali = XmlDoc.SelectSingleNode("CREDENZIALI")

    '        objAgroWS.AgroWS_XML_Parametri_Formulati_SpecieVegetali_Avversita_3(objAgroWS.enum_AWS_Xml.Xml_CODIFICA, _
    '                                                                     StrParametri, _
    '                                                                     Fr_Cod.ToString, _
    '                                                                     Veg_Cod.ToString, _
    '                                                                     "0", _
    '                                                                     Av_Cod.ToString, _
    '                                                                     Av_Gru.ToString, _
    '                                                                     "0", _
    '                                                                     Solo_Registrati, _
    '                                                                     Testo_Ricerca, _
    '                                                                     strErr)

    '        XML_Credenziali.InnerXml = StrParametri

    '        Parametri = XmlDoc.OuterXml

    '        Parametri = objAgroWS.AWS_Codifica_P(Parametri)

    '        Risultati = ObjDownloadWs.Leggi_Avversita_Da_Formulati_SpecieVegetali_Avversita_3(Parametri)

    '        Risultati = objAgroWS.AWS_Decodifica_R(Risultati)
    '        Risultati = Risultati.Replace(">", ">" & vbCrLf)

    '        objAgroWS.AgroWS_XML_Risultati_Leggi_Avversita_Da_Formulati_SpecieVegetali_Avversita(objAgroWS.enum_AWS_Xml.Xml_DECODIFICA, _
    '                                                                Risultati, _
    '                                                                Dt_Avversita, _
    '                                                                strErr)
    '        Return Dt_Avversita

    '    End If

    'End Function

    '###############################################################################
    'aggiungo la data
    Public Function Leggi_Avversita_Da_Formulati_SpecieVegetali_Avversita_4(ByVal Fr_Cod As Integer,
                                                                            ByVal Veg_Cod As Integer,
                                                                            ByVal Av_Cod As Integer,
                                                                            ByVal Av_Gru As Integer,
                                                                            ByVal Solo_Registrati As Integer,
                                                                            ByVal Testo_Ricerca As String,
                                                                            ByVal Data As Date,
                                                                            ByRef objSession As System.Web.SessionState.HttpSessionState) As DataTable

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XML_Credenziali As System.Xml.XmlElement
        Dim StrCredenziali As String
        Dim StrParametri As String
        Dim strErr As String
        Dim Parametri As String
        Dim Risultati As String
        Dim Testo As String
        Dim i As Integer
        Dim Flag_WS_Fitofarmaci_Remoto As Boolean

        Dim objAgroWS As New AgronicaCoreWebService.AgroWs

        Dim Dt_Avversita As New DataTable

        Dim ObjDownloadWs As WS_Fitofarmaci.AgroWS_Fitofarmaci



        If Not IsNothing(Configuration.ConfigurationManager.AppSettings("Flag_WS_Fitofarmaci_Remoto")) Then
            Testo = Configuration.ConfigurationManager.AppSettings("Flag_WS_Fitofarmaci_Remoto").ToString
            If Testo = "" Then Testo = "true"
        Else
            Testo = "true"
        End If

        Flag_WS_Fitofarmaci_Remoto = CBool(Testo)

        '-----------------------------------------------------------------------------------
        '--- DOSI LOCALI
        '-----------------------------------------------------------------------------------
        If Flag_WS_Fitofarmaci_Remoto = False Then

        Else

            '-----------------------------------------------------------------------------------
            '--- DOSI REMOTE
            '-----------------------------------------------------------------------------------

            Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
            objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(objSession("ASG_objParametri_Utenti"))

            ObjDownloadWs = New WS_Fitofarmaci.AgroWS_Fitofarmaci

            Dim indirizzo_ws_fitofarmaci As String
            If Not IsNothing(Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Fitofarmaci.AgroWS_Fitofarmaci")) Then
                indirizzo_ws_fitofarmaci = Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Fitofarmaci.AgroWS_Fitofarmaci").ToString()
            Else
                'lo carico dagli objwebconfig
                Dim objagrowebconfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
                indirizzo_ws_fitofarmaci = objagrowebconfig.GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci
            End If


            Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS
            objWs.NewWS(ObjDownloadWs,
                            indirizzo_ws_fitofarmaci,
                            objParametri_Utenti)

            objAgroWS.AgroWS_XML__Credenziali(objAgroWS.enum_AWS_Xml.Xml_CODIFICA,
                                    StrCredenziali,
                                    objAgroWS.enum_AWS_Schede.AWS_Fitofarmaci_Formulati_SpecieVegetali_Avversita,
                                    objAgroWS.AgroWS_DoorKey(objAgroWS.enum_AWS_Schede.AWS_Fitofarmaci_Formulati_SpecieVegetali_Avversita),
                                    objSession("ASG_ProgressivoGIAS"),
                                    objSession("ASG_SuperUser_Username").ToString,
                                    objSession("ASG_SuperUser_Password").ToString)

            XmlDoc.LoadXml(StrCredenziali)

            XML_Credenziali = XmlDoc.SelectSingleNode("CREDENZIALI")

            objAgroWS.AgroWS_XML_Parametri_Formulati_SpecieVegetali_Avversita_3(objAgroWS.enum_AWS_Xml.Xml_CODIFICA,
                                                                         StrParametri,
                                                                         Fr_Cod.ToString,
                                                                         Veg_Cod.ToString,
                                                                         "0",
                                                                         Av_Cod.ToString,
                                                                         Av_Gru.ToString,
                                                                         "0",
                                                                         Solo_Registrati,
                                                                         Testo_Ricerca,
                                                                         Data.ToShortDateString,
                                                                         "", strErr)

            XML_Credenziali.InnerXml = StrParametri

            Parametri = XmlDoc.OuterXml

            Parametri = objAgroWS.AWS_Codifica_P(Parametri)

            Risultati = ObjDownloadWs.Leggi_Avversita_Da_Formulati_SpecieVegetali_Avversita_3(Parametri)

            Risultati = objAgroWS.AWS_Decodifica_R(Risultati)
            Risultati = Risultati.Replace(">", ">" & vbCrLf)

            objAgroWS.AgroWS_XML_Risultati_Leggi_Avversita_Da_Formulati_SpecieVegetali_Avversita(objAgroWS.enum_AWS_Xml.Xml_DECODIFICA,
                                                                    Risultati,
                                                                    Dt_Avversita,
                                                                    strErr)
            Return Dt_Avversita

        End If

    End Function


    ''###############################################################################
    'Public Function Leggi_Avversita_Da_Formulati_SpecieVegetali_Avversita_EscludidoppiaEsplosione(ByVal Fr_Cod As Integer, _
    '                                                                        ByVal Veg_Cod As Integer, _
    '                                                                        ByVal Av_Cod As Integer, _
    '                                                                        ByVal Av_Gru As Integer, _
    '                                                                        ByVal Solo_Registrati As Integer, _
    '                                                                        ByRef objSession As System.Web.SessionState.HttpSessionState) As DataTable

    '    Dim XmlDoc As New System.Xml.XmlDocument
    '    Dim XML_Credenziali As System.Xml.XmlElement
    '    Dim StrCredenziali As String
    '    Dim StrParametri As String
    '    Dim strErr As String
    '    Dim Parametri As String
    '    Dim Risultati As String
    '    Dim Testo As String
    '    Dim i As Integer
    '    Dim Flag_WS_Fitofarmaci_Remoto As Boolean

    '    Dim objAgroWS As New AgronicaCoreWebService.AgroWs

    '    Dim Dt_Avversita As New DataTable

    '    Dim ObjDownloadWs As WS_Fitofarmaci.AgroWS_Fitofarmaci



    '    If Not IsNothing(System.Configuration.ConfigurationSettings.AppSettings("Flag_WS_Fitofarmaci_Remoto")) Then
    '        Testo = System.Configuration.ConfigurationSettings.AppSettings("Flag_WS_Fitofarmaci_Remoto").ToString
    '        If Testo = "" Then Testo = "true"
    '    Else
    '        Testo = "true"
    '    End If

    '    Flag_WS_Fitofarmaci_Remoto = CBool(Testo)

    '    '-----------------------------------------------------------------------------------
    '    '--- DOSI LOCALI
    '    '-----------------------------------------------------------------------------------
    '    If Flag_WS_Fitofarmaci_Remoto = False Then

    '    Else

    '        '-----------------------------------------------------------------------------------
    '        '--- DOSI REMOTE
    '        '-----------------------------------------------------------------------------------

    '        Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
    '        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(objSession("ASG_objParametri_Utenti"))

    '        ObjDownloadWs = New WS_Fitofarmaci.AgroWS_Fitofarmaci

    '        Dim indirizzo_ws_fitofarmaci As String
    '        'indirizzo_ws_fitofarmaci = System.Configuration.ConfigurationSettings.AppSettings("GiasOnline.WS_Fitofarmaci.AgroWS_Fitofarmaci").ToString()
    '        If Not IsNothing(System.Configuration.ConfigurationSettings.AppSettings("GiasOnline.WS_Fitofarmaci.AgroWS_Fitofarmaci")) Then
    '            indirizzo_ws_fitofarmaci = System.Configuration.ConfigurationSettings.AppSettings("GiasOnline.WS_Fitofarmaci.AgroWS_Fitofarmaci").ToString()
    '        Else
    '            'lo carico dagli objwebconfig
    '            Dim objagrowebconfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
    '            indirizzo_ws_fitofarmaci = objagrowebconfig.GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci
    '        End If


    '        Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS
    '        objWs.NewWS(ObjDownloadWs, _
    '                        indirizzo_ws_fitofarmaci, _
    '                        objParametri_Utenti)

    '        objAgroWS.AgroWS_XML__Credenziali(objAgroWS.enum_AWS_Xml.Xml_CODIFICA, _
    '                                StrCredenziali, _
    '                                objAgroWS.enum_AWS_Schede.AWS_Fitofarmaci_Formulati_SpecieVegetali_Avversita, _
    '                                objAgroWS.AgroWS_DoorKey(objAgroWS.enum_AWS_Schede.AWS_Fitofarmaci_Formulati_SpecieVegetali_Avversita), _
    '                                objSession("ASG_ProgressivoGIAS"), _
    '                                objSession("ASG_SuperUser_Username").ToString, _
    '                                objSession("ASG_SuperUser_Password").ToString)

    '        XmlDoc.LoadXml(StrCredenziali)

    '        XML_Credenziali = XmlDoc.SelectSingleNode("CREDENZIALI")

    '        objAgroWS.AgroWS_XML_Parametri_Formulati_SpecieVegetali_Avversita_2(objAgroWS.enum_AWS_Xml.Xml_CODIFICA, _
    '                                                                     StrParametri, _
    '                                                                     Fr_Cod.ToString, _
    '                                                                     Veg_Cod.ToString, _
    '                                                                     "0", _
    '                                                                     Av_Cod.ToString, _
    '                                                                     Av_Gru.ToString, _
    '                                                                     "0", _
    '                                                                     Solo_Registrati, _
    '                                                                     strErr)

    '        XML_Credenziali.InnerXml = StrParametri

    '        Parametri = XmlDoc.OuterXml

    '        Parametri = objAgroWS.AWS_Codifica_P(Parametri)

    '        Risultati = ObjDownloadWs.Leggi_Avversita_Da_Formulati_SpecieVegetali_Avversita_EscludidoppiaEsplosione(Parametri)

    '        Risultati = objAgroWS.AWS_Decodifica_R(Risultati)
    '        Risultati = Risultati.Replace(">", ">" & vbCrLf)

    '        objAgroWS.AgroWS_XML_Risultati_Leggi_Avversita_Da_Formulati_SpecieVegetali_Avversita(objAgroWS.enum_AWS_Xml.Xml_DECODIFICA, _
    '                                                                Risultati, _
    '                                                                Dt_Avversita, _
    '                                                                strErr)
    '        Return Dt_Avversita

    '    End If

    'End Function




    '###############################################################################################
    Public Sub AgroWS_XML_Risultati_Formulato_Completo(
                                    ByVal StringaConnessione As String,
                                    ByVal Operazione As enum_AWS_Xml,
                                    ByVal Fr_Cod As Integer,
                                    ByRef StringaXML As String,
                                    ByRef Errore As String)

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlRisultati As System.Xml.XmlElement
        Dim XmlFormulato As System.Xml.XmlElement
        Dim XmlDatiSpecie As System.Xml.XmlElement
        Dim XmlSpecie As System.Xml.XmlElement
        Dim XmlDatiAvversita As System.Xml.XmlElement
        Dim XmlAvversita As System.Xml.XmlElement
        Dim XmlDatiDosi As System.Xml.XmlElement
        Dim XmlDose As System.Xml.XmlElement

        'Dim objSql As New Codex_Utility.Sql

        'Dim RsFormulato As ADODB.Recordset
        'Dim RsA As ADODB.Recordset
        'Dim RsD As ADODB.Recordset

        Dim DtFormulato As DataTable
        Dim DtA As DataTable
        Dim DtD As DataTable

        Dim strSpecie_Tot As String
        Dim strSpecie As String

        Dim strAvversita_Tot As String
        Dim strAvversita As String

        Dim Veg_Cod As Integer
        Dim Grsp_Cod As Integer

        Dim For_Veg_Cod As Integer

        Dim For_Veg_Av_Cod_Avv As Integer
        Dim For_Veg_Av_Cod_Inf As Integer

        Dim Av_Cod As Integer
        Dim Av_Des_Vol As String
        Dim Av_Gru As Integer
        Dim Av_Gru_Des As String

        Dim i, j, d As Integer

        '===========================================================================================
        '   <RISULTATI errore="...">
        '       <FORMULATO fr_cod="..." fr_des="..." data_reg="..." newcltoss_cod="..." ditta_cod="..."   
        '                  revocato="..." data_revo="..." sospeso="..." data_sosp="..." termine="..." data_term="..." 
        '                  data_fine_comm="..." data_fine_usoscorte="..." normeprecauzionali="..."  sempl_comp="..." />
        '             <DATISPECIE>
        '             <SPECIE for_veg_cod="..." veg_cod="..." veg_des="..." grsp_cod="..." grsp_des="..." note="..." tempocarenza="..." />
        '             <SPECIE for_veg_cod="..." veg_cod="..." veg_des="..." grsp_cod="..." grsp_des="..." note="..." tempocarenza="..." />
        '                   <DATIAVVERSITA>
        '                   <AVVERSITA For_Veg_Av_Cod_A="..." Av_Cod_A="..." Av_Gru_A="..." Av_Des_Vol_A="..." Av_Gru_Des_A="..."
        '                              For_Veg_Av_Cod_I="..." Av_Cod_I="..." Av_Gru_I="..." Av_Des_Vol_I="..." Av_Gru_Des_I="..." />
        '                   <AVVERSITA />
        '                           <DATIDOSI>
        '                           <DOSE For_Veg_Av_Dos_Cod_A="..." Dose_Min_A="..." Dose_Max_A="..." UDM_COD_A="..." Udm_Des_A="..." Note_A="..." 
        '                                 For_Veg_Av_Dos_Cod_I="..." Dose_Min_I="..." Dose_Max_I="..." UDM_COD_I="..." Udm_Des_I="..." Note_I="..." />
        '                           <DOSE />
        '   </RISULTATI>
        '===========================================================================================
        '   <RISULTATI errore="...">
        '       <DATI cod_disciplinare="..." fr_des="..." cltoss="... " pa_des="..." />
        '       <DATI cod_disciplinare="..." fr_des="..." cltoss="... " pa_des="..." />
        '       <DATI cod_disciplinare="..." fr_des="..." cltoss="... " pa_des="..." />
        '   </RISULTATI>
        '===========================================================================================

        Select Case Operazione

            Case enum_AWS_Xml.Xml_CODIFICA
                '///////////////////////////////////////////////
                '/// CODIFICA //////////////////////////////////
                '///////////////////////////////////////////////

                'Creo il nodo RISULTATI
                XmlRisultati = XmlDoc.CreateElement("RISULTATI")

                'Imposto gli attributi
                XmlRisultati.SetAttribute("errore", "")

                Dim StrSQL As String


                '--------------------------------
                '----- SPECIE VEGETALE
                '--------------------------------

                'Genero la query SQL
                StrSQL = ""
                StrSQL += " SELECT  Formulati.Fr_Cod, Formulati.Fr_Des, Formulati.Data_Reg, Formulati.NewCLTOSS_COD, Formulati.Ditta_Cod, "
                StrSQL += "         Formulati.Revocato, Formulati.Data_Revo, Formulati.Sospeso, Formulati.Data_Sosp, Formulati.Termine, Formulati.Data_Term, "
                StrSQL += "         Formulati.Data_Fine_Comm, Formulati.Data_Fine_UsoScorte, "
                StrSQL += "         Formulati.NormePrecauzionali, Formulati.Sempl_Comp, "
                StrSQL += "         FormulatixSpecieVegetali.For_Veg_Cod, "
                StrSQL += "         FormulatixSpecieVegetali.Veg_Cod, FormulatixSpecieVegetali.Grsp_Cod, "
                StrSQL += "         FormulatixSpecieVegetali.Note, FormulatixSpecieVegetali.TempoCarenza, "
                StrSQL += "         ISNULL(GruppoColturale.Grsp_Des,-1) AS Grsp_Des, ISNULL(SpecieVegetali.Veg_Des,-1) AS Veg_Des "

                StrSQL += " FROM    GruppoColturale RIGHT OUTER JOIN"
                StrSQL += "         Formulati INNER JOIN"
                StrSQL += "         FormulatixSpecieVegetali ON Formulati.Fr_Cod = FormulatixSpecieVegetali.Fr_Cod ON "
                StrSQL += "         GruppoColturale.Grsp_Cod = FormulatixSpecieVegetali.Grsp_Cod LEFT OUTER JOIN"
                StrSQL += "         SpecieVegetali ON FormulatixSpecieVegetali.Veg_Cod = SpecieVegetali.Veg_Cod"

                StrSQL += " WHERE   (Formulati.Fr_Cod = " & Fr_Cod & ") "
                StrSQL += " ORDER BY SpecieVegetali.Veg_Des, GruppoColturale.Grsp_Des "

                'Recupero il recordset

                DtFormulato = EseguiQuery_Lettura(StringaConnessione,
                                                         StrSQL,
                                                         "")

                If (Not IsNothing(DtFormulato)) Then

                    'Creo il nodo FORMULATO
                    XmlFormulato = XmlDoc.CreateElement("FORMULATO")

                    XmlRisultati.AppendChild(XmlFormulato)

                    'Imposto gli attributi
                    XmlFormulato.SetAttribute(LCase("Fr_Cod"), DtFormulato.Rows(0).Item("Fr_Cod").ToString)
                    XmlFormulato.SetAttribute(LCase("Fr_Des"), DtFormulato.Rows(0).Item("Fr_Des").ToString)
                    XmlFormulato.SetAttribute(LCase("Data_Reg"), DtFormulato.Rows(0).Item("data_reg").ToString)
                    XmlFormulato.SetAttribute(LCase("Newcltoss_Cod"), DtFormulato.Rows(0).Item("newcltoss_cod").ToString)
                    XmlFormulato.SetAttribute(LCase("Ditta_Cod"), DtFormulato.Rows(0).Item("ditta_cod").ToString)
                    XmlFormulato.SetAttribute(LCase("Revocato"), DtFormulato.Rows(0).Item("revocato").ToString)
                    XmlFormulato.SetAttribute(LCase("Data_Revo"), DtFormulato.Rows(0).Item("data_revo").ToString)
                    XmlFormulato.SetAttribute(LCase("Sospeso"), DtFormulato.Rows(0).Item("sospeso").ToString)
                    XmlFormulato.SetAttribute(LCase("Data_Sosp"), DtFormulato.Rows(0).Item("data_sosp").ToString)
                    XmlFormulato.SetAttribute(LCase("Termine"), DtFormulato.Rows(0).Item("termine").ToString)
                    XmlFormulato.SetAttribute(LCase("Data_Term"), DtFormulato.Rows(0).Item("data_term").ToString)
                    XmlFormulato.SetAttribute(LCase("Data_Fine_Comm"), DtFormulato.Rows(0).Item("Data_Fine_Comm").ToString)
                    XmlFormulato.SetAttribute(LCase("Data_Fine_UsoScorte"), DtFormulato.Rows(0).Item("Data_Fine_UsoScorte").ToString)
                    XmlFormulato.SetAttribute(LCase("NormePrecauzionali"), DtFormulato.Rows(0).Item("normeprecauzionali").ToString)
                    XmlFormulato.SetAttribute(LCase("Sempl_Comp"), DtFormulato.Rows(0).Item("sempl_comp").ToString)

                    XmlDatiSpecie = XmlDoc.CreateElement("DATISPECIE")

                    XmlFormulato.AppendChild(XmlDatiSpecie)

                    For i = 0 To DtFormulato.Rows.Count - 1

                        XmlSpecie = XmlDoc.CreateElement("SPECIE")

                        XmlDatiSpecie.AppendChild(XmlSpecie)

                        For_Veg_Cod = DtFormulato.Rows(i).Item("for_veg_cod")
                        Veg_Cod = DtFormulato.Rows(i).Item("veg_cod")
                        Grsp_Cod = DtFormulato.Rows(i).Item("grsp_cod")

                        XmlSpecie.SetAttribute(LCase("For_Veg_Cod"), DtFormulato.Rows(i).Item("for_veg_cod").ToString)
                        XmlSpecie.SetAttribute(LCase("Veg_Cod"), DtFormulato.Rows(i).Item("veg_cod").ToString)
                        XmlSpecie.SetAttribute(LCase("Veg_Des"), DtFormulato.Rows(i).Item("veg_des").ToString)
                        XmlSpecie.SetAttribute(LCase("Grsp_Cod"), DtFormulato.Rows(i).Item("grsp_cod").ToString)
                        XmlSpecie.SetAttribute(LCase("Grsp_Des"), DtFormulato.Rows(i).Item("grsp_des").ToString)
                        XmlSpecie.SetAttribute(LCase("Note"), DtFormulato.Rows(i).Item("note").ToString)
                        XmlSpecie.SetAttribute(LCase("TempoCarenza"), DtFormulato.Rows(i).Item("tempocarenza").ToString)

                        XmlDatiAvversita = XmlDoc.CreateElement("DATIAVVERSITA")

                        XmlSpecie.AppendChild(XmlDatiAvversita)

                        '--------------------------------
                        '----- AVVERSITA' - INFESTANTI
                        '--------------------------------

                        'Genero la query SQL
                        StrSQL = ""
                        StrSQL = StrSQL & " SELECT  FormulatixSpeciexAvversita.For_Veg_Av_Cod AS For_Veg_Av_Cod_Avv,  "
                        StrSQL = StrSQL & "		    0 AS  For_Veg_Av_Cod_Inf, "
                        StrSQL = StrSQL & "         FormulatixSpeciexAvversita.Av_Cod,  "
                        StrSQL = StrSQL & "         Avversita.Av_Des_Vol,  "
                        StrSQL = StrSQL & "         FormulatixSpeciexAvversita.Av_Gru,  "
                        StrSQL = StrSQL & "         GruppoAvversita.Av_Gru_Des "
                        StrSQL = StrSQL & " FROM    FormulatixSpeciexAvversita LEFT OUTER JOIN "
                        StrSQL = StrSQL & "         GruppoAvversita ON FormulatixSpeciexAvversita.Av_Gru = GruppoAvversita.Av_Gru LEFT OUTER JOIN "
                        StrSQL = StrSQL & "         Avversita ON FormulatixSpeciexAvversita.Av_Cod = Avversita.Av_Cod "
                        'StrSQL = StrSQL & " WHERE   (FormulatixSpeciexAvversita.For_Veg_Cod = " & For_Veg_Cod & ") "

                        StrSQL = StrSQL & " WHERE   (FormulatixSpeciexAvversita.Fr_Cod = " & Fr_Cod & ") "
                        StrSQL = StrSQL & " AND   (FormulatixSpeciexAvversita.Veg_Cod = " & Veg_Cod & ") "
                        StrSQL = StrSQL & " AND   (FormulatixSpeciexAvversita.Grsp_Cod = " & Grsp_Cod & ") "

                        StrSQL = StrSQL & " UNION "

                        StrSQL = StrSQL & " SELECT  0 AS  For_Veg_Av_Cod_Avv, "
                        StrSQL = StrSQL & "         FormulatixSpeciexInfestanti.For_Veg_Av_Cod AS  For_Veg_Av_Cod_Inf, "
                        StrSQL = StrSQL & "         FormulatixSpeciexInfestanti.Av_Cod, Avversita.Av_Des_Vol, "
                        StrSQL = StrSQL & "         FormulatixSpeciexInfestanti.Av_Gru, GruppoAvversita.Av_Gru_Des "
                        StrSQL = StrSQL & " FROM    FormulatixSpeciexInfestanti LEFT OUTER JOIN "
                        StrSQL = StrSQL & "         GruppoAvversita ON FormulatixSpeciexInfestanti.Av_Gru = GruppoAvversita.Av_Gru LEFT OUTER JOIN "
                        StrSQL = StrSQL & "         Avversita ON FormulatixSpeciexInfestanti.Av_Cod = Avversita.Av_Cod "
                        'StrSQL = StrSQL & " WHERE   (FormulatixSpeciexInfestanti.For_Veg_Cod = " & For_Veg_Cod & ") "
                        StrSQL = StrSQL & " WHERE   (FormulatixSpeciexInfestanti.Fr_Cod = " & Fr_Cod & ") "
                        StrSQL = StrSQL & " AND   (FormulatixSpeciexInfestanti.Veg_Cod = " & Veg_Cod & ") "
                        StrSQL = StrSQL & " AND   (FormulatixSpeciexInfestanti.Grsp_Cod = " & Grsp_Cod & ") "

                        StrSQL = StrSQL & " ORDER BY Avversita.Av_Des_Vol, GruppoAvversita.Av_Gru_Des "


                        'Recupero il recordset
                        DtA = EseguiQuery_Lettura(StringaConnessione,
                                               StrSQL,
                                               "")

                        If (Not IsNothing(DtA)) Then

                            '---------- Loop AVVERSITA' - INFESTANTI

                            For j = 0 To DtA.Rows.Count - 1

                                XmlAvversita = XmlDoc.CreateElement("AVVERSITA")

                                XmlDatiAvversita.AppendChild(XmlAvversita)

                                'Recupero le informazioni
                                For_Veg_Av_Cod_Avv = DtA.Rows(j).Item("For_Veg_Av_Cod_Avv")
                                For_Veg_Av_Cod_Inf = DtA.Rows(j).Item("For_Veg_Av_Cod_Inf")

                                Av_Cod = IIf(IsDBNull(DtA.Rows(j).Item("Av_Cod")), 0, DtA.Rows(j).Item("Av_Cod"))
                                Av_Des_Vol = IIf(IsDBNull(DtA.Rows(j).Item("Av_Des_Vol")), "", DtA.Rows(j).Item("Av_Des_Vol"))
                                Av_Gru = IIf(IsDBNull(DtA.Rows(j).Item("av_gru")), 0, DtA.Rows(j).Item("av_gru"))
                                Av_Gru_Des = IIf(IsDBNull(DtA.Rows(j).Item("Av_gru_Des")), "", DtA.Rows(j).Item("Av_gru_Des"))

                                XmlAvversita.SetAttribute(LCase("For_Veg_Av_Cod_A"), For_Veg_Av_Cod_Avv.ToString)
                                XmlAvversita.SetAttribute(LCase("For_Veg_Av_Cod_I"), For_Veg_Av_Cod_Inf.ToString)

                                If For_Veg_Av_Cod_Inf = 0 Then

                                    XmlAvversita.SetAttribute(LCase("Av_Cod_A"), Av_Cod.ToString)
                                    XmlAvversita.SetAttribute(LCase("Av_Gru_A"), Av_Gru.ToString)
                                    XmlAvversita.SetAttribute(LCase("Av_Des_Vol_A"), Av_Des_Vol.ToString)
                                    XmlAvversita.SetAttribute(LCase("Av_Gru_Des_A"), Av_Gru_Des.ToString)

                                    XmlAvversita.SetAttribute(LCase("Av_Cod_I"), "-1")
                                    XmlAvversita.SetAttribute(LCase("Av_Gru_I"), "-1")
                                    XmlAvversita.SetAttribute(LCase("Av_Des_Vol_I"), "-1")
                                    XmlAvversita.SetAttribute(LCase("Av_Gru_Des_I"), "-1")

                                ElseIf For_Veg_Av_Cod_Avv = 0 Then

                                    XmlAvversita.SetAttribute(LCase("Av_Cod_A"), "-1")
                                    XmlAvversita.SetAttribute(LCase("Av_Gru_A"), "-1")
                                    XmlAvversita.SetAttribute(LCase("Av_Des_Vol_A"), "-1")
                                    XmlAvversita.SetAttribute(LCase("Av_Gru_Des_A"), "-1")

                                    XmlAvversita.SetAttribute(LCase("Av_Cod_I"), Av_Cod.ToString)
                                    XmlAvversita.SetAttribute(LCase("Av_Gru_I"), Av_Gru.ToString)
                                    XmlAvversita.SetAttribute(LCase("Av_Des_Vol_I"), Av_Des_Vol.ToString)
                                    XmlAvversita.SetAttribute(LCase("Av_Gru_Des_I"), Av_Gru_Des.ToString)

                                End If


                                XmlDatiDosi = XmlDoc.CreateElement("DATIDOSI")

                                XmlAvversita.AppendChild(XmlDatiDosi)

                                '--------------------------------
                                '----- DOSI
                                '--------------------------------

                                'AVVERSITA'
                                If For_Veg_Av_Cod_Inf = 0 Then

                                    'Genero la query SQL
                                    StrSQL = ""
                                    StrSQL = StrSQL & " SELECT  For_Veg_Av_Dos_Cod,  "
                                    StrSQL = StrSQL & "         For_Veg_Av_Cod,  "
                                    StrSQL = StrSQL & "         Dose_Min, Dose_Max, "
                                    StrSQL = StrSQL & "         UDM_COD, Udm_Des, Note "
                                    StrSQL = StrSQL & " FROM    FormulatixSpeciexAvversitaxDosi "
                                    StrSQL = StrSQL & " WHERE   (For_Veg_Av_Cod = " & For_Veg_Av_Cod_Avv & ") "
                                    StrSQL = StrSQL & " ORDER BY Dose_Min, Dose_Max "

                                    'Recupero il recordset
                                    DtD = EseguiQuery_Lettura(StringaConnessione,
                                                        StrSQL,
                                                        "")

                                    If (Not IsNothing(DtD)) Then

                                        For d = 0 To DtD.Rows.Count - 1

                                            XmlDose = XmlDoc.CreateElement("DOSE")

                                            XmlDatiDosi.AppendChild(XmlDose)

                                            XmlDose.SetAttribute(LCase("For_Veg_Av_Dos_Cod_A"), DtD.Rows(d).Item("For_Veg_Av_Dos_Cod").ToString)
                                            XmlDose.SetAttribute(LCase("Dose_Min_A"), DtD.Rows(d).Item("Dose_Min").ToString)
                                            XmlDose.SetAttribute(LCase("Dose_Max_A"), DtD.Rows(d).Item("Dose_Max").ToString)
                                            XmlDose.SetAttribute(LCase("Udm_Cod_A"), DtD.Rows(d).Item("Udm_Cod").ToString)
                                            XmlDose.SetAttribute(LCase("Udm_Des_A"), DtD.Rows(d).Item("Udm_Des").ToString)
                                            XmlDose.SetAttribute(LCase("Note_A"), DtD.Rows(d).Item("Note").ToString)

                                            XmlDose.SetAttribute(LCase("For_Veg_Av_Dos_Cod_I"), "-1")
                                            XmlDose.SetAttribute(LCase("Dose_Min_I"), "-1")
                                            XmlDose.SetAttribute(LCase("Dose_Max_I"), "-1")
                                            XmlDose.SetAttribute(LCase("Udm_Cod_I"), "-1")
                                            XmlDose.SetAttribute(LCase("Udm_Des_I"), "-1")
                                            XmlDose.SetAttribute(LCase("Note_I"), "-1")

                                        Next

                                    End If


                                    'INFESTANTE
                                ElseIf For_Veg_Av_Cod_Avv = 0 Then


                                    'Genero la query SQL
                                    StrSQL = ""
                                    StrSQL = StrSQL & " SELECT  For_Veg_Av_Dos_Cod,  "
                                    StrSQL = StrSQL & "         For_Veg_Av_Cod,  "
                                    StrSQL = StrSQL & "         Dose_Min, Dose_Max, "
                                    StrSQL = StrSQL & "         UDM_COD, Udm_Des, Note "
                                    StrSQL = StrSQL & " FROM    FormulatixSpeciexInfestantixDosi "
                                    StrSQL = StrSQL & " WHERE   (For_Veg_Av_Cod = " & For_Veg_Av_Cod_Inf & ") "
                                    StrSQL = StrSQL & " ORDER BY Dose_Min, Dose_Max "

                                    DtD = EseguiQuery_Lettura(StringaConnessione,
                                                        StrSQL,
                                                        "")

                                    If Errore = "" Then

                                        If (Not IsNothing(DtD)) Then

                                            For d = 0 To DtD.Rows.Count - 1

                                                XmlDose = XmlDoc.CreateElement("DOSE")

                                                XmlDatiDosi.AppendChild(XmlDose)

                                                XmlDose.SetAttribute(LCase("For_Veg_Av_Dos_Cod_I"), DtD.Rows(d).Item("For_Veg_Av_Dos_Cod").ToString)
                                                XmlDose.SetAttribute(LCase("Dose_Min_I"), DtD.Rows(d).Item("Dose_Min").ToString)
                                                XmlDose.SetAttribute(LCase("Dose_Max_I"), DtD.Rows(d).Item("Dose_Max").ToString)
                                                XmlDose.SetAttribute(LCase("Udm_Cod_I"), DtD.Rows(d).Item("Udm_Cod").ToString)
                                                XmlDose.SetAttribute(LCase("Udm_Des_I"), DtD.Rows(d).Item("Udm_Des").ToString)
                                                XmlDose.SetAttribute(LCase("Note_I"), DtD.Rows(d).Item("Note").ToString)

                                                XmlDose.SetAttribute(LCase("For_Veg_Av_Dos_Cod_A"), "-1")
                                                XmlDose.SetAttribute(LCase("Dose_Min_A"), "-1")
                                                XmlDose.SetAttribute(LCase("Dose_Max_A"), "-1")
                                                XmlDose.SetAttribute(LCase("Udm_Cod_A"), "-1")
                                                XmlDose.SetAttribute(LCase("Udm_Des_A"), "-1")
                                                XmlDose.SetAttribute(LCase("Note_A"), "-1")

                                            Next


                                        End If

                                    End If

                                End If


                                strAvversita = XmlAvversita.OuterXml

                                strAvversita_Tot += strAvversita

                            Next

                        End If

                        strSpecie = XmlSpecie.OuterXml

                        strSpecie_Tot += strSpecie

                    Next

                    XmlDatiSpecie.InnerXml = strSpecie_Tot

                End If


                '-------------------------------------------------------------------------------------------

                'Imposto XmlRisultati come figlio del documento principale
                XmlDoc.AppendChild(XmlRisultati)

                'Restituisco in uscita la stringa creata
                StringaXML = XmlDoc.InnerXml

                'Distruggo gli oggetti
                XmlRisultati = Nothing
                XmlFormulato = Nothing
                XmlDoc = Nothing
                '///////////////////////////////////////////////



            Case enum_AWS_Xml.Xml_DECODIFICA
                '///////////////////////////////////////////////
                '/// DECODIFICA ////////////////////////////////
                '///////////////////////////////////////////////

                ''Carico la stringa XML nel documento
                'XmlDoc.LoadXml(StringaXML)

                ''Prelevo il nodo XmlParametri
                'XmlParametri = XmlDoc.SelectSingleNode("//PARAMETRI")

                ''Prelevo gli attributi
                'Fr_Cod = CStr(XmlParametri.GetAttribute("fr_cod"))
                'Fr_Des = CStr(XmlParametri.GetAttribute("fr_des"))
                'Class_Cod = CStr(XmlParametri.GetAttribute("class_cod"))
                'DataRilievo = CStr(XmlParametri.GetAttribute("datarilievo"))
                'Flag_SoloAttivi = CBool(XmlParametri.GetAttribute("flag_soloattivi"))

                ''Distruggo gli oggetti
                'XmlNodo = Nothing
                'XmlParametri = Nothing
                'XmlDoc = Nothing
                '///////////////////////////////////////////////

        End Select

    End Sub


    '###############################################################################################
    Public Function Risultati_Formulato_ListaDitte(
                                    ByVal StringaConnessione As String,
                                    ByVal Fr_Cod As String, ByRef Stato_Cod As String,
                                    ByRef Errore As String) As DataTable

        Dim DtFormulato As DataTable



        Dim stb As New Text.StringBuilder

        '--------------------------------
        '----- SPECIE VEGETALE
        '--------------------------------

        'Genero la query SQL

        stb.AppendLine(" SELECT fxd.FR_COD, fxd.TIPODITTA_COD, d.* ")

        stb.AppendLine(" FROM    Formulati f")
        If Stato_Cod <> "" Then
            stb.AppendLine(" INNER JOIN  FormulatixAmbitoEstero a ON f.Fr_Cod = a.Fr_Cod AND a.Stato_Cod ='" & Agro_SQL_SaveText(Stato_Cod) & "' ")
        End If

        stb.AppendLine(" inner join FormulatiXditte fxd ")
        stb.AppendLine("     on  f.Fr_Cod = fxd.Fr_cod ")
        stb.AppendLine(" inner Join Ditte d ")
        stb.AppendLine("     On d.Ditta_Cod = fxd.DITTA_COD")


        stb.AppendLine(" WHERE   f.Fr_Cod in (" & Agro_SQL_Save_Clausola_IN(Fr_Cod) & ") ")

        stb.AppendLine(" ORDER BY fxd.FR_COD, fxd.TIPODITTA_COD DESC ")

        'Recupero il recordset

        DtFormulato = EseguiQuery_Lettura(StringaConnessione,
                                                 stb.ToString,
                                                 "")
        DtFormulato.TableName = "FormulatixDitte"
        Return DtFormulato

    End Function

    '###############################################################################################
    Public Function Risultati_Formulato_Info(
                                    ByVal StringaConnessione As String,
                                    ByVal Fr_Cod As String, ByRef Stato_Cod As String,
                                    ByRef Errore As String) As DataTable

        Dim DtFormulato As DataTable
        Dim DtA As DataTable
        Dim DtD As DataTable


        Dim StrSQL As String

        '--------------------------------
        '----- SPECIE VEGETALE
        '--------------------------------

        'Genero la query SQL
        StrSQL = ""
        StrSQL += " SELECT Formulati.*, FormulatiXPeriodoSospensione.DataSospensioneDA , FormulatiXPeriodoSospensione.DataSospensioneA "

        StrSQL += " FROM    Formulati"
        If Stato_Cod <> "" Then
            StrSQL += " INNER JOIN  FormulatixAmbitoEstero ON Formulati.Fr_Cod = FormulatixAmbitoEstero.Fr_Cod AND FormulatixAmbitoEstero.Stato_Cod ='" & Agro_SQL_SaveText(Stato_Cod) & "' "
        End If

        StrSQL += " left join FormulatiXPeriodoSospensione on  Formulati.Fr_Cod = FormulatiXPeriodoSospensione.Fr_cod "

        StrSQL += " WHERE   Formulati.Fr_Cod in (" & Fr_Cod & ") "

        'Recupero il recordset

        DtFormulato = EseguiQuery_Lettura(StringaConnessione,
                                                 StrSQL,
                                                 "")
        DtFormulato.TableName = "formulati"
        Return DtFormulato

    End Function

    '###############################################################################################
    Public Sub Verifica_Udm_Formulato(ByVal Fr_Cod As Integer,
                                      ByRef Udm As enum_UnitaMisura,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        If Fr_Cod <> 0 And Udm = enum_UnitaMisura.Numero Then

            Dim StrSQL As New System.Text.StringBuilder
            StrSQL.AppendLine(" SELECT * FROM FormulatixClassificazioni ")
            StrSQL.AppendLine(" WHERE FormulatixClassificazioni.For_Cod = " & Agro_SQL_SaveNum(Fr_Cod) & " ")

            Dim dt = EseguiQuery_Lettura(objParametri.StringaConnessione, StrSQL.ToString(), "")

            If dt.Rows.Count > 0 Then
                If dt.Select("Class_Cod IN (" & String.Join(",", Classificazioni_Confusione_Disorientamento_Sessuale) & ") ").Length > 0 Then

                    'Per il tipo formulato "Confusione/Disorientamento Sessuale" l'UDM Numero deve essere convertita in N.diffusori
                    Udm = enum_UnitaMisura.Numero_Diffusori

                ElseIf dt.Select("Class_Cod IN (" & String.Join(",", Classificazioni_Installazione_Trappole_Catture_Massa) & ") ").Length > 0 Then

                    'Per il tipo formulato "Installazione Trappole/Catture di Massa" l'UDM Numero deve essere convertita in N.trappole
                    'Udm = enum_UnitaMisura.Numero_Trappole

                    'Giulia 05/06/2026: si è scelto che vengono uniformate le udm in entrambi i casi usando sempre numero diffusori (in modo da non dover toccare il qdca)
                    Udm = enum_UnitaMisura.Numero_Diffusori

                End If
            End If

        End If

    End Sub

    '###############################################################################################
    Public Function Risultati_Formulato_Info_DS(
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                    ByVal Fr_Cod As String,
                                    ByRef Errore As String,
                                    Optional ByRef Data As String = "",
                                    Optional ByRef Tipo As String = "",
                                    Optional ByRef Fr_Des As String = "",
                                    Optional ByRef Stato_Cod As String = "") As DataSet


        Dim DsFormulato As New DataSet

        Dim objFiltro As New AgronicaCoreMetaSchemaDAL.Formulati_R

        Dim StrSQL As New System.Text.StringBuilder


        StrSQL.Length = 0
        StrSQL.Append(" SELECT DISTINCT Formulati.*, FormulatiXPeriodoSospensione.DataSospensioneDA , FormulatiXPeriodoSospensione.DataSospensioneA ")

        If Tipo <> "" AndAlso IsNumeric(Tipo) = True Then
            StrSQL.Append(" ,CASE")
            StrSQL.Append("     WHEN FormulatixClassificazioni.CLASS_COD IN (602,613,617,1003) THEN 'True'")
            StrSQL.Append("     ELSE 'False'")
            StrSQL.Append("  END AS IsTrappolaFormulato")
        Else
            StrSQL.Append(" ,'False' AS IsTrappolaFormulato ")
        End If

        StrSQL.Append(" FROM    Formulati")

        If Stato_Cod <> "" Then
            StrSQL.Append(" INNER JOIN  FormulatixAmbitoEstero ON Formulati.Fr_Cod = FormulatixAmbitoEstero.Fr_Cod AND FormulatixAmbitoEstero.Stato_Cod ='" & Agro_SQL_SaveText(Stato_Cod) & "' " & vbCrLf)
        End If

        StrSQL.Append(" left join FormulatiXPeriodoSospensione on  Formulati.Fr_Cod = FormulatiXPeriodoSospensione.Fr_cod ")

        If Tipo <> "" AndAlso IsNumeric(Tipo) = True Then
            StrSQL.Append(" INNER JOIN FormulatixClassificazioni ON Formulati.Fr_Cod = FormulatixClassificazioni.For_Cod ")
        End If

        StrSQL.Append(" WHERE 1=1 ")

        If Fr_Cod <> "" Then
            StrSQL.Append(" AND  Formulati.Fr_Cod in (" & Agro_SQL_Save_Clausola_IN(Fr_Cod) & ") ")
        End If

        If Fr_Des <> "" Then
            StrSQL.Append(" AND  Formulati.Fr_Des LIKE '%" & Fr_Des & "%' ")
        End If

        objFiltro.Filtro_Formulato_Classificazione(Tipo, StrSQL)

        StrSQL.Append(" ORDER BY Fr_Des ")

        'Recupero il recordset
        Dim DtFormulato As DataTable
        DtFormulato = EseguiQuery_Lettura(objParametri.StringaConnessione,
                                          StrSQL.ToString,
                                          "")

        DtFormulato.TableName = "formulati"

        If Data <> "" Then

            DtFormulato = objFiltro.Filtra_Formulati_2(DtFormulato, CDate(Data), objParametri)

        End If

        DsFormulato.Tables.Add(DtFormulato)


        Return DsFormulato

    End Function

    Public Function Risultati_FormulatoClassificazioni_Info(
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                    ByVal Fr_Cod As String,
                                    ByRef Errore As String,
                                    Optional ByRef Data As String = "",
                                    Optional ByRef Tipo As String = "0",
                                    Optional ByRef FiltroFr_Des As String = "",
                                    Optional ByVal strFiltro As String = "",
                                    Optional ByVal FlagVisualizza_Commercio_Tutti_Revocati As Integer = 0,
                                        Optional ByVal Stato_Cod As String = "IT") As DataTable



        Dim StrSQL As New System.Text.StringBuilder


        StrSQL.Length = 0
        StrSQL.Append(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")


        'Creazione tabelle Temporanea ordinamento String_AGG
        StrSQL.Append("  Create Table #Ord (Fr_Cod Int, Pa_Cod Int, Pa_Des nvarchar(255), Titolo float, Peso float) " & vbCrLf)
        StrSQL.Append(" Insert  #Ord (Fr_Cod, Pa_Cod, Pa_Des, Titolo, Peso) " & vbCrLf)
        StrSQL.Append(" Select  FPA.fr_cod, FPA.Pa_Cod,  PA.Pa_Des, Titolo, Peso " & vbCrLf)
        StrSQL.Append(" From PrincipiAttivi PA inner Join FormulatixPrincipiAttivi FPA " & vbCrLf)
        StrSQL.Append(" On FPA.Pa_Cod = PA.Pa_Cod " & vbCrLf)
        StrSQL.Append(" Order by FPA.Fr_Cod, Titolo Desc " & vbCrLf)

        StrSQL.Append(" SELECT DISTINCT Formulati.*, FormulatiXPeriodoSospensione.DataSospensioneDA , FormulatiXPeriodoSospensione.DataSospensioneA, ClassificazioniFormulati.Class_Des ")

        '(05/08/2020) fede aggiunti dati sostanze attive
        StrSQL.Append(" ,ISNULL(PrincipiCodici.strPA_COD,'') AS strPA_COD, " & vbCrLf)
        StrSQL.Append(" ISNULL(PrincipiCodici.strPA_DES,'') AS strPA_DES, " & vbCrLf)
        StrSQL.Append(" ISNULL(PrincipiCodici.strTITOLI,'') AS strTITOLI, " & vbCrLf)
        StrSQL.Append(" ISNULL(PrincipiCodici.strPESI,'') AS strPESI " & vbCrLf)

        StrSQL.Append("  ,ISNULL(FormulatiXAllegati.Fr_Des_Prec,'') AS Fr_Des_Prec ")
        StrSQL.Append(" ,FormulatiXAllegati.DataSmaltimentoScorte ")
        StrSQL.Append(" ,ISNULL(FormulatiXAllegatinormative.IDRiga,0) AS FormulatiXAllegatiNormative_IDRiga ")
        StrSQL.Append("   ,FormulatiXAllegatinormative.DataAttoNormativo AS DataAttoNormativo ")

        'Gestione Prodotto Secco
        StrSQL.Append(" ,Case When Upper(isNull(FormulatiXFormulazioni.For_Ni_Cod, 0)) In ('DP', 'DS') Then 1 Else 0 End AS Polverulento ")


        StrSQL.Append(" FROM    Formulati")
        StrSQL.Append(" left join FormulatiXPeriodoSospensione on  Formulati.Fr_Cod = FormulatiXPeriodoSospensione.Fr_cod ")
        StrSQL.Append(" INNER JOIN FormulatixClassificazioni ON Formulati.Fr_Cod = FormulatixClassificazioni.For_Cod ")
        StrSQL.Append(" INNER JOIN  ClassificazioniFormulati ON FormulatixClassificazioni.Class_Cod = ClassificazioniFormulati.Class_Cod  ")

        If Stato_Cod <> "" Then
            StrSQL.Append(" INNER JOIN  FormulatixAmbitoEstero ON Formulati.Fr_Cod = FormulatixAmbitoEstero.Fr_Cod AND FormulatixAmbitoEstero.Stato_Cod ='" & Agro_SQL_SaveText(Stato_Cod) & "' " & vbCrLf)
        End If

        StrSQL.Append(" Left Join FormulatiXAllegatiNormative ON Formulati.Fr_Cod=FormulatiXAllegatiNormative.FOR_COD  ")
        StrSQL.Append(" Left Join FormulatiXAllegati ON Formulati.Fr_Cod=FormulatiXAllegati.FOR_COD  ")
        StrSQL.Append("  And FormulatiXAllegatinormative.FormulatiXAllegati_IDRiga=FormulatiXAllegati.IDRiga  ")

        '(05/08/2020) fede aggiunti dati sostanze attive
        StrSQL.Append("   LEFT JOIN ( " & vbCrLf)

        StrSQL.AppendLine(" 	Select Fr_Cod,      ")
        StrSQL.AppendLine("  String_AGG(Pa_cod, '|') as [strPA_COD],    ")
        StrSQL.AppendLine("  String_AGG(Pa_Des, '|') as [strPA_DES],     ")
        StrSQL.AppendLine("  String_AGG(Titolo, '|') as [strTITOLI],    ")
        StrSQL.AppendLine("  String_AGG(Peso, '|') as [strPESI]     ")
        StrSQL.AppendLine("  FROM ( Select #Ord.Fr_Cod, #Ord.Pa_Cod, #ord.Pa_Des, #Ord.Titolo, #Ord.Peso From #Ord " & vbCrLf)
        StrSQL.AppendLine("    	 ) [MainPrincipiCodici] Group By Fr_Cod " & vbCrLf)
        StrSQL.AppendLine("  ) ")
        StrSQL.Append("   PrincipiCodici on PrincipiCodici.FR_COD=Formulati.FR_COD    " & vbCrLf)

        StrSQL.Append("   Left Outer Join FormulatiXFormulazioni on (FormulatiXFormulazioni.FR_COD=Formulati.FR_COD )   ")

        StrSQL.Append(" WHERE 1=1 ")

        If Data <> "" Then
            StrSQL.Append("  AND (")
            StrSQL.Append("    Formulati.Data_Reg<=" & Agro_SQL_SaveDate(Data) & vbCrLf)
            StrSQL.Append("  OR ")
            StrSQL.Append("    FormulatiXAllegati.DataSmaltimentoScorte >= " & Agro_SQL_SaveDate(Data) & vbCrLf)
            StrSQL.Append("      )")
        End If

        If Fr_Cod <> "" Then
            StrSQL.Append(" AND  Formulati.Fr_Cod in (" & Agro_SQL_Save_Clausola_IN(Fr_Cod) & ") ")
        End If

        If FiltroFr_Des <> "" Then
            StrSQL.Append(" AND  Formulati.Fr_Des LIKE '%" & FiltroFr_Des & "%' ")
        End If

        If strFiltro <> "" Then
            If Not Trim(strFiltro).StartsWith("AND") Then
                StrSQL.Append(" AND " & strFiltro)
            Else
                StrSQL.Append(strFiltro)
            End If
        End If

        If IsNumeric(Tipo) = True Then

            Select Case CInt(Tipo)

                Case enum_TipoFormulato.Tutti

                    '(30/04/2015 fede) modifica filtro Tutti per escludere le trappole 
                    StrSQL.Append(" And  FormulatixClassificazioni.CLASS_COD IN  (101,102,103,104,105,106,107,108,109,110,300,1000,610,611,  200,201,202,203,  400,401,402,403,404,405,407,408,409,411,412,413,414,415,418,  608,  603,  607, 606, 500,501,502,503,504,505,506) ")

                    '=================================================================

                Case enum_TipoFormulato.Antiparassitari

                    StrSQL.Append(" And  FormulatixClassificazioni.CLASS_COD IN  (101,102,103,104,105,106,107,108,109,110,610,611,606) ")

                    '=================================================================

                Case enum_TipoFormulato.Diserbanti

                    StrSQL.Append(" And  FormulatixClassificazioni.CLASS_COD IN  (200,201,202,203) ")

                    '=================================================================

                Case enum_TipoFormulato.Fitoregolatori

                    StrSQL.Append(" And  FormulatixClassificazioni.CLASS_COD IN (400,401,402,403,404,405,407,408,409,411,412,413,414,415,418)  ")


                Case enum_TipoFormulato.Coadiuvanti

                    StrSQL.Append("  And  (FormulatixClassificazioni.CLASS_COD >= 500 AND ClassificazioniFormulati.CLASS_COD <= 506)  ")

                Case enum_TipoFormulato.Concianti

                    StrSQL.Append("  And  FormulatixClassificazioni.CLASS_COD IN (608)  ")

                Case enum_TipoFormulato.Disseccanti

                    StrSQL.Append("  And  FormulatixClassificazioni.CLASS_COD IN (603,609)  ")

                Case enum_TipoFormulato.Geodisinfestanti

                    StrSQL.Append("  And  FormulatixClassificazioni.CLASS_COD IN (607)  ")

                Case enum_TipoFormulato.Antiparassitari_Concianti

                    StrSQL.Append("  And  FormulatixClassificazioni.CLASS_COD IN  (101,102,103,104,105,106,107,108,109,110,610,611,608,606) ")

                Case enum_TipoFormulato.Diserbanti_Disseccanti

                    StrSQL.Append("  And  FormulatixClassificazioni.CLASS_COD IN  (200,201,202,203,603,609) ")

                Case enum_TipoFormulato.Antiparassitari_Geodisinfestanti

                    StrSQL.Append(" And  FormulatixClassificazioni.CLASS_COD IN  (101,102,103,104,105,106,107,108,109,110,610,611,607,606) ")

                Case enum_TipoFormulato.Corroboranti_Fisiofarmaci

                    StrSQL.Append("  And  FormulatixClassificazioni.CLASS_COD IN (300, 1000)  " & vbCrLf)

                Case enum_TipoFormulato.Antiparassitari_Concianti_Geodisinfestanti_Fitoregolatori_Coadiuvanti_CorroborantiFisiofarmaci

                    StrSQL.Append(" And  FormulatixClassificazioni.CLASS_COD IN  (101,102,103,104,105,106,107,108,109,110,610,611,   608,   607, 606,  400,401,402,403,404,405,407,408,409,411,412,413,414,415,418,   500,501,502,503,504,505,506,   300, 1000) " & vbCrLf)

                Case enum_TipoFormulato.ConfusioneSessuale

                    StrSQL.Append("  And  FormulatixClassificazioni.CLASS_COD = 614  " & vbCrLf)

                Case enum_TipoFormulato.DisorientamentoSessuale

                    StrSQL.Append("  And  FormulatixClassificazioni.CLASS_COD = 615  " & vbCrLf)

                Case enum_TipoFormulato.ConfusioneDisorientamentoSessuale

                    StrSQL.Append(" And  FormulatixClassificazioni.CLASS_COD IN  (614,615) " & vbCrLf)

                Case enum_TipoFormulato.InstallazioneTrappoleCattureMassa

                    StrSQL.Append(" And  FormulatixClassificazioni.CLASS_COD IN  (602,613,617,1003) " & vbCrLf)

            End Select

        End If


        StrSQL.Append(" ORDER BY Fr_Des, ")
        StrSQL.Append(" DataAttoNormativo desc " & vbCrLf)

        StrSQL.Append(" Drop Table #Ord ")

        'Recupero il recordset
        Dim Dt_App As DataTable
        Dt_App = EseguiQuery_Lettura(objParametri.StringaConnessione,
                                          StrSQL.ToString,
                                          "")

        Dim DT As DataTable
        '(12/11/2015) aggiunto filtro revoche
        Select Case FlagVisualizza_Commercio_Tutti_Revocati
            Case 0 'solo prodotti in commercio
                '(29/11/2019 fede) temporaneamente NON filtro per prodotti ESTERI
                If Stato_Cod = "IT" Then
                    Dim objFiltro As New AgronicaCoreMetaSchemaDAL.Formulati_R
                    DT = objFiltro.Filtra_Formulati_2(Dt_App, Data, objParametri)
                Else
                    DT = Dt_App
                End If
            Case 1 'prodotti commercio + revocati
                DT = Dt_App
            Case 2 'prodotti revocati
                DT = Dt_App
        End Select

        Return DT

    End Function



    Public Function Risultati_FormulatoClassificazioni_Info_XML_Path(
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                    ByVal Fr_Cod As String,
                                    ByRef Errore As String,
                                    Optional ByRef Data As String = "",
                                    Optional ByRef Tipo As String = "0",
                                    Optional ByRef FiltroFr_Des As String = "",
                                    Optional ByVal strFiltro As String = "",
                                    Optional ByVal FlagVisualizza_Commercio_Tutti_Revocati As Integer = 0,
                                        Optional ByVal Stato_Cod As String = "IT") As DataTable



        Dim StrSQL As New System.Text.StringBuilder


        StrSQL.Length = 0
        StrSQL.Append(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
        StrSQL.Append(" SELECT DISTINCT Formulati.*, FormulatiXPeriodoSospensione.DataSospensioneDA , FormulatiXPeriodoSospensione.DataSospensioneA, ClassificazioniFormulati.Class_Des ")

        '(05/08/2020) fede aggiunti dati sostanze attive
        StrSQL.Append(" ,ISNULL(PrincipiCodici.strPA_COD,'') AS strPA_COD, " & vbCrLf)
        StrSQL.Append(" ISNULL(PrincipiCodici.strPA_DES,'') AS strPA_DES, " & vbCrLf)
        StrSQL.Append(" ISNULL(PrincipiCodici.strTITOLI,'') AS strTITOLI, " & vbCrLf)
        StrSQL.Append(" ISNULL(PrincipiCodici.strPESI,'') AS strPESI " & vbCrLf)

        StrSQL.Append("  ,ISNULL(FormulatiXAllegati.Fr_Des_Prec,'') AS Fr_Des_Prec ")
        StrSQL.Append(" ,FormulatiXAllegati.DataSmaltimentoScorte ")
        StrSQL.Append(" ,ISNULL(FormulatiXAllegatinormative.IDRiga,0) AS FormulatiXAllegatiNormative_IDRiga ")
        StrSQL.Append("   ,FormulatiXAllegatinormative.DataAttoNormativo AS DataAttoNormativo ")

        'Gestione Prodotto Secco
        StrSQL.Append(" ,Case When Upper(isNull(FormulatiXFormulazioni.For_Ni_Cod, 0)) In ('DP', 'DS') Then 1 Else 0 End AS Polverulento ")


        StrSQL.Append(" FROM    Formulati")
        StrSQL.Append(" left join FormulatiXPeriodoSospensione on  Formulati.Fr_Cod = FormulatiXPeriodoSospensione.Fr_cod ")
        StrSQL.Append(" INNER JOIN FormulatixClassificazioni ON Formulati.Fr_Cod = FormulatixClassificazioni.For_Cod ")
        StrSQL.Append(" INNER JOIN  ClassificazioniFormulati ON FormulatixClassificazioni.Class_Cod = ClassificazioniFormulati.Class_Cod  ")

        If Stato_Cod <> "" Then
            StrSQL.Append(" INNER JOIN  FormulatixAmbitoEstero ON Formulati.Fr_Cod = FormulatixAmbitoEstero.Fr_Cod AND FormulatixAmbitoEstero.Stato_Cod ='" & Agro_SQL_SaveText(Stato_Cod) & "' " & vbCrLf)
        End If

        StrSQL.Append(" Left Join FormulatiXAllegatiNormative ON Formulati.Fr_Cod=FormulatiXAllegatiNormative.FOR_COD  ")
        StrSQL.Append(" Left Join FormulatiXAllegati ON Formulati.Fr_Cod=FormulatiXAllegati.FOR_COD  ")
        StrSQL.Append("  And FormulatiXAllegatinormative.FormulatiXAllegati_IDRiga=FormulatiXAllegati.IDRiga  ")

        '(05/08/2020) fede aggiunti dati sostanze attive
        StrSQL.Append("   LEFT JOIN ( " & vbCrLf)

        StrSQL.Append("   SELECT  MainPrincipiCodici.fr_cod,  " & vbCrLf)
        StrSQL.Append("   Left(MainPrincipiCodici.strPA_COD,Len(MainPrincipiCodici.strPA_COD)-1) As strPA_COD,  " & vbCrLf)
        StrSQL.Append("   Left(MainPrincipiCodici.strPA_DES,Len(MainPrincipiCodici.strPA_DES)-1) As strPA_DES,  " & vbCrLf)
        StrSQL.Append("   Left(MainPrincipiCodici.strTITOLI,Len(MainPrincipiCodici.strTITOLI)-1) As strTITOLI, " & vbCrLf)
        StrSQL.Append("   Left(MainPrincipiCodici.strPESI,Len(MainPrincipiCodici.strPESI)-1) As strPESI " & vbCrLf)

        StrSQL.Append("   FROM	(              Select distinct FPA2.FR_COD,  " & vbCrLf)
        StrSQL.Append("   		(Select CONVERT (nvarchar, FPA1.PA_COD)  + '|' AS [text()]   " & vbCrLf)
        StrSQL.Append("   	 From FormulatixPrincipiAttivi FPA1, PrincipiAttivi PA              " & vbCrLf)
        StrSQL.Append("   	 Where FPA1.FR_COD = FPA2.FR_COD                  " & vbCrLf)
        StrSQL.Append("   	 and FPA1.PA_COD=PA.PA_COD                  " & vbCrLf)
        StrSQL.Append("   	 ORDER BY FPA1.FR_COD, titolo desc                  " & vbCrLf)
        StrSQL.Append("   	 For XML PATH ('')                 ) [strPA_COD],             " & vbCrLf)

        StrSQL.Append("   		 (Select CONVERT (nvarchar, PA.PA_DES)  + '|' AS [text()]   " & vbCrLf)
        StrSQL.Append("   	  From FormulatixPrincipiAttivi FPA1, PrincipiAttivi PA            " & vbCrLf)
        StrSQL.Append("   	  Where FPA1.FR_COD = FPA2.FR_COD         " & vbCrLf)
        StrSQL.Append("   	  and FPA1.PA_COD=PA.PA_COD            " & vbCrLf)
        StrSQL.Append("   	  ORDER BY FPA1.FR_COD, titolo desc            " & vbCrLf)
        StrSQL.Append("   	  For XML PATH ('')                 ) [strPA_DES],   " & vbCrLf)

        StrSQL.Append("   		 (Select CONVERT (nvarchar, FPA1.Titolo)  + '|' AS [text()]     " & vbCrLf)
        StrSQL.Append("   	  From FormulatixPrincipiAttivi FPA1 " & vbCrLf)
        StrSQL.Append("   	  Where FPA1.FR_COD = FPA2.FR_COD     " & vbCrLf)
        StrSQL.Append("   	  ORDER BY FPA1.FR_COD , titolo desc " & vbCrLf)
        StrSQL.Append("   	  For XML PATH ('')                 ) [strTITOLI],	 " & vbCrLf)

        StrSQL.Append("   		 (Select CONVERT (nvarchar, FPA1.Peso)  + '|' AS [text()]     " & vbCrLf)
        StrSQL.Append("   	  From FormulatixPrincipiAttivi FPA1 " & vbCrLf)
        StrSQL.Append("   	  Where FPA1.FR_COD = FPA2.FR_COD     " & vbCrLf)
        StrSQL.Append("   	  ORDER BY FPA1.FR_COD , titolo desc " & vbCrLf)
        StrSQL.Append("   	  For XML PATH ('')                 ) [strPESI]	 " & vbCrLf)

        StrSQL.Append("   	  From FormulatixPrincipiAttivi FPA2        " & vbCrLf)

        StrSQL.Append("   	 ) [MainPrincipiCodici]          " & vbCrLf)
        StrSQL.Append("   )      " & vbCrLf)
        StrSQL.Append("   PrincipiCodici on PrincipiCodici.FR_COD=Formulati.FR_COD    " & vbCrLf)

        StrSQL.Append("   Left Outer Join FormulatiXFormulazioni on (FormulatiXFormulazioni.FR_COD=Formulati.FR_COD )   ")

        StrSQL.Append(" WHERE 1=1 ")

        If Data <> "" Then
            StrSQL.Append("  AND (")
            StrSQL.Append("    Formulati.Data_Reg<=" & Agro_SQL_SaveDate(Data) & vbCrLf)
            StrSQL.Append("  OR ")
            StrSQL.Append("    FormulatiXAllegati.DataSmaltimentoScorte >= " & Agro_SQL_SaveDate(Data) & vbCrLf)
            StrSQL.Append("      )")
        End If

        If Fr_Cod <> "" Then
            StrSQL.Append(" AND  Formulati.Fr_Cod in (" & Agro_SQL_Save_Clausola_IN(Fr_Cod) & ") ")
        End If

        If FiltroFr_Des <> "" Then
            StrSQL.Append(" AND  Formulati.Fr_Des LIKE '%" & FiltroFr_Des & "%' ")
        End If

        If strFiltro <> "" Then
            If Not Trim(strFiltro).StartsWith("AND") Then
                StrSQL.Append(" AND " & strFiltro)
            Else
                StrSQL.Append(strFiltro)
            End If
        End If

        If IsNumeric(Tipo) = True Then

            Select Case CInt(Tipo)

                Case enum_TipoFormulato.Tutti

                    '(30/04/2015 fede) modifica filtro Tutti per escludere le trappole 
                    StrSQL.Append(" And  FormulatixClassificazioni.CLASS_COD IN  (101,102,103,104,105,106,107,108,109,110,300,1000,610,611,  200,201,202,203,  400,401,402,403,404,405,407,408,409,411,412,413,414,415,418,  608,  603,  607, 606, 500,501,502,503,504,505,506) ")

                    '=================================================================

                Case enum_TipoFormulato.Antiparassitari

                    StrSQL.Append(" And  FormulatixClassificazioni.CLASS_COD IN  (101,102,103,104,105,106,107,108,109,110,610,611,606) ")

                    '=================================================================

                Case enum_TipoFormulato.Diserbanti

                    StrSQL.Append(" And  FormulatixClassificazioni.CLASS_COD IN  (200,201,202,203) ")

                    '=================================================================

                Case enum_TipoFormulato.Fitoregolatori

                    StrSQL.Append(" And  FormulatixClassificazioni.CLASS_COD IN (400,401,402,403,404,405,407,408,409,411,412,413,414,415,418)  ")


                Case enum_TipoFormulato.Coadiuvanti

                    StrSQL.Append("  And  (FormulatixClassificazioni.CLASS_COD >= 500 AND ClassificazioniFormulati.CLASS_COD <= 506)  ")

                Case enum_TipoFormulato.Concianti

                    StrSQL.Append("  And  FormulatixClassificazioni.CLASS_COD IN (608)  ")

                Case enum_TipoFormulato.Disseccanti

                    StrSQL.Append("  And  FormulatixClassificazioni.CLASS_COD IN (603,609)  ")

                Case enum_TipoFormulato.Geodisinfestanti

                    StrSQL.Append("  And  FormulatixClassificazioni.CLASS_COD IN (607)  ")

                Case enum_TipoFormulato.Antiparassitari_Concianti

                    StrSQL.Append("  And  FormulatixClassificazioni.CLASS_COD IN  (101,102,103,104,105,106,107,108,109,110,610,611,608,606) ")

                Case enum_TipoFormulato.Diserbanti_Disseccanti

                    StrSQL.Append("  And  FormulatixClassificazioni.CLASS_COD IN  (200,201,202,203,603,609) ")

                Case enum_TipoFormulato.Antiparassitari_Geodisinfestanti

                    StrSQL.Append(" And  FormulatixClassificazioni.CLASS_COD IN  (101,102,103,104,105,106,107,108,109,110,610,611,607,606) ")

                Case enum_TipoFormulato.Corroboranti_Fisiofarmaci

                    StrSQL.Append("  And  FormulatixClassificazioni.CLASS_COD IN (300, 1000)  " & vbCrLf)

                Case enum_TipoFormulato.Antiparassitari_Concianti_Geodisinfestanti_Fitoregolatori_Coadiuvanti_CorroborantiFisiofarmaci

                    StrSQL.Append(" And  FormulatixClassificazioni.CLASS_COD IN  (101,102,103,104,105,106,107,108,109,110,610,611,   608,   607, 606,  400,401,402,403,404,405,407,408,409,411,412,413,414,415,418,   500,501,502,503,504,505,506,   300, 1000) " & vbCrLf)

                Case enum_TipoFormulato.ConfusioneSessuale

                    StrSQL.Append("  And  FormulatixClassificazioni.CLASS_COD = 614  " & vbCrLf)

                Case enum_TipoFormulato.DisorientamentoSessuale

                    StrSQL.Append("  And  FormulatixClassificazioni.CLASS_COD = 615  " & vbCrLf)

                Case enum_TipoFormulato.ConfusioneDisorientamentoSessuale

                    StrSQL.Append(" And  FormulatixClassificazioni.CLASS_COD IN  (614,615) " & vbCrLf)

                Case enum_TipoFormulato.InstallazioneTrappoleCattureMassa

                    StrSQL.Append(" And  FormulatixClassificazioni.CLASS_COD IN  (602,613,617,1003) " & vbCrLf)

            End Select

        End If


        StrSQL.Append(" ORDER BY Fr_Des, ")
        StrSQL.Append(" DataAttoNormativo desc ")

        'Recupero il recordset
        Dim Dt_App As DataTable
        Dt_App = EseguiQuery_Lettura(objParametri.StringaConnessione,
                                          StrSQL.ToString,
                                          "")

        Dim DT As DataTable
        '(12/11/2015) aggiunto filtro revoche
        Select Case FlagVisualizza_Commercio_Tutti_Revocati
            Case 0 'solo prodotti in commercio
                '(29/11/2019 fede) temporaneamente NON filtro per prodotti ESTERI
                If Stato_Cod = "IT" Then
                    Dim objFiltro As New AgronicaCoreMetaSchemaDAL.Formulati_R
                    DT = objFiltro.Filtra_Formulati_2(Dt_App, Data, objParametri)
                Else
                    DT = Dt_App
                End If
            Case 1 'prodotti commercio + revocati
                DT = Dt_App
            Case 2 'prodotti revocati
                DT = Dt_App
        End Select

        Return DT

    End Function


    '###############################################################################################
    '(29/11/2019 fede)
    'NOTA temporaneamente metto Default stato_cod = IT 
    'necessario perchè se diverso NON FILTRO in base a revoche, sospensione etc (non appplico Filtra_Formulati_2) 
    Public Function Risultati_Formulato_Info_con_UdM_DS(
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                    ByVal Fr_Cod As String,
                                    ByRef Errore As String,
                                    Optional ByRef Data As String = "",
                                    Optional ByRef Tipo As String = "",
                                    Optional ByRef Fr_Des As String = "",
                                    Optional ByRef Stato_Cod As String = "IT",
                                    Optional ByRef Veg_Cod As String = "0") As DataSet


        Dim DsFormulato As New DataSet
        Dim objFiltro As New AgronicaCoreMetaSchemaDAL.Formulati_R

        Dim StrSQL As New System.Text.StringBuilder

        StrSQL.Length = 0
        StrSQL.Append(" SELECT DISTINCT Formulati.*, FormulatiXPeriodoSospensione.DataSospensioneDA , FormulatiXPeriodoSospensione.DataSospensioneA, ")

        StrSQL.Append(" isnull((SELECT     TOP 1 FormulatixSpeciexAvversitaxDosi.UDM_COD ")
        StrSQL.Append("         from FormulatixSpeciexAvversita, FormulatixSpeciexAvversitaxDosi, UnitaMisura ")
        StrSQL.Append("         where FormulatixSpeciexAvversita.fr_cod = FORMULATI.fr_cod ")
        StrSQL.Append("         and FormulatixSpeciexAvversita.For_Veg_Av_Cod = FormulatixSpeciexAvversitaxDosi.For_Veg_Av_Cod ")
        StrSQL.Append("         and  FormulatixSpeciexAvversitaxDosi.UDM_COD = UnitaMisura.UDM_COD) ")
        StrSQL.Append(" ,0) as Udm_cod_A, ")
        StrSQL.Append(" isnull((SELECT     TOP 1 FormulatixSpeciexInfestantixDosi.UDM_COD ")
        StrSQL.Append("         from FormulatixSpeciexInfestanti, FormulatixSpeciexInfestantixDosi, UnitaMisura ")
        StrSQL.Append("         where FormulatixSpeciexInfestanti.fr_cod = FORMULATI.fr_cod ")
        StrSQL.Append("         and FormulatixSpeciexInfestanti.For_Veg_Av_Cod = FormulatixSpeciexInfestantixDosi.For_Veg_Av_Cod ")
        StrSQL.Append("         and  FormulatixSpeciexInfestantixDosi.UDM_COD = UnitaMisura.UDM_COD) ")
        StrSQL.Append(" ,0) as Udm_cod_I ")

        If Tipo <> "" Then
            StrSQL.Append(" ,CASE")
            StrSQL.Append("     WHEN FormulatixClassificazioni.CLASS_COD IN (602,613,617,1003) THEN 'True'")
            StrSQL.Append("     ELSE 'False'")
            StrSQL.Append("  END AS IsTrappolaFormulato")
        Else
            StrSQL.Append(" ,'False' AS IsTrappolaFormulato ")
        End If

        StrSQL.Append(" FROM    Formulati")

        If Stato_Cod <> "" Then
            StrSQL.Append(" INNER JOIN  FormulatixAmbitoEstero ON Formulati.Fr_Cod = FormulatixAmbitoEstero.Fr_Cod AND FormulatixAmbitoEstero.Stato_Cod ='" & Agro_SQL_SaveText(Stato_Cod) & "' " & vbCrLf)
        End If

        StrSQL.Append(" left join FormulatiXPeriodoSospensione on  Formulati.Fr_Cod = FormulatiXPeriodoSospensione.Fr_cod ")

        If Tipo <> "" Then
            StrSQL.Append(" INNER JOIN FormulatixClassificazioni ON Formulati.Fr_Cod = FormulatixClassificazioni.For_Cod ")
        End If

        If Not String.IsNullOrEmpty(Veg_Cod) AndAlso IsNumeric(Veg_Cod) AndAlso CInt(Veg_Cod) > 0 Then
            StrSQL.Append("   INNER JOIN FormulatixSpecieVegetalixNormative ON Formulati.Fr_Cod = FormulatixSpecieVegetalixNormative.Fr_Cod  " & vbCrLf)
        End If

        StrSQL.Append(" WHERE 1=1 ")


        If Fr_Cod <> "" Then
            StrSQL.Append(" AND  Formulati.Fr_Cod in (" & Agro_SQL_Save_Clausola_IN(Fr_Cod) & ") ")
        End If

        If Fr_Des <> "" Then
            StrSQL.Append(" AND  Formulati.Fr_Des LIKE '%" & Fr_Des & "%' ")
        End If

        If Not String.IsNullOrEmpty(Veg_Cod) AndAlso IsNumeric(Veg_Cod) AndAlso CInt(Veg_Cod) > 0 Then
            StrSQL.Append("  AND ( FormulatixSpecieVegetalixNormative.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod))
            'Per i geodisinfestanti aggiungo i prodotti registrati su 'terreno senza coltura'
            StrSQL.Append("      OR  FormulatixSpecieVegetalixNormative.Veg_Cod = 5000336 " & vbCrLf)

            StrSQL.Append("      OR  ( FormulatixSpecieVegetalixNormative.Grsp_Cod IN ( Select Grsp_Cod From GruppoColturaleXSpecieVegetali " & vbCrLf)
            StrSQL.Append("                                                   Where GruppoColturaleXSpecieVegetali.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ) ) ) " & vbCrLf)
        End If

        objFiltro.Filtro_Formulato_Classificazione(Tipo, StrSQL)

        StrSQL.Append(" ORDER BY Fr_Des ")

        Dim DtFormulato As DataTable
        DtFormulato = EseguiQuery_Lettura(objParametri.StringaConnessione,
                                          StrSQL.ToString,
                                          "")

        DtFormulato.TableName = "formulati"


        If Data <> "" Then

            '(29/11/2019 fede) temporaneamente NON filtro per prodotti ESTERI
            If Stato_Cod = "IT" Then
                DtFormulato = objFiltro.Filtra_Formulati_2(DtFormulato, CDate(Data), objParametri)
            End If

        End If

        DsFormulato.Tables.Add(DtFormulato)

        Return DsFormulato

    End Function

    '###############################################################################
    Public Function Formulati_PrincipiAttivi(ByVal Fr_Cod As Integer, ByVal Stato_Cod As String,
                                             ByRef objSession As System.Web.SessionState.HttpSessionState) As DataTable

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XML_Credenziali As System.Xml.XmlElement
        Dim StrCredenziali As String
        Dim StrParametri As String
        Dim strErr As String
        Dim Parametri As String
        Dim Risultati As String
        Dim Testo As String
        Dim i As Integer
        Dim Flag_WS_Fitofarmaci_Remoto As Boolean

        Dim objAgroWS As New AgronicaCoreWebService.AgroWs

        Dim Dt_PrincipiAttivi As New DataTable

        Dim ObjDownloadWs As WS_Fitofarmaci.AgroWS_Fitofarmaci

        If Not IsNothing(Configuration.ConfigurationManager.AppSettings("Flag_WS_Fitofarmaci_Remoto")) Then
            Testo = Configuration.ConfigurationManager.AppSettings("Flag_WS_Fitofarmaci_Remoto").ToString
            If Testo = "" Then Testo = "true"
        Else
            Testo = "true"
        End If

        Flag_WS_Fitofarmaci_Remoto = CBool(Testo)

        '-----------------------------------------------------------------------------------
        '--- DOSI LOCALI
        '-----------------------------------------------------------------------------------
        If Flag_WS_Fitofarmaci_Remoto = False Then

        Else

            '-----------------------------------------------------------------------------------
            '--- DOSI REMOTE
            '-----------------------------------------------------------------------------------

            Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
            objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(objSession("ASG_objParametri_Utenti"))

            ObjDownloadWs = New WS_Fitofarmaci.AgroWS_Fitofarmaci
            Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS

            Dim indirizzo_ws_fitofarmaci As String
            If Not IsNothing(Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Fitofarmaci.AgroWS_Fitofarmaci")) Then
                indirizzo_ws_fitofarmaci = Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Fitofarmaci.AgroWS_Fitofarmaci").ToString()
            Else
                'lo carico dagli objwebconfig
                Dim objagrowebconfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
                indirizzo_ws_fitofarmaci = objagrowebconfig.GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci
            End If

            objWs.NewWS(ObjDownloadWs,
                            indirizzo_ws_fitofarmaci,
                            objParametri_Utenti)


            objAgroWS.AgroWS_XML__Credenziali(objAgroWS.enum_AWS_Xml.Xml_CODIFICA,
                            StrCredenziali,
                            objAgroWS.enum_AWS_Schede.AWS_Fitofarmaci_Formulati_PrincipiAttivi,
                            objAgroWS.AgroWS_DoorKey(objAgroWS.enum_AWS_Schede.AWS_Fitofarmaci_Formulati_PrincipiAttivi),
                            objSession("ASG_ProgressivoGIAS"),
                            objSession("ASG_SuperUser_Username").ToString,
                            objSession("ASG_SuperUser_Password").ToString)

            XmlDoc.LoadXml(StrCredenziali)

            XML_Credenziali = XmlDoc.SelectSingleNode("CREDENZIALI")

            objAgroWS.AgroWS_XML_Parametri_Formulati_PrincipiAttivi(objAgroWS.enum_AWS_Xml.Xml_CODIFICA,
                                                                         StrParametri,
                                                                         Fr_Cod.ToString, Stato_Cod,
                                                                         strErr)

            XML_Credenziali.InnerXml = StrParametri

            Parametri = XmlDoc.OuterXml

            Parametri = objAgroWS.AWS_Codifica_P(Parametri)

            Risultati = ObjDownloadWs.Formulati_PrincipiAttivi(Parametri)

            Risultati = objAgroWS.AWS_Decodifica_R(Risultati)
            Risultati = Risultati.Replace(">", ">" & vbCrLf)

            objAgroWS.AgroWS_XML_Risultati_Formulati_PrincipiAttivi(objAgroWS.enum_AWS_Xml.Xml_DECODIFICA,
                                                                        Risultati,
                                                                        Dt_PrincipiAttivi,
                                                                        strErr)
            Return Dt_PrincipiAttivi

        End If

    End Function


    '###############################################################################
    Public Function Formulati_Bio(ByVal Fr_Cod As String, ByVal Stato_Cod As String,
                                    ByVal Progressivo_Gias As Integer,
                                    ByVal SuperUser_Password As String,
                                  ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XML_Credenziali As System.Xml.XmlElement
        Dim StrCredenziali As String
        Dim StrParametri As String
        Dim strErr As String
        Dim Parametri As String
        Dim Testo As String
        Dim i As Integer
        Dim Flag_WS_Fitofarmaci_Remoto As Boolean

        Dim objAgroWS As New AgronicaCoreWebService.AgroWs

        Dim Dt_PrincipiAttivi As New DataTable

        Dim ObjDownloadWs As WS_Fitofarmaci.AgroWS_Fitofarmaci

        If Not IsNothing(Configuration.ConfigurationManager.AppSettings("Flag_WS_Fitofarmaci_Remoto")) Then
            Testo = Configuration.ConfigurationManager.AppSettings("Flag_WS_Fitofarmaci_Remoto").ToString
            If Testo = "" Then Testo = "true"
        Else
            Testo = "true"
        End If

        Flag_WS_Fitofarmaci_Remoto = CBool(Testo)

        '-----------------------------------------------------------------------------------
        '--- DOSI LOCALI
        '-----------------------------------------------------------------------------------
        If Flag_WS_Fitofarmaci_Remoto = False Then

        Else

            ObjDownloadWs = New WS_Fitofarmaci.AgroWS_Fitofarmaci
            Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS

            Dim indirizzo_ws_fitofarmaci As String
            If Not IsNothing(Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Fitofarmaci.AgroWS_Fitofarmaci")) Then
                indirizzo_ws_fitofarmaci = Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Fitofarmaci.AgroWS_Fitofarmaci").ToString()
            Else
                'lo carico dagli objwebconfig
                Dim objagrowebconfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
                indirizzo_ws_fitofarmaci = objagrowebconfig.GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci
            End If

            objWs.NewWS(ObjDownloadWs,
                            indirizzo_ws_fitofarmaci,
                            objParametri_Utenti)


            objAgroWS.AgroWS_XML__Credenziali(objAgroWS.enum_AWS_Xml.Xml_CODIFICA,
                            StrCredenziali,
                            objAgroWS.enum_AWS_Schede.AWS_Fitofarmaci_Formulati_PrincipiAttivi,
                            objAgroWS.AgroWS_DoorKey(objAgroWS.enum_AWS_Schede.AWS_Fitofarmaci_Formulati_PrincipiAttivi),
                            Progressivo_Gias,
                            objParametri_Utenti.SuperUserUsername,
                            SuperUser_Password)

            XmlDoc.LoadXml(StrCredenziali)

            XML_Credenziali = XmlDoc.SelectSingleNode("CREDENZIALI")

            objAgroWS.AgroWS_XML_Parametri_Formulati_PrincipiAttivi(objAgroWS.enum_AWS_Xml.Xml_CODIFICA,
                                                                         StrParametri,
                                                                         Fr_Cod, Stato_Cod,
                                                                         strErr)

            XML_Credenziali.InnerXml = StrParametri

            Parametri = XmlDoc.OuterXml

            Parametri = objAgroWS.AWS_Codifica_P(Parametri)

            Dim strRisultati As String

            strRisultati = ObjDownloadWs.Formulati_Bio(Parametri, strErr)

            strRisultati = objAgroWS.AWS_Decodifica_R(strRisultati)
            strRisultati = strRisultati.Replace(">", ">" & vbCrLf)

            Dim DtRisultati As New DataTable

            objAgroWS.AgroWS_XML_Risultati_Formulati_Bio(objAgroWS.enum_AWS_Xml.Xml_DECODIFICA,
                                                                        strRisultati,
                                                                        DtRisultati,
                                                                        strErr)
            Return DtRisultati

        End If

    End Function

    '###############################################################################
    Public Function Formulati_PrincipiAttiviGruppiPrincipiAttivi(ByVal Fr_Cod As Integer, ByVal Stato_Cod As String,
                                                            ByRef objSession As System.Web.SessionState.HttpSessionState) As DataTable

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XML_Credenziali As System.Xml.XmlElement
        Dim StrCredenziali As String
        Dim StrParametri As String
        Dim strErr As String
        Dim Parametri As String
        Dim Risultati As String
        Dim Testo As String
        Dim i As Integer
        Dim Flag_WS_Fitofarmaci_Remoto As Boolean

        Dim objAgroWS As New AgronicaCoreWebService.AgroWs

        Dim Dt_PrincipiAttivi As New DataTable

        Dim ObjDownloadWs As WS_Fitofarmaci.AgroWS_Fitofarmaci

        If Not IsNothing(Configuration.ConfigurationManager.AppSettings("Flag_WS_Fitofarmaci_Remoto")) Then
            Testo = Configuration.ConfigurationManager.AppSettings("Flag_WS_Fitofarmaci_Remoto").ToString
            If Testo = "" Then Testo = "true"
        Else
            Testo = "true"
        End If

        Flag_WS_Fitofarmaci_Remoto = CBool(Testo)

        '-----------------------------------------------------------------------------------
        '--- DOSI LOCALI
        '-----------------------------------------------------------------------------------
        If Flag_WS_Fitofarmaci_Remoto = False Then

        Else

            '-----------------------------------------------------------------------------------
            '--- DOSI REMOTE
            '-----------------------------------------------------------------------------------

            Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
            objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(objSession("ASG_objParametri_Utenti"))

            ObjDownloadWs = New WS_Fitofarmaci.AgroWS_Fitofarmaci
            Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS

            Dim indirizzo_ws_fitofarmaci As String
            If Not IsNothing(Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Fitofarmaci.AgroWS_Fitofarmaci")) Then
                indirizzo_ws_fitofarmaci = Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Fitofarmaci.AgroWS_Fitofarmaci").ToString()
            Else
                'lo carico dagli objwebconfig
                Dim objagrowebconfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
                indirizzo_ws_fitofarmaci = objagrowebconfig.GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci
            End If



            objWs.NewWS(ObjDownloadWs,
                            indirizzo_ws_fitofarmaci,
                            objParametri_Utenti)


            objAgroWS.AgroWS_XML__Credenziali(objAgroWS.enum_AWS_Xml.Xml_CODIFICA,
                            StrCredenziali,
                            objAgroWS.enum_AWS_Schede.AWS_Fitofarmaci_Formulati_PrincipiAttivi,
                            objAgroWS.AgroWS_DoorKey(objAgroWS.enum_AWS_Schede.AWS_Fitofarmaci_Formulati_PrincipiAttivi),
                            objSession("ASG_ProgressivoGIAS"),
                            objSession("ASG_SuperUser_Username").ToString,
                            objSession("ASG_SuperUser_Password").ToString)

            XmlDoc.LoadXml(StrCredenziali)

            XML_Credenziali = XmlDoc.SelectSingleNode("CREDENZIALI")

            objAgroWS.AgroWS_XML_Parametri_Formulati_PrincipiAttivi(objAgroWS.enum_AWS_Xml.Xml_CODIFICA,
                                                                         StrParametri,
                                                                         Fr_Cod.ToString, Stato_Cod,
                                                                         strErr)

            XML_Credenziali.InnerXml = StrParametri

            Parametri = XmlDoc.OuterXml

            Parametri = objAgroWS.AWS_Codifica_P(Parametri)

            Risultati = ObjDownloadWs.Formulati_PrincipiAttiviGruppiPrincipiAttivi(Parametri)

            Risultati = objAgroWS.AWS_Decodifica_R(Risultati)
            Risultati = Risultati.Replace(">", ">" & vbCrLf)

            objAgroWS.AgroWS_XML_Risultati_Formulati_PrincipiAttiviGruppiPrincipiAttivi(objAgroWS.enum_AWS_Xml.Xml_DECODIFICA,
                                                                        Risultati,
                                                                        Dt_PrincipiAttivi,
                                                                        strErr)
            Return Dt_PrincipiAttivi

        End If

    End Function

    ''###############################################################################
    ''WS DEI FITOFARMACI
    Public Sub Recupera_UdM_da_FrCod(ByRef strErr As String,
                                    ByVal Fr_Cod As Integer,
                                    ByRef Udm_Cod As Integer,
                                    ByRef Udm_Sim As String,
                                    ByRef Udm_Des As String,
                                    ByRef objSession As System.Web.SessionState.HttpSessionState,
                                    Optional ByVal obj_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri = Nothing,
                                    Optional ByVal obj_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = Nothing,
                                    Optional ByVal obj_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = Nothing)


        '-----------------------------------------------------------------------------------
        '--- DOSI REMOTE
        '-----------------------------------------------------------------------------------

        Dim XmlDoc As System.Xml.XmlDocument
        Dim XML_Credenziali As System.Xml.XmlElement
        Dim StrCredenziali As String
        Dim StrParametri As String

        Dim XmlRisultati As System.Xml.XmlElement
        Dim XmlFormulato As System.Xml.XmlElement

        Dim Parametri As String
        Dim Risultati As String
        Dim y As Integer

        Dim objAgroWS As New AgronicaCoreWebService.AgroWs

        Try

            Dim ObjDownloadWs As WS_Fitofarmaci.AgroWS_Fitofarmaci
            Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri

            Dim ProgressivoGIAS As Integer = 0
            Dim SuperUser_Username As String = String.Empty
            Dim SuperUser_Password As String = String.Empty

            If Not IsNothing(objSession) Then

                objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(objSession("ASG_objParametri_Utenti"))
                ProgressivoGIAS = objSession("ASG_ProgressivoGIAS")
                SuperUser_Username = objSession("ASG_SuperUser_Username")
                SuperUser_Password = objSession("ASG_SuperUser_Password")

            ElseIf Not IsNothing(obj_Utenti) Then

                objParametri_Utenti = obj_Utenti

                SuperUser_Username = obj_Utenti.SuperUserUsername

                Dim dtUtente As DataTable
                Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Read
                dtUtente = objUtenti.Leggi_Superuser_e_ProgressivoGIAS(SuperUser_Username, "",
                                                                        Date.Now,
                                                                        CType(Now.Hour, Short),
                                                                        0, objParametri_Utenti)

                If Not dtUtente Is Nothing AndAlso dtUtente.Rows.Count > 0 Then
                    ProgressivoGIAS = dtUtente.Rows(0).Item("ProgressivoGIAS")
                    SuperUser_Password = dtUtente.Rows(0).Item("Password_SuperUser")
                End If
            End If

            ObjDownloadWs = New WS_Fitofarmaci.AgroWS_Fitofarmaci
            Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS

            Dim indirizzo_ws_fitofarmaci As String
            If Not IsNothing(Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Fitofarmaci.AgroWS_Fitofarmaci")) Then
                indirizzo_ws_fitofarmaci = Configuration.ConfigurationManager.AppSettings("GiasOnline.WS_Fitofarmaci.AgroWS_Fitofarmaci").ToString()
            Else

                If Not IsNothing(obj_Server) AndAlso Not IsNothing(obj_Super_Server) Then

                    Dim objConfigurazione_Siti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                    indirizzo_ws_fitofarmaci = objConfigurazione_Siti.Leggi_Valore(0, "GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci", "", "", obj_Server)
                    If indirizzo_ws_fitofarmaci = "" Then
                        indirizzo_ws_fitofarmaci = objConfigurazione_Siti.Leggi_Valore(0, "GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci", "", "", obj_Super_Server)
                    End If

                Else
                    'lo carico dagli objwebconfig
                    Dim objagrowebconfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
                    indirizzo_ws_fitofarmaci = objagrowebconfig.GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci
                End If

            End If


            objWs.NewWS(ObjDownloadWs,
                            indirizzo_ws_fitofarmaci,
                            objParametri_Utenti)


            XmlDoc = New System.Xml.XmlDocument

            objAgroWS.AgroWS_XML__Credenziali(objAgroWS.enum_AWS_Xml.Xml_CODIFICA,
                                StrCredenziali,
                                objAgroWS.enum_AWS_Schede.AWS_Fitofarmaci_Formulato_Completo,
                                objAgroWS.AgroWS_DoorKey(objAgroWS.enum_AWS_Schede.AWS_Fitofarmaci_Formulato_Completo),
                                ProgressivoGIAS,
                                SuperUser_Username,
                                SuperUser_Password)

            XmlDoc.LoadXml(StrCredenziali)

            XML_Credenziali = XmlDoc.SelectSingleNode("CREDENZIALI")

            objAgroWS.AgroWS_XML_Parametri_Formulato_Completo(objAgroWS.enum_AWS_Xml.Xml_CODIFICA,
                                                StrParametri,
                                                Fr_Cod,
                                                strErr)

            If strErr = "" Then

                XML_Credenziali.InnerXml = StrParametri

                Parametri = XmlDoc.OuterXml

                Parametri = objAgroWS.AWS_Codifica_P(Parametri)

                '-----------------------------------------------------------------------------------
                'Chiamata al WebService dei Fitofarmaci
                Risultati = ObjDownloadWs.Leggi_Formulato_UdM(Parametri)

                If Not Risultati Is Nothing Then

                    Risultati = objAgroWS.AWS_Decodifica_R(Risultati)
                    Risultati = Risultati.Replace(">", ">" & vbCrLf)

                    '-----------------------------------------------------------------------------------
                    'Spacchetto l'XML 

                    XmlDoc.LoadXml(Risultati)

                    XmlRisultati = XmlDoc.SelectSingleNode("RISULTATI")

                    XmlFormulato = XmlRisultati.SelectSingleNode("FORMULATO")

                    If Not IsNothing(XmlFormulato) Then

                        If CInt(XmlFormulato.GetAttribute(LCase("Udm_Cod"))) > 0 Then
                            Udm_Cod = CInt(XmlFormulato.GetAttribute(LCase("Udm_Cod")))
                        Else
                            Udm_Cod = 0
                        End If

                        If CStr(XmlFormulato.GetAttribute(LCase("Udm_Des"))) <> "-1" Then
                            Udm_Des = CStr(XmlFormulato.GetAttribute(LCase("Udm_Des")))
                        Else
                            Udm_Des = ""
                        End If

                        If CStr(XmlFormulato.GetAttribute(LCase("Udm_Sim"))) <> "-1" Then
                            Udm_Sim = CStr(XmlFormulato.GetAttribute(LCase("Udm_Sim")))
                        Else
                            Udm_Sim = ""
                        End If

                    End If

                End If

            End If


        Catch ex As Exception

            strErr = ex.Message

        End Try


    End Sub


    '###############################################################################
    'utilizzata da WS_Importa_Magazzino_2010: per importazioni di magazzino
    '---
    'a differenza della Recupera_UdM_da_FrCod vengono passati i parametri al posto della sessione
    'e l'eccezione viene ritornata
    Public Sub Recupera_UdM_da_FrCod_2(ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                       ByVal LinkWS_fitofarmaci As String, _
                                       ByVal Progressivo_GIAS As Integer, _
                                       ByVal SuperUser_Username As String, _
                                       ByVal SuperUser_Password As String, _
                                       ByVal Fr_Cod As Integer, _
                                        ByRef Udm_Cod As Integer, _
                                        ByRef Udm_Sim As String, _
                                        ByRef Udm_Des As String)

        Dim NomeRoutine As String = "AgronicaCoreDpiBIZ.Fitofarmaci_Leggi.Recupera_UdM_da_FrCod_2()"
        Dim MessaggioErrore As String = ""

        Dim XmlDoc As System.Xml.XmlDocument
        Dim XML_Credenziali As System.Xml.XmlElement
        Dim StrCredenziali As String
        Dim StrParametri As String

        Dim XmlRisultati As System.Xml.XmlElement
        Dim XmlFormulato As System.Xml.XmlElement

        Dim Parametri As String
        Dim Risultati As String
        Dim y As Integer
        Dim StrErr As String = ""
        Dim objAgroWS As New AgronicaCoreWebService.AgroWs

        Try

            Dim ObjDownloadWs As New WS_Fitofarmaci.AgroWS_Fitofarmaci
            Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS

            objWs.NewWS(ObjDownloadWs, _
                        LinkWS_fitofarmaci, _
                        objParametri_Utenti)

            XmlDoc = New System.Xml.XmlDocument

            objAgroWS.AgroWS_XML__Credenziali(objAgroWS.enum_AWS_Xml.Xml_CODIFICA, _
                                            StrCredenziali, _
                                            objAgroWS.enum_AWS_Schede.AWS_Fitofarmaci_Formulato_Completo, _
                                            objAgroWS.AgroWS_DoorKey(objAgroWS.enum_AWS_Schede.AWS_Fitofarmaci_Formulato_Completo), _
                                            Progressivo_GIAS, _
                                            SuperUser_Username, _
                                            SuperUser_Password)

            XmlDoc.LoadXml(StrCredenziali)

            XML_Credenziali = XmlDoc.SelectSingleNode("CREDENZIALI")

            objAgroWS.AgroWS_XML_Parametri_Formulato_Completo(objAgroWS.enum_AWS_Xml.Xml_CODIFICA, _
                                                                StrParametri, _
                                                                Fr_Cod, _
                                                                StrErr)

            If StrErr = "" Then

                XML_Credenziali.InnerXml = StrParametri

                Parametri = XmlDoc.OuterXml

                Parametri = objAgroWS.AWS_Codifica_P(Parametri)

                '-----------------------------------------------------------------------------------
                'Chiamata al WebService dei Fitofarmaci
                Risultati = ObjDownloadWs.Leggi_Formulato_UdM(Parametri)

                If Not Risultati Is Nothing Then

                    Risultati = objAgroWS.AWS_Decodifica_R(Risultati)
                    Risultati = Risultati.Replace(">", ">" & vbCrLf)

                    '-----------------------------------------------------------------------------------
                    'Spacchetto l'XML 

                    XmlDoc.LoadXml(Risultati)

                    XmlRisultati = XmlDoc.SelectSingleNode("RISULTATI")

                    XmlFormulato = XmlRisultati.SelectSingleNode("FORMULATO")

                    If Not IsNothing(XmlFormulato) Then

                        If CInt(XmlFormulato.GetAttribute(LCase("Udm_Cod"))) > 0 Then
                            Udm_Cod = CInt(XmlFormulato.GetAttribute(LCase("Udm_Cod")))
                        Else
                            Udm_Cod = 0
                        End If

                        If CStr(XmlFormulato.GetAttribute(LCase("Udm_Des"))) <> "-1" Then
                            Udm_Des = CStr(XmlFormulato.GetAttribute(LCase("Udm_Des")))
                        Else
                            Udm_Des = ""
                        End If

                        If CStr(XmlFormulato.GetAttribute(LCase("Udm_Sim"))) <> "-1" Then
                            Udm_Sim = CStr(XmlFormulato.GetAttribute(LCase("Udm_Sim")))
                        Else
                            Udm_Sim = ""
                        End If

                    End If

                End If
            Else
                Throw New Exception(StrErr)
            End If


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Utenti, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try


    End Sub


    '###############################################################################
    'utilizzata da WS_Importa_Magazzino_2010: per importazioni di magazzino
    '---
    Public Function Verifica_Formulato(ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                       ByVal LinkWS_fitofarmaci As String,
                                       ByVal Progressivo_GIAS As Integer,
                                       ByVal SuperUser_Username As String,
                                       ByVal SuperUser_Password As String,
                                       ByVal Fr_Cod As Integer,
                                       ByVal DataFiltroFormulati As String) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreDpiBIZ.Fitofarmaci_Leggi.Verifica_Formulato()"
        Dim MessaggioErrore As String = ""

        Dim XmlDoc As System.Xml.XmlDocument
        Dim XML_Credenziali As System.Xml.XmlElement
        Dim XmlRisultati As System.Xml.XmlElement
        Dim XmlFormulato As System.Xml.XmlElement

        Dim StrCredenziali As String
        Dim StrParametri As String
        Dim Parametri As String
        Dim Risultati As String
        Dim StrErr As String
        Dim LastFr_Des As String
        Dim Dt_Prodotti As DataTable

        Try

            Dim ObjDownloadWs As New WS_Fitofarmaci.AgroWS_Fitofarmaci
            Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS
            Dim objAgroWS As New AgronicaCoreWebService.AgroWs

            objWs.NewWS(ObjDownloadWs,
                        LinkWS_fitofarmaci,
                        objParametri_Utenti)

            XmlDoc = New System.Xml.XmlDocument

            objAgroWS.AgroWS_XML__Credenziali(objAgroWS.enum_AWS_Xml.Xml_CODIFICA,
                                            StrCredenziali,
                                            objAgroWS.enum_AWS_Schede.AWS_Fitofarmaci_Formulato_Completo,
                                            objAgroWS.AgroWS_DoorKey(objAgroWS.enum_AWS_Schede.AWS_Fitofarmaci_Formulato_Completo),
                                            Progressivo_GIAS,
                                            SuperUser_Username,
                                            SuperUser_Password)

            XmlDoc.LoadXml(StrCredenziali)

            XML_Credenziali = XmlDoc.SelectSingleNode("CREDENZIALI")

            Dim strFr_Cod As String = ""

            If Fr_Cod <> 0 Then
                strFr_Cod = Fr_Cod.ToString
            End If

            objAgroWS.AgroWS_XML_Parametri_Formulato_Completo(AgronicaCoreWebService.AgroWs.enum_AWS_Xml.Xml_CODIFICA,
                                                               StrParametri,
                                                               strFr_Cod,
                                                               StrErr,
                                                               DataFiltroFormulati,
                                                               "",
                                                               "")

            If StrErr <> "" Then
                Throw New Exception(StrErr)
            Else

                XML_Credenziali.InnerXml = StrParametri

                Parametri = XmlDoc.OuterXml

                Parametri = objAgroWS.AWS_Codifica_P(Parametri)

                Dim Ds As DataSet

                Ds = ObjDownloadWs.Leggi_Formulato_Info_con_UdM_DS(Parametri, StrErr)

                If StrErr <> "" Then
                    Throw New Exception(StrErr)
                Else

                    Dt_Prodotti = Ds.Tables("formulati")

                    ObjDownloadWs.Dispose()

                End If

            End If 'StrErr

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Utenti, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Dt_Prodotti

    End Function

End Class
