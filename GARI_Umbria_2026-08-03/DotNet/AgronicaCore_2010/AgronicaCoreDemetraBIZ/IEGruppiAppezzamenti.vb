Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports Newtonsoft.Json
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.metaschema
Imports AgronicaCoreModelsSTD.baseClass
Imports AgronicaCoreDataProvider
Imports AgronicaCoreAnagrafeBIZ

Public Class IEGruppiAppezzamenti 'Import Export

    Private Const NOTELOG_IMPORT_CAMPI As String = "Operazione registrata da Import Campi Data Publish"

    Private _objParametriServer As AgronicaCoreParametri
    Private _objParametriUtenti As AgronicaCoreParametri

    Public Sub New(ByRef objParametriServer As AgronicaCoreParametri,
                   ByRef objParametriUtenti As AgronicaCoreParametri)

        If objParametriServer Is Nothing Then
            Throw New Exception("Valorizzare objParametriServer....")
        End If

        If objParametriUtenti Is Nothing Then
            Throw New Exception("Valorizzare objParametriUtenti....")
        End If

        _objParametriServer = objParametriServer
        _objParametriUtenti = objParametriUtenti


    End Sub

    Public Function ReadCampoFromCodiceEsterno(ByVal codice_esterno As String,
                                               ByVal piva As String,
                                               ByVal saCod As Integer,
                                               Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing) As Campo

        Dim nomeRoutine As String = "AgronicaCoreDemetraBIZ.IEGruppiAppezzamenti.ReadCampoFromCodiceEsterno()"
        Dim campo As Campo = Nothing
        Dim bCloseContext As Boolean = False
        Try

            If GiasContext Is Nothing Then
                GiasContext = Gias_EF_Utility.CreateGiasContextConnection(_objParametriServer.StringaConnessione)
                bCloseContext = True
            End If

            Dim interscambioCampi_R As New AgronicaCoreInterscambioDAL.Interscambio_Campi_R
            Dim interscambio = interscambioCampi_R.GetInterscambioGruppiAppezzamentiEF(codice_esterno, enum_SistemiEsterni.demetra, GiasContext)
            If interscambio IsNot Nothing Then
                Dim objCampoR As New Campo_R
                campo = objCampoR.Leggi_Campo(piva, saCod, interscambio.Campo_Cod, _objParametriServer)
            End If
        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message, ex)
        Finally
            If bCloseContext Then
                GiasContext.Dispose()
            End If
        End Try

        Return campo

    End Function

    Public Function ReadCampoFromCampoCod(ByVal campo_cod As Integer,
                                          ByVal piva As String,
                                          ByVal saCod As Integer
                                          ) As Campo

        Dim nomeRoutine As String = "AgronicaCoreDemetraBIZ.IEGruppiAppezzamenti.ReadCampoFromCampoCod()"
        Dim campo As Campo = Nothing
        Dim bCloseContext As Boolean = False

        Try
            Dim objCampoR As New Campo_R
            campo = objCampoR.Leggi_Campo(piva, saCod, campo_cod, _objParametriServer)
        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message, ex)
        End Try

        Return campo

    End Function

    Public Function ImportCampo(ByRef objCampo As Campo,
                                ByVal tipoOperazione As enum_TipoOperazioneDB,
                                ByVal codiceSistemaEsterno As Integer,
                                ByVal codiceEsterno As String,
                                Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                Optional NoteLog As String = NOTELOG_IMPORT_CAMPI) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreDemetraBIZ.IEGruppiAppezzamenti.ImportCampo()"
        Dim ret As Boolean = True

        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(_objParametriServer.StringaConnessione)
            bCloseContext = True
        End If

        Dim objCampoW As New Campo_W
        Dim interscambioCampi_W As New AgronicaCoreInterscambioDAL.Interscambio_Campi_W

        Try

            Select Case tipoOperazione
                Case enum_TipoOperazioneDB.Scrittura
                    objCampoW.Scrivi_Campo_Anagrafica(objCampo, tipoOperazione, _objParametriServer, _objParametriUtenti, NoteLog)

                    interscambioCampi_W.PrepareCreateInterscambioEF(objCampo,
                                                                  codiceEsterno,
                                                                  codiceSistemaEsterno,
                                                                  GiasContext,
                                                                  _objParametriServer,
                                                                  _objParametriUtenti,
                                                                  False)
                Case enum_TipoOperazioneDB.Modifica
                    objCampoW.Scrivi_Campo_Anagrafica(objCampo, tipoOperazione, _objParametriServer, _objParametriUtenti, NoteLog)

                    interscambioCampi_W.PrepareEditInterscambioEF(objCampo,
                                                                codiceEsterno,
                                                                codiceSistemaEsterno,
                                                                GiasContext,
                                                                _objParametriServer,
                                                                _objParametriUtenti,
                                                                False)


                Case enum_TipoOperazioneDB.Cancellazione

                    'Todo verifica cancellazione campo ma non appezzamenti, che devono solo essere disaggregati
                    objCampoW.Scrivi_Campo_Anagrafica(objCampo, tipoOperazione, _objParametriServer, _objParametriUtenti, NoteLog, AppezzaDisaggregaSoloInCancellazione:=True)

                    interscambioCampi_W.PrepareDeleteInterscambioEF(objCampo,
                                                                   codiceEsterno,
                                                                   codiceSistemaEsterno,
                                                                   GiasContext,
                                                                   _objParametriServer,
                                                                   _objParametriUtenti,
                                                                   False)

            End Select

            GiasContext.SaveChanges()
        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message, ex)
        Finally
            If bCloseContext Then
                GiasContext.Dispose()
            End If
        End Try

        Return ret

    End Function

End Class
