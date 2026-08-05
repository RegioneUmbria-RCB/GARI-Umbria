Imports AgronicaControlli_2010
Imports AgronicaControlli_2010.UtilityPersonalizzazioniGraficheCliente
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModelsSTD.PersonalizzazioniGraficheCliente
Imports CrystalDecisions.Shared

Public Class RegistroPreparazioniBio
    Inherits System.Web.UI.Page

    '----------------------------
    '   DICHIARAZIONE REPORT
    '----------------------------
    Private Rpt_PrepBIO As Rpt_RegistroPreparazioniBio        ' report contenitore
    Private Rpt_MP As SottoRpt_MateriePrime
    Private Rpt_Lav As SottoRpt_CicloLavorazione
    Private Rpt_Agenda As SottoRpt_Agenda
    Private rptFooterLogo As FooterLogo

    '----- Gestione Querystring
    Dim Qs_Piva As String
    Dim Qs_MatDes As String
    Dim Qs_RagSoc As String
    Dim Qs_SaNome As String
    Dim Qs_SaCod As Integer
    Dim Qs_Fabbricato_Cod As Integer
    Dim Qs_MatCod As Integer
    Dim Qs_ElemCod As Integer
    Dim Qs_LineaCod As Integer
    Dim Qs_PreparazioneCod As Integer
    Dim Qs_FlagRegione As Integer
    Dim Qs_RegioneCod As String
    Dim QS_DataInizio As String
    Dim QS_DataFine As String
    Dim QS_Chk_SezioneA As Boolean
    Dim QS_Chk_SezioneB As Boolean


    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Super_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri

    Dim personalizzazioniRegioneUmbria = ""

    Dim customLoghi As PersonalizzazioniGraficheCliente = Nothing

#Region " PREPARAZIONI BIO "

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

        'istanzio gli oggetti report
        Rpt_PrepBIO = New Rpt_RegistroPreparazioniBio
        Rpt_MP = New SottoRpt_MateriePrime
        Rpt_Lav = New SottoRpt_CicloLavorazione
        Rpt_Agenda = New SottoRpt_Agenda
        rptFooterLogo = New FooterLogo

    End Sub

#End Region


    '##############################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'Faccio scadere subito la pagina memorizzata nella cache
        Response.Expires = 0


        '#################################################################################
        '#####  Recupero dati dalla QueryString 
        '#################################################################################

        Qs_Piva = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("p")),
                                            AgroKey_EncoderDecoder,
                                            Server)

        Qs_RagSoc = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("rs")),
                                                AgroKey_EncoderDecoder,
                                                Server)

        Qs_SaCod = CInt(AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("s")),
                                            AgroKey_EncoderDecoder,
                                            Server))

        Qs_Fabbricato_Cod = CInt(AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("f")),
                           AgroKey_EncoderDecoder,
                           Server))


        Qs_SaNome = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("sn")),
                                                AgroKey_EncoderDecoder,
                                                Server)

        Qs_MatCod = CInt(AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("m")),
                                                    AgroKey_EncoderDecoder,
                                                    Server))

        Qs_MatDes = CStr(AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("mdes")),
                                            AgroKey_EncoderDecoder,
                                            Server))

        Qs_ElemCod = CInt(AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("e")),
                                                  AgroKey_EncoderDecoder,
                                                  Server))

        Qs_LineaCod = CInt(AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("lc")),
                                            AgroKey_EncoderDecoder,
                                            Server))

        Qs_PreparazioneCod = CInt(AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("pc")),
                                            AgroKey_EncoderDecoder,
                                            Server))

        Qs_FlagRegione = CInt(AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("fr")),
                                     AgroKey_EncoderDecoder,
                                     Server))

        Qs_RegioneCod = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("rc")),
                                              AgroKey_EncoderDecoder,
                                              Server)

        QS_DataInizio = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("di")),
                 AgroKey_EncoderDecoder,
                 Server)

        QS_DataFine = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("df")),
                         AgroKey_EncoderDecoder,
                         Server)

        QS_Chk_SezioneA = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("seza")),
                    AgroKey_EncoderDecoder,
                    Server)

        QS_Chk_SezioneB = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("sezb")),
                AgroKey_EncoderDecoder,
                Server)

        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Super_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Super_Server"))

        customLoghi = UtilityPersonalizzazioniGraficheCliente.LeggiPersonalizzazioniGraficheCliente(objParametri_Server, objParametri_Super_Server)

        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        '    Dim CrystalReportViewer1 As CrystalDecisions.Web.CrystalReportViewer

        Dim Nome_Documento As String = "Registro_Preparazioni_BIO"
        Dim IdentificazioneDocumento As String
        Dim Log_Errori As String
        Dim strLogRiepilogo As String

        If Not Me.IsPostBack Then

            'CrystalReportViewer1 = New CrystalDecisions.Web.CrystalReportViewer
            'CrystalReportViewer1.Style.Add("LEFT", "-275px")
            'CrystalReportViewer1.Style.Add("TOP", "0px")
            'CrystalReportViewer1.Style.Add("POSITION", "Absolute")
            'Me.FindControl("Form1").Controls.Add(CrystalReportViewer1)

            '--------------------------------------------
            ' DICHIARAZIONE  E INIZIALIZZAZIONE DATASET
            '--------------------------------------------
            Dim DS_PrepBIO As New DS_RegistroPreparazioniBio
            Dim DS_Lav As New DS_CicloLavorazione
            Dim DS_MP As New Ds_MateriePrime
            Dim DS_Agenda As New DS_Agenda
            Dim DS_LogoFooter As New DS_LogoFooter

            Log_Errori = ""

            Try
                Select Case Qs_ElemCod
                    Case SEMILAVORATI_VEGETALI, SEMILAVORATI_ANIMALI
                        CType(Rpt_PrepBIO.Section3.ReportObjects("TxtProdottoFinito"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = ""
                    Case TRASFORMATI_VEGETALI, TRASFORMATI_ANIMALI
                        CType(Rpt_PrepBIO.Section3.ReportObjects("TxtProdottoSemilavorato"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = ""
                End Select

                CType(Rpt_PrepBIO.Section3.ReportObjects("TxtDescrizione"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Qs_MatDes

                'If customLoghi IsNot Nothing Then
                '    Rpt_PrepBIO.Section5.ReportObjects("Text14").ObjectFormat.EnableSuppress = True
                '    Rpt_PrepBIO.Section5.ReportObjects("Picture5").ObjectFormat.EnableSuppress = True
                '    Rpt_PrepBIO.Section5.ReportObjects("Text23").ObjectFormat.EnableSuppress = True
                'End If

                Dim Cod_Regione As String = ""

                Carica_Intestazione(DS_PrepBIO, Log_Errori, Cod_Regione)

                'imposta il logo della regione in base a Cod_Regione
                VisualizzaLogoRegione(Log_Errori)

                If QS_Chk_SezioneA = True Then
                    Carica_MateriePrime_CicloLavorazione(DS_MP, DS_Lav, Log_Errori)
                Else
                    'titolo
                    'Rpt_PrepBIO.Section1.SectionFormat.EnableSuppress = True
                    'intestazione
                    'Rpt_PrepBIO.Section12.SectionFormat.EnableSuppress = True
                    'sezione A
                    Rpt_PrepBIO.Section3.SectionFormat.EnableSuppress = True
                    'materieprime
                    Rpt_PrepBIO.Section9.SectionFormat.EnableSuppress = True
                    'ciclo lavorazione
                    Rpt_PrepBIO.Section10.SectionFormat.EnableSuppress = True
                End If

                If QS_Chk_SezioneB = True Then
                    Carica_Agenda(DS_Agenda, Log_Errori)
                Else
                    'sezione B - agenda
                    Rpt_PrepBIO.Section11.SectionFormat.EnableSuppress = True
                End If

                Dim drLogo = DS_LogoFooter.DT_LogoFooter.NewDT_LogoFooterRow
                Dim logo As ConfigurazioneLoghiStampe = TrovaLogoFooterStampe(customLoghi, Log_Errori, objParametri_Server)
                If logo.LogoStampe IsNot Nothing Then
                    drLogo.Logo = logo.LogoStampe
                    drLogo.TestoPostLogo = logo.TestoPostLogo
                    drLogo.TestoPreLogo = logo.TestoPreLogo
                End If
                DS_LogoFooter.DT_LogoFooter.Rows.Add(drLogo)

            Catch ex As Exception
                Log_Errori += "- Caricamento dati: " + vbCrLf + ex.Message + vbCrLf
            End Try

            '=============================================================
            'Aggancio dataset

            Try

                'imposto il dataset sul report
                Rpt_PrepBIO.SetDataSource(DS_PrepBIO)
                Rpt_PrepBIO.SetDataSource(DS_LogoFooter)
                Rpt_PrepBIO.OpenSubreport("FooterLogo.rpt").SetDataSource(DS_LogoFooter)

            Catch ex As Exception
                Log_Errori += "- Aggancio dataset: " + vbCrLf + ex.Message + vbCrLf
            End Try


            Try
                ' carico i sottoreport

                If QS_Chk_SezioneA = True Then
                    Rpt_PrepBIO.OpenSubreport("SottoRpt_MateriePrime.rpt").SetDataSource(DS_MP)
                    Rpt_PrepBIO.OpenSubreport("SottoRpt_CicloLavorazione.rpt").SetDataSource(DS_Lav)
                End If

            Catch ex As Exception
                Log_Errori += "- Open sottoreport SezioneA: " + vbCrLf + ex.Message + vbCrLf
            End Try

            Try

                If QS_Chk_SezioneB = True Then
                    Rpt_PrepBIO.OpenSubreport("SottoRpt_Agenda.rpt").SetDataSource(DS_Agenda)
                End If

            Catch ex As Exception
                Log_Errori += "- Open sottoreport SezioneB: " + vbCrLf + ex.Message + vbCrLf
            End Try

            Try

                Dim Data_Inizio_Allegati, Data_Fine_Allegati As Date

                If QS_DataInizio <> AGRODATAINIZIO And QS_DataFine <> AGRODATAFINE Then
                    Data_Inizio_Allegati = QS_DataInizio
                    Data_Fine_Allegati = QS_DataFine
                    IdentificazioneDocumento += "_" + Format(QS_DataInizio, "yyyy_MM_dd") + "_" + Format(QS_DataFine, "yyyy_MM_dd")
                    'Else
                    '    Data_Inizio_Allegati = "01/01/" & CStr(Anno)
                    '    Data_Fine_Allegati = "31/12/" & CStr(Anno)
                End If

                ' leggo la sottocartella da CategorieDocumenti
                Dim Sottocartella As String
                Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
                Sottocartella = objCatDoc.Sottocartella(enum_CategorieDocumenti.Biologico_Preparazioni, "", "", objParametri_Server)

                ' salvo il report in formato PDF
                Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
                objGestFile.SalvaReportPdf(Rpt_PrepBIO,
                                           enum_CategorieDocumenti.Biologico_Preparazioni,
                                           Sottocartella,
                                           Nome_Documento + "_p" & Qs_Piva + "_" + IdentificazioneDocumento + ".pdf",
                                           objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)

                Dim objAllegati As New AgronicaCoreScadenziario_BIZ.Allegati
                Dim AllegatiDocumentiCod As Integer

                AllegatiDocumentiCod = objAllegati.SalvaAllegato(Qs_Piva,
                                                                 enum_CategorieDocumenti.Biologico_Preparazioni,
                                                                 Nome_Documento,
                                                                 Nome_Documento + "_p" & Qs_Piva + "_" + IdentificazioneDocumento + ".pdf",
                                                                 Sottocartella,
                                                                 "", "", "", "",
                                                                 Data_Inizio_Allegati,
                                                                 Data_Fine_Allegati,
                                                                 objParametri_Server)



            Catch ex As Exception
                Log_Errori += "- Gestione allegati: " + vbCrLf + ex.Message + vbCrLf
            End Try


            '-----------------------------------------
            '---- salva report -------------
            '-----------------------------------------

            'MS Eliminato passaggio in session per passaggio report su file: Session("Report") = Rpt_PrepBIO
            Dim reportTemporaneo As String = CrystalHelper.getFileReportTemporaneo()
            Try
                Rpt_PrepBIO.SaveAs(reportTemporaneo, True)
            Catch ex As Exception
                Log_Errori += "- Salvataggio report temporaneo: " + vbCrLf + ex.Message + vbCrLf
            End Try

            DS_PrepBIO.Dispose()
            DS_PrepBIO = Nothing

            Rpt_PrepBIO.Close()
            Rpt_PrepBIO.Dispose()
            Rpt_PrepBIO = Nothing

            GC.Collect()


            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------
            ' Dim Path_Errore, Str_Errore_Path As String
            Dim Nome_File As String

            If Log_Errori <> "" Then

                Log_Errori = Nome_Documento + ", " + vbCrLf +
                                "Piva = " + CStr(Qs_Piva) + ", " + vbCrLf +
                                "Sa_Cod = " + CStr(Qs_SaCod) + ", " + vbCrLf +
                                "Rag_Soc = " + CStr(Qs_RagSoc) + ", " + vbCrLf +
                                "Sa_Nome = " + CStr(Qs_SaNome) + ", " + vbCrLf +
                                "Mat_Cod = " + CStr(Qs_MatCod) + ", " + vbCrLf +
                                "Mat_Des = " + CStr(Qs_MatDes) + ", " + vbCrLf +
                                "Elem_Cod = " + CStr(Qs_ElemCod) + ", " + vbCrLf +
                                "Linea_Cod  = " + CStr(Qs_LineaCod) + ", " + vbCrLf +
                                "Preparazione_Cod   = " + CStr(Qs_PreparazioneCod) + ", " + vbCrLf +
                                vbCrLf + vbCrLf + vbCrLf + vbCrLf +
                                Log_Errori

                Nome_File = "Log_Errori_" + Nome_Documento + "_" + CStr(Session("ASG_Utente_Username")) + ".txt"

                'GestioneFile_CreaCartellaNelPathWebConfig("Path_LogErrori_StampeEsportazioni", "Stampe_BIO", Path_Errore, Str_Errore_Path)
                'If Str_Errore_Path = "" And Path_Errore <> "" Then
                '    GestioneFile_CreaScriviFileConRicercaNome(Log_Errori, Path_Errore, Nome_File, Str_Errore_Path, )
                'End If

                Dim objLog As New GestioneLogStampe
                objLog.Gestione_LogErrori_Stampe(objParametri_Server,
                                                 "Stampe_Biologico",
                                                 Nome_File,
                                                 Session("ASG_Utente_Username"),
                                                 "RegistroPreparazioniBio.aspx",
                                                 Log_Errori)

            End If
            '-----------------------------------------

            Response.Redirect("..\..\VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server) &
                      "&tmpReportPath=" + Stringa_Codifica(reportTemporaneo, AgroKey_EncoderDecoder, Server) &
                        "&NomePdf=" & Stringa_Codifica(Nome_Documento, AgroKey_EncoderDecoder, Server))

        End If
    End Sub

    '####################################################################################################################
    Private Sub VisualizzaLogoRegione(ByRef Log_Errori As String)

        ' nascondo tutti i loghi delle regioni
        'Dim r As Integer
        'For r = 1 To 20
        '    'Try
        '    Rpt_PrepBIO.Section1.ReportObjects("LogoRegione" & Right("000" & r.ToString, 3)).ObjectFormat.EnableSuppress = True
        'Catch ex As CrystalDecisions.CrystalReports.Engine.InvalidArgumentException
        ' il logo non c'è
        'ex = Nothing
        'End Try
        Try

            For Each ro In Rpt_PrepBIO.Section1.ReportObjects
                If ro.Name.ToString.StartsWith("LogoRegione") Then
                    ro.ObjectFormat.EnableSuppress = True
                End If
            Next

        Catch ex As Exception
            Log_Errori += "- Impossibile nascondere i loghi delle Regioni: " + vbCrLf + ex.Message + vbCrLf + vbCrLf
        End Try
        'Next

        'se nel filtro è stato scelto di visualizzare il logo
        If Qs_FlagRegione = 1 Then
            'visualizzo il logo della regione richiesta
            If Qs_RegioneCod <> String.Empty Then
                Try
                    Rpt_PrepBIO.Section1.ReportObjects("LogoRegione" & Right("000" & Qs_RegioneCod, 3)).ObjectFormat.EnableSuppress = False
                Catch ex As CrystalDecisions.CrystalReports.Engine.InvalidArgumentException
                    ' il logo non c'è
                    Log_Errori += "- Impossibile visualizzare il logo della Regione " + Qs_RegioneCod + " perché assente: " + vbCrLf + ex.Message + vbCrLf + vbCrLf
                End Try
            End If
        End If

    End Sub


    '##############################################################
    'dati dell'intestazione 
    Private Sub Carica_Intestazione(ByRef DS As DS_RegistroPreparazioniBio, _
                                        ByRef Log_Errori As String, _
                                        ByRef Cod_Regione As String)

        Dim DT As DataTable
        Dim i As Integer
        Dim Dr As DS_RegistroPreparazioniBio.DS_RegistroPreparazioniBioRow

        Dim rag_soc, sa_nome, ind_des, frz_des, cap, comune, provincia, sigla_prov, stato, pro_cod_istat, com_cod_istat As String
        rag_soc = ""
        sa_nome = ""
        ind_des = ""
        frz_des = ""
        cap = ""
        comune = ""
        provincia = ""
        sigla_prov = ""
        stato = ""
        pro_cod_istat = ""
        com_cod_istat = ""

        Dim Codice_Operatore, Codice_Fiscale, Legale As String

        '=============================================================
        'Lettura dei dati

        Try

            Dim Indirizzo As New AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Read

            Indirizzo.Indirizzo_from_PivaSaCod3(Qs_Piva, _
                                                Qs_SaCod, _
                                                rag_soc, _
                                                sa_nome, _
                                                ind_des, _
                                                frz_des, _
                                                cap, _
                                                comune, _
                                                provincia, _
                                                sigla_prov, _
                                                stato, _
                                                pro_cod_istat, _
                                                com_cod_istat, _
                                                Cod_Regione, _
                                                 objParametri_Server)

            Dim CodOp As New AgronicaCoreAnagrafeDAL.Centri_Codici_Read

            Codice_Operatore = CodOp.ValCod_from_SaCodIdCod(Qs_Piva, _
                                                                Qs_SaCod, _
                                                                enum_CodiciAnagrafe.CodiceCentro_Attuale, _
                                                                objParametri_Server)

            Dim CF As New AgronicaCoreAnagrafeDAL.Contatti_R

            Codice_Fiscale = CF.CodiceFiscale_from_CodContatto(objParametri_Server.PivaSuperUser, _
                                                                Qs_Piva, _
                                                                 "", _
                                                                objParametri_Server)

            Legale = CF.LegaleRappresentante_from_PivaImpresa(Qs_Piva, _
                                                            "", _
                                                            objParametri_Server)


        Catch ex As Exception
            Log_Errori += "- Lettura dei dati: " + vbCrLf + ex.Message + vbCrLf + vbCrLf
        End Try

        '=============================================================
        'Valorizzazione del dataset

        Try

            '-----------------------------------------------------
            Dr = DS.DS_RegistroPreparazioniBio.NewDS_RegistroPreparazioniBioRow()
            '-----------------------------------------------------


            Dr.Piva = Qs_Piva
            Dr.Rag_Soc = rag_soc
            Dr.Codice_Fiscale = Codice_Fiscale
            Dr.Codice_Operatore = Codice_Operatore
            Dr.Sa_Nome = sa_nome
            Dr.Indirizzo = ind_des + " " + frz_des + " " + comune + " (" + sigla_prov + ")"
            Dr.Legale = Legale

            CType(Rpt_PrepBIO.Section1.ReportObjects("TxtRegolamentoBIO"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Descrizione_Regolamento_Bio_STAMPE


            '-----------------------------------------------------
            DS.DS_RegistroPreparazioniBio.Rows.Add(Dr)
            '-----------------------------------------------------

        Catch ex As Exception
            Log_Errori += "- Valorizzazione del dataset dell'intestazione: " + vbCrLf + ex.Message + vbCrLf + vbCrLf
        End Try


    End Sub


    '##############################################################
    Private Sub Carica_MateriePrime_CicloLavorazione(ByRef DS_MP As Ds_MateriePrime, _
                                                        ByRef DS_Lav As DS_CicloLavorazione, _
                                                        ByRef Log_Errori As String)

        Dim DT As DataTable
        Dim i As Integer
        Dim Dr As Ds_MateriePrime.DS_MateriePrimeRow
        Dim DrLav As DS_CicloLavorazione.DS_CicloLavorazioneRow
        Dim Temp_Prep As Integer = 0
        Dim Prep_Cod As Integer

        '=============================================================
        'Lettura dei dati

        Try

            Dim RegPrep As New AgronicaCoreStampeDAL.RegistriPreparazioni

            If Qs_PreparazioneCod <> 0 Then

                DT = RegPrep.PreparazioniBIO_Ingredienti_LineePreparazioni(Qs_Piva, _
                                                                            Qs_PreparazioneCod, _
                                                                            "", "", _
                                                                            objParametri_Server)

            Else
                If (Qs_LineaCod <> 0) Then

                    DT = RegPrep.PreparazioniBIO_Ingredienti_LineeProduzioni(Qs_Piva, _
                                                                                Qs_LineaCod, _
                                                                                Qs_MatCod, _
                                                                                "", "", _
                                                                                objParametri_Server)

                Else
                    Log_Errori += "- Preparazione_Cod e Linea_Cod non specificati!: " + vbCrLf + vbCrLf
                End If
            End If


        Catch ex As Exception
            Log_Errori += "- Lettura dei dati: " + vbCrLf + ex.Message + vbCrLf + vbCrLf
        End Try

        '=============================================================
        'Valorizzazione del dataset

        Try

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

                For i = 0 To DT.Rows.Count - 1

                    '-----------------------------------------------------
                    Dr = DS_MP.DS_MateriePrime.NewDS_MateriePrimeRow()
                    '-----------------------------------------------------

                    Dr.Elem_cod = DT.Rows(i).Item("Elem_cod")
                    Dr.Pro_cod = DT.Rows(i).Item("Pro_cod")
                    Dr.Mat_Cod = DT.Rows(i).Item("Mat_Cod")
                    Dr.Udm_Cod = DT.Rows(i).Item("Udm_Cod")
                    Dr.Udm_Sim = DT.Rows(i).Item("Udm_Sim")
                    Dr.Udm_Des = DT.Rows(i).Item("Udm_Des")
                    Dr.Descrizione = DT.Rows(i).Item("dettaglio_des")
                    Dr.Regolamento = DT.Rows(i).Item("regolamento")

                    Select Case Dr.Regolamento
                        Case enum_Cod_Regolamento.Regolamento_bio
                            Dr.Qta_Bio = DT.Rows(i).Item("Qta")
                        Case Else
                            Dr.Qta_Conv = DT.Rows(i).Item("Qta")
                    End Select

                    '-----------------------------------------------------
                    DS_MP.DS_MateriePrime.Rows.Add(Dr)
                    '-----------------------------------------------------

                    Prep_Cod = DT.Rows(i).Item("Preparazione_Cod")

                    If Temp_Prep = Prep_Cod Then
                        'stessa preparazione
                    Else
                        'nuova preparazione
                        Temp_Prep = DT.Rows(i).Item("Preparazione_Cod")

                        '-----------------------------------------------------
                        DrLav = DS_Lav.DS_CicloLavorazione.NewDS_CicloLavorazioneRow()
                        '-----------------------------------------------------

                        DrLav.Preparazione_cod = Prep_Cod
                        DrLav.Preparazione_Des = DT.Rows(i).Item("Preparazione_Des")
                        DrLav.Note = DT.Rows(i).Item("Note")
                        DrLav.Descrizione = DrLav.Preparazione_Des + ": " + DrLav.Note

                        '-----------------------------------------------------
                        DS_Lav.DS_CicloLavorazione.Rows.Add(DrLav)
                        '-----------------------------------------------------

                    End If

                Next

            End If


        Catch ex As Exception
            Log_Errori += "- Valorizzazione del dataset materie prime: " + vbCrLf + ex.Message + vbCrLf + vbCrLf
        End Try


    End Sub


    '##############################################################
    Private Sub Carica_Agenda(ByRef DS As DS_Agenda,
                            ByRef Log_Errori As String)

        Dim DT As DataTable
        Dim i As Integer
        Dim Dr As DS_Agenda.DS_AgendaRow

        '=============================================================
        'Lettura dei dati

        Try

            Dim RegPrep As New AgronicaCoreStampeDAL.RegistriPreparazioni

            DT = RegPrep.PreparazioniBIO_Agenda(Qs_Piva,
                                                Qs_LineaCod,
                                                Qs_PreparazioneCod,
                                                0,
                                                Qs_MatCod,
                                                QS_DataInizio,
                                                QS_DataFine,
                                                "", "",
                                                objParametri_Server)


        Catch ex As Exception
            Log_Errori += "- Lettura dei dati: " + vbCrLf + ex.Message + vbCrLf + vbCrLf
        End Try

        '=============================================================
        'Valorizzazione del dataset

        Try

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

                Dim Qta_ProdFinito As Double
                Dim Qta_Confezione As Double
                'Dim Udm_ProdFinito As String
                Dim Udm_Confezione As String
                Dim Tot_ProdFinito As Double = 0
                Dim Tot_Confezione As Double = 0

                For i = 0 To DT.Rows.Count - 1

                    '-----------------------------------------------------
                    Dr = DS._DS_Agenda.NewDS_AgendaRow()
                    '-----------------------------------------------------

                    Dr.Data = DT.Rows(i).Item("Data_Movimento")

                    '14/03/2019: aggiunta stampa dell'ingrediente principale con lotto e litri
                    Dr.Note = DT.Rows(i).Item("Note_Ingredienti")

                    'se il prodotto finito contiene all'interno altri beni = confezione
                    If DT.Rows(i).Item("Qta_Contenitore") > 0 Then
                        Qta_ProdFinito = DT.Rows(i).Item("Qta") * DT.Rows(i).Item("Qta_Contenitore")
                        Dr.Udm_Sim = DT.Rows(i).Item("Udm_Sim")
                        Dr.Udm_Des = DT.Rows(i).Item("Udm_Des")
                    Else
                        Qta_ProdFinito = 0 'metto 0, perchè valorizzo solo le confezioni
                        Dr.Udm_Sim = "" 'DT.Rows(i).Item("Udm_Sim")
                        Dr.Udm_Des = "" 'DT.Rows(i).Item("Udm_Des")
                    End If

                    Dr.Qta = Qta_ProdFinito
                    Dr.Udm_Cod = DT.Rows(i).Item("Udm_Cod")

                    Dr.Confez_Lotto = DT.Rows(i).Item("Lotto")

                    Dr.Confez_Tipo = ""
                    If DT.Rows(i).Item("Qta_Extra") <> 0 Then
                        Dr.Confez_Capacita = DT.Rows(i).Item("Qta_Extra") & " " & DT.Rows(i).Item("Conf_Udm_sim")
                    Else
                        Dr.Confez_Capacita = ""
                    End If

                    'Qta_Confezione = Format(CInt(DT.Rows(i).Item("Qta") / DT.Rows(i).Item("Qta_Extra")), "#,###,##0")
                    Qta_Confezione = CInt(DT.Rows(i).Item("Qta"))
                    Dr.Confez_Qta = Qta_Confezione

                    Dr.Confez_Udm_Cod = DT.Rows(i).Item("Udm_Cod")
                    Udm_Confezione = DT.Rows(i).Item("Udm_Sim")

                    Dr.Confez_Udm_Sim = Udm_Confezione

                    Tot_ProdFinito += Qta_ProdFinito
                    Dr.Tot_ProdFinito = Format(Tot_ProdFinito, "#,###,##0")

                    Tot_Confezione += Qta_Confezione
                    Dr.Tot_Conf = Format(Tot_Confezione, "#,###,##0")

                    '-----------------------------------------------------
                    DS._DS_Agenda.Rows.Add(Dr)
                    'DS.DS_Agenda.Rows.Add(Dr)
                    '-----------------------------------------------------

                Next

            End If


        Catch ex As Exception
            Log_Errori += "- Valorizzazione del dataset delle operazioni di agenda: " + vbCrLf + ex.Message + vbCrLf + vbCrLf
        End Try


    End Sub

End Class
