Imports System.ComponentModel
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreUtility
Imports AgronicaCoreDTOStd.InData.Budget
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreDataProvider.My.Resources

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class Catasto1
    Inherits System.Web.Services.WebService

    <WebMethod()>
    Public Function Leggi_Investimento_Catastale(InData As Object
                                                  ) As rispostaStandard(Of String)
        Dim r As New rispostaStandard(Of String)

        Dim settings As New JsonSerializerSettings()
        settings.DateTimeZoneHandling = DateTimeZoneHandling.Local

        Dim str = JsonConvert.SerializeObject(InData, settings)

        Dim iData As CoreWS_Generic(Of BudgetAnagrafica(Of AgronicaCoreDTOStd.InData.Metaschema.LeggiInvestimentoCatastale)) =
            JsonConvert.DeserializeObject(
            Of CoreWS_Generic(Of BudgetAnagrafica(Of AgronicaCoreDTOStd.InData.Metaschema.LeggiInvestimentoCatastale)))(JsonConvert.SerializeObject(InData, settings),
                                                                                      settings)

        If iData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If iData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objParticella As New AgronicaCoreBudgetDAL.Budget_AppezzaxParticelle_R

            Dim dt As DataTable
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_utenti)

            Dim Id_Budget As Integer = 0
            Dim Piva As String = ""
            Dim sa_Cod As Long = 0
            Dim Appezza As Long = 0
            Dim Id_Reg As Long = 0
            Dim Prov As String = ""
            Dim Com As String = ""
            Dim sezione As String = ""
            Dim foglio As Integer = 0
            Dim numero As Integer = 0
            Dim subalterno As String = ""

            If iData.InData.Id_Budget <> 0 Then
                Id_Budget = iData.InData.Id_Budget
            End If

            If iData.InData.ElementoAnagrafico.impresa IsNot Nothing AndAlso iData.InData.ElementoAnagrafico.impresa.partitaIva <> "" Then
                Piva = iData.InData.ElementoAnagrafico.impresa.partitaIva
            End If

            If iData.InData.ElementoAnagrafico.centro IsNot Nothing AndAlso
                iData.InData.ElementoAnagrafico.centro.primaryKey IsNot Nothing AndAlso
                iData.InData.ElementoAnagrafico.centro.primaryKey.codice > 0 Then

                Piva = iData.InData.ElementoAnagrafico.centro.primaryKey.partitaIva
                sa_Cod = iData.InData.ElementoAnagrafico.centro.primaryKey.codice

            End If

            If iData.InData.ElementoAnagrafico.particella IsNot Nothing AndAlso
                iData.InData.ElementoAnagrafico.particella.primaryKey IsNot Nothing Then

                Prov = iData.InData.ElementoAnagrafico.particella.primaryKey.Prov
                Com = iData.InData.ElementoAnagrafico.particella.primaryKey.Com
                sezione = iData.InData.ElementoAnagrafico.particella.primaryKey.Sezione
                foglio = iData.InData.ElementoAnagrafico.particella.primaryKey.Foglio
                numero = iData.InData.ElementoAnagrafico.particella.primaryKey.Numero
                subalterno = iData.InData.ElementoAnagrafico.particella.primaryKey.Subalterno

            End If

            If iData.InData.ElementoAnagrafico.impianto IsNot Nothing AndAlso
                iData.InData.ElementoAnagrafico.impianto.primaryKey IsNot Nothing Then
                Piva = iData.InData.ElementoAnagrafico.impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva
                sa_Cod = iData.InData.ElementoAnagrafico.impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice
                Appezza = iData.InData.ElementoAnagrafico.impianto.primaryKey.appezzamentoPK.codice
                Id_Reg = iData.InData.ElementoAnagrafico.impianto.primaryKey.codice
            End If

            dt = objParticella.LeggiAppezzamenti_Da_Particella(Id_Budget, Piva, sa_Cod, Appezza, Id_Reg, Prov, Com, sezione, foglio, numero, subalterno, "", "", objParametri_Server)

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None)

        Catch ex As Exception

            r.RispostaOK = False
            r.RispostaStringa = ex.Message
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod()>
    Public Function Leggi_Investimento_Catastale_Campo(InData As Object
                                                  ) As rispostaStandard(Of String)
        Dim r As New rispostaStandard(Of String)

        Dim settings As New JsonSerializerSettings()
        settings.DateTimeZoneHandling = DateTimeZoneHandling.Local

        Dim str = JsonConvert.SerializeObject(InData, settings)

        Dim iData As CoreWS_Generic(Of BudgetAnagrafica(Of AgronicaCoreDTOStd.InData.Metaschema.LeggiInvestimentoCatastaleCampo)) =
            JsonConvert.DeserializeObject(
            Of CoreWS_Generic(Of BudgetAnagrafica(Of AgronicaCoreDTOStd.InData.Metaschema.LeggiInvestimentoCatastaleCampo)))(JsonConvert.SerializeObject(InData, settings),
                                                                                      settings)

        If iData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If iData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objCampo As New AgronicaCoreBudgetDAL.Budget_Campi_R

            Dim dt As DataTable
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_utenti)

            Dim Id_Budget As Integer = 0
            Dim Piva As String = ""
            Dim sa_Cod As Long = 0
            Dim Campo_Cod As Long = 0
            Dim Prov As String = ""
            Dim Com As String = ""
            Dim sezione As String = ""
            Dim foglio As Integer = 0
            Dim numero As Integer = 0
            Dim subalterno As String = ""

            If iData.InData.Id_Budget <> 0 Then
                Id_Budget = iData.InData.Id_Budget
            End If

            If iData.InData.ElementoAnagrafico.impresa IsNot Nothing AndAlso iData.InData.ElementoAnagrafico.impresa.partitaIva <> "" Then
                Piva = iData.InData.ElementoAnagrafico.impresa.partitaIva
            End If

            If iData.InData.ElementoAnagrafico.centro IsNot Nothing AndAlso
                iData.InData.ElementoAnagrafico.centro.primaryKey IsNot Nothing AndAlso
                iData.InData.ElementoAnagrafico.centro.primaryKey.codice > 0 Then

                Piva = iData.InData.ElementoAnagrafico.centro.primaryKey.partitaIva
                sa_Cod = iData.InData.ElementoAnagrafico.centro.primaryKey.codice

            End If

            If iData.InData.ElementoAnagrafico.particella IsNot Nothing AndAlso
                iData.InData.ElementoAnagrafico.particella.primaryKey IsNot Nothing Then

                Prov = iData.InData.ElementoAnagrafico.particella.primaryKey.Prov
                Com = iData.InData.ElementoAnagrafico.particella.primaryKey.Com
                sezione = iData.InData.ElementoAnagrafico.particella.primaryKey.Sezione
                foglio = iData.InData.ElementoAnagrafico.particella.primaryKey.Foglio
                numero = iData.InData.ElementoAnagrafico.particella.primaryKey.Numero
                subalterno = iData.InData.ElementoAnagrafico.particella.primaryKey.Subalterno

            End If

            If iData.InData.ElementoAnagrafico.campo IsNot Nothing AndAlso
                iData.InData.ElementoAnagrafico.campo.primaryKey IsNot Nothing Then
                Piva = iData.InData.ElementoAnagrafico.campo.primaryKey.centroAziendalePK.partitaIva
                sa_Cod = iData.InData.ElementoAnagrafico.campo.primaryKey.centroAziendalePK.codice
                Campo_Cod = iData.InData.ElementoAnagrafico.campo.primaryKey.codice
            End If

            dt = objCampo.LeggiCampixParticella(Id_Budget, Piva, sa_Cod, Campo_Cod, Prov, Com, sezione, foglio, numero, subalterno, "", "", objParametri_Server)

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None)

        Catch ex As Exception

            r.RispostaOK = False
            r.RispostaStringa = ex.Message
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function
End Class