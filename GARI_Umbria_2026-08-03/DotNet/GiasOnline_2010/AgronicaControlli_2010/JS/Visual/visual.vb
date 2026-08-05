Public Class visual
    Inherits System.Web.UI.WebControls.Label


    Public Property VersionVisual As String

    Protected Overrides Sub OnPreRender(e As EventArgs)

        Dim pVisual As String = javascript_helper.IncludeJavaScript(Page.ClientScript, "AgronicaControlli_2010.visual" & VersionVisual & ".js", True)
        Dim pVisualSetup As String = javascript_helper.IncludeJavaScript(Page.ClientScript, "AgronicaControlli_2010.visual_agronica.setup" & VersionVisual & ".js", True)
        Dim plazy As String = javascript_helper.IncludeJavaScript(Page.ClientScript, "AgronicaControlli_2010.lazyload.js", True)

        Dim pD3 As String = javascript_helper.IncludeJavaScript(Page.ClientScript, "AgronicaControlli_2010.d3.v3.js", True)
        Dim pFlot As String = javascript_helper.IncludeJavaScript(Page.ClientScript, "AgronicaControlli_2010.jquery.flot.js", True)
        Dim pFlotCategories As String = javascript_helper.IncludeJavaScript(Page.ClientScript, "AgronicaControlli_2010.jquery.flot.categories.js", True)
        Dim pFlotOrderBars As String = javascript_helper.IncludeJavaScript(Page.ClientScript, "AgronicaControlli_2010.jquery.flot.orderbars.js", True)
        Dim pFlotPyramid As String = javascript_helper.IncludeJavaScript(Page.ClientScript, "AgronicaControlli_2010.jquery.flot.pyramid.js", True)
        Dim pFlotStack As String = javascript_helper.IncludeJavaScript(Page.ClientScript, "AgronicaControlli_2010.jquery.flot.stack.js", True)
        Dim pExcanvas As String = javascript_helper.IncludeJavaScript(Page.ClientScript, "AgronicaControlli_2010.excanvas.js", True)
        Dim pVisualMaps As String = javascript_helper.IncludeJavaScript(Page.ClientScript, "AgronicaControlli_2010.visual.maps.js", True)

        Dim pJQuery As String = Page.ClientScript.GetWebResourceUrl(Me.[GetType](), "AgronicaControlli_2010.jquery-1.11.2.min.js")

        Dim str As String = _
            "var pVisual='" & pVisual & "';" & vbCrLf & _
            "var pVisualSetup='" & pVisualSetup & "';" & vbCrLf & _
            "var pLazy='" & plazy & "';" & vbCrLf & _
            "var pD3='" & pD3 & "';" & vbCrLf & _
            "var pFlot='" & pFlot & "';" & vbCrLf & _
            "var pFlotCategories='" & pFlotCategories & "';" & vbCrLf & _
            "var pFlotOrderBars='" & pFlotOrderBars & "';" & vbCrLf & _
            "var pFlotPyramid='" & pFlotPyramid & "';" & vbCrLf & _
            "var pFlotStack='" & pFlotStack & "';" & vbCrLf & _
            "var pExcanvas='" & pExcanvas & "';" & vbCrLf & _
            "var pJQuery='" & pJQuery & "';" & vbCrLf & _
            "var pVisualMaps='" & pVisualMaps & "';" & vbCrLf



        Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "visual", str, True)




        'Page.ClientScript.RegisterClientScriptInclude("visual" & VersionVisual, _
        '            Page.ClientScript.GetWebResourceUrl(Me.[GetType](), "AgronicaControlli_2010.lazyvisualsetup" & VersionVisual & ".js"))




    End Sub
End Class
