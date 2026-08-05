Imports System.Xml
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class XML_Pubblico_Anagrafe


    '########################################################################################
    Public Function Xml_Impresa(ByRef XmlDoc As System.Xml.XmlDocument,
                            ByVal Tipo_Operazione As String,
                            ByVal Piva As String,
                            ByVal Rag_Soc As String,
                            ByVal Cuaa As String,
                            ByVal Codice_Fiscale As String,
                            ByVal Cod_Socio As String,
                            ByVal Sup_Totale As String,
                            ByVal Piva_Padre As String,
                            ByVal Tipo_Gerarchia As String,
                            ByVal Titolo_Possesso As String,
                            ByVal Validita_Inizio As String,
                            ByVal Validita_Fine As String,
                            ByVal i_indirizzo As String, ByVal i_frazione As String,
                            ByVal i_cap As String, ByVal i_comune As String,
                            ByVal i_provincia As String, ByVal i_stato As String,
                            ByVal i_note_indirizzo As String, ByVal i_codice_istat_comune As String,
                            ByVal i_codice_istat_provincia As String, ByVal legale_rappresentante As String,
                            ByVal lr_codice_fiscale As String, ByVal lr_sesso As String,
                            ByVal lr_validita_inizio As String, ByVal lr_validita_fine As String,
                            ByVal lr_indirizzo As String, ByVal lr_frazione As String,
                            ByVal lr_cap As String, ByVal lr_comune As String,
                            ByVal lr_provincia As String, ByVal lr_stato As String,
                            ByVal lr_note_indirizzo As String, ByVal lr_codice_istat_comune As String,
                            ByVal lr_codice_istat_provincia As String, ByVal lr_nascita_data As String,
                            ByVal lr_nascita_comune As String, ByVal lr_nascita_provincia As String,
                            ByVal lr_nascita_istat_comune As String, ByVal lr_nascita_istat_provincia As String,
                            ByVal lr_documenti As String, ByVal lr_rubrica_1 As String,
                            ByVal lr_rubrica_2 As String, ByVal lr_rubrica_3 As String,
                            ByVal lr_rubrica_4 As String, ByVal lr_rubrica_5 As String,
                            ByVal cf_tecnico_referente As String,
                            ByVal codice_cliente As String,
                            ByVal codice_fornitore As String,
                            ByVal codice_fornitore_2 As String,
                            ByVal codice_fornitore_3 As String,
                            ByVal i_Chiave_Cliente As String,
                                ByVal Codice_GGN As String,
                                ByVal data_iscrizione_libro_soci As String
                                ) As System.Xml.XmlElement

        Dim XmlTxt As System.Xml.XmlElement

        XmlTxt = XmlDoc.CreateElement("Impresa")

        XmlTxt.SetAttribute("tipo_operazione", Tipo_Operazione)
        XmlTxt.SetAttribute("partita_iva", Piva)
        XmlTxt.SetAttribute("ragione_sociale", Rag_Soc)
        XmlTxt.SetAttribute("codice_cuaa", Cuaa)
        XmlTxt.SetAttribute("codice_fiscale", Codice_Fiscale)
        XmlTxt.SetAttribute("codice_socio", Cod_Socio)
        XmlTxt.SetAttribute("codice_ggn", Codice_GGN)
        XmlTxt.SetAttribute("data_iscrizione_libro_soci", data_iscrizione_libro_soci)

        XmlTxt.SetAttribute("sup_totale", Sup_Totale)
        XmlTxt.SetAttribute("partita_iva_padre", Piva_Padre)
        XmlTxt.SetAttribute("tipo_impresa_gerarchia", Tipo_Gerarchia)
        XmlTxt.SetAttribute("titolo_possesso", Titolo_Possesso)
        XmlTxt.SetAttribute("validita_inizio", Validita_Inizio)
        XmlTxt.SetAttribute("validita_fine", Validita_Fine)

        XmlTxt.SetAttribute("i_indirizzo", i_indirizzo)
        XmlTxt.SetAttribute("i_frazione", i_frazione)
        XmlTxt.SetAttribute("i_cap", i_cap)
        XmlTxt.SetAttribute("i_comune", i_comune)
        XmlTxt.SetAttribute("i_provincia", i_provincia)
        XmlTxt.SetAttribute("i_stato", i_stato)
        XmlTxt.SetAttribute("i_note_indirizzo", i_note_indirizzo)
        XmlTxt.SetAttribute("i_codice_istat_comune", i_codice_istat_comune)
        XmlTxt.SetAttribute("i_codice_istat_provincia", i_codice_istat_provincia)

        XmlTxt.SetAttribute("legale_rappresentante", legale_rappresentante)
        XmlTxt.SetAttribute("lr_codice_fiscale", lr_codice_fiscale)
        XmlTxt.SetAttribute("lr_sesso", lr_sesso)
        XmlTxt.SetAttribute("lr_validita_inizio", lr_validita_inizio)
        XmlTxt.SetAttribute("lr_validita_fine", lr_validita_fine)
        XmlTxt.SetAttribute("lr_indirizzo", lr_indirizzo)
        XmlTxt.SetAttribute("lr_frazione", lr_frazione)
        XmlTxt.SetAttribute("lr_cap", lr_cap)
        XmlTxt.SetAttribute("lr_comune", lr_comune)
        XmlTxt.SetAttribute("lr_provincia", lr_provincia)
        XmlTxt.SetAttribute("lr_stato", lr_stato)
        XmlTxt.SetAttribute("lr_note_indirizzo", lr_note_indirizzo)
        XmlTxt.SetAttribute("lr_codice_istat_comune", lr_codice_istat_comune)
        XmlTxt.SetAttribute("lr_codice_istat_provincia", lr_codice_istat_provincia)
        XmlTxt.SetAttribute("lr_nascita_data", lr_nascita_data)
        XmlTxt.SetAttribute("lr_nascita_comune", lr_nascita_comune)
        XmlTxt.SetAttribute("lr_nascita_provincia", lr_nascita_provincia)
        XmlTxt.SetAttribute("lr_nascita_istat_comune", lr_nascita_istat_comune)
        XmlTxt.SetAttribute("lr_nascita_istat_provincia", lr_nascita_istat_provincia)

        XmlTxt.SetAttribute("lr_documenti", lr_documenti)
        XmlTxt.SetAttribute("lr_rubrica_1", lr_rubrica_1)
        XmlTxt.SetAttribute("lr_rubrica_2", lr_rubrica_2)
        XmlTxt.SetAttribute("lr_rubrica_3", lr_rubrica_3)
        XmlTxt.SetAttribute("lr_rubrica_4", lr_rubrica_4)
        XmlTxt.SetAttribute("lr_rubrica_5", lr_rubrica_5)

        XmlTxt.SetAttribute("cf_tecnico_referente", cf_tecnico_referente)

        XmlTxt.SetAttribute("codice_cliente", codice_cliente)
        XmlTxt.SetAttribute("codice_fornitore", codice_fornitore)
        XmlTxt.SetAttribute("codice_fornitore_2", codice_fornitore_2)
        XmlTxt.SetAttribute("codice_fornitore_3", codice_fornitore_3)

        XmlTxt.SetAttribute("i_chiave_cliente", i_Chiave_Cliente)

        Return XmlTxt

    End Function

    '########################################################################################
    Public Function Xml_CentroAziendale(ByRef XmlDoc As System.Xml.XmlDocument,
                                    ByVal Tipo_Operazione As String,
                                    ByVal Codice As String,
                                    ByVal Nome_Centro As String,
                                    ByVal Titolo_Possesso As String,
                                    ByVal Sup_Bosco As String,
                                    ByVal Sup_Prati As String,
                                    ByVal Cod_Operatore As String,
                                    ByVal Tipo_Attivita As String,
                                    ByVal Validita_Inizio As String,
                                    ByVal Validita_Fine As String,
                                    ByVal c_indirizzo As String,
                                    ByVal c_frazione As String,
                                    ByVal c_cap As String,
                                    ByVal c_comune As String,
                                    ByVal c_provincia As String, ByVal c_stato As String,
                                    ByVal c_note_indirizzo As String,
                                    ByVal c_codice_istat_comune As String,
                                    ByVal c_codice_istat_provincia As String,
                                    ByVal c_rubrica_1 As String, ByVal c_rubrica_2 As String,
                                    ByVal c_rubrica_3 As String, ByVal c_rubrica_4 As String,
                                    ByVal c_rubrica_5 As String, ByVal c_rubrica_6 As String,
                                    ByVal c_rubrica_7 As String, ByVal c_rubrica_8 As String,
                                    ByVal c_rubrica_9 As String,
                                    ByVal c_Chiave_Cliente As String) As System.Xml.XmlElement

        Dim XmlTxt As System.Xml.XmlElement

        XmlTxt = XmlDoc.CreateElement("CentroAziendale")

        XmlTxt.SetAttribute("tipo_operazione", Tipo_Operazione)
        XmlTxt.SetAttribute("codice_centro", Codice)
        XmlTxt.SetAttribute("nome_centro", Nome_Centro)
        XmlTxt.SetAttribute("titolo_possesso", Titolo_Possesso)
        XmlTxt.SetAttribute("sup_bosco", Sup_Bosco)
        XmlTxt.SetAttribute("sup_prati", Sup_Prati)
        XmlTxt.SetAttribute("codice_operatore", Cod_Operatore)
        XmlTxt.SetAttribute("tipo_attivita", Tipo_Attivita)
        XmlTxt.SetAttribute("validita_inizio", Validita_Inizio)
        XmlTxt.SetAttribute("validita_fine", Validita_Fine)

        XmlTxt.SetAttribute("c_indirizzo", c_indirizzo)
        XmlTxt.SetAttribute("c_frazione", c_frazione)
        XmlTxt.SetAttribute("c_cap", c_cap)
        XmlTxt.SetAttribute("c_comune", c_comune)
        XmlTxt.SetAttribute("c_provincia", c_provincia)
        XmlTxt.SetAttribute("c_stato", c_stato)
        XmlTxt.SetAttribute("c_note_indirizzo", c_note_indirizzo)
        XmlTxt.SetAttribute("c_codice_istat_comune", c_codice_istat_comune)
        XmlTxt.SetAttribute("c_codice_istat_provincia", c_codice_istat_provincia)

        XmlTxt.SetAttribute("c_rubrica_1", c_rubrica_1)
        XmlTxt.SetAttribute("c_rubrica_2", c_rubrica_2)
        XmlTxt.SetAttribute("c_rubrica_3", c_rubrica_3)
        XmlTxt.SetAttribute("c_rubrica_4", c_rubrica_4)
        XmlTxt.SetAttribute("c_rubrica_5", c_rubrica_5)
        XmlTxt.SetAttribute("c_rubrica_6", c_rubrica_6)
        XmlTxt.SetAttribute("c_rubrica_7", c_rubrica_7)
        XmlTxt.SetAttribute("c_rubrica_8", c_rubrica_8)
        XmlTxt.SetAttribute("c_rubrica_9", c_rubrica_9)

        XmlTxt.SetAttribute("c_chiave_cliente", c_Chiave_Cliente)

        Return XmlTxt

    End Function

    '########################################################################################
    Public Function Xml_Particella(ByRef XmlDoc As System.Xml.XmlDocument,
                               ByVal Tipo_Operazione As String,
                               ByVal Codice As String,
                               ByVal Cod_Particella_Azi As String,
                               ByVal p_codice_istat_comune As String,
                               ByVal p_codice_istat_provincia As String,
                               ByVal Sezione As String,
                               ByVal Foglio As String,
                               ByVal Numero As String,
                               ByVal Subalterno As String,
                               ByVal Partita_Catastale As String,
                               ByVal Ettari As String,
                               ByVal Are As String,
                               ByVal Centiare As String,
                               ByVal Titolo_Possesso As String,
                               ByVal Titolo_Possesso_Des As String,
                               ByVal Sup_Condotta As String,
                               ByVal Qualita_Catasto_Cod As String,
                               ByVal Classe As String,
                               ByVal Reddito_Dominicale As String,
                               ByVal Reddito_Agrario As String,
                               ByVal Validita_Inizio As String,
                               ByVal Validita_Fine As String) As System.Xml.XmlElement

        Dim XmlTxt As System.Xml.XmlElement

        XmlTxt = XmlDoc.CreateElement("Particella")

        XmlTxt.SetAttribute("tipo_operazione", Tipo_Operazione)
        XmlTxt.SetAttribute("codice_particella", Codice)
        XmlTxt.SetAttribute("codice_particella_azienda", Cod_Particella_Azi)
        XmlTxt.SetAttribute("p_codice_istat_comune", p_codice_istat_comune)
        XmlTxt.SetAttribute("p_codice_istat_provincia", p_codice_istat_provincia)
        XmlTxt.SetAttribute("sezione", Sezione)
        XmlTxt.SetAttribute("foglio", Foglio)
        XmlTxt.SetAttribute("numero", Numero)
        XmlTxt.SetAttribute("subalterno", Subalterno)
        XmlTxt.SetAttribute("partita_catastale", Partita_Catastale)
        XmlTxt.SetAttribute("ettari", Ettari)
        XmlTxt.SetAttribute("are", Are)
        XmlTxt.SetAttribute("centiare", Centiare)
        XmlTxt.SetAttribute("titolo_possesso", Titolo_Possesso)
        XmlTxt.SetAttribute("titolo_possesso_des", Titolo_Possesso_Des)
        XmlTxt.SetAttribute("sup_condotta", Sup_Condotta)
        XmlTxt.SetAttribute("qualita_catasto_codice", Qualita_Catasto_Cod)
        XmlTxt.SetAttribute("classe", Classe)
        XmlTxt.SetAttribute("reddito_dominicale", Reddito_Dominicale)
        XmlTxt.SetAttribute("reddito_agrario", Reddito_Agrario)
        XmlTxt.SetAttribute("validita_inizio_possesso", Validita_Inizio)
        XmlTxt.SetAttribute("validita_fine_possesso", Validita_Fine)

        Return XmlTxt

    End Function

    '########################################################################################
    Public Function Xml_Fabbricato(ByRef XmlDoc As System.Xml.XmlDocument,
                               ByVal Tipo_Operazione As String,
                               ByVal Codice As String,
                               ByVal Nome_Fabbricato As String,
                               ByVal Tipo_Fabbricato_Cod As String,
                               ByVal Vol_Convenzionale As String,
                               ByVal Vol_Conversione As String,
                               ByVal Vol_Biologico As String,
                               ByVal Titolo_Possesso As String,
                               ByVal Regolamento_Cod As String,
                               ByVal Particella_Codice As String,
                               ByVal Validita_Inizio As String,
                               ByVal Validita_Fine As String,
                               ByVal f_indirizzo As String, ByVal f_frazione As String,
                               ByVal f_cap As String, ByVal f_comune As String,
                               ByVal f_provincia As String, ByVal f_stato As String,
                               ByVal f_note_indirizzo As String, ByVal f_codice_istat_comune As String,
                               ByVal f_codice_istat_provincia As String) As System.Xml.XmlElement



        Dim XmlTxt As System.Xml.XmlElement

        XmlTxt = XmlDoc.CreateElement("Fabbricato")

        XmlTxt.SetAttribute("tipo_operazione", Tipo_Operazione)
        XmlTxt.SetAttribute("codice_fabbricato", Codice)
        XmlTxt.SetAttribute("fabbricato_denominazione", Nome_Fabbricato)
        XmlTxt.SetAttribute("tipo_fabbricato_codice", Tipo_Fabbricato_Cod)
        XmlTxt.SetAttribute("volume_convenzionale", Vol_Convenzionale)
        XmlTxt.SetAttribute("volume_conversione", Vol_Conversione)
        XmlTxt.SetAttribute("volume_biologico", Vol_Biologico)
        XmlTxt.SetAttribute("titolo_possesso", Titolo_Possesso)
        XmlTxt.SetAttribute("codice_regolamento", Regolamento_Cod)
        XmlTxt.SetAttribute("codice_particella", Particella_Codice)
        XmlTxt.SetAttribute("validita_inizio", Validita_Inizio)
        XmlTxt.SetAttribute("validita_fine", Validita_Fine)

        XmlTxt.SetAttribute("f_indirizzo", f_indirizzo)
        XmlTxt.SetAttribute("f_frazione", f_frazione)
        XmlTxt.SetAttribute("f_cap", f_cap)
        XmlTxt.SetAttribute("f_comune", f_comune)
        XmlTxt.SetAttribute("f_provincia", f_provincia)
        XmlTxt.SetAttribute("f_stato", f_stato)
        XmlTxt.SetAttribute("f_note_indirizzo", f_note_indirizzo)
        XmlTxt.SetAttribute("f_codice_istat_comune", f_codice_istat_comune)
        XmlTxt.SetAttribute("f_codice_istat_provincia", f_codice_istat_provincia)

        Return XmlTxt

    End Function

    '########################################################################################
    Public Function Xml_Campo(ByRef XmlDoc As System.Xml.XmlDocument,
                          ByVal Tipo_Operazione As String,
                          ByVal Codice As String,
                          ByVal Campo_Tipo_Cod As String,
                          ByVal Campo_Tipo_Des As String,
                          ByVal Nome_Campo As String,
                          ByVal Gru_Cod As String,
                          ByVal Veg_Cod As String,
                          ByVal Validita_Inizio As String,
                          ByVal Validita_Fine As String,
                          ByVal Cam_Chiave_Cliente As String) As System.Xml.XmlElement


        Dim XmlTxt As System.Xml.XmlElement

        XmlTxt = XmlDoc.CreateElement("Campo")

        'se ho codice=-1 inserisco solo il nodo, senza attributi..
        If Codice <> -1 Then
            XmlTxt.SetAttribute("tipo_operazione", Tipo_Operazione)
            XmlTxt.SetAttribute("codice_campo", Codice)
            XmlTxt.SetAttribute("campo_tipo_cod", Campo_Tipo_Cod)
            XmlTxt.SetAttribute("campo_tipo_des", Campo_Tipo_Des)
            XmlTxt.SetAttribute("campo_denominazione", Nome_Campo)
            XmlTxt.SetAttribute("codice_gruppo_vegetale_gias", Gru_Cod)
            XmlTxt.SetAttribute("codice_specie_gias", Veg_Cod)
            XmlTxt.SetAttribute("validita_inizio", Validita_Inizio)
            XmlTxt.SetAttribute("validita_fine", Validita_Fine)
            XmlTxt.SetAttribute("cam_chiave_cliente", Cam_Chiave_Cliente)
        End If

        Return XmlTxt

    End Function
    '########################################################################################
    Public Function Xml_Campo_Particella(ByRef XmlDoc As System.Xml.XmlDocument,
                                     ByVal Tipo_Operazione As String,
                                     ByVal Istat_Comune As String,
                                     ByVal Istat_Provincia As String,
                                     ByVal Sezione As String,
                                     ByVal Foglio As String,
                                     ByVal Numero As String,
                                     ByVal Subalterno As String,
                                     ByVal Sup As String) As System.Xml.XmlElement


        Dim XmlTxt As System.Xml.XmlElement

        XmlTxt = XmlDoc.CreateElement("Campo_Particella")

        XmlTxt.SetAttribute("tipo_operazione", Tipo_Operazione)
        XmlTxt.SetAttribute("istat_comune", Istat_Comune)
        XmlTxt.SetAttribute("istat_provincia", Istat_Provincia)
        XmlTxt.SetAttribute("sezione", Sezione)
        XmlTxt.SetAttribute("foglio", Foglio)
        XmlTxt.SetAttribute("numero", Numero)
        XmlTxt.SetAttribute("subalterno", Subalterno)
        XmlTxt.SetAttribute("superficie", Sup)

        Return XmlTxt

    End Function

    '########################################################################################
    Public Function Xml_Appezzamento(ByRef XmlDoc As System.Xml.XmlDocument,
                                ByVal Tipo_Operazione As String,
                                ByVal Codice As String,
                                ByVal Nome_Appezzamento As String,
                                ByVal Numero_Appezzamento_Bio As String,
                                ByVal Sup_Appezzamento As String,
                                ByVal Altitudine As String,
                                ByVal Titolo_Possesso As String,
                                ByVal Metodo_Produzione_Cod As String,
                                ByVal Metodo_Produzione_Des As String,
                                ByVal Contratto_Cod As String,
                                ByVal Validita_Inizio As String,
                                ByVal Validita_Fine As String,
                                ByVal App_Chiave_Cliente As String,
                                ByVal Riferimento_Alfanumerico_Appezzamento As String) As System.Xml.XmlElement


        Dim XmlTxt As System.Xml.XmlElement

        XmlTxt = XmlDoc.CreateElement("Appezzamento")

        XmlTxt.SetAttribute("tipo_operazione", Tipo_Operazione)
        XmlTxt.SetAttribute("codice_appezzamento", Codice)
        XmlTxt.SetAttribute("appezzamento_denominazione", Nome_Appezzamento)
        XmlTxt.SetAttribute("numero_appezzamento_bio", Numero_Appezzamento_Bio)
        XmlTxt.SetAttribute("sup_appezzamento", Sup_Appezzamento)
        XmlTxt.SetAttribute("altitudine", Altitudine)
        XmlTxt.SetAttribute("titolo_possesso", Titolo_Possesso)
        XmlTxt.SetAttribute("metodo_produzione_cod", Metodo_Produzione_Cod)
        XmlTxt.SetAttribute("metodo_produzione_des", Metodo_Produzione_Des)
        XmlTxt.SetAttribute("codice_contratto", Contratto_Cod)
        XmlTxt.SetAttribute("validita_inizio", Validita_Inizio)
        XmlTxt.SetAttribute("validita_fine", Validita_Fine)
        XmlTxt.SetAttribute("app_chiave_cliente", App_Chiave_Cliente)
        XmlTxt.SetAttribute("rif_qdc_app", Riferimento_Alfanumerico_Appezzamento)

        Return XmlTxt

    End Function

    '########################################################################################
    Public Function Xml_Appezzamento_Particella(ByRef XmlDoc As System.Xml.XmlDocument,
                                            ByVal Tipo_Operazione As String,
                                            ByVal Istat_Comune As String,
                                            ByVal Istat_Provincia As String,
                                            ByVal Sezione As String,
                                            ByVal Foglio As String,
                                            ByVal Numero As String,
                                            ByVal Subalterno As String,
                                            ByVal Sup As String,
                                            ByVal Validita_Inizio As String,
                                            ByVal Validita_Fine As String) As System.Xml.XmlElement


        Dim XmlTxt As System.Xml.XmlElement

        XmlTxt = XmlDoc.CreateElement("Appezzamento_Particella")

        XmlTxt.SetAttribute("tipo_operazione", Tipo_Operazione)
        XmlTxt.SetAttribute("istat_comune", Istat_Comune)
        XmlTxt.SetAttribute("istat_provincia", Istat_Provincia)
        XmlTxt.SetAttribute("sezione", Sezione)
        XmlTxt.SetAttribute("foglio", Foglio)
        XmlTxt.SetAttribute("numero", Numero)
        XmlTxt.SetAttribute("subalterno", Subalterno)
        XmlTxt.SetAttribute("superficie", Sup)
        XmlTxt.SetAttribute("validita_inizio", Validita_Inizio)
        XmlTxt.SetAttribute("validita_fine", Validita_Fine)

        Return XmlTxt

    End Function


    '########################################################################################
    Public Function Xml_Impianto(ByRef XmlDoc As System.Xml.XmlDocument,
                                ByVal Tipo_Operazione As String,
                                ByVal Codice As String,
                                ByVal Cod_Specie_Gias As String,
                                ByVal Des_Specie_Gias As String,
                                ByVal Cod_Varieta_Gias As String,
                                ByVal Des_Varieta_Gias As String,
                                ByVal Cod_DestUso_Gias As String,
                                ByVal Des_DestUso_Gias As String,
                                ByVal Flag_TerrenoNudo As Integer,
                                ByVal Cod_TipologiaVarietale_Gias As String,
                                ByVal Des_TipologiaVarietale_Gias As String,
                                ByVal Cod_Finalita_Gias As String,
                                ByVal Des_Finalita_Gias As String,
                                ByVal Portinnesto_Cod As String,
                                ByVal Portinnesto_Des As String,
                                ByVal Imp_Irrigazione_Cod As String,
                                ByVal Imp_Irrigazione_Des As String,
                                ByVal Forma_Allevamento_Cod As String,
                                ByVal Forma_Allevamento_Des As String,
                                ByVal Copertura_Cod As String,
                                ByVal Copertura_Des As String,
                                ByVal Semina_Trapianto As String,
                                ByVal Tra_Fila As String,
                                ByVal Su_Fila As String,
                                ByVal Interbina As String,
                                ByVal Germinabilita As String,
                                ByVal Sup_Imp As String,
                                ByVal Cod_Dettaglio_Specie_Personalizzato As String,
                                ByVal Cod_Specie_Cliente As String,
                                ByVal Cod_Finalita_Cliente As String,
                                ByVal Cod_Varieta_Cliente As String,
                                ByVal Cod_Portinnesto_Cliente As String,
                                ByVal Cod_Forma_Allevamento_Cliente As String,
                                ByVal Cod_Copertura_Cliente As String,
                                ByVal Cod_Impianto_Irriguo_Cliente As String,
                                ByVal Cod_Impianto As String,
                                ByVal Scarto As String,
                                ByVal Validita_Inizio As String,
                                ByVal Validita_Fine As String,
                                ByVal Imp_Chiave_Cliente As String) As System.Xml.XmlElement


        Dim XmlTxt As System.Xml.XmlElement

        XmlTxt = XmlDoc.CreateElement("Impianto")

        XmlTxt.SetAttribute("tipo_operazione", Tipo_Operazione)
        XmlTxt.SetAttribute("codice_impianto", Codice)
        XmlTxt.SetAttribute("sup_imp", Sup_Imp)
        XmlTxt.SetAttribute("codice_specie_gias", Cod_Specie_Gias)
        XmlTxt.SetAttribute("descrizione_specie_gias", Des_Specie_Gias)
        XmlTxt.SetAttribute("codice_varieta_gias", Cod_Varieta_Gias)
        XmlTxt.SetAttribute("descrizione_varieta_gias", Des_Varieta_Gias)

        XmlTxt.SetAttribute("codice_destuso_gias", Cod_DestUso_Gias)
        XmlTxt.SetAttribute("descrizione_destuso_gias", Des_DestUso_Gias)
        If Flag_TerrenoNudo = 1 Then
            XmlTxt.SetAttribute("flag_terrenonudo", "SI")
        Else
            XmlTxt.SetAttribute("flag_terrenonudo", "NO")
        End If

        XmlTxt.SetAttribute("codice_tipologia_varieta_gias", Cod_TipologiaVarietale_Gias)
        XmlTxt.SetAttribute("desc_tipologia_varieta_gias", Des_TipologiaVarietale_Gias)
        XmlTxt.SetAttribute("codice_finalita_gias", Cod_Finalita_Gias)
        XmlTxt.SetAttribute("desc_finalita_gias", Des_Finalita_Gias)
        XmlTxt.SetAttribute("codice_portinnesto_gias", Portinnesto_Cod)
        XmlTxt.SetAttribute("desc_portinnesto_gias", Portinnesto_Des)
        XmlTxt.SetAttribute("codice_impianto_irrigazione_gias", Imp_Irrigazione_Cod)
        XmlTxt.SetAttribute("desc_impianto_irrigazione_gias", Imp_Irrigazione_Des)
        XmlTxt.SetAttribute("codice_forma_allevamento_gias", Forma_Allevamento_Cod)
        XmlTxt.SetAttribute("desc_forma_allevamento_gias", Forma_Allevamento_Des)
        XmlTxt.SetAttribute("codice_copertura_gias", Copertura_Cod)
        XmlTxt.SetAttribute("desc_copertura_gias", Copertura_Des)
        XmlTxt.SetAttribute("semina_trapianto", Semina_Trapianto)
        XmlTxt.SetAttribute("distanza_tra_fila", Tra_Fila)
        XmlTxt.SetAttribute("distanza_su_fila", Su_Fila)
        XmlTxt.SetAttribute("interbina", Interbina)
        XmlTxt.SetAttribute("germinabilita", Germinabilita)

        XmlTxt.SetAttribute("codice_dettaglio_specie_personalizzato", Cod_Dettaglio_Specie_Personalizzato)
        XmlTxt.SetAttribute("codice_specie_cliente", Cod_Specie_Cliente)
        XmlTxt.SetAttribute("codice_finalita_cliente", Cod_Finalita_Cliente)
        XmlTxt.SetAttribute("codice_varieta_cliente", Cod_Varieta_Cliente)
        XmlTxt.SetAttribute("cod_portinnesto_cliente", Cod_Portinnesto_Cliente)
        XmlTxt.SetAttribute("cod_forma_allevamento_cliente", Cod_Forma_Allevamento_Cliente)
        XmlTxt.SetAttribute("cod_copertura_cliente", Cod_Copertura_Cliente)
        XmlTxt.SetAttribute("cod_impianto_irriguo_cliente", Cod_Impianto_Irriguo_Cliente)
        XmlTxt.SetAttribute("cod_impianto", Cod_Impianto)

        XmlTxt.SetAttribute("scarto", Scarto)

        XmlTxt.SetAttribute("validita_inizio", Validita_Inizio)
        XmlTxt.SetAttribute("validita_fine", Validita_Fine)
        XmlTxt.SetAttribute("imp_chiave_cliente", Imp_Chiave_Cliente)

        Return XmlTxt


    End Function

    '#################################################################################################
    Public Function Xml_ProgettoImpianto(ByRef XmlDoc As System.Xml.XmlDocument,
                                         ByVal Tipo_Operazione As String,
                                         ByVal Codice As String,
                                         ByVal Progetto_Nome As String,
                                         ByVal Progetto_Des As String,
                                         ByVal Resa_Prevista As String,
                                         ByVal Piante_Ha As String,
                                         ByVal org_referente_cod As String,
                                         ByVal org_referente_des As String,
                                         ByVal capitolato_privato_cod As String,
                                         ByVal capitolato_privato_des As String,
                                         ByVal codice_regolamento As String,
                                         ByVal desc_regolamento As String,
                                         ByVal codice_disciplinare As String,
                                         ByVal piano_semina As String,
                                         ByVal stato_impianto As String,
                                         ByVal Validita_Inizio As String,
                                         ByVal Validita_Fine As String,
                                         ByVal data_modifica As String,
                                         ByVal username_modifica As String,
                                         ByVal utente_modifica As String,
                                         ByVal Specie_Esercizio_Codice_ABOCA As String,
                                         ByVal Specie_Esercizio_Descr_ABOCA As String,
                                         ByVal Data_Chiusura_Esercizio As String,
                                         ByVal NumOPCollegamento As String) As System.Xml.XmlElement

        Dim XmlTxt As System.Xml.XmlElement

        XmlTxt = XmlDoc.CreateElement("Esercizio")

        XmlTxt.SetAttribute("tipo_operazione", Tipo_Operazione)
        XmlTxt.SetAttribute("codice_progetto", Codice)
        XmlTxt.SetAttribute("lotto", Progetto_Nome)
        XmlTxt.SetAttribute("descrizione_progetto", Progetto_Des)
        XmlTxt.SetAttribute("resa_prevista", Resa_Prevista)
        XmlTxt.SetAttribute("piante_per_ha", Piante_Ha)
        XmlTxt.SetAttribute("org_referente_cod", org_referente_cod)
        XmlTxt.SetAttribute("org_referente_des", org_referente_des)
        XmlTxt.SetAttribute("capitolato_privato_cod", capitolato_privato_cod)
        XmlTxt.SetAttribute("capitolato_privato_des", capitolato_privato_des)
        XmlTxt.SetAttribute("regolamento_cod", codice_regolamento)
        XmlTxt.SetAttribute("regolamento_des", desc_regolamento)
        XmlTxt.SetAttribute("codice_disciplinare", codice_disciplinare)
        XmlTxt.SetAttribute("piano_semina", piano_semina)
        XmlTxt.SetAttribute("stato_impianto", stato_impianto)
        XmlTxt.SetAttribute("validita_inizio", Validita_Inizio)
        XmlTxt.SetAttribute("validita_fine", Validita_Fine)
        XmlTxt.SetAttribute("data_modifica", data_modifica)
        XmlTxt.SetAttribute("username_modifica", username_modifica)
        XmlTxt.SetAttribute("utente_modifica", utente_modifica)
        XmlTxt.SetAttribute("Specie_Esercizio_Codice_ABOCA".ToLower, Specie_Esercizio_Codice_ABOCA)
        XmlTxt.SetAttribute("Specie_Esercizio_Descr_ABOCA".ToLower, Specie_Esercizio_Descr_ABOCA)
        XmlTxt.SetAttribute("Data_Chiusura_Esercizio".ToLower, Data_Chiusura_Esercizio)
        XmlTxt.SetAttribute("num_op_collegamento".ToLower, NumOPCollegamento)

        Return XmlTxt

    End Function



    '########################################################################################
    'XML PUBBLICO DEL PROGETTO
    Public Function XML_Pbl_Progetto(ByVal Tipo_Operazione As enum_TipoOperazioneDB,
                                        ByVal Codice_Progetto As Integer,
                                        ByVal Descrizione_Progetto As String,
                                        ByVal Codice_Regolamento As Integer,
                                        ByVal Codice_Disciplinare As Integer,
                                        ByVal Stato_Impianto As String,
                                        ByVal Piante_Per_HA As Decimal,
                                        ByVal Resa_Prevista As Decimal,
                                        ByVal Data_Semina_Prevista As String,
                                        ByVal Data_Fioritura_Prevista As String,
                                        ByVal Data_Raccolta_Prevista As String,
                                        ByVal Org_Referente As String,
                                        ByVal Capitolato_Privato As String,
                                        ByVal Validita_Inizio As String,
                                        ByVal Validita_Fine As String,
                                        Optional ByRef XmlDoc As XmlDocument = Nothing,
                                        Optional ByVal Disciplinare_PubblicoPrivato As Integer = 0,
                                        Optional ByVal Regolamento_concimazioni_cod As Integer = 0,
                                         Optional ByVal Magazzino_Conferimento As String = "",
                                        Optional ByVal Limite_N As Decimal = -99999999,
                                        Optional ByVal Limite_P As Decimal = -99999999,
                                        Optional ByVal Limite_K As Decimal = -99999999,
                                        Optional ByVal Limite_Mg As Decimal = -99999999,
                                            Optional ByVal Nome_Progetto As String = ""
                                        ) As XmlElement

        Dim XmlEl As XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        XmlEl = XmlDoc.CreateElement("Progetto")

        XmlEl.SetAttribute("tipo_operazione", CStr(Tipo_Operazione))
        XmlEl.SetAttribute("codice_progetto", CStr(Codice_Progetto))
        XmlEl.SetAttribute("descrizione_progetto", Descrizione_Progetto)
        XmlEl.SetAttribute("nome_progetto", Nome_Progetto)

        XmlEl.SetAttribute("Codice_Regolamento".ToLower, CStr(Codice_Regolamento))
        XmlEl.SetAttribute("Codice_Disciplinare".ToLower, CStr(Codice_Disciplinare))
        XmlEl.SetAttribute("Disciplinare_PubblicoPrivato".ToLower, CStr(Disciplinare_PubblicoPrivato))
        XmlEl.SetAttribute("Regolamento_concimazioni_cod".ToLower, CStr(Regolamento_concimazioni_cod))
        XmlEl.SetAttribute("Stato_Impianto".ToLower, CStr(Stato_Impianto))

        XmlEl.SetAttribute("Piante_Per_HA".ToLower, CStr(Piante_Per_HA))
        XmlEl.SetAttribute("Resa_Prevista".ToLower, CStr(Resa_Prevista))
        XmlEl.SetAttribute("Data_Semina_Prevista".ToLower, CStr(Data_Semina_Prevista))
        XmlEl.SetAttribute("data_fioritura_prevista".ToLower, CStr(Data_Fioritura_Prevista))
        XmlEl.SetAttribute("Data_Raccolta_Prevista".ToLower, CStr(Data_Raccolta_Prevista))

        XmlEl.SetAttribute("Org_Referente".ToLower, CStr(Org_Referente))
        XmlEl.SetAttribute("Capitolato_Privato".ToLower, CStr(Capitolato_Privato))
        If Magazzino_Conferimento <> "" Then
            XmlEl.SetAttribute("Magazzino_Conferimento".ToLower, CStr(Magazzino_Conferimento))
        End If
        If Limite_N <> -99999999 Then
            XmlEl.SetAttribute("Limite_N".ToLower, CStr(Limite_N))
        End If
        If Limite_P <> -99999999 Then
            XmlEl.SetAttribute("Limite_P".ToLower, CStr(Limite_P))
        End If
        If Limite_K <> -99999999 Then
            XmlEl.SetAttribute("Limite_K".ToLower, CStr(Limite_K))
        End If
        If Limite_Mg <> -99999999 Then
            XmlEl.SetAttribute("Limite_Mg".ToLower, CStr(Limite_Mg))
        End If

        XmlEl.SetAttribute("validita_inizio", Validita_Inizio)
        XmlEl.SetAttribute("validita_fine", Validita_Fine)

        Return XmlEl

    End Function


    '########################################################################################
    'a differenza dell'altra ha solo i dati basici
    Public Function XML_Pbl_Progetto_Basico(ByVal Tipo_Operazione As enum_TipoOperazioneDB, _
                                        ByVal Codice_Progetto As Integer, _
                                        ByVal Descrizione_Progetto As String, _
                                        ByVal Validita_Inizio As String, _
                                        ByVal Validita_Fine As String, _
                                        Optional ByRef XmlDoc As XmlDocument = Nothing _
                                        ) As XmlElement

        Dim XmlEl As XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        XmlEl = XmlDoc.CreateElement("Progetto")

        XmlEl.SetAttribute("tipo_operazione", CStr(Tipo_Operazione))
        XmlEl.SetAttribute("codice_progetto", CStr(Codice_Progetto))
        XmlEl.SetAttribute("descrizione_progetto", Descrizione_Progetto)
        XmlEl.SetAttribute("validita_inizio", Validita_Inizio)
        XmlEl.SetAttribute("validita_fine", Validita_Fine)

        Return XmlEl

    End Function


End Class
