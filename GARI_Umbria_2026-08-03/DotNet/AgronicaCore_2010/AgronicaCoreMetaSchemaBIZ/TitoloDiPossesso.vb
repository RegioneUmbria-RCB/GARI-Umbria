Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class TitoloDiPossesso
    Inherits AgronicaCoreDataProvider.LogProvider

    Public Function Leggi(ByRef objParametri_Server As AgronicaCoreParametri) As List(Of AgronicaCoreModelsSTD.metaschema.TitoloDiPossesso)
        Dim TitoloList As New List(Of AgronicaCoreModelsSTD.metaschema.TitoloDiPossesso)

        TitoloList.Add(New AgronicaCoreModelsSTD.metaschema.TitoloDiPossesso(enum_TitoloPossesso.Altro) With {.descrizione = "Altro"})
        TitoloList.Add(New AgronicaCoreModelsSTD.metaschema.TitoloDiPossesso(enum_TitoloPossesso.Proprieta) With {.descrizione = "Proprietà"})
        TitoloList.Add(New AgronicaCoreModelsSTD.metaschema.TitoloDiPossesso(enum_TitoloPossesso.Comodato) With {.descrizione = "Comodato d'uso"})
        TitoloList.Add(New AgronicaCoreModelsSTD.metaschema.TitoloDiPossesso(enum_TitoloPossesso.AffittoContratto) With {.descrizione = "Affitto con contratto"})
        TitoloList.Add(New AgronicaCoreModelsSTD.metaschema.TitoloDiPossesso(enum_TitoloPossesso.AffittoSenzaContratto) With {.descrizione = "Affitto senza contratto"})
        TitoloList.Add(New AgronicaCoreModelsSTD.metaschema.TitoloDiPossesso(enum_TitoloPossesso.InContoTerzi) With {.descrizione = "In conto terzi"})
        TitoloList.Add(New AgronicaCoreModelsSTD.metaschema.TitoloDiPossesso(enum_TitoloPossesso.InConvenzione) With {.descrizione = "In convenzione"})
        TitoloList.Add(New AgronicaCoreModelsSTD.metaschema.TitoloDiPossesso(enum_TitoloPossesso.InCompartecipazione) With {.descrizione = "In compartecipazione"})

        Return TitoloList
    End Function

    Public Function Leggi(codice As Integer, ByRef objParametri_Server As AgronicaCoreParametri) As AgronicaCoreModelsSTD.metaschema.TitoloDiPossesso


        Dim titoli = Leggi(objParametri_Server)
        Dim titoloDiPossesso = (From el In titoli Where el.codice = codice).FirstOrDefault

        Return titoloDiPossesso
    End Function

End Class
