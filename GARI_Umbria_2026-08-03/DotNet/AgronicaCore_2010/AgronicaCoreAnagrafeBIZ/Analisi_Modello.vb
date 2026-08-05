Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.analisi
Imports AgronicaCoreModelsSTD.attivita.risorse
Imports AgronicaCoreModelsSTD.metaschema
Imports System.Transactions
Imports AgronicaCoreModelsSTD.costanti
Imports System.Linq
Imports Newtonsoft.Json
Imports AgronicaCoreDTOStd.Identity
Imports InData.Analisi
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreModelsSTD.exceptions

Public Class Analisi_Modello_R
    Public Shared Function Leggi_AnalisiTerreno_Modello(Analisi_SuperUser As String,
                                                        analisi_testata_cod As Integer,
                                                        objParametri_Super_Server As AgronicaCoreParametri,
                                                        objParametri_Server As AgronicaCoreParametri,
                                                        objParametri_Utenti As AgronicaCoreParametri
                                                        ) As AnalisiTerreno


        Dim objTestata As New AgronicaCoreAnagrafeDAL.Analisi_Testata_R
        Dim DTTestata As DataTable

        DTTestata = objTestata.Leggi_AnalisiTerreno_NG(analisi_testata_cod,
                                                       "",
                                                       "",
                                                       objParametri_Server)

        If DTTestata.Rows.Count = 0 Then
            Throw New GiasException("ANALISI NON TROVATA!")
        End If

        Dim objAnalisiTipi As New AgronicaCoreAnagrafeDAL.Analisi_Tipi_R

        Dim DTCertificato As DataTable
        Dim objCertificato As New AgronicaCoreAnagrafeDAL.Analisi_Certificato_Read
        Dim DTAnalisiTipologiaDettagli As DataTable
        Dim objAnalisiTipologiaDettagli As New AgronicaCoreAnagrafeDAL.Analisi_Tipologia_Dettagli_R

        Dim objEntitaxTestata As New AgronicaCoreAnagrafeDAL.Analisi_EntitaxTestata_R
        Dim DTEntita As DataTable

        Dim objDettagli As New AgronicaCoreAnagrafeDAL.Analisi_Dettagli_R
        Dim DTDettagli As DataTable

        Dim DTCampione, DTCampionixTestata, DTCampionixDettagli As DataTable
        Dim objCampioni As New AgronicaCoreAnagrafeDAL.Analisi_Campione_Read
        Dim objCampionixDettagli As New AgronicaCoreAnagrafeDAL.Analisi_CampionexDet_R


        '------------------------------
        ' ANALISI TERRENO
        '------------------------------
        Dim item_analisi = DTTestata.Rows(0)
        Dim analisiTerreno As New AnalisiTerreno(item_analisi.Item("analisi_testata_cod"), item_analisi.Item("Analisi_Testata_Des")) With {
            .validita = New IntervalloTemporale(item_analisi.Item("analisi_testata_data_inizio"), item_analisi.Item("analisi_testata_data_fine")),
            .longitude = item_analisi.Item("Analisi_Testata_Coord_X"),
            .latitude = item_analisi.Item("Analisi_Testata_Coord_Y"),
            .riferimento1 = item_analisi.Item("Analisi_Testata_Riferimento_1"),
            .riferimento2 = item_analisi.Item("Analisi_Testata_Riferimento_2"),
            .riferimento3 = item_analisi.Item("Analisi_Testata_Riferimento_3"),
            .riferimento4 = item_analisi.Item("Analisi_Testata_Riferimento_4"),
            .riferimento5 = item_analisi.Item("Analisi_Testata_Riferimento_5"),
            .note = item_analisi.Item("analisi_testata_note1"),
            .AnalisiTipo = New AnalisiTipo(item_analisi.Item("analisi_testata_tipo"), objAnalisiTipi.TipoDes_from_TipoCod(item_analisi.Item("analisi_testata_tipo"), objParametri_Server)),
            .tessitura = New ClasseTessitura(item_analisi.Item("Id_ClasseTessitura"))
        }


        '------------------------------
        ' CERTIFICATO 1 A 1
        '------------------------------
        Dim certificato_cod = item_analisi.Item("Analisi_Certificato_Cod")
        If certificato_cod <> 0 Then

            analisiTerreno.certificatoAnalisi = New CertificatoAnalisi(certificato_cod) With {
                .numero_certificato = item_analisi.Item("analisi_certificato_des")
            }

            If item_analisi.Item("analisi_certificato_laboratorio") <> "" AndAlso item_analisi.Item("analisi_certificato_laboratorio") <> "0" Then

                analisiTerreno.laboratorio = New Contatto With {
                    .primaryKey = New Contatto.PK(item_analisi.Item("piva"), item_analisi.Item("cod_Contatto")),
                    .ragione_Sociale = item_analisi.Item("Rag_Soc"),
                    .nome = item_analisi.Item("Nome"),
                    .cognome = item_analisi.Item("Cognome")
                }

                analisiTerreno.laboratorio.risorseUmane = New List(Of RisorseUmane) From {
                    New RisorseUmane(item_analisi.Item("cod_risum"))
                }

            End If

            If item_analisi.Item("Analisi_Certificato_TipologiaCod") <> 0 Then
                Dim objAnalisiTipologia As New AgronicaCoreAnagrafeDAL.Analisi_Tipologia_R
                Dim Analisi_Tipologia_Des_Long As String = ""
                Dim Numero_Determinazioni As String = ""
                analisiTerreno.analisiTipologia = New AnalisiTipologia(item_analisi.Item("Analisi_Certificato_TipologiaCod"), item_analisi.Item("Analisi_Certificato_TipologiaDes")) With {
                    .tipo = analisiTerreno.AnalisiTipo,
                    .descrizioneLunga = Analisi_Tipologia_Des_Long,
                    .numeroDeterminazioni = Numero_Determinazioni
                }

                DTAnalisiTipologiaDettagli = objAnalisiTipologiaDettagli.Leggi(item_analisi.Item("analisi_certificato_tipologiacod"),
                                                                               "", "",
                                                                               objParametri_Server)
                If DTAnalisiTipologiaDettagli.Rows.Count > 0 Then

                    analisiTerreno.analisiTipologia.dettagli = New List(Of DettaglioTipologia)

                    For Each row_dettaglio_tipologia In DTAnalisiTipologiaDettagli.Rows
                        Dim DettaglioTipologia As New DettaglioTipologia With {
                            .parametro = Get_Parametro(row_dettaglio_tipologia.item("Analisi_Parametro_Cod"), objParametri_Server),
                            .unitaMisura = Get_UDM(row_dettaglio_tipologia.item("UDM_Cod"), objParametri_Server),
                            .ordinamento = row_dettaglio_tipologia.item("Ordinamento"),
                            .LDM = row_dettaglio_tipologia.item("LDM")
                        }

                        analisiTerreno.analisiTipologia.dettagli.Add(DettaglioTipologia)
                    Next

                End If
            Else
                analisiTerreno.analisiTipologia = New AnalisiTipologia(0, "")
            End If
        End If


        '------------------------------
        ' ENTITA'
        '------------------------------
        DTEntita = objEntitaxTestata.Leggi(analisi_testata_cod, 0,
                                           "", 0, 0, 0, 0, 0,
                                           "", "", "", 0, 0, "", "",
                                           enumSelezioneVariabile.Selezione_TabellaCompleta,
                                           "", "",
                                           objParametri_Server)

        If DTEntita.Rows.Count > 0 Then

            'Inizializzo tutte le liste
            analisiTerreno.entitaImprese = New List(Of AnalisiEntita(Of Impresa))
            analisiTerreno.entitaCentri = New List(Of AnalisiEntita(Of CentroAziendale))
            analisiTerreno.entitaCampi = New List(Of AnalisiEntita(Of Campo))
            analisiTerreno.entitaAppezzamenti = New List(Of AnalisiEntita(Of Appezzamento))
            analisiTerreno.entitaImpianti = New List(Of AnalisiEntita(Of AgronicaCoreModelsSTD.anagrafiche.Impianto))
            analisiTerreno.entitaFabbricati = New List(Of AnalisiEntita(Of Fabbricato))
            analisiTerreno.entitaParticelleCatastali = New List(Of AnalisiEntita(Of CatastoCentroAziendale))

            Dim entitaEsistenti = DTEntita.DefaultView.ToTable(True, "Analisi_Entita_Cod")
            For Each entita In entitaEsistenti.Rows()
                analisiTerreno.entitaCoinvolte.Add(entita.item("Analisi_Entita_Cod"))
            Next

            For Each row_entita In DTEntita.Rows
                Dim piva As String = row_entita.Item("piva")
                Dim sa_cod As Integer = row_entita.Item("sa_cod")

                Dim campo_cod As Integer = row_entita.Item("campo_cod")
                Dim appezza As Integer = row_entita.Item("appezza")
                Dim id_imp As Integer = row_entita.Item("id_imp")
                Dim fabbricato_cod As Integer = row_entita.Item("fabbricato_cod")

                Dim prov As String = row_entita.Item("prov")
                Dim com As String = row_entita.Item("com")

                'Per gestire il pregresso: la vecchia interfaccia salvava Sezione/Subalterno = '0', quando Sezione/Subalterno = ''
                'L'anagrafica NG salva '0' gestisce --> ''
                'In sezione vanno indicate lettere ex A, AB, CF, ...
                Dim sezione As String = If(row_entita.Item("sezione") = "0", "", row_entita.Item("sezione"))
                Dim foglio As Integer = row_entita.Item("foglio")
                Dim numero As Integer = row_entita.Item("numero")
                Dim subalterno As String = If(row_entita.Item("subalterno") = "0", "", row_entita.Item("subalterno"))

                Dim id_oggetto_grafico As String = row_entita.Item("id_oggetto_grafico")

                Select Case row_entita.item("Analisi_Entita_Cod")
                    Case enum_Entita_Analisi.Impresa
                        Dim objImprese As New Impresa_R
                        Dim entitaImpresa As New AnalisiEntita(Of Impresa) With {
                            .progressivo = row_entita.Item("Progressivo"),
                            .prodotto = New Prodotto(row_entita.item("Mat_Cod")),
                            .lotto = row_entita.Item("Lotto"),
                            .stima = row_entita.Item("ChkStima"),
                            .validazione = row_entita.Item("ChkValidazione"),
                            .elementoAnagrafico = objImprese.Impresa_Leggi_Anagrafica(piva, False, False, False, False, False, objParametri_Server)
                        }

                        analisiTerreno.entitaImprese.Add(entitaImpresa)

                    Case enum_Entita_Analisi.Centro
                        Dim objCentri As New CentroAziendale_R
                        Dim entitaCentro As New AnalisiEntita(Of CentroAziendale) With {
                            .progressivo = row_entita.Item("Progressivo"),
                            .prodotto = New Prodotto(row_entita.item("Mat_Cod")),
                            .lotto = row_entita.Item("Lotto"),
                            .stima = row_entita.Item("ChkStima"),
                            .validazione = row_entita.Item("ChkValidazione"),
                            .elementoAnagrafico = objCentri.Centro_Leggi_Anagrafica(piva, sa_cod, False, False, False, False, False, objParametri_Server)
                        }

                        analisiTerreno.entitaCentri.Add(entitaCentro)

                    Case enum_Entita_Analisi.Campo
                        Dim objCampi As New Campo_R
                        Dim entitaCampo As New AnalisiEntita(Of Campo) With {
                            .progressivo = row_entita.Item("Progressivo"),
                            .prodotto = New Prodotto(row_entita.item("Mat_Cod")),
                            .lotto = row_entita.Item("Lotto"),
                            .stima = row_entita.Item("ChkStima"),
                            .validazione = row_entita.Item("ChkValidazione"),
                            .elementoAnagrafico = objCampi.Leggi_Campo(piva, sa_cod, campo_cod, objParametri_Server)
                        }

                        analisiTerreno.entitaCampi.Add(entitaCampo)

                    Case enum_Entita_Analisi.Appezzamento
                        Dim objAppezzamento As New Appezzamento_R
                        Dim entitaAppezzamento As New AnalisiEntita(Of Appezzamento) With {
                            .progressivo = row_entita.Item("Progressivo"),
                            .prodotto = New Prodotto(row_entita.item("Mat_Cod")),
                            .lotto = row_entita.Item("Lotto"),
                            .stima = row_entita.Item("ChkStima"),
                            .validazione = row_entita.Item("ChkValidazione"),
                            .elementoAnagrafico = objAppezzamento.Leggi_Appezzamento_Anagrafica(piva, sa_cod, appezza, 0, False, False, False, AGRODATAINIZIO, False, False, False, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)
                        }

                        analisiTerreno.entitaAppezzamenti.Add(entitaAppezzamento)

                    Case enum_Entita_Analisi.Impianto
                        Dim objImpianto As New Reg_Impianto_R
                        Dim entitaImpianto As New AnalisiEntita(Of AgronicaCoreModelsSTD.anagrafiche.Impianto) With {
                            .progressivo = row_entita.Item("Progressivo"),
                            .prodotto = New Prodotto(row_entita.item("Mat_Cod")),
                            .lotto = row_entita.Item("Lotto"),
                            .stima = row_entita.Item("ChkStima"),
                            .validazione = row_entita.Item("ChkValidazione"),
                            .elementoAnagrafico = objImpianto.Leggi_Impianto_Anagrafica(piva, sa_cod, appezza, id_imp, False, False, AGRODATAINIZIO, False, False, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)
                        }

                        analisiTerreno.entitaImpianti.Add(entitaImpianto)

                    Case enum_Entita_Analisi.Fabbricato
                        Dim objFabbricato As New Fabbricato_R
                        Dim entitaFabbricato As New AnalisiEntita(Of Fabbricato) With {
                            .progressivo = row_entita.Item("Progressivo"),
                            .prodotto = New Prodotto(row_entita.item("Mat_Cod")),
                            .lotto = row_entita.Item("Lotto"),
                            .stima = row_entita.Item("ChkStima"),
                            .validazione = row_entita.Item("ChkValidazione"),
                            .elementoAnagrafico = objFabbricato.Leggi_Fabbricato_Oggetto(piva, sa_cod, fabbricato_cod, objParametri_Server)
                        }

                        analisiTerreno.entitaFabbricati.Add(entitaFabbricato)

                    Case enum_Entita_Analisi.Particella
                        Dim centroPK As New CentroAziendale.PK(sa_cod, piva)
                        Dim particellaPK As New ParticelleCatastali.PK(prov, com, sezione, foglio, numero, subalterno)
                        Dim particella As New ParticelleCatastali(particellaPK)
                        Dim entitaParticellaCatastale As New AnalisiEntita(Of CatastoCentroAziendale) With {
                            .progressivo = row_entita.Item("Progressivo"),
                            .prodotto = New Prodotto(row_entita.item("Mat_Cod")),
                            .lotto = row_entita.Item("Lotto"),
                            .stima = row_entita.Item("ChkStima"),
                            .validazione = row_entita.Item("ChkValidazione"),
                            .elementoAnagrafico = New CatastoCentroAziendale() With {
                                .centro = centroPK,
                                .particella = particella
                            }
                        }

                        analisiTerreno.entitaParticelleCatastali.Add(entitaParticellaCatastale)

                    Case enum_Entita_Analisi.EntitaGrafica

                End Select
            Next

        Else
            Throw New GiasException("ENTITA' NON TROVATE!")
        End If


        '------------------------------
        ' DETTAGLI ANALISI
        '------------------------------
        DTDettagli = objDettagli.Leggi(analisi_testata_cod, 0, 0,
                                       enumSelezioneVariabile.Selezione_TabellaCompleta,
                                       "", "", objParametri_Server)

        If DTDettagli.Rows.Count > 0 Then

            analisiTerreno.dettagli = New List(Of AnalisiDettaglio)

            For Each row_dettaglio In DTDettagli.Rows

                Dim AnalisiDettaglio As New AnalisiDettaglio(row_dettaglio.Item("analisi_dettaglio_cod")) With {
                    .parametro = Get_Parametro(row_dettaglio.Item("Analisi_Parametro_Cod"), objParametri_Server),
                    .valore1 = row_dettaglio.Item("Analisi_Dettaglio_Valore_1"),
                    .margineErrore1 = row_dettaglio.Item("Analisi_Dettaglio_MargineErrore_1"),
                    .valore2 = row_dettaglio.Item("Analisi_Dettaglio_Valore_2"),
                    .margineErrore2 = row_dettaglio.Item("Analisi_Dettaglio_MargineErrore_2")
                }

                DTCampionixDettagli = objCampionixDettagli.Leggi(analisi_testata_cod, row_dettaglio.Item("analisi_dettaglio_cod"), 0,
                                                                 enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                 "", "",
                                                                 objParametri_Server)

                If DTCampionixDettagli.Rows.Count > 0 Then
                    Dim dettaglio_campione = DTCampionixDettagli.Rows(0)
                    DTCampione = objCampioni.Leggi(dettaglio_campione.Item("Analisi_Campione_Cod"),
                                                   enumSelezioneVariabile.Selezione_JoinCompleta,
                                                   "", "",
                                                   objParametri_Server)

                    Dim campione = DTCampione.Rows(0)
                    AnalisiDettaglio.campione = Get_Campione(campione, objParametri_Server)

                End If

                analisiTerreno.dettagli.Add(AnalisiDettaglio)

            Next

        End If


        '------------------------------
        ' CAMPIONI ASSOCIATI ALL'ANALISI
        '------------------------------
        DTCampionixTestata = objCampionixDettagli.Leggi(analisi_testata_cod, 0, 0,
                                                        enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                        "analisi_dettaglio_cod = 0", "",
                                                        objParametri_Server)

        If DTCampionixTestata.Rows.Count > 0 Then

            analisiTerreno.campioni = New List(Of Campione)

            For Each row_campione_testata In DTCampionixTestata.Rows

                DTCampione = objCampioni.Leggi(row_campione_testata.Item("Analisi_Campione_Cod"),
                                               enumSelezioneVariabile.Selezione_TabellaCompleta,
                                               "", "",
                                               objParametri_Server)

                For Each row_campione In DTCampione.Rows

                    Dim Campione = Get_Campione(row_campione, objParametri_Server)

                    analisiTerreno.campioni.Add(Campione)

                Next

            Next

        End If

        'Verifico se l'analisi è agganciata al PUA o ad un PC per bloccare la modifica dei parametri
        Dim objPC_Dettagli_R As New AgronicaCoreAnagrafeDAL.PianoConcimazione_Dettagli_R
        If objPC_Dettagli_R.Leggi(0, 0, "", enumSelezioneVariabile.Selezione_TabellaCompleta, " PC_Dettagli_analisi_testata_cod = " & UtilityProvider.Agro_SQL_SaveNum(analisi_testata_cod) & " ", "", objParametri_Server).Rows.Count > 0 Then
            analisiTerreno.analisiUtilizzata = True
        End If
        If Not analisiTerreno.analisiUtilizzata Then
            Dim objParticelleCatastali_VincoliAgronomici_R As New AgronicaCoreAnagrafeDAL.ParticelleCatastalixVincoliAgronomici_R
            Dim objAnagrafe_VincoliAgronomici_R As New AgronicaCoreAnagrafeDAL.Anagrafe_VincoliAgronomici_R
            If objParticelleCatastali_VincoliAgronomici_R.Leggi(0, "", "", "", 0, 0, "", "", "", "", "", "", "", " analisi_testata_cod = " & UtilityProvider.Agro_SQL_SaveNum(analisi_testata_cod) & " ", "", objParametri_Server).Rows.Count > 0 OrElse
                objAnagrafe_VincoliAgronomici_R.Leggi(0, "", 0, 0, 0, 0, 0, 0, analisi_testata_cod, "", "", objParametri_Server).Rows.Count > 0 Then
                analisiTerreno.analisiUtilizzata = True
            End If
        End If

        Return analisiTerreno

    End Function

    Public Function DT_to_Json_AnalisiTerreno(ByVal dt As DataTable) As String
        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)
        Dim c As ColonneNome

        l.Add(New ColonneNome("Analisi_Testata_Cod", "Analisi_Testata_Cod", "number") With {._hidden = True})
        l.Add(New ColonneNome("Analisi_Certificato_Cod", "Analisi_Certificato_Cod", "number") With {._hidden = True})

        l.Add(New ColonneNome("Descrizione", Gias.Descrizione, "string") With {._Editabile = True})
        l.Add(New ColonneNome("NumeroCertificato", Gias.NumeroCertificato, "string") With {._Editabile = True})
        l.Add(New ColonneNome("Laboratorio", Gias.Laboratorio, "string"))
        l.Add(New ColonneNome("Schema", Gias.Schema, "string"))
        l.Add(New ColonneNome("Analisi_Testata_Note1", Gias.Note, "string") With {._Editabile = True})
        l.Add(New ColonneNome("Validita_Inizio", Gias.ValiditaInizio, "date") With {._Editabile = True})
        l.Add(New ColonneNome("Validita_Fine", Gias.ValiditaFine, "date") With {._Editabile = True})
        l.Add(New ColonneNome("Utente_Creazione", Gias.UtenteCreazione, "string"))
        l.Add(New ColonneNome("Utente_Modifica", Gias.UtenteModifica, "string"))
        l.Add(New ColonneNome("Data_Creazione", Gias.DataCreazione, "date") With {._Display = False})
        l.Add(New ColonneNome("Data_Modifica", Gias.DataModifica, "date") With {._Display = False})


        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable_KendoOpt(dt, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa, inParallelo:=False)

        Return risp
    End Function

#Region "Utility"
    Private Shared Function Get_UDM(Udm_Cod As Integer,
                                    objParametri_Server As AgronicaCoreParametri) As UnitaDiMisura

        Dim objUDM As New AgronicaCoreMetaSchemaDAL.UnitaMisura_R

        Dim Udm_Sim As String = ""

        Dim unitaMisura As New UnitaDiMisura(Udm_Cod) With {
            .descrizione = objUDM.UdmDes_from_UdmCod(Udm_Cod, Udm_Sim, objParametri_Server),
            .simbolo = Udm_Sim
        }

        Return unitaMisura
    End Function

    Private Shared Function Get_Parametro(Analisi_Parametro_Cod As Integer,
                                          objParametri_Server As AgronicaCoreParametri) As AnalisiParametro

        Dim objAnalisiParametri As New AgronicaCoreAnagrafeDAL.Analisi_Parametri_R

        Dim Analisi_Parametro_UdM As Integer
        Dim Analisi_Parametro_Simbolo As String = ""
        Dim Analisi_Parametro_ValoreMin As Decimal
        Dim Analisi_Parametro_ValoreMax As Decimal

        Dim AnalisiParametro As New AnalisiParametro(Analisi_Parametro_Cod, objAnalisiParametri.ParametroDes_from_ParametroCod(Analisi_Parametro_Cod, Analisi_Parametro_UdM, Analisi_Parametro_Simbolo, objParametri_Server, Analisi_Parametro_ValoreMin, Analisi_Parametro_ValoreMax)) With {
            .simbolo = Analisi_Parametro_Simbolo,
            .unitaMisura = Get_UDM(Analisi_Parametro_UdM, objParametri_Server),
            .min = Analisi_Parametro_ValoreMin,
            .max = Analisi_Parametro_ValoreMax
        }

        Return AnalisiParametro
    End Function

    Private Shared Function Get_Campione(row_campione As DataRow,
                                         objParametri_Server As AgronicaCoreParametri) As Campione

        Dim Campione = New Campione(row_campione.Item("Analisi_Campione_Cod"), row_campione.Item("Analisi_Campione_Des")) With {
            .longitude = row_campione.Item("Analisi_Campione_Coord_X"),
            .latitude = row_campione.Item("Analisi_Campione_Coord_Y"),
            .quantita = row_campione.Item("Analisi_Campione_Quantita"),
            .unitaMisura = Get_UDM(row_campione.Item("Analisi_Campione_UdM"), objParametri_Server),
            .profondita = row_campione.Item("Analisi_Campione_Profondita"),
            .profonditaMin = row_campione.Item("Analisi_Campione_Profondita_Min"),
            .profonditaMax = row_campione.Item("Analisi_Campione_Profondita_Max"),
            .riferimento1 = row_campione.Item("Analisi_Campione_Riferimento_1"),
            .riferimento2 = row_campione.Item("Analisi_Campione_Riferimento_2"),
            .riferimento3 = row_campione.Item("Analisi_Campione_Riferimento_3"),
            .riferimento4 = row_campione.Item("Analisi_Campione_Riferimento_4"),
            .riferimento5 = row_campione.Item("Analisi_Campione_Riferimento_5"),
            .note = row_campione.Item("Analisi_Campione_Note"),
            .dataPrelievo = row_campione.Item("Validita_Inizio"),
            .KeyPiva = row_campione.Item("Analisi_Campione_Key_Piva"),
            .KeySaCod = row_campione.Item("Analisi_Campione_Key_SaCod"),
            .KeyGrafica = row_campione.Item("Analisi_Campione_Key_IDGrafica"),
            .particella = New ParticelleCatastali(New ParticelleCatastali.PK(row_campione.Item("Analisi_Campione_Prov"),
                                                                                       row_campione.Item("Analisi_Campione_com"),
                                                                                       row_campione.Item("Analisi_Campione_sezione"),
                                                                                       row_campione.Item("Analisi_Campione_foglio"),
                                                                                       row_campione.Item("Analisi_Campione_numero"),
                                                                                       row_campione.Item("Analisi_Campione_subalterno")))
        }

        Return Campione

    End Function

    Public Function Check_ParametroObbligatorio(Analisi_Tipo As Integer, TipoAnalisi As String) As Boolean
        If Mid(TipoAnalisi, Analisi_Tipo, 1) = 2 Then
            Return True
        End If

        Return False
    End Function

    ''' <summary>
    ''' La tessitura del terreno viene estratta a scalare in questo:
    ''' 1) Analisi su Appezzamento
    ''' 2) Analisi su Campo associato ad appezzamento
    ''' 3) Analisi sul catasto dell'appezzamento
    ''' 4) Valori di sabbia-limo-argilla-tessitura salvati su Anagrafica Appezzamento
    ''' 5) Analisi sul centro aziendale
    ''' 
    ''' Se a parità di livello esistono più analisi con sabbia-limo-argilla, viene fatta una media aritmetica dei valori e il ricalcolo della tessitura In base ai nuovi valori.
    ''' Esempio: 
    ''' Analisi 1 Sabbia 50, Limo 20, Argilla 30 
    ''' Analisi 2: Sabbia 60, Limo 25, Argilla 15
    ''' Valori Calcolo Tessitura: Sabbia 55, Limo 23 (arrotondato), Argilla 23 (arrotondato)
    ''' (Il punto (3) fa una media delle analisi per singola particella)
    ''' 
    ''' Poniamo che per un appezzamento esistano solo analisi associate alle particelle (3) e valori registrati sull'anagrafica (4):
    ''' Se l 'appezzamento è associato a due particelle, di cui solo una ha un'analisi registrata, la tessitura della superficie rimanente sarà estratta a scalare partendo dal punto 4) in poi.",

    ''' </summary>
    ''' <param name="dtColture"></param>
    ''' <param name="objParametri_Server"></param>
    ''' <param name="objParametri_Super_Server"></param>
    Public Sub CalcolaTessiturexUMA(ByRef dtColture As DataTable,
                                    richiesta_cod As Integer,
                                    objParametri_Server As AgronicaCoreParametri,
                                    objParametri_Super_Server As AgronicaCoreParametri)

        Dim listaImpianti As New List(Of (String, Integer, Integer, Integer))
        For Each coltura In dtColture.Rows
            Dim piva As String = coltura.Item("Piva")
            Dim sa_cod As Integer = coltura.Item("Sa_Cod")
            Dim appezza As Integer = coltura.Item("Appezza")
            Dim Id_Reg As Integer = coltura.Item("Id_Reg")
            listaImpianti.Add((piva, sa_cod, appezza, Id_Reg))
        Next

        Dim objEntitaxTestata As New AgronicaCoreAnagrafeDAL.Analisi_EntitaxTestata_R
        Dim tessiture As DataTable = objEntitaxTestata.Leggi_Tessitura_Scalare_Lista_Appezzamenti(listaImpianti.Distinct().ToList(), richiesta_cod, objParametri_Server)

        If tessiture.Rows.Count > 0 Then
            'Chiamare webservice
            Dim objParametriIngresso As New List(Of AgronicaCorePianoConcimazioneBIZ.PUA_ClassiTessitura_Analisi_input)
            For Each tessitura In tessiture.Rows
                If IsDBNull(tessitura.item("Id_ClasseTessitura")) Then
                    If Not IsDBNull(tessitura.item("sabbiaAvg")) AndAlso Not IsDBNull(tessitura.item("argillaAvg")) Then

                        objParametriIngresso.Add(New AgronicaCorePianoConcimazioneBIZ.PUA_ClassiTessitura_Analisi_input With {
                                                     .Sabbia = Agro_Math.ArrotondaVal_0(CDec(CDbl(tessitura.item("sabbiaAvg")))),
                                                     .Argilla = Agro_Math.ArrotondaVal_0(CDec(CDbl(tessitura.item("argillaAvg"))))
                                                 }
                        )
                    Else
                        'Non ci sono valori per calcolare la tessitura
                        tessitura.item("Id_ClasseTessitura") = 0
                    End If
                End If
            Next

            If objParametriIngresso.Count > 0 Then
                'Calcolo la tessitura da webservice se ce n'è bisogno
                Dim url As String = ""
                Dim objAgroWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
                If IsNothing(objAgroWebConfig.GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione) Then
                    Dim agroWs As String
                    Dim objConfigurazione_Siti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                    agroWs = objConfigurazione_Siti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametri_Server)
                    If agroWs = "" Then
                        agroWs = objConfigurazione_Siti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametri_Super_Server)
                    End If
                    url = agroWs
                Else
                    url = objAgroWebConfig.GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione
                End If

                Dim objParametriUscita As New AgronicaCorePianoConcimazioneBIZ.PUA_ClassiTessitura_Analisi_output
                Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
                objParametriUscita = objPC_WS.ClassiTessituraDaListaAnalisi(url, objParametriIngresso)

                If objParametriUscita IsNot Nothing AndAlso objParametriUscita.ListaClassiTessitura IsNot Nothing Then
                    For Each tessitura In objParametriUscita.ListaClassiTessitura
                        For Each tessituraNull In tessiture.Rows
                            If IsDBNull(tessituraNull.item("Id_ClasseTessitura")) AndAlso
                                tessituraNull.item("sabbiaAvg") = tessitura.Sabbia AndAlso
                                tessituraNull.item("argillaAvg") = tessitura.Argilla Then
                                tessituraNull.item("Id_ClasseTessitura") = tessitura.Id_ClasseTessitura
                            End If
                        Next
                    Next
                End If
            End If
        End If

        For Each rowUMA In dtColture.Rows
            Dim piva As String = rowUMA.Item("Piva")
            Dim sa_cod As Integer = rowUMA.Item("Sa_Cod")
            Dim appezza As Integer = rowUMA.Item("Appezza")
            Dim Id_Reg As Integer = rowUMA.Item("Id_Reg")

            'Per ogni impianto estraggo le tessiture che ho trovato con la query
            Dim dtTessiture = tessiture.Select("piva = '" & piva & "' AND sa_cod = " & sa_cod & " AND appezza = " & appezza & " AND Id_Reg = " & Id_Reg & "").CopyToDataTable()

            If dtTessiture IsNot Nothing Then
                Dim terrenoNormale As Decimal = 0 'tessitura 0, 1
                Dim terrenoMedio As Decimal = 0 'tessitura 2
                Dim terrenoTenace As Decimal = 0 'tessitura 3

                'La query estrae un singolo tipo analisi per impianto-sup impianto-area tessitura
                'Se ci sono più righe significa che una di queste condizioni è vera:
                'l'appezzamento è su almeno una particella, che potrebbe non coprire l'interità della superficie dell'impianto
                'l'appezzamento è su almeno una particella, che potrebbe superare di superficie l'impianto di riferimento
                'verifico se ci sono più righe e se la prima è di tipo 3 (analisi su particelle)
                'se esiste un'analisi su appezzamento (1) oppure su campo (2) , prendo solo quella e la applico a tutta la superficie dell'impianto
                Dim supFrammentataParticelle As Boolean = dtTessiture.Rows.Count > 1 AndAlso dtTessiture.Rows(0).Item("TipoCod") = 3

                If Not supFrammentataParticelle AndAlso dtTessiture.Rows.Count > 1 Then
                    Do Until dtTessiture.Rows.Count = 1
                        Dim count As Integer = dtTessiture.Rows.Count
                        dtTessiture.Rows.RemoveAt(count - 1)
                    Loop
                End If

                'Se true, la somma delle particelle supera la superficie totale dell'impianto.
                'Al momento dell'estrazione della tessitura tengo conto solo di quella con costo minore (costo normale<media<tenace)
                'e la applico a tutta la superficie dell'impianto anche se la particella non coincide con l'interità della superficie
                Dim superficieParticelleIncoerenteImpianto As Boolean = False

                If supFrammentataParticelle Then
                    'Verifico se la somma delle particelle (analisi TipoCod 3) supera la superficie totale dell'impianto
                    Dim supTotaleParticelle As Decimal = dtTessiture.Compute("SUM(AreaTessitura)", "TipoCod = 3")
                    'faccio il controllo sulla prima riga delle tessiture (la colonna contiene sempre lo stesso valore)
                    superficieParticelleIncoerenteImpianto = supTotaleParticelle > dtTessiture.Rows(0).Item("Sup_TotaleImpianto")

                    If superficieParticelleIncoerenteImpianto Then
                        'Riordino il datatable per avere prima le particelle con tessitura meno costosa (costo normale<media<tenace)
                        dtTessiture = dtTessiture.Select("", "TipoCod ASC, Id_ClasseTessitura ASC").CopyToDataTable()
                    End If
                End If

                For Each tessitura In dtTessiture.Rows

                    Dim currentTipo As Integer = tessitura.Item("TipoCod")
                    ' 1 - ANALISI SU APPEZZAMENTO
                    ' 2 - ANALISI SU CAMPO
                    ' 3 - ANALISI SU CATASTO
                    ' 4 - VALORI APPEZZAMENTO
                    ' 5 - ANALISI SU CENTRO

                    Dim currentTessitura As Integer = If(IsDBNull(tessitura.Item("Id_ClasseTessitura")), 0, tessitura.Item("Id_ClasseTessitura"))

                    Dim AreaTessitura As Decimal = tessitura.Item("AreaTessitura")
                    Dim Sup_TotaleImpianto As Decimal = tessitura.Item("Sup_TotaleImpianto")

                    If superficieParticelleIncoerenteImpianto Then
                        'Se la somma delle particelle supera la superficie totale dell'impianto,
                        'imposto l'area della tessitura uguale alla superficie totale dell'impianto
                        AreaTessitura = Sup_TotaleImpianto

                    Else

                        If supFrammentataParticelle AndAlso currentTipo <> 3 Then
                            'Le tessiture sono ordinate per tipo, se non ho più particelle posso avere valori dal 4 al 5
                            'Prendo la somma delle aree delle particelle e la sottraggo alla superficie totale dell'impianto
                            Dim supDaParticelle As Decimal = terrenoNormale + terrenoMedio + terrenoTenace
                            AreaTessitura = Sup_TotaleImpianto - supDaParticelle
                        End If
                    End If

                    Select Case currentTessitura
                        Case 0, 1 'NORMALE
                            terrenoNormale += AreaTessitura
                        Case 2 'MEDIA
                            terrenoMedio += AreaTessitura
                        Case 3 'TENACE
                            terrenoTenace += AreaTessitura
                    End Select

                    If superficieParticelleIncoerenteImpianto Then
                        'Prendo solo la prima tessitura che trovo
                        Exit For
                    End If
                Next

                'Aggiorno i valori
                rowUMA.Item("Sup_Normale") = terrenoNormale
                rowUMA.Item("Sup_Media") = terrenoMedio
                rowUMA.Item("Sup_Tenace") = terrenoTenace

            End If

        Next

    End Sub

#End Region

End Class
Public Class Analisi_Modello_W

    Public Function Scrivi_AnalisiTerreno_Modello(piva As String,
                                                  ByRef analisi_testata_cod As Integer,
                                                  analisiTerreno As AnalisiTerreno,
                                                  objParametri_Super_Server As AgronicaCoreParametri,
                                                  objParametri_Server As AgronicaCoreParametri,
                                                  objParametri_Utenti As AgronicaCoreParametri,
                                                    Optional FlagConnessioneLocale As Boolean = True,
                                                    Optional FlagTransazioneLocale As Boolean = True,
                                                    Optional NoteLog As String = "",
                                                    Optional Origine As Integer = -1,
                                                    Optional saltaControlliAggancio As Boolean = False
                                                   ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Analisi_Modello_W.Analisi_Modello_Scrivi()"

        Dim messaggioErrore As String
        Dim ret As Boolean = True

        Dim username As String = objParametri_Server.UsernameOperazione

        Dim tipoOperazione As enum_TipoOperazioneDB

        Try

            'In cancellazione non ho bisogno di fare controlli sui dati obbligatori
            If Not analisiTerreno.flag_cancellazione Then
                Check_DatiObbligatori(analisiTerreno)
            End If

            ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, FlagTransazioneLocale, objParametri_Server)

            Dim objSequenze As New Agro_Sequenze
            Dim objTestate As New AgronicaCoreAnagrafeDAL.Analisi_Testata_W

            Dim leggiLingua As New AgronicaCoreUtentiDAL.Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            If analisiTerreno.flag_cancellazione Then
                tipoOperazione = enum_TipoOperazioneDB.Cancellazione
                ret = Internal_Cancella_AnalisiTerreno_Modello(piva, analisi_testata_cod, analisiTerreno, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)
            Else
                If analisi_testata_cod = 0 Then
                    tipoOperazione = enum_TipoOperazioneDB.Scrittura
                    ret = Internal_Scrivi_AnalisiTerreno_Modello(piva, analisi_testata_cod, analisiTerreno, objParametri_Server, objParametri_Super_Server)
                Else
                    tipoOperazione = enum_TipoOperazioneDB.Modifica
                    ret = Internal_Modifica_AnalisiTerreno_Modello(piva, analisi_testata_cod, analisiTerreno, objParametri_Server, objParametri_Super_Server, saltaControlli:=saltaControlliAggancio)
                End If
            End If
            '
            Dim tzh As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
            Dim DatiAnalisiTerreno = JsonConvert.SerializeObject(analisiTerreno, tzh)
            Dim objAgronicaLogAnalisi As New AgronicaCoreAnagrafeDAL.AgronicaLogAnalisi_W
            objAgronicaLogAnalisi.Scrivi(tipoOperazione, analisiTerreno.AnalisiTipo.codice, analisi_testata_cod, analisiTerreno.descrizione, analisiTerreno.validita.inizio, NoteLog, enum_Id_Servizio.GiasOnline, objParametri_Server, DatiAnalisiTerreno, Origine:=Origine, Piva:=piva)

            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)

        Catch ex As GiasException
            ret = False

            messaggioErrore = ex.Message
            If objParametri_Server.objTransazione IsNot Nothing Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If

            Throw New GiasException(messaggioErrore)

        Catch ex As Exception

            ret = False

            messaggioErrore = ex.Message
            If objParametri_Server.objTransazione IsNot Nothing Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If

            Throw New Exception("[" & nomeRoutine & "]  " & messaggioErrore)

        Finally

            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

        End Try

        Return ret

    End Function

    Private Shared Function Internal_Modifica_AnalisiTerreno_Modello(piva As String,
                                                                     analisi_testata_cod As Integer,
                                                                     analisiTerreno As AnalisiTerreno,
                                                                     objParametri_Server As AgronicaCoreParametri,
                                                                     objParametri_Super_Server As AgronicaCoreParametri,
                                                                        Optional saltaControlli As Boolean = False) As Boolean

        Dim objSequenze As New Agro_Sequenze

        Dim objTestata As New AgronicaCoreAnagrafeDAL.Analisi_Testata_W
        Dim objDettaglio As New AgronicaCoreAnagrafeDAL.Analisi_Dettagli_W

        Dim objCampione As New AgronicaCoreAnagrafeDAL.Analisi_Campione_W
        Dim objCampionixDettagli As New AgronicaCoreAnagrafeDAL.Analisi_CampionexDet_W

        Dim objCertificato_R As New AgronicaCoreAnagrafeDAL.Analisi_Certificato_Read

        Dim objCertificato As New AgronicaCoreAnagrafeDAL.Analisi_Certificato_W

        If Not saltaControlli Then
            Dim objPC_Dettagli_R As New AgronicaCoreAnagrafeDAL.PianoConcimazione_Dettagli_R
            If objPC_Dettagli_R.Leggi(0, 0, "", enumSelezioneVariabile.Selezione_TabellaCompleta, " PC_Dettagli_analisi_testata_cod = " & analisi_testata_cod & " ", "", objParametri_Server).Rows.Count > 0 Then
                Throw New GiasException(String.Format(Gias.ImpossibileCancellareAnalisiAggancioPianoConcimazione, analisiTerreno.descrizione))
            End If

            Dim objParticelleCatastali_VincoliAgronomici_R As New AgronicaCoreAnagrafeDAL.ParticelleCatastalixVincoliAgronomici_R
            Dim objAnagrafe_VincoliAgronomici_R As New AgronicaCoreAnagrafeDAL.Anagrafe_VincoliAgronomici_R
            If objParticelleCatastali_VincoliAgronomici_R.Leggi(0, "", "", "", 0, 0, "", "", "", "", "", "", "", " analisi_testata_cod = " & analisi_testata_cod & " ", "", objParametri_Server).Rows.Count > 0 OrElse
                        objAnagrafe_VincoliAgronomici_R.Leggi(0, "", 0, 0, 0, 0, 0, 0, analisi_testata_cod, "", "", objParametri_Server).Rows.Count > 0 Then
                Throw New GiasException(String.Format(Gias.ImpossibileCancellareAnalisiAggancioPUA, analisiTerreno.descrizione))
            End If
        End If

        If String.IsNullOrEmpty(analisiTerreno.descrizione) OrElse String.IsNullOrWhiteSpace(analisiTerreno.descrizione) Then
            Dim objAnalisiTipi As New AgronicaCoreAnagrafeDAL.Analisi_Tipi_R
            analisiTerreno.descrizione = objAnalisiTipi.TipoDes_from_TipoCod(analisiTerreno.AnalisiTipo.codice, objParametri_Server)
        End If

        If IsNothing(analisiTerreno.validita) Then
            analisiTerreno.validita = New IntervalloTemporale(AGRODATAINIZIO, AGRODATAFINE)
        End If

        Scrivi_Certificato(analisiTerreno, objParametri_Server)

        analisiTerreno.tessitura = Get_ClasseTessitura(analisiTerreno, objParametri_Server, objParametri_Super_Server)

        If IsNothing(analisiTerreno.longitude) Then
            analisiTerreno.longitude = 0
        End If
        If IsNothing(analisiTerreno.latitude) Then
            analisiTerreno.latitude = 0
        End If

        objTestata.Modifica(analisi_testata_cod,
                            If(IsNothing(analisiTerreno.certificatoAnalisi), 0, analisiTerreno.certificatoAnalisi.codice),
                            analisiTerreno.descrizione,
                            analisiTerreno.validita.inizio,
                            analisiTerreno.validita.fine,
                            analisiTerreno.longitude,
                            analisiTerreno.latitude,
                            analisiTerreno.riferimento1, analisiTerreno.riferimento2, analisiTerreno.riferimento3, analisiTerreno.riferimento4, analisiTerreno.riferimento5,
                            analisiTerreno.note, "", "", "",
                            analisiTerreno.AnalisiTipo.codice, 0, 0,
                            Date.Now, AGRODATAFINE,
                            objParametri_Server,
                            If(analisiTerreno.tessitura IsNot Nothing, analisiTerreno.tessitura.codice, 0))

        Scrivi_EntitaXTestata(piva, analisi_testata_cod, analisiTerreno, objParametri_Server, clean_tabella_entita:=True)

        Clean_Tabella_Dettagli(analisi_testata_cod, analisiTerreno.dettagli, objParametri_Server)
        'Clean_Tabella_Campioni(analisi_testata_cod, analisiTerreno.campioni, False, objParametri_Server)

        If analisiTerreno.dettagli IsNot Nothing Then

            For Each dettaglio In analisiTerreno.dettagli
                If dettaglio.valore1 IsNot Nothing Then
                    If dettaglio.codice = 0 Then
                        dettaglio.codice = objSequenze.NuovoId_Tabella("ANALISI_DETTAGLI", 0, 20000000, objParametri_Server)

                        objDettaglio.Scrivi(analisi_testata_cod, dettaglio.codice,
                                            dettaglio.parametro.codice,
                                            If(IsNothing(dettaglio.valore1), 0, dettaglio.valore1), dettaglio.margineErrore1,
                                            If(IsNothing(dettaglio.valore2), 0, dettaglio.valore2), dettaglio.margineErrore2,
                                            0,
                                            AGRODATAINIZIO, AGRODATAFINE,
                                            objParametri_Server)

                    Else

                        objDettaglio.Modifica(analisi_testata_cod, dettaglio.codice,
                                              dettaglio.parametro.codice,
                                              If(IsNothing(dettaglio.valore1), 0, dettaglio.valore1), dettaglio.margineErrore1,
                                              If(IsNothing(dettaglio.valore2), 0, dettaglio.valore2), dettaglio.margineErrore2,
                                              0,
                                              AGRODATAINIZIO, AGRODATAFINE,
                                              objParametri_Server)
                    End If

                    If dettaglio.campione IsNot Nothing Then
                        Dim campione = dettaglio.campione

                        If campione.codice = 0 Then
                            campione.codice = objSequenze.NuovoId_Tabella("ANALISI_CAMPIONI", 0, 20000000, objParametri_Server)
                            Scrivi_Campione(piva, analisi_testata_cod, dettaglio.codice, campione, objParametri_Server)
                        Else
                            Modifica_Campione(analisi_testata_cod, dettaglio.codice, campione, objParametri_Server)
                        End If

                    End If
                End If
            Next
        End If

        Clean_Tabella_Campioni(analisi_testata_cod, analisiTerreno.campioni, True, objParametri_Server)

        If analisiTerreno.campioni IsNot Nothing Then
            For Each campione In analisiTerreno.campioni

                If campione.codice = 0 Then
                    campione.codice = objSequenze.NuovoId_Tabella("ANALISI_CAMPIONI", 0, 20000000, objParametri_Server)
                    Scrivi_Campione(piva, analisi_testata_cod, 0, campione, objParametri_Server)
                Else
                    Modifica_Campione(analisi_testata_cod, 0, campione, objParametri_Server)
                End If

            Next
        End If

        Return True

    End Function

    Private Shared Function Internal_Scrivi_AnalisiTerreno_Modello(piva As String,
                                                                   ByRef analisi_testata_cod As Integer,
                                                                   ByRef analisiTerreno As AnalisiTerreno,
                                                                   objParametri_Server As AgronicaCoreParametri,
                                                                   objParametri_Super_Server As AgronicaCoreParametri
                                                                   ) As Boolean

        Dim objSequenze As New Agro_Sequenze

        Dim objCertificato As New AgronicaCoreAnagrafeDAL.Analisi_Certificato_W

        Dim objTestata As New AgronicaCoreAnagrafeDAL.Analisi_Testata_W
        Dim objDettaglio As New AgronicaCoreAnagrafeDAL.Analisi_Dettagli_W

        Dim objCampione As New AgronicaCoreAnagrafeDAL.Analisi_Campione_W
        Dim objCampionixDettagli As New AgronicaCoreAnagrafeDAL.Analisi_CampionexDet_W

        analisi_testata_cod = objSequenze.NuovoId_Tabella("ANALISI_TESTATA", 0, 20000000, objParametri_Server)

        Scrivi_Certificato(analisiTerreno, objParametri_Server)

        analisiTerreno.tessitura = Get_ClasseTessitura(analisiTerreno, objParametri_Server, objParametri_Super_Server)

        If IsNothing(analisiTerreno.validita) Then
            analisiTerreno.validita = New IntervalloTemporale(AGRODATAINIZIO, AGRODATAFINE)
        End If

        If IsNothing(analisiTerreno.longitude) Then
            analisiTerreno.longitude = 0
        End If
        If IsNothing(analisiTerreno.latitude) Then
            analisiTerreno.latitude = 0
        End If

        If String.IsNullOrEmpty(analisiTerreno.descrizione) OrElse String.IsNullOrWhiteSpace(analisiTerreno.descrizione) Then
            Dim objAnalisiTipi As New AgronicaCoreAnagrafeDAL.Analisi_Tipi_R
            analisiTerreno.descrizione = objAnalisiTipi.TipoDes_from_TipoCod(analisiTerreno.AnalisiTipo.codice, objParametri_Server)
        End If

        objTestata.Scrivi(analisi_testata_cod,
                          If(IsNothing(analisiTerreno.certificatoAnalisi), 0, analisiTerreno.certificatoAnalisi.codice),
                          analisiTerreno.descrizione,
                          analisiTerreno.validita.inizio, analisiTerreno.validita.fine,
                          analisiTerreno.longitude, analisiTerreno.latitude,
                          analisiTerreno.riferimento1, analisiTerreno.riferimento2, analisiTerreno.riferimento3, analisiTerreno.riferimento4, analisiTerreno.riferimento5,
                          analisiTerreno.note, "", "", "",
                          analisiTerreno.AnalisiTipo.codice, 0, 0,
                          Date.Now, Date.Now, AGRODATAFINE,
                          objParametri_Server,
                          If(analisiTerreno.tessitura IsNot Nothing, analisiTerreno.tessitura.codice, 0))

        Scrivi_EntitaXTestata(piva, analisi_testata_cod, analisiTerreno, objParametri_Server)

        If analisiTerreno.dettagli IsNot Nothing Then
            For Each dettaglio In analisiTerreno.dettagli
                If dettaglio.valore1 IsNot Nothing Then
                    dettaglio.codice = objSequenze.NuovoId_Tabella("ANALISI_DETTAGLI", 0, 20000000, objParametri_Server)

                    objDettaglio.Scrivi(analisi_testata_cod, dettaglio.codice,
                                    dettaglio.parametro.codice,
                                    If(IsNothing(dettaglio.valore1), 0, dettaglio.valore1), dettaglio.margineErrore1,
                                    If(IsNothing(dettaglio.valore2), 0, dettaglio.valore2), dettaglio.margineErrore2,
                                    0,
                                    AGRODATAINIZIO, AGRODATAFINE,
                                    objParametri_Server)

                    If dettaglio.campione IsNot Nothing Then
                        Dim campione = dettaglio.campione

                        campione.codice = objSequenze.NuovoId_Tabella("ANALISI_CAMPIONI", 0, 20000000, objParametri_Server)

                        Scrivi_Campione(piva, analisi_testata_cod, dettaglio.codice, campione, objParametri_Server)

                    End If
                End If
            Next
        End If

        If analisiTerreno.latitude <> 0 AndAlso analisiTerreno.longitude <> 0 Then

            Dim campione As New Campione(objSequenze.NuovoId_Tabella("ANALISI_CAMPIONI", 0, 20000000, objParametri_Server), "Ha.0 - " & analisiTerreno.descrizione & " - N.1") With {
                .latitude = analisiTerreno.latitude,
                .longitude = analisiTerreno.longitude,
                .dataPrelievo = analisiTerreno.validita.inizio
            }

            Scrivi_Campione(piva, analisi_testata_cod, 0, campione, objParametri_Server)

        End If

        If analisiTerreno.campioni IsNot Nothing Then

            For Each campione In analisiTerreno.campioni

                campione.codice = objSequenze.NuovoId_Tabella("ANALISI_CAMPIONI", 0, 20000000, objParametri_Server)

                Scrivi_Campione(piva, analisi_testata_cod, 0, campione, objParametri_Server)

            Next
        End If

        Return True

    End Function

    Private Shared Function Internal_Cancella_AnalisiTerreno_Modello(piva As String,
                                                                     analisi_testata_cod As Integer,
                                                                     ByRef analisiTerreno As AnalisiTerreno,
                                                                     objParametri_Super_Server As AgronicaCoreParametri,
                                                                     objParametri_Server As AgronicaCoreParametri,
                                                                     objParametri_Utenti As AgronicaCoreParametri
                                                                     ) As Boolean

        analisiTerreno = Analisi_Modello_R.Leggi_AnalisiTerreno_Modello(objParametri_Server.PivaSuperUser, analisi_testata_cod, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)

        Dim objTestata As New AgronicaCoreAnagrafeDAL.Analisi_Testata_W
        Dim objEntitaxTestata As New AgronicaCoreAnagrafeDAL.Analisi_EntitaxTestata_W

        Dim objCampioni As New AgronicaCoreAnagrafeDAL.Analisi_Campione_W
        Dim objCampionixDettagli As New AgronicaCoreAnagrafeDAL.Analisi_CampionexDet_W

        Dim objPC_Dettagli_R As New AgronicaCoreAnagrafeDAL.PianoConcimazione_Dettagli_R
        If objPC_Dettagli_R.Leggi(0, 0, "", enumSelezioneVariabile.Selezione_TabellaCompleta, " PC_Dettagli_analisi_testata_cod = " & analisi_testata_cod & " ", "", objParametri_Server).Rows.Count > 0 Then
            Throw New GiasException(String.Format(Gias.ImpossibileCancellareAnalisiAggancioPianoConcimazione, analisiTerreno.descrizione))
        End If

        Dim objParticelleCatastali_VincoliAgronomici_R As New AgronicaCoreAnagrafeDAL.ParticelleCatastalixVincoliAgronomici_R
        Dim objAnagrafe_VincoliAgronomici_R As New AgronicaCoreAnagrafeDAL.Anagrafe_VincoliAgronomici_R
        If objParticelleCatastali_VincoliAgronomici_R.Leggi(0, "", "", "", 0, 0, "", "", "", "", "", "", "", " analisi_testata_cod = " & analisi_testata_cod & " ", "", objParametri_Server).Rows.Count > 0 OrElse
            objAnagrafe_VincoliAgronomici_R.Leggi(0, "", 0, 0, 0, 0, 0, 0, analisi_testata_cod, "", "", objParametri_Server).Rows.Count > 0 Then
            Throw New GiasException(String.Format(Gias.ImpossibileCancellareAnalisiAggancioPUA, analisiTerreno.descrizione))
        End If

        objTestata.Cancella(analisi_testata_cod, "", objParametri_Server)

        objEntitaxTestata.CancellaEntitaXTestata(analisi_testata_cod, 0, objParametri_Server)

        Clean_Tabella_Dettagli(analisi_testata_cod, Nothing, objParametri_Server)

        Clean_Tabella_Campioni(analisi_testata_cod, Nothing, True, objParametri_Server)

        Clean_Tabella_Campioni(analisi_testata_cod, Nothing, False, objParametri_Server)


        'If analisiTerreno.entitaImprese IsNot Nothing AndAlso analisiTerreno.entitaImprese.Count > 0 Then
        '    For Each entita In analisiTerreno.entitaImprese
        '        Dim Impresa = entita.elementoAnagrafico

        '        objEntitaxTestata.Cancella2(analisi_testata_cod, enum_Entita_Analisi.Impresa,
        '                                    Impresa.partitaIva, 0, 0,
        '                                    0, 0, 0,
        '                                    "", "", "", 0, 0, "",
        '                                    "", 0,
        '                                    "",
        '                                    objParametri_Server)
        '    Next
        'End If
        'If analisiTerreno.entitaCentri IsNot Nothing AndAlso analisiTerreno.entitaCentri.Count > 0 Then
        '    For Each entita In analisiTerreno.entitaCentri
        '        Dim Centro = entita.elementoAnagrafico

        '        objEntitaxTestata.Cancella2(analisi_testata_cod, enum_Entita_Analisi.Centro,
        '                                    Centro.primaryKey.partitaIva, Centro.primaryKey.codice, 0,
        '                                    0, 0, 0,
        '                                    "", "", "", 0, 0, "",
        '                                    "", 0,
        '                                    "",
        '                                    objParametri_Server)
        '    Next
        'End If
        'If analisiTerreno.entitaCampi IsNot Nothing AndAlso analisiTerreno.entitaCampi.Count > 0 Then
        '    For Each entita In analisiTerreno.entitaCampi
        '        Dim Campo = entita.elementoAnagrafico

        '        objEntitaxTestata.Cancella2(analisi_testata_cod, enum_Entita_Analisi.Campo,
        '                                    Campo.primaryKey.centroAziendalePK.partitaIva, Campo.primaryKey.centroAziendalePK.codice,
        '                                    Campo.primaryKey.codice,
        '                                    0, 0, 0,
        '                                    "", "", "", 0, 0, "",
        '                                    "", 0,
        '                                    "",
        '                                    objParametri_Server)
        '    Next
        'End If
        'If analisiTerreno.entitaAppezzamenti IsNot Nothing AndAlso analisiTerreno.entitaAppezzamenti.Count > 0 Then
        '    For Each entita In analisiTerreno.entitaAppezzamenti
        '        Dim Appezzamento = entita.elementoAnagrafico

        '        objEntitaxTestata.Cancella2(analisi_testata_cod, enum_Entita_Analisi.Appezzamento,
        '                                    Appezzamento.primaryKey.centroAziendalePK.partitaIva, Appezzamento.primaryKey.centroAziendalePK.codice,
        '                                    0,
        '                                    Appezzamento.primaryKey.codice, 0, 0,
        '                                    "", "", "", 0, 0, "",
        '                                    "", 0,
        '                                    "",
        '                                    objParametri_Server)
        '    Next
        'End If
        'If analisiTerreno.entitaImpianti IsNot Nothing AndAlso analisiTerreno.entitaImpianti.Count > 0 Then
        '    For Each entita In analisiTerreno.entitaImpianti
        '        Dim Impianto = entita.elementoAnagrafico

        '        objEntitaxTestata.Cancella2(analisi_testata_cod, enum_Entita_Analisi.Impianto,
        '                                    Impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva, Impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice,
        '                                    0,
        '                                    Impianto.primaryKey.appezzamentoPK.codice, Impianto.primaryKey.codice, 0,
        '                                    "", "", "", 0, 0, "",
        '                                    "", 0,
        '                                    "",
        '                                    objParametri_Server)
        '    Next
        'End If
        'If analisiTerreno.entitaFabbricati IsNot Nothing AndAlso analisiTerreno.entitaFabbricati.Count > 0 Then
        '    For Each entita In analisiTerreno.entitaFabbricati
        '        Dim Fabbricato = entita.elementoAnagrafico

        '        objEntitaxTestata.Cancella2(analisi_testata_cod, enum_Entita_Analisi.Fabbricato,
        '                                    Fabbricato.primaryKey.centroAziendalePK.partitaIva, Fabbricato.primaryKey.centroAziendalePK.codice,
        '                                    0,
        '                                    0, 0, Fabbricato.primaryKey.codice,
        '                                    "", "", "", 0, 0, "",
        '                                    "", 0,
        '                                    "",
        '                                    objParametri_Server)
        '    Next
        'End If
        'If analisiTerreno.entitaParticelleCatastali IsNot Nothing AndAlso analisiTerreno.entitaParticelleCatastali.Count > 0 Then
        '    For Each entita In analisiTerreno.entitaParticelleCatastali
        '        Dim CentroAziendale = entita.elementoAnagrafico.centro
        '        Dim ParticellaCatastale = entita.elementoAnagrafico.particella
        '        objEntitaxTestata.Cancella2(analisi_testata_cod, enum_Entita_Analisi.Particella,
        '                                    "", 0, 0, 0, 0, 0,
        '                                    ParticellaCatastale.primaryKey.Prov, ParticellaCatastale.primaryKey.Com,
        '                                    ParticellaCatastale.primaryKey.Sezione, ParticellaCatastale.primaryKey.Foglio,
        '                                    ParticellaCatastale.primaryKey.Numero, ParticellaCatastale.primaryKey.Subalterno,
        '                                    "", 0, "",
        '                                    objParametri_Server)
        '    Next
        'End If

        'If analisiTerreno.dettagli IsNot Nothing Then

        '    Dim objDettaglio As New AgronicaCoreAnagrafeDAL.Analisi_Dettagli_W
        '    For Each dettaglio In analisiTerreno.dettagli

        '        objDettaglio.Cancella(analisi_testata_cod,
        '                              dettaglio.codice, dettaglio.parametro.codice,
        '                              "",
        '                              objParametri_Server)


        '        If dettaglio.campione IsNot Nothing Then
        '            Dim campione = dettaglio.campione

        '            Cancella_Campione(campione, objParametri_Server)

        '        End If
        '    Next

        'End If

        'If analisiTerreno.campioni IsNot Nothing Then

        'For Each campione In analisiTerreno.campioni

        '        Cancella_Campione(campione, objParametri_Server)

        '    Next

        'End If

        Return True

    End Function

    Private Shared Sub Clean_Tabella_Dettagli(analisi_testata_cod As Integer,
                                              lista_dettagli As List(Of AnalisiDettaglio),
                                              objParametri_Server As AgronicaCoreParametri)

        Dim objDettagli_R As New AgronicaCoreAnagrafeDAL.Analisi_Dettagli_R
        Dim objDettagli_W As New AgronicaCoreAnagrafeDAL.Analisi_Dettagli_W

        Dim xFiltroAggiuntivo As String = ""
        If lista_dettagli IsNot Nothing AndAlso lista_dettagli.Count > 0 Then
            Dim lista_analisi_dettaglio_cod = (From x In lista_dettagli
                                               Select x.codice).ToList()

            If lista_analisi_dettaglio_cod.Count > 0 Then
                xFiltroAggiuntivo = "Analisi_Dettagli.analisi_dettaglio_cod Not In (" & UtilityProvider.Agro_SQL_Save_Clausola_IN(String.Join(",", lista_analisi_dettaglio_cod), False) & ")"
            End If
        End If

        Dim DT_dettagli_to_delete = objDettagli_R.Leggi(analisi_testata_cod, 0, 0,
                                                        enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                        xFiltroAggiuntivo, "", objParametri_Server)

        For Each dettaglio_to_delete In DT_dettagli_to_delete.Rows
            objDettagli_W.Cancella(analisi_testata_cod,
                                   dettaglio_to_delete.item("analisi_dettaglio_cod"), dettaglio_to_delete.item("Analisi_Parametro_Cod"),
                                   "",
                                   objParametri_Server)
        Next

    End Sub

    Private Shared Sub Clean_Tabella_Campioni(analisi_testata_cod As Integer,
                                              lista_campioni As List(Of Campione),
                                              associati_testata As Boolean,
                                              objParametri_Server As AgronicaCoreParametri)

        Dim objCampionixDettagli_R As New AgronicaCoreAnagrafeDAL.Analisi_CampionexDet_R
        Dim objCampioni_W As New AgronicaCoreAnagrafeDAL.Analisi_Campione_W
        Dim objCampionixDettagli_W As New AgronicaCoreAnagrafeDAL.Analisi_CampionexDet_W

        Dim xFiltroAggiuntivoDelete As String = " Analisi_CampionixDettagli.Analisi_Dettaglio_Cod " & If(associati_testata, "=", "<>") & " 0"

        If lista_campioni IsNot Nothing AndAlso lista_campioni.Count > 0 Then
            Dim lista_Campione_Cod = (From x In lista_campioni
                                      Select x.codice).ToList()

            xFiltroAggiuntivoDelete &= " And Analisi_CampionixDettagli.Analisi_Campione_Cod Not In (" & UtilityProvider.Agro_SQL_Save_Clausola_IN(String.Join(",", lista_Campione_Cod), False) & ")"
        End If

        Dim DT_dettagli_to_delete = objCampionixDettagli_R.Leggi(analisi_testata_cod, 0, 0,
                                                                 enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                 xFiltroAggiuntivoDelete, "", objParametri_Server)

        For Each campione_to_delete In DT_dettagli_to_delete.Rows
            objCampioni_W.Cancella(analisi_testata_cod,
                                   campione_to_delete.item("Analisi_Campione_Cod"),
                                   objParametri_Server)

            objCampionixDettagli_W.Cancella(campione_to_delete.item("Analisi_Campione_Cod"),
                                            "",
                                            objParametri_Server)
        Next

        If lista_campioni IsNot Nothing Then
            For Each campione In lista_campioni
                If campione.codice <> 0 Then
                    Cancella_PuntoGIS(campione, objParametri_Server)
                End If
            Next
        End If

    End Sub

    Private Shared Sub Scrivi_Campione(piva As String,
                                       analisi_testata_cod As Integer,
                                       analisi_dettaglio_cod As Integer,
                                       campione As Campione,
                                       objParametri_Server As AgronicaCoreParametri)

        Dim objCampione As New AgronicaCoreAnagrafeDAL.Analisi_Campione_W
        Dim objCampionixDettagli As New AgronicaCoreAnagrafeDAL.Analisi_CampionexDet_W

        Dim objCentro As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read

        campione.KeyPiva = piva
        Dim dt_centri = objCentro.Leggi(campione.KeyPiva, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
        If dt_centri.Rows.Count > 0 Then
            campione.KeySaCod = dt_centri.Rows(0).Item("Sa_Cod")
        Else
            Throw New GiasException("IMPOSSIBILE CREARE UN CAMPIONE SENZA UN CENTRO ESISTENTE")
        End If

        'CAMPIONI
        objCampione.Scrivi(campione.codice,
                           campione.descrizione,
                           campione.longitude, campione.latitude,
                           campione.quantita, If(IsNothing(campione.unitaMisura), 0, campione.unitaMisura.codice),
                           campione.profondita, campione.profonditaMin, campione.profonditaMax,
                           campione.riferimento1, campione.riferimento2, campione.riferimento3, campione.riferimento4, campione.riferimento5,
                           campione.note, campione.KeyPiva, campione.KeySaCod, "",
                           0,
                           If(IsNothing(campione.particella), "", campione.particella.primaryKey.Prov),
                           If(IsNothing(campione.particella), "", campione.particella.primaryKey.Com),
                           If(IsNothing(campione.particella), "0", campione.particella.primaryKey.Sezione),
                           If(IsNothing(campione.particella), 0, campione.particella.primaryKey.Foglio),
                           If(IsNothing(campione.particella), 0, campione.particella.primaryKey.Numero),
                           If(IsNothing(campione.particella), "0", campione.particella.primaryKey.Subalterno),
                           Date.Now, Date.Now,
                           objParametri_Server)

        objCampionixDettagli.Scrivi(analisi_testata_cod, analisi_dettaglio_cod, campione.codice,
                                    0,
                                    Date.Now, Date.Now,
                                    objParametri_Server)

        Crea_PuntoGIS(campione, objParametri_Server)

    End Sub

    Private Shared Sub Modifica_Campione(analisi_testata_cod As Integer,
                                         analisi_dettaglio_cod As Integer,
                                         campione As Campione,
                                         objParametri_Server As AgronicaCoreParametri)

        Dim objCampione As New AgronicaCoreAnagrafeDAL.Analisi_Campione_W
        Dim objCampionixDettagli As New AgronicaCoreAnagrafeDAL.Analisi_CampionexDet_W

        objCampionixDettagli.Modifica(analisi_testata_cod, analisi_dettaglio_cod, campione.codice,
                                       0,
                                       Date.Now, Date.Now,
                                       objParametri_Server)

        objCampione.Modifica(campione.codice,
                             campione.descrizione,
                             campione.longitude, campione.latitude,
                             campione.quantita, campione.unitaMisura.codice,
                             campione.profondita, campione.profonditaMin, campione.profonditaMax,
                             campione.riferimento1, campione.riferimento2, campione.riferimento3, campione.riferimento4, campione.riferimento5,
                             campione.note, campione.KeyPiva, campione.KeySaCod, campione.KeyGrafica,
                             0,
                             If(IsNothing(campione.particella), "", campione.particella.primaryKey.Prov),
                             If(IsNothing(campione.particella), "", campione.particella.primaryKey.Com),
                             If(IsNothing(campione.particella), "0", campione.particella.primaryKey.Sezione),
                             If(IsNothing(campione.particella), 0, campione.particella.primaryKey.Foglio),
                             If(IsNothing(campione.particella), 0, campione.particella.primaryKey.Numero),
                             If(IsNothing(campione.particella), "0", campione.particella.primaryKey.Subalterno),
                             campione.dataPrelievo, campione.dataPrelievo,
                             objParametri_Server)

        Cancella_PuntoGIS(campione, objParametri_Server)
        Crea_PuntoGIS(campione, objParametri_Server)

    End Sub

    Private Shared Sub Cancella_Campione(campione As Campione,
                                         objParametri_Server As AgronicaCoreParametri)

        Dim objCampioni As New AgronicaCoreAnagrafeDAL.Analisi_Campione_W
        Dim objCampionixDettagli As New AgronicaCoreAnagrafeDAL.Analisi_CampionexDet_W

        objCampionixDettagli.Cancella(campione.codice,
                                      "",
                                      objParametri_Server)        '

        objCampioni.Cancella(campione.codice,
                             "",
                             objParametri_Server)

        Cancella_PuntoGIS(campione, objParametri_Server)

    End Sub

    Private Shared Sub Cancella_PuntoGIS(campione As Campione, objParametri_Server As AgronicaCoreParametri)

        Dim objGisElementiGrafici_R As New AgronicaCoreGisDAL.GIS_ElementiGrafici_R

        Dim dt = objGisElementiGrafici_R.LeggiWKTDaCampioneAnalisi(campione.codice, campione.KeyPiva, campione.KeySaCod, "", "", objParametri_Server)
        If dt.Rows.Count > 0 Then
            Dim objGisEntita_W As New AgronicaCoreGisDAL.GIS_Entita_W
            Dim objGisElementiGrafici_W As New AgronicaCoreGisDAL.GIS_ElementiGrafici_W

            objGisElementiGrafici_W.Cancella(objParametri_Server.PivaSuperUser,
                                             dt.Rows(0).Item("ElementoGrafico_Cod"),
                                             "",
                                             objParametri_Server)

            objGisEntita_W.Cancella(objParametri_Server.PivaSuperUser,
                                    dt.Rows(0).Item("Entita_Cod"),
                                    "",
                                    objParametri_Server)
        End If

    End Sub

    Private Shared Sub Crea_PuntoGIS(campione As Campione, objParametri_Server As AgronicaCoreParametri)

        If campione.latitude <> 0 AndAlso campione.longitude <> 0 Then
            Dim objSequenze As New Agro_Sequenze

            Dim objGisEntita As New AgronicaCoreGisDAL.GIS_Entita_W
            Dim objGisElementiGrafici As New AgronicaCoreGisDAL.GIS_ElementiGrafici_W

            'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
            Dim Entita_Cod = objSequenze.NuovoId_Tabella("gis_entita", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri_Server)
            'Dim Entita_Cod = objSequenze.Agronica_SequenzaTabelle_NuovoID("GIS_Entita", objParametri_Server)
            objGisEntita.Scrivi(objParametri_Server.PivaSuperUser, Entita_Cod,
                                enum_GIS2012_TipoEntita.CAMPIONAMENTI,
                                campione.KeyPiva, campione.KeySaCod, 0, 0, 0, If(IsNothing(campione.particella), "", campione.particella.primaryKey.Prov),
                                If(IsNothing(campione.particella), "", campione.particella.primaryKey.Com),
                                If(IsNothing(campione.particella), "0", campione.particella.primaryKey.Sezione),
                                If(IsNothing(campione.particella), 0, campione.particella.primaryKey.Foglio),
                                If(IsNothing(campione.particella), 0, campione.particella.primaryKey.Numero),
                                If(IsNothing(campione.particella), "0", campione.particella.primaryKey.Subalterno),
                                0, 0, 0, 0, 0, campione.codice,
                                "", 0,
                                AGRODATAINIZIO, AGRODATAFINE,
                                objParametri_Server)

            'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
            Dim ElementoGrafico_Cod = objSequenze.NuovoId_Tabella("gis_elementigrafici", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri_Server)
            'Dim ElementoGrafico_Cod = objSequenze.NuovoId_Tabella("GIS_ElementiGrafici", 0, Int32.MaxValue, objParametri_Server, True)
            objGisElementiGrafici.ScriviElementoGraficoBaseDaWKT(ElementoGrafico_Cod, campione.descrizione, Entita_Cod,
                                                                 enum_Gis_LayerElementiGrafici_std.CAMPIONAMENTI,
                                                                 "POINT (" & CStr(campione.longitude).Replace(",", ".") & " " & CStr(campione.latitude).Replace(",", ".") & ")",
                                                                 AGRODATAINIZIO, AGRODATAFINE, objParametri_Server)
        End If

    End Sub

    Private Shared Function Get_Laboratorio(Laboratorio As Contatto,
                                            objParametri_Server As AgronicaCoreParametri) As Integer

        Dim Analisi_Certificato_Laboratorio As Integer = 0

        If Laboratorio IsNot Nothing AndAlso Laboratorio.primaryKey IsNot Nothing AndAlso Laboratorio.primaryKey.codice <> "0" Then
            Dim objRisorseUmane As New AgronicaCoreAnagrafeDAL.Risorse_Umane_R
            Analisi_Certificato_Laboratorio = objRisorseUmane.CodRisUm_by_PivaCodContattoCodRapporto(objParametri_Server.PivaSuperUser, Laboratorio.primaryKey.codice, enum_Rapporti_Contabili_Standard.Laboratorio_Analisi, objParametri_Server)
        End If

        Return Analisi_Certificato_Laboratorio

    End Function

    Private Shared Function Get_ClasseTessitura(analisiTerreno As AnalisiTerreno,
                                                objParametri_Server As AgronicaCoreParametri,
                                                objParametri_Super_Server As AgronicaCoreParametri) As ClasseTessitura

        Dim sabbiaStr As String = ""
        Dim argillaStr As String = ""

        If analisiTerreno.dettagli IsNot Nothing Then
            For Each dettaglio In analisiTerreno.dettagli
                If dettaglio.valore1 IsNot Nothing Then
                    Select Case dettaglio.parametro.codice
                        Case enum_AnalisiParametri.AnalisiParametri_Argilla
                            argillaStr = dettaglio.valore1
                        Case enum_AnalisiParametri.AnalisiParametri_Sabbia
                            sabbiaStr = dettaglio.valore1
                    End Select
                End If
            Next
        End If

        Dim Id_ClasseTessitura As Integer = 0
        Dim sabbia As Decimal = -99
        Dim argilla As Decimal = -99

        If sabbiaStr <> "" AndAlso IsNumeric(sabbiaStr) Then
            sabbia = Agro_Math.ArrotondaVal_0(CDec(CDbl(sabbiaStr)))
        End If

        If argillaStr <> "" AndAlso IsNumeric(argillaStr) Then
            argilla = Agro_Math.ArrotondaVal_0(CDec(CDbl(argillaStr)))
        End If

        If sabbia <> -99 AndAlso argilla <> -99 Then
            Dim objParametriIngresso As New AgronicaCorePianoConcimazioneBIZ.PUA_ClassiTessitura_Analisi_input With {
                .Argilla = argilla,
                .Sabbia = sabbia,
                .Url = ""
            }

            Dim objAgroWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
            If IsNothing(objAgroWebConfig.GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione) Then
                Dim agroWs As String
                Dim objConfigurazione_Siti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                agroWs = objConfigurazione_Siti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametri_Server)
                If agroWs = "" Then
                    agroWs = objConfigurazione_Siti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametri_Super_Server)
                End If
                objParametriIngresso.Url = agroWs
            End If

            Dim objParametriUscita As AgronicaCorePianoConcimazioneBIZ.PUA_ClassiTessitura_Analisi_output
            Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
            objParametriUscita = objPC_WS.ClassiTessituraDaAnalisi(objParametriIngresso)

            If objParametriUscita IsNot Nothing AndAlso
                objParametriUscita.ListaClassiTessitura IsNot Nothing AndAlso
                objParametriUscita.ListaClassiTessitura.Count = 1 Then
                Id_ClasseTessitura = objParametriUscita.ListaClassiTessitura(0).Id_ClasseTessitura
            End If
        End If

        If Id_ClasseTessitura <> 0 Then
            Return New ClasseTessitura(Id_ClasseTessitura)
        Else
            Return Nothing
        End If

    End Function

    Private Shared Sub Check_DatiObbligatori(analisiTerreno As AnalisiTerreno)

        If IsNothing(analisiTerreno.AnalisiTipo) Then
            Throw New GiasException("Analisi Tipologia Obbligatoria.")
        End If

    End Sub

    Private Shared Sub Scrivi_EntitaXTestata(piva As String,
                                             analisi_testata_cod As Integer,
                                             analisiTerreno As AnalisiTerreno,
                                             objParametri_Server As AgronicaCoreParametri,
                                             Optional clean_tabella_entita As Boolean = False)

        Dim objEntitaxTestata As New AgronicaCoreAnagrafeDAL.Analisi_EntitaxTestata_W

        If clean_tabella_entita Then
            objEntitaxTestata.CancellaEntitaXTestata(analisi_testata_cod, 0, objParametri_Server)
        End If

        If analisiTerreno.entitaImprese IsNot Nothing AndAlso analisiTerreno.entitaImprese.Count > 0 Then
            For Each entita In analisiTerreno.entitaImprese
                Dim Impresa = entita.elementoAnagrafico
                objEntitaxTestata.Scrivi2(analisi_testata_cod, enum_Entita_Analisi.Impresa,
                                          Impresa.partitaIva, 0, 0,
                                          0, 0, 0,
                                          "", "", "", 0, 0, "",
                                          0, 0, 0,
                                          AGRODATAINIZIO, AGRODATAFINE,
                                          objParametri_Server)
            Next
        End If
        If analisiTerreno.entitaCentri IsNot Nothing AndAlso analisiTerreno.entitaCentri.Count > 0 Then
            For Each entita In analisiTerreno.entitaCentri
                Dim Centro = entita.elementoAnagrafico
                objEntitaxTestata.Scrivi2(analisi_testata_cod, enum_Entita_Analisi.Centro,
                                          Centro.primaryKey.partitaIva, Centro.primaryKey.codice, 0,
                                          0, 0, 0,
                                          "", "", "", 0, 0, "",
                                          "",
                                          0, 0,
                                          AGRODATAINIZIO, AGRODATAFINE,
                                          objParametri_Server)
            Next
        End If
        If analisiTerreno.entitaCampi IsNot Nothing AndAlso analisiTerreno.entitaCampi.Count > 0 Then
            For Each entita In analisiTerreno.entitaCampi
                Dim Campo = entita.elementoAnagrafico
                objEntitaxTestata.Scrivi2(analisi_testata_cod, enum_Entita_Analisi.Campo,
                                          Campo.primaryKey.centroAziendalePK.partitaIva, Campo.primaryKey.centroAziendalePK.codice,
                                          Campo.primaryKey.codice,
                                          0, 0, 0,
                                          "", "", "", 0, 0, "",
                                          "",
                                          0, 0,
                                          AGRODATAINIZIO, AGRODATAFINE,
                                          objParametri_Server)
            Next
        End If
        If analisiTerreno.entitaAppezzamenti IsNot Nothing AndAlso analisiTerreno.entitaAppezzamenti.Count > 0 Then
            For Each entita In analisiTerreno.entitaAppezzamenti
                Dim Appezzamento = entita.elementoAnagrafico
                objEntitaxTestata.Scrivi2(analisi_testata_cod, enum_Entita_Analisi.Appezzamento,
                                          Appezzamento.primaryKey.centroAziendalePK.partitaIva, Appezzamento.primaryKey.centroAziendalePK.codice,
                                          0,
                                          Appezzamento.primaryKey.codice, 0, 0,
                                          "", "", "", 0, 0, "",
                                          "", 0, 0,
                                          AGRODATAINIZIO, AGRODATAFINE,
                                          objParametri_Server)
            Next
        End If
        If analisiTerreno.entitaImpianti IsNot Nothing AndAlso analisiTerreno.entitaImpianti.Count > 0 Then
            For Each entita In analisiTerreno.entitaImpianti
                Dim Impianto = entita.elementoAnagrafico
                objEntitaxTestata.Scrivi2(analisi_testata_cod,
                                          enum_Entita_Analisi.Impianto,
                                          Impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva, Impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice,
                                          0,
                                          Impianto.primaryKey.appezzamentoPK.codice, Impianto.primaryKey.codice, 0,
                                          "", "", "", 0, 0, "",
                                          "", 0, 0,
                                          AGRODATAINIZIO, AGRODATAFINE,
                                          objParametri_Server)
            Next
        End If
        If analisiTerreno.entitaFabbricati IsNot Nothing AndAlso analisiTerreno.entitaFabbricati.Count > 0 Then
            For Each entita In analisiTerreno.entitaFabbricati
                Dim Fabbricato = entita.elementoAnagrafico
                objEntitaxTestata.Scrivi2(analisi_testata_cod, enum_Entita_Analisi.Fabbricato,
                                          Fabbricato.primaryKey.centroAziendalePK.partitaIva, Fabbricato.primaryKey.centroAziendalePK.codice,
                                          0,
                                          0, 0, Fabbricato.primaryKey.codice,
                                          "", "", "", 0, 0, "",
                                          "", 0, 0,
                                          AGRODATAINIZIO, AGRODATAFINE,
                                          objParametri_Server)
            Next
        End If

        If analisiTerreno.entitaParticelleCatastali IsNot Nothing AndAlso analisiTerreno.entitaParticelleCatastali.Count > 0 Then

            Dim entitaDistinte = analisiTerreno.entitaParticelleCatastali.
                                GroupBy(Function(e) New With {
                                    Key .Prov = e.elementoAnagrafico.particella.primaryKey.Prov,
                                    Key .Com = e.elementoAnagrafico.particella.primaryKey.Com,
                                    Key .Sez = e.elementoAnagrafico.particella.primaryKey.Sezione,
                                    Key .Fog = e.elementoAnagrafico.particella.primaryKey.Foglio,
                                    Key .Num = e.elementoAnagrafico.particella.primaryKey.Numero,
                                    Key .Sub = e.elementoAnagrafico.particella.primaryKey.Subalterno
                                }).
                                Select(Function(g) g.First())

            For Each entita In entitaDistinte
                Dim CentroAziendale = entita.elementoAnagrafico.centro
                Dim ParticellaCatastale = entita.elementoAnagrafico.particella
                objEntitaxTestata.Scrivi2(analisi_testata_cod, enum_Entita_Analisi.Particella,
                                          CentroAziendale.partitaIva, CentroAziendale.codice, 0, 0, 0, 0,
                                          ParticellaCatastale.primaryKey.Prov, ParticellaCatastale.primaryKey.Com,
                                          ParticellaCatastale.primaryKey.Sezione, ParticellaCatastale.primaryKey.Foglio,
                                          ParticellaCatastale.primaryKey.Numero, ParticellaCatastale.primaryKey.Subalterno,
                                          "", 0, 0,
                                          AGRODATAINIZIO, AGRODATAFINE,
                                          objParametri_Server)
            Next
        End If

    End Sub

    Private Shared Sub Scrivi_Certificato(analisiTerreno As AnalisiTerreno,
                                          objParametri_Server As AgronicaCoreParametri)

        Dim objSequenze As New Agro_Sequenze
        Dim objCertificato As New AgronicaCoreAnagrafeDAL.Analisi_Certificato_W

        Dim Analisi_Certificato_Laboratorio As String = Get_Laboratorio(analisiTerreno.laboratorio, objParametri_Server)

        If (analisiTerreno.certificatoAnalisi IsNot Nothing AndAlso analisiTerreno.certificatoAnalisi.codice <> 0) Then
            objCertificato.Modifica(analisiTerreno.certificatoAnalisi.codice,
                                    analisiTerreno.certificatoAnalisi.numero_certificato,
                                    analisiTerreno.validita.inizio,
                                    analisiTerreno.validita.fine,
                                    Analisi_Certificato_Laboratorio,
                                    If(analisiTerreno.analisiTipologia IsNot Nothing, analisiTerreno.analisiTipologia.codice, 0),
                                    "", "", "",
                                    "", "", "",
                                    "", "", "",
                                    AGRODATAINIZIO, "", "",
                                    0,
                                    AGRODATAINIZIO, AGRODATAFINE,
                                    objParametri_Server)

        Else
            If (analisiTerreno.certificatoAnalisi IsNot Nothing AndAlso analisiTerreno.certificatoAnalisi.numero_certificato <> "") OrElse
                Analisi_Certificato_Laboratorio <> 0 OrElse
                (analisiTerreno.analisiTipologia IsNot Nothing AndAlso analisiTerreno.analisiTipologia.codice <> 0) Then
                Dim certificato_cod As Integer = objSequenze.NuovoId_Tabella("ANALISI_CERTIFICATO", 0, 20000000, objParametri_Server)
                objCertificato.Scrivi(certificato_cod,
                                              If(analisiTerreno.certificatoAnalisi IsNot Nothing, analisiTerreno.certificatoAnalisi.numero_certificato, ""),
                                              Date.Now, AGRODATAFINE,
                                              Analisi_Certificato_Laboratorio,
                                              If(analisiTerreno.analisiTipologia IsNot Nothing, analisiTerreno.analisiTipologia.codice, 0),
                                              "", "", "",
                                              "", "", "",
                                              "", "", "",
                                              AGRODATAINIZIO, "", "",
                                              Date.Now, 0,
                                              AGRODATAINIZIO, AGRODATAFINE,
                                              objParametri_Server)

                If (analisiTerreno.certificatoAnalisi Is Nothing) Then
                    analisiTerreno.certificatoAnalisi = New CertificatoAnalisi(certificato_cod)
                Else
                    analisiTerreno.certificatoAnalisi.codice = certificato_cod
                End If
            End If
        End If
    End Sub
End Class

Public Class Analisi_Modello_Utility
    Public Function usaAnalisiTerrenoNG(objParametri_Utenti As AgronicaCoreParametri) As Boolean

        Dim utentiImpostazioniDal As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim rowsModalitaAnalisiTerreno = utentiImpostazioniDal.LeggiImpostazioneScalare(enum_Impostazioni_Utenti.SUPERUSER_Mod_Analisi_Terreno, objParametri_Utenti.UtenteUsername, objParametri_Utenti)
        Dim modalitaAnalisiTerreno As Object = ""

        If (rowsModalitaAnalisiTerreno IsNot Nothing AndAlso rowsModalitaAnalisiTerreno.Count > 0) Then
            modalitaAnalisiTerreno = rowsModalitaAnalisiTerreno.First().Item("Impostazione_Valore_1")
        End If

        Select Case modalitaAnalisiTerreno
            Case "2"  'NEW
                Return True
            Case Else 'OLD
                Return False
        End Select
    End Function

    Public Function Link_Pagina_AnalisiTerrenoNG(Piva As String,
                                                 ParametriAnalisiTerrenoNG As ParametriAnalisiTerrenoNG) As String

        Try

            Dim objAgenda As New Parametri_ObjParametriAgenda_NG

            objAgenda.Pagina_Richiesta = ParametriAnalisiTerrenoNG.Pagina_Richiesta
            objAgenda.Pagina_Provenienza = ParametriAnalisiTerrenoNG.Pagina_SitoOrigine
            objAgenda.Sito_Provenienza = ParametriAnalisiTerrenoNG.SitoOrigine
            objAgenda.Piva = ParametriAnalisiTerrenoNG.Piva
            objAgenda.TipoOperazioneDB = ParametriAnalisiTerrenoNG.Tipo_Operazione
            objAgenda.GenericObj_string = If(ParametriAnalisiTerrenoNG.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura, "", JsonConvert.SerializeObject(ParametriAnalisiTerrenoNG))
            objAgenda.QueryStringFiltrino = "?seFrame=1"

            objAgenda.Salva()

            Return RedirectGestione.IndirizzoCompleto_SitoGiasNG_PassandoDirettamente_ParametriAgenda_NG(ParametriAnalisiTerrenoNG.SitoOrigine, objAgenda)

        Catch ex As Exception

            Throw New Exception("Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True))

        End Try

    End Function

    Public Function GeneraAnalisiTerrenoPerPianoConcimazione(Piva As String,
                                                             Sa_Cod As Integer,
                                                             DataInizio As Date,
                                                             DataFine As Date,
                                                             DescrizioneAnalisi As String,
                                                                Optional Sabbia As Decimal = 0,
                                                                Optional Argilla As Decimal = 0,
                                                                Optional Limo As Decimal = 0,
                                                                Optional Ph As Decimal = 0,
                                                                Optional CalcTot As Decimal = 0,
                                                                Optional CalcAtt As Decimal = 0,
                                                                Optional SO As Decimal = 0,
                                                                Optional N_az As Decimal = 0,
                                                                Optional P2O5 As Decimal = 0,
                                                                Optional P As Decimal = 0,
                                                                Optional K2O As Decimal = 0,
                                                                Optional K As Decimal = 0,
                                                                Optional CN As Decimal = 0,
                                                                Optional Mg As Decimal = 0,
                                                                Optional CSC As Decimal = 0
                                                                ) As AnalisiTerreno
        Dim analisiTerreno As New AnalisiTerreno

        '----- Dati Testata
        analisiTerreno = New AnalisiTerreno(0, DescrizioneAnalisi) With {
            .validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(DataInizio, DataFine),
            .AnalisiTipo = New AgronicaCoreModelsSTD.metaschema.AnalisiTipo(enum_AnalisiTipo.Analisi_Terreno),
            .analisiTipologia = New AnalisiTipologia(enum_AnalisiTipologia_Schema.Piano_Concimazione)
        }

        '----- Dati EntitaxTestata
        If Sa_Cod = 0 Then
            analisiTerreno.entitaImprese = New List(Of AnalisiEntita(Of AgronicaCoreModelsSTD.anagrafiche.Impresa))
            Dim entitaImpresa As New AnalisiEntita(Of AgronicaCoreModelsSTD.anagrafiche.Impresa) With {
                .elementoAnagrafico = New AgronicaCoreModelsSTD.anagrafiche.Impresa() With {
                    .partitaIva = Piva
                }
            }
            analisiTerreno.entitaImprese.Add(entitaImpresa)
        Else
            analisiTerreno.entitaCentri = New List(Of AnalisiEntita(Of AgronicaCoreModelsSTD.anagrafiche.CentroAziendale))
            Dim entitaCentro As New AnalisiEntita(Of AgronicaCoreModelsSTD.anagrafiche.CentroAziendale) With {
                .elementoAnagrafico = New AgronicaCoreModelsSTD.anagrafiche.CentroAziendale(New AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK(Sa_Cod, Piva))
            }

            analisiTerreno.entitaCentri.Add(entitaCentro)
        End If

        '----- Dati Dettagli
        analisiTerreno.dettagli = New List(Of AnalisiDettaglio)

        'Sabbia
        Dim AnalisiDettaglio As New AnalisiDettaglio() With {
            .parametro = New AgronicaCoreModelsSTD.metaschema.AnalisiParametro(enum_AnalisiParametri.AnalisiParametri_Sabbia),'objAnalisiTerrenoUtility.Get_Parametro(enum_AnalisiParametri.AnalisiParametri_Sabbia, objParametri_Server),
            .valore1 = Sabbia
        }
        analisiTerreno.dettagli.Add(AnalisiDettaglio)

        'Limo
        AnalisiDettaglio = New AnalisiDettaglio() With {
            .parametro = New AgronicaCoreModelsSTD.metaschema.AnalisiParametro(enum_AnalisiParametri.AnalisiParametri_Limo),
            .valore1 = Limo
        }
        analisiTerreno.dettagli.Add(AnalisiDettaglio)

        'Argilla
        AnalisiDettaglio = New AnalisiDettaglio() With {
            .parametro = New AgronicaCoreModelsSTD.metaschema.AnalisiParametro(enum_AnalisiParametri.AnalisiParametri_Argilla),
            .valore1 = Argilla
        }
        analisiTerreno.dettagli.Add(AnalisiDettaglio)

        'PH
        AnalisiDettaglio = New AnalisiDettaglio() With {
        .parametro = New AgronicaCoreModelsSTD.metaschema.AnalisiParametro(enum_AnalisiParametri.AnalisiParametri_pH),
            .valore1 = Ph
        }
        analisiTerreno.dettagli.Add(AnalisiDettaglio)

        'CalcTot
        AnalisiDettaglio = New AnalisiDettaglio() With {
            .parametro = New AgronicaCoreModelsSTD.metaschema.AnalisiParametro(enum_AnalisiParametri.AnalisiParametri_CaCO3),
            .valore1 = CalcTot
        }
        analisiTerreno.dettagli.Add(AnalisiDettaglio)

        'CalcAtt
        AnalisiDettaglio = New AnalisiDettaglio() With {
            .parametro = New AgronicaCoreModelsSTD.metaschema.AnalisiParametro(enum_AnalisiParametri.AnalisiParametri_CaCO3_Attivo),
            .valore1 = CalcAtt
        }
        analisiTerreno.dettagli.Add(AnalisiDettaglio)

        'SO
        AnalisiDettaglio = New AnalisiDettaglio() With {
            .parametro = New AgronicaCoreModelsSTD.metaschema.AnalisiParametro(enum_AnalisiParametri.AnalisiParametri_SostanzaOrganica),
            .valore1 = SO
        }
        analisiTerreno.dettagli.Add(AnalisiDettaglio)

        'N_az
        AnalisiDettaglio = New AnalisiDettaglio() With {
            .parametro = New AgronicaCoreModelsSTD.metaschema.AnalisiParametro(enum_AnalisiParametri.AnalisiParametri_Ntot),
            .valore1 = N_az
        }
        analisiTerreno.dettagli.Add(AnalisiDettaglio)

        'P205
        AnalisiDettaglio = New AnalisiDettaglio() With {
            .parametro = New AgronicaCoreModelsSTD.metaschema.AnalisiParametro(enum_AnalisiParametri.AnalisiParametri_P2O5_assimilabile),
            .valore1 = P2O5
        }
        analisiTerreno.dettagli.Add(AnalisiDettaglio)

        'P
        AnalisiDettaglio = New AnalisiDettaglio() With {
            .parametro = New AgronicaCoreModelsSTD.metaschema.AnalisiParametro(enum_AnalisiParametri.AnalisiParametri_P_assimilabile),
            .valore1 = P
        }
        analisiTerreno.dettagli.Add(AnalisiDettaglio)

        'K20
        AnalisiDettaglio = New AnalisiDettaglio() With {
            .parametro = New AgronicaCoreModelsSTD.metaschema.AnalisiParametro(enum_AnalisiParametri.AnalisiParametri_K2O_scambiabile),
            .valore1 = K2O
        }
        analisiTerreno.dettagli.Add(AnalisiDettaglio)

        'K
        AnalisiDettaglio = New AnalisiDettaglio() With {
            .parametro = New AgronicaCoreModelsSTD.metaschema.AnalisiParametro(enum_AnalisiParametri.AnalisiParametri_K_scambiabile),
            .valore1 = K
        }
        analisiTerreno.dettagli.Add(AnalisiDettaglio)

        'CN
        AnalisiDettaglio = New AnalisiDettaglio() With {
            .parametro = New AgronicaCoreModelsSTD.metaschema.AnalisiParametro(enum_AnalisiParametri.AnalisiParametri_RapportoCN),
            .valore1 = CN
        }
        analisiTerreno.dettagli.Add(AnalisiDettaglio)

        'MG
        AnalisiDettaglio = New AnalisiDettaglio() With {
            .parametro = New AgronicaCoreModelsSTD.metaschema.AnalisiParametro(enum_AnalisiParametri.AnalisiParametri_Mg_assimilabile),
            .valore1 = Mg
        }
        analisiTerreno.dettagli.Add(AnalisiDettaglio)

        'CSC
        AnalisiDettaglio = New AnalisiDettaglio() With {
            .parametro = New AgronicaCoreModelsSTD.metaschema.AnalisiParametro(enum_AnalisiParametri.AnalisiParametri_CSC),
            .valore1 = CSC
        }
        analisiTerreno.dettagli.Add(AnalisiDettaglio)

        Return analisiTerreno


    End Function

End Class
