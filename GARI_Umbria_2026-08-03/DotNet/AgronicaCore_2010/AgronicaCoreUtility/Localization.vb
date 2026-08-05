Imports System.Globalization
Imports System.IO
Imports System.Reflection
Imports System.Resources
Imports System.Text
Imports System.Web
Imports System.Xml
Imports AgronicaCoreDataProvider
Imports Newtonsoft.Json

Public Class Localization

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="files">nome del file, da percorso web (es.: grafica/App_LocalResrouces/gis.js.resx) oppure come risorsa incorporata in assembly (es.: agronicacontrolligis.dll/AgronicaControlliGIS.mappaBS2.ascx)</param>
    ''' <param name="linguaGias"></param>
    ''' <param name="filepath">esempio: HttpContext.Current.Server.MapPath("~\" & fileconLingua)</param>
    ''' <returns></returns>
    Public Shared Function RitornaRisorse(ByVal files As String, linguaGias As Lingua, filepath As String) As String

        Dim stringaRisposta As New StringBuilder

        Lingua.Gias_InizializzaCultura_DaSession()

        stringaRisposta.Append("{")

        Dim flagConcatena As Boolean = False

        If files.ToLower().Contains(".dll") Then

            Dim assemblySplitPath As String() = files.Split("/"c)

            ' VAnni: 20/2/2020: per capire quali sono le risorse incorporate disponibli copiare in "espressione di controllo 1": ca.GetManifestResourceNames()
            Dim ca As Assembly = Assembly.LoadFile(Path.Combine(filepath, assemblySplitPath(0)))

            Dim crm As New ResourceManager(assemblySplitPath(1), ca)
            Dim contentAssembly As ResourceSet = crm.GetResourceSet(CultureInfo.CurrentUICulture, True, True)

            For Each item As DictionaryEntry In contentAssembly
                If flagConcatena Then stringaRisposta.Append(",")

                stringaRisposta.AppendFormat("""{0}"":""{1}""", item.Key, jSon.Escape(item.Value.ToString()))

                flagConcatena = True
            Next

        Else

            Dim radiceLingua As String = If(linguaGias.Lingua_cod <> 1, linguaGias.CodiceISO, String.Empty)
            Dim fileconLingua As String = filepath.Substring(0, filepath.Length - 4) & radiceLingua & ".resx"
            fileconLingua = fileconLingua.Replace("..", ".").Replace("/", "\")

            Dim document As New XmlDocument()
            document.Load(fileconLingua)

            Dim nodes As XmlNodeList = document.SelectNodes("//data")

            For Each node As XmlNode In nodes
                If flagConcatena Then stringaRisposta.Append(",")

                Dim resourceKey As String = node.Attributes("name").Value
                Dim resourceValue As String = node.SelectSingleNode("value").InnerText.Trim()
                stringaRisposta.AppendFormat("""{0}"":""{1}""", resourceKey, jSon.Escape(resourceValue))

                flagConcatena = True
            Next

        End If

        stringaRisposta.Append("}")

        Return stringaRisposta.ToString()

    End Function


End Class
