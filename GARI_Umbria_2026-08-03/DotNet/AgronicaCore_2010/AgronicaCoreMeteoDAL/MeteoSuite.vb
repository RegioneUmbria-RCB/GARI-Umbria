

Public Class MeteoSuite_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function LeggiSorgentiMeteo(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.AppendLine(" declare @MeteoNT_Sorgenti table (Id int, Descrizione varchar(MAX), Ordine int) ")
            Stb.AppendLine(" insert into @MeteoNT_Sorgenti values ")
            Stb.AppendLine(" 	(0, 'Gias (Stazioni Regione E.R.)', 0), ")
            Stb.AppendLine(" 	(1, 'Tutte le stazioni in visibilità', 4), ")
            Stb.AppendLine(" 	(2, 'Solo le stazioni aziendali', 5), ")
            Stb.AppendLine(" 	(3, 'Gias (Quadranti Regione E.R.)', 2), ")
            Stb.AppendLine(" 	(4, 'Stazioni Pubbliche', 3) ")
            Stb.AppendLine()
            Stb.AppendLine(" ;WITH SORGENTI_CTE AS ( ")
            Stb.AppendLine("	SELECT Id_Sorgente = s.Id, Descrizione = COALESCE(v.Alias, s.Descrizione), Ordine, Scope = IIF(v.Id_Sorgente IS NULL, 1, 0) ")
            Stb.AppendLine("	FROM @MeteoNT_Sorgenti s ")
            Stb.AppendLine("	LEFT JOIN MeteoSuite_VisibilitaSorgenti v ON v.Id_Sorgente = s.Id ")
            Stb.AppendLine(" ) ")
            Stb.AppendLine()
            Stb.AppendLine(" SELECT Id = Id_Sorgente, Descrizione ")
            Stb.AppendLine(" FROM SORGENTI_CTE ")
            Stb.AppendLine(" WHERE Scope = (SELECT MIN(Scope) FROM SORGENTI_CTE) ")
            Stb.AppendLine(" ORDER BY Ordine ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function


End Class
