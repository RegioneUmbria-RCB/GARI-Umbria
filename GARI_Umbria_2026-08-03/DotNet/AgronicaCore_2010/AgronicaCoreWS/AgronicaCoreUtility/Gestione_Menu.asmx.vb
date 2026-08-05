Imports System.Web.Services
Imports System.ComponentModel
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class Gestione_Menu
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiImprese(ByVal ricerca As String,
                                    ByVal idSezione As Integer,
                                    ByVal objP_server As String,
                                    ByVal objP_utenti As String) As rispostaStandard(Of List(Of MenuBS_2017_Buildingblocks))

        Dim r As New rispostaStandard(Of List(Of MenuBS_2017_Buildingblocks))
        r.RispostaStringa = New List(Of MenuBS_2017_Buildingblocks)

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

        Try

            r.RispostaStringa = MenuBS_2017_RedirectGestione.LeggiImprese(ricerca, idSezione, objParametri_Server)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiSezioni(ByVal IDTipoSezione As Integer,
                                        ByVal IDSezionePadre As Integer,
                                        ByVal Ricerca As String,
                                        ByVal Preferiti As String,
                                        ByVal objP_server As String,
                                        ByVal objP_utenti As String
                                        ) As rispostaStandard(Of List(Of AgronicaCoreGestioneRichieste.MenuBS_2017_Buildingblocks))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreGestioneRichieste.MenuBS_2017_Buildingblocks))
        r.RispostaStringa = New List(Of AgronicaCoreGestioneRichieste.MenuBS_2017_Buildingblocks)

        'Dim objParametri_SuperServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

        Try

            Dim ConfigMenu = LeggiConfigMenu(objParametri_Server)
            Dim NascondiMenu = GetConfigMenu(ConfigMenu, "nascondiMenu", True)
            r.RispostaStringa = MenuBS_2017_RedirectGestione.LeggiSezioni(IDTipoSezione, IDSezionePadre, Ricerca, Preferiti, NascondiMenu, objParametri_Server, objParametri_Utenti)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function

    Public Shared Function LeggiConfigMenu(ByRef objParametri_Server As AgronicaCoreParametri) As JObject
        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim DTConfigSiti = objConfigSiti.Leggi(0, "MenuBS_2017_Config", "", "", objParametri_Server)
        If Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 Then
            Return JsonConvert.DeserializeObject(DTConfigSiti.Rows(0)("valore").ToString)
        End If
        Return Nothing
    End Function

    Public Shared Function GetConfigMenu(ByRef Config As JObject, ByVal Chiave As String, ByVal DefVal As Object) As Object
        If Not IsNothing(Config) Then
            If Not IsNothing(Config.Property(Chiave)) Then
                Return Config.GetValue(Chiave)
            End If
        End If
        Return DefVal
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiWorkflow(ByVal Piva As String, ByVal WWorkflow_Cod As Integer, ByVal Servizio_Cod As Integer, ByVal objP_server As String, ByVal objP_utenti As String) As RispostaStandard


        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

        Dim r As New RispostaStandard

        Try

            Dim lettura As New AgronicaCoreProfilazioneDAL.Pratiche_Stati_Attuali_R
            Dim dt As DataTable = lettura.LeggiListaPerImpresa(Piva, WWorkflow_Cod, Servizio_Cod, "", "", objParametri_Server, objParametri_Utenti)

            Dim serializerSettings As New JsonSerializerSettings() With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

End Class