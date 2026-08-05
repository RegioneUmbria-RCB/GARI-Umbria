Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreModelsSTD.baseClass

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
Public Class Imputazioni_Fasi
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Imputazioni_Fasi_APP(ByVal piva As String,
                                        ByVal objP_super_server As String,
                                        ByVal objP_server As String,
                                        ByVal objP_utenti As String) As RispostaStandard

        Dim r As New RispostaStandard()

        Try

            Dim objParametri_SuperServer As AgronicaCoreDataProvider.AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            If objP_server = "" Then
                r.Errore = "objP_server non valorizzato"
                Return r
            End If

            Dim leggi As New AgronicaCoreContabDAL.Imputazioni_Fasi_R

            Dim Dt_Imputazioni_Fasi As DataTable = leggi.Leggi_APP(piva, "", "", objParametri_Server)


            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(Dt_Imputazioni_Fasi, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_TipiImputazioni_NG(ByVal piva As String,
                                        ByVal objP_super_server As String,
                                        ByVal objP_server As String,
                                        ByVal objP_utenti As String) As RispostaStandard

        Dim r As New RispostaStandard()
        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objImp As New AgronicaCoreContabDAL.Imputazioni_R

            Dim dt = objImp.Leggi(piva, 0, "", "", objParametri_Server)
            'Dim list = dt.AsEnumerable.Select(Function(row) New BaseCodeDescr(row("Imputazione_Cod"), row("Imputazione_Nome")))
            Dim list = dt.AsEnumerable.Select(Function(row) New BaseCodeDescr(row("Tipo_Imputazione1"), row("Tipo_Imputazione_Des"))).ToList()


            r.RispostaStringa = JsonConvert.SerializeObject(list)
            r.RispostaOK = True
        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Imputazioni_NG(ByVal piva As String,
                                        ByVal objP_super_server As String,
                                        ByVal objP_server As String,
                                        ByVal objP_utenti As String) As RispostaStandard

        Dim r As New RispostaStandard()
        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objImp As New AgronicaCoreContabDAL.Imputazioni_R

            Dim dt = objImp.Leggi(piva, 0, "", "", objParametri_Server)
            'Dim list = dt.AsEnumerable.Select(Function(row) New BaseCodeDescr(row("Imputazione_Cod"), row("Imputazione_Nome")))
            Dim list = dt.AsEnumerable.Select(Function(row) New AgronicaCoreModelsSTD.contabilita.Imputazione With {
                .codice = row("Imputazione_Cod"),
                .descrizione = row("Imputazione_Cod_Des"),
                .Nome = row("Imputazione_Nome"),
                .Tipo = New BaseCodeDescr(row("Tipo_Imputazione1"), row("Tipo_Imputazione_Des")),
                .Classe = New BaseCodeDescr(row("Imputazione_Classe_Cod"), row("Imputazione_Classe_Des"))
            }).ToList()


            r.RispostaStringa = JsonConvert.SerializeObject(list)
            r.RispostaOK = True
        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r
    End Function

End Class