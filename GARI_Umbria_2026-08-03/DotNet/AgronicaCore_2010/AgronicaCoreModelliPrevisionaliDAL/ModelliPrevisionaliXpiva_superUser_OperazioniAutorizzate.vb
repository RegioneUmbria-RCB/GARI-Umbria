
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class ModelliPrevisionaliXpiva_superUser_OperazioniAutorizzate_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(
        ByVal Modello_Cod As Integer,
        ByVal Piva_SuperUser As String,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.AppendLine(" Select Mod_Cod, Piva_SuperUser, Tipo_Visibilita, DescrizioneAggiuntiva ")
            Stb.AppendLine(" From ModelliPrevisionaliXpiva_superUser_OperazioniAutorizzate ")
            Stb.AppendLine(" where piva_SuperUser  = '" & Agro_SQL_SaveText(Piva_SuperUser) & "'")

            If Modello_Cod <> 0 Then
                Stb.AppendLine(" and Mod_Cod = " & Modello_Cod)
            End If


            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    Stb.Append(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    Stb.Append(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

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

    Public Function LeggiAutorizzati(ByVal Piva_Superuser As String, ByVal Piva As String, ByVal Veg_Cod As Integer, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.LeggiAutorizzati()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim sql As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            sql.Length = 0

            sql.AppendLine("DECLARE @PIVA_SuperUser AS VARCHAR(20) = '" & Piva_Superuser & "' ")
            sql.AppendLine("DECLARE @PIVA AS VARCHAR(20) = '" & Agro_SQL_SaveText(Piva) & "' ")
            sql.AppendLine()
            sql.AppendLine("SELECT ")
            sql.AppendLine("	sv.Veg_Cod ")
            sql.AppendLine("	, sv.Veg_Des ")
            sql.AppendLine("	, mp.Mod_Cod ")
            sql.AppendLine("	, mp.Mod_Des ")
            sql.AppendLine("	, Alg_Cod = COALESCE(alg.Algoritmo_Cod, 0) ")
            sql.AppendLine("	, Alg_Des = COALESCE(alg.Algoritmo_Des, '') ")
            sql.AppendLine("	, Mod_Des_Agg = COALESCE(OpAut.DescrizioneAggiuntiva, '') ")
            sql.AppendLine("	, avv.Av_Cod ")
            sql.AppendLine("	, Av_Des = avv.Av_Des_Vol ")
            sql.AppendLine("	, Av_Des_Lat = avv.Av_Des_Lat ")
            sql.AppendLine("	, mp.IndicatoreDisponibile ")
            sql.AppendLine("FROM (")
            sql.AppendLine("	SELECT Mod_Cod, DescrizioneAggiuntiva ")
            sql.AppendLine("	FROM ModelliPrevisionaliXpiva_superUser_OperazioniAutorizzate ")
            sql.AppendLine("	WHERE Piva_SuperUser = @PIVA_SuperUser AND Tipo_Visibilita = 0 ")
            sql.AppendLine()
            sql.AppendLine("	UNION ")
            sql.AppendLine()
            sql.AppendLine("	SELECT OpAut0.Mod_Cod, OpAut0.DescrizioneAggiuntiva ")
            sql.AppendLine("	FROM ModelliPrevisionaliXpiva_OperazioniAutorizzate OpAut0 ")
            sql.AppendLine("	INNER JOIN ModelliPrevisionaliXpiva_superUser_OperazioniAutorizzate OpAut1 ON OpAut1.Piva_SuperUser = OpAut0.Piva_SuperUser AND OpAut1.Mod_Cod = OpAut0.Mod_Cod AND OpAut1.Tipo_Visibilita = 1 ")
            sql.AppendLine("	WHERE OpAut0.Piva_SuperUser = @PIVA_SuperUser AND OpAut0.Piva = @PIVA AND OpAut0.Tipo_Visibilita = 0 ")
            sql.AppendLine(") OpAut ")
            sql.AppendLine("INNER JOIN ModelliPrevisionali mp ON mp.Mod_Cod = OpAut.Mod_Cod ")
            sql.AppendLine("LEFT JOIN ModelliPrevisionali_Algoritmo alg ON alg.modello = mp.Mod_Cod ")
            sql.AppendLine("INNER JOIN ModellixSpecieXAvversita msa ON msa.Mod_Cod = mp.Mod_Cod ")
            sql.AppendLine("INNER JOIN SpecieVegetali sv ON sv.Veg_Cod = msa.Veg_Cod ")
            sql.AppendLine("INNER JOIN Avversita avv ON avv.Av_Cod = msa.Av_Cod ")
            sql.AppendLine("WHERE msa.Attivo = 1 ")

            If Veg_Cod > 0 Then
                sql.AppendLine("    AND sv.Veg_Cod = " & Veg_Cod.ToString)
            End If

            '--------------------------------------------------------------------------
            'Select Case objParametri.FlagVisibilita
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
            '        Stb.Append(" AND   Inviato >=0 ")
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
            '        Stb.Append(" AND   Inviato =-1 ")
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
            '        '...................................
            '    Case Else
            '        Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            'End Select
            '--------------------------------------------------------------------------

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

    Public Function LeggiTutti(Piva_Superuser As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCore_DAL.LeggiTutti()"

        Dim DT As DataTable

        Try

            Dim sql As New System.Text.StringBuilder

            sql.Clear()

            sql.AppendLine("DECLARE @PIVA_SuperUser AS VARCHAR(20) = '" & Piva_Superuser & "' ")
            sql.AppendLine()
            sql.AppendLine("SELECT ")
            sql.AppendLine("	sv.Veg_Cod ")
            sql.AppendLine("	, sv.Veg_Des ")
            sql.AppendLine("	, mp.Mod_Cod ")
            sql.AppendLine("	, mp.Mod_Des ")
            sql.AppendLine("	, Alg_Cod = COALESCE(alg.Algoritmo_Cod, 0) ")
            sql.AppendLine("	, Alg_Des = COALESCE(alg.Algoritmo_Des, '') ")
            sql.AppendLine("	, Mod_Des_Agg = COALESCE(OpAut.DescrizioneAggiuntiva, '') ")
            sql.AppendLine("	, avv.Av_Cod ")
            sql.AppendLine("	, Av_Des = avv.Av_Des_Vol ")
            sql.AppendLine("	, Av_Des_Lat = avv.Av_Des_Lat ")
            sql.AppendLine("FROM ModelliPrevisionaliXpiva_superUser_OperazioniAutorizzate OpAut ")
            sql.AppendLine("INNER JOIN ModelliPrevisionali mp ON mp.Mod_Cod = OpAut.Mod_Cod ")
            sql.AppendLine("LEFT JOIN ModelliPrevisionali_Algoritmo alg ON alg.modello = mp.Mod_Cod ")
            sql.AppendLine("INNER JOIN ModellixSpecieXAvversita msa ON msa.Mod_Cod = mp.Mod_Cod ")
            sql.AppendLine("INNER JOIN SpecieVegetali sv ON sv.Veg_Cod = msa.Veg_Cod ")
            sql.AppendLine("INNER JOIN Avversita avv ON avv.Av_Cod = msa.Av_Cod ")
            sql.AppendLine("WHERE Piva_SuperUser = @PIVA_SuperUser AND msa.Attivo = 1 ")
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

