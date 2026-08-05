Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class EFRubrica

    Public Shared Function CreateRubricaEF(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                           ByRef objParametri As AgronicaCoreParametri,
                                           ByRef numero As String,
                                           ByRef descr As String,
                                           ByRef username As String
                                           ) As AgronicaCoreEntityFramework_POCO.Rubrica

        Dim rub As New AgronicaCoreEntityFramework_POCO.Rubrica
        Dim idGen As New Agro_Sequenze

        rub.cod_rubrica = idGen.NuovoId_Tabella_EF(dal, "Rubrica", 0, 2000000, objParametri)

        If numero Is Nothing Then
            numero = ""
        End If
        rub.numero = numero

        If descr Is Nothing Then
            descr = ""
        End If
        rub.descr = descr

        rub.inviato = 0
        rub.Data_Creazione = DateTime.Now
        rub.Data_Modifica = DateTime.Now
        rub.Validita_Inizio = AGRODATAINIZIO
        rub.Validita_Fine = AGRODATAFINE
        rub.Username_Creazione = username
        rub.Username_Modifica = username
        rub.Validazione = 0
        rub.Data_Validazione = DateTime.Now
        rub.UserName_Validazione = ""

        dal.Rubrica.Add(rub)
        dal.SaveChanges()

        Return rub

    End Function

End Class
