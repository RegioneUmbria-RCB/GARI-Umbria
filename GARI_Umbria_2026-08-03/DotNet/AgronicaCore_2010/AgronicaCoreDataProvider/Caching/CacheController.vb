Public Class CacheController

    Private ReadOnly _cacheFiltroAggiuntivo As CachedFiltroAggiuntivo

    Sub New()

        _cacheFiltroAggiuntivo = DataProviderFactory.Instance.CacheFiltroAggiuntivo

    End Sub

    'Public Function OttieniTutteLeChiavi() As List(Of String)
    '    If Not _cacheFiltroAggiuntivo Is Nothing Then
    '        Return _cacheFiltroAggiuntivo.ElementiInCache()
    '    Else
    '        Return New List(Of String)
    '    End If
    'End Function

End Class
