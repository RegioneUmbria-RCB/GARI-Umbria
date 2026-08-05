Imports AgronicaCoreDataProvider.UtilityProvider

Public Class PUA_Testata_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi( _
                            ByVal PUA_Cod As Int32, _
                            ByVal Piva As String, _
                            ByVal Validita_Inizio As Date, _
                            ByVal Validita_Fine As Date, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.PUA_Testata_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = "" 
        '   PUA_Cod = 0    
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '------------------------------------------------------------------

            'TODO
            'modificare la query di select
            StrSQL.Length = 0

            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  PUA_Testata ")
            StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            If objParametri.PivaSuperUser <> "" Then
                StrSQL.Append(" AND Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            End If

            If Piva <> "" Then
                StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If PUA_Cod <> 0 Then
                StrSQL.Append(" AND PUA_Cod = " & Agro_SQL_SaveNum(PUA_Cod) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Piva_SuperUser, Piva, PUA_Cod Asc ")
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

    '##############################################################################################
    Public Function Leggi_Tutte_Piva_Che_Hanno_PUA(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                   ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.PUA_Testata_R.Leggi_Tutte_Piva_Che_Hanno_PUA()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim NumContatti As Integer = 0

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" select distinct piva from PUA_Testata ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '-------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

End Class
