Imports AgronicaCoreEntityFrameworkSTD

Public Class UnitaMisura_R

    Public Function Leggi(dbContext As GiasDbContext, udm_cod As Integer) As AgronicaCoreModelloSTD.UnitaMisura

        Dim rval As AgronicaCoreModelloSTD.UnitaMisura = (
            From u In dbContext.APP_CategorieXUnitaMisura
            Where u.Udm_Cod = udm_cod
            Select New AgronicaCoreModelloSTD.UnitaMisura With {
                        .udm_cod = u.Udm_Cod,
                        .udm_des = u.Udm_des,
                        .udm_sim = u.Udm_Sim
                        }).FirstOrDefault

        Return rval

    End Function

End Class
