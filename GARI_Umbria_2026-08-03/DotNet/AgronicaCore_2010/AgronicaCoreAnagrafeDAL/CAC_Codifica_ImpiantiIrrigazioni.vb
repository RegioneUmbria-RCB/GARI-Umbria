Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class CAC_Codifica_ImpiantiIrrigazioni
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function Imp_Cod_Gias_from_Imp_Cod_Cliente(ByVal Imp_Cod_Cliente As String, _
                                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer

        Dim ob As New AgronicaCoreAnagrafeDAL.CAC_Codifica_ImpiantiIrrigazioni
        Dim NomeRoutine As String = "Imp_Cod_Gias_from_Imp_Cod_Cliente"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim Messaggio As String

        'Genero la query SQL
        StrSQL.Length = 0
        StrSQL.Append(" SELECT  Imp_Cod_Gias ")
        StrSQL.Append(" FROM    Cac_Codifica_ImpiantiIrrigazioni ")
        StrSQL.Append(" WHERE   Imp_Cod_Cliente = '" & Agro_SQL_SaveText(Imp_Cod_Cliente) & "' ")

        DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)


        If DT.Rows.Count > 0 Then
            Return DT.Rows(0).Item("Imp_Cod_Gias")
        End If
        Return (-1)

    End Function


    Public Function Leggi(ByVal Imp_Cod_Cliente As String, _
                         ByVal Imp_Cod_Gias As Integer, _
                         ByVal xFiltroAggiuntivo As String, _
                         ByVal xOrderBy As String, _
                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                         Optional ByVal Tipo_Codifica As Integer = 0 _
                         ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CAC_Codifica_ImpiantiIrrigazioni.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  Cac_Codifica_ImpiantiIrrigazioni ")
            StrSQL.Append(" WHERE 1=1 ")

            If Imp_Cod_Cliente <> "" Then
                StrSQL.Append(" AND Imp_Cod_Cliente = '" & Agro_SQL_SaveText(Imp_Cod_Cliente) & "' ")
            End If

            If Imp_Cod_Gias <> 0 Then
                StrSQL.Append(" AND Imp_Cod_Gias = " & Agro_SQL_SaveNum(Imp_Cod_Gias) & " ")
            End If

            If Tipo_Codifica <> 0 Then
                StrSQL.Append(" AND Tipo_Codifica = " & Agro_SQL_SaveNum(Tipo_Codifica) & " ")
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
                StrSQL.Append(" ORDER BY Descrizione ")
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
