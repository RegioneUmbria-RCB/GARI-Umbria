Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreAnagrafeBIZ
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<Script.Services.ScriptService()>
<WebService(Namespace:="http://tempuri.org/")>
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
Public Class UnitaMisuraAlternativa
	Inherits System.Web.Services.WebService

	<WebMethod()> <Script.Services.ScriptMethod()>
	Public Function CaricaComboUdmAlternativa(ByVal objP_server As String,
											  ByVal Udm_Cod_From As Integer,
											  ByVal Validita_Inizio As Date,
											  ByVal Validita_Fine As Date) As rispostaStandard(Of List(Of UnitaDiMisura_Alternativa))
		Dim r As New rispostaStandard(Of List(Of UnitaDiMisura_Alternativa))

		If objP_server = "" Then
			r.Errore = "objP_server non valorizzato"
			Return r
		End If

		Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

		Try
			If Validita_Inizio < AGRODATAINIZIO Then
				Validita_Inizio = AGRODATAINIZIO
			End If
			If Validita_Fine > AGRODATAFINE Then
				Validita_Fine = AGRODATAFINE
			End If

			objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(CDate(Validita_Inizio), CDate(Validita_Fine))

			Dim objUdmAlt As New AgronicaCoreAnagrafeBIZ.UnitaMisura_Alternativa_R
			Dim dtConversioni As DataTable = objUdmAlt.Leggi_Conversioni(Udm_Cod_From, objParametri_Server)

			Dim listaConversioni As New List(Of UnitaDiMisura_Alternativa)
			If Not IsNothing(dtConversioni) AndAlso dtConversioni.Rows.Count > 0 Then
				For Each row In dtConversioni.Rows
					Dim udmAlt As New UnitaDiMisura_Alternativa(row("UDM_ALT"))
					udmAlt.descrizione = row("UDM_DES_ALT")
					udmAlt.simbolo = row("UDM_SIM_ALT")
					udmAlt.tassoConversione = row("Tasso_Conv")

					listaConversioni.Add(udmAlt)
				Next
			End If

			r.RispostaStringa = listaConversioni
			r.RispostaOK = True

		Catch ex As Exception
			r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
			r.RispostaOK = False
		End Try

		Return r

	End Function

	<WebMethod()> <Script.Services.ScriptMethod()>
	Public Function LeggiTassoConversione(ByVal objP_server As String,
										  ByVal Udm_Cod_From As Integer,
										  ByVal Udm_Cod_Alt As Integer) As RispostaStandard
		Dim r As New RispostaStandard

		If objP_server = "" Then
			r.Errore = "objP_server non valorizzato"
			Return r
		End If

		Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

		Try
			objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(CDate(AGRODATAINIZIO), CDate(AGRODATAFINE))

			Dim objUdmAlt As New AgronicaCoreAnagrafeBIZ.UnitaMisura_Alternativa_R
			Dim tassoConv As Double = objUdmAlt.Leggi_TassoConversione(Udm_Cod_From, Udm_Cod_Alt, objParametri_Server)

			r.RispostaStringa = tassoConv.ToString
			r.RispostaOK = True

		Catch ex As Exception
			r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
			r.RispostaOK = False
		End Try

		Return r

	End Function

	<WebMethod()> <Script.Services.ScriptMethod()>
	Public Function GetSuperficie_Ettari(ByVal objP_server As String,
										 ByVal Superficie As Double,
										 ByVal Udm_Cod_Alt As Integer) As RispostaStandard
		Dim r As New RispostaStandard

		If objP_server = "" Then
			r.Errore = "objP_server non valorizzato"
			Return r
		End If

		Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

		Const UDM_ETTARI As Integer = 2123

		Try
			objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(CDate(AGRODATAINIZIO), CDate(AGRODATAFINE))

			Dim objUdmAlt As New AgronicaCoreAnagrafeBIZ.UnitaMisura_Alternativa_R
			Dim tassoConv As Double = objUdmAlt.Leggi_TassoConversione(UDM_ETTARI, Udm_Cod_Alt, objParametri_Server)

			Dim supEttari As Double = Convert.ToDouble(Superficie / tassoConv)

			r.RispostaStringa = supEttari.ToString()
			r.RispostaOK = True

		Catch ex As Exception
			r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
			r.RispostaOK = False
		End Try

		Return r

	End Function

End Class