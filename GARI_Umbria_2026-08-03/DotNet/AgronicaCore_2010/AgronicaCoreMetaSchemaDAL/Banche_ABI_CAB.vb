Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class Banche_ABI_CAB_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal ABI As String, _
                            ByVal CAB As String, _
                            ByVal xFiltroAggiuntivo As String, _
                            ByVal xOrderBy As String, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaschemaDAL.Banche_ABI_CAB_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '---------------------------------------------

            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  Banche_ABI_CAB ")
            StrSQL.Append(" WHERE 1 = 1 ")

            If ABI <> "" Then
                StrSQL.Append(" AND ABI = '" & Agro_SQL_SaveText(ABI) & "'   " + vbCrLf)
            End If

            If CAB <> "" Then
                StrSQL.Append(" AND CAB = '" & Agro_SQL_SaveText(CAB) & "'   " + vbCrLf)
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            'Select Case objParametri.FlagVisibilita
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
            '        StrSQL.Append(" AND     Inviato >= 0 ")
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
            '        StrSQL.Append(" AND     Inviato = -1 ")
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
            '        '...................................
            '    Case Else
            '        Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            'End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY istituto, sportello ASC")
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


    '##########################################################################################################################################
    Public Function Recupera_IstitutoSportello_byABICAB(ByVal ABI As String, _
                                                ByVal CAB As String, _
                                                ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String


        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Banche_ABI_CAB_R.Recupera_IstitutoSportello_byABICAB"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim IstitutoSportello As String = ""

        Try

            Dim Dt As DataTable

            Dt = Leggi(ABI, CAB, _
                       "", "", _
                       objParametri)

            If Not Dt Is Nothing AndAlso Dt.Rows.Count > 0 Then
                IstitutoSportello = Dt.Rows(0).Item("istituto")
                If Dt.Rows(0).Item("sportello") <> "" Then
                    IstitutoSportello &= " - " & Dt.Rows(0).Item("sportello")
                End If
            End If

            Dt = Nothing

        Catch ex As Exception
            IstitutoSportello = ""
            MessaggioErrore = ex.Message
            MyBase.Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return IstitutoSportello

    End Function

End Class
