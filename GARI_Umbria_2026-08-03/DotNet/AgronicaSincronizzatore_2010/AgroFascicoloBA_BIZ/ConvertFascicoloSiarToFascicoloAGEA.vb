Imports Sincro_Agrea2Gias.MyWsAgriRER
Imports Sincro_Agrea2Gias

Public Class ConvertFascicoloSiarToFascicoloAGEA
    Public Sub ConvertFascicoloSiarToFascicoloAGEA(ByRef source As Sincro_Agrea2Gias.MyWsAgriRER.FascicoloSiar2Response, _
                                                   ByRef destination As ISWSToOprResponse)
        'Allevamenti
        convertAllevamenti(source, destination)

        'Particelle
        convertParticelle(source, destination)

        'DatiAnagrafici
        convertDatiAnagrafici(source, destination)

        'Diritti
        convertDiritti(source, destination)

        'IscrizioneCAA
        convertIscrizioneCAA(source, destination)

        'Persone
        convertPersone(source, destination)

        'ProduzioneQualità
        convertProduzioneQualita(source, destination)

        'ReferenteAziendale
        convertReferenteAziendale(source, destination)

        'ContiCorrenti
        convertContiCorrenti(source, destination)

        'ResponsabileFito
        convertResponsabileFito(source, destination)

        'StatoAzienda
        convertStatoAzienda(source, destination)

        'UnitaAziendali
        convertUnitaAziendali(source, destination)

        'MessaggioRisposta
        convertMessaggio(source, destination)

    End Sub

    Public Sub convertAllevamenti(ByRef source As Sincro_Agrea2Gias.MyWsAgriRER.FascicoloSiar2Response, _
                                  ByRef destination As ISWSToOprResponse)
        'Allevamenti
        If source.allevamenti IsNot Nothing Then
            If source.allevamenti.allevamentoLength > 0 Then
                Dim i As Integer = 0
                Dim allevamentol As New List(Of ISWSAllevamento)
                For Each allevamento In source.allevamenti.allevamento
                    'Inserisco Campi
                    Dim ISWSAllevamento As New ISWSAllevamento
                    ISWSAllevamento.allevamentoDescr = allevamento.allevamentoDescr
                    ISWSAllevamento.CodiceAzienda = allevamento.aziendaCodice
                    ISWSAllevamento.Cap = allevamento.cap
                    ISWSAllevamento.codAllevamento = allevamento.codAllevamento
                    ISWSAllevamento.comCodice = allevamento.comCodice
                    ISWSAllevamento.Comune = allevamento.comDescr
                    ISWSAllevamento.Denominazione = allevamento.denominazione
                    ISWSAllevamento.IdAllev = allevamento.idAllevamento
                    ISWSAllevamento.CodFiscaleDeten = allevamento.idFiscale
                    ISWSAllevamento.Indirizzo = allevamento.indirizzo
                    ISWSAllevamento.Localita = allevamento.localita
                    ISWSAllevamento.CodiceSpecie = allevamento.speCodice

                    If allevamento.consistenze IsNot Nothing Then
                        Dim consistenze As New ConsistenzeType
                        consistenze.consistenzaLength = allevamento.consistenze.consistenzaLength
                        If allevamento.consistenze.consistenzaLength > 0 Then
                            Dim j As Integer = 0
                            Dim consistenzel As New List(Of ConsistenzaType)
                            For Each consistenza In allevamento.consistenze.consistenza
                                Dim cons As New ConsistenzaType
                                cons.annoRif = consistenza.annoRif
                                cons.codZootecnica = consistenza.codZootecnica
                                cons.descrZootecnica = consistenza.descrZootecnica
                                cons.numCapi = consistenza.numCapi
                                consistenzel.Add(cons)
                                j += 1
                            Next
                            consistenze.consistenza = consistenzel.ToArray
                        End If
                        ISWSAllevamento.consistenze = consistenze
                    End If

                    allevamentol.Add(ISWSAllevamento)
                    i += 1
                Next
                destination.ISWSAllevamenti = allevamentol.ToArray
            End If
        End If
    End Sub

    Public Sub convertParticelle(ByRef source As Sincro_Agrea2Gias.MyWsAgriRER.FascicoloSiar2Response, _
                                 ByRef destination As ISWSToOprResponse)
        If source.conduzioniTerreni IsNot Nothing Then
            Dim conduzioniTerreni As New ConduzioniTerreniType
            conduzioniTerreni.particelleLength = source.conduzioniTerreni.particelleLength
            If source.conduzioniTerreni.particelleLength > 0 Then
                Dim i As Integer = 0
                Dim particelle As New List(Of ISWSTerritorio1)
                For Each particella In source.conduzioniTerreni.particelle
                    'Inserisco Campi (anche consistenza)
                    Dim ISWSTerritorio As New ISWSTerritorio1
                    ISWSTerritorio.CasiParticolari = particella.casiParticolari
                    ISWSTerritorio.codCom = particella.codCom
                    ISWSTerritorio.codiceClasse = particella.codiceClasse
                    ISWSTerritorio.codiceQualita = particella.codiceQualita
                    ISWSTerritorio.codProv = particella.codProv
                    ISWSTerritorio.codReg = particella.codReg
                    ISWSTerritorio.Comune = particella.comDescr
                    ISWSTerritorio.dataFine = particella.dataFine
                    ISWSTerritorio.dataInizio = particella.dataInizio
                    ISWSTerritorio.dataInserimento = particella.dataInserimento
                    ISWSTerritorio.dataVariazione = particella.dataVariazione
                    ISWSTerritorio.fasciaAltimetrica = particella.fasciaAltimetrica
                    ISWSTerritorio.fasciaAltimetricaDescr = particella.fasciaAltimetricaDescr
                    ISWSTerritorio.foglio = particella.foglio
                    ISWSTerritorio.fonte = particella.fonte
                    ISWSTerritorio.fonteDescr = particella.fonteDescr
                    ISWSTerritorio.idParticella = particella.idParticella
                    ISWSTerritorio.particella = particella.particella
                    ISWSTerritorio.Provincia = particella.provDescr
                    ISWSTerritorio.provSigla = particella.provSigla
                    ISWSTerritorio.regDescr = particella.regDescr
                    ISWSTerritorio.sezione = particella.sezione
                    ISWSTerritorio.subalterno = particella.subalterno
                    ISWSTerritorio.supCatastale = particella.supCatastale
                    ISWSTerritorio.tipoDocumento = particella.tipoDocumento
                    ISWSTerritorio.tipoDocumentoDescr = particella.tipoDocumentoDescr
                    ISWSTerritorio.utilizzo = particella.utilizzo

                    'Conduzione
                    If particella.conduzione IsNot Nothing Then
                        Dim conduzione As New ConduzioneParticellaType
                        conduzione.biologico = particella.conduzione.biologico
                        conduzione.fasciaAltimetrica = particella.conduzione.fasciaAltimetrica
                        conduzione.fasciaAltimetricaDescr = particella.conduzione.fasciaAltimetricaDescr
                        conduzione.flagAnomaliaMacrouso = particella.conduzione.flagAnomaliaMacrouso
                        conduzione.flagContenzioso = particella.conduzione.flagContenzioso
                        conduzione.flagSupero = particella.conduzione.flagSupero
                        conduzione.idAzienda = particella.conduzione.idAzienda
                        conduzione.idParticella = particella.conduzione.idParticella
                        conduzione.irriguo = particella.conduzione.irriguo
                        conduzione.percEleggibile = particella.conduzione.percEleggibile
                        conduzione.percEleggibileSpecified = particella.conduzione.percEleggibileSpecified
                        conduzione.rotazioneColturale = particella.conduzione.rotazioneColturale
                        conduzione.supEleggibile = particella.conduzione.supEleggibile
                        conduzione.totSupPossesso = particella.conduzione.totSupPossesso

                        conduzione.contrattiLength = particella.conduzione.contrattiLength
                        If particella.conduzione.contratti IsNot Nothing Then
                            Dim contrattil As New List(Of ContrattoType)
                            For Each contratto In particella.conduzione.contratti
                                'Inserisco Campi
                                Dim ISWSContratto As New ContrattoType
                                ISWSContratto.dataVariazione = contratto.dataVariazione
                                ISWSContratto.dtFine = contratto.dtFine
                                ISWSContratto.dtInizio = contratto.dtInizio
                                ISWSContratto.formaPossesso = contratto.formaPossesso
                                ISWSContratto.formaPossessoDescr = contratto.formaPossessoDescr
                                ISWSContratto.idAzienda = contratto.idAzienda
                                ISWSContratto.idParticella = contratto.idParticella
                                ISWSContratto.progr = contratto.progr
                                ISWSContratto.supPossesso = contratto.supPossesso
                                ISWSContratto.titoloConduzione = contratto.supPossesso
                                contrattil.Add(ISWSContratto)
                            Next
                            conduzione.contratti = contrattil.ToArray
                        End If


                        conduzione.macrousiLength = particella.conduzione.macrousiLength
                        If particella.conduzione.macrousi IsNot Nothing Then
                            Dim macrousil As New List(Of MacrousoType)
                            For Each macrouso In particella.conduzione.macrousi
                                'Inserisco Campi
                                Dim ISWSmacrouso As New MacrousoType

                                ISWSmacrouso.codMacrouso = macrouso.codMacrouso
                                ISWSmacrouso.fonte = macrouso.fonte
                                ISWSmacrouso.fonteDescr = macrouso.fonteDescr
                                ISWSmacrouso.macrousoDescr = macrouso.macrousoDescr
                                ISWSmacrouso.supMacrouso = macrouso.supMacrouso

                                macrousil.Add(ISWSmacrouso)
                            Next
                            conduzione.macrousi = macrousil.ToArray
                        End If

                        conduzione.proprietariLength = particella.conduzione.proprietariLength
                        If particella.conduzione.proprietari IsNot Nothing Then
                            Dim proprietaril As New List(Of ProprietarioType)
                            For Each proprietario In particella.conduzione.proprietari
                                'Inserisco Campi
                                Dim ISWSProprietario As New ProprietarioType
                                ISWSProprietario.cuaaProprietario = proprietario.cuaaProprietario
                                proprietaril.Add(ISWSProprietario)
                            Next
                            conduzione.proprietari = proprietaril.ToArray
                        End If

                        conduzione.unitaVitateLength = particella.conduzione.unitaVitateLength
                        If particella.conduzione.unitaVitate IsNot Nothing Then
                            Dim unitaVitatel As New List(Of UnitaVitataType)
                            For Each unitaVitata In particella.conduzione.unitaVitate
                                'Inserisco Campi
                                Dim ISWSUnita As New UnitaVitataType
                                ISWSUnita.altitudineSlm = unitaVitata.altitudineSlm
                                ISWSUnita.ancoraggiTestata = unitaVitata.ancoraggiTestata
                                ISWSUnita.annoImpianto = unitaVitata.annoImpianto
                                ISWSUnita.annoRiferimento = unitaVitata.annoRiferimento
                                ISWSUnita.codFiliSostegno = unitaVitata.codFiliSostegno
                                ISWSUnita.codPaliTessitura = unitaVitata.codPaliTessitura
                                ISWSUnita.codPaliTestata = unitaVitata.codPaliTestata
                                ISWSUnita.codStatoColt = unitaVitata.codStatoColt
                                ISWSUnita.codTipoVari = unitaVitata.codTipoVari
                                ISWSUnita.codVitigno = unitaVitata.codVitigno
                                ISWSUnita.dataCessazione = unitaVitata.dataCessazione
                                ISWSUnita.dataProtocollo = unitaVitata.dataProtocollo
                                ISWSUnita.dataRilievo = unitaVitata.dataRilievo
                                ISWSUnita.densita = unitaVitata.densita
                                ISWSUnita.destProduttiva = unitaVitata.destProduttiva
                                ISWSUnita.destProduttivaDescr = unitaVitata.destProduttivaDescr
                                ISWSUnita.distanzaPali = unitaVitata.distanzaPali
                                ISWSUnita.dtFine = unitaVitata.dtFine
                                ISWSUnita.dtFineGestione = unitaVitata.dtFineGestione
                                ISWSUnita.dtInizio = unitaVitata.dtInizio
                                ISWSUnita.dtInizioGestione = unitaVitata.dtInizioGestione
                                ISWSUnita.dtIns = unitaVitata.dtIns
                                ISWSUnita.dtVar = unitaVitata.dtVar
                                ISWSUnita.fallanzePerc = unitaVitata.fallanzePerc
                                ISWSUnita.flagAnomalia = unitaVitata.flagAnomalia
                                ISWSUnita.flagAttuale = unitaVitata.flagAttuale
                                ISWSUnita.flagCessata = unitaVitata.flagCessata
                                ISWSUnita.flagContributo = unitaVitata.flagContributo
                                ISWSUnita.flagRegolarizz2009 = unitaVitata.flagRegolarizz2009
                                ISWSUnita.flagRicalcoloGis = unitaVitata.flagRicalcoloGis
                                ISWSUnita.formaAllevamento = unitaVitata.formaAllevamento
                                ISWSUnita.formaAllevamentoDescr = unitaVitata.formaAllevamentoDescr
                                ISWSUnita.giacituraTerreno = unitaVitata.giacituraTerreno
                                ISWSUnita.giornoImpianto = unitaVitata.giornoImpianto
                                ISWSUnita.idAzienda = unitaVitata.idAzienda
                                ISWSUnita.idParticella = unitaVitata.idParticella
                                ISWSUnita.idUnitaVitata = unitaVitata.idUnitaVitata
                                ISWSUnita.idUtenteIns = unitaVitata.idUtenteIns
                                ISWSUnita.idUtenteVar = unitaVitata.idUtenteVar
                                ISWSUnita.irrigazione = unitaVitata.irrigazione
                                ISWSUnita.irrigazioneDescr = unitaVitata.irrigazioneDescr
                                ISWSUnita.meseImpianto = unitaVitata.meseImpianto
                                ISWSUnita.numCeppi = unitaVitata.numCeppi
                                ISWSUnita.numeroProtocollo = unitaVitata.numeroProtocollo
                                ISWSUnita.numUnitaVitata = unitaVitata.numUnitaVitata
                                ISWSUnita.progPoligono = unitaVitata.progPoligono
                                ISWSUnita.sestoSuFila = unitaVitata.sestoSuFila
                                ISWSUnita.sestoTraFila = unitaVitata.sestoTraFila
                                ISWSUnita.superficieServizioMq = unitaVitata.superficieServizioMq
                                ISWSUnita.supVitataDich = unitaVitata.supVitataDich
                                ISWSUnita.supVitataDichPRicalcolo = unitaVitata.supVitataDichPRicalcolo
                                ISWSUnita.terrazzamenti = unitaVitata.terrazzamenti
                                ISWSUnita.tipoColtura = unitaVitata.tipoColtura
                                ISWSUnita.tipoProcedimento = unitaVitata.tipoProcedimento
                                ISWSUnita.tipoUnar = unitaVitata.tipoUnar
                                ISWSUnita.tipoVariazione = unitaVitata.tipoVariazione
                                ISWSUnita.tipoVigneto = unitaVitata.tipoVigneto
                                ISWSUnita.unar = unitaVitata.unar
                                ISWSUnita.vitignoDescr = unitaVitata.vitignoDescr

                                ISWSUnita.altriVitigniLength = unitaVitata.altriVitigniLength
                                If unitaVitata.altriVitigni IsNot Nothing Then
                                    Dim altriVitignil As New List(Of MyWsAgriRER.VitignoType)
                                    For Each altroVitigno In unitaVitata.altriVitigni
                                        'Inserisco Campi
                                        Dim ISWSAltroVitigno As New MyWsAgriRER.VitignoType

                                        ISWSAltroVitigno.codVitigno = altroVitigno.codVitigno
                                        ISWSAltroVitigno.descrVitigno = altroVitigno.descrVitigno
                                        ISWSAltroVitigno.dtIns = altroVitigno.dtIns
                                        ISWSAltroVitigno.idUnitaVitata = altroVitigno.idUnitaVitata
                                        ISWSAltroVitigno.idUtenteIns = altroVitigno.idUtenteIns
                                        ISWSAltroVitigno.perc = altroVitigno.perc
                                        ISWSAltroVitigno.progr = altroVitigno.progr

                                        altriVitignil.Add(ISWSAltroVitigno)
                                    Next
                                    unitaVitata.altriVitigni = altriVitignil.ToArray
                                End If


                                ISWSUnita.idoneitaLength = unitaVitata.idoneitaLength
                                If unitaVitata.idoneita IsNot Nothing Then
                                    Dim idoneital As New List(Of IscrizioneType)
                                    For Each idoneita In unitaVitata.idoneita
                                        'Inserisco Campi
                                        Dim ISWSIdoneita As New IscrizioneType

                                        ISWSIdoneita.codTipologia = idoneita.codTipologia
                                        ISWSIdoneita.dataRev = idoneita.dataRev
                                        ISWSIdoneita.dataRic = idoneita.dataRic
                                        ISWSIdoneita.docigtDescr = idoneita.docigtDescr
                                        ISWSIdoneita.dtInizio = idoneita.dtInizio
                                        ISWSIdoneita.dtIns = idoneita.dtIns
                                        ISWSIdoneita.dtVar = idoneita.dtVar

                                        idoneital.Add(ISWSIdoneita)
                                    Next
                                    ISWSUnita.idoneita = idoneital.ToArray
                                End If

                                unitaVitatel.Add(ISWSUnita)
                            Next
                            conduzione.unitaVitate = unitaVitatel.ToArray
                        End If

                        conduzione.zoneLength = particella.conduzione.zoneLength
                        If particella.conduzione.zone IsNot Nothing Then
                            Dim zonel As New List(Of ZonaType)
                            For Each zona In particella.conduzione.zone
                                'Inserisco Campi
                                Dim ISWSZona As New ZonaType

                                ISWSZona.codZona = zona.codZona
                                ISWSZona.conforme = zona.conforme
                                ISWSZona.fonte = zona.fonte
                                ISWSZona.fonteDescr = zona.fonteDescr
                                ISWSZona.idParticella = zona.idParticella
                                ISWSZona.zonaDescr = zona.zonaDescr

                                zonel.Add(ISWSZona)
                            Next
                            conduzione.zone = zonel.ToArray
                        End If

                        ISWSTerritorio.conduzione = conduzione
                    End If

                    particelle.Add(ISWSTerritorio)
                    i += 1
                Next
                destination.ISWSTerritori1 = particelle.ToArray
            End If
        End If
    End Sub

    Public Sub convertContiCorrenti(ByRef source As Sincro_Agrea2Gias.MyWsAgriRER.FascicoloSiar2Response, _
                                    ByRef destination As ISWSToOprResponse)
        If source.contiCorrenti IsNot Nothing Then
            If source.contiCorrenti.contoCorrenteLength > 0 Then
                Dim i = 0
                Dim contiCorrente As New List(Of ISWSContoCorrente)
                For Each contoCorrente In source.contiCorrenti.contoCorrente
                    Dim ISWSContoCorrente As New ISWSContoCorrente

                    ISWSContoCorrente.anomalo = contoCorrente.anomalo
                    ISWSContoCorrente.cap = contoCorrente.cap
                    ISWSContoCorrente.cinBban = contoCorrente.cinBban
                    ISWSContoCorrente.cinIban = contoCorrente.cinIban
                    ISWSContoCorrente.codAbi = contoCorrente.codAbi
                    ISWSContoCorrente.codAnomalia = contoCorrente.codAnomalia
                    ISWSContoCorrente.codCab = contoCorrente.codCab
                    ISWSContoCorrente.codPaese = contoCorrente.codPaese
                    ISWSContoCorrente.ContoCorrente = contoCorrente.contoCorrente
                    ISWSContoCorrente.dataFine = contoCorrente.dataFine
                    ISWSContoCorrente.dataAnomalia = contoCorrente.dataAnomalia
                    ISWSContoCorrente.dataFine = contoCorrente.dataFine
                    ISWSContoCorrente.dataFineSportello = contoCorrente.dataFineSportello
                    ISWSContoCorrente.dataInizioSportello = contoCorrente.dataInizioSportello
                    ISWSContoCorrente.dataInserimento = contoCorrente.dataInserimento
                    ISWSContoCorrente.dataVariazione = contoCorrente.dataVariazione
                    ISWSContoCorrente.descrAnomalia = contoCorrente.descrAnomalia
                    ISWSContoCorrente.descrizione = contoCorrente.descrizione
                    ISWSContoCorrente.descrizioneComune = contoCorrente.descrizioneComune
                    ISWSContoCorrente.flagTesoriere = contoCorrente.flagTesoriere
                    ISWSContoCorrente.idAzienda = contoCorrente.idAzienda
                    ISWSContoCorrente.indirizzo = contoCorrente.indirizzo
                    ISWSContoCorrente.localita = contoCorrente.localita
                    ISWSContoCorrente.nomeFiliale = contoCorrente.nomeFiliale
                    ISWSContoCorrente.preferito = contoCorrente.preferito
                    ISWSContoCorrente.progr = contoCorrente.progr
                    ISWSContoCorrente.siglaProvincia = contoCorrente.siglaProvincia
                    contiCorrente.Add(ISWSContoCorrente)

                    i += 1
                Next
                destination.ISWSContiCorrente = contiCorrente.ToArray
            End If
        End If
    End Sub

    Public Sub convertDatiAnagrafici(ByRef source As Sincro_Agrea2Gias.MyWsAgriRER.FascicoloSiar2Response, _
                                     ByRef destination As ISWSToOprResponse)
        If source.datiAnagrafici IsNot Nothing Then
            Dim datiAnagrafici As New DatiAziendaType
            If source.datiAnagrafici.cittaEsteraLegale IsNot Nothing Then
                Dim cittaEsteraLegale As New CittaEsteraType
                'destination.datiAnagrafici.cittaEsteraLegale.citta = source.datiAnagrafici.cittaEsteraLegale.citta
                'destination.datiAnagrafici.cittaEsteraLegale.codStato = source.datiAnagrafici.cittaEsteraLegale.codStato
                'destination.datiAnagrafici.cittaEsteraLegale.statoDescr = source.datiAnagrafici.cittaEsteraLegale.statoDescr
                cittaEsteraLegale.citta = source.datiAnagrafici.cittaEsteraLegale.citta
                cittaEsteraLegale.codStato = source.datiAnagrafici.cittaEsteraLegale.codStato
                cittaEsteraLegale.statoDescr = source.datiAnagrafici.cittaEsteraLegale.statoDescr
                datiAnagrafici.cittaEsteraLegale = cittaEsteraLegale
            End If
            datiAnagrafici.codEsenzione = source.datiAnagrafici.codEsenzione
            datiAnagrafici.codOp = source.datiAnagrafici.codOp
            If source.datiAnagrafici.contatti IsNot Nothing Then
                Dim contatti As New ContattiType
                contatti.email = source.datiAnagrafici.contatti.email
                contatti.fax = source.datiAnagrafici.contatti.fax
                contatti.pec = source.datiAnagrafici.contatti.pec
                contatti.telefono = source.datiAnagrafici.contatti.telefono
                datiAnagrafici.contatti = contatti
            End If
            datiAnagrafici.cuaa = source.datiAnagrafici.cuaa
            datiAnagrafici.documento = source.datiAnagrafici.documento
            datiAnagrafici.dtCessazione = source.datiAnagrafici.dtCessazione
            datiAnagrafici.dtDocumento = source.datiAnagrafici.dtDocumento
            datiAnagrafici.dtValidazione = source.datiAnagrafici.dtValidazione
            datiAnagrafici.dtVariazione = source.datiAnagrafici.dtVariazione
            datiAnagrafici.esenzioneDescr = source.datiAnagrafici.esenzioneDescr
            datiAnagrafici.flagAltreSedi = source.datiAnagrafici.flagAltreSedi
            datiAnagrafici.flagValidato = source.datiAnagrafici.flagValidato
            datiAnagrafici.flagValidato = source.datiAnagrafici.flagValidato
            datiAnagrafici.formaGiuridica = source.datiAnagrafici.formaGiuridica
            datiAnagrafici.formaGiuridicaDescr = source.datiAnagrafici.formaGiuridicaDescr
            datiAnagrafici.idAzienda = source.datiAnagrafici.idAzienda
            datiAnagrafici.idUtenteValidazione = source.datiAnagrafici.idUtenteValidazione
            datiAnagrafici.numRea = source.datiAnagrafici.numRea
            datiAnagrafici.opDescr = source.datiAnagrafici.opDescr
            datiAnagrafici.partitaIva = source.datiAnagrafici.partitaIva
            datiAnagrafici.provRea = source.datiAnagrafici.provRea
            datiAnagrafici.ragioneSociale = source.datiAnagrafici.ragioneSociale
            If source.datiAnagrafici.sedeLegale IsNot Nothing Then
                Dim sedeLegale As New IndirizzoType
                sedeLegale.cap = source.datiAnagrafici.sedeLegale.cap
                If source.datiAnagrafici.sedeLegale.cittaEstera IsNot Nothing Then
                    Dim cittaEstera As New CittaEsteraType
                    cittaEstera.citta = source.datiAnagrafici.sedeLegale.cittaEstera.citta
                    cittaEstera.codStato = source.datiAnagrafici.sedeLegale.cittaEstera.codStato
                    cittaEstera.statoDescr = source.datiAnagrafici.sedeLegale.cittaEstera.statoDescr
                    sedeLegale.cittaEstera = cittaEstera
                End If
                sedeLegale.codCom = source.datiAnagrafici.sedeLegale.codCom
                sedeLegale.codProv = source.datiAnagrafici.sedeLegale.codProv
                sedeLegale.comDescr = source.datiAnagrafici.sedeLegale.comDescr
                sedeLegale.indirizzo = source.datiAnagrafici.sedeLegale.indirizzo
                sedeLegale.localita = source.datiAnagrafici.sedeLegale.localita
                sedeLegale.provDescr = source.datiAnagrafici.sedeLegale.provDescr
                sedeLegale.provSigla = source.datiAnagrafici.sedeLegale.provSigla
                datiAnagrafici.sedeLegale = sedeLegale
            End If
            destination.datiAnagrafici = datiAnagrafici
        End If
    End Sub

    Public Sub convertDiritti(ByRef source As Sincro_Agrea2Gias.MyWsAgriRER.FascicoloSiar2Response, _
                              ByRef destination As ISWSToOprResponse)
        Dim diritti As New DirittiReimpiantoType
        If source.diritti IsNot Nothing Then
            diritti.dirittoLength = source.diritti.dirittoLength
            If source.diritti.dirittoLength > 0 Then
                Dim dirittol As New List(Of DirittoReimpiantoType)
                For Each diritto In source.diritti.diritto
                    Dim ISWSDiritto As New DirittoReimpiantoType

                    ISWSDiritto.codAutorizzazione = diritto.codAutorizzazione
                    ISWSDiritto.dataProtocollo = diritto.dataProtocollo
                    ISWSDiritto.dataRilascio = diritto.dataRilascio
                    ISWSDiritto.dataTermine = diritto.dataTermine
                    ISWSDiritto.dtIns = diritto.dtIns
                    ISWSDiritto.dtVar = diritto.dtVar
                    ISWSDiritto.idAzienda = diritto.idAzienda
                    ISWSDiritto.numDiritto = diritto.numDiritto
                    ISWSDiritto.numeroProtocollo = diritto.numeroProtocollo
                    ISWSDiritto.praticaAutorizzata = diritto.praticaAutorizzata
                    ISWSDiritto.provRilascio = diritto.provRilascio
                    ISWSDiritto.provRilascioDescr = diritto.provRilascioDescr
                    ISWSDiritto.provRilascioSigla = diritto.provRilascioSigla
                    ISWSDiritto.supAutorizzata = diritto.supAutorizzata
                    ISWSDiritto.supImpiantata = diritto.supImpiantata
                    ISWSDiritto.supResidua = diritto.supResidua
                    ISWSDiritto.tipoDiritto = diritto.tipoDiritto
                    ISWSDiritto.tipoDirittoDescr = diritto.tipoDirittoDescr
                    ISWSDiritto.tipoProcedimento = diritto.tipoProcedimento


                    dirittol.Add(ISWSDiritto)
                Next
                diritti.diritto = dirittol.ToArray
            End If
            destination.diritti = diritti
        End If
    End Sub

    Public Sub convertIscrizioneCAA(ByRef source As Sincro_Agrea2Gias.MyWsAgriRER.FascicoloSiar2Response, _
                                    ByRef destination As ISWSToOprResponse)
        If source.iscrizioneCAA IsNot Nothing Then
            Dim iscrizioneCAA As New IscrizioneCAAType
            iscrizioneCAA.capCAA = source.iscrizioneCAA.capCAA
            iscrizioneCAA.codComuneCAA = source.iscrizioneCAA.codComuneCAA
            iscrizioneCAA.codFiscaleCAA = source.iscrizioneCAA.codFiscaleCAA
            iscrizioneCAA.codProvinciaCAA = source.iscrizioneCAA.codProvinciaCAA
            iscrizioneCAA.comuneCAADescr = source.iscrizioneCAA.comuneCAADescr
            iscrizioneCAA.dataInizio = source.iscrizioneCAA.dataInizio
            iscrizioneCAA.dataRichiesta = source.iscrizioneCAA.dataRichiesta
            iscrizioneCAA.dataVariazione = source.iscrizioneCAA.dataVariazione
            iscrizioneCAA.denominazione = source.iscrizioneCAA.denominazione
            iscrizioneCAA.dtFonte = source.iscrizioneCAA.dtFonte
            iscrizioneCAA.fonte = source.iscrizioneCAA.fonte
            iscrizioneCAA.fonteDescr = source.iscrizioneCAA.fonteDescr
            iscrizioneCAA.idAzienda = source.iscrizioneCAA.idAzienda
            iscrizioneCAA.idCAA = source.iscrizioneCAA.idCAA
            iscrizioneCAA.indirizzoCAA = source.iscrizioneCAA.indirizzoCAA
            iscrizioneCAA.provinciaCAADescr = source.iscrizioneCAA.provinciaCAADescr
            iscrizioneCAA.provinciaCAASigla = source.iscrizioneCAA.provinciaCAASigla
            iscrizioneCAA.utenteVariazione = source.iscrizioneCAA.utenteVariazione
            destination.iscrizioneCAA = iscrizioneCAA
        End If
    End Sub

    Public Sub convertPersone(ByRef source As Sincro_Agrea2Gias.MyWsAgriRER.FascicoloSiar2Response, _
                              ByRef destination As ISWSToOprResponse)
        Dim personeRuoli As New PersoneRuoliType
        If source.personeRuoli IsNot Nothing Then
            personeRuoli.personeLength = source.personeRuoli.personeLength
            Dim persone As New List(Of PersonaType)
            If source.personeRuoli.personeLength > 0 Then
                Dim i = 0
                For Each persona In source.personeRuoli.persone
                    Dim ISWSPersona As New PersonaType

                    ISWSPersona.codiceFiscale = persona.codiceFiscale
                    ISWSPersona.cognome = persona.cognome
                    ISWSPersona.documento = persona.documento
                    ISWSPersona.dtDocumento = persona.dtDocumento
                    ISWSPersona.dtFonte = persona.dtFonte
                    ISWSPersona.dtVariazione = persona.dtVariazione
                    ISWSPersona.flagReferente = persona.flagReferente
                    ISWSPersona.fonte = persona.fonte
                    ISWSPersona.fonteDescr = persona.fonteDescr
                    ISWSPersona.idPersona = persona.idPersona
                    ISWSPersona.nome = persona.nome
                    ISWSPersona.sesso = persona.sesso

                    'Residenza
                    If persona.residenza IsNot Nothing Then
                        Dim residenza As New IndirizzoType1
                        residenza.cap = persona.residenza.cap
                        If persona.residenza.cittaEstera IsNot Nothing Then
                            Dim cittaEstera As New CittaEsteraType1
                            cittaEstera.citta = persona.residenza.cittaEstera.citta
                            cittaEstera.codStato = persona.residenza.cittaEstera.codStato
                            cittaEstera.statoDescr = persona.residenza.cittaEstera.statoDescr
                            residenza.cittaEstera = cittaEstera
                        End If
                        residenza.codCom = persona.residenza.codCom
                        residenza.codProv = persona.residenza.codProv
                        residenza.comDescr = persona.residenza.comDescr
                        residenza.indirizzo = persona.residenza.indirizzo
                        residenza.localita = persona.residenza.localita
                        residenza.provDescr = persona.residenza.provDescr
                        residenza.provSigla = persona.residenza.provSigla
                        persona.residenza = residenza
                    End If



                    'Dati Nascita
                    If persona.datiNascita IsNot Nothing Then
                        Dim datiNascita As New DatiNascitaType1
                        If persona.datiNascita.cittaEsteraNascita IsNot Nothing Then
                            Dim cittaEsteraNascita As New CittaEsteraType1
                            cittaEsteraNascita.citta = persona.datiNascita.cittaEsteraNascita.citta
                            cittaEsteraNascita.codStato = persona.datiNascita.cittaEsteraNascita.codStato
                            cittaEsteraNascita.statoDescr = persona.datiNascita.cittaEsteraNascita.statoDescr
                            datiNascita.cittaEsteraNascita = cittaEsteraNascita
                        End If
                        datiNascita.codCom = persona.datiNascita.codCom
                        datiNascita.codProv = persona.datiNascita.codProv
                        datiNascita.comDescr = persona.datiNascita.comDescr
                        datiNascita.dataNascita = persona.datiNascita.dataNascita
                        datiNascita.provDescr = persona.datiNascita.provDescr
                        datiNascita.provSigla = persona.datiNascita.provSigla
                        persona.datiNascita = datiNascita
                    End If

                    'Contatti
                    If persona.contatti IsNot Nothing Then
                        Dim contatti As New ContattiType1
                        contatti.email = persona.contatti.email
                        contatti.fax = persona.contatti.fax
                        contatti.pec = persona.contatti.pec
                        contatti.telefono = persona.contatti.telefono
                        persona.contatti = contatti
                    End If

                    If persona.ruolo IsNot Nothing Then
                        Dim ruolo As New RuoloPersonaType1
                        ruolo.codRuolo = persona.ruolo.codRuolo
                        ruolo.dtFineRapporto = persona.ruolo.dtFineRapporto
                        ruolo.dtInizioRapporto = persona.ruolo.dtInizioRapporto
                        ruolo.dtVariazioneRuolo = persona.ruolo.dtVariazioneRuolo
                        ruolo.fonte = persona.ruolo.fonte
                        ruolo.fonteDescr = persona.ruolo.fonteDescr
                        ruolo.idAzienda = persona.ruolo.idAzienda
                        ruolo.idPersona = persona.ruolo.idPersona
                        ruolo.ruoloDescr = persona.ruolo.ruoloDescr
                        persona.ruolo = ruolo
                    End If

                    persone.Add(ISWSPersona)
                    i += 1
                Next
            End If
            personeRuoli.persone = persone.ToArray
            destination.personeRuoli = personeRuoli
        End If
    End Sub

    Public Sub convertProduzioneQualita(ByRef source As Sincro_Agrea2Gias.MyWsAgriRER.FascicoloSiar2Response, _
                                        ByRef destination As ISWSToOprResponse)
        If source.produzioniQualita IsNot Nothing Then
            Dim produzioniQualita As New ProduzioniQualitaType
            Dim produzione As New List(Of ProduzioneQualitaType)
            produzioniQualita.produzioneLength = source.produzioniQualita.produzioneLength
            If source.produzioniQualita.produzioneLength > 0 Then
                Dim i As Integer = 0
                For Each produzioneQualita In source.produzioniQualita.produzione
                    Dim ISWSProdQual As New ProduzioneQualitaType

                    ISWSProdQual.codMacroarea = produzioneQualita.codMacroarea
                    ISWSProdQual.codProduzione = produzioneQualita.codProduzione
                    ISWSProdQual.descrMacroArea = produzioneQualita.descrMacroArea
                    ISWSProdQual.descrProduzione = produzioneQualita.descrProduzione

                    produzione.Add(ISWSProdQual)
                    i += 1
                Next
            End If
            produzioniQualita.produzione = produzione.ToArray
            destination.produzioniQualita = produzioniQualita
        End If
    End Sub

    Public Sub convertReferenteAziendale(ByRef source As Sincro_Agrea2Gias.MyWsAgriRER.FascicoloSiar2Response, _
                                         ByRef destination As ISWSToOprResponse)
        If source.referenteAziendale IsNot Nothing Then
            Dim DettaglioSoggettoWS As New DettaglioSoggettoWS
            'DatiGenerali
            Dim SoggettoWS As New SoggettoWS
            SoggettoWS.CUAA = source.referenteAziendale.persona.codiceFiscale
            SoggettoWS.Flag_pers_fisi = 1
            SoggettoWS.Desc_cogn = source.referenteAziendale.persona.cognome
            DettaglioSoggettoWS.documento = source.referenteAziendale.persona.documento
            DettaglioSoggettoWS.dtDocumento = source.referenteAziendale.persona.dtDocumento
            DettaglioSoggettoWS.dtFonte = source.referenteAziendale.persona.dtFonte
            DettaglioSoggettoWS.dtVariazione = source.referenteAziendale.persona.dtVariazione
            DettaglioSoggettoWS.flagReferente = source.referenteAziendale.persona.flagReferente
            DettaglioSoggettoWS.fonte = source.referenteAziendale.persona.fonte
            DettaglioSoggettoWS.fonteDescr = source.referenteAziendale.persona.fonteDescr
            DettaglioSoggettoWS.idPersona = source.referenteAziendale.persona.idPersona
            SoggettoWS.Desc_nome = source.referenteAziendale.persona.nome
            SoggettoWS.Codi_sess = source.referenteAziendale.persona.sesso

            'Contatti
            If source.referenteAziendale.persona.contatti IsNot Nothing Then
                Dim contatti As New ContattiType
                contatti.email = source.referenteAziendale.persona.contatti.email
                contatti.fax = source.referenteAziendale.persona.contatti.fax
                contatti.pec = source.referenteAziendale.persona.contatti.pec
                contatti.telefono = source.referenteAziendale.persona.contatti.telefono
                SoggettoWS.contatti = contatti
            End If



            'Dati Nascita
            If source.referenteAziendale.persona.datiNascita IsNot Nothing Then
                If source.referenteAziendale.persona.datiNascita.cittaEsteraNascita IsNot Nothing Then
                    Dim cittaEsteraNascita As New CittaEsteraType
                    cittaEsteraNascita.citta = source.referenteAziendale.persona.datiNascita.cittaEsteraNascita.citta
                    cittaEsteraNascita.codStato = source.referenteAziendale.persona.datiNascita.cittaEsteraNascita.codStato
                    cittaEsteraNascita.statoDescr = source.referenteAziendale.persona.datiNascita.cittaEsteraNascita.statoDescr
                    SoggettoWS.cittaEsteraNascita = cittaEsteraNascita
                End If
                SoggettoWS.codCom = source.referenteAziendale.persona.datiNascita.codCom
                SoggettoWS.codProv = source.referenteAziendale.persona.datiNascita.codProv
                SoggettoWS.Desc_comu_nasc = source.referenteAziendale.persona.datiNascita.comDescr
                SoggettoWS.Data_nasc = source.referenteAziendale.persona.datiNascita.dataNascita
                SoggettoWS.Desc_prov_nasc = source.referenteAziendale.persona.datiNascita.provDescr
                SoggettoWS.Codi_sigl_prov_nasc = source.referenteAziendale.persona.datiNascita.provSigla
            End If


            'Residenza
            If source.referenteAziendale.persona.residenza IsNot Nothing Then
                Dim residenza As New IndirizzoType
                residenza.cap = source.referenteAziendale.persona.residenza.cap
                If source.referenteAziendale.persona.residenza.cittaEstera IsNot Nothing Then
                    Dim cittaEstera As New CittaEsteraType
                    cittaEstera.citta = source.referenteAziendale.persona.residenza.cittaEstera.citta
                    cittaEstera.codStato = source.referenteAziendale.persona.residenza.cittaEstera.codStato
                    cittaEstera.statoDescr = source.referenteAziendale.persona.residenza.cittaEstera.statoDescr
                End If
                residenza.codCom = source.referenteAziendale.persona.residenza.codCom
                residenza.codProv = source.referenteAziendale.persona.residenza.codProv
                residenza.comDescr = source.referenteAziendale.persona.residenza.comDescr
                residenza.indirizzo = source.referenteAziendale.persona.residenza.indirizzo
                residenza.localita = source.referenteAziendale.persona.residenza.localita
                residenza.provDescr = source.referenteAziendale.persona.residenza.provDescr
                residenza.provSigla = source.referenteAziendale.persona.residenza.provSigla
                SoggettoWS.residenza = residenza
            End If

            'Ruolo
            If source.referenteAziendale.persona.ruolo IsNot Nothing Then
                Dim ruolo As New RuoloPersonaType
                ruolo.codRuolo = source.referenteAziendale.persona.ruolo.codRuolo
                ruolo.dtFineRapporto = source.referenteAziendale.persona.ruolo.dtFineRapporto
                ruolo.dtInizioRapporto = source.referenteAziendale.persona.ruolo.dtInizioRapporto
                ruolo.dtVariazioneRuolo = source.referenteAziendale.persona.ruolo.dtVariazioneRuolo
                ruolo.fonte = source.referenteAziendale.persona.ruolo.fonte
                ruolo.fonteDescr = source.referenteAziendale.persona.ruolo.fonteDescr
                ruolo.idAzienda = source.referenteAziendale.persona.ruolo.idAzienda
                ruolo.idPersona = source.referenteAziendale.persona.ruolo.idPersona
                ruolo.ruoloDescr = source.referenteAziendale.persona.ruolo.ruoloDescr
                DettaglioSoggettoWS.ruolo = ruolo
            End If

            DettaglioSoggettoWS.SoggettoWS = SoggettoWS
            destination.DettaglioSoggettoWS = DettaglioSoggettoWS
        End If
    End Sub

    Public Sub convertResponsabileFito(ByRef source As Sincro_Agrea2Gias.MyWsAgriRER.FascicoloSiar2Response, _
                                       ByRef destination As ISWSToOprResponse)
        If source.responsabileFito IsNot Nothing Then
            Dim responsabileFito As New ResponsabileFitoType
            responsabileFito.alboProfessionale = source.responsabileFito.alboProfessionale
            responsabileFito.alboProfessionaleDescr = source.responsabileFito.alboProfessionaleDescr
            responsabileFito.capDomicilio = source.responsabileFito.capDomicilio
            responsabileFito.cittaEsteraDomicilio = source.responsabileFito.cittaEsteraDomicilio
            responsabileFito.cittaEsteraNascita = source.responsabileFito.cittaEsteraNascita
            responsabileFito.codFiscale = source.responsabileFito.codFiscale
            responsabileFito.cognome = source.responsabileFito.cognome
            responsabileFito.comuneDomicilio = source.responsabileFito.comuneDomicilio
            responsabileFito.comuneDomicilioDescr = source.responsabileFito.comuneDomicilioDescr
            responsabileFito.comuneNascita = source.responsabileFito.comuneNascita
            responsabileFito.comuneNascitaDescr = source.responsabileFito.comuneNascitaDescr
            responsabileFito.dataInizioRapporto = source.responsabileFito.dataInizioRapporto
            responsabileFito.dataNascita = source.responsabileFito.dataNascita
            responsabileFito.emailDomicilio = source.responsabileFito.emailDomicilio
            responsabileFito.faxDomicilio = source.responsabileFito.faxDomicilio
            responsabileFito.indirizzoDomicilio = source.responsabileFito.indirizzoDomicilio
            responsabileFito.localitaDomicilio = source.responsabileFito.localitaDomicilio
            responsabileFito.nome = source.responsabileFito.nome
            responsabileFito.numeroIscizione = source.responsabileFito.numeroIscizione
            responsabileFito.provinciaDomicilio = source.responsabileFito.provinciaDomicilio
            responsabileFito.provinciaDomicilioDescr = source.responsabileFito.provinciaDomicilioDescr
            responsabileFito.provinciaDomicilioSigla = source.responsabileFito.provinciaDomicilioSigla
            responsabileFito.provinciaNascita = source.responsabileFito.provinciaNascita
            responsabileFito.provinciaNascitaDescr = source.responsabileFito.provinciaNascitaDescr
            responsabileFito.provinciaNascitaSigla = source.responsabileFito.provinciaNascitaSigla
            responsabileFito.qualifica = source.responsabileFito.qualifica
            responsabileFito.qualificaDescr = source.responsabileFito.qualificaDescr
            responsabileFito.ragioneSociale = source.responsabileFito.ragioneSociale
            responsabileFito.sesso = source.responsabileFito.sesso
            responsabileFito.statoEsteroDomicilio = source.responsabileFito.statoEsteroDomicilio
            responsabileFito.statoEsteroNascita = source.responsabileFito.statoEsteroNascita
            responsabileFito.telefonoDomicilio = source.responsabileFito.telefonoDomicilio
            responsabileFito.titoloStudio = source.responsabileFito.titoloStudio
            responsabileFito.titoloStudioDescr = source.responsabileFito.titoloStudioDescr
            destination.responsabileFito = responsabileFito
        End If

    End Sub

    Public Sub convertStatoAzienda(ByRef source As Sincro_Agrea2Gias.MyWsAgriRER.FascicoloSiar2Response, _
                                   ByRef destination As ISWSToOprResponse)
        Dim statoAzienda As New StatoAziendaType
        If source.statoAzienda IsNot Nothing Then
            statoAzienda.aziendaCessata = source.statoAzienda.aziendaCessata
            statoAzienda.aziendaIscrittaCaa = source.statoAzienda.aziendaIscrittaCaa
            statoAzienda.aziendaPresente = source.statoAzienda.aziendaPresente
            statoAzienda.aziendaValidata = source.statoAzienda.aziendaValidata
            statoAzienda.dataCessazione = source.statoAzienda.dataCessazione
            statoAzienda.dataIscrizioneCaa = source.statoAzienda.dataIscrizioneCaa
            statoAzienda.dataValidazione = source.statoAzienda.dataValidazione
            statoAzienda.dataVariazioneAzienda = source.statoAzienda.dataVariazioneAzienda
            statoAzienda.maxDataVariazioneIbanAzienda = source.statoAzienda.maxDataVariazioneIbanAzienda
            statoAzienda.maxDataVariazionePersoneAzienda = source.statoAzienda.maxDataVariazionePersoneAzienda
            statoAzienda.maxDataVariazionePossessiAzienda = source.statoAzienda.maxDataVariazionePossessiAzienda
        End If
        destination.statoAzienda = statoAzienda
    End Sub

    Public Sub convertUnitaAziendali(ByRef source As Sincro_Agrea2Gias.MyWsAgriRER.FascicoloSiar2Response, _
                                     ByRef destination As ISWSToOprResponse)
        If source.unitaAziendali IsNot Nothing Then
            Dim unitaAziendali As New UnitaAziendaliType
            unitaAziendali.unitaLength = source.unitaAziendali.unitaLength
            Dim unitaAziendale As New List(Of UnitaAziendaleType)
            If source.unitaAziendali.unitaLength > 0 Then
                Dim i = 0
                For Each unita In source.unitaAziendali.unita
                    Dim ISWSUnitaAziendale As New UnitaAziendaleType

                    ISWSUnitaAziendale.dataDocumento = unita.dataDocumento
                    ISWSUnitaAziendale.dataFine = unita.dataFine
                    ISWSUnitaAziendale.dataFonte = unita.dataFonte
                    ISWSUnitaAziendale.dataInizio = unita.dataInizio
                    ISWSUnitaAziendale.dataVariazione = unita.dataVariazione
                    ISWSUnitaAziendale.documento = unita.documento
                    ISWSUnitaAziendale.flagLegale = unita.flagLegale
                    ISWSUnitaAziendale.flagPrincipale = unita.flagPrincipale
                    ISWSUnitaAziendale.fonte = unita.fonte
                    ISWSUnitaAziendale.fonteDescr = unita.fonteDescr
                    ISWSUnitaAziendale.idAzienda = unita.idAzienda
                    ISWSUnitaAziendale.numRea = unita.numRea
                    ISWSUnitaAziendale.progr = unita.progr
                    ISWSUnitaAziendale.provRea = unita.provRea

                    'Contatti
                    If unita.contatti IsNot Nothing Then
                        Dim contatti As New ContattiType
                        contatti.email = unita.contatti.email
                        contatti.fax = unita.contatti.fax
                        contatti.pec = unita.contatti.pec
                        contatti.telefono = unita.contatti.telefono
                        ISWSUnitaAziendale.contatti = contatti
                    End If

                    'Sede
                    If unita.sede IsNot Nothing Then
                        Dim sede As New IndirizzoType
                        sede.cap = unita.sede.cap
                        If unita.sede.cittaEstera IsNot Nothing Then
                            Dim cittaEstera As New CittaEsteraType
                            cittaEstera.citta = unita.sede.cittaEstera.citta
                            cittaEstera.codStato = unita.sede.cittaEstera.codStato
                            cittaEstera.statoDescr = unita.sede.cittaEstera.statoDescr
                            sede.cittaEstera = cittaEstera
                        End If
                        sede.codCom = unita.sede.codCom
                        sede.codProv = unita.sede.codProv
                        sede.comDescr = unita.sede.comDescr
                        sede.indirizzo = unita.sede.indirizzo
                        sede.localita = unita.sede.localita
                        sede.provDescr = unita.sede.provDescr
                        sede.provSigla = unita.sede.provSigla
                        ISWSUnitaAziendale.sede = sede
                    End If


                    'SedeEstera
                    If unita.sedeEstera IsNot Nothing Then
                        Dim sedeEstera As New CittaEsteraType1
                        sedeEstera.citta = unita.sedeEstera.citta
                        sedeEstera.codStato = unita.sedeEstera.codStato
                        sedeEstera.statoDescr = unita.sedeEstera.statoDescr
                        unita.sedeEstera = sedeEstera
                    End If

                    unitaAziendale.Add(ISWSUnitaAziendale)
                    i += 1
                Next
            End If
            unitaAziendali.unita = unitaAziendale.ToArray
            destination.unitaAziendali = unitaAziendali
        End If
    End Sub

    Public Sub convertMessaggio(ByRef source As Sincro_Agrea2Gias.MyWsAgriRER.FascicoloSiar2Response, _
                                ByRef destination As ISWSToOprResponse)
        If source.messaggioRisposta IsNot Nothing Then
            Dim messaggioRisposta As New MessaggioRispostaType
            messaggioRisposta.cod = source.messaggioRisposta.cod
            messaggioRisposta.messaggio = source.messaggioRisposta.messaggio
            destination.messaggioRisposta = messaggioRisposta
        End If
    End Sub
End Class
