Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports AgronicaCoreVarieStdDAL
Imports Microsoft.EntityFrameworkCore

Public Class Configurazione_Siti
    Friend ReadOnly dbContext As GiasDbContext

    Public Sub New(dbContext As GiasDbContext)
        Me.dbContext = dbContext
    End Sub

    Public Function LeggiConfigurazione(Chiave As String) As APP_Configurazione_Siti

        Dim xLettura = New Configurazione_Siti_R()
        Return xLettura.Leggi(dbContext, Chiave)

    End Function



    Public Function LeggiConfigurazioneAPP() As String

        Dim xLettura = New Configurazione_Siti_R()
        Dim configurazioneLetta As APP_Configurazione_Siti =
            xLettura.Leggi(dbContext, "ConfigurazioneApp")

        If configurazioneLetta Is Nothing Then
            Return ""
        End If


        Return configurazioneLetta.Valore

    End Function

    Public Sub ScriviConfigurazioneApp(pivaSuperUser As String, jsonConfigurazione As String)

        Dim configurazione = New APP_Configurazione_Siti() With
            {
                .Sito_Cod = 100,
                .PivaSuperUser = pivaSuperUser,
                .Chiave = "ConfigurazioneApp",
                .Valore = jsonConfigurazione
            }


        ScriviConfigurazione(configurazione)

    End Sub

    Public Sub ScriviConfigurazione(pivaSuperUser As String, chiave As String, valore As String)

        Dim configurazione = New APP_Configurazione_Siti() With
            {
                .Sito_Cod = 100,
                .PivaSuperUser = pivaSuperUser,
                .Chiave = chiave,
                .Valore = valore
            }

        ScriviConfigurazione(configurazione)

    End Sub

    Public Sub ScriviConfigurazione(configurazione As APP_Configurazione_Siti)

        Dim xScrittura = New Configurazione_Siti_W()
        Dim config = LeggiConfigurazione(configurazione.Chiave)
        If (config Is Nothing) Then
            xScrittura.Scrivi(dbContext, configurazione)
        Else
            config.Valore = configurazione.Valore
            xScrittura.Modifica(dbContext, config)
        End If

    End Sub

End Class
