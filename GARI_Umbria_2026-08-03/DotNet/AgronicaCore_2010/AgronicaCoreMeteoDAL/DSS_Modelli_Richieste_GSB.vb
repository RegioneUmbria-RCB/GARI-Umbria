Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports Newtonsoft.Json


Public Class DSS_Modelli_Richieste_GSB
	Inherits AgronicaCoreDataProvider.DataProvider

	Public Class ChiaveTabella
		Public Meteo_Sorgente As Integer
		Public Meteo_Stazione As Integer
		Public Meteo_DataInizio As DateTime
		Public Meteo_DataFine As DateTime
		Public Meteo_Forecast As Integer
		Public Mod_Cod As Integer
		Public Veg_Cod As Integer
		Public Avv_Cod As Integer
		Public Alg_Cod As Integer
		Public ParametriModello As String
	End Class

	Public Class Richiesta
		Inherits ChiaveTabella

		Public ChiaveRichiesta As Integer
		Public ValiditaMinuti As Integer
		Public OutOfRange As Boolean
		Public RichiestaElaborazione As Boolean
		Public RisultatoElaborazione As String
		Public Nome_Stazione As String
		Public Mod_Des As String
		Public Alg_Des As String
		Public Mod_Des_Agg As String
		Public Veg_Des As String
		Public Avv_Des As String
	End Class


	Public Function RichiediElaborazione(piva_superuser As String, piva As String, richieste As List(Of Richiesta), ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As List(Of Richiesta)

		Dim NomeRoutine As String = "DSS_Modelli_Richieste_GSB.RichiediElaborazione"

		Try

			Dim rows As New List(Of String)
			Dim cols As New List(Of String)
			For Each ric In richieste
				cols.Clear()
				cols.AddRange({
							  $"{ric.ChiaveRichiesta}",
							  $"{ric.Meteo_Sorgente}",
							  $"{ric.Meteo_Stazione}",
							  $"'{ric.Meteo_DataInizio.Date:yyyy-MM-ddTHH:mm:ss}'",
							  $"'{ric.Meteo_DataFine.Date:yyyy-MM-ddTHH:mm:ss}'",
							  $"{ric.Meteo_Forecast}",
							  $"{ric.Mod_Cod}",
							  $"{ric.Veg_Cod}",
							  $"{ric.Avv_Cod}",
							  $"{ric.Alg_Cod}",
							  $"'{ric.ParametriModello}'",
							  $"{ric.ValiditaMinuti}",
							  $"{If(ric.OutOfRange, 1, 0)}"})
				rows.Add($"({String.Join(", ", cols)})")
			Next

			Dim sql As String = $"
DECLARE @DATA_VALIDITA AS DATETIME = GETDATE();
DECLARE @PIVA_SUPERUSER AS VARCHAR(50) = '{piva_superuser}';
DECLARE @PIVA AS VARCHAR(50) = '{piva}';

DECLARE @TBL_RICHIESTA TABLE (
	ChiaveRichiesta INT
	, Meteo_Sorgente INT
	, Meteo_Stazione INT
	, Meteo_DataInizio DATETIME
	, Meteo_DataFine DATETIME
	, Meteo_Forecast INT
	, Mod_Cod INT
	, Veg_Cod INT
	, Avv_Cod INT
	, Alg_Cod INT
	, ParametriModello VARCHAR(MAX)
	, ValiditaMinuti INT
	, OutOfRange BIT
	, RichiestaElaborazione BIT
	, RisultatoElaborazione VARCHAR(MAX)
	, Nome_Stazione VARCHAR(MAX)
	, Mod_Des VARCHAR(MAX)
	, Alg_Des VARCHAR(MAX)
	, Mod_Des_Agg VARCHAR(MAX)
	, Veg_Des VARCHAR(MAX)
	, Avv_Des VARCHAR(MAX)
)

INSERT INTO @TBL_RICHIESTA (
	ChiaveRichiesta
	, Meteo_Sorgente
	, Meteo_Stazione
	, Meteo_DataInizio
	, Meteo_DataFine
	, Meteo_Forecast
	, Mod_Cod
	, Veg_Cod
	, Avv_Cod
	, Alg_Cod
	, ParametriModello
	, ValiditaMinuti
	, OutOfRange
)
VALUES
{String.Join(", " & vbCrLf, rows)}


; WITH RICHIESTA_CTE AS (
	SELECT *, Seq = ROW_NUMBER() OVER (PARTITION BY Meteo_Sorgente, Meteo_Stazione, Meteo_DataInizio, Meteo_DataFine, Meteo_Forecast, Mod_Cod, Veg_Cod, Avv_Cod, Alg_Cod, ParametriModello ORDER BY ValiditaMinuti)   
	FROM @TBL_RICHIESTA
)

, REFRESH_CTE AS (
	SELECT R.Meteo_Sorgente, R.Meteo_Stazione, R.Meteo_DataInizio, R.Meteo_DataFine, R.Meteo_Forecast
		, R.Mod_Cod, R.Veg_Cod, R.Avv_Cod, R.Alg_Cod, R.ParametriModello 
		, New_Richiesta = IIF(R.OutOfRange = 1, 0, IIF(COALESCE(DATEADD(minute, R.ValiditaMinuti, T.DataOraElaborazione), '1900-01-01T00:00:00') > @DATA_VALIDITA, 0, 1))
	FROM RICHIESTA_CTE R
	LEFT JOIN DSS_Modelli_Richieste_GSB T ON
		T.Meteo_Sorgente = R.Meteo_Sorgente AND T.Meteo_Stazione = R.Meteo_Stazione
		AND T.Meteo_DataInizio = R.Meteo_DataInizio AND T.Meteo_DataFine = R.Meteo_DataFine AND T.Meteo_Forecast = R.Meteo_Forecast
		AND T.Mod_Cod = R.Mod_Cod AND T.Veg_Cod = R.Veg_Cod AND T.Avv_Cod = R.Avv_Cod AND T.Alg_Cod = R.Alg_Cod
		AND T.ParametriModello = R.ParametriModello
	WHERE R.Seq = 1
)


MERGE INTO DSS_Modelli_Richieste_GSB T
USING (SELECT * FROM REFRESH_CTE WHERE New_Richiesta = 1) S
ON T.Meteo_Sorgente = S.Meteo_Sorgente AND T.Meteo_Stazione = S.Meteo_Stazione 
	AND T.Meteo_Datainizio = S.Meteo_DataInizio AND T.Meteo_DataFine = S.Meteo_DataFine AND T.Meteo_Forecast = S.Meteo_Forecast
	AND T.Mod_Cod = S.Mod_Cod AND T.Veg_Cod = S.Veg_Cod AND T.Avv_Cod = S.Avv_Cod AND T.Alg_Cod = S.Alg_Cod
	AND T.ParametriModello = S.ParametriModello
WHEN MATCHED AND T.RichiestaElaborazione = 0 THEN UPDATE SET T.RichiestaElaborazione = 1
WHEN NOT MATCHED THEN 
INSERT (Meteo_Sorgente, Meteo_Stazione, Meteo_Datainizio, Meteo_DataFine, Meteo_Forecast, Mod_Cod, Veg_Cod, Avv_Cod, Alg_Cod, ParametriModello, RichiestaElaborazione)
VALUES (S.Meteo_Sorgente, S.Meteo_Stazione, S.Meteo_Datainizio, S.Meteo_DataFine, S.Meteo_Forecast, S.Mod_Cod, S.Veg_Cod, S.Avv_Cod, S.Alg_Cod, S.ParametriModello, 1);



;WITH STAZIONI_CTE AS (
	SELECT Sorgente = 99, Id_Stazione = S.Id, Nome_Stazione = CASE WHEN vis.Anonima = 0 THEN S.Nome ELSE F.Descrizione + ' (' + S.RifFornitore + ')' END 
	FROM MeteoNT_Stazioni S WITH (NOLOCK)
	INNER JOIN MeteoNT_Fornitori F WITH (NOLOCK) ON F.Id = S.Id_Fornitore 
	INNER JOIN ( 
		SELECT Id_Stazione, Anonima 
		FROM ( 
			SELECT Id_Stazione, Anonima, nr = ROW_NUMBER() OVER (PARTITION BY Id_Stazione ORDER BY Id_stazione, Anonima) 
			FROM MeteoNT_VisibilitaStazioni 
			WHERE PIVA_Superuser = @PIVA_SUPERUSER AND (PIVA = @PIVA OR PIVA = '*') 
		) TBL1 WHERE nr = 1 
	) vis on vis.Id_Stazione = S.Id 
	UNION
	SELECT Sorgente = 0, Id_Stazione = S.ID_Stazione, Nome_Stazione = Stazione_Des 
	FROM TB_Stazioni S 
	INNER JOIN TB_QuadrantixStazioni QS ON S.ID_Stazione = QS.ID_Stazione 
	INNER JOIN TB_Quadranti Q ON Q.ID_Quadrante = QS.ID_Quadrante 
	UNION
	SELECT Sorgente = 3, Id_Stazione = Q.ID_Quadrante, Nome_Stazione = Quadrante_Des 
	FROM TB_Quadranti Q 
	UNION
	SELECT Sorgente = 4, Id_Stazione = S.Id, Nome_Stazione = IIF(A.Id_Stazione IS NULL, F.Descrizione + ' (' + S.Nome + ')', A.Alias)
	FROM MeteoNT_Stazioni S
	INNER JOIN MeteoNT_Fornitori F ON F.Id = S.Id_Fornitore
	LEFT JOIN MeteoNT_VisibilitaStazioniAlias A ON A.Id_Stazione = S.Id AND A.PIVA_Superuser = @PIVA_SUPERUSER AND A.PIVA = @PIVA
	WHERE Categoria = 1
)

, MODELLI_CTE AS (
	SELECT 
		sv.Veg_Cod 
		, sv.Veg_Des 
		, mp.Mod_Cod 
		, mp.Mod_Des 
		, Alg_Cod = COALESCE(alg.Algoritmo_Cod, 0) 
		, Alg_Des = COALESCE(alg.Algoritmo_Des, '') 
		, Mod_Des_Agg = COALESCE(OpAut.DescrizioneAggiuntiva, '') 
		, avv.Av_Cod 
		, Av_Des = avv.Av_Des_Vol 
		, Av_Des_Lat = avv.Av_Des_Lat 
	FROM (
		SELECT Mod_Cod, DescrizioneAggiuntiva 
		FROM ModelliPrevisionaliXpiva_superUser_OperazioniAutorizzate 
		WHERE Piva_SuperUser = @PIVA_SuperUser AND Tipo_Visibilita = 0 
		UNION 
		SELECT OpAut0.Mod_Cod, OpAut0.DescrizioneAggiuntiva 
		FROM ModelliPrevisionaliXpiva_OperazioniAutorizzate OpAut0 
		INNER JOIN ModelliPrevisionaliXpiva_superUser_OperazioniAutorizzate OpAut1 ON OpAut1.Piva_SuperUser = OpAut0.Piva_SuperUser AND OpAut1.Mod_Cod = OpAut0.Mod_Cod AND OpAut1.Tipo_Visibilita = 1 
		WHERE OpAut0.Piva_SuperUser = @PIVA_SuperUser AND OpAut0.Piva = @PIVA AND OpAut0.Tipo_Visibilita = 0 
	) OpAut 
	INNER JOIN ModelliPrevisionali mp ON mp.Mod_Cod = OpAut.Mod_Cod 
	LEFT JOIN ModelliPrevisionali_Algoritmo alg ON alg.modello = mp.Mod_Cod 
	INNER JOIN ModellixSpecieXAvversita msa ON msa.Mod_Cod = mp.Mod_Cod 
	INNER JOIN SpecieVegetali sv ON sv.Veg_Cod = msa.Veg_Cod 
	INNER JOIN Avversita avv ON avv.Av_Cod = msa.Av_Cod 
	WHERE msa.Attivo = 1 
)


UPDATE R
SET
	RichiestaElaborazione = COALESCE(T.RichiestaElaborazione, 0)
	, RisultatoElaborazione = COALESCE(T.RisultatoElaborazione, '')
	, Nome_Stazione = Z.Nome_Stazione 
	, Mod_Des = M.Mod_Des 
	, Alg_Des = M.Alg_Des
	, Mod_Des_Agg = M.Mod_Des_Agg
	, Veg_Des = M.Veg_Des
	, Avv_des = M.Av_Des_Lat
FROM @TBL_RICHIESTA R
INNER JOIN STAZIONI_CTE Z ON (Z.Sorgente = R.Meteo_Sorgente OR (Z.Sorgente = 99 AND R.Meteo_Sorgente IN (1, 2))) AND Z.Id_Stazione = R.Meteo_Stazione
INNER JOIN MODELLI_CTE M ON M.Mod_Cod = R.Mod_Cod AND M.Veg_Cod = R.Veg_Cod AND M.Av_Cod = R.Avv_Cod AND M.Alg_Cod = R.Alg_Cod
LEFT JOIN DSS_Modelli_Richieste_GSB T ON
	T.Meteo_Sorgente = R.Meteo_Sorgente AND T.Meteo_Stazione = R.Meteo_Stazione
	AND T.Meteo_DataInizio = R.Meteo_DataInizio AND T.Meteo_DataFine = R.Meteo_DataFine AND T.Meteo_Forecast = R.Meteo_Forecast
	AND T.Mod_Cod = R.Mod_Cod AND T.Veg_Cod = R.Veg_Cod AND T.Avv_Cod = R.Avv_Cod AND T.Alg_Cod = R.Alg_Cod
	AND T.ParametriModello = R.ParametriModello


SELECT * FROM @TBL_RICHIESTA"

			Dim DT As DataTable = EseguiQuery_Lettura(objParametri, sql, NomeRoutine)

			Dim json = JsonConvert.SerializeObject(DT)

			Dim response = JsonConvert.DeserializeObject(Of List(Of Richiesta))(json)

			Return response

		Catch ex As Exception

			Dim MessaggioErrore As String = ex.Message
			Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
			Throw New Exception("[" & NomeRoutine & "] :  " & MessaggioErrore)

		End Try
	End Function


	Public Function LeggiRichiesti(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As List(Of ChiaveTabella)

		Dim NomeRoutine As String = "DSS_Modelli_Richieste_GSB.LeggiRichiesti"

		Try
			Dim sql As String = $"SELECT dss.* 
FROM DSS_Modelli_Richieste_GSB dss 
INNER JOIN ModelliXSpeciexAvversita msa 
    ON dss.Mod_Cod = msa.Mod_Cod 
    AND dss.Veg_Cod = msa.Veg_Cod 
    AND dss.Avv_Cod = msa.Av_Cod 
WHERE dss.RichiestaElaborazione = 1 AND msa.Attivo <> 0"

			Dim DT = EseguiQuery_Lettura(objParametri, sql, NomeRoutine)

			Dim json = JsonConvert.SerializeObject(DT)

			Dim response = JsonConvert.DeserializeObject(Of List(Of ChiaveTabella))(json)

			Return response

		Catch ex As Exception

			Dim MessaggioErrore As String = ex.Message
			Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
			Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
		End Try
	End Function


	Public Function ScriviRisultato(chiave As ChiaveTabella, Risultato As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

		Dim NomeRoutine As String = "DSS_Modelli_Richieste_GSB.ScriviRisultato"

		Try

			Dim sql As String = $"UPDATE DSS_Modelli_Richieste_GSB
SET
	RichiestaElaborazione = 0
	, DataOraElaborazione = GETDATE()
	, RisultatoElaborazione = '{Risultato.Replace("'", "''")}'
WHERE Meteo_Sorgente = {chiave.Meteo_Sorgente}
	AND Meteo_Stazione = {chiave.Meteo_Stazione}
	AND Meteo_DataInizio = '{chiave.Meteo_DataInizio:yyyy-MM-ddTHH:mm:ss}'
	AND Meteo_DataFine = '{chiave.Meteo_DataFine:yyyy-MM-ddTHH:mm:ss}'
	AND Meteo_Forecast = {chiave.Meteo_Forecast}
	AND Mod_Cod = {chiave.Mod_Cod}
	AND Veg_Cod = {chiave.Veg_Cod}
	AND Avv_Cod = {chiave.Avv_Cod}
	AND Alg_Cod = {chiave.Alg_Cod}
	AND ParametriModello = '{chiave.ParametriModello}'"

			Dim result = EseguiQuery_Scrittura(objParametri, sql, NomeRoutine)

			Return result

		Catch ex As Exception

			Dim MessaggioErrore As String = ex.Message
			Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
			Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
		End Try

	End Function


	Public Function EliminaRichiesti(MaxDate As DateTime, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

		Dim NomeRoutine As String = "DSS_Modelli_Richieste_GSB.EliminaRichiesti"

		Try

			Dim sql As String = $"DECLARE @Counter INT = 1

WHILE @Counter > 0
BEGIN

	DELETE TOP(1000)
	FROM DSS_Modelli_Richieste_GSB
	WHERE DataOraElaborazione < '{MaxDate:yyyy-MM-ddTHH:mm:ss}'

	SET @Counter = @@ROWCOUNT
END"

			Dim result = EseguiQuery_Scrittura(objParametri, sql, NomeRoutine)

			Return result

		Catch ex As Exception

			Dim MessaggioErrore As String = ex.Message
			Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
			'Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
		End Try

		Return False
	End Function

End Class
