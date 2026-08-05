Imports AgronicaCoreAnagrafeStdDAL
Imports AgronicaCoreEntityFrameworkSTD

Public Class SpecieVegetali

    Private ReadOnly dbContext As GiasDbContext

    Public Sub New(dbContext As GiasDbContext)
        Me.dbContext = dbContext
    End Sub

    ''' <summary>
    ''' estrae una lista di specie vegetali filtrate per centro aziendale e data di riferimento per impianto
    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="Sa_Cod">codice centro</param>
    ''' <param name="DataRifermimento"></param>
    ''' <returns>lista di specie</returns>
    Public Function EstraiListaSpecieVegetali(Piva As String, ByVal Sa_Cod As Integer, ByVal DataRifermimento As DateTime) As List(Of AgronicaCoreModelloSTD.SpecieVegetaliDestinazioni)

        Dim xLettura As New SpecieVegetali_R()
        Return xLettura.EstraiListaSpecieVegetali(dbContext, Piva, Sa_Cod, DataRifermimento)

    End Function

End Class
