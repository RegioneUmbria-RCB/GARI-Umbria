Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreGisBIZ
Imports Newtonsoft.Json
Imports AgronicaCoreModello
Imports System.Collections.ObjectModel
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri


' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
Public Class EntrateUscite
    Inherits System.Web.Services.WebService


    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function EntrateUscite_APP(
        ByVal objP_super_server As String,
        ByVal objP_server As String,
        ByVal objP_utenti As String,
        ByVal EntrateUsciteDaMemorizzare As EntrateUsciteAPP
        ) As RispostaStandard

        Dim result As New RispostaStandard

        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim unid As String = ""
            Dim id As Guid = Guid.NewGuid()
            unid = id.ToString()

            Dim scriviEntrateUsciteAPP As New AgronicaCoreGisBIZ.APP_EntrateUsciteCoordinate
            scriviEntrateUsciteAPP.EntrateUscite_ScriviPerAPP(
                unid,
                EntrateUsciteDaMemorizzare.APPLogEventiList,
                objParametri_Server
            )

            result.RispostaOK = True
            result.RispostaStringa = "Importazione eseguita correttamente"

        Catch ex As Exception

            result.RispostaOK = False
            result.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return result
    End Function


    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Coordinate_APP(
        ByVal objP_super_server As String,
        ByVal objP_server As String,
        ByVal objP_utenti As String,
        ByVal CoordinateDaMemorizzare As Coordinate
        ) As RispostaStandard

        Dim result As New RispostaStandard

        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim unid As String = ""
            Dim id As Guid = Guid.NewGuid()
            unid = id.ToString()

            Dim scriviEntrateUsciteAPP As New AgronicaCoreGisBIZ.APP_EntrateUsciteCoordinate
            scriviEntrateUsciteAPP.Coordinate_ScriviPerAPP(
                unid,
                CoordinateDaMemorizzare.APPGISList,
                objParametri_Server
            )

            result.RispostaOK = True
            result.RispostaStringa = "Importazione eseguita correttamente"

        Catch ex As Exception

            result.RispostaOK = False
            result.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return result
    End Function

End Class