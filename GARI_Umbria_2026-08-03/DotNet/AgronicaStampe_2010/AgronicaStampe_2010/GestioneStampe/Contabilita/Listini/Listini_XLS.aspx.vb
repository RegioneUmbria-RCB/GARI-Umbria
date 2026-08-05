Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class Listini_XLS
    Inherits System.Web.UI.Page


    Dim Piva, str_FiltroAgg As String
    Dim Listino_Classe_Cod, Listino_Cod, Elem_Cod, Pro_Cod, Mat_Cod, Lav_Cod As Integer
    Dim Tipo_Classe, Listino_Classe_Padre_Cod, ChkApplicabilita As Integer
    Dim Tipo_Iva, Tipo_Provvigione, Conto_Terzi, Cod_Conto_Default, Cal_Cod As Integer
    Dim Chk_Vettore, Cod_Rapporto, Cod_RisUm As Integer
    Dim Mezzo, Udm_Cod, Cod_Iva, Cod_Conto_Det_Default As Integer


    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Utenti As New AgronicaCoreDataProvider.AgronicaCoreParametri

    Const Numero_Colonne As Integer = 34

    '#################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Dim Riga As HtmlTableRow


        'La Pagina deve essere visualizzata come un foglio Excel
        Response.ContentType = "application/vnd.ms-excel"
        Response.AddHeader("Content-Disposition", "inline; filename = Listini.xls")

        '##############################################################
        '#####  Verifico Credenziali di Accesso  ######################
        '##############################################################

        '----- Verifico che l'utente sia autenticato

        If Session("ASG_objParametri_Server") Is Nothing Then
            Response.Redirect("~/Custom500.aspx")
        End If

        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))


        '##############################################################
        '############  Lettura Parametri Query String #################
        '##############################################################

        Try

            Piva = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("p")),
                                                                            AgroKey_EncoderDecoder,
                                                                            Server)

            Listino_Classe_Cod = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("lcc")),
                                                                      AgroKey_EncoderDecoder,
                                                                      Server)

            Listino_Cod = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("lc")),
                                                                                  AgroKey_EncoderDecoder,
                                                                                  Server)

            Elem_Cod = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("elem")),
                                                                                  AgroKey_EncoderDecoder,
                                                                                  Server)

            Pro_Cod = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("pro")),
                                                                                  AgroKey_EncoderDecoder,
                                                                                  Server)

            Mat_Cod = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("mat")),
                                                                                  AgroKey_EncoderDecoder,
                                                                                  Server)

            Tipo_Classe = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("tc")),
                                                                                        AgroKey_EncoderDecoder,
                                                                                        Server)

            Listino_Classe_Padre_Cod = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("lcpc")),
                                                                                        AgroKey_EncoderDecoder,
                                                                                        Server)

            ChkApplicabilita = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("chka")),
                                                                                        AgroKey_EncoderDecoder,
                                                                                        Server)

            Tipo_Iva = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("ti")),
                                                                                        AgroKey_EncoderDecoder,
                                                                                        Server)

            Tipo_Provvigione = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("tp")),
                                                                                        AgroKey_EncoderDecoder,
                                                                                        Server)

            Conto_Terzi = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("ct")),
                                                                                        AgroKey_EncoderDecoder,
                                                                                        Server)

            Cod_Conto_Default = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("ccd")),
                                                                                        AgroKey_EncoderDecoder,
                                                                                        Server)

            Cal_Cod = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("cal")),
                                                                                        AgroKey_EncoderDecoder,
                                                                                        Server)
            Lav_Cod = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("lav")),
                                                                                        AgroKey_EncoderDecoder,
                                                                                        Server)

            Chk_Vettore = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("chkv")),
                                                                                        AgroKey_EncoderDecoder,
                                                                                        Server)

            Cod_Rapporto = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("cr")),
                                                                                        AgroKey_EncoderDecoder,
                                                                                        Server)

            Cod_RisUm = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("risum")),
                                                                                                AgroKey_EncoderDecoder,
                                                                                                Server)

            Mezzo = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("m")),
                                                                                        AgroKey_EncoderDecoder,
                                                                                        Server)

            Udm_Cod = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("udm")),
                                                                                                AgroKey_EncoderDecoder,
                                                                                                Server)

            Cod_Iva = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("ci")),
                                                                                        AgroKey_EncoderDecoder,
                                                                                        Server)

            Cod_Conto_Det_Default = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("ccdd")),
                                                                                                AgroKey_EncoderDecoder,
                                                                                                Server)
            str_FiltroAgg = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("strfa")),
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
        '###################  ELABORA REPORT #########################
        '##############################################################

        Stampa_Listini()


    End Sub


    '##############################################################
    Public Function Crea_DT_Risultato() As DataTable

        Dim Dt As New DataTable

        '----- Definisco la struttura del DataTable

        Dt.Columns.Add(New DataColumn("Cartella_Listino", GetType(String)))
        Dt.Columns.Add(New DataColumn("Tipo_Listino", GetType(String)))
        Dt.Columns.Add(New DataColumn("Listino", GetType(String)))
        Dt.Columns.Add(New DataColumn("Codice", GetType(String)))
        Dt.Columns.Add(New DataColumn("Applicabile", GetType(String)))
        Dt.Columns.Add(New DataColumn("Validita_Inizio_Listino", GetType(String)))
        Dt.Columns.Add(New DataColumn("Validita_Fine_Listino", GetType(String)))
        'Dt.Columns.Add(New DataColumn("Tipo_Iva", GetType(String)))
        'Dt.Columns.Add(New DataColumn("Tipo_Provvigione", GetType(String)))
        Dt.Columns.Add(New DataColumn("Categoria", GetType(String)))
        Dt.Columns.Add(New DataColumn("Cod_Articolo", GetType(String)))
        Dt.Columns.Add(New DataColumn("Prodotto", GetType(String)))
        Dt.Columns.Add(New DataColumn("Conto_Terzi", GetType(String)))
        'Dt.Columns.Add(New DataColumn("Conto_Economico_default", GetType(String)))
        Dt.Columns.Add(New DataColumn("Calibro", GetType(String)))
        Dt.Columns.Add(New DataColumn("Lotto_Codice", GetType(String)))
        Dt.Columns.Add(New DataColumn("Lotto_Valore", GetType(String)))
        Dt.Columns.Add(New DataColumn("Lotto_Codice2", GetType(String)))
        Dt.Columns.Add(New DataColumn("Lotto_Valore2", GetType(String)))
        Dt.Columns.Add(New DataColumn("Operazione", GetType(String)))
        Dt.Columns.Add(New DataColumn("Vettore", GetType(String)))
        Dt.Columns.Add(New DataColumn("Mezzo", GetType(String)))
        Dt.Columns.Add(New DataColumn("Rapporto_Contabile", GetType(String)))
        Dt.Columns.Add(New DataColumn("Contatto", GetType(String)))
        Dt.Columns.Add(New DataColumn("Udm", GetType(String)))
        Dt.Columns.Add(New DataColumn("Qta", GetType(String)))
        Dt.Columns.Add(New DataColumn("Prezzo", GetType(String)))
        Dt.Columns.Add(New DataColumn("Validita_Inizio", GetType(String)))
        Dt.Columns.Add(New DataColumn("Validita_Fine", GetType(String)))
        Dt.Columns.Add(New DataColumn("Iva", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sconto", GetType(String)))
        Dt.Columns.Add(New DataColumn("Conto_Eco_Dettaglio_Default", GetType(String)))
        Dt.Columns.Add(New DataColumn("Tipo_Iva_Dettaglio", GetType(String)))
        Dt.Columns.Add(New DataColumn("Tipo_Prov_dettaglio", GetType(String)))
        Dt.Columns.Add(New DataColumn("Provvigione", GetType(String)))

        Return Dt

    End Function


    '##############################################################
    Private Sub Stampa_Listini()

        Dim Riga As HtmlTableRow


        '##############################################################
        '####################  LETTURA DATI ###########################
        '##############################################################

        Dim objListini As New AgronicaCoreStampeDAL.Listini
        Dim DT As DataTable
        Dim DtRisultato As DataTable
        Dim i As Integer
        Dim Dt_calibri As DataTable = Nothing
        Dim Dt_conti As DataTable = Nothing
        Dim Dt_Rapcont As DataTable = Nothing
        Dim Dt_contatti As DataTable = Nothing
        Dim Dt_iva As DataTable = Nothing

        DtRisultato = Crea_DT_Risultato()


        Try

            Dim objCalibri As New AgronicaCoreAnagrafeDAL.Materie_Prime_Calibri_R

            Dt_calibri = objCalibri.Leggi(0, "", "", objParametri_Server)

        Catch ex As Exception
            Riga = New HtmlTableRow
            Riga.Cells.Add(New HtmlTableCell)
            Riga.Cells(0).ColSpan = Numero_Colonne
            Riga.Cells(0).InnerHtml = "Lettura calibri: " & ex.Message
            Me.TableExcel.Rows.Add(Riga)
        End Try


        Try

            Dim objContiEc As New AgronicaCoreContabDAL.Conti_R

            Dt_conti = objContiEc.Leggi("", 0, "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

        Catch ex As Exception
            Riga = New HtmlTableRow
            Riga.Cells.Add(New HtmlTableCell)
            Riga.Cells(0).ColSpan = Numero_Colonne
            Riga.Cells(0).InnerHtml = "Lettura conti: " & ex.Message
            Me.TableExcel.Rows.Add(Riga)
        End Try


        Try

            Dim objIva As New AgronicaCoreMetaSchemaDAL.IVA_Aliquote_R

            Dt_iva = objIva.Leggi(0, -1, -1, -1, NATURA_ESCLUSIONE_NOFILTRO,
                                  "", "", objParametri_Server)

        Catch ex As Exception
            Riga = New HtmlTableRow
            Riga.Cells.Add(New HtmlTableCell)
            Riga.Cells(0).ColSpan = Numero_Colonne
            Riga.Cells(0).InnerHtml = "Lettura codici iva: " & ex.Message
            Me.TableExcel.Rows.Add(Riga)
        End Try

        Try

            Dim objRapCont As New AgronicaCoreAnagrafeDAL.Rapporti_Contabili_R

            Dt_Rapcont = objRapCont.Contatti_RapportiContabili_Leggi(
                                    SACOD_CONTATTO_NONDEFINITO,
                                    0,
                                    False, False, False, False, False, False, False,
                                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                                    "", "",
                                    objParametri_Server)

        Catch ex As Exception
            Riga = New HtmlTableRow
            Riga.Cells.Add(New HtmlTableCell)
            Riga.Cells(0).ColSpan = Numero_Colonne
            Riga.Cells(0).InnerHtml = "Lettura Rapporti contabili: " & ex.Message
            Me.TableExcel.Rows.Add(Riga)
        End Try

        Try

            Dim objContatti As New AgronicaCoreAnagrafeDAL.Risorse_Umane_R

            Dt_contatti = objContatti.LeggiSoloContatto("", 0, "", 0, 0, "",
                                                        True,
                                                        AGRODATAINIZIO, AGRODATAFINE,
                                                        enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                        "", "",
                                                        objParametri_Server)

        Catch ex As Exception
            Riga = New HtmlTableRow
            Riga.Cells.Add(New HtmlTableCell)
            Riga.Cells(0).ColSpan = Numero_Colonne
            Riga.Cells(0).InnerHtml = "Lettura contatti: " & ex.Message
            Me.TableExcel.Rows.Add(Riga)
        End Try


        Try


            DT = objListini.Listini_Export(Piva,
                                            Listino_Classe_Cod,
                                            Listino_Cod,
                                            Elem_Cod,
                                            Pro_Cod,
                                            Mat_Cod,
                                                Tipo_Classe,
                                                Listino_Classe_Padre_Cod,
                                                ChkApplicabilita,
                                                Tipo_Iva,
                                                Tipo_Provvigione,
                                                Conto_Terzi,
                                                Cod_Conto_Default,
                                                Cal_Cod,
                                                Lav_Cod,
                                                Chk_Vettore,
                                                Cod_Rapporto,
                                                Cod_RisUm,
                                                Mezzo,
                                                Udm_Cod,
                                                Cod_Iva,
                                                Cod_Conto_Det_Default,
                                                    str_FiltroAgg,
                                                    str_FiltroAgg,
                                                    "",
                                                    objParametri_Server)


            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

                For i = 0 To DT.Rows.Count - 1

                    'Try

                    Elabora_RigaDt(DT.Rows(i), DtRisultato, Dt_calibri, Dt_conti, Dt_Rapcont, Dt_contatti, Dt_iva)

                    'Catch ex As Exception
                    '    Riga = New HtmlTableRow
                    '    Riga.Cells.Add(New HtmlTableCell)
                    '    Riga.Cells(0).ColSpan = Numero_Colonne
                    '    Riga.Cells(0).InnerHtml = "elaborazione Dati: " + ex.Message
                    '    Me.TableExcel.Rows.Add(Riga)
                    'End Try

                Next

            End If


        Catch ex As Exception
            Riga = New HtmlTableRow
            Riga.Cells.Add(New HtmlTableCell)
            Riga.Cells(0).ColSpan = Numero_Colonne
            Riga.Cells(0).InnerHtml = "stampa Listini: " & ex.Message
            Me.TableExcel.Rows.Add(Riga)
        End Try


        '##############################################################
        '###################  Creazione EXCEL #########################
        '##############################################################

        Crea_EXCEL(DtRisultato)


    End Sub


    '##############################################################
    Private Sub Elabora_RigaDt(ByVal DrDoc As DataRow,
                                ByRef DtRisultato As DataTable,
                                ByVal Dt_calibri As DataTable,
                                ByVal Dt_conti As DataTable,
                                ByVal Dt_Rapcont As DataTable,
                                ByVal Dt_contatti As DataTable,
                                ByVal Dt_iva As DataTable)


        Dim Riga As HtmlTableRow

        Try

            Dim dr_Ris As DataRow

            dr_Ris = DtRisultato.NewRow

            dr_Ris.Item("Cartella_Listino") = DrDoc.Item("Listino_Classe_Des")
            Select Case DrDoc.Item("Tipo_Classe")
                Case 1
                    dr_Ris.Item("Tipo_Listino") = "Acquisto"
                Case 2
                    dr_Ris.Item("Tipo_Listino") = "Vendita"
                Case Else
                    dr_Ris.Item("Tipo_Listino") = ""
            End Select

            dr_Ris.Item("Listino") = DrDoc.Item("Listino_des")
            dr_Ris.Item("Codice") = DrDoc.Item("Listino_Cod_Des")

            If DrDoc.Item("ChkApplicabilita") = 1 Then
                dr_Ris.Item("Applicabile") = "SI"
            Else
                dr_Ris.Item("Applicabile") = "NO"
            End If

            'If DrDoc.Item("Tipo_Iva") = 0 Then
            '    dr_Ris.Item("Tipo_Iva") = "Iva Esclusa"
            'Else
            '    dr_Ris.Item("Tipo_Iva") = "Iva Inclusa"
            'End If

            'If DrDoc.Item("Tipo_Provvigione") = 0 Then
            '    dr_Ris.Item("Tipo_Provvigione") = ""
            'Else
            '    dr_Ris.Item("Tipo_Provvigione") = DrDoc.Item("Tipo_Provvigione")
            'End If

            If CDate(DrDoc.Item("Validita_Inizio_Listino")).ToShortDateString = AGRODATAINIZIO Then
                dr_Ris.Item("Validita_Inizio_Listino") = "..."
            Else
                dr_Ris.Item("Validita_Inizio_Listino") = CDate(DrDoc.Item("Validita_Inizio_Listino")).ToShortDateString
            End If
            If CDate(DrDoc.Item("Validita_Fine_Listino")).ToShortDateString = AGRODATAFINE Then
                dr_Ris.Item("Validita_Fine_Listino") = "..."
            Else
                dr_Ris.Item("Validita_Fine_Listino") = CDate(DrDoc.Item("Validita_Fine_Listino")).ToShortDateString
            End If

            '------------------------------------
            dr_Ris.Item("Categoria") = DrDoc.Item("NomeComune")

            If DrDoc.Item("mat_cod") <> 0 Then
                'dr_Ris.Item("Prodotto") = DrDoc.Item("Mat_Des") & " Cod." & DrDoc.Item("Cod_articolo")
                dr_Ris.Item("Prodotto") = DrDoc.Item("Mat_Des")
                dr_Ris.Item("Cod_Articolo") = DrDoc.Item("Cod_articolo")
            Else
                'dr_Ris.Item("Prodotto") = LeggiProdotto(Server, Session, Page,
                '                                        objParametri_Server,
                '                                        objParametri_Utenti,
                '                                         Nothing,
                '                                         Nothing,
                '                                         Nothing,
                '                                         Piva,
                '                                         DrDoc.Item("Elem_Cod"),
                '                                         DrDoc.Item("Pro_Cod"),
                '                                         0,
                '                                         , , , , , , , , , )

                Dim objContab As New AgronicaCoreContabDAL.Contabilita_R

                dr_Ris.Item("Prodotto") = objContab.LeggiProdottoStampeContab(objParametri_Server,
                                                                            objParametri_Utenti,
                                                                            Nothing,
                                                                            Nothing,
                                                                            Nothing,
                                                                            Piva,
                                                                            DrDoc.Item("Elem_Cod"),
                                                                            DrDoc.Item("Pro_Cod"),
                                                                            0,
                                                                            0,
                                                                            "",
                                                                            0,
                                                                            False,
                                                                            , ,
                                                                            ,
                                                                            , ,
                                                                            ,
                                                                            ,
                                                                            ,
                                                                            ,
)

                dr_Ris.Item("Cod_Articolo") = CStr(DrDoc.Item("Pro_Cod"))
            End If

            If DrDoc.Item("Conto_Terzi") = 1 Then
                dr_Ris.Item("Conto_Terzi") = "SI"
            Else
                dr_Ris.Item("Conto_Terzi") = "NO"
            End If

            'dr_Ris.Item("Conto_Economico_default") = Des_from_Cod(Dt_conti, "Cod_Conto", "Conto_Descr", DrDoc.Item("Cod_Conto_Default"))

            dr_Ris.Item("Calibro") = Des_from_Cod(Dt_calibri, "cal_cod", "Cal_Des", DrDoc.Item("cal_cod"))

            If DrDoc.Item("Lotto_Cod1") = 0 Then
                dr_Ris.Item("Lotto_Codice") = ""
            Else
                dr_Ris.Item("Lotto_Codice") = DrDoc.Item("Lotto_Cod1")
            End If
            dr_Ris.Item("Lotto_Valore") = DrDoc.Item("Lotto_Val1")

            If DrDoc.Item("Lotto_Cod2") = 0 Then
                dr_Ris.Item("Lotto_Codice2") = ""
            Else
                dr_Ris.Item("Lotto_Codice2") = DrDoc.Item("Lotto_Cod2")
            End If
            dr_Ris.Item("Lotto_Valore2") = DrDoc.Item("Lotto_Val2")

            If DrDoc.Item("Lav_Cod") <> 0 Then
                Dim objOp As New AgronicaCoreMetaSchemaDAL.Operazioni_R
                dr_Ris.Item("Operazione") = objOp.LavorazioneDes_from_LavorazioneCod(
                                                    DrDoc.Item("Lav_Cod"),
                                                    objParametri_Server)
            Else
                dr_Ris.Item("Operazione") = ""
            End If

            If DrDoc.Item("ChkVettore") = 1 Then
                dr_Ris.Item("Vettore") = "SI"
            Else
                dr_Ris.Item("Vettore") = "NO"
            End If

            Select Case DrDoc.Item("Mezzo")
                Case 1
                    dr_Ris.Item("Mezzo") = "Contatto"
                Case 2
                    dr_Ris.Item("Mezzo") = "Macchina"
                Case -1
                    dr_Ris.Item("Mezzo") = ""
                Case Else
                    dr_Ris.Item("Mezzo") = ""
            End Select


            dr_Ris.Item("Rapporto_Contabile") = Des_from_Cod(Dt_Rapcont, "cod_rapporto", "Rapporto_Des", DrDoc.Item("cod_rapporto"))

            dr_Ris.Item("Contatto") = Des_from_Cod(Dt_contatti, "cod_risum", "rag_soc", DrDoc.Item("cod_risum"))


            dr_Ris.Item("Udm") = DrDoc.Item("Udm_sim")
            dr_Ris.Item("Qta") = DrDoc.Item("Qta")
            dr_Ris.Item("Prezzo") = DrDoc.Item("Prezzo")

            If CDate(DrDoc.Item("Validita_Inizio")).ToShortDateString = AGRODATAINIZIO Then
                dr_Ris.Item("Validita_Inizio") = "..."
            Else
                dr_Ris.Item("Validita_Inizio") = CDate(DrDoc.Item("Validita_Inizio")).ToShortDateString
            End If
            If CDate(DrDoc.Item("Validita_Fine")).ToShortDateString = AGRODATAFINE Then
                dr_Ris.Item("Validita_Fine") = "..."
            Else
                dr_Ris.Item("Validita_Fine") = CDate(DrDoc.Item("Validita_Fine")).ToShortDateString
            End If

            dr_Ris.Item("Iva") = Des_from_Cod(Dt_iva, "codice", "Sigla", DrDoc.Item("cod_iva"))

            If DrDoc.Item("Sconto") = 0 Then
                dr_Ris.Item("Sconto") = ""
            Else
                dr_Ris.Item("Sconto") = DrDoc.Item("Sconto")
            End If

            Select Case DrDoc.Item("Cod_Conto_Det_Default")
                Case 0 'nessuno
                    'no impostato
                    dr_Ris.Item("Conto_Eco_Dettaglio_Default") = ""
                Case -1 ' default
                    'utilizza quello di Cod_Conto_Default
                    dr_Ris.Item("Conto_Eco_Dettaglio_Default") = Des_from_Cod(Dt_conti, "cod_conto", "Conto_Descr", DrDoc.Item("Cod_Conto_Default"))
                Case Else
                    dr_Ris.Item("Conto_Eco_Dettaglio_Default") = Des_from_Cod(Dt_conti, "cod_conto", "Conto_Descr", DrDoc.Item("Cod_Conto_Det_Default"))
            End Select

            Select Case DrDoc.Item("Tipo_Iva_Det")
                Case -1
                    If DrDoc.Item("Tipo_Iva") = 0 Then
                        dr_Ris.Item("Tipo_Iva_Dettaglio") = "Iva Esclusa"
                    Else
                        dr_Ris.Item("Tipo_Iva_Dettaglio") = "Iva Inclusa"
                    End If
                Case 0
                    dr_Ris.Item("Tipo_Iva_Dettaglio") = "Iva Esclusa"
                Case 1
                    dr_Ris.Item("Tipo_Iva_Dettaglio") = "Iva Inclusa"
                Case Else
                    dr_Ris.Item("Tipo_Iva_Dettaglio") = ""
            End Select

            Select Case DrDoc.Item("Tipo_Provvigione_Det")
                Case -1
                    Select Case DrDoc.Item("Tipo_Provvigione")
                        Case 0
                            dr_Ris.Item("Tipo_Prov_dettaglio") = "Anagrafica contatto"
                        Case 1
                            dr_Ris.Item("Tipo_Prov_dettaglio") = "Categoria"
                        Case 2
                            dr_Ris.Item("Tipo_Prov_dettaglio") = "Listino"
                        Case Else
                            dr_Ris.Item("Tipo_Prov_dettaglio") = ""
                    End Select
                Case 0
                    dr_Ris.Item("Tipo_Prov_dettaglio") = "Anagrafica contatto"
                Case 1
                    dr_Ris.Item("Tipo_Prov_dettaglio") = "Categoria"
                Case 2
                    dr_Ris.Item("Tipo_Prov_dettaglio") = "Listino"
                Case Else
                    dr_Ris.Item("Tipo_Prov_dettaglio") = ""
            End Select

            If DrDoc.Item("Provvigione_Listino") = 0 Then
                dr_Ris.Item("Provvigione") = ""
            Else
                dr_Ris.Item("Provvigione") = DrDoc.Item("Provvigione_Listino")
            End If

            DtRisultato.Rows.Add(dr_Ris)

        Catch ex As Exception
            Riga = New HtmlTableRow
            Riga.Cells.Add(New HtmlTableCell)
            Riga.Cells(0).ColSpan = Numero_Colonne
            Riga.Cells(0).InnerHtml = "Elabora_RigaDt: " & ex.Message
            Me.TableExcel.Rows.Add(Riga)
        End Try

    End Sub


    '##############################################################
    Private Function Des_from_Cod(ByVal DT As DataTable,
                                    ByVal NomeCampoCod As String,
                                    ByVal NomeCampoDes As String,
                                    ByVal Codice As Integer) As String

        Dim Des As String = ""

        If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

            Dim Dr As DataRow()

            Dr = DT.Select(" " & NomeCampoCod & " = " & AgronicaCoreDataProvider.UtilityProvider.Agro_SQL_SaveNum(Codice))

            If Not IsNothing(Dr) AndAlso Dr.Length > 0 Then
                Des = Dr(0).Item(NomeCampoDes)
            End If

        End If

        Return Des

    End Function


    '##############################################################
    Private Sub Crea_EXCEL(ByVal Dt_Finale As DataTable)

        Dim Riga As HtmlTableRow

        '##############################################################
        '#####  Costruisco la tabella   ###############################
        '##############################################################

        Dim i, j, k As Integer

        Try

            Dim Colonna_Dt As String

            'AgronicaCoreDataProvider.UtilityProvider.ElaboraCellaHTML(Me.TableExcel.Rows(0).Cells(0), 2, "", "", "Yellow", "left", "middle")
            'Me.TableExcel.Rows(0).Cells(0).InnerHtml = "Anagrafica Contatti"
            'Me.TableExcel.Rows(0).Cells(0).Style.Item("font-weight") = "bold"
            'Me.TableExcel.Rows(0).Cells(0).Style.Item("font-size") = "18px"
            'Me.TableExcel.Rows(0).Cells(0).Style.Item("vertical-align") = "middle"
            'Me.TableExcel.Rows(0).Cells(0).Style.Item("border-top-width") = "1px"

            Me.TableExcel.Rows(0).Cells(0).ColSpan = CInt(Dt_Finale.Columns.Count)
            Me.TableExcel.Rows(0).Cells(0).InnerText = "LISTINI"
            Me.TableExcel.Rows(0).Cells(0).Style.Item("vertical-align") = "middle"
            Me.TableExcel.Rows(0).Cells(0).Style.Item("font-weight") = "bold"
            'Me.TableExcel.Rows(0).Cells(0).Style.Item("font-size") = "12px"

            Riga = New HtmlTableRow
            Riga.Cells.Add(New HtmlTableCell)
            Riga.Cells(0).ColSpan = Numero_Colonne
            Riga.Cells(0).InnerHtml = "&nbsp;"
            Me.TableExcel.Rows.Add(Riga)
            '-----
            'Riga = New HtmlTableRow
            'Riga.Cells.Add(New HtmlTableCell)
            'Riga.Cells(0).ColSpan = Numero_Colonne
            'Riga.Cells(0).InnerHtml = ""
            'Me.TableExcel.Rows.Add(Riga)
            ''-----
            Riga = New HtmlTableRow
            Riga.Cells.Add(New HtmlTableCell)
            Riga.Cells(0).ColSpan = Numero_Colonne
            Riga.Cells(0).InnerHtml = "&nbsp;"
            Me.TableExcel.Rows.Add(Riga)
            '-----

            If IsNothing(Dt_Finale) OrElse Dt_Finale.Rows.Count = 0 Then
                Riga = New HtmlTableRow
                Riga.Cells.Add(New HtmlTableCell)
                Riga.Cells(0).ColSpan = Numero_Colonne
                Riga.Cells(0).InnerHtml = "Il criterio di filtro non ha prodotto alcun risultato."
                Me.TableExcel.Rows.Add(Riga)
                Exit Sub
            End If

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

                Colonna_Dt = Riga.Cells(k).InnerHtml

                Select Case Colonna_Dt

                    Case "Cartella_Listino"
                        Riga.Cells(k).InnerHtml = "Cartella<br/>Listino"
                    Case "Tipo_Listino"
                        Riga.Cells(k).InnerHtml = "Tipo<br/>Listino"
                    Case "Validita_Inizio_Listino"
                        Riga.Cells(k).InnerHtml = "Validita Inizio<br/>Listino"
                    Case "Validita_Fine_Listino"
                        Riga.Cells(k).InnerHtml = "Validita Fine<br/>Listino"
                    Case "Conto_Terzi"
                        Riga.Cells(k).InnerHtml = "Conto<br/>Terzi"
                    Case "Lotto_Codice"
                        Riga.Cells(k).InnerHtml = "Cod.<br/>Lotto 1"
                    Case "Lotto_Valore"
                        Riga.Cells(k).InnerHtml = "Valore<br/>Lotto 1"
                    Case "Lotto_Codice2"
                        Riga.Cells(k).InnerHtml = "Cod.<br/>Lotto 2"
                    Case "Lotto_Valore2"
                        Riga.Cells(k).InnerHtml = "Valore<br/>Lotto 2"
                    Case "Rapporto_Contabile"
                        Riga.Cells(k).InnerHtml = "Rapporto<br/>Contabile"
                    Case "Qta"
                        Riga.Cells(k).InnerHtml = "Quantità"
                    Case "Udm"
                        Riga.Cells(k).InnerHtml = "Unità<br/>Misura"
                    Case "Prezzo"
                        Riga.Cells(k).InnerHtml = "Prezzo<br/>Unitario"
                    Case "Validita_Inizio"
                        Riga.Cells(k).InnerHtml = "Validita<br/>Inizio"
                    Case "Validita_Fine"
                        Riga.Cells(k).InnerHtml = "Validita<br/>Fine"
                    Case "Conto_Eco_Dettaglio_Default"
                        Riga.Cells(k).InnerHtml = "Conto<br/>Economico"
                    Case "Tipo_Iva_Dettaglio"
                        Riga.Cells(k).InnerHtml = "Tipo<br/>Iva"
                    Case "Tipo_Prov_dettaglio"
                        Riga.Cells(k).InnerHtml = "Modalità<br/>Provvigione"
                    Case "Provvigione"
                        Riga.Cells(k).InnerHtml = "Provvigione<br/>Agente"
                    Case "Cod_Articolo"
                        Riga.Cells(k).InnerHtml = "Codice<br/>Articolo"
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

                    'Select Case Dt_Finale.Columns.Item(j).Caption.ToLower

                    'Case CStr("").ToLower, CStr("").ToLower, _
                    '        CStr("").ToLower, CStr("").ToLower
                    '    'aggiungo uno spazio davanti x salvare gli zeri...
                    '    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(Dt_Finale.Rows(i).Item(j)), "&nbsp;" & Dt_Finale.Rows(i).Item(j), "&nbsp;")

                    'Case CStr("").ToLower, "numero_conf"
                    '    If Dt_Finale.Rows(i).Item(j) <> "" Then
                    '        'aggiungo un apice davanti per vedere visualizzato il numero e non una data
                    '        'Riga.Cells(j).InnerHtml = "'" & Dt_Finale.Rows(i).Item(j)
                    '        Riga.Cells(j).InnerHtml = "&nbsp;" & Dt_Finale.Rows(i).Item(j)
                    '    End If

                    'Case Else

                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(Dt_Finale.Rows(i).Item(j)), Dt_Finale.Rows(i).Item(j), "&nbsp;")

                    'End Select

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
            Riga.Cells(0).InnerHtml = "Creazione Excel: " & ex.Message
            Me.TableExcel.Rows.Add(Riga)
        End Try

        '########################################################


    End Sub


End Class