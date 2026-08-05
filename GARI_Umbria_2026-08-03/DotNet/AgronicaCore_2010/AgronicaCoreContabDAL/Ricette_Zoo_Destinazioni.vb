Imports System.Text
Imports System.Data.Entity
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreEntityFramework
Imports InData.Anagrafica
Imports System.Net.NetworkInformation
Imports AgronicaCoreStampeDAL

Public Class Ricette_Zoo_Destinazioni_R
	Inherits AgronicaCoreDataProvider.DataProvider

	Public Function Leggi(ByVal Piva As String,
						  ByVal Sa_Cod As Integer,
						  ByVal Id_Ricetta As Integer,
						  ByVal Id_Agenda As Integer,
						  ByVal Id_Mov As Integer,
						  ByVal Id_Dettaglio As Integer,
						  ByVal Id_Destinazione As Integer,
						  ByVal Cod_Animale As Integer,
						  ByVal Matricola As String,
						  ByVal Codifica_Cod As String,
						  ByVal Diagnosi_Cod As String,
						  ByVal Data_Nascita As DateTime,
						  ByVal FlStatoAnomalia As String,
						  ByVal Identificativo As String,
						  ByVal Numero As String,
						  ByVal NumCapi As Integer,
						  ByVal Sesso As String,
						  ByVal Somm_Cod As String,
						  ByVal SottoCat_Cod As String,
						  ByVal Spe_Cod As Integer,
						  ByVal Cardinalita As Integer,
						  ByVal TempiSospensione As Integer,
						  ByVal RegSco_Numero As String,
						  ByVal xSelezioneVariabile As enumSelezioneVariabile,
						  ByVal xFiltroAggiuntivo As String,
						  ByVal xOrderBy As String,
						  ByRef objParametri As AgronicaCoreParametri) As DataTable
		Const nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Zoo_Destinazioni_R.Leggi()"

		Dim messaggioErrore As String = ""
		Dim strSQL As New StringBuilder
		Dim dt As DataTable

		Try
			strSQL.Length = 0

			Select Case xSelezioneVariabile
				Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

					strSQL.AppendLine("SELECT * ").
						AppendLine("FROM Ricette_Zoo_Destinazioni ").
						AppendLine("WHERE Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ").
						AppendLine("    AND Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

					If Id_Ricetta <> 0 Then
						strSQL.AppendLine("    AND IdRicetta = " & Agro_SQL_SaveNum(Id_Ricetta) & " ")
					End If

					If Id_Agenda <> 0 Then
						strSQL.AppendLine("    AND IdAgenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")
					End If

					If Id_Mov <> 0 Then
						strSQL.AppendLine("    AND IdMov = " & Agro_SQL_SaveNum(Id_Dettaglio) & " ")
					End If

					If Id_Dettaglio <> 0 Then
						strSQL.AppendLine("    AND IdDettaglio = " & Agro_SQL_SaveNum(Id_Dettaglio) & " ")
					End If

					If Id_Destinazione <> 0 Then
						strSQL.AppendLine("    AND IdDestinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & " ")
					End If

					If Piva <> "" Then
						strSQL.AppendLine("    AND Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
					End If

					If Sa_Cod <> 0 Then
						strSQL.AppendLine("    AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
					End If

					If Cod_Animale <> 0 Then
						strSQL.AppendLine("    AND CodAnimale = " & Agro_SQL_SaveNum(Cod_Animale) & " ")
					End If

					If Matricola <> "" Then
						strSQL.AppendLine("    AND Matricola = '" & Agro_SQL_SaveText(Matricola) & "' ")
					End If

					If Codifica_Cod <> "" Then
						strSQL.AppendLine("    AND CodificaCodice = '" & Agro_SQL_SaveText(Codifica_Cod) & "' ")
					End If

					If Diagnosi_Cod <> "" Then
						strSQL.AppendLine("    AND DiagnosiCodice = '" & Agro_SQL_SaveText(Diagnosi_Cod) & "' ")
					End If

					If Data_Nascita <> AGRODATAINIZIO Then
						strSQL.AppendLine("    AND DataNascita = " & Agro_SQL_SaveDate(Data_Nascita) & " ")
					End If

					If Identificativo <> "" Then
						strSQL.AppendLine("    AND Identificativo = '" & Agro_SQL_SaveText(Identificativo) & "' ")
					End If

					If FlStatoAnomalia <> "" Then
						strSQL.AppendLine("    AND FlStatoAnomalia = '" & Agro_SQL_SaveText(FlStatoAnomalia) & "' ")
					End If

					If Numero <> "" Then
						strSQL.AppendLine("    AND Numero = '" & Agro_SQL_SaveText(Numero) & "' ")
					End If

					If NumCapi <> 0 Then
						strSQL.AppendLine("    AND NumeroAnimali = " & Agro_SQL_SaveNum(NumCapi) & " ")
					End If

					If Sesso <> "" Then
						strSQL.AppendLine("    AND Sesso = '" & Agro_SQL_SaveText(Sesso) & "' ")
					End If

					If Somm_Cod <> "" Then
						strSQL.AppendLine("    AND SomministrazioneCodice = '" & Agro_SQL_SaveText(Somm_Cod) & "' ")
					End If

					If SottoCat_Cod <> "" Then
						strSQL.AppendLine("    AND SottocategoriaCodice = '" & Agro_SQL_SaveText(SottoCat_Cod) & "' ")
					End If

					If Spe_Cod <> "" Then
						strSQL.AppendLine("    AND SpecieCodice = '" & Agro_SQL_SaveText(Spe_Cod) & "' ")
					End If

					If Cardinalita <> "" Then
						strSQL.AppendLine("    AND Cardinalita = '" & Agro_SQL_SaveText(Cardinalita) & "' ")
					End If

					If TempiSospensione <> "" Then
						strSQL.AppendLine("    AND TempiSospensione = '" & Agro_SQL_SaveText(TempiSospensione) & "' ")
					End If

					If RegSco_Numero <> "" Then
						strSQL.AppendLine("    AND RegSco_Numero = '" & Agro_SQL_SaveText(RegSco_Numero) & "' ")
					End If

					If xFiltroAggiuntivo <> "" Then
						strSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
					End If

					'--------------------------------------------------------------------------
					Select Case objParametri.FlagVisibilita
						Case enumVisibilita.Visibilita_SoloNonCancellati
							strSQL.AppendLine("    AND Ricette_Zoo_Destinazioni.Inviato >= 0 ")
						Case enumVisibilita.Visibilita_SoloCancellati
							strSQL.AppendLine("    AND Ricette_Zoo_Destinazioni.Inviato = -1 ")
						Case enumVisibilita.Visibilita_Tutti
							'...................................
						Case Else
							Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
					End Select
					'--------------------------------------------------------------------------

					If xOrderBy <> "" Then
						strSQL.AppendLine("ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
					Else
						strSQL.AppendLine("ORDER BY Ricette_Zoo_Destinazioni.Piva, Ricette_Zoo_Destinazioni.Sa_Cod, Ricette_Zoo_Destinazioni.IdRicetta, Ricette_Zoo_Destinazioni.IdAgenda, Ricette_Zoo_Destinazioni.IdMov, Ricette_Zoo_Destinazioni.IdDettaglio, Ricette_Zoo_Destinazioni.IdDestinazione ASC ")
					End If

				Case enumSelezioneVariabile.Selezione_TabellaCompleta

					strSQL.AppendLine("SELECT * ").
						AppendLine("FROM Ricette_Zoo_Destinazioni ").
						AppendLine("WHERE Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ").
						AppendLine("    AND Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

					If Id_Ricetta <> 0 Then
						strSQL.AppendLine("    AND IdRicetta = " & Agro_SQL_SaveNum(Id_Ricetta) & " ")
					End If

					If Id_Agenda <> 0 Then
						strSQL.AppendLine("    AND IdAgenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")
					End If

					If Id_Mov <> 0 Then
						strSQL.AppendLine("    AND IdMov = " & Agro_SQL_SaveNum(Id_Dettaglio) & " ")
					End If

					If Id_Dettaglio <> 0 Then
						strSQL.AppendLine("    AND IdDettaglio = " & Agro_SQL_SaveNum(Id_Dettaglio) & " ")
					End If

					If Id_Destinazione <> 0 Then
						strSQL.AppendLine("    AND IdDestinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & " ")
					End If

					If Piva <> "" Then
						strSQL.AppendLine("    AND Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
					End If

					If Sa_Cod <> 0 Then
						strSQL.AppendLine("    AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
					End If

					If Cod_Animale <> 0 Then
						strSQL.AppendLine("    AND CodAnimale = " & Agro_SQL_SaveNum(Cod_Animale) & " ")
					End If

					If Matricola <> "" Then
						strSQL.AppendLine("    AND Matricola = '" & Agro_SQL_SaveText(Matricola) & "' ")
					End If

					If Codifica_Cod <> "" Then
						strSQL.AppendLine("    AND CodificaCodice = '" & Agro_SQL_SaveText(Codifica_Cod) & "' ")
					End If

					If Diagnosi_Cod <> "" Then
						strSQL.AppendLine("    AND DiagnosiCodice = '" & Agro_SQL_SaveText(Diagnosi_Cod) & "' ")
					End If

					If Data_Nascita <> AGRODATAINIZIO Then
						strSQL.AppendLine("    AND DataNascita = " & Agro_SQL_SaveDate(Data_Nascita) & " ")
					End If

					If FlStatoAnomalia <> "" Then
						strSQL.AppendLine("    AND FlStatoAnomalia = '" & Agro_SQL_SaveText(FlStatoAnomalia) & "' ")
					End If

					If Identificativo <> "" Then
						strSQL.AppendLine("    AND Identificativo = '" & Agro_SQL_SaveText(Identificativo) & "' ")
					End If

					If Numero <> "" Then
						strSQL.AppendLine("    AND Numero = '" & Agro_SQL_SaveText(Numero) & "' ")
					End If

					If NumCapi <> 0 Then
						strSQL.AppendLine("    AND NumeroAnimali = " & Agro_SQL_SaveNum(NumCapi) & " ")
					End If

					If Sesso <> "" Then
						strSQL.AppendLine("    AND Sesso = '" & Agro_SQL_SaveText(Sesso) & "' ")
					End If

					If Somm_Cod <> "" Then
						strSQL.AppendLine("    AND SomministrazioneCodice = '" & Agro_SQL_SaveText(Somm_Cod) & "' ")
					End If

					If SottoCat_Cod <> "" Then
						strSQL.AppendLine("    AND SottocategoriaCodice = '" & Agro_SQL_SaveText(SottoCat_Cod) & "' ")
					End If

					If Spe_Cod <> "" Then
						strSQL.AppendLine("    AND SpecieCodice = '" & Agro_SQL_SaveText(Spe_Cod) & "' ")
					End If

					If Cardinalita <> "" Then
						strSQL.AppendLine("    AND Cardinalita = '" & Agro_SQL_SaveText(Cardinalita) & "' ")
					End If

					If TempiSospensione <> "" Then
						strSQL.AppendLine("    AND TempiSospensione = '" & Agro_SQL_SaveText(TempiSospensione) & "' ")
					End If

					If RegSco_Numero <> "" Then
						strSQL.AppendLine("    AND RegSco_Numero = '" & Agro_SQL_SaveText(RegSco_Numero) & "' ")
					End If

					If xFiltroAggiuntivo <> "" Then
						strSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
					End If

					'--------------------------------------------------------------------------
					Select Case objParametri.FlagVisibilita
						Case enumVisibilita.Visibilita_SoloNonCancellati
							strSQL.AppendLine("    AND Ricette_Zoo_Destinazioni.Inviato >= 0 ")
						Case enumVisibilita.Visibilita_SoloCancellati
							strSQL.AppendLine("    AND Ricette_Zoo_Destinazioni.Inviato = -1 ")
						Case enumVisibilita.Visibilita_Tutti
							'...................................
						Case Else
							Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
					End Select
					'--------------------------------------------------------------------------

					If xOrderBy <> "" Then
						strSQL.AppendLine("ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
					Else
						strSQL.AppendLine("ORDER BY Ricette_Zoo_Destinazioni.Piva, Ricette_Zoo_Destinazioni.Sa_Cod, Ricette_Zoo_Destinazioni.IdRicetta, Ricette_Zoo_Destinazioni.IdAgenda, Ricette_Zoo_Destinazioni.IdMov, Ricette_Zoo_Destinazioni.IdDettaglio, Ricette_Zoo_Destinazioni.IdDestinazione ASC ")
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

Public Class Ricette_Zoo_Destinazioni_W
	Inherits AgronicaCoreDataProvider.DataProvider

#Region "Scrivi"

	Public Function Scrivi(ByVal Id_Ricetta As Integer,
						   ByVal Id_Agenda As Integer,
						   ByVal Id_Mov As Integer,
						   ByVal Id_Dettaglio As Integer,
						   ByVal Id_Destinazione As Integer,
						   ByVal Piva As String,
						   ByVal Sa_Cod As Integer,
						   ByVal Validita_Inizio As Date,
						   ByVal Validita_Fine As Date,
						   ByRef objParametri As AgronicaCoreParametri,
						   Optional ByVal Cod_Animale As Integer = 0,
						   Optional ByVal Matricola As String = "",
						   Optional ByVal Codifica_Cod As String = "",
						   Optional ByVal Codifica_Des As String = "",
						   Optional ByVal Diagnosi_Cod As String = "",
						   Optional ByVal Diagnosi_Des As String = "",
						   Optional ByVal Data_Nascita As DateTime = AGRODATAINIZIO,
						   Optional ByVal FlStatoAnomalia As String = "",
						   Optional ByVal Identificativo As String = "",
						   Optional ByVal Numero As String = "",
						   Optional ByVal Note As String = "",
						   Optional ByVal NumCapi As Integer = 0,
						   Optional ByVal Sesso As String = "",
						   Optional ByVal Somm_Cod As String = "",
						   Optional ByVal SottoCat_Cod As String = "",
						   Optional ByVal Spe_Cod As Integer = 0,
						   Optional ByVal Cardinalita As Integer = 0,
						   Optional ByVal TempiSospensione As Integer = 0,
						   Optional ByVal RegSco_Numero As String = "") As Boolean
		Const nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Zoo_Destinazioni_W.Scrivi()"

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

			If Id_Dettaglio = 0 Then
				Throw New Exception("IdDettaglio non e' stata valorizzato.")
			End If

			If Id_Destinazione = 0 Then
				Throw New Exception("IdDestinazione non e' stata valorizzato.")
			End If

			If Piva = "" Then
				Throw New Exception("Piva non e' stata valorizzata.")
			End If

			If Sa_Cod = 0 Then
				Throw New Exception("Sa_Cod non e' stata valorizzato.")
			End If

			strSql.Length = 0

			strSql.AppendLine("INSERT INTO Ricette_Zoo_Destinazioni ").
				AppendLine("    (IdRicetta, IdAgenda, IdMov, IdDettaglio, IdDestinazione, Piva, SaCod, ")

			strSql.AppendLine("     CodAnimale, Matricola, CodificaCodice, CodificaDescrizione, ").
				AppendLine("    DiagnosiCodice, DiagnosiDescrizione, DataNascita, FlStatoAnomalia, ").
				AppendLine("    Identificativo, Numero, Note, NumeroAnimali, Sesso, SomministrazioneCodice, ").
				AppendLine("    SottocategoriaCodice, SpecieCodice, Cardinalita, TempiSospensione, RegSco_Numero, ")

			strSql.AppendLine("     Inviato, DataInvio, StrutturaCodice, StrutturaDenominazione, ProtocolloCodice, ").
				AppendLine("     Data_Creazione, Data_Modifica, ").
				AppendLine("     Username_Creazione, Username_Modifica, ").
				AppendLine("     Validita_Inizio, Validita_Fine) ")

			strSql.AppendLine("VALUES (").
				AppendLine("    " & Agro_SQL_SaveNum(Id_Ricetta) & ", ").
				AppendLine("    " & Agro_SQL_SaveNum(Id_Agenda) & ", ").
				AppendLine("    " & Agro_SQL_SaveNum(Id_Mov) & ", ").
				AppendLine("    " & Agro_SQL_SaveNum(Id_Dettaglio) & ", ").
				AppendLine("    " & Agro_SQL_SaveNum(Id_Destinazione) & ", ").
				AppendLine("    '" & Agro_SQL_SaveText(Piva) & "', ").
				AppendLine("    " & Agro_SQL_SaveNum(Sa_Cod) & ", ")

			strSql.AppendLine("    " & Agro_SQL_SaveNum(Cod_Animale) & " ,").
				AppendLine("    '" & Agro_SQL_SaveText(Matricola) & "', ").
				AppendLine("    '" & Agro_SQL_SaveText(Codifica_Cod) & "', ").
				AppendLine("    '" & Agro_SQL_SaveText(Codifica_Des) & "', ").
				AppendLine("    '" & Agro_SQL_SaveText(Diagnosi_Cod) & "', ").
				AppendLine("    '" & Agro_SQL_SaveText(Diagnosi_Des) & "', ").
				AppendLine("    " & Agro_SQL_SaveDateTime(Data_Nascita) & ", ").
				AppendLine("    " & Agro_SQL_SaveNum(FlStatoAnomalia) & ", ").
				AppendLine("    " & Agro_SQL_SaveNum(Identificativo) & ", ").
				AppendLine("    '" & Agro_SQL_SaveText(Numero) & "', ").
				AppendLine("    '" & Agro_SQL_SaveText(Note) & "', ").
				AppendLine("    " & Agro_SQL_SaveNum(NumCapi) & ", ").
				AppendLine("    '" & Agro_SQL_SaveText(Sesso) & "', ").
				AppendLine("    '" & Agro_SQL_SaveText(Somm_Cod) & "', ").
				AppendLine("    '" & Agro_SQL_SaveText(SottoCat_Cod) & "', ").
				AppendLine("    " & Agro_SQL_SaveNum(Spe_Cod) & ", ").
				AppendLine("    " & Agro_SQL_SaveNum(Cardinalita) & ", ").
				AppendLine("    " & Agro_SQL_SaveNum(TempiSospensione) & ", ").
				AppendLine("    '" & Agro_SQL_SaveText(RegSco_Numero) & "', ")

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
							 ByVal Id_Dettaglio As Integer,
							 ByVal Id_Destinazione As Integer,
							 ByVal Piva As String,
							 ByVal Sa_Cod As Integer,
							 ByVal xFiltroAggiuntivo As String,
							 ByRef objParametri As AgronicaCoreParametri,
							 Optional ByVal Cod_Animale As Integer? = Nothing,
							 Optional ByVal Matricola As String = Nothing,
							 Optional ByVal Codifica_Cod As String = Nothing,
							 Optional ByVal Codifica_Des As String = Nothing,
							 Optional ByVal Diagnosi_Cod As String = Nothing,
							 Optional ByVal Diagnosi_Des As String = Nothing,
							 Optional ByVal Data_Nascita As DateTime? = Nothing,
							 Optional ByVal FlStatoAnomalia As String = Nothing,
							 Optional ByVal Identificativo As String = Nothing,
							 Optional ByVal Numero As String = Nothing,
							 Optional ByVal Note As String = Nothing,
							 Optional ByVal NumCapi As Integer? = Nothing,
							 Optional ByVal Sesso As String = Nothing,
							 Optional ByVal Somm_Cod As String = Nothing,
							 Optional ByVal SottoCat_Cod As String = Nothing,
							 Optional ByVal Spe_Cod As Integer? = Nothing,
							 Optional ByVal Cardinalita As Integer? = Nothing,
							 Optional ByVal TempiSospensione As Integer? = Nothing,
							 Optional ByVal RegSco_Numero As String = Nothing,
							 Optional ByVal Validita_Inizio As Date? = Nothing,
							 Optional ByVal Validita_Fine As Date? = Nothing) As Boolean
		Const nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Zoo_Destinazioni_W.Modifica()"

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

			If Id_Dettaglio = 0 Then
				Throw New Exception("Parametro non corretto nella query (IdDettaglio obbligatorio)")
			End If

			If Id_Destinazione = 0 Then
				Throw New Exception("Parametro non corretto nella query (IdDestinazione obbligatorio)")
			End If

			If Piva = "" Then
				Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
			End If

			strSql.Length = 0

			strSql.AppendLine("UPDATE Ricette_Zoo_Destinazioni ").
				AppendLine("SET Data_Modifica = " & Agro_SQL_SaveDateTime(Date.Now) & ", ").
				AppendLine("    Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "', ")

			If Not IsNothing(Cod_Animale) Then
				strSql.AppendLine("    CodAnimale = " & Agro_SQL_SaveNum(Cod_Animale) & ", ")
			End If

			If Not IsNothing(Matricola) Then
				strSql.AppendLine("    Matricola = '" & Agro_SQL_SaveText(Matricola) & "', ")
			End If

			If Not IsNothing(Codifica_Cod) Then
				strSql.AppendLine("    CodificaCodice = '" & Agro_SQL_SaveText(Codifica_Cod) & "', ")
			End If

			If Not IsNothing(Codifica_Des) Then
				strSql.AppendLine("    CodificaDescrizione = '" & Agro_SQL_SaveText(Codifica_Des) & "', ")
			End If

			If Not IsNothing(Diagnosi_Cod) Then
				strSql.AppendLine("    DiagnosiCodice = '" & Agro_SQL_SaveText(Diagnosi_Cod) & "', ")
			End If

			If Not IsNothing(Diagnosi_Des) Then
				strSql.AppendLine("    DiagnosiDescrizione = '" & Agro_SQL_SaveText(Diagnosi_Des) & "', ")
			End If

			If Not IsNothing(Data_Nascita) Then
				strSql.AppendLine("    DataNascita = " & Agro_SQL_SaveDateTime(Data_Nascita) & ", ")
			End If

			If Not IsNothing(FlStatoAnomalia) Then
				strSql.AppendLine("    DiagnosiDescrizione = '" & Agro_SQL_SaveText(Diagnosi_Des) & "', ")
			End If

			If Not IsNothing(Identificativo) Then
				strSql.AppendLine("    Identificativo = '" & Agro_SQL_SaveText(Identificativo) & "', ")
			End If

			If Not IsNothing(Numero) Then
				strSql.AppendLine("    Numero = '" & Agro_SQL_SaveText(Numero) & "', ")
			End If

			If Not IsNothing(Note) Then
				strSql.AppendLine("    Note = '" & Agro_SQL_SaveText(Note) & "', ")
			End If

			If Not IsNothing(NumCapi) Then
				strSql.AppendLine("    NumeroAnimali = " & Agro_SQL_SaveNum(NumCapi) & ", ")
			End If

			If Not IsNothing(Sesso) Then
				strSql.AppendLine("    Sesso = '" & Agro_SQL_SaveText(Sesso) & "', ")
			End If

			If Not IsNothing(Somm_Cod) Then
				strSql.AppendLine("    SomministrazioneCodice = '" & Agro_SQL_SaveText(Somm_Cod) & "', ")
			End If

			If Not IsNothing(SottoCat_Cod) Then
				strSql.AppendLine("    SottocategoriaCodice = '" & Agro_SQL_SaveText(SottoCat_Cod) & "', ")
			End If

			If Not IsNothing(Spe_Cod) Then
				strSql.AppendLine("    SpecieCodice = " & Agro_SQL_SaveNum(Spe_Cod) & ", ")
			End If

			If Not IsNothing(Cardinalita) Then
				strSql.AppendLine("    Cardinalita = " & Agro_SQL_SaveNum(Cardinalita) & ", ")
			End If

			If Not IsNothing(TempiSospensione) Then
				strSql.AppendLine("    TempiSospensione = " & Agro_SQL_SaveNum(TempiSospensione) & ", ")
			End If

			If Not IsNothing(RegSco_Numero) Then
				strSql.AppendLine("    RegSco_Numero = '" & Agro_SQL_SaveText(RegSco_Numero) & "', ")
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
				AppendLine("    AND IdMov = " & Agro_SQL_SaveNum(Id_Dettaglio) & " ").
				AppendLine("    AND IdDettaglio = " & Agro_SQL_SaveNum(Id_Dettaglio) & " ").
				AppendLine("    AND IdDestinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & " ")

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

	Public Function Cancella(ByVal Ricetta_Id As Integer,
							 ByVal Id_Agenda As Integer,
							 ByVal Id_Mov As Integer,
							 ByVal Id_Dettaglio As Integer,
							 ByVal Id_Destinazione As Integer,
							 ByVal Piva As String,
							 ByVal Sa_Cod As Integer,
							 ByVal xFiltroAggiuntivo As String,
							 ByRef objParametri As AgronicaCoreParametri) As Boolean
		Const nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Zoo_Destinazioni_W.Cancella()"

		'====================================================================================
		'Parametri opzionali :
		'   Piva = ""
		'   Sa_Cod = 0
		'====================================================================================

		Dim messaggioErrore As String = ""
		Dim strSql As New StringBuilder
		Dim xRisp As Boolean = False

		Try

			If Ricetta_Id = 0 Then
				Throw New Exception("Parametro non corretto nella query (IdRicetta obbligatorio)")
			End If

			If Id_Agenda = 0 Then
				Throw New Exception("Parametro non corretto nella query (IdAgenda obbligatorio)")
			End If

			If Id_Mov = 0 Then
				Throw New Exception("Parametro non corretto nella query (IdMov obbligatorio)")
			End If

			If Id_Dettaglio = 0 Then
				Throw New Exception("Parametro non corretto nella query (IdDettaglio obbligatorio)")
			End If

			If Id_Destinazione = 0 Then
				Throw New Exception("Parametro non corretto nella query (IdDestinazione obbligatorio)")
			End If

			'---------------------------------------------
			strSql.Length = 0

			If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
				strSql.AppendLine("UPDATE Ricette_Zoo_Destinazioni ").
					AppendLine("SET ").
					AppendLine("    Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "', ").
					AppendLine("    Inviato = -1 ").
					AppendLine("WHERE Inviato >= 0 ")
			Else
				strSql.AppendLine("DELETE ").
					AppendLine("FROM Ricette_Zoo_Destinazioni ").
					AppendLine("WHERE  1=1 ")
			End If

			strSql.AppendLine("    AND IdRicetta = " & Agro_SQL_SaveNum(Ricetta_Id) & " ").
				AppendLine("    AND IdAgenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ").
				AppendLine("    AND IdMov = " & Agro_SQL_SaveNum(Id_Mov) & " ").
				AppendLine("    AND IdDettaglio = " & Agro_SQL_SaveNum(Id_Dettaglio) & " ").
				AppendLine("    AND IdDestinazione = " & Agro_SQL_SaveNum(Id_Dettaglio) & " ")

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

	Public Sub Scrivi(ByRef Ricette_Zoo_Destinazioni As AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Destinazioni,
					  ByRef GiasContext As Gias_DeveloperServer_Entities,
					  ByRef objParametriServer As AgronicaCoreParametri)
		Const nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Zoo_Destinazioni_W.Scrivi()"
		Dim messaggioErrore As String = ""

		Try
			Valorizza(Ricette_Zoo_Destinazioni, objParametriServer)

			Ricette_Zoo_Destinazioni.Data_Creazione = DateTime.Now
			Ricette_Zoo_Destinazioni.Data_Modifica = DateTime.Now
			Ricette_Zoo_Destinazioni.Username_Creazione = objParametriServer.UsernameOperazione
			Ricette_Zoo_Destinazioni.Username_Modifica = objParametriServer.UsernameOperazione

			GiasContext.Ricette_Zoo_Destinazioni.Add(Ricette_Zoo_Destinazioni)
		Catch ex As Exception
			messaggioErrore = ex.Message
			Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
			Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
		End Try
	End Sub

	Public Sub Modifica(ByRef Ricette_Zoo_Destinazioni As AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Destinazioni,
						ByRef GiasContext As Gias_DeveloperServer_Entities,
						ByRef objParametriServer As AgronicaCoreParametri)

		Const nomeRoutine = "AgronicaCoreContabDAL.Ricette_Zoo_Destinazion_W.Modifica()"
		Dim messaggioErrore As String = ""

		Try
			Valorizza(Ricette_Zoo_Destinazioni, objParametriServer)

			Ricette_Zoo_Destinazioni.Data_Modifica = DateTime.Now
			Ricette_Zoo_Destinazioni.Username_Modifica = objParametriServer.UsernameOperazione

			GiasContext.Entry(Ricette_Zoo_Destinazioni).State = EntityState.Modified
		Catch ex As Exception
			messaggioErrore = ex.Message
			Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
			Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
		End Try
	End Sub

	Public Sub Valorizza(ByRef Ricette_Zoo_Destinazioni As AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Destinazioni,
						 ByRef objParametriServer As AgronicaCoreParametri)
		Const nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Zoo_Destinazioni_W.Valorizza()"
		Dim messaggioErrore As String = ""

		Try
			If Ricette_Zoo_Destinazioni.CodAnimale Is Nothing Then
				Ricette_Zoo_Destinazioni.CodAnimale = 0
			End If

			If Ricette_Zoo_Destinazioni.Matricola Is Nothing Then
				Ricette_Zoo_Destinazioni.Matricola = ""
			End If

			If Ricette_Zoo_Destinazioni.CodificaCodice Is Nothing Then
				Ricette_Zoo_Destinazioni.CodificaCodice = ""
			End If

			If Ricette_Zoo_Destinazioni.CodificaDescrizione Is Nothing Then
				Ricette_Zoo_Destinazioni.CodificaDescrizione = ""
			End If

			If Ricette_Zoo_Destinazioni.DiagnosiCodice Is Nothing Then
				Ricette_Zoo_Destinazioni.DiagnosiCodice = ""
			End If

			If Ricette_Zoo_Destinazioni.DiagnosiDescrizione Is Nothing Then
				Ricette_Zoo_Destinazioni.DiagnosiDescrizione = ""
			End If

			If Ricette_Zoo_Destinazioni.DataNascita Is Nothing OrElse Ricette_Zoo_Destinazioni.DataNascita < AGRODATAINIZIO Then
				Ricette_Zoo_Destinazioni.DataNascita = AGRODATAINIZIO
			End If

			If Ricette_Zoo_Destinazioni.FlStatoAnomalia Is Nothing Then
				Ricette_Zoo_Destinazioni.FlStatoAnomalia = ""
			End If

			If Ricette_Zoo_Destinazioni.Identificativo Is Nothing Then
				Ricette_Zoo_Destinazioni.Identificativo = ""
			End If

			If Ricette_Zoo_Destinazioni.Numero Is Nothing Then
				Ricette_Zoo_Destinazioni.Numero = ""
			End If

			If Ricette_Zoo_Destinazioni.Note Is Nothing Then
				Ricette_Zoo_Destinazioni.Note = ""
			End If

			If Ricette_Zoo_Destinazioni.NumeroAnimali Is Nothing Then
				Ricette_Zoo_Destinazioni.NumeroAnimali = 0
			End If

			If Ricette_Zoo_Destinazioni.Sesso Is Nothing Then
				Ricette_Zoo_Destinazioni.Sesso = ""
			End If

			If Ricette_Zoo_Destinazioni.SomministrazioneCodice Is Nothing Then
				Ricette_Zoo_Destinazioni.SomministrazioneCodice = ""
			End If

			If Ricette_Zoo_Destinazioni.SottocategoriaCodice Is Nothing Then
				Ricette_Zoo_Destinazioni.SottocategoriaCodice = ""
			End If

			If Ricette_Zoo_Destinazioni.SpecieCodice Is Nothing Then
				Ricette_Zoo_Destinazioni.SpecieCodice = 0
			End If

			If Ricette_Zoo_Destinazioni.Cardinalita Is Nothing Then
				Ricette_Zoo_Destinazioni.SpecieCodice = 0
			End If

			If Ricette_Zoo_Destinazioni.TempiSospensione Is Nothing Then
				Ricette_Zoo_Destinazioni.TempiSospensione = 0
			End If

			If Ricette_Zoo_Destinazioni.RegSco_Numero Is Nothing Then
				Ricette_Zoo_Destinazioni.RegSco_Numero = ""
			End If

			If Ricette_Zoo_Destinazioni.Inviato Is Nothing Then
				Ricette_Zoo_Destinazioni.Inviato = 0
			End If

			If Ricette_Zoo_Destinazioni.Data_Creazione Is Nothing Then
				Ricette_Zoo_Destinazioni.Data_Creazione = DateTime.Now
			End If

			If Ricette_Zoo_Destinazioni.Data_Modifica Is Nothing Then
				Ricette_Zoo_Destinazioni.Data_Modifica = DateTime.Now
			End If

			If Ricette_Zoo_Destinazioni.Username_Creazione Is Nothing Then
				Ricette_Zoo_Destinazioni.Username_Creazione = objParametriServer.UsernameOperazione
			End If

			If Ricette_Zoo_Destinazioni.Username_Modifica Is Nothing Then
				Ricette_Zoo_Destinazioni.Username_Modifica = objParametriServer.UsernameOperazione
			End If

			If Ricette_Zoo_Destinazioni.Validita_Inizio Is Nothing OrElse Ricette_Zoo_Destinazioni.Validita_Inizio < AGRODATAINIZIO Then
				Ricette_Zoo_Destinazioni.Validita_Inizio = AGRODATAINIZIO
			End If

			If Ricette_Zoo_Destinazioni.Validita_Fine Is Nothing OrElse Ricette_Zoo_Destinazioni.Validita_Fine < AGRODATAFINE Then
				Ricette_Zoo_Destinazioni.Validita_Fine = AGRODATAFINE
			End If
		Catch ex As Exception
			messaggioErrore = ex.Message
			Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
			Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
		End Try
	End Sub

	Public Sub Elimina(ByRef Ricette_Zoo_Destinazioni As AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Destinazioni,
					   ByRef GiasContext As Gias_DeveloperServer_Entities,
					   ByRef objParametriServer As AgronicaCoreParametri)
		Const nomeRoutine = "AgronicaCoreContabDAL.Ricette_Zoo_Destinazioni_W.Elimina()"
		Dim messaggioErrore As String = ""

		Try
			GiasContext.Ricette_Zoo_Destinazioni.Attach(Ricette_Zoo_Destinazioni)
			GiasContext.Ricette_Zoo_Destinazioni.Remove(Ricette_Zoo_Destinazioni)

		Catch ex As Exception
			messaggioErrore = ex.Message
			Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
			Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
		End Try
	End Sub

#End Region

End Class