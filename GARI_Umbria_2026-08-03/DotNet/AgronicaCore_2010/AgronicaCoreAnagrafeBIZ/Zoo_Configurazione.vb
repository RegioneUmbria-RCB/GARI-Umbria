Imports System.Linq
Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreVarieBIZ

Public Class Zoo_Configurazione

    Function Scrivi_Zoo_Configurazione(piva As String,
                                       Lista_Configurazioni As AgronicaCoreEntityFramework_POCO.Zoo_Configurazione(),
                                       objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                       objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                       ) As RispostaStandard
        Dim r As New RispostaStandard

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
        Dim scope As New TransactionScope()
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
        Try

            Dim objZoo As New AgronicaCoreAnagrafeDAL.Zoo_Configurazione

            'Cancello tutti i record vecchi e poi li riscrivo
            Dim oldRecord = (From configurazione In GiasContext.Zoo_Configurazione Where configurazione.PIVA = piva Select configurazione).ToList()

            For Each record In oldRecord
                objZoo.Elimina(record, GiasContext, objParametri_Server, objParametri_Utenti)
                GiasContext.SaveChanges()
            Next

            For Each configurazione In Lista_Configurazioni

                'Ovviamente scrivo solo i record inseriti dalla mia azienda
                If configurazione.PIVA = piva Then
                    objZoo.Scrivi(configurazione, GiasContext, objParametri_Server, objParametri_Utenti)
                    GiasContext.SaveChanges()
                End If
            Next

            scope.Complete()
            scope.Dispose()

            r.RispostaStringa = ""
            r.RispostaOK = True

        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
            scope.Dispose()

            Throw ex

        Finally

            GiasContext.Dispose()

        End Try

        Return r
    End Function

    Function Scrivi_Zoo_Animali_Lista_Tipi(piva As String,
                                           Lista_Tipi As Zoo_Animali_Lista_Tipi(),
                                           objParametri_Server As AgronicaCoreParametri,
                                           objParametri_Utenti As AgronicaCoreParametri
                                           ) As RispostaStandard
        Dim r As New RispostaStandard

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
        Dim scope As New TransactionScope()
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

        Try
            Dim objZoo As New AgronicaCoreAnagrafeDAL.Zoo_Animali_Lista_Tipi

            'Cancello tutti i record vecchi e poi li riscrivo
            Dim oldRecord = (From tipi In GiasContext.Zoo_Animali_Lista_Tipi Where tipi.PIVA = piva Select tipi).ToList()

            For Each record In oldRecord
                objZoo.Elimina(record, GiasContext, objParametri_Server, objParametri_Utenti)
                GiasContext.SaveChanges()
            Next


            For Each tipo In Lista_Tipi.Where(Function(x) x.PIVA = piva) 'scrivo solo i record inseriti dalla mia azienda
                If tipo.TIPO_COD = 0 Then
                    Dim dati = From zalt In GiasContext.Zoo_Animali_Lista_Tipi
                               Where zalt.GEN_COD = tipo.GEN_COD And
                                   zalt.SPE_COD = tipo.SPE_COD

                    If dati.Count = 0 Then
                        tipo.TIPO_COD = 1
                    Else
                        tipo.TIPO_COD = (From zalt In GiasContext.Zoo_Animali_Lista_Tipi
                                         Where zalt.GEN_COD = tipo.GEN_COD And
                                             zalt.SPE_COD = tipo.SPE_COD
                                         Select zalt.TIPO_COD).Max() + 1
                    End If
                End If

                objZoo.Scrivi(tipo, GiasContext, objParametri_Server, objParametri_Utenti)
                GiasContext.SaveChanges()
            Next

            scope.Complete()
            scope.Dispose()

            r.RispostaStringa = ""
            r.RispostaOK = True

        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
            scope.Dispose()

            Throw ex

        Finally

            GiasContext.Dispose()

        End Try

        Return r
    End Function

    Function Scrivi_Zoo_Animali_Lista_Stati_Accrescimento(piva As String,
                                                          Lista_Stati_Accrescimento As Zoo_Animali_Lista_Stati_Accrescimento(),
                                                          objParametri_Server As AgronicaCoreParametri,
                                                          objParametri_Utenti As AgronicaCoreParametri
                                                          ) As RispostaStandard
        Dim r As New RispostaStandard

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
        Dim scope As New TransactionScope()
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

        Try

            Dim objZoo As New AgronicaCoreAnagrafeDAL.Zoo_Animali_Lista_Stati_Accrescimento

            'Cancello tutti i record vecchi e poi li riscrivo
            Dim oldRecord = (From stati In GiasContext.Zoo_Animali_Lista_Stati_Accrescimento Where stati.PIVA = piva Select stati).ToList()

            For Each record In oldRecord
                objZoo.Elimina(record, GiasContext, objParametri_Server, objParametri_Utenti)
                GiasContext.SaveChanges()
            Next

            For Each stato In Lista_Stati_Accrescimento.Where(Function(x) x.PIVA = piva) 'scrivo solo i record inseriti dalla mia azienda
                If stato.STATO_COD = 0 Then
                    Dim dati = From zalt In GiasContext.Zoo_Animali_Lista_Stati_Accrescimento
                               Where zalt.GEN_COD = stato.GEN_COD And
                               zalt.SPE_COD = stato.SPE_COD And
                               zalt.TIPO_COD = stato.TIPO_COD

                    If dati.Count = 0 Then
                        stato.STATO_COD = 1
                    Else
                        stato.STATO_COD = (From zalt In GiasContext.Zoo_Animali_Lista_Stati_Accrescimento
                                           Where zalt.GEN_COD = stato.GEN_COD And
                                        zalt.SPE_COD = stato.SPE_COD And
                                        zalt.TIPO_COD = stato.TIPO_COD
                                           Select zalt.STATO_COD).Max() + 1
                    End If
                End If

                objZoo.Scrivi(stato, GiasContext, objParametri_Server, objParametri_Utenti)
                GiasContext.SaveChanges()
            Next

            scope.Complete()
            scope.Dispose()

            r.RispostaStringa = ""
            r.RispostaOK = True

        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
            scope.Dispose()

            Throw ex

        Finally

            GiasContext.Dispose()

        End Try

        Return r
    End Function

    Function Scrivi_Zoo_Animali_Stati_AccrescimentoxFabbisogno(piva As String,
                                                               Lista_Stati_AccrescimentoxFabbisogno As Zoo_Animali_Stati_AccrescimentoxFabbisogno(),
                                                               objParametri_Server As AgronicaCoreParametri,
                                                               objParametri_Utenti As AgronicaCoreParametri) As RispostaStandard
        Dim r As New RispostaStandard

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
        Dim scope As New TransactionScope()
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

        Try
            Dim objZoo As New AgronicaCoreAnagrafeDAL.Zoo_Animali_Stati_AccrescimentoxFabbisogno

            'Cancello tutti i record vecchi e poi li riscrivo
            Dim oldRecord = (From sxf In GiasContext.Zoo_Animali_Stati_AccrescimentoxFabbisogno Where sxf.PIVA = piva Select sxf).ToList()

            For Each record In oldRecord
                objZoo.Elimina(record, GiasContext, objParametri_Server, objParametri_Utenti)
                GiasContext.SaveChanges()
            Next

            For Each sxf In Lista_Stati_AccrescimentoxFabbisogno.Where(Function(x) x.PIVA = piva) 'scrivo solo i record inseriti dalla mia azienda

                objZoo.Scrivi(sxf, GiasContext, objParametri_Server, objParametri_Utenti)
                GiasContext.SaveChanges()

            Next

            scope.Complete()
            scope.Dispose()

            r.RispostaStringa = ""
            r.RispostaOK = True

        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
            scope.Dispose()

            Throw ex

        Finally

            GiasContext.Dispose()

        End Try

        Return r
    End Function

End Class
