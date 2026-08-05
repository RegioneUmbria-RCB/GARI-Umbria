Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class SchedeMagazzinoHelper

    '#############################################################################################################
    Public Shared Function ComposizioneFormulatiRecupera(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                                        ByRef DT As DataTable, _
                                                        ByRef objServer As System.Web.HttpServerUtility, _
                                                        ByRef objSession As System.Web.SessionState.HttpSessionState, _
                                                        ByRef objPage As System.Web.UI.Page _
                                                        ) As DataTable

        Dim NomeRoutine As String = "SchedeMagazzinoHelper.ComposizioneFormulatiRecupera()"
        Dim MessaggioErrore As String = ""
        Dim DTfor As DataTable

        Try

            Dim ElencoFormulati As String = ""
            Dim i As Integer = 0
            Dim FrCod As Integer

            'Recupero l'elenco dei formulati 
            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

                For i = 0 To DT.Rows.Count - 1
                    FrCod = DT.Rows(i).Item("Pro_Cod")
                    'Se l'elemento e' un formulato
                    If DT.Rows(i).Item("Elem_Cod") = FORMULATI Then
                        If i = 0 Then
                            ElencoFormulati = ","
                        End If
                        'Se l'elemento non e' gia' presente dentro la stringa ...
                        If InStr(1, ElencoFormulati, "," & FrCod.ToString & ",", CompareMethod.Text) = 0 Then
                            ElencoFormulati += FrCod.ToString & ","
                        End If
                    End If
                Next

                If ElencoFormulati <> "" Then

                    'Tolgo il primo e ultimo carattere (,) ... 
                    If ElencoFormulati.Length > 0 Then
                        ElencoFormulati = Mid(ElencoFormulati, 2)
                        ElencoFormulati = Mid(ElencoFormulati, 1, ElencoFormulati.Length - 1)
                    End If

                    'Creo la stringa XML di richiesta

                    Dim XmlDoc As New System.Xml.XmlDocument
                    Dim XmlCredenziali As System.Xml.XmlElement
                    Dim XmlParametri As System.Xml.XmlElement
                    Dim XmlNodo As System.Xml.XmlElement
                    Dim StringaXML As String

                    Dim WsScheda As String
                    Dim WsDoorKey As String
                    Dim WsCodiceGias As String
                    Dim WsUsernameSuperuser As String
                    Dim WsPasswordSuperuser As String

                    '===========================================================================================
                    '   <CREDENZIALI scheda="..." doorkey= "..." codicegias="..." username="..." password="..." >
                    '       <PARAMETRI fr_cod=""                        oppure  "239,150" />
                    '   </CREDENZIALI>
                    '===========================================================================================

                    WsScheda = "1080"                                               'AWS_Fitofarmaci_Formulati_PrincipiAttivi
                    WsDoorKey = "portone_grth45ksh4ghajnl32149"
                    WsCodiceGias = objSession("ASG_ProgressivoGIAS")
                    WsUsernameSuperuser = objSession("ASG_SuperUser_Username")         'Session("ASG_SuperUser_Username_Crypt")
                    WsPasswordSuperuser = objSession("ASG_SuperUser_Password")         'Session("ASG_SuperUser_Password_Crypt")

                    'Creo il nodo CREDENZIALI
                    XmlCredenziali = XmlDoc.CreateElement("CREDENZIALI")

                    'Imposto gli attributi
                    XmlCredenziali.SetAttribute("scheda", CStr(WsScheda))
                    XmlCredenziali.SetAttribute("doorkey", CStr(WsDoorKey))
                    XmlCredenziali.SetAttribute("codicegias", CStr(WsCodiceGias))
                    XmlCredenziali.SetAttribute("username", CStr(WsUsernameSuperuser))
                    XmlCredenziali.SetAttribute("password", CStr(WsPasswordSuperuser))

                    'Imposto XmlParametri come figlio del documento principale
                    XmlDoc.AppendChild(XmlCredenziali)

                    'Creo il nodo PARAMETRI
                    XmlParametri = XmlDoc.CreateElement("PARAMETRI")

                    'Imposto gli attributi
                    XmlParametri.SetAttribute("fr_cod", ElencoFormulati)

                    'Imposto XmlParametri come figlio del documento principale
                    XmlCredenziali.AppendChild(XmlParametri)

                    'Restituisco in uscita la stringa creata
                    StringaXML = XmlDoc.InnerXml

                    'Distruggo gli oggetti
                    XmlNodo = Nothing
                    XmlParametri = Nothing
                    'XmlDoc = Nothing

                    '//////////////////////////////////////////////////////////
                    '/////   Chiamo la funzione del webservice per recuperare la composizione
                    '//////////////////////////////////////////////////////////

                    Dim LinkWsFitofarmaci As String
                    Dim WsRisposta As String = ""
                    Dim objCoreWebService As New AgronicaCoreWebService.AgroWs
                    StringaXML = objCoreWebService.AWS_Codifica_P(StringaXML)

                    Try
                        Dim WsFito As New WS_Fitofarmaci.AgroWS_Fitofarmaci

                        'l'indirizzo dei web service deve stare solo nella tabella 'configurazione_siti'
                        'eliminata chiave nel web.config
                        'Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig(objParametri_Server, False)
                        Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
                        If objWebConfig.GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci <> "" Then
                            LinkWsFitofarmaci = objWebConfig.GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci
                        End If

                        'If ConfigurationSettings.AppSettings("GiasOnline.WS_Fitofarmaci.AgroWS_Fitofarmaci").ToString = "" Then
                        '    LinkWsFitofarmaci = objServer.MapPath("https://ws.netagronica.it/AgronicaWebService/AgroWS_Fitofarmaci.asmx")
                        'Else
                        '    LinkWsFitofarmaci = ConfigurationSettings.AppSettings("GiasOnline.WS_Fitofarmaci.AgroWS_Fitofarmaci").ToString
                        'End If

                        WsFito.Url = LinkWsFitofarmaci
                        WsFito.Timeout = 60000
                        WsRisposta = WsFito.Formulati_PrincipiAttivi_2(StringaXML)

                    Catch ex As Exception
                        Throw New Exception("Webservice fitofarmaci : " & ex.Message)
                    End Try


                    '//////////////////////////////////////////////////////////
                    '/////   Decodifico il risultato ==> DataTable
                    '//////////////////////////////////////////////////////////

                    'Dim DTfor As New DataTable
                    DTfor = New DataTable
                    DTfor.Columns.Add(New DataColumn("Fr_Cod", GetType(Integer)))
                    DTfor.Columns.Add(New DataColumn("Fr_Des", GetType(String)))
                    DTfor.Columns.Add(New DataColumn("Elenco_ClassiTossicologiche", GetType(String)))
                    DTfor.Columns.Add(New DataColumn("Elenco_PrincipiAttivi", GetType(String)))


                    'Dim XmlDoc As New System.Xml.XmlDocument
                    Dim XMLs_Formulati As System.Xml.XmlNodeList
                    Dim XMLs_PrincipiAttivi As System.Xml.XmlNodeList
                    Dim XMLs_ClassiTossicologiche As System.Xml.XmlNodeList

                    Dim XmlRisultati As System.Xml.XmlElement
                    Dim XmlFormulato As System.Xml.XmlElement
                    Dim XmlPrincipioAttivo As System.Xml.XmlElement
                    Dim XmlClasseTossicologica As System.Xml.XmlElement

                    Dim DR As DataRow
                    Dim ict As Integer = 0
                    Dim ipa As Integer = 0
                    Dim Testo As String = ""

                    XmlDoc.LoadXml(WsRisposta)

                    'Recupero l'elenco dei Formulati
                    XMLs_Formulati = XmlDoc.GetElementsByTagName("FORMULATO")

                    If Not IsNothing(XMLs_Formulati) Then

                        For i = 0 To XMLs_Formulati.Count - 1

                            'Formulato i-esimo
                            XmlFormulato = XMLs_Formulati.Item(i)

                            'Creo la riga per il datatable
                            DR = DTfor.NewRow

                            DR.Item("Fr_Cod") = CInt(XmlFormulato.GetAttribute("fr_cod"))
                            DR.Item("Fr_Des") = CStr(XmlFormulato.GetAttribute("fr_des"))

                            '---------------------------------------------------
                            'CLASSE TOSSICOLOGICA ==>  cod1§des1|cod2§des2
                            'Recupero l'elenco delle Classi Tossicologiche

                            XMLs_ClassiTossicologiche = XmlFormulato.GetElementsByTagName("CLASSE_TOSSICOLOGICA")
                            Testo = ""
                            For ict = 0 To XMLs_ClassiTossicologiche.Count - 1
                                XmlClasseTossicologica = XMLs_ClassiTossicologiche.Item(ict)
                                Testo &= CStr(XmlClasseTossicologica.GetAttribute("cltoss_cod")) & "§" & _
                                         CStr(XmlClasseTossicologica.GetAttribute("cltoss_des")) & "|"
                            Next
                            If Testo <> "" Then
                                Testo = Left(Testo, Testo.Length - 1)
                            End If
                            DR.Item("Elenco_ClassiTossicologiche") = Testo

                            '---------------------------------------------------
                            'PRINCIPI ATTIVI ==> cod1§des1§titolo1|cod2§des2§titolo2
                            'Recupero l'elenco dei Principi Attivi

                            XMLs_PrincipiAttivi = XmlFormulato.GetElementsByTagName("PRINCIPIO_ATTIVO")

                            Testo = ""
                            For ipa = 0 To XMLs_PrincipiAttivi.Count - 1
                                XmlPrincipioAttivo = XMLs_PrincipiAttivi.Item(ipa)
                                Testo &= CStr(XmlPrincipioAttivo.GetAttribute("pa_cod")) & "§" & _
                                         CStr(XmlPrincipioAttivo.GetAttribute("pa_des")) & "§" & _
                                         CStr(XmlPrincipioAttivo.GetAttribute("titolo")) & "|"
                            Next
                            If Testo <> "" Then
                                Testo = Left(Testo, Testo.Length - 1)
                            End If

                            DR.Item("Elenco_PrincipiAttivi") = Testo

                            '---------------------------------------------------
                            DTfor.Rows.Add(DR)

                        Next

                    End If 'XMLs_Formulati

                End If 'ElencoFormulati

            End If 'IsNothing(DT)

        Catch ex As Exception
            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            Dim Log As New AgronicaCoreDataProvider.LogProvider
            Log.Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DTfor = Nothing
        End Try

        'Restituisco il risultato
        Return DTfor

    End Function


    '#############################################################################################################
    Public Shared Function ComposizioneFormulatiRecupera(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                                        ByVal ElencoFormulati As String, _
                                                        ByRef objServer As System.Web.HttpServerUtility, _
                                                        ByRef objSession As System.Web.SessionState.HttpSessionState, _
                                                        ByRef objPage As System.Web.UI.Page _
                                                        ) As DataTable

        Dim NomeRoutine As String = "SchedeMagazzinoHelper.ComposizioneFormulatiRecupera()"
        Dim MessaggioErrore As String = ""
        Dim DTfor As DataTable

        Try

            Dim i As Integer = 0
            Dim FrCod As Integer

            'Recupero l'elenco dei formulati 
            If ElencoFormulati <> "" Then

                'Creo la stringa XML di richiesta

                Dim XmlDoc As New System.Xml.XmlDocument
                Dim XmlCredenziali As System.Xml.XmlElement
                Dim XmlParametri As System.Xml.XmlElement
                Dim XmlNodo As System.Xml.XmlElement
                Dim StringaXML As String

                Dim WsScheda As String
                Dim WsDoorKey As String
                Dim WsCodiceGias As String
                Dim WsUsernameSuperuser As String
                Dim WsPasswordSuperuser As String

                '===========================================================================================
                '   <CREDENZIALI scheda="..." doorkey= "..." codicegias="..." username="..." password="..." >
                '       <PARAMETRI fr_cod=""                        oppure  "239,150" />
                '   </CREDENZIALI>
                '===========================================================================================

                WsScheda = "1080"                                               'AWS_Fitofarmaci_Formulati_PrincipiAttivi
                WsDoorKey = "portone_grth45ksh4ghajnl32149"
                WsCodiceGias = objSession("ASG_ProgressivoGIAS")
                WsUsernameSuperuser = objSession("ASG_SuperUser_Username")         'Session("ASG_SuperUser_Username_Crypt")
                WsPasswordSuperuser = objSession("ASG_SuperUser_Password")         'Session("ASG_SuperUser_Password_Crypt")

                'Creo il nodo CREDENZIALI
                XmlCredenziali = XmlDoc.CreateElement("CREDENZIALI")

                'Imposto gli attributi
                XmlCredenziali.SetAttribute("scheda", CStr(WsScheda))
                XmlCredenziali.SetAttribute("doorkey", CStr(WsDoorKey))
                XmlCredenziali.SetAttribute("codicegias", CStr(WsCodiceGias))
                XmlCredenziali.SetAttribute("username", CStr(WsUsernameSuperuser))
                XmlCredenziali.SetAttribute("password", CStr(WsPasswordSuperuser))

                'Imposto XmlParametri come figlio del documento principale
                XmlDoc.AppendChild(XmlCredenziali)

                'Creo il nodo PARAMETRI
                XmlParametri = XmlDoc.CreateElement("PARAMETRI")

                'Imposto gli attributi
                XmlParametri.SetAttribute("fr_cod", ElencoFormulati)

                'Imposto XmlParametri come figlio del documento principale
                XmlCredenziali.AppendChild(XmlParametri)

                'Restituisco in uscita la stringa creata
                StringaXML = XmlDoc.InnerXml

                'Distruggo gli oggetti
                XmlNodo = Nothing
                XmlParametri = Nothing
                'XmlDoc = Nothing

                '//////////////////////////////////////////////////////////
                '/////   Chiamo la funzione del webservice per recuperare la composizione
                '//////////////////////////////////////////////////////////

                Dim LinkWsFitofarmaci As String
                Dim WsRisposta As String = ""
                Dim objCoreWebService As New AgronicaCoreWebService.AgroWs
                StringaXML = objCoreWebService.AWS_Codifica_P(StringaXML)

                Try
                    Dim WsFito As New WS_Fitofarmaci.AgroWS_Fitofarmaci

                    'l'indirizzo dei web service deve stare solo nella tabella 'configurazione_siti'
                    'eliminata chiave nel web.config
                    ' Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig(objParametri_Server, False)
                    Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
                    If objWebConfig.GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci <> "" Then
                        LinkWsFitofarmaci = objWebConfig.GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci
                    End If

                    'If ConfigurationSettings.AppSettings("GiasOnline.WS_Fitofarmaci.AgroWS_Fitofarmaci").ToString = "" Then
                    '    LinkWsFitofarmaci = objServer.MapPath("https://ws.netagronica.it/AgronicaWebService/AgroWS_Fitofarmaci.asmx")
                    'Else
                    '    LinkWsFitofarmaci = ConfigurationSettings.AppSettings("GiasOnline.WS_Fitofarmaci.AgroWS_Fitofarmaci").ToString
                    'End If

                    WsFito.Url = LinkWsFitofarmaci
                    WsFito.Timeout = 60000
                    WsRisposta = WsFito.Formulati_PrincipiAttivi_2(StringaXML)

                Catch ex As Exception
                    Throw New Exception("Webservice fitofarmaci : " & ex.Message)
                End Try


                '//////////////////////////////////////////////////////////
                '/////   Decodifico il risultato ==> DataTable
                '//////////////////////////////////////////////////////////

                'Dim DTfor As New DataTable
                DTfor = New DataTable
                DTfor.Columns.Add(New DataColumn("Fr_Cod", GetType(Integer)))
                DTfor.Columns.Add(New DataColumn("Fr_Des", GetType(String)))
                DTfor.Columns.Add(New DataColumn("Elenco_ClassiTossicologiche", GetType(String)))
                DTfor.Columns.Add(New DataColumn("Elenco_PrincipiAttivi", GetType(String)))


                'Dim XmlDoc As New System.Xml.XmlDocument
                Dim XMLs_Formulati As System.Xml.XmlNodeList
                Dim XMLs_PrincipiAttivi As System.Xml.XmlNodeList
                Dim XMLs_ClassiTossicologiche As System.Xml.XmlNodeList

                Dim XmlRisultati As System.Xml.XmlElement
                Dim XmlFormulato As System.Xml.XmlElement
                Dim XmlPrincipioAttivo As System.Xml.XmlElement
                Dim XmlClasseTossicologica As System.Xml.XmlElement

                Dim DR As DataRow
                Dim ict As Integer = 0
                Dim ipa As Integer = 0
                Dim Testo As String = ""

                XmlDoc.LoadXml(WsRisposta)

                'Recupero l'elenco dei Formulati
                XMLs_Formulati = XmlDoc.GetElementsByTagName("FORMULATO")

                If Not IsNothing(XMLs_Formulati) Then

                    For i = 0 To XMLs_Formulati.Count - 1

                        'Formulato i-esimo
                        XmlFormulato = XMLs_Formulati.Item(i)

                        'Creo la riga per il datatable
                        DR = DTfor.NewRow

                        DR.Item("Fr_Cod") = CInt(XmlFormulato.GetAttribute("fr_cod"))
                        DR.Item("Fr_Des") = CStr(XmlFormulato.GetAttribute("fr_des"))

                        '---------------------------------------------------
                        'CLASSE TOSSICOLOGICA ==>  cod1§des1|cod2§des2
                        'Recupero l'elenco delle Classi Tossicologiche

                        XMLs_ClassiTossicologiche = XmlFormulato.GetElementsByTagName("CLASSE_TOSSICOLOGICA")
                        Testo = ""
                        For ict = 0 To XMLs_ClassiTossicologiche.Count - 1
                            XmlClasseTossicologica = XMLs_ClassiTossicologiche.Item(ict)
                            Testo &= CStr(XmlClasseTossicologica.GetAttribute("cltoss_cod")) & "§" & _
                                     CStr(XmlClasseTossicologica.GetAttribute("cltoss_des")) & "|"
                        Next
                        If Testo <> "" Then
                            Testo = Left(Testo, Testo.Length - 1)
                        End If

                        DR.Item("Elenco_ClassiTossicologiche") = Testo

                        '---------------------------------------------------
                        'PRINCIPI ATTIVI ==> cod1§des1§titolo1|cod2§des2§titolo2
                        'Recupero l'elenco dei Principi Attivi

                        XMLs_PrincipiAttivi = XmlFormulato.GetElementsByTagName("PRINCIPIO_ATTIVO")

                        Testo = ""
                        For ipa = 0 To XMLs_PrincipiAttivi.Count - 1
                            XmlPrincipioAttivo = XMLs_PrincipiAttivi.Item(ipa)
                            Testo &= CStr(XmlPrincipioAttivo.GetAttribute("pa_cod")) & "§" & _
                                     CStr(XmlPrincipioAttivo.GetAttribute("pa_des")) & "§" & _
                                     CStr(XmlPrincipioAttivo.GetAttribute("titolo")) & "|"
                        Next
                        If Testo <> "" Then
                            Testo = Left(Testo, Testo.Length - 1)
                        End If
                        DR.Item("Elenco_PrincipiAttivi") = Testo

                        '---------------------------------------------------
                        DTfor.Rows.Add(DR)

                    Next

                End If 'XMLs_Formulati

            End If 'IsNothing(DT)

        Catch ex As Exception
            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            Dim Log As New AgronicaCoreDataProvider.LogProvider
            Log.Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DTfor = Nothing
        End Try

        'Restituisco il risultato
        Return DTfor

    End Function


    '###########################################################################
    Public Shared Function ComposizioneFormulatiDescrizioneAggiuntiva(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                                                        ByVal Fr_Cod As Integer, _
                                                                        ByVal DtFor As DataTable) As String

        Dim NomeRoutine As String = "SchedeMagazzinoHelper.ComposizioneFormulatiDescrizioneAggiuntiva()"
        Dim MessaggioErrore As String = ""

        Dim DR As DataRow()
        Dim ElencoClassiTossicologiche As String = ""
        Dim ClassiTossicologiche As String()

        Dim ElencoPrincipiAttivi As String = ""
        Dim PrincipiAttivi As String()


        Dim ClToss_Cod As String = ""
        Dim ClToss_Des As String = ""
        Dim Pa_Cod As String = ""
        Dim Pa_Des As String = ""
        Dim Titolo As String = ""

        Dim i As Integer
        Dim Risultato As String = ""

        'Inizializzo
        Risultato = ""

        Try

            If Not IsNothing(DtFor) AndAlso DtFor.Rows.Count > 0 Then

                'Cerco il formulato corrente ...
                DR = DtFor.Select("Fr_Cod = " & Fr_Cod)

                If Not IsNothing(DR) AndAlso DR.Length > 0 Then

                    'CLASSI TOSSICOLOGICHE
                    ElencoClassiTossicologiche = DR(0).Item("Elenco_ClassiTossicologiche")
                    If ElencoClassiTossicologiche <> "" Then

                        Risultato = "   (Classe Toss. =  "
                        ClassiTossicologiche = ElencoClassiTossicologiche.Split("|")
                        For i = 0 To ClassiTossicologiche.Length - 1
                            Risultato &= ClassiTossicologiche(i).Split("§")(0) & "  "
                        Next
                        Risultato &= ")"

                    End If

                    'PRINCIPI ATTIVI
                    ElencoPrincipiAttivi = DR(0).Item("Elenco_PrincipiAttivi")
                    If ElencoPrincipiAttivi <> "" Then
                        Risultato &= vbCrLf
                        PrincipiAttivi = ElencoPrincipiAttivi.Split("|")
                        For i = 0 To PrincipiAttivi.Length - 1

                            Pa_Cod = PrincipiAttivi(i).Split("§")(0)
                            Pa_Des = PrincipiAttivi(i).Split("§")(1)
                            Titolo = PrincipiAttivi(i).Split("§")(2)

                            'Nota Bene : se il titolo e' "0" allora visualizzo solo la descrizione
                            If CDbl(Titolo) <> 0 Then
                                Risultato &= "     " & Format(CDbl(Titolo), "0.####") & " % - " & Pa_Des & vbCrLf
                            Else
                                Risultato &= "     " & "(" & Pa_Des & ")" & vbCrLf
                            End If
                        Next
                    End If
                End If
            End If

        Catch ex As Exception
            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            Dim Log As New AgronicaCoreDataProvider.LogProvider
            Log.Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DtFor = Nothing
        End Try

        Return Risultato

    End Function


    '###########################################################################
    Public Shared Function ComposizioneFormulatiClasseTossicologica(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                                                    ByVal Fr_Cod As Integer, _
                                                                    ByVal DtFor As DataTable) As String

        Dim NomeRoutine As String = "SchedeMagazzinoHelper.ComposizioneFormulatiClasseTossicologica()"
        Dim MessaggioErrore As String = ""

        Dim DR As DataRow()
        Dim ElencoClassiTossicologiche As String = ""
        Dim ClassiTossicologiche As String()

        Dim ElencoPrincipiAttivi As String = ""
        Dim PrincipiAttivi As String()

        Dim ClToss_Cod As String = ""
        Dim ClToss_Des As String = ""
        Dim Pa_Cod As String = ""
        Dim Pa_Des As String = ""
        Dim Titolo As String = ""

        Dim i As Integer
        Dim Risultato As String = ""

        Try

            If Not IsNothing(DtFor) AndAlso DtFor.Rows.Count > 0 Then

                'Cerco il formulato corrente ...
                DR = DtFor.Select("Fr_Cod = " & Fr_Cod)

                If Not IsNothing(DR) AndAlso DR.Length > 0 Then

                    'CLASSI TOSSICOLOGICHE
                    ElencoClassiTossicologiche = DR(0).Item("Elenco_ClassiTossicologiche")
                    If ElencoClassiTossicologiche <> "" Then

                        Risultato = ""
                        ClassiTossicologiche = ElencoClassiTossicologiche.Split("|")
                        For i = 0 To ClassiTossicologiche.Length - 1
                            Risultato &= ClassiTossicologiche(i).Split("§")(0) & "  "
                        Next
                        Risultato &= ""

                    End If

                End If

            End If

        Catch ex As Exception
            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            Dim Log As New AgronicaCoreDataProvider.LogProvider
            Log.Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DtFor = Nothing
        End Try

        Return Risultato

    End Function



    '###########################################################################
    Public Shared Function ComposizioneFormulatiDescrizionePrincipiAttivi(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                                                            ByVal Fr_Cod As Integer, _
                                                                            ByVal DtFor As DataTable) As String

        Dim NomeRoutine As String = "SchedeMagazzinoHelper.ComposizioneFormulatiDescrizioneAggiuntiva()"
        Dim MessaggioErrore As String = ""

        Dim DR As DataRow()
        Dim ElencoClassiTossicologiche As String = ""
        Dim ClassiTossicologiche As String()

        Dim ElencoPrincipiAttivi As String = ""
        Dim PrincipiAttivi As String()

        Dim ClToss_Cod As String = ""
        Dim ClToss_Des As String = ""
        Dim Pa_Cod As String = ""
        Dim Pa_Des As String = ""
        Dim Titolo As String = ""

        Dim i As Integer
        Dim Risultato As String = ""

        Try

            If Not IsNothing(DtFor) AndAlso DtFor.Rows.Count > 0 Then

                'Cerco il formulato corrente ...
                DR = DtFor.Select("Fr_Cod = " & Fr_Cod)

                If Not IsNothing(DR) AndAlso DR.Length > 0 Then

                    'PRINCIPI ATTIVI
                    ElencoPrincipiAttivi = DR(0).Item("Elenco_PrincipiAttivi")
                    If ElencoPrincipiAttivi <> "" Then

                        Risultato = vbCrLf
                        PrincipiAttivi = ElencoPrincipiAttivi.Split("|")
                        For i = 0 To PrincipiAttivi.Length - 1

                            Pa_Cod = PrincipiAttivi(i).Split("§")(0)
                            Pa_Des = PrincipiAttivi(i).Split("§")(1)
                            Titolo = PrincipiAttivi(i).Split("§")(2)

                            'Nota Bene : se il titolo e' "0" allora visualizzo solo la descrizione
                            If CDbl(Titolo) <> 0 Then
                                Risultato &= "     " & Format(CDbl(Titolo), "0.####") & " % - " & Pa_Des & vbCrLf
                            Else
                                Risultato &= "     " & "(" & Pa_Des & ")" & vbCrLf
                            End If
                        Next
                    End If

                End If

            End If

        Catch ex As Exception
            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            Dim Log As New AgronicaCoreDataProvider.LogProvider
            Log.Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DtFor = Nothing
        End Try

        Return Risultato

    End Function






End Class
