Imports AgronicaCoreDataProvider.Agro_Math
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.Sicurezza


Public Class AgentiProvvigioni_XLS
    Inherits System.Web.UI.Page


    Private _piva, _dataInizio, _dataFine, _strFiltroAgg As String
    Private _codRisumAgente, _codRisumCliente, _matCod, _lavCod As Integer
    Private _chkIncassato, _chkNonincassato, _chkParzialmenteincassato As Integer

    Private _objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri
    Private _objParametriUtenti As AgronicaCoreDataProvider.AgronicaCoreParametri

    Const NumeroColonneDati As Integer = 14
    Const NumeroColonneQueryString As Integer = 11

    Private _filtroImpostato As String = ""

    '#################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Dim riga As HtmlTableRow

        'La Pagina deve essere visualizzata come un foglio Excel
        Response.ContentType = "application/vnd.ms-excel"
        Response.AddHeader("Content-Disposition", "inline; filename = AgentiProvvigioni.xls")


        '##############################################################
        '#####  Verifico Credenziali di Accesso  ######################
        '##############################################################

        '----- Verifico che l'utente sia autenticato

        If Session("ASG_objParametri_Server") Is Nothing Then
            Response.Redirect("~/Custom500.aspx")
        End If

        _objParametriServer = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        _objParametriUtenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))


        '##############################################################
        '############  Lettura Parametri Query String #################
        '##############################################################

        Try

            _piva = Stringa_Decodifica(CStr(Request.QueryString("p")), AgroKey_EncoderDecoder, Server)

            _dataInizio = Stringa_Decodifica(CStr(Request.QueryString("di")), AgroKey_EncoderDecoder, Server)
            If _dataInizio = "" Then
                _dataInizio = Date.Today.ToShortDateString
            End If

            _dataFine = Stringa_Decodifica(CStr(Request.QueryString("df")), AgroKey_EncoderDecoder, Server)
            If _dataFine = "" Then
                _dataFine = AGRODATAFINE.ToShortDateString
            End If

            _codRisumAgente = Stringa_Decodifica(CStr(Request.QueryString("cra")), AgroKey_EncoderDecoder, Server)

            _codRisumCliente = Stringa_Decodifica(CStr(Request.QueryString("crc")), AgroKey_EncoderDecoder, Server)

            _matCod = Stringa_Decodifica(CStr(Request.QueryString("mc")), AgroKey_EncoderDecoder, Server)

            _lavCod = Stringa_Decodifica(CStr(Request.QueryString("lc")), AgroKey_EncoderDecoder, Server)

            _chkIncassato = Stringa_Decodifica(CStr(Request.QueryString("chki")), AgroKey_EncoderDecoder, Server)

            _chkNonincassato = Stringa_Decodifica(CStr(Request.QueryString("chkni")), AgroKey_EncoderDecoder, Server)

            _chkParzialmenteincassato = Stringa_Decodifica(CStr(Request.QueryString("chkpi")), AgroKey_EncoderDecoder, Server)

            _strFiltroAgg = Stringa_Decodifica(CStr(Request.QueryString("strfa")), AgroKey_EncoderDecoder, Server)

        Catch ex As Exception
            riga = New HtmlTableRow
            riga.Cells.Add(New HtmlTableCell)
            riga.Cells(0).ColSpan = NumeroColonneQueryString
            riga.Cells(0).InnerHtml = "Dati da querystring: " & ex.Message
            Me.TableExcel.Rows.Add(riga)
        End Try

        _filtroImpostato = "Filtro impostato: "
        If _codRisumAgente <> 0 Then
            _filtroImpostato &= "Agente + "
        End If
        If _codRisumCliente <> 0 Then
            _filtroImpostato &= "Cliente + "
        End If
        If _matCod <> 0 Then
            _filtroImpostato &= "Prodotto + "
        End If
        If _lavCod <> 0 Then
            _filtroImpostato &= "Tipo Documento + "
        End If
        If _chkIncassato <> 0 Then
            _filtroImpostato &= "Saldato + "
        End If
        If _chkNonincassato <> 0 Then
            _filtroImpostato &= "Non Saldato + "
        End If
        If _chkParzialmenteincassato <> 0 Then
            _filtroImpostato &= "Saldato In Parte + "
        End If
        If _filtroImpostato <> "" Then
            _filtroImpostato = Left(_filtroImpostato, _filtroImpostato.Length - 2)
        End If


        '##############################################################
        '###################  ELABORA REPORT #########################
        '##############################################################

        Stampa_Agenti_Provvigioni()

    End Sub

    '##############################################################
    Public Function Crea_DT_Risultato() As DataTable

        Dim dt As New DataTable

        '----- Definisco la struttura del DataTable

        dt.Columns.Add(New DataColumn("NomeAgente", GetType(String)))
        dt.Columns.Add(New DataColumn("Tipo_Documento", GetType(String)))
        dt.Columns.Add(New DataColumn("Numero_Documento", GetType(String)))
        dt.Columns.Add(New DataColumn("Data_Documento", GetType(String)))
        dt.Columns.Add(New DataColumn("Cliente", GetType(String)))
        dt.Columns.Add(New DataColumn("Articolo", GetType(String)))
        dt.Columns.Add(New DataColumn("Qta", GetType(String)))
        dt.Columns.Add(New DataColumn("Prezzo", GetType(String)))
        dt.Columns.Add(New DataColumn("Imponibile", GetType(String)))
        dt.Columns.Add(New DataColumn("Totale_Doc", GetType(String)))
        dt.Columns.Add(New DataColumn("Riscosso", GetType(String)))
        dt.Columns.Add(New DataColumn("Saldato", GetType(String)))
        dt.Columns.Add(New DataColumn("Perc_Prov", GetType(String)))
        dt.Columns.Add(New DataColumn("Prov", GetType(String)))

        Return dt

    End Function

    '##############################################################
    Private Sub Stampa_Agenti_Provvigioni()

        Dim riga As HtmlTableRow


        '##############################################################
        '####################  LETTURA DATI ###########################
        '##############################################################

        Dim objDocContab As New AgronicaCoreStampeDAL.DocContab
        Dim objPagamenti As New AgronicaCoreContabDAL.Pagamenti_R
        Dim dtPag As DataTable
        Dim dt As DataTable
        Dim dtRisultato As DataTable

        dtRisultato = Crea_DT_Risultato()

        Dim totImponibile As Decimal = 0
        Dim totImponibileIncassato As Decimal = 0
        Dim totImponibileNonIncassato As Decimal = 0
        Dim totProvv As Decimal = 0
        Dim totProvvIncassato As Decimal = 0
        Dim totProvvNonIncassato As Decimal = 0


        Try

            dt = objDocContab.AgentiProvvigioni(_piva,
                                                _lavCod,
                                                _matCod,
                                                _codRisumAgente,
                                                _codRisumCliente,
                                                _dataInizio,
                                                _dataFine,
                                                _strFiltroAgg,
                                                "",
                                                _objParametriServer)


            Dim filtro As String = " Previsto_Avvenuto = " & CStr(enum_Pagamento.Avvenuto)

            dtPag = objPagamenti.Leggi(_piva,
                                       0,
                                       0,
                                       0,
                                       0,
                                       enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                       filtro, "",
                                       _objParametriServer)


            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then

                Dim flagRiscosso As Boolean
                Dim flagNonRiscosso As Boolean
                Dim flagParzialmenteRiscosso As Boolean
                Dim strRiscosso As String = ""
                Dim importoRiscosso As Decimal = 0
                Dim importoDaRiscuotere As Decimal = 0
                Dim i As Integer

                For i = 0 To dt.Rows.Count - 1

                    Try

                        objPagamenti.Riscosso_NonRiscosso_ParzialmenteRiscosso(dtPag,
                                                                                _piva,
                                                                                dt.Rows(i).Item("Id_Agenda"),
                                                                                dt.Rows(i).Item("Id_Mov_Contabile"),
                                                                                dt.Rows(i).Item("Lav_Cod"),
                                                                                dt.Rows(i).Item("Num_Protocollo"),
                                                                                flagRiscosso,
                                                                                flagNonRiscosso,
                                                                                flagParzialmenteRiscosso,
                                                                                importoRiscosso,
                                                                                importoDaRiscuotere,
                                                                                _objParametriServer)


                        If flagRiscosso = True Then
                            strRiscosso = "SI"
                        ElseIf flagNonRiscosso = True Then
                            strRiscosso = "NO"
                        ElseIf flagParzialmenteRiscosso = True Then
                            strRiscosso = "IN PARTE"
                        Else
                            strRiscosso = ""
                        End If

                        If (_chkIncassato = 0 And _chkNonincassato = 0 And _chkParzialmenteincassato = 0) Or
                           (_chkIncassato = 1 And _chkNonincassato = 1 And _chkParzialmenteincassato = 1) Then
                            'nessun filtro sui pagamenti, tutto
                            Elabora_RigaDt(dt.Rows(i), dtRisultato, strRiscosso, importoRiscosso, totImponibile, totImponibileIncassato, totProvv, totProvvIncassato)
                        End If

                        If _chkIncassato = 1 And _chkNonincassato = 0 And _chkParzialmenteincassato = 0 Then
                            'solo incassati
                            If flagRiscosso = True Then
                                Elabora_RigaDt(dt.Rows(i), dtRisultato, strRiscosso, importoRiscosso, totImponibile, totImponibileIncassato, totProvv, totProvvIncassato)
                            End If
                        End If

                        If _chkIncassato = 0 And _chkNonincassato = 1 And _chkParzialmenteincassato = 0 Then
                            'solo non incassati
                            If flagNonRiscosso = True Then
                                Elabora_RigaDt(dt.Rows(i), dtRisultato, strRiscosso, importoRiscosso, totImponibile, totImponibileIncassato, totProvv, totProvvIncassato)
                            End If
                        End If

                        If _chkIncassato = 0 And _chkNonincassato = 0 And _chkParzialmenteincassato = 1 Then
                            'solo parzialmente incassati
                            If flagParzialmenteRiscosso = True Then
                                Elabora_RigaDt(dt.Rows(i), dtRisultato, strRiscosso, importoRiscosso, totImponibile, totImponibileIncassato, totProvv, totProvvIncassato)
                            End If
                        End If

                        If _chkIncassato = 1 And _chkNonincassato = 1 And _chkParzialmenteincassato = 0 Then
                            ' incassati e non incassati
                            If flagRiscosso = True Or flagNonRiscosso = True Then
                                Elabora_RigaDt(dt.Rows(i), dtRisultato, strRiscosso, importoRiscosso, totImponibile, totImponibileIncassato, totProvv, totProvvIncassato)
                            End If
                        End If

                        If _chkIncassato = 1 And _chkNonincassato = 0 And _chkParzialmenteincassato = 1 Then
                            ' incassati e parzialmente incassati
                            If flagRiscosso = True Or flagParzialmenteRiscosso = True Then
                                Elabora_RigaDt(dt.Rows(i), dtRisultato, strRiscosso, importoRiscosso, totImponibile, totImponibileIncassato, totProvv, totProvvIncassato)
                            End If
                        End If

                        If _chkIncassato = 0 And _chkNonincassato = 1 And _chkParzialmenteincassato = 1 Then
                            'non incassati e parzialmente incassati
                            If flagNonRiscosso = True Or flagParzialmenteRiscosso = True Then
                                Elabora_RigaDt(dt.Rows(i), dtRisultato, strRiscosso, importoRiscosso, totImponibile, totImponibileIncassato, totProvv, totProvvIncassato)
                            End If
                        End If

                    Catch ex As Exception
                        riga = New HtmlTableRow
                        riga.Cells.Add(New HtmlTableCell)
                        riga.Cells(0).ColSpan = NumeroColonneDati
                        riga.Cells(0).InnerHtml = "Verifica pagamenti: " & ex.Message
                        Me.TableExcel.Rows.Add(riga)
                    End Try

                Next

                'dt.Columns.Remove("Id_Agenda")

                totImponibileNonIncassato = ArrotondaVal_2(totImponibile - totImponibileIncassato)

                totProvvNonIncassato = ArrotondaVal_2(totProvv - totProvvIncassato)

            End If


        Catch ex As Exception
            riga = New HtmlTableRow
            riga.Cells.Add(New HtmlTableCell)
            riga.Cells(0).ColSpan = NumeroColonneDati
            riga.Cells(0).InnerHtml = "stampa Agenti: " & ex.Message
            Me.TableExcel.Rows.Add(riga)
        End Try


        '##############################################################
        '###################  Creazione EXCEL #########################
        '##############################################################

        Crea_EXCEL(dtRisultato,
                   totImponibile, totImponibileIncassato, totImponibileNonIncassato,
                   totProvv, totProvvIncassato, totProvvNonIncassato)

    End Sub

    '##############################################################
    Private Sub Elabora_RigaDt(ByVal drDoc As DataRow,
                               ByRef dtRisultato As DataTable,
                               ByVal strRiscosso As String,
                               ByVal importoRiscosso As Decimal,
                               ByRef totImponibile As Decimal,
                               ByRef totImponibileIncassato As Decimal,
                               ByRef totProvv As Decimal,
                               ByRef totProvvIncassato As Decimal)

        Dim riga As HtmlTableRow

        Try

            Dim drRis As DataRow

            drRis = dtRisultato.NewRow

            drRis.Item("NomeAgente") = drDoc.Item("Nome_Agente")

            Select Case drDoc.Item("Lav_cod")
                Case LAVCOD_RICEVUTA_EMESSA
                    drRis.Item("Tipo_Documento") = "Ricevuta Fiscale"
                Case LAVCOD_FATTURA_EMESSA
                    If drDoc.Item("extra_int") = 1 Then
                        drRis.Item("Tipo_Documento") = "Fattura Immediata"
                    Else
                        drRis.Item("Tipo_Documento") = "Fattura Differita"
                    End If
                Case LAVCOD_NOTA_ACCREDITO_EMESSA
                    drRis.Item("Tipo_Documento") = "Nota di Credito"
                Case Else
                    drRis.Item("Tipo_Documento") = ""
            End Select

            drRis.Item("Numero_Documento") = Ricava_NumeroDocumento_Senza_Sequenza(drDoc.Item("Doc_Numero_Sin"),
                                                                                   drDoc.Item("Doc_Numero"),
                                                                                   drDoc.Item("Doc_Numero_Des"))


            drRis.Item("Data_Documento") = CDate(drDoc.Item("Data_Movimento")).ToShortDateString
            'Giulia - 15/02/2018: se nota di credito, allora inverto il segno
            drRis.Item("Totale_Doc") = IIf(drDoc.Item("Lav_cod") = LAVCOD_NOTA_ACCREDITO_EMESSA, -1, 1) * drDoc.Item("Num_Protocollo")
            drRis.Item("Cliente") = drDoc.Item("cliente")

            Select Case drDoc.Item("elem_cod")

                Case SERVIZI, ALTRI_BENI
                    drRis.Item("Articolo") = drDoc.Item("mov_det_des")

                Case Else
                    Dim objcontab As New AgronicaCoreContabDAL.Contabilita_R
                    drRis.Item("Articolo") = objcontab.Componi_Descrizione_MateriaPrima(_piva,
                                                                                        drDoc.Item("Elem_Cod"),
                                                                                        drDoc.Item("mat_cod"),
                                                                                        drDoc.Item("Cod_Progetto"),
                                                                                        drDoc.Item("Fase_Cod"),
                                                                                        drDoc.Item("Lotto"),
                                                                                        drDoc.Item("Cal_Cod"),
                                                                                        False,
                                                                                        AGRODATAINIZIO,
                                                                                        AGRODATAFINE,
                                                                                        False,
                                                                                        _objParametriServer,
                                                                                        _objParametriUtenti)

            End Select

            drRis.Item("Qta") = drDoc.Item("qta")
            drRis.Item("Prezzo") = ArrotondaVal_2(CDec(drDoc.Item("Prezzo_Unitario_Netto")))
            drRis.Item("Imponibile") = drDoc.Item("Imponibile_Netto")
            drRis.Item("Perc_Prov") = drDoc.Item("Provvigione_Dett")

            'TODO: CALCOLO --> inevitabile in questo punto, perchè la provvigione su db è salvata solo come totale del documento
            drRis.Item("Prov") = ArrotondaVal_2(CDec(drRis.Item("Imponibile")) * CDec(drRis.Item("Perc_Prov")) / 100)

            drRis.Item("Saldato") = strRiscosso.ToUpper
            drRis.Item("Riscosso") = importoRiscosso

            totImponibile += CDec(drRis.Item("Imponibile"))

            totProvv += CDec(drRis.Item("Prov"))

            If strRiscosso.ToUpper = "SI" Then
                totImponibileIncassato += CDec(drRis.Item("Imponibile"))
                totProvvIncassato += CDec(drRis.Item("Prov"))
            End If

            dtRisultato.Rows.Add(drRis)

        Catch ex As Exception
            riga = New HtmlTableRow
            riga.Cells.Add(New HtmlTableCell)
            riga.Cells(0).ColSpan = NumeroColonneDati
            riga.Cells(0).InnerHtml = "Elabora_RigaDt: " & ex.Message
            Me.TableExcel.Rows.Add(riga)
        End Try

    End Sub

    '##############################################################
    Private Sub Crea_EXCEL(ByVal dtFinale As DataTable,
                           ByVal totImponibile As Decimal,
                           ByVal totImponibileIncassato As Decimal,
                           ByVal totImponibileNonIncassato As Decimal,
                           ByVal totProvv As Decimal,
                           ByVal totProvvIncassata As Decimal,
                           ByVal totProvvNonIncassata As Decimal)

        Dim riga As HtmlTableRow

        '##############################################################
        '#####  Costruisco la tabella   ###############################
        '##############################################################

        Dim i, j, k As Integer

        Try

            Dim colonnaDt As String

            'ElaboraCellaHTML(Me.TableExcel.Rows(0).Cells(0), 2, "", "", "Yellow", "left", "middle")
            'Me.TableExcel.Rows(0).Cells(0).InnerHtml = "Anagrafica Contatti"
            'Me.TableExcel.Rows(0).Cells(0).Style.Item("font-weight") = "bold"
            'Me.TableExcel.Rows(0).Cells(0).Style.Item("font-size") = "18px"
            'Me.TableExcel.Rows(0).Cells(0).Style.Item("vertical-align") = "middle"
            'Me.TableExcel.Rows(0).Cells(0).Style.Item("border-top-width") = "1px"

            Me.TableExcel.Rows(0).Cells(0).ColSpan = CInt(dtFinale.Columns.Count)
            Me.TableExcel.Rows(0).Cells(0).InnerText = "CALCOLO PROVVIGIONI AGENTI"
            Me.TableExcel.Rows(0).Cells(0).Style.Item("vertical-align") = "middle"
            Me.TableExcel.Rows(0).Cells(0).Style.Item("font-weight") = "bold"
            'Me.TableExcel.Rows(0).Cells(0).Style.Item("font-size") = "12px"

            riga = New HtmlTableRow
            riga.Cells.Add(New HtmlTableCell)
            riga.Cells(0).ColSpan = NumeroColonneDati
            If _filtroImpostato <> "" Then
                riga.Cells(0).InnerHtml = _filtroImpostato
            Else
                riga.Cells(0).InnerHtml = "&nbsp;"
            End If
            Me.TableExcel.Rows.Add(riga)
            '-----
            riga = New HtmlTableRow
            riga.Cells.Add(New HtmlTableCell)
            riga.Cells(0).ColSpan = NumeroColonneDati
            riga.Cells(0).InnerHtml = "Periodo di riferimento dal " & CStr(_dataInizio) & " al " & CStr(_dataFine)
            Me.TableExcel.Rows.Add(riga)
            '-----
            riga = New HtmlTableRow
            riga.Cells.Add(New HtmlTableCell)
            riga.Cells(0).ColSpan = NumeroColonneDati
            riga.Cells(0).InnerHtml = "&nbsp;"
            Me.TableExcel.Rows.Add(riga)
            '-----

            If IsNothing(dtFinale) Or dtFinale.Rows.Count = 0 Then
                riga = New HtmlTableRow
                riga.Cells.Add(New HtmlTableCell)
                riga.Cells(0).ColSpan = NumeroColonneDati
                riga.Cells(0).InnerHtml = "Il criterio di filtro non ha prodotto alcun risultato."
                Me.TableExcel.Rows.Add(riga)
                Exit Sub
            End If


            '------------------------------------------
            '------------- INTESTAZIONE ---------------
            '------------------------------------------
            riga = New HtmlTableRow

            For k = 0 To dtFinale.Columns.Count - 1

                riga.Cells.Add(New HtmlTableCell)
                AgronicaCoreDataProvider.UtilityProvider.ElaboraCellaHTML(riga.Cells(k), 2, "", "", "Gainsboro", "center", "middle")
                riga.Cells(k).Style.Item("vertical-align") = "middle"
                riga.Cells(k).Style.Item("font-weight") = "bold"
                riga.Cells(k).Style.Item("font-size") = "12px"
                'Riga.Cells(k).Height = "26"

                'Riga.Cells(k).InnerHtml = dtFinale.Rows(CInt(dtFinale.Rows.Count - 1)).Item(k)
                riga.Cells(k).InnerHtml = dtFinale.Columns.Item(k).Caption

                colonnaDt = riga.Cells(k).InnerHtml

                Select Case colonnaDt

                    Case "NomeAgente"
                        riga.Cells(k).InnerHtml = "Nome Cognome Agente"
                    Case "Tipo_Documento"
                        riga.Cells(k).InnerHtml = "Tipo<br/>Documento"
                    Case "Numero_Documento"
                        riga.Cells(k).InnerHtml = "Numero<br/>Documento"
                    Case "Data_Documento"
                        riga.Cells(k).InnerHtml = "Data<br/>Documento"
                    Case "Totale_Doc"
                        riga.Cells(k).InnerHtml = "Totale<br/>Documento"
                    Case "Cliente"
                        riga.Cells(k).InnerHtml = "Cliente"
                    Case "Articolo"
                        riga.Cells(k).InnerHtml = "Articolo"
                    Case "Qta"
                        riga.Cells(k).InnerHtml = "Quantit&agrave;"
                    Case "Prezzo"
                        riga.Cells(k).InnerHtml = "Prezzo<br/>Unitario"
                    Case "Imponibile"
                        riga.Cells(k).InnerHtml = "Imponibile"
                    Case "Perc_Prov"
                        riga.Cells(k).InnerHtml = "%<br/>Provvigione"
                    Case "Prov"
                        riga.Cells(k).InnerHtml = "Provvigione<br/>Dovuta"
                    Case "Riscosso"
                        riga.Cells(k).InnerHtml = "Importo<br/>Riscosso"
                    Case "Saldato"
                        riga.Cells(k).InnerHtml = "Saldato"

                End Select

            Next

            Me.TableExcel.Rows.Add(riga)
            '----------------------------------
            '------- FINE INTESTAZIONE --------
            '----------------------------------

            '----------------------------------
            '------ RIEMPIMENTO TABELLA -------
            '----------------------------------

            'scorro le righe
            For i = 0 To dtFinale.Rows.Count - 1

                riga = New HtmlTableRow

                'scorro le colonne
                For j = 0 To dtFinale.Columns.Count - 1

                    'aggiungo la cella
                    riga.Cells.Add(New HtmlTableCell)
                    riga.Cells(j).Style.Item("text-align") = "center"
                    riga.Cells(j).Style.Item("vertical-align") = "middle"
                    'Riga.Cells(j).Height = "26"

                    riga.Cells(j).InnerHtml = IIf(Not IsDBNull(dtFinale.Rows(i).Item(j)), dtFinale.Rows(i).Item(j), "&nbsp;")

                    'Select Case dtFinale.Columns.Item(j).Caption.ToLower

                    'Case CStr("aaa").ToLower,
                    '    'aggiungo uno spazio davanti x salvare gli zeri...
                    '    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(dtFinale.Rows(i).Item(j)), "&nbsp;" & dtFinale.Rows(i).Item(j), "&nbsp;")

                    'Case CStr("bbb").ToLower, "numero_conf"
                    '    If dtFinale.Rows(i).Item(j) <> "" Then
                    '        'aggiungo un apice davanti per vedere visualizzato il numero e non una data
                    '        'Riga.Cells(j).InnerHtml = "'" & dtFinale.Rows(i).Item(j)
                    '        Riga.Cells(j).InnerHtml = "&nbsp;" & dtFinale.Rows(i).Item(j)
                    '    End If

                    'Case Else

                    '   riga.Cells(j).InnerHtml = IIf(Not IsDBNull(dtFinale.Rows(i).Item(j)), dtFinale.Rows(i).Item(j), "&nbsp;")

                    'End Select

                Next

                'Aggiungo la Riga alla Tabella 
                Me.TableExcel.Rows.Add(riga)

            Next
            '-------------------------------
            '--------- FINE TABELLA --------
            '-------------------------------

            '-----
            riga = New HtmlTableRow
            riga.Cells.Add(New HtmlTableCell)
            riga.Cells(0).ColSpan = NumeroColonneDati
            riga.Cells(0).InnerHtml = "&nbsp;"
            Me.TableExcel.Rows.Add(riga)
            '-----


            '-------------------------------
            '--------- TOTALI --------
            '-------------------------------
            riga = New HtmlTableRow
            riga.Cells.Add(New HtmlTableCell)
            riga.Cells(0).InnerHtml = "Tot. Imponibile"
            riga.Cells.Add(New HtmlTableCell)
            riga.Cells(1).InnerHtml = "&euro;"
            riga.Cells(1).Style.Item("text-align") = "center"
            riga.Cells.Add(New HtmlTableCell)
            riga.Cells(2).InnerText = Format(totImponibile, "###,###,##0.00")
            riga.Cells(2).Style.Item("mso-number-format") = "0\.00"
            riga.Cells.Add(New HtmlTableCell)
            riga.Cells(3).ColSpan = NumeroColonneDati - 3
            Me.TableExcel.Rows.Add(riga)
            '-----
            riga = New HtmlTableRow
            riga.Cells.Add(New HtmlTableCell)
            riga.Cells(0).InnerHtml = "Tot. Imponibile Incassato"
            riga.Cells.Add(New HtmlTableCell)
            riga.Cells(1).InnerHtml = "&euro;"
            riga.Cells(1).Style.Item("text-align") = "center"
            riga.Cells.Add(New HtmlTableCell)
            riga.Cells(2).InnerText = Format(totImponibileIncassato, "###,###,##0.00")
            riga.Cells(2).Style.Item("mso-number-format") = "0\.00"
            riga.Cells.Add(New HtmlTableCell)
            riga.Cells(3).ColSpan = NumeroColonneDati - 3
            Me.TableExcel.Rows.Add(riga)
            '----
            riga = New HtmlTableRow
            riga.Cells.Add(New HtmlTableCell)
            riga.Cells(0).InnerHtml = "Tot. Imponibile Non Incassato"
            riga.Cells.Add(New HtmlTableCell)
            riga.Cells(1).InnerHtml = "&euro;"
            riga.Cells(1).Style.Item("text-align") = "center"
            riga.Cells.Add(New HtmlTableCell)
            riga.Cells(2).InnerText = Format(totImponibileNonIncassato, "###,###,##0.00")
            riga.Cells(2).Style.Item("mso-number-format") = "0\.00"
            riga.Cells.Add(New HtmlTableCell)
            riga.Cells(3).ColSpan = NumeroColonneDati - 3
            Me.TableExcel.Rows.Add(riga)
            '----
            riga = New HtmlTableRow
            riga.Cells.Add(New HtmlTableCell)
            riga.Cells(0).InnerHtml = "Tot. Provvigione"
            riga.Cells.Add(New HtmlTableCell)
            riga.Cells(1).InnerHtml = "&euro;"
            riga.Cells(1).Style.Item("text-align") = "center"
            riga.Cells.Add(New HtmlTableCell)
            riga.Cells(2).InnerText = Format(totProvv, "###,###,##0.00")
            riga.Cells(2).Style.Item("mso-number-format") = "0\.00"
            riga.Cells.Add(New HtmlTableCell)
            riga.Cells(3).ColSpan = NumeroColonneDati - 3
            Me.TableExcel.Rows.Add(riga)
            '-----
            riga = New HtmlTableRow
            riga.Cells.Add(New HtmlTableCell)
            riga.Cells(0).InnerHtml = "Tot. Provvigione Maturata"
            riga.Cells.Add(New HtmlTableCell)
            riga.Cells(1).InnerHtml = "&euro;"
            riga.Cells(1).Style.Item("text-align") = "center"
            riga.Cells.Add(New HtmlTableCell)
            riga.Cells(2).InnerText = Format(totProvvIncassata, "###,###,##0.00")
            riga.Cells(2).Style.Item("mso-number-format") = "0\.00"
            riga.Cells.Add(New HtmlTableCell)
            riga.Cells(3).ColSpan = NumeroColonneDati - 3
            Me.TableExcel.Rows.Add(riga)
            '-----
            riga = New HtmlTableRow
            riga.Cells.Add(New HtmlTableCell)
            riga.Cells(0).InnerHtml = "Tot. Provvigione non Maturata"
            riga.Cells.Add(New HtmlTableCell)
            riga.Cells(1).InnerHtml = "&euro;"
            riga.Cells(1).Style.Item("text-align") = "center"
            riga.Cells.Add(New HtmlTableCell)
            riga.Cells(2).InnerText = Format(totProvvNonIncassata, "###,###,##0.00")
            riga.Cells(2).Style.Item("mso-number-format") = "0\.00"
            riga.Cells.Add(New HtmlTableCell)
            riga.Cells(3).ColSpan = NumeroColonneDati - 3
            Me.TableExcel.Rows.Add(riga)
            '----

            '-----
            riga = New HtmlTableRow
            riga.Cells.Add(New HtmlTableCell)
            riga.Cells(0).ColSpan = NumeroColonneDati
            riga.Cells(0).InnerHtml = "&nbsp;"
            Me.TableExcel.Rows.Add(riga)
            '-----

            '-----
            riga = New HtmlTableRow
            riga.Cells.Add(New HtmlTableCell)
            riga.Cells(0).ColSpan = NumeroColonneDati
            riga.Cells(0).InnerHtml = "Nota: l'imponibile incassato e la provvigione incassata sono calcolati solo sui documenti riscossi totalmente (colonna SALDATO = SI)."
            Me.TableExcel.Rows.Add(riga)
            '-----


        Catch ex As Exception
            riga = New HtmlTableRow
            riga.Cells.Add(New HtmlTableCell)
            riga.Cells(0).ColSpan = NumeroColonneDati
            riga.Cells(0).InnerHtml = "Creazione Excel: " & ex.Message
            Me.TableExcel.Rows.Add(riga)
        End Try

    End Sub

End Class
