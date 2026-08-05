Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreMVVCommon
Imports AgronicaCoreMVVDal

Public Class StampaController

    Private ReadOnly _objParametriServer As AgronicaCoreParametri
    Private ReadOnly _objParametriUtente As AgronicaCoreParametri
    Private ReadOnly _dal As MVVStampaDAL

    Public Sub New(
                  ByVal objParametriServer As AgronicaCoreParametri,
                  ByVal objParametriUtente As AgronicaCoreParametri
        )
        _objParametriServer = objParametriServer
        _objParametriUtente = objParametriUtente
        _dal = New MVVStampaDAL(_objParametriServer, objParametriUtente)
    End Sub

    Public Function PreparaDatiStampa(ByVal Piva As String, ByVal id_Agenda As Integer) As MVV

        Dim dati = _dal.PreparaDatiStampa(Piva, id_Agenda)

        Dim dettagliRegVino = DirectCast(dati.DettagliTeleregistri, List(Of TeleRegistri_Dettagli))
        Dim mvv = DirectCast(dati.DatiMVV, DataTable).ToMVV()
        Dim statiMembri = DirectCast(dati.StatiMembri, List(Of ACCDAA_ANAG_T004_TabellaCodiciStatiMembri))
        Dim codificheVini = DirectCast(dati.CodificheVino, List(Of AGEACodificaVini))
        Dim moduloGenerazione = DirectCast(dati.ModuloGenerazione, enum_Omni_Modulo_Generazione)
        Dim movimentiDettagli = DirectCast(dati.MovimentiDettagli, List(Of Movimenti_dettagli)).OrderBy(Function(m) m.Id_Mov_Det).ToList()

        Dim dictionaryAttributiRegVino = dettagliRegVino.ToDictionary()

        For Each d As MVV_Dettaglio In mvv.Dettagli
            Dim rv = d.RegistroVino
            rv.Annata.Descrizione = rv.Annata.Codice
            rv.CodStatoFisico.Descrizione = dictionaryAttributiRegVino.ValoreDizionario("statofisico", rv.CodStatoFisico.Codice)
            rv.CodColore.Descrizione = dictionaryAttributiRegVino.ValoreDizionario("colore", rv.CodColore.Codice)
            rv.AttoCert.Descrizione = dictionaryAttributiRegVino.ValoreDizionario("certificato", rv.AttoCert.Codice)
            rv.Biologico.Descrizione = dictionaryAttributiRegVino.ValoreDizionario("biologico", rv.Biologico.Codice)

            Dim codiceZonaViticola = If(rv.CodStatoFisico.Codice = 1, rv.CodZonaViticola.Codice, d.Extra_Zona_Viticola)
            rv.CodZonaViticola.Descrizione = dictionaryAttributiRegVino.ValoreDizionario("zonaviticola", codiceZonaViticola)

            rv.CodOperazioneVit.Descrizione = dictionaryAttributiRegVino.ValoreDizionario("praticaenologica", rv.CodOperazioneVit.Codice)

            Dim origineUveCod = statiMembri.FirstOrDefault(Function(s) s.Codice_Numerico.Equals(rv.OrigineUve.Codice))
            If Not origineUveCod Is Nothing Then
                rv.OrigineUve.Descrizione = origineUveCod.Descrizione
            End If

            Dim codDopIgp = codificheVini.FirstOrDefault(Function(v) v.CodiceIntero.Equals(rv.DocIGP.Codice))
            If Not codDopIgp Is Nothing Then
                rv.DocIGP.Descrizione = codDopIgp.Vino_des
            End If

            Dim provenienzaCod = dictionaryAttributiRegVino.ValoreDizionario("provenienza", rv.Provenienza.Codice)
            Dim provenienza = statiMembri.FirstOrDefault(Function(s) s.Codice_Numerico.Equals(provenienzaCod))
            If Not provenienza Is Nothing Then
                rv.Provenienza.Descrizione = provenienza.Descrizione
            Else
                rv.Provenienza.Descrizione = provenienzaCod
            End If

            Dim varieta = rv.Varieta.Codice.Split("|").Where(Function(v) Not String.IsNullOrEmpty(v))
            For Each v As String In varieta
                rv.Varieta.Descrizione = rv.Varieta.Descrizione + dictionaryAttributiRegVino.ValoreDizionario("varieta", v) + " - "
            Next

            'd.CodCategoria = rv.CodCategoria.Codice
            d.CodCategoria = d.CategoriaVinoCod
            d.CodiceOperazioneVit = rv.CodOperazioneVit.Codice
            d.CodiceZonaVit = rv.CodZonaViticola.Descrizione

            'imposto flag_extra
            Dim movDet = movimentiDettagli.FirstOrDefault(Function(m) m.Id_Mov_Det.Equals(d.ID_Mov_Det))
            Dim flag_extra As Boolean = False
            If Not movDet Is Nothing Then
                _dal.LeggiDescrizioneBeneServizio(movDet, flag_extra, moduloGenerazione)
                d.Flag_Extra = flag_extra
            End If

            If d.Flag_Extra = True Then
                d.UnitaMisura = IIf(d.UDMQTA.UDM_COD_EXTRA = 0, d.UDMQTA.UDM_SIM, d.UDMQTA.UDM_SIM_EXTRA)
                d.Quantita = d.UDMQTA.QTA_EXtra_Tot
            Else
                d.UnitaMisura = d.UDMQTA.UDM_SIM
                d.Quantita = d.UDMQTA.QTA
            End If

            'imposta il flag da indicare alla stampa se stampare colli o imballi
            If d.NumeroColli <> 0 AndAlso d.DescrizioneColli <> String.Empty Then
                d.UsaColli = True
            End If

        Next

        Dim nomeCognome = String.Concat(mvv.Destinatario.Nome, " ", mvv.Destinatario.Cognome).Trim()
        If mvv.Destinatario.ID_CF = enum_Contatti_IdCf.PersonaFisica Then
            If Not String.IsNullOrEmpty(mvv.Destinatario.Cognome) OrElse Not String.IsNullOrEmpty(mvv.Destinatario.Nome) Then
                mvv.Destinatario.Denominazione = nomeCognome
            End If
        Else
            If String.IsNullOrEmpty(mvv.Destinatario.Denominazione) Then
                mvv.Destinatario.Denominazione = nomeCognome
            End If
        End If

        Return mvv


    End Function

End Class
