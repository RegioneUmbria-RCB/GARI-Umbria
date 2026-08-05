
Imports System.Drawing
Imports AgroAgenda_2010.TipiEnumerativiSementieri
Imports AgronicaCoreDataProvider

Public Class GSTBootstrap
    Inherits System.Web.UI.MasterPage

    Public ReadOnly Property GSTBootstrap_Pannello_BTN_Filtro As Panel
        Get
            Return Pannello_BTN_Filtro
        End Get
    End Property

    Public ReadOnly Property GSTBootstrap_Pannello_BTN_Risultato As Panel
        Get
            Return Pannello_BTN_Risultato
        End Get
    End Property

    Public ReadOnly Property GSTBootstrap_Pannello_BTN_Stampa As Panel
        Get
            Return Pannello_BTN_Stampa
        End Get
    End Property

    Public ReadOnly Property GSTBootstrap_Pannello_BTN_Interferenze As Panel
        Get
            Return Pannello_BTN_Interferenze
        End Get
    End Property


    Public ReadOnly Property GSTBootstrap_ImgBtn_Interferenze As ImageButton
        Get
            Return ImgBtn_Interferenze
        End Get
    End Property


    Public ReadOnly Property GSTBootstrap_ImgBtn_Filtro As ImageButton
        Get
            Return ImgBtn_Filtro
        End Get
    End Property


    Public ReadOnly Property GSTBootstrap_ImgBtn_Risultato As ImageButton
        Get
            Return ImgBtn_Risultato
        End Get
    End Property



    Public ReadOnly Property GSTBootstrap_ImgBtn_Stampa As ImageButton
        Get
            Return ImgBtn_Stampa
        End Get
    End Property

    Private _LinkGiasBase As String = String.Empty
    Public ReadOnly Property PATH_GIASBASE As String
        Get
            Return _LinkGiasBase
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        _LinkGiasBase = Me.Master.PATH_GIASBASE

        Me.Master.flag_MostraBtnIndietro = True
        ImgBtn_Filtro.ImageUrl = _LinkGiasBase + "Agronica/AB_Immagini/Icone32/Check32b.ico"
        ImgBtn_Risultato.ImageUrl = _LinkGiasBase + "Agronica/AB_Immagini/icone32/Terreni.ico"
        ImgBtn_Stampa.ImageUrl = _LinkGiasBase + "Agronica/AB_Immagini/icone32/Stampa32c.ico"
        GSTBootstrap_ImgBtn_Interferenze.ImageUrl = _LinkGiasBase + "Agronica/AB_Immagini/icone32/IndiciMaturita32.ico"

    End Sub

    Private Sub GSTBootstrap_Init(sender As Object, e As EventArgs) Handles Me.Init

    End Sub




    Public Sub ImpostaVisibilitaPulsantiMaster(PannelloAttivo As enum_Pannelli)

        GSTBootstrap_Pannello_BTN_Filtro.Visible = False
        GSTBootstrap_Pannello_BTN_Risultato.Visible = False
        GSTBootstrap_Pannello_BTN_Interferenze.Visible = False
        GSTBootstrap_Pannello_BTN_Stampa.Visible = False

        GSTBootstrap_Pannello_BTN_Filtro.BackColor = Color.Transparent
        GSTBootstrap_Pannello_BTN_Risultato.BackColor = Color.Transparent
        GSTBootstrap_Pannello_BTN_Interferenze.BackColor = Color.Transparent
        GSTBootstrap_Pannello_BTN_Stampa.BackColor = Color.Transparent

        Select Case PannelloAttivo
            Case enum_Pannelli.FILTRO
                GSTBootstrap_Pannello_BTN_Filtro.BackColor = Color.Gold
                GSTBootstrap_Pannello_BTN_Filtro.Visible = True
                GSTBootstrap_Pannello_BTN_Risultato.Visible = True

            Case enum_Pannelli.RISULTATO
                GSTBootstrap_Pannello_BTN_Risultato.BackColor = Color.Gold
                GSTBootstrap_Pannello_BTN_Filtro.Visible = True
                GSTBootstrap_Pannello_BTN_Risultato.Visible = True
                GSTBootstrap_Pannello_BTN_Interferenze.Visible = True

            Case enum_Pannelli.INTERFERENZE
                GSTBootstrap_Pannello_BTN_Interferenze.BackColor = Color.Gold
                GSTBootstrap_Pannello_BTN_Filtro.Visible = True
                GSTBootstrap_Pannello_BTN_Interferenze.Visible = True
                GSTBootstrap_Pannello_BTN_Stampa.Visible = True

            Case enum_Pannelli.StAMPA
                GSTBootstrap_Pannello_BTN_Stampa.BackColor = Color.Gold
                GSTBootstrap_Pannello_BTN_Filtro.Visible = True
                GSTBootstrap_Pannello_BTN_Interferenze.Visible = True
                GSTBootstrap_Pannello_BTN_Stampa.Visible = True

        End Select

    End Sub


End Class