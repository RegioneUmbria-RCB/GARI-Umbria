Imports System.Text
Imports AgronicaCoreDataProvider
Public Class Acmotec
    Inherits AgronicaCoreDataProvider.DataProvider
#Region "Gestione Acmotec"
    Public Function Upsert_Acmotec_Devices(ByVal hlp As Upsert_Helper, ByVal ObjParametri_Server As AgronicaCoreParametri) As Integer

        Dim NomeRoutine As String = "IoT.Upsert_Acmotec_Devices"
        Dim nInsert As Integer = 0

        Try

            If hlp.Count > 0 Then

                Dim sql As New StringBuilder

                sql.Clear()

                sql.AppendLine("SET NOCOUNT ON ")
                sql.AppendLine()
                sql.AppendLine("DECLARE @TMPTABLE TABLE([guid] VARCHAR(32),type VARCHAR(30), [name] VARCHAR(250), [serial] VARCHAR(50), [Lat] FLOAT, [Lng] FLOAT, [CreatedAt] DATETIME)")
                sql.AppendLine("INSERT INTO @TMPTABLE ([guid], [type], [name], [serial], [Lat], [Lng], [CreatedAt]) VALUES ")
                sql.Append(hlp.GetAndReset())
                sql.AppendLine()
                sql.AppendLine("DECLARE @ElencoOperazioni TABLE(Operazione VARCHAR(20)); ")
                sql.AppendLine()
                sql.AppendLine("MERGE INTO Acmotec_Dispositivi AS t ")
                sql.AppendLine("    USING ")
                sql.AppendLine("        (SELECT * FROM @TMPTABLE) AS s ")
                sql.AppendLine("    ON t.[guid] = s.[guid] ")
                sql.AppendLine("	--WHEN MATCHED THEN ")
                sql.AppendLine("	--	UPDATE SET Name = s.Name, ... ")
                sql.AppendLine("	WHEN NOT MATCHED THEN ")
                sql.AppendLine("		INSERT ([guid], [type], [name], [serial], [Lat], [Lng], [CreatedAt], [FlagScaricoDati]) ")
                sql.AppendLine("		VALUES (s.[guid], s.[type], s.[name], s.[serial], s.[Lat], s.[Lng], s.[CreatedAt], 1) ")
                sql.AppendLine("        --Per i dispositivi appena inseriti attivo anche lo scarico")
                sql.AppendLine("	OUTPUT $action INTO @ElencoOperazioni; ")
                sql.AppendLine()
                sql.AppendLine("SELECT Operazione, COUNT(*) AS ContaOperazioni FROM @ElencoOperazioni GROUP BY Operazione")

                Dim DT As DataTable = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

                For Each r In DT.Rows
                    If r("Operazione").ToString.ToUpper = "INSERT" Then
                        nInsert = r("ContaOperazioni")
                    End If
                Next

            End If

        Catch ex As Exception

            nInsert = 0
            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return nInsert
    End Function

    Public Function Upsert_Acmotec_Quantities(ByVal hlp As Upsert_Helper, ByVal ObjParametri_Server As AgronicaCoreParametri) As Integer

        Dim NomeRoutine As String = "IoT.Upsert_Acmotec_Quantities"
        Dim nInsert As Integer = 0

        Try

            If hlp.Count > 0 Then

                Dim sql As New StringBuilder

                sql.Clear()

                sql.AppendLine("SET NOCOUNT ON ")
                sql.AppendLine()
                sql.AppendLine("DECLARE @TMPTABLE TABLE([Id_Dispositivo] varchar(32),[MeasureId] varchar(50), [MeasureName] varchar(MAX),  [MeasureUnit] varchar(50), [CreatedAt] DATETIME) ")
                sql.AppendLine("INSERT INTO @TMPTABLE ([Id_Dispositivo], [MeasureId], [MeasureName], [MeasureUnit], [CreatedAt] ) VALUES ")
                sql.Append(hlp.GetAndReset())
                sql.AppendLine()
                sql.AppendLine("DECLARE @ElencoOperazioni TABLE(Operazione VARCHAR(20)); ")
                sql.AppendLine()
                sql.AppendLine("MERGE INTO Acmotec_Sensori AS t ")
                sql.AppendLine("    USING ")
                sql.AppendLine("        (SELECT * FROM @TMPTABLE) AS s ")
                sql.AppendLine("    ON t.[Id_Dispositivo] = s.[Id_Dispositivo] AND t.[MeasureName] = s.[MeasureName] ")
                sql.AppendLine("	--WHEN MATCHED THEN ")
                sql.AppendLine("	--	UPDATE SET ... ")
                sql.AppendLine("	WHEN NOT MATCHED THEN ")
                sql.AppendLine("		INSERT ([Id_Dispositivo], [MeasureId], [MeasureName], [MeasureUnit], [CreatedAt] ) ")
                sql.AppendLine("		VALUES (s.[Id_Dispositivo], s.[MeasureId], s.[MeasureName], s.[MeasureUnit], s.[CreatedAt]) ")
                sql.AppendLine("	OUTPUT $action INTO @ElencoOperazioni; ")
                sql.AppendLine()
                sql.AppendLine("SELECT Operazione, COUNT(*) AS ContaOperazioni FROM @ElencoOperazioni GROUP BY Operazione")

                Dim DT As DataTable = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

                For Each r In DT.Rows
                    If r("Operazione").ToString.ToUpper = "INSERT" Then
                        nInsert = r("ContaOperazioni")
                    End If
                Next

            End If

        Catch ex As Exception

            nInsert = 0
            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return nInsert
    End Function

    Public Function Upsert_Acmotec_Dati(ByVal hlp As Upsert_Helper, ByVal ObjParametri_Server As AgronicaCoreParametri) As Integer

        Dim NomeRoutine As String = "IoT.Upsert_Acmotec_Dati"
        Dim nInsert As Integer = 0

        Try

            If hlp.Count > 0 Then

                Dim sql As New StringBuilder

                sql.Clear()

                sql.AppendLine("SET NOCOUNT ON ")
                sql.AppendLine()
                sql.AppendLine("DECLARE @TMPTABLE TABLE(Id_Dispositivo CHAR(32), MeasureId CHAR(50), DataOraUTC DATETIME, Valore REAL) ")
                sql.AppendLine("INSERT INTO @TMPTABLE VALUES ")
                sql.Append(hlp.GetAndReset())
                sql.AppendLine()
                sql.AppendLine("DECLARE @SRCTABLE TABLE(Id_Sensore INT, DataOraUTC DATETIME, Valore REAL) ")
                sql.AppendLine("INSERT INTO @SRCTABLE ")
                sql.AppendLine("SELECT Id, DataOraUTC, Valore FROM (")
                sql.AppendLine("    SELECT Id, DataOraUTC, Valore, NRow = ROW_NUMBER() OVER(PARTITION BY Id, DataOraUTC ORDER BY Id, DataOraUTC) ")
                sql.AppendLine("	FROM @TMPTABLE tt ")
                sql.AppendLine("    INNER JOIN Acmotec_Sensori s ON s.Id_Dispositivo = tt.Id_Dispositivo AND s.MeasureId = tt.MeasureId ")
                sql.AppendLine(") T WHERE NRow = 1")
                sql.AppendLine()
                sql.AppendLine("DECLARE @ElencoOperazioni TABLE(Operazione VARCHAR(20)); ")
                sql.AppendLine()
                sql.AppendLine("MERGE INTO Acmotec_Dati AS t ")
                sql.AppendLine("	USING ")
                sql.AppendLine("	(SELECT * FROM @SRCTABLE) AS s ")
                sql.AppendLine("    ON t.Id_Sensore = s.Id_Sensore AND t.DataOraUTC = s.DataOraUTC ")
                sql.AppendLine("	WHEN MATCHED THEN ")
                sql.AppendLine("		UPDATE SET Valore = s.Valore, Elaborato = 0 ")
                sql.AppendLine("	WHEN NOT MATCHED THEN ")
                sql.AppendLine("		INSERT (Id_Sensore, DataOraUTC, Valore, Elaborato) ")
                sql.AppendLine("		VALUES (s.Id_Sensore, s.DataOraUTC, s.Valore, 0) ")
                sql.AppendLine("	OUTPUT $action INTO @ElencoOperazioni; ")
                sql.AppendLine()
                sql.AppendLine("SELECT Operazione, COUNT(*) AS ContaOperazioni FROM @ElencoOperazioni GROUP BY Operazione ")

                Dim DT As DataTable = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

                For Each r In DT.Rows
                    If r("Operazione").ToString.ToUpper = "INSERT" Then
                        nInsert = r("ContaOperazioni")
                    End If
                Next

            End If

        Catch ex As Exception

            nInsert = 0
            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return nInsert
    End Function

    Public Function DispositiviXScaricoAcmotec(ByVal defDataora As DateTime, ByVal ObjParametri_Server As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "IoT.DispositiviXScaricoAcmotec"
        Dim DT As DataTable = Nothing

        Try

            Dim sql As New StringBuilder
            sql.Clear()

            sql.AppendLine("DECLARE @DEF_DATA DATETIME = " & Agro_SQL_SaveDate(defDataora))
            sql.AppendLine()
            sql.AppendLine("SELECT Id_Dispositivo = TRIM(Id_Dispositivo), UltimaDataOra = MAX(UltimaDataOra) FROM ( ")
            sql.AppendLine("	SELECT Id_Dispositivo, UltimaDataOra = CASE WHEN StazioneDataOra > UltimaDataOra THEN StazioneDataOra ELSE UltimaDataOra END FROM ( ")
            sql.AppendLine("	    SELECT sens.Id_Dispositivo, UltimaDataOra = COALESCE(dati.UltimaDataOra, @DEF_DATA), StazioneDataOra = COALESCE(staz.CreatedAt, @DEF_DATA) ")
            sql.AppendLine("	    FROM Acmotec_Sensori sens ")
            sql.AppendLine("	    INNER JOIN Acmotec_Dispositivi staz ON staz.guid = sens.Id_Dispositivo AND staz.FlagScaricoDati = 1 ")
            sql.AppendLine("	    LEFT JOIN ( ")
            sql.AppendLine("		    SELECT Id_Sensore, UltimaDataOra = MAX(DataOraUTC) FROM Acmotec_Dati GROUP BY Id_Sensore ")
            sql.AppendLine("	    ) dati ON dati.Id_Sensore = sens.Id ")
            sql.AppendLine("    ) T")
            sql.AppendLine(") T GROUP BY Id_Dispositivo ")

            DT = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

        Catch ex As Exception

            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return DT
    End Function
#End Region


End Class
