Imports System.Data
Imports AgronicaCoreDataProvider
Imports AgronicaCoreModelsSTD.PersonalizzazioniGraficheCliente
Imports AgronicaCoreUtility
Imports Newtonsoft.Json

Public Class UtilityPersonalizzazioniGraficheCliente

    Const LOGO_STAMPE_FOOTER_DEFAULT As String = "\AB_Immagini\Logo\agronica_logo_nopayoff - 5,50mm (usato nelle stampe).jpg"
    Const LOGO_STAMPE_FOOTER_DEFAULT_PianoConcimazione As String = "\PC_Stampe\agronica_logo_nopayoff - 5,50mm (usato nelle stampe).jpg"
    Const STRING_DEFAULT_PRE_LOGO As String = "Stampato con software GIAS di "
    Const STRING_DEFAULT_POST_LOGO As String = "www.agronica.it -  assistenza@agronica.it"
    Shared Function LeggiPersonalizzazioniGraficheCliente(objParametriServer As AgronicaCoreParametri,
                                                           objParametriSuperServer As AgronicaCoreParametri) As PersonalizzazioniGraficheCliente

        Try
            Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim DTConfigSiti As DataTable = Nothing

            'leggiamo prima dal DB Server
            If objParametriServer IsNot Nothing Then
                DTConfigSiti = objConfigSiti.Leggi(0, "PersonalizzazioniGraficheCliente", "", "", objParametriServer)
            End If

            'se non è presente nel DB Server, leggo dal SuperServer
            If (IsNothing(DTConfigSiti) OrElse DTConfigSiti.Rows.Count = 0 OrElse DTConfigSiti.Rows(0).Item("Valore") = "") AndAlso objParametriSuperServer IsNot Nothing Then
                DTConfigSiti = objConfigSiti.Leggi(0, "PersonalizzazioniGraficheCliente", "", "", objParametriSuperServer)
            End If

            If Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 Then
                Dim valore = DTConfigSiti.Rows(0).Item("Valore")

                If valore IsNot Nothing AndAlso valore IsNot DBNull.Value AndAlso valore <> "" Then
                    Return JsonConvert.DeserializeObject(Of PersonalizzazioniGraficheCliente)(valore.ToString)
                End If
            End If

            Return Nothing

        Catch ex As Exception
            'Log di errore consigliato
            Return Nothing
        End Try

    End Function

    'Public Shared Sub ValorizzaObjParametri(ByRef objParamServer As AgronicaCoreParametri)

    'End Sub

    Shared Function TrovaLogoFooterStampe(ByVal persGrafiche As PersonalizzazioniGraficheCliente,
                                   ByRef logErrori As String,
                                    ByRef objParametri_Server As AgronicaCoreParametri) As ConfigurazioneLoghiStampe

        Dim res As New ConfigurazioneLoghiStampe
        Dim path As String = ""
        Dim mostraLogoFooter As Integer = -1
        Dim nascondiFooter As String = ""
        Dim defaultpath = HttpContext.Current.Server.MapPath("~") & LOGO_STAMPE_FOOTER_DEFAULT
        Dim defaultpathPianoConcimazione = HttpContext.Current.Server.MapPath("~") & LOGO_STAMPE_FOOTER_DEFAULT_PianoConcimazione
        Dim objGestioneLoghi As New GestioneLoghiFooterStampe
        Dim logoBianco As Byte() = objGestioneLoghi.creaLogoBianco()
        Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
        Dim pathLogo As String = objWebConfig.Path_Directory_Loghi_Cliente
        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim DTConfigSiti = objConfigSiti.Leggi(0, "Path_Directory_Loghi_Cliente", "", "", objParametri_Server)

        If Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 Then
            Dim valore As String = DTConfigSiti.Rows(0).Item("Valore")
            If valore <> pathLogo Then
                pathLogo = valore
            End If
        End If

        If persGrafiche IsNot Nothing AndAlso persGrafiche.MostraLogoStampeFooter IsNot Nothing Then
            mostraLogoFooter = persGrafiche.MostraLogoStampeFooter

            res.TestoPreLogo = ""
            res.TestoPostLogo = ""

            Try

                Try

                    Select Case mostraLogoFooter
                        Case 0
                            res.LogoStampe = logoBianco
                        Case 1 'mostra default
                            If IO.File.Exists(defaultpath) Then
                                res.LogoStampe = My.Computer.FileSystem.ReadAllBytes(defaultpath)
                            ElseIf IO.File.Exists(defaultpathPianoConcimazione) Then
                                res.LogoStampe = My.Computer.FileSystem.ReadAllBytes(defaultpathPianoConcimazione)
                            Else
                                res.LogoStampe = logoBianco
                            End If
                            res.TestoPostLogo = STRING_DEFAULT_POST_LOGO
                            res.TestoPreLogo = STRING_DEFAULT_PRE_LOGO
                        Case 2 'mostra personalizzato
                            If Not String.IsNullOrEmpty(persGrafiche.TestoPostLogoFooterStampe) Then
                                res.TestoPostLogo = persGrafiche.TestoPostLogoFooterStampe
                            End If
                            If Not String.IsNullOrEmpty(persGrafiche.TestoPreLogoFooterStampe) Then
                                res.TestoPreLogo = persGrafiche.TestoPreLogoFooterStampe
                            End If

                            If Not String.IsNullOrEmpty(persGrafiche.LogoFooterStampe) Then
                                path = IO.Path.Combine(pathLogo, persGrafiche.LogoFooterStampe)

                                If IO.File.Exists(path) Then
                                    res.LogoStampe = My.Computer.FileSystem.ReadAllBytes(path)
                                Else
                                    res.LogoStampe = logoBianco
                                End If
                            Else
                                res.LogoStampe = logoBianco
                            End If
                        Case Else
                            res.LogoStampe = logoBianco
                    End Select

                Catch ex As Exception
                    logErrori &= "Carica Logo Footer Stampe. Caricamento logo cliente: " & ex.Message
                End Try

            Catch ex As Exception
                logErrori &= "Carica Logo Footer Stampe: " & ex.Message
            End Try
        Else
            If IO.File.Exists(defaultpath) Then
                res.LogoStampe = My.Computer.FileSystem.ReadAllBytes(defaultpath)
            ElseIf IO.File.Exists(defaultpathPianoConcimazione) Then
                res.LogoStampe = My.Computer.FileSystem.ReadAllBytes(defaultpathPianoConcimazione)
            Else
                res.LogoStampe = logoBianco
            End If
            res.TestoPostLogo = STRING_DEFAULT_POST_LOGO
            res.TestoPreLogo = STRING_DEFAULT_PRE_LOGO
        End If

        Return res

    End Function

    Public Class ConfigurazioneLoghiStampe
        Public Property LogoStampe As Byte()
        Public Property TestoPreLogo As String
        Public Property TestoPostLogo As String
    End Class

End Class
