Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class EFMov_Destinazioni
    Public Shared Function CreateMov_Destinazioni(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                                    ByRef objParametri As AgronicaCoreParametri,
                                                   ByRef piva As String,
                                                   ByRef sa_cod As String,
                                                   ByRef id_agenda As Integer,
                                                   ByRef id_mov As Integer,
                                                   ByRef id_mov_det As Integer,
                                                   ByRef Appezza As Integer,
                                                   ByRef Id_Destinazione As Integer,
                                                   ByRef Tipo_Destinazione As Integer,
                                                   ByRef Qta As Integer,
                                                   ByRef username As String) As AgronicaCoreEntityFramework_POCO.Mov_Destinazioni
        Dim mov = New AgronicaCoreEntityFramework_POCO.Mov_Destinazioni
        mov.Piva = piva
        mov.Sa_Cod = sa_cod
        mov.Id_Agenda = id_agenda
        mov.Id_Mov = id_mov
        mov.Id_Mov_Det = id_mov_det
        mov.Appezza = Appezza
        mov.Id_Destinazione = Id_Destinazione
        mov.Tipo_Destinazione = Tipo_Destinazione
        mov.Qta = Qta
        mov.Qta2 = 0
        mov.inviato = 0
        mov.data_creazione = DateTime.Now
        mov.data_modifica = DateTime.Now
        mov.username_creazione = username
        mov.username_modifica = username
        mov.validita_inizio = AGRODATAINIZIO
        mov.validita_fine = AGRODATAFINE
        mov.Tipo_Scorta = 0
        mov.Scorta_Min = 0
        mov.mov_destinazioni_graphickey = ""
        mov.Qta_Dest1 = 0
        mov.Qta_Dest2 = 0
        mov.QuotaDistribuzione = 0
        dal.Mov_Destinazioni.Add(mov)
        dal.SaveChanges()
        Return mov
    End Function
End Class
