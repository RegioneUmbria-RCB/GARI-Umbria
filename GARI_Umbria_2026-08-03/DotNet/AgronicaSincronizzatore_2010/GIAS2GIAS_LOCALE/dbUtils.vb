Public Class dbUtils
    ''' <summary>
    ''' se il valore passato è DBNull ritorna Nothing, altrimenti il valore stesso 
    ''' </summary>
    ''' <param name="valore">Valore su cui effettuare il controllo</param>
    ''' <returns>se il valore passato è DBNull ritorna Nothing, altrimenti il valore stesso <br/>
    ''' </returns>
    ''' <remarks>
    ''' Autore: vanni<br/>
    ''' Data creazione: 23/08/2005
    ''' </remarks>
    Public Shared Function DBNullToNothing(ByVal valore As Object) As Object
        If System.Convert.IsDBNull(valore) Then
            Return Nothing
        Else
            Return valore
        End If
    End Function

    ''' <summary>
    ''' se il valore passato è Nothing ritorna DBNull.value, altrimenti il valore stesso
    ''' </summary>
    ''' <param name="valore">Valore su cui effettuare il controllo</param>
    ''' <returns>se il valore passato è Nothing ritorna DBNull.Value, altrimenti il valore stesso<br/>
    ''' </returns>
    ''' <remarks>
    ''' Autore: vanni<br/>
    ''' Data creazione: 23/08/2005
    ''' </remarks>
    Public Shared Function NothingToDBNull(ByVal valore As Object) As Object
        If valore Is Nothing Then
            Return DBNull.Value
        Else
            Return valore
        End If
    End Function

End Class
