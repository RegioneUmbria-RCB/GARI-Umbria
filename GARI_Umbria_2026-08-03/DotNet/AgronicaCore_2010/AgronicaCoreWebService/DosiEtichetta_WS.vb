Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class DosiEtichetta_WS

    Public Shared Function DosiEtichetta_Elenco(Fr_Cod As Integer,
                                                Veg_Cod As Integer,
                                                Tipo_Richiesto As Integer,
                                                Av_Cod As Integer,
                                                Av_Gru As Integer,
                                                Data As String,
                                                Grfi_cod As Integer,
                                                FormulatiXAllegatiNormative_IDRiga As Integer,
                                                Copertura As String,
                                                ByRef objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                ByRef strErr As String) As DataTable

        Dim DtRisultati As New DataTable
        Dim Dati As String = ""

        Try
            Dim StrCredenziali As String = ""
            Dim XML_Credenziali As System.Xml.XmlElement
            Dim XmlDoc As New System.Xml.XmlDocument
            Dim StrParametri As String = ""
            Dim Parametri As String = ""

            Dim agroWs As String
            Dim objConfigurazione_Siti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            agroWs = objConfigurazione_Siti.Leggi_Valore(0, "GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci", "", "", objParametri_Server)
            If agroWs = "" Then
                agroWs = objConfigurazione_Siti.Leggi_Valore(0, "GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci", "", "", objParametri_Super_Server)
            End If

            Dim ObjDownloadWs = New WS_Fitofarmaci.AgroWS_Fitofarmaci
            Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS
            objWs.NewWS(ObjDownloadWs,
                            agroWs,
                            objParametri_Utenti)

            Dim ProgressivoGIAS As Integer
            Dim SuperUser_Password As String = ""

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
                                                    AgronicaCoreWebService.AgroWs.enum_AWS_Schede.AWS_Fitofarmaci_Formulati_SpecieVegetali_Avversita,
                                                    objCoreAgroWs.AgroWS_DoorKey(AgronicaCoreWebService.AgroWs.enum_AWS_Schede.AWS_Fitofarmaci_Formulati_SpecieVegetali_Avversita),
                                                    ProgressivoGIAS,
                                                    SuperUser_Piva:=objParametri_Server.PivaSuperUser,
                                                    SuperUser_UserName:=objParametri_Server.SuperUserUsername,
                                                    SuperUser_Password:=SuperUser_Password,
                                                    Utente_UserName:=objParametri_Server.UtenteUsername,
                                                    Utente_Password:="")
            Select Case Tipo_Richiesto

                Case enum_TipoFormulato.Disseccanti, enum_TipoFormulato.Diserbanti

                    objCoreAgroWs.AgroWS_XML__Credenziali(AgronicaCoreWebService.AgroWs.enum_AWS_Xml.Xml_CODIFICA,
                                                            StrCredenziali,
                                                            AgronicaCoreWebService.AgroWs.enum_AWS_Schede.AWS_Fitofarmaci_Formulati_SpecieVegetali_Infestanti_Dosi,
                                                            objCoreAgroWs.AgroWS_DoorKey(AgronicaCoreWebService.AgroWs.enum_AWS_Schede.AWS_Fitofarmaci_Formulati_SpecieVegetali_Infestanti_Dosi),
                                                            ProgressivoGIAS,
                                                            objParametri_Server.SuperUserUsername,
                                                            SuperUser_Password)

                    XmlDoc.LoadXml(StrCredenziali)

                    XML_Credenziali = XmlDoc.SelectSingleNode("CREDENZIALI")

                    objCoreAgroWs.AgroWS_XML_Parametri_Formulati_SpecieVegetali_Infestanti_Dosi(AgronicaCoreWebService.AgroWs.enum_AWS_Xml.Xml_CODIFICA,
                                                                                                  StrParametri,
                                                                                                  Fr_Cod:=Fr_Cod,
                                                                                                  Veg_Cod:=Veg_Cod,
                                                                                                  Av_Cod:=Av_Cod,
                                                                                                  Av_Gru:=Av_Gru,
                                                                                                  grfi_cod:=Grfi_cod,
                                                                                                  "0",
                                                                                                  FormulatiXAllegatiNormative_IDRiga:=FormulatiXAllegatiNormative_IDRiga,
                                                                                                  Copertura:=Copertura,
                                                                                                  Data:=Data,
                                                                                                  TipoRichiesto:=Tipo_Richiesto,
                                                                                                  strErr)

                    XML_Credenziali.InnerXml = StrParametri

                    Parametri = XmlDoc.OuterXml

                    Parametri = objCoreAgroWs.AWS_Codifica_P(Parametri)

                    DtRisultati = ObjDownloadWs.Formulati_SpecieVegetali_Infestanti_Dosi_DT(Parametri, strErr)

                Case Else

                    objCoreAgroWs.AgroWS_XML__Credenziali(AgronicaCoreWebService.AgroWs.enum_AWS_Xml.Xml_CODIFICA,
                                                            StrCredenziali,
                                                            AgronicaCoreWebService.AgroWs.enum_AWS_Schede.AWS_Fitofarmaci_Formulati_SpecieVegetali_Avversita_Dosi,
                                                            objCoreAgroWs.AgroWS_DoorKey(AgronicaCoreWebService.AgroWs.enum_AWS_Schede.AWS_Fitofarmaci_Formulati_SpecieVegetali_Avversita_Dosi),
                                                            ProgressivoGIAS,
                                                            objParametri_Server.SuperUserUsername,
                                                            SuperUser_Password)

                    XmlDoc.LoadXml(StrCredenziali)

                    XML_Credenziali = XmlDoc.SelectSingleNode("CREDENZIALI")

                    objCoreAgroWs.AgroWS_XML_Parametri_Formulati_SpecieVegetali_Avversita_Dosi(AgronicaCoreWebService.AgroWs.enum_AWS_Xml.Xml_CODIFICA,
                                                                                                  StrParametri,
                                                                                                  Fr_Cod:=Fr_Cod,
                                                                                                  Veg_Cod:=Veg_Cod,
                                                                                                  Av_Cod:=Av_Cod,
                                                                                                  Av_Gru:=Av_Gru,
                                                                                                  grfi_cod:=Grfi_cod,
                                                                                                  "0",
                                                                                                  FormulatiXAllegatiNormative_IDRiga:=FormulatiXAllegatiNormative_IDRiga,
                                                                                                  Copertura:=Copertura,
                                                                                                  Data:=Data,
                                                                                                  TipoRichiesto:=Tipo_Richiesto,
                                                                                                  strErr)

                    XML_Credenziali.InnerXml = StrParametri

                    Parametri = XmlDoc.OuterXml

                    Parametri = objCoreAgroWs.AWS_Codifica_P(Parametri)

                    DtRisultati = ObjDownloadWs.Formulati_SpecieVegetali_Avversita_Dosi_DT(Parametri, strErr)

            End Select

        Catch ex As Exception
            Dati = "Si sono verificati errori in fase di chiamata al WebService Dosi Etichetta!" & Chr(13) & ex.Message
            Throw New Exception(Dati)
        End Try

        Return DtRisultati

    End Function

End Class
