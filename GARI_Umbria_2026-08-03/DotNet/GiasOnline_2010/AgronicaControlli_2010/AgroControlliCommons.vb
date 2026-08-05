Imports System.Data

Imports AgronicaControlli_2010
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports Agronica.Helpers.GiasBase

Public Class AgroControlliCommons
    Inherits System.Web.UI.WebControls.WebControl
    Implements iAgronicaControlliCommons

    Friend _paginaOspite As Integer = 1
    Friend _LinkGiasBase As String
    Friend _GiasVersioneCorrente As String = String.Empty
    Public Property GiasVersioneCorrente As String Implements iAgronicaControlliCommons.GiasVersioneCorrente
        Get
            Return _GiasVersioneCorrente
        End Get
        Set(value As String)
            _GiasVersioneCorrente = value
        End Set
    End Property

    Public Property PaginaOspite As Integer
        Get
            Return _paginaOspite
        End Get
        Set(value As Integer)
            _paginaOspite = value
        End Set
    End Property

    Public ReadOnly Property PATH_GIASBASE As String Implements iAgronicaControlliCommons.PATH_GIASBASE
        Get
            GiasBaseHelper.Setta_Link_GiasBase("ASG_objParametri_Server", _LinkGiasBase)
            Return _LinkGiasBase
        End Get
    End Property

    Friend _BasePath As String = ""



    Protected Overridable Sub inizializza() Implements iAgronicaControlliCommons.inizializza


        If Debugger.IsAttached Then
            _BasePath = "http://localhost"
        End If

        Try
            _GiasVersioneCorrente = HttpContext.Current.Application("GiasVersioneCorrente")
        Catch ex As Exception
            _GiasVersioneCorrente = AgronicaCoreUtility.FileSystemHelper.NomeFileUnivoco("")
        End Try

    End Sub


    Friend Sub AppendCssToHeader(basePath As String, puntoInterrogativo As String, css As String, placeHolder As PlaceHolder) Implements iAgronicaControlliCommons.AppendCssToHeader
        Dim includeTemplate As String = "<link rel='stylesheet' type='text/css' href='{0}' />" & vbCrLf
        Dim include As New LiteralControl([String].Format(includeTemplate, basePath & css & puntoInterrogativo & _GiasVersioneCorrente))

        placeHolder.Controls.Add(include)

    End Sub

    Friend Function LeggiDaSessioneOppureDaConfigSiti(ByVal chiave As String, ByVal valoreDefault As String, ByVal TipoDB As agronicacoreparametri_tipoDB, Optional ByVal MemorizzaInSessioneDopoLettura As Boolean = True, Optional ByVal paramSessioneObjParametriValue As String = "") As String Implements iAgronicaControlliCommons.LeggiDaSessioneOppureDaConfigSiti

        Dim xSessioneObjParametri As String = ""
        Dim Prefisso As String = "ASG_"

        Select Case TipoDB
            Case agronicacoreparametri_tipoDB.SuperServer
                xSessioneObjParametri = "ASG_objParametri_Super_Server"
                Prefisso &= "SS_"

            Case agronicacoreparametri_tipoDB.Server
                xSessioneObjParametri = "ASG_objParametri_Server"
                Prefisso &= "M_"
        End Select

        If paramSessioneObjParametriValue <> "" Then
            xSessioneObjParametri = paramSessioneObjParametriValue
        End If

        If IsNothing(HttpContext.Current.Session(xSessioneObjParametri)) Then
            Return ""
        End If

        Dim objParametri = New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session(xSessioneObjParametri))

        Dim rval As String = HttpContext.Current.Session(Prefisso & chiave)

        If String.IsNullOrEmpty(rval) Then

            Dim xLeggiKendoCfg As New AgronicaCoreVarieDAL.Configurazione_Siti_R

            Dim xDtLeggi As DataTable = xLeggiKendoCfg.Leggi(0, chiave, "", "", objParametri)

            If xDtLeggi.Rows.Count > 0 Then
                rval = xDtLeggi.Rows(0)("Valore")
            Else
                rval = valoreDefault
            End If

            If MemorizzaInSessioneDopoLettura Then
                HttpContext.Current.Session(Prefisso & chiave) = rval
            End If

        End If

        Return rval

    End Function


End Class
