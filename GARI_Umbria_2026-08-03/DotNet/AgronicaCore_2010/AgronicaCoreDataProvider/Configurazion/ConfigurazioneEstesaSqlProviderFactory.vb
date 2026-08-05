Imports System.Data.SqlClient
Imports System.Text
Imports Newtonsoft.Json

Public Class ConfigurazioneEstesaSqlProviderFactory : Inherits ConfigurazioneBase

    Private Shared classLocker As New Object()
    Private Shared objSingleton As ConfigurazioneEstesaSqlProvider

    Private Shared ReadOnly _databaseDaNonConsiderare As List(Of String) = New List(Of String) From
        {
            "_super_server", "_utenti", "_matrice"
        }

    Public Shared Function Instance(ByVal objParametri As AgronicaCoreParametri) As ConfigurazioneEstesaSqlProvider

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

    Private Shared Function LeggiConfigurazione(ByVal objParametri As AgronicaCoreParametri) As ConfigurazioneEstesaSqlProvider

        If objParametri Is Nothing Then
            Return Nothing
        End If

        If _databaseDaNonConsiderare.Any(Function(d) objParametri.StringaConnessione.ToLower.Contains(d)) Then
            Return Nothing
        End If

        Dim provider As IDataProvider = DataProviderFactory.Instance.Provider
        Dim sb = New StringBuilder

        Try

            If Not EsisteTabella("configurazione_siti", objParametri) Then
                Return Nothing
            End If

            sb.AppendLine(" select * from configurazione_siti")
            sb.AppendLine(" where chiave = 'DataProviderLogConfig' ")
            Dim dt As DataTable = provider.EseguiQuery_Lettura(objParametri, sb.ToString, "")

            sb.Clear()
            sb.AppendLine(" select * from configurazione_siti")
            sb.AppendLine(" where chiave = 'SQL_EseguiQueryOriginale' ")
            Dim dtQo As DataTable = provider.EseguiQuery_Lettura(objParametri, sb.ToString, "")

            sb.Clear()
            sb.AppendLine(" select * from configurazione_siti")
            sb.AppendLine(" where chiave = 'LanciaEccezioneSuInjection' ")
            Dim dtEj As DataTable = provider.EseguiQuery_Lettura(objParametri, sb.ToString, "")

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then

                Dim chiavi = (From r In dt.AsEnumerable
                              Select New With {
                                    .Nome = CStr(r.Item("Chiave")),
                                    .Valore = CStr(r.Item("Valore"))
                        }).ToList()

                Dim chiave = chiavi.FirstOrDefault(Function(c) c.Nome = "DataProviderLogConfig")
                If chiave IsNot Nothing AndAlso Not String.IsNullOrEmpty(chiave.Valore) Then

                    Dim cfgEstesa As ConfigurazioneEstesaSqlProvider = JsonConvert.DeserializeObject(Of ConfigurazioneEstesaSqlProvider)(chiave.Valore)
                    If cfgEstesa IsNot Nothing Then
                        If IsNothing(cfgEstesa.TempoWarningParser_Millisecondi) Then
                            cfgEstesa.TempoWarningParser_Millisecondi = 3000
                        End If
                    End If

                    If dtQo.Rows.Count > 0 Then
                        cfgEstesa.SQL_EseguiQueryOriginale = CBool(dtQo.Rows.Item(0).Item("Valore"))
                    End If

                    If dtEj.Rows.Count > 0 Then
                        cfgEstesa.LanciaEccezioneSuInjection = CBool(dtEj.Rows.Item(0).Item("Valore"))
                    End If

                    Return cfgEstesa
                Else
                    Return Nothing
                End If

            Else
                Return Nothing
            End If
        Catch ex As Exception
            Return New ConfigurazioneEstesaSqlProvider With
            {
                .CreaParametri = True,
                .LimiteElementiClausoleIn = 50,
                .SlidingExpirationInMinuti = 60,
                .TempoWarningParser_Millisecondi = 3000,
                .UtilizzaCache = True,
                .LanciaEccezioneSuInjection = False
            }
        End Try

    End Function

End Class

