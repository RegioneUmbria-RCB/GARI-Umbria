Imports System.Data.Entity
Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework

Public Class Zoo_Animali_Stati_AccrescimentoxFabbisogno

    Public Sub Scrivi(Zoo_Animali_Stati_AccrescimentoxFabbisogno As AgronicaCoreEntityFramework_POCO.Zoo_Animali_Stati_AccrescimentoxFabbisogno,
                      GiasContext As Gias_DeveloperServer_Entities,
                      objParametri_Server As AgronicaCoreParametri,
                      objParametri_Utenti As AgronicaCoreParametri)

        Valida(Zoo_Animali_Stati_AccrescimentoxFabbisogno)

        Zoo_Animali_Stati_AccrescimentoxFabbisogno.datainvio = DateTime.Now
        Zoo_Animali_Stati_AccrescimentoxFabbisogno.Data_Creazione = DateTime.Now
        Zoo_Animali_Stati_AccrescimentoxFabbisogno.Data_Modifica = DateTime.Now
        Zoo_Animali_Stati_AccrescimentoxFabbisogno.Username_Creazione = objParametri_Server.UsernameOperazione
        Zoo_Animali_Stati_AccrescimentoxFabbisogno.Username_Modifica = objParametri_Server.UsernameOperazione

        GiasContext.Zoo_Animali_Stati_AccrescimentoxFabbisogno.Add(Zoo_Animali_Stati_AccrescimentoxFabbisogno)

    End Sub

    Private Sub Valida(Zoo_Animali_Stati_AccrescimentoxFabbisogno As AgronicaCoreEntityFramework_POCO.Zoo_Animali_Stati_AccrescimentoxFabbisogno)

        If Zoo_Animali_Stati_AccrescimentoxFabbisogno.Piva_SuperUser Is Nothing OrElse Zoo_Animali_Stati_AccrescimentoxFabbisogno.Piva_SuperUser = "" Then
            Throw New Exception("Piva_SuperUser non impostato")
        End If

        If Zoo_Animali_Stati_AccrescimentoxFabbisogno.PIVA Is Nothing OrElse Zoo_Animali_Stati_AccrescimentoxFabbisogno.PIVA = "" Then
            Throw New Exception("PIVA non impostato")
        End If

        If Zoo_Animali_Stati_AccrescimentoxFabbisogno.GEN_COD = 0 Then
            Throw New Exception("GEN_COD non specificato")
        End If

        If Zoo_Animali_Stati_AccrescimentoxFabbisogno.SPE_COD = 0 Then
            Throw New Exception("SPE_COD non specificato")
        End If

        If Zoo_Animali_Stati_AccrescimentoxFabbisogno.TIPO_COD = 0 Then
            Throw New Exception("TIPO_COD non specificato")
        End If

        If Zoo_Animali_Stati_AccrescimentoxFabbisogno.STATO_COD = 0 Then
            Throw New Exception("STATO_COD non specificato")
        End If

        If Zoo_Animali_Stati_AccrescimentoxFabbisogno.Mat_Cod = 0 Then
            Throw New Exception("Mat_Cod non specificato")
        End If

    End Sub

    Public Sub Modifica(Zoo_Animali_Stati_AccrescimentoxFabbisogno As AgronicaCoreEntityFramework_POCO.Zoo_Animali_Stati_AccrescimentoxFabbisogno,
                        GiasContext As Gias_DeveloperServer_Entities,
                        objParametri_Server As AgronicaCoreParametri,
                        objParametri_Utenti As AgronicaCoreParametri)

        Valida(Zoo_Animali_Stati_AccrescimentoxFabbisogno)

        Zoo_Animali_Stati_AccrescimentoxFabbisogno.Data_Modifica = DateTime.Now
        Zoo_Animali_Stati_AccrescimentoxFabbisogno.Username_Modifica = objParametri_Server.UsernameOperazione

        GiasContext.Entry(Zoo_Animali_Stati_AccrescimentoxFabbisogno).State = EntityState.Modified

    End Sub

    Public Sub Elimina(Zoo_Animali_Stati_AccrescimentoxFabbisogno As AgronicaCoreEntityFramework_POCO.Zoo_Animali_Stati_AccrescimentoxFabbisogno,
                       GiasContext As Gias_DeveloperServer_Entities,
                       objParametri_Server As AgronicaCoreParametri,
                       objParametri_Utenti As AgronicaCoreParametri)

        GiasContext.Zoo_Animali_Stati_AccrescimentoxFabbisogno.Remove(Zoo_Animali_Stati_AccrescimentoxFabbisogno)

    End Sub

End Class
