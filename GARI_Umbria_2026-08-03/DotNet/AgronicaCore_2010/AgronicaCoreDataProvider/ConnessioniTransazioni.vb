Imports System.Data

Public Class ConnessioniTransazioni
    Inherits AgronicaCoreDataProvider.LogProvider

    Public Shared Sub ApriConnessione(ByVal FlagInitTransazione As Boolean,
                                      ByRef objParametri As AgronicaCoreParametri)
        Try
            'Verifico se e' stata impostata una connessione
            If IsNothing(objParametri.objConnessione) Then
                'Creo la connessione
                objParametri.objConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
                objParametri.objConnessione.Open()
            Else
                If objParametri.objConnessione.State <> ConnectionState.Open Then
                    'Creo la connessione
                    objParametri.objConnessione.ConnectionString = DataProviderFactory.Instance.AggiustaStringaDiConnessione(objParametri.StringaConnessione)
                    objParametri.objConnessione.Open()

                End If
            End If

            If FlagInitTransazione = True Then
                'apro la transazione 
                If objParametri.objTransazione Is Nothing Then
                    'Inizializzo la transazione
                    objParametri.objTransazione = objParametri.objConnessione.BeginTransaction
                Else
                    Throw New Exception("la transazione era rimasta aperta")
                End If
            End If
        Catch ex As Exception
            Throw New Exception("ApriConnessione: " & ex.Message)
        End Try

    End Sub


    Public Shared Sub ApriConnessioneXCoreBiz(ByRef FlagConnessioneLocale As Boolean,
                                              ByRef FlagTransazioneLocale As Boolean,
                                              ByRef objParametri As AgronicaCoreParametri,
                                              Optional ByRef isolationLevel? As System.Data.IsolationLevel = Nothing)

        FlagConnessioneLocale = False
        FlagTransazioneLocale = False

        Try
            'Verifico se e' stata impostata una connessione sull'objparametri
            If IsNothing(objParametri.objConnessione) Then
                'Non è c'è, Creo la connessione
                objParametri.objConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
                objParametri.objConnessione.Open()
                'imposto flag connessione locale
                FlagConnessioneLocale = True
            Else
                'verifico se la connessione è aperta
                If objParametri.objConnessione.State <> ConnectionState.Open Then
                    'è chiusa, apro la connessione
                    objParametri.objConnessione.ConnectionString = DataProviderFactory.Instance.AggiustaStringaDiConnessione(objParametri.StringaConnessione)
                    objParametri.objConnessione.Open()
                    'imposto flag connessione locale
                    FlagConnessioneLocale = True
                End If
            End If

            'If FlagConnessioneLocale AndAlso FlagInitTransazione Then
            If FlagConnessioneLocale Then
                'verifico se esiste la transazione
                If objParametri.objTransazione Is Nothing Then
                    'non c'è, Inizializzo la transazione
                    If isolationLevel Is Nothing Then
                        objParametri.objTransazione = objParametri.objConnessione.BeginTransaction
                    Else
                        objParametri.objTransazione = objParametri.objConnessione.BeginTransaction(CType(isolationLevel, IsolationLevel))
                    End If

                    FlagTransazioneLocale = True
                Else
                    Throw New Exception("la transazione era rimasta aperta")
                End If
            End If

        Catch ex As Exception
            Throw New Exception("ApriConnessioneXCoreBiz: " & ex.Message)
        End Try

    End Sub


    Public Shared Sub ChiudiConnessioneXCoreBiz(ByVal FlagConnessioneLocale As Boolean,
                                                ByRef objParametri As AgronicaCoreParametri)
        Try
            If FlagConnessioneLocale = True AndAlso objParametri.objConnessione IsNot Nothing Then
                objParametri.objConnessione.Close()
                objParametri.objConnessione.Dispose()
                objParametri.objConnessione = Nothing
                objParametri.objTransazione = Nothing
            End If


        Catch ex As Exception
            Throw New Exception("ChiudiConnessioneXCoreBiz: " & ex.Message)
        End Try

    End Sub


    Public Shared Sub ChiudiConnessione(ByRef objParametri As AgronicaCoreParametri)
        Try
            If objParametri.objTransazione IsNot Nothing Then
                Throw New Exception("la transazione è ancora aperta")
            End If

            If objParametri.objConnessione IsNot Nothing Then
                objParametri.objConnessione.Close()
                objParametri.objConnessione.Dispose()
                objParametri.objConnessione = Nothing
                objParametri.objTransazione = Nothing
            End If

        Catch ex As Exception
            Throw New Exception("ChiudiConnessione: " & ex.Message)
        End Try

    End Sub


    Public Shared Sub ChiudiTransazione(ByVal Flag_Commit1_Rollback2 As Integer,
                                        ByRef objParametri As AgronicaCoreParametri)
        Try
            If objParametri.objTransazione Is Nothing Then
                Throw New Exception("la transazione è già chiusa")
            Else
                Select Case Flag_Commit1_Rollback2
                    Case 1
                        objParametri.objTransazione.Commit()
                    Case 2
                        objParametri.objTransazione.Rollback()

                End Select
                objParametri.objTransazione = Nothing
            End If

        Catch ex As Exception
            Throw New Exception("ChiudiTransazione: " & ex.Message)
        End Try

    End Sub

    Public Shared Sub ChiudiTransazioneXCoreBiz(ByVal FlagTransazioneLocale As Boolean,
                                                ByRef objParametri As AgronicaCoreParametri)
        Try

            'Se la transazione è stata avviata localmente faccio il commit
            If FlagTransazioneLocale = True Then
                objParametri.objTransazione.Commit()
            End If

        Catch ex As Exception
            Throw New Exception("ChiudiTransazioneXCoreBiz: " & ex.Message)
        End Try

    End Sub


    '######################################
    'se la transazione non è già chiusa, fa il rollback e chiude la connessione
    Public Shared Sub RollBackTransazione_ChiudiConnessione(ByRef objParametri As AgronicaCoreParametri)
        Try
            If objParametri.objTransazione IsNot Nothing Then
                objParametri.objTransazione.Rollback()
                objParametri.objTransazione = Nothing
            End If

            If objParametri.objConnessione IsNot Nothing Then
                objParametri.objConnessione.Close()
                objParametri.objConnessione.Dispose()
                objParametri.objConnessione = Nothing
            End If

        Catch ex As Exception
            Throw New Exception("RollBackTransazione_ChiudiConnessione: " & ex.Message)
        End Try

    End Sub

    '######################################
    'se la transazione non è già chiusa, fa il rollback 
    Public Shared Sub RollBackTransazione(ByRef objParametri As AgronicaCoreParametri)
        Try
            If objParametri.objTransazione IsNot Nothing Then
                objParametri.objTransazione.Rollback()
                objParametri.objTransazione = Nothing
            End If

        Catch ex As Exception
            Throw New Exception("RollBackTransazione: " & ex.Message)
        End Try

    End Sub

End Class
