Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider

Public Class Codifica_SpecieVegetali_Avepa_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi( _
                            ByVal Veg_Cod_Avepa As String, _
                            ByVal Cul_Cod_Avepa As String, _
                            ByVal Veg_Cod_Agea As String, _
                            ByVal Cul_Cod_Agea As String, _
                            ByVal Veg_Cod As Integer, _
                            ByVal Cul_Cod As Integer, _
                            ByVal Id_Cod As Integer, _
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                Optional ByVal grfi_cod As Integer = 0, _
                                Optional ByVal Grva_cod As Integer = 0 _
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Avepa_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Select Case xSelezioneVariabile


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT  Veg_Cod_Avepa, Cul_Cod_Avepa, Veg_Cod_Agea, Cul_Cod_Agea, ISNULL(Veg_Des_Agea, '') AS Veg_Des_Agea, ISNULL(Cul_Des_Agea, '') AS Cul_Des_Agea, ")
                    StrSQL.Append(" Veg_cod,  Cul_cod, Grfi_cod, Grva_cod, Metodo_Produzione_cod, Reg_cod, Id_cod ")
                    StrSQL.Append(" FROM    Codifica_SpecieVegetali_Avepa ")
                    StrSQL.Append(" WHERE   Validita_Inizio <=" & Agro_SQL_SaveDate(CDate(objParametri.FinestraTemporaleFine)) & " ")
                    StrSQL.Append(" AND     Validita_Fine >=" & Agro_SQL_SaveDate(CDate(objParametri.FinestraTemporaleInizio)) & " ")

                    If Veg_Cod_Avepa <> "" Then
                        StrSQL.Append(" AND Veg_Cod_Avepa = '" & Agro_SQL_SaveText(Veg_Cod_Avepa) & "' ")
                    End If
                    If Cul_Cod_Avepa <> "" Then
                        StrSQL.Append(" AND Cul_Cod_Avepa = '" & Agro_SQL_SaveText(Cul_Cod_Avepa) & "' ")
                    End If

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
                    StrSQL.Append(" FROM        Codifica_SpecieVegetali_Avepa ")
                    StrSQL.Append(" WHERE   Validita_Inizio <=" & Agro_SQL_SaveDate(CDate(objParametri.FinestraTemporaleFine)) & " ")
                    StrSQL.Append(" AND     Validita_Fine >=" & Agro_SQL_SaveDate(CDate(objParametri.FinestraTemporaleInizio)) & " ")

                    If Veg_Cod_Avepa <> "" Then
                        StrSQL.Append(" AND Veg_Cod_Avepa = '" & Agro_SQL_SaveText(Veg_Cod_Avepa) & "' ")
                    End If
                    If Cul_Cod_Avepa <> "" Then
                        StrSQL.Append(" AND Cul_Cod_Avepa = '" & Agro_SQL_SaveText(Cul_Cod_Avepa) & "' ")
                    End If
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
                    StrSQL.Append(" SELECT  Veg_Cod_Avepa, Cul_Cod_Avepa, Veg_Cod_Agea, Cul_Cod_Agea, ISNULL(Veg_Des_Agea, '') AS Veg_Des_Agea, ISNULL(Cul_Des_Agea, '') AS Cul_Des_Agea, ")
                    StrSQL.Append(" Codifica_SpecieVegetali_Avepa.Veg_cod,  Codifica_SpecieVegetali_Avepa.Cul_cod, Codifica_SpecieVegetali_Avepa.Grfi_cod, Codifica_SpecieVegetali_Avepa.Grva_cod, Codifica_SpecieVegetali_Avepa.Metodo_Produzione_cod, Codifica_SpecieVegetali_Avepa.Reg_cod, Codifica_SpecieVegetali_Avepa.Id_cod, ")
                    StrSQL.Append(" ISNULL(SpecieVegetali.Veg_Des,'') AS Veg_Des, ISNULL(Cultivar.Cul_Des,'') AS Cul_Des, ISNULL(GruppoVarietale.Grva_Des,'') AS Grva_Des, ISNULL(GruppoFinalita.Grfi_Des,'') AS Grfi_Des, ISNULL(Regolamenti.Reg_Des,'') AS Reg_Des, ISNULL(Codici_Anagrafe.descrizione,'') AS Id_Des ")

                    StrSQL.Append(" FROM         Codifica_SpecieVegetali_Avepa LEFT OUTER JOIN ")
                    StrSQL.Append(" Codici_Anagrafe ON Codifica_SpecieVegetali_Avepa.Id_cod = Codici_Anagrafe.codice LEFT OUTER JOIN ")
                    StrSQL.Append(" Regolamenti ON Codifica_SpecieVegetali_Avepa.Reg_cod = Regolamenti.Reg_Cod LEFT OUTER JOIN ")
                    StrSQL.Append(" GruppoFinalita ON Codifica_SpecieVegetali_Avepa.Grfi_cod = GruppoFinalita.Grfi_Cod LEFT OUTER JOIN ")
                    StrSQL.Append(" GruppoVarietale ON Codifica_SpecieVegetali_Avepa.Grva_cod = GruppoVarietale.Grva_Cod LEFT OUTER JOIN ")
                    StrSQL.Append(" Cultivar ON Codifica_SpecieVegetali_Avepa.Cul_cod = Cultivar.Cul_Cod LEFT OUTER JOIN ")
                    StrSQL.Append("  SpecieVegetali ON Codifica_SpecieVegetali_Avepa.Veg_cod = SpecieVegetali.Veg_Cod ")

                    StrSQL.Append(" WHERE   Codifica_SpecieVegetali_Avepa.Validita_Inizio <=" & Agro_SQL_SaveDate(CDate(objParametri.FinestraTemporaleFine)) & " ")
                    StrSQL.Append(" AND     Codifica_SpecieVegetali_Avepa.Validita_Fine >=" & Agro_SQL_SaveDate(CDate(objParametri.FinestraTemporaleInizio)) & " ")

                    If Veg_Cod_Avepa <> "" Then
                        StrSQL.Append(" AND Codifica_SpecieVegetali_Avepa.Veg_Cod_Avepa = '" & Agro_SQL_SaveText(Veg_Cod_Avepa) & "' ")
                    End If
                    If Cul_Cod_Avepa <> "" Then
                        StrSQL.Append(" AND Codifica_SpecieVegetali_Avepa.Cul_Cod_Avepa = '" & Agro_SQL_SaveText(Cul_Cod_Avepa) & "' ")
                    End If
                    If Veg_Cod_Agea <> "" Then
                        StrSQL.Append(" AND Codifica_SpecieVegetali_Avepa.Veg_Cod_Agea = '" & Agro_SQL_SaveText(Veg_Cod_Agea) & "' ")
                    End If
                    If Cul_Cod_Agea <> "" Then
                        StrSQL.Append(" AND Codifica_SpecieVegetali_Avepa.Cul_Cod_Agea = '" & Agro_SQL_SaveText(Cul_Cod_Agea) & "' ")
                    End If

                    If Veg_Cod <> 0 Then
                        StrSQL.Append(" AND Codifica_SpecieVegetali_Avepa.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
                    End If
                    If Cul_Cod <> 0 Then
                        StrSQL.Append(" AND Codifica_SpecieVegetali_Avepa.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & " ")
                    End If
                    If Id_Cod <> 0 Then
                        StrSQL.Append(" AND Codifica_SpecieVegetali_Avepa.Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & " ")
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
                            StrSQL.Append(" AND   Codifica_SpecieVegetali_Avepa.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Codifica_SpecieVegetali_Avepa.Inviato =-1 ")
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
                    StrSQL.Append(" FROM         Codifica_SpecieVegetali_Avepa LEFT OUTER JOIN ")
                    StrSQL.Append(" Codici_Anagrafe ON Codifica_SpecieVegetali_Avepa.Id_cod = Codici_Anagrafe.codice LEFT OUTER JOIN ")
                    StrSQL.Append(" Regolamenti ON Codifica_SpecieVegetali_Avepa.Reg_cod = Regolamenti.Reg_Cod LEFT OUTER JOIN ")
                    StrSQL.Append(" GruppoFinalita ON Codifica_SpecieVegetali_Avepa.Grfi_cod = GruppoFinalita.Grfi_Cod LEFT OUTER JOIN ")
                    StrSQL.Append(" GruppoVarietale ON Codifica_SpecieVegetali_Avepa.Grva_cod = GruppoVarietale.Grva_Cod LEFT OUTER JOIN ")
                    StrSQL.Append(" Cultivar ON Codifica_SpecieVegetali_Avepa.Cul_cod = Cultivar.Cul_Cod LEFT OUTER JOIN ")
                    StrSQL.Append("  SpecieVegetali ON Codifica_SpecieVegetali_Avepa.Veg_cod = SpecieVegetali.Veg_Cod ")

                    StrSQL.Append(" WHERE   Codifica_SpecieVegetali_Avepa.Validita_Inizio <=" & Agro_SQL_SaveDate(CDate(objParametri.FinestraTemporaleFine)) & " ")
                    StrSQL.Append(" AND     Codifica_SpecieVegetali_Avepa.Validita_Fine >=" & Agro_SQL_SaveDate(CDate(objParametri.FinestraTemporaleInizio)) & " ")

                    If Veg_Cod_Avepa <> "" Then
                        StrSQL.Append(" AND Codifica_SpecieVegetali_Avepa.Veg_Cod_Avepa = '" & Agro_SQL_SaveText(Veg_Cod_Avepa) & "' ")
                    End If
                    If Cul_Cod_Avepa <> "" Then
                        StrSQL.Append(" AND Codifica_SpecieVegetali_Avepa.Cul_Cod_Avepa = '" & Agro_SQL_SaveText(Cul_Cod_Avepa) & "' ")
                    End If
                    If Veg_Cod_Agea <> "" Then
                        StrSQL.Append(" AND Codifica_SpecieVegetali_Avepa.Veg_Cod_Agea = '" & Agro_SQL_SaveText(Veg_Cod_Agea) & "' ")
                    End If
                    If Cul_Cod_Agea <> "" Then
                        StrSQL.Append(" AND Codifica_SpecieVegetali_Avepa.Cul_Cod_Agea = '" & Agro_SQL_SaveText(Cul_Cod_Agea) & "' ")
                    End If

                    If Veg_Cod <> 0 Then
                        StrSQL.Append(" AND Codifica_SpecieVegetali_Avepa.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
                    End If
                    If Cul_Cod <> 0 Then
                        StrSQL.Append(" AND Codifica_SpecieVegetali_Avepa.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & " ")
                    End If
                    If Id_Cod <> 0 Then
                        StrSQL.Append(" AND Codifica_SpecieVegetali_Avepa.Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & " ")
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
                            StrSQL.Append(" AND   Codifica_SpecieVegetali_Avepa.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Codifica_SpecieVegetali_Avepa.Inviato =-1 ")
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

    Public Function Specie_e_Varieta_Gias_Da_Avepa_Old(ByRef LogCodificheMancantiSpecie As String,
                                              ByRef LogCodificheMancantiVarieta As String,
                                                ByVal Veg_Cod_Avepa As String,
                                                ByVal Cul_Cod_Avepa As String,
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
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                    ) As Boolean

        Dim NomeRoutine As String = "Specie_e_Varieta_Gias_Da_Agea"

        Dim LogCodificheMancantiSpecie_Excel As String = ""
        Dim LogCodificheMancantiVarieta_Excel As String = ""
        Dim LogCodificheMancantiSpecie_Excel_Completo As String = ""
        Dim DtCodifica As DataTable
        Dim DrVar() As DataRow
        Dim Esito As Boolean = False
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
        DtCodifica = Leggi(Veg_Cod_Avepa,
                            "",
                            "", "", 0, 0, 0, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                            "", "",
                            objParametri)

        Dim customLOGParamsCsv As New CustomLOGParams With {
                        .LogDescrizioneUtente = objParametri.UtenteUsername,
                        .LogDirectory = objParametri.LogDirectory,
                        .LogFileName = $"CodificaSpecieVegetaliAvepa_VARIETA_mancanti_Excel.csv"
                    }

        Dim customLOGParamsTxt As New CustomLOGParams With {
                        .LogDescrizioneUtente = objParametri.UtenteUsername,
                        .LogDirectory = objParametri.LogDirectory,
                        .LogFileName = $"CodificaSpecieVegetaliAvepa_VARIETA_mancanti.txt"
                    }

        Dim customLOGParamsCsvCompleto As New CustomLOGParams With {
                        .LogDescrizioneUtente = objParametri.UtenteUsername,
                        .LogDirectory = objParametri.LogDirectory,
                        .LogFileName = $"CodificaSpecieVegetaliAvepa_VARIETA_mancanti_Excel_Completo.csv"
                    }

        If Not IsNothing(DtCodifica) AndAlso DtCodifica.Rows.Count > 0 Then

            DrVar = DtCodifica.Select("Cul_Cod_Avepa='" & Cul_Cod_Avepa.ToString & "'")

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

                If Cul_Cod_Avepa <> "" Then
                    LogCodificheMancantiVarieta &= "Specie: " & Specie_Des_AgeaInput & ", Veg_Cod_Avepa=" & CStr(Veg_Cod_Avepa) &
                        " - la varietà " & Varieta_Des_AgeaInput & " con Cul_Cod_Avepa=" & CStr(Cul_Cod_Avepa) & " non è mappata -> è stata selezionata la varietà Altre." & vbCrLf & vbCrLf
                    LogCodificheMancantiVarieta_Excel &= ";" & ";" & Specie_Des_AgeaInput & ";" & CStr(Veg_Cod_Avepa) & ";" & Varieta_Des_AgeaInput & ";" & CStr(Cul_Cod_Avepa)
                    LogCodificheMancantiSpecie_Excel_Completo &= ";" & ";" & Specie_Des_AgeaInput & ";" & CStr(Veg_Cod_Avepa) & ";" & Varieta_Des_AgeaInput & ";" & CStr(Cul_Cod_Avepa)
                    Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiVarieta_Excel, CustomLOGParams:=customLOGParamsCsv)
                    Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiVarieta, CustomLOGParams:=customLOGParamsTxt)
                    Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiSpecie_Excel_Completo, CustomLOGParams:=customLOGParamsCsvCompleto)
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
            Specie_Des_Agea = "Nessuna mappatura con Gias"
            Varieta_Des_Agea = "Nessuna mappatura con Gias"
            LogCodificheMancantiSpecie &= "Nessun record presente per il Veg_Cod_Avepa=" & CStr(Veg_Cod_Avepa) & "  " & Specie_Des_AgeaInput & vbCrLf & vbCrLf
            LogCodificheMancantiSpecie_Excel &= vbTab & vbTab & Specie_Des_AgeaInput & vbTab & CStr(Veg_Cod_Avepa)
            LogCodificheMancantiSpecie_Excel_Completo &= ";" & ";" & Specie_Des_AgeaInput & ";" & CStr(Veg_Cod_Avepa) & ";" & Varieta_Des_AgeaInput & ";" & CStr(Cul_Cod_Avepa)
            customLOGParamsTxt.LogFileName = "CodificaSpecieVegetaliAvepa_SPECIE_mancanti.txt"
            Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiSpecie, CustomLOGParams:=customLOGParamsTxt)
            customLOGParamsCsv.LogFileName = "CodificaSpecieVegetaliAvepa_SPECIE_mancanti_Excel.csv"
            Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiSpecie_Excel, CustomLOGParams:=customLOGParamsCsv)
            customLOGParamsCsvCompleto.LogFileName = "CodificaSpecieVegetaliAvepa_SPECIE_mancanti_Excel_Completo.csv"
            Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiSpecie_Excel_Completo, CustomLOGParams:=customLOGParamsCsvCompleto)
        End If

        Return Esito

    End Function

    Public Function Specie_e_Varieta_Gias_Da_Avepa(ByRef LogCodificheMancantiSpecie As String,
                                              ByRef LogCodificheMancantiVarieta As String,
                                                ByVal Veg_Cod_Avepa As String,
                                                ByVal Cul_Cod_Avepa As String,
                                                ByRef Veg_Cod_Agea As String,
                                                ByRef Cul_Cod_Agea As String,
                                                ByRef Veg_Cod As Integer,
                                                ByRef Cul_Cod As Integer,
                                                ByRef Grfi_Cod As Integer,
                                                ByRef Grva_Cod As Integer,
                                                ByRef Id_Cod As Integer,
                                                ByRef Specie_Des_Agea As String,
                                                ByRef Varieta_Des_Agea As String,
                                                ByVal Specie_Des_AgeaInput As String,
                                                ByVal Varieta_Des_AgeaInput As String,
                                                ByRef Uso_Cod_Agea As String,
                                                ByRef Occupazione_Cod_Agea As String,
                                                ByRef Destinazione_Cod_Agea As String,
                                                ByRef Qualita_Cod_Agea As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                    ) As Boolean

        Dim NomeRoutine As String = "Specie_e_Varieta_Gias_Da_Avepa"
        Dim objMeta As New AgronicaCoreMetaSchemaDAL.Cultivar_R
        Dim Ente_Cod As Integer = enum_Planning_Fonte.Avepa
        Dim LogCodificheMancantiSpecie_Excel As String = ""
        Dim LogCodificheMancantiVarieta_Excel As String = ""
        Dim LogCodificheMancantiSpecie_Excel_Completo As String = ""
        Dim DtCodifica As DataTable
        Dim DrVar() As DataRow
        Dim DtAgea As DataTable
        Dim Esito As Boolean = False
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

        Dim objEnteR As New AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Enti_2015_2020_R
        Dim objAgeaR As New AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Agea_2015_2020_R

        DtCodifica = objEnteR.leggi(objParametri, Ente_Cod, Veg_Cod_Avepa, "", "", "", "", "", "", "", "")

        Dim customLOGParamsCsv As New CustomLOGParams With {
                        .LogDescrizioneUtente = objParametri.UtenteUsername,
                        .LogDirectory = objParametri.LogDirectory,
                        .LogFileName = $"CodificaSpecieVegetaliAGEA_GIAS_VARIETA_mancanti_Excel.csv"
                    }

        Dim customLOGParamsTxt As New CustomLOGParams With {
                        .LogDescrizioneUtente = objParametri.UtenteUsername,
                        .LogDirectory = objParametri.LogDirectory,
                        .LogFileName = $"CodificaSpecieVegetaliAGEA_GIAS_VARIETA_mancanti.txt"
                    }

        Dim customLOGParamsCsvCompleto As New CustomLOGParams With {
                        .LogDescrizioneUtente = objParametri.UtenteUsername,
                        .LogDirectory = objParametri.LogDirectory,
                        .LogFileName = $"CodificaSpecieVegetaliAGEA_GIAS_VARIETA_mancanti_Excel_Completo.csv"
                    }

        If Not IsNothing(DtCodifica) AndAlso DtCodifica.Rows.Count > 0 Then

            DrVar = DtCodifica.Select("Cul_Cod_Ente='" & Cul_Cod_Avepa.ToString & "'")

            If Not DrVar Is Nothing AndAlso DrVar.Length > 0 Then

                DtAgea = objAgeaR.leggi(objParametri, DrVar(0).Item("Veg_Cod_Agea"), DrVar(0).Item("Cul_Cod_Agea"), DrVar(0).Item("Uso_Cod"), "", DrVar(0).Item("Occupazione_Cod"), DrVar(0).Item("Destinazione_Cod"), DrVar(0).Item("Qualita_Cod"), 0, 0, 0, 0, 0, 0, 0, 0)
                If Not DtAgea Is Nothing AndAlso DtAgea.Rows.Count > 0 Then
                    Veg_Cod = DtAgea.Rows(0).Item("Veg_Cod")
                    Cul_Cod = DtAgea.Rows(0).Item("Cul_Cod")
                    Grfi_Cod = DtAgea.Rows(0).Item("Grfi_Cod")
                    Id_Cod = DtAgea.Rows(0).Item("Id_Cod")
                    Grva_Cod = DtAgea.Rows(0).Item("Grva_Cod")

                    Veg_Cod_Agea = DtAgea.Rows(0).Item("Veg_Cod_Agea")
                    Cul_Cod_Agea = DtAgea.Rows(0).Item("Cul_Cod_Agea")
                    Uso_Cod_Agea = DtAgea.Rows(0).Item("Uso_Cod")
                    Occupazione_Cod_Agea = DtAgea.Rows(0).Item("Occupazione_Cod")
                    Destinazione_Cod_Agea = DtAgea.Rows(0).Item("Destinazione_Cod")
                    Qualita_Cod_Agea = DtAgea.Rows(0).Item("Qualita_Cod")

                    If Not IsDBNull(DtAgea.Rows(0).Item("Cul_Des_Agea")) Then
                        Varieta_Des_Agea = DtAgea.Rows(0).Item("Cul_Des_Agea")
                    Else
                        Varieta_Des_Agea = ""
                    End If
                    Esito = True

                    If Veg_Cod <> 0 And Cul_Cod = 0 Then
                        Cul_Cod = objMeta.VarietaAltre(Veg_Cod, objParametri)
                    End If

                    If Veg_Cod = 0 And Cul_Cod = 0 And Id_Cod = 0 Then
                        LogCodificheMancantiVarieta &= "Specie: " & DtAgea.Rows(0).Item("Veg_Des_AGEA") & ", Veg_Cod_Avepa=" & DtAgea.Rows(0).Item("Veg_Cod_AGEA") &
                        " - la varietà " & DtAgea.Rows(0).Item("Cul_Des_Agea") & " con Cul_Cod_Avepa=" & DtAgea.Rows(0).Item("Cul_Cod_AGEA") & " non è mappata con GIAS." & vbCrLf & vbCrLf
                        LogCodificheMancantiVarieta_Excel &= ";" & ";" & DtAgea.Rows(0).Item("Veg_Des_AGEA") & ";" & DtAgea.Rows(0).Item("Veg_Cod_AGEA") & ";" & DtAgea.Rows(0).Item("Cul_Des_Agea") & ";" & DtAgea.Rows(0).Item("Cul_Cod_AGEA")
                        LogCodificheMancantiSpecie_Excel_Completo &= ";" & ";" & DtAgea.Rows(0).Item("Veg_Des_AGEA") & ";" & DtAgea.Rows(0).Item("Veg_Cod_AGEA") & ";" & DtAgea.Rows(0).Item("Cul_Des_Agea") & ";" & DtAgea.Rows(0).Item("Cul_Cod_AGEA")
                        Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiVarieta_Excel, CustomLOGParams:=customLOGParamsCsv)
                        Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiVarieta, CustomLOGParams:=customLOGParamsTxt)
                        Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiSpecie_Excel_Completo, CustomLOGParams:=customLOGParamsCsvCompleto)
                        Esito = False
                    End If

                Else
                    'Manca collegamento AVEPA-AGEA
                    LogCodificheMancantiVarieta &= "Specie: " & Specie_Des_AgeaInput & ", Veg_Cod_Avepa=" & CStr(Veg_Cod_Avepa) &
                        " - la varietà " & Varieta_Des_AgeaInput & " con Cul_Cod_Avepa=" & CStr(Cul_Cod_Avepa) & " non è mappata con AGEA." & vbCrLf & vbCrLf
                    LogCodificheMancantiVarieta_Excel &= ";" & ";" & Specie_Des_AgeaInput & ";" & CStr(Veg_Cod_Avepa) & ";" & Varieta_Des_AgeaInput & ";" & CStr(Cul_Cod_Avepa)
                    LogCodificheMancantiSpecie_Excel_Completo &= ";" & ";" & Specie_Des_AgeaInput & ";" & CStr(Veg_Cod_Avepa) & ";" & Varieta_Des_AgeaInput & ";" & CStr(Cul_Cod_Avepa)
                    Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiVarieta_Excel, CustomLOGParams:=customLOGParamsCsv)
                    Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiVarieta, CustomLOGParams:=customLOGParamsTxt)
                    Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiSpecie_Excel_Completo, CustomLOGParams:=customLOGParamsCsvCompleto)
                End If

            Else

                If Cul_Cod_Avepa <> "" Then
                    LogCodificheMancantiVarieta &= "Specie: " & Specie_Des_AgeaInput & ", Veg_Cod_Avepa=" & CStr(Veg_Cod_Avepa) &
                        " - la varietà " & Varieta_Des_AgeaInput & " con Cul_Cod_Avepa=" & CStr(Cul_Cod_Avepa) & " non è mappata -> è stata selezionata la varietà Altre." & vbCrLf & vbCrLf
                    LogCodificheMancantiVarieta_Excel &= ";" & ";" & Specie_Des_AgeaInput & ";" & CStr(Veg_Cod_Avepa) & ";" & Varieta_Des_AgeaInput & ";" & CStr(Cul_Cod_Avepa)
                    LogCodificheMancantiSpecie_Excel_Completo &= ";" & ";" & Specie_Des_AgeaInput & ";" & CStr(Veg_Cod_Avepa) & ";" & Varieta_Des_AgeaInput & ";" & CStr(Cul_Cod_Avepa)
                    Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiVarieta_Excel, CustomLOGParams:=customLOGParamsCsv)
                    Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiVarieta, CustomLOGParams:=customLOGParamsTxt)
                End If


                DtAgea = objAgeaR.leggi(objParametri, "", "", DtCodifica.Rows(0).Item("Uso_Cod"), "", DtCodifica.Rows(0).Item("Occupazione_Cod"), DtCodifica.Rows(0).Item("Destinazione_Cod"), DtCodifica.Rows(0).Item("Qualita_Cod"), 0, 0, 0, 0, 0, 0, 0, 0)

                If Not DtAgea Is Nothing AndAlso DtAgea.Rows.Count > 0 Then
                    Veg_Cod = DtAgea.Rows(0).Item("Veg_Cod")
                    If Veg_Cod = 0 Then
                        Cul_Cod = 0
                    Else
                        Dim objCultivar As New AgronicaCoreMetaSchemaDAL.Cultivar_R
                        Cul_Cod = objCultivar.VarietaAltre(Veg_Cod,
                                                           objParametri)
                    End If

                    Grfi_Cod = DtAgea.Rows(0).Item("Grfi_Cod")
                    Id_Cod = DtAgea.Rows(0).Item("Id_Cod")
                    Grva_Cod = DtAgea.Rows(0).Item("Grva_Cod")
                    Specie_Des_Agea = DtAgea.Rows(0).Item("Veg_Des_Agea")

                    Veg_Cod_Agea = DtAgea.Rows(0).Item("Veg_Cod_Agea")
                    Cul_Cod_Agea = ""
                    Uso_Cod_Agea = DtAgea.Rows(0).Item("Uso_Cod")
                    Occupazione_Cod_Agea = DtAgea.Rows(0).Item("Occupazione_Cod")
                    Destinazione_Cod_Agea = DtAgea.Rows(0).Item("Destinazione_Cod")
                    Qualita_Cod_Agea = DtAgea.Rows(0).Item("Qualita_Cod")

                    If Not IsDBNull(DtAgea.Rows(0).Item("Cul_Des_Agea")) Then
                        Varieta_Des_Agea = DtAgea.Rows(0).Item("Cul_Des_Agea")
                    Else
                        Varieta_Des_Agea = ""
                    End If

                    Esito = True

                    If Veg_Cod <> 0 And Cul_Cod = 0 Then
                        Cul_Cod = objMeta.VarietaAltre(Veg_Cod, objParametri)
                    End If

                    If Veg_Cod = 0 And Cul_Cod = 0 And Id_Cod = 0 Then
                        LogCodificheMancantiVarieta &= "Specie: " & DtAgea.Rows(0).Item("Veg_Des_AGEA") & ", Veg_Cod_Avepa=" & DtAgea.Rows(0).Item("Veg_Cod_AGEA") &
                        " - la varietà " & DtAgea.Rows(0).Item("Cul_Des_Agea") & " con Cul_Cod_Avepa=" & DtAgea.Rows(0).Item("Cul_Cod_AGEA") & " non è mappata con GIAS." & vbCrLf & vbCrLf
                        LogCodificheMancantiVarieta_Excel &= ";" & ";" & DtAgea.Rows(0).Item("Veg_Des_AGEA") & ";" & DtAgea.Rows(0).Item("Veg_Cod_AGEA") & ";" & DtAgea.Rows(0).Item("Cul_Des_Agea") & ";" & DtAgea.Rows(0).Item("Cul_Cod_AGEA")
                        LogCodificheMancantiSpecie_Excel_Completo &= ";" & ";" & DtAgea.Rows(0).Item("Veg_Des_AGEA") & ";" & DtAgea.Rows(0).Item("Veg_Cod_AGEA") & ";" & DtAgea.Rows(0).Item("Cul_Des_Agea") & ";" & DtAgea.Rows(0).Item("Cul_Cod_AGEA")
                        Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiVarieta_Excel, CustomLOGParams:=customLOGParamsCsv)
                        Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiVarieta, CustomLOGParams:=customLOGParamsTxt)
                        Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiSpecie_Excel_Completo, CustomLOGParams:=customLOGParamsCsvCompleto)
                        Esito = False
                    End If
                Else
                    'Manca collegamento AVEPA-AGEA
                    LogCodificheMancantiVarieta &= "Specie: " & Specie_Des_AgeaInput & ", Veg_Cod_Avepa=" & CStr(Veg_Cod_Avepa) &
                        " - la varietà " & Varieta_Des_AgeaInput & " con Cul_Cod_Avepa=" & CStr(Cul_Cod_Avepa) & " non è mappata con AGEA." & vbCrLf & vbCrLf
                    LogCodificheMancantiVarieta_Excel &= ";" & ";" & Specie_Des_AgeaInput & ";" & CStr(Veg_Cod_Avepa) & ";" & Varieta_Des_AgeaInput & ";" & CStr(Cul_Cod_Avepa)
                    LogCodificheMancantiSpecie_Excel_Completo &= ";" & ";" & Specie_Des_AgeaInput & ";" & CStr(Veg_Cod_Avepa) & ";" & Varieta_Des_AgeaInput & ";" & CStr(Cul_Cod_Avepa)
                    Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiVarieta_Excel, CustomLOGParams:=customLOGParamsCsv)
                    Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiVarieta, CustomLOGParams:=customLOGParamsTxt)
                    Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiSpecie_Excel_Completo, CustomLOGParams:=customLOGParamsCsvCompleto)
                End If
            End If

        Else
            Veg_Cod = 0
            Cul_Cod = 0
            Grfi_Cod = 0
            Grva_Cod = 0
            Id_Cod = enum_CodiciAnagrafe.NessunaMappaturaConGias
            Specie_Des_Agea = "Nessuna mappatura con Gias"
            Varieta_Des_Agea = "Nessuna mappatura con Gias"
            LogCodificheMancantiSpecie &= "Nessun record presente per il Veg_Cod_Avepa=" & CStr(Veg_Cod_Avepa) & "  " & Specie_Des_AgeaInput & vbCrLf & vbCrLf
            LogCodificheMancantiSpecie_Excel &= vbTab & vbTab & Specie_Des_AgeaInput & vbTab & CStr(Veg_Cod_Avepa)
            LogCodificheMancantiSpecie_Excel_Completo &= ";" & ";" & Specie_Des_AgeaInput & ";" & CStr(Veg_Cod_Avepa) & ";" & Varieta_Des_AgeaInput & ";" & CStr(Cul_Cod_Avepa)
            customLOGParamsTxt.LogFileName = "CodificaSpecieVegetaliAvepa_SPECIE_mancanti.txt"
            Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiSpecie, CustomLOGParams:=customLOGParamsTxt)
            customLOGParamsCsv.LogFileName = "CodificaSpecieVegetaliAvepa_SPECIE_mancanti_Excel.csv"
            Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiSpecie_Excel, CustomLOGParams:=customLOGParamsCsv)
            customLOGParamsCsvCompleto.LogFileName = "CodificaSpecieVegetaliAvepa_SPECIE_mancanti_Excel_Completo.csv"
            Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiSpecie_Excel_Completo, CustomLOGParams:=customLOGParamsCsvCompleto)

        End If

        Return Esito

    End Function

End Class
