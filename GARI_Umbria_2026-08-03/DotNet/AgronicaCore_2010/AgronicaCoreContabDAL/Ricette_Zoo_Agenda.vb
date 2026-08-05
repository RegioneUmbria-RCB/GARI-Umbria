Imports System.Text
Imports System.Data.Entity
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreEntityFramework
Imports InData.Anagrafica
Imports System.Net.NetworkInformation
Public Class Ricette_Zoo_Agenda_R
	Inherits AgronicaCoreDataProvider.DataProvider

	Public Function Leggi(ByVal Id_Ricetta As Integer,
						  ByVal Id_Agenda As Integer,
						  ByVal Piva As String,
						  ByVal Sa_Cod As Integer,
						  ByVal Lav_Cod As Integer,
						  ByVal RicettaNumero As String,
						  ByVal Data_Tratt_Inizio As DateTime,
						  ByVal Data_Tratt_Fine As DateTime,
						  ByVal xSelezioneVariabile As enumSelezioneVariabile,
						  ByVal xFiltroAggiuntivo As String,
						  ByVal xOrderBy As String,
						  ByRef objParametri As AgronicaCoreParametri) As DataTable
		Const nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Zoo_Agenda_R.Leggi()"

		Dim messaggioErrore As String = ""
		Dim strSQL As New StringBuilder
		Dim dt As DataTable

		Try
			strSQL.Length = 0

			Select Case xSelezioneVariabile
				Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi
					strSQL.AppendLine("SELECT * ").
						AppendLine("FROM Ricette_Zoo_Agenda ").
						AppendLine("WHERE Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ").
						AppendLine("    AND Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

					If Id_Ricetta <> 0 Then
						strSQL.AppendLine("    AND IdRicetta = " & Agro_SQL_SaveNum(Id_Ricetta) & " ")
					End If

					If Id_Agenda <> 0 Then
						strSQL.AppendLine("    AND IdAgenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")
					End If

					If Piva <> "" Then
						strSQL.AppendLine("    AND Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
					End If

					If Sa_Cod <> 0 Then
						strSQL.AppendLine("    AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
					End If

					If Lav_Cod <> 0 Then
						strSQL.AppendLine("    AND Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & " ")
					End If

					If RicettaNumero <> "" Then
						strSQL.AppendLine("    AND Numero = '" & Agro_SQL_SaveText(RicettaNumero) & "' ")
					End If

					If Data_Tratt_Inizio <> AGRODATAINIZIO Then
						strSQL.AppendLine("    AND DataInizioTrattamento = " & Agro_SQL_SaveDate(Data_Tratt_Inizio) & " ")
					End If

					If Data_Tratt_Fine <> AGRODATAFINE Then
						strSQL.AppendLine("    AND DataFineTrattamento = " & Agro_SQL_SaveDate(Data_Tratt_Fine) & " ")
					End If

					If xFiltroAggiuntivo <> "" Then
						strSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
					End If

					'--------------------------------------------------------------------------
					Select Case objParametri.FlagVisibilita
						Case enumVisibilita.Visibilita_SoloNonCancellati
							strSQL.AppendLine("    AND Ricette_Zoo_Agenda.Inviato >= 0 ")
						Case enumVisibilita.Visibilita_SoloCancellati
							strSQL.AppendLine("    AND Ricette_Zoo_Agenda.Inviato = -1 ")
						Case enumVisibilita.Visibilita_Tutti
							'...................................
						Case Else
							Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
					End Select
					'--------------------------------------------------------------------------

					If xOrderBy <> "" Then
						strSQL.AppendLine("ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
					Else
						strSQL.AppendLine("ORDER BY Ricette_Zoo_Agenda.Piva, Ricette_Zoo_Agenda.Sa_Cod, Ricette_Zoo_Agenda.IdRicetta, Ricette_Zoo_Agenda.IdAgenda ASC ")
					End If

				Case enumSelezioneVariabile.Selezione_TabellaCompleta
					strSQL.AppendLine("SELECT * ").
						AppendLine("FROM Ricette_Zoo_Agenda ").
						AppendLine("WHERE Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ").
						AppendLine("    AND Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

					If Id_Ricetta <> 0 Then
						strSQL.AppendLine("    AND IdRicetta = " & Agro_SQL_SaveNum(Id_Ricetta) & " ")
					End If

					If Id_Agenda <> 0 Then
						strSQL.AppendLine("    AND IdAgenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")
					End If

					If Piva <> "" Then
						strSQL.AppendLine("    AND Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
					End If

					If Sa_Cod <> 0 Then
						strSQL.AppendLine("    AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
					End If

					If Lav_Cod <> 0 Then
						strSQL.AppendLine("    AND Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & " ")
					End If

					If RicettaNumero <> "" Then
						strSQL.AppendLine("    AND Numero = '" & Agro_SQL_SaveText(RicettaNumero) & "' ")
					End If

					If Data_Tratt_Inizio <> AGRODATAINIZIO Then
						strSQL.AppendLine("    AND DataInizioTrattamento = " & Agro_SQL_SaveDate(Data_Tratt_Inizio) & " ")
					End If

					If Data_Tratt_Fine <> AGRODATAFINE Then
						strSQL.AppendLine("    AND DataFineTrattamento = " & Agro_SQL_SaveDate(Data_Tratt_Fine) & " ")
					End If

					If xFiltroAggiuntivo <> "" Then
						strSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
					End If

					'--------------------------------------------------------------------------
					Select Case objParametri.FlagVisibilita
						Case enumVisibilita.Visibilita_SoloNonCancellati
							strSQL.AppendLine("    AND Ricette_Zoo_Agenda.Inviato >= 0 ")
						Case enumVisibilita.Visibilita_SoloCancellati
							strSQL.AppendLine("    AND Ricette_Zoo_Agenda.Inviato = -1 ")
						Case enumVisibilita.Visibilita_Tutti
							'...................................
						Case Else
							Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
					End Select
					'--------------------------------------------------------------------------

					If xOrderBy <> "" Then
						strSQL.AppendLine("ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
					Else
						strSQL.AppendLine("ORDER BY Ricette_Zoo_Agenda.Piva, Ricette_Zoo_Agenda.Sa_Cod, Ricette_Zoo_Agenda.IdRicetta, Ricette_Zoo_Agenda.IdAgenda ASC ")
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

Public Class Ricette_Zoo_Agenda_W
	Inherits AgronicaCoreDataProvider.DataProvider

#Region "Scrittura"

	Public Function Scrivi(ByVal Id_Ricetta As Integer,
						   ByVal Id_Agenda As Integer,
						   ByVal Piva As String,
						   ByVal Sa_Cod As Integer,
						   ByVal Validita_Inizio As Date,
						   ByVal Validita_Fine As Date,
						   ByRef objParametri As AgronicaCoreParametri,
						   Optional ByVal Lav_Cod As Integer = 0,
						   Optional ByVal Des_Lib As String = "",
						   Optional ByVal Descrizione As String = "",
						   Optional ByVal Ricetta_Numero As String = "",
						   Optional ByVal Data_Tratt_Inizio As DateTime = AGRODATAINIZIO,
						   Optional ByVal Data_Tratt_Fine As DateTime = AGRODATAFINE,
						   Optional ByVal DurattaTratt As Integer = 0,
						   Optional ByVal Note As String = "") As Boolean
		Const nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Zoo_Agenda_W.Scrivi()"

		Dim messaggioErrore As String = ""
		Dim strSql As New StringBuilder
		Dim xRisp As Boolean = False

		Try
			If Id_Ricetta = 0 Then
				Throw New Exception("IdRicetta non e' stata valorizzato.")
			End If

			If Id_Agenda = 0 Then
				Throw New Exception("IdAgenda non e' stata valorizzato.")
			End If

			If Piva = "" Then
				Throw New Exception("Piva non e' stata valorizzata.")
			End If

			If Sa_Cod = 0 Then
				Throw New Exception("Sa_Cod non e' stata valorizzato.")
			End If

			strSql.Length = 0

			strSql.AppendLine("INSERT INTO Ricette_Zoo_Agenda ").
				AppendLine("    (IdRicetta, IdAgenda, Piva, Sa_Cod, Lav_Cod, Des_Lib, ").
				AppendLine("     Descrizione, DataInizioTrattamento, DataFineTrattamento, DurataTrattamento, Numero, Note, ").
				AppendLine("     Blocco_Flag, Blocco_Data, Blocco_Username, ")

			strSql.AppendLine("     Inviato, DataInvio, ").
				AppendLine("     Data_Creazione, Data_Modifica, ").
				AppendLine("     Username_Creazione, Username_Modifica, ").
				AppendLine("     Validita_Inizio, Validita_Fine) ")

			strSql.AppendLine("VALUES (").
				AppendLine("    " & Agro_SQL_SaveNum(Id_Ricetta) & ", ").
				AppendLine("    " & Agro_SQL_SaveNum(Id_Agenda) & ", ").
				AppendLine("    '" & Agro_SQL_SaveText(Piva) & "', ").
				AppendLine("    " & Agro_SQL_SaveNum(Sa_Cod) & ", ").
				AppendLine("    " & Agro_SQL_SaveNum(Lav_Cod) & ", ").
				AppendLine("    '" & Agro_SQL_SaveText(Des_Lib) & "', ")

			strSql.AppendLine("    '" & Agro_SQL_SaveText(Descrizione) & "', ").
				AppendLine("    " & Agro_SQL_SaveDateTime(Data_Tratt_Inizio) & ", ").
				AppendLine("    " & Agro_SQL_SaveDateTime(Data_Tratt_Fine) & ", ").
				AppendLine("    " & Agro_SQL_SaveNum(DurattaTratt) & ", ").
				AppendLine("    '" & Agro_SQL_SaveText(Ricetta_Numero) & "', ").
				AppendLine("    '" & Agro_SQL_SaveText(Note) & "', ").
				AppendLine("    0, NULL, '', ")

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
							 ByVal Piva As String,
							 ByVal Sa_Cod As Integer,
							 ByVal xFiltroAggiuntivo As String,
							 ByRef objParametri As AgronicaCoreParametri,
							 Optional ByVal Lav_Cod As Integer? = Nothing,
							 Optional ByVal Des_Lib As String = Nothing,
							 Optional ByVal Descrizione As String = Nothing,
							 Optional ByVal Ricetta_Numero As String = Nothing,
							 Optional ByVal Data_Tratt_Inizio As DateTime? = Nothing,
							 Optional ByVal Data_Tratt_Fine As DateTime? = Nothing,
							 Optional ByVal DurattaTratt As Integer? = Nothing,
							 Optional ByVal Note As String = Nothing,
							 Optional ByVal Validita_Inizio As Date? = Nothing,
							 Optional ByVal Validita_Fine As Date? = Nothing) As Boolean
		Const nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Zoo_Agenda_W.Modifica()"

		Dim messaggioErrore As String = ""
		Dim strSql As New StringBuilder
		Dim xRisp As Boolean = False

		Try
			If Id_Ricetta = 0 Then
				Throw New Exception("Parametro non corretto nella query (IdRicetta obbligatorio)")
			End If

			If Id_Agenda = 0 Then
				Throw New Exception("Parametro non corretto nella query (IdAgenda obbligatorio)")
			End If

			If Piva = "" Then
				Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
			End If

			strSql.Length = 0

			strSql.AppendLine("UPDATE Ricette_Zoo_Agenda ").
				AppendLine("SET Data_Modifica = " & Agro_SQL_SaveDateTime(Date.Now) & ", ").
				AppendLine("    Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "', ")

			If Not IsNothing(Lav_Cod) Then
				strSql.AppendLine("    Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & ", ")
			End If

			If Not IsNothing(Des_Lib) Then
				strSql.AppendLine("    Des_Lib = '" & Agro_SQL_SaveText(Des_Lib) & "', ")
			End If

			If Not IsNothing(Descrizione) Then
				strSql.AppendLine("    Descrizione = '" & Agro_SQL_SaveText(Descrizione) & "', ")
			End If

			If Not IsNothing(Ricetta_Numero) Then
				strSql.AppendLine("    Ricetta_Numero = '" & Agro_SQL_SaveText(Ricetta_Numero) & "', ")
			End If

			If Not IsNothing(Data_Tratt_Inizio) Then
				strSql.AppendLine("    DataInizioTrattamento = " & Agro_SQL_SaveDateTime(Data_Tratt_Inizio) & ", ")
			End If

			If Not IsNothing(Data_Tratt_Fine) Then
				strSql.AppendLine("    DataFineTrattamento = " & Agro_SQL_SaveDateTime(Data_Tratt_Fine) & ", ")
			End If

			If Not IsNothing(DurattaTratt) Then
				strSql.AppendLine("    DurattaTrattamento = " & Agro_SQL_SaveNum(DurattaTratt) & ", ")
			End If

			If Not IsNothing(Note) Then
				strSql.AppendLine("    Note = '" & Agro_SQL_SaveText(Note) & "', ")
			End If

			If Not IsNothing(Validita_Inizio) Then
				strSql.AppendLine("    Validita_Inizio = " & Agro_SQL_SaveDate(Validita_Inizio) & ", ")
			End If

			If Not IsNothing(Validita_Fine) Then
				strSql.AppendLine("    Validita_Fine = " & Agro_SQL_SaveDate(Validita_Fine) & ", ")
			End If

			strSql.AppendLine("WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ").
				AppendLine("    AND IdRicetta = " & Agro_SQL_SaveNum(Id_Ricetta) & " ").
				AppendLine("    AND IdAgenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")

			If Sa_Cod <> 0 Then
				strSql.AppendLine("    AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
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
							 ByVal Piva As String,
							 ByVal Sa_Cod As Integer,
							 ByVal xFiltroAggiuntivo As String,
							 ByRef objParametri As AgronicaCoreParametri) As Boolean
		Const nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Zoo_Agenda_W.Cancella()"

		'====================================================================================
		'Parametri opzionali :
		'   Piva = ""
		'   Sa_Cod = 0
		'====================================================================================

		Dim messaggioErrore As String = ""
		Dim strSql As New StringBuilder
		Dim xRisp As Boolean = False

		Try

			If Id_Ricetta = 0 Then
				Throw New Exception("Parametro non corretto nella query (IdRicetta obbligatorio)")
			End If

			If Id_Agenda = 0 Then
				Throw New Exception("Parametro non corretto nella query (IdAgenda obbligatorio)")
			End If

			'---------------------------------------------
			strSql.Length = 0

			If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
				strSql.AppendLine("UPDATE Ricette_Zoo_Agenda ").
					AppendLine("SET ").
					AppendLine("    Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "', ").
					AppendLine("    Inviato = -1 ").
					AppendLine("WHERE Inviato >= 0 ")
			Else
				strSql.AppendLine("DELETE ").
					AppendLine("FROM Ricette_Zoo_Agenda ").
					AppendLine("WHERE  1=1 ")
			End If

			strSql.AppendLine("    AND IdRicetta = " & Agro_SQL_SaveNum(Id_Ricetta) & " ").
				AppendLine("    AND IdAgenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")

			If Piva <> "" Then
				strSql.AppendLine("    AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
			End If

			If Sa_Cod <> 0 Then
				strSql.AppendLine("    AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
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

	Public Sub Scrivi(ByRef Ricette_Zoo_Agenda As AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Agenda,
					  ByRef GiasContext As Gias_DeveloperServer_Entities,
					  ByRef objParametriServer As AgronicaCoreParametri)
		Const nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Zoo_Agenda_W.Scrivi()"
		Dim messaggioErrore As String = ""

		Try
			Valorizza(Ricette_Zoo_Agenda, objParametriServer)

			Ricette_Zoo_Agenda.Data_Creazione = DateTime.Now
			Ricette_Zoo_Agenda.Data_Modifica = DateTime.Now
			Ricette_Zoo_Agenda.Username_Creazione = objParametriServer.UsernameOperazione
			Ricette_Zoo_Agenda.Username_Modifica = objParametriServer.UsernameOperazione

			GiasContext.Ricette_Zoo_Agenda.Add(Ricette_Zoo_Agenda)
		Catch ex As Exception
			messaggioErrore = ex.Message
			Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
			Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
		End Try
	End Sub

	Public Sub Modifica(ByRef Ricette_Zoo_Agenda As AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Agenda,
						ByRef GiasContext As Gias_DeveloperServer_Entities,
						ByRef objParametriServer As AgronicaCoreParametri)

		Const nomeRoutine = "AgronicaCoreContabDAL.Ricette_Zoo_Agenda_W.Modifica()"
		Dim messaggioErrore As String = ""

		Try
			Valorizza(Ricette_Zoo_Agenda, objParametriServer)

			Ricette_Zoo_Agenda.Data_Modifica = DateTime.Now
			Ricette_Zoo_Agenda.Username_Modifica = objParametriServer.UsernameOperazione

			GiasContext.Entry(Ricette_Zoo_Agenda).State = EntityState.Modified
		Catch ex As Exception
			messaggioErrore = ex.Message
			Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
			Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
		End Try
	End Sub

	Public Sub Valorizza(ByRef Ricette_Zoo_Agenda As AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Agenda,
						 ByRef objParametriServer As AgronicaCoreParametri)
		Const nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Zoo_Agenda_W.Valorizza()"
		Dim messaggioErrore As String = ""

		Try
			If Ricette_Zoo_Agenda.Des_Lib Is Nothing Then
				Ricette_Zoo_Agenda.Des_Lib = ""
			End If

			If Ricette_Zoo_Agenda.Descrizione Is Nothing Then
				Ricette_Zoo_Agenda.Descrizione = ""
			End If

			If Ricette_Zoo_Agenda.DataInizioTrattamento Is Nothing OrElse Ricette_Zoo_Agenda.DataInizioTrattamento < AGRODATAINIZIO Then
				Ricette_Zoo_Agenda.DataInizioTrattamento = AGRODATAINIZIO
			End If

			If Ricette_Zoo_Agenda.DataFineTrattamento Is Nothing OrElse Ricette_Zoo_Agenda.DataFineTrattamento > AGRODATAFINE Then
				Ricette_Zoo_Agenda.DataFineTrattamento = AGRODATAFINE
			End If

			If Ricette_Zoo_Agenda.Numero Is Nothing Then
				Ricette_Zoo_Agenda.Numero = ""
			End If

			If Ricette_Zoo_Agenda.Note Is Nothing Then
				Ricette_Zoo_Agenda.Note = ""
			End If

			If Ricette_Zoo_Agenda.Blocco_Flag Is Nothing Then
				Ricette_Zoo_Agenda.Blocco_Flag = 0
			End If

			If Ricette_Zoo_Agenda.Blocco_Username Is Nothing Then
				Ricette_Zoo_Agenda.Blocco_Username = ""
			End If

			If Ricette_Zoo_Agenda.Inviato Is Nothing Then
				Ricette_Zoo_Agenda.Inviato = 0
			End If

			If Ricette_Zoo_Agenda.Data_Creazione Is Nothing Then
				Ricette_Zoo_Agenda.Data_Creazione = DateTime.Now
			End If

			If Ricette_Zoo_Agenda.Data_Modifica Is Nothing Then
				Ricette_Zoo_Agenda.Data_Modifica = DateTime.Now
			End If

			If Ricette_Zoo_Agenda.Username_Creazione Is Nothing Then
				Ricette_Zoo_Agenda.Username_Creazione = objParametriServer.UsernameOperazione
			End If

			If Ricette_Zoo_Agenda.Username_Modifica Is Nothing Then
				Ricette_Zoo_Agenda.Username_Modifica = objParametriServer.UsernameOperazione
			End If

			If Ricette_Zoo_Agenda.Validita_Inizio Is Nothing OrElse Ricette_Zoo_Agenda.Validita_Inizio < AGRODATAINIZIO Then
				Ricette_Zoo_Agenda.Validita_Inizio = AGRODATAINIZIO
			End If

			If Ricette_Zoo_Agenda.Validita_Fine Is Nothing OrElse Ricette_Zoo_Agenda.Validita_Fine < AGRODATAFINE Then
				Ricette_Zoo_Agenda.Validita_Fine = AGRODATAFINE
			End If
		Catch ex As Exception
			messaggioErrore = ex.Message
			Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
			Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
		End Try
	End Sub

	Public Sub Elimina(ByRef Ricette_Zoo_Agenda As AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Agenda,
					   ByRef GiasContext As Gias_DeveloperServer_Entities,
					   ByRef objParametriServer As AgronicaCoreParametri)
		Const nomeRoutine = "AgronicaCoreContabDAL.Ricette_Zoo_Agenda_W.Elimina()"
		Dim messaggioErrore As String = ""

		Try
			GiasContext.Ricette_Zoo_Agenda.Attach(Ricette_Zoo_Agenda)
			GiasContext.Ricette_Zoo_Agenda.Remove(Ricette_Zoo_Agenda)

		Catch ex As Exception
			messaggioErrore = ex.Message
			Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
			Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
		End Try
	End Sub

#End Region

End Class
