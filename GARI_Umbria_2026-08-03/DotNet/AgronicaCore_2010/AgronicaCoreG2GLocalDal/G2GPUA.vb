Imports System.Data.Entity
Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModello

Public Class G2GPUA_R

    Public Function LeggiPerGias2Gias(ByVal piva As String,
                                      ByVal piva_destinazione As String,
                                      ByVal PivaSuperUser_Destinazione As String,
                                      ByVal objOpzioniImportImpresa As clsImpresa,
                                      ByRef objParametri_server As AgronicaCoreParametri) As G2G_Pua

        Dim gefutils As New AgronicaCoreEntityFramework.Gias_EF_Utility
        Dim efConnString As String = gefutils.GetEntityConnectionString(objParametri_server.StringaConnessione)

        Dim rval As New G2G_Pua With {
            .From_Piva = piva,
            .From_PivaSuperUser = objParametri_server.PivaSuperUser,
            .Recode = G2GUtility.Nuovo_G2G_Recode(),
            .To_Piva = piva_destinazione
        }

        Dim psu As String = objParametri_server.PivaSuperUser

        Using giasContext As New AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities(efConnString)
            giasContext.Database.CommandTimeout = 3600
            'PUA_TESTATA
            rval.pua_testata_insert = (
            From pt In giasContext.PUA_Testata
            Where pt.Piva = piva _
                AndAlso pt.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_pua _
                AndAlso Not giasContext.G2G_Recode_Pua.Any(Function(g) g.From_PivaSuperUser = pt.Piva_SuperUser And g.To_PivaSuperUser = PivaSuperUser_Destinazione And g.From_Piva = pt.Piva And g.From_Pua_Cod = pt.PUA_Cod And g.From_Regolamento_Cod = pt.Regolamento_Cod)
            Select pt).ToList()


            rval.pua_testata_update = (
            From pt In giasContext.PUA_Testata
            Join g2g In giasContext.G2G_Recode_Pua
                On pt.Piva_SuperUser Equals g2g.From_PivaSuperUser _
                And pt.Piva Equals g2g.From_Piva _
                And pt.PUA_Cod Equals g2g.From_Pua_Cod _
                And pt.Regolamento_Cod Equals g2g.From_Regolamento_Cod
            Where pt.Piva = piva _
                AndAlso System.Data.Entity.DbFunctions.AddDays(g2g.Data_Modifica, -1) < pt.Data_Modifica
            Select pt
            ).ToList()

            rval.Recode.G2GRecodePuaToDelete = (
            From r In giasContext.G2G_Recode_Pua
            Where r.From_PivaSuperUser = psu _
                AndAlso r.From_Piva = piva _
                AndAlso Not giasContext.PUA_Testata.Any(Function(pt) pt.Piva_SuperUser = r.From_PivaSuperUser And pt.Piva = r.From_Piva And pt.PUA_Cod = r.From_Pua_Cod And pt.Regolamento_Cod = r.From_Regolamento_Cod) _
                AndAlso r.To_PivaSuperUser = PivaSuperUser_Destinazione
            Select r).ToList()

            Dim listaInsert As List(Of PUA_Effluente) = (
            From pe In giasContext.PUA_Effluente
            Join pt In giasContext.PUA_Testata
                    On pt.Piva_SuperUser Equals pe.Piva_SuperUser _
                    And pt.PUA_Cod Equals pe.PUA_Cod _
                    And pt.Regolamento_Cod Equals pe.Regolamento_Cod
            Where pt.Piva = piva AndAlso Not giasContext.G2G_Recode_PUA_Effluente.Any(Function(g) g.From_PivaSuperUser = pe.Piva_SuperUser And g.To_PivaSuperUser = PivaSuperUser_Destinazione And g.From_ID = pe.ID)
            Select pe).ToList()

            Dim listaUpdate As List(Of PUA_Effluente) = (
            From pe In giasContext.PUA_Effluente
            Join pt In giasContext.PUA_Testata
                On pt.Piva_SuperUser Equals pe.Piva_SuperUser _
                And pt.PUA_Cod Equals pe.PUA_Cod _
                And pt.Regolamento_Cod Equals pe.Regolamento_Cod
            Join g2g In giasContext.G2G_Recode_PUA_Effluente
                On pe.Piva_SuperUser Equals g2g.From_PivaSuperUser _
                And pe.ID Equals g2g.From_ID
            Where pt.Piva = piva _
                AndAlso System.Data.Entity.DbFunctions.AddDays(g2g.Data_Modifica, -1) < pt.Data_Modifica
            Select pe
            ).ToList()

            rval.Recode.G2GRecodePua_EffluenteToDelete = (
            From r In giasContext.G2G_Recode_PUA_Effluente
            Where r.From_PivaSuperUser = psu _
                AndAlso Not giasContext.PUA_Effluente.Any(Function(pt) pt.Piva_SuperUser = r.From_PivaSuperUser And pt.ID = r.From_ID) _
                AndAlso r.To_PivaSuperUser = PivaSuperUser_Destinazione
            Select r).ToList()

            rval.pua_effluente_insert = listaInsert
            rval.pua_effluente_update = listaUpdate


        End Using

        Return rval

    End Function

    Public Function LeggiPerGias2GiasReverse(ByVal piva As String,
                                      ByVal piva_destinazione As String,
                                      ByVal PivaSuperUser_Destinazione As String,
                                      ByVal objOpzioniImportImpresa As clsImpresa,
                                      ByRef objParametri_server As AgronicaCoreParametri) As G2G_Pua_Reverse

        Dim gefutils As New AgronicaCoreEntityFramework.Gias_EF_Utility
        Dim efConnString As String = gefutils.GetEntityConnectionString(objParametri_server.StringaConnessione)

        Dim rval As New G2G_Pua_Reverse With {
            .From_Piva = piva,
            .From_PivaSuperUser = objParametri_server.PivaSuperUser,
            .Recode = G2GUtility.Nuovo_G2G_Recode(),
            .To_Piva = piva_destinazione
        }

        Dim psu As String = objParametri_server.PivaSuperUser

        Using giasContext As New AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities(efConnString)
            giasContext.Database.CommandTimeout = 3600
            'PUA_TESTATA
            rval.pua_testata_insert = (
            From pt In giasContext.PUA_Testata
            Where pt.Piva = piva _
                AndAlso pt.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_pua _
                AndAlso Not giasContext.G2G_Recode_Pua.Any(Function(g) g.To_PivaSuperUser = pt.Piva_SuperUser And g.From_PivaSuperUser = PivaSuperUser_Destinazione And g.To_Piva = pt.Piva And g.To_Pua_Cod = pt.PUA_Cod And g.To_Regolamento_Cod = pt.Regolamento_Cod)
            Select pt).ToList()


            rval.pua_testata_update = (
            From pt In giasContext.PUA_Testata
            Join g2g In giasContext.G2G_Recode_Pua
                On pt.Piva_SuperUser Equals g2g.To_PivaSuperUser _
                And pt.Piva Equals g2g.To_Piva _
                And pt.PUA_Cod Equals g2g.To_Pua_Cod _
                And pt.Regolamento_Cod Equals g2g.To_Regolamento_Cod
            Where pt.Piva = piva _
                AndAlso g2g.From_PivaSuperUser = PivaSuperUser_Destinazione _
                AndAlso System.Data.Entity.DbFunctions.AddDays(g2g.Data_Modifica, -1) < pt.Data_Modifica
            Select pt
            ).ToList()

            rval.Recode.G2GRecodePuaToDelete = (
            From r In giasContext.G2G_Recode_Pua
            Where r.To_PivaSuperUser = psu _
                AndAlso r.From_Piva = piva _
                AndAlso Not giasContext.PUA_Testata.Any(Function(pt) pt.Piva_SuperUser = r.To_PivaSuperUser And pt.Piva = r.To_Piva And pt.PUA_Cod = r.To_Pua_Cod And pt.Regolamento_Cod = r.To_Regolamento_Cod) _
                AndAlso r.From_PivaSuperUser = PivaSuperUser_Destinazione
            Select r).ToList()

            Dim listaInsert As List(Of PUA_Effluente) = (
            From pe In giasContext.PUA_Effluente
            Join pt In giasContext.PUA_Testata
                    On pt.Piva_SuperUser Equals pe.Piva_SuperUser _
                    And pt.PUA_Cod Equals pe.PUA_Cod _
                    And pt.Regolamento_Cod Equals pe.Regolamento_Cod
            Where pt.Piva = piva AndAlso Not giasContext.G2G_Recode_PUA_Effluente.Any(Function(g) g.To_PivaSuperUser = pt.Piva_SuperUser And g.From_PivaSuperUser = PivaSuperUser_Destinazione And g.To_ID = pe.ID)
            Select pe).ToList()

            Dim listaUpdate As List(Of PUA_Effluente) = (
            From pe In giasContext.PUA_Effluente
            Join pt In giasContext.PUA_Testata
                On pt.Piva_SuperUser Equals pe.Piva_SuperUser _
                And pt.PUA_Cod Equals pe.PUA_Cod _
                And pt.Regolamento_Cod Equals pe.Regolamento_Cod
            Join g2g In giasContext.G2G_Recode_PUA_Effluente
                On pt.Piva_SuperUser Equals g2g.To_PivaSuperUser _
                And pe.ID Equals g2g.To_ID
            Where pt.Piva = piva _
                AndAlso g2g.From_PivaSuperUser = PivaSuperUser_Destinazione _
                AndAlso System.Data.Entity.DbFunctions.AddDays(g2g.Data_Modifica, -1) < pt.Data_Modifica
            Select pe
            ).ToList()

            rval.Recode.G2GRecodePua_EffluenteToDelete = (
            From r In giasContext.G2G_Recode_PUA_Effluente
            Where r.To_PivaSuperUser = psu _
                AndAlso Not giasContext.PUA_Effluente.Any(Function(pt) pt.Piva_SuperUser = r.To_PivaSuperUser And pt.ID = r.To_ID) _
                AndAlso r.From_PivaSuperUser = PivaSuperUser_Destinazione
            Select r).ToList()


            rval.pua_effluente_insert = listaInsert
            rval.pua_effluente_update = listaUpdate

        End Using

        Return rval

    End Function

End Class

Public Class G2GPUA_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi_Pua_G2G(Origine_Piva_SuperUser As String, Destinazione_Piva_SuperUser As String, ByRef g2g As G2G_Pua, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GPUA_W.Scrivi_Pua_G2G()"

        Dim username = objParametri.UsernameOperazione
        Dim To_piva = g2g.To_Piva
        Dim From_Piva = g2g.From_Piva
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

                    'PUA_TESTATA - INSERT
                    Try

                        For Each pt_curr As PUA_Testata In g2g.pua_testata_insert

                            'Richiedo un nuovo id sequenza
                            Dim idSeq As Integer = objSequenze.NuovoId_Tabella("Pua_Testata", 0, 2000000000, objParametri, UtilizzaTransazione:=False)

                            Dim pt_2Add = Gias_EF_Utility.CopyEntity(GiasContext, pt_curr, Nothing, username, data)
                            pt_2Add.Piva = To_piva
                            pt_2Add.PUA_Cod = idSeq
                            pt_2Add.Piva_SuperUser = Destinazione_Piva_SuperUser
                            GiasContext.PUA_Testata.Add(pt_2Add)

                            Dim recode =
                                New G2G_Recode_Pua With {
                                    .From_PivaSuperUser = Origine_Piva_SuperUser,
                                    .To_PivaSuperUser = Destinazione_Piva_SuperUser,
                                    .From_Piva = pt_curr.Piva,
                                    .To_Piva = To_piva,
                                    .From_Pua_Cod = pt_curr.PUA_Cod,
                                    .To_Pua_Cod = idSeq,
                                    .From_Regolamento_Cod = pt_curr.Regolamento_Cod,
                                    .To_Regolamento_Cod = pt_curr.Regolamento_Cod,
                                    .Username_Creazione = username,
                                    .Username_Modifica = username,
                                    .Data_Creazione = Now(),
                                    .Data_Modifica = Now(),
                                    .Validita_Inizio = AGRODATAINIZIO,
                                    .Validita_Fine = AGRODATAFINE,
                                    .inviato = "0",
                                    .datainvio = Now()
                                }
                            g2g.Recode.G2GRecodePuaToInsert.Add(recode)
                            GiasContext.G2G_Recode_Pua.Add(recode)

                        Next

                        GiasContext.SaveChanges()

                    Catch ex As Exception
                        Throw New Exception("Err durante il salvataggio di PUAT: " & ex.Message)
                    End Try

                    'PUA_TESTATA - UPDATE
                    Try

                        For Each pt_curr As PUA_Testata In g2g.pua_testata_update

                            'Aggiorno i recode per gli elementi aggiornati
                            Dim recode = (From rr In GiasContext.G2G_Recode_Pua Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_Piva = pt_curr.Piva AndAlso rr.From_Pua_Cod = pt_curr.PUA_Cod AndAlso rr.From_Regolamento_Cod = pt_curr.Regolamento_Cod).First()
                            recode.Username_Modifica = username
                            recode.Data_Modifica = data
                            recode.datainvio = data
                            g2g.Recode.G2GRecodePuaToUpdate.Add(recode)
                            GiasContext.G2G_Recode_Pua.Attach(recode)
                            GiasContext.Entry(recode).State = EntityState.Modified

                            'Modifico le Testate
                            Dim pt_2Upd = (From x In GiasContext.PUA_Testata Where x.Piva_SuperUser = recode.To_PivaSuperUser AndAlso x.Piva = recode.To_Piva AndAlso x.PUA_Cod = recode.To_Pua_Cod AndAlso x.Regolamento_Cod = recode.To_Regolamento_Cod).First()
                            pt_2Upd = Gias_EF_Utility.CopyEntity(GiasContext, pt_curr, pt_2Upd, username, data)
                            pt_2Upd.Piva_SuperUser = Destinazione_Piva_SuperUser
                            pt_2Upd.Piva = To_piva
                            pt_2Upd.PUA_Cod = recode.To_Pua_Cod
                            GiasContext.PUA_Testata.Attach(pt_2Upd)
                            GiasContext.Entry(pt_2Upd).State = EntityState.Modified

                        Next

                        GiasContext.SaveChanges()

                    Catch ex As Exception
                        Throw New Exception("Err durante la modifica di PUAT (+ del PUAE): " & ex.Message)
                    End Try

                    'PUA_EFFLUENTE - INSERT
                    Try

                        For Each pe_curr As PUA_Effluente In g2g.pua_effluente_insert

                            'Richiedo un nuovo id sequenza
                            Dim idSeq As Integer = objSequenze.NuovoId_Tabella("PUA_Effluente", 0, 2000000000, objParametri, UtilizzaTransazione:=False)

                            Dim recodePua = (From rr In GiasContext.G2G_Recode_Pua Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_Pua_Cod = pe_curr.PUA_Cod AndAlso rr.From_Regolamento_Cod = pe_curr.Regolamento_Cod).First()
                            Dim pe_2Add = Gias_EF_Utility.CopyEntity(GiasContext, pe_curr, Nothing, username, data)
                            pe_2Add.PUA_Cod = recodePua.To_Pua_Cod
                            pe_2Add.ID = idSeq
                            pe_2Add.Piva_SuperUser = Destinazione_Piva_SuperUser
                            GiasContext.PUA_Effluente.Add(pe_2Add)

                            GiasContext.SaveChanges()

                            Dim recode =
                                New G2G_Recode_PUA_Effluente With {
                                    .From_PivaSuperUser = Origine_Piva_SuperUser,
                                    .To_PivaSuperUser = Destinazione_Piva_SuperUser,
                                    .From_ID = pe_curr.ID,
                                    .To_ID = pe_2Add.ID,
                                    .Username_Creazione = username,
                                    .Username_Modifica = username,
                                    .Data_Creazione = Now(),
                                    .Data_Modifica = Now(),
                                    .Validita_Inizio = AGRODATAINIZIO,
                                    .Validita_Fine = AGRODATAFINE,
                                    .inviato = "0",
                                    .datainvio = Now()
                                }
                            g2g.Recode.G2GRecodePua_EffluenteToInsert.Add(recode)
                            GiasContext.G2G_Recode_PUA_Effluente.Add(recode)

                        Next

                        GiasContext.SaveChanges()

                    Catch ex As Exception
                        Dim msg = ex.Message
                        If ex.InnerException IsNot Nothing Then
                            msg &= " InnerException:" & ex.InnerException.Message
                        End If
                        Throw New Exception("Err durante il salvataggio di PUAE 3: " & msg)
                    End Try


                    'PUA_EFFLUENTE - UPDATE
                    Try

                        For Each pe_curr As PUA_Effluente In g2g.pua_effluente_update

                            'Aggiorno i recode per gli elementi aggiornati
                            Dim recode = (From rr In GiasContext.G2G_Recode_PUA_Effluente Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_ID = pe_curr.ID).First()
                            recode.Username_Modifica = username
                            recode.Data_Modifica = data
                            recode.datainvio = data
                            g2g.Recode.G2GRecodePua_EffluenteToUpdate.Add(recode)
                            GiasContext.G2G_Recode_PUA_Effluente.Attach(recode)
                            GiasContext.Entry(recode).State = EntityState.Modified

                            'Modifico le Testate
                            Dim pt_2Upd = (From x In GiasContext.PUA_Effluente Where x.Piva_SuperUser = recode.To_PivaSuperUser AndAlso x.ID = recode.To_ID).First()
                            pt_2Upd = Gias_EF_Utility.CopyEntity(GiasContext, pe_curr, pt_2Upd, username, data)
                            pt_2Upd.Piva_SuperUser = Destinazione_Piva_SuperUser
                            pt_2Upd.PUA_Cod = (From rr In GiasContext.G2G_Recode_Pua Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_Piva = From_Piva And rr.From_Pua_Cod = pt_2Upd.PUA_Cod And rr.From_Regolamento_Cod = pt_2Upd.Regolamento_Cod Select rr.To_Pua_Cod).First()
                            pt_2Upd.ID = recode.To_ID
                            GiasContext.PUA_Effluente.Attach(pt_2Upd)
                            GiasContext.Entry(pt_2Upd).State = EntityState.Modified

                        Next

                        GiasContext.SaveChanges()

                    Catch ex As Exception
                        Dim msg = ex.Message
                        If ex.InnerException IsNot Nothing Then
                            msg &= " InnerException:" & ex.InnerException.Message
                        End If
                        Throw New Exception("Err durante il salvataggio di PUAE 1: " & msg)
                    End Try

                    'PUA_TESTATA + PUA_EFFLUENTI - CANCELLAZIONE
                    Try

                        For Each recode As G2G_Recode_Pua In g2g.Recode.G2GRecodePuaToDelete

                            'Cancello la Testata
                            Dim pua = (From pt In GiasContext.PUA_Testata Where pt.Piva_SuperUser = Destinazione_Piva_SuperUser AndAlso recode.To_Piva = pt.Piva AndAlso recode.To_Pua_Cod = pt.PUA_Cod AndAlso recode.To_Regolamento_Cod = pt.Regolamento_Cod).First()
                            GiasContext.PUA_Testata.Attach(pua)
                            GiasContext.PUA_Testata.Remove(pua)

                            ' Cancello il recode corrispondente
                            Dim recode2Delete = (From rr In GiasContext.G2G_Recode_Pua
                                                 Where rr.From_PivaSuperUser = recode.From_PivaSuperUser AndAlso rr.From_Piva = recode.From_Piva AndAlso rr.From_Pua_Cod = recode.From_Pua_Cod AndAlso rr.From_Regolamento_Cod = recode.From_Regolamento_Cod _
                                                 AndAlso rr.To_PivaSuperUser = recode.To_PivaSuperUser AndAlso rr.To_Piva = recode.To_Piva AndAlso rr.To_Pua_Cod = recode.To_Pua_Cod AndAlso rr.To_Regolamento_Cod = recode.To_Regolamento_Cod
                                                 Select rr).First()
                            GiasContext.G2G_Recode_Pua.Attach(recode2Delete)
                            GiasContext.G2G_Recode_Pua.Remove(recode2Delete)

                            'Cancello gli Effluenti 
                            Dim pe_2Del_list = (From pe In GiasContext.PUA_Effluente Where pe.Piva_SuperUser = Destinazione_Piva_SuperUser AndAlso pe.PUA_Cod = recode.To_Pua_Cod AndAlso pe.Regolamento_Cod = recode.To_Regolamento_Cod).ToList()
                            For Each pe_2Del In pe_2Del_list
                                GiasContext.PUA_Effluente.Attach(pe_2Del)
                                GiasContext.PUA_Effluente.Remove(pe_2Del)
                            Next

                        Next

                        GiasContext.SaveChanges()

                    Catch ex As Exception
                        Throw New Exception("Err durante la cancellazione di PUAT e PUAE: " & ex.Message)
                    End Try

                    'PUA_EFFLUENTI - CANCELLAZIONE
                    Try

                        For Each recode As G2G_Recode_PUA_Effluente In g2g.Recode.G2GRecodePua_EffluenteToDelete

                            'Cancello la Testata
                            Dim pua = (From pt In GiasContext.PUA_Effluente Where pt.Piva_SuperUser = Destinazione_Piva_SuperUser AndAlso recode.To_ID = pt.ID).First()
                            If pua IsNot Nothing Then

                                Dim testata = (From pt In GiasContext.PUA_Testata Where pt.PUA_Cod = pua.PUA_Cod AndAlso pt.Piva = To_piva).FirstOrDefault
                                Dim elimina As Boolean = False

                                If testata Is Nothing Then
                                    elimina = True
                                End If

                                If testata IsNot Nothing AndAlso testata.Piva = To_piva Then
                                    elimina = True
                                End If

                                If elimina Then
                                    GiasContext.PUA_Effluente.Attach(pua)
                                    GiasContext.PUA_Effluente.Remove(pua)

                                    ' Cancello il recode corrispondente
                                    Dim recode2Delete = (From rr In GiasContext.G2G_Recode_PUA_Effluente
                                                         Where rr.From_PivaSuperUser = recode.From_PivaSuperUser AndAlso rr.From_ID = recode.From_ID _
                                                         AndAlso rr.To_PivaSuperUser = recode.To_PivaSuperUser AndAlso rr.To_ID = recode.To_ID
                                                         Select rr).First()
                                    GiasContext.G2G_Recode_PUA_Effluente.Attach(recode2Delete)
                                    GiasContext.G2G_Recode_PUA_Effluente.Remove(recode2Delete)
                                End If


                            End If

                        Next

                        GiasContext.SaveChanges()

                    Catch ex As Exception
                        Throw New Exception("Err durante la cancellazione di PUAT e PUAE: " & ex.Message)
                    End Try

                    ' COMMIT Effettivo
                    scope.Complete()

                End Using
            End Using

        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message, ex)
        End Try

        Return g2g.Recode

    End Function

    Public Function Scrivi_Pua_G2GReverse(Origine_Piva_SuperUser As String, Destinazione_Piva_SuperUser As String, ByRef g2g As G2G_Pua_Reverse, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GPUA_W.Scrivi_Pua_G2GReverse()"

        Dim username = objParametri.UsernameOperazione
        Dim piva = g2g.To_Piva
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

                    'PUA_TESTATA - INSERT
                    Try

                        For Each pt_curr As PUA_Testata In g2g.pua_testata_insert

                            'Richiedo un nuovo id sequenza
                            Dim idSeq As Integer = objSequenze.NuovoId_Tabella("Pua_Testata", 0, 2000000000, objParametri, UtilizzaTransazione:=False)

                            Dim pt_2Add = Gias_EF_Utility.CopyEntity(GiasContext, pt_curr, Nothing, username, data)
                            pt_2Add.Piva = piva
                            pt_2Add.PUA_Cod = idSeq
                            pt_2Add.Piva_SuperUser = Destinazione_Piva_SuperUser
                            GiasContext.PUA_Testata.Add(pt_2Add)

                            Dim recode =
                                New G2G_Recode_Pua With {
                                    .From_PivaSuperUser = Destinazione_Piva_SuperUser,
                                    .To_PivaSuperUser = Origine_Piva_SuperUser,
                                    .From_Piva = piva,
                                    .To_Piva = pt_curr.Piva,
                                    .From_Pua_Cod = idSeq,
                                    .To_Pua_Cod = pt_curr.PUA_Cod,
                                    .From_Regolamento_Cod = pt_curr.Regolamento_Cod,
                                    .To_Regolamento_Cod = pt_curr.Regolamento_Cod,
                                    .Username_Creazione = username,
                                    .Username_Modifica = username,
                                    .Data_Creazione = Now(),
                                    .Data_Modifica = Now(),
                                    .Validita_Inizio = AGRODATAINIZIO,
                                    .Validita_Fine = AGRODATAFINE,
                                    .inviato = "0",
                                    .datainvio = Now()
                                }
                            g2g.Recode.G2GRecodePuaToInsert.Add(recode)
                            GiasContext.G2G_Recode_Pua.Add(recode)

                        Next

                        GiasContext.SaveChanges()

                    Catch ex As Exception
                        Throw New Exception("Err durante il salvataggio di PUAT: " & ex.Message)
                    End Try

                    'PUA_TESTATA - UPDATE
                    Try

                        For Each pt_curr As PUA_Testata In g2g.pua_testata_update

                            'Aggiorno i recode per gli elementi aggiornati
                            Dim recode = (From rr In GiasContext.G2G_Recode_Pua Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.To_Piva = pt_curr.Piva AndAlso rr.To_Pua_Cod = pt_curr.PUA_Cod AndAlso rr.To_Regolamento_Cod = pt_curr.Regolamento_Cod).First()
                            recode.Username_Modifica = username
                            recode.Data_Modifica = data
                            recode.datainvio = data
                            g2g.Recode.G2GRecodePuaToUpdate.Add(recode)
                            GiasContext.G2G_Recode_Pua.Attach(recode)
                            GiasContext.Entry(recode).State = EntityState.Modified

                            'Modfico le Testate
                            Dim pt_2Upd = (From x In GiasContext.PUA_Testata Where x.Piva_SuperUser = recode.From_PivaSuperUser AndAlso x.Piva = recode.From_Piva AndAlso x.PUA_Cod = recode.From_Pua_Cod AndAlso x.Regolamento_Cod = recode.From_Regolamento_Cod).First()
                            pt_2Upd = Gias_EF_Utility.CopyEntity(GiasContext, pt_curr, pt_2Upd, username, data)
                            pt_2Upd.Piva_SuperUser = Destinazione_Piva_SuperUser
                            pt_2Upd.Piva = piva
                            pt_2Upd.PUA_Cod = recode.From_Pua_Cod
                            GiasContext.PUA_Testata.Attach(pt_2Upd)
                            GiasContext.Entry(pt_2Upd).State = EntityState.Modified

                        Next

                        GiasContext.SaveChanges()

                    Catch ex As Exception
                        Throw New Exception("Err durante la modifica di PUAT (+ del PUAE): " & ex.Message)
                    End Try

                    'PUA_EFFLUENTE - INSERT
                    Try
                        For Each pe_curr As PUA_Effluente In g2g.pua_effluente_insert

                            'Richiedo un nuovo id sequenza
                            Dim idSeq As Integer = objSequenze.NuovoId_Tabella("PUA_Effluente", 0, 2000000000, objParametri, UtilizzaTransazione:=False)

                            'Recupero il recode di G2G_Recode_Pua perché ho bisogno di ricavare il Pua_Cod
                            Dim recodePua = (From rr In GiasContext.G2G_Recode_Pua Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.To_Pua_Cod = pe_curr.PUA_Cod AndAlso rr.To_Regolamento_Cod = pe_curr.Regolamento_Cod).First()
                            Dim pe_2Add = Gias_EF_Utility.CopyEntity(GiasContext, pe_curr, Nothing, username, data)
                            pe_2Add.PUA_Cod = recodePua.From_Pua_Cod
                            pe_2Add.ID = idSeq
                            pe_2Add.Piva_SuperUser = Destinazione_Piva_SuperUser
                            GiasContext.PUA_Effluente.Add(pe_2Add)


                            Dim recode =
                                New G2G_Recode_PUA_Effluente With {
                                    .From_PivaSuperUser = Destinazione_Piva_SuperUser,
                                    .To_PivaSuperUser = Origine_Piva_SuperUser,
                                    .From_ID = pe_2Add.ID,
                                    .To_ID = pe_curr.ID,
                                    .Username_Creazione = username,
                                    .Username_Modifica = username,
                                    .Data_Creazione = Now(),
                                    .Data_Modifica = Now(),
                                    .Validita_Inizio = AGRODATAINIZIO,
                                    .Validita_Fine = AGRODATAFINE,
                                    .inviato = "0",
                                    .datainvio = Now()
                                }
                            g2g.Recode.G2GRecodePua_EffluenteToInsert.Add(recode)
                            GiasContext.G2G_Recode_PUA_Effluente.Add(recode)

                        Next

                        GiasContext.SaveChanges()

                    Catch ex As Exception
                        Dim msg = ex.Message
                        If ex.InnerException IsNot Nothing Then
                            msg &= " InnerException:" & ex.InnerException.Message
                        End If
                        Throw New Exception("Err durante il salvataggio di PUAE 2: " & msg)
                    End Try

                    'PUA_EFFLUENTE - UPDATE
                    Try

                        For Each pe_curr As PUA_Effluente In g2g.pua_effluente_update

                            Dim recode = (From rr In GiasContext.G2G_Recode_PUA_Effluente Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.To_PivaSuperUser = pe_curr.Piva_SuperUser AndAlso rr.To_ID = pe_curr.ID).First()
                            recode.Username_Modifica = username
                            recode.Data_Modifica = data
                            recode.datainvio = data
                            g2g.Recode.G2GRecodePua_EffluenteToUpdate.Add(recode)
                            GiasContext.G2G_Recode_PUA_Effluente.Attach(recode)
                            GiasContext.Entry(recode).State = EntityState.Modified


                            Dim pt_2Upd = (From x In GiasContext.PUA_Effluente Where x.Piva_SuperUser = recode.From_PivaSuperUser AndAlso x.ID = recode.From_ID).First()
                            Dim temp_PuaCod = pt_2Upd.PUA_Cod
                            Gias_EF_Utility.CopyEntity(GiasContext, pe_curr, pt_2Upd, username, data)
                            pt_2Upd.Piva_SuperUser = recode.From_PivaSuperUser
                            pt_2Upd.ID = recode.From_ID
                            pt_2Upd.PUA_Cod = temp_PuaCod
                            GiasContext.PUA_Effluente.Attach(pt_2Upd)
                            GiasContext.Entry(pt_2Upd).State = EntityState.Modified

                        Next

                        GiasContext.SaveChanges()

                    Catch ex As Exception
                        Dim msg = ex.Message
                        If ex.InnerException IsNot Nothing Then
                            msg &= " InnerException:" & ex.InnerException.Message
                        End If
                        Throw New Exception("Err durante il salvataggio di PUAE 1: " & msg)
                    End Try

                    'PUA_TESTATA + PUA_EFFLUENTI - CANCELLAZIONE
                    Try
                        For Each recode As G2G_Recode_Pua In g2g.Recode.G2GRecodePuaToDelete

                            'Cancello la Testata
                            Dim pua = (From pt In GiasContext.PUA_Testata Where pt.Piva_SuperUser = Destinazione_Piva_SuperUser AndAlso recode.From_Piva = pt.Piva AndAlso recode.From_Pua_Cod = pt.PUA_Cod AndAlso recode.From_Regolamento_Cod = pt.Regolamento_Cod).First()
                            GiasContext.PUA_Testata.Attach(pua)
                            GiasContext.PUA_Testata.Remove(pua)

                            ' Cancello il recode corrispondente
                            Dim recode2Delete = (From rr In GiasContext.G2G_Recode_Pua
                                                 Where rr.To_PivaSuperUser = recode.To_PivaSuperUser AndAlso rr.To_Piva = recode.To_Piva AndAlso rr.To_Pua_Cod = recode.To_Pua_Cod AndAlso rr.To_Regolamento_Cod = recode.To_Regolamento_Cod _
                                                 AndAlso rr.From_PivaSuperUser = recode.From_PivaSuperUser AndAlso rr.From_Piva = recode.From_Piva AndAlso rr.From_Pua_Cod = recode.From_Pua_Cod AndAlso rr.From_Regolamento_Cod = recode.From_Regolamento_Cod
                                                 Select rr).First()
                            GiasContext.G2G_Recode_Pua.Attach(recode2Delete)
                            GiasContext.G2G_Recode_Pua.Remove(recode2Delete)

                            'Cancello gli Effluenti 
                            Dim pe_2Del_list = (From pe In GiasContext.PUA_Effluente Where pe.Piva_SuperUser = Destinazione_Piva_SuperUser AndAlso pe.PUA_Cod = recode.From_Pua_Cod AndAlso pe.Regolamento_Cod = recode.From_Regolamento_Cod).ToList()
                            For Each pe_2Del In pe_2Del_list
                                GiasContext.PUA_Effluente.Attach(pe_2Del)
                                GiasContext.PUA_Effluente.Remove(pe_2Del)
                            Next

                        Next

                        GiasContext.SaveChanges()

                    Catch ex As Exception
                        Throw New Exception("Err durante la cancellazione di PUAT e PUAE: " & ex.Message)
                    End Try


                    'PUA_EFFLUENTI - CANCELLAZIONE
                    Try
                        For Each recode As G2G_Recode_PUA_Effluente In g2g.Recode.G2GRecodePua_EffluenteToDelete

                            'Cancello la Testata
                            Dim pua = (From pt In GiasContext.PUA_Effluente Where pt.Piva_SuperUser = Destinazione_Piva_SuperUser AndAlso recode.From_ID = pt.ID).First()
                            GiasContext.PUA_Effluente.Attach(pua)
                            GiasContext.PUA_Effluente.Remove(pua)

                            ' Cancello il recode corrispondente
                            Dim recode2Delete = (From rr In GiasContext.G2G_Recode_PUA_Effluente
                                                 Where rr.To_PivaSuperUser = recode.To_PivaSuperUser AndAlso rr.To_ID = recode.To_ID _
                                                 AndAlso rr.From_PivaSuperUser = recode.From_PivaSuperUser AndAlso rr.From_ID = recode.From_ID
                                                 Select rr).First()
                            GiasContext.G2G_Recode_PUA_Effluente.Attach(recode2Delete)
                            GiasContext.G2G_Recode_PUA_Effluente.Remove(recode2Delete)

                        Next

                        GiasContext.SaveChanges()

                    Catch ex As Exception
                        Throw New Exception("Err durante la cancellazione di PUAT e PUAE: " & ex.Message)
                    End Try


                    ' COMMIT Effettivo
                    scope.Complete()

                End Using
            End Using

        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message, ex)
        End Try

        Return g2g.Recode

    End Function

End Class
