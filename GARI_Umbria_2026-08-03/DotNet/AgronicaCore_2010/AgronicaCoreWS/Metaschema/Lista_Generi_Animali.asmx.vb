Imports System.ComponentModel
Imports System.Web.Services
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreMetaSchemaDAL
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json

<Script.Services.ScriptService()>
<WebService(Namespace:="http://tempuri.org/")>
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class Lista_Generi_Animali
	Inherits System.Web.Services.WebService

	<WebMethod(EnableSession:=True)>
	<Script.Services.ScriptMethod()>
	Public Function get_Lista_Generi_Animali(objP_server As String, objP_utenti As String) As RispostaStandard
		Dim r As New RispostaStandard

		Dim objParams_Server As AgronicaCoreParametri
		If objP_server = "" Then
			objParams_Server = HttpContext.Current.Session("ASG_objParametri_Server")
		Else
			objParams_Server = Utility.convertStringtoOBJparametri(objP_server)
		End If

		Dim objParametri_Utenti As AgronicaCoreParametri
		If objP_utenti = "" Then
			objParametri_Utenti = HttpContext.Current.Session("ASG_objParametri_Utenti")
		Else
			objParametri_Utenti = Utility.convertStringtoOBJparametri(objP_utenti)
		End If

		Dim generiAnimali_R As New Lista_Generi_Animali_R
		Try

			Dim leggiLingua As New Lingue_Read
			Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParams_Server.Lingua_Cod,
																	  enumSelezioneVariabile.Selezione_TabellaCompleta,
																	  "", "", objParametri_Utenti)
			Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
			Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

			Dim strP As String = "get_Lista_Generi_Animali" & "_" & linguaCodiceISO

			If IsNothing(HttpContext.Current.Cache(strP)) Then
				Dim listaGeneri = generiAnimali_R.Leggi(-1, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParams_Server)

				Dim strRisp = JsonConvert.SerializeObject(listaGeneri)
				HttpContext.Current.Cache(strP) = strRisp

				r.RispostaOK = True
				r.RispostaStringa = strRisp
			Else
				r.RispostaStringa = HttpContext.Current.Cache(strP)
				r.RispostaOK = True
			End If
		Catch ex As Exception
			r.RispostaOK = False
			r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
		End Try

		Return r

	End Function

End Class