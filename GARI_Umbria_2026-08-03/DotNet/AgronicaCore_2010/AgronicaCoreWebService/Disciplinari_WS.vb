Imports System.Web
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class Disciplinari_WS

    Public Shared Function Disciplinari_Elenco_TuttigliElemInChiave(
                                                        ByRef objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                        ByVal Reg_Cod As Integer,
                                                        ByVal VEG_COD As Integer,
                                                        ByVal Id_RcDpi As Integer,
                                                        ByVal Flag_Privato_Pubblico As Integer,
                                                        ByVal Includi_Nessuno As Boolean,
                                                        ByVal Includi_Biologico As Boolean,
                                                        ByVal Flag_DisciplinareAttivo As Boolean) As DataTable


        Dim strErr As String = ""

        Dim ObjDownloadWs As WS_Disciplinari.AgroWS_Disciplinari
        Dim Dati As String

        Dim XmlDocumento As New System.Xml.XmlDocument
        Dim XmlNodo As System.Xml.XmlNodeList
        Dim XmlElemento As System.Xml.XmlElement

        Dim Dt As New DataTable
        Dim Dr As DataRow

        Dt.Columns.Add(New DataColumn("nomeEsteso", GetType(String)))
        Dt.Columns.Add(New DataColumn("codRegolamento", GetType(String)))
        Dt.Columns.Add(New DataColumn("PubblicoPrivato", GetType(String)))
        Dt.Columns.Add(New DataColumn("PUA_Regolamento_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("id_tr", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Validita_Inizio", GetType(Date)))
        Dt.Columns.Add(New DataColumn("Validita_Fine", GetType(Date)))

        Dim chiaveCache = "Disciplinari_Elenco_TuttigliElemInChiave_" & objParametri_Server.PivaSuperUser & "_" & Reg_Cod & "_" & VEG_COD & "_" & Flag_Privato_Pubblico

        If Flag_DisciplinareAttivo Then

            Try

                If HttpContext.Current IsNot Nothing AndAlso HttpContext.Current.Cache(chiaveCache) IsNot Nothing Then
                    Return HttpContext.Current.Cache(chiaveCache)
                End If

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

                Dim ASG_Utente_Username_Crypt = Sicurezza.Stringa_Codifica_LANCompatibile(objParametri_Server.SuperUserUsername, CostantiPersonalizzate.AgroKey_EncoderDecoder)
                Dim xletturautente As New AgronicaCoreUtentiDAL.Utenti_Read
                Dim pass As String = xletturautente.Password_From_UserName(objParametri_Server.SuperUserUsername, objParametri_Utenti)
                Dim ASG_Utente_Password_Crypt = Sicurezza.Stringa_Codifica_LANCompatibile(pass, AgroKey_EncoderDecoder)


                Dati = ObjDownloadWs.Leggi_Disciplinari_Elenco(CInt(0),
                                                        CInt(VEG_COD),
                                                        CInt(Reg_Cod),
                                                        CInt(Flag_Privato_Pubblico),
                                                        objParametri_Server.FinestraTemporaleInizio,
                                                        objParametri_Server.FinestraTemporaleFine,
                                                        objParametri_Server.PivaSuperUser,
                                                        ASG_Utente_Username_Crypt,
                                                        ASG_Utente_Password_Crypt,
                                                        strErr)


                If strErr = "" Then
                    If Not IsNothing(Dati) AndAlso Dati.ToLower.IndexOf("errore") < 0 Then
                        XmlDocumento.LoadXml(Dati)
                        XmlNodo = XmlDocumento.GetElementsByTagName("Record")
                        If Not XmlNodo Is Nothing Then
                            For Each XmlElemento In XmlNodo
                                Dr = Dt.NewRow
                                Dr.Item("nomeEsteso") = XmlElemento.GetAttribute("nomeesteso")
                                Dr.Item("codRegolamento") = XmlElemento.GetAttribute("cod_regolamento")
                                Dr.Item("PubblicoPrivato") = XmlElemento.GetAttribute("pubblicoprivato")
                                Dr.Item("PUA_Regolamento_Cod") = XmlElemento.GetAttribute("pua_regolamento_cod")
                                Dr.Item("id_tr") = XmlElemento.GetAttribute("id_tr")

                                If XmlElemento.HasAttribute("validodal") AndAlso IsDate(XmlElemento.GetAttribute("validodal")) Then
                                    Dr.Item("Validita_Inizio") = CDate(XmlElemento.GetAttribute("validodal"))
                                Else
                                    Dr.Item("Validita_Inizio") = AGRODATAINIZIO
                                End If


                                If XmlElemento.HasAttribute("validoal") AndAlso IsDate(XmlElemento.GetAttribute("validoal")) Then
                                    Dr.Item("Validita_Fine") = CDate(XmlElemento.GetAttribute("validoal"))
                                Else
                                    Dr.Item("Validita_Fine") = AGRODATAFINE
                                End If

                                Dt.Rows.Add(Dr)
                            Next
                        End If
                    End If
                End If

                If HttpContext.Current IsNot Nothing Then
                    HttpContext.Current.Cache(chiaveCache) = Dt
                End If

                If Not Dt Is Nothing Then
                    Return Dt
                End If


            Catch ex As Exception

                Dati = "Si sono verificati errori in fase di chiamata al WebService Discliplinari!" & Chr(13) &
                       ex.Message

                Return Dt

            End Try

        End If
        Return Dt
    End Function

    Public Shared Function Disciplinari_Elenco_xTestata(
                                                        ByRef objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                        ByVal Reg_Cod As Integer,
                                                        ByVal VEG_COD As Integer,
                                                        ByVal Id_RcDpi As Integer,
                                                        ByVal Data As Date,
                                                        ByVal Tipo_Testata As Integer) As DataTable

        Dim strErr As String = ""

        Dim ObjDownloadWs As WS_Disciplinari.AgroWS_Disciplinari
        Dim Dati As String

        Dim XmlDocumento As New System.Xml.XmlDocument
        Dim XmlNodo As System.Xml.XmlNodeList
        Dim XmlElemento As System.Xml.XmlElement

        Dim Dt As New DataTable
        Dim Dr As DataRow

        Dt.Columns.Add(New DataColumn("Dpi_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Dpi_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("IdRcdpi", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Grfi_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Flag_Protetto", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("PubblicoPrivato", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("DesRcdpi", GetType(String)))

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

            Dim ASG_Utente_Username_Crypt = Sicurezza.Stringa_Codifica_LANCompatibile(objParametri_Server.SuperUserUsername, CostantiPersonalizzate.AgroKey_EncoderDecoder)
            Dim xletturautente As New AgronicaCoreUtentiDAL.Utenti_Read
            Dim pass As String = xletturautente.Password_From_UserName(objParametri_Server.SuperUserUsername, objParametri_Utenti)
            Dim ASG_Utente_Password_Crypt = Sicurezza.Stringa_Codifica_LANCompatibile(pass, AgroKey_EncoderDecoder)

            'Per la funzione Leggi_Disciplinari_Elenco_xTestata_conRegolamentoConcimazione il Flag_DisciplinarePrivato ha il seguente significato:
            '- 0: sia pubblici che privati
            '- 1: solo pubblici
            '- 2: solo privati
            'Il Flag_DisciplinarePrivato può assumere valori true o false, per cui con:
            '- True --> 0, quindi sia pubblici che privati
            '- False --> 1, qundi solo pubblici
            Dim Flag_Privato_Pubblico As Integer = 0
            Dim flag_DisciplinarePrivato = objConfigurazione_Siti.Leggi_Valore(0, "Flag_DisciplinarePrivato", "", "", objParametri_Server)
            If flag_DisciplinarePrivato = "" Then
                flag_DisciplinarePrivato = objConfigurazione_Siti.Leggi_Valore(0, "Flag_DisciplinarePrivato", "", "", objParametri_Super_Server)
            End If

            If flag_DisciplinarePrivato <> "" Then

                Select Case CBool(flag_DisciplinarePrivato)
                    Case True
                        Flag_Privato_Pubblico = 0
                    Case Else
                        Flag_Privato_Pubblico = 1
                End Select

            End If

            Dati = ObjDownloadWs.Leggi_Disciplinari_Elenco_xTestata(Id_RcDpi,
                                                                    VEG_COD,
                                                                    Reg_Cod,
                                                                    Flag_Privato_Pubblico,
                                                                    Tipo_Testata,
                                                                    Data,
                                                                    Data,
                                                                    objParametri_Server.PivaSuperUser,
                                                                    ASG_Utente_Username_Crypt,
                                                                    ASG_Utente_Password_Crypt,
                                                                    strErr)


            If strErr = "" Then
                If Not IsNothing(Dati) AndAlso Dati.ToLower.IndexOf("errore") < 0 Then
                    XmlDocumento.LoadXml(Dati)
                    XmlNodo = XmlDocumento.GetElementsByTagName("Record")
                    If Not XmlNodo Is Nothing Then
                        For Each XmlElemento In XmlNodo
                            Dr = Dt.NewRow
                            Dr.Item("Dpi_Cod") = XmlElemento.GetAttribute("cod_regolamento")
                            Dr.Item("Dpi_Des") = XmlElemento.GetAttribute("nomeesteso")
                            Dr.Item("IdRcdpi") = XmlElemento.GetAttribute("id_rcdpi")
                            Dr.Item("Grfi_Cod") = XmlElemento.GetAttribute("grfi_cod")
                            Dr.Item("Flag_Protetto") = XmlElemento.GetAttribute("flag_protetto")
                            Dr.Item("PubblicoPrivato") = XmlElemento.GetAttribute("pubblicoprivato")
                            Dr.Item("DesRcdpi") = XmlElemento.GetAttribute("nome")
                            Dt.Rows.Add(Dr)
                        Next
                    End If
                End If
            End If

            If Not Dt Is Nothing Then
                Return Dt
            End If


        Catch ex As Exception

            Dati = "Si sono verificati errori in fase di chiamata al WebService Discliplinari!" & Chr(13) & ex.Message
            Throw New Exception(Dati)

        End Try

        Return Dt
    End Function


    Public Shared Function Disciplinari_Elenco_xTestata_conRegolamentoConcimazione(
                                                        ByRef objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                        ByVal Cod_Regolamento As Integer,
                                                        ByVal Reg_Cod As Integer,
                                                        ByVal VEG_COD As Integer,
                                                        ByVal Id_RcDpi As Integer,
                                                        ByVal Data As Date,
                                                        ByVal Tipo_Testata As Integer) As DataTable

        Dim strErr As String = ""

        Dim ObjDownloadWs As WS_Disciplinari.AgroWS_Disciplinari
        Dim Dati As String

        Dim XmlDocumento As New System.Xml.XmlDocument
        Dim XmlNodo As System.Xml.XmlNodeList
        Dim XmlElemento As System.Xml.XmlElement

        Dim Dt As New DataTable
        Dim Dr As DataRow

        Dt.Columns.Add(New DataColumn("Dpi_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Dpi_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("IdRcdpi", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Grfi_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Flag_Protetto", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("PubblicoPrivato", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("DesRcdpi", GetType(String)))
        Dt.Columns.Add(New DataColumn("PUA_Regolamento_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("PUA_Regolamento_Tipo", GetType(Integer)))

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

            Dim ASG_Utente_Username_Crypt = Sicurezza.Stringa_Codifica_LANCompatibile(objParametri_Server.SuperUserUsername, CostantiPersonalizzate.AgroKey_EncoderDecoder)
            Dim xletturautente As New AgronicaCoreUtentiDAL.Utenti_Read
            Dim pass As String = xletturautente.Password_From_UserName(objParametri_Server.SuperUserUsername, objParametri_Utenti)
            Dim ASG_Utente_Password_Crypt = Sicurezza.Stringa_Codifica_LANCompatibile(pass, AgroKey_EncoderDecoder)

            'Per la funzione Leggi_Disciplinari_Elenco_xTestata_conRegolamentoConcimazione il Flag_DisciplinarePrivato ha il seguente significato:
            '- 0: sia pubblici che privati
            '- 1: solo pubblici
            '- 2: solo privati
            'Il Flag_DisciplinarePrivato può assumere valori true o false, per cui con:
            '- True --> 0, quindi sia pubblici che privati
            '- False --> 1, qundi solo pubblici
            Dim Flag_Privato_Pubblico As Integer = 0
            Dim flag_DisciplinarePrivato = objConfigurazione_Siti.Leggi_Valore(0, "Flag_DisciplinarePrivato", "", "", objParametri_Server)
            If flag_DisciplinarePrivato = "" Then
                flag_DisciplinarePrivato = objConfigurazione_Siti.Leggi_Valore(0, "Flag_DisciplinarePrivato", "", "", objParametri_Super_Server)
            End If

            If flag_DisciplinarePrivato <> "" Then

                Select Case CBool(flag_DisciplinarePrivato)
                    Case True
                        Flag_Privato_Pubblico = 0
                    Case Else
                        Flag_Privato_Pubblico = 1
                End Select

            End If

            Dati = ObjDownloadWs.Leggi_Disciplinari_Elenco_xTestata_conRegolamentoConcimazione(
                                                                    Cod_Regolamento,
                                                                    Id_RcDpi,
                                                                    VEG_COD,
                                                                    Reg_Cod,
                                                                    Flag_Privato_Pubblico,
                                                                    Tipo_Testata,
                                                                    Data,
                                                                    Data,
                                                                    objParametri_Server.PivaSuperUser,
                                                                    ASG_Utente_Username_Crypt,
                                                                    ASG_Utente_Password_Crypt,
                                                                    strErr)


            If strErr = "" Then
                If Not IsNothing(Dati) AndAlso Dati.ToLower.IndexOf("errore") < 0 Then
                    XmlDocumento.LoadXml(Dati)
                    XmlNodo = XmlDocumento.GetElementsByTagName("Record")
                    If Not XmlNodo Is Nothing Then
                        For Each XmlElemento In XmlNodo
                            Dr = Dt.NewRow
                            Dr.Item("Dpi_Cod") = XmlElemento.GetAttribute("cod_regolamento")
                            Dr.Item("Dpi_Des") = XmlElemento.GetAttribute("nomeesteso")
                            Dr.Item("IdRcdpi") = XmlElemento.GetAttribute("id_rcdpi")
                            Dr.Item("Grfi_Cod") = XmlElemento.GetAttribute("grfi_cod")
                            Dr.Item("Flag_Protetto") = XmlElemento.GetAttribute("flag_protetto")
                            Dr.Item("PubblicoPrivato") = XmlElemento.GetAttribute("pubblicoprivato")
                            Dr.Item("DesRcdpi") = XmlElemento.GetAttribute("nome")
                            Dr.Item("PUA_Regolamento_Cod") = XmlElemento.GetAttribute("pua_regolamento_cod")
                            Dr.Item("PUA_Regolamento_Tipo") = XmlElemento.GetAttribute("pua_regolamento_tipo")
                            Dt.Rows.Add(Dr)
                        Next
                    End If
                End If
            End If

            If Not Dt Is Nothing Then
                Return Dt
            End If


        Catch ex As Exception

            Dati = "Si sono verificati errori in fase di chiamata al WebService Discliplinari!" & Chr(13) & ex.Message
            Throw New Exception(Dati)

        End Try

        Return Dt
    End Function



End Class
