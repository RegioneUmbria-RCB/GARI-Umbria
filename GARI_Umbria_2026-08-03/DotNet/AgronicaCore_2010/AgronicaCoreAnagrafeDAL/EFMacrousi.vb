Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class EFMacrousi
    Inherits AgronicaCoreEntityFramework_POCO.ParticelleCatastalixMacrousi
    Public Shared Function CreateParticelleCatastalixMacrousi(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                               ByRef prov As String,
                                               ByRef com As String,
                                               ByRef sezione As String,
                                               ByRef foglio As String,
                                               ByRef numero As String,
                                               ByRef subalterno As String,
                                               ByRef macrousoCod As String,
                                               ByRef username As String) As AgronicaCoreEntityFramework_POCO.ParticelleCatastalixMacrousi
        Dim macrouso As New AgronicaCoreEntityFramework_POCO.ParticelleCatastalixMacrousi
        Dim idGen As New AgronicaCoreDataProvider.Agro_Sequenze
        Dim macroCod = idGen.NuovoId_Tabella_EF(dal, "ParticelleCatastalixMacrousi", 0, 20000000, objParametri)
        macrouso.Macrouso_Cod = macrousoCod
        macrouso.ID = macroCod
        macrouso.PROV = prov
        macrouso.COM = com
        macrouso.SEZIONE = sezione
        macrouso.FOGLIO = foglio
        macrouso.NUMERO = numero
        macrouso.SUBALTERNO = subalterno
        macrouso.inviato = 0
        macrouso.Data_Creazione = DateTime.Now
        macrouso.Data_Modifica = DateTime.Now
        macrouso.Validita_Inizio = AGRODATAINIZIO
        macrouso.Validita_Fine = AGRODATAFINE
        macrouso.Username_Creazione = username
        macrouso.Username_Modifica = username
        macrouso.Fonte = ""
        macrouso.FonteDescr = ""
        macrouso.Superficie = 0
        'dal.AddObject(macrouso)
        dal.ParticelleCatastalixMacrousi.Add(macrouso)
        dal.SaveChanges()
        Return macrouso
    End Function

End Class
