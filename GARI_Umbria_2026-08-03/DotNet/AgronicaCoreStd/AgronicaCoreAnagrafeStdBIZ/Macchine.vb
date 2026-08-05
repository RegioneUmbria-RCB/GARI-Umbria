Imports AgronicaCoreAnagrafeStdDAL
Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models

Public Class Macchine
    Inherits BaseBiz

    Public Sub New(dbContext As GiasDbContext)
        MyBase.New(dbContext)
    End Sub


    ''' <summary>
    ''' Estra una lista di macchine pubbliche, di macchine visibili a livello di impresa e di macchine visibili a livello di centro aziendale
    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <returns></returns>
    Public Function EstraiListaMacchine(Piva As String, ByVal Sa_Cod As Integer) As List(Of APP_Parco_Macchine)

        Dim leggi As New Macchine_R()

        Return leggi.EstraiListaMacchine(dbContext, Piva, Sa_Cod)

    End Function

    Public Function LeggiMacchina(Codice As String) As APP_Parco_Macchine

        Dim xLettura = New Macchine_R()
        Return xLettura.LeggiMacchina(dbContext, Codice)

    End Function

End Class
