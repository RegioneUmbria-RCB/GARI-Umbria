Imports System.Text
Imports System.Data.Entity
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreEntityFramework
Imports InData.Anagrafica
Imports System.Net.NetworkInformation

Public Class Ricette_Zoo_Movimenti_R
	Inherits AgronicaCoreDataProvider.DataProvider

	Public Function Leggi(ByVal Id_Ricetta As Integer,
						  ByVal Id_Agenda As Integer,
						  ByVal Id_Mov As Integer,
						  ByVal Piva As String,
						  ByVal Sa_Cod As Integer,
						  ByVal Cod_RisUm As Integer,
						  ByVal Cau_Mov As String,
						  ByVal Data_Movimento As DateTime,
						  ByVal Scadenza As DateTime,
						  ByVal Doc_Numero As String,
						  ByVal Num_Protocollo As String,
						  ByVal xSelezioneVariabile As enumSelezioneVariabile,
						  ByVal xFiltroAggiuntivo As String,
						  ByVal xOrderBy As String,
						  ByRef objParametri As AgronicaCoreParametri) As DataTable
		Const nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Zoo_Movimenti_R.Leggi()"

		Dim messaggioErrore As String = ""
		Dim strSQL As New StringBuilder
		Dim dt As DataTable

		Try
			strSQL.Length = 0

			Select Case xSelezioneVariabile
				Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi
					strSQL.AppendLine("SELECT * ").
						AppendLine("FROM Ricette_Zoo_Movimenti ").
						AppendLine("WHERE Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ").
						AppendLine("    AND Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

					If Id_Ricetta <> 0 Then
						strSQL.AppendLine("    AND IdRicetta = " & Agro_SQL_SaveNum(Id_Ricetta) & " ")
					End If

					If Id_Agenda <> 0 Then
						strSQL.AppendLine("    AND IdAgenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")
					End If

					If Id_Mov <> 0 Then
						strSQL.AppendLine("    AND IdMov = " & Agro_SQL_SaveNum(Id_Mov) & " ")
					End If

					If Piva <> "" Then
						strSQL.AppendLine("    AND Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
					End If

					If Sa_Cod <> 0 Then
						strSQL.AppendLine("    AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
					End If

					If Cod_RisUm <> 0 Then
						strSQL.AppendLine("    AND Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & " ")
					End If

					If Cau_Mov <> "" Then
						strSQL.AppendLine("    AND Cau_Mov = '" & Agro_SQL_SaveText(Cau_Mov) & "' ")
					End If

					If Data_Movimento <> AGRODATAINIZIO Then
						strSQL.AppendLine("    AND Data_Movimento = " & Agro_SQL_SaveDate(Data_Movimento) & " ")
					End If

					If Scadenza <> AGRODATAINIZIO Then
						strSQL.AppendLine("    AND Scadenza = " & Agro_SQL_SaveDate(Scadenza) & " ")
					End If

					If Doc_Numero <> "" Then
						strSQL.AppendLine("    AND Doc_Numero = '" & Agro_SQL_SaveText(Doc_Numero) & "' ")
					End If

					If Num_Protocollo <> "" Then
						strSQL.AppendLine("    AND Num_Protocollo = '" & Agro_SQL_SaveText(Num_Protocollo) & "' ")
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
						strSQL.AppendLine("ORDER BY Ricette_Zoo_Agenda.Piva, Ricette_Zoo_Agenda.Sa_Cod, Ricette_Zoo_Agenda.IdRicetta, Ricette_Zoo_Agenda.IdAgenda, Ricette_Zoo_Agenda.IdMov ASC ")
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

					If Id_Mov <> 0 Then
						strSQL.AppendLine("    AND IdMov = " & Agro_SQL_SaveNum(Id_Mov) & " ")
					End If

					If Piva <> "" Then
						strSQL.AppendLine("    AND Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
					End If

					If Sa_Cod <> 0 Then
						strSQL.AppendLine("    AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
					End If

					If Cod_RisUm <> 0 Then
						strSQL.AppendLine("    AND Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & " ")
					End If

					If Cau_Mov <> "" Then
						strSQL.AppendLine("    AND Cau_Mov = '" & Agro_SQL_SaveText(Cau_Mov) & "' ")
					End If

					If Data_Movimento <> AGRODATAINIZIO Then
						strSQL.AppendLine("    AND Data_Movimento = " & Agro_SQL_SaveDate(Data_Movimento) & " ")
					End If

					If Scadenza <> AGRODATAINIZIO Then
						strSQL.AppendLine("    AND Scadenza = " & Agro_SQL_SaveDate(Scadenza) & " ")
					End If

					If Doc_Numero <> "" Then
						strSQL.AppendLine("    AND Doc_Numero = '" & Agro_SQL_SaveText(Doc_Numero) & "' ")
					End If

					If Num_Protocollo <> "" Then
						strSQL.AppendLine("    AND Num_Protocollo = '" & Agro_SQL_SaveText(Num_Protocollo) & "' ")
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
						strSQL.AppendLine("ORDER BY Ricette_Zoo_Agenda.Piva, Ricette_Zoo_Agenda.Sa_Cod, Ricette_Zoo_Agenda.IdRicetta, Ricette_Zoo_Agenda.IdAgenda, Ricette_Zoo_Agenda.IdMov ASC ")
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

Public Class Ricette_Zoo_Movimenti_W
	Inherits AgronicaCoreDataProvider.DataProvider

#Region "Scrittura"

	Public Function Scrivi(ByVal Id_Ricetta As Integer,
						   ByVal Id_Agenda As Integer,
						   ByVal Id_Mov As Integer,
						   ByVal Piva As String,
						   ByVal Cau_Mov As String,
						   ByVal Validita_Inizio As Date,
						   ByVal Validita_Fine As Date,
						   ByRef objParametri As AgronicaCoreParametri,
						   Optional ByVal Sa_Cod As Integer = 0,
						   Optional ByVal Cod_RisUm As Integer = 0,
						   Optional ByVal Mov_Desc As String = "",
						   Optional ByVal Data_Movimento As DateTime = AGRODATAINIZIO,
						   Optional ByVal Scadenza As DateTime = AGRODATAFINE,
						   Optional ByVal Doc_Numero As String = "",
						   Optional ByVal Num_Protocollo As String = "") As Boolean
		Const nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Zoo_Movimenti_W.Scrivi()"

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

			If Id_Mov = 0 Then
				Throw New Exception("IdMov non e' stata valorizzato.")
			End If

			If Piva = "" Then
				Throw New Exception("Piva non e' stata valorizzata.")
			End If

			strSql.Length = 0

			strSql.AppendLine("INSERT INTO Ricette_Zoo_Movimenti ").
				AppendLine("    (IdRicetta, IdAgenda, IdMov, Piva, SaCod, Cod_RisUm, ").
				AppendLine("     Cau_Mov, Mov_Desc, Data_Movimento, Scadenza, Doc_Numero, Num_Protocollo, ")

			strSql.AppendLine("     Inviato, DataInvio, StrutturaCodice, StrutturaDenominazione, ProtocolloCodice, ").
				AppendLine("     Data_Creazione, Data_Modifica, ").
				AppendLine("     Username_Creazione, Username_Modifica, ").
				AppendLine("     Validita_Inizio, Validita_Fine) ")

			strSql.AppendLine("VALUES (").
				AppendLine("    " & Agro_SQL_SaveNum(Id_Ricetta) & ", ").
				AppendLine("    " & Agro_SQL_SaveNum(Id_Agenda) & ", ").
				AppendLine("    " & Agro_SQL_SaveNum(Id_Mov) & ", ").
				AppendLine("    '" & Agro_SQL_SaveText(Piva) & "', ").
				AppendLine("    " & Agro_SQL_SaveNum(Sa_Cod) & ", ").
				AppendLine("    " & Agro_SQL_SaveNum(Cod_RisUm) & ", ")

			strSql.AppendLine("    '" & Agro_SQL_SaveText(Cau_Mov) & "', ").
				AppendLine("    '" & Agro_SQL_SaveText(Mov_Desc) & "', ").
				AppendLine("    " & Agro_SQL_SaveDateTime(Data_Movimento) & ", ").
				AppendLine("    " & Agro_SQL_SaveDateTime(Scadenza) & ", ").
				AppendLine("    '" & Agro_SQL_SaveText(Doc_Numero) & "', ").
				AppendLine("    '" & Agro_SQL_SaveText(Num_Protocollo) & "', ")

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
							 ByVal Piva As String,
							 ByVal Sa_Cod As Integer,
							 ByVal xFiltroAggiuntivo As String,
							 ByRef objParametri As AgronicaCoreParametri,
							 Optional ByVal Cod_RisUm As Integer? = Nothing,
							 Optional ByVal Cau_Mov As String = Nothing,
							 Optional ByVal Mov_Desc As String = Nothing,
							 Optional ByVal Data_Movimento As DateTime? = Nothing,
							 Optional ByVal Scadenza As DateTime? = Nothing,
							 Optional ByVal Doc_Numero As String = Nothing,
							 Optional ByVal Num_Protocollo As String = Nothing,
							 Optional ByVal Validita_Inizio As Date? = Nothing,
							 Optional ByVal Validita_Fine As Date? = Nothing) As Boolean
		Const nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Zoo_Movimenti_W.Modifica()"

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

			If Id_Mov = 0 Then
				Throw New Exception("Parametro non corretto nella query (IdMov obbligatorio)")
			End If

			If Piva = "" Then
				Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
			End If

			strSql.Length = 0

			strSql.AppendLine("UPDATE Ricette_Zoo_Movimenti ").
				AppendLine("SET Data_Modifica = " & Agro_SQL_SaveDateTime(Date.Now) & ", ").
				AppendLine("    Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "', ")

			If Not IsNothing(Cod_RisUm) Then
				strSql.AppendLine("    Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & ", ")
			End If

			If Not IsNothing(Cau_Mov) Then
				strSql.AppendLine("    Cau_Mov = '" & Agro_SQL_SaveText(Cau_Mov) & "', ")
			End If

			If Not IsNothing(Mov_Desc) Then
				strSql.AppendLine("    Mov_Desc = '" & Agro_SQL_SaveText(Mov_Desc) & "', ")
			End If

			If Not IsNothing(Data_Movimento) Then
				strSql.AppendLine("    Data_Movimento = " & Agro_SQL_SaveDateTime(Data_Movimento) & ", ")
			End If

			If Not IsNothing(Scadenza) Then
				strSql.AppendLine("    Scadenza = " & Agro_SQL_SaveDateTime(Scadenza) & ", ")
			End If

			If Not IsNothing(Doc_Numero) Then
				strSql.AppendLine("    Doc_Numero = '" & Agro_SQL_SaveText(Doc_Numero) & "', ")
			End If

			If Not IsNothing(Num_Protocollo) Then
				strSql.AppendLine("    Num_Protocollo = '" & Agro_SQL_SaveText(Num_Protocollo) & "', ")
			End If

			If Not IsNothing(Validita_Inizio) Then
				strSql.AppendLine("    Validita_Inizio = " & Agro_SQL_SaveDate(Validita_Inizio) & ", ")
			End If

			If Not IsNothing(Validita_Fine) Then
				strSql.AppendLine("    Validita_Fine = " & Agro_SQL_SaveDate(Validita_Fine) & ", ")
			End If

			strSql.AppendLine("WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ").
				AppendLine("    AND IdRicetta = " & Agro_SQL_SaveNum(Id_Ricetta) & " ").
				AppendLine("    AND IdAgenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ").
				AppendLine("    AND IdMov = " & Agro_SQL_SaveNum(Id_Mov) & " ")

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
							 ByVal Id_Mov As Integer,
							 ByVal Piva As String,
							 ByVal Sa_Cod As Integer,
							 ByVal Cod_RisUm As Integer,
							 ByVal xFiltroAggiuntivo As String,
							 ByRef objParametri As AgronicaCoreParametri) As Boolean
		Const nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Zoo_Movimenti_W.Cancella()"

		'====================================================================================
		'Parametri opzionali :
		'   Piva = ""
		'   Sa_Cod = 0
		'   Cod_RisUm = 0
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

			If Id_Mov = 0 Then
				Throw New Exception("Parametro non corretto nella query (IdMov obbligatorio)")
			End If

			'---------------------------------------------
			strSql.Length = 0

			If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
				strSql.AppendLine("UPDATE Ricette_Zoo_Movimenti ").
					AppendLine("SET ").
					AppendLine("    Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "', ").
					AppendLine("    Inviato = -1 ").
					AppendLine("WHERE Inviato >= 0 ")
			Else
				strSql.AppendLine("DELETE ").
					AppendLine("FROM Ricette_Zoo_Movimenti ").
					AppendLine("WHERE  1=1 ")
			End If

			strSql.AppendLine("    AND IdRicetta = " & Agro_SQL_SaveNum(Id_Ricetta) & " ").
				AppendLine("    AND IdAgenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ").
				AppendLine("    AND IdMov = " & Agro_SQL_SaveNum(Id_Mov) & " ")

			If Piva <> "" Then
				strSql.AppendLine("    AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
			End If

			If Sa_Cod <> 0 Then
				strSql.AppendLine("    AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
			End If

			If Cod_RisUm <> 0 Then
				strSql.AppendLine("    AND Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & " ")
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

	Public Sub Scrivi(ByRef Ricetta_Zoo_Movimenti As AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Movimenti,
					  ByRef GiasContext As Gias_DeveloperServer_Entities,
					  ByRef objParametriServer As AgronicaCoreParametri)
		Const nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Zoo_Movimenti_W.Scrivi()"
		Dim messaggioErrore As String = ""

		Try
			Valorizza(Ricetta_Zoo_Movimenti, objParametriServer)

			Ricetta_Zoo_Movimenti.Data_Creazione = DateTime.Now
			Ricetta_Zoo_Movimenti.Data_Modifica = DateTime.Now
			Ricetta_Zoo_Movimenti.Username_Creazione = objParametriServer.UsernameOperazione
			Ricetta_Zoo_Movimenti.Username_Modifica = objParametriServer.UsernameOperazione

			GiasContext.Ricette_Zoo_Movimenti.Add(Ricetta_Zoo_Movimenti)
		Catch ex As Exception
			messaggioErrore = ex.Message
			Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
			Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
		End Try
	End Sub

	Public Sub Modifica(ByRef Ricetta_Zoo_Movimenti As AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Movimenti,
						ByRef GiasContext As Gias_DeveloperServer_Entities,
						ByRef objParametriServer As AgronicaCoreParametri)

		Const nomeRoutine = "AgronicaCoreContabDAL.Ricette_Zoo_Movimenti_W.Modifica()"
		Dim messaggioErrore As String = ""

		Try
			Valorizza(Ricetta_Zoo_Movimenti, objParametriServer)

			Ricetta_Zoo_Movimenti.Data_Modifica = DateTime.Now
			Ricetta_Zoo_Movimenti.Username_Modifica = objParametriServer.UsernameOperazione

			GiasContext.Entry(Ricetta_Zoo_Movimenti).State = EntityState.Modified
		Catch ex As Exception
			messaggioErrore = ex.Message
			Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
			Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
		End Try
	End Sub

	Public Sub Valorizza(ByRef Ricetta_Zoo_Movimenti As AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Movimenti,
						 ByRef objParametriServer As AgronicaCoreParametri)
		Const nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Zoo_Movimenti_W.Valorizza()"
		Dim messaggioErrore As String = ""

		Try
			If Ricetta_Zoo_Movimenti.Cau_Mov Is Nothing Then
				Ricetta_Zoo_Movimenti.Cau_Mov = ""
			End If

			If Ricetta_Zoo_Movimenti.Mov_Desc Is Nothing Then
				Ricetta_Zoo_Movimenti.Mov_Desc = ""
			End If

			If Ricetta_Zoo_Movimenti.Data_Movimento Is Nothing OrElse Ricetta_Zoo_Movimenti.Data_Movimento < AGRODATAINIZIO Then
				Ricetta_Zoo_Movimenti.Data_Movimento = AGRODATAINIZIO
			End If

			If Ricetta_Zoo_Movimenti.Scadenza Is Nothing OrElse Ricetta_Zoo_Movimenti.Scadenza > AGRODATAFINE Then
				Ricetta_Zoo_Movimenti.Scadenza = AGRODATAFINE
			End If

			If Ricetta_Zoo_Movimenti.Doc_Numero Is Nothing Then
				Ricetta_Zoo_Movimenti.Doc_Numero = 0
			End If

			If Ricetta_Zoo_Movimenti.Num_Protocollo Is Nothing Then
				Ricetta_Zoo_Movimenti.Num_Protocollo = 0
			End If

			If Ricetta_Zoo_Movimenti.Inviato Is Nothing Then
				Ricetta_Zoo_Movimenti.Inviato = 0
			End If

			If Ricetta_Zoo_Movimenti.Data_Creazione Is Nothing Then
				Ricetta_Zoo_Movimenti.Data_Creazione = DateTime.Now
			End If

			If Ricetta_Zoo_Movimenti.Data_Modifica Is Nothing Then
				Ricetta_Zoo_Movimenti.Data_Modifica = DateTime.Now
			End If

			If Ricetta_Zoo_Movimenti.Username_Creazione Is Nothing Then
				Ricetta_Zoo_Movimenti.Username_Creazione = objParametriServer.UsernameOperazione
			End If

			If Ricetta_Zoo_Movimenti.Username_Modifica Is Nothing Then
				Ricetta_Zoo_Movimenti.Username_Modifica = objParametriServer.UsernameOperazione
			End If

			If Ricetta_Zoo_Movimenti.Validita_Inizio Is Nothing OrElse Ricetta_Zoo_Movimenti.Validita_Inizio < AGRODATAINIZIO Then
				Ricetta_Zoo_Movimenti.Validita_Inizio = AGRODATAINIZIO
			End If

			If Ricetta_Zoo_Movimenti.Validita_Fine Is Nothing OrElse Ricetta_Zoo_Movimenti.Validita_Fine < AGRODATAFINE Then
				Ricetta_Zoo_Movimenti.Validita_Fine = AGRODATAFINE
			End If
		Catch ex As Exception
			messaggioErrore = ex.Message
			Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
			Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
		End Try
	End Sub

	Public Sub Elimina(ByRef Ricetta_Zoo_Movimenti As AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Movimenti,
					   ByRef GiasContext As Gias_DeveloperServer_Entities,
					   ByRef objParametriServer As AgronicaCoreParametri)
		Const nomeRoutine = "AgronicaCoreContabDAL.Ricette_Zoo_Movimenti_W.Elimina()"
		Dim messaggioErrore As String = ""

		Try
			GiasContext.Ricette_Zoo_Movimenti.Attach(Ricetta_Zoo_Movimenti)
			GiasContext.Ricette_Zoo_Movimenti.Remove(Ricetta_Zoo_Movimenti)

		Catch ex As Exception
			messaggioErrore = ex.Message
			Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
			Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
		End Try
	End Sub

#End Region

End Class
