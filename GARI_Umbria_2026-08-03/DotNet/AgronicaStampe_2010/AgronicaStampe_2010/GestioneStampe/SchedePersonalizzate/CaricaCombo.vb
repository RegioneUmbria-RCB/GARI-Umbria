Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
Imports AgronicaCoreMetaSchemaDAL.GruppoColturaleXSpecieVegetali_R
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Module CaricaCombo
    '###############################################################################
    Public Sub CaricaCheckBoxList_RapportiContabili(objparametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                    objparametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                 ByRef Cbl As System.Web.UI.WebControls.CheckBoxList,
                                                 ByVal Sa_Cod As Integer,
                                                    Optional ByVal Filtro_SaCod As Boolean = True,
                                                    Optional ByVal Cod_Rapporto As Integer = 0,
                                                    Optional ByVal Cliente As Boolean = False,
                                                    Optional ByVal Fornitore As Boolean = False,
                                                    Optional ByVal Dipendente As Boolean = False,
                                                    Optional ByVal Terzista As Boolean = False,
                                                    Optional ByVal Legale As Boolean = False,
                                                    Optional ByVal FiltroAggiuntivo As String = "")






        Dim i As Integer
        Dim DT_RappCont As DataTable
        Dim FiltroFinale As String

        Dim rp As New AgronicaCoreAnagrafeDAL.Rapporti_Contabili_R

        'se voglio applicare solo il filtro sa_cod
        If Filtro_SaCod = True Then
            FiltroFinale = "AND (Sa_Cod = 0 OR Sa_Cod = " & CStr(Sa_Cod) & ")"
        Else
            'altrimento applico l'eventuale filtro passato
            FiltroFinale = FiltroAggiuntivo
        End If


        DT_RappCont = rp.Contatti_RapportiContabili_Leggi(
                                                    0,
                                                    Cod_Rapporto,
                                                    Cliente,
                                                    Fornitore,
                                                    Dipendente,
                                                    Terzista,
                                                    Legale,
                                                    False,
                                                    False,
                                                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                    FiltroFinale,
                                                    "",
                                                    objparametri_server)


        Cbl.Items.Clear()

        If DT_RappCont.Rows.Count <> 0 Then



            For i = 0 To DT_RappCont.Rows.Count - 1

                Cbl.Items.Add(New ListItem(DT_RappCont.Rows(i).Item("Rapporto_Des"),
                                        DT_RappCont.Rows(i).Item("Cod_Rapporto")))


            Next

        End If



    End Sub

    '###############################################################################
    'udare   AgronicaCoreUtility.CaricaListControl.Regioni
    Public Sub ___CaricaCombo_Regioni_OLD(ByRef objServer As System.Web.HttpServerUtility,
                                    ByRef objSession As System.Web.SessionState.HttpSessionState,
                                    ByRef objPage As System.Web.UI.Page,
                                    ByRef Cmb As System.Web.UI.WebControls.DropDownList,
                                    Optional ByVal Ordinamento_Alfabetico As Boolean = False)

        'Pulisco la combo
        Cmb.Items.Clear()

        Select Case Ordinamento_Alfabetico

            Case False

                Cmb.Items.Add(New ListItem(" ", ""))
                Cmb.Items.Add(New ListItem("Piemonte", "001"))
                Cmb.Items.Add(New ListItem("Valle d'Aosta", "002"))
                Cmb.Items.Add(New ListItem("Lombardia", "003"))
                Cmb.Items.Add(New ListItem("Trentino Alto Adige", "004"))
                Cmb.Items.Add(New ListItem("Veneto", "005"))
                Cmb.Items.Add(New ListItem("Friuli Venezia Giulia", "006"))
                Cmb.Items.Add(New ListItem("Liguria", "007"))
                Cmb.Items.Add(New ListItem("Emilia Romagna", "008"))
                Cmb.Items.Add(New ListItem("Toscana", "009"))
                Cmb.Items.Add(New ListItem("Umbria", "010"))
                Cmb.Items.Add(New ListItem("Marche", "011"))
                Cmb.Items.Add(New ListItem("Lazio", "012"))
                Cmb.Items.Add(New ListItem("Abruzzo", "013"))
                Cmb.Items.Add(New ListItem("Molise", "014"))
                Cmb.Items.Add(New ListItem("Campania", "015"))
                Cmb.Items.Add(New ListItem("Puglia", "016"))
                Cmb.Items.Add(New ListItem("Basilicata", "017"))
                Cmb.Items.Add(New ListItem("Calabria", "018"))
                Cmb.Items.Add(New ListItem("Sicilia", "019"))
                Cmb.Items.Add(New ListItem("Sardegna", "020"))

            Case True

                Cmb.Items.Add(New ListItem(" ", ""))
                Cmb.Items.Add(New ListItem("Abruzzo", "013"))
                Cmb.Items.Add(New ListItem("Basilicata", "017"))
                Cmb.Items.Add(New ListItem("Calabria", "018"))
                Cmb.Items.Add(New ListItem("Campania", "015"))
                Cmb.Items.Add(New ListItem("Emilia Romagna", "008"))
                Cmb.Items.Add(New ListItem("Friuli Venezia Giulia", "006"))
                Cmb.Items.Add(New ListItem("Lazio", "012"))
                Cmb.Items.Add(New ListItem("Liguria", "007"))
                Cmb.Items.Add(New ListItem("Lombardia", "003"))
                Cmb.Items.Add(New ListItem("Marche", "011"))
                Cmb.Items.Add(New ListItem("Molise", "014"))
                Cmb.Items.Add(New ListItem("Piemonte", "001"))
                Cmb.Items.Add(New ListItem("Puglia", "016"))
                Cmb.Items.Add(New ListItem("Sardegna", "020"))
                Cmb.Items.Add(New ListItem("Sicilia", "019"))
                Cmb.Items.Add(New ListItem("Toscana", "009"))
                Cmb.Items.Add(New ListItem("Trentino Alto Adige", "004"))
                Cmb.Items.Add(New ListItem("Umbria", "010"))
                Cmb.Items.Add(New ListItem("Valle d'Aosta", "002"))
                Cmb.Items.Add(New ListItem("Veneto", "005"))

        End Select


    End Sub

    '###############################################################################
    Public Sub CaricaCombo_TipoAttivita(ByRef Cmb As System.Web.UI.WebControls.DropDownList)

        'Pulisco la combo
        Cmb.Items.Clear()

        Cmb.Items.Add(New ListItem(" ", " "))
        Cmb.Items.Add(New ListItem("Produzione vegetale", "PV"))
        Cmb.Items.Add(New ListItem("Produzione zootecnica", "PZ"))
        Cmb.Items.Add(New ListItem("Produzione vegetale e zootecnica", "PVZ"))
        Cmb.Items.Add(New ListItem("Preparazione vegetale", "TPV"))
        Cmb.Items.Add(New ListItem("Preparazione zootecnica", "TPZ"))
        Cmb.Items.Add(New ListItem("Preparazione vegetale e zootecnica", "TPVZ"))
        Cmb.Items.Add(New ListItem("Importazione", "I"))
        Cmb.Items.Add(New ListItem("Raccolta spontanea", "RS"))
        Cmb.Items.Add(New ListItem("Produzione / Preparazione", "P/TP"))
        Cmb.Items.Add(New ListItem("Preparazione / Importazione", "TP/I"))
        Cmb.Items.Add(New ListItem("Altro", "@"))

    End Sub

    '###############################################################################
    Public Sub CaricaCombo_TitoloPossesso(ByRef Cmb As System.Web.UI.WebControls.DropDownList)

        'Pulisco la combo
        Cmb.Items.Clear()

        'Cmb.Items.Add(New ListItem(" ", " "))
        Cmb.Items.Add(New ListItem("Altro", "0"))
        Cmb.Items.Add(New ListItem("Proprietà", "1"))
        Cmb.Items.Add(New ListItem("Comodato d'uso", "2"))
        Cmb.Items.Add(New ListItem("Affitto con contratto", "3"))
        Cmb.Items.Add(New ListItem("Affitto senza contratto", "4"))
        Cmb.Items.Add(New ListItem("In conto terzi (M004)", "5"))

    End Sub

    '###############################################################################
    Public Sub CaricaCombo_OTE(ByRef Cmb As System.Web.UI.WebControls.DropDownList)

        'Pulisco la combo
        Cmb.Items.Clear()

        Cmb.Items.Add(New ListItem(" ", " "))
        Cmb.Items.Add(New ListItem("AGRICOLTURA GENERALE", "1"))
        Cmb.Items.Add(New ListItem("CEREALI", "11"))
        Cmb.Items.Add(New ListItem("CEREALI, SENZA RISO", "111"))
        Cmb.Items.Add(New ListItem("RISO", "112"))
        Cmb.Items.Add(New ListItem("CEREALI, RISO COMPRESO", "113"))
        Cmb.Items.Add(New ListItem("ALTRE COLTIVAZIONI", "12"))
        Cmb.Items.Add(New ListItem("PIANTE SARCHIATE", "121"))
        Cmb.Items.Add(New ListItem("CEREALI E PIANTE SARCHIATE", "122"))
        Cmb.Items.Add(New ListItem("ORTI IN PIENO CAMPO", "1231"))
        Cmb.Items.Add(New ListItem("COLTIVAZIONI DIVERSE", "1232"))
        Cmb.Items.Add(New ListItem("ORTOFLORICOLTURA", "2"))
        Cmb.Items.Add(New ListItem("ORTOFLORICOLTURA", "21"))
        Cmb.Items.Add(New ListItem("ORTI IN PIENA AREA", "211"))
        Cmb.Items.Add(New ListItem("ORTI SOTTO VETRO", "212"))
        Cmb.Items.Add(New ListItem("ORTI PIENA AREA / SOTTO VETRO", "213"))
        Cmb.Items.Add(New ListItem("FIORI PIENA AREA", "214"))
        Cmb.Items.Add(New ListItem("FIORI SOTTO VETRO", "215"))
        Cmb.Items.Add(New ListItem("FIORI PIENA AREA / SOTTO VETRO", "216"))
        Cmb.Items.Add(New ListItem("ORTI E FIORI PIENA AREA", "2171"))
        Cmb.Items.Add(New ListItem("ORTI E FIORI SOTTO VETRO", "2172"))
        Cmb.Items.Add(New ListItem("ORTI E FIORI", "2173"))
        Cmb.Items.Add(New ListItem("COLTIVAZIONI PERMANENTI", "3"))
        Cmb.Items.Add(New ListItem("VITE", "31"))
        Cmb.Items.Add(New ListItem("VITE DA VINO", "310"))
        Cmb.Items.Add(New ListItem("VITE DA TAVOLA", "313"))
        Cmb.Items.Add(New ListItem("VITE MISTA", "314"))
        Cmb.Items.Add(New ListItem("FRUTTA E ALTRE COLTIVAZIONI", "32"))
        Cmb.Items.Add(New ListItem("FRUTTA", "321"))
        Cmb.Items.Add(New ListItem("AGRUMI", "322"))
        Cmb.Items.Add(New ListItem("OLIVE", "323"))
        Cmb.Items.Add(New ListItem("COLTIVAZIONI PERMANENTI DIVERSE", "324"))
        Cmb.Items.Add(New ListItem("ERBIVORI", "4"))
        Cmb.Items.Add(New ListItem("BOVINI LATTE", "41"))
        Cmb.Items.Add(New ListItem("LATTE SPECIALIZZATO", "411"))
        Cmb.Items.Add(New ListItem("LATTE / ALLEVAMENTO", "412"))
        Cmb.Items.Add(New ListItem("BOVINI CARNE", "42"))
        Cmb.Items.Add(New ListItem("BOVINI CARNE", "421"))
        Cmb.Items.Add(New ListItem("BOVINI CARNE / ALLEVAMENTO", "422"))
        Cmb.Items.Add(New ListItem("BOVINI MISTI", "43"))
        Cmb.Items.Add(New ListItem("BOVINI LATTE / CARNE", "431"))
        Cmb.Items.Add(New ListItem("ALTRI ERBIVORI", "44"))
        Cmb.Items.Add(New ListItem("OVINI", "441"))
        Cmb.Items.Add(New ListItem("BOVINI E OVINI", "442"))
        Cmb.Items.Add(New ListItem("ERBIVORI DIVERSI", "443"))
        Cmb.Items.Add(New ListItem("GRANIVORI", "5"))
        Cmb.Items.Add(New ListItem("SUINI", "51"))
        Cmb.Items.Add(New ListItem("SUINI ALLEVAMENTO", "511"))
        Cmb.Items.Add(New ListItem("SUINI INGRASSO", "512"))
        Cmb.Items.Add(New ListItem("SUINI MISTI", "513"))
        Cmb.Items.Add(New ListItem("ALTRI GRANIVORI", "52"))
        Cmb.Items.Add(New ListItem("GALLINE OVAIOLE", "521"))
        Cmb.Items.Add(New ListItem("VOLATILI DA CARNE", "522"))
        Cmb.Items.Add(New ListItem("SUINI E VOLATILI", "523"))
        Cmb.Items.Add(New ListItem("GRANIVORO MISTO", "524"))
        Cmb.Items.Add(New ListItem("POLICOLTURA", "6"))
        Cmb.Items.Add(New ListItem("ORTOFLORICOLTURA E COLTIVAZIONI PERMANENTI", "61"))
        Cmb.Items.Add(New ListItem("ORTOFLORICOLTURA E COLTIVAZIONI PERMANENTI", "611"))
        Cmb.Items.Add(New ListItem("ALTRE POLICOLTURE", "62"))
        Cmb.Items.Add(New ListItem("AGRICOLTURA GENERALE E ORTOFLORICOLTURA", "621"))
        Cmb.Items.Add(New ListItem("AGRICOLTURA GENERALE E VITICOLTURA", "622"))
        Cmb.Items.Add(New ListItem("AGRICOLTURA GENERALE E FRUTTICOLTURA", "623"))
        Cmb.Items.Add(New ListItem("AGRICOLTURA GEN. PARZIALMENTE DOMINANTE", "624"))
        Cmb.Items.Add(New ListItem("ORTOFLORICOLTURA PARZIALMENTE DOMINANTE", "6251"))
        Cmb.Items.Add(New ListItem("ARBOREO PARZIALMENTE DOMINANTE", "6252"))
        Cmb.Items.Add(New ListItem("POLIALLEVAMENTO", "7"))
        Cmb.Items.Add(New ListItem("ERBIVORI PARZIALMENTE DOMINANTE", "71"))
        Cmb.Items.Add(New ListItem("LATTE PARZIALMENTE DOMINANTE", "711"))
        Cmb.Items.Add(New ListItem("ERBIVORI NON LATTIFERI PERZIAL. DOMINANTE", "712"))
        Cmb.Items.Add(New ListItem("ALTRI POLIALLEVAMENTI", "72"))
        Cmb.Items.Add(New ListItem("COLTURE E ALLEVAMENTI", "8"))
        Cmb.Items.Add(New ListItem("AGRICOLTURA GENERALE E ERBIVORI", "81"))
        Cmb.Items.Add(New ListItem("AGRICOLTURA GENERALE E LATTE", "810"))
        Cmb.Items.Add(New ListItem("AGRICOLTURA GEN. E ERBIVORI NON LATTIFERI", "8100"))
        Cmb.Items.Add(New ListItem("ALTRE COLTIVAZIONI E ALLEVAMENTI", "82"))
        Cmb.Items.Add(New ListItem("AGRICOLTURA GENERALE E GRANIVORI", "821"))
        Cmb.Items.Add(New ListItem("COLTIVAZIONI E ALLEVAMENTI DIVERSI", "822"))

    End Sub

    '###############################################################################
    'Questa CaricaCombo è simile a CaricaCombo_SpecieVegetale_Semente e a CaricaCheckBoxList_SpecieVegetale_Optimize
    'se si modifica il meccanismo in una, verificare anche le altre
    '------------------------------------------------------------------------------
    'Se Flag_FiltroUtente = true, Filtra le Specie a seconda  
    'del FILTRO impostato sull'utente in Utenti_Impostazioni_FiltroMono.
    '
    'Il Veg_Cod_daModificare è il veg_cod dell'impianto (o del prodotto, o ecc)
    'che bisogna caricare nella combo, anche se non è incluso nel filtro specie vegetali associato all'utente
    '(altrimenti si genera un errore)
    Public Sub CaricaCombo_SpecieVegetale_Optimize(ByRef objparametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                    ByRef objparametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                    ByRef Cmb As System.Web.UI.WebControls.DropDownList,
                                                    ByVal Gru_Cod As Integer,
                                                    Optional ByVal LetteraIniziale As String = "",
                                                    Optional ByVal Flag_FiltroUtente As Boolean = False,
                                                    Optional ByVal Veg_Cod As Integer = 0,
                                                    Optional ByVal Flag_PrimaRiga As Boolean = True,
                                                    Optional ByVal Testo_PrimaRiga As String = "",
                                                    Optional ByVal Cod_PrimaRiga As String = "",
                                                    Optional ByVal Cerca_VegDes As String = "",
                                                    Optional ByVal FinestraTemp_Inizio As String = "01/01/1900",
                                                    Optional ByVal FinestraTemp_Fine As String = "31/12/2100",
                                                    Optional ByVal FiltroAggiuntivo As String = "",
                                                    Optional ByVal Ordinamento As String = "",
                                                    Optional ByVal Veg_Cod_daModificare As Integer = 0,
                                                    Optional ByVal GruCod_Rif_VegCod_daModificare As Integer = 0)

        Dim Dt As DataTable
        Dim i As Integer
        Dim x_Cod As Integer
        Dim x_Des As String
        Dim NumTotale As Integer
        'Dim Impostazione_Valore_1 As String
        'Dim Flag_FiltroUtenteImpostato As Boolean = False
        Dim Flag_VegCodTrovatoNelFiltroUtente As Boolean = False


        Cmb.Items.Clear()

        If Flag_PrimaRiga = True Then
            Cmb.Items.Add(New ListItem(Testo_PrimaRiga, Cod_PrimaRiga))
        End If

        Dim objMetaschemaSpecie As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R

        If Flag_FiltroUtente = True Then

            'VECCHIA GESTIONE
            ''Attenzione! Poichè il filtro sulla specie è un filtro utente,
            ''va passata come username, la username dell'utente, non quella del superuser
            'Impostazione_Valore_1 = ImpostazioneValore1_from_ImpostazioneCod(objServer, objSession, objPage, _
            '                                                                CStr(objSession("ASG_SuperUser_CodFiscale")), _
            '                                                                CStr(objSession("ASG_Utente_Username")), _
            '                                                                COD_FILTRO_SPECIE_VEGETALI)

            'If Impostazione_Valore_1 <> "" Then
            '    Flag_FiltroUtenteImpostato = True
            '    FiltroAggiuntivo += " AND ( Veg_Cod IN (" & Impostazione_Valore_1 & ") ) "
            'End If

            Dt = objMetaschemaSpecie.SpecieVegetali_GestioneFiltroUtente_Leggi(0,
                                                           Gru_Cod,
                                                           "",
                                                           "",
                                                           FiltroAggiuntivo,
                                                           Ordinamento,
                                                           objparametri_Utenti)


            Dt = objMetaschemaSpecie.Leggi(0,
                                            Gru_Cod,
                                            "",
                                            "",
                                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                                            FiltroAggiuntivo,
                                            Ordinamento,
                                            objparametri_Server)

        Else

            Dt = objMetaschemaSpecie.Leggi(0,
                                            Gru_Cod,
                                            "",
                                            "",
                                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                                            FiltroAggiuntivo,
                                            Ordinamento,
                                            objparametri_Server)


        End If


        If Not IsNothing(Dt) Then

            NumTotale = Dt.Rows.Count

            'uso il dataview per ordinare
            Dim Dv As New DataView

            Dt.TableName = "specie"
            Dv.Table = Dt
            Dv.Sort = "Veg_Des ASC"

            If Not IsNothing(Dv) Then

                For i = 0 To NumTotale - 1

                    'se sono in info o modifica, e quindi ho passato il Veg_Cod_daModificare
                    'faccio il controllo
                    If Veg_Cod_daModificare <> 0 Then

                        ''se l'utente ha il filtro specie impostato
                        'If Flag_FiltroUtenteImpostato = True Then

                        'se all'interno delle specie filtrate, trovo il veg_cod da modificare
                        If Dv.Item(i).Item("Veg_Cod") = Veg_Cod_daModificare Then
                            Flag_VegCodTrovatoNelFiltroUtente = True
                        End If
                        'Else
                        '    'non c'è un filtro impostato
                        '    'quindi imposto il flag, come se avessi trovato la specie
                        '    'così non devo aggiungere niente
                        '    Flag_VegCodTrovatoNelFiltroUtente = True
                        'End If
                    Else
                        'non è stato passato il veg_cod
                        'quindi imposto il flag, come se avessi trovato la specie
                        'così non devo aggiungere niente
                        Flag_VegCodTrovatoNelFiltroUtente = True
                    End If

                    x_Cod = Dv.Item(i).Item("Veg_Cod")

                    x_Des = CStr(Dv.Item(i).Item("Veg_DES"))

                    If LetteraIniziale = "" Then
                        Cmb.Items.Add(New ListItem(x_Des, x_Cod))
                    Else
                        If UCase(Mid(x_Des, 1, 1)) = UCase(LetteraIniziale) Then
                            Cmb.Items.Add(New ListItem(x_Des, x_Cod))
                        End If
                    End If

                Next

            End If

            Dv = Nothing

        Else
            NumTotale = 0
        End If

        Dt = Nothing


        'se sono in info o modifica, e quindi ho passato il Veg_Cod_daModificare
        If Veg_Cod_daModificare <> 0 Then

            ''se l'utente ha il filtro specie impostato
            'If Flag_FiltroUtenteImpostato = True Then

            'se all'interno delle specie filtrate, non è stato trovato il veg_cod da modificare
            If Flag_VegCodTrovatoNelFiltroUtente = False Then

                x_Des = ""

                'se il Veg_Cod_daModificare è valorizzato, devo sapere anche il gru_cod
                '(se non l'ho passato me lo ricavo e intanto che ci sono ricavo la descrizione della specie)
                'però solo nel caso di gru_cod è <> 0 (ovvero impianto e campo)
                If Gru_Cod <> 0 And GruCod_Rif_VegCod_daModificare = 0 Then
                    GruCod_Rif_VegCod_daModificare = objMetaschemaSpecie.GruCod_and_VegDes_from_VegCod(x_Des, Veg_Cod_daModificare, objparametri_Server)
                End If

                'se il gru_cod delal specie da modificare è lo stesso di quello selezionato,
                'allora inserisco la specie vegetale da modificare nel menù
                If Gru_Cod = GruCod_Rif_VegCod_daModificare Then

                    'se la descrizione non l'ho già ricavata sopra
                    If x_Des = "" Then
                        x_Des = objMetaschemaSpecie.VegDes_from_VegCod(Veg_Cod_daModificare, objparametri_Server)
                    End If

                    Cmb.Items.Add(New ListItem(x_Des, Veg_Cod_daModificare))

                End If

            End If

            'End If

        End If


    End Sub

    '###############################################################################
    Public Sub CaricaCombo_TipologiaAnalisi(ByRef Cmb As System.Web.UI.WebControls.DropDownList)

        'Pulisco la combo
        Cmb.Items.Clear()

        Cmb.Items.Add(New ListItem("", "0"))
        Cmb.Items.Add(New ListItem("Analisi Terreno", "1"))
        Cmb.Items.Add(New ListItem("Analisi Terreno per Fanghi", "2"))
        Cmb.Items.Add(New ListItem("Analisi Acque", "3"))
        'Cmb.Items.Add(New ListItem("Analisi Residui Fitofarmaci su Vegetali", "4"))
        Cmb.Items.Add(New ListItem("Analisi Latte", "5"))
        Cmb.Items.Add(New ListItem("Analisi del Vino", "6"))
        Cmb.Items.Add(New ListItem("Analisi dei Residui", "7"))


    End Sub



    '###############################################################################
    'a differenza della precedente può restituire nella stringa del nome del fertilizzante
    'anche il dettaglio N-P-K-M
    Public Sub CaricaCheckBoxList_Fertilizzanti_2(ByRef objServer As System.Web.HttpServerUtility,
                                                  ByRef objSession As System.Web.SessionState.HttpSessionState,
                                                  ByRef objPage As System.Web.UI.Page,
                                                  ByRef Cbl As System.Web.UI.WebControls.CheckBoxList,
                                                  ByVal TestoRicerca As String,
                                                  Optional ByVal TipoRichiesto As Integer = 0,
                                                  Optional ByVal Visualizza_NPKM As Boolean = False)

        'NOTA
        'Il TipoRichiesto consente di selezionare solo i fertilizzanti specifici
        'per la particolare applicazione
        '
        '   0 = Tutti i fertilizzanti
        '   1 = Trattamenti Antibutteratura
        '   2 = Concimazione Fogliare
        '   3 = Fertirrigazione
        '   4 = Concimazione Organica
        '   5 = Concimazione in pieno Campo
        '   ecc...

        Dim objSQL As New Codex_Utility.Sql
        Dim Rs As ADODB.Recordset
        Dim StrSQL As String
        Dim Messaggio As String

        'Pulisco la combo
        Cbl.Items.Clear()

        Select Case TipoRichiesto

            Case 0  'Tutti i fertilizzanti

                StrSQL = ""
                StrSQL = StrSQL & " SELECT  distinct "
                StrSQL = StrSQL & "         Fertilizzanti.FER_COD, "
                StrSQL = StrSQL & "         Fertilizzanti.FER_DES, "
                StrSQL = StrSQL & "         Fertilizzanti.N, "
                StrSQL = StrSQL & "         Fertilizzanti.P2O5, "
                StrSQL = StrSQL & "         Fertilizzanti.K2O, "
                StrSQL = StrSQL & "         Fertilizzanti.MgO "
                StrSQL = StrSQL & " FROM    "
                StrSQL = StrSQL & "         Fertilizzanti "
                StrSQL = StrSQL & " WHERE   (FER_DES LIKE '%" & TestoRicerca & "%') "
                StrSQL = StrSQL & " ORDER BY FER_DES "
                '=================================================================


            Case 1  'Trattamenti Antibutteratura

                StrSQL = ""
                StrSQL = StrSQL & " SELECT  DISTINCT "
                StrSQL = StrSQL & "         Fertilizzanti.FER_COD, "
                StrSQL = StrSQL & "         Fertilizzanti.FER_DES, "
                StrSQL = StrSQL & "         Fertilizzanti.N, "
                StrSQL = StrSQL & "         Fertilizzanti.P2O5, "
                StrSQL = StrSQL & "         Fertilizzanti.K2O, "
                StrSQL = StrSQL & "         Fertilizzanti.MgO "
                StrSQL = StrSQL & " FROM    Fertilizzanti INNER JOIN "
                StrSQL = StrSQL & "         FertilizzantiXTipologie ON "
                StrSQL = StrSQL & "         Fertilizzanti.FER_COD = FertilizzantiXTipologie.FER_COD "
                StrSQL = StrSQL & "         INNER JOIN "
                StrSQL = StrSQL & "         Tipologie ON FertilizzantiXTipologie.TP_COD = Tipologie.TP_COD "
                StrSQL = StrSQL & " WHERE   (Fertilizzanti.FER_DES LIKE '%" & TestoRicerca & "%')"
                StrSQL = StrSQL & " AND     (Tipologie.TP_COD = 1)"
                StrSQL = StrSQL & " ORDER BY FER_DES "
                '=================================================================


            Case 2  'Concimazione Fogliare

                StrSQL = ""
                StrSQL = StrSQL & " SELECT  DISTINCT "
                StrSQL = StrSQL & "         Fertilizzanti.FER_COD, "
                StrSQL = StrSQL & "         Fertilizzanti.FER_DES, "
                StrSQL = StrSQL & "         Fertilizzanti.N, "
                StrSQL = StrSQL & "         Fertilizzanti.P2O5, "
                StrSQL = StrSQL & "         Fertilizzanti.K2O, "
                StrSQL = StrSQL & "         Fertilizzanti.MgO "
                StrSQL = StrSQL & " FROM    Fertilizzanti INNER JOIN "
                StrSQL = StrSQL & "         FertilizzantiXTipologie ON "
                StrSQL = StrSQL & "         Fertilizzanti.FER_COD = FertilizzantiXTipologie.FER_COD "
                StrSQL = StrSQL & "         INNER JOIN "
                StrSQL = StrSQL & "         Tipologie ON FertilizzantiXTipologie.TP_COD = Tipologie.TP_COD "
                StrSQL = StrSQL & " WHERE   (Fertilizzanti.FER_DES LIKE '%" & TestoRicerca & "%')"
                StrSQL = StrSQL & " AND     ("
                StrSQL = StrSQL & "         (Tipologie.TP_COD = 2) OR "
                StrSQL = StrSQL & "         (Tipologie.TP_COD = 3)    "
                StrSQL = StrSQL & "         )"
                StrSQL = StrSQL & " ORDER BY FER_DES "
                '=================================================================


            Case 3  'Fertirrigazione

                StrSQL = ""
                StrSQL = StrSQL & " SELECT  DISTINCT "
                StrSQL = StrSQL & "         Fertilizzanti.FER_COD, "
                StrSQL = StrSQL & "         Fertilizzanti.FER_DES, "
                StrSQL = StrSQL & "         Fertilizzanti.N, "
                StrSQL = StrSQL & "         Fertilizzanti.P2O5, "
                StrSQL = StrSQL & "         Fertilizzanti.K2O, "
                StrSQL = StrSQL & "         Fertilizzanti.MgO "
                StrSQL = StrSQL & " FROM    Fertilizzanti INNER JOIN "
                StrSQL = StrSQL & "         FertilizzantiXTipologie ON "
                StrSQL = StrSQL & "         Fertilizzanti.FER_COD = FertilizzantiXTipologie.FER_COD "
                StrSQL = StrSQL & "         INNER JOIN "
                StrSQL = StrSQL & "         Tipologie ON FertilizzantiXTipologie.TP_COD = Tipologie.TP_COD "
                StrSQL = StrSQL & " WHERE   (Fertilizzanti.FER_DES LIKE '%" & TestoRicerca & "%')"
                StrSQL = StrSQL & " AND     ("
                StrSQL = StrSQL & "         (Tipologie.TP_COD = 3) OR "
                StrSQL = StrSQL & "         (Tipologie.TP_COD = 5)    "
                StrSQL = StrSQL & "         )"
                StrSQL = StrSQL & " ORDER BY FER_DES "
                '=================================================================


            Case 4  'Concimazione Organica

                StrSQL = ""
                StrSQL = StrSQL & " SELECT  DISTINCT "
                StrSQL = StrSQL & "         Fertilizzanti.FER_COD, "
                StrSQL = StrSQL & "         Fertilizzanti.FER_DES, "
                StrSQL = StrSQL & "         Fertilizzanti.N, "
                StrSQL = StrSQL & "         Fertilizzanti.P2O5, "
                StrSQL = StrSQL & "         Fertilizzanti.K2O, "
                StrSQL = StrSQL & "         Fertilizzanti.MgO "
                StrSQL = StrSQL & " FROM    Fertilizzanti INNER JOIN "
                StrSQL = StrSQL & "         FertilizzantiXTipologie ON "
                StrSQL = StrSQL & "         Fertilizzanti.FER_COD = FertilizzantiXTipologie.FER_COD "
                StrSQL = StrSQL & "         INNER JOIN "
                StrSQL = StrSQL & "         Tipologie ON FertilizzantiXTipologie.TP_COD = Tipologie.TP_COD "
                StrSQL = StrSQL & " WHERE   (Fertilizzanti.FER_DES LIKE '%" & TestoRicerca & "%')"
                StrSQL = StrSQL & " AND     ("
                StrSQL = StrSQL & "         (Tipologie.TP_COD = 6) OR "
                StrSQL = StrSQL & "         (Tipologie.TP_COD = 7) OR "
                StrSQL = StrSQL & "         (Tipologie.TP_COD = 8)    "
                StrSQL = StrSQL & "         )"
                StrSQL = StrSQL & " ORDER BY FER_DES "
                '=================================================================


            Case 5  'Concimazione inpieno Campo

                StrSQL = ""
                StrSQL = StrSQL & " SELECT  DISTINCT "
                StrSQL = StrSQL & "         Fertilizzanti.FER_COD, "
                StrSQL = StrSQL & "         Fertilizzanti.FER_DES, "
                StrSQL = StrSQL & "         Fertilizzanti.N, "
                StrSQL = StrSQL & "         Fertilizzanti.P2O5, "
                StrSQL = StrSQL & "         Fertilizzanti.K2O, "
                StrSQL = StrSQL & "         Fertilizzanti.MgO "
                StrSQL = StrSQL & " FROM    Fertilizzanti INNER JOIN "
                StrSQL = StrSQL & "         FertilizzantiXTipologie ON "
                StrSQL = StrSQL & "         Fertilizzanti.FER_COD = FertilizzantiXTipologie.FER_COD "
                StrSQL = StrSQL & "         INNER JOIN "
                StrSQL = StrSQL & "         Tipologie ON FertilizzantiXTipologie.TP_COD = Tipologie.TP_COD "
                StrSQL = StrSQL & " WHERE   (Fertilizzanti.FER_DES LIKE '%" & TestoRicerca & "%')"
                StrSQL = StrSQL & " AND     (Tipologie.TP_COD = 4)"
                StrSQL = StrSQL & " ORDER BY FER_DES "
                '=================================================================

        End Select

        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(objSession("ASG_objParametri_Server"))
        'Recupero il recordset
        Rs = objSQL.SqlSelect(objParametri_Server.StringaConnessione,
                              objSession("ASG_Connessione_Server"),
                              StrSQL,
                              0,
                              Messaggio)

        'Elimino l'oggetto
        objSQL = Nothing

        'Verifico la presenza di errori
        If Not IsNothing(Messaggio) Then

            'ERRORE

        Else

            'Verifico se il recordset e' aperto
            If Rs.State <> 0 Then

                Do While Not Rs.EOF

                    If Visualizza_NPKM = False Then

                        Cbl.Items.Add(New ListItem(
                                            Rs.Fields("Fer_Des").Value,
                                            Rs.Fields("Fer_Cod").Value))

                    Else
                        Cbl.Items.Add(New ListItem(
                                            Rs.Fields("Fer_Des").Value & " {N=" & Rs.Fields("N").Value & ";P=" & Rs.Fields("P2O5").Value & ";K=" & Rs.Fields("K2O").Value & ";Mg=" & Rs.Fields("MgO").Value & "}",
                                            Rs.Fields("Fer_Cod").Value))
                    End If

                    Rs.MoveNext()
                Loop

                'Chiudo il recordset
                Rs.Close()

            End If

            'Elimino il recordset
            Rs = Nothing

        End If

    End Sub

    '###############################################################################
    Public Sub CaricaCombo_TrappolexSpecieVegetalixAvversita(ByRef objServer As System.Web.HttpServerUtility,
                                                             ByRef objSession As System.Web.SessionState.HttpSessionState,
                                                             ByRef objPage As System.Web.UI.Page,
                                                             ByRef Cmb As System.Web.UI.WebControls.DropDownList,
                                                             ByVal Veg_Cod As Integer,
                                                             ByVal Av_Cod As Integer,
                                                             ByVal Uso As Integer)

        Dim objSQL As New Codex_Utility.Sql
        Dim StrSQL As String
        Dim Messaggio As String
        Dim Rs As ADODB.Recordset

        StrSQL = ""
        StrSQL += " SELECT DISTINCT Trappole.* "
        StrSQL += " FROM TrappoleXAvversita, Trappole, SpecieVegetaliXAvversita "
        StrSQL += " WHERE TrappoleXAvversita.Trap_Cod = Trappole.Trap_Cod "
        StrSQL += " AND SpecieVegetaliXAvversita.Av_Cod = TrappoleXAvversita.Av_Cod "

        If Uso <> 0 Then
            StrSQL += " AND Trappole.Uso = " & Uso.ToString & " "
        End If

        If Veg_Cod <> 0 Then
            StrSQL += " AND SpecieVegetaliXAvversita.Veg_Cod=" & Veg_Cod.ToString & " "
        End If

        If Av_Cod <> 0 Then
            StrSQL += " AND TrappoleXAvversita.Av_Cod=" & Av_Cod.ToString & " "
        End If


        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(objSession("ASG_objParametri_Server"))
        'Recupero il recordset
        Rs = objSQL.SqlSelect(objParametri_Server.StringaConnessione,
                              objSession("ASG_Connessione_Server"),
                              StrSQL,
                              0,
                              Messaggio)

        'Elimino l'oggetto
        objSQL = Nothing

        'Verifico la presenza di errori
        If Not IsNothing(Messaggio) Then

            'ERRORE

        Else

            '----- Riempio la combo con i dati del recordset

            'Pulisco la combo
            '  Cmb.Items.Clear()

            'Se il recordset non è chiuso allora ...	

            If Rs.State <> 0 Then

                'Inserisco una riga vuota
                'Cmb.Items.Add(New ListItem("", ""))

                'Inserisco i record trovati
                Do While Not Rs.EOF

                    Cmb.Items.Add(New ListItem(Rs.Fields("TRAP_DES").Value,
                                               Rs.Fields("TRAP_COD").Value))

                    Rs.MoveNext()

                Loop

                'Chiudo il recordset
                Rs.Close()

            End If

            'Elimino il recordset
            Rs = Nothing

        End If


    End Sub


    '###############################################################################
    Public Sub CaricaCombo_AvversitaxTrappole(ByRef objServer As System.Web.HttpServerUtility,
                                              ByRef objSession As System.Web.SessionState.HttpSessionState,
                                              ByRef objPage As System.Web.UI.Page,
                                              ByRef Cmb As System.Web.UI.WebControls.DropDownList,
                                              ByVal Veg_Cod As Integer,
                                              ByVal Trap_Cod As Integer,
                                              ByVal Uso As Integer)

        Dim objSQL As New Codex_Utility.Sql
        Dim StrSQL As String
        Dim Messaggio As String
        Dim Rs As ADODB.Recordset

        StrSQL = ""
        StrSQL += " SELECT DISTINCT Avversita.* "
        StrSQL += " FROM TrappoleXAvversita, Avversita, SpecieVegetaliXAvversita,Trappole "
        StrSQL += " WHERE SpecieVegetaliXAvversita.Av_Cod = Avversita.Av_Cod "
        StrSQL += " AND SpecieVegetaliXAvversita.Av_Cod = TrappoleXAvversita.Av_Cod "
        StrSQL += " AND Trappole.Trap_Cod = TrappoleXAvversita.Trap_Cod "

        If Uso <> 0 Then
            StrSQL += " AND Trappole.Uso = " & Uso.ToString & " "
        End If

        If Veg_Cod <> 0 Then
            StrSQL += " AND SpecieVegetaliXAvversita.Veg_Cod=" & Veg_Cod.ToString & " "
        End If

        If Trap_Cod <> 0 Then
            StrSQL += " AND Trappole.Trap_Cod = " & Trap_Cod.ToString & " "
        End If

        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(objSession("ASG_objParametri_Server"))
        'Recupero il recordset
        Rs = objSQL.SqlSelect(objParametri_Server.StringaConnessione,
                              objSession("ASG_Connessione_Server"),
                              StrSQL,
                              0,
                              Messaggio)

        'Elimino l'oggetto
        objSQL = Nothing

        'Verifico la presenza di errori
        If Not IsNothing(Messaggio) Then

            'ERRORE

        Else

            '----- Riempio la combo con i dati del recordset

            'Pulisco la combo
            Cmb.Items.Clear()

            'Se il recordset non è chiuso allora ...	

            If Rs.State <> 0 Then

                'Inserisco una riga vuota
                ' Cmb.Items.Add(New ListItem("", ""))

                'Inserisco i record trovati
                Do While Not Rs.EOF

                    Cmb.Items.Add(New ListItem(Rs.Fields("AV_DES_VOL").Value,
                                               Rs.Fields("AV_COD").Value))

                    Rs.MoveNext()

                Loop

                'Chiudo il recordset
                Rs.Close()

            End If

            'Elimino il recordset
            Rs = Nothing

        End If


    End Sub


    '###############################################################################
    Public Sub CaricaCombo_DittexTrappole(ByRef objServer As System.Web.HttpServerUtility,
                                          ByRef objSession As System.Web.SessionState.HttpSessionState,
                                          ByRef objPage As System.Web.UI.Page,
                                          ByRef Cmb As System.Web.UI.WebControls.DropDownList,
                                          ByVal Trap_Cod As Integer)

        Dim objSQL As New Codex_Utility.Sql
        Dim StrSQL As String
        Dim Messaggio As String
        Dim Rs As ADODB.Recordset

        StrSQL = ""
        StrSQL += " SELECT Ditte.* "
        StrSQL += " FROM TrappoleXDitta, Ditte "
        StrSQL += " WHERE TrappoleXDitta.Ditta_Cod = Ditte.Ditta_Cod "

        If Trap_Cod <> 0 Then
            StrSQL += " AND TrappoleXDitta.Trap_Cod=" & Trap_Cod.ToString & " "
        End If



        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(objSession("ASG_objParametri_Server"))
        'Recupero il recordset
        Rs = objSQL.SqlSelect(objParametri_Server.StringaConnessione,
                              objSession("ASG_Connessione_Server"),
                              StrSQL,
                              0,
                              Messaggio)

        'Elimino l'oggetto
        objSQL = Nothing

        'Verifico la presenza di errori
        If Not IsNothing(Messaggio) Then

            'ERRORE

        Else

            '----- Riempio la combo con i dati del recordset

            'Pulisco la combo
            Cmb.Items.Clear()

            'Se il recordset non è chiuso allora ...	

            If Rs.State <> 0 Then

                'Inserisco una riga vuota
                '   Cmb.Items.Add(New ListItem("", ""))

                'Inserisco i record trovati
                Do While Not Rs.EOF

                    Cmb.Items.Add(New ListItem(Rs.Fields("DITTA_DES").Value,
                                               Rs.Fields("DITTA_COD").Value))

                    Rs.MoveNext()

                Loop

                'Chiudo il recordset
                Rs.Close()

            End If

            'Elimino il recordset
            Rs = Nothing

        End If


    End Sub

    '###############################################################################
    Public Sub CaricaCheckBoxList_Sezioni_SchedaCampagna2(ByVal TipoScheda As String,
                                                          ByRef Cbl As System.Web.UI.WebControls.CheckBoxList)


        '--------------------------------
        'a = Frontespizio
        'o = Dati Catastali
        'b = Fertilizzazioni
        'c = Trattamenti
        'd = Fasi Fenologiche
        'e = Trappole Installate
        'f = Rilievi Avversità nelle Trappole
        'g = Rilievi Avversità in Campo
        'h = Irrigazione
        'i = Altre Operazioni Colturali
        'l = Indice di Maturità e Raccolta
        'm = Piogge
        'n = Rilievo Produzione e Raccolta
        '--------------------------------

        'Pulisco la lista
        Cbl.Items.Clear()

        Select Case TipoScheda

            Case enum_CodificaStampe.SchedaCampagna_2078

                Cbl.Items.Add(New ListItem("Fertilizzazioni", "b"))
                Cbl.Items.Add(New ListItem("Trattamenti Insetticidi, Acaricidi, Funghicidi, Erbicidi e Fitoregolatori", "c"))
                Cbl.Items.Add(New ListItem("Trappole Installate", "e"))
                Cbl.Items.Add(New ListItem("Rilievi Avversità nelle Trappole", "f"))
                Cbl.Items.Add(New ListItem("Irrigazione", "h"))
                Cbl.Items.Add(New ListItem("Altre Operazioni Colturali", "i"))
                Cbl.Items.Add(New ListItem("Indice di Maturità e Raccolta", "l"))
                Cbl.Items.Add(New ListItem("Piogge", "m"))

            Case enum_CodificaStampe.RegistroTrattamenti

                Cbl.Items.Add(New ListItem("Trattamenti Insetticidi, Acaricidi, Funghicidi, Erbicidi e Fitoregolatori", "2"))

            Case enum_CodificaStampe.SchedaRegistrazione,
                 enum_CodificaStampe.SchedaColturale_Biologico

                Cbl.Items.Add(New ListItem("Fertilizzazioni", "b"))
                Cbl.Items.Add(New ListItem("Trattamenti Insetticidi, Acaricidi, Funghicidi, Erbicidi e Fitoregolatori", "c"))
                Cbl.Items.Add(New ListItem("Osservazioni Fasi Fenologiche", "d"))
                Cbl.Items.Add(New ListItem("Trappole Installate", "e"))
                Cbl.Items.Add(New ListItem("Rilievi Avversità nelle Trappole", "f"))
                Cbl.Items.Add(New ListItem("Irrigazione", "h"))
                Cbl.Items.Add(New ListItem("Altre Operazioni Colturali", "i"))
                Cbl.Items.Add(New ListItem("Indice di Maturità e Raccolta", "l"))
                Cbl.Items.Add(New ListItem("Piogge", "m"))

            Case enum_CodificaStampe.SchedaCampagna_2078_Semplificata,
                enum_CodificaStampe.SchedaRegistrazione_Semplificata

                Cbl.Items.Add(New ListItem("Frontespizio", "a"))
                Cbl.Items.Add(New ListItem("Fertilizzazioni", "b"))
                Cbl.Items.Add(New ListItem("Trattamenti Insetticidi, Acaricidi, Funghicidi, Erbicidi e Fitoregolatori", "c"))
                Cbl.Items.Add(New ListItem("Trappole Installate", "e"))
                Cbl.Items.Add(New ListItem("Rilievi Avversità nelle Trappole", "f"))
                Cbl.Items.Add(New ListItem("Rilievi Avversità in Campo", "g"))
                Cbl.Items.Add(New ListItem("Irrigazione", "h"))
                Cbl.Items.Add(New ListItem("Altre Operazioni Colturali", "i"))
                Cbl.Items.Add(New ListItem("Indice di Maturità e Raccolta", "l"))
                Cbl.Items.Add(New ListItem("Rilievo Produzione e data Raccolta", "n"))
                Cbl.Items.Add(New ListItem("Piogge", "m"))

            Case enum_CodificaStampe.SchedaCampagna_ConserveItalia

                Cbl.Items.Add(New ListItem("Frontespizio", "a"))
                Cbl.Items.Add(New ListItem("Fertilizzazioni", "b"))
                Cbl.Items.Add(New ListItem("Trattamenti Insetticidi, Acaricidi, Funghicidi, Erbicidi e Fitoregolatori", "c"))
                Cbl.Items.Add(New ListItem("Trappole Installate", "e"))
                Cbl.Items.Add(New ListItem("Rilievi Avversità nelle Trappole", "f"))
                Cbl.Items.Add(New ListItem("Irrigazione", "h"))
                Cbl.Items.Add(New ListItem("Altre Operazioni Colturali", "i"))
                Cbl.Items.Add(New ListItem("Indice di Maturità e Raccolta", "l"))
                Cbl.Items.Add(New ListItem("Piogge", "m"))

            Case enum_CodificaStampe.Eurep_Gap_Semplificata

                Cbl.Items.Add(New ListItem("Frontespizio", "a"))
                Cbl.Items.Add(New ListItem("Fertilizzazioni", "b"))
                Cbl.Items.Add(New ListItem("Trattamenti Insetticidi, Acaricidi, Funghicidi, Erbicidi e Fitoregolatori", "c"))
                Cbl.Items.Add(New ListItem("Trappole Installate", "e"))
                Cbl.Items.Add(New ListItem("Rilievi Avversità nelle Trappole", "f"))
                Cbl.Items.Add(New ListItem("Irrigazione", "h"))
                Cbl.Items.Add(New ListItem("Altre Operazioni Colturali", "i"))
                Cbl.Items.Add(New ListItem("Indice di Maturità e Raccolta", "l"))
                Cbl.Items.Add(New ListItem("Rilievo Produzione e data Raccolta", "n"))
                Cbl.Items.Add(New ListItem("Piogge", "m"))

            Case enum_CodificaStampe.RegistroTrattamenti_Semplificata

                Cbl.Items.Add(New ListItem("Frontespizio", "a"))
                Cbl.Items.Add(New ListItem("Trattamenti Insetticidi, Acaricidi, Funghicidi, Erbicidi e Fitoregolatori", "c"))

            Case enum_CodificaStampe.Eurep_Gap

                Cbl.Items.Add(New ListItem("Fertilizzazioni", "b"))
                Cbl.Items.Add(New ListItem("Trattamenti Insetticidi, Acaricidi, Funghicidi, Erbicidi e Fitoregolatori", "c"))

            Case 4, 16

                'nn ancora gestito

            Case enum_CodificaStampe.SchedaCampagna_Pizzoli

                Cbl.Items.Add(New ListItem("Frontespizio", "a"))
                Cbl.Items.Add(New ListItem("Fertilizzazioni", "b"))
                Cbl.Items.Add(New ListItem("Trattamenti Insetticidi, Acaricidi, Funghicidi, Erbicidi e Fitoregolatori", "c"))
                Cbl.Items.Add(New ListItem("Trappole Installate", "e"))
                Cbl.Items.Add(New ListItem("Rilievi Avversitànelle Trappole", "f"))
                Cbl.Items.Add(New ListItem("Irrigazione", "h"))
                Cbl.Items.Add(New ListItem("Altre Operazioni Colturali", "i"))
                Cbl.Items.Add(New ListItem("Indice di Maturità e Raccolta", "l"))
                Cbl.Items.Add(New ListItem("Rilievo Produzione e data Raccolta", "n"))
                Cbl.Items.Add(New ListItem("Piogge", "m"))


        End Select





    End Sub



    '###############################################################################
    Public Sub CaricaCheckBoxList_Cooperative(ByRef objSession As System.Web.SessionState.HttpSessionState,
                                              ByRef Cbl As System.Web.UI.WebControls.CheckBoxList)
        'Parametri Server
        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(objSession("ASG_objParametri_Server"))

        'Variabili locali
        Dim Dt As DataTable
        Dim grImpreseObj As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_R
        Dim objanagrafeDAL As New AgronicaCoreAnagrafeDAL.Imprese_Read

        Dt = grImpreseObj.CaricaBoxList_Cooperativa(objParametri_Server)

        Dt.DefaultView.Sort = "rag_soc asc"
        Dt = Dt.DefaultView.ToTable

        For Each row In Dt.Rows
            Cbl.Items.Add(New ListItem(objanagrafeDAL.RagSoc_from_Piva(row("figlio").ToString, objParametri_Server), row("figlio")))
        Next

    End Sub

    '###############################################################################
    Public Sub CaricaCheckBoxList_PianiSemina(ByRef objSession As System.Web.SessionState.HttpSessionState,
                                              ByRef Cbl As System.Web.UI.WebControls.CheckBoxList)

        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(objSession("ASG_objParametri_Server"))

        Dim Dt As DataTable

        Dim regImpObj As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R

        Dt = regImpObj.CaricaList_PianiSemina(objParametri_Server)

        Cbl.Items.Clear()

        For Each row In Dt.Rows
            Cbl.Items.Add(New ListItem(row("val_cod"), row("val_cod")))
        Next

    End Sub

    '###############################################################################
    '''Usato
    '''Versione tipo core ma non standard
    ''' <summary>
    ''' nuova versione. Spostata nell'AgronicaCoreUtility.CaricaListControl.
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Sub CaricaCheckBoxList_Cultivar(ByRef Cbl As System.Web.UI.WebControls.CheckBoxList,
                                            ByVal Veg_Cod As Integer,
                                            ByVal Cul_Cod As Integer,
                                            ByVal Cerca_CulDes As String,
                                            ByVal FiltroAggiuntivo As String,
                                            ByVal Ordinamento As String,
                                            ByVal Flag_FiltroUtente As Boolean,
                                            ByVal Cul_Cod_daModificare As Integer,
                                            ByVal VegCod_Rif_CulCod_daModificare As Integer,
                                            ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                            ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            )

        Dim Dt As DataTable
        Dim i As Integer
        Dim x_Cod As Integer
        Dim x_Des As String
        Dim NumTotale As Integer
        Dim Flag_VegCodTrovatoNelFiltroUtente As Boolean = False

        Dim objMatrice As New AgronicaCoreMetaSchemaDAL.Cultivar_R

        Cbl.Items.Clear()

        If Flag_FiltroUtente = True Then

            Dt = objMatrice.GestioneFiltroUtente_Leggi(Cul_Cod,
                                                Veg_Cod,
                                                Cerca_CulDes,
                                                AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                FiltroAggiuntivo,
                                                Ordinamento,
                                                objParametri_Utenti)



        Else

            Dt = objMatrice.Leggi(Cul_Cod,
                                        Veg_Cod,
                                        Cerca_CulDes,
                                        AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                        FiltroAggiuntivo,
                                        Ordinamento,
                                        objParametri_Server)

        End If


        If Not IsNothing(Dt) Then

            NumTotale = Dt.Rows.Count

            For i = 0 To Dt.Rows.Count - 1

                'se sono in info o modifica, e quindi ho passato il Cul_Cod_daModificare
                'faccio il controllo
                If Cul_Cod_daModificare <> 0 Then

                    'se all'interno delle varietà filtrate, trovo il cul_cod da modificare
                    If Dt.Rows(i).Item("Cul_Cod") = Cul_Cod_daModificare Then
                        Flag_VegCodTrovatoNelFiltroUtente = True
                    End If

                Else
                    'non è stato passato il cul_cod
                    'quindi imposto il flag, come se avessi trovato la varietà
                    'così non devo aggiungere niente
                    Flag_VegCodTrovatoNelFiltroUtente = True
                End If

                x_Cod = Dt.Rows(i).Item("Cul_Cod")

                x_Des = CStr(Dt.Rows(i).Item("Cul_Des"))

                Cbl.Items.Add(New ListItem(x_Des, x_Cod))

            Next

        Else
            NumTotale = 0
        End If

        Dt = Nothing

        Dim objCultivar As New AgronicaCoreMetaSchemaDAL.Cultivar_R
        'se sono in info o modifica, e quindi ho passato il Cul_Cod_daModificare
        If Cul_Cod_daModificare <> 0 Then

            'se all'interno delle varietà filtrate, non è stato trovato il cul_cod da modificare
            If Flag_VegCodTrovatoNelFiltroUtente = False Then

                x_Des = ""

                'se il Cul_Cod_daModificare è valorizzato, devo sapere anche il veg_cod
                '(se non l'ho passato me lo ricavo e intanto che ci sono ricavo la descrizione della varietà)
                'però solo nel caso di veg_cod <> 0
                If Veg_Cod <> 0 And VegCod_Rif_CulCod_daModificare = 0 Then

                    objCultivar.VegCod_VegDes_CulDes_from_CulCod(Cul_Cod_daModificare,
                                                       VegCod_Rif_CulCod_daModificare,
                                                       "",
                                                       x_Des,
                                                       objParametri_Server)
                End If

                'se il veg_cod della varietà da modificare è lo stesso di quello selezionato,
                'allora inserisco la varietà da modificare nel menù
                If Veg_Cod = VegCod_Rif_CulCod_daModificare Then

                    'se la descrizione non l'ho già ricavata sopra
                    If x_Des = "" Then
                        objCultivar.CulDes_from_CulCod(Cul_Cod_daModificare, objParametri_Server)
                        'x_Des = CulDes_from_CulCod(objServer, objSession, objPage, Cul_Cod_daModificare)
                    End If

                    Cbl.Items.Add(New ListItem(x_Des, Cul_Cod_daModificare))

                End If

            End If

        End If
    End Sub





    '###############################################################################
    Public Sub CaricaCheckBoxList_Utenti(ByRef objServer As System.Web.HttpServerUtility,
                                         ByRef objSession As System.Web.SessionState.HttpSessionState,
                                         ByRef objPage As System.Web.UI.Page,
                                         ByRef Cbl As System.Web.UI.WebControls.CheckBoxList,
                                         ByVal User_Profilo As String)

        Dim Rs As ADODB.Recordset
        Dim objSQL As New Codex_Utility.Sql
        Dim StrSql As String
        Dim Messaggio As String



        '========================================================================

        'è stata sostituita la chiamata al componente con una query diretta, 
        'per gestire il MULTIHOST 

        StrSql = ""
        StrSql = " SELECT "
        StrSql += "         Utenti_Dettagli.UserName, Utenti_Dettagli.Cognome, Utenti_Dettagli.Nome, Utenti_Dettagli.PIVA, Utenti_Dettagli.CodFisc, Utenti_Dettagli.Rag_Soc, "
        StrSql += "         Utenti_Dettagli.Data_Creazione, Utenti_Dettagli.Data_Modifica, Utenti_Dettagli.UserNameCommerciale, "
        StrSql += "         Utenti_Profili.Utente, Utenti_Profili.Utente_Profilo "
        StrSql += " FROM    Utenti_Dettagli "
        StrSql += "         INNER JOIN Utenti_Profili ON Utenti_Dettagli.UserName = Utenti_Profili.Utente "
        StrSql += " WHERE   Utenti_Profili.Utente_Profilo = '" & Agro_SQL_SaveText(User_Profilo) & "' "
        StrSql += " AND     Utenti_Profili.Id_Servizio = " & Agro_SQL_SaveNum(CInt(objSession("ASG_IdServizio")))
        StrSql += " ORDER BY Utenti_Dettagli.UserName "
        Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(objSession("ASG_objParametri_Utenti"))
        Rs = objSQL.SqlSelect(objParametri_Utenti.StringaConnessione,
                                objSession("ASG_Connessione_Utenti"),
                                StrSql,
                                0,
                                Messaggio)

        'Elimino l'oggetto
        objSQL = Nothing

        If IsNothing(Messaggio) Then

            'Se il recordset non è chiuso allora ...	
            If Rs.State <> 0 Then

                Do While Not Rs.EOF

                    Cbl.Items.Add(New ListItem(Rs.Fields("UserName").Value, Rs.Fields("CodFisc").Value))

                    Rs.MoveNext()

                Loop

                'Chiudo il recordset
                Rs.Close()

            End If

        End If

        'Elimino il recordset
        Rs = Nothing

    End Sub


    '###############################################################################
    Public Sub CaricaCombo_Anno(ByRef Cmb As System.Web.UI.WebControls.DropDownList)

        Dim i As Integer

        Cmb.Items.Clear()

        For i = 1990 To 2030
            Cmb.Items.Add(New ListItem(CStr(i)))
        Next


    End Sub

    '###############################################################################
    Public Sub CaricaCombo_Anno_Parametrizzata(ByRef Cmb As System.Web.UI.WebControls.DropDownList,
                                                ByVal Anno_Inizio As Integer,
                                                ByVal Anno_Fine As Integer,
                                                Optional ByVal Flag_PrimaRiga As Boolean = True,
                                                Optional ByVal Testo_PrimaRiga As String = "",
                                                Optional ByVal Cod_PrimaRiga As String = "0")

        If Anno_Inizio > Anno_Fine Then
            Throw New Exception("L'anno inizio deve essere minore dell'anno fine!")
        End If

        Dim i As Integer

        Cmb.Items.Clear()

        If Flag_PrimaRiga = True Then
            Cmb.Items.Add(New ListItem(Testo_PrimaRiga, Cod_PrimaRiga))
        End If

        For i = Anno_Inizio To Anno_Fine
            Cmb.Items.Add(New ListItem(CStr(i)))
        Next


    End Sub

    '###############################################################################
    Public Sub CaricaCombo_Mesi(ByRef Cmb_Mesi As System.Web.UI.WebControls.DropDownList)

        'Combo MESI
        Cmb_Mesi.Items.Clear()

        Cmb_Mesi.Items.Add(New ListItem("Gennaio", "01"))
        Cmb_Mesi.Items.Add(New ListItem("Febbraio", "02"))
        Cmb_Mesi.Items.Add(New ListItem("Marzo", "03"))

        Cmb_Mesi.Items.Add(New ListItem("Aprile", "04"))
        Cmb_Mesi.Items.Add(New ListItem("Maggio", "05"))
        Cmb_Mesi.Items.Add(New ListItem("Giugno", "06"))

        Cmb_Mesi.Items.Add(New ListItem("Luglio", "07"))
        Cmb_Mesi.Items.Add(New ListItem("Agosto", "08"))
        Cmb_Mesi.Items.Add(New ListItem("Settembre", "09"))

        Cmb_Mesi.Items.Add(New ListItem("Ottobre", "10"))
        Cmb_Mesi.Items.Add(New ListItem("Novembre", "11"))
        Cmb_Mesi.Items.Add(New ListItem("Dicembre", "12"))

    End Sub

    '###############################################################################
    Public Sub CaricaCombo_Mesi_Parametrizzata(ByRef Cmb_Mesi As System.Web.UI.WebControls.DropDownList,
                                                Optional ByVal Flag_PrimaRiga As Boolean = True,
                                                Optional ByVal Testo_PrimaRiga As String = "",
                                                Optional ByVal Cod_PrimaRiga As String = "00")

        Dim i As Integer

        Cmb_Mesi.Items.Clear()

        If Flag_PrimaRiga = True Then
            Cmb_Mesi.Items.Add(New ListItem(Testo_PrimaRiga, Cod_PrimaRiga))
        End If

        Cmb_Mesi.Items.Add(New ListItem("Gennaio", "01"))
        Cmb_Mesi.Items.Add(New ListItem("Febbraio", "02"))
        Cmb_Mesi.Items.Add(New ListItem("Marzo", "03"))

        Cmb_Mesi.Items.Add(New ListItem("Aprile", "04"))
        Cmb_Mesi.Items.Add(New ListItem("Maggio", "05"))
        Cmb_Mesi.Items.Add(New ListItem("Giugno", "06"))

        Cmb_Mesi.Items.Add(New ListItem("Luglio", "07"))
        Cmb_Mesi.Items.Add(New ListItem("Agosto", "08"))
        Cmb_Mesi.Items.Add(New ListItem("Settembre", "09"))

        Cmb_Mesi.Items.Add(New ListItem("Ottobre", "10"))
        Cmb_Mesi.Items.Add(New ListItem("Novembre", "11"))
        Cmb_Mesi.Items.Add(New ListItem("Dicembre", "12"))

    End Sub



    '###############################################################################
    '''Usato
    '''Versione tipo core ma non standard
    '''Spostare nei core a standardizzare
    ''' 
    ''' carico la lista con i gruppi operazioni colturali (chiamata dal filtrone)
    Public Sub CaricaCheckBoxList_GruppoOperazioni(ByRef Cbl As System.Web.UI.WebControls.CheckBoxList,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                    )
        Dim Dt As DataTable
        Dim i As Integer
        Dim x_Cod As Integer
        Dim x_Des As String
        Dim NumTotale As Integer
        Dim FiltroAggiuntivo, Ordinamento As String


        'Cbl.Items.Clear()

        FiltroAggiuntivo = "(GOper.GRU_COD = 1 OR GOper.GRU_COD = 2 OR GOper.GRU_COD = 3 OR GOper.GRU_COD = 4 ) "

        Dim objGrOp As New AgronicaCoreMetaSchemaDAL.GruppoOperazioni_R

        Dt = objGrOp.Leggi(0,
                            "",
                            0, "", True, False, False, False, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                FiltroAggiuntivo, "", objParametri)

        If Not IsNothing(Dt) Then

            NumTotale = Dt.Rows.Count

            For i = 0 To Dt.Rows.Count - 1

                x_Cod = Dt.Rows(i).Item("Gru_Cod")

                x_Des = CStr(Dt.Rows(i).Item("Gru_des"))

                Cbl.Items.Add(New ListItem(x_Des, x_Cod))

            Next

        Else
            NumTotale = 0
        End If

        Dt = Nothing

    End Sub

    '###############################################################################
    '''Usato
    '''Versione tipo core ma non standard
    '''Spostare nei core a standardizzare
    Public Sub CaricaCheckBoxList_GruppoVarietalexSpecie(ByRef Cbl As System.Web.UI.WebControls.CheckBoxList,
                                    ByVal VegCod As Integer,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    )

        Dim objCOM As New AgronicaCoreMetaSchemaDAL.SpecieVegetalixGruppoVarietale_R  'Agro_DB_AD.SpecieVegxGruppoVarietale_R
        Dim DT As DataTable

        'Creo gli oggetti COM
        'objCOM = .......("Agro_DB_AD.SpecieVegxGruppoVarietale_R")

        'Leggo le imprese associate al profilo selezionato			
        DT = objCOM.Leggi(CInt(VegCod),
                          0,
                           AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                           "", "",
                          objParametri)

        'Pulisco la lista
        Cbl.Items.Clear()

        'Se il recordset non è chiuso allora ...	
        If DT.Rows.Count > 0 Then

            Dim i As Integer
            For i = 0 To DT.Rows.Count - 1

                'Non Ibrido
                Cbl.Items.Add(New ListItem(DT.Rows(i).Item("Grva_Des"),
                                           DT.Rows(i).Item("Grva_Cod")))

                'Ibrido
                Cbl.Items.Add(New ListItem(DT.Rows(i).Item("Grva_Des") & " -- Ibrido",
                                           -CInt(DT.Rows(i).Item("Grva_Cod"))))

            Next

        End If


    End Sub


    '###############################################################################
    Public Sub CaricaCheckBoxList_Avversita(ByRef objServer As System.Web.HttpServerUtility,
                                            ByRef objSession As System.Web.SessionState.HttpSessionState,
                                            ByRef objPage As System.Web.UI.Page,
                                            ByRef Cbl As System.Web.UI.WebControls.CheckBoxList,
                                            Optional ByVal Veg_Cod As Integer = 0,
                                            Optional ByVal Grsp_Cod As Integer = 0)

        Dim objMetaSchemaDAL As New AgronicaCoreMetaSchemaDAL.GruppoColturaleXSpecieVegetali_R

        Dim objSQL As New Codex_Utility.Sql
        Dim objSQLGrsp As New Codex_Utility.Sql
        Dim StrSQL As String
        Dim Messaggio As String
        Dim Rs As ADODB.Recordset
        Dim RsGrsp As ADODB.Recordset
        Dim Array_VegCod() As Integer
        Dim i As Integer
        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(objSession("ASG_objParametri_Server"))
        If (Veg_Cod = 0) And (Grsp_Cod = 0) Then


            StrSQL = " "
            StrSQL += "SELECT Av_Cod, Av_Des_Vol FROM Avversita "
            StrSQL += "ORDER BY Av_Des_Vol "

        ElseIf ((Veg_Cod <> 0) And (Grsp_Cod <> 0)) Or ((Veg_Cod <> 0) And (Grsp_Cod = 0)) Then

            StrSQL = " "
            StrSQL += " SELECT Avversita.Av_Des_Vol, Avversita.Av_Cod "
            StrSQL += " FROM Avversita INNER JOIN SpecieVegetalixAvversita ON Avversita.Av_Cod = SpecieVegetalixAvversita.Av_Cod "
            StrSQL += " WHERE SpecieVegetalixAvversita.Veg_Cod = " & Veg_Cod
            StrSQL += " ORDER BY Av_Des_Vol "


        ElseIf (Veg_Cod = 0) And (Grsp_Cod <> 0) Then

            objMetaSchemaDAL.VegCod_from_GrspCod(Grsp_Cod, Array_VegCod, objParametri_Server)

            If Not Array_VegCod Is Nothing Then

                For i = 0 To Array_VegCod.Length - 1

                    Veg_Cod = Array_VegCod(i)

                    StrSQL = " "
                    StrSQL += " SELECT Avversita.Av_Des_Vol, Avversita.Av_Cod "
                    StrSQL += " FROM Avversita INNER JOIN SpecieVegetalixAvversita ON Avversita.Av_Cod = SpecieVegetalixAvversita.Av_Cod "
                    StrSQL += " WHERE SpecieVegetalixAvversita.Veg_Cod = " & Veg_Cod
                    'Recupero il recordset
                    RsGrsp = objSQLGrsp.SqlSelect(objParametri_Server.StringaConnessione,
                                                    objSession("ASG_Connessione_Server"),
                                                    StrSQL,
                                                    0,
                                                    Messaggio)

                    'Verifico la presenza di errori
                    If Not IsNothing(Messaggio) Then

                        'ERRORE
                    Else

                        If RsGrsp.State <> 0 Then

                            'Inserisco i record trovati
                            Do While Not RsGrsp.EOF

                                Cbl.Items.Add(New ListItem(RsGrsp.Fields("Av_Des_Vol").Value,
                                                           RsGrsp.Fields("Av_Cod").Value))

                                RsGrsp.MoveNext()

                            Loop

                            'Chiudo il recordset
                            RsGrsp.Close()

                        End If

                        'Elimino il recordset
                        RsGrsp = Nothing

                    End If

                Next

            End If

        End If

        'Recupero il recordset
        Rs = objSQL.SqlSelect(objParametri_Server.StringaConnessione,
                              objSession("ASG_Connessione_Server"),
                              StrSQL,
                              0,
                              Messaggio)

        'Elimino l'oggetto
        objSQL = Nothing

        'Verifico la presenza di errori
        If Not IsNothing(Messaggio) Then

            'ERRORE

        Else

            '----- Riempio la combo con i dati del recordset

            'Se il recordset non è chiuso allora ...	
            If Rs.State <> 0 Then

                'Inserisco i record trovati
                Do While Not Rs.EOF

                    Cbl.Items.Add(New ListItem(Rs.Fields("Av_Des_Vol").Value,
                                               Rs.Fields("Av_Cod").Value))

                    Rs.MoveNext()

                Loop

                'Chiudo il recordset
                Rs.Close()

            End If

            'Elimino il recordset
            Rs = Nothing

        End If


    End Sub


    '###############################################################################
    Public Sub CaricaCheckBoxList_Infestanti(ByRef objServer As System.Web.HttpServerUtility,
                                             ByRef objSession As System.Web.SessionState.HttpSessionState,
                                             ByRef objPage As System.Web.UI.Page,
                                             ByRef Cbl As System.Web.UI.WebControls.CheckBoxList)

        Dim objSQL As New Codex_Utility.Sql
        Dim StrSQL As String
        Dim Messaggio As String
        Dim Rs As ADODB.Recordset

        StrSQL = " "
        StrSQL += " SELECT Avversita.Av_Des_Vol, Avversita.Av_Cod "
        StrSQL += " FROM InfestantiAttive INNER JOIN Avversita ON InfestantiAttive.AV_COD = Avversita.Av_Cod "
        StrSQL += " ORDER BY Avversita.Av_Des_Vol "
        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(objSession("ASG_objParametri_Server"))
        'Recupero il recordset
        Rs = objSQL.SqlSelect(objParametri_Server.StringaConnessione,
                              objSession("ASG_Connessione_Server"),
                              StrSQL,
                              0,
                              Messaggio)

        'Elimino l'oggetto
        objSQL = Nothing

        'Verifico la presenza di errori
        If Not IsNothing(Messaggio) Then

            'ERRORE

        Else

            '----- Riempio la lista con i dati del recordset

            'Pulisco la lista
            Cbl.Items.Clear()

            'Se il recordset non è chiuso allora ...	

            If Rs.State <> 0 Then


                'Inserisco i record trovati
                Do While Not Rs.EOF

                    Cbl.Items.Add(New ListItem(Rs.Fields("Av_Des_Vol").Value,
                                               Rs.Fields("Av_Cod").Value))

                    Rs.MoveNext()

                Loop

                'Chiudo il recordset
                Rs.Close()

            End If

            'Elimino il recordset
            Rs = Nothing

        End If


    End Sub

    '###############################################################################
    Public Sub CaricaCheckBoxList_GruppoInfestanti(ByRef objServer As System.Web.HttpServerUtility,
                                                   ByRef objSession As System.Web.SessionState.HttpSessionState,
                                                   ByRef objPage As System.Web.UI.Page,
                                                   ByRef Cbl As System.Web.UI.WebControls.CheckBoxList)

        Dim objSQL As New Codex_Utility.Sql
        Dim StrSQL As String
        Dim Messaggio As String
        Dim Rs As ADODB.Recordset

        StrSQL = " "
        StrSQL += " SELECT GruppoAvversita.Av_Gru_Des, GruppoAvversita.Av_Gru "
        StrSQL += " FROM GruppoAvversita INNER JOIN GruppoAvversitaAttive ON GruppoAvversita.Av_Gru = GruppoAvversitaAttive.AV_GRU "
        StrSQL += " ORDER BY GruppoAvversita.Av_Gru_Des "
        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(objSession("ASG_objParametri_Server"))
        'Recupero il recordset
        Rs = objSQL.SqlSelect(objParametri_Server.StringaConnessione,
                              objSession("ASG_Connessione_Server"),
                              StrSQL,
                              0,
                              Messaggio)

        'Elimino l'oggetto
        objSQL = Nothing

        'Verifico la presenza di errori
        If Not IsNothing(Messaggio) Then

            'ERRORE

        Else

            '----- Riempio la lista con i dati del recordset

            'Pulisco la lista
            Cbl.Items.Clear()

            'Se il recordset non è chiuso allora ...	

            If Rs.State <> 0 Then


                'Inserisco i record trovati
                Do While Not Rs.EOF

                    Cbl.Items.Add(New ListItem(Rs.Fields("Av_Gru_Des").Value,
                                               Rs.Fields("Av_Gru").Value))

                    Rs.MoveNext()

                Loop

                'Chiudo il recordset
                Rs.Close()

            End If

            'Elimino il recordset
            Rs = Nothing

        End If


    End Sub



    '###############################################################################
    '''Usato
    '''Versione tipo core ma non standard
    '''Spostare nei core a standardizzare
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' nuova versione 
    ''' defautl
    ''' Flag_EseguiFiltroUtente As Boolean = True
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Sub CaricaCheckBoxList_Operazioni(ByVal Gru_Op As Integer,
                                            ByRef Cbl As System.Web.UI.WebControls.CheckBoxList,
                                            ByVal Flag_EseguiFiltroUtente As Boolean,
                                             ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                             ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            )


        'Gru_Op = PARAMETRO X FILTRARE SUL GRUPPO


        Dim Dt As DataTable
        Dim i As Integer
        Dim x_Cod As Integer
        Dim x_Des As String
        Dim NumTotale As Integer
        Dim FiltroAggiuntivo = ""
        Dim Ordinamento As String

        Ordinamento = " Operazioni.LAV_DES "


        If Flag_EseguiFiltroUtente = False Then

            '-----------------------------------------
            '--------- SENZA FILTRO UTENTE -----------
            '-----------------------------------------

            If Gru_Op = 0 Then
                FiltroAggiuntivo = " (Operazioni.gru_op = 1 OR Operazioni.gru_op = 2 OR Operazioni.gru_op = 3 OR Operazioni.gru_op = 4 ) "
            End If
            Dim objOperazioni As New AgronicaCoreMetaSchemaDAL.Operazioni_R
            Dt = objOperazioni.Leggi(0, 0,
                                 Gru_Op,
                                 "", 0, "", "", False, False, False, False,
                                  AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                  FiltroAggiuntivo, Ordinamento,
                                  objParametri_Server)


        Else

            '-----------------------------------------
            '------------ FILTRO UTENTE --------------
            '-----------------------------------------

            Select Case Gru_Op

                Case 0 'voglio vederle tutte ---> qusto caso è ancora da testare

                    FiltroAggiuntivo = " (Operazioni.gru_op = 1 OR Operazioni.gru_op = 2 OR Operazioni.gru_op = 3 OR Operazioni.gru_op = 4 ) "
                    Dim objOpeGestF As New AgronicaCoreMetaSchemaDAL.Operazioni_GestioneFiltroUtente_R

                    'devo fare una UNION tra la query delle lavorazioni e la query normale per gli altri gruppi op.
                    Dt = objOpeGestF.Leggi(0, 0, 0, "", 0, "", "", False, False, False, False,
                                 AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                 "", Ordinamento, objParametri_Server)

                    '--------------------------------------------------------------------

                Case 4 'lavorazioni

                    'query che tiene conto del filtro utente
                    Dim objOperazioniLavoriazioni As New AgronicaCoreMetaSchemaDAL.OperazioniLavorazioni_GestioneFiltroUtente_R
                    Dt = objOperazioniLavoriazioni.Leggi(0, 0, 0, "", 0, "", "", False, False, False, False,
                  AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                  "", Ordinamento, objParametri_Utenti)


                    '--------------------------------------------------------------------

                Case Else 'tutte le altre

                    'query normale
                    Dim objOperazioniR As New AgronicaCoreMetaSchemaDAL.Operazioni_R

                    Dt = objOperazioniR.Leggi(0,
                                                    0,
                                                    Gru_Op,
                                                    "", 0, "", "", False, False, False, False,
                                                     AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                     "",
                                                     Ordinamento, objParametri_Server)

                    '--------------------------------------------------------------------


            End Select


        End If


        If Not IsNothing(Dt) Then

            NumTotale = Dt.Rows.Count

            For i = 0 To Dt.Rows.Count - 1

                x_Cod = Dt.Rows(i).Item("Lav_Cod")

                x_Des = CStr(Dt.Rows(i).Item("lav_des"))

                Cbl.Items.Add(New ListItem(x_Des, x_Cod))

            Next

        Else
            NumTotale = 0
        End If

        Dt = Nothing



    End Sub



    '#######################################################################################
    Public Sub CaricaCombo_CategoriaOP(ByRef Cmb As System.Web.UI.WebControls.DropDownList)

        'Pulisco la combo
        Cmb.Items.Clear()

        'Inserisco una riga vuota
        Cmb.Items.Add(New ListItem("", ""))

        Cmb.Items.Add(New ListItem("ORTOFRUTTA", "A"))
        Cmb.Items.Add(New ListItem("FRUTTA", "B"))
        Cmb.Items.Add(New ListItem("ORTAGGI", "C"))
        Cmb.Items.Add(New ListItem("PRODOTTI DESTINATI ALLA TRASFORMAZIONE", "D"))
        Cmb.Items.Add(New ListItem("AGRUMI", "E"))
        Cmb.Items.Add(New ListItem("FRUTTA A GUSCIO", "F"))
        Cmb.Items.Add(New ListItem("FUNGHI", "G"))


    End Sub

    '#######################################################################################
    Public Sub CaricaCombo_TipoRiconoscimento(ByRef Cmb As System.Web.UI.WebControls.DropDownList)

        'Pulisco la combo
        Cmb.Items.Clear()

        'Inserisco una riga vuota
        Cmb.Items.Add(New ListItem("", ""))

        Cmb.Items.Add(New ListItem("art. 11 Reg. 2200/96", "1"))
        Cmb.Items.Add(New ListItem("art. 13 par. I Reg.to 2200/96", "2"))
        Cmb.Items.Add(New ListItem("art. 13 par. II Reg.to 2200/96", "3"))
        Cmb.Items.Add(New ListItem("art. 14 Reg.to 2200/96", "4"))

    End Sub



    '###############################################################################
    Public Sub CaricaCombo_CapitolatiPrivati(ByRef objparametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                             ByRef Cmb As System.Web.UI.WebControls.DropDownList)

        Dim objCoreAnagrafeDAL As New AgronicaCoreAnagrafeDAL.CAC_Codifica_InfoAggiuntive_R

        Dim i As Integer

        Dim Dt_Capitolati As DataTable

        Dt_Capitolati = New DataTable

        'Pulisco la combo
        Cmb.Items.Clear()

        'Inserisco una riga vuota
        Cmb.Items.Add(New ListItem("", "0"))

        Dt_Capitolati = objCoreAnagrafeDAL.Leggi(0,
                                                 "",
                                                 1,
                                                 0,
                                                 enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                 "",
                                                 "",
                                                 objparametri_Server)

        For i = 0 To Dt_Capitolati.Rows.Count - 1

            Cmb.Items.Add(New ListItem(CStr(Dt_Capitolati.Rows(i).Item("InfoAgg_Des")),
                           CStr(Dt_Capitolati.Rows(i).Item("InfoAgg_Cod"))))

        Next

        Dt_Capitolati = Nothing


    End Sub



    '###############################################################################
    Public Sub CaricaCombo_CategorieDocumenti(ByRef objparametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                              ByRef Cmb As System.Web.UI.WebControls.DropDownList)

        Dim objMetaSchemaDAL As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R

        Dim objCOM As Object 'New Agro_Contab_AD.Categorie_Magazzino_R
        Dim Dt As New DataTable

        'Leggo le imprese associate al profilo selezionato			
        Dt = objMetaSchemaDAL.Leggi(0, "", "",
                                            objparametri_Server)

        'Pulisco la combo
        Cmb.Items.Clear()

        'Inserisco una riga vuota 
        Cmb.Items.Add(New ListItem("", ""))

        '----- Riempio la combo con i dati del datatable
        Dim i As Integer
        For i = 0 To Dt.Rows.Count - 1
            Cmb.Items.Add(New ListItem(Dt.Rows(i).Item("Cat_Des"),
                                       Dt.Rows(i).Item("Cat_Cod")))
        Next

        'Elimino il DataTable
        Dt = Nothing

    End Sub




    '###############################################################################
    Public Sub CaricaCheckBoxList_Note_Intervento_utilizzo_cod(ByRef Cbl As System.Web.UI.WebControls.CheckBoxList,
                                                  ByVal NotaUtilizzo_Cod As Integer,
                                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)


        Dim ObjContab As New AgronicaCoreContabDAL.Note_Intervento_R
        Dim Dt As New DataTable
        Dim i As Integer = 0

        Dt = ObjContab.Leggi_con_Utilizzo(0,
                              0, NotaUtilizzo_Cod,
                             AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                             "",
                             "",
                             objParametri)

        ObjContab = Nothing

        '----- Riempio la combo con i dati del recordset

        'Pulisco la lista
        Cbl.Items.Clear()

        'Inserisco i record trovati
        For i = 0 To Dt.Rows.Count - 1
            Cbl.Items.Add(New ListItem(Dt.Rows(i).Item("Nota_Des"),
                                       Dt.Rows(i).Item("Nota_Cod")))
        Next

        Dt.Dispose()
        Dt = Nothing

    End Sub


    Public Sub CaricaCheckBoxList_Note_Intervento(ByRef Cbl As System.Web.UI.WebControls.CheckBoxList,
                                                  ByVal NotaGruppo_Cod As Integer,
                                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)


        Dim ObjContab As New AgronicaCoreContabDAL.Note_Intervento_R
        Dim Dt As New DataTable
        Dim i As Integer = 0

        Dt = ObjContab.Leggi(0,
                             NotaGruppo_Cod,
                             AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                             "",
                             "",
                             objParametri)

        ObjContab = Nothing

        '----- Riempio la combo con i dati del recordset

        'Pulisco la lista
        Cbl.Items.Clear()

        'Inserisco i record trovati
        For i = 0 To Dt.Rows.Count - 1
            Cbl.Items.Add(New ListItem(Dt.Rows(i).Item("Nota_Des"),
                                       Dt.Rows(i).Item("Nota_Cod")))
        Next

        Dt.Dispose()
        Dt = Nothing

    End Sub

End Module
