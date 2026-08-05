Imports AgronicaCoreEntityFramework_POCO

Public Class G2G_Recode

    Public Property LogRecode As String
    Public Property MessaggioErrore As String

    'Imprese / Centri Aziendali
    Public Property G2GRecodeImpreseCentriToInsert As List(Of G2G_Recode_Imprese)
    Public Property G2GRecodeImpreseCentriToUpdate As List(Of G2G_Recode_Imprese)
    Public Property G2GRecodeImpreseCentriToDelete As List(Of G2G_Recode_Imprese)

    'Contatti
    Public Property G2GRecodeContattiToInsert As List(Of G2G_Recode_Contatti)
    Public Property G2GRecodeContattiToUpdate As List(Of G2G_Recode_Contatti)
    Public Property G2GRecodeContattiToDelete As List(Of G2G_Recode_Contatti)

    'Pratiche
    Public Property G2GRecodePraticheToInsert As List(Of G2G_Recode_Pratiche)
    Public Property G2GRecodePraticheToUpdate As List(Of G2G_Recode_Pratiche)
    Public Property G2GRecodePraticheToDelete As List(Of G2G_Recode_Pratiche)

    Public Property G2GRecodePraticheStatiToInsert As List(Of G2G_Recode_Pratiche_Stati)
    Public Property G2GRecodePraticheStatiToUpdate As List(Of G2G_Recode_Pratiche_Stati)
    Public Property G2GRecodePraticheStatiToDelete As List(Of G2G_Recode_Pratiche_Stati)

    'Indirizzi
    Public Property G2GRecodeIndirizziToInsert As List(Of G2G_Recode_Indirizzi)
    Public Property G2GRecodeIndirizziToUpdate As List(Of G2G_Recode_Indirizzi)
    Public Property G2GRecodeIndirizziToDelete As List(Of G2G_Recode_Indirizzi)

    'Macchine
    Public Property G2GRecodeParcoMacchineToInsert As List(Of G2G_Recode_Parco_Macchine)
    Public Property G2GRecodeParcoMacchineToUpdate As List(Of G2G_Recode_Parco_Macchine)
    Public Property G2GRecodeParcoMacchineToDelete As List(Of G2G_Recode_Parco_Macchine)

    'Fabbricati
    Public Property G2GRecodeFabbricatiToInsert As List(Of G2G_Recode_Fabbricati)
    Public Property G2GRecodeFabbricatiToUpdate As List(Of G2G_Recode_Fabbricati)
    Public Property G2GRecodeFabbricatiToDelete As List(Of G2G_Recode_Fabbricati)

    'PUA
    Public Property G2GRecodePuaToInsert As List(Of G2G_Recode_Pua)
    Public Property G2GRecodePuaToUpdate As List(Of G2G_Recode_Pua)
    Public Property G2GRecodePuaToDelete As List(Of G2G_Recode_Pua)

    'PUA Effluente
    Public Property G2GRecodePua_EffluenteToInsert As List(Of G2G_Recode_PUA_Effluente)
    Public Property G2GRecodePua_EffluenteToUpdate As List(Of G2G_Recode_PUA_Effluente)
    Public Property G2GRecodePua_EffluenteToDelete As List(Of G2G_Recode_PUA_Effluente)

    'Planning
    Public Property G2GRecodeProgrammazioneTestataToInsert As List(Of G2G_Recode_Programmazione_Testata)
    Public Property G2GRecodeProgrammazioneTestataToUpdate As List(Of G2G_Recode_Programmazione_Testata)
    Public Property G2GRecodeProgrammazioneTestataToDelete As List(Of G2G_Recode_Programmazione_Testata)

    Public Property G2GRecodeProgrammazioneEntitaToInsert As List(Of G2G_Recode_Programmazione_Entita)
    Public Property G2GRecodeProgrammazioneEntitaToUpdate As List(Of G2G_Recode_Programmazione_Entita)
    Public Property G2GRecodeProgrammazioneEntitaToDelete As List(Of G2G_Recode_Programmazione_Entita)

    'Allegati
    Public Property G2GRecodeAllegatiToInsert As List(Of G2G_Recode_Allegati)
    Public Property G2GRecodeAllegatiToUpdate As List(Of G2G_Recode_Allegati)
    Public Property G2GRecodeAllegatiToDelete As List(Of G2G_Recode_Allegati)

    Public Property G2GRecodeAllegati_EntitaToInsert As List(Of G2G_Recode_Allegati_Entita)
    Public Property G2GRecodeAllegati_EntitaToUpdate As List(Of G2G_Recode_Allegati_Entita)
    Public Property G2GRecodeAllegati_EntitaToDelete As List(Of G2G_Recode_Allegati_Entita)

    'Analisi
    Public Property G2GRecodeAnalisi_CertificatoToInsert As List(Of G2G_Recode_Analisi_Certificato)
    Public Property G2GRecodeAnalisi_CertificatoToUpdate As List(Of G2G_Recode_Analisi_Certificato)
    Public Property G2GRecodeAnalisi_CertificatoToDelete As List(Of G2G_Recode_Analisi_Certificato)

    Public Property G2GRecodeAnalisi_TestataToInsert As List(Of G2G_Recode_Analisi_Testata)
    Public Property G2GRecodeAnalisi_TestataToUpdate As List(Of G2G_Recode_Analisi_Testata)
    Public Property G2GRecodeAnalisi_TestataToDelete As List(Of G2G_Recode_Analisi_Testata)

    Public Property G2GRecodeAnalisi_DettagliToInsert As List(Of G2G_Recode_Analisi_Dettagli)
    Public Property G2GRecodeAnalisi_DettagliToUpdate As List(Of G2G_Recode_Analisi_Dettagli)
    Public Property G2GRecodeAnalisi_DettagliToDelete As List(Of G2G_Recode_Analisi_Dettagli)

    Public Property G2GRecodeAnalisi_EntitaxTestataToInsert As List(Of G2G_Recode_Analisi_EntitaxTestata)
    Public Property G2GRecodeAnalisi_EntitaxTestataToUpdate As List(Of G2G_Recode_Analisi_EntitaxTestata)
    Public Property G2GRecodeAnalisi_EntitaxTestataToDelete As List(Of G2G_Recode_Analisi_EntitaxTestata)

    Public Property G2GRecodeAnalisi_CampioniToInsert As List(Of G2G_Recode_Analisi_Campioni)
    Public Property G2GRecodeAnalisi_CampioniToUpdate As List(Of G2G_Recode_Analisi_Campioni)
    Public Property G2GRecodeAnalisi_CampioniToDelete As List(Of G2G_Recode_Analisi_Campioni)



    'Piani di Concimazione
    Public Property G2GRecodePianoConcimazione_TestataToInsert As List(Of G2G_Recode_PianoConcimazione_Testata)
    Public Property G2GRecodePianoConcimazione_TestataToUpdate As List(Of G2G_Recode_PianoConcimazione_Testata)
    Public Property G2GRecodePianoConcimazione_TestataToDelete As List(Of G2G_Recode_PianoConcimazione_Testata)

    Public Property G2GRecodePianoConcimazione_DettagliToInsert As List(Of G2G_Recode_PianoConcimazione_Dettagli)
    Public Property G2GRecodePianoConcimazione_DettagliToUpdate As List(Of G2G_Recode_PianoConcimazione_Dettagli)
    Public Property G2GRecodePianoConcimazione_DettagliToDelete As List(Of G2G_Recode_PianoConcimazione_Dettagli)

    Public Property G2GRecodePianoConcimazione_EntitaxTestataToInsert As List(Of G2G_Recode_PianoConcimazione_EntitaxTestata)
    Public Property G2GRecodePianoConcimazione_EntitaxTestataToUpdate As List(Of G2G_Recode_PianoConcimazione_EntitaxTestata)
    Public Property G2GRecodePianoConcimazione_EntitaxTestataToDelete As List(Of G2G_Recode_PianoConcimazione_EntitaxTestata)

    Public Property G2GRecodePianoConcimazione_ElaborazioniToInsert As List(Of G2G_Recode_PianoConcimazione_Elaborazioni)
    Public Property G2GRecodePianoConcimazione_ElaborazioniToUpdate As List(Of G2G_Recode_PianoConcimazione_Elaborazioni)
    Public Property G2GRecodePianoConcimazione_ElaborazioniToDelete As List(Of G2G_Recode_PianoConcimazione_Elaborazioni)

    'Ricette
    Public Property G2GRecodeRicetteToInsert As List(Of G2G_Recode_Ricette)
    Public Property G2GRecodeRicetteToUpdate As List(Of G2G_Recode_Ricette)
    Public Property G2GRecodeRicetteToDelete As List(Of G2G_Recode_Ricette)

    Public Property G2GRecodeRicette_OperazioniToInsert As List(Of G2G_Recode_Ricette_Operazioni)
    Public Property G2GRecodeRicette_OperazioniToUpdate As List(Of G2G_Recode_Ricette_Operazioni)
    Public Property G2GRecodeRicette_OperazioniToDelete As List(Of G2G_Recode_Ricette_Operazioni)

    Public Property G2GRecodeRicette_DettagliToInsert As List(Of G2G_Recode_Ricette_Dettagli)
    Public Property G2GRecodeRicette_DettagliToUpdate As List(Of G2G_Recode_Ricette_Dettagli)
    Public Property G2GRecodeRicette_DettagliToDelete As List(Of G2G_Recode_Ricette_Dettagli)

    Public Property G2GRecodeRicette_Dettaglio_TecnicoToInsert As List(Of G2G_Recode_Ricette_Dettaglio_Tecnico)
    Public Property G2GRecodeRicette_Dettaglio_TecnicoToUpdate As List(Of G2G_Recode_Ricette_Dettaglio_Tecnico)
    Public Property G2GRecodeRicette_Dettaglio_TecnicoToDelete As List(Of G2G_Recode_Ricette_Dettaglio_Tecnico)

    Public Property G2GRecodeRicette_DestinazioniToInsert As List(Of G2G_Recode_Ricette_Destinazioni)
    Public Property G2GRecodeRicette_DestinazioniToUpdate As List(Of G2G_Recode_Ricette_Destinazioni)
    Public Property G2GRecodeRicette_DestinazioniToDelete As List(Of G2G_Recode_Ricette_Destinazioni)

    'ParticelleCatastalixVincoliAgronomici
    Public Property G2GRecodeParticelleCatastalixVincoliAgronomiciToInsert As List(Of G2G_Recode_ParticelleCatastalixVincoliAgronomici)
    Public Property G2GRecodeParticelleCatastalixVincoliAgronomiciToUpdate As List(Of G2G_Recode_ParticelleCatastalixVincoliAgronomici)
    Public Property G2GRecodeParticelleCatastalixVincoliAgronomiciToDelete As List(Of G2G_Recode_ParticelleCatastalixVincoliAgronomici)

    'MateriePrimeCampionature
    Public Property G2GRecodeMateriePrimeCampionatureToInsert As List(Of G2G_Recode_Materie_Prime_Campionature)
    Public Property G2GRecodeMateriePrimeCampionatureToUpdate As List(Of G2G_Recode_Materie_Prime_Campionature)
    Public Property G2GRecodeMateriePrimeCampionatureToDelete As List(Of G2G_Recode_Materie_Prime_Campionature)

    'MateriePrime
    Public Property G2GRecodeMateriePrimeToInsert As List(Of G2G_Recode_MateriePrime)
    Public Property G2GRecodeMateriePrimeToUpdate As List(Of G2G_Recode_MateriePrime)
    Public Property G2GRecodeMateriePrimeToDelete As List(Of G2G_Recode_MateriePrime)

    'Piano Colturale
    Public Property G2GRecodeCampiToInsert As List(Of G2G_Recode_Campo)
    Public Property G2GRecodeCampiToUpdate As List(Of G2G_Recode_Campo)
    Public Property G2GRecodeCampiToDelete As List(Of G2G_Recode_Campo)
    Public Property G2GRecodeAppezzamentiToInsert As List(Of G2G_Recode_Appezzamenti)
    Public Property G2GRecodeAppezzamentiToUpdate As List(Of G2G_Recode_Appezzamenti)
    Public Property G2GRecodeAppezzamentiToDelete As List(Of G2G_Recode_Appezzamenti)
    Public Property G2GRecodeImpiantiToInsert As List(Of G2G_Recode_Impianti)
    Public Property G2GRecodeImpiantiToUpdate As List(Of G2G_Recode_Impianti)
    Public Property G2GRecodeImpiantiToDelete As List(Of G2G_Recode_Impianti)
    Public Property G2GRecodeDistinteToInsert As List(Of G2G_Recode_Distinta)
    Public Property G2GRecodeDistinteToUpdate As List(Of G2G_Recode_Distinta)
    Public Property G2GRecodeDistinteToDelete As List(Of G2G_Recode_Distinta)

    'Utenti
    Public Property G2GRecodeUtentiToInsert As List(Of G2G_Recode_Utenti)
    Public Property G2GRecodeUtentiToUpdate As List(Of G2G_Recode_Utenti)
    Public Property G2GRecodeUtentiToDelete As List(Of G2G_Recode_Utenti)
    Public Property G2GRecodeGruppi_UtenteToInsert As List(Of G2G_Recode_Gruppi_Utente)
    Public Property G2GRecodeGruppi_UtenteToUpdate As List(Of G2G_Recode_Gruppi_Utente)
    Public Property G2GRecodeGruppi_UtenteToDelete As List(Of G2G_Recode_Gruppi_Utente)

    'PUA_LetamazioniPrecedenti
    Public Property G2GRecodePUA_LetamazioniPrecedentiToInsert As List(Of G2G_Recode_PUA_LetamazioniPrecedenti)
    Public Property G2GRecodePUA_LetamazioniPrecedentiToUpdate As List(Of G2G_Recode_PUA_LetamazioniPrecedenti)
    Public Property G2GRecodePUA_LetamazioniPrecedentiToDelete As List(Of G2G_Recode_PUA_LetamazioniPrecedenti)

    'Anagrafe_VincoliAgronomici
    Public Property G2GRecodeAnagrafe_VincoliAgronomiciToInsert As List(Of G2G_Recode_Anagrafe_VincoliAgronomici)
    Public Property G2GRecodeAnagrafe_VincoliAgronomiciToUpdate As List(Of G2G_Recode_Anagrafe_VincoliAgronomici)
    Public Property G2GRecodeAnagrafe_VincoliAgronomiciToDelete As List(Of G2G_Recode_Anagrafe_VincoliAgronomici)

    'Attivita
    Public Property G2GRecodeAttivitaToInsert As List(Of G2G_Recode_Attivita)
    Public Property G2GRecodeAttivitaToUpdate As List(Of G2G_Recode_Attivita)
    Public Property G2GRecodeAttivitaToDelete As List(Of G2G_Recode_Attivita)

    'PUA_Effluente_PeriodoDivieto
    Public Property G2GRecodePUA_Effluente_PeriodoDivietoToInsert As List(Of G2G_Recode_PUA_PeriodoDivieto)
    Public Property G2GRecodePUA_Effluente_PeriodoDivietoToUpdate As List(Of G2G_Recode_PUA_PeriodoDivieto)
    Public Property G2GRecodePUA_Effluente_PeriodoDivietoToDelete As List(Of G2G_Recode_PUA_PeriodoDivieto)

End Class