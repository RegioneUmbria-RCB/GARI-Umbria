Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.Agro_Math

Public Class TracciabilitaConferimenti_XLS
    Inherits System.Web.UI.Page

    Protected WithEvents TableExcel As System.Web.UI.HtmlControls.HtmlTable

#Region " Tracciabilita conferimenti "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    'NOTA: la seguente dichiarazione è richiesta da Progettazione Web Form.
    'Non spostarla o rimuoverla.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region

    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim Data_Da, Data_A As String
    'Dim Regolamento_Cod As enum_Cod_Regolamento
    Dim Regolamento_Cod As String
    Dim Sa_cod As Integer = 0
    Dim Fabbricato_cod As Integer = 0
    Dim Piva As String

    '#########################################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Dim Riga As HtmlTableRow
        Dim Numero_Colonne As Integer = 0


        'La Pagina deve essere visualizzata come un foglio Excel
        Response.ContentType = "application/vnd.ms-excel"
        Response.AddHeader("Content-Disposition", "inline; filename = TracciabilitaConferimenti.xls")

        '##############################################################
        '#####  Verifico Credenziali di Accesso  ######################
        '##############################################################

        '----- Verifico che l'utente sia autenticato

        If Session("ASG_objParametri_Server") Is Nothing Then
            Response.Redirect("~/Custom500.aspx")
        End If

        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))


        '##############################################################
        '############  Lettura Parametri Query String #################
        '##############################################################

        Try

            Regolamento_Cod = Stringa_Decodifica(CStr(Request.QueryString("reg")), _
                            AgroKey_EncoderDecoder, _
                            Server)

            Data_Da = Stringa_Decodifica(CStr(Request.QueryString("dd")), _
                         AgroKey_EncoderDecoder, _
                         Server)

            Data_A = Stringa_Decodifica(CStr(Request.QueryString("da")), _
                             AgroKey_EncoderDecoder, _
                             Server)

            Piva = Stringa_Decodifica(CStr(Request.QueryString("p")), _
                                        AgroKey_EncoderDecoder, _
                                        Server)

            'Sa_Cod = CInt(Stringa_Decodifica(CStr(Request.QueryString("s")), _
            '                          AgroKey_EncoderDecoder, _
            '                          Server))

            'Fabbricato_Cod = CInt(Stringa_Decodifica(CStr(Request.QueryString("f")), _
            '                          AgroKey_EncoderDecoder, _
            '                          Server))

            'Piva_Produttore = Stringa_Decodifica(CStr(Request.QueryString("pp")), _
            '                                    AgroKey_EncoderDecoder, _
            '                                    Server)

            'Piva_Coop1 = Stringa_Decodifica(CStr(Request.QueryString("pc1")), _
            '                               AgroKey_EncoderDecoder, _
            '                               Server)

            'Piva_Coop2 = Stringa_Decodifica(CStr(Request.QueryString("pc2")), _
            '                             AgroKey_EncoderDecoder, _
            '                             Server)


            'Mat_Cod = CInt(Stringa_Decodifica(CStr(Request.QueryString("m")), _
            '                     AgroKey_EncoderDecoder, _
            '                     Server))

            'Descr_Specie = Stringa_Decodifica(CStr(Request.QueryString("spe")), _
            '                                    AgroKey_EncoderDecoder, _
            '                                    Server)

            'Descr_Prodotto = Stringa_Decodifica(CStr(Request.QueryString("pro")), _
            '                                  AgroKey_EncoderDecoder, _
            '                                  Server)

            'Descr_Magazzino = Stringa_Decodifica(CStr(Request.QueryString("mag")), _
            '                                        AgroKey_EncoderDecoder, _
            '                                        Server)

            'RagSoc_Impresa = Stringa_Decodifica(CStr(Request.QueryString("rs")), _
            '                                   AgroKey_EncoderDecoder, _
            '                                   Server)

            'If RagSoc_Impresa = "" Then
            '    RagSoc_Impresa = RagSoc_from_Piva(Server, Session, Page, Piva)
            'End If

            'Tracciabilita_Impianti = CInt(Stringa_Decodifica(CStr(Request.QueryString("ti")), _
            '                                   AgroKey_EncoderDecoder, _
            '                                   Server))


            'Codice_Specie = Stringa_Decodifica(CStr(Request.QueryString("cs")), _
            '                        AgroKey_EncoderDecoder, _
            '                        Server)

            'If Codice_Specie = "0" Then
            '    Codice_Specie = ""
            '    'il codice non è stato passato, 
            '    'perchè si vogliono cercare tutte le specie
            '    'o il range di specie selezionato
            'End If

            'Str_FiltroSpecie = Session("Str_Codici_Specie")

            'If Str_FiltroSpecie <> "" Then
            '    'è stato selezionato un range di codici
            '    Str_FiltroSpecie = " AND MP_Raccolta.Cod_Articolo IN " & Str_FiltroSpecie
            'Else
            '    Str_FiltroSpecie = ""
            'End If


            'Codice_Conferente = Stringa_Decodifica(CStr(Request.QueryString("cc")), _
            '                        AgroKey_EncoderDecoder, _
            '                        Server)


            'If Codice_Conferente = "0" Then
            '    Codice_Conferente = ""
            '    'il codice non è stato passato, 
            '    'perchè si vogliono cercare tutti i conferenti
            '    'o il range di conferenti selezionato
            'Else
            '    Codice_Conferente = CStr(Codice_Conferente)
            'End If

            'Str_FiltroConf = Session("Str_Codici_Conferenti")

            'If Str_FiltroConf <> "" Then
            '    'è stato selezionato un range di codici
            '    Str_FiltroConf = " AND Risorse_Umane_Conferenti.settore_Des IN " & Str_FiltroConf
            'Else
            '    Str_FiltroConf = ""
            'End If


        Catch ex As Exception
            Riga = New HtmlTableRow
            Riga.Cells.Add(New HtmlTableCell)
            Riga.Cells(0).ColSpan = Numero_Colonne
            Riga.Cells(0).InnerHtml = "Dati da querystring: " + ex.Message
            Me.TableExcel.Rows.Add(Riga)
        End Try


        '##############################################################
        '####################  LETTURA DATI ###########################
        '##############################################################

        Dim objADDFun As New AgronicaCoreStampeDAL.AccettazioneDaDiversi_Funzioni
        Dim DT As DataTable
        Dim i As Integer
        'Dim Col As Integer = -1
        'Dim Id_Agenda As Integer
        Dim Setup_Cod As String
        'Dim Progetto_Nome, Veg_Des, Cul_Des, Grfi_Des, Grva_Des, Setup_Cod, Sup_Imp As String
        'Dim Validita_Inizio_Impianto, Validita_Fine_Impianto, Validita_Inizio_Distinta, Validita_Fine_Distinta As String
        'Dim Str_Progetto_Nome, Str_Veg_Des, Str_Cul_Des, Str_Grfi_Des, Str_Grva_Des, Str_Setup_Cod, Str_Sup_Imp As String
        Dim Str_Validita_Inizio_Impianto, Str_Validita_Fine_Impianto, Str_Validita_Inizio_Distinta, Str_Validita_Fine_Distinta As String
        Dim Tipo_Peso As Integer
        Dim Peso As Double


        Try

            Dim ADD As New AgronicaCoreStampeDAL.AccettazioneDaDiversi

            DT = ADD.TracciabilitaConferimenti_XLS(Piva, _
                                                    Regolamento_Cod, _
                                                    Sa_Cod, _
                                                    Fabbricato_Cod, _
                                                    Data_Da, _
                                                    Data_A, _
                                                    "", _
                                                    objParametri_Server)

            If Not IsNothing(DT) Then

                For i = 0 To DT.Rows.Count - 1

                    Try


                        With DT.Rows(i)

                            .Item("Numero_Bolla") = Ricava_NumeroDocumento_Con_Sequenza(.Item("Doc_Numero_Sin"), _
                                                                                        .Item("Doc_Numero"), _
                                                                                        .Item("Doc_Numero_Des"), _
                                                                                        .Item("Lunghezza_Sin"), _
                                                                                        .Item("Lunghezza_Centro"), _
                                                                                        .Item("Lunghezza_Des"), _
                                                                                        .Item("CarattereFormattazione"))

                            .Item("Numero_Conf") = Ricava_NumeroDocumento_Senza_Sequenza(.Item("Doc_Numero_Sin_Conf"), _
                                                                                        .Item("Doc_Numero_Conf"), _
                                                                                        .Item("Doc_Numero_Des_Conf"))


                            .Item("tara_veicolo") = RoundNumber_ParteIntera(.Item("tara_veicolo"))
                            .Item("Tara_Imballi") = RoundNumber_ParteIntera(.Item("Tara_Imballi"))

                            Tipo_Peso = .Item("Tipo_Peso")
                            Peso = .Item("Peso")
                            Peso = RoundNumber_ParteIntera(Peso)

                            .Item("peso_lordo") = Calcola_PesoLordo(Tipo_Peso, Peso, .Item("Tara_Imballi"))
                            .Item("peso_lordo") = RoundNumber_ParteIntera(.Item("peso_lordo"))

                            .Item("peso_netto") = Calcola_PesoNetto(Tipo_Peso, Peso, .Item("Tara_Imballi"))
                            .Item("peso_netto") = RoundNumber_ParteIntera(.Item("peso_netto"))

                            '.Item("peso_totale") = Calcola_PesoTotale(1, .Item("peso_lordo"), .Item("tara_veicolo"))

                            objADDFun.Calcola_NettoPagamento(.Item("peso_netto"), _
                                                    .Item("Variazione_Raccolta"), _
                                                    .Item("Udm_Cod_Raccolta"), _
                                                    .Item("Degrado"), _
                                                    .Item("netto_Pagamento"))

                            ' .Item("Degrado_Perc") = .Item("Variazione_Raccolta")

                            If .Item("Setup_Cod") <> "-1" Then
                                Setup_Cod = .Item("Setup_Cod")
                            Else
                                Setup_Cod = ""
                            End If

                            Select Case .Item("Regolamento_Prodotto")
                                Case enum_Cod_Regolamento.Non_Specificato
                                    .Item("Tipologia_Agricoltura") = "n.d."
                                Case enum_Cod_Regolamento.Regolamento_bio
                                    .Item("Tipologia_Agricoltura") = "BIO"
                                Case enum_Cod_Regolamento.Regolamento_ProduzioneIntegrata
                                    .Item("Tipologia_Agricoltura") = "P.I."
                                Case enum_Cod_Regolamento.Regolamento_Nessuno
                                    .Item("Tipologia_Agricoltura") = "CONV"
                            End Select

                            Str_Validita_Inizio_Impianto = Sistema_ValiditaInizio(CDate(.Item("Validita_Inizio_Impianto")))
                            Str_Validita_Fine_Impianto = Sistema_ValiditaFine(CDate(.Item("Validita_Fine_Impianto")))
                            Str_Validita_Inizio_Distinta = Sistema_ValiditaInizio(CDate(.Item("Validita_Inizio_Distinta")))
                            Str_Validita_Fine_Distinta = Sistema_ValiditaFine(CDate(.Item("validita_Fine_Distinta")))

                            .Item("Str_Progetto_Nome") = .Item("Progetto_Nome")
                            .Item("Str_Veg_Des") = .Item("Veg_Des")
                            .Item("Str_Cul_Des") = .Item("Cul_Des")
                            .Item("Str_Grfi_Des") = .Item("Grfi_Des")
                            .Item("Str_Grva_Des") = .Item("Grva_Des")
                            .Item("Str_Setup_Cod") = Setup_Cod
                            .Item("Str_Sup_Imp") = .Item("Sup_Imp")
                            .Item("Str_Validita_Inizio_Impianto") = Str_Validita_Inizio_Impianto
                            .Item("Str_Validita_Fine_Impianto") = Str_Validita_Fine_Impianto
                            .Item("Str_Validita_Inizio_Distinta") = Str_Validita_Inizio_Distinta
                            .Item("Str_Validita_Fine_Distinta") = Str_Validita_Fine_Distinta

                        End With

                    Catch ex As Exception
                        Riga = New HtmlTableRow
                        Riga.Cells.Add(New HtmlTableCell)
                        Riga.Cells(0).ColSpan = Numero_Colonne
                        Riga.Cells(0).InnerHtml = "Lettura dei dati: " + ex.Message
                        Me.TableExcel.Rows.Add(Riga)
                    End Try

                Next


                DT.Columns.Remove("Id_Agenda")
                DT.Columns.Remove("Peso")
                'DT.Columns.Remove("Peso_totale")
                DT.Columns.Remove("Tara_Veicolo")
                DT.Columns.Remove("Peso_Lordo")
                DT.Columns.Remove("tara_Imballi")
                DT.Columns.Remove("Tipo_Peso")
                DT.Columns.Remove("Peso_Netto")
                DT.Columns.Remove("degrado")
                'DT.Columns.Remove("Lotto_Raccolta")
                'DT.Columns.Remove("degrado_perc")
                DT.Columns.Remove("Doc_Numero_Sin")
                DT.Columns.Remove("Doc_Numero")
                DT.Columns.Remove("Doc_Numero_Des")
                DT.Columns.Remove("Doc_Numero_Sin_Conf")
                DT.Columns.Remove("Doc_Numero_Conf")
                DT.Columns.Remove("Doc_Numero_Des_Conf")
                DT.Columns.Remove("Lunghezza_Sin")
                DT.Columns.Remove("Lunghezza_Centro")
                DT.Columns.Remove("Lunghezza_Des")
                DT.Columns.Remove("CarattereFormattazione")
                'DT.Columns.Remove("Cal_Cod_Raccolta")
                DT.Columns.Remove("Udm_Cod_Raccolta")
                DT.Columns.Remove("Qta_Raccolta")
                DT.Columns.Remove("Variazione_Raccolta")
                DT.Columns.Remove("Tara")
                DT.Columns.Remove("Mat_Cod_Prodotto")
                DT.Columns.Remove("Regolamento_Prodotto")
                DT.Columns.Remove("Progetto_Nome")
                DT.Columns.Remove("Veg_Des")
                DT.Columns.Remove("Cul_Des")
                DT.Columns.Remove("Grfi_Des")
                DT.Columns.Remove("Grva_Des")
                DT.Columns.Remove("Setup_Cod")
                DT.Columns.Remove("Sup_Imp")
                DT.Columns.Remove("Validita_Inizio_Impianto")
                DT.Columns.Remove("Validita_Fine_Impianto")
                DT.Columns.Remove("Validita_Inizio_Distinta")
                DT.Columns.Remove("Validita_Fine_Distinta")
              
            End If


        Catch ex As Exception
            Riga = New HtmlTableRow
            Riga.Cells.Add(New HtmlTableCell)
            Riga.Cells(0).ColSpan = Numero_Colonne
            Riga.Cells(0).InnerHtml = "Lettura dei dati: " + ex.Message
            Me.TableExcel.Rows.Add(Riga)
        End Try



        '##############################################################
        '###################  Creazione EXCEL #########################
        '##############################################################

        'If Not IsNothing(DT) AndAlso DT.Rows.Count <> 0 Then

        Crea_EXCEL(DT)

        'End If



    End Sub


    '##############################################################
    Private Sub Crea_EXCEL(ByVal Dt_Finale As DataTable)


        Dim Riga As HtmlTableRow
        Dim Numero_Colonne As Integer = 0

        If IsNothing(Dt_Finale) Then
            Riga = New HtmlTableRow
            Riga.Cells.Add(New HtmlTableCell)
            Riga.Cells(0).ColSpan = Numero_Colonne
            Riga.Cells(0).InnerHtml = "Il criterio di filtro non ha prodotto alcun risultato."
            Me.TableExcel.Rows.Add(Riga)
            Exit Sub
        End If

        Numero_Colonne = CInt(Dt_Finale.Columns.Count)


        '##############################################################
        '#####  Costruisco la tabella   ###############################
        '##############################################################

        Dim i, j, k, z As Integer

        Try

            Dim Colonna_Dt As String

            If Not IsNothing(Dt_Finale) Then

                Me.TableExcel.Rows(0).Cells(0).ColSpan = CInt(Dt_Finale.Columns.Count)

                '------------------------------------------
                '------------- INTESTAZIONE ---------------
                '------------------------------------------
                Riga = New HtmlTableRow

                For k = 0 To Dt_Finale.Columns.Count - 1

                    Riga.Cells.Add(New HtmlTableCell)
                    ElaboraCellaHTML(Riga.Cells(k), 2, "", "", "Gainsboro", "center", "middle")
                    Riga.Cells(k).Style.Item("vertical-align") = "middle"
                    Riga.Cells(k).Style.Item("font-weight") = "bold"
                    Riga.Cells(k).Style.Item("font-size") = "12px"
                    'Riga.Cells(k).Height = "26"

                    'Riga.Cells(k).InnerHtml = Dt_Finale.Rows(CInt(Dt_Finale.Rows.Count - 1)).Item(k)
                    Riga.Cells(k).InnerHtml = Dt_Finale.Columns.Item(k).Caption

                    Colonna_Dt = Riga.Cells(k).InnerHtml.ToLower

                    Select Case Colonna_Dt

                        Case "numero_bolla"
                            Riga.Cells(k).InnerHtml = "N. Bolla"
                        Case "data_bolla"
                            Riga.Cells(k).InnerHtml = "Data Bolla"
                        Case "numero_conf"
                            Riga.Cells(k).InnerHtml = "N. DDT<br/>Conferimento"
                        Case "data_conf"
                            Riga.Cells(k).InnerHtml = "Data DDT<br/>Conferimento"
                            'Case CStr("Piva_Conferente").ToLower
                            '    Riga.Cells(k).InnerHtml = "P.IVA<br/>Conferente"
                            'Case CStr("Piva_Produttore").ToLower
                            '    Riga.Cells(k).InnerHtml = "P.IVA<br/>Produttore"
                        Case CStr("RagSoc_Produttore").ToLower
                            Riga.Cells(k).InnerHtml = "Produttore"
                            'Case CStr("Piva_Coop1").ToLower
                            '    Riga.Cells(k).InnerHtml = "P.IVA<br/>Cooperativa"
                        Case CStr("RagSoc_Coop1").ToLower
                            Riga.Cells(k).InnerHtml = "Cooperativa"
                            'Case CStr("Piva_Coop2").ToLower
                            '    Riga.Cells(k).InnerHtml = "P.IVA seconda<br/>Cooperativa"
                        Case CStr("RagSoc_Coop2").ToLower
                            Riga.Cells(k).InnerHtml = "Seconda<br/>Cooperativa"
                            'Case CStr("peso_totale").ToLower
                            '    Riga.Cells(k).InnerHtml = "Peso<br/>Totale"
                            'Case CStr("tara_veicolo").ToLower
                            '    Riga.Cells(k).InnerHtml = "Tara<br/>Veicolo"
                            'Case CStr("peso_lordo").ToLower
                            '    Riga.Cells(k).InnerHtml = "Peso<br/>Lordo"
                            'Case CStr("tara_imballi").ToLower
                            '    Riga.Cells(k).InnerHtml = "Tara<br/>Imballi"
                            'Case CStr("peso_netto").ToLower
                            '    Riga.Cells(k).InnerHtml = "Peso<br/>Netto"
                            'Case CStr("degrado_perc").ToLower
                            '    Riga.Cells(k).InnerHtml = "Degrado %"
                            'Case CStr("Listino_Des").ToLower
                            '    Riga.Cells(k).InnerHtml = "Listino<br/>applicato"
                            'Case CStr("prezzo_unitario_netto").ToLower
                            '    Riga.Cells(k).InnerHtml = "Prezzo<br/>Unitario"
                            'Case CStr("Netto_Trasportato").ToLower
                            '    Riga.Cells(k).InnerHtml = "Netto<br/>Trasportato"
                            'Case "codice_zona"
                            '    Riga.Cells(k).InnerHtml = "Codice Zona"
                            'Case CStr("RagSoc_Trasportatore").ToLower
                            '    Riga.Cells(k).InnerHtml = "Trasportatore"
                        Case CStr("Codice_Conferente").ToLower
                            Riga.Cells(k).InnerHtml = "Codice<br/>Conferente"
                        Case CStr("RagSoc_Conferente").ToLower
                            Riga.Cells(k).InnerHtml = "Conferente"
                        Case CStr("Cod_Articolo_Raccolta").ToLower
                            Riga.Cells(k).InnerHtml = "Codice<br/>Prodotto"
                        Case CStr("Mat_Des_Raccolta").ToLower
                            Riga.Cells(k).InnerHtml = "Prodotto"
                        Case CStr("Tipologia_Agricoltura").ToLower
                            Riga.Cells(k).InnerHtml = "Tipologia<br/>Agricoltura"
                            'Case CStr("Descr_Specie").ToLower
                            '    Riga.Cells(k).InnerHtml = "Specie"
                            'Case CStr("Descr_Varieta").ToLower
                            '    Riga.Cells(k).InnerHtml = "Varieta"
                            'Case CStr("Codice_Prodotto").ToLower
                            '    Riga.Cells(k).InnerHtml = "Codice Gias<br/>Prodotto"
                            'Case CStr("punteggio").ToLower
                            '    Riga.Cells(k).InnerHtml = "Indice<br/>Qualitat."
                        Case CStr("Netto_Pagamento").ToLower
                            Riga.Cells(k).InnerHtml = "Netto a<br/>Pagamento"
                        Case CStr("Str_Progetto_Nome").ToLower
                            Riga.Cells(k).InnerHtml = "Lotto"
                        Case CStr("Str_Veg_Des").ToLower
                            Riga.Cells(k).InnerHtml = "Specie"
                        Case CStr("Str_Cul_Des").ToLower
                            Riga.Cells(k).InnerHtml = "Varietà"
                        Case CStr("Str_Grfi_Des").ToLower
                            Riga.Cells(k).InnerHtml = "Finalità"
                        Case CStr("Str_Grva_Des").ToLower
                            Riga.Cells(k).InnerHtml = "Tipologia Varietale"
                        Case CStr("Str_Setup_Cod").ToLower
                            Riga.Cells(k).InnerHtml = "Sem/Trap"
                        Case CStr("Str_Sup_Imp").ToLower
                            Riga.Cells(k).InnerHtml = "Superficie"
                        Case CStr("Str_Validita_Inizio_Impianto").ToLower
                            Riga.Cells(k).InnerHtml = "Data Inizio"
                        Case CStr("Str_Validita_Fine_Impianto").ToLower
                            Riga.Cells(k).InnerHtml = "Data Fine"
                        Case CStr("Str_Validita_Inizio_Distinta").ToLower
                            Riga.Cells(k).InnerHtml = "Data Inizio<br/>Esercizio"
                        Case CStr("Str_Validita_Fine_Distinta").ToLower
                            Riga.Cells(k).InnerHtml = "Data Fine<br/>Esercizio"

                    End Select

                Next

                Me.TableExcel.Rows.Add(Riga)
                '----------------------------------
                '------- FINE INTESTAZIONE --------
                '----------------------------------

                '----------------------------------
                '------ RIEMPIMENTO TABELLA -------
                '----------------------------------

                'scorro le righe
                For i = 0 To Dt_Finale.Rows.Count - 1

                    Riga = New HtmlTableRow

                    'scorro le colonne
                    For j = 0 To Dt_Finale.Columns.Count - 1

                        'aggiungo la cella
                        Riga.Cells.Add(New HtmlTableCell)
                        Riga.Cells(j).Style.Item("text-align") = "center"
                        Riga.Cells(j).Style.Item("vertical-align") = "middle"
                        'Riga.Cells(j).Height = "26"

                        Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(Dt_Finale.Rows(i).Item(j)), Dt_Finale.Rows(i).Item(j), "&nbsp;")

                        Select Case Dt_Finale.Columns.Item(j).Caption.ToLower

                            Case CStr("Piva_Produttore").ToLower, CStr("Piva_Conferente").ToLower, _
                                    CStr("Piva_Coop1").ToLower, CStr("Piva_Coop2").ToLower
                                'aggiungo uno spazio davanti x salvare gli zeri...
                                Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(Dt_Finale.Rows(i).Item(j)), "&nbsp;" & Dt_Finale.Rows(i).Item(j), "&nbsp;")

                            Case CStr("Str_Sup_Imp").ToLower, "numero_conf"
                                If Dt_Finale.Rows(i).Item(j) <> "" Then
                                    'aggiungo un apice davanti per vedere visualizzato il numero e non una data
                                    'Riga.Cells(j).InnerHtml = "'" & Dt_Finale.Rows(i).Item(j)
                                    Riga.Cells(j).InnerHtml = "&nbsp;" & Dt_Finale.Rows(i).Item(j)
                                End If

                            Case Else

                                Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(Dt_Finale.Rows(i).Item(j)), Dt_Finale.Rows(i).Item(j), "&nbsp;")

                        End Select

                    Next

                    'Aggiungo la Riga alla Tabella 
                    Me.TableExcel.Rows.Add(Riga)

                Next
                '-------------------------------
                '--------- FINE TABELLA --------
                '-------------------------------

            End If

        Catch ex As Exception

            Riga = New HtmlTableRow
            Riga.Cells.Add(New HtmlTableCell)
            Riga.Cells(0).ColSpan = Numero_Colonne
            Riga.Cells(0).InnerHtml = "Creazione Excel Esportazione: " + ex.Message
            Me.TableExcel.Rows.Add(Riga)

        End Try

        '########################################################


    End Sub




End Class
