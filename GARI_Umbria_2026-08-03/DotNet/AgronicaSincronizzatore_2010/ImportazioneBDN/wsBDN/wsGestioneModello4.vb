Imports System.IO
Imports System.Reflection
Imports System.Security
Imports System.ServiceModel
Imports System.Xml
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class wsGestioneModello4
	Inherits WSBDN

	Dim soap_Autenticazione As wsGestioneModello.SOAPAutenticazione
	Dim ws As wsGestioneModello.wsGestioneModelloSoapClient

	Public Sub New(objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri, token As String, ruolo As String, valore_ruolo_codice As String)
		MyBase.New(objParametri_Server, objParametri_Utenti, token, ruolo, valore_ruolo_codice)
		initializeWebService()
	End Sub

	Public Sub New(objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri, link As String, username As String, password As String, token As String, ruolo As String, valore_ruolo_codice As String)
		MyBase.New(objParametri_Server, objParametri_Utenti, link, username, password, token, ruolo, valore_ruolo_codice)
		initializeWebService()
	End Sub

	Public Sub initializeWebService()

		serviceEnpoint &= "wsModelliAccompagnamento/wsGestioneModello.asmx"
		soap_Autenticazione = New wsGestioneModello.SOAPAutenticazione
		ws = New wsGestioneModello.wsGestioneModelloSoapClient(binding, theEndpoint)

	End Sub

	''' <summary>
	''' insPrenotazioneModello (wsModelliAccompagnamento.pdf, pg.21)
	''' </summary>
	''' <param name="P_PRENOTAZIONE_ID"></param>
	''' <param name="P_DOCUMENTO_ID"></param>
	''' <param name="P_AZIENDA_CODICE"></param>
	''' <param name="P_ALLEV_ID_FISCALE"></param>
	''' <param name="P_SPECIE_CODICE"></param>
	''' <param name="P_DEST_AZIENDA_CODICE"></param>
	''' <param name="P_DEST_ALLEV_ID_FISCALE"></param>
	''' <param name="P_DEST_SPECIE_CODICE"></param>
	''' <param name="P_FIERA_CODICE"></param>
	''' <param name="P_STATO_CODICE"></param>
	''' <param name="P_PASCOLO_CODICE"></param>
	''' <param name="P_MACELLO_CODICE"></param>
	''' <param name="P_REGIONE_CODICE"></param>
	''' <param name="P_ESTREMI_DOCUMENTO"></param>
	''' <param name="Lista_Capi"></param>
	''' <param name="P_DT_USCITA"></param>
	''' <param name="P_CAUSALE"></param>
	''' <param name="P_TIPO_STAMPA"></param>
	''' <param name="P_FLAG_MACELLO_1"></param>
	''' <param name="P_FLAG_MACELLO_2"></param>
	''' <param name="P_FLAG_MACELLO_2A"></param>
	''' <param name="P_FLAG_MACELLO_2B"></param>
	''' <param name="P_FLAG_MACELLO_2C"></param>
	''' <param name="Lista_Trattamenti"></param>
	''' <param name="Lista_Esami"></param>
	''' <param name="P_FLAG_MACELLO_3"></param>
	''' <param name="P_FLAG_MACELLO_3_ENTERICI"></param>
	''' <param name="P_FLAG_MACELLO_3_RESPIRATORI"></param>
	''' <param name="P_FLAG_MACELLO_3_CUTANEI"></param>
	''' <param name="P_FLAG_MACELLO_3_LOCOMOTORI"></param>
	''' <param name="P_FLAG_MACELLO_3_ALTRO"></param>
	''' <param name="P_FLAG_MACELLO_3_ALTRO_DESC"></param>
	''' <param name="P_FLAG_MACELLO_4"></param>
	''' <param name="P_FLAG_MACELLO_5"></param>
	''' <param name="P_FLAG_ELEMENTI"></param>
	''' <param name="P_FLAG_RILEVAZIONI"></param>
	''' <param name="P_FLAG_ALTRO"></param>
	''' <param name="P_FLAG_ALTRO_DESC"></param>
	''' <param name="P_FLAG_UPLOAD"></param>
	''' <param name="P_FLAG_MACELLO_6"></param>
	''' <param name="P_VET_AZIENDALE"></param>
	''' <param name="P_INDIRIZZO"></param>
	''' <param name="P_TELEFONO"></param>
	''' <param name="P_ISTAT"></param>
	''' <param name="P_SIGLA"></param>
	''' <param name="P_NUM_ISCR_ALBO"></param>
	''' <param name="P_TRASP_TARGA_MOTRICE"></param>
	''' <param name="P_TRASP_TARGA"></param>
	''' <param name="P_TRASP_CONDUCENTE"></param>
	''' <param name="P_TRASP_DENOM_TRASPORTATORE"></param>
	''' <param name="P_TRASP_TARGA_RIMORCHIO"></param>
	''' <param name="P_TRASP_NUM_AUTORIZZAZIONE"></param>
	''' <param name="P_TRASP_DT_PARTENZA"></param>
	''' <param name="P_TRASP_ORA_PARTENZA"></param>
	''' <param name="P_TRASP_DURATA_VIAGGIO"></param>
	''' <param name="P_TRASP_FLAG_MEZZO_PROPRIO"></param>
	''' <param name="P_TRASP_COD_ASL_TRASP"></param>
	''' <param name="P_TRASP_SL_COD_FISCALE"></param>
	''' <param name="P_FLAG_USCITA_AUTOMATICA"></param>
	''' <param name="P_CONFERMA_BDR"></param>
	''' <param name="P_DT_DOCUMENTO"></param>
	''' <param name="P_GIORNI_VALIDITA"></param>
	''' <param name="P_FLAG_TIPO_NOTA"></param>
	''' <param name="P_NOTA"></param>
	''' <param name="P_VETERINARIO"></param>
	''' <param name="P_DETEN_PAS_ID_FISCALE"></param>
	''' <param name="P_SIGLA_AUTOC"></param>
	''' <param name="P_ISTAT_AUTOC"></param>
	''' <param name="P_ID_FISCALE_AUTOC"></param>
	''' <param name="P_DT_RIENTRO"></param>
	''' <param name="P_DESCR_PERCORSO"></param>
	''' <param name="P_SIGLA_DEST"></param>
	''' <param name="P_ISTAT_DEST"></param>
	''' <returns></returns>
	Public Function insPrenotazioneModello(ByVal P_PRENOTAZIONE_ID As Integer,
										   ByVal P_DOCUMENTO_ID As Integer,
										   ByVal P_AZIENDA_CODICE As String,
										   ByVal P_ALLEV_ID_FISCALE As String,
										   ByVal P_SPECIE_CODICE As String,
										   ByVal P_DEST_AZIENDA_CODICE As String,
										   ByVal P_DEST_ALLEV_ID_FISCALE As String,
										   ByVal P_DEST_SPECIE_CODICE As String,
										   ByVal P_FIERA_CODICE As String,
										   ByVal P_STATO_CODICE As String,
										   ByVal P_PASCOLO_CODICE As String,
										   ByVal P_MACELLO_CODICE As String,
										   ByVal P_REGIONE_CODICE As String,
										   ByVal P_ESTREMI_DOCUMENTO As String,
										   ByVal Lista_Capi As List(Of XmlCapi_InvioModelli), ' = P_XML_CAPO
										   ByVal P_DT_USCITA As Date,
										   ByVal P_CAUSALE As String,
										   ByVal P_TIPO_STAMPA As String,
										   ByVal P_FLAG_MACELLO_1 As String,
										   ByVal P_FLAG_MACELLO_2 As String,
										   ByVal P_FLAG_MACELLO_2A As String,
										   ByVal P_FLAG_MACELLO_2B As String,
										   ByVal P_FLAG_MACELLO_2C As String,
										   ByVal Lista_Trattamenti As List(Of XmlTrattamenti_InvioModelli), ' = P_XML_TRATTAMENTI
										   ByVal Lista_Esami As List(Of XmlEsamiCapo_InvioModelli), ' = P_XML_ESAMI
										   ByVal P_FLAG_MACELLO_3 As String,
										   ByVal P_FLAG_MACELLO_3_ENTERICI As String,
										   ByVal P_FLAG_MACELLO_3_RESPIRATORI As String,
										   ByVal P_FLAG_MACELLO_3_CUTANEI As String,
										   ByVal P_FLAG_MACELLO_3_LOCOMOTORI As String,
										   ByVal P_FLAG_MACELLO_3_ALTRO As String,
										   ByVal P_FLAG_MACELLO_3_ALTRO_DESC As String,
										   ByVal P_FLAG_MACELLO_4 As String,
										   ByVal P_FLAG_MACELLO_5 As String,
										   ByVal P_FLAG_ELEMENTI As String,
										   ByVal P_FLAG_RILEVAZIONI As String,
										   ByVal P_FLAG_ALTRO As String,
										   ByVal P_FLAG_ALTRO_DESC As String,
										   ByVal P_FLAG_UPLOAD As String,
										   ByVal P_FLAG_MACELLO_6 As String,
										   ByVal P_VET_AZIENDALE As String,
										   ByVal P_INDIRIZZO As String,
										   ByVal P_TELEFONO As String,
										   ByVal P_ISTAT As String,
										   ByVal P_SIGLA As String,
										   ByVal P_NUM_ISCR_ALBO As String,
										   ByVal P_TRASP_TARGA_MOTRICE As String, 'P_XML_TRASPORTATORE INIZIO
										   ByVal P_TRASP_TARGA As String,
										   ByVal P_TRASP_CONDUCENTE As String,
										   ByVal P_TRASP_DENOM_TRASPORTATORE As String,
										   ByVal P_TRASP_TARGA_RIMORCHIO As String,
										   ByVal P_TRASP_NUM_AUTORIZZAZIONE As String,
										   ByVal P_TRASP_DT_PARTENZA As Date,
										   ByVal P_TRASP_ORA_PARTENZA As String,
										   ByVal P_TRASP_DURATA_VIAGGIO As String,
										   ByVal P_TRASP_FLAG_MEZZO_PROPRIO As String,
										   ByVal P_TRASP_COD_ASL_TRASP As String,
										   ByVal P_TRASP_SL_COD_FISCALE As String, 'P_XML_TRASPORTATORE FINE
										   ByVal P_FLAG_USCITA_AUTOMATICA As String,
										   ByVal P_CONFERMA_BDR As String,
										   ByVal P_DT_DOCUMENTO As Date,
										   ByVal P_GIORNI_VALIDITA As String,
										   ByVal P_FLAG_TIPO_NOTA As String,
										   ByVal P_NOTA As String,
										   ByVal P_VETERINARIO As String,
										   ByVal P_DETEN_PAS_ID_FISCALE As String,
										   ByVal P_SIGLA_AUTOC As String,
										   ByVal P_ISTAT_AUTOC As String,
										   ByVal P_ID_FISCALE_AUTOC As String,
										   ByVal P_DT_RIENTRO As Date,
										   ByVal P_DESCR_PERCORSO As String,
										   ByVal P_SIGLA_DEST As String,
										   ByVal P_ISTAT_DEST As String) As DataTable
		Dim dtInserimentoModello4 As New DataTable

		Try
			Dim xml_InserisciModello4 As New dsPRENOTAZIONE_MODELLO_IUS()
			xml_InserisciModello4.InitVars()
			xml_InserisciModello4.EnforceConstraints = True

			xml_InserisciModello4.PARAMETERS_LIST.InitVars()
			Dim PARAMETERS_LISTRow As dsPRENOTAZIONE_MODELLO_IUS.PARAMETERS_LISTRow = xml_InserisciModello4.PARAMETERS_LIST.AddPARAMETERS_LISTRow(P_PRENOTAZIONE_ID, P_DOCUMENTO_ID,
																																				  P_AZIENDA_CODICE, P_ALLEV_ID_FISCALE, P_SPECIE_CODICE,
																																				  P_DEST_AZIENDA_CODICE, P_DEST_ALLEV_ID_FISCALE, P_DEST_SPECIE_CODICE,
																																				  P_FIERA_CODICE, P_PASCOLO_CODICE,
																																				  P_MACELLO_CODICE, P_REGIONE_CODICE,
																																				  P_ESTREMI_DOCUMENTO, CDate(P_DT_USCITA),
																																				  P_CAUSALE, P_TIPO_STAMPA,
																																				  P_FLAG_MACELLO_1, P_FLAG_MACELLO_2,
																																				  P_FLAG_MACELLO_2A, P_FLAG_MACELLO_2B, P_FLAG_MACELLO_2C,
																																				  P_FLAG_MACELLO_3, P_FLAG_MACELLO_3_ENTERICI,
																																				  P_FLAG_MACELLO_3_RESPIRATORI, P_FLAG_MACELLO_3_CUTANEI,
																																				  P_FLAG_MACELLO_3_LOCOMOTORI, P_FLAG_MACELLO_3_ALTRO,
																																				  P_FLAG_MACELLO_3_ALTRO_DESC,
																																				  P_FLAG_MACELLO_4, P_FLAG_MACELLO_5,
																																				  P_FLAG_ELEMENTI, P_FLAG_RILEVAZIONI,
																																				  P_FLAG_ALTRO, P_FLAG_ALTRO_DESC,
																																				  P_FLAG_UPLOAD, P_FLAG_MACELLO_6,
																																				  P_VET_AZIENDALE, P_INDIRIZZO,
																																				  P_TELEFONO, P_ISTAT,
																																				  P_SIGLA, P_NUM_ISCR_ALBO,
																																				  P_FLAG_USCITA_AUTOMATICA, P_CONFERMA_BDR,
																																				  CDate(P_DT_DOCUMENTO), P_GIORNI_VALIDITA,
																																				  P_FLAG_TIPO_NOTA, P_NOTA,
																																				  P_VETERINARIO, P_DETEN_PAS_ID_FISCALE,
																																				  P_SIGLA_AUTOC, P_ISTAT_AUTOC, P_ID_FISCALE_AUTOC,
																																				  CDate(P_DT_RIENTRO), P_DESCR_PERCORSO,
																																				  P_SIGLA_DEST, P_ISTAT_DEST)

			'P_XML_CAPI
			Dim P_XML_CAPIRow As dsPRENOTAZIONE_MODELLO_IUS.P_XML_CAPORow = xml_InserisciModello4.P_XML_CAPO.AddP_XML_CAPORow(PARAMETERS_LISTRow)
			Dim ELENCO_CAPIRow As dsPRENOTAZIONE_MODELLO_IUS.ELENCO_CAPIRow = xml_InserisciModello4.ELENCO_CAPI.AddELENCO_CAPIRow(P_XML_CAPIRow)
			For Each capo In Lista_Capi
				xml_InserisciModello4.CAPO.AddCAPORow(capo.capoCodice, capo.codiceElettronico,
													  capo.identNome, capo.passaporto, capo.codiceUeln,
													  ELENCO_CAPIRow)
			Next
			'P_XML_CAPI

			'P_XML_TRATTAMENTI
			'al momento inserisce un'unica riga nulla
			Dim P_XML_TRATTAMENTIRow As dsPRENOTAZIONE_MODELLO_IUS.P_XML_TRATTAMENTIRow = xml_InserisciModello4.P_XML_TRATTAMENTI.AddP_XML_TRATTAMENTIRow(PARAMETERS_LISTRow)
			Dim ELENCO_TRATTAMENTIRow As dsPRENOTAZIONE_MODELLO_IUS.ELENCO_TRATTAMENTIRow = xml_InserisciModello4.ELENCO_TRATTAMENTI.AddELENCO_TRATTAMENTIRow(P_XML_TRATTAMENTIRow)
			Dim TRATTAMENTORow As dsPRENOTAZIONE_MODELLO_IUS.TRATTAMENTORow = xml_InserisciModello4.TRATTAMENTO.AddTRATTAMENTORow("", "",
																																	  "", "",
																																	  "", "",
																																	  Nothing, Nothing,
																																	  ELENCO_TRATTAMENTIRow)

			'P_XML_TRATTAMENTI

			'P_XML_ESAMI
			'al momento inserisce un'unica riga nulla
			Dim P_XML_ESAMI As dsPRENOTAZIONE_MODELLO_IUS.P_XML_ESAMIRow = xml_InserisciModello4.P_XML_ESAMI.AddP_XML_ESAMIRow(PARAMETERS_LISTRow)
			Dim ELENCO_ESAMIRow As dsPRENOTAZIONE_MODELLO_IUS.ELENCO_ESAMIRow = xml_InserisciModello4.ELENCO_ESAMI.AddELENCO_ESAMIRow(P_XML_ESAMI)
			Dim ESAMERow As dsPRENOTAZIONE_MODELLO_IUS.ESAMERow = xml_InserisciModello4.ESAME.AddESAMERow("", "",
																											  "", Nothing,
																											  "", "",
																											  "", "",
																											  ELENCO_ESAMIRow)

			'    NewESAMERow()
			'ESAMERow.SetParentRow(ELENCO_ESAMIRow)
			'P_XML_ESAMI

			'P_XML_TRASPORTATORE
			Dim P_XML_TRASPORTATORE As dsPRENOTAZIONE_MODELLO_IUS.P_XML_TRASPORTATORERow = xml_InserisciModello4.P_XML_TRASPORTATORE.AddP_XML_TRASPORTATORERow(PARAMETERS_LISTRow)
			Dim TRASPORTATORE As dsPRENOTAZIONE_MODELLO_IUS.TRASPORTATORERow = xml_InserisciModello4.TRASPORTATORE.AddTRASPORTATORERow(P_XML_TRASPORTATORE)
			xml_InserisciModello4.DATI.AddDATIRow(P_TRASP_TARGA_MOTRICE, P_TRASP_TARGA, P_TRASP_CONDUCENTE,
												  P_TRASP_TARGA_RIMORCHIO, P_TRASP_NUM_AUTORIZZAZIONE,
												  CDate(P_TRASP_DT_PARTENZA),
												  P_TRASP_ORA_PARTENZA, P_TRASP_DURATA_VIAGGIO,
												  P_TRASP_FLAG_MEZZO_PROPRIO, P_TRASP_COD_ASL_TRASP,
												  P_TRASP_DENOM_TRASPORTATORE, P_TRASP_SL_COD_FISCALE,
												  TRASPORTATORE)
			'P_XML_TRASPORTATORE


			'genera la stringa dall'xml
			xml_InserisciModello4.AcceptChanges()

			Dim strXml_InserisciModello4 As String = xml_InserisciModello4.GetXml()

			'gestione xml
			Dim docXmlSwap As New XmlDocument
			docXmlSwap.LoadXml(strXml_InserisciModello4)

			Dim root As XmlElement = docXmlSwap.DocumentElement
			Dim XmlNode_New As XmlNode
			Dim XmlNode_Ref As XmlNode

			'TEST
			root.FirstChild.Item("P_ESTREMI_DOCUMENTO").InnerText = P_ESTREMI_DOCUMENTO

			'swap P_XML_CAPO
			XmlNode_New = root.FirstChild.Item("P_XML_CAPO")
			XmlNode_Ref = root.FirstChild.Item("P_ESTREMI_DOCUMENTO")
			root.FirstChild.RemoveChild(XmlNode_New)
			root.FirstChild.InsertAfter(XmlNode_New, XmlNode_Ref)

			'swap P_XML_ESAMI
			XmlNode_New = root.FirstChild.Item("P_XML_ESAMI")
			XmlNode_Ref = root.FirstChild.Item("P_FLAG_MACELLO_2C")
			root.FirstChild.RemoveChild(XmlNode_New)
			root.FirstChild.InsertAfter(XmlNode_New, XmlNode_Ref)

			'swap P_XML_TRATTAMENTI 
			XmlNode_New = root.FirstChild.Item("P_XML_TRATTAMENTI")
			XmlNode_Ref = root.FirstChild.Item("P_FLAG_MACELLO_2C")
			root.FirstChild.RemoveChild(XmlNode_New)
			root.FirstChild.InsertAfter(XmlNode_New, XmlNode_Ref)

			'swap P_XML_TRASPORTATORE 
			XmlNode_New = root.FirstChild.Item("P_XML_TRASPORTATORE")
			XmlNode_Ref = root.FirstChild.Item("P_NUM_ISCR_ALBO")
			root.FirstChild.RemoveChild(XmlNode_New)
			root.FirstChild.InsertAfter(XmlNode_New, XmlNode_Ref)

			'eliminazione dati TRATTAMENTI e ESAMI 
			'al momento non vengono gestiti trattamenti ed esami sui capi in ingresso con Modello4
			'nel momento in cui verranno gestiti, modificare
			root.FirstChild.Item("P_XML_TRATTAMENTI").Item("ELENCO_TRATTAMENTI").Item("TRATTAMENTO").RemoveAll()
			root.FirstChild.Item("P_XML_ESAMI").Item("ELENCO_ESAMI").Item("ESAME").RemoveAll()


			'If P_CAUSALE = "M" Then
			root.FirstChild.Item("P_XML_TRATTAMENTI").RemoveAll()
			root.FirstChild.Item("P_XML_ESAMI").RemoveAll()
			'End If
			'conversione date in formato ("yyyy-MM-dd")
			Dim XmlNode_Date As XmlNode

			'P_DT_USCITA
			XmlNode_Date = root.FirstChild.Item("P_DT_USCITA")
			If CDate(XmlNode_Date.InnerText) < AGRODATAINIZIO Then
				XmlNode_Date.InnerText = ""
			Else
				XmlNode_Date.InnerText = CDate(XmlNode_Date.InnerText).Date.ToString("yyyy-MM-dd")
			End If

			'P_DT_DOCUMENTO
			XmlNode_Date = root.FirstChild.Item("P_DT_DOCUMENTO")
			If CDate(XmlNode_Date.InnerText) < AGRODATAINIZIO Then
				XmlNode_Date.InnerText = ""
			Else
				XmlNode_Date.InnerText = CDate(XmlNode_Date.InnerText).Date.ToString("yyyy-MM-dd")
			End If

			'P_DT_RIENTRO
			XmlNode_Date = root.FirstChild.Item("P_DT_RIENTRO")
			If CDate(XmlNode_Date.InnerText) < AGRODATAINIZIO Then
				XmlNode_Date.InnerText = ""
				root.FirstChild.RemoveChild(XmlNode_Date)
			Else
				XmlNode_Date.InnerText = CDate(XmlNode_Date.InnerText).Date.ToString("yyyy-MM-dd")
			End If

			'P_XML_TRASPORTATORE -> TRASPORTATORE -> DATI ->  DT_PARTENZA
			XmlNode_Date = root.FirstChild.Item("P_XML_TRASPORTATORE").Item("TRASPORTATORE").Item("DATI").Item("DT_PARTENZA")
			If CDate(XmlNode_Date.InnerText) < AGRODATAINIZIO Then
				XmlNode_Date.InnerText = ""
			Else
				XmlNode_Date.InnerText = CDate(XmlNode_Date.InnerText).Date.ToString("yyyy-MM-dd")
			End If

			'dopo i vari aggiustamenti al dannato xml, lo salva in stringa
			Dim finalXml_InserisciModello4 As String = docXmlSwap.InnerXml
			If (P_CAUSALE = "M" Or P_CAUSALE = "V") Then
				finalXml_InserisciModello4 = finalXml_InserisciModello4.Replace("<P_XML_TRATTAMENTI/>", "")
				finalXml_InserisciModello4 = finalXml_InserisciModello4.Replace("<P_XML_TRATTAMENTI></P_XML_TRATTAMENTI>", "")
				'"<P_XML_TRATTAMENTI></P_XML_TRATTAMENTI>"
				finalXml_InserisciModello4 = finalXml_InserisciModello4.Replace("<P_XML_ESAMI/>", "")
				finalXml_InserisciModello4 = finalXml_InserisciModello4.Replace("<P_XML_ESAMI></P_XML_ESAMI>", "")
			End If
			'genera la stringa con Escape dalla stringa dell'xml
			Dim esStrXml_InserisciModello4 As String = System.Security.SecurityElement.Escape(finalXml_InserisciModello4)

			Dim reqInsert As XmlNode = SoapRequestV2(esStrXml_InserisciModello4, Reflection.MethodBase.GetCurrentMethod().Name)

			dtInserimentoModello4 = MyBase.generateDTfromXml(reqInsert)
		Catch ex As ExpiredTokenBDNException
			Throw ex
		Catch ex As BDNException
			Throw New BDNException("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)

		Catch ex As Exception
			Throw New Exception("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)

		End Try

		Return dtInserimentoModello4

	End Function

	''' <summary>
	''' registraUscitaModello (wsModelliAccompagnamento.pdf, pg. 39)
	''' </summary>
	''' <param name="P_PRENOTAZIONE_ID"></param>
	''' <param name="P_SPECIE_CODICE"></param>
	''' <param name="P_DT_USCITA"></param>
	''' <param name="P_DT_AUTORITA"></param>
	''' <param name="P_FLAG_MACELLO_1"></param>
	''' <param name="P_FLAG_MACELLO_2"></param>
	''' <param name="P_FLAG_MACELLO_2A"></param>
	''' <param name="P_FLAG_MACELLO_2B"></param>
	''' <param name="P_FLAG_MACELLO_2C"></param>
	''' <param name="Lista_Trattamenti"></param>
	''' <param name="Lista_Esami"></param>
	''' <param name="P_FLAG_MACELLO_3"></param>
	''' <param name="P_FLAG_MACELLO_3_ENTERICI"></param>
	''' <param name="P_FLAG_MACELLO_3_RESPIRATORI"></param>
	''' <param name="P_FLAG_MACELLO_3_CUTANEI"></param>
	''' <param name="P_FLAG_MACELLO_3_LOCOMOTORI"></param>
	''' <param name="P_FLAG_MACELLO_3_ALTRO"></param>
	''' <param name="P_FLAG_MACELLO_3_ALTRO_DESC"></param>
	''' <param name="P_FLAG_MACELLO_4"></param>
	''' <param name="P_FLAG_MACELLO_5"></param>
	''' <param name="P_FLAG_ELEMENTI"></param>
	''' <param name="P_FLAG_RILEVAZIONI"></param>
	''' <param name="P_FLAG_ALTRO"></param>
	''' <param name="P_FLAG_ALTRO_DESC"></param>
	''' <param name="P_FLAG_MACELLO_6"></param>
	''' <param name="P_VET_AZIENDALE"></param>
	''' <param name="P_INDIRIZZO"></param>
	''' <param name="P_TELEFONO"></param>
	''' <param name="P_ISTAT"></param>
	''' <param name="P_SIGLA"></param>
	''' <param name="P_NUM_ISCR_ALBO"></param>
	''' <param name="P_TRASP_TARGA_MOTRICE"></param>
	''' <param name="P_TRASP_TARGA"></param>
	''' <param name="P_TRASP_CONDUCENTE"></param>
	''' <param name="P_TRASP_DENOM_TRASPORTATORE"></param>
	''' <param name="P_TRASP_TARGA_RIMORCHIO"></param>
	''' <param name="P_TRASP_NUM_AUTORIZZAZIONE"></param>
	''' <param name="P_TRASP_DT_PARTENZA"></param>
	''' <param name="P_TRASP_ORA_PARTENZA"></param>
	''' <param name="P_TRASP_DURATA_VIAGGIO"></param>
	''' <param name="P_TRASP_FLAG_MEZZO_PROPRIO"></param>
	''' <param name="P_TRASP_COD_ASL_TRASP"></param>
	''' <param name="P_TRASP_SL_COD_FISCALE"></param>
	''' <param name="Lista_Capi"></param>
	''' <param name="P_FLAG_USCITA_AUTOMATICA"></param>
	''' <returns></returns>
	Public Function registraUscitaModello(ByVal P_PRENOTAZIONE_ID As Integer,
										  ByVal P_SPECIE_CODICE As String,
										  ByVal P_DT_USCITA As Date,
										  ByVal P_DT_AUTORITA As Date,
										  ByVal P_FLAG_MACELLO_1 As String,
										  ByVal P_FLAG_MACELLO_2 As String,
										  ByVal P_FLAG_MACELLO_2A As String,
										  ByVal P_FLAG_MACELLO_2B As String,
										  ByVal P_FLAG_MACELLO_2C As String,
										  ByVal Lista_Trattamenti As List(Of XmlTrattamenti_InvioModelli), ' = P_XML_TRATTAMENTI
										  ByVal Lista_Esami As List(Of XmlEsamiCapo_InvioModelli), ' = P_XML_ESAMI
										  ByVal P_FLAG_MACELLO_3 As String,
										  ByVal P_FLAG_MACELLO_3_ENTERICI As String,
										  ByVal P_FLAG_MACELLO_3_RESPIRATORI As String,
										  ByVal P_FLAG_MACELLO_3_CUTANEI As String,
										  ByVal P_FLAG_MACELLO_3_LOCOMOTORI As String,
										  ByVal P_FLAG_MACELLO_3_ALTRO As String,
										  ByVal P_FLAG_MACELLO_3_ALTRO_DESC As String,
										  ByVal P_FLAG_MACELLO_4 As String,
										  ByVal P_FLAG_MACELLO_5 As String,
										  ByVal P_FLAG_ELEMENTI As String,
										  ByVal P_FLAG_RILEVAZIONI As String,
										  ByVal P_FLAG_ALTRO As String,
										  ByVal P_FLAG_ALTRO_DESC As String,
										  ByVal P_FLAG_MACELLO_6 As String,
										  ByVal P_VET_AZIENDALE As String,
										  ByVal P_INDIRIZZO As String,
										  ByVal P_TELEFONO As String,
										  ByVal P_ISTAT As String,
										  ByVal P_SIGLA As String,
										  ByVal P_NUM_ISCR_ALBO As String,
										  ByVal P_TRASP_TARGA_MOTRICE As String, 'P_XML_TRASPORTATORE INIZIO
										  ByVal P_TRASP_TARGA As String,
										  ByVal P_TRASP_CONDUCENTE As String,
										  ByVal P_TRASP_DENOM_TRASPORTATORE As String,
										  ByVal P_TRASP_TARGA_RIMORCHIO As String,
										  ByVal P_TRASP_NUM_AUTORIZZAZIONE As String,
										  ByVal P_TRASP_DT_PARTENZA As Date,
										  ByVal P_TRASP_ORA_PARTENZA As String,
										  ByVal P_TRASP_DURATA_VIAGGIO As String,
										  ByVal P_TRASP_FLAG_MEZZO_PROPRIO As String,
										  ByVal P_TRASP_COD_ASL_TRASP As String,
										  ByVal P_TRASP_SL_COD_FISCALE As String, 'P_XML_TRASPORTATORE FINE
										  ByVal Lista_Capi As List(Of String), ' = P_XML_CAPO
										  ByVal P_FLAG_USCITA_AUTOMATICA As String) As DataTable
		Dim dtRegistrazioneModello As New DataTable

		Try
			Dim xml_RegistraModello4 As New dsREGISTRAZ_USCITA_MODELLO_IUS()
			xml_RegistraModello4.InitVars()
			xml_RegistraModello4.EnforceConstraints = True

			xml_RegistraModello4.PARAMETERS_LIST.InitVars()
			Dim PARAMETERS_LISTRow As dsREGISTRAZ_USCITA_MODELLO_IUS.PARAMETERS_LISTRow = xml_RegistraModello4.PARAMETERS_LIST.AddPARAMETERS_LISTRow(P_PRENOTAZIONE_ID, P_SPECIE_CODICE,
																																					 P_DT_USCITA, P_DT_AUTORITA,
																																					 P_FLAG_MACELLO_1, P_FLAG_MACELLO_2,
																																					 P_FLAG_MACELLO_2A, P_FLAG_MACELLO_2B, P_FLAG_MACELLO_2C,
																																					 P_FLAG_MACELLO_3, P_FLAG_MACELLO_3_ENTERICI, P_FLAG_MACELLO_3_RESPIRATORI,
																																					 P_FLAG_MACELLO_3_CUTANEI, P_FLAG_MACELLO_3_LOCOMOTORI,
																																					 P_FLAG_MACELLO_3_ALTRO, P_FLAG_MACELLO_3_ALTRO_DESC,
																																					 P_FLAG_MACELLO_4, P_FLAG_MACELLO_5,
																																					 P_FLAG_ELEMENTI, P_FLAG_RILEVAZIONI,
																																					 P_FLAG_ALTRO, P_FLAG_ALTRO_DESC, P_FLAG_MACELLO_6,
																																					 P_VET_AZIENDALE, P_INDIRIZZO, P_TELEFONO,
																																					 P_ISTAT, P_SIGLA, P_NUM_ISCR_ALBO,
																																					 P_FLAG_USCITA_AUTOMATICA)

			'P_XML_CAPI
			Dim P_XML_ELENCO_CAPIRow As dsREGISTRAZ_USCITA_MODELLO_IUS.P_XML_ELENCO_CAPIRow = xml_RegistraModello4.P_XML_ELENCO_CAPI.AddP_XML_ELENCO_CAPIRow(PARAMETERS_LISTRow)
			Dim ELENCO_CAPIRow As dsREGISTRAZ_USCITA_MODELLO_IUS.ELENCO_CAPIRow = xml_RegistraModello4.ELENCO_CAPI.AddELENCO_CAPIRow(P_XML_ELENCO_CAPIRow)
			For Each capo In Lista_Capi
				xml_RegistraModello4.CAPO.AddCAPORow(capo, ELENCO_CAPIRow)
			Next
			'P_XML_CAPI

			'P_XML_TRATTAMENTI
			'al momento inserisce un'unica riga nulla
			Dim P_XML_TRATTAMENTIRow As dsREGISTRAZ_USCITA_MODELLO_IUS.P_XML_TRATTAMENTIRow = xml_RegistraModello4.P_XML_TRATTAMENTI.AddP_XML_TRATTAMENTIRow(PARAMETERS_LISTRow)
			Dim ELENCO_TRATTAMENTIRow As dsREGISTRAZ_USCITA_MODELLO_IUS.ELENCO_TRATTAMENTIRow = xml_RegistraModello4.ELENCO_TRATTAMENTI.AddELENCO_TRATTAMENTIRow(P_XML_TRATTAMENTIRow)
			Dim TRATTAMENTORow As dsREGISTRAZ_USCITA_MODELLO_IUS.TRATTAMENTORow = xml_RegistraModello4.TRATTAMENTO.AddTRATTAMENTORow("", "",
																																	 "", "",
																																	 "", "",
																																	 Nothing, Nothing,
																																	 ELENCO_TRATTAMENTIRow)
			'P_XML_TRATTAMENTI

			'P_XML_ESAMI
			'al momento inserisce un'unica riga nulla
			Dim P_XML_ESAMI As dsREGISTRAZ_USCITA_MODELLO_IUS.P_XML_ESAMIRow = xml_RegistraModello4.P_XML_ESAMI.AddP_XML_ESAMIRow(PARAMETERS_LISTRow)
			Dim ELENCO_ESAMIRow As dsREGISTRAZ_USCITA_MODELLO_IUS.ELENCO_ESAMIRow = xml_RegistraModello4.ELENCO_ESAMI.AddELENCO_ESAMIRow(P_XML_ESAMI)
			Dim ESAMERow As dsREGISTRAZ_USCITA_MODELLO_IUS.ESAMERow = xml_RegistraModello4.ESAME.AddESAMERow("", "",
																											 "", Nothing,
																											 "", "",
																											 "", "",
																											 ELENCO_ESAMIRow)
			'P_XML_ESAMI

			'P_XML_TRASPORTATORE
			Dim P_XML_TRASPORTATORE As dsREGISTRAZ_USCITA_MODELLO_IUS.P_XML_TRASPORTATORERow = xml_RegistraModello4.P_XML_TRASPORTATORE.AddP_XML_TRASPORTATORERow(PARAMETERS_LISTRow)
			Dim TRASPORTATORE As dsREGISTRAZ_USCITA_MODELLO_IUS.TRASPORTATORERow = xml_RegistraModello4.TRASPORTATORE.AddTRASPORTATORERow(P_XML_TRASPORTATORE)
			xml_RegistraModello4.DATI.AddDATIRow(P_TRASP_TARGA_MOTRICE, P_TRASP_TARGA, P_TRASP_CONDUCENTE,
												 P_TRASP_TARGA_RIMORCHIO, P_TRASP_NUM_AUTORIZZAZIONE,
												 P_TRASP_DT_PARTENZA, P_TRASP_ORA_PARTENZA,
												 P_TRASP_DURATA_VIAGGIO, P_TRASP_FLAG_MEZZO_PROPRIO,
												 P_TRASP_COD_ASL_TRASP, P_TRASP_DENOM_TRASPORTATORE,
												 P_TRASP_SL_COD_FISCALE,
												 TRASPORTATORE)
			'P_XML_TRASPORTATORE

			'genera la stringa dall'xml
			xml_RegistraModello4.AcceptChanges()

			Dim strXml_RegistraModello4 As String = xml_RegistraModello4.GetXml()

			'gestione xml
			Dim docXmlSwap As New XmlDocument
			docXmlSwap.LoadXml(strXml_RegistraModello4)

			Dim root As XmlElement = docXmlSwap.DocumentElement
			Dim XmlNode_New As XmlNode
			Dim XmlNode_Ref As XmlNode

			'swap P_XML_ESAMI
			XmlNode_New = root.FirstChild.Item("P_XML_ESAMI")
			XmlNode_Ref = root.FirstChild.Item("P_FLAG_MACELLO_2C")
			root.FirstChild.RemoveChild(XmlNode_New)
			root.FirstChild.InsertAfter(XmlNode_New, XmlNode_Ref)

			'swap P_XML_TRATTAMENTI 
			XmlNode_New = root.FirstChild.Item("P_XML_TRATTAMENTI")
			XmlNode_Ref = root.FirstChild.Item("P_FLAG_MACELLO_2C")
			root.FirstChild.RemoveChild(XmlNode_New)
			root.FirstChild.InsertAfter(XmlNode_New, XmlNode_Ref)

			'swap P_XML_CAPO
			XmlNode_New = root.FirstChild.Item("P_XML_ELENCO_CAPI")
			XmlNode_Ref = root.FirstChild.Item("P_NUM_ISCR_ALBO")
			root.FirstChild.RemoveChild(XmlNode_New)
			root.FirstChild.InsertAfter(XmlNode_New, XmlNode_Ref)

			'swap P_XML_TRASPORTATORE 
			XmlNode_New = root.FirstChild.Item("P_XML_TRASPORTATORE")
			XmlNode_Ref = root.FirstChild.Item("P_NUM_ISCR_ALBO")
			root.FirstChild.RemoveChild(XmlNode_New)
			root.FirstChild.InsertAfter(XmlNode_New, XmlNode_Ref)

			'eliminazione dati TRATTAMENTI e ESAMI 
			'al momento non vengono gestiti trattamenti ed esami sui capi in ingresso con Modello4
			'nel momento in cui verranno gestiti, modificare
			root.FirstChild.Item("P_XML_TRATTAMENTI").Item("ELENCO_TRATTAMENTI").Item("TRATTAMENTO").RemoveAll()
			root.FirstChild.Item("P_XML_ESAMI").Item("ELENCO_ESAMI").Item("ESAME").RemoveAll()


			If P_FLAG_MACELLO_2A = "N" Or P_FLAG_MACELLO_2A = "" Then
				root.FirstChild.Item("P_XML_TRATTAMENTI").RemoveAll()
				root.FirstChild.Item("P_XML_ESAMI").RemoveAll()
			End If

			'conversione date in formato ("yyyy-MM-dd")
			Dim XmlNode_Date As XmlNode

			'P_DT_USCITA
			XmlNode_Date = root.FirstChild.Item("P_DT_USCITA")
			If CDate(XmlNode_Date.InnerText) < AGRODATAINIZIO Then
				XmlNode_Date.InnerText = ""
			Else
				XmlNode_Date.InnerText = CDate(XmlNode_Date.InnerText).Date.ToString("yyyy-MM-dd")
			End If

			'P_DT_AUTORITA
			XmlNode_Date = root.FirstChild.Item("P_DT_AUTORITA")
			If CDate(XmlNode_Date.InnerText) < AGRODATAINIZIO Then
				XmlNode_Date.InnerText = ""
			Else
				XmlNode_Date.InnerText = CDate(XmlNode_Date.InnerText).Date.ToString("yyyy-MM-dd")
			End If

			'P_XML_TRASPORTATORE -> TRASPORTATORE -> DATI ->  DT_PARTENZA
			XmlNode_Date = root.FirstChild.Item("P_XML_TRASPORTATORE").Item("TRASPORTATORE").Item("DATI").Item("DT_PARTENZA")
			If CDate(XmlNode_Date.InnerText) < AGRODATAINIZIO Then
				XmlNode_Date.InnerText = ""
			Else
				XmlNode_Date.InnerText = CDate(XmlNode_Date.InnerText).Date.ToString("yyyy-MM-dd")
			End If

			'dopo i vari aggiustamenti al dannato xml, lo salva in stringa
			Dim finalXml_RegistraModello4 As String = docXmlSwap.InnerXml

			If P_FLAG_MACELLO_2A = "N" Or P_FLAG_MACELLO_2A = "" Then
				finalXml_RegistraModello4 = finalXml_RegistraModello4.Replace("<P_XML_TRATTAMENTI/>", "")
				finalXml_RegistraModello4 = finalXml_RegistraModello4.Replace("<P_XML_TRATTAMENTI></P_XML_TRATTAMENTI>", "")
				'"<P_XML_TRATTAMENTI></P_XML_TRATTAMENTI>"
				finalXml_RegistraModello4 = finalXml_RegistraModello4.Replace("<P_XML_ESAMI/>", "")
				finalXml_RegistraModello4 = finalXml_RegistraModello4.Replace("<P_XML_ESAMI></P_XML_ESAMI>", "")
			End If

			'genera la stringa con Escape dalla stringa dell'xml
			Dim esStrXml_RegistraModello4 As String = System.Security.SecurityElement.Escape(finalXml_RegistraModello4)

			Dim reqInsert As XmlNode = SoapRequestV2(esStrXml_RegistraModello4, Reflection.MethodBase.GetCurrentMethod().Name)

			dtRegistrazioneModello = MyBase.generateDTfromXml(reqInsert)
		Catch ex As ExpiredTokenBDNException
			Throw ex
		Catch ex As BDNException
			Throw ex
		Catch ex As Exception
			Throw ex
		End Try

		Return dtRegistrazioneModello

	End Function

	''' <summary>
	''' aggiornaPrenotazioneModello (wsModelliAccompagnamento.pdf, pg. 37)
	''' </summary>
	''' <param name="P_PRENOTAZIONE_ID"></param>
	''' <param name="P_AZIENDA_CODICE"></param>
	''' <param name="P_ALLEV_ID_FISCALE"></param>
	''' <param name="P_SPECIE_CODICE"></param>
	''' <param name="P_ESTREMI_DOCUMENTO"></param>
	''' <param name="P_DT_MODELLO"></param>
	''' <param name="P_GIORNI_VALIDITA"></param>
	''' <param name="P_FLAG_TIPO_NOTA"></param>
	''' <param name="P_NOTA"></param>
	''' <param name="P_VETERINARIO"></param>
	''' <param name="P_DETEN_PAS_ID_FISCALE"></param>
	''' <returns></returns>
	Public Function aggiornaPrenotazioneModello(ByVal P_PRENOTAZIONE_ID As Integer,
											   ByVal P_DOCUMENTO_ID As Integer,
											   ByVal P_AZIENDA_CODICE As String,
											   ByVal P_ALLEV_ID_FISCALE As String,
											   ByVal P_SPECIE_CODICE As String,
											   ByVal P_DEST_AZIENDA_CODICE As String,
											   ByVal P_DEST_ALLEV_ID_FISCALE As String,
											   ByVal P_DEST_SPECIE_CODICE As String,
											   ByVal P_FIERA_CODICE As String,
											   ByVal P_STATO_CODICE As String,
											   ByVal P_PASCOLO_CODICE As String,
											   ByVal P_MACELLO_CODICE As String,
											   ByVal P_REGIONE_CODICE As String,
											   ByVal P_ESTREMI_DOCUMENTO As String,
											   ByVal Lista_Capi As List(Of XmlCapi_InvioModelli), ' = P_XML_CAPO
											   ByVal P_DT_USCITA As Date,
											   ByVal P_CAUSALE As String,
											   ByVal P_TIPO_STAMPA As String,
											   ByVal P_FLAG_MACELLO_1 As String,
											   ByVal P_FLAG_MACELLO_2 As String,
											   ByVal P_FLAG_MACELLO_2A As String,
											   ByVal P_FLAG_MACELLO_2B As String,
											   ByVal P_FLAG_MACELLO_2C As String,
											   ByVal Lista_Trattamenti As List(Of XmlTrattamenti_InvioModelli), ' = P_XML_TRATTAMENTI
											   ByVal Lista_Esami As List(Of XmlEsamiCapo_InvioModelli), ' = P_XML_ESAMI
											   ByVal P_FLAG_MACELLO_3 As String,
											   ByVal P_FLAG_MACELLO_3_ENTERICI As String,
											   ByVal P_FLAG_MACELLO_3_RESPIRATORI As String,
											   ByVal P_FLAG_MACELLO_3_CUTANEI As String,
											   ByVal P_FLAG_MACELLO_3_LOCOMOTORI As String,
											   ByVal P_FLAG_MACELLO_3_ALTRO As String,
											   ByVal P_FLAG_MACELLO_3_ALTRO_DESC As String,
											   ByVal P_FLAG_MACELLO_4 As String,
											   ByVal P_FLAG_MACELLO_5 As String,
											   ByVal P_FLAG_ELEMENTI As String,
											   ByVal P_FLAG_RILEVAZIONI As String,
											   ByVal P_FLAG_ALTRO As String,
											   ByVal P_FLAG_ALTRO_DESC As String,
											   ByVal P_FLAG_UPLOAD As String,
											   ByVal P_FLAG_MACELLO_6 As String,
											   ByVal P_VET_AZIENDALE As String,
											   ByVal P_INDIRIZZO As String,
											   ByVal P_TELEFONO As String,
											   ByVal P_ISTAT As String,
											   ByVal P_SIGLA As String,
											   ByVal P_NUM_ISCR_ALBO As String,
											   ByVal P_TRASP_TARGA_MOTRICE As String, 'P_XML_TRASPORTATORE INIZIO
											   ByVal P_TRASP_TARGA As String,
											   ByVal P_TRASP_CONDUCENTE As String,
											   ByVal P_TRASP_DENOM_TRASPORTATORE As String,
											   ByVal P_TRASP_TARGA_RIMORCHIO As String,
											   ByVal P_TRASP_NUM_AUTORIZZAZIONE As String,
											   ByVal P_TRASP_DT_PARTENZA As Date,
											   ByVal P_TRASP_ORA_PARTENZA As String,
											   ByVal P_TRASP_DURATA_VIAGGIO As String,
											   ByVal P_TRASP_FLAG_MEZZO_PROPRIO As String,
											   ByVal P_TRASP_COD_ASL_TRASP As String,
											   ByVal P_TRASP_SL_COD_FISCALE As String, 'P_XML_TRASPORTATORE FINE
											   ByVal P_FLAG_USCITA_AUTOMATICA As String,
											   ByVal P_CONFERMA_BDR As String,
											   ByVal P_DT_DOCUMENTO As Date,
											   ByVal P_GIORNI_VALIDITA As String,
											   ByVal P_FLAG_TIPO_NOTA As String,
											   ByVal P_NOTA As String,
											   ByVal P_VETERINARIO As String,
											   ByVal P_DETEN_PAS_ID_FISCALE As String,
											   ByVal P_SIGLA_AUTOC As String,
											   ByVal P_ISTAT_AUTOC As String,
											   ByVal P_ID_FISCALE_AUTOC As String,
											   ByVal P_DT_RIENTRO As Date,
											   ByVal P_DESCR_PERCORSO As String,
											   ByVal P_SIGLA_DEST As String,
											   ByVal P_ISTAT_DEST As String) As DataTable
		Dim dtUpdateModello4 As New DataTable

		Try
			Dim xmlUpdateMod4 As New dsPRENOTAZIONE_MODELLO_IUS()
			xmlUpdateMod4.InitVars()
			xmlUpdateMod4.EnforceConstraints = True

			xmlUpdateMod4.PARAMETERS_LIST.InitVars()
			Dim PARAMETERS_LISTRow =
				xmlUpdateMod4.PARAMETERS_LIST.AddPARAMETERS_LISTRow(P_PRENOTAZIONE_ID, P_DOCUMENTO_ID,
																	P_AZIENDA_CODICE, P_ALLEV_ID_FISCALE, P_SPECIE_CODICE,
																	P_DEST_AZIENDA_CODICE, P_DEST_ALLEV_ID_FISCALE, P_DEST_SPECIE_CODICE,
																	P_FIERA_CODICE, P_PASCOLO_CODICE,
																	P_MACELLO_CODICE, P_REGIONE_CODICE,
																	P_ESTREMI_DOCUMENTO.PadLeft(5, "0"c), CDate(P_DT_USCITA),
																	P_CAUSALE, P_TIPO_STAMPA, P_FLAG_MACELLO_1, P_FLAG_MACELLO_2,
																	P_FLAG_MACELLO_2A, P_FLAG_MACELLO_2B, P_FLAG_MACELLO_2C,
																	P_FLAG_MACELLO_3, P_FLAG_MACELLO_3_ENTERICI,
																	P_FLAG_MACELLO_3_RESPIRATORI, P_FLAG_MACELLO_3_CUTANEI,
																	P_FLAG_MACELLO_3_LOCOMOTORI, P_FLAG_MACELLO_3_ALTRO,
																	P_FLAG_MACELLO_3_ALTRO_DESC,
																	P_FLAG_MACELLO_4, P_FLAG_MACELLO_5,
																	P_FLAG_ELEMENTI, P_FLAG_RILEVAZIONI,
																	P_FLAG_ALTRO, P_FLAG_ALTRO_DESC,
																	P_FLAG_UPLOAD, P_FLAG_MACELLO_6,
																	P_VET_AZIENDALE, P_INDIRIZZO, P_TELEFONO, P_ISTAT,
																	P_SIGLA, P_NUM_ISCR_ALBO,
																	P_FLAG_USCITA_AUTOMATICA, P_CONFERMA_BDR,
																	CDate(P_DT_DOCUMENTO), P_GIORNI_VALIDITA,
																	P_FLAG_TIPO_NOTA, P_NOTA, P_VETERINARIO, P_DETEN_PAS_ID_FISCALE,
																	P_SIGLA_AUTOC, P_ISTAT_AUTOC, P_ID_FISCALE_AUTOC,
																	CDate(P_DT_RIENTRO), P_DESCR_PERCORSO,
																	P_SIGLA_DEST, P_ISTAT_DEST)

			'P_XML_CAPI
			Dim P_XML_CAPIRow = xmlUpdateMod4.P_XML_CAPO.AddP_XML_CAPORow(PARAMETERS_LISTRow)
			Dim ELENCO_CAPIRow = xmlUpdateMod4.ELENCO_CAPI.AddELENCO_CAPIRow(P_XML_CAPIRow)
			For Each capo In Lista_Capi
				xmlUpdateMod4.CAPO.AddCAPORow(capo.capoCodice, capo.codiceElettronico,
											  capo.identNome, capo.passaporto, capo.codiceUeln,
											  ELENCO_CAPIRow)
			Next

			'P_XML_TRATTAMENTI
			' Al momento inserisce un'unica riga nulla
			Dim P_XML_TRATTAMENTIRow = xmlUpdateMod4.P_XML_TRATTAMENTI.AddP_XML_TRATTAMENTIRow(PARAMETERS_LISTRow)
			Dim ELENCO_TRATTAMENTIRow = xmlUpdateMod4.ELENCO_TRATTAMENTI.AddELENCO_TRATTAMENTIRow(P_XML_TRATTAMENTIRow)
			Dim TRATTAMENTORow = xmlUpdateMod4.TRATTAMENTO.AddTRATTAMENTORow("", "", "", "",
																			 "", "", Nothing, Nothing,
																			 ELENCO_TRATTAMENTIRow)

			'P_XML_ESAMI
			' Al momento inserisce un'unica riga nulla
			Dim P_XML_ESAMI = xmlUpdateMod4.P_XML_ESAMI.AddP_XML_ESAMIRow(PARAMETERS_LISTRow)
			Dim ELENCO_ESAMIRow = xmlUpdateMod4.ELENCO_ESAMI.AddELENCO_ESAMIRow(P_XML_ESAMI)
			Dim ESAMERow = xmlUpdateMod4.ESAME.AddESAMERow("", "", "", Nothing,
														   "", "", "", "",
														   ELENCO_ESAMIRow)

			'P_XML_TRASPORTATORE
			Dim P_XML_TRASPORTATORE = xmlUpdateMod4.P_XML_TRASPORTATORE.AddP_XML_TRASPORTATORERow(PARAMETERS_LISTRow)
			Dim TRASPORTATORE = xmlUpdateMod4.TRASPORTATORE.AddTRASPORTATORERow(P_XML_TRASPORTATORE)
			xmlUpdateMod4.DATI.AddDATIRow(P_TRASP_TARGA_MOTRICE, P_TRASP_TARGA, P_TRASP_CONDUCENTE,
										  P_TRASP_TARGA_RIMORCHIO, P_TRASP_NUM_AUTORIZZAZIONE,
										  CDate(P_TRASP_DT_PARTENZA), P_TRASP_ORA_PARTENZA, P_TRASP_DURATA_VIAGGIO,
										  P_TRASP_FLAG_MEZZO_PROPRIO, P_TRASP_COD_ASL_TRASP,
										  P_TRASP_DENOM_TRASPORTATORE, P_TRASP_SL_COD_FISCALE,
										  TRASPORTATORE)

			xmlUpdateMod4.AcceptChanges()
			Dim strXml_InserisciModello4 As String = xmlUpdateMod4.GetXml()

			Dim docXmlSwap As New XmlDocument
			docXmlSwap.LoadXml(strXml_InserisciModello4)

			Dim root As XmlElement = docXmlSwap.DocumentElement
			Dim XmlNode_New As XmlNode
			Dim XmlNode_Ref As XmlNode

			' Swap P_XML_CAPO
			XmlNode_New = root.FirstChild.Item("P_XML_CAPO")
			XmlNode_Ref = root.FirstChild.Item("P_ESTREMI_DOCUMENTO")
			root.FirstChild.RemoveChild(XmlNode_New)
			root.FirstChild.InsertAfter(XmlNode_New, XmlNode_Ref)

			' Swap P_XML_ESAMI
			XmlNode_New = root.FirstChild.Item("P_XML_ESAMI")
			XmlNode_Ref = root.FirstChild.Item("P_FLAG_MACELLO_2C")
			root.FirstChild.RemoveChild(XmlNode_New)
			root.FirstChild.InsertAfter(XmlNode_New, XmlNode_Ref)

			' Swap P_XML_TRATTAMENTI 
			XmlNode_New = root.FirstChild.Item("P_XML_TRATTAMENTI")
			XmlNode_Ref = root.FirstChild.Item("P_FLAG_MACELLO_2C")
			root.FirstChild.RemoveChild(XmlNode_New)
			root.FirstChild.InsertAfter(XmlNode_New, XmlNode_Ref)

			' Swap P_XML_TRASPORTATORE 
			XmlNode_New = root.FirstChild.Item("P_XML_TRASPORTATORE")
			XmlNode_Ref = root.FirstChild.Item("P_NUM_ISCR_ALBO")
			root.FirstChild.RemoveChild(XmlNode_New)
			root.FirstChild.InsertAfter(XmlNode_New, XmlNode_Ref)

			' Eliminazione dati TRATTAMENTI e ESAMI:
			' Al momento non vengono gestiti trattamenti ed esami sui capi in ingresso con Modello4;
			' nel momento in cui verranno gestiti, modificare
			root.FirstChild.Item("P_XML_TRATTAMENTI").Item("ELENCO_TRATTAMENTI").Item("TRATTAMENTO").RemoveAll()
			root.FirstChild.Item("P_XML_ESAMI").Item("ELENCO_ESAMI").Item("ESAME").RemoveAll()
			root.FirstChild.Item("P_XML_TRATTAMENTI").RemoveAll()
			root.FirstChild.Item("P_XML_ESAMI").RemoveAll()

			' Conversione date in formato ("yyyy-MM-dd")
			Dim XmlNode_Date As XmlNode

			' P_DT_USCITA
			XmlNode_Date = root.FirstChild.Item("P_DT_USCITA")
			If CDate(XmlNode_Date.InnerText) < AGRODATAINIZIO Then
				XmlNode_Date.InnerText = ""
			Else
				XmlNode_Date.InnerText = CDate(XmlNode_Date.InnerText).Date.ToString("yyyy-MM-dd")
			End If

			'P_DT_DOCUMENTO
			XmlNode_Date = root.FirstChild.Item("P_DT_DOCUMENTO")
			If CDate(XmlNode_Date.InnerText) < AGRODATAINIZIO Then
				XmlNode_Date.InnerText = ""
			Else
				XmlNode_Date.InnerText = CDate(XmlNode_Date.InnerText).Date.ToString("yyyy-MM-dd")
			End If

			' P_DT_RIENTRO
			XmlNode_Date = root.FirstChild.Item("P_DT_RIENTRO")
			If CDate(XmlNode_Date.InnerText) < AGRODATAINIZIO Then
				XmlNode_Date.InnerText = ""
				root.FirstChild.RemoveChild(XmlNode_Date)
			Else
				XmlNode_Date.InnerText = CDate(XmlNode_Date.InnerText).Date.ToString("yyyy-MM-dd")
			End If

			' P_XML_TRASPORTATORE -> TRASPORTATORE -> DATI ->  DT_PARTENZA
			XmlNode_Date = root.FirstChild.Item("P_XML_TRASPORTATORE").Item("TRASPORTATORE").Item("DATI").Item("DT_PARTENZA")
			If CDate(XmlNode_Date.InnerText) < AGRODATAINIZIO Then
				XmlNode_Date.InnerText = ""
			Else
				XmlNode_Date.InnerText = CDate(XmlNode_Date.InnerText).Date.ToString("yyyy-MM-dd")
			End If

			Dim finalXml_InserisciModello4 As String = docXmlSwap.InnerXml
			If (P_CAUSALE = "M" Or P_CAUSALE = "V") Then
				finalXml_InserisciModello4 = finalXml_InserisciModello4.Replace("<P_XML_TRATTAMENTI/>", "")
				finalXml_InserisciModello4 = finalXml_InserisciModello4.Replace("<P_XML_TRATTAMENTI></P_XML_TRATTAMENTI>", "")
				'"<P_XML_TRATTAMENTI></P_XML_TRATTAMENTI>"
				finalXml_InserisciModello4 = finalXml_InserisciModello4.Replace("<P_XML_ESAMI/>", "")
				finalXml_InserisciModello4 = finalXml_InserisciModello4.Replace("<P_XML_ESAMI></P_XML_ESAMI>", "")
				'"<P_XML_ESAMI></P_XML_ESAMI>"
			End If

			' Dopo i vari aggiustamenti al dannato xml, lo salva in stringa
			Dim finalXmlUpdateMod4 As String = docXmlSwap.InnerXml
			Dim reqInsert As XmlNode = SoapRequestV2(SecurityElement.Escape(finalXmlUpdateMod4), MethodBase.GetCurrentMethod().Name)

			dtUpdateModello4 = generateDTfromXml(reqInsert)

		Catch ex As ExpiredTokenBDNException
			Throw ex
		Catch ex As BDNException
			Throw New BDNException("Errore: " & MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)
		Catch ex As Exception
			Throw New Exception("Errore: " & MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)
		End Try

		Return dtUpdateModello4

	End Function

	''' <summary>
	''' insertCapoModello (wsModelliAccompagnamento.pdf, pg. 44)
	''' </summary>
	''' <param name="P_LISTA_PRENOT_ID"></param>
	''' <param name="P_PRENOTAZIONE_ID"></param>
	''' <param name="P_CODICE_CAPO"></param>
	''' <param name="P_DT_USCITA"></param>
	''' <param name="P_FLAG_ESAMINATO"></param>
	''' <returns></returns>
	Public Function insertCapoModello(ByVal P_LISTA_PRENOT_ID As Integer,
									  ByVal P_PRENOTAZIONE_ID As Integer,
									  ByVal P_CODICE_CAPO As String,
									  ByVal P_DT_USCITA As Date,
									  ByVal P_FLAG_ESAMINATO As String) As DataTable
		Dim dtInsertCapoModello As New DataTable

		Try
			Dim xmlInsCapoModello As New dsINS_DEL_CAPO_MODELLO_IUS()
			xmlInsCapoModello.InitVars()
			xmlInsCapoModello.EnforceConstraints = True

			xmlInsCapoModello.PARAMETERS_LIST.InitVars()
			xmlInsCapoModello.PARAMETERS_LIST.AddPARAMETERS_LISTRow(P_LISTA_PRENOT_ID, P_PRENOTAZIONE_ID, P_CODICE_CAPO, P_DT_USCITA, P_FLAG_ESAMINATO)

			Dim STRxmlInsCapoModello As String = xmlInsCapoModello.GetXml()

			Dim docXmlSwap As New XmlDocument
			docXmlSwap.LoadXml(STRxmlInsCapoModello)

			' Conversione date in formato ("yyyy-MM-dd")
			Dim XmlNode_Date As XmlNode
			Dim root As XmlElement = docXmlSwap.DocumentElement

			'P_DT_USCITA
			XmlNode_Date = root.FirstChild.Item("P_DT_USCITA")
			If CDate(XmlNode_Date.InnerText) < AGRODATAINIZIO Then
				XmlNode_Date.InnerText = ""
			Else
				XmlNode_Date.InnerText = CDate(XmlNode_Date.InnerText).Date.ToString("yyyy-MM-dd")
			End If

			' Dopo i vari aggiustamenti al dannato xml, lo salva in stringa
			Dim finalXmlInsCapoModello As String = docXmlSwap.InnerXml

			Dim reqInsert As XmlNode = SoapRequestV2(SecurityElement.Escape(finalXmlInsCapoModello), MethodBase.GetCurrentMethod().Name)

			If reqInsert.InnerText.Contains("OPERAZIONE EFFETTUATA CON SUCCESSO") Then Return dtInsertCapoModello
			dtInsertCapoModello = generateDTfromXml(reqInsert)

		Catch ex As ExpiredTokenBDNException
			Throw ex
		Catch ex As BDNException
			Throw ex
		Catch ex As Exception
			Throw ex
		End Try

		Return dtInsertCapoModello

	End Function

	''' <summary>
	''' deleteCapoModello (wsModelliAccompagnamento.pdf, pg. 50)
	''' </summary>
	''' <param name="P_LISTA_PRENOT_ID"></param>
	''' <param name="P_PRENOTAZIONE_ID"></param>
	''' <param name="P_CODICE_CAPO"></param>
	''' <param name="P_DT_USCITA"></param>
	''' <param name="P_FLAG_ESAMINATO"></param>
	''' <returns></returns>
	Public Function deleteCapoModello(ByVal P_LISTA_PRENOT_ID As Integer,
									  ByVal P_PRENOTAZIONE_ID As Integer,
									  ByVal P_CODICE_CAPO As String,
									  ByVal P_DT_USCITA As Date,
									  ByVal P_FLAG_ESAMINATO As String) As DataSet
		Dim dsCancCapoModello As New DataSet

		Try
			Dim xmlCancCapoModello As New dsINS_DEL_CAPO_MODELLO_IUS()
			xmlCancCapoModello.InitVars()
			xmlCancCapoModello.EnforceConstraints = True

			xmlCancCapoModello.PARAMETERS_LIST.InitVars()
			xmlCancCapoModello.PARAMETERS_LIST.AddPARAMETERS_LISTRow(P_LISTA_PRENOT_ID, P_PRENOTAZIONE_ID, P_CODICE_CAPO, P_DT_USCITA, P_FLAG_ESAMINATO)

			Dim STRxmlCancCapoModello As String = xmlCancCapoModello.GetXml()

			Dim docXmlSwap As New XmlDocument
			docXmlSwap.LoadXml(STRxmlCancCapoModello)

			' Conversione date in formato ("yyyy-MM-dd")
			Dim XmlNode_Date As XmlNode
			Dim root As XmlElement = docXmlSwap.DocumentElement

			'P_DT_USCITA
			XmlNode_Date = root.FirstChild.Item("P_DT_USCITA")
			If CDate(XmlNode_Date.InnerText) < AGRODATAINIZIO Then
				XmlNode_Date.InnerText = ""
			Else
				XmlNode_Date.InnerText = CDate(XmlNode_Date.InnerText).Date.ToString("yyyy-MM-dd")
			End If

			' Dopo i vari aggiustamenti al dannato xml, lo salva in stringa
			Dim finalXmlCancCapoModello As String = docXmlSwap.InnerXml

			Dim reqInsert As XmlNode = SoapRequestV2(SecurityElement.Escape(finalXmlCancCapoModello), MethodBase.GetCurrentMethod().Name)
			If reqInsert.InnerXml.Contains("PARAMETERS_LIST") Then Return dsCancCapoModello

			dsCancCapoModello = generateDSfromXml(reqInsert)

		Catch ex As ExpiredTokenBDNException
			Throw ex
		Catch ex As BDNException
			Throw ex
		Catch ex As Exception
			Throw ex
		End Try

		Return dsCancCapoModello

	End Function

End Class
