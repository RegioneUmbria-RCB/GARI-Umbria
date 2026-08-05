Imports AgronicaCoreDataProvider

Public Class BioTipoAttivita
    Inherits AgronicaCoreDataProvider.LogProvider

    Public Function Leggi(ByRef objParametri_Server As AgronicaCoreParametri) As List(Of AgronicaCoreModelsSTD.metaschema.BioTipoAttivita)
        Dim AttivitaList As New List(Of AgronicaCoreModelsSTD.metaschema.BioTipoAttivita)

        AttivitaList.Add(New AgronicaCoreModelsSTD.metaschema.BioTipoAttivita("PV") With {.descrizione = "Produzione vegetale"})
        AttivitaList.Add(New AgronicaCoreModelsSTD.metaschema.BioTipoAttivita("PZ") With {.descrizione = "Produzione zootecnica"})
        AttivitaList.Add(New AgronicaCoreModelsSTD.metaschema.BioTipoAttivita("PVZ") With {.descrizione = "Produzione vegetale e zootecnica"})
        AttivitaList.Add(New AgronicaCoreModelsSTD.metaschema.BioTipoAttivita("TPV") With {.descrizione = "Preparazione vegetale"})
        AttivitaList.Add(New AgronicaCoreModelsSTD.metaschema.BioTipoAttivita("TPZ") With {.descrizione = "Preparazione zootecnica"})
        AttivitaList.Add(New AgronicaCoreModelsSTD.metaschema.BioTipoAttivita("TPVZ") With {.descrizione = "Preparazione vegetale e zootecnica"})
        AttivitaList.Add(New AgronicaCoreModelsSTD.metaschema.BioTipoAttivita("I") With {.descrizione = "Importazione"})
        AttivitaList.Add(New AgronicaCoreModelsSTD.metaschema.BioTipoAttivita("RS") With {.descrizione = "Raccolta spontanea"})
        AttivitaList.Add(New AgronicaCoreModelsSTD.metaschema.BioTipoAttivita("P/TP") With {.descrizione = "Produzione / Preparazione"})
        AttivitaList.Add(New AgronicaCoreModelsSTD.metaschema.BioTipoAttivita("TP/I") With {.descrizione = "Preparazione / Importazione"})
        AttivitaList.Add(New AgronicaCoreModelsSTD.metaschema.BioTipoAttivita("@") With {.descrizione = "Altro"})

        Return AttivitaList
    End Function

End Class
