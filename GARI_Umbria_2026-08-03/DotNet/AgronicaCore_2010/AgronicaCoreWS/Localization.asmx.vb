Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports System.Xml
Imports AgronicaCoreUtentiDAL
Imports System.Globalization

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
Public Class Localization
    Inherits System.Web.Services.WebService


    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function RitornaRisorseBS(ByVal objP_Server As String, ByVal files As String, ByVal linguaRichiesta As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_Server)


        Try

            'Inserire il codice QUI..

            r.RispostaOK = True
            r.RispostaStringa = RitornaRisorse(files, linguaRichiesta)

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaFileRisorse(ByVal objP_Server As String,
                                     ByVal objP_Utenti As String,
                                     ByVal files As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_Server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_Utenti)

        Try

            Dim linguaCodiceISO As String = "it"
            Dim leggiLingua As New Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            If Not IsNothing(dtLingua) AndAlso dtLingua.Rows.Count > 0 Then
                linguaCodiceISO = dtLingua.Rows(0)("CodiceISO")
            End If

            Dim ci As CultureInfo = New CultureInfo(linguaCodiceISO)
            System.Threading.Thread.CurrentThread.CurrentUICulture = ci
            Dim linguaSession As Lingua = New Lingua With {.CodiceISO = ci.TwoLetterISOLanguageName, .Lingua_cod = objParametri_Server.Lingua_Cod}
            Dim p As String = ""
            If Not files.ToLower().Contains(".dll") Then
                p = HttpContext.Current.Server.MapPath("~\" & files)
            Else
                p = HttpContext.Current.Server.MapPath(".") & "\bin"
            End If
            r.RispostaStringa = AgronicaCoreUtility.Localization.RitornaRisorse(files, linguaSession, p)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try

        Return r
    End Function

    Private Function RitornaRisorse(ByVal files As String, ByVal linguaRichiesta As String) As String

        Dim stringaRisposta As New StringBuilder

        Dim radiceLingua As String = ""

        If linguaRichiesta.ToLower <> "it-it" Then
            radiceLingua = linguaRichiesta
        End If


        stringaRisposta.Append("{")

        Dim fileconLingua As String = Left(files, files.Length - 4) & radiceLingua & ".resx"
        fileconLingua = fileconLingua.Replace("..", ".")
        'Write the class (name of the without any extensions) as the object

        Dim filePath As String = HttpContext.Current.Server.MapPath("~\" & fileconLingua)
        Dim document As New XmlDocument()
        document.Load(filePath)
        Dim nodes As XmlNodeList = document.SelectNodes("//data")
        Dim flag2 As Boolean = False
        For Each node As XmlNode In nodes
            If flag2 Then stringaRisposta.Append(",")

            Dim attr As XmlAttribute = node.Attributes("name")
            Dim resourceKey As String = attr.Value
            stringaRisposta.Append("""" & resourceKey & """:""" & node.InnerText.Trim & """")
            flag2 = True
        Next
        stringaRisposta.Append("}")

        Return stringaRisposta.ToString
    End Function


End Class