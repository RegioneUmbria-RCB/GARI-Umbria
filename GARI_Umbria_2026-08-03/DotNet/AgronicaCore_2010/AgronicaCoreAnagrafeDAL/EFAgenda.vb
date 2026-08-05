Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider

Public Class EFAgenda
    Public Shared Function CreateAgenda(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                        ByRef objParametri As AgronicaCoreParametri,
                                        ByRef piva As String,
                                        ByRef sa_cod As String,
                                        ByRef lav_cod As Integer,
                                        ByRef des_lib As String,
                                        ByRef username As String
                                        ) As AgronicaCoreEntityFramework_POCO.Agenda

        Dim agenda As New AgronicaCoreEntityFramework_POCO.Agenda
        agenda.PIVA = piva
        agenda.Sa_Cod = sa_cod
        Dim idGen As New Agro_Sequenze
        agenda.Id_Agenda = idGen.NuovoId_Tabella_EF(dal, "Agenda", 0, 2000000, objParametri)
        agenda.Lav_Cod = lav_cod
        agenda.des_lib = des_lib
        agenda.Blocco_Flag = 0
        agenda.Blocco_Data = AGRODATAINIZIO
        agenda.Blocco_Username = ""
        agenda.inviato = 0
        agenda.Data_Creazione = DateTime.Now
        agenda.Data_Modifica = DateTime.Now
        agenda.Username_Creazione = username
        agenda.Username_Modifica = username
        agenda.Validita_Inizio = AGRODATAINIZIO
        agenda.Validita_Fine = AGRODATAFINE
        agenda.LINEA_COD = 0
        agenda.PREPARAZIONE_COD = 0
        agenda.ID_TRASFORMAZIONE = 0
        agenda.Tipo_Accettazione = 0
        agenda.Stato_Export = 0
        agenda.Stato_Export_2 = 0
        agenda.Tipo_Visibilita = 0
        agenda.ChkCoge_Manuale = 0
        agenda.Id_Attivita = 0
        agenda.Modulo = 0
        agenda.Audit_Cod = 0
        dal.Agenda.Add(agenda)
        dal.SaveChanges()
        Return agenda
    End Function

    Public Shared Function CreateMovimenti(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                    ByRef agenda As AgronicaCoreEntityFramework_POCO.Agenda,
                                    ByRef objParametri As AgronicaCoreParametri,
                                    ByRef cod_RisUm As Integer,
                                    ByRef cau_Mov As String,
                                    ByRef Mov_Desc As String,
                                    ByRef username As String) As AgronicaCoreEntityFramework_POCO.Movimenti
        Return EFMovimenti.CreateMovimenti(dal, objParametri, agenda.PIVA, agenda.Sa_Cod, agenda.Id_Agenda, cod_RisUm, cau_Mov, Mov_Desc, username)
    End Function

End Class
