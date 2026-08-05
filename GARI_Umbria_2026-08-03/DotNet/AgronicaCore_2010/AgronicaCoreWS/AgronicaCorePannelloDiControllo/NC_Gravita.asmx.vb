Imports System.Web.Services
Imports AgronicaCorePannelloDiControlloBIZ
Imports AgronicaCoreDataProvider
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreVarieBIZ

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
Public Class NC_ScriptService_Gravita
    Inherits System.Web.Services.WebService

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Leggi(objP_server As String) As RispostaStandard
        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

        Dim NC_R As New NC_Gravita_R
        Dim lista As List(Of NC_Gravita) = NC_R.leggi_NC_Gravita(objParametri_Server)

        Dim listaJSON As New JArray()
        For Each i As NC_Gravita In lista
            listaJSON.Add(New JObject(New JProperty("nome", i.Nome), New JProperty("id_gravita", i.ID_Gravita)))
        Next

        r.RispostaStringa = JsonConvert.SerializeObject(listaJSON, Formatting.None)
        r.RispostaOK = True

        Return r

    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Modifica(objP_server As String, nome As String, id As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

        Dim NC_W As New NC_Gravita_W
        Dim err As String = NC_W.modifica(objParametri_Server, id, nome)

        If err = "" Then
            r = Leggi(objP_server)
        Else
            r.RispostaOK = False
            r.Errore = err
        End If

        Return r

    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Aggiungi(objP_server As String, nome As String) As RispostaStandard
        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

        'Salvo l'elemento
        Dim NC_W As New NC_Gravita_W
        Dim err As String = NC_W.aggiungi(objParametri_Server, nome)

        If err = "" Then
            r = Leggi(objP_server)
        Else
            r.RispostaOK = False
            r.Errore = err
        End If

        Return r

    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Cancella(objP_server As String, id As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

        Dim NC_W As New NC_Gravita_W
        Dim err As String = NC_W.cancella(objParametri_Server, id)

        If err = "" Then
            r = Leggi(objP_server)
        Else
            r.RispostaOK = False
            r.Errore = err
        End If

        Return r

    End Function

End Class