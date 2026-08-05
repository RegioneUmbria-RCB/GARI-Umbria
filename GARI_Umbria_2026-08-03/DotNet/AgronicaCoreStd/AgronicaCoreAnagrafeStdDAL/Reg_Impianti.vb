Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports Microsoft.EntityFrameworkCore

Public Class Reg_Impianti_R


    ''' <summary>
    ''' Estrae una lista di centri aziendali
    ''' </summary>
    ''' <param name="dbContext"></param>
    ''' <param name="FiltroPiva">Imposta il filtro sulla piva se diverso da stringa vuota</param>
    ''' <returns></returns>
    Public Function EstraiListaCentriAziendali(dbContext As GiasDbContext, FiltroPiva As String) As List(Of AgronicaCoreModelloSTD.Centri_Aziendali)

        Dim rval As List(Of AgronicaCoreModelloSTD.Centri_Aziendali) = (
            From i In dbContext.APP_Reg_Impianti
            Where i.piva = FiltroPiva OrElse FiltroPiva = ""
            Distinct Select New AgronicaCoreModelloSTD.Centri_Aziendali With {
                             .Piva = i.piva,
                             .Sa_Cod = i.sa_cod,
                             .Rag_Soc = i.rag_soc,
                             .Sa_Nome = i.sa_nome
                             }
            ).Distinct().OrderBy(Function(f) f.Rag_Soc).ThenBy(Function(g) g.Sa_Nome).ToList()


        Return rval



    End Function

    Public Function LeggiImpianto(dbContext As GiasDbContext, Piva As String, ByVal Sa_Cod As Integer, Appezza As Integer, Id_Reg As Integer) As APP_Reg_Impianti

        Dim rval As APP_Reg_Impianti = (
            From reg In dbContext.APP_Reg_Impianti
            Where reg.piva = Piva AndAlso
                  reg.sa_cod = Sa_Cod AndAlso
                  reg.appezza = Appezza AndAlso
                  reg.id_reg = Id_Reg
        ).ToList.FirstOrDefault

        Return rval

    End Function

    ''' <summary>
    ''' Legge una lista di impianti filtrata per centro aziendale, specie (o destinazione d'uso in alternativa), data riferimento (tutti i parametri sono obbligatori)
    ''' </summary>
    ''' <param name="dbContext"></param>
    ''' <param name="Piva"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <param name="Veg_Cod">Specie vegetale</param>
    ''' <param name="Id_Cod">Destinazione d'uso</param>
    ''' <param name="DataRiferimento"></param>
    ''' <returns></returns>
    Public Function Leggi(dbContext As GiasDbContext, Piva As String, ByVal Sa_Cod As Integer, Veg_Cod As Integer, Id_Cod As Integer, DataRiferimento As DateTime) As List(Of APP_Reg_Impianti)


        If Veg_Cod <> 0 AndAlso Id_Cod <> 0 Then
            Throw New Exception("Non si può filtrare contemporaneamente un'impianto per specie e destinazione d'uso")
        End If

        Dim rval As List(Of APP_Reg_Impianti) = (
            From reg In dbContext.APP_Reg_Impianti
            Where reg.piva = Piva AndAlso
                  (Sa_Cod = 0 OrElse reg.sa_cod = Sa_Cod) AndAlso
                  ((Veg_Cod = 0 AndAlso Id_Cod = 0) OrElse (reg.veg_cod <> 0 AndAlso reg.veg_cod = Veg_Cod) OrElse (reg.id_cod <> 0 AndAlso reg.id_cod = Id_Cod)) _
            AndAlso (
                reg.validita_inizio_distinta <= DataRiferimento AndAlso
                reg.validita_fine_distinta >= DataRiferimento
            )
        ).OrderBy(Function(f) f.sa_nome).ThenBy(Function(f) f.veg_des).ThenBy(Function(f) f.app_nome).ToList

        Return rval

    End Function

    Public Function Leggi(dbContext As GiasDbContext, listaImpiantiFiltro As List(Of AgronicaCoreModelloSTD.Reg_Impianti)) As List(Of APP_Reg_Impianti)

        Dim rval As List(Of APP_Reg_Impianti) = (
            From reg In dbContext.APP_Reg_Impianti
        ).ToList.Where(Function(imp) listaImpiantiFiltro.Any(Function(impFiltro) imp.piva = impFiltro.piva AndAlso imp.sa_cod = impFiltro.sa_cod AndAlso imp.appezza = impFiltro.appezza AndAlso imp.id_reg = impFiltro.id_reg)).ToList()

        Return rval

    End Function

    Public Function LeggiDatoCodiceAnagrafe(dbContext As GiasDbContext, Codice As String) As APP_Reg_Impianti

        Return dbContext.APP_Reg_Impianti.Where(Function(item) (item.codici_anagrafe_appezzamento = Codice)).ToList().FirstOrDefault()
        'Return dbContext.APP_Reg_Impianti.Where(Function(item) (item.app_nome.Contains(Codice))).ToList().FirstOrDefault()

    End Function

    Public Function LeggiDatoRicettaOperazioneCod(dbContext As GiasDbContext, ricetta_operazione_cod As Integer) As List(Of APP_Reg_Impianti)

        Dim rval As List(Of APP_Reg_Impianti) = (
            From reg In dbContext.APP_Reg_Impianti
            Join opDest In dbContext.APP_Ricette_Destinazioni
                    On reg.piva Equals opDest.Piva _
                    And reg.sa_cod Equals opDest.Sa_Cod _
                    And reg.appezza Equals opDest.Appezza _
                    And reg.id_reg Equals opDest.Id_Reg
            Where opDest.Ricetta_Operazione_Cod = ricetta_operazione_cod AndAlso
                  opDest.Tipo_Destinazione = 0
            Select reg
        ).Distinct().ToList()

        Return rval

    End Function

    Public Function LeggiDatoTempiRisorseCod(dbContext As GiasDbContext, tempi_risorse_cod As Integer) As List(Of APP_Reg_Impianti)

        Dim rval As List(Of APP_Reg_Impianti) = (
            From reg In dbContext.APP_Reg_Impianti
            Join atMov In dbContext.APP_CDG_Movimenti
                    On reg.piva Equals atMov.Piva _
                    And reg.sa_cod Equals atMov.sa_cod _
                    And reg.appezza Equals atMov.appezza _
                    And reg.id_reg Equals atMov.id_reg
            Where atMov.Id_Cdg_Generale = tempi_risorse_cod AndAlso
                  atMov.id_reg <> 0
            Select reg
        ).Distinct().ToList()

        Return rval

    End Function

    Public Function LeggiDatoVisitaCod(dbContext As GiasDbContext, visita_cod As Integer) As List(Of APP_Reg_Impianti)

        Dim rval As List(Of APP_Reg_Impianti) = (
            From reg In dbContext.APP_Reg_Impianti
            Join vis In dbContext.APP_Visite_Destinazioni
                    On reg.piva Equals vis.Piva _
                    And reg.sa_cod Equals vis.Sa_Cod _
                    And reg.appezza Equals vis.Appezza _
                    And reg.id_reg Equals vis.Id_Reg
            Where vis.Visita_Cod = visita_cod AndAlso
                  vis.Id_Reg <> 0
            Select reg
        ).Distinct().ToList()

        Return rval

    End Function

End Class


Public Class Reg_Impianti_W

    Public Sub Scrivi(dbContext As GiasDbContext, impianto As APP_Reg_Impianti, commit As Boolean)

        dbContext.APP_Reg_Impianti.Add(impianto)

        If commit Then
            dbContext.SaveChanges()
        End If

    End Sub

    Public Sub Cancella(dbContext As GiasDbContext, impianto As APP_Reg_Impianti, piva As String)

        If impianto Is Nothing Then
            dbContext.Database.ExecuteSqlCommandAsync("DELETE FROM [APP_Reg_Impianti] WHERE Piva={0}", piva)
        Else
            dbContext.APP_Reg_Impianti.Remove(impianto)
            dbContext.SaveChanges()
        End If

    End Sub

End Class


