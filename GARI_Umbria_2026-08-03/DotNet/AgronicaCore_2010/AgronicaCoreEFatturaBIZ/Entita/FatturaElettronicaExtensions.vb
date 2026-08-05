Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreEFatturaBIZ.Entita
Imports AgronicaCoreEFatturaDAL
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreDataProvider.UtilityProvider

Module FatturaElettronicaExtensions

    <Extension()>
    Public Function MappaFatturaGias(ByVal fatturaGiasFromDb As FatturaGiasFromDB,
                                     ByVal mapper As IFatturaMapper) As FatturaGias

        Try
            ' dettagli tipo descrizione libere ->  elem_cod 502 RIGA_DESCRIZIONE_LIBERA
            ' dettagli tipo bolli -> elem_cod 555 CostantiPersonalizzate.SERVIZI e pro_cod 139
            Dim dettagli = fatturaGiasFromDb.DatiFattura.Dettagli.OfType(Of Movimenti_dettagli).ToList()
            Dim bolli = dettagli.Where(Function(d) d.Elem_Cod.Equals(CostantiPersonalizzate.SERVIZI) AndAlso d.Pro_Cod.Equals(139)).ToList()
            Dim prodottiServizi = dettagli.Except(bolli).ToList()

            Dim result = New FatturaGias With
            {
                .Cedente = New DatiCedenteMap With
                {
                    .TipoImpresa = fatturaGiasFromDb.Cedente.TipoImpresa,
                    .DatiPrincipali = New DatiPrincipaliCedenteMap With
                    {
                        .TipoImpresaGerarchia = fatturaGiasFromDb.Cedente.DatiPrincipali.TipoImpresaGerarchia,
                         .Anagrafica = New AnagraficaMap With
                         {
                            .rag_soc = fatturaGiasFromDb.Cedente.DatiPrincipali.rag_soc,
                            .Codice_Fiscale = fatturaGiasFromDb.Cedente.DatiPrincipali.Codice_Fiscale,
                            .Id_CF = fatturaGiasFromDb.Cedente.DatiPrincipali.Id_CF,
                            .Cod_Contatto = fatturaGiasFromDb.Cedente.DatiPrincipali.Cod_Contatto,
                            .Nome = fatturaGiasFromDb.Cedente.DatiPrincipali.Nome,
                            .Cognome = fatturaGiasFromDb.Cedente.DatiPrincipali.Cognome,
                            .PIVA = fatturaGiasFromDb.Cedente.DatiPrincipali.PIVA,
                            .PivaReale = fatturaGiasFromDb.PivaReale
                         },
                        .Indirizzo = New IndirizzoMap With
                        {
                            .ind_des = fatturaGiasFromDb.Cedente.DatiPrincipali.ind_des,
                            .frz_des = fatturaGiasFromDb.Cedente.DatiPrincipali.frz_des,
                            .CAP = fatturaGiasFromDb.Cedente.DatiPrincipali.CAP,
                            .com_des = fatturaGiasFromDb.Cedente.DatiPrincipali.com_des,
                            .pro_cod = fatturaGiasFromDb.Cedente.DatiPrincipali.pro_cod,
                            .pro_cod_istat = fatturaGiasFromDb.Cedente.DatiPrincipali.pro_cod_istat,
                            .stato = fatturaGiasFromDb.Cedente.DatiPrincipali.stato,
                            .com_cod_istat = fatturaGiasFromDb.Cedente.DatiPrincipali.com_cod_istat
                        }
                    },
                    .DatiAggiuntivi = fatturaGiasFromDb.Cedente.DatiAggiuntivi.ToList() _
                                            .Select(Function(da) New DatoAggiuntivoMap With {
                                                            .id_cod = da.id_cod,
                                                            .val_cod = da.val_cod
                                                        })
                },
                .Cessionario = New DatiCessionarioMap With
                {
                    .DatiPrincipali = New DatiPrincipaliCessionarioMap With
                    {
                        .Anagrafica = New AnagraficaMap With
                        {
                            .Cognome = fatturaGiasFromDb.Cessionario.Cognome,
                            .Nome = fatturaGiasFromDb.Cessionario.Nome,
                            .rag_soc = fatturaGiasFromDb.Cessionario.Rag_Soc,
                            .PIVA = fatturaGiasFromDb.Cessionario.Piva,
                            .Codice_Fiscale = fatturaGiasFromDb.Cessionario.Codice_Fiscale,
                            .Cod_Contatto = fatturaGiasFromDb.Cessionario.Cod_Contatto,
                            .Id_CF = fatturaGiasFromDb.Cessionario.Id_CF
                        },
                        .Cod_RisUm = fatturaGiasFromDb.Cessionario.Cod_RisUm,
                        .Indirizzo = New IndirizzoMap With
                                {
                                    .CAP = fatturaGiasFromDb.Cessionario.CAP,
                                    .com_cod_istat = fatturaGiasFromDb.Cessionario.com_cod_istat,
                                    .com_des = fatturaGiasFromDb.Cessionario.com_des,
                                    .frz_des = fatturaGiasFromDb.Cessionario.frz_des,
                                    .ind_des = fatturaGiasFromDb.Cessionario.ind_des,
                                    .pro_cod = fatturaGiasFromDb.Cessionario.pro_cod,
                                    .pro_cod_istat = fatturaGiasFromDb.Cessionario.pro_cod_istat,
                                    .stato = fatturaGiasFromDb.Cessionario.stato,
                                    .Tipo_Indirizzo = fatturaGiasFromDb.Cessionario.Tipo_Indirizzo
                                }
                    }
                },
                 .CessionarioDiverso = New DatiCessionarioDiversoMap With
                 {
                    .Anagrafica = New AnagraficaMap With
                        {
                            .Cognome = fatturaGiasFromDb.CessionarioDiverso.Cognome,
                            .Nome = fatturaGiasFromDb.CessionarioDiverso.Nome,
                            .rag_soc = fatturaGiasFromDb.CessionarioDiverso.Rag_Soc,
                            .PIVA = fatturaGiasFromDb.CessionarioDiverso.Piva,
                            .Codice_Fiscale = fatturaGiasFromDb.CessionarioDiverso.Codice_Fiscale,
                            .Cod_Contatto = fatturaGiasFromDb.CessionarioDiverso.Cod_Contatto,
                            .Id_CF = fatturaGiasFromDb.CessionarioDiverso.Id_CF
                        },
                    .Cod_RisUm = fatturaGiasFromDb.CessionarioDiverso.Cod_RisUm,
                    .Indirizzo = New IndirizzoMap With
                            {
                                .CAP = fatturaGiasFromDb.CessionarioDiverso.CAP,
                                .com_cod_istat = fatturaGiasFromDb.CessionarioDiverso.com_cod_istat,
                                .com_des = fatturaGiasFromDb.CessionarioDiverso.com_des,
                                .frz_des = fatturaGiasFromDb.CessionarioDiverso.frz_des,
                                .ind_des = fatturaGiasFromDb.CessionarioDiverso.ind_des,
                                .pro_cod = fatturaGiasFromDb.CessionarioDiverso.pro_cod,
                                .pro_cod_istat = fatturaGiasFromDb.CessionarioDiverso.pro_cod_istat,
                                .stato = fatturaGiasFromDb.CessionarioDiverso.stato,
                                .Tipo_Indirizzo = fatturaGiasFromDb.CessionarioDiverso.Tipo_Indirizzo
                            }
                },
                .Fattura = New FatturaMap With
                {
                    .Testata = New DatiTestataFatturaMap With
                    {
                        .Id_Agenda = fatturaGiasFromDb.DatiFattura.Testata.Id_Agenda,
                        .Causale_Trasporto = fatturaGiasFromDb.DatiFattura.Testata.Causale_Trasporto,
                        .Cau_Mov = fatturaGiasFromDb.DatiFattura.Testata.Cau_Mov,
                        .Cod_Destinazione = fatturaGiasFromDb.DatiFattura.Testata.Cod_Destinazione,
                        .Cod_RisUm = fatturaGiasFromDb.DatiFattura.Testata.Cod_RisUm,
                        .Cod_RisUm_Aggiuntivo = fatturaGiasFromDb.DatiFattura.Testata.Cod_RisUm_Aggiuntivo,
                        .Cod_RisUm_Altro = fatturaGiasFromDb.DatiFattura.Testata.Cod_RisUm_Altro,
                        .Data_Registrazione = fatturaGiasFromDb.DatiFattura.Testata.Data_Registrazione,
                        .Doc_Numero = fatturaGiasFromDb.DatiFattura.Testata.Doc_Numero,
                        .Doc_Numero_Des = fatturaGiasFromDb.DatiFattura.Testata.Doc_Numero_Des,
                        .Doc_Numero_Sin = fatturaGiasFromDb.DatiFattura.Testata.Doc_Numero_Sin,
                        .Cod_IndirizzoDestinazione = fatturaGiasFromDb.DatiFattura.Testata.Cod_IndirizzoDestinazione,
                        .Mov_Desc = fatturaGiasFromDb.DatiFattura.Testata.Mov_Desc,
                        .TotaleFattura = fatturaGiasFromDb.DatiFattura.Testata.TotaleFattura,
                        .Sezionale_Cod = fatturaGiasFromDb.DatiFattura.Testata.Sezionale_Cod,
                        .Data_Movimento = fatturaGiasFromDb.DatiFattura.Testata.Data_Movimento,
                        .Lav_cod = fatturaGiasFromDb.DatiFattura.Testata.Lav_cod,
                        .Des_lib = fatturaGiasFromDb.DatiFattura.Testata.Des_lib,
                        .TipoDocumento = fatturaGiasFromDb.DatiFattura.Testata.TipoDocumento
                    },
                    .Dettagli = New DatiDettaglioFatturaMap With
                    {
                        .Bolli = bolli,
                        .ProdottiServizi = prodottiServizi.Select(Function(ps) New ProdottoServizioMap With
                        {
                            .Descrizione = "",
                            .Flag_Extra = False,
                            .Movimento = ps
                        }).ToList()
                    }
                },
                .OrdiniAcquisto = fatturaGiasFromDb.OrdiniAcquisto.Select(Function(oa)
                                                                              Return New OrdineAcquistoMap With {
                                                                                    .N_Doc_Cliente = oa.N_Doc_Cliente,
                                                                                    .Data_Doc_Cliente = oa.Data_Doc_Cliente,
                                                                                    .Ordine_Det = oa.Ordine_Det
                                                                                  }
                                                                          End Function).ToList(),
                .DocumentiCollegati = New DocumentiCollegatiMap With
                                        {
                                            .Documenti = fatturaGiasFromDb.DocumentiCOllegati.Select(Function(dc)
                                                                                                         Return New DocumentoCollegatoMap With
                                                                                      {
                                                                                        .Id_Agenda = dc.Id_Agenda,
                                                                                        .Data_Movimento = dc.Data_Movimento,
                                                                                        .Doc_Numero = dc.Doc_Numero,
                                                                                        .Doc_Numero_Des = dc.Doc_Numero_Des,
                                                                                        .Doc_Numero_Sin = dc.Doc_Numero_Sin,
                                                                                        .Id_Agenda_Rif = dc.Id_Agenda_Rif,
                                                                                        .Ordine_Det = dc.Ordine_Det
                                                                                      }
                                                                                                     End Function).ToList(),
                                            .Riepilogo = fatturaGiasFromDb.RiepilogoDocumentiCollegati.Select(Function(dr)
                                                                                                                  Return New DocumentoCollegatiRiepilogoMap With
                                                                                            {
                                                                                                .Id_Agenda = dr.Id_Agenda,
                                                                                                .NumMovimenti = dr.totalCount
                                                                                            }
                                                                                                              End Function).ToList()
                                        }
            }

            ' legale rappresentante
            If Not fatturaGiasFromDb.LegaleRappresentante Is Nothing Then
                result.LegaleRappresentante = New LegaleRappresentanteMap With
                                                    {
                                                         .Anagrafica = New AnagraficaMap With
                                                            {
                                                                .Cognome = fatturaGiasFromDb.LegaleRappresentante.Cognome,
                                                                .Nome = fatturaGiasFromDb.LegaleRappresentante.Nome,
                                                                .rag_soc = fatturaGiasFromDb.LegaleRappresentante.Rag_Soc,
                                                                .PIVA = fatturaGiasFromDb.LegaleRappresentante.Piva,
                                                                .Codice_Fiscale = fatturaGiasFromDb.LegaleRappresentante.Codice_Fiscale,
                                                                .Cod_Contatto = fatturaGiasFromDb.LegaleRappresentante.Cod_Contatto,
                                                                .Id_CF = fatturaGiasFromDb.LegaleRappresentante.Id_CF
                                                            }
                                                    }
            End If

            ' riferimento nota credito / debito
            If Not fatturaGiasFromDb.RifNotaCreditoDebito Is Nothing Then
                result.RifNotaCreditoDebito = New RifNotaCreditoDebitoMap With
                                        {
                                            .N_Nota_DDT = fatturaGiasFromDb.RifNotaCreditoDebito.N_Nota_DDT,
                                            .Data_Nota_DDT = fatturaGiasFromDb.RifNotaCreditoDebito.Data_Nota_DDT,
                                            .N_Nota_Fattura = fatturaGiasFromDb.RifNotaCreditoDebito.N_Nota_Fattura,
                                            .Data_Nota_Fattura = fatturaGiasFromDb.RifNotaCreditoDebito.Data_Nota_Fattura
                                        }

            End If

            'dati Pagamenti
            If Not fatturaGiasFromDb.DatiPagamento Is Nothing AndAlso fatturaGiasFromDb.DatiPagamento.Any() Then
                result.Pagamenti = fatturaGiasFromDb.DatiPagamento.Select(Function(dp)
                                                                              Return New PagamentoMap With
                                                                              {
                                                                                    .DatiPagamento = New DatiPagamentoMap With
                                                                                    {
                                                                                        .Cau_Pagamento = dp.DatiPagamento.Cau_Pagamento,
                                                                                        .Cod_Liquidita_Avere = dp.DatiPagamento.Cod_Liquidita_Avere,
                                                                                        .Cod_Liquidita_Dare = dp.DatiPagamento.Cod_Liquidita_Dare,
                                                                                        .DataScadenza_Manuale = dp.DatiPagamento.DataScadenza_Manuale,
                                                                                        .Data_Pagamento = dp.DatiPagamento.Data_Pagamento,
                                                                                        .Importo = dp.DatiPagamento.Importo,
                                                                                        .Percentuale = dp.DatiPagamento.Percentuale,
                                                                                        .Previsto_Avvenuto = dp.DatiPagamento.Previsto_Avvenuto
                                                                                    },
                                                                                    .CausalePagamento = New CausalePagamentoMap With
                                                                                    {
                                                                                        .Cau_Pagamento_Des = dp.CausalePagamento.Cau_Pagamento_Des,
                                                                                        .Cau_Pagamento_Sigla = dp.CausalePagamento.Cau_Pagamento_Sigla,
                                                                                        .Tipo = dp.CausalePagamento.Tipo
                                                                                    },
                                                                                    .DatiBancari = New DatiBancariMap With
                                                                                    {
                                                                                        .Abi = dp.DatiBancari.Abi,
                                                                                        .Bic = dp.DatiBancari.Bic,
                                                                                        .Cab = dp.DatiBancari.Cab,
                                                                                        .Cifre_Controllo = dp.DatiBancari.Cifre_Controllo,
                                                                                        .Cin = dp.DatiBancari.Cin,
                                                                                        .Istituto_Des = dp.DatiBancari.Istituto_Des,
                                                                                        .Nazione = dp.DatiBancari.Nazione,
                                                                                        .Numero = dp.DatiBancari.Numero
                                                                                    }
                                                                              }
                                                                          End Function).ToList()
            End If


            'imposta flagPa (fattura a pubblica amministrazione)
            result.FlagPa = result.ImpostaFlagPA()

            'aggancia il regime fiscale del cedente (pre-caricati) in base al sezionale della testata fattura
            result.Cedente.DatiPrincipali.Anagrafica.CodRegimeFiscale = mapper.RegimeFiscaleXSezionale(
                    result.Cedente.DatiPrincipali.Anagrafica.PIVA, result.Fattura.Testata.Sezionale_Cod)

            ' aggancia i codici aggiuntivi del contatto pre-caricati
            result.Cessionario.DatiAggiuntivi =
                mapper.ContattoCodiciPerPIVA(result.Cessionario.DatiPrincipali.Anagrafica.PIVA, result.Cessionario.DatiPrincipali.Anagrafica.Cod_Contatto).Select(Function(x) _
                    New DatoAggiuntivoMap With {
                        .id_cod = x.Id_cod,
                        .val_cod = x.Val_cod
                })

            result.Cessionario.DatiPrincipali.IDSDI = result.Cessionario.OttieniDatoAggiuntivo(enum_CodiciAnagrafe.CodiceSDI)
            result.Cessionario.DatiPrincipali.PEC = result.Cessionario.OttieniDatoAggiuntivo(enum_CodiciAnagrafe.PecContatto)

            ' aggancia stabile organizzazione cessionario se esiste
            If result.Cessionario.DatiPrincipali.Anagrafica.Id_CF = enum_Contatti_IdCf.ContattoEstero Then

                Dim so = mapper.DecodificheMapper.OttieniStabileOrganizzazione(
                            result.Cessionario.DatiPrincipali.Anagrafica.Cod_Contatto, result.Cessionario.DatiPrincipali.Cod_RisUm)

                If Not so Is Nothing Then
                    result.Cessionario.StabileOrganizzazione = New StabileOrganizzazioneMap With
                    {
                        .Cod_Contatto = so.Cod_Contatto,
                        .Cod_RisUm = so.Cod_RisUm,
                        .Indirizzo = New IndirizzoMap With
                            {
                                .CAP = so.Indirizzo.CAP,
                                .com_cod_istat = so.Indirizzo.com_cod_istat,
                                .com_des = so.Indirizzo.com_des,
                                .frz_des = so.Indirizzo.frz_des,
                                .ind_des = so.Indirizzo.ind_des,
                                .pro_cod = so.Indirizzo.pro_cod,
                                .pro_cod_istat = so.Indirizzo.pro_cod_istat,
                                .stato = so.Indirizzo.stato,
                                .Tipo_Indirizzo = so.Indirizzo.Tipo_Indirizzo
                            }
                    }

                    If StabileOrganizzazioneVuota(result.Cessionario.StabileOrganizzazione.Indirizzo) Then
                        result.Cessionario.StabileOrganizzazione = Nothing
                    End If

                End If

            End If

            result.Fattura.PIVA = fatturaGiasFromDb.Cedente.DatiPrincipali.PIVA
            result.Fattura.NumeroFattura = result.OttieniNumeroFattura()
            result.Fattura.TipoFattura = result.OttieniTipoDocumento(mapper.DecodificheMapper).ToString()
            result.Fattura.Codice_RisUM_Cessionario = fatturaGiasFromDb.DatiFattura.Testata.Cod_RisUm

            Return result

        Catch ex As Exception
            Throw New DBToEntityMapException(ex.Message)
        End Try

    End Function

    Private Function StabileOrganizzazioneVuota(ByVal indirizzo As IndirizzoMap) As Boolean

        If String.IsNullOrEmpty(indirizzo.CAP) AndAlso
                String.IsNullOrEmpty(indirizzo.ind_des) AndAlso
                String.IsNullOrEmpty(indirizzo.com_des) AndAlso
                (String.IsNullOrEmpty(indirizzo.pro_cod) OrElse indirizzo.pro_cod = "0" OrElse indirizzo.pro_cod = "00") Then
            Return True
        End If

        Return False

    End Function
    Public Function NormalizzaPIVA(ByVal PIVA As String, ByVal codicePaese As String) As String

        If PIVA.StartsWith(codicePaese) Then
            Return PIVA.Substring(2)
        Else
            Return PIVA
        End If

    End Function

    <Extension()>
    Public Function OttieniFormatoTrasmissione(ByVal fattura As FatturaGias) As FatturaPa.FormatoTrasmissioneType
        Return If(fattura.FlagPa, FatturaPa.FormatoTrasmissioneType.FPA12, FatturaPa.FormatoTrasmissioneType.FPR12)
    End Function

    <Extension()>
    Public Function CessionarioTitolarePIVA(ByVal fatGias As FatturaGias) As Boolean
        Return fatGias.Cessionario.DatiPrincipali.Anagrafica.Id_CF <> enum_Contatti_IdCf.PersonaFisica
    End Function

    <Extension()>
    Public Function OttieniPivaCessionario(ByVal fatGias As FatturaGias) As String

        If fatGias.Cessionario.DatiPrincipali.Anagrafica.Id_CF = enum_Contatti_IdCf.PersonaFisica Then
            Return String.Empty
        End If

        Return fatGias.Cessionario.DatiPrincipali.Anagrafica.Cod_Contatto

    End Function

    <Extension()>
    Public Function OttieniCFCessionario(ByVal fatGias As FatturaGias) As String

        If fatGias.Cessionario.DatiPrincipali.Anagrafica.Id_CF <> enum_Contatti_IdCf.PersonaFisica Then
            Return String.Empty
        End If

        Return fatGias.Cessionario.DatiPrincipali.Anagrafica.Cod_Contatto

    End Function

    <Extension()>
    Public Function ImpostaFlagPA(ByVal fatGias As FatturaGias) As Boolean

        Dim flagPa As Boolean = False

        Dim pa = fatGias.Cessionario.OttieniDatoAggiuntivo(enum_CodiciAnagrafe.TipoContattoFattura)
        If String.IsNullOrEmpty(pa) Then
            Return flagPa
        End If

        Dim result As enumTipoContattoFattura
        If [Enum].TryParse(pa, result) Then
            If result = enumTipoContattoFattura.PA Then
                flagPa = True
            End If
        End If

        Return flagPa

    End Function

    <Extension()>
    Public Sub ImpostaFlagPA(ByVal fattura As FatturaGiasFromDB)

        Dim pa = fattura.Cessionario.OttieniDatoAggiuntivo(enum_CodiciAnagrafe.TipoContattoFattura)
        If String.IsNullOrEmpty(pa) Then
            fattura.FlagPa = False
        End If

        Dim result As enumTipoContattoFattura
        If [Enum].TryParse(pa, result) Then
            If result = enumTipoContattoFattura.PA Then
                fattura.FlagPa = True
            End If
        End If

    End Sub

    <Extension()>
    Public Function OttieniNomeFileXML(ByVal fatGias As FatturaGias) As String

        Dim result As String = "IT{0}_{1}.xml"

        Dim progressivo = fatGias.ProgressivoFattura.ToString().PadLeft(5, "0")
        If fatGias.FlagPa Then
            Return String.Format(result, fatGias.Cedente.DatiPrincipali.Anagrafica.PIVA, progressivo)
        Else
            Return String.Format(result, fatGias.Cedente.DatiPrincipali.Anagrafica.Codice_Fiscale, progressivo)
        End If

    End Function

    <Extension()>
    Public Function VerifichePreliminari(ByVal fatGias As FatturaGias, ByRef errori As String) As Boolean


        Dim messaggioErrore As String = ""
        Dim nomeRoutine As String = "VerifichePreliminari"

        Try

            ''''' CEDENTE '''''

            ' PIva e CF vuoti
            If String.IsNullOrEmpty(fatGias.Cedente.DatiPrincipali.Anagrafica.Codice_Fiscale) _
                AndAlso String.IsNullOrEmpty(fatGias.Cedente.DatiPrincipali.Anagrafica.PIVA) Then

                Throw New Exception()
            End If

            ''''' CESSIONARIO '''''
            ' Pec e SDI vuoti
            'If String.IsNullOrEmpty(fatGias.Cessionario.) AndAlso String.IsNullOrEmpty(fatGias.CodiceFiscale) Then

            '    Throw New Exception()
            'End If


        Catch ex As Exception
            messaggioErrore = ex.Message.ToString & If(Not IsNothing(ex.InnerException), " (" & ex.InnerException.Message.ToString & ")", "")
            errori &= "[" & nomeRoutine & "] : " & messaggioErrore & "</br>"
            Return False
        End Try

        Return True

    End Function
    <Extension()>
    Public Function NormalizzaStato(ByVal indirizzo As IndirizzoMap, ByVal id_cf As enum_Contatti_IdCf) As String
        Return NormalizzaStato(indirizzo.stato, id_cf)
    End Function

    <Extension()>
    Public Function NormalizzaStato(ByVal indirizzo As Indirizzo, ByVal id_cf As enum_Contatti_IdCf) As String
        Return NormalizzaStato(indirizzo.stato, id_cf)
    End Function

    Private Function NormalizzaStato(ByVal stato As String, ByVal id_cf As enum_Contatti_IdCf) As String

        If String.IsNullOrEmpty(stato) AndAlso id_cf <> enum_Contatti_IdCf.ContattoEstero Then
            Return "IT"
        End If

        Dim italia As String() = {"IT", "ITALIA", "ITALY"}

        If italia.Contains(stato.ToUpperInvariant()) Then
            Return "IT"
        Else
            Return stato.ToUpperInvariant()
        End If

    End Function

    <Extension()>
    Public Function NormalizzaProvincia(ByVal indirizzo As IndirizzoMap, ByVal mapper As IDecodificheMapper) As String

        If Not String.IsNullOrEmpty(indirizzo.pro_cod) Then
            Return indirizzo.pro_cod
        End If

        Dim prov = mapper.OttieniProvincia(indirizzo.pro_cod_istat)
        If Not prov Is Nothing Then
            Return prov.SIGLA.ToUpper()
        End If

        Return String.Empty

    End Function

    <Extension()>
    Public Function NormalizzaComune(
                ByVal indirizzo As IndirizzoMap,
                ByVal id_cf As enum_Contatti_IdCf,
                ByVal mapper As IDecodificheMapper) As String

        Dim descrizioneComune = String.Empty

        If Not String.IsNullOrEmpty(indirizzo.com_des) Then
            descrizioneComune = indirizzo.com_des
        End If

        Dim comune = mapper.OttieniComune(indirizzo.pro_cod_istat, indirizzo.com_cod_istat)
        If comune IsNot Nothing Then
            descrizioneComune = comune.LOCALITA
        End If

        If id_cf = enum_Contatti_IdCf.ContattoEstero Then
            Return indirizzo.frz_des
        Else
            If descrizioneComune.Trim().ToLower() = indirizzo.frz_des.Trim().ToLower() Then
                Return descrizioneComune
            Else
                Return String.Format("{0} {1}", descrizioneComune, indirizzo.frz_des)
            End If
        End If


    End Function

    <Extension()>
    Public Function OttieniCodiceDestinatario(ByVal fattura As FatturaGias) As String

        If Not String.IsNullOrEmpty(fattura.Cessionario.DatiPrincipali.IDSDI) Then
            Return fattura.Cessionario.DatiPrincipali.IDSDI.ToUpper
        Else

            If fattura.FlagPa Then
                Return "999999"
            ElseIf fattura.Cessionario.DatiPrincipali.Anagrafica.Id_CF <> enum_Contatti_IdCf.ContattoEstero Then
                Return "0000000"
            Else
                Return "XXXXXXX"
            End If
        End If

        'If fattura.FlagPa Then
        '    If String.IsNullOrEmpty(fattura.Cessionario.DatiPrincipali.IDSDI) Then
        '        Return "999999"
        '    Else
        '        Return fattura.Cessionario.DatiPrincipali.IDSDI
        '    End If

        'End If

        'If Not fattura.Cessionario.DatiPrincipali.Anagrafica.Id_CF = enum_Contatti_IdCf.ContattoEstero Then

        '    If Not String.IsNullOrEmpty(fattura.Cessionario.DatiPrincipali.IDSDI) Then
        '        Return fattura.Cessionario.DatiPrincipali.IDSDI
        '    Else
        '        Return "0000000"
        '    End If
        'Else
        '    Return "XXXXXXX"
        'End If

    End Function
    <Extension()>
    Public Function OttieniSocioUnico(ByVal fattura As FatturaGias) As FatturaPa.SocioUnicoType

        Dim formaSociale = fattura.Cedente.OttieniDatoAggiuntivo(enum_CodiciAnagrafe.TipoSocieta)
        If String.IsNullOrEmpty(formaSociale) Then
            Return Int32.MinValue
        End If

        If formaSociale.ToUpper() <> enumTipoSocieta.SRL.ToString().ToUpper() Then
            Return Int32.MinValue
        End If

        Dim socioUnico = fattura.Cedente.OttieniDatoAggiuntivo(enum_CodiciAnagrafe.NumeroSoci)
        If String.IsNullOrEmpty(socioUnico) Then
            Return Int32.MinValue
        End If

        Dim result As FatturaPa.SocioUnicoType
        If [Enum].TryParse(socioUnico, result) Then
            Return result
        Else
            Return Int32.MinValue
        End If

    End Function

    <Extension()>
    Public Function OttieniStatoLiquidazione(ByVal fattura As FatturaGias) As FatturaPa.StatoLiquidazioneType

        Dim statoLiquidazione = fattura.Cedente.OttieniDatoAggiuntivo(enum_CodiciAnagrafe.StatoLiquidazione)
        If String.IsNullOrEmpty(statoLiquidazione) Then
            Return Int32.MinValue
        End If

        Dim result As FatturaPa.StatoLiquidazioneType
        If [Enum].TryParse(statoLiquidazione, result) Then
            Return result
        Else
            Return Int32.MinValue
        End If

    End Function

    <Extension()>
    Public Function OttieniCapitaleSociale(ByVal fattura As FatturaGias) As Decimal

        Dim socCap As String() = {enumTipoSocieta.SAPA.ToString(), enumTipoSocieta.SPA.ToString(), enumTipoSocieta.SRL.ToString()}
        Dim formaSociale = fattura.Cedente.OttieniDatoAggiuntivo(enum_CodiciAnagrafe.TipoSocieta)
        Dim returnValue As Decimal = Decimal.MinusOne

        If socCap.Contains(formaSociale) Then
            Dim valCapSoc = fattura.Cedente.OttieniDatoAggiuntivo(enum_CodiciAnagrafe.CapitaleSociale)
            If Not String.IsNullOrEmpty(valCapSoc) Then
                returnValue = CDec(fattura.Cedente.OttieniDatoAggiuntivo(enum_CodiciAnagrafe.CapitaleSociale))
            End If
            Return returnValue
        Else
            Return Decimal.MinValue
        End If

    End Function

    <Extension()>
    Public Function OttieniTipoDocumento(ByVal fattura As FatturaGias, ByVal mapper As IDecodificheMapper) As FatturaPa.TipoDocumentoType

        'Leggo prima il valore preciso
        Dim tipoDoc As Integer = If(fattura.Fattura.Testata.TipoDocumento, 0)

        If tipoDoc <> 0 Then
            Return mapper.DecodificaTipoDocumento(tipoDoc)
        Else

            'Fallback per documenti precedenti dove non era valorizzato
            'If Autofattura(fattura) Then
            '    Return FatturaPa.TipoDocumentoType.TD20
            'End If

            Select Case fattura.Fattura.Testata.Lav_cod
                Case LAVCOD_NOTA_ACCREDITO_EMESSA
                    Return FatturaPa.TipoDocumentoType.TD04
                Case LAVCOD_FATTURA_EMESSA
                    Return FatturaPa.TipoDocumentoType.TD01
                Case Else
                    Return FatturaPa.TipoDocumentoType.TD01
            End Select
        End If

    End Function

    <Extension()>
    Public Function ClienteEstero(ByVal fattura As FatturaGias) As Boolean

        Return If(fattura.Cessionario.DatiPrincipali.Anagrafica.Id_CF = enum_Contatti_IdCf.ContattoEstero, True, False)

    End Function
    <Extension()>
    Public Function OttieniTipoDocumento(ByVal fattura As FatturaGiasFromDB, ByVal mapper As IDecodificheMapper) As FatturaPa.TipoDocumentoType

        'Leggo prima il valore preciso
        Dim tipoDoc As Integer = If(fattura.DatiFattura.Testata.TipoDocumento, 0)
        
        If tipoDoc <> 0 Then
            Return mapper.DecodificaTipoDocumento(tipoDoc)
        Else

            'Fallback per documenti precedenti dove non era valorizzato
            'If Autofattura(fattura) Then
            '    Return FatturaPa.TipoDocumentoType.TD20
            'End If

            Select Case fattura.DatiFattura.Testata.Lav_cod.ToString()
                Case LAVCOD_NOTA_ACCREDITO_EMESSA
                    Return FatturaPa.TipoDocumentoType.TD04
                Case LAVCOD_FATTURA_EMESSA
                    Return FatturaPa.TipoDocumentoType.TD01
                Case Else
                    Return FatturaPa.TipoDocumentoType.TD01
            End Select
        End If
    End Function

    <Extension()>
    Public Function OttieniPrezzoTotaleDettaglio(ByVal dettaglio As Movimenti_dettagli,
                                                 ByVal lavCod As Integer,
                                                 ByVal helper As AgronicaCoreContabHLP.Contabilita) As Decimal

        If dettaglio.Imponibile_Netto.HasValue Then
            Return CDec(NormalizzaImporto(helper.Leggi_Imponibile_PositivoNegativo(lavCod, dettaglio.Imponibile_Netto.Value), 8))
        End If
        Return NormalizzaImporto(0, 8)

    End Function
    Private Function Autofattura(ByVal fattura As FatturaGias) As Boolean
        Dim comparer As New AnagraficaComparer()
        Return comparer.Equals(fattura.Cedente.DatiPrincipali.Anagrafica, fattura.Cessionario.DatiPrincipali.Anagrafica)
    End Function
    Private Function Autofattura(ByVal fattura As FatturaGiasFromDB) As Boolean
        Return fattura.Cedente.DatiPrincipali.Codice_Fiscale.Equals(fattura.Cessionario.Codice_Fiscale)
    End Function
    <Extension()>
    Public Function OttieniNumeroFattura(ByVal fattura As FatturaGias) As String

        Return OttieniNumeroFattura(fattura.Fattura.Testata.Doc_Numero_Sin,
                                    fattura.Fattura.Testata.Doc_Numero,
                                    fattura.Fattura.Testata.Doc_Numero_Des)

    End Function

    <Extension()>
    Public Function OttieniNumeroFattura(ByVal fattura As FatturaGiasFromDB) As String

        Return OttieniNumeroFattura(fattura.DatiFattura.Testata.Doc_Numero_Sin,
                                    fattura.DatiFattura.Testata.Doc_Numero,
                                    fattura.DatiFattura.Testata.Doc_Numero_Des)

    End Function

    <Extension()>
    Public Function OttieniNumeroFattura(ByVal documento As DocumentoCollegatoMap) As String

        Return OttieniNumeroFattura(documento.Doc_Numero_Sin,
                                    documento.Doc_Numero,
                                    documento.Doc_Numero_Des)

    End Function
    Private Function OttieniNumeroFattura(ByVal Doc_Numero_Sin As String, ByVal Doc_Numero As String, ByVal Doc_Numero_Des As String) As String

        Return String.Format("{0}{1}{2}", Doc_Numero_Sin, Doc_Numero, Doc_Numero_Des)

    End Function

    <Extension()>
    Public Function OttieniImportoTotaleDocumento(ByVal fattura As FatturaGias) As Decimal
        Return NormalizzaImporto(fattura.Fattura.Testata.TotaleFattura, 8)
    End Function

    Public Function IsValid(ByVal fattura As Entita.FatturaPa.FatturaElettronicaType) As Boolean

        Return RecursiveIsValid(fattura)

    End Function

    <Extension()>
    Public Function IsValid(ByVal fattura As FatturaGias, ByVal mapper As IDecodificheMapper, ByVal errori As List(Of String)) As Boolean


        Dim validatori = New List(Of ValidationAttribute)
        OttieniValidatori(fattura, validatori, mapper)

        Dim results = New List(Of Tuple(Of Boolean, String))

        validatori.OrderBy(Function(v) v.Order).ToList().ForEach(Sub(v)
                                                                     Dim result = v.Validate(errori, fattura)
                                                                     Dim vName = v.Name
                                                                     results.Add(New Tuple(Of Boolean, String)(result, vName))
                                                                 End Sub)

        Return results.All(Function(r) r.Item1 = True) AndAlso errori.Count() = 0

    End Function

    Private Sub OttieniValidatori(ByVal oggetto As Object, ByRef validatori As List(Of ValidationAttribute), ByVal mapper As IDecodificheMapper)

        Dim retValue As Boolean = True

        Dim props = oggetto.GetType().GetFields()

        For Each p As FieldInfo In props

            Dim attr = p.GetCustomAttributes(GetType(ValidationAttribute), False).FirstOrDefault()
            If Not attr Is Nothing Then
                Dim va = DirectCast(attr, ValidationAttribute)
                va.ObjectToValidate = p.GetValue(oggetto)
                va.Mapper = mapper
                validatori.Add(va)
            End If

            If Not IsSimpleType(p.FieldType) Then
                Dim subObject = p.GetValue(oggetto)
                If Not subObject Is Nothing Then
                    OttieniValidatori(subObject, validatori, mapper)
                End If

            End If
        Next

    End Sub


    <Extension()>
    Public Function NormalizzaImporto(ByVal importo As Decimal, ByVal maxDec As Integer) As Decimal

        Dim decimalCount = BitConverter.GetBytes(Decimal.GetBits(importo)(3))(2)
        If decimalCount > maxDec Then
            decimalCount = maxDec
        End If
        If decimalCount < 2 Then decimalCount = 2
        Dim n As String = String.Format("N{0}", decimalCount)

        Return CDec(importo.ToString(n))

    End Function

    <Extension()>
    Public Function IsAssociazione(ByVal cod_contatto As String) As Boolean

        If VerificaEspressioneRegolare(cod_contatto, "", enum_EspressioniRegolari.RegExp_PartitaIVA) _
            AndAlso (cod_contatto.StartsWith("8") Or cod_contatto.StartsWith("9")) Then
            Return True
        Else
            Return False
        End If

    End Function

    <Extension()>
    Public Function LivellaPrezzo(ByVal prodottoServizio As ProdottoServizioMap,
                                  ByVal udm_descrizione As String,
                                  ByVal moduloGenerazione As enum_Omni_Modulo_Generazione,
                                  ByVal mapper As IDecodificheMapper) As LivelloPrezzo

        Dim lp As LivelloPrezzo = Nothing

        If moduloGenerazione = enum_Omni_Modulo_Generazione.FreshFood Then
            lp = LivellaPrezzoFF(prodottoServizio, udm_descrizione, mapper)
        Else
            lp = LivellaPrezzoNonFF(prodottoServizio, udm_descrizione, mapper)
        End If

        Return New LivelloPrezzo With {.UDM = lp.UDM, .Qta = NormalizzaImporto(lp.Qta, 8), .Prezzo_Unitario = NormalizzaImporto(lp.Prezzo_Unitario, 8)}

    End Function

    Private Function LivellaPrezzoFF(
                                    ByVal prodottoServizio As ProdottoServizioMap,
                                    ByVal udm_descrizione As String,
                                    ByVal mapper As IDecodificheMapper) As LivelloPrezzo

        Dim movimento = prodottoServizio.Movimento

        Select Case movimento.Prezzo_Livello
            Case enum_OTabelle.Confezione
                Return New LivelloPrezzo With {.UDM = "n", .Qta = movimento.Qta, .Prezzo_Unitario = movimento.Prezzo_Unitario}
            Case enum_OTabelle.Contenitore
                Return New LivelloPrezzo With {.UDM = "n", .Qta = movimento.Qta_Dettaglio1, .Prezzo_Unitario = movimento.Prezzo_Unitario}
            Case enum_OTabelle.Imballaggio
                Return New LivelloPrezzo With {.UDM = "n", .Qta = movimento.Qta_Dettaglio2, .Prezzo_Unitario = movimento.Prezzo_Unitario}
            Case enum_OTabelle.Nessuno
                Return New LivelloPrezzo With {.UDM = udm_descrizione, .Qta = movimento.Qta, .Prezzo_Unitario = movimento.Prezzo_Unitario}
            Case -1

                Dim udmCodDaDecodificare = ""
                If movimento.UDM_COD_EXTRA = 0 Then
                    If movimento.Udm_Cod <> enum_UnitaMisura.Litri AndAlso movimento.Udm_Cod <> enum_UnitaMisura.KG Then
                        Throw New GiasEntityToEFatturaMapException("Impossibile decodificare l'UDM (FF) ")
                    Else
                        udmCodDaDecodificare = udm_descrizione
                    End If
                Else
                    udmCodDaDecodificare = mapper.DecodificaUnitaMisura(movimento.UDM_COD_EXTRA)
                End If

                Return New LivelloPrezzo With
                   {
                       .UDM = udmCodDaDecodificare,
                       .Qta = movimento.Qta * movimento.QTA_EXTRA,
                       .Prezzo_Unitario = movimento.Prezzo_Effettivo
                   }
            Case Else
                Throw New GiasEntityToEFatturaMapException("Livello Prezzo non gestito")

        End Select

    End Function

    Private Function LivellaPrezzoNonFF(
                ByVal prodottoServizio As ProdottoServizioMap,
                ByVal udm_descrizione As String,
                ByVal mapper As IDecodificheMapper)

        Dim movimento = prodottoServizio.Movimento

        If prodottoServizio.Flag_Extra = True Then
            Return New LivelloPrezzo With
                {
                    .UDM = IIf(String.IsNullOrEmpty(movimento.UDM_COD_EXTRA), udm_descrizione, mapper.DecodificaUnitaMisura(movimento.UDM_COD_EXTRA)),
                    .Qta = movimento.Qta_Extra_Totale,
                    .Prezzo_Unitario = movimento.Prezzo_Effettivo
                }
        Else
            Return New LivelloPrezzo With
                {
                    .UDM = udm_descrizione,
                    .Qta = movimento.Qta,
                    .Prezzo_Unitario = movimento.Prezzo_Unitario
                }
        End If


    End Function
    Private Function RecursiveIsValid(ByVal oggetto As Object) As Boolean

        Dim retValue As Boolean = True

        Dim props = oggetto.GetType().GetProperties()

        For Each p As PropertyInfo In props

            Dim attr = p.GetCustomAttributes(False).ToList()
            attr.Add(New ValidationAttribute())

            If Not IsSimpleType(p.PropertyType) Then

                Console.WriteLine(p.Name)
                Dim subObject = p.GetValue(oggetto, Nothing)
                If Not subObject Is Nothing Then
                    retValue = RecursiveIsValid(subObject)
                End If

            Else

                If p.PropertyType.IsEnum() Then
                    Dim value = p.GetValue(oggetto, Nothing)
                    If Not value Is Nothing AndAlso Convert.ToInt32(value) = Int32.MinValue Then
                        Console.WriteLine(String.Format("{0} {1}", p.Name, p.PropertyType.Name))
                        retValue = False
                    End If

                End If

            End If
        Next

        Return retValue

    End Function

    Private Function IsSimpleType(ByVal type As Type) As Boolean
        Return type.IsValueType _
            OrElse type.IsPrimitive _
            OrElse New Type() {
                GetType(String), GetType(Decimal), GetType(DateTime),
                GetType(DateTimeOffset), GetType(TimeSpan), GetType(Guid)}.Contains(type) _
                OrElse Convert.GetTypeCode(type) <> TypeCode.Object
    End Function

End Module

