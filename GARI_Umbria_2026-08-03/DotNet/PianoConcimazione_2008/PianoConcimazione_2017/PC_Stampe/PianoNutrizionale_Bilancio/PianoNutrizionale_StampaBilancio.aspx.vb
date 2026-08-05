Option Strict Off

Imports AgronicaControlli_2010
Imports AgronicaControlli_2010.UtilityPersonalizzazioniGraficheCliente
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.ConnessioniTransazioni
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Gestione_Eccezioni_2015
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDTOStd.Identity
Imports AgronicaCoreModelsSTD.PersonalizzazioniGraficheCliente
Imports AgronicaCorePianoConcimazioneBIZ
Imports CrystalDecisions.Shared

Public Class PianoNutrizionale_StampaBilancio
    Inherits System.Web.UI.Page

#Region "Dichiarazione Variabili"
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
    Dim Nome_Documento As String = "PianoNutrizionale_StampaBilancio"

    '----- Gestione Report
    Private rptBilancio_PianoNutrizionale As RPT_PianoNutrizionale_StampaBilancio
    Private rptFooterLogo As FooterLogo

    Dim personalizzazioniGraficheCliente As PersonalizzazioniGraficheCliente = Nothing

#End Region
    '##########################################################################################################
    Private Sub PianoNutrizionale_StampaBilancio_Init(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Init
        rptBilancio_PianoNutrizionale = New RPT_PianoNutrizionale_StampaBilancio
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

        '----- Verifico se l'utente dispone dei permessi per la visualizzazione della pagina

        Dim strDummy As String     'controllo accesso negato.....
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
        End If

        '######################################################################################################################
        Session.Timeout = 180

        '############################################
        '#####    Recupero le variabili    ##########
        '############################################

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
        Dim DS_LogoFooter As New DS_LogoFooter

        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        If Not Me.IsPostBack Then

            rptBilancio_PianoNutrizionale = New RPT_PianoNutrizionale_StampaBilancio

            Dim Sup_Tot As Double = 0

            Try
                Dati_PianoNutrizionale(Qs_Piva, xSa_Cod, xAppezza, xID_Imp,
                                       Qs_PCTestataCod, "", Sup_Tot, 0,
                                       objParametri_Server, rptBilancio_PianoNutrizionale)
            Catch ex As Exception
                Log_Errori += "- Dati_Piano: " + vbCrLf + ex.Message + vbCrLf
            End Try

            'Dim DsPianoNutrizionale As New DS_PianoNutrizionale_StampaBilancio
            'Try
            '    rptBilancio_PianoNutrizionale.SetDataSource(DsPianoNutrizionale)
            'Catch ex As Exception
            '    Log_Errori += "- SetDataSource: " + vbCrLf + ex.Message + vbCrLf
            'End Try

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
                    rptBilancio_PianoNutrizionale.ReportFooterSection2.SectionFormat.EnableSuppress = True
                End If

            Catch ex As Exception
                Log_Errori += "- Lettura Utenti_Impostazioni e Nascondi/Mostra Sezioni: " + vbCrLf + ex.Message + vbCrLf
            End Try


            'creo la variabile per valorizzarla nel SalvaPdf che poi mi server per la gestione_allegati
            Dim NomeFile = CreaNomeFile(Qs_Piva, Qs_PCTestataCod, objParametri_Server)
            ' il pdf viene salvato sempre

            Dim strFile As String = ""

            Try
                Try
                    ' salvo il report in formato PDF
                    Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
                    objGestFile.SalvaReportPdf(rptBilancio_PianoNutrizionale,
                                           enum_CategorieDocumenti.PianoConcimazione,
                                           "Piano Concimazione",
                                           NomeFile,
                                           objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)

                    'strFile = SalvaPdf(objParametri_Server, rptBilancioNPK, NomeFile)
                Catch ex As Exception
                    Log_Errori += "- SalvaPdf: " + vbCrLf + ex.Message + vbCrLf
                End Try
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
                                                             "Piano Nutrizionale",
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
                            Throw New Exception("Non sono riuscito ad associare l'allegato al corrente Piano Nutrizionale. PC_cod =" & Qs_PCTestataCod.ToString)
                        End If
                    End If

                    objAllegati = Nothing

                End If
            Catch ex As Exception
                Log_Errori += "- gestione allegati: " + vbCrLf + ex.Message + vbCrLf
            End Try

            Dim reportTemporano As String = CrystalHelper.getFileReportTemporaneo()
            Try
                rptBilancio_PianoNutrizionale.SaveAs(reportTemporano, True)
            Catch ex As Exception
                Log_Errori += "- Salvataggio report temporaneo: " + vbCrLf + ex.Message + vbCrLf
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
                                      "PianoNutrizionale",
                                       Nome_File & ".txt",
                                       Session("ASG_Utente_Username"),
                                        "PianoNutrizionale_StampaBilancio",
                                         Log_Errori)

            End If

            If Qs_Anteprima = "1" Then
                'Session("Report") = rptBilancio_PianoNutrizionale
                'Response.Redirect("..\VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server))
                Response.Redirect("..\VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server) +
                              "&tmpReportPath=" + Stringa_Codifica(reportTemporano, AgroKey_EncoderDecoder, Server) &
                              "&NomePdf=" & Stringa_Codifica(NomeFile, AgroKey_EncoderDecoder, Server))
            Else

                Session("Report") = rptBilancio_PianoNutrizionale

                Dim UrlStampa As String
                Dim UrlFiltro As String

                UrlStampa = "../VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("0", AgroKey_EncoderDecoder, Server) +
                              "&tmpReportPath=" + Stringa_Codifica(reportTemporano, AgroKey_EncoderDecoder, Server) &
                              "&NomePdf=" & Stringa_Codifica(NomeFile, AgroKey_EncoderDecoder, Server)

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
        End If

        'In caso le personalizzazioni siano attive, nascondo il logo e ragione sociale Agronica.
        'If personalizzazioniGraficheCliente IsNot Nothing Then
        '    rptBilancio_PianoNutrizionale.Section5.ReportObjects("Picture3").ObjectFormat.EnableSuppress = True
        '    rptBilancio_PianoNutrizionale.Section5.ReportObjects("Text1").ObjectFormat.EnableSuppress = True
        'End If

    End Sub


    '################################################################################
    Public Function CreaNomeFile(ByVal Piva As String, ByVal PC_TestataCod As Integer,
                                 ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String


        Dim strCentro As String = Session("strCentro")
        Dim strSpecie As String = Session("strSpecie")


        strCentro = AgronicaCoreUtility.Stringhe.EliminaCaratteriSpecialiFile(strCentro)
        strSpecie = AgronicaCoreUtility.Stringhe.EliminaCaratteriSpecialiFile(strSpecie)

        '  Galassi, 03/04/2017 09.55.39: Se non ho la specie va in errore la stampa perchè non riesce a trovare il nome del file quindi controllo il strSpecie
        If IsNothing(strSpecie) OrElse strSpecie.Length < 1 Then
            strSpecie = " "
        End If

        '  Galassi, 03/04/2017 09.55.39: per sicurezza lo faccio anche per il centro
        If IsNothing(strCentro) OrElse strCentro.Length < 1 Then
            strCentro = " "
        End If

        Dim CodSocio As String = Trim(Session("Socio"))

        CodSocio = CodSocio.Replace("/", "_")
        CodSocio = CodSocio.Replace("\", "_")
        CodSocio = CodSocio.Replace("|", "_")

        Dim CentroConferimento As String

        Dim objAnagrafe As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read

        CentroConferimento = objAnagrafe.Leggi_Codice_from_Imprese_Codici(Piva, enum_CodiciAnagrafe.V01CON00_VCCOIS, objParametri)
        objAnagrafe = Nothing

        Dim NomeFile As String = "PCS" &
                                 IIf(CentroConferimento <> "", "_" & CentroConferimento, "") &
                                 "_" & strCentro.Substring(0, Math.Min(15, strCentro.Length - 1)) &
                                 IIf(CodSocio <> "", "_" & CodSocio, "") &
                                 "_" & strSpecie.Substring(0, Math.Min(15, strSpecie.Length - 1)) &
                                 "_" & PC_TestataCod & ".pdf"

        Return NomeFile
    End Function

    '################################################################################
    Public Function SalvaPdf(ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                             ByRef rptPianoNutrizionale As RPT_PianoNutrizionale_StampaBilancio,
                             Optional ByRef NomeFile As String = "") As String



        ' Dichiara le variabili e restituisce le opzioni di esportazione.
        Dim exportOpts As New ExportOptions
        Dim diskOpts As New DiskFileDestinationOptions
        Dim strPath As String = ""
        Dim PathCartella As String

        Try

            exportOpts = rptPianoNutrizionale.ExportOptions

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
            rptPianoNutrizionale.Export()

        Catch ex As Exception
            Log_Errori += "- public_SalvaPdf: " + vbCrLf + ex.Message + vbCrLf
            Return ""
        End Try

        Return strPath

    End Function



    '################################################################################
    Public Sub Dati_PianoNutrizionale(ByVal Piva As String, ByVal SaCod As Integer, ByVal Appezza As Integer, ByVal IdReg As Integer,
                                      ByVal PCTestataCod As Integer, ByRef Sa_Nome As String, ByRef Sup_Tot As Double,
                                      ByRef regolamento As AgronicaCoreDataProvider.TipiEnumerativi.enum_PUARegolamenti,
                                      ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                      ByRef rptScheda_PianoNutrizionale As RPT_PianoNutrizionale_StampaBilancio)

        Dim dsPianoNutrizionale As New DS_PianoNutrizionale_StampaBilancio()
        Dim drPianoNutrizionale = dsPianoNutrizionale.DT_DatiPiano.NewDT_DatiPianoRow

        Dim Dt_objCentri As New DataTable
        Dim Rag_Soc As String = ""
        Dim strErr As String = ""


        Dim N As Decimal = 0
        Dim P As Decimal = 0
        Dim K As Decimal = 0
        Dim Veg_Cod As Integer = 0

        Dim DS_LogoFooter As New DS_LogoFooter

        Try
            '---------------------------------------------------------------------
            Dim CodSocio As String
            Dim objCodiciImpresa As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
            CodSocio = objCodiciImpresa.Leggi_Codice_from_Imprese_Codici(Piva, enum_CodiciAnagrafe.Codice_Socio, objParametri) & "   "
            Session("Socio") = CodSocio


            '--- PC_DETTAGLI
            Dim DtPC_Dettagli As DataTable
            Dim objPC_Dettagli As New AgronicaCorePianoConcimazioneDAL.PianoConcimazione_Dettagli_R
            DtPC_Dettagli = objPC_Dettagli.Leggi_default(PCTestataCod,
                                                     0, Piva,
                                                     enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                     "", "",
                                                     objParametri)

            '--- PC_TESTATA
            Dim DtPC_Testata As DataTable
            Dim objPC_Testata As New AgronicaCorePianoConcimazioneDAL.PianoConcimazione_Testata_R
            DtPC_Testata = objPC_Testata.Leggi_default(
                                                PCTestataCod,
                                                0, 0, enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                "", "",
                                                objParametri)

            '--- CONSIGLIO N
            Dim objParametriIngresso As AgronicaCorePianoConcimazioneBIZ.PianoNutrizionaleBilancio_input

            '######################################
            '#######   DATI TESTATA PIANO   #######
            '######################################

            If Not IsNothing(DtPC_Testata) AndAlso DtPC_Testata.Rows.Count > 0 Then

                regolamento = DtPC_Testata.Rows(0).Item("Regolamento_Cod")
                drPianoNutrizionale.PianoNutrizionale = DtPC_Testata.Rows(0).Item("PC_Testata_Des")
                drPianoNutrizionale.Note = DtPC_Testata.Rows(0).Item("Note")


                Dim ValiditaInizio, ValiditaFine As String
                ValiditaInizio = DtPC_Testata.Rows(0).Item("Validita_Inizio")
                ValiditaFine = DtPC_Testata.Rows(0).Item("Validita_Fine")

                drPianoNutrizionale.ValiditaDAL = ValiditaInizio
                drPianoNutrizionale.ValiditaAL = ValiditaFine

                If ValiditaInizio = "01/01/1900" Then
                    ValiditaInizio = ""
                End If
                If ValiditaFine = "31/12/2100" Then
                    ValiditaFine = ""
                End If

                If ValiditaInizio <> "" Or ValiditaFine <> "" Then
                    drPianoNutrizionale.ValiditaDAL = ValiditaInizio
                    drPianoNutrizionale.ValiditaAL = ValiditaFine
                Else
                    drPianoNutrizionale.ValiditaDAL = ""
                    drPianoNutrizionale.ValiditaAL = ""
                End If
            End If
            DtPC_Testata = Nothing

            '##################################
            '#######   DETTAGLI PIANO   #######
            '##################################

            If Not IsNothing(DtPC_Dettagli) AndAlso DtPC_Dettagli.Rows.Count > 0 Then

                drPianoNutrizionale.Anno = DtPC_Dettagli.Rows(0).Item("PC_Dettagli_Anno")
                drPianoNutrizionale.Biologico = DtPC_Dettagli.Rows(0).Item("PC_Dettagli_bio")

                drPianoNutrizionale.Pioggia_Autunno = DtPC_Dettagli.Rows(0).Item("PC_Dettagli_Piovosita")
                drPianoNutrizionale.Pioggia_Primavera = DtPC_Dettagli.Rows(0).Item("PC_Dettagli_Piovosita_Primavera")

                '---------------------------------------------------------------------
                'Ubicazione
                Dim listaUbicazioni As List(Of Ubicazione) = GetListaUbicazioniDes(regolamento)

                If listaUbicazioni.Count > 0 Then
                    drPianoNutrizionale.Ubicazione = GetUbicazioneDes(listaUbicazioni, DtPC_Dettagli.Rows(0).Item("PC_Dettagli_Ubicazione_Cod"))
                End If


                '---------------------------------------------------------------------
                'Ricavo la Ragione Sociale e Il Nome del Centro
                Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
                Dt_objCentri = objCentri.Anagrafica_Centri_Leggi(Piva,
                                                                 DtPC_Dettagli.Rows(0).Item("PC_Dettagli_SaCod"),
                                                                 strErr,
                                                                 enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                 "", "",
                                                                 objParametri)

                If Dt_objCentri.Rows.Count > 0 Then
                    Rag_Soc = CodSocio & " " & Dt_objCentri.Rows(0).Item("Rag_Soc")
                    drPianoNutrizionale.RagSoc = Dt_objCentri.Rows(0).Item("Rag_Soc")

                    If Dt_objCentri.Rows.Count = 1 Then
                        Sa_Nome = Dt_objCentri.Rows(0).Item("Sa_Nome")
                        drPianoNutrizionale.Centro = Dt_objCentri.Rows(0).Item("Sa_Nome")
                    Else
                        drPianoNutrizionale.Centro = ""
                    End If
                    Session("strCentro") = Dt_objCentri.Rows(0).Item("Rag_Soc") & Sa_Nome
                End If


                '---------------------------------------------------------------------
                'Ricavo Precessioni e Fertilizzante
                Dim listaPrecessioni As List(Of Precessione) = GetListaPrecessioniDes(regolamento, 0)

                ' ---- PRECESSIONE
                If DtPC_Dettagli.Rows(0).Item("PC_Dettagli_Precessione_Veg_Cod") = 0 Then
                    drPianoNutrizionale.Precessione = "Non definita"
                Else
                    If listaPrecessioni.Count > 0 Then
                        drPianoNutrizionale.Precessione = GetPrecessioneDes(listaPrecessioni, DtPC_Dettagli.Rows(0).Item("PC_Dettagli_Precessione_Veg_Cod"))
                    End If
                End If


                ' ---- PRECESSIONE ANNO PRECEDENTE
                If DtPC_Dettagli.Rows(0).Item("PC_Dettagli_Precessione_Veg_Cod_AnnoPrecedente") = 0 Then
                    drPianoNutrizionale.PrecessioneAnnoPrecedente = "Non definita"
                Else
                    If listaPrecessioni.Count > 0 Then
                        drPianoNutrizionale.PrecessioneAnnoPrecedente = GetPrecessioneDes(listaPrecessioni, DtPC_Dettagli.Rows(0).Item("PC_Dettagli_Precessione_Veg_Cod_AnnoPrecedente"))
                    End If
                End If


                ' ---- PRECESSIONE 2 ANNI PRIMA
                If DtPC_Dettagli.Rows(0).Item("PC_Dettagli_Precessione_Veg_Cod_2anni_prima") = 0 Then
                    drPianoNutrizionale.PrecessioneDueAnniPrima = "Non definita"
                Else
                    If listaPrecessioni.Count > 0 Then
                        drPianoNutrizionale.PrecessioneDueAnniPrima = GetPrecessioneDes(listaPrecessioni, DtPC_Dettagli.Rows(0).Item("PC_Dettagli_Precessione_Veg_Cod_2anni_prima"))
                    End If
                End If


                ' ---- PRECESSIONE 3 ANNI PRIMA
                If DtPC_Dettagli.Rows(0).Item("PC_Dettagli_Precessione_Veg_Cod_3anni_prima") = 0 Then
                    drPianoNutrizionale.PrecessioneTreAnniPrima = "Non definita"
                Else
                    If listaPrecessioni.Count > 0 Then
                        drPianoNutrizionale.PrecessioneTreAnniPrima = GetPrecessioneDes(listaPrecessioni, DtPC_Dettagli.Rows(0).Item("PC_Dettagli_Precessione_Veg_Cod_3anni_prima"))
                    End If
                End If


                ' ---- FERTILIZZANTE
                Dim listaFertilizzanti As List(Of MatriciOrganiche) = GetListaFertilizzantiDes(regolamento, 0)

                If DtPC_Dettagli.Rows(0).Item("PC_Dettagli_Fertilizzazione_id_tp_fer") = 0 Then
                    drPianoNutrizionale.Fertilizzante = "Non definito"
                Else
                    If listaFertilizzanti.Count > 0 Then
                        drPianoNutrizionale.Fertilizzante = GetFertilizzanteDes(listaFertilizzanti, DtPC_Dettagli.Rows(0).Item("PC_Dettagli_Fertilizzazione_id_tp_fer"))
                    End If
                End If



                '---------------------------------------------------------------------
                '--- ANALISI
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

                        Analisi_Des = "Analisi "
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
                drPianoNutrizionale.Analisi = Analisi_Des

                drPianoNutrizionale.Sabbia = DtPC_Dettagli.Rows(0).Item("PC_Dettagli_Sabbia")
                drPianoNutrizionale.Limo = DtPC_Dettagli.Rows(0).Item("PC_Dettagli_Limo")
                drPianoNutrizionale.Argilla = DtPC_Dettagli.Rows(0).Item("PC_Dettagli_Argilla")
                drPianoNutrizionale.NTOT = DtPC_Dettagli.Rows(0).Item("PC_Dettagli_ntot")
                drPianoNutrizionale.NORG = DtPC_Dettagli.Rows(0).Item("PC_Dettagli_norg")

                'Dim PC_Dettagli_Flag_P As Integer = 0
                'Dim PC_Dettagli_Flag_K As Integer = 0
                'If Not IsDBNull(DtPC_Dettagli.Rows(0).Item("PC_Dettagli_Flag_P")) Then
                '    PC_Dettagli_Flag_P = DtPC_Dettagli.Rows(0).Item("PC_Dettagli_Flag_P")
                'End If
                'If Not IsDBNull(DtPC_Dettagli.Rows(0).Item("PC_Dettagli_Flag_K")) Then
                '    PC_Dettagli_Flag_K = DtPC_Dettagli.Rows(0).Item("PC_Dettagli_Flag_K")
                'End If
                'If PC_Dettagli_Flag_P = 0 Then
                '    drPianoNutrizionale.P205 = DtPC_Dettagli.Rows(0).Item("PC_Dettagli_p2o5")
                'Else
                '    drPianoNutrizionale.P205 = DtPC_Dettagli.Rows(0).Item("PC_Dettagli_p")
                'End If
                'If PC_Dettagli_Flag_K = 0 Then
                '    drPianoNutrizionale.K205 = DtPC_Dettagli.Rows(0).Item("PC_Dettagli_k2o")
                'Else
                '    drPianoNutrizionale.K205 = DtPC_Dettagli.Rows(0).Item("PC_Dettagli_k")
                'End If

                'salvati modalita nuova nel dettaglio del piano
                If Not IsDBNull(DtPC_Dettagli.Rows(0).Item("PC_Dettagli_ColturaPrincipale_Veg_Cod")) Then
                    Veg_Cod = DtPC_Dettagli.Rows(0).Item("PC_Dettagli_ColturaPrincipale_Veg_Cod")
                    If Veg_Cod <> 0 Then
                        Dim objSp As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
                        Session("strSpecie") = objSp.VegDes_from_VegCod(Veg_Cod, objParametri)

                        Dim Dt_SpecieVegetale As DataTable
                        Dt_SpecieVegetale = objSp.Leggi_x_PDC(objParametri)

                        If Dt_SpecieVegetale.Rows.Count > 0 Then
                            drPianoNutrizionale.SpecieVegetale = Dt_SpecieVegetale.Rows(6).Item("Veg_Des") '6 = Barbabietola da zucchero
                        End If
                    End If
                End If

                If Not IsDBNull(DtPC_Dettagli.Rows(0).Item("N_Ammesso")) Then
                    drPianoNutrizionale.N_Ammesso = DtPC_Dettagli.Rows(0).Item("N_Ammesso")
                End If
                'If Not IsDBNull(DtPC_Dettagli.Rows(0).Item("P_Ammesso")) Then
                '    drPianoNutrizionale.P_Ammesso = DtPC_Dettagli.Rows(0).Item("P_Ammesso")
                'End If
                'If Not IsDBNull(DtPC_Dettagli.Rows(0).Item("K_Ammesso")) Then
                '    drPianoNutrizionale.K_Ammesso = DtPC_Dettagli.Rows(0).Item("K_Ammesso")
                'End If

                DtPC_Dettagli = Nothing

            End If

            '###############################
            '#######   CONSIGLIO N   #######
            '###############################

            objParametriIngresso = New AgronicaCorePianoConcimazioneBIZ.PianoNutrizionaleBilancio_input(PCTestataCod, objParametri)

            Dim objParametriUscita As AgronicaCorePianoConcimazioneBIZ.PianoNutrizionaleBilancio_output
            Dim objBilancio As New AgronicaCoreWebService.PianoConcimazione_WS
            objParametriUscita = objBilancio.CalcolaPianoNutrizionaleBilancio(objParametriIngresso)


            drPianoNutrizionale.N_Ammesso = objParametriUscita.N_Ammesso

            If objParametriUscita.Integrazione_N <> "" Then
                drPianoNutrizionale.N_Integrazione = objParametriUscita.Integrazione_N
            End If

#Region "IMPIANTI"
            'Dim objImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read

            'If Veg_Cod = 0 Then
            '    Dt_objCentri = objImpianti.Leggi_DescrizioniImpianti(Piva, SaCod, Appezza, IdReg,
            '                                           enumSelezioneVariabile.Selezione_TabellaCompleta,
            '                                           "", "", objParametri)
            '    If Dt_objCentri.Rows.Count > 0 Then
            '        Veg_Cod = Dt_objCentri.Rows(0).Item("Veg_Cod")
            '        Session("strSpecie") = Dt_objCentri.Rows(0).Item("Veg_Des") & Dt_objCentri.Rows(0).Item("Grfi_Des")
            '    Else
            '        Veg_Cod = 0
            '    End If
            'End If


            'Dim i As Integer
            'Dim DtImp As DataTable
            'Dim strAppezza1 As String = ""
            'Dim strAppezza2 As String = ""
            'Dim SupImp As Double = 0

            'Dim objEntitaxTestata As New AgronicaCoreAnagrafeDAL.PianoConcimazione_EntitaxTestata_R

            'Dt_objCentri = objEntitaxTestata.Leggi(PCTestataCod, 0, Piva, SaCod, 0, 0, 0, 0, "", "", "", 0, 0, "", 0, "",
            '                         AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
            '                         "", "",
            '                         objParametri)

            'Dim FiltroImpianti As String = ""

            'For i = 0 To Dt_objCentri.Rows.Count - 1
            '    FiltroImpianti &= " (Reg_Impianti.PIVA='" + Dt_objCentri.Rows(i).Item("Piva") + "' " +
            '    " AND Reg_Impianti.SA_COD=" + Dt_objCentri.Rows(i).Item("Sa_Cod").ToString +
            '    " AND Reg_Impianti.APPEZZA=" + Dt_objCentri.Rows(i).Item("appezza").ToString +
            '    " AND Reg_Impianti.ID_REG=" + Dt_objCentri.Rows(i).Item("Id_Imp").ToString +
            '    " ) OR"
            'Next

            'If FiltroImpianti <> "" Then

            '    FiltroImpianti = Left(FiltroImpianti, FiltroImpianti.Length - 2)

            '    DtImp = objImpianti.Leggi_DescrizioniImpianti(Piva,
            '                                                  SaCod,
            '                                                  0,
            '                                                  0,
            '                                                  enumSelezioneVariabile.Selezione_TabellaCompleta,
            '                                                  FiltroImpianti, "", objParametri)

            '    Dim ris As Integer


            '    For i = 0 To DtImp.Rows.Count - 1

            '        Math.DivRem(i + 2, 2, ris)

            '        If ris = 0 Then
            '            strAppezza1 &= DtImp.Rows(i).Item("App_Nome") & "-" & DtImp.Rows(i).Item("Veg_Des") & "-" & DtImp.Rows(i).Item("Cul_Des") & " (" & Format(DtImp.Rows(i).Item("Sup_Imp"), "0.0000") & " ha)" & vbCrLf '& "-" & DtImp.Rows(0).Item("Grfi_Des") & vbCrLf
            '        Else
            '            strAppezza2 &= DtImp.Rows(i).Item("App_Nome") & "-" & DtImp.Rows(i).Item("Veg_Des") & "-" & DtImp.Rows(i).Item("Cul_Des") & " (" & Format(DtImp.Rows(i).Item("Sup_Imp"), "0.0000") & " ha)" & vbCrLf  '& "-" & DtImp.Rows(0).Item("Grfi_Des") & vbCrLf
            '        End If

            '        If Not IsNothing(DtImp.Rows(i).Item("Grfi_Des")) AndAlso (DtImp.Rows(i).Item("Grfi_Des") <> "") Then
            '            Session("strSpecie") = DtImp.Rows(i).Item("Veg_Des") & DtImp.Rows(i).Item("Grfi_Des")
            '        End If

            '        If Not IsNothing(DtImp.Rows(i).Item("Sup_Imp")) AndAlso IsNumeric(DtImp.Rows(i).Item("Sup_Imp")) Then
            '            SupImp += DtImp.Rows(i).Item("Sup_Imp")
            '        End If
            '    Next

            '    'se non salvati modalita nuova nel dettaglio del piano
            '    'letti modalita vecchia nelle entita
            '    If (N = 0 And P = 0 And K = 0) Then
            '        If Dt_objCentri.Rows.Count > 0 Then
            '            N = Dt_objCentri.Rows(0).Item("QtaMaxN")
            '            Session("QtaMaxN") = N
            '            P = Dt_objCentri.Rows(0).Item("QtaMaxP2O5")
            '            K = Dt_objCentri.Rows(0).Item("QtaMaxK2O")
            '        End If
            '    End If
            'End If

            'Sup_Tot = SupImp
            'objImpianti = Nothing

            ''CType(rpt.Section1.ReportObjects("TextAppezzamento"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Superficie Totale Appezzamenti : " & Format(SupImp, "0.0000") & " ha" ' "Appezzamenti (" & Format(SupImp, "0.0000") & " ha)"
            ''CType(rpt.Section1.ReportObjects("TextElencoAppezzamenti1"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = strAppezza1
            ''CType(rpt.Section1.ReportObjects("TextElencoAppezzamenti2"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = strAppezza2

            ''CType(rpt.Section3.ReportObjects("TextQtaN"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(N, "0.00")
            ''CType(rpt.Section3.ReportObjects("TextQtaP"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(P, "0.00")
            ''CType(rpt.Section3.ReportObjects("TextQtaK"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(K, "0.00")

            '' Note Intervento
            'Dim objNote As New AgronicaCoreContabDAL.Note_Intervento_R
            'Dim DtNote As DataTable
            'DtNote = objNote.Leggi_con_Utilizzo(0, 0, enum_Note_Intervento_Utilizzo.PianoConcimazione, enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
            '                            "", "", objParametri)

            'Dim strNote As String = ""
            'For i = 0 To DtNote.Rows.Count - 1
            '    strNote &= IIf(strNote <> "", vbCrLf, "") & DtNote.Rows(i).Item("Nota_Des")
            'Next

            ''If strNote <> "" Then
            ''    CType(rpt.Section4.ReportObjects("TextNoteIntervento"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = strNote
            ''End If

            'objNote = Nothing
#End Region

            '-----------------------------------------------------------------------------
            dsPianoNutrizionale.DT_DatiPiano.Rows.Add(drPianoNutrizionale)
            '-----------------------------------------------------------------------------

        Catch ex As Exception
            Log_Errori &= "- caricamento dataset: " & vbCrLf & MessaggioCompletoDataEccezione(ex, True, source:=True) & vbCrLf
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

        '--------------------------------------------
        ' AGGANCIO DATASET AL REPORT
        '--------------------------------------------
        Try
            '-----------------------------------------------------------------------------
            rptBilancio_PianoNutrizionale.SetDataSource(dsPianoNutrizionale)
            rptBilancio_PianoNutrizionale.OpenSubreport("FooterLogo.rpt").SetDataSource(DS_LogoFooter)
            '-----------------------------------------------------------------------------
        Catch ex As Exception
            Log_Errori &= "- Aggancio dataset al report: " & vbCrLf & MessaggioCompletoDataEccezione(ex, True, source:=True) & vbCrLf
        End Try


    End Sub



    '##########################################################################################################################################
    Private Shared Function GetListaPrecessioniDes(ByVal regolamentoCod As Integer, ByVal puaTipo As Integer) As List(Of Precessione)

        Dim objParametriIngresso As New PianoConcimazione_Precessione_input With {
            .Regolamento_Cod = regolamentoCod,
            .PUA_Tipo = puaTipo
        }

        Dim objParametriUscita As New PianoConcimazione_Precessione_output
        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
        objParametriUscita = objPC_WS.Precessione(objParametriIngresso)

        Return objParametriUscita.ListaPrecessione

    End Function
    Private Shared Function GetPrecessioneDes(ByRef listaPrecessioni As List(Of Precessione), ByVal precessioneCod As Integer) As String

        Dim descrizione As String = ""

        If Not listaPrecessioni Is Nothing AndAlso listaPrecessioni.Count > 0 Then
            Dim obj = (From l In listaPrecessioni Where l.Codice = precessioneCod Select l).FirstOrDefault()

            If Not obj Is Nothing Then
                descrizione = obj.Descrizione
            End If
        End If

        Return descrizione

    End Function

    Private Shared Function GetListaUbicazioniDes(ByVal regolamentoCod As Integer) As List(Of Ubicazione)

        Dim objParametriIngresso As New PianoConcimazione_Ubicazione_input With {
            .Regolamento_Cod = regolamentoCod,
            .Url = ""
        }

        Dim objParametriUscita As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Ubicazione_output
        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
        objParametriUscita = objPC_WS.Ubicazione(objParametriIngresso)

        Return objParametriUscita.ListaUbicazione

    End Function
    Private Shared Function GetUbicazioneDes(ByRef listaUbicazioni As List(Of Ubicazione), ByVal ubicazioneCod As Integer) As String

        Dim descrizione As String = ""

        If Not listaUbicazioni Is Nothing AndAlso listaUbicazioni.Count > 0 Then
            Dim obj = (From l In listaUbicazioni Where l.Codice = ubicazioneCod Select l).FirstOrDefault()

            If Not obj Is Nothing Then
                descrizione = obj.Descrizione
            End If
        End If

        Return descrizione

    End Function

    Private Shared Function GetListaFertilizzantiDes(ByVal regolamentoCod As Integer, ByVal MatriceOrganica As Integer) As List(Of MatriciOrganiche)

        Dim objParametriIngresso As New PianoConcimazione_MatriciOrganiche_input With {
            .Regolamento_Cod = regolamentoCod,
            .Codice = MatriceOrganica
        }

        Dim objParametriUscita As New PianoConcimazione_MatriciOrganiche_output
        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
        objParametriUscita = objPC_WS.MatriciOrganiche(objParametriIngresso)

        Return objParametriUscita.ListaMatriciOrganiche

    End Function
    Private Shared Function GetFertilizzanteDes(ByRef listaFertilizzanti As List(Of MatriciOrganiche), ByVal MatriceOrganica As Integer) As String

        Dim descrizione As String = ""

        If Not listaFertilizzanti Is Nothing AndAlso listaFertilizzanti.Count > 0 Then
            Dim obj = (From l In listaFertilizzanti Where l.Codice = MatriceOrganica Select l).FirstOrDefault()

            If Not obj Is Nothing Then
                descrizione = obj.Descrizione
            End If
        End If

        Return descrizione

    End Function

End Class