Imports System.Transactions
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModello
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports System.Data.Entity
Imports System.Text
Imports Newtonsoft.Json

Public Class G2GCampi_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Nuovo_Campi_G2G(ByVal From_PivaSuperUser As String, ByVal To_PivaSuperUser As String, ByVal From_Piva As String, ByVal To_Piva As String, ByVal To_Sa_Cod As Integer, ByVal To_Progressivo As Integer) As G2G_Campi

        Dim g2g As New G2G_Campi
        With g2g
            .From_PivaSuperUser = From_PivaSuperUser
            .To_PivaSuperUser = To_PivaSuperUser
            .From_Piva = From_Piva
            .To_Piva = To_Piva
            .To_Sa_Cod = To_Sa_Cod
            .To_Progressivo = To_Progressivo
            .CampiToInsert = New List(Of G2G_Campo)
            .CampiToUpdate = New List(Of G2G_Campo)
            .CampiToDelete = New List(Of G2G_Campo)
            .Recode = G2GUtility.Nuovo_G2G_Recode()
        End With

        Return g2g

    End Function

    Public Function Nuovo_Campi_G2GReverse(ByVal From_PivaSuperUser As String, ByVal To_PivaSuperUser As String, ByVal From_Piva As String, ByVal To_Piva As String, ByVal To_Sa_Cod As Integer, ByVal To_Progressivo As Integer) As G2G_Campi_Reverse

        Dim g2g As New G2G_Campi_Reverse
        With g2g
            .From_PivaSuperUser = From_PivaSuperUser
            .To_PivaSuperUser = To_PivaSuperUser
            .From_Piva = From_Piva
            .To_Piva = To_Piva
            .To_Sa_Cod = To_Sa_Cod
            .To_Progressivo = To_Progressivo
            .CampiToInsert = New List(Of G2G_Campo)
            .CampiToUpdate = New List(Of G2G_Campo)
            .CampiToDelete = New List(Of G2G_Campo)
            .Recode = G2GUtility.Nuovo_G2G_Recode()
        End With

        Return g2g

    End Function

    Public Function Leggi_Campi_G2G(ByVal Sa_Cod As Integer, ByRef g2g As G2G_Campi, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, ByRef Log_Import As StringBuilder) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GCampi_R.Leggi_Campi_G2G()"
        Dim From_Piva = g2g.From_Piva
        Dim From_PivaSuperUser = g2g.From_PivaSuperUser
        Dim To_PivaSuperUser = g2g.To_PivaSuperUser
        Dim From_Data = objParametri.FinestraTemporaleInizio
        Dim To_Data = objParametri.FinestraTemporaleFine

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
            GiasContext.Database.CommandTimeout = 3600
            Dim listaCampiInsert = (
                From c In GiasContext.Campi
                Where c.Piva = From_Piva AndAlso (Sa_Cod = 0 OrElse c.Sa_Cod = Sa_Cod) AndAlso
                    c.Validita_Inizio <= To_Data AndAlso c.Validita_Fine >= From_Data AndAlso
                    Not GiasContext.G2G_Recode_Campo.Any(Function(g) g.From_PivaSuperUser = From_PivaSuperUser AndAlso
                        g.To_PivaSuperUser = To_PivaSuperUser AndAlso g.From_Piva = c.Piva AndAlso
                        g.From_Sa_Cod = c.Sa_Cod AndAlso g.From_Campo_cod = c.Campo_Cod)
                Select c).ToList()

            G2GUtility.Log(Log_Import, "Trasferimento piano colturale: Campi --> listaCampiInsert " & listaCampiInsert.Count)

            For Each campo In listaCampiInsert
                Dim utente = (From u In GiasContext.UtentiXCampi Where u.USER = From_PivaSuperUser AndAlso u.PIVA = campo.Piva AndAlso u.SA_COD = campo.Sa_Cod AndAlso u.Campo_Cod = campo.Campo_Cod Select u).FirstOrDefault()
                Dim codici = (From c In GiasContext.Campi_Codici Where c.PIVA = campo.Piva AndAlso c.sa_cod = campo.Sa_Cod AndAlso c.campo_cod = campo.Campo_Cod Select c).ToList()
                g2g.CampiToInsert.Add(New G2G_Campo With {.Campo = campo, .Utente = utente, .Codici = codici})
            Next

            Dim listaCampiUpdate = (
                From c In GiasContext.Campi
                Where c.Piva = From_Piva AndAlso (Sa_Cod = 0 OrElse c.Sa_Cod = Sa_Cod) AndAlso
                    c.Validita_Inizio <= To_Data AndAlso c.Validita_Fine >= From_Data AndAlso
                    GiasContext.G2G_Recode_Campo.Any(Function(g) g.From_PivaSuperUser = From_PivaSuperUser AndAlso
                        g.To_PivaSuperUser = To_PivaSuperUser AndAlso g.From_Piva = c.Piva AndAlso
                        g.From_Sa_Cod = c.Sa_Cod AndAlso g.From_Campo_cod = c.Campo_Cod AndAlso g.datainvio < c.Data_Modifica)
                Select c).ToList()

            G2GUtility.Log(Log_Import, "Trasferimento piano colturale: Campi --> listaCampiUpdate " & listaCampiUpdate.Count)

            For Each campo In listaCampiUpdate
                Dim utente = (From u In GiasContext.UtentiXCampi Where u.USER = From_PivaSuperUser AndAlso u.PIVA = campo.Piva AndAlso u.SA_COD = campo.Sa_Cod AndAlso u.Campo_Cod = campo.Campo_Cod Select u).FirstOrDefault()
                Dim codici = (From c In GiasContext.Campi_Codici Where c.PIVA = campo.Piva AndAlso c.sa_cod = campo.Sa_Cod AndAlso c.campo_cod = campo.Campo_Cod Select c).ToList()
                g2g.CampiToUpdate.Add(New G2G_Campo With {.Campo = campo, .Utente = utente, .Codici = codici})
            Next

            g2g.Recode.G2GRecodeCampiToDelete = (
                From g In GiasContext.G2G_Recode_Campo
                Where g.From_PivaSuperUser = From_PivaSuperUser AndAlso g.To_PivaSuperUser = To_PivaSuperUser AndAlso g.From_Piva = From_Piva AndAlso (Sa_Cod = 0 OrElse g.From_Sa_Cod = Sa_Cod) AndAlso
                      Not GiasContext.Campi.Any(Function(c) c.Piva = g.From_Piva AndAlso c.Sa_Cod = g.From_Sa_Cod AndAlso c.Campo_Cod = g.From_Campo_cod)
                Select g).ToList()

            G2GUtility.Log(Log_Import, "Trasferimento piano colturale: Campi --> G2GRecodeCampiToDelete " & g2g.Recode.G2GRecodeCampiToDelete.Count)

        End Using

        Return g2g.CampiToInsert.Count > 0 OrElse g2g.CampiToUpdate.Count > 0 OrElse g2g.Recode.G2GRecodeCampiToDelete.Count > 0

    End Function

    Public Function Leggi_Campi_G2GReverse(ByVal Sa_Cod As Integer, ByRef g2g As G2G_Campi_Reverse, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GCampi_R.Leggi_Campi_G2G()"
        Dim From_Piva = g2g.From_Piva
        Dim From_PivaSuperUser = g2g.From_PivaSuperUser
        Dim To_PivaSuperUser = g2g.To_PivaSuperUser
        Dim From_Data = objParametri.FinestraTemporaleInizio
        Dim To_Data = objParametri.FinestraTemporaleFine

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
            GiasContext.Database.CommandTimeout = 3600
            Dim listaCampiInsert = (
                From c In GiasContext.Campi
                Where c.Piva = From_Piva AndAlso (Sa_Cod = 0 OrElse c.Sa_Cod = Sa_Cod) AndAlso
                    c.Validita_Inizio <= To_Data AndAlso c.Validita_Fine >= From_Data AndAlso
                    Not GiasContext.G2G_Recode_Campo.Any(Function(g) g.To_PivaSuperUser = From_PivaSuperUser AndAlso
                        g.From_PivaSuperUser = To_PivaSuperUser AndAlso g.From_Piva = c.Piva AndAlso
                        g.To_Sa_Cod = c.Sa_Cod AndAlso g.To_Campo_cod = c.Campo_Cod)
                Select c).ToList()

            For Each campo In listaCampiInsert
                Dim utente = (From u In GiasContext.UtentiXCampi Where u.USER = From_PivaSuperUser AndAlso u.PIVA = campo.Piva AndAlso u.SA_COD = campo.Sa_Cod AndAlso u.Campo_Cod = campo.Campo_Cod Select u).FirstOrDefault()
                Dim codici = (From c In GiasContext.Campi_Codici Where c.PIVA = campo.Piva AndAlso c.sa_cod = campo.Sa_Cod AndAlso c.campo_cod = campo.Campo_Cod Select c).ToList()
                g2g.CampiToInsert.Add(New G2G_Campo With {.Campo = campo, .Utente = utente, .Codici = codici})
            Next

            Dim listaCampiUpdate = (
                From c In GiasContext.Campi
                Where c.Piva = From_Piva AndAlso (Sa_Cod = 0 OrElse c.Sa_Cod = Sa_Cod) AndAlso
                    c.Validita_Inizio <= To_Data AndAlso c.Validita_Fine >= From_Data AndAlso
                    GiasContext.G2G_Recode_Campo.Any(Function(g) g.To_PivaSuperUser = From_PivaSuperUser AndAlso
                        g.From_PivaSuperUser = To_PivaSuperUser AndAlso g.From_Piva = c.Piva AndAlso
                        g.To_Sa_Cod = c.Sa_Cod AndAlso g.To_Campo_cod = c.Campo_Cod AndAlso g.datainvio < c.Data_Modifica)
                Select c).ToList()

            For Each campo In listaCampiUpdate
                Dim utente = (From u In GiasContext.UtentiXCampi Where u.USER = From_PivaSuperUser AndAlso u.PIVA = campo.Piva AndAlso u.SA_COD = campo.Sa_Cod AndAlso u.Campo_Cod = campo.Campo_Cod Select u).FirstOrDefault()
                Dim codici = (From c In GiasContext.Campi_Codici Where c.PIVA = campo.Piva AndAlso c.sa_cod = campo.Sa_Cod AndAlso c.campo_cod = campo.Campo_Cod Select c).ToList()
                g2g.CampiToUpdate.Add(New G2G_Campo With {.Campo = campo, .Utente = utente, .Codici = codici})
            Next

            g2g.Recode.G2GRecodeCampiToDelete = (
                From g In GiasContext.G2G_Recode_Campo
                Where g.From_PivaSuperUser = To_PivaSuperUser AndAlso g.To_PivaSuperUser = From_PivaSuperUser AndAlso g.From_Piva = From_Piva AndAlso (Sa_Cod = 0 OrElse g.To_Sa_Cod = Sa_Cod) AndAlso
                      Not GiasContext.Campi.Any(Function(c) c.Piva = g.From_Piva AndAlso c.Sa_Cod = g.To_Sa_Cod AndAlso c.Campo_Cod = g.To_Campo_cod)
                Select g).ToList()

        End Using

        Dim PivaSuperUser_Appoggio = ""
        PivaSuperUser_Appoggio = g2g.From_PivaSuperUser
        g2g.From_PivaSuperUser = g2g.To_PivaSuperUser
        g2g.To_PivaSuperUser = PivaSuperUser_Appoggio

        Return g2g.CampiToInsert.Count > 0 OrElse g2g.CampiToUpdate.Count > 0 OrElse g2g.Recode.G2GRecodeCampiToDelete.Count > 0

    End Function

End Class

Public Class G2GCampi_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi_Campi_G2G(ByRef g2g As G2G_Campi, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GCampi_W.Scrivi_Campi_G2G()"
        Dim messaggioErrore As String = ""

        If G2GUtility.transactionScope Then

            Dim transactionOptions As New TransactionOptions()
            transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            transactionOptions.Timeout = G2GUtility.GetTransactionTimeout(objParametri)
            Dim g2gRecode As G2G_Recode = g2g.Recode

            Try

                Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)
                    g2gRecode = Scrivi_Campi(g2g, objParametri)
                    scope.Complete()
                End Using

            Catch ex As Exception

                messaggioErrore = ex.Message
                g2gRecode.MessaggioErrore = messaggioErrore
                Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)

            End Try

            Return g2gRecode

        End If

        Return Scrivi_Campi(g2g, objParametri)

    End Function

    Public Function Scrivi_Campi_G2GReverse(ByRef g2g As G2G_Campi_Reverse, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GCampi_W.Scrivi_Campi_G2G_Reverse()"
        Dim messaggioErrore As String = ""

        If G2GUtility.transactionScope Then

            Dim transactionOptions As New TransactionOptions()
            transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            transactionOptions.Timeout = G2GUtility.GetTransactionTimeout(objParametri)
            Dim g2gRecode As G2G_Recode = g2g.Recode

            Try

                Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)
                    g2gRecode = Scrivi_Campi_Reverse(g2g, objParametri)
                    scope.Complete()
                End Using

            Catch ex As Exception

                messaggioErrore = ex.Message
                g2gRecode.MessaggioErrore = messaggioErrore
                Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)

            End Try

            Return g2gRecode

        End If

        Return Scrivi_Campi_Reverse(g2g, objParametri)

    End Function

    Public Function Scrivi_Campi(ByRef g2g As G2G_Campi, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GCampi_W.Scrivi_Campi()"
        Dim messaggioErrore As String = ""

        Dim From_PivaSuperUser = g2g.From_PivaSuperUser
        Dim To_PivaSuperUser = g2g.To_PivaSuperUser
        Dim From_Piva = g2g.From_Piva
        Dim To_Piva = g2g.To_Piva
        Dim To_Sa_Cod = g2g.To_Sa_Cod

        Dim username = objParametri.UsernameOperazione
        Dim data = Date.Now

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Dim BaseCode As Long = 0
        Dim TopCode As Long = 2000000000
        Dim objSequenze As New AgronicaCoreDataProvider.Agro_Sequenze
        objSequenze.Calcola_BaseCode(g2g.To_Progressivo, TopCode, BaseCode, objParametri)

        Try

            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                ' CAMPI DA INSERIRE
                If g2g.CampiToInsert.Count > 0 Then

                    Dim index As Integer = 0

                    Dim listG2G_Recode_Campi_GiaInseriti As List(Of AgronicaCoreEntityFramework_POCO.G2G_Recode_Campo)

                    Dim firstFromPiva = g2g.CampiToInsert(0).Campo.Piva
                    listG2G_Recode_Campi_GiaInseriti = (From rr In GiasContext.G2G_Recode_Campo
                                                        Where rr.From_PivaSuperUser = From_PivaSuperUser AndAlso
                                                               rr.To_PivaSuperUser = To_PivaSuperUser AndAlso
                                                               rr.From_Piva = firstFromPiva).ToList()

                    For Each c As G2G_Campo In g2g.CampiToInsert

                        Dim recodeEsistenti = (From r In listG2G_Recode_Campi_GiaInseriti Where r.From_Piva = c.Campo.Piva And
                                                                                              r.From_Sa_Cod = c.Campo.Sa_Cod AndAlso
                                                                                              r.From_Campo_cod = c.Campo.Campo_Cod).ToList

                        If recodeEsistenti.Count = 0 Then
                            ' nuovo campo
                            Dim idCampo = objSequenze.NuovoId_Campi(To_Piva, To_Sa_Cod, BaseCode, TopCode, objParametri)
                            Dim campo = Gias_EF_Utility.CopyEntity(GiasContext, c.Campo, Nothing, username, data)
                            campo.Piva = To_Piva
                            campo.Sa_Cod = To_Sa_Cod
                            campo.Campo_Cod = idCampo
                            GiasContext.Campi.Add(campo)

                            ' inserisce utente campo
                            Dim utente = Gias_EF_Utility.CopyEntity(GiasContext, c.Utente, Nothing, username, data)
                            utente.USER = To_PivaSuperUser
                            utente.PIVA = To_Piva
                            utente.SA_COD = To_Sa_Cod
                            utente.Campo_Cod = campo.Campo_Cod
                            GiasContext.UtentiXCampi.Add(utente)

                            ' inserisce codici campo
                            For Each cc As Campi_Codici In c.Codici
                                Dim codice = Gias_EF_Utility.CopyEntity(GiasContext, cc, Nothing, username, data)
                                codice.PIVA = campo.Piva
                                codice.sa_cod = campo.Sa_Cod
                                codice.campo_cod = campo.Campo_Cod
                                GiasContext.Campi_Codici.Add(codice)
                            Next

                            'nuovo recode campo
                            Dim recode =
                                New G2G_Recode_Campo With {
                                    .From_PivaSuperUser = From_PivaSuperUser,
                                    .To_PivaSuperUser = To_PivaSuperUser,
                                    .From_Piva = c.Campo.Piva,
                                    .To_Piva = campo.Piva,
                                    .From_Sa_Cod = c.Campo.Sa_Cod,
                                    .To_Sa_Cod = campo.Sa_Cod,
                                    .From_Campo_cod = c.Campo.Campo_Cod,
                                    .To_Campo_cod = campo.Campo_Cod,
                                    .Username_Creazione = username,
                                    .Username_Modifica = username,
                                    .Data_Creazione = Now(),
                                    .Data_Modifica = Now(),
                                    .Validita_Inizio = AGRODATAINIZIO,
                                    .Validita_Fine = AGRODATAFINE,
                                    .inviato = 0,
                                    .datainvio = Now()
                                }
                            g2g.Recode.G2GRecodeCampiToInsert.Add(recode)
                            GiasContext.G2G_Recode_Campo.Add(recode)
                        Else
                            g2g.Recode.G2GRecodeCampiToInsert.Add(recodeEsistenti(0))
                            Dim messaggio = "Campo non importato: " & JsonConvert.SerializeObject(c.Campo) & vbCrLf &
                                "Recode già presente"
                            Scrivi_LOG(objParametri, nomeRoutine, messaggio)
                        End If

                        G2GUtility.SaveChanges(GiasContext, index)

                    Next

                    GiasContext.SaveChanges()

                End If

                ' CAMPI DA MODIFICARE
                If g2g.CampiToUpdate.Count > 0 Then

                    Dim index As Integer = 0

                    ' campi da modificare
                    For Each c As G2G_Campo In g2g.CampiToUpdate

                        ' modifica recode campo
                        Dim recode = (From rr In GiasContext.G2G_Recode_Campo Where rr.From_PivaSuperUser = From_PivaSuperUser AndAlso rr.To_PivaSuperUser = To_PivaSuperUser AndAlso rr.From_Piva = c.Campo.Piva AndAlso rr.From_Sa_Cod = c.Campo.Sa_Cod AndAlso rr.From_Campo_cod = c.Campo.Campo_Cod).FirstOrDefault()
                        If recode Is Nothing Then
                            Throw New Exception("Recode non presente in destinazione")
                        End If
                        recode.Username_Modifica = username
                        recode.Data_Modifica = data
                        recode.datainvio = data
                        g2g.Recode.G2GRecodeCampiToUpdate.Add(recode)
                        GiasContext.G2G_Recode_Campo.Attach(recode)
                        GiasContext.Entry(recode).State = EntityState.Modified

                        ' modifica campo
                        Dim campo = (From cc In GiasContext.Campi Where cc.Piva = recode.To_Piva AndAlso cc.Sa_Cod = recode.To_Sa_Cod AndAlso cc.Campo_Cod = recode.To_Campo_cod).FirstOrDefault()
                        campo = Gias_EF_Utility.CopyEntity(GiasContext, c.Campo, campo, username, data)
                        campo.Piva = recode.To_Piva
                        campo.Sa_Cod = recode.To_Sa_Cod
                        campo.Campo_Cod = recode.To_Campo_cod
                        GiasContext.Campi.Attach(campo)
                        GiasContext.Entry(campo).State = EntityState.Modified

                        ' cancella codici campo
                        Dim codici = (From cc In GiasContext.Campi_Codici Where cc.PIVA = campo.Piva AndAlso cc.sa_cod = campo.Sa_Cod AndAlso cc.campo_cod = campo.Campo_Cod Select cc).ToList()
                        For Each cc As Campi_Codici In codici
                            GiasContext.Campi_Codici.Attach(cc)
                            GiasContext.Campi_Codici.Remove(cc)
                        Next

                        ' inserisce codici campo
                        For Each cc As Campi_Codici In c.Codici
                            Dim codice = Gias_EF_Utility.CopyEntity(GiasContext, cc, Nothing, username, data)
                            codice.PIVA = campo.Piva
                            codice.sa_cod = campo.Sa_Cod
                            codice.campo_cod = campo.Campo_Cod
                            GiasContext.Campi_Codici.Add(codice)
                        Next

                        G2GUtility.SaveChanges(GiasContext, index)

                    Next

                    GiasContext.SaveChanges()

                End If

                ' CAMPI DA CANCELLARE
                If g2g.Recode.G2GRecodeCampiToDelete.Count > 0 Then

                    Dim index As Integer = 0

                    For Each r As G2G_Recode_Campo In g2g.Recode.G2GRecodeCampiToDelete

                        Dim campo = (From c In GiasContext.Campi Where c.Piva = r.To_Piva AndAlso c.Sa_Cod = r.To_Sa_Cod AndAlso c.Campo_Cod = r.To_Campo_cod).FirstOrDefault()

                        'cancella campo
                        If campo IsNot Nothing Then

                            ' cancella codici campo
                            Dim codici = (From cc In GiasContext.Campi_Codici Where cc.PIVA = campo.Piva AndAlso cc.sa_cod = campo.Sa_Cod AndAlso cc.campo_cod = campo.Campo_Cod Select cc).ToList()
                            For Each cc As Campi_Codici In codici
                                Dim ccEntry = GiasContext.Entry(cc)
                                If ccEntry.State = EntityState.Detached Then
                                    GiasContext.Campi_Codici.Attach(cc)
                                End If
                                If ccEntry.State <> EntityState.Deleted Then
                                    GiasContext.Campi_Codici.Remove(cc)
                                End If
                            Next

                            'cancella campo
                            Dim campoEntry = GiasContext.Entry(campo)
                            If campoEntry.State = EntityState.Detached Then
                                GiasContext.Campi.Attach(campo)
                            End If
                            If campoEntry.State <> EntityState.Deleted Then
                                GiasContext.Campi.Remove(campo)
                            End If

                        End If

                        ' cancella recode campo - trova l'entità tracciata se esiste
                        Dim recodeToDelete As G2G_Recode_Campo = r
                        Dim trackedRecode = GiasContext.G2G_Recode_Campo.Local.FirstOrDefault(Function(x) _
                            x.From_PivaSuperUser = r.From_PivaSuperUser AndAlso
                            x.To_PivaSuperUser = r.To_PivaSuperUser AndAlso
                            x.From_Piva = r.From_Piva AndAlso
                            x.To_Piva = r.To_Piva AndAlso
                            x.From_Sa_Cod = r.From_Sa_Cod AndAlso
                            x.To_Sa_Cod = r.To_Sa_Cod AndAlso
                            x.From_Campo_cod = r.From_Campo_cod AndAlso
                            x.To_Campo_cod = r.To_Campo_cod)

                        If trackedRecode IsNot Nothing Then
                            ' Usa l'entità già tracciata
                            recodeToDelete = trackedRecode
                        Else
                            ' Attach solo se non è già tracciata
                            Dim recodeEntry = GiasContext.Entry(r)
                            If recodeEntry.State = EntityState.Detached Then
                                ' Verifica che il record esista ancora nel database prima dell'attach
                                Dim existsInDb = GiasContext.G2G_Recode_Campo.Any(Function(x) _
                                    x.From_PivaSuperUser = r.From_PivaSuperUser AndAlso
                                    x.To_PivaSuperUser = r.To_PivaSuperUser AndAlso
                                    x.From_Piva = r.From_Piva AndAlso
                                    x.To_Piva = r.To_Piva AndAlso
                                    x.From_Sa_Cod = r.From_Sa_Cod AndAlso
                                    x.To_Sa_Cod = r.To_Sa_Cod AndAlso
                                    x.From_Campo_cod = r.From_Campo_cod AndAlso
                                    x.To_Campo_cod = r.To_Campo_cod)

                                If existsInDb Then
                                    GiasContext.G2G_Recode_Campo.Attach(r)
                                Else
                                    ' Il record è già stato eliminato, salta questo elemento
                                    g2g.Recode.LogRecode &= objParametri.Recupera_NomeDB() & " - Recode Impianto già eliminato: " & r.From_PivaSuperUser & " -> " & r.To_PivaSuperUser & " - " & r.From_Piva & " -> " & r.To_Piva & " - " & r.From_Sa_Cod & " -> " & r.To_Sa_Cod & " - " & r.From_Campo_cod & " -> " & r.To_Campo_cod & Environment.NewLine
                                    Continue For
                                End If
                            End If
                        End If

                        Dim finalEntry = GiasContext.Entry(recodeToDelete)
                        If finalEntry.State <> EntityState.Deleted Then
                            GiasContext.G2G_Recode_Campo.Remove(recodeToDelete)
                        End If

                        G2GUtility.SaveChanges(GiasContext, index)

                    Next

                    GiasContext.SaveChanges()

                End If

                ' log importazione
                g2g.Recode.LogRecode &= " - campi: " & g2g.CampiToInsert.Count & " nuovi, " & g2g.CampiToUpdate.Count & " modificati, " & g2g.Recode.G2GRecodeCampiToDelete.Count & " cancellati"

            End Using

        Catch ex As Exception

            messaggioErrore = ex.ToString
            g2g.Recode.MessaggioErrore = messaggioErrore
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)

        End Try

        Return g2g.Recode

    End Function

    Public Function Scrivi_Campi_Reverse(ByRef g2g As G2G_Campi_Reverse, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GCampi_W.Scrivi_Campi()"
        Dim messaggioErrore As String = ""

        Dim From_PivaSuperUser = g2g.From_PivaSuperUser
        Dim To_PivaSuperUser = g2g.To_PivaSuperUser
        Dim From_Piva = g2g.From_Piva
        Dim To_Piva = g2g.To_Piva
        Dim To_Sa_Cod = g2g.To_Sa_Cod

        Dim username = objParametri.UsernameOperazione
        Dim data = Date.Now

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Dim BaseCode As Long = 0
        Dim TopCode As Long = 2000000000
        Dim objSequenze As New AgronicaCoreDataProvider.Agro_Sequenze
        objSequenze.Calcola_BaseCode(g2g.To_Progressivo, TopCode, BaseCode, objParametri)

        Try

            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                ' CAMPI DA INSERIRE
                If g2g.CampiToInsert.Count > 0 Then

                    Dim index As Integer = 0

                    For Each c As G2G_Campo In g2g.CampiToInsert

                        ' nuovo campo
                        Dim idCampo = objSequenze.NuovoId_Campi(To_Piva, To_Sa_Cod, BaseCode, TopCode, objParametri)
                        Dim campo = Gias_EF_Utility.CopyEntity(GiasContext, c.Campo, Nothing, username, data)
                        campo.Piva = To_Piva
                        campo.Sa_Cod = To_Sa_Cod
                        campo.Campo_Cod = idCampo
                        GiasContext.Campi.Add(campo)

                        ' inserisce utente campo
                        Dim utente = Gias_EF_Utility.CopyEntity(GiasContext, c.Utente, Nothing, username, data)
                        utente.USER = From_PivaSuperUser
                        utente.PIVA = To_Piva
                        utente.SA_COD = To_Sa_Cod
                        utente.Campo_Cod = campo.Campo_Cod
                        GiasContext.UtentiXCampi.Add(utente)

                        ' inserisce codici campo
                        For Each cc As Campi_Codici In c.Codici
                            Dim codice = Gias_EF_Utility.CopyEntity(GiasContext, cc, Nothing, username, data)
                            codice.PIVA = campo.Piva
                            codice.sa_cod = campo.Sa_Cod
                            codice.campo_cod = campo.Campo_Cod
                            GiasContext.Campi_Codici.Add(codice)
                        Next

                        'nuovo recode campo
                        Dim recode =
                            New G2G_Recode_Campo With {
                                .From_PivaSuperUser = From_PivaSuperUser,
                                .To_PivaSuperUser = To_PivaSuperUser,
                                .From_Piva = c.Campo.Piva,
                                .To_Piva = campo.Piva,
                                .From_Sa_Cod = campo.Sa_Cod,
                                .To_Sa_Cod = c.Campo.Sa_Cod,
                                .From_Campo_cod = campo.Campo_Cod,
                                .To_Campo_cod = c.Campo.Campo_Cod,
                                .Username_Creazione = username,
                                .Username_Modifica = username,
                                .Data_Creazione = Now(),
                                .Data_Modifica = Now(),
                                .Validita_Inizio = AGRODATAINIZIO,
                                .Validita_Fine = AGRODATAFINE,
                                .inviato = 0,
                                .datainvio = Now()
                            }
                        g2g.Recode.G2GRecodeCampiToInsert.Add(recode)
                        GiasContext.G2G_Recode_Campo.Add(recode)

                        G2GUtility.SaveChanges(GiasContext, index)

                    Next

                    GiasContext.SaveChanges()

                End If

                ' CAMPI DA MODIFICARE
                If g2g.CampiToUpdate.Count > 0 Then

                    Dim index As Integer = 0

                    ' campi da modificare
                    For Each c As G2G_Campo In g2g.CampiToUpdate

                        ' modifica recode campo
                        Dim recode = (From rr In GiasContext.G2G_Recode_Campo Where rr.From_PivaSuperUser = From_PivaSuperUser AndAlso rr.To_PivaSuperUser = To_PivaSuperUser AndAlso rr.From_Piva = c.Campo.Piva AndAlso rr.To_Sa_Cod = c.Campo.Sa_Cod AndAlso rr.To_Campo_cod = c.Campo.Campo_Cod).FirstOrDefault()
                        If recode Is Nothing Then
                            Throw New Exception("Recode non presente in destinazione")
                        End If
                        recode.Username_Modifica = username
                        recode.Data_Modifica = data
                        recode.datainvio = data
                        g2g.Recode.G2GRecodeCampiToUpdate.Add(recode)
                        GiasContext.G2G_Recode_Campo.Attach(recode)
                        GiasContext.Entry(recode).State = EntityState.Modified

                        ' modifica campo
                        Dim campo = (From cc In GiasContext.Campi Where cc.Piva = recode.From_Piva AndAlso cc.Sa_Cod = recode.From_Sa_Cod AndAlso cc.Campo_Cod = recode.From_Campo_cod).FirstOrDefault()
                        campo = Gias_EF_Utility.CopyEntity(GiasContext, c.Campo, campo, username, data)
                        campo.Piva = recode.From_Piva
                        campo.Sa_Cod = recode.From_Sa_Cod
                        campo.Campo_Cod = recode.From_Campo_cod
                        GiasContext.Campi.Attach(campo)
                        GiasContext.Entry(campo).State = EntityState.Modified

                        ' cancella codici campo
                        Dim codici = (From cc In GiasContext.Campi_Codici Where cc.PIVA = campo.Piva AndAlso cc.sa_cod = campo.Sa_Cod AndAlso cc.campo_cod = campo.Campo_Cod Select cc).ToList()
                        For Each cc As Campi_Codici In codici
                            GiasContext.Campi_Codici.Attach(cc)
                            GiasContext.Campi_Codici.Remove(cc)
                        Next

                        ' inserisce codici campo
                        For Each cc As Campi_Codici In c.Codici
                            Dim codice = Gias_EF_Utility.CopyEntity(GiasContext, cc, Nothing, username, data)
                            codice.PIVA = campo.Piva
                            codice.sa_cod = campo.Sa_Cod
                            codice.campo_cod = campo.Campo_Cod
                            GiasContext.Campi_Codici.Add(codice)
                        Next

                        G2GUtility.SaveChanges(GiasContext, index)

                    Next

                    GiasContext.SaveChanges()

                End If

                ' CAMPI DA CANCELLARE
                If g2g.Recode.G2GRecodeCampiToDelete.Count > 0 Then

                    Dim index As Integer = 0

                    For Each r As G2G_Recode_Campo In g2g.Recode.G2GRecodeCampiToDelete

                        Dim campo = (From c In GiasContext.Campi Where c.Piva = r.To_Piva AndAlso c.Sa_Cod = r.To_Sa_Cod AndAlso c.Campo_Cod = r.To_Campo_cod).FirstOrDefault()

                        'cancella campo
                        If campo IsNot Nothing Then

                            ' cancella codici campo
                            Dim codici = (From cc In GiasContext.Campi_Codici Where cc.PIVA = campo.Piva AndAlso cc.sa_cod = campo.Sa_Cod AndAlso cc.campo_cod = campo.Campo_Cod Select cc).ToList()
                            For Each cc As Campi_Codici In codici
                                GiasContext.Campi_Codici.Attach(cc)
                                GiasContext.Campi_Codici.Remove(cc)
                            Next

                            'cancella campo
                            GiasContext.Campi.Attach(campo)
                            GiasContext.Campi.Remove(campo)

                        End If

                        ' cancella recode campo
                        GiasContext.G2G_Recode_Campo.Attach(r)
                        GiasContext.G2G_Recode_Campo.Remove(r)

                        G2GUtility.SaveChanges(GiasContext, index)

                    Next

                    GiasContext.SaveChanges()

                End If

                ' log importazione
                g2g.Recode.LogRecode &= " - campi: " & g2g.CampiToInsert.Count & " nuovi, " & g2g.CampiToUpdate.Count & " modificati, " & g2g.Recode.G2GRecodeCampiToDelete.Count & " cancellati"

            End Using

        Catch ex As Exception

            messaggioErrore = ex.Message
            g2g.Recode.MessaggioErrore = messaggioErrore
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)

        End Try

        Return g2g.Recode

    End Function

End Class