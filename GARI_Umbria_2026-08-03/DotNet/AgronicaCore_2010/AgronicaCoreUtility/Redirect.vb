Imports System.Web.UI

Public Class Redirect

    '##########################################################################################
    'la querystring vuole il ?
    'per le altre page new windows andare in AgronicaCoreDataProvider.UtilityProvider
    Public Shared Sub Page_NewWindow_xUpdatePanel(ByRef objPage As System.Web.UI.Page, _
                                                ByVal NomePaginaAspx As String, _
                                                ByVal QueryString As String, _
                                                Optional ByVal Pagina_Titolo As String = "GiasOnline", _
                                                Optional ByVal Pagina_Height As Integer = 700, _
                                                Optional ByVal Pagina_Width As Integer = 1000, _
                                                Optional ByVal Pagina_Top As Integer = 0, _
                                                Optional ByVal Pagina_Left As Integer = 0, _
                                                Optional ByVal Menubar As String = "yes", _
                                                Optional ByVal Resizable As String = "yes", _
                                                Optional ByVal Scrollbars As String = "yes", _
                                                Optional ByVal ID_UpdatePanel As String = "FORM1")

        Dim strJS As String

        strJS = AgronicaCoreDataProvider.UtilityProvider.Page_NewWindow_RitornaJavascript( _
                                                objPage, _
                                                NomePaginaAspx, _
                                                QueryString, _
                                                Pagina_Titolo, _
                                                , , , , , , )

        ScriptManager.RegisterClientScriptBlock(CType(AgronicaCoreUtility.Ricerca.FindControlIterative(objPage, ID_UpdatePanel), UpdatePanel), _
                                                CType(AgronicaCoreUtility.Ricerca.FindControlIterative(objPage, ID_UpdatePanel), UpdatePanel).GetType(), _
                                                "jQuery_{0}", strJS, False)

    End Sub


End Class
