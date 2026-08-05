
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider


Public Class Epoche_R

    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal EP_COD As Int32, _
                          ByVal Validita_Inizio As Date, _
                          ByVal Validita_Fine As Date, _
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Epoche_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT * FROM  Epoche " & _
                                  " WHERE Epoche.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine) & " " & _
                                  " AND   Epoche.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " ")


                    If EP_COD <> 0 Then
                        StrSQL.Append(" AND Epoche.EP_COD =  " & Agro_SQL_SaveNum(EP_COD) & "  ")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Epoche.Descrizione ASC ")
                    End If


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
                    '
                    '
                    '
                    '


            End Select



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

    Public Function EpocaDes_from_EpocaCod(ByVal EP_COD As Int32, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                      ) As String

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Epoche_R.EpocaDes_from_EpocaCod()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

   
            StrSQL.Length = 0

            StrSQL.Append(" SELECT Descrizione FROM  Epoche ")


            If EP_COD <> 0 Then
                StrSQL.Append(" WHERE EP_COD =  " & Agro_SQL_SaveNum(EP_COD) & "  ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then
                Return DT.Rows(0).Item("Descrizione")
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Return ""
            'Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

    End Function
    '#############################################################################################################
    '#############################################################################################################
    '#############################################################################################################




End Class
