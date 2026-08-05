Imports System.ServiceModel
Imports System.Xml
Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility
Imports System.IO

Public Class wsInterrogazioneModello4
	Inherits WSBDN

	Dim soap_Autenticazione As wsInterrogazioni.SOAPAutenticazione
	Dim ws As wsInterrogazioni.wsInterrogazioniSoapClient

	Public Sub New(objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri, token As String, ruolo As String, valore_ruolo_codice As String)
		MyBase.New(objParametri_Server, objParametri_Utenti, token, ruolo, valore_ruolo_codice)
		initializeWebService()
	End Sub

	Public Sub New(objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri, link As String, username As String, password As String, token As String, ruolo As String, valore_ruolo_codice As String)
		MyBase.New(objParametri_Server, objParametri_Utenti, link, username, password, token, ruolo, valore_ruolo_codice)
		initializeWebService()
	End Sub

	Public Sub initializeWebService()

		serviceEnpoint &= "wsModelliAccompagnamento/wsInterrogazioni.asmx"

		soap_Autenticazione = New wsInterrogazioni.SOAPAutenticazione

		'If Me.username <> "" AndAlso Me.password <> "" Then
		'    soap_Autenticazione.username = Me.username
		'    soap_Autenticazione.password = Me.password
		'End If
		'If Me.token <> "" Then
		'    soap_Autenticazione.token = Me.token
		'End If

		ws = New wsInterrogazioni.wsInterrogazioniSoapClient(binding, theEndpoint)


	End Sub

	Public Function getListaPrenotazioniModelli(p_data_da As Date,
											 p_data_a As Date,
											 p_asl_codice_prov As String,
											 p_azienda_codice_prov As String,
											 p_tipo_dest As String,
											 p_stato_modello As String,
											 p_asl_codice_dest As String,
											 p_codice_struttura_dest As String,
											 p_regione_codice_dest As String,
											 Optional cacheable As Boolean = False) As DataTable
		Try
			Dim str_data_da = getStrDataFromDate(p_data_da)
			Dim str_data_a = getStrDataFromDate(p_data_a)

			Dim a = SoapRequest(Reflection.MethodBase.GetCurrentMethod().Name,
								New Dictionary(Of String, String) From {{"p_data_da", str_data_da},
								{"p_data_a", str_data_a},
								{"p_asl_codice_prov", p_asl_codice_prov},
								{"p_azienda_codice_prov", p_azienda_codice_prov},
								{"p_tipo_dest", p_tipo_dest},
								{"p_stato_modello", p_stato_modello},
								{"p_asl_codice_dest", p_asl_codice_dest},
								{"p_codice_struttura_dest", p_codice_struttura_dest},
								{"p_regione_codice_dest", p_regione_codice_dest}},
								cacheable)
			Dim dt = MyBase.generateDTfromXml(a)
			Return dt



			'Dim b = ws.getListaPrenotazioniModelli(soap_Autenticazione,
			'                                       p_data_da:=str_data_da,
			'                                       p_data_a:=str_data_a,
			'                                       p_asl_codice_prov:=p_asl_codice_prov,
			'                                       p_azienda_codice_prov:=p_azienda_codice_prov,
			'                                       p_tipo_dest:=p_tipo_dest,
			'                                       p_stato_modello:=p_stato_modello,
			'                                       p_asl_codice_dest:=p_asl_codice_dest,
			'                                       p_codice_struttura_dest:=p_codice_struttura_dest,
			'                                       p_regione_codice_dest:=p_regione_codice_dest)

			'Dim dt1 = MyBase.generateDTfromXml(b)
			'Return dt1
		Catch ex As ExpiredTokenBDNException
			Throw ex
		Catch ex As BDNException
			Throw New BDNException("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)
		Catch ex As Exception
			Return Nothing
		End Try
	End Function

	Public Function getListaPrenotazioniModelliPascolo(p_data_da As Date,
											 p_data_a As Date,
											 p_asl_codice_prov As String,
											 p_pascolo_codice_prov As String,
											 p_tipo_dest As String,
											 p_stato_modello As String,
											 p_asl_codice_dest As String,
											 p_codice_struttura_dest As String,
											 p_regione_codice_dest As String) As DataTable
		Try
			Dim str_data_da = getStrDataFromDate(p_data_da)
			Dim str_data_a = getStrDataFromDate(p_data_a)

			Dim a = SoapRequest(Reflection.MethodBase.GetCurrentMethod().Name,
								New Dictionary(Of String, String) From {{"p_data_da", str_data_da},
								{"p_data_a", str_data_a},
								{"p_asl_codice_prov", p_asl_codice_prov},
								{"p_pascolo_codice_prov", p_pascolo_codice_prov},
								{"p_tipo_dest", p_tipo_dest},
								{"p_stato_modello", p_stato_modello},
								{"p_asl_codice_dest", p_asl_codice_dest},
								{"p_codice_struttura_dest", p_codice_struttura_dest},
								{"p_regione_codice_dest", p_regione_codice_dest}})
			Dim dt = MyBase.generateDTfromXml(a)
			Return dt



			'Dim b = ws.getListaPrenotazioniModelli(soap_Autenticazione,
			'                                       p_data_da:=str_data_da,
			'                                       p_data_a:=str_data_a,
			'                                       p_asl_codice_prov:=p_asl_codice_prov,
			'                                       p_azienda_codice_prov:=p_azienda_codice_prov,
			'                                       p_tipo_dest:=p_tipo_dest,
			'                                       p_stato_modello:=p_stato_modello,
			'                                       p_asl_codice_dest:=p_asl_codice_dest,
			'                                       p_codice_struttura_dest:=p_codice_struttura_dest,
			'                                       p_regione_codice_dest:=p_regione_codice_dest)

			'Dim dt1 = MyBase.generateDTfromXml(b)
			'Return dt1
		Catch ex As ExpiredTokenBDNException
			Throw ex
		Catch ex As BDNException
			Throw New BDNException("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)
		Catch ex As Exception
			Return Nothing
		End Try
	End Function

	Public Function getPrenotazioneModello(p_prenotazione_id As String,
										   p_grspe_codice As String,
										   Optional cacheable As Boolean = False) As DataSet
		Try
			Dim ds = New DataSet
			'Dim a = ws.getPrenotazioneModello(soap_Autenticazione, p_prenotazione_id:=p_prenotazione_id, p_grspe_codice:=p_grspe_codice)

			Dim a = SoapRequest(Reflection.MethodBase.GetCurrentMethod().Name,
								New Dictionary(Of String, String) From {
								{"p_prenotazione_id", p_prenotazione_id},
								{"p_grspe_codice", p_grspe_codice}
								}, cacheable)
			Dim dt = MyBase.generateDTfromXml(a)
			'Return dt

			'Dim dt = MyBase.generateDTfromXml(a)
			If dt IsNot Nothing Then
				ds.Tables.Add(dt)

				If dt.Rows.Count = 1 Then
					Dim dtCapi = generateDTFromElencoCapi(dt.Rows(0)("XML_CAPO"))
					ds.Tables.Add(dtCapi)
				Else
					Throw New BDNException("Modello4 con più righe")
				End If

			End If
			Return ds
		Catch ex As ExpiredTokenBDNException
			Throw ex
		Catch ex As BDNException
			Throw New BDNException("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)
		Catch ex As Exception
			Return Nothing
		End Try
	End Function

	Public Function getPrenotazioneModelloDettaglio(p_prenotazione_id As String,
													p_grspe_codice As String,
													Optional cacheable As Boolean = False) As DataSet
		Dim ds = New DataSet

		Try
			Dim a = SoapRequest(Reflection.MethodBase.GetCurrentMethod().Name,
								New Dictionary(Of String, String) From {
									{"p_prenotazione_id", p_prenotazione_id},
									{"p_grspe_codice", p_grspe_codice}
								}, cacheable)

			Dim dt = generateDTfromXml(a)
			If dt IsNot Nothing Then
				ds.Tables.Add(dt)

				If dt.Rows.Count = 1 Then
					Dim dtCapi = generateDTFromElencoCapi(dt.Rows(0)("XML_CAPO"))
					ds.Tables.Add(dtCapi)
				Else
					Throw New BDNException("Modello4 con più righe")
				End If
			End If

		Catch ex As ExpiredTokenBDNException
			Throw ex
		Catch ex As BDNException
			Throw New BDNException("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)
		Catch ex As Exception
			Return Nothing
		End Try

		Return ds

	End Function

	Public Function caricaModello4(PRENOTAZIONE_ID As Integer, SPE_CODICE As String, Optional cacheable As Boolean = False) As CaricaModelli4_Response
		Dim objModelli4 As New CaricaModelli4_Response
		objModelli4.listaMatricoleCapi = New List(Of String)

		Dim dsPrenotazioneModello = getPrenotazioneModello(PRENOTAZIONE_ID, SPE_CODICE, cacheable)

		Dim dtModello4 As DataTable = dsPrenotazioneModello.Tables(0)
		objModelli4.prenotazioneId = dtModello4.Rows(0).Item("PRENOTAZIONE_ID")
		objModelli4.documentoId = dtModello4.Rows(0).Item("DOCUMENTO_ID")
		objModelli4.numModello = dtModello4.Rows(0).Item("NUM_MODELLO")
		objModelli4.giorniValidita = dtModello4.Rows(0).Item("GIORNI_VALIDITA")
		objModelli4.tipoDestinazione = dtModello4.Rows(0).Item("TIPOLOGIA_DEST")
		objModelli4.codAzienda_Prov = dtModello4.Rows(0).Item("AZIENDA_CODICE")
		If Not dtModello4.Columns.Contains("ORA_PARTENZA") OrElse IsNothing(dtModello4.Rows(0).Item("ORA_PARTENZA")) Then
			objModelli4.oraPartenza = "00:00"
		Else
			objModelli4.oraPartenza = dtModello4.Rows(0).Item("ORA_PARTENZA")
		End If
		If objModelli4.tipoDestinazione = "MACELLO" Then
			objModelli4.codAzienda_Dest = dtModello4.Rows(0).Item("MACELLO_CODICE")
		Else
			objModelli4.codAzienda_Dest = dtModello4.Rows(0).Item("DEST_AZIENDA_CODICE")
			objModelli4.idFiscaleAzienda_Dest = dtModello4.Rows(0).Item("DEST_ALLEV_ID_FISCALE")
		End If

		If dtModello4.Columns.Contains("COD_ASL_TRASP") AndAlso Not IsNothing(dtModello4.Rows(0).Item("COD_ASL_TRASP")) AndAlso dtModello4.Rows(0).Item("COD_ASL_TRASP") <> "" Then
			objModelli4.codAsl_Trasp = dtModello4.Rows(0).Item("COD_ASL_TRASP")
		End If

		If Not dtModello4.Columns.Contains("DURATA_VIAGGIO") OrElse IsNothing(dtModello4.Rows(0).Item("DURATA_VIAGGIO")) Then
			objModelli4.durataViaggio = "0g0h0m"
		Else
			objModelli4.durataViaggio = dtModello4.Rows(0).Item("DURATA_VIAGGIO")
		End If

		If dtModello4.Columns.Contains("NUM_AUTORIZZAZIONE") AndAlso Not IsNothing(dtModello4.Rows(0).Item("NUM_AUTORIZZAZIONE")) AndAlso dtModello4.Rows(0).Item("NUM_AUTORIZZAZIONE") <> "" Then
			objModelli4.numAutorizzazione = dtModello4.Rows(0).Item("NUM_AUTORIZZAZIONE")
		End If

		If dtModello4.Columns.Contains("TARGA_MOTRICE") AndAlso Not IsNothing(dtModello4.Rows(0).Item("TARGA_MOTRICE")) AndAlso dtModello4.Rows(0).Item("TARGA_MOTRICE") <> "" Then
			objModelli4.targa = dtModello4.Rows(0).Item("TARGA_MOTRICE")
		End If
		If dtModello4.Columns.Contains("TARGA_RIMORCHIO") AndAlso Not IsNothing(dtModello4.Rows(0).Item("TARGA_RIMORCHIO")) AndAlso dtModello4.Rows(0).Item("TARGA_RIMORCHIO") <> "" Then
			objModelli4.targa_rimorchio = dtModello4.Rows(0).Item("TARGA_RIMORCHIO")
		End If

		objModelli4.dataUscita = dtModello4.Rows(0).Item("DT_USCITA")
		objModelli4.dataPrenotazione = Nothing

		If dtModello4.Columns.Contains("DT_DOCUMENTO") AndAlso Not IsNothing(dtModello4.Rows(0).Item("DT_DOCUMENTO")) Then
			objModelli4.dataDocumento = dtModello4.Rows(0).Item("DT_DOCUMENTO")
		End If

		If dtModello4.Columns.Contains("REGIONE_CODICE") AndAlso Not IsNothing(dtModello4.Rows(0).Item("REGIONE_CODICE")) AndAlso dtModello4.Rows(0).Item("REGIONE_CODICE") <> "" Then
			objModelli4.regione_codice = dtModello4.Rows(0).Item("REGIONE_CODICE")
		End If

		Dim listaCapi_Modello4 = dsPrenotazioneModello.Tables(1).ToExpandoObject.ToList()
		Dim listaMatricoleCapi_Modello4 = (From cp In listaCapi_Modello4
										   Select cp.Item("CAPO_CODICE")).ToList()

		For Each mat In listaMatricoleCapi_Modello4
			objModelli4.listaMatricoleCapi.Add(mat.ToString)
		Next

		Return objModelli4
	End Function

	Public Function getPrenotazioneModelloPascolo(p_prenotazione_id As String,
											 p_grspe_codice As String) As DataSet
		Try
			Dim ds = New DataSet
			'Dim a = ws.getPrenotazioneModello(soap_Autenticazione, p_prenotazione_id:=p_prenotazione_id, p_grspe_codice:=p_grspe_codice)

			Dim a = SoapRequest(Reflection.MethodBase.GetCurrentMethod().Name,
								New Dictionary(Of String, String) From {
								{"p_prenotazione_id", p_prenotazione_id},
								{"p_grspe_codice", p_grspe_codice}
								})
			Dim dt = MyBase.generateDTfromXml(a)
			'Return dt

			'Dim dt = MyBase.generateDTfromXml(a)
			If dt IsNot Nothing Then
				ds.Tables.Add(dt)

				If dt.Rows.Count = 1 Then
					Dim dtCapi = generateDTFromElencoCapi(dt.Rows(0)("XML_CAPO"))
					ds.Tables.Add(dtCapi)
				Else
					Throw New BDNException("Modello4 con più righe")
				End If

			End If
			Return ds
		Catch ex As ExpiredTokenBDNException
			Throw ex
		Catch ex As BDNException
			Throw New BDNException("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)
		Catch ex As Exception
			Return Nothing
		End Try
	End Function

	Public Function calcolaEstremiModello(p_codice_azienda As String,
										  p_grspe_codice As String,
										  p_tipo_prov As String) As String
		Try
			Dim ds = New DataSet

			Dim a = SoapRequest(Reflection.MethodBase.GetCurrentMethod().Name,
								New Dictionary(Of String, String) From {
								{"p_codice_azienda", p_codice_azienda},
								{"p_grspe_codice", p_grspe_codice},
								{"p_tipo_prov", p_tipo_prov}
								})
			Dim estremiDoc As String = a.InnerText.Substring(a.InnerText.Length - 5, 5)

			Return estremiDoc
		Catch ex As ExpiredTokenBDNException
			Throw ex
		Catch ex As BDNException
			Throw New BDNException("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)
		Catch ex As Exception
			Return Nothing
		End Try
	End Function

	Public Function creaModelloSingoliConDatiCorrenti(p_prenotazione_id As String,
													  p_grspe_codice As String) As DataTable
		Try
			Dim dt = New DataTable

			Dim a = SoapRequest(Reflection.MethodBase.GetCurrentMethod().Name,
								New Dictionary(Of String, String) From {
								{"p_prenotazione_id", p_prenotazione_id},
								{"p_grspe_codice", p_grspe_codice}
								})
			dt = MyBase.generateDTfromXml(a)

			Return dt

		Catch ex As ExpiredTokenBDNException
			Throw ex
		Catch ex As BDNException
			If ex.Message.Contains("ERRORE INTERNO DEL SERVIZIO WEB Object reference not set to an instance of an object.") Then
				Throw New BDNException("Errore DL01: " & ex.Message)
			Else
				Throw New BDNException("Errore: " & ex.Message)
			End If

		Catch ex As Exception
			Return Nothing
		End Try

	End Function

	Private Function generateDTFromElencoCapi(strElencoCapi As String) As DataTable
		Dim dt As New DataTable
		dt.Columns.Add(New DataColumn("CAPO_CODICE", GetType(String)))

		Dim isDettaglio = strElencoCapi.Contains("LISTA_PRENOT_ID")
		If isDettaglio Then dt.Columns.Add(New DataColumn("PRENOT_ID", GetType(String)))

		Dim doc As New XmlDocument()
		doc.LoadXml(strElencoCapi)

		Dim nodoDati = (From a As XmlNode In doc.ChildNodes Where a.Name = "ELENCO_CAPI").FirstOrDefault
		If nodoDati IsNot Nothing AndAlso
			nodoDati.HasChildNodes AndAlso
			nodoDati.ChildNodes.Count > 0 Then

			For Each nodoCapo As XmlNode In nodoDati.ChildNodes
				Dim nodoCodiceCapo = (From a As XmlNode In nodoCapo Where a.Name = "CAPO_CODICE").FirstOrDefault

				Dim row = dt.NewRow
				row("CAPO_CODICE") = nodoCodiceCapo.InnerText

				If isDettaglio Then
					Dim nodoPrenotId = (From a As XmlNode In nodoCapo Where a.Name = "LISTA_PRENOT_ID").FirstOrDefault
					row("PRENOT_ID") = nodoPrenotId.InnerText
				End If

				dt.Rows.Add(row)
			Next

			Return dt
		End If

	End Function

End Class
