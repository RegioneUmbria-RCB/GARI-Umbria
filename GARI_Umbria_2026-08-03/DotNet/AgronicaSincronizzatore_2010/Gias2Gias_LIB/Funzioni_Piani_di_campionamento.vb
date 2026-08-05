Imports System.Xml
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider

Imports AgronicaCoreDataProvider

Imports System.Text
Imports AgronicaCorePianidiCampionamentoDAL

'Imports AgronicaCorePianidiCampionamentoDAL



Partial Public Class Funzioni


    Public Sub Elabora_Piani_di_campionamento_Salva( _
                                    ByVal objOpzioni As clsOpzioni, _
                                    ByRef Log_Import As StringBuilder, _
                                    ByRef Log_Errori As StringBuilder, _
                                    ByRef Log_Riepilogo As StringBuilder _
                            )

        Const nomeFunzione As String = "Elabora_Piani_di_campionamento_Salva"



        Try

            Dim dtPDC_Analisi As DataTable
            Dim dtPDC_Campioni As DataTable
            Dim dtPDC_CapitolatiCliente_Attivi As DataTable
            Dim dtPDC_Dettagli As DataTable
            Dim dtPDC_LFO As DataTable
            Dim dtPDC_Sblocca As DataTable
            Dim dtPDC_Testata As DataTable
            Dim dtAnalisi_Conformita_Capitolato_Cliente As DataTable


            Dim leggiPDC_Analisi As New PDC_Analisi_R
            Dim leggiPDC_Campioni As New PDC_Campioni_R
            Dim leggiPDC_CapitolatiCliente_Attivi As New PDC_CapitolatiCliente_Attivi_R
            Dim leggiPDC_Dettagli As New PDC_Dettagli_R
            Dim leggiPDC_LFO As New PDC_LFO_R
            Dim leggiPDC_Sblocca As New PDC_Sblocca_R
            Dim leggiPDC_Testata As New PDC_R
            Dim leggiAnalisi_Conformita_Capitolato_Cliente As New Analisi_Conformita_Capitolato_Cliente_R



            Dim scriviPDC_Analisi As New PDC_Analisi_W
            Dim scriviPDC_Campioni As New PDC_Campioni_W
            Dim scriviPDC_CapitolatiCliente_Attivi As New PDC_CapitolatiCliente_Attivi_W
            Dim scriviPDC_Dettagli As New PDC_Dettagli_W
            Dim scriviPDC_LFO As New PDC_LFO_W
            Dim scriviPDC_Sblocca As New PDC_Sblocca_W
            Dim scriviPDC_Testata As New PDC_W
            Dim scriviAnalisi_Conformita_Capitolato_Cliente As New Analisi_Conformita_Capitolato_Cliente_W


            Dim SeqTab As New AgronicaCoreDataProvider.Agro_Sequenze

            Dim recodeLFO As New List(Of recode_LFO)



            dtPDC_Testata = leggiPDC_Testata.Leggi(0, "", 0, 0, "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objOpzioni.objParametri_Server_GIAS_ORIGINE)


            For Each iCurrdtPDC_Testata In dtPDC_Testata.Rows

                Dim NuovaTestata As Integer
                'NuovaTestata = SeqTab.Agronica_SequenzaTabelle_NuovoID(
                '    "pdc_testata",
                '    objOpzioni.objParametri_Server_GIAS_DESTINAZIONE
                '    )

                'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
                NuovaTestata = SeqTab.NuovoId_Tabella("pdc_testata", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)

                Dim lICampagna As Integer = 0
                If Not IsDBNull(iCurrdtPDC_Testata("Da_Campagna")) Then
                    lICampagna = iCurrdtPDC_Testata("Da_Campagna")
                End If

                scriviPDC_Testata.Scrivi( _
                    NuovaTestata _
                    , iCurrdtPDC_Testata("PDC_Testata_Des") _
                    , iCurrdtPDC_Testata("PDC_Data_Istantanea") _
                    , iCurrdtPDC_Testata("PDC_Stato") _
                    , lICampagna _
                    , iCurrdtPDC_Testata("Validita_inizio") _
                    , iCurrdtPDC_Testata("Validita_fine") _
                    , iCurrdtPDC_Testata("CodiceStabilimento") _
                    , objOpzioni.objParametri_Server_GIAS_DESTINAZIONE _
                    , iCurrdtPDC_Testata("data_creazione") _
                    , iCurrdtPDC_Testata("data_modifica") _
                    , iCurrdtPDC_Testata("username_creazione") _
                    , iCurrdtPDC_Testata("username_modifica") _
                )


                Dim lVecchiaTestata As Integer = iCurrdtPDC_Testata("ID_PDC_Testata")
                dtPDC_Dettagli = leggiPDC_Dettagli.Leggi( _
                                lVecchiaTestata, _
                                0, _
                                "", _
                                0, _
                                0, _
                                0, _
                                0, _
                                0, _
                                0, _
                                0, _
                                "", _
                                "", _
                                objOpzioni.objParametri_Server_GIAS_ORIGINE)


                dtPDC_LFO = leggiPDC_LFO.Leggi(lVecchiaTestata, "", "", objOpzioni.objParametri_Server_GIAS_ORIGINE)

                'ID lfo è anche in dettagli
                For Each iCurrdtPDC_LFO In dtPDC_LFO.Rows
                    Dim nuovoLFO As Integer
                    'nuovoLFO = SeqTab.Agronica_SequenzaTabelle_NuovoID(
                    '    "pdc_lfo",
                    '    objOpzioni.objParametri_Server_GIAS_DESTINAZIONE
                    ')

                    'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
                    nuovoLFO = SeqTab.NuovoId_Tabella("pdc_lfo", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)

                    Dim llfo As Object = iCurrdtPDC_LFO("ID_LFO")
                    scriviPDC_LFO.Scrivi( _
                        NuovaTestata _
                        , nuovoLFO _
                        , iCurrdtPDC_LFO("LFO_DES") _
                        , objOpzioni.objParametri_Server_GIAS_DESTINAZIONE _
                        )

                    recodeLFO.Add(New recode_LFO With {.FROM_LFO = llfo, .To_LFO = nuovoLFO})


                Next


                For Each iCurrdtPDC_Dettagli In dtPDC_Dettagli.Rows

                    Dim nuovoDettaglio As Integer
                    'nuovoDettaglio = SeqTab.Agronica_SequenzaTabelle_NuovoID(
                    '"pdc_dettagli",
                    'objOpzioni.objParametri_Server_GIAS_DESTINAZIONE
                    ')

                    'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
                    nuovoDettaglio = SeqTab.NuovoId_Tabella("pdc_dettagli", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)


                    'per sa_cod, appezza, id_reg non esiste nulla in zani (e la cosa va gestita per altri)

                    Dim lDecLFO As Integer
                    Dim vecLFO As Object = iCurrdtPDC_Dettagli("ID_LFO")
                    lDecLFO = ( _
                        From l In recodeLFO _
                        Where l.FROM_LFO = vecLFO _
                        Select l.To_LFO _
                        ).FirstOrDefault

                    Dim lDtSa_Nome As String = ""

                    If Not IsDBNull(iCurrdtPDC_Dettagli("Sa_Nome")) Then
                        lDtSa_Nome = iCurrdtPDC_Dettagli("Sa_Nome")
                    End If
                    Dim lDtApp_Nome As String = ""

                    If Not IsDBNull(iCurrdtPDC_Dettagli("App_Nome")) Then
                        lDtApp_Nome = iCurrdtPDC_Dettagli("App_Nome")
                    End If
                    Dim lDtSup_Imp As Double = 0

                    If Not IsDBNull(iCurrdtPDC_Dettagli("Sup_Imp")) Then
                        lDtSup_Imp = iCurrdtPDC_Dettagli("Sup_Imp")
                    End If
                    Dim lDtCapitolatoPrivato As String = ""

                    If Not IsDBNull(iCurrdtPDC_Dettagli("CapitolatoPrivato")) Then
                        lDtCapitolatoPrivato = iCurrdtPDC_Dettagli("CapitolatoPrivato")
                    End If
                    Dim lDtCertificato As String = ""

                    If Not IsDBNull(iCurrdtPDC_Dettagli("Certificato")) Then
                        lDtCertificato = iCurrdtPDC_Dettagli("Certificato")
                    End If

                    Dim lDtData_Semina As DateTime = #1/1/1900#

                    If Not IsDBNull(iCurrdtPDC_Dettagli("Data_Semina")) Then
                        lDtData_Semina = iCurrdtPDC_Dettagli("Data_Semina")
                    End If

                    Dim lDtData_Raccolta As DateTime = #12/31/2100#

                    If Not IsDBNull(iCurrdtPDC_Dettagli("Data_Raccolta")) Then
                        lDtData_Raccolta = iCurrdtPDC_Dettagli("Data_Raccolta")
                    End If


                    Dim lDtDescrizione As String = ""

                    If Not IsDBNull(iCurrdtPDC_Dettagli("Descrizione")) Then
                        lDtDescrizione = iCurrdtPDC_Dettagli("Descrizione")
                    End If

                    Dim lDtDocumentazione As String = ""

                    If Not IsDBNull(iCurrdtPDC_Dettagli("Documentazione")) Then
                        lDtDocumentazione = iCurrdtPDC_Dettagli("Documentazione")
                    End If
                    Dim lDtRegolamento As String = ""

                    If Not IsDBNull(iCurrdtPDC_Dettagli("Regolamento")) Then
                        lDtRegolamento = iCurrdtPDC_Dettagli("Regolamento")
                    End If

                    Dim lDtNote2 As String = ""
                    If Not IsDBNull(iCurrdtPDC_Dettagli("Note2")) Then
                        lDtNote2 = iCurrdtPDC_Dettagli("Note2")
                    End If



                    scriviPDC_Dettagli.Scrivi_x_Acquisti( _
                        NuovaTestata _
                        , nuovoDettaglio _
                    , iCurrdtPDC_Dettagli("PDC_Dettagli_Des") _
                    , iCurrdtPDC_Dettagli("Data_Fornitura") _
                    , iCurrdtPDC_Dettagli("Piva") _
                    , iCurrdtPDC_Dettagli("Sa_Cod") _
                    , iCurrdtPDC_Dettagli("Appezza") _
                    , iCurrdtPDC_Dettagli("Id_Reg") _
                    , iCurrdtPDC_Dettagli("Rag_Soc") _
                    , iCurrdtPDC_Dettagli("CodiceFornitore") _
                    , iCurrdtPDC_Dettagli("Veg_Cod") _
                    , iCurrdtPDC_Dettagli("Cul_Cod") _
                    , iCurrdtPDC_Dettagli("Grva_Cod") _
                    , lDecLFO _
                    , iCurrdtPDC_Dettagli("Mat_Cod") _
                    , iCurrdtPDC_Dettagli("CodiceArticolo") _
                    , iCurrdtPDC_Dettagli("NomeArticolo") _
                    , iCurrdtPDC_Dettagli("LottoFornitore") _
                    , iCurrdtPDC_Dettagli("TipoLotta") _
                    , iCurrdtPDC_Dettagli("PuntoPrelievo") _
                    , iCurrdtPDC_Dettagli("Disciplinare_Cod") _
                    , iCurrdtPDC_Dettagli("note_impianto") _
                    , iCurrdtPDC_Dettagli("Documentazione") _
                    , iCurrdtPDC_Dettagli("Note_Documentazione") _
                    , iCurrdtPDC_Dettagli("Flag_Pdc") _
                    , iCurrdtPDC_Dettagli("Fornitore_Azienda") _
                    , iCurrdtPDC_Dettagli("Tipo_Lotta_Acquisti") _
                    , iCurrdtPDC_Dettagli("Fornitore_2") _
                    , objOpzioni.objParametri_Server_GIAS_DESTINAZIONE _
                    , lDtSa_Nome _
                    , lDtApp_Nome _
                    , lDtSup_Imp _
                    , lDtCapitolatoPrivato _
                    , lDtCertificato _
                    , lDtData_Semina _
                    , lDtData_Raccolta _
                    , lDtDescrizione _
                    , lDtRegolamento _
                    , lDtNote2 _
                    , iCurrdtPDC_Dettagli("data_creazione") _
                    , iCurrdtPDC_Dettagli("data_modifica") _
                    , iCurrdtPDC_Dettagli("username_creazione") _
                    , iCurrdtPDC_Dettagli("username_modifica") _
                    )



                    dtPDC_Sblocca = leggiPDC_Sblocca.Leggi(lVecchiaTestata, iCurrdtPDC_Dettagli("piva"),
                                                           iCurrdtPDC_Dettagli("sa_cod"), iCurrdtPDC_Dettagli("appezza"),
                                                           iCurrdtPDC_Dettagli("id_reg"),
                                                           0, 0,
                                                           "", "", objOpzioni.objParametri_Server_GIAS_ORIGINE)
                    For Each iCurrdtPDC_Sblocca In dtPDC_Sblocca.Rows
                        scriviPDC_Sblocca.Scrivi(NuovaTestata,
                                                 iCurrdtPDC_Sblocca("piva"),
                                                 iCurrdtPDC_Sblocca("sa_cod"),
                                                 iCurrdtPDC_Sblocca("appezza"),
                                                 iCurrdtPDC_Sblocca("id_reg"),
                                                 iCurrdtPDC_Sblocca("capitolatoCliente_Cod"),
                                                 iCurrdtPDC_Sblocca("Esito"),
                                                 iCurrdtPDC_Sblocca("Note"),
                                                 iCurrdtPDC_Sblocca("Analisi_Tipologia"),
                                                 If(IsDBNull(iCurrdtPDC_Sblocca("Analisi_Associata")), 0, iCurrdtPDC_Sblocca("Analisi_Associata")),
                                                 objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)
                    Next

                    Dim vecchioDettagliCod As Integer = iCurrdtPDC_Dettagli("ID_PDC_Dettagli")

                    dtPDC_Campioni = leggiPDC_Campioni.Leggi(lVecchiaTestata, vecchioDettagliCod, 0, "", "", objOpzioni.objParametri_Server_GIAS_ORIGINE)
                    For Each iCurrdtPDC_Campioni In dtPDC_Campioni.Rows

                        Dim nuovoCampione As Integer
                        'nuovoCampione = SeqTab.Agronica_SequenzaTabelle_NuovoID(
                        '    "pdc_campioni",
                        '    objOpzioni.objParametri_Server_GIAS_DESTINAZIONE
                        '    )

                        'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
                        nuovoCampione = SeqTab.NuovoId_Tabella("pdc_campioni", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)

                        scriviPDC_Campioni.Scrivi(
                            NuovaTestata _
                            , nuovoDettaglio _
                            , nuovoCampione _
                            , iCurrdtPDC_Campioni("PDC_Campione_Des") _
                            , iCurrdtPDC_Campioni("ID_PDC_Stato_Campione") _
                            , iCurrdtPDC_Campioni("Data_Campionamento") _
                            , iCurrdtPDC_Campioni("Codice_Campione") _
                            , iCurrdtPDC_Campioni("Punto_Prelievo") _
                            , iCurrdtPDC_Campioni("Note_Campione") _
                            , iCurrdtPDC_Campioni("Tecnico_Campione") _
                            , iCurrdtPDC_Campioni("Codice_Progressivo_Inizio_Anno") _
                            , iCurrdtPDC_Campioni("Codice_Anno") _
                            , iCurrdtPDC_Campioni("Tipo_Campione") _
                            , objOpzioni.objParametri_Server_GIAS_DESTINAZIONE
                        )


                        Dim nuovoCodiceAnalisi As Integer
                        Dim lICurrdtPDC_Campioni As Integer = iCurrdtPDC_Campioni("id_pdc_campione")
                        nuovoCodiceAnalisi = ( _
                            From a In FunzioniGLOBAL.RecodeAnalisi _
                            Where a.FromAnalisi_Testata_Cod = lICurrdtPDC_Campioni _
                            Select a.ToAnalisi_Testata_cod).FirstOrDefault


                        dtPDC_Analisi = leggiPDC_Analisi.Leggi( _
                            lVecchiaTestata _
                            , iCurrdtPDC_Dettagli("id_pdc_Dettagli") _
                            , iCurrdtPDC_Campioni("id_pdc_campione") _
                            , 0 _
                            , "" _
                            , "" _
                            , objOpzioni.objParametri_Server_GIAS_ORIGINE _
                        )



                        For Each iCurrdtPDC_Analisi In dtPDC_Analisi.Rows

                            Dim nuovoCodRisum As Integer
                            nuovoCodRisum = ( _
                                From ris In FunzioniGLOBAL.Contatti _
                                Where ris.From_Cod_RisUm _
                                Select ris.To_Cod_RisUm _
                            ).FirstOrDefault


                            Dim iOldAnalisiTipologia_cod As Integer = iCurrdtPDC_Analisi("Analisi_Tipologia_Cod")
                            Dim iNewAnalisiTipologia_cod As Integer = _
                                ( _
                                    From at In FunzioniGLOBAL.RecodeAnalisiTipologia _
                                    Where at.FROM_AnalisiTipologia_cod = iOldAnalisiTipologia_cod _
                                    Select at.TO_AnalisiTipologia_cod _
                                ).FirstOrDefault

                            scriviPDC_Analisi.Scrivi( _
                                NuovaTestata _
                                , nuovoDettaglio _
                                , nuovoCampione _
                                , nuovoCodiceAnalisi _
                                , iCurrdtPDC_Analisi("PDC_Stato_Analisi") _
                                , nuovoCodRisum _
                                , iNewAnalisiTipologia_cod _
                                , iCurrdtPDC_Analisi("Altre_Molecole") _
                                , iCurrdtPDC_Analisi("Data_Richiesta_Analisi") _
                                , iCurrdtPDC_Analisi("Note_Richiesta_Analisi") _
                                , iCurrdtPDC_Analisi("Piva_FornitoreFatturazione") _
                                , iCurrdtPDC_Analisi("tipo_campione_apofruit") _
                                , iCurrdtPDC_Analisi("mostra_in_stampe") _
                                , objOpzioni.objParametri_Server_GIAS_DESTINAZIONE _
                                , iCurrdtPDC_Analisi("Data_Creazione") _
                                , iCurrdtPDC_Analisi("Data_Modifica") _
                                , iCurrdtPDC_Analisi("Username_Creazione") _
                                , iCurrdtPDC_Analisi("Username_Modifica") _
                            )


                        Next

                    Next

                Next






                dtPDC_CapitolatiCliente_Attivi = leggiPDC_CapitolatiCliente_Attivi.Leggi(lVecchiaTestata, 0, "", "", objOpzioni.objParametri_Server_GIAS_ORIGINE)

                For Each iCurrdtPDC_CapitolatiCliente_Attivi In dtPDC_CapitolatiCliente_Attivi.Rows
                    scriviPDC_CapitolatiCliente_Attivi.Scrivi(NuovaTestata,
                                                              iCurrdtPDC_CapitolatiCliente_Attivi("ID_CApitolatoPrivato"),
                                                              iCurrdtPDC_CapitolatiCliente_Attivi("DEs_Capitolato_Privato"),
                                                              iCurrdtPDC_CapitolatiCliente_Attivi("Sigla_Capitolato_Privato"),
                                                              objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)

                Next

                dtAnalisi_Conformita_Capitolato_Cliente = leggiAnalisi_Conformita_Capitolato_Cliente.Leggi(lVecchiaTestata, 0, objOpzioni.objParametri_Server_GIAS_ORIGINE)
                For Each iCurrdtAnalisi_Conformita_Capitolato_Cliente In dtAnalisi_Conformita_Capitolato_Cliente.Rows
                    scriviAnalisi_Conformita_Capitolato_Cliente.Scrivi( _
                        NuovaTestata _
                        , iCurrdtAnalisi_Conformita_Capitolato_Cliente("CapitolatoCliente_cod") _
                        , iCurrdtAnalisi_Conformita_Capitolato_Cliente("Esito") _
                        , iCurrdtAnalisi_Conformita_Capitolato_Cliente("Descrizione_Esito") _
                        , iCurrdtAnalisi_Conformita_Capitolato_Cliente("Capitolato_Des") _
                        , objOpzioni.objParametri_Server_GIAS_DESTINAZIONE _
                    )

                Next


            Next





        Catch ex As Exception

            Dim msg As String
            msg = CStr(Date.Now) + " - " + nomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)

        End Try


    End Sub

    Public Function GetDistinctRecords(ByVal dt As DataTable, ByVal Columns As String()) As DataTable
        Dim dtUniqRecords As DataTable = New DataTable()
        dtUniqRecords = dt.DefaultView.ToTable(True, Columns)
        Return dtUniqRecords
    End Function
End Class
