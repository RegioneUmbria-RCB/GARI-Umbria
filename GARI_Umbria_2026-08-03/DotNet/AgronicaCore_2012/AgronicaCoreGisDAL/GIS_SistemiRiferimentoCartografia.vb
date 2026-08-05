Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.DataProvider
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi




'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################



Public Class GIS_SistemiRiferimentoCartografia_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '//////////////////////////////////////////////////////////////////////////////////////////
    '//////////////////////////////////////////////////////////////////////////////////////////
    Public Function Leggi( _
                             ByVal GEORiferimento_COD As Integer _
                                    , ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile _
                                    , ByVal xFiltroAggiuntivo As String _
                                    , ByVal xOrderBy As String _
                                    , ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_R.Leggi()"

        
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    '
                    '
                    '

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT  * ")
                    StrSQL.Append(" FROM  GIS_SistemiRiferimentoCartografia Entita ")

                    Dim xWhereUsato As String = " WHERE "
                    If GEORiferimento_COD <> 0 Then
                        StrSQL.Append(xWhereUsato & " Entita.GEORiferimento_COD = " & Agro_SQL_SaveNum(GEORiferimento_COD) & " ")
                        xWhereUsato = " AND "
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(xWhereUsato & xFiltroAggiuntivo)
                    End If


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni
                    '
                    '
                    '

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
                    '
                    '
                    '

            End Select

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




Public Class GIS_SistemiRiferimentoCartografia_W
    Inherits AgronicaCoreDataProvider.DataProvider



    Public Function ModificaProiezioni( _
                              ByVal GEORiferimento_COD As Integer _
                            , ByVal AgronicaLatOffSet As Double _
                            , ByVal AgronicaLonOffSet As Double _
                            , ByVal CSFrom As String _
                            , ByVal CSTo As String _
                            , ByVal CStoGEO As String _
                            , ByVal Descrizione As String _
                            , ByVal Note As String _
                            , ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                            ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_W.Modifica()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False


        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE GIS_SistemiRiferimentoCartografia ")
            StrSQL.Append(" ")
            StrSQL.Append(" SET ")
            StrSQL.Append("  CSFrom = '" & Agro_SQL_SaveText(CSFrom) & "'")
            StrSQL.Append(" ,CSTo = '" & Agro_SQL_SaveText(CSTo) & "'")
            StrSQL.Append(" ,CSToGEO = '" & Agro_SQL_SaveText(CStoGEO) & "'")
            StrSQL.Append(" ,Note = '" & Agro_SQL_SaveText(Note) & "'")
            StrSQL.Append(" ,Descrizione = '" & Agro_SQL_SaveText(Descrizione) & "'")
            StrSQL.Append(" ,AgronicaLatOffSet = " & Agro_SQL_SaveNum(AgronicaLatOffSet) & " ")
            StrSQL.Append(" ,AgronicaLonOffSet = " & Agro_SQL_SaveNum(AgronicaLonOffSet) & " ")

            StrSQL.Append("  WHERE GEORiferimento_COD =  " & GEORiferimento_COD & vbCrLf)


            '---------------------------------------------

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp



    End Function


    Public Function Scrivi( _
                              ByVal GEORiferimento_COD As Integer _
                            , ByVal AgronicaLatOffSet As Double _
                            , ByVal AgronicaLonOffSet As Double _
                            , ByVal CSFrom As String _
                            , ByVal CSTo As String _
                            , ByVal CStoGEO As String _
                            , ByVal Descrizione As String _
                            , ByVal Note As String _
                            , ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                            ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Entita_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False


        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("INSERT INTO GIS_SistemiRiferimentoCartografia ")
            StrSQL.Append("                   ( ")
            StrSQL.Append("  ,[GEORiferimento_COD] " & vbCrLf)
            StrSQL.Append("  ,[AgronicaLatOffSet] " & vbCrLf)
            StrSQL.Append("  ,[AgronicaLonOffSet] " & vbCrLf)
            StrSQL.Append("  ,[CSFrom] " & vbCrLf)
            StrSQL.Append("  ,[CSTo] " & vbCrLf)
            StrSQL.Append("  ,[CStoGEO] " & vbCrLf)
            StrSQL.Append("  ,[Descrizione] " & vbCrLf)
            StrSQL.Append("  ,[Note] " & vbCrLf)
            StrSQL.Append("                   ) ")

            StrSQL.Append("VALUES (")
            StrSQL.Append(", " & Agro_SQL_SaveNum(GEORiferimento_COD) & " " & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveText(AgronicaLatOffSet) & " " & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveText(AgronicaLonOffSet) & " " & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(CSFrom) & "'" & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(CSTo) & "'" & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(CStoGEO) & "'" & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Descrizione) & "'" & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Note) & "'" & vbCrLf)

            StrSQL.Append("                   ) ")


            '---------------------------------------------

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception


            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function

End Class
