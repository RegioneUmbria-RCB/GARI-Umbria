Public Class ConfiguratoreImportatoreComparer : Implements IEqualityComparer(Of ConfigurazioneImportatore)

    Public Function Equals(x As ConfigurazioneImportatore, y As ConfigurazioneImportatore) As Boolean Implements IEqualityComparer(Of ConfigurazioneImportatore).Equals

        If x Is y Then Return True

        If x Is Nothing OrElse y Is Nothing Then Return False

        Return (x.IdServizio = y.IdServizio)

    End Function

    Public Function GetHashCode(obj As ConfigurazioneImportatore) As Integer Implements IEqualityComparer(Of ConfigurazioneImportatore).GetHashCode

        If obj Is Nothing Then Return 0

        Dim hashIdServizio = If(String.IsNullOrEmpty(obj.IdServizio), 0, obj.IdServizio.GetHashCode())

        Return hashIdServizio

    End Function
End Class
