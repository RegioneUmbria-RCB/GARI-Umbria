Imports System.Collections.Concurrent
Imports System.Text
Imports AgronicaConversioneCartografiaGias.Agronica
Imports AgronicaConversioneCartografiaGias.FormatsConverter
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDTOStd.InData.Anagrafica
Imports AgronicaCoreDTOStd.InData.Demetra
Imports AgronicaCoreDTOStd.InData.Notifiche
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.costanti
Imports AgronicaCoreModelsSTD.exceptions
Imports NetTopologySuite.CoordinateSystems.Transformations
Imports NetTopologySuite.Geometries
Imports NetTopologySuite.Mathematics
Imports Newtonsoft.Json
Imports ProjNet.Converters.WellKnownText
Imports ProjNet.CoordinateSystems
Imports ProjNet.CoordinateSystems.Transformations


Public Class Helper
    Implements IDisposable

    Private _idExtSys As Integer
    Private _ObjParametri As AgronicaCoreParametri
    Private _logger As LoggerManager

    Private Const UTWGS_84_UTM_zone_32N As String = "PROJCS[""WGS_1984_UTM_Zone_32N"",GEOGCS[""GCS_WGS_1984"",DATUM[""D_WGS_1984"",SPHEROID[""WGS_1984"",6378137.0,298.257223563]],PRIMEM[""Greenwich"",0.0],UNIT[""Degree"",0.0174532925199433]],PROJECTION[""Transverse_Mercator""],PARAMETER[""False_Easting"",500000.0],PARAMETER[""False_Northing"",0.0],PARAMETER[""Central_Meridian"",9.0],PARAMETER[""Scale_Factor"",0.9996],PARAMETER[""Latitude_Of_Origin"",0.0],UNIT[""Meter"",1.0]]"
    Private Const UTWGS_84_UTM_zone_33N As String = "PROJCS[""WGS_1984_UTM_Zone_33N"",GEOGCS[""GCS_WGS_1984"",DATUM[""D_WGS_1984"",SPHEROID[""WGS_1984"",6378137.0,298.257223563]],PRIMEM[""Greenwich"",0.0],UNIT[""Degree"",0.0174532925199433]],PROJECTION[""Transverse_Mercator""],PARAMETER[""False_Easting"",500000.0],PARAMETER[""False_Northing"",0.0],PARAMETER[""Central_Meridian"",15.0],PARAMETER[""Scale_Factor"",0.9996],PARAMETER[""Latitude_Of_Origin"",0.0],UNIT[""Meter"",1.0]]"
    Private Const WGS_84 As String = "GEOGCS[""GCS_WGS_1984"",DATUM[""D_WGS_1984"",SPHEROID[""WGS_1984"",6378137.0,298.257223563]],PRIMEM[""Greenwich"",0.0],UNIT[""Degree"",0.0174532925199433]]"
    Private Const MonteMario_Italy_Zone_1_3003 As String = "PROJCS[""Monte Mario / Italy zone 1"",GEOGCS[""Monte Mario"",DATUM[""Monte_Mario"",SPHEROID[""International 1924"", 6378388, 297],TOWGS84[-104.1, -49.1, -9.9, 0.971, -2.917, 0.714, -11.68]],PRIMEM[""Greenwich"", 0,AUTHORITY[""EPSG"", ""8901""]],UNIT[""degree"", 0.0174532925199433,AUTHORITY[""EPSG"", ""9122""]],AUTHORITY[""EPSG"", ""4265""]],PROJECTION[""Transverse_Mercator""],PARAMETER[""latitude_of_origin"", 0],PARAMETER[""central_meridian"", 9],PARAMETER[""scale_factor"", 0.9996],PARAMETER[""false_easting"", 1500000],PARAMETER[""false_northing"", 0],UNIT[""metre"", 1,AUTHORITY[""EPSG"", ""9001""]],AXIS[""Easting"", EAST],AXIS[""Northing"", NORTH],AUTHORITY[""EPSG"", ""3003""]]"
    Private Const MonteMario_Italy_Zone_2_3004 As String = "PROJCS[""Monte Mario / Italy zone 2"",GEOGCS[""Monte Mario"",DATUM[""Monte_Mario"",SPHEROID[""International 1924"", 6378388, 297],TOWGS84[-104.1, -49.1, -9.9, 0.971, -2.917, 0.714, -11.68]],PRIMEM[""Greenwich"", 0,AUTHORITY[""EPSG"", ""8901""]],UNIT[""degree"", 0.0174532925199433,AUTHORITY[""EPSG"", ""9122""]],AUTHORITY[""EPSG"", ""4265""]],PROJECTION[""Transverse_Mercator""],PARAMETER[""latitude_of_origin"", 0],PARAMETER[""central_meridian"", 15],PARAMETER[""scale_factor"", 0.9996],PARAMETER[""false_easting"", 2520000],PARAMETER[""false_northing"", 0],UNIT[""metre"", 1,AUTHORITY[""EPSG"", ""9001""]],AXIS[""Easting"", EAST],AXIS[""Northing"", NORTH],AUTHORITY[""EPSG"", ""3004""]]"

    Private Const CODICE_Codice_AGEA_DEMETRA As Integer = 2315
    Private Const CODICE_Codice_Agea_EffluentiZootecnici As Integer = 1356
    Private Const CODICE_Codice_Agea_TipoDiSemina As Integer = 1357
    Private Const CODICE_Codice_flag_irriguo As Integer = 1188 'CATIMP0F_CIFIR ?

    Private Const CODICE_Appezzamento_AGEA As Integer = 1360
    Private Const CODICE_Impianto_AGEA As Integer = 1361
    Private Const CODICE_Campagna As Integer = 1362

    Public Sub New(ByVal extSysReg As String,
                   ByVal ObjParametri_Server As AgronicaCoreParametri,
                   ByRef logger As LoggerManager)
        _ObjParametri = ObjParametri_Server
        _logger = logger
        _idExtSys = GetExtSysID(extSysReg, _ObjParametri)
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        _ObjParametri = Nothing
        _logger = Nothing
    End Sub

    Public Function GetCUAAList(ByVal TipoOperazione As Integer,
                                Optional ByVal NumberOfRecords As Integer = 0,
                                Optional ByVal TagName As String = "",
                                Optional ByVal TipoAnagrafica As TipoAnagrafiche = Nothing) As List(Of ObjNotifica(Of AgronicaCoreDTOStd.InData.Notifiche.CUAAObj))

        Dim ret As New List(Of ObjNotifica(Of AgronicaCoreDTOStd.InData.Notifiche.CUAAObj))
        Try
            Dim reader As New AgronicaCoreNotifichePushBIZ.SistemiEsterni_RicezioneNotifiche_R

            Dim temp = reader.LeggiElencoNotificheDaElaborare(Of AgronicaCoreDTOStd.InData.Notifiche.CUAAObj)(_idExtSys,
                                                                                                              AGRODATAINIZIO,
                                                                                                              _ObjParametri,
                                                                                                              TipoOperazione,
                                                                                                              NumberOfRecords,
                                                                                                              TagName,
                                                                                                              If(TipoAnagrafica IsNot Nothing AndAlso TipoAnagrafica.Anagrafica = True, 1, 0),
                                                                                                              If(TipoAnagrafica IsNot Nothing AndAlso TipoAnagrafica.Terreni = True, 1, 0),
                                                                                                              If(TipoAnagrafica IsNot Nothing AndAlso TipoAnagrafica.PCG = True, 1, 0),
                                                                                                              If(TipoAnagrafica IsNot Nothing AndAlso TipoAnagrafica.PCG_Terreni = True, 1, 0),
                                                                                                              If(TipoAnagrafica IsNot Nothing AndAlso TipoAnagrafica.Equipaggiamenti = True, 1, 0),
                                                                                                              If(TipoAnagrafica IsNot Nothing AndAlso TipoAnagrafica.Lavoratori = True, 1, 0),
                                                                                                              If(TipoAnagrafica IsNot Nothing AndAlso TipoAnagrafica.Gruppi_Appezzamenti = True, 1, 0)
                                                                                                              )
            Select Case TipoOperazione
                Case 1
                    Dim lst = temp.Where(Function(x) x.payload.Operazione = "C").ToList()
                    If NumberOfRecords > 0 Then
                        If lst.Count >= NumberOfRecords Then
                            ret = lst.GetRange(0, NumberOfRecords)
                        Else
                            ret = lst
                        End If
                    Else
                        ret = lst
                    End If
                Case 2
                    Dim lst = temp.Where(Function(x) x.payload.Operazione = "U").ToList()
                    If NumberOfRecords > 0 Then
                        If lst.Count >= NumberOfRecords Then
                            ret = lst.GetRange(0, NumberOfRecords)
                        Else
                            ret = lst
                        End If
                    Else
                        ret = lst
                    End If
                Case Else
                    ret = Nothing
            End Select


        Catch ex As Exception
            _logger.AppendLog(Nothing, "", ex.Message, LogType.Errore, ex, bypassElastiSearch:=True)
        End Try
        Return ret
    End Function

    Public Function CheckIfExistCreationToElabForCUAA(ByVal cuaa As String, ByVal campagna As Integer) As Boolean
        Dim ret As Boolean = False
        Try
            If cuaa = "" Then
                Throw New Exception("Cuaa parametro obbligatorio")
            End If
            Dim reader As New AgronicaCoreNotifichePushBIZ.SistemiEsterni_RicezioneNotifiche_R

            Dim temp = reader.LeggiElencoNotificheDaElaborare(Of AgronicaCoreDTOStd.InData.Notifiche.CUAAObj)(_idExtSys, AGRODATAINIZIO, _ObjParametri, 1)
            Dim tempCreation = temp.Where(Function(x) x.payload.Operazione = "C" AndAlso x.payload.CUAA = cuaa AndAlso x.payload.Campagna = campagna).ToList()
            If tempCreation Is Nothing OrElse tempCreation.Count <= 0 Then
                ret = False
            Else
                ret = True
            End If
        Catch ex As Exception
            ret = Nothing
            _logger.AppendLog(Nothing, "", ex.Message, LogType.Errore, ex, bypassElastiSearch:=True)
        End Try
        Return ret
    End Function

    Private Function GetExtSysID(ByVal ExtRif As String,
                                 ByRef ObjParametri As AgronicaCoreParametri) As Integer
        Dim ret As Integer = -1
        Try
            Dim xread As New AgronicaCoreMetaSchemaDAL.Sistemi_Esterni_R
            Dim d = xread.Leggi(0, "", AGRODATAINIZIO, AGRODATAFINE, "Sistema_Des='" & ExtRif & "'", "", ObjParametri)
            If d.Rows.Count > 0 Then
                ret = d.Rows(0)("Sistema_Cod")
            End If
        Catch ex As Exception
            _logger.AppendLog(Nothing, "", ex.Message, LogType.Errore, ex, True)
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Public Sub SetStatoNotifica(ByVal notifiche As ObjNotifica(Of CUAAObj),
                                ByRef ObjParametri As AgronicaCoreParametri)
        Try
            Dim xwrite As New AgronicaCoreNotifichePushBIZ.SistemiEsterni_RicezioneNotifiche_W
            Dim lst = New List(Of ObjNotifica(Of CUAAObj))
            lst.Add(notifiche)
            Dim d = xwrite.AggiornaNotificheCUAA(lst, ObjParametri)
        Catch ex As Exception
            _logger.AppendLog(Nothing, "", ex.Message, LogType.Errore, ex, True)
            Throw New Exception(ex.Message, ex)
        End Try

    End Sub

    Public Sub SetEsitoNotifica(ByVal notifica As ObjNotifica(Of CUAAObj),
                                ByVal EsitoText As String)
        Try
            Dim xwrite As New AgronicaCoreNotifichePushBIZ.SistemiEsterni_RicezioneNotifiche_W
            xwrite.AggiornaEsitoNotificaCUAA(notifica, EsitoText, _ObjParametri)
        Catch ex As Exception
            _logger.AppendLog(Nothing, "", ex.Message, LogType.Errore, ex, True)
            Throw New Exception(ex.Message, ex)
        End Try
    End Sub

    Public Sub SetStatisticheElaborazioneStepNotifica(ByVal notifica As ObjNotifica(Of CUAAObj),
                                                      ByVal Inizio_Anagrafica As DateTime,
                                                      ByVal Fine_Anagrafica As DateTime,
                                                      ByVal Inizio_Catasto As DateTime,
                                                      ByVal Fine_Catasto As DateTime,
                                                      ByVal Inizio_PCG As DateTime,
                                                      ByVal Fine_PCG As DateTime,
                                                      ByVal N_App As Integer,
                                                      ByVal Inizio_Lavoratori As DateTime,
                                                      ByVal Fine_Lavoratori As DateTime,
                                                      ByVal Inizio_Equipaggiamenti As DateTime,
                                                      ByVal Fine_Equipaggiamenti As DateTime,
                                                      ByVal Inizio_Gruppi_Appezzamenti As DateTime,
                                                      ByVal Fine_Gruppi_Appezzamenti As DateTime,
                                                      Optional ByVal LogError As Boolean = True)
        Try
            Dim xwrite As New AgronicaCoreNotifichePushBIZ.SistemiEsterni_RicezioneNotifiche_W
            xwrite.AggiornaStatisticheNotificaNotificaCUAA(notifica,
                                                           Inizio_Anagrafica,
                                                            Fine_Anagrafica,
                                                            Inizio_Catasto,
                                                            Fine_Catasto,
                                                            Inizio_PCG,
                                                            Fine_PCG,
                                                            N_App,
                                                            Inizio_Lavoratori,
                                                            Fine_Lavoratori,
                                                            Inizio_Equipaggiamenti,
                                                            Fine_Equipaggiamenti,
                                                            Inizio_Gruppi_Appezzamenti,
                                                            Fine_Gruppi_Appezzamenti,
                                                            _ObjParametri,
                                                            LogError
                                                            )
        Catch ex As Exception
            _logger.AppendLog(Nothing, "", ex.Message, LogType.Errore, ex, True)
            Throw New Exception(ex.Message, ex)
        End Try
    End Sub

    Public Shared Function MapAnagraficaToImpresaPadre(ByVal anag As Anagrafica,
                                                       ByRef _logger As LoggerManager,
                                                       ByRef ObjParametriServer As AgronicaCoreParametri) As AgronicaCoreModelsSTD.anagrafiche.Impresa
        Dim ret As AgronicaCoreModelsSTD.anagrafiche.Impresa = Nothing
        Dim xImpR As New AgronicaCoreAnagrafeBIZ.Impresa_R
        Dim xComuniR As New AgronicaCoreMetaSchemaDAL.Istat_R




        Try
            ret = New AgronicaCoreModelsSTD.anagrafiche.Impresa

            ret.partitaIva = "UZ" & anag.mandato.codice_detentore
            ret.ragioneSociale = anag.mandato.codice_detentore
            ret.CUAA = anag.mandato.codice_detentore
            ret.forma_Giuridica = New AgronicaCoreModelsSTD.metaschema.FormeGiuridiche(2)


            Dim dtRes = xComuniR.Leggi("", "", "", "", "", AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", ObjParametriServer, anag.sede.codice_belfiore)
            If dtRes.Rows.Count <= 0 Then
                Throw New Exception("Nessun comune codificato per il codice belfiore ( " & anag.sede.codice_belfiore & " ).")
            End If

            ret.impresaPadre = New List(Of AgronicaCoreModelsSTD.anagrafiche.ImpresaPadre)
            ret.impresaPadre.Add(New AgronicaCoreModelsSTD.anagrafiche.ImpresaPadre() With {
                                                    .partitaIva = ObjParametriServer.PivaSuperUser
                                                })

            ret.indirizzi = New List(Of AgronicaCoreModelsSTD.anagrafiche.IndirizzoAssociato)
            ret.indirizzi.Add(New AgronicaCoreModelsSTD.anagrafiche.IndirizzoAssociato() With {
                                .tipo_Indirizzo = 1,
                                .indirizzo = New AgronicaCoreModelsSTD.anagrafiche.Indirizzo() With {
                                    .istatComune = New AgronicaCoreModelsSTD.metaschema.Istat() With {
                                                    .prov = "000",
                                                    .com = "000"
                                                },
                                        .cap = "00000",
                                        .stato = New AgronicaCoreModelsSTD.metaschema.CodiciNazioniISO3166() With {
                                                    .codice = "IT"
                                                },
                                        .codice = 0,
                                        .frazione = "",
                                        .note = "",
                                        .via = "."
                                }
                                })


        Catch ex As Exception
            ret = Nothing
            _logger.AppendLog(Nothing, "", ex.Message, LogType.Errore, ex, True)
        End Try
        Return ret

    End Function

    Public Shared Function LeggiCentroPKDaCUAA(ByVal cuaa As ObjNotifica(Of CUAAObj),
                                                  ByRef _logger As LoggerManager,
                                                  ByRef ObjParametriServer As AgronicaCoreParametri) As AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK

        Dim ret As AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK = Nothing
        Dim xImpR As New AgronicaCoreAnagrafeBIZ.Impresa_R
        Dim xCA_R As New AgronicaCoreAnagrafeBIZ.CentroAziendale_R
        Try
            If xImpR.VerificaEsistenzaImpresaByCUAA(cuaa.payload.CUAA, ObjParametriServer) = False Then
                Throw New DataPublishException("Nessuna azienda trovata per il CUAA " & cuaa.payload.CUAA)
            End If

            Dim lst = xCA_R.LeggiCentriAziendaliPKPerCUAA(cuaa.payload.CUAA, ObjParametriServer)
            If lst.Count > 0 Then
                ret = lst(0)    '' prendo solo il primo
            End If

        Catch ex As DataPublishException
            ret = Nothing
            _logger.AppendLog(cuaa, "anagrafica", ex.Message, LogType.Errore, ex)
            Throw New DataPublishException(ex.Message, ex)
        Catch ex As Exception
            ret = Nothing
            _logger.AppendLog(cuaa, "anagrafica", ex.Message, LogType.Errore, ex)
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Public Shared Function MapAnagraficaToImpresa(ByVal cuaa As ObjNotifica(Of CUAAObj),
                                                  ByVal anag As Anagrafica,
                                                  ByRef _logger As LoggerManager,
                                                  ByRef ObjParametriServer As AgronicaCoreParametri,
                                                  Optional ByVal ModalitaOperativa As enum_DataPublish_Configurazione = enum_DataPublish_Configurazione.Demetra
                                                  ) As AgronicaCoreModelsSTD.anagrafiche.Impresa

        Dim ret As AgronicaCoreModelsSTD.anagrafiche.Impresa = Nothing
        Dim xImpR As New AgronicaCoreAnagrafeBIZ.Impresa_R
        Dim xImpCodR As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
        Dim xCentriR As New AgronicaCoreAnagrafeBIZ.CentroAziendale_R
        Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
        Dim xComuniR As New AgronicaCoreMetaSchemaDAL.Istat_R



        Try
            ret = New AgronicaCoreModelsSTD.anagrafiche.Impresa

            '0- controllo se esiste già una piva per il cuaa

            If xImpR.VerificaEsistenzaImpresaByCUAA(anag.cuaa, ObjParametriServer) = False Then
                'creazione

                'Lavez - 15/09/2025 - fix per gestione piva =cuaa
                ret.partitaIva = IIf(anag.partita_iva = "" AndAlso anag.codice_fiscale = "", anag.cuaa, IIf(anag.partita_iva = "", anag.codice_fiscale, anag.partita_iva))
                'ret.partitaIva = IIf(anag.partita_iva = "" , anag.codice_fiscale, anag.partita_iva)
                ret.ragioneSociale = anag.denominazione
                ret.CUAA = anag.cuaa

                ret.codici = New List(Of AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori)
                If anag.subject_id.HasValue Then
                    ret.codici.Add(New CodiciAnagrafeValori() With {
                        .codiceAnagrafe = New CodiceAnagrafe(enum_CodiciAnagrafe.Chiave_CUAA_Demetra),
                        .valore = anag.subject_id.Value.ToString()
                    })
                End If

                ret.tipo_Impresa = 1     'lavez - 18/01/2024 - fix per mancata valorizzazione
                Select Case anag.tipo_anagrafica
                    Case "P"
                        ret.forma_Giuridica = New AgronicaCoreModelsSTD.metaschema.FormeGiuridiche(1)
                    Case "G"
                        ret.forma_Giuridica = New AgronicaCoreModelsSTD.metaschema.FormeGiuridiche(2)
                    Case Else
                        Throw New DataPublishException("Forma giuridica anagrafica (" & anag.tipo_anagrafica.ToString() & ") non mappato.")
                End Select


                If ModalitaOperativa = enum_DataPublish_Configurazione.Demetra Then
                    ret.impresaPadre = New List(Of AgronicaCoreModelsSTD.anagrafiche.ImpresaPadre)
                    ret.impresaPadre.Add(New AgronicaCoreModelsSTD.anagrafiche.ImpresaPadre() With {
                                                       .partitaIva = If(ModalitaOperativa = enum_DataPublish_Configurazione.Demetra, "UZ" & anag.mandato.codice_detentore, anag.mandato.codice_detentore)
                                                  })
                Else
                    ret.impresaPadre = New List(Of AgronicaCoreModelsSTD.anagrafiche.ImpresaPadre)
                    ret.impresaPadre.Add(New AgronicaCoreModelsSTD.anagrafiche.ImpresaPadre() With {
                                                       .partitaIva = If(anag.mandato.codice_detentore = ObjParametriServer.PivaSuperUser, anag.mandato.codice_detentore, "UZ" & anag.mandato.codice_detentore.Substring(0, 3) + "000000")
                                                  })
                End If

                ret.validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale()
                ret.indirizzi = New List(Of AgronicaCoreModelsSTD.anagrafiche.IndirizzoAssociato)

                If anag.sede IsNot Nothing Then
                    Dim dtRes = xComuniR.Leggi("", "", "", "", "", AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", ObjParametriServer, anag.sede.codice_belfiore)
                    If dtRes.Rows.Count <= 0 Then
                        Throw New DataPublishException("Nessun comune codificato per il codice belfiore ( " & anag.sede.codice_belfiore & " ).")
                    End If

                    ret.indirizzi.Add(New AgronicaCoreModelsSTD.anagrafiche.IndirizzoAssociato() With {
                                    .tipo_Indirizzo = 1,
                                    .indirizzo = New AgronicaCoreModelsSTD.anagrafiche.Indirizzo() With {
                                        .istatComune = New AgronicaCoreModelsSTD.metaschema.Istat() With {
                                                        .codiceBelfiore = anag.sede.codice_belfiore,
                                                        .prov = dtRes.Rows(0)("Prov"),
                                                        .com = dtRes.Rows(0)("Com")
                                                    },
                                            .cap = anag.sede.cap,
                                            .stato = New AgronicaCoreModelsSTD.metaschema.CodiciNazioniISO3166() With {
                                                        .codice = dtRes.Rows(0)("Stato_Country")
                                                    },
                                            .codice = 0,
                                            .frazione = "",
                                            .note = "",
                                            .via = anag.sede.indirizzo
                                    }
                                    })
                Else
                    ret.indirizzi.Add(New AgronicaCoreModelsSTD.anagrafiche.IndirizzoAssociato() With {
                                    .tipo_Indirizzo = 1,
                                    .indirizzo = New AgronicaCoreModelsSTD.anagrafiche.Indirizzo() With {
                                        .istatComune = New AgronicaCoreModelsSTD.metaschema.Istat() With {
                                                        .prov = "000",
                                                        .com = "000"
                                                    },
                                            .cap = "00000",
                                            .stato = New AgronicaCoreModelsSTD.metaschema.CodiciNazioniISO3166() With {
                                                        .codice = "IT"
                                                    },
                                            .codice = 0,
                                            .frazione = "",
                                            .note = "",
                                            .via = "#"
                                    }
                                    })
                End If

                If anag.centro_aziendale IsNot Nothing Then

                    ret.centriAziendali = New List(Of AgronicaCoreModelsSTD.anagrafiche.CentroAziendale)

                    Dim c As New AgronicaCoreModelsSTD.anagrafiche.CentroAziendale(New AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK(0, anag.partita_iva))
                    c.lat = anag.centro_aziendale.lat
                    c.lng = anag.centro_aziendale.lng

                    c.titolo_Di_Possesso = New AgronicaCoreModelsSTD.metaschema.TitoloDiPossesso(1)
                    c.bioTipoAttivita = New AgronicaCoreModelsSTD.metaschema.BioTipoAttivita("")

                    c.codici = New List(Of AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori)
                    c.codici.Add(New AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori() With {
                                .codiceAnagrafe = New AgronicaCoreModelsSTD.anagrafiche.CodiceAnagrafe(102),
                                .valore = "102",
                                .validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale()
                                })

                    ret.centriAziendali.Add(c)
                End If
            Else
                'modifica

                Dim piva = xImpCodR.Piva_from_CUAA(anag.cuaa, ObjParametriServer)

                If piva = "" Then
                    Throw New DataPublishException("Nessuna azienda trovata per il CUAA (" & anag.cuaa & ")")
                End If

                ret = xImpR.Impresa_Leggi_Anagrafica(piva, True, True, False, False, True, ObjParametriServer)

                If ret.centriAziendali Is Nothing Then
                    ret.centriAziendali = New List(Of AgronicaCoreModelsSTD.anagrafiche.CentroAziendale)
                End If

                Dim dtCentriAziendali = objCentri.Leggi(piva, 0, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", ObjParametriServer)
                Dim centriAziendali As New List(Of AgronicaCoreModelsSTD.anagrafiche.CentroAziendale)

                For Each row In dtCentriAziendali.Rows
                    Dim centro = xCentriR.Centro_Leggi_Anagrafica(piva, row("Sa_Cod"), False, False, False, False, False, ObjParametriServer)
                    ret.centriAziendali.Add(centro)
                Next

                ret.partitaIvaReale = anag.partita_iva

                ret.ragioneSociale = anag.denominazione
                Select Case anag.tipo_anagrafica
                    Case "P"
                        ret.forma_Giuridica = New AgronicaCoreModelsSTD.metaschema.FormeGiuridiche(1)
                    Case "G"
                        ret.forma_Giuridica = New AgronicaCoreModelsSTD.metaschema.FormeGiuridiche(2)
                    Case Else
                        Throw New DataPublishException("Forma giuridica anagrafica (" & anag.tipo_anagrafica.ToString() & ") non mappato.")
                End Select

                If anag.subject_id.HasValue Then

                    Dim existsCodiceChiave_CUAA_Demetra = ret.codici.FindIndex(Function(elem) elem.codiceAnagrafe.codice = enum_CodiciAnagrafe.Chiave_CUAA_Demetra)

                    If existsCodiceChiave_CUAA_Demetra >= 0 Then
                        ret.codici.Item(existsCodiceChiave_CUAA_Demetra).valore = anag.subject_id.Value.ToString()
                    Else
                        ret.codici.Add(New CodiciAnagrafeValori() With {
                            .codiceAnagrafe = New CodiceAnagrafe(enum_CodiciAnagrafe.Chiave_CUAA_Demetra),
                            .valore = anag.subject_id.Value.ToString()
                        })
                    End If
                End If

                If ModalitaOperativa = enum_DataPublish_Configurazione.Demetra Then
                    ret.impresaPadre = New List(Of AgronicaCoreModelsSTD.anagrafiche.ImpresaPadre)
                    ret.impresaPadre.Add(New AgronicaCoreModelsSTD.anagrafiche.ImpresaPadre() With {
                                                       .partitaIva = If(ModalitaOperativa = enum_DataPublish_Configurazione.Demetra, "UZ" & anag.mandato.codice_detentore, anag.mandato.codice_detentore)
                                                  })
                Else
                    ret.impresaPadre = New List(Of AgronicaCoreModelsSTD.anagrafiche.ImpresaPadre)
                    ret.impresaPadre.Add(New AgronicaCoreModelsSTD.anagrafiche.ImpresaPadre() With {
                                                       .partitaIva = If(anag.mandato.codice_detentore = ObjParametriServer.PivaSuperUser, anag.mandato.codice_detentore, "UZ" & anag.mandato.codice_detentore.Substring(0, 3) + "000000")
                                                  })

                    _logger.AppendLog(cuaa, "anagrafica", String.Format("[{0} - {1} - {2}] Aggiornamento impresa -> piva codice detentore {3})", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, If(anag.mandato.codice_detentore = ObjParametriServer.PivaSuperUser, anag.mandato.codice_detentore, "UZ" & anag.mandato.codice_detentore.Substring(0, 3) + "000000")), LogType.Informazione, Nothing, True)
                End If

                If anag.sede IsNot Nothing Then
                    Dim dtRes = xComuniR.Leggi("", "", "", "", "", AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", ObjParametriServer, anag.sede.codice_belfiore)
                    If dtRes.Rows.Count <= 0 Then
                        Throw New DataPublishException("Nessun comune codificato per il codice belfiore ( " & anag.sede.codice_belfiore & " ).")
                    End If

                    ret.indirizzi(0).indirizzo.istatComune.codiceBelfiore = anag.sede.codice_belfiore
                    ret.indirizzi(0).indirizzo.istatComune.prov = dtRes.Rows(0)("PROV")
                    ret.indirizzi(0).indirizzo.istatComune.com = dtRes.Rows(0)("COM")
                    ret.indirizzi(0).indirizzo.cap = anag.sede.cap
                    ret.indirizzi(0).indirizzo.stato.codice = "IT"
                    ret.indirizzi(0).indirizzo.via = anag.sede.indirizzo
                End If

                If anag.centro_aziendale IsNot Nothing Then
                    ret.centriAziendali(0).lat = anag.centro_aziendale.lat
                    ret.centriAziendali(0).lng = anag.centro_aziendale.lng
                End If
            End If

        Catch ex As DataPublishException
            ret = Nothing
            _logger.AppendLog(cuaa, "anagrafica", ex.Message, LogType.Errore, ex)
            Throw New DataPublishException(ex.Message, ex)
        Catch ex As Exception
            ret = Nothing
            _logger.AppendLog(cuaa, "anagrafica", ex.Message, LogType.Errore, ex)
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret

    End Function



    Public Shared Function MapTerreniToCatasto(ByVal cuaa As ObjNotifica(Of CUAAObj),
                                               ByVal CentroPK As AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK,
                                               ByVal catasto As Terreni,
                                               ByRef _logger As LoggerManager) As List(Of AgronicaCoreDTOStd.InData.Anagrafica.ScriviCatasto)
        Dim catastoAziendale = New List(Of AgronicaCoreDTOStd.InData.Anagrafica.ScriviCatasto)

        Try
            For Each rec In catasto.records
                Dim possessi As New List(Of AgronicaCoreModelsSTD.anagrafiche.PossessoParticella)
                possessi.Add(New AgronicaCoreModelsSTD.anagrafiche.PossessoParticella() With {
                                                .titolo_Di_Possesso = New AgronicaCoreModelsSTD.metaschema.TitoloDiPossesso(IIf(rec.codice_titolo = "1", 1, 3)),
                                                .validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(DecodeDate(rec.data_inizio_conduzione),
                                                                                                                        DecodeDate(rec.data_fine_conduzione)),
                                                .Area = rec.area_condotta_mq / 10000
                                                })
                Dim newValue = New AgronicaCoreModelsSTD.anagrafiche.CatastoCentroAziendale() With {
                                        .centro = CentroPK,
                                        .particella = New AgronicaCoreModelsSTD.anagrafiche.ParticelleCatastali() With {
                                            .primaryKey = New AgronicaCoreModelsSTD.anagrafiche.ParticelleCatastali.PK(rec.istat_prov,
                                                                                                                        rec.istat_comu,
                                                                                                                        IIf(rec.sezione Is Nothing, "", rec.sezione),
                                                                                                                        rec.foglio,
                                                                                                                        rec.particella,
                                                                                                                        rec.subalterno),
                                            .Area = rec.area_catasto_mq / 10000
                                        },
                                        .possessiParticella = possessi
                                    }
                catastoAziendale.Add(New AgronicaCoreDTOStd.InData.Anagrafica.ScriviCatasto(Nothing, newValue))
            Next

        Catch ex As Exception
            catastoAziendale = Nothing
            _logger.AppendLog(cuaa, "terreni", ex.Message, LogType.Errore, ex)
            Throw New Exception(ex.Message, ex)
        End Try
        Return catastoAziendale

    End Function

    Public Shared Function MapTerreniToCatasto(ByVal cuaa As ObjNotifica(Of CUAAObj),
                                               ByVal CentroPK As AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK,
                                               ByVal catasto_changes As List(Of TerrenoSync),
                                               ByRef _logger As LoggerManager) As List(Of AgronicaCoreDTOStd.InData.Anagrafica.ScriviCatasto)
        Dim catastoAziendale = New List(Of AgronicaCoreDTOStd.InData.Anagrafica.ScriviCatasto)
        Dim possessi As List(Of AgronicaCoreModelsSTD.anagrafiche.PossessoParticella) = Nothing
        Try
            For Each particella In catasto_changes
                Select Case particella.tipo_modifica
                    Case "I"
                        'inserimento
                        possessi = New List(Of AgronicaCoreModelsSTD.anagrafiche.PossessoParticella)
                        possessi.Add(New AgronicaCoreModelsSTD.anagrafiche.PossessoParticella() With {
                                                        .titolo_Di_Possesso = New AgronicaCoreModelsSTD.metaschema.TitoloDiPossesso(IIf(particella.codice_titolo = "1", 1, 3)),
                                                        .validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(DecodeDate(particella.data_inizio_conduzione),
                                                                                                                                DecodeDate(particella.data_fine_conduzione)),
                                                        .Area = particella.area_condotta_mq / 10000
                                                        })
                        Dim newValue = New AgronicaCoreModelsSTD.anagrafiche.CatastoCentroAziendale() With {
                                                .centro = CentroPK,
                                                .particella = New AgronicaCoreModelsSTD.anagrafiche.ParticelleCatastali() With {
                                                    .primaryKey = New AgronicaCoreModelsSTD.anagrafiche.ParticelleCatastali.PK(particella.istat_prov,
                                                                                                                                particella.istat_comu,
                                                                                                                                IIf(particella.sezione Is Nothing, "", particella.sezione),
                                                                                                                                particella.foglio,
                                                                                                                                particella.particella,
                                                                                                                                particella.subalterno),
                                                    .Area = particella.area_catasto_mq / 10000
                                                },
                                                .possessiParticella = possessi
                                            }
                        catastoAziendale.Add(New AgronicaCoreDTOStd.InData.Anagrafica.ScriviCatasto(Nothing, newValue))

                    Case "A"
                        'aggiornamento
                        possessi = New List(Of AgronicaCoreModelsSTD.anagrafiche.PossessoParticella)
                        possessi.Add(New AgronicaCoreModelsSTD.anagrafiche.PossessoParticella() With {
                                                        .titolo_Di_Possesso = New AgronicaCoreModelsSTD.metaschema.TitoloDiPossesso(IIf(particella.codice_titolo = "1", 1, 3)),
                                                        .validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(DecodeDate(particella.data_inizio_conduzione),
                                                                                                                                DecodeDate(particella.data_fine_conduzione)),
                                                        .Area = particella.area_condotta_mq / 10000
                                                        })
                        Dim newValue = New AgronicaCoreModelsSTD.anagrafiche.CatastoCentroAziendale() With {
                                                .centro = CentroPK,
                                                .particella = New AgronicaCoreModelsSTD.anagrafiche.ParticelleCatastali() With {
                                                    .primaryKey = New AgronicaCoreModelsSTD.anagrafiche.ParticelleCatastali.PK(particella.istat_prov,
                                                                                                                                particella.istat_comu,
                                                                                                                                IIf(particella.sezione Is Nothing, "", particella.sezione),
                                                                                                                                particella.foglio,
                                                                                                                                particella.particella,
                                                                                                                                particella.subalterno),
                                                    .Area = particella.area_catasto_mq / 10000
                                                },
                                                .possessiParticella = possessi
                                            }
                        catastoAziendale.Add(New AgronicaCoreDTOStd.InData.Anagrafica.ScriviCatasto(Nothing, newValue))
                    Case "C"
                        'cancellazione
                        possessi = New List(Of AgronicaCoreModelsSTD.anagrafiche.PossessoParticella)
                        possessi.Add(New AgronicaCoreModelsSTD.anagrafiche.PossessoParticella() With {
                                                        .titolo_Di_Possesso = New AgronicaCoreModelsSTD.metaschema.TitoloDiPossesso(IIf(particella.codice_titolo = "1", 1, 3)),
                                                        .validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(DecodeDate(particella.data_inizio_conduzione),
                                                                                                                                DecodeDate(particella.data_fine_conduzione)),
                                                        .Area = particella.area_condotta_mq / 10000
                                                        })
                        Dim newValue = New AgronicaCoreModelsSTD.anagrafiche.CatastoCentroAziendale() With {
                                                .centro = CentroPK,
                                                .particella = New AgronicaCoreModelsSTD.anagrafiche.ParticelleCatastali() With {
                                                    .primaryKey = New AgronicaCoreModelsSTD.anagrafiche.ParticelleCatastali.PK(particella.istat_prov,
                                                                                                                                particella.istat_comu,
                                                                                                                                IIf(particella.sezione Is Nothing, "", particella.sezione),
                                                                                                                                particella.foglio,
                                                                                                                                particella.particella,
                                                                                                                                particella.subalterno),
                                                    .Area = particella.area_catasto_mq / 10000
                                                },
                                                .possessiParticella = possessi,
                                                .flag_cancellazione = True
                                            }
                        catastoAziendale.Add(New AgronicaCoreDTOStd.InData.Anagrafica.ScriviCatasto(Nothing, newValue))
                End Select
            Next

        Catch ex As Exception
            catastoAziendale = Nothing
            _logger.AppendLog(cuaa, "terreni", ex.Message, LogType.Errore, ex)
            Throw New Exception(ex.Message, ex)
        End Try
        Return catastoAziendale

    End Function

    Public Shared Function MapPCGToAppezzamentoCreazione(ByVal cuaa As ObjNotifica(Of CUAAObj),
                                                         ByVal CentroPK As AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK,
                                                         ByVal pcg As Appezzamento,
                                                         ByVal pcgterreni As List(Of AppezzamentoTerreno),
                                                         ByRef ObjParametriSuperServer As AgronicaCoreParametri,
                                                         ByRef ObjParametriServer As AgronicaCoreParametri,
                                                         ByRef ObjParametriUtenti As AgronicaCoreParametri,
                                                         ByRef _logger As LoggerManager,
                                                         Optional ByVal VincoliList As List(Of AgronicaCoreModelsSTD.metaschema.Vincolo) = Nothing,
                                                         Optional ByVal TagName As String = "",
                                                         Optional ByVal ReplaceInvalidPolygon As Boolean = True,
                                                         Optional ByVal TipoConfigurazione As enum_DataPublish_Configurazione = enum_DataPublish_Configurazione.Demetra,
                                                         Optional ByVal SetAppezzaAddress As Boolean = False,
                                                         Optional ByVal LogVerbose As Boolean = False
                                                         ) As AgronicaCoreModelsSTD.anagrafiche.Appezzamento
        Dim ret As AgronicaCoreModelsSTD.anagrafiche.Appezzamento = Nothing
        Dim AppCheck As AgronicaCoreModelsSTD.anagrafiche.Appezzamento = Nothing
        Dim xAPPR As New AgronicaCoreAnagrafeBIZ.Appezzamento_R
        Dim xGrfiXVegCod_R As New AgronicaCoreMetaSchemaDAL.GruppoFinalitaxSpecieVegetali_R
        Try
            'Lavez - 27/05/2025 - Log verboso
            If LogVerbose Then
                _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}]  Inizio mapping oggetto appezzamento creazione", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), LogType.Informazione, bypassElastiSearch:=LogVerbose)
            End If
            If pcg.gis.wkt = "" Then
                Throw New DataPublishException("Poligono gis appezzamento non presente")
            End If

            Dim appPK = AgronicaCoreAnagrafeBIZ.Appezzamento_R.VerificaEsistenzaAppezzamentoDaCodice(CentroPK.partitaIva, CentroPK.codice, pcg.id_appezzamento, ObjParametriServer, False)
            If appPK Is Nothing Then
                'Lavez - 27/05/2025 - Log verboso
                If LogVerbose Then
                    _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}]  Inizio creazione oggetto appezzamento", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), LogType.Informazione, bypassElastiSearch:=LogVerbose)
                End If
                'creazione
                ret = New AgronicaCoreModelsSTD.anagrafiche.Appezzamento
                ret.primaryKey = New AgronicaCoreModelsSTD.anagrafiche.Appezzamento.PK(0, CentroPK)

                ret.metodo_Produzione = New AgronicaCoreModelsSTD.metaschema.MetodoProduzione(enum_MetodoProduzione.Integrato)
                ret.descrizione = pcg.denominazione
                ret.flag_gps = False
                ret.rif_Appezzamento = pcg.id_appezzamento
                ret.cartografia = GetCartography(cuaa, pcg.gis.wkt, pcg.gis.sr_code, "EPSG:4326", pcg.id_appezzamento_padre, pcg.id_appezzamento, _logger, ObjParametriServer, ReplaceInvalidPolygon, LogVerbose:=LogVerbose)
                ret.superficie = pcg.area_mq / 10000

                ret.validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(DecodeDate(pcg.data_inizio_validita), DecodeDate(pcg.data_fine_validita))
                ret.impianti = New List(Of AgronicaCoreModelsSTD.anagrafiche.Impianto)

                'Lavez - 18/12/2024 - aggiunto codice 1360 (Appezzamento proveniente da AGEA)
                ret.codici = New List(Of CodiciAnagrafeValori)
                ret.codici.Add(New AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori() With {
                                    .codiceAnagrafe = New AgronicaCoreModelsSTD.anagrafiche.CodiceAnagrafe(CODICE_Appezzamento_AGEA),
                                    .validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(),
                                    .valore = If(pcg.is_certified = True, 1, 0).ToString()
                                })

                'Lavez - 26/05/2025 - aggiunta recupero indirizzo appezzamento da codice nazionale
                'Lavez - 26/05/2025 - aggiunta recupero indirizzo appezzamento da codice nazionale
                If SetAppezzaAddress Then
                    ImpostaAppezzamentoIndirizzoCreazione(cuaa, pcg, ret, ObjParametriServer, _logger)
                End If

            Else
                'modifica / estensione annuale
                'Lavez - 27/05/2025 - Log verboso
                If LogVerbose Then
                    _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] Inizio modifica oggetto appezzamento", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), LogType.Informazione, bypassElastiSearch:=LogVerbose)
                End If
                ret = xAPPR.Leggi_Appezzamento_Anagrafica(CentroPK.partitaIva, CentroPK.codice, appPK.codice, 0, True, True, True, Date.Now, False, True, False, ObjParametriSuperServer, ObjParametriServer, ObjParametriUtenti)

                'controllo valorizzazione pregresso GRFI_COD se specie valorizzzata ma gruppo finalità no-> eccezione
                For Each imp In ret.impianti
                    If imp.utilizzoTerreno.classType = ClassType.Varieta Then
                        If imp.gruppoFinalita IsNot Nothing AndAlso imp.gruppoFinalita.codice = 0 Then
                            Throw New GiasException(String.Format("Impianto GIAS ({0}) associato a specie vegetale {1} con GruppoFinalità nullo. Correggere l'anomalia prima di proseguire", imp.codiceImpianto, CType(imp.utilizzoTerreno, AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta).specie.codice))
                        End If
                    End If
                Next

                ret.cartografia = GetCartography(cuaa, pcg.gis.wkt, pcg.gis.sr_code, "EPSG:4326", pcg.id_appezzamento_padre, pcg.id_appezzamento, _logger, ObjParametriServer, ReplaceInvalidPolygon, LogVerbose:=LogVerbose)
                ret.superficie = pcg.area_mq / 10000

                'Lavez - 28/10/2024 - Ticket #155377
                ret.descrizione = pcg.denominazione

                'Lavez - 18/12/2024 - aggiunto codice 1360 (Appezzamento proveniente da AGEA)
                ret.codici = New List(Of CodiciAnagrafeValori)
                ret.codici.Add(New AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori() With {
                                    .codiceAnagrafe = New AgronicaCoreModelsSTD.anagrafiche.CodiceAnagrafe(CODICE_Appezzamento_AGEA),
                                    .validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(),
                                    .valore = If(pcg.is_certified = True, 1, 0).ToString()
                                })
                'Lavez - 26/05/2025 - aggiunta recupero indirizzo appezzamento da codice nazionale
                If SetAppezzaAddress Then
                    ImpostaAppezzamentoIndirizzoCreazione(cuaa, pcg, ret, ObjParametriServer, _logger)
                End If
            End If

            'Lavez - 21/03/2024 - aggiunta gestione chiavi nuovo tracciato agea
            If pcg.chiave IsNot Nothing Then
                MapCodiciChiaveAgeaAppezzamento(ret, pcg.chiave)
            End If


            For Each dic In pcg.dichiarazioni
                Dim impianto = ret.impianti.Where(Function(x) x.codiceImpianto = pcg.id_appezzamento & "|" & dic.id_dichiarazione).FirstOrDefault()
                If impianto Is Nothing Then
                    AggiungiNuovoImpianto(cuaa,
                                          ret.impianti,
                                          ret.primaryKey,
                                          pcg,
                                          dic,
                                          ret.cartografia,
                                          _logger,
                                          ObjParametriSuperServer,
                                          ObjParametriServer,
                                          ObjParametriUtenti,
                                          VincoliList,
                                          TagName,
                                          TipoConfigurazione,
                                          LogVerbose)
                Else
                    ModificaImpiantoEsistente(cuaa,
                                              impianto,
                                              pcg,
                                              dic,
                                              ret.cartografia,
                                              _logger,
                                              ObjParametriSuperServer,
                                              ObjParametriServer,
                                              ObjParametriUtenti,
                                              VincoliList,
                                              TagName,
                                              TipoConfigurazione,
                                              LogVerbose)
                End If
            Next

            RicalcolaDateInizioFineValiditaAppezzamento(cuaa, ret, pcg, _logger)

            'appezzamenti per particelle
            ret.catastoAppezzamento = New List(Of AgronicaCoreModelsSTD.anagrafiche.CatastoAppezzamento)
            If pcgterreni IsNot Nothing Then
                For Each part In pcgterreni
                    ret.catastoAppezzamento.Add(New AgronicaCoreModelsSTD.anagrafiche.CatastoAppezzamento() With {
                                                            .area = 0.1,
                                                            .particella = New AgronicaCoreModelsSTD.anagrafiche.ParticelleCatastali.PK(part.istat_prov, part.istat_comu, IIf(part.sezione Is Nothing, "", part.sezione), part.foglio, part.particella, part.subalterno)
                                                        })
                Next
            End If

            ret.metodo_Produzione = GetMetodoProduzione(pcg.dichiarazioni)

            'Lavez - 27/05/2025 - Log verboso
            If LogVerbose Then
                _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] fine mapping oggetto appezzamento creazione", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), LogType.Informazione, bypassElastiSearch:=LogVerbose)
            End If
        Catch ex As DataPublishException
            ret = Nothing
            _logger.AppendLog(cuaa, "pcg", ex.Message, LogType.Errore, ex)
            Throw New DataPublishException(ex.Message, ex)
        Catch ex As Exception
            ret = Nothing
            _logger.AppendLog(cuaa, "pcg", ex.Message, LogType.Errore, ex)
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Public Shared Function MapPCGToAppezzamentoModifica(ByVal cuaa As ObjNotifica(Of CUAAObj),
                                                        ByVal CentroPK As AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK,
                                                        ByVal pcg As AppezzamentoSync,
                                                        ByVal pcgterreni As List(Of AppezzamentoTerreno),
                                                        ByRef ObjParametriSuperServer As AgronicaCoreParametri,
                                                        ByRef ObjParametriServer As AgronicaCoreParametri,
                                                        ByRef ObjParametriUtenti As AgronicaCoreParametri,
                                                        ByRef _logger As LoggerManager,
                                                        Optional ByVal VincoliList As List(Of AgronicaCoreModelsSTD.metaschema.Vincolo) = Nothing,
                                                        Optional ByVal TagName As String = "",
                                                        Optional ByVal ReplaceInvalidPolygon As Boolean = True,
                                                        Optional ByVal TipoConfigurazione As enum_DataPublish_Configurazione = enum_DataPublish_Configurazione.Demetra,
                                                        Optional ByVal SetAppezzaAddress As Boolean = False,
                                                        Optional ByVal LogVerbose As Boolean = False,
                                                        Optional ByVal CancellazioneLogica As Boolean = False
                                                        ) As AgronicaCoreModelsSTD.anagrafiche.Appezzamento
        Dim ret As AgronicaCoreModelsSTD.anagrafiche.Appezzamento = Nothing

        Try
            ret = New AgronicaCoreModelsSTD.anagrafiche.Appezzamento

            Dim appPK As AgronicaCoreModelsSTD.anagrafiche.Appezzamento.PK = Nothing
            Dim xAppR As New AgronicaCoreAnagrafeBIZ.Appezzamento_R

            If pcg.tipo_modifica <> "C" Then
                If pcg.gis.wkt = "" Then
                    Throw New DataPublishException("Poligono gis appezzamento non presente")
                End If
            End If

            'Lavez - 27/05/2025 - Log verboso
            If LogVerbose Then
                _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}]  Inizio mapping oggetto appezzamento", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), LogType.Informazione, bypassElastiSearch:=LogVerbose)
            End If

            Select Case pcg.tipo_modifica
                Case "I"
                    _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] Evento di inserimento per piano colturale grafico. Chiavi: CUAA ({0}) - Campagna ({1}) - id_appezzamento_padre ({4}) - id_appezzamento ({5})", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, pcg.campagna, pcg.id_appezzamento_padre, pcg.id_appezzamento))
                    'creazione
                    'Lavez - 27/05/2025 - Log verboso
                    If LogVerbose Then
                        _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] - tipo_modifica I - prima di lettura appezzamento", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), LogType.Informazione, bypassElastiSearch:=LogVerbose)
                    End If

                    appPK = AgronicaCoreAnagrafeBIZ.Appezzamento_R.VerificaEsistenzaAppezzamentoDaCodice(CentroPK.partitaIva, CentroPK.codice, pcg.id_appezzamento, ObjParametriServer, False)
                    If appPK Is Nothing Then
                        'creazione
                        ret.primaryKey = New AgronicaCoreModelsSTD.anagrafiche.Appezzamento.PK(0, CentroPK)


                        ret.descrizione = pcg.denominazione
                        ret.flag_gps = False
                        ret.rif_Appezzamento = pcg.id_appezzamento

                        ret.cartografia = GetCartography(cuaa, pcg.gis.wkt, pcg.gis.sr_code, "EPSG:4326", pcg.id_appezzamento_padre, pcg.id_appezzamento, _logger, ObjParametriServer, ReplaceInvalidPolygon, LogVerbose:=LogVerbose)
                        ret.superficie = pcg.area_mq / 10000
                        ret.validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(DecodeDate(pcg.data_inizio_validita), DecodeDate(pcg.data_fine_validita))
                        ret.impianti = New List(Of AgronicaCoreModelsSTD.anagrafiche.Impianto)

                        'Lavez - 18/12/2024 - aggiunto codice 1360 (Appezzamento proveniente da AGEA)
                        ret.codici = New List(Of CodiciAnagrafeValori)
                        ret.codici.Add(New AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori() With {
                                    .codiceAnagrafe = New AgronicaCoreModelsSTD.anagrafiche.CodiceAnagrafe(CODICE_Appezzamento_AGEA),
                                    .validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(),
                                    .valore = If(pcg.is_certified = True, 1, 0).ToString()
                                })
                        'Lavez - 27/05/2025 - Log verboso
                        If LogVerbose Then
                            _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] - Creazione - prima di mapping appezzamento indirizzi", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), LogType.Informazione, bypassElastiSearch:=LogVerbose)
                        End If

                        'Lavez - 26/05/2025 - aggiunta recupero indirizzo appezzamento da codice nazionale
                        If SetAppezzaAddress Then
                            ImpostaAppezzamentoIndirizzoModifica(cuaa, pcg, ret, ObjParametriServer, _logger)
                        End If

                        'Lavez - 27/05/2025 - Log verboso
                        If LogVerbose Then
                            _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] - Creazione - dopo di mapping appezzamento indirizzi", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), LogType.Informazione, bypassElastiSearch:=LogVerbose)
                        End If

                        'Lavez - 21/03/2024 - aggiunta gestione chiavi nuovo tracciato agea
                        If pcg.chiave IsNot Nothing Then
                            MapCodiciChiaveAgeaAppezzamento(ret, pcg.chiave)
                        End If

                        For Each dic In pcg.dichiarazioni

                            AggiungiNuovoImpianto(cuaa,
                                                  ret.impianti,
                                                  ret.primaryKey,
                                                  pcg,
                                                  dic,
                                                  ret.cartografia,
                                                  _logger,
                                                  ObjParametriSuperServer,
                                                  ObjParametriServer,
                                                  ObjParametriUtenti,
                                                  VincoliList,
                                                  TagName,
                                                  TipoConfigurazione,
                                                  LogVerbose)

                        Next
                    Else
                        'modifica
                        ret.primaryKey = New AgronicaCoreModelsSTD.anagrafiche.Appezzamento.PK(appPK.codice, CentroPK)

                        'Lavez - 27/05/2025 - Log verboso
                        If LogVerbose Then
                            _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] - Modifica - prima di lettura appezzamento", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), LogType.Informazione, bypassElastiSearch:=LogVerbose)
                        End If

                        ret = xAppR.Leggi_Appezzamento_Anagrafica(appPK.centroAziendalePK.partitaIva,
                                                             appPK.centroAziendalePK.codice,
                                                             appPK.codice,
                                                             0,
                                                             True,
                                                             True,
                                                             True,
                                                             Date.Now,
                                                             False,
                                                             True,
                                                             False,
                                                             ObjParametriSuperServer,
                                                             ObjParametriServer,
                                                             ObjParametriUtenti)
                        'Lavez - 27/05/2025 - Log verboso
                        If LogVerbose Then
                            _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] - Modifica - dopo di lettura appezzamento", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), LogType.Informazione, bypassElastiSearch:=LogVerbose)
                        End If

                        ret.cartografia = GetCartography(cuaa, pcg.gis.wkt, pcg.gis.sr_code, "EPSG:4326", pcg.id_appezzamento_padre, pcg.id_appezzamento, _logger, ObjParametriServer, ReplaceInvalidPolygon, LogVerbose:=LogVerbose)
                        ret.superficie = pcg.area_mq / 10000

                        'Lavez - 28/10/2024 - Ticket #155377
                        ret.descrizione = pcg.denominazione

                        'Lavez - 18/12/2024 - aggiunto codice 1360 (Appezzamento proveniente da AGEA)
                        ret.codici = New List(Of CodiciAnagrafeValori)
                        ret.codici.Add(New AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori() With {
                                    .codiceAnagrafe = New AgronicaCoreModelsSTD.anagrafiche.CodiceAnagrafe(CODICE_Appezzamento_AGEA),
                                    .validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(),
                                    .valore = If(pcg.is_certified = True, 1, 0).ToString()
                                })

                        'Lavez - 27/05/2025 - Log verboso
                        If LogVerbose Then
                            _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] - Modifica - prima di mapping appezzamento indirizzi", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), LogType.Informazione, bypassElastiSearch:=LogVerbose)
                        End If

                        'Lavez - 26/05/2025 - aggiunta recupero indirizzo appezzamento da codice nazionale
                        If SetAppezzaAddress Then
                            ImpostaAppezzamentoIndirizzoModifica(cuaa, pcg, ret, ObjParametriServer, _logger)
                        End If

                        'Lavez - 27/05/2025 - Log verboso
                        If LogVerbose Then
                            _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] - Modifica - dopo di mapping appezzamento indirizzi", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), LogType.Informazione, bypassElastiSearch:=LogVerbose)
                        End If

                        'Lavez - 21/03/2024 - aggiunta gestione chiavi nuovo tracciato agea
                        If pcg.chiave IsNot Nothing Then
                            MapCodiciChiaveAgeaAppezzamento(ret, pcg.chiave)
                        End If

                        For Each dic In pcg.dichiarazioni


                            Dim impianto = ret.impianti.Where(Function(x) x.codiceImpianto = pcg.id_appezzamento & "|" & dic.id_dichiarazione).FirstOrDefault()
                            If impianto Is Nothing Then
                                'non esiste impianto con quel codice dichiarazione lo creo
                                _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] Chiavi: CUAA ({0}) - Campagna ({1}) - id_appezzamento_padre ({4}) - id_appezzamento ({5}) - codiceimpianto ({6}): nessun impianto trovato -> lo creo", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, pcg.campagna, pcg.id_appezzamento_padre, pcg.id_appezzamento, pcg.id_appezzamento & "|" & dic.id_dichiarazione))

                                AggiungiNuovoImpianto(cuaa,
                                                      ret.impianti,
                                                      ret.primaryKey,
                                                      pcg,
                                                      dic,
                                                      ret.cartografia,
                                                      _logger,
                                                      ObjParametriSuperServer,
                                                      ObjParametriServer,
                                                      ObjParametriUtenti,
                                                      VincoliList,
                                                      TagName,
                                                      TipoConfigurazione,
                                                      LogVerbose)

                            Else
                                _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] Chiavi: CUAA ({0}) - Campagna ({1}) - id_appezzamento_padre ({4}) - id_appezzamento ({5}) - codiceimpianto ({6}): impianto trovato -> eseguo aggiornamento", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, pcg.campagna, pcg.id_appezzamento_padre, pcg.id_appezzamento, pcg.id_appezzamento & "|" & dic.id_dichiarazione))

                                ModificaImpiantoEsistente(cuaa,
                                                          impianto,
                                                          pcg,
                                                          dic,
                                                          ret.cartografia,
                                                          _logger,
                                                          ObjParametriSuperServer,
                                                          ObjParametriServer,
                                                          ObjParametriUtenti,
                                                          VincoliList,
                                                          TagName,
                                                          TipoConfigurazione,
                                                          LogVerbose)

                            End If

                        Next

                        ' ricalcolo delle date inizio\fine validità appezzamento
                        RicalcolaDateInizioFineValiditaAppezzamento(cuaa,
                                                                    ret,
                                                                    pcg,
                                                                    _logger)

                    End If

                    ret.metodo_Produzione = GetMetodoProduzione(pcg.dichiarazioni)

                    'Lavez - 27/05/2025 - Log verboso
                    If LogVerbose Then
                        _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] - tipo_modifica I - fine mapping", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), LogType.Informazione, bypassElastiSearch:=LogVerbose)
                    End If

                Case "A"
                    _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] Evento di aggiornamento per piano colturale grafico. Chiavi: CUAA ({0}) - Campagna ({1}) - id_appezzamento_padre ({4}) - id_appezzamento ({5})", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, pcg.campagna, pcg.id_appezzamento_padre, pcg.id_appezzamento))

                    'Lavez - 27/05/2025 - Log verboso
                    If LogVerbose Then
                        _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] - tipo_modifica A - prima di lettura appezzamento", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), LogType.Informazione, bypassElastiSearch:=LogVerbose)
                    End If

                    'creazione
                    appPK = AgronicaCoreAnagrafeBIZ.Appezzamento_R.VerificaEsistenzaAppezzamentoDaCodice(CentroPK.partitaIva, CentroPK.codice, pcg.id_appezzamento, ObjParametriServer, False)
                    If appPK Is Nothing Then
                        'creazione
                        ret.primaryKey = New AgronicaCoreModelsSTD.anagrafiche.Appezzamento.PK(0, CentroPK)
                        ret.metodo_Produzione = New AgronicaCoreModelsSTD.metaschema.MetodoProduzione(enum_MetodoProduzione.Integrato)
                        ret.descrizione = pcg.denominazione
                        ret.flag_gps = False
                        ret.rif_Appezzamento = pcg.id_appezzamento

                        ret.cartografia = GetCartography(cuaa, pcg.gis.wkt, pcg.gis.sr_code, "EPSG:4326", pcg.id_appezzamento_padre, pcg.id_appezzamento, _logger, ObjParametriServer, ReplaceInvalidPolygon, LogVerbose:=LogVerbose)
                        ret.superficie = pcg.area_mq / 10000
                        ret.validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(DecodeDate(pcg.data_inizio_validita), DecodeDate(pcg.data_fine_validita))
                        ret.impianti = New List(Of AgronicaCoreModelsSTD.anagrafiche.Impianto)

                        'Lavez - 18/12/2024 - aggiunto codice 1360 (Appezzamento proveniente da AGEA)
                        ret.codici = New List(Of CodiciAnagrafeValori)
                        ret.codici.Add(New AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori() With {
                                    .codiceAnagrafe = New AgronicaCoreModelsSTD.anagrafiche.CodiceAnagrafe(CODICE_Appezzamento_AGEA),
                                    .validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(),
                                    .valore = If(pcg.is_certified = True, 1, 0).ToString()
                                })

                        'Lavez - 27/05/2025 - Log verboso
                        If LogVerbose Then
                            _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] - Creazione - prima di mapping appezzamento indirizzi", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), LogType.Informazione, bypassElastiSearch:=LogVerbose)
                        End If

                        'Lavez - 26/05/2025 - aggiunta recupero indirizzo appezzamento da codice nazionale
                        If SetAppezzaAddress Then
                            ImpostaAppezzamentoIndirizzoModifica(cuaa, pcg, ret, ObjParametriServer, _logger)
                        End If

                        'Lavez - 27/05/2025 - Log verboso
                        If LogVerbose Then
                            _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] - Creazione - dopo di mapping appezzamento indirizzi", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), LogType.Informazione, bypassElastiSearch:=LogVerbose)
                        End If

                        'Lavez - 21/03/2024 - aggiunta gestione chiavi nuovo tracciato agea
                        If pcg.chiave IsNot Nothing Then
                            MapCodiciChiaveAgeaAppezzamento(ret, pcg.chiave)
                        End If

                        For Each dic In pcg.dichiarazioni

                            AggiungiNuovoImpianto(cuaa,
                                                  ret.impianti,
                                                  ret.primaryKey,
                                                  pcg,
                                                  dic,
                                                  ret.cartografia,
                                                  _logger,
                                                  ObjParametriSuperServer,
                                                  ObjParametriServer,
                                                  ObjParametriUtenti,
                                                  VincoliList,
                                                  TagName,
                                                  TipoConfigurazione,
                                                  LogVerbose)
                        Next

                    Else
                        'modifica
                        ret.primaryKey = New AgronicaCoreModelsSTD.anagrafiche.Appezzamento.PK(appPK.codice, CentroPK)

                        'Lavez - 27/05/2025 - Log verboso
                        If LogVerbose Then
                            _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] - Modifica - prima di lettura appezzamento", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), LogType.Informazione, bypassElastiSearch:=LogVerbose)
                        End If

                        ret = xAppR.Leggi_Appezzamento_Anagrafica(appPK.centroAziendalePK.partitaIva,
                                                             appPK.centroAziendalePK.codice,
                                                             appPK.codice,
                                                             0,
                                                             True,
                                                             True,
                                                             True,
                                                             Date.Now,
                                                             False,
                                                             True,
                                                             False,
                                                             ObjParametriSuperServer,
                                                             ObjParametriServer,
                                                             ObjParametriUtenti)

                        'Lavez - 27/05/2025 - Log verboso
                        If LogVerbose Then
                            _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] - Modifica - dopo di lettura appezzamento", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), LogType.Informazione, bypassElastiSearch:=LogVerbose)
                        End If

                        ret.cartografia = GetCartography(cuaa, pcg.gis.wkt, pcg.gis.sr_code, "EPSG:4326", pcg.id_appezzamento_padre, pcg.id_appezzamento, _logger, ObjParametriServer, ReplaceInvalidPolygon, LogVerbose:=LogVerbose)
                        ret.superficie = pcg.area_mq / 10000

                        'Lavez - 28/10/2024 - Ticket #155377
                        ret.descrizione = pcg.denominazione

                        'Lavez - 18/12/2024 - aggiunto codice 1360 (Appezzamento proveniente da AGEA)
                        ret.codici = New List(Of CodiciAnagrafeValori)
                        ret.codici.Add(New AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori() With {
                                    .codiceAnagrafe = New AgronicaCoreModelsSTD.anagrafiche.CodiceAnagrafe(CODICE_Appezzamento_AGEA),
                                    .validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(),
                                    .valore = If(pcg.is_certified = True, 1, 0).ToString()
                                })

                        'Lavez - 27/05/2025 - Log verboso
                        If LogVerbose Then
                            _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] - Modifica - prima di mapping appezzamento indirizzi", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), LogType.Informazione, bypassElastiSearch:=LogVerbose)
                        End If

                        'Lavez - 26/05/2025 - aggiunta recupero indirizzo appezzamento da codice nazionale
                        If SetAppezzaAddress Then
                            ImpostaAppezzamentoIndirizzoModifica(cuaa, pcg, ret, ObjParametriServer, _logger)
                        End If

                        'Lavez - 27/05/2025 - Log verboso
                        If LogVerbose Then
                            _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] - Modifica - dopo di mapping appezzamento indirizzi", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), LogType.Informazione, bypassElastiSearch:=LogVerbose)
                        End If

                        'Lavez - 21/03/2024 - aggiunta gestione chiavi nuovo tracciato agea
                        If pcg.chiave IsNot Nothing Then
                            MapCodiciChiaveAgeaAppezzamento(ret, pcg.chiave)
                        End If

                        'elimino gli impianti relativi agli avvicendamenti non più presenti nelle dichiarazioni
                        EliminaImpiantiNonPresentiInDemetra(cuaa, pcg, ret, _logger)

                        For Each dic In pcg.dichiarazioni
                            Dim impianto = ret.impianti.Where(Function(x) x.codiceImpianto = pcg.id_appezzamento & "|" & dic.id_dichiarazione).FirstOrDefault()
                            If impianto Is Nothing Then
                                'non esiste impianto con quel codice dichiarazione lo creo
                                _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] Chiavi: CUAA ({0}) - Campagna ({1}) - id_appezzamento_padre ({4}) - id_appezzamento ({5}) - codiceimpianto ({6}): nessun impianto trovato -> lo creo", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, pcg.campagna, pcg.id_appezzamento_padre, pcg.id_appezzamento, pcg.id_appezzamento & "|" & dic.id_dichiarazione), bypassElastiSearch:=True)

                                AggiungiNuovoImpianto(cuaa,
                                                      ret.impianti,
                                                      ret.primaryKey,
                                                      pcg,
                                                      dic,
                                                      ret.cartografia,
                                                      _logger,
                                                      ObjParametriSuperServer,
                                                      ObjParametriServer,
                                                      ObjParametriUtenti,
                                                      VincoliList,
                                                      TagName,
                                                      TipoConfigurazione,
                                                      LogVerbose)

                            Else
                                _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] Chiavi: CUAA ({0}) - Campagna ({1}) - id_appezzamento_padre ({4}) - id_appezzamento ({5}) - codiceimpianto ({6}): impianto trovato -> eseguo aggiornamento", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, pcg.campagna, pcg.id_appezzamento_padre, pcg.id_appezzamento, pcg.id_appezzamento & "|" & dic.id_dichiarazione), bypassElastiSearch:=True)

                                ModificaImpiantoEsistente(cuaa,
                                                          impianto,
                                                          pcg,
                                                          dic,
                                                          ret.cartografia,
                                                          _logger,
                                                          ObjParametriSuperServer,
                                                          ObjParametriServer,
                                                          ObjParametriUtenti,
                                                          VincoliList,
                                                          TagName,
                                                          TipoConfigurazione,
                                                          LogVerbose)

                            End If
                        Next

                        ' ricalcolo delle date inizio\fine validità appezzamento
                        RicalcolaDateInizioFineValiditaAppezzamento(cuaa,
                                                                    ret,
                                                                    pcg,
                                                                    _logger)
                    End If

                    ret.metodo_Produzione = GetMetodoProduzione(pcg.dichiarazioni)

                    'Lavez - 27/05/2025 - Log verboso
                    If LogVerbose Then
                        _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] - tipo_modifica A - fine mapping", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), LogType.Informazione, bypassElastiSearch:=LogVerbose)
                    End If

                Case "C"
                    _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] Evento di cancellazione per piano colturale grafico. Chiavi: CUAA ({0}) - Campagna ({3}) - id_appezzamento_padre ({4}) - id_appezzamento ({5})", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, pcg.campagna, pcg.id_appezzamento_padre, pcg.id_appezzamento))

                    'Lavez - 27/05/2025 - Log verboso
                    If LogVerbose Then
                        _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] - tipo_modifica C - prima di lettura appezzamento", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), LogType.Informazione, bypassElastiSearch:=LogVerbose)
                    End If

                    'cancellazione
                    appPK = AgronicaCoreAnagrafeBIZ.Appezzamento_R.VerificaEsistenzaAppezzamentoDaCodice(CentroPK.partitaIva, CentroPK.codice, pcg.id_appezzamento, ObjParametriServer, False)
                    If appPK Is Nothing Then
                        _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] Evento di cancellazione per piano colturale grafico. Chiavi: CUAA ({0}) - Campagna ({3}) - id_appezzamento_padre ({4}) - id_appezzamento ({5}) ma non presente in gias. record ignorato", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, pcg.campagna, pcg.id_appezzamento_padre, pcg.id_appezzamento), bypassElastiSearch:=True)
                        ret = Nothing  'forzo il valore di ritorno a nothing per non richiamare la scrivi_appezzamento
                    Else
                        'Lavez - 23/06/2025 - prima di tutto controllo la pr
                        If CancellazioneLogica Then
                            _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] Evento di cancellazione per piano colturale grafico CON cancellazione logica attiva. Chiavi: CUAA ({0}) - Campagna ({3}) - id_appezzamento_padre ({4}) - id_appezzamento ({5})", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, pcg.campagna, pcg.id_appezzamento_padre, pcg.id_appezzamento), bypassElastiSearch:=True)

                            '0 - verifica condizion per cancellazione logica
                            '- ha movimenti associati ? QDC/UMA/CDG


                            Dim appezza = xAppR.Leggi_Appezzamento_Anagrafica(appPK.centroAziendalePK.partitaIva,
                                         appPK.centroAziendalePK.codice,
                                         appPK.codice,
                                         0,
                                         True,
                                         True,
                                         True,
                                         Date.Now,
                                         False,
                                         True,
                                         False,
                                         ObjParametriSuperServer,
                                         ObjParametriServer,
                                         ObjParametriUtenti)

                            If CheckRequisitiCancellazione(cuaa,
                                                           appezza,
                                                           ObjParametriSuperServer,
                                                           ObjParametriServer,
                                                           ObjParametriUtenti,
                                                           _logger) Then
                                'cancellazione
                                ImpostaEliminaAppezzamentoImpianti(cuaa,
                                                                   pcg,
                                                                   ret,
                                                                   appPK,
                                                                   ObjParametriSuperServer,
                                                                   ObjParametriServer,
                                                                   ObjParametriUtenti,
                                                                   _logger,
                                                                   LogVerbose,
                                                                   TipoConfigurazione,
                                                                   False)

                            Else
                                'aggiornamento con flagCessata="1"
                                For Each impianto In appezza.impianti
                                    _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] Chiavi: CUAA ({0}) - Campagna ({3}) - id_appezzamento_padre ({4}) - id_appezzamento ({5}) : aggiornamento impianto {6} con flag cessata", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, pcg.campagna, pcg.id_appezzamento_padre, pcg.id_appezzamento, impianto.codiceImpianto), bypassElastiSearch:=True)
                                    impianto.flagCessata = "1"
                                Next

                                ret = appezza
                            End If


                        Else
                            ImpostaEliminaAppezzamentoImpianti(cuaa,
                                                               pcg,
                                                               ret,
                                                               appPK,
                                                               ObjParametriSuperServer,
                                                               ObjParametriServer,
                                                               ObjParametriUtenti,
                                                               _logger,
                                                               LogVerbose,
                                                               TipoConfigurazione,
                                                               False)

                        End If

                        'lavez - 30/01/2024 - controllo di sicurezza per evitare orfani sugli appezzamenti
                        If ret.impianti.Count = 0 AndAlso ret.flag_cancellazione = False Then
                            _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] Chiavi: CUAA ({0}) - Campagna ({3}) - id_appezzamento_padre ({4}) - id_appezzamento ({5}) - appezzamento senza impianti con flag_cancellazione a false. forzo la cancellazione dell'appezzamento ", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, pcg.campagna, pcg.id_appezzamento_padre, pcg.id_appezzamento), bypassElastiSearch:=True)
                            _logger.AppendLog(cuaa, "pcg", "DBG: " & vbCrLf & JsonConvert.SerializeObject(ret), bypassElastiSearch:=True)
                            ret.flag_cancellazione = True
                        End If

                        ' ricalcolo delle date inizio\fine validità appezzamento
                        RicalcolaDateInizioFineValiditaAppezzamento(cuaa,
                                                                    ret,
                                                                    pcg,
                                                                    _logger)

                    End If
                    'Lavez - 27/05/2025 - Log verboso
                    If LogVerbose Then
                        _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] - tipo_modifica C - fine mapping", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), LogType.Informazione, bypassElastiSearch:=LogVerbose)
                    End If
            End Select

            'Lavez - 27/05/2025 - Log verboso
            If LogVerbose Then
                _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}]  Fine mapping oggetto appezzamento", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), LogType.Informazione, bypassElastiSearch:=LogVerbose)
            End If

        Catch ex As DataPublishException
            ret = Nothing
            _logger.AppendLog(cuaa, "pcg", ex.Message, LogType.Errore, ex)
            Throw New DataPublishException(ex.Message, ex)
        Catch ex As Exception
            ret = Nothing
            _logger.AppendLog(cuaa, "pcg", ex.Message, LogType.Errore, ex)
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Private Shared Sub ImpostaEliminaAppezzamentoImpianti(ByVal cuaa As ObjNotifica(Of CUAAObj),
                                                   ByVal pcg As AppezzamentoSync,
                                                   ByRef appezza As AgronicaCoreModelsSTD.anagrafiche.Appezzamento,
                                                   ByVal appPK As AgronicaCoreModelsSTD.anagrafiche.Appezzamento.PK,
                                                   ByRef ObjParametriSuperServer As AgronicaCoreParametri,
                                                   ByRef ObjParametriServer As AgronicaCoreParametri,
                                                   ByRef ObjParametriUtenti As AgronicaCoreParametri,
                                                   ByRef _logger As LoggerManager,
                                                   Optional ByVal LogVerbose As Boolean = False,
                                                   Optional ByVal TipoConfigurazione As enum_DataPublish_Configurazione = enum_DataPublish_Configurazione.Demetra,
                                                   Optional ByVal CancellazioneLogica As Boolean = False)


        Dim xAppR As New AgronicaCoreAnagrafeBIZ.Appezzamento_R

        _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] Evento di cancellazione per piano colturale grafico SENZA cancellazione logica attiva. Chiavi: CUAA ({0}) - Campagna ({3}) - id_appezzamento_padre ({4}) - id_appezzamento ({5})", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, pcg.campagna, pcg.id_appezzamento_padre, pcg.id_appezzamento), bypassElastiSearch:=True)

        Dim chkImpiantiInCampagneDifferentiDaQuellaInLinea As Boolean = False

        'Lavez - 27/05/2025 - Log verboso
        If LogVerbose Then
            _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] - Cancellazione - prima di lettura appezzamento", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), LogType.Informazione, bypassElastiSearch:=LogVerbose)
        End If

        appezza = xAppR.Leggi_Appezzamento_Anagrafica(appPK.centroAziendalePK.partitaIva,
                                                 appPK.centroAziendalePK.codice,
                                                 appPK.codice,
                                                 0,
                                                 True,
                                                 True,
                                                 True,
                                                 Date.Now,
                                                 False,
                                                 True,
                                                 False,
                                                 ObjParametriSuperServer,
                                                 ObjParametriServer,
                                                 ObjParametriUtenti)

        'Lavez - 27/05/2025 - Log verboso
        If LogVerbose Then
            _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] - Cancellazione - dopo di lettura appezzamento", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), LogType.Informazione, bypassElastiSearch:=LogVerbose)
        End If

        If TipoConfigurazione = enum_DataPublish_Configurazione.Demetra Then
            If Not (appezza.impianti.Count = 0 OrElse appezza.impianti.Where(Function(x) x.validita.fine.Year <> pcg.campagna).ToList().Count = 0) Then
                chkImpiantiInCampagneDifferentiDaQuellaInLinea = True
                _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] Evento di cancellazione per piano colturale grafico. Chiavi: CUAA ({0}) - Campagna ({3}) - id_appezzamento_padre ({4}) - id_appezzamento ({5}) rilevati impianti anche su campagne diverse da quella in elaborazione, procedo con la cancellazione del solo impianto", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, pcg.campagna, pcg.id_appezzamento_padre, pcg.id_appezzamento), bypassElastiSearch:=True)
            End If
        Else
            'Lavez - 05/03/2025 - In Regione umbria non c'è il concetto di campagna come in Demetra per cui la la logica delle colture pluriennali da ripartire su anni differenti non c'è.
            chkImpiantiInCampagneDifferentiDaQuellaInLinea = False
        End If

        'If pcg.id_appezzamento_padre = pcg.id_appezzamento AndAlso chkImpiantiInCampagneDifferentiDaQuellaInLinea = False Then
        If chkImpiantiInCampagneDifferentiDaQuellaInLinea = False Then

            'appezzamento + impianto + esercizio (oppure cancellazione primo impianto della coltura pluriennale)
            EliminaAppezzamentoImpianti(cuaa,
                                    appezza,
                                    pcg,
                                    _logger,
                                    CancellazioneLogica)

        Else
            'impianto+esercizio
            _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] Chiavi: CUAA ({0}) - Campagna ({3}) - id_appezzamento_padre ({4}) - id_appezzamento ({5}) : cancellazione appezzamento di una coltura pluriennale.", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, pcg.campagna, pcg.id_appezzamento_padre, pcg.id_appezzamento), bypassElastiSearch:=True)

            If chkImpiantiInCampagneDifferentiDaQuellaInLinea = True Then
                'devo cancellare gli impianti presenti nella campagna in linea
                EliminaImpiantiCampagnaCorrente(cuaa,
                                            appezza,
                                            pcg,
                                            _logger,
                                            CancellazioneLogica)

            Else
                'devo cancellare gli impianti non più presenti (****)

                EliminaImpiantiNonPresenti(cuaa,
                                       appezza,
                                       pcg,
                                       _logger,
                                       CancellazioneLogica)

            End If

        End If
    End Sub

    Private Shared Function CheckRequisitiCancellazione(ByVal cuaa As ObjNotifica(Of CUAAObj),
                                                        ByRef appezzamento As AgronicaCoreModelsSTD.anagrafiche.Appezzamento,
                                                        ByRef ObjParametriSuperServer As AgronicaCoreParametri,
                                                        ByRef ObjParametriServer As AgronicaCoreParametri,
                                                        ByRef ObjParametriUtenti As AgronicaCoreParametri,
                                                        ByRef _logger As LoggerManager) As Boolean
        Dim ret As Boolean = False
        Dim objAppezzamenti As New AgronicaCoreAnagrafeDAL.Appezzamento_Read

        Try
            '0 - controllo di contatori collegati all'appezzamento
            Dim dt = objAppezzamenti.Leggi_x_anagraficaNG(appezzamento.primaryKey.centroAziendalePK.partitaIva,
                                           appezzamento.primaryKey.centroAziendalePK.codice,
                                           appezzamento.campoPK.codice,
                                           appezzamento.primaryKey.codice,
                                           "",
                                           "",
                                           ObjParametriServer)
            If dt.Rows.Count <= 0 Then
                Throw New Exception(String.Format("Appezzamento con chiave {0} - {1} - {2} non trovato. Impossibile proseguire", appezzamento.primaryKey.centroAziendalePK.partitaIva, appezzamento.primaryKey.centroAziendalePK.codice, appezzamento.primaryKey.codice))
            End If

            If dt.Rows(0)("Blk_Flag") = -1 Then
                Throw New Exception(String.Format(AgronicaCoreDataProvider.My.Resources.Gias.ErroreEliminazioneImpiantoBloccato, appezzamento.descrizione))
            End If

            Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            Dim leggiMacchineAssociate As Boolean = objPermessi.Controlla_Permessi_Utente(
                ObjParametriUtenti.UtenteUsername,
                enum_Id_Servizio.GiasOnline,
                enum_Security_Attivita.GestioneAssociazioneAppezzamentiXParcoMacchine,
                enum_Security_Operazione.Modifica,
                Date.Now,
                "",
                ObjParametriUtenti
                )

            If leggiMacchineAssociate Then
                Dim objAxPR As New AgronicaCoreAnagrafeBIZ.AppezzamentiXParcoMacchine_R
                Dim dtLetture = objAxPR.ReadJoinLettureContatori(
                    ObjParametriServer,
                    ObjParametriUtenti,
                    New List(Of AgronicaCoreModelsSTD.anagrafiche.Appezzamento) From {appezzamento}
                    )

                If dtLetture.Rows.Count > 0 Then
                    Throw New Exception("Errore in cancellazione appezzamento, ci sono contatori collegati")
                End If
            End If


            Dim MessaggioErroreAgenda As String = ""
            Dim MessaggioErroreCdG As String = ""
            Dim MessaggioErroreUMA As String = ""

            ObjParametriServer.FinestraTemporaleInizio = AGRODATAINIZIO
            ObjParametriServer.FinestraTemporaleFine = AGRODATAFINE



            For Each impiantoDaEliminare In appezzamento.impianti

                Dim id_Reg = impiantoDaEliminare.primaryKey.codice
                _logger.AppendLog(cuaa, "pcg", String.Format("debug: impianto key {0}-{1}-{2}-{3}", appezzamento.primaryKey.centroAziendalePK.partitaIva, appezzamento.primaryKey.centroAziendalePK.codice, appezzamento.primaryKey.codice, id_Reg), LogType.Informazione, bypassElastiSearch:=True)

                'Prima di poter eliminare controllo CdG e Movimenti

                Dim descrizione_imp As String = "[dal " & impiantoDaEliminare.validita.inizio & " al " & impiantoDaEliminare.validita.fine & "]"

                Dim objControlloCdG As New AgronicaCoreAnagrafeBIZ.Progetto_W
                Dim controlloCdG = objControlloCdG.controllo_CdGxEliminazione(Nothing,
                                                                              appezzamento.primaryKey.centroAziendalePK.partitaIva,
                                                                              appezzamento.primaryKey.centroAziendalePK.codice,
                                                                              appezzamento.primaryKey.codice,
                                                                              id_Reg,
                                                                              0,
                                                                              ObjParametriServer)

                Dim objControlli As New AgronicaCoreAnagrafeBIZ.Reg_Impianto_W
                If objControlli.controllo_MovimentiRicettexEliminazione(Nothing,
                                                                        appezzamento.primaryKey.centroAziendalePK.partitaIva,
                                                                        appezzamento.primaryKey.centroAziendalePK.codice,
                                                                        appezzamento.primaryKey.codice,
                                                                        id_Reg,
                                                                        ObjParametriServer) Then

                    MessaggioErroreAgenda &= MessaggioErroreAgenda & "movimenti agenda e/o ricette " & descrizione_imp & vbCrLf

                    'MessaggioErroreAgenda.Append(String.Format(Gias.ImpossibileEliminareImpiantoXEsistonoOperazioniRegistrate, descrizione_imp) & vbCrLf)

                ElseIf controlloCdG.errore Then


                    MessaggioErroreCdG &= MessaggioErroreCdG & "movimenti cdg " & descrizione_imp & vbCrLf

                    'If Not controlloCdG.messaggioSpecifico Then

                    '    MessaggioErroreCdG.Append(String.Format(Gias.ImpossibileEliminareImpiantoXEsistonoCdGEsercizio, descrizione_imp) & vbCrLf)

                    'Else

                    '    MessaggioErroreCdG.Append(String.Format(Gias.ImpossibileEliminareImpiantoXEsistonoCdGEsercizioInData, descrizione_imp, controlloCdG.dataCdG.ToShortDateString()) & vbCrLf)

                    'End If


                ElseIf objControlli.controllo_UMA_RichiestexEliminazione(appezzamento.primaryKey.centroAziendalePK.partitaIva,
                                                                         appezzamento.primaryKey.centroAziendalePK.codice,
                                                                         appezzamento.primaryKey.codice,
                                                                         id_Reg,
                                                                         ObjParametriServer) Then

                    MessaggioErroreCdG &= MessaggioErroreCdG & "pratiche uma cdg " & descrizione_imp & vbCrLf

                    'MessaggioErroreUMA.Append(String.Format(Gias.ImpossibileEliminareImpiantoXPraticheUMA, descrizione_imp) & vbCrLf)
                End If

            Next

            _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] Verifica requisiti cancellazione appezzamento - MessaggioErroreAgenda : {3}", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, MessaggioErroreAgenda), LogType.Informazione)
            _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] Verifica requisiti cancellazione appezzamento - MessaggioErroreCdG : {3}", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, MessaggioErroreCdG), LogType.Informazione)
            _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] Verifica requisiti cancellazione appezzamento - MessaggioErroreUMA : {3}", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, MessaggioErroreUMA), LogType.Informazione)

            If MessaggioErroreAgenda <> "" OrElse MessaggioErroreCdG <> "" OrElse MessaggioErroreUMA <> "" Then
                Throw New Exception(MessaggioErroreAgenda & MessaggioErroreCdG & MessaggioErroreUMA)
            End If

            ret = True
        Catch ex As Exception
            _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] Verifica requisiti cancellazione appezzamento - Errore: {3}", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, ex.Message), LogType.Errore)
            ret = False
        End Try
        Return ret
    End Function


    Private Shared Sub RicalcolaDateInizioFineValiditaAppezzamento(ByVal cuaa As ObjNotifica(Of CUAAObj),
                                                                  ByRef appezzamento As AgronicaCoreModelsSTD.anagrafiche.Appezzamento,
                                                                   ByVal pcg As Appezzamento,
                                                                   ByRef _logger As LoggerManager)
        ' ricalcolo delle date inizio\fine validità appezzamento
        Dim new_app_start As Date = AGRODATAFINE
        Dim new_app_end As Date = AGRODATAINIZIO
        For Each imp In appezzamento.impianti
            If imp.validita.inizio <= new_app_start Then
                new_app_start = imp.validita.inizio
            End If
            If imp.validita.fine >= new_app_end Then
                new_app_end = imp.validita.fine
            End If
        Next

        _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] Chiavi: CUAA ({0}) - Campagna ({3}) - id_appezzamento_padre ({4}) - id_appezzamento ({5}) - ricalcolo validità appezzamento -> nuove validità {6} - {7} ", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, cuaa.payload.Campagna, pcg.id_appezzamento_padre, pcg.id_appezzamento, new_app_start, new_app_end))

        appezzamento.validita.inizio = new_app_start
        appezzamento.validita.fine = new_app_end
    End Sub

    Private Shared Sub RicalcolaDateInizioFineValiditaAppezzamento(ByVal cuaa As ObjNotifica(Of CUAAObj),
                                                                   ByRef appezzamento As AgronicaCoreModelsSTD.anagrafiche.Appezzamento,
                                                                   ByVal pcg As AppezzamentoSync,
                                                                   ByRef _logger As LoggerManager)
        ' ricalcolo delle date inizio\fine validità appezzamento
        Dim new_app_start As Date = AGRODATAFINE
        Dim new_app_end As Date = AGRODATAINIZIO
        For Each imp In appezzamento.impianti
            If imp.validita.inizio <= new_app_start Then
                new_app_start = imp.validita.inizio
            End If
            If imp.validita.fine >= new_app_end Then
                new_app_end = imp.validita.fine
            End If
        Next

        _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] Chiavi: CUAA ({0}) - Campagna ({3}) - id_appezzamento_padre ({4}) - id_appezzamento ({5}) - ricalcolo validità appezzamento -> nuove validità {5} - {6} ", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, cuaa.payload.Campagna, pcg.id_appezzamento_padre, pcg.id_appezzamento, new_app_start, new_app_end))

        appezzamento.validita.inizio = new_app_start
        appezzamento.validita.fine = new_app_end
    End Sub

    'Private Shared Sub VerificaMetodoProduzioneDaDisciplinare(ByRef appezzamento As AgronicaCoreModelsSTD.anagrafiche.Appezzamento,
    '                                                          ByVal pcg As AppezzamentoSync)
    '    appezzamento.metodo_Produzione = New AgronicaCoreModelsSTD.metaschema.MetodoProduzione(enum_MetodoProduzione.Integrato)
    '    For Each dic In pcg.dichiarazioni
    '        For Each proto In dic.protocolli
    '            If proto.protocollo.Equals("BIO") Then
    '                appezzamento.metodo_Produzione = New AgronicaCoreModelsSTD.metaschema.MetodoProduzione(4)
    '            End If
    '        Next
    '    Next
    'End Sub

    'Private Shared Sub VerificaMetodoProduzioneDaDisciplinare(ByRef appezzamento As AgronicaCoreModelsSTD.anagrafiche.Appezzamento,
    '                                                          ByVal pcg As Appezzamento)

    '    appezzamento.metodo_Produzione = New AgronicaCoreModelsSTD.metaschema.MetodoProduzione(1)
    '    For Each dic In pcg.dichiarazioni
    '        For Each proto In dic.protocolli
    '            If proto.protocollo.Equals("BIO") Then
    '                appezzamento.metodo_Produzione = New AgronicaCoreModelsSTD.metaschema.MetodoProduzione(4)
    '            End If
    '        Next
    '    Next
    'End Sub

    Private Shared Function CheckDisciplinare(ByVal protocolli As List(Of Protocollo),
                                              ByVal Code As String) As Boolean
        Dim ret As Boolean = False
        For Each proto In protocolli
            If proto.protocollo.Equals(Code) Then
                ret = True
                Exit For
            End If
        Next
        Return ret
    End Function

    Private Shared Function GetMetodoProduzione(ByVal dichiarazioni As List(Of Coltura)) As AgronicaCoreModelsSTD.metaschema.MetodoProduzione
        'Lavez - 07/02/2024 - se ho almeno una dichiarazione con disciplinare BIO associato il metodo di produzione dell'appezzamento è Biologico, diversamente integrato
        'Lavez - 11/11/2024 - Fix gestione BIO
        Dim mp As AgronicaCoreModelsSTD.metaschema.MetodoProduzione

        Dim check_bio As Boolean = False
        Dim check_con As Boolean = False
        For Each dic In dichiarazioni
            If CheckDisciplinare(dic.protocolli, "BIO") Then
                check_bio = True
            End If
            If CheckDisciplinare(dic.protocolli, "IN_CONV") Then
                check_con = True
            End If
        Next

        If check_bio Then
            mp = New AgronicaCoreModelsSTD.metaschema.MetodoProduzione(enum_MetodoProduzione.Biologico)
        ElseIf check_con Then
            mp = New AgronicaCoreModelsSTD.metaschema.MetodoProduzione(enum_MetodoProduzione.InConversione)
        Else
            mp = New AgronicaCoreModelsSTD.metaschema.MetodoProduzione(enum_MetodoProduzione.Integrato)
        End If

        Return mp

    End Function

    Private Shared Sub ImpostaVincoloEsercizio(ByRef esercizio As AgronicaCoreModelsSTD.anagrafiche.Esercizio,
                                           ByVal protocolli As List(Of Protocollo),
                                           ByVal VincoliList As List(Of AgronicaCoreModelsSTD.metaschema.Vincolo))
        'lavez - 07/02/2024 - gestione disciplinari Biologico\DPI (by design Disciplinare nazionale riferito alla campagna)
        If protocolli IsNot Nothing AndAlso protocolli.Count > 0 Then
            If CheckDisciplinare(protocolli, "BIO") OrElse CheckDisciplinare(protocolli, "IN_CONV") Then
                'recupero disciplinare nazionale dell'anno della campagna
                Dim vincolo = VincoliList.Where(Function(x) x.descrizione = "Bio").FirstOrDefault()
                If vincolo IsNot Nothing Then
                    'Lavez - 11/11/2024 - Fix assegnazione disciplinare BIO
                    esercizio.vincolo = New AgronicaCoreModelsSTD.metaschema.Vincolo() With {
                        .regolamento = New AgronicaCoreModelsSTD.metaschema.Regolamenti(enum_Cod_Regolamento.Regolamento_bio),
                        .disciplinare = vincolo.disciplinare
                    }
                    esercizio.regolamento = esercizio.vincolo.regolamento
                    esercizio.disciplinare = esercizio.vincolo.disciplinare
                End If
            Else
                If VincoliList IsNot Nothing Then
                    'recupero disciplinare nazionale dell'anno della campagna
                    Dim vincolo = VincoliList.Where(Function(x) x.IdEnte = 20).FirstOrDefault()
                    If vincolo IsNot Nothing Then
                        esercizio.vincolo = New AgronicaCoreModelsSTD.metaschema.Vincolo() With {
                            .regolamento = vincolo.regolamento,
                            .disciplinare = vincolo.disciplinare
                        }
                        esercizio.regolamento = esercizio.vincolo.regolamento
                        esercizio.disciplinare = esercizio.vincolo.disciplinare
                    End If
                End If
            End If
        Else
            esercizio.vincolo = Nothing
            esercizio.regolamento = Nothing
            esercizio.disciplinare = Nothing
        End If
    End Sub

    Private Shared Sub AggiungiNuovoImpianto(ByVal cuaa As ObjNotifica(Of CUAAObj),
                                             ByRef Impianti As List(Of AgronicaCoreModelsSTD.anagrafiche.Impianto),
                                             ByVal AppKey As AgronicaCoreModelsSTD.anagrafiche.Appezzamento.PK,
                                             ByVal pcg As Appezzamento,
                                             ByVal dic As Coltura,
                                             ByVal app_cartography As String,
                                             ByRef _logger As LoggerManager,
                                             ByRef ObjParametriSuperServer As AgronicaCoreParametri,
                                             ByRef ObjParametriServer As AgronicaCoreParametri,
                                             ByRef ObjParametriUtenti As AgronicaCoreParametri,
                                             Optional ByVal VincoliList As List(Of AgronicaCoreModelsSTD.metaschema.Vincolo) = Nothing,
                                             Optional ByVal TagName As String = "",
                                             Optional ByVal TipoConfigurazione As enum_DataPublish_Configurazione = enum_DataPublish_Configurazione.Demetra,
                                             Optional ByVal LogVerbose As Boolean = False)

        Dim xGrfiXVegCod_R As New AgronicaCoreMetaSchemaDAL.GruppoFinalitaxSpecieVegetali_R

        Dim veg_cod As Integer = 0
        Dim id_cod As Integer = 0
        Dim cul_cod As Integer = 0
        Dim grfi_cod As Integer = 0
        Dim Grva_Cod As Integer = 0

        'Lavez - 27/05/2025 - Log verboso
        If LogVerbose Then
            _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] Inizio mappatura impianto creazione", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), LogType.Informazione, bypassElastiSearch:=LogVerbose)
        End If

        DecodificaCodiceAgea(dic.codice_coltura, veg_cod, id_cod, cul_cod, grfi_cod, Grva_Cod, ObjParametriServer)
        'Lavez - 27/05/2025 - Log verboso
        If LogVerbose Then
            _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] dopo decodifica codice agea", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), LogType.Informazione)
        End If

        If veg_cod = 0 AndAlso cul_cod = 0 AndAlso id_cod = 0 Then
            _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] Chiavi: CUAA ({0}) - Campagna ({3}) - id_appezzamento_padre ({4}) - id_appezzamento ({5}) :  Decodifica codice agea ({6}) fallita,acquisizione piano colturale per cuaa\campagna interrotta", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, cuaa.payload.Campagna, pcg.id_appezzamento_padre, pcg.id_appezzamento, dic.codice_coltura), LogType.Errore)
            Throw New DataPublishException("Id_Appezzamento " & pcg.id_appezzamento & " - Decodifica codice agea (" & dic.codice_coltura & ") fallita, acquisizione piano colturale per cuaa\campagna interrotta")
        End If

        If grfi_cod = 0 AndAlso veg_cod <> 0 Then
            grfi_cod = xGrfiXVegCod_R.PrimoGrfiCod_from_Vegcod(veg_cod, ObjParametriServer)
            If LogVerbose Then
                _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] dopo gruppo finalita", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), LogType.Informazione, bypassElastiSearch:=LogVerbose)
            End If
        End If

        Dim impiantoNew As New AgronicaCoreModelsSTD.anagrafiche.Impianto()
        impiantoNew.primaryKey = New AgronicaCoreModelsSTD.anagrafiche.Impianto.PK(0, AppKey)

        impiantoNew.validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(DecodeDate(dic.data_inizio_validita), DecodeDate(dic.data_fine_validita))

        impiantoNew.gruppoVarietale = New AgronicaCoreModelsSTD.metaschema.utilizzi.GruppoVarietale() With {
                                    .codice = Grva_Cod
                                    }

        If veg_cod <> 0 Then
            impiantoNew.utilizzoTerreno = New AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta() With {
                            .classType = NameOf(AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta),
                            .codice = cul_cod,
                            .specie = New AgronicaCoreModelsSTD.metaschema.utilizzi.Specie() With {
                                .codice = veg_cod
                            }
                        }
            impiantoNew.gruppoFinalita = New AgronicaCoreModelsSTD.metaschema.utilizzi.GruppoFinalita() With {
                        .codice = grfi_cod,
                        .specieCod = veg_cod
                    }
        Else
            impiantoNew.utilizzoTerreno = New AgronicaCoreModelsSTD.metaschema.utilizzi.DestinazioneUso(id_cod)
        End If

        impiantoNew.codiceImpianto = pcg.id_appezzamento & "|" & dic.id_dichiarazione
        impiantoNew.superficie = dic.area_mq / 10000


        impiantoNew.cartografia = app_cartography
        impiantoNew.flag_gps = False
        impiantoNew.impianto_Ibrido = True
        impiantoNew.germinabilita = 100
        impiantoNew.data_Inizio_Portinnesto = AGRODATAINIZIO

        impiantoNew.codici = New List(Of AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori)
        impiantoNew.codici.Add(New AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori() With {
                    .codiceAnagrafe = New AgronicaCoreModelsSTD.anagrafiche.CodiceAnagrafe(enum_CodiceAnagrafe_Clienti.Demetra),
                    .validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(),
                    .valore = dic.id_dichiarazione
                })

        impiantoNew.codici.Add(New AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori() With {
                    .codiceAnagrafe = New AgronicaCoreModelsSTD.anagrafiche.CodiceAnagrafe(CODICE_Codice_AGEA_DEMETRA),
                    .validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(),
                    .valore = dic.codice_coltura
                })

        'Lavez - 18/12/2024 - aggiunto codice 1361 (Impianto proveniente da AGEA)
        impiantoNew.codici.Add(New AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori() With {
                                    .codiceAnagrafe = New AgronicaCoreModelsSTD.anagrafiche.CodiceAnagrafe(CODICE_Impianto_AGEA),
                                    .validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(),
                                    .valore = If(dic.is_overturned = True, 1, 0).ToString()
                                })

        'Lavez - 26/03/2025 - aggiunto codice 1362 (Codice campagna)
        If TipoConfigurazione = enum_DataPublish_Configurazione.RegioneUmbria Then
            impiantoNew.codici.Add(New AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori() With {
                        .codiceAnagrafe = New AgronicaCoreModelsSTD.anagrafiche.CodiceAnagrafe(CODICE_Campagna),
                        .validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(),
                        .valore = cuaa.payload.Campagna
                    })
        End If

        'Lavez - 21/03/2024 - aggiunta gestione nuove chiavi agea
        If dic.chiave IsNot Nothing Then
            MapCodiciChiaveAgeaImpianto(impiantoNew, dic.chiave)
        End If

        Dim esercizioNew As New AgronicaCoreModelsSTD.anagrafiche.Esercizio()
        esercizioNew.impiantoPK = impiantoNew.primaryKey

        esercizioNew.superficie = 0
        esercizioNew.prodotto = New AgronicaCoreModelsSTD.attivita.risorse.Prodotto(0)
        esercizioNew.descrizione = "Esercizio " & impiantoNew.codiceImpianto
        esercizioNew.validita = impiantoNew.validita
        esercizioNew.apportiMassimiMacroelementi = New AgronicaCoreModelsSTD.metaschema.ApportoMacroelementi()
        esercizioNew.apportiMassimiMacroelementi.pianoConcimazione = New AgronicaCoreModelsSTD.metaschema.RegolamentoConcimazione()

        'lavez - 07/02/2024 - gestione disciplinari Biologico\DPI (by design Disciplinare nazionale riferito alla campagna)
        ImpostaVincoloEsercizio(esercizioNew, dic.protocolli, VincoliList)

        'Lavez - 11/04/2024 - datiAggiuntiviPCG Agea
        If dic.datiAggiuntiviPCG IsNot Nothing Then
            MapDatiAggiuntiviPCGAgeaImpiantoEsercizio(impiantoNew,
                                                    esercizioNew,
                                                    dic,
                                                    veg_cod,
                                                    cuaa,
                                                    _logger,
                                                    ObjParametriServer,
                                                    LogVerbose)

        End If

        impiantoNew.esercizi = New List(Of AgronicaCoreModelsSTD.anagrafiche.Esercizio)
        impiantoNew.esercizi.Add(esercizioNew)

        Impianti.Add(impiantoNew)
        If LogVerbose Then
            _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] fine mappatura impianto crezione", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), LogType.Informazione, bypassElastiSearch:=LogVerbose)
        End If
    End Sub

    Private Shared Sub ModificaImpiantoEsistente(ByVal cuaa As ObjNotifica(Of CUAAObj),
                                                 ByRef Impianto As AgronicaCoreModelsSTD.anagrafiche.Impianto,
                                                 ByVal pcg As Appezzamento,
                                                 ByVal dic As Coltura,
                                                 ByVal app_cartography As String,
                                                 ByRef _logger As LoggerManager,
                                                 ByRef ObjParametriSuperServer As AgronicaCoreParametri,
                                                 ByRef ObjParametriServer As AgronicaCoreParametri,
                                                 ByRef ObjParametriUtenti As AgronicaCoreParametri,
                                                 Optional ByVal VincoliList As List(Of AgronicaCoreModelsSTD.metaschema.Vincolo) = Nothing,
                                                 Optional ByVal TagName As String = "",
                                                 Optional ByVal TipoConfigurazione As enum_DataPublish_Configurazione = enum_DataPublish_Configurazione.Demetra,
                                                 Optional ByVal LogVerbose As Boolean = False)

        Dim xGrfiXVegCod_R As New AgronicaCoreMetaSchemaDAL.GruppoFinalitaxSpecieVegetali_R

        Dim veg_cod As Integer = 0
        Dim id_cod As Integer = 0
        Dim cul_cod As Integer = 0
        Dim grfi_cod As Integer = 0
        Dim Grva_Cod As Integer = 0

        'Lavez - 27/05/2025 - Log verboso
        If LogVerbose Then
            _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] Inizio mappatura impianto modifica", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), LogType.Informazione, bypassElastiSearch:=LogVerbose)
        End If

        DecodificaCodiceAgea(dic.codice_coltura, veg_cod, id_cod, cul_cod, grfi_cod, Grva_Cod, ObjParametriServer, TagName)
        If LogVerbose Then
            _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] dopo decodifica codice agea", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), LogType.Informazione, bypassElastiSearch:=LogVerbose)
        End If

        If veg_cod = 0 AndAlso cul_cod = 0 AndAlso id_cod = 0 Then
            _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] Chiavi: CUAA ({0}) - Campagna ({3}) - id_appezzamento_padre ({4}) - id_appezzamento ({5}) :  Decodifica codice agea ({6}) fallita,acquisizione piano colturale per cuaa\campagna interrotta", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, cuaa.payload.Campagna, pcg.id_appezzamento_padre, pcg.id_appezzamento, dic.codice_coltura), LogType.Errore)
            Throw New DataPublishException("Id_Appezzamento " & pcg.id_appezzamento & " - Decodifica codice agea (" & dic.codice_coltura & ") fallita, acquisizione piano colturale per cuaa\campagna interrotta")
        End If

        If grfi_cod = 0 AndAlso veg_cod <> 0 Then
            grfi_cod = xGrfiXVegCod_R.PrimoGrfiCod_from_Vegcod(veg_cod, ObjParametriServer)
            If LogVerbose Then
                _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] dopo gruppo finalita", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), LogType.Informazione, bypassElastiSearch:=LogVerbose)
            End If
        End If

        Impianto.validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(DecodeDate(dic.data_inizio_validita), DecodeDate(dic.data_fine_validita))

        Impianto.gruppoVarietale = New AgronicaCoreModelsSTD.metaschema.utilizzi.GruppoVarietale() With {
                                    .codice = Grva_Cod
                                    }

        If veg_cod <> 0 Then
            Impianto.utilizzoTerreno = New AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta() With {
                                .classType = NameOf(AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta),
                                .codice = cul_cod,
                                .specie = New AgronicaCoreModelsSTD.metaschema.utilizzi.Specie() With {
                                    .codice = veg_cod
                                }
                            }
            Impianto.gruppoFinalita = New AgronicaCoreModelsSTD.metaschema.utilizzi.GruppoFinalita() With {
                        .codice = grfi_cod,
                        .specieCod = veg_cod
                    }
        Else
            Impianto.utilizzoTerreno = New AgronicaCoreModelsSTD.metaschema.utilizzi.DestinazioneUso(id_cod)
        End If

        Impianto.codiceImpianto = pcg.id_appezzamento & "|" & dic.id_dichiarazione
        Impianto.superficie = dic.area_mq / 10000


        Impianto.cartografia = app_cartography
        Impianto.flag_gps = False
        Impianto.impianto_Ibrido = True
        Impianto.germinabilita = 100
        Impianto.data_Inizio_Portinnesto = AGRODATAINIZIO

        Impianto.codici = New List(Of AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori)
        Impianto.codici.Add(New AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori() With {
                    .codiceAnagrafe = New AgronicaCoreModelsSTD.anagrafiche.CodiceAnagrafe(enum_CodiceAnagrafe_Clienti.Demetra),
                    .validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(),
                    .valore = dic.id_dichiarazione
                })

        Impianto.codici.Add(New AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori() With {
                    .codiceAnagrafe = New AgronicaCoreModelsSTD.anagrafiche.CodiceAnagrafe(CODICE_Codice_AGEA_DEMETRA),
                    .validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(),
                    .valore = dic.codice_coltura
                })

        'Lavez - 18/12/2024 - aggiunto codice 1361 (Impianto proveniente da AGEA)
        Impianto.codici.Add(New AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori() With {
                                    .codiceAnagrafe = New AgronicaCoreModelsSTD.anagrafiche.CodiceAnagrafe(CODICE_Impianto_AGEA),
                                    .validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(),
                                    .valore = If(dic.is_overturned = True, 1, 0).ToString()
                                })

        'Lavez - 26/03/2025 - aggiunto codice 1362 (Codice campagna)
        If TipoConfigurazione = enum_DataPublish_Configurazione.RegioneUmbria Then
            Impianto.codici.Add(New AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori() With {
                        .codiceAnagrafe = New AgronicaCoreModelsSTD.anagrafiche.CodiceAnagrafe(CODICE_Campagna),
                        .validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(),
                        .valore = cuaa.payload.Campagna
                    })
        End If

        'Lavez - 21/03/2024 - aggiunta gestione nuove chiavi agea
        If dic.chiave IsNot Nothing Then
            MapCodiciChiaveAgeaImpianto(Impianto, dic.chiave)
        End If

        Impianto.esercizi(0).validita = Impianto.validita

        'lavez - 07/02/2024 - gestione disciplinari Biologico\DPI (by design Disciplinare nazionale riferito alla campagna)
        ImpostaVincoloEsercizio(Impianto.esercizi(0), dic.protocolli, VincoliList)

        'Lavez - 11/04/2024 - datiAggiuntiviPCG Agea
        If dic.datiAggiuntiviPCG IsNot Nothing Then
            MapDatiAggiuntiviPCGAgeaImpiantoEsercizio(Impianto,
                                                      Impianto.esercizi(0),
                                                      dic,
                                                      veg_cod,
                                                      cuaa,
                                                      _logger,
                                                      ObjParametriServer,
                                                      LogVerbose)

        End If
        'Lavez - 27/05/2025 - Log verboso
        If LogVerbose Then
            _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] fine mappatura impianto modifica", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), LogType.Informazione, bypassElastiSearch:=LogVerbose)
        End If

    End Sub

    Private Shared Sub AggiungiNuovoImpianto(ByVal cuaa As ObjNotifica(Of CUAAObj),
                                             ByRef Impianti As List(Of AgronicaCoreModelsSTD.anagrafiche.Impianto),
                                             ByVal AppKey As AgronicaCoreModelsSTD.anagrafiche.Appezzamento.PK,
                                             ByVal pcg As AppezzamentoSync,
                                             ByVal dic As Coltura,
                                             ByVal app_cartography As String,
                                             ByRef _logger As LoggerManager,
                                             ByRef ObjParametriSuperServer As AgronicaCoreParametri,
                                             ByRef ObjParametriServer As AgronicaCoreParametri,
                                             ByRef ObjParametriUtenti As AgronicaCoreParametri,
                                             Optional ByVal VincoliList As List(Of AgronicaCoreModelsSTD.metaschema.Vincolo) = Nothing,
                                             Optional ByVal TagName As String = "",
                                             Optional ByVal TipoConfigurazione As enum_DataPublish_Configurazione = enum_DataPublish_Configurazione.Demetra)

        Dim xGrfiXVegCod_R As New AgronicaCoreMetaSchemaDAL.GruppoFinalitaxSpecieVegetali_R

        Dim veg_cod As Integer = 0
        Dim id_cod As Integer = 0
        Dim cul_cod As Integer = 0
        Dim grfi_cod As Integer = 0
        Dim Grva_Cod As Integer = 0

        DecodificaCodiceAgea(dic.codice_coltura, veg_cod, id_cod, cul_cod, grfi_cod, Grva_Cod, ObjParametriServer, TagName)

        If veg_cod = 0 AndAlso cul_cod = 0 AndAlso id_cod = 0 Then
            _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] Chiavi: CUAA ({0}) - Campagna ({3}) - id_appezzamento_padre ({4}) - id_appezzamento ({5}) :  Decodifica codice agea ({6}) fallita,acquisizione piano colturale per cuaa\campagna interrotta", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, pcg.campagna, pcg.id_appezzamento_padre, pcg.id_appezzamento, dic.codice_coltura), LogType.Errore)
            Throw New DataPublishException("Id_Appezzamento " & pcg.id_appezzamento & " - Decodifica codice agea (" & dic.codice_coltura & ") fallita, acquisizione piano colturale per cuaa\campagna interrotta")
        End If

        If grfi_cod = 0 AndAlso veg_cod <> 0 Then
            grfi_cod = xGrfiXVegCod_R.PrimoGrfiCod_from_Vegcod(veg_cod, ObjParametriServer)
        End If

        Dim impiantoNew As New AgronicaCoreModelsSTD.anagrafiche.Impianto()
        impiantoNew.primaryKey = New AgronicaCoreModelsSTD.anagrafiche.Impianto.PK(0, AppKey)

        impiantoNew.validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(DecodeDate(dic.data_inizio_validita), DecodeDate(dic.data_fine_validita))

        impiantoNew.gruppoVarietale = New AgronicaCoreModelsSTD.metaschema.utilizzi.GruppoVarietale() With {
                                    .codice = Grva_Cod
                                    }

        If veg_cod <> 0 Then
            impiantoNew.utilizzoTerreno = New AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta() With {
                            .classType = NameOf(AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta),
                            .codice = cul_cod,
                            .specie = New AgronicaCoreModelsSTD.metaschema.utilizzi.Specie() With {
                                .codice = veg_cod
                            }
                        }
            impiantoNew.gruppoFinalita = New AgronicaCoreModelsSTD.metaschema.utilizzi.GruppoFinalita() With {
                        .codice = grfi_cod,
                        .specieCod = veg_cod
                    }
        Else
            impiantoNew.utilizzoTerreno = New AgronicaCoreModelsSTD.metaschema.utilizzi.DestinazioneUso(id_cod)
        End If

        impiantoNew.codiceImpianto = pcg.id_appezzamento & "|" & dic.id_dichiarazione
        impiantoNew.superficie = dic.area_mq / 10000


        impiantoNew.cartografia = app_cartography
        impiantoNew.flag_gps = False
        impiantoNew.impianto_Ibrido = True
        impiantoNew.germinabilita = 100
        impiantoNew.data_Inizio_Portinnesto = AGRODATAINIZIO

        impiantoNew.codici = New List(Of AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori)
        impiantoNew.codici.Add(New AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori() With {
                    .codiceAnagrafe = New AgronicaCoreModelsSTD.anagrafiche.CodiceAnagrafe(enum_CodiceAnagrafe_Clienti.Demetra),
                    .validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(),
                    .valore = dic.id_dichiarazione
                })

        impiantoNew.codici.Add(New AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori() With {
                    .codiceAnagrafe = New AgronicaCoreModelsSTD.anagrafiche.CodiceAnagrafe(CODICE_Codice_AGEA_DEMETRA),
                    .validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(),
                    .valore = dic.codice_coltura
                })

        'Lavez - 18/12/2024 - aggiunto codice 1361 (Impianto proveniente da AGEA)
        impiantoNew.codici.Add(New AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori() With {
                                    .codiceAnagrafe = New AgronicaCoreModelsSTD.anagrafiche.CodiceAnagrafe(CODICE_Impianto_AGEA),
                                    .validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(),
                                    .valore = If(dic.is_overturned = True, 1, 0).ToString()
                                })

        'Lavez - 26/03/2025 - aggiunto codice 1362 (Codice campagna)
        If TipoConfigurazione = enum_DataPublish_Configurazione.RegioneUmbria Then
            impiantoNew.codici.Add(New AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori() With {
                        .codiceAnagrafe = New AgronicaCoreModelsSTD.anagrafiche.CodiceAnagrafe(CODICE_Campagna),
                        .validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(),
                        .valore = cuaa.payload.Campagna
                    })
        End If

        'Lavez - 21/03/2024 - aggiunta gestione nuove chiavi agea
        If dic.chiave IsNot Nothing Then
            MapCodiciChiaveAgeaImpianto(impiantoNew, dic.chiave)
        End If


        Dim esercizioNew As New AgronicaCoreModelsSTD.anagrafiche.Esercizio()
        esercizioNew.impiantoPK = impiantoNew.primaryKey

        esercizioNew.superficie = 0
        esercizioNew.prodotto = New AgronicaCoreModelsSTD.attivita.risorse.Prodotto(0)
        esercizioNew.descrizione = "Esercizio " & impiantoNew.codiceImpianto
        esercizioNew.validita = impiantoNew.validita
        esercizioNew.apportiMassimiMacroelementi = New AgronicaCoreModelsSTD.metaschema.ApportoMacroelementi()
        esercizioNew.apportiMassimiMacroelementi.pianoConcimazione = New AgronicaCoreModelsSTD.metaschema.RegolamentoConcimazione()

        'lavez - 07/02/2024 - gestione disciplinari Biologico\DPI (by design Disciplinare nazionale riferito alla campagna)
        ImpostaVincoloEsercizio(esercizioNew, dic.protocolli, VincoliList)

        'Lavez - 11/04/2024 - datiAggiuntiviPCG Agea
        If dic.datiAggiuntiviPCG IsNot Nothing Then
            MapDatiAggiuntiviPCGAgeaImpiantoEsercizio(impiantoNew,
                                                      esercizioNew,
                                                      dic,
                                                      veg_cod,
                                                      cuaa,
                                                      _logger,
                                                      ObjParametriServer)

        End If

        impiantoNew.esercizi = New List(Of AgronicaCoreModelsSTD.anagrafiche.Esercizio)
        impiantoNew.esercizi.Add(esercizioNew)

        Impianti.Add(impiantoNew)
    End Sub

    Private Shared Sub ModificaImpiantoEsistente(ByVal cuaa As ObjNotifica(Of CUAAObj),
                                                 ByRef Impianto As AgronicaCoreModelsSTD.anagrafiche.Impianto,
                                                 ByVal pcg As AppezzamentoSync,
                                                 ByVal dic As Coltura,
                                                 ByVal app_cartography As String,
                                                 ByRef _logger As LoggerManager,
                                                 ByRef ObjParametriSuperServer As AgronicaCoreParametri,
                                                 ByRef ObjParametriServer As AgronicaCoreParametri,
                                                 ByRef ObjParametriUtenti As AgronicaCoreParametri,
                                                 Optional ByVal VincoliList As List(Of AgronicaCoreModelsSTD.metaschema.Vincolo) = Nothing,
                                                 Optional ByVal TipoConfigurazione As enum_DataPublish_Configurazione = enum_DataPublish_Configurazione.Demetra)

        Dim xGrfiXVegCod_R As New AgronicaCoreMetaSchemaDAL.GruppoFinalitaxSpecieVegetali_R

        Dim veg_cod As Integer = 0
        Dim id_cod As Integer = 0
        Dim cul_cod As Integer = 0
        Dim grfi_cod As Integer = 0
        Dim Grva_Cod As Integer = 0

        DecodificaCodiceAgea(dic.codice_coltura, veg_cod, id_cod, cul_cod, grfi_cod, Grva_Cod, ObjParametriServer)

        If veg_cod = 0 AndAlso cul_cod = 0 AndAlso id_cod = 0 Then
            _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] Chiavi: CUAA ({0}) - Campagna ({3}) - id_appezzamento_padre ({4}) - id_appezzamento ({5}) :  Decodifica codice agea ({6}) fallita,acquisizione piano colturale per cuaa\campagna interrotta", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, pcg.campagna, pcg.id_appezzamento_padre, pcg.id_appezzamento, dic.codice_coltura), LogType.Errore)
            Throw New DataPublishException("Id_Appezzamento " & pcg.id_appezzamento & " - Decodifica codice agea (" & dic.codice_coltura & ") fallita, acquisizione piano colturale per cuaa\campagna interrotta")
        End If

        If grfi_cod = 0 AndAlso veg_cod <> 0 Then
            grfi_cod = xGrfiXVegCod_R.PrimoGrfiCod_from_Vegcod(veg_cod, ObjParametriServer)
        End If

        Impianto.validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(DecodeDate(dic.data_inizio_validita), DecodeDate(dic.data_fine_validita))

        Impianto.gruppoVarietale = New AgronicaCoreModelsSTD.metaschema.utilizzi.GruppoVarietale() With {
                                    .codice = Grva_Cod
                                    }

        If veg_cod <> 0 Then
            Impianto.utilizzoTerreno = New AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta() With {
                                .classType = NameOf(AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta),
                                .codice = cul_cod,
                                .specie = New AgronicaCoreModelsSTD.metaschema.utilizzi.Specie() With {
                                    .codice = veg_cod
                                }
                            }
            Impianto.gruppoFinalita = New AgronicaCoreModelsSTD.metaschema.utilizzi.GruppoFinalita() With {
                        .codice = grfi_cod,
                        .specieCod = veg_cod
                    }
        Else
            Impianto.utilizzoTerreno = New AgronicaCoreModelsSTD.metaschema.utilizzi.DestinazioneUso(id_cod)
        End If

        Impianto.codiceImpianto = pcg.id_appezzamento & "|" & dic.id_dichiarazione
        Impianto.superficie = dic.area_mq / 10000


        Impianto.cartografia = app_cartography
        Impianto.flag_gps = False
        Impianto.impianto_Ibrido = True
        Impianto.germinabilita = 100
        Impianto.data_Inizio_Portinnesto = AGRODATAINIZIO

        Impianto.codici = New List(Of AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori)
        Impianto.codici.Add(New AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori() With {
                    .codiceAnagrafe = New AgronicaCoreModelsSTD.anagrafiche.CodiceAnagrafe(enum_CodiceAnagrafe_Clienti.Demetra),
                    .validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(),
                    .valore = dic.id_dichiarazione
                })

        Impianto.codici.Add(New AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori() With {
                    .codiceAnagrafe = New AgronicaCoreModelsSTD.anagrafiche.CodiceAnagrafe(CODICE_Codice_AGEA_DEMETRA),
                    .validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(),
                    .valore = dic.codice_coltura
                })

        'Lavez - 18/12/2024 - aggiunto codice 1361 (Impianto proveniente da AGEA)
        Impianto.codici.Add(New AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori() With {
                                    .codiceAnagrafe = New AgronicaCoreModelsSTD.anagrafiche.CodiceAnagrafe(CODICE_Impianto_AGEA),
                                    .validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(),
                                    .valore = If(dic.is_overturned = True, 1, 0).ToString()
                                })

        'Lavez - 26/03/2025 - aggiunto codice 1362 (Codice campagna)
        If TipoConfigurazione = enum_DataPublish_Configurazione.RegioneUmbria Then
            Impianto.codici.Add(New AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori() With {
                        .codiceAnagrafe = New AgronicaCoreModelsSTD.anagrafiche.CodiceAnagrafe(CODICE_Campagna),
                        .validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(),
                        .valore = cuaa.payload.Campagna
                    })
        End If

        'Lavez - 21/03/2024 - aggiunta gestione nuove chiavi agea
        If dic.chiave IsNot Nothing Then
            MapCodiciChiaveAgeaImpianto(Impianto, dic.chiave)
        End If

        Impianto.esercizi(0).validita = Impianto.validita

        'lavez - 07/02/2024 - gestione disciplinari Biologico\DPI (by design Disciplinare nazionale riferito alla campagna)
        ImpostaVincoloEsercizio(Impianto.esercizi(0), dic.protocolli, VincoliList)

        'Lavez - 11/04/2024 - datiAggiuntiviPCG Agea
        If dic.datiAggiuntiviPCG IsNot Nothing Then
            MapDatiAggiuntiviPCGAgeaImpiantoEsercizio(Impianto,
                                                      Impianto.esercizi(0),
                                                      dic,
                                                      veg_cod,
                                                      cuaa,
                                                      _logger,
                                                      ObjParametriServer)

        End If

    End Sub

    Private Shared Sub EliminaAppezzamentoImpianti(ByVal cuaa As ObjNotifica(Of CUAAObj),
                                                   ByRef ret As AgronicaCoreModelsSTD.anagrafiche.Appezzamento,
                                                   ByVal pcg As AppezzamentoSync,
                                                   ByRef _logger As LoggerManager,
                                                   Optional ByVal CancellazioneLogica As Boolean = False)

        'appezzamento + impianto + esercizio (oppure cancellazione primo impianto della coltura pluriennale)
        Try
            'String.Format("[{0} - {1} - {2}] Chiavi: CUAA ({0}) - Campagna ({3}) - id_appezzamento_padre ({4}) - id_appezzamento ({5}) :  Decodifica codice agea ({6}) fallita,acquisizione piano colturale per cuaa\campagna interrotta", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal,
            If ret.impianti.Count > 1 Then
                _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] Chiavi: CUAA ({0}) - Campagna ({3}) - id_appezzamento_padre ({4}) - id_appezzamento ({5}) - coltura pluriennale cancello tutti gli impianti relativi all'appezzamento {5} ", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, pcg.campagna, pcg.id_appezzamento_padre, pcg.id_appezzamento))
                Dim impiantiList = ret.impianti.Where(Function(x) x.codiceImpianto.StartsWith(pcg.id_appezzamento & "|"))
                If impiantiList IsNot Nothing Then
                    For Each impianto In impiantiList.ToList()
                        If CancellazioneLogica Then
                            _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] ELIMINAZIONE LOGICA ATTIVA Chiavi: CUAA ({0}) - Campagna ({3}) - id_appezzamento_padre ({4}) - id_appezzamento ({5}) - elimino impianto {6} ", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, pcg.campagna, pcg.id_appezzamento_padre, pcg.id_appezzamento, impianto.codiceImpianto))
                            ret.impianti.Where(Function(x) x.codiceImpianto = impianto.codiceImpianto).FirstOrDefault().flagCessata = "1"
                        Else
                            _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] Chiavi: CUAA ({0}) - Campagna ({3}) - id_appezzamento_padre ({4}) - id_appezzamento ({5}) - elimino impianto {6} ", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, pcg.campagna, pcg.id_appezzamento_padre, pcg.id_appezzamento, impianto.codiceImpianto))

                            ret.impianti.Remove(impianto)
                        End If

                    Next
                End If
            Else
                If CancellazioneLogica Then
                    _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] ELIMINAZIONE LOGICA ATTIVA Chiavi: CUAA ({0}) - Campagna ({3}) - id_appezzamento_padre ({4}) - id_appezzamento ({5}) - cancellazione intero appezzamento ", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, pcg.campagna, pcg.id_appezzamento_padre, pcg.id_appezzamento))
                    For Each imp In ret.impianti
                        _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] ELIMINAZIONE LOGICA ATTIVA Chiavi: CUAA ({0}) - Campagna ({3}) - id_appezzamento_padre ({4}) - id_appezzamento ({5}) - cancellazione impianto {6} ", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, pcg.campagna, pcg.id_appezzamento_padre, pcg.id_appezzamento, imp.codiceImpianto))
                        imp.flagCessata = "1"
                    Next
                Else
                    _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] Chiavi: CUAA ({0}) - Campagna ({3}) - id_appezzamento_padre ({4}) - id_appezzamento ({5}) - cancellazione intero appezzamento ", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, pcg.campagna, pcg.id_appezzamento_padre, pcg.id_appezzamento))
                    ret.flag_cancellazione = True
                End If

            End If

        Catch ex As Exception
            _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] Chiavi: CUAA ({0}) - Campagna ({3}) - id_appezzamento_padre ({4}) - id_appezzamento ({5}) : Record non trovato. impossibile eseguire la cancellazione", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, pcg.campagna, pcg.id_appezzamento_padre, pcg.id_appezzamento), LogType.Errore)
            ret = Nothing
        End Try

    End Sub

    Private Shared Sub EliminaImpiantiCampagnaCorrente(ByVal cuaa As ObjNotifica(Of CUAAObj),
                                                       ByRef ret As AgronicaCoreModelsSTD.anagrafiche.Appezzamento,
                                                       ByVal pcg As AppezzamentoSync,
                                                       ByRef _logger As LoggerManager,
                                                       Optional ByVal CancellazioneLogica As Boolean = False)
        Try
            If ret.impianti.Count > 1 Then
                _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] Chiavi: CUAA ({0}) - Campagna ({3}) - id_appezzamento_padre ({4}) - id_appezzamento ({5}) - coltura pluriennale cancello tutti gli impianti relativi all'appezzamento {6} ", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, pcg.campagna, pcg.id_appezzamento_padre, pcg.id_appezzamento))
                Dim impiantiList = ret.impianti.Where(Function(x) x.codiceImpianto.StartsWith(pcg.id_appezzamento & "|"))
                If impiantiList IsNot Nothing Then
                    For Each impianto In impiantiList.ToList()
                        If impianto.validita.fine.Year = pcg.campagna Then
                            If CancellazioneLogica Then
                                _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] ELIMINAZIONE LOGICA ATTIVA Chiavi: CUAA ({0}) - Campagna ({3}) - id_appezzamento_padre ({4}) - id_appezzamento ({5}) - elimino impianto {6} ", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, pcg.campagna, pcg.id_appezzamento_padre, pcg.id_appezzamento, impianto.codiceImpianto))
                                ret.impianti.Where(Function(x) x.codiceImpianto = impianto.codiceImpianto).FirstOrDefault().flagCessata = "1"
                            Else
                                _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] Chiavi: CUAA ({0}) - Campagna ({3}) - id_appezzamento_padre ({4}) - id_appezzamento ({5}) - elimino impianto {6} ", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, pcg.campagna, pcg.id_appezzamento_padre, pcg.id_appezzamento, impianto.codiceImpianto))
                                ret.impianti.Remove(impianto)
                            End If

                        End If
                    Next
                End If
            End If
        Catch ex As Exception
            _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] Chiavi: CUAA ({0}) - Campagna ({3}) - id_appezzamento_padre ({4}) - id_appezzamento ({5}) : Record non trovato. impossibile eseguire la cancellazione", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, pcg.campagna, pcg.id_appezzamento_padre, pcg.id_appezzamento), LogType.Errore)
            ret = Nothing
        End Try
    End Sub

    Private Shared Sub EliminaImpiantiNonPresenti(ByVal cuaa As ObjNotifica(Of CUAAObj),
                                                  ByRef ret As AgronicaCoreModelsSTD.anagrafiche.Appezzamento,
                                                   ByVal pcg As AppezzamentoSync,
                                                   ByRef _logger As LoggerManager,
                                                  Optional ByVal CancellazioneLogica As Boolean = False)
        Try
            For Each dic In pcg.dichiarazioni
                Dim codiceImpiato As String = pcg.id_appezzamento & "|" & dic.id_dichiarazione
                Dim imp = ret.impianti.Where(Function(x) x.codiceImpianto = codiceImpiato).FirstOrDefault()
                If imp IsNot Nothing Then
                    If CancellazioneLogica Then
                        _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] ELIMINAZIONE LOGICA ATTIVA Chiavi: CUAA ({0}) - Campagna ({3}) - id_appezzamento_padre ({4}) - id_appezzamento ({5}) - elimino impianto {6} ", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, pcg.campagna, pcg.id_appezzamento_padre, pcg.id_appezzamento, imp.codiceImpianto))
                        ret.impianti.Where(Function(x) x.codiceImpianto = imp.codiceImpianto).FirstOrDefault().flagCessata = "1"
                    Else
                        _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] Chiavi: CUAA ({0}) - Campagna ({3}) - id_appezzamento_padre ({4}) - id_appezzamento ({5}) - elimino impianto {6} ", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, pcg.campagna, pcg.id_appezzamento_padre, pcg.id_appezzamento, imp.codiceImpianto))
                        ret.impianti.Remove(imp)
                    End If

                End If
            Next

        Catch ex As Exception
            _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] Chiavi: CUAA ({0}) - Campagna ({3}) - id_appezzamento_padre ({4}) - id_appezzamento ({5}) : Record non trovato. impossibile eseguire la cancellazione", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, pcg.campagna, pcg.id_appezzamento_padre, pcg.id_appezzamento), LogType.Errore)
            ret = Nothing
        End Try
    End Sub

    Private Shared Sub EliminaImpiantiNonPresentiInDemetra(ByVal cuaa As ObjNotifica(Of CUAAObj),
                                                           ByVal pcg As AppezzamentoSync,
                                                           ByRef ret As AgronicaCoreModelsSTD.anagrafiche.Appezzamento,
                                                           ByRef _logger As LoggerManager)

        _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] Chiavi: CUAA ({0}) - Campagna ({3}) - id_appezzamento_padre ({4}) - id_appezzamento ({5}): pulizia avvicendamenti non più presenti in demetra", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, pcg.campagna, pcg.id_appezzamento_padre, pcg.id_appezzamento), bypassElastiSearch:=True)

        'pre elaborazione elimino tutti gli impianti e le distinte che non sono riportate nelle dichiarazioni (vedi caso di creazione avvicendamento e successiva cancellazione)
        Dim ElencoCodiciImpianto As New List(Of String)

        For Each dic In pcg.dichiarazioni
            _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] Chiavi: CUAA ({0}) - Campagna ({3}) - id_appezzamento_padre ({4}) - id_appezzamento ({5}) - codice impianto demetra ({6})", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, pcg.campagna, pcg.id_appezzamento_padre, pcg.id_appezzamento, pcg.id_appezzamento & "|" & dic.id_dichiarazione))
            ElencoCodiciImpianto.Add(pcg.id_appezzamento & "|" & dic.id_dichiarazione)
        Next

        Dim ImpiantiList As New List(Of AgronicaCoreModelsSTD.anagrafiche.Impianto)

        For Each imp In ret.impianti
            _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] Chiavi: CUAA ({0}) - Campagna ({3}) - id_appezzamento_padre ({4}) - id_appezzamento ({5}) - codice impianto gias ({6})", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, pcg.campagna, pcg.id_appezzamento_padre, pcg.id_appezzamento, imp.codiceImpianto))
            ImpiantiList.Add(JsonConvert.DeserializeObject(Of AgronicaCoreModelsSTD.anagrafiche.Impianto)(JsonConvert.SerializeObject(imp)))
        Next

        For Each impianto In ImpiantiList
            If Not ElencoCodiciImpianto.Contains(impianto.codiceImpianto) AndAlso impianto.validita.fine.Year = pcg.campagna Then
                'prendo in considerazione solo gli impianti relativi al periodo di validità dell'appezzamento (su demetra) che viene aggiornato
                'perché demetra tratta il piano colturale per campagna (annuale) mentre su gias abbiamo una piramide con rapporto 1-n per ogni livello di entità coinvolte (appezzamento\impianto\esercizio)
                If impianto.validita.inizio >= DecodeDate(pcg.data_inizio_validita) AndAlso impianto.validita.fine <= DecodeDate(pcg.data_fine_validita) Then

                    Dim tmpRemove = ret.impianti.Where(Function(x) x.codiceImpianto = impianto.codiceImpianto).First()
                    If tmpRemove IsNot Nothing Then
                        If ret.impianti.IndexOf(tmpRemove) >= 0 Then
                            _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] Chiavi: CUAA ({0}) - Campagna ({3}) - id_appezzamento_padre ({4}) - id_appezzamento ({5}) - codice impianto ({6}): avvicendamento non presente in DEMETRA -> elimino l'impianto su Agronica", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, pcg.campagna, pcg.id_appezzamento_padre, pcg.id_appezzamento, impianto.codiceImpianto))
                            ret.impianti.RemoveAt(ret.impianti.IndexOf(tmpRemove))
                        End If
                    End If
                End If
            End If
        Next
    End Sub

    Public Shared Function MapUserToAgronicaUtente(ByVal cuaa As ObjNotifica(Of CUAAObj),
                                                   ByVal datiColdiretti As UtenteColdiretti,
                                                   ByVal username As String,
                                                   ByVal Piva As String,
                                                   ByVal profilo_default As String,
                                                   ByRef ObjParametriServer As AgronicaCoreParametri,
                                                   ByRef _logger As LoggerManager
                                                   ) As AgronicaCoreModelsSTD.profilazione.LeggiScriviVisibilitaUtente
        Dim result As AgronicaCoreModelsSTD.profilazione.LeggiScriviVisibilitaUtente = Nothing
        Dim ret As AgronicaCoreModelsSTD.profilazione.Utente = Nothing
        Try
            result = New AgronicaCoreModelsSTD.profilazione.LeggiScriviVisibilitaUtente
            result.Utente = New AgronicaCoreModelsSTD.profilazione.UtenteDTO()
            result.Utente.Tipologia = New AgronicaCoreModelsSTD.profilazione.TipologiaUtente() With {
                        .codice = profilo_default
                    }

            result.Utente.Piva_SuperUser = ObjParametriServer.PivaSuperUser
            If datiColdiretti.COD_TIPO_ANAG = "G" AndAlso datiColdiretti.data.SGL_CODFIS.Length = 16 Then
                result.Utente.UserName = username
                result.Utente.Nome = datiColdiretti.data.titolare.DES_NOME
                result.Utente.Cognome = datiColdiretti.data.titolare.DES_COGNOME
                result.Utente.Rag_Soc = ""
                result.Utente.codice_fiscale = datiColdiretti.data.SGL_CODFIS
                result.Utente.piva = ""
                result.Utente.flag_azienda_persona = 2
            Else
                result.Utente.UserName = username
                result.Utente.Nome = ""
                result.Utente.Cognome = ""
                result.Utente.Rag_Soc = datiColdiretti.data.DES_RAGIONE_SOCIALE
                result.Utente.codice_fiscale = datiColdiretti.data.SGL_CODFIS
                result.Utente.piva = ""
                result.Utente.flag_azienda_persona = 1
            End If

            result.Utente.username_commerciale = "UT.PDS.DEMETRA"
            result.Utente.Email = ""
            result.Utente.Password = ""

            result.AziendeVisibili = New List(Of ImpresaDto)
            result.AziendeVisibili.Add(New ImpresaDto() With {
                                        .piva = Piva,
                                        .Sa_Cod = 0,
                                        .Sa_Nome = ""
                                       })


        Catch ex As Exception
            result = Nothing
            _logger.AppendLog(cuaa, "user", ex.Message, LogType.Errore, ex)
            Throw New Exception(ex.Message, ex)
        End Try
        Return result
    End Function

    Public Shared Function MapUserToAgronicaUtenteRequest(ByVal cuaa As ObjNotifica(Of CUAAObj),
                                                          ByVal datiColdiretti As UtenteColdiretti,
                                                          ByVal Username As String,
                                                           ByVal Pass As String,
                                                           ByVal Piva As String,
                                                           ByVal profilo_default As String,
                                                           ByRef ObjParametriServer As AgronicaCoreParametri,
                                                           ByRef _logger As LoggerManager
                                                           ) As AgronicaCoreModelsSTD.profilazione.Utente
        Dim ret As AgronicaCoreModelsSTD.profilazione.Utente = Nothing
        Try
            ret = New AgronicaCoreModelsSTD.profilazione.Utente


            If datiColdiretti.COD_TIPO_ANAG = "G" AndAlso datiColdiretti.data.SGL_CODFIS.Length = 16 Then
                ret.UserName = Username
                ret.CodFisc = datiColdiretti.data.titolare.SGL_CODFIS
                ret.Rag_Soc = ""
                ret.Cognome = datiColdiretti.data.titolare.DES_COGNOME
                ret.Nome = datiColdiretti.data.titolare.DES_NOME
                ret.Azienda_Persona = "P"
            Else
                ret.UserName = Username
                ret.CodFisc = datiColdiretti.data.SGL_CODFIS
                ret.Rag_Soc = datiColdiretti.data.DES_RAGIONE_SOCIALE
                ret.Cognome = ""
                ret.Nome = ""
                ret.Azienda_Persona = "I"
            End If

            ret.Password = Pass
            ret.Data_Creazione = Date.Now
            ret.Email = ""
            ret.Tipologia_Cod = profilo_default

        Catch ex As Exception
            ret = Nothing
            _logger.AppendLog(cuaa, "user", ex.Message, LogType.Errore, ex)
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Public Shared Function MapEquipaggiamentoToMacchina(ByVal cuaa As ObjNotifica(Of CUAAObj),
                                                        ByVal CentroPK As AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK,
                                                        ByVal Equip As AnagraficaWrapper(Of Equipaggiamento),
                                                        ByVal Mac_Cod As Integer,
                                                        ByVal ContattoPK As AgronicaCoreModelsSTD.anagrafiche.Contatto.PK,
                                                        ByRef ObjParametriServer As AgronicaCoreParametri,
                                                        ByRef _logger As LoggerManager) As Dictionary(Of String, AgronicaCoreModelsSTD.anagrafiche.ParcoMacchine)
        Dim ret As Dictionary(Of String, AgronicaCoreModelsSTD.anagrafiche.ParcoMacchine) = Nothing
        Try
            Dim xAgeaMacchine As New AgronicaCoreMetaSchemaDAL.Codifica_Macchine_Agea
            Dim class_code = xAgeaMacchine.Class_Cod_da_AGEA_Cod(ObjParametriServer, Equip.elemento_anagrafico.tipo, "")

            If class_code = "" Then
                Throw New GiasException("Class code macchina nullo. impossibile proseguire")
            End If

            Dim tipo = "", dettaglio_1 = "", dettaglio_2 = ""
            DecodeClassCode(class_code, tipo, dettaglio_1, dettaglio_2)

            'lavez - 31/07/2024 - default macchina pubbliche
            CentroPK.codice = 0

            ret = New Dictionary(Of String, AgronicaCoreModelsSTD.anagrafiche.ParcoMacchine)

            ret.Add(Equip.codice, New ParcoMacchine With {
                .codice = Mac_Cod,
                .partitaIva = CentroPK.partitaIva,
                .centroPK = CentroPK,
                .CUAA_Proprietario = cuaa.payload.CUAA,
                .visibilitaPubblica = False,
                .targa = IIf(Equip.elemento_anagrafico.targa Is Nothing, "", Equip.elemento_anagrafico.targa),
                .telaio = IIf(Equip.elemento_anagrafico.telaio Is Nothing, "", Equip.elemento_anagrafico.telaio),
                .modello = IIf(Equip.elemento_anagrafico.modello Is Nothing, "", Equip.elemento_anagrafico.modello),
                .data_Ultima_Taratura = IIf(Equip.elemento_anagrafico.data_ultima_taratura Is Nothing, AGRODATAINIZIO, Equip.elemento_anagrafico.data_ultima_taratura),
                .descrizione = IIf(Equip.elemento_anagrafico.descrizione Is Nothing, "", Equip.elemento_anagrafico.descrizione),
                .scadenza_Taratura = IIf(Equip.elemento_anagrafico.scadenza_taratura Is Nothing, AGRODATAINIZIO, Equip.elemento_anagrafico.scadenza_taratura),
                .tipo = New AgronicaCoreModelsSTD.metaschema.Macchine(tipo),
                .dettaglio_1 = New AgronicaCoreModelsSTD.metaschema.MacchineDettaglio1(dettaglio_1),
                .dettaglio_2 = New AgronicaCoreModelsSTD.metaschema.MacchineDettaglio2(dettaglio_2),
                .validita = New IntervalloTemporale(Helper.CheckDate(Equip.elemento_anagrafico.validita.inizio), Helper.CheckDate(Equip.elemento_anagrafico.validita.fine)),
                .alimentazione = New AgronicaCoreModelsSTD.metaschema.Carburante(IIf(Equip.elemento_anagrafico.alimentazione Is Nothing OrElse Equip.elemento_anagrafico.alimentazione = "", 0, CInt(Equip.elemento_anagrafico.alimentazione))),
                .ageaCod = New AgronicaCoreModelsSTD.metaschema.MacchineCodificaAgea(Equip.elemento_anagrafico.tipo),
                .numero_certificato = IIf(Equip.elemento_anagrafico.nr_certificato Is Nothing, "", Equip.elemento_anagrafico.nr_certificato),
                .contatto = If(ContattoPK Is Nothing, Nothing, New AgronicaCoreModelsSTD.anagrafiche.Contatto() With {.primaryKey = ContattoPK})
                })
        Catch ex As GiasException
            ret = Nothing
            _logger.AppendLog(cuaa, "equipaggiamenti", ex.Message, LogType.Errore, ex)
            Throw New GiasException(ex.Message, ex)
        Catch ex As Exception
            ret = Nothing
            _logger.AppendLog(cuaa, "equipaggiamenti", ex.Message, LogType.Errore, ex)
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Public Shared Function MapEquipaggiamentoToMacchina(ByVal cuaa As ObjNotifica(Of CUAAObj),
                                                        ByVal CentroPK As AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK,
                                                        ByVal Equip As EquipaggiamentoCUAASync,
                                                        ByVal ParcoMacchine As ParcoMacchine,
                                                        ByRef ObjParametriServer As AgronicaCoreParametri,
                                                        ByRef _logger As LoggerManager) As Dictionary(Of String, AgronicaCoreModelsSTD.anagrafiche.ParcoMacchine)
        Dim ret As Dictionary(Of String, AgronicaCoreModelsSTD.anagrafiche.ParcoMacchine) = Nothing
        Try
            Dim xAgeaMacchine As New AgronicaCoreMetaSchemaDAL.Codifica_Macchine_Agea
            Dim class_code = xAgeaMacchine.Class_Cod_da_AGEA_Cod(ObjParametriServer, Equip.elemento_anagrafico.tipo, "")

            If class_code = "" Then
                Throw New GiasException("Class code macchina nullo. impossibile proseguire")
            End If

            Dim tipo = "", dettaglio_1 = "", dettaglio_2 = ""
            DecodeClassCode(class_code, tipo, dettaglio_1, dettaglio_2)

            'lavez - 31/07/2024 - default macchina pubbliche
            CentroPK.codice = 0

            ret = New Dictionary(Of String, AgronicaCoreModelsSTD.anagrafiche.ParcoMacchine)

            Dim Mac_Cod = 0
            Dim ContattoPK As AgronicaCoreModelsSTD.anagrafiche.Contatto.PK = Nothing
            If ParcoMacchine IsNot Nothing Then
                Mac_Cod = ParcoMacchine.codice
                If ParcoMacchine.contatto IsNot Nothing Then
                    ContattoPK = ParcoMacchine.contatto.primaryKey
                End If
            End If

            ret.Add(Equip.codice, New ParcoMacchine With {
                .codice = Mac_Cod,
                .partitaIva = CentroPK.partitaIva,
                .centroPK = CentroPK,
                .CUAA_Proprietario = cuaa.payload.CUAA,
                .visibilitaPubblica = False,
                .alimentazione = New AgronicaCoreModelsSTD.metaschema.Carburante(IIf(Equip.elemento_anagrafico.alimentazione Is Nothing OrElse Equip.elemento_anagrafico.alimentazione = "", 0, Equip.elemento_anagrafico.alimentazione)),
                .targa = IIf(Equip.elemento_anagrafico.targa Is Nothing, "", Equip.elemento_anagrafico.targa),
                .telaio = IIf(Equip.elemento_anagrafico.telaio Is Nothing, "", Equip.elemento_anagrafico.telaio),
                .modello = IIf(Equip.elemento_anagrafico.modello Is Nothing, "", Equip.elemento_anagrafico.modello),
                .data_Ultima_Taratura = IIf(Equip.elemento_anagrafico.data_ultima_taratura Is Nothing, AGRODATAINIZIO, Equip.elemento_anagrafico.data_ultima_taratura),
                .descrizione = IIf(Equip.elemento_anagrafico.descrizione Is Nothing, "", Equip.elemento_anagrafico.descrizione),
                .scadenza_Taratura = IIf(Equip.elemento_anagrafico.scadenza_taratura Is Nothing, AGRODATAINIZIO, Equip.elemento_anagrafico.scadenza_taratura),
                .tipo = New AgronicaCoreModelsSTD.metaschema.Macchine(tipo),
                .dettaglio_1 = New AgronicaCoreModelsSTD.metaschema.MacchineDettaglio1(dettaglio_1),
                .dettaglio_2 = New AgronicaCoreModelsSTD.metaschema.MacchineDettaglio2(dettaglio_2),
                .validita = New IntervalloTemporale(Helper.CheckDate(Equip.elemento_anagrafico.validita.inizio), Helper.CheckDate(Equip.elemento_anagrafico.validita.fine)),
                .ageaCod = New AgronicaCoreModelsSTD.metaschema.MacchineCodificaAgea(Equip.elemento_anagrafico.tipo),
                .numero_certificato = IIf(Equip.elemento_anagrafico.nr_certificato Is Nothing, "", Equip.elemento_anagrafico.nr_certificato),
                .contatto = If(ContattoPK Is Nothing, Nothing, New AgronicaCoreModelsSTD.anagrafiche.Contatto() With {.primaryKey = ContattoPK})
                })
        Catch ex As GiasException
            ret = Nothing
            _logger.AppendLog(cuaa, "equipaggiamenti", ex.Message, LogType.Errore, ex)
            Throw New GiasException(ex.Message, ex)
        Catch ex As Exception
            ret = Nothing
            _logger.AppendLog(cuaa, "equipaggiamenti", ex.Message, LogType.Errore, ex)
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Private Shared Sub DecodeClassCode(ByVal class_code As String, ByRef tipo As String, ByRef dettaglio1 As String, ByRef dettaglio2 As String)

        Try
            Dim cod = Split(class_code, ".")
            tipo = cod(0)
            If cod.Length > 1 Then
                dettaglio1 = cod(1)
            Else
                dettaglio1 = ""
            End If
            If cod.Length > 2 Then
                dettaglio2 = cod(2)
            Else
                dettaglio2 = ""
            End If
        Catch ex As Exception
            tipo = ""
            dettaglio1 = ""
            dettaglio2 = ""
            Throw New Exception(ex.Message, ex)
        End Try
    End Sub

    Public Shared Function GetUsernameFromUserColdiretti(ByVal cuaa As ObjNotifica(Of CUAAObj),
                                                         ByVal datiColdiretti As UtenteColdiretti,
                                                         ByRef _logger As LoggerManager) As String
        Dim ret As String = ""
        Try
            If datiColdiretti.COD_TIPO_ANAG = "G" AndAlso datiColdiretti.data.SGL_CODFIS.Length = 16 Then
                If datiColdiretti.data.titolare Is Nothing Then
                    Throw New ColdirettiPDSException(String.Format("Anagrafica coldiretti di tipo G con codice fiscale {0} ma senza dati titolare. impossibile creare l'utente", datiColdiretti.data.SGL_CODFIS))
                End If
                ret = datiColdiretti.data.titolare.COD_ANAGEN
            Else
                ret = datiColdiretti.data.COD_ANAGEN
            End If
        Catch ex As ColdirettiPDSException
            _logger.AppendLog(cuaa, "user", ex.Message, LogType.Errore, ex)
        Catch ex As Exception
            _logger.AppendLog(cuaa, "user", ex.Message, LogType.Errore, ex)
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Public Shared Function GeneraPraticaRequest(ByVal cuaa As ObjNotifica(Of CUAAObj),
                                                ByVal piva As String,
                                                ByVal Campagna As Integer,
                                                ByVal id_servizio As Integer,
                                                ByRef _logger As LoggerManager) As AgronicaCoreDTOStd.InData.Pratiche.GeneraPratica
        Dim ret As AgronicaCoreDTOStd.InData.Pratiche.GeneraPratica = Nothing
        Try
            ret = New AgronicaCoreDTOStd.InData.Pratiche.GeneraPratica() With {
                    .piva = piva,
                    .servizio_cod = id_servizio,
                    .Data_Inizio = New Date(Campagna, 1, 1),
                    .Data_Fine = New Date(Campagna, 12, 31),
                    .Data_Inizio_Pratica = IIf(Date.Now > New Date(Campagna, 12, 31), New Date(Campagna, 12, 31), Date.Now),
                    .Numero = ""
                }
        Catch ex As Exception
            ret = Nothing
            _logger.AppendLog(cuaa, "workflow", ex.Message, LogType.Errore, ex)
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Public Shared Function AvanzamentoStatoPraticaRequest(ByVal cuaa As ObjNotifica(Of CUAAObj),
                                                          ByVal piva As String,
                                                          ByVal pratica_cod As Integer,
                                                          ByVal id_servizio As Integer,
                                                          ByVal newState As Integer,
                                                          ByRef _logger As LoggerManager) As AgronicaCoreDTOStd.InData.Pratiche.Pratica_AvanzaStato
        Dim ret As AgronicaCoreDTOStd.InData.Pratiche.Pratica_AvanzaStato = Nothing
        Try
            ret = New AgronicaCoreDTOStd.InData.Pratiche.Pratica_AvanzaStato() With {
                    .piva = piva,
                    .servizio_cod = id_servizio,
                    .pratica_cod = pratica_cod,
                    .statoFinaleRichiesto = newState,
                    .note = ""
                }
        Catch ex As Exception
            ret = Nothing
            _logger.AppendLog(cuaa, "workflow", ex.Message, LogType.Errore, ex)
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Public Shared Function ControlliAnagrafica(ByVal cuaa As ObjNotifica(Of AgronicaCoreDTOStd.InData.Notifiche.CUAAObj),
                                               ByVal anag As Anagrafica,
                                               ByRef _logger As LoggerManager,
                                               ByRef EsitoSync As SyncAcknowledge,
                                               Optional ByVal TipoConfigurazione As enum_DataPublish_Configurazione = enum_DataPublish_Configurazione.Demetra) As Boolean
        Dim ret As Boolean = True
        Try
            If TipoConfigurazione = enum_DataPublish_Configurazione.Demetra Then
                If anag.mandato Is Nothing OrElse anag.mandato.codice_detentore = "" Then
                    _logger.AppendLog(cuaa, "anagrafica", String.Format("[{0} - {1} - {2}] Codice detentore non valorizzato", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), LogType.Errore)
                    EsitoSync.errors.Add(New SyncErrors() With {.code = "001", .msg = "Codice detentore non valorizzato", .key = anag.cuaa})
                    ret = False
                End If
            End If
            'Lavez - 15/09/2025 - Disabilitato per inserimento forzatura CF = CUAA nel caso in cui piva e cf siano entrambi vuoti
            'If anag.codice_fiscale = "" AndAlso anag.partita_iva = "" Then
            '    _logger.AppendLog(cuaa, "anagrafica", String.Format("[{0} - {1} - {2}] Anagrafica senza codice fiscale o partita iva", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), LogType.Errore)
            '    EsitoSync.errors.Add(New SyncErrors() With {.code = "002", .msg = "Anagrafica senza codice fiscale o partita iva", .key = anag.cuaa})
            '    ret = False
            'End If
        Catch ex As Exception
            ret = False
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Public Shared Function ControlliCatasto(ByVal CUAA As ObjNotifica(Of AgronicaCoreDTOStd.InData.Notifiche.CUAAObj),
                                            ByVal catasto As Terreni,
                                            ByRef _logger As LoggerManager,
                                               ByRef EsitoSync As SyncAcknowledge) As Boolean
        Dim ret As Boolean = True
        Try
            Dim cast_int As Integer
            For Each particella In catasto.records
                If Integer.TryParse(particella.foglio, cast_int) = False Then
                    _logger.AppendLog(CUAA, "terreni", String.Format("[{0} - {1} - {2}]  campo foglio non numerico ({3})", CUAA.payload.CUAA, CUAA.payload.Operazione, CUAA.payload.id_signal, particella.GetKey()), LogType.Errore)
                    EsitoSync.errors.Add(New SyncErrors() With {.code = "104", .msg = "campo foglio non numerico", .key = particella.GetKey()})
                    ret = False
                End If
                If Integer.TryParse(particella.particella, cast_int) = False Then
                    _logger.AppendLog(CUAA, "terreni", String.Format("[{0} - {1} - {2}]  campo particella non numerico ({3})", CUAA.payload.CUAA, CUAA.payload.Operazione, CUAA.payload.id_signal, particella.GetKey()), LogType.Errore)
                    EsitoSync.errors.Add(New SyncErrors() With {.code = "105", .msg = "campo particella non numerico", .key = particella.GetKey()})
                    ret = False
                End If

                If particella.area_condotta_mq = 0 OrElse particella.area_condotta_mq Is Nothing Then
                    _logger.AppendLog(CUAA, "terreni", String.Format("[{0} - {1} - {2}]  Particella catastale senza superfiche di conduzione ({3})", CUAA.payload.CUAA, CUAA.payload.Operazione, CUAA.payload.id_signal, particella.GetKey()), LogType.Errore)
                    EsitoSync.errors.Add(New SyncErrors() With {.code = "101", .msg = "Particella catastale senza superfiche di conduzione", .key = particella.GetKey()})
                    ret = False
                End If
                If particella.sezione Is Nothing Then
                    _logger.AppendLog(CUAA, "terreni", String.Format("[{0} - {1} - {2}]  Particella catastale con campo sezione non valorizzato ({3})", CUAA.payload.CUAA, CUAA.payload.Operazione, CUAA.payload.id_signal, particella.GetKey()), LogType.Errore)
                    EsitoSync.errors.Add(New SyncErrors() With {.code = "102", .msg = "Particella catastale con campo sezione non valorizzato", .key = particella.GetKey()})
                    ret = False
                Else
                    If particella.sezione.Length > 2 Then
                        _logger.AppendLog(CUAA, "terreni", String.Format("[{0} - {1} - {2}]  Particella catastale con campo sezione più lungo di due caratteri ({3})", CUAA.payload.CUAA, CUAA.payload.Operazione, CUAA.payload.id_signal, particella.GetKey()), LogType.Errore)
                        EsitoSync.errors.Add(New SyncErrors() With {.code = "103", .msg = "Particella catastale con campo sezione più lungo di due caratteri", .key = particella.GetKey()})
                        ret = False
                    End If
                End If

            Next
        Catch ex As Exception
            ret = False
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Public Shared Function ControlliVariazioniCatasto(ByVal CUAA As ObjNotifica(Of AgronicaCoreDTOStd.InData.Notifiche.CUAAObj),
                                                      ByVal catasto As TerreniChangeLog,
                                                      ByRef _logger As LoggerManager,
                                                      ByRef EsitoSync As SyncAcknowledge) As Boolean
        Dim ret As Boolean = True
        Try
            Dim cast_int As Integer
            For Each particella In catasto.records
                If Integer.TryParse(particella.foglio, cast_int) = False Then
                    _logger.AppendLog(CUAA, "terreni", String.Format("[{0} - {1} - {2}]  campo foglio non numerico ({3})", CUAA.payload.CUAA, CUAA.payload.Operazione, CUAA.payload.id_signal, particella.GetKey()), LogType.Errore)
                    EsitoSync.errors.Add(New SyncErrors() With {.code = "104", .msg = "campo foglio non numerico", .key = particella.GetKey()})
                    ret = False
                End If
                If Integer.TryParse(particella.particella, cast_int) = False Then
                    _logger.AppendLog(CUAA, "terreni", String.Format("[{0} - {1} - {2}]  campo particella non numerico ({3})", CUAA.payload.CUAA, CUAA.payload.Operazione, CUAA.payload.id_signal, particella.GetKey()), LogType.Errore)
                    EsitoSync.errors.Add(New SyncErrors() With {.code = "105", .msg = "campo particella non numerico", .key = particella.GetKey()})
                    ret = False
                End If

                If particella.area_condotta_mq = 0 OrElse particella.area_condotta_mq Is Nothing Then
                    _logger.AppendLog(CUAA, "terreni", String.Format("[{0} - {1} - {2}]  Particella catastale senza superfiche di conduzione ({3})", CUAA.payload.CUAA, CUAA.payload.Operazione, CUAA.payload.id_signal, particella.GetKey()), LogType.Errore)
                    EsitoSync.errors.Add(New SyncErrors() With {.code = "101", .msg = "Particella catastale senza superfiche di conduzione", .key = particella.GetKey()})
                    ret = False
                End If
                If particella.sezione Is Nothing Then
                    _logger.AppendLog(CUAA, "terreni", String.Format("[{0} - {1} - {2}]  Particella catastale con campo sezione non valorizzato ({3})", CUAA.payload.CUAA, CUAA.payload.Operazione, CUAA.payload.id_signal, particella.GetKey()), LogType.Errore)
                    EsitoSync.errors.Add(New SyncErrors() With {.code = "102", .msg = "Particella catastale con campo sezione non valorizzato", .key = particella.GetKey()})
                    ret = False
                Else
                    If particella.sezione.Length > 2 Then
                        _logger.AppendLog(CUAA, "terreni", String.Format("[{0} - {1} - {2}]  Particella catastale con campo sezione più lungo di due caratteri ({3})", CUAA.payload.CUAA, CUAA.payload.Operazione, CUAA.payload.id_signal, particella.GetKey()), LogType.Errore)
                        EsitoSync.errors.Add(New SyncErrors() With {.code = "103", .msg = "Particella catastale con campo sezione più lungo di due caratteri", .key = particella.GetKey()})
                        ret = False
                    End If
                End If
            Next
        Catch ex As Exception
            ret = False
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Public Shared Function ControlliVariazioniRipartizioniCatastoAppezzamento(ByVal CUAA As ObjNotifica(Of AgronicaCoreDTOStd.InData.Notifiche.CUAAObj),
                                                      ByVal catasto As AppezzamentoTerrenoChangeLog,
                                                      ByRef _logger As LoggerManager,
                                                      ByRef EsitoSync As SyncAcknowledge,
                                                      ByVal CentroPK As AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK,
                                                              Optional ByRef ObjParametri_Server As AgronicaCoreParametri = Nothing) As Boolean
        Dim ret As Boolean = True
        Try
            For Each particella In catasto.records
                If particella.sezione Is Nothing Then
                    _logger.AppendLog(CUAA, "pcg_terreni", String.Format("[{0} - {1} - {2}] Variazione ripartizione catastale su appezzamento - Particella catastale con campo sezione non valorizzato ({3})", CUAA.payload.CUAA, CUAA.payload.Operazione, CUAA.payload.id_signal, particella.GetKeyParticella()), LogType.Errore)
                    EsitoSync.errors.Add(New SyncErrors() With {.code = "104", .msg = "Variazione ripartizione catastale su appezzamento - Particella catastale con campo sezione non valorizzato", .key = particella.GetKey()})
                    ret = False
                Else
                    If particella.sezione.Length > 2 Then
                        _logger.AppendLog(CUAA, "pcg_terreni", String.Format("[{0} - {1} - {2}] Variazione ripartizione catastale su appezzamento - Particella catastale con campo sezione più lungo di due caratteri ({3})", CUAA.payload.CUAA, CUAA.payload.Operazione, CUAA.payload.id_signal, particella.GetKeyParticella()), LogType.Errore)
                        EsitoSync.errors.Add(New SyncErrors() With {.code = "105", .msg = "Variazione ripartizione catastale su appezzamento - Particella catastale con campo sezione più lungo di due caratteri", .key = particella.GetKey()})
                        ret = False
                    End If
                End If
                If particella.id_appezzamento = "" Then
                    _logger.AppendLog(CUAA, "pcg_terreni", String.Format("[{0} - {1} - {2}] Variazione ripartizione catastale su appezzamento - id appezzamento non specificato ({3})", CUAA.payload.CUAA, CUAA.payload.Operazione, CUAA.payload.id_signal, particella.GetKeyParticella()), LogType.Errore)
                    EsitoSync.errors.Add(New SyncErrors() With {.code = "106", .msg = "Variazione ripartizione catastale su appezzamento - id appezzamento non specificato", .key = particella.GetKey()})
                    ret = False
                End If
                'If particella.id_appezzamento_padre = "" Then
                '    _logger.AppendLog(CUAA, "pcg_terreni", String.Format("[{0} - {1} - {2}] Variazione ripartizione catastale su appezzamento - id appezzamento padre non specificato ({3})", CUAA.payload.CUAA, CUAA.payload.Operazione, CUAA.payload.id_signal, particella.GetKeyParticella()), LogType.Errore)
                '    EsitoSync.errors.Add(New SyncErrors() With {.code = "107", .msg = "Variazione ripartizione catastale su appezzamento - id appezzamento padre non specificato", .key = particella.GetKey()})
                '    ret = False
                'End If
                Dim AppKey = AgronicaCoreAnagrafeBIZ.Appezzamento_R.VerificaEsistenzaAppezzamentoDaCodice(CentroPK.partitaIva, CentroPK.codice, particella.id_appezzamento_padre, ObjParametri_Server, False)
                If AppKey Is Nothing Then
                    _logger.AppendLog(CUAA, "pcg_terreni", String.Format("[{0} - {1} - {2}] Variazione ripartizione catastale su appezzamento - appezzamento non presente in GIAS ({3})", CUAA.payload.CUAA, CUAA.payload.Operazione, CUAA.payload.id_signal, particella.id_appezzamento_padre), LogType.Errore)
                    EsitoSync.errors.Add(New SyncErrors() With {.code = "108", .msg = "Variazione ripartizione catastale su appezzamento - appezzamento non presente in GIAS ({3})", .key = particella.id_appezzamento_padre})
                    ret = False
                End If
            Next
        Catch ex As Exception
            ret = False
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Public Shared Function ControlliVariazioniEquipaggiamento(ByVal CUAA As ObjNotifica(Of AgronicaCoreDTOStd.InData.Notifiche.CUAAObj),
                                                              ByVal equipaggiamento_changes As EquipaggiamentoCUAAChangeLog,
                                                              ByRef _logger As LoggerManager,
                                                              ByRef ObjParametri_Server As AgronicaCoreParametri,
                                                              ByRef EsitoSync As SyncAcknowledge) As Boolean
        Dim ret As Boolean = True
        Try

            Dim xAgeaMacchine As New AgronicaCoreMetaSchemaDAL.Codifica_Macchine_Agea
            For Each equip In equipaggiamento_changes.records
                Dim class_code = xAgeaMacchine.Class_Cod_da_AGEA_Cod(ObjParametri_Server, equip.elemento_anagrafico.tipo, "")

                If class_code = "" Then
                    _logger.AppendLog(CUAA, "equipaggiamenti", String.Format("[{0} - {1} - {2}] Variazione equipaggiamento ({3}) - Codice agea senza corrispondenza ClassCode Gias. Impossibile proseguire ({4})", CUAA.payload.CUAA, CUAA.payload.Operazione, CUAA.payload.id_signal, equip.codice, equip.elemento_anagrafico.tipo), LogType.Errore)
                    EsitoSync.errors.Add(New SyncErrors() With {.code = "601", .msg = "Variazione equipaggiamento ({3}) - Codice agea senza corrispondenza ClassCode Gias. Impossibile proseguire", .key = equip.codice})
                    ret = False
                End If
            Next

        Catch ex As Exception
            ret = False
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Public Shared Function ControlliVariazioniLavoratori(ByVal CUAA As ObjNotifica(Of AgronicaCoreDTOStd.InData.Notifiche.CUAAObj),
                                                      ByVal lavoratore_changes As LavoratoriCUAAChangeLog,
                                                      ByRef _logger As LoggerManager,
                                                      ByRef EsitoSync As SyncAcknowledge) As Boolean
        Dim ret As Boolean = True
        Try
            'TO-DO
        Catch ex As Exception
            ret = False
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Public Shared Function ControlliPreCaricamentoPCG(ByVal CUAA As ObjNotifica(Of AgronicaCoreDTOStd.InData.Notifiche.CUAAObj),
                                                      ByVal pcg As PianoColturaleGrafico,
                                                      ByRef ObjParametriServer As AgronicaCoreParametri,
                                                      ByRef _logger As LoggerManager,
                                                      ByRef EsitoSync As SyncAcknowledge,
                                                      Optional ByVal TagName As String = "",
                                                      Optional ByVal ReplaceInvalidPolygon As Boolean = False,
                                                      Optional ByVal TipoConfigurazione As enum_DataPublish_Configurazione = enum_DataPublish_Configurazione.Demetra) As Boolean
        Dim ret As Boolean = True
        Try
            If ControllaParametri(CUAA, pcg, ObjParametriServer, _logger, EsitoSync, TagName, ReplaceInvalidPolygon, TipoConfigurazione) = False Then
                ret = False
            End If

        Catch ex As Exception
            ret = False
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Public Shared Function ControlliPreAggiornamentoPCG(ByVal CUAA As ObjNotifica(Of AgronicaCoreDTOStd.InData.Notifiche.CUAAObj),
                                                        ByVal pcg As PianoColturaleGraficoChangeLog,
                                                        ByRef ObjParametriServer As AgronicaCoreParametri,
                                                        ByRef _logger As LoggerManager,
                                                        ByRef EsitoSync As SyncAcknowledge,
                                                        Optional ByVal TagName As String = "",
                                                        Optional ByVal ReplaceInvalidPolygon As Boolean = False,
                                                      Optional ByVal TipoConfigurazione As enum_DataPublish_Configurazione = enum_DataPublish_Configurazione.Demetra) As Boolean
        Dim ret As Boolean = True
        Try
            If ControllaParametriPreAggiornamento(CUAA, pcg, ObjParametriServer, _logger, EsitoSync, TagName, ReplaceInvalidPolygon, TipoConfigurazione) = False Then
                ret = False
            End If

        Catch ex As Exception
            ret = False
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Private Shared Function ControllaParametri(ByVal CUAA As ObjNotifica(Of AgronicaCoreDTOStd.InData.Notifiche.CUAAObj),
                                               ByVal pcg As PianoColturaleGrafico,
                                               ByVal ObjParametriServer As AgronicaCoreParametri,
                                               ByVal _logger As LoggerManager,
                                               ByRef EsitoSync As SyncAcknowledge,
                                               Optional ByVal TagName As String = "",
                                               Optional ByVal ReplaceInvalidPolygon As Boolean = False,
                                               Optional ByVal TipoConfigurazione As enum_DataPublish_Configurazione = enum_DataPublish_Configurazione.Demetra) As Boolean
        Dim ret As Boolean = True
        Dim xGrfiXVegCod_R As New AgronicaCoreMetaSchemaDAL.GruppoFinalitaxSpecieVegetali_R
        Try
            '0 - controllo sovrapposizione date di validità a parità di id_appezamento_padre
            ret = ControlloSovrapposizioneDate(CUAA, pcg, _logger, EsitoSync)
            If ret = True Then
                '1- controlli puntuali per singolo appezzamento
                For Each appezzamento In pcg.records

                    'presenza dichiarazioni per definizione varietà\destinazione uso
                    If appezzamento.dichiarazioni Is Nothing OrElse appezzamento.dichiarazioni.Count <= 0 Then
                        _logger.AppendLog(CUAA, "pcg", String.Format("[{0} - {1} - {2}] Id_Appezzamento {3} - senza dichiarazioni colture, acquisizione piano colturale per cuaa\campagna ({0}\{4}) non possibile", CUAA.payload.CUAA, CUAA.payload.Operazione, CUAA.payload.id_signal, appezzamento.id_appezzamento, CUAA.payload.Campagna.ToString()), LogType.Errore)
                        EsitoSync.errors.Add(New SyncErrors() With {.code = "301", .msg = String.Format("Id_Appezzamento {1} - senza dichiarazioni colture, acquisizione piano colturale per cuaa\campagna ({0}\{2}) non possibile", CUAA.payload.CUAA, appezzamento.id_appezzamento, CUAA.payload.Campagna.ToString()), .key = appezzamento.id_appezzamento})
                        ret = False
                    Else
                        Dim id_dich_list = appezzamento.dichiarazioni.Select(Of String)(Function(x) x.id_dichiarazione).Distinct().ToList()
                        For Each id_dich In id_dich_list
                            If appezzamento.dichiarazioni.Where(Function(x) x.id_dichiarazione = id_dich).Count > 1 Then
                                _logger.AppendLog(CUAA, "pcg", String.Format("[{0} - {1} - {2}] Id_Appezzamento {3} - id_dichirazione {5} duplicato, acquisizione piano colturale per cuaa\campagna ({0}\{4}) non possibile", CUAA.payload.CUAA, CUAA.payload.Operazione, CUAA.payload.id_signal, appezzamento.id_appezzamento, CUAA.payload.Campagna.ToString(), id_dich), LogType.Errore)
                                EsitoSync.errors.Add(New SyncErrors() With {.code = "307", .msg = String.Format("Id_Appezzamento {1} - id_dichirazione {3} duplicato, acquisizione piano colturale per cuaa\campagna ({0}\{2}) non possibile", CUAA.payload.CUAA, appezzamento.id_appezzamento, CUAA.payload.Campagna.ToString(), id_dich), .key = appezzamento.id_appezzamento})
                                ret = False
                            End If
                        Next

                        For Each dich In appezzamento.dichiarazioni
                            'lavez - 2024-07-23 - controllo coerenza date inizio\fine validità dichiarazione
                            If dich.data_inizio_validita >= dich.data_fine_validita Then
                                _logger.AppendLog(CUAA, "pcg", String.Format("[{0} - {1} - {2}] Id_Appezzamento {3} - id_dichirazione {4} La data di fine validità è antecedente la data di inizio validità, acquisizione piano colturale per cuaa\campagna ({0}\{2}) non possibile ", CUAA.payload.CUAA, CUAA.payload.Operazione, CUAA.payload.id_signal, appezzamento.id_appezzamento, dich.id_dichiarazione), LogType.Errore)
                                EsitoSync.errors.Add(New SyncErrors() With {.code = "308", .msg = String.Format("[{0} - {1} - {2}] Id_Appezzamento {3} - id_dichirazione {4} La data di fine validità è antecedente la data di inizio validità, acquisizione piano colturale per cuaa\campagna ({0}\{2}) non possibile ", CUAA.payload.CUAA, CUAA.payload.Operazione, CUAA.payload.id_signal, appezzamento.id_appezzamento, dich.id_dichiarazione), .key = dich.id_dichiarazione})
                                ret = False
                            End If

                            Dim dich_list = appezzamento.dichiarazioni.Where(Function(x) x.id_dichiarazione <> dich.id_dichiarazione).ToList()
                            For Each dd In dich_list
                                If dd.data_inizio_validita >= dich.data_inizio_validita AndAlso dd.data_inizio_validita <= dich.data_fine_validita OrElse
                                dd.data_fine_validita >= dich.data_inizio_validita AndAlso dd.data_fine_validita <= dich.data_fine_validita Then
                                    _logger.AppendLog(CUAA, "pcg", String.Format("[{0} - {1} - {2}] Id_Appezzamento {3} - id_dichirazione {5} con validità sovrapposta rispetto id_dichiarazione {6}, acquisizione piano colturale per cuaa\campagna ({0}\{4}) non possibile", CUAA.payload.CUAA, CUAA.payload.Operazione, CUAA.payload.id_signal, appezzamento.id_appezzamento, CUAA.payload.Campagna.ToString(), dd.id_dichiarazione, dich.id_dichiarazione), LogType.Errore)
                                    EsitoSync.errors.Add(New SyncErrors() With {.code = "308", .msg = String.Format("Id_Appezzamento {1} - id_dichirazione {3} con validità sovrapposta rispetto id_dichiarazione {4}, acquisizione piano colturale per cuaa\campagna ({0}\{2}) non possibile", CUAA.payload.CUAA, appezzamento.id_appezzamento, CUAA.payload.Campagna.ToString(), dd.id_dichiarazione, dich.id_dichiarazione), .key = dd.id_dichiarazione})
                                    ret = False
                                End If
                            Next
                        Next
                    End If

                    If appezzamento.dichiarazioni IsNot Nothing Then
                        'decodifica codice agea
                        For Each dic In appezzamento.dichiarazioni
                            Dim veg_cod As Integer = 0
                            Dim id_cod As Integer = 0
                            Dim cul_cod As Integer = 0
                            Dim grfi_cod As Integer = 0
                            Dim Grva_Cod As Integer = 0
                            DecodificaCodiceAgea(dic.codice_coltura, veg_cod, id_cod, cul_cod, grfi_cod, Grva_Cod, ObjParametriServer, TagName)
                            If veg_cod = 0 AndAlso cul_cod = 0 AndAlso id_cod = 0 Then
                                _logger.AppendLog(CUAA, "pcg", String.Format("[{0} - {1} - {2}] Id_Appezzamento {3} - Decodifica codice agea ({5}) fallita, acquisizione piano colturale per cuaa\campagna ({0}\{4}) non possibile", CUAA.payload.CUAA, CUAA.payload.Operazione, CUAA.payload.id_signal, appezzamento.id_appezzamento, CUAA.payload.Campagna.ToString(), dic.codice_coltura), LogType.Errore)
                                EsitoSync.errors.Add(New SyncErrors() With {.code = "302", .msg = String.Format("Id_Appezzamento {1} - Decodifica codice agea ({3}) fallita, acquisizione piano colturale per cuaa\campagna ({0}\{2}) non possibile", CUAA.payload.CUAA, appezzamento.id_appezzamento, CUAA.payload.Campagna.ToString(), dic.codice_coltura), .key = dic.codice_coltura})
                                ret = False
                            End If

                            If grfi_cod = 0 AndAlso veg_cod <> 0 AndAlso id_cod = 0 Then
                                grfi_cod = xGrfiXVegCod_R.PrimoGrfiCod_from_Vegcod(veg_cod, ObjParametriServer)
                            End If

                            If grfi_cod = 0 AndAlso veg_cod <> 0 Then
                                _logger.AppendLog(CUAA, "pcg", String.Format("[{0} - {1} - {2}] Id_Appezzamento {3} - Nessun gruppo finalità recuperato per la specie vegetale {5} , acquisizione piano colturale per cuaa\campagna ({0}\{4}) non possibile", CUAA.payload.CUAA, CUAA.payload.Operazione, CUAA.payload.id_signal, appezzamento.id_appezzamento, CUAA.payload.Campagna.ToString(), veg_cod.ToString()), LogType.Errore)
                                EsitoSync.errors.Add(New SyncErrors() With {.code = "306", .msg = String.Format("Id_Appezzamento {1} -  Nessun gruppo finalità recuperato per la specie vegetale {3}, acquisizione piano colturale per cuaa\campagna ({0}\{2}) non possibile", CUAA.payload.CUAA, appezzamento.id_appezzamento, CUAA.payload.Campagna.ToString(), veg_cod.ToString()), .key = dic.codice_coltura})
                                ret = False
                            End If
                        Next
                    End If


                    'presenza poligono gis
                    If appezzamento.gis Is Nothing OrElse appezzamento.gis.wkt = "" Then
                        _logger.AppendLog(CUAA, "pcg", String.Format("[{0} - {1} - {2}] Id_Appezzamento {3} - Senza poligono gis, acquisizione piano colturale per cuaa\campagna ({0}\{4}) non possibile", CUAA.payload.CUAA, CUAA.payload.Operazione, CUAA.payload.id_signal, appezzamento.id_appezzamento, CUAA.payload.Campagna.ToString()), LogType.Errore)
                        EsitoSync.errors.Add(New SyncErrors() With {.code = "303", .msg = String.Format("Id_Appezzamento {1} - Senza poligono gis, acquisizione piano colturale per cuaa\campagna ({0}\{2}) non possibile", CUAA.payload.CUAA, appezzamento.id_appezzamento, CUAA.payload.Campagna.ToString()), .key = appezzamento.id_appezzamento})
                        ret = False
                    Else
                        'Lavez - 13/05/2025 - condizionata al tipo configurazione (SOLO NEW AGRI)
                        'Lavez - 14/03/2025 - riattivata
                        'Lavez - 22/10/2024 - controllo disattivato ed introdotta sostituzione con poligono arbitrario e registrazione del poligono originale in tabella apposita
                        ''Validità poligono
                        If (TipoConfigurazione = enum_DataPublish_Configurazione.RegioneUmbria) Then
                            Dim errMsg As String = ""
                            If CheckCartography(CUAA, appezzamento.gis.wkt, appezzamento.gis.sr_code, "EPSG:4326", appezzamento.id_appezzamento_padre, appezzamento.id_appezzamento, _logger, ObjParametriServer, errMsg, ReplaceInvalidPolygon, False) = False Then
                                _logger.AppendLog(CUAA, "pcg", String.Format("[{0} - {1} - {2}] Id_Appezzamento {3} - Poligono non valido ( {3} ), acquisizione piano colturale per cuaa\campagna ({0}\{4}) non possibile. {5}", CUAA.payload.CUAA, CUAA.payload.Operazione, CUAA.payload.id_signal, appezzamento.id_appezzamento, CUAA.payload.Campagna.ToString(), errMsg), LogType.Errore)
                                EsitoSync.errors.Add(New SyncErrors() With {.code = "304", .msg = String.Format("Id_Appezzamento {1} - Poligono non valido ( {3} ), acquisizione piano colturale per cuaa\campagna ({0}\{2}) non possibile", CUAA.payload.CUAA, appezzamento.id_appezzamento, CUAA.payload.Campagna.ToString(), errMsg), .key = appezzamento.id_appezzamento})
                                ret = False
                            End If
                        End If
                    End If

                    'area maggiore di 0
                    If appezzamento.area_mq <= 0 Then
                        _logger.AppendLog(CUAA, "pcg", String.Format("[{0} - {1} - {2}] Id_Appezzamento {3} - Superficie appezzamento nulla, acquisizione piano colturale per cuaa\campagna ({0}\{4}) non possibile", CUAA.payload.CUAA, CUAA.payload.Operazione, CUAA.payload.id_signal, appezzamento.id_appezzamento, CUAA.payload.Campagna.ToString()), LogType.Errore)
                        EsitoSync.errors.Add(New SyncErrors() With {.code = "305", .msg = String.Format("Id_Appezzamento {1} - Superficie appezzamento nulla, acquisizione piano colturale per cuaa\campagna ({0}\{2}) non possibile", CUAA.payload.CUAA, appezzamento.id_appezzamento, CUAA.payload.Campagna.ToString()), .key = appezzamento.id_appezzamento})
                        ret = False
                    End If
                Next
            End If

        Catch ex As Exception
            ret = False
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret

    End Function

    Private Shared Function ControlloSovrapposizioneDate(ByVal CUAA As ObjNotifica(Of AgronicaCoreDTOStd.InData.Notifiche.CUAAObj),
                                                         ByVal pcg As PianoColturaleGrafico,
                                                         ByVal _logger As LoggerManager,
                                                         ByRef EsitoSync As SyncAcknowledge) As Boolean
        Dim ret As Boolean = True
        Try
            Dim date_check As New List(Of PCG_Validity_List)

            For Each app In pcg.records
                For Each dich In app.dichiarazioni
                    date_check.Add(New PCG_Validity_List() With {
                                        .id_appezzamento_padre = app.id_appezzamento_padre,
                                        .id_appezzamento = app.id_appezzamento,
                                        .id_dichiarazione = dich.id_dichiarazione,
                                        .validita_inizio = dich.data_inizio_validita,
                                        .validita_fine = dich.data_fine_validita
                                   })
                Next
            Next

            For Each id_app_padre In date_check.Select(Of String)(Function(x) x.id_appezzamento_padre).Distinct().ToList()
                Dim id_app_date_list = date_check.Where(Function(x) x.id_appezzamento_padre = id_app_padre).ToList()

                For Each elem In id_app_date_list
                    Dim curr_key = elem.GetKey()
                    If CheckDate(elem.validita_inizio) = CheckDate(elem.validita_fine) Then
                        _logger.AppendLog(CUAA, "pcg", String.Format("[{0} - {1} - {2}] Id_Appezzamento {3} - id_dichirazione {5} con date inizio/fine validità sovrapposte ({6}/{7}), acquisizione piano colturale per cuaa\campagna ({0}\{4}) non possibile", CUAA.payload.CUAA, CUAA.payload.Operazione, CUAA.payload.id_signal, elem.id_appezzamento, CUAA.payload.Campagna.ToString(), elem.id_dichiarazione, CheckDate(elem.validita_inizio), CheckDate(elem.validita_fine)), LogType.Errore)
                        EsitoSync.errors.Add(New SyncErrors() With {.code = "310", .msg = String.Format("Id_Appezzamento {1} - id_dichirazione {3} con date inizio/fine validità sovrapposte ({4}/{5}), acquisizione piano colturale per cuaa\campagna ({0}\{2}) non possibile", CUAA.payload.CUAA, elem.id_appezzamento, CUAA.payload.Campagna.ToString(), elem.id_dichiarazione, CheckDate(elem.validita_inizio), CheckDate(elem.validita_fine)), .key = elem.id_appezzamento & "|" & elem.id_dichiarazione})
                        ret = False
                    End If
                    For Each sub_elem In id_app_date_list.Where(Function(x) x.GetKey <> curr_key).ToList()
                        If sub_elem.validita_inizio >= elem.validita_inizio AndAlso sub_elem.validita_inizio <= elem.validita_fine OrElse
                        sub_elem.validita_fine >= elem.validita_inizio AndAlso sub_elem.validita_fine <= elem.validita_fine Then
                            _logger.AppendLog(CUAA, "pcg", String.Format("[{0} - {1} - {2}] Id_Appezzamento_Padre {3} - id_dichirazione {5} con validità sovrapposta rispetto id_dichiarazione {6}, acquisizione piano colturale per cuaa\campagna ({0}\{4}) non possibile", CUAA.payload.CUAA, CUAA.payload.Operazione, CUAA.payload.id_signal, elem.id_appezzamento_padre, CUAA.payload.Campagna.ToString(), sub_elem.id_dichiarazione, elem.id_dichiarazione), LogType.Errore)
                            EsitoSync.errors.Add(New SyncErrors() With {.code = "309", .msg = String.Format("Id_Appezzamento {1} - id_dichirazione {3} con validità sovrapposta rispetto id_dichiarazione {4}, acquisizione piano colturale per cuaa\campagna ({0}\{2}) non possibile", CUAA.payload.CUAA, elem.id_appezzamento_padre, CUAA.payload.Campagna.ToString(), sub_elem.id_dichiarazione, elem.id_dichiarazione), .key = elem.id_appezzamento_padre & "|" & elem.id_dichiarazione & "|" & sub_elem.id_dichiarazione})
                            ret = False
                        End If
                        If CheckDate(sub_elem.validita_inizio) = CheckDate(sub_elem.validita_fine) Then
                            _logger.AppendLog(CUAA, "pcg", String.Format("[{0} - {1} - {2}] Id_Appezzamento {3} - id_dichirazione {5} con date inizio/fine validità sovrapposte ({6}/{7}), acquisizione piano colturale per cuaa\campagna ({0}\{4}) non possibile", CUAA.payload.CUAA, CUAA.payload.Operazione, CUAA.payload.id_signal, sub_elem.id_appezzamento, CUAA.payload.Campagna.ToString(), sub_elem.id_dichiarazione, CheckDate(sub_elem.validita_inizio), CheckDate(sub_elem.validita_fine)), LogType.Errore)
                            EsitoSync.errors.Add(New SyncErrors() With {.code = "310", .msg = String.Format("Id_Appezzamento {1} - id_dichirazione {3} con date inizio/fine validità sovrapposte ({4}/{5}), acquisizione piano colturale per cuaa\campagna ({0}\{2}) non possibile", CUAA.payload.CUAA, sub_elem.id_appezzamento, CUAA.payload.Campagna.ToString(), sub_elem.id_dichiarazione, CheckDate(sub_elem.validita_inizio), CheckDate(sub_elem.validita_fine)), .key = sub_elem.id_appezzamento & "|" & sub_elem.id_dichiarazione})
                            ret = False
                        End If
                    Next
                Next
            Next
            date_check.Clear()
        Catch ex As Exception
            ret = False
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret

    End Function

    Private Shared Function ControllaParametriPreAggiornamento(ByVal CUAA As ObjNotifica(Of AgronicaCoreDTOStd.InData.Notifiche.CUAAObj),
                                                               ByVal pcg As PianoColturaleGraficoChangeLog,
                                                               ByVal ObjParametriServer As AgronicaCoreParametri,
                                                               ByVal _logger As LoggerManager,
                                                               ByRef EsitoSync As SyncAcknowledge,
                                                               Optional ByVal TagName As String = "",
                                                               Optional ByVal ReplaceInvalidPolygon As Boolean = False,
                                                               Optional ByVal TipoConfigurazione As enum_DataPublish_Configurazione = enum_DataPublish_Configurazione.Demetra) As Boolean
        Dim ret As Boolean = True
        Dim xGrfiXVegCod_R As New AgronicaCoreMetaSchemaDAL.GruppoFinalitaxSpecieVegetali_R
        Try
            '0 - controllo sovrapposizione date di validità a parità di id_appezamento_padre (senza considerare le cancellazioni)
            ret = ControlloSovrapposizioneDate_Variazione(CUAA, pcg, _logger, EsitoSync)
            If ret = True Then
                '1- controlli puntuali per singolo appezzamento
                For Each appezzamento In pcg.records.Where(Function(x) x.campagna = CUAA.payload.Campagna).ToList()
                    If appezzamento.tipo_modifica <> "C" Then

                        'presenza dichiarazioni per definizione varietà\destinazione uso
                        If (appezzamento.dichiarazioni Is Nothing OrElse appezzamento.dichiarazioni.Count <= 0) Then
                            _logger.AppendLog(CUAA, "pcg", String.Format("[{0} - {1} - {2}] Id_Appezzamento {3} - senza dichiarazioni colture, acquisizione piano colturale per cuaa\campagna ({0}\{4})  non possibile", CUAA.payload.CUAA, CUAA.payload.Operazione, CUAA.payload.id_signal, appezzamento.id_appezzamento, CUAA.payload.Campagna.ToString()), LogType.Errore)
                            EsitoSync.errors.Add(New SyncErrors() With {.code = "301", .msg = String.Format("Id_Appezzamento {1} - senza dichiarazioni colture, acquisizione piano colturale per cuaa\campagna ({0}\{2})  non possibile", CUAA.payload.CUAA, appezzamento.id_appezzamento, CUAA.payload.Campagna.ToString()), .key = appezzamento.id_appezzamento})
                            ret = False
                        Else
                            Dim id_dich_list = appezzamento.dichiarazioni.Select(Of String)(Function(x) x.id_dichiarazione).Distinct().ToList()
                            For Each id_dich In id_dich_list
                                If appezzamento.dichiarazioni.Where(Function(x) x.id_dichiarazione = id_dich).Count > 1 Then
                                    _logger.AppendLog(CUAA, "pcg", String.Format("[{0} - {1} - {2}] Id_Appezzamento {3} - id_dichirazione {5} duplicato, acquisizione piano colturale per cuaa\campagna ({0}\{4})  non possibile", CUAA.payload.CUAA, CUAA.payload.Operazione, CUAA.payload.id_signal, appezzamento.id_appezzamento, CUAA.payload.Campagna.ToString(), id_dich), LogType.Errore)
                                    EsitoSync.errors.Add(New SyncErrors() With {.code = "307", .msg = String.Format("Id_Appezzamento {1} - id_dichirazione {3} duplicato, acquisizione piano colturale per cuaa\campagna ({0}\{2})  non possibile", CUAA.payload.CUAA, appezzamento.id_appezzamento, CUAA.payload.Campagna.ToString(), id_dich), .key = appezzamento.id_appezzamento})
                                    ret = False
                                End If
                            Next

                            For Each dich In appezzamento.dichiarazioni
                                'lavez - 2024-07-23 - controllo coerenza date inizio\fine validità dichiarazione
                                If dich.data_inizio_validita >= dich.data_fine_validita Then
                                    _logger.AppendLog(CUAA, "pcg", String.Format("[{0} - {1} - {2}] Id_Appezzamento {3} - id_dichirazione {4} La data di fine validità è antecedente la data di inizio validità, acquisizione piano colturale per cuaa\campagna ({0}\{2}) non possibile ", CUAA.payload.CUAA, CUAA.payload.Operazione, CUAA.payload.id_signal, appezzamento.id_appezzamento, dich.id_dichiarazione), LogType.Errore)
                                    EsitoSync.errors.Add(New SyncErrors() With {.code = "308", .msg = String.Format("[{0} - {1} - {2}] Id_Appezzamento {3} - id_dichirazione {4} La data di fine validità è antecedente la data di inizio validità, acquisizione piano colturale per cuaa\campagna ({0}\{2}) non possibile ", CUAA.payload.CUAA, CUAA.payload.Operazione, CUAA.payload.id_signal, appezzamento.id_appezzamento, dich.id_dichiarazione), .key = dich.id_dichiarazione})
                                    ret = False
                                End If

                                Dim dich_list = appezzamento.dichiarazioni.Where(Function(x) x.id_dichiarazione <> dich.id_dichiarazione).ToList()
                                For Each dd In dich_list
                                    If dd.data_inizio_validita >= dich.data_inizio_validita AndAlso dd.data_inizio_validita <= dich.data_fine_validita OrElse
                                    dd.data_fine_validita >= dich.data_inizio_validita AndAlso dd.data_fine_validita <= dich.data_fine_validita Then
                                        _logger.AppendLog(CUAA, "pcg", String.Format("[{0} - {1} - {2}] Id_Appezzamento {3} - id_dichirazione {5} con validità sovrapposta rispetto id_dichiarazione {6}, acquisizione piano colturale per cuaa\campagna ({0}\{4}) non possibile", CUAA.payload.CUAA, CUAA.payload.Operazione, CUAA.payload.id_signal, appezzamento.id_appezzamento, CUAA.payload.Campagna.ToString(), dd.id_dichiarazione, dich.id_dichiarazione), LogType.Errore)
                                        EsitoSync.errors.Add(New SyncErrors() With {.code = "308", .msg = String.Format("Id_Appezzamento {1} - id_dichirazione {3} con validità sovrapposta rispetto id_dichiarazione {4}, acquisizione piano colturale per cuaa\campagna ({0}\{2}) non possibile", CUAA.payload.CUAA, appezzamento.id_appezzamento, CUAA.payload.Campagna.ToString(), dd.id_dichiarazione, dich.id_dichiarazione), .key = dd.id_dichiarazione})
                                        ret = False
                                    End If
                                Next
                            Next
                        End If

                        If appezzamento.dichiarazioni IsNot Nothing Then
                            'decodifica codice agea
                            For Each dic In appezzamento.dichiarazioni
                                Dim veg_cod As Integer = 0
                                Dim id_cod As Integer = 0
                                Dim cul_cod As Integer = 0
                                Dim grfi_cod As Integer = 0
                                Dim Grva_Cod As Integer = 0
                                DecodificaCodiceAgea(dic.codice_coltura, veg_cod, id_cod, cul_cod, grfi_cod, Grva_Cod, ObjParametriServer, TagName)
                                If veg_cod = 0 AndAlso cul_cod = 0 AndAlso id_cod = 0 Then
                                    _logger.AppendLog(CUAA, "pcg", String.Format("[{0} - {1} - {2}] Id_Appezzamento {3} - Decodifica codice agea ({5}) fallita, acquisizione piano colturale per cuaa\campagna ({0}\{4}) non possibile", CUAA.payload.CUAA, CUAA.payload.Operazione, CUAA.payload.id_signal, appezzamento.id_appezzamento, CUAA.payload.Campagna.ToString(), dic.codice_coltura), LogType.Errore)
                                    EsitoSync.errors.Add(New SyncErrors() With {.code = "302", .msg = String.Format("Id_Appezzamento {1} - Decodifica codice agea ({3}) fallita, acquisizione piano colturale per cuaa\campagna ({0}\{2}) non possibile", CUAA.payload.CUAA, appezzamento.id_appezzamento, CUAA.payload.Campagna.ToString(), dic.codice_coltura), .key = dic.codice_coltura})
                                    ret = False
                                End If

                                If grfi_cod = 0 AndAlso veg_cod <> 0 AndAlso id_cod = 0 Then
                                    grfi_cod = xGrfiXVegCod_R.PrimoGrfiCod_from_Vegcod(veg_cod, ObjParametriServer)
                                End If

                                If grfi_cod = 0 AndAlso veg_cod <> 0 Then
                                    _logger.AppendLog(CUAA, "pcg", String.Format("[{0} - {1} - {2}] Id_Appezzamento {3} - Nessun gruppo finalità recuperato per la specie vegetale {5} verificare banca dati Agronica, acquisizione piano colturale per cuaa\campagna ({0}\{4}) non possibile", CUAA.payload.CUAA, CUAA.payload.Operazione, CUAA.payload.id_signal, appezzamento.id_appezzamento, CUAA.payload.Campagna.ToString(), veg_cod.ToString()), LogType.Errore)
                                    EsitoSync.errors.Add(New SyncErrors() With {.code = "306", .msg = String.Format("Id_Appezzamento {1} -  Nessun gruppo finalità recuperato per la specie vegetale {3} verificare banca dati Agronica, acquisizione piano colturale per cuaa\campagna ({0}\{2}) non possibile", CUAA.payload.CUAA, appezzamento.id_appezzamento, CUAA.payload.Campagna.ToString(), veg_cod.ToString()), .key = dic.codice_coltura})
                                    ret = False
                                End If

                            Next
                        End If

                        'presenza poligono gis
                        If appezzamento.gis Is Nothing OrElse appezzamento.gis.wkt = "" Then
                            _logger.AppendLog(CUAA, "pcg", String.Format("[{0} - {1} - {2}] Id_Appezzamento {3} - Senza poligono gis, acquisizione piano colturale per cuaa\campagna ({0}\{4}) non possibile", CUAA.payload.CUAA, CUAA.payload.Operazione, CUAA.payload.id_signal, appezzamento.id_appezzamento, CUAA.payload.Campagna.ToString()), LogType.Errore)
                            EsitoSync.errors.Add(New SyncErrors() With {.code = "303", .msg = String.Format("Id_Appezzamento {1} - Senza poligono gis, acquisizione piano colturale per cuaa\campagna ({0}\{2}) non possibile", CUAA.payload.CUAA, appezzamento.id_appezzamento, CUAA.payload.Campagna.ToString()), .key = appezzamento.id_appezzamento})
                            ret = False
                        Else
                            'Lavez - 13/05/2025 - condizionata al tipo configurazione (SOLO NEW AGRI)
                            'Lavez - 22/10/2024 - controllo disattivato ed introdotta sostituzione con poligono arbitrario e registrazione del poligono originale in tabella apposita
                            ''Validità poligono
                            If (TipoConfigurazione = enum_DataPublish_Configurazione.RegioneUmbria) Then
                                Dim errMsg As String = ""
                                If CheckCartography(CUAA, appezzamento.gis.wkt, appezzamento.gis.sr_code, "EPSG:4326", appezzamento.id_appezzamento_padre, appezzamento.id_appezzamento, _logger, ObjParametriServer, errMsg, ReplaceInvalidPolygon, False) = False Then
                                    _logger.AppendLog(CUAA, "pcg", String.Format("[{0} - {1} - {2}] Id_Appezzamento {3} - Poligono non valido ( {3} ), acquisizione piano colturale per cuaa\campagna ({0}\{4}) non possibile", CUAA.payload.CUAA, CUAA.payload.Operazione, CUAA.payload.id_signal, appezzamento.id_appezzamento, CUAA.payload.Campagna.ToString(), errMsg), LogType.Errore)
                                    EsitoSync.errors.Add(New SyncErrors() With {.code = "304", .msg = String.Format("Id_Appezzamento {1} - Poligono non valido ( {3} ), acquisizione piano colturale per cuaa\campagna ({0}\{2}) non possibile", CUAA.payload.CUAA, appezzamento.id_appezzamento, CUAA.payload.Campagna.ToString(), errMsg), .key = appezzamento.id_appezzamento})
                                    ret = False
                                End If
                            End If
                        End If
                        'area maggiore di 0
                        If appezzamento.area_mq <= 0 Then
                            _logger.AppendLog(CUAA, "pcg", String.Format("[{0} - {1} - {2}] Id_Appezzamento {3} - Superficie appezzamento nulla, acquisizione piano colturale per cuaa\campagna ({0}\{4}) non possibile", CUAA.payload.CUAA, CUAA.payload.Operazione, CUAA.payload.id_signal, appezzamento.id_appezzamento, CUAA.payload.Campagna.ToString()), LogType.Errore)
                            EsitoSync.errors.Add(New SyncErrors() With {.code = "305", .msg = String.Format("Id_Appezzamento {1} - Superficie appezzamento nulla, acquisizione piano colturale per cuaa\campagna ({0}\{2}) non possibile", CUAA.payload.CUAA, appezzamento.id_appezzamento, CUAA.payload.Campagna.ToString()), .key = appezzamento.id_appezzamento})
                            ret = False
                        End If
                    Else
                        _logger.AppendLog(CUAA, "pcg", String.Format("[{0} - {1} - {2}] Id_Appezzamento {3} - evento di cancellazione. nessun controllo", CUAA.payload.CUAA, CUAA.payload.Operazione, CUAA.payload.id_signal, appezzamento.id_appezzamento))
                    End If
                Next
            End If

        Catch ex As Exception
            ret = False
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret

    End Function

    Private Shared Function ControlloSovrapposizioneDate_Variazione(ByVal CUAA As ObjNotifica(Of AgronicaCoreDTOStd.InData.Notifiche.CUAAObj),
                                                                    ByVal pcg As PianoColturaleGraficoChangeLog,
                                                                    ByVal _logger As LoggerManager,
                                                                    ByRef EsitoSync As SyncAcknowledge) As Boolean
        Dim ret As Boolean = True
        Try
            Dim date_check As New List(Of PCG_Validity_List)

            For Each app In pcg.records.Where(Function(x) x.tipo_modifica <> "C").ToList()
                For Each dich In app.dichiarazioni
                    date_check.Add(New PCG_Validity_List() With {
                                        .id_appezzamento_padre = app.id_appezzamento_padre,
                                        .id_appezzamento = app.id_appezzamento,
                                        .id_dichiarazione = dich.id_dichiarazione,
                                        .validita_inizio = dich.data_inizio_validita,
                                        .validita_fine = dich.data_fine_validita
                                   })
                Next
            Next

            For Each id_app_padre In date_check.Select(Of String)(Function(x) x.id_appezzamento_padre).Distinct().ToList()
                Dim id_app_date_list = date_check.Where(Function(x) x.id_appezzamento_padre = id_app_padre).ToList()

                For Each elem In id_app_date_list
                    Dim curr_key = elem.GetKey()
                    If CheckDate(elem.validita_inizio) = CheckDate(elem.validita_fine) Then
                        _logger.AppendLog(CUAA, "pcg", String.Format("[{0} - {1} - {2}] Id_Appezzamento {3} - id_dichirazione {5} con date inizio/fine validità sovrapposte ({6}/{7}), acquisizione piano colturale per cuaa\campagna ({0}\{4}) non possibile", CUAA.payload.CUAA, CUAA.payload.Operazione, CUAA.payload.id_signal, elem.id_appezzamento, CUAA.payload.Campagna.ToString(), elem.id_dichiarazione, CheckDate(elem.validita_inizio), CheckDate(elem.validita_fine)), LogType.Errore)
                        EsitoSync.errors.Add(New SyncErrors() With {.code = "310", .msg = String.Format("Id_Appezzamento {1} - id_dichirazione {3} con date inizio/fine validità sovrapposte ({4}/{5}), acquisizione piano colturale per cuaa\campagna ({0}\{2}) non possibile", CUAA.payload.CUAA, elem.id_appezzamento, CUAA.payload.Campagna.ToString(), elem.id_dichiarazione, CheckDate(elem.validita_inizio), CheckDate(elem.validita_fine)), .key = elem.id_appezzamento & "|" & elem.id_dichiarazione})
                        ret = False
                    End If
                    For Each sub_elem In id_app_date_list.Where(Function(x) x.GetKey <> curr_key).ToList()
                        If sub_elem.validita_inizio >= elem.validita_inizio AndAlso sub_elem.validita_inizio <= elem.validita_fine OrElse
                        sub_elem.validita_fine >= elem.validita_inizio AndAlso sub_elem.validita_fine <= elem.validita_fine Then
                            _logger.AppendLog(CUAA, "pcg", String.Format("[{0} - {1} - {2}] Id_Appezzamento_Padre {3} - id_dichirazione {5} con validità sovrapposta rispetto id_dichiarazione {6}, acquisizione piano colturale per cuaa\campagna ({0}\{4}) non possibile", CUAA.payload.CUAA, CUAA.payload.Operazione, CUAA.payload.id_signal, elem.id_appezzamento_padre, CUAA.payload.Campagna.ToString(), sub_elem.id_dichiarazione, elem.id_dichiarazione), LogType.Errore)
                            EsitoSync.errors.Add(New SyncErrors() With {.code = "309", .msg = String.Format("Id_Appezzamento {1} - id_dichirazione {3} con validità sovrapposta rispetto id_dichiarazione {4}, acquisizione piano colturale per cuaa\campagna ({0}\{2}) non possibile", CUAA.payload.CUAA, elem.id_appezzamento_padre, CUAA.payload.Campagna.ToString(), sub_elem.id_dichiarazione, elem.id_dichiarazione), .key = elem.id_appezzamento_padre & "|" & elem.id_dichiarazione & "|" & sub_elem.id_dichiarazione})
                            ret = False
                        End If
                        If CheckDate(sub_elem.validita_inizio) = CheckDate(sub_elem.validita_fine) Then
                            _logger.AppendLog(CUAA, "pcg", String.Format("[{0} - {1} - {2}] Id_Appezzamento {3} - id_dichirazione {5} con date inizio/fine validità sovrapposte ({6}/{7}), acquisizione piano colturale per cuaa\campagna ({0}\{4}) non possibile", CUAA.payload.CUAA, CUAA.payload.Operazione, CUAA.payload.id_signal, sub_elem.id_appezzamento, CUAA.payload.Campagna.ToString(), sub_elem.id_dichiarazione, CheckDate(sub_elem.validita_inizio), CheckDate(sub_elem.validita_fine)), LogType.Errore)
                            EsitoSync.errors.Add(New SyncErrors() With {.code = "310", .msg = String.Format("Id_Appezzamento {1} - id_dichirazione {3} con date inizio/fine validità sovrapposte ({4}/{5}), acquisizione piano colturale per cuaa\campagna ({0}\{2}) non possibile", CUAA.payload.CUAA, sub_elem.id_appezzamento, CUAA.payload.Campagna.ToString(), sub_elem.id_dichiarazione, CheckDate(sub_elem.validita_inizio), CheckDate(sub_elem.validita_fine)), .key = sub_elem.id_appezzamento & "|" & sub_elem.id_dichiarazione})
                            ret = False
                        End If
                    Next
                Next
            Next
            date_check.Clear()
        Catch ex As Exception
            ret = False
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret

    End Function

    Public Shared Function ControlliPreCaricamentoEquipaggiamenti(ByVal CUAA As ObjNotifica(Of AgronicaCoreDTOStd.InData.Notifiche.CUAAObj),
                                                  ByVal equipaggiamenti As EquipaggiamentiCUAA,
                                                  ByRef ObjParametriServer As AgronicaCoreParametri,
                                                  ByRef _logger As LoggerManager,
                                                  ByRef EsitoSync As SyncAcknowledge) As Boolean
        Dim ret As Boolean = True
        Try
            If ControllaParametriEquipaggiamenti(CUAA, equipaggiamenti, ObjParametriServer, _logger, EsitoSync) = False Then
                ret = False
            End If

        Catch ex As Exception
            ret = False
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Private Shared Function ControllaParametriEquipaggiamenti(ByVal CUAA As ObjNotifica(Of AgronicaCoreDTOStd.InData.Notifiche.CUAAObj),
                                                              ByVal equipaggiamenti As EquipaggiamentiCUAA,
                                                              ByVal ObjParametriServer As AgronicaCoreParametri,
                                                              ByVal _logger As LoggerManager,
                                                              ByRef EsitoSync As SyncAcknowledge) As Boolean
        Dim ret As Boolean = True
        Try
            Dim xAgeaMacchine As New AgronicaCoreMetaSchemaDAL.Codifica_Macchine_Agea
            For Each equip In equipaggiamenti.records
                Dim class_code = xAgeaMacchine.Class_Cod_da_AGEA_Cod(ObjParametriServer, equip.elemento_anagrafico.tipo, "")

                If class_code = "" Then
                    _logger.AppendLog(CUAA, "equipaggiamenti", String.Format("[{0} - {1} - {2}] Creazione equipaggiamento ({3}) - Codice agea senza corrispondenza ClassCode Gias. Impossibile proseguire ({4})", CUAA.payload.CUAA, CUAA.payload.Operazione, CUAA.payload.id_signal, equip.codice, equip.elemento_anagrafico.tipo), LogType.Errore)
                    EsitoSync.errors.Add(New SyncErrors() With {.code = "701", .msg = "Creazione equipaggiamento ({3}) - Codice agea senza corrispondenza ClassCode Gias. Impossibile proseguire", .key = equip.codice})
                    ret = False
                End If
            Next
        Catch ex As Exception
            ret = False
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret

    End Function

    Public Shared Function ControlliPreCaricamentoLavoratori(ByVal CUAA As ObjNotifica(Of AgronicaCoreDTOStd.InData.Notifiche.CUAAObj),
                                                  ByVal lavoratori As LavoratoriCUAA,
                                                  ByRef ObjParametriServer As AgronicaCoreParametri,
                                                  ByRef _logger As LoggerManager,
                                                  ByRef EsitoSync As SyncAcknowledge) As Boolean
        Dim ret As Boolean = True
        Try
            If ControllaParametriLavoratori(CUAA, lavoratori, ObjParametriServer, _logger, EsitoSync) = False Then
                ret = False
            End If

        Catch ex As Exception
            ret = False
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Private Shared Function ControllaParametriLavoratori(ByVal CUAA As ObjNotifica(Of AgronicaCoreDTOStd.InData.Notifiche.CUAAObj),
                                                   ByVal lavoratori As LavoratoriCUAA,
                                                   ByVal ObjParametriServer As AgronicaCoreParametri,
                                                   ByVal _logger As LoggerManager,
                                                   ByRef EsitoSync As SyncAcknowledge) As Boolean
        Dim ret As Boolean = False
        Try
            'stub
            ret = True
        Catch ex As Exception
            ret = False
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret

    End Function


    Private Shared Function DecodeDate(ByVal daStr As String) As Date
        Dim ret = Convert.ToDateTime(daStr)
        ret = CheckDate(ret)
        Return ret
    End Function

    Private Shared Function CheckDate(ByVal data As Date) As Date

        If data < AGRODATAINIZIO Then
            data = AGRODATAINIZIO
        End If
        If data > AGRODATAFINE Then
            data = AGRODATAFINE
        End If
        Return data
    End Function

    Private Shared Function CheckCartography(ByVal CUAA As ObjNotifica(Of AgronicaCoreDTOStd.InData.Notifiche.CUAAObj),
                                             ByVal oriWKT As String,
                                             ByVal srOri As String,
                                             ByVal srDest As String,
                                             ByVal id_appezzamento_padre As String,
                                             ByVal id_appezzamento As String,
                                             ByVal logger As LoggerManager,
                                             ByRef ObjParametriServer As AgronicaCoreParametri,
                                             ByRef ReturnError As String,
                                             Optional ByVal ReplaceInvalidPolygon As Boolean = True,
                                             Optional ByVal DoReplaceInvalidPolygon As Boolean = True) As Boolean

        Dim ret As Boolean = False
        Try
            Dim wktParsed = GetCartography(CUAA, oriWKT, srOri, srDest, id_appezzamento_padre, id_appezzamento, logger, ObjParametriServer, ReplaceInvalidPolygon, DoReplaceInvalidPolygon)
            If wktParsed <> "" Then
                ret = True
            End If
        Catch ex As DataPublishException
            ReturnError = ex.Message
            ret = False
        Catch ex As Exception
            ReturnError = ex.Message
            ret = False
        End Try
        Return ret
    End Function

    Private Shared Function GetCartography(ByVal CUAA As ObjNotifica(Of AgronicaCoreDTOStd.InData.Notifiche.CUAAObj),
                                           ByVal oriWKT As String,
                                           ByVal srOri As String,
                                           ByVal srDest As String,
                                           ByVal id_appezzamento_padre As String,
                                           ByVal id_appezzamento As String,
                                           ByVal logger As LoggerManager,
                                           ByRef ObjParametriServer As AgronicaCoreParametri,
                                           Optional ByVal ReplaceInvalidPolygon As Boolean = True,
                                           Optional ByVal DoReplaceInvalidPolygon As Boolean = True,
                                           Optional ByVal LogVerbose As Boolean = False) As String


        Dim xTest As New AgronicaCoreGisDAL.GIS_ElementiGrafici_W
        Dim xPolygonHistoryW As New AgronicaCoreVarieBIZ.DataPublish_Poligoni_NonValidi_W
        Dim newWKTString As String = oriWKT
        Dim geoCheckStr = ""
        Try
            'Lavez - 27/05/2025 - Log verboso
            If LogVerbose Then
                logger.AppendLog(CUAA, "pcg", String.Format("[{0} - {1} - {2}] Inizio conversione poligono", CUAA.payload.CUAA, CUAA.payload.Operazione, CUAA.payload.id_signal), LogType.Informazione, bypassElastiSearch:=LogVerbose)
            End If

            Dim poligonoCorretto As Boolean = False
            Dim esitoOrientamento As Boolean = False

            Dim ntsGeoInstance = New NetTopologySuite.NtsGeometryServices()

            Dim oriGeoFact = ntsGeoInstance.CreateGeometryFactory(Integer.Parse(srOri.Replace("EPSG:", "")))
            Dim destGeoFact = ntsGeoInstance.CreateGeometryFactory(Integer.Parse(srDest.Replace("EPSG:", "")))


            Dim r As New NetTopologySuite.IO.WKTReader(ntsGeoInstance)
            Dim geocheck As Geometry = r.Read(oriWKT)

            geoCheckStr = geocheck.GeometryType.ToLower

            Select Case geocheck.GeometryType.ToLower
                Case "multipolygon"
                    Dim geom As MultiPolygon = geocheck

                    Dim polyList As New List(Of Polygon)

                    For Each geo In geom.Geometries
                        Dim g As Polygon = geo

                        'exterior ring
                        Dim Shell = destGeoFact.CreateLinearRing(GetTransformCoordinatesVectors(g.ExteriorRing.Coordinates, Integer.Parse(srOri.Replace("EPSG:", "")), Integer.Parse(srDest.Replace("EPSG:", ""))))

                        'internal rings
                        Dim Holes = New List(Of LinearRing)
                        For Each hole In g.Holes
                            Holes.Add(destGeoFact.CreateLinearRing(GetTransformCoordinatesVectors(hole.Coordinates, Integer.Parse(srOri.Replace("EPSG:", "")), Integer.Parse(srDest.Replace("EPSG:", "")))))
                        Next

                        Dim poly = destGeoFact.CreatePolygon(Shell, Holes.ToArray())

                        newWKTString = poly.ToText()

                        esitoOrientamento = xTest.TestaPoligonoWKTValid(newWKTString, ObjParametriServer, False)

                        If Not esitoOrientamento Then
                            newWKTString = AgronicaGIS2012.Commons.PolygonOrder.InvertiPoligono_wkt(newWKTString, False, False)
                            esitoOrientamento = xTest.TestaPoligonoWKTValid(newWKTString, ObjParametriServer, False)
                        End If
                        If Not esitoOrientamento Then
                            Throw New DataPublishException("[MULTIPOLYGON] Test poligono invertito non valido, decodica WKT fallita")
                        End If

                        polyList.Add(poly)
                    Next
                    newWKTString = ""
                    Dim mpoly = destGeoFact.CreateMultiPolygon(polyList.ToArray())
                    newWKTString = mpoly.ToText()

                    esitoOrientamento = xTest.TestaPoligonoWKTValid(newWKTString, ObjParametriServer, False)

                    If Not esitoOrientamento Then
                        Throw New DataPublishException("[MULTIPOLYGON] Test multi-poligono non valido, decodica WKT fallita")
                    End If
                Case "polygon"
                    Dim geom As Polygon = geocheck

                    Dim poly = destGeoFact.CreatePolygon(GetTransformCoordinatesVectors(geom.ExteriorRing.Coordinates, Integer.Parse(srOri.Replace("EPSG:", "")), Integer.Parse(srDest.Replace("EPSG:", ""))))

                    newWKTString = poly.ToText()

                    esitoOrientamento = xTest.TestaPoligonoWKTValid(newWKTString, ObjParametriServer, False)

                    If Not esitoOrientamento Then
                        newWKTString = AgronicaGIS2012.Commons.PolygonOrder.InvertiPoligono_wkt(newWKTString, False, False)
                        esitoOrientamento = xTest.TestaPoligonoWKTValid(newWKTString, ObjParametriServer, False)
                    End If
                    If Not esitoOrientamento Then
                        Throw New DataPublishException("[POLYGON] Test Poligono invertito non valido, decodifica WKT fallita")
                    End If
                Case Else
                    Throw New DataPublishException("Geometria [" & geocheck.GeometryType & "] non consentita in acquisizione piano colturale")
            End Select

        Catch ex As DataPublishException
            'Lavez - 14/03/2025 - Check di livello superiore per riabilitare il controllo della geometria in pre-elaborazione
            If DoReplaceInvalidPolygon Then
                'Lavez - 22/10/2024 - introdotta sostituzione con poligono arbitrario e registrazione del poligono originale in tabella apposita
                If ReplaceInvalidPolygon AndAlso {"multipolygon", "polygon"}.Contains(geoCheckStr) Then
                    logger.AppendLog(CUAA, "pcg", String.Format("[{0} - {1} - {2}] Chiavi: CUAA ({0}) - Campagna ({3}) - id_appezzamento_padre ({4}) - id_appezzamento ({5}) ESEGUITA SOSTITUZIONE CON POLIGONO ARBITRARIO" & vbCrLf & " {6} ", CUAA.payload.CUAA, CUAA.payload.Operazione, CUAA.payload.id_signal, CUAA.payload.Campagna, id_appezzamento_padre, id_appezzamento, ex.Message, LogType.Warning))

                    If xPolygonHistoryW.RegistraNuovoPoligonoInvalido(TipiEnumerativi.enum_SistemiEsterni.demetra, CUAA.payload.CUAA, CUAA.payload.Campagna, id_appezzamento, Integer.Parse(srOri.Replace("EPSG:", "")), oriWKT, ObjParametriServer) Then
                        newWKTString = CreateTrianglePolygon(newWKTString, geoCheckStr, logger, ObjParametriServer)
                    Else
                        'se qualcosa non funziona faccio scattare eccezzione così da intercettare il problema.
                        newWKTString = ""
                        Throw New DataPublishException(ex.Message, ex)
                    End If
                Else
                    newWKTString = ""
                    Throw New DataPublishException(ex.Message, ex)
                End If
            Else
                newWKTString = ""
                Throw New DataPublishException(ex.Message, ex)
            End If
        Catch ex As Exception
            newWKTString = ""
            Throw New Exception(ex.Message, ex)
        End Try
        'Lavez - 27/05/2025 - Log verboso
        If LogVerbose Then
            logger.AppendLog(CUAA, "pcg", String.Format("[{0} - {1} - {2}] fine conversione poligono", CUAA.payload.CUAA, CUAA.payload.Operazione, CUAA.payload.id_signal), LogType.Informazione, bypassElastiSearch:=LogVerbose)
        End If
        Return newWKTString
    End Function

    Private Shared Function CreateTrianglePolygon(ByVal newWKTString As String,
                                                  ByVal geometryType As String,
                                                  ByVal logger As LoggerManager,
                                                  ByRef ObjParametriServer As AgronicaCoreParametri) As String
        Dim retWKT As String = ""

        Dim esitoOrientamento As Boolean = False
        Dim xTest As New AgronicaCoreGisDAL.GIS_ElementiGrafici_W
        Dim pt As String()

        Select Case geometryType
            Case "multipolygon"
                Dim str As String = ""
                If newWKTString.Contains("MULTIPOLYGON (((") Then
                    str = newWKTString.Substring(0, newWKTString.IndexOf(",")).Replace("MULTIPOLYGON (((", "")
                Else
                    str = newWKTString.Substring(0, newWKTString.IndexOf(",")).Replace("POLYGON ((", "")
                End If
                pt = str.Split({" "}.ToArray(), StringSplitOptions.RemoveEmptyEntries)
                retWKT = GetTriangleMultiPolygon2(Double.Parse(pt(0).Replace(".", Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator)),
                                                  Double.Parse(pt(1).Replace(".", Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator)))
            Case "polygon"
                Dim str As String = newWKTString.Substring(0, newWKTString.IndexOf(",")).Replace("POLYGON ((", "")
                pt = str.Split({" "}.ToArray(), StringSplitOptions.RemoveEmptyEntries)
                retWKT = GetTrianglePolygon2(Double.Parse(pt(0).Replace(".", Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator)),
                                             Double.Parse(pt(1).Replace(".", Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator)))
            Case Else
                Throw New DataPublishException("Geometria [" & geometryType & "] non consentita in acquisizione piano colturale")
        End Select


        'rec.WKT = GetTrianglePolygon(Double.Parse(pt(0).Replace(".", CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator)),
        '                         Double.Parse(pt(1).Replace(".", CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator)), 90)


        esitoOrientamento = xTest.TestaPoligonoWKT(retWKT, ObjParametriServer)
        If Not esitoOrientamento Then
            retWKT = AgronicaGIS2012.Commons.PolygonOrder.InvertiPoligono_wkt(retWKT, False)
            esitoOrientamento = xTest.TestaPoligonoWKT(retWKT, ObjParametriServer)
        End If

        If esitoOrientamento = False Then
            retWKT = ""
            Throw New Exception("Poligono triangolo non valido.")
        End If

        Return retWKT
    End Function

    Private Shared Function GetTriangleMultiPolygon2(ByVal currlat As Double, ByVal currlng As Double) As String
        Dim ALat As Double
        Dim ALong As Double
        Dim BLat As Double
        Dim BLong As Double
        Dim CLat As Double
        Dim CLong As Double

        Dim distance As Double = 1500  '(0,001 circa 100 m)

        ALat = currlat
        ALong = currlng

        BLat = ALat + (0.0001 * (distance / 1000))
        BLong = ALong - 0.0002

        CLat = ALat + ((0.0001 * (distance / 1000)) * 2)
        CLong = ALong

        Return "MULTIPOLYGON (((" & ALat.ToString().Replace(",", ".") & " " & ALong.ToString().Replace(",", ".") & ", " & BLat.ToString().Replace(",", ".") & " " & BLong.ToString().Replace(",", ".") & ", " & CLat.ToString().Replace(",", ".") & " " & CLong.ToString().Replace(",", ".") & ", " & ALat.ToString().Replace(",", ".") & " " & ALong.ToString().Replace(",", ".") & ")))"

    End Function

    Private Shared Function GetTrianglePolygon2(ByVal currlat As Double, ByVal currlng As Double) As String
        Dim ALat As Double
        Dim ALong As Double
        Dim BLat As Double
        Dim BLong As Double
        Dim CLat As Double
        Dim CLong As Double

        Dim distance As Double = 1500  '(0,001 circa 100 m)

        ALat = currlat
        ALong = currlng

        BLat = ALat + (0.0001 * (distance / 1000))
        BLong = ALong - 0.0002

        CLat = ALat + ((0.0001 * (distance / 1000)) * 2)
        CLong = ALong

        Return "POLYGON ((" & ALat.ToString().Replace(",", ".") & " " & ALong.ToString().Replace(",", ".") & ", " & BLat.ToString().Replace(",", ".") & " " & BLong.ToString().Replace(",", ".") & ", " & CLat.ToString().Replace(",", ".") & " " & CLong.ToString().Replace(",", ".") & ", " & ALat.ToString().Replace(",", ".") & " " & ALong.ToString().Replace(",", ".") & "))"

    End Function


    Private Shared Function GetCoordinateSystemFromEPSG(ByVal id_code As Integer) As GeoAPI.CoordinateSystems.ICoordinateSystem
        Dim csfactory = New CoordinateSystemFactory()

        Select Case id_code
            Case 4326
                Return csfactory.CreateFromWkt(WGS_84)
            Case 32632
                Return csfactory.CreateFromWkt(UTWGS_84_UTM_zone_32N)
            Case 32633
                Return csfactory.CreateFromWkt(UTWGS_84_UTM_zone_33N)
            Case 3003
                Return csfactory.CreateFromWkt(MonteMario_Italy_Zone_1_3003)
            Case 3004
                Return csfactory.CreateFromWkt(MonteMario_Italy_Zone_2_3004)
            Case Else
                Throw New Exception("EPSG Code non mappato")
        End Select
    End Function

    Private Shared Function GetTransformCoordinatesVectors(ByVal coordinates As NetTopologySuite.Geometries.Coordinate(),
                                                           ByVal srOri As Integer,
                                                           ByVal srDest As Integer
                                                           ) As NetTopologySuite.Geometries.Coordinate()
        Dim ret As New List(Of NetTopologySuite.Geometries.Coordinate)
        Dim trf = New CoordinateTransformationFactory()

        Try
            Dim tr = trf.CreateFromCoordinateSystems(GetCoordinateSystemFromEPSG(srOri),
                                        GetCoordinateSystemFromEPSG(srDest))

            Dim transformedPoint = tr.MathTransform.TransformList(ConvCoordinateToPointList(coordinates))

            For Each point In transformedPoint
                ret.Add(New NetTopologySuite.Geometries.Coordinate With {
                            .X = point.X,
                            .Y = point.Y
                        })
            Next

        Catch ex As Exception
            ret = Nothing
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret.ToArray()

    End Function

    'Private Shared Function GetTransformCoordinatesVectors(ByVal coordinates As NetTopologySuite.Geometries.Coordinate(),
    '                                                       ByVal srOri As Integer,
    '                                                       ByVal srDest As Integer
    '                                                       ) As NetTopologySuite.Geometries.Coordinate()
    '    Dim ret As New List(Of NetTopologySuite.Geometries.Coordinate)
    '    Dim trf = New CoordinateTransformationFactory()

    '    Try
    '        Dim tr = trf.CreateFromCoordinateSystems(GetCoordinateSystemFromEPSG(srOri),
    '                                    GetCoordinateSystemFromEPSG(srDest))

    '        Dim transformedPoint = tr.MathTransform.TransformList(ConvCoordinateToPointList(coordinates))

    '        For Each point In transformedPoint
    '            ret.Add(New NetTopologySuite.Geometries.Coordinate With {
    '                        .X = point.X,
    '                        .Y = point.Y
    '                    })
    '        Next

    '    Catch ex As Exception
    '        ret = Nothing
    '        Throw New Exception(ex.Message, ex)
    '    End Try
    '    Return ret.ToArray()

    'End Function

    Private Shared Function ConvCoordinateToPointList(ByVal coordinates As NetTopologySuite.Geometries.Coordinate()) As List(Of GeoAPI.Geometries.Coordinate)
        Dim ret As New List(Of GeoAPI.Geometries.Coordinate)
        For Each point In coordinates
            ret.Add(New GeoAPI.Geometries.Coordinate(point.X, point.Y))
        Next
        Return ret
    End Function

    Private Shared Sub GmlRiproiettatoDatoWKT(wkt As String, wkt_georiferimento_cod As Integer,
                                              ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                              ByRef sFinalDoc1 As String,
                                              Optional ByVal ChiudiPoligono As Boolean = False)
        Dim ParametriCartografici As ParametriCoordinateConverter



        Dim leggiTrasformazione As New AgronicaCoreGisDAL.GIS_SistemiRiferimentoCartografia_R
        Dim dtLeggiTrasformazione As DataTable = leggiTrasformazione.Leggi(wkt_georiferimento_cod, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)


        ParametriCartografici = New ParametriCoordinateConverter With {
                .CSFromText = dtLeggiTrasformazione(0)("CSFrom"),
                .CStoText = dtLeggiTrasformazione(0)("CSTo"),
                .CStoGeoText = dtLeggiTrasformazione(0)("CStoGeo"),
                .AgronicaLatOffset = dtLeggiTrasformazione(0)("AgronicaLatOffset"),
                .AgronicaLonOffset = dtLeggiTrasformazione(0)("AgronicaLonOffset"),
                .LibreriaDaUsare = dtLeggiTrasformazione(0)("LibreriaDaUsare")
            }

        Dim wktHelp As New WKT
        Dim wktToGeoML As New wkt_gml
        Dim cconverter As New AgronicaConversioneCartografiaGias.Agronica.CoordinateConverter

        sFinalDoc1 = cconverter.WKTPolygonWGS84_from_WKTPolygonED50(wkt, False, ParametriCartografici, ChiudiPoligono)
    End Sub


    Private Shared Sub DecodificaCodiceAgea(ByVal codice_agea As String,
                                            ByRef veg_cod As Integer,
                                            ByRef id_cod As Integer,
                                            ByRef cul_cod As Integer,
                                            ByRef grfi_cod As Integer,
                                            ByRef Grva_Cod As Integer,
                                            ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                            Optional ByVal FilePrefix As String = "")


        Dim objUtilizzi As New AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Agea_2015_2020_R
        Dim objCultivar As New AgronicaCoreMetaSchemaDAL.Cultivar_R

        veg_cod = 0
        id_cod = 0
        cul_cod = 0
        grfi_cod = 0
        Grva_Cod = 0

        Dim cod_ColturaArr = codice_agea.Split("-")

        If cod_ColturaArr.Length < 5 Then
            Return
        End If

        Dim Veg_Cod_Agea As String = ""
        Dim Cul_Cod_Agea As String = cod_ColturaArr(4)
        Dim Uso_Cod_Agea As String = cod_ColturaArr(2)
        Dim Occupazione_Cod_Agea As String = cod_ColturaArr(0)
        Dim Destinazione_Cod_Agea As String = cod_ColturaArr(1)
        Dim Qualita_Cod_Agea As String = cod_ColturaArr(3)

        objUtilizzi.Specie_e_Varieta_Gias_Da_AGEA("",
                                            "",
                                            Veg_Cod_Agea,
                                            Cul_Cod_Agea,
                                            Uso_Cod_Agea,
                                            Occupazione_Cod_Agea,
                                            Destinazione_Cod_Agea,
                                            Qualita_Cod_Agea,
                                            veg_cod, cul_cod,
                                            grfi_cod,
                                            Grva_Cod,
                                            id_cod,
                                            "",
                                            "",
                                            objParametri_Server,
                                            "",
                                            "",
                                            FilePrefix)

        If CInt(veg_cod) <> 0 AndAlso CInt(cul_cod) = 0 Then
            cul_cod = objCultivar.VarietaAltre(veg_cod, objParametri_Server)
        End If
    End Sub

    Private Shared Sub MapCodiciChiaveAgeaAppezzamento(ByRef appRef As AgronicaCoreModelsSTD.anagrafiche.Appezzamento, ByVal chiave As AttributiChiaveAgeaAppezzamento)

        If chiave.idSchedaValidazione IsNot Nothing Then
            appRef.Agea_idSchedaValidazione = chiave.idSchedaValidazione
        End If

        If chiave.identificativoPianoColtivazione IsNot Nothing Then
            appRef.Agea_identificativoPianoColtivazione = chiave.identificativoPianoColtivazione
        End If

        'codiBarrSchedaValidazione
        If chiave.codiBarrSchedaValidazione IsNot Nothing Then
            appRef.Agea_codiBarrScheVali = chiave.codiBarrSchedaValidazione
        End If

        If chiave.identificativoIsola IsNot Nothing Then
            appRef.Agea_identificativoIsola = chiave.identificativoIsola
        End If

        If chiave.identificativoAppezzamento IsNot Nothing Then
            appRef.Agea_identificativoAppezzamento = chiave.identificativoAppezzamento
        End If

        If chiave.idAppezzamentoOrig IsNot Nothing Then
            appRef.Agea_idAppezzamentoOrig = chiave.idAppezzamentoOrig
        End If

    End Sub

    Private Shared Sub MapCodiciChiaveAgeaImpianto(ByRef impRef As AgronicaCoreModelsSTD.anagrafiche.Impianto, ByVal chiave As AttributiChiaveAgeaColtura)

        If chiave.idColt IsNot Nothing Then
            impRef.Agea_idColt = chiave.idColt
        End If

    End Sub

    Private Shared Sub MapDatiAggiuntiviPCGAgeaImpiantoEsercizio(ByRef impRef As AgronicaCoreModelsSTD.anagrafiche.Impianto,
                                                                 ByRef eseRef As AgronicaCoreModelsSTD.anagrafiche.Esercizio,
                                                                 ByVal dic As Coltura,
                                                                 ByVal veg_cod As Integer,
                                                                 ByVal cuaa As ObjNotifica(Of CUAAObj),
                                                                 ByRef _logger As LoggerManager,
                                                                 ByRef ObjParametri_Server As AgronicaCoreParametri,
                                                                 Optional ByVal LogVerbose As Boolean = False)

        If LogVerbose Then
            _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] inizio mappatura attributi aggiuntivi agea", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), LogType.Informazione, bypassElastiSearch:=LogVerbose)
        End If
        'lavez - 20240708 - rivista gestione
        If dic.datiAggiuntiviPCG.zootechnical_effluents IsNot Nothing Then
            If eseRef.codici Is Nothing Then
                eseRef.codici = New List(Of CodiciAnagrafeValori)
            End If
            Dim EseRefCodice = eseRef.codici.Where(Function(x) x.codiceAnagrafe.codice = CODICE_Codice_Agea_EffluentiZootecnici).ToList()
            If EseRefCodice.Count <= 0 OrElse EseRefCodice Is Nothing Then
                eseRef.codici.Add(New AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori() With {
                    .codiceAnagrafe = New AgronicaCoreModelsSTD.anagrafiche.CodiceAnagrafe(CODICE_Codice_Agea_EffluentiZootecnici),
                    .validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(),
                    .valore = getKeyValueToSearch(dic.datiAggiuntiviPCG.zootechnical_effluents.code, dic.datiAggiuntiviPCG.zootechnical_effluents.key)
                })
            Else
                eseRef.codici.RemoveAll(Function(x) x.codiceAnagrafe.codice = CODICE_Codice_Agea_EffluentiZootecnici)
                eseRef.codici.Add(New AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori() With {
                    .codiceAnagrafe = New AgronicaCoreModelsSTD.anagrafiche.CodiceAnagrafe(CODICE_Codice_Agea_EffluentiZootecnici),
                    .validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(),
                    .valore = getKeyValueToSearch(dic.datiAggiuntiviPCG.zootechnical_effluents.code, dic.datiAggiuntiviPCG.zootechnical_effluents.key)
                })
            End If
        Else
            If eseRef.codici IsNot Nothing Then
                If eseRef.codici.Where(Function(x) x.codiceAnagrafe.codice = CODICE_Codice_Agea_EffluentiZootecnici).ToList().Count > 0 Then
                    eseRef.codici.RemoveAll(Function(x) x.codiceAnagrafe.codice = CODICE_Codice_Agea_EffluentiZootecnici)
                End If
            End If
        End If

        'lavez - 20240708 - rivista gestione
        If dic.datiAggiuntiviPCG.irrigation_potential IsNot Nothing Then
            If eseRef.codici Is Nothing Then
                eseRef.codici = New List(Of CodiciAnagrafeValori)
            End If
            Dim EseRefCodice = eseRef.codici.Where(Function(x) x.codiceAnagrafe.codice = CODICE_Codice_flag_irriguo).ToList()
            If EseRefCodice.Count <= 0 OrElse EseRefCodice Is Nothing Then
                eseRef.codici.Add(New AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori() With {
                    .codiceAnagrafe = New AgronicaCoreModelsSTD.anagrafiche.CodiceAnagrafe(CODICE_Codice_flag_irriguo),
                    .validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(),
                    .valore = getKeyValueToSearch(dic.datiAggiuntiviPCG.irrigation_potential.code, dic.datiAggiuntiviPCG.irrigation_potential.key)
                                })
            Else
                eseRef.codici.RemoveAll(Function(x) x.codiceAnagrafe.codice = CODICE_Codice_flag_irriguo)
                eseRef.codici.Add(New AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori() With {
                    .codiceAnagrafe = New AgronicaCoreModelsSTD.anagrafiche.CodiceAnagrafe(CODICE_Codice_flag_irriguo),
                    .validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(),
                    .valore = getKeyValueToSearch(dic.datiAggiuntiviPCG.irrigation_potential.code, dic.datiAggiuntiviPCG.irrigation_potential.key)
                                })
            End If
        Else
            If eseRef.codici IsNot Nothing Then
                If eseRef.codici.Where(Function(x) x.codiceAnagrafe.codice = CODICE_Codice_flag_irriguo).ToList().Count > 0 Then
                    eseRef.codici.RemoveAll(Function(x) x.codiceAnagrafe.codice = CODICE_Codice_flag_irriguo)
                End If
            End If
        End If

        If dic.datiAggiuntiviPCG.irrigation_type IsNot Nothing Then
            If LogVerbose Then
                _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] prima di mappatura tipo irrigazione ", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), LogType.Informazione, bypassElastiSearch:=LogVerbose)
            End If
            impRef.irrigazione = New AgronicaCoreModelsSTD.metaschema.Irrigazione(GetTipoIrrigazioneCod(getKeyValueToSearch(dic.datiAggiuntiviPCG.irrigation_type.code, dic.datiAggiuntiviPCG.irrigation_type.key),
                                                                                                        cuaa,
                                                                                                        _logger,
                                                                                                        ObjParametri_Server))
            If LogVerbose Then
                _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] dopo mappatura tipo irrigazione ", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), LogType.Informazione, bypassElastiSearch:=LogVerbose)
            End If
        Else
            impRef.irrigazione = New AgronicaCoreModelsSTD.metaschema.Irrigazione(0)    'LL - 29/05/2024 - fix valorizzazione default
        End If

        If dic.datiAggiuntiviPCG.company_structures IsNot Nothing Then
            If LogVerbose Then
                _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] prima di mappatura copertura ", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), LogType.Informazione, bypassElastiSearch:=LogVerbose)
            End If
            impRef.copertura = New AgronicaCoreModelsSTD.metaschema.Copertura(GetCopertureCod(getKeyValueToSearch(dic.datiAggiuntiviPCG.company_structures.code, dic.datiAggiuntiviPCG.company_structures.key),
                                                                                                cuaa,
                                                                                                _logger,
                                                                                                ObjParametri_Server))
            If LogVerbose Then
                _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] dopo mappatura copertura ", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), LogType.Informazione, bypassElastiSearch:=LogVerbose)
            End If
        Else
            impRef.copertura = New AgronicaCoreModelsSTD.metaschema.Copertura(0)    'LL - 29/05/2024 - fix valorizzazione default
        End If

        'lavez - 20240708 - rivista gestione
        If dic.datiAggiuntiviPCG.sowing_type IsNot Nothing Then
            If eseRef.codici Is Nothing Then
                eseRef.codici = New List(Of CodiciAnagrafeValori)
            End If
            Dim EseRefCodice = eseRef.codici.Where(Function(x) x.codiceAnagrafe.codice = CODICE_Codice_Agea_TipoDiSemina).ToList()
            If EseRefCodice.Count <= 0 OrElse EseRefCodice Is Nothing Then
                eseRef.codici.Add(New AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori() With {
                    .codiceAnagrafe = New AgronicaCoreModelsSTD.anagrafiche.CodiceAnagrafe(CODICE_Codice_Agea_TipoDiSemina),
                    .validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(),
                    .valore = getKeyValueToSearch(dic.datiAggiuntiviPCG.sowing_type.code, dic.datiAggiuntiviPCG.sowing_type.key)
                })
            Else
                eseRef.codici.RemoveAll(Function(x) x.codiceAnagrafe.codice = CODICE_Codice_Agea_TipoDiSemina)
                eseRef.codici.Add(New AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori() With {
                    .codiceAnagrafe = New AgronicaCoreModelsSTD.anagrafiche.CodiceAnagrafe(CODICE_Codice_Agea_TipoDiSemina),
                    .validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(),
                    .valore = getKeyValueToSearch(dic.datiAggiuntiviPCG.sowing_type.code, dic.datiAggiuntiviPCG.sowing_type.key)
                })
            End If
        Else
            If eseRef.codici IsNot Nothing Then
                If eseRef.codici.Where(Function(x) x.codiceAnagrafe.codice = CODICE_Codice_Agea_TipoDiSemina).ToList().Count > 0 Then
                    eseRef.codici.RemoveAll(Function(x) x.codiceAnagrafe.codice = CODICE_Codice_Agea_TipoDiSemina)
                End If
            End If
        End If

        If LogVerbose Then
            _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] prima di fase ciclo specie vegetale", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), LogType.Informazione, bypassElastiSearch:=LogVerbose)
        End If
        If dic.datiAggiuntiviPCG.breeding_phase IsNot Nothing Then
            eseRef.apportiMassimiMacroelementi = New AgronicaCoreModelsSTD.metaschema.ApportoMacroelementi() With {
                .fase = New AgronicaCoreModelsSTD.metaschema.FaseCicloColturale() With {
                        .codice = Helper.getFaseCicloVitaSpecieVegetale(dic.datiAggiuntiviPCG.breeding_phase.code, veg_cod, dic.data_impianto, dic.data_inizio_validita, cuaa, _logger, ObjParametri_Server)
                    }
                }
        Else
            eseRef.apportiMassimiMacroelementi = New AgronicaCoreModelsSTD.metaschema.ApportoMacroelementi() With {
                .fase = New AgronicaCoreModelsSTD.metaschema.FaseCicloColturale() With {
                        .codice = Helper.getFaseCicloVitaSpecieVegetale("", veg_cod, dic.data_impianto, dic.data_inizio_validita, cuaa, _logger, ObjParametri_Server)  'external code non valorizzato, calcolo il default come da standard
                    }
                }
        End If
        If LogVerbose Then
            _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] dopo di fase ciclo specie vegetale", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), LogType.Informazione, bypassElastiSearch:=LogVerbose)
        End If

        If dic.datiAggiuntiviPCG.breeding_form IsNot Nothing Then
            If LogVerbose Then
                _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] prima di forma allevamento - breeding form", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), LogType.Informazione, bypassElastiSearch:=LogVerbose)
            End If
            impRef.formaAllevamento = New AgronicaCoreModelsSTD.metaschema.DensitaImpianto.FormaAllevamento(GetFormaAllevamentoCod(getKeyValueToSearch(dic.datiAggiuntiviPCG.breeding_form.code, dic.datiAggiuntiviPCG.breeding_form.key),
                                                                                                                                    cuaa,
                                                                                                                                    _logger,
                                                                                                                                    ObjParametri_Server))
            If LogVerbose Then
                _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] dopo forma allevamento - breeding form", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), LogType.Informazione, bypassElastiSearch:=LogVerbose)
            End If
        End If
        If dic.datiAggiuntiviPCG.breeding_type IsNot Nothing Then
            If LogVerbose Then
                _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] prima di forma allevamento - breeding type", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), LogType.Informazione, bypassElastiSearch:=LogVerbose)
            End If
            impRef.formaAllevamento = New AgronicaCoreModelsSTD.metaschema.DensitaImpianto.FormaAllevamento(GetFormaAllevamentoCod(getKeyValueToSearch(dic.datiAggiuntiviPCG.breeding_type.code, dic.datiAggiuntiviPCG.breeding_type.key),
                                                                                                                        cuaa,
                                                                                                                        _logger,
                                                                                                                        ObjParametri_Server))
            If LogVerbose Then
                _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] dopo forma allevamento - breeding type", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), LogType.Informazione, bypassElastiSearch:=LogVerbose)
            End If
        End If

        If dic.datiAggiuntiviPCG.breeding_form Is Nothing AndAlso dic.datiAggiuntiviPCG.breeding_type Is Nothing Then
            impRef.formaAllevamento = New AgronicaCoreModelsSTD.metaschema.DensitaImpianto.FormaAllevamento(0)    'LL - 29/05/2024 - fix valorizzazione default
        End If

        If dic.datiAggiuntiviPCG.year_month IsNot Nothing Then
            Dim pDate As Date
            Dim chkDate = Date.TryParse(dic.datiAggiuntiviPCG.year_month, pDate)
            If chkDate = True Then
                impRef.data_Inizio_Impianto = pDate
            End If
        Else
            impRef.data_Inizio_Impianto = AGRODATAINIZIO    'LL - 29/05/2024 - fix valorizzazione default
        End If
        If LogVerbose Then
            _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] fine mappatura attributi aggiuntivi agea", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), LogType.Informazione, bypassElastiSearch:=LogVerbose)
        End If
    End Sub

    Private Shared Function GetTipoIrrigazioneCod(ByVal code As String,
                                                  ByVal cuaa As ObjNotifica(Of CUAAObj),
                                                  ByRef _logger As LoggerManager,
                                                  ByRef ObjParametri_Server As AgronicaCoreParametri) As Integer
        Dim ret As Integer = -1
        Try
            Dim decode_dal As New AgronicaCoreMetaSchemaDAL.Codifica_ImpiantiIrrigui_SistemiEsterni_R

            Dim res = decode_dal.Leggi(TipiEnumerativi.enum_SistemiEsterni.agea, code, 0, "", "", ObjParametri_Server)
            If res.Rows.Count <= 0 Then
                Throw New Exception(String.Format("Nessuna decodicifica trovata per il codice {0}", code))
            Else
                ret = res.Rows(0)("Imp_Cod")
            End If
        Catch ex As Exception
            _logger.AppendLog(cuaa, "pcg", "Errore decodifica proprieta agea irrigation_type " & ex.Message & If(ex.InnerException IsNot Nothing, ex.InnerException.Message, ""), LogType.Errore, ex)
            ret = -1
        End Try
        Return ret

    End Function

    Private Shared Function GetCopertureCod(ByVal code As String,
                                              ByVal cuaa As ObjNotifica(Of CUAAObj),
                                              ByRef _logger As LoggerManager,
                                              ByRef ObjParametri_Server As AgronicaCoreParametri) As Integer
        Dim ret As Integer = -1
        Try
            Dim decode_dal As New AgronicaCoreMetaSchemaDAL.Codifica_Coperture_SistemiEsterni_R

            Dim res = decode_dal.Leggi(TipiEnumerativi.enum_SistemiEsterni.agea, code, 0, "", "", ObjParametri_Server)
            If res.Rows.Count <= 0 Then
                Throw New Exception(String.Format("Nessuna decodicifica trovata per il codice {0}", code))
            Else
                ret = res.Rows(0)("Cop_Cod")
            End If
        Catch ex As Exception
            _logger.AppendLog(cuaa, "pcg", "Errore decodifica proprieta agea company_structures " & ex.Message & If(ex.InnerException IsNot Nothing, ex.InnerException.Message, ""), LogType.Errore, ex)
            ret = -1
        End Try
        Return ret

    End Function

    Private Shared Function GetFormaAllevamentoCod(ByVal code As String,
                                                  ByVal cuaa As ObjNotifica(Of CUAAObj),
                                                  ByRef _logger As LoggerManager,
                                                  ByRef ObjParametri_Server As AgronicaCoreParametri) As Integer
        Dim ret As Integer = -1
        Try
            Dim decode_dal As New AgronicaCoreMetaSchemaDAL.Codifica_FormeAllevamento_SistemiEsterni_R

            Dim res = decode_dal.Leggi(0, TipiEnumerativi.enum_SistemiEsterni.agea, code, 0, AGRODATAINIZIO, AGRODATAFINE, "", "", ObjParametri_Server)
            If res.Rows.Count <= 0 Then
                Throw New Exception(String.Format("Nessuna decodicifica trovata per il codice {0}", code))
            Else
                ret = res.Rows(0)("Foral_Cod")
            End If
        Catch ex As Exception
            _logger.AppendLog(cuaa, "pcg", "Errore decodifica proprieta agea breeding_form " & ex.Message & If(ex.InnerException IsNot Nothing, ex.InnerException.Message, ""), LogType.Errore, ex)
            ret = -1
        End Try
        Return ret

    End Function

    Private Shared Function getKeyValueToSearch(ByVal code As String,
                                                ByVal key As String) As String

        If (code Is Nothing AndAlso key Is Nothing) Then
            Return ""
        ElseIf (code Is Nothing AndAlso key IsNot Nothing) Then
            Return key
        ElseIf (code IsNot Nothing AndAlso key Is Nothing) Then
            Return code
        Else
            Return code & "|" & key
        End If

    End Function

    Private Shared Function getFaseCicloVitaSpecieVegetale(ByVal external_code As String,
                                                           ByVal veg_cod As Integer,
                                                           ByVal data_inizio_impianto As Date,
                                                           ByVal data_inizio_validita As Date,
                                                           ByVal cuaa As ObjNotifica(Of CUAAObj),
                                                           ByRef _logger As LoggerManager,
                                                           ByRef ObjParametri_Server As AgronicaCoreParametri) As Integer
        Dim ret As Integer = 21     'Non produttivo

        ' solo se ho il codice specie valorizzato ed il codice esterno è diverso da 70021 (NON PRODUTTIVO)
        If veg_cod <> 0 AndAlso external_code <> "70021" Then

            Dim xSpecieVegR As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
            'Dim xFasiCicloR As New AgronicaCoreMetaSchemaDAL.PC_FasiCicloColturalexGruppoVegetale_R
            Try
                Dim dtSpeVeg = xSpecieVegR.Leggi(veg_cod, 0, "", "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", ObjParametri_Server)
                If dtSpeVeg.Rows.Count > 0 Then
                    'Lavez - 28/05/2024 - come concordato con Federica Monti
                    ' Se arborea  = 4
                    ' Se orticola o erbacea = 17
                    Select Case CInt(dtSpeVeg.Rows(0)("Gru_Cod"))
                        Case 1
                            'Arboree
                            ret = 4
                        Case 2, 3
                            'Erbacee\Orticole
                            ret = 17
                    End Select
                End If
            Catch ex As Exception
                _logger.AppendLog(cuaa, "pcg", "Errore decodifica proprieta agea breeding_phase " & ex.Message & If(ex.InnerException IsNot Nothing, ex.InnerException.Message, ""), LogType.Errore, ex)
                ret = 21
            End Try
        End If
        Return ret
    End Function

    Private Shared Sub GetCodIstatFromCodNazionale(ByVal cod_nazionale As String,
                                                   ByVal cuaa As ObjNotifica(Of CUAAObj),
                                                   ByRef _logger As LoggerManager,
                                                   ByRef ObjParametri_Server As AgronicaCoreParametri,
                                                   ByRef CodProv As String,
                                                   ByRef CodCom As String,
                                                   ByRef CAP As String,
                                                   ByRef Nazione As String)
        Dim xRead As New AgronicaCoreMetaSchemaDAL.Istat_R
        CodCom = ""
        CodProv = ""
        CAP = ""
        Nazione = ""
        Dim COM As String = ""
        Dim PROV As String = ""
        Dim Sezione As String = ""

        Try
            xRead.CodIstat_from_Cod_Nazionale(CodProv, CodCom, PROV, COM, Sezione, CAP, Nazione, cod_nazionale, ObjParametri_Server)
        Catch ex As Exception
            _logger.AppendLog(cuaa, "pcg", "Errore decodifica codice nazionale per recupero indirizzo appezzamento " & ex.Message & If(ex.InnerException IsNot Nothing, ex.InnerException.Message, ""), LogType.Errore, ex)
        End Try


    End Sub

    Private Shared Sub ImpostaAppezzamentoIndirizzoCreazione(ByVal cuaa As ObjNotifica(Of CUAAObj),
                                         ByVal pcg As Appezzamento,
                                         ByRef objAppezza As AgronicaCoreModelsSTD.anagrafiche.Appezzamento,
                                         ByRef ObjParametriServer As AgronicaCoreParametri,
                                         ByRef _logger As LoggerManager)
        If (pcg.cod_nazionale IsNot Nothing OrElse pcg.cod_nazionale <> "") AndAlso (objAppezza.indirizzi Is Nothing OrElse objAppezza.indirizzi.Count = 0) Then
            Dim CodProv As String = ""
            Dim CodCom As String = ""
            Dim CAP As String = ""
            Dim Nazione As String = ""

            GetCodIstatFromCodNazionale(pcg.cod_nazionale,
                                    cuaa,
                                    _logger,
                                    ObjParametriServer,
                                    CodProv,
                                    CodCom,
                                    CAP,
                                    Nazione)

            objAppezza.indirizzi = New List(Of AgronicaCoreModelsSTD.anagrafiche.IndirizzoAssociato)
            objAppezza.indirizzi.Add(New AgronicaCoreModelsSTD.anagrafiche.IndirizzoAssociato() With {
                            .tipo_Indirizzo = 1,
                            .indirizzo = New AgronicaCoreModelsSTD.anagrafiche.Indirizzo() With {
                                .istatComune = New AgronicaCoreModelsSTD.metaschema.Istat() With {
                                                .prov = If(CodProv = "", "000", CodProv),
                                                .com = If(CodCom = "", "000", CodCom)
                                            },
                                    .cap = If(CAP = "", "00000", CAP),
                                    .stato = New AgronicaCoreModelsSTD.metaschema.CodiciNazioniISO3166() With {
                                                .codice = If(Nazione = "", "IT", Nazione)
                                            },
                                    .codice = 0,
                                    .frazione = "",
                                    .note = "",
                                    .via = "#"
                            }
                            })
        End If
    End Sub

    Private Shared Sub ImpostaAppezzamentoIndirizzoModifica(ByVal cuaa As ObjNotifica(Of CUAAObj),
                                     ByVal pcg As AppezzamentoSync,
                                     ByRef objAppezza As AgronicaCoreModelsSTD.anagrafiche.Appezzamento,
                                     ByRef ObjParametriServer As AgronicaCoreParametri,
                                     ByRef _logger As LoggerManager)
        If (pcg.cod_nazionale IsNot Nothing OrElse pcg.cod_nazionale <> "") AndAlso (objAppezza.indirizzi Is Nothing OrElse objAppezza.indirizzi.Count = 0) Then
            Dim CodProv As String = ""
            Dim CodCom As String = ""
            Dim CAP As String = ""
            Dim Nazione As String = ""

            GetCodIstatFromCodNazionale(pcg.cod_nazionale,
                                    cuaa,
                                    _logger,
                                    ObjParametriServer,
                                    CodProv,
                                    CodCom,
                                    CAP,
                                    Nazione)

            objAppezza.indirizzi = New List(Of AgronicaCoreModelsSTD.anagrafiche.IndirizzoAssociato)
            objAppezza.indirizzi.Add(New AgronicaCoreModelsSTD.anagrafiche.IndirizzoAssociato() With {
                            .tipo_Indirizzo = 1,
                            .indirizzo = New AgronicaCoreModelsSTD.anagrafiche.Indirizzo() With {
                                .istatComune = New AgronicaCoreModelsSTD.metaschema.Istat() With {
                                                .prov = If(CodProv = "", "000", CodProv),
                                                .com = If(CodCom = "", "000", CodCom)
                                            },
                                    .cap = If(CAP = "", "00000", CAP),
                                    .stato = New AgronicaCoreModelsSTD.metaschema.CodiciNazioniISO3166() With {
                                                .codice = If(Nazione = "", "IT", Nazione)
                                            },
                                    .codice = 0,
                                    .frazione = "",
                                    .note = "",
                                    .via = "#"
                            }
                            })
        End If
    End Sub

End Class
