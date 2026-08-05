Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate



Imports System
Imports System.IO

Public Class Esporta_XSL_AllGridview
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


        Dim attachment As String = "attachment; filename=" & Session("Nome_xls") & ".xls"
        Response.ClearContent()
        Response.AddHeader("content-disposition", attachment)
        Response.ContentType = "application/vnd.ms-excel"
        Dim tab As String = ""

        Dim stw As New StringWriter()
        Dim htextw As New HtmlTextWriter(stw)

        Dim dtgr As New GridView
        Dim frm As New HtmlForm

        dtgr = CType(Session("GridView_xls"), GridView)



        dtgr.Visible = True

        frm.Controls.Add(dtgr)
        dtgr.RenderControl(htextw)

        'rimuovo le immagini
        Dim variabile As String
        variabile = stw.ToString()

        variabile = Replace(variabile, "<img src='../img/ok.png' />", "")
        variabile = Replace(variabile, "<img src='../img/ok.png'/>", "")
        variabile = Replace(variabile, "<img src='../img/no.png' />", "")
        variabile = Replace(variabile, "<img src='../img/no.png'/>", "")


        Response.Write(variabile)
        Response.End()

    End Sub



End Class