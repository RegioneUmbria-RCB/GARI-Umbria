Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi


Public Class ImpiantiIrrigazioni_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Da usare con Selezione_TabellaCompleta
    ''' </summary>
    ''' <param name="Imp_Cod"></param>
    ''' <param name="xSelezioneVariabile"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	17/01/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function Leggi(ByVal Imp_Cod As Integer, _
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.ImpiantiIrrigazioni_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  ImpiantiIrrigazioni ")
                    StrSQL.Append(" WHERE Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Imp_Cod >= 0 Then
                        StrSQL.Append(" AND ImpiantiIrrigazioni.Imp_Cod =  " & Agro_SQL_SaveNum(Imp_Cod) & "  ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Imp_Des ASC ")
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta


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

    Function Imp_Des_From_Imp_Cod(Imp_Cod As Integer, objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String
        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.ImpiantiIrrigazioni_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT Imp_Des ")
            StrSQL.Append(" FROM  ImpiantiIrrigazioni ")
            StrSQL.Append(" WHERE Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Imp_Cod >= 0 Then
                StrSQL.Append(" AND IMP_COD =  " & Agro_SQL_SaveNum(Imp_Cod) & "  ")
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

        If IsDBNull(DT.Rows(0).Item("Imp_Des")) Then
            Return ""
        End If
        Return CStr(DT.Rows(0).Item("Imp_Des"))
    End Function


    '##############################################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Da usare con Selezione_TabellaCompleta
    ''' </summary>
    ''' <param name="Imp_Cod"></param>
    ''' <param name="xSelezioneVariabile"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	17/01/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function Leggi_ImpiantiIrrigazioni_APP(
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.ImpiantiIrrigazioni_R.Leggi_ImpiantiIrrigazioni_APP()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT Imp_Cod, Imp_Des ")
                    StrSQL.Append(" FROM  ImpiantiIrrigazioni ")
                    StrSQL.Append(" WHERE Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Imp_Des ASC ")
                    End If


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta


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

    Public Function LeggiImpiantoIrrigazioneDefaultPerImpiantoColturale(ByVal piva As String,
                                                                    ByVal saCod As Integer,
                                                                    ByVal appezza As Integer,
                                                                    ByVal idReg As Integer,
                                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.ImpiantiIrrigazioni_R.LeggiImpiantoIrrigazioneDefaultPerImpiantoColturale()"
        Dim MessaggioErrore As String = ""
        Dim sb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            sb.Length = 0

            sb.AppendLine(" SELECT ")
            sb.AppendLine("         irri.Imp_Cod, irri.Imp_Des ")
            sb.AppendLine(" FROM  ")
            sb.AppendLine("         Reg_Impianti imp")
            sb.AppendLine(" JOIN ")
            sb.AppendLine("         ImpiantiIrrigazioni irri ")
            sb.AppendLine("         ON irri.Imp_Cod = ISNULL(imp.IMP_COD, 0)")
            sb.AppendLine(" WHERE ")
            sb.AppendLine("         imp.PIVA = '" & Agro_SQL_SaveText(piva) & "' ")
            sb.AppendLine("         AND imp.Sa_Cod = " & Agro_SQL_SaveNum(saCod) & " ")
            sb.AppendLine("         AND imp.Appezza = " & Agro_SQL_SaveNum(appezza) & " ")
            sb.AppendLine("         AND imp.Id_Reg = " & Agro_SQL_SaveNum(idReg) & " ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, sb.ToString, NomeRoutine)
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



Public Class ImpiantiIrrigazioniXSpecie
    Inherits AgronicaCoreDataProvider.DataProvider



    '###################################################################################
    Public Function Esiste_VegCodXImpCod(ByVal Veg_Cod As Integer, _
                                            ByVal Imp_Cod As Integer, _
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                            ) As Integer

        Dim NomeRoutine As String = "Esiste_VegCodXImpCod()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim risp As Boolean = False

        Try

            DT = Leggi(Veg_Cod, _
                        Imp_Cod, _
                        objParametri)

            If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then
                risp = True
            End If

            DT = Nothing

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return risp

    End Function


    Public Function Leggi(ByVal Veg_Cod As Integer,
                                            ByVal Imp_Cod As Integer,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.ImpiantiIrrigazioni_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            StrSQL.Length = 0

            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  [ImpiantiIrrigazionixSpecie] ")
            StrSQL.Append(" WHERE Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            StrSQL.Append(" AND Veg_Cod =  " & Agro_SQL_SaveNum(Veg_Cod) & "  ")
            StrSQL.Append(" AND IMP_COD =  " & Agro_SQL_SaveNum(Imp_Cod) & "  ")


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

    Public Function LeggiImpiantiIrrigazioni(ByVal Veg_Cod As Integer,
                                            ByVal Imp_Cod As Integer,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.ImpiantiIrrigazioni_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            StrSQL.Length = 0

            StrSQL.Append(" SELECT imp.Imp_Cod, imp.Imp_Des " & vbCrLf)
            StrSQL.Append(" FROM  ImpiantiIrrigazioni imp " & vbCrLf)
            StrSQL.Append(" INNER JOIN ImpiantiIrrigazionixSpecie imps ON imp.imp_cod = imps.imp_cod " & vbCrLf)
            StrSQL.Append(" WHERE imp.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " & vbCrLf)
            StrSQL.Append(" AND   imp.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " " & vbCrLf)

            If Veg_Cod <> 0 Then
                StrSQL.Append(" AND imps.Veg_Cod =  " & Agro_SQL_SaveNum(Veg_Cod) & "  " & vbCrLf)
            End If
            If Imp_Cod <> 0 Then
                StrSQL.Append(" AND imp.IMP_COD =  " & Agro_SQL_SaveNum(Imp_Cod) & "  " & vbCrLf)
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