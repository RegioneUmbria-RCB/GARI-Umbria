Imports AgronicaCoreAnagrafeStdDAL
Imports AgronicaCoreDataProviderSTD.TipiEnumerativi
Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports Microsoft.EntityFrameworkCore

Public Class Attivita
    Inherits BaseBiz

    Public Sub New(dbContext As GiasDbContext)
        MyBase.New(dbContext)
    End Sub

    Public Function LeggiAttivita() As List(Of APP_Attivita)

        Dim xLettura = New Attivita_R()
        Return xLettura.Leggi(dbContext, 0, 0)

    End Function

    Public Function LeggiAttivitaExtraCampagna() As List(Of APP_Attivita)

        Dim xLettura = New Attivita_R()
        Return xLettura.Leggi(dbContext, 0, 1)

    End Function

    Public Function EstraiListaAttivitaPerOperazione(ByVal Piva As String) As List(Of AgronicaCoreModelloSTD.OperazioneAttivita)

        Dim leggi As New Attivita_R()
        Dim listaOperazioni = Operazioni.OperazioniDisponibiliDaTipoRicetta(enum_TipoRicetta_DB.Standard_Destinazioni)
        Return leggi.EstraiListaAttivitaPerOperazione(dbContext, Piva, listaOperazioni)

    End Function

    Public Function EstraiListaAttivitaExtraCampagna(ByVal Piva As String, ByVal Extra_Campagna As Integer) As List(Of AgronicaCoreModelloSTD.Attivita)

        Dim leggi As New Attivita_R()
        Return leggi.EstraiListaAttivitaExtraCampagna(dbContext, Piva, Extra_Campagna)

    End Function

    Public Function EstraiListaAttivitaFiltrataPerOperazione(ByVal Piva As String, ByVal Lav_Cod As Integer) As List(Of AgronicaCoreModelloSTD.Attivita)

        Dim leggi As New Attivita_R()
        Return leggi.EstraiListaAttivitaFiltrataPerOperazione(dbContext, Piva, Lav_Cod)

    End Function

    Public Function EstraiListaAttivitaFiltrataPerCentroAziendale(ByVal Piva As String, ByVal Sa_Cod As Integer, ByVal Inclusa As Integer) As List(Of AgronicaCoreModelloSTD.Attivita)

        Dim leggi As New Attivita_R()
        Return leggi.EstraiListaAttivitaFiltrataPerCentroAziendale(dbContext, Piva, Sa_Cod, Inclusa)

    End Function

    Public Sub ScriviAttivita(listAttivita As List(Of APP_Attivita), cancella As Boolean)

        Dim count As Integer = 0
        Dim commit As Boolean = False
        Dim numAttivita As Integer = listAttivita.Count
        Dim commitCount As Integer = 100
        Dim xScrittura = New Attivita_W()

        If cancella Then
            xScrittura.Cancella(dbContext, Nothing)
        End If

        For Each operazione In listAttivita
            count += 1
            commit = count Mod commitCount = 0 OrElse count = numAttivita
            xScrittura.Scrivi(dbContext, operazione, commit)
        Next

    End Sub

    Public Sub CancellaAttivita(attivita As APP_Attivita)

        Dim xScrittura = New Attivita_W()
        xScrittura.Cancella(dbContext, attivita)

    End Sub

End Class
