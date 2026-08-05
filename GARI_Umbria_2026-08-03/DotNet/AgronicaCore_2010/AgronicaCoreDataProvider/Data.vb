Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class Data


    '################################################################################
    Public Shared Function get_Annata_Agraria_Inizio(ByVal Data As Date) As Date
        Dim d_ritorno As Date
        Select Case Data.Month
            Case 11
                d_ritorno = New Date(Data.Year, 11, 1)
            Case 12
                d_ritorno = New Date(Data.Year, 11, 1)
            Case Else
                d_ritorno = New Date(Data.Year - 1, 11, 1)
        End Select

        Return d_ritorno
    End Function

    Public Shared Function get_Annata_Agraria_Fine(ByVal Data As Date) As Date
        Dim d_ritorno As Date
        Select Case Data.Month
            Case 11
                d_ritorno = New Date(Data.Year + 1, 10, 31)
            Case 12
                d_ritorno = New Date(Data.Year + 1, 10, 31)
            Case Else
                d_ritorno = New Date(Data.Year, 10, 31)
        End Select

        Return d_ritorno
    End Function






End Class
