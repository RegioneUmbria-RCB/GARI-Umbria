

Imports SincroAnagrafeBA
Imports Importazione_Avepa_DAL

Public Class Fascicolo_R


    Public Function GetSingoloFascicoloAgea( _
            ByVal EnteValidatore_COD As Integer, _
            ByVal cuaa As String, _
            ByVal Validazione_Numero As String, _
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    ) As Fascicolo



        Dim COD_SOGGETTO As Integer = 0
        Dim Cod_Ufficio As String = ""


        Dim f As New Fascicolo
        Dim LettoreAvepa As New AvepaAnagrafe_R("CAA.")

        Dim DtAnagrafica As DataTable = LettoreAvepa.Leggi_Anagrafiche(cuaa, objParametri)

        Dim t1 As New List(Of ISWSTerritorio1)

        Dim utiliz As New ISWSUtilizzoTerra1

        f.fascicolo = New ISWSRespAnagFascicolo2


        'info di testata
        f.fascicolo.CUAA = cuaa

        If Not DtAnagrafica Is Nothing AndAlso DtAnagrafica.Rows.Count > 0 Then

            f.fascicolo.PartitaIVA = DtAnagrafica.Rows(0).Item("PART_IVA")

            'detentore
            Dim Detentore As String = "103"
            Cod_Ufficio = DtAnagrafica.Rows(0).Item("CODICE_RIFERIMENTO")
            If Cod_Ufficio.Length > 5 Then
                Dim ProvSigla As String = Mid(Cod_Ufficio, 2, 2)
                Dim UffZona As String = Mid(Cod_Ufficio, 5)
                Dim ProvIstat As String = ""
                Dim objProv As New AgronicaCoreMetaSchemaDAL.Istat_R
                ProvIstat = objProv.CodIstat_from_Provincia(ProvSigla, objParametri)
                Detentore &= ProvIstat & UffZona.PadRight(3)
            End If
            f.fascicolo.Detentore = Detentore

            f.fascicolo.TipoAzienda = DtAnagrafica.Rows(0).Item("FLAG_PERS_FISICA")
            f.fascicolo.Denominazione = DtAnagrafica.Rows(0).Item("DENOMINAZIONE")


            f.fascicolo.Denominazione = DtAnagrafica.Rows(0).Item("DENOMINAZIONE")


            f.fascicolo.Recapito.Cap = DtAnagrafica.Rows(0).Item("CAP_RESI")
            f.fascicolo.Recapito.Indirizzo = DtAnagrafica.Rows(0).Item("INDIRIZZO_RESI")
            f.fascicolo.Recapito.Provincia = DtAnagrafica.Rows(0).Item("COD_ISTAT_PROVINCIA")
            f.fascicolo.Recapito.Comune = DtAnagrafica.Rows(0).Item("COD_ISTAT_COMUNE")

            If f.fascicolo.TipoAzienda = "P" Then

                f.fascicolo.DettaglioSoggettoWS.SoggettoWS.Desc_nome = DtAnagrafica.Rows(0).Item("NOME")
                f.fascicolo.DettaglioSoggettoWS.SoggettoWS.Desc_cogn = DtAnagrafica.Rows(0).Item("COGNOME")
                f.fascicolo.DettaglioSoggettoWS.SoggettoWS.CUAA = DtAnagrafica.Rows(0).Item("COD_FISCALE")
                f.fascicolo.DettaglioSoggettoWS.SoggettoWS.Codi_sess = DtAnagrafica.Rows(0).Item("SESSO")

                f.fascicolo.DettaglioSoggettoWS.SoggettoWS.Data_nasc = DtAnagrafica.Rows(0).Item("DATA_NASCITA")
                f.fascicolo.DettaglioSoggettoWS.SoggettoWS.Desc_comu_nasc = DtAnagrafica.Rows(0).Item("DE_COMUNE_NASC")
                f.fascicolo.DettaglioSoggettoWS.SoggettoWS.Desc_prov_nasc = DtAnagrafica.Rows(0).Item("DE_PROVINCIA_NASC")

            End If

            ''qui la data nascita è intero
            'If Not ws_fasciResponse.out.fascicolo.fascicolo.DettaglioSoggettoWS.SoggettoWS.Data_nasc Is Nothing Then
            '    'qui la data nascita è data
            '    If IsDate(ws_fasciResponse.out.fascicolo.fascicolo.DettaglioSoggettoWS.SoggettoWS.Data_nasc) Then
            '        Legale_Rappresentante_Data_Nascita = CDate(ws_fasciResponse.out.fascicolo.fascicolo.DettaglioSoggettoWS.SoggettoWS.Data_nasc).ToShortDateString
            '    ElseIf IsNumeric(ws_fasciResponse.out.fascicolo.fascicolo.DettaglioSoggettoWS.SoggettoWS.Data_nasc) Then
            '        Legale_Rappresentante_Data_Nascita = CDate(AgronicaCoreDataProvider.Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse.out.fascicolo.fascicolo.DettaglioSoggettoWS.SoggettoWS.Data_nasc)).ToShortDateString
            '    Else
            '        Legale_Rappresentante_Data_Nascita = AgronicaCoreDataProvider.UtilityProvider.DataNascita_from_CodFisc(Legale_Rappresentante_CF)
            '    End If
            'Else
            '    Legale_Rappresentante_Data_Nascita = AgronicaCoreDataProvider.UtilityProvider.DataNascita_from_CodFisc(Legale_Rappresentante_CF)
            'End If

            'catasto..

            Dim DtParticelle As DataTable = LettoreAvepa.Leggi_Terreni(COD_SOGGETTO, objParametri)

            Dim DtPianoColturale As DataTable = LettoreAvepa.Leggi_Piani_Colturali(COD_SOGGETTO, 0, objParametri)


            t1 = getTerr1_Particelle(DtParticelle)

            For Each p In t1

                p.ISWSUtilizzoTerra1 = getUtilizzoTerra1(p, DtParticelle).ToArray

                For Each m In p.ISWSUtilizzoTerra1

                    m.ISWSUtilizzoTerra = getUso(p, m, DtParticelle).ToArray

                Next
            Next

            f.fascicolo.ISWSTerritorio1 = t1.ToArray

        End If





        Return f

    End Function

    'PARTICELLE
    Private Function getTerr1_Particelle(ByVal dt As DataTable) As List(Of ISWSTerritorio1)

        Return ( _
            From ii In dt.AsEnumerable _
            Group By _
                gProvincia = ii("COD_ISTAT_PROVINCIA"), _
                gComune = ii("COD_ISTAT_COMUNE"), _
                gSezione = ii("SEZIONE"), _
                gFoglio = ii("FOGLIO"), _
                gParticella = ii("PARTICELLA"), _
                gSubalterno = ii("SUBALTERNO"), _
                gSuperficieCatastale = ii("SUPERFICIE_CATASTALE"), _
                gCodiceTipoConduzione = ii("FLAG_TITOLO_CONDUZIONE"), _
                gSuperficieCondotta = ii("SUPERFICIE_CONDOTTA"), _
                gDataInizioConduzione = ii("DATA_INIZIO_CONDUZIONE"), _
                gDataFineConduzione = ii("DATA_FINE_CONDUZIONE") _
                Into gg = Group _
             Select New ISWSTerritorio1 With { _
                .Provincia = gProvincia, _
                .Comune = gComune, _
                .Sezione = gSezione, _
                .Foglio = gFoglio, _
                .Particella = gParticella, _
                .Subalterno = gSubalterno, _
                .SuperficieCatastale = gSuperficieCatastale, _
                .CodiceTipoConduzione = gCodiceTipoConduzione, _
                .SuperficieCondotta = gSuperficieCondotta, _
                .DataInizioConduzione = AgronicaCoreDataProvider.Conversioni.DateTime_To_DataintYYYYMMGG(gDataInizioConduzione), _
                .DataFineConduzione = AgronicaCoreDataProvider.Conversioni.DateTime_To_DataintYYYYMMGG(gDataFineConduzione) _
            }).ToList


    End Function

    'MACROUSI
    Private Function getUtilizzoTerra1(ByVal t1 As ISWSTerritorio1, ByVal dt As DataTable) As List(Of ISWSUtilizzoTerra1)

        Return ( _
            From ii In dt.AsEnumerable _
            Where t1.Provincia = ii("Provincia") _
            And t1.Comune = ii("Comune") _
            And t1.Sezione = ii("Sezione") _
            And t1.Foglio = ii("Foglio") _
            And t1.Particella = ii("Particella") _
            And t1.Subalterno = ii("Subalterno") _
            And ii("Destinazione_codicemacrouso") <> "" _
            Group By _
               gDestinazione_codicemacrouso = ii("Destinazione_codicemacrouso"), _
               gDestinazione_superficieutilizzata = CInt(ii("Destinazione_superficieutilizzata")) _
            Into gg = Group _
            Select New ISWSUtilizzoTerra1 With { _
                .CodiceMacrouso = gDestinazione_codicemacrouso, _
                .SuperficieUtilizzata = CInt(gDestinazione_superficieutilizzata), _
                .SuperficieUtilizzataSpecified = True _
            }).ToList


    End Function

    'UTILIZZI
    Private Function getUso(ByVal t1 As ISWSTerritorio1, ByVal u1 As ISWSUtilizzoTerra1, ByVal dt As DataTable) As List(Of ISWSUtilizzoTerra)
        Return ( _
                    From ii In dt.AsEnumerable _
                    Where t1.Provincia = ii("Provincia") _
                    And t1.Comune = ii("Comune") _
                    And t1.Sezione = ii("Sezione") _
                    And t1.Foglio = ii("Foglio") _
                    And t1.Particella = ii("Particella") _
                    And t1.Subalterno = ii("Subalterno") _
                    And u1.CodiceMacrouso = ii("Destinazione_codicemacrouso") _
                    And ii("USO_codiceprodotto") <> "" _
                    Select New ISWSUtilizzoTerra With { _
                        .CodiceProdotto = ii("USO_codiceprodotto"), _
                        .SuperficieUtilizzata = CInt(ii("USO_superficieutilizzata")), _
                        .SuperficieUtilizzataSpecified = True, _
                        .CodiceVarieta = "" _
                    }).ToList


    End Function
End Class
