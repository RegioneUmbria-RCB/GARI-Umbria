Imports System.Xml
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi


'//////////////////////////////////////////////////////////////////////////////////////////////
'//////////////////////////////////////////////////////////////////////////////////////////////
'//////////////////////////////////////////////////////////////////////////////////////////////
'//////////////////////////////////////////////////////////////////////////////////////////////
'//////////////////////////////////////////////////////////////////////////////////////////////
'           USARE I FILE XML_Privato e XML_Pubblico
'//////////////////////////////////////////////////////////////////////////////////////////////
'//////////////////////////////////////////////////////////////////////////////////////////////
'//////////////////////////////////////////////////////////////////////////////////////////////
'//////////////////////////////////////////////////////////////////////////////////////////////
'//////////////////////////////////////////////////////////////////////////////////////////////
'//////////////////////////////////////////////////////////////////////////////////////////////


Public Class AnagrafeXML

    '################################################################################
    <Obsolete("ELIMINARE: Usare invece AgronicaCoreDataProvider.Conversioni.EttariAreCentiare_from_Ettari()", False)>
    Public Sub EttariAreCentiare_from_Ettari(ByVal EttariAreCentiare As Decimal,
                                             ByRef Ettari As Decimal,
                                             ByRef Are As Decimal,
                                             ByRef Centiare As Decimal)

        Dim ettariAreCentiareLocal As Decimal

        Dim supAreCentiare As Decimal
        Dim supCentiare As Decimal

        If EttariAreCentiare < 0 Then
            ettariAreCentiareLocal = (-1) * EttariAreCentiare
        Else
            ettariAreCentiareLocal = EttariAreCentiare
        End If

        Ettari = Int(ettariAreCentiareLocal)
        supAreCentiare = ettariAreCentiareLocal - Ettari + 0.00001
        Are = Int(supAreCentiare * 100)
        supCentiare = supAreCentiare * 100 - Are
        Centiare = Int(supCentiare * 100)

        If EttariAreCentiare < 0 Then
            Ettari = (-1) * Ettari
        End If

    End Sub

    '################################################################################
    <Obsolete("ELIMINARE: Usare invece AgronicaCoreDataProvider.Conversioni.Ettari_from_EttariAreCentiare() (lì c'è CInt di ettari)", False)>
    Public Function Ettari_from_EttariAreCentiare(ByVal Ettari As Decimal,
                                                  ByVal Are As Decimal,
                                                  ByVal Centiare As Decimal
                                                  ) As Decimal

        Dim supHa As Decimal

        'Calcolo
        supHa = Ettari + (Are / 100) + (Centiare / 10000)

        'Restituisco il risultato
        Return supHa

    End Function



    '##########################################################################################
    Public Function XML_VariabiliSessione(ByVal Progressivo_Gias As String,
                                          ByVal Cn_Server As String,
                                          ByVal Cn_Utenti As String,
                                          ByVal Utente_Usr As String,
                                          ByVal Utente_Pwd As String,
                                          ByVal SuperUser_Usr As String,
                                          ByVal SuperUser_Pwd As String,
                                          ByVal SuperUser_Piva As String
                                          ) As String

        Dim xmlDoc As New XmlDocument
        Dim xmlTxt As XmlElement

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        xmlTxt = xmlDoc.CreateElement("VariabiliSessione")

        'Imposto gli attributi
        xmlTxt.SetAttribute(LCase("Progressivo_Gias"), CStr(Progressivo_Gias))
        xmlTxt.SetAttribute(LCase("Cn_Server"), CStr(Cn_Server))
        xmlTxt.SetAttribute(LCase("Cn_Utenti"), CStr(Cn_Utenti))
        xmlTxt.SetAttribute(LCase("Utente_Usr"), CStr(Utente_Usr))
        xmlTxt.SetAttribute(LCase("Utente_Pwd"), CStr(Utente_Pwd))
        xmlTxt.SetAttribute(LCase("SuperUser_Usr"), CStr(SuperUser_Usr))
        xmlTxt.SetAttribute(LCase("SuperUser_Pwd"), CStr(SuperUser_Pwd))
        xmlTxt.SetAttribute(LCase("SuperUser_Piva"), CStr(SuperUser_Piva))

        'Imposto XmlTxt come figlio del documento principale
        xmlDoc.AppendChild(xmlTxt)

        'Restituisco in uscita la stringa creata
        Return xmlDoc.InnerXml

    End Function


    '########################################################################################
    Public Function Xml_Pubblico_Utente(Optional ByRef XmlDoc As XmlDocument = Nothing,
                                        Optional ByVal strUsername As String = "#",
                                        Optional ByVal strPassword As String = "#",
                                        Optional ByVal strCodice As String = "#"
                                        ) As XmlElement

        Dim xmlTxt As XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        xmlTxt = XmlDoc.CreateElement("Utente")

        xmlTxt.SetAttribute("username", strUsername)
        xmlTxt.SetAttribute("password", strPassword)
        xmlTxt.SetAttribute("codice", strCodice)

        'Restituisco in uscita 
        Return xmlTxt


    End Function

    '##########################################################################################
    Public Function Xml_Pubblico_Impresa(ByVal Tipo_Operazione As String,
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
                                         ByVal lr_cognome As String, ByVal lr_nome As String,
                                         ByVal lr_codice_fiscale As String, ByVal lr_sesso As String,
                                         ByVal lr_validita_inizio As String, ByVal lr_validita_fine As String,
                                         ByVal lr_indirizzo As String, ByVal lr_frazione As String,
                                         ByVal lr_cap As String, ByVal lr_comune As String,
                                         ByVal lr_provincia As String, ByVal lr_stato As String,
                                         ByVal lr_note_indirizzo As String, ByVal lr_codice_istat_comune As String,
                                         ByVal lr_codice_istat_provincia As String, ByVal lr_nascita_data As String,
                                         ByVal lr_nascita_comune As String, ByVal lr_nascita_provincia As String,
                                         ByVal lr_nascita_istat_comune As String,
                                         ByVal lr_nascita_istat_provincia As String,
                                         ByVal lr_documenti As String, ByVal lr_rubrica_1 As String,
                                         ByVal lr_rubrica_2 As String, ByVal lr_rubrica_3 As String,
                                         ByVal lr_rubrica_4 As String, ByVal lr_rubrica_5 As String,
                                         ByVal cf_tecnico_referente As String,
                                         ByVal codice_cliente As String,
                                         ByVal codice_fornitore As String,
                                         ByVal codice_fornitore_2 As String,
                                         ByVal codice_fornitore_3 As String,
                                         ByVal codice_ausl As String,
                                         ByVal i_Chiave_Cliente As String,
                                         Optional ByRef XmlDoc As XmlDocument = Nothing,
                                         Optional ByVal codice_libro_soci As String = "#",
                                         Optional ByVal data_iscrizione_libro_soci As String = "#",
                                         Optional ByVal CodiceConferente As String = "#",
                                         Optional ByVal Codice_Zona As String = "#",
                                         Optional ByVal Codice_Pat As String = "#",
                                         Optional ByVal Codice_GlobalGap As String = "#",
                                         Optional ByVal Codice_GrowerNumber As String = "#",
                                         Optional ByVal Codice_Contratto As String = "#",
                                         Optional ByVal Codice_Ufficio_REA As String = "#",
                                         Optional ByVal Codice_Numero_REA As String = "#"
                                         ) As XmlElement

        Dim xmlTxt As XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        xmlTxt = XmlDoc.CreateElement("Impresa")

        xmlTxt.SetAttribute("tipo_operazione", Tipo_Operazione)
        xmlTxt.SetAttribute("partita_iva", Piva)
        xmlTxt.SetAttribute("ragione_sociale", Rag_Soc)
        xmlTxt.SetAttribute("codice_cuaa", Cuaa)
        xmlTxt.SetAttribute("codice_fiscale", Codice_Fiscale)
        xmlTxt.SetAttribute("codice_socio", Cod_Socio)
        xmlTxt.SetAttribute("sup_totale", Sup_Totale)
        xmlTxt.SetAttribute("partita_iva_padre", Piva_Padre)
        xmlTxt.SetAttribute("tipo_impresa_gerarchia", Tipo_Gerarchia)
        xmlTxt.SetAttribute("titolo_possesso", Titolo_Possesso)
        xmlTxt.SetAttribute("validita_inizio", Validita_Inizio)
        xmlTxt.SetAttribute("validita_fine", Validita_Fine)

        xmlTxt.SetAttribute("i_indirizzo", i_indirizzo)
        xmlTxt.SetAttribute("i_frazione", i_frazione)
        xmlTxt.SetAttribute("i_cap", i_cap)
        xmlTxt.SetAttribute("i_comune", i_comune)
        xmlTxt.SetAttribute("i_provincia", i_provincia)
        xmlTxt.SetAttribute("i_stato", i_stato)
        xmlTxt.SetAttribute("i_note_indirizzo", i_note_indirizzo)
        xmlTxt.SetAttribute("i_codice_istat_comune", i_codice_istat_comune)
        xmlTxt.SetAttribute("i_codice_istat_provincia", i_codice_istat_provincia)

        xmlTxt.SetAttribute("legale_rappresentante", legale_rappresentante)
        xmlTxt.SetAttribute("lr_cognome", lr_cognome)
        xmlTxt.SetAttribute("lr_nome", lr_nome)
        xmlTxt.SetAttribute("lr_codice_fiscale", lr_codice_fiscale)
        xmlTxt.SetAttribute("lr_sesso", lr_sesso)
        xmlTxt.SetAttribute("lr_validita_inizio", lr_validita_inizio)
        xmlTxt.SetAttribute("lr_validita_fine", lr_validita_fine)
        xmlTxt.SetAttribute("lr_indirizzo", lr_indirizzo)
        xmlTxt.SetAttribute("lr_frazione", lr_frazione)
        xmlTxt.SetAttribute("lr_cap", lr_cap)
        xmlTxt.SetAttribute("lr_comune", lr_comune)
        xmlTxt.SetAttribute("lr_provincia", lr_provincia)
        xmlTxt.SetAttribute("lr_stato", lr_stato)
        xmlTxt.SetAttribute("lr_note_indirizzo", lr_note_indirizzo)
        xmlTxt.SetAttribute("lr_codice_istat_comune", lr_codice_istat_comune)
        xmlTxt.SetAttribute("lr_codice_istat_provincia", lr_codice_istat_provincia)
        xmlTxt.SetAttribute("lr_nascita_data", lr_nascita_data)
        xmlTxt.SetAttribute("lr_nascita_comune", lr_nascita_comune)
        xmlTxt.SetAttribute("lr_nascita_provincia", lr_nascita_provincia)
        xmlTxt.SetAttribute("lr_nascita_istat_comune", lr_nascita_istat_comune)
        xmlTxt.SetAttribute("lr_nascita_istat_provincia", lr_nascita_istat_provincia)

        xmlTxt.SetAttribute("lr_documenti", lr_documenti)
        xmlTxt.SetAttribute("lr_rubrica_1", lr_rubrica_1)
        xmlTxt.SetAttribute("lr_rubrica_2", lr_rubrica_2)
        xmlTxt.SetAttribute("lr_rubrica_3", lr_rubrica_3)
        xmlTxt.SetAttribute("lr_rubrica_4", lr_rubrica_4)
        xmlTxt.SetAttribute("lr_rubrica_5", lr_rubrica_5)

        xmlTxt.SetAttribute("cf_tecnico_referente", cf_tecnico_referente)

        xmlTxt.SetAttribute("codice_cliente", codice_cliente)
        xmlTxt.SetAttribute("codice_fornitore", codice_fornitore)
        xmlTxt.SetAttribute("codice_fornitore_2", codice_fornitore_2)
        xmlTxt.SetAttribute("codice_fornitore_3", codice_fornitore_3)
        xmlTxt.SetAttribute("codice_ausl", codice_ausl)
        xmlTxt.SetAttribute("codice_libro_soci", codice_libro_soci)
        xmlTxt.SetAttribute("data_iscrizione_libro_soci", data_iscrizione_libro_soci)

        xmlTxt.SetAttribute("i_chiave_cliente", i_Chiave_Cliente)

        xmlTxt.SetAttribute("CodiceConferente", CodiceConferente)
        xmlTxt.SetAttribute("Codice_Zona", Codice_Zona)
        xmlTxt.SetAttribute("codice_pat", Codice_Pat)
        xmlTxt.SetAttribute(("Codice_GlobalGap").ToLower, Codice_GlobalGap)
        xmlTxt.SetAttribute(("Codice_GrowerNumber").ToLower, Codice_GrowerNumber)
        xmlTxt.SetAttribute("Codice_Contratto".ToLower, Codice_Contratto)       'Lavez - 15/4/2021 - aggiunto codice contratto

        xmlTxt.SetAttribute("Codice_Ufficio_REA".ToLower, Codice_Ufficio_REA)
        xmlTxt.SetAttribute("Codice_Numero_REA".ToLower, Codice_Numero_REA)

        'Restituisco in uscita 
        Return xmlTxt

    End Function

    '##########################################################################################
    Public Function Xml_Pubblico_Fascicolo(ByVal Nome_File As String,
                                           ByVal Numero As String,
                                           ByVal Data_Validazione As String,
                                           ByVal Codice_Detentore As String,
                                           ByVal Validita_Inizio_Mandato As String,
                                           ByVal Validita_Fine_Mandato As String,
                                           ByVal Validita_Inizio As String,
                                           ByVal Validita_Fine As String,
                                           Optional ByRef XmlDoc As XmlDocument = Nothing,
                                           Optional ByRef AllegatiDocumentiXML As String = ""
                                           ) As XmlElement

        Dim xmlTxt As XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        xmlTxt = XmlDoc.CreateElement("Fascicolo")

        xmlTxt.SetAttribute("nome_file", Nome_File)
        xmlTxt.SetAttribute("numero", Numero)
        xmlTxt.SetAttribute("data_validazione", Data_Validazione)
        xmlTxt.SetAttribute("codice_detentore", Codice_Detentore)
        xmlTxt.SetAttribute("validita_inizio_mandato", Validita_Inizio_Mandato)
        xmlTxt.SetAttribute("validita_fine_mandato", Validita_Fine_Mandato)
        xmlTxt.SetAttribute("validita_inizio", Validita_Inizio)
        xmlTxt.SetAttribute("validita_fine", Validita_Fine)
        xmlTxt.SetAttribute("allegatiDocumentiXML", AllegatiDocumentiXML)

        'Restituisco in uscita 
        Return xmlTxt

    End Function

    '##########################################################################################
    Public Function Xml_Pubblico_CentroAziendale(ByVal Tipo_Operazione As String,
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
                                                 ByVal c_provincia As String,
                                                 ByVal c_stato As String,
                                                 ByVal c_note_indirizzo As String,
                                                 ByVal c_codice_istat_comune As String,
                                                 ByVal c_codice_istat_provincia As String,
                                                 ByVal c_rubrica_1 As String, ByVal c_rubrica_2 As String,
                                                 ByVal c_rubrica_3 As String, ByVal c_rubrica_4 As String,
                                                 ByVal c_rubrica_5 As String, ByVal c_rubrica_6 As String,
                                                 ByVal c_rubrica_7 As String, ByVal c_rubrica_8 As String,
                                                 ByVal c_rubrica_9 As String,
                                                 ByVal c_Chiave_Cliente As String,
                                                 Optional ByRef XmlDoc As XmlDocument = Nothing,
                                                 Optional ByVal X As Decimal = 0,
                                                 Optional ByVal Y As Decimal = 0,
                                                 Optional ByVal Longitudine As Decimal = 0,
                                                 Optional ByVal Latitudine As Decimal = 0,
                                                 Optional ByVal Codice_Centro As String = "",
                                                 Optional ByVal Id_UTE As String = ""
                                                 ) As XmlElement

        Dim xmlTxt As XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        xmlTxt = XmlDoc.CreateElement("CentroAziendale")

        xmlTxt.SetAttribute("tipo_operazione", Tipo_Operazione)
        xmlTxt.SetAttribute("codice_centro", Codice)
        xmlTxt.SetAttribute("nome_centro", Nome_Centro)
        xmlTxt.SetAttribute("titolo_possesso", Titolo_Possesso)
        xmlTxt.SetAttribute("sup_bosco", Sup_Bosco)
        xmlTxt.SetAttribute("sup_prati", Sup_Prati)
        xmlTxt.SetAttribute("codice_operatore", Cod_Operatore)
        xmlTxt.SetAttribute("tipo_attivita", Tipo_Attivita)
        xmlTxt.SetAttribute("validita_inizio", Validita_Inizio)
        xmlTxt.SetAttribute("validita_fine", Validita_Fine)

        xmlTxt.SetAttribute("x", X)
        xmlTxt.SetAttribute("y", Y)
        xmlTxt.SetAttribute("Long".ToLower, Longitudine)
        xmlTxt.SetAttribute("Lat".ToLower, Latitudine)
        xmlTxt.SetAttribute("a_codice_Centro".ToLower, Codice_Centro)

        xmlTxt.SetAttribute("Id_UTE".ToLower, Id_UTE)

        xmlTxt.SetAttribute("c_indirizzo", c_indirizzo)
        xmlTxt.SetAttribute("c_frazione", c_frazione)
        xmlTxt.SetAttribute("c_cap", c_cap)
        xmlTxt.SetAttribute("c_comune", c_comune)
        xmlTxt.SetAttribute("c_provincia", c_provincia)
        xmlTxt.SetAttribute("c_stato", c_stato)
        xmlTxt.SetAttribute("c_note_indirizzo", c_note_indirizzo)
        xmlTxt.SetAttribute("c_codice_istat_comune", c_codice_istat_comune)
        xmlTxt.SetAttribute("c_codice_istat_provincia", c_codice_istat_provincia)

        xmlTxt.SetAttribute("c_rubrica_1", c_rubrica_1)
        xmlTxt.SetAttribute("c_rubrica_2", c_rubrica_2)
        xmlTxt.SetAttribute("c_rubrica_3", c_rubrica_3)
        xmlTxt.SetAttribute("c_rubrica_4", c_rubrica_4)
        xmlTxt.SetAttribute("c_rubrica_5", c_rubrica_5)
        xmlTxt.SetAttribute("c_rubrica_6", c_rubrica_6)
        xmlTxt.SetAttribute("c_rubrica_7", c_rubrica_7)
        xmlTxt.SetAttribute("c_rubrica_8", c_rubrica_8)
        xmlTxt.SetAttribute("c_rubrica_9", c_rubrica_9)

        xmlTxt.SetAttribute("c_chiave_cliente", c_Chiave_Cliente)

        'Restituisco in uscita 
        Return xmlTxt

    End Function

    '########################################################################################
    Public Function Xml_Pubblico_Fabbricato(ByVal Tipo_Operazione As String,
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
                                            ByVal f_indirizzo As String,
                                            ByVal f_frazione As String,
                                            ByVal f_cap As String,
                                            ByVal f_comune As String,
                                            ByVal f_provincia As String,
                                            ByVal f_stato As String,
                                            ByVal f_note_indirizzo As String,
                                            ByVal f_codice_istat_comune As String,
                                            ByVal f_codice_istat_provincia As String,
                                            ByVal f_Chiave_Cliente As String,
                                            Optional ByRef XmlDoc As XmlDocument = Nothing
                                            ) As XmlElement

        Dim xmlTxt As XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        'Creo il nodo 
        xmlTxt = XmlDoc.CreateElement("Fabbricato")

        xmlTxt.SetAttribute("tipo_operazione", Tipo_Operazione)
        xmlTxt.SetAttribute("codice_fabbricato", Codice)
        xmlTxt.SetAttribute("fabbricato_denominazione", Nome_Fabbricato)
        xmlTxt.SetAttribute("tipo_fabbricato_codice", Tipo_Fabbricato_Cod)
        xmlTxt.SetAttribute("volume_convenzionale", Vol_Convenzionale)
        xmlTxt.SetAttribute("volume_conversione", Vol_Conversione)
        xmlTxt.SetAttribute("volume_biologico", Vol_Biologico)
        xmlTxt.SetAttribute("titolo_possesso", Titolo_Possesso)
        xmlTxt.SetAttribute("codice_regolamento", Regolamento_Cod)
        xmlTxt.SetAttribute("codice_particella", Particella_Codice)
        xmlTxt.SetAttribute("validita_inizio", Validita_Inizio)
        xmlTxt.SetAttribute("validita_fine", Validita_Fine)

        xmlTxt.SetAttribute("f_indirizzo", f_indirizzo)
        xmlTxt.SetAttribute("f_frazione", f_frazione)
        xmlTxt.SetAttribute("f_cap", f_cap)
        xmlTxt.SetAttribute("f_comune", f_comune)
        xmlTxt.SetAttribute("f_provincia", f_provincia)
        xmlTxt.SetAttribute("f_stato", f_stato)
        xmlTxt.SetAttribute("f_note_indirizzo", f_note_indirizzo)
        xmlTxt.SetAttribute("f_codice_istat_comune", f_codice_istat_comune)
        xmlTxt.SetAttribute("f_codice_istat_provincia", f_codice_istat_provincia)

        xmlTxt.SetAttribute("f_chiave_cliente", f_Chiave_Cliente)

        'Restituisco in uscita 
        Return xmlTxt

    End Function

    '########################################################################################
    Public Function Xml_Pubblico_Stalla(ByVal Tipo_Operazione As String,
                                        ByVal Data_Costruzione As String,
                                        ByVal Data_Chiusura As String,
                                        ByVal Codice_Genere_Animale_Gias As String,
                                        ByVal Codice_Specie_Animale_Gias As String,
                                        ByVal Codice_Indirizzo_Produttivo_Animale_Gias As String,
                                        ByVal Codice_Animale_Cliente As String,
                                        ByVal Latitudine As String,
                                        ByVal Longitudine As String,
                                        Optional ByRef XmlDoc As XmlDocument = Nothing
                                        ) As XmlElement

        Dim xmlTxt As XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        'Creo il nodo 
        xmlTxt = XmlDoc.CreateElement("Stalla")

        xmlTxt.SetAttribute("tipo_operazione", Tipo_Operazione)
        xmlTxt.SetAttribute("data_costruzione", Data_Costruzione)
        xmlTxt.SetAttribute("data_chiusura", Data_Chiusura)
        xmlTxt.SetAttribute("codice_genere_animale_gias", Codice_Genere_Animale_Gias)
        xmlTxt.SetAttribute("codice_specie_animale_gias", Codice_Specie_Animale_Gias)
        xmlTxt.SetAttribute("codice_indirizzo_produttivo_animale_gias", Codice_Indirizzo_Produttivo_Animale_Gias)
        xmlTxt.SetAttribute("codice_animale_cliente", Codice_Animale_Cliente)
        xmlTxt.SetAttribute("latitudine", Latitudine)
        xmlTxt.SetAttribute("longitudine", Longitudine)

        'Restituisco in uscita 
        Return xmlTxt

    End Function

    '########################################################################################
    Public Function Xml_Pubblico_CapoAnimale(ByVal Tipo_Operazione As String,
                                             ByVal Codice_Capo As String,
                                             ByVal Codice_Indirizzo_Produttivo_Animale_Gias As String,
                                             ByVal Codice_Categoria_Animale_Gias As String,
                                             ByVal Codice_Razza_Animale_Gias As String,
                                             ByVal Numero As String,
                                             ByVal Matricola As String,
                                             ByVal Nome As String,
                                             ByVal Collare As String,
                                             ByVal Data_Nascita As String,
                                             ByVal Sesso As String,
                                             ByVal Matricola_Madre As String,
                                             ByVal Matricola_Padre As String,
                                             ByVal Peso As String,
                                             ByVal Metodo_Produzione As String,
                                             Optional ByRef XmlDoc As XmlDocument = Nothing
                                             ) As XmlElement

        Dim xmlTxt As XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        'Creo il nodo 
        xmlTxt = XmlDoc.CreateElement("Capo_Animale")

        xmlTxt.SetAttribute("tipo_operazione", Tipo_Operazione)
        xmlTxt.SetAttribute("codice_capo", Codice_Capo)
        xmlTxt.SetAttribute("codice_indirizzo_produttivo_animale_gias", Codice_Indirizzo_Produttivo_Animale_Gias)
        xmlTxt.SetAttribute("codice_categoria_animale_gias", Codice_Categoria_Animale_Gias)
        xmlTxt.SetAttribute("codice_razza_animale_gias", Codice_Razza_Animale_Gias)
        xmlTxt.SetAttribute("numero", Numero)
        xmlTxt.SetAttribute("matricola", Matricola)
        xmlTxt.SetAttribute("nome", Nome)
        xmlTxt.SetAttribute("collare", Collare)
        xmlTxt.SetAttribute("data_nascita", Data_Nascita)
        xmlTxt.SetAttribute("sesso", Sesso)
        xmlTxt.SetAttribute("matricola_madre", Matricola_Madre)
        xmlTxt.SetAttribute("matricola_padre", Matricola_Padre)
        xmlTxt.SetAttribute("peso", Peso)
        xmlTxt.SetAttribute("metodo_produzione", Metodo_Produzione)

        'Restituisco in uscita 
        Return xmlTxt

    End Function

    '########################################################################################
    Public Function Xml_Pubblico_Particella(ByVal Tipo_Operazione As String,
                                            ByVal Codice As String,
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
                                            ByVal Qualita_Catasto_Cod As String,
                                            ByVal Classe As String,
                                            ByVal Reddito_Dominicale As String,
                                            ByVal Reddito_Agrario As String,
                                            ByVal Sup_Condotta As String,
                                            ByVal Validita_Inizio As String,
                                            ByVal Validita_Fine As String,
                                            Optional ByRef XmlDoc As XmlDocument = Nothing,
                                            Optional ByVal wkt As String = "",
                                            Optional ByVal wkt_georiferimento_Cod As String = ""
                                            ) As XmlElement

        Dim xmlTxt As XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        'Creo il nodo 
        xmlTxt = XmlDoc.CreateElement("Particella")

        xmlTxt.SetAttribute("tipo_operazione", Tipo_Operazione)
        xmlTxt.SetAttribute("codice_particella", Codice)
        xmlTxt.SetAttribute("p_codice_istat_comune", p_codice_istat_comune)
        xmlTxt.SetAttribute("p_codice_istat_provincia", p_codice_istat_provincia)
        xmlTxt.SetAttribute("sezione", Sezione)
        xmlTxt.SetAttribute("foglio", Foglio)
        xmlTxt.SetAttribute("numero", Numero)
        xmlTxt.SetAttribute("subalterno", Subalterno)
        xmlTxt.SetAttribute("partita_catastale", Partita_Catastale)
        xmlTxt.SetAttribute("ettari", Ettari)
        xmlTxt.SetAttribute("are", Are)
        xmlTxt.SetAttribute("centiare", Centiare)
        xmlTxt.SetAttribute("titolo_possesso", Titolo_Possesso)
        xmlTxt.SetAttribute("qualita_catasto_codice", Qualita_Catasto_Cod)
        xmlTxt.SetAttribute("classe", Classe)
        xmlTxt.SetAttribute("reddito_dominicale", Reddito_Dominicale)
        xmlTxt.SetAttribute("reddito_agrario", Reddito_Agrario)
        xmlTxt.SetAttribute("superficie_condotta", Sup_Condotta)
        xmlTxt.SetAttribute("validita_inizio_possesso", Validita_Inizio)
        xmlTxt.SetAttribute("validita_fine_possesso", Validita_Fine)
        xmlTxt.SetAttribute("wkt", wkt)
        xmlTxt.SetAttribute("wkt_georiferimento_cod", wkt_georiferimento_Cod)

        'Restituisco in uscita 
        Return xmlTxt

    End Function


    Public Function Xml_Pubblico_Particella_MetodoProduzione(ByVal Tipo_Operazione As String,
                                                             ByVal Codice As String,
                                                             ByVal p_codice_istat_comune As String,
                                                             ByVal p_codice_istat_provincia As String,
                                                             ByVal Sezione As String,
                                                             ByVal Foglio As String,
                                                             ByVal Numero As String,
                                                             ByVal Subalterno As String,
                                                             ByVal MetodoProduzione_Cod As Integer,
                                                             ByVal Validita_Inizio As String,
                                                             ByVal Validita_Fine As String,
                                                             ByRef XmlDoc As XmlDocument
                                                             ) As XmlElement

        Dim xmlTxt As XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        'Creo il nodo 
        xmlTxt = XmlDoc.CreateElement("Particella_MetodoProduzione")

        xmlTxt.SetAttribute("tipo_operazione", Tipo_Operazione)
        xmlTxt.SetAttribute("codice_particella", Codice)
        xmlTxt.SetAttribute("p_codice_istat_comune", p_codice_istat_comune)
        xmlTxt.SetAttribute("p_codice_istat_provincia", p_codice_istat_provincia)
        xmlTxt.SetAttribute("sezione", Sezione)
        xmlTxt.SetAttribute("foglio", Foglio)
        xmlTxt.SetAttribute("numero", Numero)
        xmlTxt.SetAttribute("subalterno", Subalterno)
        xmlTxt.SetAttribute("metodoproduzione_cod", MetodoProduzione_Cod)
        xmlTxt.SetAttribute("validita_inizio_possesso", Validita_Inizio)
        xmlTxt.SetAttribute("validita_fine_possesso", Validita_Fine)

        'Restituisco in uscita 
        Return xmlTxt

    End Function

    '########################################################################################
    Public Function Xml_Pubblico_Particella_Possesso(ByVal Tipo_Operazione As String,
                                                     ByVal Partita_Catastale As String,
                                                     ByVal Titolo_Possesso As String,
                                                     ByVal Sup_Condotta As String,
                                                     ByVal Validita_Inizio As String,
                                                     ByVal Validita_Fine As String,
                                                     Optional ByRef XmlDoc As XmlDocument = Nothing
                                                     ) As XmlElement

        Dim xmlTxt As XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        'Creo il nodo 
        xmlTxt = XmlDoc.CreateElement("Possesso")

        xmlTxt.SetAttribute("tipo_operazione", Tipo_Operazione)
        xmlTxt.SetAttribute("partita_catastale", Partita_Catastale)
        xmlTxt.SetAttribute("titolo_possesso", Titolo_Possesso)
        xmlTxt.SetAttribute("superficie_condotta", Sup_Condotta)
        xmlTxt.SetAttribute("validita_inizio_possesso", Validita_Inizio)
        xmlTxt.SetAttribute("validita_fine_possesso", Validita_Fine)

        'Restituisco in uscita 
        Return xmlTxt

    End Function

    '########################################################################################
    Public Function Xml_Pubblico_Zona(ByVal Tipo_Operazione As String,
                                      ByVal Codice_Zona_Gias As String,
                                      ByVal Area As String,
                                      Optional ByRef XmlDoc As XmlDocument = Nothing,
                                      Optional ByRef Validita_Inizio As Date = AGRODATAINIZIO,
                                      Optional ByRef Validita_Fine As Date = AGRODATAFINE
                                      ) As XmlElement

        Dim xmlTxt As XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        'Creo il nodo 
        xmlTxt = XmlDoc.CreateElement("Zona")

        xmlTxt.SetAttribute("tipo_operazione", Tipo_Operazione)
        xmlTxt.SetAttribute("codice_zona_gias", Codice_Zona_Gias)
        xmlTxt.SetAttribute("area", Area)
        xmlTxt.SetAttribute("validita_inizio", Validita_Inizio.ToShortDateString)
        xmlTxt.SetAttribute("validita_fine", Validita_Fine.ToShortDateString)

        'Restituisco in uscita 
        Return xmlTxt

    End Function

    '########################################################################################
    Public Function Xml_Pubblico_Macrouso(ByVal Tipo_Operazione As String,
                                          ByVal Piva As String,
                                          ByVal Codice_Macrouso_Gias As String,
                                          ByVal Superficie As String,
                                          ByVal Validita_Inizio As String,
                                          ByVal Validita_Fine As String,
                                          Optional ByRef XmlDoc As XmlDocument = Nothing
                                          ) As XmlElement

        Dim xmlTxt As XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        'Creo il nodo 
        xmlTxt = XmlDoc.CreateElement("Macrouso")

        xmlTxt.SetAttribute("tipo_operazione", Tipo_Operazione)
        xmlTxt.SetAttribute("piva", Piva)
        xmlTxt.SetAttribute("codice_macrouso_gias", Codice_Macrouso_Gias)
        xmlTxt.SetAttribute("superficie", Superficie)
        xmlTxt.SetAttribute("validita_inizio", Validita_Inizio)
        xmlTxt.SetAttribute("validita_fine", Validita_Fine)

        'Restituisco in uscita 
        Return xmlTxt

    End Function


    '########################################################################################
    Public Function Xml_Pubblico_Utilizzo(ByVal Tipo_Operazione As String,
                                          ByVal Piva As String,
                                          ByVal Codice_Specie_Agea As String,
                                          ByVal Codice_Varieta_Agea As String,
                                          ByVal Superficie As String,
                                          ByVal Validita_Inizio As String,
                                          ByVal Validita_Fine As String,
                                          ByVal ID_Utilizzo As String,
                                          Optional ByRef XmlDoc As XmlDocument = Nothing
                                          ) As XmlElement

        Dim xmlTxt As XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        'Creo il nodo 
        xmlTxt = XmlDoc.CreateElement("Utilizzo")

        xmlTxt.SetAttribute("tipo_operazione", Tipo_Operazione)
        xmlTxt.SetAttribute("piva", Piva)
        xmlTxt.SetAttribute("codice_specie_agea", Codice_Specie_Agea)
        xmlTxt.SetAttribute("codice_varieta_agea", Codice_Varieta_Agea)
        xmlTxt.SetAttribute("superficie", Superficie)
        xmlTxt.SetAttribute("validita_inizio", Validita_Inizio)
        xmlTxt.SetAttribute("validita_fine", Validita_Fine)
        xmlTxt.SetAttribute("id_utilizzo", ID_Utilizzo)

        'Restituisco in uscita 
        Return xmlTxt

    End Function


    '########################################################################################
    Public Function Xml_Pubblico_Classamento(ByVal Tipo_Operazione As String,
                                             ByVal Qualita_Cod As String,
                                             ByVal Classe As String,
                                             ByVal Validita_Inizio As String,
                                             ByVal Validita_Fine As String,
                                             Optional ByRef XmlDoc As XmlDocument = Nothing
                                             ) As XmlElement

        Dim xmlTxt As XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        'Creo il nodo 
        xmlTxt = XmlDoc.CreateElement("Classamento")

        xmlTxt.SetAttribute("tipo_operazione", Tipo_Operazione)
        xmlTxt.SetAttribute("qualita_cod", Qualita_Cod)
        xmlTxt.SetAttribute("classe", Classe)
        xmlTxt.SetAttribute("validita_inizio", Validita_Inizio)
        xmlTxt.SetAttribute("validita_fine", Validita_Fine)

        'Restituisco in uscita 
        Return xmlTxt

    End Function


    '########################################################################################
    Public Function Xml_Pubblico_Eleggibilita(ByVal Tipo_Operazione As String,
                                              ByVal Codice_Eleggibilita As String,
                                              ByVal Superficie As String,
                                              ByVal Validita_Inizio As String,
                                              ByVal Validita_Fine As String,
                                              Optional ByRef XmlDoc As XmlDocument = Nothing
                                              ) As XmlElement

        Dim xmlTxt As XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        'Creo il nodo 
        xmlTxt = XmlDoc.CreateElement("Eleggibilita")

        xmlTxt.SetAttribute("tipo_operazione", Tipo_Operazione)
        xmlTxt.SetAttribute("codice_eleggibilita", Codice_Eleggibilita)
        xmlTxt.SetAttribute("superficie", Superficie)
        xmlTxt.SetAttribute("validita_inizio", Validita_Inizio)
        xmlTxt.SetAttribute("validita_fine", Validita_Fine)

        'Restituisco in uscita 
        Return xmlTxt

    End Function


    '########################################################################################
    Public Function Xml_Pubblico_Campo(ByVal Tipo_Operazione As String,
                                       ByVal Codice As String,
                                       ByVal Campo_Tipo As String,
                                       ByVal Nome_Campo As String,
                                       ByVal Gru_Cod As String,
                                       ByVal Veg_Cod As String,
                                       ByVal Validita_Inizio As String,
                                       ByVal Validita_Fine As String,
                                       ByVal Cam_Chiave_Cliente As String,
                                       Optional ByRef XmlDoc As XmlDocument = Nothing,
                                       Optional ByVal codice_alfanumerico_campo As String = "#",
                                       Optional ByVal superficie_contratto As Decimal = 0,
                                       Optional ByVal filiera As String = "#"
                                       ) As XmlElement

        Dim xmlTxt As XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        'Creo il nodo 
        xmlTxt = XmlDoc.CreateElement("Campo")

        'se ho codice=-1 inserisco solo il nodo, senza attributi..
        If Codice <> -1 Then
            xmlTxt.SetAttribute("tipo_operazione", Tipo_Operazione)
            xmlTxt.SetAttribute("codice_campo", Codice)
            xmlTxt.SetAttribute("campo_tipo", Campo_Tipo)
            xmlTxt.SetAttribute("campo_denominazione", Nome_Campo)
            xmlTxt.SetAttribute("codice_gruppo_vegetale_gias", Gru_Cod)
            xmlTxt.SetAttribute("codice_specie_gias", Veg_Cod)
            xmlTxt.SetAttribute("validita_inizio", Validita_Inizio)
            xmlTxt.SetAttribute("validita_fine", Validita_Fine)
            xmlTxt.SetAttribute("cam_chiave_cliente", Cam_Chiave_Cliente)
            xmlTxt.SetAttribute("codice_alfanumerico_campo", codice_alfanumerico_campo)
            xmlTxt.SetAttribute("superficie_contratto", superficie_contratto)
            xmlTxt.SetAttribute("filiera", filiera)
            'NOTA: Cam_Chiave_Cliente il WS_Importa_GIAS non lo gestisce, è commentato!
        End If

        'Restituisco in uscita 
        Return xmlTxt

    End Function

    '########################################################################################
    Public Function Xml_Pubblico_Campo_Particella(ByVal Tipo_Operazione As String,
                                                  ByVal Istat_Comune As String,
                                                  ByVal Istat_Provincia As String,
                                                  ByVal Sezione As String,
                                                  ByVal Foglio As String,
                                                  ByVal Numero As String,
                                                  ByVal Subalterno As String,
                                                  ByVal Sup As String,
                                                  Optional ByRef XmlDoc As XmlDocument = Nothing
                                                  ) As XmlElement

        Dim xmlTxt As XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        'Creo il nodo 
        xmlTxt = XmlDoc.CreateElement("Campo_Particella")

        xmlTxt.SetAttribute("tipo_operazione", Tipo_Operazione)
        xmlTxt.SetAttribute("istat_comune", Istat_Comune)
        xmlTxt.SetAttribute("istat_provincia", Istat_Provincia)
        xmlTxt.SetAttribute("sezione", Sezione)
        xmlTxt.SetAttribute("foglio", Foglio)
        xmlTxt.SetAttribute("numero", Numero)
        xmlTxt.SetAttribute("subalterno", Subalterno)
        xmlTxt.SetAttribute("superficie", Sup)

        'Restituisco in uscita 
        Return xmlTxt

    End Function

    '########################################################################################
    Public Function Xml_Pubblico_AppezzamentoCampo(ByVal Tipo_Operazione As String,
                                                   ByVal Codice As String,
                                                   ByVal Nome_Appezzamento As String,
                                                   ByVal Sup_Appezzamento As String,
                                                   ByVal Altitudine As String,
                                                   ByVal Titolo_Possesso As String,
                                                   ByVal Metodo_Produzione As String,
                                                   ByVal Contratto_Cod As String,
                                                   ByVal Validita_Inizio As String,
                                                   ByVal Validita_Fine As String,
                                                   ByVal App_Chiave_Cliente As String,
                                                   Optional ByRef XmlDoc As XmlDocument = Nothing,
                                                   Optional ByVal codice_alfanumerico As String = "#",
                                                   Optional ByVal ubicazione As String = "#",
                                                   Optional ByVal Bloccato As String = "#",
                                                   Optional ByVal Numero_Appezzamento_Bio As String = "#",
                                                   Optional ByVal LunghezzaConfine_CorpiIdrici As String = "#",
                                                   Optional ByVal LunghezzaConfine_AreeResidenziali As String = "#",
                                                   Optional ByVal LunghezzaConfine_Allevamenti As String = "#",
                                                   Optional ByVal LunghezzaConfine_VegetazioneNaturale As String = "#",
                                                   Optional ByVal Lunghezza_Capezzagna As String = "#"
                                                   ) As XmlElement

        Dim xmlTxt As XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        'Creo il nodo 
        xmlTxt = XmlDoc.CreateElement("AppezzamentoCampo")

        xmlTxt.SetAttribute("tipo_operazione", Tipo_Operazione)
        xmlTxt.SetAttribute("codice_appezzamento", Codice)
        xmlTxt.SetAttribute("appezzamento_denominazione", Nome_Appezzamento)
        xmlTxt.SetAttribute("sup_appezzamento", Sup_Appezzamento)
        xmlTxt.SetAttribute("altitudine", Altitudine)
        xmlTxt.SetAttribute("titolo_possesso", Titolo_Possesso)
        xmlTxt.SetAttribute("metodo_produzione", Metodo_Produzione)
        xmlTxt.SetAttribute("codice_contratto", Contratto_Cod)
        xmlTxt.SetAttribute("validita_inizio", Validita_Inizio)
        xmlTxt.SetAttribute("validita_fine", Validita_Fine)
        xmlTxt.SetAttribute("app_chiave_cliente", App_Chiave_Cliente)

        xmlTxt.SetAttribute("codice_alfanumerico", codice_alfanumerico)
        xmlTxt.SetAttribute("ubicazione", CStr(ubicazione))

        xmlTxt.SetAttribute("bloccato", CStr(Bloccato))

        xmlTxt.SetAttribute("numero_appezzamento_bio", Numero_Appezzamento_Bio)

        xmlTxt.SetAttribute("lunghezzaconfine_corpiidrici", LunghezzaConfine_CorpiIdrici)
        xmlTxt.SetAttribute("lunghezzaconfine_areeresidenziali", LunghezzaConfine_AreeResidenziali)
        xmlTxt.SetAttribute("lunghezzaconfine_allevamenti", LunghezzaConfine_Allevamenti)
        xmlTxt.SetAttribute("lunghezzaconfine_vegetazionenaturale", LunghezzaConfine_VegetazioneNaturale)
        xmlTxt.SetAttribute("lunghezza_capezzagna", Lunghezza_Capezzagna)

        'Restituisco in uscita 
        Return xmlTxt

    End Function

    '########################################################################################
    Public Function Xml_Pubblico_Appezzamento(ByVal Tipo_Operazione As String,
                                              ByVal Codice As String,
                                              ByVal Nome_Appezzamento As String,
                                              ByVal Numero_Appezzamento_Bio As String,
                                              ByVal Sup_Appezzamento As String,
                                              ByVal Altitudine As String,
                                              ByVal Titolo_Possesso As String,
                                              ByVal Metodo_Produzione As String,
                                              ByVal Contratto_Cod As String,
                                              ByVal Validita_Inizio As String,
                                              ByVal Validita_Fine As String,
                                              ByVal App_Chiave_Cliente As String,
                                              Optional ByRef XmlDoc As XmlDocument = Nothing,
                                              Optional ByVal codice_alfanumerico As String = "#",
                                              Optional ByVal Bloccato As String = "#",
                                              Optional ByVal LunghezzaConfine_CorpiIdrici As String = "#",
                                              Optional ByVal LunghezzaConfine_AreeResidenziali As String = "#",
                                              Optional ByVal LunghezzaConfine_Allevamenti As String = "#",
                                              Optional ByVal LunghezzaConfine_VegetazioneNaturale As String = "#",
                                              Optional ByVal Lunghezza_Capezzagna As String = "#",
                                              Optional ByVal wkt As String = "",
                                              Optional ByVal wkt_georiferimento_cod As String = ""
                                              ) As XmlElement

        Dim xmlTxt As XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        'Creo il nodo 
        xmlTxt = XmlDoc.CreateElement("Appezzamento")

        xmlTxt.SetAttribute("tipo_operazione", Tipo_Operazione)
        xmlTxt.SetAttribute("codice_appezzamento", Codice)
        xmlTxt.SetAttribute("appezzamento_denominazione", Nome_Appezzamento)
        xmlTxt.SetAttribute("numero_appezzamento_bio", Numero_Appezzamento_Bio)
        xmlTxt.SetAttribute("sup_appezzamento", Sup_Appezzamento)
        xmlTxt.SetAttribute("altitudine", Altitudine)
        xmlTxt.SetAttribute("titolo_possesso", Titolo_Possesso)
        xmlTxt.SetAttribute("metodo_produzione", Metodo_Produzione)
        xmlTxt.SetAttribute("codice_contratto", Contratto_Cod)
        xmlTxt.SetAttribute("validita_inizio", Validita_Inizio)
        xmlTxt.SetAttribute("validita_fine", Validita_Fine)
        xmlTxt.SetAttribute("app_chiave_cliente", App_Chiave_Cliente)

        xmlTxt.SetAttribute("codice_alfanumerico", codice_alfanumerico)

        xmlTxt.SetAttribute("bloccato", CStr(Bloccato))

        xmlTxt.SetAttribute("lunghezzaconfine_corpiidrici", LunghezzaConfine_CorpiIdrici)
        xmlTxt.SetAttribute("lunghezzaconfine_areeresidenziali", LunghezzaConfine_AreeResidenziali)
        xmlTxt.SetAttribute("lunghezzaconfine_allevamenti", LunghezzaConfine_Allevamenti)
        xmlTxt.SetAttribute("lunghezzaconfine_vegetazionenaturale", LunghezzaConfine_VegetazioneNaturale)
        xmlTxt.SetAttribute("lunghezza_capezzagna", Lunghezza_Capezzagna)

        xmlTxt.SetAttribute("wkt", wkt)
        xmlTxt.SetAttribute("wkt_georiferimento_cod", wkt_georiferimento_cod)

        'Restituisco in uscita 
        Return xmlTxt

    End Function

    Public Function XML_Pubblico_Indirizzo(ByVal Tipo_Operazione As String,
                                           ByVal Codice As String,
                                           ByRef Tipo_Indirizzo As Integer,
                                           ByRef Ind_Des As String,
                                           ByRef Frz_Des As String,
                                           ByRef CAP As String,
                                           ByRef Com_Des As String,
                                           ByRef Pro_Cod As String,
                                           ByRef Stato As String,
                                           ByRef Note As String,
                                           ByRef Pro_Cod_Istat As String,
                                           ByRef Com_Cod_Istat As String,
                                           ByRef Validita_Inizio As Date,
                                           ByRef Validita_Fine As Date,
                                           Optional ByRef XmlDoc As XmlDocument = Nothing
                                           ) As XmlElement

        Dim xmlTxt As XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        'Creo il nodo 
        xmlTxt = XmlDoc.CreateElement("Indirizzo")

        xmlTxt.SetAttribute("tipo_operazione", Tipo_Operazione)
        xmlTxt.SetAttribute("codice_indirizzo", Codice)
        xmlTxt.SetAttribute("tipo_indirizzo", Tipo_Indirizzo)
        xmlTxt.SetAttribute("ind_des", Ind_Des)
        xmlTxt.SetAttribute("frz_des", Frz_Des)
        xmlTxt.SetAttribute("cap", CAP)
        xmlTxt.SetAttribute("com_des", Com_Des)
        xmlTxt.SetAttribute("pro_cod", Pro_Cod)
        xmlTxt.SetAttribute("stato", Stato)
        xmlTxt.SetAttribute("note", Note)
        xmlTxt.SetAttribute("pro_cod_istat", Pro_Cod_Istat)
        xmlTxt.SetAttribute("com_cod_istat", Com_Cod_Istat)
        xmlTxt.SetAttribute("validita_inizio", Validita_Inizio)
        xmlTxt.SetAttribute("validita_fine", Validita_Fine)

        'Restituisco in uscita 
        Return xmlTxt

    End Function

    '########################################################################################
    Public Function Xml_Pubblico_Appezzamento_Particella(ByVal Tipo_Operazione As String,
                                                         ByVal Istat_Comune As String,
                                                         ByVal Istat_Provincia As String,
                                                         ByVal Sezione As String,
                                                         ByVal Foglio As String,
                                                         ByVal Numero As String,
                                                         ByVal Subalterno As String,
                                                         ByVal Sup As String,
                                                         ByVal Validita_Inizio As String,
                                                         ByVal Validita_Fine As String,
                                                         Optional ByRef XmlDoc As XmlDocument = Nothing
                                                         ) As XmlElement

        Dim xmlTxt As XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        'Creo il nodo 
        xmlTxt = XmlDoc.CreateElement("Appezzamento_Particella")

        xmlTxt.SetAttribute("tipo_operazione", Tipo_Operazione)
        xmlTxt.SetAttribute("istat_comune", Istat_Comune)
        xmlTxt.SetAttribute("istat_provincia", Istat_Provincia)
        xmlTxt.SetAttribute("sezione", Sezione)
        xmlTxt.SetAttribute("foglio", Foglio)
        xmlTxt.SetAttribute("numero", Numero)
        xmlTxt.SetAttribute("subalterno", Subalterno)
        xmlTxt.SetAttribute("superficie", Sup)
        xmlTxt.SetAttribute("validita_inizio", Validita_Inizio)
        xmlTxt.SetAttribute("validita_fine", Validita_Fine)

        'Restituisco in uscita 
        Return xmlTxt

    End Function

    '########################################################################################
    Public Function Xml_Pubblico_Impianto(ByVal Tipo_Operazione As String,
                                          ByVal Codice As String,
                                          ByVal Cod_Specie_Gias As String,
                                          ByVal Cod_Finalita_Gias As String,
                                          ByVal Cod_Varieta_Gias As String,
                                          ByVal Cod_TipologiaVarietale_Gias As String,
                                          ByVal Cod_RaggruppamentoVarietale_Gias As String,
                                          ByVal Cod_DestinazioneUso_Gias As String,
                                          ByVal Cod_Specie_Cliente As String,
                                          ByVal Cod_Finalita_Cliente As String,
                                          ByVal Cod_Varieta_Cliente As String,
                                          ByVal Titolo_Possesso As String,
                                          ByVal Metodo_Produzione As String,
                                          ByVal Resa_Prevista As String,
                                          ByVal Resa_Effettiva As String,
                                          ByVal Scarto As String,
                                          ByVal Portinnesto_Cod As String,
                                          ByVal Imp_Irrigazione_Cod As String,
                                          ByVal Regolamento_Cod As String,
                                          ByVal Forma_Allevamento_Cod As String,
                                          ByVal Copertura_Cod As String,
                                          ByVal Semina_Trapianto As String,
                                          ByVal Tra_Fila As String,
                                          ByVal Su_Fila As String,
                                          ByVal Interbina As String,
                                          ByVal Piante_Ha As String,
                                          ByVal Piano_Semina As String,
                                          ByVal Seme_Qta As String,
                                          ByVal Seme_Udm As String,
                                          ByVal Seme_Lotto As String,
                                          ByVal Sup_Imp As String,
                                          ByVal Cooperativa_Referente As String,
                                          ByVal Capitolato_Privato As String,
                                          ByVal Validita_Inizio As String,
                                          ByVal Validita_Fine As String,
                                          ByVal Imp_Chiave_Cliente As String,
                                          Optional ByRef XmlDoc As XmlDocument = Nothing,
                                          Optional ByVal Data_Semina_Prevista As Date = AGRODATAINIZIO,
                                          Optional ByVal Data_Raccolta_Prevista As Date = AGRODATAFINE,
                                          Optional ByVal unita_vitata As String = "#",
                                          Optional ByVal Dettaglio_Specie_Personalizzato As String = "#",
                                          Optional ByVal Validita_Inizio_Distinta As Date = AGRODATAINIZIO,
                                          Optional ByVal Validita_Fine_Distinta As Date = AGRODATAFINE,
                                          Optional ByVal lotto_progetto As String = "#",
                                          Optional ByVal descrizione_progetto As String = "#",
                                          Optional ByVal codice_impianto As String = "#"
                                          ) As XmlElement

        Dim xmlTxt As XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        'Creo il nodo 
        xmlTxt = XmlDoc.CreateElement("Impianto")

        xmlTxt.SetAttribute("tipo_operazione", Tipo_Operazione)
        xmlTxt.SetAttribute("codice_impianto", Codice)
        xmlTxt.SetAttribute("codice_specie_gias", Cod_Specie_Gias)
        xmlTxt.SetAttribute("codice_finalita_gias", Cod_Finalita_Gias)
        xmlTxt.SetAttribute("codice_varieta_gias", Cod_Varieta_Gias)
        xmlTxt.SetAttribute("codice_tipologia_varieta_gias", Cod_TipologiaVarietale_Gias)
        xmlTxt.SetAttribute("codice_raggruppamento_varieta_gias", Cod_RaggruppamentoVarietale_Gias)
        xmlTxt.SetAttribute("codice_destinazioneuso_gias", Cod_DestinazioneUso_Gias)
        xmlTxt.SetAttribute("codice_specie_cliente", Cod_Specie_Cliente)
        xmlTxt.SetAttribute("codice_finalita_cliente", Cod_Finalita_Cliente)
        xmlTxt.SetAttribute("codice_varieta_cliente", Cod_Varieta_Cliente)
        xmlTxt.SetAttribute("titolo_possesso", Titolo_Possesso)
        xmlTxt.SetAttribute("metodo_produzione", Metodo_Produzione)
        xmlTxt.SetAttribute("resa_prevista", Resa_Prevista)
        xmlTxt.SetAttribute("resa_effettiva", Resa_Effettiva)
        xmlTxt.SetAttribute("scarto", Scarto)
        xmlTxt.SetAttribute("codice_portinnesto", Portinnesto_Cod)
        xmlTxt.SetAttribute("codice_impianto_irrigazione", Imp_Irrigazione_Cod)
        xmlTxt.SetAttribute("codice_regolamento", Regolamento_Cod)
        xmlTxt.SetAttribute("codice_forma_allevamento", Forma_Allevamento_Cod)
        xmlTxt.SetAttribute("codice_copertura", Copertura_Cod)
        xmlTxt.SetAttribute("semina_trapianto", Semina_Trapianto)
        xmlTxt.SetAttribute("distanza_tra_fila", Tra_Fila)
        xmlTxt.SetAttribute("distanza_su_fila", Su_Fila)
        xmlTxt.SetAttribute("interbina", Interbina)
        xmlTxt.SetAttribute("piante_per_ha", Piante_Ha)
        xmlTxt.SetAttribute("piano_semina", Piano_Semina)
        xmlTxt.SetAttribute("seme_qta", Seme_Qta)
        xmlTxt.SetAttribute("seme_udm", Seme_Udm)
        xmlTxt.SetAttribute("seme_lotto", Seme_Lotto)
        xmlTxt.SetAttribute("cooperativa_referente", Cooperativa_Referente)
        xmlTxt.SetAttribute("capitolato_privato", Capitolato_Privato)
        xmlTxt.SetAttribute("validita_inizio", Validita_Inizio)
        xmlTxt.SetAttribute("validita_fine", Validita_Fine)
        xmlTxt.SetAttribute("sup_imp", Sup_Imp)
        xmlTxt.SetAttribute("imp_chiave_cliente", Imp_Chiave_Cliente)

        xmlTxt.SetAttribute(CStr("Data_Semina_Prevista").ToLower, Data_Semina_Prevista.ToShortDateString)
        xmlTxt.SetAttribute(CStr("Data_Raccolta_Prevista").ToLower, Data_Raccolta_Prevista.ToShortDateString)

        xmlTxt.SetAttribute(CStr("unita_vitata").ToLower, unita_vitata)

        xmlTxt.SetAttribute(CStr("Dettaglio_Specie_Personalizzato").ToLower, Dettaglio_Specie_Personalizzato)
        '26/10/2017 serve nei casi di importazione in cui non c'è il nodo progetto ma i dati sono riportati sull'impianto
        xmlTxt.SetAttribute(CStr("Validita_Inizio_Distinta").ToLower, Validita_Inizio_Distinta)
        xmlTxt.SetAttribute(CStr("Validita_Fine_Distinta").ToLower, Validita_Fine_Distinta)
        xmlTxt.SetAttribute(CStr("lotto_progetto").ToLower, lotto_progetto)

        '20/11/2018 fede
        xmlTxt.SetAttribute(CStr("descrizione_progetto").ToLower, descrizione_progetto)
        xmlTxt.SetAttribute(CStr("codice_impianto_anagrafe").ToLower, codice_impianto)

        'Restituisco in uscita 
        Return xmlTxt

    End Function

    Public Function Xml_Pubblico_Macchina(ByVal Tipo_Operazione As enum_TipoOperazioneDB,
                                          ByVal Mac_Cod As Integer,
                                          ByVal Class_Code As String,
                                          ByVal Mac_Des As String,
                                          Optional ByVal Costo_Acquisto As Double = 0,
                                          Optional ByVal Targa As String = "",
                                          Optional ByVal Telaio As String = "",
                                          Optional ByVal Ditta_Cod As Integer = 0,
                                          Optional ByVal Modello As String = "",
                                          Optional ByVal Potenza As String = "",
                                          Optional ByVal Ammortamento As Double = 0,
                                          Optional ByVal Ammortizzato As Double = 0,
                                          Optional ByVal Data_Immatricolazione As Date = AGRODATAINIZIO,
                                          Optional ByVal Ultima_Manutenzione As Date = AGRODATAINIZIO,
                                          Optional ByVal Ultima_Revisione As Date = AGRODATAINIZIO,
                                          Optional ByVal Stato_Utilizzo As String = "",
                                          Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
                                          Optional ByVal Validita_Fine As Date = AGRODATAFINE,
                                          Optional ByVal Note As String = "",
                                          Optional ByVal Tipo As Integer = 0,
                                          Optional ByVal N_Immatricolazione As String = "",
                                          Optional ByVal N_Immatricolazione_Rimorchio As String = "",
                                          Optional ByVal N_Autorizzazione_Trasporto As String = "",
                                          Optional ByVal Data_Rilascio_Autorizzazione As Date = AGRODATAINIZIO,
                                          Optional ByVal Peso As Double = 0,
                                          Optional ByVal Mac_Cod_Origine As Integer = 0,
                                          Optional ByVal Piva_SuperUser_Origine As String = "",
                                          Optional ByVal ChkDefault As Integer = 0,
                                          Optional ByVal Portata_Max As Double = 0,
                                          Optional ByVal Cod_Contatto As String = "",
                                          Optional ByVal CUAA_Proprietario As String = "",
                                          Optional ByVal Denominazione_Proprietario As String = "",
                                          Optional ByVal Alimentazione_Cod As Integer = 0,
                                          Optional ByVal Potenza_Udm_Cod As Integer = 0,
                                          Optional ByVal Tipo_Targa_Cod As Integer = 0,
                                          Optional ByVal Tipo_Trazione_Cod As Integer = 0,
                                          Optional ByVal N_Omologazione As String = "",
                                          Optional ByVal Ditta_Cod_Motore As Integer = 0,
                                          Optional ByVal Tipo_Motore As String = "",
                                          Optional ByVal Matricola_Motore As String = "",
                                          Optional ByVal Data_reimmatricolazione As Date = AGRODATAINIZIO,
                                          Optional ByVal Data_Carico As Date = AGRODATAINIZIO,
                                          Optional ByVal Data_Scarico As Date = AGRODATAINIZIO,
                                          Optional ByVal TitoloPossesso As Integer = 0,
                                          Optional ByVal Flag_Attrezzatura_Macchina As String = "",
                                          Optional ByVal Taratura_Ugello As Double = 0,
                                          Optional ByRef XmlDoc As XmlDocument = Nothing
                                          ) As XmlElement

        Dim xmlTxt As XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        'Creo il nodo 
        xmlTxt = XmlDoc.CreateElement("Macchina")

        xmlTxt.SetAttribute("tipo_operazione", Tipo_Operazione)
        xmlTxt.SetAttribute(CStr("Mac_Cod").ToLower, Mac_Cod.ToString)
        xmlTxt.SetAttribute(CStr("Class_Code").ToLower, Class_Code.ToString)
        xmlTxt.SetAttribute(CStr("Mac_Des").ToLower, If(Mac_Des.ToString = "", "Targa: " & Targa.ToString, Mac_Des.ToString))
        xmlTxt.SetAttribute(CStr("Costo_Acquisto").ToLower, Costo_Acquisto.ToString)
        xmlTxt.SetAttribute(CStr("Targa").ToLower, Targa.ToString)
        xmlTxt.SetAttribute(CStr("Telaio").ToLower, Telaio.ToString)
        xmlTxt.SetAttribute(CStr("Ditta_Cod").ToLower, Ditta_Cod.ToString)
        xmlTxt.SetAttribute(CStr("Modello").ToLower, Modello.ToString)
        xmlTxt.SetAttribute(CStr("Potenza").ToLower, Potenza.ToString)
        xmlTxt.SetAttribute(CStr("Ammortamento").ToLower, Ammortamento.ToString)
        xmlTxt.SetAttribute(CStr("Ammortizzato").ToLower, Ammortizzato.ToString)
        xmlTxt.SetAttribute(CStr("Data_Immatricolazione").ToLower, Data_Immatricolazione.ToShortDateString)
        xmlTxt.SetAttribute(CStr("Ultima_Manutenzione").ToLower, Ultima_Manutenzione.ToShortDateString)
        xmlTxt.SetAttribute(CStr("Ultima_Revisione").ToLower, Ultima_Revisione.ToShortDateString)
        xmlTxt.SetAttribute(CStr("Stato_Utilizzo").ToLower, Stato_Utilizzo.ToString)
        xmlTxt.SetAttribute(CStr("validita_inizio").ToLower, Validita_Inizio.ToShortDateString)
        xmlTxt.SetAttribute(CStr("validita_fine").ToLower, Validita_Fine.ToShortDateString)
        xmlTxt.SetAttribute(CStr("Note").ToLower, Note.ToString)
        xmlTxt.SetAttribute(CStr("Tipo").ToLower, Tipo.ToString)
        xmlTxt.SetAttribute(CStr("N_Immatricolazione").ToLower, N_Immatricolazione.ToString)
        xmlTxt.SetAttribute(CStr("N_Immatricolazione_Rimorchio").ToLower, N_Immatricolazione_Rimorchio.ToString)
        xmlTxt.SetAttribute(CStr("N_Autorizzazione_Trasporto").ToLower, N_Autorizzazione_Trasporto.ToString)
        xmlTxt.SetAttribute(CStr("Data_Rilascio_Autorizzazione").ToLower, Data_Rilascio_Autorizzazione.ToShortDateString)
        xmlTxt.SetAttribute(CStr("Peso").ToLower, Peso.ToString)
        xmlTxt.SetAttribute(CStr("Mac_Cod_Origine").ToLower, Mac_Cod_Origine.ToString)
        xmlTxt.SetAttribute(CStr("Piva_SuperUser_Origine").ToLower, Piva_SuperUser_Origine.ToString)
        xmlTxt.SetAttribute(CStr("ChkDefault").ToLower, ChkDefault.ToString)
        xmlTxt.SetAttribute(CStr("Portata_Max").ToLower, Portata_Max.ToString)
        xmlTxt.SetAttribute(CStr("Cod_Contatto").ToLower, Cod_Contatto.ToString)
        xmlTxt.SetAttribute(CStr("CUAA_Proprietario").ToLower, CUAA_Proprietario.ToString)
        xmlTxt.SetAttribute(CStr("Denominazione_Proprietario").ToLower, Denominazione_Proprietario.ToString)
        xmlTxt.SetAttribute(CStr("Alimentazione_Cod").ToLower, Alimentazione_Cod.ToString)
        xmlTxt.SetAttribute(CStr("Potenza_Udm_Cod").ToLower, Potenza_Udm_Cod.ToString)
        xmlTxt.SetAttribute(CStr("Tipo_Targa_Cod").ToLower, Tipo_Targa_Cod.ToString)
        xmlTxt.SetAttribute(CStr("Tipo_Trazione_Cod").ToLower, Tipo_Trazione_Cod.ToString)
        xmlTxt.SetAttribute(CStr("N_Omologazione").ToLower, N_Omologazione.ToString)
        xmlTxt.SetAttribute(CStr("Ditta_Cod_Motore").ToLower, Ditta_Cod_Motore.ToString)
        xmlTxt.SetAttribute(CStr("Tipo_Motore").ToLower, Tipo_Motore.ToString)
        xmlTxt.SetAttribute(CStr("Matricola_Motore").ToLower, Matricola_Motore.ToString)
        xmlTxt.SetAttribute(CStr("Data_Reimmatricolazione").ToLower, Data_reimmatricolazione.ToShortDateString)
        xmlTxt.SetAttribute(CStr("Data_Carico").ToLower, Data_Carico.ToString)
        xmlTxt.SetAttribute(CStr("Data_Scarico").ToLower, Data_Scarico.ToString)
        xmlTxt.SetAttribute(CStr("TitoloPossesso").ToLower, TitoloPossesso.ToString)
        xmlTxt.SetAttribute(CStr("Flag_Attrezzatura_Macchina").ToLower, Flag_Attrezzatura_Macchina.ToString)
        xmlTxt.SetAttribute(CStr("Taratura_Ugello").ToLower, Taratura_Ugello.ToString)

        'Restituisco in uscita 
        Return xmlTxt

    End Function

    '########################################################################################
    <Obsolete("Usare invece AgronicaCoreXML.XML_Pubblico_Anagrafe.Progetto [commit 2016]", False)>
    Public Function Xml_Pubblico_ProgettoPerImpianto(ByVal Tipo_Operazione As String,
                                                     ByVal Codice As String,
                                                     ByVal Descrizione_Progetto As String,
                                                     ByVal Validita_Inizio As String,
                                                     ByVal Validita_Fine As String,
                                                     Optional ByRef XmlDoc As XmlDocument = Nothing,
                                                     Optional ByRef kpin As String = "",
                                                     Optional ByRef block_name As String = "",
                                                     Optional ByRef grower_number As String = "",
                                                     Optional ByRef Produzione_Prevista As String = "0"
                                                     ) As XmlElement

        Dim xmlTxt As XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        'Creo il nodo 
        xmlTxt = XmlDoc.CreateElement("Progetto")

        xmlTxt.SetAttribute("tipo_operazione", Tipo_Operazione)
        xmlTxt.SetAttribute("codice_progetto", Codice)
        xmlTxt.SetAttribute("descrizione_progetto", Descrizione_Progetto)
        xmlTxt.SetAttribute("validita_inizio", Validita_Inizio)
        xmlTxt.SetAttribute("validita_fine", Validita_Fine)
        xmlTxt.SetAttribute("zespri_kpin", kpin)
        xmlTxt.SetAttribute("zespri_blockname", block_name)
        xmlTxt.SetAttribute("zespri_growernumber", grower_number)

        xmlTxt.SetAttribute("resa_prevista", Produzione_Prevista)

        'Restituisco in uscita 
        Return xmlTxt

    End Function

    '##########################################################################################
    <Obsolete("Usare invece AgronicaCoreXML.XML_Anagrafe [commit 2016]", False)>
    Public Sub XML_Campo(ByVal OperazioneRichiesta As enum_TipoOperazioneDB,
                         ByRef StringaXML As String,
                         ByRef TipoOperazioneDB As enum_TipoOperazioneDB,
                         ByRef Piva As String,
                         ByRef Sa_Cod As Integer,
                         ByRef Campo_Cod As Long,
                         ByRef Campo_tipo As Integer,
                         ByRef Campo_Des As String,
                         ByRef Conversione_Inizio As Date,
                         ByRef Conversione_Fine As Date,
                         ByRef SAU_Totale As Decimal,
                         ByRef SAU_Biologico As Decimal,
                         ByRef SAU_Conversione As Decimal,
                         ByRef SAU_Convenzionale As Decimal,
                         ByRef ConfiniRischio As String,
                         ByRef Gru_Cod As Integer,
                         ByRef Veg_Cod As Integer,
                         ByRef Validita_Inizio As Date,
                         ByRef Validita_Fine As Date,
                         ByRef BaseCode As Integer,
                         ByRef TopCode As Integer)

        Dim xmlDoc As New XmlDocument
        Dim xmlCampo As XmlElement

        'Verifico quale operazione deve essere effettuata
        Select Case OperazioneRichiesta

            Case enum_CodificaDecodifica.Codifica

                '----- Genero la stringa XML a partire dai valori dei parametri

                'Creo il nodo 
                xmlCampo = xmlDoc.CreateElement("Campo")

                xmlCampo.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
                xmlCampo.SetAttribute("piva", CStr(Piva))
                xmlCampo.SetAttribute("sa_cod", CStr(Sa_Cod))
                xmlCampo.SetAttribute("campo_cod", CStr(Campo_Cod))
                xmlCampo.SetAttribute("campo_tipo", CStr(Campo_tipo))
                xmlCampo.SetAttribute("campo_des", Campo_Des)
                xmlCampo.SetAttribute("conversione_inizio", CStr(Conversione_Inizio))
                xmlCampo.SetAttribute("conversione_fine", CStr(Conversione_Fine))
                xmlCampo.SetAttribute("sau_totale", CStr(SAU_Totale))
                xmlCampo.SetAttribute("sau_biologico", CStr(SAU_Biologico))
                xmlCampo.SetAttribute("sau_conversione", CStr(SAU_Conversione))
                xmlCampo.SetAttribute("sau_convenzionale", CStr(SAU_Convenzionale))
                xmlCampo.SetAttribute("confinirischio", CStr(ConfiniRischio))
                xmlCampo.SetAttribute("gru_cod", CStr(Gru_Cod))
                xmlCampo.SetAttribute("veg_cod", CStr(Veg_Cod))
                xmlCampo.SetAttribute("validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
                xmlCampo.SetAttribute("validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
                xmlCampo.SetAttribute("basecode", CStr(BaseCode))
                xmlCampo.SetAttribute("topcode", CStr(TopCode))

                'Imposto XmlIndirizzo come figlio del documento principale
                xmlDoc.AppendChild(xmlCampo)

                'Restituisco in uscita la stringa creata
                StringaXML = xmlDoc.InnerXml

                'Distruggo gli oggetti
                xmlCampo = Nothing
                xmlDoc = Nothing


            Case enum_CodificaDecodifica.Decodifica

                '----- Recupero i valori dei parametri a partire dalla stringa XML

                'Carico la stringa XML nel documento
                xmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo
                xmlCampo = xmlDoc.SelectSingleNode("//Campo")

                'Prelevo gli attributi
                TipoOperazioneDB = CInt(xmlCampo.GetAttribute("TipoOperazioneDB"))
                Campo_Cod = CInt(xmlCampo.GetAttribute("campo_cod"))
                Campo_Des = CStr(xmlCampo.GetAttribute("campo_des"))
                Campo_tipo = CStr(xmlCampo.GetAttribute("campo_tipo"))
                Gru_Cod = CInt(xmlCampo.GetAttribute("gru_cod"))
                Veg_Cod = CInt(xmlCampo.GetAttribute("veg_cod"))
                Validita_Inizio = CDate(xmlCampo.GetAttribute("validita_inizio"))
                Validita_Fine = CDate(xmlCampo.GetAttribute("validita_fine"))
                BaseCode = 0
                TopCode = 0

                'Distruggo gli oggetti
                xmlCampo = Nothing
                xmlDoc = Nothing

        End Select

    End Sub


    '##########################################################################################
    <Obsolete("Usare invece AgronicaCoreXML.XML_Anagrafe [commit 2016]", False)>
    Public Sub XML_Appezzamento(ByVal OperazioneRichiesta As enum_TipoOperazioneDB,
                                ByRef StringaXML As String,
                                ByRef TipoOperazioneDB As enum_TipoOperazioneDB,
                                ByRef Piva As String,
                                ByRef Sa_Cod As Integer,
                                ByRef Appezza As Integer,
                                ByRef Sup_App As Decimal,
                                ByRef Data_App As String,
                                ByRef Ep_Camp As String,
                                ByRef X As Decimal,
                                ByRef Y As Decimal,
                                ByRef Zslm As Decimal,
                                ByRef Esposiz As String,
                                ByRef Pende As Decimal,
                                ByRef Ubicazione As String,
                                ByRef Num_Del As Integer,
                                ByRef Clas As String,
                                ByRef Sabbia As Decimal,
                                ByRef Limo As Decimal,
                                ByRef Argilla As Decimal,
                                ByRef pH As Decimal,
                                ByRef CalTot As Decimal,
                                ByRef CalAtt As Decimal,
                                ByRef SostOrg As Decimal,
                                ByRef K2OAss As Decimal,
                                ByRef P2O5Ass As Decimal,
                                ByRef Mg As Decimal,
                                ByRef Ntot As Decimal,
                                ByRef Um_S As Decimal,
                                ByRef Cl_Dren As String,
                                ByRef Falda As Integer,
                                ByRef CsC As Decimal,
                                ByRef K2OAss_Data As String,
                                ByRef MatOrg As Decimal,
                                ByRef MatOrg_Data As String,
                                ByRef NOtot_Data As String,
                                ByRef NOtot As Decimal,
                                ByRef P2O5Ass_Data As String,
                                ByRef Suolo_CodAttri As String,
                                ByRef App_Nome As String,
                                ByRef Campo_Spia As Integer,
                                ByRef Campo_Spia_Area As Decimal,
                                ByRef Cs_SIPI As String,
                                ByRef Campo_Cod As Integer,
                                ByRef Prossimo As Integer,
                                ByRef Data_Inizio As String,
                                ByRef Data_Fine As String,
                                ByRef Validita_Inizio As Date,
                                ByRef Validita_Fine As Date,
                                ByRef BaseCode As Integer,
                                ByRef TopCode As Integer,
                                Optional DistBZ_CorpiIdrici As Decimal = 0,
                                Optional DistBZ_AreeResPub As Decimal = 0,
                                Optional DistBZ_Allevamenti As Decimal = 0,
                                Optional DistBZ_VegNatNonColt As Decimal = 0,
                                Optional SupBZ_Riduzione As Decimal = 0)

        Dim xmlDoc As New XmlDocument
        Dim xmlAppezzamento As XmlElement

        'Verifico quale operazione deve essere effettuata
        Select Case OperazioneRichiesta

            Case enum_CodificaDecodifica.Codifica

                '----- Genero la stringa XML a partire dai valori dei parametri

                'Creo il nodo 
                xmlAppezzamento = xmlDoc.CreateElement("Appezzamento")

                'Imposto gli attributi
                xmlAppezzamento.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))

                xmlAppezzamento.SetAttribute("piva", CStr(Piva))
                xmlAppezzamento.SetAttribute("sa_cod", CStr(Sa_Cod))
                xmlAppezzamento.SetAttribute("appezza", CStr(Appezza))

                xmlAppezzamento.SetAttribute("sup_app", CStr(Sup_App))
                xmlAppezzamento.SetAttribute("data_app", CStr(Data_App))
                xmlAppezzamento.SetAttribute("ep_camp", CStr(Ep_Camp))
                xmlAppezzamento.SetAttribute("x", CStr(X))
                xmlAppezzamento.SetAttribute("y", CStr(Y))
                xmlAppezzamento.SetAttribute("zslm", CStr(Zslm))
                xmlAppezzamento.SetAttribute("esposiz", CStr(Esposiz))
                xmlAppezzamento.SetAttribute("pende", CStr(Pende))
                xmlAppezzamento.SetAttribute("ubicazione", CStr(Ubicazione))
                xmlAppezzamento.SetAttribute("num_del", CStr(Num_Del))
                xmlAppezzamento.SetAttribute("clas", CStr(Clas))

                xmlAppezzamento.SetAttribute("sabbia", CStr(Sabbia))
                xmlAppezzamento.SetAttribute("limo", CStr(Limo))
                xmlAppezzamento.SetAttribute("argilla", CStr(Argilla))
                xmlAppezzamento.SetAttribute("ph", CStr(pH))

                xmlAppezzamento.SetAttribute("caltot", CStr(CalTot))
                xmlAppezzamento.SetAttribute("calatt", CStr(CalAtt))
                xmlAppezzamento.SetAttribute("sostorg", CStr(SostOrg))
                xmlAppezzamento.SetAttribute("k2oass", CStr(K2OAss))
                xmlAppezzamento.SetAttribute("p2o5ass", CStr(P2O5Ass))
                xmlAppezzamento.SetAttribute("mg", CStr(Mg))
                xmlAppezzamento.SetAttribute("ntot", CStr(Ntot))
                xmlAppezzamento.SetAttribute("um_s", CStr(Um_S))
                xmlAppezzamento.SetAttribute("cl_dren", CStr(Cl_Dren))
                xmlAppezzamento.SetAttribute("falda", CStr(Falda))
                xmlAppezzamento.SetAttribute("csc", CStr(CsC))
                xmlAppezzamento.SetAttribute("k2oass_data", CStr(K2OAss_Data))
                xmlAppezzamento.SetAttribute("matorg", CStr(MatOrg))
                xmlAppezzamento.SetAttribute("matorg_data", CStr(MatOrg_Data))
                xmlAppezzamento.SetAttribute("notot_data", CStr(NOtot_Data))
                xmlAppezzamento.SetAttribute("notot", CStr(NOtot))
                xmlAppezzamento.SetAttribute("p2o5ass_data", CStr(P2O5Ass_Data))
                xmlAppezzamento.SetAttribute("suolo_codattri", CStr(Suolo_CodAttri))

                xmlAppezzamento.SetAttribute("app_nome", CStr(App_Nome))
                xmlAppezzamento.SetAttribute("campo_spia", CStr(Campo_Spia))
                xmlAppezzamento.SetAttribute("campo_spia_area", CStr(Campo_Spia_Area))
                xmlAppezzamento.SetAttribute("cs_sipi", CStr(Cs_SIPI))
                xmlAppezzamento.SetAttribute("campo_cod", CStr(Campo_Cod))
                xmlAppezzamento.SetAttribute("prossimo", CStr(Prossimo))
                xmlAppezzamento.SetAttribute("data_inizio", CStr(Data_Inizio))
                xmlAppezzamento.SetAttribute("data_fine", CStr(Data_Fine))

                xmlAppezzamento.SetAttribute("validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
                xmlAppezzamento.SetAttribute("validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
                xmlAppezzamento.SetAttribute("basecode", CStr(BaseCode))
                xmlAppezzamento.SetAttribute("topcode", CStr(TopCode))

                xmlAppezzamento.SetAttribute("bz_corpiidrici", CStr(DistBZ_CorpiIdrici))
                xmlAppezzamento.SetAttribute("bz_areerespub", CStr(DistBZ_AreeResPub))
                xmlAppezzamento.SetAttribute("bz_allevamenti", CStr(DistBZ_Allevamenti))
                xmlAppezzamento.SetAttribute("bz_vegnatnoncolt", CStr(DistBZ_VegNatNonColt))
                xmlAppezzamento.SetAttribute("bz_supriduzione", CStr(SupBZ_Riduzione))

                'Imposto XmlIndirizzo come figlio del documento principale
                xmlDoc.AppendChild(xmlAppezzamento)

                'Restituisco in uscita la stringa creata
                StringaXML = xmlDoc.InnerXml

                'Distruggo gli oggetti
                xmlAppezzamento = Nothing
                xmlDoc = Nothing


            Case enum_CodificaDecodifica.Decodifica

                '----- Recupero i valori dei parametri a partire dalla stringa XML

                'Carico la stringa XML nel documento
                xmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo
                xmlAppezzamento = xmlDoc.SelectSingleNode("//Appezzamento")

                'Prelevo gli attributi
                TipoOperazioneDB = CInt(xmlAppezzamento.GetAttribute("TipoOperazioneDB"))

                Piva = CStr(xmlAppezzamento.GetAttribute("piva"))
                Sa_Cod = CInt(xmlAppezzamento.GetAttribute("sa_cod"))
                Appezza = CInt(xmlAppezzamento.GetAttribute("appezza"))

                Sup_App = CDbl(xmlAppezzamento.GetAttribute("sup_app"))
                Data_App = CStr(xmlAppezzamento.GetAttribute("data_app"))
                Ep_Camp = CStr(xmlAppezzamento.GetAttribute("ep_camp"))
                X = CDbl(xmlAppezzamento.GetAttribute("x"))
                Y = CDbl(xmlAppezzamento.GetAttribute("y"))
                Zslm = CDbl(xmlAppezzamento.GetAttribute("zslm"))

                Esposiz = CStr(xmlAppezzamento.GetAttribute("esposiz"))
                Pende = CDbl(xmlAppezzamento.GetAttribute("pende"))
                Ubicazione = CStr(xmlAppezzamento.GetAttribute("ubicazione"))
                Num_Del = CInt(xmlAppezzamento.GetAttribute("num_del"))
                Clas = CStr(xmlAppezzamento.GetAttribute("clas"))

                Sabbia = CDbl(xmlAppezzamento.GetAttribute("sabbia"))
                Limo = CDbl(xmlAppezzamento.GetAttribute("limo"))
                Argilla = CDbl(xmlAppezzamento.GetAttribute("argilla"))
                pH = CDbl(xmlAppezzamento.GetAttribute("ph"))

                CalTot = CDbl(xmlAppezzamento.GetAttribute("caltot"))
                CalAtt = CDbl(xmlAppezzamento.GetAttribute("calatt"))
                SostOrg = CDbl(xmlAppezzamento.GetAttribute("sostorg"))
                K2OAss = CDbl(xmlAppezzamento.GetAttribute("k2oass"))
                P2O5Ass = CDbl(xmlAppezzamento.GetAttribute("p2o5ass"))
                Mg = CDbl(xmlAppezzamento.GetAttribute("mg"))
                Ntot = CDbl(xmlAppezzamento.GetAttribute("ntot"))
                Um_S = CDbl(xmlAppezzamento.GetAttribute("um_s"))
                Cl_Dren = CStr(xmlAppezzamento.GetAttribute("cl_dren"))
                Falda = CInt(xmlAppezzamento.GetAttribute("falda"))
                CsC = CDbl(xmlAppezzamento.GetAttribute("csc"))
                K2OAss_Data = CStr(xmlAppezzamento.GetAttribute("k2oass_data"))
                MatOrg = CDbl(xmlAppezzamento.GetAttribute("matorg"))
                MatOrg_Data = CStr(xmlAppezzamento.GetAttribute("matorg_data"))
                NOtot_Data = CStr(xmlAppezzamento.GetAttribute("notot_data"))
                NOtot = CDbl(xmlAppezzamento.GetAttribute("notot"))
                P2O5Ass_Data = CStr(xmlAppezzamento.GetAttribute("p2o5ass_data"))
                Suolo_CodAttri = CStr(xmlAppezzamento.GetAttribute("suolo_codattri"))

                App_Nome = CStr(xmlAppezzamento.GetAttribute("app_nome"))
                Campo_Spia = CInt(xmlAppezzamento.GetAttribute("campo_spia"))
                Campo_Spia_Area = CDbl(xmlAppezzamento.GetAttribute("campo_spia_area"))
                Cs_SIPI = CStr(xmlAppezzamento.GetAttribute("cs_sipi"))
                Campo_Cod = CInt(xmlAppezzamento.GetAttribute("campo_cod"))
                Prossimo = CInt(xmlAppezzamento.GetAttribute("prossimo"))
                Data_Inizio = CStr(xmlAppezzamento.GetAttribute("data_inizio"))
                Data_Fine = CStr(xmlAppezzamento.GetAttribute("data_fine"))

                Validita_Inizio = CDate(xmlAppezzamento.GetAttribute("validita_inizio"))
                Validita_Fine = CDate(xmlAppezzamento.GetAttribute("validita_fine"))
                BaseCode = 0
                TopCode = 0

                'Distruggo gli oggetti
                xmlAppezzamento = Nothing
                xmlDoc = Nothing

        End Select

    End Sub

    Public Sub XML_Indirizzo(ByVal OperazioneRichiesta As enum_CodificaDecodifica,
                             ByRef StringaXML As String,
                             ByRef TipoOperazioneDB As enum_TipoOperazioneDB,
                             ByRef Tipo_Indirizzo As Integer,
                             ByRef Cod_Indirizzo As Integer,
                             ByRef Ind_Des As String,
                             ByRef Frz_Des As String,
                             ByRef CAP As String,
                             ByRef Com_Des As String,
                             ByRef Pro_Cod As String,
                             ByRef Stato As String,
                             ByRef Note As String,
                             ByRef Pro_Cod_Istat As String,
                             ByRef Com_Cod_Istat As String,
                             ByRef Validita_Inizio As Date,
                             ByRef Validita_Fine As Date,
                             ByRef BaseCode As Integer,
                             ByRef TopCode As Integer)

        Dim xmlDoc As New XmlDocument
        Dim xmlIndirizzo As XmlElement

        'Verifico quale operazione deve essere effettuata
        Select Case OperazioneRichiesta

            Case enum_CodificaDecodifica.Codifica

                '----- Genero la stringa XML a partire dai valori dei parametri

                'Creo il nodo 
                xmlIndirizzo = xmlDoc.CreateElement("Indirizzo")

                'Imposto gli attributi
                xmlIndirizzo.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
                xmlIndirizzo.SetAttribute("tipo_indirizzo", CStr(Tipo_Indirizzo))
                xmlIndirizzo.SetAttribute("cod_indirizzo", CStr(Cod_Indirizzo))
                xmlIndirizzo.SetAttribute("ind_des", Ind_Des)
                xmlIndirizzo.SetAttribute("frz_des", Frz_Des)
                xmlIndirizzo.SetAttribute("cap", CAP)
                xmlIndirizzo.SetAttribute("com_des", Com_Des)
                xmlIndirizzo.SetAttribute("pro_cod", Pro_Cod)
                xmlIndirizzo.SetAttribute("stato", Stato)
                xmlIndirizzo.SetAttribute("note", Note)
                xmlIndirizzo.SetAttribute("pro_cod_istat", Pro_Cod_Istat)
                xmlIndirizzo.SetAttribute("com_cod_istat", Com_Cod_Istat)
                xmlIndirizzo.SetAttribute("validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
                xmlIndirizzo.SetAttribute("validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
                xmlIndirizzo.SetAttribute("basecode", CStr(BaseCode))
                xmlIndirizzo.SetAttribute("topcode", CStr(TopCode))

                'Imposto XmlIndirizzo come figlio del documento principale
                xmlDoc.AppendChild(xmlIndirizzo)

                'Restituisco in uscita la stringa creata
                StringaXML = xmlDoc.InnerXml

                'Distruggo gli oggetti
                xmlIndirizzo = Nothing
                xmlDoc = Nothing


            Case enum_CodificaDecodifica.Decodifica

                '----- Recupero i valori dei parametri a partire dalla stringa XML

                'Carico la stringa XML nel documento
                xmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo
                xmlIndirizzo = xmlDoc.SelectSingleNode("//Indirizzo")

                'Prelevo gli attributi
                TipoOperazioneDB = CInt(xmlIndirizzo.GetAttribute("TipoOperazioneDB"))
                Tipo_Indirizzo = CInt(xmlIndirizzo.GetAttribute("tipo_indirizzo"))
                Cod_Indirizzo = CInt(xmlIndirizzo.GetAttribute("cod_indirizzo"))
                Ind_Des = CStr(xmlIndirizzo.GetAttribute("ind_des"))
                Frz_Des = CStr(xmlIndirizzo.GetAttribute("frz_des"))
                CAP = CStr(xmlIndirizzo.GetAttribute("cap"))
                Com_Des = CStr(xmlIndirizzo.GetAttribute("com_des"))
                Pro_Cod = CStr(xmlIndirizzo.GetAttribute("pro_cod"))
                Stato = CStr(xmlIndirizzo.GetAttribute("stato"))
                Note = CStr(xmlIndirizzo.GetAttribute("note"))
                Pro_Cod_Istat = CStr(xmlIndirizzo.GetAttribute("pro_cod_istat"))
                Com_Cod_Istat = CStr(xmlIndirizzo.GetAttribute("com_cod_istat"))
                Validita_Inizio = CDate(xmlIndirizzo.GetAttribute("validita_inizio"))
                Validita_Fine = CDate(xmlIndirizzo.GetAttribute("validita_fine"))
                BaseCode = 0
                TopCode = 0

                'Distruggo gli oggetti
                xmlIndirizzo = Nothing
                xmlDoc = Nothing

        End Select

    End Sub

    '##########################################################################################
    <Obsolete("Usare invece AgronicaCoreXML.XML_Anagrafe [commit 2016]", False)>
    Public Sub XML_Codice(ByVal OperazioneRichiesta As enum_CodificaDecodifica,
                          ByRef StringaXML As String,
                          ByRef TipoOperazioneDB As enum_TipoOperazioneDB,
                          ByRef Id_Cod As Integer,
                          ByRef Val_Cod As String,
                          ByRef Validita_Inizio As Date,
                          ByRef Validita_Fine As Date,
                          ByRef BaseCode As Integer,
                          ByRef TopCode As Integer,
                          Optional ByVal ElementoInteressato As String = "")

        'Nota: 
        'Per ElementoInteressato si intende una stringa che vale "Appezzamento" oppure "Impianto"
        'a seconda che il codice appartenga all'uno o all'altro elemento

        Dim xmlDoc As New XmlDocument
        Dim xmlCodice As XmlElement

        'Verifico quale operazione deve essere effettuata
        Select Case OperazioneRichiesta

            Case enum_CodificaDecodifica.Codifica

                '----- Genero la stringa XML a partire dai valori dei parametri

                'Creo il nodo 
                xmlCodice = xmlDoc.CreateElement("Codice" & ElementoInteressato)

                'Imposto gli attributi
                xmlCodice.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
                xmlCodice.SetAttribute("id_cod", CStr(Id_Cod))
                xmlCodice.SetAttribute("val_cod", Val_Cod)
                xmlCodice.SetAttribute("validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
                xmlCodice.SetAttribute("validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
                xmlCodice.SetAttribute("basecode", CStr(BaseCode))
                xmlCodice.SetAttribute("topcode", CStr(TopCode))

                'Imposto XmlIndirizzo come figlio del documento principale
                xmlDoc.AppendChild(xmlCodice)

                'Restituisco in uscita la stringa creata
                StringaXML = xmlDoc.InnerXml

                'Distruggo gli oggetti
                xmlCodice = Nothing
                xmlDoc = Nothing


            Case enum_CodificaDecodifica.Decodifica

                '----- Recupero i valori dei parametri a partire dalla stringa XML

                'Carico la stringa XML nel documento
                xmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo
                xmlCodice = xmlDoc.SelectSingleNode("//Codice")

                'Prelevo gli attributi
                TipoOperazioneDB = CInt(xmlCodice.GetAttribute("TipoOperazioneDB"))
                Id_Cod = CInt(xmlCodice.GetAttribute("id_cod"))
                Val_Cod = CStr(xmlCodice.GetAttribute("val_cod"))
                Validita_Inizio = CDate(xmlCodice.GetAttribute("validita_inizio"))
                Validita_Fine = CDate(xmlCodice.GetAttribute("validita_fine"))
                BaseCode = 0
                TopCode = 0

                'Distruggo gli oggetti
                xmlCodice = Nothing
                xmlDoc = Nothing

        End Select

    End Sub


    '##########################################################################################
    <Obsolete("Usare invece AgronicaCoreXML.XML_Anagrafe [commit 2016]", False)>
    Public Sub XML_Impianto(ByVal OperazioneRichiesta As enum_CodificaDecodifica,
                            ByRef StringaXML As String,
                            ByRef TipoOperazioneDB As enum_TipoOperazioneDB,
                            ByRef Piva As String,
                            ByRef Sa_Cod As Integer,
                            ByRef Campo_Cod As Integer,
                            ByRef Appezza As Integer,
                            ByRef Id_Imp As Integer,
                            ByRef Sup_Imp As Decimal,
                            ByRef Cod_Resp As Integer,
                            ByRef Cod_Ente As Integer,
                            ByRef Campo_Spia As Integer,
                            ByRef Data As Date,
                            ByRef Cul_Cod As Integer,
                            ByRef Grva_Cod_Veg As Integer,
                            ByRef Data_Raccolta As String,
                            ByRef ResaPrevista As Decimal,
                            ByRef ResaEffettiva As Decimal,
                            ByRef Scarto As Integer,
                            ByRef Ind_Mat_Cod As Integer,
                            ByRef Ind_Mat_Ril As String,
                            ByRef Sta_Ter As String,
                            ByRef Cop_DI As String,
                            ByRef Cop_DF As String,
                            ByRef Tra_Fila As Decimal,
                            ByRef Su_Fila As Decimal,
                            ByRef P_HA As Decimal,
                            ByRef Setup_Cod As String,
                            ByRef Port_Cod As Integer,
                            ByRef Stru_Prot As Integer,
                            ByRef Pro_Pag As Integer,
                            ByRef Seme_Q As Integer,
                            ByRef Seme_T As Integer,
                            ByRef Seme_P As Integer,
                            ByRef Seme_D As Integer,
                            ByRef Stato_Residui As String,
                            ByRef Denitrificazione As Integer,
                            ByRef Volatilizzazione As Integer,
                            ByRef ProfonditaLav As Integer,
                            ByRef Cover As Integer,
                            ByRef Monitorato As Integer,
                            ByRef Codice_Ficale_Tecnico As String,
                            ByRef Data_Conversione As String,
                            ByRef Grfi_Cod As Integer,
                            ByRef Imp_Cod As Integer,
                            ByRef Regolamento As Integer,
                            ByRef Finanziamento As Integer,
                            ByRef Su_Cod As Integer,
                            ByRef Cop_Cod As Integer,
                            ByRef Foral_Cod As Integer,
                            ByRef Tecn_Cod As Integer,
                            ByRef ProvenienzaSeme As Integer,
                            ByRef Validita_Inizio As Date,
                            ByRef Validita_Fine As Date,
                            ByRef BaseCode As Integer,
                            ByRef TopCode As Integer,
                            Optional ByVal Unita_Vitata As Integer = 0,
                            Optional ByVal Data_Inizio_Portinnesto As Date = AGRODATAINIZIO)

        Dim xmlDoc As New XmlDocument
        Dim xmlImpianto As XmlElement

        'Verifico quale operazione deve essere effettuata
        Select Case OperazioneRichiesta

            Case enum_CodificaDecodifica.Codifica

                '----- Genero la stringa XML a partire dai valori dei parametri

                'Creo il nodo 
                xmlImpianto = xmlDoc.CreateElement("Reg_Impianto")

                'Imposto gli attributi
                xmlImpianto.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))

                xmlImpianto.SetAttribute("piva", CStr(Piva))
                xmlImpianto.SetAttribute("sa_cod", CStr(Sa_Cod))
                xmlImpianto.SetAttribute("id_campo", CStr(Campo_Cod))
                xmlImpianto.SetAttribute("appezza", CStr(Appezza))

                xmlImpianto.SetAttribute("sup_imp", CStr(Sup_Imp))

                xmlImpianto.SetAttribute("id_reg", CStr(Id_Imp))
                xmlImpianto.SetAttribute("cod_resp", CStr(Cod_Resp))
                xmlImpianto.SetAttribute("cod_ente", CStr(Cod_Ente))
                xmlImpianto.SetAttribute("campo_spia", CStr(Campo_Spia))
                xmlImpianto.SetAttribute("data", CStr(Data))
                xmlImpianto.SetAttribute("cul_cod", CStr(Cul_Cod))
                xmlImpianto.SetAttribute("grva_cod_veg", CStr(Grva_Cod_Veg))
                xmlImpianto.SetAttribute("data_raccolta", CStr(Data_Raccolta))
                xmlImpianto.SetAttribute("resa_prevista", CStr(ResaPrevista))
                xmlImpianto.SetAttribute("resa_effettiva", CStr(ResaEffettiva))
                xmlImpianto.SetAttribute("scarto", CStr(Scarto))
                xmlImpianto.SetAttribute("ind_mat_cod", CStr(Ind_Mat_Cod))
                xmlImpianto.SetAttribute("ind_mat_ril", CStr(Ind_Mat_Ril))
                xmlImpianto.SetAttribute("sta_ter", CStr(Sta_Ter))
                xmlImpianto.SetAttribute("cop_di", CStr(Cop_DI))
                xmlImpianto.SetAttribute("cop_df", CStr(Cop_DF))
                xmlImpianto.SetAttribute("tra_fila", CStr(Tra_Fila))
                xmlImpianto.SetAttribute("su_fila", CStr(Su_Fila))
                xmlImpianto.SetAttribute("p_ha", CStr(P_HA))
                xmlImpianto.SetAttribute("setup_cod", CStr(Setup_Cod))
                xmlImpianto.SetAttribute("port_cod", CStr(Port_Cod))
                xmlImpianto.SetAttribute("stru_prot", CStr(Stru_Prot))
                xmlImpianto.SetAttribute("pro_pag", CStr(Pro_Pag))
                xmlImpianto.SetAttribute("seme_q", CStr(Seme_Q))
                xmlImpianto.SetAttribute("seme_t", CStr(Seme_T))
                xmlImpianto.SetAttribute("seme_p", CStr(Seme_P))
                xmlImpianto.SetAttribute("seme_d", CStr(Seme_D))

                xmlImpianto.SetAttribute("stato_residui", CStr(Stato_Residui))
                xmlImpianto.SetAttribute("denitrificazione", CStr(Denitrificazione))
                xmlImpianto.SetAttribute("volatilizzazione", CStr(Volatilizzazione))
                xmlImpianto.SetAttribute("profonditalav", CStr(ProfonditaLav))
                xmlImpianto.SetAttribute("cover", CStr(Cover))
                xmlImpianto.SetAttribute("monitorato", CStr(Monitorato))
                xmlImpianto.SetAttribute("codice_fiscale_tecnico", CStr(Codice_Ficale_Tecnico))
                xmlImpianto.SetAttribute("data_conversione", CStr(Data_Conversione))
                xmlImpianto.SetAttribute("grfi_cod", CStr(Grfi_Cod))
                xmlImpianto.SetAttribute("imp_cod", CStr(Imp_Cod))
                xmlImpianto.SetAttribute("regolamento", CStr(Regolamento))
                xmlImpianto.SetAttribute("finanziamento", CStr(Finanziamento))
                xmlImpianto.SetAttribute("su_cod", CStr(Su_Cod))
                xmlImpianto.SetAttribute("cop_cod", CStr(Cop_Cod))
                xmlImpianto.SetAttribute("foral_cod", CStr(Foral_Cod))
                xmlImpianto.SetAttribute("tecn_cod", CStr(Tecn_Cod))
                xmlImpianto.SetAttribute("provenienzaseme", CStr(ProvenienzaSeme))

                xmlImpianto.SetAttribute("validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
                xmlImpianto.SetAttribute("validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
                xmlImpianto.SetAttribute("basecode", CStr(BaseCode))
                xmlImpianto.SetAttribute("topcode", CStr(TopCode))

                xmlImpianto.SetAttribute("unita_vitata", CStr(Unita_Vitata))

                xmlImpianto.SetAttribute("Data_Inizio_Portinnesto", Data_Inizio_Portinnesto.ToShortDateString)

                'Imposto XmlIndirizzo come figlio del documento principale
                xmlDoc.AppendChild(xmlImpianto)

                'Restituisco in uscita la stringa creata
                StringaXML = xmlDoc.InnerXml

                'Distruggo gli oggetti
                xmlImpianto = Nothing
                xmlDoc = Nothing



            Case enum_CodificaDecodifica.Decodifica

                '----- Recupero i valori dei parametri a partire dalla stringa XML

                'Carico la stringa XML nel documento
                xmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo
                xmlImpianto = xmlDoc.SelectSingleNode("//Reg_Impianto")

                'Prelevo gli attributi
                TipoOperazioneDB = CInt(xmlImpianto.GetAttribute("TipoOperazioneDB"))

                Piva = CStr(xmlImpianto.GetAttribute("piva"))
                Sa_Cod = CInt(xmlImpianto.GetAttribute("sa_cod"))
                Campo_Cod = CInt(xmlImpianto.GetAttribute("id_campo"))
                Appezza = CInt(xmlImpianto.GetAttribute("appezza"))



                Validita_Inizio = CDate(xmlImpianto.GetAttribute("validita_inizio"))
                Validita_Fine = CDate(xmlImpianto.GetAttribute("validita_fine"))
                BaseCode = 0
                TopCode = 0

                'Distruggo gli oggetti
                xmlImpianto = Nothing
                xmlDoc = Nothing

        End Select

    End Sub


    '#################################################################################################
    <Obsolete("Usare invece AgronicaCoreXML.XML_Anagrafe [commit 2016]", False)>
    Public Sub Xml_ProgettoPerImpianto(ByVal OperazioneRichiesta As enum_CodificaDecodifica,
                                        ByRef StringaXML As String,
                                        ByRef TipoOperazioneDB As enum_TipoOperazioneDB,
                                        ByRef Piva As String,
                                        ByRef Sa_Cod As Integer,
                                        ByRef Progetto_Cod As Integer,
                                        ByRef Progetto_Nome As String,
                                        ByRef Progetto_Des As String,
                                        ByRef Cau_Progetto As Integer,
                                        ByRef Cod_Conto As Integer,
                                        ByRef Cod_Contratto As Integer,
                                        ByRef Data_Inizio_Prevista As Date,
                                        ByRef Data_Fine_Prevista As Date,
                                        ByRef Giudizio As String,
                                        ByRef Appezza As Integer,
                                        ByRef Id_Reg As Integer,
                                        ByRef Veg_Cod As Integer,
                                        ByRef Grfi_Cod As Integer,
                                        ByRef Stato_Impianto As Integer,
                                        ByRef Regolamento_Cod As Integer,
                                        ByRef Disciplinare_Cod As Integer,
                                        ByRef Regolamento_Concimazioni_Cod As Integer,
                                        ByRef Csprogetto_Cod As Integer,
                                        ByRef Ricavi_Previsti As Decimal,
                                        ByRef Produzione_Prevista As Decimal,
                                        ByRef Validita_Inizio As Date,
                                        ByRef Validita_Fine As Date,
                                        ByRef BaseCode As Integer,
                                        ByRef TopCode As Integer,
                                        Optional ByVal Data_Fioritura_Prevista As Date = AGRODATAINIZIO,
                                        Optional ByVal p_ha As Decimal = 0,
                                        Optional ByVal Disciplinare_PubblicoPrivato As Integer = 0,
                                        Optional ByVal Mat_Cod As Integer = 0)


        Dim xmlDoc As New XmlDocument

        Dim xmlDatiProgetto As XmlElement
        Dim xmlProgetto As XmlElement

        'Verifico quale operazione deve essere effettuata
        Select Case OperazioneRichiesta

            Case enum_CodificaDecodifica.Codifica

                '----- Genero la stringa XML a partire dai valori dei parametri

                xmlDatiProgetto = xmlDoc.CreateElement("DatiProgetto")
                xmlProgetto = xmlDoc.CreateElement("Progetto")

                xmlProgetto.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
                xmlProgetto.SetAttribute("piva", CStr(Piva))
                xmlProgetto.SetAttribute("sa_cod", CStr(Sa_Cod))
                xmlProgetto.SetAttribute("progetto_cod", CStr(Progetto_Cod))
                xmlProgetto.SetAttribute("progetto_nome", CStr(Progetto_Nome))
                xmlProgetto.SetAttribute("progetto_des", CStr(Progetto_Des))
                xmlProgetto.SetAttribute("cau_progetto", CInt(Cau_Progetto))
                xmlProgetto.SetAttribute("cod_conto", CInt(Cod_Conto))
                xmlProgetto.SetAttribute("cod_contratto", CInt(Cod_Contratto))
                xmlProgetto.SetAttribute("data_inizio_prevista", CDate(Data_Inizio_Prevista))
                xmlProgetto.SetAttribute("data_fine_prevista", CDate(Data_Fine_Prevista))
                xmlProgetto.SetAttribute("giudizio", CStr(Giudizio))
                xmlProgetto.SetAttribute("appezza", CInt(Appezza))
                xmlProgetto.SetAttribute("id_reg", CInt(Id_Reg))
                xmlProgetto.SetAttribute("veg_cod", CInt(Veg_Cod))
                xmlProgetto.SetAttribute("grfi_cod", CInt(Grfi_Cod))
                xmlProgetto.SetAttribute("stato_impianto", CInt(Stato_Impianto))
                xmlProgetto.SetAttribute("regolamento_cod", CInt(Regolamento_Cod))
                xmlProgetto.SetAttribute("disciplinare_cod", CInt(Disciplinare_Cod))
                xmlProgetto.SetAttribute("disciplinare_pubblicoprivato", CInt(Disciplinare_PubblicoPrivato))
                xmlProgetto.SetAttribute("regolamento_concimazioni_cod", CInt(Regolamento_Concimazioni_Cod))
                xmlProgetto.SetAttribute("csprogetto_cod", CInt(Csprogetto_Cod))
                xmlProgetto.SetAttribute("ricavi_previsti", CDbl(Ricavi_Previsti))
                xmlProgetto.SetAttribute("produzione_prevista", CDbl(Produzione_Prevista))
                xmlProgetto.SetAttribute("validita_inizio", CDate(Validita_Inizio))
                xmlProgetto.SetAttribute("validita_fine", CDate(Validita_Fine))
                xmlProgetto.SetAttribute("basecode", CInt(BaseCode))
                xmlProgetto.SetAttribute("topcode", CInt(TopCode))
                xmlProgetto.SetAttribute("data_fioritura_prevista", CDate(Data_Fioritura_Prevista))
                xmlProgetto.SetAttribute("p_ha", CDec(p_ha))
                xmlProgetto.SetAttribute("mat_cod", CInt(Mat_Cod))
                xmlDatiProgetto.AppendChild(xmlProgetto)

                'Imposto XmlIndirizzo come figlio del documento principale
                xmlDoc.AppendChild(xmlDatiProgetto)

                'Restituisco in uscita la stringa creata
                StringaXML = xmlDoc.InnerXml

                'Distruggo gli oggetti
                xmlProgetto = Nothing
                xmlDoc = Nothing



            Case enum_CodificaDecodifica.Decodifica

                '----- Recupero i valori dei parametri a partire dalla stringa XML


        End Select


    End Sub

    '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
    Public Function XML_AppezzamentoParticella(ByRef TipoOperazioneDB As String,
                                               ByRef Piva As String,
                                               ByRef Sa_Cod As Integer,
                                               ByRef Appezza As Long,
                                               ByRef Prov As String,
                                               ByRef Com As String,
                                               ByRef Sezione As String,
                                               ByRef Foglio As Integer,
                                               ByRef Numero As Integer,
                                               ByRef Subalterno As String,
                                               ByRef Area As Decimal,
                                               ByRef SAU_Convenz_Ettari As Decimal,
                                               ByRef SAU_Convenz_Are As Integer,
                                               ByRef SAU_Convenz_Centiare As Integer,
                                               ByRef SAU_Convers_Ettari As Decimal,
                                               ByRef SAU_Convers_Are As Integer,
                                               ByRef SAU_Convers_Centiare As Integer,
                                               ByRef SAU_Bio_Ettari As Decimal,
                                               ByRef SAU_Bio_Are As Integer,
                                               ByRef SAU_Bio_Centiare As Integer,
                                               ByRef Validita_Inizio As Date,
                                               ByRef Validita_Fine As Date,
                                               ByRef BaseCode As Integer,
                                               ByRef TopCode As Integer
                                               ) As String

        Dim xmlDoc As New XmlDocument
        Dim xmlAppezzamentoPart As XmlElement


        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        xmlAppezzamentoPart = xmlDoc.CreateElement("Particella")

        'Imposto gli attributi
        xmlAppezzamentoPart.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        xmlAppezzamentoPart.SetAttribute("piva", CStr(Piva))
        xmlAppezzamentoPart.SetAttribute("sa_cod", CStr(Sa_Cod))
        xmlAppezzamentoPart.SetAttribute("appezza", CStr(Appezza))
        xmlAppezzamentoPart.SetAttribute("prov", CStr(Prov))
        xmlAppezzamentoPart.SetAttribute("com", CStr(Com))
        xmlAppezzamentoPart.SetAttribute("sezione", CStr(Sezione))
        xmlAppezzamentoPart.SetAttribute("foglio", CStr(Foglio))
        xmlAppezzamentoPart.SetAttribute("numero", CStr(Numero))
        xmlAppezzamentoPart.SetAttribute("subalterno", CStr(Subalterno))
        xmlAppezzamentoPart.SetAttribute("area", CStr(Area))
        xmlAppezzamentoPart.SetAttribute("sau_convenz_ettari", CStr(SAU_Convenz_Ettari))
        xmlAppezzamentoPart.SetAttribute("sau_convenz_are", CStr(SAU_Convenz_Are))
        xmlAppezzamentoPart.SetAttribute("sau_convenz_centiare", CStr(SAU_Convenz_Centiare))
        xmlAppezzamentoPart.SetAttribute("sau_convers_ettari", CStr(SAU_Convers_Ettari))
        xmlAppezzamentoPart.SetAttribute("sau_convers_are", CStr(SAU_Convers_Are))
        xmlAppezzamentoPart.SetAttribute("sau_convers_centiare", CStr(SAU_Convers_Centiare))
        xmlAppezzamentoPart.SetAttribute("sau_bio_ettari", CStr(SAU_Bio_Ettari))
        xmlAppezzamentoPart.SetAttribute("sau_bio_are", CStr(SAU_Bio_Are))
        xmlAppezzamentoPart.SetAttribute("sau_bio_centiare", CStr(SAU_Bio_Centiare))
        xmlAppezzamentoPart.SetAttribute("validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
        xmlAppezzamentoPart.SetAttribute("validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
        xmlAppezzamentoPart.SetAttribute("basecode", CStr(BaseCode))
        xmlAppezzamentoPart.SetAttribute("topcode", CStr(TopCode))

        'Imposto XmlIndirizzo come figlio del documento principale
        xmlDoc.AppendChild(xmlAppezzamentoPart)

        'Restituisco in uscita la stringa creata
        Return xmlDoc.InnerXml

    End Function



    '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
    Public Function XML_RegImpiantiProgrammazioni(ByRef TipoOperazioneDB As String,
                                                  ByRef Piva As String,
                                                  ByRef Sa_Cod As Integer,
                                                  ByRef Appezza As Long,
                                                  ByRef Id_Reg As Integer,
                                                  ByRef Progetto_Cod As Integer,
                                                  ByRef Programmazione_Cod As Integer,
                                                  ByRef Programmazione_Entita_Cod As Integer,
                                                  ByRef Validita_Inizio As Date,
                                                  ByRef Validita_Fine As Date
                                                  ) As String

        Dim xmlDoc As New XmlDocument
        Dim xmlProgrammazione As XmlElement

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        xmlProgrammazione = xmlDoc.CreateElement("Programmazione")

        'Imposto gli attributi
        xmlProgrammazione.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        xmlProgrammazione.SetAttribute("piva", CStr(Piva))
        xmlProgrammazione.SetAttribute("sa_cod", CStr(Sa_Cod))
        xmlProgrammazione.SetAttribute("appezza", CStr(Appezza))
        xmlProgrammazione.SetAttribute("id_reg", CStr(Id_Reg))
        xmlProgrammazione.SetAttribute("progetto_cod", CStr(Progetto_Cod))
        xmlProgrammazione.SetAttribute("programmazione_cod", CStr(Programmazione_Cod))
        xmlProgrammazione.SetAttribute("programmazione_entita_cod", CStr(Programmazione_Entita_Cod))
        xmlProgrammazione.SetAttribute("validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
        xmlProgrammazione.SetAttribute("validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))

        'Imposto XmlIndirizzo come figlio del documento principale
        xmlDoc.AppendChild(xmlProgrammazione)

        'Restituisco in uscita la stringa creata
        Return xmlDoc.InnerXml

    End Function

    '########################################################################################
    Public Function Xml_Pubblico_ProgrammazioneTestata(ByVal Tipo_Operazione As String,
                                                       ByVal Codice As String,
                                                       ByVal Piva As String,
                                                       ByVal Descrizione As String,
                                                       ByVal DescrizioneLunga As String,
                                                       ByVal Note As String,
                                                       ByVal Fonte_Cod As enum_Planning_Fonte,
                                                       ByVal Tipo As String,
                                                       ByVal Validita_Inizio As String,
                                                       ByVal Validita_Fine As String,
                                                       Optional ByRef XmlDoc As XmlDocument = Nothing
                                                       ) As XmlElement

        Dim xmlTxt As XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        'Creo il nodo 
        xmlTxt = XmlDoc.CreateElement("Pianificazione")

        xmlTxt.SetAttribute("tipo_operazione", Tipo_Operazione)
        xmlTxt.SetAttribute("codice_pianificazione", Codice)
        xmlTxt.SetAttribute("pianificazione_descrizione", Descrizione)
        xmlTxt.SetAttribute("pianificazione_descrizione_lunga", DescrizioneLunga)
        xmlTxt.SetAttribute("partita_iva", Piva)
        xmlTxt.SetAttribute("note", Note)
        xmlTxt.SetAttribute("fonte_cod", Fonte_Cod)
        xmlTxt.SetAttribute("pianificazione_tipo", Tipo)
        xmlTxt.SetAttribute("validita_inizio", Validita_Inizio)
        xmlTxt.SetAttribute("validita_fine", Validita_Fine)

        'Restituisco in uscita 
        Return xmlTxt

    End Function


    '########################################################################################
    Public Function Xml_Pubblico_ProgrammazioneEntita(ByVal Tipo_Operazione As String,
                                                      ByVal Codice As String,
                                                      ByVal Descrizione As String,
                                                      ByVal Codice_Centro As String,
                                                      ByVal Codice_Campo As String,
                                                      ByVal Codice_Appezzamento As String,
                                                      ByVal Codice_Impianto As String,
                                                      ByVal Codice_Progetto As String,
                                                      ByVal Descrizione_Progetto As String,
                                                      ByVal Cod_Specie_Gias As String,
                                                      ByVal Cod_Finalita_Gias As String,
                                                      ByVal Cod_Varieta_Gias As String,
                                                      ByVal Cod_RaggruppamentoVarietale_Gias As String,
                                                      ByVal Cod_Specie_Cliente As String,
                                                      ByVal Cod_Varieta_Cliente As String,
                                                      ByVal Cod_Finalita_Cliente As String,
                                                      ByVal Cod_Macrouso As String,
                                                      ByVal Codice_Id As String,
                                                      ByVal Copertura_Cod As String,
                                                      ByVal Superficie As String,
                                                      ByVal Resa As String,
                                                      ByVal TipoZona As String,
                                                      ByVal Cod_Specie_Prec_Cliente As String,
                                                      ByVal Cod_Specie_Prec_Gias As String,
                                                      ByVal Indice_Matrici_Organiche As String,
                                                      ByVal Codice_Frequenza As String,
                                                      ByVal Azoto_Distribuito As String,
                                                      ByVal Numero_Piante As String,
                                                      ByVal Stato_Cod As String,
                                                      ByVal CicloColturale As String,
                                                      ByVal Data_Semina As String,
                                                      ByVal Data_Raccolta As String,
                                                      ByVal Validita_Inizio As String,
                                                      ByVal Validita_Fine As String,
                                                      ByVal Validita_Inizio_Impianto As String,
                                                      Optional ByRef XmlDoc As XmlDocument = Nothing,
                                                      Optional ByVal FlagIrrigabilita As String = "",
                                                      Optional ByVal FlagSecondoRaccolto As String = "",
                                                      Optional ByVal Tra_Fila As String = "0",
                                                      Optional ByVal Su_Fila As String = "0",
                                                      Optional ByVal Cod_FormaAllevamento_Gias As String = "0",
                                                      Optional ByVal Unita_Vitata As String = "0",
                                                      Optional ByVal Regolamento_Cod As String = "1",
                                                      Optional ByVal MetodoProduzione_Cod As String = "1",
                                                      Optional ByVal Conversione_Data_Inizio As String = "#",
                                                      Optional ByVal Conversione_Data_Fine As String = "#",
                                                      Optional ByVal Codice_Fiscale_Tecnico As String = "",
                                                      Optional ByVal Imp_Cod As String = "#",
                                                      Optional ByVal wkt As String = "",
                                                      Optional ByVal wkt_georiferimento_cod As String = "",
                                                      Optional ByVal Veg_Cod_Agea As String = "",
                                                      Optional ByVal Cul_Cod_Agea As String = "",
                                                      Optional ByVal Uso_Cod_Agea As String = "",
                                                      Optional ByVal Occupazione_Cod_Agea As String = "",
                                                      Optional ByVal Destinazione_Cod_Agea As String = "",
                                                      Optional ByVal Qualita_Cod_Agea As String = "",
                                                      Optional ByVal LunghezzaConfine_CorpiIdrici As String = "0",
                                                      Optional ByVal LunghezzaConfine_AreeResidenziali As String = "0",
                                                      Optional ByVal LunghezzaConfine_Allevamenti As String = "0",
                                                      Optional ByVal LunghezzaConfine_VegetazioneNaturale As String = "0",
                                                      Optional ByVal Lunghezza_Capezzagna As String = "0",
                                                      Optional ByVal Riferimento_Alfanumerico_Appezzamento As String = "",
                                                      Optional ByVal Isola As String = ""
                                                      ) As XmlElement

        Dim xmlTxt As XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        'Creo il nodo 
        xmlTxt = XmlDoc.CreateElement("Entita")

        xmlTxt.SetAttribute("tipo_operazione", Tipo_Operazione)
        xmlTxt.SetAttribute("codice_entita", Codice)
        xmlTxt.SetAttribute("entita_descrizione", Descrizione)
        xmlTxt.SetAttribute("codice_centro", Codice_Centro)
        xmlTxt.SetAttribute("codice_campo", Codice_Campo)
        xmlTxt.SetAttribute("codice_appezzamento", Codice_Appezzamento)
        xmlTxt.SetAttribute("codice_impianto", Codice_Impianto)
        xmlTxt.SetAttribute("codice_progetto", Codice_Progetto)
        xmlTxt.SetAttribute("progetto_descrizione", Descrizione_Progetto)
        xmlTxt.SetAttribute("codice_specie_gias", Cod_Specie_Gias)
        xmlTxt.SetAttribute("codice_finalita_gias", Cod_Finalita_Gias)
        xmlTxt.SetAttribute("codice_varieta_gias", Cod_Varieta_Gias)
        xmlTxt.SetAttribute("codice_tipologia_varieta_gias", Cod_RaggruppamentoVarietale_Gias)
        xmlTxt.SetAttribute("codice_specie_cliente", Cod_Specie_Cliente)
        xmlTxt.SetAttribute("codice_varieta_cliente", Cod_Varieta_Cliente)
        xmlTxt.SetAttribute("codice_finalita_cliente", Cod_Finalita_Cliente)
        xmlTxt.SetAttribute("codice_macrouso", Cod_Macrouso)
        xmlTxt.SetAttribute("codice_destinazioneuso_gias", Codice_Id)
        xmlTxt.SetAttribute("codice_copertura_gias", Copertura_Cod)
        xmlTxt.SetAttribute("superficie", Superficie)
        xmlTxt.SetAttribute("resa", Resa)
        xmlTxt.SetAttribute("tipo_zona", TipoZona)
        xmlTxt.SetAttribute("codice_specie_precedente_cliente", Cod_Specie_Prec_Cliente)
        xmlTxt.SetAttribute("codice_specie_precedente_gias", Cod_Specie_Prec_Gias)
        xmlTxt.SetAttribute("indici_matrici_organiche", Indice_Matrici_Organiche)
        xmlTxt.SetAttribute("codice_frequenza", Codice_Frequenza)
        xmlTxt.SetAttribute("azoto_distribuito", Azoto_Distribuito)
        xmlTxt.SetAttribute("piante_per_ha", Numero_Piante)
        xmlTxt.SetAttribute("codice_stato_impianto_gias", Stato_Cod)
        xmlTxt.SetAttribute("ciclo_colturale", CicloColturale)
        xmlTxt.SetAttribute("data_semina", Data_Semina)
        xmlTxt.SetAttribute("data_raccolta", Data_Raccolta)
        xmlTxt.SetAttribute("validita_inizio", Validita_Inizio)
        xmlTxt.SetAttribute("validita_fine", Validita_Fine)
        xmlTxt.SetAttribute("flagirrigabilita", FlagIrrigabilita)
        xmlTxt.SetAttribute("flagsecondoraccolto", FlagSecondoRaccolto)
        xmlTxt.SetAttribute("tra_fila", Tra_Fila)
        xmlTxt.SetAttribute("su_fila", Su_Fila)
        xmlTxt.SetAttribute("unita_vitata", Unita_Vitata)
        xmlTxt.SetAttribute("codice_formaallevamento_gias", Cod_FormaAllevamento_Gias)
        xmlTxt.SetAttribute("validita_inizio_impianto", Validita_Inizio_Impianto)
        xmlTxt.SetAttribute("Regolamento_Cod".ToLower, Regolamento_Cod)
        xmlTxt.SetAttribute("MetodoProduzione_Cod".ToLower, MetodoProduzione_Cod)
        xmlTxt.SetAttribute("Conversione_Data_Inizio", Conversione_Data_Inizio)
        xmlTxt.SetAttribute("Conversione_Data_Fine", Conversione_Data_Fine)
        xmlTxt.SetAttribute("Codice_Fiscale_Tecnico", Codice_Fiscale_Tecnico)
        xmlTxt.SetAttribute("Imp_Cod", Imp_Cod)
        xmlTxt.SetAttribute("wkt", wkt)
        xmlTxt.SetAttribute("wkt_georiferimento_cod", wkt_georiferimento_cod)
        xmlTxt.SetAttribute("Veg_Cod_Agea", Veg_Cod_Agea)
        xmlTxt.SetAttribute("Cul_Cod_Agea", Cul_Cod_Agea)
        xmlTxt.SetAttribute("Uso_Cod_Agea", Uso_Cod_Agea)
        xmlTxt.SetAttribute("Occupazione_Cod_Agea", Occupazione_Cod_Agea)
        xmlTxt.SetAttribute("Destinazione_Cod_Agea", Destinazione_Cod_Agea)
        xmlTxt.SetAttribute("Qualita_Cod_Agea", Qualita_Cod_Agea)

        xmlTxt.SetAttribute("lunghezzaconfine_corpiidrici", LunghezzaConfine_CorpiIdrici)
        xmlTxt.SetAttribute("lunghezzaconfine_areeresidenziali", LunghezzaConfine_AreeResidenziali)
        xmlTxt.SetAttribute("lunghezzaconfine_allevamenti", LunghezzaConfine_Allevamenti)
        xmlTxt.SetAttribute("lunghezzaconfine_vegetazionenaturale", LunghezzaConfine_VegetazioneNaturale)
        xmlTxt.SetAttribute("lunghezza_capezzagna", Lunghezza_Capezzagna)


        xmlTxt.SetAttribute("riferimento_alfanumerico_appezzamento", Riferimento_Alfanumerico_Appezzamento)
        xmlTxt.SetAttribute("isola", Isola)

        'Restituisco in uscita 
        Return xmlTxt

    End Function


    '########################################################################################
    Public Function Xml_Pubblico_ProgrammazioneParticella(ByVal Tipo_Operazione As String,
                                                          ByVal p_codice_istat_comune As String,
                                                          ByVal p_codice_istat_provincia As String,
                                                          ByVal Sezione As String,
                                                          ByVal Foglio As String,
                                                          ByVal Numero As String,
                                                          ByVal Subalterno As String,
                                                          ByVal Superficie As String,
                                                          ByVal Validita_Inizio As String,
                                                          ByVal Validita_Fine As String,
                                                          Optional ByRef XmlDoc As XmlDocument = Nothing
                                                          ) As XmlElement

        Dim xmlTxt As XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        'Creo il nodo 
        xmlTxt = XmlDoc.CreateElement("Entita_Particella")

        xmlTxt.SetAttribute("tipo_operazione", Tipo_Operazione)
        xmlTxt.SetAttribute("p_codice_istat_comune", p_codice_istat_comune)
        xmlTxt.SetAttribute("p_codice_istat_provincia", p_codice_istat_provincia)
        xmlTxt.SetAttribute("sezione", Sezione)
        xmlTxt.SetAttribute("foglio", Foglio)
        xmlTxt.SetAttribute("numero", Numero)
        xmlTxt.SetAttribute("subalterno", Subalterno)
        xmlTxt.SetAttribute("superficie", Superficie)
        xmlTxt.SetAttribute("validita_inizio_possesso", Validita_Inizio)
        xmlTxt.SetAttribute("validita_fine_possesso", Validita_Fine)

        'Restituisco in uscita 
        Return xmlTxt

    End Function


    '########################################################################################
    Public Function Xml_Pubblico_Contatto(ByVal Tipo_Operazione As String,
                                          ByVal Piva As String,
                                          ByVal RagioneSociale As String,
                                          ByVal CodiceFiscale As String,
                                          ByVal Nome As String,
                                          ByVal Cognome As String,
                                          ByVal Sesso As String,
                                          ByVal eMail As String,
                                          ByVal Telefono As String,
                                          ByVal Fax As String,
                                          ByVal Nascita_Data As String,
                                          ByVal Nascita_Comune As String,
                                          ByVal Nascita_Provincia As String,
                                          ByVal Nascita_Istat_Comune As String,
                                          ByVal Nascita_Istat_Provincia As String,
                                          ByVal Residenza_Indirizzo As String,
                                          ByVal Residenza_Frazione As String,
                                          ByVal Residenza_CAP As String,
                                          ByVal Residenza_Comune As String,
                                          ByVal Residenza_Provincia As String,
                                          ByVal Residenza_Istat_Comune As String,
                                          ByVal Residenza_Istat_Provincia As String,
                                          ByVal Residenza_Stato As String,
                                          ByVal Residenza_Note As String,
                                          ByVal Validita_Inizio As String,
                                          ByVal Validita_Fine As String,
                                          Optional ByRef XmlDoc As XmlDocument = Nothing,
                                          Optional ByVal FlagPubblico As Boolean = False,
                                          Optional ByVal OwnerContatto As String = "#"
                                          ) As XmlElement

        Dim xmlTxt As XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        'Creo il nodo 
        xmlTxt = XmlDoc.CreateElement("Contatto")

        xmlTxt.SetAttribute("tipo_operazione", Tipo_Operazione)
        xmlTxt.SetAttribute("partita_iva", Piva)
        xmlTxt.SetAttribute("ragione_sociale", RagioneSociale)
        xmlTxt.SetAttribute("codice_fiscale", CodiceFiscale)
        xmlTxt.SetAttribute("nome", Nome)
        xmlTxt.SetAttribute("cognome", Cognome)
        xmlTxt.SetAttribute("sesso", Sesso)

        ' rubrica
        xmlTxt.SetAttribute("email", eMail)
        xmlTxt.SetAttribute("fax", Fax)
        xmlTxt.SetAttribute("telefono", Telefono)

        'luogo di nascita
        xmlTxt.SetAttribute("nascita_data", Nascita_Data)
        xmlTxt.SetAttribute("nascita_comune", Nascita_Comune)
        xmlTxt.SetAttribute("nascita_provincia", Nascita_Provincia)
        xmlTxt.SetAttribute("nascita_istat_comune", Nascita_Istat_Comune)
        xmlTxt.SetAttribute("nascita_istat_provincia", Nascita_Istat_Provincia)

        'residenza
        xmlTxt.SetAttribute("residenza_indirizzo", Residenza_Indirizzo)
        xmlTxt.SetAttribute("residenza_frazione", Residenza_Frazione)
        xmlTxt.SetAttribute("residenza_cap", Residenza_CAP)
        xmlTxt.SetAttribute("residenza_comune", Residenza_Comune)
        xmlTxt.SetAttribute("residenza_provincia", Residenza_Provincia)
        xmlTxt.SetAttribute("residenza_stato", Residenza_Stato)
        xmlTxt.SetAttribute("residenza_note_indirizzo", Residenza_Note)
        xmlTxt.SetAttribute("residenza_codice_istat_comune", Residenza_Istat_Comune)
        xmlTxt.SetAttribute("residenza_codice_istat_provincia", Residenza_Istat_Provincia)

        xmlTxt.SetAttribute("validita_inizio", Validita_Inizio)
        xmlTxt.SetAttribute("validita_fine", Validita_Fine)

        'lavez - 05/07/2022 - per flag pubblico\privato
        xmlTxt.SetAttribute("pubblico", FlagPubblico)

        'lavez - 06/07/2022 - gestione referente del contatto
        xmlTxt.SetAttribute("owner", OwnerContatto)

        'Restituisco in uscita 
        Return xmlTxt

    End Function



    '########################################################################################
    Public Function Xml_Pubblico_Ruolo(ByVal Tipo_Operazione As String,
                                       ByVal CodiceRuolo As String,
                                       ByVal DescRuolo As String,
                                       Optional ByRef XmlDoc As XmlDocument = Nothing
                                       ) As XmlElement

        Dim xmlTxt As XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        'Creo il nodo 
        xmlTxt = XmlDoc.CreateElement("Ruolo")

        xmlTxt.SetAttribute("tipo_operazione", Tipo_Operazione)
        xmlTxt.SetAttribute("codice_ruolo", CodiceRuolo)
        xmlTxt.SetAttribute("descrizione_ruolo", DescRuolo)

        'Restituisco in uscita 
        Return xmlTxt

    End Function

End Class
