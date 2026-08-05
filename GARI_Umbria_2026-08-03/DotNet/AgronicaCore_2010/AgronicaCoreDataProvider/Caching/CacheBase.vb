Imports System.Runtime.Caching

Public Class CacheBase
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Shared Function Ottieni_Oggetto_Cache_Da_Chiave(ByVal chiave As String) As Agronica_Oggetto_Cache

        If String.IsNullOrEmpty(chiave) Then
            Return Nothing
        End If

        Dim attributi = chiave.Split({"§"c})

        Return New Agronica_Oggetto_Cache With
        {
            .NomeGruppoCache = attributi(0),
            .NomeServer = attributi(1),
            .NomeClasse = attributi(2),
            .NomeMetodo = attributi(3),
            .StringaSql = attributi(4)
        }

    End Function

    Public Shared Sub Pulisci_Cache_Puntuale(ByVal cacheKeyPrefix As String, ByVal nomeServer As String, ByVal nomeGruppo As String, ByRef cache As ObjectCache)

        If IsNothing(cache) Then
            Return
        End If

        Dim tutteLeChiavi = cache.Select(Function(k) k.Key).ToList
        For Each c As String In tutteLeChiavi
            If cache.Contains(c) Then
                Dim elementiChiave = c.Split({"§"c}).Select(Of String)(Function(s) s.ToLower).ToList().Take(3)
                If elementiChiave(0).Equals(cacheKeyPrefix.ToLower) _
                    AndAlso elementiChiave(1).Equals(nomeServer.ToLower) _
                    AndAlso elementiChiave(2).Equals(nomeGruppo.ToLower) Then
                    cache.Remove(c)
                End If
            End If
        Next


    End Sub

    Public Shared Sub Pulisci_Cache(ByRef cache As ObjectCache)

        If IsNothing(cache) Then
            Return
        End If

        Dim tutteLeChiavi = cache.Select(Function(k) k.Key).ToList
        For Each c As String In tutteLeChiavi
            If cache.Contains(c) Then
                cache.Remove(c)
            End If
        Next

    End Sub

    Public Shared Function Rimuovi_Elemento_Dalla_Cache(chiave As String, ByRef cache As ObjectCache) As Boolean

        If String.IsNullOrEmpty(chiave) OrElse IsNothing(cache) Then
            Return False
        End If

        If cache.Contains(chiave) Then
            cache.Remove(chiave)
            Return True
        Else
            Return False
        End If

    End Function

End Class

Public Class Agronica_Oggetto_Cache

    Public NomeServer As String
    Public NomeGruppoCache As String
    Public NomeClasse As String
    Public NomeMetodo As String
    Public Valore As Object
    Public StringaSql As String
    Public ValoreJson As String
    Public Chiave As String

    Public NumeroRisultati As Int32

End Class

