Imports System.IO
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreModello
Imports AgronicaCoreVarieDAL
Imports AgronicaCoreGestioneRichieste
Imports System.Globalization
Imports AgronicaCoreUtentiDAL

Public Class headerHelper
    Inherits System.Web.UI.WebControls.Label
    Private _parametriHeader As ParametriHeader2022
    Private _logProvider As LogProvider = New LogProvider

    Public Sub New(parametriHeader As ParametriHeader2022)
        _parametriHeader = parametriHeader

    End Sub

    Private Function LoadUserControl() As String

        Lingua.Gias_InizializzaCultura_DaSession()

        Dim pageHolder As New Page()
        Dim header As New headerMenuUC
        header.AssemblyName = "AgronicaControlli_2010"
        header.ControlClassName = "headerMenuUC"
        header.ControlNamespace = "AgronicaControlli_2010"
        pageHolder.Controls.Add(header)

        Using output As New StringWriter()
            HttpContext.Current.Server.Execute(pageHolder, output, False)
            Return output.ToString()
        End Using

    End Function

    Private Property linkMenuBS2017 As String
        Get
            Dim objWebConfig As New AgroWebConfig
            Dim linkAgenda = objWebConfig.LinkAgronicaAgenda2010

            Return Replace(linkAgenda, "GestioneRichieste.aspx", "Menu/MenuBS_2017.aspx")
        End Get
        Set(value As String)

        End Set
    End Property



    Public Function RenderUserControl() As String

        Dim ComponentePrincipale As String = LoadUserControl()
        Lingua.Gias_InizializzaCultura_DaSession()
        Dim objParametri_Server As AgronicaCoreParametri = Nothing
        'Dim basePath As String = ""
        Dim piva As String = String.Empty
        Dim ragSoc As String = String.Empty

        If Not IsNothing(HttpContext.Current.Session("ASG_objParametri_Server")) Then
            objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")
        End If


        ' TODO AgronicaUma e Agenda usano entrambi ParametriAgenda. Va separato???

        Dim objParam As Object = Nothing
        Select Case _parametriHeader.sitoOspite
            Case Enum_SiteRedirector.Sito_AgronicaAgenda_2010, Enum_SiteRedirector.Sito_AgronicaUma
                objParam = New ParametriAgenda
            Case Enum_SiteRedirector.Sito_PianoConcimazione_2017
                objParam = New ParametriConcimazione_2017
                DirectCast(objParam, ParametriConcimazione_2017).Leggi()
            Case Enum_SiteRedirector.Sito_AgronicaStampe_2010
                objParam = New ParametriAgronicaStampe_2010
                DirectCast(objParam, ParametriAgronicaStampe_2010).Leggi()
            Case Enum_SiteRedirector.Sito_AgronicaSincronizzatore
                objParam = New ParametriSincronizzatore_2010
                DirectCast(objParam, ParametriSincronizzatore_2010).Leggi()
            Case Enum_SiteRedirector.Sito_AgronicaPianiCampionamento
                objParam = New ParametriPianidiCampionamento_2010
                DirectCast(objParam, ParametriPianidiCampionamento_2010).Leggi()
            Case Enum_SiteRedirector.Sito_AgronicaAnalisi_2010
                objParam = New ParametriAnalisi_2010
                DirectCast(objParam, ParametriAnalisi_2010).Leggi()
            Case Enum_SiteRedirector.Sito_AgronicaProfilazione
                objParam = New ParametriProfilazione_2010
                DirectCast(objParam, ParametriProfilazione_2010).Leggi()
            Case Enum_SiteRedirector.Sito_AgronicaPlanning
                objParam = New ParametriPlanning
                DirectCast(objParam, ParametriPlanning).Leggi()
            Case Enum_SiteRedirector.Sito_AgronicaAudit
                objParam = New ParametriAgronicaAuditPUA
                DirectCast(objParam, ParametriAgronicaAuditPUA).Leggi()


        End Select

        If Not IsNothing(objParam) Then
            Estrai_Piva_E_Rag_Soc(piva, ragSoc, objParam, objParametri_Server)
        End If

        ComponentePrincipale = ComponentePrincipale.Replace("PIVA_CORRENTE", piva)
        ComponentePrincipale = ComponentePrincipale.Replace("AZIENDA_CORRENTE", ragSoc.Replace("""", ""))

        ComponentePrincipale = ComponentePrincipale.Replace("PATH_LOGO", _parametriHeader.pathLogo)

        Dim testoPerBreadCrum = ""
        If _parametriHeader.testoBreadcrum.Contains("_") OrElse _parametriHeader.testoBreadcrum.Contains("~") Then
            testoPerBreadCrum = _parametriHeader.testoBreadcrum
        ElseIf _parametriHeader.testoBreadcrum.Length > 0 Then
            testoPerBreadCrum = "~" & _parametriHeader.testoBreadcrum
        End If
        testoPerBreadCrum = testoPerBreadCrum.Replace(Environment.NewLine, "").Replace(vbLf, "")
        ComponentePrincipale = ComponentePrincipale.Replace("REPLACE_breadcrum_Info", testoPerBreadCrum)

        Dim idSezione = 0
        If (_parametriHeader.idSezione <> 0) AndAlso (_parametriHeader.idSezione <> -1) Then
            idSezione = _parametriHeader.idSezione
        ElseIf (_parametriHeader.idSezioneSessione <> 0) AndAlso (_parametriHeader.idSezioneSessione <> -1) Then
            idSezione = _parametriHeader.idSezioneSessione
        End If
        ComponentePrincipale = ComponentePrincipale.Replace("REPLACE_IDSezione", idSezione)

        If _parametriHeader.funzioneIndietro <> "" Then
            ComponentePrincipale = ComponentePrincipale.Replace("REPLACE_azioneIndietro", _parametriHeader.funzioneIndietro)
        Else
            ComponentePrincipale = ComponentePrincipale.Replace("REPLACE_azioneIndietro", "")
        End If

        If _parametriHeader.idIndietro <> "" Then
            ComponentePrincipale = ComponentePrincipale.Replace("REPLACE_idIndietro", _parametriHeader.idIndietro)
        Else
            ComponentePrincipale = ComponentePrincipale.Replace("REPLACE_idIndietros", "")
        End If

        Dim apiController As New CoreApiControllerFactory()
        apiController.Inizializza(_parametriHeader.objParametri_Super_Server, _parametriHeader.objParametri_Server, _parametriHeader.sitoOspite)
        ComponentePrincipale = ComponentePrincipale.Replace("__pathCoreAPI__", apiController.LinkCoreApi.Trim)

        'If Debugger.IsAttached Then
        '    basePath = "http://localhost"
        'End If

        ComponentePrincipale = ComponentePrincipale.Replace("REPLACE_Menu_BS17", linkMenuBS2017)
        'ComponentePrincipale = ComponentePrincipale.Replace("__basePath__", basePath)
        ComponentePrincipale = ComponentePrincipale.Replace("__basePath__", _parametriHeader.PATHPATH_GIASBASE)

        Dim autenticaUtente = New AgronicaCoreUtentiDAL.AutenticaUtente
        Dim statoLicenza = autenticaUtente.Verifica_Validita_Permessi_E_Chiave_Licenza(_parametriHeader.objParametri_Utenti)
        Dim errore = statoLicenza.Permessi_Scaduti_O_In_Scadenza OrElse statoLicenza.Licenza_Scaduta_O_In_Scadenza

        ComponentePrincipale = ComponentePrincipale.Replace("REPLACE_FLAG_LICENZA", errore.ToString.ToLowerInvariant)
        ComponentePrincipale = ComponentePrincipale.Replace("REPLACE_TESTO_LICENZA_SCADUTA", statoLicenza.Messaggio)
        'ComponentePrincipale = ComponentePrincipale.Replace("__GiasBase__", _parametriHeader.PATHPATH_GIASBASE)

        Dim ci As CultureInfo = System.Threading.Thread.CurrentThread.CurrentUICulture
        Dim runTimeResourceSet As Object = My.Resources.headerMenuUC_ascx.ResourceManager.GetResourceSet(ci, True, True)
        For Each dictEntry As DictionaryEntry In runTimeResourceSet
            ComponentePrincipale = ComponentePrincipale.Replace(dictEntry.Key, dictEntry.Value)
        Next


        Dim app = Utilita_Compressione.RemoveWhitespaceFromHtml(ComponentePrincipale)
        Return app


    End Function

    Private Sub Estrai_Piva_E_Rag_Soc(
            ByRef piva As String,
            ByRef rag_soc As String,
            ByVal objParametriSito As Object,
            objParametri_Server As AgronicaCoreParametri)


        If IsNothing(objParametriSito) Then
            Exit Sub
        End If

        Try
            Dim pPiva = objParametriSito.GetType().GetProperty("Piva")
            If Not IsNothing(pPiva) Then
                piva = If(IsNothing(pPiva.GetValue(objParametriSito)), "", pPiva.GetValue(objParametriSito).ToString)
            End If

            Dim pRagSoc = objParametriSito.GetType().GetProperty("RagSoc")
            If Not IsNothing(pRagSoc) Then
                rag_soc = If(IsNothing(pRagSoc.GetValue(objParametriSito)), "", pRagSoc.GetValue(objParametriSito).ToString)
                If String.IsNullOrEmpty(rag_soc) Then
                    If Not String.IsNullOrEmpty(piva) Then
                        Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
                        pRagSoc.SetValue(objParametriSito, objImprese.RagSoc_from_Piva(piva, objParametri_Server))
                    End If
                End If
                If Not IsNothing(pRagSoc.GetValue(objParametriSito)) Then
                    rag_soc = pRagSoc.GetValue(objParametriSito).ToString
                End If
            End If
        Catch ex As Exception
            _logProvider.Scrivi_LOG(objParametri_Server, "headerHelper.Estrai_Piva_E_Rag_Soc()", ex.Message)
        End Try

    End Sub

    Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
        'Dim idDiv As String = "header2022" & Me.ClientID

        Dim componentePrincipale As String = RenderUserControl()
        writer.Write(componentePrincipale)
        MyBase.Render(writer)

    End Sub

End Class
