Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreUtentiDAL

Public Class ReportPercorsi
    Inherits System.Web.UI.Page

    Private objParametriAgenda As ParametriAgenda
    Public permessi As PermessiUtente
    Public MostraReportRaccolteGIS As Boolean = False

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        permessi = New PermessiUtente()

        Master().Lbl_Titolo.Text = "Report Percorsi"

        ' Setto la visibilità dei bottoni in Master
        If Not Master.flag_MenuBS_2017 Then
            Master.flag_pag_Anagrafica = True
        End If
        

        objParametriAgenda = Session("objParametriAgenda")

        MostraReportRaccolteGIS = permessi.getPermesso(enum_Security_Attivita.ReportRaccolteGIS).Lettura


    End Sub

    Private Sub ReportPercorsi_Init(sender As Object, e As EventArgs) Handles Me.Init
        AddHandler CType(Me.Master, AgendaBootstrap).ImgBtn_AnnullaTutto.Click, AddressOf Me.AnnullaTutto
    End Sub


    Private Sub AnnullaTutto()

        ' VAnni: 8/3/2018: da verificare i due livelli nella chiamata standard ("trova redirect corretto")
        Response.Redirect("../../menu/menubs_agenda_nuovo.aspx")

    End Sub
End Class