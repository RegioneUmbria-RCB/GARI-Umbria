Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class EFZoo_Animali
    Public Shared Function CreateZoo_Animali(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                           ByRef fabbricato As AgronicaCoreEntityFramework_POCO.Fabbricati,
                                           ByRef mov_Dettagli As AgronicaCoreEntityFramework_POCO.Mov_Destinazioni,
                                           ByRef matricola As String,
                                           ByRef gen_cod As Integer,
                                           ByRef spe_cod As Integer,
                                           ByRef ipro_cod As Integer,
                                           ByRef cat_cod As Integer,
                                           ByRef raz_cod As Integer,
                                           ByRef username As String) As AgronicaCoreEntityFramework_POCO.Zoo_Animali
        Dim zoo As New AgronicaCoreEntityFramework_POCO.Zoo_Animali
        zoo.PIVA = fabbricato.PIVA
        zoo.sa_cod = fabbricato.SA_COD
        zoo.Cod_Progetto = mov_Dettagli.Id_Destinazione
        zoo.Matricola = ""
        zoo.GEN_COD = gen_cod
        zoo.SPE_COD = spe_cod
        zoo.IPRO_COD = ipro_cod
        zoo.CAT_COD = cat_cod
        zoo.RAZ_COD = raz_cod
        zoo.Nome = ""
        zoo.Collare = ""
        zoo.NOME_AIA = ""
        zoo.MATRICOLA_AIA = ""
        zoo.DAT_NASCITA = Now.Date
        zoo.PROV_NASCITA = ""
        zoo.STATO_NASCITA = ""
        zoo.AUA_AZI_NASCITA = ""
        zoo.AUSL_AZI_NASCITA = ""
        zoo.Sesso = ""
        zoo.MAT_MADRE = ""
        zoo.MAT_PADRE = ""
        zoo.CF_PROPRIETARIO = ""
        zoo.CF_DETENTORE = ""
        zoo.PRESENTE = 0
        zoo.inviato = 0
        zoo.datainvio = DateTime.Now
        zoo.Data_Creazione = DateTime.Now
        zoo.Data_Modifica = DateTime.Now
        zoo.Validita_Inizio = AGRODATAINIZIO
        zoo.Validita_Fine = AGRODATAFINE
        zoo.Username_Creazione = username
        zoo.Username_Modifica = username
        dal.Zoo_Animali.Add(zoo)
        dal.SaveChanges()
        Return zoo
    End Function
End Class
