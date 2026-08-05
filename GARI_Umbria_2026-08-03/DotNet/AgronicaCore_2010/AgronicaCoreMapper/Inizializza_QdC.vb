Imports AgronicaControlli_2010
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModelsSTD.attivita.Attivita
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDTOStd.InData.Agenda
Imports AgronicaCoreModelsSTD.attivita.dettagli
Imports AgronicaCoreModelsSTD.costanti
Imports AgronicaCoreMetaSchemaDAL
Imports AgronicaCoreModelsSTD.attivita
Imports AgronicaCoreModelsSTD.exceptions


Public Class Inizializza_QdC

    Dim obj_Inizializza_QdC As New AgronicaCoreDTOStd.InData.Agenda.Inizializza_QdC

    Public Function Inizializza_QdC_NG(InDataInizializza As AgronicaCoreDTOStd.InData.Agenda.LeggiInizializza_QdC,
                                       objParametri_Super_Server As AgronicaCoreParametri,
                                        objParametri_Server As AgronicaCoreParametri,
                                        objParametri_Utenti As AgronicaCoreParametri) As rispostaStandard(Of AgronicaCoreDTOStd.InData.Agenda.Inizializza_QdC)

        Dim objAnagrafeDAL As New AgronicaCoreAnagrafeDAL.Imprese_Read

        Dim objFabbricatoBIZ As New AgronicaCoreAnagrafeBIZ.Fabbricato_R

        Dim leggiLingua As New Lingue_Read
        Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod,
                                                                  AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                  "", "", objParametri_Server)

        Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
        Threading.Thread.CurrentThread.CurrentUICulture = New System.Globalization.CultureInfo(linguaCodiceISO)



        Dim r As New rispostaStandard(Of AgronicaCoreDTOStd.InData.Agenda.Inizializza_QdC)

        r.RispostaOK = True

        Dim objMapper As New AgronicaCoreMapper.Utility

        obj_Inizializza_QdC.flagNuovoControlloRiduzioneDiserbo = objMapper.Leggi_FlagNuovoControlloRiduzioneDiserbo(objParametri_Server)

        Dim tipoOperazione = STD_Utility.getTipoOperazione(InDataInizializza.tipoAttivita, InDataInizializza.statoAttivita)

        Dim Piva As String = InDataInizializza.impresa.partitaIva


        InizializzaData(Piva, InDataInizializza, tipoOperazione, objParametri_Server, objParametri_Utenti, r)

        'Controllo se ci sono delle Pive in questo archivio con delle Agenzie se sono nelle ricette

        If tipoOperazione = enum_Tipo_Operazione_Agenda.Ricetta AndAlso
            InDataInizializza.tipoRicetta = Tipo_Ricetta.Standard_Destinazioni Then

            Dim listaPivaAgenzie = objAnagrafeDAL.Imprese_Leggi_VisibilitaUtente_Agenzie_PIVA(objParametri_Server, objParametri_Utenti)

            If Not IsNothing(listaPivaAgenzie) AndAlso listaPivaAgenzie.Count > 0 Then
                obj_Inizializza_QdC.ListaPivaAgenzie = listaPivaAgenzie
            Else
                obj_Inizializza_QdC.ListaPivaAgenzie = New List(Of String)
            End If

        End If


        Dim listaFabbricati_Uso_da_terzi = objFabbricatoBIZ.Fabbricati_con_Uso_da_Terzi_Visibilita_Utente(objParametri_Server, objParametri_Utenti)

        If Not IsNothing(listaFabbricati_Uso_da_terzi) AndAlso listaFabbricati_Uso_da_terzi.Count > 0 Then

            'Escludo tra la lista dei fabbricati esterni quelli che sono dell'azienda con cui sto facendo l'Operazione di Campagna
            Dim listaFabbricati_Uso_da_terzi_diversi_da_Azienda_Campagna = listaFabbricati_Uso_da_terzi.Where(Function(f) f.primaryKey.centroAziendalePK.partitaIva <> Piva)

            If Not IsNothing(listaFabbricati_Uso_da_terzi_diversi_da_Azienda_Campagna) AndAlso listaFabbricati_Uso_da_terzi_diversi_da_Azienda_Campagna.Count > 0 Then
                obj_Inizializza_QdC.ListaFabbricatiConUsodaTerzi = listaFabbricati_Uso_da_terzi_diversi_da_Azienda_Campagna.ToList()
            Else
                obj_Inizializza_QdC.ListaFabbricatiConUsodaTerzi = New List(Of AgronicaCoreModelsSTD.anagrafiche.Fabbricato)
            End If

        Else
            obj_Inizializza_QdC.ListaFabbricatiConUsodaTerzi = New List(Of AgronicaCoreModelsSTD.anagrafiche.Fabbricato)
        End If

        Ottieni_Giacenze_Prodotti(InDataInizializza, objParametri_Server, objParametri_Utenti)

        obj_Inizializza_QdC.ListaLavCodNonGestitiSuAPP = STR_OP_NON_GESTITE_APP.Split(",").AsEnumerable().Select(Function(s) Convert.ToInt32(s)).ToList()

        Dim objUtility As New AgronicaCoreMapper.Utility

        obj_Inizializza_QdC.pua = objUtility.Recupera_Pua_Valido_Alla_Data(Piva, InDataInizializza.data, LAVCOD_DISTRIBUZIONE_AMMENDANTI,
                                                                              InDataInizializza.tipoAttivita, InDataInizializza.tipoRicetta, InDataInizializza.statoAttivita,
                                                                              InDataInizializza.disciplinare,
                                                                              objParametri_Server)

        r.RispostaStringa = obj_Inizializza_QdC


        Return r

    End Function

    Private Sub InizializzaData(ByVal Piva As String,
                                ByVal LeggiInizializza_QdC As AgronicaCoreDTOStd.InData.Agenda.LeggiInizializza_QdC,
                                ByVal TipoOperazione As enum_Tipo_Operazione_Agenda,
                                ByVal objParametri_Server As AgronicaCoreParametri,
                                ByVal objParametri_Utenti As AgronicaCoreParametri,
                                ByRef r As rispostaStandard(Of AgronicaCoreDTOStd.InData.Agenda.Inizializza_QdC))

        'Dim objMapper As New AgronicaCoreMapper.Utility
        Dim Utility As New AgronicaCoreVarieBIZ.SottoscrizioneServizioQDC

        obj_Inizializza_QdC.Data = LeggiInizializza_QdC.data

        Dim SportelloAperto As Boolean = True
        Dim dataMin As Date = AGRODATAINIZIO
        Dim dataMax As Date = AGRODATAFINE

        Dim AziendaInVerifica As Boolean = False
        Dim dataMinxVerifica = AGRODATAINIZIO
        Dim dataMaxxVerifica = AGRODATAFINE

        If LeggiInizializza_QdC.tipoOperazioneDB = enum_TipoOperazioneDB.Scrittura OrElse
            LeggiInizializza_QdC.tipoOperazioneDB = enum_TipoOperazioneDB.Modifica Then


            Dim Servizio_cod As Integer = enum_Servizi.Quaderno_Campagna_Caa

            If LeggiInizializza_QdC.operazioni.Exists(Function(o) {LAVCOD_DISTRIBUZIONE_CONCIME, LAVCOD_DISTRIBUZIONE_AMMENDANTI, LAVCOD_CONCIMAZIONE_FOGLIARE, LAVCOD_FERTIRRIGAZIONE, LAVCOD_FERTILIZZAZIONI_DICHIARAZIONE_NON_UTILIZZO}.Contains(o.primaryKey.codice)) Then
                If LeggiInizializza_QdC.tipoRicetta = enum_TipoRicetta.PianoDistribuzionePua Then
                    Servizio_cod = enum_Servizi.PUA
                End If
            End If

            Dim objPraticheBIZ As New AgronicaCoreProfilazioneBIZ.Pratiche_R

            'Azienda in Verifica
            objPraticheBIZ.Limitazione_Data_Per_VerificaInCorso(Piva,
                                                                   Servizio_cod,
                                                                   LeggiInizializza_QdC.data,
                                                                   AziendaInVerifica,
                                                                   dataMinxVerifica,
                                                                   dataMaxxVerifica,
                                                                   objParametri_Server,
                                                                   objParametri_Utenti)


            'Controllo Sportello
            objPraticheBIZ.Data_Sportello_Da_Servizio(Piva,
                                                       Servizio_cod,
                                                       LeggiInizializza_QdC.data,
                                                       SportelloAperto,
                                                       dataMin,
                                                       dataMax,
                                                       objParametri_Server,
                                                       objParametri_Utenti)

            If Servizio_cod = enum_Servizi.Quaderno_Campagna_Caa Then
                objPraticheBIZ.VerificaInCorso_ChiamataSecondaria_SeNessunCambiamento(Piva,
                                                                                        enum_Servizi.QuadernoCampagnaBio,
                                                                                         LeggiInizializza_QdC.data,
                                                                                        False,
                                                                                        AGRODATAINIZIO,
                                                                                        AGRODATAFINE,
                                                                                        AziendaInVerifica,
                                                                                        dataMinxVerifica,
                                                                                        dataMaxxVerifica,
                                                                                        objParametri_Server,
                                                                                        objParametri_Utenti)

                objPraticheBIZ.Sportello_ChiamataSecondaria_SeNessunCambiamento(Piva,
                                                                            enum_Servizi.QuadernoCampagnaBio,
                                                                            LeggiInizializza_QdC.data,
                                                                            True,
                                                                            AGRODATAINIZIO,
                                                                            AGRODATAFINE,
                                                                            SportelloAperto,
                                                                            dataMin,
                                                                            dataMax,
                                                                            objParametri_Server,
                                                                            objParametri_Utenti)
            End If
            If dataMinxVerifica > dataMin Then
                dataMin = dataMinxVerifica
            End If

            If LeggiInizializza_QdC.tipoOperazioneDB = enum_TipoOperazioneDB.Scrittura Then

                If LeggiInizializza_QdC.data < dataMin Then
                    obj_Inizializza_QdC.Data = dataMin
                End If

                If LeggiInizializza_QdC.data > dataMax Then
                    obj_Inizializza_QdC.Data = dataMax
                End If

            End If

            If Not SportelloAperto OrElse (LeggiInizializza_QdC.data < dataMin OrElse LeggiInizializza_QdC.data > dataMax) AndAlso
                AziendaInVerifica = False Then

                If LeggiInizializza_QdC.tipoOperazioneDB = enum_TipoOperazioneDB.Scrittura Then

                    r.RispostaOK = False
                    r.Errore = AgronicaCoreDataProvider.My.Resources.Gias.Errore
                    r.ErroriGias.Add(New ErroreGias With {
                                        .severity = ErroreGias_Severity.Bloccante,
                                        .tipo = ErroreGias_Tipo.Generico,
                                        .messaggio = " " & My.Resources.AgronicaCoreMapper.NonPossibileInserireOperazioneSportelloChiuso & " "
                                    })

                End If

            ElseIf AziendaInVerifica Then

                If LeggiInizializza_QdC.tipoOperazioneDB = enum_TipoOperazioneDB.Scrittura Then

                    r.RispostaOK = False
                    r.Errore = AgronicaCoreDataProvider.My.Resources.Gias.Errore
                    r.ErroriGias.Add(New ErroreGias With {
                                        .severity = ErroreGias_Severity.Bloccante,
                                        .tipo = ErroreGias_Tipo.Generico,
                                        .messaggio = " " & My.Resources.AgronicaCoreMapper.NonPossibileInserireOperazioneAziendaInVerifica & " "
                                    })

                End If

            End If

        End If

        obj_Inizializza_QdC.SportelloAperto = SportelloAperto

        obj_Inizializza_QdC.AziendaInVerifica = AziendaInVerifica

        obj_Inizializza_QdC.Data_Min = dataMin

        obj_Inizializza_QdC.Data_Max = dataMax

        obj_Inizializza_QdC.PraticaTrovata = True

        If TipoOperazione = enum_Tipo_Operazione_Agenda.QuadernoDiCampagna Then
            obj_Inizializza_QdC.PraticaTrovata = Utility.VerificaSottoscrizioneServizioQDC(Piva, LeggiInizializza_QdC.data, objParametri_Server)
        End If

    End Sub



    Private Sub Ottieni_Giacenze_Prodotti(ByVal LeggiInizializza_QdC As AgronicaCoreDTOStd.InData.Agenda.LeggiInizializza_QdC,
                                          ByVal objParametri_Server As AgronicaCoreParametri,
                                          ByVal objParametri_Utenti As AgronicaCoreParametri)

        obj_Inizializza_QdC.ListaGiacenzeXProdotto = New List(Of AgronicaCoreDTOStd.InData.Agenda.GiacenzeXProdotto)

        If Not IsNothing(LeggiInizializza_QdC.lista_Attivita) AndAlso LeggiInizializza_QdC.lista_Attivita.Count > 0 Then

            Dim lista_magazzini As New List(Of AgronicaCoreModelsSTD.anagrafiche.Fabbricato)

            Dim lista_dettagliSemina As New List(Of DettaglioSemina)

            Dim lista_dettagliFertilizzazione As New List(Of DettaglioFertilizzazione)

            Dim lista_dettagliTrattamento As New List(Of DettaglioTrattamento)

            For Each attivita In LeggiInizializza_QdC.lista_Attivita

                If Not IsNothing(attivita.risorse) AndAlso attivita.risorse.Count > 0 Then

                    For Each risorsa In attivita.risorse
                        If risorsa.classType = ClassType.DettaglioTrattamento Then

                            Dim dettaglioTrattamento = CType(risorsa, AgronicaCoreModelsSTD.attivita.dettagli.DettaglioTrattamento)


                            If Not IsNothing(dettaglioTrattamento) Then

                                If Not IsNothing(dettaglioTrattamento.MagazziniMovimentazioni) Then

                                    Dim Index = obj_Inizializza_QdC.ListaGiacenzeXProdotto.FindIndex(Function(g) g.Categoria_Magazzino = FORMULATI AndAlso Not IsNothing(g.dettaglioTrattamento) AndAlso
                                                                                                                    g.dettaglioTrattamento.prodotto.codice = dettaglioTrattamento.prodotto.codice AndAlso
                                                                                                                   g.dettaglioTrattamento.dettaglioProdotto = dettaglioTrattamento.dettaglioProdotto AndAlso
                                                                                                                   g.dettaglioTrattamento.tipoFormulato = dettaglioTrattamento.tipoFormulato)

                                    If Index = -1 Then

                                        Dim new_dettaglioTrattamento As DettaglioTrattamento = dettaglioTrattamento.Clona()



                                        For Each MagazziniMovimentazioni In new_dettaglioTrattamento.MagazziniMovimentazioni

                                            MagazziniMovimentazioni.Qta = 0

                                            MagazziniMovimentazioni.QtaTot = 0

                                        Next

                                        obj_Inizializza_QdC.ListaGiacenzeXProdotto.Add(New GiacenzeXProdotto With {
                                                            .Categoria_Magazzino = FORMULATI,
                                                            .Operazione = attivita.job,
                                                            .dettaglioTrattamento = new_dettaglioTrattamento,
                                                            .dettaglioFertilizzazione = Nothing,
                                                            .dettaglioSemina = Nothing
                                                       })
                                    Else
                                        For Each magazzinoMovimentazione In dettaglioTrattamento.MagazziniMovimentazioni
                                            Dim Index_MagazzinoMovimentazione = obj_Inizializza_QdC.ListaGiacenzeXProdotto(Index).dettaglioTrattamento.MagazziniMovimentazioni.FindIndex(Function(m) m.Magazzino.primaryKey.centroAziendalePK.partitaIva = magazzinoMovimentazione.Magazzino.primaryKey.centroAziendalePK.partitaIva AndAlso
                                                                                                                                                                  m.Magazzino.primaryKey.centroAziendalePK.codice = magazzinoMovimentazione.Magazzino.primaryKey.centroAziendalePK.codice AndAlso
                                                                                                                                                                   m.Magazzino.primaryKey.codice = magazzinoMovimentazione.Magazzino.primaryKey.codice AndAlso
                                                                                                                                                                   m.Lotto = magazzinoMovimentazione.Lotto)

                                            If Index_MagazzinoMovimentazione = -1 Then

                                                Dim new_magazzinoMovimentazione As AgronicaCoreModelsSTD.attivita.RilevamentoDiMagazzino = magazzinoMovimentazione

                                                new_magazzinoMovimentazione.Qta = 0

                                                new_magazzinoMovimentazione.QtaTot = 0

                                                obj_Inizializza_QdC.ListaGiacenzeXProdotto(Index).dettaglioTrattamento.MagazziniMovimentazioni.Add(new_magazzinoMovimentazione)
                                            End If
                                        Next

                                    End If
                                End If


                                If Not IsNothing(dettaglioTrattamento.avversitaGruppo) AndAlso Not IsNothing(dettaglioTrattamento.avversitaGruppo.MagazziniMovimentazioni) Then


                                    Dim Index = obj_Inizializza_QdC.ListaGiacenzeXProdotto.FindIndex(Function(g) g.Categoria_Magazzino = INNESCHI AndAlso Not IsNothing(g.avversitaGruppo) AndAlso
                                                                                                                    g.avversitaGruppo.codice = dettaglioTrattamento.avversitaGruppo.codice)

                                    If Index = -1 Then

                                        Dim new_avversitaGruppo As AgronicaCoreModelsSTD.metaschema.avversita.AvversitaGruppo = dettaglioTrattamento.avversitaGruppo.Clona()



                                        For Each MagazziniMovimentazioni In new_avversitaGruppo.MagazziniMovimentazioni

                                            MagazziniMovimentazioni.Qta = 0

                                            MagazziniMovimentazioni.QtaTot = 0

                                        Next

                                        obj_Inizializza_QdC.ListaGiacenzeXProdotto.Add(New GiacenzeXProdotto With {
                                                            .Categoria_Magazzino = INNESCHI,
                                                            .Operazione = attivita.job,
                                                            .dettaglioTrattamento = Nothing,
                                                            .dettaglioFertilizzazione = Nothing,
                                                            .dettaglioSemina = Nothing,
                                                            .avversitaGruppo = new_avversitaGruppo
                                                       })
                                    Else
                                        For Each magazzinoMovimentazione In dettaglioTrattamento.avversitaGruppo.MagazziniMovimentazioni
                                            Dim Index_MagazzinoMovimentazione = obj_Inizializza_QdC.ListaGiacenzeXProdotto(Index).avversitaGruppo.MagazziniMovimentazioni.FindIndex(Function(m) m.Magazzino.primaryKey.centroAziendalePK.partitaIva = magazzinoMovimentazione.Magazzino.primaryKey.centroAziendalePK.partitaIva AndAlso
                                                                                                                                                                  m.Magazzino.primaryKey.centroAziendalePK.codice = magazzinoMovimentazione.Magazzino.primaryKey.centroAziendalePK.codice AndAlso
                                                                                                                                                                   m.Magazzino.primaryKey.codice = magazzinoMovimentazione.Magazzino.primaryKey.codice AndAlso
                                                                                                                                                                   m.Lotto = magazzinoMovimentazione.Lotto)

                                            If Index_MagazzinoMovimentazione = -1 Then

                                                Dim new_magazzinoMovimentazione As AgronicaCoreModelsSTD.attivita.RilevamentoDiMagazzino = magazzinoMovimentazione

                                                new_magazzinoMovimentazione.Qta = 0

                                                new_magazzinoMovimentazione.QtaTot = 0

                                                obj_Inizializza_QdC.ListaGiacenzeXProdotto(Index).avversitaGruppo.MagazziniMovimentazioni.Add(new_magazzinoMovimentazione)
                                            End If
                                        Next

                                    End If

                                End If


                            End If


                        End If

                        If risorsa.classType = ClassType.DettaglioFertilizzazione Then


                            Dim dettaglioFertilizzazione = CType(risorsa, AgronicaCoreModelsSTD.attivita.dettagli.DettaglioFertilizzazione)

                            If Not IsNothing(dettaglioFertilizzazione) AndAlso Not IsNothing(dettaglioFertilizzazione.MagazziniMovimentazioni) Then

                                Dim Index = obj_Inizializza_QdC.ListaGiacenzeXProdotto.FindIndex(Function(g) g.Categoria_Magazzino = FERTILIZZANTI AndAlso Not IsNothing(g.dettaglioFertilizzazione) AndAlso
                                                            g.dettaglioFertilizzazione.prodotto.codice = dettaglioFertilizzazione.prodotto.codice)

                                If Index = -1 Then

                                    Dim new_dettaglioFertilizzazione As DettaglioFertilizzazione = dettaglioFertilizzazione.Clona()


                                    For Each MagazziniMovimentazioni In new_dettaglioFertilizzazione.MagazziniMovimentazioni

                                        MagazziniMovimentazioni.Qta = 0

                                        MagazziniMovimentazioni.QtaTot = 0
                                    Next

                                    obj_Inizializza_QdC.ListaGiacenzeXProdotto.Add(New GiacenzeXProdotto With {
                                                        .Categoria_Magazzino = FERTILIZZANTI,
                                                        .Operazione = attivita.job,
                                                        .dettaglioTrattamento = Nothing,
                                                        .dettaglioFertilizzazione = new_dettaglioFertilizzazione,
                                                        .dettaglioSemina = Nothing
                                                   })
                                Else
                                    For Each magazzinoMovimentazione In dettaglioFertilizzazione.MagazziniMovimentazioni
                                        Dim Index_MagazzinoMovimentazione = obj_Inizializza_QdC.ListaGiacenzeXProdotto(Index).dettaglioFertilizzazione.MagazziniMovimentazioni.FindIndex(Function(m) m.Magazzino.primaryKey.centroAziendalePK.partitaIva = magazzinoMovimentazione.Magazzino.primaryKey.centroAziendalePK.partitaIva AndAlso
                                                                                                                                                              m.Magazzino.primaryKey.centroAziendalePK.codice = magazzinoMovimentazione.Magazzino.primaryKey.centroAziendalePK.codice AndAlso
                                                                                                                                                               m.Magazzino.primaryKey.codice = magazzinoMovimentazione.Magazzino.primaryKey.codice AndAlso
                                                                                                                                                               m.Lotto = magazzinoMovimentazione.Lotto)

                                        If Index_MagazzinoMovimentazione = -1 Then

                                            Dim new_magazzinoMovimentazione As AgronicaCoreModelsSTD.attivita.RilevamentoDiMagazzino = magazzinoMovimentazione

                                            new_magazzinoMovimentazione.Qta = 0

                                            new_magazzinoMovimentazione.QtaTot = 0

                                            obj_Inizializza_QdC.ListaGiacenzeXProdotto(Index).dettaglioFertilizzazione.MagazziniMovimentazioni.Add(new_magazzinoMovimentazione)
                                        End If
                                    Next


                                End If

                            End If

                        End If

                        If risorsa.classType = ClassType.DettaglioSemina Then

                            Dim dettaglioSemina = CType(risorsa, AgronicaCoreModelsSTD.attivita.dettagli.DettaglioSemina)

                            If Not IsNothing(dettaglioSemina) AndAlso Not IsNothing(dettaglioSemina.MagazziniMovimentazioni) Then
                                Dim Index = obj_Inizializza_QdC.ListaGiacenzeXProdotto.FindIndex(Function(g) g.Categoria_Magazzino = SEMENTI AndAlso Not IsNothing(g.dettaglioSemina) AndAlso
                                                            g.dettaglioSemina.prodotto.codice = dettaglioSemina.prodotto.codice AndAlso
                                                            g.dettaglioSemina.prodotto.elemCod = dettaglioSemina.prodotto.elemCod)

                                If Index = -1 Then

                                    Dim new_dettaglioSemina As DettaglioSemina = dettaglioSemina.Clona()


                                    For Each MagazziniMovimentazioni In new_dettaglioSemina.MagazziniMovimentazioni

                                        MagazziniMovimentazioni.Qta = 0

                                        MagazziniMovimentazioni.QtaTot = 0
                                    Next

                                    obj_Inizializza_QdC.ListaGiacenzeXProdotto.Add(New GiacenzeXProdotto With {
                                                        .Categoria_Magazzino = SEMENTI,
                                                        .Operazione = attivita.job,
                                                        .dettaglioTrattamento = Nothing,
                                                        .dettaglioFertilizzazione = Nothing,
                                                        .dettaglioSemina = new_dettaglioSemina
                                                   })
                                Else
                                    For Each magazzinoMovimentazione In dettaglioSemina.MagazziniMovimentazioni
                                        Dim Index_MagazzinoMovimentazione = obj_Inizializza_QdC.ListaGiacenzeXProdotto(Index).dettaglioSemina.MagazziniMovimentazioni.FindIndex(Function(m) m.Magazzino.primaryKey.centroAziendalePK.partitaIva = magazzinoMovimentazione.Magazzino.primaryKey.centroAziendalePK.partitaIva AndAlso
                                                                                                                                                              m.Magazzino.primaryKey.centroAziendalePK.codice = magazzinoMovimentazione.Magazzino.primaryKey.centroAziendalePK.codice AndAlso
                                                                                                                                                               m.Magazzino.primaryKey.codice = magazzinoMovimentazione.Magazzino.primaryKey.codice AndAlso
                                                                                                                                                               m.Lotto = magazzinoMovimentazione.Lotto)

                                        If Index_MagazzinoMovimentazione = -1 Then

                                            Dim new_magazzinoMovimentazione As AgronicaCoreModelsSTD.attivita.RilevamentoDiMagazzino = magazzinoMovimentazione

                                            new_magazzinoMovimentazione.Qta = 0

                                            new_magazzinoMovimentazione.QtaTot = 0

                                            obj_Inizializza_QdC.ListaGiacenzeXProdotto(Index).dettaglioSemina.MagazziniMovimentazioni.Add(new_magazzinoMovimentazione)
                                        End If
                                    Next


                                End If
                            End If

                        End If
                    Next

                End If

            Next


            Dim objG As New AgronicaCoreStampeDAL.Magazzino

            Dim validitaFine = obj_Inizializza_QdC.Data

            For Each giacenzeXProdotto In obj_Inizializza_QdC.ListaGiacenzeXProdotto

                Dim Piva As String = LeggiInizializza_QdC.impresa.partitaIva

                Dim Dt_Giacenze As DataTable = Nothing

                Dim Dt_Giacenze_Tot As DataTable = Nothing

                Dim Qta As Decimal = 0

                Dim QtaTot As Decimal = 0

                Dim Pro_Cod As Integer = 0

                Dim Mat_Cod As Integer = 0

                Dim Categoria_Magazzino As Integer = giacenzeXProdotto.Categoria_Magazzino

                Dim MagazziniMovimentazioni As New List(Of AgronicaCoreModelsSTD.attivita.RilevamentoDiMagazzino)

                Select Case Categoria_Magazzino
                    Case FORMULATI
                        Pro_Cod = giacenzeXProdotto.dettaglioTrattamento.prodotto.codice
                    Case FERTILIZZANTI
                        Pro_Cod = giacenzeXProdotto.dettaglioFertilizzazione.prodotto.codice
                    Case SEMENTI
                        Mat_Cod = giacenzeXProdotto.dettaglioSemina.prodotto.codice
                    Case INNESCHI
                        Pro_Cod = giacenzeXProdotto.avversitaGruppo.codice
                End Select


                Dt_Giacenze = objG.SchedaGiacenzeMagazzino(validitaFine,
                                                                       Piva, 0, 0,
                                                                       Categoria_Magazzino,
                                                                       Pro_Cod,
                                                                       Mat_Cod, 0, 0, 0, 0,
                                                                       LOTTO_NONDEFINITO,
                                                                       Flag_QtaNoZero:=False,
                                                                       "",
                                                                       "",
                                                                       "", "",
                                                                       "", "",
                                                                       "", "",
                                                                       "", "",
                                                                       "", "",
                                                                       "",
                                                                       objParametri_Server, objParametri_Utenti)


                Dt_Giacenze_Tot = objG.SchedaGiacenzeMagazzino(AGRODATAFINE,
                                                           Piva, 0, 0,
                                                           Categoria_Magazzino,
                                                           Pro_Cod,
                                                           Mat_Cod, 0, 0, 0, 0,
                                                           LOTTO_NONDEFINITO,
                                                           Flag_QtaNoZero:=False,
                                                           "",
                                                           "",
                                                           "", "",
                                                           "", "",
                                                           "", "",
                                                           "", "",
                                                           "", "",
                                                           "",
                                                           objParametri_Server, objParametri_Utenti)


                If (Not IsNothing(Dt_Giacenze) AndAlso Dt_Giacenze.Rows.Count > 0) OrElse
                        (Not IsNothing(Dt_Giacenze_Tot) AndAlso Dt_Giacenze_Tot.Rows.Count > 0) Then

                    Select Case Categoria_Magazzino
                        Case FORMULATI
                            MagazziniMovimentazioni = giacenzeXProdotto.dettaglioTrattamento.MagazziniMovimentazioni
                        Case FERTILIZZANTI
                            MagazziniMovimentazioni = giacenzeXProdotto.dettaglioFertilizzazione.MagazziniMovimentazioni
                        Case SEMENTI
                            MagazziniMovimentazioni = giacenzeXProdotto.dettaglioSemina.MagazziniMovimentazioni
                        Case INNESCHI
                            MagazziniMovimentazioni = giacenzeXProdotto.avversitaGruppo.MagazziniMovimentazioni
                    End Select

                    If Not IsNothing(MagazziniMovimentazioni) AndAlso MagazziniMovimentazioni.Count > 0 Then

                        For Each magazzinoMovimentazione In MagazziniMovimentazioni

                            Qta = magazzinoMovimentazione.Qta

                            QtaTot = magazzinoMovimentazione.Qta

                            Dim Piva_Magazzino As String = magazzinoMovimentazione.Magazzino.primaryKey.centroAziendalePK.partitaIva

                            Dim Sa_Cod_Magazzino As Integer = magazzinoMovimentazione.Magazzino.primaryKey.centroAziendalePK.codice

                            Dim Id_Destinazione As Integer = magazzinoMovimentazione.Magazzino.primaryKey.codice

                            Dim Lotto As String = magazzinoMovimentazione.Lotto

                            Dim Udm_Cod As Integer = magazzinoMovimentazione.udm.codice

                            If Not IsNothing(Dt_Giacenze) AndAlso Dt_Giacenze.Rows.Count > 0 Then
                                Dim DrGiacenze As DataRow() = Dt_Giacenze.Select("Piva = '" & Piva_Magazzino & "' And Sa_Cod = " & Sa_Cod_Magazzino & " And Id_Destinazione = " & Id_Destinazione & " And Lotto = '" & UtilityProvider.Agro_SQL_SaveText(Lotto) & "' And Udm_Cod = " & Udm_Cod)

                                If Not IsNothing(DrGiacenze) AndAlso DrGiacenze.Length > 0 Then
                                    For g = 0 To DrGiacenze.Length - 1
                                        Qta += DrGiacenze(g).Item("Giacenza")
                                    Next
                                End If
                            End If

                            If Not IsNothing(Dt_Giacenze_Tot) AndAlso Dt_Giacenze_Tot.Rows.Count > 0 Then
                                Dim DrGiacenze_Tot As DataRow() = Dt_Giacenze_Tot.Select("Piva = '" & Piva_Magazzino & "' And Sa_Cod = " & Sa_Cod_Magazzino & " And Id_Destinazione = " & Id_Destinazione & " And Lotto = '" & UtilityProvider.Agro_SQL_SaveText(Lotto) & "' And Udm_Cod = " & Udm_Cod)

                                If Not IsNothing(DrGiacenze_Tot) AndAlso DrGiacenze_Tot.Length > 0 Then
                                    For g = 0 To DrGiacenze_Tot.Length - 1
                                        QtaTot += DrGiacenze_Tot(g).Item("Giacenza")
                                    Next
                                End If
                            End If

                            magazzinoMovimentazione.QtaTot = Utility.ConvertValueFromUdM(QtaTot, magazzinoMovimentazione.udm)
                            magazzinoMovimentazione.Qta = Utility.ConvertValueFromUdM(Qta, magazzinoMovimentazione.udm)

                        Next


                        Select Case Categoria_Magazzino
                            Case FORMULATI
                                giacenzeXProdotto.dettaglioTrattamento.MagazziniMovimentazioni = MagazziniMovimentazioni
                            Case FERTILIZZANTI
                                giacenzeXProdotto.dettaglioFertilizzazione.MagazziniMovimentazioni = MagazziniMovimentazioni
                            Case SEMENTI
                                giacenzeXProdotto.dettaglioSemina.MagazziniMovimentazioni = MagazziniMovimentazioni
                            Case INNESCHI
                                giacenzeXProdotto.avversitaGruppo.MagazziniMovimentazioni = MagazziniMovimentazioni
                        End Select

                    End If

                End If

            Next

        End If

    End Sub


End Class
