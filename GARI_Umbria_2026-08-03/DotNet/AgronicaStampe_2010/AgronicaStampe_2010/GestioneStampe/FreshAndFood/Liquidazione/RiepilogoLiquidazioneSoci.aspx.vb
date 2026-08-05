Imports AgronicaCoreContabDAL
Imports AgronicaCoreContabDAL.FF_LiquidazioneSoci_R
Imports AgronicaCoreDataProvider.Agro_Math
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreMetaSchemaDAL
Imports CrystalDecisions.CrystalReports
Imports Newtonsoft.Json


Public Class RiepilogoLiquidazioneSoci
    Inherits System.Web.UI.Page

    Private _rptStampa As Engine.ReportDocument
    Private _logErrori As String = ""
    Private _catCod As Integer

    Private _objParametriServer As New AgronicaCoreDataProvider.AgronicaCoreParametri

    Private _nomeFileReport As String = ""
    Private _pathRpt As String = "~/GestioneStampe/FreshAndFood/Liquidazione/Report"
    Private _nomeDocumento As String

    Private _piva As String
    Private _idAccontoLiquidazione As Integer
    Private _fornitoreFiltro As String
    Private _arrCodRisUm() As Integer


    '####################################################################
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        '#################################################################################
        '#####  Prendo parametri da Query String
        '#################################################################################

        _piva = Stringa_Decodifica(CStr(Request.QueryString("p")), AgroKey_EncoderDecoder)
        _idAccontoLiquidazione = Stringa_Decodifica(CStr(Request.QueryString("liq")), AgroKey_EncoderDecoder)

        'Parametro _fornitoreFiltro -> stringa formattata con un array di valori Cod_RisUm
        _fornitoreFiltro = Stringa_Decodifica(CStr(Request.QueryString("f")), AgroKey_EncoderDecoder)

        Dim arrStrCodRisUm() As String = _fornitoreFiltro.Split(New Char() {"|"c}, StringSplitOptions.RemoveEmptyEntries)
        _arrCodRisUm = Array.ConvertAll(arrStrCodRisUm, Function(str) Integer.Parse(str))

        _objParametriServer = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))


        _nomeDocumento = "RiepilogoLiquidazioneSoci"
        _catCod = enum_CategorieDocumenti.FF_RiepilogoSoci_Liquidazione


        '#################################################################################
        '#####  Carico il report da File (no Risorsa incorporata, ma Contenuto)
        '#################################################################################
        Dim crHlp As New CrystalHelper


        Select Case _objParametriServer.PivaSuperUser
            Case "01271980391"
                ' FRUTTAGL  
                _nomeFileReport = "CR_RiepilogoLiquidazioneSoci_FRUTTAGEL_FF.rpt"
            Case "00040710295"
                ' COFRUTA
                _nomeFileReport = "CR_RiepilogoLiquidazioneSoci.rpt"
            Case Else
                _nomeFileReport = "CR_RiepilogoLiquidazioneSoci.rpt"
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

        If Not IsPostBack Then

            Dim dsRiepilogoLiquidazioneSoci As New DS_RiepilogoLiquidazioneSoci
            Dim identificazioneDocumento As String = _nomeDocumento + "_" & DateTime.Now().ToString("yyyy_MM_dd_hhmmssff")

            Try

                Stampa_RiepilogoLiquidazione(dsRiepilogoLiquidazioneSoci)

            Catch ex As Exception
                Dim messaggioErrore = "- Stampa: " & ex.Message & If(Not IsNothing(ex.InnerException), " [" & ex.InnerException.Message & "]", "")
                _logErrori &= messaggioErrore & vbCrLf
                SalvaLogErrori(_logErrori, _nomeDocumento, identificazioneDocumento)
                _logErrori = ""
                Throw New Exception("[RiepilogoLiquidazioneSoci] : " & messaggioErrore)
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
                _logErrori &= "- Gestione allegati: " & vbCrLf & ex.Message & vbCrLf
            End Try


            SalvaLogErrori(_logErrori, _nomeDocumento, identificazioneDocumento)
            _logErrori = ""


            'Dispose del report per evitare problema deallocazione.
            dsRiepilogoLiquidazioneSoci.Dispose()
            'dsFatturaLiquidazioneSoci = Nothing

            _rptStampa.Close()
            _rptStampa.Dispose()
            _rptStampa = Nothing

            GC.Collect()

            If pathCompletoPdf <> "" Then
                Response.Redirect("..\..\VisualizzatoreReport.aspx?pdf=" + Stringa_Codifica(pathCompletoPdf, AgroKey_EncoderDecoder))
            End If

            SalvaLogErrori(_logErrori, _nomeDocumento, identificazioneDocumento)
            _logErrori = ""

        End If

    End Sub

    '#####################################################################
    Private Sub Stampa_RiepilogoLiquidazione(ByRef dsRiepilogo As DS_RiepilogoLiquidazioneSoci)

        Const nomeRoutine = "RiepilogoLiquidazioneSoci.Stampa_RiepilogoLiquidazione()"
        Dim messaggioErrore As String
        Dim sezione As String

        Dim drIntestazioneNew As DS_RiepilogoLiquidazioneSoci.DT_IntestazioneRow
        
        Try

            Dim objCampioConfeR As New FF_CampionamentoConferimento_R
            Dim strAnagAccLiq = objCampioConfeR.Leggi_Elem_AnagAccontiLiquidazioni(_piva, _idAccontoLiquidazione, _objParametriServer)
            Dim serializerSettings As New JsonSerializerSettings() With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            Dim objAnagAccLiq = JsonConvert.DeserializeObject(Of AnagAccontiLiquidazioni_CampionamentoConferito)(strAnagAccLiq, serializerSettings)



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

            Dim objLiquidazioneSociR As New FF_LiquidazioneSoci_R
            'Dim dtLiquidazioni As New DataTable
            'Dim dtTrattenute As New DataTable

            Dim siglaIva As String = ""
            Dim aliquotaIva As Decimal = 0

            Dim listaMovDettagliCollegati As New List(Of Movimenti_dettagli)
            Dim listTestate = objLiquidazioneSociR.Leggi_DatiLiquidazione(_piva, _idAccontoLiquidazione, Nothing,
                                                                          _arrCodRisUm, Nothing, _objParametriServer,
                                                                          listaMovDettagliCollegati)
            Dim listAcconti = objLiquidazioneSociR.Leggi_TrattenuteLiquidazione(_piva, _idAccontoLiquidazione, Nothing,
                                                                                _arrCodRisUm, Nothing, _objParametriServer)


            Dim contatoreDettaglio As Integer = 0
            Dim contatoreAcconto As Integer = 0

            For Each testata In listTestate
                sezione = "Testata: "

                Dim sommaAccontiDetrai1 As Decimal = 0
                Dim sommaAccontiDetrai0 As Decimal = 0
                Dim sommaDettagli As Decimal = 0
                Dim sommaIva As Decimal = 0
                Dim codIva As Integer = 0

                Dim codRisUm As Integer = testata.Cod_RisUm
                Dim cedenteObj As IntestazioneObj

                Dim contattoTrovato As Boolean = objLiquidazioneSociR.IntestazioneDoc(codRisUm,
                                                                                      cedenteObj,
                                                                                      _objParametriServer)

                If contattoTrovato = False Then
                    Throw New Exception("Il Cod_RisUm " & CStr(codRisUm) & "non ha il contatto o gli indirizzi correttamente valorizzati")
                End If

                drIntestazioneNew = dsRiepilogo.DT_Intestazione.NewDT_IntestazioneRow()

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
                drIntestazioneNew.Progressivo_Cedente = cedenteObj.Progressivo

                If Not IsNothing(testata.Data_Documento) Then
                    drIntestazioneNew.Data_Doc = testata.Data_Documento
                End If


                sezione = "Raggruppa Dettaglio: "
                Dim listaDettagli = testata.Liquid_Mov_CampionamentoConferito
                Dim listaDettagliGroup = (From t In listaDettagli
                                        Group Join m In listaMovDettagliCollegati
                                            On t.PIVA Equals m.PIVA And t.Id_Mov_Det Equals m.Id_Mov_Det
                                            Into m_group = Group
                                        From _m In m_group.DefaultIfEmpty()
                                        Group By x = New With {
                                            Key t.Grp_Fatt_Cod,
                                            Key t.Grp_Fatt_Descr,
                                            Key t.Cod_Iva
                                        } Into g = Group
                                        Order By x.Grp_Fatt_Descr
                                        Select New With {
                                            .GrpFattCod = x.Grp_Fatt_Cod,
                                            .GrpFattDescr = x.Grp_Fatt_Descr,
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

                    CreaRigaDettaglio(dsRiepilogo,
                                      contatoreDettaglio,
                                      descrizione:=dettaglio.GrpFattDescr,
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
                    listAccontiRisUm = listAcconti.FindAll(Function(x) x.Detrarre_Da_Fattura = 1 AndAlso x.Cod_RisUm = codRisUm)
                End If

                For Each acconto In listAccontiRisUm
                    sezione = "Acconto: "

                    contatoreAcconto += 1

                    CreaRigaAcconto(dsRiepilogo,
                                    contatoreAcconto,
                                    acconto.Doc_Numero_Sin,
                                    CInt(acconto.Doc_Numero),
                                    acconto.Doc_Numero_Des,
                                    acconto.Data_Movimento,
                                    acconto.Riferimento,
                                    valore:=Decimal.Negate(acconto.Imponibile),
                                    detrarreDaFattura:=1,
                                    idIntestazione:=CStr(codRisUm),
                                    sommaAcconti:=sommaAccontiDetrai1)

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
                drIntestazioneNew.Imponibile_Post_Acconti = drIntestazioneNew.Imponibile_Pre_Acconti + sommaAccontiDetrai1
                drIntestazioneNew.Descr_Iva = "IMPONIBILE " & siglaIva

                '  Giulia, 06/12/2017 12:33:40: per il momento viene calcolato qui secco il 4%
                'drIntestazioneNew.Valore_Iva = sommaIva
                drIntestazioneNew.Valore_Iva = ArrotondaVal_2(drIntestazioneNew.Imponibile_Post_Acconti * aliquotaIva / 100)

                drIntestazioneNew.Prezzo_Ivato = drIntestazioneNew.Imponibile_Post_Acconti + drIntestazioneNew.Valore_Iva


                Dim listAccontiRisUmDetrai0 As New List(Of Liquid_Mov_Trattenute_CampionamentoConferito)
                If Not IsNothing(listAcconti) AndAlso listAcconti.Count > 0 Then
                    listAccontiRisUmDetrai0 = listAcconti.FindAll(Function(x) x.Detrarre_Da_Fattura = 0 AndAlso x.Cod_RisUm = codRisUm)
                End If

                For Each acconto In listAccontiRisUmDetrai0
                    sezione = "Acconto Detrai 0: "

                    contatoreAcconto += 1

                    CreaRigaAcconto(dsRiepilogo,
                                    contatoreAcconto,
                                    acconto.Doc_Numero_Sin,
                                    CInt(acconto.Doc_Numero),
                                    acconto.Doc_Numero_Des,
                                    acconto.Data_Movimento,
                                    acconto.Riferimento,
                                    valore:=Decimal.Negate(acconto.Imponibile + acconto.Iva),
                                    detrarreDaFattura:=0,
                                    idIntestazione:=CStr(codRisUm),
                                    sommaAcconti:=sommaAccontiDetrai0)

                Next

                drIntestazioneNew.Totale = drIntestazioneNew.Prezzo_Ivato + sommaAccontiDetrai0

                dsRiepilogo.DT_Intestazione.Rows.Add(drIntestazioneNew)

            Next

            'imposto il dataset sul report
            sezione = "Aggancio dataset: "
            _rptStampa.SetDataSource(dsRiepilogo)

            sezione = "Impostazione parametri: "
            _rptStampa.SetParameterValue("Intest_Sede_Legale_Ind1", intestSedeLegale)
            _rptStampa.SetParameterValue("Intest_Sede_Legale_Ind2", intestIndSedelegale)
            _rptStampa.SetParameterValue("Intest_Tel_Fax_Cell", intestTelFaxCell)
            _rptStampa.SetParameterValue("Intest_Pec", intestPec)
            _rptStampa.SetParameterValue("Intest_Piva_Cf", intestPivaCf)
            _rptStampa.SetParameterValue("Data_Liquidazione", If(IsNothing(objAnagAccLiq.data_documento), "", CType(objAnagAccLiq.data_documento, Date)))
            _rptStampa.SetParameterValue("Titolo_Liquidazione", objAnagAccLiq.descrizione)

        Catch ex As Exception
            messaggioErrore = sezione & " " & ex.Message & If(Not IsNothing(ex.InnerException), " [" & ex.InnerException.Message & "]", "")
            _logErrori &= messaggioErrore & vbCrLf
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore & vbCrLf)
        End Try

    End Sub

    '###################################################
    Private Sub CreaRigaDettaglio(ByRef dsRiepilogo As DS_RiepilogoLiquidazioneSoci,
                                  ByVal contatore As Integer,
                                  ByVal descrizione As String,
                                  ByVal kgConferiti As Decimal,
                                  ByVal importo As Decimal,
                                  ByVal idIntestazione As String,
                                  ByVal gruppoFattCod As Integer,
                                  ByVal gruppoFattDes As String,
                                  ByRef sommaDettagli As Decimal)

        Dim messaggioErrore As String
        Const nomeRoutine = "RiepilogoLiquidazioneSoci.CreaRigaDettaglio()"

        Dim drDettaglioNew As DS_RiepilogoLiquidazioneSoci.DT_DettaglioRow

        Try
            drDettaglioNew = dsRiepilogo.DT_Dettaglio.NewDT_DettaglioRow()

            drDettaglioNew.Contatore = contatore
            drDettaglioNew.Descrizione = descrizione
            drDettaglioNew.KG_Conferiti = kgConferiti
            drDettaglioNew.Importo_Complessivo = importo
            drDettaglioNew.Id_Intestazione = idIntestazione
            drDettaglioNew.GruppoFatt_Cod = gruppoFattCod
            drDettaglioNew.GruppoFatt_Des = gruppoFattDes

            sommaDettagli += importo

            dsRiepilogo.DT_Dettaglio.Rows.Add(drDettaglioNew)

        Catch ex As Exception
            messaggioErrore = ex.Message & If(Not IsNothing(ex.InnerException), " [" & ex.InnerException.Message & "]", "")
            _logErrori &= messaggioErrore & vbCrLf
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore & vbCrLf)
        End Try

    End Sub

    '###################################################
    Private Sub CreaRigaAcconto(ByRef dsRiepilogo As DS_RiepilogoLiquidazioneSoci,
                                ByVal contatore As Integer,
                                ByVal docNumeroSin As String,
                                ByVal docNumero As Integer,
                                ByVal docNumeroDes As String,
                                ByVal dataMovimento As Date,
                                ByVal riferimento As String,
                                ByVal valore As Decimal,
                                ByVal detrarreDaFattura As Integer,
                                ByVal idIntestazione As String,
                                ByRef sommaAcconti As Decimal)

        Dim messaggioErrore As String
        Const nomeRoutine = "RiepilogoLiquidazioneSoci.CreaRigaAcconto()"

        Dim drAccontoNew As DS_RiepilogoLiquidazioneSoci.DT_AccontiRow

        Try
            drAccontoNew = dsRiepilogo.DT_Acconti.NewDT_AccontiRow()

            drAccontoNew.Contatore = contatore
            drAccontoNew.Descrizione = Trim(If(docNumero <> 0, "Fatt " & docNumeroSin & CStr(docNumero) & docNumeroDes & " del " & dataMovimento.ToShortDateString() & " ", "") & riferimento)
            drAccontoNew.Detrarre_Da_Fattura = detrarreDaFattura
            drAccontoNew.Valore = valore
            drAccontoNew.Id_Intestazione = idIntestazione

            sommaAcconti += valore

            dsRiepilogo.DT_Acconti.Rows.Add(drAccontoNew)

        Catch ex As Exception
            messaggioErrore = ex.Message & If(Not IsNothing(ex.InnerException), " [" & ex.InnerException.Message & "]", "")
            _logErrori &= messaggioErrore & vbCrLf
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore & vbCrLf)
        End Try

    End Sub

    Private Sub SalvaLogErrori(logErrori As String, nomeDocumento As String, identificazioneDocumento As String)
        '-----------------------------------------
        '---- Salvataggio Log Errori -------------
        '-----------------------------------------
        Dim nomeFile As String

        If logErrori <> "" Then

            logErrori = nomeDocumento & vbCrLf & vbCrLf & logErrori

            nomeFile = "Log_Errori_" & identificazioneDocumento & CStr(Session("ASG_Utente_Username"))

            Dim objLog As New GestioneLogStampe
            objLog.Gestione_LogErrori_Stampe(_objParametriServer,
                                             "Stampe_FreshAndFood",
                                             nomeFile & ".txt",
                                             Session("ASG_Utente_Username"),
                                             "RiepilogoLiquidazioneSoci.aspx",
                                             logErrori)
        End If

    End Sub

End Class