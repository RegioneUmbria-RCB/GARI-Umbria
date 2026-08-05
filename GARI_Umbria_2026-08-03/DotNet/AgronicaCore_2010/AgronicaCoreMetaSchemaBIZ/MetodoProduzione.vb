Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class MetodoProduzione

    Public Function Leggi(ByRef objParametri_Server As AgronicaCoreParametri) As List(Of AgronicaCoreModelsSTD.metaschema.MetodoProduzione)
        Dim metodoList As New List(Of AgronicaCoreModelsSTD.metaschema.MetodoProduzione)

        metodoList.Add(New AgronicaCoreModelsSTD.metaschema.MetodoProduzione(enum_MetodoProduzione.Integrato) With {.descrizione = "Integrato"})
        metodoList.Add(New AgronicaCoreModelsSTD.metaschema.MetodoProduzione(enum_MetodoProduzione.InConversione) With {.descrizione = "In Conversione"})
        metodoList.Add(New AgronicaCoreModelsSTD.metaschema.MetodoProduzione(enum_MetodoProduzione.Biologico) With {.descrizione = "Biologico"})

        Return metodoList
    End Function

    Public Function Leggi(codice As Integer, ByRef objParametri_Server As AgronicaCoreParametri) As AgronicaCoreModelsSTD.metaschema.MetodoProduzione


        Dim metodi = Leggi(objParametri_Server)
        Dim metodoProduzione = (From el In metodi Where el.codice = codice).FirstOrDefault

        Return metodoProduzione
    End Function

End Class
