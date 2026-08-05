Imports System.Data.Entity
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class EFIst_Credito
    Public Shared Function CreateIstCredito(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                           ByRef piva As String,
                                           ByRef sa_cod As Integer,
                                           ByRef description As String,
                                           ByRef username As String) As AgronicaCoreEntityFramework_POCO.Ist_Credito
        Dim ist As New AgronicaCoreEntityFramework_POCO.Ist_Credito
        Dim idGen As New AgronicaCoreDataProvider.Agro_Sequenze
        ist.Piva = piva
        ist.Sa_Cod = sa_cod
        ist.Cod_Istituto = idGen.NuovoId_Tabella_EF(dal, "Ist_Credito", 0, 2000000, objParametri)
        ist.Istituto_Des = description
        ist.Filiale = 0
        ist.Per_Risorsa = 1
        ist.Inviato = 0
        ist.Validita_Inizio = AGRODATAINIZIO
        ist.Validita_Fine = AGRODATAFINE
        ist.Username_Creazione = username
        ist.Username_Modifica = username
        dal.Ist_Credito.Add(ist)
        dal.SaveChanges()
        Return ist
    End Function

    Public Shared Function CreateIndirizzoIstCredito(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                           ByRef credito As AgronicaCoreEntityFramework_POCO.Ist_Credito,
                                           ByRef username As String) As AgronicaCoreEntityFramework_POCO.Indirizzi
        Dim indirizzo = EFIndirizzi.CreateIndirizziEF(dal, objParametri, username)
        credito.Cod_Indirizzo = indirizzo.cod_indirizzo
        dal.Entry(credito).State = EntityState.Modified
        dal.SaveChanges()
        Return indirizzo
    End Function

End Class
