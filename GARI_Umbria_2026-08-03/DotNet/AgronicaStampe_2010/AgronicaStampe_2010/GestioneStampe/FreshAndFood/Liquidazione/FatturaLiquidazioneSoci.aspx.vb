Imports AgronicaCoreContabDAL.FF_LiquidazioneSoci_R
Imports AgronicaCoreDataProvider.Agro_Math
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreMetaSchemaDAL
Imports CrystalDecisions.CrystalReports


Public Class FatturaLiquidazioneSoci
    Inherits System.Web.UI.Page

    'Private _rptStampa As CR_FatturaLiquidazioneSoci
    Private _rptStampa As Engine.ReportDocument
    Private _logErrori As String = ""
    Private _catCod As Integer

    Private _objParametriServer As New AgronicaCoreDataProvider.AgronicaCoreParametri

    Private _nomeFileReport As String = ""
    Private _pathRpt As String = "~/GestioneStampe/FreshAndFood/Liquidazione/Report"
    Private _nomeDocumento As String '= "FatturaLiquidazioneSoci"

    Private _piva As String
    Private _report As enum_CodificaStampe
    Private _idAccontoLiquidazione As Integer
    Private _fornitoreFiltro As String
    Private _arrCodRisUm() As Integer


    '####################################################################
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        '#################################################################################
        '#####  Prendo parametri da Query String
        '#################################################################################

        _piva = Stringa_Decodifica(CStr(Request.QueryString("p")), AgroKey_EncoderDecoder)
        _report = CInt(Stringa_Decodifica(CStr(Request.QueryString("rep")), AgroKey_EncoderDecoder))
        _idAccontoLiquidazione = Stringa_Decodifica(CStr(Request.QueryString("liq")), AgroKey_EncoderDecoder)

        'Parametro _fornitoreFiltro -> stringa formattata con un array di valori Cod_RisUm
        _fornitoreFiltro = Stringa_Decodifica(CStr(Request.QueryString("f")), AgroKey_EncoderDecoder)

        Dim arrStrCodRisUm() As String = _fornitoreFiltro.Split(New Char() {"|"c}, StringSplitOptions.RemoveEmptyEntries)
        _arrCodRisUm = Array.ConvertAll(arrStrCodRisUm, Function(str) Integer.Parse(str))

        _objParametriServer = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))


        Select Case _report

            Case enum_CodificaStampe.FreshFood_FatturaLiquidazioneSoci

                _nomeDocumento = "FatturaLiquidazioneSoci"
                _catCod = enum_CategorieDocumenti.FF_FatturaSoci_Liquidazione

            Case enum_CodificaStampe.FreshFood_AutofatturaLiquidazioneSoci

                _nomeDocumento = "AutofatturaLiquidazioneSoci"
                _catCod = enum_CategorieDocumenti.FF_AutofatturaSoci_Liquidazione

        End Select


        '#################################################################################
        '#####  Carico il report da File (no Risorsa incorporata, ma Contenuto)
        '#################################################################################
        Dim crHlp As New CrystalHelper

        Select Case _objParametriServer.PivaSuperUser
            Case "01271980391"
                ' FRUTTAGL  
                _nomeFileReport = "CR_FatturaLiquidazioneSoci_FRUTTAGEL_FF.rpt"
            Case "00040710295"
                ' COFRUTA
                _nomeFileReport = "CR_FatturaLiquidazioneSoci.rpt"
            Case Else
                _nomeFileReport = "CR_FatturaLiquidazioneSoci.rpt"
        End Select

        'lettura da file system
        If _nomeFileReport <> "" Then

            Dim pFile As String = HttpContext.Current.Server.MapPath(_pathRpt)
            pFile = AgronicaCoreUtility.FileSystemHelper.AggiungiSlashSeNonEsiste(pFile)

            If My.Computer.FileSystem.FileExists(pFile & _nomeFileReport) Then
                _rptStampa = crHlp.getReportDaFile(pFile, _nomeFileReport)
            End If

        End If




        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        '_rptStampa = New CR_FatturaLiquidazioneSoci

        If Not IsPostBack Then

            Dim dsFatturaLiquidazioneSoci As New DS_FatturaLiquidazioneSoci
            Dim identificazioneDocumento As String = _nomeDocumento + "_" & DateTime.Now().ToString("yyyy_MM_dd_hhmmssff")

            Try

                Stampa_FatturaLiquidazione(dsFatturaLiquidazioneSoci)

            Catch ex As Exception
                Dim messaggioErrore = "- Stampa: " & ex.Message & If(Not IsNothing(ex.InnerException), " [" & ex.InnerException.Message & "]", "")
                _logErrori &= messaggioErrore & vbCrLf
                SalvaLogErrori(_logErrori, _nomeDocumento, identificazioneDocumento)
                _logErrori = ""
                Throw New Exception("[FatturaLiquidazioneSoci] : " & messaggioErrore)
            End Try


            Dim pathCompletoPdf As String
            Dim objCatDoc As New CategorieDocumenti_R

            Try
                Dim nomeFile As String = AgronicaCoreUtility.Stringhe.EliminaCaratteriSpecialiFile(identificazioneDocumento) & ".pdf"

                ' leggo la sottocartella da CategorieDocumenti
                Dim sottocartella As String = objCatDoc.Sottocartella(_catCod, "", "", _objParametriServer)

                ' salvo il report in formato PDF
                Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
                pathCompletoPdf = objGestFile.SalvaReportDocumentPdf(_rptStampa,
                                                                     _catCod,
                                                                     sottocartella,
                                                                     nomeFile,
                                                                     _objParametriServer,
                                                                     New AgronicaCoreGestioneRichieste.AgroWebConfig)

                'Dim objAllegati As New AgronicaCoreScadenziario_BIZ.Allegati
                'Dim AllegatiDocumentiCod As Integer

                'Dim Data_Inizio_Allegato As Date = Now
                'Dim Data_Fine_Allegato As Date = AGRODATAFINE

                'AllegatiDocumentiCod = objAllegati.SalvaAllegato(_piva, _
                '                                                 CatCod, _
                '                                                 "FatturaLiquidazioneSoci", _
                '                                                 NomeFile, _
                '                                                 Sottocartella, _
                '                                                 "", "", "", "", _
                '                                                 Data_Inizio_Allegato, _
                '                                                 Data_Fine_Allegato, _
                '                                                 _objParametriServer)

            Catch ex As Exception
                _logErrori += "- Gestione allegati: " + vbCrLf + ex.Message + vbCrLf
            End Try


            SalvaLogErrori(_logErrori, _nomeDocumento, identificazioneDocumento)
            _logErrori = ""


            'Dispose del report per evitare problema deallocazione.
            dsFatturaLiquidazioneSoci.Dispose()
            'dsFatturaLiquidazioneSoci = Nothing

            _rptStampa.Close()
            _rptStampa.Dispose()
            _rptStampa = Nothing

            GC.Collect()

            If pathCompletoPdf <> "" Then
                Response.Redirect("..\..\VisualizzatoreReport.aspx?pdf=" & Stringa_Codifica(pathCompletoPdf, AgroKey_EncoderDecoder))
            End If

            SalvaLogErrori(_logErrori, _nomeDocumento, identificazioneDocumento)
            _logErrori = ""

        End If

    End Sub

    '#####################################################################
    Private Sub Stampa_FatturaLiquidazione(ByRef dsFattura As DS_FatturaLiquidazioneSoci)

        Const nomeRoutine = "FatturaLiquidazioneSoci.Stampa_FatturaLiquidazione()"
        Dim messaggioErrore As String
        Dim sezione As String

        Dim drIntestazioneNew As DS_FatturaLiquidazioneSoci.DT_IntestazioneRow


        Try

            '############################################################################################
            '############################### INTESTAZIONE DELLA FATTURA #################################
            '############################################################################################
            sezione = "Lettura dei dati dell'intestazione dell'impresa: "

            Dim intestPivaCf As String = ""
            Dim intestCodici As String = ""
            Dim intestIscrizioneAlbo As String = ""
            Dim intestSedeLegale As String = ""
            Dim intestIndSedelegale As String = ""
            Dim intestTelFaxCell As String = ""
            Dim intestEmailSito As String = ""
            Dim intestPec As String = ""
            Dim intestSedeOperativa As String = ""
            Dim intestIndsedeOperativa As String = ""
            Dim intesttelFaxCellSedeOper As String = ""
            Dim intestSocialNetwork As String = ""
            Dim intestJolly1 As String = ""
            Dim intestJolly2 As String = ""

            Dim xRagSocImpresa As String = ""
            Dim xCodiceFiscaleImpresa As String = ""
            Dim xIndDesImpresa As String = ""
            Dim xFrzDesImpresa As String = ""
            Dim xCapImpresa As String = ""
            Dim xComuneImpresa As String = ""
            Dim xProvinciaImpresa As String = ""
            Dim xStatoImpresa As String = ""

            Dim xIdCfCliente As Integer = 0

            Leggi_Intestazione_Impresa_2(_objParametriServer,
                                         Progressivo_GIAS:=CInt(Session("ASG_ProgressivoGIAS")),
                                         Flag_DOCO:=False,
                                         Flag_DAA:=False,
                                         x_Id_Cf_Cliente:=xIdCfCliente,
                                         Log_Errori:=_logErrori,
                                         x_Piva:=_piva,
                                         x_CodiceFiscale_Impresa:=xCodiceFiscaleImpresa,
                                         x_RagSoc_Impresa:=xRagSocImpresa,
                                         x_IndDes_Impresa:=xIndDesImpresa,
                                         x_FrzDes_Impresa:=xFrzDesImpresa,
                                         x_Cap_Impresa:=xCapImpresa,
                                         x_Comune_Impresa:=xComuneImpresa,
                                         x_Provincia_Impresa:=xProvinciaImpresa,
                                         x_Stato_Impresa:=xStatoImpresa,
                                         Intestazione_Riga3:=intestCodici,
                                         Intestazione_Riga4:=intestIscrizioneAlbo,
                                         Intestazione_Riga5:=intestSedeLegale,
                                         Intestazione_Riga6:=intestIndSedelegale,
                                         Intestazione_Riga7:=intestTelFaxCell,
                                         Intestazione_Riga8:=intestEmailSito,
                                         Intestazione_Riga9:=intestPec,
                                         Intestazione_Riga10:=intestSedeOperativa,
                                         Intestazione_Riga11:=intestIndsedeOperativa,
                                         Intestazione_Riga12:=intesttelFaxCellSedeOper,
                                         Intestazione_Riga13:=intestSocialNetwork,
                                         Intestazione_Riga14:=intestJolly1,
                                         Intestazione_Riga15:=intestJolly2,
                                         False, Nothing, "", False)

            If xIdCfCliente = enum_Contatti_IdCf.ContattoEstero Then
                intestPivaCf = "VAT: IT" + _piva
            Else
                intestPivaCf = "Partita IVA: " + _piva
            End If
            If xCodiceFiscaleImpresa <> "" Then
                intestPivaCf &= "   Codice Fiscale: " + xCodiceFiscaleImpresa
            End If

            '==============================================
            '====== QUERY E CARICAMENTO DATASET ===========
            '==============================================

            Dim objIvaAliquoteR As New IVA_Aliquote_R

            Dim objLiquidazioneSociR As New AgronicaCoreContabDAL.FF_LiquidazioneSoci_R

            Dim siglaIva As String = ""
            Dim aliquotaIva As Decimal = 0

            Dim listaMovDettagliCollegati As New List(Of Movimenti_dettagli)
            Dim listTestate = objLiquidazioneSociR.Leggi_DatiLiquidazione(_piva, _idAccontoLiquidazione, FiltroTipoFattura(),
                                                                          _arrCodRisUm, Nothing, _objParametriServer,
                                                                          listaMovDettagliCollegati)
            Dim listAcconti = objLiquidazioneSociR.Leggi_TrattenuteLiquidazione(_piva, _idAccontoLiquidazione, 1,
                                                                                _arrCodRisUm, Nothing, _objParametriServer)

            Dim contatoreDettaglio As Integer = 0
            Dim contatoreAcconto As Integer = 0

            For Each testata In listTestate
                sezione = "Testata: "

                Dim sommaAcconti As Decimal = 0
                Dim sommaDettagli As Decimal = 0
                Dim sommaIva As Decimal = 0
                Dim codIva As Integer = 0

                Dim codRisUm As Integer = testata.Cod_RisUm
                Dim cedenteObj As IntestazioneObj

                Dim contattoTrovato As Boolean = objLiquidazioneSociR.IntestazioneDoc(codRisUm,
                                                                                      cedenteObj,
                                                                                      _objParametriServer)

                If contattoTrovato = False Then
                    Throw New Exception(String.Format("Il Cod_RisUm {0} non ha il contatto o gli indirizzi correttamente valorizzati", codRisUm))
                End If

                drIntestazioneNew = dsFattura.DT_Intestazione.NewDT_IntestazioneRow()

                drIntestazioneNew.Piva_Acquirente = testata.PIVA
                drIntestazioneNew.CF_Acquirente = xCodiceFiscaleImpresa
                drIntestazioneNew.Rag_Soc_Acquirente = xRagSocImpresa

                drIntestazioneNew.Cod_RisUm_Cedente = CStr(codRisUm)

                drIntestazioneNew.Piva_Cedente = cedenteObj.Cod_Contatto
                drIntestazioneNew.CF_Cedente = cedenteObj.Codice_Fiscale
                drIntestazioneNew.Rag_Soc_Cedente = cedenteObj.Chi()
                drIntestazioneNew.CAP_Cedente = cedenteObj.CAP
                drIntestazioneNew.Comune_Cedente = cedenteObj.Dove()
                drIntestazioneNew.Indirizzo_Cedente = cedenteObj.Ind_Des
                drIntestazioneNew.Prov_Cedente = cedenteObj.Pro_Cod
                drIntestazioneNew.Stato_Cedente = cedenteObj.Stato


                If IsNumeric(testata.Doc_Numero) AndAlso CInt(testata.Doc_Numero) <> 0 Then
                    drIntestazioneNew.Doc_Numero_Sin = testata.Doc_Numero_Sin
                    drIntestazioneNew.Doc_Numero = CInt(testata.Doc_Numero)
                    drIntestazioneNew.Doc_Numero_Des = testata.Doc_Numero_Des

                    drIntestazioneNew.Doc_Numero_Totale = testata.Doc_Numero_Sin & CStr(testata.Doc_Numero) & testata.Doc_Numero_Des

                    If Not IsNothing(testata.Data_Documento) Then
                        drIntestazioneNew.Data_Doc = testata.Data_Documento
                    End If
                End If


                sezione = "Raggruppa Dettaglio: "
                Dim listaDettagli = testata.Liquid_Mov_CampionamentoConferito

                'Prima prendo il prezzo_effettivo dalla movimenti_dettagli, se questo è zero,
                'allora prendo quello da Liquid_Mov_CampionamentoConferito
                'Questo solo quando sto stampando una liquidazione

                Dim listaDettagliGroup = (From t In listaDettagli
                                        Group Join m In listaMovDettagliCollegati
                                            On t.PIVA Equals m.PIVA And t.Id_Mov_Det Equals m.Id_Mov_Det
                                            Into m_group = Group
                                        From _m In m_group.DefaultIfEmpty()
                                        Group By x = New With {
                                            Key t.Grp_Fatt_Cod,
                                            Key t.Grp_Fatt_Descr,
                                            Key t.Qual_Cod,
                                            Key t.Qual_Sigla,
                                            Key t.Certif_Cod,
                                            Key t.Certif_Sigla,
                                            Key t.Cod_Iva
                                        } Into g = Group
                                        Order By x.Grp_Fatt_Descr, x.Qual_Sigla & " " & x.Certif_Sigla
                                        Select New With {
                                            .GrpFattCod = x.Grp_Fatt_Cod,
                                            .GrpFattDescr = x.Grp_Fatt_Descr,
                                            .QualCod = x.Qual_Cod,
                                            .QualSigla = x.Qual_Sigla,
                                            .CertifCod = x.Certif_Cod,
                                            .CertifSigla = x.Certif_Sigla,
                                            .CodIva = x.Cod_Iva,
                                            .KgNettiTot = g.Sum(Function(r) r.t.KgNetti),
                                            .ImponibileTot = g.Sum(Function(r) If((r._m Is Nothing OrElse r._m.Imponibile_Netto Is Nothing OrElse CDec(r._m.Imponibile_Netto) = 0D),
                                                                                  r.t.Imponibile,
                                                                                  CDec(-1 * r._m.Imponibile_Netto))),
                                            .IvaTot = g.Sum(Function(r) If((r._m Is Nothing OrElse r._m.Iva Is Nothing OrElse CDec(r._m.Iva) = 0D),
                                                                           r.t.Iva,
                                                                           CDec(r._m.Iva)))
                                        }).Tolist

                'Elimino le voci con imponibile totale 0
                listaDettagliGroup.RemoveAll(Function(x) x.ImponibileTot = 0)

                For Each dettaglio In listaDettagliGroup
                    sezione = "Dettaglio: "

                    contatoreDettaglio += 1

                    CreaRigaDettaglio(dsFattura,
                                      contatoreDettaglio,
                                      descrizione:=Trim(dettaglio.QualSigla & " " & dettaglio.CertifSigla),
                                      kgConferiti:=ArrotondaVal_2(dettaglio.KgNettiTot),
                                      importo:=dettaglio.ImponibileTot,
                                      idIntestazione:=CStr(codRisUm),
                                      gruppoFattCod:=dettaglio.GrpFattCod,
                                      gruppoFattDes:=dettaglio.GrpFattDescr,
                                      sommaDettagli:=sommaDettagli)

                    If codIva = 0 Then
                        codIva = dettaglio.CodIva
                    End If

                    sommaIva += dettaglio.IvaTot
                Next

                Dim listAccontiRisUm As New List(Of Liquid_Mov_Trattenute_CampionamentoConferito)
                If Not IsNothing(listAcconti) AndAlso listAcconti.Count > 0 Then
                    listAccontiRisUm = listAcconti.FindAll(Function(x) x.Cod_RisUm = codRisUm)
                End If

                For Each acconto In listAccontiRisUm
                    sezione = "Acconto: "

                    contatoreAcconto += 1

                    CreaRigaAcconto(dsFattura,
                                    contatoreAcconto,
                                    acconto.Doc_Numero_Sin,
                                    CInt(acconto.Doc_Numero),
                                    acconto.Doc_Numero_Des,
                                    acconto.Data_Movimento,
                                    acconto.Riferimento,
                                    valore:=Decimal.Negate(acconto.Imponibile),
                                    idIntestazione:=CStr(codRisUm),
                                    sommaAcconti:=sommaAcconti)

                Next

                '  Giulia, 30/11/2017 11:02:52: per il momento è sempre 4, quindi la descrizione la pesco solo la prima volta
                '       se si dovesse cambiare, basta tirare fuori la riga dall'if
                If siglaIva = "" Then
                    siglaIva = objIvaAliquoteR.Aliquota_from_CodIVA(codIva, "", _objParametriServer)
                    aliquotaIva = objIvaAliquoteR.AliquotaFloat_from_CodIVA(codIva, "", _objParametriServer)
                End If

                sezione = "Totali Fattura: "
                drIntestazioneNew.Imponibile_Pre_Acconti = sommaDettagli
                'qui sommo perché tanto gli acconti saranno tutti negativi, quindi di fatto sto sottraendo
                drIntestazioneNew.Imponibile_Post_Acconti = drIntestazioneNew.Imponibile_Pre_Acconti + sommaAcconti
                drIntestazioneNew.Descr_Iva = "IMPONIBILE " & siglaIva

                '  Giulia, 06/12/2017 12:33:40: per il momento viene calcolato qui secco il 4%
                'drIntestazioneNew.Valore_Iva = sommaIva
                drIntestazioneNew.Valore_Iva = ArrotondaVal_2(drIntestazioneNew.Imponibile_Post_Acconti * aliquotaIva / 100)

                drIntestazioneNew.Totale_Fattura = drIntestazioneNew.Imponibile_Post_Acconti + drIntestazioneNew.Valore_Iva

                dsFattura.DT_Intestazione.Rows.Add(drIntestazioneNew)

            Next

            'imposto visibilità sulle sezioni per fattura/autofattura
            ImpostaVisibilitaSezioni()

            'imposto il dataset sul report
            sezione = "Aggancio dataset: "
            _rptStampa.SetDataSource(dsFattura)

            sezione = "Impostazione parametri: "
            _rptStampa.SetParameterValue("Intest_Sede_Legale_Ind1", intestSedeLegale)
            _rptStampa.SetParameterValue("Intest_Sede_Legale_Ind2", intestIndSedelegale)
            _rptStampa.SetParameterValue("Intest_Tel_Fax_Cell", intestTelFaxCell)
            _rptStampa.SetParameterValue("Intest_Pec", intestPec)
            _rptStampa.SetParameterValue("Intest_Piva_Cf", intestPivaCf)



        Catch ex As Exception
            messaggioErrore = sezione & " " & ex.Message & If(Not IsNothing(ex.InnerException), " [" & ex.InnerException.Message & "]", "")
            _logErrori &= messaggioErrore & vbCrLf
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore & vbCrLf)
        End Try

    End Sub

    '###################################################
    Private Function FiltroTipoFattura() As String
        Dim messaggioErrore As String
        Const nomeRoutine = "FatturaLiquidazioneSoci.FiltroTipoFattura()"

        Try
            Select Case _report

                Case enum_CodificaStampe.FreshFood_FatturaLiquidazioneSoci

                    Return "F"

                Case enum_CodificaStampe.FreshFood_AutofatturaLiquidazioneSoci

                    Return "A"

            End Select

        Catch ex As Exception
            messaggioErrore = ex.Message & If(Not IsNothing(ex.InnerException), " [" & ex.InnerException.Message & "]", "")
            _logErrori &= messaggioErrore & vbCrLf
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore & vbCrLf)
        End Try

        Return ""

    End Function

    '###################################################
    Private Sub ImpostaVisibilitaSezioni()
        Dim messaggioErrore As String
        Const nomeRoutine = "FatturaLiquidazioneSoci.ImpostaVisibilitaSezioni()"

        Try
            'Prima le nascondo tutte
            _rptStampa.ReportDefinition.Sections("PageHeaderSection1").SectionFormat.EnableSuppress = True
            _rptStampa.ReportDefinition.Sections("PageHeaderSection2").SectionFormat.EnableSuppress = True
            _rptStampa.ReportDefinition.Sections("PageFooterSection1").SectionFormat.EnableSuppress = True
            _rptStampa.ReportDefinition.Sections("PageFooterSection2").SectionFormat.EnableSuppress = True
            _rptStampa.Subreports("CR_FatturaLiqDettaglioSub").ReportDefinition.Sections("ReportHeaderSection1").SectionFormat.EnableSuppress = True

            'Rendo visibili solo quelle giuste
            Select Case _report

                Case enum_CodificaStampe.FreshFood_FatturaLiquidazioneSoci

                    _rptStampa.ReportDefinition.Sections("PageHeaderSection1").SectionFormat.EnableSuppress = False
                    _rptStampa.ReportDefinition.Sections("PageFooterSection1").SectionFormat.EnableSuppress = False

                Case enum_CodificaStampe.FreshFood_AutofatturaLiquidazioneSoci

                    _rptStampa.ReportDefinition.Sections("PageHeaderSection2").SectionFormat.EnableSuppress = False
                    _rptStampa.ReportDefinition.Sections("PageFooterSection2").SectionFormat.EnableSuppress = False
                    _rptStampa.Subreports("CR_FatturaLiqDettaglioSub").ReportDefinition.Sections("ReportHeaderSection1").SectionFormat.EnableSuppress = False

            End Select

        Catch ex As Exception
            messaggioErrore = ex.Message & If(Not IsNothing(ex.InnerException), " [" & ex.InnerException.Message & "]", "")
            _logErrori &= messaggioErrore & vbCrLf
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore & vbCrLf)
        End Try

    End Sub

    '###################################################
    Private Sub CreaRigaDettaglio(ByRef dsFattura As DS_FatturaLiquidazioneSoci,
                                  ByVal contatore As Integer,
                                  ByVal descrizione As String,
                                  ByVal kgConferiti As Decimal,
                                  ByVal importo As Decimal,
                                  ByVal idIntestazione As String,
                                  ByVal gruppoFattCod As Integer,
                                  ByVal gruppoFattDes As String,
                                  ByRef sommaDettagli As Decimal)

        Dim messaggioErrore As String
        Const nomeRoutine = "FatturaLiquidazioneSoci.CreaRigaDettaglio()"

        Dim drDettaglioNew As DS_FatturaLiquidazioneSoci.DT_DettaglioRow

        Try
            drDettaglioNew = dsFattura.DT_Dettaglio.NewDT_DettaglioRow()

            drDettaglioNew.Contatore = contatore
            drDettaglioNew.Descrizione = descrizione
            drDettaglioNew.KG_Conferiti = kgConferiti
            drDettaglioNew.Importo_Complessivo = importo
            drDettaglioNew.Id_Intestazione = idIntestazione
            drDettaglioNew.GruppoFatt_Cod = gruppoFattCod
            drDettaglioNew.GruppoFatt_Des = gruppoFattDes

            sommaDettagli += importo

            dsFattura.DT_Dettaglio.Rows.Add(drDettaglioNew)

        Catch ex As Exception
            messaggioErrore = ex.Message & If(Not IsNothing(ex.InnerException), " [" & ex.InnerException.Message & "]", "")
            _logErrori &= messaggioErrore & vbCrLf
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore & vbCrLf)
        End Try

    End Sub

    '###################################################
    Private Sub CreaRigaAcconto(ByRef dsFattura As DS_FatturaLiquidazioneSoci,
                                ByVal contatore As Integer,
                                ByVal docNumeroSin As String,
                                ByVal docNumero As Integer,
                                ByVal docNumeroDes As String,
                                ByVal dataMovimento As Date,
                                ByVal riferimento As String,
                                ByVal valore As Decimal,
                                ByVal idIntestazione As String,
                                ByRef sommaAcconti As Decimal)

        Dim messaggioErrore As String
        Const nomeRoutine = "FatturaLiquidazioneSoci.CreaRigaAcconto()"

        Dim drAccontoNew As DS_FatturaLiquidazioneSoci.DT_AccontiRow

        Try
            drAccontoNew = dsFattura.DT_Acconti.NewDT_AccontiRow()

            drAccontoNew.Contatore = contatore
            drAccontoNew.Descrizione = Trim(If(docNumero <> 0, "Fatt " & docNumeroSin & CStr(docNumero) & docNumeroDes & " del " & dataMovimento.ToShortDateString() & " ", "") & riferimento)
            drAccontoNew.Valore = valore
            drAccontoNew.Id_Intestazione = idIntestazione

            sommaAcconti += valore

            dsFattura.DT_Acconti.Rows.Add(drAccontoNew)

        Catch ex As Exception
            messaggioErrore = ex.Message & If(Not IsNothing(ex.InnerException), " [" & ex.InnerException.Message & "]", "")
            _logErrori &= messaggioErrore & vbCrLf
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore & vbCrLf)
        End Try

    End Sub

    Private Sub SalvaLogErrori(ByVal logErrori As String, ByVal nomeDocumento As String, ByVal identificazioneDocumento As String)
        '-----------------------------------------
        '---- Salvataggio Log Errori -------------
        '-----------------------------------------
        Dim nomeFile As String

        If logErrori <> "" Then

            logErrori = nomeDocumento & vbCrLf & vbCrLf & logErrori

            nomeFile = "Log_Errori_" & identificazioneDocumento & CStr(Session("ASG_Utente_Username"))

            Dim objLog As New GestioneLogStampe
            objLog.Gestione_LogErrori_Stampe(_objParametriServer, _
                                             "Stampe_FreshAndFood", _
                                             nomeFile & ".txt", _
                                             Session("ASG_Utente_Username"), _
                                             "FatturaLiquidazioneSoci.aspx", _
                                             logErrori)
        End If

    End Sub

End Class