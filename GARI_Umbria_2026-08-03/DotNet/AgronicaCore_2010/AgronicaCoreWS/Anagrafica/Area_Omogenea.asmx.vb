Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreContabObject
Imports System.IO
Imports System.Text
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDTOStd.InData.Anagrafica


' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
Public Class Area_Omogenea
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiAree_NG(ByVal InData As CoreWS_Generic(Of LeggiAree)) As RispostaStandard

        Dim r As New RispostaStandard()

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim objR As New AgronicaCoreAnagrafeDAL.Area_Omogenea_R
            Dim dt As DataTable = objR.Leggi(InData.InData.piva, InData.InData.areaCod, "", "", objParametri_Server)

            Dim jarr = New JArray()

            If InData.InData.PrimaRiga_Flag = True Then

                Dim jobj As JObject = New JObject
                jobj.Add(New JProperty("Piva", InData.InData.piva))
                jobj.Add(New JProperty("Area_Cod", InData.InData.PrimaRiga_Value))
                jobj.Add(New JProperty("Area_Des", InData.InData.PrimaRiga_Text))

                jobj.Add(New JProperty("Tessitura_Cod", 0))
                jobj.Add(New JProperty("Altimetria", ""))
                jobj.Add(New JProperty("SO", 0))
                jobj.Add(New JProperty("TipoZona", ""))

                jarr.Add(jobj)

            End If


            For Each dr As DataRow In dt.Rows
                Dim jobj As JObject = New JObject

                jobj.Add(New JProperty("Piva", dr.Item("Piva")))
                jobj.Add(New JProperty("Area_Cod", dr.Item("Area_Cod")))
                jobj.Add(New JProperty("Area_Des", dr.Item("Area_Des")))
                jobj.Add(New JProperty("Tessitura_Cod", dr.Item("Tessitura_Cod")))
                jobj.Add(New JProperty("Altimetria", dr.Item("Altimetria")))
                jobj.Add(New JProperty("SO", dr.Item("SO")))
                jobj.Add(New JProperty("TipoZona", dr.Item("TipoZona")))

                jarr.Add(jobj)
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(jarr, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiAree(ByVal objP_server As String,
                              ByVal PrimaRiga_Flag As Boolean,
                              ByVal PrimaRiga_Text As String,
                              ByVal PrimaRiga_Value As String,
                              ByVal piva As String,
                              ByVal areaCod As Integer
                              ) As RispostaStandard

        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim objR As New AgronicaCoreAnagrafeDAL.Area_Omogenea_R
            Dim dt As DataTable = objR.Leggi(piva, areaCod, "", "", objParametri_Server)

            Dim jarr = New JArray()

            If PrimaRiga_Flag = True Then

                Dim jobj As JObject = New JObject
                jobj.Add(New JProperty("Piva", piva))
                jobj.Add(New JProperty("Area_Cod", PrimaRiga_Value))
                jobj.Add(New JProperty("Area_Des", PrimaRiga_Text))

                jobj.Add(New JProperty("Tessitura_Cod", 0))
                jobj.Add(New JProperty("Altimetria", ""))
                jobj.Add(New JProperty("SO", 0))
                jobj.Add(New JProperty("TipoZona", ""))

                jarr.Add(jobj)

            End If


            For Each dr As DataRow In dt.Rows
                Dim jobj As JObject = New JObject

                jobj.Add(New JProperty("Piva", dr.Item("Piva")))
                jobj.Add(New JProperty("Area_Cod", dr.Item("Area_Cod")))
                jobj.Add(New JProperty("Area_Des", dr.Item("Area_Des")))
                jobj.Add(New JProperty("Tessitura_Cod", dr.Item("Tessitura_Cod")))
                jobj.Add(New JProperty("Altimetria", dr.Item("Altimetria")))
                jobj.Add(New JProperty("SO", dr.Item("SO")))
                jobj.Add(New JProperty("TipoZona", dr.Item("TipoZona")))

                jarr.Add(jobj)
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(jarr, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function
End Class