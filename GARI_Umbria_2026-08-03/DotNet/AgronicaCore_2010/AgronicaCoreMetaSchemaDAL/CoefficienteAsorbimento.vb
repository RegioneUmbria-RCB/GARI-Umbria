Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider


Public Class CoefficienteAsorbimento_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function Leggi(Regolamento_Cod As Integer,
                          Veg_Cod As Integer,
                          Grfi_Cod As Integer,
                          Validita_Inizio As Date,
                          Validita_Fine As Date,
                          xFiltroAggiuntivo As String,
                          xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaschemaDAL.CoefficienteAsorbimento_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM CoefficenteAssorbimento ")
            StrSQL.AppendLine(" WHERE Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.AppendLine(" AND Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            If Regolamento_Cod <> 0 Then
                StrSQL.AppendLine(" AND Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod))
            End If

            If Veg_Cod <> 0 Then
                StrSQL.AppendLine(" AND Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod))
            End If

            If Grfi_Cod <> 0 Then
                StrSQL.AppendLine(" AND Grfi_Cod = " & Agro_SQL_SaveNum(Grfi_Cod))
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Regolamento_Cod ASC, Veg_Cod ASC, Grfi_Cod ASC")
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
