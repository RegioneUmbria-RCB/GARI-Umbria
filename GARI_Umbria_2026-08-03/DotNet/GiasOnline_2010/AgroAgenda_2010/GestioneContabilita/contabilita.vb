Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider
Imports AgronicaCoreXML.XML_Stampe

Module contabilita


    '###############################################################################
    'questa funzione è collegata a:
    'CaricaCombo_TipoMovimentoContabile
    'CaricaCombo_TipoCausale
    Public Function TipoCausale_from_LavCod_e_Tipo(ByVal TipoMovimentoContabile As Integer, ByVal Lav_Cod As Integer) As String


        Select Case TipoMovimentoContabile

            Case enum_TipoMovimentoContabile.MovEconomici

                Select Case Lav_Cod

                    'RESI SU ACQUISTI
                    ' LAVCOD_NOTA_ACCREDITO_DAL_FORNITORE

                    'RESI SU VENDITE
                    'LAVCOD_NOTA_ACCREDITO_AL_CLIENTE 

                    Case LAVCOD_FATTURA_EMESSA
                        Return "Fattura Emessa"

                    Case LAVCOD_FATTURA_RICEVUTA
                        Return "Fattura Ricevuta"

                    Case LAVCOD_RICEVUTA_EMESSA
                        Return "Ricevuta Fiscale Emessa"

                    Case LAVCOD_ALTRI_RICAVI
                        Return "Altri Ricavi"

                    Case LAVCOD_ALTRI_COSTI
                        Return "Altri Costi"

                    Case LAVCOD_VENDITA
                        Return "Vendite"

                    Case LAVCOD_ACQUISTO
                        Return "Acquisti"

                    Case LAVCOD_REG_COMPENSI
                        Return "Corrispettivi"

                End Select

                '---------------------------------------------

            Case enum_TipoMovimentoContabile.Pagamenti

                Select Case Lav_Cod

                    'RESI SU ACQUISTI
                    ' LAVCOD_NOTA_ACCREDITO_DAL_FORNITORE

                    'RESI SU VENDITE
                    'LAVCOD_NOTA_ACCREDITO_AL_CLIENTE 

                    Case LAVCOD_FATTURA_EMESSA
                        Return "Pagamento Fattura Emessa"

                    Case LAVCOD_FATTURA_RICEVUTA
                        Return "Pagamento Fattura Ricevuta"

                    Case LAVCOD_RICEVUTA_EMESSA
                        Return "Pagamento Ricevuta Fiscale Emessa"

                    Case LAVCOD_ALTRI_RICAVI
                        Return "Pagamento Altri Ricavi"

                    Case LAVCOD_ALTRI_COSTI
                        Return "Pagamento Altri Costi"

                    Case LAVCOD_VENDITA
                        Return "Pagamento Vendite"

                    Case LAVCOD_ACQUISTO
                        Return "Pagamento Acquisti"

                    Case LAVCOD_REG_COMPENSI
                        Return "Pagamento Corrispettivi"

                End Select

                '---------------------------------------------

            Case enum_TipoMovimentoContabile.MovFinanziari

                Select Case Lav_Cod

                    Case LAVCOD_MOV_FINANZIARIO


                End Select

                '---------------------------------------------

            Case enum_TipoMovimentoContabile.BolleDDT

                Select Case Lav_Cod

                    Case LAVCOD_DDT_CONTABILIZZATO_EMESSO
                        Return "D.d.T. Contabilizzato Emesso"

                    Case LAVCOD_BOLLA_EMESSA
                        Return "D.d.T. Emesso"

                    Case LAVCOD_BOLLA_RICEVUTA
                        Return "D.d.T. Ricevuto"

                    Case LAVCOD_CONFERIMENTO
                        Return "Bolla di Conferimento a Soci"

                    Case LAVCOD_CONFERIMENTO_DIVERSI
                        Return "Bolla di Conferimento a Diversi"

                    Case LAVCOD_ACCETTAZIONE
                        Return "Bolla di Accettazione "

                    Case LAVCOD_ACCETTAZIONE_DIVERSI
                        Return "Bolla di Accettazione da Diversi"

                End Select

            Case Else
                Return ""

        End Select

    End Function



    '###############################################################################
    Public Function Scadenza_from_LavCod(ByVal Lav_Cod As Integer, ByVal Scadenza As String, ByVal Scadenza_Extra As String) As String

        If Scadenza = "31/12/2100" Or Scadenza = "01/01/1900" Then
            Scadenza = ""
        End If

        If Scadenza_Extra = "31/12/2100" Or Scadenza_Extra = "01/01/1900" Then
            Scadenza_Extra = ""
        End If

        Select Case Lav_Cod

            Case LAVCOD_FATTURA_EMESSA
                Return Scadenza

            Case LAVCOD_FATTURA_RICEVUTA
                Return Scadenza

            Case LAVCOD_RICEVUTA_EMESSA
                Return Scadenza

            Case LAVCOD_ALTRI_RICAVI
                Return ""

            Case LAVCOD_ALTRI_COSTI
                Return ""

            Case LAVCOD_VENDITA
                Return ""

            Case LAVCOD_ACQUISTO
                Return ""

            Case LAVCOD_REG_COMPENSI
                Return Scadenza_Extra

            Case Else
                Return ""

        End Select

    End Function




    '###############################################################################
    'Se si cambia questa caricacombo, 
    'adeguare la corrispondente TipoOrdinamentoQueryContabilita_from_Cod
    'La query di riferimento è:
    'NewCom_Contabilita_Movimenti_Dettagli_Leggi
    Public Sub CaricaCombo_TipoOrdinamentoQueryContabilita(ByRef objServer As System.Web.HttpServerUtility, _
                                                            ByRef objSession As System.Web.SessionState.HttpSessionState, _
                                                            ByRef objPage As System.Web.UI.Page, _
                                                            ByRef Cmb As System.Web.UI.WebControls.DropDownList)

        Cmb.Items.Clear()

        Cmb.Items.Add(New ListItem("Data Decrescente", 0))
        Cmb.Items.Add(New ListItem("Data Crescente", 1))

        Cmb.Items.Add(New ListItem("Num. Doc. Decrescente", 2))
        Cmb.Items.Add(New ListItem("Num. Doc. Crescente", 3))

        Cmb.Items.Add(New ListItem("Causale Decrescente", 4))
        Cmb.Items.Add(New ListItem("Causale Crescente", 5))

        Cmb.Items.Add(New ListItem("Descrizione Decrescente", 6))
        Cmb.Items.Add(New ListItem("Descrizione Crescente", 7))

        Cmb.Items.Add(New ListItem("Scadenza Decrescente", 8))
        Cmb.Items.Add(New ListItem("Scadenza Crescente", 9))

        Cmb.Items.Add(New ListItem("Importo Pagato Decrescente", 10))
        Cmb.Items.Add(New ListItem("Importo Pagato Crescente", 11))

        Cmb.Items.Add(New ListItem("Piva/C.F Decrescente", 12))
        Cmb.Items.Add(New ListItem("Piva/C.F Crescente", 13))

        Cmb.Items.Add(New ListItem("Contatto Decrescente", 14))
        Cmb.Items.Add(New ListItem("Contatto Crescente", 15))

        Cmb.Items.Add(New ListItem("ID Ricl Decrescente", 16))
        Cmb.Items.Add(New ListItem("ID Ricl Crescente", 17))

        Cmb.Items.Add(New ListItem("Conto Decrescente", 18))
        Cmb.Items.Add(New ListItem("Conto Crescente", 19))

        Cmb.Items.Add(New ListItem("Quantità Decrescente", 28))
        Cmb.Items.Add(New ListItem("Quantità Crescente", 29))

        Cmb.Items.Add(New ListItem("Prezzo Unitario Decrescente", 30))
        Cmb.Items.Add(New ListItem("Prezzo Unitario Crescente", 31))

        Cmb.Items.Add(New ListItem("Prezzo Unitario Netto Decrescente", 32))
        Cmb.Items.Add(New ListItem("Prezzo Unitario Netto Crescente", 33))

        Cmb.Items.Add(New ListItem("Sconto Decrescente", 34))
        Cmb.Items.Add(New ListItem("Sconto Crescente", 35))

        Cmb.Items.Add(New ListItem("Imponibile Decrescente", 20))
        Cmb.Items.Add(New ListItem("Imponibile Crescente", 21))

        'Cmb.Items.Add(New ListItem("Aliquota Decrescente", 22))
        'Cmb.Items.Add(New ListItem("Aliquota Crescente", 23))

        Cmb.Items.Add(New ListItem("IVA Decrescente", 24))
        Cmb.Items.Add(New ListItem("IVA Crescente", 25))

        Cmb.Items.Add(New ListItem("Importo Decrescente", 26))
        Cmb.Items.Add(New ListItem("Importo Crescente", 27))

        Cmb.Items.Add(New ListItem("Data Blocco Decrescente", 36))
        Cmb.Items.Add(New ListItem("Data Blocco Crescente", 37))

        If CInt(objSession("ASG_ProgressivoGIAS")) = enum_CodiceGIAS_Clienti.Fruttagel Then
            Cmb.Items.Add(New ListItem("Stato Export Decrescente", 38))
            Cmb.Items.Add(New ListItem("Stato Export Crescente", 39))
        End If

    End Sub

    '###############################################################################
    'questa funzione è collegata a:
    'CaricaCombo_TipoCausale
    'TipoCausale_from_LavCod_e_Tipo
    'utilizza: enum_TipoMovimentoContabile
    Public Sub CaricaCombo_TipoMovimentoContabile(ByRef Cmb As System.Web.UI.WebControls.DropDownList, _
                                                    Optional ByVal Flag_VisualizzaDocTrasporto As Boolean = False)

        Cmb.Items.Clear()

        'Cmb.Items.Add(New ListItem(" ", ""))

        Cmb.Items.Add(New ListItem("Mov Economici e Pagamenti", enum_TipoMovimentoContabile.MovEconomiciPagamenti))
        Cmb.Items.Add(New ListItem("Movimenti Economici", enum_TipoMovimentoContabile.MovEconomici))
        Cmb.Items.Add(New ListItem("Pagamenti", enum_TipoMovimentoContabile.Pagamenti))
        'Cmb.Items.Add(New ListItem("Movimenti Finanziari", enum_TipoMovimentoContabile.MovFinanziari))
        If Flag_VisualizzaDocTrasporto = True Then
            Cmb.Items.Add(New ListItem("Bolle e DDT", enum_TipoMovimentoContabile.BolleDDT))
        End If

    End Sub


    '##########################################################################################
    Public Sub CaricaCombo_RapportiContabili_Manuale(ByRef Cmb As System.Web.UI.WebControls.DropDownList)

        'movimenti che riguardano clienti o fornitori 
        'o clienti e fornitori 
        'o dipendenti o terzisti
        'o dipendenti e terzisti 
        'o tutti;

        Cmb.Items.Clear()
        Cmb.Items.Add(New ListItem("Tutti", 0))
        Cmb.Items.Add(New ListItem("Clienti", COD_CLIENTE))
        Cmb.Items.Add(New ListItem("Fornitori", COD_FORNITORE))
        Cmb.Items.Add(New ListItem("Clienti e Fornitori", COD_CLIENTE_FORNITORE))
        Cmb.Items.Add(New ListItem("Dipendenti", COD_DIPENDENTE))
        Cmb.Items.Add(New ListItem("Terzisti", COD_TERZISTA))
        Cmb.Items.Add(New ListItem("Dipendenti e Terzisti", COD_DIPENDENTE_TERZISTA))


    End Sub


    '###############################################################################
    'questa funzione è collegata a:
    'CaricaCombo_TipoMovimentoContabile
    'TipoCausale_from_LavCod_e_Tipo
    Public Sub CaricaCombo_TipoCausale(ByRef Cmb As System.Web.UI.WebControls.DropDownList, _
                                        ByVal TipoMovimentoContabile As Integer)

        Cmb.Items.Clear()

        Cmb.Items.Add(New ListItem("", "-1"))

        Select Case TipoMovimentoContabile

            Case enum_TipoMovimentoContabile.MovEconomiciPagamenti

                Cmb.Items.Add(New ListItem("Fatture Ricevute", LAVCOD_FATTURA_RICEVUTA))
                Cmb.Items.Add(New ListItem("Fatture Emesse", LAVCOD_FATTURA_EMESSA))
                Cmb.Items.Add(New ListItem("Ricevute Fiscali Emesse", LAVCOD_RICEVUTA_EMESSA))
                Cmb.Items.Add(New ListItem("Acquisti", LAVCOD_ACQUISTO))
                Cmb.Items.Add(New ListItem("Vendite", LAVCOD_VENDITA))
                Cmb.Items.Add(New ListItem("Altri Costi", LAVCOD_ALTRI_COSTI))
                Cmb.Items.Add(New ListItem("Altri Ricavi", LAVCOD_ALTRI_RICAVI))
                Cmb.Items.Add(New ListItem("Corrispettivi", LAVCOD_REG_COMPENSI))

            Case enum_TipoMovimentoContabile.MovEconomici, enum_TipoMovimentoContabile.Pagamenti

                Cmb.Items.Add(New ListItem("Fatture Ricevute", LAVCOD_FATTURA_RICEVUTA))
                Cmb.Items.Add(New ListItem("Fatture Emesse", LAVCOD_FATTURA_EMESSA))
                Cmb.Items.Add(New ListItem("Ricevute Fiscali Emesse", LAVCOD_RICEVUTA_EMESSA))
                Cmb.Items.Add(New ListItem("Acquisti", LAVCOD_ACQUISTO))
                Cmb.Items.Add(New ListItem("Vendite", LAVCOD_VENDITA))
                Cmb.Items.Add(New ListItem("Altri Costi", LAVCOD_ALTRI_COSTI))
                Cmb.Items.Add(New ListItem("Altri Ricavi", LAVCOD_ALTRI_RICAVI))
                Cmb.Items.Add(New ListItem("Corrispettivi", LAVCOD_REG_COMPENSI))

            Case enum_TipoMovimentoContabile.MovFinanziari

            Case enum_TipoMovimentoContabile.BolleDDT

                Cmb.Items.Add(New ListItem("D.D.T. Ricevuto", LAVCOD_BOLLA_RICEVUTA))
                Cmb.Items.Add(New ListItem("D.D.T. Emesso", LAVCOD_BOLLA_EMESSA))
                Cmb.Items.Add(New ListItem("D.D.T. Contabilizzato Emesso", LAVCOD_DDT_CONTABILIZZATO_EMESSO))
                Cmb.Items.Add(New ListItem("Bolla di Conferimento a Soci", LAVCOD_CONFERIMENTO))
                Cmb.Items.Add(New ListItem("Bolla di Conferimento a Diversi", LAVCOD_CONFERIMENTO_DIVERSI))
                Cmb.Items.Add(New ListItem("Bolla di Accettazione", LAVCOD_ACCETTAZIONE))
                Cmb.Items.Add(New ListItem("Bolla di Accettazione da Diversi", LAVCOD_ACCETTAZIONE_DIVERSI))


        End Select


        'Me.cmb_Operazioni.Items.Add(New ListItem( "Note di Accredito Ricevute",LAVCOD_NOTA_ACCREDITO_DAL_FORNITORE))
        'Me.cmb_Operazioni.Items.Add(New ListItem( "Note di Accredito Emesse",LAVCOD_NOTA_ACCREDITO_AL_CLIENTE))


    End Sub






    ''################################################################################
    '===========================================
    ' Decodifica della descrizione del prodotto 
    '-------------------------------------------
    Public Function LeggiProdotto(ByRef objServer As System.Web.HttpServerUtility, _
                                            ByRef objSession As System.Web.SessionState.HttpSessionState, _
                                            ByRef objPage As System.Web.UI.Page, _
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                            ByRef Nome_Categoria As String, _
                                            ByVal Piva As String, _
                                            ByVal Elem_Cod As Integer, _
                                            ByVal Pro_Cod As Integer, _
                                            ByVal Mat_Cod As Integer, _
                                            Optional ByVal Cod_Progetto As Integer = 0, _
                                            Optional ByVal Fase_Cod As Integer = 0, _
                                            Optional ByVal Lotto As String = "", _
                                            Optional ByVal Cal_Cod As Integer = 0, _
                                            Optional ByVal Flag_VisualizzaCategoria As Boolean = False, _
                                            Optional ByVal FinestraTemp_Inizio As Date = AGRODATAINIZIO, _
                                            Optional ByVal FinestraTemp_Fine As Date = AGRODATAFINE, _
                                            Optional ByVal Flag_FiltraFormulati As Boolean = True) As String


        Dim RsProdotto As DataTable
        'Dim Tipo_Des As String = ""
        Dim Nome_Prodotto As String = ""

        '=======================================================
        'Decodifica del tipo prodotto e lettura della descrizione
        '-------------------------------------------------------

        Select Case Elem_Cod

            Case MACCHINE '--------------------- Parco Macchine

                Dim RsMacchina As DataTable

                Dim ParcoMacchine As New AgronicaCoreContabDAL.Parco_Macchine_R

                RsProdotto = ParcoMacchine.Leggi(CStr(Piva), _
                                            CInt(Mat_Cod), _
                                            False, _
                                           "", _
                                           "", _
                                           "", _
                                           "", _
                                           "", _
                                           0, _
                                           "", _
                                           False, _
                                           0, _
                                           "", _
                                           False, _
                                           FinestraTemp_Inizio, _
                                           FinestraTemp_Fine, _
                                           "", _
                                           "", _
                                           objParametri)

                If Not IsNothing(RsProdotto) AndAlso RsProdotto.Rows.Count <> 0 Then

                    Nome_Prodotto = RsProdotto.Rows(0).Item("CLASS_DESC")
                Else
                    'Eccezione
                    Return ""

                End If

                RsMacchina = Nothing

                '================================================================================


            Case ZOO_CONSISTENZA '-------------- ZOO ANIMALI

                If Cod_Progetto <> 0 Then

                    Dim RsConsistenza As DataTable

                    'Lettura Consistenza
                    'RsConsistenza = LeggiAnimale_Anagrafe(objServer, objSession, objPage, Piva, Cod_Progetto)
                    Dim zoo As New AgronicaCoreZooDAL.Zoo_Animali_R
                    RsConsistenza = zoo.Leggi(Piva,
                                              0,
                                              Cod_Progetto,
                                              "",
                                              0,
                                              0,
                                              0,
                                              0,
                                              "",
                                              "",
                                              AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                              "",
                                              "",
                                              objParametri)

                    If Not IsNothing(RsConsistenza) AndAlso RsConsistenza.Rows.Count <> 0 Then

                        Dim RsIPro As DataTable
                        Dim zooprod As New AgronicaCoreZooDAL.Lista_Indprod_Animali_R
                        RsIPro = zooprod.Leggi(CLng(RsConsistenza.Rows(0).Item("Gen_Cod")),
                                               CLng(RsConsistenza.Rows(0).Item("Spe_Cod")),
                                               CLng(RsConsistenza.Rows(0).Item("Ipro_Cod")),
                                               AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                               "",
                                               "", objParametri)
                        
                        If Not IsNothing(RsIPro) AndAlso RsIPro.Rows.Count <> 0 Then

                            Nome_Prodotto = "Consistenze Zootecniche: " & RsIPro.Rows(0).Item("IPro_Des") & " - Matricola: " & RsConsistenza.Rows(0).Item("Matricola")

                        End If

                    End If

                End If

                '================================================================================

            Case Else '-------------------- Altro Tipo di Prodotto


                'Dim objCategorie As Object
                'Dim RsCategorie As ADODB.Recordset
                Dim NomeTabella, NomeCodice, NomeDescrizione As String

                Dim Dt_Categorie As DataTable
                Dim Dt_Prodotti As DataTable
                Dim Dt_Materie_Prime As DataTable
                Dim i As Integer

                'legge le categorie di magazzino
                Dim leggiCategorieMagazzino As New AgronicaCoreMetaSchemaDAL.Categorie_Magazzino_R
                Dt_Categorie = leggiCategorieMagazzino.Leggi(Elem_Cod,
                                                             CAU_MAGAZZINO,
                                                             False,
                                                             "",
                                                             "",
                                                             objParametri)

                If Not IsNothing(Dt_Categorie) Then

                    If Dt_Categorie.Rows.Count <> 0 Then

                        NomeTabella = Dt_Categorie.Rows(i).Item("Tabella")
                        NomeCodice = Dt_Categorie.Rows(i).Item("Tabella_Cod")
                        NomeDescrizione = Dt_Categorie.Rows(i).Item("Tabella_Des")
                        Nome_Categoria = Dt_Categorie.Rows(i).Item("NomeComune")


                        '=================================================================
                        'Verifico se il prodotto è Generale o Aziendale
                        '-----------------------------------------------------------------
                        If Pro_Cod <> 0 Then

                            'Leggo i prodotti con pro_cod

                            Dt_Prodotti = NewCom_LeggiTabella_da_CategorieMagazzino(objParametri, objSession, objPage, _
                                                                                    NomeTabella, _
                                                                                    NomeCodice, _
                                                                                    NomeDescrizione, _
                                                                                    "", _
                                                                                    Pro_Cod, _
                                                                                    "")

                            If Not IsNothing(Dt_Prodotti) Then

                                '(03/08/2015) commentato perchè i dati delle revoca etc non sono piu in locale
                                'Dim objFiltraFormulati As New AgronicaCoreMetaSchemaDAL.Formulati_R
                                'If Elem_Cod = FORMULATI And Flag_FiltraFormulati = True Then
                                '    Dt_Prodotti = objFiltraFormulati.Filtra_Formulati(Dt_Prodotti, CDate(FinestraTemp_Fine), objParametri)
                                'End If

                                If Not IsNothing(Dt_Prodotti) Then
                                    If Dt_Prodotti.Rows.Count <> 0 Then
                                        'Nome_Prodotto = Dt_Prodotti.Rows(j).Item(NomeDescrizione)
                                        Nome_Prodotto = Dt_Prodotti.Rows(0).Item(NomeDescrizione)
                                    Else
                                        '0 prodotti
                                        Return ""
                                    End If
                                Else
                                    'dt prodotti nothing
                                    Return ""
                                End If
                            Else
                                'dt prodotti nothing
                                Return ""
                            End If

                        Else ' Mat_Cod <> 0

                            Dim materiePrimeLetti As New AgronicaCoreAnagrafeDAL.Materie_Prime_R


                            'TODO
                            Dt_Materie_Prime = materiePrimeLetti.Leggi(Piva, _
                                0, _
                                Elem_Cod, _
                                Mat_Cod, _
                                "", _
                                0, _
                                0, _
                                0, _
                                0, _
                                0, _
                                0, _
                                0, _
                                "", _
                                0, _
                                "", _
                                False, _
                                False, _
                                "", _
                                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                "", _
                                "", _
                                objParametri)

                            'Leggo l'anagrafica materie prime
                            'Dt_Materie_Prime = NewCom_MateriePrime_Anagrafica_Leggi( _
                            '                                            objServer, _
                            '                                            objSession, _
                            '                                            objPage, _
                            '                                            Piva, _
                            '                                            0, _
                            '                                            Elem_Cod, _
                            '                                            Mat_Cod, _
                            '                                            , _
                            '                                            , _
                            '                                            , _
                            '                                            , _
                            '                                            , _
                            '                                            , _
                            '                                            , _
                            '                                            , _
                            '                                            FinestraTemp_Inizio, _
                            '                                            FinestraTemp_Fine, _
                            '                                            "", _
                            '                                            , , )


                            If Not IsNothing(Dt_Materie_Prime) Then

                                If Dt_Materie_Prime.Rows.Count <> 0 Then

                                    Select Case Elem_Cod
                                        Case SEMILAVORATI_VEGETALI, SEMILAVORATI_ANIMALI, SEMENTI, ALTRE_MATERIE
                                            Nome_Prodotto = CStr(Dt_Materie_Prime.Rows(i).Item("Mat_Des")) & _
                                            " (Cod.Articolo: " & CStr(Dt_Materie_Prime.Rows(i).Item("Cod_Articolo")) + ")"
                                        Case Else
                                            Nome_Prodotto = CStr(Dt_Materie_Prime.Rows(i).Item("Mat_Des"))
                                    End Select

                                    'Lotto Interno
                                    If Cod_Progetto <> 0 Then

                                        Nome_Prodotto += " - Lotto Impianto: " & Leggi_NomeProgetto(objParametri, _
                                                                                                Piva, _
                                                                                                Cod_Progetto, _
                                                                                                CAU_PROGETTO_PRODUZIONE)

                                    End If

                                    'Lotto Accettazione
                                    If Lotto <> "" And Lotto <> "-1" And Lotto.ToLower <> "indefinito" Then
                                        Nome_Prodotto += " - Lotto Mgazzino: " & Lotto
                                    End If

                                    'Calibro
                                    If Cal_Cod <> 0 Then
                                        Nome_Prodotto += " - Campionatura: " & Leggi_CampionaturaRaccolto(objParametri, objSession, objPage, _
                                                                                            Cal_Cod, "", 0, 0, _
                                                                                            0, "", True)
                                    End If

                                Else
                                    '0 materie prime
                                    Return ""
                                End If
                            Else
                                'Dt_Materie_Prime nothing
                                Return ""
                            End If
                        End If
                    Else
                        '0 categorie
                        Return ""
                    End If
                Else
                    'dt categorie nothing
                    Return ""
                End If

        End Select


        If Flag_VisualizzaCategoria = True Then
            Return Nome_Categoria & ": " & Nome_Prodotto
        Else
            Return Nome_Prodotto
        End If

    End Function


    '################################################################################
    Public Sub Stampa_Documento(ByRef objServer As System.Web.HttpServerUtility, _
                                ByRef objParametri_server As AgronicaCoreParametri, _
                                    ByRef objSession As System.Web.SessionState.HttpSessionState, _
                                    ByRef objPage As System.Web.UI.Page, _
                                    ByVal Lav_Cod As Integer, _
                                    ByVal Piva As String, _
                                    ByVal Id_Agenda As Integer, _
                                    ByVal PathToRoot As String, _
                                    Optional ByRef UpdatePanel As UpdatePanel = Nothing)


        '####################################################################
        '#####  Costruisco la stringa xml e lancio la stampa  ###############
        '####################################################################

        Dim Report As Integer

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim Xml_FiltroStampa As System.Xml.XmlElement

        Dim StrVariabiliStampe As String = ""
        Dim StrNodiVariabili As String = ""
        Dim StrNodo As String = ""

        Dim LinkPaginaStampa As String = "../GestioneStampe/ChiamaStampe.aspx"
        Dim LinkAgronicaStampe As String = ConfigurationManager.AppSettings("LinkAgronicaStampe")
        Dim LinkSitoStampe As String = LinkAgronicaStampe


        Select Case Lav_Cod

            Case LAVCOD_BOLLA_RICEVUTA, LAVCOD_FATTURA_RICEVUTA, LAVCOD_NOTA_ACCREDITO_RICEVUTA, LAVCOD_DOCO_RICEVUTO
                'Messaggi.AgroMsgBox("Non è possibile stampare i documenti contabili ricevuti!", objPage)
                Exit Sub

            Case LAVCOD_DDT_CONTABILIZZATO_EMESSO
                Report = enum_CodificaStampe.DDT_Contabilizzato_Emesso

            Case LAVCOD_BOLLA_EMESSA
                Report = enum_CodificaStampe.Bolle

            Case LAVCOD_FATTURA_EMESSA, LAVCOD_FATTURA_LIQ_CONF_EMESSA, LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA
                Report = enum_CodificaStampe.Fatture

            Case LAVCOD_NOTA_ACCREDITO_EMESSA
                Report = enum_CodificaStampe.Nota_Accredito

            Case LAVCOD_RICEVUTA_EMESSA
                Report = enum_CodificaStampe.RicevuteFiscali

            Case LAVCOD_DOCO_EMESSO
                Report = enum_CodificaStampe.DOCO

            Case LAVCOD_DAA_EMESSO
                Report = enum_CodificaStampe.DAA

            Case LAVCOD_CONFERIMENTO
                Report = enum_CodificaStampe.Bolle_Conferimento_Soci

            Case LAVCOD_CONFERIMENTO_DIVERSI
                Report = enum_CodificaStampe.Bolle_Conferimento_Diversi

            Case LAVCOD_ACCETTAZIONE_DIVERSI
                Report = enum_CodificaStampe.Buono_Accettazione_Diversi

            Case Else
                'AgroMsgBox("Tipo di stampa momentaneamente in manutenzione.", objPage)
                Exit Sub

        End Select


        'Cerco i movimenti selezionati

        Dim vVarStampe(4) As ElementoStampe

        vVarStampe(0).Nome = "piva"
        vVarStampe(0).Valore = Piva

        vVarStampe(1).Nome = "id_agenda"
        vVarStampe(1).Valore = Id_Agenda

        vVarStampe(2).Nome = "printcode"
        vVarStampe(2).Valore = Lav_Cod

        vVarStampe(3).Nome = "printtoprinter"
        vVarStampe(3).Valore = 0

        vVarStampe(4).Nome = "printname"
        vVarStampe(4).Valore = ""


        StrNodo = XML_VariabiliStampe(vVarStampe)

        'Inserisco l'XML nella stringa complessiva
        StrNodiVariabili = StrNodiVariabili & StrNodo

        'Creo il nodo "FiltroStampa"
        Xml_FiltroStampa = XmlDoc.CreateElement("FiltroStampa")

        Xml_FiltroStampa.SetAttribute("report", Report)
        Xml_FiltroStampa.SetAttribute("username", CStr(objSession("ASG_Utente_Username")))
        Xml_FiltroStampa.SetAttribute("user_profilo", CStr(objSession("ASG_SuperUser_CodFiscale")))

        'Rendo l'albero figlio del documento
        XmlDoc.AppendChild(Xml_FiltroStampa)

        'Inserisco gli elementi "VariabiliStampe" come figli del nodo "FiltroStampa"
        Xml_FiltroStampa.InnerXml = StrNodiVariabili

        'Estraggo la stringa XML complessiva
        StrVariabiliStampe = XmlDoc.InnerXml


        If StrVariabiliStampe.IndexOf("Errore") < 0 Then

            objSession("VariabiliStampe") = StrVariabiliStampe


            '##############################################################
            '#####  Apertura pagina di stampa  ############################
            '##############################################################


            Dim strOpen As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoStampe_PassandoDirettamenteIParametri( _
                                            Enum_SiteRedirector.Sito_GiasOnline, _
                                            Report, _
                                            CStr(objSession("ASG_Utente_Username")), _
                                            CStr(objSession("ASG_SuperUser_CodFiscale")), _
                                            StrNodiVariabili, _
                                            "", _
                                            "", _
                                            "", _
                                            "", _
                                            "", _
                                            "", _
                                            "")

            If Not objPage.FindControl("FORM1") Is Nothing Then
                objPage.FindControl("FORM1").Controls.Add(New LiteralControl(strOpen))
            Else
                If UpdatePanel Is Nothing Then
                    objPage.FindControl("aspnetForm").Controls.Add(New LiteralControl(strOpen))
                Else
                    ScriptManager.RegisterClientScriptBlock(UpdatePanel, UpdatePanel.GetType(),
                                                        String.Format("jQuery_{0}", UpdatePanel.ClientID), strOpen, False)
                End If

            End If

        End If



    End Sub



    '###############################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' duplicata in agronicacoreanagrafe imprese_r VerificaEsistenza_PivaGIAS
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function VerificaEsistenza_PivaGIAS(ByRef objParametri_Server As AgronicaCoreParametri, ByVal Piva As String) As Boolean

        Dim DT As DataTable
        Dim Imprese_Leggi As New AgronicaCoreAnagrafeDAL.Imprese_Read
        DT = Imprese_Leggi.Leggi(Piva, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)

        If Not IsNothing(DT) Then
            If DT.Rows.Count <> 0 Then
                Return True
            Else
                Return False
            End If
        Else
            Return False
        End If


        'Else

        '    Throw New Exception("Modulo AccessoVeloceDB : " & DescrizioneFunzione & " : " & MessaggioErrore)
        '    Return Nothing

        'End If


    End Function


    '###############################################################################
    'In base a Tipo_Value_Combo viene costruito il value della combo
    'Tipo_Value_Combo=0 -> Cod_RisUm | Piva (default)
    'Tipo_Value_Combo=1 -> Cod_RisUm
    'Tipo_Value_Combo=2 -> Cod_RisUm | Piva | Cod_Contatto
    Public Sub CaricaCombo_Contatti(ByRef objParametri_Server As AgronicaCoreParametri, _
                                    ByRef objSession As System.Web.SessionState.HttpSessionState, _
                                    ByRef objPage As System.Web.UI.Page, _
                                    ByRef Cmb As System.Web.UI.WebControls.DropDownList, _
                                    ByVal Piva As String, _
                                    ByVal Piva_SuperUser As String, _
                                    Optional ByVal Cliente As Integer = 0, _
                                    Optional ByVal Fornitore As Integer = 0, _
                                    Optional ByVal Dipendente As Integer = 0, _
                                    Optional ByVal Terzista As Integer = 0, _
                                    Optional ByVal Legale As Integer = 0, _
                                    Optional ByVal FiltroAggiuntivo As String = "", _
                                    Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO, _
                                    Optional ByVal Validita_Fine As Date = AGRODATAFINE, _
                                    Optional ByVal Flag_AncheImportati As Boolean = True, _
                                    Optional ByVal Flag_VisualizzaSeStesso As Boolean = True, _
                                    Optional ByVal Cod_Rapporto As Integer = 0, _
                                    Optional ByVal Tipo_Value_Combo As Integer = 0)


        Dim Dt_Contatti As DataTable
        Dim i As Integer
        Dim Rag_Soc As String


        Dim leggiContatto As New AgronicaCoreAnagrafeDAL.Contatti_R
        Dt_Contatti = leggiContatto.Leggi(Piva, _
                                    "", _
                                    0, _
                                    Cod_Rapporto, _
                                    True, _
                                    False, _
                                    0, _
                                    0, _
                                    0, _
                                    0, _
                                    ID_CF_NOFILTRO, _
                                    0, _
                                    "", _
                                    Flag_AncheImportati, _
                                    Cliente, _
                                    Fornitore, _
                                    Dipendente, _
                                    Terzista, _
                                    Legale, _
                                    Validita_Inizio, _
                                    Validita_Fine, _
                                    False,
                                    FiltroAggiuntivo, _
                                    "",
                                    objParametri_Server)


        'Pulisco la combo
        Cmb.Items.Clear()

        'Inserisco una riga vuota
        Cmb.Items.Add(New ListItem("", ""))

        For i = 0 To Dt_Contatti.Rows.Count - 1

            Rag_Soc = CStr(Dt_Contatti.Rows(i).Item("Rag_Soc")) + _
                      CStr(Dt_Contatti.Rows(i).Item("Cognome")) + " " + _
                          CStr(Dt_Contatti.Rows(i).Item("Nome"))

            Select Case Tipo_Value_Combo

                Case 0
                    Cmb.Items.Add(New ListItem(Rag_Soc & " (" & Dt_Contatti.Rows(i).Item("Rapporto_Des") & ")", _
                                                Dt_Contatti.Rows(i).Item("Cod_RisUm") & "|" & Dt_Contatti.Rows(i).Item("Piva")))

                Case 1
                    Cmb.Items.Add(New ListItem(Rag_Soc & " (" & Dt_Contatti.Rows(i).Item("Rapporto_Des") & ")", _
                                                Dt_Contatti.Rows(i).Item("Cod_RisUm")))

                Case 2
                    Cmb.Items.Add(New ListItem(Rag_Soc & " (" & Dt_Contatti.Rows(i).Item("Rapporto_Des") & ")", _
                                                Dt_Contatti.Rows(i).Item("Cod_RisUm") & "|" & Dt_Contatti.Rows(i).Item("Piva") & "|" & Dt_Contatti.Rows(i).Item("Cod_Contatto")))

            End Select


        Next

    End Sub

    '########################################################################################################
    Public Sub CaricaListBox_Indirizzo_Contatto(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                ByRef List As System.Web.UI.WebControls.ListBox,
                                                ByRef Cod_Indirizzo As Integer,
                                                ByVal Dt_Contatto As DataTable,
                                                ByVal Piva As String,
                                                ByVal Cod_RisUm As Integer,
                                                ByVal Tipo_Indirizzo As Integer)


        Dim Cod_Contatto As String = ""
        Dim Indirizzo As String = ""

        If IsNothing(Dt_Contatto) Then

            Dim leggiContatto As New AgronicaCoreAnagrafeDAL.Contatti_R

            Dt_Contatto = leggiContatto.Contatti_Contatto_Leggi(Piva,
                                                                "",
                                                                Cod_RisUm,
                                                                0,
                                                                False,
                                                                True, 0,
                                                                Cod_Indirizzo,
                                                                False,
                                                                0,
                                                                ID_CF_NOFILTRO,
                                                                0,
                                                                "",
                                                                True,
                                                                0,
                                                                0,
                                                                0,
                                                                0,
                                                                0,
                                                                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                                "",
                                                                "",
                                                                objParametri_Server)

        End If

        List.Items.Clear()

        'se non è un'impresa GIAS o se è una persona fisica

        If Tipo_Indirizzo <> 0 Then

            Dim i As Integer = 0

            For i = 0 To Dt_Contatto.Rows.Count - 1

                If Dt_Contatto.Rows(i).Item("Tipo_Indirizzo") = Tipo_Indirizzo Then

                    Cod_Indirizzo = Dt_Contatto.Rows(i).Item("Cod_Indirizzo")

                    '----- Indirizzo
                    Indirizzo = Dt_Contatto.Rows(i).Item("ind_des")
                    List.Items.Add(New ListItem(Indirizzo))

                    '----- Frazione
                    Indirizzo = Dt_Contatto.Rows(i).Item("frz_des")
                    List.Items.Add(New ListItem(Indirizzo))

                    '----- CAP Comune Provincia
                    Indirizzo = " " & Dt_Contatto.Rows(i).Item("cap") &
                                "  -  " & Dt_Contatto.Rows(i).Item("com_des") &
                                "  -  " & Dt_Contatto.Rows(i).Item("pro_cod")
                    List.Items.Add(New ListItem(Indirizzo))

                    '----- Stato
                    Indirizzo = Dt_Contatto.Rows(i).Item("stato")
                    List.Items.Add(New ListItem(Indirizzo))

                    '----- Note
                    Indirizzo = "Note : " & Dt_Contatto.Rows(i).Item("note")
                    List.Items.Add(New ListItem(Indirizzo))

                End If

            Next

        End If


        '-----------------------------------------------------------------------

        'Dim RsIndirizzo As ADODB.Recordset
        'Dim Testo As String

        'If VerificaEsistenza_PivaGIAS(objServer, objSession, objPage, Cod_Contatto) Then

        '    'se è un'impresa GIAS

        '    Dim ObjIndirizzoImpresa As Object   'New Agro_Anagrafe_AD.ImpresexIndirizzi_R
        '   *   CreateCANCELLATOObject("Agro_Anagrafe_AD.ImpresexIndirizzi_R")

        '    RsIndirizzo = ObjIndirizzoImpresa.Leggi(CStr(Cod_Contatto), _
        '                                        , _
        '                                        CInt(Tipo_Indirizzo), _
        '                                        , , , , _
        '                                        CStr(objSession("ASG_Connessione_Server").ToString))

        '    ObjIndirizzoImpresa = Nothing



        'Else

        '    'se non è un'impresa GIAS o se è una persona fisica

        '    Dim ObjIndirizzoContatto As Object  'New Agro_Contab_AD.ContattiXIndirizzi_R
        '   *   CreateCANCELLATOObject("Agro_Contab_AD.ContattiXIndirizzi_R")

        '    RsIndirizzo = ObjIndirizzoContatto.Leggi(Piva, CStr(Cod_Contatto), , _
        '                                 CInt(Tipo_Indirizzo), _
        '                                 , , , , _
        '                                 CStr(objSession("ASG_Connessione_Server").ToString))

        '    ObjIndirizzoContatto = Nothing



        'End If


        'If RsIndirizzo.State <> 0 AndAlso Not RsIndirizzo.EOF Then


        '    Txt.Text = RsIndirizzo.Fields("cod_indirizzo").Value
        '    Txt.Visible = False

        '    '----- Indirizzo
        '    Testo = RsIndirizzo.Fields("ind_des").Value
        '    List.Items.Add(New ListItem(Testo))

        '    '----- Frazione
        '    Testo = RsIndirizzo.Fields("frz_des").Value
        '    List.Items.Add(New ListItem(Testo))

        '    '----- CAP Comune Provincia
        '    Testo = " " & RsIndirizzo.Fields("cap").Value & _
        '            "  -  " & RsIndirizzo.Fields("com_des").Value & _
        '            "  -  " & RsIndirizzo.Fields("pro_cod").Value
        '    List.Items.Add(New ListItem(Testo))

        '    '----- Stato
        '    Testo = RsIndirizzo.Fields("stato").Value
        '    List.Items.Add(New ListItem(Testo))

        '    '----- Note
        '    Testo = "Note : " & RsIndirizzo.Fields("note").Value
        '    List.Items.Add(New ListItem(Testo))


        '    'Chiudo il recordset
        '    RsIndirizzo.Close()


        'End If


        ''Elimino il recordset (anche se non necessario in ASP.NET)
        'RsIndirizzo = Nothing


    End Sub

    Public Sub CaricaCombo_TipoIndirizzoContatto(ByRef objServer As System.Web.HttpServerUtility,
                                                 ByRef objSession As System.Web.SessionState.HttpSessionState,
                                                 ByRef objPage As System.Web.UI.Page,
                                                 ByVal Cmb_RisUm As System.Web.UI.WebControls.DropDownList,
                                                 ByRef Cod_Contatto As String,
                                                 ByVal Piva_SuperUser As String,
                                                 ByRef Cmb As System.Web.UI.WebControls.DropDownList,
                                                 ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri)

        If Cmb_RisUm.SelectedIndex < 1 Then

            'se non è stato selezionato niente non faccio niente
            Exit Sub

        End If


        Cmb.Enabled = True

        Cmb.Items.Clear()


        Dim RsRisUm As DataTable


        Dim objRisorse_Umane As New AgronicaCoreAnagrafeDAL.Risorse_Umane_R
        RsRisUm = objRisorse_Umane.LeggiContattixSuperUser(CStr(Piva_SuperUser),
                                                           CStr(Split(Cmb_RisUm.SelectedItem.Value, "|")(1)),
                                                           CInt(Split(Cmb_RisUm.SelectedItem.Value, "|")(0)),
                                                           0, 0, "", True,
                                                           AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta,
                                                           "", "",
                                                           objParametri_Server)

        For Each drow In RsRisUm.Rows

            'carico il cod_contatto in una text_box invisibile
            'mi serve per caricare gli indirizzi
            Cod_Contatto = drow("Cod_Contatto").Value


            'carico gli indirizzi a seconda della persona fisica o giuridica
            If IsNumeric(drow("Cod_Contatto").Value) Then

                'persona giuridica (PIVA)
                Cmb.Items.Add(New ListItem("Sede Operativa", "1"))
                Cmb.Items.Add(New ListItem("Sede Legale", "101"))
                Cmb.Items.Add(New ListItem("Sede Aziendale", "102"))
                Cmb.Items.Add(New ListItem("Stabilimento", "103"))

            Else

                'persona fisica (CF)
                Cmb.Items.Add(New ListItem("Residenza", "3"))
                Cmb.Items.Add(New ListItem("Luogo di Nascita", "5"))
                Cmb.Items.Add(New ListItem("Domicilio", "2"))
                Cmb.Items.Add(New ListItem("Residenza Estiva", "4"))

            End If

        Next

    End Sub


    '########################################################################################################
    Public Sub CaricaListBox_Indirizzo_Impresa(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                               ByRef List As System.Web.UI.WebControls.ListBox,
                                               ByRef Cod_Indirizzo As Integer,
                                               ByVal Dt_Impresa As DataTable,
                                               ByVal Piva As String,
                                               ByVal Tipo_Indirizzo As Integer)

        Dim indirizzo As String = ""

        If IsNothing(Dt_Impresa) Then

            Dim impreseLeggimi As New AgronicaCoreAnagrafeDAL.Imprese_Read

            Dt_Impresa = impreseLeggimi.Leggi_3(Piva,
                                                True,
                                                Tipo_Indirizzo,
                                                Cod_Indirizzo,
                                                False,
                                                False,
                                                False,
                                                False,
                                                False,
                                                False,
                                                False,
                                                False,
                                                False,
                                                False,
                                                "",
                                                "",
                                                objParametri_Server)

        End If

        List.Items.Clear()

        If Tipo_Indirizzo <> 0 Then

            Dim i As Integer = 0

            For i = 0 To Dt_Impresa.Rows.Count - 1

                If Dt_Impresa.Rows(i).Item("Tipo_Indirizzo") = Tipo_Indirizzo Then

                    Cod_Indirizzo = Dt_Impresa.Rows(i).Item("Cod_Indirizzo")

                    '----- Indirizzo
                    indirizzo = Dt_Impresa.Rows(i).Item("ind_des")
                    List.Items.Add(New ListItem(indirizzo))

                    '----- Frazione
                    indirizzo = Dt_Impresa.Rows(i).Item("frz_des")
                    List.Items.Add(New ListItem(indirizzo))

                    '----- CAP Comune Provincia
                    indirizzo = " " & Dt_Impresa.Rows(i).Item("cap") &
                                "  -  " & Dt_Impresa.Rows(i).Item("com_des") &
                                "  -  " & Dt_Impresa.Rows(i).Item("pro_cod")
                    List.Items.Add(New ListItem(indirizzo))

                    '----- Stato
                    indirizzo = Dt_Impresa.Rows(i).Item("stato")
                    List.Items.Add(New ListItem(indirizzo))

                    '----- Note
                    indirizzo = "Note : " & Dt_Impresa.Rows(i).Item("note")
                    List.Items.Add(New ListItem(indirizzo))

                End If

            Next

        End If

    End Sub


    '################################################################################
    Public Function Ricava_MacroQta(ByVal Udm_Cod As Integer, ByVal Qta As Decimal) As Decimal

        'Trova la Macro Quantità
        Select Case Udm_Cod

            Case 2, 29 'Kg, Litri

                Return Qta 'Inalterata

            Case 3, 104, 101 'G, Cc, Ml

                Return Qta / 1000

            Case 4 'Quintali

                Return Qta * 100

            Case 304 'Tonnellate

                Return Qta * 1000

            Case 1003, 93, 92, 38 'Confezioni, Unità di seme, Numero piante, Numero Puro

                Return Qta 'Inalterata

            Case Else

                'Eccezione
                Return 0

        End Select

    End Function

    '################################################################################
    Public Function Ricava_MacroUdm(ByVal Udm_Cod As Integer) As Integer

        'Trova la Macro Unità di Misura
        Select Case Udm_Cod

            Case 2, 3, 4, 304 'Kg, G, Q, T

                Return 2 'Kg

            Case 29, 104, 101 'L, Cc, Ml

                Return 29 'Litri

            Case 1003, 93, 92, 38 'Confezioni/Unità di seme/N. piante/Numero Puro

                Return Udm_Cod 'Inalterata

            Case Else

                'Eccezione
                Return 0

        End Select

    End Function


    '########################################################################################################
    Public Sub CaricaListBox_Indirizzo_2(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                         ByRef objSession As System.Web.SessionState.HttpSessionState,
                                         ByRef objPage As System.Web.UI.Page,
                                         ByRef List As System.Web.UI.WebControls.ListBox,
                                         ByRef Cod_Indirizzo As Integer,
                                         ByVal Flag_Contatto_GIAS As Boolean,
                                         ByVal Dt_Impresa_Contatto As DataTable,
                                         ByVal Piva_Contatto As String,
                                         ByVal Cod_Contatto As String,
                                         ByVal Cod_RisUm As Integer,
                                         Optional ByVal Tipo_Indirizzo As Integer = 0)
        
        List.Items.Clear()

        If Flag_Contatto_GIAS = True Then
            'se è un'impresa GIAS
            CaricaListBox_Indirizzo_Impresa(objParametri_Server, List, Cod_Indirizzo, Dt_Impresa_Contatto, Cod_Contatto, Tipo_Indirizzo)

        Else
            'se non è un'impresa GIAS o se è una persona fisica
            CaricaListBox_Indirizzo_Contatto(objParametri_Server, List, Cod_Indirizzo, Dt_Impresa_Contatto, Piva_Contatto, Cod_RisUm, Tipo_Indirizzo)

        End If

    End Sub



    Public Function TipoIndirizzo_from_IndirizzoContatto2(ByRef objParametri_Server As AgronicaCoreParametri,
                                                          ByRef objSession As System.Web.SessionState.HttpSessionState,
                                                          ByRef objPage As System.Web.UI.Page,
                                                          ByVal Cod_Indirizzo As Integer,
                                                          ByVal Flag_Contatto_GIAS As Boolean,
                                                          ByRef Dt_Impresa_Contatto As DataTable,
                                                          ByVal Piva_Contatto As String,
                                                          ByVal Cod_Contatto As String,
                                                          ByVal Cod_RisUm As Integer) As Integer

        Dim tipoIndirizzo As Integer = 0

        If Flag_Contatto_GIAS = True Then

            'se è un'impresa GIAS
            Dim dtImpresa As DataTable

            'TODO
            Dim Imprese_Leggi As New AgronicaCoreAnagrafeDAL.Imprese_Read
            dtImpresa = Imprese_Leggi.Leggi_3(Cod_Contatto,
                                              True,
                                              0,
                                              Cod_Indirizzo,
                                              False,
                                              False,
                                              False,
                                              False,
                                              False,
                                              False,
                                              False,
                                              False,
                                              False,
                                              False,
                                              "",
                                              "",
                                              objParametri_Server)

            If Not IsNothing(dtImpresa) Then
                If dtImpresa.Rows.Count <> 0 Then
                    tipoIndirizzo = dtImpresa.Rows(0).Item("Tipo_Indirizzo")
                Else
                    tipoIndirizzo = -1
                End If
            Else
                tipoIndirizzo = -1
            End If

            Dt_Impresa_Contatto = dtImpresa

        Else

            'se non è un'impresa GIAS o se è una persona fisica
            tipoIndirizzo = -1

            Dim i As Integer = 0
            Dim dtContatto As DataTable

            'dtContatto = NewCom_Contatti_Contatto_Leggi(objParametri_Server, objSession, objPage,
            '                                            Piva_Contatto,
            '                                            ,
            '                                            Cod_RisUm,
            '                                            , False, , True, , , False, , , , , , , , , , , , , )

            Dim leggiContatto As New AgronicaCoreAnagrafeDAL.Contatti_R

            dtContatto = leggiContatto.Contatti_Contatto_Leggi(Piva_Contatto,
                                                               "",
                                                               Cod_RisUm,
                                                               0,
                                                               False,
                                                               True, 0,
                                                               0,
                                                               False,
                                                               0,
                                                               ID_CF_NOFILTRO,
                                                               0,
                                                               "",
                                                               True,
                                                               0,
                                                               0,
                                                               0,
                                                               0,
                                                               0,
                                                               AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                               "",
                                                               "",
                                                               objParametri_Server)

            For i = 0 To dtContatto.Rows.Count - 1

                If dtContatto.Rows(i).Item("Cod_Indirizzo") = Cod_Indirizzo Then
                    tipoIndirizzo = dtContatto.Rows(i).Item("Tipo_Indirizzo")
                End If

            Next

            Dt_Impresa_Contatto = dtContatto

        End If


        Return tipoIndirizzo


    End Function

    Public Function CUAA_from_PIVA(ByRef objParametri_Server As AgronicaCoreParametri, ByVal Piva As String) As String

        Dim dtImpresa As DataTable

        Dim leggiI As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
        dtImpresa = leggiI.Leggi(Piva,
                                 1010,
                                 AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                 "",
                                 "",
                                 objParametri_Server)

        If Not IsNothing(dtImpresa) Then
            If dtImpresa.Rows.Count <> 0 Then
                Return dtImpresa.Rows(0).Item("val_cod")
            Else
                Return ""
            End If
        Else
            Return ""
        End If

    End Function


    Public Function VerificaNumDocumento(ByRef objParametri_Server As AgronicaCoreParametri,
                                         ByRef objSession As System.Web.SessionState.HttpSessionState,
                                         ByRef objPage As System.Web.UI.Page,
                                         ByVal Piva As String,
                                         ByVal Anno As Integer,
                                         ByVal Lav_Cod As Integer,
                                         ByVal cod_Risum As Integer,
                                         ByVal Doc_Numero_Sin As String,
                                         ByVal Doc_Numero As Decimal,
                                         ByVal Doc_Numero_Des As String,
                                         Optional ByVal Cau_Mov As String = ""
                                         ) As Boolean

        Dim dt As DataTable
        Dim leggimovcont As New AgronicaCoreContabDAL.Movimenti_R

        dt = leggimovcont.MovimentiContabili(Lav_Cod,
                                             Piva,
                                             0,
                                             0,
                                             0,
                                             cod_Risum,
                                             Cau_Mov,
                                             Anno,
                                             AGRODATAINIZIO,
                                             AGRODATAINIZIO,
                                             Doc_Numero_Sin,
                                             Doc_Numero,
                                             Doc_Numero_Des,
                                             0,
                                             0,
                                             AGRODATAINIZIO,
                                             "",
                                             " Movimenti.Doc_Numero DESC ",
                                             objParametri_Server)

        If dt.Rows.Count = 0 Then
            Return False
        Else
            Return True
        End If

    End Function

    'verifica solo il doc_numero e ritorna anche il numero e la data del documento presente
    Public Function VerificaNumDocumento2(ByRef objParametri_Server As AgronicaCoreParametri,
                                         ByRef objSession As System.Web.SessionState.HttpSessionState,
                                         ByRef objPage As System.Web.UI.Page,
                                         ByVal Piva As String,
                                         ByVal Anno As Integer,
                                         ByVal Lav_Cod As Integer,
                                         ByVal cod_Risum As Integer,
                                         ByVal Doc_Numero As Decimal,
                                         ByVal Cau_Mov As String,
                                          ByRef Str_docPresente As String
                                         ) As Boolean

        Dim dt As DataTable
        Dim leggimovcont As New AgronicaCoreContabDAL.Movimenti_R
        Str_docPresente = ""

        dt = leggimovcont.MovimentiContabili(Lav_Cod,
                                             Piva,
                                             0,
                                             0,
                                             0,
                                             cod_Risum,
                                             Cau_Mov,
                                             Anno,
                                             AGRODATAINIZIO,
                                             AGRODATAINIZIO,
                                             "XYZ",
                                             Doc_Numero,
                                             "XYZ",
                                             0,
                                             0,
                                             AGRODATAINIZIO,
                                             "",
                                             " Movimenti.Doc_Numero DESC ",
                                             objParametri_Server)

        If Not IsNothing(dt) Then
            If dt.Rows.Count = 0 Then
                Return False
            Else
                Str_docPresente = "num. " & CStr(dt.Rows(0).Item("Doc_Numero_Sin")) & CStr(dt.Rows(0).Item("Doc_Numero")) & CStr(dt.Rows(0).Item("Doc_Numero_Des")) & " del " & CStr(dt.Rows(0).Item("data_movimento"))
                Return True
            End If
        Else
            Return False
        End If

    End Function

    Public Sub RicavaNumDocumentoEmesso(ByRef objparametri_Server As AgronicaCoreParametri,
                                        ByRef objSession As System.Web.SessionState.HttpSessionState,
                                        ByRef objPage As System.Web.UI.Page,
                                        ByRef Doc_Numero_Sin As String,
                                        ByRef Doc_Numero As Decimal,
                                        ByRef Doc_Numero_Des As String,
                                        ByVal Piva As String,
                                        ByVal Anno As Integer,
                                        ByVal Lav_Cod As Integer,
                                        Optional ByVal Cau_Mov As String = "",
                                        Optional ByVal Cod_RisUm As Integer = 0)


        Dim dt As DataTable
        Dim leggimovcont As New AgronicaCoreContabDAL.Movimenti_R

        dt = leggimovcont.MovimentiContabili(Lav_Cod,
                                             Piva,
                                             0,
                                             0,
                                             0,
                                             Cod_RisUm,
                                             Cau_Mov,
                                             Anno,
                                             AGRODATAINIZIO,
                                             AGRODATAINIZIO,
                                             "",
                                             0,
                                             "",
                                             0,
                                             0,
                                             AGRODATAINIZIO,
                                             "",
                                             " Movimenti.Doc_Numero DESC ",
                                             objparametri_Server)
        
        If dt.Rows.Count <> 0 Then

            Doc_Numero_Sin = dt.Rows(0).Item("Doc_Numero_Sin")
            Doc_Numero = dt.Rows(0).Item("Doc_Numero")
            Doc_Numero_Des = dt.Rows(0).Item("Doc_Numero_Des")

        End If

    End Sub


    Public Sub CaricaCombo_TipoSconto(ByRef objServer As System.Web.HttpServerUtility, _
                                        ByRef objSession As System.Web.SessionState.HttpSessionState, _
                                        ByRef objPage As System.Web.UI.Page, _
                                        ByRef Cmb As System.Web.UI.WebControls.DropDownList)

        Cmb.Items.Clear()

        Cmb.Items.Add(New ListItem("Variazione sul Prezzo Unitario", enum_TipoSconto.PrezzoUnitario))

        Cmb.Items.Add(New ListItem("Variazione sull'Imponibile", enum_TipoSconto.Imponibile))

        Cmb.Items.Add(New ListItem("Variazione a Piede Fattura", enum_TipoSconto.Totale))


    End Sub

    '################################################################################
    Public Function Leggi_NomeProgetto(ByRef objParametri_Server As AgronicaCoreParametri, _
                                        ByVal Piva As String, _
                                        ByVal Progetto_Cod As Integer, _
                                        Optional ByVal CAU_PROGETTO As String = "", _
                                        Optional ByVal Cod_Contratto As Integer = 0, _
                                        Optional ByVal Sa_Cod As Integer = 0, _
                                        Optional ByVal Appezza As Integer = 0, _
                                        Optional ByVal Id_Reg As Integer = 0, _
                                        Optional ByVal Veg_Cod As Integer = 0, _
                                        Optional ByVal Grfi_Cod As Integer = 0) As String

        Dim NomeProgetto As String = ""
        Dim Dt As DataTable

        Dim leggiprogetto As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
        Dt = leggiprogetto.Leggi( _
                Piva, _
                Progetto_Cod, _
                CAU_PROGETTO, _
                Cod_Contratto, _
                Sa_Cod, _
                Appezza, _
                Id_Reg, _
                Veg_Cod, _
                Grfi_Cod, _
                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                "", _
                "", _
                objParametri_Server _
            )

        If Not IsNothing(Dt) Then

            If Dt.Rows.Count <> 0 Then

                NomeProgetto = CStr(Dt.Rows(0).Item("Progetto_Nome"))

            End If

        End If

        Return NomeProgetto


    End Function


    '################################################################################
    Public Function Leggi_CampionaturaRaccolto(ByRef objServer As AgronicaCoreParametri, _
                                                ByRef objSession As System.Web.SessionState.HttpSessionState, _
                                                ByRef objPage As System.Web.UI.Page, _
                                                ByVal Codice As Integer, _
                                                Optional ByVal Tipo As String = "", _
                                                Optional ByVal Tipo_Cod As Long = 0, _
                                                Optional ByVal Udm_Cod As Long = 0, _
                                                Optional ByVal Progressivo_Origine As Integer = 0, _
                                                Optional ByVal Piva_SuperUser_Origine As String = "", _
                                                Optional ByVal Flag_AncheImportati As Boolean = False) As String

        Dim Str_CampionaturaRaccolto As String = ""


        Select Case Codice

            Case Is > 0 'cal_cod

                'Calibro
                Str_CampionaturaRaccolto = Leggi_CalibroFrutto(objServer, objSession, objPage, _
                                                        Codice, _
                                                        , , , )


            Case Is <= 0 ' progressivo

                'Campionatura

                Str_CampionaturaRaccolto = Leggi_MateriaPrima_Campionature(objServer, objSession, objPage, _
                                                                            Codice, _
                                                                            Tipo, _
                                                                            Tipo_Cod, _
                                                                            Udm_Cod, _
                                                                            Progressivo_Origine, _
                                                                            Piva_SuperUser_Origine, _
                                                                            Flag_AncheImportati, _
                                                                            , , , )


        End Select

        Return Str_CampionaturaRaccolto


    End Function

    '################################################################################
    Public Function Leggi_CalibroFrutto(ByRef objServer As AgronicaCoreParametri, _
                                        ByRef objSession As System.Web.SessionState.HttpSessionState, _
                                        ByRef objPage As System.Web.UI.Page, _
                                        ByVal Cal_Cod As Integer, _
                                        Optional ByVal TestoRicerca As String = "", _
                                        Optional ByVal FinestraTemp_Inizio As String = "01/01/1900", _
                                        Optional ByVal FinestraTemp_Fine As String = "31/12/2100", _
                                        Optional ByVal Ordinamento As String = "") As String

        Dim Calibro_Des As String = ""
        Dim Dt As DataTable

        Dim leggiCalibriFrutti As New AgronicaCoreMetaSchemaDAL.CalibriFrutti_R

        Dt = leggiCalibriFrutti.Leggi( _
                                        Cal_Cod, _
                                        AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                        "", _
                                        "", _
                                        objServer _
                                        )

        If Not IsNothing(Dt) Then

            If Dt.Rows.Count <> 0 Then

                Calibro_Des = CStr(Dt.Rows(0).Item("Cal_Des"))

            End If

        End If

        Return Calibro_Des


    End Function



    '################################################################################
    Public Function Leggi_MateriaPrima_Campionature(ByRef objServer As AgronicaCoreParametri, _
                                                    ByRef objSession As System.Web.SessionState.HttpSessionState, _
                                                    ByRef objPage As System.Web.UI.Page, _
                                                    ByVal Progressivo As Integer, _
                                                    Optional ByVal Tipo As String = "", _
                                                    Optional ByVal Tipo_Cod As Integer = 0, _
                                                    Optional ByVal Udm_Cod As Integer = 0, _
                                                    Optional ByVal Progressivo_Origine As Integer = 0, _
                                                    Optional ByVal Piva_SuperUser_Origine As String = "", _
                                                    Optional ByVal Flag_AncheImportati As Boolean = False, _
                                                    Optional ByVal FinestraTemp_Inizio As Date = AGRODATAINIZIO, _
                                                    Optional ByVal FinestraTemp_Fine As Date = AGRODATAFINE, _
                                                    Optional ByVal FiltroAggiuntivo As String = "", _
                                                    Optional ByVal Ordinamento As String = "") As String

        Dim Campionatura_Des As String = ""
        Dim Dt As DataTable
        Dim i As Integer

        Dim campiona As New AgronicaCoreContabDAL.Materie_Prime_Campionature_R

        Dt = campiona.Leggi( _
                Progressivo, _
                Tipo, _
                Tipo_Cod, _
                Udm_Cod, _
                Progressivo_Origine, _
                Piva_SuperUser_Origine, _
                True, _
                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                "", _
                "", _
                objServer _
                )

        If Not IsNothing(Dt) Then

            For i = 0 To Dt.Rows.Count - 1

                Campionatura_Des += IIf(Trim(Campionatura_Des) = "", "", ", ") & CStr(Dt.Rows(i).Item("Descrizione"))

            Next

        End If

        Return Campionatura_Des


    End Function



    '###############################################################################
    'adeguare questa funzione alla CaricaCombo_TipoOrdinamentoQueryContabilita
    'La query di riferimento è:
    'NewCom_Contabilita_Movimenti_Dettagli_Leggi
    Public Function TipoOrdinamentoQueryContabilita_from_Cod(ByVal Cod As Integer) As String


        Select Case Cod

            Case 0
                Return " ORDER BY Movimenti_Contab.Data_Movimento DESC "
            Case 1
                Return " ORDER BY Movimenti_Contab.Data_Movimento ASC "
            Case 2
                Return " ORDER BY Movimenti_Contab.Doc_Numero DESC "
            Case 3
                Return " ORDER BY Movimenti_Contab.Doc_Numero, Movimenti_Contab.Doc_Numero_Sin, Movimenti_Contab.Doc_Numero_Des ASC "
            Case 4
                Return " ORDER BY Lav_Des DESC "
            Case 5
                Return " ORDER BY Lav_Des ASC "
            Case 6
                Return " ORDER BY Movimenti_Contab.Extra_Str DESC "
            Case 7
                Return " ORDER BY Movimenti_Contab.Extra_Str, Movimenti_Contab.Mov_Desc ASC "
            Case 8
                Return " ORDER BY Movimenti_Contab.Scadenza DESC "
            Case 9
                Return " ORDER BY Movimenti_Contab.Scadenza ASC "
            Case 10
                Return " ORDER BY Importo_Pagato DESC "
            Case 11
                Return " ORDER BY Importo_Pagato ASC "
            Case 12
                Return " ORDER BY Cod_Contatto_Contab DESC "
            Case 13
                Return " ORDER BY Cod_Contatto_Contab ASC "
            Case 14
                Return " ORDER BY Rag_Soc_Contab DESC "
            Case 15
                Return " ORDER BY Rag_Soc_Contab ASC "
            Case 16
                Return " ORDER BY Id_Riclassificazione DESC "
            Case 17
                Return " ORDER BY Id_Riclassificazione ASC "
            Case 18
                Return " ORDER BY Conto_Descr DESC "
            Case 19
                Return " ORDER BY Conto_Descr ASC "
            Case 20
                'Return " ORDER BY ABS(Imponibile_Netto) DESC"
                Return " ORDER BY Imponibile_Netto DESC "
            Case 21
                'Return " ORDER BY ABS(Imponibile_Netto) ASC"
                Return " ORDER BY Imponibile_Netto ASC "
            Case 22
                Return " ORDER BY Cod_Iva DESC "
            Case 23
                Return " ORDER BY Cod_Iva ASC "
            Case 24
                'Return " ORDER BY ABS(IVA) DESC"
                Return " ORDER BY IVA DESC "
            Case 25
                'Return " ORDER BY ABS(IVA) ASC"
                Return " ORDER BY IVA ASC "
            Case 26
                'Return " ORDER BY ABS(Movimenti_dettagli.Imponibile_Netto) + ABS(Movimenti_dettagli.Iva) DESC" 'non ho l'importo
                Return " ORDER BY Movimenti_dettagli.Imponibile_Netto + Movimenti_dettagli.Iva DESC " 'non ho l'importo
            Case 27
                'Return " ORDER BY ABS(Movimenti_dettagli.Imponibile_Netto) + ABS(Movimenti_dettagli.Iva) ASC" 'non ho l'importo
                Return " ORDER BY Movimenti_dettagli.Imponibile_Netto + Movimenti_dettagli.Iva ASC " 'non ho l'importo
            Case 28
                Return " ORDER BY Movimenti_dettagli.Qta  DESC "
            Case 29
                Return " ORDER BY Movimenti_dettagli.Qta  ASC "
            Case 30
                Return " ORDER BY Movimenti_dettagli.Prezzo_Unitario  DESC "
            Case 31
                Return " ORDER BY Movimenti_dettagli.Prezzo_Unitario  ASC "
            Case 32
                Return " ORDER BY Movimenti_dettagli.Prezzo_Unitario_Netto DESC "
            Case 33
                Return " ORDER BY Movimenti_dettagli.Prezzo_Unitario_Netto ASC "
            Case 34
                Return " ORDER BY Movimenti_dettagli.Sconto  DESC "
            Case 35
                Return " ORDER BY Movimenti_dettagli.Sconto  ASC "
            Case 36
                Return " ORDER BY Agenda.Data_Blocco DESC "
            Case 37
                Return " ORDER BY Agenda.Data_Blocco ASC "
            Case 38
                Return " ORDER BY Agenda.Stato_Export DESC "
            Case 39
                Return " ORDER BY Agenda.Stato_Export ASC "
            Case Else
                Return ""
        End Select

    End Function


    '################################################################################
    Public Function PaginaContabile_from_LavCod(ByRef objServer As System.Web.HttpServerUtility,
                                                ByVal PathToRoot As String,
                                                ByVal Piva As String,
                                                ByVal Lav_Cod As Integer,
                                                ByVal Id_Agenda As Integer,
                                                ByVal Enum_Pagina_Origine As Integer,
                                                ByVal Rag_Soc As String) As String

        Dim qs As String = ""
        Dim Carico_Scarico As String = ""

        Select Case Lav_Cod

            Case LAVCOD_BOLLA_EMESSA, LAVCOD_BOLLA_RICEVUTA, LAVCOD_DDT_CONTABILIZZATO_EMESSO

                qs = "?p=" & Stringa_Codifica(Piva, AgroKey_EncoderDecoder, objServer) &
                     "&s=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, objServer) &
                     "&d=" & Stringa_Codifica(CStr(Date.Today), AgroKey_EncoderDecoder, objServer) &
                     "&i=" & Stringa_Codifica(Id_Agenda, AgroKey_EncoderDecoder, objServer) &
                     "&o=" & Stringa_Codifica(enum_Security_Operazione.Lettura, AgroKey_EncoderDecoder, objServer) &
                     "&l=" & Stringa_Codifica(Lav_Cod, AgroKey_EncoderDecoder, objServer) &
                     "&orig=" & Stringa_Codifica(Enum_Pagina_Origine, AgroKey_EncoderDecoder, objServer) &
                     "&rs=" & Stringa_Codifica(Rag_Soc, AgroKey_EncoderDecoder, objServer)

                Return PathToRoot & "GestioneContabilita/DocumentiTrasporto/Bolle_New.aspx" & qs

                '-----------------------------------------------

            Case LAVCOD_FATTURA_EMESSA, LAVCOD_FATTURA_RICEVUTA

                qs = "?p=" & Stringa_Codifica(Piva, AgroKey_EncoderDecoder, objServer) &
                     "&s=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, objServer) &
                     "&d=" & Stringa_Codifica(CStr(Date.Today), AgroKey_EncoderDecoder, objServer) &
                     "&i=" & Stringa_Codifica(Id_Agenda, AgroKey_EncoderDecoder, objServer) &
                     "&o=" & Stringa_Codifica(enum_Security_Operazione.Lettura, AgroKey_EncoderDecoder, objServer) &
                     "&l=" & Stringa_Codifica(Lav_Cod, AgroKey_EncoderDecoder, objServer) &
                     "&orig=" & Stringa_Codifica(Enum_Pagina_Origine, AgroKey_EncoderDecoder, objServer) &
                     "&rs=" & Stringa_Codifica(Rag_Soc, AgroKey_EncoderDecoder, objServer) &
                     "&tf=" & Stringa_Codifica(-1, AgroKey_EncoderDecoder, objServer)

                Return PathToRoot & "GestioneContabilita/MovEconomici/Fattura.aspx" & qs

                '-----------------------------------------------

            Case LAVCOD_RICEVUTA_EMESSA

                'Return PathToRoot & "GestioneContabilita/MovEconomici/.aspx"
                Return ""

                '-----------------------------------------------

            Case LAVCOD_ALTRI_RICAVI, LAVCOD_ALTRI_COSTI

                qs = "?o=" & Stringa_Codifica(enum_Security_Operazione.Lettura, AgroKey_EncoderDecoder, objServer) &
                     "&orig=" & Stringa_Codifica(Enum_Pagina_Origine, AgroKey_EncoderDecoder, objServer) &
                     "&p=" & Stringa_Codifica(Piva, AgroKey_EncoderDecoder, objServer) &
                     "&l=" & Stringa_Codifica(Lav_Cod, AgroKey_EncoderDecoder, objServer) &
                     "&d=" & Stringa_Codifica(CStr(Date.Today), AgroKey_EncoderDecoder, objServer) &
                     "&s=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, objServer) &
                     "&i=" & Stringa_Codifica(Id_Agenda, AgroKey_EncoderDecoder, objServer) &
                     "&a=" & Stringa_Codifica(CStr(Date.Today.Year), AgroKey_EncoderDecoder, objServer) &
                     "&rs=" & Stringa_Codifica(Rag_Soc, AgroKey_EncoderDecoder, objServer)

                '-----------------------------------------------

                Return PathToRoot & "GestioneContabilita/MovEconomici/Altri_Costi_Ricavi.aspx" & qs

                '-----------------------------------------------

            Case LAVCOD_MOV_FINANZIARIO

                'Return PathToRoot & "GestioneContabilita/MovFinanziari/.aspx"
                Return ""

                '-----------------------------------------------

            Case LAVCOD_REG_COMPENSI

                'Return PathToRoot & "GestioneContabilita/MovEconomici/.aspx"
                Return ""

                '-----------------------------------------------

            Case LAVCOD_VENDITA, LAVCOD_ACQUISTO

                Dim Chiave As String = ""

                If Lav_Cod = LAVCOD_ACQUISTO Then
                    Carico_Scarico = "C"
                ElseIf Lav_Cod = LAVCOD_VENDITA Then
                    Carico_Scarico = "S"
                End If

                'Genero la chiave
                Albero.ChiaveAlbero_Codifica(Chiave,
                                             enum_TipoNodo.x_GiacenzeMagazzino,
                                             Piva, , , , , , , , , , , , , , , , , , , )


                qs = "?k=" & Stringa_Codifica(Chiave, AgroKey_EncoderDecoder, objServer) &
                     "&c=" & Stringa_Codifica(Carico_Scarico, AgroKey_EncoderDecoder, objServer) &
                     "&o=" & Stringa_Codifica(enum_Security_Operazione.Lettura, AgroKey_EncoderDecoder, objServer) &
                     "&orig=" & Stringa_Codifica(Enum_Pagina_Origine, AgroKey_EncoderDecoder, objServer) &
                     "&mode=" & Stringa_Codifica("compravendita", AgroKey_EncoderDecoder, objServer) &
                     "&l=" & Stringa_Codifica(Lav_Cod, AgroKey_EncoderDecoder, objServer) &
                     "&d=" & Stringa_Codifica(CStr(Date.Today), AgroKey_EncoderDecoder, objServer) &
                     "&s=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, objServer) &
                     "&a=" & Stringa_Codifica(Id_Agenda, AgroKey_EncoderDecoder, objServer)

                Return PathToRoot & "GestioneMagazzini/FormProdotto.aspx" & qs

                '-----------------------------------------------
            Case Else
                Return ""

        End Select

    End Function

End Module
