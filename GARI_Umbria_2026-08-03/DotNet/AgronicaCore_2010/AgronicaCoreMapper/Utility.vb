Imports System.Globalization
Imports AgronicaControlli_2010
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.attivita
Imports AgronicaCoreModelsSTD.attivita.risorse
Imports AgronicaCoreModelsSTD.documenti
Imports AgronicaCoreModelsSTD.metaschema
Imports AgronicaCoreModelsSTD.metaschema.avversita
Imports AgronicaCoreModelsSTD.metaschema.utilizzi
Imports AgronicaCoreModelsSTD.costanti

Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreVarieDAL
Imports AgronicaCoreDTOStd.InData

Imports AgronicaCoreModello
Imports AgronicaCoreModello.Utility_Agenda
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreModelsSTD.attivita.dettagli
Imports AgronicaCoreModelsSTD
Imports Newtonsoft.Json
Imports AgronicaCoreDTOStd.InData.Agenda
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreContabDAL
Imports AgronicaCoreModelsSTD.attivita.centri_di_costo
Imports System.Configuration
Imports AgronicaCoreModelsSTD.exceptions
'Imports AgronicaCoreMetaSchemaBIZ

Public Class Utility

    Public Cau_Mov As String = ""
    Public Elem_Cod As Integer = 0
    Private Shared List_UdM As New List(Of UnitaDiMisura)

    Public Shared Sub CompleteVerbose(ByRef attivita As Attivita, isRibaltamentoToAgenda As Boolean, ByRef listParametriAggiuntivi As List(Of Parametri_Aggiuntivi_Attivita), objParametri_Super_Server As AgronicaCoreParametri, objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri)

        If attivita Is Nothing Then
            Exit Sub
        End If

        Dim formulatiNonCorretti As New List(Of DettaglioTrattamento)
        Dim formulatiAmbigui As New List(Of DettaglioTrattamento)
        Dim avversitaNonCorrette As New List(Of DettaglioTrattamento)
        Dim avversitaAmbigue As New List(Of DettaglioTrattamento)
        Dim avversitaNonValorizzate As New List(Of DettaglioTrattamento)

        Dim fertilizzantiNonCorretti As New List(Of DettaglioFertilizzazione)
        Dim fertilizzantiAmbigui As New List(Of DettaglioFertilizzazione)

        Dim InfoOperazione As InfoOperazione = GetInfoOperazione(attivita.job.primaryKey.codice, attivita.tipo)

        Dim specie = Utility.GetSpecieFromUtilizzoTerreno(attivita.utilizzoTerreno)

        If attivita.job IsNot Nothing Then
            Dim objLavorazione = New AgronicaCoreAnagrafeBIZ.Lavorazione_R
            attivita.job.descrizione = objLavorazione.GetLavDes(attivita.job.primaryKey.codice, objParametri_Server)
        End If

        If attivita.centroAziendale IsNot Nothing AndAlso Not String.IsNullOrEmpty(attivita.centroAziendale.primaryKey.partitaIva) AndAlso attivita.centroAziendale.primaryKey.codice <> 0 Then
            Dim objCentro As New AgronicaCoreAnagrafeBIZ.CentroAziendale_R
            attivita.centroAziendale = objCentro.Centro_Leggi_Anagrafica(Piva:=attivita.centroAziendale.primaryKey.partitaIva,
                                                            Sa_Cod:=attivita.centroAziendale.primaryKey.codice,
                                                            Leggi_Impresa:=True,
                                                            Leggi_Indirizzo:=True,
                                                            Leggi_Codici:=True,
                                                            Leggi_Rubrica:=True,
                                                            Leggi_Catasto:=False,
                                                            objParametri_Server)
        End If

        If attivita.job IsNot Nothing AndAlso attivita.disciplinare IsNot Nothing AndAlso attivita.epoca IsNot Nothing AndAlso attivita.epoca.codice <> 0 Then

            Dim EpocheBiz As New AgronicaControlli_2010.STD_Epoche

            If InfoOperazione.IsTrattamento Then
                Dim epocheList = EpocheBiz.LeggiEpocheDPI(attivita.job,
                                                            attivita.disciplinare,
                                                            modulo:=attivita.epoca.codice,
                                                            objParametri_Super_Server,
                                                            objParametri_Server,
                                                            objParametri_Utenti)

                For Each objEpoca In epocheList
                    If objEpoca.codice = attivita.epoca.codice Then
                        attivita.epoca.descrizione = objEpoca.descrizione
                        Exit For
                    End If
                Next
            End If

            If InfoOperazione.IsFertilizzazione Then

                Dim epocheList = EpocheBiz.LeggiEpocheFertilizzazione(specie,
                                                            attivita.disciplinare,
                                                            attivita.epoca.codice,
                                                            objParametri_Super_Server,
                                                            objParametri_Server)

                For Each objEpoca In epocheList
                    If objEpoca.codice = attivita.epoca.codice Then
                        attivita.epoca.descrizione = objEpoca.descrizione
                        Exit For
                    End If
                Next
            End If

        End If

        If attivita.attivitaPersonalizzata IsNot Nothing AndAlso attivita.attivitaPersonalizzata.codice > 0 Then

            Dim objAttivita As New AgronicaCoreContabBIZ.AttivitaPersonalizzata
            Dim attivitaPersonalizzataList = objAttivita.LeggiAttivitaPersonalizzata(attivita.attivitaPersonalizzata.codice, attivita.job.primaryKey.codice, objParametri_Server)

            If attivitaPersonalizzataList IsNot Nothing AndAlso attivitaPersonalizzataList.Count > 0 Then
                attivita.attivitaPersonalizzata = attivitaPersonalizzataList(0)

                If InfoOperazione.IsVisita Then
                    If attivita.attivitaPersonalizzata.operazioni IsNot Nothing AndAlso attivita.attivitaPersonalizzata.operazioni.Count > 0 Then
                        If attivita.attivitaPersonalizzata.operazioni(0).primaryKey IsNot Nothing AndAlso attivita.attivitaPersonalizzata.operazioni(0).primaryKey.codice <> "0" Then
                            Dim objLavorazione = New AgronicaCoreAnagrafeBIZ.Lavorazione_R
                            attivita.attivitaPersonalizzata.operazioni(0).descrizione = objLavorazione.GetLavDes(attivita.attivitaPersonalizzata.operazioni(0).primaryKey.codice, objParametri_Server)
                        End If
                    End If
                End If
            End If

        End If

        Dim lstImpiantiSelezionati As New List(Of Impianto)
        Dim lstProdottiDaTrattareSelezionati As New List(Of MovimentoDiMagazzino)

        Dim lstCentriSelezionati As New List(Of Integer)

        For Each centroDiCosto In attivita.centriDiCosto
            If InfoOperazione.TipoCentroDiCosto = Tipo.ProdottoDaTrattare Then
                Dim prodottoDaTrattareCdC As centri_di_costo.ProdottoDaTrattareCDC = CType(centroDiCosto, centri_di_costo.ProdottoDaTrattareCDC)

                Dim magazzinoPK = New Fabbricato(prodottoDaTrattareCdC.giacenzaMagazzino.Magazzino.primaryKey.centroAziendalePK.partitaIva, prodottoDaTrattareCdC.giacenzaMagazzino.Magazzino.primaryKey.centroAziendalePK.codice, prodottoDaTrattareCdC.giacenzaMagazzino.Magazzino.primaryKey.codice, "")
                Dim prodottoPK = New Prodotto(prodottoDaTrattareCdC.giacenzaMagazzino.Prodotto.codice)


                Dim objFabbricati_R As New AgronicaCoreAnagrafeDAL.Fabbricati_R
                Dim objMateriePrime_R As New AgronicaCoreAnagrafeDAL.Materie_Prime_R

                Dim magazzinoDT = objFabbricati_R.Leggi_3(magazzinoPK.primaryKey.centroAziendalePK.partitaIva,
                                                          magazzinoPK.primaryKey.centroAziendalePK.codice,
                                                          magazzinoPK.primaryKey.codice,
                                                          "", "",
                                                          objParametri_Server)

                If magazzinoDT.Rows.Count > 0 Then
                    magazzinoPK.descrizione = magazzinoDT.Rows(0).Item("Fabbricato_Des")
                End If

                Dim materiePrimeDT = objMateriePrime_R.Leggi(magazzinoPK.primaryKey.centroAziendalePK.partitaIva,
                                                             Sa_Cod:=0,
                                                             prodottoDaTrattareCdC.giacenzaMagazzino.Prodotto.elemCod,
                                                             prodottoDaTrattareCdC.giacenzaMagazzino.Prodotto.codice, "",
                                                             0, 0,
                                                             0, 0,
                                                             0, 0, 0,
                                                             "", 0,
                                                             "",
                                                             False,
                                                             Flag_MateriePrimeSoloPrivate:=False,
                                                             "",
                                                             enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                             "", "",
                                                             objParametri_Server)

                If materiePrimeDT.Rows.Count > 0 Then
                    prodottoPK.elemCod = materiePrimeDT.Rows(0).Item("Elem_Cod")
                    prodottoPK.descrizione = materiePrimeDT.Rows(0).Item("Mat_Des")
                    prodottoPK.codice_alfanumerico = materiePrimeDT.Rows(0).Item("Cod_Articolo")
                    prodottoPK.regolamento = New Regolamenti(materiePrimeDT.Rows(0).Item("Regolamento"))
                End If

                Dim movimentoMagazzino As New MovimentoDiMagazzino With {
                    .Magazzino = magazzinoPK,
                    .Prodotto = prodottoPK
                }

                If Not lstCentriSelezionati.Contains(magazzinoPK.primaryKey.centroAziendalePK.codice) Then
                    lstCentriSelezionati.Add(magazzinoPK.primaryKey.centroAziendalePK.codice)
                End If

                lstProdottiDaTrattareSelezionati.Add(movimentoMagazzino)

            Else
                Dim esercizioCDC As centri_di_costo.EsercizioCDC = CType(centroDiCosto, centri_di_costo.EsercizioCDC)

                Dim appezzamentoPK = New Appezzamento.PK(esercizioCDC.esercizio.impiantoPK.appezzamentoPK.codice, attivita.centroAziendale.primaryKey)
                Dim impiantoPK = New Impianto.PK(esercizioCDC.esercizio.impiantoPK.codice, appezzamentoPK)
                Dim impianto As New Impianto(impiantoPK)

                Dim objImpianti_R As New AgronicaCoreAnagrafeBIZ.Reg_Impianto_R

                Dim imp = objImpianti_R.Leggi_Impianto_Anagrafica(attivita.centroAziendale.primaryKey.partitaIva,
                                                            attivita.centroAziendale.primaryKey.codice,
                                                            appezzamentoPK.codice,
                                                            impiantoPK.codice,
                                                            False,
                                                            False,
                                                            AGRODATAINIZIO,
                                                            filtroData:=False,
                                                            Leggi_Cartografia:=False,
                                                            objParametri_Super_Server,
                                                            objParametri_Server,
                                                            objParametri_Utenti)

                If imp IsNot Nothing Then
                    If imp.gruppoFinalita IsNot Nothing Then
                        impianto.gruppoFinalita = imp.gruppoFinalita
                    End If
                    If imp.copertura IsNot Nothing Then
                        impianto.copertura = imp.copertura
                    End If
                End If

                lstImpiantiSelezionati.Add(impianto)
            End If
        Next

        Dim dtDSSIrrigazione As DataTable = Nothing

        Dim ListIndiciMaturita As List(Of AgronicaCoreMetaSchemaBIZ.IndiciMaturita) = Nothing

        Dim ListMisuraXDR As List(Of AgronicaCoreMetaSchemaBIZ.MisuraXDR) = Nothing

        Dim ListMisuraXAvv As List(Of AgronicaCoreMetaSchemaBIZ.MisuraXAvv) = Nothing

        Dim ListFaseFenologica As List(Of AgronicaCoreMetaSchemaBIZ.FaseFenologica) = Nothing

        Dim ListIndiciRese As List(Of AgronicaCoreMetaSchemaBIZ.IndiciMaturita) = Nothing

        Dim ListInfestanti As (List(Of AgronicaCoreMetaSchemaBIZ.InfestantiAttive), List(Of AgronicaCoreMetaSchemaBIZ.GruppoInfestantiAttive)) = Nothing

        Dim ListAvversitaTrappole As List(Of AgronicaCoreModelsSTD.attivita.dettagli.AvversitaTrappole) = Nothing

        If attivita.risorse IsNot Nothing Then
            For Each risorsa In attivita.risorse

                Select Case risorsa.classType

                    Case ClassType.RisorsaSpecie

                        Dim risorsaSpecie = CType(risorsa, risorse.RisorsaSpecie)

                        If risorsaSpecie.specie IsNot Nothing Then
                            risorsaSpecie.specie.descrizione = New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R().VegDes_from_VegCod(risorsaSpecie.specie.codice, objParametri_Server)
                        End If


                    Case ClassType.RisorsaDestinazioneUso

                        Dim risorsaDestinazioneUso = CType(risorsa, risorse.RisorsaDestinazioneUso)

                        If risorsaDestinazioneUso.destinazioneUso IsNot Nothing Then

                            Dim codice_R As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R

                            Dim DT = codice_R.Leggi(risorsaDestinazioneUso.destinazioneUso.codice, "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

                            If (DT.Rows.Count = 1) Then
                                risorsaDestinazioneUso.destinazioneUso.descrizione = DT.Rows(0)("descrizione")
                            End If

                        End If

                    Case ClassType.RisorsaZootecnica

                        Dim risorsaZootecnica = CType(risorsa, risorse.RisorsaZootecnica)

                        If risorsaZootecnica.genere IsNot Nothing Then

                            Dim codice_R As New AgronicaCoreZooDAL.Lista_Indprod_Animali_R
                            Dim DT = codice_R.LeggiRisorseZootecniche(If(risorsaZootecnica.genere IsNot Nothing, risorsaZootecnica.genere.codice, 0), If(risorsaZootecnica.specie IsNot Nothing, risorsaZootecnica.specie.codice, 0), If(risorsaZootecnica.indirizzoProd IsNot Nothing, risorsaZootecnica.indirizzoProd.codice, 0), "", "", objParametri_Server)

                            If (DT.Rows.Count = 1) Then
                                risorsaZootecnica.descrizione = DT.Rows(0)("ZOO_DES")
                            End If

                        End If

                    Case ClassType.RisorsaMacchina

                        Dim risorsaMacchina = CType(risorsa, risorse.RisorsaMacchina)

                        If risorsaMacchina.macchina IsNot Nothing Then

                            Dim parco_macchine_R As New AgronicaCoreContabBIZ.Parco_Macchine_R()
                            risorsaMacchina.macchina = parco_macchine_R.Leggi_Macchina(Piva:="", risorsaMacchina.macchina.codice, objParametri_Server)

                        End If

                    Case ClassType.RisorsaCausale

                        Dim risorsaCausale = CType(risorsa, risorse.RisorsaCausale)

                        If risorsaCausale.id <> 0 Then
                            Dim objOperazioneCausale As New AgronicaCoreContabBIZ.Operazione_Causale_R
                            risorsaCausale.causale = objOperazioneCausale.Leggi_CausaleDes_From_CausaleId(risorsaCausale.id, objParametri_Server)
                        End If

                    Case ClassType.RisorsaPersona, ClassType.RisorsaAssegnatarioVisita
                        'DT: verbose già gestito, nulla da completare

                    Case ClassType.DettaglioTrattamento

                        Dim dettaglioTrattamento = CType(risorsa, dettagli.DettaglioTrattamento)
                        dettaglioTrattamento.dataSmaltimentoScorte = AGRODATAINIZIO

                        If InfoOperazione.Elem_Cod <> INSETTI Then

                            'DT: i dati non presenti in DB vanno caricati tramite WS
                            Dim formulatiList As New List(Of AgronicaCoreModelsSTD.attivita.dettagli.DettaglioTrattamento)
                            Dim FormulatiBiz As New AgronicaControlli_2010.STD_Formulati

                            'DT: si fa la chiamata senza filtri tranne il prodotto, poi si filtra il risultato per la chiave indicata in seguito
                            formulatiList = FormulatiBiz.LeggiFormulati(attivita.tipo,
                                                                        attivita.stato,
                                                                        attivita.job,
                                                                        lstImpiantiSelezionati.ToArray,
                                                                        lstProdottiDaTrattareSelezionati.ToArray,
                                                                        specie,
                                                                        attivita.disciplinare,
                                                                        Nothing,
                                                                        dettaglioTrattamento.avversitaGruppo,
                                                                        filtroPerDescrizione:="",
                                                                        attivita.inizio,
                                                                        escludiGiacenzeZero:=False,
                                                                        magazziniAgenzie:=False,
                                                                        magazziniEsterni:=False,
                                                                        dettaglioTrattamento.prodotto.codice,
                                                                        soloLetturaAnagrafica:=True,
                                                                        objParametri_Super_Server,
                                                                        objParametri_Server,
                                                                        objParametri_Utenti,
                                                                        isRibaltamentoToAgenda)



                            If formulatiList IsNot Nothing AndAlso formulatiList.Count > 0 Then

                                'DT: chiave prodotto: fr_cod (passato in input al ws) + dettaglioProdotto (extra_str) + tipoFormulato (tipoRichiesto)
                                'DT: se ne vengono trovati 0 o più di 1, vuol dire che il prodotto è incompatibile con gli altri dati dell'operazione (Es: specie) o che mancano i dati necessari per decodificarlo univocamente
                                Dim prodotti = formulatiList _
                                    .Where(Function(x) (x.dettaglioProdotto = dettaglioTrattamento.dettaglioProdotto _
                                        OrElse dettaglioTrattamento.dettaglioProdotto = 0) _
                                        AndAlso (x.tipoFormulato = dettaglioTrattamento.tipoFormulato _
                                        OrElse dettaglioTrattamento.tipoFormulato = 0)) _
                                    .ToList

                                If prodotti.Count = 1 Then

                                    Dim prodotto As DettaglioTrattamento = prodotti.FirstOrDefault()

                                    'dati che potrebbero non essere stati presenti nel db (es: dati da APP) ma che completo in quanto identificati in modo univoco tramite frCod
                                    dettaglioTrattamento.dettaglioProdotto = prodotto.dettaglioProdotto
                                    dettaglioTrattamento.tipoFormulato = prodotto.tipoFormulato


                                    'dati che vengono presi INTERAMENTE dal WS perchè non salvati in DB
                                    dettaglioTrattamento.descrizionePrecedente = prodotto.descrizionePrecedente
                                    dettaglioTrattamento.dataSmaltimentoScorte = prodotto.dataSmaltimentoScorte
                                    dettaglioTrattamento.inRevisione = prodotto.inRevisione
                                    dettaglioTrattamento.dataAttoNormativo = prodotto.dataAttoNormativo
                                    dettaglioTrattamento.formulatiXAllegatiNormative_IDRiga = prodotto.formulatiXAllegatiNormative_IDRiga
                                    dettaglioTrattamento.epocheBlocchi = prodotto.epocheBlocchi
                                    dettaglioTrattamento.dettagliDose = prodotto.dettagliDose
                                    dettaglioTrattamento.protezione = prodotto.protezione
                                    dettaglioTrattamento.modalitaImpiego = prodotto.modalitaImpiego
                                    dettaglioTrattamento.classificazioni = prodotto.classificazioni

                                    'dati che vengono SOLO decodificati da WS se presenti in DB
                                    If dettaglioTrattamento.principiAttivi IsNot Nothing AndAlso prodotto.principiAttivi IsNot Nothing Then
                                        For Each prAtt In dettaglioTrattamento.principiAttivi
                                            For Each prAttProdotto In prodotto.principiAttivi
                                                If prAtt.codice = prAttProdotto.codice Then
                                                    prAtt.descrizione = prAttProdotto.descrizione
                                                    Exit For
                                                End If
                                            Next
                                        Next
                                    End If

                                    'dati che vengono presi INTERAMENTE da WS se non presenti in DB (es: ricette che arrivano da APP)
                                    If dettaglioTrattamento.principiAttivi Is Nothing OrElse dettaglioTrattamento.principiAttivi.Count = 0 Then
                                        dettaglioTrattamento.principiAttivi = prodotto.principiAttivi
                                    End If
                                    If dettaglioTrattamento.tempoCarenza = 0 Then
                                        dettaglioTrattamento.tempoCarenza = prodotto.tempoCarenza
                                    End If
                                    If dettaglioTrattamento.bufferzone Is Nothing OrElse (dettaglioTrattamento.bufferzone.minimo = 0 AndAlso dettaglioTrattamento.bufferzone.massimo = 0) Then
                                        dettaglioTrattamento.bufferzone = prodotto.bufferzone
                                    End If
                                    If dettaglioTrattamento.polverulento = 0 Then
                                        dettaglioTrattamento.polverulento = prodotto.polverulento
                                    End If
                                    If dettaglioTrattamento.durataFeromone = 0 Then
                                        dettaglioTrattamento.durataFeromone = prodotto.durataFeromone
                                        dettaglioTrattamento.scadenzaFeromone = attivita.inizio.AddDays(prodotto.durataFeromone)
                                    End If
                                Else
                                    If prodotti.Count = 0 Then
                                        formulatiNonCorretti.Add(dettaglioTrattamento)
                                    Else
                                        formulatiAmbigui.Add(dettaglioTrattamento)
                                    End If
                                End If
                            Else
                                formulatiNonCorretti.Add(dettaglioTrattamento)
                            End If

                        End If

                        If attivita.job.primaryKey.codice = LAVCOD_TRATTAMENTO_ANTIPARASSITARIO OrElse
                           attivita.job.primaryKey.codice = LAVCOD_DISERBO OrElse
                           attivita.job.primaryKey.codice = LAVCOD_GEODISINFESTAZIONE OrElse
                           attivita.job.primaryKey.codice = LAVCOD_CONCIA_SEME OrElse
                           attivita.job.primaryKey.codice = LAVCOD_DISTRIBUZIONE_INSETTI OrElse
                           attivita.job.primaryKey.codice = LAVCOD_CONFUSIONE_DISORIENTAMENTO_SESSUALE OrElse
                           attivita.job.primaryKey.codice = LAVCOD_TRATTAMENTO_POST_RACCOLTA OrElse
                           attivita.job.primaryKey.codice = LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA OrElse
                           attivita.job.primaryKey.codice = LAVCOD_REINNESCO_TRAPPOLE Then


                            If dettaglioTrattamento.avversitaGruppo IsNot Nothing AndAlso dettaglioTrattamento.avversitaGruppo.codice > 0 Then

                                Dim avversitaVerbose As AvversitaGruppo = Nothing

                                Dim AvversitaBiz As New AgronicaControlli_2010.STD_Avversita

                                Dim avversitaList As New List(Of metaschema.avversita.AvversitaGruppo)
                                Select Case InfoOperazione.Elem_Cod
                                    Case INSETTI
                                        avversitaList = AvversitaBiz.LeggiAvversitaInsettiUtili(attivita.tipo,
                                                                                                attivita.job,
                                                                                                lstImpiantiSelezionati.ToArray,
                                                                                                specie,
                                                                                                dettaglioTrattamento,
                                                                                                attivita.inizio,
                                                                                                objParametri_Super_Server,
                                                                                                objParametri_Server,
                                                                                                objParametri_Utenti)

                                    Case Else
                                        avversitaList = AvversitaBiz.LeggiAvversita(attivita.tipo,
                                                                                attivita.job,
                                                                                lstImpiantiSelezionati.ToArray,
                                                                                specie,
                                                                                attivita.disciplinare,
                                                                                attivita.epoca,
                                                                                dettaglioTrattamento,
                                                                                attivita.inizio,
                                                                                objParametri_Super_Server,
                                                                                objParametri_Server,
                                                                                objParametri_Utenti,
                                                                                isRibaltamentoToAgenda)

                                End Select

                                If avversitaList IsNot Nothing AndAlso avversitaList.Any() Then

                                    'DT: chiave avversita: av_cod/av_gru, se ce ne fossero più di una si prende la prima (TODO_DT: in attesa di valutare se salvare for_veg_av_cod)
                                    Dim listAvversitaVerbose = avversitaList _
                                        .Where(Function(x) x.codice = dettaglioTrattamento.avversitaGruppo.codice _
                                            AndAlso x.classType = dettaglioTrattamento.avversitaGruppo.classType) _
                                        .ToList

                                    Dim totale = listAvversitaVerbose.Count
                                    If totale = 1 Then

                                        avversitaVerbose = listAvversitaVerbose.FirstOrDefault

                                        dettaglioTrattamento.avversitaGruppo.descrizione = avversitaVerbose.descrizione
                                        dettaglioTrattamento.avversitaGruppo.dataSmaltimentoScorte = avversitaVerbose.dataSmaltimentoScorte
                                        GetDescrizioneMagazzini(dettaglioTrattamento.avversitaGruppo.MagazziniMovimentazioni, dettaglioTrattamento.avversitaGruppo.descrizione, objParametri_Server)
                                    ElseIf totale = 0 Then
                                        avversitaNonCorrette.Add(dettaglioTrattamento)
                                    Else
                                        avversitaAmbigue.Add(dettaglioTrattamento)
                                    End If
                                Else
                                    avversitaNonCorrette.Add(dettaglioTrattamento)
                                End If

                                If avversitaVerbose Is Nothing Then
                                    Select Case dettaglioTrattamento.avversitaGruppo.classType
                                        Case ClassType.Avversita
                                            Dim objAvversita As New AgronicaCoreMetaSchemaDAL.Avversita_R
                                            dettaglioTrattamento.avversitaGruppo.descrizione = objAvversita.AvDes_from_AvCod(dettaglioTrattamento.avversitaGruppo.codice, "", AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                                            dettaglioTrattamento.avversitaGruppo.dataSmaltimentoScorte = AGRODATAFINE

                                        Case ClassType.GruppoAvversita
                                            Dim objAvversita As New AgronicaCoreMetaSchemaDAL.GruppoAvversita_R
                                            dettaglioTrattamento.avversitaGruppo.descrizione = objAvversita.AvGruDes_from_AvGruCod(dettaglioTrattamento.avversitaGruppo.codice, "", AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                                            dettaglioTrattamento.avversitaGruppo.dataSmaltimentoScorte = AGRODATAFINE
                                    End Select
                                End If
                            Else
                                If dettaglioTrattamento.avversitaGruppo Is Nothing OrElse (Not IsNothing(dettaglioTrattamento.avversitaGruppo) AndAlso dettaglioTrattamento.avversitaGruppo.codice = 0) Then
                                    avversitaNonValorizzate.Add(dettaglioTrattamento)
                                End If
                            End If

                            If dettaglioTrattamento.soglia IsNot Nothing AndAlso dettaglioTrattamento.soglia.codice <> 0 AndAlso dettaglioTrattamento.avversitaGruppo IsNot Nothing Then
                                Dim soglia = STD_Utility.getSoglia(dettaglioTrattamento.soglia.codice, attivita.disciplinare, dettaglioTrattamento.avversitaGruppo, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)

                                If soglia IsNot Nothing Then
                                    dettaglioTrattamento.soglia = soglia
                                End If
                            End If

                        End If

                    Case ClassType.DettaglioSemina

                        Dim dettaglioSemina = CType(risorsa, dettagli.DettaglioSemina)

                        Dim objMaterie As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
                        Dim dtMaterie As DataTable = objMaterie.Leggi_da_MatCod_senzaFiltroVisibilita(dettaglioSemina.prodotto.elemCod, dettaglioSemina.prodotto.codice, "", objParametri_Server)

                        If dtMaterie IsNot Nothing AndAlso dtMaterie.Rows.Count = 1 Then

                            Dim objCultivar As New AgronicaCoreMetaSchemaDAL.Cultivar_R
                            Dim vegDes As String = ""
                            Dim culDes As String = ""
                            objCultivar.VegDes_CulDes_from_Vegcod_CulCod(dtMaterie.Rows(0).Item("Veg_cod"), dtMaterie.Rows(0).Item("Cul_cod"), vegDes, culDes, objParametri_Server)

                            dettaglioSemina.varieta = New utilizzi.Varieta(dtMaterie.Rows(0).Item("Cul_cod")) With {.descrizione = culDes}
                            dettaglioSemina.varieta.specie = New utilizzi.Specie(dtMaterie.Rows(0).Item("Veg_cod")) With {.descrizione = vegDes}

                            dettaglioSemina.codArticolo = dtMaterie.Rows(0).Item("Cod_Articolo")
                            dettaglioSemina.regolamento = dtMaterie.Rows(0).Item("Regolamento")
                        End If

                    Case ClassType.DettaglioFertilizzazione

                        Dim dettaglioFertilizzazione = CType(risorsa, dettagli.DettaglioFertilizzazione)

                        Dim pua As Pua = Nothing

                        'Se siamo in una ricetta di tipo PUA (9) recupero il PUA dalla testataRicetta perchè ho salvato il Programmazione_Cod
                        'altrimenti vado a recuperare il PUA
                        If CInt(attivita.job.primaryKey.codice) = LAVCOD_DISTRIBUZIONE_AMMENDANTI Then

                            If Not IsNothing(attivita.testataRicetta) AndAlso
                                attivita.tipo = AgronicaCoreModelsSTD.attivita.Attivita.Tipo_Attivita.Ricetta AndAlso
                                attivita.tipoRicetta = AgronicaCoreModelsSTD.attivita.Attivita.Tipo_Ricetta.PianoDistribuzionePua AndAlso
                                attivita.stato = AgronicaCoreModelsSTD.attivita.Attivita.Stati.Da_Eseguire Then

                                pua = attivita.testataRicetta.pua
                            Else
                                pua = Utility.Recupera_Pua_Valido_Alla_Data(attivita.centroAziendale.primaryKey.partitaIva, attivita.inizio, LAVCOD_DISTRIBUZIONE_AMMENDANTI, attivita.tipo,
                                                                        attivita.tipoRicetta, attivita.stato, attivita.disciplinare,
                                                                        objParametri_Server)
                            End If

                        End If

                        'DT: si fa la chiamata senza filtri tranne il fertilizzante, poi si filtra il risultato per la chiave indicata in seguito
                        Dim fertilizzantiList As New List(Of AgronicaCoreModelsSTD.attivita.dettagli.DettaglioFertilizzazione)
                        Dim FertilizzantiBiz As New AgronicaControlli_2010.STD_Fertilizzanti
                        fertilizzantiList = FertilizzantiBiz.LeggiFertilizzanti(attivita.tipo,
                                                                    attivita.stato,
                                                                    attivita.job,
                                                                    lstImpiantiSelezionati.ToArray,
                                                                    attivita.disciplinare,
                                                                    filtroPerDescrizione:="",
                                                                    attivita.inizio,
                                                                    escludiGiacenzeZero:=False,
                                                                    magazziniAgenzie:=False,
                                                                    magazziniEsterni:=False,
                                                                    tipoRicetta:=0, 'TODO_DT: enum_TipoRicetta, sviluppare con le ricette
                                                                    pua,
                                                                    codiceFertilizzante:=dettaglioFertilizzazione.prodotto.codice,
                                                                    soloLetturaAnagrafica:=True,
                                                                    objParametri_Super_Server,
                                                                    objParametri_Server,
                                                                    objParametri_Utenti)

                        If Not IsNothing(fertilizzantiList) AndAlso fertilizzantiList.Any() Then

                            If fertilizzantiList.Count = 1 Then

                                Dim fertilizzante = fertilizzantiList.FirstOrDefault()

                                dettaglioFertilizzazione.effluente = fertilizzante.effluente
                                dettaglioFertilizzazione.tipoFertilizzante = fertilizzante.tipoFertilizzante
                                dettaglioFertilizzazione.tipologieFertilizzante = fertilizzante.tipologieFertilizzante
                            Else
                                fertilizzantiAmbigui.Add(dettaglioFertilizzazione)
                            End If
                        Else
                            fertilizzantiNonCorretti.Add(dettaglioFertilizzazione)
                        End If

                    Case ClassType.DettaglioIrrigazione

                        Dim dettaglioIrrigazione = CType(risorsa, dettagli.DettaglioIrrigazione)

                        GetUdM(dettaglioIrrigazione.unitaDiMisura, flagTipoDose:=-1, objParametri_Server)
                        If dettaglioIrrigazione.unitaDiMisura.simbolo.EndsWith("/ha") Then
                            dettaglioIrrigazione.unitaDiMisura.simbolo = dettaglioIrrigazione.unitaDiMisura.simbolo.Replace("/ha", "/Ha")
                        End If

                        Dim objIrrigazioni As New AgronicaCoreMetaSchemaDAL.ImpiantiIrrigazioni_R
                        Dim dtIrrigazioni = objIrrigazioni.Leggi(dettaglioIrrigazione.tipoIrrigazione.codice,
                                  enumSelezioneVariabile.Selezione_TabellaCompleta,
                                  "",
                                  "", objParametri_Server)

                        If dtIrrigazioni IsNot Nothing AndAlso dtIrrigazioni.Rows.Count = 1 Then

                            dettaglioIrrigazione.tipoIrrigazione.descrizione = dtIrrigazioni.Rows(0).Item("Imp_DES")
                        End If

                        ' aggiungo informazioni sulla macchina per irrigazioni se presente
                        If Not IsNothing(dettaglioIrrigazione.macchina) AndAlso dettaglioIrrigazione.macchina.codice <> 0 Then
                            Dim parco_macchine_R As New AgronicaCoreContabBIZ.Parco_Macchine_R()
                            dettaglioIrrigazione.macchina = parco_macchine_R.Leggi_Macchina(Piva:="", dettaglioIrrigazione.macchina.codice, objParametri_Server)
                        End If

                        'Aggiungo le informazioni dell'eventuale consiglio DSS salvato
                        If Not IsNothing(dettaglioIrrigazione.consiglioIrrigazione) AndAlso dettaglioIrrigazione.consiglioIrrigazione.codice > 0 Then

                            If IsNothing(dtDSSIrrigazione) Then
                                Dim objDSSIrrigazione As New AgronicaCoreMeteoDAL.DSS_Irrigazione_R

                                dtDSSIrrigazione = objDSSIrrigazione.Leggi_Precedenti(Piva:=attivita.centroAziendale.primaryKey.partitaIva,
                                                                                      0, 0, 0, 0, AGRODATAINIZIO, AGRODATAINIZIO,
                                                                                        "", "", objParametri_Server)
                            End If

                            If Not IsNothing(dtDSSIrrigazione) AndAlso dtDSSIrrigazione.Rows.Count > 0 Then
                                Dim dr = dtDSSIrrigazione.Select("ID_DSS_Irrigazione = " & dettaglioIrrigazione.consiglioIrrigazione.codice)

                                If Not IsNothing(dr) AndAlso dr.Count = 1 Then
                                    dettaglioIrrigazione.consiglioIrrigazione.descrizione = dr(0)("Descrizione_DSS_Irrigazione")
                                    dettaglioIrrigazione.consiglioIrrigazione.dataEsecuzione = dr(0)("Data_Esecuzione")
                                    dettaglioIrrigazione.consiglioIrrigazione.dataConsiglio = dr(0)("Data_Consiglio")
                                    dettaglioIrrigazione.consiglioIrrigazione.qtaAcqua = dr(0)("Qta_Acqua_DSS_Irrigazione")
                                    dettaglioIrrigazione.consiglioIrrigazione.unitaDiMisura = New UnitaDiMisura(dr(0)("Udm_Cod_DSS_Irrigazione")) With {
                                        .descrizione = dr(0)("Udm_Des_DSS_Irrigazione"),
                                        .simbolo = dr(0)("Udm_Sim_DSS_Irrigazione")
                                    }
                                    dettaglioIrrigazione.consiglioIrrigazione.providerConsiglio = dr(0)("Provider_Consiglio")
                                    dettaglioIrrigazione.consiglioIrrigazione.modelloConsiglio = dr(0)("Modello_Consiglio")
                                End If
                            End If
                        End If

                    Case ClassType.DettaglioRilievo

                        Dim dettaglioRilievo = CType(risorsa, dettagli.DettaglioRilievo)

                        Dim objRilievi As New AgronicaControlli_2010.Rilievi

                        Dim Personalizzata As Boolean = True

                        Dim LeggiPersonalizzataDaImpostazione As Boolean = True

                        Dim LeggiIndiciMaturitaLocali As Boolean = True

                        Dim veg_Cod As Integer = If(GetVegCodFromUtilizzoTerreno(attivita.utilizzoTerreno) = 0, NessunaSpecieQdC, GetVegCodFromUtilizzoTerreno(attivita.utilizzoTerreno))

                        Select Case attivita.job.primaryKey.codice
                            Case LAVCOD_RILIEVO_AVVERSITA_CAMPO

                                If IsNothing(ListIndiciMaturita) Then

                                    Dim dpiCod As Integer = 0

                                    Dim idRcdpi As Integer = 0

                                    Dim pubblicoPrivato As Integer = 0

                                    If Not IsNothing(attivita.disciplinare) Then
                                        dpiCod = attivita.disciplinare.codice

                                        If Not IsNothing(attivita.disciplinare.raggruppamentiColturaliDPI) Then
                                            idRcdpi = attivita.disciplinare.raggruppamentiColturaliDPI.codice
                                        End If

                                        pubblicoPrivato = attivita.disciplinare.disciplinarePubblicoPrivato
                                    End If

                                    ListMisuraXAvv = objRilievi.LetturaMisureXAvversita(veg_Cod, dpiCod, idRcdpi, pubblicoPrivato, 0, Personalizzata, LeggiPersonalizzataDaImpostazione,
                                                                                        objParametri_Server, objParametri_Super_Server, objParametri_Utenti)
                                End If

                                If Not IsNothing(ListMisuraXAvv) AndAlso ListMisuraXAvv.Any() Then
                                    Dim av_cod As Integer = 0
                                    Dim av_gru As Integer = 0
                                    STD_Utility.GetCodiciAvversita(dettaglioRilievo.avversitaGruppo, tipoFormulato:=0, av_cod, av_gru)

                                    dettaglioRilievo.Descrizione = ListMisuraXAvv _
                                        .Where(Function(x) x.Av_Cod = av_cod AndAlso
                                            x.Av_Gru = av_gru AndAlso x.Udm_Cod = dettaglioRilievo.unitaDiMisura.codice) _
                                        .Select(Function(x) x.Av_Des_Vol) _
                                        .DefaultIfEmpty("") _
                                        .First
                                End If

                            Case LAVCOD_RILIEVO_INDICI_MATURITA

                                If IsNothing(ListIndiciMaturita) Then
                                    ListIndiciMaturita = objRilievi.LetturaIndiciMaturita(veg_Cod, 0, Personalizzata, LeggiPersonalizzataDaImpostazione, LeggiIndiciMaturitaLocali,
                                                                                              objParametri_Server, objParametri_Super_Server, objParametri_Utenti,
                                                                                              LAVCOD_RILIEVO_INDICI_MATURITA)
                                End If


                                If Not IsNothing(ListIndiciMaturita) Then
                                    Dim index As Integer = ListIndiciMaturita.FindIndex(Function(x) x.ind_mat_cod = dettaglioRilievo.IndiceMaturita)

                                    If index > -1 Then
                                        dettaglioRilievo.Descrizione = ListIndiciMaturita(index).ind_mat_des
                                    End If
                                End If

                            Case LAVCOD_DANNI_RACCOLTA

                                If IsNothing(ListMisuraXDR) Then
                                    ListMisuraXDR = objRilievi.LetturaDanniRaccolta(veg_Cod, Personalizzata, LeggiPersonalizzataDaImpostazione, objParametri_Server, objParametri_Super_Server, objParametri_Utenti)
                                End If


                                If Not IsNothing(ListMisuraXDR) Then
                                    Dim index As Integer = ListMisuraXDR.FindIndex(Function(x) x.Dr_Cod = dettaglioRilievo.DannoRaccolta)

                                    If index > -1 Then
                                        dettaglioRilievo.Descrizione = ListMisuraXDR(index).Dr_Des
                                    End If
                                End If


                            Case LAVCOD_FASI_FENOLOGICHE

                                If IsNothing(ListFaseFenologica) Then
                                    ListFaseFenologica = objRilievi.LetturaFasiFenologiche(veg_Cod, False, Personalizzata, LeggiPersonalizzataDaImpostazione,
                                                                                           objParametri_Server, objParametri_Super_Server, objParametri_Utenti)
                                End If

                                If Not IsNothing(ListFaseFenologica) Then
                                    Dim index As Integer = ListFaseFenologica.FindIndex(Function(x) Not IsNothing(dettaglioRilievo.faseFenologica) AndAlso
                                                                                                    x.Cod_SS = dettaglioRilievo.faseFenologica.codice)

                                    If index > -1 Then
                                        dettaglioRilievo.Descrizione = ListFaseFenologica(index).Descrizione

                                        If Not IsNothing(dettaglioRilievo.faseFenologica) Then
                                            dettaglioRilievo.faseFenologica.stadioCrescitaBBCH = New baseClass.BaseCodeDescr(ListFaseFenologica(index).ID_BBCH, "")
                                            dettaglioRilievo.faseFenologica.fioritura = If(ListFaseFenologica(index).Fioritura = 1, True, False)
                                            dettaglioRilievo.faseFenologica.stadio = ListFaseFenologica(index).Stadio
                                            dettaglioRilievo.faseFenologica.specieVegetale = GetSpecieFromUtilizzoTerreno(attivita.utilizzoTerreno)
                                            dettaglioRilievo.faseFenologica.descrizione = ListFaseFenologica(index).Descrizione
                                        End If

                                    End If
                                End If

                            Case LAVCOD_RILIEVO_ERBE_INFESTANTI

                                If IsNothing(ListInfestanti) OrElse IsNothing(ListInfestanti.Item1) OrElse IsNothing(ListInfestanti.Item2) Then
                                    ListInfestanti = objRilievi.LetturaErbeInfestanti(objParametri_Server, objParametri_Super_Server, objParametri_Utenti)
                                End If

                                If Not IsNothing(ListInfestanti) AndAlso Not IsNothing(ListInfestanti.Item1) AndAlso Not IsNothing(ListInfestanti.Item2) AndAlso
                                    (ListInfestanti.Item1.Any() OrElse ListInfestanti.Item2.Any()) Then

                                    Dim av_cod As Integer = 0

                                    Dim av_gru As Integer = 0

                                    STD_Utility.GetCodiciAvversita(dettaglioRilievo.erbaInfestante, tipoFormulato:=0, av_cod, av_gru)

                                    If av_cod > 0 AndAlso av_gru = 0 Then
                                        Dim index As Integer = ListInfestanti.Item1.FindIndex(Function(x) x.AV_COD = av_cod AndAlso
                                                                                                          x.AV_GRU = av_gru)

                                        If index > -1 Then
                                            dettaglioRilievo.Descrizione = ListInfestanti.Item1(index).Av_Des_Vol
                                        End If
                                    ElseIf av_cod = 0 AndAlso av_gru > 0 Then
                                        Dim index As Integer = ListInfestanti.Item2.FindIndex(Function(x) x.AV_GRU = av_gru)

                                        If index > -1 Then
                                            dettaglioRilievo.Descrizione = ListInfestanti.Item2(index).Av_Gru_Des
                                        End If
                                    End If

                                End If

                            Case LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA

                                If Not IsNothing(ListIndiciRese) Then
                                    ListIndiciRese = objRilievi.LetturaIndiciMaturita(veg_Cod, 1, Personalizzata, LeggiPersonalizzataDaImpostazione, LeggiIndiciMaturitaLocali,
                                                                                        objParametri_Server, objParametri_Super_Server, objParametri_Utenti, LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA)
                                End If

                                If Not IsNothing(ListIndiciRese) Then
                                    Dim index As Integer = ListIndiciRese.FindIndex(Function(x) x.ind_mat_cod = dettaglioRilievo.IndiceResa)

                                    If index > -1 Then
                                        dettaglioRilievo.Descrizione = ListIndiciRese(index).ind_mat_des
                                    End If
                                End If

                            Case LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE

                                Dim eserciziCDC As List(Of centri_di_costo.EsercizioCDC) = attivita.centriDiCosto.ConvertAll(Of centri_di_costo.EsercizioCDC)(Function(c) CType(c, centri_di_costo.EsercizioCDC)).ToList

                                If IsNothing(ListAvversitaTrappole) Then
                                    ListAvversitaTrappole = objRilievi.LetturaAvversitaTrappole(eserciziCDC, dettaglioRilievo.avversitaGruppo, objParametri_Server)
                                End If

                                If Not IsNothing(ListAvversitaTrappole) Then
                                    Dim index As Integer = ListAvversitaTrappole.FindIndex(Function(x) x.unitaDiMisura.codice = dettaglioRilievo.unitaDiMisura.codice)

                                    If index > -1 Then
                                        dettaglioRilievo.Descrizione = ListAvversitaTrappole(index).descrizione

                                        If Not IsNothing(ListAvversitaTrappole(index).utilizzi) AndAlso ListAvversitaTrappole(index).utilizzi.Count > 0 Then

                                            Dim utilizzoTrappole = ListAvversitaTrappole(index).utilizzi.Find(Function(x) Not IsNothing(x.risorsaProdotto) AndAlso
                                                                                                                         Not IsNothing(x.risorsaProdotto.prodotto) AndAlso
                                                                                                                        x.risorsaProdotto.prodotto.codice = dettaglioRilievo.risorsaProdotto.prodotto.codice)

                                            If Not IsNothing(utilizzoTrappole) Then
                                                dettaglioRilievo.risorsaProdotto = utilizzoTrappole.risorsaProdotto
                                            End If
                                        End If
                                    End If
                                End If

                        End Select


                End Select

                If risorsa.GetType.BaseType Is GetType(risorse.RisorsaProdotto) Then
                    Dim risorsaProdotto = CType(risorsa, risorse.RisorsaProdotto)
                    risorsaProdotto.prodotto.descrizione = GetDescrizioneProdotto(attivita.centroAziendale.primaryKey.partitaIva, risorsaProdotto.prodotto, objParametri_Server)
                    GetUdM(risorsaProdotto.unitaDiMisura, flagTipoDose:=-1, objParametri_Server)
                    GetUdM(risorsaProdotto.unitaDiMisuraIndicata, flagTipoDose:=risorsaProdotto.flagTipoDose, objParametri_Server)
                    GetDescrizioneMagazzini(risorsaProdotto.MagazziniMovimentazioni, risorsaProdotto.prodotto.descrizione, objParametri_Server)

                    If Not IsNothing(risorsaProdotto.MagazziniMovimentazioni) Then
                        For Each rilevamentoMagazzino In risorsaProdotto.MagazziniMovimentazioni
                            If Not IsNothing(rilevamentoMagazzino.registrazioniCollegate) Then
                                For Each registrazione In rilevamentoMagazzino.registrazioniCollegate
                                    CompleteVerbose(registrazione, isRibaltamentoToAgenda, New List(Of Parametri_Aggiuntivi_Attivita), objParametri_Super_Server, objParametri_Server, objParametri_Utenti)
                                Next
                            End If
                        Next
                    End If
                End If

                If risorsa.GetType Is GetType(risorse.RisorsaRegistrazione) Then
                    Dim risorsaRegistrazione = CType(risorsa, risorse.RisorsaRegistrazione)
                    risorsaRegistrazione.prodotto.descrizione = GetDescrizioneProdotto(attivita.centroAziendale.primaryKey.partitaIva, risorsaRegistrazione.prodotto, objParametri_Server)
                    GetDescrizioneMagazzini(risorsaRegistrazione.MagazziniMovimentazioni, risorsaRegistrazione.prodotto.descrizione, objParametri_Server)
                End If
            Next
        End If

        If listParametriAggiuntivi IsNot Nothing Then

            If formulatiNonCorretti.Any() Then
                Dim paramAggiuntivo = New Parametri_Aggiuntivi_Attivita
                paramAggiuntivo.key = Key_Parametri_Aggiuntivi_Attivita.Formulati_Non_Corretti
                paramAggiuntivo.value = JsonConvert.SerializeObject(formulatiNonCorretti)
                listParametriAggiuntivi.Add(paramAggiuntivo)
            End If

            If formulatiAmbigui.Any() Then
                Dim paramAggiuntivo = New Parametri_Aggiuntivi_Attivita
                paramAggiuntivo.key = Key_Parametri_Aggiuntivi_Attivita.Formulati_Ambigui
                paramAggiuntivo.value = JsonConvert.SerializeObject(formulatiAmbigui)
                listParametriAggiuntivi.Add(paramAggiuntivo)
            End If

            If avversitaNonCorrette.Any() Then
                Dim paramAggiuntivo = New Parametri_Aggiuntivi_Attivita
                paramAggiuntivo.key = Key_Parametri_Aggiuntivi_Attivita.Avversita_Non_Corrette
                paramAggiuntivo.value = JsonConvert.SerializeObject(avversitaNonCorrette)
                listParametriAggiuntivi.Add(paramAggiuntivo)
            End If

            If avversitaAmbigue.Any() Then
                Dim paramAggiuntivo = New Parametri_Aggiuntivi_Attivita
                paramAggiuntivo.key = Key_Parametri_Aggiuntivi_Attivita.Avversita_Ambigue
                paramAggiuntivo.value = JsonConvert.SerializeObject(avversitaAmbigue)
                listParametriAggiuntivi.Add(paramAggiuntivo)
            End If

            If avversitaNonValorizzate.Any() Then
                Dim paramAggiuntivo = New Parametri_Aggiuntivi_Attivita
                paramAggiuntivo.key = Key_Parametri_Aggiuntivi_Attivita.Avversita_Non_Valorizzate
                paramAggiuntivo.value = JsonConvert.SerializeObject(avversitaNonValorizzate)
                listParametriAggiuntivi.Add(paramAggiuntivo)
            End If

            If fertilizzantiNonCorretti.Any() Then
                Dim paramAggiuntivo = New Parametri_Aggiuntivi_Attivita
                paramAggiuntivo.key = Key_Parametri_Aggiuntivi_Attivita.Fertilizzanti_Non_Corretti
                paramAggiuntivo.value = JsonConvert.SerializeObject(fertilizzantiNonCorretti)
                listParametriAggiuntivi.Add(paramAggiuntivo)
            End If

            If fertilizzantiAmbigui.Any() Then
                Dim paramAggiuntivo = New Parametri_Aggiuntivi_Attivita
                paramAggiuntivo.key = Key_Parametri_Aggiuntivi_Attivita.Fertilizzanti_Ambigui
                paramAggiuntivo.value = JsonConvert.SerializeObject(fertilizzantiAmbigui)
                listParametriAggiuntivi.Add(paramAggiuntivo)
            End If

        End If
    End Sub

    Public Shared Function GetDescrizioneProdotto(piva As String, prodotto As risorse.Prodotto, objParametri_Server As AgronicaCoreParametri) As String

        Dim descrizioneProdotto = ""

        Select Case prodotto.elemCod
            Case FORMULATI
                Dim objDes As New AgronicaCoreMetaSchemaDAL.Formulati_R
                descrizioneProdotto = objDes.FrDes_from_FrCod(prodotto.codice, objParametri_Server)
            Case FERTILIZZANTI
                Dim objDes As New AgronicaCoreMetaSchemaDAL.Fertilizzanti_R
                descrizioneProdotto = objDes.FerDes_from_FerCod(prodotto.codice, objParametri_Server)
            Case SEMENTI
                Dim objDes As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
                descrizioneProdotto = objDes.MatDes_from_MatCod(piva, SEMENTI, prodotto.codice, "", "", "", objParametri_Server)
            Case INSETTI
                Dim objDes As New AgronicaCoreMetaSchemaDAL.InsettiUtili_R
                descrizioneProdotto = objDes.InsDes_from_InsCod(prodotto.codice, objParametri_Server)
            Case TRAPPOLE
                Dim objDes As New AgronicaCoreMetaSchemaDAL.Trappole_R
                descrizioneProdotto = objDes.TrapDes_from_TrapCod(prodotto.codice, objParametri_Server)
        End Select

        Return descrizioneProdotto

    End Function

    Public Shared Sub GetDescrizioneMagazzini(ByRef MagazziniMovimentazioni As List(Of RilevamentoDiMagazzino), descrizioneProdotto As String, objParametri_Server As AgronicaCoreParametri)

        If MagazziniMovimentazioni IsNot Nothing AndAlso MagazziniMovimentazioni.Count > 0 Then
            Dim ObjFabbricati = New AgronicaCoreAnagrafeDAL.Fabbricati_R
            For Each rilevamentoMagazzino In MagazziniMovimentazioni

                If rilevamentoMagazzino.Magazzino IsNot Nothing Then
                    Dim DtFabbricati As DataTable = ObjFabbricati.Leggi(rilevamentoMagazzino.Magazzino.primaryKey.centroAziendalePK.partitaIva,
                                                        rilevamentoMagazzino.Magazzino.primaryKey.centroAziendalePK.codice,
                                                        rilevamentoMagazzino.Magazzino.primaryKey.codice,
                                                        enumSelezioneVariabile.Selezione_JoinCompleta,
                                                        "",
                                                        "",
                                                        objParametri_Server)

                    If DtFabbricati IsNot Nothing AndAlso DtFabbricati.Rows.Count > 0 Then
                        rilevamentoMagazzino.Magazzino.descrizione = CStr(DtFabbricati.Rows(0).Item("Fabbricato_Des")) & " (" & CStr(DtFabbricati.Rows(0).Item("Sa_Nome")) & ")"

                        Dim DrFabbricato_Uso_da_Terzi = DtFabbricati.Select("Fabbricato_Codice = " & enum_CodiciAnagrafe.Fabbricato_Uso_da_Terzi & " And Fabbricato_Codice_Val = '1'")

                        If Not IsNothing(DrFabbricato_Uso_da_Terzi) AndAlso DrFabbricato_Uso_da_Terzi.Count > 0 Then
                            rilevamentoMagazzino.Magazzino.usoDaTerzi = True
                        Else
                            rilevamentoMagazzino.Magazzino.usoDaTerzi = False
                        End If
                    End If
                End If

                rilevamentoMagazzino.Prodotto.descrizione = descrizioneProdotto
                GetUdM(rilevamentoMagazzino.udm, flagTipoDose:=-1, objParametri_Server)
            Next
        End If

    End Sub

    Public Shared Sub GetUdM(ByRef udm As UnitaDiMisura, flagTipoDose As Integer, objParametri_Server As AgronicaCoreParametri)

        If udm IsNot Nothing AndAlso udm.codice <> 0 Then

            If List_UdM.Count = 0 Then
                Dim objDesUM As New AgronicaControlli_2010.STD_UnitaDiMisura
                List_UdM = objDesUM.LeggiUnitaDiMisuraConTipoControllo(objParametri_Server)
            End If

            Dim Udm_Cod As Integer = udm.codice

            udm = List_UdM.Find(Function(u) u.codice = Udm_Cod).Clona()

            Select Case flagTipoDose
                Case enum_TipoMezzo.Ettolitro
                    If udm.simbolo.Contains("/hl") Then
                        udm.simbolo = "[" & udm.simbolo & "]"
                    Else
                        udm.simbolo = "[" & udm.simbolo & "/hl]"
                    End If
                Case enum_TipoMezzo.Ettaro
                    If udm.simbolo.Contains("/ha") Then
                        udm.simbolo = "[" & udm.simbolo & "]"
                    Else
                        udm.simbolo = "[" & udm.simbolo & "/ha]"
                    End If
            End Select

        End If

    End Sub

    Public Shared Function GetTipoRichiesto(ByVal AvCod As String, ByVal AvGru As String, ByVal Lav_Cod As Integer) As Integer
        Dim tipoRichiesto As Integer = 0

        If AvGru = "-1" And AvCod = "-1" Then
            tipoRichiesto = enum_TipoFormulato.Coadiuvanti
        ElseIf AvGru = "-2" And AvCod = "-2" Then
            tipoRichiesto = enum_TipoFormulato.Corroboranti_Fisiofarmaci
        Else
            Select Case Lav_Cod
                Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO
                    tipoRichiesto = enum_TipoFormulato.Antiparassitari

                Case LAVCOD_CONCIA_SEME
                    tipoRichiesto = enum_TipoFormulato.Concianti

                Case LAVCOD_DISSECCAMENTO
                    tipoRichiesto = enum_TipoFormulato.Disseccanti

                Case LAVCOD_GEODISINFESTAZIONE
                    tipoRichiesto = enum_TipoFormulato.Geodisinfestanti

                Case LAVCOD_TRATTAMENTO_FITOREGOLATORE
                    tipoRichiesto = enum_TipoFormulato.Fitoregolatori

                Case LAVCOD_DISERBO
                    tipoRichiesto = enum_TipoFormulato.Diserbanti

                Case LAVCOD_CONFUSIONE_SESSUALE
                    tipoRichiesto = enum_TipoFormulato.ConfusioneSessuale

                Case LAVCOD_DISORIENTAMENTO_SESSUALE
                    tipoRichiesto = enum_TipoFormulato.DisorientamentoSessuale

            End Select
        End If

        Return tipoRichiesto

    End Function

    Public Shared Function GetVegCodFromUtilizzoTerreno(ByVal utilizzoTerreno As UtilizzoTerreno) As Integer

        Dim veg_cod As Integer = 0

        If utilizzoTerreno IsNot Nothing Then
            Select Case utilizzoTerreno.classType
                Case ClassType.Varieta
                    veg_cod = CType(utilizzoTerreno, Varieta).specie.codice

                Case ClassType.DestinazioneUso
                    veg_cod = 0

            End Select
        End If

        Return veg_cod

    End Function

    Public Shared Function GetIdCodFromUtilizzoTerreno(ByVal utilizzoTerreno As UtilizzoTerreno) As Integer

        Dim id_cod As Integer = 0

        If utilizzoTerreno IsNot Nothing Then
            Select Case utilizzoTerreno.classType
                Case ClassType.Varieta
                    id_cod = 0

                Case ClassType.DestinazioneUso
                    id_cod = utilizzoTerreno.codice

            End Select
        End If

        Return id_cod

    End Function

    Public Shared Function GetPrincipiAttivi(ByVal principiAttiviTitoli As String, ByVal principiAttiviPesi As String, ByVal principiAttiviPercentualiSuperficieTrattabile As String) As List(Of PrincipioAttivo)
        Dim principiAttivi As New List(Of PrincipioAttivo)

        Dim dictPrincipiPesi As New Dictionary(Of Integer, Decimal)
        Dim principiPesiArray = Split(principiAttiviPesi, "|")
        For Each itemPrincipioPeso In principiPesiArray
            If itemPrincipioPeso <> "" Then
                Dim arrayItemPrincipioPeso = itemPrincipioPeso.Split("§")
                If Not dictPrincipiPesi.ContainsKey(arrayItemPrincipioPeso(0)) Then
                    dictPrincipiPesi.Add(arrayItemPrincipioPeso(0), Decimal.Parse(arrayItemPrincipioPeso(1), CultureInfo.InvariantCulture))
                End If
            End If
        Next

        Dim dictPrincipiPerc As New Dictionary(Of Integer, Decimal)
        Dim principiPercArray = Split(principiAttiviPercentualiSuperficieTrattabile, "|")
        For Each itemPrincipioPerc In principiPercArray
            If itemPrincipioPerc <> "" Then
                Dim arrayItemPrincipioPerc = itemPrincipioPerc.Split("§")
                If Not dictPrincipiPerc.ContainsKey(arrayItemPrincipioPerc(0)) Then
                    'DT: la percentuale di abbattimento su db è salvata con la virgola decimale, invece che il punto decimale.
                    'Poichè a tendere questa cosa potrebbe essere corretta, il seguente codice gestisce correttamente sia la virgola che il punto.
                    If (arrayItemPrincipioPerc(1).Contains(",")) Then
                        dictPrincipiPerc.Add(arrayItemPrincipioPerc(0), arrayItemPrincipioPerc(1))
                    Else
                        dictPrincipiPerc.Add(arrayItemPrincipioPerc(0), Decimal.Parse(arrayItemPrincipioPerc(1), CultureInfo.InvariantCulture))
                    End If
                End If
            End If
        Next

        If principiAttiviTitoli <> "" Then
            Dim principiTitoliArray = Split(principiAttiviTitoli, "|")
            For Each itemPrincipioTitolo In principiTitoliArray
                If itemPrincipioTitolo <> "" Then
                    Dim arrayItemPrincipioTitolo = itemPrincipioTitolo.Split("§")
                    If arrayItemPrincipioTitolo.Length = 2 Then
                        Dim principio As New PrincipioAttivo With {
                            .codice = arrayItemPrincipioTitolo(0),
                            .titolo = Decimal.Parse(arrayItemPrincipioTitolo(1), CultureInfo.InvariantCulture)
                        }
                        If dictPrincipiPesi.ContainsKey(arrayItemPrincipioTitolo(0)) Then
                            principio.peso = dictPrincipiPesi(arrayItemPrincipioTitolo(0))
                        End If
                        If dictPrincipiPerc.ContainsKey(arrayItemPrincipioTitolo(0)) Then
                            principio.percentualeSuperficieTrattabile = dictPrincipiPerc(arrayItemPrincipioTitolo(0))
                        End If
                        principiAttivi.Add(principio)
                    End If
                End If
            Next
        End If

        Return principiAttivi
    End Function

    Public Shared Function GetBufferZone(ByVal buffer As String) As BufferZone

        Dim bufferZone As New BufferZone

        If buffer <> "" Then
            Dim bufferArray = Split(buffer, "|")
            If bufferArray.Length = 2 Then
                bufferZone.minimo = bufferArray(0)
                bufferZone.massimo = bufferArray(1)
            End If
        Else
            bufferZone.minimo = 0
            bufferZone.massimo = 0
        End If

        Return bufferZone

    End Function

    Public Shared Function GetCodiceProdotto(Pro_Cod As Integer, Mat_Cod As Integer, infoOperazione As InfoOperazione) As Integer

        Dim prodottoCod = 0

        If Mat_Cod = 0 Then
            If infoOperazione.IsSemina Then 'DT: compatibilità con il pregresso
                If Pro_Cod = 1 Then
                    Pro_Cod = 0
                End If
            End If
            prodottoCod = Pro_Cod
        Else
            prodottoCod = Math.Abs(Mat_Cod) 'DT: ho dovuto mettere abs perchè, in funzione dei contesti, può arrivare negativo
        End If

        Return prodottoCod

    End Function

    Public Shared Function GetDettaglioProdotto(ByVal extra_str As String) As Integer

        Dim dettProdotto As Integer = 0

        If extra_str IsNot Nothing Then
            Integer.TryParse(extra_str, dettProdotto)
        End If

        Return dettProdotto

    End Function

    Public Shared Function GetAvversitaGruppo(av_cod As Integer, av_gru As Integer) As AvversitaGruppo

        Dim avversitaGruppo As AvversitaGruppo = Nothing

        If av_cod <> 0 Then
            avversitaGruppo = New AgronicaCoreModelsSTD.metaschema.avversita.Avversita(av_cod)

            'DT: se avversità singola con codice negativo, valorizzare con lo stesso valore il gruppo
            If av_cod < 0 Then
                CType(avversitaGruppo, AgronicaCoreModelsSTD.metaschema.avversita.Avversita).gruppo = New AgronicaCoreModelsSTD.metaschema.avversita.GruppoAvversita(av_gru)
            Else
                CType(avversitaGruppo, AgronicaCoreModelsSTD.metaschema.avversita.Avversita).gruppo = New AgronicaCoreModelsSTD.metaschema.avversita.GruppoAvversita(0)
            End If

        ElseIf av_gru <> 0 Then
            avversitaGruppo = New AgronicaCoreModelsSTD.metaschema.avversita.GruppoAvversita(av_gru)
        End If

        Return avversitaGruppo

    End Function

    Public Shared Function GetTipoFormulato(ByVal avversitaGruppo As AvversitaGruppo, lav_cod As Integer) As Integer

        Dim tipoFormulato As Integer = 0

        If avversitaGruppo IsNot Nothing Then
            Select Case avversitaGruppo.classType
                Case ClassType.Avversita
                    tipoFormulato = GetTipoRichiesto(avversitaGruppo.codice, CType(avversitaGruppo, AgronicaCoreModelsSTD.metaschema.avversita.Avversita).gruppo.codice, lav_cod)
                Case ClassType.GruppoAvversita
                    tipoFormulato = GetTipoRichiesto(avversitaGruppo.codice, 0, lav_cod)
            End Select
        End If

        Return tipoFormulato

    End Function

    Public Shared Function GetSoglia(Soglia_Cod As Integer, Soglia_Quantita As Decimal) As Soglia

        Dim soglia As New Soglia

        soglia.codice = Soglia_Cod
        soglia.quantita = Decimal.Parse(Soglia_Quantita, CultureInfo.InvariantCulture)

        Return soglia

    End Function

    Public Shared Function GetSpecieFromUtilizzoTerreno(ByVal utilizzoTerreno As UtilizzoTerreno) As utilizzi.Specie

        Dim specie As New utilizzi.Specie(0)

        If utilizzoTerreno IsNot Nothing AndAlso utilizzoTerreno.classType = ClassType.Varieta Then
            specie = CType(utilizzoTerreno, utilizzi.Varieta).specie
        End If

        Return specie

    End Function

    Public Shared Function GetSpecieFromProdottiTrattati(prodottiDaTrattare As List(Of ProdottoDaTrattareCDC)) As utilizzi.Specie

        Dim specie As New utilizzi.Specie(0)

        If prodottiDaTrattare IsNot Nothing AndAlso prodottiDaTrattare.Count > 0 Then
            Dim prodotto = prodottiDaTrattare(0).giacenzaMagazzino.Prodotto
            Dim specieProdotto = prodotto.specie.codice
            specie = New utilizzi.Specie(specieProdotto)
        End If

        Return specie

    End Function

    Public Shared Function GetDoseTrasformata(doseIndicata As Decimal, udmIndicata As Integer) As Decimal
        Dim doseTrasformata As Decimal = 0

        Select Case udmIndicata
            Case enum_UnitaMisura.Grammi
                doseTrasformata = doseIndicata / 1000
            Case enum_UnitaMisura.Milligrammi
                doseTrasformata = doseIndicata / 1000000
            Case enum_UnitaMisura.Quintali
                doseTrasformata = doseIndicata * 100
            Case enum_UnitaMisura.Tonnellate
                doseTrasformata = doseIndicata * 1000
            Case enum_UnitaMisura.Millilitri
                doseTrasformata = doseIndicata / 1000
            Case enum_UnitaMisura.CentimetriCubi
                doseTrasformata = doseIndicata / 1000
            Case enum_UnitaMisura.Metri_Cubi
                doseTrasformata = doseIndicata * 1000
            Case enum_UnitaMisura.Litri, enum_UnitaMisura.KG, enum_UnitaMisura.Unita_Seme, enum_UnitaMisura.Num_Piante, enum_UnitaMisura.Confezioni, enum_UnitaMisura.Numero, enum_UnitaMisura.Numero_Trappole
                doseTrasformata = doseIndicata
            Case Else
                doseTrasformata = doseIndicata
        End Select

        Return doseTrasformata
    End Function

    Public Shared Function GetDosePerMagazzino(doseTrasformata As Decimal, udmIndicata As Integer) As Decimal
        Dim doseIndicata As Decimal = 0

        Select Case udmIndicata
            Case enum_UnitaMisura.Grammi
                doseIndicata = doseTrasformata * 1000
            Case enum_UnitaMisura.Milligrammi
                doseIndicata = doseTrasformata * 1000000
            Case enum_UnitaMisura.Quintali
                doseIndicata = doseTrasformata / 100
            Case enum_UnitaMisura.Tonnellate
                doseIndicata = doseTrasformata / 1000
            Case enum_UnitaMisura.Millilitri
                doseIndicata = doseTrasformata * 1000
            Case enum_UnitaMisura.CentimetriCubi
                doseIndicata = doseTrasformata * 1000
            Case enum_UnitaMisura.Metri_Cubi
                doseIndicata = doseTrasformata / 1000
            Case enum_UnitaMisura.Litri, enum_UnitaMisura.KG, enum_UnitaMisura.Unita_Seme, enum_UnitaMisura.Num_Piante, enum_UnitaMisura.Confezioni, enum_UnitaMisura.Numero, enum_UnitaMisura.Numero_Trappole
                doseIndicata = doseTrasformata
            Case Else
                doseIndicata = doseTrasformata
        End Select

        Return doseIndicata
    End Function

    Public Shared Function GetTrappola(trap_cod As Integer) As Trappola

        Dim trappola As Trappola = Nothing

        If trap_cod <> 0 Then
            trappola = New Trappola(trap_cod)
        End If

        Return trappola

    End Function

    Public Shared Function GetDittaTrappola(ditta_cod As Integer) As AgronicaCoreModelsSTD.metaschema.Ditta

        Dim dittaTrappola As AgronicaCoreModelsSTD.metaschema.Ditta = Nothing

        If ditta_cod <> 0 Then
            dittaTrappola = New AgronicaCoreModelsSTD.metaschema.Ditta(ditta_cod)
        End If

        Return dittaTrappola

    End Function

    Public Shared Function GetDurataFeromone(trap_cod As Integer, objParametri_server As AgronicaCoreParametri) As Integer

        Dim giorni As Integer = 0

        If trap_cod <> 0 Then

            Dim Dt As DataTable
            Dim objTrappole As New AgronicaCoreMetaSchemaDAL.Trappole_R

            'Recupero le informazioni	
            Dt = objTrappole.Leggi(CInt(trap_cod),
                                   0, 0, 0,
                                   AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                   "", "", objParametri_server)

            'Se il recordset non è chiuso allora ...	
            If Not Dt Is Nothing AndAlso Dt.Rows.Count > 0 Then
                giorni = Dt.Rows(0).Item("Trap_Dur")
            End If
        End If

        Return giorni

    End Function
    Public Shared Function DosiEtichetta_from_Stringhe(doseEtichettaDescr As String, doseEtichettaValue As String, verbose As Boolean, objParametri_Server As AgronicaCoreParametri) As List(Of DoseEtichetta)
        Dim dosiEtichette As List(Of DoseEtichetta) = Nothing

        If Not String.IsNullOrEmpty(doseEtichettaDescr) AndAlso Not String.IsNullOrEmpty(doseEtichettaValue) Then
            dosiEtichette = New List(Of DoseEtichetta)

            Dim arrayDosi = Split(doseEtichettaDescr, "<br>")
            Dim arrayDosiValue = Split(doseEtichettaValue, "<br>")

            For i = 0 To arrayDosi.Length - 1
                Dim doseEtichetta = GetDoseEtichettaFromDoseValue(arrayDosiValue(i), objParametri_Server)
                doseEtichetta.CodiceConcatenato = arrayDosiValue(i)
                doseEtichetta.DescrizioneConcatenata = arrayDosi(i)

                If verbose Then
                    doseEtichetta.Flag_Fioritura.descrizione = STD_Utility.getFlagFiorituraDes(doseEtichetta, objParametri_Server)
                    doseEtichetta.Flag_Protetto.descrizione = STD_Utility.getFlagProtettoDes(doseEtichetta, objParametri_Server)
                    doseEtichetta.Mdi.descrizione = STD_Utility.getMidDes(doseEtichetta, objParametri_Server)
                    doseEtichetta.Epoca_Des = STD_Utility.getEpocaDes(doseEtichetta, objParametri_Server)
                End If
                dosiEtichette.Add(doseEtichetta)
            Next

        End If

        Return dosiEtichette

    End Function

    Private Shared Function GetDoseEtichettaFromDoseValue(ByVal strDoseValue As String, objParametri_Server As AgronicaCoreParametri) As DoseEtichetta

        Dim doseEtichetta As DoseEtichetta = Nothing

        Dim ArrayDoseValue() As String
        If strDoseValue <> "" And strDoseValue <> "0" Then

            ArrayDoseValue = Split(strDoseValue, "$")

            If ArrayDoseValue IsNot Nothing AndAlso ArrayDoseValue.Length > 0 Then

                Select Case ArrayDoseValue.Length

                    Case 24

                        doseEtichetta = New DoseEtichetta With {
                            .codice = ArrayDoseValue(0),
                            .descrizione = "",
                            .DoseMin = ArrayDoseValue(1),
                            .DoseMax = ArrayDoseValue(2),
                            .Udm = New UnitaDiMisura With {.codice = ArrayDoseValue(3), .simbolo = ArrayDoseValue(4)},
                            .AcquaMin = ArrayDoseValue(5),
                            .AcquaMax = ArrayDoseValue(6),
                            .UdmAcqua = New UnitaDiMisura With {.codice = ArrayDoseValue(7), .simbolo = ArrayDoseValue(8)},
                            .Da_Epoca = ArrayDoseValue(9),
                            .A_Epoca = ArrayDoseValue(10),
                            .Limite = ArrayDoseValue(11),
                            .UdmLimite = New UnitaDiMisura With {.codice = ArrayDoseValue(12), .simbolo = ArrayDoseValue(13)},
                            .strCLTOSS_Grado = ArrayDoseValue(14),
                            .Flag_Fioritura = New FlagFioritura(ArrayDoseValue(15)),
                            .IntervalloTrattamenti_Min = ArrayDoseValue(16),
                            .IntervalloTrattamenti_Max = ArrayDoseValue(17),
                            .Mdi = New Mdi(ArrayDoseValue(18)),
                            .Flag_Protetto = New FlagProtetto(ArrayDoseValue(19)),
                            .FormulatiXAllegatiNormative_IDRiga = ArrayDoseValue(20),
                            .DataSmaltimentoScorte = ArrayDoseValue(21),
                            .Gruppo_Dosaggi = ArrayDoseValue(22),
                            .Num_Max_Interventi_Globali = ArrayDoseValue(23)
                        }

                    Case 23 'DT: per gestire il caso in cui non fosse stato salvato il strCLTOSS_Grado

                        doseEtichetta = New DoseEtichetta With {
                            .codice = ArrayDoseValue(0),
                            .descrizione = "",
                            .DoseMin = ArrayDoseValue(1),
                            .DoseMax = ArrayDoseValue(2),
                            .Udm = New UnitaDiMisura With {.codice = ArrayDoseValue(3), .simbolo = ArrayDoseValue(4)},
                            .AcquaMin = ArrayDoseValue(5),
                            .AcquaMax = ArrayDoseValue(6),
                            .UdmAcqua = New UnitaDiMisura With {.codice = ArrayDoseValue(7), .simbolo = ArrayDoseValue(8)},
                            .Da_Epoca = ArrayDoseValue(9),
                            .A_Epoca = ArrayDoseValue(10),
                            .Limite = ArrayDoseValue(11),
                            .UdmLimite = New UnitaDiMisura With {.codice = ArrayDoseValue(12), .simbolo = ArrayDoseValue(13)},
                            .strCLTOSS_Grado = "",
                            .Flag_Fioritura = New FlagFioritura(ArrayDoseValue(14)),
                            .IntervalloTrattamenti_Min = ArrayDoseValue(15),
                            .IntervalloTrattamenti_Max = ArrayDoseValue(16),
                            .Mdi = New Mdi(ArrayDoseValue(17)),
                            .Flag_Protetto = New FlagProtetto(ArrayDoseValue(18)),
                            .FormulatiXAllegatiNormative_IDRiga = ArrayDoseValue(19),
                            .DataSmaltimentoScorte = ArrayDoseValue(20),
                            .Gruppo_Dosaggi = ArrayDoseValue(21),
                            .Num_Max_Interventi_Globali = ArrayDoseValue(22)
                        }


                    Case Else 'DT: se il caso non è chiaro, si prende solo il codice, per evitare rotture

                        doseEtichetta = New DoseEtichetta With {
                            .codice = ArrayDoseValue(0)
                        }

                End Select
            End If
        End If

        Return doseEtichetta

    End Function

    Public Function Leggi_FlagNuovoControlloRiduzioneDiserbo(ByVal objParametri_Server As AgronicaCoreParametri) As Boolean

        Dim flagNuovoControlloRiduzioneDiserbo As Boolean = False
        Dim csr As New Configurazione_Siti_R
        Dim dt As DataTable = csr.Leggi(0, "Flag_Nuovo_Controllo_Riduzione_Diserbo", "", "", objParametri_Server)
        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            flagNuovoControlloRiduzioneDiserbo = CBool(dt.Rows(0)("Valore"))
        End If
        Return flagNuovoControlloRiduzioneDiserbo

    End Function

    Private Function OttieniPercAbbDaApplicareSuCambioFormulato(XML_DatiPrincipiAttivi As System.Xml.XmlElement) As Decimal

        Dim i_principi_Attivi As Integer = 0
        Dim XMLs_PrincipiAttivo As System.Xml.XmlNodeList = Nothing
        Dim XML_Principio_Attivo As System.Xml.XmlElement = Nothing
        Dim percAbbDaApplicare As Decimal = 100

        If Not IsNothing(XML_DatiPrincipiAttivi) Then

            XMLs_PrincipiAttivo = XML_DatiPrincipiAttivi.GetElementsByTagName("PrincipioAttivo")

            Do While i_principi_Attivi < XMLs_PrincipiAttivo.Count

                XML_Principio_Attivo = XMLs_PrincipiAttivo.Item(i_principi_Attivi)

                Dim percAbbDpi As Decimal = 0
                If XML_Principio_Attivo.HasAttribute("perc_abb_dpi") AndAlso XML_Principio_Attivo.GetAttribute("perc_abb_dpi") <> "" Then
                    If IsNumeric(XML_Principio_Attivo.GetAttribute("perc_abb_dpi")) Then
                        percAbbDpi = CDec(XML_Principio_Attivo.GetAttribute("perc_abb_dpi"))

                        If percAbbDpi > 0 AndAlso percAbbDpi < 100 Then
                            If percAbbDpi < percAbbDaApplicare Then
                                percAbbDaApplicare = percAbbDpi
                            End If
                        End If

                    End If
                End If

                i_principi_Attivi += 1

            Loop

        End If

        Return IIf(percAbbDaApplicare = 100, 0, percAbbDaApplicare)

    End Function

    Private Function OttieniPrincipiAttiviPercAbbPerFormulatoSelezionato(ByVal frCod As Integer, ByVal XML_DatiPrincipiAttivi As System.Xml.XmlElement) As String

        Dim i_principi_Attivi As Integer = 0
        Dim XMLs_PrincipiAttivo As System.Xml.XmlNodeList = Nothing
        Dim XML_Principio_Attivo As System.Xml.XmlElement = Nothing
        Dim strPrincipiAttiviPercAbb As String = String.Empty
        Dim paPercAbb As Decimal = 0
        Dim paCod As String = ""
        Dim paList As New List(Of String)

        If Not IsNothing(XML_DatiPrincipiAttivi) Then

            XMLs_PrincipiAttivo = XML_DatiPrincipiAttivi.GetElementsByTagName("PrincipioAttivo")

            Do While i_principi_Attivi < XMLs_PrincipiAttivo.Count

                XML_Principio_Attivo = XMLs_PrincipiAttivo.Item(i_principi_Attivi)

                Dim paFrCod = CInt(XML_Principio_Attivo.GetAttribute("fr_cod"))
                If paFrCod = frCod Then
                    paCod = XML_Principio_Attivo.GetAttribute("pa_cod")
                    If XML_Principio_Attivo.HasAttribute("perc_abb_dpi") AndAlso XML_Principio_Attivo.GetAttribute("perc_abb_dpi") <> "" Then
                        paPercAbb = XML_Principio_Attivo.GetAttribute("perc_abb_dpi")
                    End If
                    paList.Add(String.Format("{0}§{1}", paCod, paPercAbb))
                End If

                i_principi_Attivi += 1

            Loop

        End If

        strPrincipiAttiviPercAbb = String.Join("|", paList)

        Return strPrincipiAttiviPercAbb

    End Function

    Public Function Imposta_DoseConsentitaDiserbo(ByVal objParametri_Super_Server As AgronicaCoreParametri,
                                                  ByVal objParametri_Server As AgronicaCoreParametri,
                                                  ByVal objParametri_Utenti As AgronicaCoreParametri,
                                                  ByVal attivita As AgronicaCoreModelsSTD.attivita.Attivita,
                                                  ByVal dettaglioTrattamento As AgronicaCoreModelsSTD.attivita.dettagli.DettaglioTrattamento,
                                                  ByVal fabbricato As AgronicaCoreModelsSTD.anagrafiche.Fabbricato,
                                                  ByVal lotto As String,
                                                  ByVal avversita As AgronicaCoreModelsSTD.metaschema.avversita.AvversitaGruppo
                                                  ) As Agenda.DoseConsentitaDiserbo

        Dim leggiLingua As New Lingue_Read
        Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod,
                                                                  AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                  "", "", objParametri_Server)

        Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
        Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

        Dim dose_ConsentitaDiserbo As New Agenda.DoseConsentitaDiserbo

        dose_ConsentitaDiserbo.D_HA_Max_Diserbo = 0

        Dim FrCod As Integer = dettaglioTrattamento.prodotto.codice

        Dim DoseMax As Decimal
        Dim Udm_Cod As Integer

        Dim Dose_Consigliata As Decimal
        Dim Dose_Consentita_Anno As Decimal
        Dim PercAbb As Decimal = 0


        ''--------------------------------------------------
        ''--------------------------------------------------
        ''--------- CREAZIONE OGGETTO DA SALVARE   ---------
        ''--------------------------------------------------
        ''--------------------------------------------------

        Dim InseritoPerControllo As Boolean = False

        If attivita.risorse IsNot Nothing AndAlso attivita.risorse.Count > 0 Then

            Dim DosePresente As Boolean = False

            For Each r As Risorsa In attivita.risorse
                If r.classType.Equals(ClassType.DettaglioTrattamento) Then
                    Dim dettaglioTrattamentoAttivita As dettagli.DettaglioTrattamento = CType(r, dettagli.DettaglioTrattamento)


                    If dettaglioTrattamentoAttivita.prodotto.codice = dettaglioTrattamento.prodotto.codice AndAlso
                      ((dettaglioTrattamentoAttivita.MagazziniMovimentazioni.Count = 0 AndAlso IsNothing(fabbricato)) OrElse (
                      dettaglioTrattamentoAttivita.MagazziniMovimentazioni.Count = 1 AndAlso Not IsNothing(fabbricato) AndAlso
                      dettaglioTrattamentoAttivita.MagazziniMovimentazioni(0).Magazzino.primaryKey.codice = fabbricato.primaryKey.codice AndAlso
                      dettaglioTrattamentoAttivita.MagazziniMovimentazioni(0).Magazzino.primaryKey.centroAziendalePK.codice = fabbricato.primaryKey.centroAziendalePK.codice AndAlso
                      dettaglioTrattamentoAttivita.MagazziniMovimentazioni(0).Magazzino.primaryKey.centroAziendalePK.partitaIva = fabbricato.primaryKey.centroAziendalePK.partitaIva AndAlso
                      dettaglioTrattamentoAttivita.MagazziniMovimentazioni(0).Lotto = lotto)) Then

                        DosePresente = True
                        Exit For

                    End If

                End If
            Next

            If DosePresente = False Then
                InseritoPerControllo = True

                dettaglioTrattamento.avversitaGruppo = avversita

                dettaglioTrattamento.unitaDiMisura = New UnitaDiMisura(0) With {
                    .descrizione = ""
                }

                dettaglioTrattamento.unitaDiMisuraIndicata = New UnitaDiMisura(0) With {
                    .descrizione = ""
                }

                dettaglioTrattamento.MagazziniMovimentazioni = New List(Of RilevamentoDiMagazzino)

                If Not IsNothing(fabbricato) Then
                    dettaglioTrattamento.MagazziniMovimentazioni.Add(New RilevamentoDiMagazzino())
                    dettaglioTrattamento.MagazziniMovimentazioni(0).Magazzino = fabbricato
                    dettaglioTrattamento.MagazziniMovimentazioni(0).Lotto = lotto
                End If

                attivita.risorse.Add(dettaglioTrattamento)
            End If

        End If

        Dim eseguiVerificheConformita As Boolean = False

        Dim obj As New AttivitaToAgenda

        Dim agenda = obj.MappaAttivitaToAgenda(attivita, objParametri_Super_Server, objParametri_Server, objParametri_Utenti, eseguiVerificheConformita)

        If IsNothing(agenda) Then
            Return dose_ConsentitaDiserbo
        End If


        Dim dtDosiVuoto As Boolean = False
        Dim movControllo = agenda.Movimenti.FirstOrDefault(Function(m) m.Cau_Mov = CAU_TRATTAMENTO)
        If Not IsNothing(movControllo) Then
            If Not IsNothing(movControllo.Movimenti_Dettagli) Then
                If movControllo.Movimenti_Dettagli.Count = 1 AndAlso InseritoPerControllo Then
                    dtDosiVuoto = True
                End If
            End If
        End If

        Dim Dpi_Cod As Integer = 0
        Dim Dpi_PubblicoPrivato As Integer = 0
        Dim Bio As Boolean = False
        'una volta creato l'oggetto agenda posso invocare il suo metodo che mi genera l'xml
        Dim Helper As New AgronicaCoreModello.OperazioneAgenda_Temp.Agenda_Operazione_Helper
        Dim StringaXmlCreazione As String = Helper.GeneraXML_CAU_TRATTAMENTO(agenda, Dpi_Cod, Dpi_PubblicoPrivato)

        StringaXmlCreazione = Replace(StringaXmlCreazione, "TipoOperazioneDB=""1""", "TipoOperazioneDB =""0""")

        '#################################################################################################
        '########################### VERIFICA CONFORMITA #################################################
        '#################################################################################################

        Dim objDpiVerifica As New AgronicaCoreDpiBIZ.DPI_Verifica
        Dim strRisultatoVerifica As String

        Dim rval As New rispostaStandard(Of AgronicaCoreDpiBIZ.Verifica_Disciplinare_Intervento)
        rval = objDpiVerifica.Verifica_Conformita_Intervento_New(objParametri_Server, objParametri_Utenti,
                                                                      attivita.centroAziendale.primaryKey.partitaIva,
                                                                      CStr(StringaXmlCreazione),
                                                                      0,
                                                                      CInt(attivita.codice),
                                                                      True,
                                                                      enum_Disciplinare_Operazione.QuelloDellOperazione, 0,
                                                                      "", 0, 0,
                                                                      0, dtDosiVuoto:=dtDosiVuoto, objParametri_Super_Server:=objParametri_Super_Server)

        If rval.RispostaOK = True Then

            strRisultatoVerifica = rval.RispostaStringa.Risultato


            '------------------------------------------
            '----- Analizzo la stringa XML
            '------------------------------------------

            If strRisultatoVerifica <> "-1" And strRisultatoVerifica <> "" Then

                Dim XmlDoc As New System.Xml.XmlDocument

                Dim XML_DatiGenerali As System.Xml.XmlElement
                Dim XML_DatiFormulati As System.Xml.XmlElement
                Dim XMLs_Formulato As System.Xml.XmlNodeList
                Dim XML_Formulato As System.Xml.XmlElement

                Dim XML_DatiPrincipiAttivi As System.Xml.XmlElement
                Dim XMLs_PrincipiAttivo As System.Xml.XmlNodeList
                Dim XML_PrincipiAttivo As System.Xml.XmlElement

                Dim i_Formulati As Integer = 0
                Dim intDummy As Integer = 100000

                'Carico la stringa nel documento XML
                XmlDoc.LoadXml(strRisultatoVerifica)

                XML_DatiGenerali = XmlDoc.SelectSingleNode("DatiRisultati/DatiGenerali")
                XML_DatiFormulati = XmlDoc.SelectSingleNode("DatiRisultati/DatiGenerali/DatiDiserbo/DatiFormulati")

                XML_DatiPrincipiAttivi = XmlDoc.SelectSingleNode("DatiRisultati/DatiGenerali/DatiDiserbo/DatiPrincipiAttivi")
                If Not IsNothing(XML_DatiPrincipiAttivi) Then
                    XMLs_PrincipiAttivo = XML_DatiPrincipiAttivi.GetElementsByTagName("PrincipioAttivo")
                End If


                If Not IsNothing(XML_DatiFormulati) Then

                    XMLs_Formulato = XML_DatiFormulati.GetElementsByTagName("Formulato")

                    Dim Fr_Cod As Integer = 0

                    i_Formulati = 0
                    Dose_Consigliata = 0
                    DoseMax = 0
                    PercAbb = 100

                    Do While i_Formulati < XMLs_Formulato.Count

                        XML_Formulato = XMLs_Formulato.Item(i_Formulati)

                        Fr_Cod = XML_Formulato.GetAttribute("fr_cod")

                        If Fr_Cod = FrCod Then

                            Udm_Cod = CInt(XML_Formulato.GetAttribute("udm_cod"))

                            'DOSE ETICHETTA
                            If XML_Formulato.GetAttribute("dose_etichetta") <> "" AndAlso IsNumeric(XML_Formulato.GetAttribute("dose_etichetta")) AndAlso XML_Formulato.GetAttribute("dose_etichetta") <> intDummy.ToString Then
                                DoseMax = CDec(XML_Formulato.GetAttribute("dose_etichetta"))
                            End If

                            'converto la dose max di etichetta in Kg o l, se necessario,
                            'x il confronto con la dose disciplinare espressa sempre in kg o l
                            Select Case Udm_Cod

                                Case 3, 20, 23, 37, 171, 174, 300, 301   'g
                                Case 3, 20, 23, 37, 171, 174, 300, 301   'g
                                    DoseMax = DoseMax / 1000
                                    Udm_Cod = 2

                                Case 21
                                    DoseMax = DoseMax / 100
                                    Udm_Cod = 29

                                Case 101, 104, 163, 164, 165, 170, 172   'ml
                                    DoseMax = DoseMax / 1000
                                    Udm_Cod = 29

                            End Select

                            'DOSE DISCIPLINARE
                            dose_ConsentitaDiserbo.percAbbDaApplicare = 0
                            dose_ConsentitaDiserbo.principiAttiviPercAbb = ""

                            ' 2022-03-16 Lettura nuova chiave da configurazione siti Flag_Nuovo_Controllo_Riduzione_Diserbo
                            ' per aggiunta tag a xml se usare vecchio o nuovo controllo
                            Dim flagNuovoControlloRiduzioneDiserbo As Boolean = Leggi_FlagNuovoControlloRiduzioneDiserbo(objParametri_Server)
                            If Not flagNuovoControlloRiduzioneDiserbo Then

                                ' Gestione GUI vecchio controllo
                                If XML_Formulato.GetAttribute("dose_consentita") <> "" AndAlso IsNumeric(XML_Formulato.GetAttribute("dose_consentita")) AndAlso XML_Formulato.GetAttribute("dose_consentita") <> intDummy.ToString Then

                                    If Dose_Consigliata = 0 Then
                                        Dose_Consigliata = CDec(XML_Formulato.GetAttribute("dose_consentita"))
                                        dose_ConsentitaDiserbo.Lbl_Dose_Consigliata = "<FONT color=red>" & XML_Formulato.GetAttribute("dose_consentita") & "</FONT>"
                                        dose_ConsentitaDiserbo.Udm_Radice_HA = Udm_Cod
                                        '(22/01/2020 fede) aggiungo il dettaglio dell sup già ridotta
                                        If Not XMLs_PrincipiAttivo Is Nothing AndAlso XMLs_PrincipiAttivo.Count > 0 Then
                                            For p = 0 To XMLs_PrincipiAttivo.Count - 1
                                                XML_PrincipiAttivo = XMLs_PrincipiAttivo.Item(p)
                                                If Fr_Cod = XML_PrincipiAttivo.GetAttribute("fr_cod") AndAlso XML_PrincipiAttivo.HasAttribute("dettaglio") AndAlso XML_PrincipiAttivo.GetAttribute("dettaglio") <> "" Then
                                                    dose_ConsentitaDiserbo.Lbl_Dose_Consigliata &= " " & XML_PrincipiAttivo.GetAttribute("dettaglio")
                                                End If
                                            Next
                                        End If
                                    Else
                                        If Dose_Consigliata > CDec(XML_Formulato.GetAttribute("dose_consentita")) Then
                                            Dose_Consigliata = CDec(XML_Formulato.GetAttribute("dose_consentita"))
                                            dose_ConsentitaDiserbo.Lbl_Dose_Consigliata = "<FONT color=red>" & XML_Formulato.GetAttribute("dose_consentita") & "</FONT>"
                                            dose_ConsentitaDiserbo.Udm_Radice_HA = Udm_Cod
                                            '(22/01/2020 fede) aggiungo il dettaglio dell sup già ridotta
                                            If Not XMLs_PrincipiAttivo Is Nothing AndAlso XMLs_PrincipiAttivo.Count > 0 Then
                                                For p = 0 To XMLs_PrincipiAttivo.Count - 1
                                                    XML_PrincipiAttivo = XMLs_PrincipiAttivo.Item(p)
                                                    If Fr_Cod = XML_PrincipiAttivo.GetAttribute("fr_cod") AndAlso XML_PrincipiAttivo.HasAttribute("dettaglio") AndAlso XML_PrincipiAttivo.GetAttribute("dettaglio") <> "" Then
                                                        dose_ConsentitaDiserbo.Lbl_Dose_Consigliata &= " " & XML_PrincipiAttivo.GetAttribute("dettaglio")
                                                    End If
                                                Next
                                            End If
                                        End If
                                    End If

                                End If

                            Else

                                ' Gestione GUI nuovo controllo

                                ' cerco principio attivo con perabb più restringente
                                Dim percAbbDaApplicare As Decimal = OttieniPercAbbDaApplicareSuCambioFormulato(XML_DatiPrincipiAttivi)
                                dose_ConsentitaDiserbo.percAbbDaApplicare = percAbbDaApplicare

                                Dim principiAttiviPercAbb As String = OttieniPrincipiAttiviPercAbbPerFormulatoSelezionato(FrCod, XML_DatiPrincipiAttivi)
                                dose_ConsentitaDiserbo.principiAttiviPercAbb = principiAttiviPercAbb

                                If XML_Formulato.GetAttribute("dose_consentita") <> "" AndAlso IsNumeric(XML_Formulato.GetAttribute("dose_consentita")) AndAlso XML_Formulato.GetAttribute("dose_consentita") <> intDummy.ToString Then

                                    If Dose_Consigliata = 0 Then
                                        Dose_Consigliata = CDec(XML_Formulato.GetAttribute("dose_consentita"))

                                        ' 2022-02-14  Se dose_consentita_anno = 0 significa che doseAssenteDPI = true (non visualizzo la dose consentita)
                                        Dose_Consentita_Anno = If(String.IsNullOrEmpty(XML_Formulato.GetAttribute("dose_consentita_anno")), 0, CDec(XML_Formulato.GetAttribute("dose_consentita_anno")))
                                        dose_ConsentitaDiserbo.Lbl_Dose_Consigliata = ""
                                        If Dose_Consentita_Anno > 0 Then
                                            dose_ConsentitaDiserbo.Lbl_Dose_Consigliata = "<FONT color=red>" & XML_Formulato.GetAttribute("dose_consentita") & "</FONT>"
                                        End If

                                        dose_ConsentitaDiserbo.Udm_Radice_HA = Udm_Cod
                                        '(22/01/2020 fede) aggiungo il dettaglio dell sup già ridotta
                                        If Not XMLs_PrincipiAttivo Is Nothing AndAlso XMLs_PrincipiAttivo.Count > 0 Then
                                            For p = 0 To XMLs_PrincipiAttivo.Count - 1
                                                XML_PrincipiAttivo = XMLs_PrincipiAttivo.Item(p)
                                                If Fr_Cod = XML_PrincipiAttivo.GetAttribute("fr_cod") AndAlso XML_PrincipiAttivo.HasAttribute("dettaglio") AndAlso XML_PrincipiAttivo.GetAttribute("dettaglio") <> "" Then
                                                    dose_ConsentitaDiserbo.Lbl_Dose_Consigliata2 = "<FONT color=red>" & " " & "<b>" & XML_PrincipiAttivo.GetAttribute("dettaglio") & "</b> </FONT>"
                                                End If
                                            Next
                                        End If
                                    Else
                                        If Dose_Consigliata > CDec(XML_Formulato.GetAttribute("dose_consentita")) Then
                                            Dose_Consigliata = CDec(XML_Formulato.GetAttribute("dose_consentita"))

                                            ' 2022-02-14  Se dose_consentita_anno = 0 significa che doseAssenteDPI = true (non visualizzo la dose consentita)
                                            Dose_Consentita_Anno = If(String.IsNullOrEmpty(XML_Formulato.GetAttribute("dose_consentita_anno")), 0, CDec(XML_Formulato.GetAttribute("dose_consentita_anno")))
                                            dose_ConsentitaDiserbo.Lbl_Dose_Consigliata = ""
                                            If Dose_Consentita_Anno > 0 Then
                                                dose_ConsentitaDiserbo.Lbl_Dose_Consigliata = "<FONT color=red>" & XML_Formulato.GetAttribute("dose_consentita") & "</FONT>"
                                            End If

                                            dose_ConsentitaDiserbo.Udm_Radice_HA = Udm_Cod
                                            '(22/01/2020 fede) aggiungo il dettaglio dell sup già ridotta
                                            If Not XMLs_PrincipiAttivo Is Nothing AndAlso XMLs_PrincipiAttivo.Count > 0 Then
                                                For p = 0 To XMLs_PrincipiAttivo.Count - 1
                                                    XML_PrincipiAttivo = XMLs_PrincipiAttivo.Item(p)
                                                    If Fr_Cod = XML_PrincipiAttivo.GetAttribute("fr_cod") AndAlso XML_PrincipiAttivo.HasAttribute("dettaglio") AndAlso XML_PrincipiAttivo.GetAttribute("dettaglio") <> "" Then
                                                        dose_ConsentitaDiserbo.Lbl_Dose_Consigliata2 = "<FONT color=red>" & " " & "<b>" & XML_PrincipiAttivo.GetAttribute("dettaglio") & "</b> </FONT>"
                                                    End If
                                                Next
                                            End If
                                        End If
                                    End If
                                End If

                            End If
                            dose_ConsentitaDiserbo.D_HA_Max_Diserbo = Dose_Consigliata
                            dose_ConsentitaDiserbo.lbl_qta_residua = My.Resources.AgronicaCoreMapper.DoseHaConsetitaDisciplinarekgl
                            dose_ConsentitaDiserbo.Div_DettaglioDoseConsentita = True

                        End If

                        i_Formulati = i_Formulati + 1
                    Loop

                End If

            End If

        End If

        Return dose_ConsentitaDiserbo

    End Function

    Public Shared Function BuildDisciplinare(infoOperazione As InfoOperazione, numProtocollo As Integer, docNumero As Integer, utilizzoTerreno As UtilizzoTerreno, lavCod As Integer, dataInizio As Date, verbose As Boolean, objParametri_Super_Server As AgronicaCoreParametri, objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri) As Disciplinare
        'DT: in GiasWeb il codice della combo è codiceRegolamento/tipoRegolamentoPUA
        'il codiceRegolamento è il numero di protocollo
        'il tipoRegolamentoPUA non è a DB, è funzione del codiceRegolamento

        Dim disciplinare As Disciplinare = Nothing

        If infoOperazione.IsTrattamento OrElse infoOperazione.IsFertilizzazione OrElse infoOperazione.IsRilievo Then

            Dim specie = Utility.GetSpecieFromUtilizzoTerreno(utilizzoTerreno)

            Dim disciplinare_cod As Integer = 0
            Dim regolamento_cod As Integer = 0
            Dim doc_numero As Integer = 0

            If infoOperazione.IsTrattamento Then
                Select Case numProtocollo
                    Case 0 'NessunDpiNessunaEtichetta
                        disciplinare_cod = -999
                    Case -1 'NessunDpi
                        disciplinare_cod = 0
                    Case -2   'BIO
                        disciplinare_cod = -2
                    Case Else
                        disciplinare_cod = numProtocollo
                End Select

                doc_numero = docNumero
            End If

            If infoOperazione.IsFertilizzazione Then
                Select Case numProtocollo
                    Case 0, -1 'NessunDpi (-1 arriva da APP)
                        regolamento_cod = 0
                    Case -2   'BIO
                        regolamento_cod = -2
                    Case Else
                        regolamento_cod = numProtocollo
                End Select
            End If

            If infoOperazione.IsRilievo Then
                If lavCod = LAVCOD_RILIEVO_AVVERSITA_CAMPO Then
                    Select Case numProtocollo
                        Case 0, -1 'NessunDpi (-1 arriva da APP)
                            disciplinare_cod = 0
                        Case -2   'BIO
                            disciplinare_cod = -2
                        Case Else
                            disciplinare_cod = numProtocollo
                    End Select
                    doc_numero = docNumero
                Else
                    disciplinare_cod = -1
                End If

            End If

            Dim stringList As String() = {lavCod}
            Dim lav_cod_list As Integer() = Array.ConvertAll(stringList, Function(str) Int32.Parse(str))

            Dim objDPI As New AgronicaControlli_2010.STD_Disciplinari
            Dim disciplinariList As List(Of Disciplinare) = Nothing

            Dim disciplinareFound = False

            'DT: per le distribuzioni ammendanti si tenta prima di decodificarle come direttiva nitrati, altrimenti come concimazioni
            If lavCod = LAVCOD_DISTRIBUZIONE_AMMENDANTI Then

                disciplinariList = objDPI.Leggi_Disciplinari_Testata_DirettivaNitrati(regolamento_cod,
                                                                        lav_cod_list,
                                                                        dataInizio,
                                                                        objParametri_Super_Server,
                                                                        objParametri_Server,
                                                                        objParametri_Utenti)

                For Each objDisciplinare In disciplinariList
                    If objDisciplinare.regolamentoConcimazione IsNot Nothing Then
                        If objDisciplinare.regolamentoConcimazione.codice = regolamento_cod Then
                            disciplinare = objDisciplinare
                            disciplinareFound = True
                        End If
                    End If
                Next

            End If

            If Not disciplinareFound Then

                disciplinariList = objDPI.Leggi_Disciplinari_Testata_conRegolamentoConcimazione(disciplinare_cod,
                                                                                                specie.codice,
                                                                                                regolamento_cod,
                                                                                                lav_cod_list,
                                                                                                dataInizio,
                                                                                                objParametri_Super_Server,
                                                                                                objParametri_Server,
                                                                                                objParametri_Utenti)
                If disciplinariList IsNot Nothing AndAlso disciplinariList.Count > 0 Then

                    For Each objDisciplinare In disciplinariList
                        If infoOperazione.IsTrattamento OrElse infoOperazione.IsRilievo Then
                            If objDisciplinare.codice = disciplinare_cod Then
                                If objDisciplinare.raggruppamentiColturaliDPI IsNot Nothing Then
                                    If objDisciplinare.raggruppamentiColturaliDPI.codice = doc_numero Then
                                        disciplinare = objDisciplinare
                                        disciplinareFound = True
                                    End If
                                Else
                                    disciplinare = objDisciplinare
                                    disciplinareFound = True
                                End If
                            End If
                        End If

                        If infoOperazione.IsFertilizzazione AndAlso objDisciplinare.regolamentoConcimazione IsNot Nothing Then
                            If objDisciplinare.regolamentoConcimazione.codice = regolamento_cod Then
                                disciplinare = objDisciplinare
                                disciplinareFound = True
                            End If
                        End If

                        If disciplinareFound Then
                            Exit For
                        End If

                    Next
                End If
            End If

            If Not disciplinareFound Then
                If infoOperazione.IsFertilizzazione Then

                    Dim objPermessi As New Utenti_Permessi_R
                    Dim permessoPianoNutrizionale = objPermessi.Controlla_Permessi_Utente(
                                                                        objParametri_Server.UtenteUsername, enum_Id_Servizio.GiasOnline,
                                                                        enum_Security_Attivita.Piano_Nutrizionale,
                                                                        enum_Security_Operazione.Lettura,
                                                                        Date.Now,
                                                                        "",
                                                                        objParametri_Utenti)
                    If permessoPianoNutrizionale Then
                        disciplinariList = objDPI.Leggi_Disciplinari_Testata_PianoNutrizionale(0,
                                                                                               specie.codice,
                                                                                               dataInizio,
                                                                                               objParametri_Super_Server,
                                                                                               objParametri_Server,
                                                                                               objParametri_Utenti)

                        If disciplinariList IsNot Nothing AndAlso disciplinariList.Count > 0 Then

                            For Each objDisciplinare In disciplinariList
                                If infoOperazione.IsFertilizzazione AndAlso objDisciplinare.regolamentoConcimazione IsNot Nothing Then
                                    If objDisciplinare.regolamentoConcimazione.codice = regolamento_cod Then
                                        disciplinare = objDisciplinare
                                        disciplinareFound = True
                                    End If
                                End If

                                If disciplinareFound Then
                                    Exit For
                                End If

                            Next
                        End If
                    End If
                End If
            End If

            If Not verbose Then
                'pulisco le descrizioni che verranno popolate solo in modalità verbose
                If disciplinare IsNot Nothing Then
                    disciplinare.descrizione = String.Empty

                    If disciplinare.gruppoFinalita IsNot Nothing Then
                        disciplinare.gruppoFinalita.descrizione = String.Empty
                    End If

                    If disciplinare.raggruppamentiColturaliDPI IsNot Nothing Then
                        disciplinare.raggruppamentiColturaliDPI.descrizione = String.Empty
                    End If

                    If disciplinare.regolamentoConcimazione IsNot Nothing Then
                        disciplinare.regolamentoConcimazione.descrizione = String.Empty
                    End If

                End If

            End If

        End If

        Return disciplinare

    End Function

    Public Shared Function BuildUtilizzoTerreno(Piva As String, Sa_Cod As Integer, Appezza As Integer, Id_Reg As Integer, verbose As Boolean, objParametri_Server As AgronicaCoreParametri) As UtilizzoTerreno

        Dim utilizzoTerreno As UtilizzoTerreno = Nothing

        If Piva <> "" AndAlso Sa_Cod > 0 AndAlso Appezza > 0 AndAlso Id_Reg > 0 Then

            Dim objAppezzamento As New AgronicaCoreAnagrafeBIZ.Appezzamento_R
            utilizzoTerreno = objAppezzamento.Leggi_Appezzamento_UtilizzoTerreno(Piva,
                                                                                        Sa_Cod,
                                                                                        Appezza,
                                                                                        Id_Reg,
                                                                                        objParametri_Server)

            If Not verbose Then
                Select Case utilizzoTerreno.classType
                    Case ClassType.Varieta
                        Dim varieta As utilizzi.Varieta = CType(utilizzoTerreno, utilizzi.Varieta)
                        varieta.descrizione = ""
                        If varieta.specie IsNot Nothing Then
                            varieta.specie.descrizione = ""
                        End If
                    Case ClassType.DestinazioneUso
                        Dim destinazioneUso As utilizzi.DestinazioneUso = CType(utilizzoTerreno, utilizzi.DestinazioneUso)
                        destinazioneUso.descrizione = ""
                End Select
            End If
        End If



        Return utilizzoTerreno
    End Function

    Public Shared Function BuildUtilizzoTerrenoFromProdottoDaTrattare(Piva As String, Elem_Cod As Integer, Mat_Cod As Integer, verbose As Boolean, objParametri_Server As AgronicaCoreParametri) As UtilizzoTerreno

        Dim veg_cod As Integer = 0
        Dim cul_cod As Integer = 0

        Dim veg_des As String = ""
        Dim cul_des As String = ""

        Dim objMateriePrime As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
        objMateriePrime.VegCod_CulCod_from_MatCod(Piva, Elem_Cod, Mat_Cod, veg_cod, cul_cod, objParametri_Server, verbose:=verbose, Veg_Des:=veg_des, Cul_Des:=cul_des)

        Dim utilizzoTerreno = Nothing

        If cul_cod <> 0 Then
            utilizzoTerreno = New AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta(cul_cod) With {
                .descrizione = cul_des,
                .specie = New AgronicaCoreModelsSTD.metaschema.utilizzi.Specie(veg_cod) With {
                    .descrizione = veg_des
                }
            }
        End If

        If Not verbose Then
            Select Case utilizzoTerreno.classType
                Case ClassType.Varieta
                    Dim varieta As utilizzi.Varieta = CType(utilizzoTerreno, utilizzi.Varieta)
                    varieta.descrizione = ""
                    If varieta.specie IsNot Nothing Then
                        varieta.specie.descrizione = ""
                    End If
                Case ClassType.DestinazioneUso
                    Dim destinazioneUso As utilizzi.DestinazioneUso = CType(utilizzoTerreno, utilizzi.DestinazioneUso)
                    destinazioneUso.descrizione = ""
            End Select
        End If

        Return utilizzoTerreno
    End Function

    Public Shared Function LoadAssociazionePK(attivita As Attivita, objParametri_Server As AgronicaCoreParametri) As AssociazionePK
        Dim associazionePK As AssociazionePK = Nothing

        Dim objRicettexAgenda As New AgronicaCoreContabDAL.RicettexAgenda_R
        Dim DtRicettexAgenda As DataTable = Nothing
        Select Case attivita.tipo
            Case Attivita.Tipo_Attivita.QuadernoDiCampagna
                DtRicettexAgenda = objRicettexAgenda.LeggiPerAssociazionePK(0,
                                                        0,
                                                        CInt(attivita.codice),
                                                        AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO,
                                                        AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAFINE,
                                                        objParametri_Server)

            Case Attivita.Tipo_Attivita.Ricetta
                DtRicettexAgenda = objRicettexAgenda.LeggiPerAssociazionePK(attivita.testataRicetta.Ricetta_Cod,
                                                           CInt(attivita.codice),
                                                           0,
                                                            AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO,
                                                            AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAFINE,
                                                            objParametri_Server)

        End Select

        If DtRicettexAgenda IsNot Nothing AndAlso DtRicettexAgenda.Rows.Count > 0 Then
            associazionePK = New AssociazionePK
            associazionePK.id_agenda = DtRicettexAgenda.Rows(0).Item("Id_Agenda")
            associazionePK.Ricetta_Cod = DtRicettexAgenda.Rows(0).Item("Ricetta_Cod")
            associazionePK.Ricetta_Operazione_Cod = DtRicettexAgenda.Rows(0).Item("Ricetta_Operazione_Cod")
        End If

        Return associazionePK
    End Function


    Public Shared Function BuildRisorsaUmana(codiceRisUm As Integer, verbose As Boolean, objParametri_Server As AgronicaCoreParametri) As AgronicaCoreModelsSTD.anagrafiche.RisorseUmane

        Dim risorse_umane_R As New AgronicaCoreAnagrafeBIZ.RisorseUmane_R

        Dim risorsaUmana As AgronicaCoreModelsSTD.anagrafiche.RisorseUmane = risorse_umane_R.Decodifica(Piva:="", codiceRisUm, objParametri_Server, verbose)

        If verbose Then

            If risorsaUmana IsNot Nothing AndAlso risorsaUmana.contatto IsNot Nothing AndAlso risorsaUmana.contatto.primaryKey IsNot Nothing Then

                Dim allegatiLeggiPerEntita As New AgronicaCoreAnagrafeDAL.Allegati_EntitaxDocumenti_R

                Dim xFiltroAggiuntivo = “ Alert_Elenco.ID_Tipologia = " & enum_ID_Area_Tipologia.Patentino_trattamenti & ""

                Dim dtAllegati = allegatiLeggiPerEntita.Leggi_Allegati_Entita(Piva:="", risorsaUmana.contatto.primaryKey.codice, 0, 0, 0, 0, 0, 0, xFiltroAggiuntivo, "", objParametri_Server)

                If dtAllegati IsNot Nothing AndAlso dtAllegati.Rows.Count > 0 Then
                    risorsaUmana.contatto.documenti = New List(Of Documento)

                    For Each drAllegato In dtAllegati.Rows

                        Dim documento As New Documento
                        documento.ID_Tipologia = enum_ID_Area_Tipologia.Patentino_trattamenti
                        documento.Numero = drAllegato("Allegati_Documenti_Numero")
                        documento.Data_Rilascio = drAllegato("Validazione_Data")
                        documento.Data_Scadenza = drAllegato("Data_Scadenza")
                        documento.Descrizione = drAllegato("Descrizione_Scadenza")
                        documento.Ente_Rilascio = New EnteRilascio()
                        documento.Ente_Rilascio.codice = drAllegato("Allegati_Documenti_Ente_Cod")
                        documento.Ente_Rilascio.descrizione = drAllegato("Allegati_Documenti_Ente_Des")

                        risorsaUmana.contatto.documenti.Add(documento)

                    Next
                End If
            End If

        End If

        Return risorsaUmana
    End Function

    Public Shared Sub SetDisciplinareMulti(ByRef attivitaList As List(Of AttivitaConParametriAggiuntivi))

        If attivitaList.Count > 1 Then

            Dim selectedDisciplinare As Disciplinare = Nothing

            For Each attivita In attivitaList

                Dim infoOperazione As InfoOperazione = GetInfoOperazione(attivita.job.primaryKey.codice, attivita.tipo)

                If infoOperazione.IsTrattamento Then
                    selectedDisciplinare = attivita.disciplinare
                    Exit For
                End If

            Next

            For Each attivita In attivitaList
                Dim infoOperazione As InfoOperazione = GetInfoOperazione(attivita.job.primaryKey.codice, attivita.tipo)

                Dim isPua As Boolean = False
                If attivita.disciplinare IsNot Nothing AndAlso attivita.disciplinare.regolamentoConcimazione IsNot Nothing AndAlso attivita.disciplinare.regolamentoConcimazione.tipo = enum_PUARegolamenti_Tipo.PUA Then
                    isPua = True
                End If

                If Not (infoOperazione.IsFertilizzazione AndAlso isPua) Then
                    If selectedDisciplinare IsNot Nothing Then
                        attivita.disciplinare = selectedDisciplinare
                    End If
                End If

            Next
        End If

    End Sub

    Public Function Default_DPI_QdC(ByVal InDataDefault_DPI As AgronicaCoreDTOStd.InData.Agenda.LeggiDefault_DPI_QdC,
                                    ByVal objParametri_Server As AgronicaCoreParametri,
                                    ByVal objParametri_Utenti As AgronicaCoreParametri) As Disciplinare

        Dim disciplinari As List(Of Disciplinare) = InDataDefault_DPI.disciplinari

        Dim Piva As String = InDataDefault_DPI.impresa.partitaIva

        Dim disciplinare As Disciplinare = Nothing

        If InDataDefault_DPI.tipoOperazioneDB = enum_TipoOperazioneDB.Scrittura Then

            If Not (InDataDefault_DPI.tipoRicetta = enum_TipoRicetta.PianoDistribuzionePua AndAlso InDataDefault_DPI.operazioni.Exists(Function(o) {LAVCOD_DISTRIBUZIONE_AMMENDANTI}.Contains(o.primaryKey.codice))) AndAlso
                    Not (InDataDefault_DPI.tipoAttivita = enum_Tipo_Operazione_Agenda.QuadernoDiCampagna AndAlso InDataDefault_DPI.codiciAttivita.Exists(Function(c) c.CodiceRicetta <> "0" AndAlso c.CodiceRicetta <> "")) Then

                Dim ic_r As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
                Dim val As String = ic_r.Leggi_Codice_from_Imprese_Codici(Piva, enum_CodiciAnagrafe.Disciplinare_Aziendale_Default, objParametri_Server)

                If val <> "" Then
                    disciplinare = impostaDisciplinareDaPreferenza(val, InDataDefault_DPI.data, disciplinari, InDataDefault_DPI.impianti,
                                                                 objParametri_Server, objParametri_Utenti)
                Else
                    'leggo l'impostazione utente UTENTE_COD_DEFAULT_DPI_PREDEFINITO perchè devo impostare questa oppure quella dell'azienda
                    Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
                    Dim Dt_Impostazioni As New DataTable
                    If InDataDefault_DPI.tipoOperazioneDB = enum_TipoOperazioneDB.Scrittura OrElse
                       InDataDefault_DPI.tipoOperazioneDB = enum_TipoOperazioneDB.Modifica Then
                        Dt_Impostazioni = ObjUtenti.Leggi(0,
                                                          1,
                                                          AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                          "",
                                                          "",
                                                          objParametri_Utenti)

                    End If

                    If Dt_Impostazioni IsNot Nothing AndAlso Dt_Impostazioni.Rows.Count > 0 Then

                        Dim Dr_Utente_Cod_Default_DPI_Predefinito As DataRow() = Dt_Impostazioni.Select("Impostazione_Cod = '" & enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_DPI_PREDEFINITO & "'")

                        If Not IsNothing(Dr_Utente_Cod_Default_DPI_Predefinito) AndAlso Dr_Utente_Cod_Default_DPI_Predefinito.Length = 1 Then

                            If Not (InDataDefault_DPI.tipoRicetta = enum_TipoRicetta.PianoDistribuzionePua AndAlso InDataDefault_DPI.operazioni.Exists(Function(o) {LAVCOD_DISTRIBUZIONE_AMMENDANTI}.Contains(o.primaryKey.codice))) AndAlso
                                Not (InDataDefault_DPI.tipoAttivita = enum_Tipo_Operazione_Agenda.QuadernoDiCampagna AndAlso InDataDefault_DPI.codiciAttivita.Exists(Function(c) c.CodiceRicetta <> "0" AndAlso c.CodiceRicetta <> "")) Then

                                Dim preferenza As String = Dr_Utente_Cod_Default_DPI_Predefinito(0).Item("Impostazione_Valore_1")

                                If preferenza <> "" Then
                                    disciplinare = impostaDisciplinareDaPreferenza(preferenza, InDataDefault_DPI.data, disciplinari, InDataDefault_DPI.impianti,
                                                objParametri_Server, objParametri_Utenti)
                                End If
                            End If


                        End If

                    End If
                End If

            End If

        End If

        If IsNothing(disciplinare) AndAlso disciplinari.Count > 0 Then
            'default lo imposto come Nessun Disciplinare
            disciplinare = disciplinari.Find(Function(d) d.codice = 0)
        End If

        Return disciplinare

    End Function

    Private Function impostaDisciplinareDaPreferenza(ByVal preferenza As String,
                                                    ByVal Data As DateTime,
                                                    ByVal disciplinari As List(Of Disciplinare),
                                                    ByVal impianti As List(Of Impianto),
                                                    ByVal objParametri_Server As AgronicaCoreParametri,
                                                    ByVal objParametri_Utenti As AgronicaCoreParametri) As Disciplinare


        Dim xletturautente As New AgronicaCoreUtentiDAL.Utenti_Read
        Dim pass As String = xletturautente.Password_From_UserName(objParametri_Utenti.SuperUserUsername, objParametri_Utenti)

        Dim ASG_SuperUser_CodFiscale As String = objParametri_Server.PivaSuperUser

        Dim ASG_Utente_Username_Crypt As String = Sicurezza.Stringa_Codifica_LANCompatibile(objParametri_Utenti.SuperUserUsername, CostantiPersonalizzate.AgroKey_EncoderDecoder)

        Dim ASG_Utente_Password_Crypt As String = Sicurezza.Stringa_Codifica_LANCompatibile(pass, CostantiPersonalizzate.AgroKey_EncoderDecoder)

        Dim default_disciplinare As Disciplinare = Nothing

        Dim default_disciplinari As New List(Of Disciplinare)

        Select Case preferenza
            Case "0"
                'default lo imposto come Nessun Disciplinare
                default_disciplinari.Add(disciplinari.Find(Function(d) d.codice = 0))
            Case Else

                Dim x As New AgronicaCoreDpiBIZ.CaricaListControl

                Dim disciplinare As String = preferenza

                If disciplinare.Contains("e:") Then
                    'Caso nuovo salvo idEnte
                    objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(Data, Data)

                    Dim disciplinare_cod As String = ""
                    Dim fp As String = ""

                    x.Trova_Disciplinare_Ente(disciplinare.Split("/")(0).Split(":")(1),
                                                            disciplinare.Split("/")(1).Split(":")(1),
                                                            False, "", "", Nothing, objParametri_Server, objParametri_Utenti,
                                                            0, 0, 0, 0, True, True, False,
                                                New AgronicaCoreGestioneRichieste.AgroWebConfig() With {.Flag_DisciplinarePrivato = True, .Flag_DisciplinareAttivo = True}, disciplinare_cod, fp,
                                                       ASG_SuperUser_CodFiscale, ASG_Utente_Username_Crypt, ASG_Utente_Password_Crypt)

                    objParametri_Server.ResettaFinestra()

                    disciplinare = disciplinare_cod & "/" & fp

                End If


                If disciplinari.Count > 0 Then

                    For kk = 0 To disciplinari.Count - 1
                        If disciplinari(kk).codice = disciplinare.Split("/")(0) Then

                            If disciplinare.Split("/").Length = 2 Then
                                If disciplinari(kk).disciplinarePubblicoPrivato = disciplinare.Split("/")(1) Then
                                    default_disciplinari.Add(disciplinari(kk))
                                End If
                            Else
                                default_disciplinari.Add(disciplinari(kk))
                            End If
                        End If
                    Next

                    If Not default_disciplinari.Count > 0 AndAlso disciplinare.Split("/").Length > 1 Then

                        Dim ente_cod As String = ""
                        Dim fp As String = ""

                        x.Trova_Ente_Disciplinare(disciplinare, False, "", "",
                                                                   Nothing, objParametri_Server, objParametri_Utenti,
                                                                   0, 0, 0, 0, True, True, False,
                                                                   New AgronicaCoreGestioneRichieste.AgroWebConfig() With {.Flag_DisciplinarePrivato = True, .Flag_DisciplinareAttivo = True},
                                                                   ente_cod, fp,
                                                                   ASG_SuperUser_CodFiscale, ASG_Utente_Username_Crypt, ASG_Utente_Password_Crypt)

                        If ente_cod <> "" Then

                            objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(Data, Data)

                            Dim disciplinare_cod As String = ""

                            x.Trova_Disciplinare_Ente(ente_cod, fp, False, "", "",
                                                        Nothing, objParametri_Server, objParametri_Utenti,
                                                        0, 0, 0, 0, True, True, False,
                                                       New AgronicaCoreGestioneRichieste.AgroWebConfig() With {.Flag_DisciplinarePrivato = True, .Flag_DisciplinareAttivo = True}, disciplinare_cod, fp,
                                                        ASG_SuperUser_CodFiscale, ASG_Utente_Username_Crypt, ASG_Utente_Password_Crypt)

                            objParametri_Server.ResettaFinestra()

                            disciplinare = disciplinare_cod & "/" & fp

                            For kk = 0 To disciplinari.Count - 1
                                If disciplinari(kk).codice = disciplinare.Split("/")(0) Then

                                    If disciplinare.Split("/").Length = 2 Then
                                        If disciplinari(kk).disciplinarePubblicoPrivato = disciplinare.Split("/")(1) Then
                                            default_disciplinari.Add(disciplinari(kk))
                                        End If
                                    Else
                                        default_disciplinari.Add(disciplinari(kk))
                                    End If
                                End If
                            Next

                        End If

                    End If

                End If
        End Select

        'Cerco tra i disciplinari trovati quello che è compatibile con gli impianti selezionati
        If Not IsNothing(default_disciplinari) AndAlso default_disciplinari.Count > 0 Then

            If default_disciplinari.Count = 1 Then
                default_disciplinare = default_disciplinari(0)
            Else
                If impianti.Count > 0 AndAlso default_disciplinari.Count > 1 Then

                    default_disciplinare = default_disciplinari.Find(Function(dpi)

                                                                         Return impianti.FindAll(Function(imp)

                                                                                                     Dim Copertura_Impianto As Integer = CInt(STD_Utility.IdentificaCopertura({imp}, objParametri_Server))

                                                                                                     Return dpi.gruppoFinalita.codice = imp.gruppoFinalita.codice AndAlso
                                                                                                            Copertura_Impianto = dpi.flagProtetto

                                                                                                 End Function).Count = impianti.Count
                                                                     End Function)

                    If IsNothing(default_disciplinare) Then
                        default_disciplinare = default_disciplinari.Find(Function(dpi)

                                                                             Return impianti.FindAll(Function(imp)

                                                                                                         Dim Copertura_Impianto As Integer = CInt(STD_Utility.IdentificaCopertura({imp}, objParametri_Server))

                                                                                                         Return dpi.gruppoFinalita.codice = 0 AndAlso
                                                                                                                Copertura_Impianto = dpi.flagProtetto

                                                                                                     End Function).Count = impianti.Count
                                                                         End Function)

                        If IsNothing(default_disciplinare) Then
                            default_disciplinare = default_disciplinari.Find(Function(dpi)

                                                                                 Return impianti.FindAll(Function(imp)

                                                                                                             Dim trovato As Boolean = False

                                                                                                             Dim Copertura_Impianto As Integer = CInt(STD_Utility.IdentificaCopertura({imp}, objParametri_Server))

                                                                                                             If dpi.gruppoFinalita.codice = imp.gruppoFinalita.codice OrElse dpi.gruppoFinalita.codice = 0 Then

                                                                                                                 If (Copertura_Impianto = 0 AndAlso dpi.flagProtetto = 0) OrElse (Copertura_Impianto = 0 AndAlso dpi.flagProtetto = -1) OrElse (Copertura_Impianto = 0 AndAlso dpi.flagProtetto = 2) Then
                                                                                                                     trovato = True
                                                                                                                 ElseIf (Copertura_Impianto = 1 AndAlso dpi.flagProtetto = 1) OrElse (Copertura_Impianto = 1 AndAlso dpi.flagProtetto = 0) Then
                                                                                                                     trovato = True
                                                                                                                 End If
                                                                                                             End If

                                                                                                             Return trovato
                                                                                                         End Function).Count = impianti.Count
                                                                             End Function)
                        End If

                    End If
                End If
            End If


            'Se tra i disciplinari trovati non ce ne è nessuno compatibile con gli impianti selezionati allora prendo l'ultimo disciplinare trovato
            If IsNothing(default_disciplinare) Then
                default_disciplinare = default_disciplinari(default_disciplinari.Count - 1)
            End If

        End If

        Return default_disciplinare

    End Function

    Public Shared Function checkDosiEtichettaUguali(dosi1 As List(Of DoseEtichetta), dosi2 As List(Of DoseEtichetta)) As Boolean

        Dim dosiUguali As Boolean = True

        Dim countDosi1 As Integer = If(dosi1 IsNot Nothing, dosi1.Count, 0)
        Dim countDosi2 As Integer = If(dosi2 IsNot Nothing, dosi2.Count, 0)

        If countDosi1 + countDosi2 > 0 Then

            If countDosi1 = countDosi2 Then

                For Each doseInserita1 In dosi1
                    Dim found = False
                    For Each doseInserita2 In dosi2
                        If doseInserita1.codice = doseInserita2.codice Then
                            found = True
                            Exit For
                        End If
                    Next
                    If Not found Then
                        dosiUguali = False
                        Exit For
                    End If
                Next

            Else
                dosiUguali = False
            End If
        End If

        Return dosiUguali

    End Function

    Public Shared Function checkAcqua(risorsaAcqua As risorse.RisorsaAcqua, lavorazione As AgronicaCoreModelsSTD.attivita.Lavorazione, risorse As List(Of risorse.Risorsa), ByRef lista_Errori As List(Of ErroreGias)) As Boolean

        Dim messaggio As String = ""

        Dim Lav_Cod = lavorazione.primaryKey.codice
        Dim InfoOperazione As InfoOperazione = GetInfoOperazione(Lav_Cod, tipoAttivita:=1) 'RR: Ho fissato tipoAttivita a QDC perchè non si controlla serve sapere solo operazione("Trattamento..")

        If Lav_Cod = LAVCOD_CONCIMAZIONE_FOGLIARE OrElse Lav_Cod = LAVCOD_FERTIRRIGAZIONE OrElse Lav_Cod = LAVCOD_TRATTAMENTO_ANTIBUTTERATURA Then

            If risorsaAcqua.acqua <= 0 Then
                messaggio = My.Resources.AgronicaCoreMapper.AcquaObbligatoria
                lista_Errori.Add(generaErroreGias(ErroreGias_Severity.Bloccante, "", messaggio, Key_Parametri_Aggiuntivi_Attivita.mostraWarning_CheckListaAttivita))
                Return False
            End If

        Else

            If InfoOperazione.IsTrattamento AndAlso InfoOperazione.controlloPolverulenti Then

                Dim hasPolverulenti As Boolean = False
                Dim hasNoNPolverulenti As Boolean = False

                Dim risorseProdotto As List(Of dettagli.DettaglioTrattamento) = risorse.FindAll(Function(c) (c.classType = costanti.ClassType.DettaglioTrattamento)).ConvertAll(Function(obj1) CType(obj1, dettagli.DettaglioTrattamento))

                For Each risorsaProdotto In risorseProdotto
                    If risorsaProdotto.polverulento = Tipo_Polverulento.NonPolverulento Then
                        hasNoNPolverulenti = True
                    Else
                        hasPolverulenti = True
                    End If
                Next

                'Tutti non polverulenti --> acqua obbligatoria
                If hasNoNPolverulenti = True AndAlso hasPolverulenti = False Then
                    If risorsaAcqua.acqua <= 0 Then
                        messaggio = My.Resources.AgronicaCoreMapper.AcquaObbligatoria
                        lista_Errori.Add(generaErroreGias(ErroreGias_Severity.Bloccante, "", messaggio, Key_Parametri_Aggiuntivi_Attivita.mostraWarning_CheckListaAttivita))
                        Return False
                    End If
                End If

                'Tutti polverulenti --> acqua obbligatoriamente 0
                If hasNoNPolverulenti = False AndAlso hasPolverulenti = True Then
                    If risorsaAcqua.acqua > 0 Then
                        messaggio = My.Resources.AgronicaCoreMapper.ImpossibileAcquaPolverulenti
                        lista_Errori.Add(generaErroreGias(ErroreGias_Severity.Bloccante, "", messaggio, Key_Parametri_Aggiuntivi_Attivita.mostraWarning_CheckListaAttivita))
                        Return False
                    End If
                End If

            End If

        End If

        Return True

    End Function

    Public Shared Function BuildMagazzinoEsternoKey(fabbricato As Fabbricato, magazzino_esterno_tipo As enum_MagazzinoEsterno_Tipo) As String
        Dim magazzinoEsternoKey = ""

        If fabbricato IsNot Nothing AndAlso fabbricato.primaryKey IsNot Nothing AndAlso fabbricato.primaryKey.centroAziendalePK IsNot Nothing Then
            magazzinoEsternoKey = fabbricato.primaryKey.centroAziendalePK.partitaIva & "-" & fabbricato.primaryKey.centroAziendalePK.codice & "-" & fabbricato.primaryKey.codice & "-" & magazzino_esterno_tipo
        End If

        Return magazzinoEsternoKey
    End Function

    Public Shared Function GetAgendeMulticentroFromParametri(parametri_aggiuntivi_list As List(Of Parametri_Aggiuntivi_ControllaDosi)) As List(Of Integer)
        Dim codiciAttivita_x_CentriAziendali_List As New List(Of Integer)

        If parametri_aggiuntivi_list IsNot Nothing Then
            For Each parametroAggiuntivo In parametri_aggiuntivi_list
                If parametroAggiuntivo.key = Key_Parametri_Aggiuntivi_Attivita.lista_Codici_Attivita_x_CentriAziendali Then
                    Dim codiciAttivita_x_CentriAziendali_array = JsonConvert.DeserializeObject(Of List(Of CodiciAttivita_x_CentriAziendali))(parametroAggiuntivo.value)
                    For Each item In codiciAttivita_x_CentriAziendali_array
                        codiciAttivita_x_CentriAziendali_List.Add(item.ID_Agenda)
                    Next

                End If
            Next
        End If

        Return codiciAttivita_x_CentriAziendali_List

    End Function


    Public Shared Function GetAttivita_for_Redirect_To_NG(ByVal objParametriAgendaNG As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG, ByVal verbose As Boolean,
                                                   ByVal objParametri_Server As AgronicaCoreParametri, ByVal objParametri_Utenti As AgronicaCoreParametri, ByVal objParametri_Super_Server As AgronicaCoreParametri) As AgronicaCoreModelsSTD.attivita.Attivita

        Dim attivita As New AgronicaCoreModelsSTD.attivita.Attivita

        Dim ObjAgendaToAttivita As New AgendaToAttivita

        ObjAgendaToAttivita.CreaJob(attivita, objParametriAgendaNG.Lav_Cod, objParametri_Server)

        With attivita
            .tipo = objParametriAgendaNG.TipoOperazioneAgenda
            .codice = objParametriAgendaNG.Id_Agenda
            .inizio = objParametriAgendaNG.Data
            .stato = objParametriAgendaNG.Stato
            .tipoRicetta = objParametriAgendaNG.TipoRicetta
            .oraInizio = New Date(objParametriAgendaNG.Data.Year, objParametriAgendaNG.Data.Month, objParametriAgendaNG.Data.Day,
                                 0, 0, 0)
        End With

        If attivita.tipo = Attivita.Tipo_Attivita.Ricetta Then
            If Not String.IsNullOrEmpty(objParametriAgendaNG.Piva) AndAlso objParametriAgendaNG.Ricetta_Cod > 0 Then

                Dim objRicette As New AgronicaCoreContabDAL.Ricette_R

                Dim DT As DataTable = objRicette.Leggi(objParametriAgendaNG.Ricetta_Cod, objParametriAgendaNG.Piva, 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE,
                                                        enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
                If Not IsNothing(DT) AndAlso DT.Rows.Count = 1 Then
                    Dim ObjRicettaToAttivita As New RicettaToAttivita

                    attivita.testataRicetta = ObjRicettaToAttivita.GetTestataRicetta(attivita, CInt(DT(0)("Ricetta_Cod")), DT(0)("Ricetta_Des").ToString(), DT(0)("Ricetta_Des_Long").ToString(),
                                                     DT(0)("Ricetta_Numero").ToString(), DT(0)("Note").ToString(), CDate(DT(0)("Validita_Inizio")),
                                                     CDate(DT(0)("Validita_Fine")), objParametriAgendaNG.Lav_Cod, CInt(DT(0)("Programmazione_Cod")), objParametriAgendaNG.Piva, True, objParametri_Server,
                                                        objParametri_Utenti, objParametri_Super_Server)
                End If

            End If

        End If

        Dim InfoOperazione As InfoOperazione = GetInfoOperazione(attivita.job.primaryKey.codice, attivita.tipo)

        attivita.centroAziendale = ObjAgendaToAttivita.CreaCentroAziendale(objParametriAgendaNG.Piva, objParametriAgendaNG.Sa_Cod)

        If Not IsNothing(objParametriAgendaNG.Impianti) AndAlso objParametriAgendaNG.Impianti.Count > 0 Then

            attivita.utilizzoTerreno = BuildUtilizzoTerreno(objParametriAgendaNG.Impianti(0).Piva, objParametriAgendaNG.Impianti(0).Sa_Cod, objParametriAgendaNG.Impianti(0).Appezza, objParametriAgendaNG.Impianti(0).Id_Reg, verbose, objParametri_Server)

            Dim MovimentoOperazione As New AgronicaCoreModello.OperazioneAgenda_Temp.Movimento With {
                .Movimenti_Dettagli = New List(Of OperazioneAgenda_Temp.Movimento_Dettaglio)
            }


            MovimentoOperazione.Movimenti_Dettagli.Add(New OperazioneAgenda_Temp.Movimento_Dettaglio With {
                .Movimenti_Destinazioni = New List(Of OperazioneAgenda_Temp.Movimento_Destinazione)
            })

            For Each obj In objParametriAgendaNG.Impianti

                Dim MovimentoDestinazione As New AgronicaCoreModello.OperazioneAgenda_Temp.Movimento_Destinazione With {
                    .Appezza = obj.Appezza,
                    .Qta2 = obj.Sup_Imp_help,
                    .Piva = obj.Piva,
                    .Sa_Cod = obj.Sa_Cod,
                    .Id_Destinazione = obj.Id_Reg,
                    .Progetto_Cod = obj.Progetto_Cod,
                    .Sup_Riduzione_BufferZone = obj.Sup_Riduzione_BufferZone,
                    .Perc_Riduzione_Deriva = obj.Perc_Riduzione_Deriva
                }

                MovimentoOperazione.Movimenti_Dettagli(0).Movimenti_Destinazioni.Add(MovimentoDestinazione)

            Next

            ObjAgendaToAttivita.CreaEserciziCdC(attivita, InfoOperazione, MovimentoOperazione)

        End If

        attivita.disciplinare = BuildDisciplinare(InfoOperazione, objParametriAgendaNG.Regolamento_Cod, 0, attivita.utilizzoTerreno, attivita.job.primaryKey.codice, attivita.inizio, verbose, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)

        If verbose Then
            CompleteVerbose(attivita, isRibaltamentoToAgenda:=False, listParametriAggiuntivi:=Nothing, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)
        End If

        Return attivita
    End Function

    Public Shared Function Recupera_Pua_Valido_Alla_Data(ByVal Piva As String, ByVal Data As DateTime,
                                                 ByVal Lav_Cod As Integer, ByVal tipo_attivita As Attivita.Tipo_Attivita, ByVal tipo_ricetta As Attivita.Tipo_Ricetta,
                                                 ByVal stato As Attivita.Stati, ByVal disciplinare As Disciplinare,
                                                 ByVal objParametri_Server As AgronicaCoreParametri) As Pua
        Dim pua As Pua = Nothing

        'Cerco se ci sono dei PUA per l'operazione di distribuzione ammendanti
        '(se sono in una operazione di agenda oppure in una ricetta non di tipo PUA perchè in nel caso del tipo PUA mi arriva già valorizzato e non ho bisogno di leggerlo )

        If Lav_Cod = LAVCOD_DISTRIBUZIONE_AMMENDANTI AndAlso
           Not IsNothing(disciplinare) AndAlso Not IsNothing(disciplinare.regolamentoConcimazione) AndAlso disciplinare.regolamentoConcimazione.tipo = enum_PUARegolamenti_Tipo.PUA AndAlso
           (tipo_attivita = Attivita.Tipo_Attivita.QuadernoDiCampagna OrElse
           (tipo_attivita = Attivita.Tipo_Attivita.Ricetta AndAlso tipo_ricetta <> Attivita.Tipo_Ricetta.PianoDistribuzionePua AndAlso stato = Attivita.Stati.Da_Eseguire)) Then

            Dim Regolamento_Cod As Integer = disciplinare.regolamentoConcimazione.codice

            'Recupero il PUA
            Dim objPUA_Testata_Read As New AgronicaCorePUA_DAL.PUA_Testata_R

            Dim xOrderBy As String = "Data_Creazione DESC"

            Dim dt As DataTable = objPUA_Testata_Read.Leggi(Regolamento_Cod, 0, Piva, AGRODATAINIZIO, AGRODATAFINE, "", xOrderBy, objParametri_Server)

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then


                Dim dr As DataRow() = dt.Select("Validita_Inizio <= '" & Data.ToShortDateString() & "' And Validita_Fine >= '" & Data.ToShortDateString() & "'")

                'Se ci sono più PUA prendo quello più recente
                If Not IsNothing(dr) AndAlso dr.Length > 0 Then

                    Dim dtRicFiltrate As DataTable = dr.CopyToDataTable()

                    pua = New Pua(dtRicFiltrate.Rows(0).Item("Pua_Cod"))

                End If
            End If

            'Imposto come disciplinare nel PUA quello trovato
            If Not IsNothing(pua) AndAlso pua.codice > 0 Then
                pua.disciplinare = disciplinare
            End If

        End If

        Return pua
    End Function

    Public Shared Function Controlla_Se_Stesso_Prodotto(ByVal Elem_Cod_1 As Integer, ByVal Pro_Cod_1 As Integer, ByVal Mat_Cod_1 As Integer, ByVal Lotto_1 As String, ByVal Sa_Cod_1 As Integer,
                                                       ByVal Elem_Cod_2 As Integer, ByVal Pro_Cod_2 As Integer, ByVal Mat_Cod_2 As Integer, ByVal Lotto_2 As String, ByVal Sa_Cod_2 As Integer) As Boolean

        Dim Stesso_Prodotto As Boolean = False

        Dim Elem_Cod As Integer = IIf(Elem_Cod_1 = Elem_Cod_2, Elem_Cod_1, 0)

        Select Case Elem_Cod
            Case SEMENTI, TRASFORMATI_VEGETALI
                If Mat_Cod_1 = Mat_Cod_2 AndAlso
                  Lotto_1.ToUpper() = Lotto_2.ToUpper() AndAlso
                  Sa_Cod_1 = Sa_Cod_2 Then

                    Stesso_Prodotto = True

                End If
            Case Else
                If Pro_Cod_1 = Pro_Cod_2 Then
                    Stesso_Prodotto = True
                End If
        End Select

        Return Stesso_Prodotto

    End Function

    Public Shared Function Carico_Scarico_Collegate_Al_QdC(ByVal Mov_Dettaglio_Riferimento As OperazioneAgenda_Temp.Movimento_Dettaglio_Riferimento) As Boolean

        Dim Carico_Scarico As Boolean = False

        If Not IsNothing(Mov_Dettaglio_Riferimento) AndAlso
           ((Mov_Dettaglio_Riferimento.Lav_Cod_Rif = LAVCOD_SCARICO OrElse Mov_Dettaglio_Riferimento.Lav_Cod_Rif = LAVCOD_BOLLA_EMESSA) AndAlso Mov_Dettaglio_Riferimento.Cau_Mov_Rif = CAU_SCARICO) OrElse
            ((Mov_Dettaglio_Riferimento.Lav_Cod_Rif = LAVCOD_CARICO OrElse Mov_Dettaglio_Riferimento.Lav_Cod_Rif = LAVCOD_BOLLA_RICEVUTA) AndAlso Mov_Dettaglio_Riferimento.Cau_Mov_Rif = CAU_CARICO) Then

            Carico_Scarico = True

        End If

        Return Carico_Scarico

    End Function

    Public Shared Sub Controllo_Compatibilita_Impostazioni_Tra_Azienda_QdC_Esterna(ByVal Visualizza_Magazzini_Esterni As Boolean, ByVal Rag_Soc As String, ByVal Categoria_Magazzino As Integer, Magazzino As Fabbricato, ByVal Magazzino_Esterno As Fabbricato,
                                                                                    ByVal Lotto As String, ByVal objParametri_Server As AgronicaCoreParametri, ByVal objParametri_Utenti As AgronicaCoreParametri, ByRef ListaErrori As List(Of ErroreGias))

        If Not IsNothing(Magazzino_Esterno) Then

            Dim objImpreseImpostazioni As New AgronicaCoreAnagrafeDAL.Imprese_Impostazioni_R

            Dim Magazzino_Esterno_Des As String = Magazzino_Esterno.descrizione

            Dim Piva_Magazzino_Esterno As String = Magazzino_Esterno.primaryKey.centroAziendalePK.partitaIva

            Dim Piva As String = Magazzino.primaryKey.centroAziendalePK.partitaIva

            Dim gestioneMagazzino_Azienda_Esterna = CInt(objImpreseImpostazioni.LeggiScalareMulticentroAziendaSuperUser_ElemCod(Piva_Magazzino_Esterno, Nothing, enum_Impostazioni_Utenti.SUPERUSER_GESTIONE_MAGAZZINO_ABILITATA, Categoria_Magazzino, valoreDefault:=1, objParametri_Utenti, objParametri_Server))
            Dim gestioneGiacenze_Azienda_Esterna = CInt(objImpreseImpostazioni.LeggiScalareMulticentroAziendaSuperUser_ElemCod(Piva_Magazzino_Esterno, Nothing, enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_GIACENZE, Categoria_Magazzino, enum_Gestione_Giacenze.TuttiProdotti, objParametri_Utenti, objParametri_Server))
            Dim gestioneLotto_Azienda_Esterna = CInt(objImpreseImpostazioni.LeggiScalareMulticentroAziendaSuperUser_ElemCod(Piva_Magazzino_Esterno, Nothing, enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_LOTTI, Categoria_Magazzino, enum_Gestione_Lotti.Obbligatoria, objParametri_Utenti, objParametri_Server))

            Dim ImpostazioneLottiEsterna_Descrizione As String = ""

            Select Case gestioneLotto_Azienda_Esterna
                Case enum_Gestione_Lotti.Nessuna
                    ImpostazioneLottiEsterna_Descrizione = Gias.Nessuna
                Case enum_Gestione_Lotti.Obbligatoria
                    ImpostazioneLottiEsterna_Descrizione = Gias.Obbligatoria
                Case enum_Gestione_Lotti.Facoltativa
                    ImpostazioneLottiEsterna_Descrizione = Gias.Facoltativa
            End Select

            Dim gestioneMagazzino_Azienda = CInt(objImpreseImpostazioni.LeggiScalareMulticentroAziendaSuperUser_ElemCod(Piva, Nothing, enum_Impostazioni_Utenti.SUPERUSER_GESTIONE_MAGAZZINO_ABILITATA, Categoria_Magazzino, valoreDefault:=1, objParametri_Utenti, objParametri_Server))
            Dim gestioneGiacenze_Azienda = CInt(objImpreseImpostazioni.LeggiScalareMulticentroAziendaSuperUser_ElemCod(Piva, Nothing, enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_GIACENZE, Categoria_Magazzino, enum_Gestione_Giacenze.TuttiProdotti, objParametri_Utenti, objParametri_Server))
            Dim gestioneLotto_Azienda = CInt(objImpreseImpostazioni.LeggiScalareMulticentroAziendaSuperUser_ElemCod(Piva, Nothing, enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_LOTTI, Categoria_Magazzino, enum_Gestione_Lotti.Obbligatoria, objParametri_Utenti, objParametri_Server))

            Dim ImpostazioneLottiQdC_Descrizione As String = ""

            Select Case gestioneLotto_Azienda
                Case enum_Gestione_Lotti.Nessuna
                    ImpostazioneLottiQdC_Descrizione = Gias.Nessuna
                Case enum_Gestione_Lotti.Obbligatoria
                    ImpostazioneLottiQdC_Descrizione = Gias.Obbligatoria
                Case enum_Gestione_Lotti.Facoltativa
                    ImpostazioneLottiQdC_Descrizione = Gias.Facoltativa
            End Select

            'Controllo Impostazioni in comune tra Azienda Terzista e Azienda QdC
            If gestioneMagazzino_Azienda = 1 AndAlso gestioneMagazzino_Azienda_Esterna = 1 Then

                If gestioneLotto_Azienda = enum_Gestione_Lotti.Obbligatoria AndAlso gestioneLotto_Azienda_Esterna = enum_Gestione_Lotti.Obbligatoria AndAlso String.IsNullOrWhiteSpace(Lotto) Then
                    ListaErrori.Add(generaErroreGias(ErroreGias_Severity.WarningBloccante, "", String.Format(My.Resources.AgronicaCoreMapper.ImpostazioneLottiObbligatoriaPerAziendaQdCEsterna, Rag_Soc, Magazzino_Esterno_Des), ""))
                Else
                    If gestioneLotto_Azienda_Esterna = enum_Gestione_Lotti.Obbligatoria AndAlso String.IsNullOrWhiteSpace(Lotto) Then
                        ListaErrori.Add(generaErroreGias(ErroreGias_Severity.WarningBloccante, "", String.Format(My.Resources.AgronicaCoreMapper.ImpostazioneLottiObbligatoriaPerAziendaMagazzinoEsterno, Magazzino_Esterno_Des, Rag_Soc, ImpostazioneLottiQdC_Descrizione, Magazzino_Esterno_Des), ""))
                    End If

                    If gestioneLotto_Azienda = enum_Gestione_Lotti.Obbligatoria AndAlso String.IsNullOrWhiteSpace(Lotto) Then
                        ListaErrori.Add(generaErroreGias(ErroreGias_Severity.WarningBloccante, "", String.Format(My.Resources.AgronicaCoreMapper.ImpostazioneLottiObbligatoriaPerAziendaQdC, Rag_Soc, ImpostazioneLottiEsterna_Descrizione, Rag_Soc), ""))
                    End If
                End If

                If gestioneLotto_Azienda = enum_Gestione_Lotti.Nessuna AndAlso gestioneLotto_Azienda_Esterna = enum_Gestione_Lotti.Nessuna AndAlso Not String.IsNullOrWhiteSpace(Lotto) Then
                    ListaErrori.Add(generaErroreGias(ErroreGias_Severity.WarningBloccante, "", String.Format(My.Resources.AgronicaCoreMapper.ImpostazioneLottiNessunaPerAziendaQdCEsterna, Rag_Soc, Magazzino_Esterno_Des), ""))
                Else
                    If gestioneLotto_Azienda_Esterna = enum_Gestione_Lotti.Nessuna AndAlso Not String.IsNullOrWhiteSpace(Lotto) Then
                        ListaErrori.Add(generaErroreGias(ErroreGias_Severity.WarningBloccante, "", String.Format(My.Resources.AgronicaCoreMapper.ImpostazioneLottiNessunaPerAziendaMagazzinoEsterno, Magazzino_Esterno_Des, Rag_Soc, ImpostazioneLottiQdC_Descrizione, Magazzino_Esterno_Des), ""))
                    End If

                    If gestioneLotto_Azienda = enum_Gestione_Lotti.Nessuna AndAlso Not String.IsNullOrWhiteSpace(Lotto) Then
                        ListaErrori.Add(generaErroreGias(ErroreGias_Severity.WarningBloccante, "", String.Format(My.Resources.AgronicaCoreMapper.ImpostazioneLottiNessunaPerAziendaQdC, Rag_Soc, ImpostazioneLottiEsterna_Descrizione, Rag_Soc), ""))
                    End If
                End If

            Else

                If gestioneMagazzino_Azienda = 1 AndAlso gestioneMagazzino_Azienda_Esterna = 0 Then
                    ListaErrori.Add(generaErroreGias(ErroreGias_Severity.WarningBloccante, "", String.Format(My.Resources.AgronicaCoreMapper.AbilitareGestioneMagazzinoImpresaEsterna, Magazzino_Esterno_Des), ""))
                End If

                If gestioneMagazzino_Azienda = 0 AndAlso gestioneMagazzino_Azienda_Esterna = 1 Then
                    ListaErrori.Add(generaErroreGias(ErroreGias_Severity.WarningBloccante, "", String.Format(My.Resources.AgronicaCoreMapper.AbilitareGestioneMagazzinoPerProdottiMagazzinoEsterno, Rag_Soc), ""))
                End If

            End If

        ElseIf Visualizza_Magazzini_Esterni Then

            ListaErrori.Add(generaErroreGias(ErroreGias_Severity.WarningBloccante, "", String.Format(My.Resources.AgronicaCoreMapper.SelezionareProdottoDaMagazzinoEsterno), ""))

        End If

    End Sub

    Public Shared Function LoadModalitaApplicazione(ByVal modalitaApplicazione As Integer, ByVal verbose As Boolean, ByVal objParametri_Server As AgronicaCoreParametri) As baseClass.BaseCodeDescr

        Dim Modalita_Applicazione As New baseClass.BaseCodeDescr(modalitaApplicazione, "")

        If verbose Then

            Dim DT As New DataTable

            If modalitaApplicazione < 0 Then

                Dim objMetaschemaDAL As New AgronicaCoreMetaSchemaDAL.Modalita_Applicazione_Globali

                DT = objMetaschemaDAL.Leggi(modalitaApplicazione, "", "", "", objParametri_Server)

                If Not IsNothing(DT) AndAlso DT.Rows.Count = 1 Then
                    Modalita_Applicazione = New baseClass.BaseCodeDescr(DT(0)("Codice"), DT(0)("AGEA_Des"))
                End If

            ElseIf modalitaApplicazione > 0 Then

                Dim objVarieDAL As New AgronicaCoreVarieDAL.Modalita_Applicazione

                DT = objVarieDAL.Leggi(modalitaApplicazione, 0, "", "", objParametri_Server)

                If Not IsNothing(DT) AndAlso DT.Rows.Count = 1 Then
                    Modalita_Applicazione = New baseClass.BaseCodeDescr(DT(0)("Codice"), DT(0)("Descrizione_Personalizzata"))
                End If

            End If
        Else

        End If

        Return Modalita_Applicazione
    End Function
    
    Public Shared Function CreaEsercizioCdC(attivita As Attivita, infoOperazione As InfoOperazione,
                                            appezza As Integer, id_reg As Integer, progetto_cod As Integer, qta2 As Decimal,
                                            Sup_Riduzione_BufferZone As Decimal, Perc_Riduzione_Deriva As Decimal) As EsercizioCDC

        Dim esercizio = CreaEsercizio(attivita, infoOperazione, appezza, id_reg, progetto_cod)

        Dim esercizioCDC As New EsercizioCDC With {
            .superficieTrattata = qta2,
            .esercizio = esercizio
        }

        If infoOperazione.IsTrattamento OrElse infoOperazione.IsFertilizzazione Then
            esercizioCDC.superficieRiduzioneBufferZone = Sup_Riduzione_BufferZone
            esercizioCDC.percentualeRiduzioneDeriva = Perc_Riduzione_Deriva
        End If

        Return esercizioCDC

    End Function

    Private Shared Function CreaEsercizio(attivita As Attivita, infoOperazione As InfoOperazione, appezza As Integer, id_reg As Integer, progetto_cod As Integer) As Esercizio

        Dim appezzamentoPK = New Appezzamento.PK(appezza, attivita.centroAziendale.primaryKey)
        Dim impiantoPK = New Impianto.PK(id_reg, appezzamentoPK)

        Dim esercizio = New Esercizio(progetto_cod, "") With {
            .impiantoPK = impiantoPK
        }

        Return esercizio

    End Function


#Region "Controlli RICETTE X APP"
    Public Shared Function componiErroreFinale_controlloMagazziniRicettexApp(isFromMenu As Boolean,
                                                                             listErrori As List(Of String),
                                                                             listErroriRicetteMultiAttivita As List(Of String),
                                                                             listErroriRicetteNonGestite As List(Of String),
                                                                             counterRicetteToRemove As Integer
                                                                             ) As List(Of ErroreGias)
        Dim listErroriGias As New List(Of ErroreGias)
        Dim msg As String = ""

        If counterRicetteToRemove > 0 Then
            If counterRicetteToRemove = 1 AndAlso isFromMenu Then
                msg &= "<b>" & Gias.NonPossibileInviareSeguenteRicettaAPP & ": " & "</b>" & NEWLINE &
                String.Join(NEWLINE, listErrori) & NEWLINE
            Else
                If listErrori.Count > 0 Then
                    msg &= "<b>" & String.Format(Gias.NonPossibileInviareSeguentiNRicetteAPP, counterRicetteToRemove.ToString()) & ":" & "</b>" & NEWLINE &
                        String.Join(NEWLINE, listErrori) & NEWLINE
                End If
            End If

            If listErroriRicetteMultiAttivita.Count > 0 Then
                If listErroriRicetteMultiAttivita.Count > 1 AndAlso isFromMenu Then
                    msg &= NEWLINE & Gias.APPNoAncoraGestiteMultiAttivitaSeguentiRicetteNonInviabili & ":" & NEWLINE &
                        String.Join(NEWLINE, listErroriRicetteMultiAttivita)
                Else
                    msg &= NEWLINE & Gias.APPNoAncoraGestiteMultiAttivitaRicettaNonInviabile
                End If
            End If

            If listErroriRicetteNonGestite.Count > 0 Then
                If listErroriRicetteNonGestite.Count > 1 AndAlso isFromMenu Then
                    msg &= NEWLINE & Gias.APPNoAncoraGestiteOperazioneRicetteNonInviabili & ":" & NEWLINE &
                        String.Join(NEWLINE, listErroriRicetteNonGestite)
                Else
                    msg &= NEWLINE & Gias.APPNoAncoraGestiteOperazioneRicettaNonInviabile
                End If
            End If

            listErroriGias.Add(New ErroreGias With {
                               .severity = ErroreGias_Severity.WarningBloccante,
                               .messaggio = msg
                               })
        End If

        Return listErroriGias

    End Function

    Public Shared Sub componiErrorexRicetta_controlloMagazziniRicettexApp(attivita As Attivita,
                                                                          listErrorixRicetta As List(Of String),
                                                                          ByRef listErrori As List(Of String),
                                                                          isFromMenu As Boolean)

        If listErrorixRicetta.Count > 0 Then
            If isFromMenu Then
                listErrori.Add("• " & attivita.inizio & " - " & attivita.job.descrizione & ":" & NEWLINE & String.Join(NEWLINE, listErrorixRicetta))
            Else
                listErrori.Add(attivita.job.descrizione & ":" & NEWLINE & String.Join(NEWLINE, listErrorixRicetta))
            End If
        End If

    End Sub

    Public Shared Sub AddErroreToRicettexApp(ByVal ricetta As APP_Ricette_Operazioni,
                                             ByRef ricetteToRemove As List(Of APP_Ricette_Operazioni))

        If Not IsNothing(ricetta) AndAlso Not IsNothing(ricetteToRemove) Then
            If ricetteToRemove.FindIndex(Function(r) r.Ricetta_Cod = ricetta.Ricetta_Cod AndAlso r.Ricetta_Operazione_Cod = ricetta.Ricetta_Operazione_Cod) = -1 Then
                ricetteToRemove.Add(ricetta)
            End If
        End If

    End Sub

    Public Shared Sub esegui_ControlloMagazziniRicettexApp(attivita As Attivita,
                                                            ByRef listErrorixRicetta As List(Of String),
                                                            ByRef listErrori As List(Of String),
                                                            objParametri_Server As AgronicaCoreParametri)

        isMagazzinoVisibileApp_RicettexApp(attivita, listErrorixRicetta, objParametri_Server)
        isScaricoCompatibile_RicettexApp(attivita, listErrorixRicetta)

    End Sub

    Private Shared Sub isMagazzinoVisibileApp_RicettexApp(attivita As Attivita,
                                                          ByRef listErrorixRicetta As List(Of String),
                                                          objParametri_Server As AgronicaCoreParametri)

        Dim listMagazziniControllati As New List(Of String)

        Dim risorseProdotto As List(Of RisorsaProdotto) = attivita.risorse.FindAll(Function(c) (c.classType = ClassType.DettaglioTrattamento OrElse c.classType = ClassType.DettaglioFertilizzazione OrElse c.classType = ClassType.DettaglioSemina)).ConvertAll(Function(obj1) CType(obj1, AgronicaCoreModelsSTD.attivita.risorse.RisorsaProdotto))
        If risorseProdotto IsNot Nothing AndAlso risorseProdotto.Count > 0 Then
            For Each risorsaProdotto In risorseProdotto
                If risorsaProdotto.prodotto IsNot Nothing AndAlso risorsaProdotto.prodotto.codice <> 0 Then
                    If risorsaProdotto.MagazziniMovimentazioni IsNot Nothing AndAlso risorsaProdotto.MagazziniMovimentazioni.Count > 0 Then
                        For Each movimentoMagazzino In risorsaProdotto.MagazziniMovimentazioni

                            Dim Magazzino = movimentoMagazzino.Magazzino
                            Dim keyMagazzino As String = Magazzino.primaryKey.centroAziendalePK.partitaIva & "_" &
                                                               Magazzino.primaryKey.centroAziendalePK.codice & "_" &
                                                               Magazzino.primaryKey.codice

                            Dim magazzinoCompatibile As Boolean = True

                            If listMagazziniControllati.Contains(keyMagazzino) Then
                                Continue For
                            Else
                                Dim objFabbricatiCodici As New AgronicaCoreAnagrafeDAL.Fabbricati_Codici_R
                                Dim dt = objFabbricatiCodici.Leggi(Magazzino.primaryKey.centroAziendalePK.partitaIva,
                                                                   Magazzino.primaryKey.centroAziendalePK.codice,
                                                                   Magazzino.primaryKey.codice,
                                                                   enum_CodiciAnagrafe.Visibile_da_App, "",
                                                                   enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                   "", "",
                                                                   objParametri_Server)

                                If dt.Rows.Count > 0 Then
                                    If dt(0).Item("Val_Cod") = 0 Then
                                        magazzinoCompatibile = False
                                    End If
                                Else
                                    magazzinoCompatibile = False
                                End If

                                If Not magazzinoCompatibile Then
                                    'AF TODO msg
                                    listErrorixRicetta.Add(TEXTINDENT & String.Format(Gias.MagazzinoIndicatoScaricoXNonVisibileAPP, Magazzino.descrizione) & ".")
                                End If

                                listMagazziniControllati.Add(keyMagazzino)
                            End If
                        Next
                    End If
                End If
            Next
        End If

    End Sub

    Private Shared Sub isScaricoCompatibile_RicettexApp(attivita As Attivita, ByRef listErrorixRicetta As List(Of String))

        Dim dicProdottoxMagazzino As New Dictionary(Of Integer, (String, String))
        Dim dicProdottoxMagazzinoxLotto As New Dictionary(Of Integer, String)

        Dim risorseProdotto As List(Of RisorsaProdotto) = attivita.risorse.FindAll(Function(c) (c.classType = ClassType.DettaglioTrattamento OrElse c.classType = ClassType.DettaglioFertilizzazione OrElse c.classType = ClassType.DettaglioSemina)).ConvertAll(Function(obj1) CType(obj1, AgronicaCoreModelsSTD.attivita.risorse.RisorsaProdotto))
        If risorseProdotto IsNot Nothing AndAlso risorseProdotto.Count > 0 Then
            For Each risorsaProdotto In risorseProdotto
                If risorsaProdotto.prodotto IsNot Nothing AndAlso risorsaProdotto.prodotto.codice <> 0 Then
                    If risorsaProdotto.MagazziniMovimentazioni IsNot Nothing AndAlso risorsaProdotto.MagazziniMovimentazioni.Count > 0 Then
                        For Each movimentoMagazzino In risorsaProdotto.MagazziniMovimentazioni

                            Dim Magazzino = movimentoMagazzino.Magazzino
                            Dim keyMagazzino As String = Magazzino.primaryKey.centroAziendalePK.partitaIva & "_" &
                                                               Magazzino.primaryKey.centroAziendalePK.codice & "_" &
                                                               Magazzino.primaryKey.codice

                            If dicProdottoxMagazzino.ContainsKey(risorsaProdotto.prodotto.codice) Then
                                If dicProdottoxMagazzino(risorsaProdotto.prodotto.codice).Item1 <> keyMagazzino Then
                                    'af todo msg
                                    listErrorixRicetta.Add(TEXTINDENT & String.Format(Gias.APPImpossibileScaricareStessoProdottoXDaDiversiMagazziniStessoIntervento, risorsaProdotto.prodotto.descrizione) & ".")
                                End If

                                If dicProdottoxMagazzino(risorsaProdotto.prodotto.codice).Item1 = keyMagazzino AndAlso
                                    dicProdottoxMagazzino(risorsaProdotto.prodotto.codice).Item2 <> movimentoMagazzino.Lotto.ToUpper() Then
                                    If Not dicProdottoxMagazzinoxLotto.ContainsKey(risorsaProdotto.prodotto.codice) Then
                                        listErrorixRicetta.Add(TEXTINDENT & String.Format(Gias.APPImpossibileScaricareStessoProdottoXLottiDiversiStessoIntervento, risorsaProdotto.prodotto.descrizione) & ".")
                                        dicProdottoxMagazzinoxLotto.Add(risorsaProdotto.prodotto.codice, keyMagazzino)
                                    End If
                                End If

                            Else
                                dicProdottoxMagazzino.Add(risorsaProdotto.prodotto.codice, (keyMagazzino, movimentoMagazzino.Lotto.ToUpper()))
                            End If
                        Next
                    End If
                End If
            Next
        End If
    End Sub

    Public Shared Function controlloMagazziniRicettexApp(listaRicette As List(Of AgronicaCoreDTOStd.InData.Agenda.APP_Ricette_Operazioni),
                                                         fromMenu As Boolean,
                                                         objParametri_Utenti As AgronicaCoreParametri,
                                                         objParametri_Server As AgronicaCoreParametri,
                                                         objParametri_Super_Server As AgronicaCoreParametri
                                                         ) As List(Of ErroreGias)
        Dim listErrorixRicetta As New List(Of String)
        Dim listErroriRicetteMultiAttivita As New List(Of String)
        Dim listErrori As New List(Of String)

        Dim listErroriGias As New List(Of ErroreGias)

        Dim currentAttivitaDes As String = ""

        Dim gefutils As New AgronicaCoreEntityFramework.Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
        Dim db As New AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities(EFConnString)

        Dim ricetteToRemove As New List(Of APP_Ricette_Operazioni)

        For Each Ricetta_Operazione In listaRicette
            Dim ricettaOP As AgronicaCoreEntityFramework_POCO.Ricette_Operazioni = AgronicaCoreContabDAL.EFRicette.ReadRicettaOperazione(db, objParametri_Server.PivaSuperUser, Ricetta_Operazione.Ricetta_Operazione_Cod)

            If ricettaOP.Raccoglitore_Cod <> 0 Then

                If fromMenu Then

                    'NON GESTIAMO IL MULTI OPERAZIONE IN APP
                    currentAttivitaDes = ricettaOP.Ricetta_Operazione_Des
                    'AF TODO MSG
                    listErroriRicetteMultiAttivita.Add("• " & ricettaOP.Validita_Inizio & " - " & currentAttivitaDes)

                    ricetteToRemove.Add(Ricetta_Operazione)
                Else

                End If
            Else
                listErrorixRicetta.Clear()

                Dim map As New RicettaToAttivita
                currentAttivitaDes = ricettaOP.Ricetta_Operazione_Des
                Dim attivita = map.RicettaOperazioneSuAttivita(ricettaOP, listParametriAggiuntivi:=Nothing, verbose:=True, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)

                isMagazzinoVisibileApp_RicettexApp(attivita, listErrorixRicetta, objParametri_Server)

                isScaricoCompatibile_RicettexApp(attivita, listErrorixRicetta)

                If listErrorixRicetta.Count > 0 Then
                    listErrori.Add("• " & ricettaOP.Validita_Inizio & " - " & currentAttivitaDes & ":" & NEWLINE & String.Join(NEWLINE, listErrorixRicetta))
                    ricetteToRemove.Add(Ricetta_Operazione)
                End If
            End If
        Next

        If ricetteToRemove.Count > 0 Then
            Dim msg As String = ""
            If ricetteToRemove.Count = 1 Then
                msg &= "<b>" & Gias.NonPossibileInviareSeguenteRicettaAPP & ": " & "</b>" & NEWLINE &
                String.Join(NEWLINE, listErrori) & NEWLINE
            Else
                If listErrori.Count > 0 Then
                    msg &= "<b>" & String.Format(Gias.NonPossibileInviareSeguentiNRicetteAPP, ricetteToRemove.Count.ToString()) & ":" & "</b>" & NEWLINE &
                        String.Join(NEWLINE, listErrori) & NEWLINE
                End If
            End If

            If listErroriRicetteMultiAttivita.Count > 0 Then
                msg &= NEWLINE & Gias.APPNoAncoraGestiteMultiAttivitaSeguentiRicetteNonInviabili & ":" & NEWLINE &
                    String.Join(NEWLINE, listErroriRicetteMultiAttivita)
            End If

            listErroriGias.Add(New ErroreGias With {
                               .severity = ErroreGias_Severity.WarningBloccante,
                               .messaggio = msg
                               })

            For Each ricetta In ricetteToRemove
                listaRicette.Remove(ricetta)
            Next

        End If

        Return listErroriGias
    End Function

#End Region

    Public Shared Function GetDescrizioneFromUtilizzoTerreno(ByVal utilizzoTerreno As UtilizzoTerreno) As String

        Dim descrizione As String = String.Empty

        If utilizzoTerreno IsNot Nothing Then
            Select Case utilizzoTerreno.classType
                Case ClassType.Varieta
                    descrizione = CType(utilizzoTerreno, Varieta).specie.descrizione

                Case ClassType.DestinazioneUso
                    descrizione = CType(utilizzoTerreno, DestinazioneUso).descrizione

            End Select
        End If

        Return descrizione

    End Function

    Public Shared Function RilievoSenzaImpianti(ByVal attivita As Attivita) As Boolean
        Dim NoImpianti As Boolean = False

        If Not IsNothing(attivita) Then

            If Not IsNothing(attivita.job) Then

                Dim key_list = attivita.job.primaryKey.codice.Split("|")

                If Not IsNothing(key_list) AndAlso key_list.Count > 0 AndAlso
                    (key_list.Contains(LAVCOD_RILIEVO_INDICI_MATURITA) OrElse key_list.Contains(LAVCOD_DANNI_RACCOLTA)) Then

                    Dim specie As Specie = GetSpecieFromUtilizzoTerreno(attivita.utilizzoTerreno)

                    If IsNothing(specie) OrElse specie.codice = NessunaSpecieQdC Then

                        Dim eserciziCDC As List(Of EsercizioCDC) = attivita.centriDiCosto.FindAll(Function(c) c.classType.Equals(ClassType.EsercizioCDC)).
                                                                        ConvertAll(Function(obj1) CType(obj1, EsercizioCDC))

                        If IsNothing(eserciziCDC) OrElse eserciziCDC.Count = 0 Then
                            Dim dettagliRilievo As List(Of dettagli.DettaglioRilievo) = attivita.risorse.FindAll(Function(c) c.classType.Equals(ClassType.DettaglioRilievo)).
                                                                                        ConvertAll(Function(obj1) CType(obj1, dettagli.DettaglioRilievo))

                            If Not IsNothing(dettagliRilievo) AndAlso dettagliRilievo.Count > 0 Then
                                Dim dettagliRilievoSenzaImpianti As List(Of dettagli.DettaglioRilievo) = (From dettaglio In dettagliRilievo
                                                                                                          Where IsNothing(dettaglio.esercizioCDC) OrElse
                                                                                                             (dettaglio.esercizioCDC.esercizio.codice = 0 AndAlso
                                                                                                             dettaglio.esercizioCDC.esercizio.impiantoPK.codice = 0 AndAlso
                                                                                                             dettaglio.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.codice = 0)
                                                                                                          Select dettaglio).ToList()

                                If dettagliRilievo.Count = dettagliRilievoSenzaImpianti.Count Then
                                    NoImpianti = True
                                End If

                            End If

                        End If

                    End If

                End If

            End If
        End If

        Return NoImpianti
    End Function

    Public Shared Function GetAcquaTotale(ByVal risorsaAcqua As RisorsaAcqua, ByVal sup_trattata As Decimal) As Decimal
        Dim acquaTotale As Decimal = 0

        If Not IsNothing(risorsaAcqua) Then
            Select Case risorsaAcqua.doseAcqua
                Case RisorsaAcqua.TipoDoseAcqua.HA
                    If sup_trattata > 0 Then
                        acquaTotale = risorsaAcqua.acqua * sup_trattata
                    End If
                Case RisorsaAcqua.TipoDoseAcqua.TOTALE
                    acquaTotale = risorsaAcqua.acqua
            End Select
        End If

        Return acquaTotale
    End Function

    Public Shared Function ConvertValueFromTipoControllo(ByVal value As Decimal, ByVal tipoControllo_Cod As enum_TipoControllo)

        Dim value_converted As Decimal = value

        Select Case tipoControllo_Cod
            Case enum_TipoControllo.NUMERO_DECIMALE
                value_converted = Agro_Math.ArrotondaVal_4(value_converted)
            Case enum_TipoControllo.NUMERO_INTERO
                value_converted = Agro_Math.ArrotondaVal_0(value_converted)
        End Select

        Return value_converted

    End Function

    Public Shared Function GetOrigineRicetta(ByVal listAttivita As List(Of Attivita)) As String
        Dim origine As String = ""

        Dim listAttivitaConAssociazionePK = listAttivita.FindAll(Function(a) Not IsNothing(a.associazionePK) AndAlso a.codice = "0")

        If Not IsNothing(listAttivitaConAssociazionePK) AndAlso listAttivitaConAssociazionePK.Count > 0 Then
            Dim listaOrigine As List(Of String) = listAttivitaConAssociazionePK.GroupBy(Function(a) a.origine).Select(Of String)(Function(g) g.Key).ToList()

            If Not IsNothing(listaOrigine) AndAlso listaOrigine.Count > 0 Then
                If listaOrigine.Count = 1 Then
                    origine = listaOrigine(0)
                End If
            End If
        End If

        Return origine
    End Function

    Public Shared Function Controlla_Giacenza_Fertilizzanti(ByVal lista_Attivita As List(Of AgronicaCoreModelsSTD.attivita.Attivita),
                                                           ByVal objParametri_Super_Server As AgronicaCoreParametri,
                                                           ByVal objParametri_Server As AgronicaCoreParametri,
                                                           ByVal objParametri_Utenti As AgronicaCoreParametri) As List(Of ErroreGias)

        Dim listErroriGias As New List(Of ErroreGias)

        Dim TupleMagazziniFertilizzanti As New List(Of Tuple(Of String, String, Decimal))

        If Not IsNothing(lista_Attivita) AndAlso lista_Attivita.Count > 0 Then
            Dim dettagliFertilizzazione As List(Of dettagli.DettaglioFertilizzazione) = lista_Attivita.SelectMany(Function(a) a.risorse).Where(Function(r) r.classType = ClassType.DettaglioFertilizzazione).Cast(Of dettagli.DettaglioFertilizzazione)().ToList()

            If Not IsNothing(dettagliFertilizzazione) AndAlso dettagliFertilizzazione.Count > 0 Then

                Dim existDettagliFertilizzazioneMagazzino As Integer = dettagliFertilizzazione.FindIndex(Function(d) Not IsNothing(d.MagazziniMovimentazioni) AndAlso d.MagazziniMovimentazioni.Count > 0)

                If existDettagliFertilizzazioneMagazzino > -1 Then

                    Dim Dt_Giacenze As New DataTable

                    For Each attivita In lista_Attivita

                        Dim objG As New AgronicaCoreStampeDAL.Magazzino

                        Dim data As Date = attivita.inizio

                        Dim piva As String = ""

                        If Not IsNothing(attivita.centriDiCosto) AndAlso attivita.centriDiCosto.FindIndex(Function(c) c.classType = ClassType.EsercizioCDC) > -1 Then
                            piva = attivita.centriDiCosto.Where(Function(r) r.classType = ClassType.EsercizioCDC).Cast(Of EsercizioCDC)().Select(Of String)(Function(c) c.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva).FirstOrDefault()
                        End If

                        Dim xFiltroAggiuntivo_MagazzinoAttivoAllaData As String = " AND (Fabbricati.Validita_Inizio <= " & AgronicaCoreDataProvider.UtilityProvider.Agro_SQL_SaveDate(data) & " AND Fabbricati.Validita_Fine >= " & AgronicaCoreDataProvider.UtilityProvider.Agro_SQL_SaveDate(data) & ")"

                        Dim Dt_Giacenze_Tmp = objG.SchedaGiacenzeMagazzino(data,
                                                                    piva, 0, 0,
                                                                    FERTILIZZANTI,
                                                                    Pro_Cod:=0,
                                                                    0, 0, 0, 0, 0,
                                                                    LOTTO_NONDEFINITO,
                                                                    Flag_QtaNoZero:=False, 'DT: ex escludiGiacenzeZero
                                                                    xFiltroAggiuntivo_MagazzinoAttivoAllaData, "",
                                                                    "", "",
                                                                    "", "",
                                                                    "", "",
                                                                    "", "",
                                                                    "", "",
                                                                    "",
                                                                    objParametri_Server, objParametri_Utenti,
                                                                    Flag_QtaMaggioreZero:=False)

                        Dt_Giacenze.Merge(Dt_Giacenze_Tmp)
                    Next

                    For Each dettaglioFertilizzazione In dettagliFertilizzazione
                        If Not IsNothing(dettaglioFertilizzazione.MagazziniMovimentazioni) AndAlso dettaglioFertilizzazione.MagazziniMovimentazioni.Count > 0 Then
                            For Each movimentoMagazzino In dettaglioFertilizzazione.MagazziniMovimentazioni

                                Dim N As Decimal = dettaglioFertilizzazione.N

                                Dim P As Decimal = dettaglioFertilizzazione.P

                                Dim K As Decimal = dettaglioFertilizzazione.K

                                Dim Cu As Decimal = dettaglioFertilizzazione.Cu

                                Dim Pro_Des As String = dettaglioFertilizzazione.prodotto.descrizione

                                Dim Pro_Cod As Integer = dettaglioFertilizzazione.prodotto.codice

                                Dim newFertilizzante As String = Pro_Des & " ( " & N & " - " & P & " - " & K & " - " & Cu & " )"

                                Dim newQta As Decimal = movimentoMagazzino.Qta

                                Dim newUdm_Sim As String = movimentoMagazzino.udm.simbolo

                                Dim Lotto As String = movimentoMagazzino.Lotto

                                Dim Fabbricato_Des As String = movimentoMagazzino.Magazzino.descrizione

                                Dim Fabbricato_Cod As Integer = movimentoMagazzino.Magazzino.primaryKey.codice

                                Dim Sa_Cod As Integer = movimentoMagazzino.Magazzino.primaryKey.centroAziendalePK.codice

                                Dim Piva As String = movimentoMagazzino.Magazzino.primaryKey.centroAziendalePK.partitaIva

                                Dim keyMagazzino As String = Pro_Cod & "_" &
                                                             Lotto & "_" &
                                                             Fabbricato_Cod & "_" &
                                                             Sa_Cod & "_" &
                                                             Piva

                                Dim index = TupleMagazziniFertilizzanti.FindIndex(Function(m) m.Item1 = keyMagazzino)

                                If index = -1 Then

                                    Dim descrizione As String = ""

                                    If Lotto <> "" Then
                                        descrizione = String.Format(My.Resources.AgronicaCoreMapper.GiacenzaMagazzinoProdottoConCaricoLotto, Fabbricato_Des, Pro_Des, Lotto, 0, "", newFertilizzante, newQta, newUdm_Sim, Fabbricato_Des, Lotto)
                                    Else
                                        descrizione = String.Format(My.Resources.AgronicaCoreMapper.GiacenzaMagazzinoProdottoConCarico, Fabbricato_Des, Pro_Des, 0, "", newFertilizzante, newQta, newUdm_Sim, Fabbricato_Des)
                                    End If

                                    TupleMagazziniFertilizzanti.Add(New Tuple(Of String, String, Decimal)(keyMagazzino, descrizione, 0))

                                    index = TupleMagazziniFertilizzanti.Count - 1
                                End If

                                If Not IsNothing(Dt_Giacenze) AndAlso Dt_Giacenze.Rows.Count > 0 Then
                                    Dim Dr_Giacenze = Dt_Giacenze.Select("Pro_Cod = " & Pro_Cod & " AND Sa_Cod = " & Sa_Cod & " AND Id_Destinazione = " & Fabbricato_Cod & " AND Lotto = '" & Lotto & "'")

                                    If Not IsNothing(Dr_Giacenze) AndAlso Dr_Giacenze.Length > 0 Then
                                        For Each riga As DataRow In Dr_Giacenze
                                            If Not IsNothing(riga("Giacenza")) AndAlso Not IsDBNull(riga("Giacenza")) Then

                                                Dim Udm_Sim_Giacenza As String = riga("Udm_Sim")

                                                Dim Qta_Giacenza As Decimal = TupleMagazziniFertilizzanti(index).Item3 + riga("Giacenza")

                                                Dim descrizione As String = ""

                                                If Lotto <> "" Then
                                                    descrizione = String.Format(My.Resources.AgronicaCoreMapper.GiacenzaMagazzinoProdottoConCaricoLotto, Fabbricato_Des, Pro_Des, Lotto, Qta_Giacenza, Udm_Sim_Giacenza, newFertilizzante, newQta, newUdm_Sim, Fabbricato_Des, Lotto)
                                                Else
                                                    descrizione = String.Format(My.Resources.AgronicaCoreMapper.GiacenzaMagazzinoProdottoConCarico, Fabbricato_Des, Pro_Des, Qta_Giacenza, Udm_Sim_Giacenza, newFertilizzante, newQta, newUdm_Sim, Fabbricato_Des)
                                                End If

                                                TupleMagazziniFertilizzanti(index) = New Tuple(Of String, String, Decimal)(
                                                    TupleMagazziniFertilizzanti(index).Item1,
                                                    descrizione,
                                                    Qta_Giacenza)
                                            End If
                                        Next
                                    End If

                                End If
                            Next
                        End If
                    Next
                End If
            End If
        End If

        If Not IsNothing(TupleMagazziniFertilizzanti) AndAlso TupleMagazziniFertilizzanti.Count > 0 Then

            Dim ListMessaggio As New List(Of String)

            For Each tupleMagazzino In TupleMagazziniFertilizzanti
                ListMessaggio.Add(tupleMagazzino.Item2)
            Next

            listErroriGias.Add(generaErroreGias(ErroreGias_Severity.Warning, "", String.Join(",", ListMessaggio), ""))
        End If


        Return listErroriGias


    End Function


    Public Shared Function ConvertValueFromUdM(ByVal value As Decimal, ByVal udm As UnitaDiMisura, Optional ByVal default_tipoControllo_Cod As enum_TipoControllo = enum_TipoControllo.NUMERO_DECIMALE)

        Dim value_converted As Decimal = ConvertValueFromTipoControllo(value, default_tipoControllo_Cod)

        If Not IsNothing(udm) AndAlso Not IsNothing(udm.tipoControllo) AndAlso udm.tipoControllo.codice > 0 Then
            value_converted = ConvertValueFromTipoControllo(value, udm.tipoControllo.codice)
        End If

        Return value_converted

    End Function
End Class