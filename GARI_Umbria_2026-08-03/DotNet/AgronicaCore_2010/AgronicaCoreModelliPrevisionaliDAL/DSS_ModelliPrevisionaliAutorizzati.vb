Public Class DSS_ModelliPrevisionaliAutorizzati_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function LeggiAutorizzati(ByVal Piva As String, ByVal Veg_Cod As Integer, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "DSS_ModelliPrevisionaliAutorizzati_R.LeggiAutorizzati()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim sql As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            sql.Length = 0

            sql.AppendLine("DECLARE @PIVA AS VARCHAR(20) = '" & Agro_SQL_SaveText(Piva) & "' ")
            sql.AppendLine()
            sql.AppendLine("SELECT ")
            sql.AppendLine("	sv.Veg_Cod ")
            sql.AppendLine("	, sv.Veg_Des ")
            sql.AppendLine("	, mp.Mod_Cod ")
            sql.AppendLine("	, mp.Mod_Ref_Code ")
            sql.AppendLine("	, Mod_Des = '' ")
            sql.AppendLine("	, Alg_Cod = 0 ")
            sql.AppendLine("	, Alg_Des = '' ")
            sql.AppendLine("	, Mod_Des_Agg = COALESCE(mp.DescrizioneAggiuntiva, '') ")
            sql.AppendLine("	, Av_Cod = 0 ")
            sql.AppendLine("	, Av_Des = '' ")
            sql.AppendLine("	, Av_Des_Lat = '' ")
            sql.AppendLine("	, IndicatoreDisponibile = 1 ")
            sql.AppendLine("FROM (")
            sql.AppendLine("	SELECT Veg_Cod, Mod_Cod, Mod_Ref_Code, DescrizioneAggiuntiva ")
            sql.AppendLine("	FROM DSS_ModelliPrevisionaliAutorizzati ")
            sql.AppendLine("	WHERE Tipo_Visibilita = 0 ")
            sql.AppendLine()
            sql.AppendLine("	UNION ")
            sql.AppendLine()
            sql.AppendLine("	SELECT mp0.Veg_Cod, mp0.Mod_Cod, mp0.Mod_Ref_Code, mp0.DescrizioneAggiuntiva ")
            sql.AppendLine("	FROM DSS_ModelliPrevisionaliAutorizzati_Imprese mp0 ")
            sql.AppendLine("	INNER JOIN DSS_ModelliPrevisionaliAutorizzati mp1 ON mp1.Veg_Cod = mp0.Veg_Cod AND mp1.Mod_Cod = mp0.Mod_Cod AND mp1.Tipo_Visibilita = 1 ")
            sql.AppendLine("	WHERE mp0.Piva = @PIVA AND mp0.Tipo_Visibilita = 0 ")
            sql.AppendLine(") mp ")
            sql.AppendLine("INNER JOIN SpecieVegetali sv ON sv.Veg_Cod = mp.Veg_Cod ")

            If Veg_Cod > 0 Then
                sql.AppendLine("    AND sv.Veg_Cod = " & Veg_Cod.ToString)
            End If

            sql.AppendLine("ORDER BY Veg_Cod, Mod_Cod, Av_Cod, Alg_Cod")

            DT = EseguiQuery_Lettura(objParametri, sql.ToString, NomeRoutine)

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT
    End Function

    Public Function LeggiTutti(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "DSS_ModelliPrevisionaliAutorizzati_R.LeggiTutti()"

        Dim DT As DataTable

        Try

            Dim sql As New System.Text.StringBuilder

            sql.Clear()

            sql.AppendLine("SELECT ")
            sql.AppendLine("	sv.Veg_Cod ")
            sql.AppendLine("	, sv.Veg_Des ")
            sql.AppendLine("	, mp.Mod_Cod ")
            sql.AppendLine("	, mp.Mod_Ref_Code ")
            sql.AppendLine("	, Mod_Des = '' ")
            sql.AppendLine("	, Alg_Cod = 0 ")
            sql.AppendLine("	, Alg_Des = '' ")
            sql.AppendLine("	, Mod_Des_Agg = COALESCE(mp.DescrizioneAggiuntiva, '') ")
            sql.AppendLine("	, Av_Cod = 0 ")
            sql.AppendLine("	, Av_Des = '' ")
            sql.AppendLine("	, Av_Des_Lat = '' ")
            sql.AppendLine("FROM DSS_ModelliPrevisionaliAutorizzati mp ")
            sql.AppendLine("INNER JOIN SpecieVegetali sv ON sv.Veg_Cod = mp.Veg_Cod ")
            sql.AppendLine("ORDER BY Veg_Cod, Mod_Cod, Av_Cod, Alg_Cod")

            DT = EseguiQuery_Lettura(objParametri, sql.ToString, NomeRoutine)

        Catch ex As Exception

            Dim MessaggioErrore As String = ex.Message

            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)

            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT
    End Function

End Class
