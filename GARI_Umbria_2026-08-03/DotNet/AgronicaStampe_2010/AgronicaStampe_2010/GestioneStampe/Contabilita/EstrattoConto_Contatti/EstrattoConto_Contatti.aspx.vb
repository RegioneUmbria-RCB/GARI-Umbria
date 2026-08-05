Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class EstrattoConto_Contatti
    Inherits System.Web.UI.Page

    Private _rptEcContatto As Rpt_EstrattoConto_Contatti
    Private _logErrori As String

    Private _gestContFlagConsideraSaldiIniziali As Boolean
    Private _gestContDataInizio As Date

    Private _report As Integer
    Private _piva, _ragSoc As String
    Private _dataInizio As String
    Private _dataFine As String
    Private _anno, _annoMin As Integer
    Private _codContatto As String
    Private _ragSocContatto As String
    Private _sezionaleCod As Integer = SEZIONALE_NOFILTRO
    Private _sezionaleDes As String = ""
    Private _sezionaleChkDefault As Integer = -1
    'dim Nome_Agente, Sezionale_Des
    'Dim Cod_Rapporto As Integer
    ' Dim Cod_RisUm As Integer
    ' Dim Tipo_Scadenza As Integer
    'Dim Scadenza As String
    '  Dim Tipo_Filtro As Integer
    'Dim Cod_RisUm_Agente As Integer
    ' Dim Ordinamento As enum_ReportInsoluti_Ordinamento
    Private _nomeDocumento As String = "EstrattoConto_Contatti"
    Private _objParametriServer As New AgronicaCoreDataProvider.AgronicaCoreParametri
    Private _nuoviArrotondamenti As Boolean = False

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init

        _rptEcContatto = New Rpt_EstrattoConto_Contatti

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        _piva = Stringa_Decodifica(CStr(Request.QueryString("p")), AgroKey_EncoderDecoder, Server)

        _report = CInt(Stringa_Decodifica(CStr(Request.QueryString("r")), AgroKey_EncoderDecoder, Server))

        _anno = CInt(Stringa_Decodifica(CStr(Request.QueryString("a")), AgroKey_EncoderDecoder, Server))

        _dataInizio = Stringa_Decodifica(CStr(Request.QueryString("di")), AgroKey_EncoderDecoder, Server)

        _dataFine = Stringa_Decodifica(CStr(Request.QueryString("df")), AgroKey_EncoderDecoder, Server)

        _codContatto = Stringa_Decodifica(CStr(Request.QueryString("cc")), AgroKey_EncoderDecoder, Server)

        _ragSocContatto = Stringa_Decodifica(CStr(Request.QueryString("rsc")), AgroKey_EncoderDecoder, Server)

        _gestContFlagConsideraSaldiIniziali = Stringa_Decodifica(CStr(Request.QueryString("gcfcsi")), AgroKey_EncoderDecoder, Server)

        _gestContDataInizio = Stringa_Decodifica(CStr(Request.QueryString("gcdi")), AgroKey_EncoderDecoder, Server)


        '_sezionaleCod = CInt(Stringa_Decodifica(CStr(Request.QueryString("szc")), AgroKey_EncoderDecoder, Server))

        '_sezionaleDes = Stringa_Decodifica(CStr(Request.QueryString("szd")), AgroKey_EncoderDecoder, Server)

        '_sezionaleChkDefault = Stringa_Decodifica(CStr(Request.QueryString("szdf")), AgroKey_EncoderDecoder, Server)

        'Cod_RisUm = Stringa_Decodifica(CStr(Request.QueryString("cru")), AgroKey_EncoderDecoder, Server)

        'Cod_Rapporto = Stringa_Decodifica(CStr(Request.QueryString("cr")), AgroKey_EncoderDecoder, Server)

        'Tipo_Scadenza = Stringa_Decodifica(CStr(Request.QueryString("tsc")), AgroKey_EncoderDecoder, Server)

        'Scadenza = Stringa_Decodifica(CStr(Request.QueryString("sc")), AgroKey_EncoderDecoder, Server)

        'Ordinamento = Stringa_Decodifica(CStr(Request.QueryString("ord")), AgroKey_EncoderDecoder, Server)

        ''0 = solo insoluti
        'If Not IsNothing(Request.QueryString("tf")) Then
        '    Tipo_Filtro = Stringa_Decodifica(CStr(Request.QueryString("tf")), AgroKey_EncoderDecoder, Server)
        'Else
        '    Tipo_Filtro = 0
        'End If

        'If Not IsNothing(Request.QueryString("cra")) Then
        '    Cod_RisUm_Agente = Stringa_Decodifica(CStr(Request.QueryString("cra")), _
        '                            AgroKey_EncoderDecoder, Server)
        '    Nome_Agente = Stringa_Decodifica(CStr(Request.QueryString("ag")), AgroKey_EncoderDecoder, Server)
        'Else
        '    Cod_RisUm_Agente = 0
        'End If

        _objParametriServer = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        _nuoviArrotondamenti = UsaNuoviArrotondamenti(_objParametriServer)

        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        Dim identificazioneDocumento As String = ""
        Dim cat_cod As enum_CategorieDocumenti = enum_CategorieDocumenti.ScadenziarioPagamentiClienti

        If Not Me.IsPostBack Then

            Try

                Stampa_EC_Contatto()

            Catch exc As Exception
                _logErrori &= "- PageLoad: " & vbCrLf & exc.Message & vbCrLf
            End Try


            Dim nomeFilePdf As String = ""

            Try

                Dim dataInizioAllegati As Date = CDate(_dataInizio)
                Dim dataFineAllegati As Date = CDate(_dataFine)
                identificazioneDocumento &= "_" & Format(dataInizioAllegati, "yyyy_MM_dd") & "_" & Format(dataFineAllegati, "yyyy_MM_dd")
                nomeFilePdf = _nomeDocumento & "_p" & _piva & "_" & identificazioneDocumento & ".pdf"

                ' leggo la sottocartella da CategorieDocumenti
                Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
                Dim sottoCartella As String = objCatDoc.Sottocartella(cat_cod, "", "", _objParametriServer)

                ' salvo il report in formato PDF
                Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
                objGestFile.SalvaReportPdf(_rptEcContatto,
                                           cat_cod, sottoCartella, nomeFilePdf,
                                           _objParametriServer,
                                           New AgronicaCoreGestioneRichieste.AgroWebConfig)

                Dim objAllegati As New AgronicaCoreScadenziario_BIZ.Allegati
                Dim allegatiDocumentiCod As Integer = objAllegati.SalvaAllegato(_piva,
                                                                                cat_cod, _nomeDocumento,
                                                                                nomeFilePdf, sottoCartella,
                                                                                "", "", "", "",
                                                                                dataInizioAllegati,
                                                                                dataFineAllegati,
                                                                                _objParametriServer)

            Catch ex As Exception
                _logErrori &= "- Gestione allegati: " & vbCrLf & ex.Message & vbCrLf
            End Try


            'MS Eliminato passaggio report in session per giro su file: Session("Report") = _rptEcContatto
            Dim reportTemporaneo As String = CrystalHelper.getFileReportTemporaneo()
            Try
                _rptEcContatto.SaveAs(reportTemporaneo, True)
            Catch ex As Exception
                _logErrori &= "- Salvataggio report temporaneo: " & vbCrLf & ex.Message & vbCrLf
            End Try

            'MS Dispose del report per evitare problema deallocazione.
            _rptEcContatto.Close()
            _rptEcContatto.Dispose()
            _rptEcContatto = Nothing

            GC.Collect()

            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------
            Dim infoInLog As String = "Partita Iva = " & CStr(_piva) & ", Data Inizio = " & CStr(_dataInizio) & ", Data Fine = " & CStr(_dataFine)
            SalvaLogErrori_Generico(_logErrori, _nomeDocumento, identificazioneDocumento, "EstrattoConto_Contatti.aspx", "Stampe_Contabilita", infoInLog, _objParametriServer)

            '==================================================================

            Try

                Dim pdf As String = Stringa_Decodifica(CStr(Request.QueryString("PDF")), AgroKey_EncoderDecoder, Server)
                If pdf = "1" Then
                    Response.Redirect("..\..\VisualizzatoreReport.aspx?tmpReportPath=" & Stringa_Codifica(reportTemporaneo, AgroKey_EncoderDecoder, Server) &
                                      "&NomePdf=" & Stringa_Codifica(nomeFilePdf, AgroKey_EncoderDecoder, Server))
                Else
                    Response.Redirect("..\..\VisualizzatoreReport.aspx?ForzaAnteprima=" & Stringa_Codifica("true", AgroKey_EncoderDecoder, Server) &
                                      "&tmpReportPath=" & Stringa_Codifica(reportTemporaneo, AgroKey_EncoderDecoder, Server))
                End If

            Catch ex As Exception
                _logErrori &= "- Export: " & vbCrLf & ex.Message & vbCrLf
            End Try


        End If

    End Sub

    '#####################################################################################################
    Private Sub Stampa_EC_Contatto()

        Dim i As Integer
        Dim dsEcContatto As New DS_EstrattoConto_Contatti

        Dim Parametro_Titolo As String = ""
        Dim Parametro_AnnoContabile As String = ""
        Dim Parametro_IntervalloTemporale As String = ""
        Dim Parametro_PartitaIva As String = ""
        Dim Parametro_CodiceFiscale As String = ""
        Dim Parametro_RagioneSociale As String = ""
        Dim Parametro_Indirizzo As String = ""

        Dim dataSaldo As Date

        'calcolo il saldo al giorno primo (saldo iniziale al quale sommare i movimenti del periodo)
        dataSaldo = DateAdd(DateInterval.Day, -1, CDate(_dataInizio))

        _annoMin = CDate(_dataInizio).Year

        '#######################################################################


        Try

            Parametro_Titolo = "Estratto Conto " & _ragSocContatto

            Dim dtIntestazione As DataTable

            Dim str_inttemp As String = ""
            If _dataInizio <> AGRODATAINIZIO AndAlso _dataFine <> AGRODATAFINE Then
                str_inttemp = "Periodo dal " & _dataInizio & " al " & _dataFine
            End If

            Parametro_AnnoContabile = CStr(_anno)
            Parametro_IntervalloTemporale = str_inttemp

            Dim objIntest As New AgronicaCoreStampeDAL.DocContab
            dtIntestazione = objIntest.DatiIntestazioneImpresa(_piva, Nothing, "", "", _objParametriServer)

            If Not IsNothing(dtIntestazione) AndAlso dtIntestazione.Rows.Count > 0 Then
                Parametro_PartitaIva = dtIntestazione.Rows(0).Item("Piva")
                Parametro_CodiceFiscale = dtIntestazione.Rows(0).Item("Codice_Fiscale")
                Parametro_RagioneSociale = dtIntestazione.Rows(0).Item("rag_soc")
                Parametro_Indirizzo = dtIntestazione.Rows(0).Item("ind_impresa") & " " &
                                      dtIntestazione.Rows(0).Item("CAP") & " " &
                                      dtIntestazione.Rows(0).Item("frz_des") & " - " &
                                      dtIntestazione.Rows(0).Item("LOCALITA") & " (" & dtIntestazione.Rows(0).Item("COMUNI_PROV") & ") "
            End If

        Catch ex As Exception
            _logErrori &= "- Intestazione report: " & vbCrLf & ex.Message & vbCrLf
        End Try

        '########################################################################

        Dim listaCodRisum As String

        Try

            If _codContatto <> "" Then
                'devo ricavare i cod_risum

                Dim objRisum As New AgronicaCoreAnagrafeDAL.Risorse_Umane_R

                listaCodRisum = objRisum.Lista_CodRisUm_ByChiaveContatto(_piva, _codContatto,
                                                                         "", True, "",
                                                                         _objParametriServer)
            End If

        Catch ex As Exception
            _logErrori &= "- varie " & _nomeDocumento & ": " & vbCrLf & ex.Message & vbCrLf
        End Try

        '########################################################################

        Dim dt As DataTable = Nothing
        Dim dtSaldi As DataTable = Nothing
        Dim objPConti As New AgronicaCoreStampeDAL.PianoConti
        Dim dtCodifichePat As DataTable
        Dim objRicXContiPat As New AgronicaCoreContabDAL.RicxConti_Patrimonio_R

        Try

            'leggi DT con codifiche conti personalizzati-gias (x chi ha i conti gias, i codici coincidono)
            dtCodifichePat = objRicXContiPat.Leggi_Codifica_ContiPatrimoniali(_piva, BILANCIO_PERSONALIZZATO, _anno, _annoMin, "", _objParametriServer)

            dt = objPConti.EstrattoConto_Contatti(_dataInizio,
                                                  _dataFine,
                                                  _piva,
                                                  _anno,
                                                  BILANCIO_PERSONALIZZATO,
                                                  "",
                                                  "",
                                                  0,
                                                  0,
                                                  listaCodRisum,
                                                  "", "", "", "",
                                                  "",
                                                  dtCodifichePat,
                                                  _objParametriServer,
                                                  flagNuoviArrotondamenti:=_nuoviArrotondamenti)

            Dim Flag_AggiungiContoPadre_CreditiDebitiBanche As Boolean = False


            dtSaldi = objPConti.Saldo_Conti_Patrimoniali(AGRODATAINIZIO,
                                                        dataSaldo,
                                                        False,
                                                          _gestContFlagConsideraSaldiIniziali,
                                                        _gestContDataInizio,
                                                        _piva, _anno, BILANCIO_PERSONALIZZATO,
                                                        "", "", SP_CONTO_IMPUTABILE_NOFILTRO,
                                                        0,
                                                        CONTO_UE_NOFILTRO, "",
                                                        SEZIONALE_NOFILTRO,
                                                        -1,
                                                        0,
                                                        listaCodRisum,
                                                        CODLIQUIDITA_NOFILTRO,
                                                        False,
                                                        Flag_AggiungiContoPadre_CreditiDebitiBanche,
                                                        "", "", "", "",
                                                        dtCodifichePat,
                                                        _objParametriServer,
                                                        flagNuoviArrotondamenti:=_nuoviArrotondamenti)

        Catch ex As Exception
            _logErrori &= "- Query di Lettura: " & vbCrLf & ex.Message & vbCrLf
        End Try

        Dim saldoConto As Decimal = 0
        Dim importoDare As Decimal = 0
        Dim importoAvere As Decimal = 0
        Dim codiceSplitGruppo As String = ""
        Dim dv As DataView = Nothing

        Dim idAgendaTemp As Long
        Dim desLibTemp As String = ""
        Dim htMovimenti As New Hashtable
        Dim listTemp As New List(Of DS_EstrattoConto_Contatti.DT_EstrattoContoContattiRow)


        Try

            If Not IsNothing(dt) Then

                '§§§GESTIONE DEL SALDO INIZIALE COME RIGA A PARTE
                If Not IsNothing(dtSaldi) AndAlso dtSaldi.Rows.Count > 0 Then

                    Dim drSaldiIniziali As DataRow
                    For i = 0 To dtSaldi.Rows.Count - 1

                        drSaldiIniziali = dt.NewRow

                        drSaldiIniziali.Item("CodiceSplitGruppo") = dtSaldi.Rows(i).Item("CodiceSplitGruppo")
                        drSaldiIniziali.Item("Cod_Conto_Pat") = dtSaldi.Rows(i).Item("Cod_Conto_Pat")
                        drSaldiIniziali.Item("Conto_Pat_Descr") = dtSaldi.Rows(i).Item("Conto_Pat_Descr")
                        drSaldiIniziali.Item("Id_Riclassificazione") = dtSaldi.Rows(i).Item("Id_Riclassificazione")
                        drSaldiIniziali.Item("Id_Agenda") = 0
                        drSaldiIniziali.Item("Lav_Cod") = 0
                        drSaldiIniziali.Item("Des_Lib") = "Saldo al " & CStr(dataSaldo)
                        drSaldiIniziali.Item("Data_Movimento") = AGRODATAINIZIO
                        drSaldiIniziali.Item("Numero_Doc") = ""
                        drSaldiIniziali.Item("Data_Registrazione") = AGRODATAINIZIO
                        drSaldiIniziali.Item("Progr_Protocollo") = 0
                        drSaldiIniziali.Item("Dare") = CDec(dtSaldi.Rows(i).Item("Saldo_Dare"))
                        drSaldiIniziali.Item("Avere") = CDec(dtSaldi.Rows(i).Item("Saldo_Avere"))
                        'DR.Articolo = ""

                        dt.Rows.Add(drSaldiIniziali)
                    Next
                End If

                dv = New DataView(dt)
                dv.Sort = "Data_Registrazione, Progr_Registrazione, id_agenda"

            End If 'dt

            ' If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
            If Not IsNothing(dv) AndAlso dv.Count > 0 Then

                Dim dr As DS_EstrattoConto_Contatti.DT_EstrattoContoContattiRow

                ' For i = 0 To DT.Rows.Count - 1
                'With DT.Rows(i)

                For i = 0 To dv.Count - 1
                    With dv(i)

                        idAgendaTemp = .Item("Id_Agenda")
                        desLibTemp = .Item("Des_Lib")

                        dr = dsEcContatto.DT_EstrattoContoContatti.NewRow

                        'Try

                        codiceSplitGruppo = Replace(.Item("CodiceSplitGruppo"), "SP_", "")

                        dr.CodiceSplitGruppo = codiceSplitGruppo

                        dr.Cod_Conto = .Item("Cod_Conto_Pat")
                        dr.Conto_Descr = codiceSplitGruppo & " - " & .Item("Conto_Pat_Descr")
                        dr.Id_Riclassificazione = .Item("Id_Riclassificazione")
                        dr.Articolo = ""
                        dr.Id_Agenda = .Item("Id_Agenda")
                        dr.Lav_Cod = .Item("Lav_Cod")
                        dr.Des_Lib = .Item("Des_Lib")

                        If .Item("Data_Movimento") = AGRODATAINIZIO Then
                            dr.Data_Movimento = ""
                        Else
                            dr.Data_Movimento = CDate(.Item("Data_Movimento")).ToShortDateString
                        End If

                        dr.Numero_Doc = .Item("Numero_Doc")

                        If .Item("Data_Registrazione") = AGRODATAINIZIO Then
                            dr.Data_Registrazione = ""
                        Else
                            dr.Data_Registrazione = CDate(.Item("Data_Registrazione")).ToShortDateString
                        End If

                        If .Item("Progr_Protocollo") = 0 Then
                            dr.Progr_Protocollo = ""
                        Else
                            dr.Progr_Protocollo = .Item("Progr_Protocollo")
                        End If

                        importoDare = CDec(.Item("Dare"))
                        importoAvere = CDec(.Item("Avere"))
                        saldoConto += importoDare - importoAvere

                        dr.Dare = importoDare
                        dr.Avere = importoAvere
                        dr.Saldo_Riga = saldoConto

                        '  Giulia, 21/10/2016 11.44.40: Se ho già incontrato dei movimenti per quella fattura, in questo conto, li devo raggruppare
                        Dim keyTemp = codiceSplitGruppo & "|" & idAgendaTemp & "|" & desLibTemp
                        If Not htMovimenti.ContainsKey(keyTemp) Then
                            htMovimenti.Add(keyTemp, dr)
                            listTemp.Add(dr)

                        Else
                            'Se sono pagamenti non li devo accorpare perché deve rimanere tracciabilità
                            If desLibTemp.Contains("Pagamento") OrElse desLibTemp.Contains("Incasso") Then
                                listTemp.Add(dr)
                            Else
                                Dim rowTemp As DS_EstrattoConto_Contatti.DT_EstrattoContoContattiRow = (From l In listTemp
                                        Where l.CodiceSplitGruppo = codiceSplitGruppo AndAlso
                                              l.Id_Agenda = idAgendaTemp AndAlso
                                              l.Des_Lib = desLibTemp
                                        Select l).First

                                Dim rowCopy As DS_EstrattoConto_Contatti.DT_EstrattoContoContattiRow = rowTemp

                                rowCopy.Dare += importoDare
                                rowCopy.Avere += importoAvere
                                Dim saldoTemp As Decimal = importoDare - importoAvere
                                rowCopy.Saldo_Riga += saldoTemp

                                htMovimenti.Item(keyTemp) = rowCopy

                                'Sostituisco i valori
                                listTemp.Item(listTemp.FindIndex(Function(x) x.Equals(rowTemp))) = rowCopy
                            End If
                        End If


                        'DSECContatto.DT_EstrattoContoContatti.Rows.Add(Dr)

                    End With

                    'Catch ex As Exception
                    '    Log_Errori &= "Errore al giro " & CStr(i) & ": " & vbCrLf & ex.Message & vbCrLf
                    'End Try
                Next


                'Aggiungo solo alla fine le righe, così le ho potute raggrupare
                For Each row As DS_EstrattoConto_Contatti.DT_EstrattoContoContattiRow In listTemp
                    dsEcContatto.DT_EstrattoContoContatti.Rows.Add(row)
                Next

            End If

        Catch ex As Exception
            _logErrori &= "- analisi movimenti: " & vbCrLf & ex.Message & vbCrLf
        End Try

        '#########################################################

        Try

            _rptEcContatto.SetDataSource(dsEcContatto)

        Catch ex As Exception
            _logErrori &= "- SetDataSource: " & vbCrLf & ex.Message & vbCrLf
        End Try

        '#########################################################


        Try
            _rptEcContatto.SetParameterValue("Titolo", Parametro_Titolo)
            _rptEcContatto.SetParameterValue("AnnoContabile", Parametro_AnnoContabile)
            _rptEcContatto.SetParameterValue("PartitaIva", Parametro_PartitaIva)
            _rptEcContatto.SetParameterValue("CodiceFiscale", Parametro_CodiceFiscale)
            _rptEcContatto.SetParameterValue("RagioneSociale", Parametro_RagioneSociale)
            _rptEcContatto.SetParameterValue("IntervalloTemporale", Parametro_IntervalloTemporale)
            _rptEcContatto.SetParameterValue("Indirizzo", Parametro_Indirizzo)

        Catch ex As Exception
            _logErrori &= "- impostazione parametri: " & vbCrLf & ex.Message & vbCrLf
        End Try

    End Sub

End Class