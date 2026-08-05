Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class CAC_Codifica_UnitaMisura
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Udm_Cod_Cliente As String, _
                        ByVal Udm_Cod_Gias As Integer, _
                        ByVal xFiltroAggiuntivo As String, _
                        ByVal xOrderBy As String, _
                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CAC_Codifica_UnitaMisura.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  CAC_Codifica_UnitaMisura ")
            StrSQL.Append(" WHERE 1=1")

            If Udm_Cod_Cliente <> "" Then
                StrSQL.Append(" AND Udm_Cod_Cliente = '" & Agro_SQL_SaveText(Udm_Cod_Cliente) & "' ")
            End If

            If Udm_Cod_Gias <> 0 Then
                StrSQL.Append(" AND Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod_Gias) & " ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            'Select Case objParametri.FlagVisibilita
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
            '        StrSQL.Append(" AND   Inviato >=0 ")
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
            '        StrSQL.Append(" AND   Inviato =-1 ")
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
            '        '...................................
            '    Case Else
            '        Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            'End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Udm_Des ")
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
