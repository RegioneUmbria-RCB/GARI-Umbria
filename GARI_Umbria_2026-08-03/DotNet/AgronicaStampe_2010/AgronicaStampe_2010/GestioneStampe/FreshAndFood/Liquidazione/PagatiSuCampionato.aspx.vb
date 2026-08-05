Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreContabDAL
Imports AgronicaCoreEntityFramework_POCO
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.IO
Imports CrystalDecisions.CrystalReports



Public Class PagatiSuCampionato
    Inherits System.Web.UI.Page

    Private _rptStampa As Engine.ReportDocument

    Private _nomeFileReport As String = ""
    Private _pathRpt As String = "~/GestioneStampe/FreshAndFood/Liquidazione/Report"
    Private _nomeDocumento As String = "PagatiSuCampionato"

    Dim _piva As String
    Dim _logErrori As String

    Dim _objParametriServer As New AgronicaCoreDataProvider.AgronicaCoreParametri

    Private Function QueryString_To_IntArray(ByVal qs_name As String) As Integer()

        Dim str As String = Stringa_Decodifica(Request.QueryString(qs_name), AgroKey_EncoderDecoder, Server)
        Dim arr_str() As String = str.Split(New Char() {"|"c}, StringSplitOptions.RemoveEmptyEntries)
        Return Array.ConvertAll(arr_str, Function(s) Int32.Parse(s))

    End Function

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        _piva = Stringa_Decodifica(CStr(Request.QueryString("p")), AgroKey_EncoderDecoder, Server)

        Dim id_liq As Integer = Stringa_Decodifica(Request.QueryString("liq"), AgroKey_EncoderDecoder, Server)
        Dim filtro_fornitori As Integer() = QueryString_To_IntArray("f")
        Dim filtro_grpfatt As Integer() = QueryString_To_IntArray("g")
        Dim filtro_specie As Integer = Stringa_Decodifica(Request.QueryString("s"), AgroKey_EncoderDecoder, Server)
        Dim filtro_varieta As Integer() = QueryString_To_IntArray("v")

        _objParametriServer = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        '#################################################################################
        '#####  Carico il report da File (no Risorsa incorporata, ma Contenuto)
        '#################################################################################
        Dim crHlp As New CrystalHelper

        Select Case _objParametriServer.PivaSuperUser
            Case "01271980391"
                ' FRUTTAGL  
                _nomeFileReport = "CR_PagatiSuCampionato_FRUTTAGEL_FF.rpt"
            Case "00040710295"
                ' COFRUTA
                _nomeFileReport = "CR_PagatiSuCampionato.rpt"
            Case Else
                _nomeFileReport = "CR_PagatiSuCampionato.rpt"
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

        Dim dsPagatiSuCampionato As New DS_PagatiSuCampionato

        Try

            _logErrori = ""

            EseguiStampa(id_liq, filtro_fornitori, filtro_grpfatt, filtro_specie, filtro_varieta, dsPagatiSuCampionato)

        Catch ex As Exception
            _logErrori += "- Stampa: " + vbCrLf + ex.Message + vbCrLf
        End Try


        Dim IdentificazioneDocumento As String = ""
        Dim CatCod As Integer = enum_CategorieDocumenti.FF_PagatiSuCampionato_Liquidazione
        Dim pathCompletoPdf As String = ""

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
        dsPagatiSuCampionato.Dispose()
        dsPagatiSuCampionato = Nothing

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
                                             "PagatiSuCampionato.aspx", _
                                             Log_Errori)
        End If

    End Sub

    Private Class Calibro
        Implements IComparable(Of Calibro)

        Private Ordine_Calibro As Integer
        Private Id_Calibro As Integer
        Private Des_Calibro As String
        Private KgNetti As Decimal
        Private Imponibile As Decimal
        Private Prezzo As Decimal

        Public ReadOnly Property Ordine As Integer
            Get
                Return Ordine_Calibro
            End Get
        End Property
        Public ReadOnly Property Id As Integer
            Get
                Return Id_Calibro
            End Get
        End Property
        Public ReadOnly Property Des As String
            Get
                Return Des_Calibro
            End Get
        End Property

        Public Function CompareTo(other As Calibro) As Integer Implements System.IComparable(Of Calibro).CompareTo
            If Me.Ordine_Calibro < other.Ordine_Calibro Then
                Return -1
            End If
            If Me.Ordine_Calibro > other.Ordine_Calibro Then
                Return 1
            End If
            If Me.Id_Calibro < other.Id_Calibro Then
                Return -1
            End If
            If Me.Id_Calibro > other.Id_Calibro Then
                Return 1
            End If
            Return 0
        End Function

        Public Sub New(ByVal obj As Object)
            Ordine_Calibro = obj.Ordine
            Id_Calibro = obj.Id_Calibro
            Des_Calibro = obj.Des_Calibro
            KgNetti = obj.KgNetti
            Imponibile = obj.Imponibile
            Prezzo = obj.Prezzo
        End Sub
        'Public Sub New(ByVal cal As Calibro)
        '    Ordine_Calibro = cal.Ordine_Calibro
        '    Id_Calibro = cal.Id_Calibro
        '    Des_Calibro = cal.Des_Calibro
        '    KgNetti = cal.KgNetti
        '    Imponibile = cal.Imponibile
        '    Prezzo = cal.Prezzo
        'End Sub
        Public Sub New(ByVal ordine As Integer, ByVal id As Integer, ByVal des As String)
            Ordine_Calibro = ordine
            Id_Calibro = id
            Des_Calibro = des
            KgNetti = 0
            Imponibile = 0
            Prezzo = 0
        End Sub
        Public Sub resetta()
            KgNetti = 0
            Imponibile = 0
            Prezzo = 0
        End Sub
        Public Sub aggiorna(ByVal cal As Calibro)
            'Ordine_Calibro = cal.Ordine_Calibro
            'Id_Calibro = cal.Id_Calibro
            'Des_Calibro = cal.Des_Calibro
            KgNetti = cal.KgNetti
            Imponibile = cal.Imponibile
            Prezzo = cal.Prezzo
        End Sub
        Public Sub Trasferisci(ByVal prog As Integer, ByRef dr As DS_PagatiSuCampionato.DT_PagatiSuCampionatoRow)
            dr.Cal_Prog = prog
            dr.Cal_Id = Id_Calibro
            dr.Cal_Des = Des_Calibro
            dr.Netto = KgNetti
            dr.Imponibile = Imponibile
            dr.Prezzo = Prezzo
        End Sub

    End Class

    Private Class Documento
        Implements IComparable(Of Documento)

        Private Id_Mov_Det As Integer
        Private NumDoc_Sin As String
        Private NumDoc As Integer
        Private NumDoc_Des As String
        Private NrRiga As String
        Private DataDoc As Date

        Public Calibri As List(Of Calibro)

        Public Function CompareTo(other As Documento) As Integer Implements System.IComparable(Of Documento).CompareTo
            'Ordinare grp.Dettagli per Nr_Doc o DataDoc
            If Me.NumDoc < other.NumDoc Then
                Return -1
            ElseIf Me.NumDoc > other.NumDoc Then
                Return 1
            End If
            Return Me.NrRiga.CompareTo(other.NrRiga)
        End Function

        Public Sub New(ByVal elem As Object)

            Id_Mov_Det = elem.Id_Mov_Det
            NumDoc_Sin = elem.Doc_Numero_Sin
            NumDoc = elem.Doc_Numero
            NumDoc_Des = elem.Doc_Numero_Des
            NrRiga = elem.NrRiga
            DataDoc = elem.DtDoc

            Calibri = New List(Of Calibro)

        End Sub

        Public Function IsEqual(ByVal elem As Object)
            Return Id_Mov_Det = elem.Id_Mov_Det
            'Dim this_doc = elem.Doc_Numero_Sin & CStr(elem.Doc_Numero) & elem.Doc_Numero_Des
            'If (String.Compare(doc_nr, this_doc) <> 0) Then
        End Function

        Public Sub Trasferisci(ByVal prog As Integer, ByRef dr As DS_PagatiSuCampionato.DT_PagatiSuCampionatoRow)
            dr.Doc_Prog = prog
            dr.Doc_Nr = NumDoc_Sin & CStr(NumDoc) & NumDoc_Des & " Riga " & NrRiga.Trim()
            dr.Doc_Data = CType(DataDoc, DateTime)
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
        Private PercValoreAcconto As Decimal

        Public Documenti As List(Of Documento)

        Public Function CompareTo(other As Gruppo) As Integer Implements System.IComparable(Of Gruppo).CompareTo
            'Ordinare conf.Gruppi per Gruppo_Fatturazione + Des_Gruppo
            Dim cmp = Me.Gruppo_Fatturazione.CompareTo(other.Gruppo_Fatturazione)
            If cmp = 0 Then
                cmp = Me.Des_Gruppo.CompareTo(other.Des_Gruppo)
            End If
            Return cmp
        End Function

        Public Sub New(ByVal elem As Object)

            Mat_Cod = elem.Mat_Cod
            Qual_Cod = elem.Qual_Cod
            Cal_Cod = elem.Calibro_Entrata_Cod
            Certif_Cod = elem.Certif_Cod
            Gruppo_Fatturazione = elem.Grp_Fatt_Descr 'elem.Grp_Fatt_Sigla
            Dim des As String = elem.Mat_Des & " " & elem.Qual_Sigla
            des = des.Trim()
            des &= " " & elem.Calibro_Entrata_Sigla
            des = des.Trim()
            des &= " " & elem.Certif_Sigla
            Des_Gruppo = des.Trim()
            PercValoreAcconto = elem.PercValoreAcconto

            Documenti = New List(Of Documento)

        End Sub

        Public Function IsEqual(ByVal elem As Object) As Boolean
            Return (Mat_Cod = elem.Mat_Cod AndAlso
                    Qual_Cod = elem.Qual_Cod AndAlso
                    Cal_Cod = elem.Calibro_Entrata_Cod AndAlso
                    Certif_Cod = elem.Certif_Cod)
        End Function

        Public Sub Trasferisci(ByVal prog As Integer, ByRef dr As DS_PagatiSuCampionato.DT_PagatiSuCampionatoRow)
            dr.Cod_Gruppo = prog
            dr.Des_Gruppo = Des_Gruppo
            dr.GruppoFatturazione = Gruppo_Fatturazione
            dr.PercValoreAcconto = PercValoreAcconto
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

        Public Sub New(ByVal elem As Object, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

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

        Public Function IsEqual(ByVal elem As Object) As Boolean
            Return elem.Cod_RisUm = Cod_RisUm
        End Function

        Public Sub Trasferisci(ByRef dr As DS_PagatiSuCampionato.DT_PagatiSuCampionatoRow)
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

        Public Sub New(ByVal elenco As List(Of Object), ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

            Conferenti = New List(Of Conferente)

            Dim _currConferente As Conferente = Nothing
            Dim _currGruppo As Gruppo = Nothing
            Dim _currDocumento As Documento = Nothing

            For Each elem In elenco

                If (_currConferente Is Nothing OrElse Not _currConferente.IsEqual(elem)) Then

                    _currConferente = New Conferente(elem, objParametri)
                    _currGruppo = New Gruppo(elem)
                    _currDocumento = New Documento(elem)

                    _currGruppo.Documenti.Add(_currDocumento)
                    _currConferente.Gruppi.Add(_currGruppo)
                    Conferenti.Add(_currConferente)

                Else

                    If (Not _currGruppo.IsEqual(elem)) Then

                        _currGruppo = New Gruppo(elem)
                        _currDocumento = New Documento(elem)

                        _currGruppo.Documenti.Add(_currDocumento)
                        _currConferente.Gruppi.Add(_currGruppo)

                    Else

                        If (Not _currDocumento.IsEqual(elem)) Then

                            _currDocumento = New Documento(elem)

                            _currGruppo.Documenti.Add(_currDocumento)

                        End If

                    End If

                End If

                For Each c In elem.Calibro
                    _currDocumento.Calibri.Add(New Calibro(c))
                Next
            
            Next

        End Sub

        Public Sub GeneraDS(ByRef ds As DS_PagatiSuCampionato)



            Dim calGrpList As New List(Of Calibro)



            Conferenti.Sort()
            For Each conf In Conferenti

                conf.Gruppi.Sort()
                Dim prog_grp As Integer = 0
                For Each grp In conf.Gruppi


                    'Devo aggiustare i calibri... ogni gruppo deve avere sempre la lista di calibri completa...
                    calGrpList.Clear()
                    Dim found As Boolean
                    Dim idx As Integer
                    For Each doc In grp.Documenti
                        For Each cal In doc.Calibri

                            found = False
                            idx = 0
                            While Not found And idx < calGrpList.Count
                                found = calGrpList(idx).Id = cal.Id
                                idx += 1
                            End While
                            If Not found Then
                                calGrpList.Add(New Calibro(cal.Ordine, cal.Id, cal.Des))
                            End If

                        Next
                    Next
                    calGrpList.Sort()


                    prog_grp += 1
                    grp.Documenti.Sort()
                    Dim prog_doc As Integer = 0
                    For Each doc In grp.Documenti



                        For Each cal In doc.Calibri
                            found = False
                            idx = 0
                            While Not found And idx < calGrpList.Count
                                If calGrpList(idx).Id = cal.Id Then
                                    calGrpList(idx).aggiorna(cal)
                                    found = True
                                End If
                                idx += 1
                            End While
                        Next



                        prog_doc += 1
                        'doc.Calibri.Sort()
                        Dim prog_cal As Integer = 0
                        For Each cal In calGrpList 'doc.Calibri

                            prog_cal += 1

                            Dim dr As DS_PagatiSuCampionato.DT_PagatiSuCampionatoRow
                            dr = ds.DT_PagatiSuCampionato.NewDT_PagatiSuCampionatoRow()

                            conf.Trasferisci(dr)
                            grp.Trasferisci(prog_grp, dr)
                            doc.Trasferisci(prog_doc, dr)
                            cal.Trasferisci(prog_cal, dr)

                            cal.resetta()

                            ds.DT_PagatiSuCampionato.Rows.Add(dr)

                        Next
                    Next
                Next
            Next

        End Sub

    End Class

    Private Sub EseguiStampa(ByVal id_acc_liq As Integer,
                             ByVal filtro_fornitori As Integer(),
                             ByVal filtro_grpfatt As Integer(),
                             ByVal filtro_specie As Integer,
                             ByVal filtro_varieta As Integer(),
                             ByRef ds As DS_PagatiSuCampionato)

        '==============================================
        '====== QUERY E CARICAMENTO DATASET ===========
        '==============================================

        Try

            Dim reader As New FF_LiquidazioneSoci_R
            Dim elenco = reader.Prepara_Scheda_Liquidazione_Su_Campionato(_piva, id_acc_liq, filtro_fornitori, filtro_grpfatt, filtro_specie, filtro_varieta, _objParametriServer)

            Dim stampaHelper As New StampaHelper(elenco, _objParametriServer)

            stampaHelper.GeneraDS(ds)

            'imposto il dataset sul report
            _rptStampa.SetDataSource(ds)

            Dim leggi As New FF_CampionamentoConferimento_R
            Dim strAnagAccLiq = leggi.Leggi_Elem_AnagAccontiLiquidazioni(_piva, id_acc_liq, _objParametriServer)
            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            Dim anagAccLiq = JsonConvert.DeserializeObject(Of AnagAccontiLiquidazioni_CampionamentoConferito)(strAnagAccLiq, serializerSettings)
            _rptStampa.SetParameterValue("Des_AccLiq", anagAccLiq.descrizione)

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