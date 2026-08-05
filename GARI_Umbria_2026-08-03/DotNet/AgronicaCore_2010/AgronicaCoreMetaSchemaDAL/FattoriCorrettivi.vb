Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi


Public Class FattoriCorrettivi_R
    Inherits AgronicaCoreDataProvider.DataProvider


    ''' -----------------------------------------------------------------------------
    Public Function Leggi(ByVal Regolamento_Cod As Integer, _
                          ByVal Fattore_Cod As Integer, _
                          ByVal Tipo As String, _
                          ByVal Variazione As String, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.FattoriCorrettivi_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  FattoriCorrettivi ")
            StrSQL.Append(" WHERE FattoriCorrettivi.Regolamento_cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & "  ")

            If Fattore_Cod <> 0 Then
                StrSQL.Append(" AND FattoriCorrettivi.Fattore_Cod =  " & Agro_SQL_SaveNum(Fattore_Cod) & "  ")
            End If

            If Tipo <> "" Then
                StrSQL.Append(" AND FattoriCorrettivi.Tipo =  '" & Agro_SQL_SaveText(Tipo) & "'  ")
            End If

            If Variazione <> "" Then
                StrSQL.Append(" AND FattoriCorrettivi.Variazione =  '" & Agro_SQL_SaveText(Variazione) & "'  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   dbo.FattoriCorrettivi.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   dbo.FattoriCorrettivi.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

