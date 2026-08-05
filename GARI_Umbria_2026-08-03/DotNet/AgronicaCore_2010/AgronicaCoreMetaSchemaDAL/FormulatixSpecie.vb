Imports AgronicaCoreDataProvider.UtilityProvider


Public Class FormulatixSpecie_R

    Inherits AgronicaCoreDataProvider.DataProvider


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="FOR_VEG_COD"></param>
    ''' <param name="FR_COD"></param>
    ''' <param name="VEG_COD"></param>
    ''' <param name="CUL_COD"></param>
    ''' <param name="GRSP_COD"></param>
    ''' <param name="GRVA_COD"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[magnani]	22/04/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function Leggi(ByVal TestoRicerca As String,
                          ByVal FOR_VEG_COD As Int32,
                          ByVal FR_COD As Integer,
                          ByVal VEG_COD As Integer,
                          ByVal CUL_COD As Integer,
                          ByVal GRSP_COD As Integer,
                          ByVal GRVA_COD As Integer,
                          ByVal TipoRichiesto As Integer,
                          ByVal Grfi_Cod As Integer,
                          ByVal Copertura As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          Optional ByVal Data As String = ""
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.FormulatixSpecie_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try


            StrSQL.Length = 0

            StrSQL.Append(" SELECT * FROM FormulatixSpecieVegetali " &
                                  " INNER JOIN Formulati ON FormulatixSpecieVegetali.FR_COD = Formulati.FR_COD " &
                                  " INNER JOIN FormulatixClassificazioni ON FormulatixClassificazioni.FOR_COD = Formulati.FR_COD ")

            StrSQL.Append(" WHERE 1=1 ")

            If Data <> "" AndAlso IsDate(Data) Then
                StrSQL.Append(" AND FormulatixSpecieVegetali.Validita_inizio <= " & Agro_SQL_SaveDate(Data) & " " &
                                " AND   FormulatixSpecieVegetali.Validita_Fine >= " & Agro_SQL_SaveDate(Data) & " " &
                                " AND   Formulati.Validita_inizio <= " & Agro_SQL_SaveDate(Data) & " " &
                                " AND   Formulati.Validita_Fine >= " & Agro_SQL_SaveDate(Data) & " ")
            End If

            If TestoRicerca <> "" Then
                StrSQL.Append(" AND Formulati.FR_DES LIKE '%" & TestoRicerca & "%'   ")
            End If

            If FOR_VEG_COD <> 0 Then
                StrSQL.Append(" AND FormulatixSpecieVegetali.FOR_VEG_COD =  " & Agro_SQL_SaveNum(FOR_VEG_COD) & "  ")
            End If

            If FR_COD <> 0 Then
                StrSQL.Append(" AND FormulatixSpecieVegetali.FR_COD =  " & Agro_SQL_SaveNum(FR_COD) & "  ")
            End If

            If VEG_COD <> 0 Then
                StrSQL.Append(" AND (FormulatixSpecieVegetali.VEG_COD =  " & Agro_SQL_SaveNum(VEG_COD) & "  " &
                                      " OR FormulatixSpecieVegetali.Grsp_Cod IN  ( SELECT  GruppoColturaleXSpecieVegetali.Grsp_Cod " &
                                                                                  " FROM   GruppoColturaleXSpecieVegetali          " &
                                                                                  " WHERE  GruppoColturaleXSpecieVegetali.Veg_Cod = " & Agro_SQL_SaveNum(VEG_COD) & "))  ")

            End If

            If CUL_COD <> 0 Then
                StrSQL.Append(" AND FormulatixSpecieVegetali.CUL_COD =  " & Agro_SQL_SaveNum(CUL_COD) & "  ")
            End If

            If GRSP_COD <> 0 Then
                StrSQL.Append(" AND FormulatixSpecieVegetali.GRSP_COD =  " & Agro_SQL_SaveNum(GRSP_COD) & "  ")
            End If

            If GRVA_COD <> 0 Then
                StrSQL.Append(" AND FormulatixSpecieVegetali.GRVA_COD =  " & Agro_SQL_SaveNum(GRVA_COD) & "  ")
            End If

            '(11/05/2018 fede) aggiunte verifiche finalità e copertura 
            If Grfi_Cod <> 0 Then

                StrSQL.Append("  AND ( FormulatixSpecieVegetali.grfi_cod = 0  ")
                StrSQL.Append("  OR (FormulatixSpecieVegetali.grfi_cod <> 0 and   ")
                StrSQL.Append("     FormulatixSpecieVegetali.grfi_cod = " & Agro_SQL_SaveNum(Grfi_Cod) & "  ")
                StrSQL.Append(" )  ")
                StrSQL.Append(" )  ")

            End If


            Select Case Copertura

                    Case 0 'solo fuori campo (fuori campo + non specificato)
                        StrSQL.Append("  and  FormulatixSpecieVegetali.Flag_Protetto <> 1  ")

                    Case 1 'solo serra (serra + non specificato)
                        StrSQL.Append("  and  FormulatixSpecieVegetali.Flag_Protetto <> 2  ")

                Case Else 'entrambi (non specificato)
                    StrSQL.Append("  and  FormulatixSpecieVegetali.Flag_Protetto = 0  ")

                End Select


            Select Case TipoRichiesto

                Case 0  'Tutti i formulati

                    StrSQL.Append(" And  FormulatixClassificazioni.CLASS_COD IN  (101,102,103,104,105,106,107,108,109,110,300,610,611,  200,201,202,203,  400,401,402,403,404,405,407,408,409,411,412,413,414,415,418,  608,  603,  607,  500,501,502,503,504,505,506,606,602,613,617,1003) ")

                Case 1  'Trattamento Antiparassitario

                    StrSQL.Append(" And  FormulatixClassificazioni.CLASS_COD IN  (101,102,103,104,105,106,107,108,109,110,610,611,606) ")

                Case 2  'Diserbo

                    StrSQL.Append(" And  FormulatixClassificazioni.CLASS_COD IN  (200,201,202,203) ")

                Case 3  'Trattamenti Fitoregolatori

                    StrSQL.Append(" And  FormulatixClassificazioni.CLASS_COD IN (400,401,402,403,404,405,407,408,409,411,412,413,414,415,418)  ")

                Case 4  'Coadiuvanti, Bagnanti, Antischiuma

                    StrSQL.Append(" And  (FormulatixClassificazioni.CLASS_COD >= 500 AND FormulatixClassificazioni.CLASS_COD <= 506)  ")

                Case 5  'Concia

                    StrSQL.Append(" And  FormulatixClassificazioni.CLASS_COD IN (608)  ")

                Case 6  'Disseccamento

                    StrSQL.Append(" And  FormulatixClassificazioni.CLASS_COD IN (603)  ")

                Case 7  'Geodisinfestazione

                    StrSQL.Append(" And  FormulatixClassificazioni.CLASS_COD IN (607)  ")

                Case 8  'Trattamenti Antiparassitari + Concianti

                    StrSQL.Append(" And  FormulatixClassificazioni.CLASS_COD IN  (101,102,103,104,105,106,107,108,109,110,610,611,608,606) ")

                Case 9  'Diserbo + Disseccanti

                    StrSQL.Append(" And  FormulatixClassificazioni.CLASS_COD IN  (200,201,202,203,603) ")

                Case 10  'Trattamenti Antiparassitari + Geodisinfestanti

                    StrSQL.Append(" And  FormulatixClassificazioni.CLASS_COD IN  (101,102,103,104,105,106,107,108,109,110,610,611,607,606) ")

            End Select

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Formulati.FR_DES ASC ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Leggi_FormulatixSpecieVegetalixNormative(ByVal TestoRicerca As String,
                            ByVal FOR_VEG_COD As Int32,
                            ByVal FR_COD As Integer,
                            ByVal VEG_COD As Integer,
                            ByVal CUL_COD As Integer,
                            ByVal GRSP_COD As Integer,
                            ByVal GRVA_COD As Integer,
                            ByVal TipoRichiesto As Integer,
                                ByVal Grfi_Cod As Integer,
                                ByVal Copertura As Integer,
                           ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            Optional ByVal Data As String = ""
                            ) As DataTable

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.FormulatixSpecieVegetalixNormative_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT * FROM FormulatixSpecieVegetalixNormative " &
                                  " INNER JOIN Formulati ON FormulatixSpecieVegetalixNormative.FR_COD = Formulati.FR_COD " &
                                  " INNER JOIN FormulatixClassificazioni ON FormulatixClassificazioni.FOR_COD = Formulati.FR_COD ")

            StrSQL.Append(" WHERE 1=1 ")

            If Data <> "" AndAlso IsDate(Data) Then

                '(06/09/2019 fede) modificato controllo per gestire il fine scorta
                StrSQL.Append("   AND ( " &
                                "  (  FormulatixSpecieVegetalixNormative.validita_Inizio<=" & Agro_SQL_SaveDate(Data) & vbCrLf &
                                "  AND  FormulatixSpecieVegetalixNormative.validita_fine>=" & Agro_SQL_SaveDate(Data) & ")" & vbCrLf &
                                "  OR " &
                                "  (  FormulatixSpecieVegetalixNormative.DataSmaltimentoScorte >= " & Agro_SQL_SaveDate(Data) & ")" & vbCrLf &
                                "      )")
            End If

            If TestoRicerca <> "" Then
                StrSQL.Append(" AND Formulati.FR_DES LIKE '%" & TestoRicerca & "%'   ")
            End If

            If FOR_VEG_COD <> 0 Then
                StrSQL.Append(" AND FormulatixSpecieVegetalixNormative.FOR_VEG_COD =  " & Agro_SQL_SaveNum(FOR_VEG_COD) & "  ")
            End If

            If FR_COD <> 0 Then
                StrSQL.Append(" AND FormulatixSpecieVegetalixNormative.FR_COD =  " & Agro_SQL_SaveNum(FR_COD) & "  ")
            End If

            If VEG_COD <> 0 Then
                StrSQL.Append(" AND (FormulatixSpecieVegetalixNormative.VEG_COD =  " & Agro_SQL_SaveNum(VEG_COD) & "  " &
                                      " OR FormulatixSpecieVegetalixNormative.Grsp_Cod IN  ( SELECT  GruppoColturaleXSpecieVegetali.Grsp_Cod " &
                                                                                  " FROM   GruppoColturaleXSpecieVegetali          " &
                                                                                  " WHERE  GruppoColturaleXSpecieVegetali.Veg_Cod = " & Agro_SQL_SaveNum(VEG_COD) & "))  ")

            End If

            If CUL_COD <> 0 Then
                StrSQL.Append(" AND FormulatixSpecieVegetalixNormative.CUL_COD =  " & Agro_SQL_SaveNum(CUL_COD) & "  ")
            End If

            If GRSP_COD <> 0 Then
                StrSQL.Append(" AND FormulatixSpecieVegetalixNormative.GRSP_COD =  " & Agro_SQL_SaveNum(GRSP_COD) & "  ")
            End If

            If GRVA_COD <> 0 Then
                StrSQL.Append(" AND FormulatixSpecieVegetalixNormative.GRVA_COD =  " & Agro_SQL_SaveNum(GRVA_COD) & "  ")
            End If

            '(11/05/2018 fede) aggiunte verifiche finalità e copertura 
            If Grfi_Cod <> 0 Then

                StrSQL.Append("  AND ( FormulatixSpecieVegetalixNormative.grfi_cod = 0  ")
                StrSQL.Append("  OR (FormulatixSpecieVegetalixNormative.grfi_cod <> 0 and   ")
                StrSQL.Append("     FormulatixSpecieVegetalixNormative.grfi_cod = " & Agro_SQL_SaveNum(Grfi_Cod) & "  ")
                StrSQL.Append(" )  ")
                StrSQL.Append(" )  ")

            End If


            Select Case Copertura

                Case 0 'solo fuori campo (fuori campo + non specificato)
                    StrSQL.Append("  and  FormulatixSpecieVegetalixNormative.Flag_Protetto <> 1  ")

                Case 1 'solo serra (serra + non specificato)
                    StrSQL.Append("  and  FormulatixSpecieVegetalixNormative.Flag_Protetto <> 2  ")

                Case Else 'entrambi (non specificato)
                    StrSQL.Append("  and  FormulatixSpecieVegetalixNormative.Flag_Protetto = 0  ")

            End Select


            Select Case TipoRichiesto

                Case 0  'Tutti i formulati

                    StrSQL.Append(" And  FormulatixClassificazioni.CLASS_COD IN  (101,102,103,104,105,106,107,108,109,110,300,610,611,  200,201,202,203,  400,401,402,403,404,405,407,408,409,411,412,413,414,415,418,  608,  603,  607,  500,501,502,503,504,505,506,606,602,613,617,1003) ")

                Case 1  'Trattamento Antiparassitario

                    StrSQL.Append(" And  FormulatixClassificazioni.CLASS_COD IN  (101,102,103,104,105,106,107,108,109,110,610,611,606) ")

                Case 2  'Diserbo

                    StrSQL.Append(" And  FormulatixClassificazioni.CLASS_COD IN  (200,201,202,203) ")

                Case 3  'Trattamenti Fitoregolatori

                    StrSQL.Append(" And  FormulatixClassificazioni.CLASS_COD IN (400,401,402,403,404,405,407,408,409,411,412,413,414,415,418)  ")

                Case 4  'Coadiuvanti, Bagnanti, Antischiuma

                    StrSQL.Append(" And  (FormulatixClassificazioni.CLASS_COD >= 500 AND FormulatixClassificazioni.CLASS_COD <= 506)  ")

                Case 5  'Concia

                    StrSQL.Append(" And  FormulatixClassificazioni.CLASS_COD IN (608)  ")

                Case 6  'Disseccamento

                    StrSQL.Append(" And  FormulatixClassificazioni.CLASS_COD IN (603)  ")

                Case 7  'Geodisinfestazione

                    StrSQL.Append(" And  FormulatixClassificazioni.CLASS_COD IN (607)  ")

                Case 8  'Trattamenti Antiparassitari + Concianti

                    StrSQL.Append(" And  FormulatixClassificazioni.CLASS_COD IN  (101,102,103,104,105,106,107,108,109,110,610,611,608,606) ")

                Case 9  'Diserbo + Disseccanti

                    StrSQL.Append(" And  FormulatixClassificazioni.CLASS_COD IN  (200,201,202,203,603) ")

                Case 10  'Trattamenti Antiparassitari + Geodisinfestanti

                    StrSQL.Append(" And  FormulatixClassificazioni.CLASS_COD IN  (101,102,103,104,105,106,107,108,109,110,610,611,607,606) ")

            End Select

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Formulati.FR_DES ASC ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    '#############################################################################################################
    '#############################################################################################################
    '#############################################################################################################

End Class
