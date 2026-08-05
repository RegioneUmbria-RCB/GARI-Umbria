Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreContabDAL
Imports AgronicaCoreEntityFramework_POCO
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.IO
Imports CrystalDecisions.CrystalReports



Public Class PagatiSuConferito
    Inherits System.Web.UI.Page

    Private _rptStampa As Engine.ReportDocument

    Private _nomeFileReport As String = ""
    Private _pathRpt As String = "~/GestioneStampe/FreshAndFood/Liquidazione/Report"
    Private _nomeDocumento As String = "PagatiSuConferito"

    Dim _piva As String
    Dim _logErrori As String

    Dim _objParametriServer As New AgronicaCoreDataProvider.AgronicaCoreParametri

    Private Function QueryString_To_IntArray(ByVal qs_name As String) As Integer()

        Dim str As String = Stringa_Decodifica(Request.QueryString(qs_name), AgroKey_EncoderDecoder)
        Dim arr_str() As String = str.Split(New Char() {"|"c}, StringSplitOptions.RemoveEmptyEntries)
        Return Array.ConvertAll(arr_str, Function(s) Int32.Parse(s))

    End Function

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        _piva = Stringa_Decodifica(CStr(Request.QueryString("p")), AgroKey_EncoderDecoder)

        Dim id_liq As Integer = Stringa_Decodifica(Request.QueryString("liq"), AgroKey_EncoderDecoder)
        Dim filtro_fornitori As Integer() = QueryString_To_IntArray("f")
        Dim filtro_grpfatt As Integer() = QueryString_To_IntArray("g")
        Dim filtro_specie As Integer = Stringa_Decodifica(Request.QueryString("s"), AgroKey_EncoderDecoder)
        Dim filtro_varieta As Integer() = QueryString_To_IntArray("v")

        _objParametriServer = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        '#################################################################################
        '#####  Carico il report da File (no Risorsa incorporata, ma Contenuto)
        '#################################################################################
        Dim crHlp As New CrystalHelper

        Select Case _objParametriServer.PivaSuperUser
            Case "01271980391"
                ' FRUTTAGL  
                _nomeFileReport = "CR_PagatiSuConferito_FRUTTAGEL_FF.rpt"
            Case "00040710295"
                ' COFRUTA
                _nomeFileReport = "CR_PagatiSuConferito.rpt"
            Case Else
                _nomeFileReport = "CR_PagatiSuConferito.rpt"
        End Select

        'lettura da file system
        If _nomeFileReport <> "" Then

            Dim pFile As String = HttpContext.Current.Server.MapPath(_pathRpt)
            pFile = AgronicaCoreUtility.FileSystemHelper.AggiungiSlashSeNonEsiste(pFile)

            If My.Computer.FileSystem.FileExists(pFile & _nomeFileReport) Then
                _rptStampa = crHlp.getReportDaFile(pFile, _nomeFileReport)
            End If

        End If

        If Me.IsPostBack Then
            Return
        End If

        _logErrori = ""

        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        Try
            'Dim DT_Intestazione As DataTable
            'Dim objIntest As New AgronicaCoreStampeDAL.DocContab
            'DT_Intestazione = objIntest.DatiIntestazioneImpresa(_piva, Log_Errori, "", "", _objParametriServer)
            'If Not IsNothing(DT_Intestazione) AndAlso DT_Intestazione.Rows.Count > 0 Then
            '    CType(rptStampa.Section2.ReportObjects("TxtPiva"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = DT_Intestazione.Rows(0).Item("Piva")
            '    CType(rptStampa.Section2.ReportObjects("TxtCodFisc"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = DT_Intestazione.Rows(0).Item("Codice_Fiscale")
            '    CType(rptStampa.Section2.ReportObjects("TxtAzAgr"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = DT_Intestazione.Rows(0).Item("rag_soc")
            '    CType(rptStampa.Section2.ReportObjects("TxtIndirizzo"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = DT_Intestazione.Rows(0).Item("ind_impresa") & " " & DT_Intestazione.Rows(0).Item("CAP") + " " + DT_Intestazione.Rows(0).Item("frz_des") + " - " + DT_Intestazione.Rows(0).Item("LOCALITA") + " (" + DT_Intestazione.Rows(0).Item("COMUNI_PROV") + ") "
            'End If

        Catch ex As Exception
            _logErrori += "- Intestazione report: " + vbCrLf + ex.Message + vbCrLf
        End Try

        Dim dsPagatiSuConferito As New DS_PagatiSuConferito

        Try

            _logErrori = ""

            Stampa_PagatiSuConferito(id_liq, filtro_fornitori, filtro_grpfatt, filtro_specie, filtro_varieta, dsPagatiSuConferito)

        Catch ex As Exception
            _logErrori += "- Stampa: " + vbCrLf + ex.Message + vbCrLf
        End Try


        Dim IdentificazioneDocumento As String = ""
        Dim CatCod As Integer = enum_CategorieDocumenti.FF_PagatiSuConferito_Liquidazione
        Dim pathCompletoPdf As String

        Try
            IdentificazioneDocumento = _nomeDocumento + "_" & DateTime.Now().ToString("yyyy_MM_dd_hhmmssff")
            Dim NomeFile As String = AgronicaCoreUtility.Stringhe.EliminaCaratteriSpecialiFile(IdentificazioneDocumento) & ".pdf"

            ' leggo la sottocartella da CategorieDocumenti
            Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
            Dim Sottocartella As String = objCatDoc.Sottocartella(CatCod, "", "", _objParametriServer)
            objCatDoc = Nothing

            ' salvo il report in formato PDF
            Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
            pathCompletoPdf = objGestFile.SalvaReportDocumentPdf(_rptStampa,
                                                                 CatCod,
                                                                 Sottocartella,
                                                                 NomeFile,
                                                                 _objParametriServer,
                                                                 New AgronicaCoreGestioneRichieste.AgroWebConfig)

        Catch ex As Exception
            _logErrori += "- Gestione allegati: " + vbCrLf + ex.Message + vbCrLf
        End Try

        SalvaLogErrori(_logErrori, _nomeDocumento, IdentificazioneDocumento)
        _logErrori = ""

        'MS Dispose del report per evitare problema deallocazione.
        dsPagatiSuConferito.Dispose()
        dsPagatiSuConferito = Nothing

        _rptStampa.Close()
        _rptStampa.Dispose()
        _rptStampa = Nothing

        GC.Collect()

        Response.Redirect("..\..\VisualizzatoreReport.aspx?pdf=" + Stringa_Codifica(pathCompletoPdf, AgroKey_EncoderDecoder, Server))

        SalvaLogErrori(_logErrori, _nomeDocumento, IdentificazioneDocumento)

    End Sub

    Private Sub SalvaLogErrori(Log_Errori As String, Nome_Documento As String, IdentificazioneDocumento As String)
        '-----------------------------------------
        '---- Salvataggio Log Errori -------------
        '-----------------------------------------
        Dim Nome_File As String

        If Log_Errori <> "" Then

            Log_Errori = Nome_Documento & vbCrLf & vbCrLf & Log_Errori

            Nome_File = "Log_Errori_" & IdentificazioneDocumento & CStr(Session("ASG_Utente_Username"))

            Dim objLog As New GestioneLogStampe
            objLog.Gestione_LogErrori_Stampe(_objParametriServer, _
                                             "Stampe_FreshAndFood", _
                                             Nome_File & ".txt", _
                                             Session("ASG_Utente_Username"), _
                                             "PagatiSuConferito.aspx", _
                                             Log_Errori)
        End If

    End Sub

    Private Class Dettaglio
        Implements IComparable(Of Dettaglio)

        Private NumDoc_Sin As String
        Private NumDoc As Integer
        Private NumDoc_Des As String
        Private NrRiga As String
        Private DataDoc As String
        Private PesoLordo As Decimal
        Private Tara As Decimal
        Private PesoNetto As Decimal
        Private ValAcconto As Decimal
        Private Degrado As Decimal
        Private DegradoPerc As Decimal

        Public Function CompareTo(other As Dettaglio) As Integer Implements IComparable(Of Dettaglio).CompareTo
            'Ordinare per Nr_Doc o DataDoc
            If Me.NumDoc < other.NumDoc Then
                Return -1
            End If
            If Me.NumDoc > other.NumDoc Then
                Return 1
            End If
            Return Me.NrRiga.CompareTo(other.NrRiga)
        End Function

        Public Sub New(ByVal elem As Liquid_Mov_CampionamentoConferito, ByVal movDettaglioCollegato As Movimenti_dettagli)

            NumDoc_Sin = elem.Doc_Numero_Sin
            NumDoc = elem.Doc_Numero
            NumDoc_Des = elem.Doc_Numero_Des
            NrRiga = elem.NrRiga
            Degrado = elem.Degrado
            DegradoPerc = elem.DegradoPerc
            DataDoc = CType(elem.Data_Documento, DateTime)
            PesoLordo = elem.KgNetti + elem.Tara + elem.Degrado
            Tara = elem.Tara
            PesoNetto = elem.KgNetti

            If Not movDettaglioCollegato Is Nothing AndAlso CDec(If(movDettaglioCollegato.Imponibile_Netto, 0D)) <> 0D Then
                ValAcconto = Decimal.Negate(CDec(movDettaglioCollegato.Imponibile_Netto))
            Else
                ValAcconto = elem.Imponibile
            End If

        End Sub

        Public Sub Trasferisci(ByRef dr As DS_PagatiSuConferito.DT_PagatiSuConferitoRow)
            dr.Nr_Doc = NumDoc_Sin & CStr(NumDoc) & NumDoc_Des & " Riga " & NrRiga
            dr.Data_Doc = DataDoc
            dr.Peso_Lordo = PesoLordo
            dr.Tara = Tara
            dr.Peso_Netto = PesoNetto
            dr.Val_Acconto = ValAcconto
            dr.Degrado = Degrado
            dr.DegradoPerc = DegradoPerc
        End Sub

    End Class

    Private Class Gruppo
        Implements IComparable(Of Gruppo)

        Private Mat_Cod As Integer
        Private Qual_Cod As Integer
        Private Cal_Cod As Integer
        Private Certif_Cod As Integer
        Private Gruppo_Fatturazione As String
        Private Des_Gruppo As String
        Private Prezzo_Gruppo As Decimal

        Public Dettagli As List(Of Dettaglio)

        Public Function CompareTo(other As Gruppo) As Integer Implements IComparable(Of Gruppo).CompareTo
            'Ordinare per Gruppo_Fatturazione + Des_Gruppo
            Dim cmp = Me.Gruppo_Fatturazione.CompareTo(other.Gruppo_Fatturazione)
            If cmp = 0 Then
                cmp = Me.Des_Gruppo.CompareTo(other.Des_Gruppo)
            End If
            Return cmp
        End Function

        Public Sub New(ByVal elem As Liquid_Mov_CampionamentoConferito, ByVal movDettaglioCollegato As Movimenti_dettagli)

            Mat_Cod = elem.Mat_Cod
            Qual_Cod = elem.Qual_Cod
            Cal_Cod = elem.Calibro_Entrata_Cod
            Certif_Cod = elem.Certif_Cod
            Gruppo_Fatturazione = elem.Grp_Fatt_Sigla
            Dim des_grp As String = elem.Mat_Des & " " & elem.Qual_Sigla
            des_grp = des_grp.Trim()
            des_grp &= " " + elem.Calibro_Entrata_Sigla
            des_grp = des_grp.Trim()
            des_grp &= " " & elem.Certif_Sigla
            Des_Gruppo = des_grp.Trim()

            If Not movDettaglioCollegato Is Nothing AndAlso CDec(movDettaglioCollegato.Prezzo_Effettivo) <> 0D Then
                Prezzo_Gruppo = CDec(movDettaglioCollegato.Prezzo_Effettivo)
            Else
                Prezzo_Gruppo = elem.PrezzoTotaleAlKg
            End If

            Dettagli = New List(Of Dettaglio)

        End Sub

        Public Function IsEqual(ByVal elem As Liquid_Mov_CampionamentoConferito) As Boolean
            Return (Mat_Cod = elem.Mat_Cod AndAlso
                    Qual_Cod = elem.Qual_Cod AndAlso
                    Cal_Cod = elem.Calibro_Entrata_Cod AndAlso
                    Certif_Cod = elem.Certif_Cod)
        End Function

        Public Sub Trasferisci(ByVal prog As Integer, ByRef dr As DS_PagatiSuConferito.DT_PagatiSuConferitoRow)
            dr.Cod_Gruppo = prog
            dr.Des_Gruppo = Des_Gruppo
            dr.Prezzo_Gruppo = Prezzo_Gruppo
        End Sub

    End Class

    Private Class Conferente
        Implements IComparable(Of Conferente)

        Private Cod_RisUm As Integer
        Private Convenevoli As String
        Private Rag_Soc As String
        Private Progressivo As String
        Private Indirizzo As String
        Private CAP As String
        Private Citta As String
        Private Prov As String

        Public Gruppi As List(Of Gruppo)

        Public Function CompareTo(other As Conferente) As Integer Implements IComparable(Of Conferente).CompareTo
            Return Me.Rag_Soc.CompareTo(other.Rag_Soc)
        End Function

        Public Sub New(ByVal elem As Liquid_Mov_CampionamentoConferito, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

            Cod_RisUm = elem.Cod_RisUm

            Dim reader As New FF_LiquidazioneSoci_R
            Dim intestazione As New FF_LiquidazioneSoci_R.IntestazioneObj
            If (reader.IntestazioneDoc(elem.Cod_RisUm, intestazione, objParametri)) Then

                Convenevoli = intestazione.Convenevoli
                Rag_Soc = intestazione.Chi()
                Progressivo = intestazione.Progressivo
                Indirizzo = intestazione.Ind_Des
                CAP = intestazione.CAP
                Citta = intestazione.Dove()
                Prov = intestazione.Pro_Cod

            End If

            Gruppi = New List(Of Gruppo)

        End Sub

        Public Function IsEqual(ByVal elem As Liquid_Mov_CampionamentoConferito) As Boolean
            Return elem.Cod_RisUm = Cod_RisUm
        End Function

        Public Sub Trasferisci(ByRef dr As DS_PagatiSuConferito.DT_PagatiSuConferitoRow)
            dr.Cod_RisUm = Cod_RisUm
            dr.Convenevoli = Convenevoli
            dr.Progressivo = Progressivo
            dr.Rag_Soc = Rag_Soc
            dr.Indirizzo = Indirizzo
            dr.CAP = CAP
            dr.Citta = Citta
            dr.Prov = Prov
        End Sub

    End Class

    Private Class StampaHelper
        Private Conferenti As List(Of Conferente)

        Public Sub New(ByRef elenco As List(Of Liquid_Mov_CampionamentoConferito),
                       ByRef listaMovDettagliCollegati As List(Of Movimenti_dettagli),
                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

            Conferenti = New List(Of Conferente)

            Dim _currConferente As Conferente = Nothing
            Dim _currGruppo As Gruppo = Nothing

            For Each elem In elenco

                Dim movDettaglioCollegato As Movimenti_dettagli = Nothing
                If Not listaMovDettagliCollegati Is Nothing AndAlso listaMovDettagliCollegati.Count > 0 Then
                    movDettaglioCollegato = (From m In listaMovDettagliCollegati Where m.PIVA = elem.PIVA AndAlso m.Id_Mov_Det = elem.Id_Mov_Det Select m).FirstOrDefault()
                End If

                If (_currConferente Is Nothing OrElse Not _currConferente.IsEqual(elem)) Then

                    _currConferente = New Conferente(elem, objParametri)
                    _currGruppo = New Gruppo(elem, movDettaglioCollegato)

                    _currConferente.Gruppi.Add(_currGruppo)
                    Conferenti.Add(_currConferente)

                Else

                    If (Not _currGruppo.IsEqual(elem)) Then

                        _currGruppo = New Gruppo(elem, movDettaglioCollegato)

                        _currConferente.Gruppi.Add(_currGruppo)

                    End If

                End If

                _currGruppo.Dettagli.Add(New Dettaglio(elem, movDettaglioCollegato))

            Next

        End Sub

        Public Sub GeneraDS(ByRef ds As DS_PagatiSuConferito)

            Conferenti.Sort()
            For Each conf In Conferenti

                conf.Gruppi.Sort()
                Dim prog_grp As Integer = 0
                For Each grp In conf.Gruppi

                    prog_grp += 1
                    grp.Dettagli.Sort()
                    For Each dett In grp.Dettagli

                        Dim dr As DS_PagatiSuConferito.DT_PagatiSuConferitoRow
                        dr = ds.DT_PagatiSuConferito.NewDT_PagatiSuConferitoRow()

                        conf.Trasferisci(dr)
                        grp.Trasferisci(prog_grp, dr)
                        dett.Trasferisci(dr)

                        ds.DT_PagatiSuConferito.Rows.Add(dr)

                    Next
                Next
            Next

        End Sub

    End Class

    Private Sub Stampa_PagatiSuConferito(ByVal id_acc_liq As Integer,
                                         ByVal filtro_fornitori As Integer(),
                                         ByVal filtro_grpfatt As Integer(),
                                         ByVal filtro_specie As Integer,
                                         ByVal filtro_varieta As Integer(),
                                         ByRef ds As DS_PagatiSuConferito)

        '==============================================
        '====== QUERY E CARICAMENTO DATASET ===========
        '==============================================

        Try
            Dim reader As New FF_LiquidazioneSoci_R
            Dim listaMovDettagliCollegati As New List(Of Movimenti_dettagli)
            Dim elenco = reader.ElencoPagatiSuConferito(_piva, id_acc_liq,
                                                        filtro_fornitori, filtro_grpfatt, filtro_specie, filtro_varieta, _objParametriServer,
                                                        listaMovDettagliCollegati)

            Dim stampaHelper As New StampaHelper(elenco, listaMovDettagliCollegati, _objParametriServer)

            stampaHelper.GeneraDS(ds)

            'imposto il dataset sul report
            _rptStampa.SetDataSource(ds)

            Dim leggi As New FF_CampionamentoConferimento_R
            Dim strAnagAccLiq = leggi.Leggi_Elem_AnagAccontiLiquidazioni(_piva, id_acc_liq, _objParametriServer)
            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            Dim anagAccLiq = JsonConvert.DeserializeObject(Of AnagAccontiLiquidazioni_CampionamentoConferito)(strAnagAccLiq, serializerSettings)
            _rptStampa.SetParameterValue("Data_AccLiq", CType(anagAccLiq.data_documento, DateTime))

            LeggiIntestazione()

        Catch ex As Exception
            _logErrori += "- Aggancio dataset: " + vbCrLf + ex.Message + vbCrLf
        End Try

    End Sub

    Private Sub LeggiIntestazione()

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

        _rptStampa.SetParameterValue("Intest_Sede_Legale_Ind1", intestSedeLegale)
        _rptStampa.SetParameterValue("Intest_Sede_Legale_Ind2", intestIndSedelegale)
        _rptStampa.SetParameterValue("Intest_Tel_Fax_Cell", intestTelFaxCell)
        _rptStampa.SetParameterValue("Intest_Pec", intestPec)
        _rptStampa.SetParameterValue("Intest_Piva_Cf", intestPivaCf)

    End Sub
End Class