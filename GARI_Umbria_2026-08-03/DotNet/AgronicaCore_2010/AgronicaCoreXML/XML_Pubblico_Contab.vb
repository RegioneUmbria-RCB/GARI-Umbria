Imports System.Xml
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi


Public Class XML_Pubblico_Contab

    '###############################################################
    Public Function tipo_documento_from_lavcod_extraint(ByVal lav_cod As Integer, _
                                                        ByVal extra_int As Integer) As String

        Dim tipo_documento As String = ""

        Select Case lav_cod

            Case LAVCOD_BOLLA_EMESSA, LAVCOD_BOLLA_RICEVUTA
                tipo_documento = 1
            Case LAVCOD_FATTURA_EMESSA, LAVCOD_FATTURA_RICEVUTA
                If extra_int = 1 Then
                    tipo_documento = 2 'immediata
                Else
                    tipo_documento = 3 'differita
                End If
        End Select

        Return tipo_documento

    End Function

    '###############################################################
    Public Function categoria_magazzino_from_elemcod(ByVal elem_cod As Integer) As String

        Dim categoria_magazzino As String = ""

        Select Case elem_cod

            Case SEMENTI
                categoria_magazzino = "S"
            Case ALTRE_MATERIE
                categoria_magazzino = "A"
            Case FERTILIZZANTI
                categoria_magazzino = "C"
            Case FORMULATI
                categoria_magazzino = "F"
            Case CARBURANTI
                categoria_magazzino = "CA"
            Case RICAMBI
                categoria_magazzino = "R"
            Case TRASFORMATI_VEGETALI
                categoria_magazzino = "TV"
            Case Else
                categoria_magazzino = ""
        End Select

        Return categoria_magazzino

    End Function

    '###############################################################
    Public Function lotto_from_lotto(ByVal lotto As String) As String

        Dim lotto_return As String = ""

        If lotto.ToLower = "indefinito" Then
            lotto_return = ""
        Else
            lotto_return = lotto
        End If

        Return lotto_return

    End Function

    '###############################################################
    Public Function bio_from_regolamentocod(ByVal elem_cod As Integer, _
                                             ByVal pro_cod As Integer, _
                                             ByVal mat_cod As Integer, _
                                             ByVal regolamento_cod As Integer) As String

        Dim bio As String = ""

        If mat_cod <> 0 Then
            If regolamento_cod = enum_Cod_Regolamento.Regolamento_bio Then
                bio = "S"
            Else
                bio = "N"
            End If
        ElseIf pro_cod <> 0 Then
            bio = ""
        End If

        'Select Case elem_cod
        '    Case SEMENTI, ALTRE_MATERIE, _
        '        SEMILAVORATI_VEGETALI, MATERIE_VEGETALI, BENI_CONFEZ_VEGETALE, TRASFORMATI_VEGETALI, _
        '        SEMILAVORATI_ANIMALI, MATERIE_ANIMALI, BENI_CONFEZ_ANIMALE, TRASFORMATI_ANIMALI, CONFEZIONI_PRODOTTI
        'End Select

        Return bio

    End Function

    '###############################################################
    Public Function numero_registrazione_from_elemcod_procod(ByVal elem_cod As Integer, _
                                                             ByVal pro_cod As Integer) As String

        Dim numero_registrazione As String = ""

        If elem_cod = FORMULATI Then
            numero_registrazione = pro_cod
        Else
            numero_registrazione = ""
        End If

        Return numero_registrazione

    End Function

    '###############################################################
    Public Function unita_misura_from_udmcod(ByVal udm_cod As Integer) As String

        Dim unita_misura As String = ""

        Select Case udm_cod

            Case enum_UnitaMisura.KG
                unita_misura = "kg"
            Case enum_UnitaMisura.Quintali
                unita_misura = "q"
            Case enum_UnitaMisura.Tonnellate
                unita_misura = "tn"
            Case enum_UnitaMisura.Litri
                unita_misura = "lt"
            Case enum_UnitaMisura.Num_Piante
                unita_misura = "n_piante"
            Case enum_UnitaMisura.Unita_Seme
                unita_misura = "unita_seme"
            Case enum_UnitaMisura.Confezioni
                unita_misura = "conf"
            Case Else
                unita_misura = ""
        End Select

        Return unita_misura

    End Function

    '##########################################################################################
    'Crea l'xml pubblico della bolla di accettazione (da diversi)
    Public Function XML_BollaAccettazionedaDiversi(ByVal Tipo_Invio As enum_TipoInvio_ExportBolleAccettazione,
                                                    ByVal Data_Esportazione As Date,
                                                    ByVal Ora_Esportazione As String,
                                                   ByVal tipo_esportazione As String,
                                                    ByVal numero_bolla As String,
                                                    ByVal data_bolla As Date,
                                                    ByVal ora_bolla As String,
                                                    ByVal numero_ddt As String,
                                                    ByVal data_ddt As String,
                                                    ByVal numero_colli As Integer,
                                                    ByVal note As String,
                                                    ByVal piva_conferitore As String,
                                                    ByVal piva_conferente As String,
                                                    ByVal piva_coop As String,
                                                    ByVal piva_coop_2 As String,
                                                    ByVal piva_produttore As String,
                                                    ByVal peso_totale As Decimal,
                                                    ByVal tara_veicolo As Decimal,
                                                    ByVal peso_lordo As Decimal,
                                                    ByVal tara_imballi As Decimal,
                                                    ByVal peso_netto As Decimal,
                                                    ByVal degrado As Decimal,
                                                    ByVal peso_netto_pagamento As Decimal,
                                                    ByVal udm_pesi As String,
                                                    ByVal fabbricato_des As String,
                                                    ByVal piva_cf_vettore As String,
                                                    ByVal ragsoc_vettore As String,
                                                    ByVal DT_Dettagli As DataTable,
                                                    ByVal DT_Impianti As DataTable,
                                                    Optional ByRef XmlDoc As XmlDocument = Nothing) _
                                                    As XmlElement


        Dim XML_Bolla As System.Xml.XmlElement
        Dim XML_numero_bolla As System.Xml.XmlElement
        Dim XML_data_bolla As System.Xml.XmlElement
        Dim XML_ora_bolla As System.Xml.XmlElement
        Dim XML_numero_ddt As System.Xml.XmlElement
        Dim XML_data_ddt As System.Xml.XmlElement
        Dim XML_numero_colli As System.Xml.XmlElement
        Dim XML_note As System.Xml.XmlElement
        Dim XML_piva_conferitore As System.Xml.XmlElement
        Dim XML_piva_conferente As System.Xml.XmlElement
        Dim XML_piva_coop As System.Xml.XmlElement
        Dim XML_piva_coop_2 As System.Xml.XmlElement
        Dim XML_piva_produttore As System.Xml.XmlElement
        Dim XML_dettagli As System.Xml.XmlElement
        Dim XML_dettaglio As System.Xml.XmlElement
        Dim XML_impianti As System.Xml.XmlElement
        Dim XML_impianto As System.Xml.XmlElement
        Dim XML_peso_totale As System.Xml.XmlElement
        Dim XML_tara_veicolo As System.Xml.XmlElement
        Dim XML_peso_lordo As System.Xml.XmlElement
        Dim XML_tara_imballi As System.Xml.XmlElement
        Dim XML_peso_netto As System.Xml.XmlElement
        Dim XML_degrado As System.Xml.XmlElement
        Dim XML_peso_netto_pagamento As System.Xml.XmlElement
        Dim XML_udm_pesi As System.Xml.XmlElement
        Dim XML_magazzino, XML_piva_cf_vettore, XML_ragsoc_vettore As XmlElement

        Dim i As Integer


        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        XML_Bolla = XmlDoc.CreateElement("bolla")

        'Imposto gli attributi
        XML_Bolla.SetAttribute(LCase("Tipo_Invio"), CStr(Tipo_Invio))
        XML_Bolla.SetAttribute(LCase("Data_Esportazione"), CStr(Data_Esportazione))
        XML_Bolla.SetAttribute(LCase("Ora_Esportazione"), CStr(Ora_Esportazione))
        XML_Bolla.SetAttribute(LCase("tipo_esportazione"), CStr(tipo_esportazione))

        XML_numero_bolla = XmlDoc.CreateElement("numero_bolla")
        XML_numero_bolla.InnerText = numero_bolla
        XML_Bolla.AppendChild(XML_numero_bolla)

        XML_data_bolla = XmlDoc.CreateElement("data_bolla")
        XML_data_bolla.InnerText = CStr(data_bolla)
        XML_Bolla.AppendChild(XML_data_bolla)

        XML_ora_bolla = XmlDoc.CreateElement("ora_bolla")
        XML_ora_bolla.InnerText = ora_bolla
        XML_Bolla.AppendChild(XML_ora_bolla)

        XML_numero_ddt = XmlDoc.CreateElement("numero_ddt")
        XML_numero_ddt.InnerText = numero_ddt
        XML_Bolla.AppendChild(XML_numero_ddt)

        XML_data_ddt = XmlDoc.CreateElement("data_ddt")
        XML_data_ddt.InnerText = data_ddt
        XML_Bolla.AppendChild(XML_data_ddt)

        XML_numero_colli = XmlDoc.CreateElement("numero_colli")
        XML_numero_colli.InnerText = CStr(numero_colli)
        XML_Bolla.AppendChild(XML_numero_colli)

        XML_note = XmlDoc.CreateElement("note")
        If note <> "" Then
            XML_note.InnerText = note
        Else
            XML_note.InnerText = "#"
        End If
        XML_Bolla.AppendChild(XML_note)

        XML_piva_conferitore = XmlDoc.CreateElement("piva_conferitore")
        XML_piva_conferitore.InnerText = piva_conferitore
        XML_Bolla.AppendChild(XML_piva_conferitore)

        XML_piva_conferente = XmlDoc.CreateElement("piva_conferente")
        XML_piva_conferente.InnerText = piva_conferente
        XML_Bolla.AppendChild(XML_piva_conferente)

        XML_piva_coop = XmlDoc.CreateElement("piva_coop")
        If piva_coop <> "" Then
            XML_piva_coop.InnerText = piva_coop
        Else
            XML_piva_coop.InnerText = "#"
        End If
        XML_Bolla.AppendChild(XML_piva_coop)

        XML_piva_coop_2 = XmlDoc.CreateElement("piva_coop_2")
        If piva_coop_2 <> "" Then
            XML_piva_coop_2.InnerText = piva_coop_2
        Else
            XML_piva_coop_2.InnerText = "#"
        End If
        XML_Bolla.AppendChild(XML_piva_coop_2)

        XML_piva_produttore = XmlDoc.CreateElement("piva_produttore")
        If piva_produttore <> "" Then
            XML_piva_produttore.InnerText = piva_produttore
        Else
            XML_piva_produttore.InnerText = "#"
        End If
        XML_Bolla.AppendChild(XML_piva_produttore)

        'DETTAGLI
        XML_dettagli = XmlDoc.CreateElement("dettagli")
        XML_Bolla.AppendChild(XML_dettagli)

        For i = 0 To DT_Dettagli.Rows.Count - 1

            XML_dettaglio = XML_BollaAccettazionedaDiversi_Dettagli(XmlDoc, DT_Dettagli.Rows(i))
            XML_dettagli.AppendChild(XML_dettaglio)

        Next
        '-------------

        'IMPIANTI
        XML_impianti = XmlDoc.CreateElement("impianti")
        XML_Bolla.AppendChild(XML_impianti)

        For i = 0 To DT_Impianti.Rows.Count - 1

            XML_impianto = XML_BollaAccettazionedaDiversi_Impianti(XmlDoc, DT_Impianti.Rows(i))
            XML_impianti.AppendChild(XML_impianto)

        Next
        '-------------

        XML_peso_totale = XmlDoc.CreateElement("peso_totale")
        XML_peso_totale.InnerText = CStr(peso_totale)
        XML_Bolla.AppendChild(XML_peso_totale)

        XML_tara_veicolo = XmlDoc.CreateElement("tara_veicolo")
        XML_tara_veicolo.InnerText = CStr(tara_veicolo)
        XML_Bolla.AppendChild(XML_tara_veicolo)

        XML_peso_lordo = XmlDoc.CreateElement("peso_lordo")
        XML_peso_lordo.InnerText = CStr(peso_lordo)
        XML_Bolla.AppendChild(XML_peso_lordo)

        XML_tara_imballi = XmlDoc.CreateElement("tara_imballi")
        XML_tara_imballi.InnerText = CStr(tara_imballi)
        XML_Bolla.AppendChild(XML_tara_imballi)

        XML_peso_netto = XmlDoc.CreateElement("peso_netto")
        XML_peso_netto.InnerText = CStr(peso_netto)
        XML_Bolla.AppendChild(XML_peso_netto)

        XML_degrado = XmlDoc.CreateElement("degrado")
        XML_degrado.InnerText = CStr(degrado)
        XML_Bolla.AppendChild(XML_degrado)

        XML_peso_netto_pagamento = XmlDoc.CreateElement("peso_netto_pagamento")
        XML_peso_netto_pagamento.InnerText = CStr(peso_netto_pagamento)
        XML_Bolla.AppendChild(XML_peso_netto_pagamento)

        XML_udm_pesi = XmlDoc.CreateElement("udm_pesi")
        XML_udm_pesi.InnerText = udm_pesi
        XML_Bolla.AppendChild(XML_udm_pesi)

        '---------------------------------------------------
        '02/02/2016: modifica x richiesta di terremerse:
        'aggiunta info sul magazzino e sul trasportatore
        XML_magazzino = XmlDoc.CreateElement("magazzino")
        XML_magazzino.InnerText = fabbricato_des
        XML_Bolla.AppendChild(XML_magazzino)

        XML_piva_cf_vettore = XmlDoc.CreateElement("piva_cf_vettore")
        XML_piva_cf_vettore.InnerText = piva_cf_vettore
        XML_Bolla.AppendChild(XML_piva_cf_vettore)

        XML_ragsoc_vettore = XmlDoc.CreateElement("ragsoc_vettore")
        XML_ragsoc_vettore.InnerText = ragsoc_vettore
        XML_Bolla.AppendChild(XML_ragsoc_vettore)
        '---------------------------------------------------

        Return XML_Bolla

        'Distruggo gli oggetti
        XML_Bolla = Nothing


    End Function


    '########################################################################################
    Public Function DtForXml_Genera_BollaAccettazionedaDiversi_Dettagli() As DataTable

        '----- Definizione delle variabili

        Dim Dt As New DataTable

        '----- Definisco la struttura del DataTable

        With Dt.Columns

            .Add(New DataColumn("codice_specie_gias", GetType(Integer)))
            .Add(New DataColumn("codice_varieta_gias", GetType(Integer)))
            .Add(New DataColumn("codice_tipvarietale_gias", GetType(Integer)))
            .Add(New DataColumn("codice_prodotto", GetType(String)))
            .Add(New DataColumn("descrizione_prodotto", GetType(String)))
            .Add(New DataColumn("bio", GetType(String)))
            .Add(New DataColumn("lotto", GetType(String)))
            .Add(New DataColumn("codice_param_qualitativo", GetType(Integer)))
            .Add(New DataColumn("udm_param_qualitativo", GetType(String)))
            .Add(New DataColumn("rilievo_param_qualitativo", GetType(Integer)))
            .Add(New DataColumn("codice_campionatura", GetType(Integer)))
            .Add(New DataColumn("desc_campionatura", GetType(String)))
            .Add(New DataColumn("grado_tenderom", GetType(Decimal)))
            .Add(New DataColumn("residuo_secco", GetType(Decimal)))
            .Add(New DataColumn("degrado_perc", GetType(Decimal)))
            .Add(New DataColumn("peso_netto_dettaglio", GetType(Decimal)))
            .Add(New DataColumn("degrado_dettaglio", GetType(Decimal)))
            .Add(New DataColumn("peso_netto_pagamento_dettaglio", GetType(Decimal)))
            .Add(New DataColumn("udm_dettaglio", GetType(String)))
            .Add(New DataColumn("prezzo_unitario", GetType(Decimal)))

        End With

        '----- Definisco le chiavi per il datatable

        'Dim DtKeys(2) As DataColumn

        ''Valorizzo le celle del vettore
        'DtKeys(0) = Dt.Columns("abc")
        'DtKeys(1) = Dt.Columns("def")
        'DtKeys(2) = Dt.Columns("ghi")

        ''Assegno il vettore delle chiavi al DataTable
        'Dt.PrimaryKey = DtKeys

        Return Dt

    End Function

    '########################################################################################
    Public Sub DtForXml_InserisciRiga_BollaAccettazionedaDiversi_Dettagli_OLD(
                                                    ByRef DT As DataTable,
                                                    ByVal codice_specie_gias As String,
                                                    ByVal codice_varieta_gias As String,
                                                    ByVal codice_tipvarietale_gias As String,
                                                    ByVal codice_prodotto As String,
                                                    ByVal descrizione_prodotto As String,
                                                    ByVal bio As String,
                                                    ByVal lotto As String,
                                                    ByVal codice_param_qualitativo As Integer,
                                                    ByVal udm_param_qualitativo As String,
                                                    ByVal rilievo_param_qualitativo As Integer,
                                                    ByVal codice_campionatura As Integer,
                                                    ByVal degrado_perc As Decimal,
                                                    ByVal peso_netto_dettaglio As Decimal,
                                                    ByVal degrado_dettaglio As Decimal,
                                                    ByVal peso_netto_pagamento_dettaglio As Decimal,
                                                    ByVal udm_dettaglio As String,
                                                    ByVal prezzo_unitario As Decimal)

        Dim Dr As DataRow

        '----- Creo una nuova riga

        Dr = DT.NewRow

        '----- Definisco i valori

        With Dr

            .Item("codice_specie_gias") = codice_specie_gias
            .Item("codice_varieta_gias") = codice_varieta_gias
            .Item("codice_tipvarietale_gias") = codice_tipvarietale_gias
            .Item("codice_prodotto") = codice_prodotto
            .Item("descrizione_prodotto") = descrizione_prodotto
            .Item("bio") = bio
            .Item("lotto") = lotto
            .Item("codice_param_qualitativo") = codice_param_qualitativo
            .Item("udm_param_qualitativo") = udm_param_qualitativo
            .Item("rilievo_param_qualitativo") = rilievo_param_qualitativo
            .Item("codice_campionatura") = codice_campionatura
            .Item("degrado_perc") = degrado_perc
            .Item("peso_netto_dettaglio") = peso_netto_dettaglio
            .Item("degrado_dettaglio") = degrado_dettaglio
            .Item("peso_netto_pagamento_dettaglio") = peso_netto_pagamento_dettaglio
            .Item("udm_dettaglio") = udm_dettaglio
            .Item("prezzo_unitario") = prezzo_unitario

        End With

        '----- Associo al datatable la nuova riga creata

        DT.Rows.Add(Dr)

    End Sub

    '########################################################################################
    Public Sub DtForXml_InserisciRiga_BollaAccettazionedaDiversi_Dettagli_NEW(
                                                    ByRef DT As DataTable,
                                                    ByVal codice_specie_gias As String,
                                                    ByVal codice_varieta_gias As String,
                                                    ByVal codice_tipvarietale_gias As String,
                                                    ByVal codice_prodotto As String,
                                                    ByVal descrizione_prodotto As String,
                                                    ByVal bio As String,
                                                    ByVal lotto As String,
                                                    ByVal codice_param_qualitativo As Integer,
                                                    ByVal udm_param_qualitativo As String,
                                                    ByVal rilievo_param_qualitativo As Integer,
                                                    ByVal codice_campionatura As Integer,
                                                    ByVal desc_campionatura As String,
                                                    ByVal grado_tender As Decimal,
                                                    ByVal residuo_secco As Decimal,
                                                    ByVal degrado_perc As Decimal,
                                                    ByVal peso_netto_dettaglio As Decimal,
                                                    ByVal degrado_dettaglio As Decimal,
                                                    ByVal peso_netto_pagamento_dettaglio As Decimal,
                                                    ByVal udm_dettaglio As String,
                                                    ByVal prezzo_unitario As Decimal)

        Dim Dr As DataRow

        '----- Creo una nuova riga

        Dr = DT.NewRow

        '----- Definisco i valori

        With Dr

            .Item("codice_specie_gias") = codice_specie_gias
            .Item("codice_varieta_gias") = codice_varieta_gias
            .Item("codice_tipvarietale_gias") = codice_tipvarietale_gias
            .Item("codice_prodotto") = codice_prodotto
            .Item("descrizione_prodotto") = descrizione_prodotto
            .Item("bio") = bio
            .Item("lotto") = lotto
            .Item("codice_param_qualitativo") = codice_param_qualitativo
            .Item("udm_param_qualitativo") = udm_param_qualitativo
            .Item("rilievo_param_qualitativo") = rilievo_param_qualitativo
            .Item("codice_campionatura") = codice_campionatura
            .Item("desc_campionatura") = desc_campionatura
            .Item("grado_tenderom") = grado_tender
            .Item("residuo_secco") = residuo_secco
            .Item("degrado_perc") = degrado_perc
            .Item("peso_netto_dettaglio") = peso_netto_dettaglio
            .Item("degrado_dettaglio") = degrado_dettaglio
            .Item("peso_netto_pagamento_dettaglio") = peso_netto_pagamento_dettaglio
            .Item("udm_dettaglio") = udm_dettaglio
            .Item("prezzo_unitario") = prezzo_unitario

        End With

        '----- Associo al datatable la nuova riga creata

        DT.Rows.Add(Dr)

    End Sub



    '##########################################################################################
    Public Function XML_BollaAccettazionedaDiversi_Dettagli(ByRef XmlDoc As XmlDocument, _
                                                            ByVal Dr As DataRow) _
                                                            As XmlElement

        Dim XML_dettaglio As System.Xml.XmlElement
        Dim XML_codice_specie_gias As System.Xml.XmlElement
        Dim XML_codice_varieta_gias As System.Xml.XmlElement
        Dim XML_codice_tipvarietale_gias As System.Xml.XmlElement
        Dim XML_codice_prodotto As System.Xml.XmlElement
        Dim XML_descrizione_prodotto As System.Xml.XmlElement
        Dim XML_bio As System.Xml.XmlElement
        Dim XML_lotto As System.Xml.XmlElement
        Dim XML_codice_param_qualitativo As System.Xml.XmlElement
        Dim XML_udm_param_qualitativo As System.Xml.XmlElement
        Dim XML_rilievo_param_qualitativo As System.Xml.XmlElement
        Dim XML_codice_campionatura As System.Xml.XmlElement
        Dim XML_desc_campionatura As System.Xml.XmlElement
        Dim XML_grado_tender As System.Xml.XmlElement
        Dim XML_residuo_secco As System.Xml.XmlElement
        Dim XML_degrado_perc As System.Xml.XmlElement
        Dim XML_peso_netto_dettaglio As System.Xml.XmlElement
        Dim XML_degrado_dettaglio As System.Xml.XmlElement
        Dim XML_peso_netto_pagamento_dettaglio As System.Xml.XmlElement
        Dim XML_udm_dettaglio As System.Xml.XmlElement
        Dim XML_prezzo_unitario As System.Xml.XmlElement


        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        XML_dettaglio = XmlDoc.CreateElement("dettaglio")

        XML_codice_specie_gias = XmlDoc.CreateElement("codice_specie_gias")
        XML_codice_specie_gias.InnerText = CStr(Dr.Item("codice_specie_gias"))
        XML_dettaglio.AppendChild(XML_codice_specie_gias)

        XML_codice_varieta_gias = XmlDoc.CreateElement("codice_varieta_gias")
        XML_codice_varieta_gias.InnerText = CStr(Dr.Item("codice_varieta_gias"))
        XML_dettaglio.AppendChild(XML_codice_varieta_gias)

        XML_codice_tipvarietale_gias = XmlDoc.CreateElement("codice_tipvarietale_gias")
        If Dr.Item("codice_tipvarietale_gias") <> 0 Then
            XML_codice_tipvarietale_gias.InnerText = CStr(Dr.Item("codice_tipvarietale_gias"))
        Else
            XML_codice_tipvarietale_gias.InnerText = "#"
        End If
        XML_dettaglio.AppendChild(XML_codice_tipvarietale_gias)

        XML_codice_prodotto = XmlDoc.CreateElement("codice_prodotto")
        XML_codice_prodotto.InnerText = Dr.Item("codice_prodotto")
        XML_dettaglio.AppendChild(XML_codice_prodotto)

        XML_descrizione_prodotto = XmlDoc.CreateElement("descrizione_prodotto")
        XML_descrizione_prodotto.InnerText = Dr.Item("descrizione_prodotto")
        XML_dettaglio.AppendChild(XML_descrizione_prodotto)

        XML_bio = XmlDoc.CreateElement("bio")
        XML_bio.InnerText = Dr.Item("bio")
        XML_dettaglio.AppendChild(XML_bio)

        XML_lotto = XmlDoc.CreateElement("lotto")
        If Dr.Item("lotto") <> "" Then
            XML_lotto.InnerText = Dr.Item("lotto")
        Else
            XML_lotto.InnerText = "#"
        End If
        XML_dettaglio.AppendChild(XML_lotto)

        XML_codice_param_qualitativo = XmlDoc.CreateElement("codice_param_qualitativo")
        XML_codice_param_qualitativo.InnerText = CStr(Dr.Item("codice_param_qualitativo"))
        XML_dettaglio.AppendChild(XML_codice_param_qualitativo)

        XML_udm_param_qualitativo = XmlDoc.CreateElement("udm_param_qualitativo")
        XML_udm_param_qualitativo.InnerText = Dr.Item("udm_param_qualitativo")
        XML_dettaglio.AppendChild(XML_udm_param_qualitativo)

        XML_rilievo_param_qualitativo = XmlDoc.CreateElement("rilievo_param_qualitativo")
        XML_rilievo_param_qualitativo.InnerText = CStr(Dr.Item("rilievo_param_qualitativo"))
        XML_dettaglio.AppendChild(XML_rilievo_param_qualitativo)

        XML_codice_campionatura = XmlDoc.CreateElement("codice_campionatura")
        XML_codice_campionatura.InnerText = CStr(Dr.Item("codice_campionatura"))
        XML_dettaglio.AppendChild(XML_codice_campionatura)

        '----------------------------------------------------
        '14/09/2020: introdotti
        If Not IsNothing(Dr.Item("desc_campionatura")) AndAlso Not IsDBNull(Dr.Item("desc_campionatura")) Then
            XML_desc_campionatura = XmlDoc.CreateElement("desc_campionatura")
            XML_desc_campionatura.InnerText = CStr(Dr.Item("desc_campionatura"))
            XML_dettaglio.AppendChild(XML_desc_campionatura)
        End If

        If Not IsNothing(Dr.Item("grado_tenderom")) AndAlso Not IsDBNull(Dr.Item("grado_tenderom")) Then
            XML_grado_tender = XmlDoc.CreateElement("grado_tenderom")
            XML_grado_tender.InnerText = CStr(Dr.Item("grado_tenderom"))
            XML_dettaglio.AppendChild(XML_grado_tender)
        End If

        If Not IsNothing(Dr.Item("residuo_secco")) And Not IsDBNull(Dr.Item("residuo_secco")) Then
            XML_residuo_secco = XmlDoc.CreateElement("residuo_secco")
            XML_residuo_secco.InnerText = CStr(Dr.Item("residuo_secco"))
            XML_dettaglio.AppendChild(XML_residuo_secco)
        End If
        '----------------------------------------------------

        XML_degrado_perc = XmlDoc.CreateElement("degrado_perc")
        XML_degrado_perc.InnerText = CStr(Dr.Item("degrado_perc"))
        XML_dettaglio.AppendChild(XML_degrado_perc)

        XML_peso_netto_dettaglio = XmlDoc.CreateElement("peso_netto_dettaglio")
        XML_peso_netto_dettaglio.InnerText = CStr(Dr.Item("peso_netto_dettaglio"))
        XML_dettaglio.AppendChild(XML_peso_netto_dettaglio)

        XML_degrado_dettaglio = XmlDoc.CreateElement("degrado_dettaglio")
        XML_degrado_dettaglio.InnerText = CStr(Dr.Item("degrado_dettaglio"))
        XML_dettaglio.AppendChild(XML_degrado_dettaglio)

        XML_peso_netto_pagamento_dettaglio = XmlDoc.CreateElement("peso_netto_pagamento_dettaglio")
        XML_peso_netto_pagamento_dettaglio.InnerText = CStr(Dr.Item("peso_netto_pagamento_dettaglio"))
        XML_dettaglio.AppendChild(XML_peso_netto_pagamento_dettaglio)

        XML_udm_dettaglio = XmlDoc.CreateElement("udm_dettaglio")
        XML_udm_dettaglio.InnerText = Dr.Item("udm_dettaglio")
        XML_dettaglio.AppendChild(XML_udm_dettaglio)

        XML_prezzo_unitario = XmlDoc.CreateElement("prezzo_unitario")
        XML_prezzo_unitario.InnerText = CStr(Dr.Item("prezzo_unitario"))
        XML_dettaglio.AppendChild(XML_prezzo_unitario)


        'Restituisco in uscita 
        Return XML_dettaglio

        'Distruggo gli oggetti
        XML_dettaglio = Nothing


    End Function


    '########################################################################################
    Public Function DtForXml_Genera_BollaAccettazionedaDiversi_Impianti() As DataTable

        '----- Definizione delle variabili

        Dim Dt As New DataTable

        '----- Definisco la struttura del DataTable

        With Dt.Columns

            .Add(New DataColumn("lotto_impianto", GetType(String)))
            .Add(New DataColumn("sup", GetType(Decimal)))
            .Add(New DataColumn("data_inizio", GetType(String)))
            .Add(New DataColumn("codice_finalita", GetType(Integer)))
            .Add(New DataColumn("codice_tipologia_varietale", GetType(Integer)))

        End With

        '----- Definisco le chiavi per il datatable

        'Dim DtKeys(2) As DataColumn

        ''Valorizzo le celle del vettore
        'DtKeys(0) = Dt.Columns("abc")
        'DtKeys(1) = Dt.Columns("def")
        'DtKeys(2) = Dt.Columns("ghi")

        ''Assegno il vettore delle chiavi al DataTable
        'Dt.PrimaryKey = DtKeys

        Return Dt

    End Function



    '########################################################################################
    Public Sub DtForXml_InserisciRiga_BollaAccettazionedaDiversi_Impianti( _
                                ByRef DT As DataTable, _
                                    ByVal lotto_impianto As String, _
                                    ByVal sup As Decimal, _
                                    ByVal data_inizio As String, _
                                    ByVal codice_finalita As Integer, _
                                    ByVal codice_tipologia_varietale As Integer)


        Dim Dr As DataRow

        '----- Creo una nuova riga

        Dr = DT.NewRow

        '----- Definisco i valori

        With Dr

            .Item("lotto_impianto") = lotto_impianto
            .Item("sup") = CStr(sup)
            .Item("data_inizio") = data_inizio
            .Item("codice_finalita") = CStr(codice_finalita)
            .Item("codice_tipologia_varietale") = CStr(codice_tipologia_varietale)

        End With

        '----- Associo al datatable la nuova riga creata

        DT.Rows.Add(Dr)

    End Sub



    '##########################################################################################
    Public Function XML_BollaAccettazionedaDiversi_Impianti(ByRef XmlDoc As XmlDocument, _
                                                            ByVal Dr As DataRow) _
                                                            As XmlElement

        Dim XML_impianto As System.Xml.XmlElement

        Dim XML_lotto_impianto As System.Xml.XmlElement
        Dim XML_sup As System.Xml.XmlElement
        Dim XML_data_inizio As System.Xml.XmlElement
        Dim XML_codice_finalita As System.Xml.XmlElement
        Dim XML_codice_tipologia_varietale As System.Xml.XmlElement

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        XML_impianto = XmlDoc.CreateElement("impianto")

        XML_lotto_impianto = XmlDoc.CreateElement("lotto_impianto")
        XML_lotto_impianto.InnerText = Dr.Item("lotto_impianto")
        XML_impianto.AppendChild(XML_lotto_impianto)

        XML_sup = XmlDoc.CreateElement("sup")
        XML_sup.InnerText = CStr(Dr.Item("sup"))
        XML_impianto.AppendChild(XML_sup)

        XML_data_inizio = XmlDoc.CreateElement("data_inizio")
        XML_data_inizio.InnerText = Dr.Item("data_inizio")
        XML_impianto.AppendChild(XML_data_inizio)

        XML_codice_finalita = XmlDoc.CreateElement("codice_finalita")
        XML_codice_finalita.InnerText = CStr(Dr.Item("codice_finalita"))
        XML_impianto.AppendChild(XML_codice_finalita)

        XML_codice_tipologia_varietale = XmlDoc.CreateElement("codice_tipologia_varietale")
        If Dr.Item("codice_tipologia_varietale") <> 0 Then
            XML_codice_tipologia_varietale.InnerText = CStr(Dr.Item("codice_tipologia_varietale"))
        Else
            XML_codice_tipologia_varietale.InnerText = "#"
        End If
        XML_impianto.AppendChild(XML_codice_tipologia_varietale)


        'Restituisco in uscita 
        Return XML_impianto

        'Distruggo gli oggetti
        XML_impianto = Nothing



    End Function



    '##########################################################################################
    'Crea l'xml pubblico del documento (ddt, fattura)
    'utilizzato da Importazione_Magazzino_XMLPubblico (agronica sincronizzatore)
    Public Function XML_Documento_Testata(ByVal Flag_CreaRoot As Boolean,
                                            ByVal data_generazione As Date,
                                            ByVal piva_fornitore As String,
                                            ByVal piva_impresa As String,
                                            ByVal tipo_documento As String,
                                            ByVal prefisso_numero As String,
                                            ByVal numero As Integer,
                                            ByVal suffisso_numero As String,
                                            ByVal data As Date,
                                            ByVal num_reg_doc As Integer,
                                            ByVal data_reg_doc As Date,
                                            ByVal numero_colli As Integer,
                                            ByVal causale_trasporto As String,
                                            ByVal aspetto_beni As String,
                                            ByVal peso As Decimal,
                                            ByVal note As String,
                                            ByVal data_consegna As Date,
                                            ByVal data_partenza As Date,
                                            ByVal ora_partenza As String,
                                            ByVal scadenza As String,
                                            ByVal totale_doc As String,
                                            Optional ByRef XmlDoc As XmlDocument = Nothing,
                                            Optional ByVal codice_magazzino_cliente As String = "",
                                            Optional ByVal identificativo_cliente As String = "",
                                            Optional ByVal cuaa As String = "",
                                            Optional ByVal cod_impresa As String = "",
                                            Optional ByVal cod_fornitore As String = ""
                                            ) _
                                            As XmlElement

        'ByVal tipo_trasporto As Integer, _
        'ByVal tipo_operazione_vettore As Integer, _
        'ByVal vettore_piva As String, _
        'ByVal vettore_cod_fisc As String, _
        'ByVal vettore_rag_soc As String, _
        'ByVal vettore_indirizzo As String, _
        'ByVal vettore_frazione As String, _
        'ByVal vettore_comune_codistat As String, _
        'ByVal vettore_provincia_codistat As String, _
        'ByVal vettore_progressivo As String, _
        'ByVal vettore_attivita As String, _

        Dim XML_root As XmlElement = Nothing
        Dim XML_documento As XmlElement
        Dim XML_piva_fornitore, XML_identificativo_cliente, XML_piva_impresa, XML_prefisso_numero, XML_numero, XML_suffisso_numero, XML_data, XML_numero_colli, XML_causale_trasporto, XML_aspetto_beni, XML_peso, XML_note, XML_data_consegna, XML_data_partenza, XML_ora_partenza, XML_scadenza As XmlElement
        ' Dim XML_trasporto, XML_vettore, XML_vettore_piva, XML_vettore_cod_fisc, XML_vettore_rag_soc, XML_vettore_indirizzo, XML_vettore_frazione, XML_vettore_comune_codistat, XML_vettore_provincia_codistat, XML_vettore_progressivo, XML_vettore_attivita As XmlElement
        Dim XML_tipo_documento, XML_totale_doc, XML_num_reg_doc, XML_data_reg_doc, XML_dettagli As XmlElement
        Dim XML_cod_magazzino_cliente, XML_cod_fornitore, XML_cod_impresa, xml_cuaa As XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        If Flag_CreaRoot = True Then
            XML_root = XmlDoc.CreateElement("root")
            XmlDoc.AppendChild(XML_root)
            XML_root.SetAttribute(LCase("data_generazione"), CStr(data_generazione))
        End If

        XML_documento = XmlDoc.CreateElement("documento")

        XML_piva_fornitore = XmlDoc.CreateElement("piva_fornitore")
        XML_piva_fornitore.InnerText = CStr(piva_fornitore)
        XML_documento.AppendChild(XML_piva_fornitore)

        XML_cod_fornitore = XmlDoc.CreateElement("cod_fornitore")
        XML_cod_fornitore.InnerText = CStr(cod_fornitore)
        XML_documento.AppendChild(XML_cod_fornitore)

        XML_identificativo_cliente = XmlDoc.CreateElement("identificativo_cliente")
        XML_identificativo_cliente.InnerText = CStr(identificativo_cliente)
        XML_documento.AppendChild(XML_identificativo_cliente)

        XML_piva_impresa = XmlDoc.CreateElement("piva_impresa")
        XML_piva_impresa.InnerText = CStr(piva_impresa)
        XML_documento.AppendChild(XML_piva_impresa)

        xml_cuaa = XmlDoc.CreateElement("cuaa")
        xml_cuaa.InnerText = CStr(cuaa)
        XML_documento.AppendChild(xml_cuaa)

        XML_cod_impresa = XmlDoc.CreateElement("cod_impresa")
        XML_cod_impresa.InnerText = CStr(cod_impresa)
        XML_documento.AppendChild(XML_cod_impresa)

        XML_tipo_documento = XmlDoc.CreateElement("tipo_documento")
        XML_tipo_documento.InnerText = CStr(tipo_documento)
        XML_documento.AppendChild(XML_tipo_documento)

        XML_cod_magazzino_cliente = XmlDoc.CreateElement("codice_magazzino")
        XML_cod_magazzino_cliente.InnerText = CStr(codice_magazzino_cliente)
        XML_documento.AppendChild(XML_cod_magazzino_cliente)

        XML_prefisso_numero = XmlDoc.CreateElement("prefisso_numero")
        XML_prefisso_numero.InnerText = CStr(prefisso_numero)
        XML_documento.AppendChild(XML_prefisso_numero)

        XML_numero = XmlDoc.CreateElement("numero")
        XML_numero.InnerText = CStr(numero)
        XML_documento.AppendChild(XML_numero)

        XML_suffisso_numero = XmlDoc.CreateElement("suffisso_numero")
        XML_suffisso_numero.InnerText = CStr(suffisso_numero)
        XML_documento.AppendChild(XML_suffisso_numero)

        XML_data = XmlDoc.CreateElement("data")
        XML_data.InnerText = CStr(data)
        XML_documento.AppendChild(XML_data)

        XML_num_reg_doc = XmlDoc.CreateElement("num_reg_doc")
        XML_num_reg_doc.InnerText = CStr(num_reg_doc)
        XML_documento.AppendChild(XML_num_reg_doc)

        XML_data_reg_doc = XmlDoc.CreateElement("data_reg_doc")
        XML_data_reg_doc.InnerText = CStr(data_reg_doc)
        XML_documento.AppendChild(XML_data_reg_doc)

        XML_numero_colli = XmlDoc.CreateElement("numero_colli")
        XML_numero_colli.InnerText = CStr(numero_colli)
        XML_documento.AppendChild(XML_numero_colli)

        XML_causale_trasporto = XmlDoc.CreateElement("causale_trasporto")
        XML_causale_trasporto.InnerText = CStr(causale_trasporto)
        XML_documento.AppendChild(XML_causale_trasporto)

        XML_aspetto_beni = XmlDoc.CreateElement("aspetto_beni")
        XML_aspetto_beni.InnerText = CStr(aspetto_beni)
        XML_documento.AppendChild(XML_aspetto_beni)

        XML_peso = XmlDoc.CreateElement("peso")
        XML_peso.InnerText = CStr(peso)
        XML_documento.AppendChild(XML_peso)

        XML_note = XmlDoc.CreateElement("note")
        XML_note.InnerText = CStr(note)
        XML_documento.AppendChild(XML_note)

        XML_data_consegna = XmlDoc.CreateElement("data_consegna")
        XML_data_consegna.InnerText = CStr(data_consegna)
        XML_documento.AppendChild(XML_data_consegna)

        XML_data_partenza = XmlDoc.CreateElement("data_partenza")
        XML_data_partenza.InnerText = CStr(data_partenza)
        XML_documento.AppendChild(XML_data_partenza)

        XML_ora_partenza = XmlDoc.CreateElement("ora_partenza")
        XML_ora_partenza.InnerText = CStr(ora_partenza)
        XML_documento.AppendChild(XML_ora_partenza)

        XML_scadenza = XmlDoc.CreateElement("scadenza")
        XML_scadenza.InnerText = CStr(scadenza)
        XML_documento.AppendChild(XML_scadenza)

        XML_totale_doc = XmlDoc.CreateElement("totale_doc")
        XML_totale_doc.InnerText = CStr(totale_doc)
        XML_documento.AppendChild(XML_totale_doc)

        'XML_trasporto = XmlDoc.CreateElement("trasporto")
        'XML_trasporto.SetAttribute(LCase("tipo_trasporto"), CStr(tipo_trasporto))
        'XML_documento.AppendChild(XML_trasporto)

        ''------------------------------------------------
        ''VETTORE
        'If tipo_trasporto = 2 Then
        '    XML_vettore = XmlDoc.CreateElement("vettore")
        '    XML_vettore.SetAttribute(LCase("tipo_operazione"), CStr(tipo_operazione_vettore))
        '    XML_trasporto.AppendChild(XML_vettore)

        '    XML_vettore_piva = XmlDoc.CreateElement("vettore_piva")
        '    XML_vettore_piva.InnerText = CStr(vettore_piva)
        '    XML_vettore.AppendChild(XML_vettore_piva)

        '    XML_vettore_cod_fisc = XmlDoc.CreateElement("vettore_cod_fisc")
        '    XML_vettore_cod_fisc.InnerText = CStr(vettore_cod_fisc)
        '    XML_vettore.AppendChild(XML_vettore_cod_fisc)

        '    XML_vettore_rag_soc = XmlDoc.CreateElement("vettore_rag_soc")
        '    XML_vettore_rag_soc.InnerText = CStr(vettore_rag_soc)
        '    XML_vettore.AppendChild(XML_vettore_rag_soc)

        '    XML_vettore_indirizzo = XmlDoc.CreateElement("vettore_indirizzo")
        '    XML_vettore_indirizzo.InnerText = CStr(vettore_indirizzo)
        '    XML_vettore.AppendChild(XML_vettore_indirizzo)

        '    XML_vettore_frazione = XmlDoc.CreateElement("vettore_frazione")
        '    XML_vettore_frazione.InnerText = CStr(vettore_frazione)
        '    XML_vettore.AppendChild(XML_vettore_frazione)

        '    XML_vettore_comune_codistat = XmlDoc.CreateElement("vettore_comune_codistat")
        '    XML_vettore_comune_codistat.InnerText = CStr(vettore_comune_codistat)
        '    XML_vettore.AppendChild(XML_vettore_comune_codistat)

        '    XML_vettore_provincia_codistat = XmlDoc.CreateElement("vettore_provincia_codistat")
        '    XML_vettore_provincia_codistat.InnerText = CStr(vettore_provincia_codistat)
        '    XML_vettore.AppendChild(XML_vettore_provincia_codistat)

        '    XML_vettore_progressivo = XmlDoc.CreateElement("vettore_progressivo")
        '    XML_vettore_progressivo.InnerText = CStr(vettore_progressivo)
        '    XML_vettore.AppendChild(XML_vettore_progressivo)

        '    XML_vettore_attivita = XmlDoc.CreateElement("vettore_attivita")
        '    XML_vettore_attivita.InnerText = CStr(vettore_attivita)
        '    XML_vettore.AppendChild(XML_vettore_attivita)
        'End If
        ''------------------------------------------------

        'DETTAGLI

        XML_dettagli = XmlDoc.CreateElement("dettagli")
        XML_documento.AppendChild(XML_dettagli)

        'For i = 0 To DT_Dettagli.Rows.Count - 1

        '    Dim XML_prodotto, XML_categoria_magazzino, XML_codice_prodotto, XML_descrizione_prodotto, XML_codice_rg, XML_descrizione_rg, XML_lotto, XML_codice_specie_gias, XML_codice_varieta_gias, XML_bio, XML_numero_registrazione, XML_unita_misura, XML_qta As XmlElement


        '    XML_dettaglio = XML_BollaAccettazionedaDiversi_Dettagli(XmlDoc, DT_Dettagli.Rows(i))
        '    XML_dettagli.AppendChild(XML_dettaglio)

        'Next

        If Flag_CreaRoot = True Then
            XML_root.AppendChild(XML_documento)
            Return XML_root
        Else
            ' XmlDoc.AppendChild(XML_documento)
            Return XML_documento
        End If

    End Function

    '##########################################################################################
    'Crea l'xml pubblico dei dettagli del documento (ddt, fattura)
    'utilizzato da importatore di Coldiretti: Importazione_Magazzino_XMLPubblico (agronica sincronizzatore)
    Public Function XML_Documento_Dettagli(ByRef XmlDoc As XmlDocument,
                                            ByVal categoria_magazzino As String,
                                            ByVal codice_prodotto As String,
                                            ByVal descrizione_prodotto As String,
                                            ByVal lotto As String,
                                            ByVal bio As String,
                                            ByVal numero_registrazione As String,
                                            ByVal codice_specie_gias As String,
                                            ByVal codice_varieta_gias As String,
                                            ByVal unita_misura As String,
                                            ByVal qta As Decimal,
                                            ByVal prezzo_unitario As Decimal,
                                            ByVal sconto As Decimal,
                                            ByVal prezzo_unitario_netto As Decimal,
                                            ByVal imponibile_lordo As Decimal,
                                            ByVal imponibile_netto As Decimal,
                                            ByVal aliquota_iva As Decimal,
                                            ByVal iva As Decimal,
                                            ByVal prefisso_numero_ddt_rif As String,
                                            ByVal numero_ddt_rif As String,
                                            ByVal suffisso_numero_ddt_rif As String,
                                            ByVal data_ddt_rif As String,
                                            Optional ByVal codice_magazzino_cliente As String = "",
                                            Optional ByVal modalita_sconto As enModalitaSconto = enModalitaSconto.Percentuale,
                                            Optional ByVal cod_conto_eco As Integer = 0,
                                            Optional ByVal cod_conto_pat As Integer = 0) _
                                            As XmlElement

        Dim XML_prodotto, XML_categoria_magazzino, XML_codice_prodotto, XML_descrizione_prodotto, XML_lotto, XML_bio, XML_numero_registrazione, XML_unita_misura, XML_qta As XmlElement
        Dim XML_prezzo_unitario, XML_prezzo_unitario_netto, XML_imponibile_lordo, XML_imponibile_netto As XmlElement
        Dim XML_sconto, XML_modalita_sconto, XML_aliquota_iva, XML_iva, XML_prefisso_numero_ddt_rif, XML_numero_ddt_rif, XML_suffisso_numero_ddt_rif, XML_data_ddt_rif As XmlElement
        Dim XML_codice_specie_gias, XML_codice_varieta_gias As XmlElement
        Dim XML_cod_magazzino_cliente As XmlElement
        Dim XML_cod_conto_eco, XML_cod_conto_pat As XmlElement

        XML_prodotto = XmlDoc.CreateElement("prodotto")

        XML_cod_magazzino_cliente = XmlDoc.CreateElement("codice_magazzino")
        XML_cod_magazzino_cliente.InnerText = CStr(codice_magazzino_cliente)
        XML_prodotto.AppendChild(XML_cod_magazzino_cliente)

        XML_categoria_magazzino = XmlDoc.CreateElement("categoria_magazzino")
        XML_categoria_magazzino.InnerText = CStr(categoria_magazzino)
        XML_prodotto.AppendChild(XML_categoria_magazzino)

        XML_codice_prodotto = XmlDoc.CreateElement("codice_prodotto")
        XML_codice_prodotto.InnerText = CStr(codice_prodotto)
        XML_prodotto.AppendChild(XML_codice_prodotto)

        XML_descrizione_prodotto = XmlDoc.CreateElement("descrizione_prodotto")
        XML_descrizione_prodotto.InnerText = CStr(descrizione_prodotto)
        XML_prodotto.AppendChild(XML_descrizione_prodotto)

        XML_lotto = XmlDoc.CreateElement("lotto")
        XML_lotto.InnerText = CStr(lotto)
        XML_prodotto.AppendChild(XML_lotto)

        XML_bio = XmlDoc.CreateElement("bio")
        XML_bio.InnerText = CStr(bio)
        XML_prodotto.AppendChild(XML_bio)

        XML_numero_registrazione = XmlDoc.CreateElement("numero_registrazione")
        XML_numero_registrazione.InnerText = CStr(numero_registrazione)
        XML_prodotto.AppendChild(XML_numero_registrazione)

        XML_codice_specie_gias = XmlDoc.CreateElement("codice_specie_gias")
        XML_codice_specie_gias.InnerText = CStr(codice_specie_gias)
        XML_prodotto.AppendChild(XML_codice_specie_gias)

        XML_codice_varieta_gias = XmlDoc.CreateElement("codice_varieta_gias")
        XML_codice_varieta_gias.InnerText = CStr(codice_varieta_gias)
        XML_prodotto.AppendChild(XML_codice_varieta_gias)

        XML_unita_misura = XmlDoc.CreateElement("unita_misura")
        XML_unita_misura.InnerText = CStr(unita_misura)
        XML_prodotto.AppendChild(XML_unita_misura)

        XML_qta = XmlDoc.CreateElement("qta")
        XML_qta.InnerText = CStr(qta)
        XML_prodotto.AppendChild(XML_qta)

        XML_prezzo_unitario = XmlDoc.CreateElement("prezzo_unitario")
        XML_prezzo_unitario.InnerText = CStr(prezzo_unitario)
        XML_prodotto.AppendChild(XML_prezzo_unitario)

        XML_sconto = XmlDoc.CreateElement("sconto")
        XML_sconto.InnerText = CStr(sconto)
        XML_prodotto.AppendChild(XML_sconto)

        XML_modalita_sconto = XmlDoc.CreateElement("modalita_sconto")
        XML_modalita_sconto.InnerText = CStr(modalita_sconto)
        XML_prodotto.AppendChild(XML_modalita_sconto)

        XML_cod_conto_eco = XmlDoc.CreateElement("cod_conto_eco")
        XML_cod_conto_eco.InnerText = CStr(cod_conto_eco)
        XML_prodotto.AppendChild(XML_cod_conto_eco)

        XML_cod_conto_pat = XmlDoc.CreateElement("cod_conto_pat")
        XML_cod_conto_pat.InnerText = CStr(cod_conto_pat)
        XML_prodotto.AppendChild(XML_cod_conto_pat)

        XML_prezzo_unitario_netto = XmlDoc.CreateElement("prezzo_unitario_netto")
        XML_prezzo_unitario_netto.InnerText = CStr(prezzo_unitario_netto)
        XML_prodotto.AppendChild(XML_prezzo_unitario_netto)

        XML_imponibile_lordo = XmlDoc.CreateElement("imponibile_lordo")
        XML_imponibile_lordo.InnerText = CStr(imponibile_lordo)
        XML_prodotto.AppendChild(XML_imponibile_lordo)

        XML_imponibile_netto = XmlDoc.CreateElement("imponibile_netto")
        XML_imponibile_netto.InnerText = CStr(imponibile_netto)
        XML_prodotto.AppendChild(XML_imponibile_netto)

        XML_aliquota_iva = XmlDoc.CreateElement("aliquota_iva")
        XML_aliquota_iva.InnerText = CStr(aliquota_iva)
        XML_prodotto.AppendChild(XML_aliquota_iva)

        XML_iva = XmlDoc.CreateElement("iva")
        XML_iva.InnerText = CStr(iva)
        XML_prodotto.AppendChild(XML_iva)

        XML_prefisso_numero_ddt_rif = XmlDoc.CreateElement("prefisso_numero_ddt_rif")
        XML_prefisso_numero_ddt_rif.InnerText = CStr(prefisso_numero_ddt_rif)
        XML_prodotto.AppendChild(XML_prefisso_numero_ddt_rif)

        XML_numero_ddt_rif = XmlDoc.CreateElement("numero_ddt_rif")
        XML_numero_ddt_rif.InnerText = CStr(numero_ddt_rif)
        XML_prodotto.AppendChild(XML_numero_ddt_rif)

        XML_suffisso_numero_ddt_rif = XmlDoc.CreateElement("suffisso_numero_ddt_rif")
        XML_suffisso_numero_ddt_rif.InnerText = CStr(suffisso_numero_ddt_rif)
        XML_prodotto.AppendChild(XML_suffisso_numero_ddt_rif)

        XML_data_ddt_rif = XmlDoc.CreateElement("data_ddt_rif")
        XML_data_ddt_rif.InnerText = CStr(data_ddt_rif)
        XML_prodotto.AppendChild(XML_data_ddt_rif)

        Return XML_prodotto

    End Function


    '##########################################################################################
    'Crea l'xml pubblico del documento di trasporto DDT DETTAGLI
    Public Function XML_DocumentoTrasporto_Dettagli(ByRef XmlDoc As XmlDocument, _
                                                    ByVal categoria_magazzino As String, _
                                                    ByVal codice_prodotto As String, _
                                                    ByVal descrizione_prodotto As String, _
                                                    ByVal lotto As String, _
                                                    ByVal codice_specie_gias As String, _
                                                    ByVal codice_varieta_gias As String, _
                                                    ByVal bio As String, _
                                                    ByVal numero_registrazione As String, _
                                                    ByVal unita_misura As String, _
                                                    ByVal qta As Decimal, _
                                                    ByVal prezzo_unitario As Decimal, _
                                                    ByVal prezzo_unitario_netto As Decimal, _
                                                    ByVal imponibile_lordo As Decimal, _
                                                    ByVal imponibile_netto As Decimal) _
                                                    As XmlElement

        Dim XML_prodotto, XML_categoria_magazzino, XML_codice_prodotto, XML_descrizione_prodotto, XML_lotto, XML_codice_specie_gias, XML_codice_varieta_gias, XML_bio, XML_numero_registrazione, XML_unita_misura, XML_qta As XmlElement
        Dim XML_prezzo_unitario, XML_prezzo_unitario_netto, XML_imponibile_lordo, XML_imponibile_netto As XmlElement

        XML_prodotto = XmlDoc.CreateElement("prodotto")

        XML_categoria_magazzino = XmlDoc.CreateElement("categoria_magazzino")
        XML_categoria_magazzino.InnerText = CStr(categoria_magazzino)
        XML_prodotto.AppendChild(XML_categoria_magazzino)

        XML_codice_prodotto = XmlDoc.CreateElement("codice_prodotto")
        XML_codice_prodotto.InnerText = CStr(codice_prodotto)
        XML_prodotto.AppendChild(XML_codice_prodotto)

        XML_descrizione_prodotto = XmlDoc.CreateElement("descrizione_prodotto")
        XML_descrizione_prodotto.InnerText = CStr(descrizione_prodotto)
        XML_prodotto.AppendChild(XML_descrizione_prodotto)

        XML_lotto = XmlDoc.CreateElement("lotto")
        XML_lotto.InnerText = CStr(lotto)
        XML_prodotto.AppendChild(XML_lotto)

        XML_codice_specie_gias = XmlDoc.CreateElement("codice_specie_gias")
        XML_codice_specie_gias.InnerText = CStr(codice_specie_gias)
        XML_prodotto.AppendChild(XML_codice_specie_gias)

        XML_codice_varieta_gias = XmlDoc.CreateElement("codice_varieta_gias")
        XML_codice_varieta_gias.InnerText = CStr(codice_varieta_gias)
        XML_prodotto.AppendChild(XML_codice_varieta_gias)

        XML_bio = XmlDoc.CreateElement("bio")
        XML_bio.InnerText = CStr(bio)
        XML_prodotto.AppendChild(XML_bio)

        XML_numero_registrazione = XmlDoc.CreateElement("numero_registrazione")
        XML_numero_registrazione.InnerText = CStr(numero_registrazione)
        XML_prodotto.AppendChild(XML_numero_registrazione)

        XML_unita_misura = XmlDoc.CreateElement("unita_misura")
        XML_unita_misura.InnerText = CStr(unita_misura)
        XML_prodotto.AppendChild(XML_unita_misura)

        XML_qta = XmlDoc.CreateElement("qta")
        XML_qta.InnerText = CStr(qta)
        XML_prodotto.AppendChild(XML_qta)

        XML_prezzo_unitario = XmlDoc.CreateElement("prezzo_unitario")
        XML_prezzo_unitario.InnerText = CStr(prezzo_unitario)
        XML_prodotto.AppendChild(XML_prezzo_unitario)

        XML_prezzo_unitario_netto = XmlDoc.CreateElement("prezzo_unitario_netto")
        XML_prezzo_unitario_netto.InnerText = CStr(prezzo_unitario_netto)
        XML_prodotto.AppendChild(XML_prezzo_unitario_netto)

        XML_imponibile_lordo = XmlDoc.CreateElement("imponibile_lordo")
        XML_imponibile_lordo.InnerText = CStr(imponibile_lordo)
        XML_prodotto.AppendChild(XML_imponibile_lordo)

        XML_imponibile_netto = XmlDoc.CreateElement("imponibile_netto")
        XML_imponibile_netto.InnerText = CStr(imponibile_netto)
        XML_prodotto.AppendChild(XML_imponibile_netto)

        Return XML_prodotto

    End Function


    '##########################################################################################
    'Crea l'xml pubblico del documento di trasporto DDT
    'utilizzato da importatore di Agrisol: Importazione_DDTFatture_Seled (agronica sincronizzatore)
    'utilizzato da importatore di Terremerse: ImportatoreDDTPianoColturale (progetto vs2003 dedicato)
    Public Function XML_DocumentoTrasporto_Testata(ByVal Flag_CreaRoot As Boolean, _
                                                    ByVal data_generazione As Date, _
                                                    ByVal piva_fornitore As String, _
                                                    ByVal codice_fornitore As String, _
                                                    ByVal piva_impresa As String, _
                                                    ByVal prefisso_numero_ddt As String, _
                                                    ByVal numero_ddt As Integer, _
                                                    ByVal suffisso_numero_ddt As String, _
                                                    ByVal data_ddt As Date, _
                                                    ByVal num_reg_doc As Integer, _
                                                    ByVal data_reg_doc As Date, _
                                                    ByVal numero_colli As Integer, _
                                                    ByVal causale_trasporto As String, _
                                                    ByVal aspetto_beni As String, _
                                                    ByVal peso As Decimal, _
                                                    ByVal note_ddt As String, _
                                                    ByVal data_consegna As Date, _
                                                    ByVal data_partenza As Date, _
                                                    ByVal ora_partenza As String, _
                                                    ByVal tipo_trasporto As Integer, _
                                                    ByVal tipo_operazione_vettore As Integer, _
                                                    ByVal vettore_piva As String, _
                                                    ByVal vettore_cod_fisc As String, _
                                                    ByVal vettore_rag_soc As String, _
                                                    ByVal vettore_indirizzo As String, _
                                                    ByVal vettore_frazione As String, _
                                                    ByVal vettore_comune_codistat As String, _
                                                    ByVal vettore_provincia_codistat As String, _
                                                    ByVal vettore_progressivo As String, _
                                                    ByVal vettore_attivita As String, _
                                                    Optional ByRef XmlDoc As XmlDocument = Nothing _
                                                    ) _
                                                    As XmlElement

        Dim XML_root As XmlElement = Nothing
        Dim XML_ddt_ricevuto As XmlElement
        Dim XML_piva_fornitore, XML_piva_impresa, XML_prefisso_numero_ddt, XML_numero_ddt, XML_suffisso_numero_ddt, XML_data_ddt, XML_numero_colli, XML_causale_trasporto, XML_aspetto_beni, XML_peso, XML_note_ddt, XML_data_consegna, XML_data_partenza, XML_ora_partenza, XML_trasporto, XML_dettagli As XmlElement
        Dim XML_vettore, XML_vettore_piva, XML_vettore_cod_fisc, XML_vettore_rag_soc, XML_vettore_indirizzo, XML_vettore_frazione, XML_vettore_comune_codistat, XML_vettore_provincia_codistat, XML_vettore_progressivo, XML_vettore_attivita As XmlElement
        Dim XML_num_reg_doc, XML_data_reg_doc, XML_el As XmlElement
        'Dim XML_prodotto, XML_categoria_magazzino, XML_codice_prodotto, XML_descrizione_prodotto, XML_codice_rg, XML_descrizione_rg, XML_lotto, XML_codice_specie_gias, XML_codice_varieta_gias, XML_bio, XML_numero_registrazione, XML_unita_misura, XML_qta As XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        If Flag_CreaRoot = True Then
            XML_root = XmlDoc.CreateElement("root")
            XmlDoc.AppendChild(XML_root)
            XML_root.SetAttribute(LCase("data_generazione"), CStr(data_generazione))
        End If

        XML_ddt_ricevuto = XmlDoc.CreateElement("ddt_ricevuto")

        XML_piva_fornitore = XmlDoc.CreateElement("piva_fornitore")
        XML_piva_fornitore.InnerText = CStr(piva_fornitore)
        XML_ddt_ricevuto.AppendChild(XML_piva_fornitore)

        XML_el = XmlDoc.CreateElement("codice_fornitore")
        XML_el.InnerText = CStr(codice_fornitore)
        XML_ddt_ricevuto.AppendChild(XML_el)

        XML_piva_impresa = XmlDoc.CreateElement("piva_impresa")
        XML_piva_impresa.InnerText = CStr(piva_impresa)
        XML_ddt_ricevuto.AppendChild(XML_piva_impresa)

        XML_prefisso_numero_ddt = XmlDoc.CreateElement("prefisso_numero_ddt")
        XML_prefisso_numero_ddt.InnerText = CStr(prefisso_numero_ddt)
        XML_ddt_ricevuto.AppendChild(XML_prefisso_numero_ddt)

        XML_numero_ddt = XmlDoc.CreateElement("numero_ddt")
        XML_numero_ddt.InnerText = CStr(numero_ddt)
        XML_ddt_ricevuto.AppendChild(XML_numero_ddt)

        XML_suffisso_numero_ddt = XmlDoc.CreateElement("suffisso_numero_ddt")
        XML_suffisso_numero_ddt.InnerText = CStr(suffisso_numero_ddt)
        XML_ddt_ricevuto.AppendChild(XML_suffisso_numero_ddt)

        XML_data_ddt = XmlDoc.CreateElement("data_ddt")
        XML_data_ddt.InnerText = CStr(data_ddt)
        XML_ddt_ricevuto.AppendChild(XML_data_ddt)

        XML_num_reg_doc = XmlDoc.CreateElement("num_reg_doc")
        XML_num_reg_doc.InnerText = CStr(num_reg_doc)
        XML_ddt_ricevuto.AppendChild(XML_num_reg_doc)

        XML_data_reg_doc = XmlDoc.CreateElement("data_reg_doc")
        XML_data_reg_doc.InnerText = CStr(data_reg_doc)
        XML_ddt_ricevuto.AppendChild(XML_data_reg_doc)

        XML_numero_colli = XmlDoc.CreateElement("numero_colli")
        XML_numero_colli.InnerText = CStr(numero_colli)
        XML_ddt_ricevuto.AppendChild(XML_numero_colli)

        XML_causale_trasporto = XmlDoc.CreateElement("causale_trasporto")
        XML_causale_trasporto.InnerText = CStr(causale_trasporto)
        XML_ddt_ricevuto.AppendChild(XML_causale_trasporto)

        XML_aspetto_beni = XmlDoc.CreateElement("aspetto_beni")
        XML_aspetto_beni.InnerText = CStr(aspetto_beni)
        XML_ddt_ricevuto.AppendChild(XML_aspetto_beni)

        XML_peso = XmlDoc.CreateElement("peso")
        XML_peso.InnerText = CStr(peso)
        XML_ddt_ricevuto.AppendChild(XML_peso)

        XML_note_ddt = XmlDoc.CreateElement("note_ddt")
        XML_note_ddt.InnerText = CStr(note_ddt)
        XML_ddt_ricevuto.AppendChild(XML_note_ddt)

        XML_data_consegna = XmlDoc.CreateElement("data_consegna")
        XML_data_consegna.InnerText = CStr(data_consegna)
        XML_ddt_ricevuto.AppendChild(XML_data_consegna)

        XML_data_partenza = XmlDoc.CreateElement("data_partenza")
        XML_data_partenza.InnerText = CStr(data_partenza)
        XML_ddt_ricevuto.AppendChild(XML_data_partenza)

        XML_ora_partenza = XmlDoc.CreateElement("ora_partenza")
        XML_ora_partenza.InnerText = CStr(ora_partenza)
        XML_ddt_ricevuto.AppendChild(XML_ora_partenza)

        XML_trasporto = XmlDoc.CreateElement("trasporto")
        XML_trasporto.SetAttribute(LCase("tipo_trasporto"), CStr(tipo_trasporto))
        XML_ddt_ricevuto.AppendChild(XML_trasporto)

        '------------------------------------------------
        'VETTORE
        If tipo_trasporto = 2 Then
            XML_vettore = XmlDoc.CreateElement("vettore")
            XML_vettore.SetAttribute(LCase("tipo_operazione"), CStr(tipo_operazione_vettore))
            XML_trasporto.AppendChild(XML_vettore)

            XML_vettore_piva = XmlDoc.CreateElement("vettore_piva")
            XML_vettore_piva.InnerText = CStr(vettore_piva)
            XML_vettore.AppendChild(XML_vettore_piva)

            XML_vettore_cod_fisc = XmlDoc.CreateElement("vettore_cod_fisc")
            XML_vettore_cod_fisc.InnerText = CStr(vettore_cod_fisc)
            XML_vettore.AppendChild(XML_vettore_cod_fisc)

            XML_vettore_rag_soc = XmlDoc.CreateElement("vettore_rag_soc")
            XML_vettore_rag_soc.InnerText = CStr(vettore_rag_soc)
            XML_vettore.AppendChild(XML_vettore_rag_soc)

            XML_vettore_indirizzo = XmlDoc.CreateElement("vettore_indirizzo")
            XML_vettore_indirizzo.InnerText = CStr(vettore_indirizzo)
            XML_vettore.AppendChild(XML_vettore_indirizzo)

            XML_vettore_frazione = XmlDoc.CreateElement("vettore_frazione")
            XML_vettore_frazione.InnerText = CStr(vettore_frazione)
            XML_vettore.AppendChild(XML_vettore_frazione)

            XML_vettore_comune_codistat = XmlDoc.CreateElement("vettore_comune_codistat")
            XML_vettore_comune_codistat.InnerText = CStr(vettore_comune_codistat)
            XML_vettore.AppendChild(XML_vettore_comune_codistat)

            XML_vettore_provincia_codistat = XmlDoc.CreateElement("vettore_provincia_codistat")
            XML_vettore_provincia_codistat.InnerText = CStr(vettore_provincia_codistat)
            XML_vettore.AppendChild(XML_vettore_provincia_codistat)

            XML_vettore_progressivo = XmlDoc.CreateElement("vettore_progressivo")
            XML_vettore_progressivo.InnerText = CStr(vettore_progressivo)
            XML_vettore.AppendChild(XML_vettore_progressivo)

            XML_vettore_attivita = XmlDoc.CreateElement("vettore_attivita")
            XML_vettore_attivita.InnerText = CStr(vettore_attivita)
            XML_vettore.AppendChild(XML_vettore_attivita)
        End If
        '------------------------------------------------

        'DETTAGLI

        XML_dettagli = XmlDoc.CreateElement("dettagli")
        XML_ddt_ricevuto.AppendChild(XML_dettagli)

        'For i = 0 To DT_Dettagli.Rows.Count - 1

        '    Dim XML_prodotto, XML_categoria_magazzino, XML_codice_prodotto, XML_descrizione_prodotto, XML_codice_rg, XML_descrizione_rg, XML_lotto, XML_codice_specie_gias, XML_codice_varieta_gias, XML_bio, XML_numero_registrazione, XML_unita_misura, XML_qta As XmlElement


        '    XML_dettaglio = XML_BollaAccettazionedaDiversi_Dettagli(XmlDoc, DT_Dettagli.Rows(i))
        '    XML_dettagli.AppendChild(XML_dettaglio)

        'Next

        If Flag_CreaRoot = True Then
            XML_root.AppendChild(XML_ddt_ricevuto)
            Return XML_root
        Else
            XmlDoc.AppendChild(XML_ddt_ricevuto)
            Return XML_ddt_ricevuto
        End If

    End Function



End Class
