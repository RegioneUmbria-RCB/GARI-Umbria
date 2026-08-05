Imports System.Runtime.CompilerServices
Imports System.Text
Public Module ListExtensions

    '''<summary>
    ''' Crea una stringa del tipo " ( valore1, valore2, ..., valoreN ) "
    '''</summary>
    <Extension()>
    Function ToQueryInExpression(ByVal listaValori As List(Of String)) As String
        Dim stringBuilder As New StringBuilder
        stringBuilder.Length = 0
        stringBuilder.Append(" ('")
        stringBuilder.Append(listaValori.First)
        stringBuilder.Append("'")
        For Each s As String In listaValori
            stringBuilder.Append(", '")
            stringBuilder.Append(s)
            stringBuilder.Append("'")
        Next
        stringBuilder.Append(") ")
        Return stringBuilder.ToString
    End Function

    <Extension()>
    Function ToQueryInExpression(ByVal listaValori As List(Of Integer)) As String
        'Return listaValori.Aggregate(New StringBuilder("( "), Function(stb, x) stb.Append(x).Append(", "), Function(stb) stb.Remove(stb.Length - 6, 4).Append(") ").ToString)
        Dim stringBuilder As New StringBuilder
        stringBuilder.Length = 0
        stringBuilder.Append(" (").Append(listaValori.First)
        For Each s As String In listaValori
            stringBuilder.Append(", ").Append(s)
        Next
        stringBuilder.Append(") ")
        Return stringBuilder.ToString
    End Function


    '''<summary>
    ''' Divide gli elementi di una sequenza in chunk che hanno dimensione massima chunkSize
    '''</summary>
    <Extension()>
    Function ChunkBy(Of TSource)(ByVal source As IEnumerable(Of TSource), ByVal chunkSize As Integer) As IEnumerable(Of IEnumerable(Of TSource))
        Return ChunkByWorker(Of TSource)(source, chunkSize)
    End Function


    Private Iterator Function ChunkByWorker(Of TSource)(ByVal source As IEnumerable(Of TSource), ByVal chunkSize As Integer) As IEnumerable(Of IEnumerable(Of TSource))
        While source.Any()
            Yield source.Take(chunkSize)
            source = source.Skip(chunkSize)
        End While
    End Function
End Module
