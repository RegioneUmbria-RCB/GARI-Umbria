Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class Documenti_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi_Documenti( _
                                    ByVal Documento_Cod As Integer?, _
                                    ByVal Modello_Codice As String, _
                                    ByVal Modello_Revisione As String, _
                                    ByVal Articolo_Cod As String, _
                                    ByVal DataOraCreazione As DateTime?, _
                                    ByVal Stato As Integer?, _
                                    ByVal Conforme_StdNome As String, _
                                    ByVal Conforme_StdRev As String, _
                                    ByVal xFiltroAggiuntivo As String, _
                                    ByVal xOrderBy As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "LabControlloQualitaDAL.Documenti_R.Leggi_Documenti()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.Append(" SELECT PivaSuperUser, Documento_Cod, Modello_Codice, Modello_Revisione, Articolo_Cod, DataOraCreazione, Stato, Conforme_StdNome, Conforme_StdRev " + vbCrLf)
            strSQL.Append(" FROM LCQ_Documenti" + vbCrLf)
            strSQL.Append(" WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser) + vbCrLf)

            If Not IsNothing(Documento_Cod) Then
                strSQL.Append(" AND Documento_Cod = " & Agro_SQL_SaveNum_NULL(Documento_Cod))
            End If

            If Not IsNothing(Modello_Codice) Then
                strSQL.Append(" AND Modello_Codice = " & Agro_SQL_SaveText_NULL(Modello_Codice))
            End If

            If Not IsNothing(Modello_Revisione) Then
                strSQL.Append(" AND Modello_Revisione = " & Agro_SQL_SaveText_NULL(Modello_Revisione))
            End If

            If Not IsNothing(Articolo_Cod) Then
                strSQL.Append(" AND Articolo_Cod = " & Agro_SQL_SaveText_NULL(Articolo_Cod))
            End If

            If Not IsNothing(DataOraCreazione) Then
                strSQL.Append(" AND DataOraCreazione = " & Agro_SQL_SaveDateTime_NULL(DataOraCreazione))
            End If

            If Not IsNothing(Stato) Then
                strSQL.Append(" AND Stato = " & Agro_SQL_SaveNum_NULL(Stato))
            End If

            If Not IsNothing(Conforme_StdNome) Then
                strSQL.Append(" AND Conforme_StdNome = " & Agro_SQL_SaveText_NULL(Conforme_StdNome))
            End If

            If Not IsNothing(Conforme_StdRev) Then
                strSQL.Append(" AND Conforme_StdRev = " & Agro_SQL_SaveText_NULL(Conforme_StdRev))
            End If

            If xFiltroAggiuntivo <> "" Then
                strSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    strSQL.Append(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    strSQL.Append(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                strSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message & " QUERY: " & strSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function Leggi_Documenti( _
                                    ByVal Documento_Cod As Integer?, _
                                    ByVal Modello_Codice As String, _
                                    ByVal Modello_Revisione As String, _
                                    ByVal Articolo_Cod As String, _
                                    ByVal DataOraCreazioneFrom As DateTime?, _
                                    ByVal DataOraCreazioneTo As DateTime?, _
                                    ByVal Stato As Integer?, _
                                    ByVal Conforme_StdNome As String, _
                                    ByVal Conforme_StdRev As String, _
                                    ByVal xFiltroAggiuntivo As String, _
                                    ByVal xOrderBy As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "LabControlloQualitaDAL.Documenti_R.Leggi_Documenti()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.Append(" SELECT PivaSuperUser, Documento_Cod, Modello_Codice, Modello_Revisione, Articolo_Cod, DataOraCreazione, Stato, Conforme_StdNome, Conforme_StdRev " + vbCrLf)
            strSQL.Append(" FROM LCQ_Documenti" + vbCrLf)
            strSQL.Append(" WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser) + vbCrLf)

            If Not IsNothing(Documento_Cod) Then
                strSQL.Append(" AND Documento_Cod = " & Agro_SQL_SaveNum_NULL(Documento_Cod))
            End If

            If Not IsNothing(Modello_Codice) Then
                strSQL.Append(" AND Modello_Codice = " & Agro_SQL_SaveText_NULL(Modello_Codice))
            End If

            If Not IsNothing(Modello_Revisione) Then
                strSQL.Append(" AND Modello_Revisione = " & Agro_SQL_SaveText_NULL(Modello_Revisione))
            End If

            If Not IsNothing(Articolo_Cod) Then
                strSQL.Append(" AND Articolo_Cod = " & Agro_SQL_SaveText_NULL(Articolo_Cod))
            End If

            If Not IsNothing(DataOraCreazioneFrom) Then
                strSQL.Append(" AND DataOraCreazione >= " & Agro_SQL_SaveDateTime_NULL(DataOraCreazioneFrom))
            End If
            If Not IsNothing(DataOraCreazioneTo) Then
                strSQL.Append(" AND DataOraCreazione <= " & Agro_SQL_SaveDateTime_NULL(DataOraCreazioneTo))
            End If

            If Not IsNothing(Stato) Then
                strSQL.Append(" AND Stato = " & Agro_SQL_SaveNum_NULL(Stato))
            End If

            If Not IsNothing(Conforme_StdNome) Then
                strSQL.Append(" AND Conforme_StdNome = " & Agro_SQL_SaveText_NULL(Conforme_StdNome))
            End If

            If Not IsNothing(Conforme_StdRev) Then
                strSQL.Append(" AND Conforme_StdRev = " & Agro_SQL_SaveText_NULL(Conforme_StdRev))
            End If

            If xFiltroAggiuntivo <> "" Then
                strSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    strSQL.Append(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    strSQL.Append(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                strSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message & " QUERY: " & strSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function Leggi_DocumentiConRilevazioniTraDate( _
                                    ByVal Documento_Cod As Integer?, _
                                    ByVal Modello_Codice As String, _
                                    ByVal Modello_Revisione As String, _
                                    ByVal Articolo_Cod As String, _
                                    ByVal DataOraRilevazioneFrom As DateTime?, _
                                    ByVal DataOraRilevazioneTo As DateTime?, _
                                    ByVal Stato As Integer?, _
                                    ByVal Conforme_StdNome As String, _
                                    ByVal Conforme_StdRev As String, _
                                    ByVal xFiltroAggiuntivo As String, _
                                    ByVal xOrderBy As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "LabControlloQualitaDAL.Documenti_R.Leggi_Documenti()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.Append(" SELECT DISTINCT d.PivaSuperUser, d.Documento_Cod, d.Modello_Codice, d.Modello_Revisione, d.Articolo_Cod, d.DataOraCreazione, d.Stato, d.Conforme_StdNome, d.Conforme_StdRev " + vbCrLf)
            strSQL.Append(" FROM LCQ_Documenti d" + vbCrLf)
            strSQL.Append(" INNER JOIN LCQ_ParametriValori pv" + vbCrLf)
            strSQL.Append(" ON d.Documento_Cod = pv.Documento_Cod" + vbCrLf)
            strSQL.Append(" WHERE d.PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser) + vbCrLf)

            If Not IsNothing(Documento_Cod) Then
                strSQL.Append(" AND d.Documento_Cod = " & Agro_SQL_SaveNum_NULL(Documento_Cod))
            End If

            If Not IsNothing(Modello_Codice) Then
                strSQL.Append(" AND d.Modello_Codice = " & Agro_SQL_SaveText_NULL(Modello_Codice))
            End If

            If Not IsNothing(Modello_Revisione) Then
                strSQL.Append(" AND d.Modello_Revisione = " & Agro_SQL_SaveText_NULL(Modello_Revisione))
            End If

            If Not IsNothing(Articolo_Cod) Then
                strSQL.Append(" AND d.Articolo_Cod = " & Agro_SQL_SaveText_NULL(Articolo_Cod))
            End If

            If Not IsNothing(DataOraRilevazioneFrom) Then
                strSQL.Append(" AND pv.DataOraRilevazione >= " & Agro_SQL_SaveDateTime_NULL(DataOraRilevazioneFrom))
            End If
            If Not IsNothing(DataOraRilevazioneTo) Then
                strSQL.Append(" AND pv.DataOraRilevazione <= " & Agro_SQL_SaveDateTime_NULL(DataOraRilevazioneTo))
            End If

            If Not IsNothing(Stato) Then
                strSQL.Append(" AND d.Stato = " & Agro_SQL_SaveNum_NULL(Stato))
            End If

            If Not IsNothing(Conforme_StdNome) Then
                strSQL.Append(" AND d.Conforme_StdNome = " & Agro_SQL_SaveText_NULL(Conforme_StdNome))
            End If

            If Not IsNothing(Conforme_StdRev) Then
                strSQL.Append(" AND d.Conforme_StdRev = " & Agro_SQL_SaveText_NULL(Conforme_StdRev))
            End If

            If xFiltroAggiuntivo <> "" Then
                strSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    strSQL.Append(" AND   d.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    strSQL.Append(" AND   d.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                strSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message & " QUERY: " & strSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function Leggi_DocumentiConParametroTestataTraDate( _
                                    ByVal Documento_Cod As Integer?, _
                                    ByVal Modello_Codice As String, _
                                    ByVal Modello_Revisione As String, _
                                    ByVal Articolo_Cod As String, _
                                    ByVal Stato As Integer?, _
                                    ByVal Conforme_StdNome As String, _
                                    ByVal Conforme_StdRev As String, _
                                    ByVal NomeParametro As String, _
                                    ByVal DataOraInizio As DateTime?, _
                                    ByVal DataOraFine As DateTime?, _
                                    ByVal xFiltroAggiuntivo As String, _
                                    ByVal xOrderBy As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "LabControlloQualitaDAL.Documenti_R.Leggi_DocumentiConParametroTestataTraDate()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.Append(" SELECT d.PivaSuperUser, d.Documento_Cod, d.Modello_Codice, d.Modello_Revisione, d.Articolo_Cod, pv.DateTime_Valore as 'DataOraCreazione', d.Stato, d.Conforme_StdNome, d.Conforme_StdRev " + vbCrLf)
            strSQL.Append(" FROM LCQ_Parametri p" + vbCrLf)
            strSQL.Append("            INNER JOIN LCQ_ParametriXModelli pxm " & vbCrLf)
            strSQL.Append(" ON pxm.PivaSuperUser=p.PivaSuperUser AND pxm.Parametro_Cod=p.Parametri_Cod " & vbCrLf)
            strSQL.Append(" INNER JOIN LCQ_ParametriValori pv " & vbCrLf)
            strSQL.Append(" ON pv.PivaSuperUser = pxm.PivaSuperUser AND pv.PrmXMod_Cod= pxm.ParametriXModelli_Cod " & vbCrLf)
            strSQL.Append(" INNER JOIN LCQ_Documenti d " & vbCrLf)
            strSQL.Append(" ON pv.PivaSuperUser = d.PivaSuperUser AND pv.Documento_Cod= d.Documento_Cod " & vbCrLf)
            strSQL.Append(" WHERE d.PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser) + vbCrLf)

            strSQL.Append(" AND p.Nome = " & Agro_SQL_SaveText_NULL(NomeParametro) & vbCrLf)
            strSQL.Append(" AND p.DaValutare = 0 " & vbCrLf)

            If Not IsNothing(Documento_Cod) Then
                strSQL.Append(" AND d.Documento_Cod = " & Agro_SQL_SaveNum_NULL(Documento_Cod))
            End If

            If Not IsNothing(Modello_Codice) Then
                strSQL.Append(" AND d.Modello_Codice = " & Agro_SQL_SaveText_NULL(Modello_Codice))
            End If

            If Not IsNothing(Modello_Revisione) Then
                strSQL.Append(" AND d.Modello_Revisione = " & Agro_SQL_SaveText_NULL(Modello_Revisione))
            End If

            If Not IsNothing(Articolo_Cod) Then
                strSQL.Append(" AND d.Articolo_Cod = " & Agro_SQL_SaveText_NULL(Articolo_Cod))
            End If

            If Not IsNothing(DataOraInizio) Then
                strSQL.Append(" AND pv.DateTime_Valore >= " & Agro_SQL_SaveDateTime_NULL(DataOraInizio))
            End If
            If Not IsNothing(DataOraFine) Then
                strSQL.Append(" AND pv.DateTime_Valore <= " & Agro_SQL_SaveDateTime_NULL(DataOraFine))
            End If

            If Not IsNothing(Stato) Then
                strSQL.Append(" AND d.Stato = " & Agro_SQL_SaveNum_NULL(Stato))
            End If

            If Not IsNothing(Conforme_StdNome) Then
                strSQL.Append(" AND d.Conforme_StdNome = " & Agro_SQL_SaveText_NULL(Conforme_StdNome))
            End If

            If Not IsNothing(Conforme_StdRev) Then
                strSQL.Append(" AND d.Conforme_StdRev = " & Agro_SQL_SaveText_NULL(Conforme_StdRev))
            End If

            If xFiltroAggiuntivo <> "" Then
                strSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    strSQL.Append(" AND   d.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    strSQL.Append(" AND   d.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                strSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message & " QUERY: " & strSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function Leggi_DocumentiSenzaDataDInteresseConCreazioneTraDate( _
                                    ByVal Documento_Cod As Integer?, _
                                    ByVal Modello_Codice As String, _
                                    ByVal Modello_Revisione As String, _
                                    ByVal Articolo_Cod As String, _
                                    ByVal Stato As Integer?, _
                                    ByVal Conforme_StdNome As String, _
                                    ByVal Conforme_StdRev As String, _
                                    ByVal DataOraCreazioneDa As DateTime?, _
                                    ByVal DataOraCreazioneA As DateTime?, _
                                    ByVal xFiltroAggiuntivo As String, _
                                    ByVal xOrderBy As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "LabControlloQualitaDAL.Documenti_R.Leggi_DocumentiSenzaDataDInteresseConCreazioneTraDate()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.Append(" SELECT d.PivaSuperUser, d.Documento_Cod, d.Modello_Codice, d.Modello_Revisione, d.Articolo_Cod, d.DataOraCreazione, d.Stato, d.Conforme_StdNome, d.Conforme_StdRev " & vbCrLf)
            strSQL.Append(" FROM LCQ_Documenti d " & vbCrLf)
            strSQL.Append(" WHERE d.PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser) + vbCrLf)
            strSQL.Append(" AND d.Documento_Cod NOT IN " & vbCrLf)
            strSQL.Append(" ( " & vbCrLf)
            strSQL.Append("    SELECT distinct pv.Documento_Cod " & vbCrLf)
            strSQL.Append("    FROM LCQ_ParametriValori pv " & vbCrLf)
            strSQL.Append("    WHERE pv.PrmXMod_Cod IN " & vbCrLf)
            strSQL.Append("    ( " & vbCrLf)
            strSQL.Append("        SELECT pxm.ParametriXModelli_Cod  " & vbCrLf)
            strSQL.Append("        FROM LCQ_ParametriXModelli pxm " & vbCrLf)
            strSQL.Append("        INNER JOIN LCQ_Parametri p " & vbCrLf)
            strSQL.Append("        ON pxm.PivaSuperUser=p.PivaSuperUser AND pxm.Parametro_Cod=p.Parametri_Cod " & vbCrLf)
            strSQL.Append("        WHERE p.Nome in ('Data Produzione','Data Fornitura','Data Confezionamento') " & vbCrLf)
            strSQL.Append("        AND p.DaValutare=0 " & vbCrLf)
            strSQL.Append("    ) " & vbCrLf)
            strSQL.Append(" ) " & vbCrLf)

            If Not IsNothing(Documento_Cod) Then
                strSQL.Append(" AND d.Documento_Cod = " & Agro_SQL_SaveNum_NULL(Documento_Cod))
            End If

            If Not IsNothing(Modello_Codice) Then
                strSQL.Append(" AND d.Modello_Codice = " & Agro_SQL_SaveText_NULL(Modello_Codice))
            End If

            If Not IsNothing(Modello_Revisione) Then
                strSQL.Append(" AND d.Modello_Revisione = " & Agro_SQL_SaveText_NULL(Modello_Revisione))
            End If

            If Not IsNothing(Articolo_Cod) Then
                strSQL.Append(" AND d.Articolo_Cod = " & Agro_SQL_SaveText_NULL(Articolo_Cod))
            End If

            If Not IsNothing(DataOraCreazioneDa) Then
                strSQL.Append(" AND d.DataOraCreazione >= " & Agro_SQL_SaveDateTime_NULL(DataOraCreazioneDa))
            End If
            If Not IsNothing(DataOraCreazioneA) Then
                strSQL.Append(" AND d.DataOraCreazione <= " & Agro_SQL_SaveDateTime_NULL(DataOraCreazioneA))
            End If

            If Not IsNothing(Stato) Then
                strSQL.Append(" AND d.Stato = " & Agro_SQL_SaveNum_NULL(Stato))
            End If

            If Not IsNothing(Conforme_StdNome) Then
                strSQL.Append(" AND d.Conforme_StdNome = " & Agro_SQL_SaveText_NULL(Conforme_StdNome))
            End If

            If Not IsNothing(Conforme_StdRev) Then
                strSQL.Append(" AND d.Conforme_StdRev = " & Agro_SQL_SaveText_NULL(Conforme_StdRev))
            End If

            If xFiltroAggiuntivo <> "" Then
                strSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    strSQL.Append(" AND   d.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    strSQL.Append(" AND   d.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                strSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message & " QUERY: " & strSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function Leggi_Documenti( _
                                    ByVal htParametri As Hashtable, _
                                    ByVal Documento_Cod As Integer?, _
                                    ByVal Modello_Codice As String, _
                                    ByVal Modello_Revisione As String, _
                                    ByVal Articolo_Cod As String, _
                                    ByVal DataOraCreazioneFrom As DateTime?, _
                                    ByVal DataOraCreazioneTo As DateTime?, _
                                    ByVal Stato As Integer?, _
                                    ByVal Conforme_StdNome As String, _
                                    ByVal Conforme_StdRev As String, _
                                    ByVal xFiltroAggiuntivo As String, _
                                    ByVal xOrderBy As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "LabControlloQualitaDAL.Documenti_R.Leggi_Documenti()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.Append(" SELECT PivaSuperUser, Documento_Cod, Modello_Codice, Modello_Revisione, Articolo_Cod, DataOraCreazione, Stato, Conforme_StdNome, Conforme_StdRev " + vbCrLf)
            strSQL.Append(" FROM LCQ_Documenti" + vbCrLf)
            strSQL.Append(" WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser) + vbCrLf)

            If Not IsNothing(Documento_Cod) Then
                strSQL.Append(" AND Documento_Cod = " & Agro_SQL_SaveNum_NULL(Documento_Cod))
            End If

            If Not IsNothing(Modello_Codice) Then
                strSQL.Append(" AND Modello_Codice = " & Agro_SQL_SaveText_NULL(Modello_Codice))
            End If

            If Not IsNothing(Modello_Revisione) Then
                strSQL.Append(" AND Modello_Revisione = " & Agro_SQL_SaveText_NULL(Modello_Revisione))
            End If

            If Not IsNothing(Articolo_Cod) Then
                strSQL.Append(" AND Articolo_Cod = " & Agro_SQL_SaveText_NULL(Articolo_Cod))
            End If

            If Not IsNothing(DataOraCreazioneFrom) AndAlso Not IsNothing(DataOraCreazioneTo) Then
                strSQL.Append(" AND DataOraCreazione >= " & Agro_SQL_SaveDateTime_NULL(DataOraCreazioneFrom))
                strSQL.Append(" AND DataOraCreazione <= " & Agro_SQL_SaveDateTime_NULL(DataOraCreazioneTo))
            End If

            If Not IsNothing(Stato) Then
                strSQL.Append(" AND Stato = " & Agro_SQL_SaveNum_NULL(Stato))
            End If

            If Not IsNothing(Conforme_StdNome) Then
                strSQL.Append(" AND Conforme_StdNome = " & Agro_SQL_SaveText_NULL(Conforme_StdNome))
            End If

            If Not IsNothing(Conforme_StdRev) Then
                strSQL.Append(" AND Conforme_StdRev = " & Agro_SQL_SaveText_NULL(Conforme_StdRev))
            End If

            If xFiltroAggiuntivo <> "" Then
                strSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    strSQL.Append(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    strSQL.Append(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                strSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message & " QUERY: " & strSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

End Class

Public Class Documenti_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Scrivi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                    ByVal Documento_Cod As Integer, _
                                    ByVal Modello_Codice As String, _
                                    ByVal Modello_Revisione As String, _
                                    ByVal Articolo_Cod As String, _
                                    ByVal DataOraCreazione As DateTime, _
                                    ByVal Stato As Integer, _
                                    ByVal Conforme_StdNome As String, _
                                    ByVal Conforme_StdRev As String, _
                                    Optional ByVal Data_creazione As Date = #2/1/1900#, _
                                    Optional ByVal Data_modifica As Date = #2/1/1900#, _
                                    Optional ByVal username_creazione As String = "", _
                                    Optional ByVal username_modifica As String = "" _
                                    ) As Boolean


        Dim NomeRoutine As String = "LabControlloQualitaDAL.Documenti_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_creazione = #2/1/1900# Then
                Data_creazione = DateTime.Now
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = DateTime.Now
            End If

            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If



            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" INSERT INTO  LCQ_Documenti" + vbCrLf)

            StrSQL.Append("              (")
            StrSQL.Append("              PivaSuperUser,         Documento_Cod, ")
            StrSQL.Append("              Modello_Codice,        Modello_Revisione, ")
            StrSQL.Append("              Articolo_Cod,         ")
            StrSQL.Append("              DataOraCreazione,      Stato, ")
            StrSQL.Append("              Conforme_StdNome,      Conforme_StdRev, ")

            StrSQL.Append("              Inviato,            datainvio, ")
            StrSQL.Append("              Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("              UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("              Validita_Inizio,    Validita_Fine " + vbCrLf)
            StrSQL.Append("              ) ")

            StrSQL.Append(" VALUES ( ")

            StrSQL.Append("			 " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.Append("			," & Agro_SQL_SaveText_NULL(Documento_Cod))
            StrSQL.Append("			," & Agro_SQL_SaveText_NULL(Modello_Codice))
            StrSQL.Append("			," & Agro_SQL_SaveText_NULL(Modello_Revisione))
            StrSQL.Append("			," & Agro_SQL_SaveText_NULL(Articolo_Cod))
            StrSQL.Append("			," & Agro_SQL_SaveDateTime_NULL(DataOraCreazione))
            StrSQL.Append("			," & Agro_SQL_SaveNum_NULL(Stato))
            StrSQL.Append("			," & Agro_SQL_SaveText_NULL(NothingToDBNull(Conforme_StdNome)))
            StrSQL.Append("			," & Agro_SQL_SaveText_NULL(NothingToDBNull(Conforme_StdRev)))


            StrSQL.Append("         , 0  " + vbCrLf)
            StrSQL.Append("         , Null  " + vbCrLf)
            StrSQL.Append("			, " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            StrSQL.Append("			, " & Agro_SQL_SaveDate(AGRODATAINIZIO) & "  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDate(AGRODATAFINE) & "  ")



            StrSQL.Append(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message & " QUERY: " & StrSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function

    '##############################################################################################
    Public Function Modifica(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                ByVal Old_Documento_Cod As String, _
                ByVal New_Stato As Integer, _
                ByVal New_Conforme_StdNome As String, _
                ByVal New_Conforme_StdRev As String, _
                Optional ByVal Data_modifica As Date = #2/1/1900#, _
                Optional ByVal username_modifica As String = "" _
                ) As Boolean


        Dim NomeRoutine As String = "LabControlloQualitaDAL.Documenti_W.Modifica()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_modifica = #2/1/1900# Then
                Data_modifica = DateTime.Now
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If


            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" UPDATE LCQ_Documenti SET " + vbCrLf)

            StrSQL.Append("  Stato = " & Agro_SQL_SaveNum_NULL(New_Stato) & vbCrLf)
            StrSQL.Append(", Conforme_StdNome = " & Agro_SQL_SaveText_NULL(NothingToDBNull(New_Conforme_StdNome)) & vbCrLf)
            StrSQL.Append(", Conforme_StdRev = " & Agro_SQL_SaveText_NULL(NothingToDBNull(New_Conforme_StdRev)) & vbCrLf)

            StrSQL.Append(", Data_Modifica = " & Agro_SQL_SaveDateTime(Data_modifica) & vbCrLf)
            StrSQL.Append(", UserName_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "' " & vbCrLf)

            StrSQL.Append("	WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.Append("	AND Documento_Cod =		" & Agro_SQL_SaveNum_NULL(Old_Documento_Cod))


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message & " QUERY: " & StrSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function

    '##############################################################################################
    Public Function ModificaStato(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                ByVal Old_Documento_Cod As String, _
                ByVal New_Stato As Integer, _
                Optional ByVal Data_modifica As Date = #2/1/1900#, _
                Optional ByVal username_modifica As String = "" _
                ) As Boolean


        Dim NomeRoutine As String = "LabControlloQualitaDAL.Documenti_W.Modifica()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_modifica = #2/1/1900# Then
                Data_modifica = DateTime.Now
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If


            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" UPDATE LCQ_Documenti SET " + vbCrLf)

            StrSQL.Append("  Stato = " & Agro_SQL_SaveNum_NULL(New_Stato) & vbCrLf)

            StrSQL.Append(", Data_Modifica = " & Agro_SQL_SaveDateTime(Data_modifica) & vbCrLf)
            StrSQL.Append(", UserName_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "' " & vbCrLf)

            StrSQL.Append("	WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.Append("	AND Documento_Cod =		" & Agro_SQL_SaveNum_NULL(Old_Documento_Cod))


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message & " QUERY: " & StrSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function

    '##############################################################################################
    Public Function ModificaConformita(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                ByVal Old_Documento_Cod As String, _
                ByVal New_Conforme_StdNome As String, _
                ByVal New_Conforme_StdRev As String, _
                Optional ByVal Data_modifica As Date = #2/1/1900#, _
                Optional ByVal username_modifica As String = "" _
                ) As Boolean


        Dim NomeRoutine As String = "LabControlloQualitaDAL.Documenti_W.Modifica()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_modifica = #2/1/1900# Then
                Data_modifica = DateTime.Now
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If


            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" UPDATE LCQ_Documenti SET " + vbCrLf)

            StrSQL.Append("  Conforme_StdNome = " & Agro_SQL_SaveText_NULL(NothingToDBNull(New_Conforme_StdNome)) & vbCrLf)
            StrSQL.Append(", Conforme_StdRev = " & Agro_SQL_SaveText_NULL(NothingToDBNull(New_Conforme_StdRev)) & vbCrLf)

            StrSQL.Append(", Data_Modifica = " & Agro_SQL_SaveDateTime(Data_modifica) & vbCrLf)
            StrSQL.Append(", UserName_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "' " & vbCrLf)

            StrSQL.Append("	WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.Append("	AND Documento_Cod =		" & Agro_SQL_SaveNum_NULL(Old_Documento_Cod))


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message & " QUERY: " & StrSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function

    '#################################################################
    Public Function Cancella(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                             ByVal Documento_Cod As Integer, _
                             ByVal xFiltroAggiuntivo As String _
                              ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "LabControlloQualitaDAL.Documenti_W.Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                'StrSQL.Append(" UPDATE ... ")
                'StrSQL.Append(" SET ")
                'StrSQL.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                'StrSQL.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                'StrSQL.Append("         ,Inviato = -1 ")
                'StrSQL.Append(" WHERE   1=1 ")
                'StrSQL.Append(" AND     Inviato >= 0 ")
            Else
                StrSQL.Append(" DELETE FROM LCQ_Documenti ")
                StrSQL.Append(" WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
                StrSQL.Append("	AND Documento_Cod =		" & Agro_SQL_SaveNum_NULL(Documento_Cod))
            End If
            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message & " QUERY: " & StrSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function


End Class