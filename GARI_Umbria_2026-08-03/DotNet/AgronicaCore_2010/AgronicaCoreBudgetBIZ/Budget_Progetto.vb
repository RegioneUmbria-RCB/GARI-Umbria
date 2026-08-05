Imports System.Xml
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.metaschema
Imports AgronicaCoreModelsSTD.metaschema.utilizzi
Imports AgronicaCoreAnagrafeBIZ.ChiamateWS
Imports AgronicaCoreModelsSTD.exceptions

Public Class Budget_Progetto_R
    Public Function Leggi_Esercizi_Anagrafica(ByVal Id_Budget As Integer,
                                              ByVal Piva As String,
                                              ByVal Sa_Cod As Integer,
                                              ByVal Appezza As Integer,
                                              ByVal Id_Reg As Integer,
                                              ByVal specie As Specie,
                                              ByVal data As Date,
                                              ByVal filtroData As Boolean,
                                              ByRef objParametri_Super_Server As AgronicaCoreParametri,
                                               ByRef objParametri_Server As AgronicaCoreParametri,
                                               ByRef objParametri_Utenti As AgronicaCoreParametri
                                              ) As List(Of AgronicaCoreModelsSTD.anagrafiche.Esercizio)
        Dim Esercizi As New List(Of AgronicaCoreModelsSTD.anagrafiche.Esercizio)

        If Id_Budget <> 0 AndAlso Piva <> "" AndAlso Sa_Cod <> 0 AndAlso Appezza <> 0 AndAlso Id_Reg <> 0 Then
            Dim objProgetto As New AgronicaCoreBudgetDAL.Budget_Impresa_Progetti_R

            Dim objImpianti As New AgronicaCoreBudgetDAL.Budget_Reg_Impianti_Read
            Dim appInizio As Date = objParametri_Server.FinestraTemporaleInizio
            Dim appFine As Date = objParametri_Server.FinestraTemporaleFine

            If filtroData Then
                objParametri_Server.FinestraTemporaleInizio = data
                objParametri_Server.FinestraTemporaleFine = data
            Else
                objParametri_Server.FinestraTemporaleInizio = CDate(AGRODATAINIZIO)
                objParametri_Server.FinestraTemporaleFine = CDate(AGRODATAFINE)
            End If


            Dim Dt As DataTable
            Dt = objProgetto.Leggi(CInt(Id_Budget),
                                   CStr(Piva),
                                    0,
                                    CInt(9100),
                                    0,
                                    CInt(Sa_Cod),
                                    CInt(Appezza),
                                    CInt(Id_Reg),
                                    0, 0,
                                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                                    "",
                                    " Budget_Imprese_Progetti.Validita_Inizio ASC ",
                                    objParametri_Server)

            objParametri_Server.FinestraTemporaleInizio = CDate(appInizio)
            objParametri_Server.FinestraTemporaleFine = CDate(appFine)

            For Each row In Dt.Rows

                Dim Esercizio = Leggi_Esercizio_Anagrafica(specie, row("Id_Budget"), row("Progetto_Cod"), objParametri_Super_Server, objParametri_Server, objParametri_Utenti)
                Esercizi.Add(Esercizio)

            Next

        End If

        Return Esercizi
    End Function

    Public Function Leggi_Esercizio_Anagrafica(Specie As Specie,
                                               ByVal Id_Budget As Integer,
                                               ByVal Progetto_Cod As Integer,
                                               ByRef objParametri_Super_Server As AgronicaCoreParametri,
                                               ByRef objParametri_Server As AgronicaCoreParametri,
                                               ByRef objParametri_Utenti As AgronicaCoreParametri) As AgronicaCoreModelsSTD.anagrafiche.Esercizio

        Dim esercizio As New AgronicaCoreModelsSTD.anagrafiche.Esercizio(Progetto_Cod, "")
        Dim objProgetto As New AgronicaCoreBudgetDAL.Budget_Impresa_Progetti_R
        Dim Dt As DataTable
        Dt = objProgetto.Leggi(Id_Budget, "", Progetto_Cod, CInt(9100), 0, 0, 0, 0, 0, 0,
                                enumSelezioneVariabile.Selezione_TabellaCompleta,
                                "",
                                " Budget_Imprese_Progetti.Validita_Inizio DESC ",
                                objParametri_Server)

        esercizio.impiantoPK = New AgronicaCoreModelsSTD.anagrafiche.Impianto.PK(
            Dt.Rows(0).Item("id_reg"),
            New AgronicaCoreModelsSTD.anagrafiche.Appezzamento.PK(
                Dt.Rows(0).Item("appezza"),
                New AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK(
                    Dt.Rows(0).Item("sa_cod"),
                    Dt.Rows(0).Item("piva")
                )
            )
        )

        Dim objRegImpianti As New AgronicaCoreBudgetDAL.Budget_Reg_Impianti_Read
        Dim sup_imp = objRegImpianti.LeggiSuperficie(Dt.Rows(0).Item("Id_Budget"), Dt.Rows(0).Item("piva"), Dt.Rows(0).Item("sa_cod"), Dt.Rows(0).Item("appezza"), Dt.Rows(0).Item("id_reg"), objParametri_Server)






        esercizio.validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(Dt.Rows(0)("Validita_Inizio"), Dt.Rows(0)("Validita_Fine"))
        esercizio.lotto = Dt.Rows(0)("Progetto_Nome")
        esercizio.descrizione = Dt.Rows(0)("Progetto_Des")

        Dim objRegolamento As New AgronicaCoreMetaSchemaDAL.Regolamenti_R
        Dim Regolamento_Des = ""
        If Dt.Rows(0).Item("Regolamento_Cod") <> 0 Then
            Dim dtReg = objRegolamento.Leggi(Dt.Rows(0).Item("Regolamento_Cod"), enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
            If dtReg.Rows.Count > 0 Then
                Regolamento_Des = dtReg.Rows(0)("Reg_DES")
            End If
        End If
        esercizio.regolamento = New AgronicaCoreModelsSTD.metaschema.Regolamenti(Dt.Rows(0).Item("Regolamento_Cod")) With {.descrizione = Regolamento_Des}
        esercizio.disciplinare = New AgronicaCoreModelsSTD.metaschema.Disciplinare(Dt.Rows(0).Item("Disciplinare_cod")) With {
            .disciplinarePubblicoPrivato = Dt.Rows(0).Item("disciplinare_pubblicoprivato"),
            .regolamentoConcimazione = New RegolamentoConcimazione(0),
            .raggruppamentiColturaliDPI = New RaggruppamentiColturaliDPI(0)
        }
        If Specie IsNot Nothing Then
            esercizio.disciplinare = Leggi_Disciplinare_Completo(Specie,
                                                                 esercizio.regolamento,
                                                                 Dt.Rows(0).Item("Disciplinare_cod"),
                                                                 Dt.Rows(0).Item("disciplinare_pubblicoprivato"),
                                                                 objParametri_Super_Server,
                                                                 objParametri_Server,
                                                                 objParametri_Utenti)
        End If

        Dim apportoMacroElementi As New ApportoMacroelementi

        apportoMacroElementi.pianoConcimazione = New RegolamentoConcimazione(Dt.Rows(0).Item("Regolamento_Concimazioni_Cod"))
        apportoMacroElementi.fase = New FaseCicloColturale(Dt.Rows(0).Item("Stato_Impianto")) With {.descrizione = Dt.Rows(0).Item("Fase_Des")}
        apportoMacroElementi.tipologia = New FinalitaPianoConcimazione(Dt.Rows(0).Item("Tipologia_Cod")) With {.descrizione = Dt.Rows(0).Item("Tipologia_Des")}

        If esercizio.regolamento.codice = 4 Then
            'Regolamento Bio --> No Disciplinare
            esercizio.vincolo = New Vincolo(esercizio.regolamento.codice) With {
                .descrizione = esercizio.regolamento.descrizione,
                .regolamento = esercizio.regolamento,
                .disciplinare = New Disciplinare("") With {
                    .descrizione = "",
                    .disciplinarePubblicoPrivato = 1,
                    .idTr = 3,
                    .regolamentoConcimazione = New RegolamentoConcimazione(0)
                }
            }
        Else
            Dim disciplinareDescrizione = "Nessuno"
            If esercizio.disciplinare.descrizione <> "" Then
                disciplinareDescrizione = esercizio.disciplinare.descrizione
            End If
            Dim disciplinareCodice As String = "1"
            If esercizio.disciplinare.codice <> "0" Then
                If esercizio.disciplinare.codice.Split("/").Length = 4 Then
                    disciplinareCodice = esercizio.disciplinare.codice.Split("/")(1) &
                        "_" & esercizio.disciplinare.codice.Split("/")(0)
                Else
                    disciplinareCodice = esercizio.disciplinare.codice
                End If
            End If
            Dim regolamento As Regolamenti = New Regolamenti(1) With {.descrizione = "Nessuno"}
            If esercizio.regolamento.codice <> 0 Then
                regolamento = esercizio.regolamento
            End If
            'Disciplinare Selezionato
            esercizio.vincolo = New Vincolo(disciplinareCodice) With {
                .descrizione = disciplinareDescrizione,
                .regolamento = regolamento,
                .disciplinare = esercizio.disciplinare
            }
        End If

        If Not IsDBNull(Dt.Rows(0).Item("p_ha")) Then
            esercizio.piante_Ha = Dt.Rows(0).Item("p_ha")
            esercizio.piante_Impianto = Math.Round(Dt.Rows(0).Item("p_ha") * sup_imp)
        End If
        'rowNew("PianteHa") = Dt.Rows(i).Item("PianteHa")
        esercizio.data_Fioritura_Prevista = CDate(Dt.Rows(0).Item("data_fioritura_prevista"))
        esercizio.data_Raccolta_Prevista = CDate(Dt.Rows(0).Item("data_fine_prevista"))
        esercizio.data_Semina_Trapianto_Prevista = CDate(Dt.Rows(0).Item("data_inizio_prevista"))
        esercizio.id_tr = 3
        esercizio.resa_prevista = Dt.Rows(0).Item("produzione_prevista")

        If Not IsDBNull(Dt.Rows(0).Item("P_HA_Femmine")) Then
            esercizio.piante_Ha_Femmine = Dt.Rows(0).Item("P_HA_Femmine")
            esercizio.Piante_Ha_Impianto_Femmine = Math.Round(Dt.Rows(0).Item("P_HA_Femmine") * sup_imp)
        End If

        If Not IsDBNull(Dt.Rows(0).Item("P_HA_Maschi")) Then
            esercizio.Piante_Ha_Maschi = Dt.Rows(0).Item("P_HA_Maschi")
            esercizio.Piante_Ha_Impianto_Maschi = Math.Round(Dt.Rows(0).Item("P_HA_Maschi") * sup_imp)
        End If

        esercizio.prodotto = New AgronicaCoreModelsSTD.attivita.risorse.Prodotto(0, 210) With {.descrizione = ""}
        If Not IsDBNull(Dt.Rows(0)("Mat_Cod")) AndAlso CInt(Dt.Rows(0)("Mat_Cod")) > 0 Then
            Dim objMateriePrime As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
            Dim dtProdotto = objMateriePrime.Leggi(Dt.Rows(0)("Piva"), 0, 210, CInt(Dt.Rows(0)("Mat_Cod")), "", 0, 0, 0, 0, 0, 0, 0, "", 0, "", True, False, "", enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
            If dtProdotto.Rows.Count > 0 Then
                Dim prodotto_des = dtProdotto.Rows(0)("Mat_Des")
                If Not IsDBNull(dtProdotto.Rows(0)("Cod_Articolo")) AndAlso CStr(dtProdotto.Rows(0)("Cod_Articolo")) <> "" Then
                    prodotto_des &= " - " & CStr(dtProdotto.Rows(0)("Cod_Articolo"))
                End If
                esercizio.prodotto = New AgronicaCoreModelsSTD.attivita.risorse.Prodotto(Dt.Rows(0)("Mat_Cod"), 210) With {.descrizione = prodotto_des}
            End If
        End If


        ''
        ''  CODICI PROGETTO
        ''
        Dim objCodici = New AgronicaCoreBudgetDAL.Budget_Reg_Impianti_Codici_R
        Dim DtCodici = objCodici.LeggixProgetto(Id_Budget, "", 0, 0, 0, "", Progetto_Cod, 0, "",
                                                AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "",
                                                objParametri_Server)

        Dim OrganismoReferente = ""

        Dim StrCodiciImpianto As String
        Dim objcodAn As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R
        StrCodiciImpianto = objcodAn.Filtro_Codici_Anagrafe(4, 3, 2, objParametri_Server)
        'Elimino il codice Titolo Possesso, Metodo Produzione, Magazzino Conferimento,
        'Organismo Referente, Capitolato Privato e Dettaglio Specie Personalizzato 
        'perchè già presenti
        StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " & CStr(enum_CodiciAnagrafe.TitoloPossesso), "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " & CStr(enum_CodiciAnagrafe.TitoloPossesso) & " Or ", "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " & CStr(enum_CodiciAnagrafe.MetodoDiProduzione), "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " & CStr(enum_CodiciAnagrafe.MetodoDiProduzione) & " Or ", "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " & CStr(enum_CodiciAnagrafe.Riferimento_Trasferimento_Dati), "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " & CStr(enum_CodiciAnagrafe.Riferimento_Trasferimento_Dati) & " Or ", "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " & CStr(enum_CodiciAnagrafe.Organismo_Referente), "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " & CStr(enum_CodiciAnagrafe.Organismo_Referente) & " Or ", "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " & CStr(enum_CodiciAnagrafe.Capitolato_Privato), "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " & CStr(enum_CodiciAnagrafe.Capitolato_Privato) & " Or ", "")
        ' - - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - -
        StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " & CStr(enum_CodiciAnagrafe.Zespri_Fasi_Fase), "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " & CStr(enum_CodiciAnagrafe.Zespri_Fasi_Fase) & " Or ", "")
        ' - - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - -
        StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " & CStr(enum_CodiciAnagrafe.Dettaglio_Specie_Personalizzato), "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " & CStr(enum_CodiciAnagrafe.Dettaglio_Specie_Personalizzato) & " Or ", "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " & CStr(enum_CodiciAnagrafe.Magazzino_Conferimento), "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " & CStr(enum_CodiciAnagrafe.Magazzino_Conferimento) & " Or ", "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " & CStr(enum_CodiciAnagrafe.Data_Inizio_Portinnesto), "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " & CStr(enum_CodiciAnagrafe.Data_Inizio_Portinnesto) & " Or ", "")



        Dim Codici As New List(Of CodiciAnagrafeValori)

        'StrCodiciImpianto = StrCodiciImpianto & ", 1287, 1288"
        If DtCodici IsNot Nothing AndAlso DtCodici.Rows.Count > 0 Then

            For Each rowCodice In DtCodici.Rows
                Dim id_cod As Integer = rowCodice("ID_Cod")
                Dim val_cod As String = ""
                If Not IsDBNull(rowCodice("Val_Cod")) Then
                    val_cod = rowCodice("Val_Cod")
                End If
                Dim validita As New IntervalloTemporale(rowCodice("Validita_Inizio1"),
                                                        rowCodice("Validita_Fine1"))

                Select Case id_cod

                    Case enum_CodiciAnagrafe.Finalita_Concimazione_Impianto
                        'If val_cod <> "" Then
                        '    apportoMacroElementi.tipologia = New FinalitaPianoConcimazione(val_cod)
                        'End If
                    Case enum_CodiciAnagrafe.Impianto_LimiteN
                        If val_cod <> "" Then
                            apportoMacroElementi.n = val_cod
                        End If
                    Case enum_CodiciAnagrafe.Impianto_LimiteP
                        If val_cod <> "" Then
                            apportoMacroElementi.p2o5 = val_cod
                        End If
                    Case enum_CodiciAnagrafe.Impianto_LimiteK
                        If val_cod <> "" Then
                            apportoMacroElementi.k2o = val_cod
                        End If
                    Case enum_CodiciAnagrafe.Impianto_LimiteMg
                        If val_cod <> "" Then
                            apportoMacroElementi.mgo = val_cod
                        End If
                    Case enum_CodiciAnagrafe.Riferimento_Trasferimento_Dati
                        If val_cod <> "" Then
                            Dim objRif As New AgronicaCoreAnagrafeDAL.RiferimentoTrasferimentoDati_R
                            Dt = objRif.Leggi(True, val_cod, val_cod, "", "", objParametri_Server)

                            If Dt.Rows.Count > 0 Then
                                esercizio.riferimento_Trasferimento_Dati = New AgronicaCoreModelsSTD.anagrafiche.Contatto()
                                esercizio.riferimento_Trasferimento_Dati.primaryKey = New AgronicaCoreModelsSTD.anagrafiche.Contatto.PK(val_cod, "")
                                esercizio.riferimento_Trasferimento_Dati.primaryKey.partitaIva = ""
                                esercizio.riferimento_Trasferimento_Dati.primaryKey.codice = val_cod
                                esercizio.riferimento_Trasferimento_Dati.ragione_Sociale = Dt.Rows(0)("ragsoc_padre")
                            End If
                        End If
                        'esercizio.riferimento_Trasferimento_Dati = val_cod

                    Case enum_CodiciAnagrafe.Organismo_Referente
                        If val_cod <> "" Then
                            Dim objOrganismoRef As New AgronicaCoreAnagrafeDAL.OrganismoReferente_Read
                            Dim orgRef = objOrganismoRef.OrganismoReferente_from_Piva(val_cod, objParametri_Server)
                            If orgRef <> "" Then
                                esercizio.organismo_Referente = New AgronicaCoreModelsSTD.anagrafiche.Contatto()
                                esercizio.organismo_Referente.primaryKey = New AgronicaCoreModelsSTD.anagrafiche.Contatto.PK(val_cod, "")
                                esercizio.organismo_Referente.primaryKey.partitaIva = ""
                                esercizio.organismo_Referente.primaryKey.codice = val_cod
                                esercizio.organismo_Referente.ragione_Sociale = orgRef
                            End If
                        End If
                    Case enum_CodiciAnagrafe.Magazzino_Conferimento

                        If val_cod <> "" Then
                            'Dim ValCodModificato As Boolean

                            'Dim objFabbricati As New AgronicaCoreBudgetDAL.Budget_Fabbricati_R
                            'Dim FiltroAggiuntivoMag As String = " (Budget_Fabbricati.Tipo_Fabbricato_Cod = 20 OR Budget_Fabbricati.Tipo_Fabbricato_Cod = 50 OR Budget_Fabbricati.Tipo_Fabbricato_Cod = 120 OR Budget_Fabbricati.Tipo_Fabbricato_Cod = 121 OR Budget_Fabbricati.Tipo_Fabbricato_Cod = 122 OR Budget_Fabbricati.Tipo_Fabbricato_Cod = 123) "

                            'Dim Dt_Mag = objFabbricati.Leggi_Magazzini_Organismoreferente(Id_Budget, "",
                            '                                        0,
                            '                                        0,
                            '                                        0,
                            '                                        FiltroAggiuntivoMag,
                            '                                        "",
                            '                                        objParametri_Server)
                            Dim objFabbricati As New AgronicaCoreAnagrafeDAL.Fabbricati_R
                            Dim FiltroAggiuntivoMag As String = " (Fabbricati.Tipo_Fabbricato_Cod = 20 OR Fabbricati.Tipo_Fabbricato_Cod = 50 OR Fabbricati.Tipo_Fabbricato_Cod = 120 OR Fabbricati.Tipo_Fabbricato_Cod = 121 OR Fabbricati.Tipo_Fabbricato_Cod = 122 OR Fabbricati.Tipo_Fabbricato_Cod = 123) "

                            Dim Dt_Mag = objFabbricati.Leggi_Magazzini_Organismoreferente("",
                                                                    0,
                                                                    0,
                                                                    0,
                                                                    FiltroAggiuntivoMag,
                                                                    "",
                                                                    objParametri_Server)
                            Dim mag_arr = val_cod.Split("|")

                            Dim expression As String = ""
                            If mag_arr.Length > 2 Then
                                expression = "Fabbricato_Cod = " & mag_arr(0) & " And sa_cod = " & mag_arr(1) & " And PIVA = '" & mag_arr(2) & "' "
                            End If
                            Dim foundRows As DataRow()

                            ' Use the Select method to find all rows matching the filter.
                            foundRows = Dt_Mag.Select(expression)

                            'val_cod = Controlla_ValCod_MagazzinoConferimento(val_cod, OrganismoReferente, ValCodModificato)
                            If mag_arr.Length = 3 Then
                                Dim pivaMag As String = mag_arr(2)
                                Dim sa_codMag As Integer = mag_arr(1)
                                Dim fabbricato_CodMag As Integer = mag_arr(0)
                                Dim rag_soc_mag = ""
                                If foundRows.Length > 0 Then
                                    rag_soc_mag = foundRows(0).Item("fabbricato_des") & " (" & foundRows(0).Item("rag_soc") & ")"
                                End If

                                esercizio.magazzino_Conferimento = New Fabbricato() With {
                                    .primaryKey = New Fabbricato.PK() With {
                                        .codice = fabbricato_CodMag,
                                        .centroAziendalePK = New CentroAziendale.PK(sa_codMag, pivaMag)
                                    },
                                    .descrizione = rag_soc_mag
                                }
                            End If
                        End If
                    Case enum_CodiciAnagrafe.Capitolato_Privato
                        If val_cod <> "" Then
                            Dim objCAC As New AgronicaCoreAnagrafeDAL.CAC_Codifica_InfoAggiuntive_R
                            Dt = objCAC.Leggi(1, "", 0, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "InfoAgg_Cod like '%" & val_cod & "%'", "", objParametri_Server)
                            If Dt.Rows.Count > 0 Then
                                esercizio.capitolato_Privato = New AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr(val_cod, Dt.Rows(0)("InfoAgg_Des"))
                            End If
                        End If
                    Case enum_CodiciAnagrafe.Codice_Residuo
                        If val_cod <> "" Then
                            Dim objCAC As New AgronicaCoreAnagrafeDAL.CAC_Codifica_InfoAggiuntive_R
                            Dt = objCAC.Leggi(enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod.Residuo, val_cod, 0, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                            If Dt.Rows.Count > 0 Then
                                esercizio.residuo = New AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr(val_cod, Dt.Rows(0)("InfoAgg_Des"))
                            End If
                        End If
                    Case enum_CodiciAnagrafe.Codice_Certificazione
                        If val_cod <> "" Then
                            Dim objCAC As New AgronicaCoreAnagrafeDAL.CertificazioniAziendali_R
                            Dim list As List(Of AgronicaCoreModelsSTD.baseClass.BaseCodeDescr) = New List(Of AgronicaCoreModelsSTD.baseClass.BaseCodeDescr)
                            Dim listaCodici As List(Of Integer) = New List(Of Integer)
                            For Each codice In val_cod.ToString.Split("|".ToCharArray)
                                listaCodici.Add(CInt(codice))
                            Next
                            Dt = objCAC.Leggi(listaCodici, "", AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server)
                            For Each codice In Dt.Rows
                                list.Add(New AgronicaCoreModelsSTD.baseClass.BaseCodeDescr(code:=CInt(codice.Item("CA_Cod")), descr:=codice.Item("CA_Des")))
                            Next
                            esercizio.certificazioneAziendale = list
                        End If
                    Case enum_CodiciAnagrafe.Contributi
                        If val_cod <> "" Then
                            Dim objCAC As New AgronicaCoreMetaSchemaDAL.Contributi
                            Dim list As List(Of AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr) = New List(Of AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr)
                            Dim listaCodici As List(Of Integer) = New List(Of Integer)
                            For Each codice In val_cod.ToString.Split("|".ToCharArray)
                                listaCodici.Add(CInt(codice))
                            Next
                            Dt = objCAC.LeggiContributi(AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server, listaCodici)
                            For Each codice In Dt.Rows
                                list.Add(New AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr(code:=CInt(codice.Item("Contributo_Cod")), description:=codice.Item("Contributo_Des")))
                            Next
                            esercizio.contributi = list
                        End If
                    Case enum_CodiciAnagrafe.Tecnico
                        If val_cod <> "" Then
                            Dim objContattiR As New AgronicaCoreAnagrafeDAL.Contatti_R
                            Dim list As List(Of AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr) = New List(Of AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr)
                            Dim listaCodici As List(Of String) = New List(Of String)
                            For Each codice In val_cod.ToString.Split("|".ToCharArray)
                                listaCodici.Add(codice)
                            Next
                            Dt = objContattiR.Contatti_Contatto_Leggi(CStr(esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva),
                                                  "",
                                                  0,
                                                  -6,
                                                  True,
                                                  False,
                                                  0,
                                                  0,
                                                  False,
                                                  0,
                                                  0,
                                                  0,
                                                  "", True, 0, 0, 0, 0, 0,
                                                  enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                  "", "", objParametri_Server, Lista_Cod_Contatto:=listaCodici)

                            For Each codice In Dt.Rows
                                list.Add(New AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr(code:=codice.Item("Cod_Contatto"), description:=codice.Item("Cognome") & " " & codice.Item("Nome")))
                            Next
                            esercizio.tecnico = list
                        End If
                    Case enum_CodiciAnagrafe.Codice_Certificazione_Prodotto
                        If val_cod <> "" Then
                            Dim objCAC As New AgronicaCoreAnagrafeDAL.CAC_Codifica_InfoAggiuntive_R
                            Dt = objCAC.Leggi(enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod.Certificazione_Prodotto, val_cod, 0, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                            If Dt.Rows.Count > 0 Then
                                esercizio.certificazioneProdotto = New AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr(val_cod, Dt.Rows(0)("InfoAgg_Des"))
                            End If
                        End If
                    Case enum_CodiciAnagrafe.Zespri_Fasi_Fase
                        If val_cod <> "" Then
                            Dim objOTab As New AgronicaCoreAnagrafeDAL.OTabelle_R
                            Dt = objOTab.LeggiXDescrizione(enum_CodiciAnagrafe.Zespri_Fasi_Fase, val_cod, objParametri_Server)
                            If Dt.Rows.Count > 0 Then
                                esercizio.licenza_Coltivazione = New AgronicaCoreModelsSTD.metaschema.LicenzaColtivazione(val_cod)
                                esercizio.licenza_Coltivazione.descrizione = Dt.Rows(0)("Descrizione")
                            End If
                        End If
                    Case enum_CodiciAnagrafe.Impianto_IAF_ImpegniAggiuntiviFacoltativi
                        If val_cod <> "" Then
                            esercizio.iaf = Leggi_IAF_Completi(val_cod,
                                                               esercizio.disciplinare,
                                                               Specie,
                                                               objParametri_Super_Server,
                                                               objParametri_Server,
                                                               objParametri_Utenti)
                        End If
                    Case enum_CodiciAnagrafe.Impianto_PianoSemina
                        If val_cod <> "" Then
                            Dim objCAC As New AgronicaCoreAnagrafeDAL.CAC_Codifica_InfoAggiuntive_R
                            Dt = objCAC.Leggi(5, val_cod, 0, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                            If Dt.Rows.Count > 0 Then
                                esercizio.piano_Semina = New AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr(val_cod, Dt.Rows(0)("InfoAgg_Des"))
                                esercizio.piano_Semina.descrizione = Dt.Rows(0)("InfoAgg_Cod") & " " & Dt.Rows(0)("InfoAgg_Des")
                            End If
                        End If

                    Case enum_CodiciAnagrafe.Lavorazione
                        If val_cod <> "" Then
                            Dim objCAC As New AgronicaCoreAnagrafeDAL.CAC_Codifica_InfoAggiuntive_R
                            Dt = objCAC.Leggi(enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod.Lavorazione, val_cod, 0, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                            If Dt.Rows.Count > 0 Then
                                esercizio.lavorazione = New AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr(val_cod, Dt.Rows(0)("InfoAgg_Des"))
                            End If
                        End If

                    Case enum_CodiciAnagrafe.Modalita_Liquidazione
                        If val_cod <> "" Then
                            Dim objCAC As New AgronicaCoreAnagrafeDAL.CAC_Codifica_InfoAggiuntive_R
                            Dt = objCAC.Leggi(enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod.ModalitaLiquidazione, val_cod, 0, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                            If Dt.Rows.Count > 0 Then
                                esercizio.modalita_liquidazione = New AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr(val_cod, Dt.Rows(0)("InfoAgg_Des"))
                            End If
                        End If

                    Case enum_CodiciAnagrafe.Origine_Prodotto
                        If val_cod <> "" Then
                            Dim objCAC As New AgronicaCoreAnagrafeDAL.CAC_Codifica_InfoAggiuntive_R
                            Dt = objCAC.Leggi(enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod.OrigineProdotto, val_cod, 0, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                            If Dt.Rows.Count > 0 Then
                                esercizio.origine_prodotto = New AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr(val_cod, Dt.Rows(0)("InfoAgg_Des"))
                            End If
                        End If

                    Case enum_CodiciAnagrafe.Specifica
                        If val_cod <> "" Then
                            Dim objCAC As New AgronicaCoreAnagrafeDAL.CAC_Codifica_InfoAggiuntive_R
                            Dt = objCAC.Leggi(enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod.Specifica, val_cod, 0, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                            If Dt.Rows.Count > 0 Then
                                esercizio.specifica = New AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr(val_cod, Dt.Rows(0)("InfoAgg_Des"))
                            End If
                        End If

                    Case enum_CodiciAnagrafe.Distinta_Chiusa
                        If val_cod <> "" Then
                            esercizio.esercizio_Chiuso = val_cod
                        End If
                    Case enum_CodiciAnagrafe.Codice_Impianto_Ribaltato

                        'rowNew("distinta_replica_codice") = val_cod
                        'rowNew("distinta_replica") = If(String.IsNullOrEmpty(val_cod), "0", "1")

                    Case Else

                        If InStr(StrCodiciImpianto, id_cod) <> 0 Then
                            'Dim codice As New AnagrafeNG.Codici

                            Dim objCodiceAnagrafe As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R
                            Dim CodiceAnagrafeDes = objCodiceAnagrafe.CodiceAnagrafeDes_from_CodiceAnagrafeCod(
                                                                    CInt(id_cod), objParametri_Server)

                            Dim codice = New CodiciAnagrafeValori() With {
                                .valore = val_cod,
                                .validita = validita,
                                .codiceAnagrafe = New CodiceAnagrafe(id_cod) With {
                                    .descrizione = CodiceAnagrafeDes
                                }
                            }

                            Codici.Add(codice)

                        End If
                End Select
            Next

        End If

        esercizio.codici = Codici

        esercizio.apportiMassimiMacroelementi = apportoMacroElementi
        esercizio.catastoEsercizio = Leggi_Particelle_Progetto(Progetto_Cod, objParametri_Server)


        Return esercizio
    End Function

    Public Function Leggi_Disciplinare_Completo(specie As Specie,
                                                regolamento As Regolamenti,
                                                disciplinare_cod As Integer,
                                                disciplinarePubblico_Privato As Integer,
                                                objParametri_Super_Server As AgronicaCoreParametri,
                                                objParametri_Server As AgronicaCoreParametri,
                                                objParametri_Utenti As AgronicaCoreParametri) As Disciplinare
        Dim disciplinare As New Disciplinare(CStr(disciplinare_cod)) With {
            .descrizione = "",
            .regolamentoConcimazione = New RegolamentoConcimazione(0),
            .raggruppamentiColturaliDPI = New RaggruppamentiColturaliDPI(0)
        }

        If disciplinare_cod = -2 Then
            Return New Disciplinare(CStr(disciplinare_cod)) With {
            .descrizione = "Nessuno",
            .regolamentoConcimazione = New RegolamentoConcimazione(0),
            .raggruppamentiColturaliDPI = New RaggruppamentiColturaliDPI(0)
        }
        End If

        If disciplinare_cod <> 0 Then
            Dim leggiDpi As New AgronicaCoreDpiDAL.Dpi_R
            Dim dt = leggiDpi.Leggi_DPI_Regolamenti(-1, disciplinare_cod, 0, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server)
            'Dim dt = AgronicaCoreWebService.Disciplinari_WS.Disciplinari_Elenco_TuttigliElemInChiave(objParametri_Super_Server,
            '                                                            objParametri_Server,
            '                                                            objParametri_Utenti,
            '                                                            0,
            '                                                            specie.codice, 0, disciplinarePubblico_Privato, True, False, True)

            If dt.Rows.Count > 0 Then
                disciplinare.codice = CStr(disciplinare_cod) & "/" & dt.Rows(0)("Flag_Privato_Pubblico") & "/" & dt.Rows(0)("PUA_Regolamento_Cod") & "/" & dt.Rows(0)("ID_TR")
                disciplinare.descrizione = dt.Rows(0)("NomeEsteso")
                disciplinare.disciplinarePubblicoPrivato = dt.Rows(0)("Flag_Privato_Pubblico")
                disciplinare.idTr = dt.Rows(0)("ID_TR")
                disciplinare.regolamentoConcimazione = New RegolamentoConcimazione(dt.Rows(0)("PUA_Regolamento_Cod"))
            End If
        End If


        Return disciplinare
    End Function

    Public Function Leggi_Disciplinare_Completo_OLD(specie As Specie,
                                                regolamento As Regolamenti,
                                                disciplinare_cod As Integer,
                                                objParametri_Super_Server As AgronicaCoreParametri,
                                                objParametri_Server As AgronicaCoreParametri,
                                                objParametri_Utenti As AgronicaCoreParametri) As Disciplinare
        Dim disciplinare As New Disciplinare(CStr(disciplinare_cod)) With {
            .descrizione = "",
            .regolamentoConcimazione = New RegolamentoConcimazione(0),
            .raggruppamentiColturaliDPI = New RaggruppamentiColturaliDPI(0)
        }

        If disciplinare_cod = -2 Then
            Return New Disciplinare(CStr(disciplinare_cod)) With {
            .descrizione = "Nessuno",
            .regolamentoConcimazione = New RegolamentoConcimazione(0),
            .raggruppamentiColturaliDPI = New RaggruppamentiColturaliDPI(0)
        }
        End If

        If disciplinare_cod <> 0 Then
            Dim dt = AgronicaCoreWebService.Disciplinari_WS.Disciplinari_Elenco_TuttigliElemInChiave(objParametri_Super_Server,
                                                                        objParametri_Server,
                                                                        objParametri_Utenti,
                                                                        0,
                                                                        specie.codice, 0, 1, True, False, True)

            If dt.Rows.Count > 0 Then
                Dim filteredDT = dt.Select(" codRegolamento =  '" & disciplinare_cod & "' ").CopyToDataTable
                If filteredDT.Rows.Count > 0 Then
                    disciplinare.codice = CStr(disciplinare_cod)
                    disciplinare.descrizione = filteredDT.Rows(0)("nomeEsteso")
                    disciplinare.disciplinarePubblicoPrivato = filteredDT.Rows(0)("PubblicoPrivato")
                    disciplinare.idTr = filteredDT.Rows(0)("id_tr")
                    disciplinare.regolamentoConcimazione = New RegolamentoConcimazione(filteredDT.Rows(0)("PUA_Regolamento_Cod"))
                End If
            End If
        End If


        Return disciplinare
    End Function

    Public Function Leggi_IAF_Completi(listIaf As String,
                                       disciplinare As Disciplinare,
                                       specie As Specie,
                                       objParametri_Super_Server As AgronicaCoreParametri,
                                       objParametri_Server As AgronicaCoreParametri,
                                       objParametri_Utenti As AgronicaCoreParametri) As List(Of ImpegniAggiuntiviFacoltativi)
        Dim list As New List(Of ImpegniAggiuntiviFacoltativi)
        Dim listIafInt As New List(Of Integer)
        Dim arrIafStr = listIaf.Split("|")
        For Each el In arrIafStr
            If IsNumeric(el) Then
                listIafInt.Add(CInt(el))
            End If
        Next

        If listIafInt.Count > 0 Then

            Dim dt = AgronicaCoreAnagrafeBIZ.ChiamateWS.IAF_Elenco(objParametri_Super_Server, objParametri_Server, objParametri_Utenti, disciplinare.codice, specie.codice, True)

            For Each iaf_cod In listIafInt

                Dim dr = dt.Select(" IAF_Cod = " & iaf_cod)

                If dr.Length > 0 Then
                    list.Add(New ImpegniAggiuntiviFacoltativi(dr(0)("IAF_Cod")) With {.descrizione = dr(0)("IAF_Descrizione")})
                End If

            Next

        End If

        Return list
    End Function

    Public Function Leggi_Particelle_Progetto(Progetto_Cod As Integer, ByRef objParametri_Server As AgronicaCoreParametri) As List(Of CatastoEsercizio)
        Dim particelleList As New List(Of CatastoEsercizio)
        Dim objParticelle As New AgronicaCoreAnagrafeDAL.ProgettixParticelle_R

        Dim DtParticelle = objParticelle.LeggixProgetto("", 0, 0, 0, Progetto_Cod,
                                                0, "", AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                "", "", objParametri_Server)

        For Each partRow In DtParticelle.Rows


            Dim id_Cod = partRow("id_Cod")
            Dim val_Cod = partRow("val_cod")

            Dim prov = partRow("Prov")
            Dim provincia = partRow("Provincia")
            Dim com = partRow("Com")
            Dim comune = partRow("comune")
            Dim sezione = ""
            If partRow("sezione") <> "0" Then
                sezione = partRow("sezione")
            End If
            Dim foglio = partRow("Foglio")
            Dim numero = partRow("Numero")
            Dim subalterno = ""
            If partRow("subalterno") <> "0" Then
                subalterno = partRow("subalterno")
            End If

            Dim testo = "(" & prov & ") " & provincia & " - (" & com & ") " & comune & ":" & sezione & ":" & foglio & ":" & numero & ":" & subalterno
            Dim valore = prov & "_" & com & "_" & sezione & "_" & foglio & "_" & numero & "_" & subalterno

            Dim objCodiceAnagrafe As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R
            Dim CodiceAnagrafeDes = objCodiceAnagrafe.CodiceAnagrafeDes_from_CodiceAnagrafeCod(
                                                                    CInt(id_Cod), objParametri_Server)

            Dim particella_progetto = New ParticelleCatastali(New ParticelleCatastali.PK(prov, com, sezione, foglio, numero, subalterno))
            Dim codiceAnagrafe As New CodiciAnagrafeValori()

            codiceAnagrafe.codiceAnagrafe = New CodiceAnagrafe(id_Cod) With {.descrizione = CodiceAnagrafeDes}
            codiceAnagrafe.valore = valore

            Dim catastoEsercizio = New CatastoEsercizio(particella_progetto, codiceAnagrafe)

            catastoEsercizio.particella = particella_progetto

            catastoEsercizio.codice = codiceAnagrafe

            particelleList.Add(catastoEsercizio)

        Next

        Return particelleList
    End Function

End Class

Public Class Budget_Progetto_W

    Public Function Verifica_ValiditaInizioFine(ByRef Id_Budget As Integer,
                                                ByRef esercizio As AgronicaCoreModelsSTD.anagrafiche.Esercizio,
                                                ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim MessaggioErrore As String = ""

        Dim Validita_Fine = esercizio.validita.fine
        Dim Validita_Inizio = esercizio.validita.inizio

        'Dim idbudget = Id_Budget
        Dim sa_cod = esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice 'PrimaryKey.esercizio.primaryKey.appezzamentoPK.centroAziendalePK.codice
        Dim piva = esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva
        Dim appezza = esercizio.impiantoPK.appezzamentoPK.codice
        Dim id_reg = esercizio.impiantoPK.codice
        Dim progetto_cod = esercizio.codice
        Dim progetto_nome = esercizio.lotto

        Dim objImpiantoR As New AgronicaCoreBudgetDAL.Budget_Reg_Impianti_Read
        Dim impianto = objImpiantoR.Leggi(Id_Budget, piva, sa_cod, appezza, id_reg, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

        Dim objEsercizioR As New AgronicaCoreBudgetDAL.Budget_Impresa_Progetti_R
        Dim ese = objEsercizioR.LeggiDistinta3(Id_Budget, piva, sa_cod, appezza, id_reg, " Budget_Imprese_Progetti.Progetto_Cod = " & progetto_cod, "", objParametri_Server)

        Try
            If ese.Rows.Count > 0 Then
                For Each esercizio_db In ese.Rows
                    If Validita_Inizio <> esercizio_db("Validita_Inizio") Or Validita_Fine <> esercizio_db("Validita_Fine") Then

                        If Validita_Inizio < CDate(impianto.Rows(0)("Validita_Inizio")) Then
                            MessaggioErrore += ("La data di inizio della distinta non può essere inferiore alla data di inizio dell'impianto") & " (" & impianto.Rows(0)("Validita_Inizio") & ")."
                            Throw New Exception(MessaggioErrore)
                        End If

                        If Validita_Fine > CDate(impianto.Rows(0)("Validita_Fine")) Then
                            MessaggioErrore &= ("La data di fine della distinta non può essere superiore alla data di fine dell'impianto") & " (" & impianto.Rows(0)("Validita_Fine") & ")."
                            Throw New Exception(MessaggioErrore)
                        End If

                        'COSTI DI GESTIONE
                        Dim objControllo As New AgronicaCoreAnagrafeBIZ.Progetto_W
                        Dim controllo = objControllo.controllo_CdG(esercizio, piva, sa_cod, appezza, id_reg, progetto_cod, Validita_Inizio, Validita_Fine, objParametri_Server, Id_Budget:=Id_Budget)
                        If controllo.errore Then
                            Dim MessaggioErroreCdG As String = ""
                            If progetto_nome = "" Then
                                progetto_nome = "[dal " & esercizio_db("Validita_Inizio") & " al " & esercizio_db("Validita_Fine") & "]"
                            End If

                            If Not controllo.messaggioSpecifico Then
                                MessaggioErroreCdG &= ("Non è possibile modificare la " & controllo.inizio_fine & " dell'esercizio " & progetto_nome & ", perché sono stati associati Costi di Gestione in data successiva a quella selezionata")
                            Else
                                MessaggioErroreCdG &= ("Non è possibile modificare la " & controllo.inizio_fine & " dell'esercizio " & progetto_nome & ", perché sono stati associati Costi di Gestione in data " & controllo.dataCdG)
                            End If

                            Throw New GiasException(MessaggioErroreCdG)
                        End If

                    End If
                Next
            End If

            If Validita_Fine < Validita_Inizio Then
                MessaggioErrore += ("La fine della distinta non può precedere la sua data di inizio.")
                Throw New Exception(MessaggioErrore)
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception(MessaggioErrore)
        End Try

        Return MessaggioErrore
    End Function

End Class
