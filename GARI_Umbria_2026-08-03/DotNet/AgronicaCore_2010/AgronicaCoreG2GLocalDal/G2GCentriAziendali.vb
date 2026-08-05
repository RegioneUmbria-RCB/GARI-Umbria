Imports System.Data.Entity
Imports System.Transactions
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModello

Public Class G2GCentriAziendali_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Nuovo_CentriAziendali_G2G(ByVal From_PivaSuperUser As String, ByVal To_PivaSuperUser As String, ByVal From_Piva As String, ByVal To_Piva As String, ByVal To_Progressivo As Integer) As G2G_CentriAziendali

        Dim g2g As New G2G_CentriAziendali
        With g2g
            .From_PivaSuperUser = From_PivaSuperUser
            .To_PivaSuperUser = To_PivaSuperUser
            .From_Piva = From_Piva
            .To_Piva = To_Piva
            .To_Progressivo = To_Progressivo
            .CentriAziendaliToInsert = New List(Of G2G_CentroAziendale)
            .CentriAziendaliToUpdate = New List(Of G2G_CentroAziendale)
            .CentriAziendaliToDelete = New List(Of G2G_CentroAziendale)
            .Recode = G2GUtility.Nuovo_G2G_Recode()
        End With

        Return g2g

    End Function

    Public Function Nuovo_CentriAziendali_G2GReverse(ByVal From_PivaSuperUser As String, ByVal To_PivaSuperUser As String, ByVal From_Piva As String, ByVal To_Piva As String, ByVal To_Progressivo As Integer) As G2G_CentriAziendali_Reverse

        Dim g2g As New G2G_CentriAziendali_Reverse
        With g2g
            .From_PivaSuperUser = From_PivaSuperUser
            .To_PivaSuperUser = To_PivaSuperUser
            .From_Piva = From_Piva
            .To_Piva = To_Piva
            .To_Progressivo = To_Progressivo
            .CentriAziendaliToInsert = New List(Of G2G_CentroAziendale)
            .CentriAziendaliToUpdate = New List(Of G2G_CentroAziendale)
            .CentriAziendaliToDelete = New List(Of G2G_CentroAziendale)
            .Recode = G2GUtility.Nuovo_G2G_Recode()
        End With

        Return g2g

    End Function

    Public Function Leggi_CentriAziendali_G2G(ByVal Sa_Cod As Integer, ByRef g2g As G2G_CentriAziendali, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GCentriAziendali_R.Leggi_CentriAziendali_G2G()"
        Dim From_Piva = g2g.From_Piva
        Dim From_PivaSuperUser = g2g.From_PivaSuperUser
        Dim To_PivaSuperUser = g2g.To_PivaSuperUser

        Dim objFabbricati As New G2GFabbricati_R
        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
            GiasContext.Database.CommandTimeout = 3600
            Dim listaCentriAziendaliInsert = (
                From c In GiasContext.Centri_Aziendali
                Where c.PIVA = From_Piva AndAlso (Sa_Cod = 0 OrElse c.sa_cod = Sa_Cod) AndAlso
                    Not GiasContext.G2G_Recode_Imprese.Any(Function(g) g.From_PivaSuperUser = From_PivaSuperUser AndAlso
                        g.To_PivaSuperUser = To_PivaSuperUser AndAlso g.FROM_Piva = c.PIVA AndAlso
                        g.FROM_SaCod = c.sa_cod AndAlso g.FROM_SaCod <> 0)
                Select c).ToList()

            For Each centro In listaCentriAziendaliInsert
                Dim utente = (From u In GiasContext.UtentiXStrutture Where u.USER = From_PivaSuperUser AndAlso u.PIVA = centro.PIVA AndAlso u.SA_COD = centro.sa_cod Select u).FirstOrDefault()
                Dim codici = (From c In GiasContext.Centri_Aziendali_Codici Where c.PIVA = centro.PIVA AndAlso c.sa_cod = centro.sa_cod Select c).ToList()
                Dim indirizzi = (From ii In GiasContext.CentrixIndirizzi Join i In GiasContext.Indirizzi On ii.cod_indirizzo Equals i.cod_indirizzo Where ii.PIVA = centro.PIVA AndAlso ii.sa_cod = centro.sa_cod Select New G2G_Indirizzo With {.Indirizzo = i, .Tipo_Indirizzo = ii.Tipo_Indirizzo}).ToList()
                'Dim fabbricati = objFabbricati.Nuovo_Fabbricati_G2G(From_PivaSuperUser, To_PivaSuperUser, From_Piva, g2g.To_Piva, 0, g2g.To_Progressivo)
                'objFabbricati.Leggi_Fabbricati_G2G(Sa_Cod, fabbricati, objParametri) , .Fabbricati = fabbricati
                g2g.CentriAziendaliToInsert.Add(New G2G_CentroAziendale With {.Centro = centro, .Utente = utente, .Codici = codici, .Indirizzi = indirizzi})
            Next

            Dim listaCentriAziendaliUpdate = (
                From c In GiasContext.Centri_Aziendali
                Where c.PIVA = From_Piva AndAlso (Sa_Cod = 0 OrElse c.sa_cod = Sa_Cod) AndAlso
                    GiasContext.G2G_Recode_Imprese.Any(Function(g) g.From_PivaSuperUser = From_PivaSuperUser AndAlso
                        g.To_PivaSuperUser = To_PivaSuperUser AndAlso g.FROM_Piva = c.PIVA AndAlso
                        g.FROM_SaCod = c.sa_cod AndAlso g.FROM_SaCod <> 0 AndAlso g.datainvio < c.Data_Modifica)
                Select c).ToList()

            For Each centro In listaCentriAziendaliUpdate
                Dim utente = (From u In GiasContext.UtentiXStrutture Where u.USER = From_PivaSuperUser AndAlso u.PIVA = centro.PIVA AndAlso u.SA_COD = centro.sa_cod Select u).FirstOrDefault()
                Dim codici = (From c In GiasContext.Centri_Aziendali_Codici Where c.PIVA = centro.PIVA AndAlso c.sa_cod = centro.sa_cod Select c).ToList()
                Dim indirizzi = (From ii In GiasContext.CentrixIndirizzi Join i In GiasContext.Indirizzi On ii.cod_indirizzo Equals i.cod_indirizzo Where ii.PIVA = centro.PIVA AndAlso ii.sa_cod = centro.sa_cod Select New G2G_Indirizzo With {.Indirizzo = i, .Tipo_Indirizzo = ii.Tipo_Indirizzo}).ToList()
                'Dim fabbricati = objFabbricati.Nuovo_Fabbricati_G2G(From_PivaSuperUser, To_PivaSuperUser, From_Piva, g2g.To_Piva, 0, g2g.To_Progressivo)
                'objFabbricati.Leggi_Fabbricati_G2G(Sa_Cod, fabbricati, objParametri) , .Fabbricati = fabbricati
                g2g.CentriAziendaliToUpdate.Add(New G2G_CentroAziendale With {.Centro = centro, .Utente = utente, .Codici = codici, .Indirizzi = indirizzi})
            Next

            'g2g.Recode.G2GRecodeImpreseCentriToDelete = (
            '    From g In GiasContext.G2G_Recode_Imprese
            '    Where g.From_PivaSuperUser = From_PivaSuperUser AndAlso g.To_PivaSuperUser = To_PivaSuperUser _
            '         AndAlso g.FROM_Piva = From_Piva AndAlso (Sa_Cod = 0 OrElse c.sa_cod = Sa_Cod) AndAlso g.FROM_SaCod <> 0 AndAlso Not GiasContext.Imprese.Any(Function(i) i.PIVA = g.FROM_Piva)
            '    Select g).ToList()

        End Using

        Return g2g.CentriAziendaliToInsert.Count > 0 OrElse g2g.CentriAziendaliToUpdate.Count > 0

    End Function

    Public Function Leggi_CentriAziendali_G2GReverse(ByVal Sa_Cod As Integer, ByRef g2g As G2G_CentriAziendali_Reverse, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GCentriAziendali_R.Leggi_CentriAziendali_G2GReverse()"
        Dim From_Piva = g2g.From_Piva
        Dim From_PivaSuperUser = g2g.From_PivaSuperUser
        Dim To_PivaSuperUser = g2g.To_PivaSuperUser

        Dim objFabbricati As New G2GFabbricati_R
        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
            GiasContext.Database.CommandTimeout = 3600
            Dim listaCentriAziendaliInsert = (
                From c In GiasContext.Centri_Aziendali
                Where c.PIVA = From_Piva AndAlso (Sa_Cod = 0 OrElse c.sa_cod = Sa_Cod) AndAlso
                    Not GiasContext.G2G_Recode_Imprese.Any(Function(g) g.To_PivaSuperUser = From_PivaSuperUser AndAlso
                        g.From_PivaSuperUser = To_PivaSuperUser AndAlso g.FROM_Piva = c.PIVA AndAlso
                        g.TO_SaCod = c.sa_cod AndAlso g.TO_SaCod <> 0)
                Select c).ToList()

            For Each centro In listaCentriAziendaliInsert
                Dim utente = (From u In GiasContext.UtentiXStrutture Where u.USER = From_PivaSuperUser AndAlso u.PIVA = centro.PIVA AndAlso u.SA_COD = centro.sa_cod Select u).FirstOrDefault()
                Dim codici = (From c In GiasContext.Centri_Aziendali_Codici Where c.PIVA = centro.PIVA AndAlso c.sa_cod = centro.sa_cod Select c).ToList()
                Dim indirizzi = (From ii In GiasContext.CentrixIndirizzi Join i In GiasContext.Indirizzi On ii.cod_indirizzo Equals i.cod_indirizzo Where ii.PIVA = centro.PIVA AndAlso ii.sa_cod = centro.sa_cod Select New G2G_Indirizzo With {.Indirizzo = i, .Tipo_Indirizzo = ii.Tipo_Indirizzo}).ToList()
                'Dim fabbricati = objFabbricati.Nuovo_Fabbricati_G2G(From_PivaSuperUser, To_PivaSuperUser, From_Piva, g2g.To_Piva, 0, g2g.To_Progressivo)
                'objFabbricati.Leggi_Fabbricati_G2G(Sa_Cod, fabbricati, objParametri) , .Fabbricati = fabbricati
                g2g.CentriAziendaliToInsert.Add(New G2G_CentroAziendale With {.Centro = centro, .Utente = utente, .Codici = codici, .Indirizzi = indirizzi})
            Next

            Dim listaCentriAziendaliUpdate = (
                From c In GiasContext.Centri_Aziendali
                Where c.PIVA = From_Piva AndAlso (Sa_Cod = 0 OrElse c.sa_cod = Sa_Cod) AndAlso
                    GiasContext.G2G_Recode_Imprese.Any(Function(g) g.To_PivaSuperUser = From_PivaSuperUser AndAlso
                        g.From_PivaSuperUser = To_PivaSuperUser AndAlso g.FROM_Piva = c.PIVA AndAlso
                        g.TO_SaCod = c.sa_cod AndAlso g.TO_SaCod <> 0 AndAlso g.datainvio < c.Data_Modifica)
                Select c).ToList()

            For Each centro In listaCentriAziendaliUpdate
                Dim utente = (From u In GiasContext.UtentiXStrutture Where u.USER = From_PivaSuperUser AndAlso u.PIVA = centro.PIVA AndAlso u.SA_COD = centro.sa_cod Select u).FirstOrDefault()
                Dim codici = (From c In GiasContext.Centri_Aziendali_Codici Where c.PIVA = centro.PIVA AndAlso c.sa_cod = centro.sa_cod Select c).ToList()
                Dim indirizzi = (From ii In GiasContext.CentrixIndirizzi Join i In GiasContext.Indirizzi On ii.cod_indirizzo Equals i.cod_indirizzo Where ii.PIVA = centro.PIVA AndAlso ii.sa_cod = centro.sa_cod Select New G2G_Indirizzo With {.Indirizzo = i, .Tipo_Indirizzo = ii.Tipo_Indirizzo}).ToList()
                'Dim fabbricati = objFabbricati.Nuovo_Fabbricati_G2G(From_PivaSuperUser, To_PivaSuperUser, From_Piva, g2g.To_Piva, 0, g2g.To_Progressivo)
                'objFabbricati.Leggi_Fabbricati_G2G(Sa_Cod, fabbricati, objParametri) , .Fabbricati = fabbricati
                g2g.CentriAziendaliToUpdate.Add(New G2G_CentroAziendale With {.Centro = centro, .Utente = utente, .Codici = codici, .Indirizzi = indirizzi})
            Next

            'g2g.Recode.G2GRecodeImpreseCentriToDelete = (
            '    From g In GiasContext.G2G_Recode_Imprese
            '    Where g.From_PivaSuperUser = From_PivaSuperUser AndAlso g.To_PivaSuperUser = To_PivaSuperUser _
            '         AndAlso g.FROM_Piva = From_Piva AndAlso (Sa_Cod = 0 OrElse c.sa_cod = Sa_Cod) AndAlso g.FROM_SaCod <> 0 AndAlso Not GiasContext.Imprese.Any(Function(i) i.PIVA = g.FROM_Piva)
            '    Select g).ToList()

        End Using

        Dim PivaSuperUser_Appoggio = ""
        PivaSuperUser_Appoggio = g2g.From_PivaSuperUser
        g2g.From_PivaSuperUser = g2g.To_PivaSuperUser
        g2g.To_PivaSuperUser = PivaSuperUser_Appoggio

        Dim sacod_Appoggio = 0
        sacod_Appoggio = g2g.From_Sa_Cod
        g2g.From_Sa_Cod = g2g.To_Sa_Cod
        g2g.To_Sa_Cod = sacod_Appoggio

        Return g2g.CentriAziendaliToInsert.Count > 0 OrElse g2g.CentriAziendaliToUpdate.Count > 0

    End Function

End Class


Public Class G2GCentriAziendali_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi_CentriAziendali_G2G(ByRef g2g As G2G_CentriAziendali, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GCentriAziendali_W.Scrivi_CentriAziendali_G2G()"
        Dim messaggioErrore As String = ""

        If G2GUtility.transactionScope Then

            Dim transactionOptions As New TransactionOptions()
            transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            transactionOptions.Timeout = G2GUtility.GetTransactionTimeout(objParametri)
            Dim g2gRecode As G2G_Recode = g2g.Recode

            Try

                Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)
                    g2gRecode = Scrivi_CentriAziendali(g2g, objParametri)
                    scope.Complete()
                End Using

            Catch ex As Exception

                messaggioErrore = ex.Message
                g2gRecode.MessaggioErrore = messaggioErrore
                Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)

            End Try

            Return g2gRecode

        End If

        Return Scrivi_CentriAziendali(g2g, objParametri)

    End Function

    Public Function Scrivi_CentriAziendali_G2GReverse(ByRef g2g As G2G_CentriAziendali_Reverse, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GCentriAziendali_W.Scrivi_CentriAziendali_G2GReverse()"
        Dim messaggioErrore As String = ""

        If G2GUtility.transactionScope Then

            Dim transactionOptions As New TransactionOptions()
            transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            transactionOptions.Timeout = G2GUtility.GetTransactionTimeout(objParametri)
            Dim g2gRecode As G2G_Recode = g2g.Recode

            Try

                Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)
                    g2gRecode = Scrivi_CentriAziendaliReverse(g2g, objParametri)
                    scope.Complete()
                End Using

            Catch ex As Exception

                messaggioErrore = ex.Message
                g2gRecode.MessaggioErrore = messaggioErrore
                Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)

            End Try

            Return g2gRecode

        End If

        Return Scrivi_CentriAziendaliReverse(g2g, objParametri)

    End Function

    Public Function Scrivi_CentriAziendali(ByRef g2g As G2G_CentriAziendali, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GCentriAziendali_W.Scrivi_CentriAziendali()"
        Dim messaggioErrore As String = ""

        Dim From_PivaSuperUser = g2g.From_PivaSuperUser
        Dim To_PivaSuperUser = g2g.To_PivaSuperUser
        Dim From_Piva = g2g.From_Piva
        Dim To_Piva = g2g.To_Piva

        Dim username = objParametri.UsernameOperazione
        Dim data = Date.Now

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Dim BaseCode As Long = 0
        Dim TopCode As Long = 2000000000
        Dim objSequenze As New AgronicaCoreDataProvider.Agro_Sequenze
        objSequenze.Calcola_BaseCode(g2g.To_Progressivo, TopCode, BaseCode, objParametri)
        Dim objFabbricati As New G2GFabbricati_W

        Try

            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                ' CENTRI AZIENDALI DA INSERIRE
                If g2g.CentriAziendaliToInsert.Count > 0 Then

                    Dim index As Integer = 0

                    For Each c As G2G_CentroAziendale In g2g.CentriAziendaliToInsert

                        ' nuovo centro aziendale
                        Dim idCentro = objSequenze.NuovoId_CentriAziendali(To_Piva, BaseCode, TopCode, objParametri)
                        Dim centro = Gias_EF_Utility.CopyEntity(GiasContext, c.Centro, Nothing, username, data)
                        centro.PIVA = To_Piva
                        centro.sa_cod = idCentro
                        GiasContext.Centri_Aziendali.Add(centro)

                        ' inserisce utente struttura
                        Dim utente = Gias_EF_Utility.CopyEntity(GiasContext, c.Utente, Nothing, username, data)
                        utente.USER = To_PivaSuperUser
                        utente.PIVA = To_Piva
                        utente.SA_COD = centro.sa_cod
                        GiasContext.UtentiXStrutture.Add(utente)

                        ' inserisce codici centro
                        For Each cc As Centri_Aziendali_Codici In c.Codici
                            Dim codice = Gias_EF_Utility.CopyEntity(GiasContext, cc, Nothing, username, data)
                            codice.PIVA = centro.PIVA
                            codice.sa_cod = centro.sa_cod
                            GiasContext.Centri_Aziendali_Codici.Add(codice)
                        Next

                        ' inserisco indirizzi centro
                        For Each ii As G2G_Indirizzo In c.Indirizzi

                            ' nuovo indirizzo
                            Dim idSeq = G2GUtility.NuovoId_Tabella(GiasContext, "Indirizzi", 0, 2000000000, objParametri)
                            Dim indirizzo = Gias_EF_Utility.CopyEntity(GiasContext, ii.Indirizzo, Nothing, username, data)
                            indirizzo.cod_indirizzo = idSeq
                            GiasContext.Indirizzi.Add(indirizzo)

                            ' nuovo indirizzo centro
                            Dim cxi = New CentrixIndirizzi With {
                                .PIVA = centro.PIVA,
                                .sa_cod = centro.sa_cod,
                                .cod_indirizzo = indirizzo.cod_indirizzo,
                                .Tipo_Indirizzo = ii.Tipo_Indirizzo,
                                .inviato = 0,
                                .Data_Creazione = data,
                                .Data_Modifica = data,
                                .Validita_Inizio = AGRODATAINIZIO,
                                .Validita_Fine = AGRODATAFINE,
                                .Username_Creazione = username,
                                .Username_Modifica = username
                            }
                            GiasContext.CentrixIndirizzi.Add(cxi)

                            ' nuovo recode indirizzo
                            Dim recode_indirizzo =
                                New G2G_Recode_Indirizzi With {
                                    .From_PivaSuperUser = From_PivaSuperUser,
                                    .To_PivaSuperUser = To_PivaSuperUser,
                                    .From_Cod_Indirizzo = ii.Indirizzo.cod_indirizzo,
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

                        Next

                        'nuovo recode centro aziendale
                        Dim recode =
                            New G2G_Recode_Imprese With {
                                .From_PivaSuperUser = From_PivaSuperUser,
                                .To_PivaSuperUser = To_PivaSuperUser,
                                .FROM_Piva = c.Centro.PIVA,
                                .To_Piva = centro.PIVA,
                                .FROM_SaCod = c.Centro.sa_cod,
                                .TO_SaCod = centro.sa_cod,
                                .Username_Creazione = username,
                                .Username_Modifica = username,
                                .Data_Creazione = Now(),
                                .Data_Modifica = Now(),
                                .Validita_Inizio = AGRODATAINIZIO,
                                .Validita_Fine = AGRODATAFINE,
                                .inviato = 0,
                                .datainvio = Now()
                            }
                        g2g.Recode.G2GRecodeImpreseCentriToInsert.Add(recode)
                        GiasContext.G2G_Recode_Imprese.Add(recode)

                        G2GUtility.SaveChanges(GiasContext, index)

                        ' scrive fabbricati centro
                        'c.Fabbricati.To_Sa_Cod = centro.sa_cod
                        'objFabbricati.Scrivi_Fabbricati(c.Fabbricati, objParametri)
                        'G2GUtility.Copia_G2G_Recode(c.Fabbricati.Recode, g2g.Recode)

                    Next

                    GiasContext.SaveChanges()

                End If

                ' CENTRI AZIENDALI DA MODIFICARE
                If g2g.CentriAziendaliToUpdate.Count > 0 Then

                    Dim index As Integer = 0

                    ' centri da modificare
                    For Each c As G2G_CentroAziendale In g2g.CentriAziendaliToUpdate

                        ' modifica recode centro
                        Dim recode = (From rr In GiasContext.G2G_Recode_Imprese Where rr.From_PivaSuperUser = From_PivaSuperUser AndAlso rr.To_PivaSuperUser = To_PivaSuperUser AndAlso rr.FROM_Piva = c.Centro.PIVA AndAlso rr.FROM_SaCod = c.Centro.sa_cod).FirstOrDefault()
                        If recode Is Nothing Then
                            Throw New Exception("Recode non presente in destinazione")
                        End If
                        recode.Username_Modifica = username
                        recode.Data_Modifica = data
                        recode.datainvio = data
                        g2g.Recode.G2GRecodeImpreseCentriToUpdate.Add(recode)
                        GiasContext.G2G_Recode_Imprese.Attach(recode)
                        GiasContext.Entry(recode).State = EntityState.Modified

                        ' modifica centro
                        Dim centro = (From cc In GiasContext.Centri_Aziendali Where cc.PIVA = recode.To_Piva AndAlso cc.sa_cod = recode.TO_SaCod).FirstOrDefault()
                        centro = Gias_EF_Utility.CopyEntity(GiasContext, c.Centro, centro, username, data)
                        centro.PIVA = recode.To_Piva
                        centro.sa_cod = recode.TO_SaCod
                        GiasContext.Centri_Aziendali.Attach(centro)
                        GiasContext.Entry(centro).State = EntityState.Modified

                        ' cancella codici centro
                        Dim codici = (From cc In GiasContext.Centri_Aziendali_Codici Where cc.PIVA = centro.PIVA AndAlso cc.sa_cod = centro.sa_cod Select cc).ToList()
                        For Each cc As Centri_Aziendali_Codici In codici
                            GiasContext.Centri_Aziendali_Codici.Attach(cc)
                            GiasContext.Centri_Aziendali_Codici.Remove(cc)
                        Next

                        ' inserisce codici centro
                        For Each cc As Centri_Aziendali_Codici In c.Codici
                            Dim codice = Gias_EF_Utility.CopyEntity(GiasContext, cc, Nothing, username, data)
                            codice.PIVA = centro.PIVA
                            codice.sa_cod = centro.sa_cod
                            GiasContext.Centri_Aziendali_Codici.Add(codice)
                        Next

                        ' cancello indirizzi centro
                        Dim centrixindirizzi = (From ii In GiasContext.CentrixIndirizzi Where ii.PIVA = centro.PIVA AndAlso ii.sa_cod = centro.sa_cod Select ii).ToList()
                        For Each cxi As CentrixIndirizzi In centrixindirizzi
                            GiasContext.CentrixIndirizzi.Attach(cxi)
                            GiasContext.CentrixIndirizzi.Remove(cxi)
                        Next

                        ' inserisco / modifico indirizzi centro
                        For Each ii As G2G_Indirizzo In c.Indirizzi

                            Dim recode_indirizzo = (From rr In GiasContext.G2G_Recode_Indirizzi Where rr.From_PivaSuperUser = From_PivaSuperUser AndAlso rr.To_PivaSuperUser = To_PivaSuperUser AndAlso rr.From_Cod_Indirizzo = ii.Indirizzo.cod_indirizzo).FirstOrDefault()
                            Dim indirizzo = If(recode_indirizzo Is Nothing, Nothing, (From iii In GiasContext.Indirizzi Where iii.cod_indirizzo = recode_indirizzo.To_Cod_Indirizzo Select iii).FirstOrDefault())

                            If indirizzo Is Nothing Then

                                ' nuovo indirizzo
                                Dim idSeq = G2GUtility.NuovoId_Tabella(GiasContext, "Indirizzi", 0, 2000000000, objParametri)
                                indirizzo = Gias_EF_Utility.CopyEntity(GiasContext, ii.Indirizzo, Nothing, username, data)
                                indirizzo.cod_indirizzo = idSeq
                                GiasContext.Indirizzi.Add(indirizzo)

                                ' nuovo recode indirizzi
                                Dim ri =
                                    New G2G_Recode_Indirizzi With {
                                        .From_PivaSuperUser = From_PivaSuperUser,
                                        .To_PivaSuperUser = To_PivaSuperUser,
                                        .From_Cod_Indirizzo = ii.Indirizzo.cod_indirizzo,
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
                                indirizzo = Gias_EF_Utility.CopyEntity(GiasContext, ii.Indirizzo, indirizzo, username, data)
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

                            ' nuovo indirizzo centro
                            Dim cxi = New CentrixIndirizzi With {
                                .PIVA = centro.PIVA,
                                .sa_cod = centro.sa_cod,
                                .cod_indirizzo = indirizzo.cod_indirizzo,
                                .Tipo_Indirizzo = ii.Tipo_Indirizzo,
                                .inviato = 0,
                                .Data_Creazione = data,
                                .Data_Modifica = data,
                                .Validita_Inizio = AGRODATAINIZIO,
                                .Validita_Fine = AGRODATAFINE,
                                .Username_Creazione = username,
                                .Username_Modifica = username
                            }
                            GiasContext.CentrixIndirizzi.Add(cxi)

                        Next

                        G2GUtility.SaveChanges(GiasContext, index)

                        ' scrive fabbricati centro
                        'c.Fabbricati.To_Sa_Cod = centro.sa_cod
                        'objFabbricati.Scrivi_Fabbricati(c.Fabbricati, objParametri)
                        'G2GUtility.Copia_G2G_Recode(c.Fabbricati.Recode, g2g.Recode)

                    Next

                    GiasContext.SaveChanges()

                End If

                ' log importazione
                g2g.Recode.LogRecode = g2g.CentriAziendaliToInsert.Count & " nuovi, " & g2g.CentriAziendaliToUpdate.Count & " modificati, " & g2g.Recode.G2GRecodeImpreseCentriToDelete.Count & " cancellati"

            End Using

        Catch ex As Exception

            messaggioErrore = ex.Message
            g2g.Recode.MessaggioErrore = messaggioErrore
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)

        End Try

        Return g2g.Recode

    End Function

    Public Function Scrivi_CentriAziendaliReverse(ByRef g2g As G2G_CentriAziendali_Reverse, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GCentriAziendali_W.Scrivi_CentriAziendaliReverse()"
        Dim messaggioErrore As String = ""

        Dim From_PivaSuperUser = g2g.From_PivaSuperUser
        Dim To_PivaSuperUser = g2g.To_PivaSuperUser
        Dim From_Piva = g2g.From_Piva
        Dim To_Piva = g2g.To_Piva

        Dim username = objParametri.UsernameOperazione
        Dim data = Date.Now

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Dim BaseCode As Long = 0
        Dim TopCode As Long = 2000000000
        Dim objSequenze As New AgronicaCoreDataProvider.Agro_Sequenze
        objSequenze.Calcola_BaseCode(g2g.To_Progressivo, TopCode, BaseCode, objParametri)
        Dim objFabbricati As New G2GFabbricati_W

        Try

            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                ' CENTRI AZIENDALI DA INSERIRE
                If g2g.CentriAziendaliToInsert.Count > 0 Then

                    Dim index As Integer = 0

                    For Each c As G2G_CentroAziendale In g2g.CentriAziendaliToInsert

                        ' nuovo centro aziendale
                        Dim idCentro = objSequenze.NuovoId_CentriAziendali(To_Piva, BaseCode, TopCode, objParametri)
                        Dim centro = Gias_EF_Utility.CopyEntity(GiasContext, c.Centro, Nothing, username, data)
                        centro.PIVA = To_Piva
                        centro.sa_cod = idCentro
                        GiasContext.Centri_Aziendali.Add(centro)

                        ' inserisce utente struttura
                        Dim utente = Gias_EF_Utility.CopyEntity(GiasContext, c.Utente, Nothing, username, data)
                        utente.USER = From_PivaSuperUser
                        utente.PIVA = To_Piva
                        utente.SA_COD = centro.sa_cod
                        GiasContext.UtentiXStrutture.Add(utente)

                        ' inserisce codici centro
                        For Each cc As Centri_Aziendali_Codici In c.Codici
                            Dim codice = Gias_EF_Utility.CopyEntity(GiasContext, cc, Nothing, username, data)
                            codice.PIVA = centro.PIVA
                            codice.sa_cod = centro.sa_cod
                            GiasContext.Centri_Aziendali_Codici.Add(codice)
                        Next

                        ' inserisco indirizzi centro
                        For Each ii As G2G_Indirizzo In c.Indirizzi

                            ' nuovo indirizzo
                            Dim idSeq = G2GUtility.NuovoId_Tabella(GiasContext, "Indirizzi", 0, 2000000000, objParametri)
                            Dim indirizzo = Gias_EF_Utility.CopyEntity(GiasContext, ii.Indirizzo, Nothing, username, data)
                            indirizzo.cod_indirizzo = idSeq
                            GiasContext.Indirizzi.Add(indirizzo)

                            ' nuovo indirizzo centro
                            Dim cxi = New CentrixIndirizzi With {
                                .PIVA = centro.PIVA,
                                .sa_cod = centro.sa_cod,
                                .cod_indirizzo = indirizzo.cod_indirizzo,
                                .Tipo_Indirizzo = ii.Tipo_Indirizzo,
                                .inviato = 0,
                                .Data_Creazione = data,
                                .Data_Modifica = data,
                                .Validita_Inizio = AGRODATAINIZIO,
                                .Validita_Fine = AGRODATAFINE,
                                .Username_Creazione = username,
                                .Username_Modifica = username
                            }
                            GiasContext.CentrixIndirizzi.Add(cxi)

                            ' nuovo recode indirizzo
                            Dim recode_indirizzo =
                                New G2G_Recode_Indirizzi With {
                                    .From_PivaSuperUser = From_PivaSuperUser,
                                    .To_PivaSuperUser = To_PivaSuperUser,
                                    .From_Cod_Indirizzo = indirizzo.cod_indirizzo,
                                    .To_Cod_Indirizzo = ii.Indirizzo.cod_indirizzo,
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

                        Next

                        'nuovo recode centro aziendale
                        Dim recode =
                            New G2G_Recode_Imprese With {
                                .From_PivaSuperUser = From_PivaSuperUser,
                                .To_PivaSuperUser = To_PivaSuperUser,
                                .FROM_Piva = c.Centro.PIVA,
                                .To_Piva = centro.PIVA,
                                .FROM_SaCod = centro.sa_cod,
                                .TO_SaCod = c.Centro.sa_cod,
                                .Username_Creazione = username,
                                .Username_Modifica = username,
                                .Data_Creazione = Now(),
                                .Data_Modifica = Now(),
                                .Validita_Inizio = AGRODATAINIZIO,
                                .Validita_Fine = AGRODATAFINE,
                                .inviato = 0,
                                .datainvio = Now()
                            }
                        g2g.Recode.G2GRecodeImpreseCentriToInsert.Add(recode)
                        GiasContext.G2G_Recode_Imprese.Add(recode)

                        G2GUtility.SaveChanges(GiasContext, index)

                        ' scrive fabbricati centro
                        'c.Fabbricati.To_Sa_Cod = centro.sa_cod
                        'objFabbricati.Scrivi_Fabbricati(c.Fabbricati, objParametri)
                        'G2GUtility.Copia_G2G_Recode(c.Fabbricati.Recode, g2g.Recode)

                    Next

                    GiasContext.SaveChanges()

                End If

                ' CENTRI AZIENDALI DA MODIFICARE
                If g2g.CentriAziendaliToUpdate.Count > 0 Then

                    Dim index As Integer = 0

                    ' centri da modificare
                    For Each c As G2G_CentroAziendale In g2g.CentriAziendaliToUpdate

                        ' modifica recode centro
                        Dim recode = (From rr In GiasContext.G2G_Recode_Imprese Where rr.From_PivaSuperUser = From_PivaSuperUser AndAlso rr.To_PivaSuperUser = To_PivaSuperUser AndAlso rr.FROM_Piva = c.Centro.PIVA AndAlso rr.TO_SaCod = c.Centro.sa_cod).FirstOrDefault()
                        If recode Is Nothing Then
                            Throw New Exception("Recode non presente in destinazione")
                        End If
                        recode.Username_Modifica = username
                        recode.Data_Modifica = data
                        recode.datainvio = data
                        g2g.Recode.G2GRecodeImpreseCentriToUpdate.Add(recode)
                        GiasContext.G2G_Recode_Imprese.Attach(recode)
                        GiasContext.Entry(recode).State = EntityState.Modified

                        ' modifica centro
                        Dim centro = (From cc In GiasContext.Centri_Aziendali Where cc.PIVA = recode.FROM_Piva AndAlso cc.sa_cod = recode.FROM_SaCod).FirstOrDefault()
                        centro = Gias_EF_Utility.CopyEntity(GiasContext, c.Centro, centro, username, data)
                        centro.PIVA = recode.FROM_Piva
                        centro.sa_cod = recode.FROM_SaCod
                        GiasContext.Centri_Aziendali.Attach(centro)
                        GiasContext.Entry(centro).State = EntityState.Modified

                        ' cancella codici centro
                        Dim codici = (From cc In GiasContext.Centri_Aziendali_Codici Where cc.PIVA = centro.PIVA AndAlso cc.sa_cod = centro.sa_cod Select cc).ToList()
                        For Each cc As Centri_Aziendali_Codici In codici
                            GiasContext.Centri_Aziendali_Codici.Attach(cc)
                            GiasContext.Centri_Aziendali_Codici.Remove(cc)
                        Next

                        ' inserisce codici centro
                        For Each cc As Centri_Aziendali_Codici In c.Codici
                            Dim codice = Gias_EF_Utility.CopyEntity(GiasContext, cc, Nothing, username, data)
                            codice.PIVA = centro.PIVA
                            codice.sa_cod = centro.sa_cod
                            GiasContext.Centri_Aziendali_Codici.Add(codice)
                        Next

                        ' cancello indirizzi centro
                        Dim centrixindirizzi = (From ii In GiasContext.CentrixIndirizzi Where ii.PIVA = centro.PIVA AndAlso ii.sa_cod = centro.sa_cod Select ii).ToList()
                        For Each cxi As CentrixIndirizzi In centrixindirizzi
                            GiasContext.CentrixIndirizzi.Attach(cxi)
                            GiasContext.CentrixIndirizzi.Remove(cxi)
                        Next

                        ' inserisco / modifico indirizzi centro
                        For Each ii As G2G_Indirizzo In c.Indirizzi

                            Dim recode_indirizzo = (From rr In GiasContext.G2G_Recode_Indirizzi Where rr.From_PivaSuperUser = From_PivaSuperUser AndAlso rr.To_PivaSuperUser = To_PivaSuperUser AndAlso rr.From_Cod_Indirizzo = ii.Indirizzo.cod_indirizzo).FirstOrDefault()
                            Dim indirizzo = If(recode_indirizzo Is Nothing, Nothing, (From iii In GiasContext.Indirizzi Where iii.cod_indirizzo = recode_indirizzo.To_Cod_Indirizzo Select iii).FirstOrDefault())

                            If indirizzo Is Nothing Then

                                ' nuovo indirizzo
                                Dim idSeq = G2GUtility.NuovoId_Tabella(GiasContext, "Indirizzi", 0, 2000000000, objParametri)
                                indirizzo = Gias_EF_Utility.CopyEntity(GiasContext, ii.Indirizzo, Nothing, username, data)
                                indirizzo.cod_indirizzo = idSeq
                                GiasContext.Indirizzi.Add(indirizzo)

                                ' nuovo recode indirizzi
                                Dim ri =
                                    New G2G_Recode_Indirizzi With {
                                        .From_PivaSuperUser = From_PivaSuperUser,
                                        .To_PivaSuperUser = To_PivaSuperUser,
                                        .From_Cod_Indirizzo = ii.Indirizzo.cod_indirizzo,
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
                                indirizzo = Gias_EF_Utility.CopyEntity(GiasContext, ii.Indirizzo, indirizzo, username, data)
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

                            ' nuovo indirizzo centro
                            Dim cxi = New CentrixIndirizzi With {
                                .PIVA = centro.PIVA,
                                .sa_cod = centro.sa_cod,
                                .cod_indirizzo = indirizzo.cod_indirizzo,
                                .Tipo_Indirizzo = ii.Tipo_Indirizzo,
                                .inviato = 0,
                                .Data_Creazione = data,
                                .Data_Modifica = data,
                                .Validita_Inizio = AGRODATAINIZIO,
                                .Validita_Fine = AGRODATAFINE,
                                .Username_Creazione = username,
                                .Username_Modifica = username
                            }
                            GiasContext.CentrixIndirizzi.Add(cxi)

                        Next

                        G2GUtility.SaveChanges(GiasContext, index)

                        ' scrive fabbricati centro
                        'c.Fabbricati.To_Sa_Cod = centro.sa_cod
                        'objFabbricati.Scrivi_Fabbricati(c.Fabbricati, objParametri)
                        'G2GUtility.Copia_G2G_Recode(c.Fabbricati.Recode, g2g.Recode)

                    Next

                    GiasContext.SaveChanges()

                End If

                ' log importazione
                g2g.Recode.LogRecode = g2g.CentriAziendaliToInsert.Count & " nuovi, " & g2g.CentriAziendaliToUpdate.Count & " modificati, " & g2g.Recode.G2GRecodeImpreseCentriToDelete.Count & " cancellati"

            End Using

        Catch ex As Exception

            messaggioErrore = ex.Message
            g2g.Recode.MessaggioErrore = messaggioErrore
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)

        End Try

        Return g2g.Recode

    End Function

End Class
