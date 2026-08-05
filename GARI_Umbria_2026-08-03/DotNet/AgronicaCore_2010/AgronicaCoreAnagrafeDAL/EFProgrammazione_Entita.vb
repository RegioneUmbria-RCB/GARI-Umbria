Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class EFProgrammazione_Entita
    Public Shared Function CreateProgrammazione_EntitaEF(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                                          ByRef pivaSuperUser As String,
                                                        ByRef programmazione_Cod As Integer,
                                                        ByRef entita_Des As String,
                                                       ByRef piva As String,
                                                       ByRef sa_cod As Integer,
                                                       ByRef campo_Cod As Integer,
                                                       ByRef appezza As Integer,
                                                       ByRef id_reg As Integer,
                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                               ByRef username As String) As AgronicaCoreEntityFramework_POCO.Programmazione_Entita
        Dim programmazioneEntita As New AgronicaCoreEntityFramework_POCO.Programmazione_Entita
        Dim idGen As New AgronicaCoreDataProvider.Agro_Sequenze
        Dim prog_Entita_cod = idGen.NuovoId_Tabella_EF(dal, "Programmazione_Entita", 0, 20000000, objParametri)
        programmazioneEntita.Piva_SuperUser = pivaSuperUser
        programmazioneEntita.Programmazione_Entita_Cod = prog_Entita_cod
        programmazioneEntita.Programmazione_Cod = programmazione_Cod
        programmazioneEntita.Entita_Des = entita_Des
        programmazioneEntita.Piva = piva
        programmazioneEntita.Sa_Cod = sa_cod
        programmazioneEntita.Campo_Cod = campo_Cod
        programmazioneEntita.Appezza = appezza
        programmazioneEntita.Id_Reg = id_reg
        programmazioneEntita.inviato = 0
        programmazioneEntita.Data_Creazione = DateTime.Now
        programmazioneEntita.Data_Modifica = DateTime.Now
        programmazioneEntita.Validita_Inizio = AGRODATAINIZIO
        programmazioneEntita.Validita_Fine = AGRODATAFINE
        programmazioneEntita.Username_Creazione = username
        programmazioneEntita.Username_Modifica = username
        dal.Programmazione_Entita.Add(programmazioneEntita)
        dal.SaveChanges()
        Return programmazioneEntita
    End Function
End Class
