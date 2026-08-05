Imports System.IO
Imports AgronicaCoreVarieDAL

Public Class bootstrap_WaitFrame
    Inherits System.Web.UI.WebControls.Label

    Protected Overrides Sub OnPreRender(e As EventArgs)
        'Page.ClientScript.RegisterClientScriptInclude("bootstrap-waitframe", Page.ClientScript.GetWebResourceUrl(Me.[GetType](), "AgronicaControlli_2010.bootstrap-WaitFrame.js"))

    End Sub

    Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)

        Dim ComponentePrincipale As String
        getResource("AgronicaControlli_2010.bootstrap-WaitFrame.js", ComponentePrincipale)
        Dim rotella As String = Page.ClientScript.GetWebResourceUrl(Me.[GetType](), "AgronicaControlli_2010.rotella.gif")
        ComponentePrincipale = ComponentePrincipale.Replace("__img__", rotella)
        Dim app As String = Utilita_Compressione.RemoveWhitespaceFromHtml("<script>" & ComponentePrincipale & "</script>")

        writer.Write(app)

        MyBase.Render(writer)
    End Sub



    Private Shared Sub getResource(ByVal risorsa As String, ByRef str_e As String)

        'Dim readStream  New FileStream("c:\testBinary.dat", FileMode.Open)
        Dim readBinary As New BinaryReader(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(risorsa))

        str_e = readBinary.ReadChars(1)
        str_e = ""

        Dim length As Integer = readBinary.BaseStream.Length
        Dim allData As Byte() = readBinary.ReadBytes(length)

        str_e = System.Text.Encoding.UTF8.GetString(allData)

        readBinary.Close()

    End Sub

End Class
