Imports System.ServiceModel
Imports AgronicaCoreVarieBIZ
Public Class Import_WS_Agea

    Public Function Importa_Fascicolo(cuaa As String,
                                      anno As Integer,
                                      ByRef ISWSRespAnagFascicolo15 As List(Of ISWSRespAnagFascicolo15),
                                      ByRef ISWSContoCorrente As List(Of ISWSContoCorrente),
                                      ByRef ISWSFabbricatoFS6 As List(Of ISWSFabbricatoFS6),
                                      ByRef ISWSMacchina As List(Of ISWSMacchina),
                                      ByRef ISWSTerritorioFS6 As List(Of ISWSTerritorioFS6),
                                      objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                      Optional ByVal linkEsterno As String = "",
                                      Optional ByVal userEsterno As String = "",
                                      Optional ByVal pwdEsterno As String = "",
                                      Optional ByVal ScaricaSoloTestata As Boolean = False) As RispostaStandard
        Dim r As New RispostaStandard

        Try

            Dim configSitiLeggi As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim link As String = ""
            Dim username As String = ""
            Dim password As String = ""

            If linkEsterno = "" AndAlso userEsterno = "" AndAlso pwdEsterno = "" Then

                If configSitiLeggi.Leggi_Valore(16, "AGEA_RealTime_Link", "", "", objParametri_Server) <> "" Then
                    link = configSitiLeggi.Leggi_Valore(16, "AGEA_RealTime_Link", "", "", objParametri_Server)
                End If

                If configSitiLeggi.Leggi_Valore(16, "AGEA_RealTime_Username", "", "", objParametri_Server) <> "" Then
                    username = configSitiLeggi.Leggi_Valore(16, "AGEA_RealTime_Username", "", "", objParametri_Server)
                End If

                If configSitiLeggi.Leggi_Valore(16, "AGEA_RealTime_Password", "", "", objParametri_Server) <> "" Then
                    password = configSitiLeggi.Leggi_Valore(16, "AGEA_RealTime_Password", "", "", objParametri_Server)
                End If


                If link = "" Then
                    link = "https://cooperazione.bluarancio.com/wsTOAST/services/OprCFascicoloFS6"
                End If

                If username = "" Then
                    username = "XXXYEOWIRQEWODDJ_BLUARANCIO_USERNAME"
                End If

                If password = "" Then
                    password = "XXXYEOWIRQEWODDJ_BLUARANCIO_PASSWORD"
                End If

            Else

                link = linkEsterno
                username = userEsterno
                password = pwdEsterno

            End If

            Dim soap = New SOAPAutenticazione
            soap.nomeServizio = "LeggiConsistenzaCFS6"
            soap.username = username
            soap.password = password
            Dim basicHttpBinding As New BasicHttpBinding()
            basicHttpBinding.Name = "iFascicoloBinding"
            basicHttpBinding.MaxReceivedMessageSize = Integer.MaxValue
            basicHttpBinding.Security.Mode = BasicHttpSecurityMode.Transport
            basicHttpBinding.SendTimeout = TimeSpan.MaxValue
            'Dim EndpointAddress As New EndpointAddress("https://cooptest.bluarancio.com/wsTOAST/services/OprCFascicoloFS6")
            Dim EndpointAddress As New EndpointAddress(link)
            Dim isc As New InterServiceClient(basicHttpBinding, EndpointAddress)

            Try
                soap.nomeServizio = "TrovaFascicoloFS6"
                Dim fascicolo = isc.TrovaFascicoloFS6(soap, cuaa)
                If fascicolo.ISWSResponse.codRet = "012" Then
                    For Each item In fascicolo.Items
                        Dim isws As New ISWSRespAnagFascicolo15
                        isws = CType(item, ISWSRespAnagFascicolo15)
                        ISWSRespAnagFascicolo15.Add(isws)
                    Next
                End If
            Catch ex As Exception

            End Try

            If ISWSRespAnagFascicolo15.Count > 0 AndAlso Not ScaricaSoloTestata Then

                Try
                    soap.nomeServizio = "LeggiContiCorrentiFS6"
                    Dim conticorrenti = isc.LeggiContiCorrentiFS6(soap, cuaa)
                    If conticorrenti.ISWSResponse.codRet = "012" Then
                        For Each item In conticorrenti.Items
                            Dim isws As New ISWSContoCorrente
                            isws = CType(item, ISWSContoCorrente)
                            ISWSContoCorrente.Add(isws)
                        Next
                    End If
                Catch ex As Exception

                End Try

                Try
                    soap.nomeServizio = "LeggiFabbricatiFS6"
                    Dim fabbricati6 = isc.LeggiFabbricatiFS6(soap, cuaa)
                    If fabbricati6.ISWSResponse.codRet = "012" Then
                        For Each item In fabbricati6.Items
                            Dim isws As New ISWSFabbricatoFS6
                            isws = CType(item, ISWSFabbricatoFS6)
                            ISWSFabbricatoFS6.Add(isws)
                        Next
                    End If
                Catch ex As Exception

                End Try

                Try
                    soap.nomeServizio = "LeggiMacchineFS6"
                    Dim macchine = isc.LeggiMacchineFS6(soap, cuaa)
                    If macchine.ISWSResponse.codRet = "012" Then
                        For Each item In macchine.Items
                            Dim isws As New ISWSMacchina
                            isws = CType(item, ISWSMacchina)
                            ISWSMacchina.Add(isws)
                        Next
                    End If
                Catch ex As Exception

                End Try

                Try
                    soap.nomeServizio = "LeggiManodoperaFS6"
                    Dim manodopera = isc.LeggiManodoperaFS6(soap, cuaa)
                Catch ex As Exception

                End Try

                Try
                    soap.nomeServizio = "LeggiConsistenzaFS6"
                    Dim consistenze = isc.LeggiConsistenzaFS6(soap, cuaa)
                    If consistenze.ISWSResponse.codRet = "012" Then
                        For Each item In consistenze.Items
                            Dim isws As New ISWSTerritorioFS6
                            isws = CType(item, ISWSTerritorioFS6)
                            ISWSTerritorioFS6.Add(isws)
                        Next
                    End If
                Catch ex As Exception

                End Try

            End If

            r.RispostaOK = True
            r.RispostaStringa = Newtonsoft.Json.JsonConvert.SerializeObject("")
        Catch ex As Exception
            r.RispostaOK = False
            r.RispostaStringa = ex.Message
        End Try

        Return r
    End Function

    Public Function Importa_FascicoloFS7(cuaa As String,
                                      anno As Integer,
                                      ByRef ISWSRespAnagFascicolo15 As List(Of ISWSRespAnagFascicolo15),
                                      ByRef ISWSContoCorrente As List(Of ISWSContoCorrente),
                                      ByRef ISWSFabbricatoFS6 As List(Of ISWSFabbricatoFS6),
                                      ByRef ISWSMacchina As List(Of ISWSMacchina),
                                      ByRef ISWSTerritorioFS6 As List(Of ISWSTerritorioFS6),
                                      objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                      Optional ByVal linkEsterno As String = "",
                                      Optional ByVal userEsterno As String = "",
                                      Optional ByVal pwdEsterno As String = "",
                                      Optional ByVal ScaricaSoloTestata As Boolean = False) As RispostaStandard
        Dim r As New RispostaStandard

        Try

            Dim configSitiLeggi As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim link As String = ""
            Dim username As String = ""
            Dim password As String = ""

            If linkEsterno = "" AndAlso userEsterno = "" AndAlso pwdEsterno = "" Then

                If configSitiLeggi.Leggi_Valore(16, "AGEA_RealTime_Link", "", "", objParametri_Server) <> "" Then
                    link = configSitiLeggi.Leggi_Valore(16, "AGEA_RealTime_Link", "", "", objParametri_Server)
                End If

                If configSitiLeggi.Leggi_Valore(16, "AGEA_RealTime_Username", "", "", objParametri_Server) <> "" Then
                    username = configSitiLeggi.Leggi_Valore(16, "AGEA_RealTime_Username", "", "", objParametri_Server)
                End If

                If configSitiLeggi.Leggi_Valore(16, "AGEA_RealTime_Password", "", "", objParametri_Server) <> "" Then
                    password = configSitiLeggi.Leggi_Valore(16, "AGEA_RealTime_Password", "", "", objParametri_Server)
                End If


                If link = "" Then
                    link = "https://cooperazione.bluarancio.com/wsTOAST/services/OprCFascicoloFS6"
                End If

                If username = "" Then
                    username = "XXXYEOWIRQEWODDJ_BLUARANCIO_USERNAME"
                End If

                If password = "" Then
                    password = "XXXYEOWIRQEWODDJ_BLUARANCIO_PASSWORD"
                End If

            Else

                link = linkEsterno
                username = userEsterno
                password = pwdEsterno

            End If

            Dim soap = New SOAPAutenticazione
            soap.nomeServizio = "LeggiSchedeCUAA"
            soap.username = username
            soap.password = password
            Dim basicHttpBinding As New BasicHttpBinding()
            basicHttpBinding.Name = "iFascicoloBinding"
            basicHttpBinding.MaxReceivedMessageSize = Integer.MaxValue
            basicHttpBinding.Security.Mode = BasicHttpSecurityMode.Transport
            basicHttpBinding.SendTimeout = TimeSpan.MaxValue
            'Dim EndpointAddress As New EndpointAddress("https://cooptest.bluarancio.com/wsTOAST/services/OprCFascicoloFS6")
            Dim EndpointAddress As New EndpointAddress(link)
            Dim isc As New InterServiceClient(basicHttpBinding, EndpointAddress)

            Try
                soap.nomeServizio = "LeggiSchedeCUAA"
                Dim richiestaSchede = New RichiestaSchede
                Dim ISWSCHEDE = New ISWSCHEDE
                ISWSCHEDE.AnnoCampagna = 2021
                ISWSCHEDE.CUAA = cuaa
                richiestaSchede.ISWSCHEDE = ISWSCHEDE
                Dim fascicolo = isc.LeggiSchedeCUAA(soap, richiestaSchede)

                If fascicolo.ISWSResponse.codRet = "012" Then
                    For Each item In fascicolo.Items
                        Dim isws As New ISWSRespAnagFascicolo15
                        isws = CType(item, ISWSRespAnagFascicolo15)
                        ISWSRespAnagFascicolo15.Add(isws)
                    Next
                End If
            Catch ex As Exception

            End Try

            If ISWSRespAnagFascicolo15.Count > 0 AndAlso Not ScaricaSoloTestata Then

                Try
                    soap.nomeServizio = "LeggiContiCorrentiFS6"
                    Dim conticorrenti = isc.LeggiContiCorrentiFS6(soap, cuaa)
                    If conticorrenti.ISWSResponse.codRet = "012" Then
                        For Each item In conticorrenti.Items
                            Dim isws As New ISWSContoCorrente
                            isws = CType(item, ISWSContoCorrente)
                            ISWSContoCorrente.Add(isws)
                        Next
                    End If
                Catch ex As Exception

                End Try

                Try
                    soap.nomeServizio = "LeggiFabbricatiFS6"
                    Dim fabbricati6 = isc.LeggiFabbricatiFS6(soap, cuaa)
                    If fabbricati6.ISWSResponse.codRet = "012" Then
                        For Each item In fabbricati6.Items
                            Dim isws As New ISWSFabbricatoFS6
                            isws = CType(item, ISWSFabbricatoFS6)
                            ISWSFabbricatoFS6.Add(isws)
                        Next
                    End If
                Catch ex As Exception

                End Try

                Try
                    soap.nomeServizio = "LeggiMacchineFS6"
                    Dim macchine = isc.LeggiMacchineFS6(soap, cuaa)
                    If macchine.ISWSResponse.codRet = "012" Then
                        For Each item In macchine.Items
                            Dim isws As New ISWSMacchina
                            isws = CType(item, ISWSMacchina)
                            ISWSMacchina.Add(isws)
                        Next
                    End If
                Catch ex As Exception

                End Try

                Try
                    soap.nomeServizio = "LeggiManodoperaFS6"
                    Dim manodopera = isc.LeggiManodoperaFS6(soap, cuaa)
                Catch ex As Exception

                End Try

                Try
                    soap.nomeServizio = "LeggiConsistenzaFS6"
                    Dim consistenze = isc.LeggiConsistenzaFS6(soap, cuaa)
                    If consistenze.ISWSResponse.codRet = "012" Then
                        For Each item In consistenze.Items
                            Dim isws As New ISWSTerritorioFS6
                            isws = CType(item, ISWSTerritorioFS6)
                            ISWSTerritorioFS6.Add(isws)
                        Next
                    End If
                Catch ex As Exception

                End Try

            End If

            r.RispostaOK = True
            r.RispostaStringa = Newtonsoft.Json.JsonConvert.SerializeObject("")
        Catch ex As Exception
            r.RispostaOK = False
            r.RispostaStringa = ex.Message
        End Try

        Return r
    End Function

End Class
