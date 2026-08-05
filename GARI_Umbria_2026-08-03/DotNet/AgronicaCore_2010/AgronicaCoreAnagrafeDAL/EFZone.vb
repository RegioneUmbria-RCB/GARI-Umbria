Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class EFZone
    Inherits AgronicaCoreEntityFramework_POCO.ZonexParticelle
    Public Shared Function CreateZonexParticelle(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                               ByRef prov As String,
                                               ByRef com As String,
                                               ByRef sezione As String,
                                               ByRef foglio As Integer,
                                               ByRef numero As Integer,
                                               ByRef subalterno As String,
                                               ByRef Zona_Cod As Integer,
                                               ByRef username As String) As AgronicaCoreEntityFramework_POCO.ZonexParticelle
        Dim zona As New AgronicaCoreEntityFramework_POCO.ZonexParticelle
        zona.Zona_Cod = Zona_Cod
        zona.Piva_SuperUser = objParametri.PivaSuperUser
        zona.PROV = prov
        zona.COM = com
        zona.SEZIONE = sezione
        zona.FOGLIO = foglio
        zona.NUMERO = numero
        zona.SUBALTERNO = subalterno
        zona.Area = 0
        zona.inviato = 0
        zona.Data_Creazione = DateTime.Now
        zona.Data_Modifica = DateTime.Now
        zona.Validita_Inizio = AGRODATAINIZIO
        zona.Validita_Fine = AGRODATAFINE
        zona.Username_Creazione = username
        zona.Username_Modifica = username
        zona.Fonte = ""
        zona.FonteDescr = ""
        zona.Conforme = ""
        'dal.AddObject(macrouso)
        dal.ZonexParticelle.Add(zona)
        dal.SaveChanges()
        Return zona
    End Function

End Class
