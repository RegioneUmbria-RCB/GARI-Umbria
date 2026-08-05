
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports System.Runtime.Serialization
Imports System.Text


#If False Then

Public Class DSS_Stazioni_X_Modelli_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal piva As String, ByVal xFiltroAggiuntivo As String, ByVal xOrderBy As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "DSS_Stazioni_X_Modelli_R.Leggi()"

        Dim DT As DataTable = Nothing

        Try

            Dim sql As New Text.StringBuilder

            sql.Clear()
            sql.AppendLine("SELECT  ")
            sql.AppendLine("    s.Tipo_Sorgente ")
            sql.AppendLine("    , Stazione_Cod ")
            sql.AppendLine("    , s.Mod_Cod ")
            sql.AppendLine("    , s.Veg_Cod ")
            sql.AppendLine("    , s.Avv_Cod ")
            sql.AppendLine("    , s.Alg_Cod ")
            sql.AppendLine("    , ParametriElaborazione ")
            sql.AppendLine("    , InizioPeriodo_gg ")
            sql.AppendLine("    , FinePeriodo_gg ")
            sql.AppendLine("    , Validita_minuti ")
            sql.AppendLine("	, DatiMeteoInizio_gg = COALESCE(i.DatiMeteoInizio_gg, 1) ")
            sql.AppendLine("	, DatiMeteoFine_gg = COALESCE(DatiMeteoFine_gg, 365) ")
            sql.AppendLine("FROM DSS_Stazioni_X_Modelli s ")
            sql.AppendLine("LEFT JOIN DSS_ModelliImpostazioni i ON i.Mod_Cod = s.Mod_Cod AND i.Veg_Cod = s.Veg_Cod AND i.Avv_Cod = s.Avv_Cod AND i.Alg_Cod = s.Alg_Cod ")

            If String.IsNullOrEmpty(xOrderBy) Then
                sql.AppendLine("INNER JOIN ( ")
                sql.AppendLine("    SELECT O.* FROM (VALUES (1, 2), (2, 1), (3, 3), (4, 0)) O(ordine, tipo_sorgente) ")
                sql.AppendLine(") o ON o.tipo_sorgente = s.Tipo_Sorgente ")
            End If

            sql.AppendLine("WHERE s.piva = '" & Agro_SQL_SaveText(piva) & "'")

            If xFiltroAggiuntivo <> "" Then
                sql.AppendLine("AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    sql.AppendLine("AND s.Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    sql.AppendLine("AND s.Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If String.IsNullOrEmpty(xOrderBy) Then
                sql.AppendLine("ORDER BY o.ordine ")
            Else
                sql.AppendLine("ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            DT = EseguiQuery_Lettura(objParametri, sql.ToString, NomeRoutine)

        Catch ex As Exception

            Dim MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing

            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT
    End Function

End Class


Public Class DSS_Stazioni_X_Irriga_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal piva As String, ByVal xFiltroAggiuntivo As String, ByVal xOrderBy As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "DSS_Stazioni_X_Irriga_R.Leggi()"

        Dim DT As DataTable = Nothing

        Try

            Dim sql As New Text.StringBuilder

            sql.Clear()
            sql.AppendLine("SELECT  ")
            sql.AppendLine("    s.Tipo_Sorgente ")
            sql.AppendLine("    , Stazione_Cod ")
            sql.AppendLine("    , s.Sa_Cod ")
            sql.AppendLine("    , ca.Sa_Nome ")
            sql.AppendLine("FROM DSS_Stazioni_X_Irriga s ")
            sql.AppendLine("INNER JOIN Centri_Aziendali ca ON ca.PIVA = s.PIVA AND ca.Sa_Cod = s.Sa_Cod ")
            If String.IsNullOrEmpty(xOrderBy) Then
                sql.AppendLine("INNER JOIN ( ")
                sql.AppendLine("    SELECT O.* FROM (VALUES (1, 2), (2, 1), (3, 3), (4, 0)) O(ordine, tipo_sorgente) ")
                sql.AppendLine(") o ON o.tipo_sorgente = s.Tipo_Sorgente ")
            End If

            sql.AppendLine("WHERE s.piva = '" & Agro_SQL_SaveText(piva) & "'")

            If xFiltroAggiuntivo <> "" Then
                sql.AppendLine("AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    sql.AppendLine("AND s.Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    sql.AppendLine("AND s.Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If String.IsNullOrEmpty(xOrderBy) Then
                sql.AppendLine("ORDER BY o.ordine ")
            Else
                sql.AppendLine("ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            DT = EseguiQuery_Lettura(objParametri, sql.ToString, NomeRoutine)

        Catch ex As Exception

            Dim MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing

            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT
    End Function

    Public Function LeggiBlocco(ByVal piva As String, ByVal tipo_sorgente As Integer, ByVal stazione_cod As Integer, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "DSS_Stazioni_X_Irriga_R.LeggiBlocco()"

        Dim DT As DataTable = Nothing

        Try

            Dim sql As New Text.StringBuilder

            sql.Clear()
            sql.AppendLine("SELECT Sa_Cod ")
            sql.AppendLine("FROM DSS_Stazioni_X_Irriga ")
            sql.AppendLine("WHERE PIVA = '" & Agro_SQL_SaveText(piva) & "'")
            sql.AppendLine("AND Tipo_Sorgente = " & tipo_sorgente)
            sql.AppendLine("AND Stazione_Cod = " & stazione_cod)

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    sql.AppendLine("AND Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    sql.AppendLine("AND Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            DT = EseguiQuery_Lettura(objParametri, sql.ToString, NomeRoutine)

        Catch ex As Exception

            Dim MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing

            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function
End Class


Public Class DSS_Stazioni_X_Monitor_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal piva As String, ByVal xFiltroAggiuntivo As String, ByVal xOrderBy As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "DSS_Stazioni_X_Monitor_R.Leggi()"

        Dim DT As DataTable = Nothing

        Try

            Dim sql As New Text.StringBuilder

            sql.Clear()
            sql.AppendLine("SELECT  ")
            sql.AppendLine("    s.Tipo_Sorgente ")
            sql.AppendLine("    , Stazione_Cod ")
            sql.AppendLine("    , Monitor_Ore ")
            sql.AppendLine("FROM DSS_Stazioni_X_Monitor s ")

            If String.IsNullOrEmpty(xOrderBy) Then
                sql.AppendLine("INNER JOIN ( ")
                sql.AppendLine("    SELECT O.* FROM (VALUES (1, 2), (2, 1), (3, 3), (4, 0)) O(ordine, tipo_sorgente) ")
                sql.AppendLine(") o ON o.tipo_sorgente = s.Tipo_Sorgente ")
            End If

            sql.AppendLine("WHERE PIVA = '" & Agro_SQL_SaveText(piva) & "'")

            If xFiltroAggiuntivo <> "" Then
                sql.AppendLine("AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    sql.AppendLine("AND Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    sql.AppendLine("AND Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If String.IsNullOrEmpty(xOrderBy) Then
                sql.AppendLine("ORDER BY o.ordine ")
            Else
                sql.AppendLine("ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            DT = EseguiQuery_Lettura(objParametri, sql.ToString, NomeRoutine)

        Catch ex As Exception

            Dim MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing

            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT
    End Function

End Class


Public Class DSS_Stazioni_X_Controllo_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal piva As String, ByVal xFiltroAggiuntivo As String, ByVal xOrderBy As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "DSS_Stazioni_X_Controllo_R.Leggi()"

        Dim DT As DataTable = Nothing

        Try

            Dim sql As New Text.StringBuilder

            sql.Clear()
            sql.AppendLine("SELECT  ")
            sql.AppendLine("    Tipo_Sorgente ")
            sql.AppendLine("    , Stazione_Cod ")
            sql.AppendLine("    , Parametri ")
            sql.AppendLine("FROM DSS_Stazioni_X_Controllo s ")

            If String.IsNullOrEmpty(xOrderBy) Then
                sql.AppendLine("INNER JOIN ( ")
                sql.AppendLine("    SELECT O.* FROM (VALUES (1, 2), (2, 1), (3, 3), (4, 0)) O(ordine, sorgente) ")
                sql.AppendLine(") o ON o.sorgente = s.Tipo_Sorgente ")
            End If

            sql.AppendLine("WHERE PIVA = '" & Agro_SQL_SaveText(piva) & "'")

            If xFiltroAggiuntivo <> "" Then
                sql.AppendLine("AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    sql.AppendLine("AND Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    sql.AppendLine("AND Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If String.IsNullOrEmpty(xOrderBy) Then
                sql.AppendLine("ORDER BY o.ordine ")
            Else
                sql.AppendLine("ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            DT = EseguiQuery_Lettura(objParametri, sql.ToString, NomeRoutine)

        Catch ex As Exception

            Dim MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing

            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT
    End Function

End Class


Public Class DSS_Stazioni_X_Applicazioni
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function LeggiApplicazioni(ByVal Id_Stazione As Integer, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "DSS_Stazioni_X_Applicazioni.LeggiApplicazioni()"

        Dim DT As DataTable = Nothing

        Try

            Dim sql As New Text.StringBuilder

            sql.Clear()

            sql.AppendLine("DECLARE @STAZ_ID AS INTEGER = " & Id_Stazione)
            sql.AppendLine()
            sql.AppendLine("SELECT Applic, PIVA FROM ( ")
            sql.AppendLine("	SELECT Applic = 'Modelli', PIVA, inviato FROM DSS_Stazioni_X_Modelli WHERE Stazione_Cod = @STAZ_ID ")
            sql.AppendLine("	UNION ")
            sql.AppendLine("	SELECT Applic = 'Irriga', PIVA, inviato FROM DSS_Stazioni_X_Irriga WHERE Stazione_Cod = @STAZ_ID ")
            sql.AppendLine("	UNION ")
            sql.AppendLine("	SELECT Applic = 'Monitor', PIVA, inviato FROM DSS_Stazioni_X_Monitor WHERE Stazione_Cod = @STAZ_ID ")
            sql.AppendLine("	UNION ")
            sql.AppendLine("	SELECT Applic = 'Controllo', PIVA, inviato FROM DSS_Stazioni_X_Controllo WHERE Stazione_Cod = @STAZ_ID ")
            sql.AppendLine(") A ")
            sql.AppendLine("WHERE 1 = 1 ")

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    sql.AppendLine("AND Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    sql.AppendLine("AND Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            DT = EseguiQuery_Lettura(objParametri, sql.ToString, NomeRoutine)

        Catch ex As Exception

            Dim MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing

            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT
    End Function
End Class


#End If









Public Class DSS_Stazioni_X_Applicazioni_Reader
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function LeggiApplicazioni(Id_Stazione As Integer, objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "DSS_Stazioni_X_Applicazioni_Reader.LeggiApplicazioni()"

        Try

            Dim sql As New Text.StringBuilder

            sql.Clear()

            sql.AppendLine("DECLARE @STAZ_ID AS INTEGER = " & Id_Stazione)
            sql.AppendLine()
            sql.AppendLine("SELECT Applic, PIVA FROM ( ")
            sql.AppendLine("	SELECT Applic = 'Modelli', PIVA, inviato FROM DSS_Stazioni_X_Modelli WHERE Stazione_Cod = @STAZ_ID ")
            sql.AppendLine("	UNION ")
            sql.AppendLine("	SELECT Applic = 'Irriga', PIVA, inviato FROM DSS_Stazioni_X_Irriga WHERE Stazione_Cod = @STAZ_ID ")
            sql.AppendLine("	UNION ")
            sql.AppendLine("	SELECT Applic = 'Monitor', PIVA, inviato FROM DSS_Stazioni_X_Monitor WHERE Stazione_Cod = @STAZ_ID ")
            sql.AppendLine("	UNION ")
            sql.AppendLine("	SELECT Applic = 'Controllo', PIVA, inviato FROM DSS_Stazioni_X_Controllo WHERE Stazione_Cod = @STAZ_ID ")
            sql.AppendLine(") A ")
            sql.AppendLine("WHERE 1 = 1 ")

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    sql.AppendLine("AND Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    sql.AppendLine("AND Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            Dim DT = EseguiQuery_Lettura(objParametri, sql.ToString, NomeRoutine)

            Return DT

        Catch ex As Exception

            Dim MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)

            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Nothing
    End Function

    Protected Function LeggiOrdinato(staz_sql As StringBuilder, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, NomeRoutine As String) As DataTable

        Dim sql As New StringBuilder

        sql.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
        sql.AppendLine("; WITH STAZIONI_CTE AS (")
        sql.Append(staz_sql.ToString)
        sql.AppendLine(")")
        sql.AppendLine()
        sql.AppendLine("SELECT S.* FROM STAZIONI_CTE S")
        sql.AppendLine("INNER JOIN ( ")
        sql.Append("    SELECT O.* FROM (VALUES ")
        sql.Append("(1, " + CInt(enum_Meteo_Tiposorgente.Aziendali).ToString + "), ")
        sql.Append("(2, " + CInt(enum_Meteo_Tiposorgente.RetiPartner).ToString + "), ")
        sql.Append("(3, " + CInt(enum_Meteo_Tiposorgente.Pubbliche).ToString + "), ")
        sql.Append("(4, " + CInt(enum_Meteo_Tiposorgente.Gias_RER_Quadranti).ToString + "), ")
        sql.Append("(5, " + CInt(enum_Meteo_Tiposorgente.Gias_RER).ToString + ")")
        sql.AppendLine(") O(ordine, tipo_sorgente) ")
        sql.AppendLine(") O ON O.tipo_sorgente = S.Tipo_Sorgente ")
        sql.AppendLine("ORDER BY O.ordine ")

        Dim DT = EseguiQuery_Lettura(objParametri, sql.ToString, NomeRoutine)

        Return DT
    End Function

End Class


Public Class DSS_Stazioni_X_Modelli_Reader
    Inherits DSS_Stazioni_X_Applicazioni_Reader

    Public Class Filter
        Public TipoSorgente As Integer
        Public StazioneCod As Integer
    End Class

    Public Function Leggi(piva As String, filtroAggiuntivo As Filter, objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "DSS_Stazioni_X_Modelli_Reader.Leggi()"

        Try

            Dim sql As New StringBuilder

            sql.Clear()
            sql.AppendLine("SELECT  ")
            sql.AppendLine("    Tipo_Sorgente ")
            sql.AppendLine("    , Stazione_Cod ")
            sql.AppendLine("    , s.Mod_Cod ")
            sql.AppendLine("    , s.Veg_Cod ")
            sql.AppendLine("    , s.Avv_Cod ")
            sql.AppendLine("    , s.Alg_Cod ")
            sql.AppendLine("    , ParametriElaborazione ")
            sql.AppendLine("    , InizioPeriodo_gg ")
            sql.AppendLine("    , FinePeriodo_gg ")
            sql.AppendLine("    , Validita_minuti ")
            sql.AppendLine("	, DatiMeteoInizio_gg = COALESCE(i.DatiMeteoInizio_gg, 1) ")
            sql.AppendLine("	, DatiMeteoFine_gg = COALESCE(DatiMeteoFine_gg, 365) ")
            sql.AppendLine("FROM DSS_Stazioni_X_Modelli s ")
            sql.AppendLine("LEFT JOIN DSS_ModelliImpostazioni i ON i.Mod_Cod = s.Mod_Cod AND i.Veg_Cod = s.Veg_Cod AND i.Avv_Cod = s.Avv_Cod AND i.Alg_Cod = s.Alg_Cod ")
            sql.AppendLine("WHERE PIVA = '" & Agro_SQL_SaveText(piva) & "'")

            If filtroAggiuntivo IsNot Nothing AndAlso filtroAggiuntivo.TipoSorgente >= 0 Then

                sql.AppendLine("AND Tipo_Sorgente = " & filtroAggiuntivo.TipoSorgente.ToString)

                If filtroAggiuntivo.StazioneCod > 0 Then

                    sql.AppendLine("AND Stazione_Cod = " & filtroAggiuntivo.StazioneCod.ToString)
                End If
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    sql.AppendLine("AND s.Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    sql.AppendLine("AND s.Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            Dim DT = LeggiOrdinato(sql, objParametri, NomeRoutine)

            Return DT

        Catch ex As Exception

            Dim MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)

            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Nothing
    End Function

End Class


Public Class DSS_Stazioni_X_Irriga_Reader
    Inherits DSS_Stazioni_X_Applicazioni_Reader

    Public Class Filter
        Public CodiceCentro As Integer
        Public TipoSorgente As Integer
        Public StazioneCod As Integer
    End Class

    Public Function Leggi(piva As String, filtroAggiuntivo As Filter, objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "DSS_Stazioni_X_Irriga_R.Leggi()"

        Try

            Dim sql As New StringBuilder

            sql.Clear()
            sql.AppendLine("SELECT  ")
            sql.AppendLine("    Tipo_Sorgente ")
            sql.AppendLine("    , Stazione_Cod ")
            sql.AppendLine("    , s.Sa_Cod ")
            sql.AppendLine("    , ca.Sa_Nome ")
            sql.AppendLine("FROM DSS_Stazioni_X_Irriga s ")
            sql.AppendLine("INNER JOIN Centri_Aziendali ca ON ca.PIVA = s.PIVA AND ca.Sa_Cod = s.Sa_Cod ")
            sql.AppendLine("WHERE s.piva = '" & Agro_SQL_SaveText(piva) & "'")

            If filtroAggiuntivo IsNot Nothing Then

                If filtroAggiuntivo.TipoSorgente >= 0 Then

                    sql.AppendLine("AND Tipo_Sorgente = " & filtroAggiuntivo.TipoSorgente.ToString)

                    If filtroAggiuntivo.StazioneCod > 0 Then

                        sql.AppendLine("AND Stazione_Cod = " & filtroAggiuntivo.StazioneCod.ToString)
                    End If
                End If

                If filtroAggiuntivo.CodiceCentro > 0 Then

                    sql.AppendLine("AND ca.Sa_Cod = " & filtroAggiuntivo.CodiceCentro.ToString)
                End If
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    sql.AppendLine("AND s.Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    sql.AppendLine("AND s.Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            Dim DT = LeggiOrdinato(sql, objParametri, NomeRoutine)

            Return DT
        Catch ex As Exception

            Dim MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)

            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Nothing
    End Function

    Public Function LeggiBlocco(piva As String, tipo_sorgente As Integer, stazione_cod As Integer, objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "DSS_Stazioni_X_Irriga_R.LeggiBlocco()"

        Try

            Dim sql As New StringBuilder

            sql.Clear()
            sql.AppendLine("SELECT Sa_Cod ")
            sql.AppendLine("FROM DSS_Stazioni_X_Irriga ")
            sql.AppendLine("WHERE PIVA = '" & Agro_SQL_SaveText(piva) & "'")
            sql.AppendLine("AND Tipo_Sorgente = " & tipo_sorgente)
            sql.AppendLine("AND Stazione_Cod = " & stazione_cod)

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    sql.AppendLine("AND Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    sql.AppendLine("AND Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            Dim DT = EseguiQuery_Lettura(objParametri, sql.ToString, NomeRoutine)

            Return DT
        Catch ex As Exception

            Dim MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)

            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Nothing
    End Function

End Class


Public Class DSS_Stazioni_X_Monitor_Reader
    Inherits DSS_Stazioni_X_Applicazioni_Reader

    Public Function Leggi(piva As String, objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "DSS_Stazioni_X_Monitor_R.Leggi()"

        Try

            Dim sql As New StringBuilder

            sql.Clear()
            sql.AppendLine("SELECT  ")
            sql.AppendLine("    Tipo_Sorgente ")
            sql.AppendLine("    , Stazione_Cod ")
            sql.AppendLine("    , Monitor_Ore ")
            sql.AppendLine("FROM DSS_Stazioni_X_Monitor ")
            sql.AppendLine("WHERE PIVA = '" & Agro_SQL_SaveText(piva) & "'")

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    sql.AppendLine("AND Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    sql.AppendLine("AND Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            Dim DT = LeggiOrdinato(sql, objParametri, NomeRoutine)

            Return DT
        Catch ex As Exception

            Dim MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)

            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Nothing
    End Function

End Class


Public Class DSS_Stazioni_X_Controllo_Reader
    Inherits DSS_Stazioni_X_Applicazioni_Reader

    Public Class Filter
        Public TipoSorgente As Integer
        Public StazioneCod As Integer
    End Class

    Public Function Leggi(piva As String, filtroAggiuntivo As Filter, objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "DSS_Stazioni_X_Controllo_R.Leggi()"

        Try

            Dim sql As New StringBuilder

            sql.Clear()

            sql.AppendLine("SELECT  ")
            sql.AppendLine("    Tipo_Sorgente ")
            sql.AppendLine("    , Stazione_Cod ")
            sql.AppendLine("    , Parametri ")
            sql.AppendLine("FROM DSS_Stazioni_X_Controllo ")
            sql.AppendLine("WHERE PIVA = '" & Agro_SQL_SaveText(piva) & "'")

            If filtroAggiuntivo IsNot Nothing AndAlso filtroAggiuntivo.TipoSorgente >= 0 Then

                sql.AppendLine("AND Tipo_Sorgente = " & filtroAggiuntivo.TipoSorgente.ToString)

                If filtroAggiuntivo.StazioneCod > 0 Then

                    sql.AppendLine("AND Stazione_Cod = " & filtroAggiuntivo.StazioneCod.ToString)
                End If
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    sql.AppendLine("AND Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    sql.AppendLine("AND Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            Dim DT = LeggiOrdinato(sql, objParametri, NomeRoutine)

            Return DT
        Catch ex As Exception

            Dim MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)

            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Nothing
    End Function

End Class





Public Class DSS_Stazioni_X_Modelli_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(piva As String, Tipo_Sorgente As Integer, Stazione_Cod As Integer,
                           Mod_Cod As Integer, Veg_Cod As Integer, Avv_Cod As Integer, Alg_Cod As Integer,
                           ParametriElaborazione As String, InizioPeriodo_gg As Integer, FinePeriodo_gg As Integer, Validita_minuti As Integer,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean


        Dim NomeRoutine As String = "DSS_Stazioni_X_Modelli_W.Scrivi()"

        Dim xRisp As Boolean = False

        Try

            Dim sql As New Text.StringBuilder

            sql.Clear()
            sql.AppendLine("INSERT DSS_Stazioni_X_Modelli ")
            sql.AppendLine("([PIVA], [Tipo_Sorgente], [Stazione_Cod], [Mod_Cod], [Veg_Cod], [Avv_Cod], [Alg_Cod], [ParametriElaborazione], [InizioPeriodo_gg], [FinePeriodo_gg], [Validita_minuti], [inviato], [datainvio]) ")
            sql.AppendLine("VALUES ( ")
            sql.AppendLine("    '" & Agro_SQL_SaveText(piva) & "' ")
            sql.AppendLine("    , " & Agro_SQL_SaveNum(Tipo_Sorgente) & " ")
            sql.AppendLine("    , " & Agro_SQL_SaveNum(Stazione_Cod) & " ")
            sql.AppendLine("    , " & Agro_SQL_SaveNum(Mod_Cod) & " ")
            sql.AppendLine("    , " & Agro_SQL_SaveNum(Veg_Cod) & " ")
            sql.AppendLine("    , " & Agro_SQL_SaveNum(Avv_Cod) & " ")
            sql.AppendLine("    , " & Agro_SQL_SaveNum(Alg_Cod) & " ")
            sql.AppendLine("    , '" & Agro_SQL_SaveText(ParametriElaborazione) & "' ")
            sql.AppendLine("    , " & Agro_SQL_SaveNum(InizioPeriodo_gg) & " ")
            sql.AppendLine("    , " & Agro_SQL_SaveNum(FinePeriodo_gg) & " ")
            sql.AppendLine("    , " & Agro_SQL_SaveNum(Validita_minuti) & " ")
            sql.AppendLine("    , 0 ")
            sql.AppendLine("    , NULL ")
            sql.AppendLine(") ")

            xRisp = EseguiQuery_Scrittura(objParametri, sql.ToString, NomeRoutine)

        Catch ex As Exception

            Dim MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)

            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp
    End Function

    Public Function Cancella(piva As String, Tipo_Sorgente As Integer, Stazione_Cod As Integer,
                             Mod_Cod As Integer, Veg_Cod As Integer, Avv_Cod As Integer, Alg_Cod As Integer, ParametriElaborazione As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "DSS_Stazioni_X_Modelli_W.Cancella()"

        Dim xRisp As Boolean = False

        Try

            Dim sql As New Text.StringBuilder

            sql.Clear()

            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                sql.AppendLine("UPDATE DSS_Stazioni_X_Modelli ")
                sql.AppendLine("SET ")
                sql.AppendLine("    Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                sql.AppendLine("    , Data_Modifica = " & Agro_SQL_SaveDate(Date.Now) & " ")
                sql.AppendLine("    , Inviato = -1 ")
                sql.AppendLine("WHERE PIVA = '" & Agro_SQL_SaveText(piva) & "' ")
                sql.AppendLine("    AND Tipo_Sorgente = " & Tipo_Sorgente & " ")
                sql.AppendLine("    AND Stazione_Cod = " & Stazione_Cod & " ")
                sql.AppendLine("    AND Mod_Cod = " & Mod_Cod & " ")
                sql.AppendLine("    AND Veg_Cod = " & Veg_Cod & " ")
                sql.AppendLine("    AND Avv_Cod = " & Avv_Cod & " ")
                sql.AppendLine("    AND Alg_Cod = " & Alg_Cod & " ")
                sql.AppendLine("    AND ParametriElaborazione = '" & Agro_SQL_SaveText(ParametriElaborazione) & "' ")
                sql.AppendLine("    AND Inviato >= 0 ")

            Else

                sql.AppendLine("DELETE FROM DSS_Stazioni_X_Modelli ")
                sql.AppendLine("WHERE PIVA = '" & Agro_SQL_SaveText(piva) & "' ")
                sql.AppendLine("    AND Tipo_Sorgente = " & Tipo_Sorgente & " ")
                sql.AppendLine("    AND Stazione_Cod = " & Stazione_Cod & " ")
                sql.AppendLine("    AND Mod_Cod = " & Mod_Cod & " ")
                sql.AppendLine("    AND Veg_Cod = " & Veg_Cod & " ")
                sql.AppendLine("    AND Avv_Cod = " & Avv_Cod & " ")
                sql.AppendLine("    AND Alg_Cod = " & Alg_Cod & " ")
                sql.AppendLine("    AND ParametriElaborazione = '" & Agro_SQL_SaveText(ParametriElaborazione) & "' ")

            End If

            xRisp = EseguiQuery_Scrittura(objParametri, sql.ToString, NomeRoutine)

        Catch ex As Exception

            Dim MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)

            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp
    End Function

End Class


Public Class DSS_Stazioni_X_Irriga_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Class _key
        Public PIVA As String
        Public Tipo_Sorgente As Integer
        Public Stazione_Cod As Integer
        Public Sa_Cod As Integer
    End Class

    Public Function ScriviBlocco(keys2ins As List(Of _key), keys2del As List(Of _key), ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "DSS_Stazioni_X_Irriga_W.ScriviBlocco()"

        Dim xRisp As Boolean = False

        Try

            Dim sql As New Text.StringBuilder

            sql.Clear()

            sql.AppendLine("SET NOCOUNT ON ")
            sql.AppendLine()
            sql.AppendLine("DECLARE @FLAG_OK AS BIT = 1 ")
            sql.AppendLine()
            sql.AppendLine("BEGIN TRY ")
            sql.AppendLine()
            sql.AppendLine("    BEGIN TRANSACTION ")
            sql.AppendLine()

            For Each k In keys2del

                sql.AppendLine("    DELETE FROM DSS_Stazioni_X_Irriga ")
                sql.AppendLine("    WHERE PIVA = '" & Agro_SQL_SaveText(k.PIVA) & "' ")
                sql.AppendLine("        AND Tipo_Sorgente = " & k.Tipo_Sorgente)
                sql.AppendLine("        AND Stazione_Cod = " & k.Stazione_Cod)
                sql.AppendLine("        AND Sa_Cod = " & k.Sa_Cod)
                sql.AppendLine()

            Next

            If keys2ins.Any Then
                sql.AppendLine("    INSERT DSS_Stazioni_X_Irriga ")
                sql.AppendLine("    ([PIVA], [Tipo_Sorgente], [Stazione_Cod], [Sa_Cod], [inviato], [datainvio]) ")
                sql.AppendLine("    VALUES ")

                Dim comma As String = ""
                For Each k In keys2ins

                    Dim values As String = "'" & Agro_SQL_SaveText(k.PIVA) & "'"
                    values &= ", " & k.Tipo_Sorgente
                    values &= ", " & k.Stazione_Cod
                    values &= ", " & k.Sa_Cod
                    values &= ", 0"
                    values &= ", NULL"

                    sql.AppendLine("    " & comma & "(" & values & ")")
                    comma = ","
                Next

                sql.AppendLine()
            End If

            sql.AppendLine("    COMMIT TRANSACTION ")
            sql.AppendLine()
            sql.AppendLine("END TRY ")
            sql.AppendLine("BEGIN CATCH ")
            sql.AppendLine()
            sql.AppendLine("    ROLLBACK TRANSACTION ")
            sql.AppendLine()
            sql.AppendLine("    SET @FLAG_OK = 0 ")
            sql.AppendLine()
            sql.AppendLine("END CATCH ")
            sql.AppendLine()
            sql.AppendLine("SELECT Flag_OK = @FLAG_OK ")

            Dim DT = EseguiQuery_Lettura(objParametri, sql.ToString, NomeRoutine)

            If DT IsNot Nothing AndAlso DT.Rows.Count >= 1 Then

                xRisp = DT.Rows(0)("Flag_OK")
            End If

        Catch ex As Exception

            Dim MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)

            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp
    End Function

End Class


Public Class DSS_Stazioni_X_Monitor_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(piva As String, Tipo_Sorgente As Integer, Stazione_Cod As Integer, Monitor_Ore As Integer, IsNew As Boolean,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean


        Dim NomeRoutine As String = "DSS_Stazioni_X_Monitor_W.Scrivi()"

        Dim xRisp As Boolean = False

        Try

            Dim sql As New Text.StringBuilder

            sql.Clear()

            If IsNew Then

                sql.AppendLine("INSERT DSS_Stazioni_X_Monitor ")
                sql.AppendLine("([PIVA], [Tipo_Sorgente], [Stazione_Cod], [Monitor_Ore], [inviato], [datainvio]) ")
                sql.AppendLine("VALUES ( ")
                sql.AppendLine("    '" & Agro_SQL_SaveText(piva) & "'")
                sql.AppendLine("    , " & Agro_SQL_SaveNum(Tipo_Sorgente))
                sql.AppendLine("    , " & Agro_SQL_SaveNum(Stazione_Cod))
                sql.AppendLine("    , " & Agro_SQL_SaveNum(Monitor_Ore))
                sql.AppendLine("    , 0")
                sql.AppendLine("    , NULL")
                sql.AppendLine(")")

            Else

                sql.AppendLine("UPDATE DSS_Stazioni_X_Monitor ")
                sql.AppendLine("SET Monitor_Ore = " & Agro_SQL_SaveNum(Monitor_Ore))
                sql.AppendLine("WHERE PIVA = '" & Agro_SQL_SaveText(piva) & "' ")
                sql.AppendLine("    AND Tipo_Sorgente = " & Agro_SQL_SaveNum(Tipo_Sorgente))
                sql.AppendLine("    AND Stazione_Cod = " & Agro_SQL_SaveNum(Stazione_Cod))

            End If

            xRisp = EseguiQuery_Scrittura(objParametri, sql.ToString, NomeRoutine)

        Catch ex As Exception

            Dim MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)

            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp
    End Function

    Public Function Cancella(piva As String, Tipo_Sorgente As Integer, Stazione_Cod As Integer,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "DSS_Stazioni_X_Monitor_W.Cancella()"

        Dim xRisp As Boolean = False

        Try

            Dim sql As New Text.StringBuilder

            sql.Clear()

            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                sql.AppendLine("UPDATE DSS_Stazioni_X_Monitor ")
                sql.AppendLine("SET ")
                sql.AppendLine("    Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                sql.AppendLine("    , Data_Modifica = " & Agro_SQL_SaveDate(Date.Now) & " ")
                sql.AppendLine("    , Inviato = -1 ")
                sql.AppendLine("WHERE PIVA = '" & Agro_SQL_SaveText(piva) & "' ")
                sql.AppendLine("    AND Tipo_Sorgente = " & Tipo_Sorgente & " ")
                sql.AppendLine("    AND Stazione_Cod = " & Stazione_Cod & " ")
                sql.AppendLine("    AND Inviato >= 0 ")

            Else

                sql.AppendLine("DELETE FROM DSS_Stazioni_X_Monitor ")
                sql.AppendLine("WHERE PIVA = '" & Agro_SQL_SaveText(piva) & "' ")
                sql.AppendLine("    AND Tipo_Sorgente = " & Tipo_Sorgente & " ")
                sql.AppendLine("    AND Stazione_Cod = " & Stazione_Cod & " ")

            End If

            xRisp = EseguiQuery_Scrittura(objParametri, sql.ToString, NomeRoutine)

        Catch ex As Exception

            Dim MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)

            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp
    End Function

End Class


Public Class DSS_Stazioni_X_Controllo_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(piva As String, Tipo_Sorgente As Integer, Stazione_Cod As Integer, Parametri As String, IsNew As Boolean,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "DSS_Stazioni_X_Controllo_W.Scrivi()"

        Dim xRisp As Boolean = False

        Try

            Dim sql As New Text.StringBuilder

            sql.Clear()

            If IsNew Then

                sql.AppendLine("INSERT DSS_Stazioni_X_Controllo ")
                sql.AppendLine("([PIVA], [Tipo_Sorgente], [Stazione_Cod], [Parametri], [inviato], [datainvio]) ")
                sql.AppendLine("VALUES ( ")
                sql.AppendLine("    '" & Agro_SQL_SaveText(piva) & "'")
                sql.AppendLine("    , " & Agro_SQL_SaveNum(Tipo_Sorgente))
                sql.AppendLine("    , " & Agro_SQL_SaveNum(Stazione_Cod))
                sql.AppendLine("    , '" & Agro_SQL_SaveText(Parametri) & "'")
                sql.AppendLine("    , 0")
                sql.AppendLine("    , NULL")
                sql.AppendLine(")")

            Else

                sql.AppendLine("UPDATE DSS_Stazioni_X_Controllo ")
                sql.AppendLine("SET Parametri = '" & Agro_SQL_SaveText(Parametri) & "' ")
                sql.AppendLine("WHERE PIVA = '" & Agro_SQL_SaveText(piva) & "' ")
                sql.AppendLine("    AND Tipo_Sorgente = " & Agro_SQL_SaveNum(Tipo_Sorgente))
                sql.AppendLine("    AND Stazione_Cod = " & Agro_SQL_SaveNum(Stazione_Cod))

            End If

            xRisp = EseguiQuery_Scrittura(objParametri, sql.ToString, NomeRoutine)

        Catch ex As Exception

            Dim MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)

            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp
    End Function

    Public Function Cancella(piva As String, Tipo_Sorgente As Integer, Stazione_Cod As Integer,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "DSS_Stazioni_X_Controllo_W.Cancella()"

        Dim xRisp As Boolean = False

        Try

            Dim sql As New Text.StringBuilder

            sql.Clear()

            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                sql.AppendLine("UPDATE DSS_Stazioni_X_Controllo ")
                sql.AppendLine("SET ")
                sql.AppendLine("    Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                sql.AppendLine("    , Data_Modifica = " & Agro_SQL_SaveDate(Date.Now) & " ")
                sql.AppendLine("    , Inviato = -1 ")
                sql.AppendLine("WHERE PIVA = '" & Agro_SQL_SaveText(piva) & "' ")
                sql.AppendLine("    AND Tipo_Sorgente = " & Tipo_Sorgente & " ")
                sql.AppendLine("    AND Stazione_Cod = " & Stazione_Cod & " ")
                sql.AppendLine("    AND Inviato >= 0 ")

            Else

                sql.AppendLine("DELETE FROM DSS_Stazioni_X_Controllo ")
                sql.AppendLine("WHERE PIVA = '" & Agro_SQL_SaveText(piva) & "' ")
                sql.AppendLine("    AND Tipo_Sorgente = " & Tipo_Sorgente & " ")
                sql.AppendLine("    AND Stazione_Cod = " & Stazione_Cod & " ")

            End If

            xRisp = EseguiQuery_Scrittura(objParametri, sql.ToString, NomeRoutine)

        Catch ex As Exception

            Dim MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)

            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp
    End Function

End Class





Public Class DSS_ModelliImpostazioni
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(xFiltroAggiuntivo As String, xOrderBy As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "DSS_ModelliImpostazioni.Leggi()"

        Try

            Dim sql As New Text.StringBuilder

            sql.Clear()
            sql.AppendLine("SELECT Mod_Cod, Veg_Cod, Avv_Cod, Alg_Cod, DatiMeteoInizio_gg, DatiMeteoFine_gg ")
            sql.AppendLine("FROM DSS_ModelliImpostazioni ")
            sql.AppendLine("WHERE 1 = 1 ")
            If Not String.IsNullOrEmpty(xFiltroAggiuntivo) Then
                sql.AppendLine("AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    sql.AppendLine("AND Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    sql.AppendLine("AND Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If Not String.IsNullOrEmpty(xOrderBy) Then
                sql.AppendLine("ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            Dim DT = EseguiQuery_Lettura(objParametri, sql.ToString, NomeRoutine)

            Return DT
        Catch ex As Exception

            Dim MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)

            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Nothing
    End Function


    <DataContract>
    Public Class impostazioneModello
        <DataMember>
        Public Mod_Cod As Integer
        <DataMember>
        Public Veg_Cod As Integer
        <DataMember>
        Public Avv_Cod As Integer
        <DataMember>
        Public Alg_Cod As Integer
        <DataMember>
        Public InizioPeriodo_gg As Integer
        <DataMember>
        Public FinePeriodo_gg As Integer
    End Class

    Public Function Upsert(listaImpostazioni As List(Of impostazioneModello), ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "DSS_ModelliImpostazioni.Upsert()"

        Try

            Dim sql As New Text.StringBuilder

            sql.Clear()
            sql.AppendLine("SET NOCOUNT ON ")
            sql.AppendLine()
            sql.AppendLine("DECLARE @TMPTABLE TABLE(Mod_Cod INT, Veg_Cod INT, Avv_Cod INT, Alg_Cod INT, DatiMeteoInizio_gg INT, DatiMeteoFine_gg INT) ")
            sql.AppendLine()
            sql.AppendLine("INSERT INTO @TMPTABLE (Mod_Cod, Veg_Cod, Avv_Cod, Alg_Cod, DatiMeteoInizio_gg, DatiMeteoFine_gg) VALUES ")

            Dim comma As String = ""
            For Each imp In listaImpostazioni
                sql.AppendLine(comma &
                               "(" & imp.Mod_Cod.ToString & ", " &
                               imp.Veg_Cod.ToString & ", " &
                               imp.Avv_Cod.ToString & ", " &
                               imp.Alg_Cod.ToString & ", " &
                               imp.InizioPeriodo_gg.ToString & ", " &
                               imp.FinePeriodo_gg.ToString & ")")
                comma = ","
            Next

            sql.AppendLine()
            sql.AppendLine("DECLARE @ElencoOperazioni TABLE(Operazione VARCHAR(20)); ")
            sql.AppendLine()
            sql.AppendLine("MERGE INTO DSS_ModelliImpostazioni AS t ")
            sql.AppendLine("    USING ")
            sql.AppendLine("        (SELECT * FROM @TMPTABLE) AS s ")
            sql.AppendLine("    ON t.Mod_Cod = s.Mod_Cod AND t.Veg_Cod = s.Veg_Cod AND t.Avv_Cod = s.Avv_Cod AND t.Alg_Cod = s.Alg_Cod ")
            sql.AppendLine("	WHEN MATCHED THEN ")
            sql.AppendLine("		UPDATE SET DatiMeteoInizio_gg = s.DatiMeteoInizio_gg, DatiMeteoFine_gg = s.DatiMeteoFine_gg ")
            sql.AppendLine("	WHEN NOT MATCHED THEN ")
            sql.AppendLine("		INSERT (Mod_Cod, Veg_Cod, Avv_Cod, Alg_Cod, DatiMeteoInizio_gg, DatiMeteoFine_gg) ")
            sql.AppendLine("		VALUES (s.Mod_Cod, s.Veg_Cod, s.Avv_Cod, s.Alg_Cod, s.DatiMeteoInizio_gg, s.DatimeteoFine_gg) ")
            sql.AppendLine("	OUTPUT $action INTO @ElencoOperazioni; ")
            sql.AppendLine()
            sql.AppendLine("SELECT Operazione, COUNT(*) AS ContaOperazioni FROM @ElencoOperazioni GROUP BY Operazione")

            Dim DT As DataTable = EseguiQuery_Lettura(objParametri, sql.ToString, NomeRoutine)

            Dim nInsert As Integer = 0
            Dim nUpdate As Integer = 0
            For Each r In DT.Rows
                Dim op = r("Operazione").ToString.ToUpper
                If op = "INSERT" Then
                    nInsert = r("ContaOperazioni")
                ElseIf op = "UPDATE" Then
                    nUpdate = r("ContaOperazioni")
                End If
            Next

        Catch ex As Exception

            Dim MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)

            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return True
    End Function
End Class

