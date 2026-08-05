Imports System.Web.Services
Imports AgronicaCoreDataProvider
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
Public Class Visite_Categoria1
    Inherits System.Web.Services.WebService

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Leggi(ByVal objP_server As String) As RispostaStandard
        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim v_R As New AgronicaCoreVisiteBIZ.Visite_Categoria1_R
            Dim dt As DataTable = v_R.Leggi(objParametri_Server)

            'Creo una lista di oggetti con l'elenco delle categorie
            Dim JArrayListaOp As New JArray()
            For Each dr In dt.Rows
                JArrayListaOp.Add(New JObject(New JProperty("id_categoria1", dr.Item("id_categoria1")), New JProperty("nome", dr.Item("nome"))))
            Next

            'ritorno l'array trasformato in json 
            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            r.RispostaStringa = JsonConvert.SerializeObject(JArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Aggiungi(ByVal objP_server As String,
                                        ByVal Nome As String) As RispostaStandard

        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim v_W As New AgronicaCoreVisiteBIZ.Visite_Categoria1_W
            Dim res As String = v_W.Aggiungi(objParametri_Server, Nome)

            If res <> "" Then
                r.Errore = res
                Return r
            End If

            r.RispostaStringa = "Categoria1 salvata correttamente"
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Modifica(ByVal objP_server As String,
                                        ByVal ID_Categoria1 As Integer,
                                        ByVal Nome As String) As RispostaStandard

        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim v_W As New AgronicaCoreVisiteBIZ.Visite_Categoria1_W
            Dim res As String = v_W.Modifica(objParametri_Server, ID_Categoria1, Nome)

            If res <> "" Then
                r.Errore = res
                Return r
            End If

            r.RispostaStringa = "Categoria1 modificata correttamente"
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Cancella(ByVal objP_server As String,
                                        ByVal ID_Categoria1 As Integer) As RispostaStandard

        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim v_W As New AgronicaCoreVisiteBIZ.Visite_Categoria1_W
            Dim res As String = v_W.Cancella(objParametri_Server, ID_Categoria1)

            If res <> "" Then
                r.Errore = res
                Return r
            End If

            r.RispostaStringa = "Categoria1 eliminata correttamente"
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function

End Class