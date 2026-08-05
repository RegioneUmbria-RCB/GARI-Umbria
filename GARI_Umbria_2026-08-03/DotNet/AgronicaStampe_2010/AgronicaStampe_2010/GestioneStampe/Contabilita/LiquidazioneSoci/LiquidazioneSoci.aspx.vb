Imports AgronicaCoreDataProvider
Imports AgronicaCoreStampeDAL
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate


Public Class LiquidazioneSoci
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        GeneraReport()
    End Sub

    Private objParametri_server As AgronicaCoreParametri

    Private Sub GeneraReport()



        objParametri_server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))


        Dim id_Agenda As String = Request.QueryString("i")
        If Not String.IsNullOrEmpty(id_Agenda) Then
            id_Agenda = Stringa_Decodifica(id_Agenda, AgroKey_EncoderDecoder, Server)
        End If


        Dim dt As New DataTable

        Dim letturaDati As New AgronicaCoreContabDAL.FF_LiquidazioneSoci_R

        dt = letturaDati.Leggi_Operazione_Agenda( _
            id_Agenda, _
            "", _
            "NO_TOTALI", _
            "", _
            objParametri_server _
        )

        Dim rpt As New Liquidazione_Bolletta
        rpt.SetDataSource(dt)

        Session("Report") = rpt

        Response.Redirect("~/GestioneStampe/VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server))


    End Sub

End Class