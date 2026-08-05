Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.Agro_Math
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Bolle_Conf_XLS
    Inherits System.Web.UI.Page

    Protected WithEvents TableExcel As System.Web.UI.HtmlControls.HtmlTable


#Region " Bolle_Conf_XLS "

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
    Dim Cod_RisUm As Integer
    Dim Codice_Conferente As String
    Dim Cod_Articolo As String
    Dim Codice_Esterno As String
    Dim FiltroAggConferenti As String
    Dim FiltroAggArticoli As String
    Dim Piva As String
    Dim Piva_Produttore, Piva_Coop1, Piva_Coop2 As String
    Dim Mat_Cod, Veg_Cod, Cul_Cod As Integer
    Dim Sa_Cod As Integer
    Dim RagSoc_Impresa As String
    Dim Descr_Specie As String
    Dim Mat_Des As String
    Dim Tracciabilita_Impianti As Integer
    'Dim GestCodice_0Art_1Est As Integer
    Dim Flag01_GestioneCodEsterno As Integer
    Dim QS_ti As String

    '-------------------------------------------
    '---------- EXCEL ------------
    Const PRIMA_CELLA_EXCEL As Integer = 0
    Dim ULTIMA_CELLA_EXCEL As Integer = 0
    Dim PRIMA_CELLA_CALIBRI As Integer = 0
    Dim ULTIMA_CELLA_CALIBRI As Integer = 0

    '--------------------------------------
    '----------- MATRICE ------------
    Const R_MTX_COD_RAGGR As Integer = 0
    Const R_MTX_MAT_COD As Integer = 1
    Const R_MTX_VEG_COD As Integer = 2
    Const R_MTX_CUL_COD As Integer = 3
    Const R_MTX_REG_COD As Integer = 4
    Const R_MTX_SPECIE As Integer = 5
    Const R_MTX_VARIETA As Integer = 6
    Const R_MTX_REG_DES As Integer = 7
    Const R_MTX_TOT_NETTO As Integer = 8
    Const R_MTX_NO_CLASS_QUAL As Integer = 9
    'dal 10 partono i calibri


    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri


    '#########################################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Dim Riga As HtmlTableRow
        Dim Numero_Colonne As Integer = 0
        Dim objImpreseR As New AgronicaCoreAnagrafeDAL.Imprese_Read

        'La Pagina deve essere visualizzata come un foglio Excel
        Response.ContentType = "application/vnd.ms-excel"
        Response.AddHeader("Content-Disposition", "inline; filename = Conferimenti.xls")

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

            Data_Da = CDate(Stringa_Decodifica(CStr(Request.QueryString("dd")),
                             AgroKey_EncoderDecoder,
                             Server)).ToShortDateString

            Data_A = CDate(Stringa_Decodifica(CStr(Request.QueryString("da")),
                             AgroKey_EncoderDecoder,
                             Server)).ToShortDateString

            Piva = Stringa_Decodifica(CStr(Request.QueryString("p")),
                                        AgroKey_EncoderDecoder,
                                        Server)


            RagSoc_Impresa = Stringa_Decodifica(CStr(Request.QueryString("rs")),
                                               AgroKey_EncoderDecoder,
                                               Server)

            If RagSoc_Impresa = "" Then
                RagSoc_Impresa = objImpreseR.RagSoc_from_Piva(Piva, objParametri_Server)
            End If

            Sa_Cod = CInt(Stringa_Decodifica(CStr(Request.QueryString("s")),
                                      AgroKey_EncoderDecoder,
                                      Server))

            'Descr_Centro = Stringa_Decodifica(CStr(Request.QueryString("sn")),
            '                                    AgroKey_EncoderDecoder,
            '                                    Server)

            Veg_Cod = CInt(Stringa_Decodifica(CStr(Request.QueryString("spe")),
                                  AgroKey_EncoderDecoder,
                                  Server))

            Cul_Cod = CInt(Stringa_Decodifica(CStr(Request.QueryString("var")),
                                  AgroKey_EncoderDecoder,
                                  Server))

            Mat_Cod = CInt(Stringa_Decodifica(CStr(Request.QueryString("m")),
                                  AgroKey_EncoderDecoder,
                                  Server))

            Cod_Articolo = Stringa_Decodifica(CStr(Request.QueryString("ca")),
                                AgroKey_EncoderDecoder,
                                Server)

            Flag01_GestioneCodEsterno = Stringa_Decodifica(CStr(Request.QueryString("fce")),
                                AgroKey_EncoderDecoder,
                                Server)

            'GestCodice_0Art_1Est = Stringa_Decodifica(CStr(Request.QueryString("opt")),
            '                  AgroKey_EncoderDecoder,
            '                  Server)

            Mat_Des = Stringa_Decodifica(CStr(Request.QueryString("dpr")),
                                AgroKey_EncoderDecoder,
                                Server)

            If Cod_Articolo = "0" Then
                Cod_Articolo = ""
                'il codice non è stato passato, 
                'perchè si vogliono cercare tutti
                'o è stato selezionato un range di codici
            End If

            QS_ti = Stringa_Decodifica(CStr(Request.QueryString("ti")),
                                               AgroKey_EncoderDecoder,
                                               Server)

            If QS_ti.ToLower = "false" Then
                Tracciabilita_Impianti = 0
            Else
                Tracciabilita_Impianti = 1
            End If

            'FiltroAggArticoli = Session("Str_Codici_Specie")

            'If FiltroAggArticoli <> "" Then
            '    'è stato selezionato un range di codici
            '    FiltroAggArticoli = " AND MP_Raccolta.Cod_Articolo IN " & FiltroAggArticoli
            'Else
            '    FiltroAggArticoli = ""
            'End If

            FiltroAggArticoli = Session("Str_Codici_Prodotto")

            Cod_RisUm = CInt(Stringa_Decodifica(CStr(Request.QueryString("ru")),
                                  AgroKey_EncoderDecoder,
                                  Server))



            Codice_Conferente = Stringa_Decodifica(CStr(Request.QueryString("cc")),
                                AgroKey_EncoderDecoder,
                                Server)

            If Codice_Conferente = "0" Then
                Codice_Conferente = ""
                'il codice non è stato passato, 
                'perchè si vogliono cercare tutti i conferenti
                'o il range di conferenti selezionato
            Else
                Codice_Conferente = CStr(Codice_Conferente)
            End If

            'old
            'FiltroAggConferenti = Session("Str_Codici_Conferenti")
            'If FiltroAggConferenti <> "" Then
            '    'è stato selezionato un range di codici
            '    FiltroAggConferenti = " AND Risorse_Umane_Conferenti.settore_Des IN " & FiltroAggConferenti
            'Else
            '    FiltroAggConferenti = ""
            'End If

            FiltroAggConferenti = Session("Str_CodRisUm_Conferenti")

            Piva_Produttore = Stringa_Decodifica(CStr(Request.QueryString("pp")),
                                    AgroKey_EncoderDecoder,
                                    Server)

            Piva_Coop1 = Stringa_Decodifica(CStr(Request.QueryString("pc1")),
                                           AgroKey_EncoderDecoder,
                                           Server)

            Piva_Coop2 = Stringa_Decodifica(CStr(Request.QueryString("pc2")),
                                         AgroKey_EncoderDecoder,
                                         Server)

        Catch ex As Exception
            Riga = New HtmlTableRow
            Riga.Cells.Add(New HtmlTableCell)
            Riga.Cells(0).ColSpan = Numero_Colonne
            Riga.Cells(0).InnerHtml = "Dati da querystring: " & ex.Message
            Me.TableExcel.Rows.Add(Riga)
        End Try


        '##############################################################
        '####################  LETTURA DATI ###########################
        '##############################################################

        Dim DT As DataTable = Nothing
        Dim i, j As Integer
        Dim DT_Impianti As DataTable = Nothing
        Dim Id_Agenda As Integer
        Dim Progetto_Nome, Veg_Des, Cul_Des, Grfi_Des, Grva_Des, Setup_Cod, Sup_Imp As String
        Dim Validita_Inizio_Impianto, Validita_Fine_Impianto, Validita_Inizio_Distinta, Validita_Fine_Distinta As String
        Dim Str_Progetto_Nome, Str_Veg_Des, Str_Cul_Des, Str_Grfi_Des, Str_Grva_Des, Str_Setup_Cod, Str_Sup_Imp As String
        Dim Str_Validita_Inizio_Impianto, Str_Validita_Fine_Impianto, Str_Validita_Inizio_Distinta, Str_Validita_Fine_Distinta As String

        Dim Tara_Imballi_Vuoti As Decimal
        Dim Tara_Imballi_Riga As Decimal
        Dim Num_Righe_Bolla As Integer

        Dim HT_Intest As New Hashtable
        Dim n_calibri As Integer = 0
        Dim sigla_calib As String
        Dim HT_OTab As New Hashtable
        Dim L_OTabParDes As New SortedSet(Of String)(New ClassQualComparer)

        'MATRICE:
        'una colonna per ogni codice_raggr (x fruttagel = mat_cod)
        'una riga per tot netto pagamento + nessuna class.qual. + calibro
        'La matrice ha nelle righe dei dati identificativi del raggruppamento e nelle colonne l'elenco dei raggruppamenti trovati. 
        'Il totale di righe risulta calcolabile ed è dato dalle prime 10 righe fisse indicate dalle costanti R_MTX_* seguite da tante righe quanti sono
        'i calibri configurati. Invece il numero di colonne è dinamico e risulta il numero di raggruppamenti di prodotti ricavati dalla query
        Dim Mtx_riepilogo As String(,)
        Dim HT_Calibri_RigheMtx As New Hashtable
        Dim HT_Calibri_ColExcel As New Hashtable
        Dim col_EX_calibro As Integer
        Dim i_Mtx_righe As Integer

        Try

            Dim objFF As New AgronicaCoreStampeDAL.FreshAndFood
            'Dim objHLP As New AgronicaCoreContabHLP.Contabilita
            Dim objConfFun As New AgronicaCoreStampeDAL.Conferimento_Funzioni
            Dim objADDFun As New AgronicaCoreStampeDAL.AccettazioneDaDiversi_Funzioni
            Dim DT_OTabPar As DataTable = Nothing

            FiltroAggArticoli = objConfFun.Ricava_FiltroMatCod_da_StrCodiciProdotto(FiltroAggArticoli)
            FiltroAggConferenti = objConfFun.Ricava_FiltroCodRisUm_da_StrCodiciContatto(FiltroAggConferenti)

            Try
                

                'If GestCodice_0Art_1Est = 0 Then
                '    codiceXriepilogo = "Cod_Articolo"
                'ElseIf GestCodice_0Art_1Est = 1 Then
                '    codiceXriepilogo = "Codice_Esterno"
                'End If

                Dim objOTab As New AgronicaCoreContabDAL.OModuli_Referenze_Config_Dettagli_R

                'da usare per aggiungere le colonne degli indici qual. e class. qual. sull'excel
                'non vanno letti gli imballi 
                Dim filtroAggOTab As String = " OTabelle.tabella_cod <> " & Agro_SQL_SaveText(enum_OTabelle.Imballaggio) & " " & vbCrLf

                If CInt(Session("ASG_ProgressivoGIAS")) = enum_CodiceGIAS_Clienti.Fruttagel Then
                    ' X FRUTTAGEL non vanno letti i param. qual del pomodoro perchè sono gestiti diversamente e su exceld edicato
                    filtroAggOTab &= " AND OModuli_Referenze_Config_Testata.ofiltro_veg_cod<>'|52|' "

                    ' X FRUTTAGEL ho due colonne aggiuntive rappresentanti i dati dei prodotti del loro gestionale
                    PRIMA_CELLA_CALIBRI = 36
                Else
                    PRIMA_CELLA_CALIBRI = 34
                End If

                DT_OTabPar = objOTab.OTabelleParametriDistinct(Piva,
                                                        filtroAggOTab,
                                                        objParametri_Server)


                'non posso usare la lista perchè mi serve anche il codice
                'Dim OTabelle As DataTable
                ''da usare per la query su campionature bolle
                'OTabelle = objOTab.OTabelleDistinct(Piva,
                '                                    " OTabelle.tabella_cod <> " & Agro_SQL_SaveText(enum_OTabelle.Imballaggio) & " ",
                '                                    objParametri_Server)
                'Dim OTab As List(Of String)
                'OTab = OTabelle.Rows.Cast(Of DataRow).Select(Function(dr) dr(0).ToString).ToList


                '§§§
                col_EX_calibro = PRIMA_CELLA_CALIBRI
                i_Mtx_righe = R_MTX_NO_CLASS_QUAL
                'Memorizzo la corrispondenza fra colonne dell'excel e righe della matrice per posizionare quest'ultima nel documento
                HT_Calibri_ColExcel.Add(col_EX_calibro - 4, "Specie")
                HT_Calibri_ColExcel.Add(col_EX_calibro - 3, "Varietà")
                HT_Calibri_ColExcel.Add(col_EX_calibro - 2, "Regolamento")
                HT_Calibri_ColExcel.Add(col_EX_calibro - 1, "Tot. Netto a Pag.")
                HT_Calibri_ColExcel.Add(col_EX_calibro, "Nessuna Class.Qual.")
                'Memorizzo a quale indice di riga della matrice corrisponde ogni calibro di modo da impostare 
                'correttamente la colonna "netto a pagamento"
                HT_Calibri_RigheMtx.Add("Nessuna_Class", i_Mtx_righe)

                For Each dr In DT_OTabPar.Rows
                    If Not HT_OTab.ContainsKey(dr("tabella_cod")) Then
                        HT_OTab.Add(dr("tabella_cod"), CStr("o" & dr("Tabella_Cod_Des")).ToLower)
                    End If
                    If dr("tabella_cod") = enum_OTabelle.Calibro Then
                        'La classe SortedSet fornisce una lista nella quale ogni elemento è univoco ed il metodo add restituisce true/false
                        'per indicare se ha aggiunto o meno l'elemento, senza lanciare eccezione
                        L_OTabParDes.Add(Trim(dr("sigla")))
                    End If
                    If Not HT_Intest.ContainsKey(dr("Tabella_Cod_Des").tolower) Then
                        HT_Intest.Add(dr("Tabella_Cod_Des").tolower, dr("Tabella_Des"))
                    End If

                Next

                '§§§
                For Each OTabParDes In L_OTabParDes
                    'Controllo superfluo in quanto gli elementi di L_OTabParDes sono univoci
                    'If Not HT_Calibri_RigheMtx.ContainsKey(OTabParDes) Then
                        n_calibri += 1
                        'quando elaboro il dt bolle dalla sigla del calibro devo ricavare l'indice di riga della matrice riepilogo
                        'e l'indice di colonna dell'excel
                        i_Mtx_righe += 1
                        col_EX_calibro += 1
                        HT_Calibri_ColExcel.Add(col_EX_calibro, OTabParDes)
                        HT_Calibri_RigheMtx.Add(OTabParDes, i_Mtx_righe)
                    'End If

                Next

                '§§§
                ULTIMA_CELLA_CALIBRI = PRIMA_CELLA_CALIBRI + n_calibri

            Catch ex As Exception
                Riga = New HtmlTableRow
                Riga.Cells.Add(New HtmlTableCell)
                Riga.Cells(0).ColSpan = Numero_Colonne
                Riga.Cells(0).InnerHtml = "Errore valorizzazione Mtx - HT: " & ex.Message
                Me.TableExcel.Rows.Add(Riga)
            End Try

            Try

                DT = objFF.Bolle_XLS(Piva,
                                    Sa_Cod,
                                    Cod_Articolo,
                                    Codice_Esterno,
                                    Codice_Conferente,
                                    Cod_RisUm,
                                    Piva_Produttore,
                                    Piva_Coop1,
                                    Piva_Coop2,
                                    Data_Da,
                                    Data_A,
                                    Mat_Cod,
                                    Veg_Cod,
                                    Cul_Cod,
                                    FiltroAggArticoli,
                                    FiltroAggConferenti,
                                    "",
                                    HT_OTab,
                                    L_OTabParDes,
                                    Tracciabilita_Impianti,
                                    CInt(Session("ASG_ProgressivoGIAS")),
                                    objParametri_Server)


            Catch ex As Exception
                Riga = New HtmlTableRow
                Riga.Cells.Add(New HtmlTableCell)
                Riga.Cells(0).ColSpan = Numero_Colonne
                Riga.Cells(0).InnerHtml = "Errore query di lettura: " & ex.Message
                Me.TableExcel.Rows.Add(Riga)
            End Try

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
                Dim curr_i_Mtx_righe As Integer
                Dim curr_i_Mtx_col As Integer
                Dim i_Mtx_Col As Integer = -1
                Dim Tot_Netto_Su_Raggruppamento As Decimal
                Dim Tot_Netto_x_Calibro As Decimal
                Dim CodiceXRaggruppamento, Descr_Specie, Descr_Varieta As String
                Dim HT_riepilogo = New Hashtable
                Dim HT_raggr_col = New Hashtable

                Mtx_riepilogo = New String(i_Mtx_righe, 0) {}

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

                            Try
                                DT_Impianti = objFF.RifImpiantiDaRaccolta_Leggi(Piva,
                                                                            0,
                                                                            Id_Agenda,
                                                                            "",
                                                                            0,
                                                                            0,
                                                                            "", "",
                                                                            objParametri_Server)

                            Catch ex As Exception
                                Riga = New HtmlTableRow
                                Riga.Cells.Add(New HtmlTableCell)
                                Riga.Cells(0).ColSpan = Numero_Colonne
                                Riga.Cells(0).InnerHtml = "Errore query di lettura tracciabilità Impianti sulla riga " & CStr(i + 1) & ": " & ex.Message
                                Me.TableExcel.Rows.Add(Riga)
                            End Try

                            If DT_Impianti IsNot Nothing AndAlso DT_Impianti.Rows.Count <> 0 Then

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
                                        Str_Progetto_Nome &= Progetto_Nome & " - "
                                        Str_Veg_Des &= Veg_Des & " - "
                                        Str_Cul_Des &= Cul_Des & " - "
                                        Str_Grfi_Des &= Grfi_Des & " - "
                                        Str_Grva_Des &= Grva_Des & " - "
                                        Str_Setup_Cod &= Setup_Cod & " - "
                                        Str_Sup_Imp &= Sup_Imp & " - "
                                        Str_Validita_Inizio_Impianto &= Validita_Inizio_Impianto & " - "
                                        Str_Validita_Fine_Impianto &= Validita_Fine_Impianto & " - "
                                        Str_Validita_Inizio_Distinta &= Validita_Inizio_Distinta & " - "
                                        Str_Validita_Fine_Distinta &= Validita_Fine_Distinta & " - "
                                    Else
                                        Str_Progetto_Nome &= Progetto_Nome
                                        Str_Veg_Des &= Veg_Des
                                        Str_Cul_Des &= Cul_Des
                                        Str_Grfi_Des &= Grfi_Des
                                        Str_Grva_Des &= Grva_Des
                                        Str_Setup_Cod &= Setup_Cod
                                        Str_Sup_Imp &= Sup_Imp
                                        Str_Validita_Inizio_Impianto &= Validita_Inizio_Impianto
                                        Str_Validita_Fine_Impianto &= Validita_Fine_Impianto
                                        Str_Validita_Inizio_Distinta &= Validita_Inizio_Distinta
                                        Str_Validita_Fine_Distinta &= Validita_Fine_Distinta
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

                            .Item("Numero_Conf") = Ricava_NumeroDocumento_Senza_Sequenza(.Item("Doc_Numero_Sin_Conf"),
                                                                                        .Item("Doc_Numero_Conf"),
                                                                                        .Item("Doc_Numero_Des_Conf"))

                            'PESI - CONF NUOVO:
                            'bisogna leggere solo quelli su movimenti_dettagli
                            'peso netto = qta_extra_totale
                            'tara imballi = tara se impostata in riga
                            'il resto da calcolare

                            .Item("peso_totale") = AgronicaCoreDataProvider.Agro_Math.ArrotondaVal_0(.Item("peso_totale"))

                            .Item("tara_veicolo") = AgronicaCoreDataProvider.Agro_Math.ArrotondaVal_0(.Item("tara_veicolo"))

                            Tara_Imballi_Vuoti = .Item("tara_totale_imb_entrata")
                            Tara_Imballi_Riga = .Item("Tara_Dettaglio")
                            Num_Righe_Bolla = .Item("Num_Righe")

                            .Item("Tara_Imballi") = objConfFun.Calcola_TaraImballi(Tara_Imballi_Vuoti,
                                                                                     Tara_Imballi_Riga,
                                                                                     Num_Righe_Bolla)

                            .Item("peso_netto") = ArrotondaVal_0(.Item("peso_netto"))

                            'Non va bene fare questa differenza, perché potrebbero esserci più articoli
                            '.Item("peso_lordo") = objHLP.Calcola_PesoLordo_NewConf(.Item("peso_totale"), .Item("tara_veicolo"))
                            '.Item("peso_lordo") = AgronicaCoreDataProvider.Agro_Math.ArrotondaVal_0(.Item("peso_lordo"))

                            .Item("peso_lordo") = .Item("peso_netto") + .Item("Tara_Imballi")
                            .Item("peso_lordo") = ArrotondaVal_0(.Item("peso_lordo"))

                            objADDFun.Calcola_NettoPagamento(.Item("peso_netto"),
                                                    .Item("Variazione_Raccolta"),
                                                    .Item("Udm_Cod_Raccolta"),
                                                    .Item("Degrado"),
                                                    .Item("netto_Pagamento"))

                            .Item("Degrado_Perc") = .Item("Variazione_Raccolta")

                            If CInt(Session("ASG_ProgressivoGIAS")) = enum_CodiceGIAS_Clienti.Fruttagel Then
                                objConfFun.Ricava_Specie_Varieta_FRG(.Item("Mat_Des"), .Item("Descrizione_JDE"), .Item("Varieta_FRG"))
                            End If

                            sigla_calib = String.Empty

                            If Not IsNothing(DT_OTabPar) AndAlso DT_OTabPar.Rows.Count > 0 Then

                                Dim datarowsTemp = DT_OTabPar.Select("Tabella_Cod = " & enum_OTabelle.Calibro & " AND Tabella_Par_Cod = " & .Item("tipo_cod"))
                                If Not IsNothing(datarowsTemp) AndAlso datarowsTemp.Length > 0 Then
                                    sigla_calib = datarowsTemp(0).Item("sigla")
                                End If

                            End If

                            Dim nettoPagamento As Decimal = If(IsDBNull(.Item("netto_Pagamento")), 0, CDec(.Item("netto_Pagamento")))

                            'sigla_calib = objHLP.Des_from_Cod(OTabPar, "Tabella_Par_Cod", "sigla", .Item("tipo_cod"))
                            If Not String.IsNullOrWhiteSpace(sigla_calib) Then
                                .Item(sigla_calib) = nettoPagamento
                            Else
                                .Item("Nessuna_Class") = nettoPagamento
                            End If
                            Mat_Cod = .Item("mat_cod")

                            '§§§
                            ''riuso i_matrice
                            'i_matrice = HT_Calibri_Intest(sigla_calib)
                            If CInt(Session("ASG_ProgressivoGIAS")) = enum_CodiceGIAS_Clienti.Fruttagel Then
                                'x fruttagel il mat_cod implicitamente raggruppa per la loro specie (codice jde), 
                                'varietà(di cui non abbiamo codice ma è il testo dopo il trattino e regolamento (che è implicito nel codice jde)
                                CodiceXRaggruppamento = .Item("mat_cod").ToString
                                Descr_Specie = .Item("Descrizione_JDE")
                                Descr_Varieta = .Item("Varieta_FRG")
                            Else
                                CodiceXRaggruppamento = .Item("veg_cod").ToString & "_" & .Item("cul_cod").ToString & "_" & .Item("reg_cod").ToString
                                Descr_Specie = .Item("Descr_Specie")
                                Descr_Varieta = .Item("Descr_Varieta")
                            End If

                            '§§§
                            If Not HT_riepilogo.Contains(CodiceXRaggruppamento) Then
                                '-------------------------------------
                                '-------- CODICE NON PRESENTE -------
                                '-------------------------------------

                                'aggiungo una colonna alla matrice per ogni CodiceXRaggruppamento
                                'i_Mtx_Col è inizializzato a -1
                                i_Mtx_Col += 1

                                'key = CodiceXRaggruppamento; valore = totale netto a pagamento (x quel raggruppamento)
                                HT_riepilogo.Add(CodiceXRaggruppamento, nettoPagamento)
                                HT_raggr_col.Add(CodiceXRaggruppamento, i_Mtx_Col)

                                'NOTA VB - Ridimensionamento con Preserve: Se si utilizza Preserve , è possibile ridimensionare solo l'ultima dimensione della matrice. Per ogni dimensione, è necessario specificare il limite della matrice esistente.
                                'Ad esempio, se la matrice contiene solo una dimensione, è possibile ridimensionarla e mantenere tutto il contenuto della matrice, poiché si sta modificando l'ultima e unica dimensione. Tuttavia, se la matrice dispone di due o più dimensioni e si utilizza Preserve, è possibile modificare i valori soltanto per l'ultima dimensione.
                                '(le righe della matrice sono i calibri, le colonne della matrice sono le righe dell'excel)
                                'una colonna per ogni CodiceXRaggruppamento
                                'una riga per tot netto pagamento + nessuna class qual + calibro 
                                ReDim Preserve Mtx_riepilogo(i_Mtx_righe, i_Mtx_Col)

                                Mtx_riepilogo(R_MTX_COD_RAGGR, i_Mtx_Col) = CodiceXRaggruppamento
                                'questi codici li salvo (dato che li ho) nel caso mi servissero dopo (senza doverli estrapolare dal CodiceXRaggruppamento)
                                'mal che vada non li uso
                                Mtx_riepilogo(R_MTX_MAT_COD, i_Mtx_Col) = If(IsDBNull(.Item("Mat_Cod")), 0, CInt(.Item("Mat_Cod")))
                                Mtx_riepilogo(R_MTX_VEG_COD, i_Mtx_Col) = If(IsDBNull(.Item("Veg_cod")), 0, CInt(.Item("Veg_cod")))
                                Mtx_riepilogo(R_MTX_CUL_COD, i_Mtx_Col) = If(IsDBNull(.Item("cul_cod")), 0, CInt(.Item("cul_cod")))
                                Mtx_riepilogo(R_MTX_REG_COD, i_Mtx_Col) = If(IsDBNull(.Item("Reg_cod")), 0, CInt(.Item("Reg_cod")))
                                '--
                                Mtx_riepilogo(R_MTX_SPECIE, i_Mtx_Col) = Descr_Specie
                                Mtx_riepilogo(R_MTX_VARIETA, i_Mtx_Col) = Descr_Varieta
                                Mtx_riepilogo(R_MTX_REG_DES, i_Mtx_Col) = If(IsDBNull(.Item("Reg_des")), "", CStr(.Item("Reg_des")))
                                '--
                                Mtx_riepilogo(R_MTX_TOT_NETTO, i_Mtx_Col) = nettoPagamento

                                'mettere il netto a pagamento sul calibro corrispondente:
                                'dalla sigla del calibro ricavare l'indice di riga della matrice riepilogo
                                If Not String.IsNullOrWhiteSpace(sigla_calib) Then
                                    curr_i_Mtx_righe = HT_Calibri_RigheMtx(sigla_calib)
                                Else
                                    curr_i_Mtx_righe = R_MTX_NO_CLASS_QUAL
                                End If

                                'e mettere il netto a pagamento nella cella
                                Mtx_riepilogo(curr_i_Mtx_righe, i_Mtx_Col) = nettoPagamento

                            Else
                                '-------------------------------------
                                '-------- CODICE GIA' PRESENTE ------
                                '-------------------------------------
                                'ricavo il totale precedente 
                                Tot_Netto_Su_Raggruppamento = HT_riepilogo(CodiceXRaggruppamento)
                                'aggiungo al totale precedente il netto a pagamento della riga corrente
                                HT_riepilogo(CodiceXRaggruppamento) = Tot_Netto_Su_Raggruppamento + nettoPagamento

                                curr_i_Mtx_col = HT_raggr_col(CodiceXRaggruppamento)

                                'aggiorno la colonna del totale netto pag. sulla matrice di riepilogo
                                Mtx_riepilogo(R_MTX_TOT_NETTO, curr_i_Mtx_col) = HT_riepilogo(CodiceXRaggruppamento)

                                'aggiornare il netto a pagamento sul calibro corrispondente:
                                'dalla sigla del calibro ricavare l'indice di riga della matrice riepilogo
                                If Not String.IsNullOrWhiteSpace(sigla_calib) Then
                                    curr_i_Mtx_righe = HT_Calibri_RigheMtx(sigla_calib)
                                Else
                                    curr_i_Mtx_righe = R_MTX_NO_CLASS_QUAL
                                End If

                                'ricavo il totale precedente
                                Tot_Netto_x_Calibro = Mtx_riepilogo(curr_i_Mtx_righe, curr_i_Mtx_col)

                                'aggiungo al totale precedente il netto a pagamento della riga corrente
                                Mtx_riepilogo(curr_i_Mtx_righe, curr_i_Mtx_col) = Tot_Netto_x_Calibro + nettoPagamento

                            End If


                        End With

                    Catch ex As Exception
                        Riga = New HtmlTableRow
                        Riga.Cells.Add(New HtmlTableCell)
                        Riga.Cells(0).ColSpan = Numero_Colonne
                        Riga.Cells(0).InnerHtml = "Errore su riga " & CStr(i + 1) & ": " & ex.Message
                        Me.TableExcel.Rows.Add(Riga)
                    End Try

                Next


                'If Tracciabilita_Impianti = 0 Then
                '    DT.Columns.Remove("Str_Progetto_Nome")
                '    DT.Columns.Remove("Str_Veg_Des")
                '    DT.Columns.Remove("Str_Cul_Des")
                '    DT.Columns.Remove("Str_Grfi_Des")
                '    DT.Columns.Remove("Str_Grva_Des")
                '    DT.Columns.Remove("Str_Setup_Cod")
                '    DT.Columns.Remove("Str_Sup_Imp")
                '    DT.Columns.Remove("Str_Validita_Inizio_Impianto")
                '    DT.Columns.Remove("Str_Validita_Fine_Impianto")
                '    DT.Columns.Remove("Str_Validita_Inizio_Distinta")
                '    DT.Columns.Remove("Str_Validita_Fine_Distinta")
                'End If

                DT.Columns.Remove("Id_Agenda")
                DT.Columns.Remove("Doc_Numero_Sin")
                DT.Columns.Remove("Doc_Numero")
                DT.Columns.Remove("Doc_Numero_Des")
                DT.Columns.Remove("Doc_Numero_Sin_Conf")
                DT.Columns.Remove("Doc_Numero_Conf")
                DT.Columns.Remove("Doc_Numero_Des_Conf")
                DT.Columns.Remove("Mat_Cod")
                DT.Columns.Remove("Veg_Cod")
                DT.Columns.Remove("cul_cod")
                DT.Columns.Remove("Reg_Cod")
                DT.Columns.Remove("Lotto_Raccolta")
                DT.Columns.Remove("Cal_Cod_Raccolta")
                DT.Columns.Remove("Udm_Cod_Raccolta")
                DT.Columns.Remove("Qta_Raccolta")
                DT.Columns.Remove("Variazione_Raccolta")
                DT.Columns.Remove("ordine_det")
                DT.Columns.Remove("Tipo_cod")
                DT.Columns.Remove("Tara_Dettaglio")
                DT.Columns.Remove("num_righe")
                DT.Columns.Remove("tara_totale_imb_entrata")
            End If

            ULTIMA_CELLA_EXCEL = DT.Columns.Count - 1


        Catch ex As Exception
            Riga = New HtmlTableRow
            Riga.Cells.Add(New HtmlTableCell)
            Riga.Cells(0).ColSpan = Numero_Colonne
            Riga.Cells(0).InnerHtml = "Errore nel Load: " & ex.Message
            Me.TableExcel.Rows.Add(Riga)
        End Try



        '##############################################################
        '###################  Creazione EXCEL #########################
        '##############################################################

        'If Not IsNothing(DT) AndAlso DT.Rows.Count <> 0 Then

        Crea_EXCEL(DT, HT_Intest, Mtx_riepilogo, HT_Calibri_ColExcel)

        'End If



    End Sub


    '##############################################################
    Private Sub Crea_EXCEL(ByVal Dt_Finale As DataTable,
                           ByVal HT_Intest As Hashtable,
                            ByVal Mtx_riepilogo(,) As String,
                           ByVal HT_riepilogo_Intest As Hashtable)

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

        Dim i, j, k As Integer

        Try

            Dim Colonna_Dt As String

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

                If Colonna_Dt.StartsWith("replace_") Then
                    Colonna_Dt = Colonna_Dt.Replace("replace_o", "")
                    Riga.Cells(k).InnerHtml = HT_Intest(Colonna_Dt)
                ElseIf Colonna_Dt.StartsWith("+") Then
                    Riga.Cells(k).InnerHtml = "&nbsp;" & Colonna_Dt
                Else

                    Select Case Colonna_Dt
                        Case "sa_nome"
                            Riga.Cells(k).InnerHtml = "Centro Aziendale"
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
                        Case CStr("CF_Conferente").ToLower
                            Riga.Cells(k).InnerHtml = "C.F.<br/>Conferente"
                        Case CStr("Regione_Conferente").ToLower
                            Riga.Cells(k).InnerHtml = "Regione<br/>Conferente"
                        Case CStr("Piva_Produttore").ToLower
                            Riga.Cells(k).InnerHtml = "P.IVA<br/>Produttore"
                        Case CStr("CF_Produttore").ToLower
                            Riga.Cells(k).InnerHtml = "C.F.<br/>Produttore"
                        Case CStr("Regione_Produttore").ToLower
                            Riga.Cells(k).InnerHtml = "Regione<br/>Produttore"
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
                        Case CStr("Mat_Cod").ToLower
                            Riga.Cells(k).InnerHtml = "Codice GIAS<br/>Articolo"
                        Case CStr("Mat_Des").ToLower
                            Riga.Cells(k).InnerHtml = "Articolo"
                        Case CStr("Cod_Articolo").ToLower
                            Riga.Cells(k).InnerHtml = "Cod.<br/>Articolo"
                        Case CStr("Codice_Esterno").ToLower
                            Riga.Cells(k).InnerHtml = "Cod.<br/>Esterno"
                        Case CStr("Descr_Specie").ToLower
                            Riga.Cells(k).InnerHtml = "Specie"
                        Case CStr("Descr_Varieta").ToLower
                            Riga.Cells(k).InnerHtml = "Varieta"
                        Case CStr("grva_des").ToLower
                            Riga.Cells(k).InnerHtml = "Tip.<br/>Varietale"
                        Case CStr("reg_des").ToLower
                            Riga.Cells(k).InnerHtml = "Regolamento"
                        Case CStr("Netto_Pagamento").ToLower
                            Riga.Cells(k).InnerHtml = "Netto a<br/>Pagamento"
                        Case CStr("Peso_Netto_Pagamento").ToLower
                            Riga.Cells(k).InnerHtml = "Peso<br/>Netto a<br/>Pagamento"
                        Case CStr("Descrizione_JDE").ToLower
                            Riga.Cells(k).InnerHtml = "Descrizione<br/>JDE"
                        Case CStr("Varieta_FRG").ToLower
                            Riga.Cells(k).InnerHtml = "Varieta<br/>FRG"
                        Case CStr("Nessuna_Class").ToLower
                            Riga.Cells(k).InnerHtml = "Nessuna<br/>Class. Qual."
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

                End If

            Next

            Me.TableExcel.Rows.Add(Riga)
            '----------------------------------
            '------- FINE INTESTAZIONE --------
            '----------------------------------

            '----------------------------------
            '------ RIEMPIMENTO TABELLA -------
            '----------------------------------

            If Dt_Finale.Rows.Count = 0 Then
                Riga = New HtmlTableRow
                Riga.Cells.Add(New HtmlTableCell)
                Riga.Cells(0).ColSpan = Numero_Colonne
                Riga.Cells(0).InnerHtml = "Il criterio di filtro non ha prodotto alcun risultato."
                Me.TableExcel.Rows.Add(Riga)
            End If


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
                    'Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(Dt_Finale.Rows(i).Item(j)), Dt_Finale.Rows(i).Item(j), "&nbsp;")

                    Select Case Dt_Finale.Columns.Item(j).Caption.ToLower

                        Case CStr("Piva_Produttore").ToLower, CStr("Piva_Conferente").ToLower,
                                CStr("Piva_Coop1").ToLower, CStr("Piva_Coop2").ToLower,
                                CStr("CF_Conferente").ToLower, CStr("CF_Produttore").ToLower
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

        Catch ex As Exception

            Riga = New HtmlTableRow
            Riga.Cells.Add(New HtmlTableCell)
            Riga.Cells(0).ColSpan = Numero_Colonne
            Riga.Cells(0).InnerHtml = "Creazione Excel Esportazione: " & ex.Message
            Me.TableExcel.Rows.Add(Riga)

        End Try

        '########################################################

        Try

            If Not IsNothing(Mtx_riepilogo) Then

                '------------------------------------------
                '------------- RIGHE VUOTE ----------------
                '------------------------------------------

                Riga = New HtmlTableRow
                For j = PRIMA_CELLA_EXCEL To ULTIMA_CELLA_EXCEL
                    Riga.Cells.Add(New HtmlTableCell)
                    Riga.Cells(j).InnerHtml = "&nbsp;"
                Next
                Me.TableExcel.Rows.Add(Riga)

                Riga = New HtmlTableRow
                For j = PRIMA_CELLA_EXCEL To ULTIMA_CELLA_EXCEL
                    Riga.Cells.Add(New HtmlTableCell)
                    Riga.Cells(j).InnerHtml = "&nbsp;"
                Next
                Me.TableExcel.Rows.Add(Riga)

                '------------------------------------------
                '------------- INTESTAZIONE RIEPILOGO ---------------
                '------------------------------------------
                Riga = New HtmlTableRow

                For j = PRIMA_CELLA_EXCEL To ULTIMA_CELLA_EXCEL
                    Riga.Cells.Add(New HtmlTableCell)
                Next

                ' SPAZI VUOTI
                '4 (numero di colonne specie - varietà - reg -tip. var)
                For j = PRIMA_CELLA_EXCEL To PRIMA_CELLA_CALIBRI - 4
                    Riga.Cells(j).InnerHtml = "&nbsp;"
                Next

                For k = j - 1 To ULTIMA_CELLA_CALIBRI
                    AgronicaCoreDataProvider.UtilityProvider.ElaboraCellaHTML(Riga.Cells(k), 2, "", "", "Gainsboro", "center", "middle")
                    Riga.Cells(k).Style.Item("font-weight") = "bold"
                    Riga.Cells(k).Style.Item("font-size") = "12px"
                    Riga.Cells(k).InnerHtml = HT_riepilogo_Intest(k)
                Next

                While k <= ULTIMA_CELLA_EXCEL
                    Riga.Cells(k).InnerHtml = "&nbsp;"
                    k += 1
                End While

                Me.TableExcel.Rows.Add(Riga)

                '--------------------------------------------
                '------- FINE INTESTAZIONE RIEPILOGO --------
                '--------------------------------------------

                '------------------------------------
                '------ RIEMPIMENTO TABELLA ---------
                '------------------------------------

                'scorro le colonne della matrice (che diventano le righe della tabella)
                For y = 0 To Mtx_riepilogo.GetUpperBound(1)

                    Riga = New HtmlTableRow

                    For j = PRIMA_CELLA_EXCEL To ULTIMA_CELLA_EXCEL
                        Riga.Cells.Add(New HtmlTableCell)
                    Next

                    ' SPAZI VUOTI
                    For j = PRIMA_CELLA_EXCEL To PRIMA_CELLA_CALIBRI - 4
                        Riga.Cells(j).InnerHtml = "&nbsp;"
                    Next

                    j -= 1

                    For x = R_MTX_SPECIE To Mtx_riepilogo.GetUpperBound(0)
                        Riga.Cells(j).Style.Item("vertical-align") = "middle"
                        ' Riga.Cells(j).Style.Item("font-size") = "12px"
                        Riga.Cells(j).InnerHtml = Mtx_riepilogo(x, y)

                        j += 1
                    Next

                    While j <= ULTIMA_CELLA_EXCEL
                        Riga.Cells(j).InnerHtml = "&nbsp;"
                        j += 1
                    End While


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
            Riga.Cells(0).InnerHtml = "Creazione Riepilogo: " & ex.Message
            Me.TableExcel.Rows.Add(Riga)
        End Try

    End Sub


End Class

Friend Class ClassQualComparer
    Inherits Comparer(Of String)

    Public Overrides Function Compare(x As String, y As String) As Integer
        Dim xToNumber As Double
        Dim yToNumber As Double

        Dim isXNumeric As Boolean 
        Dim isYNumeric As Boolean

        Dim arrX = x.Split("-")
        If Not String.IsNullOrWhiteSpace(arrX(0)) Then
            'Gestisco eventuali valori rappresentanti range, come "101-120" evitando casi come "-80". 
            'Prendo solo il primo valore del range per effettuare il confronto numerico
            x = arrX(0)
        End If
        isXNumeric = Double.TryParse(x, xToNumber)

        Dim arrY = y.Split("-")
        If Not String.IsNullOrWhiteSpace(arrY(0)) Then
            y = arrY(0)
        End If
        isYNumeric = Double.TryParse(y, yToNumber)

        If isXNumeric AndAlso isYNumeric Then
            Return xToNumber.CompareTo(yToNumber)
        End If

        If Not isXNumeric AndAlso Not isYNumeric Then
            Return String.Compare(x, y, True)
        End If

        If isXNumeric Then
            'In questo mio confronto imposto i numeri come "più grandi" delle stringhe, e quindi successivi nell'ordinamento
            Return 1
        End If

        If isYNumeric Then
            Return -1
        End If

    End Function
End Class