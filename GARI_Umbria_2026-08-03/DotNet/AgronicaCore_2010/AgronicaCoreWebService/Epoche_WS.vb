Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class Epoche_WS

    Public Shared Function EpocheDPI_Elenco(Dpi_Cod As Integer,
                                            Id_Rcdpi As Integer,
                                            Tipo_Testata As Integer,
                                            Modulo As Integer,
                                    ByRef objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                    ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                    ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                    ByRef strErr As String) As DataTable

        Dim DtRisultati As New DataTable
        DtRisultati.Columns.Add(New DataColumn("codice", GetType(Integer)))
        DtRisultati.Columns.Add(New DataColumn("descrizione", GetType(String)))
        DtRisultati.TableName = "EpocheDPI"

        Dim Dati As String = ""

        If Tipo_Testata = enum_Disciplinare_Tipo_Testata.Difesa OrElse Tipo_Testata = enum_Disciplinare_Tipo_Testata.Diserbo Then

            Try
                Dim agroWs As String
                Dim objConfigurazione_Siti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                agroWs = objConfigurazione_Siti.Leggi_Valore(0, "GiasOnline_WS_Disciplinari_AgroWS_Disciplinari", "", "", objParametri_Server)
                If agroWs = "" Then
                    agroWs = objConfigurazione_Siti.Leggi_Valore(0, "GiasOnline_WS_Disciplinari_AgroWS_Disciplinari", "", "", objParametri_Super_Server)
                End If

                Dim ObjDownloadWs = New WS_Disciplinari.AgroWS_Disciplinari
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

                Select Case Tipo_Testata

                    Case enum_Disciplinare_Tipo_Testata.Difesa

                        Dati = ObjDownloadWs.Leggi_EpocheDifesa(Id_RCDPI:=Id_Rcdpi,
                                                      Cod_Disciplinare:=Dpi_Cod,
                                                      Modulo:=Modulo,
                                                      NomeUtente:=ASG_Utente_Username_Crypt,
                                                      Password:=ASG_Utente_Password_Crypt)

                    Case enum_Disciplinare_Tipo_Testata.Diserbo

                        Dati = ObjDownloadWs.Leggi_EpocheDiserbo(Id_RCDPI:=Id_Rcdpi,
                                                        Cod_Disciplinare:=Dpi_Cod,
                                                        NomeUtente:=ASG_Utente_Username_Crypt,
                                                        Password:=ASG_Utente_Password_Crypt)
                End Select

                If Not IsNothing(Dati) AndAlso Dati.ToLower.IndexOf("errore") < 0 Then

                    Dim XmlDocumento As New System.Xml.XmlDocument
                    Dim XmlNodo As System.Xml.XmlNodeList
                    Dim XmlElemento As System.Xml.XmlElement
                    XmlDocumento.LoadXml(Dati)
                    XmlNodo = XmlDocumento.GetElementsByTagName("Record")

                    If Not XmlNodo Is Nothing Then
                        For Each XmlElemento In XmlNodo

                            Dim Dr = DtRisultati.NewRow

                            Select Case Tipo_Testata

                                Case 0 'difesa
                                    Dr.Item("codice") = CInt(XmlElemento.GetAttribute("modulo"))
                                    Dr.Item("descrizione") = XmlElemento.GetAttribute("descrizioneperiododa")

                                Case 1 'diserbo
                                    Dr.Item("codice") = CInt(XmlElemento.GetAttribute("ep_cod"))
                                    Dr.Item("descrizione") = XmlElemento.GetAttribute("descrizione")

                            End Select

                            DtRisultati.Rows.Add(Dr)
                        Next
                    End If

                End If

            Catch ex As Exception
                Dati = "Si sono verificati errori in fase di chiamata al WebService EpocheDPI!" & Chr(13) & ex.Message
                Throw New Exception(Dati)
            End Try

        End If

        Return DtRisultati

    End Function

End Class
