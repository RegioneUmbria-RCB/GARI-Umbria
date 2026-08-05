Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreModelsSTD.attivita

Public Class Operazioni

    Public Function Leggi(ByVal FiltraImpostazioniUtente As Boolean, ByVal Tipo_GruppoOperazioni As String,
                          ByRef objParametri_Server As AgronicaCoreParametri, ByRef objParametri_Utenti As AgronicaCoreParametri
                          ) As List(Of AgronicaCoreModelsSTD.attivita.Lavorazione)
        Dim operazioniList As New List(Of AgronicaCoreModelsSTD.attivita.Lavorazione)


        Dim DTOperazioni As DataTable

        Dim Flag_FiltroOperazioniUtente As Boolean = False

        Dim objUtente As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

        Dim filtroUtente As String = ""
        Dim dt_FiltroUtente As DataTable

        If FiltraImpostazioniUtente = True Then

            dt_FiltroUtente = objUtente.Leggi(enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_LAVORAZIONI,
                                                       1,
                                                       enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                    "", "",
                                                    objParametri_Utenti)
            If dt_FiltroUtente.Rows.Count > 0 Then
                filtroUtente = " AND Operazioni.Lav_Cod in ("
                Dim j As Integer = 0
                For j = 0 To dt_FiltroUtente.Rows.Count - 1

                    If j <> 0 Then
                        filtroUtente = filtroUtente & " ,"
                    End If
                    filtroUtente = filtroUtente & dt_FiltroUtente.Rows(j).Item("ID_0")
                Next
                filtroUtente = filtroUtente & " )  "
                Flag_FiltroOperazioniUtente = True
            End If

        End If

        Dim FiltroAggiuntivo As String = ""

        FiltroAggiuntivo = STR_OP_NON_GESTITE & " AND  GruppoOperazioni.Tipo IN (" & Tipo_GruppoOperazioni & ")"

        FiltroAggiuntivo = FiltroAggiuntivo & filtroUtente

        Dim Ordinamento As String = " Operazioni.Lav_Des "

        Dim objOperazioniLeggi As New AgronicaCoreMetaSchemaDAL.Operazioni_R

        DTOperazioni = objOperazioniLeggi.Leggi(0, 0, 0, "", 0, "", "", False, False, False, False,
                                                    enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                    FiltroAggiuntivo,
                                                    Ordinamento,
                                                    objParametri_Server)

        operazioniList = (From row As DataRow In DTOperazioni.Rows
                          Select New AgronicaCoreModelsSTD.attivita.Lavorazione(row("LAV_COD"), row("LAV_DES"))).ToList

        Return operazioniList

    End Function

    ''' <summary>
    ''' Legge le operazioni e i relativi gruppi disponibili per l'impostazione 7 (filtro lavorazioni/filtro operazioni)
    ''' </summary>
    ''' <param name="objParametri_Server"></param>
    ''' <returns></returns>
    Public Function CaricaOperazioniPerFiltroImpostazione(ByRef objParametri_Server As AgronicaCoreParametri)
        Dim objOperazioniLeggi As New AgronicaCoreMetaSchemaDAL.Operazioni_R
        Dim xFiltro = "GRU_COD in (1, 2, 3, 4, 6, 10)"

        Dim DTOperazioni = objOperazioniLeggi.Leggi(0, 0, 0, "", 0, "", "",
                                                    False, False, False, False,
                                                    enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                    xFiltro,
                                                    "GRU_COD",
                                                    objParametri_Server)

        Dim lavorazioni = (From row As DataRow In DTOperazioni.Rows
                           Select New AgronicaCoreModelsSTD.attivita.Lavorazione With {
                              .tipo = TipiJob.LAVORAZIONE,
                              .primaryKey = New Job.PK("Lavorazione", row("LAV_COD").ToString()),
                              .descrizione = row("LAV_DES"),
                              .categoriaOperazione = New categorie.CategoriaOperazione(row("GRU_COD"), row("GRU_DES"))
                          }).ToList()

        Return lavorazioni
    End Function

End Class
