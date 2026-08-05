Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreMVVBIZ.Integrazione.SIAN.MVV
Imports AgronicaCoreMVVCommon
Imports AgronicaCoreMVVDal
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class MVV_XmlGenerator

    Private ReadOnly _objParametriServer As AgronicaCoreParametri
    Private ReadOnly _objParametriUtente As AgronicaCoreParametri
    Private ReadOnly _stampaController As StampaController
    Private ReadOnly _MVVElettronico_R As MVVElettronico_R
    Private ReadOnly _decodificatore As TabelleDecoficaOnline
    Private ReadOnly _configMin As MVVServiceConfigMin
    Private ReadOnly _logger As MVVLogger
    Private ReadOnly _matriceCampi As MatriceCampi

    Public Sub New(
                  ByVal objParametriServer As AgronicaCoreParametri,
                  ByVal objParametriUtente As AgronicaCoreParametri,
                  ByVal configMin As MVVServiceConfigMin,
                  ByVal logger As MVVLogger
        )
        _objParametriServer = objParametriServer
        _objParametriUtente = objParametriUtente
        _configMin = configMin
        _MVVElettronico_R = New MVVElettronico_R(objParametriServer)
        _stampaController = New StampaController(_objParametriServer, _objParametriUtente)
        _decodificatore = New TabelleDecoficaOnline(_objParametriServer)
        _matriceCampi = New MatriceCampi
        _logger = logger

    End Sub
    Public Function GeneraMVV(ByVal codOper As CUAA,
                              ByVal PIVA As String,
                              ByVal idAgenda As Integer,
                              ByRef errori As List(Of String)) As MVVSiRPVInput

        Dim nomeProcedura As String = "MVV_XmlGenerator.GeneraMVV"
        Dim messaggioErrore As String = String.Empty
        Dim mvv = _stampaController.PreparaDatiStampa(PIVA, idAgenda)
        If mvv.HaLuogoConsegnaDiverso AndAlso mvv.CodIndirizzoConsegnaDiverso <> -1 Then
            mvv.IndirizzoConsegnaDiverso = _MVVElettronico_R.LeggiIndirizzoConsegnaDiverso(mvv.CodIndirizzoConsegnaDiverso)
        End If

        Dim dtSoggetti As DataTable = Nothing
        Dim listaSoggettiSian As IEnumerable(Of Object)
        Dim mvve = New MVVSiRPVInput

        errori.Clear()

        Try

            ' Check Preliminare sui titoli alcool


            ' per ogni soggetto recuper le info dall' anagrafica Sian se presente
            Dim listaCodiciSoggetto As New List(Of String) From
            {
                String.Format("{0}{1}{0}", "'", mvv.Speditore.CODICE_SOGGETTO),
                String.Format("{0}{1}{0}", "'", mvv.Destinatario.CODICE_SOGGETTO),
                String.Format("{0}{1}{0}", "'", mvv.DestinatarioDiverso.CODICE_SOGGETTO),
                String.Format("{0}{1}{0}", "'", mvv.Vettore.CODICE_SOGGETTO)
            }
            If Not mvv.Venditore Is Nothing Then
                listaCodiciSoggetto.Add(String.Format("{0}{1}{0}", "'", mvv.Venditore.CODICE_SOGGETTO))
            End If
            If Not mvv.Acquirente Is Nothing Then
                listaCodiciSoggetto.Add(String.Format("{0}{1}{0}", "'", mvv.Acquirente.CODICE_SOGGETTO))
            End If

            If _configMin.ModalitaInvioSoggetti = 1 Then
                dtSoggetti = _MVVElettronico_R.CercaSoggettiSian(listaCodiciSoggetto.Where(Function(f) f <> "''").Distinct.ToList)
                If Not dtSoggetti Is Nothing AndAlso dtSoggetti.Rows.Count > 0 Then
                    listaSoggettiSian = (From s In dtSoggetti.AsEnumerable() Select New With
                                                                     {
                                                                        .CodiceSoggetto = If(s.Item("CodiceSoggetto") Is DBNull.Value, "", s.Item("CodiceSoggetto")),
                                                                        .CodOper = If(s.Item("CodOper") Is DBNull.Value, "", s.Item("CodOper")),
                                                                        .CodOper_Fisiche = If(s.Item("CodOper_Fisiche") Is DBNull.Value, "", s.Item("CodOper_Fisiche")),
                                                                        .CUAA = If(s.Item("CUAA") Is DBNull.Value, "", s.Item("CUAA")),
                                                                        .TipoSoggetto = If(s.Item("TipoSoggetto") Is DBNull.Value, "", s.Item("TipoSoggetto")),
                                                                        .Nome = If(s.Item("Nome") Is DBNull.Value, "", s.Item("Nome")),
                                                                        .Cognome = If(s.Item("Cognome") Is DBNull.Value, "", s.Item("Cognome")),
                                                                        .Ragione_Sociale = If(s.Item("Ragione_Sociale") Is DBNull.Value, "", s.Item("Ragione_Sociale"))
                                                                     }).ToList()

                End If


                If Not ControllaSoggettoTelematizzato(mvv.Speditore.CODICE_SOGGETTO, mvv.Speditore, listaSoggettiSian, messaggioErrore) Then
                    errori.Add(messaggioErrore)
                End If
                If Not IsNothing(mvv.Destinatario) AndAlso Not String.IsNullOrEmpty(mvv.Destinatario.CODICE_SOGGETTO) Then
                    If Not ControllaSoggettoTelematizzato(mvv.Destinatario.CODICE_SOGGETTO, mvv.Destinatario, listaSoggettiSian, messaggioErrore) Then
                        errori.Add(messaggioErrore)
                    End If
                End If
                If Not IsNothing(mvv.DestinatarioDiverso) AndAlso Not String.IsNullOrEmpty(mvv.DestinatarioDiverso.CODICE_SOGGETTO) Then
                    If Not ControllaSoggettoTelematizzato(mvv.DestinatarioDiverso.CODICE_SOGGETTO, mvv.DestinatarioDiverso, listaSoggettiSian, messaggioErrore) Then
                        errori.Add(messaggioErrore)
                    End If
                End If
                If Not IsNothing(mvv.Vettore) AndAlso Not String.IsNullOrEmpty(mvv.Vettore.CODICE_SOGGETTO) Then
                    If Not ControllaSoggettoTelematizzato(mvv.Vettore.CODICE_SOGGETTO, mvv.Vettore, listaSoggettiSian, messaggioErrore) Then
                        errori.Add(messaggioErrore)
                    End If
                End If
                If Not IsNothing(mvv.Venditore) AndAlso Not String.IsNullOrEmpty(mvv.Venditore.CODICE_SOGGETTO) Then
                    If Not ControllaSoggettoTelematizzato(mvv.Venditore.CODICE_SOGGETTO, mvv.Venditore, listaSoggettiSian, messaggioErrore) Then
                        errori.Add(messaggioErrore)
                    End If
                End If
                If Not IsNothing(mvv.Acquirente) AndAlso Not String.IsNullOrEmpty(mvv.Acquirente.CODICE_SOGGETTO) Then
                    If Not ControllaSoggettoTelematizzato(mvv.Acquirente.CODICE_SOGGETTO, mvv.Acquirente, listaSoggettiSian, messaggioErrore) Then
                        errori.Add(messaggioErrore)
                    End If
                End If

                If errori.Any() Then
                    Return Nothing
                End If

            End If

            mvve.CodOper = codOper
            mvve.CodIcqrfSped = mvv.Documento.CodiceICQRF
            If Not String.IsNullOrEmpty(mvv.Speditore.CodiceAccisa) Then
                mvve.seed = mvv.Speditore.CodiceAccisa
            End If
            If Not String.IsNullOrEmpty(mvv.Destinatario.CodiceAccisa) Then
                mvve.seedDest = mvv.Destinatario.CodiceAccisa
            End If
            If mvv.HaDestinatarioDiverso Then
                mvve.seedDest = mvv.DestinatarioDiverso.CodiceAccisa
            End If

            mvve.FlagArt29 = CType(mvv.Documento.FlagArt29, MVVSiRPVInputFlagArt29)
            mvve.FlagArt29Specified = (mvve.FlagArt29 = MVVSiRPVInputFlagArt29.Item1)
            mvve.FlagArt33 = MVVSiRPVInputFlagArt33.Item0
            mvve.FlagArt33Specified = False

            ' destinatario
            If _configMin.ModalitaInvioSoggetti = 2 Then

                ' Se presente un luogo di consegna diverso e non presente destinatario diverso sostituisco indirizzo
                If mvv.HaLuogoConsegnaDiverso AndAlso Not mvv.HaDestinatarioDiverso Then
                    mvv.Destinatario.Indirizzo = mvv.IndirizzoConsegnaDiverso
                End If

                'Anagrafica Completa
                Dim destinatario = MappaSoggetto(mvv.Destinatario, errori)
                If Not destinatario Is Nothing Then
                    mvve.Destinatario = New MVVSiRPVInputDestinatario With
                    {
                        .Item = destinatario
                    }
                Else
                    messaggioErrore = "Non è stato possibile inviare l'anagrafica completa del Destinatario"
                    _logger.Logga(nomeProcedura, messaggioErrore)
                    ' tento comunque invio attraverso codice soggetto
                    If Not IsNothing(mvv.Destinatario) AndAlso Not String.IsNullOrEmpty(mvv.Destinatario.CODICE_SOGGETTO) Then
                        If ControllaSoggettoTelematizzato(mvv.Destinatario.CODICE_SOGGETTO, mvv.Destinatario, listaSoggettiSian, messaggioErrore) Then
                            messaggioErrore = "Tentativo invio anagrafica Destinatario attraverso Codici Sian"
                            _logger.Logga(nomeProcedura, messaggioErrore)
                            'Codice Soggetto
                            mvve.Destinatario = New MVVSiRPVInputDestinatario With
                            {
                                .Item = mvv.Destinatario.CODICE_SOGGETTO
                            }
                        Else
                            errori.Add(messaggioErrore)
                            Return Nothing
                        End If
                    End If
                End If

            Else
                'Codice Soggetto
                mvve.Destinatario = New MVVSiRPVInputDestinatario With
                {
                    .Item = mvv.Destinatario.CODICE_SOGGETTO
                }
            End If

            ' destinatario diverso
            If mvv.HaDestinatarioDiverso Then
                If _configMin.ModalitaInvioSoggetti = 2 Then

                    ' Anagrafica

                    ' Se presente un luogo di consegna diverso 
                    If mvv.HaLuogoConsegnaDiverso Then
                        mvv.DestinatarioDiverso.Indirizzo = mvv.IndirizzoConsegnaDiverso
                    End If

                    Dim destDiverso = MappaSoggetto(mvv.DestinatarioDiverso, errori)
                    If Not destDiverso Is Nothing Then
                        mvve.LuogoDestinatario = New MVVSiRPVInputLuogoDestinatario With
                        {
                            .Item = destDiverso
                        }
                    Else
                        messaggioErrore = "Non è stato possibile inviare l'anagrafica completa del Destinatario Diverso"
                        _logger.Logga(nomeProcedura, messaggioErrore)
                        ' tento comunque invio attraverso codice soggetto
                        If Not IsNothing(mvv.DestinatarioDiverso) AndAlso Not String.IsNullOrEmpty(mvv.DestinatarioDiverso.CODICE_SOGGETTO) Then
                            If ControllaSoggettoTelematizzato(mvv.DestinatarioDiverso.CODICE_SOGGETTO, mvv.DestinatarioDiverso, listaSoggettiSian, messaggioErrore) Then
                                messaggioErrore = "Tentativo invio anagrafica Destinatario Diverso attraverso Codici Sian"
                                _logger.Logga(nomeProcedura, messaggioErrore)
                                'Codice Soggetto
                                mvve.LuogoDestinatario = New MVVSiRPVInputLuogoDestinatario With
                                {
                                    .Item = mvv.DestinatarioDiverso.CODICE_SOGGETTO
                                }
                            Else
                                errori.Add(messaggioErrore)
                                Return Nothing
                            End If
                        End If
                    End If

                Else
                    ' Codice
                    mvve.LuogoDestinatario = New MVVSiRPVInputLuogoDestinatario With
                    {
                        .Item = mvv.DestinatarioDiverso.CODICE_SOGGETTO
                    }
                End If
            End If

            ' acquirente 
            If Not mvv.Acquirente Is Nothing Then
                If _configMin.ModalitaInvioSoggetti = 2 Then

                    ' Anagrafica

                    Dim acquirente = MappaSoggetto(mvv.Acquirente, errori)
                    If Not acquirente Is Nothing Then
                        mvve.Acquirente = New MVVSiRPVInputAcquirente With
                        {
                            .Item = acquirente
                        }
                    Else
                        messaggioErrore = "Non è stato possibile inviare l'anagrafica completa dell' Acquirente"
                        _logger.Logga(nomeProcedura, messaggioErrore)
                        ' tento comunque invio attraverso codice soggetto
                        If Not IsNothing(mvv.Acquirente) AndAlso Not String.IsNullOrEmpty(mvv.Acquirente.CODICE_SOGGETTO) Then
                            If ControllaSoggettoTelematizzato(mvv.Acquirente.CODICE_SOGGETTO, mvv.Acquirente, listaSoggettiSian, messaggioErrore) Then
                                messaggioErrore = "Tentativo invio anagrafica Acquirente attraverso Codici Sian"
                                _logger.Logga(nomeProcedura, messaggioErrore)
                                'Codice Soggetto
                                mvve.Acquirente = New MVVSiRPVInputAcquirente With
                                {
                                    .Item = mvv.Acquirente.CODICE_SOGGETTO
                                }
                            Else
                                errori.Add(messaggioErrore)
                                Return Nothing
                            End If
                        End If
                    End If

                Else
                    ' Codice
                    mvve.Acquirente = New MVVSiRPVInputAcquirente With
                    {
                        .Item = mvv.Acquirente.CODICE_SOGGETTO
                    }
                End If
            End If


            ' venditore 
            If Not mvv.Venditore Is Nothing Then
                If _configMin.ModalitaInvioSoggetti = 2 Then

                    ' Anagrafica

                    Dim venditore = MappaSoggetto(mvv.Venditore, errori)
                    If Not venditore Is Nothing Then
                        mvve.Venditore = New MVVSiRPVInputVenditore With
                            {
                                .Item = venditore
                            }
                    Else
                        messaggioErrore = "Non è stato possibile inviare l'anagrafica completa del Venditore"
                        _logger.Logga(nomeProcedura, messaggioErrore)
                        ' tento comunque invio attraverso codice soggetto
                        If Not IsNothing(mvv.Venditore) AndAlso Not String.IsNullOrEmpty(mvv.Venditore.CODICE_SOGGETTO) Then
                            If ControllaSoggettoTelematizzato(mvv.Venditore.CODICE_SOGGETTO, mvv.Venditore, listaSoggettiSian, messaggioErrore) Then
                                messaggioErrore = "Tentativo invio anagrafica Venditore attraverso Codici Sian"
                                _logger.Logga(nomeProcedura, messaggioErrore)
                                'Codice Soggetto
                                mvve.Venditore = New MVVSiRPVInputVenditore With
                                    {
                                        .Item = mvv.Venditore.CODICE_SOGGETTO
                                    }
                            Else
                                errori.Add(messaggioErrore)
                                Return Nothing
                            End If
                        End If
                    End If

                Else
                    ' Codice
                    mvve.Venditore = New MVVSiRPVInputVenditore With
                        {
                            .Item = mvv.Venditore.CODICE_SOGGETTO
                        }
                End If
            End If


            ' trasportatore
            Dim anagraficaVettore As MVV_Anagrafica = Nothing
            Select Case mvv.Vettore.Mezzo
                ' Cedente
                Case 0
                    anagraficaVettore = mvv.Speditore
                ' Cessionario
                Case 1
                    anagraficaVettore = mvv.Destinatario
                ' vettore
                Case 2
                    anagraficaVettore = mvv.Vettore
            End Select


            If _configMin.ModalitaInvioSoggetti = 2 Then

                ' Anagrafica
                Dim trasportatore = MappaSoggetto(anagraficaVettore, errori)
                If Not trasportatore Is Nothing Then
                    mvve.Trasportatore = New MVVSiRPVInputTrasportatore With
                    {
                        .Item = trasportatore
                    }
                Else
                    messaggioErrore = "Non è stato possibile inviare l'anagrafica completa del Trasportatore"
                    _logger.Logga(nomeProcedura, messaggioErrore)
                    ' tento comunque invio attraverso codice soggetto
                    If Not IsNothing(anagraficaVettore) AndAlso Not String.IsNullOrEmpty(anagraficaVettore.CODICE_SOGGETTO) Then
                        If ControllaSoggettoTelematizzato(anagraficaVettore.CODICE_SOGGETTO, anagraficaVettore, listaSoggettiSian, messaggioErrore) Then
                            messaggioErrore = "Tentativo invio anagrafica Vettore attraverso Codici Sian"
                            _logger.Logga(nomeProcedura, messaggioErrore)
                            'Codice Soggetto
                            mvve.Trasportatore = New MVVSiRPVInputTrasportatore With
                                {
                                    .Item = anagraficaVettore.CODICE_SOGGETTO
                                }
                        Else
                            errori.Add(messaggioErrore)
                            Return Nothing
                        End If
                    End If
                End If
            Else
                ' Codice
                mvve.Trasportatore = New MVVSiRPVInputTrasportatore With
                {
                    .Item = mvv.Vettore.CODICE_SOGGETTO
                }
            End If

            mvve.TipmeCod = _decodificatore.DecodificaUnitaTrasporto(mvv.MezzoTrasoprto.Codice)
            If String.IsNullOrEmpty(mvve.TipmeCod) Then
                errori.Add(String.Format("Campo TipmeCod non valorizzato. Decodifica non trovata per mezzo di trasporto con codice {0}", mvv.MezzoTrasoprto.Codice))
                Return Nothing
            End If

            'Marco: Elimino enodotto
            If Not String.IsNullOrEmpty(mvv.MezzoTrasoprto.Targa) AndAlso mvve.TipmeCod <> 16 Then
                mvve.MezzoTarga = mvv.MezzoTrasoprto.Targa
            Else
                errori.Add("Campo MezzoTarga non valorizzato.")
                Return Nothing
            End If


            If Not String.IsNullOrEmpty(mvv.MezzoTrasoprto.TargaRimorchio) Then
                mvve.RimorchioTarga = mvv.MezzoTrasoprto.TargaRimorchio
            End If

            If Not String.IsNullOrEmpty(mvv.Conducente_Cognome) Then
                mvve.CondCognome = mvv.Conducente_Cognome.Trim()
            End If

            If Not String.IsNullOrEmpty(mvv.Conducente_Nome) Then
                mvve.CondNome = mvv.Conducente_Nome.Trim()
            End If

            mvve.TitrCod = _decodificatore.DecodificaCausaleTrasporto(mvv.Documento.CausaleTrasporto)
            If String.IsNullOrEmpty(mvve.TitrCod) Then
                errori.Add(String.Format("La casuale di trasporto {0} non è tra quelle ammesse.", mvv.Documento.CausaleTrasporto))
                Return Nothing
            End If


            mvve.DataTrasp = New Date(mvv.Documento.DataInizioTrasporto.Year, mvv.Documento.DataInizioTrasporto.Month, mvv.Documento.DataInizioTrasporto.Day)
            'mvve.DataTrasp = New Date(2022, 4, 14)

            mvve.OraTrasp = mvv.Documento.OraTrasporto
            'mvve.OraTrasp = 15

            mvve.MinutiTrasp = mvv.Documento.MinutiTrasporto

            ' Note
            If Not String.IsNullOrEmpty(mvv.Note) Then
                mvve.AltreInfo = mvv.Note.Trim
            End If

            ' Sezione prodotti
            Dim listaProdottiMvv As New List(Of ProdMVV)
            For Each d As MVV_Dettaglio In mvv.Dettagli

                Dim dettaglio = MappaProdottto(codOper.Item, mvv, d, errori)
                If Not dettaglio Is Nothing Then
                    listaProdottiMvv.Add(dettaglio)
                Else
                    ' TODO messaglio da migliorare 
                    errori.Add("Errore durante la mappatura di un prodotto di dettaglio")
                    Return Nothing
                End If

            Next
            mvve.ProdottiMVV = listaProdottiMvv.ToArray

        Catch ex As Exception
            messaggioErrore = String.Format("Si è verificato un errore durante la generazione del file Xml. {0} {1}", vbCrLf, ex.Message)
            _logger.Logga(nomeProcedura, messaggioErrore)
            errori.Add(messaggioErrore)
            Return Nothing
        End Try

        Return mvve

    End Function

    Public Function CheckPreliminari(ByVal PIVA As String,
                                     ByVal idAgenda As Integer) As List(Of String)


        Dim nomeProcedura As String = "MVV_XmlGenerator.CheckPreliminari"
        Dim errori As New List(Of String)
        Dim messaggioErrore As String

        Try
            Dim mvv = _stampaController.PreparaDatiStampa(PIVA, idAgenda)

            ' Controlle se trasporto a mezzo vettore
            If mvv.Vettore.Mezzo <> 2 Then
                errori.Add(String.Format("ATTENZIONE: Il trasporto non è a cura del VETTORE!"))
                errori.Add(Environment.NewLine)
            End If

            ' Controllo titoli alcolici a 0
            For Each d As MVV_Dettaglio In mvv.Dettagli
                If d.TitoloAlcolEff = 0 OrElse d.TitoloAlcolPot = 0 OrElse d.TitoloAlcolTot = 0 Then
                    'OrElse String.IsNullOrEmpty(d.RegistroVino.CodZonaViticola.Codice.Trim) Then
                    errori.Add(String.Format("{0} :", d.DescrizioneGias))
                End If

                If d.TitoloAlcolEff = 0 Then
                    errori.Add("   >> Il Titolo Alcol Effettivo ha valore 0")
                End If
                If d.TitoloAlcolPot = 0 Then
                    errori.Add("   >> Il Titolo Alcol Potenziale ha valore 0")
                End If
                If d.TitoloAlcolTot = 0 Then
                    errori.Add("   >> Il Titolo Alcol Totale ha valore 0")
                End If
                'If String.IsNullOrEmpty(d.RegistroVino.CodZonaViticola.Codice.Trim) Then
                '    errori.Add("   >> Codice Zona Viticola mancante")
                'End If

                If errori.Any Then
                    errori.Add(Environment.NewLine)
                End If

            Next

        Catch ex As Exception
            messaggioErrore = String.Format("Si è verificato un errore durante la fase di controllo preliminare. {0} {1}", vbCrLf, ex.Message)
            _logger.Logga(nomeProcedura, messaggioErrore)
            errori.Add(messaggioErrore)
        End Try

        Return errori

    End Function
    Private Function MappaSoggetto(ByVal soggetto As MVV_Anagrafica, ByRef errori As List(Of String)) As DettaglioSoggettoMVV

        Dim messaggio As String = ""

        Dim sogMvv = New DettaglioSoggettoMVV
        Dim cuaa As CUAA = Nothing

        If soggetto.ID_CF = enum_Contatti_IdCf.PersonaGiuridica Then
            cuaa = New CUAA With {
                .ItemElementName = ItemChoiceType.PersonaGiuridica,
                .Item = soggetto.COD_CONTATTO
                }
        ElseIf soggetto.ID_CF = enum_Contatti_IdCf.PersonaFisica Then
            cuaa = New CUAA With {
                .ItemElementName = ItemChoiceType.PersonaFisica,
                .Item = soggetto.COD_CONTATTO
                }
        End If

        If Not cuaa Is Nothing Then
            sogMvv.CUAAMVV = cuaa
        End If

        If soggetto.ID_CF = enum_Contatti_IdCf.PersonaGiuridica Then
            sogMvv.RagSocSoggMVV = soggetto.Denominazione
        Else
            If Not String.IsNullOrEmpty(soggetto.Cognome) And Not String.IsNullOrEmpty(soggetto.Nome) Then
                sogMvv.CognomeSoggMVV = soggetto.Cognome
                sogMvv.NomeSoggMVV = soggetto.Nome
            Else
                If Not String.IsNullOrEmpty(soggetto.Denominazione) Then
                    Dim nc = soggetto.Denominazione.Split(" ")
                    sogMvv.CognomeSoggMVV = nc(0)
                    If nc.Count > 1 Then
                        sogMvv.NomeSoggMVV = nc(1)
                    End If
                End If
            End If
        End If

        Dim indirizzo = New Indirizzo

        If Not String.IsNullOrEmpty(soggetto.Indirizzo.CAP) Then
            indirizzo.CAP = soggetto.Indirizzo.CAP
        End If
        If Not String.IsNullOrEmpty(soggetto.Indirizzo.Indirizzo) Then
            indirizzo.Indirizzo1 = soggetto.Indirizzo.Indirizzo
        Else
            messaggio = String.Format("Campo Indirizzo non valorizzato per il {0} ", soggetto.TipoSoggetto)
            errori.Add(messaggio)
            Return Nothing
        End If

        If soggetto.ID_CF <> enum_Contatti_IdCf.ContattoEstero Then
            indirizzo.Provincia = soggetto.Indirizzo.Istat_Provincia
            If String.IsNullOrEmpty(indirizzo.Provincia) Then
                messaggio = String.Format("Campo Provincia non valorizzato per il {0} ", soggetto.TipoSoggetto)
                errori.Add(messaggio)
                Return Nothing
            End If
            indirizzo.Comune = soggetto.Indirizzo.Istat_Comune
            If String.IsNullOrEmpty(indirizzo.Comune) Then
                messaggio = String.Format("Campo Comune non valorizzato per il {0} ", soggetto.TipoSoggetto)
                errori.Add(messaggio)
                Return Nothing
            End If
        End If
        indirizzo.Stato = _decodificatore.DecodificaStato(soggetto.Indirizzo.Stato)
        If String.IsNullOrEmpty(indirizzo.Stato) Then
            messaggio = String.Format("Campo Stato non valorizzato per il {0} ", soggetto.TipoSoggetto)
            errori.Add(messaggio)
            Return Nothing
        End If
        If soggetto.ID_CF <> enum_Contatti_IdCf.ContattoEstero Then
            sogMvv.TipoSoggMVV = DettaglioSoggettoMVVTipoSoggMVV.IT
        Else
            If _decodificatore.StatoMembro(indirizzo.Stato) Then
                sogMvv.TipoSoggMVV = DettaglioSoggettoMVVTipoSoggMVV.UE
            Else
                sogMvv.TipoSoggMVV = DettaglioSoggettoMVVTipoSoggMVV.EX
            End If
        End If


        sogMvv.IndirizzoSoggMVV = indirizzo

        Return sogMvv

    End Function

    Private Function MappaProdottto(ByVal codOper As String,
                                    ByVal mvv As MVV,
                                    ByVal dettaglio As MVV_Dettaglio,
                                    ByRef errori As List(Of String)) As ProdMVV

        Dim messaggioErrore As String = String.Empty
        Dim nomeProcedura = "MVV_XmlGenerator.MappaProdottto"
        Dim pe As New ProdMVV

        pe.CodTipoProdMVV = _decodificatore.DecodificaCodiceTipoProdotto(
                                dettaglio.CodiceGenerazione,
                                dettaglio.RegistroVino.CodCategoria.Codice,
                                dettaglio.RegistroVino.CodClassificazione.Codice
                                )

        If String.IsNullOrEmpty(pe.CodTipoProdMVV) Then
            errori.Add(String.Format("Campo CodTipoProdMVV non valorizzato. Non trovata decodifica per Codice Generazione {0}, Categoria {1} e Codice Classificazione {2}",
                                     dettaglio.CodiceGenerazione, dettaglio.RegistroVino.CodCategoria.Codice, dettaglio.RegistroVino.CodClassificazione.Codice))
            Return Nothing
        End If

        ' Cerco il prodotto nei teleregistri sian
        Dim prodSian = CercaProdottoSian(codOper, dettaglio.Mat_Cod, dettaglio.Lotto)

        If _configMin.ModalitaInvioProdotti = 2 Then
            ' Prodotto Catalogo
            Dim pc = MappaProdottoCatalogo(mvv, dettaglio, errori)
            If Not pc Is Nothing Then
                pe.Prodotto = New ProdMVVProdotto With
                {
                    .Item = pc
                }
            Else
                messaggioErrore = String.Format("Non è stato possibile inviare l'anagrafica completa del Prodotto {0} ", dettaglio.RegistroVino.Designazione)
                _logger.Logga(nomeProcedura, messaggioErrore)
                ' invio attraverso codici Sian
                If Not prodSian Is Nothing Then
                    messaggioErrore = "Tentativo invio anagrafica Prodotto attraverso Codici Sian"
                    _logger.Logga(nomeProcedura, messaggioErrore)
                    Dim cp As New CodiceProdotto With
                    {
                        .CodPrimario = prodSian.CodPrimario,
                        .CodSecondario = prodSian.CodSecondario
                    }
                    pe.Prodotto = New ProdMVVProdotto With
                    {
                       .Item = cp
                    }
                Else
                    messaggioErrore = String.Format("Non è stato possibile recuperare i codici Sian per il Prodotto {0} ", dettaglio.RegistroVino.Designazione)
                    errori.Add(messaggioErrore)
                    _logger.Logga(nomeProcedura, messaggioErrore)
                    Return Nothing
                End If
            End If
        Else
            ' Codice Prodotto
            If Not prodSian Is Nothing Then
                Dim cp As New CodiceProdotto With
                    {
                        .CodPrimario = prodSian.CodPrimario,
                        .CodSecondario = prodSian.CodSecondario
                    }
                pe.Prodotto = New ProdMVVProdotto With
                    {
                       .Item = cp
                    }
            Else
                messaggioErrore = String.Format("Non è stato possibile recuperare i codici Sian per il Prodotto {0} ", dettaglio.RegistroVino.Designazione)
                errori.Add(messaggioErrore)
                _logger.Logga(nomeProcedura, messaggioErrore)
                Return Nothing
            End If

        End If


        If Not String.IsNullOrEmpty(dettaglio.CodiceNC) AndAlso dettaglio.CodiceNC.Length = 8 Then
            pe.CodiciNomenclatura = New CodiciNomenclatura With
                {
                    .CodNC1 = Left(dettaglio.CodiceNC, 4),
                    .CodNC2 = Mid(dettaglio.CodiceNC, 5, 2),
                    .CodNC3 = Right(dettaglio.CodiceNC, 2)
            }
        End If

        If _matriceCampi.CampoObbligatorio(enumAttributiSian.AlcoolTotale, dettaglio.RegistroVino.CodCategoria.Codice) Then
            pe.TitoloAlcolTot = dettaglio.TitoloAlcolTot
            pe.TitoloAlcolTotSpecified = True
        End If
        If _matriceCampi.CampoObbligatorio(enumAttributiSian.AlcoolPotenziale, dettaglio.RegistroVino.CodCategoria.Codice) Then
            pe.TitoloAlcolPot = dettaglio.TitoloAlcolPot
            pe.TitoloAlcolPotSpecified = True
        End If
        If _matriceCampi.CampoObbligatorio(enumAttributiSian.AlcoolEffettivo, dettaglio.RegistroVino.CodCategoria.Codice) Then
            pe.TitoloAlcolEff = dettaglio.TitoloAlcolEff
            pe.TitoloAlcolEffSpecified = True
        End If

        If Not String.IsNullOrEmpty(dettaglio.Lotto) Then
            pe.Lotto = dettaglio.Lotto
        End If

        ' Tabellati
        Dim udmSin As String = dettaglio.UDMQTA.UDM_SIM
        Dim qta As Decimal = dettaglio.Quantita
        If dettaglio.UDMQTA.UDM_COD_EXTRA <> 0 Then
            udmSin = dettaglio.UDMQTA.UDM_SIN_EXTRA2
            qta = dettaglio.UDMQTA.QTA_EXtra_Tot
            If qta = 0 Then
                qta = dettaglio.Quantita * dettaglio.UDMQTA.QTA_Extra
            End If
        End If
        pe.UniMis = udmSin
        pe.Quant = qta
        If pe.Quant = 0 Then
            messaggioErrore = String.Format("Non è stato possibile valorizzare la Qtà in Litri.")
            errori.Add(messaggioErrore)
            _logger.Logga(nomeProcedura, messaggioErrore)
            Return Nothing
        End If

        ' Descrizione Addizionale
        If Not String.IsNullOrEmpty(dettaglio.DescrizioneGiasAddizionale) Then
            pe.NoteDes = TroncaStringa(dettaglio.DescrizioneGiasAddizionale, 200)
        End If

        ' Setto Colli e imballi in base aloo stati fisico
        If dettaglio.RegistroVino.CodStatoFisico.Codice.Trim = "1" Then

            ' 1 Sfusi --> Mette imballi al posto dei colli
            If dettaglio.NumeroImballi <> 0 Then
                pe.NumColli = dettaglio.NumeroImballi
                pe.NumColliSpecified = True
            End If

        Else

            '2 Imbottigliato/confezionato, 3 Imbottigliato senza etichetta  -> Mette colli nei colli, imballi negli imballi e setta CapacitaImballo
            If dettaglio.NumeroColli <> 0 Then
                pe.NumColli = dettaglio.NumeroColli
                pe.NumColliSpecified = True
            End If

            If dettaglio.NumeroImballi <> 0 Then
                pe.NumImb = dettaglio.NumeroImballi
                pe.NumImbSpecified = True
            Else
                If dettaglio.UDMQTA.QTA <> 0 Then
                    pe.NumImb = dettaglio.UDMQTA.QTA
                    pe.NumImbSpecified = True
                End If
            End If

            If dettaglio.CapacitaImballo_MateriaPrima <> 0 Then
                pe.CapacitaImb = dettaglio.CapacitaImballo_MateriaPrima
                pe.CapacitaImbSpecified = True
                pe.UniMisCapacitaImb = dettaglio.UDMQTA_MAteriaPrima.UDM_SIM_EXTRA
            End If

        End If

        ' TODO QUI per colli e imballi
        'If dettaglio.UsaColli Then
        '    pe.NumColli = dettaglio.NumeroColli
        '    pe.NumColliSpecified = True
        'Else
        '    pe.NumImb = dettaglio.NumeroImballi
        '    pe.NumImbSpecified = True
        'End If

        ' tenore zuccherino
        If Not String.IsNullOrEmpty(dettaglio.RegistroVino.CodTenoreZucchero.Codice) Then
            If dettaglio.RegistroVino.CodTenoreZucchero.Codice.Trim() <> "0" Then
                pe.CodTenoreZucc = _decodificatore.DecodificaTenoreZucchero(dettaglio.RegistroVino.CodTenoreZucchero.Codice.Trim)
                If String.IsNullOrEmpty(pe.CodTenoreZucc) Then
                    errori.Add(String.Format("Campo CodTenoreZucchero non valorizzato correttamente. Il valore {0} non è ammesso.", dettaglio.RegistroVino.CodTenoreZucchero.Codice))
                    Return Nothing
                End If
            End If
        End If


        Return pe

    End Function

    Private Function MappaProdottoCatalogo(ByVal mvv As MVV,
                                      ByVal dettaglio As MVV_Dettaglio,
                                      ByVal errori As List(Of String)) As ProdottoCatalogo

        Dim messaggio As String = ""
        Dim pc = New ProdottoCatalogo

        If IsNothing(dettaglio.RegistroVino) Then
            errori.Add("Non sono state trovate le decodifiche dei teleregistri per uno o più prodotti")
            Return Nothing
        End If

        pc.CodCategoria = dettaglio.RegistroVino.CodCategoria.Codice
        If (String.IsNullOrEmpty(pc.CodCategoria)) Then
            errori.Add("Campo CodCategoria non valorizzato per per uno o più dettagli")
            Return Nothing
        End If
        If Not String.IsNullOrEmpty(dettaglio.RegistroVino.CodClassificazione.Codice) Then
            pc.CodClassificazione = dettaglio.RegistroVino.CodClassificazione.Codice
        End If
        If Not String.IsNullOrEmpty(dettaglio.RegistroVino.DocIGP.Codice) AndAlso dettaglio.RegistroVino.DocIGP.Codice <> "0" Then
            pc.CodDopIgp = dettaglio.RegistroVino.DocIGP.Codice
        End If
        If Not String.IsNullOrEmpty(dettaglio.RegistroVino.AttoCert.Codice) Then
            pc.AttoCert = dettaglio.RegistroVino.AttoCert.Codice
        End If
        If Not String.IsNullOrEmpty(dettaglio.RegistroVino.CodEbacchus.Codice) Then
            pc.CodEbacchus = dettaglio.RegistroVino.CodEbacchus.Codice
        End If
        If Not String.IsNullOrEmpty(dettaglio.RegistroVino.OrigineUve.Codice) Then
            pc.OrigineUve = dettaglio.RegistroVino.OrigineUve.Codice
        End If
        If Not String.IsNullOrEmpty(dettaglio.RegistroVino.Provenienza.Codice) Then
            pc.Provenienza = dettaglio.RegistroVino.Provenienza.Codice
        End If
        If Not String.IsNullOrEmpty(dettaglio.RegistroVino.PaesiProvenienza) Then
            Dim listaPp = dettaglio.RegistroVino.PaesiProvenienza.Split("|")
            Dim pp = New List(Of PaesiProvenienza)
            For Each p As String In listaPp
                pp.Add(New PaesiProvenienza With {.Codice = p})
            Next
            If pp.Any Then
                pc.PaesiProvenienza = pp.ToArray
            End If
        End If

        ' TODO Modifiche a CodiceZonaViticola in base allo stato fisico
        If Not String.IsNullOrEmpty(dettaglio.RegistroVino.CodStatoFisico.Codice) Then
            Dim statoFisico = _decodificatore.DecodificaStatoFisico(dettaglio.RegistroVino.CodStatoFisico.Codice)

            Select Case statoFisico
                ' Sfuso
                Case "1"
                    ' Prendo zona viticola da weregvinoProdotti
                    If Not String.IsNullOrEmpty(dettaglio.RegistroVino.CodZonaViticola.Codice) Then
                        pc.CodZonaViticola = dettaglio.RegistroVino.CodZonaViticola.Codice
                    End If

                ' Imbottigliato/confezionato
                Case "2"
                  ' non mappo la zona viticola

                ' Imbottigliato senza etichetta
                Case "3"
                    'Prendo la zona viticola dalla linea di produzione
                    If Not String.IsNullOrEmpty(dettaglio.CodZonaViticola_Linea) Then
                        pc.CodZonaViticola = dettaglio.CodZonaViticola_Linea
                    Else
                        ' Se vuoto quello della linea di produzione prende quello scritto in OGenerazioni_Anagrafe_Moduli_Log
                        If Not String.IsNullOrEmpty(dettaglio.CodZonaViticola_Gen) Then
                            pc.CodZonaViticola = dettaglio.CodZonaViticola_Gen
                        End If
                    End If

            End Select

        End If


        ' TODO varieta
        'If Not String.IsNullOrEmpty(dettaglio.RegistroVino.Varieta.Codice) Then
        '    Dim listaV = dettaglio.RegistroVino.Varieta.Codice.Split("|").Where(Function(v) v <> "").ToList
        '    Dim varieta As New List(Of Cod_ValorePerc)
        '    For Each v As String In listaV
        '        varieta.Add(New Cod_ValorePerc With
        '                    {
        '                        .Codice = v,
        '                        .Percentuale = 0,
        '                        .PercentualeSpecified = False
        '                    })
        '    Next
        '    pc.Varieta = varieta.ToArray
        'End If

        'altre varieta
        'If Not String.IsNullOrEmpty(dettaglio.RegistroVino.AltreVarieta) Then
        '    pc.AltreVarieta = dettaglio.RegistroVino.AltreVarieta
        'End If

        If Not String.IsNullOrEmpty(dettaglio.RegistroVino.CodSottozona.Codice) AndAlso dettaglio.RegistroVino.CodSottozona.Codice <> "0" Then
            pc.CodSottozona = dettaglio.RegistroVino.CodSottozona.Codice
        End If

        If Not String.IsNullOrEmpty(dettaglio.RegistroVino.COdVigna.Codice) Then
            pc.CodVigna = dettaglio.RegistroVino.COdVigna.Codice
        End If

        If Not String.IsNullOrEmpty(dettaglio.RegistroVino.CodColore.Codice) Then
            pc.CodColore = dettaglio.RegistroVino.CodColore.Codice
        End If

        If Not String.IsNullOrEmpty(dettaglio.RegistroVino.Menzioni) AndAlso dettaglio.RegistroVino.Menzioni <> "0" Then
            Dim listaM = dettaglio.RegistroVino.Menzioni.Split("|").Where(Function(m) m <> "").ToList
            Dim menzioni As New List(Of Menzioni)
            For Each m As String In listaM
                menzioni.Add(New Menzioni With
                            {
                                .Codice = m
                            })
            Next
            pc.Menzioni = menzioni.ToArray
        End If

        If Not String.IsNullOrEmpty(dettaglio.RegistroVino.Biologico.Codice) Then
            If dettaglio.RegistroVino.Biologico.Codice <> "0" Then
                pc.Biologico = dettaglio.RegistroVino.Biologico.Codice
            End If
        End If

        ' 2022-03-03 CodPartita fa Giacenza separata (implementazioni di Marco per incorporarlo nel lotto)
        If Not String.IsNullOrEmpty(dettaglio.RegistroVino.CodPartita) Then
            pc.CodPartita = dettaglio.RegistroVino.CodPartita
        End If

        If Not String.IsNullOrEmpty(dettaglio.RegistroVino.Annata.Codice) Then
            Dim ta As New TipoAnnata
            ta.Codice = dettaglio.RegistroVino.Annata.Codice
            If Not String.IsNullOrEmpty(dettaglio.RegistroVino.PercAnnata) AndAlso
                    IsNumeric(dettaglio.RegistroVino.PercAnnata) AndAlso CDbl(dettaglio.RegistroVino.PercAnnata) <> 0 Then
                ta.Percentuale = CDbl(dettaglio.RegistroVino.PercAnnata)
                ta.PercentualeSpecified = True

            End If
            pc.Annata = ta
        End If

        If dettaglio.RegistroVino.MassaVolumica <> 0 Then
            pc.MassaVolumica = dettaglio.RegistroVino.MassaVolumica
            pc.MassaVolumicaSpecified = True
        End If

        If dettaglio.RegistroVino.DataCertDOP <> AGRODATAFINE Then
            pc.DataCertDOP = dettaglio.RegistroVino.DataCertDOP
            pc.DataCertDOPSpecified = True
        End If

        If Not String.IsNullOrEmpty(dettaglio.RegistroVino.NumCertDOP) Then
            pc.NumCertDOP = dettaglio.RegistroVino.NumCertDOP
        End If

        If Not String.IsNullOrEmpty(dettaglio.RegistroVino.CodStatoFisico.Codice) Then
            pc.CodStatoFisico = _decodificatore.DecodificaStatoFisico(dettaglio.RegistroVino.CodStatoFisico.Codice)
            If String.IsNullOrEmpty(pc.CodStatoFisico) Then
                errori.Add(String.Format("Campo CodStatoFisico non valorizzato correttamente. Il valore {0} non è ammesso.", dettaglio.RegistroVino.CodStatoFisico.Codice))
                Return Nothing
            End If
        End If

        If _matriceCampi.CampoObbligatorio(enumAttributiSian.PraticaEnologica_N, dettaglio.RegistroVino.CodCategoria.Codice) Then

            If Not String.IsNullOrEmpty(dettaglio.RegistroVino.PraticheEnologiche) Then

                Dim codiciPe = dettaglio.RegistroVino.PraticheEnologiche.Split("|").Where(Function(s) s <> "").ToList
                Dim pe As New List(Of PraticheEnologiche)
                For Each p As String In codiciPe
                    If Not String.IsNullOrEmpty(_decodificatore.DecodificaPraticaEno(p)) Then
                        pe.Add(New PraticheEnologiche With {.Codice = p})
                    End If
                Next
                If pe.Any Then
                    pc.PraticheEnologiche = pe.ToArray
                End If

            End If

        End If

        Return pc

    End Function

    Private Function ControllaSoggettoTelematizzato(ByVal codiceSoggetto As String,
                                                    ByVal soggettoMvv As MVV_Anagrafica,
                                                    ByVal soggetti As IEnumerable(Of Object),
                                                    ByRef messaggio As String) As Boolean
        messaggio = ""

        Dim sog = soggetti.FirstOrDefault(Function(s) s.CodiceSoggetto.Equals(codiceSoggetto))
        If Not sog Is Nothing Then
            Return True
        Else
            Dim sb = New StringBuilder
            sb.AppendLine("ANAGRAFICA NON ACORA TELEMATIZZATA SUL SIAN")
            sb.AppendLine()
            sb.Append(soggettoMvv.ToString())
            messaggio = sb.ToString
            Return False
        End If

    End Function

    Private Function CercaProdottoSian(ByVal codOper As String, ByVal mat_cod As Integer, ByVal lotto As String) As Object

        Dim dt As DataTable = _MVVElettronico_R.CercaProdottoSian(codOper, mat_cod, lotto)
        If Not dt Is Nothing AndAlso dt.Rows.Count = 1 Then

            Dim prodSian = New With
            {
                .CodPrimario = If(dt.Rows(0).Item("CodPrimario") Is DBNull.Value, "", dt.Rows(0).Item("CodPrimario")),
                .CodSecondario = If(dt.Rows(0).Item("CodSecondario") Is DBNull.Value, "", dt.Rows(0).Item("CodSecondario"))
            }
            If String.IsNullOrEmpty(prodSian.CodPrimario) OrElse String.IsNullOrEmpty(prodSian.CodSecondario) Then
                Return Nothing
            Else
                Return prodSian
            End If

        End If

        Return Nothing

    End Function
    Private Function TroncaStringa(ByVal testo As String, ByVal numCaratteri As Integer) As String

        If String.IsNullOrEmpty(testo) Then
            Return String.Empty
        End If

        If testo.Length > numCaratteri Then
            Return testo.Substring(0, numCaratteri)
        Else
            Return testo
        End If

    End Function

End Class

