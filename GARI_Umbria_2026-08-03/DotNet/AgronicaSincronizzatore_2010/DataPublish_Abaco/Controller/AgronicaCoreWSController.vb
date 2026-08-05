Imports System.Net.Http
Imports System.Web
Imports System.Text
Imports Newtonsoft.Json
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreModelsSTD.profilazione
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreDTOStd.InData.Demetra
Imports AgronicaCoreDTOStd.InData.Anagrafica
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDemetraBIZ
Imports AgronicaCoreDTOStd.InData.Notifiche
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreModelsSTD.exceptions

Public Class AgronicaCoreWSController
    Inherits WebApiCaller

    Private Property _logger As LoggerManager
    Private Property _objp As CoreWS_GenericObjP
    Private Property _objp_superserver As AgronicaCoreParametri
    Private Property _objp_server As AgronicaCoreParametri
    Private Property _objp_utenti As AgronicaCoreParametri


    Public Sub New(ByVal baseUrl As String,
                   ByVal token As Tuple(Of String, String),
                   ByVal ObjParametri_SuperServer As AgronicaCoreParametri,
                   ByVal ObjParametri_Server As AgronicaCoreParametri,
                   ByVal ObjParametri_Utenti As AgronicaCoreParametri,
                   ByRef logger As LoggerManager)
        MyBase.New(baseUrl, token, NameOf(AgronicaCoreWSController))
        _objp_superserver = ObjParametri_SuperServer
        _objp_server = ObjParametri_Server
        _objp_utenti = ObjParametri_Utenti
        _objp = New CoreWS_GenericObjP(Sicurezza.Stringa_Codifica_LANCompatibile(JsonConvert.SerializeObject(_objp_superserver), AgroKey_EncoderDecoder),
                                                          Sicurezza.Stringa_Codifica_LANCompatibile(JsonConvert.SerializeObject(_objp_server), AgroKey_EncoderDecoder),
                                                          Sicurezza.Stringa_Codifica_LANCompatibile(JsonConvert.SerializeObject(_objp_utenti), AgroKey_EncoderDecoder))
        _logger = logger

    End Sub

    Public Function VerificaScriviImpresaPadre(ByVal anag As Anagrafica,
                                               Optional ByVal ModalitaOperativa As enum_DataPublish_Configurazione = enum_DataPublish_Configurazione.Demetra) As Boolean
        Dim ret As Boolean = False
        Dim xImpR As New AgronicaCoreAnagrafeBIZ.Impresa_R

        Try
            If ModalitaOperativa = enum_DataPublish_Configurazione.Demetra Then
                ret = xImpR.VerificaEsistenzaImpresaByCUAA("UZ" + anag.mandato.codice_detentore, _objp_server)
            Else
                If anag.mandato.codice_detentore <> _objp_server.PivaSuperUser Then
                    ret = xImpR.VerificaEsistenzaImpresaByCUAA("UZ" + anag.mandato.codice_detentore.Substring(0, 3) + "000000", _objp_server)
                Else
                    ret = True  'se il mandato è uguale a regioneumbria va sempre bene
                End If
            End If
        Catch ex As Exception
            ret = False
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Public Function VerificaImpresa(ByVal anag As Anagrafica) As Boolean
        Dim ret As Boolean = False
        Dim xImpR As New AgronicaCoreAnagrafeBIZ.Impresa_R

        Try
            ret = xImpR.VerificaEsistenzaImpresaByCUAA(anag.cuaa, _objp_server)

        Catch ex As Exception
            ret = False
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Public Function VerificaAppezzamento(ByVal appezzamento As Appezzamento,
                                         ByVal CentroPK As AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK) As Boolean
        Dim ret As Boolean = False
        Dim xAppR As New AgronicaCoreAnagrafeBIZ.Appezzamento_R

        Try
            If xAppR.VerificaEsistenzaAppezzamentoDaCodice(CentroPK.partitaIva, CentroPK.codice, appezzamento.id_appezzamento, _objp_server, False) Is Nothing Then
                ret = False
            Else
                ret = True
            End If

        Catch ex As Exception
            ret = False
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Public Function ScriviModificaImpresa(ByVal cuaa As ObjNotifica(Of AgronicaCoreDTOStd.InData.Notifiche.CUAAObj),
                                          ByVal anag As Anagrafica,
                                          ByRef CentroPK As AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK,
                                          Optional ByVal ModalitaOperativa As enum_DataPublish_Configurazione = enum_DataPublish_Configurazione.Demetra
                                          ) As Boolean
        Dim ret As Boolean = False
        Dim impresa As AgronicaCoreModelsSTD.anagrafiche.Impresa = Nothing
        Dim impresaRet As AgronicaCoreModelsSTD.anagrafiche.Impresa = Nothing
        Try
            Dim xImpW As New AgronicaCoreAnagrafeBIZ.Impresa_W

            impresa = Helper.MapAnagraficaToImpresa(cuaa, anag, _logger, _objp_server, ModalitaOperativa)

            impresaRet = xImpW.Scrivi_Impresa_Anagrafica(impresa, _objp_server, _objp_utenti)

            If impresaRet IsNot Nothing Then
                If impresaRet.centriAziendali Is Nothing OrElse impresaRet.centriAziendali.Count = 0 Then
                    _logger.AppendLog(cuaa, "anagrafica", "CUAA " + anag.cuaa + " - creata\aggiornata azienda senza centro aziendale", LogType.Warning)
                Else
                    CentroPK = impresaRet.centriAziendali(0).primaryKey
                End If
                ret = True
            Else
                Throw New Exception("Impresa non creata. Impossibile proseguire")
            End If

        Catch ex As GiasException
            ret = False
            Throw New GiasException(ex.Message, ex)
        Catch ex As DataPublishException
            ret = False
            Throw New DataPublishException(ex.Message, ex)
        Catch ex As Exception
            ret = False
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Public Function ScriviCatasto(ByVal cuaa As ObjNotifica(Of AgronicaCoreDTOStd.InData.Notifiche.CUAAObj),
                                  ByVal catasto As Terreni,
                                  ByVal CentroPK As AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK) As Boolean
        Dim ret As Boolean = False
        Dim catastoList As List(Of AgronicaCoreDTOStd.InData.Anagrafica.ScriviCatasto) = Nothing
        Try

            Dim xCatastoW As New AgronicaCoreAnagrafeBIZ.Particella_W
            catastoList = Helper.MapTerreniToCatasto(cuaa, CentroPK, catasto, _logger)

            ret = xCatastoW.ParticelleCatastali_Scrivi(catastoList, _objp_server, _objp_utenti, False, NoteLog:=NOTELOG_ANAGRAFE_NG)

        Catch ex As GiasException
            ret = False
            Throw New GiasException(ex.Message, ex)
        Catch ex As DataPublishException
            ret = False
            Throw New DataPublishException(ex.Message, ex)
        Catch ex As Exception
            ret = False
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function


    Public Function ModificaCatasto(ByVal cuaa As ObjNotifica(Of AgronicaCoreDTOStd.InData.Notifiche.CUAAObj),
                                    ByVal catasto_changes As TerreniChangeLog,
                                  ByVal CentroPK As AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK) As Boolean
        Dim ret As Boolean = False
        Dim catasto As List(Of AgronicaCoreDTOStd.InData.Anagrafica.ScriviCatasto) = Nothing
        Try
            Dim xCatastoW As New AgronicaCoreAnagrafeBIZ.Particella_W
            catasto = Helper.MapTerreniToCatasto(cuaa, CentroPK, catasto_changes.records, _logger)

            ret = xCatastoW.ParticelleCatastali_Scrivi(catasto, _objp_server, _objp_utenti, False, NoteLog:=NOTELOG_ANAGRAFE_NG)

        Catch ex As GiasException
            ret = False
            Throw New GiasException(ex.Message, ex)
        Catch ex As DataPublishException
            ret = False
            Throw New DataPublishException(ex.Message, ex)
        Catch ex As Exception
            ret = False
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Public Function ModificaRipartizioneCatastaleAppezzamenti(ByVal id_appezzamento As String,
                                                              ByVal appezzamentixParticelle As List(Of AppezzamentoTerrenoSync),
                                                              ByVal CentroPK As AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK,
                                                              Optional ByRef ObjParametri_Server As AgronicaCoreParametri = Nothing) As Boolean

        Dim ret As Boolean = False
        Try
            If id_appezzamento = "" Then
                ret = False
                Throw New AgronicaCoreWSControllerException("id appezzaento obbligatorio")
            End If
            Dim xAppR As New AgronicaCoreAnagrafeBIZ.Appezzamento_R

            Dim AppKey = AgronicaCoreAnagrafeBIZ.Appezzamento_R.VerificaEsistenzaAppezzamentoDaCodice(CentroPK.partitaIva, CentroPK.codice, id_appezzamento, _objp_server, False)
            If AppKey Is Nothing Then
                Throw New AgronicaCoreWSControllerException(String.Format("Nessun appezzamento trovato per la chiave (id_appezzamento) {0}", id_appezzamento))
            End If
            Dim AppObj = xAppR.Leggi_Appezzamento_Anagrafica(AppKey.centroAziendalePK.partitaIva,
                                                             AppKey.centroAziendalePK.codice,
                                                             AppKey.codice,
                                                             0,
                                                             False,
                                                             False,
                                                             False,
                                                             Date.Now,
                                                             False,
                                                             False,
                                                             False,
                                                             _objp_superserver,
                                                             _objp_server,
                                                             _objp_utenti)
            Dim newAppXParticelle As New List(Of CatastoAppezzamento)
            For Each particella In appezzamentixParticelle
                Select Case particella.tipo_modifica
                    Case "I", "A"
                        newAppXParticelle.Add(New CatastoAppezzamento() With {
                                        .area = 0.1,
                                        .flag_cancellazione = False,
                                        .particella = New ParticelleCatastali.PK(particella.istat_prov, particella.istat_comu, IIf(particella.sezione Is Nothing, "", particella.sezione), particella.foglio, particella.particella, particella.subalterno)
                                      })
                    Case "C"
                        'newAppXParticelle.Add(New CatastoAppezzamento() With {
                        '                .area = 0.1,
                        '                .flag_cancellazione = True,
                        '                .particella = New ParticelleCatastali.PK(particella.istat_prov, particella.istat_comu, IIf(particella.sezione Is Nothing, "", particella.sezione), particella.foglio, particella.particella, particella.subalterno)
                        '              })
                End Select

            Next

            AppObj.catastoAppezzamento = newAppXParticelle

            Dim xAppW As New AgronicaCoreAnagrafeBIZ.Appezzamento_W

            ret = xAppW.Appezzamento_ScriviModifica(AppObj, _objp_server, _objp_utenti)

        Catch ex As AgronicaCoreWSControllerException
            ret = False
            Throw New AgronicaCoreWSControllerException(ex.Message, ex)
        Catch ex As GiasException
            ret = False
            Throw New GiasException(ex.Message, ex)
        Catch ex As DataPublishException
            ret = False
            Throw New DataPublishException(ex.Message, ex)
        Catch ex As Exception
            ret = False
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Public Function ScriviModificaAppezzamenti(ByVal cuaa As ObjNotifica(Of AgronicaCoreDTOStd.InData.Notifiche.CUAAObj),
                                               ByVal appezzamento As Appezzamento,
                                               ByVal appezzamentixParticelle As List(Of AppezzamentoTerreno),
                                               ByVal CentroPK As AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK,
                                               Optional ByRef ObjParametri_SuperServer As AgronicaCoreParametri = Nothing,
                                               Optional ByRef ObjParametri_Server As AgronicaCoreParametri = Nothing,
                                               Optional ByRef ObjParametri_Utenti As AgronicaCoreParametri = Nothing,
                                               Optional ScriviLog As Boolean = True,
                                               Optional VincoliList As List(Of AgronicaCoreModelsSTD.metaschema.Vincolo) = Nothing,
                                               Optional ByVal TagName As String = "",
                                               Optional ByVal ReplaceInvalidPolygon As Boolean = True,
                                               Optional ByVal TipoConfigurazione As enum_DataPublish_Configurazione = enum_DataPublish_Configurazione.Demetra,
                                               Optional ByVal SetAppezzaAddress As Boolean = False,
                                               Optional ByVal EnableVerboseLogPCG As Boolean = False
                                               ) As Boolean
        Dim ret As Boolean = False
        Dim ObjAppezza As AgronicaCoreModelsSTD.anagrafiche.Appezzamento = Nothing
        Try
            Dim xAppW As New AgronicaCoreAnagrafeBIZ.Appezzamento_W
            ObjAppezza = Helper.MapPCGToAppezzamentoCreazione(cuaa,
                                                              CentroPK,
                                                              appezzamento,
                                                              appezzamentixParticelle,
                                                              IIf(ObjParametri_SuperServer Is Nothing, _objp_superserver, ObjParametri_SuperServer),
                                                              IIf(ObjParametri_Server Is Nothing, _objp_server, ObjParametri_Server),
                                                              IIf(ObjParametri_Utenti Is Nothing, _objp_utenti, ObjParametri_Utenti),
                                                              _logger,
                                                              VincoliList,
                                                              TagName,
                                                              ReplaceInvalidPolygon,
                                                              TipoConfigurazione,
                                                              SetAppezzaAddress,
                                                              EnableVerboseLogPCG)

            ret = xAppW.Appezzamento_ScriviModifica(ObjAppezza,
                                                    _objp_server,
                                                    _objp_utenti,
                                                    ScriviLog:=ScriviLog,
                                                    LogVerbose:=EnableVerboseLogPCG)

        Catch ex As GiasException
            ret = False
            Throw New GiasException(ex.Message, ex)
        Catch ex As DataPublishException
            ret = False
            Throw New DataPublishException(ex.Message, ex)
        Catch ex As Exception
            ret = False
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Public Function ModificaAppezzamenti(ByVal cuaa As ObjNotifica(Of AgronicaCoreDTOStd.InData.Notifiche.CUAAObj),
                                         ByVal pcg As AppezzamentoSync,
                                         ByVal pcgterreni As List(Of AppezzamentoTerreno),
                                         ByVal CentroPK As AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK,
                                         Optional ByRef ObjParametri_SuperServer As AgronicaCoreParametri = Nothing,
                                         Optional ByRef ObjParametri_Server As AgronicaCoreParametri = Nothing,
                                         Optional ByRef ObjParametri_Utenti As AgronicaCoreParametri = Nothing,
                                         Optional ScriviLog As Boolean = True,
                                         Optional VincoliList As List(Of AgronicaCoreModelsSTD.metaschema.Vincolo) = Nothing,
                                         Optional ByVal TagName As String = "",
                                         Optional ByVal ReplaceInvalidPolygon As Boolean = True,
                                         Optional ByVal TipoConfigurazione As enum_DataPublish_Configurazione = enum_DataPublish_Configurazione.Demetra,
                                         Optional ByVal SetAppezzaAddress As Boolean = False,
                                         Optional ByVal EnableVerboseLogPCG As Boolean = False,
                                         Optional ByVal CancellazioneLogica As Boolean = False) As Boolean
        Dim ret As Boolean = False
        Try

            Dim xAppW As New AgronicaCoreAnagrafeBIZ.Appezzamento_W
            Dim ObjAppezza = Helper.MapPCGToAppezzamentoModifica(cuaa,
                                                                 CentroPK,
                                                                 pcg,
                                                                 pcgterreni,
                                                                 IIf(ObjParametri_SuperServer Is Nothing, _objp_superserver, ObjParametri_SuperServer),
                                                                 IIf(ObjParametri_Server Is Nothing, _objp_server, ObjParametri_Server),
                                                                 IIf(ObjParametri_Utenti Is Nothing, _objp_utenti, ObjParametri_Utenti),
                                                                 _logger,
                                                                 VincoliList,
                                                                 TagName,
                                                                 ReplaceInvalidPolygon,
                                                                 TipoConfigurazione,
                                                                 SetAppezzaAddress,
                                                                 EnableVerboseLogPCG,
                                                                 CancellazioneLogica)

            If ObjAppezza IsNot Nothing Then
                ret = xAppW.Appezzamento_ScriviModifica(ObjAppezza,
                                                        _objp_server,
                                                        _objp_utenti,
                                                        ScriviLog:=ScriviLog,
                                                        LogVerbose:=EnableVerboseLogPCG)
            Else
                'bypass per non mandare in eccezzione
                ret = True
            End If

        Catch ex As GiasException
            ret = False
            Throw New GiasException(ex.Message, ex)
        Catch ex As DataPublishException
            ret = False
            Throw New DataPublishException(ex.Message, ex)
        Catch ex As Exception
            ret = False
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Public Function CreaUtenteAziendaAgricola(ByVal cuaa As ObjNotifica(Of AgronicaCoreDTOStd.InData.Notifiche.CUAAObj),
                                              ByVal datiColdiretti As UtenteColdiretti,
                                              ByVal username As String,
                                              ByVal piva As String,
                                              ByVal profilo_default As String) As Boolean
        Dim ret As Boolean = False
        Try
            Dim objUtentiBIZ = New AgronicaCoreUtentiBIZ.Utenti_Visibilita
            Dim objGisBIZ As New AgronicaCoreGisBIZ.GIS_LayerElementiGrafici_Permessi_Utente_GruppiUtente_W

            Dim req = Helper.MapUserToAgronicaUtente(cuaa, datiColdiretti, username, piva, profilo_default, _objp_server, _logger)


            objUtentiBIZ.ImpostaVisibilitaAzienda(req.Utente, req.AziendeVisibili,
                                                True, True, _objp_server, _objp_utenti)
            objGisBIZ.ImpostaLayerGisSePermessiCartografia(req.Utente.UserName, _objp_server)

            Dim xGDPR As New AgronicaCoreUtentiBIZ.GDPR

            If xGDPR.CheckGDPRAcceptation(username, _objp_utenti) = False Then
                xGDPR.ForceGDPRAcceptation(username, _objp_utenti)
            End If

            ret = True

        Catch ex As Exception
            ret = False
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Public Function CheckUtenteAziendaAgricola(ByVal cuaa As ObjNotifica(Of AgronicaCoreDTOStd.InData.Notifiche.CUAAObj),
                                               ByVal datiColdiretti As UtenteColdiretti,
                                               ByVal username As String) As Boolean
        Dim ret As Boolean = False
        Try
            Dim req = Helper.MapUserToAgronicaUtenteRequest(cuaa, datiColdiretti, username, "", "", "", _objp_server, _logger)

            Dim rUtenti As New AgronicaCoreUtentiDAL.Utenti_Read

            Dim dt = rUtenti.Leggi(enumSelezioneVariabile.Selezione_TabellaCompleta, " UserName='" + req.UserName + "' ", "", _objp_utenti)
            If dt.Rows.Count <= 0 Then
                ret = False
            Else
                ret = True
            End If


        Catch ex As Exception
            ret = False
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    'Public Function CheckAnagraficaMacchine(ByVal equipaggiamento As AnagraficaWrapper(Of Equipaggiamento),
    '                                        ) As Boolean
    '    Dim ret As Boolean = False
    '    Try

    '    Catch ex As Exception
    '        ret = False
    '        Throw New Exception(ex.Message, ex)
    '    End Try
    '    Return ret
    'End Function

    Public Function GeneraPratica_Gias(ByVal cuaa As ObjNotifica(Of AgronicaCoreDTOStd.InData.Notifiche.CUAAObj),
                                       ByVal piva As String,
                                       ByVal Campagna As Integer,
                                       ByVal id_servizio As Integer,
                                       ByRef Pratica_Cod_Return As Integer) As Boolean
        Dim ret As Boolean = False
        Try

            Dim xPraticheW As New AgronicaCoreProfilazioneBIZ.Pratiche_W

            Dim Pratica_Cod As Integer = 0
            Dim req = Helper.GeneraPraticaRequest(cuaa, piva, Campagna, id_servizio, _logger)

            Dim res = xPraticheW.GeneraPratica(req.piva,
                                                  req.servizio_cod,
                                                  req.Data_Inizio.ToString("yyyy-MM-dd"),
                                                  req.Data_Fine.ToString("yyyy-MM-dd"),
                                                  req.Data_Inizio_Pratica.ToString("yyyy-MM-dd"),
                                                  req.Numero,
                                                  _objp_server,
                                                  _objp_utenti,
                                                  True,
                                                  Pratica_Cod)

            ret = res.RispostaOK
            If ret Then
                Pratica_Cod_Return = Pratica_Cod
            Else
                Throw New Exception(res.Errore)
            End If
        Catch ex As Exception
            ret = False
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Public Function AvanzaPraticaGias(ByVal cuaa As ObjNotifica(Of AgronicaCoreDTOStd.InData.Notifiche.CUAAObj),
                                      ByVal piva As String,
                                      ByVal pratica_cod As Integer,
                                      ByVal id_servizio As Integer,
                                      ByVal newState As Integer) As Boolean
        Dim ret As Boolean = False
        Try

            Dim xPraticheW As New AgronicaCoreProfilazioneBIZ.Pratiche_W
            Dim gefutils As New Gias_EF_Utility
            Dim EFConnString As String = gefutils.GetEntityConnectionString(_objp_server.StringaConnessione)

            Dim req = Helper.AvanzamentoStatoPraticaRequest(cuaa, piva, id_servizio, pratica_cod, newState, _logger)

            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
                ret = xPraticheW.Esegui_PassaggioDiStato_SuperUser_SenzaVerifiche(GiasContext,
                                                                            req.piva,
                                                                            req.pratica_cod,
                                                                            req.servizio_cod,
                                                                            req.statoFinaleRichiesto,
                                                                            req.note,
                                                                            _objp_server)
                If ret Then
                    GiasContext.SaveChanges()
                End If
            End Using

        Catch ex As Exception
            ret = False
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Public Sub RecuperaPraticaGias(ByVal piva As String,
                                        ByVal id_servizio As Integer,
                                        ByVal DataInizio As Date,
                                        ByVal DataFine As Date,
                                        ByRef pratica_cod As Integer)
        pratica_cod = 0
        Dim objPratiche_R As New AgronicaCoreProfilazioneDAL.Pratiche_R
        Try


            Dim dt_Pratiche = objPratiche_R.Leggi(0,
                                                  "",
                                                  piva,
                                                  "",
                                                  0,
                                                  0,
                                                  0,
                                                  id_servizio,
                                                  DataInizio,
                                                  DataFine,
                                                  "",
                                                  "",
                                                  _objp_server,
                                                  0,
                                                  0)
            If dt_Pratiche.Rows.Count <= 0 Then
                'Throw New Exception(String.Format("Nessuna pratica provata per la piva {0} / servizio {1} per il periodo indicato {2}/{3}", piva, id_servizio, DataInizio, DataFine))
                pratica_cod = 0 'lavez - 05/01/2024 - non faccio più scattare eccezzione in modo da porter creare la pratica nuova al cambio di campagna
            Else
                pratica_cod = dt_Pratiche.Rows(0)("Pratica_Cod")
            End If
        Catch ex As Exception
            pratica_cod = 0
            Throw New Exception(ex.Message, ex)
        End Try
    End Sub

    Public Function VerificaPraticaGiasXAvanzamentoDiStato(ByVal pratica_cod As Integer) As Boolean
        Dim ret As Boolean = False
        Dim objPraticheStati_R As New AgronicaCoreProfilazioneDAL.Pratiche_Stati_Attuali_R
        Try
            Dim dt_Pratiche = objPraticheStati_R.Leggi(pratica_cod,
                                                       "",
                                                       "",
                                                       _objp_server)
            If dt_Pratiche.Rows.Count <= 0 Then
                Throw New Exception(String.Format("Nessuna pratica provata ({0})", pratica_cod))
            Else
                If dt_Pratiche.Rows(0)("Stato_Cod") <= TipiEnumerativi.enum_WWorflow_WAnagraficaStati.QdC_Bluarancio_Demetra_Azienda_Attivata Then
                    ret = True
                End If
            End If
        Catch ex As Exception
            ret = False
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Public Function ScriviModifica_Macchina(ByVal cuaa As ObjNotifica(Of AgronicaCoreDTOStd.InData.Notifiche.CUAAObj),
                                            ByVal CentroPK As AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK,
                                            ByVal Equip As AnagraficaWrapper(Of Equipaggiamento),
                                            ByRef EsitoSync As SyncAcknowledge) As Boolean
        Dim ret As Boolean = False
        Try
            Dim xMaccR As New AgronicaCoreContabBIZ.Parco_Macchine_R
            Dim xMaccW As New AgronicaCoreDemetraBIZ.ImportParcoMacchine
            Dim xUserDemetraR As New AgronicaCoreDemetraBIZ.decodificaUtenti

            Dim errors As New Concurrent.ConcurrentDictionary(Of Tuple(Of String, Integer), String)
            Dim successes As New Concurrent.ConcurrentBag(Of Tuple(Of String, ParcoMacchine))

            Dim bkp_username_operazione As String = ""

            If Equip.elemento_anagrafico.utente_ultima_modifica <> "" Then
                bkp_username_operazione = _objp_server.UsernameOperazione
                _objp_server.UsernameOperazione = xUserDemetraR.decoficaUtenteGiasDaUtenteDemetra(Equip.elemento_anagrafico.utente_ultima_modifica, cuaa.payload.CUAA, _objp_utenti)
                _objp_utenti.UsernameOperazione = _objp_server.UsernameOperazione
            End If

            Dim Mac_Cod = 0
            Dim ContattoPK As AgronicaCoreModelsSTD.anagrafiche.Contatto.PK = Nothing
            Dim chkMacc = xMaccW.ReadParcoMacchineFromCodiceEsterno(Equip.codice, CentroPK.partitaIva, Nothing, _objp_server, _objp_utenti)
            If chkMacc IsNot Nothing Then
                Mac_Cod = chkMacc.codice
                If chkMacc.contatto IsNot Nothing Then
                    ContattoPK = chkMacc.contatto.primaryKey
                End If
            End If

            Dim ObjMacchina = Helper.MapEquipaggiamentoToMacchina(cuaa, CentroPK, Equip, Mac_Cod, ContattoPK, _objp_server, _logger)
            If ObjMacchina IsNot Nothing Then
                If chkMacc Is Nothing Then
                    xMaccW.CreateMacchineD2G(cuaa.payload.CUAA, ObjMacchina, errors, successes, _objp_server, _objp_utenti)
                Else
                    xMaccW.EditMacchineD2G(cuaa.payload.CUAA, ObjMacchina, errors, successes, _objp_server, _objp_utenti)

                End If
                If successes.Count > 0 Then
                    ret = True
                Else
                    ret = False
                    SetErrorMacchinaEsitoSync("I", EsitoSync, errors)
                End If
            Else
                'bypass per non mandare in eccezzione
                ret = True
            End If

            If bkp_username_operazione <> "" Then
                _objp_server.UsernameOperazione = bkp_username_operazione
                _objp_utenti.UsernameOperazione = bkp_username_operazione
            End If

        Catch ex As GiasException
            ret = False
            Throw New GiasException(ex.Message, ex)
        Catch ex As DataPublishException
            ret = False
            Throw New DataPublishException(ex.Message, ex)
        Catch ex As Exception
            ret = False
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Public Function CreaModificaCancellaMacchina(ByVal cuaa As ObjNotifica(Of AgronicaCoreDTOStd.InData.Notifiche.CUAAObj),
                                                 ByVal CentroPK As AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK,
                                                 ByVal Equip As EquipaggiamentoCUAASync,
                                                 ByRef EsitoSync As SyncAcknowledge) As Boolean
        Dim ret As Boolean = False
        Try
            Dim xMaccR As New AgronicaCoreContabBIZ.Parco_Macchine_R
            Dim xMaccW As New AgronicaCoreDemetraBIZ.ImportParcoMacchine
            Dim xUserDemetraR As New AgronicaCoreDemetraBIZ.decodificaUtenti

            Dim bkp_username_operazione As String = ""

            If Equip.elemento_anagrafico.utente_ultima_modifica <> "" Then
                bkp_username_operazione = _objp_server.UsernameOperazione
                _objp_server.UsernameOperazione = xUserDemetraR.decoficaUtenteGiasDaUtenteDemetra(Equip.elemento_anagrafico.utente_ultima_modifica, cuaa.payload.CUAA, _objp_utenti)
                _objp_utenti.UsernameOperazione = _objp_server.UsernameOperazione
            End If
            Dim chkMacc As ParcoMacchine = Nothing
            'Lavez - 14/02/2029 - controllo commentato per gestire correttamente il giro macchina gias -> export to demetra -> modifica su demetra -> notifica a gias-> aggiornamento
            If Equip.codice_esterno <> "" Then
                'chiave gias
                Dim chiaveMacchina = Equip.codice_esterno.ToString().Split("_"c)
                chkMacc = xMaccW.ReadParcoMacchineFromMacCod(chiaveMacchina(2), CentroPK.partitaIva, Nothing, _objp_server, _objp_utenti)
            ElseIf Equip.codice <> "" Then
                chkMacc = xMaccW.ReadParcoMacchineFromCodiceEsterno(Equip.codice, CentroPK.partitaIva, Nothing, _objp_server, _objp_utenti)
            End If

            Dim ObjMacchina = Helper.MapEquipaggiamentoToMacchina(cuaa, CentroPK, Equip, chkMacc, _objp_server, _logger)
            If ObjMacchina IsNot Nothing Then
                Dim errors As New Concurrent.ConcurrentDictionary(Of Tuple(Of String, Integer), String)
                Dim successes As New Concurrent.ConcurrentBag(Of Tuple(Of String, ParcoMacchine))

                Select Case Equip.tipo_modifica
                    Case "I"
                        If chkMacc Is Nothing Then
                            xMaccW.CreateMacchineD2G(cuaa.payload.CUAA, ObjMacchina, errors, successes, _objp_server, _objp_utenti)
                        Else
                            xMaccW.EditMacchineD2G(cuaa.payload.CUAA, ObjMacchina, errors, successes, _objp_server, _objp_utenti)
                        End If
                        If successes.Count > 0 Then
                            ret = True
                        Else
                            ret = False
                            SetErrorMacchinaEsitoSync(Equip.tipo_modifica, EsitoSync, errors)
                        End If

                    Case "A"
                        'Lavez - 14/02/2024 - controllo commentato per gestire correttamente il giro macchina gias -> export to demetra -> modifica su demetra -> notifica a gias-> aggiornamento
                        'Lavez - 16/02/2024 - gestione creazione\aggiornamento macchina anche nel tipo_modifica A
                        If chkMacc Is Nothing Then
                            xMaccW.CreateMacchineD2G(cuaa.payload.CUAA, ObjMacchina, errors, successes, _objp_server, _objp_utenti)
                        Else
                            xMaccW.EditMacchineD2G(cuaa.payload.CUAA, ObjMacchina, errors, successes, _objp_server, _objp_utenti)
                        End If
                        If successes.Count > 0 Then
                            ret = True
                        Else
                            ret = False
                            SetErrorMacchinaEsitoSync(Equip.tipo_modifica, EsitoSync, errors)
                        End If

                    Case "C"
                        If chkMacc Is Nothing Then
                            _logger.AppendLog(cuaa, "equipaggiamenti", String.Format("CUAA {0} - Macchina con codice({0}) non presente su GIAS. Record ignorato" + Equip.cuaa, Equip.codice), LogType.Warning, bypassElastiSearch:=True)
                            ret = True
                        Else
                            xMaccW.DeleteMacchineD2G(cuaa.payload.CUAA, ObjMacchina, errors, successes, _objp_server, _objp_utenti)
                            If successes.Count > 0 Then
                                ret = True
                            Else
                                ret = False
                                SetErrorMacchinaEsitoSync(Equip.tipo_modifica, EsitoSync, errors)
                            End If
                        End If

                End Select
            Else
                'bypass per non mandare in eccezzione
                ret = True
            End If

            If bkp_username_operazione <> "" Then
                _objp_server.UsernameOperazione = bkp_username_operazione
                _objp_utenti.UsernameOperazione = bkp_username_operazione
            End If

        Catch ex As GiasException
            ret = False
            Throw New GiasException(ex.Message, ex)
        Catch ex As DataPublishException
            ret = False
            Throw New DataPublishException(ex.Message, ex)
        Catch ex As Exception
            ret = False
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Private Sub SetErrorMacchinaEsitoSync(ByVal TipoModifica As String,
                                          ByRef EsitoSync As SyncAcknowledge,
                                          ByRef errors As Concurrent.ConcurrentDictionary(Of Tuple(Of String, Integer), String))
        For Each er In errors
            Select Case TipoModifica
                Case "I"
                    EsitoSync.errors.Add(New SyncErrors() With {.code = "609", .msg = "Errore in inserimento macchina " + er.Value, .key = er.Key.Item1})
                Case "A"
                    EsitoSync.errors.Add(New SyncErrors() With {.code = "610", .msg = "Errore in modifica macchina " + er.Value, .key = er.Key.Item1})
                Case "C"
                    EsitoSync.errors.Add(New SyncErrors() With {.code = "611", .msg = "Errore in cancellazione macchina " + er.Value, .key = er.Key.Item1})
            End Select
        Next
    End Sub

    Public Function ScriviModificaCancella_Contatto(ByVal cuaa As String,
                                                    ByVal Lavoratore As AnagraficaWrapper(Of AgronicaCoreDTOStd.InData.Demetra.Contatto),
                                                    ByRef messaggioErrore As String,
                                                    Optional ByVal ParametriContatti As ParametriInterscambioContatti = Nothing) As Boolean

        Dim ret As Boolean = False
        Try
            Dim importer As New ImportContatti(cuaa, ParametriContatti, _objp_server, _objp_utenti)
            messaggioErrore = importer.Importa(cuaa, Lavoratore)
            If messaggioErrore = "" Then
                ret = True
            Else
                ret = False
                'Throw New AgronicaCoreWSControllerException(messaggioErrore)
            End If
        Catch ex As AgronicaCoreWSControllerException
            ret = False
            Throw New AgronicaCoreWSControllerException(ex.Message, ex)
        Catch ex As GiasException
            ret = False
            Throw New GiasException(ex.Message, ex)
        Catch ex As DataPublishException
            ret = False
            Throw New DataPublishException(ex.Message, ex)
        Catch ex As Exception
            ret = False
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

End Class
