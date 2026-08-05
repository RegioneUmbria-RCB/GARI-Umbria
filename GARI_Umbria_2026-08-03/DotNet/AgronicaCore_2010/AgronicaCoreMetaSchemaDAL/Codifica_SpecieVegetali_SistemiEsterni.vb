Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.DataProviderExtensions


Public Class Codifica_SpecieVegetali_SistemiEsterni_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function Leggi(ByVal ID As Integer,
                          ByVal Sistema_Cod As Integer,
                          ByVal Veg_Cod_Esterno As String,
                          ByVal Cul_Cod_Esterno As String,
                          ByVal Veg_Cod As Integer,
                          ByVal Cul_Cod As Integer,
                          ByVal Grfi_Cod As Integer,
                          ByVal Grva_Cod As Integer,
                          ByVal Metodo_Produzione_Cod As Integer,
                          ByVal Reg_Cod As Integer,
                          ByVal Id_Cod As Integer,
                          ByVal Grsp_Cod As Integer,
                          ByVal Port_Cod As Integer,
                          ByVal Foral_Cod As Integer,
                          ByVal Validita_Inizio As Date,
                          ByVal Validita_Fine As Date,
                              ByVal xFiltroAggiuntivo As String,
                              ByVal xOrderBy As String,
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaschemaDAL.Codifica_SpecieVegetali_SistemiEsterni_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM  Codifica_SpecieVegetali_SistemiEsterni ")
            StrSQL.AppendLine(" WHERE Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.AppendLine(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            If ID <> 0 Then
                StrSQL.AppendLine(" AND ID  = " & Agro_SQL_SaveNum(ID))
            End If

            If Sistema_Cod <> 0 Then
                StrSQL.AppendLine(" AND Sistema_Cod  = " & Agro_SQL_SaveNum(Sistema_Cod))
            End If

            If Veg_Cod_Esterno <> "" Then
                StrSQL.AppendLine(" AND Veg_Cod_Esterno  = " & Agro_SQL_SaveText(Veg_Cod_Esterno))
            End If

            If Cul_Cod_Esterno <> "" Then
                StrSQL.AppendLine(" AND Cul_Cod_Esterno  = " & Agro_SQL_SaveText(Cul_Cod_Esterno))
            End If

            If Veg_Cod <> 0 Then
                StrSQL.AppendLine(" AND Veg_Cod  = " & Agro_SQL_SaveNum(Veg_Cod))
            End If

            If Cul_Cod <> 0 Then
                StrSQL.AppendLine(" AND Cul_Cod  = " & Agro_SQL_SaveNum(Cul_Cod))
            End If

            If Grfi_Cod <> 0 Then
                StrSQL.AppendLine(" AND Grfi_Cod  = " & Agro_SQL_SaveNum(Grfi_Cod))
            End If

            If Grva_Cod <> 0 Then
                StrSQL.AppendLine(" AND Grva_Cod  = " & Agro_SQL_SaveNum(Grva_Cod))
            End If

            If Metodo_Produzione_Cod <> 0 Then
                StrSQL.AppendLine(" AND Metodo_Produzione_Cod  = " & Agro_SQL_SaveNum(Metodo_Produzione_Cod))
            End If

            If Reg_Cod <> 0 Then
                StrSQL.AppendLine(" AND Reg_Cod  = " & Agro_SQL_SaveNum(Reg_Cod))
            End If

            If Id_Cod <> 0 Then
                StrSQL.AppendLine(" AND Id_Cod  = " & Agro_SQL_SaveNum(Id_Cod))
            End If

            If Grsp_Cod <> 0 Then
                StrSQL.AppendLine(" AND Grsp_Cod  = " & Agro_SQL_SaveNum(Grsp_Cod))
            End If

            If Port_Cod <> 0 Then
                StrSQL.AppendLine(" AND Port_Cod  = " & Agro_SQL_SaveNum(Port_Cod))
            End If

            If Foral_Cod <> 0 Then
                StrSQL.AppendLine(" AND Foral_Cod  = " & Agro_SQL_SaveNum(Foral_Cod))
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND     Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND     Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Veg_Des_Esterno ASC")
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
