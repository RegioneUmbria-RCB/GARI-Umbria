Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Codifica_SpecieVegetali_Clienti_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Codice_Cliente As Integer,
                                ByVal Veg_Cod_Cliente As String,
                                ByVal Cul_Cod_Cliente As String,
                                ByVal Veg_Cod As Integer,
                                ByVal Cul_Cod As Integer,
                                ByVal Id_Cod As Integer,
                                ByVal Data As Date,
                                    ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                    Optional ByVal grfi_cod As Integer = 0,
                                    Optional ByVal Grva_cod As Integer = 0
                                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Clienti_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Select Case xSelezioneVariabile


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT  Codice_Cliente, Veg_Cod_Cliente, Cul_Cod_Cliente, ISNULL(Veg_Des_Cliente, '') AS Veg_Des_Cliente, ISNULL(Cul_Des_Cliente, '') AS Cul_Des_Cliente, ")
                    StrSQL.Append(" Veg_cod,  Cul_cod, Grfi_cod, Grva_cod, Metodo_Produzione_cod, Reg_cod, Id_cod, ISNULL(Grsp_Cod, 0) AS Grsp_Cod  ")
                    StrSQL.Append(" FROM    Codifica_SpecieVegetali_Clienti ")
                    StrSQL.Append(" WHERE   Validita_Inizio <=" & Agro_SQL_SaveDate(Data) & " ")
                    StrSQL.Append(" AND     Validita_Fine >=" & Agro_SQL_SaveDate(Data) & " ")
                    StrSQL.Append(" AND     Codice_Cliente = " & Agro_SQL_SaveNum(Codice_Cliente) & " ")

                    If Veg_Cod_Cliente <> "" Then
                        StrSQL.Append(" AND Veg_Cod_Cliente = '" & Agro_SQL_SaveText(Veg_Cod_Cliente) & "' ")
                    End If
                    If Cul_Cod_Cliente <> "" Then
                        StrSQL.Append(" AND Cul_Cod_Cliente = '" & Agro_SQL_SaveText(Cul_Cod_Cliente) & "' ")
                    End If

                    If Veg_Cod <> 0 Then
                        StrSQL.Append(" AND Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
                    End If
                    If Cul_Cod <> 0 Then
                        StrSQL.Append(" AND Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & " ")
                    End If
                    If Id_Cod <> 0 Then
                        StrSQL.Append(" AND Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & " ")
                    End If

                    If grfi_cod <> 0 Then
                        StrSQL.Append(" AND grfi_cod = " & Agro_SQL_SaveNum(grfi_cod) & " ")
                    End If

                    If Grva_cod <> 0 Then
                        StrSQL.Append(" AND Grva_cod = " & Agro_SQL_SaveNum(Grva_cod) & " ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Veg_Des_Cliente, Cul_Des_Cliente ")
                    End If


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT  * ")
                    StrSQL.Append(" FROM        Codifica_SpecieVegetali_Clienti ")
                    StrSQL.Append(" WHERE   Validita_Inizio <=" & Agro_SQL_SaveDate(Data) & " ")
                    StrSQL.Append(" AND     Validita_Fine >=" & Agro_SQL_SaveDate(Data) & " ")
                    StrSQL.Append(" AND     Codice_Cliente = " & Agro_SQL_SaveNum(Codice_Cliente) & " ")

                    If Veg_Cod_Cliente <> "" Then
                        StrSQL.Append(" AND Veg_Cod_Cliente = '" & Agro_SQL_SaveText(Veg_Cod_Cliente) & "' ")
                    End If
                    If Cul_Cod_Cliente <> "" Then
                        StrSQL.Append(" AND Cul_Cod_Cliente = '" & Agro_SQL_SaveText(Cul_Cod_Cliente) & "' ")
                    End If

                    If Veg_Cod <> 0 Then
                        StrSQL.Append(" AND Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
                    End If
                    If Cul_Cod <> 0 Then
                        StrSQL.Append(" AND Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & " ")
                    End If
                    If Id_Cod <> 0 Then
                        StrSQL.Append(" AND Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & " ")
                    End If

                    If grfi_cod <> 0 Then
                        StrSQL.Append(" AND grfi_cod = " & Agro_SQL_SaveNum(grfi_cod) & " ")
                    End If

                    If Grva_cod <> 0 Then
                        StrSQL.Append(" AND Grva_cod = " & Agro_SQL_SaveNum(Grva_cod) & " ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Veg_Des_Cliente, Cul_Des_Cliente ")
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT  Veg_Cod_Cliente, Cul_Cod_Cliente, ISNULL(Veg_Des_Cliente, '') AS Veg_Des_Cliente, ISNULL(Cul_Des_Cliente, '') AS Cul_Des_Cliente, ")
                    StrSQL.Append(" Codifica_SpecieVegetali_Clienti.Veg_cod,  Codifica_SpecieVegetali_Clienti.Cul_cod, Codifica_SpecieVegetali_Clienti.Grfi_cod, Codifica_SpecieVegetali_Clienti.Grva_cod, Codifica_SpecieVegetali_Clienti.Metodo_Produzione_cod, Codifica_SpecieVegetali_Clienti.Reg_cod, Codifica_SpecieVegetali_Clienti.Id_cod,  ISNULL(Codifica_SpecieVegetali_Clienti.Grsp_Cod, 0) AS Grsp_Cod,")
                    StrSQL.Append(" ISNULL(SpecieVegetali.Veg_Des,'') AS Veg_Des, ISNULL(Cultivar.Cul_Des,'') AS Cul_Des, ISNULL(GruppoVarietale.Grva_Des,'') AS Grva_Des, ISNULL(GruppoFinalita.Grfi_Des,'') AS Grfi_Des, ISNULL(Regolamenti.Reg_Des,'') AS Reg_Des, ISNULL(Codici_Anagrafe.descrizione,'') AS Id_Des ")

                    StrSQL.Append(" FROM         Codifica_SpecieVegetali_Clienti LEFT OUTER JOIN ")
                    StrSQL.Append(" Codici_Anagrafe ON Codifica_SpecieVegetali_Clienti.Id_cod = Codici_Anagrafe.codice LEFT OUTER JOIN ")
                    StrSQL.Append(" Regolamenti ON Codifica_SpecieVegetali_Clienti.Reg_cod = Regolamenti.Reg_Cod LEFT OUTER JOIN ")
                    StrSQL.Append(" GruppoFinalita ON Codifica_SpecieVegetali_Clienti.Grfi_cod = GruppoFinalita.Grfi_Cod LEFT OUTER JOIN ")
                    StrSQL.Append(" GruppoVarietale ON Codifica_SpecieVegetali_Clienti.Grva_cod = GruppoVarietale.Grva_Cod LEFT OUTER JOIN ")
                    StrSQL.Append(" Cultivar ON Codifica_SpecieVegetali_Clienti.Cul_cod = Cultivar.Cul_Cod LEFT OUTER JOIN ")
                    StrSQL.Append("  SpecieVegetali ON Codifica_SpecieVegetali_Clienti.Veg_cod = SpecieVegetali.Veg_Cod ")

                    StrSQL.Append(" WHERE   Codifica_SpecieVegetali_Clienti.Validita_Inizio <=" & Agro_SQL_SaveDate(Data) & " ")
                    StrSQL.Append(" AND     Codifica_SpecieVegetali_Clienti.Validita_Fine >=" & Agro_SQL_SaveDate(Data) & " ")
                    StrSQL.Append(" AND     Codifica_SpecieVegetali_Clienti.Codice_Cliente = " & Agro_SQL_SaveNum(Codice_Cliente) & " ")


                    If Veg_Cod_Cliente <> "" Then
                        StrSQL.Append(" AND Codifica_SpecieVegetali_Clienti.Veg_Cod_Cliente = '" & Agro_SQL_SaveText(Veg_Cod_Cliente) & "' ")
                    End If
                    If Cul_Cod_Cliente <> "" Then
                        StrSQL.Append(" AND Codifica_SpecieVegetali_Clienti.Cul_Cod_Cliente = '" & Agro_SQL_SaveText(Cul_Cod_Cliente) & "' ")
                    End If

                    If Veg_Cod <> 0 Then
                        StrSQL.Append(" AND Codifica_SpecieVegetali_Clienti.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
                    End If
                    If Cul_Cod <> 0 Then
                        StrSQL.Append(" AND Codifica_SpecieVegetali_Clienti.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & " ")
                    End If
                    If Id_Cod <> 0 Then
                        StrSQL.Append(" AND Codifica_SpecieVegetali_Clienti.Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & " ")
                    End If


                    If grfi_cod <> 0 Then
                        StrSQL.Append(" AND grfi_cod = " & Agro_SQL_SaveNum(grfi_cod) & " ")
                    End If

                    If Grva_cod <> 0 Then
                        StrSQL.Append(" AND Grva_cod = " & Agro_SQL_SaveNum(Grva_cod) & " ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Codifica_SpecieVegetali_Clienti.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Codifica_SpecieVegetali_Clienti.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Veg_Des_Cliente, Cul_Des_Cliente ")
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT  * ")
                    StrSQL.Append(" FROM         Codifica_SpecieVegetali_Clienti LEFT OUTER JOIN ")
                    StrSQL.Append(" Codici_Anagrafe ON Codifica_SpecieVegetali_Clienti.Id_cod = Codici_Anagrafe.codice LEFT OUTER JOIN ")
                    StrSQL.Append(" Regolamenti ON Codifica_SpecieVegetali_Clienti.Reg_cod = Regolamenti.Reg_Cod LEFT OUTER JOIN ")
                    StrSQL.Append(" GruppoFinalita ON Codifica_SpecieVegetali_Clienti.Grfi_cod = GruppoFinalita.Grfi_Cod LEFT OUTER JOIN ")
                    StrSQL.Append(" GruppoVarietale ON Codifica_SpecieVegetali_Clienti.Grva_cod = GruppoVarietale.Grva_Cod LEFT OUTER JOIN ")
                    StrSQL.Append(" Cultivar ON Codifica_SpecieVegetali_Clienti.Cul_cod = Cultivar.Cul_Cod LEFT OUTER JOIN ")
                    StrSQL.Append(" SpecieVegetali ON Codifica_SpecieVegetali_Clienti.Veg_cod = SpecieVegetali.Veg_Cod ")

                    StrSQL.Append(" WHERE   Codifica_SpecieVegetali_Clienti.Validita_Inizio <=" & Agro_SQL_SaveDate(Data) & " ")
                    StrSQL.Append(" AND     Codifica_SpecieVegetali_Clienti.Validita_Fine >=" & Agro_SQL_SaveDate(Data) & " ")
                    StrSQL.Append(" AND     Codifica_SpecieVegetali_Clienti.Codice_Cliente = " & Agro_SQL_SaveNum(Codice_Cliente) & " ")

                    If Veg_Cod_Cliente <> "" Then
                        StrSQL.Append(" AND Codifica_SpecieVegetali_Clienti.Veg_Cod_Cliente = '" & Agro_SQL_SaveText(Veg_Cod_Cliente) & "' ")
                    End If
                    If Cul_Cod_Cliente <> "" Then
                        StrSQL.Append(" AND Codifica_SpecieVegetali_Clienti.Cul_Cod_Cliente = '" & Agro_SQL_SaveText(Cul_Cod_Cliente) & "' ")
                    End If

                    If Veg_Cod <> 0 Then
                        StrSQL.Append(" AND Codifica_SpecieVegetali_Clienti.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
                    End If
                    If Cul_Cod <> 0 Then
                        StrSQL.Append(" AND Codifica_SpecieVegetali_Clienti.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & " ")
                    End If
                    If Id_Cod <> 0 Then
                        StrSQL.Append(" AND Codifica_SpecieVegetali_Clienti.Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & " ")
                    End If


                    If grfi_cod <> 0 Then
                        StrSQL.Append(" AND grfi_cod = " & Agro_SQL_SaveNum(grfi_cod) & " ")
                    End If

                    If Grva_cod <> 0 Then
                        StrSQL.Append(" AND Grva_cod = " & Agro_SQL_SaveNum(Grva_cod) & " ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Codifica_SpecieVegetali_Clienti.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Codifica_SpecieVegetali_Clienti.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Veg_Des_Cliente, Cul_Des_Cliente ")
                    End If

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

    Public Function LeggixCliVegCul(ByVal Codice_Cliente As Integer,
                                    ByVal Veg_Cod As Integer,
                                    ByVal Cul_Cod As Integer,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Clienti_R.LeggixCliVegCul()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine("SELECT *")
            StrSQL.AppendLine("FROM Codifica_SpecieVegetali_Clienti")
            StrSQL.AppendLine("WHERE 1 = 1")
            'StrSQL.AppendLine("AND Validita_Inizio <=" & Agro_SQL_SaveDate(Data))
            'StrSQL.AppendLine("AND Validita_Fine >=" & Agro_SQL_SaveDate(Data))
            StrSQL.AppendLine("AND Codice_Cliente = " & Agro_SQL_SaveNum(Codice_Cliente))
            If Veg_Cod > 0 Then
                StrSQL.AppendLine("AND Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod))
            End If
            If Cul_Cod > 0 Then
                StrSQL.AppendLine("AND Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine("AND Inviato >= 0")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine("AND Inviato = -1")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            StrSQL.Append(" ORDER BY Veg_Des_Cliente, Cul_Des_Cliente ")

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

    Public Function LeggixCliente(ByVal Codice_Cliente As Integer, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Clienti_R.LeggixCli()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine("SELECT *")
            StrSQL.AppendLine("FROM Codifica_SpecieVegetali_Clienti")
            StrSQL.AppendLine("WHERE Codice_Cliente = " & Agro_SQL_SaveNum(Codice_Cliente))

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine("AND Inviato >= 0")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine("AND Inviato = -1")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            StrSQL.Append(" ORDER BY Veg_Cod, Cul_Cod ")

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
