Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza


Public Class SchedaMovimentiMagazzinoExcel
    Inherits System.Web.UI.Page

    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Utenti As New AgronicaCoreDataProvider.AgronicaCoreParametri

    '----- Gestione Querystring
    Dim Qs_Piva As String
    Dim Qs_Sa_Cod As String
    Dim Qs_Fabbricato_Cod As String
    Dim Qs_Elem_Cod As String
    Dim Qs_Pro_Cod As String
    Dim Qs_Mat_Cod As String
    Dim QS_DataInizio As String
    Dim QS_DataFine As String
    Dim Qs_Arrotondamento As Integer
    Dim Qs_Ordinamento As Integer
    Dim Qs_Filtro_ElemCod, Qs_Lotto As String
    Protected WithEvents TableDati As System.Web.UI.HtmlControls.HtmlTable
    Dim Qs_Stampalotto As Integer
    Dim Qs_CaricoScarico As Integer = 0

#Region " Codice generato da Progettazione Web Form "

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

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Inserire qui il codice utente necessario per inizializzare la pagina

        'La Pagina deve essere visualizzata come un foglio Excel
        Response.ContentType = "application/vnd.ms-excel"
        Response.AddHeader("Content-Disposition", "inline; filename = SchedaMovimentiMagazzinoExcel.xls")


        'Tolgo la pagina dalla cache
        Response.Expires = 0

        '----- Verifico che l'utente sia autenticato
        If Session("ASG_objParametri_Server") Is Nothing Then
            Response.Redirect("~/Custom500.aspx")
        End If

        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))


        '#################################################################################
        '#####  Recupero i valori dalla QueryString 
        '#################################################################################

        QS_DataInizio = Stringa_Decodifica(Request.QueryString("dI").ToString, _
                            AgroKey_EncoderDecoder, _
                            Server)

        QS_DataFine = Stringa_Decodifica(Request.QueryString("dF").ToString, _
                                    AgroKey_EncoderDecoder, _
                                    Server)

        Qs_Piva = Stringa_Decodifica(Request.QueryString("p").ToString, _
                                    AgroKey_EncoderDecoder, _
                                    Server)

        Qs_Sa_Cod = Stringa_Decodifica(Request.QueryString("s").ToString, _
                            AgroKey_EncoderDecoder, _
                            Server)

        Qs_Fabbricato_Cod = Stringa_Decodifica(Request.QueryString("f").ToString, _
                    AgroKey_EncoderDecoder, _
                    Server)

        Qs_Elem_Cod = Stringa_Decodifica(Request.QueryString("e").ToString, _
                    AgroKey_EncoderDecoder, _
                    Server)

        Qs_Pro_Cod = Stringa_Decodifica(Request.QueryString("pro").ToString, _
                    AgroKey_EncoderDecoder, _
                    Server)

        Qs_Mat_Cod = Stringa_Decodifica(Request.QueryString("mat").ToString, _
                    AgroKey_EncoderDecoder, _
                    Server)

        Qs_Arrotondamento = Stringa_Decodifica(Request.QueryString("arr").ToString, _
                 AgroKey_EncoderDecoder, _
                 Server)

        Qs_Ordinamento = Stringa_Decodifica(Request.QueryString("ord").ToString, _
              AgroKey_EncoderDecoder, _
              Server)

        Qs_Filtro_ElemCod = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("fec")), _
               AgroKey_EncoderDecoder, _
               Server)

        Qs_Lotto = Trim(AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("lot")), _
                        AgroKey_EncoderDecoder, _
                        Server))

        Qs_Stampalotto = Trim(AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("sl")), _
                        AgroKey_EncoderDecoder, _
                        Server))

        If Not IsNothing(Request.QueryString("cs")) AndAlso IsNumeric(Stringa_Decodifica(Request.QueryString("cs").ToString, _
                   AgroKey_EncoderDecoder, _
                   Server)) Then
            Qs_CaricoScarico = Stringa_Decodifica(Request.QueryString("cs").ToString, _
                   AgroKey_EncoderDecoder, _
                   Server)
        End If

        Dim DTMovimentiMagazzino As New DataTable
        Dim Log As String = ""
        CaricaMovimenti(DTMovimentiMagazzino, Log, Qs_CaricoScarico)
        If Log <> "" Then
            AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox("Errore Stampa: " & Log, Page)
            Exit Sub
        End If



    End Sub


    Private Sub CaricaMovimenti(ByRef DTMovimentiMagazzino As DataTable, _
                                            ByRef Log_Errori As String, _
                                            Optional ByVal caricoScarico As Integer = 0)

        Dim FlagSingoloProdotto As Boolean = False
        Dim xOrderBy As String
        Dim DT As New DataTable
        Dim takeFROM_SaCod As Boolean

        Dim objConfigSitiR As New AgronicaCoreVarieDAL.Configurazione_Siti_R()

        Try

            Dim fromSaCod = objConfigSitiR.Recupera_Valore_ByChiave(Enum_SiteRedirector.Sito_AgronicaStampe_2010,
                                                                     "Export_MovimentiMagazzino_Utilizzo_Recode",
                                                                     objParametri_Server)

            takeFROM_SaCod = If(fromSaCod = "", False, fromSaCod)

            If (Qs_Pro_Cod = 0 And Qs_Mat_Cod <> 0) Or
                           (Qs_Pro_Cod <> 0 And Qs_Mat_Cod = 0) Then
                FlagSingoloProdotto = True
            End If

            Select Case Qs_Ordinamento
                Case 0 'data
                    xOrderBy = " Movimenti.Data_Movimento, Descrizione_Prodotto, Movimenti.Cau_Mov "
                Case 1 'prodotto
                    xOrderBy = " Descrizione_Prodotto, Movimenti.Data_Movimento, Movimenti.Cau_Mov "
            End Select

            Dim objMovMag As New AgronicaCoreStampeDAL.Magazzino

            Dim filtro As String = ""
            If Qs_Lotto <> "" Then
                filtro = " AND Movimenti_dettagli.Lotto like '%" & Qs_Lotto & "%'"
            End If

            DT = objMovMag.SchedaMovimentiMagazzino(CDate(QS_DataInizio),
                                                    CDate(QS_DataFine),
                                                        CStr(Qs_Piva),
                                                        CInt(Qs_Sa_Cod),
                                                        CInt(Qs_Fabbricato_Cod),
                                                        CInt(Qs_Elem_Cod),
                                                        CInt(Qs_Pro_Cod),
                                                        CInt(Qs_Mat_Cod),
                                                        0, 0, 0, 0, LOTTO_NONDEFINITO,
                                                        filtro, filtro, filtro, filtro,
                                                        filtro, filtro, filtro, filtro,
                                                        filtro, filtro, filtro, "",
                                                        xOrderBy,
                                                        objParametri_Server, objParametri_Utenti,
                                                        takeFROM_SaCod:=takeFROM_SaCod)

            Dim i, j, righe As Integer
            Dim strId_Agenda() As String
            Dim strIDAgenda As String = ""
            'Dim DTVegCod As New DataTable
            Dim DTMov As New DataTable
            Dim DtScarichi As New DataTable
            If DT.Rows.Count > 0 Then


                '  Galassi, 20/09/2016 14:58:57: è stato scelto di fare una select a valle per non toccare la funzione sopra (not my choose)
                Select Case caricoScarico
                    Case 0
                        'Carico e Scarico
                    Case 1
                        'Solo Carico
                        Dim tempDr = DT.Select("Cau_Mov ='" & CAU_CARICO & "' or Cau_Mov = '" & CAU_CONFERIMENTO & "'")
                        If Not IsNothing(tempDr) AndAlso tempDr.Count > 0 Then
                            DT = CType(tempDr.CopyToDataTable, DataTable)
                        Else
                            Exit Sub
                        End If
                        'CAU_CARICO, CAU_CONFERIMENTO
                    Case 2
                        'Solo Scarico
                        Dim tempDr = DT.Select("Cau_Mov ='" & CAU_SCARICO & "' or Cau_Mov = '" & CAU_CONFERIMENTO_DIVERSI & "'")
                        If Not IsNothing(tempDr) AndAlso tempDr.Count > 0 Then
                            DT = tempDr.CopyToDataTable
                        Else
                            Exit Sub
                        End If
                        'CAU_SCARICO, CAU_CONFERIMENTO_DIVERSI
                    Case Else

                End Select

                Dim objSqlDis As New AgronicaCoreDataProvider.DatatableUtility
                strId_Agenda = objSqlDis.SelectDistinct(DT, "id_agenda", False)
                If Not strId_Agenda Is Nothing AndAlso strId_Agenda.Length > 0 Then
                    For i = 0 To strId_Agenda.Length - 1
                        strIDAgenda &= strId_Agenda(i) & ","
                    Next
                End If
                If strIDAgenda <> "" Then
                    strIDAgenda = Left(strIDAgenda, strIDAgenda.Length - 1)
                End If
                Dim objMov As New AgronicaCoreContabDAL.Movimenti_R
                DTMov = objMov.MovimentiContabili_Contatto(0, "", 0, 0, 0, 0, CAU_REGISTRAZIONI, 0, AGRODATAINIZIO, AGRODATAFINE, "XYZ", 0, "XYZ", 0, 0, AGRODATAINIZIO, " agenda.id_agenda IN (" & strIDAgenda & ") ", "", objParametri_Server)
                Dim objMovDet As New AgronicaCoreContabDAL.Mov_Destinazioni_R
                DtScarichi = objMovDet.Leggi_Dettagli_Impianti_2(Qs_Piva, strIDAgenda, "", "", objParametri_Server)
            End If

            'se richiesto BLOCCO le operazioni
            If Not Session("BloccaOperazioni") Is Nothing AndAlso Session("BloccaOperazioni") = True AndAlso strIDAgenda <> "" Then
                Dim objAgendaW As New AgronicaCoreContabDAL.Agenda_W
                Dim Bloccati As Boolean = objAgendaW.Agenda_Blocca("", 0, strIDAgenda, "", objParametri_Server)
            End If



            '##############################################################
            '#####  Costruisco la tabella   ###############################
            '##############################################################

            For j = 0 To 29
                If j <> 0 Then
                    TableDati.Rows(0).Cells.Add(New HtmlTableCell)
                End If
                AgronicaCoreDataProvider.UtilityProvider.ElaboraCellaHTML(TableDati.Rows(0).Cells(j), 2, "", "", "Gainsboro", "center", "top")
            Next

            TableDati.Rows(0).Cells(0).InnerHtml = "Piva"
            TableDati.Rows(0).Cells(1).InnerHtml = "Codice Centro"
            TableDati.Rows(0).Cells(2).InnerHtml = "Centro"
            TableDati.Rows(0).Cells(3).InnerHtml = "Codice Magazzino"
            TableDati.Rows(0).Cells(4).InnerHtml = "Magazzino"
            TableDati.Rows(0).Cells(5).InnerHtml = "Codice Categoria"
            TableDati.Rows(0).Cells(6).InnerHtml = "Categoria"
            TableDati.Rows(0).Cells(7).InnerHtml = "Codice Prodotto / Materia Prima"
            TableDati.Rows(0).Cells(8).InnerHtml = "Prodotto / Materia Prima"
            TableDati.Rows(0).Cells(9).InnerHtml = "Lotto Interno"
            TableDati.Rows(0).Cells(10).InnerHtml = "Codice Unità di Misura"
            TableDati.Rows(0).Cells(11).InnerHtml = "Unità di Misura"
            TableDati.Rows(0).Cells(12).InnerHtml = "Qta"
            TableDati.Rows(0).Cells(13).InnerHtml = "Prezzo Unit."
            TableDati.Rows(0).Cells(14).InnerHtml = "Prezzo Tot."
            TableDati.Rows(0).Cells(15).InnerHtml = "Data"
            TableDati.Rows(0).Cells(16).InnerHtml = "Codice Causale Mov."
            TableDati.Rows(0).Cells(17).InnerHtml = "Causale Mov."
            TableDati.Rows(0).Cells(18).InnerHtml = "Causale Trasporto"

            TableDati.Rows(0).Cells(19).InnerHtml = "Specie Scarico"
            TableDati.Rows(0).Cells(20).InnerHtml = "Finalità"
            TableDati.Rows(0).Cells(21).InnerHtml = "App."
            TableDati.Rows(0).Cells(22).InnerHtml = "Codice App."
            TableDati.Rows(0).Cells(23).InnerHtml = "Inizio Impianto"
            TableDati.Rows(0).Cells(24).InnerHtml = "Stato Impianto"
            TableDati.Rows(0).Cells(25).InnerHtml = "Lotto Impianto"

            TableDati.Rows(0).Cells(26).InnerHtml = "Doc. N."
            TableDati.Rows(0).Cells(27).InnerHtml = "Doc. Data"
            TableDati.Rows(0).Cells(28).InnerHtml = "Doc. Fornitore"
            TableDati.Rows(0).Cells(29).InnerHtml = "Doc. P.IVA Fornitore"

            Dim Riga As HtmlTableRow

            For i = 0 To DT.Rows.Count - 1

                Dim Doc_Numero As String = ""
                Dim Contatto As String = ""
                Dim Cod_Contatto As String = ""
                Dim Causale_Trasporto As String = ""
                If Not IsDBNull(DT.Rows(i).Item("id_agenda")) Then
                    Dim DrMov() As DataRow = DTMov.Select("id_agenda=" & DT.Rows(i).Item("id_agenda"))
                    If Not DrMov Is Nothing AndAlso DrMov.Length > 0 Then
                        Doc_Numero = DrMov(0).Item("doc_numero")
                        Cod_Contatto = DrMov(0).Item("cod_contatto")
                        If Not IsDBNull(DrMov(0).Item("Causale_Trasporto")) Then
                            Causale_Trasporto = DrMov(0).Item("Causale_Trasporto")
                        End If
                        If Not IsDBNull(DrMov(0).Item("rag_soc")) AndAlso DrMov(0).Item("rag_soc") <> "" Then
                            Contatto = DrMov(0).Item("rag_soc") '& " (" & DrMov(0).Item("cod_contatto") & ")"
                        ElseIf Not IsDBNull(DrMov(0).Item("cognome")) AndAlso DrMov(0).Item("cognome") <> "" Then
                            Contatto = DrMov(0).Item("cognome") & " " & DrMov(0).Item("nome") '& " (" & DrMov(0).Item("cod_contatto") & ")"
                        End If
                    End If
                End If

                Dim Prezzo_Unitario As Double = 0
                Dim Prezzo_Unitario_Netto As Double = 0
                Dim Prezzo_Unitario_DocAllegato As Double = 0
                Dim Prezzo_Unitario_Netto_DocAllegato As Double = 0

                Dim Id_Agenda As Integer = DT.Rows(i).Item("id_agenda")
                Dim Id_Mov_Det As Integer = DT.Rows(i).Item("Id_Mov_Det")
                Dim Lav_cod As Integer = DT.Rows(i).Item("lav_cod")
                Dim Piva As String = DT.Rows(i).Item("piva")

                Dim Cau_Mov As String = DT.Rows(i).Item("Cau_Mov")



                Select Case Lav_cod

                    Case LAVCOD_BOLLA_RICEVUTA, LAVCOD_BOLLA_EMESSA, LAVCOD_DDT_CONTABILIZZATO_EMESSO

                        Dim lav_cod_fattura As Integer
                        Dim cau_movimento As String

                        If Lav_cod = LAVCOD_BOLLA_RICEVUTA Then
                            lav_cod_fattura = LAVCOD_FATTURA_RICEVUTA
                            cau_movimento = CAU_CARICO
                        Else
                            lav_cod_fattura = LAVCOD_FATTURA_EMESSA
                            cau_movimento = CAU_SCARICO
                        End If

                        Dim objRif As New AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R
                        objRif.Recupera_PrezzoUnitario_FatturaAllegata(objParametri_Server,
                                                                        Prezzo_Unitario_DocAllegato,
                                                                        Prezzo_Unitario_Netto_DocAllegato,
                                                                        CStr(Piva),
                                                                        0,
                                                                        Id_Agenda,
                                                                        0,
                                                                        Id_Mov_Det,
                                                                        Lav_cod,
                                                                        CAU_REGISTRAZIONI,
                                                                        lav_cod_fattura,
                                                                        cau_movimento)

                End Select

                If Prezzo_Unitario_DocAllegato <> 0 Then
                    Prezzo_Unitario = Prezzo_Unitario_DocAllegato
                Else
                    Prezzo_Unitario = DT.Rows(i).Item("prezzo_unitario")
                End If

                If Prezzo_Unitario_Netto_DocAllegato <> 0 Then
                    Prezzo_Unitario_Netto = Prezzo_Unitario_Netto_DocAllegato
                Else
                    Prezzo_Unitario_Netto = DT.Rows(i).Item("Prezzo_Unitario_Netto")
                End If

                If Prezzo_Unitario_Netto <> 0 Then
                    Prezzo_Unitario = Prezzo_Unitario_Netto
                End If


                Select Case Lav_cod

                    'OPERAZIONI CONTABILI
                    Case 1000 To 1999

                        Riga = New HtmlTableRow

                        For j = 0 To 29

                            'aggiungo la cella
                            Riga.Cells.Add(New HtmlTableCell)

                            Select Case j

                                Case 0
                                    Riga.Cells(j).InnerHtml = "'" & Piva
                                Case 1
                                    If takeFROM_SaCod Then
                                        If Not IsDBNull(DT.Rows(i).Item("FROM_SaCod")) Then
                                            Riga.Cells(j).InnerHtml = DT.Rows(i).Item("FROM_SaCod")
                                        Else
                                            If Not IsDBNull(DT.Rows(i).Item("sa_cod")) Then
                                                Riga.Cells(j).InnerHtml = DT.Rows(i).Item("sa_cod")
                                            Else
                                                Riga.Cells(j).InnerHtml = ""
                                            End If
                                        End If
                                    Else
                                        If Not IsDBNull(DT.Rows(i).Item("sa_cod")) Then
                                            Riga.Cells(j).InnerHtml = DT.Rows(i).Item("sa_cod")
                                        Else
                                            Riga.Cells(j).InnerHtml = ""
                                        End If
                                    End If

                                Case 2
                                    If Not IsDBNull(DT.Rows(i).Item("sa_nome")) Then
                                        Riga.Cells(j).InnerHtml = DT.Rows(i).Item("sa_nome")
                                    Else
                                        Riga.Cells(j).InnerHtml = ""
                                    End If

                                Case 3
                                    If Not IsDBNull(DT.Rows(i).Item("id_destinazione")) Then
                                        Riga.Cells(j).InnerHtml = DT.Rows(i).Item("id_destinazione")
                                    Else
                                        Riga.Cells(j).InnerHtml = ""
                                    End If

                                Case 4
                                    If Not IsDBNull(DT.Rows(i).Item("fabbricato_des")) Then
                                        Riga.Cells(j).InnerHtml = DT.Rows(i).Item("fabbricato_des")
                                    Else
                                        Riga.Cells(j).InnerHtml = ""
                                    End If

                                Case 5
                                    If Not IsDBNull(DT.Rows(i).Item("elem_cod")) Then
                                        Riga.Cells(j).InnerHtml = DT.Rows(i).Item("elem_cod")
                                    Else
                                        Riga.Cells(j).InnerHtml = ""
                                    End If

                                Case 6
                                    If Not IsDBNull(DT.Rows(i).Item("cat_des")) Then
                                        Riga.Cells(j).InnerHtml = DT.Rows(i).Item("cat_des")
                                    Else
                                        Riga.Cells(j).InnerHtml = ""
                                    End If

                                Case 7
                                    If Not IsDBNull(DT.Rows(i).Item("pro_cod")) AndAlso DT.Rows(i).Item("pro_cod") <> 0 Then
                                        Riga.Cells(j).InnerHtml = DT.Rows(i).Item("pro_cod")
                                    ElseIf Not IsDBNull(DT.Rows(i).Item("mat_cod")) AndAlso DT.Rows(i).Item("mat_cod") <> 0 Then
                                        Riga.Cells(j).InnerHtml = DT.Rows(i).Item("mat_cod")
                                    Else
                                        Riga.Cells(j).InnerHtml = ""
                                    End If
                                Case 8
                                    If Not IsDBNull(DT.Rows(i).Item("descrizione_prodotto")) Then
                                        Riga.Cells(j).InnerHtml = DT.Rows(i).Item("descrizione_prodotto")
                                    Else
                                        Riga.Cells(j).InnerHtml = ""
                                    End If
                                Case 9
                                    If Not IsDBNull(DT.Rows(i).Item("Cod_Progetto")) AndAlso DT.Rows(i).Item("Cod_Progetto") <> 0 Then
                                        Dim objProgetto As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
                                        Riga.Cells(j).InnerHtml = objProgetto.ProgettoNome_from_ProgettoCod(DT.Rows(i).Item("Cod_Progetto"), Nothing, objParametri_Server)
                                    Else
                                        Riga.Cells(j).InnerHtml = ""
                                    End If
                                Case 10
                                    If Not IsDBNull(DT.Rows(i).Item("udm_cod")) Then
                                        Riga.Cells(j).InnerHtml = DT.Rows(i).Item("udm_cod")
                                    Else
                                        Riga.Cells(j).InnerHtml = ""
                                    End If
                                Case 11
                                    If Not IsDBNull(DT.Rows(i).Item("udm_sim")) Then
                                        Riga.Cells(j).InnerHtml = DT.Rows(i).Item("udm_sim")
                                    Else
                                        Riga.Cells(j).InnerHtml = ""
                                    End If
                                Case 12
                                    If Not IsDBNull(DT.Rows(i).Item("Qta_Dest")) Then
                                        Select Case Qs_Arrotondamento
                                            Case 0
                                                Riga.Cells(j).InnerHtml = DT.Rows(i).Item("Qta_Dest")
                                            Case 1
                                                Riga.Cells(j).InnerHtml = Math.Round(DT.Rows(i).Item("Qta_Dest"), 0, MidpointRounding.AwayFromZero)
                                            Case 2
                                                Riga.Cells(j).InnerHtml = Math.Round(DT.Rows(i).Item("Qta_Dest"), 1, MidpointRounding.AwayFromZero)
                                            Case 3
                                                Riga.Cells(j).InnerHtml = Math.Round(DT.Rows(i).Item("Qta_Dest"), 2, MidpointRounding.AwayFromZero)
                                            Case 4
                                                Riga.Cells(j).InnerHtml = Math.Round(DT.Rows(i).Item("Qta_Dest"), 3, MidpointRounding.AwayFromZero)
                                            Case 5
                                                Riga.Cells(j).InnerHtml = Math.Round(DT.Rows(i).Item("Qta_Dest"), 4, MidpointRounding.AwayFromZero)
                                        End Select
                                    Else
                                        Riga.Cells(j).InnerHtml = ""
                                    End If
                                Case 13
                                    Riga.Cells(j).InnerHtml = Prezzo_Unitario
                                Case 14
                                    Riga.Cells(j).InnerHtml = Prezzo_Unitario * DT.Rows(i).Item("qta")
                                Case 15
                                    If Not IsDBNull(DT.Rows(i).Item("data_movimento")) Then
                                        Riga.Cells(j).InnerHtml = CDate(DT.Rows(i).Item("data_movimento")).ToShortDateString
                                    Else
                                        Riga.Cells(j).InnerHtml = ""
                                    End If
                                Case 16
                                    Riga.Cells(j).InnerHtml = Lav_cod
                                Case 17
                                    If Not IsDBNull(DT.Rows(i).Item("lav_des")) Then
                                        Riga.Cells(j).InnerHtml = DT.Rows(i).Item("lav_des")
                                    Else
                                        Riga.Cells(j).InnerHtml = ""
                                    End If
                                Case 18
                                    Riga.Cells(j).InnerHtml = Causale_Trasporto

                                Case 19
                                    Riga.Cells(j).InnerHtml = ""

                                Case 20 'finalita
                                    Riga.Cells(j).InnerHtml = ""
                                Case 21 'app nome
                                    Riga.Cells(j).InnerHtml = ""
                                Case 22 'codici
                                    Riga.Cells(j).InnerHtml = ""
                                Case 23 'inizio
                                    Riga.Cells(j).InnerHtml = ""
                                Case 24 'stato
                                    Riga.Cells(j).InnerHtml = ""
                                Case 25 'lotto
                                    Riga.Cells(j).InnerHtml = ""

                                Case 26
                                    Riga.Cells(j).InnerHtml = Doc_Numero
                                Case 27
                                    If Not IsDBNull(DT.Rows(i).Item("data_movimento")) Then
                                        Riga.Cells(j).InnerHtml = CDate(DT.Rows(i).Item("data_movimento")).ToShortDateString
                                    Else
                                        Riga.Cells(j).InnerHtml = ""
                                    End If
                                Case 28
                                    Riga.Cells(j).InnerHtml = Contatto
                                Case 29
                                    If Cod_Contatto <> "" Then
                                        Riga.Cells(j).InnerHtml = "'" & Cod_Contatto
                                    Else
                                        Riga.Cells(j).InnerHtml = ""
                                    End If

                            End Select

                        Next

                        Me.TableDati.Rows.Add(Riga)

                    Case Else

                        If Not IsDBNull(DT.Rows(i).Item("id_agenda")) AndAlso DtScarichi.Rows.Count > 0 Then

                            Dim DrImp() As DataRow

                            If Lav_cod = LAVCOD_RACCOLTA AndAlso DT.Rows(i).Item("cod_progetto") <> 0 Then
                                DrImp = DtScarichi.Select("id_agenda=" & DT.Rows(i).Item("id_agenda") &
                                                               " AND Elem_Cod=" & DT.Rows(i).Item("elem_cod") &
                                                               " AND Pro_COd=" & DT.Rows(i).Item("pro_cod") &
                                                               " AND Mat_Cod=" & DT.Rows(i).Item("mat_cod") &
                                                               " AND Udm_Cod=" & DT.Rows(i).Item("udm_cod") &
                                                               " AND progetto_cod=" & DT.Rows(i).Item("cod_progetto"))
                            Else
                                DrImp = DtScarichi.Select("id_agenda=" & DT.Rows(i).Item("id_agenda") &
                                                               " AND Elem_Cod=" & DT.Rows(i).Item("elem_cod") &
                                                               " AND Pro_COd=" & DT.Rows(i).Item("pro_cod") &
                                                               " AND Mat_Cod=" & DT.Rows(i).Item("mat_cod") &
                                                               " AND Udm_Cod=" & DT.Rows(i).Item("udm_cod") &
                                                               " AND Lotto='" & DT.Rows(i).Item("Lotto") & "'")
                            End If

                            If Not DrImp Is Nothing Then

                                For righe = 0 To DrImp.Length - 1

                                    Riga = New HtmlTableRow

                                    For j = 0 To 29

                                        'aggiungo la cella
                                        Riga.Cells.Add(New HtmlTableCell)

                                        Select Case j

                                            Case 0
                                                Riga.Cells(j).InnerHtml = "'" & Piva
                                            Case 1
                                                If takeFROM_SaCod Then
                                                    If Not IsDBNull(DT.Rows(i).Item("FROM_SaCod")) Then
                                                        Riga.Cells(j).InnerHtml = DT.Rows(i).Item("FROM_SaCod")
                                                    Else
                                                        If Not IsDBNull(DT.Rows(i).Item("sa_cod")) Then
                                                            Riga.Cells(j).InnerHtml = DT.Rows(i).Item("sa_cod")
                                                        Else
                                                            Riga.Cells(j).InnerHtml = ""
                                                        End If
                                                    End If
                                                Else
                                                    If Not IsDBNull(DT.Rows(i).Item("sa_cod")) Then
                                                        Riga.Cells(j).InnerHtml = DT.Rows(i).Item("sa_cod")
                                                    Else
                                                        Riga.Cells(j).InnerHtml = ""
                                                    End If
                                                End If

                                            Case 2
                                                If Not IsDBNull(DT.Rows(i).Item("sa_nome")) Then
                                                    Riga.Cells(j).InnerHtml = DT.Rows(i).Item("sa_nome")
                                                Else
                                                    Riga.Cells(j).InnerHtml = ""
                                                End If

                                            Case 3
                                                If Not IsDBNull(DT.Rows(i).Item("id_destinazione")) Then
                                                    Riga.Cells(j).InnerHtml = DT.Rows(i).Item("id_destinazione")
                                                Else
                                                    Riga.Cells(j).InnerHtml = ""
                                                End If

                                            Case 4
                                                If Not IsDBNull(DT.Rows(i).Item("fabbricato_des")) Then
                                                    Riga.Cells(j).InnerHtml = DT.Rows(i).Item("fabbricato_des")
                                                Else
                                                    Riga.Cells(j).InnerHtml = ""
                                                End If

                                            Case 5
                                                If Not IsDBNull(DT.Rows(i).Item("elem_cod")) Then
                                                    Riga.Cells(j).InnerHtml = DT.Rows(i).Item("elem_cod")
                                                Else
                                                    Riga.Cells(j).InnerHtml = ""
                                                End If

                                            Case 6
                                                If Not IsDBNull(DT.Rows(i).Item("cat_des")) Then
                                                    Riga.Cells(j).InnerHtml = DT.Rows(i).Item("cat_des")
                                                Else
                                                    Riga.Cells(j).InnerHtml = ""
                                                End If

                                            Case 7
                                                If Not IsDBNull(DT.Rows(i).Item("pro_cod")) AndAlso DT.Rows(i).Item("pro_cod") <> 0 Then
                                                    Riga.Cells(j).InnerHtml = DT.Rows(i).Item("pro_cod")
                                                ElseIf Not IsDBNull(DT.Rows(i).Item("mat_cod")) AndAlso DT.Rows(i).Item("mat_cod") <> 0 Then
                                                    Riga.Cells(j).InnerHtml = DT.Rows(i).Item("mat_cod")
                                                Else
                                                    Riga.Cells(j).InnerHtml = ""
                                                End If
                                            Case 8
                                                If Not IsDBNull(DT.Rows(i).Item("descrizione_prodotto")) Then
                                                    Riga.Cells(j).InnerHtml = DT.Rows(i).Item("descrizione_prodotto")
                                                Else
                                                    Riga.Cells(j).InnerHtml = ""
                                                End If
                                            Case 9
                                                If Not IsDBNull(DT.Rows(i).Item("Cod_Progetto")) AndAlso DT.Rows(i).Item("Cod_Progetto") <> 0 Then
                                                    Dim objProgetto As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
                                                    Riga.Cells(j).InnerHtml = objProgetto.ProgettoNome_from_ProgettoCod(DT.Rows(i).Item("Cod_Progetto"), Nothing, objParametri_Server)
                                                Else
                                                    Riga.Cells(j).InnerHtml = ""
                                                End If
                                            Case 10
                                                If Not IsDBNull(DT.Rows(i).Item("udm_cod")) Then
                                                    Riga.Cells(j).InnerHtml = DT.Rows(i).Item("udm_cod")
                                                Else
                                                    Riga.Cells(j).InnerHtml = ""
                                                End If
                                            Case 11
                                                If Not IsDBNull(DT.Rows(i).Item("udm_sim")) Then
                                                    Riga.Cells(j).InnerHtml = DT.Rows(i).Item("udm_sim")
                                                Else
                                                    Riga.Cells(j).InnerHtml = ""
                                                End If
                                            Case 12
                                                If Not IsDBNull(DrImp(righe).Item("Qta_Dest")) Then
                                                    Select Case Qs_Arrotondamento
                                                        Case 0
                                                            Riga.Cells(j).InnerHtml = DrImp(righe).Item("Qta_Dest")
                                                        Case 1
                                                            Riga.Cells(j).InnerHtml = Math.Round(DrImp(righe).Item("Qta_Dest"), 0, MidpointRounding.AwayFromZero)
                                                        Case 2
                                                            Riga.Cells(j).InnerHtml = Math.Round(DrImp(righe).Item("Qta_Dest"), 1, MidpointRounding.AwayFromZero)
                                                        Case 3
                                                            Riga.Cells(j).InnerHtml = Math.Round(DrImp(righe).Item("Qta_Dest"), 2, MidpointRounding.AwayFromZero)
                                                        Case 4
                                                            Riga.Cells(j).InnerHtml = Math.Round(DrImp(righe).Item("Qta_Dest"), 3, MidpointRounding.AwayFromZero)
                                                        Case 5
                                                            Riga.Cells(j).InnerHtml = Math.Round(DrImp(righe).Item("Qta_Dest"), 4, MidpointRounding.AwayFromZero)
                                                    End Select
                                                    'Riga.Cells(j).InnerHtml = DrImp(righe).Item("Qta_Dest")
                                                Else
                                                    Riga.Cells(j).InnerHtml = ""
                                                End If
                                            Case 13
                                                Riga.Cells(j).InnerHtml = Prezzo_Unitario
                                            Case 14
                                                Riga.Cells(j).InnerHtml = Prezzo_Unitario * DT.Rows(i).Item("qta")
                                            Case 15
                                                If Not IsDBNull(DT.Rows(i).Item("data_movimento")) Then
                                                    Riga.Cells(j).InnerHtml = CDate(DT.Rows(i).Item("data_movimento")).ToShortDateString
                                                Else
                                                    Riga.Cells(j).InnerHtml = ""
                                                End If
                                            Case 16
                                                Riga.Cells(j).InnerHtml = Lav_cod
                                            Case 17
                                                If Not IsDBNull(DT.Rows(i).Item("lav_des")) Then
                                                    Riga.Cells(j).InnerHtml = DT.Rows(i).Item("lav_des")
                                                Else
                                                    Riga.Cells(j).InnerHtml = ""
                                                End If
                                            Case 18
                                                Riga.Cells(j).InnerHtml = Causale_Trasporto

                                            Case 19
                                                If Not IsDBNull(DrImp(righe).Item("veg_des")) Then
                                                    Riga.Cells(j).InnerHtml = DrImp(righe).Item("veg_des")
                                                Else
                                                    'verifico se è destinazione uso
                                                    Dim Destinazione As String = ""
                                                    Dim objDestUso As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R
                                                    objDestUso.Esiste_DestinazioneUso_2(DrImp(righe).Item("piva"), DrImp(righe).Item("sa_cod"), DrImp(righe).Item("appezza"), DrImp(righe).Item("id_destinazione"), Destinazione, objParametri_Server)
                                                    If Destinazione <> "" Then
                                                        Riga.Cells(j).InnerHtml = Destinazione
                                                    Else
                                                        Riga.Cells(j).InnerHtml = ""
                                                    End If
                                                End If

                                            Case 20 'finalita
                                                If Not IsDBNull(DrImp(righe).Item("grfi_des")) Then
                                                    Riga.Cells(j).InnerHtml = DrImp(righe).Item("grfi_des")
                                                Else
                                                    Riga.Cells(j).InnerHtml = ""
                                                End If
                                            Case 21 'app nome
                                                If Not IsDBNull(DrImp(righe).Item("app_nome")) Then
                                                    Riga.Cells(j).InnerHtml = DrImp(righe).Item("app_nome")
                                                Else
                                                    Riga.Cells(j).InnerHtml = ""
                                                End If

                                            Case 22 'codici
                                                If Not IsDBNull(DrImp(righe).Item("appezza")) AndAlso Not IsDBNull(DrImp(righe).Item("Id_Destinazione")) Then
                                                    Riga.Cells(j).InnerHtml = DrImp(righe).Item("appezza") & "-" & DrImp(righe).Item("Id_Destinazione")
                                                Else
                                                    Riga.Cells(j).InnerHtml = ""
                                                End If

                                            Case 23 'inizio
                                                If Not IsDBNull(DrImp(righe).Item("validita_inizio")) Then
                                                    Riga.Cells(j).InnerHtml = CDate(DrImp(righe).Item("validita_inizio")).ToShortDateString
                                                Else
                                                    Riga.Cells(j).InnerHtml = ""
                                                End If
                                            Case 24 'stato
                                                If Not IsDBNull(DrImp(righe).Item("stato")) Then
                                                    Riga.Cells(j).InnerHtml = DrImp(righe).Item("stato")
                                                Else
                                                    Riga.Cells(j).InnerHtml = ""
                                                End If
                                            Case 25 'lotto
                                                If Not IsDBNull(DrImp(righe).Item("Progetto_Nome")) Then
                                                    Riga.Cells(j).InnerHtml = DrImp(righe).Item("Progetto_Nome")
                                                Else
                                                    Riga.Cells(j).InnerHtml = ""
                                                End If

                                            Case 26
                                                Riga.Cells(j).InnerHtml = ""
                                            Case 27
                                                Riga.Cells(j).InnerHtml = ""
                                            Case 28
                                                Riga.Cells(j).InnerHtml = ""
                                            Case 29
                                                Riga.Cells(j).InnerHtml = ""
                                        End Select

                                    Next

                                    'Aggiungo la Riga alla Tabella 
                                    Me.TableDati.Rows.Add(Riga)

                                Next

                            End If

                        End If


                End Select


            Next


        Catch ex As Exception
            Log_Errori += "Errore durante il caricamento dei dati: " + ex.Message
        End Try

    End Sub



End Class
