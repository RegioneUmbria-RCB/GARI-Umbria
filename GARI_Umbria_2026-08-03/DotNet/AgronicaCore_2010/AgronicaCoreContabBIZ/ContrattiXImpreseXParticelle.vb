Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreContabDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModelsSTD.exceptions

Public Class ContrattiXImpreseXParticelle
    Inherits AgronicaCoreDataProvider.DataProvider

    Private _objParametriServer As AgronicaCoreParametri
    Private _nomeFileLogAggiornaImpreseParticelle As String
    Private _scriviLogAggiornaImpreseParticelle As Boolean = False

    Private _progElab As Integer = 0

    Private ReadOnly _formatoData As String = "dd/MM/yyyy"
    Private ReadOnly _separatore As String = StrDup(120, "-")
    Private ReadOnly _separatoreDoppio As String = StrDup(120, "=")
    Private ReadOnly _flagModificato As String = "*"
    Private ReadOnly _flagInvariato As String = " "
    Private ReadOnly _descrizioneEnum As String = "G"
    Private ReadOnly _valoreEnum As String = "D"
    Private ReadOnly _orderByImpreseParticelleData = " ImpresexParticelle.Validita_Inizio , ImpresexParticelle.Validita_Fine "

    Public Sub New(objParametriServer As AgronicaCoreParametri)

        _objParametriServer = objParametriServer

        _nomeFileLogAggiornaImpreseParticelle = String.Format("AggiornaImpreseParticelleDaContrattoLog_{0}_{1}_{2}.txt",
                                                              Trim(_objParametriServer.SuperUserUsername),
                                                              Now().ToString("yyyy"),
                                                              Now().ToString("MM"))

        'Lettura chiave scrittura log in fase di aggiornamento imprese per particelle

        Dim objCfgSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim VCS_ContrattiAffitto = objCfgSiti.Leggi_Valore_JSON(Of AgronicaCoreVarieDAL.VCS_ContrattiAffitto)(_objParametriServer)
        If VCS_ContrattiAffitto.ScriviLogAggiornaImpreseParticelle > 0 Then
            _scriviLogAggiornaImpreseParticelle = True
        End If

    End Sub

    Public Sub AggiornaContrattiImpreseParticelle(ByVal tipoOperazione As enum_TipoOperazioneRiga,
                                                  ByVal piva As String,
                                                  ByVal saCod As Integer,
                                                  ByVal idAgenda As Integer,
                                                  ByVal Particella As ContrattiXImpreseXParticelle_Particella,
                                                  ByVal superficieCatastale As Decimal,
                                                  ByVal idImprPart As Integer,
                                                  ByVal codImprPart As String,
                                                  ByVal inserisciNuovoCodParticella As Boolean,
                                                  ByVal superficie As Decimal,
                                                  ByVal validitaInizio As Date,
                                                  ByVal validitaFine As Date,
                                                  ByVal bioVincolo As Integer,
                                                  ByVal forzaCancellazioneLegamiAppezzamentiCampi As enum_forzaCancella)

        Const nomeRoutine = "AgronicaCoreContabBIZ.ContrattiXImpreseXParticelle.AggiornaContrattiImpreseParticelle()"

        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False

        Try

            Utility.VerificaApriTransazione(_objParametriServer, flagConnessione, flagTransazione)

            Dim leggiMovimentiDettagli As New AgronicaCoreContabDAL.Movimenti_Dettagli_R

            Dim dtMovimentiDettagli = leggiMovimentiDettagli.Leggi(piva,
                                                                   saCod,
                                                                   idAgenda,
                                                                   0,
                                                                   0,
                                                                   0,
                                                                   0,
                                                                   0,
                                                                   "",
                                                                   0,
                                                                   0,
                                                                   0,
                                                                   0,
                                                                   0,
                                                                   0,
                                                                   enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                   "",
                                                                   "",
                                                                   _objParametriServer)

            Dim leggiContrattiImpreseParticelle As New ContrattiXImpreseXParticelle_R
            Dim scriviContrattiImpreseParticelle As New ContrattiXImpreseXParticelle_W

            Dim primaRiga As Boolean = True

            Dim dtRigaContratto As DataTable = Nothing

            If SeTabellaConRighe(dtMovimentiDettagli) Then

                For Each riga As DataRow In dtMovimentiDettagli.Rows

                    If IsModificaCancellazione(tipoOperazione) AndAlso primaRiga Then

                        dtRigaContratto = leggiContrattiImpreseParticelle.Leggi(piva,
                                                                                saCod,
                                                                                idAgenda,
                                                                                riga.Item("Id_Mov_Det"),
                                                                                Particella,
                                                                                enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                "",
                                                                                "",
                                                                                _objParametriServer)

                    End If

                    AggiornaRigaContrattoAffitto(tipoOperazione,
                                                 piva,
                                                 saCod,
                                                 idAgenda,
                                                 riga.Item("Id_Mov_Det"),
                                                 Particella,
                                                 superficie,
                                                 validitaInizio,
                                                 validitaFine,
                                                 bioVincolo,
                                                 scriviContrattiImpreseParticelle)

                Next

            End If

            'Inserimento codice particella

            If inserisciNuovoCodParticella Then

                idImprPart = inserisciNuovoCodiceParticella(codImprPart,
                                                            piva,
                                                            saCod,
                                                            idAgenda,
                                                            Particella,
                                                            superficie,
                                                            validitaInizio,
                                                            validitaFine)

            End If

            'Algoritmo di aggiornamento ImpreseXParticelle

            Dim objDatiModificati = DeterminaDatiModificati(tipoOperazione,
                                                            superficie,
                                                            validitaInizio,
                                                            validitaFine,
                                                            bioVincolo,
                                                            dtRigaContratto)

            If DatiSensibiliModificati(tipoOperazione, objDatiModificati) Then

                AggiornaImpreseParticellePerModificaContratto(tipoOperazione,
                                                              piva,
                                                              saCod,
                                                              idAgenda,
                                                              Particella,
                                                              superficieCatastale,
                                                              idImprPart,
                                                              codImprPart,
                                                              inserisciNuovoCodParticella,
                                                              objDatiModificati,
                                                              leggiContrattiImpreseParticelle,
                                                              forzaCancellazioneLegamiAppezzamentiCampi)

            End If

            Utility.VerificaChiudiTransazione(_objParametriServer, flagTransazione)

        Catch ex As GiasException

            Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            Throw ex

        Catch ex As Exception

            Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)

        Finally

            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)

        End Try

    End Sub

    Private Function IsModificaInserimento(ByVal tipoOperazione As enum_TipoOperazioneRiga)

        Return tipoOperazione = enum_TipoOperazioneRiga.Modifica OrElse tipoOperazione = enum_TipoOperazioneRiga.Inserimento

    End Function

    Private Function IsModificaCancellazione(ByVal tipoOperazione As enum_TipoOperazioneRiga)

        Return tipoOperazione = enum_TipoOperazioneRiga.Modifica OrElse tipoOperazione = enum_TipoOperazioneRiga.Cancellazione

    End Function

    Private Sub AggiornaRigaContrattoAffitto(ByVal tipoOperazioneRiga As enum_TipoOperazioneRiga,
                                             ByVal piva As String,
                                             ByVal saCod As Integer,
                                             ByVal idAgenda As Integer,
                                             ByVal idMovDet As Integer,
                                             ByVal Particella As ContrattiXImpreseXParticelle_Particella,
                                             ByVal superficie As Decimal,
                                             ByVal validitaInizio As Date,
                                             ByVal validitaFine As Date,
                                             ByVal bioVincolo As Integer,
                                             ByRef scriviContrattiImpreseParticelle As ContrattiXImpreseXParticelle_W)

        Select Case tipoOperazioneRiga

            Case enum_TipoOperazioneRiga.Inserimento

                scriviContrattiImpreseParticelle.Scrivi(piva,
                                                        saCod,
                                                        idAgenda,
                                                        idMovDet,
                                                        Particella,
                                                        superficie,
                                                        validitaInizio,
                                                        validitaFine,
                                                        bioVincolo,
                                                        _objParametriServer)

            Case enum_TipoOperazioneRiga.Modifica

                scriviContrattiImpreseParticelle.Modifica(0,
                                                          piva,
                                                          saCod,
                                                          idAgenda,
                                                          idMovDet,
                                                          Particella,
                                                          superficie,
                                                          validitaInizio,
                                                          validitaFine,
                                                          bioVincolo,
                                                          "",
                                                          _objParametriServer)

            Case enum_TipoOperazioneRiga.Cancellazione

                scriviContrattiImpreseParticelle.Cancella(0,
                                                          piva,
                                                          saCod,
                                                          idAgenda,
                                                          idMovDet,
                                                          Particella,
                                                          Nothing,
                                                          Nothing,
                                                          "",
                                                          _objParametriServer)

        End Select


    End Sub

    Public Sub InserisciContrattiImpreseParticelleNuovaRiga(ByVal piva As String,
                                                            ByVal idAgenda As Integer,
                                                            ByVal idMovDet As Integer)

        Const nomeRoutine = "AgronicaCoreContabBIZ.ContrattiXImpreseXParticelle.InserisciContrattiImpreseParticelleNuovaRiga()"

        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False

        Try

            Utility.VerificaApriTransazione(_objParametriServer, flagConnessione, flagTransazione)

            Dim leggiContrattiImpreseParticelle As New ContrattiXImpreseXParticelle_R
            Dim scriviContrattiImpreseParticelle As New ContrattiXImpreseXParticelle_W

            Dim dtContrattiImpreseParticelle = leggiContrattiImpreseParticelle.Leggi(piva,
                                                                                     0,
                                                                                     idAgenda,
                                                                                     0,
                                                                                     Nothing,
                                                                                     enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                     "",
                                                                                     "ContrattiXImpreseXParticelle.Id_Mov_Det",
                                                                                     _objParametriServer)

            Dim idMovDetPrimaRiga As Integer = 0

            If SeTabellaConRighe(dtContrattiImpreseParticelle) Then

                For Each riga As DataRow In dtContrattiImpreseParticelle.Rows

                    If idMovDetPrimaRiga = 0 Then

                        idMovDetPrimaRiga = riga.Item("Id_Mov_Det")

                    End If

                    If riga.Item("Id_Mov_Det") = idMovDetPrimaRiga Then

                        Dim Particella As New ContrattiXImpreseXParticelle_Particella With {
                            .PROV = riga.Item("PROV"),
                            .COM = riga.Item("COM"),
                            .SEZIONE = riga.Item("SEZIONE"),
                            .FOGLIO = riga.Item("FOGLIO"),
                            .NUMERO = riga.Item("NUMERO"),
                            .SUBALTERNO = riga.Item("SUBALTERNO")
                        }

                        scriviContrattiImpreseParticelle.Scrivi(piva,
                                                                riga.Item("Sa_Cod"),
                                                                idAgenda,
                                                                idMovDet,
                                                                Particella,
                                                                riga.Item("Superficie"),
                                                                riga.Item("Validita_Inizio"),
                                                                riga.Item("Validita_Fine"),
                                                                riga.Item("BioVincolo"),
                                                                _objParametriServer)

                    Else

                        Exit For

                    End If

                Next

            End If

            Utility.VerificaChiudiTransazione(_objParametriServer, flagTransazione)

        Catch ex As GiasException

            Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            Throw ex

        Catch ex As Exception

            Utility.VerificaAnnullaTransazione(_objParametriServer, flagTransazione)
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)

        Finally

            Utility.VerificaChiudiConnessione(_objParametriServer, flagConnessione)

        End Try

    End Sub

    Private Function inserisciNuovoCodiceParticella(ByVal codImprPart As String,
                                                    ByVal piva As String,
                                                    ByVal saCod As Integer,
                                                    ByVal idAgenda As Integer,
                                                    ByVal Particella As ContrattiXImpreseXParticelle_Particella,
                                                    ByVal superficie As Decimal,
                                                    ByVal validitaInizio As Date,
                                                    ByVal validitaFine As Date) As Integer

        Dim nuovoIdImpreseParticelle As Integer

        Dim leggiImpreseParticelle As New ImpresexParticelle2_R
        Dim scriviImpreseParticelle As New ImpresexParticelle2_W

        Dim scriviImpreseParticelleCodici As New ImpresexParticelle_Codici_W

        '--------------------------------------------------
        'Inserimento Imprese x Particelle
        '--------------------------------------------------

        scriviImpreseParticelle.Scrivi_2(piva,
                                         saCod,
                                         Particella.PROV,
                                         Particella.COM,
                                         Particella.SEZIONE,
                                         Particella.FOGLIO,
                                         Particella.NUMERO,
                                         Particella.SUBALTERNO,
                                         "",
                                         enum_TitoloPossesso.AffittoContratto,
                                         superficie,
                                         validitaInizio,
                                         validitaFine,
                                         _objParametriServer)

        'Rileggo per ottenere ID rk appena inserito (eventualmente usare EF per evitare tale lettura)

        Dim filtroAggiuntivo = String.Format("ImpresexParticelle.TitoloPossesso = {0} AND " &
                                             "ImpresexParticelle.Validita_Inizio = {1} AND " &
                                             "ImpresexParticelle.Validita_Fine = {2} ",
                                             enum_TitoloPossesso.AffittoContratto.ToString(_valoreEnum),
                                             Agro_SQL_SaveDate(validitaInizio),
                                             Agro_SQL_SaveDate(validitaFine))

        Dim dtImpreseParticelleInserito = leggiImpreseParticelle.Leggi(0,
                                                                       piva,
                                                                       saCod,
                                                                       0,
                                                                       Particella.PROV,
                                                                       Particella.COM,
                                                                       Particella.SEZIONE,
                                                                       Particella.FOGLIO,
                                                                       Particella.NUMERO,
                                                                       Particella.SUBALTERNO,
                                                                       enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                       filtroAggiuntivo,
                                                                       "",
                                                                       _objParametriServer)

        ControllaEsitoLettura(dtImpreseParticelleInserito, enum_NomeTabella.ImpreseXParticelle)

        nuovoIdImpreseParticelle = CInt(dtImpreseParticelleInserito(0).Item("Id"))

        '--------------------------------------------------
        'Inserimento Imprese x Particelle Codici
        '--------------------------------------------------

        scriviImpreseParticelleCodici.Scrivi(nuovoIdImpreseParticelle,
                                             enum_CodiciAnagrafe.CodiceParticella,
                                             codImprPart,
                                             AGRODATAINIZIO,
                                             AGRODATAFINE,
                                             _objParametriServer)

        ScriviLogNuovoCodiceParticella(idAgenda, Particella, codImprPart, nuovoIdImpreseParticelle)

        Return nuovoIdImpreseParticelle

    End Function

    Private Function DeterminaDatiModificati(ByVal tipoOperazione As enum_TipoOperazioneRiga,
                                             ByVal superficie As Decimal,
                                             ByVal validitaInizio As Date,
                                             ByVal validitaFine As Date,
                                             ByVal bioVincolo As Integer,
                                             ByVal dtRigaContratto As DataTable) As DatiModificati

        Dim objDatiModificati As New DatiModificati

        'Determina nuove dati

        If IsModificaInserimento(tipoOperazione) Then
            objDatiModificati.validitaInizNew = validitaInizio
            objDatiModificati.validitaFineNew = validitaFine
            objDatiModificati.superficieNew = superficie
            objDatiModificati.bioVincoloNew = bioVincolo
        End If

        'Determina vecchi dati

        If SeTabellaConRighe(dtRigaContratto) Then
            objDatiModificati.validitaInizOld = dtRigaContratto(0)("Validita_Inizio")
            objDatiModificati.validitaFineOld = dtRigaContratto(0)("Validita_Fine")
            objDatiModificati.superficieOld = CDec(dtRigaContratto(0)("Superficie"))
            objDatiModificati.bioVincoloOld = CInt(dtRigaContratto(0)("BioVincolo"))
        End If

        Return objDatiModificati

    End Function

    Private Function DatiSensibiliModificati(ByVal tipoOperazione As enum_TipoOperazioneRiga,
                                             ByVal objDatiModificati As DatiModificati) As Boolean

        Dim DatiSensibiliNonModificati As Boolean = False

        If tipoOperazione = enum_TipoOperazioneRiga.Modifica Then

            If objDatiModificati.validitaInizNew = objDatiModificati.validitaInizOld AndAlso
               objDatiModificati.validitaFineNew = objDatiModificati.validitaFineOld AndAlso
               objDatiModificati.superficieNew = objDatiModificati.superficieOld Then

                DatiSensibiliNonModificati = True

            End If

        End If

        Return Not DatiSensibiliNonModificati

    End Function

    Private Sub AggiornaImpreseParticellePerModificaContratto(ByVal tipoOperazione As enum_TipoOperazioneRiga,
                                                              ByVal piva As String,
                                                              ByVal saCod As Integer,
                                                              ByVal idAgenda As Integer,
                                                              ByVal Particella As ContrattiXImpreseXParticelle_Particella,
                                                              ByVal superficeCatastale As Decimal,
                                                              ByVal idImprPartPilota As Integer,
                                                              ByVal codImprPartPilota As String,
                                                              ByVal inseritaImprPartPilota As Boolean,
                                                              ByVal objDatiModificati As DatiModificati,
                                                              ByRef leggiContrattiImpreseParticelle As ContrattiXImpreseXParticelle_R,
                                                              ByVal forzaCancellazioneLegamiAppezzamentiCampi As enum_forzaCancella)

        '--------------------------------------------------------------------------------
        'Inizio elaborazione
        '--------------------------------------------------------------------------------

        ScriviLogInizioElaborazione(tipoOperazione,
                                    idAgenda,
                                    Particella,
                                    objDatiModificati)

        '--------------------------------------------------------------------------------
        'Lettura contratti coinvolti nella modifica
        '--------------------------------------------------------------------------------

        Dim leggiImpreseParticelle As New ImpresexParticelle2_R
        Dim scriviImpreseParticelle As New ImpresexParticelle2_W

        'Determinazione periodo contratti

        Dim listaPeriodiContratto As New List(Of ContrattiXImpreseXParticelle_Periodo)

        If IsModificaInserimento(tipoOperazione) Then
            listaPeriodiContratto.Add(CreaPeriodoContratto(objDatiModificati.validitaInizNew,
                                                           objDatiModificati.validitaFineNew))
        End If

        If IsModificaCancellazione(tipoOperazione) Then
            listaPeriodiContratto.Add(CreaPeriodoContratto(objDatiModificati.validitaInizOld,
                                                           objDatiModificati.validitaFineOld))
        End If

        'Esecuzione lettura contratti

        Dim orderByContratti = " ContrattiXImpreseXParticelle.Validita_Inizio , ContrattiXImpreseXParticelle.Validita_Fine "

        Dim dtContrattiImpreseParticelleModifica = leggiContrattiImpreseParticelle.LeggiPeriodiValidita(piva,
                                                                                                        0,
                                                                                                        0,
                                                                                                        0,
                                                                                                        Particella,
                                                                                                        listaPeriodiContratto,
                                                                                                        "",
                                                                                                        orderByContratti,
                                                                                                        _objParametriServer)

        'Controllo contratti coinvolti in cascata

        Dim livello As Integer = 0
        Dim periodiValiditaInseriti As Integer = 0
        Dim dtContrattiImpreseParticelleCascata As DataTable = Nothing
        Dim listaPeriodiContrattiCascata As New List(Of ContrattiXImpreseXParticelle_Periodo)

        If tipoOperazione = enum_TipoOperazioneRiga.Cancellazione Then
            'In caso di cancellazione devo inserire il rispettivo periodo in quanto, essendo già stato eliminato,
            'non verrà individuato nella lettura contratti appena eseguita
            listaPeriodiContrattiCascata.Add(CreaPeriodoContratto(objDatiModificati.validitaInizOld,
                                                                  objDatiModificati.validitaFineOld))
        End If

        ScriviLogAggiornaImpreseParticelle(_separatore)
        ScriviLogAggiornaImpreseParticelle("Ricerca contratti coinvolti...")

        periodiValiditaInseriti = RicercaContrattiLivello(livello, dtContrattiImpreseParticelleModifica, listaPeriodiContrattiCascata)

        While periodiValiditaInseriti > 0

            'TODO: migliorare per escludere nella query i contratti già trattati in precedenza

            dtContrattiImpreseParticelleCascata = leggiContrattiImpreseParticelle.LeggiPeriodiValidita(piva,
                                                                                                        0,
                                                                                                        0,
                                                                                                        0,
                                                                                                        Particella,
                                                                                                        listaPeriodiContrattiCascata,
                                                                                                        "",
                                                                                                        orderByContratti,
                                                                                                        _objParametriServer)

            periodiValiditaInseriti = RicercaContrattiLivello(livello, dtContrattiImpreseParticelleCascata, listaPeriodiContrattiCascata)

        End While

        '--------------------------------------------------------------------------------
        'Lettura imprese per particelle coinvolte nella modifica
        '--------------------------------------------------------------------------------

        'Determinazione periodo imprese per particelle coinvolte nella modifica

        Dim listaPeriodiImpreseParticelle As New List(Of ImpresexParticelle2_Periodo)

        For Each periodoContrattoCascata In listaPeriodiContrattiCascata

            listaPeriodiImpreseParticelle.Add(CreaPeriodoImpreseParticelle(periodoContrattoCascata.Validita_Inizio,
                                                                           periodoContrattoCascata.Validita_Fine))

        Next

        'Esecuzione lettura imprese per particelle

        Dim filtroAggiuntivoImpreseParticelle = ""

        If listaPeriodiImpreseParticelle.Count > 0 Then

            filtroAggiuntivoImpreseParticelle = String.Format("ImpresexParticelle.TitoloPossesso = {0}",
                                                              enum_TitoloPossesso.AffittoContratto.ToString(_valoreEnum))

        Else

            filtroAggiuntivoImpreseParticelle = String.Format("ImpresexParticelle.ID = {0}",
                                                              idImprPartPilota)

        End If

        Dim dtImpreseParticelle = leggiImpreseParticelle.LeggiPeriodiValidita(piva,
                                                                              saCod,
                                                                              Particella.PROV,
                                                                              Particella.COM,
                                                                              Particella.SEZIONE,
                                                                              Particella.FOGLIO,
                                                                              Particella.NUMERO,
                                                                              Particella.SUBALTERNO,
                                                                              listaPeriodiImpreseParticelle,
                                                                              filtroAggiuntivoImpreseParticelle,
                                                                              _orderByImpreseParticelleData,
                                                                              _objParametriServer)

        '--------------------------------------------------------------------------------
        'Imprese particelle pilota da cui verranno copiati tutti gli altri
        '--------------------------------------------------------------------------------

        ScriviLogAggiornaImpreseParticelle(_separatore)
        ScriviLogAggiornaImpreseParticelle(String.Format("Imprese particelle pilota [{0}]:", idImprPartPilota))
        Dim listaImpreseParticellePilota = dtImpreseParticelle.Select(String.Format("ID={0}", idImprPartPilota))

        If IsNothing(listaImpreseParticellePilota) OrElse listaImpreseParticellePilota.Count = 0 Then

            Dim messaggioEccezione = "Dati identificativo particella pilota non trovati"

            GeneraEccezione(messaggioEccezione)

        End If

        Dim impreseParticellePilota As DataRow = listaImpreseParticellePilota(0)
        ScriviLogRigaImpreseParticelle(impreseParticellePilota)

        '--------------------------------------------------------------------------------
        'Controlli iniziali
        '--------------------------------------------------------------------------------

        If Not IsNothing(dtImpreseParticelle) Then

            'Controllo omogeneità dati imprese per particelle da cancellare

            Dim listaIdImpreseParticelle As New List(Of Integer)

            If idImprPartPilota > 0 Then

                listaIdImpreseParticelle.Add(idImprPartPilota)

            End If

            If dtImpreseParticelle.Rows.Count > 1 Then

                ControlloDatiOmogeneiImpreseParticelle(dtImpreseParticelle,
                                                       idImprPartPilota,
                                                       impreseParticellePilota,
                                                       listaIdImpreseParticelle)

            End If

            'Controllo presenza contatti

            If listaIdImpreseParticelle.Count > 0 Then

                ControlloPresenzaContatti(listaIdImpreseParticelle, leggiImpreseParticelle)

            End If

            'Controllo omogeneità codici particelle

            If listaIdImpreseParticelle.Count > 1 Then

                ControlloOmogeneitaCodiciParticella(listaIdImpreseParticelle, codImprPartPilota)

            End If

        End If

        '--------------------------------------------------------------------------------
        'Cancellazione imprese per particelle
        '--------------------------------------------------------------------------------

        Dim scriviImpreseParticelleCodici As New ImpresexParticelle_Codici_W

        If SeTabellaConRighe(dtImpreseParticelle) Then

            CancellaImpreseParticelle(dtImpreseParticelle, idImprPartPilota, scriviImpreseParticelle, scriviImpreseParticelleCodici)

        End If

        '--------------------------------------------------------------------------------
        'Simulazione aggiornamento imprese per particelle in memoria
        '--------------------------------------------------------------------------------

        Dim listaImpreseParticelleDaAggiornare = ElaboraImpreseParticelleDaAggiornare(dtContrattiImpreseParticelleCascata,
                                                                                      idImprPartPilota,
                                                                                      inseritaImprPartPilota)

        '--------------------------------------------------------------------------------
        'Effettivo aggiornamento imprese per particelle
        '--------------------------------------------------------------------------------

        'Dim forzaEccezione As Boolean = False
        'If forzaEccezione Then
        '    Throw New Exception("Errore inaspettato!")
        'End If

        Dim particelleAggiornate = False

        If Not IsNothing(listaImpreseParticelleDaAggiornare) AndAlso listaImpreseParticelleDaAggiornare.Count > 0 Then

            Dim indiceMax As Integer = listaImpreseParticelleDaAggiornare.Count - 1
            Dim indice As Integer = 0

            ScriviLogAggiornaImpreseParticelle(_separatore)
            ScriviLogAggiornaImpreseParticelle(String.Format("Aggiornamento imprese particelle:", idImprPartPilota))

            Dim dtImpreseParticelleCodiciPilota As DataTable = Nothing
            Dim dtImpreseParticelleContattiPilota As DataTable = Nothing

            'Ordino la lista da aggiornare per validità inizio

            listaImpreseParticelleDaAggiornare.Sort(Function(x, y) x.validitaInizio.CompareTo(y.validitaInizio))

            For Each rigaImpreseParticelle In listaImpreseParticelleDaAggiornare

                'Controllo superamento superficie catastale

                If rigaImpreseParticelle.superficie > superficeCatastale Then

                    Dim messaggioEccezione = String.Format("Superficie ({0}) maggiore di quella catastale ({1}) nel periodo dal {2} al {3}",
                                                           rigaImpreseParticelle.superficie,
                                                           superficeCatastale,
                                                           formattaDataInStringa(rigaImpreseParticelle.validitaInizio),
                                                           formattaDataInStringa(rigaImpreseParticelle.validitaFine))

                    GeneraEccezione(messaggioEccezione)

                End If

                'Controllo sovrapposizione periodi

                If indice < indiceMax Then

                    Dim indiceSucc As Integer = indice + 1
                    Dim validitaInizioAttuale = listaImpreseParticelleDaAggiornare(indice).validitaInizio
                    Dim validitaFineAttuale = listaImpreseParticelleDaAggiornare(indice).validitaFine
                    Dim validitaInizioSuccessiva = listaImpreseParticelleDaAggiornare(indiceSucc).validitaInizio
                    Dim validitaFineSuccessiva = listaImpreseParticelleDaAggiornare(indiceSucc).validitaFine

                    If validitaFineAttuale >= validitaInizioSuccessiva Then

                        Dim messaggioEccezione = String.Format("Sovrapposizione temporale particelle elaborate nei seguenti periodi: {0}-{1} e {2}-{3}",
                                                               formattaDataInStringa(validitaInizioAttuale),
                                                               formattaDataInStringa(validitaFineAttuale),
                                                               formattaDataInStringa(validitaInizioSuccessiva),
                                                               formattaDataInStringa(validitaFineSuccessiva))

                        GeneraEccezione(messaggioEccezione)

                    End If

                End If

                'Aggiornamento

                If rigaImpreseParticelle.idImprPart = 0 Then

                    InserisciImpreseParticelle(impreseParticellePilota,
                                               dtImpreseParticelleCodiciPilota,
                                               dtImpreseParticelleContattiPilota,
                                               rigaImpreseParticelle,
                                               scriviImpreseParticelle,
                                               leggiImpreseParticelle,
                                               scriviImpreseParticelleCodici)

                Else

                    AggiornaImpreseParticellePilota(impreseParticellePilota, rigaImpreseParticelle, scriviImpreseParticelle)

                End If

                indice += 1

            Next

            particelleAggiornate = True

        End If

        '--------------------------------------------------------------------------------
        'Controllo cancellazione/modifica particella pilota
        '--------------------------------------------------------------------------------

        Dim aggiornamentoParticellaPilota = enum_TipoAgg.Nessuno

        If tipoOperazione = enum_TipoOperazioneRiga.Cancellazione Then

            aggiornamentoParticellaPilota = AggiornamentoImpreseParticellePilota(piva,
                                                                                 Particella,
                                                                                 listaPeriodiContratto,
                                                                                 impreseParticellePilota,
                                                                                 leggiContrattiImpreseParticelle,
                                                                                 scriviImpreseParticelle,
                                                                                 scriviImpreseParticelleCodici,
                                                                                 forzaCancellazioneLegamiAppezzamentiCampi,
                                                                                 leggiImpreseParticelle,
                                                                                 superficeCatastale)

        End If

        '--------------------------------------------------------------------------------
        'Controllo globale sovrapposizioni
        '--------------------------------------------------------------------------------

        If particelleAggiornate OrElse aggiornamentoParticellaPilota = enum_TipoAgg.Modifica Then

            ControlloGlobaleSovrapposizioniParticella(piva, saCod, Particella, leggiImpreseParticelle)

        End If

        '--------------------------------------------------------------------------------
        'Controllo globale appezzamenti/campi non coperti da imprese per particelle
        '--------------------------------------------------------------------------------

        If particelleAggiornate OrElse ParticellaPilotaAggiornata(aggiornamentoParticellaPilota) Then

            ControlloGlobaleAppezzamentiCampiScoperti(impreseParticellePilota, forzaCancellazioneLegamiAppezzamentiCampi)

        End If

        '--------------------------------------------------------------------------------
        'Fine elaborazione
        '--------------------------------------------------------------------------------

        ScriviLogAggiornaImpreseParticelle(_separatore)
        ScriviLogAggiornaImpreseParticelle("### Fine elaborazione")

    End Sub

    Private Function RicercaContrattiLivello(ByRef livello As Integer,
                                             ByVal dtContrattiImpreseParticelle As DataTable,
                                             ByRef listaPeriodiContrattiCascata As List(Of ContrattiXImpreseXParticelle_Periodo)
                                             ) As Integer

        livello += 1

        Dim periodiValiditaInseriti As Integer = 0

        If SeTabellaConRighe(dtContrattiImpreseParticelle) Then

            For Each rigaContratto In dtContrattiImpreseParticelle.Rows

                If listaPeriodiContrattiCascata.Count = 0 Then

                    CreaPeriodoContrattoCascata(livello, rigaContratto, listaPeriodiContrattiCascata, periodiValiditaInseriti)

                Else

                    Dim validitaInizio = CDate(rigaContratto.Item("Validita_Inizio"))
                    Dim validitaFine = CDate(rigaContratto.Item("Validita_Fine"))

                    Dim indice = listaPeriodiContrattiCascata.FindIndex(Function(x) x.Validita_Inizio = validitaInizio AndAlso
                                                                                    x.Validita_Fine = validitaFine)
                    If indice < 0 Then

                        CreaPeriodoContrattoCascata(livello, rigaContratto, listaPeriodiContrattiCascata, periodiValiditaInseriti)

                    End If

                End If

            Next

        End If

        Return periodiValiditaInseriti

    End Function

    Private Sub CreaPeriodoContrattoCascata(ByVal livello As Integer,
                                            ByVal rigaContratto As DataRow,
                                            ByRef listaPeriodiContrattiCascata As List(Of ContrattiXImpreseXParticelle_Periodo),
                                            ByRef periodiValiditaInseriti As Integer)

        If periodiValiditaInseriti = 0 Then
            ScriviLogAggiornaImpreseParticelle(String.Format("...Livello {0}: ", livello))
        End If

        ScriviLogRigaContratto(rigaContratto)

        Dim validitaInizio = CDate(rigaContratto.Item("Validita_Inizio"))
        Dim validitaFine = CDate(rigaContratto.Item("Validita_Fine"))

        listaPeriodiContrattiCascata.Add(CreaPeriodoContratto(validitaInizio, validitaFine))

        periodiValiditaInseriti += 1

    End Sub

    Private Function CreaPeriodoContratto(ByVal Validita_Inizio As Date?,
                                          ByVal Validita_Fine As Date?
                                          ) As ContrattiXImpreseXParticelle_Periodo

        Dim periodoContratto As New ContrattiXImpreseXParticelle_Periodo With {
            .Validita_Inizio = Validita_Inizio,
            .Validita_Fine = Validita_Fine
        }

        Return periodoContratto

    End Function

    Private Function CreaPeriodoImpreseParticelle(ByVal Validita_Inizio As Date?,
                                                  ByVal Validita_Fine As Date?
                                                  ) As ImpresexParticelle2_Periodo

        Dim periodoImpreseParticelle As New ImpresexParticelle2_Periodo With {
            .Validita_Inizio = Validita_Inizio,
            .Validita_Fine = Validita_Fine
        }

        Return periodoImpreseParticelle

    End Function

    Private Function ElaboraImpreseParticelleDaAggiornare(ByVal dtContrattiImpreseParticelle As DataTable,
                                                          ByVal idImprPartPilota As Integer,
                                                          ByVal inseritaImprPartPilota As Boolean
                                                          ) As List(Of ImpreseParticelleDaAggiornare)

        ScriviLogAggiornaImpreseParticelle(_separatore)
        ScriviLogAggiornaImpreseParticelle("Elaborazione imprese particelle da aggiornare:")

        Dim listaImpreseParticelleDaAggiornare As New List(Of ImpreseParticelleDaAggiornare)

        If IsNothing(dtContrattiImpreseParticelle) OrElse dtContrattiImpreseParticelle.Rows.Count = 0 Then
            Return listaImpreseParticelleDaAggiornare
        End If

        Dim i As Integer = 0

        AzzeraProgressivoElaborazione()

        For Each rigaContratto In dtContrattiImpreseParticelle.Rows

            ScriviLogRigaContratto(rigaContratto, visualizzaParticella:=False, faseElaborazioneContratti:=True)

            Dim datiContratto As New DatiDaAggiornare With {
                .superficie = CDec(rigaContratto.Item("Superficie")),
                .validitaInizio = CDate(rigaContratto.Item("Validita_Inizio")),
                .validitaFine = CDate(rigaContratto.Item("Validita_Fine"))
            }

            i += 1

            If i = 1 Then

                ScriviLogAggiornaImpreseParticelle("  ##Caso0##")

                'Inserisco elemento pilota

                InserisciImpreseParticelleDaAggiornareInLista(datiContratto,
                                                              listaImpreseParticelleDaAggiornare,
                                                              idImprPart:=idImprPartPilota)

            Else

                '--------------------------------------------------------------------------------
                '1. Ricerca imprese particelle stesso periodo
                '--------------------------------------------------------------------------------

                Dim indice = listaImpreseParticelleDaAggiornare.FindIndex(Function(x) x.validitaInizio = datiContratto.validitaInizio AndAlso
                                                                                      x.validitaFine = datiContratto.validitaFine)

                If indice >= 0 Then

                    ScriviLogAggiornaImpreseParticelle("  ##Caso5##")

                    'Aumento solo la superficie

                    ModificaImpreseParticelleDaAggiornareInLista(listaImpreseParticelleDaAggiornare(indice),
                                                                 modificaSuperficie:=True,
                                                                 superficie:=listaImpreseParticelleDaAggiornare(indice).superficie + datiContratto.superficie)

                    'Passo al contratto successivo

                    Continue For

                End If

                '--------------------------------------------------------------------------------
                '2. Ricerca imprese particelle coinvolte
                '--------------------------------------------------------------------------------

                Dim listaIntersezioni = listaImpreseParticelleDaAggiornare.FindAll(Function(x) x.validitaInizio <= datiContratto.validitaFine AndAlso
                                                                                               x.validitaFine >= datiContratto.validitaInizio)

                If listaIntersezioni.Count > 0 Then

                    '--------------------------------------------------------------------------------
                    '3. Contratto con intersezioni
                    '--------------------------------------------------------------------------------

                    ElaboraContrattoConIntersezioni(datiContratto,
                                                    listaIntersezioni,
                                                    listaImpreseParticelleDaAggiornare)

                Else

                    '--------------------------------------------------------------------------------
                    '4. Contratto senza intersezioni
                    '--------------------------------------------------------------------------------

                    ScriviLogAggiornaImpreseParticelle("  ##Caso6##")

                    InserisciImpreseParticelleDaAggiornareInLista(datiContratto, listaImpreseParticelleDaAggiornare)

                End If

            End If

        Next

        Return listaImpreseParticelleDaAggiornare

    End Function

    Private Sub ElaboraContrattoConIntersezioni(ByVal datiContratto As DatiDaAggiornare,
                                                ByVal listaIntersezioni As List(Of ImpreseParticelleDaAggiornare),
                                                ByRef listaImpreseParticelleDaAggiornare As List(Of ImpreseParticelleDaAggiornare))

        Dim datiDaInserire As DatiDaAggiornare = Nothing

        'Ordino la lista intersezioni per validità inizio

        listaIntersezioni.Sort(Function(x, y) x.validitaInizio.CompareTo(y.validitaInizio))

        'Elaboro la lista delle intersezioni

        For indiceLista = 0 To listaIntersezioni.Count() - 1

            Dim rigaListaIntersezioni = listaIntersezioni(indiceLista)

            Select Case True

                '--------------------------------------------------------------------------------
                'Caso 1 - Riga lista compresa in contratto
                '--------------------------------------------------------------------------------

                Case rigaListaIntersezioni.validitaInizio >= datiContratto.validitaInizio AndAlso
                     rigaListaIntersezioni.validitaFine <= datiContratto.validitaFine

                    ScriviLogAggiornaImpreseParticelle("  ##Caso1##")

                    '1A. Aggiorno superficie riga lista

                    Dim indice = DeterminaIndiceDaProgressivoElaborazione(listaImpreseParticelleDaAggiornare, rigaListaIntersezioni.progElab)

                    ModificaImpreseParticelleDaAggiornareInLista(listaImpreseParticelleDaAggiornare(indice),
                                                                 modificaSuperficie:=True,
                                                                 superficie:=listaImpreseParticelleDaAggiornare(indice).superficie + datiContratto.superficie)

                    '1B. Inserisco riga lista precedente contratto

                    If datiContratto.validitaInizio < rigaListaIntersezioni.validitaInizio Then

                        SeInserisciRigaListaPrecedenteContratto(datiContratto,
                                                                listaIntersezioni,
                                                                indiceLista,
                                                                listaImpreseParticelleDaAggiornare)

                    End If

                    '1C. Inserisco riga lista successiva contratto

                    If datiContratto.validitaFine > rigaListaIntersezioni.validitaFine Then

                        SeInserisciRigaListaSuccessivaContratto(datiContratto,
                                                                listaIntersezioni,
                                                                indiceLista,
                                                                listaImpreseParticelleDaAggiornare)

                    End If

                '--------------------------------------------------------------------------------
                'Caso 2 - Riga lista include inizio contratto
                '--------------------------------------------------------------------------------

                Case rigaListaIntersezioni.validitaInizio < datiContratto.validitaInizio AndAlso
                     rigaListaIntersezioni.validitaFine <= datiContratto.validitaFine

                    ScriviLogAggiornaImpreseParticelle("  ##Caso2##")

                    '2A. Aggiorno superficie riga lista + validità Inizio

                    Dim indice = DeterminaIndiceDaProgressivoElaborazione(listaImpreseParticelleDaAggiornare, rigaListaIntersezioni.progElab)

                    ModificaImpreseParticelleDaAggiornareInLista(listaImpreseParticelleDaAggiornare(indice),
                                                                 modificaSuperficie:=True,
                                                                 superficie:=listaImpreseParticelleDaAggiornare(indice).superficie + datiContratto.superficie,
                                                                 modificaValiditaInizio:=True,
                                                                 validitaInizio:=datiContratto.validitaInizio)

                    '2B. Inserisco riga lista precedente intersezione

                    InserisciRigaListaPrecedenteIntersezione(datiContratto,
                                                             rigaListaIntersezioni,
                                                             listaImpreseParticelleDaAggiornare)

                    '2C. Inserisco riga lista successiva contratto

                    SeInserisciRigaListaSuccessivaContratto(datiContratto,
                                                            listaIntersezioni,
                                                            indiceLista,
                                                            listaImpreseParticelleDaAggiornare)

                '--------------------------------------------------------------------------------
                'Caso 3 - Riga lista include fine contratto
                '--------------------------------------------------------------------------------

                Case rigaListaIntersezioni.validitaInizio >= datiContratto.validitaInizio AndAlso
                     rigaListaIntersezioni.validitaFine > datiContratto.validitaFine

                    ScriviLogAggiornaImpreseParticelle("  ##Caso3##")

                    '3A. Aggiorno superficie riga lista + validità Fine

                    Dim indice = DeterminaIndiceDaProgressivoElaborazione(listaImpreseParticelleDaAggiornare, rigaListaIntersezioni.progElab)

                    ModificaImpreseParticelleDaAggiornareInLista(listaImpreseParticelleDaAggiornare(indice),
                                                                 modificaSuperficie:=True,
                                                                 superficie:=listaImpreseParticelleDaAggiornare(indice).superficie + datiContratto.superficie,
                                                                 modificaValiditaFine:=True,
                                                                 validitaFine:=datiContratto.validitaFine)

                    '3B. Inserisco riga lista precedente contratto

                    SeInserisciRigaListaPrecedenteContratto(datiContratto,
                                                            listaIntersezioni,
                                                            indiceLista,
                                                            listaImpreseParticelleDaAggiornare)

                    '3C. Inserisco riga lista successiva intersezione

                    InserisciRigaListaSuccessivaIntersezione(datiContratto,
                                                             rigaListaIntersezioni,
                                                             listaImpreseParticelleDaAggiornare)

                '--------------------------------------------------------------------------------
                'Caso 4 - Riga lista include intero contratto
                '--------------------------------------------------------------------------------

                Case rigaListaIntersezioni.validitaInizio < datiContratto.validitaInizio AndAlso
                     rigaListaIntersezioni.validitaFine > datiContratto.validitaFine

                    ScriviLogAggiornaImpreseParticelle("  ##Caso4##")

                    '4A. Aggiorno superficie riga lista + validità Inizio + validità Fine

                    Dim indice = DeterminaIndiceDaProgressivoElaborazione(listaImpreseParticelleDaAggiornare, rigaListaIntersezioni.progElab)

                    ModificaImpreseParticelleDaAggiornareInLista(listaImpreseParticelleDaAggiornare(indice),
                                                                 modificaSuperficie:=True,
                                                                 superficie:=listaImpreseParticelleDaAggiornare(indice).superficie + datiContratto.superficie,
                                                                 modificaValiditaInizio:=True,
                                                                 validitaInizio:=datiContratto.validitaInizio,
                                                                 modificaValiditaFine:=True,
                                                                 validitaFine:=datiContratto.validitaFine)

                    '4B. Inserisco riga lista precedente intersezione

                    InserisciRigaListaPrecedenteIntersezione(datiContratto,
                                                             rigaListaIntersezioni,
                                                             listaImpreseParticelleDaAggiornare)


                    '4C. Inserisco riga lista successiva intersezione

                    InserisciRigaListaSuccessivaIntersezione(datiContratto,
                                                             rigaListaIntersezioni,
                                                             listaImpreseParticelleDaAggiornare)

                Case Else

                    Dim messaggioEccezione = String.Format("Intersezione non gestita. " &
                                                           "Periodo Contratto: {0}-{1}. Periodo Intersezione: {2}-{3}.",
                                                           formattaDataInStringa(datiContratto.validitaInizio),
                                                           formattaDataInStringa(datiContratto.validitaFine),
                                                           formattaDataInStringa(rigaListaIntersezioni.validitaInizio),
                                                           formattaDataInStringa(rigaListaIntersezioni.validitaFine))

                    GeneraEccezione(messaggioEccezione)

            End Select

        Next

    End Sub

#Region "Inserisci Riga Lista Precedente"

    Private Sub SeInserisciRigaListaPrecedenteContratto(ByVal datiContratto As DatiDaAggiornare,
                                                        ByVal listaIntersezioni As List(Of ImpreseParticelleDaAggiornare),
                                                        ByVal indiceLista As Integer,
                                                        ByRef listaImpreseParticelleDaAggiornare As List(Of ImpreseParticelleDaAggiornare))

        Dim validitaInizio As Date? = Nothing

        If InserisciRigaListaPrecedenteContratto(datiContratto, listaIntersezioni, indiceLista, validitaInizio) Then

            Dim datiDaInserire = New DatiDaAggiornare With {
                .superficie = datiContratto.superficie,
                .validitaInizio = validitaInizio,
                .validitaFine = listaIntersezioni(indiceLista).validitaInizio.AddDays(-1)
            }

            InserisciImpreseParticelleDaAggiornareInLista(datiDaInserire, listaImpreseParticelleDaAggiornare)

        End If

    End Sub

    Private Function InserisciRigaListaPrecedenteContratto(ByVal datiContratto As DatiDaAggiornare,
                                                           ByVal listaIntersezioni As List(Of ImpreseParticelleDaAggiornare),
                                                           ByVal indiceLista As Integer,
                                                           ByRef validitaInizio As Date?)

        validitaInizio = Nothing

        Dim inserisciRigaListaPrec As Boolean = False
        Dim primoIndiceLista = 0

        Dim validitaFine = listaIntersezioni(indiceLista).validitaInizio.AddDays(-1)

        If indiceLista = primoIndiceLista Then

            'Se primo elemento, posso inserire in lista un elemento valido da inizio contratto

            inserisciRigaListaPrec = PeriodoValiditaCorretto(datiContratto.validitaInizio, validitaFine)
            validitaInizio = datiContratto.validitaInizio

        Else

            Dim validitaFinePrecedente = listaIntersezioni(indiceLista - 1).validitaFine

            Dim nuovaValiditaInizioDaFinePrecedente = validitaFinePrecedente.AddDays(1)

            'Se nuova validità da fine precedente è successiva a inizio contratto,
            'verifico se è possibile inserire un elemento in tale lasso temporale

            If nuovaValiditaInizioDaFinePrecedente > datiContratto.validitaInizio Then

                Dim validitaInizioRigaLista = listaIntersezioni(indiceLista).validitaInizio

                If DateDiff(DateInterval.Day, nuovaValiditaInizioDaFinePrecedente, validitaInizioRigaLista) > 0 Then

                    inserisciRigaListaPrec = True
                    validitaInizio = nuovaValiditaInizioDaFinePrecedente

                End If

            Else

                'Se l'elemento precedente è anteriore all'inizio contratto,
                'posso inserire in lista un elemento valido da inizio contratto

                inserisciRigaListaPrec = PeriodoValiditaCorretto(datiContratto.validitaInizio, validitaFine)
                validitaInizio = datiContratto.validitaInizio

            End If

        End If

        Return inserisciRigaListaPrec

    End Function

    Private Sub InserisciRigaListaPrecedenteIntersezione(ByVal datiContratto As DatiDaAggiornare,
                                                         ByVal rigaListaIntersezioni As ImpreseParticelleDaAggiornare,
                                                         ByRef listaImpreseParticelleDaAggiornare As List(Of ImpreseParticelleDaAggiornare))

        Dim datiDaInserire = New DatiDaAggiornare With {
            .superficie = rigaListaIntersezioni.superficie,
            .validitaInizio = rigaListaIntersezioni.validitaInizio,
            .validitaFine = datiContratto.validitaInizio.AddDays(-1)
        }

        InserisciImpreseParticelleDaAggiornareInLista(datiDaInserire, listaImpreseParticelleDaAggiornare)

    End Sub

#End Region

#Region "Inserisci Riga Lista Successiva"

    Private Sub SeInserisciRigaListaSuccessivaContratto(ByVal datiContratto As DatiDaAggiornare,
                                                        ByVal listaIntersezioni As List(Of ImpreseParticelleDaAggiornare),
                                                        ByVal indiceLista As Integer,
                                                        ByRef listaImpreseParticelleDaAggiornare As List(Of ImpreseParticelleDaAggiornare))

        Dim validitaFine As Date? = Nothing

        If InserisciRigaListaSuccessivaContratto(datiContratto, listaIntersezioni, indiceLista, validitaFine) Then

            Dim datiDaInserire = New DatiDaAggiornare With {
                .superficie = datiContratto.superficie,
                .validitaInizio = listaIntersezioni(indiceLista).validitaFine.AddDays(1),
                .validitaFine = validitaFine
            }

            InserisciImpreseParticelleDaAggiornareInLista(datiDaInserire, listaImpreseParticelleDaAggiornare)

        End If

    End Sub

    Private Function InserisciRigaListaSuccessivaContratto(ByVal datiContratto As DatiDaAggiornare,
                                                           ByVal listaIntersezioni As List(Of ImpreseParticelleDaAggiornare),
                                                           ByVal indiceLista As Integer,
                                                           ByRef validitaFine As Date?)

        validitaFine = Nothing

        Dim inserisciRigaListaSucc As Boolean = False
        Dim ultimoIndiceLista = listaIntersezioni.Count() - 1

        Dim validitaInizio = listaIntersezioni(indiceLista).validitaFine.AddDays(1)

        If indiceLista = ultimoIndiceLista Then

            'Se ultimo elemento, posso inserire in lista un elemento valido alla fine contratto

            inserisciRigaListaSucc = PeriodoValiditaCorretto(validitaInizio, datiContratto.validitaFine)
            validitaFine = datiContratto.validitaFine

        Else

            Dim validitaInizioSuccessiva = listaIntersezioni(indiceLista + 1).validitaInizio

            Dim nuovaValiditaFineDaInizioSuccessivo = validitaInizioSuccessiva.AddDays(-1)

            'Se nuova validità da inizio successivo è precedente a fine contratto,
            'verifico se è possibile inserire un elemento in tale lasso temporale

            If nuovaValiditaFineDaInizioSuccessivo < datiContratto.validitaFine Then

                Dim validitaFineRigaLista = listaIntersezioni(indiceLista).validitaFine

                If DateDiff(DateInterval.Day, validitaFineRigaLista, nuovaValiditaFineDaInizioSuccessivo) > 0 Then

                    inserisciRigaListaSucc = True
                    validitaFine = nuovaValiditaFineDaInizioSuccessivo

                End If

            Else

                'Se l'elemento successivo è posteriore alla fine contratto,
                'posso inserire in lista un elemento valido alla fine contratto

                inserisciRigaListaSucc = PeriodoValiditaCorretto(validitaInizio, datiContratto.validitaFine)
                validitaFine = datiContratto.validitaFine

            End If

        End If

        Return inserisciRigaListaSucc

    End Function

    Private Sub InserisciRigaListaSuccessivaIntersezione(ByVal datiContratto As DatiDaAggiornare,
                                                         ByVal rigaListaIntersezioni As ImpreseParticelleDaAggiornare,
                                                         ByRef listaImpreseParticelleDaAggiornare As List(Of ImpreseParticelleDaAggiornare))

        Dim datiDaInserire = New DatiDaAggiornare With {
            .superficie = rigaListaIntersezioni.superficie,
            .validitaInizio = datiContratto.validitaFine.AddDays(1),
            .validitaFine = rigaListaIntersezioni.validitaFine
        }

        InserisciImpreseParticelleDaAggiornareInLista(datiDaInserire, listaImpreseParticelleDaAggiornare)

    End Sub

#End Region

#Region "Inserimento/Modifica Lista Imprese Particelle Da Aggiornare"

    Private Sub InserisciImpreseParticelleDaAggiornareInLista(ByVal datiDaAggiornare As DatiDaAggiornare,
                                                              ByRef listaImpreseParticelleDaAggiornare As List(Of ImpreseParticelleDaAggiornare),
                                                              Optional ByVal idImprPart As Integer = 0)

        Dim impreseParticelleDaAggiornare = New ImpreseParticelleDaAggiornare With {
            .progElab = IncrementaProgressivoElaborazione(),
            .idImprPart = idImprPart,
            .superficie = datiDaAggiornare.superficie,
            .validitaInizio = datiDaAggiornare.validitaInizio,
            .validitaFine = datiDaAggiornare.validitaFine
        }

        listaImpreseParticelleDaAggiornare.Add(impreseParticelleDaAggiornare)

        ScriviLogImpreseParticelleDaAggiornare(enum_TipoOperazioneRiga.Inserimento, impreseParticelleDaAggiornare)

    End Sub

    Private Sub ModificaImpreseParticelleDaAggiornareInLista(ByRef impreseParticelleDaAggiornare As ImpreseParticelleDaAggiornare,
                                                             Optional ByVal modificaSuperficie As Boolean = False,
                                                             Optional ByVal modificaValiditaInizio As Boolean = False,
                                                             Optional ByVal modificaValiditaFine As Boolean = False,
                                                             Optional ByVal superficie As Decimal? = Nothing,
                                                             Optional ByVal validitaInizio As Date? = Nothing,
                                                             Optional ByVal validitaFine As Date? = Nothing)

        If modificaSuperficie AndAlso Not IsNothing(superficie) Then
            impreseParticelleDaAggiornare.superficie = superficie
        End If

        If modificaValiditaInizio AndAlso Not IsNothing(validitaInizio) Then
            impreseParticelleDaAggiornare.validitaInizio = validitaInizio
        End If

        If modificaValiditaFine AndAlso Not IsNothing(validitaFine) Then
            impreseParticelleDaAggiornare.validitaFine = validitaFine
        End If

        If modificaSuperficie OrElse modificaValiditaInizio OrElse modificaValiditaFine Then

            ScriviLogImpreseParticelleDaAggiornare(enum_TipoOperazioneRiga.Modifica,
                                                   impreseParticelleDaAggiornare,
                                                   flagModificaSuperficie:=modificaSuperficie,
                                                   flagModificaValiditaInizio:=modificaValiditaInizio,
                                                   flagModificaValiditaFine:=modificaValiditaFine)

        End If

    End Sub

#End Region

#Region "Controlli Pre-Aggiornamento Imprese Particelle"

    Private Sub ControlloDatiOmogeneiImpreseParticelle(ByVal dtImpreseParticelle As DataTable,
                                                       ByVal idImprPartPilota As Integer,
                                                       ByVal impreseParticellePilota As DataRow,
                                                       ByRef listaIdImpreseParticelle As List(Of Integer)
                                                       )

        For Each rigaImpreseParticelle In dtImpreseParticelle.Rows

            Dim idImprPart = CInt(rigaImpreseParticelle.Item("ID"))

            If idImprPart <> idImprPartPilota Then

                listaIdImpreseParticelle.Add(idImprPart)

                Dim elencoColonneDaControllare As String() = {"TitoloPossesso", "PARTITA_CATASTALE", "Sup_Spandibile", "Sup_Divieto"}

                Dim elencoColonneNonOmogenee As String = ""

                For Each colonna In elencoColonneDaControllare

                    If rigaImpreseParticelle.Item(colonna) <> impreseParticellePilota.Item(colonna) Then

                        If elencoColonneNonOmogenee <> "" Then
                            elencoColonneNonOmogenee += ", "
                        End If
                        elencoColonneNonOmogenee += colonna

                    End If

                Next

                If elencoColonneNonOmogenee <> "" Then

                    Dim messaggioEccezione = String.Format("Dati imprese per particelle non omogenei: {0} (IDCorrente={1} IDPilota={2})",
                                                           elencoColonneNonOmogenee,
                                                           idImprPart,
                                                           idImprPartPilota)
                    GeneraEccezione(messaggioEccezione)

                End If

            End If

        Next

    End Sub

    Private Sub ControlloPresenzaContatti(ByVal listaIdImpreseParticelle As List(Of Integer),
                                          ByRef leggiImpreseParticelle As ImpresexParticelle2_R)

        Dim elencoId = String.Join(",", listaIdImpreseParticelle)

        Dim filtroAggiuntivoContatti = String.Format(" IPC.ID IN ({0}) ", elencoId)

        Dim dtImpreseParticelleContatti = leggiImpreseParticelle.Leggi_Contatti(0,
                                                                                "",
                                                                                0,
                                                                                "",
                                                                                "",
                                                                                "",
                                                                                0,
                                                                                0,
                                                                                "",
                                                                                filtroAggiuntivoContatti,
                                                                                "",
                                                                                _objParametriServer)

        If SeTabellaConRighe(dtImpreseParticelleContatti) Then

            Dim messaggioEccezione = String.Format("Esistono contatti particella ({0}) per le imprese particelle da elaborare (ID={1})",
                                                   dtImpreseParticelleContatti.Rows.Count,
                                                   elencoId)

            GeneraEccezione(messaggioEccezione)

        End If

    End Sub

    Private Sub ControlloOmogeneitaCodiciParticella(ByVal listaIdImpreseParticelle As List(Of Integer),
                                                    ByVal codImprPartPilota As String)

        Dim elencoId = String.Join(",", listaIdImpreseParticelle)

        Dim filtroAggiuntivoCodici = String.Format(" IPC.ID IN ({0}) AND IPC.Val_Cod <> '{1}' ",
                                                   elencoId,
                                                   codImprPartPilota)

        Dim leggiImpreseParticelleCodici As New ImpresexParticelle_Codici_R

        Dim dtImpreseParticelleCodici = leggiImpreseParticelleCodici.Leggi(0,
                                                                           "",
                                                                           0,
                                                                           "",
                                                                           "",
                                                                           "",
                                                                           0,
                                                                           0,
                                                                           "",
                                                                           0,
                                                                           "",
                                                                           enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                           filtroAggiuntivoCodici,
                                                                           "",
                                                                           _objParametriServer)

        If SeTabellaConRighe(dtImpreseParticelleCodici) Then

            Dim elencoCodici As New List(Of String)

            For Each rigaCodice As DataRow In dtImpreseParticelleCodici.Rows

                Dim Codice = rigaCodice.Item("Val_Cod")

                If Not elencoCodici.Contains(Codice) Then

                    elencoCodici.Add(Codice)

                End If

            Next

            Dim messaggioEccezione = String.Format("Esistono codici particella ({0}) diversi da {1} per le imprese particelle da elaborare (ID={2})",
                                                   String.Join(",", elencoCodici),
                                                   codImprPartPilota,
                                                   elencoId)

            GeneraEccezione(messaggioEccezione)

        End If

    End Sub

#End Region

#Region "Aggiornamento Imprese Particelle"

    Private Sub CancellaImpreseParticelle(ByVal dtImpreseParticelle As DataTable,
                                          ByVal idImprPartPilota As Integer,
                                          ByRef scriviImpreseParticelle As ImpresexParticelle2_W,
                                          ByRef scriviImpreseParticelleCodici As ImpresexParticelle_Codici_W)

        ScriviLogAggiornaImpreseParticelle(_separatore)
        ScriviLogAggiornaImpreseParticelle("Cancellazione imprese particelle:")

        For Each rigaImpreseParticelle In dtImpreseParticelle.Rows

            Dim idImprPart = CInt(rigaImpreseParticelle.Item("ID"))

            If idImprPart <> idImprPartPilota Then

                CancellaSingolaImpresaParticella(rigaImpreseParticelle,
                                                 scriviImpreseParticelle,
                                                 scriviImpreseParticelleCodici)

            End If

        Next

    End Sub

    Private Sub CancellaSingolaImpresaParticella(ByVal rigaImpreseParticelle As DataRow,
                                                 ByRef scriviImpreseParticelle As ImpresexParticelle2_W,
                                                 ByRef scriviImpreseParticelleCodici As ImpresexParticelle_Codici_W)

        ScriviLogRigaImpreseParticelle(rigaImpreseParticelle)

        Dim idImprPart = CInt(rigaImpreseParticelle.Item("ID"))

        Dim filtroAggiuntivo = String.Format("ImpresexParticelle.ID = {0}", idImprPart)

        scriviImpreseParticelle.Cancella(rigaImpreseParticelle.Item("Piva"),
                                         rigaImpreseParticelle.Item("Sa_Cod"),
                                         rigaImpreseParticelle.Item("PROV"),
                                         rigaImpreseParticelle.Item("COM"),
                                         rigaImpreseParticelle.Item("SEZIONE"),
                                         rigaImpreseParticelle.Item("FOGLIO"),
                                         rigaImpreseParticelle.Item("NUMERO"),
                                         rigaImpreseParticelle.Item("SUBALTERNO"),
                                         filtroAggiuntivo,
                                         _objParametriServer)

        scriviImpreseParticelleCodici.Cancella(idImprPart,
                                               enum_CodiciAnagrafe.CodiceParticella,
                                               "",
                                               _objParametriServer)

    End Sub

    Private Function SeControlliPreventiviCancellazioneImpresaParticella(ByVal rigaImpreseParticelle As DataRow,
                                                                         ByRef leggiImpreseParticelle As ImpresexParticelle2_R) As Boolean

        Dim particellaCancellabile = True

        'Controllo se è l'ultima impresa particella per impresa/centro

        Dim dtImpreseParticelle = leggiImpreseParticelle.Leggi(0,
                                                               rigaImpreseParticelle.Item("PIVA"),
                                                               rigaImpreseParticelle.Item("Sa_Cod"),
                                                               0,
                                                               rigaImpreseParticelle.Item("PROV"),
                                                               rigaImpreseParticelle.Item("COM"),
                                                               rigaImpreseParticelle.Item("Sezione"),
                                                               rigaImpreseParticelle.Item("Foglio"),
                                                               rigaImpreseParticelle.Item("Numero"),
                                                               rigaImpreseParticelle.Item("Subalterno"),
                                                               enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                               "",
                                                               "",
                                                               _objParametriServer)

        If SeTabellaConRighe(dtImpreseParticelle) AndAlso dtImpreseParticelle.Rows.Count = 1 Then

            particellaCancellabile = ControlliPreventiviCancellazioneImpresaParticella(rigaImpreseParticelle)

        End If

        Return particellaCancellabile

    End Function

    Private Function ControlliPreventiviCancellazioneImpresaParticella(ByVal rigaImpreseParticelle As DataRow) As Boolean

        ScriviLogAggiornaImpreseParticelle(_separatore)
        ScriviLogAggiornaImpreseParticelle("Controlli preventivi cancellazione imprese particelle pilota:")

        Dim objParticelle As New ParticelleCatastali_R

        Dim particellaCancellabile = objParticelle.CheckParticellaPerCancellazione(rigaImpreseParticelle.Item("Piva"),
                                                                                   rigaImpreseParticelle.Item("Sa_Cod"),
                                                                                   rigaImpreseParticelle.Item("PROV"),
                                                                                   rigaImpreseParticelle.Item("COM"),
                                                                                   rigaImpreseParticelle.Item("SEZIONE"),
                                                                                   rigaImpreseParticelle.Item("FOGLIO"),
                                                                                   rigaImpreseParticelle.Item("NUMERO"),
                                                                                   rigaImpreseParticelle.Item("SUBALTERNO"),
                                                                                   _objParametriServer)

        If Not particellaCancellabile Then

            Dim messaggio = String.Format("Particella {0} : cancellazione NON consentita. " &
                                          "La particella risulta associata a SQNPI, GIS o Analisi. " &
                                          "Rimuovere l'associazione per poter procedere alla cancellazione.",
                                          ComponiDescrizioneParticella(rigaImpreseParticelle))

            ScriviLogAggiornaImpreseParticelle(messaggio)

        End If

        Return particellaCancellabile

    End Function

    Private Sub CancellazioneLegamiAppezzamentiCampi(ByVal objRigheScoperteParticella As RigheScoperteParticella)

        If objRigheScoperteParticella.righeAppezzamenti > 0 Then

            Dim objAppezzaXParticelle As New AppezzaxParticelle_W

            For Each appezzaXparticella In objRigheScoperteParticella.dtAppezzamenti.Rows

                objAppezzaXParticelle.Cancella(appezzaXparticella.item("Piva"),
                                               appezzaXparticella.Item("Sa_Cod"),
                                               appezzaXparticella.Item("Appezza"),
                                               appezzaXparticella.Item("PROV"),
                                               appezzaXparticella.Item("COM"),
                                               appezzaXparticella.Item("Sezione"),
                                               appezzaXparticella.Item("Foglio"),
                                               appezzaXparticella.Item("Numero"),
                                               appezzaXparticella.Item("Subalterno"),
                                               "",
                                               _objParametriServer)

                Dim messaggioLog = String.Format("Cancellazione legame appezzamento: {0} dal {1} al {2} [ Piva={3} SaCod={4} Appezza={5} ]",
                                 appezzaXparticella.Item("App_Nome"),
                                 formattaDataInStringa(CDate(appezzaXparticella.item("xValidita_Inizio"))),
                                 formattaDataInStringa(CDate(appezzaXparticella.item("xValidita_Fine"))),
                                 appezzaXparticella.Item("Piva"),
                                 appezzaXparticella.Item("Sa_Cod"),
                                 appezzaXparticella.Item("Appezza"))

                ScriviLogAggiornaImpreseParticelle(messaggioLog)

            Next

        End If

        If objRigheScoperteParticella.righeCampi > 0 Then

            Dim objCampiXParticelle As New CampixParticelle_W

            For Each campoXparticella In objRigheScoperteParticella.dtCampi.Rows

                objCampiXParticelle.Cancella(campoXparticella.item("Piva"),
                                             campoXparticella.Item("Sa_Cod"),
                                             campoXparticella.Item("Campo_Cod"),
                                             campoXparticella.Item("PROV"),
                                             campoXparticella.Item("COM"),
                                             campoXparticella.Item("Sezione"),
                                             campoXparticella.Item("Foglio"),
                                             campoXparticella.Item("Numero"),
                                             campoXparticella.Item("Subalterno"),
                                             "",
                                             _objParametriServer)

                Dim messaggioLog = String.Format("Cancellazione legame campo: {0} dal {1} al {2} [ Piva={3} SaCod={4} CampoCod={5} ]",
                                 campoXparticella.Item("Campo_Des"),
                                 formattaDataInStringa(CDate(campoXparticella.item("xValidita_Inizio"))),
                                 formattaDataInStringa(CDate(campoXparticella.item("xValidita_Fine"))),
                                 campoXparticella.Item("Piva"),
                                 campoXparticella.Item("Sa_Cod"),
                                 campoXparticella.Item("Campo_Cod"))

                ScriviLogAggiornaImpreseParticelle(messaggioLog)

            Next

        End If

    End Sub

    Private Sub InserisciImpreseParticelle(ByVal impreseParticellePilota As DataRow,
                                           ByRef dtImpreseParticelleCodiciPilota As DataTable,
                                           ByRef dtImpreseParticelleContattiPilota As DataTable,
                                           ByVal rigaImpreseParticelle As ImpreseParticelleDaAggiornare,
                                           ByRef scriviImpreseParticelle As ImpresexParticelle2_W,
                                           ByRef leggiImpreseParticelle As ImpresexParticelle2_R,
                                           ByRef scriviImpreseParticelleCodici As ImpresexParticelle_Codici_W)

        '--------------------------------------------------
        'Inserimento Imprese x Particelle
        '--------------------------------------------------

        scriviImpreseParticelle.Scrivi_2(impreseParticellePilota.Item("PIVA"),
                                         impreseParticellePilota.Item("Sa_Cod"),
                                         impreseParticellePilota.Item("PROV"),
                                         impreseParticellePilota.Item("COM"),
                                         impreseParticellePilota.Item("Sezione"),
                                         impreseParticellePilota.Item("Foglio"),
                                         impreseParticellePilota.Item("Numero"),
                                         impreseParticellePilota.Item("Subalterno"),
                                         impreseParticellePilota.Item("Partita_Catastale"),
                                         impreseParticellePilota.Item("TitoloPossesso"),
                                         rigaImpreseParticelle.superficie,
                                         rigaImpreseParticelle.validitaInizio,
                                         rigaImpreseParticelle.validitaFine,
                                         _objParametriServer)

        'Rileggo per ottenere ID rk appena inserito (eventualmente usare EF per evitare tale lettura)

        Dim filtroAggiuntivo = String.Format("ImpresexParticelle.TitoloPossesso = {0} And " &
                                             "ImpresexParticelle.Validita_Inizio = {1} And " &
                                             "ImpresexParticelle.Validita_Fine = {2} And " &
                                             "ImpresexParticelle.ID <> {3}",
                                             enum_TitoloPossesso.AffittoContratto.ToString(_valoreEnum),
                                             Agro_SQL_SaveDate(rigaImpreseParticelle.validitaInizio),
                                             Agro_SQL_SaveDate(rigaImpreseParticelle.validitaFine),
                                             impreseParticellePilota.Item("ID"))

        Dim dtImpreseParticelleInserito = leggiImpreseParticelle.Leggi(0,
                                                                       impreseParticellePilota.Item("PIVA"),
                                                                       impreseParticellePilota.Item("Sa_Cod"),
                                                                       0,
                                                                       impreseParticellePilota.Item("PROV"),
                                                                       impreseParticellePilota.Item("COM"),
                                                                       impreseParticellePilota.Item("Sezione"),
                                                                       impreseParticellePilota.Item("Foglio"),
                                                                       impreseParticellePilota.Item("Numero"),
                                                                       impreseParticellePilota.Item("Subalterno"),
                                                                       enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                       filtroAggiuntivo,
                                                                       "",
                                                                       _objParametriServer)

        ControllaEsitoLettura(dtImpreseParticelleInserito, enum_NomeTabella.ImpreseXParticelle)

        '--------------------------------------------------
        'Inserimento Imprese x Particelle Codici
        '--------------------------------------------------

        If IsNothing(dtImpreseParticelleCodiciPilota) Then

            dtImpreseParticelleCodiciPilota = LetturaImpreseParticelleCodici(impreseParticellePilota)

        End If

        If Not IsNothing(dtImpreseParticelleCodiciPilota) Then

            scriviImpreseParticelleCodici.Scrivi(dtImpreseParticelleInserito(0).Item("Id"),
                                                 dtImpreseParticelleCodiciPilota(0).Item("Id_Cod"),
                                                 dtImpreseParticelleCodiciPilota(0).Item("Val_Cod"),
                                                 AGRODATAINIZIO,
                                                 AGRODATAFINE,
                                                 _objParametriServer)

        End If

        ScriviLogImpreseParticelleAggiornate(enum_TipoOperazioneRiga.Inserimento,
                                             rigaImpreseParticelle,
                                             dtImpreseParticelleInserito(0).Item("Id"))

    End Sub

    Private Function LetturaImpreseParticelleCodici(ByVal impreseParticellePilota As DataRow) As DataTable

        Dim leggiImpreseParticelleCodici As New ImpresexParticelle_Codici_R

        Dim dtImpreseParticelleCodiciPilota = leggiImpreseParticelleCodici.Leggi(impreseParticellePilota.Item("ID"),
                                                                                 impreseParticellePilota.Item("PIVA"),
                                                                                 impreseParticellePilota.Item("Sa_Cod"),
                                                                                 impreseParticellePilota.Item("PROV"),
                                                                                 impreseParticellePilota.Item("COM"),
                                                                                 impreseParticellePilota.Item("Sezione"),
                                                                                 impreseParticellePilota.Item("Foglio"),
                                                                                 impreseParticellePilota.Item("Numero"),
                                                                                 impreseParticellePilota.Item("Subalterno"),
                                                                                 0,
                                                                                 "",
                                                                                 enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                                 "",
                                                                                 "",
                                                                                 _objParametriServer)

        ControllaEsitoLettura(dtImpreseParticelleCodiciPilota, enum_NomeTabella.ImpreseXParticelle_Codici)

        Return dtImpreseParticelleCodiciPilota

    End Function

    Public Function Estrazione_CatastoAffitti(ByVal piva As String, ByVal dataDal As String, ByVal dataAl As String, ByVal centriAziendali As String) As DataTable

        Dim leggiCatastoAffitti As New ContrattiXImpreseXParticelle_R

        Dim tempDataInizio As DateTime = AGRODATAINIZIO
        Dim tempDataFine As DateTime = AGRODATAFINE

        'Casi data Dal/Al = NULL
        If (dataDal = "") Then
            dataDal = tempDataInizio.ToString("dd/MM/yyyy")
        End If

        If (dataAl = "") Then
            dataAl = tempDataFine.ToString("dd/MM/yyyy")
        End If

        'Chiama la funzione nel DAL
        Dim dtCatastoAffittiPilota = leggiCatastoAffitti.Estrazione_CatastoAffitti_All(piva, dataDal, dataAl, centriAziendali, _objParametriServer)

        Return dtCatastoAffittiPilota

    End Function

    Private Sub ControllaEsitoLettura(ByVal dtLettura As DataTable, ByVal nomeTabella As enum_NomeTabella)

        Dim messaggioEccezione As String = ""

        If IsNothing(dtLettura) OrElse dtLettura.Rows.Count = 0 OrElse dtLettura.Rows.Count > 1 Then

            Dim descrizioneTabella As String = ""

            Select Case nomeTabella

                Case enum_NomeTabella.ImpreseXParticelle
                    descrizioneTabella = "Particelle Per Impresa"

                Case enum_NomeTabella.ImpreseXParticelle_Codici
                    descrizioneTabella = "Codici Particelle Per Impresa"

                Case enum_NomeTabella.ImpreseXParticelle_Contatti
                    descrizioneTabella = "Contatti Particelle Per Impresa"

            End Select

            Select Case True

                Case IsNothing(dtLettura)
                    messaggioEccezione = String.Format("Errore in lettura {0} (tabella: {1})",
                                                       descrizioneTabella,
                                                       nomeTabella.ToString(_descrizioneEnum))

                Case dtLettura.Rows.Count = 0
                    messaggioEccezione = String.Format("Nessun elemento trovato in {0} (tabella: {1})",
                                                       descrizioneTabella,
                                                       nomeTabella.ToString(_descrizioneEnum))

                Case dtLettura.Rows.Count > 1
                    messaggioEccezione = String.Format("Più elementi trovati in {0} (tabella: {1})",
                                                       descrizioneTabella,
                                                       nomeTabella.ToString(_descrizioneEnum))

            End Select

            GeneraEccezione(messaggioEccezione)

        End If

    End Sub

    Private Sub AggiornaImpreseParticellePilota(ByVal impreseParticellePilota As DataRow,
                                                ByVal rigaImpreseParticelle As ImpreseParticelleDaAggiornare,
                                                ByRef scriviImpreseParticelle As ImpresexParticelle2_W)

        scriviImpreseParticelle.Modifica_2(rigaImpreseParticelle.idImprPart,
                                           impreseParticellePilota.Item("PIVA"),
                                           impreseParticellePilota.Item("Sa_Cod"),
                                           impreseParticellePilota.Item("PROV"),
                                           impreseParticellePilota.Item("COM"),
                                           impreseParticellePilota.Item("Sezione"),
                                           impreseParticellePilota.Item("Foglio"),
                                           impreseParticellePilota.Item("Numero"),
                                           impreseParticellePilota.Item("Subalterno"),
                                           impreseParticellePilota.Item("Partita_Catastale"),
                                           impreseParticellePilota.Item("TitoloPossesso"),
                                           rigaImpreseParticelle.superficie,
                                           rigaImpreseParticelle.validitaInizio,
                                           rigaImpreseParticelle.validitaFine,
                                           "",
                                           _objParametriServer)

        ScriviLogImpreseParticelleAggiornate(enum_TipoOperazioneRiga.Modifica,
                                             rigaImpreseParticelle,
                                             rigaImpreseParticelle.idImprPart)

    End Sub

#End Region

#Region "Aggiornamento Imprese Particelle Pilota"

    Private Function AggiornamentoImpreseParticellePilota(ByVal piva As String,
                                                          ByVal Particella As ContrattiXImpreseXParticelle_Particella,
                                                          ByRef listaPeriodiContratto As List(Of ContrattiXImpreseXParticelle_Periodo),
                                                          ByVal impreseParticellePilota As DataRow,
                                                          ByRef leggiContrattiImpreseParticelle As ContrattiXImpreseXParticelle_R,
                                                          ByRef scriviImpreseParticelle As ImpresexParticelle2_W,
                                                          ByRef scriviImpreseParticelleCodici As ImpresexParticelle_Codici_W,
                                                          ByVal forzaCancellazioneLegamiAppezzamentiCampi As enum_forzaCancella,
                                                          ByRef leggiImpreseParticelle As ImpresexParticelle2_R,
                                                          ByVal superficeCatastale As Decimal
                                                          ) As enum_TipoAgg

        Dim aggiornamentoParticellaPilota = enum_TipoAgg.Nessuno

        Dim particellaCancellabile = True

        'Verifico se imprese particelle pilota è presente in contratti di affitto in base al suo periodo di validità

        Dim listaPeriodiContrattoPilota As New List(Of ContrattiXImpreseXParticelle_Periodo)

        listaPeriodiContratto.Add(CreaPeriodoContratto(impreseParticellePilota.Item("Validita_Inizio"),
                                                       impreseParticellePilota.Item("Validita_Fine")))

        Dim dtContrattiImpreseParticellePilota = leggiContrattiImpreseParticelle.LeggiPeriodiValidita(piva,
                                                                                                      0,
                                                                                                      0,
                                                                                                      0,
                                                                                                      Particella,
                                                                                                      listaPeriodiContratto,
                                                                                                      "",
                                                                                                      "",
                                                                                                      _objParametriServer)

        'Se non presente, controllo se imprese particelle usato in appezzamenti/campi

        If IsNothing(dtContrattiImpreseParticellePilota) OrElse dtContrattiImpreseParticellePilota.Rows.Count = 0 Then

            Dim objRigheScoperteParticella As New RigheScoperteParticella

            LetturaAppezzamentiParticella(impreseParticellePilota, objRigheScoperteParticella)

            LetturaCampiParticella(impreseParticellePilota, objRigheScoperteParticella)

            'Verifico se è possibile cancellare la particella

            If objRigheScoperteParticella.NessunaRigaScoperta() Then

                particellaCancellabile = SeControlliPreventiviCancellazioneImpresaParticella(impreseParticellePilota, leggiImpreseParticelle)

            End If

            'Controllo per eventuale richiesta conferma da parte dell'utente

            If particellaCancellabile AndAlso forzaCancellazioneLegamiAppezzamentiCampi = enum_forzaCancella.Indefinita AndAlso objRigheScoperteParticella.EsistonoRigheScoperte() Then

                Dim messaggioEccezione = ComponiMessaggioAppezzamentiCampi(impreseParticellePilota, objRigheScoperteParticella, enum_contesto.CancellazioneUltimaRigaContratto)

                GeneraEccezioneParticellaConLegami(messaggioEccezione)

            End If

            'Azione finale su imprese particella pilota

            ScriviLogAggiornaImpreseParticelle(_separatore)

            If particellaCancellabile AndAlso (forzaCancellazioneLegamiAppezzamentiCampi = enum_forzaCancella.Si OrElse objRigheScoperteParticella.NessunaRigaScoperta()) Then

                '----------------------------------------------------------------------------------------------------
                'Se prevista forzatura cancellazione o non presenti appezzamenti/campi -> Cancello
                '----------------------------------------------------------------------------------------------------

                ScriviLogAggiornaImpreseParticelle("Cancellazione imprese particelle pilota:")

                CancellaSingolaImpresaParticella(impreseParticellePilota,
                                                 scriviImpreseParticelle,
                                                 scriviImpreseParticelleCodici)

                If objRigheScoperteParticella.EsistonoRigheScoperte() Then
                    CancellazioneLegamiAppezzamentiCampi(objRigheScoperteParticella)
                End If

                aggiornamentoParticellaPilota = enum_TipoAgg.Cancellazione

            Else

                '----------------------------------------------------------------------------------------------------
                'Se presenti appezzamenti/campi -> Modifico
                '----------------------------------------------------------------------------------------------------

                ScriviLogAggiornaImpreseParticelle("Modifica imprese particelle pilota:")

                ModificaImpresaParticellaPilota(impreseParticellePilota,
                                                objRigheScoperteParticella,
                                                scriviImpreseParticelle,
                                                superficeCatastale)

                aggiornamentoParticellaPilota = enum_TipoAgg.Modifica

            End If

        End If

        Return aggiornamentoParticellaPilota

    End Function

    Private Sub LetturaAppezzamentiParticella(ByVal impreseParticellePilota As DataRow,
                                              ByRef objRigheScoperteParticella As RigheScoperteParticella)

        objRigheScoperteParticella.dtAppezzamenti = Nothing

        objRigheScoperteParticella.righeAppezzamenti = 0

        '--------------------------------------------------------------------------------
        'Lettura appezzamenti per particella
        '--------------------------------------------------------------------------------
        'Controllo se per la particella pilota, nel relativo periodo di validità,
        'esistono appezzamenti per i quali NON esistono altre imprese x particelle
        'nel periodo.
        '--------------------------------------------------------------------------------

        Dim leggiAppezzamentiParticella As New AppezzaxParticelle_R

        Dim objModificaFinestraTemporale As New ModificaFinestraTemporale

        objModificaFinestraTemporale.Modifica(impreseParticellePilota.Item("Validita_Inizio"),
                                              impreseParticellePilota.Item("Validita_Fine"),
                                              _objParametriServer)

        Try

            objRigheScoperteParticella.dtAppezzamenti = leggiAppezzamentiParticella.AppezzamentixParticelle_Leggi(impreseParticellePilota.Item("piva"),
                                                                                                                  impreseParticellePilota.Item("sa_cod"),
                                                                                                                  0,
                                                                                                                  impreseParticellePilota.Item("PROV"),
                                                                                                                  impreseParticellePilota.Item("COM"),
                                                                                                                  impreseParticellePilota.Item("SEZIONE"),
                                                                                                                  impreseParticellePilota.Item("FOGLIO"),
                                                                                                                  impreseParticellePilota.Item("NUMERO"),
                                                                                                                  impreseParticellePilota.Item("SUBALTERNO"),
                                                                                                                  enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                                                                  "",
                                                                                                                  "",
                                                                                                                  _objParametriServer,
                                                                                                                  False,
                                                                                                                  joinImpreseParticelle:=True,
                                                                                                                  idImpreseParticelleDaEscludere:=impreseParticellePilota.Item("ID"),
                                                                                                                  filtroImpreseParticelleAssenti:=True)

            If Not IsNothing(objRigheScoperteParticella.dtAppezzamenti) Then
                objRigheScoperteParticella.righeAppezzamenti = objRigheScoperteParticella.dtAppezzamenti.Rows.Count
            End If

        Catch ex As GiasException

            Throw ex

        Catch ex As Exception

            Throw New Exception(ex.Message)

        Finally

            objModificaFinestraTemporale.Ripristina(_objParametriServer)

        End Try

    End Sub

    Private Sub LetturaCampiParticella(ByVal impreseParticellePilota As DataRow,
                                       ByRef objRigheScoperteParticella As RigheScoperteParticella)

        objRigheScoperteParticella.dtCampi = Nothing

        objRigheScoperteParticella.righeCampi = 0

        '--------------------------------------------------------------------------------
        'Lettura campi per particella
        '--------------------------------------------------------------------------------
        'Controllo se per la particella pilota, nel relativo periodo di validità,
        'esistono campi per i quali NON esistono altre imprese x particelle
        'nel periodo.
        '--------------------------------------------------------------------------------

        Dim leggiCampiParticella As New CampixParticelle_R

        Dim objModificaFinestraTemporale As New ModificaFinestraTemporale

        objModificaFinestraTemporale.Modifica(impreseParticellePilota.Item("Validita_Inizio"),
                                              impreseParticellePilota.Item("Validita_Fine"),
                                              _objParametriServer)

        Try

            objRigheScoperteParticella.dtCampi = leggiCampiParticella.Leggi(impreseParticellePilota.Item("piva"),
                                                                            impreseParticellePilota.Item("sa_cod"),
                                                                            0,
                                                                            impreseParticellePilota.Item("PROV"),
                                                                            impreseParticellePilota.Item("COM"),
                                                                            impreseParticellePilota.Item("SEZIONE"),
                                                                            impreseParticellePilota.Item("FOGLIO"),
                                                                            impreseParticellePilota.Item("NUMERO"),
                                                                            impreseParticellePilota.Item("SUBALTERNO"),
                                                                            enumSelezioneVariabile.Selezione_JoinCompleta,
                                                                            "",
                                                                            "",
                                                                            _objParametriServer,
                                                                            joinImpreseParticelle:=True,
                                                                            idImpreseParticelleDaEscludere:=impreseParticellePilota.Item("ID"),
                                                                            filtroImpreseParticelleAssenti:=True)

            If Not IsNothing(objRigheScoperteParticella.dtCampi) Then
                objRigheScoperteParticella.righeCampi = objRigheScoperteParticella.dtCampi.Rows.Count
            End If

        Catch ex As GiasException

            Throw ex

        Catch ex As Exception

            Throw New Exception(ex.Message)

        Finally

            objModificaFinestraTemporale.Ripristina(_objParametriServer)

        End Try

    End Sub

    Private Sub ModificaImpresaParticellaPilota(ByVal impreseParticellePilota As DataRow,
                                                ByVal objRigheScoperteParticella As RigheScoperteParticella,
                                                ByRef scriviImpreseParticelle As ImpresexParticelle2_W,
                                                ByVal superficeCatastale As Decimal)

        Dim datiDaModificareSuPilota As New ImpreseParticelleDaAggiornare

        Dim datiAppezzamenti As New ImpreseParticelleDaAggiornare

        Dim datiCampi As New ImpreseParticelleDaAggiornare

        'Lettura dati appezzamenti/campi

        If objRigheScoperteParticella.righeAppezzamenti > 0 Then
            datiAppezzamenti.superficie = objRigheScoperteParticella.dtAppezzamenti.Compute("Max(Area)", "")
            datiAppezzamenti.validitaInizio = objRigheScoperteParticella.dtAppezzamenti.Compute("Min(Validita_Inizio)", "")
            datiAppezzamenti.validitaFine = objRigheScoperteParticella.dtAppezzamenti.Compute("Max(Validita_Fine)", "")
        End If

        If objRigheScoperteParticella.righeCampi > 0 Then
            datiCampi.superficie = objRigheScoperteParticella.dtCampi.Compute("Max(Area)", "")
            datiCampi.validitaInizio = objRigheScoperteParticella.dtCampi.Compute("Min(Validita_Inizio)", "")
            datiCampi.validitaFine = objRigheScoperteParticella.dtCampi.Compute("Max(Validita_Fine)", "")
        End If

        'Attribuzione dati da modificare

        Select Case True

            Case objRigheScoperteParticella.righeAppezzamenti > 0 AndAlso objRigheScoperteParticella.righeCampi = 0

                datiDaModificareSuPilota.superficie = datiAppezzamenti.superficie
                datiDaModificareSuPilota.validitaInizio = datiAppezzamenti.validitaInizio
                datiDaModificareSuPilota.validitaFine = datiAppezzamenti.validitaFine

            Case objRigheScoperteParticella.righeAppezzamenti = 0 AndAlso objRigheScoperteParticella.righeCampi > 0

                datiDaModificareSuPilota.superficie = datiCampi.superficie
                datiDaModificareSuPilota.validitaInizio = datiCampi.validitaInizio
                datiDaModificareSuPilota.validitaFine = datiCampi.validitaFine

            Case objRigheScoperteParticella.righeAppezzamenti > 0 AndAlso objRigheScoperteParticella.righeCampi > 0

                If datiAppezzamenti.superficie > datiCampi.superficie Then
                    datiDaModificareSuPilota.superficie = datiAppezzamenti.superficie
                Else
                    datiDaModificareSuPilota.superficie = datiCampi.superficie
                End If
                If datiAppezzamenti.validitaInizio < datiCampi.validitaInizio Then
                    datiDaModificareSuPilota.validitaInizio = datiAppezzamenti.validitaInizio
                Else
                    datiDaModificareSuPilota.validitaInizio = datiCampi.validitaInizio
                End If
                If datiAppezzamenti.validitaFine > datiCampi.validitaFine Then
                    datiDaModificareSuPilota.validitaFine = datiAppezzamenti.validitaFine
                Else
                    datiDaModificareSuPilota.validitaFine = datiCampi.validitaFine
                End If

            Case Else

                datiDaModificareSuPilota.superficie = superficeCatastale
                datiDaModificareSuPilota.validitaInizio = AGRODATAINIZIO
                datiDaModificareSuPilota.validitaFine = AGRODATAFINE

        End Select

        'Aggiorna particella pilota

        scriviImpreseParticelle.Modifica_2(impreseParticellePilota.Item("ID"),
                                           impreseParticellePilota.Item("PIVA"),
                                           impreseParticellePilota.Item("Sa_Cod"),
                                           impreseParticellePilota.Item("PROV"),
                                           impreseParticellePilota.Item("COM"),
                                           impreseParticellePilota.Item("Sezione"),
                                           impreseParticellePilota.Item("Foglio"),
                                           impreseParticellePilota.Item("Numero"),
                                           impreseParticellePilota.Item("Subalterno"),
                                           impreseParticellePilota.Item("Partita_Catastale"),
                                           impreseParticellePilota.Item("TitoloPossesso"),
                                           datiDaModificareSuPilota.superficie,
                                           datiDaModificareSuPilota.validitaInizio,
                                           datiDaModificareSuPilota.validitaFine,
                                           "",
                                           _objParametriServer)

        ScriviLogImpreseParticelleAggiornate(enum_TipoOperazioneRiga.Modifica,
                                             datiDaModificareSuPilota,
                                             impreseParticellePilota.Item("ID"))

    End Sub

#End Region

#Region "Controlli Post-Aggiornamento Imprese Particelle"

    Private Sub ControlloGlobaleSovrapposizioniParticella(ByVal piva As String,
                                                          ByVal saCod As Integer,
                                                          ByVal particella As ContrattiXImpreseXParticelle_Particella,
                                                          ByRef leggiImpreseParticelle As ImpresexParticelle2_R)

        Dim dtImpreseParticelleGlobale = leggiImpreseParticelle.Leggi(0,
                                                                      piva,
                                                                      saCod,
                                                                      0,
                                                                      particella.PROV,
                                                                      particella.COM,
                                                                      particella.SEZIONE,
                                                                      particella.FOGLIO,
                                                                      particella.NUMERO,
                                                                      particella.SUBALTERNO,
                                                                      enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                      "",
                                                                      _orderByImpreseParticelleData,
                                                                      _objParametriServer)

        If SeTabellaConRighe(dtImpreseParticelleGlobale, numeroRigheMaggioreDi:=1) Then

            Dim indiceMax = dtImpreseParticelleGlobale.Rows.Count - 1

            For indice As Integer = 0 To indiceMax

                Dim rigaImpreseParticelleGlobaleAtt As DataRow = dtImpreseParticelleGlobale.Rows(indice)

                If indice < indiceMax Then

                    Dim indiceSucc As Integer = indice + 1
                    Dim rigaImpreseParticelleGlobaleSucc As DataRow = dtImpreseParticelleGlobale.Rows(indiceSucc)

                    Dim validitaInizioAttuale = rigaImpreseParticelleGlobaleAtt.Item("Validita_Inizio")
                    Dim validitaFineAttuale = rigaImpreseParticelleGlobaleAtt.Item("Validita_Fine")
                    Dim validitaInizioSuccessiva = rigaImpreseParticelleGlobaleSucc.Item("Validita_Inizio")
                    Dim validitaFineSuccessiva = rigaImpreseParticelleGlobaleSucc.Item("Validita_Fine")

                    If validitaFineAttuale >= validitaInizioSuccessiva Then

                        Dim messaggioEccezione = String.Format("Sovrapposizione temporale particelle nei seguenti periodi: {0}-{1} e {2}-{3}",
                                                               formattaDataInStringa(validitaInizioAttuale),
                                                               formattaDataInStringa(validitaFineAttuale),
                                                               formattaDataInStringa(validitaInizioSuccessiva),
                                                               formattaDataInStringa(validitaFineSuccessiva))

                        GeneraEccezione(messaggioEccezione)

                    End If

                End If

            Next

        End If

    End Sub

    Private Function ParticellaPilotaAggiornata(aggiornamentoParticellaPilota As enum_TipoAgg)

        Return aggiornamentoParticellaPilota = enum_TipoAgg.Modifica OrElse aggiornamentoParticellaPilota = enum_TipoAgg.Cancellazione

    End Function

    Private Sub ControlloGlobaleAppezzamentiCampiScoperti(ByVal impreseParticellePilota As DataRow,
                                                          ByVal forzaCancellazioneLegamiAppezzamentiCampi As enum_forzaCancella)

        Dim objRigheScoperteParticella As New RigheScoperteParticella

        LetturaGlobaleAppezzamentiParticella(impreseParticellePilota, objRigheScoperteParticella)

        LetturaGlobaleCampiParticella(impreseParticellePilota, objRigheScoperteParticella)

        If objRigheScoperteParticella.righeAppezzamenti > 0 OrElse objRigheScoperteParticella.righeCampi > 0 Then

            If forzaCancellazioneLegamiAppezzamentiCampi = enum_forzaCancella.Si Then

                CancellazioneLegamiAppezzamentiCampi(objRigheScoperteParticella)

            Else

                Dim messaggioEccezione = ComponiMessaggioAppezzamentiCampi(impreseParticellePilota, objRigheScoperteParticella, enum_contesto.ControlloGlobale)

                If forzaCancellazioneLegamiAppezzamentiCampi = enum_forzaCancella.No Then

                    GeneraEccezione(messaggioEccezione)

                Else

                    GeneraEccezioneParticellaConLegami(messaggioEccezione)

                End If

            End If

        End If

    End Sub

    Private Sub LetturaGlobaleAppezzamentiParticella(ByVal impreseParticellePilota As DataRow,
                                                     ByRef objRigheScoperteParticella As RigheScoperteParticella)

        objRigheScoperteParticella.dtAppezzamenti = Nothing

        objRigheScoperteParticella.righeAppezzamenti = 0

        '--------------------------------------------------------------------------------
        'Lettura globale appezzamenti per particella
        '--------------------------------------------------------------------------------
        'Controllo se esistono appezzamenti senza imprese per particelle
        '--------------------------------------------------------------------------------

        Dim leggiAppezzamentiParticella As New AppezzaxParticelle_R

        Dim objModificaFinestraTemporale As New ModificaFinestraTemporale

        objModificaFinestraTemporale.Modifica(AGRODATAINIZIO, AGRODATAFINE, _objParametriServer)

        Dim orderByAppezzamentiParticella = " AppezzamentixParticelle.Validita_Inizio, AppezzamentixParticelle.Validita_Fine "

        Try

            objRigheScoperteParticella.dtAppezzamenti = leggiAppezzamentiParticella.AppezzamentixParticelle_Leggi(impreseParticellePilota.Item("piva"),
                                                                                                                  impreseParticellePilota.Item("sa_cod"),
                                                                                                                  0,
                                                                                                                  impreseParticellePilota.Item("PROV"),
                                                                                                                  impreseParticellePilota.Item("COM"),
                                                                                                                  impreseParticellePilota.Item("SEZIONE"),
                                                                                                                  impreseParticellePilota.Item("FOGLIO"),
                                                                                                                  impreseParticellePilota.Item("NUMERO"),
                                                                                                                  impreseParticellePilota.Item("SUBALTERNO"),
                                                                                                                  enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                                                                  "",
                                                                                                                  orderByAppezzamentiParticella,
                                                                                                                  _objParametriServer,
                                                                                                                  False,
                                                                                                                  joinImpreseParticelle:=True,
                                                                                                                  filtroImpreseParticelleAssenti:=True)

            If Not IsNothing(objRigheScoperteParticella.dtAppezzamenti) Then
                objRigheScoperteParticella.righeAppezzamenti = objRigheScoperteParticella.dtAppezzamenti.Rows.Count
            End If

        Catch ex As GiasException

            Throw ex

        Catch ex As Exception

            Throw New Exception(ex.Message)

        Finally

            objModificaFinestraTemporale.Ripristina(_objParametriServer)

        End Try

    End Sub

    Private Sub LetturaGlobaleCampiParticella(ByVal impreseParticellePilota As DataRow,
                                              ByRef objRigheScoperteParticella As RigheScoperteParticella)

        objRigheScoperteParticella.dtCampi = Nothing

        objRigheScoperteParticella.righeCampi = 0

        '--------------------------------------------------------------------------------
        'Lettura globale campi per particella
        '--------------------------------------------------------------------------------
        'Controllo se esistono campi senza imprese per particelle
        '--------------------------------------------------------------------------------

        Dim leggiCampiParticella As New CampixParticelle_R

        Dim objModificaFinestraTemporale As New ModificaFinestraTemporale

        objModificaFinestraTemporale.Modifica(AGRODATAINIZIO, AGRODATAFINE, _objParametriServer)

        Dim orderByCampiParticella = " CP.Validita_Inizio, CP.Validita_Fine "

        Try

            objRigheScoperteParticella.dtCampi = leggiCampiParticella.Leggi(impreseParticellePilota.Item("piva"),
                                                                            impreseParticellePilota.Item("sa_cod"),
                                                                            0,
                                                                            impreseParticellePilota.Item("PROV"),
                                                                            impreseParticellePilota.Item("COM"),
                                                                            impreseParticellePilota.Item("SEZIONE"),
                                                                            impreseParticellePilota.Item("FOGLIO"),
                                                                            impreseParticellePilota.Item("NUMERO"),
                                                                            impreseParticellePilota.Item("SUBALTERNO"),
                                                                            enumSelezioneVariabile.Selezione_JoinCompleta,
                                                                            "",
                                                                            orderByCampiParticella,
                                                                            _objParametriServer,
                                                                            joinImpreseParticelle:=True,
                                                                            filtroImpreseParticelleAssenti:=True)

            If Not IsNothing(objRigheScoperteParticella.dtCampi) Then
                objRigheScoperteParticella.righeCampi = objRigheScoperteParticella.dtCampi.Rows.Count
            End If

        Catch ex As GiasException

            Throw ex

        Catch ex As Exception

            Throw New Exception(ex.Message)

        Finally

            objModificaFinestraTemporale.Ripristina(_objParametriServer)

        End Try

    End Sub

    Private Function ComponiMessaggioAppezzamentiCampi(ByVal impreseParticellePilota As DataRow,
                                                       ByVal objRigheScoperteParticella As RigheScoperteParticella,
                                                       ByVal contesto As enum_contesto
                                                       ) As String

        Dim descrContesto = ""

        If contesto = enum_contesto.CancellazioneUltimaRigaContratto Then
            descrContesto = "cancellazione della riga contratto"
        Else
            descrContesto = "modifica del contratto"
        End If

        Dim particella = ComponiDescrizioneParticella(impreseParticellePilota)

        Dim messaggio = String.Format("La particella <b>{0}</b> presenta dei legami con appezzamenti e/o campi indicati a seguire.<br>",
                                      particella)

        If objRigheScoperteParticella.righeAppezzamenti > 0 Then

            messaggio += String.Format("<br>Appezzamenti che rimarrebbero senza possesso dopo la {0}:", descrContesto)

            For Each rigaAppezzamentoParticella In objRigheScoperteParticella.dtAppezzamenti.Rows

                messaggio += String.Format("<br>- <b>{0}</b> : dal {1} al {2}",
                                           rigaAppezzamentoParticella.item("App_Nome"),
                                           formattaDataInStringa(CDate(rigaAppezzamentoParticella.item("xValidita_Inizio"))),
                                           formattaDataInStringa(CDate(rigaAppezzamentoParticella.item("xValidita_Fine"))))

            Next

        End If

        If objRigheScoperteParticella.righeCampi > 0 Then

            If objRigheScoperteParticella.righeAppezzamenti > 0 Then
                messaggio += "<br>"
            End If

            messaggio += String.Format("<br>Campi che rimarrebbero senza possesso dopo la {0}:", descrContesto)

            For Each rigaCampiParticella In objRigheScoperteParticella.dtCampi.Rows

                messaggio += String.Format("<br>- <b>{0}</b> : dal {1} al {2}",
                                           rigaCampiParticella.item("Campo_Des"),
                                           formattaDataInStringa(CDate(rigaCampiParticella.item("xValidita_Inizio"))),
                                           formattaDataInStringa(CDate(rigaCampiParticella.item("xValidita_Fine"))))

            Next

        End If

        Return messaggio

    End Function

#End Region

#Region "Utility"

    Private Function PeriodoValiditaCorretto(ByVal validitaInizio As Date,
                                             ByVal validitaFine As Date
                                             ) As Boolean

        Return validitaFine >= validitaInizio

    End Function

    Private Sub AzzeraProgressivoElaborazione()

        _progElab = 0

    End Sub

    Private Function IncrementaProgressivoElaborazione() As Integer

        _progElab += 1

        Return _progElab

    End Function

    Private Function DeterminaIndiceDaProgressivoElaborazione(ByRef listaImpreseParticelleDaAggiornare As List(Of ImpreseParticelleDaAggiornare),
                                                              ByVal progElab As Integer
                                                              ) As Integer

        Return listaImpreseParticelleDaAggiornare.FindIndex(Function(x) x.progElab = progElab)

    End Function

    Private Function formattaDataInStringa(ByVal data As Date) As String

        Dim dataOra As DateTime = data

        Return dataOra.ToString(_formatoData)

    End Function

    Private Function SeTabellaConRighe(ByVal tabella As DataTable,
                                       Optional ByVal numeroRigheMaggioreDi As Integer = 0)

        Return Not IsNothing(tabella) AndAlso tabella.Rows.Count > numeroRigheMaggioreDi

    End Function

    Private Function ComponiDescrizioneParticella(ByVal impreseParticellePilota As DataRow)

        Return String.Format("{0} ({1}) Sez.{2} Fgl.{3} Num.{4} S.{5}",
                                       impreseParticellePilota.Item("LOCALITA"),
                                       impreseParticellePilota.Item("COMUNI_PROV"),
                                       impreseParticellePilota.Item("SEZIONE"),
                                       impreseParticellePilota.Item("FOGLIO"),
                                       impreseParticellePilota.Item("NUMERO"),
                                       impreseParticellePilota.Item("SUBALTERNO"))

    End Function

#End Region

#Region "Gestione Errori"

    Private Sub GeneraEccezione(ByVal messaggioEccezione As String)

        ScriviLogAggiornaImpreseParticelleErrore(messaggioEccezione, False)

        Throw New GiasException(messaggioEccezione)

    End Sub

    Private Sub GeneraEccezioneParticellaConLegami(ByVal messaggioEccezione As String)

        ScriviLogAggiornaImpreseParticelleErrore(messaggioEccezione, True)

        Throw New AgroEccezioni_ParticellaConLegami_Exception(messaggioEccezione)

    End Sub

#End Region

#Region "Scrittura Log"

    Private Sub ScriviLogNuovoCodiceParticella(ByVal idAgenda As Integer,
                                               ByVal Particella As ContrattiXImpreseXParticelle_Particella,
                                               ByVal codImprPart As String,
                                               ByVal IdImprPart As Integer)

        ScriviLogAggiornaImpreseParticelle(_separatoreDoppio)

        Dim messaggioLog = String.Format("### Inserimento Nuovo Codice Particella [ IdAgenda={0} Particella={1}|{2}|{3}|{4}|{5}|{6} Codice={7} Id={8} ]",
                                         idAgenda,
                                         Particella.PROV,
                                         Particella.COM,
                                         Particella.SEZIONE,
                                         Particella.FOGLIO,
                                         Particella.NUMERO,
                                         Particella.SUBALTERNO,
                                         codImprPart,
                                         IdImprPart)

        ScriviLogAggiornaImpreseParticelle(messaggioLog)

    End Sub

    Private Sub ScriviLogInizioElaborazione(ByVal tipoOperazione As enum_TipoOperazioneRiga,
                                            ByVal idAgenda As Integer,
                                            ByVal Particella As ContrattiXImpreseXParticelle_Particella,
                                            ByVal objDatiModificati As DatiModificati)


        ScriviLogAggiornaImpreseParticelle(_separatoreDoppio)

        Dim messaggioLog = String.Format("### Inizio elaborazione [ IdAgenda={0} Particella={1}|{2}|{3}|{4}|{5}|{6} ]",
                                         idAgenda,
                                         Particella.PROV,
                                         Particella.COM,
                                         Particella.SEZIONE,
                                         Particella.FOGLIO,
                                         Particella.NUMERO,
                                         Particella.SUBALTERNO)

        ScriviLogAggiornaImpreseParticelle(messaggioLog)

        ScriviLogAggiornaImpreseParticelle(_separatoreDoppio)

        messaggioLog = String.Format("Tipo operazione: {0}", tipoOperazione.ToString(_descrizioneEnum))

        ScriviLogAggiornaImpreseParticelle(messaggioLog)

        Dim dataIniz As DateTime
        Dim dataFine As DateTime

        If IsModificaInserimento(tipoOperazione) Then

            dataIniz = objDatiModificati.validitaInizNew
            dataFine = objDatiModificati.validitaFineNew

            Dim dataInizModificata = _flagInvariato
            Dim dataFineModificata = _flagInvariato
            Dim superficieModificata = _flagInvariato

            If tipoOperazione = enum_TipoOperazioneRiga.Modifica Then
                If objDatiModificati.validitaInizNew <> objDatiModificati.validitaInizOld Then
                    dataInizModificata = _flagModificato
                End If
                If objDatiModificati.validitaFineNew <> objDatiModificati.validitaFineOld Then
                    dataFineModificata = _flagModificato
                End If
                If objDatiModificati.superficieNew <> objDatiModificati.superficieOld Then
                    superficieModificata = _flagModificato
                End If
            End If

            messaggioLog = String.Format("Nuovi dati: Inizio={0}{1} Fine={2}{3} Superficie={4}{5}",
                                         dataIniz.ToString(_formatoData),
                                         dataInizModificata,
                                         dataFine.ToString(_formatoData),
                                         dataFineModificata,
                                         objDatiModificati.superficieNew.ToString(),
                                         superficieModificata)

            ScriviLogAggiornaImpreseParticelle(messaggioLog)

        End If

        If IsModificaCancellazione(tipoOperazione) Then

            dataIniz = objDatiModificati.validitaInizOld
            dataFine = objDatiModificati.validitaFineOld

            messaggioLog = String.Format("Prec. dati: Inizio={0}  Fine={1}  Superficie={2}",
                                         dataIniz.ToString(_formatoData),
                                         dataFine.ToString(_formatoData),
                                         objDatiModificati.superficieOld.ToString())

            ScriviLogAggiornaImpreseParticelle(messaggioLog)

        End If

    End Sub

    Private Sub ScriviLogRigaContratto(ByVal rigaContratto As DataRow,
                                       Optional visualizzaParticella As Boolean = True,
                                       Optional faseElaborazioneContratti As Boolean = False)

        If faseElaborazioneContratti Then
            ScriviLogAggiornaImpreseParticelle("------------")
        End If

        Dim superficie = CDec(rigaContratto.Item("Superficie"))
        Dim dataIniz As DateTime = CDate(rigaContratto.Item("Validita_Inizio"))
        Dim dataFine As DateTime = CDate(rigaContratto.Item("Validita_Fine"))

        Dim datiParticella As String = ""
        If visualizzaParticella Then
            datiParticella = String.Format("Particella={0}|{1}|{2}|{3}|{4}|{5} ",
                                           rigaContratto.Item("PROV"),
                                           rigaContratto.Item("COM"),
                                           rigaContratto.Item("SEZIONE"),
                                           rigaContratto.Item("FOGLIO"),
                                           rigaContratto.Item("NUMERO"),
                                           rigaContratto.Item("SUBALTERNO"))
        End If

        Dim seFaseContratto As String = ""
        If faseElaborazioneContratti Then
            seFaseContratto = "Contratto : "
        End If

        Dim messaggioLog = String.Format("- {0}Centro={1} IdAgenda={2} {3}Inizio={4} Fine={5} Superficie={6}",
                                         seFaseContratto,
                                         rigaContratto.Item("Sa_Cod"),
                                         rigaContratto.Item("Id_Agenda"),
                                         datiParticella,
                                         dataIniz.ToString(_formatoData),
                                         dataFine.ToString(_formatoData),
                                         superficie.ToString())

        ScriviLogAggiornaImpreseParticelle(messaggioLog)

    End Sub

    Private Sub ScriviLogImpreseParticelleDaAggiornare(ByVal tipoOperazione As enum_TipoOperazioneRiga,
                                                       ByVal impreseParticelleDaAggiornare As ImpreseParticelleDaAggiornare,
                                                       Optional ByVal flagModificaValiditaInizio As Boolean = False,
                                                       Optional ByVal flagModificaValiditaFine As Boolean = False,
                                                       Optional ByVal flagModificaSuperficie As Boolean = False,
                                                       Optional ByVal idImprPartEffettivo As Integer = 0
                                                       )

        Dim dataIniz As DateTime = impreseParticelleDaAggiornare.validitaInizio
        Dim dataFine As DateTime = impreseParticelleDaAggiornare.validitaFine
        Dim idImprPart As Integer = 0

        If idImprPartEffettivo <> 0 Then
            idImprPart = idImprPartEffettivo
        Else
            idImprPart = impreseParticelleDaAggiornare.idImprPart
        End If

        Dim messaggioLog = String.Format("  - [{0}] : ProgElab={1} IdImprPart={2} Inizio={3}{4} Fine={5}{6} Superficie={7}{8}",
                                         tipoOperazione.ToString(_descrizioneEnum).Substring(0, 3),
                                         impreseParticelleDaAggiornare.progElab.ToString().PadRight(3, " "),
                                         idImprPart.ToString().PadRight(6, " "),
                                         dataIniz.ToString(_formatoData),
                                         IIf(flagModificaValiditaInizio, _flagModificato, _flagInvariato),
                                         dataFine.ToString(_formatoData),
                                         IIf(flagModificaValiditaFine, _flagModificato, _flagInvariato),
                                         impreseParticelleDaAggiornare.superficie.ToString(),
                                         IIf(flagModificaSuperficie, _flagModificato, _flagInvariato))

        ScriviLogAggiornaImpreseParticelle(messaggioLog)

    End Sub

    Private Sub ScriviLogImpreseParticelleAggiornate(ByVal tipoOperazione As enum_TipoOperazioneRiga,
                                                     ByVal impreseParticelleDaAggiornare As ImpreseParticelleDaAggiornare,
                                                     ByVal idImpreseParticelle As Integer)

        ScriviLogImpreseParticelleDaAggiornare(tipoOperazione,
                                               impreseParticelleDaAggiornare,
                                               idImprPartEffettivo:=idImpreseParticelle)

    End Sub

    Private Sub ScriviLogRigaImpreseParticelle(ByVal rigaImpreseParticelle As DataRow)

        Dim superficieCondotta = CDec(rigaImpreseParticelle.Item("Sup_Condotta"))
        Dim dataIniz As DateTime = CDate(rigaImpreseParticelle.Item("Validita_Inizio"))
        Dim dataFine As DateTime = CDate(rigaImpreseParticelle.Item("Validita_Fine"))

        Dim messaggioLog = String.Format("- Centro={0} Id={1} Particella={2}|{3}|{4}|{5}|{6}|{7} Inizio={8} Fine={9} Sup.Condotta={10}",
                                         rigaImpreseParticelle.Item("Sa_Cod"),
                                         rigaImpreseParticelle.Item("Id"),
                                         rigaImpreseParticelle.Item("PROV"),
                                         rigaImpreseParticelle.Item("COM"),
                                         rigaImpreseParticelle.Item("SEZIONE"),
                                         rigaImpreseParticelle.Item("FOGLIO"),
                                         rigaImpreseParticelle.Item("NUMERO"),
                                         rigaImpreseParticelle.Item("SUBALTERNO"),
                                         dataIniz.ToString(_formatoData),
                                         dataFine.ToString(_formatoData),
                                         superficieCondotta.ToString())

        ScriviLogAggiornaImpreseParticelle(messaggioLog)

    End Sub

    Private Sub ScriviLogAggiornaImpreseParticelleErrore(ByVal messaggioErrore As String,
                                                         ByVal erroreDaConfermare As Boolean)

        If erroreDaConfermare Then

            ScriviLogAggiornaImpreseParticelle(String.Format("!!! ERRORE (RICHIESTA CONFERMA) : {0}", messaggioErrore))

        Else

            ScriviLogAggiornaImpreseParticelle(String.Format("!!! ERRORE : {0}", messaggioErrore))

        End If

    End Sub

    Private Sub ScriviLogAggiornaImpreseParticelle(ByVal messaggio As String)

        If _scriviLogAggiornaImpreseParticelle Then
            Dim customLOGParams As New CustomLOGParams With {
                .LogDescrizioneUtente = _objParametriServer.LogDescrizioneUtente,
                .LogDirectory = _objParametriServer.LogDirectory,
                .LogFileName = _nomeFileLogAggiornaImpreseParticelle
            }
            Scrivi_LOG(_objParametriServer,
                       "AgronicaCoreContabBIZ.ContrattiXImpreseXParticelle.ScriviLogAggiornaImpreseParticelle",
                       messaggio,
                       CustomLOGParams:=customLOGParams)
        End If

    End Sub

#End Region

    Private Class DatiModificati

        Public validitaInizNew As Date? = Nothing
        Public validitaFineNew As Date? = Nothing
        Public superficieNew As Decimal? = Nothing
        Public bioVincoloNew As Integer? = Nothing

        Public validitaInizOld As Date? = Nothing
        Public validitaFineOld As Date? = Nothing
        Public superficieOld As Decimal? = Nothing
        Public bioVincoloOld As Integer? = Nothing

    End Class

    Private Structure ImpreseParticelleDaAggiornare

        Public progElab As Integer

        Public idImprPart As Integer

        Public superficie As Decimal

        Public validitaInizio As Date

        Public validitaFine As Date

    End Structure

    Private Class DatiDaAggiornare

        Public superficie As Decimal

        Public validitaInizio As Date

        Public validitaFine As Date

    End Class

    Private Class ModificaFinestraTemporale

        Private _finestraTemporaleInizioOriginale As Date

        Private _finestraTemporaleFineOriginale As Date

        Public Sub Disabilita(ByRef objParamServer As AgronicaCoreParametri)

            _finestraTemporaleInizioOriginale = objParamServer.FinestraTemporaleInizio

            _finestraTemporaleFineOriginale = objParamServer.FinestraTemporaleFine

            objParamServer.FinestraTemporaleInizio = AGRODATAINIZIO

            objParamServer.FinestraTemporaleFine = AGRODATAFINE

        End Sub


        Public Sub Modifica(ByVal validitaInizio As Date,
                            ByVal validitaFine As Date,
                            ByRef objParamServer As AgronicaCoreParametri)

            _finestraTemporaleInizioOriginale = objParamServer.FinestraTemporaleInizio

            _finestraTemporaleFineOriginale = objParamServer.FinestraTemporaleFine

            objParamServer.FinestraTemporaleInizio = validitaInizio

            objParamServer.FinestraTemporaleFine = validitaFine

        End Sub

        Public Sub Ripristina(ByRef objParamServer As AgronicaCoreParametri)

            objParamServer.FinestraTemporaleInizio = _finestraTemporaleInizioOriginale

            objParamServer.FinestraTemporaleFine = _finestraTemporaleFineOriginale

        End Sub

    End Class

    Private Class RigheScoperteParticella

        Public righeAppezzamenti As Integer = 0

        Public dtAppezzamenti As DataTable = Nothing

        Public righeCampi As Integer = 0

        Public dtCampi As DataTable = Nothing

        Public Function NessunaRigaScoperta()

            Return righeAppezzamenti = 0 AndAlso righeCampi = 0

        End Function

        Public Function EsistonoRigheScoperte()

            Return righeAppezzamenti > 0 OrElse righeCampi > 0

        End Function

    End Class

    Public Enum enum_TipoOperazioneRiga
        Inserimento = 1
        Modifica = 2
        Cancellazione = 3
    End Enum

    Public Enum enum_NomeTabella
        ImpreseXParticelle = 1
        ImpreseXParticelle_Codici = 2
        ImpreseXParticelle_Contatti = 3
    End Enum

    Public Enum enum_TipoAgg
        Nessuno = 0
        Modifica = 1
        Cancellazione = 2
    End Enum

    Public Enum enum_forzaCancella
        Indefinita = 0
        Si = 1
        No = 2
    End Enum

    Public Enum enum_contesto
        CancellazioneUltimaRigaContratto = 1
        ControlloGlobale = 2
    End Enum


End Class
