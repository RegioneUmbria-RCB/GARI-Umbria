Imports System.Data.Entity
Imports AgronicaCoreEntityFramework

Public Class Zoo_Configurazione
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Sub Scrivi(Zoo_Configurazione As AgronicaCoreEntityFramework_POCO.Zoo_Configurazione,
                      GiasContext As Gias_DeveloperServer_Entities,
                      objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                      objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Zoo_Configurazione.datainvio = DateTime.Now
        Zoo_Configurazione.Data_Creazione = DateTime.Now
        Zoo_Configurazione.Data_Modifica = DateTime.Now
        Zoo_Configurazione.Username_Creazione = objParametri_Server.UsernameOperazione
        Zoo_Configurazione.Username_Modifica = objParametri_Server.UsernameOperazione

        GiasContext.Zoo_Configurazione.Add(Zoo_Configurazione)

    End Sub

    Public Sub Modifica(Zoo_Configurazione As AgronicaCoreEntityFramework_POCO.Zoo_Configurazione,
                        GiasContext As Gias_DeveloperServer_Entities,
                        objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                        objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri)


        Zoo_Configurazione.Data_Modifica = DateTime.Now
        Zoo_Configurazione.Username_Modifica = objParametri_Server.UsernameOperazione

        GiasContext.Entry(Zoo_Configurazione).State = EntityState.Modified

    End Sub

    Public Sub Elimina(Zoo_Configurazione As AgronicaCoreEntityFramework_POCO.Zoo_Configurazione,
                       GiasContext As Gias_DeveloperServer_Entities,
                       objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                       objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri)


        GiasContext.Zoo_Configurazione.Remove(Zoo_Configurazione)

    End Sub

    Public Sub Elimina(PIVA As String,
                       sa_Cod As Integer,
                       GEN_COD As Integer,
                       SPE_COD As Integer,
                       GiasContext As Gias_DeveloperServer_Entities,
                       objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                       objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim Zoo_Configurazione = From zc In GiasContext.Zoo_Configurazione
                                 Where zc.PIVA = PIVA AndAlso
                                       zc.sa_cod = sa_Cod AndAlso
                                       zc.GEN_COD = GEN_COD AndAlso
                                       zc.SPE_COD = SPE_COD
                                 Select zc

        If Zoo_Configurazione.Count > 0 Then
            GiasContext.Zoo_Configurazione.Remove(Zoo_Configurazione.FirstOrDefault)
        End If

    End Sub

End Class
