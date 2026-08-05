Imports AgronicaCoreEntityFrameworkSTD

Public Class SpecieVegetali_R

    Public Function EstraiSpecieVegetaliImpianto(
        dbcontext As GiasDbContext,
        FiltroPiva As String,
        ByVal FiltroSa_Cod As Integer,
        ByVal FiltroAppezza As Integer,
        ByVal FiltroIdReg As Integer
    ) As AgronicaCoreModelloSTD.SpecieVegetaliDestinazioni

        Dim specieLetta = (
           From i In dbcontext.APP_Reg_Impianti
           Where i.piva = FiltroPiva AndAlso
                 i.sa_cod = FiltroSa_Cod AndAlso
                 i.appezza = FiltroAppezza AndAlso
                 i.id_reg = FiltroIdReg
           Select New With {
                            .veg_cod = i.veg_cod,
                            .id_cod = i.id_cod
                            }
           ).FirstOrDefault()

        Dim rval As AgronicaCoreModelloSTD.SpecieVegetaliDestinazioni = Nothing
        If specieLetta IsNot Nothing Then
            rval = New AgronicaCoreModelloSTD.SpecieVegetaliDestinazioni

            If specieLetta.id_cod = 0 Then
                rval.cod = CStr(specieLetta.veg_cod)
            Else
                rval.cod = "0/" & specieLetta.id_cod
            End If

        End If

        Return rval

    End Function

    ''' <summary>
    ''' Estrae lista specie
    ''' </summary>
    ''' <param name="dbcontext"></param>
    ''' <param name="FiltroPiva"></param>
    ''' <param name="FiltroSa_Cod"></param>
    ''' <param name="DataRifermimento"></param>
    ''' <returns>lista di specie</returns>
    Public Function EstraiListaSpecieVegetali(dbcontext As GiasDbContext, FiltroPiva As String, ByVal FiltroSa_Cod As Integer, ByVal DataRiferimento As DateTime) As List(Of AgronicaCoreModelloSTD.SpecieVegetaliDestinazioni)


        Dim listaSpecie As List(Of AgronicaCoreModelloSTD.SpecieVegetaliDestinazioni) = (
           From i In dbcontext.APP_Reg_Impianti
           Where i.piva = FiltroPiva AndAlso
                 i.sa_cod = FiltroSa_Cod AndAlso
                 i.id_cod = 0 AndAlso
                 (
                    i.validita_inizio_distinta <= DataRiferimento AndAlso
                    i.validita_fine_distinta >= DataRiferimento
                 )
           Select New AgronicaCoreModelloSTD.SpecieVegetaliDestinazioni With {
                .veg_des = i.veg_des,
                .cod = i.veg_cod.ToString()
            } Distinct
           ).ToList()


        Dim listaDestinazioniUso As List(Of AgronicaCoreModelloSTD.SpecieVegetaliDestinazioni) = (
           From i In dbcontext.APP_Reg_Impianti
           Where i.piva = FiltroPiva AndAlso
                 i.sa_cod = FiltroSa_Cod AndAlso
                 i.id_cod <> 0 AndAlso (
                i.validita_inizio_distinta <= DataRiferimento AndAlso
                i.validita_fine_distinta >= DataRiferimento
            )
           Select New AgronicaCoreModelloSTD.SpecieVegetaliDestinazioni With {
                            .veg_des = i.codici_anagrafe_des,
                            .cod = "0/" & i.id_cod.ToString()
                            } Distinct
        ).ToList()


        listaSpecie = listaSpecie.Concat(listaDestinazioniUso).ToList()

        Dim rval As List(Of AgronicaCoreModelloSTD.SpecieVegetaliDestinazioni) =
            listaSpecie.GroupBy(Function(g) g.cod).Select(Function(f) f.First()).OrderBy(Function(f) f.veg_des).ToList()



        Return rval

    End Function

End Class
