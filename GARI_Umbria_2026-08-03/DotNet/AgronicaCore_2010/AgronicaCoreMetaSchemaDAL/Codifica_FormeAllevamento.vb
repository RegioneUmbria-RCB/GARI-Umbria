Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider

Public Class Codifica_FormeAllevamento
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(Ente As String,
                          Veg_Cod_Cliente As String,
                          Veg_Cod_Gias As String,
                          Foral_Cod_Cliente As String,
                          Foral_Des_Cliente As String,
                          Foral_Cod_Gias As String,
                          ValiditaInizio As Date,
                          ValiditaFine As Date,
                          ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Codifica_FormeAllevamento.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Select Case xSelezioneVariabile


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    StrSQL.Length = 0
                    StrSQL.Append("Select Ente " & vbCrLf)
                    StrSQL.Append("       ,Veg_Cod_Cliente " & vbCrLf)
                    StrSQL.Append("       ,Veg_Cod_Gias " & vbCrLf)
                    StrSQL.Append("       ,Foral_Cod_Cliente " & vbCrLf)
                    StrSQL.Append("       ,Foral_Des_Cliente " & vbCrLf)
                    StrSQL.Append("       ,Foral_Cod_Gias " & vbCrLf)
                    StrSQL.Append("       ,inviato " & vbCrLf)
                    StrSQL.Append("       ,datainvio " & vbCrLf)
                    StrSQL.Append("       ,Data_Creazione " & vbCrLf)
                    StrSQL.Append("       ,Data_Modifica " & vbCrLf)
                    StrSQL.Append("       ,Username_Creazione " & vbCrLf)
                    StrSQL.Append("       ,Username_Modifica " & vbCrLf)
                    StrSQL.Append("       ,Validita_Inizio " & vbCrLf)
                    StrSQL.Append("       ,Validita_Fine " & vbCrLf)
                    StrSQL.Append("   FROM Codifica_FormeAllevamento " & vbCrLf)
                    StrSQL.Append("   WHERE 1=1 " & vbCrLf)
                    If Ente <> "" Then
                        StrSQL.Append("   AND Ente=" + Agro_SQL_SaveText_NULL(Ente) + " " & vbCrLf)
                    End If
                    If Veg_Cod_Cliente <> "" Then
                        StrSQL.Append("   AND Veg_Cod_Cliente = " + Agro_SQL_SaveText_NULL(Veg_Cod_Cliente) + " " & vbCrLf)
                    End If
                    If Veg_Cod_Gias <> 0 Then
                        StrSQL.Append("   AND Veg_Cod_Gias = " + Agro_SQL_SaveNum(Veg_Cod_Gias) + " " & vbCrLf)
                    End If
                    If Foral_Cod_Cliente <> "" Then
                        StrSQL.Append("   AND Foral_Cod_Cliente = " + Agro_SQL_SaveText_NULL(Foral_Cod_Cliente) + " " & vbCrLf)
                    End If
                    If Foral_Des_Cliente <> "" Then
                        StrSQL.Append("   AND Foral_Des_Cliente = " + Agro_SQL_SaveText_NULL(Foral_Des_Cliente) + " " & vbCrLf)
                    End If
                    If Foral_Cod_Gias <> 0 Then
                        StrSQL.Append("   AND Foral_Cod_Gias = " + Agro_SQL_SaveNum(Foral_Cod_Gias) + " ")
                    End If
                    StrSQL.Append("   AND Validita_Inizio >= " + Agro_SQL_SaveDateTime_NULL(ValiditaInizio) + " ")
                    StrSQL.Append("   AND Validita_Fine <= " + Agro_SQL_SaveDateTime_NULL(ValiditaFine) + " ")

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
                        StrSQL.Append(" ORDER BY Veg_Cod_Cliente, Foral_Cod_Cliente ")
                    End If


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

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

    Public Function Codifica_FormaAllevamento(ByRef LogCodificheMancantiSpecie As String, _
                                                  ByRef LogCodificheMancantiVarieta As String, _
                                                  ByRef Ente As String,
                                                    ByRef Veg_Cod_Cliente As String, _
                                                    ByRef Veg_Cod As Integer, _
                                                    ByRef Foral_Cod_Cliente As String, _
                                                    ByRef Foral_Des_Cliente As String, _
                                                    ByRef Foral_Cod_Gias As Integer, _
                                                    ByRef Data As Date, _
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Codifica_FormaAllevamento.Leggi()"
        Dim LogCodificheForal_Excel As String = ""
        Dim LogCodificheForal As String = ""
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim Esito As Boolean
        Try
            Dim DTForal = Leggi(Ente, Veg_Cod_Cliente, Veg_Cod, Foral_Cod_Cliente, "", 0, AGRODATAINIZIO, AGRODATAFINE,
                                AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri)

            If Not IsNothing(DTForal) AndAlso DTForal.Rows.Count > 0 Then
                Foral_Cod_Gias = DTForal.Rows(0).Item("Foral_Cod_Gias")
                Esito = True
            Else
                Dim Specie_Des As String = ""
                Select Case Ente
                    Case CStr(enum_EnteValidadore.Agea)
                        Dim objCodifica_Specie As New Codifica_SpecieVegetali_Agea_R
                        Dim DtCodifica = objCodifica_Specie.Leggi(Veg_Cod_Cliente, "", 0, 0, 0, AGRODATAINIZIO, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri)
                        If Not IsNothing(DtCodifica) AndAlso DtCodifica.Rows.Count > 0 Then
                            Specie_Des = DtCodifica.Rows(0).Item("Veg_Des_Agea")
                        End If
                    Case CStr(enum_EnteValidadore.Agrea)
                        Dim objCodifica_Specie As New Codifica_SpecieVegetali_Agrea_R
                        Dim DtCodifica = objCodifica_Specie.Leggi(Veg_Cod_Cliente, "", "", "", Veg_Cod, 0, 0, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri)
                        If Not IsNothing(DtCodifica) AndAlso DtCodifica.Rows.Count > 0 Then
                            Specie_Des = DtCodifica.Rows(0).Item("Veg_Des_Agrea")
                        End If
                End Select
                LogCodificheForal &= "Specie:" + Veg_Cod_Cliente + "-" + Specie_Des + "     ForalCod:" + Foral_Cod_Cliente + "-" + Foral_Des_Cliente + " Non Mappato!"
                LogCodificheForal_Excel &= vbTab & vbTab + Veg_Cod_Cliente & vbTab & Specie_Des & vbTab & Foral_Cod_Cliente & vbTab & Foral_Des_Cliente

                Dim customLOGParamsCsv As New CustomLOGParams With {
                    .LogDescrizioneUtente = objParametri.UtenteUsername,
                    .LogDirectory = objParametri.LogDirectory,
                    .LogFileName = "Codifica_FormeAllevamento_Mancanti.csv"
                }
                Scrivi_LOG(objParametri, NomeRoutine, LogCodificheForal_Excel, CustomLOGParams:=customLOGParamsCsv)

                Dim customLOGParamsTxt As New CustomLOGParams With {
                    .LogDescrizioneUtente = objParametri.UtenteUsername,
                    .LogDirectory = objParametri.LogDirectory,
                    .LogFileName = "Codifica_FormeAllevamento_Mancanti.txt"
                }
                Scrivi_LOG(objParametri, NomeRoutine, LogCodificheForal, CustomLOGParams:=customLOGParamsTxt)
                Esito = False
            End If

            Return Esito
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Return False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
    End Function

End Class
