Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreModelsSTD.baseClass
Imports AgronicaCoreModelsSTD.metaschema.utilizzi

Public Class SpecieVegetali_R

    Public Function Leggi_Da_Cultivar(
        Cul_Cod As Integer, Cul_Des As String,
        xFiltroAgiuntivo As String, xOderBy As String,
        obj_server As AgronicaCoreParametri
    ) As Specie
        Dim objSpecVeg As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R()
        Dim DT = objSpecVeg.Leggi_con_Cul_Des(0, Cul_Cod, xFiltroAgiuntivo, xOderBy, obj_server)
        Return DT.AsEnumerable.
            Select(Function(row) New Specie(row.Item("Veg_Cod"), row.Item("Veg_Des"))).
            FirstOrDefault
    End Function

    Public Function LeggiGruppiVegetali(objServer As AgronicaCoreParametri) As IEnumerable(Of BaseCodeDescr)
        Dim objGrVeg As New AgronicaCoreMetaSchemaDAL.GruppoVegetale_R
        Dim noFiltro = ""
        Return objGrVeg.Leggi(0, enumSelezioneVariabile.Selezione_TabellaCompleta, noFiltro, noFiltro, objServer).
        AsEnumerable.
        Select(Function(row) New BaseCodeDescr(row.Item("Gru_Cod"), row.Item("Gru_Des")))
    End Function

    Public Function mapCodificaAgeaToSpecie(
        codificheAgea As IEnumerable(Of AgronicaCoreMetaSchemaDAL.SpecieAgea),
        ByRef objParametri As AgronicaCoreParametri
    ) As String
        Dim codifiche As New AgronicaCoreMetaSchemaDAL.Agea_Codifiche_R
        If codificheAgea Is Nothing Then
            codificheAgea = New List(Of AgronicaCoreMetaSchemaDAL.SpecieAgea)
        End If
        Return codifiche.LeggiVegCodDaCodificaAgea(codificheAgea, objParametri).
            DefaultIfEmpty.Aggregate(Function(acc, x) acc & "," & x)
    End Function

End Class