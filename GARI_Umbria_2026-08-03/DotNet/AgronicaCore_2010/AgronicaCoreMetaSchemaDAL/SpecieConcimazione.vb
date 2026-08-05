Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi

'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§

Public Class SpecieConcimazione_R
    Inherits AgronicaCoreDataProvider.DataProvider



    'Versione che utilizza AgronicaCoreParametri
    '##############################################################################################
    Public Function Leggi( _
                            ByVal Veg_Cod As Integer, _
                            ByVal Gru_Cod As Integer, _
                            ByVal LetteraIniziale As String, _
                            ByVal StringaCerca As String, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.SpecieConcimazione_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Veg_Cod = 0           =>  tutti i record
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT SpecieVegetali.* ")
            StrSQL.Append(" FROM   SpecieConcimazione INNER JOIN SpecieVegetali ON SpecieConcimazione.VEG_COD = SpecieVegetali.VEG_COD ")
            StrSQL.Append(" WHERE  SpecieConcimazione.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND    SpecieConcimazione.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            'le specie vegetali che il Gias non deve caricare hanno gru_cod = -1
            StrSQL.Append(" AND Gru_Cod <> -1 ")

            If Gru_Cod <> 0 Then
                StrSQL.Append(" AND Gru_Cod = " & Agro_SQL_SaveNum(Gru_Cod) & " ")
            End If

            If Veg_Cod <> 0 Then
                StrSQL.Append(" AND Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
            End If

            If LetteraIniziale <> "" Then
                StrSQL.Append(" AND Veg_Des LIKE '" & Agro_SQL_SaveText(LetteraIniziale) & "%' ")
            End If

            If StringaCerca <> "" Then
                StrSQL.Append(" AND Veg_Des LIKE '%" & Agro_SQL_SaveText(StringaCerca) & "%' ")
            End If
            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   SpecieConcimazione.Inviato >=0 ")
                    StrSQL.Append(" AND   SpecieVegetali.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   SpecieConcimazione.Inviato =-1 ")
                    StrSQL.Append(" AND   SpecieVegetali.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY SpecieVegetali.Veg_Des ASC")
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
