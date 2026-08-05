Imports System.Data.Entity
Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModello

Public Class G2GPUA_LetamazioniPrecedenti_R

    Public Function LeggiPerGias2Gias(ByVal piva As String,
                                      ByVal piva_destinazione As String,
                                      ByVal PivaSuperUser_Destinazione As String,
                                      ByVal objOpzioniImportImpresa As clsImpresa,
                                      ByRef objParametri_server As AgronicaCoreParametri) As G2G_PUA_LetamazioniPrecedenti

        Dim gefutils As New AgronicaCoreEntityFramework.Gias_EF_Utility
        Dim efConnString As String = gefutils.GetEntityConnectionString(objParametri_server.StringaConnessione)

        Dim rval As New G2G_PUA_LetamazioniPrecedenti With {
            .From_Piva = piva,
            .From_PivaSuperUser = objParametri_server.PivaSuperUser,
            .Recode = G2GUtility.Nuovo_G2G_Recode(),
            .To_Piva = piva_destinazione
        }

        Dim psu As String = objParametri_server.PivaSuperUser

        Using giasContext As New AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities(efConnString)
            giasContext.Database.CommandTimeout = 3600
            rval.pua_letamazioni_precedenti_insert = (
            From plp In giasContext.PUA_LetamazioniPrecedenti
            Where plp.Piva = piva _
                AndAlso plp.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_pua _
                AndAlso Not giasContext.G2G_Recode_PUA_LetamazioniPrecedenti.Any(Function(g) g.From_PivaSuperUser = psu And g.To_PivaSuperUser = PivaSuperUser_Destinazione And g.From_ID = plp.ID)
            Select plp).Distinct().ToList()


            rval.pua_letamazioni_precedenti_update = (
            From plp In giasContext.PUA_LetamazioniPrecedenti
            Join g2g In giasContext.G2G_Recode_PUA_LetamazioniPrecedenti
                On plp.ID Equals g2g.From_ID
            Where plp.Piva = piva _
                AndAlso g2g.From_PivaSuperUser = psu AndAlso g2g.To_PivaSuperUser = PivaSuperUser_Destinazione _
                AndAlso System.Data.Entity.DbFunctions.AddDays(g2g.Data_Modifica, -1) < plp.Data_Modifica
            Select plp
            ).Distinct().ToList()

            rval.Recode.G2GRecodePUA_LetamazioniPrecedentiToDelete = (
            From r In giasContext.G2G_Recode_PUA_LetamazioniPrecedenti
            Where r.From_PivaSuperUser = psu AndAlso r.To_PivaSuperUser = PivaSuperUser_Destinazione _
                AndAlso Not giasContext.PUA_LetamazioniPrecedenti.Any(Function(plp) psu = r.From_PivaSuperUser And plp.ID = r.From_ID) _
                AndAlso r.To_PivaSuperUser = PivaSuperUser_Destinazione
            Select r).Distinct().ToList()

        End Using

        Return rval

    End Function

    Public Function LeggiPerGias2GiasReverse(ByVal piva As String,
                                      ByVal piva_destinazione As String,
                                      ByVal PivaSuperUser_Destinazione As String,
                                      ByVal objOpzioniImportImpresa As clsImpresa,
                                      ByRef objParametri_server As AgronicaCoreParametri) As G2G_PUA_LetamazioniPrecedenti_Reverse

        Dim gefutils As New AgronicaCoreEntityFramework.Gias_EF_Utility
        Dim efConnString As String = gefutils.GetEntityConnectionString(objParametri_server.StringaConnessione)

        Dim rval As New G2G_PUA_LetamazioniPrecedenti_Reverse With {
            .From_Piva = piva,
            .From_PivaSuperUser = objParametri_server.PivaSuperUser,
            .Recode = G2GUtility.Nuovo_G2G_Recode(),
            .To_Piva = piva_destinazione
        }

        Dim psu As String = objParametri_server.PivaSuperUser

        Using giasContext As New AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities(efConnString)
            giasContext.Database.CommandTimeout = 3600
            rval.pua_letamazioni_precedenti_insert = (
            From plp In giasContext.PUA_LetamazioniPrecedenti
            Where plp.Piva = piva _
                AndAlso plp.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_pua _
                AndAlso Not giasContext.G2G_Recode_PUA_LetamazioniPrecedenti.Any(Function(g) g.To_PivaSuperUser = psu And g.From_PivaSuperUser = PivaSuperUser_Destinazione And g.To_ID = plp.ID)
            Select plp).Distinct().ToList()


            rval.pua_letamazioni_precedenti_update = (
            From plp In giasContext.PUA_LetamazioniPrecedenti
            Join g2g In giasContext.G2G_Recode_PUA_LetamazioniPrecedenti
                On plp.ID Equals g2g.From_ID
            Where plp.Piva = piva _
                AndAlso g2g.To_PivaSuperUser = psu AndAlso g2g.From_PivaSuperUser = PivaSuperUser_Destinazione _
                AndAlso System.Data.Entity.DbFunctions.AddDays(g2g.Data_Modifica, -1) < plp.Data_Modifica
            Select plp
            ).Distinct().ToList()

            rval.Recode.G2GRecodePUA_LetamazioniPrecedentiToDelete = (
            From r In giasContext.G2G_Recode_PUA_LetamazioniPrecedenti
            Where r.To_PivaSuperUser = psu AndAlso r.From_PivaSuperUser = PivaSuperUser_Destinazione _
                AndAlso Not giasContext.PUA_LetamazioniPrecedenti.Any(Function(plp) psu = r.To_PivaSuperUser And plp.ID = r.To_ID) _
                AndAlso r.From_PivaSuperUser = PivaSuperUser_Destinazione
            Select r).Distinct().ToList()

        End Using

        Return rval

    End Function


End Class

Public Class G2GPUA_LetamazioniPrecedenti_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi_PUA_LetamazioniPrecedenti_G2G(Origine_Piva_SuperUser As String, Destinazione_Piva_SuperUser As String, ByRef g2g As G2G_PUA_LetamazioniPrecedenti, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GPUA_LetamazioniPrecedenti_W.Scrivi_PUA_LetamazioniPrecedenti_G2G()"

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

                    'PUA_LetamazioniPrecedenti - INSERT
                    Try

                        For Each plp As PUA_LetamazioniPrecedenti In g2g.pua_letamazioni_precedenti_insert

                            'Richiedo un nuovo id sequenza
                            Dim idSeq As Integer = objSequenze.NuovoId_Tabella("PUA_LetamazioniPrecedenti", 0, 2000000000, objParametri, UtilizzaTransazione:=False)

                            Dim plp_2Add = Gias_EF_Utility.CopyEntity(GiasContext, plp, Nothing, username, data)
                            plp_2Add.ID = idSeq

                            'Sistemo le codifiche
                            Dim recImp = (From rr In GiasContext.G2G_Recode_Impianti Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_Piva = plp.Piva And rr.From_Sa_Cod = plp.Sa_Cod And rr.From_Appezza = plp.Appezza And rr.From_Id_Reg = plp.Id_Reg Select rr).First()
                            plp_2Add.Piva = recImp.To_Piva
                            plp_2Add.Sa_Cod = recImp.To_Sa_Cod
                            plp_2Add.Appezza = recImp.To_Appezza
                            plp_2Add.Id_Reg = recImp.To_Id_Reg

                            plp_2Add.Piva_SuperUser = Destinazione_Piva_SuperUser
                            plp_2Add.Progetto_Cod = (From rr In GiasContext.G2G_Recode_Distinta Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_Piva = plp.Piva And rr.From_Progetto_cod = plp.Progetto_Cod Select rr.To_Progetto_cod).First()
                            plp_2Add.Pua_Cod = (From rr In GiasContext.G2G_Recode_Pua Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_Piva = plp.Piva And rr.From_Pua_Cod = plp.Pua_Cod And rr.From_Regolamento_Cod = plp.Regolamento_Cod Select rr.To_Pua_Cod).First()

                            GiasContext.PUA_LetamazioniPrecedenti.Add(plp_2Add)

                            Dim recode =
                                New G2G_Recode_PUA_LetamazioniPrecedenti With {
                                    .From_PivaSuperUser = Origine_Piva_SuperUser,
                                    .To_PivaSuperUser = Destinazione_Piva_SuperUser,
                                    .From_ID = plp.ID,
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
                            g2g.Recode.G2GRecodePUA_LetamazioniPrecedentiToInsert.Add(recode)
                            GiasContext.G2G_Recode_PUA_LetamazioniPrecedenti.Add(recode)

                        Next

                        GiasContext.SaveChanges()

                    Catch ex As Exception
                        Throw New Exception("Err durante il salvataggio di PLP: " & ex.Message)
                    End Try

                    'PUA_LetamazioniPrecedenti - UPDATE
                    Try

                        For Each plp As PUA_LetamazioniPrecedenti In g2g.pua_letamazioni_precedenti_update

                            'Aggiorno i recode per gli elementi aggiornati
                            Dim recode = (From rr In GiasContext.G2G_Recode_PUA_LetamazioniPrecedenti Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_ID = plp.ID).First()
                            recode.Username_Modifica = username
                            recode.Data_Modifica = data
                            recode.datainvio = data
                            g2g.Recode.G2GRecodePUA_LetamazioniPrecedentiToUpdate.Add(recode)
                            GiasContext.G2G_Recode_PUA_LetamazioniPrecedenti.Attach(recode)
                            GiasContext.Entry(recode).State = EntityState.Modified

                            'Modifico l'oggetto
                            Dim plp_2Upd = (From x In GiasContext.PUA_LetamazioniPrecedenti Where Destinazione_Piva_SuperUser = recode.To_PivaSuperUser AndAlso x.ID = recode.To_ID).First()
                            plp_2Upd = Gias_EF_Utility.CopyEntity(GiasContext, plp, plp_2Upd, username, data)
                            plp_2Upd.ID = recode.To_ID

                            'Sistemo le codifiche
                            Dim recImp = (From rr In GiasContext.G2G_Recode_Impianti Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_Piva = plp.Piva And rr.From_Sa_Cod = plp.Sa_Cod And rr.From_Appezza = plp.Appezza And rr.From_Id_Reg = plp.Id_Reg Select rr).First()
                            plp_2Upd.Piva = recImp.To_Piva
                            plp_2Upd.Sa_Cod = recImp.To_Sa_Cod
                            plp_2Upd.Appezza = recImp.To_Appezza
                            plp_2Upd.Id_Reg = recImp.To_Id_Reg

                            plp_2Upd.Piva_SuperUser = Destinazione_Piva_SuperUser
                            plp_2Upd.Progetto_Cod = (From rr In GiasContext.G2G_Recode_Distinta Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_Piva = plp.Piva And rr.From_Progetto_cod = plp.Progetto_Cod Select rr.To_Progetto_cod).First()
                            plp_2Upd.Pua_Cod = (From rr In GiasContext.G2G_Recode_Pua Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_Piva = plp.Piva And rr.From_Pua_Cod = plp.Pua_Cod And rr.From_Regolamento_Cod = plp.Regolamento_Cod Select rr.To_Pua_Cod).First()

                            GiasContext.PUA_LetamazioniPrecedenti.Attach(plp_2Upd)
                            GiasContext.Entry(plp_2Upd).State = EntityState.Modified
                        Next

                        GiasContext.SaveChanges()

                    Catch ex As Exception
                        Throw New Exception("Err durante la modifica di PLP: " & ex.Message)
                    End Try

                    'PUA_LetamazioniPrecedenti - CANCELLAZIONE
                    Try

                        For Each recode As G2G_Recode_PUA_LetamazioniPrecedenti In g2g.Recode.G2GRecodePUA_LetamazioniPrecedentiToDelete

                            Dim obj = (From plp In GiasContext.PUA_LetamazioniPrecedenti Where recode.To_ID = plp.ID).First()
                            GiasContext.PUA_LetamazioniPrecedenti.Attach(obj)
                            GiasContext.PUA_LetamazioniPrecedenti.Remove(obj)

                            ' Cancello il recode corrispondente
                            Dim recode2Delete = (From rr In GiasContext.G2G_Recode_PUA_LetamazioniPrecedenti
                                                 Where rr.From_PivaSuperUser = recode.From_PivaSuperUser AndAlso rr.From_ID = recode.From_ID _
                                                 AndAlso rr.To_PivaSuperUser = recode.To_PivaSuperUser AndAlso rr.To_ID = recode.To_ID
                                                 Select rr).First()
                            GiasContext.G2G_Recode_PUA_LetamazioniPrecedenti.Attach(recode2Delete)
                            GiasContext.G2G_Recode_PUA_LetamazioniPrecedenti.Remove(recode2Delete)

                        Next

                        GiasContext.SaveChanges()

                    Catch ex As Exception
                        Throw New Exception("Err durante la cancellazione di PLP: " & ex.Message)
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

    Public Function Scrivi_PUA_LetamazioniPrecedenti_G2GReverse(Origine_Piva_SuperUser As String, Destinazione_Piva_SuperUser As String, ByRef g2g As G2G_PUA_LetamazioniPrecedenti_Reverse, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GPUA_LetamazioniPrecedenti_W.Scrivi_PUA_LetamazioniPrecedenti_G2GReverse()"

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

                    'PUA_LetamazioniPrecedenti - INSERT
                    Try

                        For Each plp As PUA_LetamazioniPrecedenti In g2g.pua_letamazioni_precedenti_insert

                            'Richiedo un nuovo id sequenza
                            Dim idSeq As Integer = objSequenze.NuovoId_Tabella("PUA_LetamazioniPrecedenti", 0, 2000000000, objParametri, UtilizzaTransazione:=False)

                            Dim plp_2Add = Gias_EF_Utility.CopyEntity(GiasContext, plp, Nothing, username, data)
                            plp_2Add.ID = idSeq

                            'Sistemo le codifiche
                            Dim recImp = (From rr In GiasContext.G2G_Recode_Impianti Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.To_Piva = plp.Piva And rr.To_Sa_Cod = plp.Sa_Cod And rr.To_Appezza = plp.Appezza And rr.To_Id_Reg = plp.Id_Reg Select rr).First()
                            plp_2Add.Piva = recImp.From_Piva
                            plp_2Add.Sa_Cod = recImp.From_Sa_Cod
                            plp_2Add.Appezza = recImp.From_Appezza
                            plp_2Add.Id_Reg = recImp.From_Id_Reg

                            plp_2Add.Piva_SuperUser = Destinazione_Piva_SuperUser
                            plp_2Add.Progetto_Cod = (From rr In GiasContext.G2G_Recode_Distinta Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.To_Piva = plp.Piva And rr.To_Progetto_cod = plp.Progetto_Cod Select rr.From_Progetto_cod).First()
                            plp_2Add.Pua_Cod = (From rr In GiasContext.G2G_Recode_Pua Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.To_Piva = plp.Piva And rr.To_Pua_Cod = plp.Pua_Cod And rr.To_Regolamento_Cod = plp.Regolamento_Cod Select rr.From_Pua_Cod).First()

                            GiasContext.PUA_LetamazioniPrecedenti.Add(plp_2Add)

                            Dim recode =
                                New G2G_Recode_PUA_LetamazioniPrecedenti With {
                                    .From_PivaSuperUser = Destinazione_Piva_SuperUser,
                                    .To_PivaSuperUser = Origine_Piva_SuperUser,
                                    .From_ID = idSeq,
                                    .To_ID = plp.ID,
                                    .Username_Creazione = username,
                                    .Username_Modifica = username,
                                    .Data_Creazione = Now(),
                                    .Data_Modifica = Now(),
                                    .Validita_Inizio = AGRODATAINIZIO,
                                    .Validita_Fine = AGRODATAFINE,
                                    .inviato = "0",
                                    .datainvio = Now()
                                }
                            g2g.Recode.G2GRecodePUA_LetamazioniPrecedentiToInsert.Add(recode)
                            GiasContext.G2G_Recode_PUA_LetamazioniPrecedenti.Add(recode)

                        Next

                        GiasContext.SaveChanges()

                    Catch ex As Exception
                        Throw New Exception("Err durante il salvataggio di PLP: " & ex.Message)
                    End Try

                    'PUA_LetamazioniPrecedenti - UPDATE
                    Try

                        For Each plp As PUA_LetamazioniPrecedenti In g2g.pua_letamazioni_precedenti_update

                            'Aggiorno i recode per gli elementi aggiornati
                            Dim recode = (From rr In GiasContext.G2G_Recode_PUA_LetamazioniPrecedenti Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.To_ID = plp.ID).First()
                            recode.Username_Modifica = username
                            recode.Data_Modifica = data
                            recode.datainvio = data
                            g2g.Recode.G2GRecodePUA_LetamazioniPrecedentiToUpdate.Add(recode)
                            GiasContext.G2G_Recode_PUA_LetamazioniPrecedenti.Attach(recode)
                            GiasContext.Entry(recode).State = EntityState.Modified

                            'Modifico l'oggetto
                            Dim plp_2Upd = (From x In GiasContext.PUA_LetamazioniPrecedenti Where Destinazione_Piva_SuperUser = recode.From_PivaSuperUser AndAlso x.ID = recode.From_ID).First()
                            plp_2Upd = Gias_EF_Utility.CopyEntity(GiasContext, plp, plp_2Upd, username, data)
                            plp_2Upd.ID = recode.From_ID

                            'Sistemo le codifiche
                            Dim recImp = (From rr In GiasContext.G2G_Recode_Impianti Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.To_Piva = plp.Piva And rr.To_Sa_Cod = plp.Sa_Cod And rr.To_Appezza = plp.Appezza And rr.To_Id_Reg = plp.Id_Reg Select rr).First()
                            plp_2Upd.Piva = recImp.From_Piva
                            plp_2Upd.Sa_Cod = recImp.From_Sa_Cod
                            plp_2Upd.Appezza = recImp.From_Appezza
                            plp_2Upd.Id_Reg = recImp.From_Id_Reg

                            plp_2Upd.Piva_SuperUser = Destinazione_Piva_SuperUser
                            plp_2Upd.Progetto_Cod = (From rr In GiasContext.G2G_Recode_Distinta Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.To_Piva = plp.Piva And rr.To_Progetto_cod = plp.Progetto_Cod Select rr.From_Progetto_cod).First()
                            plp_2Upd.Pua_Cod = (From rr In GiasContext.G2G_Recode_Pua Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.To_Piva = plp.Piva And rr.To_Pua_Cod = plp.Pua_Cod And rr.To_Regolamento_Cod = plp.Regolamento_Cod Select rr.From_Pua_Cod).First()

                            GiasContext.PUA_LetamazioniPrecedenti.Attach(plp_2Upd)
                            GiasContext.Entry(plp_2Upd).State = EntityState.Modified
                        Next

                        GiasContext.SaveChanges()

                    Catch ex As Exception
                        Throw New Exception("Err durante la modifica di PLP: " & ex.Message)
                    End Try

                    'PUA_LetamazioniPrecedenti - CANCELLAZIONE
                    Try

                        For Each recode As G2G_Recode_PUA_LetamazioniPrecedenti In g2g.Recode.G2GRecodePUA_LetamazioniPrecedentiToDelete

                            Dim obj = (From plp In GiasContext.PUA_LetamazioniPrecedenti Where recode.From_ID = plp.ID).First()
                            GiasContext.PUA_LetamazioniPrecedenti.Attach(obj)
                            GiasContext.PUA_LetamazioniPrecedenti.Remove(obj)

                            ' Cancello il recode corrispondente
                            Dim recode2Delete = (From rr In GiasContext.G2G_Recode_PUA_LetamazioniPrecedenti
                                                 Where rr.To_PivaSuperUser = recode.To_PivaSuperUser AndAlso rr.To_ID = recode.To_ID _
                                                 AndAlso rr.From_PivaSuperUser = recode.From_PivaSuperUser AndAlso rr.From_ID = recode.From_ID
                                                 Select rr).First()
                            GiasContext.G2G_Recode_PUA_LetamazioniPrecedenti.Attach(recode2Delete)
                            GiasContext.G2G_Recode_PUA_LetamazioniPrecedenti.Remove(recode2Delete)

                        Next

                        GiasContext.SaveChanges()

                    Catch ex As Exception
                        Throw New Exception("Err durante la cancellazione di PLP: " & ex.Message)
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
