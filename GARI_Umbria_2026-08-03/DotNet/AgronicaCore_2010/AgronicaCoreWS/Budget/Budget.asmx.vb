Imports System.ComponentModel
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports System.Xml
Imports AgronicaCoreDTOStd.InData.Budget
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreModelsSTD.budget

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class Budget
    Inherits System.Web.Services.WebService

    <WebMethod()>
    Public Function Leggi_Budget_Attivo(ByVal InData As CoreWS_Generic(Of Parametri_ObjParametriAgenda_NG)) As rispostaStandard(Of Budget_Testata)

        Dim r As New rispostaStandard(Of Budget_Testata)

        Dim settings As New JsonSerializerSettings()
        settings.DateTimeZoneHandling = DateTimeZoneHandling.Local

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try

            Dim objCampi As New AgronicaCoreBudgetDAL.Budget_Testata_R
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)
            Dim dt As DataTable = objCampi.Leggi_BudgetAttivo(InData.InData.Piva, 0, "", "", objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings()

            Dim newBudgTestata As New Budget_Testata
            newBudgTestata.Id_Budget = dt.Rows(0).Item("Id_Budget")
            newBudgTestata.Nome_Budget = dt.Rows(0).Item("Nome_Budget")
            newBudgTestata.Sa_Cod = dt.Rows(0).Item("Sa_Cod")
            newBudgTestata.Piva = dt.Rows(0).Item("Piva")
            r.RispostaOK = True
            r.RispostaStringa = newBudgTestata

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod>
    Public Function LeggiElencoBudgetTestata(ByVal InData As CoreWS_Generic(Of ReadBudgetTestataParams)) As rispostaStandard(Of List(Of Budget_Testata))
        Dim r As New rispostaStandard(Of List(Of Budget_Testata))

        Dim settings As New JsonSerializerSettings()
        settings.DateTimeZoneHandling = DateTimeZoneHandling.Local

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try

            Dim objCampi As New AgronicaCoreBudgetDAL.Budget_Testata_R
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)
            Dim dt As DataTable = objCampi.leggiElencoBdgTestata("", "", objParametri_Server, InData.InData.piva, InData.InData.inUso, InData.InData.tipoBudget)

            Dim serializerSettings As New JsonSerializerSettings()

            Dim bdgTestataList As List(Of Budget_Testata) = New List(Of Budget_Testata)
            For Each row In dt.Rows
                Dim newBdgTestata As New Budget_Testata
                newBdgTestata.Id_Budget = row.Item("Id_Budget")
                newBdgTestata.Nome_Budget = row.Item("Nome_Budget")
                newBdgTestata.Sa_Cod = row.Item("Sa_Cod")

                bdgTestataList.Add(newBdgTestata)
            Next

            r.RispostaOK = True
            r.RispostaStringa = bdgTestataList

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r

    End Function

End Class

Public Class Budget_Testata
    Public Id_Budget As Integer
    Public Nome_Budget As String
    Public Sa_Cod As Integer
    Public Piva As String
End Class

Public Class ReadBudgetTestataParams
    Public piva As String
    Public inUso As Boolean
    Public tipoBudget As Integer
End Class