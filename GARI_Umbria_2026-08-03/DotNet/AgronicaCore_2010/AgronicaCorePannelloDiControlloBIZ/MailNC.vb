Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class MailNC

    Public Function programmaMailPerAperturaNC(ByRef objParametri_Server As AgronicaCoreParametri, _
                                           ByRef objParametri_Utenti As AgronicaCoreParametri, _
                                           nc As NC_Testata) As Boolean

        Return programmaMailPerEvento(objParametri_Server, objParametri_Utenti, enum_NC_Eventi.AperturaNC, enum_MailTipo.NonConformita_AperturaNC, DateTime.Now, nc, Nothing)

    End Function

    Public Function programmaMailPerChiusuraNC(ByRef objParametri_Server As AgronicaCoreParametri, _
                                           ByRef objParametri_Utenti As AgronicaCoreParametri, _
                                           nc As NC_Testata) As Boolean

        Return programmaMailPerEvento(objParametri_Server, objParametri_Utenti, enum_NC_Eventi.ChiusuraNC, enum_MailTipo.NonConformita_ChiusuraNC, DateTime.Now, nc, Nothing)

    End Function

    Public Function programmaMailPerAperturaFaseNC(ByRef objParametri_Server As AgronicaCoreParametri, _
                                               ByRef objParametri_Utenti As AgronicaCoreParametri, _
                                            nc As NC_Testata, det As NC_Dettagli) As Boolean

        Return programmaMailPerEvento(objParametri_Server, objParametri_Utenti, enum_NC_Eventi.AperturaFaseNC, enum_MailTipo.NonConformita_AperturaFase, DateTime.Now, nc, det)

    End Function

    Public Function programmaMailPerChiusuraFaseNC(ByRef objParametri_Server As AgronicaCoreParametri, _
                                               ByRef objParametri_Utenti As AgronicaCoreParametri, _
                                               nc As NC_Testata, det As NC_Dettagli) As Boolean

        Return programmaMailPerEvento(objParametri_Server, objParametri_Utenti, enum_NC_Eventi.ChiusuraFaseNC, enum_MailTipo.NonConformita_ChiusuraFase, DateTime.Now, nc, det)

    End Function

    Public Function programmaMailPerFaseNCinScadenza(objParametri_Server As AgronicaCoreParametri, _
                                                     objParametri_Utenti As AgronicaCoreParametri, _
                                                     nc As NC_Testata, det As NC_Dettagli) As Boolean

        Return programmaMailPerEvento(objParametri_Server, objParametri_Utenti, enum_NC_Eventi.FaseNcInScadenza, enum_MailTipo.NonConformita_FaseInScadenza, det.DataChiusuraPrevista, nc, det)

    End Function

    Sub cancellaProgrammazioneMailPerFaseNCinScadenza(objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri, nc As NC_Testata, det As NC_Dettagli)

        Dim mp_W As New AgronicaCoreMailBIZ.Mail_Programmazione_W

        Dim chiave As String = nc.ID_NC & If(IsNothing(det), "", "|" & det.ID_Dettaglio)
        mp_W.CancellaProgrammazioniFromChiaveTestataFaseNC(objParametri_Server, enum_MailTipo.NonConformita_FaseInScadenza, chiave)

    End Sub

    Sub cancellaProgrammazioneMailPerFaseNCinScadenza(objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri, nc As NC_Testata)

        Dim mp_W As New AgronicaCoreMailBIZ.Mail_Programmazione_W

        Dim chiave As String = nc.ID_NC & "|%"
        mp_W.CancellaProgrammazioniFromChiaveTestataNC(objParametri_Server, enum_MailTipo.NonConformita_FaseInScadenza, chiave)

    End Sub

    Sub cancellaProgrammazioneMailPerFaseNCinScadenzaPassandoIdDet(objParametri_Server As AgronicaCoreParametri, id_det As Integer)

        Dim mp_W As New AgronicaCoreMailBIZ.Mail_Programmazione_W

        Dim chiave As String = "%|" & id_det
        mp_W.CancellaProgrammazioniFromChiaveTestataNC(objParametri_Server, enum_MailTipo.NonConformita_FaseInScadenza, chiave)

    End Sub

    Sub cancellaProgrammazioneMailPerFaseNCinScadenzaPassandoIdNc(objParametri_Server As AgronicaCoreParametri, id_nc As Integer)

        Dim mp_W As New AgronicaCoreMailBIZ.Mail_Programmazione_W

        Dim chiave As String = id_nc & "|%"
        mp_W.CancellaProgrammazioniFromChiaveTestataNC(objParametri_Server, enum_MailTipo.NonConformita_FaseInScadenza, chiave)

    End Sub

    Sub aggiornaProgrammazioneMailPerFaseNCinScadenza(objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri, nc As NC_Testata, det As NC_Dettagli)

        cancellaProgrammazioneMailPerFaseNCinScadenza(objParametri_Server, objParametri_Utenti, nc, det)
        programmaMailPerEvento(objParametri_Server, objParametri_Utenti, enum_NC_Eventi.FaseNcInScadenza, enum_MailTipo.NonConformita_FaseInScadenza, det.DataChiusuraPrevista, nc, det)

    End Sub

    Sub aggiornaProgrammazioneMailPerFaseNCinScadenza(objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri, nc As NC_Testata)

        cancellaProgrammazioneMailPerFaseNCinScadenza(objParametri_Server, objParametri_Utenti, nc)

        If Not IsNothing(nc.Dettagli) Then
            For Each det As NC_Dettagli In nc.Dettagli
                If IsNothing(det.DataChiusuraEffettiva) Then
                    programmaMailPerEvento(objParametri_Server, objParametri_Utenti, enum_NC_Eventi.FaseNcInScadenza, enum_MailTipo.NonConformita_FaseInScadenza, det.DataChiusuraPrevista, nc, det)
                End If
            Next
        End If

    End Sub


    Private Function programmaMailPerEvento(ByRef objParametri_Server As AgronicaCoreParametri, _
                                            ByRef objParametri_Utenti As AgronicaCoreParametri, _
                                            ID_Evento As enum_NC_Eventi, _
                                            TipoMail As enum_MailTipo, _
                                            DataSpedizione As DateTime, _
                                            nc As NC_Testata, det As NC_Dettagli _
                                           ) As Boolean

        'Leggo l'area della NC
        Dim cat_R As New NC_Categorie_R
        Dim ncCat As List(Of NC_Categorie) = cat_R.leggi_NC_Categorie(objParametri_Server, nc.ID_Categoria)

        'Leggo le regole per l'alerting
        Dim avv_R As New NC_Avvisi_R
        Dim ncAvv As List(Of NC_Avvisi) = avv_R.leggi_NC_Avvisi(objParametri_Server, Nothing, ncCat(0).Area, ID_Evento, Nothing)

        Dim oggetto As String = ""
        Dim testoMail As String = ""

        Select Case objParametri_Server.PivaSuperUser
            Case "01271980391" ' Fruttagel
                ImpostaOggettoTestoFruttagel(objParametri_Server, objParametri_Utenti, oggetto, testoMail, ID_Evento, nc, det)
            Case Else
                ImpostaOggettoTestoDefault(oggetto, testoMail, ID_Evento, nc)
        End Select

        Dim mp As New AgronicaCoreMailBIZ.Mail_Programmazione_W

        For Each avv As NC_Avvisi In ncAvv

            'Aggiorno la data di spedizione in base ai giorni di attesa
            If IsNumeric(avv.GGAttesa) Then
                DataSpedizione = DataSpedizione.AddDays(avv.GGAttesa)

                'Se la data è già passata e non è un invio immediato, ignoro la regola di avviso
                If avv.GGAttesa <> 0 AndAlso DataSpedizione < DateTime.Now Then
                    Continue For
                End If

            End If

            'Inizializzo ed estraggo gli indirizzi mail
            Dim strMailA As String = ""
            Dim strMailCC As String = ""
            Dim strMailCCN As String = ""
            Dim Filtro_RapConMail = avv.Filtro_RapCon
            estraiIndirizziMailDestinatari(avv, nc, det, strMailA, strMailCC, strMailCCN, objParametri_Utenti)

            Dim chiave As String = nc.ID_NC & If(IsNothing(det), "", "|" & det.ID_Dettaglio)
            Dim res As Boolean = mp.ScriviNuovaMail(objParametri_Server, TipoMail,
                                    chiave, avv.MailMittente, strMailA, strMailCC, strMailCCN,
                                    oggetto, testoMail, True, "", DataSpedizione,,,,, Filtro_RapConMail)

        Next

        Return True

    End Function

    Private Sub ImpostaOggettoTestoFruttagel(ByRef objParametri_Server As AgronicaCoreParametri, _
                                             ByRef objParametri_Utenti As AgronicaCoreParametri, _
                                             ByRef oggetto As String, ByRef testoMail As String, _
                                             ID_Evento As enum_NC_Eventi, _
                                             nc As NC_Testata, det As NC_Dettagli)

        Dim str_CatArea As String = ""
        Dim str_CatTipologia As String = ""
        GetAreaTipologiaCategoria(If(IsNumeric(nc.ID_Categoria), nc.ID_Categoria, ""), str_CatArea, str_CatTipologia, objParametri_Server)

        'Imposto Oggetto e Testo della Mail
        Select Case ID_Evento
            Case TipiEnumerativi.enum_NC_Eventi.AperturaNC
                Dim str_UtenteNC As String = GetNomeUtente(nc.Utente, objParametri_Utenti)

                oggetto = "Aperta NC: " & nc.CodiceNC & ", " & nc.CodiceProdotto & " - " & nc.DescrizioneProdotto
                testoMail &= aggiungiRigaParametro("Codice NC", nc.CodiceNC, True)
                testoMail &= aggiungiRigaParametro("Categoria (Area)", str_CatTipologia & " (" & str_CatArea & ")", True)
                testoMail &= aggiungiRigaParametro("Descrizione NC", nc.Descrizione, True)
                testoMail &= aggiungiRigaParametro("Utente", str_UtenteNC, True)
                testoMail &= aggiungiRigaParametro("Data Apertura", If(IsDate(nc.Data_Apertura), CDate(nc.Data_Apertura).ToShortDateString, Nothing), True)
                testoMail &= aggiungiRigaParametro("Codice Prodotto", nc.CodiceProdotto, True)
                testoMail &= aggiungiRigaParametro("Descrizione Prodotto", nc.DescrizioneProdotto, True)
                testoMail &= aggiungiRigaParametro("Lotto JDE", nc.LottoJDE, True)
                testoMail &= aggiungiRigaParametro("Lotto Fruttagel", nc.LottoFruttagel, True)
                testoMail &= aggiungiRigaParametro("Lotto Fornitore", nc.LottoFornitore, True)
                testoMail &= aggiungiRigaParametro("Data Arrivo Prodotto", If(IsDate(nc.DataArrivoProdotto), CDate(nc.DataArrivoProdotto).ToShortDateString, Nothing), True)
                testoMail &= aggiungiRigaParametro("Q.tà Bloccata", nc.QtaBloccati_Qta, True)
                testoMail &= aggiungiRigaParametro("UM", nc.QtaBloccati_UdM, True)
                testoMail &= aggiungiRigaParametro("Progressivi Bloccati", nc.QtaBloccati_Progressivi, True)
            Case TipiEnumerativi.enum_NC_Eventi.ChiusuraNC
                Dim str_CausaChiusura As String = GetNomeCausaChiusura(If(IsNumeric(nc.ID_CausaChiusura), nc.ID_CausaChiusura, ""), objParametri_Server)

                oggetto = "Chiusa NC: " & nc.CodiceNC & ", " & nc.CodiceProdotto & " - " & nc.DescrizioneProdotto
                testoMail &= aggiungiRigaParametro("Codice NC", nc.CodiceNC, True)
                testoMail &= aggiungiRigaParametro("Categoria (Area)", str_CatTipologia & " (" & str_CatArea & ")", True)
                testoMail &= aggiungiRigaParametro("Descrizione NC", nc.Descrizione, True)
                testoMail &= aggiungiRigaParametro("Codice Prodotto", nc.CodiceProdotto, True)
                testoMail &= aggiungiRigaParametro("Descrizione Prodotto", nc.DescrizioneProdotto, True)
                testoMail &= aggiungiRigaParametro("Lotto JDE", nc.LottoJDE, True)
                testoMail &= aggiungiRigaParametro("Lotto Fruttagel", nc.LottoFruttagel, True)
                testoMail &= aggiungiRigaParametro("Lotto Fornitore", nc.LottoFornitore, True)
                testoMail &= aggiungiRigaParametro("Data Arrivo Prodotto", If(IsDate(nc.DataArrivoProdotto), CDate(nc.DataArrivoProdotto).ToShortDateString, Nothing), True)
                testoMail &= aggiungiRigaParametro("Data Chiusura", If(IsDate(nc.Data_Chiusura), CDate(nc.Data_Chiusura).ToShortDateString, Nothing), True)
                testoMail &= aggiungiRigaParametro("Causa Chiusura", str_CausaChiusura, True)
            Case TipiEnumerativi.enum_NC_Eventi.AperturaFaseNC
                Dim str_Fase_Resp As String = GetNomeUtente(det.Responsabile, objParametri_Utenti)
                Dim str_CodiceDettaglio As String = If(IsNothing(det.CodiceDettaglio), "", " n° " & det.CodiceDettaglio & " ")

                oggetto = "Aperto TRATTAMENTO" & str_CodiceDettaglio & " in NC: " & nc.CodiceNC & ", " & nc.CodiceProdotto & " - " & nc.DescrizioneProdotto
                testoMail &= "<h1>RIFERIMENTO NC</h1>"
                testoMail &= aggiungiRigaParametro("Codice NC", nc.CodiceNC, True)
                testoMail &= aggiungiRigaParametro("Categoria (Area)", str_CatTipologia & " (" & str_CatArea & ")", True)
                testoMail &= aggiungiRigaParametro("Descrizione NC", nc.Descrizione, True)
                testoMail &= aggiungiRigaParametro("Codice Prodotto", nc.CodiceProdotto, True)
                testoMail &= aggiungiRigaParametro("Descrizione Prodotto", nc.DescrizioneProdotto, True)
                testoMail &= aggiungiRigaParametro("Lotto JDE", nc.LottoJDE, True)
                testoMail &= aggiungiRigaParametro("Lotto Fruttagel", nc.LottoFruttagel, True)
                testoMail &= aggiungiRigaParametro("Lotto Fornitore", nc.LottoFornitore, True)
                testoMail &= aggiungiRigaParametro("Data Arrivo Prodotto", If(IsDate(nc.DataArrivoProdotto), CDate(nc.DataArrivoProdotto).ToShortDateString, Nothing), True)
                testoMail &= "<br><h1>TRATTAMENTO NC</h1>"
                testoMail &= aggiungiRigaParametro("Responsabile", str_Fase_Resp, True)
                testoMail &= aggiungiRigaParametro("Coinvolti", det.PersoneCoinvolte, True)
                testoMail &= aggiungiRigaParametro("Descrizione Fase", det.Descrizione, True)
                testoMail &= aggiungiRigaParametro("Data Chiusura Prevista", If(IsDate(det.DataChiusuraPrevista), CDate(det.DataChiusuraPrevista).ToShortDateString, Nothing), True)
            Case TipiEnumerativi.enum_NC_Eventi.ChiusuraFaseNC
                Dim str_Fase_Resp As String = GetNomeUtente(det.Responsabile, objParametri_Utenti)
                Dim str_CodiceDettaglio As String = If(IsNothing(det.CodiceDettaglio), "", " n° " & det.CodiceDettaglio & " ")

                oggetto = "Chiuso TRATTAMENTO" & str_CodiceDettaglio & " in NC: " & nc.CodiceNC & ", " & nc.CodiceProdotto & " - " & nc.DescrizioneProdotto
                testoMail &= "<h1>RIFERIMENTO NC</h1>"
                testoMail &= aggiungiRigaParametro("Codice NC", nc.CodiceNC, True)
                testoMail &= aggiungiRigaParametro("Categoria (Area)", str_CatTipologia & " (" & str_CatArea & ")", True)
                testoMail &= aggiungiRigaParametro("Descrizione NC", nc.Descrizione, True)
                testoMail &= aggiungiRigaParametro("Codice Prodotto", nc.CodiceProdotto, True)
                testoMail &= aggiungiRigaParametro("Descrizione Prodotto", nc.DescrizioneProdotto, True)
                testoMail &= aggiungiRigaParametro("Lotto JDE", nc.LottoJDE, True)
                testoMail &= aggiungiRigaParametro("Lotto Fruttagel", nc.LottoFruttagel, True)
                testoMail &= aggiungiRigaParametro("Lotto Fornitore", nc.LottoFornitore, True)
                testoMail &= aggiungiRigaParametro("Data Arrivo Prodotto", If(IsDate(nc.DataArrivoProdotto), CDate(nc.DataArrivoProdotto).ToShortDateString, Nothing), True)
                testoMail &= "<br><h1>TRATTAMENTO NC</h1>"
                testoMail &= aggiungiRigaParametro("Responsabile", str_Fase_Resp, True)
                testoMail &= aggiungiRigaParametro("Coinvolti", det.PersoneCoinvolte, True)
                testoMail &= aggiungiRigaParametro("Descrizione Fase", det.Descrizione, True)
                testoMail &= aggiungiRigaParametro("Data Chiusura Effettiva", If(IsDate(det.DataChiusuraEffettiva), CDate(det.DataChiusuraEffettiva).ToShortDateString, Nothing), True)
            Case TipiEnumerativi.enum_NC_Eventi.FaseNcInScadenza
                Dim str_Fase_Resp As String = GetNomeUtente(det.Responsabile, objParametri_Utenti)
                Dim str_CodiceDettaglio As String = If(IsNothing(det.CodiceDettaglio), "", " n° " & det.CodiceDettaglio & " ")

                oggetto = "SCADENZA TRATTAMENTO" & str_CodiceDettaglio & " in NC: " & nc.CodiceNC & ", " & nc.CodiceProdotto & " - " & nc.DescrizioneProdotto
                testoMail &= "<h1>RIFERIMENTO NC</h1>"
                testoMail &= aggiungiRigaParametro("Codice NC", nc.CodiceNC, True)
                testoMail &= aggiungiRigaParametro("Categoria (Area)", str_CatTipologia & " (" & str_CatArea & ")", True)
                testoMail &= aggiungiRigaParametro("Descrizione NC", nc.Descrizione, True)
                testoMail &= aggiungiRigaParametro("Codice Prodotto", nc.CodiceProdotto, True)
                testoMail &= aggiungiRigaParametro("Descrizione Prodotto", nc.DescrizioneProdotto, True)
                testoMail &= aggiungiRigaParametro("Lotto JDE", nc.LottoJDE, True)
                testoMail &= aggiungiRigaParametro("Lotto Fruttagel", nc.LottoFruttagel, True)
                testoMail &= aggiungiRigaParametro("Lotto Fornitore", nc.LottoFornitore, True)
                testoMail &= aggiungiRigaParametro("Data Arrivo Prodotto", If(IsDate(nc.DataArrivoProdotto), CDate(nc.DataArrivoProdotto).ToShortDateString, Nothing), True)
                testoMail &= "<br><h1>TRATTAMENTO NC</h1>"
                testoMail &= aggiungiRigaParametro("Responsabile", str_Fase_Resp, True)
                testoMail &= aggiungiRigaParametro("Coinvolti", det.PersoneCoinvolte, True)
                testoMail &= aggiungiRigaParametro("Descrizione Fase", det.Descrizione, True)
                testoMail &= aggiungiRigaParametro("Data Chiusura Prevista", If(IsDate(det.DataChiusuraPrevista), "<b>" & CDate(det.DataChiusuraPrevista).ToShortDateString & "</b>", ""), True)
            Case Else
                oggetto = "Evento Generico NC"
                testoMail = "Errore, il codice evento non è stato gestito: " & ID_Evento
        End Select

        'testoMail &= "<br><br><br>Mail inviata automaticamente da GIAS di Agronica per Fruttagel in data " & DateTime.Now.ToString

    End Sub

    Private Function aggiungiRigaParametro(nomeParametro As String, parametro As Object, aggiungiSoloSeValorizzato As Boolean)
        If aggiungiSoloSeValorizzato Then
            Return If(IsNothing(parametro) OrElse parametro.ToString() = "", "", "<b>" & nomeParametro & ": </b>" & parametro.ToString() & "<br>")
        Else
            Return "<b>" & nomeParametro & ": </b>" & parametro.ToString() & "<br>"
        End If
    End Function

    Private Sub ImpostaOggettoTestoDefault(ByRef oggetto As String, ByRef testoMail As String, _
                                            ID_Evento As enum_NC_Eventi, _
                                            nc As NC_Testata)

        Select Case ID_Evento
            Case TipiEnumerativi.enum_NC_Eventi.AperturaNC
                oggetto = "Aperta nuova NC: " & nc.CodiceNC
                testoMail = "Default"
            Case TipiEnumerativi.enum_NC_Eventi.ChiusuraNC
                oggetto = "Chiusa una NC: "
                testoMail = "Default"
            Case TipiEnumerativi.enum_NC_Eventi.AperturaFaseNC
                oggetto = "Aperta nuova fase in NC: "
                testoMail = "Default"
            Case TipiEnumerativi.enum_NC_Eventi.ChiusuraFaseNC
                oggetto = "Chiusa una fase in NC: "
                testoMail = "Default"
            Case TipiEnumerativi.enum_NC_Eventi.FaseNcInScadenza
                oggetto = "SCADENZA fase in NC: "
                testoMail = "Default"
            Case Else
                oggetto = "Evento Generico NC"
                testoMail = "Errore, il codice evento non è stato gestito: " & ID_Evento
        End Select

    End Sub

    Private Function GetIndirizzoMailResponsabile(cfResponsabile As String, objParametri_Utenti As AgronicaCoreParametri)
        Dim email As String = ""

        If Not IsNothing(cfResponsabile) AndAlso cfResponsabile.Trim.Length > 0 Then

            Dim ut_R As New AgronicaCoreUtentiDAL.Utenti_Dettagli_R
            Dim dtNC_Fase_Resp As DataTable = ut_R.Utenti_Dettagli_from_CF(cfResponsabile, _
                        AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, _
                        "", "", objParametri_Utenti)

            If dtNC_Fase_Resp.Rows.Count > 0 Then
                email = dtNC_Fase_Resp.Rows(0).Item("Email").trim()
            End If

        End If

        Return email
    End Function

    Private Function GetNomeUtente(cfUtente As String, objParametri_Utenti As AgronicaCoreParametri) As String

        Dim utente As String = ""
        If Not IsNothing(cfUtente) AndAlso cfUtente <> "" Then
            Dim ut_R As New AgronicaCoreUtentiDAL.Utenti_Dettagli_R
            Dim dtNC_Ut As DataTable = ut_R.Utenti_Dettagli_from_CF(cfUtente, _
                                            AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, _
                                            "", "", objParametri_Utenti)

            If dtNC_Ut.Rows(0).Item("Flag_Azienda_Persona") = "1" Then
                utente = dtNC_Ut.Rows(0).Item("Rag_Soc")
            Else
                utente = dtNC_Ut.Rows(0).Item("Nome") & " " & dtNC_Ut.Rows(0).Item("Cognome")
            End If
        End If

        Return utente

    End Function

    Private Function GetNomeCausaChiusura(ID_CausaChiusura As String, objParametri_Server As AgronicaCoreParametri) As String

        Dim nomeCausaChiusura As String = ""
        If IsNumeric(ID_CausaChiusura) Then
            Dim cc_r As New NC_CauseChiusura_R
            Dim cc As NC_CauseChiusura = cc_r.leggi_NC_CauseChiusura(objParametri_Server, ID_CausaChiusura).First
            nomeCausaChiusura = cc.Nome
        End If

        Return nomeCausaChiusura

    End Function

    Private Sub GetAreaTipologiaCategoria(ID_Categoria As String, ByRef catArea As String, ByRef catTipologia As String, objParametri_Server As AgronicaCoreParametri)
        If IsNumeric(ID_Categoria) Then
            Dim cat_r As New NC_Categorie_R
            Dim cat As List(Of NC_Categorie) = cat_r.leggi_NC_Categorie(objParametri_Server, ID_Categoria)

            catArea = cat(0).Area
            catTipologia = cat(0).Tipologia
        End If
    End Sub

    Private Sub estraiIndirizziMailDestinatari(avv As NC_Avvisi, nc As NC_Testata, det As NC_Dettagli, ByRef strMailA As String, ByRef strMailCC As String, ByRef strMailCCN As String, objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri)

        'Estraggo le mail
        strMailA = avv.MailA.Trim()
        strMailCC = avv.MailCC.Trim()
        strMailCCN = ""

        'Capisco se devo includere in A o CC il responsabile
        Dim inA As Boolean = Not IsNothing(avv.MailA_IncludiResponsabile) AndAlso avv.MailA_IncludiResponsabile = True
        Dim inCC As Boolean = Not IsNothing(avv.MailCC_IncludiResponsabile) AndAlso avv.MailCC_IncludiResponsabile = True

        If inA OrElse inCC Then

            'Se c'è un dettaglio vuol dire che il dettaglio è l'oggetto della modifica, quindi prendo il suo responsabile
            If Not IsNothing(det) Then

                'Cerco l'indirizzo email del responsabile
                Dim strMailResponsabile As String = GetIndirizzoMailResponsabile(det.Responsabile, objParametri_Utenti)

                'Aggiungo l'indirizzo email del responsabile tra i destinatari
                If strMailResponsabile.Length > 0 Then

                    If inA Then
                        strMailA &= If(strMailA.Length = 0, strMailResponsabile, ", " & strMailResponsabile & " ")
                    End If

                    If inCC Then
                        strMailCC &= If(strMailCC.Length = 0, strMailResponsabile, ", " & strMailResponsabile & " ")
                    End If

                End If

            Else

                'Vuol dire che l'oggetto della modifica è la testata quindi metto tutti i responsabili
                For Each d As NC_Dettagli In nc.Dettagli

                    'Cerco l'indirizzo email del responsabile
                    Dim strMailResponsabile As String = GetIndirizzoMailResponsabile(d.Responsabile, objParametri_Utenti)

                    'Aggiungo l'indirizzo email del responsabile tra i destinatari
                    If strMailResponsabile.Length > 0 Then

                        If inA Then
                            strMailA &= If(strMailA.Length = 0, strMailResponsabile, ", " & strMailResponsabile & " ")
                        End If

                        If inCC Then
                            strMailCC &= If(strMailCC.Length = 0, strMailResponsabile, ", " & strMailResponsabile & " ")
                        End If

                    End If

                Next

            End If

        End If

    End Sub


End Class
