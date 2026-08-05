Imports System.Collections.Generic
Imports AgronicaCoreEFatturaBIZ

Public Class AnagraficaComparer : Implements IEqualityComparer(Of AnagraficaMap)

    Public Function Equals(x As AnagraficaMap, y As AnagraficaMap) As Boolean Implements IEqualityComparer(Of AnagraficaMap).Equals

        If x Is Nothing AndAlso y Is Nothing Then Return True
        If x Is Nothing OrElse y Is Nothing Then Return False

        If x.Cod_Contatto.Equals(y.Cod_Contatto) AndAlso x.rag_soc.ToLower().Equals(y.rag_soc.ToLower()) _
            AndAlso x.Cognome.ToLower().Equals(y.Cognome.ToLower()) _
            AndAlso x.Nome.ToLower().Equals(y.Nome.ToLower()) Then
            Return True
        End If

        Return False

    End Function

    Public Function GetHashCode(obj As AnagraficaMap) As Integer Implements IEqualityComparer(Of AnagraficaMap).GetHashCode
        Return obj.Cod_Contatto.GetHashCode()
    End Function
End Class
