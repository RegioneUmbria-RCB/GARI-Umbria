Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.Agro_Math
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Tracciabilita_Conf_XLS
    Inherits System.Web.UI.Page

    Protected WithEvents TableExcel As System.Web.UI.HtmlControls.HtmlTable


#Region " Trasportatori "

    Private Property Codice_Specie As String

    'This call is required by the Web Form Designer.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    'NOTE: The following placeholder declaration is required by the Web Form Designer.
    'Do not delete or move it.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: This method call is required by the Web Form Designer
        'Do not modify it using the code editor.
        InitializeComponent()
    End Sub

#End Region

    Dim Data_Da, Data_A As String
    Dim Codice_ConfCli As String
    Dim RagSoc_ConfCli As String
    Dim Cod_Rapporto As Integer = 0
    Dim Codice_Prodotto As String
    Dim Str_FiltroConf As String
    Dim Str_FiltroSpecie As String
    Dim Piva As String
    Dim int_Configurazione_Moduli As enum_Omni_Modulo_Generazione
    Dim Mat_Cod As Integer
    Dim Mat_Des As String = ""
    Dim Cod_Articolo As String = ""
    Dim Sa_Cod, Fabbricato_Cod As Integer
    Dim RagSoc_Impresa As String
    Dim Descr_Specie As String
    Dim Descr_Prodotto As String
    Dim Descr_Magazzino As String
    Dim Tracciabilita_Impianti As Integer


    '--------------------------------------
    '----------- MATRICE ------------

    Const NUM_RIGHE As Integer = 21

    
    '------------------------------------------


    '-------------------------------------------
    '---------- EXCEL ------------
    Const PRIMA_CELLA_INIZIO As Integer = 0
    Const ULTIMA_CELLA_INIZIO As Integer = 24
    Const PRIMA_CELLA_CALIBRI As Integer = 25
    Const ULTIMA_CELLA_CALIBRI As Integer = 46


    '-------------------------------------------

    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri


    '#########################################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Dim Riga As HtmlTableRow
        Dim Numero_Colonne As Integer = 0
        Dim objImpreseR As New AgronicaCoreAnagrafeDAL.Imprese_Read

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

            Data_Da = Stringa_Decodifica(CStr(Request.QueryString("dd")), _
                             AgroKey_EncoderDecoder, _
                             Server)

            Data_A = Stringa_Decodifica(CStr(Request.QueryString("da")), _
                             AgroKey_EncoderDecoder, _
                             Server)

            Piva = Stringa_Decodifica(CStr(Request.QueryString("p")), _
                                        AgroKey_EncoderDecoder, _
                                        Server)

 

            Sa_Cod = CInt(Stringa_Decodifica(CStr(Request.QueryString("s")), _
                                      AgroKey_EncoderDecoder, _
                                      Server))

            Fabbricato_Cod = CInt(Stringa_Decodifica(CStr(Request.QueryString("f")), _
                                      AgroKey_EncoderDecoder, _
                                      Server))

            Mat_Cod = CInt(Stringa_Decodifica(CStr(Request.QueryString("m")), _
                                 AgroKey_EncoderDecoder, _
                                 Server))

            Mat_Des = Stringa_Decodifica(CStr(Request.QueryString("dpr")), _
                                           AgroKey_EncoderDecoder, _
                                           Server)


            Cod_Articolo = Stringa_Decodifica(CStr(Request.QueryString("ca")), _
                                           AgroKey_EncoderDecoder, _
                                           Server)



            Descr_Prodotto = Stringa_Decodifica(CStr(Request.QueryString("pro")), _
                                              AgroKey_EncoderDecoder, _
                                              Server)

            Descr_Magazzino = Stringa_Decodifica(CStr(Request.QueryString("mag")), _
                                                    AgroKey_EncoderDecoder, _
                                                    Server)

            RagSoc_Impresa = Stringa_Decodifica(CStr(Request.QueryString("rs")), _
                                               AgroKey_EncoderDecoder, _
                                               Server)


            If RagSoc_Impresa = "" Then
                RagSoc_Impresa = objImpreseR.RagSoc_from_Piva(Piva, objParametri_Server)
            End If

            'Tracciabilita_Impianti = CInt(Stringa_Decodifica(CStr(Request.QueryString("ti")), _
            '                                   AgroKey_EncoderDecoder, _
            '                                   Server))

            Tracciabilita_Impianti = 1

            Codice_ConfCli = Stringa_Decodifica(CStr(Request.QueryString("cc")), _
                                    AgroKey_EncoderDecoder, _
                                    Server)


            If Codice_ConfCli = "0" Then
                Codice_ConfCli = ""
                'il codice non è stato passato, 
                'perchè si vogliono cercare tutti i conferenti
                'o il range di conferenti selezionato
            Else
                Codice_ConfCli = CStr(Codice_ConfCli)
            End If

            RagSoc_ConfCli = Stringa_Decodifica(CStr(Request.QueryString("rsc")), _
                        AgroKey_EncoderDecoder, _
                        Server)

            Cod_Rapporto = CInt(Stringa_Decodifica(CStr(Request.QueryString("rc")), _
                 AgroKey_EncoderDecoder, _
                 Server))


            int_Configurazione_Moduli = CInt(Stringa_Decodifica(CStr(Request.QueryString("cm")), _
                                  AgroKey_EncoderDecoder, _
                                  Server))



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


        Dim i, j, k As Integer
        Dim Hash_Varieta As Hashtable
        Dim Mtx_Varieta(NUM_RIGHE, 0) As String

        Dim Str_Progetto_Nome, Str_Veg_Des, Str_Cul_Des, Str_Grfi_Des, Str_Grva_Des, Str_Setup_Cod, Str_Sup_Imp As String
        Dim Str_Validita_Inizio_Impianto, Str_Validita_Fine_Impianto, Str_Validita_Inizio_Distinta, Str_Validita_Fine_Distinta As String
        Dim Tipo_Peso As Integer
        Dim Peso, Tara_imballi As Double


        Dim NumeroBolla As String
        Dim NumeroConf As String


        Dim DT_DatiRaccoltaCollegatiABolle As DataTable

        Try

            Dim objConf As New AgronicaCoreStampeDAL.ConferimentoAccettazione
            Dim str_ElencoBolle As String

            Dim DT_DatiBolle As DataTable
            Dim str_selectDatiBolle As String



            DT_DatiBolle = objConf.DatiBollePerTracciabilita_XLS(Piva, _
                                                                Codice_ConfCli, _
                                                                RagSoc_ConfCli, _
                                                                Cod_Rapporto, _
                                                                Data_Da, _
                                                                Data_A, _
                                                                Mat_Cod, _
                                                                Mat_Des, _
                                                                Cod_Articolo, _
                                                                "", _
                                                                str_selectDatiBolle, _
                                                                int_Configurazione_Moduli, _
                                                                objParametri_Server)



            str_ElencoBolle = objConf.ElencoBolle(str_selectDatiBolle, _
                                                  objParametri_Server)


            DT_DatiRaccoltaCollegatiABolle = objConf.DatiRaccoltaCollegatiABolle(Piva, _
                                                                                 str_ElencoBolle, _
                                                                                 "", "", _
                                                                                 int_Configurazione_Moduli, _
                                                                                 objParametri_Server)



            Try
                If Not IsNothing(DT_DatiRaccoltaCollegatiABolle) Then

                    For i = 0 To DT_DatiRaccoltaCollegatiABolle.Rows.Count - 1


                        With DT_DatiRaccoltaCollegatiABolle.Rows(i)

                            'I dati di testata bolla sono gli stessi per tutti i dettagli
                            For j = 0 To DT_DatiBolle.Rows.Count - 1
                                If DT_DatiBolle.Rows(j).Item("Id_agenda") = .Item("Id_Agenda_Bolla") Then

                                    
                                    '' AS Codice_ConfCli,
                                    .Item("Codice_ConfCli") = DT_DatiBolle.Rows(j).Item("Codice_ConfCli")
                                    '' AS Piva_ConfCli,  
                                    .Item("Piva_ConfCli") = DT_DatiBolle.Rows(j).Item("Piva_ConfCli")
                                    '' AS RagSoc_ConfCli,
                                    .Item("RagSoc_ConfCli") = DT_DatiBolle.Rows(j).Item("RagSoc_ConfCli")


                                    NumeroBolla = Ricava_NumeroDocumento_Senza_Sequenza(DT_DatiBolle.Rows(j).Item("Doc_Numero_Sin"), _
                                                                                                                    DT_DatiBolle.Rows(j).Item("Doc_Numero"), _
                                                                                                                    DT_DatiBolle.Rows(j).Item("Doc_Numero_Des"))
                                    .Item("Numero_Bolla") = NumeroBolla



                                    '' AS Data_Bolla,
                                    .Item("Data_Bolla") = DT_DatiBolle.Rows(j).Item("Data_Bolla")

                                    NumeroConf = Ricava_NumeroDocumento_Senza_Sequenza(DT_DatiBolle.Rows(j).Item("Doc_Numero_Sin_Conf"), _
                                                                                       DT_DatiBolle.Rows(j).Item("Doc_Numero_Conf"), _
                                                                                       DT_DatiBolle.Rows(j).Item("Doc_Numero_Des_Conf"))


                                    .Item("Numero_Conf") = NumeroConf

                                    '' AS Data_Conf,
                                    .Item("Data_Conf") = DT_DatiBolle.Rows(j).Item("Data_Conf")

                                    '' AS Lotto_Raccolta, '' AS Cal_Cod_Raccolta,       
                                    '' AS Tara,   
                                    '' AS Listino, '' AS Prezzo_Unitario_Netto,  
                                    .Item("Listino") = DT_DatiBolle.Rows(j).Item("Listino")
                                    .Item("Prezzo_Unitario_Netto") = DT_DatiBolle.Rows(j).Item("Prezzo_Unitario_Netto")


                                    '' AS Peso_Lordo, '' AS Tara_Imballi, '' AS Tipo_Peso, '' AS Peso_Netto, 
                                    Tipo_Peso = DT_DatiBolle.Rows(j).Item("Tipo_Peso")
                                    Peso = DT_DatiBolle.Rows(j).Item("Peso")
                                    'Peso = RoundNumber_ParteIntera(Peso)
                                    Tara_imballi = DT_DatiBolle.Rows(j).Item("Tara_Imballi")




                                    .Item("peso_lordo") = Calcola_PesoLordo(0, Peso, Tara_imballi)
                                    '.Item("peso_lordo") = RoundNumber_ParteIntera(.Item("peso_lordo"))

                                    .Item("peso_netto") = Calcola_PesoNetto(0, Peso, Tara_imballi)
                                    '.Item("peso_netto") = RoundNumber_ParteIntera(.Item("peso_netto"))
                                    .Item("tara_imballi") = DT_DatiBolle.Rows(j).Item("Tara_Imballi")


                                    '' AS Degrado_Perc, '' AS Degrado, '' AS Netto_Pagamento, 
                                    If .Item("peso_netto") <> 0 Then
                                        .Item("Percentuale_rip") = (.Item("Qta_Raccolta") * 100) / .Item("peso_netto")
                                        .Item("Percentuale_rip") = RoundNumber_1Decimale(.Item("Percentuale_rip"))
                                    End If
                                End If
                            Next



                            If LCase(.Item("Calibro_Des")) = "indefinito" Then
                                .Item("Calibro_Des") = ""
                            End If
                            If LCase(.Item("Indice")) = "0" Then
                                .Item("Indice") = ""
                            End If
                            If LCase(.Item("Lotto_Raccolta")) = "indefinito" Then
                                .Item("Lotto_Raccolta") = ""
                            End If



                            'Tipologia_Agricoltura 
                            'impostato in base al regolamento della tabella Materie_Prime

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

                            If Not IsDBNull(.Item("Progetto_Nome")) Then
                                .Item("Progetto_Nome") = CStr(.Item("Progetto_Nome"))
                                .Item("Veg_Des") = CStr(.Item("Veg_Des"))
                                .Item("Cul_Des") = CStr(.Item("Cul_Des"))
                                .Item("Grfi_Des") = CStr(.Item("Grfi_Des"))
                                .Item("Grva_Des") = CStr(.Item("Grva_Des"))
                                .Item("Setup_Cod") = CStr(.Item("Setup_Cod"))
                                .Item("Sup_Imp") = CStr(.Item("Sup_Imp"))
                                .Item("Validita_Inizio_Impianto") = CStr(.Item("Validita_Inizio_Impianto"))
                                .Item("Validita_Fine_Impianto") = CStr(.Item("Validita_Fine_Impianto"))
                                .Item("Validita_Inizio_Distinta") = CStr(.Item("Validita_Inizio_Distinta"))
                                .Item("Validita_Fine_Distinta") = CStr(.Item("Validita_Fine_Distinta"))

                                If LCase(.Item("Setup_Cod")) = "-1" Then
                                    .Item("Setup_Cod") = ""
                                End If
                            End If

                        End With

                    Next



                    If int_Configurazione_Moduli = enum_Omni_Modulo_Generazione.FreshFood Then
                        DT_DatiRaccoltaCollegatiABolle.Columns.Remove("Calibro_Des")
                        DT_DatiRaccoltaCollegatiABolle.Columns.Remove("Indice")
                    End If


                    DT_DatiRaccoltaCollegatiABolle.Columns.Remove("Id_Agenda_Bolla")
                    DT_DatiRaccoltaCollegatiABolle.Columns.Remove("IdAgenda_OpRaccolta")


                    DT_DatiRaccoltaCollegatiABolle.Columns.Remove("Doc_Numero_Sin")
                    DT_DatiRaccoltaCollegatiABolle.Columns.Remove("Doc_Numero")
                    DT_DatiRaccoltaCollegatiABolle.Columns.Remove("Doc_Numero_Des")
                    DT_DatiRaccoltaCollegatiABolle.Columns.Remove("Doc_Numero_Sin_Conf")
                    DT_DatiRaccoltaCollegatiABolle.Columns.Remove("Doc_Numero_Conf")
                    DT_DatiRaccoltaCollegatiABolle.Columns.Remove("Doc_Numero_Des_Conf")

                    DT_DatiRaccoltaCollegatiABolle.Columns.Remove("Cal_Cod_Raccolta")
                    DT_DatiRaccoltaCollegatiABolle.Columns.Remove("Regolamento_Prodotto")
                    DT_DatiRaccoltaCollegatiABolle.Columns.Remove("Variazione_Raccolta")
                    DT_DatiRaccoltaCollegatiABolle.Columns.Remove("Descr_varieta")
                    DT_DatiRaccoltaCollegatiABolle.Columns.Remove("Udm_Cod_Raccolta")
                    DT_DatiRaccoltaCollegatiABolle.Columns.Remove("Grva_Des")
                    DT_DatiRaccoltaCollegatiABolle.Columns.Remove("Descr_Specie")
                    DT_DatiRaccoltaCollegatiABolle.Columns.Remove("Veg_Des")
                    DT_DatiRaccoltaCollegatiABolle.Columns.Remove("Codice_Prodotto")


                End If

            Catch ex As Exception
                Riga = New HtmlTableRow
                Riga.Cells.Add(New HtmlTableCell)
                Riga.Cells(0).ColSpan = Numero_Colonne
                Riga.Cells(0).InnerHtml = "Lettura dei dati: " + ex.Message
                Me.TableExcel.Rows.Add(Riga)
            End Try





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
        Crea_EXCEL(DT_DatiRaccoltaCollegatiABolle, Mtx_Varieta, Hash_Varieta)



    End Sub





    '##############################################################
    Private Sub Crea_EXCEL(ByVal Dt_Finale As DataTable, _
                            ByVal Mtx_Varieta(,) As String, _
                            ByVal Hash_Varieta As Hashtable)


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
            Dim lbl_ConfCli As String

            Select Case int_Configurazione_Moduli
                Case enum_Omni_Modulo_Generazione.FreshFood
                    lbl_ConfCli = "Conferente"
                Case Else
                    lbl_ConfCli = "Cliente"
            End Select

            If Not IsNothing(Dt_Finale) Then

                Me.TableExcel.Rows(0).Cells(0).ColSpan = CInt(Dt_Finale.Columns.Count)

                '------------------------------------------
                '------------- INTESTAZIONE ---------------
                '------------------------------------------
                Riga = New HtmlTableRow

                For k = 0 To Dt_Finale.Columns.Count - 1

                    Riga.Cells.Add(New HtmlTableCell)
                    AgronicaCoreDataProvider.UtilityProvider.ElaboraCellaHTML(Riga.Cells(k), 2, "", "", "Gainsboro", "center", "middle")
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
                        Case CStr("Piva_ConfCli").ToLower
                            Riga.Cells(k).InnerHtml = "P.IVA<br/>" + lbl_ConfCli

                        Case CStr("peso_totale").ToLower
                            Riga.Cells(k).InnerHtml = "Peso<br/>Totale"
                        Case CStr("tara_veicolo").ToLower
                            Riga.Cells(k).InnerHtml = "Tara<br/>Veicolo"
                        Case CStr("peso_lordo").ToLower
                            Riga.Cells(k).InnerHtml = "Peso<br/>Lordo<br/>Tot."
                        Case CStr("tara_imballi").ToLower
                            Riga.Cells(k).InnerHtml = "Tara<br/>Imballi<br/>Tot."
                        Case CStr("peso_netto").ToLower
                            Riga.Cells(k).InnerHtml = "Peso<br/>Netto<br/>Tot."
                        Case CStr("degrado_perc").ToLower
                            Riga.Cells(k).InnerHtml = "Degrado %"
                        Case CStr("Listino_Des").ToLower
                            Riga.Cells(k).InnerHtml = "Listino<br/>applicato"
                        Case CStr("prezzo_unitario_netto").ToLower
                            Riga.Cells(k).InnerHtml = "Prezzo<br/>Unitario"
                        Case CStr("Netto_Trasportato").ToLower
                            Riga.Cells(k).InnerHtml = "Netto<br/>Trasportato"
                        Case "codice_zona"
                            Riga.Cells(k).InnerHtml = "Codice Zona"
                        Case CStr("RagSoc_Trasportatore").ToLower
                            Riga.Cells(k).InnerHtml = "Trasportatore"
                        Case CStr("Codice_ConfCli").ToLower
                            Riga.Cells(k).InnerHtml = "Codice<br/>" + lbl_ConfCli
                        Case CStr("RagSoc_ConfCli").ToLower
                            Riga.Cells(k).InnerHtml = lbl_ConfCli
                        Case CStr("Cod_Articolo_Raccolta").ToLower
                            Riga.Cells(k).InnerHtml = "Codice<br/>Articolo"
                        
                        Case CStr("Descr_Varieta").ToLower
                            Riga.Cells(k).InnerHtml = "Varieta"
                        Case CStr("Mat_Des_Raccolta").ToLower
                            Riga.Cells(k).InnerHtml = "Prodotto"
                        Case CStr("Codice_Prodotto").ToLower
                            Riga.Cells(k).InnerHtml = "Codice<br/>Prodotto" '"Codice Gias<br/>Prodotto"
                        Case CStr("Indice").ToLower
                            Riga.Cells(k).InnerHtml = "Indice<br/>Qualitat."
                        Case CStr("Netto_Pagamento").ToLower
                            Riga.Cells(k).InnerHtml = "Netto a<br/>Pagamento"
                        Case CStr("Progetto_Nome").ToLower
                            Riga.Cells(k).InnerHtml = "Lotto Impianti"
                        Case CStr("Veg_Des").ToLower
                            Riga.Cells(k).InnerHtml = "Specie<br/>Impianti" '"Specie Gias<br/>Impianti"
                        Case CStr("Cul_Des").ToLower
                            Riga.Cells(k).InnerHtml = "Varietà<br/>Impianti" '"Varietà Gias<br/>Impianti"
                        Case CStr("Grfi_Des").ToLower
                            Riga.Cells(k).InnerHtml = "Finalità<br/>Impianti"
                        Case CStr("Grva_Des").ToLower
                            Riga.Cells(k).InnerHtml = "Tipologia Varietale<br/>Impianti"
                        Case CStr("Setup_Cod").ToLower
                            Riga.Cells(k).InnerHtml = "Sem/Trap"
                        Case CStr("Sup_Imp").ToLower
                            Riga.Cells(k).InnerHtml = "Superficie<br/>Impianti"
                        Case CStr("Validita_Inizio_Impianto").ToLower
                            Riga.Cells(k).InnerHtml = "Data Inizio<br/>Impianti"
                        Case CStr("Validita_Fine_Impianto").ToLower
                            Riga.Cells(k).InnerHtml = "Data Fine<br/>Impianti"
                        Case CStr("Validita_Inizio_Distinta").ToLower
                            Riga.Cells(k).InnerHtml = "Data Inizio<br/>Distinta Impianti"
                        Case CStr("Validita_Fine_Distinta").ToLower
                            Riga.Cells(k).InnerHtml = "Data Fine<br/>Distinta Impianti"
                        Case CStr("Tipologia_Agricoltura").ToLower
                            Riga.Cells(k).InnerHtml = "Tipologia<br/>Agricoltura"
                        Case CStr("Calibro_Des").ToLower
                            Riga.Cells(k).InnerHtml = "Campionatura"
                        Case CStr("Udm_Des_Raccolta").ToLower
                            Riga.Cells(k).InnerHtml = "Un. misura"
                        Case CStr("Qta_Raccolta").ToLower
                            Riga.Cells(k).InnerHtml = "Peso Netto<br/>Prodotto"
                        Case CStr("Percentuale_rip").ToLower
                            Riga.Cells(k).InnerHtml = "%<br/>Ripartizione"
                        Case CStr("Ripartizioni_qta_imp").ToLower
                            Riga.Cells(k).InnerHtml = "Ripartizioni<br/>Impianti"
                        Case CStr("Lotto_Raccolta").ToLower
                            Riga.Cells(k).InnerHtml = "Lotto<br/>Raccolta"
                        Case CStr("Unita_misura").ToLower
                            Riga.Cells(k).InnerHtml = "Unita<br/>Misura"
                        Case CStr("Cella_Stiva").ToLower
                            Riga.Cells(k).InnerHtml = "Cella/Stiva"

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

                            Case CStr("Piva_ConfCli").ToLower

                                'aggiungo uno spazio davanti x salvare gli zeri...
                                Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(Dt_Finale.Rows(i).Item(j)), "&nbsp;" & Dt_Finale.Rows(i).Item(j), "&nbsp;")

                                'Case CStr("Piva_Produttore").ToLower,_
                                'CStr("Piva_Coop1").ToLower, CStr("Piva_Coop2").ToLower
                                'aggiungo uno spazio davanti x salvare gli zeri...
                                '    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(Dt_Finale.Rows(i).Item(j)), "&nbsp;" & Dt_Finale.Rows(i).Item(j), "&nbsp;")

                            Case "numero_conf"
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

