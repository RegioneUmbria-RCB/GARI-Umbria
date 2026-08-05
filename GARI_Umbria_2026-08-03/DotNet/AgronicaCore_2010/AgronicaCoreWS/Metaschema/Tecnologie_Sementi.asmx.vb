Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider


' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class Tecnologie_Sementi
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaCombo_TecnologieSementi(objP_server As String) As RispostaStandard

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Tecnologie_Sementi.CaricaCombo_TecnologieSementi()"

        Dim r As New RispostaStandard

        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)


            Dim objCom As New AgronicaCoreMetaSchemaDAL.TecnologieSementi
            Dim Dt As DataTable = objCom.Leggi(objParametri_Server)

            Dim JArrayLista As New JArray()
            Dt.ToExpandoObject.Select(Of JObject)(Function(dr) New JObject(New JProperty("Cod_TecnologiaSementi", dr.Item("Cod_TecnologiaSementi")), New JProperty("Descrizione", dr.Item("Descrizione")))) _
                .ToList.ForEach(Sub(jOb)
                                    JArrayLista.Add(jOb)
                                End Sub)

            r.RispostaStringa = JsonConvert.SerializeObject(JArrayLista, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "[" & NomeRoutine & "] : " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

End Class