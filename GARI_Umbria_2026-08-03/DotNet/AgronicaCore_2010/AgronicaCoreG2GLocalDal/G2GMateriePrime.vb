Imports System.Data.Entity
Imports System.Transactions
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModello

Public Class G2GMateriePrime_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Nuovo_MateriePrime_G2G(ByVal From_PivaSuperUser As String, ByVal To_PivaSuperUser As String, ByVal From_Piva As String, ByVal To_Piva As String) As G2G_MateriePrime

        Dim g2g As New G2G_MateriePrime
        With g2g
            .From_PivaSuperUser = From_PivaSuperUser
            .To_PivaSuperUser = To_PivaSuperUser
            .From_Piva = From_Piva
            .To_Piva = To_Piva
            .MateriePrimeToInsert = New List(Of Materie_Prime)
            .MateriePrimeToUpdate = New List(Of Materie_Prime)
            .MateriePrimeToDelete = New List(Of Materie_Prime)
            .Recode = G2GUtility.Nuovo_G2G_Recode()
        End With

        Return g2g

    End Function

    Public Function Nuovo_MateriePrime_G2GReverse(ByVal From_PivaSuperUser As String, ByVal To_PivaSuperUser As String, ByVal From_Piva As String, ByVal To_Piva As String) As G2G_MateriePrime_Reverse

        Dim g2g As New G2G_MateriePrime_Reverse
        With g2g
            .From_PivaSuperUser = From_PivaSuperUser
            .To_PivaSuperUser = To_PivaSuperUser
            .From_Piva = From_Piva
            .To_Piva = To_Piva
            .MateriePrimeToInsert = New List(Of Materie_Prime)
            .MateriePrimeToUpdate = New List(Of Materie_Prime)
            .MateriePrimeToDelete = New List(Of Materie_Prime)
            .Recode = G2GUtility.Nuovo_G2G_Recode()
        End With

        Return g2g

    End Function

    Public Function Leggi_MateriePrime_G2G(ByVal Piva As String, ByVal Flag_Pubblico As Boolean, ByVal listaImprese As List(Of String), DataValidita As Date, ByRef g2g As G2G_MateriePrime, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GMateriePrime_R.Leggi_MateriePrime_G2G()"
        Dim From_PivaSuperUser = g2g.From_PivaSuperUser
        Dim To_PivaSuperUser = g2g.To_PivaSuperUser

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
            GiasContext.Database.CommandTimeout = 3600
            Dim listaElemCod As New List(Of Integer)({2, 10, 200, 201, 204, 205, 210, 301, 304, 305, 306, 307, 310, 400, 401, 500, 700})

            Dim lista_mat_cod_Insert As New List(Of Integer)
            Dim lista_mat_cod_Update As New List(Of Integer)

            g2g.MateriePrimeToInsert = (
                    From d In GiasContext.Movimenti_dettagli
                    Join m In GiasContext.Movimenti On d.Id_Agenda Equals m.Id_Agenda And d.PIVA Equals m.PIVA And d.Id_Mov Equals m.Id_Mov
                    Join mp In GiasContext.Materie_Prime On d.Mat_Cod Equals mp.Mat_Cod
                    Where m.Data_Movimento >= DataValidita AndAlso (listaImprese.Count = 0 Or listaImprese.Contains(m.PIVA)) AndAlso d.Mat_Cod <> 0 AndAlso
                        listaElemCod.Contains(d.Elem_Cod) AndAlso If(Flag_Pubblico, mp.Sa_Cod = -1, mp.Piva = Piva And mp.Sa_Cod <> -1) AndAlso
                        Not GiasContext.G2G_Recode_MateriePrime.Any(Function(g) g.From_PivaSuperUser = From_PivaSuperUser AndAlso
                            g.To_PivaSuperUser = To_PivaSuperUser AndAlso g.From_Piva = mp.Piva AndAlso g.From_Mat_Cod = mp.Mat_Cod)
                    Select mp).Distinct().ToList()

            For Each materia_prima In g2g.MateriePrimeToInsert
                lista_mat_cod_Insert.Add(materia_prima.Mat_Cod)
            Next

            g2g.MateriePrimeToUpdate = (
                    From mp In GiasContext.Materie_Prime
                    Join d In GiasContext.Movimenti_dettagli On mp.Mat_Cod Equals d.Mat_Cod
                    Join g In GiasContext.G2G_Recode_MateriePrime On mp.Mat_Cod Equals g.From_Mat_Cod
                    Where g.From_PivaSuperUser = From_PivaSuperUser AndAlso g.To_PivaSuperUser = To_PivaSuperUser AndAlso g.datainvio < mp.data_modifica AndAlso
                        (listaImprese.Count = 0 Or listaImprese.Contains(d.PIVA)) AndAlso d.Mat_Cod <> 0 AndAlso
                        listaElemCod.Contains(d.Elem_Cod) AndAlso If(Flag_Pubblico, mp.Sa_Cod = -1, mp.Piva = Piva And mp.Sa_Cod <> -1)
                    Select mp).Distinct().ToList()

            For Each materia_prima In g2g.MateriePrimeToUpdate
                lista_mat_cod_Update.Add(materia_prima.Mat_Cod)
            Next

            g2g.Materie_Prime_XLingueToInsert = (From m In GiasContext.Materie_Prime_XLingue
                                                 Where lista_mat_cod_Insert.Contains(m.Mat_Cod) Or
                                                     lista_mat_cod_Update.Contains(m.Mat_Cod)).ToList

            g2g.Materie_Prime_XLingueToDelete = (From m In GiasContext.Materie_Prime_XLingue
                                                 Where lista_mat_cod_Update.Contains(m.Mat_Cod)).ToList

            'g2g.Recode.G2GRecodeMateriePrimeToDelete = (
            '            From r In GiasContext.G2G_Recode_MateriePrime
            '            Where Not GiasContext.Materie_Prime.Any(Function(p) r.From_PivaSuperUser = From_PivaSuperUser And p.Mat_Cod = r.From_Mat_Cod)
            '            Select r
            '        ).Distinct.ToList

        End Using

        Return g2g.MateriePrimeToInsert.Count > 0 OrElse g2g.MateriePrimeToUpdate.Count > 0

    End Function

    Public Function Leggi_MateriePrime_G2GReverse(ByVal Piva As String, ByVal Flag_Pubblico As Boolean, ByVal listaImprese As List(Of String), DataValidita As Date, ByRef g2g As G2G_MateriePrime_Reverse, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GMateriePrime_R.Leggi_MateriePrime_G2GReverse()"
        Dim From_PivaSuperUser = g2g.From_PivaSuperUser
        Dim To_PivaSuperUser = g2g.To_PivaSuperUser

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
            GiasContext.Database.CommandTimeout = 3600
            Dim listaElemCod As New List(Of Integer)({2, 10, 200, 201, 204, 205, 210, 301, 304, 305, 306, 307, 310, 400, 401, 500, 700})

            g2g.MateriePrimeToInsert = (
                    From d In GiasContext.Movimenti_dettagli
                    Join m In GiasContext.Movimenti On d.Id_Agenda Equals m.Id_Agenda And d.PIVA Equals m.PIVA And d.Id_Mov Equals m.Id_Mov
                    Join mp In GiasContext.Materie_Prime On d.Mat_Cod Equals mp.Mat_Cod
                    Where m.Data_Movimento >= DataValidita AndAlso (listaImprese.Count = 0 Or listaImprese.Contains(m.PIVA)) AndAlso d.Mat_Cod <> 0 AndAlso
                        listaElemCod.Contains(d.Elem_Cod) AndAlso If(Flag_Pubblico, mp.Sa_Cod = -1, mp.Piva = Piva And mp.Sa_Cod <> -1) AndAlso
                        Not GiasContext.G2G_Recode_MateriePrime.Any(Function(g) g.To_PivaSuperUser = From_PivaSuperUser AndAlso
                            g.From_PivaSuperUser = To_PivaSuperUser AndAlso g.From_Piva = mp.Piva AndAlso g.To_Mat_Cod = mp.Mat_Cod)
                    Select mp).Distinct().ToList()



            g2g.MateriePrimeToUpdate = (
                    From mp In GiasContext.Materie_Prime
                    Join d In GiasContext.Movimenti_dettagli On mp.Mat_Cod Equals d.Mat_Cod
                    Join g In GiasContext.G2G_Recode_MateriePrime On mp.Mat_Cod Equals g.From_Mat_Cod
                    Where g.To_PivaSuperUser = From_PivaSuperUser AndAlso g.From_PivaSuperUser = To_PivaSuperUser AndAlso g.datainvio < mp.data_modifica AndAlso
                        (listaImprese.Count = 0 Or listaImprese.Contains(d.PIVA)) AndAlso d.Mat_Cod <> 0 AndAlso
                        listaElemCod.Contains(d.Elem_Cod) AndAlso If(Flag_Pubblico, mp.Sa_Cod = -1, mp.Piva = Piva And mp.Sa_Cod <> -1)
                    Select mp).Distinct().ToList()

            'g2g.Recode.G2GRecodeMateriePrimeToDelete = (
            '            From r In GiasContext.G2G_Recode_MateriePrime
            '            Where Not GiasContext.Materie_Prime.Any(Function(p) r.From_PivaSuperUser = From_PivaSuperUser And p.Mat_Cod = r.From_Mat_Cod)
            '            Select r
            '        ).Distinct.ToList

        End Using

        Return g2g.MateriePrimeToInsert.Count > 0 OrElse g2g.MateriePrimeToUpdate.Count > 0

    End Function

End Class

Public Class G2GMateriePrime_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi_MateriePrime_G2G(ByRef g2g As G2G_MateriePrime, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GParcoMacchine_W.Scrivi_MateriePrime_G2G()"
        Dim messaggioErrore As String = ""

        If G2GUtility.transactionScope Then

            Dim transactionOptions As New TransactionOptions()
            transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            transactionOptions.Timeout = G2GUtility.GetTransactionTimeout(objParametri)
            Dim g2gRecode As G2G_Recode = g2g.Recode

            Try

                Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)
                    g2gRecode = Scrivi_MateriePrime(g2g, objParametri)
                    scope.Complete()
                End Using

            Catch ex As Exception

                messaggioErrore = ex.Message
                g2gRecode.MessaggioErrore = messaggioErrore
                Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)

            End Try

            Return g2gRecode

        End If

        Return Scrivi_MateriePrime(g2g, objParametri)

    End Function

    Public Function Scrivi_MateriePrime_G2GReverse(ByRef g2g As G2G_MateriePrime_Reverse, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GParcoMacchine_W.Scrivi_MateriePrime_G2GReverse()"
        Dim messaggioErrore As String = ""

        If G2GUtility.transactionScope Then

            Dim transactionOptions As New TransactionOptions()
            transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            transactionOptions.Timeout = G2GUtility.GetTransactionTimeout(objParametri)
            Dim g2gRecode As G2G_Recode = g2g.Recode

            Try

                Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)
                    g2gRecode = Scrivi_MateriePrimeReverse(g2g, objParametri)
                    scope.Complete()
                End Using

            Catch ex As Exception

                messaggioErrore = ex.Message
                g2gRecode.MessaggioErrore = messaggioErrore
                Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)

            End Try

            Return g2gRecode

        End If

        Return Scrivi_MateriePrimeReverse(g2g, objParametri)

    End Function

    Public Function Scrivi_MateriePrime(ByRef g2g As G2G_MateriePrime, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GMateriePrime_W.Scrivi_MateriePrime()"
        Dim messaggioErrore As String = ""

        Dim From_PivaSuperUser = g2g.From_PivaSuperUser
        Dim To_PivaSuperUser = g2g.To_PivaSuperUser
        Dim From_Piva = g2g.From_Piva
        Dim To_Piva = g2g.To_Piva
        Dim username = objParametri.UsernameOperazione
        Dim data = Date.Now

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try

            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                ' CANCELLA MATERIE PRIME
                If g2g.Recode.G2GRecodeMateriePrimeToDelete.Count > 0 Then

                    Dim index As Integer = 0

                    For Each r As G2G_Recode_MateriePrime In g2g.Recode.G2GRecodeMateriePrimeToDelete

                        ''cancella materia
                        Dim mp = (From m In GiasContext.Materie_Prime Where m.Mat_Cod = r.To_Mat_Cod).FirstOrDefault()
                        GiasContext.Materie_Prime.Attach(mp)
                        GiasContext.Materie_Prime.Remove(mp)

                        '' cancella recode materia
                        Dim recode = (From rr In GiasContext.G2G_Recode_MateriePrime Where rr.From_PivaSuperUser = From_PivaSuperUser AndAlso rr.To_PivaSuperUser = To_PivaSuperUser AndAlso rr.From_Mat_Cod = r.From_Mat_Cod).FirstOrDefault()
                        GiasContext.G2G_Recode_MateriePrime.Attach(recode)
                        GiasContext.G2G_Recode_MateriePrime.Remove(recode)

                        G2GUtility.SaveChanges(GiasContext, index)

                    Next

                    GiasContext.SaveChanges()

                End If


                ' INSERISCE MATERIE PRIME
                If g2g.MateriePrimeToInsert.Count > 0 Then

                    Dim index As Integer = 0

                    For Each m As Materie_Prime In g2g.MateriePrimeToInsert

                        Dim materia_prima As Materie_Prime = Nothing
                        Dim materia_prima_pubblico As Boolean = String.IsNullOrEmpty(To_Piva) OrElse m.Sa_Cod = -1
                        Dim materia_prima_piva = If(materia_prima_pubblico, m.Piva, To_Piva)

                        ' verifico se esiste già la materia prima in destinazione
                        If materia_prima_pubblico Then
                            materia_prima = (From mp In GiasContext.Materie_Prime
                                             Where mp.Piva = m.Piva _
                                               And mp.Sa_Cod = -1 _
                                               And mp.Elem_Cod = m.Elem_Cod _
                                               And mp.Sem_Cod = m.Sem_Cod _
                                               And mp.Veg_Cod = m.Veg_Cod _
                                               And mp.Cul_Cod = m.Cul_Cod _
                                               And mp.Regolamento = m.Regolamento).FirstOrDefault()
                        End If

                        If materia_prima Is Nothing Then
                            Dim idSeq = G2GUtility.NuovoId_Tabella(GiasContext, "materie_prime", 0, 2000000000, objParametri)
                            materia_prima = Gias_EF_Utility.CopyEntity(GiasContext, m, Nothing, username, data)
                            materia_prima.Mat_Cod = idSeq
                            materia_prima.Piva = materia_prima_piva
                            GiasContext.Materie_Prime.Add(materia_prima)
                        End If

                        Dim recode =
                        New G2G_Recode_MateriePrime With {
                            .From_PivaSuperUser = From_PivaSuperUser,
                            .To_PivaSuperUser = To_PivaSuperUser,
                            .From_Piva = m.Piva,
                            .To_Piva = materia_prima.Piva,
                            .From_Mat_Cod = m.Mat_Cod,
                            .To_Mat_Cod = materia_prima.Mat_Cod,
                            .Username_Creazione = username,
                            .Username_Modifica = username,
                            .Data_Creazione = Now(),
                            .Data_Modifica = Now(),
                            .Validita_Inizio = AGRODATAINIZIO,
                            .Validita_Fine = AGRODATAFINE,
                            .inviato = "0",
                            .datainvio = Now()
                        }
                        g2g.Recode.G2GRecodeMateriePrimeToInsert.Add(recode)
                        GiasContext.G2G_Recode_MateriePrime.Add(recode)

                        G2GUtility.SaveChanges(GiasContext, index)

                    Next

                    GiasContext.SaveChanges()

                End If


                ' MODIFICA MATERIE PRIME
                If g2g.MateriePrimeToUpdate.Count > 0 Then

                    Dim index As Integer = 0

                    For Each m As Materie_Prime In g2g.MateriePrimeToUpdate

                        Dim materia_prima_pubblico As Boolean = String.IsNullOrEmpty(To_Piva) OrElse m.Sa_Cod = -1
                        Dim materia_prima_piva = If(materia_prima_pubblico, m.Piva, To_Piva)

                        Dim recode = (From rr In GiasContext.G2G_Recode_MateriePrime Where rr.From_PivaSuperUser = From_PivaSuperUser AndAlso rr.To_PivaSuperUser = To_PivaSuperUser AndAlso rr.To_Piva = materia_prima_piva AndAlso rr.From_Mat_Cod = m.Mat_Cod).FirstOrDefault()
                        recode.Username_Modifica = username
                        recode.Data_Modifica = data
                        recode.datainvio = data
                        g2g.Recode.G2GRecodeMateriePrimeToUpdate.Add(recode)
                        GiasContext.G2G_Recode_MateriePrime.Attach(recode)
                        GiasContext.Entry(recode).State = EntityState.Modified

                        Dim mp = (From mm In GiasContext.Materie_Prime Where mm.Mat_Cod = recode.To_Mat_Cod).FirstOrDefault()
                        mp = Gias_EF_Utility.CopyEntity(GiasContext, m, mp, username, data)
                        mp.Mat_Cod = recode.To_Mat_Cod
                        mp.Piva = recode.To_Piva
                        GiasContext.Materie_Prime.Attach(mp)
                        GiasContext.Entry(mp).State = EntityState.Modified

                        G2GUtility.SaveChanges(GiasContext, index)

                    Next

                    GiasContext.SaveChanges()

                End If

                If g2g.Materie_Prime_XLingueToDelete.Count > 0 Then

                    For Each m In g2g.Materie_Prime_XLingueToDelete

                        Dim materia_prima_pubblico As Boolean = String.IsNullOrEmpty(To_Piva) OrElse m.Sa_Cod = -1
                        Dim materia_prima_piva = If(materia_prima_pubblico, m.Piva, To_Piva)

                        Dim recode = (From rr In GiasContext.G2G_Recode_MateriePrime
                                      Where rr.From_PivaSuperUser = From_PivaSuperUser AndAlso
                                          rr.To_PivaSuperUser = To_PivaSuperUser AndAlso
                                          rr.To_Piva = materia_prima_piva AndAlso
                                          rr.From_Mat_Cod = m.Mat_Cod).FirstOrDefault

                        ''cancella materia
                        Dim mp = (From mpr In GiasContext.Materie_Prime_XLingue
                                  Where mpr.Mat_Cod = recode.To_Mat_Cod).ToList
                        For Each mat_prima_xLingue In mp
                            GiasContext.Materie_Prime_XLingue.Attach(mat_prima_xLingue)
                            GiasContext.Materie_Prime_XLingue.Remove(mat_prima_xLingue)
                        Next


                    Next

                    GiasContext.SaveChanges()

                End If

                If g2g.Materie_Prime_XLingueToInsert.Count > 0 Then

                    For Each m As Materie_Prime_XLingue In g2g.Materie_Prime_XLingueToInsert

                        Dim recode = (From r In GiasContext.G2G_Recode_MateriePrime
                                      Where r.From_PivaSuperUser = From_PivaSuperUser And
                                              r.To_PivaSuperUser = To_PivaSuperUser And
                                              r.From_Piva = m.Piva And
                                              r.From_Mat_Cod = m.Mat_Cod).FirstOrDefault

                        Dim materia_prima_xLingue = Gias_EF_Utility.CopyEntity(GiasContext, m, Nothing, username, data)

                        materia_prima_xLingue.Piva = recode.To_Piva
                        materia_prima_xLingue.Mat_Cod = recode.To_Mat_Cod
                        GiasContext.Materie_Prime_XLingue.Add(materia_prima_xLingue)
                    Next

                    GiasContext.SaveChanges()

                End If

                ' log importazione
                g2g.Recode.LogRecode = g2g.MateriePrimeToInsert.Count & " nuove, " & g2g.MateriePrimeToUpdate.Count & " modificate, " & g2g.Recode.G2GRecodeMateriePrimeToDelete.Count & " cancellate"

            End Using

        Catch ex As Exception

            messaggioErrore = ex.Message
            g2g.Recode.MessaggioErrore = messaggioErrore
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)

        End Try

        Return g2g.Recode

    End Function

    Public Function Scrivi_MateriePrimeReverse(ByRef g2g As G2G_MateriePrime_Reverse, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GMateriePrime_W.Scrivi_MateriePrimeReverse()"
        Dim messaggioErrore As String = ""

        Dim From_PivaSuperUser = g2g.From_PivaSuperUser
        Dim To_PivaSuperUser = g2g.To_PivaSuperUser
        Dim From_Piva = g2g.From_Piva
        Dim To_Piva = g2g.To_Piva
        Dim username = objParametri.UsernameOperazione
        Dim data = Date.Now

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try

            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)


                ' INSERISCE MATERIE PRIME
                If g2g.MateriePrimeToInsert.Count > 0 Then

                    Dim index As Integer = 0

                    For Each m As Materie_Prime In g2g.MateriePrimeToInsert

                        Dim materia_prima As Materie_Prime = Nothing
                        Dim materia_prima_pubblico As Boolean = String.IsNullOrEmpty(To_Piva) OrElse m.Sa_Cod = -1
                        Dim materia_prima_piva = If(materia_prima_pubblico, m.Piva, To_Piva)

                        ' verifico se esiste già la materia prima in destinazione
                        If materia_prima_pubblico Then
                            materia_prima = (From mp In GiasContext.Materie_Prime
                                             Where mp.Piva = m.Piva _
                                               And mp.Sa_Cod = -1 _
                                               And mp.Elem_Cod = m.Elem_Cod _
                                               And mp.Sem_Cod = m.Sem_Cod _
                                               And mp.Veg_Cod = m.Veg_Cod _
                                               And mp.Cul_Cod = m.Cul_Cod _
                                               And mp.Regolamento = m.Regolamento).FirstOrDefault()
                        End If

                        If materia_prima Is Nothing Then
                            Dim idSeq = G2GUtility.NuovoId_Tabella(GiasContext, "materie_prime", 0, 2000000000, objParametri)
                            materia_prima = Gias_EF_Utility.CopyEntity(GiasContext, m, Nothing, username, data)
                            materia_prima.Mat_Cod = idSeq
                            materia_prima.Piva = materia_prima_piva
                            GiasContext.Materie_Prime.Add(materia_prima)
                        End If

                        Dim recode =
                        New G2G_Recode_MateriePrime With {
                            .From_PivaSuperUser = To_PivaSuperUser,
                            .To_PivaSuperUser = From_PivaSuperUser,
                            .From_Piva = materia_prima.Piva,
                            .To_Piva = m.Piva,
                            .From_Mat_Cod = materia_prima.Mat_Cod,
                            .To_Mat_Cod = m.Mat_Cod,
                            .Username_Creazione = username,
                            .Username_Modifica = username,
                            .Data_Creazione = Now(),
                            .Data_Modifica = Now(),
                            .Validita_Inizio = AGRODATAINIZIO,
                            .Validita_Fine = AGRODATAFINE,
                            .inviato = "0",
                            .datainvio = Now()
                        }
                        g2g.Recode.G2GRecodeMateriePrimeToInsert.Add(recode)
                        GiasContext.G2G_Recode_MateriePrime.Add(recode)

                        G2GUtility.SaveChanges(GiasContext, index)

                    Next

                    GiasContext.SaveChanges()

                End If


                ' MODIFICA MATERIE PRIME
                If g2g.MateriePrimeToUpdate.Count > 0 Then

                    Dim index As Integer = 0

                    For Each m As Materie_Prime In g2g.MateriePrimeToUpdate

                        Dim materia_prima_pubblico As Boolean = String.IsNullOrEmpty(To_Piva) OrElse m.Sa_Cod = -1
                        Dim materia_prima_piva = If(materia_prima_pubblico, m.Piva, To_Piva)

                        Dim recode = (From rr In GiasContext.G2G_Recode_MateriePrime Where rr.To_PivaSuperUser = From_PivaSuperUser AndAlso rr.From_PivaSuperUser = To_PivaSuperUser AndAlso rr.To_Piva = materia_prima_piva AndAlso rr.To_Mat_Cod = m.Mat_Cod).FirstOrDefault()
                        recode.Username_Modifica = username
                        recode.Data_Modifica = data
                        recode.datainvio = data
                        g2g.Recode.G2GRecodeMateriePrimeToUpdate.Add(recode)
                        GiasContext.G2G_Recode_MateriePrime.Attach(recode)
                        GiasContext.Entry(recode).State = EntityState.Modified

                        Dim mp = (From mm In GiasContext.Materie_Prime Where mm.Mat_Cod = recode.From_Mat_Cod).FirstOrDefault()
                        mp = Gias_EF_Utility.CopyEntity(GiasContext, m, mp, username, data)
                        mp.Mat_Cod = recode.From_Mat_Cod
                        mp.Piva = recode.To_Piva
                        GiasContext.Materie_Prime.Attach(mp)
                        GiasContext.Entry(mp).State = EntityState.Modified

                        G2GUtility.SaveChanges(GiasContext, index)

                    Next

                    GiasContext.SaveChanges()

                End If


                ' CANCELLA MATERIE PRIME
                If g2g.Recode.G2GRecodeMateriePrimeToDelete.Count > 0 Then

                    Dim index As Integer = 0

                    For Each r As G2G_Recode_MateriePrime In g2g.Recode.G2GRecodeMateriePrimeToDelete

                        ''cancella materia
                        Dim mp = (From m In GiasContext.Materie_Prime Where m.Mat_Cod = r.From_Mat_Cod).FirstOrDefault()
                        GiasContext.Materie_Prime.Attach(mp)
                        GiasContext.Materie_Prime.Remove(mp)

                        '' cancella recode materia
                        Dim recode = (From rr In GiasContext.G2G_Recode_MateriePrime Where rr.To_PivaSuperUser = From_PivaSuperUser AndAlso rr.From_PivaSuperUser = To_PivaSuperUser AndAlso rr.To_Mat_Cod = r.From_Mat_Cod).FirstOrDefault()
                        GiasContext.G2G_Recode_MateriePrime.Attach(recode)
                        GiasContext.G2G_Recode_MateriePrime.Remove(recode)

                        G2GUtility.SaveChanges(GiasContext, index)

                    Next

                    GiasContext.SaveChanges()

                End If

                ' log importazione
                g2g.Recode.LogRecode = g2g.MateriePrimeToInsert.Count & " nuove, " & g2g.MateriePrimeToUpdate.Count & " modificate, " & g2g.Recode.G2GRecodeMateriePrimeToDelete.Count & " cancellate"

            End Using

        Catch ex As Exception

            messaggioErrore = ex.Message
            g2g.Recode.MessaggioErrore = messaggioErrore
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)

        End Try

        Return g2g.Recode

    End Function

End Class