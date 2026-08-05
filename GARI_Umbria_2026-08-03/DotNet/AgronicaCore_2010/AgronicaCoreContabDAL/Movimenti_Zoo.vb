Imports System.Text
Imports System.Data.Entity
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreEntityFramework


Public Class Movimenti_Zoo_R
	Inherits AgronicaCoreDataProvider.DataProvider

	Public Function Leggi(ByVal Piva As String,
						  ByVal Sa_Cod As Integer,
						  ByVal Id_Agenda As Integer,
						  ByVal Id_Mov As Integer,
						  ByVal Raz_Cod As Integer,
						  ByVal Nazione_Cod As String,
						  ByVal Progetto As String,
						  ByVal Certificato As String,
						  ByVal Data_Documento_Ingresso As DateTime,
						  ByVal Fornitore_Provenienza As String,
						  ByVal Fornitore_Fatturazione As String,
						  ByVal Lotto_Fornitore As String,
						  ByVal N_Bolla_Fornitore As String,
						  ByVal Data_DDT_Ingresso As DateTime,
						  ByVal Modello4_Ingresso As String,
						  ByVal Modello4_Ingresso_Numero As String,
						  ByVal xSelezioneVariabile As enumSelezioneVariabile,
						  ByVal xFiltroAggiuntivo As String,
						  ByVal xOrderBy As String,
						  ByRef objParametri As AgronicaCoreParametri) As DataTable
		Const NomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_Zoo_R.Leggi()"

		Dim messaggioErrore As String = ""
		Dim strSQL As New StringBuilder
		Dim dt As DataTable

		Try
			strSQL.Length = 0

			Select Case xSelezioneVariabile
				Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

					strSQL.Append("SELECT * ")
					strSQL.Append("FROM Movimenti_Zoo ")
					strSQL.Append("WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
					strSQL.Append("    AND Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

					If Piva <> "" Then
						strSQL.Append("    AND Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
					End If

					If Sa_Cod <> 0 Then
						strSQL.Append("    AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
					End If

					If Id_Agenda <> 0 Then
						strSQL.Append("    AND Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")
					End If

					If Id_Mov <> 0 Then
						strSQL.Append("    AND Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & " ")
					End If

					If Raz_Cod <> 0 Then
						strSQL.Append("    AND Raz_Cod = " & Agro_SQL_SaveNum(Raz_Cod) & " ")
					End If

					If Nazione_Cod <> "" Then
						strSQL.Append("    AND Nazione_Cod = '" & Agro_SQL_SaveText(Nazione_Cod) & "' ")
					End If

					If Progetto <> "" Then
						strSQL.Append("    AND Progetto = '" & Agro_SQL_SaveText(Progetto) & "' ")
					End If

					If Certificato <> "" Then
						strSQL.Append("    AND Certificato = '" & Agro_SQL_SaveText(Certificato) & "' ")
					End If

					If Data_Documento_Ingresso <> AGRODATAINIZIO Then
						strSQL.Append("    AND Data_Documento_Ingresso = " & Agro_SQL_SaveDate(Data_Documento_Ingresso) & " ")
					End If

					If Fornitore_Provenienza <> "" Then
						strSQL.Append("    AND Fornitore_Provenienza = '" & Agro_SQL_SaveText(Fornitore_Provenienza) & "' ")
					End If

					If Fornitore_Fatturazione <> "" Then
						strSQL.Append("    AND Fornitore_Fatturazione = '" & Agro_SQL_SaveText(Fornitore_Fatturazione) & "' ")
					End If

					If Lotto_Fornitore <> "" Then
						strSQL.Append("    AND Lotto_Fornitore = '" & Agro_SQL_SaveText(Lotto_Fornitore) & "' ")
					End If

					If N_Bolla_Fornitore <> "" Then
						strSQL.Append("    AND N_Bolla_Fornitore = '" & Agro_SQL_SaveText(N_Bolla_Fornitore) & "' ")
					End If

					If Data_DDT_Ingresso <> AGRODATAINIZIO Then
						strSQL.Append("    AND Data_DDT_Ingresso = " & Agro_SQL_SaveDate(Data_DDT_Ingresso) & " ")
					End If

					If Modello4_Ingresso <> "" Then
						strSQL.Append("    AND Modello4_Ingresso = '" & Agro_SQL_SaveText(Modello4_Ingresso) & "' ")
					End If

					If Modello4_Ingresso_Numero <> "" Then
						strSQL.Append("    AND Modello4_Ingresso_Numero = '" & Agro_SQL_SaveText(Modello4_Ingresso_Numero) & "' ")
					End If

					If xFiltroAggiuntivo <> "" Then
						strSQL.Append("    AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) & " ")
					End If

					'--------------------------------------------------------------------------
					Select Case objParametri.FlagVisibilita
						Case enumVisibilita.Visibilita_SoloNonCancellati
							strSQL.Append("    AND Movimenti_Zoo.Inviato >= 0 ")
						Case enumVisibilita.Visibilita_SoloCancellati
							strSQL.Append("    AND Movimenti_Zoo.Inviato = -1 ")
						Case enumVisibilita.Visibilita_Tutti
							'...................................
						Case Else
							Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
					End Select
					'--------------------------------------------------------------------------

					If xOrderBy <> "" Then
						strSQL.Append("ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
					Else
						strSQL.Append("ORDER BY Piva, Sa_Cod, Id_Agenda, Id_Mov ASC ")
					End If

				Case enumSelezioneVariabile.Selezione_TabellaCompleta

					strSQL.Append("SELECT Movimenti_Zoo.*, Movimenti.Cau_Mov, Movimenti.Mov_Desc, Movimenti.Data_Movimento ")
					strSQL.Append("FROM Movimenti_Zoo ")

					'JOIN Movimenti
					strSQL.Append("INNER JOIN Movimenti ON Movimenti.PIVA = Movimenti_Zoo.Piva ")
					strSQL.Append("    AND Movimenti.Sa_Cod = Movimenti_Zoo.Sa_Cod ")
					strSQL.Append("    AND Movimenti.Id_Agenda = Movimenti_Zoo.Id_Agenda ")
					strSQL.Append("    AND Movimenti.Id_Mov = Movimenti_Zoo.Id_Mov ")

					strSQL.Append("WHERE Movimenti_Zoo.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
					strSQL.Append("    AND Movimenti_Zoo.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

					If Piva <> "" Then
						strSQL.Append("    AND Movimenti_Zoo.Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
					End If

					If Sa_Cod <> 0 Then
						strSQL.Append("    AND Movimenti_Zoo.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
					End If

					If Id_Agenda <> 0 Then
						strSQL.Append("    AND Movimenti_Zoo.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")
					End If

					If Id_Mov <> 0 Then
						strSQL.Append("    AND Movimenti_Zoo.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & " ")
					End If

					If Raz_Cod <> 0 Then
						strSQL.Append("    AND Raz_Cod = " & Agro_SQL_SaveNum(Raz_Cod) & " ")
					End If

					If Nazione_Cod <> "" Then
						strSQL.Append("    AND Nazione_Cod = '" & Agro_SQL_SaveText(Nazione_Cod) & "' ")
					End If

					If Progetto <> "" Then
						strSQL.Append("    AND Progetto = '" & Agro_SQL_SaveText(Progetto) & "' ")
					End If

					If Certificato <> "" Then
						strSQL.Append("    AND Certificato = '" & Agro_SQL_SaveText(Certificato) & "' ")
					End If

					If Data_Documento_Ingresso <> AGRODATAINIZIO Then
						strSQL.Append("    AND Data_Documento_Ingresso = " & Agro_SQL_SaveDate(Data_Documento_Ingresso) & " ")
					End If

					If Fornitore_Provenienza <> "" Then
						strSQL.Append("    AND Fornitore_Provenienza = '" & Agro_SQL_SaveText(Fornitore_Provenienza) & "' ")
					End If

					If Fornitore_Fatturazione <> "" Then
						strSQL.Append("    AND Fornitore_Fatturazione = '" & Agro_SQL_SaveText(Fornitore_Fatturazione) & "' ")
					End If

					If Lotto_Fornitore <> "" Then
						strSQL.Append("    AND Lotto_Fornitore = '" & Agro_SQL_SaveText(Lotto_Fornitore) & "' ")
					End If

					If N_Bolla_Fornitore <> "" Then
						strSQL.Append("    AND N_Bolla_Fornitore = '" & Agro_SQL_SaveText(N_Bolla_Fornitore) & "' ")
					End If

					If Data_DDT_Ingresso <> AGRODATAINIZIO Then
						strSQL.Append("    AND Data_DDT_Ingresso = " & Agro_SQL_SaveDate(Data_DDT_Ingresso) & " ")
					End If

					If Modello4_Ingresso <> "" Then
						strSQL.Append("    AND Modello4_Ingresso = '" & Agro_SQL_SaveText(Modello4_Ingresso) & "' ")
					End If

					If Modello4_Ingresso_Numero <> "" Then
						strSQL.Append("    AND Modello4_Ingresso_Numero = '" & Agro_SQL_SaveText(Modello4_Ingresso_Numero) & "' ")
					End If

					If xFiltroAggiuntivo <> "" Then
						strSQL.Append("    AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) & " ")
					End If

					'--------------------------------------------------------------------------
					Select Case objParametri.FlagVisibilita
						Case enumVisibilita.Visibilita_SoloNonCancellati
							strSQL.Append("    AND Movimenti_Zoo.Inviato >= 0 ")
						Case enumVisibilita.Visibilita_SoloCancellati
							strSQL.Append("    AND Movimenti_Zoo.Inviato = -1 ")
						Case enumVisibilita.Visibilita_Tutti
							'...................................
						Case Else
							Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
					End Select
					'--------------------------------------------------------------------------

					If xOrderBy <> "" Then
						strSQL.Append("ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
					Else
						strSQL.Append("ORDER BY Movimenti_Zoo.Piva, Movimenti_Zoo.Sa_Cod, Movimenti_Zoo.Id_Agenda, Movimenti_Zoo.Id_Mov ASC ")
					End If

				Case enumSelezioneVariabile.Selezione_JoinDescrizioni

					strSQL.Append("SELECT Movimenti_Zoo.*, ")
					strSQL.Append("    Imprese.rag_soc, ")
					strSQL.Append("    Agenda.des_lib, ")
					strSQL.Append("    Movimenti.Cau_Mov, Movimenti.Mov_Desc, Movimenti.Data_Movimento ")
					strSQL.Append("FROM Movimenti_Zoo ")

					'JOIN Imprese
					strSQL.AppendLine("INNER JOIN Imprese ON Imprese.Piva = Movimenti_Zoo.Piva ")

					'JOIN Agenda
					strSQL.AppendLine("INNER JOIN Agenda ON Agenda.Piva = Movimenti_Zoo.Piva ")
					strSQL.AppendLine("    AND Agenda.Id_Agenda = Movimenti_Zoo.Id_Agenda ")

					'JOIN Movimenti
					strSQL.Append("INNER JOIN Movimenti ON Movimenti.PIVA = Movimenti_Zoo.Piva ")
					strSQL.Append("    AND Movimenti.Sa_Cod = Movimenti_Zoo.Sa_Cod ")
					strSQL.Append("    AND Movimenti.Id_Agenda = Movimenti_Zoo.Id_Agenda ")
					strSQL.Append("    AND Movimenti.Id_Mov = Movimenti_Zoo.Id_Mov ")

					strSQL.Append("WHERE Movimenti_Zoo.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
					strSQL.Append("    AND Movimenti_Zoo.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

					If Piva <> "" Then
						strSQL.Append("    AND Movimenti_Zoo.Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
					End If

					If Sa_Cod <> 0 Then
						strSQL.Append("    AND Movimenti_Zoo.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
					End If

					If Id_Agenda <> 0 Then
						strSQL.Append("    AND Movimenti_Zoo.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")
					End If

					If Id_Mov <> 0 Then
						strSQL.Append("    AND Movimenti_Zoo.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & " ")
					End If

					If Raz_Cod <> 0 Then
						strSQL.Append("    AND Raz_Cod = " & Agro_SQL_SaveNum(Raz_Cod) & " ")
					End If

					If Nazione_Cod <> "" Then
						strSQL.Append("    AND Nazione_Cod = '" & Agro_SQL_SaveText(Nazione_Cod) & "' ")
					End If

					If Progetto <> "" Then
						strSQL.Append("    AND Progetto = '" & Agro_SQL_SaveText(Progetto) & "' ")
					End If

					If Certificato <> "" Then
						strSQL.Append("    AND Certificato = '" & Agro_SQL_SaveText(Certificato) & "' ")
					End If

					If Data_Documento_Ingresso <> AGRODATAINIZIO Then
						strSQL.Append("    AND Data_Documento_Ingresso = " & Agro_SQL_SaveDate(Data_Documento_Ingresso) & " ")
					End If

					If Fornitore_Provenienza <> "" Then
						strSQL.Append("    AND Fornitore_Provenienza = '" & Agro_SQL_SaveText(Fornitore_Provenienza) & "' ")
					End If

					If Fornitore_Fatturazione <> "" Then
						strSQL.Append("    AND Fornitore_Fatturazione = '" & Agro_SQL_SaveText(Fornitore_Fatturazione) & "' ")
					End If

					If Lotto_Fornitore <> "" Then
						strSQL.Append("    AND Lotto_Fornitore = '" & Agro_SQL_SaveText(Lotto_Fornitore) & "' ")
					End If

					If N_Bolla_Fornitore <> "" Then
						strSQL.Append("    AND N_Bolla_Fornitore = '" & Agro_SQL_SaveText(N_Bolla_Fornitore) & "' ")
					End If

					If Data_DDT_Ingresso <> AGRODATAINIZIO Then
						strSQL.Append("    AND Data_DDT_Ingresso = " & Agro_SQL_SaveDate(Data_DDT_Ingresso) & " ")
					End If

					If Modello4_Ingresso <> "" Then
						strSQL.Append("    AND Modello4_Ingresso = '" & Agro_SQL_SaveText(Modello4_Ingresso) & "' ")
					End If

					If Modello4_Ingresso_Numero <> "" Then
						strSQL.Append("    AND Modello4_Ingresso_Numero = '" & Agro_SQL_SaveText(Modello4_Ingresso_Numero) & "' ")
					End If

					If xFiltroAggiuntivo <> "" Then
						strSQL.Append("    AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) & " ")
					End If

					'--------------------------------------------------------------------------
					Select Case objParametri.FlagVisibilita
						Case enumVisibilita.Visibilita_SoloNonCancellati
							strSQL.Append("    AND Movimenti_Zoo.Inviato >= 0 ")
						Case enumVisibilita.Visibilita_SoloCancellati
							strSQL.Append("    AND Movimenti_Zoo.Inviato = -1 ")
						Case enumVisibilita.Visibilita_Tutti
							'...................................
						Case Else
							Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
					End Select
					'--------------------------------------------------------------------------

					If xOrderBy <> "" Then
						strSQL.Append("ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
					Else
						strSQL.Append("ORDER BY Movimenti_Zoo.Piva, Movimenti_Zoo.Sa_Cod, Movimenti_Zoo.Id_Agenda, Movimenti_Zoo.Id_Mov ASC ")
					End If

				Case enumSelezioneVariabile.Selezione_JoinCompleta

			End Select

			'--------------------------------------------------------------------------
			dt = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)
			'--------------------------------------------------------------------------

		Catch ex As Exception
			messaggioErrore = ex.Message
			Scrivi_LOG(objParametri, NomeRoutine, messaggioErrore)
			dt = Nothing
			Throw New Exception("[" & NomeRoutine & "] : " & messaggioErrore)
		End Try

		Return dt

	End Function

End Class


Public Class Movimenti_Zoo_W
	Inherits AgronicaCoreDataProvider.DataProvider

#Region "Scrittura"

	Public Function Scrivi(ByVal Piva As String,
						   ByVal Sa_Cod As Integer,
						   ByVal Id_Agenda As Integer,
						   ByVal Id_Mov As Integer,
						   ByVal Validita_Inizio As Date,
						   ByVal Validita_Fine As Date,
						   ByRef objParametri As AgronicaCoreParametri,
						   Optional ByVal Raz_Cod As Integer = 0,
						   Optional ByVal Nazione_Cod As String = "",
						   Optional ByVal Progetto As String = "",
						   Optional ByVal Certificato As String = "",
						   Optional ByVal Data_Documento_Ingresso As DateTime = #2/1/1900#,
						   Optional ByVal Fornitore_Provenienza As String = "",
						   Optional ByVal Fornitore_Fatturazione As String = "",
						   Optional ByVal Lotto_Fornitore As String = "",
						   Optional ByVal N_Bolla_Fornitore As String = "",
						   Optional ByVal Data_DDT_Ingresso As DateTime = #2/1/1900#,
						   Optional ByVal Modello4_Ingresso As String = "",
						   Optional ByVal Modello4_Ingresso_Numero As String = "") As Boolean
		Const nomeRoutine = "AgronicaCoreContabDAL.Movimenti_Zoo_W.Scrivi()"

		Dim messaggioErrore As String = ""
		Dim strSql As New StringBuilder
		Dim xRisp As Boolean = False

		Try
			strSql.Length = 0

			strSql.AppendLine("INSERT INTO Movimenti_Zoo ")
			strSql.AppendLine("    (Piva, Sa_cod, Id_Agenda, Id_Mov, ")
			strSql.AppendLine("     Raz_Cod, Nazione_Cod, Progetto, Certificato, Data_Documento_Ingresso, ")
			strSql.AppendLine("     Fornitore_Provenienza, Fornitore_Fatturazione, Lotto_Fornitore, ")
			strSql.AppendLine("     N_Bolla_Fornitore, Data_DDT_Ingresso, Modello4_Ingresso, Modello4_Ingresso_Numero, ")
			strSql.AppendLine("     Inviato, DataInvio, ")
			strSql.AppendLine("     Data_Creazione, Data_Modifica, ")
			strSql.AppendLine("     Username_Creazione, Username_Modifica, ")
			strSql.AppendLine("     Validita_Inizio, Validita_Fine) ")

			strSql.AppendLine("VALUES (")
			strSql.AppendLine("    '" & Agro_SQL_SaveText(Piva) & "', ")
			strSql.AppendLine("    " & Agro_SQL_SaveNum(Sa_Cod) & ", ")
			strSql.AppendLine("    " & Agro_SQL_SaveNum(Id_Agenda) & ", ")
			strSql.AppendLine("    " & Agro_SQL_SaveNum(Id_Mov) & ", ")
			strSql.AppendLine("    " & Agro_SQL_SaveNum(Raz_Cod) & ", ")
			strSql.AppendLine("    '" & Agro_SQL_SaveText(Nazione_Cod) & "', ")
			strSql.AppendLine("    '" & Agro_SQL_SaveText(Progetto) & "', ")
			strSql.AppendLine("    '" & Agro_SQL_SaveText(Certificato) & "', ")
			strSql.AppendLine("    " & Agro_SQL_SaveDateTime(Data_Documento_Ingresso) & ", ")
			strSql.AppendLine("    '" & Agro_SQL_SaveText(Fornitore_Provenienza) & "', ")
			strSql.AppendLine("    '" & Agro_SQL_SaveText(Fornitore_Fatturazione) & "', ")
			strSql.AppendLine("    '" & Agro_SQL_SaveText(Lotto_Fornitore) & "', ")
			strSql.AppendLine("    '" & Agro_SQL_SaveText(N_Bolla_Fornitore) & "', ")
			strSql.AppendLine("    " & Agro_SQL_SaveDateTime(Data_DDT_Ingresso) & ", ")
			strSql.AppendLine("    '" & Agro_SQL_SaveText(Modello4_Ingresso) & "', ")
			strSql.AppendLine("    '" & Agro_SQL_SaveText(Modello4_Ingresso_Numero) & "', ")
			strSql.AppendLine("    0, NULL,  ")
			strSql.AppendLine("    " & Agro_SQL_SaveDateTime(DateTime.Now) & ", ")
			strSql.AppendLine("    " & Agro_SQL_SaveDateTime(DateTime.Now) & ", ")
			strSql.AppendLine("    '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "', ")
			strSql.AppendLine("    '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "', ")
			strSql.AppendLine("    " & Agro_SQL_SaveDate(Validita_Inizio) & ", ")
			strSql.AppendLine("    " & Agro_SQL_SaveDate(Validita_Fine) & " ")
			strSql.AppendLine(") ")

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

	Public Function Modifica(ByVal Piva As String,
							 ByVal Sa_Cod As Integer,
							 ByVal Id_Agenda As Integer,
							 ByVal Id_Mov As Integer,
							 ByRef objParametri As AgronicaCoreParametri,
							 Optional ByVal Raz_Cod As Integer? = Nothing,
							 Optional ByVal Nazione_Cod As String = Nothing,
							 Optional ByVal Progetto As String = Nothing,
							 Optional ByVal Certificato As String = Nothing,
							 Optional ByVal Data_Documento_Ingresso As DateTime? = Nothing,
							 Optional ByVal Fornitore_Provenienza As String = Nothing,
							 Optional ByVal Fornitore_Fatturazione As String = Nothing,
							 Optional ByVal Lotto_Fornitore As String = Nothing,
							 Optional ByVal N_Bolla_Fornitore As String = Nothing,
							 Optional ByVal Data_DDT_Ingresso As DateTime? = Nothing,
							 Optional ByVal Modello4_Ingresso As String = Nothing,
							 Optional ByVal Modello4_Ingresso_Numero As String = Nothing,
							 Optional ByVal Validita_Inizio As Date? = Nothing,
							 Optional ByVal Validita_Fine As Date? = Nothing,
							 Optional ByVal Data_Modifica As DateTime = AGRODATAINIZIO,
							 Optional ByVal Username_Modifica As String = "",
							 Optional ByVal xFiltroAggiuntivo As String = "") As Boolean
		Const nomeRoutine = "AgronicaCoreContabDAL.Movimenti_Zoo_W.Modifica()"

		'====================================================================================
		'Parametri opzionali :
		'   Tutti i valori non chiave (se impostati a nothing o non passati 
		'   non ne verrà fatto l'aggiornamento e rimarranno i valori precedenti)
		'====================================================================================

		Dim messaggioErrore As String = ""
		Dim strSql As New StringBuilder
		Dim xRisp As Boolean = False

		Try
			If Data_Modifica = AGRODATAINIZIO Then
				Data_Modifica = Date.Now
			End If

			If Username_Modifica = "" Then
				Username_Modifica = objParametri.UsernameOperazione
			End If

			If Piva = "" Then
				Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
			End If

			'If Sa_Cod = 0 Then
			'    Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
			'End If

			If Id_Agenda = 0 Then
				Throw New Exception("Parametro non corretto nella query (Id_Agenda obbligatorio)")
			End If

			'If Id_Mov = 0 Then
			'    Throw New Exception("Parametro non corretto nella query (Id_Mov obbligatorio)")
			'End If

			'---------------------------------------------
			strSql.Length = 0

			strSql.AppendLine("UPDATE Movimenti_Zoo ")
			strSql.AppendLine("SET Data_Modifica = " & Agro_SQL_SaveDateTime(Data_Modifica) & ", ")
			strSql.AppendLine("    Username_Modifica = '" & Agro_SQL_SaveText(Username_Modifica) & "', ")

			If Not IsNothing(Raz_Cod) Then
				strSql.AppendLine("    Raz_Cod = " & Agro_SQL_SaveNum(Raz_Cod) & ", ")
			End If

			If Not IsNothing(Nazione_Cod) Then
				strSql.AppendLine("    Nazione_Cod = '" & Agro_SQL_SaveText(Nazione_Cod) & "', ")
			End If

			If Not IsNothing(Progetto) Then
				strSql.AppendLine("    Progetto = '" & Agro_SQL_SaveText(Progetto) & "', ")
			End If

			If Not IsNothing(Certificato) Then
				strSql.AppendLine("    Certificato = '" & Agro_SQL_SaveText(Certificato) & "', ")
			End If

			If Not IsNothing(Data_Documento_Ingresso) Then
				strSql.AppendLine("    Data_Documento_Ingresso = " & Agro_SQL_SaveDateTime(Data_Documento_Ingresso) & ", ")
			End If

			If Not IsNothing(Fornitore_Provenienza) Then
				strSql.AppendLine("    Fornitore_Provenienza = '" & Agro_SQL_SaveText(Fornitore_Provenienza) & "', ")
			End If

			If Not IsNothing(Fornitore_Fatturazione) Then
				strSql.AppendLine("    Fornitore_Fatturazione = '" & Agro_SQL_SaveText(Fornitore_Fatturazione) & "', ")
			End If

			If Not IsNothing(Lotto_Fornitore) Then
				strSql.AppendLine("    Lotto_Fornitore = '" & Agro_SQL_SaveText(Lotto_Fornitore) & "', ")
			End If

			If Not IsNothing(N_Bolla_Fornitore) Then
				strSql.AppendLine("    N_Bolla_Fornitore = '" & Agro_SQL_SaveText(N_Bolla_Fornitore) & "', ")
			End If

			If Not IsNothing(Data_DDT_Ingresso) Then
				strSql.AppendLine("    Data_DDT_Ingresso = " & Agro_SQL_SaveDateTime(Data_DDT_Ingresso) & ", ")
			End If

			If Not IsNothing(Modello4_Ingresso) Then
				strSql.AppendLine("    Modello4_Ingresso = '" & Agro_SQL_SaveText(Modello4_Ingresso) & "', ")
			End If

			If Not IsNothing(Modello4_Ingresso_Numero) Then
				strSql.AppendLine("    Modello4_Ingresso_Numero = '" & Agro_SQL_SaveText(Modello4_Ingresso_Numero) & "', ")
			End If

			If Not IsNothing(Validita_Inizio) Then
				strSql.AppendLine("    Validita_Inizio = " & Agro_SQL_SaveDate(Validita_Inizio) & ", ")
			End If

			If Not IsNothing(Validita_Fine) Then
				strSql.AppendLine("    Validita_Fine = " & Agro_SQL_SaveDate(Validita_Fine) & ", ")
			End If

			strSql.AppendLine("WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

			If Sa_Cod <> 0 Then
				strSql.AppendLine("    AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
			End If

			strSql.AppendLine("    AND Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")

			If Id_Mov <> 0 Then
				strSql.AppendLine("    AND Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & " ")
			End If

			'----------------------------------------------------------------------
			If xFiltroAggiuntivo <> "" Then
				strSql.AppendLine("    AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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

	Public Function Cancella(ByVal Piva As String,
							 ByVal Sa_Cod As Integer,
							 ByVal Id_Agenda As Integer,
							 ByVal Id_Mov As Integer,
							 ByVal xFiltroAggiuntivo As String,
							 ByRef objParametri As AgronicaCoreParametri) As Boolean

		Const nomeRoutine = "AgronicaCoreContabDAL.Movimenti_Zoo_W.Cancella()"

		'====================================================================================
		'Parametri opzionali :
		'   Piva = ""
		'   Sa_Cod = 0
		'   Id_Agenda = 0
		'   Id_Mov = 0
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
				strSql.AppendLine("UPDATE Movimenti_Zoo ")
				strSql.AppendLine("SET ")
				strSql.AppendLine("    Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "', ")
				strSql.AppendLine("    Inviato = -1 ")
				strSql.AppendLine("WHERE Inviato >= 0 ")
			Else
				strSql.AppendLine("DELETE ")
				strSql.AppendLine("FROM Movimenti_Zoo ")
				strSql.AppendLine("WHERE  1=1 ")
			End If

			If Piva <> "" Then
				strSql.AppendLine("    AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
			End If

			If Sa_Cod <> 0 Then
				strSql.AppendLine("    AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
			End If

			strSql.AppendLine("    AND Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")

			If Id_Mov <> 0 Then
				strSql.AppendLine("    AND Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & " ")
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

	Public Sub Scrivi(ByRef Movimenti_Zoo As AgronicaCoreEntityFramework_POCO.Movimenti_Zoo,
					  ByRef GiasContext As Gias_DeveloperServer_Entities,
					  ByRef objParametriServer As AgronicaCoreParametri)
		Const nomeRoutine = "AgronicaCoreContabDAL.Movimenti_Zoo_W.Scrivi()"
		Dim messaggioErrore As String = ""

		Try
			Valorizza(Movimenti_Zoo, objParametriServer)

			Movimenti_Zoo.Data_Creazione = DateTime.Now
			Movimenti_Zoo.Data_Modifica = DateTime.Now
			Movimenti_Zoo.Username_Creazione = objParametriServer.UsernameOperazione
			Movimenti_Zoo.Username_Modifica = objParametriServer.UsernameOperazione

			GiasContext.Movimenti_Zoo.Add(Movimenti_Zoo)
		Catch ex As Exception
			messaggioErrore = ex.Message
			Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
			Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
		End Try
	End Sub

	Public Sub Modifica(ByRef Movimenti_Zoo As AgronicaCoreEntityFramework_POCO.Movimenti_Zoo,
						ByRef GiasContext As Gias_DeveloperServer_Entities,
						ByRef objParametriServer As AgronicaCoreParametri)

		Const nomeRoutine = "AgronicaCoreContabDAL.Movimenti_Zoo_W.Modifica()"
		Dim messaggioErrore As String = ""

		Try
			Valorizza(Movimenti_Zoo, objParametriServer)

			Movimenti_Zoo.Data_Modifica = DateTime.Now
			Movimenti_Zoo.Username_Modifica = objParametriServer.UsernameOperazione

			GiasContext.Entry(Movimenti_Zoo).State = EntityState.Modified
		Catch ex As Exception
			messaggioErrore = ex.Message
			Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
			Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
		End Try
	End Sub

	Public Sub Valorizza(ByRef Movimenti_Zoo As AgronicaCoreEntityFramework_POCO.Movimenti_Zoo,
						 ByRef objParametriServer As AgronicaCoreParametri)
		Const nomeRoutine = "AgronicaCoreContabDAL.Movimenti_Zoo_W.Valorizza()"
		Dim messaggioErrore As String = ""

		Try
			If Movimenti_Zoo.Raz_Cod Is Nothing Then
				Movimenti_Zoo.Raz_Cod = 0
			End If

			If Movimenti_Zoo.Nazione_Cod Is Nothing Then
				Movimenti_Zoo.Nazione_Cod = ""
			End If

			If Movimenti_Zoo.Progetto Is Nothing Then
				Movimenti_Zoo.Progetto = ""
			End If

			If Movimenti_Zoo.Certificato Is Nothing Then
				Movimenti_Zoo.Certificato = ""
			End If

			If Movimenti_Zoo.Data_Documento_Ingresso Is Nothing OrElse Movimenti_Zoo.Data_Documento_Ingresso < AGRODATAINIZIO Then
				Movimenti_Zoo.Data_Documento_Ingresso = AGRODATAINIZIO
			End If

			If Movimenti_Zoo.Fornitore_Provenienza Is Nothing Then
				Movimenti_Zoo.Fornitore_Provenienza = ""
			End If

			If Movimenti_Zoo.Fornitore_Fatturazione Is Nothing Then
				Movimenti_Zoo.Fornitore_Fatturazione = ""
			End If

			If Movimenti_Zoo.Lotto_Fornitore Is Nothing Then
				Movimenti_Zoo.Lotto_Fornitore = ""
			End If

			If Movimenti_Zoo.N_Bolla_Fornitore Is Nothing Then
				Movimenti_Zoo.N_Bolla_Fornitore = ""
			End If

			If Movimenti_Zoo.Data_DDT_Ingresso Is Nothing OrElse Movimenti_Zoo.Data_DDT_Ingresso < AGRODATAINIZIO Then
				Movimenti_Zoo.Data_DDT_Ingresso = AGRODATAINIZIO
			End If

			If Movimenti_Zoo.Modello4_Ingresso Is Nothing Then
				Movimenti_Zoo.Modello4_Ingresso = ""
			End If

			If Movimenti_Zoo.Modello4_Ingresso_Numero Is Nothing Then
				Movimenti_Zoo.Modello4_Ingresso_Numero = ""
			End If

			If Movimenti_Zoo.Pres_Numero Is Nothing Then
				Movimenti_Zoo.Pres_Numero = ""
			End If

			If Movimenti_Zoo.Pres_Numero Is Nothing Then
				Movimenti_Zoo.Pres_Numero = ""
			End If

			If Movimenti_Zoo.PresRiga_Numero Is Nothing Then
				Movimenti_Zoo.PresRiga_Numero = ""
			End If

			If Movimenti_Zoo.Data_Creazione Is Nothing Then
				Movimenti_Zoo.Data_Creazione = DateTime.Now
			End If

			If Movimenti_Zoo.Data_Modifica Is Nothing Then
				Movimenti_Zoo.Data_Modifica = DateTime.Now
			End If

			If Movimenti_Zoo.Username_Creazione Is Nothing Then
				Movimenti_Zoo.Username_Creazione = objParametriServer.UsernameOperazione
			End If

			If Movimenti_Zoo.Username_Modifica Is Nothing Then
				Movimenti_Zoo.Username_Modifica = objParametriServer.UsernameOperazione
			End If

			If Movimenti_Zoo.Validita_Inizio Is Nothing OrElse Movimenti_Zoo.Validita_Inizio < AGRODATAINIZIO Then
				Movimenti_Zoo.Validita_Inizio = AGRODATAINIZIO
			End If

			If Movimenti_Zoo.Validita_Fine Is Nothing OrElse Movimenti_Zoo.Validita_Fine < AGRODATAFINE Then
				Movimenti_Zoo.Validita_Fine = AGRODATAFINE
			End If
		Catch ex As Exception
			messaggioErrore = ex.Message
			Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
			Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
		End Try
	End Sub

	Public Sub Elimina(ByRef Movimenti_Zoo As AgronicaCoreEntityFramework_POCO.Movimenti_Zoo,
					   ByRef GiasContext As Gias_DeveloperServer_Entities,
					   ByRef objParametriServer As AgronicaCoreParametri)
		Const nomeRoutine = "AgronicaCoreContabDAL.Movimenti_Zoo_W.Elimina()"
		Dim messaggioErrore As String = ""

		Try
			GiasContext.Movimenti_Zoo.Attach(Movimenti_Zoo)
			GiasContext.Movimenti_Zoo.Remove(Movimenti_Zoo)

		Catch ex As Exception
			messaggioErrore = ex.Message
			Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
			Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
		End Try
	End Sub

#End Region

End Class
