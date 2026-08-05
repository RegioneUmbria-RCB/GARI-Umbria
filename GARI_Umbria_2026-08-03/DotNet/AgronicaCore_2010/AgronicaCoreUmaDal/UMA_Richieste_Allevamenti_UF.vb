Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class UMA_Richieste_Allevamenti_UF_R
    Inherits DataProvider

    Public Function LeggiColtureUF(ByVal piva As String,
                                   ByVal richiestaCod As Integer,
                                   ByVal programmazioneCod As Integer,
                                   ByVal tipoTerritorio As Integer,
                                   ByVal occupazione_Cod As String,
                                   ByVal destinazione_Cod As String,
                                   ByVal uso_Cod As String,
                                   ByVal qualita_Cod As String,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByVal xOrderBy As String,
                                   ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Richieste_Allevamenti_UF_R.LeggiColtureUF()"

        Dim dt As DataTable
        Dim strSql As New StringBuilder()

        Try

            strSql.Length = 0

            strSql.AppendLine("with occupazione_CTE as ( ")
            strSql.AppendLine(" Select occupazione_Cod, MAX(occupazione_Des) as occupazione_Des From Codifica_SpecieVegetali_Agea_2015_2020 GROUP BY occupazione_Cod")
            strSql.AppendLine("), ")

            strSql.AppendLine("destinazione_CTE as ( ")
            strSql.AppendLine(" Select Destinazione_Cod, MAX(destinazione_Des) As Destinazione_Des From Codifica_SpecieVegetali_Agea_2015_2020 GROUP BY destinazione_Cod")
            strSql.AppendLine("), ")

            strSql.AppendLine("uso_CTE as ( ")
            strSql.AppendLine(" select Uso_Cod, MAX(Uso_Des) As Uso_Des From Codifica_SpecieVegetali_Agea_2015_2020 GROUP BY Uso_Cod")
            strSql.AppendLine("), ")

            strSql.AppendLine("qualita_CTE as ( ")
            strSql.AppendLine(" Select Qualita_Cod, MAX(Qualita_Des) As Qualita_Des From Codifica_SpecieVegetali_Agea_2015_2020 GROUP BY Qualita_Cod")
            strSql.AppendLine("), ")

            strSql.AppendLine("macrouso_CTE as ( ")
            strSql.AppendLine(" select DISTINCT Macrouso_UMA_Cod, Macrouso_UMA_Des From Codifica_SpecieVegetali_Agea_2015_2020 ")
            strSql.AppendLine(") ")

            strSql.AppendLine("SELECT DISTINCT ISNULL(octe.Occupazione_Des, '') as Occupazione_Des, ISNULL(ucte.Uso_Des, '') as Uso_Des, ISNULL(qcte.Qualita_Des, '') as Qualita_Des, ISNULL(dcte.Destinazione_Des, '') as Destinazione_Des, ISNULL(mcte.Macrouso_UMA_Des, '') as Macrouso_UMA_Des, ")
            strSql.AppendLine(" UMA_Richieste_Allevamenti_UF.Occupazione_Cod, UMA_Richieste_Allevamenti_UF.Uso_Cod, UMA_Richieste_Allevamenti_UF.Qualita_Cod, UMA_Richieste_Allevamenti_UF.Destinazione_Cod, 'NO' As Produce_UF, ")
            strSql.AppendLine(" fas.Programmazione_Des, UMA_Richieste_Allevamenti_UF.*, case when UMA_Richieste_Allevamenti_UF.Tipo_Territorio = 1 then 'Interno Umbria' else 'Esterno Umbria' END as TipoTerritorio_Des, ")
            strSql.AppendLine(" ROW_NUMBER() OVER(ORDER BY UMA_Richieste_Allevamenti_UF.Programmazione_Cod DESC) As ID ")
            strSql.AppendLine("FROM UMA_Richieste_Allevamenti_UF ")
            strSql.AppendLine("LEFT join Programmazione_Testata fas on fas.Programmazione_Cod = UMA_Richieste_Allevamenti_UF.Programmazione_Cod")
            strSql.AppendLine("left join destinazione_CTE dcte on dcte.Destinazione_Cod = UMA_Richieste_Allevamenti_UF.Destinazione_Cod")
            strSql.AppendLine("left join occupazione_CTE octe on octe.Occupazione_Cod = UMA_Richieste_Allevamenti_UF.Occupazione_Cod")
            strSql.AppendLine("left join uso_CTE ucte on ucte.Uso_Cod = UMA_Richieste_Allevamenti_UF.Uso_Cod")
            strSql.AppendLine("left join qualita_CTE qcte on qcte.Qualita_Cod = UMA_Richieste_Allevamenti_UF.Qualita_Cod")
            strSql.AppendLine("join macrouso_CTE mcte on mcte.Macrouso_UMA_Cod = UMA_Richieste_Allevamenti_UF.Macrouso_Uma_Cod")
            strSql.AppendLine("WHERE 1=1 ")

            If piva <> "" Then
                strSql.AppendLine("And UMA_Richieste_Allevamenti_UF.Piva = '" & Agro_SQL_SaveText(piva) & "'")
            End If
            If richiestaCod <> 0 Then
                strSql.AppendLine("AND UMA_Richieste_Allevamenti_UF.Richiesta_Cod = " & richiestaCod)
            End If
            If programmazioneCod <> 0 Then
                strSql.AppendLine("AND UMA_Richieste_Allevamenti_UF.Programmazione_Cod = " & programmazioneCod)
            End If
            If tipoTerritorio <> 0 Then
                strSql.AppendLine("AND UMA_Richieste_Allevamenti_UF.Tipo_Territorio = " & tipoTerritorio)
            End If
            If occupazione_Cod <> "" Then
                strSql.AppendLine("AND UMA_Richieste_Allevamenti_UF.Occupazione_Cod = " & occupazione_Cod)
            End If
            If destinazione_Cod <> "" Then
                strSql.AppendLine("AND UMA_Richieste_Allevamenti_UF.Destinazione_Cod = " & destinazione_Cod)
            End If
            If uso_Cod <> "" Then
                strSql.AppendLine("AND UMA_Richieste_Allevamenti_UF.Uso_Cod = " & uso_Cod)
            End If
            If qualita_Cod <> "" Then
                strSql.AppendLine("AND UMA_Richieste_Allevamenti_UF.Qualita_Cod = " & qualita_Cod)
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.Append("AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            strSql.AppendLine("AND UMA_Richieste_Allevamenti_UF.Piva_SuperUser = '" & objParametri.PivaSuperUser & "'")

            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.Append("AND   UMA_Richieste_Allevamenti_UF.Inviato >= 0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.Append("AND   UMA_Richieste_Allevamenti_UF.Inviato = -1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return dt
    End Function

    Public Function EstraiMacrousoUmaDaAgea(ByVal Occupazione_Cod As String,
                                            ByVal Destinazione_Cod As String,
                                            ByVal Uso_Cod As String,
                                            ByVal Qualita_Cod As String,
                                            ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Richieste_Allevamenti_UF_R.EstraiMacrousoUmaDaAgea()"

        Dim dt As DataTable
        Dim strSql As New StringBuilder()

        Try

            strSql.Length = 0

            strSql.AppendLine("SELECT DISTINCT Macrouso_Cod, Macrouso_UMA_Cod ")
            strSql.AppendLine("FROM Codifica_SpecieVegetali_Agea_2015_2020 ")
            strSql.AppendLine("WHERE 1=1 ")

            If Occupazione_Cod <> "" Then
                strSql.AppendLine("AND Occupazione_Cod = '" & Agro_SQL_SaveText(Occupazione_Cod) & "' ")
            End If
            If Destinazione_Cod <> "" Then
                strSql.AppendLine("AND Destinazione_Cod = '" & Agro_SQL_SaveText(Destinazione_Cod) & "' ")
            End If
            If Uso_Cod <> "" Then
                strSql.AppendLine("AND Uso_Cod = '" & Agro_SQL_SaveText(Uso_Cod) & "' ")
            End If
            If Qualita_Cod <> "" Then
                strSql.AppendLine("AND Qualita_Cod = '" & Agro_SQL_SaveText(Qualita_Cod) & "' ")
            End If

            strSql.AppendLine(" Order By Macrouso_Cod Desc, Macrouso_UMA_Cod Desc")

            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return dt

    End Function

End Class

Public Class UMA_Richieste_Allevamenti_UF_W
    Inherits DataProvider

    Public Function InserisciRiga(ByVal piva As String,
                                  ByVal Richiesta_Cod As Integer,
                                  ByVal programmazione_cod As Integer,
                                  ByVal tipoTerritorio As Integer,
                                  ByVal Occupazione_cod As String,
                                  ByVal Destinazione_cod As String,
                                  ByVal Uso_cod As String,
                                  ByVal Qualita_cod As String,
                                  ByVal Macrouso_UMA_cod As String,
                                  ByVal Superficie As Decimal,
                                  ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Richieste_Allevamenti_UF_W.InserisciRiga()"
        Dim res As Boolean
        Dim strSql As New StringBuilder()

        Try

            strSql.Length = 0

            strSql.AppendLine("INSERT INTO [dbo].[UMA_Richieste_Allevamenti_UF] ")
            strSql.AppendLine("([Piva_SuperUser] ,[Piva] ,[Richiesta_Cod] ,[Programmazione_Cod] ")
            strSql.AppendLine(",[Tipo_Territorio] ,[Occupazione_Cod] ,[Destinazione_Cod] ,[Uso_Cod] ")
            strSql.AppendLine(",[Qualita_Cod] ,[Macrouso_Uma_Cod] ,[Superf_Fascicolo] ,[Superf_Calcolo] ")
            strSql.AppendLine(",[Inviato] ,[DataInvio] ,[Data_Creazione] ,[Data_Modifica] ")
            strSql.AppendLine(",[Username_Creazione] ,[Username_Modifica] ,[Validita_Inizio] ,[Validita_Fine]) ")
            strSql.AppendLine("VALUES ")
            strSql.AppendLine("( '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            strSql.AppendLine(", '" & Agro_SQL_SaveText(piva) & "' ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Richiesta_Cod) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(programmazione_cod) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(tipoTerritorio) & " ")
            strSql.AppendLine(", '" & Agro_SQL_SaveText(Occupazione_cod) & "' ")
            strSql.AppendLine(", '" & Agro_SQL_SaveText(Destinazione_cod) & "' ")
            strSql.AppendLine(", '" & Agro_SQL_SaveText(Uso_cod) & "' ")
            strSql.AppendLine(", '" & Agro_SQL_SaveText(Qualita_cod) & "' ")
            strSql.AppendLine(", '" & Agro_SQL_SaveText(Macrouso_UMA_cod) & "' ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Superficie) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Superficie) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(0) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveDate(AGRODATAINIZIO) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveDate(DateTime.Now) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveDate(DateTime.Now) & " ")
            strSql.AppendLine(", '" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "' ")
            strSql.AppendLine(", '" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "' ")
            strSql.AppendLine(", " & Agro_SQL_SaveDate(AGRODATAINIZIO) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveDate(AGRODATAFINE) & " ) ")

            '--------------------------------------------------------------------------
            res = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return res

    End Function

    Public Function CancellaRiga(ByVal piva As String,
                                  ByVal Richiesta_Cod As Integer,
                                  ByVal programmazione_cod As Integer,
                                  ByVal tipoTerritorio As Integer,
                                  ByVal Occupazione_cod As String,
                                  ByVal Destinazione_cod As String,
                                  ByVal Uso_cod As String,
                                  ByVal Qualita_cod As String,
                                  ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Richieste_Allevamenti_UF_W.CancellaRiga()"
        Dim res As Boolean
        Dim strSql As New StringBuilder()

        Try

            strSql.Length = 0

            strSql.AppendLine(" DELETE FROM [dbo].[UMA_Richieste_Allevamenti_UF] ")
            strSql.AppendLine(" WHERE ")
            strSql.AppendLine(" Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' AND ")
            strSql.AppendLine(" Piva = '" & Agro_SQL_SaveText(piva) & "' AND ")
            strSql.AppendLine(" Richiesta_Cod = " & Agro_SQL_SaveNum(Richiesta_Cod) & " AND ")
            strSql.AppendLine(" Programmazione_Cod = " & Agro_SQL_SaveNum(programmazione_cod) & " AND ")
            strSql.AppendLine(" Tipo_Territorio = " & Agro_SQL_SaveNum(tipoTerritorio) & " AND ")
            strSql.AppendLine(" Occupazione_Cod = '" & Agro_SQL_SaveText(Occupazione_cod) & "' AND ")
            strSql.AppendLine(" Destinazione_cod = '" & Agro_SQL_SaveText(Destinazione_cod) & "' AND ")
            strSql.AppendLine(" Uso_cod = '" & Agro_SQL_SaveText(Uso_cod) & "' AND ")
            strSql.AppendLine(" Qualita_Cod = '" & Agro_SQL_SaveText(Qualita_cod) & "' ")

            '--------------------------------------------------------------------------
            res = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return res

    End Function

    Public Function ModificaRiga(ByVal piva As String,
                                  ByVal Richiesta_Cod As Integer,
                                  ByVal programmazione_cod As Integer,
                                  ByVal tipoTerritorio As Integer,
                                  ByVal Occupazione_cod As String,
                                  ByVal Destinazione_cod As String,
                                  ByVal Uso_cod As String,
                                  ByVal Qualita_cod As String,
                                  ByVal Superf_Calcolo As Double,
                                  ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Richieste_Allevamenti_UF_W.ModificaRiga()"
        Dim res As Boolean
        Dim strSql As New StringBuilder()

        Try

            strSql.Length = 0

            strSql.AppendLine(" UPDATE [dbo].[UMA_Richieste_Allevamenti_UF] ")
            strSql.AppendLine(" SET ")
            strSql.AppendLine(" Superf_Calcolo = " & Agro_SQL_SaveNum(Superf_Calcolo) & " ")
            strSql.AppendLine(" WHERE ")
            strSql.AppendLine(" Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' AND ")
            strSql.AppendLine(" Piva = '" & Agro_SQL_SaveText(piva) & "' AND ")
            strSql.AppendLine(" Richiesta_Cod = " & Agro_SQL_SaveNum(Richiesta_Cod) & " AND ")
            strSql.AppendLine(" Programmazione_Cod = " & Agro_SQL_SaveNum(programmazione_cod) & " AND ")
            strSql.AppendLine(" Tipo_Territorio = " & Agro_SQL_SaveNum(tipoTerritorio) & " AND ")
            strSql.AppendLine(" Occupazione_Cod = '" & Agro_SQL_SaveText(Occupazione_cod) & "' AND ")
            strSql.AppendLine(" Destinazione_cod = '" & Agro_SQL_SaveText(Destinazione_cod) & "' AND ")
            strSql.AppendLine(" Uso_cod = '" & Agro_SQL_SaveText(Uso_cod) & "' AND ")
            strSql.AppendLine(" Qualita_Cod = '" & Agro_SQL_SaveText(Qualita_cod) & "' ")

            '--------------------------------------------------------------------------
            res = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return res

    End Function

End Class
