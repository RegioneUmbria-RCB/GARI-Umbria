Imports System.Text
Imports Newtonsoft.Json

Public Class ConfigurazioneAjaxFactory : Inherits ConfigurazioneBase

    Private Shared classLocker As New Object()
    Private Shared objSingleton As ConfigurazioneAjax

    Private Shared ReadOnly _databaseDaNonConsiderare As List(Of String) = New List(Of String) From
        {
            "_super_server", "_utenti", "_matrice"
        }

    Public Shared Function Instance(ByVal objParametri As AgronicaCoreParametri) As ConfigurazioneAjax

        If (objSingleton Is Nothing) Then
            ' Thread Safe
            SyncLock (classLocker)
                If (objSingleton Is Nothing) Then

                    objSingleton = LeggiConfigurazione(objParametri)

                End If
            End SyncLock
        End If
        Return objSingleton

    End Function

    Public Shared Sub Reset(ByVal objParametri As AgronicaCoreParametri)

        SyncLock (classLocker)
            objSingleton = Nothing
        End SyncLock
        If Not IsNothing(objParametri) Then
            Instance(objParametri)
        End If

    End Sub

    Private Shared Function LeggiConfigurazione(ByVal objParametri As AgronicaCoreParametri) As ConfigurazioneAjax

        If objParametri Is Nothing Then
            Return Nothing
        End If

        If _databaseDaNonConsiderare.Any(Function(d) objParametri.StringaConnessione.ToLower.Contains(d)) Then
            Return Nothing
        End If

        Dim provider As IDataProvider = DataProviderFactory.Instance.Provider
        Dim sb = New StringBuilder
        Dim retvVal As New ConfigurazioneAjax()


        Try

            If Not EsisteTabella("configurazione_siti", objParametri) Then
                Return Nothing
            End If

            sb.AppendLine(" select * from configurazione_siti")
            sb.AppendLine(" where chiave = 'CompressioneRispostaAjax'")
            Dim dt As DataTable = provider.EseguiQuery_Lettura(objParametri, sb.ToString, "")

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then

                Dim chiavi = (From r In dt.AsEnumerable
                              Select New With {
                                    .Nome = CStr(r.Item("Chiave")),
                                    .Valore = CStr(r.Item("Valore"))
                        }).ToList()

                Dim chiave = chiavi.FirstOrDefault(Function(c) c.Nome = "CompressioneRispostaAjax")
                If chiave IsNot Nothing AndAlso Not String.IsNullOrEmpty(chiave.Valore) Then
                    retvVal = JsonConvert.DeserializeObject(Of ConfigurazioneAjax)(chiave.Valore)
                Else
                    Return retvVal
                End If

                Return retvVal
            Else
                Return New ConfigurazioneAjax()
            End If
        Catch ex As Exception
            Return retvVal
        End Try

    End Function

End Class
