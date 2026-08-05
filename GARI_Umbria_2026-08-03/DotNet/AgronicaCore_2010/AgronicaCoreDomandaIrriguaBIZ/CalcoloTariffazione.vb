Imports AgronicaCoreDTOStd.InData.DomandaIrrigua
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDomandaIrriguaDAL

Public Class CalcoloTariffazione
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function EseguiCalcolo(ByVal parametri As AgronicaCoreDTOStd.InData.DomandaIrrigua.CalcoloTariffazioneParametri,
                                  ByRef ObjParametri As AgronicaCoreParametri) As RisultatoTariffazione
        Dim ret As New RisultatoTariffazione

        Try
            Dim DaData As DateTime = New DateTime(parametri.anno, 1, 1, 0, 0, 0)
            Dim AData As DateTime = New DateTime(parametri.anno, 12, 31, 23, 59, 59)

            Dim aux As New AgronicaCoreDomandaIrriguaBIZ.DatiAggiuntiviDocumenti_R

            '0- recupero l'elendo delle aziende e relativa domanda da considerare nel calcolo
            Dim ElencoDomande = RecuperoElencoAziendeConDomandaIrrigua(DaData, AData, ObjParametri)
            If ElencoDomande.Count <= 0 Then
                Throw New Exception(String.Format("Nessuna domanda valida trovata nel periodo {0} - {1}", DaData, AData))
            End If

            '1- Recupero Totale Volume e Totale Superficie
            Dim TotVol = RecuperoTotaleVolume(ElencoDomande, DaData, AData, ObjParametri)
            Dim TotSup = RecuperoTotaleSuperficie(ElencoDomande, ObjParametri)

            Dim tmpElenco As New List(Of ElencoDomandeAziende)

            ret.tariffazioneAzienda = New List(Of RisultatoTariffazioneAzienda)

            '2- elaborazione dati per singola azienda
            For Each domanda In ElencoDomande
                tmpElenco.Clear()
                tmpElenco.Add(New ElencoDomandeAziende() With {
                                .id_domanda = domanda.id_domanda,
                                .piva = domanda.piva
                              })
                Dim resAzienda As New RisultatoTariffazioneAzienda
                resAzienda.RagioneSociale = ""
                resAzienda.cuaa = ""
                resAzienda.CodiceSDI = ""

                Dim DatiAzienda = aux.GetDatiAzienda(domanda.piva, ObjParametri)
                resAzienda.PivaReale = domanda.piva
                If DatiAzienda IsNot Nothing Then
                    resAzienda.PivaReale = DatiAzienda.PivaReale
                    resAzienda.RagioneSociale = DatiAzienda.RagioneSociale
                    resAzienda.cuaa = DatiAzienda.CUAA
                End If


                resAzienda.piva = domanda.piva
                resAzienda.IdDomanda = domanda.id_domanda

                resAzienda.SuperficieTotale = RecuperoTotaleSuperficie(tmpElenco, ObjParametri)
                If resAzienda.SuperficieTotale < 1 Then
                    resAzienda.SuperficieTotale = 1
                End If
                resAzienda.VolumeTotale = RecuperoTotaleVolume(tmpElenco, DaData, AData, ObjParametri)
                resAzienda.Incidenza = Math.Round(resAzienda.VolumeTotale / resAzienda.SuperficieTotale, 5)

                If resAzienda.Incidenza <= 1500 Then
                    resAzienda.IncidenzaFasciaT1 = resAzienda.Incidenza
                    resAzienda.IncidenzaFasciaT2 = 0
                    resAzienda.IncidenzaFasciaT3 = 0
                End If
                If resAzienda.Incidenza > 1500 And resAzienda.Incidenza <= 3000 Then
                    resAzienda.IncidenzaFasciaT1 = 1500
                    resAzienda.IncidenzaFasciaT2 = resAzienda.Incidenza - 1500
                    resAzienda.IncidenzaFasciaT3 = 0
                End If
                If resAzienda.Incidenza > 3000 Then
                    resAzienda.IncidenzaFasciaT1 = 1500
                    resAzienda.IncidenzaFasciaT2 = 1500
                    resAzienda.IncidenzaFasciaT3 = resAzienda.Incidenza - 3000
                End If

                resAzienda.VolumeFasciaT1 = resAzienda.IncidenzaFasciaT1 * resAzienda.SuperficieTotale
                resAzienda.VolumeFasciaT2 = resAzienda.IncidenzaFasciaT2 * resAzienda.SuperficieTotale
                resAzienda.VolumeFasciaT3 = resAzienda.IncidenzaFasciaT3 * resAzienda.SuperficieTotale

                resAzienda.QuotaFissa = parametri.spesaquotafissa

                ret.tariffazioneAzienda.Add(resAzienda)
            Next

            Dim TotVolT2 As Decimal = 0
            Dim TotVolT3 As Decimal = 0

            For Each res In ret.tariffazioneAzienda
                TotVolT2 += res.VolumeFasciaT2
                TotVolT3 += res.VolumeFasciaT3
            Next

            Dim CostoFasciaT1 As Decimal = CalcolaImportoSpesaT1(TotSup, TotVol, parametri.spesaquotafissa, TotVolT2, parametri.incrementofascia2, TotVolT3, (parametri.incrementofascia2 + parametri.incrementofascia3), parametri.spesatotale)
            For Each res In ret.tariffazioneAzienda
                res.QuotaVariabileT1 = Math.Round(CostoFasciaT1, 5)
                res.QuotaVariabileT2 = Math.Round(CostoFasciaT1 + parametri.incrementofascia2, 5)
                res.QuotaVariabileT3 = Math.Round(CostoFasciaT1 + parametri.incrementofascia2 + parametri.incrementofascia3, 5)

                res.ImponibileQuotaFissa = Math.Round(res.SuperficieTotale * res.QuotaFissa, 2)
                res.ImponibileQuotaVariabileT1 = Math.Round(res.VolumeFasciaT1 * res.QuotaVariabileT1, 2)
                res.ImponibileQuotaVariabileT2 = Math.Round(res.VolumeFasciaT2 * res.QuotaVariabileT2, 2)
                res.ImponibileQuotaVariabileT3 = Math.Round(res.VolumeFasciaT3 * res.QuotaVariabileT3, 2)

                res.TotaleImponibile = Math.Round(res.ImponibileQuotaFissa + res.ImponibileQuotaVariabileT1 + res.ImponibileQuotaVariabileT2 + res.ImponibileQuotaVariabileT3, 2)
            Next

        Catch ex As Exception
            Throw New Exception(ex.Message, ex.InnerException)
        End Try
        Return ret
    End Function

    Private Function RecuperoTotaleVolume(ByVal ElencoDomande As List(Of ElencoDomandeAziende),
                                          ByVal DaData As DateTime,
                                          ByVal AData As DateTime,
                                          ByRef ObjParametri As AgronicaCoreParametri) As Decimal
        Dim Totale As Decimal = 0
        Try
            Dim lca_biz As New AgronicaCoreDomandaIrriguaBIZ.LettureContatoriAziendali_R
            For Each domanda In ElencoDomande
                'lettura max
                Dim letture = lca_biz.LeggiLettureContatore(domanda.piva, 0, DaData, AData, ObjParametri)
                If letture.elencoLetture.Count > 0 Then
                    Dim MaxDate = letture.elencoLetture.Max(Function(x) x.datalettura)
                    Dim MinDate = letture.elencoLetture.Min(Function(x) x.datalettura)
                    Dim max = letture.elencoLetture.Where(Function(x) x.datalettura = MaxDate).FirstOrDefault().valore
                    Dim min = letture.elencoLetture.Where(Function(x) x.datalettura = MinDate).FirstOrDefault().valore

                    Totale += max - min
                End If
            Next
        Catch ex As Exception
            Throw New Exception(ex.Message, ex.InnerException)
        End Try
        Return Totale
    End Function

    Private Function RecuperoTotaleSuperficie(ByVal ElencoDomande As List(Of ElencoDomandeAziende),
                                              ByRef ObjParametri As AgronicaCoreParametri) As Decimal
        Dim Totale As Decimal = 0
        Try
            Dim di_biz As New AgronicaCoreDomandaIrriguaBIZ.DomandaIrrigua_R
            For Each row In ElencoDomande
                'lettura max
                Dim domanda = di_biz.LeggiDomanda(row.id_domanda, "", 0, ObjParametri)
                If domanda.dettaglio.Count > 0 Then
                    Dim SubTot As Decimal = 0
                    For Each det In domanda.dettaglio
                        If det.Selezionato = True Then
                            SubTot += det.Superficie
                        End If
                    Next

                    If SubTot < 1 Then
                        SubTot = 1
                    End If

                    Totale += SubTot
                End If
            Next
        Catch ex As Exception
            Throw New Exception(ex.Message, ex.InnerException)
        End Try
        Return Totale

    End Function

    Private Function RecuperoElencoAziendeConDomandaIrrigua(ByVal DaData As DateTime,
                                                            ByVal AData As DateTime,
                                                            ByRef ObjParametriServer As AgronicaCoreParametri) As List(Of ElencoDomandeAziende)
        Dim ret As New List(Of ElencoDomandeAziende)
        Try
            Dim dih_dal As New AgronicaCoreDomandaIrriguaDAL.DomandaIrrigua_Testata_R
            Dim dt = dih_dal.Leggi(0, "", 1, "", "", ObjParametriServer, DaData, AData)
            For Each row In dt.Rows
                ret.Add(New ElencoDomandeAziende() With {
                            .piva = row("piva"),
                            .id_domanda = row("id")
                        })
            Next

        Catch ex As Exception
            Throw New Exception(ex.Message, ex.InnerException)
        End Try
        Return ret
    End Function

    Private Function CalcolaImportoSpesaT1(ByVal SupTot As Decimal,
                                             ByVal VolTot As Decimal,
                                             ByVal ImportoFisso As Decimal,
                                             ByVal VolT2Tot As Decimal,
                                             ByVal deltaincrementot2 As Decimal,
                                             ByVal VolT3Tot As Decimal,
                                             ByVal deltaincrementot3 As Decimal,
                                             ByVal SpesaComplessiva As Decimal
                                             ) As Decimal
        Dim Val As Decimal = 0
        Try
            Dim SommaQuoteFisseAziedali = Math.Round(SupTot * ImportoFisso, 5)
            Dim TotaleIncrementoInFascia2 = Math.Round(VolT2Tot * deltaincrementot2, 5)
            Dim TotaleIncrementoInFascia3 = Math.Round(VolT3Tot * deltaincrementot3, 5)

            Dim SpesaComplessivaFascia1 = SpesaComplessiva - SommaQuoteFisseAziedali - TotaleIncrementoInFascia2 - TotaleIncrementoInFascia3

            Val = Math.Round(SpesaComplessivaFascia1 / VolTot, 5)

        Catch ex As Exception
            Throw New Exception(ex.Message, ex.InnerException)
        End Try
        Return Val
    End Function

End Class

Public Class ElencoDomandeAziende
    Public Property piva As String
    Public Property id_domanda As Integer
End Class
