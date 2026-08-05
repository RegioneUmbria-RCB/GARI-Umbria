

Imports System.Text
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreMeteoDAL
Imports Newtonsoft.Json

Public Class DSS_ModelliPrevisionali_Elaborazione_GSB
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Class ChiaveElaborazione : Implements IEquatable(Of ChiaveElaborazione)
        Public Tipo_Sorgente As Integer
        Public Stazione_Cod As Integer
        Public Mod_Cod As Integer
        Public Veg_Cod As Integer
        Public Avv_Cod As Integer
        Public Alg_Cod As Integer
        Public ParametriElaborazione As String

        Public Overrides Function Equals(other As Object) As Boolean
            If other.GetType Is GetType(ChiaveElaborazione) Then
                Return _equals(CType(other, ChiaveElaborazione))
            End If
            Return False
        End Function

        Public Function _equals(other As ChiaveElaborazione) As Boolean Implements IEquatable(Of ChiaveElaborazione).Equals

            If Tipo_Sorgente <> other.Tipo_Sorgente Then
                Return False
            End If
            If Stazione_Cod <> other.Stazione_Cod Then
                Return False
            End If
            If Mod_Cod <> other.Mod_Cod Then
                Return False
            End If
            If Veg_Cod <> other.Veg_Cod Then
                Return False
            End If
            If Avv_Cod <> other.Avv_Cod Then
                Return False
            End If
            If Alg_Cod <> other.Alg_Cod Then
                Return False
            End If
            Return ParametriElaborazione.CompareTo(other.ParametriElaborazione) = 0
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return 0
        End Function
    End Class

    Public Class ListaChiaviElaborazione
        Private _lista As List(Of ChiaveElaborazione)
        Public Sub New()
            _lista = New List(Of ChiaveElaborazione)
        End Sub
        Public Function IsEmpty() As Boolean
            Return Not _lista.Any()
        End Function
        Public Function Add(ByVal elem As ChiaveElaborazione)
            If _lista.Contains(elem) Then
                Return False
            End If
            _lista.Add(elem)
            Return True
        End Function
        Public Function SQL() As String
            Dim _sql As New StringBuilder
            _sql.Clear()
            Dim comma As String = ""
            For Each c In _lista
                _sql.Append(comma)
                _sql.Append("(")
                _sql.Append(c.Tipo_Sorgente.ToString() & ", ")
                _sql.Append(c.Stazione_Cod.ToString() & ", ")
                _sql.Append(c.Mod_Cod.ToString() & ", ")
                _sql.Append(c.Veg_Cod.ToString() & ", ")
                _sql.Append(c.Avv_Cod.ToString() & ", ")
                _sql.Append(c.Alg_Cod.ToString() & ", ")
                _sql.Append("'" & c.ParametriElaborazione & "'")
                _sql.Append(")")
                comma = "," & vbCrLf
            Next
            Return _sql.ToString()
        End Function
    End Class

    Public Function LeggiRisultatoModello(ByVal chiave As ChiaveElaborazione, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataRow

        Dim NomeRoutine As String = "DSS_ModelliPrevisionali_Elaborazione_GSB.LeggiRisultatoModello"

        Dim DT As DataTable = Nothing

        Try

            Dim sql As New StringBuilder

            sql.Clear()

            sql.AppendLine("SELECT ")
            sql.AppendLine("    COALESCE(DataOraElaborazione, CAST('1900-01-01' AS DATE)) AS DataOraElaborazione ")
            sql.AppendLine("    , COALESCE(RisultatoElaborazione, '') AS RisultatoElaborazione ")
            sql.AppendLine("    , RichiestaElaborazione ")
            sql.AppendLine("FROM DSS_ModelliPrevisionali_Elaborazione_GSB ")
            sql.AppendLine("WHERE Tipo_Sorgente = " & chiave.Tipo_Sorgente)
            sql.AppendLine("    AND Stazione_Cod = " & chiave.Stazione_Cod)
            sql.AppendLine("    AND Mod_Cod = " & chiave.Mod_Cod)
            sql.AppendLine("    AND Veg_Cod = " & chiave.Veg_Cod)
            sql.AppendLine("    AND Avv_Cod = " & chiave.Avv_Cod)
            sql.AppendLine("    AND Alg_Cod = " & chiave.Alg_Cod)
            sql.AppendLine("    AND ParametriElaborazione = '" & Agro_SQL_SaveText(chiave.ParametriElaborazione) & "'")

            Select Case objParametri.FlagVisibilita

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    sql.AppendLine("    AND Inviato >= 0 ")

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    sql.AppendLine("    AND Inviato = -1 ")

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti

                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select


            DT = EseguiQuery_Lettura(objParametri, sql.ToString, NomeRoutine)

        Catch ex As Exception

            DT = Nothing

            Dim MessaggioErrore As String = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        If DT Is Nothing OrElse DT.Rows.Count = 0 Then
            Return Nothing
        End If

        Return DT.Rows(0)
    End Function

    Public Function RichiediElaborazioneModelli(ByVal listaChiavi As ListaChiaviElaborazione, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "DSS_ModelliPrevisionali_Elaborazione_GSB.RichiediElaborazioneModelli"

        If listaChiavi.IsEmpty() Then
            Return False
        End If

        Dim xRisp As Boolean = False

        Try

            Dim sql As New StringBuilder
            sql.Clear()

            sql.AppendLine("SET NOCOUNT ON ")
            sql.AppendLine()
            sql.AppendLine("DECLARE @TMPTABLE TABLE(Tipo_Sorgente INT, Stazione_Cod INT, Mod_Cod INT, Veg_Cod INT, Avv_Cod INT, Alg_Cod INT, ParametriElaborazione VARCHAR(300)) ")
            sql.AppendLine("INSERT INTO @TMPTABLE (Tipo_Sorgente, Stazione_Cod, Mod_Cod, Veg_Cod, Avv_Cod, Alg_Cod, ParametriElaborazione) VALUES ")
            sql.Append(listaChiavi.SQL())
            sql.AppendLine()
            sql.AppendLine("DECLARE @ElencoOperazioni TABLE(Operazione VARCHAR(20)); ")
            sql.AppendLine()
            sql.AppendLine("MERGE INTO DSS_ModelliPrevisionali_Elaborazione_GSB AS t ")
            sql.AppendLine("USING (SELECT * FROM @TMPTABLE) AS s ")
            sql.AppendLine("ON t.Tipo_Sorgente = s.Tipo_Sorgente ")
            sql.AppendLine("	AND t.Stazione_Cod = s.Stazione_Cod  ")
            sql.AppendLine("	AND t.Mod_Cod = s.Mod_Cod ")
            sql.AppendLine("	AND t.Veg_Cod = s.Veg_Cod ")
            sql.AppendLine("	AND t.Avv_Cod = s.Avv_Cod ")
            sql.AppendLine("	AND t.Alg_Cod = s.Alg_Cod ")
            sql.AppendLine("	AND t.ParametriElaborazione = s.ParametriElaborazione ")
            sql.AppendLine("WHEN MATCHED AND t.RichiestaElaborazione = 0 THEN ")
            sql.AppendLine("    UPDATE Set RichiestaElaborazione = 1 ")
            sql.AppendLine("WHEN NOT MATCHED THEN ")
            sql.AppendLine("    INSERT (Tipo_Sorgente, Stazione_Cod, Mod_Cod, Veg_Cod, Avv_Cod, Alg_Cod, ParametriElaborazione, RichiestaElaborazione) ")
            sql.AppendLine("	VALUES (s.Tipo_Sorgente, s.Stazione_Cod, s.Mod_Cod, s.Veg_Cod, s.Avv_Cod, s.Alg_Cod, s.ParametriElaborazione, 1) ")
            sql.AppendLine("OUTPUT $action INTO @ElencoOperazioni; ")
            sql.AppendLine()
            sql.AppendLine("SELECT Operazione, ContaOperazioni = COUNT(*) FROM @ElencoOperazioni GROUP BY Operazione ")

            Dim DT As DataTable = EseguiQuery_Lettura(objParametri, sql.ToString, NomeRoutine)

            If DT IsNot Nothing AndAlso DT.Rows.Count > 0 Then

                Dim nInsert As Integer = 0
                Dim nUpdate As Integer = 0

                For Each r In DT.Rows
                    If r("Operazione").ToString.ToUpper = "INSERT" Then
                        nInsert = r("ContaOperazioni")
                    End If
                    If r("Operazione").ToString.ToUpper = "UPDATE" Then
                        nUpdate = r("ContaOperazioni")
                    End If
                Next

                xRisp = (nInsert + nUpdate) > 0
            End If

        Catch ex As Exception

            xRisp = False

            Dim MessaggioErrore As String = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp
    End Function





    Public Class RigaRichiesta
        Public Tipo_Sorgente As Integer
        Public Stazione_Cod As Integer
        Public Mod_Cod As Integer
        Public Veg_Cod As Integer
        Public Avv_Cod As Integer
        Public Alg_Cod As Integer
        Public ParametriElaborazioneKey As String
        Public ParametriElaborazioneOut As String
        Public Validita_Minuti As Integer
        Public OutOfRange As Boolean
        Public RichiestaElaborazione As Boolean
        Public RisultatoElaborazione As String
        Public Nome_Stazione As String
        Public Mod_Des As String
        Public Alg_Des As String
        Public Mod_Des_Agg As String
        Public Veg_Des As String
        Public Avv_Des As String
        Public IdGroup As Integer
        Public DescrGroup As String
    End Class

    Private Class KeyRigaRichiesta : Implements IEquatable(Of KeyRigaRichiesta)

        Public Id As Integer
        Public Tipo_Sorgente As Integer
        Public Stazione_Cod As Integer
        Public Mod_Cod As Integer
        Public Veg_Cod As Integer
        Public Avv_Cod As Integer
        Public Alg_Cod As Integer
        Public ParametriElaborazione As String
        Public ValiditaMinuti As Integer
        Public OutOfRange As Boolean

        Public Sub New(id_ As Integer, riga As RigaRichiesta)
            Id = id_
            Tipo_Sorgente = riga.Tipo_Sorgente
            Stazione_Cod = riga.Stazione_Cod
            Mod_Cod = riga.Mod_Cod
            Veg_Cod = riga.Veg_Cod
            Avv_Cod = riga.Avv_Cod
            Alg_Cod = riga.Alg_Cod
            ParametriElaborazione = riga.ParametriElaborazioneKey
            ValiditaMinuti = riga.Validita_Minuti
            OutOfRange = riga.OutOfRange
        End Sub

        Public Overrides Function Equals(other As Object) As Boolean
            If other.GetType Is GetType(KeyRigaRichiesta) Then
                Return _equals(CType(other, KeyRigaRichiesta))
            End If
            Return False
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return 0
        End Function

        Private Function _equals(other As KeyRigaRichiesta) As Boolean Implements IEquatable(Of KeyRigaRichiesta).Equals
            Return Tipo_Sorgente = other.Tipo_Sorgente AndAlso
                Stazione_Cod = other.Stazione_Cod AndAlso
                Mod_Cod = other.Mod_Cod AndAlso
                Veg_Cod = other.Veg_Cod AndAlso
                Avv_Cod = other.Avv_Cod AndAlso
                Alg_Cod = other.Alg_Cod AndAlso
                ParametriElaborazione.CompareTo(other.ParametriElaborazione) = 0
        End Function
    End Class

    Private Class RigaRisposta
        Public Id As Integer
        Public RichiestaElaborazione As Boolean
        Public RisultatoElaborazione As String
        Public Nome_Stazione As String
        Public Mod_Des As String
        Public Alg_Des As String
        Public Mod_Des_Agg As String
        Public Veg_Des As String
        Public Avv_Des As String
    End Class

    Public Function RichiediElaborazioneModelliV2(piva_superuser As String, piva As String, righe As List(Of RigaRichiesta), ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As List(Of RigaRichiesta)

        Dim NomeRoutine As String = "DSS_ModelliPrevisionali_Elaborazione_GSB.RichiediElaborazioneModelliV2"

        Try
            Dim tblFields As New List(Of String) From {
                "Id INT",
                "Tipo_Sorgente INT",
                "Stazione_Cod INT",
                "Mod_Cod INT",
                "Veg_Cod INT",
                "Avv_Cod INT",
                "Alg_Cod INT",
                "ParametriElaborazione VARCHAR(MAX)",
                "ValiditaMinuti INT",
                "OutOfRange BIT"
            }

            Dim tblRows As New List(Of String)

            Dim hashRighe As New HashSet(Of KeyRigaRichiesta)
            Dim dictRighe As New Dictionary(Of Integer, List(Of RigaRichiesta))
            Dim id As Integer = 1

            For Each r In righe

                Dim key0 As New KeyRigaRichiesta(id, r)
                Dim key1 As KeyRigaRichiesta

                If hashRighe.TryGetValue(key0, key1) Then

                    dictRighe(key1.Id).Add(r)
                Else

                    hashRighe.Add(key0)
                    dictRighe.Add(id, New List(Of RigaRichiesta) From {r})

                    Dim cols As New List(Of String) From {
                        Convert.ToString(id, Globalization.CultureInfo.InvariantCulture),
                        Convert.ToString(key0.Tipo_Sorgente, Globalization.CultureInfo.InvariantCulture),
                        Convert.ToString(key0.Stazione_Cod, Globalization.CultureInfo.InvariantCulture),
                        Convert.ToString(key0.Mod_Cod, Globalization.CultureInfo.InvariantCulture),
                        Convert.ToString(key0.Veg_Cod, Globalization.CultureInfo.InvariantCulture),
                        Convert.ToString(key0.Avv_Cod, Globalization.CultureInfo.InvariantCulture),
                        Convert.ToString(key0.Alg_Cod, Globalization.CultureInfo.InvariantCulture),
                        "'" & Agro_SQL_SaveText(key0.ParametriElaborazione) & "'",
                        Convert.ToString(key0.ValiditaMinuti, Globalization.CultureInfo.InvariantCulture),
                        If(key0.OutOfRange, "1", "0")
                    }

                    tblRows.Add("(" & String.Join(", ", cols) & ")")

                    id += 1
                End If
            Next

            Dim sql As New StringBuilder

            sql.Clear()
            sql.AppendLine("DECLARE @DATA_VALIDITA AS DATETIME = GETDATE();")
            sql.AppendLine("DECLARE @PIVA_SUPERUSER AS VARCHAR(50) = '" & Agro_SQL_SaveText(piva_superuser) & "';")
            sql.AppendLine("DECLARE @PIVA AS VARCHAR(50) = '" & Agro_SQL_SaveText(piva) & "';")
            sql.AppendLine()
            sql.AppendLine("DECLARE @TBL_RICHIESTA TABLE (" & String.Join(", ", tblFields) & ")")
            sql.AppendLine()
            sql.AppendLine("INSERT INTO @TBL_RICHIESTA VALUES")
            sql.AppendLine(String.Join(", " & vbCrLf, tblRows))
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine("DECLARE @TBL_RISULTATI TABLE (Id INT, Tipo_Sorgente INT, Stazione_Cod INT, Mod_Cod INT, Veg_Cod INT, Avv_Cod INT, Alg_Cod INT, ParametriElaborazione VARCHAR(MAX), RichiestaElaborazione BIT, RisultatoElaborazione VARCHAR(MAX))")
            sql.AppendLine("INSERT INTO @TBL_RISULTATI")
            sql.AppendLine("SELECT ")
            sql.AppendLine("	Id, Tipo_Sorgente, Stazione_Cod, Mod_Cod, Veg_Cod, Avv_Cod, Alg_Cod, ParametriElaborazione")
            sql.AppendLine("	, RichiestaElaborazione = IIF((OutOfRange = 0) AND ((FlagRichiesta = 1) OR (@DATA_VALIDITA > DataOraValidita)), 1, 0)")
            sql.AppendLine("	, RisultatoElaborazione")
            sql.AppendLine("FROM (")
            sql.AppendLine("	SELECT ")
            sql.AppendLine("		R.Id, R.Tipo_Sorgente, R.Stazione_Cod, R.Mod_Cod, R.Veg_Cod, R.Avv_Cod, R.Alg_Cod, R.ParametriElaborazione, R.OutOfRange")
            sql.AppendLine("		, DataOraValidita  = DATEADD(minute, R.ValiditaMinuti, COALESCE(DataOraElaborazione, CAST('1900-01-01' AS DATE))) ")
            sql.AppendLine("		, RisultatoElaborazione = COALESCE(T.RisultatoElaborazione, '') ")
            sql.AppendLine("		, FlagRichiesta = COALESCE(T.RichiestaElaborazione, 1) ")
            sql.AppendLine("	FROM @TBL_RICHIESTA R")
            sql.AppendLine("	LEFT JOIN DSS_ModelliPrevisionali_Elaborazione_GSB T ON")
            sql.AppendLine("		T.Tipo_Sorgente = R.Tipo_Sorgente AND T.Stazione_Cod = R.Stazione_Cod")
            sql.AppendLine("		AND T.Mod_Cod = R.Mod_Cod AND T.Veg_Cod = R.Veg_Cod")
            sql.AppendLine("		AND T.Avv_Cod = R.Avv_Cod AND T.Alg_Cod = R.Alg_Cod")
            sql.AppendLine("		AND T.ParametriElaborazione = R.ParametriElaborazione")
            sql.AppendLine(") T ")
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine("DECLARE @TBL_REFRESH TABLE (Tipo_Sorgente INT, Stazione_Cod INT, Mod_Cod INT, Veg_Cod INT, Avv_Cod INT, Alg_Cod INT, ParametriElaborazione VARCHAR(MAX))")
            sql.AppendLine("INSERT INTO @TBL_REFRESH")
            sql.AppendLine("SELECT DISTINCT Tipo_Sorgente, Stazione_Cod, Mod_Cod, Veg_Cod, Avv_Cod, Alg_Cod, ParametriElaborazione ")
            sql.AppendLine("FROM @TBL_RISULTATI")
            sql.AppendLine("WHERE RichiestaElaborazione = 1 ")
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine("DECLARE @CNT_REFRESH AS INTEGER = @@ROWCOUNT")
            sql.AppendLine()
            sql.AppendLine("IF @CNT_REFRESH > 0")
            sql.AppendLine("BEGIN")
            sql.AppendLine()
            sql.AppendLine("	MERGE INTO DSS_ModelliPrevisionali_Elaborazione_GSB AS T")
            sql.AppendLine("	USING @TBL_REFRESH S")
            sql.AppendLine("	ON T.Tipo_Sorgente = S.Tipo_Sorgente AND T.Stazione_Cod = S.Stazione_Cod")
            sql.AppendLine("		AND T.Mod_Cod = S.Mod_Cod AND T.Veg_Cod = S.Veg_Cod AND T.Avv_Cod = S.Avv_Cod AND T.Alg_Cod = S.Alg_Cod")
            sql.AppendLine("		AND T.ParametriElaborazione = S.ParametriElaborazione")
            sql.AppendLine("	WHEN MATCHED AND T.RichiestaElaborazione = 0 THEN")
            sql.AppendLine("		UPDATE SET T.RichiestaElaborazione = 1")
            sql.AppendLine("	WHEN NOT MATCHED THEN")
            sql.AppendLine("	INSERT (Tipo_Sorgente, Stazione_Cod, Mod_Cod, Veg_Cod, Avv_Cod, Alg_Cod, ParametriElaborazione, RichiestaElaborazione) ")
            sql.AppendLine("	VALUES (s.Tipo_Sorgente, s.Stazione_Cod, s.Mod_Cod, s.Veg_Cod, s.Avv_Cod, s.Alg_Cod, s.ParametriElaborazione, 1);")
            sql.AppendLine()
            sql.AppendLine("END")
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine("; WITH STAZIONI_CTE AS (")
            sql.AppendLine("	SELECT Sorgente = 99, Id_Stazione = S.Id, Nome_Stazione = CASE WHEN vis.Anonima = 0 THEN S.Nome ELSE F.Descrizione + ' (' + S.RifFornitore + ')' END ")
            sql.AppendLine("	FROM MeteoNT_Stazioni S ")
            sql.AppendLine("	INNER JOIN MeteoNT_Fornitori F ON F.Id = S.Id_Fornitore ")
            sql.AppendLine("	INNER JOIN ( ")
            sql.AppendLine("		SELECT Id_Stazione, Anonima ")
            sql.AppendLine("		FROM ( ")
            sql.AppendLine("			SELECT Id_Stazione, Anonima, nr = ROW_NUMBER() OVER (PARTITION BY Id_Stazione ORDER BY Id_stazione, Anonima) ")
            sql.AppendLine("			FROM MeteoNT_VisibilitaStazioni ")
            sql.AppendLine("			WHERE PIVA_Superuser = @PIVA_SUPERUSER AND (PIVA = @PIVA OR PIVA = '*') ")
            sql.AppendLine("		) TBL1 WHERE nr = 1 ")
            sql.AppendLine("	) vis on vis.Id_Stazione = S.Id ")
            sql.AppendLine("	UNION")
            sql.AppendLine("	SELECT Sorgente = 0, Id_Stazione = S.ID_Stazione, Nome_Stazione = Stazione_Des ")
            sql.AppendLine("	FROM TB_Stazioni S ")
            sql.AppendLine("	INNER JOIN TB_QuadrantixStazioni QS ON S.ID_Stazione = QS.ID_Stazione ")
            sql.AppendLine("	INNER JOIN TB_Quadranti Q ON Q.ID_Quadrante = QS.ID_Quadrante ")
            sql.AppendLine("	UNION")
            sql.AppendLine("	SELECT Sorgente = 3, Id_Stazione = Q.ID_Quadrante, Nome_Stazione = Quadrante_Des ")
            sql.AppendLine("	FROM TB_Quadranti Q ")
            sql.AppendLine("	UNION")
            sql.AppendLine("	SELECT Sorgente = 4, Id_Stazione = S.Id, Nome_Stazione = IIF(A.Id_Stazione IS NULL, F.Descrizione + ' (' + S.Nome + ')', A.Alias)")
            sql.AppendLine("	FROM MeteoNT_Stazioni S")
            sql.AppendLine("	INNER JOIN MeteoNT_Fornitori F ON F.Id = S.Id_Fornitore")
            sql.AppendLine("	LEFT JOIN MeteoNT_VisibilitaStazioniAlias A ON A.Id_Stazione = S.Id AND A.PIVA_Superuser = @PIVA_SUPERUSER AND A.PIVA = @PIVA")
            sql.AppendLine("	WHERE Categoria = 1")
            sql.AppendLine(")")
            sql.AppendLine("")
            sql.AppendLine(", MODELLI_CTE AS (")
            sql.AppendLine("	SELECT ")
            sql.AppendLine("		sv.Veg_Cod ")
            sql.AppendLine("		, sv.Veg_Des ")
            sql.AppendLine("		, mp.Mod_Cod ")
            sql.AppendLine("		, mp.Mod_Des ")
            sql.AppendLine("		, Alg_Cod = COALESCE(alg.Algoritmo_Cod, 0) ")
            sql.AppendLine("		, Alg_Des = COALESCE(alg.Algoritmo_Des, '') ")
            sql.AppendLine("		, Mod_Des_Agg = COALESCE(OpAut.DescrizioneAggiuntiva, '') ")
            sql.AppendLine("		, avv.Av_Cod ")
            sql.AppendLine("		, Av_Des = avv.Av_Des_Vol ")
            sql.AppendLine("		, Av_Des_Lat = avv.Av_Des_Lat ")
            sql.AppendLine("	FROM (")
            sql.AppendLine("		SELECT Mod_Cod, DescrizioneAggiuntiva ")
            sql.AppendLine("		FROM ModelliPrevisionaliXpiva_superUser_OperazioniAutorizzate ")
            sql.AppendLine("		WHERE Piva_SuperUser = @PIVA_SuperUser AND Tipo_Visibilita = 0 ")
            sql.AppendLine("		UNION ")
            sql.AppendLine("		SELECT OpAut0.Mod_Cod, OpAut0.DescrizioneAggiuntiva ")
            sql.AppendLine("		FROM ModelliPrevisionaliXpiva_OperazioniAutorizzate OpAut0 ")
            sql.AppendLine("		INNER JOIN ModelliPrevisionaliXpiva_superUser_OperazioniAutorizzate OpAut1 ON OpAut1.Piva_SuperUser = OpAut0.Piva_SuperUser AND OpAut1.Mod_Cod = OpAut0.Mod_Cod AND OpAut1.Tipo_Visibilita = 1 ")
            sql.AppendLine("		WHERE OpAut0.Piva_SuperUser = @PIVA_SuperUser AND OpAut0.Piva = @PIVA AND OpAut0.Tipo_Visibilita = 0 ")
            sql.AppendLine("	) OpAut ")
            sql.AppendLine("	INNER JOIN ModelliPrevisionali mp ON mp.Mod_Cod = OpAut.Mod_Cod ")
            sql.AppendLine("	LEFT JOIN ModelliPrevisionali_Algoritmo alg ON alg.modello = mp.Mod_Cod ")
            sql.AppendLine("	INNER JOIN ModellixSpecieXAvversita msa ON msa.Mod_Cod = mp.Mod_Cod ")
            sql.AppendLine("	INNER JOIN SpecieVegetali sv ON sv.Veg_Cod = msa.Veg_Cod ")
            sql.AppendLine("	INNER JOIN Avversita avv ON avv.Av_Cod = msa.Av_Cod ")
            sql.AppendLine("	WHERE msa.Attivo = 1 ")
            sql.AppendLine(")")
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine("SELECT ")
            sql.AppendLine("	R.Id, R.RichiestaElaborazione, R.RisultatoElaborazione")
            sql.AppendLine("	, Z.Nome_Stazione ")
            sql.AppendLine("	, M.Mod_Des, M.Alg_Des, M.Mod_Des_Agg, M.Veg_Des, M.Av_Des")
            sql.AppendLine("FROM @TBL_RISULTATI R")
            sql.AppendLine("INNER JOIN STAZIONI_CTE Z ON (Z.Sorgente = R.Tipo_Sorgente OR (Z.Sorgente = 99 AND R.Tipo_Sorgente IN (1, 2))) AND Z.Id_Stazione = R.Stazione_Cod")
            sql.AppendLine("INNER JOIN MODELLI_CTE M ON M.Mod_Cod = R.Mod_Cod AND M.Veg_Cod = R.Veg_Cod AND M.Av_Cod = R.Avv_Cod AND M.Alg_Cod = R.Alg_Cod")

            Dim DT As DataTable = EseguiQuery_Lettura(objParametri, sql.ToString, NomeRoutine)

            Dim json = JsonConvert.SerializeObject(DT)

            Dim response = JsonConvert.DeserializeObject(Of List(Of RigaRisposta))(json)

            Dim vLista As List(Of RigaRichiesta)

            For Each rr In response

                If dictRighe.TryGetValue(rr.Id, vLista) Then

                    For Each r In vLista

                        r.RichiestaElaborazione = rr.RichiestaElaborazione
                        r.RisultatoElaborazione = rr.RisultatoElaborazione
                        r.Nome_Stazione = rr.Nome_Stazione
                        r.Mod_Des = rr.Mod_Des
                        r.Alg_Des = rr.Alg_Des
                        r.Mod_Des_Agg = rr.Mod_Des_Agg
                        r.Veg_Des = rr.Veg_Des
                        r.Avv_Des = rr.Avv_Des
                    Next
                End If
            Next

            Return righe

        Catch ex As Exception

            Dim MessaggioErrore As String = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] :  " & MessaggioErrore)
        End Try
    End Function









    Public Function LeggiModelliDaElaborare(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "DSS_ModelliPrevisionali_Elaborazione_GSB.LeggiModelliDaElaborare"

        Dim DT As DataTable = Nothing

        Try
            Dim sql As New StringBuilder

            sql.Clear()
            sql.AppendLine("SELECT dss.* ")
            sql.AppendLine("FROM DSS_ModelliPrevisionali_Elaborazione_GSB dss ")
            sql.AppendLine("INNER JOIN ModelliXSpeciexAvversita msa ")
            sql.AppendLine("    ON dss.Mod_Cod = msa.Mod_Cod ")
            sql.AppendLine("    AND dss.Veg_Cod = msa.Veg_Cod ")
            sql.AppendLine("    AND dss.Avv_Cod = msa.Av_Cod ")
            sql.AppendLine("WHERE dss.RichiestaElaborazione = 1 AND msa.Attivo <> 0 ")

            Select Case objParametri.FlagVisibilita

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati

                    sql.Append(" AND   dss.Inviato >=0 ")

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati

                    sql.Append(" AND   dss.Inviato =-1 ")

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti

                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            DT = EseguiQuery_Lettura(objParametri, sql.ToString, NomeRoutine)

        Catch ex As Exception

            DT = Nothing

            Dim MessaggioErrore As String = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT
    End Function

    Public Function ScriviRisultatoModello(ByVal chiave As ChiaveElaborazione, ByVal RisultatoElaborazione As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim xRisp As Boolean = False

        Dim NomeRoutine As String = "DSS_ModelliPrevisionali_Elaborazione_GSB.ScriviRisultatoModello"

        Try

            Dim sql As New StringBuilder

            sql.Clear()

            sql.AppendLine("UPDATE DSS_ModelliPrevisionali_Elaborazione_GSB ")
            sql.AppendLine("SET ")
            sql.AppendLine("    RichiestaElaborazione = 0 ")
            sql.AppendLine("    , DataOraElaborazione = " & Agro_SQL_SaveDateTime(Now))
            sql.AppendLine("    , RisultatoElaborazione = '" & Agro_SQL_SaveText(RisultatoElaborazione, False) & "' ")
            sql.AppendLine("    , Data_Modifica = " & Agro_SQL_SaveDateTime(Now))
            sql.AppendLine("    , UserName_Modifica = 'agronica' ")
            sql.AppendLine("WHERE Tipo_Sorgente = " & chiave.Tipo_Sorgente)
            sql.AppendLine("    AND Stazione_Cod = " & chiave.Stazione_Cod)
            sql.AppendLine("    AND Mod_Cod = " & chiave.Mod_Cod)
            sql.AppendLine("    AND Veg_Cod = " & chiave.Veg_Cod)
            sql.AppendLine("    AND Avv_Cod = " & chiave.Avv_Cod)
            sql.AppendLine("    AND Alg_Cod = " & chiave.Alg_Cod)
            sql.AppendLine("    AND ParametriElaborazione = '" & Agro_SQL_SaveText(chiave.ParametriElaborazione) & "'")

            xRisp = EseguiQuery_Scrittura(objParametri, sql.ToString, NomeRoutine)

        Catch ex As Exception

            xRisp = False

            Dim MessaggioErrore As String = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp
    End Function

End Class
