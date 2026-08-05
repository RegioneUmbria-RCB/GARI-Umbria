Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.DataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class Conti_Patrimonio_R
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function Leggi(ByVal Piva As String, _
                             ByVal Cod_Conto_Pat As Integer, _
                            ByVal xFiltroAggiuntivo As String, _
                           ByVal xOrderBy As String, _
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                           ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Conti_Patrimonio_R.Leggi"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  Conti_Patrimonio ")
            StrSQL.Append(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
  
            If Piva <> "" Then
                StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Cod_Conto_Pat <> 0 Then
                StrSQL.Append(" AND Cod_Conto_Pat = " & Agro_SQL_SaveNum(Cod_Conto_Pat) & "   ")
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
            Else
                StrSQL.Append(" ORDER BY Conto_Pat_Descr  ")
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


    '###################################################################################
    Public Function Esiste_Conto(ByVal Piva As String, _
                                 ByVal Cod_Conto_Pat As Integer, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Conti_Patrimonio_R.Esiste_Conto()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim Flag_Esiste As Boolean

        Try

            DT = Leggi(Piva, Cod_Conto_Pat, _
                        xFiltroAggiuntivo, "", _
                        objParametri)

            If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then
                Flag_Esiste = True
            End If

            DT = Nothing

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Flag_Esiste

    End Function

End Class
