Imports System.Data.Entity
Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModello

Public Class G2GAnagrafe_VincoliAgronomici_R

    Public Function LeggiPerGias2Gias(ByVal piva As String,
                                      ByVal piva_destinazione As String,
                                      ByVal PivaSuperUser_Destinazione As String,
                                      ByVal objOpzioniImportImpresa As clsImpresa,
                                      ByRef objParametri_server As AgronicaCoreParametri) As G2G_Anagrafe_VincoliAgronomici

        Dim gefutils As New AgronicaCoreEntityFramework.Gias_EF_Utility
        Dim efConnString As String = gefutils.GetEntityConnectionString(objParametri_server.StringaConnessione)

        Dim rval As New G2G_Anagrafe_VincoliAgronomici With {
            .From_Piva = piva,
            .From_PivaSuperUser = objParametri_server.PivaSuperUser,
            .Recode = G2GUtility.Nuovo_G2G_Recode(),
            .To_Piva = piva_destinazione
        }

        Dim psu As String = objParametri_server.PivaSuperUser

        Using giasContext As New AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities(efConnString)
            giasContext.Database.CommandTimeout = 3600
            rval.anagrafe_vincoli_agronomici_insert = (
            From ava In giasContext.Anagrafe_VincoliAgronomici
            Where ava.Piva = piva _
                AndAlso ava.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_pua _
                AndAlso Not giasContext.G2G_Recode_Anagrafe_VincoliAgronomici.Any(Function(g) g.From_PivaSuperUser = psu And g.To_PivaSuperUser = PivaSuperUser_Destinazione And g.From_ID = ava.ID)
            Select ava).Distinct().ToList()


            rval.anagrafe_vincoli_agronomici_update = (
            From ava In giasContext.Anagrafe_VincoliAgronomici
            Join g2g In giasContext.G2G_Recode_Anagrafe_VincoliAgronomici
                On ava.ID Equals g2g.From_ID
            Where ava.Piva = piva _
                AndAlso g2g.From_PivaSuperUser = psu _
                AndAlso g2g.To_PivaSuperUser = PivaSuperUser_Destinazione _
                AndAlso System.Data.Entity.DbFunctions.AddDays(g2g.Data_Modifica, -1) < ava.Data_Modifica
            Select ava
            ).Distinct().ToList()

            rval.Recode.G2GRecodeAnagrafe_VincoliAgronomiciToDelete = (
            From r In giasContext.G2G_Recode_Anagrafe_VincoliAgronomici
            Group Join ava In giasContext.Anagrafe_VincoliAgronomici.Where(Function(x) x.Piva_SuperUser = psu) On ava.ID Equals r.From_ID
                Into ava_group = Group
            From _ava_group In ava_group.DefaultIfEmpty
            Where r.From_PivaSuperUser = psu AndAlso r.To_PivaSuperUser = PivaSuperUser_Destinazione _
                AndAlso r.To_PivaSuperUser = PivaSuperUser_Destinazione _
                AndAlso _ava_group Is Nothing
            Select r).Distinct().ToList()

        End Using

        Return rval

    End Function

    Public Function LeggiPerGias2GiasReverse(ByVal piva As String,
                                      ByVal piva_destinazione As String,
                                      ByVal PivaSuperUser_Destinazione As String,
                                      ByVal objOpzioniImportImpresa As clsImpresa,
                                      ByRef objParametri_server As AgronicaCoreParametri) As G2G_Anagrafe_VincoliAgronomici_Reverse

        Dim gefutils As New AgronicaCoreEntityFramework.Gias_EF_Utility
        Dim efConnString As String = gefutils.GetEntityConnectionString(objParametri_server.StringaConnessione)

        Dim rval As New G2G_Anagrafe_VincoliAgronomici_Reverse With {
            .From_Piva = piva,
            .From_PivaSuperUser = objParametri_server.PivaSuperUser,
            .Recode = G2GUtility.Nuovo_G2G_Recode(),
            .To_Piva = piva_destinazione
        }

        Dim psu As String = objParametri_server.PivaSuperUser

        Using giasContext As New AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities(efConnString)
            giasContext.Database.CommandTimeout = 3600
            rval.anagrafe_vincoli_agronomici_insert = (
            From ava In giasContext.Anagrafe_VincoliAgronomici
            Where ava.Piva = piva _
                AndAlso ava.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_pua _
                AndAlso Not giasContext.G2G_Recode_Anagrafe_VincoliAgronomici.Any(Function(g) g.To_PivaSuperUser = psu And g.From_PivaSuperUser = PivaSuperUser_Destinazione And g.To_ID = ava.ID)
            Select ava).Distinct().ToList()


            rval.anagrafe_vincoli_agronomici_update = (
            From ava In giasContext.Anagrafe_VincoliAgronomici
            Join g2g In giasContext.G2G_Recode_Anagrafe_VincoliAgronomici
                On ava.ID Equals g2g.From_ID
            Where ava.Piva = piva _
                AndAlso g2g.To_PivaSuperUser = psu _
                AndAlso g2g.From_PivaSuperUser = PivaSuperUser_Destinazione _
                AndAlso System.Data.Entity.DbFunctions.AddDays(g2g.Data_Modifica, -1) < ava.Data_Modifica
            Select ava
            ).Distinct().ToList()

            rval.Recode.G2GRecodeAnagrafe_VincoliAgronomiciToDelete = (
            From r In giasContext.G2G_Recode_Anagrafe_VincoliAgronomici
            Where r.To_PivaSuperUser = psu AndAlso r.From_PivaSuperUser = PivaSuperUser_Destinazione _
                AndAlso Not giasContext.Anagrafe_VincoliAgronomici.Any(Function(ava) psu = r.To_PivaSuperUser And ava.ID = r.To_ID) _
                AndAlso r.From_PivaSuperUser = PivaSuperUser_Destinazione
            Select r).Distinct().ToList()

        End Using

        Return rval

    End Function

End Class

Public Class G2GAnagrafe_VincoliAgronomici_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi_Anagrafe_VincoliAgronomici_G2G(Origine_Piva_SuperUser As String, Destinazione_Piva_SuperUser As String, ByRef g2g As G2G_Anagrafe_VincoliAgronomici, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GAnagrafe_VincoliAgronomici_W.Scrivi_Anagrafe_VincoliAgronomici_G2G()"

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

                    'Anagrafe_VincoliAgronomici - INSERT
                    Try

                        For Each ava As Anagrafe_VincoliAgronomici In g2g.anagrafe_vincoli_agronomici_insert

                            'Richiedo un nuovo id sequenza
                            Dim idSeq As Integer = objSequenze.NuovoId_Tabella("Anagrafe_VincoliAgronomici", 0, 2000000000, objParametri, UtilizzaTransazione:=False)

                            Dim ava_2Add = Gias_EF_Utility.CopyEntity(GiasContext, ava, Nothing, username, data)
                            ava_2Add.ID = idSeq

                            'Sistemo le codifiche
                            Dim recImp = (From rr In GiasContext.G2G_Recode_Impianti Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_Piva = ava.Piva And rr.From_Sa_Cod = ava.Sa_Cod And rr.From_Appezza = ava.Appezza And rr.From_Id_Reg = ava.Id_Reg Select rr).First()
                            ava_2Add.Piva = recImp.To_Piva
                            ava_2Add.Sa_Cod = recImp.To_Sa_Cod
                            ava_2Add.Appezza = recImp.To_Appezza
                            ava_2Add.Id_Reg = recImp.To_Id_Reg

                            ava_2Add.Piva_SuperUser = Destinazione_Piva_SuperUser
                            ava_2Add.Progetto_Cod = (From rr In GiasContext.G2G_Recode_Distinta Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_Piva = ava.Piva And rr.From_Progetto_cod = ava.Progetto_Cod Select rr.To_Progetto_cod).First()

                            If IsNumeric(ava.Analisi_Testata_Cod) AndAlso ava.Analisi_Testata_Cod > 0 Then
                                ava_2Add.Analisi_Testata_Cod = (From rr In GiasContext.G2G_Recode_Analisi_Testata Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_Analisi_Testata_Cod = ava.Analisi_Testata_Cod Select rr.To_Analisi_Testata_Cod).First()
                            End If

                            If IsNumeric(ava.Pua_Cod) AndAlso ava.Pua_Cod > 0 Then
                                ava_2Add.Pua_Cod = (From rr In GiasContext.G2G_Recode_Pua Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_Piva = ava.Piva And rr.From_Pua_Cod = ava.Pua_Cod And rr.From_Regolamento_Cod = ava.Regolamento_Cod Select rr.To_Pua_Cod).First()
                            End If

                            GiasContext.Anagrafe_VincoliAgronomici.Add(ava_2Add)

                            Dim recode =
                                New G2G_Recode_Anagrafe_VincoliAgronomici With {
                                    .From_PivaSuperUser = Origine_Piva_SuperUser,
                                    .To_PivaSuperUser = Destinazione_Piva_SuperUser,
                                    .From_ID = ava.ID,
                                    .To_ID = idSeq,
                                    .Username_Creazione = username,
                                    .Username_Modifica = username,
                                    .Data_Creazione = Now(),
                                    .Data_Modifica = Now(),
                                    .Validita_Inizio = AGRODATAINIZIO,
                                    .Validita_Fine = AGRODATAFINE,
                                    .inviato = "0",
                                    .datainvio = Now()
                                }
                            g2g.Recode.G2GRecodeAnagrafe_VincoliAgronomiciToInsert.Add(recode)
                            GiasContext.G2G_Recode_Anagrafe_VincoliAgronomici.Add(recode)

                        Next

                        GiasContext.SaveChanges()

                    Catch ex As Exception
                        Throw New Exception("Err durante il salvataggio di AVA: " & ex.Message)
                    End Try

                    'Anagrafe_VincoliAgronomici - UPDATE
                    Try

                        For Each ava As Anagrafe_VincoliAgronomici In g2g.anagrafe_vincoli_agronomici_update

                            'Aggiorno i recode per gli elementi aggiornati
                            Dim recode = (From rr In GiasContext.G2G_Recode_Anagrafe_VincoliAgronomici Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_ID = ava.ID).First()
                            recode.Username_Modifica = username
                            recode.Data_Modifica = data
                            recode.datainvio = data
                            g2g.Recode.G2GRecodeAnagrafe_VincoliAgronomiciToUpdate.Add(recode)
                            GiasContext.G2G_Recode_Anagrafe_VincoliAgronomici.Attach(recode)
                            GiasContext.Entry(recode).State = EntityState.Modified

                            'Modfico l'oggetto
                            Dim ava_2Upd = (From x In GiasContext.Anagrafe_VincoliAgronomici Where Destinazione_Piva_SuperUser = recode.To_PivaSuperUser AndAlso x.ID = recode.To_ID).First()
                            ava_2Upd = Gias_EF_Utility.CopyEntity(GiasContext, ava, ava_2Upd, username, data)
                            ava_2Upd.ID = recode.To_ID

                            'Sistemo le codifiche
                            Dim recImp = (From rr In GiasContext.G2G_Recode_Impianti Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_Piva = ava.Piva And rr.From_Sa_Cod = ava.Sa_Cod And rr.From_Appezza = ava.Appezza And rr.From_Id_Reg = ava.Id_Reg Select rr).First()
                            ava_2Upd.Piva = recImp.To_Piva
                            ava_2Upd.Sa_Cod = recImp.To_Sa_Cod
                            ava_2Upd.Appezza = recImp.To_Appezza
                            ava_2Upd.Id_Reg = recImp.To_Id_Reg

                            ava_2Upd.Piva_SuperUser = Destinazione_Piva_SuperUser
                            ava_2Upd.Progetto_Cod = (From rr In GiasContext.G2G_Recode_Distinta Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_Piva = ava.Piva And rr.From_Progetto_cod = ava.Progetto_Cod Select rr.To_Progetto_cod).First()

                            If IsNumeric(ava.Analisi_Testata_Cod) AndAlso ava.Analisi_Testata_Cod > 0 Then
                                ava_2Upd.Analisi_Testata_Cod = (From rr In GiasContext.G2G_Recode_Analisi_Testata Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_Analisi_Testata_Cod = ava.Analisi_Testata_Cod Select rr.To_Analisi_Testata_Cod).First()
                            End If

                            If IsNumeric(ava.Pua_Cod) AndAlso ava.Pua_Cod > 0 Then
                                ava_2Upd.Pua_Cod = (From rr In GiasContext.G2G_Recode_Pua Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_Piva = ava.Piva And rr.From_Pua_Cod = ava.Pua_Cod And rr.From_Regolamento_Cod = ava.Regolamento_Cod Select rr.To_Pua_Cod).First()
                            End If

                            GiasContext.Anagrafe_VincoliAgronomici.Attach(ava_2Upd)
                            GiasContext.Entry(ava_2Upd).State = EntityState.Modified
                        Next

                        GiasContext.SaveChanges()

                    Catch ex As Exception
                        Throw New Exception("Err durante la modifica di AVA: " & ex.Message)
                    End Try

                    'Anagrafe_VincoliAgronomici - CANCELLAZIONE
                    Try

                        For Each recode As G2G_Recode_Anagrafe_VincoliAgronomici In g2g.Recode.G2GRecodeAnagrafe_VincoliAgronomiciToDelete

                            Dim obj = (From ava In GiasContext.Anagrafe_VincoliAgronomici Where recode.To_ID = ava.ID).FirstOrDefault
                            If obj IsNot Nothing Then
                                GiasContext.Anagrafe_VincoliAgronomici.Attach(obj)
                                GiasContext.Anagrafe_VincoliAgronomici.Remove(obj)
                            End If

                            ' Cancello il recode corrispondente
                            Dim recode2Delete = (From rr In GiasContext.G2G_Recode_Anagrafe_VincoliAgronomici
                                                 Where rr.From_PivaSuperUser = recode.From_PivaSuperUser AndAlso rr.From_ID = recode.From_ID _
                                                 AndAlso rr.To_PivaSuperUser = recode.To_PivaSuperUser AndAlso rr.To_ID = recode.To_ID
                                                 Select rr).FirstOrDefault()
                            If recode2Delete IsNot Nothing Then
                                GiasContext.G2G_Recode_Anagrafe_VincoliAgronomici.Attach(recode2Delete)
                                GiasContext.G2G_Recode_Anagrafe_VincoliAgronomici.Remove(recode2Delete)
                            End If

                        Next

                        GiasContext.SaveChanges()

                    Catch ex As Exception
                        Throw New Exception("Err durante la cancellazione di AVA: " & ex.Message)
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

    Public Function Scrivi_Anagrafe_VincoliAgronomici_G2GReverse(Origine_Piva_SuperUser As String, Destinazione_Piva_SuperUser As String, ByRef g2g As G2G_Anagrafe_VincoliAgronomici_Reverse, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GAnagrafe_VincoliAgronomici_W.Scrivi_Anagrafe_VincoliAgronomici_G2GReverse()"

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

                    'Anagrafe_VincoliAgronomici - INSERT
                    Try

                        For Each ava As Anagrafe_VincoliAgronomici In g2g.anagrafe_vincoli_agronomici_insert

                            'Richiedo un nuovo id sequenza
                            Dim idSeq As Integer = objSequenze.NuovoId_Tabella("Anagrafe_VincoliAgronomici", 0, 2000000000, objParametri, UtilizzaTransazione:=False)

                            Dim ava_2Add = Gias_EF_Utility.CopyEntity(GiasContext, ava, Nothing, username, data)
                            ava_2Add.ID = idSeq

                            'Sistemo le codifiche
                            Dim recImp = (From rr In GiasContext.G2G_Recode_Impianti Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.To_Piva = ava.Piva And rr.To_Sa_Cod = ava.Sa_Cod And rr.To_Appezza = ava.Appezza And rr.To_Id_Reg = ava.Id_Reg Select rr).First()
                            ava_2Add.Piva = recImp.From_Piva
                            ava_2Add.Sa_Cod = recImp.From_Sa_Cod
                            ava_2Add.Appezza = recImp.From_Appezza
                            ava_2Add.Id_Reg = recImp.From_Id_Reg

                            ava_2Add.Piva_SuperUser = Destinazione_Piva_SuperUser
                            ava_2Add.Progetto_Cod = (From rr In GiasContext.G2G_Recode_Distinta Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.To_Piva = ava.Piva And rr.To_Progetto_cod = ava.Progetto_Cod Select rr.From_Progetto_cod).First()

                            If IsNumeric(ava.Analisi_Testata_Cod) AndAlso ava.Analisi_Testata_Cod > 0 Then
                                ava_2Add.Analisi_Testata_Cod = (From rr In GiasContext.G2G_Recode_Analisi_Testata Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.To_Analisi_Testata_Cod = ava.Analisi_Testata_Cod Select rr.From_Analisi_Testata_Cod).First()
                            End If

                            If IsNumeric(ava.Pua_Cod) AndAlso ava.Pua_Cod > 0 Then
                                ava_2Add.Pua_Cod = (From rr In GiasContext.G2G_Recode_Pua Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.To_Piva = ava.Piva And rr.To_Pua_Cod = ava.Pua_Cod And rr.To_Regolamento_Cod = ava.Regolamento_Cod Select rr.From_Pua_Cod).First()
                            End If

                            GiasContext.Anagrafe_VincoliAgronomici.Add(ava_2Add)

                            Dim recode =
                                New G2G_Recode_Anagrafe_VincoliAgronomici With {
                                    .From_PivaSuperUser = Destinazione_Piva_SuperUser,
                                    .To_PivaSuperUser = Origine_Piva_SuperUser,
                                    .From_ID = idSeq,
                                    .To_ID = ava.ID,
                                    .Username_Creazione = username,
                                    .Username_Modifica = username,
                                    .Data_Creazione = Now(),
                                    .Data_Modifica = Now(),
                                    .Validita_Inizio = AGRODATAINIZIO,
                                    .Validita_Fine = AGRODATAFINE,
                                    .inviato = "0",
                                    .datainvio = Now()
                                }
                            g2g.Recode.G2GRecodeAnagrafe_VincoliAgronomiciToInsert.Add(recode)
                            GiasContext.G2G_Recode_Anagrafe_VincoliAgronomici.Add(recode)

                        Next

                        GiasContext.SaveChanges()

                    Catch ex As Exception
                        Throw New Exception("Err durante il salvataggio di AVA: " & ex.Message)
                    End Try

                    'Anagrafe_VincoliAgronomici - UPDATE
                    Try

                        For Each ava As Anagrafe_VincoliAgronomici In g2g.anagrafe_vincoli_agronomici_update

                            'Aggiorno i recode per gli elementi aggiornati
                            Dim recode = (From rr In GiasContext.G2G_Recode_Anagrafe_VincoliAgronomici Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.To_ID = ava.ID).First()
                            recode.Username_Modifica = username
                            recode.Data_Modifica = data
                            recode.datainvio = data
                            g2g.Recode.G2GRecodeAnagrafe_VincoliAgronomiciToUpdate.Add(recode)
                            GiasContext.G2G_Recode_Anagrafe_VincoliAgronomici.Attach(recode)
                            GiasContext.Entry(recode).State = EntityState.Modified

                            'Modfico l'oggetto
                            Dim ava_2Upd = (From x In GiasContext.Anagrafe_VincoliAgronomici Where Destinazione_Piva_SuperUser = recode.From_PivaSuperUser AndAlso x.ID = recode.From_ID).First()
                            ava_2Upd = Gias_EF_Utility.CopyEntity(GiasContext, ava, ava_2Upd, username, data)
                            ava_2Upd.ID = recode.From_ID

                            'Sistemo le codifiche
                            Dim recImp = (From rr In GiasContext.G2G_Recode_Impianti Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.To_Piva = ava.Piva And rr.To_Sa_Cod = ava.Sa_Cod And rr.To_Appezza = ava.Appezza And rr.To_Id_Reg = ava.Id_Reg Select rr).First()
                            ava_2Upd.Piva = recImp.From_Piva
                            ava_2Upd.Sa_Cod = recImp.From_Sa_Cod
                            ava_2Upd.Appezza = recImp.From_Appezza
                            ava_2Upd.Id_Reg = recImp.From_Id_Reg

                            ava_2Upd.Piva_SuperUser = Destinazione_Piva_SuperUser
                            ava_2Upd.Progetto_Cod = (From rr In GiasContext.G2G_Recode_Distinta Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.To_Piva = ava.Piva And rr.To_Progetto_cod = ava.Progetto_Cod Select rr.From_Progetto_cod).First()

                            If IsNumeric(ava.Analisi_Testata_Cod) AndAlso ava.Analisi_Testata_Cod > 0 Then
                                ava_2Upd.Analisi_Testata_Cod = (From rr In GiasContext.G2G_Recode_Analisi_Testata Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.To_Analisi_Testata_Cod = ava.Analisi_Testata_Cod Select rr.From_Analisi_Testata_Cod).First()
                            End If

                            If IsNumeric(ava.Pua_Cod) AndAlso ava.Pua_Cod > 0 Then
                                ava_2Upd.Pua_Cod = (From rr In GiasContext.G2G_Recode_Pua Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.To_Piva = ava.Piva And rr.To_Pua_Cod = ava.Pua_Cod And rr.To_Regolamento_Cod = ava.Regolamento_Cod Select rr.From_Pua_Cod).First()
                            End If

                            GiasContext.Anagrafe_VincoliAgronomici.Attach(ava_2Upd)
                            GiasContext.Entry(ava_2Upd).State = EntityState.Modified
                        Next

                        GiasContext.SaveChanges()

                    Catch ex As Exception
                        Throw New Exception("Err durante la modifica di AVA: " & ex.Message)
                    End Try

                    'Anagrafe_VincoliAgronomici - CANCELLAZIONE
                    Try

                        For Each recode As G2G_Recode_Anagrafe_VincoliAgronomici In g2g.Recode.G2GRecodeAnagrafe_VincoliAgronomiciToDelete

                            Dim obj = (From ava In GiasContext.Anagrafe_VincoliAgronomici Where recode.From_ID = ava.ID).FirstOrDefault()
                            if obj IsNot Nothing Then
                                GiasContext.Anagrafe_VincoliAgronomici.Attach(obj)
                                GiasContext.Anagrafe_VincoliAgronomici.Remove(obj)
                            End If

                            ' Cancello il recode corrispondente
                            Dim recode2Delete = (From rr In GiasContext.G2G_Recode_Anagrafe_VincoliAgronomici
                                                 Where rr.To_PivaSuperUser = recode.To_PivaSuperUser AndAlso rr.To_ID = recode.To_ID _
                                                 AndAlso rr.From_PivaSuperUser = recode.From_PivaSuperUser AndAlso rr.From_ID = recode.From_ID
                                                 Select rr).FirstOrDefault()

                            if recode2Delete IsNot Nothing Then
                                GiasContext.G2G_Recode_Anagrafe_VincoliAgronomici.Attach(recode2Delete)
                                GiasContext.G2G_Recode_Anagrafe_VincoliAgronomici.Remove(recode2Delete)
                            End If

                        Next

                        GiasContext.SaveChanges()

                    Catch ex As Exception
                        Throw New Exception("Err durante la cancellazione di AVA: " & ex.Message)
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
