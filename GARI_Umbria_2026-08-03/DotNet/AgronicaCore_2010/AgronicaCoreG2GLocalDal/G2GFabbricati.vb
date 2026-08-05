Imports System.Data.Entity
Imports System.Transactions
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModello

Public Class G2GFabbricati_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Nuovo_Fabbricati_G2G(ByVal From_PivaSuperUser As String, ByVal To_PivaSuperUser As String, ByVal From_Piva As String, ByVal To_Piva As String, ByVal To_Sa_Cod As Integer, ByVal To_Progressivo As Integer) As G2G_Fabbricati

        Dim g2g As New G2G_Fabbricati
        With g2g
            .From_PivaSuperUser = From_PivaSuperUser
            .To_PivaSuperUser = To_PivaSuperUser
            .From_Piva = From_Piva
            .To_Piva = To_Piva
            .To_Sa_Cod = To_Sa_Cod
            .To_Progressivo = To_Progressivo
            .FabbricatiToInsert = New List(Of G2G_Fabbricato)
            .FabbricatiToUpdate = New List(Of G2G_Fabbricato)
            .FabbricatiToDelete = New List(Of G2G_Fabbricato)
            .Recode = G2GUtility.Nuovo_G2G_Recode()
        End With

        Return g2g

    End Function

    Public Function Nuovo_Fabbricati_G2GReverse(ByVal From_PivaSuperUser As String, ByVal To_PivaSuperUser As String, ByVal From_Piva As String, ByVal To_Piva As String, ByVal To_Sa_Cod As Integer, ByVal To_Progressivo As Integer) As G2G_Fabbricati_Reverse

        Dim g2g As New G2G_Fabbricati_Reverse
        With g2g
            .From_PivaSuperUser = From_PivaSuperUser
            .To_PivaSuperUser = To_PivaSuperUser
            .From_Piva = From_Piva
            .To_Piva = To_Piva
            .To_Sa_Cod = To_Sa_Cod
            .To_Progressivo = To_Progressivo
            .FabbricatiToInsert = New List(Of G2G_Fabbricato)
            .FabbricatiToUpdate = New List(Of G2G_Fabbricato)
            .FabbricatiToDelete = New List(Of G2G_Fabbricato)
            .Recode = G2GUtility.Nuovo_G2G_Recode()
        End With

        Return g2g

    End Function

    Public Function Leggi_Fabbricati_G2G(ByVal Sa_Cod As Integer, ByRef fabbricati As List(Of AgronicaCoreModello.Anagrafe.Magazzino), ByRef g2g As G2G_Fabbricati, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GFabbricati_R.Leggi_Fabbricati_G2G()"
        Dim From_Piva = g2g.From_Piva
        Dim From_PivaSuperUser = g2g.From_PivaSuperUser
        Dim To_PivaSuperUser = g2g.To_PivaSuperUser

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
            GiasContext.Database.CommandTimeout = 3600
            Dim listaFabbricatiInsert = (
                From f In GiasContext.Fabbricati
                Where f.PIVA = From_Piva AndAlso (Sa_Cod = 0 OrElse f.SA_COD = Sa_Cod) AndAlso
                    Not GiasContext.G2G_Recode_Fabbricati.Any(Function(g) g.From_PivaSuperUser = From_PivaSuperUser AndAlso
                        g.To_PivaSuperUser = To_PivaSuperUser AndAlso g.FromPiva = f.PIVA AndAlso g.FromSa_cod = f.SA_COD AndAlso
                        g.From_FabbricatoCod = f.Fabbricato_Cod)
                Select f).ToList()

            For Each fabbricato In listaFabbricatiInsert
                Dim filtroFabbricato As Boolean = (From i In fabbricati Where i.Piva = fabbricato.PIVA And i.Sa_Cod = fabbricato.SA_COD And i.Fabbricato_Cod = fabbricato.Fabbricato_Cod).ToList.Count > 0
                If fabbricati.Count = 0 OrElse filtroFabbricato Then
                    Dim codici = (From c In GiasContext.Fabbricati_Codici Where c.PIVA = fabbricato.PIVA AndAlso c.sa_cod = fabbricato.SA_COD AndAlso c.Fabbricato_cod = fabbricato.Fabbricato_Cod Select c).ToList()
                    Dim indirizzo = (From i In GiasContext.Indirizzi Where i.cod_indirizzo = fabbricato.Indirizzo_Cod Select i).FirstOrDefault()
                    g2g.FabbricatiToInsert.Add(New G2G_Fabbricato With {.Fabbricato = fabbricato, .Codici = codici, .Indirizzo = indirizzo})
                End If
            Next

            Dim listaFabbricatiUpdate = (
                From f In GiasContext.Fabbricati
                Where f.PIVA = From_Piva AndAlso (Sa_Cod = 0 OrElse f.SA_COD = Sa_Cod) AndAlso
                    GiasContext.G2G_Recode_Fabbricati.Any(Function(g) g.From_PivaSuperUser = From_PivaSuperUser AndAlso
                        g.To_PivaSuperUser = To_PivaSuperUser AndAlso g.FromPiva = f.PIVA AndAlso g.FromSa_cod = f.SA_COD AndAlso
                        g.From_FabbricatoCod = f.Fabbricato_Cod AndAlso g.datainvio < f.Data_Modifica)
                Select f).ToList()

            For Each fabbricato In listaFabbricatiUpdate
                Dim codici = (From c In GiasContext.Fabbricati_Codici Where c.PIVA = fabbricato.PIVA AndAlso c.sa_cod = fabbricato.SA_COD AndAlso c.Fabbricato_cod = fabbricato.Fabbricato_Cod Select c).ToList()
                Dim indirizzo = (From i In GiasContext.Indirizzi Where i.cod_indirizzo = fabbricato.Indirizzo_Cod Select i).FirstOrDefault()
                g2g.FabbricatiToUpdate.Add(New G2G_Fabbricato With {.Fabbricato = fabbricato, .Codici = codici, .Indirizzo = indirizzo})
            Next

            g2g.Recode.G2GRecodeFabbricatiToDelete = (
                From g In GiasContext.G2G_Recode_Fabbricati
                Where g.From_PivaSuperUser = From_PivaSuperUser AndAlso g.To_PivaSuperUser = To_PivaSuperUser _
                     AndAlso g.FromPiva = From_Piva AndAlso (Sa_Cod = 0 OrElse g.FromSa_cod = Sa_Cod) AndAlso Not GiasContext.Fabbricati.Any(Function(f) f.PIVA = g.FromPiva AndAlso f.SA_COD = g.FromSa_cod AndAlso f.Fabbricato_Cod = g.From_FabbricatoCod)
                Select g).ToList()

        End Using

        Return g2g.FabbricatiToInsert.Count > 0 OrElse g2g.FabbricatiToUpdate.Count > 0 OrElse g2g.Recode.G2GRecodeFabbricatiToDelete.Count > 0

    End Function

    Public Function Leggi_Fabbricati_G2GReverse(ByVal Sa_Cod As Integer, ByRef g2g As G2G_Fabbricati_Reverse, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GFabbricati_R.Leggi_Fabbricati_G2GReverse()"
        Dim From_Piva = g2g.From_Piva
        Dim From_PivaSuperUser = g2g.From_PivaSuperUser
        Dim To_PivaSuperUser = g2g.To_PivaSuperUser

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
            GiasContext.Database.CommandTimeout = 3600
            Dim listaFabbricatiInsert = (
                From f In GiasContext.Fabbricati
                Where f.PIVA = From_Piva AndAlso (Sa_Cod = 0 OrElse f.SA_COD = Sa_Cod) AndAlso
                    Not GiasContext.G2G_Recode_Fabbricati.Any(Function(g) g.To_PivaSuperUser = From_PivaSuperUser AndAlso
                        g.From_PivaSuperUser = To_PivaSuperUser AndAlso g.FromPiva = f.PIVA AndAlso g.ToSa_cod = f.SA_COD AndAlso
                        g.To_FabbricatoCod = f.Fabbricato_Cod)
                Select f).ToList()

            For Each fabbricato In listaFabbricatiInsert
                Dim codici = (From c In GiasContext.Fabbricati_Codici Where c.PIVA = fabbricato.PIVA AndAlso c.sa_cod = fabbricato.SA_COD AndAlso c.Fabbricato_cod = fabbricato.Fabbricato_Cod Select c).ToList()
                Dim indirizzo = (From i In GiasContext.Indirizzi Where i.cod_indirizzo = fabbricato.Indirizzo_Cod Select i).FirstOrDefault()
                g2g.FabbricatiToInsert.Add(New G2G_Fabbricato With {.Fabbricato = fabbricato, .Codici = codici, .Indirizzo = indirizzo})
            Next

            Dim listaFabbricatiUpdate = (
                From f In GiasContext.Fabbricati
                Where f.PIVA = From_Piva AndAlso (Sa_Cod = 0 OrElse f.SA_COD = Sa_Cod) AndAlso
                    GiasContext.G2G_Recode_Fabbricati.Any(Function(g) g.To_PivaSuperUser = From_PivaSuperUser AndAlso
                        g.From_PivaSuperUser = To_PivaSuperUser AndAlso g.FromPiva = f.PIVA AndAlso g.ToSa_cod = f.SA_COD AndAlso
                        g.To_FabbricatoCod = f.Fabbricato_Cod AndAlso g.datainvio < f.Data_Modifica)
                Select f).ToList()

            For Each fabbricato In listaFabbricatiUpdate
                Dim codici = (From c In GiasContext.Fabbricati_Codici Where c.PIVA = fabbricato.PIVA AndAlso c.sa_cod = fabbricato.SA_COD AndAlso c.Fabbricato_cod = fabbricato.Fabbricato_Cod Select c).ToList()
                Dim indirizzo = (From i In GiasContext.Indirizzi Where i.cod_indirizzo = fabbricato.Indirizzo_Cod Select i).FirstOrDefault()
                g2g.FabbricatiToUpdate.Add(New G2G_Fabbricato With {.Fabbricato = fabbricato, .Codici = codici, .Indirizzo = indirizzo})
            Next

            g2g.Recode.G2GRecodeFabbricatiToDelete = (
                From g In GiasContext.G2G_Recode_Fabbricati
                Where g.To_PivaSuperUser = From_PivaSuperUser AndAlso g.From_PivaSuperUser = To_PivaSuperUser _
                     AndAlso g.FromPiva = From_Piva AndAlso (Sa_Cod = 0 OrElse g.FromSa_cod = Sa_Cod) AndAlso Not GiasContext.Fabbricati.Any(Function(f) f.PIVA = g.FromPiva AndAlso f.SA_COD = g.FromSa_cod AndAlso f.Fabbricato_Cod = g.From_FabbricatoCod)
                Select g).ToList()

        End Using

        Dim PivaSuperUser_Appoggio = ""
        PivaSuperUser_Appoggio = g2g.From_PivaSuperUser
        g2g.From_PivaSuperUser = g2g.To_PivaSuperUser
        g2g.To_PivaSuperUser = PivaSuperUser_Appoggio

        g2g.From_Sa_Cod = g2g.To_Sa_Cod
        g2g.To_Sa_Cod = Sa_Cod

        Return g2g.FabbricatiToInsert.Count > 0 OrElse g2g.FabbricatiToUpdate.Count > 0 OrElse g2g.Recode.G2GRecodeFabbricatiToDelete.Count > 0

    End Function

End Class

Public Class G2GFabbricati_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi_Fabbricati_G2G(ByRef g2g As G2G_Fabbricati, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GFabbricati_W.Scrivi_Fabbricati_G2G()"
        Dim messaggioErrore As String = ""

        If G2GUtility.transactionScope Then

            Dim transactionOptions As New TransactionOptions()
            transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            transactionOptions.Timeout = G2GUtility.GetTransactionTimeout(objParametri)
            Dim g2gRecode As G2G_Recode = g2g.Recode

            Try

                Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)
                    g2gRecode = Scrivi_Fabbricati(g2g, objParametri)
                    scope.Complete()
                End Using

            Catch ex As Exception

                messaggioErrore = ex.Message
                g2gRecode.MessaggioErrore = messaggioErrore
                Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)

            End Try

            Return g2gRecode

        End If

        Return Scrivi_Fabbricati(g2g, objParametri)

    End Function

    Public Function Scrivi_Fabbricati_G2GReverse(ByRef g2g As G2G_Fabbricati_Reverse, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GFabbricati_W.Scrivi_Fabbricati_G2G()"
        Dim messaggioErrore As String = ""

        If G2GUtility.transactionScope Then

            Dim transactionOptions As New TransactionOptions()
            transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            transactionOptions.Timeout = G2GUtility.GetTransactionTimeout(objParametri)
            Dim g2gRecode As G2G_Recode = g2g.Recode

            Try

                Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)
                    g2gRecode = Scrivi_FabbricatiReverse(g2g, objParametri)
                    scope.Complete()
                End Using

            Catch ex As Exception

                messaggioErrore = ex.Message
                g2gRecode.MessaggioErrore = messaggioErrore
                Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)

            End Try

            Return g2gRecode

        End If

        Return Scrivi_FabbricatiReverse(g2g, objParametri)

    End Function

    Public Function Scrivi_Fabbricati(ByRef g2g As G2G_Fabbricati, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GFabbricati_W.Scrivi_Fabbricati()"
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

                ' FABBRICATI DA INSERIRE
                If g2g.FabbricatiToInsert.Count > 0 Then

                    Dim index As Integer = 0

                    For Each f As G2G_Fabbricato In g2g.FabbricatiToInsert

                        ' nuovo indirizzo
                        Dim idSeq = G2GUtility.NuovoId_Tabella(GiasContext, "Indirizzi", 0, 2000000000, objParametri)
                        Dim indirizzo = Gias_EF_Utility.CopyEntity(GiasContext, f.Indirizzo, Nothing, username, data)
                        indirizzo.cod_indirizzo = idSeq
                        GiasContext.Indirizzi.Add(indirizzo)

                        ' nuovo recode indirizzo
                        Dim recode_indirizzo =
                            New G2G_Recode_Indirizzi With {
                                .From_PivaSuperUser = From_PivaSuperUser,
                                .To_PivaSuperUser = To_PivaSuperUser,
                                .From_Cod_Indirizzo = f.Indirizzo.cod_indirizzo,
                                .To_Cod_Indirizzo = indirizzo.cod_indirizzo,
                                .Username_Creazione = username,
                                .Username_Modifica = username,
                                .Data_Creazione = Now(),
                                .Data_Modifica = Now(),
                                .Validita_Inizio = AGRODATAINIZIO,
                                .Validita_Fine = AGRODATAFINE,
                                .inviato = 0,
                                .datainvio = Now()
                            }
                        g2g.Recode.G2GRecodeIndirizziToInsert.Add(recode_indirizzo)
                        GiasContext.G2G_Recode_Indirizzi.Add(recode_indirizzo)

                        ' nuovo fabbricato
                        Dim idFabbricato = objSequenze.NuovoId_SeqMagazzino(To_Piva, To_Sa_Cod, BaseCode, TopCode, objParametri)
                        Dim fabbricato = Gias_EF_Utility.CopyEntity(GiasContext, f.Fabbricato, Nothing, username, data)
                        fabbricato.PIVA = To_Piva
                        fabbricato.SA_COD = To_Sa_Cod
                        fabbricato.Fabbricato_Cod = idFabbricato
                        fabbricato.Indirizzo_Cod = indirizzo.cod_indirizzo
                        GiasContext.Fabbricati.Add(fabbricato)

                        ' inserisce codici fabbricato
                        For Each fc As Fabbricati_Codici In f.Codici
                            Dim codice = Gias_EF_Utility.CopyEntity(GiasContext, fc, Nothing, username, data)
                            codice.PIVA = fabbricato.PIVA
                            codice.sa_cod = fabbricato.SA_COD
                            codice.Fabbricato_cod = fabbricato.Fabbricato_Cod
                            GiasContext.Fabbricati_Codici.Add(codice)
                        Next

                        'nuovo recode fabbricato
                        Dim recode =
                            New G2G_Recode_Fabbricati With {
                                .From_PivaSuperUser = From_PivaSuperUser,
                                .To_PivaSuperUser = To_PivaSuperUser,
                                .FromPiva = f.Fabbricato.PIVA,
                                .ToPiva = fabbricato.PIVA,
                                .FromSa_cod = f.Fabbricato.SA_COD,
                                .ToSa_cod = fabbricato.SA_COD,
                                .From_FabbricatoCod = f.Fabbricato.Fabbricato_Cod,
                                .To_FabbricatoCod = fabbricato.Fabbricato_Cod,
                                .Username_Creazione = username,
                                .Username_Modifica = username,
                                .Data_Creazione = Now(),
                                .Data_Modifica = Now(),
                                .Validita_Inizio = AGRODATAINIZIO,
                                .Validita_Fine = AGRODATAFINE,
                                .inviato = 0,
                                .datainvio = Now()
                            }
                        g2g.Recode.G2GRecodeFabbricatiToInsert.Add(recode)
                        GiasContext.G2G_Recode_Fabbricati.Add(recode)

                        G2GUtility.SaveChanges(GiasContext, index)

                    Next

                    GiasContext.SaveChanges()

                End If

                ' FABBRICATI DA MODIFICARE
                If g2g.FabbricatiToUpdate.Count > 0 Then

                    Dim index As Integer = 0

                    ' fabbricati da modificare
                    For Each f As G2G_Fabbricato In g2g.FabbricatiToUpdate

                        ' modifica recode fabbricato
                        Dim recode = (From rr In GiasContext.G2G_Recode_Fabbricati Where rr.From_PivaSuperUser = From_PivaSuperUser AndAlso rr.To_PivaSuperUser = To_PivaSuperUser AndAlso rr.FromPiva = f.Fabbricato.PIVA AndAlso rr.FromSa_cod = f.Fabbricato.SA_COD AndAlso rr.From_FabbricatoCod = f.Fabbricato.Fabbricato_Cod).FirstOrDefault()
                        If recode Is Nothing Then
                            Throw New Exception("Recode non presente in destinazione")
                        End If
                        recode.Username_Modifica = username
                        recode.Data_Modifica = data
                        recode.datainvio = data
                        g2g.Recode.G2GRecodeFabbricatiToUpdate.Add(recode)
                        GiasContext.G2G_Recode_Fabbricati.Attach(recode)
                        GiasContext.Entry(recode).State = EntityState.Modified

                        ' inserisco / modifico indirizzo fabbricato
                        Dim recode_indirizzo = (From rr In GiasContext.G2G_Recode_Indirizzi Where rr.From_PivaSuperUser = From_PivaSuperUser AndAlso rr.To_PivaSuperUser = To_PivaSuperUser AndAlso rr.From_Cod_Indirizzo = f.Indirizzo.cod_indirizzo).FirstOrDefault()
                        Dim indirizzo = If(recode_indirizzo Is Nothing, Nothing, (From iii In GiasContext.Indirizzi Where iii.cod_indirizzo = recode_indirizzo.To_Cod_Indirizzo Select iii).FirstOrDefault())

                        If indirizzo Is Nothing Then

                            ' nuovo indirizzo
                            Dim idSeq = G2GUtility.NuovoId_Tabella(GiasContext, "Indirizzi", 0, 2000000000, objParametri)
                            indirizzo = Gias_EF_Utility.CopyEntity(GiasContext, f.Indirizzo, Nothing, username, data)
                            indirizzo.cod_indirizzo = idSeq
                            GiasContext.Indirizzi.Add(indirizzo)

                            ' nuovo recode indirizzi
                            Dim ri =
                                New G2G_Recode_Indirizzi With {
                                    .From_PivaSuperUser = From_PivaSuperUser,
                                    .To_PivaSuperUser = To_PivaSuperUser,
                                    .From_Cod_Indirizzo = f.Indirizzo.cod_indirizzo,
                                    .To_Cod_Indirizzo = indirizzo.cod_indirizzo,
                                    .Username_Creazione = username,
                                    .Username_Modifica = username,
                                    .Data_Creazione = Now(),
                                    .Data_Modifica = Now(),
                                    .Validita_Inizio = AGRODATAINIZIO,
                                    .Validita_Fine = AGRODATAFINE,
                                    .inviato = 0,
                                    .datainvio = Now()
                                }
                            g2g.Recode.G2GRecodeIndirizziToInsert.Add(ri)
                            GiasContext.G2G_Recode_Indirizzi.Add(ri)

                        Else

                            ' modifica indirizzo
                            indirizzo = Gias_EF_Utility.CopyEntity(GiasContext, f.Indirizzo, indirizzo, username, data)
                            indirizzo.cod_indirizzo = recode_indirizzo.To_Cod_Indirizzo
                            GiasContext.Indirizzi.Attach(indirizzo)
                            GiasContext.Entry(indirizzo).State = EntityState.Modified

                            ' modifica recode indirizzo
                            recode_indirizzo.Username_Modifica = username
                            recode_indirizzo.Data_Modifica = data
                            recode_indirizzo.datainvio = data
                            g2g.Recode.G2GRecodeIndirizziToUpdate.Add(recode_indirizzo)
                            GiasContext.G2G_Recode_Indirizzi.Attach(recode_indirizzo)
                            GiasContext.Entry(recode_indirizzo).State = EntityState.Modified

                        End If


                        ' modifica fabbricato
                        Dim fabbricato = (From ff In GiasContext.Fabbricati Where ff.PIVA = recode.ToPiva AndAlso ff.SA_COD = recode.ToSa_cod AndAlso ff.Fabbricato_Cod = recode.To_FabbricatoCod).FirstOrDefault()
                        fabbricato = Gias_EF_Utility.CopyEntity(GiasContext, f.Fabbricato, fabbricato, username, data)
                        fabbricato.PIVA = recode.ToPiva
                        fabbricato.SA_COD = recode.ToSa_cod
                        fabbricato.Fabbricato_Cod = recode.To_FabbricatoCod
                        fabbricato.Indirizzo_Cod = indirizzo.cod_indirizzo
                        GiasContext.Fabbricati.Attach(fabbricato)
                        GiasContext.Entry(fabbricato).State = EntityState.Modified

                        ' cancella codici fabbricato
                        Dim codici = (From cc In GiasContext.Fabbricati_Codici Where cc.PIVA = fabbricato.PIVA AndAlso cc.sa_cod = fabbricato.SA_COD AndAlso cc.Fabbricato_cod = fabbricato.Fabbricato_Cod Select cc).ToList()
                        For Each cc As Fabbricati_Codici In codici
                            GiasContext.Fabbricati_Codici.Attach(cc)
                            GiasContext.Fabbricati_Codici.Remove(cc)
                        Next

                        ' inserisce codici fabbricato
                        For Each cc As Fabbricati_Codici In f.Codici
                            Dim codice = Gias_EF_Utility.CopyEntity(GiasContext, cc, Nothing, username, data)
                            codice.PIVA = fabbricato.PIVA
                            codice.sa_cod = fabbricato.SA_COD
                            codice.Fabbricato_cod = fabbricato.Fabbricato_Cod
                            GiasContext.Fabbricati_Codici.Add(codice)
                        Next

                        G2GUtility.SaveChanges(GiasContext, index)

                    Next

                    GiasContext.SaveChanges()

                End If

                ' FABBRICATI DA CANCELLARE
                If g2g.Recode.G2GRecodeFabbricatiToDelete.Count > 0 Then

                    Dim index As Integer = 0

                    For Each r As G2G_Recode_Fabbricati In g2g.Recode.G2GRecodeFabbricatiToDelete

                        Dim fabbricato = (From f In GiasContext.Fabbricati Where f.PIVA = r.ToPiva AndAlso f.SA_COD = r.ToSa_cod AndAlso f.Fabbricato_Cod = r.To_FabbricatoCod).FirstOrDefault()

                        'cancella fabbricato
                        If fabbricato IsNot Nothing Then

                            ' cancella codici fabbricato
                            Dim codici = (From cc In GiasContext.Fabbricati_Codici Where cc.PIVA = fabbricato.PIVA AndAlso cc.sa_cod = fabbricato.SA_COD AndAlso cc.Fabbricato_cod = fabbricato.Fabbricato_Cod Select cc).ToList()
                            For Each cc As Fabbricati_Codici In codici
                                GiasContext.Fabbricati_Codici.Attach(cc)
                                GiasContext.Fabbricati_Codici.Remove(cc)
                            Next

                            'cancella indirizzo fabbricato
                            Dim indirizzo = (From i In GiasContext.Indirizzi Where i.cod_indirizzo = fabbricato.Indirizzo_Cod Select i).FirstOrDefault()
                            If indirizzo IsNot Nothing Then
                                GiasContext.Indirizzi.Attach(indirizzo)
                                GiasContext.Indirizzi.Remove(indirizzo)
                            End If

                            'cancella fabbricato
                            GiasContext.Fabbricati.Attach(fabbricato)
                            GiasContext.Fabbricati.Remove(fabbricato)

                        End If

                        ' cancella recode fabbricato
                        GiasContext.G2G_Recode_Fabbricati.Attach(r)
                        GiasContext.G2G_Recode_Fabbricati.Remove(r)

                        G2GUtility.SaveChanges(GiasContext, index)

                    Next

                    GiasContext.SaveChanges()

                End If

                ' log importazione
                g2g.Recode.LogRecode = g2g.FabbricatiToInsert.Count & " nuovi, " & g2g.FabbricatiToUpdate.Count & " modificati, " & g2g.Recode.G2GRecodeFabbricatiToDelete.Count & " cancellati"

            End Using

        Catch ex As Exception

            messaggioErrore = ex.Message
            g2g.Recode.MessaggioErrore = messaggioErrore
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)

        End Try

        Return g2g.Recode

    End Function

    Public Function Scrivi_FabbricatiReverse(ByRef g2g As G2G_Fabbricati_Reverse, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GFabbricati_W.Scrivi_Fabbricati()"
        Dim messaggioErrore As String = ""

        Dim From_PivaSuperUser = g2g.From_PivaSuperUser
        Dim To_PivaSuperUser = g2g.To_PivaSuperUser
        Dim From_Piva = g2g.From_Piva
        Dim To_Piva = g2g.To_Piva
        Dim To_Sa_Cod = g2g.From_Sa_Cod

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

                ' FABBRICATI DA INSERIRE
                If g2g.FabbricatiToInsert.Count > 0 Then

                    Dim index As Integer = 0

                    For Each f As G2G_Fabbricato In g2g.FabbricatiToInsert

                        ' nuovo indirizzo
                        Dim idSeq = G2GUtility.NuovoId_Tabella(GiasContext, "Indirizzi", 0, 2000000000, objParametri)
                        Dim indirizzo = Gias_EF_Utility.CopyEntity(GiasContext, f.Indirizzo, Nothing, username, data)
                        indirizzo.cod_indirizzo = idSeq
                        GiasContext.Indirizzi.Add(indirizzo)

                        ' nuovo recode indirizzo
                        Dim recode_indirizzo =
                            New G2G_Recode_Indirizzi With {
                                .From_PivaSuperUser = From_PivaSuperUser,
                                .To_PivaSuperUser = To_PivaSuperUser,
                                .From_Cod_Indirizzo = indirizzo.cod_indirizzo,
                                .To_Cod_Indirizzo = f.Indirizzo.cod_indirizzo,
                                .Username_Creazione = username,
                                .Username_Modifica = username,
                                .Data_Creazione = Now(),
                                .Data_Modifica = Now(),
                                .Validita_Inizio = AGRODATAINIZIO,
                                .Validita_Fine = AGRODATAFINE,
                                .inviato = 0,
                                .datainvio = Now()
                            }
                        g2g.Recode.G2GRecodeIndirizziToInsert.Add(recode_indirizzo)
                        GiasContext.G2G_Recode_Indirizzi.Add(recode_indirizzo)

                        ' nuovo fabbricato
                        Dim idFabbricato = objSequenze.NuovoId_SeqMagazzino(To_Piva, To_Sa_Cod, BaseCode, TopCode, objParametri)
                        Dim fabbricato = Gias_EF_Utility.CopyEntity(GiasContext, f.Fabbricato, Nothing, username, data)
                        fabbricato.PIVA = To_Piva
                        fabbricato.SA_COD = To_Sa_Cod
                        fabbricato.Fabbricato_Cod = idFabbricato
                        fabbricato.Indirizzo_Cod = indirizzo.cod_indirizzo
                        GiasContext.Fabbricati.Add(fabbricato)

                        ' inserisce codici fabbricato
                        For Each fc As Fabbricati_Codici In f.Codici
                            Dim codice = Gias_EF_Utility.CopyEntity(GiasContext, fc, Nothing, username, data)
                            codice.PIVA = fabbricato.PIVA
                            codice.sa_cod = fabbricato.SA_COD
                            codice.Fabbricato_cod = fabbricato.Fabbricato_Cod
                            GiasContext.Fabbricati_Codici.Add(codice)
                        Next

                        'nuovo recode fabbricato
                        Dim recode =
                            New G2G_Recode_Fabbricati With {
                                .From_PivaSuperUser = From_PivaSuperUser,
                                .To_PivaSuperUser = To_PivaSuperUser,
                                .FromPiva = f.Fabbricato.PIVA,
                                .ToPiva = fabbricato.PIVA,
                                .FromSa_cod = fabbricato.SA_COD,
                                .ToSa_cod = g2g.To_Sa_Cod,
                                .From_FabbricatoCod = fabbricato.Fabbricato_Cod,
                                .To_FabbricatoCod = f.Fabbricato.Fabbricato_Cod,
                                .Username_Creazione = username,
                                .Username_Modifica = username,
                                .Data_Creazione = Now(),
                                .Data_Modifica = Now(),
                                .Validita_Inizio = AGRODATAINIZIO,
                                .Validita_Fine = AGRODATAFINE,
                                .inviato = 0,
                                .datainvio = Now()
                            }
                        g2g.Recode.G2GRecodeFabbricatiToInsert.Add(recode)
                        GiasContext.G2G_Recode_Fabbricati.Add(recode)

                        G2GUtility.SaveChanges(GiasContext, index)

                    Next

                    GiasContext.SaveChanges()

                End If

                ' FABBRICATI DA MODIFICARE
                If g2g.FabbricatiToUpdate.Count > 0 Then

                    Dim index As Integer = 0

                    ' fabbricati da modificare
                    For Each f As G2G_Fabbricato In g2g.FabbricatiToUpdate

                        ' modifica recode fabbricato
                        Dim recode = (From rr In GiasContext.G2G_Recode_Fabbricati Where rr.From_PivaSuperUser = From_PivaSuperUser AndAlso rr.To_PivaSuperUser = To_PivaSuperUser AndAlso rr.ToPiva = f.Fabbricato.PIVA AndAlso rr.ToSa_cod = f.Fabbricato.SA_COD AndAlso rr.To_FabbricatoCod = f.Fabbricato.Fabbricato_Cod).FirstOrDefault()
                        If recode Is Nothing Then
                            Throw New Exception("Recode non presente in destinazione")
                        End If
                        recode.Username_Modifica = username
                        recode.Data_Modifica = data
                        recode.datainvio = data
                        g2g.Recode.G2GRecodeFabbricatiToUpdate.Add(recode)
                        GiasContext.G2G_Recode_Fabbricati.Attach(recode)
                        GiasContext.Entry(recode).State = EntityState.Modified

                        ' inserisco / modifico indirizzo fabbricato
                        Dim recode_indirizzo = (From rr In GiasContext.G2G_Recode_Indirizzi Where rr.From_PivaSuperUser = From_PivaSuperUser AndAlso rr.To_PivaSuperUser = To_PivaSuperUser AndAlso rr.To_Cod_Indirizzo = f.Indirizzo.cod_indirizzo).FirstOrDefault()
                        Dim indirizzo = If(recode_indirizzo Is Nothing, Nothing, (From iii In GiasContext.Indirizzi Where iii.cod_indirizzo = recode_indirizzo.From_Cod_Indirizzo Select iii).FirstOrDefault())

                        If indirizzo Is Nothing Then

                            ' nuovo indirizzo
                            Dim idSeq = G2GUtility.NuovoId_Tabella(GiasContext, "Indirizzi", 0, 2000000000, objParametri)
                            indirizzo = Gias_EF_Utility.CopyEntity(GiasContext, f.Indirizzo, Nothing, username, data)
                            indirizzo.cod_indirizzo = idSeq
                            GiasContext.Indirizzi.Add(indirizzo)

                            ' nuovo recode indirizzi
                            Dim ri =
                                New G2G_Recode_Indirizzi With {
                                    .From_PivaSuperUser = From_PivaSuperUser,
                                    .To_PivaSuperUser = To_PivaSuperUser,
                                    .From_Cod_Indirizzo = indirizzo.cod_indirizzo,
                                    .To_Cod_Indirizzo = f.Indirizzo.cod_indirizzo,
                                    .Username_Creazione = username,
                                    .Username_Modifica = username,
                                    .Data_Creazione = Now(),
                                    .Data_Modifica = Now(),
                                    .Validita_Inizio = AGRODATAINIZIO,
                                    .Validita_Fine = AGRODATAFINE,
                                    .inviato = 0,
                                    .datainvio = Now()
                                }
                            g2g.Recode.G2GRecodeIndirizziToInsert.Add(ri)
                            GiasContext.G2G_Recode_Indirizzi.Add(ri)

                        Else

                            ' modifica indirizzo
                            indirizzo = Gias_EF_Utility.CopyEntity(GiasContext, f.Indirizzo, indirizzo, username, data)
                            indirizzo.cod_indirizzo = recode_indirizzo.From_Cod_Indirizzo
                            GiasContext.Indirizzi.Attach(indirizzo)
                            GiasContext.Entry(indirizzo).State = EntityState.Modified

                            ' modifica recode indirizzo
                            recode_indirizzo.Username_Modifica = username
                            recode_indirizzo.Data_Modifica = data
                            recode_indirizzo.datainvio = data
                            g2g.Recode.G2GRecodeIndirizziToUpdate.Add(recode_indirizzo)
                            GiasContext.G2G_Recode_Indirizzi.Attach(recode_indirizzo)
                            GiasContext.Entry(recode_indirizzo).State = EntityState.Modified

                        End If


                        ' modifica fabbricato
                        Dim fabbricato = (From ff In GiasContext.Fabbricati Where ff.PIVA = recode.FromPiva AndAlso ff.SA_COD = recode.FromSa_cod AndAlso ff.Fabbricato_Cod = recode.From_FabbricatoCod).FirstOrDefault()
                        fabbricato = Gias_EF_Utility.CopyEntity(GiasContext, f.Fabbricato, fabbricato, username, data)
                        fabbricato.PIVA = recode.FromPiva
                        fabbricato.SA_COD = recode.FromSa_cod
                        fabbricato.Fabbricato_Cod = recode.From_FabbricatoCod
                        fabbricato.Indirizzo_Cod = indirizzo.cod_indirizzo
                        GiasContext.Fabbricati.Attach(fabbricato)
                        GiasContext.Entry(fabbricato).State = EntityState.Modified

                        ' cancella codici fabbricato
                        Dim codici = (From cc In GiasContext.Fabbricati_Codici Where cc.PIVA = fabbricato.PIVA AndAlso cc.sa_cod = fabbricato.SA_COD AndAlso cc.Fabbricato_cod = fabbricato.Fabbricato_Cod Select cc).ToList()
                        For Each cc As Fabbricati_Codici In codici
                            GiasContext.Fabbricati_Codici.Attach(cc)
                            GiasContext.Fabbricati_Codici.Remove(cc)
                        Next

                        ' inserisce codici fabbricato
                        For Each cc As Fabbricati_Codici In f.Codici
                            Dim codice = Gias_EF_Utility.CopyEntity(GiasContext, cc, Nothing, username, data)
                            codice.PIVA = fabbricato.PIVA
                            codice.sa_cod = fabbricato.SA_COD
                            codice.Fabbricato_cod = fabbricato.Fabbricato_Cod
                            GiasContext.Fabbricati_Codici.Add(codice)
                        Next

                        G2GUtility.SaveChanges(GiasContext, index)

                    Next

                    GiasContext.SaveChanges()

                End If

                ' FABBRICATI DA CANCELLARE
                If g2g.Recode.G2GRecodeFabbricatiToDelete.Count > 0 Then

                    Dim index As Integer = 0

                    For Each r As G2G_Recode_Fabbricati In g2g.Recode.G2GRecodeFabbricatiToDelete

                        Dim fabbricato = (From f In GiasContext.Fabbricati Where f.PIVA = r.FromPiva AndAlso f.SA_COD = r.FromSa_cod AndAlso f.Fabbricato_Cod = r.From_FabbricatoCod).FirstOrDefault()

                        'cancella fabbricato
                        If fabbricato IsNot Nothing Then

                            ' cancella codici fabbricato
                            Dim codici = (From cc In GiasContext.Fabbricati_Codici Where cc.PIVA = fabbricato.PIVA AndAlso cc.sa_cod = fabbricato.SA_COD AndAlso cc.Fabbricato_cod = fabbricato.Fabbricato_Cod Select cc).ToList()
                            For Each cc As Fabbricati_Codici In codici
                                GiasContext.Fabbricati_Codici.Attach(cc)
                                GiasContext.Fabbricati_Codici.Remove(cc)
                            Next

                            'cancella indirizzo fabbricato
                            Dim indirizzo = (From i In GiasContext.Indirizzi Where i.cod_indirizzo = fabbricato.Indirizzo_Cod Select i).FirstOrDefault()
                            If indirizzo IsNot Nothing Then
                                GiasContext.Indirizzi.Attach(indirizzo)
                                GiasContext.Indirizzi.Remove(indirizzo)
                            End If

                            'cancella fabbricato
                            GiasContext.Fabbricati.Attach(fabbricato)
                            GiasContext.Fabbricati.Remove(fabbricato)

                        End If

                        ' cancella recode fabbricato
                        GiasContext.G2G_Recode_Fabbricati.Attach(r)
                        GiasContext.G2G_Recode_Fabbricati.Remove(r)

                        G2GUtility.SaveChanges(GiasContext, index)

                    Next

                    GiasContext.SaveChanges()

                End If

                ' log importazione
                g2g.Recode.LogRecode = g2g.FabbricatiToInsert.Count & " nuovi, " & g2g.FabbricatiToUpdate.Count & " modificati, " & g2g.Recode.G2GRecodeFabbricatiToDelete.Count & " cancellati"

            End Using

        Catch ex As Exception

            messaggioErrore = ex.Message
            g2g.Recode.MessaggioErrore = messaggioErrore
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)

        End Try

        Return g2g.Recode

    End Function

End Class
