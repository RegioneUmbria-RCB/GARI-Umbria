Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider


Public Class Attivita
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Shared Function TipoAttivitaDes_from_TipoAttivitaCod(
                                    ByVal TipoAttivitaCod As String) As String

        Dim Des As String

        Select Case TipoAttivitaCod

            Case "PV"
                Des = "Produzione vegetale"
            Case "PZ"
                Des = "Produzione zootecnica"
            Case "PVZ"
                Des = "Produzione vegetale e zootecnica"
            Case "TPV"
                Des = "Preparazione vegetale"
            Case "TPZ"
                Des = "Preparazione zootecnica"
            Case "TPVZ"
                Des = "Preparazione vegetale e zootecnica"
            Case "I"
                Des = "Importazione"
            Case "RS"
                Des = "Raccolta spontanea"
            Case "P/TP"
                Des = "Produzione / Preparazione"
            Case "TP/I"
                Des = "Preparazione / Importazione"
            Case Else

                If TipoAttivitaCod.StartsWith("@") Then
                    'Elimino il primo carattere "@"
                    Des = "Altro : " & TipoAttivitaCod.Substring(1)
                Else
                    Des = "Altro"
                End If

        End Select

        'Restituisco il risultato
        Return Des
    End Function


End Class
