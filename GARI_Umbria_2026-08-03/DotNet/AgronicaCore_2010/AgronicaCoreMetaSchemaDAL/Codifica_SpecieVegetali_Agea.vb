Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider

<CachedDataProviderAttribute("Codifica_SpecieVegetali_Agea_R")>
Public Class Codifica_SpecieVegetali_Agea_R
    Inherits AgronicaCoreDataProvider.CachedDataProvider
    Dim hashRisultati As Hashtable
    Dim hashRisultati2 As Hashtable
    Dim hashGruCod As Hashtable
    Dim hashSpecieEVarietaDaMacrouso As Hashtable
    Dim objSpecieVegetali As AgronicaCoreMetaSchemaDAL.SpecieVegetali_R

    Public Sub New()
        hashRisultati = New Hashtable
        hashRisultati2 = New Hashtable
        hashGruCod = New Hashtable
        hashSpecieEVarietaDaMacrouso = New Hashtable
        objSpecieVegetali = New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
    End Sub


    '##############################################################################################
    <Cacheable(True)>
    Public Function Leggi(
                                ByVal Veg_Cod_Agea As String,
                                ByVal Cul_Cod_Agea As String,
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

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Agea_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Select Case xSelezioneVariabile


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT  Veg_Cod_Agea, Cul_Cod_Agea, ISNULL(Veg_Des_Agea, '') AS Veg_Des_Agea, ISNULL(Cul_Des_Agea, '') AS Cul_Des_Agea, ")
                    StrSQL.Append(" Veg_cod,  Cul_cod, Grfi_cod, Grva_cod, Metodo_Produzione_cod, Reg_cod, Id_cod, ISNULL(Grsp_Cod, 0) AS Grsp_Cod  ")
                    StrSQL.Append(" FROM    Codifica_SpecieVegetali_Agea_2015_2020 ")
                    StrSQL.Append(" WHERE   1=1 ")

                    If Veg_Cod_Agea <> "" Then
                        StrSQL.Append(" AND Veg_Cod_Agea = '" & Agro_SQL_SaveText(Veg_Cod_Agea) & "' ")
                    End If
                    If Cul_Cod_Agea <> "" Then
                        StrSQL.Append(" AND Cul_Cod_Agea = '" & Agro_SQL_SaveText(Cul_Cod_Agea) & "' ")
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
                        StrSQL.Append(" ORDER BY Veg_Des_Agea, Cul_Des_Agea ")
                    End If


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT  * ")
                    StrSQL.Append(" FROM        Codifica_SpecieVegetali_Agea ")
                    StrSQL.Append(" WHERE   1=1 ")

                    If Veg_Cod_Agea <> "" Then
                        StrSQL.Append(" AND Veg_Cod_Agea = '" & Agro_SQL_SaveText(Veg_Cod_Agea) & "' ")
                    End If
                    If Cul_Cod_Agea <> "" Then
                        StrSQL.Append(" AND Cul_Cod_Agea = '" & Agro_SQL_SaveText(Cul_Cod_Agea) & "' ")
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
                        StrSQL.Append(" ORDER BY Veg_Des_Agea, Cul_Des_Agea ")
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT  Veg_Cod_Agea, Cul_Cod_Agea, ISNULL(Veg_Des_Agea, '') AS Veg_Des_Agea, ISNULL(Cul_Des_Agea, '') AS Cul_Des_Agea, ")
                    StrSQL.Append(" Codifica_SpecieVegetali_Agea.Veg_cod,  Codifica_SpecieVegetali_Agea.Cul_cod, Codifica_SpecieVegetali_Agea.Grfi_cod, Codifica_SpecieVegetali_Agea.Grva_cod, Codifica_SpecieVegetali_Agea.Metodo_Produzione_cod, Codifica_SpecieVegetali_Agea.Reg_cod, Codifica_SpecieVegetali_Agea.Id_cod,  ISNULL(Codifica_SpecieVegetali_Agea.Grsp_Cod, 0) AS Grsp_Cod,")
                    StrSQL.Append(" ISNULL(SpecieVegetali.Veg_Des,'') AS Veg_Des, ISNULL(Cultivar.Cul_Des,'') AS Cul_Des, ISNULL(GruppoVarietale.Grva_Des,'') AS Grva_Des, ISNULL(GruppoFinalita.Grfi_Des,'') AS Grfi_Des, ISNULL(Regolamenti.Reg_Des,'') AS Reg_Des, ISNULL(Codici_Anagrafe.descrizione,'') AS Id_Des ")

                    StrSQL.Append(" FROM         Codifica_SpecieVegetali_Agea LEFT OUTER JOIN ")
                    StrSQL.Append(" Codici_Anagrafe ON Codifica_SpecieVegetali_Agea.Id_cod = Codici_Anagrafe.codice LEFT OUTER JOIN ")
                    StrSQL.Append(" Regolamenti ON Codifica_SpecieVegetali_Agea.Reg_cod = Regolamenti.Reg_Cod LEFT OUTER JOIN ")
                    StrSQL.Append(" GruppoFinalita ON Codifica_SpecieVegetali_Agea.Grfi_cod = GruppoFinalita.Grfi_Cod LEFT OUTER JOIN ")
                    StrSQL.Append(" GruppoVarietale ON Codifica_SpecieVegetali_Agea.Grva_cod = GruppoVarietale.Grva_Cod LEFT OUTER JOIN ")
                    StrSQL.Append(" Cultivar ON Codifica_SpecieVegetali_Agea.Cul_cod = Cultivar.Cul_Cod LEFT OUTER JOIN ")
                    StrSQL.Append("  SpecieVegetali ON Codifica_SpecieVegetali_Agea.Veg_cod = SpecieVegetali.Veg_Cod ")

                    StrSQL.Append(" WHERE   1=1 ")

                    If Veg_Cod_Agea <> "" Then
                        StrSQL.Append(" AND Codifica_SpecieVegetali_Agea.Veg_Cod_Agea = '" & Agro_SQL_SaveText(Veg_Cod_Agea) & "' ")
                    End If
                    If Cul_Cod_Agea <> "" Then
                        StrSQL.Append(" AND Codifica_SpecieVegetali_Agea.Cul_Cod_Agea = '" & Agro_SQL_SaveText(Cul_Cod_Agea) & "' ")
                    End If

                    If Veg_Cod <> 0 Then
                        StrSQL.Append(" AND Codifica_SpecieVegetali_Agea.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
                    End If
                    If Cul_Cod <> 0 Then
                        StrSQL.Append(" AND Codifica_SpecieVegetali_Agea.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & " ")
                    End If
                    If Id_Cod <> 0 Then
                        StrSQL.Append(" AND Codifica_SpecieVegetali_Agea.Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & " ")
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
                            StrSQL.Append(" AND   Codifica_SpecieVegetali_Agea.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Codifica_SpecieVegetali_Agea.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Veg_Des_Agea, Cul_Des_Agea ")
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT  * ")
                    StrSQL.Append(" FROM         Codifica_SpecieVegetali_Agea LEFT OUTER JOIN ")
                    StrSQL.Append(" Codici_Anagrafe ON Codifica_SpecieVegetali_Agea.Id_cod = Codici_Anagrafe.codice LEFT OUTER JOIN ")
                    StrSQL.Append(" Regolamenti ON Codifica_SpecieVegetali_Agea.Reg_cod = Regolamenti.Reg_Cod LEFT OUTER JOIN ")
                    StrSQL.Append(" GruppoFinalita ON Codifica_SpecieVegetali_Agea.Grfi_cod = GruppoFinalita.Grfi_Cod LEFT OUTER JOIN ")
                    StrSQL.Append(" GruppoVarietale ON Codifica_SpecieVegetali_Agea.Grva_cod = GruppoVarietale.Grva_Cod LEFT OUTER JOIN ")
                    StrSQL.Append(" Cultivar ON Codifica_SpecieVegetali_Agea.Cul_cod = Cultivar.Cul_Cod LEFT OUTER JOIN ")
                    StrSQL.Append("  SpecieVegetali ON Codifica_SpecieVegetali_Agea.Veg_cod = SpecieVegetali.Veg_Cod ")

                    StrSQL.Append(" WHERE   1=1 ")

                    If Veg_Cod_Agea <> "" Then
                        StrSQL.Append(" AND Codifica_SpecieVegetali_Agea.Veg_Cod_Agea = '" & Agro_SQL_SaveText(Veg_Cod_Agea) & "' ")
                    End If
                    If Cul_Cod_Agea <> "" Then
                        StrSQL.Append(" AND Codifica_SpecieVegetali_Agea.Cul_Cod_Agea = '" & Agro_SQL_SaveText(Cul_Cod_Agea) & "' ")
                    End If

                    If Veg_Cod <> 0 Then
                        StrSQL.Append(" AND Codifica_SpecieVegetali_Agea.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
                    End If
                    If Cul_Cod <> 0 Then
                        StrSQL.Append(" AND Codifica_SpecieVegetali_Agea.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & " ")
                    End If
                    If Id_Cod <> 0 Then
                        StrSQL.Append(" AND Codifica_SpecieVegetali_Agea.Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & " ")
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
                            StrSQL.Append(" AND   Codifica_SpecieVegetali_Agea.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Codifica_SpecieVegetali_Agea.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Veg_Des_Agea, Cul_Des_Agea ")
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

    '##############################################################################################
    Public Function SpecieDistinteAgea(ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Agea_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            StrSQL.Length = 0
            StrSQL.Append(" SELECT DISTINCT  Veg_Cod_Agea, ISNULL(Veg_Des_Agea, '') AS Veg_Des_Agea  ")
            StrSQL.Append(" FROM    Codifica_SpecieVegetali_Agea_2015_2020 ")
            StrSQL.Append(" WHERE   Validita_Inizio <=" & Agro_SQL_SaveDate(CDate(objParametri.FinestraTemporaleFine)) & " ")
            StrSQL.Append(" AND     Validita_Fine >=" & Agro_SQL_SaveDate(CDate(objParametri.FinestraTemporaleInizio)) & " ")


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
                StrSQL.Append(" ORDER BY Veg_Des_Agea ")
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


    '##############################################################################################
    Public Function VegDesAgea_from_VegCodAgea_Old(
                                     ByVal Veg_Cod_Agea As String,
                                     ByVal Cul_Cod_Agea As String,
                                     ByRef Cul_Des_Agea As String,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As String

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Agea_R.VegDesAgea_from_VegCodAgea()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim Veg_Des_Agea As String = ""

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT  Veg_Des_Agea, Cul_Des_Agea ")
            StrSQL.Append(" FROM    Codifica_SpecieVegetali_Agea ")

            StrSQL.Append(" WHERE   Validita_Inizio <=" & Agro_SQL_SaveDate(CDate(objParametri.FinestraTemporaleFine)) & " ")
            StrSQL.Append(" AND     Validita_Fine >=" & Agro_SQL_SaveDate(CDate(objParametri.FinestraTemporaleInizio)) & " ")
            StrSQL.Append(" AND     Veg_Cod_Agea = '" & Agro_SQL_SaveText(Veg_Cod_Agea) & "' ")

            If Cul_Cod_Agea <> "" Then
                StrSQL.Append(" AND Cul_Cod_Agea = '" & Agro_SQL_SaveText(Cul_Cod_Agea) & "' ")
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
                StrSQL.Append(" ORDER BY Veg_Des_Agea, Cul_Des_Agea ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then
                Veg_Des_Agea = DT.Rows(0).Item("Veg_Des_Agea")
                'Cul_Des_Agea = DT.Rows(0).Item("Cul_Des_Agea")
                If Not IsDBNull(DT.Rows(0).Item("Cul_Des_Agea")) Then
                    Cul_Des_Agea = DT.Rows(0).Item("Cul_Des_Agea")
                Else
                    Cul_Des_Agea = ""
                End If

            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Veg_Des_Agea

    End Function


    Public Function Specie_e_Varieta_Gias_Da_Agea_Old(ByRef LogCodificheMancantiSpecie As String,
                                                  ByRef LogCodificheMancantiVarieta As String,
                                                    ByVal Veg_Cod_Agea As String,
                                                    ByVal Cul_Cod_Agea As String,
                                                    ByRef Veg_Cod As Integer,
                                                    ByRef Cul_Cod As Integer,
                                                    ByRef Grfi_Cod As Integer,
                                                    ByRef Grva_Cod As Integer,
                                                    ByRef Id_Cod As Integer,
                                                    ByRef Specie_Des_Agea As String,
                                                    ByRef Varieta_Des_Agea As String,
                                                    ByVal Specie_Des_AgeaInput As String,
                                                    ByVal Varieta_Des_AgeaInput As String,
                                                    ByVal Data As Date,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                        ) As Boolean

        Dim NomeRoutine As String = "Specie_e_Varieta_Gias_Da_Agea"

        Dim DtCodifica As DataTable
        Dim DrVar() As DataRow
        Dim Esito As Boolean = False
        Dim LogCodificheMancantiVarieta_Excel As String = ""
        Dim LogCodificheMancantiSpecie_Excel As String = ""
        Veg_Cod = 0
        Cul_Cod = 0
        Grfi_Cod = 0
        Id_Cod = 0
        Grva_Cod = 0
        Specie_Des_Agea = ""
        Varieta_Des_Agea = ""

        'If Veg_Cod_Agea = "154" Then
        '    Dim i = 0
        'End If


        'LEGGO TUTTI I RECORD DELLA SPECIE  
        'filtro poi se è mappata anche la varietà
        'altrimenti metto 'altre'....
        DtCodifica = Leggi(Veg_Cod_Agea,
                            "",
                            0, 0, 0,
                            Data,
                            AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                            "", "",
                            objParametri)


        If Not IsNothing(DtCodifica) AndAlso DtCodifica.Rows.Count > 0 Then

            DrVar = DtCodifica.Select("Cul_Cod_Agea='" & Cul_Cod_Agea.ToString & "'")

            If Not DrVar Is Nothing AndAlso DrVar.Length > 0 Then

                Veg_Cod = DrVar(0).Item("Veg_Cod")
                Cul_Cod = DrVar(0).Item("Cul_Cod")
                Grfi_Cod = DrVar(0).Item("Grfi_Cod")
                Id_Cod = DrVar(0).Item("Id_Cod")
                Grva_Cod = DrVar(0).Item("Grva_Cod")
                Specie_Des_Agea = DrVar(0).Item("Veg_Des_Agea")

                If Not IsDBNull(DrVar(0).Item("Cul_Des_Agea")) Then
                    Varieta_Des_Agea = DrVar(0).Item("Cul_Des_Agea")
                Else
                    Varieta_Des_Agea = ""
                End If
                Esito = True

            Else

                If Cul_Cod_Agea <> "" Then
                    LogCodificheMancantiVarieta &= "Specie: " & Specie_Des_AgeaInput & ", Veg_Cod_Agea=" & CStr(Veg_Cod_Agea) &
                        " - la varietà " & Varieta_Des_AgeaInput & " con Cul_Cod_Agea=" & CStr(Cul_Cod_Agea) & " non è mappata -> è stata selezionata la varietà Altre." & vbCrLf & vbCrLf
                    LogCodificheMancantiVarieta_Excel &= vbTab & vbTab & Specie_Des_AgeaInput & vbTab & CStr(Veg_Cod_Agea) & vbTab & Varieta_Des_AgeaInput & vbTab & CStr(Cul_Cod_Agea)
                    Dim customLOGParamsCsv As New CustomLOGParams With {
                        .LogDescrizioneUtente = objParametri.UtenteUsername,
                        .LogDirectory = objParametri.LogDirectory,
                        .LogFileName = "CodificaSpecieVegetaliAgea_VARIETA_mancanti_Excel.csv"
                    }
                    Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiVarieta_Excel, CustomLOGParams:=customLOGParamsCsv)

                    Dim customLOGParamsTxt As New CustomLOGParams With {
                        .LogDescrizioneUtente = objParametri.UtenteUsername,
                        .LogDirectory = objParametri.LogDirectory,
                        .LogFileName = "CodificaSpecieVegetaliAgea_VARIETA_mancanti.txt"
                    }
                    Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiVarieta, CustomLOGParams:=customLOGParamsTxt)


                End If

                Veg_Cod = DtCodifica.Rows(0).Item("Veg_Cod")

                If Veg_Cod = 0 Then
                    Cul_Cod = 0
                Else
                    Dim objCultivar As New AgronicaCoreMetaSchemaDAL.Cultivar_R
                    Cul_Cod = objCultivar.VarietaAltre(Veg_Cod,
                                                       objParametri)
                End If

                Grfi_Cod = DtCodifica.Rows(0).Item("Grfi_Cod")
                Id_Cod = DtCodifica.Rows(0).Item("Id_Cod")
                Grva_Cod = DtCodifica.Rows(0).Item("Grva_Cod")
                Specie_Des_Agea = DtCodifica.Rows(0).Item("Veg_Des_Agea")

                If Not IsDBNull(DtCodifica.Rows(0).Item("Cul_Des_Agea")) Then
                    Varieta_Des_Agea = DtCodifica.Rows(0).Item("Cul_Des_Agea")
                Else
                    Varieta_Des_Agea = ""
                End If

                Esito = True

            End If

        Else
            Veg_Cod = 0
            Cul_Cod = 0
            Grfi_Cod = 0
            Grva_Cod = 0
            Id_Cod = enum_CodiciAnagrafe.NessunaMappaturaConGias
            Specie_Des_Agea = "Nessuna mappatura con Gias."
            Varieta_Des_Agea = "Nessuna mappatura con Gias."
            'Specie_Des_Agea = "Nessuna mappatura con Gias (cod." & Veg_Cod_Agea & ")."
            'Varieta_Des_Agea = "Nessuna mappatura con Gias (cod." & Cul_Cod_Agea & ")."
            LogCodificheMancantiSpecie &= "Nessun record presente per il Veg_Cod_Agea=" & CStr(Veg_Cod_Agea) & "  " & Specie_Des_AgeaInput & vbCrLf & vbCrLf
            LogCodificheMancantiSpecie_Excel &= vbTab & vbTab & Specie_Des_AgeaInput & vbTab & CStr(Veg_Cod_Agea)

            Dim customLOGParamsTxt As New CustomLOGParams With {
                .LogDescrizioneUtente = objParametri.UtenteUsername,
                .LogDirectory = objParametri.LogDirectory,
                .LogFileName = "CodificaSpecieVegetaliAgea_SPECIE_mancanti.txt"
            }
            Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiSpecie, CustomLOGParams:=customLOGParamsTxt)

            Dim customLOGParamsCsv As New CustomLOGParams With {
                .LogDescrizioneUtente = objParametri.UtenteUsername,
                .LogDirectory = objParametri.LogDirectory,
                .LogFileName = "CodificaSpecieVegetaliAgea_SPECIE_mancanti_Excel.csv"
            }
            Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiSpecie_Excel, CustomLOGParams:=customLOGParamsCsv)




        End If

        Return Esito

    End Function

    Public Function Specie_e_Varieta_Gias_Da_Agea(ByRef LogCodificheMancantiSpecie As String,
                                                  ByRef LogCodificheMancantiVarieta As String,
                                                    ByVal Veg_Cod_Agea As String,
                                                    ByVal Cul_Cod_Agea As String,
                                                    ByRef Veg_Cod As Integer,
                                                    ByRef Cul_Cod As Integer,
                                                    ByRef Grfi_Cod As Integer,
                                                    ByRef Grva_Cod As Integer,
                                                    ByRef Id_Cod As Integer,
                                                    ByRef Specie_Des_Agea As String,
                                                    ByRef Varieta_Des_Agea As String,
                                                    ByVal Specie_Des_AgeaInput As String,
                                                    ByVal Varieta_Des_AgeaInput As String,
                                                    ByVal Data As Date,
                                                    ByRef Uso_Cod As String,
                                                    ByRef Occupazione_Cod As String,
                                                    ByRef Destinazione_Cod As String,
                                                    ByRef Qualita_Cod As String,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                    Optional ByRef Gru_Cod As Integer = 0
                                                        ) As Boolean

        Dim NomeRoutine As String = "Specie_e_Varieta_Gias_Da_Agea"

        Dim DtCodifica As DataTable
        Dim DrVar() As DataRow
        Dim Esito As Boolean = False
        Dim LogCodificheMancantiVarieta_Excel As String = ""
        Dim LogCodificheMancantiSpecie_Excel As String = ""
        Veg_Cod = 0
        Cul_Cod = 0
        Grfi_Cod = 0
        Id_Cod = 0
        Grva_Cod = 0
        Specie_Des_Agea = ""
        Varieta_Des_Agea = ""
        Gru_Cod = 0
        'If Veg_Cod_Agea = "154" Then
        '    Dim i = 0
        'End If


        'LEGGO TUTTI I RECORD DELLA SPECIE  
        'filtro poi se è mappata anche la varietà
        'altrimenti metto 'altre'....
        Dim objAgeaR As New AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Agea_2015_2020_R
        'DtCodifica = Leggi(Veg_Cod_Agea,
        '                    "",
        '                    0, 0, 0,
        '                    Data,
        '                    AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
        '                    "", "",
        '                    objParametri)
        Dim key = NomeRoutine & "_" & Veg_Cod_Agea & "_" & Occupazione_Cod
        If Not hashRisultati.Contains(key) Then
            DtCodifica = objAgeaR.leggi(objParametri, Veg_Cod_Agea, "", "", "", Occupazione_Cod, "", "", 0, 0, 0, 0, 0, 0, 0, 0)
            hashRisultati.Add(key, DtCodifica)
        Else
            DtCodifica = hashRisultati(key)
        End If


        If Not IsNothing(DtCodifica) AndAlso DtCodifica.Rows.Count > 0 Then

            DrVar = DtCodifica.Select("Cul_Cod_Agea='" & Cul_Cod_Agea.ToString & "'")

            If Not DrVar Is Nothing AndAlso DrVar.Length > 0 Then

                Veg_Cod = DrVar(0).Item("Veg_Cod")
                Cul_Cod = DrVar(0).Item("Cul_Cod")
                Grfi_Cod = DrVar(0).Item("Grfi_Cod")
                Id_Cod = DrVar(0).Item("Id_Cod")
                Grva_Cod = DrVar(0).Item("Grva_Cod")
                Specie_Des_Agea = DrVar(0).Item("Veg_Des_Agea")

                Uso_Cod = DrVar(0).Item("Uso_Cod")
                Occupazione_Cod = DrVar(0).Item("Occupazione_Cod")
                Destinazione_Cod = DrVar(0).Item("Destinazione_Cod")
                Qualita_Cod = DrVar(0).Item("Qualita_Cod")

                If Not hashGruCod.Contains(Veg_Cod) Then
                    Gru_Cod = objSpecieVegetali.Leggi(Veg_Cod, 0, "", "", AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri).Rows(0).Item("Gru_Cod")
                    hashGruCod.Add(Veg_Cod, Gru_Cod)
                Else
                    Gru_Cod = hashGruCod(Veg_Cod)
                End If

                If Not IsDBNull(DrVar(0).Item("Cul_Des_Agea")) Then
                    Varieta_Des_Agea = DrVar(0).Item("Cul_Des_Agea")

                Else
                    Varieta_Des_Agea = ""

                End If
                Esito = True


            Else

                Occupazione_Cod = DtCodifica.Rows(0).Item("Occupazione_Cod")

                Dim key2 = Cul_Cod_Agea & "_" & Occupazione_Cod

                Dim DtCodifica2 As New DataTable
                If Not hashRisultati2.Contains(key2) Then
                    DtCodifica2 = objAgeaR.leggi(objParametri, "", Cul_Cod_Agea, "", "", Occupazione_Cod, "", "", 0, 0, 0, 0, 0, 0, 0, 0)
                    hashRisultati2.Add(key2, DtCodifica2)
                Else
                    DtCodifica2 = hashRisultati2(key2)
                End If


                If DtCodifica2.Rows.Count > 0 Then

                    Veg_Cod = DtCodifica2.Rows(0).Item("Veg_Cod")
                    Cul_Cod = DtCodifica2.Rows(0).Item("Cul_Cod")
                    Grfi_Cod = DtCodifica2.Rows(0).Item("Grfi_Cod")
                    Id_Cod = DtCodifica2.Rows(0).Item("Id_Cod")
                    Grva_Cod = DtCodifica2.Rows(0).Item("Grva_Cod")
                    Specie_Des_Agea = DtCodifica2.Rows(0).Item("Veg_Des_Agea")

                    Uso_Cod = DtCodifica2.Rows(0).Item("Uso_Cod")
                    Occupazione_Cod = DtCodifica2.Rows(0).Item("Occupazione_Cod")
                    Destinazione_Cod = DtCodifica2.Rows(0).Item("Destinazione_Cod")
                    Qualita_Cod = DtCodifica2.Rows(0).Item("Qualita_Cod")

                    If Not hashGruCod.Contains(Veg_Cod) Then
                        Gru_Cod = objSpecieVegetali.Leggi(Veg_Cod, 0, "", "", AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri).Rows(0).Item("Gru_Cod")
                        hashGruCod.Add(Veg_Cod, Gru_Cod)
                    Else
                        Gru_Cod = hashGruCod(Veg_Cod)
                    End If

                    If Not IsDBNull(DtCodifica2.Rows(0).Item("Cul_Des_Agea")) Then
                        Varieta_Des_Agea = DtCodifica2.Rows(0).Item("Cul_Des_Agea")
                    Else
                        Varieta_Des_Agea = ""
                    End If
                    Esito = True

                Else
                    If Cul_Cod_Agea <> "" Then
                        LogCodificheMancantiVarieta &= "Specie: " & Specie_Des_AgeaInput & ", Occupazione=" & Occupazione_Cod & ", Veg_Cod_Agea=" & CStr(Veg_Cod_Agea) &
                            " - la varietà " & Varieta_Des_AgeaInput & " con Cul_Cod_Agea=" & CStr(Cul_Cod_Agea) & " non è mappata -> è stata selezionata la varietà Altre." & vbCrLf & vbCrLf
                        LogCodificheMancantiVarieta_Excel &= vbTab & vbTab & Specie_Des_AgeaInput & vbTab & CStr(Veg_Cod_Agea) & vbTab & CStr(Occupazione_Cod) & vbTab & Varieta_Des_AgeaInput & vbTab & CStr(Cul_Cod_Agea)
                        Dim customLOGParamsCsv As New CustomLOGParams With {
                            .LogDescrizioneUtente = objParametri.UtenteUsername,
                            .LogDirectory = objParametri.LogDirectory,
                            .LogFileName = "CodificaSpecieVegetaliAgea_VARIETA_mancanti_Excel.csv"
                        }
                        Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiVarieta_Excel, CustomLOGParams:=customLOGParamsCsv)

                        Dim customLOGParamsTxt As New CustomLOGParams With {
                            .LogDescrizioneUtente = objParametri.UtenteUsername,
                            .LogDirectory = objParametri.LogDirectory,
                            .LogFileName = "CodificaSpecieVegetaliAgea_VARIETA_mancanti.txt"
                        }
                        Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiVarieta, CustomLOGParams:=customLOGParamsTxt)
                    End If

                    Veg_Cod = DtCodifica.Rows(0).Item("Veg_Cod")

                    If Veg_Cod = 0 Then
                        Cul_Cod = 0
                    Else
                        Dim objCultivar As New AgronicaCoreMetaSchemaDAL.Cultivar_R
                        Cul_Cod = objCultivar.VarietaAltre(Veg_Cod,
                                                           objParametri)
                    End If

                    Grfi_Cod = DtCodifica.Rows(0).Item("Grfi_Cod")
                    Id_Cod = DtCodifica.Rows(0).Item("Id_Cod")
                    Grva_Cod = DtCodifica.Rows(0).Item("Grva_Cod")
                    Specie_Des_Agea = DtCodifica.Rows(0).Item("Veg_Des_Agea")

                    Uso_Cod = DtCodifica.Rows(0).Item("Uso_Cod")
                    Occupazione_Cod = DtCodifica.Rows(0).Item("Occupazione_Cod")
                    Destinazione_Cod = DtCodifica.Rows(0).Item("Destinazione_Cod")
                    Qualita_Cod = DtCodifica.Rows(0).Item("Qualita_Cod")

                    If Veg_Cod <> 0 Then
                        If Not hashGruCod.Contains(Veg_Cod) Then
                            Gru_Cod = objSpecieVegetali.Leggi(Veg_Cod, 0, "", "", AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri).Rows(0).Item("Gru_Cod")
                            hashGruCod.Add(Veg_Cod, Gru_Cod)
                        Else
                            Gru_Cod = hashGruCod(Veg_Cod)
                        End If
                    End If

                    If Not IsDBNull(DtCodifica.Rows(0).Item("Cul_Des_Agea")) Then
                        Varieta_Des_Agea = DtCodifica.Rows(0).Item("Cul_Des_Agea")
                    Else
                        Varieta_Des_Agea = ""
                    End If

                    Esito = True
                End If

            End If

        Else
            Veg_Cod = 0
            Cul_Cod = 0
            Grfi_Cod = 0
            Grva_Cod = 0
            Uso_Cod = ""
            Occupazione_Cod = ""
            Destinazione_Cod = ""
            Qualita_Cod = ""
            Id_Cod = enum_CodiciAnagrafe.NessunaMappaturaConGias
            Specie_Des_Agea = "Nessuna mappatura con Gias."
            Varieta_Des_Agea = "Nessuna mappatura con Gias."
            'Specie_Des_Agea = "Nessuna mappatura con Gias (cod." & Veg_Cod_Agea & ")."
            'Varieta_Des_Agea = "Nessuna mappatura con Gias (cod." & Cul_Cod_Agea & ")."
            LogCodificheMancantiSpecie &= "Nessun record presente per il Veg_Cod_Agea=" & CStr(Veg_Cod_Agea) & "  " & Specie_Des_AgeaInput & ", Occupazione=" & CStr(Occupazione_Cod) & vbCrLf & vbCrLf
            LogCodificheMancantiSpecie_Excel &= vbTab & vbTab & Specie_Des_AgeaInput & vbTab & CStr(Occupazione_Cod) & vbTab & CStr(Veg_Cod_Agea)
            Dim customLOGParamsTxt As New CustomLOGParams With {
                .LogDescrizioneUtente = objParametri.UtenteUsername,
                .LogDirectory = objParametri.LogDirectory,
                .LogFileName = "CodificaSpecieVegetaliAgea_SPECIE_mancanti.txt"
            }
            Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiSpecie, CustomLOGParams:=customLOGParamsTxt)

            Dim customLOGParamsCsv As New CustomLOGParams With {
                .LogDescrizioneUtente = objParametri.UtenteUsername,
                .LogDirectory = objParametri.LogDirectory,
                .LogFileName = "CodificaSpecieVegetaliAgea_SPECIE_mancanti_Excel.csv"
            }
            Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiSpecie_Excel, CustomLOGParams:=customLOGParamsCsv)
        End If

        Return Esito

    End Function

    Public Function Specie_e_Varieta_Gias_Da_Agea_Da5Parametri(ByRef LogCodificheMancantiSpecie As String,
                                                  ByRef LogCodificheMancantiVarieta As String,
                                                    ByVal Cul_Cod_Agea As String,
                                                    ByRef Veg_Cod As Integer,
                                                    ByRef Cul_Cod As Integer,
                                                    ByRef Grfi_Cod As Integer,
                                                    ByRef Grva_Cod As Integer,
                                                    ByRef Id_Cod As Integer,
                                                    ByRef Specie_Des_Agea As String,
                                                    ByRef Varieta_Des_Agea As String,
                                                    ByVal Specie_Des_AgeaInput As String,
                                                    ByVal Varieta_Des_AgeaInput As String,
                                                    ByVal Data As Date,
                                                    ByRef Uso_Cod As String,
                                                    ByRef Occupazione_Cod As String,
                                                    ByRef Destinazione_Cod As String,
                                                    ByRef Qualita_Cod As String,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                    Optional ByRef Gru_Cod As Integer = 0,
                                                    Optional ByRef Veg_Cod_Agea As String = ""
                                                        ) As Boolean

        Dim NomeRoutine As String = "Specie_e_Varieta_Gias_Da_Agea"

        Dim DtCodifica As DataTable
        Dim DrVar() As DataRow
        Dim Esito As Boolean = False
        Dim LogCodificheMancantiVarieta_Excel As String = ""
        Dim LogCodificheMancantiSpecie_Excel As String = ""
        Veg_Cod = 0
        Cul_Cod = 0
        Grfi_Cod = 0
        Id_Cod = 0
        Grva_Cod = 0
        Specie_Des_Agea = ""
        Varieta_Des_Agea = ""
        Gru_Cod = 0
        'If Veg_Cod_Agea = "154" Then
        '    Dim i = 0
        'End If


        'LEGGO TUTTI I RECORD DELLA SPECIE  
        'filtro poi se è mappata anche la varietà
        'altrimenti metto 'altre'....
        Dim objAgeaR As New AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Agea_2015_2020_R
        'DtCodifica = Leggi(Veg_Cod_Agea,
        '                    "",
        '                    0, 0, 0,
        '                    Data,
        '                    AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
        '                    "", "",
        '                    objParametri)
        Dim objSpecieVegetali As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R

        If Veg_Cod_Agea <> "" AndAlso Veg_Cod_Agea <> "000" Then
            DtCodifica = objAgeaR.leggi(objParametri, Veg_Cod_Agea, "", Uso_Cod, "", Occupazione_Cod, Destinazione_Cod, Qualita_Cod, 0, 0, 0, 0, 0, 0, 0, 0)

            If DtCodifica.Rows.Count = 0 Then
                DtCodifica = objAgeaR.leggi(objParametri, Veg_Cod_Agea, "", Uso_Cod, "", Occupazione_Cod, Destinazione_Cod, Qualita_Cod, 0, 0, 0, 0, 0, 0, 0, 0)
            End If

            If DtCodifica.Rows.Count = 0 Then
                DtCodifica = objAgeaR.leggi(objParametri, Veg_Cod_Agea, "", Uso_Cod, "", Occupazione_Cod, Destinazione_Cod, "", 0, 0, 0, 0, 0, 0, 0, 0)
            End If

            If DtCodifica.Rows.Count = 0 Then
                DtCodifica = objAgeaR.leggi(objParametri, Veg_Cod_Agea, "", Uso_Cod, "", Occupazione_Cod, "", "", 0, 0, 0, 0, 0, 0, 0, 0)
            End If

            If DtCodifica.Rows.Count = 0 Then
                DtCodifica = objAgeaR.leggi(objParametri, Veg_Cod_Agea, "", "", "", "", "", "", 0, 0, 0, 0, 0, 0, 0, 0)
            End If
        Else
            DtCodifica = objAgeaR.leggi(objParametri, Veg_Cod_Agea, "", Uso_Cod, "", Occupazione_Cod, Destinazione_Cod, Qualita_Cod, 0, 0, 0, 0, 0, 0, 0, 0)

            If DtCodifica.Rows.Count = 0 Then
                DtCodifica = objAgeaR.leggi(objParametri, "", "", Uso_Cod, "", Occupazione_Cod, Destinazione_Cod, Qualita_Cod, 0, 0, 0, 0, 0, 0, 0, 0)
            End If

            If DtCodifica.Rows.Count = 0 Then
                DtCodifica = objAgeaR.leggi(objParametri, "", "", Uso_Cod, "", Occupazione_Cod, Destinazione_Cod, "", 0, 0, 0, 0, 0, 0, 0, 0)
            End If

            If DtCodifica.Rows.Count = 0 Then
                DtCodifica = objAgeaR.leggi(objParametri, "", "", Uso_Cod, "", Occupazione_Cod, "", "", 0, 0, 0, 0, 0, 0, 0, 0)
            End If

            If DtCodifica.Rows.Count = 0 Then
                DtCodifica = objAgeaR.leggi(objParametri, "", "", "", "", Occupazione_Cod, "", "", 0, 0, 0, 0, 0, 0, 0, 0)
            End If
        End If



        If Not IsNothing(DtCodifica) AndAlso DtCodifica.Rows.Count > 0 Then

            DrVar = DtCodifica.Select("Cul_Cod_Agea='" & Cul_Cod_Agea.ToString & "'")

            If Not DrVar Is Nothing AndAlso DrVar.Length > 0 Then

                Veg_Cod = DrVar(0).Item("Veg_Cod")


                If Veg_Cod <> 0 Then
                    Dim objCultivar As New AgronicaCoreMetaSchemaDAL.Cultivar_R
                    Dim Cul_Cod_Altre = objCultivar.VarietaAltre(Veg_Cod,
                                                       objParametri)

                    Cul_Cod = DrVar(0).Item("Cul_Cod")

                    For i = 0 To DrVar.Length - 1
                        If DrVar(i).Item("Cul_Cod") <> 0 AndAlso DrVar(i).Item("Cul_Cod") <> Cul_Cod_Altre And DrVar(i).Item("veg_cod") = Veg_Cod Then
                            Cul_Cod = DrVar(i).Item("Cul_Cod")
                            Exit For
                        End If
                    Next
                End If
                Grfi_Cod = DrVar(0).Item("Grfi_Cod")
                Id_Cod = DrVar(0).Item("Id_Cod")
                Grva_Cod = DrVar(0).Item("Grva_Cod")
                Specie_Des_Agea = DrVar(0).Item("Veg_Des_Agea")

                Uso_Cod = DrVar(0).Item("Uso_Cod")
                Occupazione_Cod = DrVar(0).Item("Occupazione_Cod")
                Destinazione_Cod = DrVar(0).Item("Destinazione_Cod")
                Qualita_Cod = DrVar(0).Item("Qualita_Cod")

                Gru_Cod = objSpecieVegetali.Leggi(Veg_Cod, 0, "", "", AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri).Rows(0).Item("Gru_Cod")

                If Not IsDBNull(DrVar(0).Item("Cul_Des_Agea")) Then
                    Varieta_Des_Agea = DrVar(0).Item("Cul_Des_Agea")
                Else
                    Varieta_Des_Agea = ""
                End If
                Esito = True

            Else

                If Cul_Cod_Agea <> "" Then
                    LogCodificheMancantiVarieta &= "Specie: " & Specie_Des_AgeaInput & ", Veg_Cod_Agea=" & CStr("") &
                        " - la varietà " & Varieta_Des_AgeaInput & " con Cul_Cod_Agea=" & CStr(Cul_Cod_Agea) & " non è mappata -> è stata selezionata la varietà Altre." & vbCrLf & vbCrLf
                    LogCodificheMancantiVarieta_Excel &= vbTab & vbTab & Specie_Des_AgeaInput & vbTab & CStr("") & vbTab & Varieta_Des_AgeaInput & vbTab & CStr(Cul_Cod_Agea)

                    Dim customLOGParamsCsv As New CustomLOGParams With {
                        .LogDescrizioneUtente = objParametri.UtenteUsername,
                        .LogDirectory = objParametri.LogDirectory,
                        .LogFileName = "CodificaSpecieVegetaliAgea_VARIETA_mancanti_Excel.csv"
                    }
                    Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiVarieta_Excel, CustomLOGParams:=customLOGParamsCsv)

                    Dim customLOGParamsTxt As New CustomLOGParams With {
                        .LogDescrizioneUtente = objParametri.UtenteUsername,
                        .LogDirectory = objParametri.LogDirectory,
                        .LogFileName = "CodificaSpecieVegetaliAgea_VARIETA_mancanti.txt"
                    }
                    Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiVarieta, CustomLOGParams:=customLOGParamsTxt)

                End If

                Veg_Cod = DtCodifica.Rows(0).Item("Veg_Cod")

                If Veg_Cod = 0 Then
                    Cul_Cod = 0
                Else
                    Dim objCultivar As New AgronicaCoreMetaSchemaDAL.Cultivar_R
                    Cul_Cod = objCultivar.VarietaAltre(Veg_Cod,
                                                       objParametri)
                End If

                Grfi_Cod = DtCodifica.Rows(0).Item("Grfi_Cod")
                Id_Cod = DtCodifica.Rows(0).Item("Id_Cod")
                Grva_Cod = DtCodifica.Rows(0).Item("Grva_Cod")
                Specie_Des_Agea = DtCodifica.Rows(0).Item("Veg_Des_Agea")

                Uso_Cod = DtCodifica.Rows(0).Item("Uso_Cod")
                Occupazione_Cod = DtCodifica.Rows(0).Item("Occupazione_Cod")
                Destinazione_Cod = DtCodifica.Rows(0).Item("Destinazione_Cod")
                Qualita_Cod = DtCodifica.Rows(0).Item("Qualita_Cod")

                If Veg_Cod <> 0 Then
                    Gru_Cod = objSpecieVegetali.Leggi(Veg_Cod, 0, "", "", AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri).Rows(0).Item("Gru_Cod")
                End If

                If Not IsDBNull(DtCodifica.Rows(0).Item("Cul_Des_Agea")) Then
                    Varieta_Des_Agea = DtCodifica.Rows(0).Item("Cul_Des_Agea")
                Else
                    Varieta_Des_Agea = ""
                End If

                Esito = True

            End If

        Else
            Veg_Cod = 0
            Cul_Cod = 0
            Grfi_Cod = 0
            Grva_Cod = 0
            Uso_Cod = ""
            Occupazione_Cod = ""
            Destinazione_Cod = ""
            Qualita_Cod = ""
            Id_Cod = enum_CodiciAnagrafe.NessunaMappaturaConGias
            Specie_Des_Agea = "Nessuna mappatura con Gias."
            Varieta_Des_Agea = "Nessuna mappatura con Gias."
            'Specie_Des_Agea = "Nessuna mappatura con Gias (cod." & Veg_Cod_Agea & ")."
            'Varieta_Des_Agea = "Nessuna mappatura con Gias (cod." & Cul_Cod_Agea & ")."
            LogCodificheMancantiSpecie &= "Nessun record presente per il Veg_Cod_Agea=" & CStr("") & "  " & Specie_Des_AgeaInput & vbCrLf & vbCrLf
            LogCodificheMancantiSpecie_Excel &= vbTab & vbTab & Specie_Des_AgeaInput & vbTab & CStr("")

            Dim customLOGParamsTxt As New CustomLOGParams With {
                .LogDescrizioneUtente = objParametri.UtenteUsername,
                .LogDirectory = objParametri.LogDirectory,
                .LogFileName = "CodificaSpecieVegetaliAgea_SPECIE_mancanti.txt"
            }
            Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiSpecie, CustomLOGParams:=customLOGParamsTxt)

            Dim customLOGParamsCsv As New CustomLOGParams With {
                .LogDescrizioneUtente = objParametri.UtenteUsername,
                .LogDirectory = objParametri.LogDirectory,
                .LogFileName = "CodificaSpecieVegetaliAgea_SPECIE_mancanti_Excel.csv"
            }
            Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiSpecie_Excel, CustomLOGParams:=customLOGParamsCsv)

        End If

        Return Esito

    End Function

    Public Function VegDesAgea_from_VegCodAgea(
                                     ByVal Veg_Cod_Agea As String,
                                     ByVal Cul_Cod_Agea As String,
                                     ByRef Cul_Des_Agea As String,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As String

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Agea_R.VegDesAgea_from_VegCodAgea()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim Veg_Des_Agea As String = ""

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT  Veg_Des_Agea, Cul_Des_Agea ")
            StrSQL.Append(" FROM    Codifica_SpecieVegetali_Agea_2015_2020 ")

            StrSQL.Append(" WHERE   Validita_Inizio <=" & Agro_SQL_SaveDate(CDate(objParametri.FinestraTemporaleFine)) & " ")
            StrSQL.Append(" AND     Validita_Fine >=" & Agro_SQL_SaveDate(CDate(objParametri.FinestraTemporaleInizio)) & " ")
            StrSQL.Append(" AND     Veg_Cod_Agea = " & Agro_SQL_SaveText_NULL(Veg_Cod_Agea) & " ")

            If Cul_Cod_Agea <> "" Then
                StrSQL.Append(" AND Cul_Cod_Agea = " & Agro_SQL_SaveText_NULL(Cul_Cod_Agea) & " ")
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
                StrSQL.Append(" ORDER BY Veg_Des_Agea, Cul_Des_Agea ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then
                Veg_Des_Agea = DT.Rows(0).Item("Veg_Des_Agea")
                'Cul_Des_Agea = DT.Rows(0).Item("Cul_Des_Agea")
                If Not IsDBNull(DT.Rows(0).Item("Cul_Des_Agea")) Then
                    Cul_Des_Agea = DT.Rows(0).Item("Cul_Des_Agea")
                Else
                    Cul_Des_Agea = ""
                End If

            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Veg_Des_Agea

    End Function

    Public Function Specie_e_Varieta_Gias_Da_Macrouso_Agea(ByRef LogCodificheMancantiSpecie As String,
                                                  ByRef LogCodificheMancantiVarieta As String,
                                                    ByVal Macrouso_Cod_Agea As String,
                                                    ByRef Veg_Cod_Agea As String,
                                                    ByRef Cul_Cod_Agea As String,
                                                    ByRef Veg_Cod As Integer,
                                                    ByRef Cul_Cod As Integer,
                                                    ByRef Grfi_Cod As Integer,
                                                    ByRef Grva_Cod As Integer,
                                                    ByRef Id_Cod As Integer,
                                                    ByRef Uso_Cod As String,
                                                    ByRef Occupazione_Cod As String,
                                                    ByRef Destinazione_Cod As String,
                                                    ByRef Qualita_Cod As String,
                                                    ByRef Veg_Des As String,
                                                    ByRef Cul_Des As String,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                    Optional ByRef Gru_Cod As Integer = 0
                                                        ) As Boolean

        Dim NomeRoutine As String = "Specie_e_Varieta_Gias_Da_Agea"

        Dim DtCodifica As DataTable
        Dim DrVar() As DataRow
        Dim Esito As Boolean = False
        Dim LogCodificheMancantiVarieta_Excel As String = ""
        Dim LogCodificheMancantiSpecie_Excel As String = ""
        Veg_Cod = 0
        Cul_Cod = 0
        Grfi_Cod = 0
        Id_Cod = 0
        Grva_Cod = 0
        Gru_Cod = 0
        'If Veg_Cod_Agea = "154" Then
        '    Dim i = 0
        'End If


        'LEGGO TUTTI I RECORD DELLA SPECIE  
        'filtro poi se è mappata anche la varietà
        'altrimenti metto 'altre'....
        Dim objAgeaR As New AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Agea_2015_2020_R
        'DtCodifica = Leggi(Veg_Cod_Agea,
        '                    "",
        '                    0, 0, 0,
        '                    Data,
        '                    AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
        '                    "", "",
        '                    objParametri)
        If Not hashSpecieEVarietaDaMacrouso.Contains(Macrouso_Cod_Agea) Then
            DtCodifica = objAgeaR.leggi(objParametri, "", "", "", Macrouso_Cod_Agea, "", "", "", 0, 0, 0, 0, 0, 0, 0, 0)
            hashSpecieEVarietaDaMacrouso.Add(Macrouso_Cod_Agea, DtCodifica)
        Else
            DtCodifica = hashSpecieEVarietaDaMacrouso(Macrouso_Cod_Agea)
        End If


        Dim objSpecieVegetali As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
        Dim objCultivar As New AgronicaCoreMetaSchemaDAL.Cultivar_R

        If DtCodifica IsNot Nothing Then
            If DtCodifica.Rows.Count > 0 Then
                Veg_Cod = DtCodifica.Rows(0).Item("Veg_Cod")
                Cul_Cod = DtCodifica.Rows(0).Item("Cul_Cod")
                Grfi_Cod = DtCodifica.Rows(0).Item("Grfi_Cod")
                Grva_Cod = DtCodifica.Rows(0).Item("Grva_Cod")
                Id_Cod = DtCodifica.Rows(0).Item("Id_Cod")
                Veg_Cod_Agea = DtCodifica.Rows(0).Item("Veg_Cod_Agea")
                Cul_Cod_Agea = DtCodifica.Rows(0).Item("Cul_Cod_Agea")
                Uso_Cod = DtCodifica.Rows(0).Item("Uso_Cod")
                Occupazione_Cod = DtCodifica.Rows(0).Item("Occupazione_Cod")
                Destinazione_Cod = DtCodifica.Rows(0).Item("Destinazione_Cod")
                Qualita_Cod = DtCodifica.Rows(0).Item("Qualita_Cod")
                Veg_Des = objSpecieVegetali.Leggi(Veg_Cod, 0, "", "", 1, "", "", objParametri).Rows(0).Item("Veg_Des")
                Cul_Des = objCultivar.Leggi(Cul_Cod, Veg_Cod, "", 1, "", "", objParametri).Rows(0).Item("Cul_Des")
            End If
        End If


        Return Esito

    End Function

End Class
