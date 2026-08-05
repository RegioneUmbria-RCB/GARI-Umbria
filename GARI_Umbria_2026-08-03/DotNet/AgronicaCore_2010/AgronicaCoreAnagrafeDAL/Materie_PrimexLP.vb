Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider


Public Class Materie_PrimexLP_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi( _
                         ByVal PIVA As String, _
                        ByVal ID As Integer, _
                        ByVal Sa_Cod As Integer, _
                        ByVal Elem_Cod As Integer, _
                        ByVal Pro_Cod As Integer, _
                        ByVal Mat_Cod As Integer, _
                        ByVal Lotto_Cod1 As Integer, _
                        ByVal Lotto_Val1 As String, _
                        ByVal Lotto_Cod2 As Integer, _
                        ByVal Lotto_Val2 As String, _
                        ByVal Lotto_Cod3 As Integer, _
                        ByVal Lotto_Val3 As String, _
                        ByVal ID_Proprieta As Integer, _
                                 ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                                 ByVal xFiltroAggiuntivo As String, _
                                 ByVal xOrderBy As String, _
                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                 ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Materie_PrimexLP_R.Leggi()"


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '------------------------------------------------------------------
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi




                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT Distinct Materie_PrimexLotto_Proprieta.* ")
                    StrSQL.Append(" FROM  Materie_PrimexLotto_Proprieta ")
                    StrSQL.Append(" WHERE Materie_PrimexLotto_Proprieta.Piva_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "' ")
                    StrSQL.Append(" AND   Materie_PrimexLotto_Proprieta.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Materie_PrimexLotto_Proprieta.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")

                    If ID <> 0 Then
                        StrSQL.Append(" AND Materie_PrimexLotto_Proprieta.id = " & Agro_SQL_SaveNum(ID) & "   ")
                    End If

                    'Nota Marco: Sa_Cod non Utilizzato (messo in interfaccia solo per standardizzazione)
                    StrSQL.Append(" AND (Materie_PrimexLotto_Proprieta.Piva = '" & Agro_SQL_SaveText(PIVA) & "' Or Materie_PrimexLotto_Proprieta.Sa_Cod = -1)  ")

                    If Elem_Cod <> 0 Then
                        StrSQL.Append(" AND Materie_PrimexLotto_Proprieta.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
                    End If

                    If Pro_Cod <> 0 Then
                        StrSQL.Append(" AND Materie_PrimexLotto_Proprieta.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   ")
                    End If

                    If Mat_Cod <> 0 Then
                        StrSQL.Append(" AND Materie_PrimexLotto_Proprieta.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
                    End If

                    If Lotto_Cod1 <> 0 Or Lotto_Cod2 <> 0 Or Lotto_Cod3 <> 0 Then
                        StrSQL.Append(" AND (Materie_PrimexLotto_Proprieta.Lotto_Cod1 = " & Lotto_Cod1 & " And Materie_PrimexLotto_Proprieta.Lotto_Val1 = '" & Agro_SQL_SaveText(Trim(Lotto_Val1)) & "' Or Materie_PrimexLotto_Proprieta.Lotto_Cod1 = " & Lotto_Cod2 & " And Materie_PrimexLotto_Proprieta.Lotto_Val1 = '" & Agro_SQL_SaveText(Trim(Lotto_Val2)) & "' Or Materie_PrimexLotto_Proprieta.Lotto_Cod1 = " & Lotto_Cod3 & " And Materie_PrimexLotto_Proprieta.Lotto_Val1 = '" & Agro_SQL_SaveText(Trim(Lotto_Val3)) & "')")
                        StrSQL.Append(" AND (Materie_PrimexLotto_Proprieta.Lotto_Cod2 = " & Lotto_Cod1 & " And Materie_PrimexLotto_Proprieta.Lotto_Val2 = '" & Agro_SQL_SaveText(Trim(Lotto_Val1)) & "' Or Materie_PrimexLotto_Proprieta.Lotto_Cod2 = " & Lotto_Cod2 & " And Materie_PrimexLotto_Proprieta.Lotto_Val2 = '" & Agro_SQL_SaveText(Trim(Lotto_Val2)) & "' Or Materie_PrimexLotto_Proprieta.Lotto_Cod2 = " & Lotto_Cod3 & " And Materie_PrimexLotto_Proprieta.Lotto_Val2 = '" & Agro_SQL_SaveText(Trim(Lotto_Val3)) & "')")
                        StrSQL.Append(" AND (Materie_PrimexLotto_Proprieta.Lotto_Cod3 = " & Lotto_Cod1 & " And Materie_PrimexLotto_Proprieta.Lotto_Val3 = '" & Agro_SQL_SaveText(Trim(Lotto_Val1)) & "' Or Materie_PrimexLotto_Proprieta.Lotto_Cod3 = " & Lotto_Cod2 & " And Materie_PrimexLotto_Proprieta.Lotto_Val3 = '" & Agro_SQL_SaveText(Trim(Lotto_Val2)) & "' Or Materie_PrimexLotto_Proprieta.Lotto_Cod3 = " & Lotto_Cod3 & " And Materie_PrimexLotto_Proprieta.Lotto_Val3 = '" & Agro_SQL_SaveText(Trim(Lotto_Val3)) & "')")
                    End If

                    If ID_Proprieta <> 0 Then
                        StrSQL.Append(" AND Materie_PrimexLotto_Proprieta.ID_Proprieta = " & Agro_SQL_SaveNum(ID_Proprieta) & "   ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Materie_PrimexLotto_Proprieta.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Materie_PrimexLotto_Proprieta.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Materie_PrimexLotto_Proprieta.Piva, Materie_PrimexLotto_Proprieta.Elem_Cod, Materie_PrimexLotto_Proprieta.Pro_Cod, Materie_PrimexLotto_Proprieta.Mat_Cod, Materie_PrimexLotto_Proprieta.ID_Proprieta")
                    End If



                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta



                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni


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

End Class
