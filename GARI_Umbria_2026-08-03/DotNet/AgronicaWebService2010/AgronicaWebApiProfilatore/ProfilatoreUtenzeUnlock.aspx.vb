Imports AgronicaCoreDataProvider
Imports AgronicaCoreProfilazioneBIZ
Imports AgronicaCoreVarieBIZ

Public Class ProfilatoreUtenzeUnlock
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim t As String
        t = Request.QueryString("t")
        ProceduraDiSblocco(t)
    End Sub


    Private Sub ProceduraDiSblocco(t As String)

        Const ProceduraDiSbloccoErrore As String = "Si è verificato un errore durante la procedura di accreditamento. Riprovare più tardi."
        Const ProceduraDiSbloccoOk As String = "La la procedura di accreditamento è stata eseguita correttamente. Riceverete una email con le istruzioni per proseguire. Potete chiudere questa pagina"

        Dim ObjParametri_Utenti As AgronicaCoreParametri = Nothing
        Dim ObjParametri_Server As AgronicaCoreParametri = Nothing
        Dim ObjParametri_SuperServer As AgronicaCoreParametri = Nothing

        ProfilatoreUtenzeHelper.GetObjParametri("", "", ObjParametri_Server, ObjParametri_Utenti, ObjParametri_SuperServer)

        Dim pS As New AgronicaCoreProfilazioneBIZ.ProfilazioneProcedure
        Dim r As RispostaStandard =
            pS.ProceduraDiSbloccoUtenteViaToken(t, ObjParametri_Utenti, ObjParametri_Server, ObjParametri_SuperServer)


        If r.RispostaOK Then

            Dim o As New AccountManagerEstesaRequest

            o.username = ObjParametri_Utenti.UtenteUsername

            Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Read
            Dim dt_Utente = objUtenti.Leggi_DatiUtente_e_DatiSuperUser(o.username, "", "", 0, 0, Nothing, 0, False, 0, "", "", ObjParametri_Utenti)

            If dt_Utente.Rows.Count > 0 Then

                If dt_Utente.Rows(0)("Utente_Flag_Azienda_Persona") = 1 Then
                    o.ragsoc = dt_Utente.Rows(0)("Rag_Soc")
                Else
                    o.nome = dt_Utente.Rows(0)("Nome")
                    o.cognome = dt_Utente.Rows(0)("Cognome")
                End If

                o.password = dt_Utente.Rows(0)("Password")
                o.email = dt_Utente.Rows(0)("email")

                Dim UtenteCodiceFiscaleLetto As String = dt_Utente.Rows(0)("CodFisc")
                ProfilatoreUtenzeHelper.AccountManagerEstesaImpostaValoriObjParametri(
                    o.username, UtenteCodiceFiscaleLetto, ObjParametri_Utenti, ObjParametri_Server, ObjParametri_SuperServer
                )

                Dim TipoMail_Chiave As String = AgronicaCoreUtility.FileSystemHelper.NomeFileUnivoco("") & "_" & o.email

                Dim fullFileNameWithPathTestoEmail As String =
                Hosting.HostingEnvironment.ApplicationPhysicalPath & "\Testi\HTML_EMAIL_RISPOSTA_PostAttivazione.htm"

                Dim AccodaEmailUserPassword As New AgronicaCoreProfilazioneBIZ.ProfilazioneMailing
                Dim emailProfilazioneTestoBody As String = ProfilatoreUtenzeHelper.GetEmailProfilazioneTestoBody(o, "", fullFileNameWithPathTestoEmail, ObjParametri_Server)
                Dim EmailProfilazioneOggetto As String = ProfilatoreUtenzeHelper.GetEmailProfilazioneOggetto()
                AccodaEmailUserPassword.AccodaEmailNotifica(o.email, EmailProfilazioneOggetto, emailProfilazioneTestoBody, TipoMail_Chiave, ObjParametri_Server)

            Else
                r.RispostaOK = False
            End If


        End If


        If r.RispostaOK Then
            r.RispostaStringa = ProceduraDiSbloccoOk
        Else

            r.RispostaStringa = ProceduraDiSbloccoErrore
        End If

        lblEsito.Text = r.RispostaStringa

    End Sub




End Class