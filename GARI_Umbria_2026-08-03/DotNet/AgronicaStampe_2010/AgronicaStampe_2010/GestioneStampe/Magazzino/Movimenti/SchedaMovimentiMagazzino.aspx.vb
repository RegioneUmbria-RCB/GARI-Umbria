Imports AgronicaControlli_2010
Imports AgronicaControlli_2010.UtilityPersonalizzazioniGraficheCliente
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModelsSTD.PersonalizzazioniGraficheCliente
Imports AgronicaCoreUtility
Imports CrystalDecisions.Shared

Public Class SchedaMovimentiMagazzino

    Inherits System.Web.UI.Page
    Private rptStampa As Rpt_SchedaMovimentiMagazzino
    Private rptFooterLogo As FooterLogo
    Private DSMovimentiMagazzino As DS_MovimentiMagazzino
    Private DSLogoFooter As New DS_LogoFooter
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
    Dim Qs_Stampalotto As Integer
    Dim Qs_CaricoScarico As Integer = 0

    Const COL_ELEM_COD As Integer = 0
    Const COL_PRO_COD As Integer = 1
    Const COL_MAT_COD As Integer = 2
    Const COL_COD_PROGETTO As Integer = 3
    Const COL_FASE_COD As Integer = 4
    Const COL_LOTTO As Integer = 5
    Const COL_CAL_COD As Integer = 6
    Const COL_UDM_COD As Integer = 7
    Const COL_GIACENZA As Integer = 8

    Const NUM_COLONNE As Integer = 9


    Dim Matrice(0, 0) As Object
    Dim Num_Prodotti As Integer

    Dim LinkPaginaStampa As String

    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Super_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Utenti As New AgronicaCoreDataProvider.AgronicaCoreParametri

    Dim personalizzazioniRegioneUmbria = ""

    Dim customLoghi As PersonalizzazioniGraficheCliente = Nothing

#Region " MOVIMENTI MAGAZZINO "

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
        'istanzio l'oggetto report
        rptStampa = New Rpt_SchedaMovimentiMagazzino
        rptFooterLogo = New FooterLogo
    End Sub

#End Region

    '###########################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Dim objCentriAz As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read

        '#################################################################################
        '#####  Recupero i valori dalla QueryString 
        '#################################################################################

        QS_DataInizio = Stringa_Decodifica(Request.QueryString("dI").ToString,
                            AgroKey_EncoderDecoder,
                            Server)

        QS_DataFine = Stringa_Decodifica(Request.QueryString("dF").ToString,
                                    AgroKey_EncoderDecoder,
                                    Server)

        Qs_Piva = Stringa_Decodifica(Request.QueryString("p").ToString,
                                    AgroKey_EncoderDecoder,
                                    Server)

        Qs_Sa_Cod = Stringa_Decodifica(Request.QueryString("s").ToString,
                            AgroKey_EncoderDecoder,
                            Server)

        Qs_Fabbricato_Cod = Stringa_Decodifica(Request.QueryString("f").ToString,
                    AgroKey_EncoderDecoder,
                    Server)

        Qs_Elem_Cod = Stringa_Decodifica(Request.QueryString("e").ToString,
                    AgroKey_EncoderDecoder,
                    Server)

        Qs_Pro_Cod = Stringa_Decodifica(Request.QueryString("pro").ToString,
                    AgroKey_EncoderDecoder,
                    Server)

        Qs_Mat_Cod = Stringa_Decodifica(Request.QueryString("mat").ToString,
                    AgroKey_EncoderDecoder,
                    Server)

        Qs_Arrotondamento = Stringa_Decodifica(Request.QueryString("arr").ToString,
                 AgroKey_EncoderDecoder,
                 Server)

        Qs_Ordinamento = Stringa_Decodifica(Request.QueryString("ord").ToString,
              AgroKey_EncoderDecoder,
              Server)

        Qs_Filtro_ElemCod = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("fec")),
               AgroKey_EncoderDecoder,
               Server)

        Qs_Lotto = Trim(AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("lot")),
                        AgroKey_EncoderDecoder,
                        Server))

        Qs_Stampalotto = Trim(AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("sl")),
                        AgroKey_EncoderDecoder,
                        Server))

        If Not IsNothing(Request.QueryString("cs")) AndAlso IsNumeric(Stringa_Decodifica(Request.QueryString("cs").ToString,
                   AgroKey_EncoderDecoder,
                   Server)) Then
            Qs_CaricoScarico = Stringa_Decodifica(Request.QueryString("cs").ToString,
                   AgroKey_EncoderDecoder,
                   Server)
        End If


        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Super_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Super_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))

        customLoghi = UtilityPersonalizzazioniGraficheCliente.LeggiPersonalizzazioniGraficheCliente(objParametri_Server, objParametri_Super_Server)

        ''##############################################################
        ''#####  Recupero piva e sa_cod dalla stringa xml
        ''##############################################################

        'Dim strXmlVariabilistampe As String
        'Dim htVariabiliStampe As System.Collections.Hashtable
        'Dim strErr As String

        'Dim XmlDoc As New System.Xml.XmlDocument
        'Dim XML_FiltroStampa As System.Xml.XmlElement

        'strXmlVariabilistampe = Session("strXmlVariabilistampe")

        ''Carico la stringa xml in un nuovo documento
        'XmlDoc = New System.Xml.XmlDocument
        'XmlDoc.LoadXml(strXmlVariabilistampe)

        'If XmlDoc.HasChildNodes Then

        '    XML_FiltroStampa = XmlDoc.SelectSingleNode("FiltroStampa")

        '    'Ricavo i parametri che servono

        '    Session("ASG_Utente_Username") = XML_FiltroStampa.GetAttribute("username")


        '    If Not IsNothing(XML_FiltroStampa.GetAttribute("arrotondamento")) Then
        '        If XML_FiltroStampa.GetAttribute("arrotondamento") <> "" Then
        '            Qs_Arrotondamento = XML_FiltroStampa.GetAttribute("arrotondamento")
        '        Else
        '            Qs_Arrotondamento = 5
        '        End If
        '    Else
        '        Qs_Arrotondamento = 5
        '    End If


        '    XML_EstraiVariabiliStampe(strXmlVariabilistampe, htVariabiliStampe, strErr)

        '    Qs_Piva = CStr(htVariabiliStampe("piva"))
        '    Qs_Sa_Cod = CStr(htVariabiliStampe("sa_cod"))
        '    Qs_Fabbricato_Cod = CStr(htVariabiliStampe("fabbricato_cod"))
        '    Qs_Elem_Cod = CStr(htVariabiliStampe("elem_cod"))
        '    Qs_Pro_Cod = CStr(htVariabiliStampe("pro_cod"))
        '    Qs_Mat_Cod = CStr(htVariabiliStampe("mat_cod"))

        'End If



        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        'Dim CrystalReportViewer1 As CrystalDecisions.Web.CrystalReportViewer

        Dim Nome_Documento As String = "MovimentiMagazzino"
        Dim Log_Errori As String

        If Not Me.IsPostBack Then

            Try

                '--------------------------------------------
                ' DICHIARAZIONE  E INIZIALIZZAZIONE DATASET
                '--------------------------------------------
                Dim DSMovimentiMagazzino As New DS_MovimentiMagazzino

                'CrystalReportViewer1 = New CrystalDecisions.Web.CrystalReportViewer
                'CrystalReportViewer1.Style.Add("LEFT", "-275px")
                'CrystalReportViewer1.Style.Add("TOP", "0px")
                'CrystalReportViewer1.Style.Add("POSITION", "Absolute")
                'Me.FindControl("Form1").Controls.Add(CrystalReportViewer1)

                Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
                CType(rptStampa.Section1.ReportObjects("TextAzienda"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = objImprese.RagSoc_from_Piva(Qs_Piva, objParametri_Server)
                CType(rptStampa.Section1.ReportObjects("TextCentro"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = objCentriAz.SaNome_from_SaCod(Qs_Piva, Qs_Sa_Cod, objParametri_Server)
                CType(rptStampa.Section1.ReportObjects("TextMagazzino"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = AgronicaCoreAnagrafeDAL.Fabbricati_R.FabbricatoDes_from_FabbricatoCod(Qs_Piva, Qs_Sa_Cod, Qs_Fabbricato_Cod, objParametri_Server)
                CType(rptStampa.Section1.ReportObjects("TextData"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = QS_DataInizio & " - " & QS_DataFine


                'In caso le personalizzazioni siano attive, nascondo il logo e ragione sociale Agronica.
                'If personalizzazioniRegioneUmbria <> "" And personalizzazioniRegioneUmbria IsNot Nothing Then
                '    rptStampa.Section5.ReportObjects("Text6").ObjectFormat.EnableSuppress = True
                '    rptStampa.Section5.ReportObjects("Picture5").ObjectFormat.EnableSuppress = True
                '    rptStampa.Section5.ReportObjects("Text14").ObjectFormat.EnableSuppress = True
                'End If

                Log_Errori = ""

                'carico i dati nel datatable x il quadro P
                Carica_DSMovimentiMagazzino(DSMovimentiMagazzino, Log_Errori, Qs_CaricoScarico)

                Dim drLogo = DSLogoFooter.DT_LogoFooter.NewDT_LogoFooterRow
                Dim logo As ConfigurazioneLoghiStampe = TrovaLogoFooterStampe(customLoghi, Log_Errori, objParametri_Server)
                If logo.LogoStampe IsNot Nothing Then
                    drLogo.Logo = logo.LogoStampe
                    drLogo.TestoPostLogo = logo.TestoPostLogo
                    drLogo.TestoPreLogo = logo.TestoPreLogo
                End If
                DSLogoFooter.DT_LogoFooter.Rows.Add(drLogo)

                'sorgente dati.....
                rptStampa.SetDataSource(DSMovimentiMagazzino)
                rptFooterLogo.SetDataSource(DSLogoFooter)
                rptStampa.OpenSubreport("FooterLogo.rpt").SetDataSource(DSLogoFooter)
            Catch exc As Exception
                Log_Errori += "- PageLoad: " + vbCrLf + exc.Message + vbCrLf
            End Try

            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------
            'Dim Path_Errore, Str_Errore_Path, Nome_File As String

            '-----------------------------------------

            '    CrystalReportViewer1.DisplayToolbar = True

            '    'faccio il databind col visualizzatore dei reports...
            '    CrystalReportViewer1.ReportSource = rptStampa
            '    CrystalReportViewer1.DataBind()

            '    'array di dataset e data table
            '    Dim dsRpt() As DataSet = {DSMovimentiMagazzino}

            '    'salvo il report nella sessione
            '    Session("DS") = dsRpt


            'Else 'ad ogni post back devo impostare la fonte dati x il visualizzatore..

            '    If Not IsNothing(Session("DS")) Then

            '        'imposto la sorgente dati x il report...
            '        rptStampa.SetDataSource(CType(Session("DS")(0), DataSet))

            '        'faccio il databind col visualizzatore dei reports...
            '        CrystalReportViewer1.ReportSource = rptStampa
            '        CrystalReportViewer1.DataBind()

            '    End If


            Dim nomeFile As String

            '==================================================================

            'MS Eliminato passaggio in session per passaggio report su file: Session("Report") = rptStampa
            Try
                Dim identificazioneDocumento = "MovimentiMagazzino" & "_p" & Qs_Piva & "_" & Format(Date.Now, "yyyy_MM_dd")
                nomeFile = Stringhe.EliminaCaratteriSpecialiFile(identificazioneDocumento) & ".pdf"
                Dim catCod As Integer = enum_CategorieDocumenti.Magazzino_Movimenti

                ' leggo la sotto cartella da CategorieDocumenti
                Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
                Dim sottoCartella As String = objCatDoc.Sottocartella(catCod, "", "", objParametri_Server)
                objCatDoc = Nothing

                ' salvo il report in formato PDF
                Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
                objGestFile.SalvaReportPdf(rptStampa, catCod,
                               sottoCartella, nomeFile,
                               objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)

            Catch ex As Exception
                Log_Errori += "- Salvataggio report PDF: " + vbCrLf + ex.Message + vbCrLf
            End Try

            Dim reportTemporano As String = CrystalHelper.getFileReportTemporaneo()
            Try
                rptStampa.SaveAs(reportTemporano, True)
            Catch ex As Exception
                Log_Errori += "- Salvataggio report temporaneo: " + vbCrLf + ex.Message + vbCrLf
            End Try

            Dim Nome_File As String

            If Log_Errori <> "" Then

                Log_Errori = Nome_Documento + ", " + vbCrLf +
                                "Data Inizio = " + CStr(QS_DataInizio) + ", " + vbCrLf +
                                "Data Fine = " + CStr(QS_DataFine) + ", " + vbCrLf +
                               "Piva = " + CStr(Qs_Piva) + ", " + vbCrLf +
                                "Sa_Cod = " + CStr(Qs_Sa_Cod) + ", " + vbCrLf +
                                "Fabbricato_Cod = " + CStr(Qs_Fabbricato_Cod) + ", " + vbCrLf +
                                "Elem_Cod = " + CStr(Qs_Elem_Cod) + ", " + vbCrLf +
                                "Mat_Cod = " + CStr(Qs_Mat_Cod) + ", " + vbCrLf +
                                "Pro_Cod = " + CStr(Qs_Pro_Cod) + ", " + vbCrLf +
                                "Arrotondamento = " + CStr(Qs_Arrotondamento) + ", " + vbCrLf +
                                "Ordinamento = " + CStr(Qs_Ordinamento) + ", " + vbCrLf +
                                vbCrLf + vbCrLf + vbCrLf + vbCrLf +
                                Log_Errori

                Nome_File = "Log_Errori_" + Nome_Documento + "_" + CStr(Session("ASG_Utente_Username")) & ".txt"

                'GestioneFile_CreaCartellaNelPathWebConfig("Path_LogErrori_StampeEsportazioni", "Stampe_Magazzino", Path_Errore, Str_Errore_Path)
                'If Str_Errore_Path = "" And Path_Errore <> "" Then
                '    GestioneFile_CreaScriviFileConRicercaNome(Log_Errori, Path_Errore, Nome_File, Str_Errore_Path, )
                'End If

                Dim objLog As New GestioneLogStampe
                objLog.Gestione_LogErrori_Stampe(objParametri_Server,
                                                 "Stampe_Magazzino",
                                                 Nome_File,
                                                 Session("ASG_Utente_Username"),
                                                 "SchedaMovimentiMagazzino.aspx",
                                                 Log_Errori)



            End If
            'MS Dispose del report una volta salvato su disco.
            rptStampa.Close()
            rptStampa.Dispose()
            rptStampa = Nothing

            GC.Collect()

            Response.Redirect("..\..\VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server) +
                              "&tmpReportPath=" + Stringa_Codifica(reportTemporano, AgroKey_EncoderDecoder, Server) &
                              "&NomePdf=" & Stringa_Codifica(nomeFile, AgroKey_EncoderDecoder, Server))

        End If
    End Sub

    '###########################################################################
    Private Sub Calcola_Totali_Qta(ByRef Tot_Qta_Carico As Decimal, _
                                    ByRef Tot_Qta_Scarico As Decimal, _
                                    ByVal FlagSingoloProdotto As Boolean, _
                                    ByVal Cau_Mov As String, _
                                    ByVal Qta As Decimal, _
                                    ByVal Udm_Cod As Integer, _
                                    ByVal Udm_Cod_totale As Integer)

        If FlagSingoloProdotto = True Then

            If Udm_Cod_totale = Udm_Cod Then

                If Cau_Mov = CAU_CARICO Then
                    Tot_Qta_Carico += Qta
                ElseIf Cau_Mov = CAU_SCARICO Then
                    Tot_Qta_Scarico += Qta
                End If
            Else
                'caso sgaffo
                'è cambiata l'unità di misura per lo stesso prodotto
                '-> si son sbagliati
                'non sommo la quantità
                Dim debug As Integer = 0
            End If

        End If

    End Sub

    '###########################################################################
    Private Sub Carica_DSMovimentiMagazzino(ByRef DSMovimentiMagazzino As DS_MovimentiMagazzino, _
                                            ByRef Log_Errori As String, _
                                            Optional ByVal caricoScarico As Integer = 0)

        Dim i As Integer
        Dim DT As DataTable
        Dim Lav_Cod, Udm_Cod As Integer
        Dim Cau_Mov As String

        'Dim Qta As decimal
        'Dim Giacenza_Iniziale As decimal
        'Dim Giacenza_Finale As decimal

        Dim Qta As Decimal
        Dim Giacenza_Iniziale As Decimal
        Dim Giacenza_Finale As Decimal

        '  Giulia, 06/04/2017 10:47:23: mi servono anche i numeri secchi, non arrotondati
        '       perchè per calcolare giacenza finale devo sommnare i due numeri non arrotondati ed arrotondare solo la loro somma
        Dim Qta_No_Round As Decimal
        Dim Giacenza_Iniziale_No_Round As Decimal
        Dim Giacenza_Finale_No_Round As Decimal



        Dim Prezzo_Unitario As Decimal = 0
        Dim Prezzo_Unitario_Netto As Decimal = 0
        Dim Prezzo_Unitario_DocAllegato As Decimal = 0
        Dim Prezzo_Unitario_Netto_DocAllegato As Decimal = 0
        Dim HashGiacenze As New Hashtable
        Dim Hash_key As String
        Dim Hash_value As String
        Dim xOrderBy As String
        Dim DrR As DS_MovimentiMagazzino.DS_MovimentiMagazzinoRow
        Dim objRif As New AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R

        'Dim Tot_Qta_Carico As decimal = 0
        'Dim Tot_Qta_Scarico As decimal = 0

        Dim Tot_Qta_Carico As Decimal = 0
        Dim Tot_Qta_Scarico As Decimal = 0

        Dim FlagSingoloProdotto As Boolean = False
        Dim Udm_Cod_totale As Integer
        Dim Udm_Sim_totale As String
        Dim prodotto_selezionato As String
        Dim objGiacenze2 As New AgronicaCoreContabDAL.Giacenze_R
        Dim DT_Centri As DataTable

        Try
            Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
            DT_Centri = objCentri.Leggi(Qs_Piva, 0, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
        Catch ex As Exception
            DT_Centri = Nothing
        End Try

        Try

            If (Qs_Pro_Cod = 0 And Qs_Mat_Cod <> 0) Or _
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

            DT = objMovMag.SchedaMovimentiMagazzino(CDate(QS_DataInizio), _
                                                    CDate(QS_DataFine), _
                                                        CStr(Qs_Piva), _
                                                        CInt(Qs_Sa_Cod), _
                                                        CInt(Qs_Fabbricato_Cod), _
                                                        CInt(Qs_Elem_Cod), _
                                                        CInt(Qs_Pro_Cod), _
                                                        CInt(Qs_Mat_Cod), _
                                                        0, 0, 0, 0, LOTTO_NONDEFINITO, _
                                                        filtro, filtro, filtro, filtro, _
                                                        filtro, filtro, filtro, filtro, _
                                                        filtro, filtro, filtro, "", _
                                                        xOrderBy, _
                                                        objParametri_Server, objParametri_Utenti)


            If Not IsNothing(DT) Then

                'Leggo i moduli installati
                Dim objO As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Moduli_Log_R
                Dim moduliCliente = objO.Recupera_Moduli_Cliente(Qs_Piva, objParametri_Server)

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

                For i = 0 To DT.Rows.Count - 1

                    DrR = DSMovimentiMagazzino.DS_MovimentiMagazzino.NewDS_MovimentiMagazzinoRow

                    Lav_Cod = DT.Rows(i).Item("lav_cod")

                    DrR.Data_Movimento = CDate(DT.Rows(i).Item("Data_Movimento")).ToShortDateString

                    DrR.Piva = Qs_Piva
                    DrR.Rag_Soc = DT.Rows(i).Item("Rag_Soc")
                    DrR.Sa_Cod = Qs_Sa_Cod
                    DrR.Sa_Nome = "" 'DT.Rows(i).Item("Sa_Nome")
                    DrR.Fabbricato_Cod = Qs_Fabbricato_Cod
                    DrR.Fabbricato_Des = DT.Rows(i).Item("Fabbricato_des")
                    DrR.Annata = "" 'DT.Rows(i).Item("Annata")
                    DrR.Mov_Desc = DT.Rows(i).Item("Des_Lib")

                    '23/08/2018: richiesta di agrisfera
                    If DT.Rows(i).Item("Sa_Cod_Agenda") <> 0 Then
                        Dim objHLP As New AgronicaCoreContabHLP.Contabilita
                        DrR.Mov_Desc &= " - Centro Az.: " & objHLP.Des_from_Cod(DT_Centri, "sa_cod", "sa_nome", DT.Rows(i).Item("Sa_Cod_Agenda"))
                    End If
                    DrR.Cat_Cod = DT.Rows(i).Item("Elem_Cod")
                    DrR.Cat_Des = DT.Rows(i).Item("Cat_Des")
                    Cau_Mov = DT.Rows(i).Item("Cau_Mov")
                    DrR.Cau_Mov = Cau_Mov

                    DrR.Elem_Cod = DT.Rows(i).Item("Elem_Cod")
                    DrR.Pro_Cod = IIf(Not IsDBNull(DT.Rows(i).Item("Pro_Cod")), DT.Rows(i).Item("Pro_Cod"), 0)
                    DrR.Mat_Cod = IIf(Not IsDBNull(DT.Rows(i).Item("Mat_Cod")), DT.Rows(i).Item("Mat_Cod"), 0)
                    DrR.Cod_Progetto = DT.Rows(i).Item("Cod_Progetto")
                    DrR.Fase_Cod = DT.Rows(i).Item("Fase_Cod")
                    DrR.Lotto = DT.Rows(i).Item("Lotto")
                    DrR.Cal_Cod = DT.Rows(i).Item("Cal_Cod")
                    Udm_Cod = DT.Rows(i).Item("Udm_Cod")
                    DrR.Udm_Cod = Udm_Cod
                    DrR.Udm_Sim = DT.Rows(i).Item("Udm_Sim")

                    If i = 0 Then
                        Udm_Cod_totale = DT.Rows(i).Item("Udm_Cod")
                        Udm_Sim_totale = DT.Rows(i).Item("Udm_Sim")
                    End If

                    DrR.Prodotto = DT.Rows(i).Item("Descrizione_Prodotto")
                    prodotto_selezionato = DT.Rows(i).Item("Descrizione_Prodotto")

                    Select Case DrR.Elem_Cod
                        Case FORMULATI
                            DrR.Prodotto += " (N.Reg. " + CStr(DrR.Pro_Cod) + ")"

                        Case SEMENTI

                            'If DrR.Lotto.ToLower <> "indefinito" And DrR.Lotto <> "" Then
                            '    DrR.Prodotto += " - Lotto: " + DrR.Lotto
                            'End If

                            If InStr(Qs_Filtro_ElemCod, CStr(SEMENTI) + ",") > 0 Then
                                DrR.Prodotto += " (Cod. " + CStr(DT.Rows(i).Item("Cod_articolo")) + ")"
                                prodotto_selezionato += " (Cod. " + CStr(DT.Rows(i).Item("Cod_articolo")) + ")"
                            End If

                        Case SEMILAVORATI_VEGETALI, TRASFORMATI_VEGETALI, _
                                TRASFORMATI_ANIMALI, SEMILAVORATI_ANIMALI

                            'DrR.Prodotto += " - Lotto: " + DrR.Lotto

                            If InStr(Qs_Filtro_ElemCod, CStr(DrR.Elem_Cod)) > 0 Then
                                DrR.Prodotto += " (Cod. " + CStr(DT.Rows(i).Item("Cod_articolo")) + ")"
                                prodotto_selezionato += " (Cod. " + CStr(DT.Rows(i).Item("Cod_articolo")) + ")"
                            End If

                        Case Else

                            If InStr(Qs_Filtro_ElemCod, CStr(DrR.Elem_Cod) + ",") > 0 Then
                                DrR.Prodotto += " (Cod. " + CStr(DT.Rows(i).Item("Cod_articolo")) + ")"
                                prodotto_selezionato += " (Cod. " + CStr(DT.Rows(i).Item("Cod_articolo")) + ")"
                            End If

                            'ALTRE_MATERIE,MATERIE_VEGETALI,bENI_CONFEZ_VEGETALE,SEMILAVORATI_ANIMALI,MATERIE_ANIMALI,BENI_CONFEZ_ANIMALE,TRASFORMATI_ANIMALI

                    End Select

                    '----------------------------------
                    'GESTIONE LOTTO PRODOTTI
                    If Qs_Stampalotto = 1 Then
                        'STAMPA IN BASE A CONFIGURAZIONE SU PRODOTTO
                        Dim DettagliLotto As String = ""
                        Dim objLotto As New AgronicaCoreAnagrafeDAL.Materie_PrimexLC_R

                        DettagliLotto = objLotto.Gestione_LottoProdotto(DT.Rows(i).Item("Piva"),
                                                                         DT.Rows(i).Item("Elem_Cod"),
                                                                         DT.Rows(i).Item("Mat_Cod"),
                                                                         DT.Rows(i).Item("Lotto"),
                                                                         moduliCliente,
                                                                         objParametri_Server)

                        If DettagliLotto <> "" Then
                            DrR.Prodotto += " " + DettagliLotto
                        End If

                    Else
                        'STAMPA SEMPRE
                        If CStr(DT.Rows(i).Item("Lotto")).ToLower <> "indefinito" And DT.Rows(i).Item("Lotto") <> "" Then
                            DrR.Prodotto += " - Lotto: " & DT.Rows(i).Item("Lotto")
                        End If
                    End If
                    '----------------------------------

                    Prezzo_Unitario = 0
                    Prezzo_Unitario_Netto = 0
                    Prezzo_Unitario_DocAllegato = 0
                    Prezzo_Unitario_Netto_DocAllegato = 0

                    Select Case Lav_Cod

                        Case LAVCOD_BOLLA_RICEVUTA, LAVCOD_BOLLA_EMESSA, LAVCOD_DDT_CONTABILIZZATO_EMESSO

                            Dim lav_cod_fattura As Integer
                            Dim cau_movimento As String

                            If Lav_Cod = LAVCOD_BOLLA_RICEVUTA Then
                                lav_cod_fattura = LAVCOD_FATTURA_RICEVUTA
                                cau_movimento = CAU_CARICO
                            Else
                                lav_cod_fattura = LAVCOD_FATTURA_EMESSA
                                cau_movimento = CAU_SCARICO
                            End If

                            'non posso passare come id_mov DT.Rows(i).Item("Id_Mov")
                            'perchè altrimenti non viene trovato il record... marco è un genio!

                            objRif.Recupera_PrezzoUnitario_FatturaAllegata(objParametri_Server, _
                                                                            Prezzo_Unitario_DocAllegato, _
                                                                            Prezzo_Unitario_Netto_DocAllegato, _
                                                                            DrR.Piva, _
                                                                            0, _
                                                                            DT.Rows(i).Item("Id_Agenda"), _
                                                                            0, _
                                                                            DT.Rows(i).Item("Id_Mov_Det"), _
                                                                            DT.Rows(i).Item("lav_cod"), _
                                                                            CAU_REGISTRAZIONI, _
                                                                            lav_cod_fattura, _
                                                                            cau_movimento)

                    End Select

                    If Prezzo_Unitario_DocAllegato <> 0 Then
                        Prezzo_Unitario = Prezzo_Unitario_DocAllegato
                    Else
                        Prezzo_Unitario = DT.Rows(i).Item("Prezzo_Unitario")
                    End If

                    If Prezzo_Unitario_Netto_DocAllegato <> 0 Then
                        Prezzo_Unitario_Netto = Prezzo_Unitario_Netto_DocAllegato
                    Else
                        Prezzo_Unitario_Netto = DT.Rows(i).Item("Prezzo_Unitario_Netto")
                    End If

                    If Prezzo_Unitario_Netto <> 0 Then
                        DrR.Prezzo_Unitario = Prezzo_Unitario_Netto
                    Else
                        DrR.Prezzo_Unitario = Prezzo_Unitario
                    End If

                    Qta = DT.Rows(i).Item("Qta_Dest")
                    Qta_No_Round = CDec(DT.Rows(i).Item("Qta_Dest"))

                    Select Case Qs_Arrotondamento
                        Case 0
                        Case 1
                            Qta = Math.Round(Qta, 0, MidpointRounding.AwayFromZero)
                        Case 2
                            Qta = Math.Round(Qta, 1, MidpointRounding.AwayFromZero)
                        Case 3
                            Qta = Math.Round(Qta, 2, MidpointRounding.AwayFromZero)
                        Case 4
                            Qta = Math.Round(Qta, 3, MidpointRounding.AwayFromZero)
                        Case 5
                            Qta = Math.Round(Qta, 4, MidpointRounding.AwayFromZero)
                    End Select

                    Calcola_Totali_Qta(Tot_Qta_Carico, _
                                        Tot_Qta_Scarico, _
                                        FlagSingoloProdotto, _
                                        Cau_Mov, _
                                        Qta_No_Round, _
                                        Udm_Cod, _
                                        Udm_Cod_totale)



                    Hash_key = CStr(DrR.Elem_Cod) + "|" + CStr(DrR.Pro_Cod) + "|" + CStr(DrR.Mat_Cod) + "|" + _
                                CStr(DrR.Cod_Progetto) + "|" + CStr(DrR.Fase_Cod) + "|" + CStr(DrR.Lotto) + "|" + _
                                CStr(DrR.Cal_Cod) + "|" + CStr(DrR.Udm_Cod)

                    If Not HashGiacenze.ContainsKey(Hash_key) Then
                        'il prodotto non è presente in lista
                        'come giacenza, ricavo la sua giacenza al giorno prima
                        'Giacenza_Iniziale = NewCom_Verifica_Giacenze(Server, Session, Page, _
                        '                                    CStr(Qs_Piva), _
                        '                                    CInt(Qs_Sa_Cod), _
                        '                                    CInt(Qs_Fabbricato_Cod), _
                        '                                    CInt(DrR.Elem_Cod), _
                        '                                    CInt(DrR.Pro_Cod), _
                        '                                    CInt(DrR.Mat_Cod), _
                        '                                    CInt(DrR.Cod_Progetto), _
                        '                                    CInt(DrR.Fase_Cod), _
                        '                                    CStr(DrR.Lotto), _
                        '                                    CInt(DrR.Cal_Cod), _
                        '                                    CStr(DrR.Udm_Cod), _
                        '                                    AGRODATAINIZIO, _
                        '                                    CDate(DateAdd(DateInterval.Day, -1, CDate(QS_DataInizio))), _
                        '                                     , )


                        Giacenza_Iniziale = objGiacenze2.Verifica_Giacenza(CDate(DateAdd(DateInterval.Day, -1, CDate(QS_DataInizio))), _
                                                                          CStr(Qs_Piva), _
                                                                            CInt(Qs_Sa_Cod), _
                                                                            CInt(Qs_Fabbricato_Cod), _
                                                                            CInt(DrR.Elem_Cod), _
                                                                            CInt(DrR.Pro_Cod), _
                                                                            CInt(DrR.Mat_Cod), _
                                                                            CInt(DrR.Cal_Cod), _
                                                                            CInt(DrR.Cod_Progetto), _
                                                                            CInt(DrR.Fase_Cod), _
                                                                             CStr(DrR.Udm_Cod), _
                                                                            CStr(DrR.Lotto), _
                                                                            objParametri_Server, objParametri_Utenti)

                        'Giacenza_Iniziale_No_Round = CDec(Giacenza_Iniziale)

                        'Select Case Qs_Arrotondamento
                        '    Case 0
                        '    Case 1
                        '        Giacenza_Iniziale = Math.Round(Giacenza_Iniziale, 0, MidpointRounding.AwayFromZero)
                        '    Case 2
                        '        Giacenza_Iniziale = Math.Round(Giacenza_Iniziale, 1, MidpointRounding.AwayFromZero)
                        '    Case 3
                        '        Giacenza_Iniziale = Math.Round(Giacenza_Iniziale, 2, MidpointRounding.AwayFromZero)
                        '    Case 4
                        '        Giacenza_Iniziale = Math.Round(Giacenza_Iniziale, 3, MidpointRounding.AwayFromZero)
                        '    Case 5
                        '        Giacenza_Iniziale = Math.Round(Giacenza_Iniziale, 4, MidpointRounding.AwayFromZero)
                        'End Select

                        HashGiacenze.Add(Hash_key, Giacenza_Iniziale)

                    Else
                        'il prodotto è già presente
                        Giacenza_Iniziale = CDec(HashGiacenze(Hash_key))
                    End If


                    ''ricavo la giacenza dalla matrice dei prodotti
                    'DrR.Giacenza_Iniziale = _Leggi_Giacenza_da_MatriceProdotti(DrR.Elem_Cod, _
                    '                                                            DrR.Pro_Cod, _
                    '                                                            DrR.Mat_Cod, _
                    '                                                            DrR.Cod_Progetto, _
                    '                                                            DrR.Fase_Cod, _
                    '                                                            DrR.Lotto, _
                    '                                                            DrR.Cal_Cod, _
                    '                                                            DrR.Udm_Cod)

                    '  Giulia, 06/04/2017 10:59:05: Mi tocca fare l'arrotondamento della giacenza inziale qui ogni volta

                    Giacenza_Iniziale_No_Round = CDec(Giacenza_Iniziale)

                    Select Case Qs_Arrotondamento
                        Case 0
                        Case 1
                            Giacenza_Iniziale = Math.Round(Giacenza_Iniziale, 0, MidpointRounding.AwayFromZero)
                        Case 2
                            Giacenza_Iniziale = Math.Round(Giacenza_Iniziale, 1, MidpointRounding.AwayFromZero)
                        Case 3
                            Giacenza_Iniziale = Math.Round(Giacenza_Iniziale, 2, MidpointRounding.AwayFromZero)
                        Case 4
                            Giacenza_Iniziale = Math.Round(Giacenza_Iniziale, 3, MidpointRounding.AwayFromZero)
                        Case 5
                            Giacenza_Iniziale = Math.Round(Giacenza_Iniziale, 4, MidpointRounding.AwayFromZero)
                    End Select


                    Select Case DrR.Cau_Mov
                        Case CAU_CARICO, CAU_CONFERIMENTO
                            DrR.Cau_Mov = "+"
                            Giacenza_Finale = Giacenza_Iniziale_No_Round + Qta_No_Round
                        Case Else
                            DrR.Cau_Mov = "-"
                            Giacenza_Finale = Giacenza_Iniziale_No_Round - Qta_No_Round
                    End Select

                    Giacenza_Finale_No_Round = Giacenza_Finale

                    Select Case Qs_Arrotondamento
                        Case 0
                        Case 1
                            Giacenza_Finale = Math.Round(Giacenza_Finale, 0, MidpointRounding.AwayFromZero)
                        Case 2
                            Giacenza_Finale = Math.Round(Giacenza_Finale, 1, MidpointRounding.AwayFromZero)
                        Case 3
                            Giacenza_Finale = Math.Round(Giacenza_Finale, 2, MidpointRounding.AwayFromZero)
                        Case 4
                            Giacenza_Finale = Math.Round(Giacenza_Finale, 3, MidpointRounding.AwayFromZero)
                        Case 5
                            Giacenza_Finale = Math.Round(Giacenza_Finale, 4, MidpointRounding.AwayFromZero)
                    End Select

                    'aggiorno la giacenza
                    HashGiacenze(Hash_key) = Giacenza_Finale_No_Round

                    ''aggiorno la giacenza sulla matrice dei prodotti
                    '_Aggiorna_Giacenza_su_MatriceProdotti(DrR.Elem_Cod, _
                    '                                        DrR.Pro_Cod, _
                    '                                        DrR.Mat_Cod, _
                    '                                        DrR.Cod_Progetto, _
                    '                                        DrR.Fase_Cod, _
                    '                                        DrR.Lotto, _
                    '                                        DrR.Cal_Cod, _
                    '                                        DrR.Udm_Cod, _
                    '                                        DrR.Giacenza_Finale)


                    DrR.Qta = Qta
                    DrR.Giacenza_Iniziale = Giacenza_Iniziale
                    DrR.Giacenza_Finale = Giacenza_Finale

                    Select Case Qs_Arrotondamento

                        Case 0 'nessun arrotondamento
                            DrR.Str_Qta = CStr(Qta)
                            DrR.Str_Giacenza_Iniziale = CStr(Giacenza_Iniziale)
                            DrR.Str_Giacenza_Finale = CStr(Giacenza_Finale)

                        Case 1 'arrotondamento all'intero
                            DrR.Str_Qta = Format(Qta, "#,###,##0")
                            DrR.Str_Giacenza_Iniziale = Format(Giacenza_Iniziale, "#,###,##0")
                            DrR.Str_Giacenza_Finale = Format(Giacenza_Finale, "#,###,##0")

                        Case 2 'arrotondamento al primo decimale
                            DrR.Str_Qta = Format(Qta, "#,###,##0.0")
                            DrR.Str_Giacenza_Iniziale = Format(Giacenza_Iniziale, "#,###,##0.0")
                            DrR.Str_Giacenza_Finale = Format(Giacenza_Finale, "#,###,##0.0")

                        Case 3 '2 decimali
                            DrR.Str_Qta = Format(Qta, "#,###,##0.00")
                            DrR.Str_Giacenza_Iniziale = Format(Giacenza_Iniziale, "#,###,##0.00")
                            DrR.Str_Giacenza_Finale = Format(Giacenza_Finale, "#,###,##0.00")

                        Case 4 '3 decimali
                            DrR.Str_Qta = Format(Qta, "#,###,##0.000")
                            DrR.Str_Giacenza_Iniziale = Format(Giacenza_Iniziale, "#,###,##0.000")
                            DrR.Str_Giacenza_Finale = Format(Giacenza_Finale, "#,###,##0.000")

                        Case 5 '4 decimali
                            DrR.Str_Qta = Format(Qta, "#,###,##0.0000")
                            DrR.Str_Giacenza_Iniziale = Format(Giacenza_Iniziale, "#,###,##0.0000")
                            DrR.Str_Giacenza_Finale = Format(Giacenza_Finale, "#,###,##0.0000")
                    End Select

                    DrR.Totale = (DrR.Prezzo_Unitario * Qta)

                    DSMovimentiMagazzino.DS_MovimentiMagazzino.Rows.Add(DrR)

                Next

            End If

            If Qs_Lotto <> "" Then
                prodotto_selezionato &= " Filtro Lotto: " & Qs_Lotto
            End If

            Visualizza_Totali_PerSingoloProdotto(Log_Errori, _
                                                FlagSingoloProdotto, _
                                                Tot_Qta_Carico, _
                                                Tot_Qta_Scarico, _
                                                Udm_Sim_totale, _
                                                prodotto_selezionato)


        Catch ex As Exception
            Log_Errori += "Errore durante il caricamento del dataset: " + ex.Message
        End Try


    End Sub

    '###########################################################################
    Private Sub Visualizza_Totali_PerSingoloProdotto(ByRef Log_Errori As String,
                                                    ByVal FlagSingoloProdotto As Boolean,
                                                    ByVal Tot_Qta_Carico As Decimal,
                                                    ByVal Tot_Qta_Scarico As Decimal,
                                                    ByVal Udm_Sim_totale As String,
                                                    ByVal prodotto_selezionato As String)

        Try

            If FlagSingoloProdotto = True Then

                CType(rptStampa.Section4.ReportObjects("LineaDifferenza"), CrystalDecisions.CrystalReports.Engine.LineObject).ObjectFormat.EnableSuppress = False

                CType(rptStampa.Section1.ReportObjects("TxtProdotto"), CrystalDecisions.CrystalReports.Engine.TextObject).Text =
                "Prodotto selezionato: " & prodotto_selezionato


                'rptStampa.Section4.SectionFormat.EnableSuppress = True
                '  rptStampa.Section6.SectionFormat.EnableSuppress = False
                Dim TxtTotCarichi, TxtTotScarichi, differenza As String

                Select Case Qs_Arrotondamento
                    Case 0 'nessun arrotondamento
                        TxtTotCarichi = Tot_Qta_Carico
                        TxtTotScarichi = Tot_Qta_Scarico
                        differenza = CDec(Tot_Qta_Carico - Tot_Qta_Scarico).ToString
                    Case 1 'arrotondamento all'intero
                        TxtTotCarichi = Format(Math.Round(Tot_Qta_Carico, 0, MidpointRounding.AwayFromZero), "#,###,##0")
                        TxtTotScarichi = Format(Math.Round(Tot_Qta_Scarico, 0, MidpointRounding.AwayFromZero), "#,###,##0")
                        differenza = Format(CDec(Math.Round(Tot_Qta_Carico - Tot_Qta_Scarico, 0, MidpointRounding.AwayFromZero)), "#,###,##0")
                    Case 2 'arrotondamento al primo decimale
                        TxtTotCarichi = Format(Math.Round(Tot_Qta_Carico, 1, MidpointRounding.AwayFromZero), "#,###,##0.0")
                        TxtTotScarichi = Format(Math.Round(Tot_Qta_Scarico, 1, MidpointRounding.AwayFromZero), "#,###,##0.0")
                        differenza = Format(CDec(Math.Round(Tot_Qta_Carico - Tot_Qta_Scarico, 1, MidpointRounding.AwayFromZero)), "#,###,##0.0")
                    Case 3 '2 decimali
                        TxtTotCarichi = Format(Math.Round(Tot_Qta_Carico, 2, MidpointRounding.AwayFromZero), "#,###,##0.00")
                        TxtTotScarichi = Format(Math.Round(Tot_Qta_Scarico, 2, MidpointRounding.AwayFromZero), "#,###,##0.00")
                        differenza = Format(CDec(Math.Round(Tot_Qta_Carico - Tot_Qta_Scarico, 2, MidpointRounding.AwayFromZero)), "#,###,##0.00")
                    Case 4 '3 decimali
                        TxtTotCarichi = Format(Math.Round(Tot_Qta_Carico, 3, MidpointRounding.AwayFromZero), "#,###,##0.000")
                        TxtTotScarichi = Format(Math.Round(Tot_Qta_Scarico, 3, MidpointRounding.AwayFromZero), "#,###,##0.000")
                        differenza = Format(CDec(Math.Round(Tot_Qta_Carico - Tot_Qta_Scarico, 3, MidpointRounding.AwayFromZero)), "#,###,##0.000")
                    Case 5 '4 decimali
                        TxtTotCarichi = Format(Math.Round(Tot_Qta_Carico, 4, MidpointRounding.AwayFromZero), "#,###,##0.0000")
                        TxtTotScarichi = Format(Math.Round(Tot_Qta_Scarico, 4, MidpointRounding.AwayFromZero), "#,###,##0.0000")
                        differenza = Format(CDec(Math.Round(Tot_Qta_Carico - Tot_Qta_Scarico, 4, MidpointRounding.AwayFromZero)), "#,###,##0.0000")
                End Select

                If CInt(Qs_Mat_Cod) <> 0 Then

                    Dim objMP As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
                    Dim filtro As String = " Udm_Cod_Extra <> 0 AND qta_extra <> 0 "
                    Dim udm_cod_extra, peso_set, flag_extra As Integer
                    Dim udm_sim_extra, udm_des_extra As String
                    Dim qta_extra As Decimal = 0

                    'peso_set =0 -< capacità nominale, in anagrafica materie prime
                    objMP.Recupera_Extra(Qs_Elem_Cod,
                                        Qs_Mat_Cod,
                                        0,
                                        filtro,
                                        udm_cod_extra,
                                        udm_sim_extra,
                                        udm_des_extra,
                                        qta_extra,
                                        peso_set,
                                        flag_extra,
                                        objParametri_Server)

                    If qta_extra <> 0 And udm_cod_extra <> 0 Then
                        Dim TxtTotCarichiExtra, TxtTotScarichiExtra, TxtDifferenzaExtra As String
                        Dim Tot_Qta_CaricoExtra, Tot_Qta_ScaricoExtra As Decimal

                        Tot_Qta_CaricoExtra = Tot_Qta_Carico * qta_extra
                        Tot_Qta_ScaricoExtra = Tot_Qta_Scarico * qta_extra

                        Select Case Qs_Arrotondamento
                            Case 0 'nessun arrotondamento
                                TxtTotCarichiExtra = Tot_Qta_CaricoExtra
                                TxtTotScarichiExtra = Tot_Qta_ScaricoExtra
                                TxtDifferenzaExtra = CDec(Tot_Qta_CaricoExtra - Tot_Qta_ScaricoExtra).ToString
                            Case 1 'arrotondamento all'intero
                                TxtTotCarichiExtra = Format(Tot_Qta_CaricoExtra, "#,###,##0")
                                TxtTotScarichiExtra = Format(Tot_Qta_ScaricoExtra, "#,###,##0")
                                TxtDifferenzaExtra = Format(CDec(Tot_Qta_CaricoExtra - Tot_Qta_ScaricoExtra), "#,###,##0")
                            Case 2 'arrotondamento al primo decimale
                                TxtTotCarichiExtra = Format(Tot_Qta_CaricoExtra, "#,###,##0.0")
                                TxtTotScarichiExtra = Format(Tot_Qta_ScaricoExtra, "#,###,##0.0")
                                TxtDifferenzaExtra = Format(CDec(Tot_Qta_CaricoExtra - Tot_Qta_ScaricoExtra), "#,###,##0.0")
                            Case 3 '2 decimali
                                TxtTotCarichiExtra = Format(Tot_Qta_CaricoExtra, "#,###,##0.00")
                                TxtTotScarichiExtra = Format(Tot_Qta_ScaricoExtra, "#,###,##0.00")
                                TxtDifferenzaExtra = Format(CDec(Tot_Qta_CaricoExtra - Tot_Qta_ScaricoExtra), "#,###,##0.00")
                            Case 4 '3 decimali
                                TxtTotCarichiExtra = Format(Tot_Qta_CaricoExtra, "#,###,##0.000")
                                TxtTotScarichiExtra = Format(Tot_Qta_ScaricoExtra, "#,###,##0.000")
                                TxtDifferenzaExtra = Format(CDec(Tot_Qta_CaricoExtra - Tot_Qta_ScaricoExtra), "#,###,##0.000")
                            Case 5 '4 decimali
                                TxtTotCarichiExtra = Format(Tot_Qta_CaricoExtra, "#,###,##0.0000")
                                TxtTotScarichiExtra = Format(Tot_Qta_ScaricoExtra, "#,###,##0.0000")
                                TxtDifferenzaExtra = Format(CDec(Tot_Qta_CaricoExtra - Tot_Qta_ScaricoExtra), "#,###,##0.0000")
                        End Select


                        CType(rptStampa.Section4.ReportObjects("TxtTotCarichiQtaExtra"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = TxtTotCarichiExtra & " " & udm_sim_extra
                        CType(rptStampa.Section4.ReportObjects("TxtTotScarichiQtaExtra"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = TxtTotScarichiExtra & " " & udm_sim_extra
                        CType(rptStampa.Section4.ReportObjects("TxtDifferenzaExtra"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = TxtDifferenzaExtra & " " & udm_sim_extra
                    End If 'extra

                End If 'mat_cod <>0

                CType(rptStampa.Section4.ReportObjects("TxtTotCarichi"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = TxtTotCarichi & " " & Udm_Sim_totale
                CType(rptStampa.Section4.ReportObjects("TxtTotScarichi"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = TxtTotScarichi & " " & Udm_Sim_totale
                CType(rptStampa.Section4.ReportObjects("TxtDifferenza"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = differenza & " " & Udm_Sim_totale

            Else
                'tutti i prodotti
                CType(rptStampa.Section4.ReportObjects("LblCarichi"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = ""
                CType(rptStampa.Section4.ReportObjects("LblScarichi"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = ""
                CType(rptStampa.Section4.ReportObjects("LblDifferenza"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = ""
            End If 'FlagSingoloProdotto

        Catch ex As Exception
            Log_Errori += "Visualizzazione totali: " + ex.Message
        End Try

    End Sub


End Class
