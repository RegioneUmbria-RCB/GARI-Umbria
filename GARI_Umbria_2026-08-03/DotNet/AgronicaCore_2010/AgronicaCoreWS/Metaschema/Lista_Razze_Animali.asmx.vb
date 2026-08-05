Imports System.Web.Services
Imports System.ComponentModel
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDTOStd.InData.Metaschema
Imports AgronicaCoreMetaSchemaDAL
Imports Newtonsoft.Json
Imports AgronicaCoreUtentiDAL
'Imports AgronicaCoreMetaSchemaDAL

<Script.Services.ScriptService()>
<WebService(Namespace:="http://tempuri.org/")>
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class Lista_Razze_Animali
	Inherits System.Web.Services.WebService

	<WebMethod(EnableSession:=True)>
	<Script.Services.ScriptMethod()>
	Public Function get_Lista_Razze_Animali_NG(InData As CoreWS_Generic(Of get_Lista_Razze_Animali)) As RispostaStandard
		Dim r As New RispostaStandard

		Dim objP_Server As AgronicaCoreParametri
		If InData.objP.objP_server = "" Then
			objP_Server = HttpContext.Current.Session("ASG_objParametri_Server")
		Else
			objP_Server = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
		End If

		Dim objParametri_Utenti As AgronicaCoreParametri
		If InData.objP.objP_utenti = "" Then
			objParametri_Utenti = HttpContext.Current.Session("ASG_objParametri_Utenti")
		Else
			objParametri_Utenti = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)
		End If

		Dim genCod As Integer = InData.InData.gen_cod
		Dim speCod As Integer = InData.InData.spe_cod

		Dim razzeAnimali_R As New Lista_Razze_Animali_R
		Try
            Dim leggiLingua As New Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objP_Server.Lingua_Cod,
                                                                          enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                          "", "", objParametri_Utenti)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
			Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

			Dim strP As String = "get_Lista_Razze_Animali" & genCod & "_" & speCod & "_" & linguaCodiceISO

			If IsNothing(HttpContext.Current.Cache(strP)) Then
				Dim listaRazze = razzeAnimali_R.Leggi(genCod, speCod, -1, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objP_Server)

				Dim strRisp = JsonConvert.SerializeObject(listaRazze)
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

	<WebMethod(EnableSession:=True)>
	<Script.Services.ScriptMethod()>
	Public Function get_Lista_Razze_Animali(gen_cod As Integer, spe_cod As Integer, objP_server As String, objP_utenti As String) As RispostaStandard
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

		Dim razzeAnimali_R As New Lista_Razze_Animali_R
		Try
			Dim leggiLingua As New Lingue_Read
			Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParams_Server.Lingua_Cod,
																		  enumSelezioneVariabile.Selezione_TabellaCompleta,
																		  "", "", objParametri_Utenti)
			Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
			Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

			Dim strP As String = "get_Lista_Razze_Animali" & gen_cod & "_" & spe_cod & "_" & linguaCodiceISO

			If IsNothing(HttpContext.Current.Cache(strP)) Then
				Dim listaRazze = razzeAnimali_R.Leggi(gen_cod, spe_cod, -1, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParams_Server)

				Dim strRisp = JsonConvert.SerializeObject(listaRazze)
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