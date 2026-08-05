Imports System.Text
Imports System.Data.Entity
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreEntityFramework

Public Class Ricette_Zoo_Dettaglio_Tecnico_R
	Inherits AgronicaCoreDataProvider.DataProvider

	Public Function Leggi(ByVal Id_Ricetta As Integer,
						  ByVal Id_Agenda As Integer,
						  ByVal Id_Mov As Integer,
						  ByVal Id_Mov_Det As Integer,
						  ByVal Id_Reg_Dettaglio As Integer,
						  ByVal Piva As String,
						  ByVal Sa_Cod As Integer,
						  ByVal Dett_Cod As Integer,
						  ByVal Lotto As String,
						  ByVal Extra_Str As String,
						  ByVal Extra_Int As Integer,
						  ByVal Extra_Date As DateTime,
						  ByVal xSelezioneVariabile As enumSelezioneVariabile,
						  ByVal xFiltroAggiuntivo As String,
						  ByVal xOrderBy As String,
						  ByRef objParametri As AgronicaCoreParametri) As DataTable
		Const nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Zoo_Dettaglio_Tecnico_R.Leggi()"

		Dim messaggioErrore As String = ""
		Dim strSQL As New StringBuilder
		Dim dt As DataTable

		Try
			strSQL.Length = 0

			Select Case xSelezioneVariabile
				Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

					strSQL.AppendLine("SELECT * ").
						AppendLine("FROM Ricette_Zoo_Dettaglio_Tecnico ").
						AppendLine("WHERE Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ").
						AppendLine("    AND Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

					If Id_Agenda <> 0 Then
						strSQL.AppendLine("    AND Id_Ricetta = " & Agro_SQL_SaveNum(Id_Ricetta) & " ")
					End If

					If Id_Agenda <> 0 Then
						strSQL.AppendLine("    AND Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")
					End If

					If Id_Mov <> 0 Then
						strSQL.AppendLine("    AND Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & " ")
					End If

					If Id_Mov_Det <> 0 Then
						strSQL.AppendLine("    AND Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & " ")
					End If

					If Id_Reg_Dettaglio <> 0 Then
						strSQL.AppendLine("    AND Id_Reg_Dettaglio = " & Agro_SQL_SaveNum(Id_Reg_Dettaglio) & " ")
					End If

					If Piva <> "" Then
						strSQL.AppendLine("    AND Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
					End If

					If Sa_Cod <> 0 Then
						strSQL.AppendLine("    AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
					End If

					If Dett_Cod <> 0 Then
						strSQL.AppendLine("    AND Dett_Cod = " & Agro_SQL_SaveNum(Dett_Cod) & " ")
					End If

					If Lotto <> "" Then
						strSQL.AppendLine("    AND Lotto = '" & Agro_SQL_SaveText(Lotto) & "' ")
					End If

					If Extra_Str <> "" Then
						strSQL.AppendLine("    AND Extra_Str = '" & Agro_SQL_SaveText(Extra_Str) & "' ")
					End If

					If Extra_Int <> 0 Then
						strSQL.AppendLine("    AND Extra_Int = " & Agro_SQL_SaveNum(Extra_Int) & " ")
					End If

					If Extra_Date <> AGRODATAINIZIO Then
						strSQL.AppendLine("    AND Extra_Date = " & Agro_SQL_SaveDate(Extra_Date) & " ")
					End If

					If xFiltroAggiuntivo <> "" Then
						strSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
					End If

					'--------------------------------------------------------------------------
					Select Case objParametri.FlagVisibilita
						Case enumVisibilita.Visibilita_SoloNonCancellati
							strSQL.AppendLine("    AND Inviato >= 0 ")
						Case enumVisibilita.Visibilita_SoloCancellati
							strSQL.AppendLine("    AND Inviato = -1 ")
						Case enumVisibilita.Visibilita_Tutti
							'...................................
						Case Else
							Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
					End Select
					'--------------------------------------------------------------------------

					If xOrderBy <> "" Then
						strSQL.AppendLine("ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
					Else
						strSQL.AppendLine("ORDER BY Ricette_Zoo_Dettaglio_Tecnico.Piva, Ricette_Zoo_Dettaglio_Tecnico.Sa_Cod, Ricette_Zoo_Dettaglio_Tecnico.Id_Ricetta, Ricette_Zoo_Dettaglio_Tecnico.Id_Agenda ASC ")
					End If

				Case enumSelezioneVariabile.Selezione_TabellaCompleta

					strSQL.AppendLine("SELECT * ").
						AppendLine("FROM Ricette_Zoo_Dettaglio_Tecnico ").
						AppendLine("WHERE Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ").
						AppendLine("    AND Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

					If Id_Agenda <> 0 Then
						strSQL.AppendLine("    AND Id_Ricetta = " & Agro_SQL_SaveNum(Id_Ricetta) & " ")
					End If

					If Id_Agenda <> 0 Then
						strSQL.AppendLine("    AND Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")
					End If

					If Id_Mov <> 0 Then
						strSQL.AppendLine("    AND Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & " ")
					End If

					If Id_Mov_Det <> 0 Then
						strSQL.AppendLine("    AND Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & " ")
					End If

					If Id_Reg_Dettaglio <> 0 Then
						strSQL.AppendLine("    AND Id_Reg_Dettaglio = " & Agro_SQL_SaveNum(Id_Reg_Dettaglio) & " ")
					End If

					If Piva <> "" Then
						strSQL.AppendLine("    AND Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
					End If

					If Sa_Cod <> 0 Then
						strSQL.AppendLine("    AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
					End If

					If Dett_Cod <> 0 Then
						strSQL.AppendLine("    AND Dett_Cod = " & Agro_SQL_SaveNum(Dett_Cod) & " ")
					End If

					If Lotto <> "" Then
						strSQL.AppendLine("    AND Lotto = '" & Agro_SQL_SaveText(Lotto) & "' ")
					End If

					If Extra_Str <> "" Then
						strSQL.AppendLine("    AND Extra_Str = '" & Agro_SQL_SaveText(Extra_Str) & "' ")
					End If

					If Extra_Int <> 0 Then
						strSQL.AppendLine("    AND Extra_Int = " & Agro_SQL_SaveNum(Extra_Int) & " ")
					End If

					If Extra_Date <> AGRODATAINIZIO Then
						strSQL.AppendLine("    AND Extra_Date = " & Agro_SQL_SaveDate(Extra_Date) & " ")
					End If

					If xFiltroAggiuntivo <> "" Then
						strSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
					End If

					'--------------------------------------------------------------------------
					Select Case objParametri.FlagVisibilita
						Case enumVisibilita.Visibilita_SoloNonCancellati
							strSQL.AppendLine("    AND Inviato >= 0 ")
						Case enumVisibilita.Visibilita_SoloCancellati
							strSQL.AppendLine("    AND Inviato = -1 ")
						Case enumVisibilita.Visibilita_Tutti
							'...................................
						Case Else
							Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
					End Select
					'--------------------------------------------------------------------------

					If xOrderBy <> "" Then
						strSQL.AppendLine("ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
					Else
						strSQL.AppendLine("ORDER BY Ricette_Zoo_Dettaglio_Tecnico.Piva, Ricette_Zoo_Dettaglio_Tecnico.Sa_Cod, Ricette_Zoo_Dettaglio_Tecnico.Id_Ricetta, Ricette_Zoo_Dettaglio_Tecnico.Id_Agenda ASC ")
					End If

				Case enumSelezioneVariabile.Selezione_JoinDescrizioni

				Case enumSelezioneVariabile.Selezione_JoinCompleta

			End Select

			'--------------------------------------------------------------------------
			dt = EseguiQuery_Lettura(objParametri, strSQL.ToString, nomeRoutine)
			'--------------------------------------------------------------------------

		Catch ex As Exception
			messaggioErrore = ex.Message
			Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
			dt = Nothing
			Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
		End Try

		Return dt

	End Function

End Class

Public Class Ricette_Zoo_Dettaglio_Tecnico_W
	Inherits AgronicaCoreDataProvider.DataProvider

#Region "Scrittura"

	Public Function Scrivi(ByVal Id_Ricetta As Integer,
						   ByVal Id_Agenda As Integer,
						   ByVal Id_Mov As Integer,
						   ByVal Id_Mov_Det As Integer,
						   ByVal Id_Reg_Dettaglio As Integer,
						   ByVal Piva As String,
						   ByVal Sa_Cod As Integer,
						   ByVal Validita_Inizio As Date,
						   ByVal Validita_Fine As Date,
						   ByRef objParametri As AgronicaCoreParametri,
						   Optional ByVal Dett_Cod As Integer = 0,
						   Optional ByVal Lotto As String = "",
						   Optional ByVal Extra_Str As String = "",
						   Optional ByVal Extra_Int As Integer = 0,
						   Optional ByVal Extra_Date As DateTime = AGRODATAINIZIO) As Boolean
		Const nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Zoo_Dettaglio_Tecnico_W.Scrivi()"

		Dim messaggioErrore As String = ""
		Dim strSql As New StringBuilder
		Dim xRisp As Boolean = False

		Try
			If Id_Ricetta = 0 Then
				Throw New Exception("Id_Ricetta non e' stata valorizzato.")
			End If

			If Id_Agenda = 0 Then
				Throw New Exception("Id_Agenda non e' stata valorizzato.")
			End If

			If Id_Mov = 0 Then
				Throw New Exception("Id_Mov non e' stata valorizzato.")
			End If

			If Id_Mov_Det = 0 Then
				Throw New Exception("Id_Mov_Det non e' stata valorizzato.")
			End If

			If Id_Reg_Dettaglio = 0 Then
				Throw New Exception("Id_Reg_Dettaglio non e' stata valorizzato.")
			End If

			If Piva = "" Then
				Throw New Exception("Piva non e' stata valorizzata.")
			End If

			If Sa_Cod = 0 Then
				Throw New Exception("Sa_Cod non e' stata valorizzato.")
			End If

			strSql.Length = 0

			strSql.AppendLine("INSERT INTO Ricette_Zoo_Dettaglio_Tecnico ").
				AppendLine("    (Id_Ricetta,Id_Agenda, Id_Mov, Id_Mov_Det, Id_Reg_Dettaglio, Piva, SaCod, ")

			strSql.AppendLine("     Dett_Cod, Lotto, Extra_Str, Extra_Int, Extra_Date, ")

			strSql.AppendLine("     Inviato, DataInvio, StrutturaCodice, StrutturaDenominazione, ProtocolloCodice, ").
				AppendLine("     Data_Creazione, Data_Modifica, ").
				AppendLine("     Username_Creazione, Username_Modifica, ").
				AppendLine("     Validita_Inizio, Validita_Fine) ")

			strSql.AppendLine("VALUES (").
				AppendLine("    " & Agro_SQL_SaveNum(Id_Ricetta) & ", ").
				AppendLine("    " & Agro_SQL_SaveNum(Id_Agenda) & ", ").
				AppendLine("    " & Agro_SQL_SaveNum(Id_Mov) & ", ").
				AppendLine("    " & Agro_SQL_SaveNum(Id_Mov_Det) & ", ").
				AppendLine("    " & Agro_SQL_SaveNum(Id_Reg_Dettaglio) & ", ").
				AppendLine("    '" & Agro_SQL_SaveText(Piva) & "', ").
				AppendLine("    " & Agro_SQL_SaveNum(Sa_Cod) & ", ")

			strSql.AppendLine("    " & Agro_SQL_SaveNum(Dett_Cod) & ", ").
				AppendLine("    '" & Agro_SQL_SaveText(Lotto) & "', ").
				AppendLine("    '" & Agro_SQL_SaveText(Extra_Str) & "', ").
				AppendLine("    " & Agro_SQL_SaveNum(Extra_Int) & ", ").
				AppendLine("    " & Agro_SQL_SaveDateTime(Extra_Date) & ", ")

			strSql.AppendLine("    0, NULL, ").
				AppendLine("    " & Agro_SQL_SaveDateTime(DateTime.Now) & ", ").
				AppendLine("    " & Agro_SQL_SaveDateTime(DateTime.Now) & ", ").
				AppendLine("    '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "', ").
				AppendLine("    '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "', ").
				AppendLine("    " & Agro_SQL_SaveDate(Validita_Inizio) & ", ").
				AppendLine("    " & Agro_SQL_SaveDate(Validita_Fine) & " ").
				AppendLine(") ")

			'--------------------------------------------------------------------------
			xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
			'--------------------------------------------------------------------------

		Catch ex As Exception
			messaggioErrore = ex.Message
			Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
			xRisp = False
			Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
		End Try

		Return xRisp

	End Function

#End Region

#Region "Modifica"

	Public Function Modifica(ByVal Id_Ricetta As Integer,
							 ByVal Id_Agenda As Integer,
							 ByVal Id_Mov As Integer,
							 ByVal Id_Mov_Det As Integer,
							 ByVal Id_Reg_Dettaglio As Integer,
							 ByVal Piva As String,
							 ByVal Sa_Cod As Integer,
							 ByVal xFiltroAggiuntivo As String,
							 ByRef objParametri As AgronicaCoreParametri,
							 Optional ByVal Dett_Cod As Integer? = Nothing,
							 Optional ByVal Lotto As String = Nothing,
							 Optional ByVal Extra_Str As String = Nothing,
							 Optional ByVal Extra_Int As Integer? = Nothing,
							 Optional ByVal Extra_Date As DateTime? = Nothing,
							 Optional ByVal Validita_Inizio As Date? = Nothing,
							 Optional ByVal Validita_Fine As Date? = Nothing) As Boolean
		Const nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Zoo_Dettaglio_Tecnico_W.Modifica()"

		Dim messaggioErrore As String = ""
		Dim strSql As New StringBuilder
		Dim xRisp As Boolean = False

		Try
			If Id_Agenda = 0 Then
				Throw New Exception("Parametro non corretto nella query (Id_Agenda obbligatorio)")
			End If

			If Piva = "" Then
				Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
			End If

			strSql.Length = 0

			strSql.AppendLine("UPDATE Ricette_Zoo_Dettaglio_Tecnico ").
				AppendLine("SET Data_Modifica = " & Agro_SQL_SaveDateTime(Date.Now) & ", ").
				AppendLine("    Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "', ")

			If Not IsNothing(Dett_Cod) Then
				strSql.AppendLine("    Dett_Cod = " & Agro_SQL_SaveNum(Dett_Cod) & ", ")
			End If

			If Not IsNothing(Lotto) Then
				strSql.AppendLine("    Lotto = '" & Agro_SQL_SaveText(Lotto) & "', ")
			End If

			If Not IsNothing(Extra_Str) Then
				strSql.AppendLine("    Extra_Str = '" & Agro_SQL_SaveText(Extra_Str) & "', ")
			End If

			If Not IsNothing(Extra_Int) Then
				strSql.AppendLine("    Extra_Int = " & Agro_SQL_SaveNum(Extra_Int) & ", ")
			End If

			If Not IsNothing(Extra_Date) Then
				strSql.AppendLine("    Extra_Date = " & Agro_SQL_SaveDateTime(Extra_Date) & ", ")
			End If

			If Not IsNothing(Validita_Inizio) Then
				strSql.AppendLine("    Validita_Inizio = " & Agro_SQL_SaveDate(Validita_Inizio) & ", ")
			End If

			If Not IsNothing(Validita_Fine) Then
				strSql.AppendLine("    Validita_Fine = " & Agro_SQL_SaveDate(Validita_Fine) & ", ")
			End If

			strSql.AppendLine("WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ").
				AppendLine("    AND Id_Ricetta = " & Agro_SQL_SaveNum(Id_Ricetta) & " ")

			If Sa_Cod <> 0 Then
				strSql.AppendLine("    AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
			End If

			If Id_Agenda <> 0 Then
				strSql.AppendLine("    AND Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")
			End If

			If Id_Mov <> 0 Then
				strSql.AppendLine("    AND Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & " ")
			End If

			If Id_Mov_Det <> 0 Then
				strSql.AppendLine("    AND Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & " ")
			End If

			If Id_Reg_Dettaglio <> 0 Then
				strSql.AppendLine("    AND Id_Reg_Dettaglio = " & Agro_SQL_SaveNum(Id_Reg_Dettaglio) & " ")
			End If

			'----------------------------------------------------------------------
			If xFiltroAggiuntivo <> "" Then
				strSql.AppendLine("    AND " & xFiltroAggiuntivo)
			End If

			'--------------------------------------------------------------------------
			xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
			'--------------------------------------------------------------------------

		Catch ex As Exception
			messaggioErrore = ex.Message
			Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
			xRisp = False
			Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
		End Try

		Return xRisp

	End Function

#End Region

#Region "Cancellazione"

	Public Function Cancella(ByVal Id_Ricetta As Integer,
							 ByVal Id_Agenda As Integer,
							 ByVal Id_Mov As Integer,
							 ByVal Id_Mov_Det As Integer,
							 ByVal Id_Reg_Dettaglio As Integer,
							 ByVal Piva As String,
							 ByVal Sa_Cod As Integer,
							 ByVal xFiltroAggiuntivo As String,
							 ByRef objParametri As AgronicaCoreParametri) As Boolean
		Const nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Zoo_Dettaglio_Tecnico_W.Cancella()"

		'====================================================================================
		'Parametri opzionali :
		'   Piva = ""
		'   Sa_Cod = 0
		'   Id_Agenda = 0
		'   Id_Mov = 0
		'   Id_Mov_Det = 0
		'   Id_Reg_Dettaglio = 0
		'====================================================================================

		Dim messaggioErrore As String = ""
		Dim strSql As New StringBuilder
		Dim xRisp As Boolean = False

		Try

			If Id_Agenda = 0 Then
				Throw New Exception("Parametro non corretto nella query (Id_Agenda obbligatorio)")
			End If

			'---------------------------------------------
			strSql.Length = 0

			If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
				strSql.AppendLine("UPDATE Ricette_Zoo_Dettaglio_Tecnico ").
					AppendLine("SET ").
					AppendLine("    Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "', ").
					AppendLine("    Inviato = -1 ").
					AppendLine("WHERE Inviato >= 0 ")
			Else
				strSql.AppendLine("DELETE ").
					AppendLine("FROM Ricette_Zoo_Dettaglio_Tecnico ").
					AppendLine("WHERE  1=1 ")
			End If

			strSql.AppendLine("    AND Id_Ricetta = " & Agro_SQL_SaveNum(Id_Ricetta) & " ")

			If Piva <> "" Then
				strSql.AppendLine("    AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
			End If

			If Sa_Cod <> 0 Then
				strSql.AppendLine("    AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
			End If

			If Id_Agenda <> 0 Then
				strSql.AppendLine("    AND Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")
			End If

			If Id_Mov <> 0 Then
				strSql.AppendLine("    AND Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & " ")
			End If

			If Id_Mov_Det <> 0 Then
				strSql.AppendLine("    AND Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & " ")
			End If

			If Id_Reg_Dettaglio <> 0 Then
				strSql.AppendLine("    AND Id_Reg_Dettaglio = " & Agro_SQL_SaveNum(Id_Reg_Dettaglio) & " ")
			End If

			If xFiltroAggiuntivo <> "" Then
				strSql.AppendLine("    AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) & " ")
			End If

			'--------------------------------------------------------------------------
			xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
			'--------------------------------------------------------------------------

		Catch ex As Exception
			messaggioErrore = ex.Message
			Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
			xRisp = False
			Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
		End Try

		Return xRisp

	End Function

#End Region

#Region "Entity Framework"

	Public Sub Scrivi(ByRef Ricette_Zoo_Dettaglio_Tecnico As AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Dettaglio_Tecnico,
					  ByRef GiasContext As Gias_DeveloperServer_Entities,
					  ByRef objParametriServer As AgronicaCoreParametri)
		Const nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Zoo_Dettaglio_Tecnico_W.Scrivi()"
		Dim messaggioErrore As String = ""

		Try
			Valorizza(Ricette_Zoo_Dettaglio_Tecnico, objParametriServer)

			Ricette_Zoo_Dettaglio_Tecnico.Data_Creazione = DateTime.Now
			Ricette_Zoo_Dettaglio_Tecnico.Data_Modifica = DateTime.Now
			Ricette_Zoo_Dettaglio_Tecnico.Username_Creazione = objParametriServer.UsernameOperazione
			Ricette_Zoo_Dettaglio_Tecnico.Username_Modifica = objParametriServer.UsernameOperazione

			GiasContext.Ricette_Zoo_Dettaglio_Tecnico.Add(Ricette_Zoo_Dettaglio_Tecnico)
		Catch ex As Exception
			messaggioErrore = ex.Message
			Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
			Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
		End Try
	End Sub

	Public Sub Modifica(ByRef Ricette_Zoo_Dettaglio_Tecnico As AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Dettaglio_Tecnico,
						ByRef GiasContext As Gias_DeveloperServer_Entities,
						ByRef objParametriServer As AgronicaCoreParametri)

		Const nomeRoutine = "AgronicaCoreContabDAL.Ricette_Zoo_Dettaglio_Tecnico_W.Modifica()"
		Dim messaggioErrore As String = ""

		Try
			Valorizza(Ricette_Zoo_Dettaglio_Tecnico, objParametriServer)

			Ricette_Zoo_Dettaglio_Tecnico.Data_Modifica = DateTime.Now
			Ricette_Zoo_Dettaglio_Tecnico.Username_Modifica = objParametriServer.UsernameOperazione

			GiasContext.Entry(Ricette_Zoo_Dettaglio_Tecnico).State = EntityState.Modified
		Catch ex As Exception
			messaggioErrore = ex.Message
			Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
			Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
		End Try
	End Sub

	Public Sub Valorizza(ByRef Ricette_Zoo_Dettaglio_Tecnico As AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Dettaglio_Tecnico,
						 ByRef objParametriServer As AgronicaCoreParametri)
		Const nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Zoo_Dettaglio_Tecnico_W.Valorizza()"
		Dim messaggioErrore As String = ""

		Try
			If Ricette_Zoo_Dettaglio_Tecnico.Dett_Cod Is Nothing Then
				Ricette_Zoo_Dettaglio_Tecnico.Dett_Cod = 0
			End If

			If Ricette_Zoo_Dettaglio_Tecnico.Lotto Is Nothing Then
				Ricette_Zoo_Dettaglio_Tecnico.Lotto = ""
			End If

			If Ricette_Zoo_Dettaglio_Tecnico.Extra_Str Is Nothing Then
				Ricette_Zoo_Dettaglio_Tecnico.Extra_Str = ""
			End If

			If Ricette_Zoo_Dettaglio_Tecnico.Extra_Int Is Nothing Then
				Ricette_Zoo_Dettaglio_Tecnico.Extra_Int = 0
			End If

			If Ricette_Zoo_Dettaglio_Tecnico.Extra_Date < AGRODATAINIZIO Then
				Ricette_Zoo_Dettaglio_Tecnico.Extra_Date = AGRODATAINIZIO
			End If

			If Ricette_Zoo_Dettaglio_Tecnico.Inviato Is Nothing Then
				Ricette_Zoo_Dettaglio_Tecnico.Inviato = 0
			End If

			If Ricette_Zoo_Dettaglio_Tecnico.Data_Creazione Is Nothing Then
				Ricette_Zoo_Dettaglio_Tecnico.Data_Creazione = DateTime.Now
			End If

			If Ricette_Zoo_Dettaglio_Tecnico.Data_Modifica Is Nothing Then
				Ricette_Zoo_Dettaglio_Tecnico.Data_Modifica = DateTime.Now
			End If

			If Ricette_Zoo_Dettaglio_Tecnico.Username_Creazione Is Nothing Then
				Ricette_Zoo_Dettaglio_Tecnico.Username_Creazione = objParametriServer.UsernameOperazione
			End If

			If Ricette_Zoo_Dettaglio_Tecnico.Username_Modifica Is Nothing Then
				Ricette_Zoo_Dettaglio_Tecnico.Username_Modifica = objParametriServer.UsernameOperazione
			End If

			If Ricette_Zoo_Dettaglio_Tecnico.Validita_Inizio Is Nothing OrElse Ricette_Zoo_Dettaglio_Tecnico.Validita_Inizio < AGRODATAINIZIO Then
				Ricette_Zoo_Dettaglio_Tecnico.Validita_Inizio = AGRODATAINIZIO
			End If

			If Ricette_Zoo_Dettaglio_Tecnico.Validita_Fine Is Nothing OrElse Ricette_Zoo_Dettaglio_Tecnico.Validita_Fine < AGRODATAFINE Then
				Ricette_Zoo_Dettaglio_Tecnico.Validita_Fine = AGRODATAFINE
			End If
		Catch ex As Exception
			messaggioErrore = ex.Message
			Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
			Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
		End Try
	End Sub

	Public Sub Elimina(ByRef Ricette_Zoo_Dettaglio_Tecnico As AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Dettaglio_Tecnico,
					   ByRef GiasContext As Gias_DeveloperServer_Entities,
					   ByRef objParametriServer As AgronicaCoreParametri)
		Const nomeRoutine = "AgronicaCoreContabDAL.Ricette_Zoo_Dettaglio_Tecnico_W.Elimina()"
		Dim messaggioErrore As String = ""

		Try
			GiasContext.Ricette_Zoo_Dettaglio_Tecnico.Attach(Ricette_Zoo_Dettaglio_Tecnico)
			GiasContext.Ricette_Zoo_Dettaglio_Tecnico.Remove(Ricette_Zoo_Dettaglio_Tecnico)

		Catch ex As Exception
			messaggioErrore = ex.Message
			Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
			Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
		End Try
	End Sub

#End Region

End Class
