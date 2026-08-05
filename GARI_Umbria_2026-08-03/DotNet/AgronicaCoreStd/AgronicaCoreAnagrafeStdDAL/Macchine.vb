Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models

Public Class Macchine_R



    Public Function EstraiListaMacchine(dbContext As GiasDbContext, FiltroPiva As String, ByVal FiltroSaCod As Integer) As List(Of APP_Parco_Macchine)

        Dim rval As List(Of APP_Parco_Macchine) = (
            From m In dbContext.APP_Parco_Macchine
            Where (m.Sa_Cod = -1 OrElse (
                m.Piva = FiltroPiva AndAlso m.Sa_Cod = 0
                ) OrElse (
                m.Piva = FiltroPiva AndAlso m.Sa_Cod = FiltroSaCod
            ))
            Select m
        ).OrderBy(Function(f) f.Mac_Des).ToList()

        Return rval

    End Function

    Public Function LeggiMacchina(dbContext As GiasDbContext, Codice As String) As APP_Parco_Macchine

        Return dbContext.APP_Parco_Macchine.Where(Function(item) (item.Codice = Codice)).ToList().FirstOrDefault()

    End Function

End Class
