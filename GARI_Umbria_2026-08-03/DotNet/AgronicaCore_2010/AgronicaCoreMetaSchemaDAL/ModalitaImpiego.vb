Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Public Class ModalitaImpiego_R

    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal MDI_COD As Int32,
                          ByVal Validita_Inizio As Date,
                          ByVal Validita_Fine As Date,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.ModalitaImpiego_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT * FROM  ModalitaImpiego " &
                                  " WHERE Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine) & " " &
                                  " AND   Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " ")


            If MDI_COD <> 0 Then
                StrSQL.Append(" AND MDI_COD =  " & Agro_SQL_SaveNum(MDI_COD) & "  ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY mdi_des ASC ")
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

    Public Function MdiDes_from_MdiCod(ByVal MDI_COD As Int32,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                      ) As String

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.ModalitaImpiego_R.MdiDes_from_MdiCod()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT mdi_des FROM  ModalitaImpiego ")

            If MDI_COD <> 0 Then
                StrSQL.Append(" WHERE MDI_COD =  " & Agro_SQL_SaveNum(MDI_COD) & "  ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then
                Return DT.Rows(0).Item("mdi_des")
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Return ""

        End Try

    End Function

End Class
