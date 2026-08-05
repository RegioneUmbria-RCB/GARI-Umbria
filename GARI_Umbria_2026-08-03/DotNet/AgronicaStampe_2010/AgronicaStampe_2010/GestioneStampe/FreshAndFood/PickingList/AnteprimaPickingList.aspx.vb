


Imports AgronicaCoreStampeDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Imports System.Web.Services

Public Class AnteprimaPickingList
    Inherits System.Web.UI.Page

    Private objParametri_server As AgronicaCoreParametri

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Session("ASG_objParametri_Server") Is Nothing Then
            Response.Redirect("~/Custom500.aspx")
        End If

        objParametri_server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        RenderReport()
    End Sub

    Private Sub RenderReport()

        Dim id_Agenda As String = Request.QueryString("i")
        If Not String.IsNullOrEmpty(id_Agenda) Then
            id_Agenda = Stringa_Decodifica(id_Agenda, AgroKey_EncoderDecoder, Server)
        End If


        Dim dsR As DataSet
        Dim leggi As New AgronicaCoreStampeDAL.FF_Etichette_R
        dsR = leggi.LeggiPickingList_Full( _
            id_Agenda, _
            "", _
            "", _
            "", _
            "B", _
            3, _
            "", _
            "", _
            objParametri_server _
        )

        Dim rpt As New PickingList
        rpt.SetDataSource(dsR)


        Session("Report") = rpt

        Response.Redirect("~/GestioneStampe/VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server))


    End Sub

End Class