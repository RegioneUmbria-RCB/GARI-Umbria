Imports System.Data.Entity
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO

Public Class Zoo_AnimalixAnomalie
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Sub Scrivi(ByRef Zoo_AnimalixAnomalie As AgronicaCoreEntityFramework_POCO.Zoo_AnimalixAnomalie,
                      ByRef GiasContext As Gias_DeveloperServer_Entities,
                      ByRef objParametriServer As AgronicaCoreParametri)

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Zoo_AnimalixAnomalie.Scrivi()"
        Dim messaggioErrore As String = ""

        Try
            Valorizza(Zoo_AnimalixAnomalie)

            Zoo_AnimalixAnomalie.inviato = 0
            Zoo_AnimalixAnomalie.datainvio = DateTime.Now
            Zoo_AnimalixAnomalie.Data_Creazione = DateTime.Now
            Zoo_AnimalixAnomalie.Data_Modifica = DateTime.Now
            Zoo_AnimalixAnomalie.Username_Creazione = objParametriServer.UsernameOperazione
            Zoo_AnimalixAnomalie.Username_Modifica = objParametriServer.UsernameOperazione

            If Zoo_AnimalixAnomalie.Validita_Inizio < AGRODATAINIZIO Then
                Zoo_AnimalixAnomalie.Validita_Inizio = AGRODATAINIZIO
            End If
            If Zoo_AnimalixAnomalie.Validita_Fine > AGRODATAFINE Then
                Zoo_AnimalixAnomalie.Validita_Fine = AGRODATAFINE
            End If

            GiasContext.Zoo_AnimalixAnomalie.Add(Zoo_AnimalixAnomalie)

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

    Public Sub Modifica(ByRef Zoo_AnimalixAnomalie As AgronicaCoreEntityFramework_POCO.Zoo_AnimalixAnomalie,
                        ByRef GiasContext As Gias_DeveloperServer_Entities,
                        ByRef objParametriServer As AgronicaCoreParametri)

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Zoo_AnimalixAnomalie.Modifica()"
        Dim messaggioErrore As String = ""

        Try
            Valorizza(Zoo_AnimalixAnomalie)

            Zoo_AnimalixAnomalie.Data_Modifica = DateTime.Now
            Zoo_AnimalixAnomalie.Username_Modifica = objParametriServer.UsernameOperazione

            If Zoo_AnimalixAnomalie.Validita_Inizio < AGRODATAINIZIO Then
                Zoo_AnimalixAnomalie.Validita_Inizio = AGRODATAINIZIO
            End If
            If Zoo_AnimalixAnomalie.Validita_Fine > AGRODATAFINE Then
                Zoo_AnimalixAnomalie.Validita_Fine = AGRODATAFINE
            End If

            GiasContext.Entry(Zoo_AnimalixAnomalie).State = EntityState.Modified

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

    Private Sub Valorizza(ByRef Zoo_AnimalixAnomalie As AgronicaCoreEntityFramework_POCO.Zoo_AnimalixAnomalie)

        If Zoo_AnimalixAnomalie.Piva Is Nothing AndAlso Zoo_AnimalixAnomalie.Piva = "" Then
            Throw New Exception("Impostare Piva per Zoo_AnimalixAnomalie")
        End If

        If Zoo_AnimalixAnomalie.Cod_Animale = 0 Then
            Throw New Exception("Impostare Cod_Animale per Zoo_AnimalixAnomalie")
        End If

    End Sub

    Public Sub Elimina(ByRef Zoo_AnimalixAnomalie As AgronicaCoreEntityFramework_POCO.Zoo_AnimalixAnomalie,
                       ByRef GiasContext As Gias_DeveloperServer_Entities,
                       ByRef objParametriServer As AgronicaCoreParametri)

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Zoo_AnimalixAnomalie.Elimina()"
        Dim messaggioErrore As String = ""

        Try
            GiasContext.Entry(Zoo_AnimalixAnomalie).State = EntityState.Modified
            GiasContext.Zoo_AnimalixAnomalie.Remove(Zoo_AnimalixAnomalie)

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

End Class
