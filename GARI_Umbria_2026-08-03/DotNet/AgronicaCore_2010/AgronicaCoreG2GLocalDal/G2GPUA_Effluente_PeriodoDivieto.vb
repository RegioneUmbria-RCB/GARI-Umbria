Imports System.Data.Entity
Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModello

Public Class G2GPUA_Effluente_PeriodoDivieto_R

    Public Function LeggiPerGias2Gias(ByVal piva As String,
                                      ByVal piva_destinazione As String,
                                      ByVal PivaSuperUser_Destinazione As String,
                                      ByVal objOpzioniImportImpresa As clsImpresa,
                                      ByRef objParametri_server As AgronicaCoreParametri) As G2G_PUA_Effluente_PeriodoDivieto

        Dim gefutils As New AgronicaCoreEntityFramework.Gias_EF_Utility
        Dim efConnString As String = gefutils.GetEntityConnectionString(objParametri_server.StringaConnessione)

        Dim rval As New G2G_PUA_Effluente_PeriodoDivieto With {
            .From_Piva = piva,
            .From_PivaSuperUser = objParametri_server.PivaSuperUser,
            .Recode = G2GUtility.Nuovo_G2G_Recode(),
            .To_Piva = piva_destinazione
        }

        Dim psu As String = objParametri_server.PivaSuperUser

        Using giasContext As New AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities(efConnString)
            giasContext.Database.CommandTimeout = 3600
            rval.pua_effluente_periododivieto_insert = (
            From plp In giasContext.PUA_Effluente_PeriodoDivieto
            Join pua In giasContext.PUA_Testata On plp.Pua_Cod Equals pua.PUA_Cod And plp.Regolamento_Cod Equals pua.Regolamento_Cod
            Where pua.Piva = piva _
                AndAlso plp.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_pua _
                AndAlso Not giasContext.G2G_Recode_PUA_PeriodoDivieto.Any(Function(g) g.From_PivaSuperUser = psu And g.To_PivaSuperUser = PivaSuperUser_Destinazione And g.From_ID = plp.ID)
            Select plp).Distinct().ToList()


            rval.pua_effluente_periododivieto_update = (
            From plp In giasContext.PUA_Effluente_PeriodoDivieto
            Join pua In giasContext.PUA_Testata On plp.Pua_Cod Equals pua.PUA_Cod And plp.Regolamento_Cod Equals pua.Regolamento_Cod
            Join g2g In giasContext.G2G_Recode_PUA_PeriodoDivieto
                On plp.ID Equals g2g.From_ID
            Where pua.Piva = piva _
                AndAlso g2g.From_PivaSuperUser = psu AndAlso g2g.To_PivaSuperUser = PivaSuperUser_Destinazione _
                AndAlso System.Data.Entity.DbFunctions.AddDays(g2g.Data_Modifica, -1) < plp.Data_Modifica
            Select plp
            ).Distinct().ToList()

            rval.Recode.G2GRecodePUA_Effluente_PeriodoDivietoToDelete = (
            From r In giasContext.G2G_Recode_PUA_PeriodoDivieto
            Where r.From_PivaSuperUser = psu AndAlso r.To_PivaSuperUser = PivaSuperUser_Destinazione _
                AndAlso Not giasContext.PUA_Effluente_PeriodoDivieto.Any(Function(plp) psu = r.From_PivaSuperUser And plp.ID = r.From_ID) _
                AndAlso r.To_PivaSuperUser = PivaSuperUser_Destinazione
            Select r).Distinct().ToList()

        End Using

        Return rval

    End Function

End Class


Public Class G2GPUA_Effluente_PeriodoDivieto_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi_PUA_Effluente_PeriodoDivieto_G2G(Origine_Piva_SuperUser As String, Destinazione_Piva_SuperUser As String, ByRef g2g As G2G_PUA_Effluente_PeriodoDivieto, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GPUA_Effluente_PeriodoDivieto_W.Scrivi_PUA_Effluente_PeriodoDivieto_G2G()"

        Dim username = objParametri.UsernameOperazione
        Dim data = Date.Now

        Dim To_Piva = g2g.To_Piva
        Dim From_Piva = g2g.From_Piva

        Dim objSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Dim transactionOptions As New TransactionOptions()
        transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
        transactionOptions.Timeout = G2GUtility.GetTransactionTimeout(objParametri)

        Try

            Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    'PUA_Effluente_PeriodoDivieto - INSERT
                    Try

                        For Each plp As PUA_Effluente_PeriodoDivieto In g2g.pua_effluente_periododivieto_insert

                            'Richiedo un nuovo id sequenza
                            Dim idSeq As Integer = objSequenze.NuovoId_Tabella("PUA_Effluente_PeriodoDivieto", 0, 2000000000, objParametri, UtilizzaTransazione:=False)

                            Dim plp_2Add = Gias_EF_Utility.CopyEntity(GiasContext, plp, Nothing, username, data)
                            plp_2Add.ID = idSeq
                            plp_2Add.Piva_SuperUser = Destinazione_Piva_SuperUser

                            'Sistemo le codifiche
                            '--PUA_Cod
                            Dim pua = (From rr In GiasContext.G2G_Recode_Pua
                                       Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso
                                            rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso
                                            rr.From_Piva = From_Piva And
                                            rr.From_Pua_Cod = plp.Pua_Cod And
                                            rr.From_Regolamento_Cod = plp.Regolamento_Cod Select rr).FirstOrDefault
                            If pua Is Nothing Then
                                Throw New Exception("To_Pua_Cod non trovato PUA_Effluente_PeriodoDivieto Insert")
                            End If
                            plp_2Add.Pua_Cod = pua.To_Pua_Cod

                            '--PUA_Effluente
                            Dim effluente = (From rr In GiasContext.G2G_Recode_PUA_Effluente
                                             Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso
                                            rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso
                                            rr.From_ID = plp.ID_Pua_Effluente
                                             Select rr).FirstOrDefault

                            If effluente Is Nothing Then
                                Throw New Exception("To_ID_Pua_Effluente non trovato PUA_Effluente_PeriodoDivieto Insert")
                            End If
                            plp_2Add.ID_Pua_Effluente = effluente.To_ID

                            GiasContext.PUA_Effluente_PeriodoDivieto.Add(plp_2Add)

                            Dim recode =
                                New G2G_Recode_PUA_PeriodoDivieto With {
                                    .From_PivaSuperUser = Origine_Piva_SuperUser,
                                    .To_PivaSuperUser = Destinazione_Piva_SuperUser,
                                    .From_Piva = From_Piva,
                                    .To_Piva = To_Piva,
                                    .From_Pua_Cod = plp.Pua_Cod,
                                    .To_Pua_Cod = plp_2Add.Pua_Cod,
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
                            g2g.Recode.G2GRecodePUA_Effluente_PeriodoDivietoToInsert.Add(recode)
                            GiasContext.G2G_Recode_PUA_PeriodoDivieto.Add(recode)

                        Next

                        GiasContext.SaveChanges()

                    Catch ex As Exception
                        Throw New Exception("Err durante il salvataggio di PUA_Effluente_PeriodoDivieto: " & ex.Message)
                    End Try

                    'PUA_Effluente_PeriodoDivieto - UPDATE
                    Try

                        For Each plp As PUA_Effluente_PeriodoDivieto In g2g.pua_effluente_periododivieto_update

                            'Aggiorno i recode per gli elementi aggiornati
                            Dim recode = (From rr In GiasContext.G2G_Recode_PUA_PeriodoDivieto Where
                                                                                                   rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso
                                                                                                   rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso
                                                                                                   rr.To_Piva = To_Piva AndAlso
                                                                                                   rr.From_Piva = From_Piva AndAlso
                                                                                                   rr.From_Pua_Cod = plp.Pua_Cod AndAlso
                                                                                                   rr.From_ID = plp.ID).First()
                            recode.Username_Modifica = username
                            recode.Data_Modifica = data
                            recode.datainvio = data
                            g2g.Recode.G2GRecodePUA_Effluente_PeriodoDivietoToUpdate.Add(recode)
                            GiasContext.G2G_Recode_PUA_PeriodoDivieto.Attach(recode)
                            GiasContext.Entry(recode).State = EntityState.Modified

                            'Modifico l'oggetto
                            Dim plp_2Upd = (From x In GiasContext.PUA_Effluente_PeriodoDivieto Where
                                                                                                   x.Piva_SuperUser = recode.To_PivaSuperUser AndAlso
                                                                                                   x.Pua_Cod = recode.To_Pua_Cod AndAlso
                                                                                                   x.ID = recode.To_ID).First()

                            plp_2Upd = Gias_EF_Utility.CopyEntity(GiasContext, plp, plp_2Upd, username, data)
                            plp_2Upd.ID = recode.To_ID
                            plp_2Upd.Piva_SuperUser = Destinazione_Piva_SuperUser

                            'Sistemo le codifiche
                            '--PUA_Cod
                            Dim pua = (From rr In GiasContext.G2G_Recode_Pua
                                       Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso
                                            rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso
                                            rr.From_Piva = From_Piva And
                                            rr.From_Pua_Cod = plp.Pua_Cod And
                                            rr.From_Regolamento_Cod = plp.Regolamento_Cod Select rr).FirstOrDefault
                            If pua Is Nothing Then
                                Throw New Exception("To_Pua_Cod non trovato PUA_Effluente_PeriodoDivieto Update")
                            End If
                            plp_2Upd.Pua_Cod = pua.To_Pua_Cod

                            '--PUA_Effluente
                            Dim effluente = (From rr In GiasContext.G2G_Recode_PUA_Effluente
                                             Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso
                                            rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso
                                            rr.From_ID = plp.ID_Pua_Effluente
                                             Select rr).FirstOrDefault

                            If effluente Is Nothing Then
                                Throw New Exception("To_ID_Pua_Effluente non trovato PUA_Effluente_PeriodoDivieto Update")
                            End If
                            plp_2Upd.ID_Pua_Effluente = effluente.To_ID

                            GiasContext.PUA_Effluente_PeriodoDivieto.Attach(plp_2Upd)
                            GiasContext.Entry(plp_2Upd).State = EntityState.Modified
                        Next

                        GiasContext.SaveChanges()

                    Catch ex As Exception
                        Throw New Exception("Err durante la modifica di PUA_Effluente_PeriodoDivieto: " & ex.Message)
                    End Try

                    'G2G_Recode_PUA_PeriodoDivieto - CANCELLAZIONE
                    Try

                        For Each recode As G2G_Recode_PUA_PeriodoDivieto In g2g.Recode.G2GRecodePUA_Effluente_PeriodoDivietoToDelete

                            Dim obj = (From plp In GiasContext.PUA_Effluente_PeriodoDivieto Where recode.To_PivaSuperUser = plp.Piva_SuperUser AndAlso
                                                                                                recode.To_Pua_Cod = plp.Pua_Cod AndAlso
                                                                                                recode.To_ID = plp.ID).First()
                            GiasContext.PUA_Effluente_PeriodoDivieto.Attach(obj)
                            GiasContext.PUA_Effluente_PeriodoDivieto.Remove(obj)

                            ' Cancello il recode corrispondente
                            Dim recode2Delete = (From rr In GiasContext.G2G_Recode_PUA_PeriodoDivieto
                                                 Where rr.From_PivaSuperUser = recode.From_PivaSuperUser AndAlso
                                                     rr.From_Piva = recode.From_Piva AndAlso
                                                     rr.From_Pua_Cod = recode.From_Pua_Cod AndAlso
                                                     rr.From_ID = recode.From_ID AndAlso
                                                     rr.To_PivaSuperUser = recode.To_PivaSuperUser AndAlso
                                                     rr.To_Piva = recode.To_Piva AndAlso
                                                     rr.To_Pua_Cod = recode.To_Pua_Cod AndAlso
                                                     rr.To_ID = recode.To_ID
                                                 Select rr).First()
                            GiasContext.G2G_Recode_PUA_PeriodoDivieto.Attach(recode2Delete)
                            GiasContext.G2G_Recode_PUA_PeriodoDivieto.Remove(recode2Delete)

                        Next

                        GiasContext.SaveChanges()

                    Catch ex As Exception
                        Throw New Exception("Err durante la cancellazione di PUA_Effluente_PeriodoDivieto: " & ex.Message)
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
