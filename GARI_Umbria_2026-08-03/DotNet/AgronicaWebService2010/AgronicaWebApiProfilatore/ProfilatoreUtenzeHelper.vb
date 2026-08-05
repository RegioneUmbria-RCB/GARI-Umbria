Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreProfilazioneBIZ

Public Class ProfilatoreUtenzeHelper


    Public Shared Function GetEmailProfilazioneOggetto() As String
        Return "Attivazione Servizio Banca Dati Profitosan"
    End Function

    Private Shared Function DurataServizioDaStringa(s As String) As String
        If Not String.IsNullOrEmpty(s) AndAlso s.Contains("-") Then
            Dim d As String() = s.Split("-")
            Return d(1)
        Else
            Return ""
        End If

    End Function



    Public Shared Function GetEmailProfilazioneTestoBody(
        o As AccountManagerEstesaRequest,
        tokenPerURL As String,
        fullFileNameWithPath As String,
        objparametri_server As AgronicaCoreParametri
    ) As String



        Dim testoDelServizioAcquistato As String = "Accesso via web "
        Dim durataServizio As String = DurataServizioDaStringa(o.tipologia)

        If Not String.IsNullOrEmpty(o.tipologia) AndAlso o.tipologia.ToLower.Contains("app") Then
            testoDelServizioAcquistato = "e con App "
        End If

        Dim BaseUrlForMailConfirm As String = "https://www.agronica.it/"
        Try
            BaseUrlForMailConfirm = ConfigurationManager.AppSettings("BaseUrlForMailConfirm")
        Catch ex As Exception

        End Try

        If Not fullFileNameWithPath.Contains("HTML_EMAIL_RISPOSTA_PostAttivazione") Then
            testoDelServizioAcquistato &= ", durata del servizio: " & durataServizio
        End If


        If Not fullFileNameWithPath.Contains("HTML_EMAIL_RISPOSTA_PostAttivazione") Then
            BaseUrlForMailConfirm &= Hosting.HostingEnvironment.ApplicationVirtualPath & "/ProfilatoreUtenzeUnlock.aspx"
            BaseUrlForMailConfirm &= "?t=" & tokenPerURL
        Else
            Dim leggiConfigurazioneSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            BaseUrlForMailConfirm = leggiConfigurazioneSiti.Leggi_Valore(6, "LinkProfitosan", "", "", objparametri_server)
            If BaseUrlForMailConfirm = "" Then
                BaseUrlForMailConfirm = "https://www.profitosan.it"
            End If

        End If

        Dim titoloMail As String = "Gentile "

        Dim HtmlTemplateStr As String = My.Computer.FileSystem.ReadAllText(fullFileNameWithPath)
        HtmlTemplateStr = HtmlTemplateStr.Replace(
            "%Titolo%", titoloMail).Replace(
            "%Nome%", o.nome).Replace(
            "%Cognome%", o.cognome).Replace(
            "%RagSoc%", o.ragsoc).Replace(
            "%Username%", o.username).Replace(
            "%Password%", o.password).Replace(
            "%ServizioAcquistato%", testoDelServizioAcquistato).Replace(
            "%LinkAttivazione%", BaseUrlForMailConfirm)

        Return HtmlTemplateStr

    End Function


    Public Shared Sub AccountManagerEstesaImpostaValoriObjParametri(UtenteUsername As String, UtenteCodiceFiscale As String, ByRef ObjParametri_Utenti As AgronicaCoreParametri, ByRef ObjParametri_Server As AgronicaCoreParametri, ByRef ObjParametri_SuperServer As AgronicaCoreParametri)
        ObjParametri_Server.UtenteUsername = UtenteUsername
        ObjParametri_Utenti.UtenteUsername = UtenteUsername
        ObjParametri_SuperServer.UtenteUsername = UtenteUsername

        ObjParametri_Server.UtenteCodFiscale = UtenteCodiceFiscale
        ObjParametri_Utenti.UtenteCodFiscale = UtenteCodiceFiscale
        ObjParametri_SuperServer.UtenteCodFiscale = UtenteCodiceFiscale

        ObjParametri_Server.UsernameOperazione = UtenteCodiceFiscale
        ObjParametri_Utenti.UsernameOperazione = UtenteCodiceFiscale
        ObjParametri_SuperServer.UsernameOperazione = UtenteCodiceFiscale
    End Sub

    Public Shared Sub GetObjParametri(
        ByVal utente As String,
        ByVal utenteCodiceFiscale As String,
        ByRef objParametri_Server As AgronicaCoreParametri,
        ByRef objParametri_Utenti As AgronicaCoreParametri,
        ByRef objParametri_SuperServer As AgronicaCoreParametri
    )


        Dim oTokenParametri As AgronicaCoreUtentiDAL.TokenParametri =
            Newtonsoft.Json.JsonConvert.DeserializeObject(Of AgronicaCoreUtentiDAL.TokenParametri)(cfgTokenParametri)

        Dim objParametriHLP As New AgronicaCoreParametri_Helper


        objParametri_SuperServer = objParametriHLP.Crea_ObjParametri(
                AGRODATAINIZIO,
                AGRODATAFINE,
                AgronicaCore_Flag_CancellazioneLogica,
                AgronicaCore_Flag_Visibilita,
                AgronicaCore_DirectoryLOG,
                AgronicaCore_FileNameLOG,
                SuperUserUsername,
                PivaSuperUser,
                utente,
                utenteCodiceFiscale,
                StringaConnessione_SuperServer
        )

        Dim stringaConnessione As String

        Dim objAgronicaCore As New DataProvider
        stringaConnessione = objAgronicaCore.FindConnessione_Su_Ini_O_Superserver(PathFileINI, oTokenParametri.idDB_Server, objParametri_SuperServer)

        objParametri_Server = objParametriHLP.Crea_ObjParametri(
                AGRODATAINIZIO,
                AGRODATAFINE,
                AgronicaCore_Flag_CancellazioneLogica,
                AgronicaCore_Flag_Visibilita,
                AgronicaCore_DirectoryLOG,
                AgronicaCore_FileNameLOG,
                SuperUserUsername,
                PivaSuperUser,
                utente,
                utenteCodiceFiscale,
                stringaConnessione
        )


        stringaConnessione = objAgronicaCore.FindConnessione_Su_Ini_O_Superserver(PathFileINI, oTokenParametri.idDB_Utenti, objParametri_SuperServer)
        objParametri_Utenti = objParametriHLP.Crea_ObjParametri(
                AGRODATAINIZIO,
                AGRODATAFINE,
                AgronicaCore_Flag_CancellazioneLogica,
                AgronicaCore_Flag_Visibilita,
                AgronicaCore_DirectoryLOG,
                AgronicaCore_FileNameLOG,
                SuperUserUsername,
                PivaSuperUser,
                utente,
                utenteCodiceFiscale,
                stringaConnessione
        )


    End Sub

End Class
