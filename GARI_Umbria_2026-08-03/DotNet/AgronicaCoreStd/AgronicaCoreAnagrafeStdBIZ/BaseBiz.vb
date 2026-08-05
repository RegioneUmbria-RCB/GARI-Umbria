Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports AgronicaCoreDataProviderSTD.CostantiPersonalizzate

Public Class BaseBiz


    Friend ReadOnly dbContext As GiasDbContext

    Public Sub New(dbContext As GiasDbContext)
        Me.dbContext = dbContext
    End Sub

    Public Sub ScritturaDatiComuni(oggettoDoveScrivere As APP_Agronica_Entity, username As String)


        Dim dataora As Date = System.DateTime.Now()

        oggettoDoveScrivere.Data_Creazione = dataora
        oggettoDoveScrivere.Data_Modifica = dataora
        oggettoDoveScrivere.Username_Creazione = username
        oggettoDoveScrivere.Username_Modifica = username

        'valori non memorizzati danno luogo a data "01/01/0001", quindi allineo su agro-data.inizio/fine

        If oggettoDoveScrivere.Validita_Inizio < AGRODATAINIZIO Then
            oggettoDoveScrivere.Validita_Inizio = AGRODATAINIZIO
        End If

        If oggettoDoveScrivere.Validita_Fine < AGRODATAINIZIO Then
            oggettoDoveScrivere.Validita_Fine = AGRODATAFINE
        End If

    End Sub

End Class
