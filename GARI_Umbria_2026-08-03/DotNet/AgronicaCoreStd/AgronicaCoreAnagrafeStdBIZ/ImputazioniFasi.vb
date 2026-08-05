Imports AgronicaCoreAnagrafeStdDAL
Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models

Public Class ImputazioniFasi
    Inherits BaseBiz

    Public Sub New(dbContext As GiasDbContext)
        MyBase.New(dbContext)
    End Sub

    Public Function LeggiImputazioneFasi() As List(Of APP_Imputazioni_Fasi)

        Dim xLettura = New ImputazioniFasi_R()
        Return xLettura.Leggi(dbContext, 0, 0, "")

    End Function

    Public Function EstraiListaProgettiFiltrataPerAttivita(ByVal Id_Attivita As Integer, ByVal Piva As String) As List(Of APP_Imputazioni_Fasi)

        Dim leggi As New ImputazioniFasi_R()
        Return leggi.Leggi(dbContext, Id_Attivita, 0, Piva)

    End Function

    Public Sub ScriviImputazioneFasi(listProgetti As List(Of APP_Imputazioni_Fasi), piva As String, cancella As Boolean)

        Dim count As Integer = 0
        Dim commit As Boolean = False
        Dim numProgetti As Integer = listProgetti.Count
        Dim commitCount As Integer = 100
        Dim xScrittura = New ImputazioniFasi_W()

        If cancella Then
            xScrittura.Cancella(dbContext, Nothing, piva)
        End If

        For Each progetto In listProgetti
            ' forzo partita iva
            progetto.Piva = piva
            count += 1
            commit = count Mod commitCount = 0 OrElse count = numProgetti
            xScrittura.Scrivi(dbContext, progetto, commit)
        Next

    End Sub

    Public Sub CancellaImputazioneFasi(progetto As APP_Imputazioni_Fasi)

        Dim xScrittura = New ImputazioniFasi_W()
        xScrittura.Cancella(dbContext, progetto, Nothing)

    End Sub
End Class
