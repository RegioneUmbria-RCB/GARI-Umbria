Imports System.Data.Entity
Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework

Public Class Zoo_Animali_Lista_Stati_Accrescimento
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Sub Scrivi(Zoo_Animali_Lista_Stati_Accrescimento As AgronicaCoreEntityFramework_POCO.Zoo_Animali_Lista_Stati_Accrescimento,
                      GiasContext As Gias_DeveloperServer_Entities,
                      objParametri_Server As AgronicaCoreParametri,
                      objParametri_Utenti As AgronicaCoreParametri)

        Valida(Zoo_Animali_Lista_Stati_Accrescimento)

        Zoo_Animali_Lista_Stati_Accrescimento.datainvio = DateTime.Now
        Zoo_Animali_Lista_Stati_Accrescimento.Data_Creazione = DateTime.Now
        Zoo_Animali_Lista_Stati_Accrescimento.Data_Modifica = DateTime.Now
        Zoo_Animali_Lista_Stati_Accrescimento.Username_Creazione = objParametri_Server.UsernameOperazione
        Zoo_Animali_Lista_Stati_Accrescimento.Username_Modifica = objParametri_Server.UsernameOperazione

        GiasContext.Zoo_Animali_Lista_Stati_Accrescimento.Add(Zoo_Animali_Lista_Stati_Accrescimento)

    End Sub

    Public Sub Modifica(Zoo_Animali_Lista_Stati_Accrescimento As AgronicaCoreEntityFramework_POCO.Zoo_Animali_Lista_Stati_Accrescimento,
                        GiasContext As Gias_DeveloperServer_Entities,
                        objParametri_Server As AgronicaCoreParametri,
                        objParametri_Utenti As AgronicaCoreParametri)

        Valida(Zoo_Animali_Lista_Stati_Accrescimento)

        Zoo_Animali_Lista_Stati_Accrescimento.Data_Modifica = DateTime.Now
        Zoo_Animali_Lista_Stati_Accrescimento.Username_Modifica = objParametri_Server.UsernameOperazione

        GiasContext.Entry(Zoo_Animali_Lista_Stati_Accrescimento).State = EntityState.Modified

    End Sub

    Public Sub Elimina(Zoo_Animali_Lista_Stati_Accrescimento As AgronicaCoreEntityFramework_POCO.Zoo_Animali_Lista_Stati_Accrescimento,
                       GiasContext As Gias_DeveloperServer_Entities,
                       objParametri_Server As AgronicaCoreParametri,
                       objParametri_Utenti As AgronicaCoreParametri)

        GiasContext.Zoo_Animali_Lista_Stati_Accrescimento.Remove(Zoo_Animali_Lista_Stati_Accrescimento)

    End Sub

    Private Sub Valida(Zoo_Animali_Lista_Stati_Accrescimento As AgronicaCoreEntityFramework_POCO.Zoo_Animali_Lista_Stati_Accrescimento)

        If Zoo_Animali_Lista_Stati_Accrescimento.Piva_SuperUser Is Nothing OrElse
           Zoo_Animali_Lista_Stati_Accrescimento.Piva_SuperUser = "" Then
            Throw New Exception("Piva_SuperUser non impostato")
        End If

        If Zoo_Animali_Lista_Stati_Accrescimento.PIVA Is Nothing OrElse
           Zoo_Animali_Lista_Stati_Accrescimento.PIVA = "" Then
            Throw New Exception("PIVA non impostato")
        End If

        If Zoo_Animali_Lista_Stati_Accrescimento.GEN_COD = 0 Then
            Throw New Exception("GEN_COD non impostato")
        End If

        If Zoo_Animali_Lista_Stati_Accrescimento.SPE_COD = 0 Then
            Throw New Exception("SPE_COD non impostato")
        End If

        If Zoo_Animali_Lista_Stati_Accrescimento.TIPO_COD = 0 Then
            Throw New Exception("TIPO_COD non impostato")
        End If

        If Zoo_Animali_Lista_Stati_Accrescimento.STATO_COD = 0 Then
            Throw New Exception("STATO_COD non impostato")
        End If

        'If Zoo_Animali_Lista_Stati_Accrescimento.Giorno_Da Is Nothing Then
        '    Zoo_Animali_Lista_Stati_Accrescimento.Giorno_Da = 0
        'End If

        'If Zoo_Animali_Lista_Stati_Accrescimento.Giorno_A Is Nothing Then
        '    Zoo_Animali_Lista_Stati_Accrescimento.Giorno_A = 0
        'End If

    End Sub

End Class
