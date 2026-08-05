Imports System.Data.Entity
Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModello

Public Class G2GParticelleCatastalixVincoliAgronomici_R

    Public Function LeggiPerGias2Gias(ByVal piva As String,
                                      ByVal piva_destinazioni As String,
                                      ByVal PivaSuperUser_Destinazione As String,
                                      ByVal objOpzioniImportImpresa As clsImpresa,
                                      ByRef objParametri_server As AgronicaCoreParametri) As G2G_ParticelleCatastalixVincoliAgronomici

        Dim gefutils As New AgronicaCoreEntityFramework.Gias_EF_Utility
        Dim efConnString As String = gefutils.GetEntityConnectionString(objParametri_server.StringaConnessione)

        Dim rval As New G2G_ParticelleCatastalixVincoliAgronomici With {
            .From_Piva = piva,
            .From_PivaSuperUser = objParametri_server.PivaSuperUser,
            .Recode = G2GUtility.Nuovo_G2G_Recode(),
            .To_Piva = piva_destinazioni
        }

        Dim psu As String = objParametri_server.PivaSuperUser

        Using giasContext As New AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities(efConnString)
            giasContext.Database.CommandTimeout = 3600
            rval.particelle_catastali_x_vincoli_agronomici_insert = (
            From pcxva In giasContext.ParticelleCatastalixVincoliAgronomici
            Join ixa In giasContext.ImpreseXParticelle
                On pcxva.PROV Equals ixa.PROV _
                And pcxva.COM Equals ixa.COM _
                And pcxva.SEZIONE Equals ixa.SEZIONE _
                And pcxva.FOGLIO Equals ixa.FOGLIO _
                And pcxva.NUMERO Equals ixa.NUMERO _
                And pcxva.SUBALTERNO Equals ixa.SUBALTERNO
            Where ixa.PIVA = piva _
                AndAlso pcxva.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_pua _
                AndAlso Not giasContext.G2G_Recode_ParticelleCatastalixVincoliAgronomici.Any(Function(g) g.From_PivaSuperUser = psu And g.To_PivaSuperUser = PivaSuperUser_Destinazione And g.From_Id = pcxva.ID)
            Select pcxva).Distinct().ToList()
            'AndAlso ixa.Validita_Inizio <= pcxva.Validita_Fine AndAlso ixa.Validita_Fine >= pcxva.Validita_Inizio _

            rval.particelle_catastali_x_vincoli_agronomici_update = (
            From pcxva In giasContext.ParticelleCatastalixVincoliAgronomici
            Join ixa In giasContext.ImpreseXParticelle
                On pcxva.PROV Equals ixa.PROV _
                And pcxva.COM Equals ixa.COM _
                And pcxva.SEZIONE Equals ixa.SEZIONE _
                And pcxva.FOGLIO Equals ixa.FOGLIO _
                And pcxva.NUMERO Equals ixa.NUMERO _
                And pcxva.SUBALTERNO Equals ixa.SUBALTERNO
            Join g2g In giasContext.G2G_Recode_ParticelleCatastalixVincoliAgronomici
                On pcxva.ID Equals g2g.From_Id
            Where ixa.PIVA = piva _
                AndAlso g2g.From_PivaSuperUser = psu AndAlso g2g.To_PivaSuperUser = PivaSuperUser_Destinazione _
                AndAlso System.Data.Entity.DbFunctions.AddDays(g2g.Data_Modifica, -1) < pcxva.Data_Modifica
            Select pcxva
            ).Distinct().ToList()
            'AndAlso ixa.Validita_Inizio <= pcxva.Validita_Fine AndAlso ixa.Validita_Fine >= pcxva.Validita_Inizio _

            rval.Recode.G2GRecodeParticelleCatastalixVincoliAgronomiciToDelete = (
            From r In giasContext.G2G_Recode_ParticelleCatastalixVincoliAgronomici
            Where r.From_PivaSuperUser = psu AndAlso r.To_PivaSuperUser = PivaSuperUser_Destinazione _
                AndAlso Not giasContext.ParticelleCatastalixVincoliAgronomici.Any(Function(pcxva) psu = r.From_PivaSuperUser And pcxva.ID = r.From_Id) _
                AndAlso r.To_PivaSuperUser = PivaSuperUser_Destinazione
            Select r).Distinct().ToList()

        End Using

        Return rval

    End Function

    Public Function LeggiPerGias2GiasReverse(ByVal piva As String,
                                      ByVal piva_destinazioni As String,
                                      ByVal PivaSuperUser_Destinazione As String,
                                      ByVal objOpzioniImportImpresa As clsImpresa,
                                      ByRef objParametri_server As AgronicaCoreParametri) As G2G_ParticelleCatastalixVincoliAgronomici_Reverse

        Dim gefutils As New AgronicaCoreEntityFramework.Gias_EF_Utility
        Dim efConnString As String = gefutils.GetEntityConnectionString(objParametri_server.StringaConnessione)

        Dim rval As New G2G_ParticelleCatastalixVincoliAgronomici_Reverse With {
            .From_Piva = piva,
            .From_PivaSuperUser = objParametri_server.PivaSuperUser,
            .Recode = G2GUtility.Nuovo_G2G_Recode(),
            .To_Piva = piva_destinazioni
        }

        Dim psu As String = objParametri_server.PivaSuperUser

        Using giasContext As New AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities(efConnString)
            giasContext.Database.CommandTimeout = 3600
            rval.particelle_catastali_x_vincoli_agronomici_insert = (
            From pcxva In giasContext.ParticelleCatastalixVincoliAgronomici
            Join ixa In giasContext.ImpreseXParticelle
                On pcxva.PROV Equals ixa.PROV _
                And pcxva.COM Equals ixa.COM _
                And pcxva.SEZIONE Equals ixa.SEZIONE _
                And pcxva.FOGLIO Equals ixa.FOGLIO _
                And pcxva.NUMERO Equals ixa.NUMERO _
                And pcxva.SUBALTERNO Equals ixa.SUBALTERNO
            Where ixa.PIVA = piva _
                AndAlso pcxva.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_pua _
                AndAlso Not giasContext.G2G_Recode_ParticelleCatastalixVincoliAgronomici.Any(Function(g) g.To_PivaSuperUser = psu And g.From_PivaSuperUser = PivaSuperUser_Destinazione And g.To_Id = pcxva.ID)
            Select pcxva).Distinct().ToList()
            'AndAlso ixa.Validita_Inizio <= pcxva.Validita_Fine AndAlso ixa.Validita_Fine >= pcxva.Validita_Inizio _

            rval.particelle_catastali_x_vincoli_agronomici_update = (
            From pcxva In giasContext.ParticelleCatastalixVincoliAgronomici
            Join ixa In giasContext.ImpreseXParticelle
                On pcxva.PROV Equals ixa.PROV _
                And pcxva.COM Equals ixa.COM _
                And pcxva.SEZIONE Equals ixa.SEZIONE _
                And pcxva.FOGLIO Equals ixa.FOGLIO _
                And pcxva.NUMERO Equals ixa.NUMERO _
                And pcxva.SUBALTERNO Equals ixa.SUBALTERNO
            Join g2g In giasContext.G2G_Recode_ParticelleCatastalixVincoliAgronomici
                On pcxva.ID Equals g2g.To_Id
            Where ixa.PIVA = piva _
                AndAlso g2g.To_PivaSuperUser = psu AndAlso g2g.From_PivaSuperUser = PivaSuperUser_Destinazione _
                AndAlso System.Data.Entity.DbFunctions.AddDays(g2g.Data_Modifica, -1) < pcxva.Data_Modifica
            Select pcxva
            ).Distinct().ToList()
            'AndAlso ixa.Validita_Inizio <= pcxva.Validita_Fine AndAlso ixa.Validita_Fine >= pcxva.Validita_Inizio _

            rval.Recode.G2GRecodeParticelleCatastalixVincoliAgronomiciToDelete = (
            From r In giasContext.G2G_Recode_ParticelleCatastalixVincoliAgronomici
            Where r.To_PivaSuperUser = psu AndAlso r.From_PivaSuperUser = PivaSuperUser_Destinazione _
                AndAlso Not giasContext.ParticelleCatastalixVincoliAgronomici.Any(Function(pcxva) psu = r.To_PivaSuperUser And pcxva.ID = r.To_Id) _
                AndAlso r.From_PivaSuperUser = PivaSuperUser_Destinazione
            Select r).Distinct().ToList()

        End Using

        Return rval

    End Function

End Class

Public Class G2GParticelleCatastalixVincoliAgronomici_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi_ParticelleCatastalixVincoliAgronomici_G2G(Origine_Piva_SuperUser As String, Destinazione_Piva_SuperUser As String, ByRef g2g As G2G_ParticelleCatastalixVincoliAgronomici, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GParticelleCatastalixVincoliAgronomici_W.Scrivi_ParticelleCatastalixVincoliAgronomici_G2G()"

        Dim username = objParametri.UsernameOperazione
        Dim data = Date.Now

        Dim objSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Dim transactionOptions As New TransactionOptions()
        transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
        transactionOptions.Timeout = G2GUtility.GetTransactionTimeout(objParametri)

        Try

            Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    'ParticelleCatastalixVincoliAgronomici - INSERT
                    Try

                        For Each pcxva_curr As ParticelleCatastalixVincoliAgronomici In g2g.particelle_catastali_x_vincoli_agronomici_insert

                            'Richiedo un nuovo id sequenza
                            Dim idSeq As Integer = objSequenze.NuovoId_Tabella("ParticelleCatastalixVincoliAgronomici", 0, 2000000000, objParametri, UtilizzaTransazione:=False)

                            Dim pcxva_2Add = Gias_EF_Utility.CopyEntity(GiasContext, pcxva_curr, Nothing, username, data)
                            pcxva_2Add.ID = idSeq
                            'Sistemo le codifiche delle analisi
                            If IsNumeric(pcxva_curr.Analisi_Testata_Cod) AndAlso pcxva_curr.Analisi_Testata_Cod > 0 Then
                                pcxva_2Add.Analisi_Testata_Cod = (From rr In GiasContext.G2G_Recode_Analisi_Testata Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_Analisi_Testata_Cod = pcxva_curr.Analisi_Testata_Cod Select rr.To_Analisi_Testata_Cod).First()
                            End If

                            GiasContext.ParticelleCatastalixVincoliAgronomici.Add(pcxva_2Add)

                            Dim recode =
                                New G2G_Recode_ParticelleCatastalixVincoliAgronomici With {
                                    .From_PivaSuperUser = Origine_Piva_SuperUser,
                                    .To_PivaSuperUser = Destinazione_Piva_SuperUser,
                                    .From_Id = pcxva_curr.ID,
                                    .To_Id = idSeq,
                                    .Username_Creazione = username,
                                    .Username_Modifica = username,
                                    .Data_Creazione = Now(),
                                    .Data_Modifica = Now(),
                                    .Validita_Inizio = AGRODATAINIZIO,
                                    .Validita_Fine = AGRODATAFINE,
                                    .inviato = "0",
                                    .datainvio = Now()
                                }
                            g2g.Recode.G2GRecodeParticelleCatastalixVincoliAgronomiciToInsert.Add(recode)
                            GiasContext.G2G_Recode_ParticelleCatastalixVincoliAgronomici.Add(recode)

                        Next

                        GiasContext.SaveChanges()

                    Catch ex As Exception
                        Throw New Exception("Err durante il salvataggio di PCXVA: " & ex.Message)
                    End Try

                    'ParticelleCatastalixVincoliAgronomici - UPDATE
                    Try

                        For Each pcxva_curr As ParticelleCatastalixVincoliAgronomici In g2g.particelle_catastali_x_vincoli_agronomici_update

                            'Aggiorno i recode per gli elementi aggiornati
                            Dim recode = (From rr In GiasContext.G2G_Recode_ParticelleCatastalixVincoliAgronomici Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_Id = pcxva_curr.ID).First()
                            recode.Username_Modifica = username
                            recode.Data_Modifica = data
                            recode.datainvio = data
                            g2g.Recode.G2GRecodeParticelleCatastalixVincoliAgronomiciToUpdate.Add(recode)
                            GiasContext.G2G_Recode_ParticelleCatastalixVincoliAgronomici.Attach(recode)
                            GiasContext.Entry(recode).State = EntityState.Modified

                            'Modfico l'oggetto
                            Dim pcxva_2Upd = (From x In GiasContext.ParticelleCatastalixVincoliAgronomici Where Destinazione_Piva_SuperUser = recode.To_PivaSuperUser AndAlso x.ID = recode.To_Id).First()
                            pcxva_2Upd = Gias_EF_Utility.CopyEntity(GiasContext, pcxva_curr, pcxva_2Upd, username, data)
                            pcxva_2Upd.ID = recode.To_Id

                            'Sistemo le codifiche delle analisi
                            If IsNumeric(pcxva_curr.Analisi_Testata_Cod) AndAlso pcxva_curr.Analisi_Testata_Cod > 0 Then
                                pcxva_2Upd.Analisi_Testata_Cod = (From rr In GiasContext.G2G_Recode_Analisi_Testata Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_Analisi_Testata_Cod = pcxva_curr.Analisi_Testata_Cod Select rr.To_Analisi_Testata_Cod).First()
                            End If

                            GiasContext.ParticelleCatastalixVincoliAgronomici.Attach(pcxva_2Upd)
                            GiasContext.Entry(pcxva_2Upd).State = EntityState.Modified
                        Next

                        GiasContext.SaveChanges()

                    Catch ex As Exception
                        Throw New Exception("Err durante la modifica di PCXVA: " & ex.Message)
                    End Try

                    'ParticelleCatastalixVincoliAgronomici - CANCELLAZIONE
                    Try

                        For Each recode As G2G_Recode_ParticelleCatastalixVincoliAgronomici In g2g.Recode.G2GRecodeParticelleCatastalixVincoliAgronomiciToDelete

                            Dim obj = (From pcxva In GiasContext.ParticelleCatastalixVincoliAgronomici Where recode.To_Id = pcxva.ID).First()
                            GiasContext.ParticelleCatastalixVincoliAgronomici.Attach(obj)
                            GiasContext.ParticelleCatastalixVincoliAgronomici.Remove(obj)

                            ' Cancello il recode corrispondente
                            Dim recode2Delete = (From rr In GiasContext.G2G_Recode_ParticelleCatastalixVincoliAgronomici
                                                 Where rr.From_PivaSuperUser = recode.From_PivaSuperUser AndAlso rr.From_Id = recode.From_Id _
                                                 AndAlso rr.To_PivaSuperUser = recode.To_PivaSuperUser AndAlso rr.To_Id = recode.To_Id
                                                 Select rr).First()
                            GiasContext.G2G_Recode_ParticelleCatastalixVincoliAgronomici.Attach(recode2Delete)
                            GiasContext.G2G_Recode_ParticelleCatastalixVincoliAgronomici.Remove(recode2Delete)

                        Next

                        GiasContext.SaveChanges()

                    Catch ex As Exception
                        Throw New Exception("Err durante la cancellazione di PCXVA: " & ex.Message)
                    End Try

                    ' COMMIT Effettivo
                    scope.Complete()

                End Using
            End Using

        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            g2g.Recode.MessaggioErrore = ex.Message
        End Try

        Return g2g.Recode

    End Function

    Public Function Scrivi_ParticelleCatastalixVincoliAgronomici_G2GReverse(Origine_Piva_SuperUser As String, Destinazione_Piva_SuperUser As String, ByRef g2g As G2G_ParticelleCatastalixVincoliAgronomici_Reverse, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GParticelleCatastalixVincoliAgronomici_W.Scrivi_ParticelleCatastalixVincoliAgronomici_G2G()"

        Dim username = objParametri.UsernameOperazione
        Dim data = Date.Now

        Dim objSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Dim transactionOptions As New TransactionOptions()
        transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
        transactionOptions.Timeout = G2GUtility.GetTransactionTimeout(objParametri)

        Try

            Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    'ParticelleCatastalixVincoliAgronomici - INSERT
                    Try

                        For Each pcxva_curr As ParticelleCatastalixVincoliAgronomici In g2g.particelle_catastali_x_vincoli_agronomici_insert

                            'Richiedo un nuovo id sequenza
                            Dim idSeq As Integer = objSequenze.NuovoId_Tabella("ParticelleCatastalixVincoliAgronomici", 0, 2000000000, objParametri, UtilizzaTransazione:=False)

                            Dim pcxva_2Add = Gias_EF_Utility.CopyEntity(GiasContext, pcxva_curr, Nothing, username, data)
                            pcxva_2Add.ID = idSeq
                            'Sistemo le codifiche delle analisi
                            If IsNumeric(pcxva_curr.Analisi_Testata_Cod) AndAlso pcxva_curr.Analisi_Testata_Cod > 0 Then
                                pcxva_2Add.Analisi_Testata_Cod = (From rr In GiasContext.G2G_Recode_Analisi_Testata Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.To_Analisi_Testata_Cod = pcxva_curr.Analisi_Testata_Cod Select rr.From_Analisi_Testata_Cod).First()
                            End If

                            GiasContext.ParticelleCatastalixVincoliAgronomici.Add(pcxva_2Add)

                            Dim recode =
                                New G2G_Recode_ParticelleCatastalixVincoliAgronomici With {
                                    .From_PivaSuperUser = Destinazione_Piva_SuperUser,
                                    .To_PivaSuperUser = Origine_Piva_SuperUser,
                                    .From_Id = idSeq,
                                    .To_Id = pcxva_curr.ID,
                                    .Username_Creazione = username,
                                    .Username_Modifica = username,
                                    .Data_Creazione = Now(),
                                    .Data_Modifica = Now(),
                                    .Validita_Inizio = AGRODATAINIZIO,
                                    .Validita_Fine = AGRODATAFINE,
                                    .inviato = "0",
                                    .datainvio = Now()
                                }
                            g2g.Recode.G2GRecodeParticelleCatastalixVincoliAgronomiciToInsert.Add(recode)
                            GiasContext.G2G_Recode_ParticelleCatastalixVincoliAgronomici.Add(recode)

                        Next

                        GiasContext.SaveChanges()

                    Catch ex As Exception
                        Throw New Exception("Err durante il salvataggio di PCXVA: " & ex.Message)
                    End Try

                    'ParticelleCatastalixVincoliAgronomici - UPDATE
                    Try

                        For Each pcxva_curr As ParticelleCatastalixVincoliAgronomici In g2g.particelle_catastali_x_vincoli_agronomici_update

                            'Aggiorno i recode per gli elementi aggiornati
                            Dim recode = (From rr In GiasContext.G2G_Recode_ParticelleCatastalixVincoliAgronomici Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.To_Id = pcxva_curr.ID).First()
                            recode.Username_Modifica = username
                            recode.Data_Modifica = data
                            recode.datainvio = data
                            g2g.Recode.G2GRecodeParticelleCatastalixVincoliAgronomiciToUpdate.Add(recode)
                            GiasContext.G2G_Recode_ParticelleCatastalixVincoliAgronomici.Attach(recode)
                            GiasContext.Entry(recode).State = EntityState.Modified

                            'Modfico l'oggetto
                            Dim pcxva_2Upd = (From x In GiasContext.ParticelleCatastalixVincoliAgronomici Where Destinazione_Piva_SuperUser = recode.From_PivaSuperUser AndAlso x.ID = recode.From_Id).First()
                            pcxva_2Upd = Gias_EF_Utility.CopyEntity(GiasContext, pcxva_curr, pcxva_2Upd, username, data)
                            pcxva_2Upd.ID = recode.From_Id

                            'Sistemo le codifiche delle analisi
                            If IsNumeric(pcxva_curr.Analisi_Testata_Cod) AndAlso pcxva_curr.Analisi_Testata_Cod > 0 Then
                                pcxva_2Upd.Analisi_Testata_Cod = (From rr In GiasContext.G2G_Recode_Analisi_Testata Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.To_Analisi_Testata_Cod = pcxva_curr.Analisi_Testata_Cod Select rr.From_Analisi_Testata_Cod).First()
                            End If

                            GiasContext.ParticelleCatastalixVincoliAgronomici.Attach(pcxva_2Upd)
                            GiasContext.Entry(pcxva_2Upd).State = EntityState.Modified
                        Next

                        GiasContext.SaveChanges()

                    Catch ex As Exception
                        Throw New Exception("Err durante la modifica di PCXVA: " & ex.Message)
                    End Try

                    'ParticelleCatastalixVincoliAgronomici - CANCELLAZIONE
                    Try

                        For Each recode As G2G_Recode_ParticelleCatastalixVincoliAgronomici In g2g.Recode.G2GRecodeParticelleCatastalixVincoliAgronomiciToDelete

                            Dim obj = (From pcxva In GiasContext.ParticelleCatastalixVincoliAgronomici Where recode.From_Id = pcxva.ID).First()
                            GiasContext.ParticelleCatastalixVincoliAgronomici.Attach(obj)
                            GiasContext.ParticelleCatastalixVincoliAgronomici.Remove(obj)

                            ' Cancello il recode corrispondente
                            Dim recode2Delete = (From rr In GiasContext.G2G_Recode_ParticelleCatastalixVincoliAgronomici
                                                 Where rr.To_PivaSuperUser = recode.To_PivaSuperUser AndAlso rr.To_Id = recode.To_Id _
                                                 AndAlso rr.From_PivaSuperUser = recode.From_PivaSuperUser AndAlso rr.From_Id = recode.From_Id
                                                 Select rr).First()
                            GiasContext.G2G_Recode_ParticelleCatastalixVincoliAgronomici.Attach(recode2Delete)
                            GiasContext.G2G_Recode_ParticelleCatastalixVincoliAgronomici.Remove(recode2Delete)

                        Next

                        GiasContext.SaveChanges()

                    Catch ex As Exception
                        Throw New Exception("Err durante la cancellazione di PCXVA: " & ex.Message)
                    End Try

                    ' COMMIT Effettivo
                    scope.Complete()

                End Using
            End Using

        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            g2g.Recode.MessaggioErrore = ex.Message
        End Try

        Return g2g.Recode

    End Function

End Class
