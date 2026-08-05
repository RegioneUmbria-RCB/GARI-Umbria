Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class EfficienzaxTipiAllevamentixEffluenti_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##########################################################################################################
    Public Function Efficienza_From_ALLCOD_EmCod_EffCod_Dose(ByVal ALL_COD As Integer,
                                                             ByVal Em_Cod As Integer,
                                                             ByVal Eff_Cod As Integer,
                                                             ByVal Dose As Integer,
                                                             ByVal Regolamento_Cod As Integer,
                                                             ByRef objParametri As AgronicaCoreParametri
                                                             ) As Decimal

        Const nomeRoutine = "EfficienzaxTipiAllevamentixEffluenti_R.Efficienza_From_ALLCOD_EmCod_EffCod_Dose()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Dim Efficienza As Decimal = 0

        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT Efficienza_Val  ")
            strSql.AppendLine(" FROM   EfficienzaxTipiAllevamentixEffluenti INNER JOIN ")
            strSql.AppendLine(" EpocheModalita ON EfficienzaxTipiAllevamentixEffluenti.Efficienza_Cod = EpocheModalita.Efficienza_Cod ")
            strSql.AppendLine("  AND  EfficienzaxTipiAllevamentixEffluenti.Regolamento_Cod = EpocheModalita.Regolamento_Cod ")

            strSql.AppendLine(" WHERE EfficienzaxTipiAllevamentixEffluenti.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            strSql.AppendLine(" AND   EfficienzaxTipiAllevamentixEffluenti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            strSql.AppendLine(" AND   EfficienzaxTipiAllevamentixEffluenti.Dose = " & Agro_SQL_SaveNum(Dose) & " ")

            If ALL_COD <> 0 Then
                strSql.AppendLine(" AND EfficienzaxTipiAllevamentixEffluenti.ALL_COD =  " & Agro_SQL_SaveNum(ALL_COD) & "  ")
            End If

            If Em_Cod <> 0 Then
                strSql.AppendLine(" AND EpocheModalita.EM_Cod =  " & Agro_SQL_SaveNum(Em_Cod) & "  ")
            End If

            If Eff_Cod <> 0 Then
                strSql.AppendLine(" AND EfficienzaxTipiAllevamentixEffluenti.Eff_Cod =  " & Agro_SQL_SaveNum(Eff_Cod) & "  ")
            End If

            If Regolamento_Cod <> 0 Then
                strSql.AppendLine(" AND EfficienzaxTipiAllevamentixEffluenti.Regolamento_Cod =  " & Agro_SQL_SaveNum(Regolamento_Cod) & "  ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                Efficienza = dt.Rows(0).Item("Efficienza_Val")
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return Efficienza

    End Function

    Public Function Leggi(ByVal ALL_COD As Integer,
                            ByVal Em_Cod As Integer,
                                ByVal Efficienza_Cod As Integer,
                            ByVal Eff_Cod As Integer,
                            ByVal Dose As Integer,
                            ByVal Regolamento_Cod As Integer,
                            ByVal Id_ClasseTessitura As Integer,
                            ByRef objParametri As AgronicaCoreParametri
                            ) As DataTable

        Const nomeRoutine = "EfficienzaxTipiAllevamentixEffluenti_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Dim Efficienza As Decimal = 0

        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT EfficienzaxTipiAllevamentixEffluenti.*,  ")
            strSql.AppendLine("        ISNULL(Considera_CoefficienteTempo_Efficienza,0) AS Considera_CoefficienteTempo_Efficienza  ")
            strSql.AppendLine(" FROM   EfficienzaxTipiAllevamentixEffluenti  ")
            If Em_Cod <> 0 Then
                strSql.AppendLine(" INNER JOIN EpocheModalita ON EfficienzaxTipiAllevamentixEffluenti.Efficienza_Cod = EpocheModalita.Efficienza_Cod ")
                strSql.AppendLine(" AND  EfficienzaxTipiAllevamentixEffluenti.Regolamento_Cod = EpocheModalita.Regolamento_Cod ")
            End If

            strSql.AppendLine(" INNER JOIN PUA_Regolamenti ON EfficienzaxTipiAllevamentixEffluenti.Regolamento_Cod = PUA_Regolamenti.Regolamento_Cod ")

            strSql.AppendLine(" WHERE EfficienzaxTipiAllevamentixEffluenti.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            strSql.AppendLine(" AND   EfficienzaxTipiAllevamentixEffluenti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Dose <> 0 Then
                strSql.AppendLine(" AND   EfficienzaxTipiAllevamentixEffluenti.Dose = " & Agro_SQL_SaveNum(Dose) & " ")
            End If

            If ALL_COD <> 0 Then
                strSql.AppendLine(" AND EfficienzaxTipiAllevamentixEffluenti.ALL_COD =  " & Agro_SQL_SaveNum(ALL_COD) & "  ")
            End If

            If Em_Cod <> 0 Then
                strSql.AppendLine(" AND EpocheModalita.EM_Cod =  " & Agro_SQL_SaveNum(Em_Cod) & "  ")
            End If

            If Eff_Cod <> 0 Then
                strSql.AppendLine(" AND EfficienzaxTipiAllevamentixEffluenti.Eff_Cod =  " & Agro_SQL_SaveNum(Eff_Cod) & "  ")
            End If

            If Efficienza_Cod <> 0 Then
                strSql.AppendLine(" AND EfficienzaxTipiAllevamentixEffluenti.Efficienza_Cod =  " & Agro_SQL_SaveNum(Efficienza_Cod) & "  ")
            End If

            If Regolamento_Cod <> 0 Then
                strSql.AppendLine(" AND EfficienzaxTipiAllevamentixEffluenti.Regolamento_Cod =  " & Agro_SQL_SaveNum(Regolamento_Cod) & "  ")
            End If

            If Id_ClasseTessitura <> 0 Then
                strSql.AppendLine(" AND EfficienzaxTipiAllevamentixEffluenti.Id_ClasseTessitura =  " & Agro_SQL_SaveNum(Id_ClasseTessitura) & "  ")
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

    '##########################################################################################################
    ''' <summary>
    ''' Default se non trovato è -1
    ''' </summary>
    Public Function Leggi_EfficienzaVal(ByVal ALL_COD As Integer,
                                        ByVal Efficienza_Cod As Integer,
                                        ByVal Eff_Cod As Integer,
                                        ByVal Dose As Integer,
                                        ByVal Regolamento_Cod As Integer,
                                        ByRef objParametri As AgronicaCoreParametri
                                        ) As Decimal

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.EfficienzaxTipiAllevamentixEffluenti_R.Leggi_EfficienzaVal()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Dim Efficienza As Decimal = -1

        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT Efficienza_Val  ")
            strSql.AppendLine(" FROM   EfficienzaxTipiAllevamentixEffluenti ")

            strSql.AppendLine(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            strSql.AppendLine(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            strSql.AppendLine(" AND   Dose = " & Agro_SQL_SaveNum(Dose) & " ")

            If ALL_COD <> 0 Then
                strSql.AppendLine(" AND ALL_COD =  " & Agro_SQL_SaveNum(ALL_COD) & "  ")
            End If

            If Efficienza_Cod <> 0 Then
                strSql.AppendLine(" AND Efficienza_Cod =  " & Agro_SQL_SaveNum(Efficienza_Cod) & "  ")
            End If

            If Eff_Cod <> 0 Then
                strSql.AppendLine(" AND Eff_Cod =  " & Agro_SQL_SaveNum(Eff_Cod) & "  ")
            End If

            If Regolamento_Cod <> 0 Then
                strSql.AppendLine(" AND Regolamento_Cod =  " & Agro_SQL_SaveNum(Regolamento_Cod) & "  ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                Efficienza = dt.Rows(0).Item("Efficienza_Val")
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return Efficienza

    End Function

    '##########################################################################################################
    Public Function EffCodFromFerCod(ByVal Fer_Cod As Integer, ByVal Regolamento_Cod As Integer,
                                     ByRef objParametri As AgronicaCoreParametri) As Integer

        Dim objEffluentixFertilizzanti As EffluentixFertilizzanti
        Dim Eff_Cod As Integer = 0

        Try
            objEffluentixFertilizzanti = New EffluentixFertilizzanti

            Dim dt As DataTable
            dt = objEffluentixFertilizzanti.Leggi(0, Fer_Cod, Regolamento_Cod, objParametri)

            If dt.Rows.Count > 0 Then
                Eff_Cod = dt.Rows(0).Item("Eff_Cod")
            End If


        Catch ex As Exception
            Eff_Cod = EffCodFromFerCod_old(Fer_Cod, Regolamento_Cod)
        End Try

        Return Eff_Cod

    End Function


    '##########################################################################################################
    Public Function EffCodFromFerCod_old(ByVal Fer_Cod As Integer, ByVal Regolamento_Cod As Integer) As Integer

        Dim Eff_Cod As Integer = 0

        ' commentare
        ' Dim Regolamento_Cod As Integer = 2

        If Regolamento_Cod = enum_PUARegolamenti.CBPA_Umbria Then

            Eff_Cod = 1

        Else

            Select Case Fer_Cod
                Case 3896
                    ' liquame zootecnico tal quale
                    Eff_Cod = 1
                Case 3897
                    ' liquame zootecnico chiarificato
                    Eff_Cod = 2
                Case 3898
                    ' ammendante
                    Eff_Cod = 3
                Case 3899
                    ' Palabile zootecnico
                    Eff_Cod = 4
                Case 3900
                    ' Liquame digestato tal quale
                    Eff_Cod = 5
                Case 3901
                    ' liquame digestato chiarificato
                    Eff_Cod = 6
                Case 3902
                    ' Palabile da digestato
                    Eff_Cod = 7
                Case Else
                    Eff_Cod = 0
            End Select

        End If

        Return Eff_Cod

    End Function

End Class
