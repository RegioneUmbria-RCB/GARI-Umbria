Imports System.Data.Entity
Imports System.Transactions
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModello

Public Class G2GMateriePrimeCampionature_R
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function Leggi_MateriePrimeCampionature_G2G(ByVal Piva As String, ByVal PivaSuperUser_Destinazione As String, ByVal listaImprese As List(Of String), DataValidita As Date, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_MateriePrimeCampionature

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GMateriePrimeCampionature_R.Leggi_MateriePrimeCampionature_G2G()"
        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Dim rval As New G2G_MateriePrimeCampionature

        'quanto sicuramente non assegnato ...
        With rval
            .From_Piva = Piva
            .From_PivaSuperUser = objParametri.PivaSuperUser
        End With

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
            GiasContext.Database.CommandTimeout = 3600
            rval.MateriePrimeCampionatureToInsert = (
                    From d In GiasContext.Movimenti_dettagli
                    Join m In GiasContext.Movimenti On d.Id_Agenda Equals m.Id_Agenda And d.PIVA Equals m.PIVA And d.Id_Mov Equals m.Id_Mov
                    Join mpc In GiasContext.Materie_Prime_Campionature On d.Cal_Cod Equals mpc.Progressivo
                    Where m.Data_Movimento >= DataValidita _
                        AndAlso (listaImprese.Count = 0 Or listaImprese.Contains(m.PIVA)) _
                        AndAlso d.Cal_Cod < 0 _
                        AndAlso Not GiasContext.G2G_Recode_Materie_Prime_Campionature.Any(Function(g) g.From_PivaSuperUser = rval.From_PivaSuperUser And g.To_PivaSuperUser = PivaSuperUser_Destinazione _
                                                                                                        And g.From_Progressivo = mpc.Progressivo _
                                                                                                        And g.From_Tipo = mpc.Tipo _
                                                                                                        And g.From_Tipo_Cod = mpc.Tipo_Cod)
                    Select mpc).Distinct().ToList()

            rval.MateriePrimeCampionatureToUpdate = (
                    From mpc In GiasContext.Materie_Prime_Campionature
                    Join m In GiasContext.Movimenti_dettagli On mpc.Progressivo Equals m.Cal_Cod
                    Join g2g In GiasContext.G2G_Recode_Materie_Prime_Campionature
                        On mpc.Progressivo Equals g2g.From_Progressivo _
                        And mpc.Tipo Equals g2g.From_Tipo _
                        And mpc.Tipo_Cod Equals g2g.From_Tipo_Cod
                    Where g2g.Data_Modifica < mpc.Data_Modifica _
                        AndAlso (listaImprese.Count = 0 Or listaImprese.Contains(m.PIVA))
                    Select mpc).Distinct().ToList()

            rval.G2G_Recode_Materie_Prime_Campionature_Delete = (
                        From r In GiasContext.G2G_Recode_Materie_Prime_Campionature
                        Where Not GiasContext.Materie_Prime_Campionature.Any(Function(p) p.Progressivo = r.To_Progressivo _
                                                                            And p.Tipo = r.To_Tipo _
                                                                            And p.Tipo_Cod = r.To_Tipo_Cod) _
                            AndAlso r.To_PivaSuperUser = rval.To_PivaSuperUser _
                            AndAlso r.From_PivaSuperUser = rval.From_PivaSuperUser
                        Select r
                    ).Distinct.ToList

        End Using

        Return rval

    End Function

    Public Function Leggi_MateriePrimeCampionature_G2GReverse(ByVal Piva As String, ByVal PivaSuperUser_Destinazione As String, ByVal listaImprese As List(Of String), DataValidita As Date, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_MateriePrimeCampionature_Reverse

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GMateriePrimeCampionature_R.Leggi_MateriePrimeCampionature_G2GReverse()"
        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Dim rval As New G2G_MateriePrimeCampionature_Reverse

        'quanto sicuramente non assegnato ...
        With rval
            .From_Piva = Piva
            .From_PivaSuperUser = objParametri.PivaSuperUser
        End With

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            GiasContext.Database.CommandTimeout = 3600

            rval.MateriePrimeCampionatureToInsert = (
                    From d In GiasContext.Movimenti_dettagli
                    Join m In GiasContext.Movimenti On d.Id_Agenda Equals m.Id_Agenda And d.PIVA Equals m.PIVA And d.Id_Mov Equals m.Id_Mov
                    Join mpc In GiasContext.Materie_Prime_Campionature On d.Cal_Cod Equals mpc.Progressivo
                    Where m.Data_Movimento >= DataValidita _
                        AndAlso (listaImprese.Count = 0 Or listaImprese.Contains(m.PIVA)) _
                        AndAlso d.Cal_Cod < 0 _
                        AndAlso Not GiasContext.G2G_Recode_Materie_Prime_Campionature.Any(Function(g) g.To_PivaSuperUser = rval.From_PivaSuperUser And g.From_PivaSuperUser = PivaSuperUser_Destinazione _
                                                                                                        And g.To_Progressivo = mpc.Progressivo _
                                                                                                        And g.To_Tipo = mpc.Tipo _
                                                                                                        And g.To_Tipo_Cod = mpc.Tipo_Cod)
                    Select mpc).Distinct().ToList()

            rval.MateriePrimeCampionatureToUpdate = (
                    From mpc In GiasContext.Materie_Prime_Campionature
                    Join m In GiasContext.Movimenti_dettagli On mpc.Progressivo Equals m.Cal_Cod
                    Join g2g In GiasContext.G2G_Recode_Materie_Prime_Campionature
                        On mpc.Progressivo Equals g2g.To_Progressivo _
                        And mpc.Tipo Equals g2g.To_Tipo _
                        And mpc.Tipo_Cod Equals g2g.To_Tipo_Cod
                    Where g2g.Data_Modifica < mpc.Data_Modifica _
                        AndAlso (listaImprese.Count = 0 Or listaImprese.Contains(m.PIVA))
                    Select mpc).Distinct().ToList()

            rval.G2G_Recode_Materie_Prime_Campionature_Delete = (
                        From r In GiasContext.G2G_Recode_Materie_Prime_Campionature
                        Where Not GiasContext.Materie_Prime_Campionature.Any(Function(p) p.Progressivo = r.To_Progressivo _
                                                                            And p.Tipo = r.To_Tipo _
                                                                            And p.Tipo_Cod = r.To_Tipo_Cod) _
                            AndAlso r.To_PivaSuperUser = rval.From_PivaSuperUser _
                            AndAlso r.From_PivaSuperUser = rval.To_PivaSuperUser
                        Select r
                    ).Distinct.ToList

        End Using

        Return rval

    End Function


End Class

Public Class G2GMateriePrimeCampionature_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi_MateriePrimeCampionature_G2G(Origine_Piva_SuperUser As String, Destinazione_Piva_SuperUser As String, ByRef g2g As G2G_MateriePrimeCampionature, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GMateriePrimeCampionature_W.Scrivi_MateriePrimeCampionature_G2G()"
        Dim messaggioErrore As String = ""

        Dim From_PivaSuperUser = g2g.From_PivaSuperUser
        Dim To_PivaSuperUser = g2g.To_PivaSuperUser
        Dim From_Piva = g2g.From_Piva
        Dim To_Piva = g2g.To_Piva
        Dim username = objParametri.UsernameOperazione
        Dim data = Date.Now

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
        Dim objSequenze = New AgronicaCoreDataProvider.Agro_Sequenze
        Dim success As Boolean = True

        Dim transactionOptions As New TransactionOptions()
        transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
        transactionOptions.Timeout = G2GUtility.GetTransactionTimeout(objParametri)

        Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)

            Try

                g2g.G2G_Recode_Materie_Prime_Campionature_Delete = New List(Of G2G_Recode_Materie_Prime_Campionature)
                g2g.G2G_Recode_Materie_Prime_Campionature_Insert = New List(Of G2G_Recode_Materie_Prime_Campionature)
                g2g.G2G_Recode_Materie_Prime_Campionature_Update = New List(Of G2G_Recode_Materie_Prime_Campionature)

                For Each r As G2G_Recode_Materie_Prime_Campionature In g2g.G2G_Recode_Materie_Prime_Campionature_Delete

                    ''cancella materia
                    Dim mpc = (From m In GiasContext.Materie_Prime_Campionature Where m.Progressivo = r.To_Progressivo _
                                                                                                And m.Tipo = r.To_Tipo _
                                                                                                And m.Tipo_Cod = r.To_Tipo_Cod).FirstOrDefault()
                    GiasContext.Materie_Prime_Campionature.Attach(mpc)
                    GiasContext.Materie_Prime_Campionature.Remove(mpc)

                    '' cancella recode materia
                    Dim recode = (From rr In GiasContext.G2G_Recode_Materie_Prime_Campionature Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                                   AndAlso rr.From_Progressivo = r.From_Progressivo _
                                                                                                   AndAlso rr.From_Tipo = r.From_Tipo _
                                                                                                   AndAlso rr.From_Tipo_Cod = r.From_Tipo_Cod).FirstOrDefault()
                    GiasContext.G2G_Recode_Materie_Prime_Campionature.Attach(recode)
                    GiasContext.G2G_Recode_Materie_Prime_Campionature.Remove(recode)

                    g2g.G2G_Recode_Materie_Prime_Campionature_Delete.Add(recode)

                    GiasContext.SaveChanges()

                Next

                Dim idSeq As Integer = 0
                For Each m As Materie_Prime_Campionature In g2g.MateriePrimeCampionatureToInsert

                    idSeq = objSequenze.NuovoId_Tabella_EF(GiasContext, "materie_prime_campionature", 0, 2000000000, objParametri)

                    Dim materia_prima_campionatura = Gias_EF_Utility.CopyEntity(GiasContext, m, Nothing, username, data)

                    If materia_prima_campionatura.Tipo_Cod < 0 Then
                        success = False
                        Throw New Exception("Mappatura Materia prima fallita, Tipo_Cod < 0")
                        'TODO
                    End If

                    materia_prima_campionatura.Progressivo = -idSeq

                    GiasContext.Materie_Prime_Campionature.Add(materia_prima_campionatura)
                    GiasContext.SaveChanges()


                    Dim recode =
                        New G2G_Recode_Materie_Prime_Campionature With {
                            .From_PivaSuperUser = Origine_Piva_SuperUser,
                            .To_PivaSuperUser = Destinazione_Piva_SuperUser,
                            .From_Progressivo = m.Progressivo,
                            .To_Progressivo = materia_prima_campionatura.Progressivo,
                            .From_Tipo = m.Tipo,
                            .To_Tipo = materia_prima_campionatura.Tipo,
                            .From_Tipo_Cod = m.Tipo_Cod,
                            .To_Tipo_Cod = materia_prima_campionatura.Tipo_Cod,
                            .Username_Creazione = username,
                            .Username_Modifica = username,
                            .Data_Creazione = Now(),
                            .Data_Modifica = Now(),
                            .Validita_Inizio = AGRODATAINIZIO,
                            .Validita_Fine = AGRODATAFINE,
                            .inviato = "0",
                            .datainvio = Now()
                        }
                    g2g.G2G_Recode_Materie_Prime_Campionature_Insert.Add(recode)
                    GiasContext.G2G_Recode_Materie_Prime_Campionature.Add(recode)

                    GiasContext.SaveChanges()
                    success = True

                Next

                For Each m As Materie_Prime_Campionature In g2g.MateriePrimeCampionatureToUpdate

                    Dim recode = (From rr In GiasContext.G2G_Recode_Materie_Prime_Campionature Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                                   AndAlso rr.From_Progressivo = m.Progressivo _
                                                                                                   AndAlso rr.From_Tipo = m.Tipo _
                                                                                                   AndAlso rr.From_Tipo_Cod = m.Tipo_Cod).FirstOrDefault()
                    recode.Username_Modifica = username
                    recode.Data_Modifica = data
                    recode.datainvio = data
                    g2g.G2G_Recode_Materie_Prime_Campionature_Update.Add(recode)
                    GiasContext.G2G_Recode_Materie_Prime_Campionature.Attach(recode)
                    GiasContext.Entry(recode).State = EntityState.Modified

                    Dim mpc = (From mm In GiasContext.Materie_Prime_Campionature Where mm.Progressivo = recode.To_Progressivo _
                                                                                          And mm.Tipo = recode.To_Tipo _
                                                                                          And mm.Tipo_Cod = recode.To_Tipo_Cod).FirstOrDefault()
                    mpc = Gias_EF_Utility.CopyEntity(GiasContext, m, mpc, username, data)
                    mpc.Progressivo = recode.To_Progressivo
                    mpc.Tipo = recode.To_Tipo
                    mpc.Tipo_Cod = recode.To_Tipo_Cod
                    GiasContext.Materie_Prime_Campionature.Attach(mpc)
                    GiasContext.Entry(mpc).State = EntityState.Modified

                    g2g.G2G_Recode_Materie_Prime_Campionature_Update.Add(recode)

                    GiasContext.SaveChanges()
                Next


                g2g.Recode = New G2G_Recode

                g2g.Recode.G2GRecodeMateriePrimeCampionatureToInsert = g2g.G2G_Recode_Materie_Prime_Campionature_Insert
                g2g.Recode.G2GRecodeMateriePrimeCampionatureToUpdate = g2g.G2G_Recode_Materie_Prime_Campionature_Update
                g2g.Recode.G2GRecodeMateriePrimeCampionatureToDelete = g2g.G2G_Recode_Materie_Prime_Campionature_Delete

                ' COMIT Effettivo
                scope.Complete()
                scope.Dispose()

                ' log importazione
                g2g.Recode.LogRecode = g2g.MateriePrimeCampionatureToInsert.Count & " nuove, " & g2g.MateriePrimeCampionatureToUpdate.Count & " modificate, " & g2g.G2G_Recode_Materie_Prime_Campionature_Delete.Count & " cancellate"


            Catch ex As Exception

                messaggioErrore = ex.Message
                g2g.Recode.MessaggioErrore = messaggioErrore
                Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
                'Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore, ex)
                scope.Dispose()

            Finally

                GiasContext.Dispose()

            End Try

        End Using

        Return g2g.Recode

    End Function

    Public Function Scrivi_MateriePrimeCampionature_G2GReverse(Origine_Piva_SuperUser As String, Destinazione_Piva_SuperUser As String, ByRef g2g As G2G_MateriePrimeCampionature_Reverse, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GMateriePrimeCampionature_W.Scrivi_MateriePrimeCampionature_G2GReverse()"
        Dim messaggioErrore As String = ""

        Dim From_PivaSuperUser = g2g.From_PivaSuperUser
        Dim To_PivaSuperUser = g2g.To_PivaSuperUser
        Dim From_Piva = g2g.From_Piva
        Dim To_Piva = g2g.To_Piva
        Dim username = objParametri.UsernameOperazione
        Dim data = Date.Now

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
        Dim objSequenze = New AgronicaCoreDataProvider.Agro_Sequenze
        Dim success As Boolean = True

        Dim transactionOptions As New TransactionOptions()
        transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
        transactionOptions.Timeout = G2GUtility.GetTransactionTimeout(objParametri)

        Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)

            Try

                g2g.G2G_Recode_Materie_Prime_Campionature_Delete = New List(Of G2G_Recode_Materie_Prime_Campionature)
                g2g.G2G_Recode_Materie_Prime_Campionature_Insert = New List(Of G2G_Recode_Materie_Prime_Campionature)
                g2g.G2G_Recode_Materie_Prime_Campionature_Update = New List(Of G2G_Recode_Materie_Prime_Campionature)

                For Each r As G2G_Recode_Materie_Prime_Campionature In g2g.G2G_Recode_Materie_Prime_Campionature_Delete

                    ''cancella materia
                    Dim mpc = (From m In GiasContext.Materie_Prime_Campionature Where m.Progressivo = r.From_Progressivo _
                                                                                                And m.Tipo = r.From_Tipo _
                                                                                                And m.Tipo_Cod = r.From_Tipo_Cod).FirstOrDefault()
                    GiasContext.Materie_Prime_Campionature.Attach(mpc)
                    GiasContext.Materie_Prime_Campionature.Remove(mpc)

                    '' cancella recode materia
                    Dim recode = (From rr In GiasContext.G2G_Recode_Materie_Prime_Campionature Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                                   AndAlso rr.To_Progressivo = r.To_Progressivo _
                                                                                                   AndAlso rr.To_Tipo = r.To_Tipo _
                                                                                                   AndAlso rr.To_Tipo_Cod = r.To_Tipo_Cod).FirstOrDefault()
                    GiasContext.G2G_Recode_Materie_Prime_Campionature.Attach(recode)
                    GiasContext.G2G_Recode_Materie_Prime_Campionature.Remove(recode)

                    g2g.G2G_Recode_Materie_Prime_Campionature_Delete.Add(recode)

                    GiasContext.SaveChanges()

                Next

                Dim idSeq As Integer = 0
                For Each m As Materie_Prime_Campionature In g2g.MateriePrimeCampionatureToInsert

                    idSeq = objSequenze.NuovoId_Tabella_EF(GiasContext, "materie_prime_campionature", 0, 2000000000, objParametri)

                    Dim materia_prima_campionatura = Gias_EF_Utility.CopyEntity(GiasContext, m, Nothing, username, data)

                    If materia_prima_campionatura.Tipo_Cod < 0 Then
                        success = False
                        Throw New Exception("Mappatura Materia prima fallita, Tipo_Cod < 0")
                        'TODO
                    End If

                    materia_prima_campionatura.Progressivo = -idSeq

                    GiasContext.Materie_Prime_Campionature.Add(materia_prima_campionatura)
                    GiasContext.SaveChanges()


                    Dim recode =
                        New G2G_Recode_Materie_Prime_Campionature With {
                            .From_PivaSuperUser = Destinazione_Piva_SuperUser,
                            .To_PivaSuperUser = Origine_Piva_SuperUser,
                            .From_Progressivo = materia_prima_campionatura.Progressivo,
                            .To_Progressivo = m.Progressivo,
                            .From_Tipo = materia_prima_campionatura.Tipo,
                            .To_Tipo = m.Tipo,
                            .From_Tipo_Cod = materia_prima_campionatura.Tipo_Cod,
                            .To_Tipo_Cod = m.Tipo_Cod,
                            .Username_Creazione = username,
                            .Username_Modifica = username,
                            .Data_Creazione = Now(),
                            .Data_Modifica = Now(),
                            .Validita_Inizio = AGRODATAINIZIO,
                            .Validita_Fine = AGRODATAFINE,
                            .inviato = "0",
                            .datainvio = Now()
                        }
                    g2g.G2G_Recode_Materie_Prime_Campionature_Insert.Add(recode)
                    GiasContext.G2G_Recode_Materie_Prime_Campionature.Add(recode)

                    GiasContext.SaveChanges()
                    success = True

                Next

                For Each m As Materie_Prime_Campionature In g2g.MateriePrimeCampionatureToUpdate

                    Dim recode = (From rr In GiasContext.G2G_Recode_Materie_Prime_Campionature Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                                   AndAlso rr.To_Progressivo = m.Progressivo _
                                                                                                   AndAlso rr.To_Tipo = m.Tipo _
                                                                                                   AndAlso rr.To_Tipo_Cod = m.Tipo_Cod).FirstOrDefault()
                    recode.Username_Modifica = username
                    recode.Data_Modifica = data
                    recode.datainvio = data
                    g2g.G2G_Recode_Materie_Prime_Campionature_Update.Add(recode)
                    GiasContext.G2G_Recode_Materie_Prime_Campionature.Attach(recode)
                    GiasContext.Entry(recode).State = EntityState.Modified

                    Dim mpc = (From mm In GiasContext.Materie_Prime_Campionature Where mm.Progressivo = recode.From_Progressivo _
                                                                                          And mm.Tipo = recode.From_Tipo _
                                                                                          And mm.Tipo_Cod = recode.From_Tipo_Cod).FirstOrDefault()
                    mpc = Gias_EF_Utility.CopyEntity(GiasContext, m, mpc, username, data)
                    mpc.Progressivo = recode.From_Progressivo
                    mpc.Tipo = recode.From_Tipo
                    mpc.Tipo_Cod = recode.From_Tipo_Cod
                    GiasContext.Materie_Prime_Campionature.Attach(mpc)
                    GiasContext.Entry(mpc).State = EntityState.Modified

                    g2g.G2G_Recode_Materie_Prime_Campionature_Update.Add(recode)

                    GiasContext.SaveChanges()
                Next


                g2g.Recode = New G2G_Recode

                g2g.Recode.G2GRecodeMateriePrimeCampionatureToInsert = g2g.G2G_Recode_Materie_Prime_Campionature_Insert
                g2g.Recode.G2GRecodeMateriePrimeCampionatureToUpdate = g2g.G2G_Recode_Materie_Prime_Campionature_Update
                g2g.Recode.G2GRecodeMateriePrimeCampionatureToDelete = g2g.G2G_Recode_Materie_Prime_Campionature_Delete

                ' COMIT Effettivo
                scope.Complete()
                scope.Dispose()

                ' log importazione
                g2g.Recode.LogRecode = g2g.MateriePrimeCampionatureToInsert.Count & " nuove, " & g2g.MateriePrimeCampionatureToUpdate.Count & " modificate, " & g2g.G2G_Recode_Materie_Prime_Campionature_Delete.Count & " cancellate"


            Catch ex As Exception

                messaggioErrore = ex.Message
                g2g.Recode.MessaggioErrore = messaggioErrore
                Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
                'Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore, ex)
                scope.Dispose()

            Finally

                GiasContext.Dispose()

            End Try

        End Using

        Return g2g.Recode

    End Function

End Class
