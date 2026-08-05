Imports System.Text
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi


Public Class Codifica_SpecieVegetali_Dogane_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal NC_COD As String,
                          ByVal Veg_Cod As Integer,
                          ByVal Grva_Cod As Integer,
                          ByVal Cul_Cod As Integer,
                          ByVal Grfi_Cod As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Dogane_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT * ")
            strSql.AppendLine(" FROM  Codifica_SpecieVegetali_Dogane ")
            strSql.AppendLine(" WHERE  1=1  ")

            If NC_COD <> "" Then
                strSql.AppendLine(" AND    NC_Cod = '" & Agro_SQL_SaveText(NC_COD) & "'   ")
            End If

            If Veg_Cod <> 0 Then
                strSql.AppendLine(" AND     Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & "   ")
            End If

            If Cul_Cod <> 0 Then
                strSql.AppendLine(" AND     Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & "   ")
            End If

            If Grfi_Cod <> 0 Then
                strSql.AppendLine(" AND     Grfi_Cod = " & Agro_SQL_SaveNum(Grfi_Cod) & "   ")
            End If

            If Grva_Cod <> 0 Then
                strSql.AppendLine(" AND     Grva_Cod = " & Agro_SQL_SaveNum(Grva_Cod) & "   ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return dt

    End Function

    '###################################################################################
    'fa sempre le query
    'DA TESTARE
    Public Function Recupera_NCcod_from_VegGrvaCulGrfiCod1(ByVal Veg_Cod As Integer,
                                               ByVal Grva_Cod As Integer,
                                                  ByVal Cul_Cod As Integer,
                                                  ByVal Grfi_Cod As Integer,
                                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                  ) As String

        Dim nomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Dogane_R.Recupera_NCcod_from_VegGrvaCulGrfiCod1()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt1, dt2, dt3, dt4 As DataTable
        Dim NC_cod As String = ""

        Try

            dt1 = Leggi("",
                       Veg_Cod, 0, 0, 0,
                       "", "",
                        objParametri)

            If Not dt1 Is Nothing AndAlso dt1.Rows.Count > 0 Then

                Select Case dt1.Rows.Count
                    Case 1
                        NC_cod = dt1.Rows(0).Item("NC_cod")
                    Case > 1

                        dt2 = Leggi("",
                                     Veg_Cod, Grva_Cod, 0, 0,
                                     "", "",
                                      objParametri)
                        If Not dt2 Is Nothing AndAlso dt2.Rows.Count > 0 Then

                            Select Case dt2.Rows.Count
                                Case 1
                                    NC_cod = dt2.Rows(0).Item("NC_cod")
                                Case > 1
                                    dt3 = Leggi("",
                                                Veg_Cod, Grva_Cod, Cul_Cod, 0,
                                                "", "",
                                                 objParametri)
                                    If Not dt3 Is Nothing AndAlso dt3.Rows.Count > 0 Then

                                        Select Case dt3.Rows.Count
                                            Case 1
                                                NC_cod = dt3.Rows(0).Item("NC_cod")
                                            Case > 1

                                                dt4 = Leggi("",
                                                           Veg_Cod, Grva_Cod, Cul_Cod, Grfi_Cod,
                                                           "", "",
                                                            objParametri)
                                                If Not dt4 Is Nothing AndAlso dt4.Rows.Count > 0 Then

                                                    Select Case dt4.Rows.Count
                                                        Case 1
                                                            NC_cod = dt4.Rows(0).Item("NC_cod")
                                                        Case > 1
                                                            Throw New Exception("Trovati più NC_Cod per i dati inviati.")
                                                    End Select
                                                End If

                                        End Select
                                    End If
                            End Select
                        End If
                End Select


            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
            Return ""
        End Try

        Return NC_cod

    End Function


    '###################################################################################
    'passato il dt fa le select
    'DA TESTARE
    Public Function Recupera_NCcod_from_VegGrvaCulGrfiCod2(ByVal DT As DataTable,
                                                           ByVal Veg_Cod As Integer,
                                                           ByVal Grva_Cod As Integer,
                                                              ByVal Cul_Cod As Integer,
                                                              ByVal Grfi_Cod As Integer
                                                             ) As String

        Dim nomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Dogane_R.Recupera_NCcod_from_VegGrvaCulGrfiCod2()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dr1 As DataRow()
        Dim dr2 As DataRow()
        Dim dr3 As DataRow()
        Dim dr4 As DataRow()

        Dim NC_cod As String = ""

        Try

            If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then

                dr1 = DT.Select(" Veg_Cod = " & CStr(Veg_Cod))

                Select Case dr1.Length
                    Case 1
                        NC_cod = dr1(0).Item("NC_cod")
                    Case > 1

                        dr2 = DT.Select(" Veg_Cod = " & CStr(Veg_Cod) & " AND  Grva_Cod = " & CStr(Grva_Cod))
                        If Not dr2 Is Nothing AndAlso dr2.Length > 0 Then

                            Select Case dr2.Length
                                Case 1
                                    NC_cod = dr2(0).Item("NC_cod")
                                Case > 1
                                    dr3 = DT.Select(" Veg_Cod = " & CStr(Veg_Cod) & " AND  Grva_Cod = " & CStr(Grva_Cod) & " AND  Cul_Cod = " & CStr(Cul_Cod))
                                    If Not dr3 Is Nothing AndAlso dr3.Length > 0 Then

                                        Select Case dr3.Length
                                            Case 1
                                                NC_cod = dr3(0).Item("NC_cod")
                                            Case > 1

                                                dr4 = DT.Select(" Veg_Cod = " & CStr(Veg_Cod) & " AND  Grva_Cod = " & CStr(Grva_Cod) & " AND  Cul_Cod = " & CStr(Cul_Cod) & " AND  Grfi_Cod = " & CStr(Grfi_Cod))
                                                If Not dr4 Is Nothing AndAlso dr4.Length > 0 Then

                                                    Select Case dr4.Length
                                                        Case 1
                                                            NC_cod = dr4(0).Item("NC_cod")
                                                        Case > 1
                                                            Throw New Exception("Trovati più NC_Cod per i dati inviati.")
                                                    End Select
                                                End If

                                        End Select
                                    End If
                            End Select
                        End If
                End Select


            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
            Return ""
        End Try

        Return NC_cod

    End Function

    '###################################################################################
    Public Function Recupera_NCcod_from_VegCod(ByVal DT As DataTable,
                                               ByVal Veg_Cod As Integer,
                                               ByRef Flag_Doppioni As Boolean
                                               ) As String

        Dim nomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Dogane_R.Recupera_NCcod_from_VegCod()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dr1 As DataRow()
        Dim NC_cod As String = ""
        Flag_Doppioni = False

        Try

            If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then

                dr1 = DT.Select(" Veg_Cod = " & CStr(Veg_Cod))

                Select Case dr1.Count
                    Case 1
                        NC_cod = dr1(0).Item("NC_cod")
                    Case > 1
                        NC_cod = ""
                        Flag_Doppioni = True
                End Select

            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
            Return ""
        End Try

        Return NC_cod

    End Function

End Class


