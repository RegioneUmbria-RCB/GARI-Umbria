Imports AgronicaCoreDataProvider

Public Class Avversita_WS

    Public Shared Function Avversita_Elenco(Fr_Cod As String,
                                            Veg_Cod As String,
                                            Dpi_Cod As String,
                                            DPI_Privato_Pubblico As Integer,
                                            Id_Rcdpi As Integer,
                                            Tipo_Testata As Integer,
                                            Modulo As Integer,
                                            Ep_Cod As Integer,
                                            Validita_Fine As String,
                                            strListaComuni As String,
                                            Storico As Boolean,
                                            FormulatiXAllegatiNormative_IDRiga As Integer,
                                            Lingua_Cod As String,
                                            TipiFormulato As String,
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

            XmlDoc.LoadXml(StrCredenziali)

            XML_Credenziali = XmlDoc.SelectSingleNode("CREDENZIALI")

            objCoreAgroWs.AgroWS_XML_Parametri_Avversita(Operazione:=AgronicaCoreWebService.AgroWs.enum_AWS_Xml.Xml_CODIFICA,
                                                                            StringaXML:=StrParametri,
                                                                            Fr_Cod:=Fr_Cod,
                                                                            Veg_Cod:=Veg_Cod,
                                                                            Id_RCDPI:=Id_Rcdpi,
                                                                            Disciplinare_Cod:=Dpi_Cod,
                                                                            DPI_Privato_Pubblico:=DPI_Privato_Pubblico,
                                                                            Tipo_Testata:=Tipo_Testata,
                                                                            Id_GaDPI:=0,
                                                                            Av_Cod:=0,
                                                                            Av_Gru:=0,
                                                                            Pa_Cod:=0,
                                                                            Gru_Pa_Cod:=0,
                                                                            Id_Paa:=0,
                                                                            str_Avversita:="",
                                                                            Modulo:=Modulo,
                                                                            Ep_Cod:=Ep_Cod,
                                                                            FormulatiXAllegatiNormative_IDRiga:=FormulatiXAllegatiNormative_IDRiga,
                                                                            Storico:=Storico,
                                                                            Lingua_Cod:=Lingua_Cod,
                                                                            str_Filtro:="",
                                                                            str_Sort:="",
                                                                            Data:=Validita_Fine,
                                                                            ListaComuni:=strListaComuni,
                                                                            TipiFormulato:=TipiFormulato,
                                                                            strErr)

            XML_Credenziali.InnerXml = StrParametri

            Parametri = XmlDoc.OuterXml

            Parametri = objCoreAgroWs.AWS_Codifica_P(Parametri)

            Dt = ObjDownloadWs.Leggi_Infestanti_conDescrizioniAvversita_DT(Parametri, strErr)

        Catch ex As Exception
            Dati = "Si sono verificati errori in fase di chiamata al WebService Avversità!" & Chr(13) & ex.Message
            Throw New Exception(Dati)
        End Try

        Return Dt

    End Function


    Public Shared Function Soglie_Avversita_Elenco(Dpi_Cod As String,
                                                    Id_Rcdpi As Integer,
                                                    Av_Cod As Integer,
                                                    ByRef objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                    ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                    ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                    ByRef strErr As String) As DataTable

        Dim ObjDownloadWs As WS_Disciplinari.AgroWS_Disciplinari

        Dim DtRisultati As New DataTable
        DtRisultati.Columns.Add(New DataColumn("Av_Cod", GetType(Integer)))
        DtRisultati.Columns.Add(New DataColumn("Soglia", GetType(String)))
        DtRisultati.Columns.Add(New DataColumn("Soglia_Cod", GetType(String)))
        DtRisultati.Columns.Add(New DataColumn("Quantita", GetType(String)))
        DtRisultati.Columns.Add(New DataColumn("Udm_Cod", GetType(Integer)))
        DtRisultati.Columns.Add(New DataColumn("Lav_Cod", GetType(Integer)))
        DtRisultati.TableName = "SoglieAvversita"

        Dim Dati As String

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

            Dim SuperUser_Password As String = ""

            Dim dtUtente As DataTable
            Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Read
            dtUtente = objUtenti.Leggi_Superuser_e_ProgressivoGIAS(objParametri_Utenti.UtenteUsername, "",
                                                            Date.Now,
                                                            CType(Now.Hour, Short),
                                                            0, objParametri_Utenti)
            If Not dtUtente Is Nothing AndAlso dtUtente.Rows.Count > 0 Then
                SuperUser_Password = dtUtente.Rows(0).Item("Password_SuperUser")
            End If

            Dim ASG_Utente_Username_Crypt = Sicurezza.Stringa_Codifica_LANCompatibile(objParametri_Server.SuperUserUsername, CostantiPersonalizzate.AgroKey_EncoderDecoder)
            Dim ASG_Utente_Password_Crypt = Sicurezza.Stringa_Codifica_LANCompatibile(SuperUser_Password, CostantiPersonalizzate.AgroKey_EncoderDecoder)


            Dati = ObjDownloadWs.Leggi_SoglieIntervento(Id_RCDPI:=Id_Rcdpi,
                                                            Disciplinare_Cod:=CInt(Dpi_Cod),
                                                            0,
                                                            0,
                                                            0,
                                                            0,
                                                            0,
                                                            strAvversita:=" Av_Cod=" & Av_Cod,
                                                            0,
                                                            ASG_Utente_Username_Crypt,
                                                            ASG_Utente_Password_Crypt)

            If Not IsNothing(Dati) AndAlso Dati.ToLower.IndexOf("errore") < 0 Then

                Dim XmlDocumento As New System.Xml.XmlDocument
                Dim XmlNodo As System.Xml.XmlNodeList
                Dim XmlElemento As System.Xml.XmlElement
                XmlDocumento.LoadXml(Dati)
                XmlNodo = XmlDocumento.GetElementsByTagName("Record")

                If Not XmlNodo Is Nothing Then
                    For Each XmlElemento In XmlNodo

                        Dim Dr = DtRisultati.NewRow

                        Dr.Item("Av_Cod") = CInt(XmlElemento.GetAttribute("av_cod"))
                        Dr.Item("Soglia") = CStr(XmlElemento.GetAttribute("dichiarazione_des").ToString.ToLower)
                        Dr.Item("Soglia_Cod") = CStr(XmlElemento.GetAttribute("si_cod"))

                        Dr.Item("Quantita") = CStr(XmlElemento.GetAttribute("quantita"))
                        Dr.Item("Udm_Cod") = 0
                        If IsNumeric(XmlElemento.GetAttribute("udm_cod_specifico")) Then
                            Dr.Item("Udm_Cod") = CInt(XmlElemento.GetAttribute("udm_cod_specifico"))
                        End If
                        Dr.Item("Lav_Cod") = 0
                        If IsNumeric(XmlElemento.GetAttribute("lav_cod")) Then
                            Dr.Item("Lav_Cod") = CInt(XmlElemento.GetAttribute("lav_cod"))
                        End If

                        DtRisultati.Rows.Add(Dr)
                    Next
                End If

            End If

        Catch ex As Exception
            Dati = "Si sono verificati errori in fase di chiamata al WebService Soglie Avversità!" & Chr(13) & ex.Message
            Throw New Exception(Dati)
        End Try

        Return DtRisultati

    End Function

    Public Shared Function Soglia_Intervento(Dpi_Cod As String,
                                                Si_Cod As Integer,
                                                ByRef objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                ByRef strErr As String) As Integer

        Dim ObjDownloadWs As WS_Disciplinari.AgroWS_Disciplinari

        Dim lav_cod As Integer = 0

        Dim Dati As String

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

            Dim SuperUser_Password As String = ""

            Dim dtUtente As DataTable
            Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Read
            dtUtente = objUtenti.Leggi_Superuser_e_ProgressivoGIAS(objParametri_Utenti.UtenteUsername, "",
                                                            Date.Now,
                                                            CType(Now.Hour, Short),
                                                            0, objParametri_Utenti)
            If Not dtUtente Is Nothing AndAlso dtUtente.Rows.Count > 0 Then
                SuperUser_Password = dtUtente.Rows(0).Item("Password_SuperUser")
            End If

            Dim ASG_Utente_Username_Crypt = Sicurezza.Stringa_Codifica_LANCompatibile(objParametri_Server.SuperUserUsername, CostantiPersonalizzate.AgroKey_EncoderDecoder)
            Dim ASG_Utente_Password_Crypt = Sicurezza.Stringa_Codifica_LANCompatibile(SuperUser_Password, CostantiPersonalizzate.AgroKey_EncoderDecoder)


            Dati = ObjDownloadWs.Leggi_SogliaIntervento(Disciplinare_Cod:=CInt(Dpi_Cod),
                                                            Si_Cod:=Si_Cod,
                                                            ASG_Utente_Username_Crypt,
                                                            ASG_Utente_Password_Crypt)

            If Not IsNothing(Dati) AndAlso Dati.ToLower.IndexOf("errore") < 0 Then

                Dim XmlDocumento As New System.Xml.XmlDocument
                Dim XmlNodo As System.Xml.XmlNodeList
                Dim XmlElemento As System.Xml.XmlElement
                XmlDocumento.LoadXml(Dati)
                XmlNodo = XmlDocumento.GetElementsByTagName("Record")

                If XmlNodo IsNot Nothing AndAlso XmlNodo.Count > 0 Then

                    XmlElemento = XmlNodo.Item(0)
                    Dim lavCodStr As String = XmlElemento.GetAttribute("lav_cod")
                    If IsNumeric(lavCodStr) Then
                        lav_cod = CInt(lavCodStr)
                    End If

                End If

            End If

        Catch ex As Exception
            Dati = "Si sono verificati errori in fase di chiamata al WebService Soglie Avversità!" & Chr(13) & ex.Message
            Throw New Exception(Dati)
        End Try

        Return lav_cod

    End Function

End Class
