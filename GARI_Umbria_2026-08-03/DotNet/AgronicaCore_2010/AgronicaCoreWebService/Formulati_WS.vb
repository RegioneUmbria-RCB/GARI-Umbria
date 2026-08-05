Imports System.Web
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class Formulati_WS


    Public Sub RecuperaDittaInDescrizioneFormulati(
        ByRef DTInput As DataTable,
        ByVal objParametri_Utenti As AgronicaCoreParametri,
        NomeColonnaFrCod As String,
        nomecolonnaDescrizione As String,
        accodaDittaSuDescrizione As Boolean,
        nomecolonnaDitta As String
    )

        Dim XML_Credenziali As System.Xml.XmlElement
        Dim StrCredenziali As String = ""
        Dim StrParametri As String = ""
        Dim Parametri As String = ""
        Dim strErr As String = ""
        Dim LastFr_Des As String = ""
        Dim i As Integer


        Try
            If nomecolonnaDitta = "" Then
                nomecolonnaDitta = "Ditta_Des"
            End If

            If Not DTInput.Columns.Contains(nomecolonnaDitta) Then
                DTInput.Columns.Add(nomecolonnaDitta, System.Type.GetType("System.String"))
            End If

            Dim ObjDownloadWs As New WS_Fitofarmaci.AgroWS_Fitofarmaci
            Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS

            Dim objAgroWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
            Dim XmlDoc As New System.Xml.XmlDocument


            objWs.NewWS(ObjDownloadWs,
                            objAgroWebConfig.GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci,
                            objParametri_Utenti)
            XmlDoc = New System.Xml.XmlDocument

            Dim objCoreAgroWs As New AgronicaCoreWebService.AgroWs

            objCoreAgroWs.AgroWS_XML__Credenziali(AgronicaCoreWebService.AgroWs.enum_AWS_Xml.Xml_CODIFICA,
                                            StrCredenziali,
                                            AgronicaCoreWebService.AgroWs.enum_AWS_Schede.AWS_Fitofarmaci_Formulato_Completo,
                                            objCoreAgroWs.AgroWS_DoorKey(AgronicaCoreWebService.AgroWs.enum_AWS_Schede.AWS_Fitofarmaci_Formulato_Completo),
                                            HttpContext.Current.Session("ASG_ProgressivoGIAS"),
                                            HttpContext.Current.Session("ASG_SuperUser_Username").ToString,
                                            HttpContext.Current.Session("ASG_SuperUser_Password").ToString)

            XmlDoc.LoadXml(StrCredenziali)

            XML_Credenziali = XmlDoc.SelectSingleNode("CREDENZIALI")

            Dim listaFrCod As List(Of String) = (From l In DTInput.AsEnumerable Select CStr(l(NomeColonnaFrCod))).Distinct.ToList()

            Dim fr_cod As String =
                String.Join(",", listaFrCod)

            'Dim grfi_Cod As Integer = 0
            objCoreAgroWs.AgroWS_XML_Parametri_Formulato_Completo(AgronicaCoreWebService.AgroWs.enum_AWS_Xml.Xml_CODIFICA,
                                                   StrParametri,
                                                   fr_cod,
                                                   strErr)

            XML_Credenziali.InnerXml = StrParametri

            Parametri = XmlDoc.OuterXml

            Parametri = objCoreAgroWs.AWS_Codifica_P(Parametri)
            Dim dtListaDitte As DataTable
            dtListaDitte = ObjDownloadWs.Leggi_Formulati_ListaDitte_DT(Parametri, strErr)

            For Each dr1 In DTInput.Rows

                Dim dittaXformulato As DataRow() =
                    dtListaDitte.Select(" fr_cod = " & dr1(NomeColonnaFrCod))
                Dim des As String = " "
                For Each df In dittaXformulato
                    'des &= df("Ditta_Des") & " "
                    'Select Case df("TipoDitta_cod").ToString
                    '    Case "2"
                    '        des &= " (Detentrice) "
                    '    Case "1"
                    '        des &= " (Distributrice) "
                    'End Select

                    If df("TipoDitta_cod").ToString = "2" Then
                        des &= df("Ditta_Des") & " "
                    End If

                Next

                If accodaDittaSuDescrizione Then
                    dr1(nomecolonnaDescrizione) = dr1(nomecolonnaDescrizione) & "|" & dr1("fr_cod") & "|" & des
                Else
                    dr1(nomecolonnaDitta) = des
                End If

            Next

        Catch ex As Exception

        End Try


    End Sub

    Public Shared Function Formulati_Elenco(Veg_Cod As String,
                                            Dpi_Cod As String,
                                            DPI_Privato_Pubblico As Integer,
                                            Id_Rcdpi As Integer,
                                            Tipo_Testata As Integer,
                                            Opt_Avversita_Infestanti As String,
                                            Opt_Singola_Gruppo As String,
                                            Av_Gru As Integer(),
                                            Av_Cod As Integer(),
                                            strAvversita As String,
                                            Modulo As Integer,
                                            Ep_Cod As Integer,
                                            TipoRichiesto As String,
                                            TestoRicerca As String,
                                            Validita_Fine As String,
                                            strFiltro As String,
                                            Grfi_cod As Integer,
                                            strListaComuni As String,
                                            FormulatiXAllegatiNormative_IDRiga As Integer,
                                            Copertura As String,
                                            Stato_Cod As String,
                                            Lingua_Cod As String,
                                    ByRef objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                    ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                    ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                    ByRef strErr As String) As DataTable

        Dim ObjDownloadWs As WS_Disciplinari.AgroWS_Disciplinari
        Dim Dati As String
        Dim StrCredenziali As String = ""
        Dim XML_Credenziali As System.Xml.XmlElement

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlDocumento As New System.Xml.XmlDocument

        Dim Dt As New DataTable

        Dim StrParametri As String = ""
        Dim Parametri As String = ""

        Try

            Dim agroWs As String
            Dim objConfigurazione_Siti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            agroWs = objConfigurazione_Siti.Leggi_Valore(0, "GiasOnline_WS_Disciplinari_AgroWS_Disciplinari", "", "", objParametri_Server)
            If agroWs = "" Then
                agroWs = objConfigurazione_Siti.Leggi_Valore(0, "GiasOnline_WS_Disciplinari_AgroWS_Disciplinari", "", "", objParametri_Super_Server)
            End If

            ObjDownloadWs = New WS_Disciplinari.AgroWS_Disciplinari
            Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS
            objWs.NewWS(ObjDownloadWs,
                            agroWs,
                            objParametri_Utenti)

            Dim ProgressivoGIAS As Integer
            Dim SuperUser_Password As String

            Dim dtUtente As DataTable
            Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Read
            dtUtente = objUtenti.Leggi_Superuser_e_ProgressivoGIAS(objParametri_Utenti.UtenteUsername, "",
                                                            Date.Now,
                                                            CType(Now.Hour, Short),
                                                            0, objParametri_Utenti)
            If Not dtUtente Is Nothing AndAlso dtUtente.Rows.Count > 0 Then
                ProgressivoGIAS = dtUtente.Rows(0).Item("ProgressivoGIAS")
                SuperUser_Password = dtUtente.Rows(0).Item("Password_SuperUser")
            End If

            Dim objCoreAgroWs As New AgronicaCoreWebService.AgroWs
            objCoreAgroWs.AgroWS_XML__Credenziali(AgronicaCoreWebService.AgroWs.enum_AWS_Xml.Xml_CODIFICA,
                                                    StrCredenziali,
                                                    AgronicaCoreWebService.AgroWs.enum_AWS_Schede.AWS_Disciplinari_Formulato_Completo,
                                                    objCoreAgroWs.AgroWS_DoorKey(AgronicaCoreWebService.AgroWs.enum_AWS_Schede.AWS_Disciplinari_Formulato_Completo),
                                                    ProgressivoGIAS,
                                                    SuperUser_Piva:=objParametri_Server.PivaSuperUser,
                                                    SuperUser_UserName:=objParametri_Server.SuperUserUsername,
                                                    SuperUser_Password:=SuperUser_Password,
                                                    Utente_UserName:=objParametri_Server.UtenteUsername,
                                                    Utente_Password:="")

            XmlDoc.LoadXml(StrCredenziali)

            XML_Credenziali = XmlDoc.SelectSingleNode("CREDENZIALI")

            objCoreAgroWs.AgroWS_XML_Parametri_Formulati_ConDosi_Profitosan(Operazione:=AgronicaCoreWebService.AgroWs.enum_AWS_Xml.Xml_CODIFICA,
                                                                            StringaXML:=StrParametri,
                                                                            Applicazione_Richiedente:=enum_AWS_ApplicazioneRichiedente.AgronicaAgenda,
                                                                            Veg_Cod:=Veg_Cod,
                                                                            Dpi_Cod:=Dpi_Cod,
                                                                            DPI_Privato_Pubblico:=DPI_Privato_Pubblico,
                                                                            Id_Rcdpi:=Id_Rcdpi,
                                                                            Tipo_Testata:=Tipo_Testata,
                                                                            Opt_Avversita_Infestanti:=Opt_Avversita_Infestanti,
                                                                            Opt_Singola_Gruppo:=Opt_Singola_Gruppo,
                                                                            Av_Gru:=Av_Gru,
                                                                            Av_Cod:=Av_Cod,
                                                                            strAvversita:=strAvversita,
                                                                            Modulo:=Modulo,
                                                                            Ep_Cod:=Ep_Cod,
                                                                            strPA:="",
                                                                            TipoRichiesto:=TipoRichiesto,
                                                                            TestoRicerca:=TestoRicerca,
                                                                            Data:=Validita_Fine,
                                                                            strFiltro:=strFiltro,
                                                                            strSort:="",
                                                                            Errore:=strErr,
                                                                            Grfi_cod:=Grfi_cod,
                                                                            FlagVisualizza_Commercio_Tutti_Revocati:=0,
                                                                            ListaComuni:=strListaComuni,
                                                                            FormulatiXAllegatiNormative_IDRiga:=FormulatiXAllegatiNormative_IDRiga,
                                                                            Copertura:=Copertura,
                                                                            Stato_Cod:=Stato_Cod,
                                                                            Lingua_Cod:=Lingua_Cod)

            XML_Credenziali.InnerXml = StrParametri

            Parametri = XmlDoc.OuterXml

            Parametri = objCoreAgroWs.AWS_Codifica_P(Parametri)

            Dt = ObjDownloadWs.Leggi_Formulati_DT(Parametri, strErr)

        Catch ex As Exception
            Dati = "Si sono verificati errori in fase di chiamata al WebService Formulati!" & Chr(13) & ex.Message
            Throw New Exception(Dati)
        End Try

        Return Dt

    End Function

End Class
