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
Imports AgronicaCoreAnagrafeBIZ
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDTOStd.InData.Budget
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDTOStd.InData.Anagrafica

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class Reg_Impianto1
    Inherits System.Web.Services.WebService

    <WebMethod()>
    Public Function HelloWorld() As String
        Return "Hello World"
    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Leggi_Esercizi_Anagrafica(InData As CoreWS_Generic(Of BudgetAnagrafica(Of Parametri_ObjParametriAgenda_NG))) As RispostaStandard
        Dim r As New RispostaStandard

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objImpianto As New AgronicaCoreBudgetBIZ.Budget_Reg_Impianto_R

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim Id_Budget = InData.InData.Id_Budget
            Dim Piva = InData.InData.ElementoAnagrafico.Piva
            Dim Sa_Cod = InData.InData.ElementoAnagrafico.Sa_Cod
            Dim Appezza = InData.InData.ElementoAnagrafico.Appezza
            Dim Id_Reg = InData.InData.ElementoAnagrafico.Id_Reg
            Dim Campo_Cod = InData.InData.ElementoAnagrafico.Campo_Cod
            Dim Data_Filtro = InData.InData.ElementoAnagrafico.Data


            Dim DT_appezzamenti = objImpianto.Leggi_Esercizi_Anagrafica(Id_Budget:=Id_Budget,
                                                Piva:=Piva,
                                                Sa_Cod:=Sa_Cod,
                                                Appezza:=Appezza,
                                                Id_Reg:=Id_Reg,
                                                Campo_Cod:=Campo_Cod,
                                                Data_Filtro:=Data_Filtro,
                                                objParametri_Server, objParametri_Utenti)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(DT_appezzamenti, serializerSettings)

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod(ResponseFormat:=Script.Services.ResponseFormat.Json)>
    Public Function CaricaDatiCatastali_NG(InData As CoreWS_Generic(Of BudgetAnagrafica(Of CaricaDatiCatastali))) As RispostaStandard

        Dim r As New RispostaStandard

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objAppezzamento As New AgronicaCoreAnagrafeBIZ.Appezzamento_R

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim Piva = InData.InData.ElementoAnagrafico.parametri_ObjParametriAgenda_NG.Piva
            Dim Sa_Cod = InData.InData.ElementoAnagrafico.parametri_ObjParametriAgenda_NG.Sa_Cod
            Dim Appezza = InData.InData.ElementoAnagrafico.parametri_ObjParametriAgenda_NG.Appezza
            Dim Tipo_Operazione = InData.InData.ElementoAnagrafico.parametri_ObjParametriAgenda_NG.TipoOperazioneDB

            Dim SuperficieIntersezioneTotale As Double = 0

            Dim DT = objAppezzamento.LeggiDatiCatastali(
                Piva:=Piva,
                Sa_Cod:=Sa_Cod,
                Appezza:=Appezza,
                Campo_Cod:=InData.InData.ElementoAnagrafico.Campo_Cod,
                flag_Macrousi:=InData.InData.ElementoAnagrafico.flag_Macrousi,
                flag_Utilizzi:=InData.InData.ElementoAnagrafico.flag_Utilizzi,
                flag_Varieta:=InData.InData.ElementoAnagrafico.flag_Varieta,
                ValiditaInizio:=InData.InData.ElementoAnagrafico.ValiditaInizio,
                ValiditaFine:=InData.InData.ElementoAnagrafico.ValiditaFine,
                Tipo_Operazione:=Tipo_Operazione,
                SuperficieIntersezioneTotale:=SuperficieIntersezioneTotale,
                objParametri_Server,
                isBudget:=True,
                idBudget:=InData.InData.Id_Budget
                )

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(DT, serializerSettings)
            r.ParametroDue_stringa = SuperficieIntersezioneTotale

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

End Class