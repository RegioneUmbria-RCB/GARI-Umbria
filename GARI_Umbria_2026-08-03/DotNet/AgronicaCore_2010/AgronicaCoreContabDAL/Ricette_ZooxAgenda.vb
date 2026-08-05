Imports System.Text
Imports System.Data.Entity
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO

Public Class Ricette_ZooxAgenda_R
	Inherits AgronicaCoreDataProvider.DataProvider

	Public Function Leggi(ByVal Id_Ricetta As Integer,
						  ByVal Id_RigaRicetta As Integer,
						  ByVal Id_Agenda As Integer,
						  ByVal SuperUser_Ricetta As String,
						  ByVal xSelezioneVariabile As enumSelezioneVariabile,
						  ByVal xFiltroAggiuntivo As String,
						  ByVal xOrderBy As String,
						  ByRef objParametri As AgronicaCoreParametri) As DataTable
		Const nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_ZooxAgenda_R.Leggi()"

		Dim messaggioErrore As String = ""
		Dim strSQL As New StringBuilder
		Dim dt As DataTable

		Try
			strSQL.Length = 0

			Select Case xSelezioneVariabile
				Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

					strSQL.AppendLine("SELECT * ").
						AppendLine("FROM Ricette_ZooxAgenda ").
						AppendLine("WHERE Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ").
						AppendLine("    AND Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

					If Id_Ricetta <> 0 Then
						strSQL.AppendLine("    AND IdRicetta = " & Agro_SQL_SaveNum(Id_Ricetta) & " ")
					End If

					If Id_RigaRicetta <> 0 Then
						strSQL.AppendLine("    AND Id_RigaRicetta = " & Agro_SQL_SaveNum(Id_RigaRicetta) & " ")
					End If

					If Id_Agenda <> 0 Then
						strSQL.AppendLine("    AND Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")
					End If

					If SuperUser_Ricetta <> "" Then
						strSQL.AppendLine("    AND SuperUser_Ricetta = '" & Agro_SQL_SaveText(SuperUser_Ricetta) & "' ")
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
						strSQL.AppendLine("ORDER BY Ricette_ZooxAgenda.Id_Ricetta, Ricette_ZooxAgenda.Id_RigaRicetta, Ricette_ZooxAgenda.Id_Agenda ASC ")
					End If

				Case enumSelezioneVariabile.Selezione_TabellaCompleta

					strSQL.AppendLine("SELECT * ").
						AppendLine("FROM Ricette_ZooxAgenda ").
						AppendLine("WHERE Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ").
						AppendLine("    AND Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

					If Id_Ricetta <> 0 Then
						strSQL.AppendLine("    AND IdRicetta = " & Agro_SQL_SaveNum(Id_Ricetta) & " ")
					End If

					If Id_RigaRicetta <> 0 Then
						strSQL.AppendLine("    AND Id_RigaRicetta = " & Agro_SQL_SaveNum(Id_RigaRicetta) & " ")
					End If

					If Id_Agenda <> 0 Then
						strSQL.AppendLine("    AND Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")
					End If

					If SuperUser_Ricetta <> "" Then
						strSQL.AppendLine("    AND SuperUser_Ricetta = '" & Agro_SQL_SaveText(SuperUser_Ricetta) & "' ")
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
						strSQL.AppendLine("ORDER BY Ricette_ZooxAgenda.Id_Ricetta, Ricette_ZooxAgenda.Id_RigaRicetta, Ricette_ZooxAgenda.Id_Agenda ASC ")
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

Public Class Ricette_ZooxAgenda_W
	Inherits AgronicaCoreDataProvider.DataProvider

#Region "Scrittura"

	Public Function Scrivi(ByVal Id_Ricetta As Integer,
						   ByVal Id_RigaRicetta As Integer,
						   ByVal Id_Agenda As Integer,
						   ByVal Validita_Inizio As Date,
						   ByVal Validita_Fine As Date,
						   ByRef objParametri As AgronicaCoreParametri,
						   Optional ByVal SuperUser_Ricetta As String = "") As Boolean
		Const nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_ZooxAgenda_W.Scrivi()"

		Dim messaggioErrore As String = ""
		Dim strSql As New StringBuilder
		Dim xRisp As Boolean = False

		Try
			If Id_Ricetta = 0 Then
				Throw New Exception("Id_Ricetta non e' stata valorizzato.")
			End If

			If Id_RigaRicetta = 0 Then
				Throw New Exception("Id_RigaRicetta non e' stata valorizzato.")
			End If

			If Id_Agenda = 0 Then
				Throw New Exception("Id_Agenda non e' stata valorizzato.")
			End If

			strSql.Length = 0

			strSql.AppendLine("INSERT INTO Ricette_ZooxAgenda ").
				AppendLine("    (Id_Ricetta, Id_RigaRicetta, Id_Agenda, SuperUser_Ricetta, ")

			strSql.AppendLine("     Inviato, DataInvio, ").
				AppendLine("     Data_Creazione, Data_Modifica, ").
				AppendLine("     Username_Creazione, Username_Modifica, ").
				AppendLine("     Validita_Inizio, Validita_Fine) ")

			strSql.AppendLine("VALUES (").
				AppendLine("    " & Agro_SQL_SaveNum(Id_Ricetta) & ", ").
				AppendLine("    " & Agro_SQL_SaveNum(Id_RigaRicetta) & ", ").
				AppendLine("    " & Agro_SQL_SaveNum(Id_Agenda) & ", ").
				AppendLine("    '" & Agro_SQL_SaveText(SuperUser_Ricetta) & "', ")

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
							 ByVal Id_RigaRicetta As Integer,
							 ByVal Id_Agenda As Integer,
							 ByVal xFiltroAggiuntivo As String,
							 ByRef objParametri As AgronicaCoreParametri,
							 Optional ByVal Validita_Inizio As Date? = Nothing,
							 Optional ByVal Validita_Fine As Date? = Nothing,
							 Optional ByVal SuperUser_Ricetta As String = Nothing) As Boolean
		Const nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_ZooxAgenda_W.Modifica()"

		Dim messaggioErrore As String = ""
		Dim strSql As New StringBuilder
		Dim xRisp As Boolean = False

		Try
			If Id_Ricetta = 0 Then
				Throw New Exception("Parametro non corretto nella query (Id_Ricetta obbligatorio)")
			End If

			If Id_RigaRicetta = 0 Then
				Throw New Exception("Parametro non corretto nella query (Id_RigaRicetta obbligatorio)")
			End If

			If Id_Agenda = 0 Then
				Throw New Exception("Parametro non corretto nella query (Id_Agenda obbligatorio)")
			End If

			strSql.Length = 0

			strSql.AppendLine("UPDATE Ricette_ZooxAgenda ").
				AppendLine("SET Data_Modifica = " & Agro_SQL_SaveDateTime(Date.Now) & ", ").
				AppendLine("    Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "', ")

			If Not IsNothing(Validita_Inizio) Then
				strSql.AppendLine("    Validita_Inizio = " & Agro_SQL_SaveDate(Validita_Inizio) & ", ")
			End If

			If Not IsNothing(Validita_Fine) Then
				strSql.AppendLine("    Validita_Fine = " & Agro_SQL_SaveDate(Validita_Fine) & ", ")
			End If

			If Not IsNothing(SuperUser_Ricetta) Then
				strSql.AppendLine("    SuperUser_Ricetta = '" & Agro_SQL_SaveText(SuperUser_Ricetta) & "', ")
			End If

			strSql.AppendLine("WHERE Id_Ricetta = " & Agro_SQL_SaveNum(Id_Ricetta) & " ").
				AppendLine("    AND Id_RigaRicetta = " & Agro_SQL_SaveNum(Id_RigaRicetta) & " ").
				AppendLine("    AND Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")

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
							 ByVal Id_RigaRicetta As Integer,
							 ByVal Id_Agenda As Integer,
							 ByVal xFiltroAggiuntivo As String,
							 ByRef objParametri As AgronicaCoreParametri) As Boolean
		Const nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_ZooxAgenda_W.Cancella()"

		'====================================================================================
		'Parametri opzionali :
		'   Id_RigaRicetta = 0
		'   Id_Agenda = 0
		'====================================================================================

		Dim messaggioErrore As String = ""
		Dim strSql As New StringBuilder
		Dim xRisp As Boolean = False

		Try

			If Id_Ricetta = 0 Then
				Throw New Exception("Parametro non corretto nella query (IdRicetta obbligatorio)")
			End If

			'---------------------------------------------
			strSql.Length = 0

			If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
				strSql.AppendLine("UPDATE Ricette_ZooxAgenda ").
					AppendLine("SET ").
					AppendLine("    Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "', ").
					AppendLine("    Inviato = -1 ").
					AppendLine("WHERE Inviato >= 0 ")
			Else
				strSql.AppendLine("DELETE ").
					AppendLine("FROM Ricette_ZooxAgenda ").
					AppendLine("WHERE  1=1 ")
			End If

			strSql.AppendLine("    AND Id_Ricetta = " & Agro_SQL_SaveNum(Id_Ricetta) & " ")

			If Id_RigaRicetta <> 0 Then
				strSql.AppendLine("    AND Id_RigaRicetta = " & Agro_SQL_SaveNum(Id_RigaRicetta) & " ")
			End If

			If Id_Agenda <> 0 Then
				strSql.AppendLine("    AND Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")
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

	Public Sub Scrivi(ByRef Ricette_ZooxAgenda As AgronicaCoreEntityFramework_POCO.Ricette_ZooxAgenda,
					  ByRef GiasContext As Gias_DeveloperServer_Entities,
					  ByRef objParametriServer As AgronicaCoreParametri)
		Const nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_ZooxAgenda_W.Scrivi()"
		Dim messaggioErrore As String = ""

		Try
			Valorizza(Ricette_ZooxAgenda, objParametriServer)

			Ricette_ZooxAgenda.Data_Creazione = DateTime.Now
			Ricette_ZooxAgenda.Data_Modifica = DateTime.Now
			Ricette_ZooxAgenda.Username_Creazione = objParametriServer.UsernameOperazione
			Ricette_ZooxAgenda.Username_Modifica = objParametriServer.UsernameOperazione

			GiasContext.Ricette_ZooxAgenda.Add(Ricette_ZooxAgenda)
		Catch ex As Exception
			messaggioErrore = ex.Message
			Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
			Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
		End Try
	End Sub

	Public Sub Modifica(ByRef Ricette_ZooxAgenda As AgronicaCoreEntityFramework_POCO.Ricette_ZooxAgenda,
						ByRef GiasContext As Gias_DeveloperServer_Entities,
						ByRef objParametriServer As AgronicaCoreParametri)

		Const nomeRoutine = "AgronicaCoreContabDAL.Ricette_ZooxAgenda_W.Modifica()"
		Dim messaggioErrore As String = ""

		Try
			Valorizza(Ricette_ZooxAgenda, objParametriServer)

			Ricette_ZooxAgenda.Data_Modifica = DateTime.Now
			Ricette_ZooxAgenda.Username_Modifica = objParametriServer.UsernameOperazione

			GiasContext.Entry(Ricette_ZooxAgenda).State = EntityState.Modified
		Catch ex As Exception
			messaggioErrore = ex.Message
			Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
			Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
		End Try
	End Sub

	Public Sub Valorizza(ByRef Ricette_ZooxAgenda As AgronicaCoreEntityFramework_POCO.Ricette_ZooxAgenda,
						 ByRef objParametriServer As AgronicaCoreParametri)
		Const nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_ZooxAgenda_W.Valorizza()"
		Dim messaggioErrore As String = ""

		Try
			If Ricette_ZooxAgenda.SuperUser_Ricetta Is Nothing Then
				Ricette_ZooxAgenda.SuperUser_Ricetta = ""
			End If

			If Ricette_ZooxAgenda.Inviato Is Nothing Then
				Ricette_ZooxAgenda.Inviato = 0
			End If

			If Ricette_ZooxAgenda.Data_Creazione Is Nothing Then
				Ricette_ZooxAgenda.Data_Creazione = DateTime.Now
			End If

			If Ricette_ZooxAgenda.Data_Modifica Is Nothing Then
				Ricette_ZooxAgenda.Data_Modifica = DateTime.Now
			End If

			If Ricette_ZooxAgenda.Username_Creazione Is Nothing Then
				Ricette_ZooxAgenda.Username_Creazione = objParametriServer.UsernameOperazione
			End If

			If Ricette_ZooxAgenda.Username_Modifica Is Nothing Then
				Ricette_ZooxAgenda.Username_Modifica = objParametriServer.UsernameOperazione
			End If

			If Ricette_ZooxAgenda.Validita_Inizio Is Nothing OrElse Ricette_ZooxAgenda.Validita_Inizio < AGRODATAINIZIO Then
				Ricette_ZooxAgenda.Validita_Inizio = AGRODATAINIZIO
			End If

			If Ricette_ZooxAgenda.Validita_Fine Is Nothing OrElse Ricette_ZooxAgenda.Validita_Fine < AGRODATAFINE Then
				Ricette_ZooxAgenda.Validita_Fine = AGRODATAFINE
			End If

		Catch ex As Exception
			messaggioErrore = ex.Message
			Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
			Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
		End Try
	End Sub

	Public Sub Elimina(ByRef Ricette_ZooxAgenda As AgronicaCoreEntityFramework_POCO.Ricette_ZooxAgenda,
					   ByRef GiasContext As Gias_DeveloperServer_Entities,
					   ByRef objParametriServer As AgronicaCoreParametri)
		Const nomeRoutine = "AgronicaCoreContabDAL.Ricette_ZooxAgenda_W.Elimina()"
		Dim messaggioErrore As String = ""

		Try
			GiasContext.Ricette_ZooxAgenda.Attach(Ricette_ZooxAgenda)
			GiasContext.Ricette_ZooxAgenda.Remove(Ricette_ZooxAgenda)

		Catch ex As Exception
			messaggioErrore = ex.Message
			Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
			Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
		End Try
	End Sub

#End Region

End Class
