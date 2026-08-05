Imports System.Data.OleDb
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports System.Reflection
Imports System.Text

'Questa classe viene utilizzata per leggere dal DB generico GIAS_SUPER_SERVER la tabella CONNESSIONI
'da cui ricavare le connessioni ai vari db server, utenti etc utilizzabili dall'installazione del giasonline presente


'cnAWS_Disciplinari as string
'cnAWS_Server
'cnAWS_Tabelle
'cnGIAS_DPI
'cnGIAS_Meteo
'cnONLINE_Server
'cnONLINE_Server
'cnONLINE_Utenti
'cnONLINE_Utenti
'cnPIANOCONCIMAZIONE_PUA_Server



Public Class Connessioni
	Inherits AgronicaCoreDataProvider.DataProvider

	''' <summary>
	''' conversione da string Ole db a stringa SqlClient
	''' </summary>
	''' <param name="oleDB_connnectionString"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Shared Function OleDB_Converti_SqlClient(ByVal oleDB_connnectionString As String) As String
		Dim vAppoggio As String() = oleDB_connnectionString.Split(New Char() {";"c, "="c})

		Dim rval As String = ""

		For i As Integer = 0 To vAppoggio.Length - 2 Step 2
			Select Case vAppoggio(i).ToLower
				Case "provider"
				Case Else
					rval &= vAppoggio(i) & "= " & vAppoggio(i + 1) & ";"
			End Select
		Next

		Return rval & "multipleactiveresultsets=True;App=EntityFramework"

	End Function


	'###################################################################
	Public Function Recupera_IdDb(ByVal TipoDB As TipiEnumerativi.enum_Tipo_DB,
								  ByVal Server As String,
								  ByVal DB As String,
								  ByVal Provider As String,
								  ByVal UserId As String,
								  ByVal Password As String,
								  ByVal PivaSuperUser As String,
								  ByVal Note As String,
								  ByVal Progressivo As Integer,
								  ByVal Descrizione As String,
								  ByVal xFiltroAggiuntivo As String,
								  ByVal xOrderBy As String,
								  ByRef objParametri As AgronicaCoreParametri
								  ) As Integer

		Const nomeRoutine = "AgronicaCoreDataProvider.Connessioni.Recupera_IdDb()"

		Dim messaggioErrore As String = ""
		Dim dt As DataTable
		Dim ID_DB As Integer = 0

		Try

			'---------------------------------------------
			dt = Leggi(0,
					   TipoDB,
					   Server,
					   DB,
					   Provider,
					   UserId,
					   Password,
					   PivaSuperUser,
					   Note,
					   Progressivo,
					   Descrizione,
					   AGRODATAINIZIO,
					   AGRODATAFINE,
					   xFiltroAggiuntivo,
					   xOrderBy,
					   objParametri)

			If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
				ID_DB = CInt(dt.Rows(0).Item("ID_DB"))
			End If

		Catch ex As Exception
			messaggioErrore = ex.Message
			Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
			dt = Nothing
			Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
		End Try

		Return ID_DB

	End Function

	'###################################################################
	Public Function Recupera_ConnessioneGiasLan(ByVal TipoDB As TipiEnumerativi.enum_Tipo_DB,
												  ByVal Server As String,
												  ByVal DB As String,
												  ByVal Provider As String,
												  ByVal UserId As String,
												  ByVal Password As String,
												  ByVal PivaSuperUser As String,
												  ByVal Note As String,
												  ByVal Progressivo As Integer,
												  ByVal Descrizione As String,
												  ByVal xFiltroAggiuntivo As String,
												  ByVal xOrderBy As String,
												  ByRef objParametri As AgronicaCoreParametri
												  ) As String

		Const nomeRoutine = "AgronicaCoreDataProvider.Connessioni.Recupera_ConnessioneGiasLan()"

		Dim messaggioErrore As String = ""
		Dim dt As DataTable
		'Dim ID_DB As Integer = 0
		Dim ConnessioneGiasLan_DBserver As String = ""

		Try

			'---------------------------------------------
			dt = Leggi(0,
					   TipoDB,
					   Server,
					   DB,
					   Provider,
					   UserId,
					   Password,
					   PivaSuperUser,
					   Note,
					   Progressivo,
					   Descrizione,
					   AGRODATAINIZIO,
					   AGRODATAFINE,
					   xFiltroAggiuntivo,
					   xOrderBy,
					   objParametri)

			If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
				ConnessioneGiasLan_DBserver = CStr(dt.Rows(0).Item("ConnessioneGiasLan"))
			End If

		Catch ex As Exception
			messaggioErrore = ex.Message
			Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
			dt = Nothing
			Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
		End Try

		Return ConnessioneGiasLan_DBserver

	End Function

	'###################################################################
	Public Function Recupera_StringaConnessione(ByVal ID_DB As Integer,
												ByVal TipoDB As TipiEnumerativi.enum_Tipo_DB,
												ByVal Server As String,
												ByVal DB As String,
												ByVal Provider As String,
												ByVal UserId As String,
												ByVal Password As String,
												ByVal PivaSuperUser As String,
												ByVal Note As String,
												ByVal Progressivo As Integer,
												ByVal Descrizione As String,
												ByVal xFiltroAggiuntivo As String,
												ByVal xOrderBy As String,
												ByRef objParametri As AgronicaCoreParametri
												) As String

		Const nomeRoutine = "AgronicaCoreDataProvider.Connessioni.Recupera_StringaConnessione()"

		Dim messaggioErrore As String = ""
		Dim dt As DataTable
		Dim Stringa_Connessione As String = ""

		Try

			'---------------------------------------------
			dt = Leggi(ID_DB,
					   TipoDB,
					   Server,
					   DB,
					   Provider,
					   UserId,
					   Password,
					   PivaSuperUser,
					   Note,
					   Progressivo,
					   Descrizione,
					   AGRODATAINIZIO,
					   AGRODATAFINE,
					   xFiltroAggiuntivo,
					   xOrderBy,
					   objParametri)

			If dt.Rows.Count = 0 Then
				Throw New Exception("Nessun record.")
			End If
			If dt.Rows.Count > 1 Then
				Throw New Exception("Sono stati trovati più record, non consentito.")
			End If

			If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
				Stringa_Connessione = "Provider=" & CStr(dt.Rows(0).Item("Provider")) &
										";Server=" & CStr(dt.Rows(0).Item("Server")) &
										";Initial Catalog=" & CStr(dt.Rows(0).Item("DB")) &
										";User Id=" & CStr(dt.Rows(0).Item("UserId")) &
										";Password=" & CStr(dt.Rows(0).Item("Password")) & ";"
			End If

		Catch ex As Exception
			messaggioErrore = ex.Message
			Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
			dt = Nothing
			Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
		End Try

		Return Stringa_Connessione

	End Function

	'###################################################################
	Public Function Recupera_Lista_StringheConnessione(ByVal ID_DB As Integer,
													   ByVal TipoDB As TipiEnumerativi.enum_Tipo_DB,
													   ByVal Server As String,
													   ByVal DB As String,
													   ByVal Provider As String,
													   ByVal UserId As String,
													   ByVal Password As String,
													   ByVal PivaSuperUser As String,
													   ByVal Note As String,
													   ByVal Progressivo As Integer,
													   ByVal Descrizione As String,
													   ByVal xFiltroAggiuntivo As String,
													   ByVal xOrderBy As String,
													   ByRef objParametri As AgronicaCoreParametri
													   ) As List(Of String)

		Const nomeRoutine = "AgronicaCoreDataProvider.Connessioni.Recupera_Lista_StringheConnessione()"

		Dim messaggioErrore As String = ""
		Dim dt As DataTable
		Dim listStrConnessioni As New List(Of String)

		Try

			'---------------------------------------------
			dt = Leggi(ID_DB,
					   TipoDB,
					   Server,
					   DB,
					   Provider,
					   UserId,
					   Password,
					   PivaSuperUser,
					   Note,
					   Progressivo,
					   Descrizione,
					   AGRODATAINIZIO,
					   AGRODATAFINE,
					   xFiltroAggiuntivo,
					   xOrderBy,
					   objParametri)

			If Not IsNothing(dt) Then

				For Each row As DataRow In dt.Rows

					Dim Stringa_Connessione As String = "Provider=" & CStr(row.Item("Provider")) &
														";Server=" & CStr(row.Item("Server")) &
														";Initial Catalog=" & CStr(row.Item("DB")) &
														";User Id=" & CStr(row.Item("UserId")) &
														";Password=" & CStr(row.Item("Password")) & ";"

					listStrConnessioni.Add(Stringa_Connessione)
				Next

			End If

		Catch ex As Exception
			messaggioErrore = ex.Message
			Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
			dt = Nothing
			Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
		End Try

		Return listStrConnessioni

	End Function

	Public Function Leggi_Connessione(ByVal ID_DB As Integer, ByRef objParametriSuperServer As AgronicaCoreParametri) As DataTable

		Const nomeRoutine = "AgronicaCoreVarieDAL.Connessioni.Leggi_Connessione()"

		Dim messaggioErrore As String = ""
		Dim dt As DataTable

		Try

			If ID_DB < 1 Then
				Throw New Exception("Occorre specificare l'id database (maggiore di 0)")
			End If

			dt = Leggi(ID_DB,
					   TipiEnumerativi.enum_Tipo_DB.TUTTI,
					   "",
					   "",
					   "",
					   "",
					   "",
					   "",
					   "",
					   0,
					   "",
					   AGRODATAINIZIO,
					   AGRODATAFINE,
					   "",
					   "",
					   objParametriSuperServer)

			If dt.Rows.Count = 0 Then
				Throw New Exception("Non è stato trovato il record del server gias selezionato")
			End If
			If dt.Rows.Count > 1 Then
				Throw New Exception("Sono stati caricati più server gias, non è consentito")
			End If

		Catch ex As Exception
			messaggioErrore = ex.Message
			Scrivi_LOG(objParametriSuperServer, nomeRoutine, messaggioErrore)
			Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
		End Try

		Return dt

	End Function

	Public Function Leggi_Stringa_Connessione(ByVal ID_DB As Integer, ByRef objParametriSuperServer As AgronicaCoreParametri) As String

		Const nomeRoutine = "AgronicaCoreVarieDAL.Connessioni.Leggi_Stringa_Connessione()"

		Dim messaggioErrore As String = ""
		Dim Stringa_Connessione As String = ""

		Try

			Dim dt As DataTable = Leggi_Connessione(ID_DB, objParametriSuperServer)

			Stringa_Connessione = "Provider=" & CStr(dt.Rows(0).Item("Provider")) &
								  ";Server=" & CStr(dt.Rows(0).Item("Server")) &
								  ";Initial Catalog=" & CStr(dt.Rows(0).Item("DB")) &
								  ";User Id=" & CStr(dt.Rows(0).Item("UserId")) &
								  ";Password=" & CStr(dt.Rows(0).Item("Password")) & ";"

		Catch ex As Exception
			messaggioErrore = ex.Message
			Scrivi_LOG(objParametriSuperServer, nomeRoutine, messaggioErrore)
			Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
		End Try

		Return Stringa_Connessione

	End Function


	Public Function Leggi(ByVal ID_DB As Integer,
						  ByVal TipoDB As TipiEnumerativi.enum_Tipo_DB,
						  ByVal Server As String,
						  ByVal DB As String,
						  ByVal Provider As String,
						  ByVal UserId As String,
						  ByVal Password As String,
						  ByVal PivaSuperUser As String,
						  ByVal Note As String,
						  ByVal Progressivo As Integer,
						  ByVal Descrizione As String,
						  ByVal Validita_Inizio As Date,
						  ByVal Validita_Fine As Date,
						  ByVal xFiltroAggiuntivo As String,
						  ByVal xOrderBy As String,
						  ByRef objParametriSuperServer As AgronicaCoreDataProvider.AgronicaCoreParametri
						  ) As DataTable

		Dim NomeRoutine As String = "AgronicaCoreVarieDAL.Connessioni.Leggi()"

		'====================================================================================
		'Parametri opzionali :
		'
		'====================================================================================

		Dim MessaggioErrore As String = ""
		Dim StrSQL As New System.Text.StringBuilder
		Dim DT As DataTable

		Try

			StrSQL.Length = 0

			StrSQL.Append(" SELECT * ")
			StrSQL.Append(" FROM  Connessioni ")
			StrSQL.Append(" WHERE 1=1 ")

			'correzione bug del 17/04/2014: non va bene il filtro secco sulla data odierna
			'(esempio da considerare: giaslan su archivio storicizzato che chiama le stampe)
			'StrSQL.Append(" AND   (Validita_Inizio <= " & Agro_SQL_SaveDate(Now.Date) & ")  ")
			'StrSQL.Append(" AND     (Validita_Fine >= " & Agro_SQL_SaveDate(Now.Date) & ") ")

			StrSQL.Append(" AND     (Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & ")  ")
			StrSQL.Append(" AND     (Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & ") ")

			If ID_DB <> 0 Then
				StrSQL.Append(" AND ID_DB =" & Agro_SQL_SaveNum(ID_DB) & " ")
			End If

			If TipoDB <> TipiEnumerativi.enum_Tipo_DB.TUTTI Then
				StrSQL.Append(" AND TipoDB =" & Agro_SQL_SaveNum(TipoDB) & " ")
			End If

			If Server <> "" Then
				StrSQL.Append(" AND Server ='" & Agro_SQL_SaveText(Server) & "' ")
			End If

			If DB <> "" Then
				StrSQL.Append(" AND DB ='" & Agro_SQL_SaveText(DB) & "' ")
			End If

			If Provider <> "" Then
				StrSQL.Append(" AND Provider ='" & Agro_SQL_SaveText(Provider) & "' ")
			End If

			If UserId <> "" Then
				StrSQL.Append(" AND UserId ='" & Agro_SQL_SaveText(UserId) & "' ")
			End If
			If Password <> "" Then
				StrSQL.Append(" AND Password ='" & Agro_SQL_SaveText(Password) & "' ")
			End If

			If PivaSuperUser <> "" Then
				StrSQL.Append(" AND PivaSuperUser ='" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
			End If

			If Note <> "" Then
				StrSQL.Append(" AND Note ='" & Agro_SQL_SaveText(Note) & "' ")
			End If

			If Progressivo <> 0 Then
				StrSQL.Append(" AND Progressivo =" & Agro_SQL_SaveNum(Progressivo) & " ")
			End If

			If Descrizione <> "" Then
				StrSQL.Append(" AND Descrizione ='" & Agro_SQL_SaveText(Descrizione) & "' ")
			End If

			'---------------------------------------------

			If xFiltroAggiuntivo <> "" Then
				StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametriSuperServer))
			End If

			'--------------------------------------------------------------------------
			If xOrderBy <> "" Then
				StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametriSuperServer))
			Else
				StrSQL.Append(" ORDER BY PivaSuperUser,DB ")
			End If


			'--------------------------------------------------------------------------
			DT = EseguiQuery_Lettura(objParametriSuperServer, StrSQL.ToString, NomeRoutine)
			'--------------------------------------------------------------------------

		Catch ex As Exception
			MessaggioErrore = ex.Message
			Scrivi_LOG(objParametriSuperServer, NomeRoutine, MessaggioErrore)
			DT = Nothing
			Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
		End Try

		Return DT

	End Function

	'Legge il db utenti corrispondente al db server in base alla partita iva
	Public Function Leggi_DB_Utenti_Da_Server(ByVal Server As String,
											  ByVal DB As String,
											  ByRef objParametriSuperServer As AgronicaCoreDataProvider.AgronicaCoreParametri
											  ) As DataTable

		Dim NomeRoutine As String = "AgronicaCoreVarieDAL.Connessioni.Leggi()"

		'====================================================================================
		'Parametri opzionali :
		'
		'   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
		'   DirectoryLOG = ""           =>  viene usato il valore di default
		'   FileLOG = ""                =>  viene usato il valore di default
		'====================================================================================

		Dim MessaggioErrore As String = ""
		Dim StrSQL As New System.Text.StringBuilder
		Dim DT As DataTable

		Try

			If Server = "" Then
				Throw New Exception("E' necessario specificare il Server")
			End If

			If DB = "" Then
				Throw New Exception("E' necessario specificare il DB")
			End If

			StrSQL.Length = 0

			StrSQL.Append(" SELECT * ")
			StrSQL.Append(" FROM  Connessioni ")
			StrSQL.Append(" WHERE 1=1 ")

			StrSQL.Append(" AND TipoDB =" & Agro_SQL_SaveNum(CInt(TipiEnumerativi.enum_Tipo_DB.GIAS_UTENTI)) & " ")

			StrSQL.Append(" AND Server ='" & Agro_SQL_SaveText(Server) & "' ")

			StrSQL.Append(" AND PivaSuperUser = ( ")

			StrSQL.Append("     SELECT PivaSuperUser ")
			StrSQL.Append("     FROM  Connessioni ")
			StrSQL.Append("     WHERE 1=1 ")
			StrSQL.Append("     AND TipoDB =" & Agro_SQL_SaveNum(CInt(TipiEnumerativi.enum_Tipo_DB.GIAS_SERVER)) & " ")
			StrSQL.Append("     AND Server ='" & Agro_SQL_SaveText(Server) & "' ")
			StrSQL.Append("     AND DB ='" & Agro_SQL_SaveText(DB) & "' ")

			StrSQL.Append(" ) ")

			'--------------------------------------------------------------------------
			DT = EseguiQuery_Lettura(objParametriSuperServer, StrSQL.ToString, NomeRoutine)
			'--------------------------------------------------------------------------

			If (DT.Rows.Count > 1) Then
				Throw New Exception("Non possono esistere piu' DB utenti per lo stesso server/partita iva")
			End If



		Catch ex As Exception
			MessaggioErrore = ex.Message
			Scrivi_LOG(objParametriSuperServer, NomeRoutine, MessaggioErrore)
			DT = Nothing
			Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
		End Try

		Return DT

	End Function

	''' <summary>
	''' Verifica che sia raggiungibile e attivo
	''' </summary>
	''' <param name="objP_Super_Server"></param>
	''' <returns></returns>
	Public Function IsAlive(ByRef objP_Super_Server As AgronicaCoreParametri) As Boolean
		Dim nomeRoutine As String = "AgronicaCoreDataProvider.Connessioni.IsAlive()"

		Dim messaggioErrore As String = ""
		Dim strSql As New StringBuilder
		Dim dt As DataTable
		Dim r As Boolean = False

		Try
			strSql.Length = 0
			strSql.AppendLine(" SELECT TOP(1) * FROM Connessioni ")

			'--------------------------------------------------------------------------
			dt = EseguiQuery_Lettura(objP_Super_Server, strSql.ToString, nomeRoutine)
			'--------------------------------------------------------------------------

			If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
				r = True
			End If

		Catch ex As Exception
			messaggioErrore = ex.Message
			Scrivi_LOG(objP_Super_Server, nomeRoutine, messaggioErrore)
			r = False
			'Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
		End Try

		Return r

	End Function


	'#######################################################################
	'######           SCRITTURA                                     ########
	'#######################################################################


	Public Function Scrivi(ByVal ID_DB As Integer,
						   ByVal TipoDB As TipiEnumerativi.enum_Tipo_DB,
						   ByVal Server As String,
						   ByVal DB As String,
						   ByVal Provider As String,
						   ByVal UserId As String,
						   ByVal Password As String,
						   ByVal PivaSuperUser As String,
						   ByVal Note As String,
						   ByVal Progressivo As Integer,
						   ByVal Descrizione As String,
						   ByVal xFiltroAggiuntivo As String,
						   ByVal xOrderBy As String,
						   ByRef objParametriSuperServer As AgronicaCoreDataProvider.AgronicaCoreParametri
						   ) As Boolean

		Dim NomeRoutine As String = "AgronicaCoreVarieDAL.Connessioni.Scrivi()"



		Dim MessaggioErrore As String = ""
		Dim StrSQL As New System.Text.StringBuilder
		Dim xRisp As Boolean

		Try

			If ID_DB <> 0 Then
				Throw New Exception("E' necessario specificare l'ID DB")
			End If

			If TipoDB = TipiEnumerativi.enum_Tipo_DB.TUTTI Then
				Throw New Exception("E' necessario specificare il tipo db")
			End If

			If Server = "" Then
				Throw New Exception("E' necessario specificare il Server")
			End If

			If DB = "" Then
				Throw New Exception("E' necessario specificare il DB")
			End If

			If Provider = "" Then
				Throw New Exception("E' necessario specificare il Provider")
			End If

			If UserId = "" Then
				Throw New Exception("E' necessario specificare lo UserId")
			End If
			If Password = "" Then
				Throw New Exception("E' necessario specificare la Password")
			End If

			If PivaSuperUser = "" Then

			End If

			If Note = "" Then

			End If

			If Progressivo <> 0 Then
				Throw New Exception("E' necessario specificare il Progressivo")
			End If

			If Descrizione <> "" Then
				Throw New Exception("E' necessario specificare una descrizione")
			End If

			StrSQL.Length = 0

			StrSQL.Append(" INSERT INTO [Connessioni] ")
			StrSQL.Append("     ([ID_DB]")
			StrSQL.Append("     ,[TipoDB]")
			StrSQL.Append("     ,[Provider]")
			StrSQL.Append("     ,[Server]")
			StrSQL.Append("     ,[DB]")
			StrSQL.Append("     ,[UserId]")
			StrSQL.Append("     ,[Password]")
			StrSQL.Append("     ,[PivaSuperUser]")
			StrSQL.Append("     ,[Note]")
			StrSQL.Append("     ,[Progressivo]")
			StrSQL.Append("     ,[Descrizione])")
			StrSQL.Append(" VALUES ")
			StrSQL.Append("     " & Agro_SQL_SaveNum(ID_DB) & " ")
			StrSQL.Append("     ," & Agro_SQL_SaveNum(TipoDB) & " ")
			StrSQL.Append("     ,'" & Agro_SQL_SaveText(Provider) & "' ")
			StrSQL.Append("     ,'" & Agro_SQL_SaveText(Server) & "' ")
			StrSQL.Append("     ,'" & Agro_SQL_SaveText(DB) & "' ")
			StrSQL.Append("     ,'" & Agro_SQL_SaveText(UserId) & "' ")
			StrSQL.Append("     ,'" & Agro_SQL_SaveText(Password) & "' ")
			StrSQL.Append("     ,'" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
			StrSQL.Append("     ,'" & Agro_SQL_SaveText(Note) & "' ")
			StrSQL.Append("     ," & Agro_SQL_SaveNum(Progressivo) & " ")
			StrSQL.Append("     ,'" & Agro_SQL_SaveText(Descrizione) & "' ")
			StrSQL.Append(" )")

			'--------------------------------------------------------------------------
			xRisp = EseguiQuery_Scrittura(objParametriSuperServer, StrSQL.ToString, NomeRoutine)
			'--------------------------------------------------------------------------

		Catch ex As Exception
			MessaggioErrore = ex.Message
			Scrivi_LOG(objParametriSuperServer, NomeRoutine, MessaggioErrore)
			xRisp = False
			Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
		End Try

		Return xRisp


	End Function


	Public Function Modifica(ByVal ID_DB As Integer,
							 ByVal TipoDB As TipiEnumerativi.enum_Tipo_DB,
							 ByVal Server As String,
							 ByVal DB As String,
							 ByVal Provider As String,
							 ByVal UserId As String,
							 ByVal Password As String,
							 ByVal PivaSuperUser As String,
							 ByVal Note As String,
							 ByVal Progressivo As Integer,
							 ByVal Descrizione As String,
							 ByVal xFiltroAggiuntivo As String,
							 ByVal xOrderBy As String,
							 ByRef objParametriSuperServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
							 Optional ByVal Encrypted As String = ""
							 ) As Boolean

		Dim NomeRoutine As String = "AgronicaCoreVarieDAL.Connessioni.Modifica()"



		Dim MessaggioErrore As String = ""
		Dim StrSQL As New System.Text.StringBuilder
		Dim xRisp As Boolean

		Try

			'per cancellazioni accidentali imposto la chiave obbligatoria
			If ID_DB = 0 Then
				Throw New Exception("E' necessario specificare l'ID_DB")
			End If


			StrSQL.Length = 0

			StrSQL.Append(" UPDATE [Connessioni] SET ")
			StrSQL.Append(" Provider = Provider ")


			If TipoDB <> TipiEnumerativi.enum_Tipo_DB.TUTTI Then
				StrSQL.Append("     ,[TipoDB] = " & Agro_SQL_SaveNum(TipoDB) & " ")
			End If

			If Server <> "" Then
				StrSQL.Append("     ,[Server] = '" & Agro_SQL_SaveText(Server) & "' ")
			End If

			If DB <> "" Then
				StrSQL.Append("     ,[DB] = '" & Agro_SQL_SaveText(DB) & "' ")
			End If

			If Provider <> "" Then
				StrSQL.Append("     ,[Provider] = '" & Agro_SQL_SaveText(Provider) & "' ")
			End If

			If UserId <> "" Then
				StrSQL.Append("     ,[UserId] = '" & Agro_SQL_SaveText(UserId) & "' ")
			End If

			If Password <> "" Then
				StrSQL.Append("     ,[Password] = '" & Agro_SQL_SaveText(Password) & "' ")
			End If

			If PivaSuperUser <> "" Then
				StrSQL.Append("     ,[PivaSuperUser] = '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
			End If

			If Note <> "" Then
				StrSQL.Append("     ,[Note] = '" & Agro_SQL_SaveText(Note) & "' ")
			End If

			If Progressivo <> 0 Then
				StrSQL.Append("     ,[Progressivo] = " & Agro_SQL_SaveNum(Progressivo) & " ")
			End If

			If Descrizione <> "" Then
				StrSQL.Append("     ,[Descrizione] = '" & Agro_SQL_SaveText(Descrizione) & "' ")
			End If

			If Encrypted <> "" Then
				StrSQL.Append("     ,[Flag_Encrypted] = " & Agro_SQL_SaveNum(If(Encrypted = "1", 1, 0)) & " ")
			End If


			StrSQL.Append(" WHERE ")

			StrSQL.Append("      [ID_DB] = " & Agro_SQL_SaveNum(ID_DB) & " ")



			'--------------------------------------------------------------------------
			xRisp = EseguiQuery_Scrittura(objParametriSuperServer, StrSQL.ToString, NomeRoutine)
			'--------------------------------------------------------------------------

		Catch ex As Exception
			MessaggioErrore = ex.Message
			Scrivi_LOG(objParametriSuperServer, NomeRoutine, MessaggioErrore)
			xRisp = False
			Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
		End Try

		Return xRisp

	End Function


End Class

