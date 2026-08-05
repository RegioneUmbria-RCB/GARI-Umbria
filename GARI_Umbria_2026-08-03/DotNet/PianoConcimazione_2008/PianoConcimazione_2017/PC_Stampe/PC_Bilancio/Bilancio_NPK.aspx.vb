Option Strict Off

Imports Agronica.Helpers.GiasBase
Imports AgronicaControlli_2010
Imports AgronicaControlli_2010.UtilityPersonalizzazioniGraficheCliente
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDTOStd.Identity
Imports AgronicaCoreModelsSTD.PersonalizzazioniGraficheCliente
Imports CrystalDecisions.Shared

Public Class Bilancio_NPK
    Inherits System.Web.UI.Page


    '----- Variabili globali nella pagina
    Dim Qs_Key As String
    Dim Qs_AnalisiTestataCod As String
    Dim Qs_Operazione As String
    Dim Qs_Piva As String
    Dim Qs_PCTestataCod As String
    Dim Qs_Anteprima As String

    Dim xChiave As String
    Dim xTipoNodo As enum_TipoNodo

    Dim xPiva As String
    Dim xSa_Cod As Integer
    Dim xCampo_Cod As Integer
    Dim xAppezza As Integer
    Dim xID_Imp As Integer
    Dim xPart_Cod As Integer
    Dim xFabbricato_Cod As Integer
    Dim xCodFiscale As String

    Dim xCodProvincia As String
    Dim xCodComune As String
    Dim xSezione As String
    Dim xFoglio As Integer
    Dim xNumero As Integer
    Dim xSubalterno As String

    Dim xProgetto_Cod As Integer

    Dim xPianoConcimazione_Testata_Cod As Integer

    '----- objParametri
    Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri

    Dim Log_Errori As String = ""
    Dim Nome_Documento As String = "Bilancio_NPK"

    '----- Gestione Report
    Private rptBilancioNPK As RPT_Bilancio_NPK
    Private rptFooterLogo As FooterLogo

    Dim personalizzazioniGraficheCliente As PersonalizzazioniGraficheCliente = Nothing

    Private _LinkGiasBase As String = String.Empty
    Public ReadOnly Property PATH_GIASBASE As String
        Get
            Return _LinkGiasBase
        End Get
    End Property

    '##########################################################################################################
    Private Sub Bilancio_NPK_Init(sender As Object, e As System.EventArgs) Handles Me.Init
        rptBilancioNPK = New RPT_Bilancio_NPK
        rptFooterLogo = New FooterLogo
    End Sub

    '##########################################################################################################
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        '##############################################################
        '#####  Verifico Credenziali di Accesso  ######################
        '##############################################################

        '----- Verifico che l'utente sia autenticato

        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("Messaggi/AccessoNegato.htm")
        End If

        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Super_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Super_Server"))

        GiasBaseHelper.Setta_Link_GiasBase(objParametri_Server, _LinkGiasBase)

        '----- Verifico se l'utente dispone dei permessi per la visualizzazione della pagina

        Dim strDummy As String      'controllo accesso negato.....
        Dim UtenteAbilitato As Boolean

        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        UtenteAbilitato = objPermessi.Controlla_Permessi_Utente(
                            Session("ASG_Utente_Username"),
                            Session("ASG_IdServizio"),
                            TipiEnumerativi.enum_Security_Attivita.SupportoDecisioni_PianoConcimazione,
                            TipiEnumerativi.enum_Security_Operazione.Lettura,
                            Date.Now,
                            "",
                            objParametri_Utenti)

        If UtenteAbilitato = False Then
            Response.Redirect("Messaggi/AccessoNegato.htm")
            'AgroMsgBox("Non si dispongono dei permessi necessari per la gestione dei piani di concimazione", Page)
            'Exit Sub
        End If

        '######################################################################################################################
        '######################################################################################################################

        Session.Timeout = 180

        '##############################################################
        '#####  Recupero le variabili                        ##########
        '##############################################################

        If Not IsNothing(Request.QueryString("t")) Then

            Qs_AnalisiTestataCod = Stringa_Decodifica(Request.QueryString("t").ToString,
                                                      AgroKey_EncoderDecoder,
                                                      Server)
        Else
            Qs_AnalisiTestataCod = ""
        End If

        If Not IsNothing(Request.QueryString("n")) Then
            'in caso di Inserimento è il nodo padre, in caso di modifica è il nodo "piano concimazione"
            Qs_Key = Stringa_Decodifica(Request.QueryString("n").ToString,
                                                AgroKey_EncoderDecoder,
                                                Server)
        Else
            Qs_Key = ""
        End If

        Qs_Operazione = Stringa_Decodifica(Request.QueryString("o").ToString,
                            AgroKey_EncoderDecoder,
                            Server)

        Qs_Piva = Stringa_Decodifica(Request.QueryString("p").ToString,
                                    AgroKey_EncoderDecoder,
                                    Server)

        Session("PartitaIVA") = Qs_Piva

        Qs_PCTestataCod = Stringa_Decodifica(Request.QueryString("q").ToString,
                                            AgroKey_EncoderDecoder,
                                            Server)

        If Not IsNothing(Request.QueryString("anteprima")) Then

            Qs_Anteprima = Stringa_Decodifica(Request.QueryString("anteprima").ToString,
                                                      AgroKey_EncoderDecoder,
                                                      Server)
        Else
            Qs_Anteprima = "0"
        End If

        personalizzazioniGraficheCliente = UtilityPersonalizzazioniGraficheCliente.LeggiPersonalizzazioniGraficheCliente(objParametri_Server, objParametri_Super_Server)

        Dim Dt As New DataTable
        Dim Rag_Soc As String = ""
        Dim Sa_Nome As String = ""
        Dim strErr As String = ""

        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        ' Dim CrystalReportViewer1 As CrystalDecisions.Web.CrystalReportViewer


        If Not Me.IsPostBack Then

            'CrystalReportViewer1 = New CrystalDecisions.Web.CrystalReportViewer

            'CrystalReportViewer1.Style.Add("LEFT", "-275px")
            'CrystalReportViewer1.Style.Add("TOP", "0px")
            'CrystalReportViewer1.Style.Add("POSITION", "Absolute")

            'Me.FindControl("Form1").Controls.Add(CrystalReportViewer1)

            Dim Sup_Tot As Decimal = 0
            Dim Mas As Decimal = -1
            Dim MasSalvato As Boolean = False

            Try
                Dati_Piano(Qs_Piva, xSa_Cod, xAppezza, xID_Imp, Qs_PCTestataCod, "", Sup_Tot, 0, Mas, MasSalvato, objParametri_Server, rptBilancioNPK)
            Catch ex As Exception
                Log_Errori += "- Dati_Piano: " + vbCrLf + ex.Message + vbCrLf
            End Try

            Dim DsNecessita As New DS_Fattori
            Dim DsDisponibilita As New DS_Fattori
            Dim DS_LogoFooter As New DS_LogoFooter

            Try
                Carica_Dati(DsDisponibilita, DsNecessita, Qs_PCTestataCod, Qs_Piva, Sup_Tot, Mas, MasSalvato, objParametri_Server, rptBilancioNPK)
            Catch ex As Exception
                Log_Errori += "- Carica_Dati: " + vbCrLf + ex.Message + vbCrLf
            End Try

            Try
                Dim drLogo = DS_LogoFooter.DT_LogoFooter.NewDT_LogoFooterRow
                Dim logo As ConfigurazioneLoghiStampe = TrovaLogoFooterStampe(personalizzazioniGraficheCliente, Log_Errori, objParametri_Server)
                If logo.LogoStampe IsNot Nothing Then
                    drLogo.Logo = logo.LogoStampe
                    drLogo.TestoPostLogo = logo.TestoPostLogo
                    drLogo.TestoPreLogo = logo.TestoPreLogo
                End If
                DS_LogoFooter.DT_LogoFooter.Rows.Add(drLogo)
            Catch ex As Exception
                Log_Errori += "- Carica Loghi: " + vbCrLf + ex.Message + vbCrLf
            End Try

            Try
                'rptBilancioNPK.SetDataSource(DS_LogoFooter)
                rptBilancioNPK.OpenSubreport("RPT_Necessita.rpt").SetDataSource(DsNecessita)
                rptBilancioNPK.OpenSubreport("RPT_Disponibilita.rpt").SetDataSource(DsDisponibilita)
                rptBilancioNPK.OpenSubreport("FooterLogo.rpt").SetDataSource(DS_LogoFooter)
            Catch ex As Exception
                Log_Errori += "- sottoreport SetDataSource: " + vbCrLf + ex.Message + vbCrLf
            End Try

            'In caso le personalizzazioni siano attive, nascondo il logo e ragione sociale Agronica.
            'If personalizzazioniGraficheCliente IsNot Nothing Then
            '    rptBilancioNPK.Section5.ReportObjects("Text9").ObjectFormat.EnableSuppress = True
            '    rptBilancioNPK.Section5.ReportObjects("Picture3").ObjectFormat.EnableSuppress = True
            '    rptBilancioNPK.Section5.ReportObjects("Text1").ObjectFormat.EnableSuppress = True
            'End If

            Try
                Dim MostraDataFirma As Boolean = False

                Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

                Dim DtImpostazioni = objUtenti.Leggi(0, 1,
                                                     enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                     "", "", objParametri_Utenti)

                If Not IsNothing(DtImpostazioni) Then

                    If DtImpostazioni.Select("Impostazione_Cod = " & enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_PIANOCONCIMAZIONE_CAMPAGNA_DATA_FIRMA & "And Impostazione_Valore_1 = '1'").Length > 0 Then
                        MostraDataFirma = True
                    End If

                End If


                'Nascondo la Sezione con la Data e la Firma
                If Not MostraDataFirma Then
                    rptBilancioNPK.ReportFooterSection1.SectionFormat.EnableSuppress = True
                End If

            Catch ex As Exception
                Log_Errori += "- Lettura Utenti_Impostazioni e Nascondi/Mostra Sezioni: " + vbCrLf + ex.Message + vbCrLf
            End Try


            'creo la variabile per valorizzarla nel SalvaPdf che poi mi server per la gestione_allegati
            Dim NomeFile As String = CreaNomeFile(Qs_Piva, Qs_PCTestataCod, objParametri_Server)
            ' il pdf viene salvato sempre

            Dim strFile As String = ""

            Try
                ' salvo il report in formato PDF
                Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
                objGestFile.SalvaReportPdf(rptBilancioNPK,
                                           enum_CategorieDocumenti.PianoConcimazione,
                                           "Piano Concimazione",
                                           NomeFile,
                                           objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)

                'strFile = SalvaPdf(objParametri_Server, rptBilancioNPK, NomeFile)
            Catch ex As Exception
                Log_Errori += "- SalvaPdf: " + vbCrLf + ex.Message + vbCrLf
            End Try


            Try

                If strFile <> "" Then

                    Dim objAllegati As New AgronicaCoreScadenziario_BIZ.Allegati
                    Dim AllegatiDocumentiCod As Integer

                    Dim anno As Integer
                    If Not IsNothing(Session("Anno")) AndAlso IsNumeric(Session("Anno")) Then
                        anno = CInt(Session("Anno"))
                    Else
                        anno = Now.Year
                    End If

                    AllegatiDocumentiCod = objAllegati.SalvaAllegato(Qs_Piva.ToString,
                                                             enum_CategorieDocumenti.PianoConcimazione,
                                                             "Piano Concimazione",
                                                             NomeFile,
                                                             Session("Sottocartella"),
                                                             Qs_PCTestataCod, Qs_Piva.ToString, "", "",
                                                             CDate("01/01/" & anno.ToString),
                                                             CDate("31/12/" & anno.ToString),
                                                             objParametri_Server)

                    Dim objPC_dettagli_W As New AgronicaCorePianoConcimazioneDAL.PianoConcimazione_Dettagli_W
                    'INSERISCO IL CODICE ALLEGATO NELLA TABELLA Piano_Concimazione_Dettagli del PC salvato
                    If Not IsNothing(AllegatiDocumentiCod) AndAlso AllegatiDocumentiCod <> 0 Then
                        If Not objPC_dettagli_W.UpdateCodAllegato(Qs_PCTestataCod, 0, Qs_Piva.ToString, AllegatiDocumentiCod, objParametri_Server) Then
                            Throw New Exception("Non sono riuscito ad associare l'allegato al corrente Piano di Concimanzione. PC_cod =" & Qs_PCTestataCod.ToString)
                        End If
                    End If

                    objAllegati = Nothing

                End If

            Catch ex As Exception
                Log_Errori += "- gestione allegati: " + vbCrLf + ex.Message + vbCrLf
            End Try

            Dim reportTemporano As String = CrystalHelper.getFileReportTemporaneo()
            Try
                rptBilancioNPK.SaveAs(reportTemporano, True)
            Catch ex As Exception

            End Try

            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------
            If Log_Errori <> "" Then

                Log_Errori = Nome_Documento + ", Partita Iva = " + CStr(Qs_Piva) +
                 ", Qs_PCTestataCod = " + CStr(Qs_PCTestataCod) + vbCrLf + vbCrLf + Log_Errori

                Dim Nome_File As String = "Log_Errori_" + Nome_Documento

                Dim objLog As New AgronicaCoreDataProvider.LogProvider
                objLog.Gestione_LogErrori(objParametri_Server,
                                      "PianoConcimazione",
                                       Nome_File & ".txt",
                                       Session("ASG_Utente_Username"),
                                        "Bilancio_NPK",
                                         Log_Errori)

            End If

            rptBilancioNPK.Close()
            rptBilancioNPK.Dispose()
            rptBilancioNPK = Nothing

            GC.Collect()

            If Qs_Anteprima = "1" Then
                'Session("Report") = rptBilancioNPK
                'Response.Redirect("..\VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server))


                Response.Redirect("..\VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server) +
                              "&tmpReportPath=" + Stringa_Codifica(reportTemporano, AgroKey_EncoderDecoder, Server) &
                              "&NomePdf=" & Stringa_Codifica(NomeFile, AgroKey_EncoderDecoder, Server))

            Else

                'Session("Report") = rptBilancioNPK

                Dim UrlStampa As String
                Dim UrlFiltro As String

                UrlStampa = "../VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("0", AgroKey_EncoderDecoder, Server) +
                              "&tmpReportPath=" + Stringa_Codifica(reportTemporano, AgroKey_EncoderDecoder, Server) &
                              "&NomePdf=" & Stringa_Codifica(NomeFile, AgroKey_EncoderDecoder, Server)

                'UrlStampa = "../VisualizzatoreReport.aspx" &
                '            "?anteprima=" + Stringa_Codifica("0", AgroKey_EncoderDecoder, Server) &
                '            "&pdf=" + Stringa_Codifica(strFile, AgroKey_EncoderDecoder, Server) & ""

                UrlFiltro = "../Filtro_StampaScadenza.aspx" &
                           "?a=" + Stringa_Codifica(CStr(TipiEnumerativi.enum_Security_Attivita.SupportoDecisioni_PianoConcimazione), AgroKey_EncoderDecoder, Server) +
                           "&o=" + Stringa_Codifica(CStr(TipiEnumerativi.enum_Security_Operazione.Modifica), AgroKey_EncoderDecoder, Server) &
                           "&piva=" + Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) &
                           "&pc=" + Stringa_Codifica(CStr(Qs_PCTestataCod), AgroKey_EncoderDecoder, Server) &
                           "&scadenziario=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server) &
                           "&pdf=" + Stringa_Codifica(strFile, AgroKey_EncoderDecoder, Server)

                Dim strOpen As String = "<script language='javascript'>" & vbNewLine &
                                        "window.open('" & UrlStampa & "');" & vbNewLine &
                                        "location.href='" & UrlFiltro & "';" & vbNewLine &
                                        "</script>"

                Me.Page.FindControl("Form1").Controls.Add(New LiteralControl(strOpen))

                Exit Sub

            End If

            '    CrystalReportViewer1.ReportSource = rptBilancioNPK

            '    CrystalReportViewer1.DataBind()

            'Else 'ad ogni post back devo impostare la fonte dati x il visualizzatore..

            '    If Not IsNothing(Session("DS")) Then

            '        'imposto la sorgente dati x il report...
            '        rptBilancioNPK.SetDataSource(CType(Session("DS")(0), DataSet))

            '        'faccio il databind col visualizzatore dei reports...
            '        CrystalReportViewer1.ReportSource = rptBilancioNPK

            '        CrystalReportViewer1.DataBind()

            '    End If

        End If


        ''==================================================================

        '' Dichiara le variabili e restituisce le opzioni di esportazione.
        'Dim exportOpts As New ExportOptions
        'Dim diskOpts As New DiskFileDestinationOptions

        'exportOpts = rptBilancioNPK.ExportOptions

        '' Imposta il formato di esportazione.
        'exportOpts.ExportFormatType = ExportFormatType.PortableDocFormat
        'exportOpts.ExportDestinationType = ExportDestinationType.DiskFile



        '' Imposta le opzioni relative al file del disco.
        'Dim strPath As String
        'Dim PathFileTemporanei As String


        'If ConfigurationSettings.AppSettings("CartellaFileTemporanei").ToString = "" Then
        '    PathFileTemporanei = Server.MapPath("../../File_Temporanei")
        'Else
        '    PathFileTemporanei = ConfigurationSettings.AppSettings("CartellaFileTemporanei").ToString
        'End If

        'strPath = PathFileTemporanei & "\Allegato_5_" & Session.SessionID.ToString & ".pdf"
        'diskOpts.DiskFileName = strPath
        'exportOpts.DestinationOptions = diskOpts

        ''ApriScadenziario()
        ''Exit Sub

        '' Esportazione del report.
        'rptBilancioNPK.Export()

        '' Con il seguente codice il file pdf viene scritto 
        ''  nel browser del client.
        ''Response.ClearContent()
        ''Response.ClearHeaders()
        ''Response.ContentType = "application/pdf"
        ''Response.WriteFile(strPath)
        ''Response.Flush()
        ''Response.Close()

        '' il file esportato viene eliminato dal disco
        'System.IO.File.Delete(strPath)

    End Sub


    '################################################################################
    Public Function CreaNomeFile(ByVal Piva As String, ByVal PC_TestataCod As Integer, _
                                 ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String


        Dim strCentro As String = Session("strCentro")
        Dim strSpecie As String = Session("strSpecie")

        'strCentro = strCentro.Replace(" ", "")
        'strCentro = strCentro.Replace("'", "")
        'strCentro = strCentro.Replace("""", "")
        'strSpecie = strSpecie.Replace(" ", "")
        'strSpecie = strSpecie.Replace("'", "")

        strCentro = AgronicaCoreUtility.Stringhe.EliminaCaratteriSpecialiFile(strCentro)
        strSpecie = AgronicaCoreUtility.Stringhe.EliminaCaratteriSpecialiFile(strSpecie)

        Dim CodSocio As String = Trim(Session("Socio"))

        CodSocio = CodSocio.Replace("/", "_")
        CodSocio = CodSocio.Replace("\", "_")
        CodSocio = CodSocio.Replace("|", "_")

        Dim CentroConferimento As String

        Dim objAnagrafe As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
        ' CodSocio = objAnagrafe.Leggi_Codice_from_Imprese_Codici(Piva, enum_CodiciAnagrafe.Codice_Socio, objParametri)

        CentroConferimento = objAnagrafe.Leggi_Codice_from_Imprese_Codici(Piva, enum_CodiciAnagrafe.V01CON00_VCCOIS, objParametri)
        objAnagrafe = Nothing

        Dim NomeFile As String = "PC" &
                                 IIf(CentroConferimento <> "", "_" & CentroConferimento, "") &
                                 "_" & strCentro.Substring(0, Math.Min(15, strCentro.Length - 1)) &
                                 IIf(CodSocio <> "", "_" & CodSocio, "") &
                                 "_" & strSpecie.Substring(0, Math.Min(15, strSpecie.Length - 1)) &
                                 "_" & PC_TestataCod & ".pdf"

        Return NomeFile


    End Function


    '################################################################################
    Public Function SalvaPdf(ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                             ByRef rpt As RPT_Bilancio_NPK, _
                             Optional ByRef NomeFile As String = "") As String



        ' Dichiara le variabili e restituisce le opzioni di esportazione.
        Dim exportOpts As New ExportOptions
        Dim diskOpts As New DiskFileDestinationOptions
        Dim strPath As String = ""
        Dim PathCartella As String

        Try

            exportOpts = rpt.ExportOptions

            ' Imposta il formato di esportazione.
            exportOpts.ExportFormatType = ExportFormatType.PortableDocFormat
            exportOpts.ExportDestinationType = ExportDestinationType.DiskFile

            ' leggo la sottocartella da CategorieDocumenti
            Dim Sottocartella As String
            Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
            Sottocartella = objCatDoc.Sottocartella(enum_CategorieDocumenti.PianoConcimazione, "", "", objParametri)
            Session("Sottocartella") = Sottocartella    ' mi serve nel piano concimazione massivo
            objCatDoc = Nothing

            Dim objAgroWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig

            If Sottocartella <> "" Then
                PathCartella = objAgroWeb.GestioneAllegati_Repository & Sottocartella
            Else
                If objAgroWeb.GestioneAllegati_Repository.EndsWith("\") Then
                    PathCartella = objAgroWeb.GestioneAllegati_Repository.Substring(0, objAgroWeb.GestioneAllegati_Repository.Length - 1)
                Else
                    PathCartella = objAgroWeb.GestioneAllegati_Repository
                End If

            End If


            ' se il path non esiste, creo tutte le cartelle e sottocartelle
            If Not System.IO.Directory.Exists(PathCartella) Then
                System.IO.Directory.CreateDirectory(PathCartella)
            End If

            If NomeFile = "" Then

                NomeFile = CreaNomeFile(Qs_Piva, Qs_PCTestataCod, objParametri)
                'NomeFile = "\PC_" & Qs_PCTestataCod & "_" & Session.SessionID.ToString & ".pdf"
            End If

            strPath = PathCartella & "\" & NomeFile

            diskOpts.DiskFileName = strPath
            exportOpts.DestinationOptions = diskOpts

            ' Esportazione del report.
            rpt.Export()

        Catch ex As Exception
            Log_Errori += "- public_SalvaPdf: " + vbCrLf + ex.Message + vbCrLf
            Return ""
        End Try

        Return strPath

    End Function


    ''###############################################################################
    'Private Sub TipoNodo_Albero(ByVal xChiave As String, ByRef xTipoNodo As Integer)

    '    If xChiave = "" Then
    '        Exit Sub
    '    End If

    '    'Identifico il tipo di nodo 
    '    Call ChiaveAlbero_Decodifica_TipoNodo(xChiave, xTipoNodo)

    '    'Chiamo la decodifica opportuna a seconda del tipo di nodo
    '    Select Case xTipoNodo

    '        Case TipiEnumerativi.enum_TipoNodo.DistintaDiProduzione

    '            'Data la chiave ricavo gli elementi che la compongono
    '            Call ChiaveAlbero_Decodifica_DistintaProduzione( _
    '                                                xChiave, _
    '                                                xTipoNodo, _
    '                                                xPiva, _
    '                                                xSa_Cod, _
    '                                                xCampo_Cod, _
    '                                                xAppezza, _
    '                                                xID_Imp, _
    '                                                xProgetto_Cod)


    '        Case TipiEnumerativi.enum_TipoNodo.Impresa, _
    '             TipiEnumerativi.enum_TipoNodo.Centro, _
    '             TipiEnumerativi.enum_TipoNodo.Campo, _
    '             TipiEnumerativi.enum_TipoNodo.Appezzamento, _
    '             TipiEnumerativi.enum_TipoNodo.ImpiantoArborea, _
    '             TipiEnumerativi.enum_TipoNodo.ImpiantoErbacea, _
    '             TipiEnumerativi.enum_TipoNodo.ImpiantoOrticola, _
    '             TipiEnumerativi.enum_TipoNodo.ImpiantoNudo, _
    '             TipiEnumerativi.enum_TipoNodo.CatastoAziendale


    '            'Data la chiave ricavo gli elementi che la compongono
    '            Call ChiaveAlbero_Decodifica_ImpiantiVegetali( _
    '                                                xChiave, _
    '                                                xTipoNodo, _
    '                                                xPiva, _
    '                                                xSa_Cod, _
    '                                                xCampo_Cod, _
    '                                                xAppezza, _
    '                                                xID_Imp, _
    '                                                xCodFiscale, _
    '                                                xFabbricato_Cod)
    '            '-----------------------------------------


    '        Case TipiEnumerativi.enum_TipoNodo.Utente


    '            Call AgroMsgBox("Dati associati al nodo utente non disponibili!!", Page)
    '            Exit Sub
    '            '
    '            '
    '            '-----------------------------------------


    '        Case TipiEnumerativi.enum_TipoNodo.Persona

    '            Call AgroMsgBox("Dati associati al nodo persona non disponibili!!", Page)
    '            Exit Sub

    '            '-----------------------------------------


    '        Case TipiEnumerativi.enum_TipoNodo.f_Abitazione, _
    '             TipiEnumerativi.enum_TipoNodo.f_CellaFrigorifera, _
    '             TipiEnumerativi.enum_TipoNodo.f_ImpiantoLavorazione, _
    '             TipiEnumerativi.enum_TipoNodo.f_Magazzino, _
    '             TipiEnumerativi.enum_TipoNodo.f_Silos, _
    '             TipiEnumerativi.enum_TipoNodo.f_Stalla, _
    '             TipiEnumerativi.enum_TipoNodo.f_Fienile, _
    '             TipiEnumerativi.enum_TipoNodo.Fabbricato_Generico

    '            Call AgroMsgBox("I dati associati a questo nodo non sono disponibili!!", Page)
    '            Exit Sub

    '            '-----------------------------------------


    '        Case TipiEnumerativi.enum_TipoNodo.Particella


    '            Call AgroMsgBox("I dati associati a questo nodo non sono disponibili!!", Page)
    '            Exit Sub


    '        Case TipiEnumerativi.enum_TipoNodo.PianoConcimazione_Testata

    '            Call ChiaveAlbero_Decodifica_PianoConcimazione( _
    '                                                xChiave, _
    '                                                xTipoNodo, _
    '                                                xPiva, _
    '                                                xSa_Cod, _
    '                                                xCampo_Cod, _
    '                                                xAppezza, _
    '                                                xID_Imp, _
    '                                                xCodProvincia, _
    '                                                xCodComune, _
    '                                                xSezione, _
    '                                                xFoglio, _
    '                                                xNumero, _
    '                                                xSubalterno, _
    '                                                xPart_Cod, _
    '                                                xFabbricato_Cod, _
    '                                                xPianoConcimazione_Testata_Cod)

    '            If xCodProvincia = "0" Then
    '                xCodProvincia = Nothing
    '            End If
    '            If xCodComune = "0" Then
    '                xCodComune = Nothing
    '            End If
    '            If xSezione = "0" Then
    '                xSezione = Nothing
    '            End If
    '            If xSubalterno = "0" Then
    '                xSubalterno = Nothing
    '            End If

    '    End Select

    'End Sub



    '############################################################################################################
    Public Sub Dati_Piano(ByVal Piva As String, ByVal SaCod As Integer, ByVal Appezza As Integer, ByVal IdReg As Integer,
                            ByVal PCTestataCod As Integer, ByRef Sa_Nome As String, ByRef Sup_Tot As Double,
                            ByRef regolamento As AgronicaCoreDataProvider.TipiEnumerativi.enum_PUARegolamenti,
                            ByRef Mas As Decimal, ByRef MasSalvato As Boolean,
                            ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            ByRef rpt As RPT_Bilancio_NPK)

        Dim Dt As New DataTable
        Dim Rag_Soc As String = ""
        Dim strErr As String = ""

        Dim N As Decimal = 0
        Dim P As Decimal = 0
        Dim K As Decimal = 0
        Dim Veg_Cod As Integer = 0
        Dim Grfi_Cod As Integer = 0
        Dim FaseCiclo As Integer = 0


        '---------------------------------------------------------------------
        Dim CodSocio As String
        Dim objCodiciImpresa As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
        CodSocio = objCodiciImpresa.Leggi_Codice_from_Imprese_Codici(Piva, enum_CodiciAnagrafe.Codice_Socio, objParametri) & "   "
        Session("Socio") = CodSocio

        Dim DtPC_Testata As DataTable
        Dim objPC_Testata As New AgronicaCorePianoConcimazioneDAL.PianoConcimazione_Testata_R
        Dim DtPC_Dettagli As DataTable
        Dim objPC_Dettagli As New AgronicaCorePianoConcimazioneDAL.PianoConcimazione_Dettagli_R

        '--- PC_TESTATA
        DtPC_Testata = objPC_Testata.Leggi_default(PCTestataCod,
                                            0, 0, enumSelezioneVariabile.Selezione_TabellaCompleta,
                                            "", "", objParametri)

        If Not IsNothing(DtPC_Testata) AndAlso DtPC_Testata.Rows.Count > 0 Then

            CType(rpt.Section1.ReportObjects("TextDescrizione"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = DtPC_Testata.Rows(0).Item("PC_Testata_Des")

            Dim ValiditaInizio As String
            Dim ValiditaFine As String
            ValiditaInizio = DtPC_Testata.Rows(0).Item("Validita_Inizio")
            ValiditaFine = DtPC_Testata.Rows(0).Item("Validita_Fine")

            If ValiditaInizio = "01/01/1900" Then
                ValiditaInizio = ""
            End If
            If ValiditaFine = "31/12/2100" Then
                ValiditaFine = ""
            End If

            If ValiditaInizio <> "" Or ValiditaFine <> "" Then
                CType(rpt.Section1.ReportObjects("TextValidita"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = ValiditaInizio &
                                                                                                                                IIf(ValiditaInizio <> "" And ValiditaFine <> "", " - ", "") _
                                                                                                                                & ValiditaFine
            Else
                CType(rpt.Section1.ReportObjects("TextValidita"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = ""
            End If

            regolamento = DtPC_Testata.Rows(0).Item("Regolamento_Cod")

            CType(rpt.ReportHeaderSection7.ReportObjects("TextDichiarazioneNonUtilizzo"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = ""
            If Not IsDBNull(DtPC_Testata.Rows(0).Item("Flag_NonUtilizzo_Fertilizzanti")) AndAlso CInt(DtPC_Testata.Rows(0).Item("Flag_NonUtilizzo_Fertilizzanti")) > 0 Then
                CType(rpt.ReportHeaderSection7.ReportObjects("TextDichiarazioneNonUtilizzo"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Dichiarazione di Non Utilizzo Fertilizzanti"
            End If

            CType(rpt.ReportHeaderSection7.ReportObjects("TextNota"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = ""
            If Not IsDBNull(DtPC_Testata.Rows(0).Item("Note")) AndAlso CStr(DtPC_Testata.Rows(0).Item("Note")) <> "" Then
                CType(rpt.ReportHeaderSection7.ReportObjects("TextNota"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "NOTA :" & CStr(DtPC_Testata.Rows(0).Item("Note"))
            End If

        End If

        DtPC_Testata = Nothing


        '--- PC_DETTAGLI
        DtPC_Dettagli = objPC_Dettagli.Leggi_default(PCTestataCod,
                                                     0, Piva,
                                                     enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                     "", "",
                                                     objParametri)


        If Not IsNothing(DtPC_Dettagli) AndAlso DtPC_Dettagli.Rows.Count > 0 Then

            '---------------------------------------------------------------------
            'Ricavo la Ragione Sociale e Il Nome del Centro
            Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read

            Dt = objCentri.Anagrafica_Centri_Leggi(Piva,
                                               DtPC_Dettagli.Rows(0).Item("PC_Dettagli_SaCod"),
                                              strErr,
                                              enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                              "", "",
                                              objParametri)

            If Dt.Rows.Count > 0 Then

                Rag_Soc = CodSocio & " " & Dt.Rows(0).Item("Rag_Soc")
                CType(rpt.Section1.ReportObjects("TextRagSoc"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Impresa : " & Rag_Soc

                If Dt.Rows.Count = 1 Then
                    Sa_Nome = Dt.Rows(0).Item("Sa_Nome")
                    CType(rpt.Section1.ReportObjects("TextCentro"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Centro : " & Sa_Nome
                Else
                    CType(rpt.Section1.ReportObjects("TextCentro"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = ""
                End If

                Session("strCentro") = Dt.Rows(0).Item("Rag_Soc") & Sa_Nome

            End If

            'TxtResa.Text = RsPC_Dettagli("PC_Dettagli_Resa").Value

            CType(rpt.Section1.ReportObjects("TextZVN"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = IIf(DtPC_Dettagli.Rows(0).Item("PC_Dettagli_AreaVulnerabile") = 1, "ZVN", "")

            CType(rpt.Section1.ReportObjects("TextAnno"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Anno " & DtPC_Dettagli.Rows(0).Item("PC_Dettagli_Anno")
            Session("Anno") = DtPC_Dettagli.Rows(0).Item("PC_Dettagli_Anno")   '   ' mi serve nel piano concimazione massivo
            CType(rpt.Section1.ReportObjects("TextAreaOmogenea"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = IIf(DtPC_Dettagli.Rows(0).Item("PC_Dettagli_AreaOmogenea") <> "", "Area Omogenea " & DtPC_Dettagli.Rows(0).Item("PC_Dettagli_AreaOmogenea"), "")

            Dim Analisi_Cod As Integer = 0
            Dim Analisi_Des As String = ""
            If Not IsDBNull(DtPC_Dettagli.Rows(0).Item("PC_Dettagli_Analisi_Testata_Cod")) Then
                Analisi_Cod = DtPC_Dettagli.Rows(0).Item("PC_Dettagli_Analisi_Testata_Cod")
            End If
            If Analisi_Cod <> 0 Then
                Dim ObjAnalisiR As New AgronicaCoreAnagrafeDAL.Analisi_Testata_R
                Dim DtAnalisi As New DataTable
                DtAnalisi = ObjAnalisiR.LeggiConCertificati(Piva, DtPC_Dettagli.Rows(0).Item("PC_Dettagli_SaCod"),
                                                            Analisi_Cod, 0, "", "", objParametri)
                If Not DtAnalisi Is Nothing AndAlso DtAnalisi.Rows.Count > 0 Then
                    Analisi_Des = " - Analisi "
                    If Not IsDBNull(DtAnalisi.Rows(0).Item("analisi_testata_des")) AndAlso DtAnalisi.Rows(0).Item("analisi_testata_des") <> "" Then
                        Analisi_Des &= DtAnalisi.Rows(0).Item("analisi_testata_des")
                    End If
                    If Not IsDBNull(DtAnalisi.Rows(0).Item("analisi_testata_data_inizio")) AndAlso IsDate(DtAnalisi.Rows(0).Item("analisi_testata_data_inizio")) AndAlso CDate(DtAnalisi.Rows(0).Item("analisi_testata_data_inizio")) <> AGRODATAINIZIO Then
                        Analisi_Des &= " del " & CDate(DtAnalisi.Rows(0).Item("analisi_testata_data_inizio")).ToShortDateString
                    End If
                    If Not IsDBNull(DtAnalisi.Rows(0).Item("Analisi_Certificato_Des")) AndAlso DtAnalisi.Rows(0).Item("Analisi_Certificato_Des") <> "" Then
                        Analisi_Des &= " - N° Certificato " & DtAnalisi.Rows(0).Item("Analisi_Certificato_Des")
                    End If
                    If Not IsDBNull(DtAnalisi.Rows(0).Item("Analisi_Testata_Note1")) AndAlso DtAnalisi.Rows(0).Item("Analisi_Testata_Note1") <> "" Then
                        Analisi_Des &= " - " & DtAnalisi.Rows(0).Item("Analisi_Testata_Note1")
                    End If
                End If
            End If
            CType(rpt.ReportHeaderSection8.ReportObjects("TextAnalisi"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Analisi_Des

            CType(rpt.Section1.ReportObjects("TextSabbia"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Sabbia: " & DtPC_Dettagli.Rows(0).Item("PC_Dettagli_Sabbia") & " %"
            CType(rpt.Section1.ReportObjects("TextLimo"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Limo: " & DtPC_Dettagli.Rows(0).Item("PC_Dettagli_Limo") & " %"
            CType(rpt.Section1.ReportObjects("TextArgilla"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Argilla: " & DtPC_Dettagli.Rows(0).Item("PC_Dettagli_Argilla") & " %"
            CType(rpt.Section1.ReportObjects("TextPH"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "pH: " & DtPC_Dettagli.Rows(0).Item("PC_Dettagli_Ph")
            CType(rpt.Section1.ReportObjects("TextCalcTot"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Calc. Tot: " & DtPC_Dettagli.Rows(0).Item("PC_Dettagli_Caco3") & " %"
            CType(rpt.Section1.ReportObjects("TextCalcAtt"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Calc. Att.: " & DtPC_Dettagli.Rows(0).Item("PC_Dettagli_Caco3_Attivo") & " %"
            CType(rpt.Section1.ReportObjects("TextSO"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "S.O.: " & DtPC_Dettagli.Rows(0).Item("PC_Dettagli_So") & " %"
            CType(rpt.Section1.ReportObjects("TextN"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "N: " & DtPC_Dettagli.Rows(0).Item("PC_Dettagli_ntot") & " g/kg"

            Dim PC_Dettagli_Flag_P As Integer = 0
            Dim PC_Dettagli_Flag_K As Integer = 0
            If Not IsDBNull(DtPC_Dettagli.Rows(0).Item("PC_Dettagli_Flag_P")) Then
                PC_Dettagli_Flag_P = DtPC_Dettagli.Rows(0).Item("PC_Dettagli_Flag_P")
            End If
            If Not IsDBNull(DtPC_Dettagli.Rows(0).Item("PC_Dettagli_Flag_K")) Then
                PC_Dettagli_Flag_K = DtPC_Dettagli.Rows(0).Item("PC_Dettagli_Flag_K")
            End If
            If PC_Dettagli_Flag_P = 0 Then
                CType(rpt.Section1.ReportObjects("TextP2O5"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "P2O5: " & DtPC_Dettagli.Rows(0).Item("PC_Dettagli_p2o5") & " ppm"
            Else
                CType(rpt.Section1.ReportObjects("TextP2O5"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "P: " & DtPC_Dettagli.Rows(0).Item("PC_Dettagli_p") & " ppm"
            End If
            If PC_Dettagli_Flag_K = 0 Then
                CType(rpt.Section1.ReportObjects("TextK2O"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "K2O: " & DtPC_Dettagli.Rows(0).Item("PC_Dettagli_k2o") & " ppm"
            Else
                CType(rpt.Section1.ReportObjects("TextK2O"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "K: " & DtPC_Dettagli.Rows(0).Item("PC_Dettagli_k") & " ppm"
            End If
            CType(rpt.Section1.ReportObjects("TextCN"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "C/N: " & DtPC_Dettagli.Rows(0).Item("PC_Dettagli_CN")
            CType(rpt.Section1.ReportObjects("TextMg"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "MgO: " & DtPC_Dettagli.Rows(0).Item("PC_Dettagli_Mg") & " ppm"
            CType(rpt.Section1.ReportObjects("TextCsc"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "C.S.C.: " & DtPC_Dettagli.Rows(0).Item("PC_Dettagli_CSC") & " meq/100 g"

            'salvati modalita nuova nel dettaglio del piano
            If Not IsDBNull(DtPC_Dettagli.Rows(0).Item("PC_Dettagli_ColturaPrincipale_Veg_Cod")) Then
                Veg_Cod = DtPC_Dettagli.Rows(0).Item("PC_Dettagli_ColturaPrincipale_Veg_Cod")
                If Veg_Cod <> 0 Then
                    Dim objSp As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
                    Session("strSpecie") = objSp.VegDes_from_VegCod(Veg_Cod, objParametri)
                End If
            End If

            If Not IsDBNull(DtPC_Dettagli.Rows(0).Item("PC_Dettagli_Finalita_GRFI_COD")) Then
                Grfi_Cod = DtPC_Dettagli.Rows(0).Item("PC_Dettagli_Finalita_GRFI_COD")
            End If

            If Not IsDBNull(DtPC_Dettagli.Rows(0).Item("PC_Dettagli_FaseCicloColturale_id_fase")) Then
                FaseCiclo = DtPC_Dettagli.Rows(0).Item("PC_Dettagli_FaseCicloColturale_id_fase")
            End If


            If Not IsDBNull(DtPC_Dettagli.Rows(0).Item("N_Ammesso")) Then
                N = DtPC_Dettagli.Rows(0).Item("N_Ammesso")
            End If
            If Not IsDBNull(DtPC_Dettagli.Rows(0).Item("P_Ammesso")) Then
                P = DtPC_Dettagli.Rows(0).Item("P_Ammesso")
            End If
            If Not IsDBNull(DtPC_Dettagli.Rows(0).Item("K_Ammesso")) Then
                K = DtPC_Dettagli.Rows(0).Item("K_Ammesso")
            End If

            Mas = -1

            If Not IsDBNull(DtPC_Dettagli.Rows(0).Item("N_Mas")) Then
                MasSalvato = True
                If IsNumeric(DtPC_Dettagli.Rows(0).Item("N_Mas")) Then
                    Mas = CDec(DtPC_Dettagli.Rows(0).Item("N_Mas"))
                End If
            End If

            DtPC_Dettagli = Nothing


        End If



        Dim objImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read

        If Veg_Cod = 0 Then

            Dt = objImpianti.Leggi_DescrizioniImpianti(Piva,
                                                       SaCod,
                                                       Appezza,
                                                       IdReg, enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                        "", "", objParametri)
            If Dt.Rows.Count > 0 Then
                Veg_Cod = Dt.Rows(0).Item("Veg_Cod")
                Session("strSpecie") = Dt.Rows(0).Item("Veg_Des") & Dt.Rows(0).Item("Grfi_Des")
            Else
                Veg_Cod = 0
            End If

        End If


        Dim objFinalitaInput As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FinalitaRER_input
        objFinalitaInput.Regolamento_Cod = regolamento
        objFinalitaInput.Veg_Cod = Veg_Cod
        Dim objFinalitaRer As New AgronicaCoreWebService.PianoConcimazione_WS
        Dim objFinalitaOutput As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FinalitaRER_output
        objFinalitaOutput = objFinalitaRer.FinalitaRER(objFinalitaInput)


        Dim objFaseCicloInput As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FasiCicloColturale_input
        objFaseCicloInput.Regolamento_Cod = regolamento
        objFaseCicloInput.Veg_Cod = Veg_Cod
        Dim objFaseCicloOutput As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FasiCicloColturale_output
        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
        objFaseCicloOutput = objPC_WS.FasiCicloColturale(objFaseCicloInput)

        CType(rpt.Section1.ReportObjects("TextFinalita"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = ""

        If Not IsNothing(objFinalitaOutput) Then
            'CType(rpt.Section1.ReportObjects("TextFinalita"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Fase/Ciclo: " & objFinalitaOutput.ListaFinalita(0).Descrizione 'Dt.Rows(0).Item("Grfi_Des")
            Dim Finalita As New AgronicaCorePianoConcimazioneBIZ.Finalita
            Finalita = objFinalitaOutput.ListaFinalita.Where(Function(x) x.Codice = Grfi_Cod)(0)
            If Not Finalita Is Nothing Then
                CType(rpt.Section1.ReportObjects("TextFinalita"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Finalità: " & Finalita.Descrizione
            End If
        End If

        If Not IsNothing(objFaseCicloOutput) Then
            Dim Fase As New AgronicaCorePianoConcimazioneBIZ.Fase
            Fase = objFaseCicloOutput.ListaFasi.Where(Function(x) x.Codice = FaseCiclo)(0)
            If Not Fase Is Nothing Then
                CType(rpt.Section1.ReportObjects("TextFinalita"), CrystalDecisions.CrystalReports.Engine.TextObject).Text &= " - Fase/Ciclo: " & Fase.Descrizione
            End If
        End If


        Dim i As Integer
        Dim DtImp As DataTable
        Dim strAppezza1 As String = ""
        Dim strAppezza2 As String = ""
        Dim SupImp As Double = 0

        Dim objEntitaxTestata As New AgronicaCoreAnagrafeDAL.PianoConcimazione_EntitaxTestata_R

        'Dim Dt As DataTable
        Dt = objEntitaxTestata.Leggi(PCTestataCod, 0, Piva, SaCod, 0, 0, 0, 0, "", "", "", 0, 0, "", 0, "",
                                     AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                     "", "",
                                     objParametri)

        Dim FiltroImpianti As String = ""
        For i = 0 To Dt.Rows.Count - 1
            FiltroImpianti &= " (Reg_Impianti.PIVA='" + Dt.Rows(i).Item("Piva").ToString + "' " +
                " AND Reg_Impianti.SA_COD=" + Dt.Rows(i).Item("Sa_Cod").ToString +
                " AND Reg_Impianti.APPEZZA=" + Dt.Rows(i).Item("appezza").ToString +
                " AND Reg_Impianti.ID_REG=" + Dt.Rows(i).Item("Id_Imp").ToString +
                " ) OR"
        Next

        If FiltroImpianti <> "" Then

            FiltroImpianti = Left(FiltroImpianti, FiltroImpianti.Length - 2)

            DtImp = objImpianti.Leggi_DescrizioniImpianti(Piva,
                                              SaCod,
                                              0,
                                              0,
                                              enumSelezioneVariabile.Selezione_TabellaCompleta,
                                              FiltroImpianti, "", objParametri)

            Dim ris As Integer

            Dim InseritaFinalita As Boolean = False

            For i = 0 To DtImp.Rows.Count - 1

                Math.DivRem(i + 2, 2, ris)

                If ris = 0 Then
                    strAppezza1 &= DtImp.Rows(i).Item("App_Nome") & "-" & DtImp.Rows(i).Item("Veg_Des") & "-" & DtImp.Rows(i).Item("Cul_Des") & " (" & Format(DtImp.Rows(i).Item("Sup_Imp"), "0.0000") & " ha)" & vbCrLf '& "-" & DtImp.Rows(0).Item("Grfi_Des") & vbCrLf
                Else
                    strAppezza2 &= DtImp.Rows(i).Item("App_Nome") & "-" & DtImp.Rows(i).Item("Veg_Des") & "-" & DtImp.Rows(i).Item("Cul_Des") & " (" & Format(DtImp.Rows(i).Item("Sup_Imp"), "0.0000") & " ha)" & vbCrLf  '& "-" & DtImp.Rows(0).Item("Grfi_Des") & vbCrLf
                End If

                If Not IsNothing(DtImp.Rows(i).Item("Grfi_Des")) AndAlso (DtImp.Rows(i).Item("Grfi_Des") <> "" And Not InseritaFinalita) Then
                    'CType(rpt.Section1.ReportObjects("TextFinalita"), CrystalDecisions.CrystalReports.Engine.TextObject).Text &= " Finalità: " & DtImp.Rows(i).Item("Grfi_Des")
                    Session("strSpecie") = DtImp.Rows(i).Item("Veg_Des") & DtImp.Rows(i).Item("Grfi_Des")
                    InseritaFinalita = True
                End If

                If Not IsNothing(DtImp.Rows(i).Item("Sup_Imp")) AndAlso IsNumeric(DtImp.Rows(i).Item("Sup_Imp")) Then
                    SupImp += DtImp.Rows(i).Item("Sup_Imp")
                End If

            Next

            'se non salvati modalita nuova nel dettaglio del piano
            'letti modalita vecchia nelle entita
            If (N = 0 And P = 0 And K = 0) Then
                If Dt.Rows.Count > 0 Then
                    N = Dt.Rows(0).Item("QtaMaxN")
                    Session("QtaMaxN") = N
                    P = Dt.Rows(0).Item("QtaMaxP2O5")
                    K = Dt.Rows(0).Item("QtaMaxK2O")
                End If
            End If

        End If

        Sup_Tot = SupImp

        objImpianti = Nothing

        CType(rpt.Section1.ReportObjects("TextAppezzamento"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Superficie Totale Appezzamenti : " & Format(SupImp, "0.0000") & " ha" ' "Appezzamenti (" & Format(SupImp, "0.0000") & " ha)"
        CType(rpt.ReportHeaderSection6.ReportObjects("TextElencoAppezzamenti1"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = strAppezza1
        CType(rpt.ReportHeaderSection6.ReportObjects("TextElencoAppezzamenti2"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = strAppezza2

        CType(rpt.Section3.ReportObjects("TextQtaN"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(N, "0.00")
        CType(rpt.Section3.ReportObjects("TextQtaP"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(P, "0.00")
        CType(rpt.Section3.ReportObjects("TextQtaK"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(K, "0.00")

        ' Note Intervento
        Dim objNote As New AgronicaCoreContabDAL.Note_Intervento_R
        Dim DtNote As DataTable
        DtNote = objNote.Leggi_con_Utilizzo(0, 0, enum_Note_Intervento_Utilizzo.PianoConcimazione, enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                        "", "", objParametri)

        Dim strNote As String = ""
        For i = 0 To DtNote.Rows.Count - 1
            strNote &= IIf(strNote <> "", vbCrLf, "") & DtNote.Rows(i).Item("Nota_Des")
        Next

        If strNote <> "" Then
            CType(rpt.Section4.ReportObjects("TextNoteIntervento"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = strNote
        End If

        objNote = Nothing


    End Sub




    '#######################################################################################################
    Public Sub Carica_Dati(ByRef Disponibilita As DS_Fattori, ByRef Necessita As DS_Fattori,
                            ByVal PCTestataCod As Integer, ByVal Piva As String, ByVal Sup_Tot As Decimal,
                            ByVal Mas As Decimal, ByVal MasSalvato As Boolean,
                            ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            ByRef rpt As RPT_Bilancio_NPK)

        Dim objParametriIngresso As AgronicaCorePianoConcimazioneBIZ.PianoConcimazioneBilancio_input

        Try

            objParametriIngresso = New AgronicaCorePianoConcimazioneBIZ.PianoConcimazioneBilancio_input(PCTestataCod, objParametri)

            Dim objParametriUscita As AgronicaCorePianoConcimazioneBIZ.PianoConcimazioneBilancio_output
            Dim objBilancio As New AgronicaCoreWebService.PianoConcimazione_WS
            objParametriUscita = objBilancio.CalcolaBilancio(objParametriIngresso)

            Dim n_riga As Integer = 0
            For Each nec In objParametriUscita.ListaNecessita
                n_riga += 1
                NuovaRiga(Necessita, n_riga, nec.Descrizione, Format(nec.Valore_N, "0.00"), Format(nec.Valore_P, "0.00"), Format(nec.Valore_K, "0.00"))
            Next


            n_riga += 1
            NuovaRiga(Necessita, n_riga, " Totale Necessità", Format(objParametriUscita.N_Necessario, "0.00"), Format(objParametriUscita.P_Necessario, "0.00"), Format(objParametriUscita.K_Necessario, "0.00"))


            n_riga = 0
            For Each disp In objParametriUscita.ListaDisponibilita
                n_riga += 1
                NuovaRiga(Disponibilita, n_riga, disp.Descrizione, Format(disp.Valore_N, "0.00"), Format(disp.Valore_P, "0.00"), Format(disp.Valore_K, "0.00"))
            Next

            n_riga += 1
            NuovaRiga(Disponibilita, n_riga, "Totale Disponibilità", Format(objParametriUscita.N_Disponibile, "0.00"), Format(objParametriUscita.P_Disponibile, "0.00"), Format(objParametriUscita.K_Disponibile, "0.00"))


            n_riga += 1
            NuovaRiga(Disponibilita, n_riga, "Bisogno Calcolato", Format(objParametriUscita.N_Calcolato, "0.00"),
                                                                Format(objParametriUscita.P_Calcolato, "0.00"),
                                                                Format(objParametriUscita.K_Calcolato, "0.00"))


            Dim AzotoInStampa As Double

            If Not IsNothing(Session("QtaMaxN")) AndAlso CDbl(Session("QtaMaxN")) <> 0 Then

                Dim QtaMax As Double = CDbl(Session("QtaMaxN"))
                If QtaMax >= objParametriUscita.N_Ammesso Then
                    rpt.ReportHeaderSection5.SectionFormat.EnableSuppress = True
                    AzotoInStampa = objParametriUscita.N_Ammesso

                Else
                    AzotoInStampa = QtaMax
                End If

            Else

                AzotoInStampa = objParametriUscita.N_Ammesso

            End If

            NuovaRiga(Disponibilita, 7, "Apporto ammesso col bilancio (kg/ha)", Format(AzotoInStampa, "0.00"),
                                                                                Format(objParametriUscita.P_Ammesso, "0.00"),
                                                                                Format(objParametriUscita.K_Ammesso, "0.00"))

            Dim supImp As Double = Sup_Tot


            NuovaRiga(Disponibilita, 8, "Apporto ammesso col bilancio (kg)", Format(AzotoInStampa * supImp, "0.00"),
                                                                             Format(objParametriUscita.P_Ammesso * supImp, "0.00"),
                                                                             Format(objParametriUscita.K_Ammesso * supImp, "0.00"))

            Sup_Tot = 0


            '(06/10/2020 fede) aggiunta eventuale considerazione del fattore correttivo per resa maggiore
            'Dim MAS As Decimal = 0
            Dim strFattoreCorrettivo As String = ""

            '(23/07/2021 fede) spostato salvataggio del mas in tabella

            'pregresso
            If MasSalvato = False Then
                Mas = objParametriUscita.Limite_Mas
            End If


            If Mas >= 0 Then

                If objParametriUscita.FattoreCorrettivo_N > 0 Then
                    If objParametriIngresso.Resa > 0 AndAlso objParametriUscita.Resa_Rif > 0 Then
                        If objParametriIngresso.Resa - objParametriUscita.Resa_Rif > 0 Then
                            Mas = Mas + ((objParametriIngresso.Resa - objParametriUscita.Resa_Rif) * objParametriUscita.FattoreCorrettivo_N)
                            strFattoreCorrettivo = " (" & "considerando il Fattore Correttivo di " & objParametriUscita.FattoreCorrettivo_N & " Kg N/t)"
                        End If
                    End If
                End If

                Dim strAttenzione As String = ""
                If objParametriUscita.N_Ammesso > Mas Then
                    strAttenzione = "ATTENZIONE: l'apporto di N non può superare il limite MAS di " & Mas.ToString & " kg/ha" & strFattoreCorrettivo
                Else
                    strAttenzione = "Limite MAS: " & Mas.ToString & " kg/ha" & strFattoreCorrettivo
                End If

                CType(rpt.Section5.ReportObjects("TextAttenzione"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = strAttenzione

            Else

                CType(rpt.Section5.ReportObjects("TextAttenzione"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Limite MAS: n.d."

            End If


        Catch ex As Exception

        End Try


    End Sub

    '#######################################################################################################
    Private Sub CreaDt(ByRef Dt As DataTable)

        Dt.Columns.Add(New DataColumn("Indice", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Descrizione", GetType(String)))
        Dt.Columns.Add(New DataColumn("N", GetType(String)))
        Dt.Columns.Add(New DataColumn("P", GetType(String)))
        Dt.Columns.Add(New DataColumn("K", GetType(String)))

    End Sub

    '###################################################################################################
    Private Sub NuovaRiga(ByRef dsFattori As DS_Fattori, _
                          ByVal Indice As Integer, _
                          ByVal Descrizione As String, _
                          ByVal N As String, _
                          ByVal P As String, _
                          ByVal K As String)

        Dim Dr As DS_Fattori.DT_FattoriRow

        Dr = dsFattori.DT_Fattori.NewDT_FattoriRow

        Dr.Codice = Indice.ToString
        Dr.Descrizione = Descrizione
        Dr.N = N
        Dr.P2O5 = P
        Dr.K2O = K

        dsFattori.DT_Fattori.AddDT_FattoriRow(Dr)

    End Sub



    Private Sub ApriScadenziario()

        'If IsNothing(Session("Analisi_Testata_Cod")) OrElse Session("Analisi_Testata_Cod") = 0 Then
        '    AgronicaCoreUtility.Messaggi.AgroMsgBox("Impossibile associare un allegato ad una nuova analisi ", Page)
        '    Exit Sub
        'End If

        'Dim objXmlPassaggio As New AgronicaCoreGestioneRichieste.Scrivi.
        'ha già creato i parametri di sessione e i parametri del web config
        Dim ParametriScadenziario As New AgronicaCoreGestioneRichieste.ParametriScadenziario
        ParametriScadenziario.Pagina_Richiesta = enum_PagineGiasOnline_2010.Nuova_Scandenza
        ParametriScadenziario.Piva = Qs_Piva

        ParametriScadenziario.Id_Area = enum_ID_Area_Alert.PianiConcimazione
        ParametriScadenziario.Id_Tipologia = enum_ID_Area_Tipologia.Piano_Concimazione
        ParametriScadenziario.Analisi_Testata_Cod = Qs_PCTestataCod
        'If IsDate(Txt_DataFine.Text) Then
        '    objXmlPassaggio.ParametriScadenziario.Data_Scadenza = Txt_DataFine.Text
        'Else
        'objXmlPassaggio.ParametriScadenziario.Data_Scadenza = Now.Date.ToShortDateString
        ' End If


        'Dim script As String

        'Dim objAgroWebconfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
        'script = objXmlPassaggio.ApriIframeConSito(objAgroWebconfig.LinkGiasOnline_2010, TipiEnumerativi.Enum_SiteRedirector.Sito_PianoConcimazione, TipiEnumerativi.Enum_SiteRedirector.Sito_GiasOnline_2010)

        Dim script As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriIFrame_SitoOnline2010_PassandoDirettamente_ParametriScadenziario(AgronicaCoreDataProvider.TipiEnumerativi.Enum_SiteRedirector.Sito_PianoConcimazione, ParametriScadenziario)


        'aggiungo lo script 
        ClientScript.RegisterClientScriptBlock(Page.GetType, "frame", script)


    End Sub

End Class