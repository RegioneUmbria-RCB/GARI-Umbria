Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreContabDAL
Imports Newtonsoft.Json
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.metaschema
Imports AgronicaCoreModelsSTD.baseClass
Imports AgronicaCoreDataProvider
Imports AgronicaCoreContabBIZ
Imports AgronicaCoreModelsSTD.exceptions

Public Class ImportParcoMacchine
    Inherits AgronicaCoreDataProvider.LogProvider

#Region "ParcoMacchine_D2G"

    Private Function ExistsMacchinaD2G(
                                      ByVal macchina As ParcoMacchine,
                                      ByVal codice_esterno As String,
                                      ByRef GiasContext As Gias_DeveloperServer_Entities
                                      ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreDemetraBIZ.ImportParcoMacchine.ExistsMacchinaD2G()"

        Try
            Dim parco_macchine = (From mac In GiasContext.Parco_Macchine
                                  Where mac.Piva = macchina.partitaIva AndAlso
                                      mac.Mac_Cod = macchina.codice
                                  Select mac).FirstOrDefault

            Dim interscambioPC_R As New AgronicaCoreInterscambioBIZ.Interscambio_Parco_Macchine_R
            Dim interscambio = interscambioPC_R.GetInterscambioParcoMacchine(codice_esterno, enum_SistemiEsterni.demetra, GiasContext, macchina.codice)

            If parco_macchine IsNot Nothing AndAlso interscambio IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            Dim MessaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore, ex)
        End Try

    End Function

    Private Function ExistsInterscambioD2G(
                                          ByVal codice_esterno As String,
                                          ByRef GiasContext As Gias_DeveloperServer_Entities
                                          ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreDemetraBIZ.ImportParcoMacchine.ExistsInterscambioD2G()"

        Try
            Dim interscambioPC_R As New AgronicaCoreInterscambioBIZ.Interscambio_Parco_Macchine_R
            Dim interscambio = interscambioPC_R.GetInterscambioParcoMacchine(codice_esterno, enum_SistemiEsterni.demetra, GiasContext)

            If interscambio IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            Dim MessaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore, ex)
        End Try

    End Function

    Public Function ReadParcoMacchineFromCodiceEsterno(
                                                      ByVal codice_esterno As String,
                                                      ByVal piva As String,
                                                      Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                                      Optional ByVal objParametriServer As AgronicaCoreParametri = Nothing,
                                                      Optional ByVal objParametriUtenti As AgronicaCoreParametri = Nothing
                                                      ) As ParcoMacchine

        Dim nomeRoutine As String = "AgronicaCoreDemetraBIZ.ImportParcoMacchine.ReadParcoMacchineFromCodiceEsterno()"
        Dim parcoMacchine As ParcoMacchine = Nothing
        Dim bCloseContext As Boolean = False

        If objParametriServer Is Nothing Then
            Throw New Exception("Valorizzare objParametriServer....")
        End If

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametriServer.StringaConnessione)
            bCloseContext = True
        End If

        Try
            Dim interscambioPC_R As New AgronicaCoreInterscambioBIZ.Interscambio_Parco_Macchine_R
            Dim interscambio = interscambioPC_R.GetInterscambioParcoMacchine(codice_esterno, enum_SistemiEsterni.demetra, GiasContext)
            If interscambio IsNot Nothing Then
                Dim objParcoMacchineR As New AgronicaCoreContabBIZ.Parco_Macchine_R

                If objParcoMacchineR.Exists_Macchina(piva, interscambio.Mac_Cod, objParametriServer) Then
                    parcoMacchine = objParcoMacchineR.Leggi_Macchina(piva, interscambio.Mac_Cod, objParametriServer)
                End If
            End If
        Catch ex As Exception
            parcoMacchine = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message, ex)
        Finally
            If bCloseContext Then
                GiasContext.Dispose()
            End If
        End Try

        Return parcoMacchine

    End Function

    Public Function ReadParcoMacchineFromMacCod(ByVal mac_cod As String,
                                                ByVal piva As String,
                                                Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                                Optional ByVal objParametriServer As AgronicaCoreParametri = Nothing,
                                                Optional ByVal objParametriUtenti As AgronicaCoreParametri = Nothing
                                                ) As ParcoMacchine

        Dim nomeRoutine As String = "AgronicaCoreDemetraBIZ.ImportParcoMacchine.ReadParcoMacchineFromMacCod()"
        Dim parcoMacchine As ParcoMacchine = Nothing
        Dim bCloseContext As Boolean = False

        If objParametriServer Is Nothing Then
            Throw New Exception("Valorizzare objParametriServer....")
        End If

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametriServer.StringaConnessione)
            bCloseContext = True
        End If

        Try
            Dim objParcoMacchineR As New AgronicaCoreContabBIZ.Parco_Macchine_R

            If objParcoMacchineR.Exists_Macchina(piva, mac_cod, objParametriServer) Then
                parcoMacchine = objParcoMacchineR.Leggi_Macchina(piva, mac_cod, objParametriServer)
            End If
        Catch ex As Exception
            parcoMacchine = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message, ex)
        Finally
            If bCloseContext Then
                GiasContext.Dispose()
            End If
        End Try

        Return parcoMacchine

    End Function

    Public Sub CreateMacchineD2G(ByVal cuaa As String,
                                ByVal macchine As Dictionary(Of String, ParcoMacchine),
                                ByRef errors As System.Collections.Concurrent.ConcurrentDictionary(Of Tuple(Of String, Integer), String),
                                ByRef successes As System.Collections.Concurrent.ConcurrentBag(Of Tuple(Of String, ParcoMacchine)),
                                ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                ByVal objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim nomeRoutine As String = "AgronicaCoreDemetraBIZ.ImportParcoMacchine.CreateMacchineD2G()"

        Dim GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri_Server.StringaConnessione)

        'Threading.Tasks.Parallel.ForEach(macchine, Function(mac)
        '                                               Dim parcoMacchine As ParcoMacchine

        '                                               Try
        '                                                   parcoMacchine = prepareCreateMacchinaD2G(mac.Value, mac.Key, GiasContext, objParametri_Server, objParametri_Utenti)
        '                                               Catch ex As Exception
        '                                                   errors.TryAdd(New Tuple(Of String, Integer)(mac.Key, mac.Value.codice), ex.Message)
        '                                               End Try

        '                                               successes.Add(New Tuple(Of String, ParcoMacchine)(mac.Key, parcoMacchine))
        '                                           End Function)

        For Each mac In macchine
            Dim parcoMacchine As ParcoMacchine

            Try
                parcoMacchine = PrepareCreateMacchinaD2G(cuaa, mac.Value, mac.Key, GiasContext, objParametri_Server, objParametri_Utenti)
                successes.Add(New Tuple(Of String, ParcoMacchine)(mac.Key, parcoMacchine))
            Catch ex As Exception
                errors.TryAdd(New Tuple(Of String, Integer)(mac.Key, mac.Value.codice), ex.Message)
            End Try


        Next

        GiasContext.SaveChanges()
        GiasContext.Dispose()

    End Sub

    Public Sub EditMacchineD2G(ByVal cuaa As String,
                              ByVal macchine As Dictionary(Of String, ParcoMacchine),
                              ByRef errors As System.Collections.Concurrent.ConcurrentDictionary(Of Tuple(Of String, Integer), String),
                              ByRef successes As System.Collections.Concurrent.ConcurrentBag(Of Tuple(Of String, ParcoMacchine)),
                              ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                              ByVal objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                              )

        Dim nomeRoutine As String = "AgronicaCoreDemetraBIZ.ImportParcoMacchine.EditMacchineD2G()"

        Dim GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri_Server.StringaConnessione)

        'Threading.Tasks.Parallel.ForEach(macchine, Function(mac)
        '                                               Dim parcoMacchine As ParcoMacchine

        '                                               Try
        '                                                   parcoMacchine = prepareEditMacchinaD2G(mac.Value, mac.Key, GiasContext, objParametri_Server, objParametri_Utenti)
        '                                               Catch ex As Exception
        '                                                   errors.TryAdd(New Tuple(Of String, Integer)(mac.Key, mac.Value.codice), ex.Message)
        '                                               End Try

        '                                               successes.Add(New Tuple(Of String, ParcoMacchine)(mac.Key, parcoMacchine))
        '                                           End Function)

        For Each mac In macchine
            Dim parcoMacchine As ParcoMacchine

            Try
                parcoMacchine = PrepareEditMacchinaD2G(cuaa, mac.Value, mac.Key, GiasContext, objParametri_Server, objParametri_Utenti)
                successes.Add(New Tuple(Of String, ParcoMacchine)(mac.Key, parcoMacchine))
            Catch ex As Exception
                errors.TryAdd(New Tuple(Of String, Integer)(mac.Key, mac.Value.codice), ex.Message)
            End Try


        Next

        GiasContext.SaveChanges()
        GiasContext.Dispose()

    End Sub

    Public Sub DeleteMacchineD2G(ByVal cuaa As String,
                                ByVal macchine As Dictionary(Of String, ParcoMacchine),
                                ByRef errors As System.Collections.Concurrent.ConcurrentDictionary(Of Tuple(Of String, Integer), String),
                                ByRef successes As System.Collections.Concurrent.ConcurrentBag(Of Tuple(Of String, ParcoMacchine)),
                                ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                ByVal objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim nomeRoutine As String = "AgronicaCoreDemetraBIZ.ImportParcoMacchine.DeleteMacchineD2G()"

        Dim GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri_Server.StringaConnessione)

        'Threading.Tasks.Parallel.ForEach(macchine, Function(mac)
        '                                               Dim parcoMacchine As ParcoMacchine

        '                                               Try
        '                                                   parcoMacchine = prepareDeleteMacchinaD2G(mac.Value, mac.Key, GiasContext, objParametri_Server, objParametri_Utenti)
        '                                               Catch ex As Exception
        '                                                   errors.TryAdd(New Tuple(Of String, Integer)(mac.Key, mac.Value.codice), ex.Message)
        '                                               End Try

        '                                               successes.Add(New Tuple(Of String, ParcoMacchine)(mac.Key, parcoMacchine))
        '                                           End Function)

        For Each mac In macchine
            Dim parcoMacchine As ParcoMacchine

            Try
                parcoMacchine = PrepareDeleteMacchinaD2G(cuaa, mac.Value, mac.Key, GiasContext, objParametri_Server, objParametri_Utenti)
                successes.Add(New Tuple(Of String, ParcoMacchine)(mac.Key, parcoMacchine))
            Catch ex As Exception
                errors.TryAdd(New Tuple(Of String, Integer)(mac.Key, mac.Value.codice), ex.Message)
            End Try


        Next

        GiasContext.SaveChanges()
        GiasContext.Dispose()

    End Sub

    Private Function PrepareCreateMacchinaD2G(ByVal cuaa As String,
                                             ByVal macchina As ParcoMacchine,
                                             ByVal codice_esterno As String,
                                             ByRef GiasContext As Gias_DeveloperServer_Entities,
                                             ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                             ByVal objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                             ) As ParcoMacchine

        Dim nomeRoutine As String = "AgronicaCoreDemetraBIZ.ImportParcoMacchine.PrepareCreateMacchinaD2G()"
        Dim parcoMacchine As Parco_Macchine

        Try
            If ExistsInterscambioD2G(codice_esterno, GiasContext) Then
                Throw New Exception("Un import di macchina con CodiceEsterno = " & codice_esterno & " esiste già")
            Else
                Dim objEFMacchine As New EFMacchine
                parcoMacchine = objEFMacchine.PrepareCreateParcoMacchineD2G(macchina, codice_esterno, GiasContext, objParametri_Server, objParametri_Utenti)

                'Lavez - 30/07/2024 - numero certificato taratura
                If macchina.codice <> 0 Then
                    Dim objEFMacchineCodici As New EFParcoMacchineCodici
                    If objEFMacchineCodici.Retrive(macchina, enum_CodiciAnagrafe.MacchinaCertificazioneTaratura, GiasContext) Is Nothing Then
                        objEFMacchineCodici.Write(macchina, objParametri_Server, GiasContext, False)
                    Else
                        objEFMacchineCodici.Update(macchina, objParametri_Server, GiasContext, False)
                    End If
                End If

                Dim interscambioPC_W As New AgronicaCoreInterscambioBIZ.Interscambio_Parco_Macchine_W
                Dim interscambio = interscambioPC_W.PrepareCreateInterscambioEF(
                parcoMacchine,
                codice_esterno,
                enum_SistemiEsterni.demetra,
                GiasContext,
                objParametri_Server,
                objParametri_Utenti,
                False
                )

                '-----------------------------------
                '   SCRITTURA LOG INVIO ANAGRAFE 'OK'
                '-----------------------------------
                CallCreateLogInvioChiamateD2G(cuaa, macchina, codice_esterno, macchina.centroPK.partitaIva, 0, macchina.codice, enum_TipoOperazioneDB.Scrittura, "OK", "", objParametri_Server, GiasContext)

            End If
        Catch ex As Exception
            Dim MessaggioErrore = ex.Message

            '-----------------------------------
            '   SCRITTURA LOG INVIO ANAGRAFE 'KO'
            '-----------------------------------
            CallCreateLogInvioChiamateD2G(cuaa, macchina, codice_esterno, macchina.centroPK.partitaIva, 0, macchina.codice, enum_TipoOperazioneDB.Scrittura, "KO", ex.Message, objParametri_Server, GiasContext)

            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore, ex)
        End Try

        Return macchina

    End Function

    Private Function PrepareEditMacchinaD2G(ByVal cuaa As String,
                                           ByVal macchina As ParcoMacchine,
                                           ByVal codice_esterno As String,
                                           ByRef GiasContext As Gias_DeveloperServer_Entities,
                                           ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                           ByVal objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                           ) As ParcoMacchine

        Dim nomeRoutine As String = "AgronicaCoreDemetraBIZ.ImportParcoMacchine.PrepareEditMacchinaD2G()"
        Dim parcoMacchine As Parco_Macchine

        Try
            'TODO Salvo: Controllare che vi sia record corrispondente nella tabella Interscambio_Parco_Macchine
            'lavez - 14/02/2024 - non necessario
            'If ExistsMacchinaD2G(macchina, codice_esterno, GiasContext) Then
            'Dim objParcoMacchine As New AgronicaCoreContabBIZ.Parco_Macchine_W
            'If ParcoMacchineUtility.VerificaMovimentiMacchina(macchina, objParametri_Server) Then
            '    Throw New Exception("La Macchina: " & macchina.codice & " è stata movimentata")
            'End If

            Try
                If Not ParcoMacchineUtility.IsEditAllowed(macchina, objParametri_Server) Then
                    Throw New Exception(AgronicaCoreDataProvider.My.Resources.Gias.ImpossibileModificareMacchinaMovimentiAssociati)
                End If
            Catch ex As GiasException
                Throw New Exception($"{ex.Message}  (Mac_Cod: {macchina.codice})")
            End Try

            Dim objEFMacchine As New EFMacchine
            parcoMacchine = objEFMacchine.PrepareEditParcoMacchineD2G(macchina, codice_esterno, GiasContext, objParametri_Server, objParametri_Utenti)

            'Lavez - 30/07/2024 - numero certificato taratura
            If macchina.codice <> 0 Then
                Dim objEFMacchineCodici As New EFParcoMacchineCodici
                If objEFMacchineCodici.Retrive(macchina, enum_CodiciAnagrafe.MacchinaCertificazioneTaratura, GiasContext) Is Nothing Then
                    objEFMacchineCodici.Write(macchina, objParametri_Server, GiasContext, False)
                Else
                    objEFMacchineCodici.Update(macchina, objParametri_Server, GiasContext, False)
                End If
            End If

            'lavez - 14/06/2024 - se la chiave esterna di demetra è valorizzata e non esiste su gias la creo
            If codice_esterno <> "" AndAlso ExistsMacchinaD2G(macchina, codice_esterno, GiasContext) = False Then
                Dim interscambioPC_W As New AgronicaCoreInterscambioBIZ.Interscambio_Parco_Macchine_W
                Dim interscambio = interscambioPC_W.PrepareCreateInterscambioEF(
                    parcoMacchine,
                    codice_esterno,
                    enum_SistemiEsterni.demetra,
                    GiasContext,
                    objParametri_Server,
                    objParametri_Utenti,
                    False
                    )
            End If

            '-----------------------------------
            '   SCRITTURA LOG INVIO ANAGRAFE 'OK'
            '-----------------------------------
            CallCreateLogInvioChiamateD2G(cuaa, macchina, codice_esterno, macchina.centroPK.partitaIva, 0, macchina.codice, enum_TipoOperazioneDB.Modifica, "OK", "", objParametri_Server, GiasContext)

            'Else
            '    Throw New Exception("La macchina con Mac_Cod = " & macchina.codice & " e CodiceEsterno = " & codice_esterno & " non esiste")
            'End If
        Catch ex As Exception

            Dim MessaggioErrore = ex.Message

            '-----------------------------------
            '   SCRITTURA LOG INVIO ANAGRAFE 'KO'
            '-----------------------------------
            CallCreateLogInvioChiamateD2G(cuaa, macchina, codice_esterno, macchina.centroPK.partitaIva, 0, macchina.codice, enum_TipoOperazioneDB.Modifica, "KO", ex.Message, objParametri_Server, GiasContext)


            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore, ex)
        End Try

        Return macchina
    End Function

    Private Function PrepareDeleteMacchinaD2G(ByVal cuaa As String,
                                             ByVal macchina As ParcoMacchine,
                                             ByVal codice_esterno As String,
                                             ByRef GiasContext As Gias_DeveloperServer_Entities,
                                             ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                             ByVal objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                             ) As ParcoMacchine

        Dim nomeRoutine As String = "AgronicaCoreDemetraBIZ.ImportParcoMacchine.PrepareDeleteMacchinaD2G()"
        Dim parcoMacchine As Parco_Macchine

        Try
            'lavez - 14/06/2024 - vincolo rilassato per gestire la casistica :
            'Macchina creata su gias -> inviata a demetra -> cancellata da demetra -> cancellata su gias
            'If ExistsMacchinaD2G(macchina, codice_esterno, GiasContext) Then
            'Dim objParcoMacchine As New AgronicaCoreContabBIZ.Parco_Macchine_W
            'If ParcoMacchineUtility.VerificaMovimentiMacchina(macchina, objParametri_Server, True) Then
            '    Throw New Exception("La Macchina: " & macchina.codice & " è stata movimentata")
            'End If

            Try
                If Not ParcoMacchineUtility.IsRemoveAllowed(macchina, objParametri_Server) Then
                    Throw New Exception(AgronicaCoreDataProvider.My.Resources.Gias.ImpossibileCancellareMacchinaMovimentiAssociati)
                End If
            Catch ex As GiasException
                Throw New Exception($"{ex.Message}  (Mac_Cod: {macchina.codice})")
            End Try

            Dim objEFMacchine As New EFMacchine
            parcoMacchine = objEFMacchine.PrepareDeleteParcoMacchineD2G(macchina, codice_esterno, GiasContext, objParametri_Server, objParametri_Utenti)

            'Lavez - 30/07/2024 - numero certificato taratura
            Dim objEFMacchineCodici As New EFParcoMacchineCodici
            objEFMacchineCodici.Remove(macchina, objParametri_Server, GiasContext, False)

            '-----------------------------------
            '   SCRITTURA LOG INVIO ANAGRAFE 'OK'
            '-----------------------------------
            CallCreateLogInvioChiamateD2G(cuaa, macchina, codice_esterno, macchina.centroPK.partitaIva, 0, macchina.codice, enum_TipoOperazioneDB.Cancellazione, "OK", "", objParametri_Server, GiasContext)

            'Else
            '    Throw New Exception("La macchina con Mac_Cod = " & macchina.codice & " e CodiceEsterno = " & codice_esterno & " non esiste")
            'End If
        Catch ex As Exception
            Dim MessaggioErrore = ex.Message

            '-----------------------------------
            '   SCRITTURA LOG INVIO ANAGRAFE 'KO'
            '-----------------------------------
            CallCreateLogInvioChiamateD2G(cuaa, macchina, codice_esterno, macchina.centroPK.partitaIva, 0, macchina.codice, enum_TipoOperazioneDB.Cancellazione, "KO", ex.Message, objParametri_Server, GiasContext)

            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore, ex)
        End Try

        Return macchina

    End Function

    Private Sub CallCreateLogInvioChiamateD2G(cuaa As String, macchina As ParcoMacchine,
                                              chiave_esterna As String,
                                              piva As String, sa_cod As Integer, mac_cod As Integer,
                                              tipoOperazione As enum_TipoOperazioneDB,
                                              esito As String, dati_ricevuti As String,
                                              objParametri_Server As AgronicaCoreParametri,
                                              giasContext As Gias_DeveloperServer_Entities)

        Dim chiave_GIAS As String = piva & "_" & sa_cod & "_" & mac_cod

        Dim tzh As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
        Dim pacchettoDaLoggare As New AgronicaCoreDTOStd.InData.importazioni.ImportDemetra With {
            .CUAA = cuaa,
            .dati = JsonConvert.SerializeObject(macchina, tzh)
        }

        Dim strPacchettoDaLoggare As String = JsonConvert.SerializeObject(pacchettoDaLoggare, tzh)

        Dim objLogInvioChiamateW As New AgronicaCoreVarieBIZ.Agronica_Log_Invio_Chiamate_W
        objLogInvioChiamateW.Create_Log_Invio_Anagrafe(enum_Esportazioni_Sistema_Cod.Demetra_Import_Macchine,
                                                       strPacchettoDaLoggare, enum_TipoEntita_Des.ParcoMacchine,
                                                       chiave_GIAS, chiave_esterna,
                                                       piva, sa_cod, 0, 0, 0, 0, 0, "",
                                                       tipoOperazione, esito,
                                                       dati_ricevuti,
                                                       objParametri_Server,
                                                       giasContext,
                                                       Mac_Cod:=mac_cod)

    End Sub

#End Region


End Class
