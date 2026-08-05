Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class ImpresexParticellexMacrousi_R
    Inherits AgronicaCoreDataProvider.DataProvider



    Public Function Leggi( _
                           ByVal Piva As String, _
                           ByVal PROV As String, _
                           ByVal COM As String, _
                           ByVal SEZIONE As String, _
                           ByVal FOGLIO As Int32, _
                           ByVal NUMERO As Int32, _
                           ByVal SUBALTERNO As String, _
                           ByVal Macrouso_Cod As String, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexParticellexMacrousi_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva
        '   Part_Cod = ""
        '   PROV = ""
        '   COM = ""
        '   SEZIONE = ""
        '   FOGLIO = 0
        '   NUMERO = 0
        '   SUBALTERNO = ""
        '   Macrouso_Cod = ""
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT IPM.* , Macrousi.Macrouso_Des as Macrouso_Des ")
            StrSQL.Append(" FROM  ImpresexParticellexMacrousi IPM ")
            StrSQL.Append(" INNER Join Macrousi M ON IPM.Macrouso_Cod = M.Macrouso_Cod")
            StrSQL.Append(" WHERE IPM.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   IPM.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If PROV <> "" Then
                StrSQL.Append(" AND IPM.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
            End If

            If COM <> "" Then
                StrSQL.Append(" AND IPM.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
            End If

            If SEZIONE <> "" Then
                StrSQL.Append(" AND IPM.SEZIONE = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
            End If

            If FOGLIO <> 0 Then
                StrSQL.Append(" AND IPM.FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
            End If

            If NUMERO <> 0 Then
                StrSQL.Append(" AND IPM.NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
            End If

            If SUBALTERNO <> "" Then
                StrSQL.Append(" AND IPM.SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")
            End If

            If Macrouso_Cod <> "" Then
                StrSQL.Append(" AND IPM.Macrouso_Cod =  '" & Agro_SQL_SaveText(Macrouso_Cod) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   IPM.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   IPM.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY IPM.Validita_inizio ASC")
            End If



            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
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
