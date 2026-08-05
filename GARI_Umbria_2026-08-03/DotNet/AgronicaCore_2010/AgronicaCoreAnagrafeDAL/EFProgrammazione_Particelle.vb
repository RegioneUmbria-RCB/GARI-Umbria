Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class EFProgrammazione_Particelle
    Public Shared Function CreateProgrammazione_ParticelleEF(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                                          ByRef pivaSuperUser As String,
                                                        ByRef programmazione_Entita_Cod As Integer,
                                                        ByRef prov As String,
                                                       ByRef com As String,
                                                       ByRef sezione As String,
                                                       ByRef foglio As Integer,
                                                       ByRef numero As Integer,
                                                       ByRef subalterno As String,
                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                               ByRef username As String) As AgronicaCoreEntityFramework_POCO.Programmazione_Particelle
        Dim progPart As New AgronicaCoreEntityFramework_POCO.Programmazione_Particelle
        Dim idGen As New AgronicaCoreDataProvider.Agro_Sequenze
        progPart.Piva_SuperUser = pivaSuperUser
        progPart.Programmazione_Entita_Cod = programmazione_Entita_Cod
        progPart.Prov = prov
        progPart.Com = com
        progPart.Sezione = sezione
        progPart.Foglio = foglio
        progPart.Numero = numero
        progPart.Subalterno = subalterno
        progPart.inviato = 0
        progPart.Data_Creazione = DateTime.Now
        progPart.Data_Modifica = DateTime.Now
        progPart.Validita_Inizio = AGRODATAINIZIO
        progPart.Validita_Fine = AGRODATAFINE
        progPart.Username_Creazione = username
        progPart.Username_Modifica = username
        dal.Programmazione_Particelle.Add(progPart)
        dal.SaveChanges()
        Return progPart
    End Function
End Class
