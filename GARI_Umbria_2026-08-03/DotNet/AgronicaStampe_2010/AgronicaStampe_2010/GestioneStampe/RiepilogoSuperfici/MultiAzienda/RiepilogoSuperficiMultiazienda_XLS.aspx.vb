Imports AgronicaCoreDataProvider

Public Class RiepilogoSuperficiMultiazienda_XLS
    Inherits System.Web.UI.Page

    Dim objParametri_Server As AgronicaCoreParametri
    Dim ListPiva() As String
    Dim ListSa_cod() As Integer
    Dim ListAppezza() As Integer
    Dim ListId_reg() As Integer
    Dim GroupParam() As Boolean
    Dim DataValiditaProgetto As Date
    Dim DT As DataTable
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'La Pagina deve essere visualizzata come un foglio Excel
        Response.Clear()
        Response.ContentType = "application/vnd.ms-excel"
        Response.AddHeader("Content-Disposition", "inline; filename = RiepilogoSuperficieManutenzione.xls")


        '##############################################################
        '#####  Verifico Credenziali di Accesso  ######################
        '##############################################################

        '----- Verifico che l'utente sia autenticato

        If Session("ASG_objParametri_Server") Is Nothing Then
            Response.Redirect("~/Custom500.aspx")
        End If

        'inizializzazione oggetti objParametri_Utenti e objParametri_Server
        '---
        objParametri_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        '---


        ListPiva = If(Session("ListPiva"), New String() {})
        ListSa_cod = If(Session("ListSa_cod"), New Integer() {})
        ListAppezza = If(Session("ListAppezza"), New Integer() {})
        ListId_reg = If(Session("ListId_reg"), New Integer() {})

        Dim colStampaStr As String = Request.Form()("msColonneStampa")
        Dim colStampaArr As String() = New String() {}
        If colStampaStr IsNot Nothing AndAlso colStampaStr <> "" Then
            colStampaArr = colStampaStr.Split(",")
        End If

        'options.success([
        '    { id: "PartitaIva", nome: "Partita IVA" },
        '    { id: "RagSoc", nome: "Ragione Sociale" },
        '    { id: "CentroAziendale", nome: "Centro Aziendale" },
        '    { id: "SpecieVegetale", nome: "Specie Vegetale" },
        '    { id: "Varieta", nome: "Varietà" },
        '    { id: "SupImpianto", nome: "Superficie Impianto" },
        '    { id: "NumeroPiante", nome: "Numero Piante" },
        '    { id: "Copertura", nome: "Copertura" },
        '    { id: "Finalita", nome: "Finalità" },
        ']);
        ReDim GroupParam(8)
        GroupParam(0) = colStampaArr.Contains("PartitaIva")
        GroupParam(1) = colStampaArr.Contains("RagSoc")
        GroupParam(2) = colStampaArr.Contains("CentroAziendale")
        GroupParam(3) = colStampaArr.Contains("SpecieVegetale")
        GroupParam(4) = colStampaArr.Contains("Varieta")
        GroupParam(5) = colStampaArr.Contains("SupImpianto")
        GroupParam(6) = colStampaArr.Contains("NumeroPiante")
        GroupParam(7) = colStampaArr.Contains("Copertura")
        GroupParam(8) = colStampaArr.Contains("Finalita")

        Dim dataRif As String = Request.Form()("dpDataRif")
        If dataRif IsNot Nothing AndAlso dataRif <> "" Then
            DataValiditaProgetto = dataRif
        Else
            DataValiditaProgetto = Date.Today
        End If

        LeggiSupMultiAzienda()

        Genera_Excel()

    End Sub


    Private Sub LeggiSupMultiAzienda()

        'chiama query di lettura
        Dim Reg_Impianti_Read As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read

        DT = Reg_Impianti_Read.RiepilogoSuperficiMultiazienda(ListPiva, ListSa_cod, ListAppezza, ListId_reg, GroupParam, DataValiditaProgetto, "", "", objParametri_Server)
        DT.Columns.Remove("Partita_IVA")
        DT.Columns("PivaReale").ColumnName = "Partita_IVA"

    End Sub

    Private Sub Genera_Excel()

        Dim stw As New IO.StringWriter
        Dim htextw As New HtmlTextWriter(stw)

        Dim dtgr As New DataGrid
        dtgr.DataSource = DT
        dtgr.DataBind()

        dtgr.RenderControl(htextw)
        Response.Write(stw.ToString())
        Response.End()

    End Sub

End Class