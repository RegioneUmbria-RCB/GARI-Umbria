Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class EFClassamento
    Inherits AgronicaCoreEntityFramework_POCO.ParticelleCatastaliClassamento
    Public Shared Function CreateParticelleCatastaliClassamento(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                               ByRef prov As String,
                                               ByRef com As String,
                                               ByRef sezione As String,
                                               ByRef foglio As String,
                                               ByRef numero As String,
                                               ByRef subalterno As String,
                                               ByRef Qualita_Cod As Integer,
                                               ByRef username As String) As AgronicaCoreEntityFramework_POCO.ParticelleCatastaliClassamento
        Dim classamento As New AgronicaCoreEntityFramework_POCO.ParticelleCatastaliClassamento
        classamento.QUALITA_COD = Qualita_Cod
        classamento.PROV = prov
        classamento.COM = com
        classamento.SEZIONE = sezione
        classamento.FOGLIO = foglio
        classamento.NUMERO = numero
        classamento.SUBALTERNO = subalterno
        classamento.Porzione = ""
        classamento.CLASSE = ""
        classamento.Sup_Classe = 0
        classamento.REDDITO_AGRARIO = 0
        classamento.REDDITO_DOMINICALE = 0
        classamento.Deduzione = ""
        classamento.inviato = 0
        classamento.Data_Creazione = DateTime.Now
        classamento.Data_Modifica = DateTime.Now
        classamento.Validita_Inizio = AGRODATAINIZIO
        classamento.Validita_Fine = AGRODATAFINE
        classamento.Username_Creazione = username
        classamento.Username_Modifica = username

        'dal.AddObject(macrouso)
        dal.ParticelleCatastaliClassamento.Add(classamento)
        dal.SaveChanges()
        Return classamento
    End Function

End Class
