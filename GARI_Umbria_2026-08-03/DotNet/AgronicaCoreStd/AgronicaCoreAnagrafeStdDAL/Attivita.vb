Imports AgronicaCoreDataProviderSTD.TipiEnumerativi
Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports Microsoft.EntityFrameworkCore

Public Class Attivita_R

    Public Function Leggi(dbContext As GiasDbContext, Id_Attivita As Integer, Extra_Campagna As Integer) As List(Of APP_Attivita)

        Return dbContext.APP_Attivita.Where(
            Function(item) ((Id_Attivita = 0 OrElse item.Id_Attivita = Id_Attivita) AndAlso (Extra_Campagna = 0 OrElse item.Attivita_Extra_Campagna = Extra_Campagna))
        ).OrderBy(Function(f) f.Desc).ToList()

    End Function

    Public Function EstraiListaAttivitaPerOperazione(dbContext As GiasDbContext, ByVal Piva As String, listaOperazioni As List(Of Integer)) As List(Of AgronicaCoreModelloSTD.OperazioneAttivita)

        Dim listaOperazioneAttivita As List(Of AgronicaCoreModelloSTD.OperazioneAttivita)

        listaOperazioneAttivita = (
                From m In dbContext.APP_AttivitaXOperazioni
                Join a In dbContext.APP_Attivita On m.Id_Attivita Equals a.Id_Attivita
                Join o In dbContext.APP_Operazioni On m.Lav_Cod Equals o.lav_cod
                Where (a.Piva = Piva OrElse a.Sa_Cod = -1) AndAlso listaOperazioni.Contains(o.lav_cod)
                Select New AgronicaCoreModelloSTD.OperazioneAttivita With {
                    .operazione = New AgronicaCoreModelloSTD.Operazione With {
                        .lav_cod = o.lav_cod,
                        .lav_des = o.lav_des
                    },
                    .attivita = New AgronicaCoreModelloSTD.Attivita With {
                        .Cod = m.Id_Attivita,
                        .Descrizione = m.Descrizione
                    }
                }
            ).Distinct().OrderBy(Function(f) f.Descrizione).ToList()

        Return listaOperazioneAttivita

    End Function

    ''' <summary>
    ''' legge la lista delle attivita filtrata per operazione
    ''' </summary>
    ''' <param name="dbContext"></param>
    ''' <param name="Lav_Cod">codice operazione, se vale 0 non viene applicato alcun filtro alle attività</param>
    ''' <returns></returns>
    Public Function EstraiListaAttivitaFiltrataPerOperazione(dbContext As GiasDbContext, ByVal Piva As String, ByVal Lav_Cod As Integer) As List(Of AgronicaCoreModelloSTD.Attivita)

        Dim listaAttivita As List(Of AgronicaCoreModelloSTD.Attivita)

        If Lav_Cod = 0 Then
            listaAttivita = (
                From m In dbContext.APP_Attivita
                Where m.Piva = Piva OrElse m.Sa_Cod = -1
                Select New AgronicaCoreModelloSTD.Attivita With {
                    .Cod = m.Id_Attivita,
                    .Descrizione = m.Desc
                    }
            ).Distinct().OrderBy(Function(f) f.Descrizione).ToList()
        Else
            listaAttivita = (
                From m In dbContext.APP_AttivitaXOperazioni
                Join a In dbContext.APP_Attivita On m.Id_Attivita Equals a.Id_Attivita
                Where (m.Lav_Cod = Lav_Cod AndAlso (a.Piva = Piva OrElse a.Sa_Cod = -1))
                Select New AgronicaCoreModelloSTD.Attivita With {
                    .Cod = m.Id_Attivita,
                    .Descrizione = m.Descrizione
                    }
            ).Distinct().OrderBy(Function(f) f.Descrizione).ToList()
        End If

        Return listaAttivita

    End Function

    ''' <summary>
    ''' legge la lista delle attivita filtrata per centro aziendale
    ''' </summary>
    ''' <param name="dbContext"></param>
    ''' <param name="Sa_Cod">codice centro aziendale</param>
    ''' <returns></returns>
    Public Function EstraiListaAttivitaFiltrataPerCentroAziendale(dbContext As GiasDbContext, ByVal Piva As String, ByVal Sa_Cod As Integer, ByVal Inclusa As Integer) As List(Of AgronicaCoreModelloSTD.Attivita)

        Dim listaAttivita As List(Of AgronicaCoreModelloSTD.Attivita)

        listaAttivita = (
                From m In dbContext.APP_AttivitaXCentri_Aziendali
                Join a In dbContext.APP_Attivita On m.Id_Attivita Equals a.Id_Attivita
                Where (m.Inclusa = Inclusa AndAlso m.Sa_Cod = Sa_Cod AndAlso (a.Piva = Piva OrElse a.Sa_Cod = -1))
                Select New AgronicaCoreModelloSTD.Attivita With {
                    .Cod = m.Id_Attivita,
                    .Descrizione = a.Desc
                    }
            ).Distinct().OrderBy(Function(f) f.Descrizione).ToList()

        Return listaAttivita

    End Function

    ''' <summary>
    ''' legge la lista delle attivita filtrata per operazione
    ''' </summary>
    ''' <param name="dbContext"></param>
    ''' <returns></returns>
    Public Function EstraiListaAttivitaExtraCampagna(dbContext As GiasDbContext, Piva As String, Extra_Campagna As Integer) As List(Of AgronicaCoreModelloSTD.Attivita)

        Dim listaAttivita As List(Of AgronicaCoreModelloSTD.Attivita)

        'Join p In dbContext.APP_Imputazioni_Fasi On m.Id_Attivita Equals p.Id_Attivita
        'Where(m.Attivita_Extra_Campagna = 1 And (m.Piva = Piva Or m.Sa_Cod = -1) And p.Piva = Piva)

        listaAttivita = (
                From m In dbContext.APP_Attivita
                Where ((Extra_Campagna = 0 OrElse m.Attivita_Extra_Campagna = 1) AndAlso (m.Piva = Piva OrElse m.Sa_Cod = -1))
                Select New AgronicaCoreModelloSTD.Attivita With {
                    .Cod = m.Id_Attivita,
                    .Descrizione = m.Desc
                    }
            ).Distinct().OrderBy(Function(f) f.Descrizione).ToList()

        Return listaAttivita

    End Function

End Class

Public Class Attivita_W

    Public Sub Scrivi(dbContext As GiasDbContext, attivita As APP_Attivita, commit As Boolean)

        dbContext.APP_Attivita.Add(attivita)

        If commit Then
            dbContext.SaveChanges()
        End If

    End Sub

    Public Sub Cancella(dbContext As GiasDbContext, attivita As APP_Attivita)

        If attivita Is Nothing Then
            dbContext.Database.ExecuteSqlCommandAsync("DELETE FROM [APP_Attivita]")
        Else
            dbContext.APP_Attivita.Remove(attivita)
            dbContext.SaveChanges()
        End If

    End Sub

End Class