Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class EFStalla
    Public Shared Function CreateStalla(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                           ByRef fabbricato As AgronicaCoreEntityFramework_POCO.Fabbricati,
                                           ByRef username As String) As AgronicaCoreEntityFramework_POCO.Stalla
        Dim sta As New AgronicaCoreEntityFramework_POCO.Stalla
        sta.PIVA = fabbricato.PIVA
        sta.sa_cod = fabbricato.SA_COD
        sta.STA_NUM = fabbricato.Fabbricato_Cod
        sta.STA_DES = ""
        sta.AUSL_COD = 0
        sta.DAT_COSTR = AGRODATAINIZIO
        sta.DAT_CHIU = AGRODATAFINE
        sta.COD_FABB = "1.2.0"
        sta.GEN_COD = 0
        sta.SPE_COD = 0
        sta.IPRO_COD = 0
        sta.X = 0
        sta.Y = 0
        sta.DAT_ULT_AGG = DateTime.Now
        sta.inviato = 0
        sta.Data_Creazione = DateTime.Now
        sta.Data_Modifica = DateTime.Now
        sta.Validita_Inizio = AGRODATAINIZIO
        sta.Validita_Fine = AGRODATAFINE
        sta.Username_Creazione = username
        sta.Username_Modifica = username
        sta.Latitudine = 0
        sta.Longitudine = 0
        sta.CUAA_Proprietario = ""
        sta.CUAA_Detentore = ""
        sta.Denominazione_Detentore = ""
        sta.Denominazione_Proprietario = ""
        dal.Stalla.Add(sta)
        dal.SaveChanges()
        Return sta
    End Function
End Class
