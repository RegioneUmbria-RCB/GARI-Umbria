Imports AgronicaCoreEntityFrameworkSTD_POCO.Models

Public Class VisitePerScarico

    Public Property Visite As List(Of APP_Visite)

    Public Property VisiteDettagli As List(Of APP_Visite_Dettagli)

    Public Property VisiteDestinazioni As List(Of APP_Visite_Destinazioni)

    Public Property VisiteDocumenti As List(Of DocumentoPerScarico)

    Public Property VisiteRilievi As RicettePerScarico

    Public Property guid As String

    Public Property cancellato As Boolean

End Class
