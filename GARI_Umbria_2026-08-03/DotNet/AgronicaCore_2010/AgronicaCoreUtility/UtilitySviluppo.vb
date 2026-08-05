Imports System.Security.Cryptography
Imports System.Text

' Funzioni utili durante lo sviluppo
Public Class UtilitySviluppo

    ''' <summary>
    ''' Calcola un codice hash basato sui dati di una DataTable.
    ''' Questo codice hash può essere utilizzato per verificare se due DataTable
    ''' contengono gli stessi dati.
    ''' </summary>
    ''' <param name="table">La DataTable da cui calcolare il codice hash.</param>
    ''' <returns>Una stringa che rappresenta il codice hash dei dati della DataTable.</returns>
    Public Shared Function GetDataTableHashCode(ByVal table As DataTable) As String
        Using md5 As MD5 = MD5.Create()
            Dim sb As New StringBuilder()

            For Each row As DataRow In table.Rows
                For Each col As DataColumn In table.Columns
                    sb.Append(row(col).ToString())
                Next
            Next

            Dim dataBytes As Byte() = Encoding.UTF8.GetBytes(sb.ToString())

            Dim hashBytes As Byte() = md5.ComputeHash(dataBytes)

            Dim hash As New StringBuilder()
            For Each b As Byte In hashBytes
                hash.Append(b.ToString("x2"))
            Next

            Return hash.ToString()
        End Using

    End Function

    ''' <summary>
    ''' Verifica se due DataTable contengono gli stessi dati confrontando i loro codici hash.
    ''' </summary>
    ''' <param name="table1">La prima DataTable da confrontare.</param>
    ''' <param name="table2">La seconda DataTable da confrontare.</param>
    ''' <returns>True se le DataTable contengono gli stessi dati; False altrimenti.</returns>
    Public Shared Function AreDataTablesEqual(ByVal table1 As DataTable, ByVal table2 As DataTable) As Boolean
        Dim hash1 As String = GetDataTableHashCode(table1)
        Dim hash2 As String = GetDataTableHashCode(table2)

        Return hash1 = hash2
    End Function

End Class
