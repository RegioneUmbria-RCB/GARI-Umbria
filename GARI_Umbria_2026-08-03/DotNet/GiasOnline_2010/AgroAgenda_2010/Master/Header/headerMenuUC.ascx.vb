Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.Menu
Imports AgronicaCoreModelsSTD.profilazione

Public Class headerMenuUC
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'caricaInformazioniUtenti()
        caricaInformazioniUtenti()
    End Sub



    Private Sub caricaInformazioniUtenti()
        'aggiungere try catch

        Dim risposta = CoreApiControllerFactory.Instance.Call_Endpoint(Of rispostaStandard(Of Utente), IControllerWrapper)(New MenuControllerWrapper, "informazioniUtente")
        Dim utente As Utente = risposta.RispostaStringa

        Me.Nome_RagioneSociale.InnerText = utente.Nome
        Me.emailField.InnerText = utente.Email
        Me.visibilita.InnerText = utente.Visibilita
        Me.ultimoAccesso.InnerText = utente.UltimoAccesso

        Me.Azienda2.InnerText = "Da guardare dove prendere"
        'Me.Azienda2.InnerText = "ABOCA S.P.A. SOCIET&Agrave; AGRICOLA - SANSEPOLCRO"

    End Sub

    Public Shared Function caricaMenu() As rispostaStandard(Of Utente)
        Dim menu = CoreApiControllerFactory.Instance.Call_Endpoint(Of rispostaStandard(Of Utente), IControllerWrapper)(New MenuControllerWrapper, "informazioniUtente")

        Return menu

    End Function

    Public Shared Function LeggiAlberoMenu(ByVal InData As Object) As rispostaStandard(Of AlberoMenu)

    End Function

    Public Shared Function aggiornaAttivitaNavigazioneAziende(ByVal InData As Object) As rispostaStandard(Of String)

    End Function
    Public Shared Function ultimeAziendeSelezionate(ByVal InData As Object) As rispostaStandard(Of List(Of Impresa))

    End Function





End Class