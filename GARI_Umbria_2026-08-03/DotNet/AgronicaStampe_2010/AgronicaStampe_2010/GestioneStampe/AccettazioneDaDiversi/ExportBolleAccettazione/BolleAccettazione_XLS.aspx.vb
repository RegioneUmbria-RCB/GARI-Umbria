Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.Agro_Math
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class BolleAccettazione_XLS
    Inherits System.Web.UI.Page

    Protected WithEvents TableExcel As System.Web.UI.HtmlControls.HtmlTable


#Region " Trasportatori "

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
    Dim Codice_Conferente As String
    Dim Codice_Specie As String
    Dim Codice_Prodotto As String
    Dim Str_FiltroConf As String
    Dim Str_FiltroSpecie As String
    Dim Piva As String
    Dim Piva_Produttore, Piva_Coop1, Piva_Coop2 As String
    Dim Mat_Cod As Integer
    Dim Sa_Cod, Fabbricato_Cod As Integer
    Dim RagSoc_Impresa As String
    Dim Descr_Specie As String
    Dim Descr_Prodotto As String
    Dim Descr_Magazzino As String
    Dim Tracciabilita_Impianti As Integer


    '--------------------------------------
    '----------- MATRICE ------------

    Const NUM_RIGHE As Integer = 21

    Const C_A As Integer = 0
    Const C_B As Integer = 1
    Const C_C As Integer = 2
    Const C_D As Integer = 3
    Const C_CREMA As Integer = 4
    Const C_FOGLIA_FOGLIA As Integer = 5
    Const C_SECONDO_SFALCIO As Integer = 6
    Const C_SUB_STANDARD As Integer = 7
    Const C_POMODORO As Integer = 8
    Const GT_80 As Integer = 9
    Const GT_80_85 As Integer = 10
    Const GT_86_90 As Integer = 11
    Const GT_91_95 As Integer = 12
    Const GT_96_100 As Integer = 13
    Const GT_101_110 As Integer = 14
    Const GT_111_120 As Integer = 15
    Const GT_121_130 As Integer = 16
    Const GT_131_140 As Integer = 17
    Const GT_140 As Integer = 18

    Const R_COD_PROD As Integer = 19
    Const R_SPECIE As Integer = 20
    Const R_VARIETA As Integer = 21
    '------------------------------------------


    '-------------------------------------------
    '---------- EXCEL ------------
    Const PRIMA_CELLA_INIZIO As Integer = 0
    Const ULTIMA_CELLA_INIZIO As Integer = 24
    Const PRIMA_CELLA_CALIBRI As Integer = 25
    Const ULTIMA_CELLA_CALIBRI As Integer = 46

    Const N_COL_SPECIE As Integer = 25
    Const N_COL_VARIETA As Integer = 26
    Const N_COL_NETTO_PAG As Integer = 27
    Const N_COL_A As Integer = 28
    Const N_COL_B As Integer = 29
    Const N_COL_C As Integer = 30
    Const N_COL_D As Integer = 31
    Const N_COL_CREMA As Integer = 32
    Const N_COL_FOGLIA As Integer = 33
    Const N_COL_SEC_SFALCIO As Integer = 34
    Const N_COL_SS As Integer = 35
    Const N_COL_POMO As Integer = 36
    Const N_COL_80 As Integer = 37
    Const N_COL_80_85 As Integer = 38
    Const N_COL_86_90 As Integer = 39
    Const N_COL_91_95 As Integer = 40
    Const N_COL_96_100 As Integer = 41
    Const N_COL_101_110 As Integer = 42
    Const N_COL_111_120 As Integer = 43
    Const N_COL_121_130 As Integer = 44
    Const N_COL_131_140 As Integer = 45
    Const N_COL_140 As Integer = 46
    '-------------------------------------------

    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri


    '#########################################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Dim Riga As HtmlTableRow
        Dim Numero_Colonne As Integer = 0
        Dim objImpreseR As New AgronicaCoreAnagrafeDAL.Imprese_Read

        'La Pagina deve essere visualizzata come un foglio Excel
        Response.ContentType = "application/vnd.ms-excel"
        Response.AddHeader("Content-Disposition", "inline; filename = BolleAccettazione.xls")

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

            Piva_Produttore = Stringa_Decodifica(CStr(Request.QueryString("pp")), _
                                                AgroKey_EncoderDecoder, _
                                                Server)

            Piva_Coop1 = Stringa_Decodifica(CStr(Request.QueryString("pc1")), _
                                           AgroKey_EncoderDecoder, _
                                           Server)

            Piva_Coop2 = Stringa_Decodifica(CStr(Request.QueryString("pc2")), _
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

            Descr_Specie = Stringa_Decodifica(CStr(Request.QueryString("spe")), _
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

            Tracciabilita_Impianti = CInt(Stringa_Decodifica(CStr(Request.QueryString("ti")), _
                                               AgroKey_EncoderDecoder, _
                                               Server))


            Codice_Specie = Stringa_Decodifica(CStr(Request.QueryString("cs")), _
                                    AgroKey_EncoderDecoder, _
                                    Server)

            If Codice_Specie = "0" Then
                Codice_Specie = ""
                'il codice non è stato passato, 
                'perchè si vogliono cercare tutte le specie
                'o il range di specie selezionato
            End If

            Str_FiltroSpecie = Session("Str_Codici_Specie")

            If Str_FiltroSpecie <> "" Then
                'è stato selezionato un range di codici
                Str_FiltroSpecie = " AND MP_Raccolta.Cod_Articolo IN " & Str_FiltroSpecie
            Else
                Str_FiltroSpecie = ""
            End If


            Codice_Conferente = Stringa_Decodifica(CStr(Request.QueryString("cc")), _
                                    AgroKey_EncoderDecoder, _
                                    Server)


            If Codice_Conferente = "0" Then
                Codice_Conferente = ""
                'il codice non è stato passato, 
                'perchè si vogliono cercare tutti i conferenti
                'o il range di conferenti selezionato
            Else
                Codice_Conferente = CStr(Codice_Conferente)
            End If

            Str_FiltroConf = Session("Str_Codici_Conferenti")

            If Str_FiltroConf <> "" Then
                'è stato selezionato un range di codici
                Str_FiltroConf = " AND Risorse_Umane_Conferenti.settore_Des IN " & Str_FiltroConf
            Else
                Str_FiltroConf = ""
            End If


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

        Dim DT As DataTable
        Dim i, j As Integer
        Dim Hash_Varieta As Hashtable
        Dim Mtx_Varieta(NUM_RIGHE, 0) As String
        Dim Tot_Netto_Varieta As Double
        Dim Tot_Netto_Calibro_Varieta As Double
        Dim Col As Integer = -1
        Dim DT_Impianti As DataTable
        Dim Id_Agenda As Integer
        Dim Progetto_Nome, Veg_Des, Cul_Des, Grfi_Des, Grva_Des, Setup_Cod, Sup_Imp As String
        Dim Validita_Inizio_Impianto, Validita_Fine_Impianto, Validita_Inizio_Distinta, Validita_Fine_Distinta As String
        Dim Str_Progetto_Nome, Str_Veg_Des, Str_Cul_Des, Str_Grfi_Des, Str_Grva_Des, Str_Setup_Cod, Str_Sup_Imp As String
        Dim Str_Validita_Inizio_Impianto, Str_Validita_Fine_Impianto, Str_Validita_Inizio_Distinta, Str_Validita_Fine_Distinta As String
        Dim Tipo_Peso As Integer
        Dim Peso As Double


        Try

            Dim ADD As New AgronicaCoreStampeDAL.AccettazioneDaDiversi
            Dim objADDFun As New AgronicaCoreStampeDAL.AccettazioneDaDiversi_Funzioni

            'DT = NewCom_ADD_BolleAccettazione_Leggi(Server, Session, Page, _
            '                                        Piva, _
            '                                        Sa_Cod, _
            '                                        Fabbricato_Cod, _
            '                                        Codice_Specie, _
            '                                        Codice_Conferente, _
            '                                        Piva_Produttore, _
            '                                        Data_Da, _
            '                                        Data_A, _
            '                                        Mat_Cod, _
            '                                        Str_FiltroSpecie, _
            '                                        Str_FiltroConf, _
            '                                        "")

            DT = ADD.Bolle_XLS(Piva, _
                                Sa_Cod, _
                                Fabbricato_Cod, _
                                Codice_Specie, _
                                Codice_Conferente, _
                                Piva_Produttore, _
                                piva_coop1, _
                                piva_coop2, _
                                Data_Da, _
                                Data_A, _
                                Mat_Cod, _
                                Str_FiltroSpecie, _
                                Str_FiltroConf, _
                                "", _
                                objParametri_Server)

            If Not IsNothing(DT) Then

                Hash_Varieta = New Hashtable

                For i = 0 To DT.Rows.Count - 1

                    Try

                        If Tracciabilita_Impianti = 1 Then

                            Id_Agenda = DT.Rows(i).Item("Id_Agenda")

                            Str_Progetto_Nome = ""
                            Str_Veg_Des = ""
                            Str_Cul_Des = ""
                            Str_Grfi_Des = ""
                            Str_Grva_Des = ""
                            Str_Setup_Cod = ""
                            Str_Sup_Imp = ""
                            Str_Validita_Inizio_Impianto = ""
                            Str_Validita_Fine_Impianto = ""
                            Str_Validita_Inizio_Distinta = ""
                            Str_Validita_Fine_Distinta = ""

                            'DT_Impianti = NewCom_AccettazioneBeniDaDiversi_RifRaccoltaImpianti_Leggi(Server, Session, Page, _
                            '                            Piva, _
                            '                            0, _
                            '                            Id_Agenda, _
                            '                            , , , , , , )


                            DT_Impianti = ADD.RifImpiantiDaRaccolta_Leggi(Piva, _
                                                                            0, _
                                                                            Id_Agenda, _
                                                                            "", _
                                                                            0, _
                                                                            0, _
                                                                            "", "", _
                                                                            objParametri_Server)

                            If Not IsNothing(DT_Impianti) AndAlso DT_Impianti.Rows.Count <> 0 Then

                                For j = 0 To DT_Impianti.Rows.Count - 1

                                    Progetto_Nome = DT_Impianti.Rows(j).Item("Progetto_Nome")
                                    Veg_Des = DT_Impianti.Rows(j).Item("Veg_Des")
                                    Cul_Des = DT_Impianti.Rows(j).Item("Cul_Des")
                                    Grfi_Des = DT_Impianti.Rows(j).Item("Grfi_Des")
                                    Grva_Des = DT_Impianti.Rows(j).Item("Grva_Des")
                                    If DT_Impianti.Rows(j).Item("Setup_Cod") <> "-1" Then
                                        Setup_Cod = DT_Impianti.Rows(j).Item("Setup_Cod")
                                    Else
                                        Setup_Cod = ""
                                    End If
                                    Sup_Imp = DT_Impianti.Rows(j).Item("Sup_Imp")

                                    Validita_Inizio_Impianto = Sistema_ValiditaInizio(CDate(DT_Impianti.Rows(j).Item("Validita_Inizio_Impianto")))
                                    Validita_Fine_Impianto = Sistema_ValiditaFine(CDate(DT_Impianti.Rows(j).Item("Validita_Fine_Impianto")))
                                    Validita_Inizio_Distinta = Sistema_ValiditaInizio(CDate(DT_Impianti.Rows(j).Item("Validita_Inizio_Distinta")))
                                    Validita_Fine_Distinta = Sistema_ValiditaFine(CDate(DT_Impianti.Rows(j).Item("validita_Fine_Distinta")))

                                    If j <> DT_Impianti.Rows.Count - 1 Then
                                        Str_Progetto_Nome += Progetto_Nome + " - "
                                        Str_Veg_Des += Veg_Des + " - "
                                        Str_Cul_Des += Cul_Des + " - "
                                        Str_Grfi_Des += Grfi_Des + " - "
                                        Str_Grva_Des += Grva_Des + " - "
                                        Str_Setup_Cod += Setup_Cod + " - "
                                        Str_Sup_Imp += Sup_Imp + " - "
                                        Str_Validita_Inizio_Impianto += Validita_Inizio_Impianto + " - "
                                        Str_Validita_Fine_Impianto += Validita_Fine_Impianto + " - "
                                        Str_Validita_Inizio_Distinta += Validita_Inizio_Distinta + " - "
                                        Str_Validita_Fine_Distinta += Validita_Fine_Distinta + " - "
                                    Else
                                        Str_Progetto_Nome += Progetto_Nome
                                        Str_Veg_Des += Veg_Des
                                        Str_Cul_Des += Cul_Des
                                        Str_Grfi_Des += Grfi_Des
                                        Str_Grva_Des += Grva_Des
                                        Str_Setup_Cod += Setup_Cod
                                        Str_Sup_Imp += Sup_Imp
                                        Str_Validita_Inizio_Impianto += Validita_Inizio_Impianto
                                        Str_Validita_Fine_Impianto += Validita_Fine_Impianto
                                        Str_Validita_Inizio_Distinta += Validita_Inizio_Distinta
                                        Str_Validita_Fine_Distinta += Validita_Fine_Distinta
                                    End If

                                Next

                                'Str_Progetto_Nome = Mid(Str_Progetto_Nome, 1, Str_Progetto_Nome.Length - 3)
                                'Str_Veg_Des = Mid(Str_Veg_Des, 1, Str_Veg_Des.Length - 3)
                                'Str_Cul_Des = Mid(Str_Cul_Des, 1, Str_Cul_Des.Length - 3)
                                'Str_Validita_Inizio_Impianto = Mid(Str_Validita_Inizio_Impianto, 1, Str_Validita_Inizio_Impianto.Length - 3)
                                'Str_Validita_Fine_Impianto = Mid(Str_Validita_Fine_Impianto, 1, Str_Validita_Fine_Impianto.Length - 3)
                                'Str_Validita_Inizio_Distinta = Mid(Str_Validita_Inizio_Distinta, 1, Str_Validita_Inizio_Distinta.Length - 3)
                                'Str_Validita_Fine_Distinta = Mid(Str_Validita_Fine_Distinta, 1, Str_Validita_Fine_Distinta.Length - 3)

                                With DT.Rows(i)
                                    .Item("Str_Progetto_Nome") = Str_Progetto_Nome
                                    .Item("Str_Veg_Des") = Str_Veg_Des
                                    .Item("Str_Cul_Des") = Str_Cul_Des
                                    .Item("Str_Grfi_Des") = Str_Grfi_Des
                                    .Item("Str_Grva_Des") = Str_Grva_Des
                                    .Item("Str_Setup_Cod") = Str_Setup_Cod
                                    .Item("Str_Sup_Imp") = Str_Sup_Imp
                                    .Item("Str_Validita_Inizio_Impianto") = Str_Validita_Inizio_Impianto
                                    .Item("Str_Validita_Fine_Impianto") = Str_Validita_Fine_Impianto
                                    .Item("Str_Validita_Inizio_Distinta") = Str_Validita_Inizio_Distinta
                                    .Item("Str_Validita_Fine_Distinta") = Str_Validita_Fine_Distinta
                                End With

                            End If

                        End If


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

                            .Item("peso_totale") = objADDFun.Calcola_PesoTotale(1, .Item("peso_lordo"), .Item("tara_veicolo"))

                            'Calcola_PesoTotale(1, _
                            '                    , _
                            '                    .Item("tara_imballi"), _
                            '                    .Item("Qta_Raccolta"), _
                            '                    .Item("tara_veicolo"), _
                            '                    .Item("tara_imballi"), _
                            '                    .Item("peso_netto"), _
                            '                    .Item("peso_totale"))

                            objADDFun.Calcola_NettoPagamento(.Item("peso_netto"), _
                                                    .Item("Variazione_Raccolta"), _
                                                    .Item("Udm_Cod_Raccolta"), _
                                                    .Item("Degrado"), _
                                                    .Item("netto_Pagamento"))

                            .Item("Degrado_Perc") = .Item("Variazione_Raccolta")


                            objADDFun.Ricava_Specie_Varieta_FRG(CStr(.Item("Mat_Des_Raccolta")), .Item("Descr_Specie"), .Item("Descr_Varieta"))



                            If Not Hash_Varieta.Contains(.Item("Codice_Prodotto")) Then
                                '-------------------------------------
                                '-------- MAT_COD NON PRESENTE -------
                                '-------------------------------------

                                'codice = mat_cod; valore = totale netto a pagamento di quel prodotto
                                Hash_Varieta.Add(CInt(.Item("Codice_Prodotto")), CDbl(.Item("netto_Pagamento")))

                                'If i <> 0 Then
                                ''aggiungo una colonna per ogni mat_cod
                                Col += 1
                                ReDim Preserve Mtx_Varieta(NUM_RIGHE, Col)
                                'End If

                                Mtx_Varieta(R_COD_PROD, Col) = CInt(.Item("Codice_Prodotto"))
                                Mtx_Varieta(R_SPECIE, Col) = CStr(.Item("Descr_Specie"))
                                Mtx_Varieta(R_VARIETA, Col) = CStr(.Item("Descr_Varieta"))

                                Select Case CStr(.Item("Calibro_Des")).ToLower

                                    Case CStr("a").ToLower
                                        .Item("A") = Format(CDbl(.Item("netto_Pagamento")), "#,###,##0")
                                        Mtx_Varieta(C_A, Col) = CDbl(.Item("netto_Pagamento"))

                                    Case CStr("b").ToLower
                                        .Item("B") = Format(CDbl(.Item("netto_Pagamento")), "#,###,##0")
                                        Mtx_Varieta(C_B, Col) = CDbl(.Item("netto_Pagamento"))

                                    Case CStr("c").ToLower
                                        .Item("C") = Format(CDbl(.Item("netto_Pagamento")), "#,###,##0")
                                        Mtx_Varieta(C_C, Col) = CDbl(.Item("netto_Pagamento"))

                                    Case CStr("d").ToLower
                                        .Item("D") = Format(CDbl(.Item("netto_Pagamento")), "#,###,##0")
                                        Mtx_Varieta(C_D, Col) = CDbl(.Item("netto_Pagamento"))

                                    Case CStr("crema").ToLower
                                        .Item("CREMA") = Format(CDbl(.Item("netto_Pagamento")), "#,###,##0")
                                        Mtx_Varieta(C_CREMA, Col) = CDbl(.Item("netto_Pagamento"))

                                    Case CStr("FOGLIA FOGLIA").ToLower
                                        .Item("FOGLIA") = Format(CDbl(.Item("netto_Pagamento")), "#,###,##0")
                                        Mtx_Varieta(C_FOGLIA_FOGLIA, Col) = CDbl(.Item("netto_Pagamento"))

                                    Case CStr("SECONDO SFALCIO").ToLower
                                        .Item("S_SFALCIO") = Format(CDbl(.Item("netto_Pagamento")), "#,###,##0")
                                        Mtx_Varieta(C_SECONDO_SFALCIO, Col) = CDbl(.Item("netto_Pagamento"))

                                    Case CStr("SS").ToLower, CStr("S.S.").ToLower, CStr("S.S").ToLower
                                        .Item("SUB_STANDARD") = Format(CDbl(.Item("netto_Pagamento")), "#,###,##0")
                                        Mtx_Varieta(C_SUB_STANDARD, Col) = CDbl(.Item("netto_Pagamento"))

                                    Case CStr("POMODORO").ToLower
                                        .Item("POMODORO") = Format(CDbl(.Item("netto_Pagamento")), "#,###,##0")
                                        Mtx_Varieta(C_POMODORO, Col) = CDbl(.Item("netto_Pagamento"))

                                    Case CStr("-80").ToLower
                                        .Item("_80") = Format(CDbl(.Item("netto_Pagamento")), "#,###,##0")
                                        Mtx_Varieta(GT_80, Col) = CDbl(.Item("netto_Pagamento"))

                                    Case CStr("80-85").ToLower
                                        .Item("_80_85") = Format(CDbl(.Item("netto_Pagamento")), "#,###,##0")
                                        Mtx_Varieta(GT_80_85, Col) = CDbl(.Item("netto_Pagamento"))

                                    Case CStr("86-90").ToLower
                                        .Item("_86_90") = Format(CDbl(.Item("netto_Pagamento")), "#,###,##0")
                                        Mtx_Varieta(GT_86_90, Col) = CDbl(.Item("netto_Pagamento"))

                                    Case CStr("91-95").ToLower
                                        .Item("_91_95") = Format(CDbl(.Item("netto_Pagamento")), "#,###,##0")
                                        Mtx_Varieta(GT_91_95, Col) = CDbl(.Item("netto_Pagamento"))

                                    Case CStr("96-100").ToLower
                                        .Item("_96_100") = Format(CDbl(.Item("netto_Pagamento")), "#,###,##0")
                                        Mtx_Varieta(GT_96_100, Col) = CDbl(.Item("netto_Pagamento"))

                                    Case CStr("101-110").ToLower
                                        .Item("_101_110") = Format(CDbl(.Item("netto_Pagamento")), "#,###,##0")
                                        Mtx_Varieta(GT_101_110, Col) = CDbl(.Item("netto_Pagamento"))

                                    Case CStr("111-120").ToLower
                                        .Item("_111_120") = Format(CDbl(.Item("netto_Pagamento")), "#,###,##0")
                                        Mtx_Varieta(GT_111_120, Col) = CDbl(.Item("netto_Pagamento"))

                                    Case CStr("121-130").ToLower
                                        .Item("_121_130") = Format(CDbl(.Item("netto_Pagamento")), "#,###,##0")
                                        Mtx_Varieta(GT_121_130, Col) = CDbl(.Item("netto_Pagamento"))

                                    Case CStr("131-140").ToLower
                                        .Item("_131_140") = Format(CDbl(.Item("netto_Pagamento")), "#,###,##0")
                                        Mtx_Varieta(GT_131_140, Col) = CDbl(.Item("netto_Pagamento"))

                                    Case CStr("+140").ToLower
                                        .Item("_140_") = Format(CDbl(.Item("netto_Pagamento")), "#,###,##0")
                                        Mtx_Varieta(GT_140, Col) = CDbl(.Item("netto_Pagamento"))

                                End Select

                            Else
                                '-------------------------------------
                                '-------- MAT_COD GIA' PRESENTE ------
                                '-------------------------------------
                                Tot_Netto_Varieta = Hash_Varieta(CInt(.Item("Codice_Prodotto")))
                                Hash_Varieta(CInt(.Item("Codice_Prodotto"))) = Tot_Netto_Varieta + CDbl(.Item("netto_Pagamento"))

                                Select Case CStr(.Item("Calibro_Des")).ToLower

                                    Case CStr("a").ToLower
                                        .Item("A") = Format(CDbl(.Item("netto_Pagamento")), "#,###,##0")
                                        Tot_Netto_Calibro_Varieta = Mtx_Varieta(C_A, Col)
                                        Mtx_Varieta(C_A, Col) = CDbl(.Item("netto_Pagamento")) + Tot_Netto_Calibro_Varieta

                                    Case CStr("b").ToLower
                                        .Item("B") = Format(CDbl(.Item("netto_Pagamento")), "#,###,##0")
                                        Tot_Netto_Calibro_Varieta = Mtx_Varieta(C_B, Col)
                                        Mtx_Varieta(C_B, Col) = CDbl(.Item("netto_Pagamento")) + Tot_Netto_Calibro_Varieta

                                    Case CStr("c").ToLower
                                        .Item("C") = Format(CDbl(.Item("netto_Pagamento")), "#,###,##0")
                                        Tot_Netto_Calibro_Varieta = Mtx_Varieta(C_C, Col)
                                        Mtx_Varieta(C_C, Col) = CDbl(.Item("netto_Pagamento")) + Tot_Netto_Calibro_Varieta

                                    Case CStr("d").ToLower
                                        .Item("D") = Format(CDbl(.Item("netto_Pagamento")), "#,###,##0")
                                        Tot_Netto_Calibro_Varieta = Mtx_Varieta(C_D, Col)
                                        Mtx_Varieta(C_D, Col) = CDbl(.Item("netto_Pagamento")) + Tot_Netto_Calibro_Varieta

                                    Case CStr("crema").ToLower
                                        .Item("CREMA") = Format(CDbl(.Item("netto_Pagamento")), "#,###,##0")
                                        Tot_Netto_Calibro_Varieta = Mtx_Varieta(C_CREMA, Col)
                                        Mtx_Varieta(C_CREMA, Col) = CDbl(.Item("netto_Pagamento")) + Tot_Netto_Calibro_Varieta

                                    Case CStr("FOGLIA FOGLIA").ToLower
                                        .Item("FOGLIA") = Format(CDbl(.Item("netto_Pagamento")), "#,###,##0")
                                        Tot_Netto_Calibro_Varieta = Mtx_Varieta(C_FOGLIA_FOGLIA, Col)
                                        Mtx_Varieta(C_FOGLIA_FOGLIA, Col) = CDbl(.Item("netto_Pagamento")) + Tot_Netto_Calibro_Varieta

                                    Case CStr("SECONDO SFALCIO").ToLower
                                        .Item("S_SFALCIO") = Format(CDbl(.Item("netto_Pagamento")), "#,###,##0")
                                        Tot_Netto_Calibro_Varieta = Mtx_Varieta(C_SECONDO_SFALCIO, Col)
                                        Mtx_Varieta(C_SECONDO_SFALCIO, Col) = CDbl(.Item("netto_Pagamento")) + Tot_Netto_Calibro_Varieta

                                    Case CStr("SS").ToLower, CStr("S.S.").ToLower, CStr("S.S").ToLower
                                        .Item("SUB_STANDARD") = Format(CDbl(.Item("netto_Pagamento")), "#,###,##0")
                                        Tot_Netto_Calibro_Varieta = Mtx_Varieta(C_SUB_STANDARD, Col)
                                        Mtx_Varieta(C_SUB_STANDARD, Col) = CDbl(.Item("netto_Pagamento")) + Tot_Netto_Calibro_Varieta

                                    Case CStr("POMODORO").ToLower
                                        .Item("POMODORO") = Format(CDbl(.Item("netto_Pagamento")), "#,###,##0")
                                        Tot_Netto_Calibro_Varieta = Mtx_Varieta(C_POMODORO, Col)
                                        Mtx_Varieta(C_POMODORO, Col) = CDbl(.Item("netto_Pagamento")) + Tot_Netto_Calibro_Varieta

                                    Case CStr("-80").ToLower
                                        .Item("_80") = Format(CDbl(.Item("netto_Pagamento")), "#,###,##0")
                                        Tot_Netto_Calibro_Varieta = Mtx_Varieta(GT_80, Col)
                                        Mtx_Varieta(GT_80, Col) = CDbl(.Item("netto_Pagamento")) + Tot_Netto_Calibro_Varieta

                                    Case CStr("80-85").ToLower
                                        .Item("_80_85") = Format(CDbl(.Item("netto_Pagamento")), "#,###,##0")
                                        Tot_Netto_Calibro_Varieta = Mtx_Varieta(GT_80_85, Col)
                                        Mtx_Varieta(GT_80_85, Col) = CDbl(.Item("netto_Pagamento")) + Tot_Netto_Calibro_Varieta

                                    Case CStr("86-90").ToLower
                                        .Item("_86_90") = Format(CDbl(.Item("netto_Pagamento")), "#,###,##0")
                                        Tot_Netto_Calibro_Varieta = Mtx_Varieta(GT_86_90, Col)
                                        Mtx_Varieta(GT_86_90, Col) = CDbl(.Item("netto_Pagamento")) + Tot_Netto_Calibro_Varieta

                                    Case CStr("91-95").ToLower
                                        .Item("_91_95") = Format(CDbl(.Item("netto_Pagamento")), "#,###,##0")
                                        Tot_Netto_Calibro_Varieta = Mtx_Varieta(GT_91_95, Col)
                                        Mtx_Varieta(GT_91_95, Col) = CDbl(.Item("netto_Pagamento")) + Tot_Netto_Calibro_Varieta

                                    Case CStr("96-100").ToLower
                                        .Item("_96_100") = Format(CDbl(.Item("netto_Pagamento")), "#,###,##0")
                                        Tot_Netto_Calibro_Varieta = Mtx_Varieta(GT_96_100, Col)
                                        Mtx_Varieta(GT_96_100, Col) = CDbl(.Item("netto_Pagamento")) + Tot_Netto_Calibro_Varieta

                                    Case CStr("101-110").ToLower
                                        .Item("_101_110") = Format(CDbl(.Item("netto_Pagamento")), "#,###,##0")
                                        Tot_Netto_Calibro_Varieta = Mtx_Varieta(GT_101_110, Col)
                                        Mtx_Varieta(GT_101_110, Col) = CDbl(.Item("netto_Pagamento")) + Tot_Netto_Calibro_Varieta

                                    Case CStr("111-120").ToLower
                                        .Item("_111_120") = Format(CDbl(.Item("netto_Pagamento")), "#,###,##0")
                                        Tot_Netto_Calibro_Varieta = Mtx_Varieta(GT_111_120, Col)
                                        Mtx_Varieta(GT_111_120, Col) = CDbl(.Item("netto_Pagamento")) + Tot_Netto_Calibro_Varieta

                                    Case CStr("121-130").ToLower
                                        .Item("_121_130") = Format(CDbl(.Item("netto_Pagamento")), "#,###,##0")
                                        Tot_Netto_Calibro_Varieta = Mtx_Varieta(GT_121_130, Col)
                                        Mtx_Varieta(GT_121_130, Col) = CDbl(.Item("netto_Pagamento")) + Tot_Netto_Calibro_Varieta

                                    Case CStr("131-140").ToLower
                                        .Item("_131_140") = Format(CDbl(.Item("netto_Pagamento")), "#,###,##0")
                                        Tot_Netto_Calibro_Varieta = Mtx_Varieta(GT_131_140, Col)
                                        Mtx_Varieta(GT_131_140, Col) = CDbl(.Item("netto_Pagamento")) + Tot_Netto_Calibro_Varieta

                                    Case CStr("+140").ToLower
                                        .Item("_140_") = Format(CDbl(.Item("netto_Pagamento")), "#,###,##0")
                                        Tot_Netto_Calibro_Varieta = Mtx_Varieta(GT_140, Col)
                                        Mtx_Varieta(GT_140, Col) = CDbl(.Item("netto_Pagamento")) + Tot_Netto_Calibro_Varieta

                                End Select

                            End If

                        End With

                    Catch ex As Exception
                        Riga = New HtmlTableRow
                        Riga.Cells.Add(New HtmlTableCell)
                        Riga.Cells(0).ColSpan = Numero_Colonne
                        Riga.Cells(0).InnerHtml = "Lettura dei dati: " + ex.Message
                        Me.TableExcel.Rows.Add(Riga)
                    End Try

                Next

                If Tracciabilita_Impianti = 0 Then
                    DT.Columns.Remove("Str_Progetto_Nome")
                    DT.Columns.Remove("Str_Veg_Des")
                    DT.Columns.Remove("Str_Cul_Des")
                    DT.Columns.Remove("Str_Grfi_Des")
                    DT.Columns.Remove("Str_Grva_Des")
                    DT.Columns.Remove("Str_Setup_Cod")
                    DT.Columns.Remove("Str_Sup_Imp")
                    DT.Columns.Remove("Str_Validita_Inizio_Impianto")
                    DT.Columns.Remove("Str_Validita_Fine_Impianto")
                    DT.Columns.Remove("Str_Validita_Inizio_Distinta")
                    DT.Columns.Remove("Str_Validita_Fine_Distinta")
                End If

                DT.Columns.Remove("Id_Agenda")
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
                DT.Columns.Remove("Peso")
                'DT.Columns.Remove("Tara_Veicolo")
                DT.Columns.Remove("Tipo_Peso")
                DT.Columns.Remove("Codice_Prodotto")

                DT.Columns.Remove("Lotto_Raccolta")
                DT.Columns.Remove("Cal_Cod_Raccolta")
                DT.Columns.Remove("Udm_Cod_Raccolta")
                DT.Columns.Remove("Qta_Raccolta")
                DT.Columns.Remove("Variazione_Raccolta")
                DT.Columns.Remove("Tara")
                DT.Columns.Remove("Mat_Des_Raccolta")
                'DT.Columns.Remove("Degrado")
                'DT.Columns.Remove("tara_Imballi")
                DT.Columns.Remove("calibro_des")

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

        Crea_EXCEL(DT, Mtx_Varieta, Hash_Varieta)

        'End If



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

            'ElaboraCellaHTML(Me.TableExcel.Rows(0).Cells(0), 2, "", "", "Yellow", "left", "middle")
            'Me.TableExcel.Rows(0).Cells(0).InnerHtml = "Anagrafica Contatti"
            'Me.TableExcel.Rows(0).Cells(0).Style.Item("font-weight") = "bold"
            'Me.TableExcel.Rows(0).Cells(0).Style.Item("font-size") = "18px"
            'Me.TableExcel.Rows(0).Cells(0).Style.Item("vertical-align") = "middle"
            'Me.TableExcel.Rows(0).Cells(0).Style.Item("border-top-width") = "1px"

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
                        Case CStr("Piva_Conferente").ToLower
                            Riga.Cells(k).InnerHtml = "P.IVA<br/>Conferente"
                        Case CStr("Piva_Produttore").ToLower
                            Riga.Cells(k).InnerHtml = "P.IVA<br/>Produttore"
                        Case CStr("RagSoc_Produttore").ToLower
                            Riga.Cells(k).InnerHtml = "Produttore"
                        Case CStr("Piva_Coop1").ToLower
                            Riga.Cells(k).InnerHtml = "P.IVA<br/>Cooperativa"
                        Case CStr("RagSoc_Coop1").ToLower
                            Riga.Cells(k).InnerHtml = "Cooperativa"
                        Case CStr("Piva_Coop2").ToLower
                            Riga.Cells(k).InnerHtml = "P.IVA seconda<br/>Cooperativa"
                        Case CStr("RagSoc_Coop2").ToLower
                            Riga.Cells(k).InnerHtml = "Seconda<br/>Cooperativa"
                        Case CStr("peso_totale").ToLower
                            Riga.Cells(k).InnerHtml = "Peso<br/>Totale"
                        Case CStr("tara_veicolo").ToLower
                            Riga.Cells(k).InnerHtml = "Tara<br/>Veicolo"
                        Case CStr("peso_lordo").ToLower
                            Riga.Cells(k).InnerHtml = "Peso<br/>Lordo"
                        Case CStr("tara_imballi").ToLower
                            Riga.Cells(k).InnerHtml = "Tara<br/>Imballi"
                        Case CStr("peso_netto").ToLower
                            Riga.Cells(k).InnerHtml = "Peso<br/>Netto"
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
                        Case CStr("Codice_Conferente").ToLower
                            Riga.Cells(k).InnerHtml = "Codice<br/>Conferente"
                        Case CStr("RagSoc_Conferente").ToLower
                            Riga.Cells(k).InnerHtml = "Conferente"
                        Case CStr("Cod_Articolo_Raccolta").ToLower
                            Riga.Cells(k).InnerHtml = "Codice<br/>Specie"
                        Case CStr("Descr_Specie").ToLower
                            Riga.Cells(k).InnerHtml = "Specie"
                        Case CStr("Descr_Varieta").ToLower
                            Riga.Cells(k).InnerHtml = "Varieta"
                        Case CStr("Codice_Prodotto").ToLower
                            Riga.Cells(k).InnerHtml = "Codice Gias<br/>Prodotto"
                        Case CStr("punteggio").ToLower
                            Riga.Cells(k).InnerHtml = "Indice<br/>Qualitat."
                        Case CStr("Netto_Pagamento").ToLower
                            Riga.Cells(k).InnerHtml = "Netto a<br/>Pagamento"
                        Case CStr("a").ToLower
                            Riga.Cells(k).InnerHtml = "A"
                        Case CStr("b").ToLower
                            Riga.Cells(k).InnerHtml = "B"
                        Case CStr("c").ToLower
                            Riga.Cells(k).InnerHtml = "C"
                        Case CStr("d").ToLower
                            Riga.Cells(k).InnerHtml = "D"
                        Case CStr("crema").ToLower
                            Riga.Cells(k).InnerHtml = "CREMA"
                        Case CStr("FOGLIA").ToLower
                            Riga.Cells(k).InnerHtml = "FOGLIA<BR/>FOGLIA"
                        Case CStr("S_SFALCIO").ToLower
                            Riga.Cells(k).InnerHtml = "SECONDO<BR/>SFALCIO"
                        Case CStr("SUB_STANDARD").ToLower
                            Riga.Cells(k).InnerHtml = "SUB<BR/>STANDARD"
                        Case CStr("POMODORO").ToLower
                            Riga.Cells(k).InnerHtml = "POMODORO"
                        Case CStr("crema").ToLower
                            Riga.Cells(k).InnerHtml = "CREMA"
                        Case CStr("_80").ToLower
                            Riga.Cells(k).InnerHtml = "-80"
                        Case CStr("_80_85").ToLower
                            Riga.Cells(k).InnerHtml = "80-85"
                        Case CStr("_86_90").ToLower
                            Riga.Cells(k).InnerHtml = "86-90"
                        Case CStr("_91_95").ToLower
                            Riga.Cells(k).InnerHtml = "91-95"
                        Case CStr("_96_100").ToLower
                            Riga.Cells(k).InnerHtml = "96-100"
                        Case CStr("_101_110").ToLower
                            Riga.Cells(k).InnerHtml = "101-110"
                        Case CStr("_111_120").ToLower
                            Riga.Cells(k).InnerHtml = "111-120"
                        Case CStr("_121_130").ToLower
                            Riga.Cells(k).InnerHtml = "121-130"
                        Case CStr("_131_140").ToLower
                            Riga.Cells(k).InnerHtml = "131-140"
                        Case CStr("_140_").ToLower
                            Riga.Cells(k).InnerHtml = "140+"
                        Case CStr("Str_Progetto_Nome").ToLower
                            Riga.Cells(k).InnerHtml = "Lotto Impianti"
                        Case CStr("Str_Veg_Des").ToLower
                            Riga.Cells(k).InnerHtml = "Specie Gias<br/>Impianti"
                        Case CStr("Str_Cul_Des").ToLower
                            Riga.Cells(k).InnerHtml = "Varietà Gias<br/>Impianti"
                        Case CStr("Str_Grfi_Des").ToLower
                            Riga.Cells(k).InnerHtml = "Finalità<br/>Impianti"
                        Case CStr("Str_Grva_Des").ToLower
                            Riga.Cells(k).InnerHtml = "Tipologia Varietale<br/>Impianti"
                        Case CStr("Str_Setup_Cod").ToLower
                            Riga.Cells(k).InnerHtml = "Sem/Trap"
                        Case CStr("Str_Sup_Imp").ToLower
                            Riga.Cells(k).InnerHtml = "Superficie<br/>Impianti"
                        Case CStr("Str_Validita_Inizio_Impianto").ToLower
                            Riga.Cells(k).InnerHtml = "Data Inizio<br/>Impianti"
                        Case CStr("Str_Validita_Fine_Impianto").ToLower
                            Riga.Cells(k).InnerHtml = "Data Fine<br/>Impianti"
                        Case CStr("Str_Validita_Inizio_Distinta").ToLower
                            Riga.Cells(k).InnerHtml = "Data Inizio<br/>Distinta Impianti"
                        Case CStr("Str_Validita_Fine_Distinta").ToLower
                            Riga.Cells(k).InnerHtml = "Data Fine<br/>Distinta Impianti"

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

        Try

            If Not IsNothing(Mtx_Varieta) Then

                '------------------------------------------
                '------------- RIGHE VUOTE ----------------
                '------------------------------------------

                Riga = New HtmlTableRow
                For j = PRIMA_CELLA_INIZIO To ULTIMA_CELLA_CALIBRI
                    Riga.Cells.Add(New HtmlTableCell)
                    Riga.Cells(j).InnerHtml = "&nbsp;"
                Next
                Me.TableExcel.Rows.Add(Riga)

                Riga = New HtmlTableRow
                For j = PRIMA_CELLA_INIZIO To ULTIMA_CELLA_CALIBRI
                    Riga.Cells.Add(New HtmlTableCell)
                    Riga.Cells(j).InnerHtml = "&nbsp;"
                Next
                Me.TableExcel.Rows.Add(Riga)

                '------------------------------------------
                '------------- INTESTAZIONE ---------------
                '------------------------------------------
                Riga = New HtmlTableRow

                For j = PRIMA_CELLA_INIZIO To ULTIMA_CELLA_INIZIO
                    Riga.Cells.Add(New HtmlTableCell)
                    Riga.Cells(j).InnerHtml = "&nbsp;"
                Next

                For k = PRIMA_CELLA_CALIBRI To ULTIMA_CELLA_CALIBRI

                    Riga.Cells.Add(New HtmlTableCell)
                    AgronicaCoreDataProvider.UtilityProvider.ElaboraCellaHTML(Riga.Cells(k), 2, "", "", "Gainsboro", "center", "middle")
                    Riga.Cells(k).Style.Item("vertical-align") = "middle"
                    Riga.Cells(k).Style.Item("font-weight") = "bold"
                    Riga.Cells(k).Style.Item("font-size") = "12px"

                    Select Case k
                        Case N_COL_SPECIE
                            Riga.Cells(k).InnerHtml = "Specie"
                        Case N_COL_VARIETA
                            Riga.Cells(k).InnerHtml = "Varietà"
                        Case N_COL_NETTO_PAG
                            Riga.Cells(k).InnerHtml = "Tot. Netto<br/>Pagamento"
                        Case N_COL_A
                            Riga.Cells(k).InnerHtml = "Tot. A"
                        Case N_COL_B
                            Riga.Cells(k).InnerHtml = "Tot. B"
                        Case N_COL_C
                            Riga.Cells(k).InnerHtml = "Tot. C"
                        Case N_COL_D
                            Riga.Cells(k).InnerHtml = "Tot. D"
                        Case N_COL_CREMA
                            Riga.Cells(k).InnerHtml = "Tot. CREMA"
                        Case N_COL_FOGLIA
                            Riga.Cells(k).InnerHtml = "Tot. FOGLIA<br/>FOGLIA"
                        Case N_COL_SEC_SFALCIO
                            Riga.Cells(k).InnerHtml = "Tot. SECONDO<br/>SFALCIO"
                        Case N_COL_SS
                            Riga.Cells(k).InnerHtml = "Tot. SUB<br/>STANDARD"
                        Case N_COL_POMO
                            Riga.Cells(k).InnerHtml = "Tot. POMODORO"
                        Case N_COL_80
                            Riga.Cells(k).InnerHtml = "Tot. -80"
                        Case N_COL_80_85
                            Riga.Cells(k).InnerHtml = "Tot. 80-85"
                        Case N_COL_86_90
                            Riga.Cells(k).InnerHtml = "Tot. 86-90"
                        Case N_COL_91_95
                            Riga.Cells(k).InnerHtml = "Tot. 91-95"
                        Case N_COL_96_100
                            Riga.Cells(k).InnerHtml = "Tot. 96-100"
                        Case N_COL_101_110
                            Riga.Cells(k).InnerHtml = "Tot. 101-110"
                        Case N_COL_111_120
                            Riga.Cells(k).InnerHtml = "Tot. 111-120"
                        Case N_COL_121_130
                            Riga.Cells(k).InnerHtml = "Tot. 121-130"
                        Case N_COL_131_140
                            Riga.Cells(k).InnerHtml = "Tot. 131-140"
                        Case N_COL_140
                            Riga.Cells(k).InnerHtml = "Tot 140+"
                    End Select

                Next

                Me.TableExcel.Rows.Add(Riga)

                '----------------------------------
                '------- FINE INTESTAZIONE --------
                '----------------------------------

                '----------------------------------
                '------ RIEMPIMENTO TABELLA -------
                '----------------------------------

                'scorro le colonne della matrice (che diventano le righe della tabella)
                For i = 0 To UBound(Mtx_Varieta, 2)

                    Riga = New HtmlTableRow

                    For j = PRIMA_CELLA_INIZIO To ULTIMA_CELLA_INIZIO
                        Riga.Cells.Add(New HtmlTableCell)
                        Riga.Cells(j).InnerHtml = "&nbsp;"
                    Next

                    'scorro le righe della matrice (che diventano le colonne della tabella)
                    For j = PRIMA_CELLA_CALIBRI To ULTIMA_CELLA_CALIBRI '(righe di calibri)

                        'j= indice della tabella html
                        'z indice riga matrice (parte da 0)
                        z = j - PRIMA_CELLA_CALIBRI - 3 'indice celle html - 8 celle - le 3 righe gestite a mano

                        'aggiungo la cella
                        Riga.Cells.Add(New HtmlTableCell)
                        Riga.Cells(j).Style.Item("text-align") = "center"
                        Riga.Cells(j).Style.Item("vertical-align") = "middle"

                        Select Case j

                            Case N_COL_SPECIE
                                'specie
                                Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(Mtx_Varieta(R_SPECIE, i)), Mtx_Varieta(R_SPECIE, i), "&nbsp;")
                            Case N_COL_VARIETA
                                'varietà
                                Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(Mtx_Varieta(R_VARIETA, i)), Mtx_Varieta(R_VARIETA, i), "&nbsp;")
                            Case N_COL_NETTO_PAG
                                'tot netto pagamento
                                Mat_Cod = Mtx_Varieta(R_COD_PROD, i)
                                Riga.Cells(j).InnerHtml = CStr(Hash_Varieta(Mat_Cod))
                            Case Else
                                'calibri
                                Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(Mtx_Varieta(z, i)), Mtx_Varieta(z, i), "&nbsp;")

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
            Riga.Cells(0).InnerHtml = "Creazione Excel Riepilogo Varietà: " + ex.Message
            Me.TableExcel.Rows.Add(Riga)

        End Try

    End Sub




End Class

