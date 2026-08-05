Imports System.ComponentModel
Imports System.Web.Services
Imports AgronicaCoreAnagrafeBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreVarieBIZ
Imports InData.DatiPrevisionaliColture
Imports Newtonsoft.Json

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class DatiPrevisionaliColture
    Inherits System.Web.Services.WebService

#Region "Read Data"

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function readDatiPrevisionaliColture(ByVal InData As Object) As rispostaStandard(Of InData.DatiPrevisionaliColture.DatiPrevisionaliColture)

        Dim r As New rispostaStandard(Of InData.DatiPrevisionaliColture.DatiPrevisionaliColture)

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
        Dim params As CoreWS_Generic(Of DatiPrevisionaliColtureRequest) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of DatiPrevisionaliColtureRequest))(JsonConvert.SerializeObject(InData), a)

        If params.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If params.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Super_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_utenti)

            ' Servono per istanziare AgroWebConfig
            HttpContext.Current.Session("ASG_objParametri_Super_Server") = objParametri_Super_Server
            HttpContext.Current.Session("ASG_objParametri_Server") = objParametri_Server
            HttpContext.Current.Session("ASG_objParametri_Utenti") = objParametri_Utenti

            Dim objDatiPrevisionali As New DatiPrevisionaliColtureRead

            r.RispostaStringa = objDatiPrevisionali.readDatiPrevisionali(params.InData, objParametri_Server)

        Catch ex As DatiPrevisionaliNoDataFoundException
            r.Errore = ex.Message
            r.RispostaOK = False
            Return r
            'Throw New GiasException(ex.Message)

            'Dim dati As New InData.DatiPrevisionaliColture.DatiPrevisionaliColture
            'dati.parametroCod = 0
            'dati.udm = New AgronicaCoreModelsSTD.metaschema.UnitaDiMisura(0)
            'dati.udm.descrizione = ""
            'dati.udm.simbolo = ""
            'dati.valore = 0
            'r.RispostaStringa = dati
        Catch ex As Exception
            Throw ex
        End Try

        r.RispostaOK = True

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function readDatiPrevisionaliColtureDT(ByVal InData As Object) As RispostaStandard

        Dim r As New RispostaStandard

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
        Dim params As CoreWS_Generic(Of DatiPrevisionaliColtureRequest) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of DatiPrevisionaliColtureRequest))(JsonConvert.SerializeObject(InData), a)

        If params.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If params.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Super_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_utenti)

            ' Servono per istanziare AgroWebConfig
            HttpContext.Current.Session("ASG_objParametri_Super_Server") = objParametri_Super_Server
            HttpContext.Current.Session("ASG_objParametri_Server") = objParametri_Server
            HttpContext.Current.Session("ASG_objParametri_Utenti") = objParametri_Utenti

            Dim objDatiPrevisionali As New DatiPrevisionaliColtureRead

            Dim dt = objDatiPrevisionali.readDatiPrevisionaliDT(params.InData, objParametri_Server)
            Dim risp = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None)

            r.RispostaStringa = risp

        Catch ex As Exception

        End Try

        r.RispostaOK = True

        Return r

    End Function

#End Region

#Region "Edit Data"

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function editDatiPrevisionaliColture(ByVal InData As CoreWS_Generic(Of DatiPrevisionaliColtureComplete)) As RispostaStandard
        Dim r As New RispostaStandard

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
        Dim params As CoreWS_Generic(Of DatiPrevisionaliColtureComplete) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of DatiPrevisionaliColtureComplete))(JsonConvert.SerializeObject(InData), a)

        If params.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If params.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Super_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_utenti)

            Dim objDatiPrevisionali As New DatiPrevisionaliColtureEdit

            objDatiPrevisionali.editDatiPrevisionaliColture(
                params.InData,
                objParametri_Server,
                objParametri_Utenti
                )

            r.RispostaStringa = JsonConvert.SerializeObject(params.InData, Newtonsoft.Json.Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function createDatiPrevisionaliColture(ByVal InData As CoreWS_Generic(Of DatiPrevisionaliColtureComplete)) As RispostaStandard
        Dim r As New RispostaStandard

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
        Dim params As CoreWS_Generic(Of DatiPrevisionaliColtureComplete) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of DatiPrevisionaliColtureComplete))(JsonConvert.SerializeObject(InData), a)

        If params.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If params.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Super_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_utenti)

            Dim objDatiPrevisionali As New DatiPrevisionaliColtureEdit

            objDatiPrevisionali.createDatiPrevisionaliColture(
                params.InData,
                objParametri_Server,
                objParametri_Utenti
                )

            r.RispostaStringa = JsonConvert.SerializeObject(params.InData, Newtonsoft.Json.Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function deleteDatiPrevisionaliColture(ByVal InData As CoreWS_Generic(Of DatiPrevisionaliColtureComplete)) As RispostaStandard
        Dim r As New RispostaStandard

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
        Dim params As CoreWS_Generic(Of DatiPrevisionaliColtureComplete) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of DatiPrevisionaliColtureComplete))(JsonConvert.SerializeObject(InData), a)

        If params.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If params.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Super_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_utenti)

            Dim objDatiPrevisionali As New DatiPrevisionaliColtureEdit

            objDatiPrevisionali.deleteDatiPrevisionaliColture(
                params.InData,
                objParametri_Server,
                objParametri_Utenti
                )

            r.RispostaStringa = JsonConvert.SerializeObject(params.InData, Newtonsoft.Json.Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function

#End Region

End Class