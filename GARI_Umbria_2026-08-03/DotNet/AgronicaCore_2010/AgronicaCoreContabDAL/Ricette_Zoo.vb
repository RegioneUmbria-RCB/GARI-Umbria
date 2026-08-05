Imports System.Text
Imports System.Data.Entity
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreEntityFramework

Public Class Ricette_Zoo_R
	Inherits AgronicaCoreDataProvider.DataProvider

	Public Function Leggi(ByVal Id_Ricetta As Integer,
						  ByVal Piva As String,
						  ByVal Sa_Cod As Integer,
						  ByVal Sta_Num As Integer,
						  ByVal Ricetta_Numero As String,
						  ByVal Detentore_IdFiscale As String,
						  ByVal Proprietario_IdFiscale As String,
						  ByVal Veterinario_IdFiscale As String,
						  ByVal Stato As Integer,
						  ByVal Tipo As Integer,
						  ByVal Data_Emissione As DateTime,
						  ByVal StrutturaCodice As String,
						  ByVal ProtocolloCodice As String,
						  ByVal xSelezioneVariabile As enumSelezioneVariabile,
						  ByVal xFiltroAggiuntivo As String,
						  ByVal xOrderBy As String,
						  ByRef objParametri As AgronicaCoreParametri) As DataTable
		Const nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Zoo_R.Leggi()"

		Dim messaggioErrore As String = ""
		Dim strSQL As New StringBuilder
		Dim dt As DataTable

		Try
			strSQL.Length = 0

			Select Case xSelezioneVariabile
				Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

					strSQL.AppendLine("SELECT * ").
						AppendLine("FROM Ricette_Zoo ").
						AppendLine("WHERE Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ").
						AppendLine("    AND Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

					If Id_Ricetta <> 0 Then
						strSQL.AppendLine("    AND IdRicetta = " & Agro_SQL_SaveNum(Id_Ricetta) & " ")
					End If

					If Piva <> "" Then
						strSQL.AppendLine("    AND Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
					End If

					If Sa_Cod <> 0 Then
						strSQL.AppendLine("    AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
					End If

					If Sta_Num <> 0 Then
						strSQL.AppendLine("    AND Sta_Num = " & Agro_SQL_SaveNum(Sta_Num) & " ")
					End If

					If Ricetta_Numero <> "" Then
						strSQL.AppendLine("    AND Numero = '" & Agro_SQL_SaveText(Ricetta_Numero) & "' ")
					End If

					If Detentore_IdFiscale <> "" Then
						strSQL.AppendLine("    AND DetentoreIdFiscale = '" & Agro_SQL_SaveText(Detentore_IdFiscale) & "' ")
					End If

					If Proprietario_IdFiscale <> "" Then
						strSQL.AppendLine("    AND ProprietarioIdFiscale = '" & Agro_SQL_SaveText(Proprietario_IdFiscale) & "' ")
					End If

					If Veterinario_IdFiscale <> "" Then
						strSQL.AppendLine("    AND VeterinarioIdFiscale = '" & Agro_SQL_SaveText(Veterinario_IdFiscale) & "' ")
					End If

					If Stato <> 0 Then
						strSQL.AppendLine("    AND StatoCodice = " & Agro_SQL_SaveNum(Stato) & " ")
					End If

					If Tipo <> 0 Then
						strSQL.AppendLine("    AND TipoCodice = " & Agro_SQL_SaveNum(Tipo) & " ")
					End If

					If Data_Emissione <> AGRODATAINIZIO Then
						strSQL.AppendLine("    AND DataEmissione = " & Agro_SQL_SaveDate(Data_Emissione) & " ")
					End If

					If StrutturaCodice <> "" Then
						strSQL.AppendLine("    AND StrutturaCodice = '" & Agro_SQL_SaveText(StrutturaCodice) & "' ")
					End If

					If ProtocolloCodice <> "" Then
						strSQL.AppendLine("    AND ProtocolloCodice = '" & Agro_SQL_SaveText(ProtocolloCodice) & "' ")
					End If

					If xFiltroAggiuntivo <> "" Then
						strSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
					End If

					'--------------------------------------------------------------------------
					Select Case objParametri.FlagVisibilita
						Case enumVisibilita.Visibilita_SoloNonCancellati
							strSQL.AppendLine("    AND Ricette_Zoo.Inviato >= 0 ")
						Case enumVisibilita.Visibilita_SoloCancellati
							strSQL.AppendLine("    AND Ricette_Zoo.Inviato = -1 ")
						Case enumVisibilita.Visibilita_Tutti
							'...................................
						Case Else
							Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
					End Select
					'--------------------------------------------------------------------------

					If xOrderBy <> "" Then
						strSQL.AppendLine("ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
					Else
						strSQL.AppendLine("ORDER BY Ricette_Zoo.Piva, Ricette_Zoo.Sa_Cod, Ricette_Zoo.Sta_Num, Ricette_Zoo.IdRicetta ASC ")
					End If

				Case enumSelezioneVariabile.Selezione_TabellaCompleta

					strSQL.AppendLine("SELECT * ").
						AppendLine("FROM Ricette_Zoo ").
						AppendLine("WHERE Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ").
						AppendLine("    AND Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

					If Id_Ricetta <> 0 Then
						strSQL.AppendLine("    AND IdRicetta = " & Agro_SQL_SaveNum(Id_Ricetta) & " ")
					End If

					If Piva <> "" Then
						strSQL.AppendLine("    AND Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
					End If

					If Sa_Cod <> 0 Then
						strSQL.AppendLine("    AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
					End If

					If Sta_Num <> 0 Then
						strSQL.AppendLine("    AND Sta_Num = " & Agro_SQL_SaveNum(Sta_Num) & " ")
					End If

					If Ricetta_Numero <> "" Then
						strSQL.AppendLine("    AND Numero = '" & Agro_SQL_SaveText(Ricetta_Numero) & "' ")
					End If

					If Detentore_IdFiscale <> "" Then
						strSQL.AppendLine("    AND DetentoreIdFiscale = '" & Agro_SQL_SaveText(Detentore_IdFiscale) & "' ")
					End If

					If Proprietario_IdFiscale <> "" Then
						strSQL.AppendLine("    AND ProprietarioIdFiscale = '" & Agro_SQL_SaveText(Proprietario_IdFiscale) & "' ")
					End If

					If Veterinario_IdFiscale <> "" Then
						strSQL.AppendLine("    AND VeterinarioIdFiscale = '" & Agro_SQL_SaveText(Veterinario_IdFiscale) & "' ")
					End If

					If Stato <> 0 Then
						strSQL.AppendLine("    AND StatoCodice = " & Agro_SQL_SaveNum(Stato) & " ")
					End If

					If Tipo <> 0 Then
						strSQL.AppendLine("    AND TipoCodice = " & Agro_SQL_SaveNum(Tipo) & " ")
					End If

					If Data_Emissione <> AGRODATAINIZIO Then
						strSQL.AppendLine("    AND DataEmissione = " & Agro_SQL_SaveDate(Data_Emissione) & " ")
					End If

					If StrutturaCodice <> "" Then
						strSQL.AppendLine("    AND StrutturaCodice = '" & Agro_SQL_SaveText(StrutturaCodice) & "' ")
					End If

					If ProtocolloCodice <> "" Then
						strSQL.AppendLine("    AND ProtocolloCodice = '" & Agro_SQL_SaveText(ProtocolloCodice) & "' ")
					End If

					If xFiltroAggiuntivo <> "" Then
						strSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
					End If

					'--------------------------------------------------------------------------
					Select Case objParametri.FlagVisibilita
						Case enumVisibilita.Visibilita_SoloNonCancellati
							strSQL.AppendLine("    AND Ricette_Zoo.Inviato >= 0 ")
						Case enumVisibilita.Visibilita_SoloCancellati
							strSQL.AppendLine("    AND Ricette_Zoo.Inviato = -1 ")
						Case enumVisibilita.Visibilita_Tutti
							'...................................
						Case Else
							Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
					End Select
					'--------------------------------------------------------------------------

					If xOrderBy <> "" Then
						strSQL.AppendLine("ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
					Else
						strSQL.AppendLine("ORDER BY Ricette_Zoo.Piva, Ricette_Zoo.Sa_Cod, Ricette_Zoo.Sta_Num, Ricette_Zoo.IdRicetta ASC ")
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

Public Class Ricette_Zoo_W
	Inherits AgronicaCoreDataProvider.DataProvider

#Region "Scrittura"

	Public Function Scrivi(ByVal Id_Ricetta As Integer,
						   ByVal Piva As String,
						   ByVal Sa_Cod As Integer,
						   ByVal Sta_Num As Integer,
						   ByVal Validita_Inizio As Date,
						   ByVal Validita_Fine As Date,
						   ByRef objParametri As AgronicaCoreParametri,
						   Optional ByVal Ricetta_Numero As String = "",
						   Optional ByVal Detentore_IdFiscale As String = "",
						   Optional ByVal Proprietario_IdFiscale As String = "",
						   Optional ByVal Veterinario_IdFiscale As String = "",
						   Optional ByVal Stato As Integer = 0,
						   Optional ByVal Tipo As Integer = 0,
						   Optional ByVal Data_Emissione As DateTime = AGRODATAINIZIO,
						   Optional ByVal StrutturaCod As String = "",
						   Optional ByVal StrutturaDes As String = "",
						   Optional ByVal ProtocolloCod As String = "",
						   Optional ByVal Note As String = "",
						   Optional ByVal Pin As String = "",
						   Optional ByVal Riga_Cardinalita As Integer = 0) As Boolean
		Const nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Zoo_W.Scrivi()"

		Dim messaggioErrore As String = ""
		Dim strSql As New StringBuilder
		Dim xRisp As Boolean = False

		Try
			If Id_Ricetta = 0 Then
				Throw New Exception("IdRicetta non e' stata valorizzato.")
			End If

			If Piva = "" Then
				Throw New Exception("Piva non e' stata valorizzata.")
			End If

			If Sa_Cod = 0 Then
				Throw New Exception("Sa_Cod non e' stata valorizzato.")
			End If

			If Sta_Num = 0 Then
				Throw New Exception("Sta_Num non e' stata valorizzato.")
			End If

			strSql.Length = 0

			strSql.AppendLine("INSERT INTO Ricette_Zoo ").
				AppendLine("    (IdRicetta, Piva, SaCod, Sta_Num, ")

			strSql.AppendLine("     Numero, DetentoreIdFiscale, ProprietarioIdFiscale, VeterinarioIdFiscale, DataEmissione, ").
				AppendLine("     StatoCodice, TipoCodice, StrutturaCodice, StrutturaDenominazione, ProtocolloCodice, ").
				AppendLine("     Note, Pin, RigaCardinalita, ").
				AppendLine("     Blocco_Flag, Blocco_Data, Blocco_Username, ")

			strSql.AppendLine("     Inviato, DataInvio, StrutturaCodice, StrutturaDenominazione, ProtocolloCodice, ").
				AppendLine("     Data_Creazione, Data_Modifica, ").
				AppendLine("     Username_Creazione, Username_Modifica, ").
				AppendLine("     Validita_Inizio, Validita_Fine) ")

			strSql.AppendLine("VALUES (").
				AppendLine("    " & Agro_SQL_SaveNum(Id_Ricetta) & ", ").
				AppendLine("    '" & Agro_SQL_SaveText(Piva) & "', ").
				AppendLine("    " & Agro_SQL_SaveNum(Sa_Cod) & ", ").
				AppendLine("    " & Agro_SQL_SaveNum(Sta_Num) & ", ")

			strSql.AppendLine("    '" & Agro_SQL_SaveText(Ricetta_Numero) & "', ").
				AppendLine("    '" & Agro_SQL_SaveText(Detentore_IdFiscale) & "', ").
				AppendLine("    '" & Agro_SQL_SaveText(Proprietario_IdFiscale) & "', ").
				AppendLine("    '" & Agro_SQL_SaveText(Veterinario_IdFiscale) & "', ").
				AppendLine("    " & Agro_SQL_SaveDateTime(Data_Emissione) & ", ")

			strSql.AppendLine("    " & Agro_SQL_SaveNum(Stato) & ", ").
				AppendLine("    " & Agro_SQL_SaveNum(Tipo) & ", ").
				AppendLine("    '" & Agro_SQL_SaveText(StrutturaCod) & "', ").
				AppendLine("    '" & Agro_SQL_SaveText(StrutturaDes) & "', ").
				AppendLine("    '" & Agro_SQL_SaveText(ProtocolloCod) & "', ").
				AppendLine("    '" & Agro_SQL_SaveText(Note) & "', ").
				AppendLine("    '" & Agro_SQL_SaveText(Pin) & "', ").
				AppendLine("    " & Agro_SQL_SaveNum(Riga_Cardinalita) & ", ").
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
							 ByVal Piva As String,
							 ByVal Sa_Cod As Integer,
							 ByVal Sta_Num As Integer,
							 ByVal xFiltroAggiuntivo As String,
							 ByRef objParametri As AgronicaCoreParametri,
							 Optional ByVal Ricetta_Numero As String = Nothing,
							 Optional ByVal Detentore_IdFiscale As String = Nothing,
							 Optional ByVal Proprietario_IdFiscale As String = Nothing,
							 Optional ByVal Veterinario_IdFiscale As String = Nothing,
							 Optional ByVal Stato As Integer? = Nothing,
							 Optional ByVal Tipo As Integer? = Nothing,
							 Optional ByVal Data_Emissione As DateTime? = Nothing,
							 Optional ByVal StrutturaCod As String = Nothing,
							 Optional ByVal StrutturaDen As String = Nothing,
							 Optional ByVal ProtocolloCod As String = Nothing,
							 Optional ByVal Validita_Inizio As Date? = Nothing,
							 Optional ByVal Validita_Fine As Date? = Nothing,
							 Optional ByVal Note As String = Nothing,
							 Optional ByVal Pin As String = Nothing,
							 Optional ByVal Riga_Cardinalita As Integer? = Nothing) As Boolean
		Const nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Zoo_W.Modifica()"

		Dim messaggioErrore As String = ""
		Dim strSql As New StringBuilder
		Dim xRisp As Boolean = False

		Try
			If Id_Ricetta = 0 Then
				Throw New Exception("Parametro non corretto nella query (IdRicetta obbligatorio)")
			End If

			If Piva = "" Then
				Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
			End If

			strSql.Length = 0

			strSql.AppendLine("UPDATE Ricette_Zoo ").
				AppendLine("SET Data_Modifica = " & Agro_SQL_SaveDateTime(Date.Now) & ", ").
				AppendLine("    Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "', ")

			If Not IsNothing(Ricetta_Numero) Then
				strSql.AppendLine("    Numero = '" & Agro_SQL_SaveText(Ricetta_Numero) & "', ")
			End If

			If Not IsNothing(Detentore_IdFiscale) Then
				strSql.AppendLine("    DetentoreIdFiscale = '" & Agro_SQL_SaveText(Detentore_IdFiscale) & "', ")
			End If

			If Not IsNothing(Proprietario_IdFiscale) Then
				strSql.AppendLine("    ProprietarioIdFiscale = '" & Agro_SQL_SaveText(Proprietario_IdFiscale) & "', ")
			End If

			If Not IsNothing(Veterinario_IdFiscale) Then
				strSql.AppendLine("    VeterinarioIdFiscale = '" & Agro_SQL_SaveText(Veterinario_IdFiscale) & "', ")
			End If

			If Not IsNothing(Stato) Then
				strSql.AppendLine("    StatoCodice = " & Agro_SQL_SaveNum(Stato) & ", ")
			End If

			If Not IsNothing(Tipo) Then
				strSql.AppendLine("    TipoCodice = " & Agro_SQL_SaveNum(Tipo) & ", ")
			End If

			If Not IsNothing(Data_Emissione) Then
				strSql.AppendLine("    DataEmissione = " & Agro_SQL_SaveDateTime(Data_Emissione) & ", ")
			End If

			If Not IsNothing(StrutturaCod) Then
				strSql.AppendLine("    StrutturaCodice = '" & Agro_SQL_SaveText(StrutturaCod) & "', ")
			End If

			If Not IsNothing(StrutturaDen) Then
				strSql.AppendLine("    StrutturaDenominazione = '" & Agro_SQL_SaveText(StrutturaDen) & "', ")
			End If

			If Not IsNothing(ProtocolloCod) Then
				strSql.AppendLine("    ProtocolloCodice = '" & Agro_SQL_SaveText(ProtocolloCod) & "', ")
			End If

			If Not IsNothing(Validita_Inizio) Then
				strSql.AppendLine("    Validita_Inizio = " & Agro_SQL_SaveDate(Validita_Inizio) & ", ")
			End If

			If Not IsNothing(Validita_Fine) Then
				strSql.AppendLine("    Validita_Fine = " & Agro_SQL_SaveDate(Validita_Fine) & ", ")
			End If

			If Not IsNothing(Note) Then
				strSql.AppendLine("    Note = '" & Agro_SQL_SaveText(Note) & "', ")
			End If

			If Not IsNothing(Pin) Then
				strSql.AppendLine("    Pin = '" & Agro_SQL_SaveText(Pin) & "', ")
			End If

			If Not IsNothing(Riga_Cardinalita) Then
				strSql.AppendLine("    RigaCardinalita = " & Agro_SQL_SaveNum(Riga_Cardinalita) & ", ")
			End If

			strSql.AppendLine("WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ").
				AppendLine("    AND IdRicetta = " & Agro_SQL_SaveNum(Id_Ricetta) & " ")

			If Sa_Cod <> 0 Then
				strSql.AppendLine("    AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
			End If

			If Sta_Num <> 0 Then
				strSql.AppendLine("    AND Sta_Num = " & Agro_SQL_SaveNum(Sta_Num) & " ")
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
							 ByVal Piva As String,
							 ByVal Sa_Cod As Integer,
							 ByVal Sta_Num As Integer,
							 ByVal xFiltroAggiuntivo As String,
							 ByRef objParametri As AgronicaCoreParametri) As Boolean
		Const nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Zoo_W.Cancella()"

		'====================================================================================
		'Parametri opzionali :
		'   Piva = ""
		'   Sa_Cod = 0
		'   Sta_Num = 0
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
				strSql.AppendLine("UPDATE Ricette_Zoo ").
					AppendLine("SET ").
					AppendLine("    Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "', ").
					AppendLine("    Inviato = -1 ").
					AppendLine("WHERE Inviato >= 0 ")
			Else
				strSql.AppendLine("DELETE ").
					AppendLine("FROM Ricette_Zoo ").
					AppendLine("WHERE  1=1 ")
			End If

			strSql.AppendLine("    AND IdRicetta = " & Agro_SQL_SaveNum(Id_Ricetta) & " ")

			If Piva <> "" Then
				strSql.AppendLine("    AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
			End If

			If Sa_Cod <> 0 Then
				strSql.AppendLine("    AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
			End If

			If Sta_Num <> 0 Then
				strSql.AppendLine("    AND Sta_Num = " & Agro_SQL_SaveNum(Sta_Num) & " ")
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

	Public Sub Scrivi(ByRef Ricette_Zoo As AgronicaCoreEntityFramework_POCO.Ricette_Zoo,
					  ByRef GiasContext As Gias_DeveloperServer_Entities,
					  ByRef objParametriServer As AgronicaCoreParametri)
		Const nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Zoo_W.Scrivi()"
		Dim messaggioErrore As String = ""

		Try
			Valorizza(Ricette_Zoo, objParametriServer)

			Ricette_Zoo.Data_Creazione = DateTime.Now
			Ricette_Zoo.Data_Modifica = DateTime.Now
			Ricette_Zoo.Username_Creazione = objParametriServer.UsernameOperazione
			Ricette_Zoo.Username_Modifica = objParametriServer.UsernameOperazione

			GiasContext.Ricette_Zoo.Add(Ricette_Zoo)
		Catch ex As Exception
			messaggioErrore = ex.Message
			Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
			Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
		End Try
	End Sub

	Public Sub Modifica(ByRef Ricette_Zoo As AgronicaCoreEntityFramework_POCO.Ricette_Zoo,
						ByRef GiasContext As Gias_DeveloperServer_Entities,
						ByRef objParametriServer As AgronicaCoreParametri)

		Const nomeRoutine = "AgronicaCoreContabDAL.Ricette_Zoo_W.Modifica()"
		Dim messaggioErrore As String = ""

		Try
			Valorizza(Ricette_Zoo, objParametriServer)

			Ricette_Zoo.Data_Modifica = DateTime.Now
			Ricette_Zoo.Username_Modifica = objParametriServer.UsernameOperazione

			GiasContext.Entry(Ricette_Zoo).State = EntityState.Modified
		Catch ex As Exception
			messaggioErrore = ex.Message
			Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
			Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
		End Try
	End Sub

	Public Sub Valorizza(ByRef Ricette_Zoo As AgronicaCoreEntityFramework_POCO.Ricette_Zoo,
						 ByRef objParametriServer As AgronicaCoreParametri)
		Const nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Zoo_W.Valorizza()"
		Dim messaggioErrore As String = ""

		Try
			If Ricette_Zoo.DetentoreIdFiscale Is Nothing Then
				Ricette_Zoo.DetentoreIdFiscale = ""
			End If

			If Ricette_Zoo.DataEmissione < AGRODATAINIZIO Then
				Ricette_Zoo.DataEmissione = AGRODATAINIZIO
			End If

			If Ricette_Zoo.Note Is Nothing Then
				Ricette_Zoo.Note = ""
			End If

			If Ricette_Zoo.Numero Is Nothing Then
				Ricette_Zoo.Numero = ""
			End If

			If Ricette_Zoo.Pin Is Nothing Then
				Ricette_Zoo.Pin = ""
			End If

			If Ricette_Zoo.ProprietarioIdFiscale Is Nothing Then
				Ricette_Zoo.ProprietarioIdFiscale = ""
			End If

			If Ricette_Zoo.StatoCodice Is Nothing Then
				Ricette_Zoo.StatoCodice = 0
			End If

			If Ricette_Zoo.StrutturaCodice Is Nothing Then
				Ricette_Zoo.StrutturaCodice = ""
			End If

			If Ricette_Zoo.StrutturaDenominazione Is Nothing Then
				Ricette_Zoo.StrutturaDenominazione = ""
			End If

			If Ricette_Zoo.TipoCodice Is Nothing Then
				Ricette_Zoo.TipoCodice = 0
			End If

			If Ricette_Zoo.VeterinarioIdFiscale Is Nothing Then
				Ricette_Zoo.VeterinarioIdFiscale = ""
			End If

			If Ricette_Zoo.RigaCardinalita Is Nothing Then
				Ricette_Zoo.RigaCardinalita = 0
			End If

			If Ricette_Zoo.ProtocolloCodice Is Nothing Then
				Ricette_Zoo.ProtocolloCodice = ""
			End If

			If Ricette_Zoo.Blocco_Flag Is Nothing Then
				Ricette_Zoo.Blocco_Flag = 0
			End If

			If Ricette_Zoo.Blocco_Username Is Nothing Then
				Ricette_Zoo.Blocco_Username = ""
			End If

			If Ricette_Zoo.Inviato Is Nothing Then
				Ricette_Zoo.Inviato = 0
			End If

			If Ricette_Zoo.Data_Creazione Is Nothing Then
				Ricette_Zoo.Data_Creazione = DateTime.Now
			End If

			If Ricette_Zoo.Data_Modifica Is Nothing Then
				Ricette_Zoo.Data_Modifica = DateTime.Now
			End If

			If Ricette_Zoo.Username_Creazione Is Nothing Then
				Ricette_Zoo.Username_Creazione = objParametriServer.UsernameOperazione
			End If

			If Ricette_Zoo.Username_Modifica Is Nothing Then
				Ricette_Zoo.Username_Modifica = objParametriServer.UsernameOperazione
			End If

			If Ricette_Zoo.Validita_Inizio Is Nothing OrElse Ricette_Zoo.Validita_Inizio < AGRODATAINIZIO Then
				Ricette_Zoo.Validita_Inizio = AGRODATAINIZIO
			End If

			If Ricette_Zoo.Validita_Fine Is Nothing OrElse Ricette_Zoo.Validita_Fine < AGRODATAFINE Then
				Ricette_Zoo.Validita_Fine = AGRODATAFINE
			End If
		Catch ex As Exception
			messaggioErrore = ex.Message
			Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
			Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
		End Try
	End Sub

	Public Sub Elimina(ByRef Ricette_Zoo As AgronicaCoreEntityFramework_POCO.Ricette_Zoo,
					   ByRef GiasContext As Gias_DeveloperServer_Entities,
					   ByRef objParametriServer As AgronicaCoreParametri)
		Const nomeRoutine = "AgronicaCoreContabDAL.Ricette_Zoo_W.Elimina()"
		Dim messaggioErrore As String = ""

		Try
			GiasContext.Ricette_Zoo.Attach(Ricette_Zoo)
			GiasContext.Ricette_Zoo.Remove(Ricette_Zoo)

		Catch ex As Exception
			messaggioErrore = ex.Message
			Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
			Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
		End Try
	End Sub

#End Region

End Class
