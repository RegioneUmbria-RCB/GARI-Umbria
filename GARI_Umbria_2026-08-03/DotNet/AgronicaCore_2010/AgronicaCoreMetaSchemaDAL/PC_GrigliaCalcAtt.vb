Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Public Class PC_GrigliaCalcAtt_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Regolamento_Cod As Int32,
                          ByVal Calc_Att As Decimal,
                          ByVal IdDotazione As Integer,
                              ByVal xFiltroAggiuntivo As String,
                              ByVal xOrderBy As String,
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.PC_GrigliaCalcAtt_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim intRapporto As Integer = 0

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM    PC_GrigliaCalcAtt ")
            StrSQL.Append(" WHERE   1=1 ")

            If Regolamento_Cod <> 0 Then
                StrSQL.Append(" AND   Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & " ")
            End If

            If Calc_Att <> 0 Then
                StrSQL.Append(" AND   Minimo <= " & Agro_SQL_SaveNum(Calc_Att) & " ")
                StrSQL.Append(" AND   Massimo >= " & Agro_SQL_SaveNum(Calc_Att) & " ")
            End If

            If IdDotazione <> 0 Then
                StrSQL.Append(" AND   Id_Dotazione = " & Agro_SQL_SaveNum(IdDotazione) & " ")
            End If

            '--------------------------------------------------------------------------
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

    Public Function Leggi_Dotazione(ByVal Regolamento_Cod As Int32,
                                    ByVal Calc_Att As Decimal,
                                          ByVal xFiltroAggiuntivo As String,
                                          ByVal xOrderBy As String,
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                      ) As Integer

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.PC_GrigliaCalcAtt_R.Leggi_Dotazione()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim Id_Dotazione As Integer = 0

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT Id_Dotazione ")
            StrSQL.Append(" FROM    PC_GrigliaCalcAtt ")
            StrSQL.Append(" WHERE   1=1 ")

            If Regolamento_Cod <> 0 Then
                StrSQL.Append(" AND   Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & " ")
            End If

            StrSQL.Append(" AND   Minimo <= " & Agro_SQL_SaveNum(Calc_Att) & " ")
            StrSQL.Append(" AND   Massimo >= " & Agro_SQL_SaveNum(Calc_Att) & " ")


            '--------------------------------------------------------------------------
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
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If DT.Rows.Count > 0 Then
                Id_Dotazione = DT.Rows(0).Item("Id_Dotazione")
            End If

        Catch ex As Exception
            Return 0
        End Try

        Return Id_Dotazione

    End Function

End Class
