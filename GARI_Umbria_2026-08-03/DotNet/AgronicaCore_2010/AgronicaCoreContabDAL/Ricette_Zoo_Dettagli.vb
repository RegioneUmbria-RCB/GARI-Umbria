Imports System.Text
Imports System.Data.Entity
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreEntityFramework
Imports InData.Anagrafica
Imports System.Net.NetworkInformation

Public Class Ricette_Zoo_Dettagli_R
	Inherits AgronicaCoreDataProvider.DataProvider

	Public Function Leggi(ByVal Piva As String,
						  ByVal Sa_Cod As Integer,
						  ByVal Id_Ricetta As Integer,
						  ByVal Id_Agenda As Integer,
						  ByVal Id_Mov As Integer,
						  ByVal Id_Dettaglio As Integer,
						  ByVal RegScoNumero As String,
						  ByVal Elem_Cod As Integer,
						  ByVal Pro_Cod As Integer,
						  ByVal Mat_Cod As Integer,
						  ByVal Udm_Cod As Integer,
						  ByVal FlTipoMedicinale As String,
						  ByVal Aic As String,
						  ByVal xSelezioneVariabile As enumSelezioneVariabile,
						  ByVal xFiltroAggiuntivo As String,
						  ByVal xOrderBy As String,
						  ByRef objParametri As AgronicaCoreParametri) As DataTable
		Const nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Zoo_Dettagli_R.Leggi()"

		Dim messaggioErrore As String = ""
		Dim strSQL As New StringBuilder
		Dim dt As DataTable

		Try
			strSQL.Length = 0

			Select Case xSelezioneVariabile
				Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

					strSQL.AppendLine("SELECT * ").
						AppendLine("FROM Ricette_Zoo_Dettagli ").
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

					If Id_Dettaglio <> 0 Then
						strSQL.AppendLine("    AND IdDettaglio = " & Agro_SQL_SaveNum(Id_Dettaglio) & " ")
					End If

					If Piva <> "" Then
						strSQL.AppendLine("    AND Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
					End If

					If Sa_Cod <> 0 Then
						strSQL.AppendLine("    AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
					End If

					If RegScoNumero <> "" Then
						strSQL.AppendLine("    AND RegScoNumero = '" & Agro_SQL_SaveText(RegScoNumero) & "' ")
					End If

					If Elem_Cod <> 0 Then
						strSQL.AppendLine("    AND Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & " ")
					End If

					If Pro_Cod <> 0 Then
						strSQL.AppendLine("    AND Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & " ")
					End If

					If Mat_Cod <> 0 Then
						strSQL.AppendLine("    AND Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & " ")
					End If

					If Udm_Cod <> 0 Then
						strSQL.AppendLine("    AND Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & " ")
					End If

					If FlTipoMedicinale <> "" Then
						strSQL.AppendLine("    AND FlTipoMedicinale = '" & Agro_SQL_SaveText(FlTipoMedicinale) & "' ")
					End If

					If Aic <> "" Then
						strSQL.AppendLine("    AND Aic = '" & Agro_SQL_SaveText(Aic) & "' ")
					End If

					If xFiltroAggiuntivo <> "" Then
						strSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
					End If

					'--------------------------------------------------------------------------
					Select Case objParametri.FlagVisibilita
						Case enumVisibilita.Visibilita_SoloNonCancellati
							strSQL.AppendLine("    AND Ricette_Zoo_Dettagli.Inviato >= 0 ")
						Case enumVisibilita.Visibilita_SoloCancellati
							strSQL.AppendLine("    AND Ricette_Zoo_Dettagli.Inviato = -1 ")
						Case enumVisibilita.Visibilita_Tutti
							'...................................
						Case Else
							Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
					End Select
					'--------------------------------------------------------------------------

					If xOrderBy <> "" Then
						strSQL.AppendLine("ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
					Else
						strSQL.AppendLine("ORDER BY Ricette_Zoo_Dettagli.Piva, Ricette_Zoo_Dettagli.Sa_Cod, Ricette_Zoo_Dettagli.IdRicetta, Ricette_Zoo_Dettagli.IdAgenda, Ricette_Zoo_Dettagli.IdMov, Ricette_Zoo_Dettagli.IdDettaglio ASC ")
					End If

				Case enumSelezioneVariabile.Selezione_TabellaCompleta

					strSQL.AppendLine("SELECT * ").
						AppendLine("FROM Ricette_Zoo_Dettagli ").
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

					If Id_Dettaglio <> 0 Then
						strSQL.AppendLine("    AND IdDettaglio = " & Agro_SQL_SaveNum(Id_Dettaglio) & " ")
					End If

					If Piva <> "" Then
						strSQL.AppendLine("    AND Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
					End If

					If Sa_Cod <> 0 Then
						strSQL.AppendLine("    AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
					End If

					If RegScoNumero <> "" Then
						strSQL.AppendLine("    AND RegScoNumero = '" & Agro_SQL_SaveText(RegScoNumero) & "' ")
					End If

					If FlTipoMedicinale <> "" Then
						strSQL.AppendLine("    AND FlTipoMedicinale = '" & Agro_SQL_SaveText(FlTipoMedicinale) & "' ")
					End If

					If Aic <> "" Then
						strSQL.AppendLine("    AND Aic = '" & Agro_SQL_SaveText(Aic) & "' ")
					End If

					If xFiltroAggiuntivo <> "" Then
						strSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
					End If

					'--------------------------------------------------------------------------
					Select Case objParametri.FlagVisibilita
						Case enumVisibilita.Visibilita_SoloNonCancellati
							strSQL.AppendLine("    AND Ricette_Zoo_Dettagli.Inviato >= 0 ")
						Case enumVisibilita.Visibilita_SoloCancellati
							strSQL.AppendLine("    AND Ricette_Zoo_Dettagli.Inviato = -1 ")
						Case enumVisibilita.Visibilita_Tutti
							'...................................
						Case Else
							Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
					End Select
					'--------------------------------------------------------------------------

					If xOrderBy <> "" Then
						strSQL.AppendLine("ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
					Else
						strSQL.AppendLine("ORDER BY Ricette_Zoo_Dettagli.Piva, Ricette_Zoo_Dettagli.Sa_Cod, Ricette_Zoo_Dettagli.IdRicetta, Ricette_Zoo_Dettagli.IdAgenda, Ricette_Zoo_Dettagli.IdMov, Ricette_Zoo_Dettagli.IdDettaglio ASC ")
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

Public Class Ricette_Zoo_Dettagli_W
	Inherits AgronicaCoreDataProvider.DataProvider

#Region "Scrivi"

	Public Function Scrivi(ByVal Id_Ricetta As Integer,
						   ByVal Id_Agenda As Integer,
						   ByVal Id_Mov As Integer,
						   ByVal Id_Dettaglio As Integer,
						   ByVal Piva As String,
						   ByVal Sa_Cod As Integer,
						   ByVal Validita_Inizio As Date,
						   ByVal Validita_Fine As Date,
						   ByRef objParametri As AgronicaCoreParametri,
						   Optional ByVal RegScoNumero As String = "",
						   Optional ByVal Elem_Cod As Integer = 0,
						   Optional ByVal Pro_Cod As Integer = 0,
						   Optional ByVal Mat_Cod As Integer = 0,
						   Optional ByVal Udm_Cod As Integer = 0,
						   Optional ByVal FlAntimicrobico As Boolean = False,
						   Optional ByVal FlOrmonale As Boolean = False,
						   Optional ByVal FlTipoMed As String = "",
						   Optional ByVal FlVaccino As Boolean = False,
						   Optional ByVal MangimeComp As String = "",
						   Optional ByVal MangimeDen As String = "",
						   Optional ByVal Posologia As String = "",
						   Optional ByVal Aic As String = "",
						   Optional ByVal Quantitativo As Integer = 0) As Boolean
		Const nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Zoo_Dettagli_W.Scrivi()"

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

			If Piva = "" Then
				Throw New Exception("Piva non e' stata valorizzata.")
			End If

			If Sa_Cod = 0 Then
				Throw New Exception("Sa_Cod non e' stata valorizzato.")
			End If

			strSql.Length = 0

			strSql.AppendLine("INSERT INTO Ricette_Zoo_Dettagli ").
				AppendLine("    (IdRicetta, IdAgenda, IdMov, IdDettaglio, Piva, SaCod, ")

			strSql.AppendLine("    RegScoNumero, Elem_Cod, Pro_Cod, Mat_Cod, Udm_Cod, ").
				AppendLine("    FlAntimicrobico, FlOrmonale, FlTipoMedicinale, FlVaccino,").
				AppendLine("    MangimeComposizione, MangimeDenominazione, Posologia, ProdottoAic, Quantitativo, ")

			strSql.AppendLine("     Inviato, DataInvio, StrutturaCodice, StrutturaDenominazione, ProtocolloCodice, ").
				AppendLine("     Data_Creazione, Data_Modifica, ").
				AppendLine("     Username_Creazione, Username_Modifica, ").
				AppendLine("     Validita_Inizio, Validita_Fine) ")

			strSql.AppendLine("VALUES (").
				AppendLine("    " & Agro_SQL_SaveNum(Id_Ricetta) & ", ").
				AppendLine("    " & Agro_SQL_SaveNum(Id_Agenda) & ", ").
				AppendLine("    " & Agro_SQL_SaveNum(Id_Mov) & ", ").
				AppendLine("    " & Agro_SQL_SaveNum(Id_Dettaglio) & ", ").
				AppendLine("    '" & Agro_SQL_SaveText(Piva) & "', ").
				AppendLine("    " & Agro_SQL_SaveNum(Sa_Cod) & ", ")

			strSql.AppendLine("    '" & Agro_SQL_SaveText(RegScoNumero) & "', ").
				AppendLine("    " & Agro_SQL_SaveNum(Elem_Cod) & ", ").
				AppendLine("    " & Agro_SQL_SaveNum(Pro_Cod) & ", ").
				AppendLine("    " & Agro_SQL_SaveNum(Mat_Cod) & ", ").
				AppendLine("    " & Agro_SQL_SaveNum(Udm_Cod) & ", ").
				AppendLine("    " & Agro_SQL_SaveNum(FlAntimicrobico) & ", ").
				AppendLine("    " & Agro_SQL_SaveNum(FlOrmonale) & ", ").
				AppendLine("    '" & Agro_SQL_SaveText(FlTipoMed) & "', ").
				AppendLine("    " & Agro_SQL_SaveNum(FlVaccino) & ", ").
				AppendLine("    '" & Agro_SQL_SaveText(MangimeComp) & "', ").
				AppendLine("    '" & Agro_SQL_SaveText(MangimeDen) & "', ").
				AppendLine("    '" & Agro_SQL_SaveText(Posologia) & "', ").
				AppendLine("    '" & Agro_SQL_SaveText(Aic) & "', ").
				AppendLine("    " & Agro_SQL_SaveNum(Quantitativo) & ", ")

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
							 ByVal Piva As String,
							 ByVal Sa_Cod As Integer,
							 ByVal xFiltroAggiuntivo As String,
							 ByRef objParametri As AgronicaCoreParametri,
							 Optional ByVal RegScoNumero As String = Nothing,
							 Optional ByVal Elem_Cod As Integer? = Nothing,
							 Optional ByVal Pro_Cod As Integer? = Nothing,
							 Optional ByVal Mat_Cod As Integer? = Nothing,
							 Optional ByVal Udm_Cod As Integer? = Nothing,
							 Optional ByVal FlAntimicrobico As Boolean? = Nothing,
							 Optional ByVal FlOrmonale As Boolean? = Nothing,
							 Optional ByVal FlTipoMed As String = Nothing,
							 Optional ByVal FlVaccino As Boolean? = Nothing,
							 Optional ByVal MangimeComp As String = Nothing,
							 Optional ByVal MangimeDen As String = Nothing,
							 Optional ByVal Posologia As String = Nothing,
							 Optional ByVal Aic As String = Nothing,
							 Optional ByVal Quantitativo As Integer? = Nothing,
							 Optional ByVal Validita_Inizio As Date? = Nothing,
							 Optional ByVal Validita_Fine As Date? = Nothing) As Boolean
		Const nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Zoo_Dettagli_W.Modifica()"

		Dim messaggioErrore As String = ""
		Dim strSql As New StringBuilder
		Dim xRisp As Boolean = False

		Try
			If Id_Ricetta = 0 Then
				Throw New Exception("Parametro non corretto nella query (Ricetta_Id obbligatorio)")
			End If

			If Id_Agenda = 0 Then
				Throw New Exception("Parametro non corretto nella query (Riga_Id obbligatorio)")
			End If

			If Id_Dettaglio = 0 Then
				Throw New Exception("Parametro non corretto nella query (Dettaglio_Id obbligatorio)")
			End If

			If Piva = "" Then
				Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
			End If

			strSql.Length = 0

			strSql.AppendLine("UPDATE Ricette_Zoo_Dettagli ").
				AppendLine("SET Data_Modifica = " & Agro_SQL_SaveDateTime(Date.Now) & ", ").
				AppendLine("    Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "', ")

			If Not IsNothing(RegScoNumero) Then
				strSql.AppendLine("    RegScoNumero = '" & Agro_SQL_SaveText(RegScoNumero) & "', ")
			End If

			If Not IsNothing(Elem_Cod) Then
				strSql.AppendLine("    Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & ", ")
			End If

			If Not IsNothing(Pro_Cod) Then
				strSql.AppendLine("    Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & ", ")
			End If

			If Not IsNothing(Mat_Cod) Then
				strSql.AppendLine("    Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & ", ")
			End If

			If Not IsNothing(Udm_Cod) Then
				strSql.AppendLine("    Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & ", ")
			End If

			If Not IsNothing(FlAntimicrobico) Then
				strSql.AppendLine("    FlAntimicrobico = " & Agro_SQL_SaveNum(FlAntimicrobico) & ", ")
			End If

			If Not IsNothing(FlOrmonale) Then
				strSql.AppendLine("    FlOrmonale = " & Agro_SQL_SaveNum(FlOrmonale) & ", ")
			End If

			If Not IsNothing(FlTipoMed) Then
				strSql.AppendLine("    FlTipoMedicinale = '" & Agro_SQL_SaveText(FlTipoMed) & "', ")
			End If

			If Not IsNothing(FlVaccino) Then
				strSql.AppendLine("    FlVaccino = " & Agro_SQL_SaveNum(FlVaccino) & ", ")
			End If

			If Not IsNothing(MangimeComp) Then
				strSql.AppendLine("    MangimeComposizione = '" & Agro_SQL_SaveText(MangimeComp) & "', ")
			End If

			If Not IsNothing(MangimeDen) Then
				strSql.AppendLine("    MangimeDenominazione = '" & Agro_SQL_SaveText(MangimeDen) & "', ")
			End If

			If Not IsNothing(Posologia) Then
				strSql.AppendLine("    Posologia = '" & Agro_SQL_SaveText(Posologia) & "', ")
			End If

			If Not IsNothing(Aic) Then
				strSql.AppendLine("    ProdottoAic = '" & Agro_SQL_SaveText(Aic) & "', ")
			End If

			If Not IsNothing(Quantitativo) Then
				strSql.AppendLine("    Quantitativo = " & Agro_SQL_SaveNum(Quantitativo) & ", ")
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
				AppendLine("    AND IdMov = " & Agro_SQL_SaveNum(Id_Mov) & " ").
				AppendLine("    AND IdDettaglio = " & Agro_SQL_SaveNum(Id_Dettaglio) & " ")

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
							 ByVal Id_Dettaglio As Integer,
							 ByVal Piva As String,
							 ByVal Sa_Cod As Integer,
							 ByVal xFiltroAggiuntivo As String,
							 ByRef objParametri As AgronicaCoreParametri) As Boolean
		Const nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Zoo_Dettagli_W.Cancella()"

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

			If Id_Mov = 0 Then
				Throw New Exception("Parametro non corretto nella query (IdMov obbligatorio)")
			End If

			If Id_Dettaglio = 0 Then
				Throw New Exception("Parametro non corretto nella query (IdDettaglio obbligatorio)")
			End If

			'---------------------------------------------
			strSql.Length = 0

			If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
				strSql.AppendLine("UPDATE Ricette_Zoo_Dettagli ").
					AppendLine("SET ").
					AppendLine("    Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "', ").
					AppendLine("    Inviato = -1 ").
					AppendLine("WHERE Inviato >= 0 ")
			Else
				strSql.AppendLine("DELETE ").
					AppendLine("FROM Ricette_Zoo_Dettagli ").
					AppendLine("WHERE  1=1 ")
			End If

			strSql.AppendLine("    AND IdRicetta = " & Agro_SQL_SaveNum(Id_Ricetta) & " ").
				AppendLine("    AND IdAgenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ").
				AppendLine("    AND IdMov = " & Agro_SQL_SaveNum(Id_Mov) & " ").
				AppendLine("    AND IdDettaglio = " & Agro_SQL_SaveNum(Id_Dettaglio) & " ")

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

	Public Sub Scrivi(ByRef Ricette_Zoo_Dettagli As AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Dettagli,
					  ByRef GiasContext As Gias_DeveloperServer_Entities,
					  ByRef objParametriServer As AgronicaCoreParametri)
		Const nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Zoo_Dettagli_W.Scrivi()"
		Dim messaggioErrore As String = ""

		Try
			Valorizza(Ricette_Zoo_Dettagli, objParametriServer)

			Ricette_Zoo_Dettagli.Data_Creazione = DateTime.Now
			Ricette_Zoo_Dettagli.Data_Modifica = DateTime.Now
			Ricette_Zoo_Dettagli.Username_Creazione = objParametriServer.UsernameOperazione
			Ricette_Zoo_Dettagli.Username_Modifica = objParametriServer.UsernameOperazione

			GiasContext.Ricette_Zoo_Dettagli.Add(Ricette_Zoo_Dettagli)
		Catch ex As Exception
			messaggioErrore = ex.Message
			Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
			Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
		End Try
	End Sub

	Public Sub Modifica(ByRef Ricette_Zoo_Dettagli As AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Dettagli,
						ByRef GiasContext As Gias_DeveloperServer_Entities,
						ByRef objParametriServer As AgronicaCoreParametri)

		Const nomeRoutine = "AgronicaCoreContabDAL.Ricette_Zoo_Dettagli_W.Modifica()"
		Dim messaggioErrore As String = ""

		Try
			Valorizza(Ricette_Zoo_Dettagli, objParametriServer)

			Ricette_Zoo_Dettagli.Data_Modifica = DateTime.Now
			Ricette_Zoo_Dettagli.Username_Modifica = objParametriServer.UsernameOperazione

			GiasContext.Entry(Ricette_Zoo_Dettagli).State = EntityState.Modified
		Catch ex As Exception
			messaggioErrore = ex.Message
			Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
			Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
		End Try
	End Sub

	Public Sub Valorizza(ByRef Ricette_Zoo_Dettagli As AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Dettagli,
						 ByRef objParametriServer As AgronicaCoreParametri)
		Const nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Zoo_Dettagli_W.Valorizza()"
		Dim messaggioErrore As String = ""

		Try
			If Ricette_Zoo_Dettagli.RegScoNumero Is Nothing Then
				Ricette_Zoo_Dettagli.RegScoNumero = ""
			End If

			If IsNothing(Ricette_Zoo_Dettagli.Elem_Cod) Then
				Ricette_Zoo_Dettagli.Elem_Cod = 0
			End If

			If IsNothing(Ricette_Zoo_Dettagli.Pro_Cod) Then
				Ricette_Zoo_Dettagli.Pro_Cod = 0
			End If

			If IsNothing(Ricette_Zoo_Dettagli.Mat_Cod) Then
				Ricette_Zoo_Dettagli.Mat_Cod = 0
			End If

			If IsNothing(Ricette_Zoo_Dettagli.Udm_Cod) Then
				Ricette_Zoo_Dettagli.Udm_Cod = 0
			End If

			If Ricette_Zoo_Dettagli.FlAntimicrobico Is Nothing Then
				Ricette_Zoo_Dettagli.FlAntimicrobico = False
			End If

			If Ricette_Zoo_Dettagli.FlOrmonale Is Nothing Then
				Ricette_Zoo_Dettagli.FlOrmonale = False
			End If

			If Ricette_Zoo_Dettagli.FlTipoMedicinale Is Nothing Then
				Ricette_Zoo_Dettagli.FlTipoMedicinale = ""
			End If

			If Ricette_Zoo_Dettagli.FlVaccino Is Nothing Then
				Ricette_Zoo_Dettagli.FlVaccino = False
			End If

			If Ricette_Zoo_Dettagli.MangimeComposizione Is Nothing Then
				Ricette_Zoo_Dettagli.MangimeComposizione = ""
			End If

			If Ricette_Zoo_Dettagli.MangimeDenominazione Is Nothing Then
				Ricette_Zoo_Dettagli.MangimeDenominazione = ""
			End If

			If Ricette_Zoo_Dettagli.Posologia Is Nothing Then
				Ricette_Zoo_Dettagli.Posologia = ""
			End If

			If Ricette_Zoo_Dettagli.ProdottoAic Is Nothing Then
				Ricette_Zoo_Dettagli.ProdottoAic = ""
			End If

			If Ricette_Zoo_Dettagli.Quantitativo Is Nothing Then
				Ricette_Zoo_Dettagli.Quantitativo = 0
			End If

			If Ricette_Zoo_Dettagli.Inviato Is Nothing Then
				Ricette_Zoo_Dettagli.Inviato = 0
			End If

			If Ricette_Zoo_Dettagli.Data_Creazione Is Nothing Then
				Ricette_Zoo_Dettagli.Data_Creazione = DateTime.Now
			End If

			If Ricette_Zoo_Dettagli.Data_Modifica Is Nothing Then
				Ricette_Zoo_Dettagli.Data_Modifica = DateTime.Now
			End If

			If Ricette_Zoo_Dettagli.Username_Creazione Is Nothing Then
				Ricette_Zoo_Dettagli.Username_Creazione = objParametriServer.UsernameOperazione
			End If

			If Ricette_Zoo_Dettagli.Username_Modifica Is Nothing Then
				Ricette_Zoo_Dettagli.Username_Modifica = objParametriServer.UsernameOperazione
			End If

			If Ricette_Zoo_Dettagli.Validita_Inizio Is Nothing OrElse Ricette_Zoo_Dettagli.Validita_Inizio < AGRODATAINIZIO Then
				Ricette_Zoo_Dettagli.Validita_Inizio = AGRODATAINIZIO
			End If

			If Ricette_Zoo_Dettagli.Validita_Fine Is Nothing OrElse Ricette_Zoo_Dettagli.Validita_Fine < AGRODATAFINE Then
				Ricette_Zoo_Dettagli.Validita_Fine = AGRODATAFINE
			End If
		Catch ex As Exception
			messaggioErrore = ex.Message
			Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
			Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
		End Try
	End Sub

	Public Sub Elimina(ByRef Ricette_Zoo_Dettagli As AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Dettagli,
					   ByRef GiasContext As Gias_DeveloperServer_Entities,
					   ByRef objParametriServer As AgronicaCoreParametri)
		Const nomeRoutine = "AgronicaCoreContabDAL.Ricette_Zoo_Dettagli_W.Elimina()"
		Dim messaggioErrore As String = ""

		Try
			GiasContext.Ricette_Zoo_Dettagli.Attach(Ricette_Zoo_Dettagli)
			GiasContext.Ricette_Zoo_Dettagli.Remove(Ricette_Zoo_Dettagli)

		Catch ex As Exception
			messaggioErrore = ex.Message
			Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
			Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
		End Try
	End Sub

#End Region

End Class
