Imports System.Data.Entity
Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework

Public Class Zoo_Animali_Lista_Tipi
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Sub Scrivi(Zoo_Animali_Lista_Tipi As AgronicaCoreEntityFramework_POCO.Zoo_Animali_Lista_Tipi,
                      GiasContext As Gias_DeveloperServer_Entities,
                      objParametri_Server As AgronicaCoreParametri,
                      objParametri_Utenti As AgronicaCoreParametri)

        Valida(Zoo_Animali_Lista_Tipi)

        Zoo_Animali_Lista_Tipi.datainvio = DateTime.Now
        Zoo_Animali_Lista_Tipi.Data_Creazione = DateTime.Now
        Zoo_Animali_Lista_Tipi.Data_Modifica = DateTime.Now
        Zoo_Animali_Lista_Tipi.Username_Creazione = objParametri_Server.UsernameOperazione
        Zoo_Animali_Lista_Tipi.Username_Modifica = objParametri_Server.UsernameOperazione

        GiasContext.Zoo_Animali_Lista_Tipi.Add(Zoo_Animali_Lista_Tipi)

    End Sub

    Public Sub Modifica(Zoo_Animali_Lista_Tipi As AgronicaCoreEntityFramework_POCO.Zoo_Animali_Lista_Tipi,
                        GiasContext As Gias_DeveloperServer_Entities,
                        objParametri_Server As AgronicaCoreParametri,
                        objParametri_Utenti As AgronicaCoreParametri)

        Valida(Zoo_Animali_Lista_Tipi)

        Zoo_Animali_Lista_Tipi.Data_Modifica = DateTime.Now
        Zoo_Animali_Lista_Tipi.Username_Modifica = objParametri_Server.UsernameOperazione
        
        GiasContext.Entry(Zoo_Animali_Lista_Tipi).State = EntityState.Modified

    End Sub

    Public Sub Elimina(Zoo_Animali_Lista_Tipi As AgronicaCoreEntityFramework_POCO.Zoo_Animali_Lista_Tipi,
                       GiasContext As Gias_DeveloperServer_Entities,
                       objParametri_Server As AgronicaCoreParametri,
                       objParametri_Utenti As AgronicaCoreParametri)

        GiasContext.Zoo_Animali_Lista_Tipi.Remove(Zoo_Animali_Lista_Tipi)

    End Sub

    Public Sub Valida(Zoo_Animali_Lista_Tipi As AgronicaCoreEntityFramework_POCO.Zoo_Animali_Lista_Tipi)

        If Zoo_Animali_Lista_Tipi.Piva_SuperUser Is Nothing OrElse Zoo_Animali_Lista_Tipi.Piva_SuperUser = "" Then
            Throw New Exception("Piva_SuperUser non impostato")
        End If

        If Zoo_Animali_Lista_Tipi.PIVA Is Nothing OrElse Zoo_Animali_Lista_Tipi.PIVA = "" Then
            Throw New Exception("PIVA non impostato")
        End If

        If Zoo_Animali_Lista_Tipi.GEN_COD = 0 Then
            Throw New Exception("GEN_COD non impostato")
        End If

        If Zoo_Animali_Lista_Tipi.SPE_COD = 0 Then
            Throw New Exception("SPE_COD non impostato")
        End If

        If Zoo_Animali_Lista_Tipi.TIPO_COD = 0 Then
            Throw New Exception("TIPO_COD non impostato")
        End If

    End Sub

End Class
