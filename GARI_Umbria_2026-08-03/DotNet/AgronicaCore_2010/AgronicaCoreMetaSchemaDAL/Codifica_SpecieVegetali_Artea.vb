Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider

Public Class Codifica_SpecieVegetali_Artea_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(
                            ByVal Veg_Cod_Artea As String,
                            ByVal Cul_Cod_Artea As String,
                            ByVal Veg_Cod_Agea As String,
                            ByVal Cul_Cod_Agea As String,
                            ByVal Veg_Cod As Integer,
                            ByVal Cul_Cod As Integer,
                            ByVal Id_Cod As Integer,
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                Optional ByVal grfi_cod As Integer = 0,
                                Optional ByVal Grva_cod As Integer = 0
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Artea_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Select Case xSelezioneVariabile


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT  Veg_Cod_Artea, Cul_Cod_Artea, Veg_Cod_Agea, Cul_Cod_Agea, ISNULL(Veg_Des_Agea, '') AS Veg_Des_Agea, ISNULL(Cul_Des_Agea, '') AS Cul_Des_Agea, ")
                    StrSQL.Append(" Veg_cod,  Cul_cod, Grfi_cod, Grva_cod, Metodo_Produzione_cod, Reg_cod, Id_cod ")
                    StrSQL.Append(" FROM    Codifica_SpecieVegetali_Artea ")
                    StrSQL.Append(" WHERE   Validita_Inizio <=" & Agro_SQL_SaveDate(CDate(objParametri.FinestraTemporaleFine)) & " ")
                    StrSQL.Append(" AND     Validita_Fine >=" & Agro_SQL_SaveDate(CDate(objParametri.FinestraTemporaleInizio)) & " ")

                    If Veg_Cod_Artea <> "" Then
                        StrSQL.Append(" AND Veg_Cod_Artea = '" & Agro_SQL_SaveText(Veg_Cod_Artea) & "' ")
                    End If
                    If Cul_Cod_Artea <> "" Then
                        StrSQL.Append(" AND Cul_Cod_Artea = '" & Agro_SQL_SaveText(Cul_Cod_Artea) & "' ")
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
                    StrSQL.Append(" FROM        Codifica_SpecieVegetali_Artea ")
                    StrSQL.Append(" WHERE   Validita_Inizio <=" & Agro_SQL_SaveDate(CDate(objParametri.FinestraTemporaleFine)) & " ")
                    StrSQL.Append(" AND     Validita_Fine >=" & Agro_SQL_SaveDate(CDate(objParametri.FinestraTemporaleInizio)) & " ")

                    If Veg_Cod_Artea <> "" Then
                        StrSQL.Append(" AND Veg_Cod_Artea = '" & Agro_SQL_SaveText(Veg_Cod_Artea) & "' ")
                    End If
                    If Cul_Cod_Artea <> "" Then
                        StrSQL.Append(" AND Cul_Cod_Artea = '" & Agro_SQL_SaveText(Cul_Cod_Artea) & "' ")
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
                    StrSQL.Append(" SELECT  Veg_Cod_Artea, Cul_Cod_Artea, Veg_Cod_Agea, Cul_Cod_Agea, ISNULL(Veg_Des_Agea, '') AS Veg_Des_Agea, ISNULL(Cul_Des_Agea, '') AS Cul_Des_Agea, ")
                    StrSQL.Append(" Codifica_SpecieVegetali_Artea.Veg_cod,  Codifica_SpecieVegetali_Artea.Cul_cod, Codifica_SpecieVegetali_Artea.Grfi_cod, Codifica_SpecieVegetali_Artea.Grva_cod, Codifica_SpecieVegetali_Artea.Metodo_Produzione_cod, Codifica_SpecieVegetali_Artea.Reg_cod, Codifica_SpecieVegetali_Artea.Id_cod, ")
                    StrSQL.Append(" ISNULL(SpecieVegetali.Veg_Des,'') AS Veg_Des, ISNULL(Cultivar.Cul_Des,'') AS Cul_Des, ISNULL(GruppoVarietale.Grva_Des,'') AS Grva_Des, ISNULL(GruppoFinalita.Grfi_Des,'') AS Grfi_Des, ISNULL(Regolamenti.Reg_Des,'') AS Reg_Des, ISNULL(Codici_Anagrafe.descrizione,'') AS Id_Des ")

                    StrSQL.Append(" FROM         Codifica_SpecieVegetali_Artea LEFT OUTER JOIN ")
                    StrSQL.Append(" Codici_Anagrafe ON Codifica_SpecieVegetali_Artea.Id_cod = Codici_Anagrafe.codice LEFT OUTER JOIN ")
                    StrSQL.Append(" Regolamenti ON Codifica_SpecieVegetali_Artea.Reg_cod = Regolamenti.Reg_Cod LEFT OUTER JOIN ")
                    StrSQL.Append(" GruppoFinalita ON Codifica_SpecieVegetali_Artea.Grfi_cod = GruppoFinalita.Grfi_Cod LEFT OUTER JOIN ")
                    StrSQL.Append(" GruppoVarietale ON Codifica_SpecieVegetali_Artea.Grva_cod = GruppoVarietale.Grva_Cod LEFT OUTER JOIN ")
                    StrSQL.Append(" Cultivar ON Codifica_SpecieVegetali_Artea.Cul_cod = Cultivar.Cul_Cod LEFT OUTER JOIN ")
                    StrSQL.Append("  SpecieVegetali ON Codifica_SpecieVegetali_Artea.Veg_cod = SpecieVegetali.Veg_Cod ")

                    StrSQL.Append(" WHERE   Codifica_SpecieVegetali_Artea.Validita_Inizio <=" & Agro_SQL_SaveDate(CDate(objParametri.FinestraTemporaleFine)) & " ")
                    StrSQL.Append(" AND     Codifica_SpecieVegetali_Artea.Validita_Fine >=" & Agro_SQL_SaveDate(CDate(objParametri.FinestraTemporaleInizio)) & " ")

                    If Veg_Cod_Artea <> "" Then
                        StrSQL.Append(" AND Codifica_SpecieVegetali_Artea.Veg_Cod_Artea = '" & Agro_SQL_SaveText(Veg_Cod_Artea) & "' ")
                    End If
                    If Cul_Cod_Artea <> "" Then
                        StrSQL.Append(" AND Codifica_SpecieVegetali_Artea.Cul_Cod_Artea = '" & Agro_SQL_SaveText(Cul_Cod_Artea) & "' ")
                    End If
                    If Veg_Cod_Agea <> "" Then
                        StrSQL.Append(" AND Codifica_SpecieVegetali_Artea.Veg_Cod_Agea = '" & Agro_SQL_SaveText(Veg_Cod_Agea) & "' ")
                    End If
                    If Cul_Cod_Agea <> "" Then
                        StrSQL.Append(" AND Codifica_SpecieVegetali_Artea.Cul_Cod_Agea = '" & Agro_SQL_SaveText(Cul_Cod_Agea) & "' ")
                    End If

                    If Veg_Cod <> 0 Then
                        StrSQL.Append(" AND Codifica_SpecieVegetali_Artea.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
                    End If
                    If Cul_Cod <> 0 Then
                        StrSQL.Append(" AND Codifica_SpecieVegetali_Artea.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & " ")
                    End If
                    If Id_Cod <> 0 Then
                        StrSQL.Append(" AND Codifica_SpecieVegetali_Artea.Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & " ")
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
                            StrSQL.Append(" AND   Codifica_SpecieVegetali_Artea.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Codifica_SpecieVegetali_Artea.Inviato =-1 ")
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
                    StrSQL.Append(" FROM         Codifica_SpecieVegetali_Artea LEFT OUTER JOIN ")
                    StrSQL.Append(" Codici_Anagrafe ON Codifica_SpecieVegetali_Artea.Id_cod = Codici_Anagrafe.codice LEFT OUTER JOIN ")
                    StrSQL.Append(" Regolamenti ON Codifica_SpecieVegetali_Artea.Reg_cod = Regolamenti.Reg_Cod LEFT OUTER JOIN ")
                    StrSQL.Append(" GruppoFinalita ON Codifica_SpecieVegetali_Artea.Grfi_cod = GruppoFinalita.Grfi_Cod LEFT OUTER JOIN ")
                    StrSQL.Append(" GruppoVarietale ON Codifica_SpecieVegetali_Artea.Grva_cod = GruppoVarietale.Grva_Cod LEFT OUTER JOIN ")
                    StrSQL.Append(" Cultivar ON Codifica_SpecieVegetali_Artea.Cul_cod = Cultivar.Cul_Cod LEFT OUTER JOIN ")
                    StrSQL.Append("  SpecieVegetali ON Codifica_SpecieVegetali_Artea.Veg_cod = SpecieVegetali.Veg_Cod ")

                    StrSQL.Append(" WHERE   Codifica_SpecieVegetali_Artea.Validita_Inizio <=" & Agro_SQL_SaveDate(CDate(objParametri.FinestraTemporaleFine)) & " ")
                    StrSQL.Append(" AND     Codifica_SpecieVegetali_Artea.Validita_Fine >=" & Agro_SQL_SaveDate(CDate(objParametri.FinestraTemporaleInizio)) & " ")

                    If Veg_Cod_Artea <> "" Then
                        StrSQL.Append(" AND Codifica_SpecieVegetali_Artea.Veg_Cod_Artea = '" & Agro_SQL_SaveText(Veg_Cod_Artea) & "' ")
                    End If
                    If Cul_Cod_Artea <> "" Then
                        StrSQL.Append(" AND Codifica_SpecieVegetali_Artea.Cul_Cod_Artea = '" & Agro_SQL_SaveText(Cul_Cod_Artea) & "' ")
                    End If
                    If Veg_Cod_Agea <> "" Then
                        StrSQL.Append(" AND Codifica_SpecieVegetali_Artea.Veg_Cod_Agea = '" & Agro_SQL_SaveText(Veg_Cod_Agea) & "' ")
                    End If
                    If Cul_Cod_Agea <> "" Then
                        StrSQL.Append(" AND Codifica_SpecieVegetali_Artea.Cul_Cod_Agea = '" & Agro_SQL_SaveText(Cul_Cod_Agea) & "' ")
                    End If

                    If Veg_Cod <> 0 Then
                        StrSQL.Append(" AND Codifica_SpecieVegetali_Artea.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
                    End If
                    If Cul_Cod <> 0 Then
                        StrSQL.Append(" AND Codifica_SpecieVegetali_Artea.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & " ")
                    End If
                    If Id_Cod <> 0 Then
                        StrSQL.Append(" AND Codifica_SpecieVegetali_Artea.Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & " ")
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
                            StrSQL.Append(" AND   Codifica_SpecieVegetali_Artea.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Codifica_SpecieVegetali_Artea.Inviato =-1 ")
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

    Public Function Specie_e_Varieta_Gias_Da_Artea_Old(ByRef LogCodificheMancantiSpecie As String,
                                              ByRef LogCodificheMancantiVarieta As String,
                                                ByVal Veg_Cod_Artea As String,
                                                ByVal Cul_Cod_Artea As String,
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

        Dim NomeRoutine As String = "Specie_e_Varieta_Gias_Da_Artea"

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
        DtCodifica = Leggi(Veg_Cod_Artea,
                            "",
                            "", "", 0, 0, 0, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                            "", "",
                            objParametri)

        Dim customLOGParamsCsv As New CustomLOGParams With {
                        .LogDescrizioneUtente = objParametri.UtenteUsername,
                        .LogDirectory = objParametri.LogDirectory,
                        .LogFileName = $"CodificaSpecieVegetaliArtea_SPECIE_mancanti_Excel.csv"
                    }

        Dim customLOGParamsTxt As New CustomLOGParams With {
                        .LogDescrizioneUtente = objParametri.UtenteUsername,
                        .LogDirectory = objParametri.LogDirectory,
                        .LogFileName = $"CodificaSpecieVegetaliArtea_SPECIE_mancanti.txt"
                    }

        Dim customLOGParamsCsvCompleto As New CustomLOGParams With {
                        .LogDescrizioneUtente = objParametri.UtenteUsername,
                        .LogDirectory = objParametri.LogDirectory,
                        .LogFileName = $"CodificaSpecieVegetaliArtea_SPECIE_mancanti_Excel_Completo.csv"
        }

        If Not IsNothing(DtCodifica) AndAlso DtCodifica.Rows.Count > 0 Then

            DrVar = DtCodifica.Select("Cul_Cod_Artea='" & Cul_Cod_Artea.ToString & "'")

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

                If Cul_Cod_Artea <> "" Then
                    LogCodificheMancantiVarieta &= "Specie: " & Specie_Des_AgeaInput & ", Veg_Cod_Artea=" & CStr(Veg_Cod_Artea) &
                        " - la varietà " & Varieta_Des_AgeaInput & " con Cul_Cod_Artea=" & CStr(Cul_Cod_Artea) & " non è mappata -> è stata selezionata la varietà Altre." & vbCrLf & vbCrLf
                    LogCodificheMancantiVarieta_Excel &= ";" & ";" & Specie_Des_AgeaInput & ";" & CStr(Veg_Cod_Artea) & ";" & Varieta_Des_AgeaInput & ";" & CStr(Cul_Cod_Artea)
                    LogCodificheMancantiSpecie_Excel_Completo &= ";" & ";" & Specie_Des_AgeaInput & ";" & CStr(Veg_Cod_Artea) & ";" & Varieta_Des_AgeaInput & ";" & CStr(Cul_Cod_Artea)
                    Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiVarieta_Excel, CustomLOGParams:=customLOGParamsCsv)
                    Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiVarieta, CustomLOGParams:=customLOGParamsTxt)
                    'Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiSpecie_Excel_Completo, CustomLOGParams:=customLOGParamsCsvCompleto)
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
            LogCodificheMancantiSpecie &= "Nessun record presente per il Veg_Cod_Artea=" & CStr(Veg_Cod_Artea) & "  " & Specie_Des_AgeaInput & vbCrLf & vbCrLf
            LogCodificheMancantiSpecie_Excel &= vbTab & vbTab & Specie_Des_AgeaInput & vbTab & CStr(Veg_Cod_Artea)
            LogCodificheMancantiSpecie_Excel_Completo &= ";" & ";" & Specie_Des_AgeaInput & ";" & CStr(Veg_Cod_Artea) & ";" & Varieta_Des_AgeaInput & ";" & CStr(Cul_Cod_Artea)
            Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiSpecie, CustomLOGParams:=customLOGParamsTxt)
            Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiSpecie_Excel, CustomLOGParams:=customLOGParamsCsv)
            Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiSpecie_Excel_Completo, CustomLOGParams:=customLOGParamsCsvCompleto)
        End If

        Return Esito

    End Function

    Public Function Specie_e_Varieta_Gias_Da_Artea(ByRef LogCodificheMancantiSpecie As String,
                                              ByRef LogCodificheMancantiVarieta As String,
                                                ByVal Veg_Cod_Artea As String,
                                                ByVal Cul_Cod_Artea As String,
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
        Dim NomeRoutine As String = "Specie_e_Varieta_Gias_Da_Artea"
        Dim objMeta As New AgronicaCoreMetaSchemaDAL.Cultivar_R
        Dim Ente_Cod As Integer = enum_Planning_Fonte.Artea
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

        DtCodifica = objEnteR.leggi(objParametri, Ente_Cod, Veg_Cod_Artea, "", "", "", "", "", "", "", "")

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

            DrVar = DtCodifica.Select("Cul_Cod_Ente='" & Cul_Cod_Artea.ToString & "'")

            If Not DrVar Is Nothing AndAlso DrVar.Length > 0 Then

                Cul_Cod_Agea = ""
                If Not IsDBNull(DrVar(0).Item("Cul_Cod_Agea")) Then
                    Cul_Cod_Agea = DrVar(0).Item("Cul_Cod_Agea")
                End If

                Dim Occupazione_Cod As String = ""
                If Not IsDBNull(DrVar(0).Item("Occupazione_Cod")) Then
                    Occupazione_Cod = DrVar(0).Item("Occupazione_Cod")
                End If

                Dim Destinazione_Cod As String = ""
                If Not IsDBNull(DrVar(0).Item("Destinazione_Cod")) Then
                    Destinazione_Cod = DrVar(0).Item("Destinazione_Cod")
                End If

                Dim Qualita_Cod As String = ""
                If Not IsDBNull(DrVar(0).Item("Qualita_Cod")) Then
                    Qualita_Cod = DrVar(0).Item("Qualita_Cod")
                End If

                Dim Uso_Cod As String = ""
                If Not IsDBNull(DrVar(0).Item("Uso_Cod")) Then
                    Uso_Cod = DrVar(0).Item("Uso_Cod")
                End If

                DtAgea = objAgeaR.leggi(objParametri, DrVar(0).Item("Veg_Cod_Agea"), Cul_Cod_Agea, Uso_Cod, "", Occupazione_Cod, Destinazione_Cod, Qualita_Cod, 0, 0, 0, 0, 0, 0, 0, 0)
                If DtAgea.Rows.Count = 0 Then
                    DtAgea = objAgeaR.leggi(objParametri, "", Cul_Cod_Agea, Uso_Cod, "", Occupazione_Cod, Destinazione_Cod, Qualita_Cod, 0, 0, 0, 0, 0, 0, 0, 0)
                End If
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
                        Id_Cod = enum_CodiciAnagrafe.NessunaMappaturaConGias
                        Specie_Des_Agea = "Nessuna mappatura con Gias"
                        Varieta_Des_Agea = "Nessuna mappatura con Gias"
                        LogCodificheMancantiVarieta &= "Specie: " & DtAgea.Rows(0).Item("Veg_Des_AGEA") & ", Veg_Cod_Artea=" & DtAgea.Rows(0).Item("Veg_Cod_AGEA") &
                        " - la varietà " & DtAgea.Rows(0).Item("Cul_Des_Agea") & " con Cul_Cod_Artea=" & DtAgea.Rows(0).Item("Cul_Cod_AGEA") & " non è mappata con GIAS." & vbCrLf & vbCrLf
                        LogCodificheMancantiVarieta_Excel &= ";" & ";" & DtAgea.Rows(0).Item("Veg_Des_AGEA") & ";" & DtAgea.Rows(0).Item("Veg_Cod_AGEA") & ";" & DtAgea.Rows(0).Item("Cul_Des_Agea") & ";" & DtAgea.Rows(0).Item("Cul_Cod_AGEA") & "; Occupazione:" & ";" & CStr(Occupazione_Cod) & ";" & "Destinazione:" & ";" & CStr(Destinazione_Cod) & ";" & "Uso:" & ";" & CStr(Uso_Cod) & ";Qualita;" & CStr(Qualita_Cod) & ";VarietaAGEA:" & ";" & CStr(Cul_Cod_Agea)
                        LogCodificheMancantiSpecie_Excel_Completo &= ";" & ";" & DtAgea.Rows(0).Item("Veg_Des_AGEA") & ";" & DtAgea.Rows(0).Item("Veg_Cod_AGEA") & ";" & DtAgea.Rows(0).Item("Cul_Des_Agea") & ";" & DtAgea.Rows(0).Item("Cul_Cod_AGEA") & "; Occupazione:" & ";" & CStr(Occupazione_Cod) & ";" & "Destinazione:" & ";" & CStr(Destinazione_Cod) & ";" & "Uso:" & ";" & CStr(Uso_Cod) & ";Qualita;" & CStr(Qualita_Cod) & ";VarietaAGEA:" & ";" & CStr(Cul_Cod_Agea)
                        Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiVarieta_Excel, CustomLOGParams:=customLOGParamsCsv)
                        Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiVarieta, CustomLOGParams:=customLOGParamsTxt)
                        Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiSpecie_Excel_Completo, CustomLOGParams:=customLOGParamsCsvCompleto)
                        Esito = False
                    End If

                Else
                    Id_Cod = enum_CodiciAnagrafe.NessunaMappaturaConGias
                    Specie_Des_Agea = "Nessuna mappatura con Gias"
                    Varieta_Des_Agea = "Nessuna mappatura con Gias"
                    'Manca collegamento Artea-AGEA
                    LogCodificheMancantiVarieta &= "Specie: " & Specie_Des_AgeaInput & ", Veg_Cod_Artea=" & CStr(Veg_Cod_Artea) &
                        " - la varietà " & Varieta_Des_AgeaInput & " con Cul_Cod_Artea=" & CStr(Cul_Cod_Artea) & " non è mappata con AGEA." & vbCrLf & vbCrLf
                    LogCodificheMancantiVarieta_Excel &= ";" & ";" & Specie_Des_AgeaInput & ";" & CStr(Veg_Cod_Artea) & ";" & Varieta_Des_AgeaInput & ";" & CStr(Cul_Cod_Artea) & "; Occupazione:" & ";" & CStr(Occupazione_Cod) & ";" & "Destinazione:" & ";" & CStr(Destinazione_Cod) & ";" & "Uso:" & ";" & CStr(Uso_Cod) & ";Qualita;" & CStr(Qualita_Cod) & ";VarietaAGEA:" & ";" & CStr(Cul_Cod_Agea)
                    LogCodificheMancantiSpecie_Excel_Completo &= ";" & ";" & Specie_Des_AgeaInput & ";" & CStr(Veg_Cod_Artea) & ";" & Varieta_Des_AgeaInput & ";" & CStr(Cul_Cod_Artea) & "; Occupazione:" & ";" & CStr(Occupazione_Cod) & ";" & "Destinazione:" & ";" & CStr(Destinazione_Cod) & ";" & "Uso:" & ";" & CStr(Uso_Cod) & ";Qualita;" & CStr(Qualita_Cod) & ";VarietaAGEA:" & ";" & CStr(Cul_Cod_Agea)
                    Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiVarieta_Excel, CustomLOGParams:=customLOGParamsCsv)
                    Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiVarieta, CustomLOGParams:=customLOGParamsTxt)
                    Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiSpecie_Excel_Completo, CustomLOGParams:=customLOGParamsCsvCompleto)
                End If

            Else

                If Cul_Cod_Artea <> "" Then
                    LogCodificheMancantiVarieta &= "Specie: " & Specie_Des_AgeaInput & ", Veg_Cod_Artea=" & CStr(Veg_Cod_Artea) &
                        " - la varietà " & Varieta_Des_AgeaInput & " con Cul_Cod_Artea=" & CStr(Cul_Cod_Artea) & " non è mappata -> è stata selezionata la varietà Altre." & vbCrLf & vbCrLf
                    LogCodificheMancantiVarieta_Excel &= ";" & ";" & Specie_Des_AgeaInput & ";" & CStr(Veg_Cod_Artea) & ";" & Varieta_Des_AgeaInput & ";" & CStr(Cul_Cod_Artea)
                    LogCodificheMancantiSpecie_Excel_Completo &= ";" & ";" & Specie_Des_AgeaInput & ";" & CStr(Veg_Cod_Artea) & ";" & Varieta_Des_AgeaInput & ";" & CStr(Cul_Cod_Artea)
                    Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiVarieta_Excel, CustomLOGParams:=customLOGParamsCsv)
                    Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiVarieta, CustomLOGParams:=customLOGParamsTxt)
                    'Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiSpecie_Excel_Completo, CustomLOGParams:=customLOGParamsCsvCompleto)
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
                        Id_Cod = enum_CodiciAnagrafe.NessunaMappaturaConGias
                        Specie_Des_Agea = "Nessuna mappatura con Gias"
                        Varieta_Des_Agea = "Nessuna mappatura con Gias"
                        LogCodificheMancantiVarieta &= "Specie: " & DtAgea.Rows(0).Item("Veg_Des_AGEA") & ", Veg_Cod_Artea=" & DtAgea.Rows(0).Item("Veg_Cod_AGEA") &
                        " - la varietà " & DtAgea.Rows(0).Item("Cul_Des_Agea") & " con Cul_Cod_Artea=" & DtAgea.Rows(0).Item("Cul_Cod_AGEA") & " non è mappata con GIAS." & vbCrLf & vbCrLf
                        LogCodificheMancantiVarieta_Excel &= ";" & ";" & DtAgea.Rows(0).Item("Veg_Des_AGEA") & ";" & DtAgea.Rows(0).Item("Veg_Cod_AGEA") & ";" & DtAgea.Rows(0).Item("Cul_Des_Agea") & ";" & DtAgea.Rows(0).Item("Cul_Cod_AGEA")
                        LogCodificheMancantiSpecie_Excel_Completo &= ";" & ";" & DtAgea.Rows(0).Item("Veg_Des_AGEA") & ";" & DtAgea.Rows(0).Item("Veg_Cod_AGEA") & ";" & DtAgea.Rows(0).Item("Cul_Des_Agea") & ";" & DtAgea.Rows(0).Item("Cul_Cod_AGEA")
                        Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiVarieta_Excel, CustomLOGParams:=customLOGParamsCsv)
                        Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiVarieta, CustomLOGParams:=customLOGParamsTxt)
                        Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiSpecie_Excel_Completo, CustomLOGParams:=customLOGParamsCsvCompleto)
                        Esito = False
                    End If
                Else
                    'Manca collegamento Artea-AGEA
                    LogCodificheMancantiVarieta &= "Specie: " & Specie_Des_AgeaInput & ", Veg_Cod_Artea=" & CStr(Veg_Cod_Artea) &
                        " - la varietà " & Varieta_Des_AgeaInput & " con Cul_Cod_Artea=" & CStr(Cul_Cod_Artea) & " non è mappata con AGEA." & vbCrLf & vbCrLf
                    LogCodificheMancantiVarieta_Excel &= ";" & ";" & Specie_Des_AgeaInput & ";" & CStr(Veg_Cod_Artea) & ";" & Varieta_Des_AgeaInput & ";" & CStr(Cul_Cod_Artea)
                    LogCodificheMancantiSpecie_Excel_Completo &= ";" & ";" & Specie_Des_AgeaInput & ";" & CStr(Veg_Cod_Artea) & ";" & Varieta_Des_AgeaInput & ";" & CStr(Cul_Cod_Artea)
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
            LogCodificheMancantiSpecie &= "Nessun record presente per il Veg_Cod_Artea=" & CStr(Veg_Cod_Artea) & "  " & Specie_Des_AgeaInput & vbCrLf & vbCrLf
            LogCodificheMancantiSpecie_Excel &= vbTab & vbTab & Specie_Des_AgeaInput & vbTab & CStr(Veg_Cod_Artea)
            LogCodificheMancantiSpecie_Excel_Completo &= ";" & ";" & Specie_Des_AgeaInput & ";" & CStr(Veg_Cod_Artea) & ";" & Varieta_Des_AgeaInput & ";" & CStr(Cul_Cod_Artea)
            Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiSpecie, CustomLOGParams:=customLOGParamsTxt)
            Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiSpecie_Excel, CustomLOGParams:=customLOGParamsCsv)
            Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiSpecie_Excel_Completo, CustomLOGParams:=customLOGParamsCsvCompleto)
        End If

        Return Esito
    End Function

End Class
