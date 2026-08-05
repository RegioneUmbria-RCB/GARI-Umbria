Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports Agronica.Helpers.GiasBase
Public Class GST_Stampe_Menu
    Inherits System.Web.UI.Page


    Private _linkGiasBase As String = ""

    '#############################################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load


        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        GiasBaseHelper.Setta_Link_GiasBase("ASG_objParametri_Server", _linkGiasBase)

        If Me.Rbl_InterferenzeInterne.SelectedValue = "0" Then
            Session("Visualizza_Interferenze_Interne") = True
        Else
            Session("Visualizza_Interferenze_Interne") = False
        End If

        Me.Master.ImpostaVisibilitaPulsantiMaster(TipiEnumerativiSementieri.enum_Pannelli.StAMPA)
        ImgBtn_Stampa_RPT.ImageUrl = _linkGiasBase + "Agronica/AB_Immagini/icone32/Stampa32c.ico"
        ImgBtn_Stampa_LOG.ImageUrl = _linkGiasBase + "Agronica/AB_Immagini/icone32/Stampa32c.ico"

    End Sub


    '#############################################################################################################
    Private Sub ImgBtn_Stampa_LOG_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Stampa_LOG.Click

        If Me.Chk_LOG_FlagRigheVuote.Checked = True Then
            Session("AgroSementi_FlagRigheVuote") = True
        Else
            Session("AgroSementi_FlagRigheVuote") = False
        End If

        If Me.Chk_LOG_FlagRiepilogo.Checked = True Then
            Session("AgroSementi_FlagRiepilogo") = True
        Else
            Session("AgroSementi_FlagRiepilogo") = False
        End If

        Response.Redirect("LOG_Interferenze/LOG_Interferenze.aspx")

    End Sub


    '#############################################################################################################
    Private Sub ImgBtnAnnulla_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs)

        Response.Redirect("../GST_Interferenze/Interferenze_Init.aspx")

    End Sub


    '#############################################################################################################
    Private Sub ImgBtn_Stampa_RPT_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Stampa_RPT.Click

        Response.Redirect("Notifica_Interferenze/Notifica_Interferenze.aspx")

    End Sub

    Private Sub ImgBtn_Interferenze_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs)
        Response.Redirect("../GST_Interferenze/GST_Interferenze_Init.aspx")
    End Sub

    Private Sub ImgBtn_Filtro_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs)
        Response.Redirect("../GST_Filtro/GST_Filtro_Impianti.aspx")
    End Sub

    Private Sub GST_Stampe_Menu_Init(sender As Object, e As EventArgs) Handles Me.Init

        AddHandler CType(Me.Master, GSTBootstrap).GSTBootstrap_ImgBtn_Filtro.Click, AddressOf ImgBtn_Filtro_Click
        AddHandler CType(Me.Master, GSTBootstrap).GSTBootstrap_ImgBtn_Interferenze.Click, AddressOf ImgBtn_Interferenze_Click

        AddHandler CType(Me.Master.Master, AgendaBootstrap).ImgBtn_AnnullaTutto.Click, AddressOf Me.AnnullaTutto
    End Sub


    Private Sub AnnullaTutto()
        Response.Redirect("../GST_Menu/GST_Menu.aspx")
    End Sub

End Class