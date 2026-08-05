Imports System.Data.Entity
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO

Public Class Zoo_AnimalixStati_Accrescimento
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Sub Scrivi(ByRef Zoo_AnimalixStati_Accrescimento As AgronicaCoreEntityFramework_POCO.Zoo_AnimalixStati_Accrescimento,
                      ByRef GiasContext As Gias_DeveloperServer_Entities,
                      ByRef objParametriServer As AgronicaCoreParametri)

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Zoo_AnimalixStati_Accrescimento.Scrivi()"
        Dim messaggioErrore As String = ""

        Try

            Valorizza(Zoo_AnimalixStati_Accrescimento)

            Zoo_AnimalixStati_Accrescimento.inviato = 0
            Zoo_AnimalixStati_Accrescimento.datainvio = DateTime.Now
            Zoo_AnimalixStati_Accrescimento.Data_Creazione = DateTime.Now
            Zoo_AnimalixStati_Accrescimento.Data_Modifica = DateTime.Now
            Zoo_AnimalixStati_Accrescimento.Username_Creazione = objParametriServer.UsernameOperazione
            Zoo_AnimalixStati_Accrescimento.Username_Modifica = objParametriServer.UsernameOperazione

            If Zoo_AnimalixStati_Accrescimento.Validita_Inizio Is Nothing Then
                Zoo_AnimalixStati_Accrescimento.Validita_Inizio = AGRODATAINIZIO
            End If
            If Zoo_AnimalixStati_Accrescimento.Validita_Fine Is Nothing Then
                Zoo_AnimalixStati_Accrescimento.Validita_Fine = AGRODATAFINE
            End If

            GiasContext.Zoo_AnimalixStati_Accrescimento.Add(Zoo_AnimalixStati_Accrescimento)

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

    Public Sub Modifica(ByRef Zoo_AnimalixStati_Accrescimento As AgronicaCoreEntityFramework_POCO.Zoo_AnimalixStati_Accrescimento,
                        ByRef GiasContext As Gias_DeveloperServer_Entities,
                        ByRef objParametriServer As AgronicaCoreParametri)

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Zoo_AnimalixStati_Accrescimento.Modifica()"
        Dim messaggioErrore As String = ""

        Try

            Valorizza(Zoo_AnimalixStati_Accrescimento)

            Zoo_AnimalixStati_Accrescimento.Data_Modifica = DateTime.Now
            Zoo_AnimalixStati_Accrescimento.Username_Modifica = objParametriServer.UsernameOperazione

            If Zoo_AnimalixStati_Accrescimento.Validita_Inizio Is Nothing Then
                Zoo_AnimalixStati_Accrescimento.Validita_Inizio = AGRODATAINIZIO
            End If

            If Zoo_AnimalixStati_Accrescimento.Validita_Fine Is Nothing Then
                Zoo_AnimalixStati_Accrescimento.Validita_Fine = AGRODATAFINE
            End If

            GiasContext.Entry(Zoo_AnimalixStati_Accrescimento).State = EntityState.Modified

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

    Private Sub Valorizza(ByRef Zoo_AnimalixStati_Accrescimento As AgronicaCoreEntityFramework_POCO.Zoo_AnimalixStati_Accrescimento)

        If Zoo_AnimalixStati_Accrescimento.PIVA Is Nothing AndAlso Zoo_AnimalixStati_Accrescimento.PIVA = "" Then
            Throw New Exception("Impostare PIVA per Zoo_AnimalixStati_Accrescimento")
        End If

        If Zoo_AnimalixStati_Accrescimento.Cod_Progetto = 0 Then
            Throw New Exception("Impostare Cod_Progetto per Zoo_AnimalixStati_Accrescimento")
        End If

        If Zoo_AnimalixStati_Accrescimento.GEN_COD = 0 Then
            Throw New Exception("Impostare GEN_COD per Zoo_AnimalixStati_Accrescimento")
        End If

        If Zoo_AnimalixStati_Accrescimento.SPE_COD = 0 Then
            Throw New Exception("Impostare SPE_COD per Zoo_AnimalixStati_Accrescimento")
        End If

        If Zoo_AnimalixStati_Accrescimento.TIPO_COD = 0 Then
            Throw New Exception("Impostare TIPO_COD per Zoo_AnimalixStati_Accrescimento")
        End If

        If Zoo_AnimalixStati_Accrescimento.STATO_COD = 0 Then
            Throw New Exception("Impostare STATO_COD per Zoo_AnimalixStati_Accrescimento")
        End If

    End Sub

    Public Sub Elimina(ByRef Zoo_AnimalixStati_Accrescimento As AgronicaCoreEntityFramework_POCO.Zoo_AnimalixStati_Accrescimento,
                       ByRef GiasContext As Gias_DeveloperServer_Entities,
                       ByRef objParametriServer As AgronicaCoreParametri)

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Zoo_AnimalixStati_Accrescimento.Elimina()"
        Dim messaggioErrore As String = ""

        Try

            GiasContext.Entry(Zoo_AnimalixStati_Accrescimento).State = EntityState.Modified
            GiasContext.Zoo_AnimalixStati_Accrescimento.Remove(Zoo_AnimalixStati_Accrescimento)

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

End Class
