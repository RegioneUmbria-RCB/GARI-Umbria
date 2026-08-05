Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class EFProgrammazione_Testata
    Public Shared Function CreateProgrammazione_TestataEF(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                                          ByRef pivaSuperUser As String,
                                                       ByRef Programmazione_Des As String,
                                                       ByRef Programmazione_Des_Long As String,
                                                       ByRef piva As String,
                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                               ByRef username As String) As AgronicaCoreEntityFramework_POCO.Programmazione_Testata
        Dim programmazioneTestata As New AgronicaCoreEntityFramework_POCO.Programmazione_Testata
        Dim idGen As New AgronicaCoreDataProvider.Agro_Sequenze
        Dim prog_cod = idGen.NuovoId_Tabella_EF(dal, "Programmazione_Testata", 0, 20000000, objParametri)
        programmazioneTestata.Piva_SuperUser = pivaSuperUser
        programmazioneTestata.Piva = piva
        programmazioneTestata.Programmazione_Cod = prog_cod
        programmazioneTestata.Inviato = 0
        programmazioneTestata.Data_Creazione = DateTime.Now
        programmazioneTestata.Data_Modifica = DateTime.Now
        programmazioneTestata.Validita_Inizio = AGRODATAINIZIO
        programmazioneTestata.Validita_Fine = AGRODATAFINE
        programmazioneTestata.Username_Creazione = username
        programmazioneTestata.Username_Modifica = username
        dal.Programmazione_Testata.Add(programmazioneTestata)
        dal.SaveChanges()
        Return programmazioneTestata
    End Function
End Class
