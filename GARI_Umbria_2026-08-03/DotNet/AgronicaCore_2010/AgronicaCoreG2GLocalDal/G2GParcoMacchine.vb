Imports System.Data.Entity
Imports System.Transactions
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModello

Public Class G2GParcoMacchine_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Nuovo_Parco_Macchine_G2G(ByVal From_PivaSuperUser As String, ByVal To_PivaSuperUser As String, ByVal From_Piva As String, ByVal To_Piva As String) As G2G_Parco_Macchine

        Dim g2g As New G2G_Parco_Macchine
        With g2g
            .From_PivaSuperUser = From_PivaSuperUser
            .To_PivaSuperUser = To_PivaSuperUser
            .From_Piva = From_Piva
            .To_Piva = To_Piva
            .ParcoMacchineToInsert = New List(Of G2G_Macchina)
            .ParcoMacchineToUpdate = New List(Of G2G_Macchina)
            .ParcoMacchineToDelete = New List(Of G2G_Macchina)
            .Recode = G2GUtility.Nuovo_G2G_Recode()
        End With

        Return g2g

    End Function

    Public Function Nuovo_Parco_Macchine_G2GReverse(ByVal From_PivaSuperUser As String, ByVal To_PivaSuperUser As String, ByVal From_Piva As String, ByVal To_Piva As String) As G2G_Parco_Macchine_Reverse

        Dim g2g As New G2G_Parco_Macchine_Reverse
        With g2g
            .From_PivaSuperUser = From_PivaSuperUser
            .To_PivaSuperUser = To_PivaSuperUser
            .From_Piva = From_Piva
            .To_Piva = To_Piva
            .ParcoMacchineToInsert = New List(Of G2G_Macchina)
            .ParcoMacchineToUpdate = New List(Of G2G_Macchina)
            .ParcoMacchineToDelete = New List(Of G2G_Macchina)
            .Recode = G2GUtility.Nuovo_G2G_Recode()
        End With

        Return g2g

    End Function

    Public Function Leggi_Parco_Macchine_G2G(ByVal Piva As String, ByVal Flag_Pubblico As Boolean, ByVal listaImprese As List(Of String), ByVal dataValidita As Date, ByRef g2g As G2G_Parco_Macchine, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GParcoMacchine_R.Leggi_Parco_Macchine_G2G()"
        Dim From_PivaSuperUser = g2g.From_PivaSuperUser
        Dim To_PivaSuperUser = g2g.To_PivaSuperUser

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
            GiasContext.Database.CommandTimeout = 3600
            Dim listaMacchine As New List(Of Integer?)
            Dim listaCausali As New List(Of String)({"6800", "6850", "6851", "8100"})

            If dataValidita > AGRODATAINIZIO Then

                ' macchine movimentate
                listaMacchine = (
                    From d In GiasContext.Movimenti_dettagli
                    Join m In GiasContext.Movimenti On d.Id_Agenda Equals m.Id_Agenda And d.PIVA Equals m.PIVA And d.Id_Mov Equals m.Id_Mov
                    Where listaCausali.Contains(m.Cau_Mov) AndAlso d.Elem_Cod = 1 AndAlso d.Mat_Cod <> 0 AndAlso
                        m.Data_Movimento >= dataValidita AndAlso (listaImprese.Count = 0 OrElse listaImprese.Contains(m.PIVA))
                    Select d.Mat_Cod).Distinct().ToList()

            End If

            Dim listaMacchineInsert = (
                From m In GiasContext.Parco_Macchine
                Where If(Flag_Pubblico, m.Sa_Cod = -1, m.Sa_Cod <> -1 AndAlso m.Piva = Piva) AndAlso
                    (dataValidita = AGRODATAINIZIO OrElse listaMacchine.Contains(m.Mac_Cod)) AndAlso
                    Not GiasContext.G2G_Recode_Parco_Macchine.Any(Function(g) g.From_PivaSuperUser = From_PivaSuperUser AndAlso
                        g.To_PivaSuperUser = To_PivaSuperUser AndAlso g.From_Mac_Cod = m.Mac_Cod)
                Select m).ToList()

            For Each macchina In listaMacchineInsert

                Dim costi = (From c In GiasContext.Prodotti_Costi Where c.Piva = macchina.Piva And c.Mat_Cod = macchina.Mac_Cod And c.Elem_Cod = 1 And c.Id_Budget = 0 Select c).ToList()

                ' forzo il centro aziendale macchina con quello destinazione
                If Not Flag_Pubblico AndAlso macchina.Sa_Cod <> 0 Then
                    Dim recode = (From rr In GiasContext.G2G_Recode_Imprese Where rr.From_PivaSuperUser = From_PivaSuperUser AndAlso rr.To_PivaSuperUser = To_PivaSuperUser AndAlso rr.FROM_Piva = macchina.Piva AndAlso rr.FROM_SaCod = macchina.Sa_Cod).FirstOrDefault()
                    macchina.Sa_Cod = If(recode Is Nothing, 0, recode.TO_SaCod)
                End If

                g2g.ParcoMacchineToInsert.Add(New G2G_Macchina With {.Macchina = macchina, .Costi = costi})

            Next

            Dim listaMacchineUpdate = (
                From m In GiasContext.Parco_Macchine
                Where If(Flag_Pubblico, m.Sa_Cod = -1, m.Sa_Cod <> -1 AndAlso m.Piva = Piva) AndAlso
                    (dataValidita = AGRODATAINIZIO OrElse listaMacchine.Contains(m.Mac_Cod)) AndAlso
                    GiasContext.G2G_Recode_Parco_Macchine.Any(Function(g) g.From_PivaSuperUser = From_PivaSuperUser AndAlso
                        g.To_PivaSuperUser = To_PivaSuperUser AndAlso g.From_Mac_Cod = m.Mac_Cod AndAlso g.datainvio < m.data_modifica)
                Select m).ToList()

            For Each macchina In listaMacchineUpdate

                Dim costi = (From c In GiasContext.Prodotti_Costi Where c.Piva = macchina.Piva And c.Mat_Cod = macchina.Mac_Cod And c.Elem_Cod = 1 And c.Id_Budget = 0 Select c).ToList()

                ' forzo il centro aziendale macchina con quello destinazione
                If Not Flag_Pubblico AndAlso macchina.Sa_Cod <> 0 Then
                    Dim recode = (From rr In GiasContext.G2G_Recode_Imprese Where rr.From_PivaSuperUser = From_PivaSuperUser AndAlso rr.To_PivaSuperUser = To_PivaSuperUser AndAlso rr.FROM_Piva = macchina.Piva AndAlso rr.FROM_SaCod = macchina.Sa_Cod).FirstOrDefault()
                    macchina.Sa_Cod = If(recode Is Nothing, 0, recode.TO_SaCod)
                End If

                g2g.ParcoMacchineToUpdate.Add(New G2G_Macchina With {.Macchina = macchina, .Costi = costi})

            Next

            If Not Flag_Pubblico Then
                g2g.Recode.G2GRecodeParcoMacchineToDelete = (
                    From g In GiasContext.G2G_Recode_Parco_Macchine
                    Where g.From_PivaSuperUser = From_PivaSuperUser AndAlso g.To_PivaSuperUser = To_PivaSuperUser AndAlso
                        Not GiasContext.Parco_Macchine.Any(Function(m) m.Mac_Cod = g.From_Mac_Cod)
                    Select g).ToList()
            End If

        End Using

        Return g2g.ParcoMacchineToInsert.Count > 0 OrElse g2g.ParcoMacchineToUpdate.Count > 0 OrElse g2g.Recode.G2GRecodeParcoMacchineToDelete.Count > 0

    End Function

    Public Function Leggi_Parco_Macchine_G2GReverse(ByVal Piva As String, ByVal Flag_Pubblico As Boolean, ByVal listaImprese As List(Of String), ByVal dataValidita As Date, ByRef g2g As G2G_Parco_Macchine_Reverse, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GParcoMacchine_R.Leggi_Parco_Macchine_G2GReverse()"
        Dim From_PivaSuperUser = g2g.From_PivaSuperUser
        Dim To_PivaSuperUser = g2g.To_PivaSuperUser

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            GiasContext.Database.CommandTimeout = 3600

            Dim listaMacchine As New List(Of Integer?)
            Dim listaCausali As New List(Of String)({"6800", "6850", "6851", "8100"})

            If dataValidita > AGRODATAINIZIO Then

                ' macchine movimentate
                listaMacchine = (
                    From d In GiasContext.Movimenti_dettagli
                    Join m In GiasContext.Movimenti On d.Id_Agenda Equals m.Id_Agenda And d.PIVA Equals m.PIVA And d.Id_Mov Equals m.Id_Mov
                    Where listaCausali.Contains(m.Cau_Mov) AndAlso d.Elem_Cod = 1 AndAlso d.Mat_Cod <> 0 AndAlso
                        m.Data_Movimento >= dataValidita AndAlso (listaImprese.Count = 0 OrElse listaImprese.Contains(m.PIVA))
                    Select d.Mat_Cod).Distinct().ToList()

            End If

            Dim listaMacchineInsert = (
                From m In GiasContext.Parco_Macchine
                Where If(Flag_Pubblico, m.Sa_Cod = -1, m.Sa_Cod <> -1 AndAlso m.Piva = Piva) AndAlso
                    (dataValidita = AGRODATAINIZIO OrElse listaMacchine.Contains(m.Mac_Cod)) AndAlso
                    Not GiasContext.G2G_Recode_Parco_Macchine.Any(Function(g) g.To_PivaSuperUser = From_PivaSuperUser AndAlso
                        g.From_PivaSuperUser = To_PivaSuperUser AndAlso g.To_Mac_Cod = m.Mac_Cod)
                Select m).ToList()

            For Each macchina In listaMacchineInsert

                Dim costi = (From c In GiasContext.Prodotti_Costi Where c.Piva = macchina.Piva And c.Mat_Cod = macchina.Mac_Cod And c.Elem_Cod = 1 And c.Id_Budget = 0 Select c).ToList()

                ' forzo il centro aziendale macchina con quello destinazione
                If Not Flag_Pubblico AndAlso macchina.Sa_Cod <> 0 Then
                    Dim recode = (From rr In GiasContext.G2G_Recode_Imprese Where rr.To_PivaSuperUser = From_PivaSuperUser AndAlso rr.From_PivaSuperUser = To_PivaSuperUser AndAlso rr.FROM_Piva = macchina.Piva AndAlso rr.TO_SaCod = macchina.Sa_Cod).FirstOrDefault()
                    macchina.Sa_Cod = If(recode Is Nothing, 0, recode.FROM_SaCod)
                End If

                g2g.ParcoMacchineToInsert.Add(New G2G_Macchina With {.Macchina = macchina, .Costi = costi})

            Next

            Dim listaMacchineUpdate = (
                From m In GiasContext.Parco_Macchine
                Where If(Flag_Pubblico, m.Sa_Cod = -1, m.Sa_Cod <> -1 AndAlso m.Piva = Piva) AndAlso
                    (dataValidita = AGRODATAINIZIO OrElse listaMacchine.Contains(m.Mac_Cod)) AndAlso
                    GiasContext.G2G_Recode_Parco_Macchine.Any(Function(g) g.To_PivaSuperUser = From_PivaSuperUser AndAlso
                        g.From_PivaSuperUser = To_PivaSuperUser AndAlso g.To_Mac_Cod = m.Mac_Cod AndAlso g.datainvio < m.data_modifica)
                Select m).ToList()

            For Each macchina In listaMacchineUpdate

                Dim costi = (From c In GiasContext.Prodotti_Costi Where c.Piva = macchina.Piva And c.Mat_Cod = macchina.Mac_Cod And c.Elem_Cod = 1 And c.Id_Budget = 0 Select c).ToList()

                ' forzo il centro aziendale macchina con quello destinazione
                If Not Flag_Pubblico AndAlso macchina.Sa_Cod <> 0 Then
                    Dim recode = (From rr In GiasContext.G2G_Recode_Imprese Where rr.To_PivaSuperUser = From_PivaSuperUser AndAlso rr.From_PivaSuperUser = To_PivaSuperUser AndAlso rr.FROM_Piva = macchina.Piva AndAlso rr.TO_SaCod = macchina.Sa_Cod).FirstOrDefault()
                    macchina.Sa_Cod = If(recode Is Nothing, 0, recode.FROM_SaCod)
                End If

                g2g.ParcoMacchineToUpdate.Add(New G2G_Macchina With {.Macchina = macchina, .Costi = costi})

            Next

            If Not Flag_Pubblico Then
                g2g.Recode.G2GRecodeParcoMacchineToDelete = (
                    From g In GiasContext.G2G_Recode_Parco_Macchine
                    Where g.To_PivaSuperUser = From_PivaSuperUser AndAlso g.From_PivaSuperUser = To_PivaSuperUser AndAlso
                        Not GiasContext.Parco_Macchine.Any(Function(m) m.Mac_Cod = g.To_Mac_Cod)
                    Select g).ToList()
            End If

        End Using

        Return g2g.ParcoMacchineToInsert.Count > 0 OrElse g2g.ParcoMacchineToUpdate.Count > 0 OrElse g2g.Recode.G2GRecodeParcoMacchineToDelete.Count > 0

    End Function

End Class

Public Class G2GParcoMacchine_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi_Parco_Macchine_G2G(ByRef g2g As G2G_Parco_Macchine, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GParcoMacchine_W.Scrivi_Parco_Macchine_G2G()"
        Dim messaggioErrore As String = ""

        If G2GUtility.transactionScope Then

            Dim transactionOptions As New TransactionOptions()
            transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            transactionOptions.Timeout = G2GUtility.GetTransactionTimeout(objParametri)
            Dim g2gRecode As G2G_Recode = g2g.Recode

            Try

                Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)
                    g2gRecode = Scrivi_Parco_Macchine(g2g, objParametri)
                    scope.Complete()
                End Using

            Catch ex As Exception

                messaggioErrore = ex.Message
                g2gRecode.MessaggioErrore = messaggioErrore
                Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)

            End Try

            Return g2gRecode

        End If

        Return Scrivi_Parco_Macchine(g2g, objParametri)

    End Function

    Public Function Scrivi_Parco_Macchine_G2GReverse(ByRef g2g As G2G_Parco_Macchine_Reverse, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GParcoMacchine_W.Scrivi_Parco_Macchine_G2GReverse()"
        Dim messaggioErrore As String = ""

        If G2GUtility.transactionScope Then

            Dim transactionOptions As New TransactionOptions()
            transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            transactionOptions.Timeout = G2GUtility.GetTransactionTimeout(objParametri)
            Dim g2gRecode As G2G_Recode = g2g.Recode

            Try

                Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)
                    g2gRecode = Scrivi_Parco_MacchineReverse(g2g, objParametri)
                    scope.Complete()
                End Using

            Catch ex As Exception

                messaggioErrore = ex.Message
                g2gRecode.MessaggioErrore = messaggioErrore
                Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)

            End Try

            Return g2gRecode

        End If

        Return Scrivi_Parco_MacchineReverse(g2g, objParametri)

    End Function

    Public Function Scrivi_Parco_Macchine(ByRef g2g As G2G_Parco_Macchine, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GParcoMacchine_W.Scrivi_Parco_Macchine()"
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

                ' MACCHINE DA INSERIRE
                If g2g.ParcoMacchineToInsert.Count > 0 Then

                    Dim index As Integer = 0

                    For Each m As G2G_Macchina In g2g.ParcoMacchineToInsert

                        ' nuova macchina
                        Dim idSeq = G2GUtility.NuovoId_Tabella(GiasContext, "Parco_Macchine", 0, 2000000000, objParametri)
                        Dim macchina = Gias_EF_Utility.CopyEntity(GiasContext, m.Macchina, Nothing, username, data)
                        macchina.Piva = To_Piva
                        macchina.Mac_Cod = idSeq
                        GiasContext.Parco_Macchine.Add(macchina)

                        ' inserisce costi macchina
                        For Each c As Prodotti_Costi In m.Costi
                            Dim costo = Gias_EF_Utility.CopyEntity(GiasContext, c, Nothing, username, data)
                            costo.Piva = To_Piva
                            costo.Mat_Cod = macchina.Mac_Cod
                            GiasContext.Prodotti_Costi.Add(costo)
                        Next

                        'nuovo recode macchina
                        Dim recode =
                                    New G2G_Recode_Parco_Macchine With {
                                        .From_PivaSuperUser = From_PivaSuperUser,
                                        .To_PivaSuperUser = To_PivaSuperUser,
                                        .From_Mac_Cod = m.Macchina.Mac_Cod,
                                        .To_Mac_Cod = macchina.Mac_Cod,
                                        .Username_Creazione = username,
                                        .Username_Modifica = username,
                                        .Data_Creazione = Now(),
                                        .Data_Modifica = Now(),
                                        .Validita_Inizio = AGRODATAINIZIO,
                                        .Validita_Fine = AGRODATAFINE,
                                        .inviato = "0",
                                        .datainvio = Now()
                                    }
                        g2g.Recode.G2GRecodeParcoMacchineToInsert.Add(recode)
                        GiasContext.G2G_Recode_Parco_Macchine.Add(recode)

                        G2GUtility.SaveChanges(GiasContext, index)

                    Next

                    GiasContext.SaveChanges()

                End If

                ' MACCHINE DA MODIFICARE
                If g2g.ParcoMacchineToUpdate.Count > 0 Then

                    Dim index As Integer = 0

                    ' macchine da modificare
                    For Each m As G2G_Macchina In g2g.ParcoMacchineToUpdate

                        ' modifica recode macchina
                        Dim recode = (From rr In GiasContext.G2G_Recode_Parco_Macchine Where rr.From_PivaSuperUser = From_PivaSuperUser AndAlso rr.To_PivaSuperUser = To_PivaSuperUser AndAlso rr.From_Mac_Cod = m.Macchina.Mac_Cod).FirstOrDefault()
                        recode.Username_Modifica = username
                        recode.Data_Modifica = data
                        recode.datainvio = data
                        g2g.Recode.G2GRecodeParcoMacchineToUpdate.Add(recode)
                        GiasContext.G2G_Recode_Parco_Macchine.Attach(recode)
                        GiasContext.Entry(recode).State = EntityState.Modified

                        ' modifica macchina
                        Dim macchina = (From mm In GiasContext.Parco_Macchine Where mm.Mac_Cod = recode.To_Mac_Cod).FirstOrDefault()
                        macchina = Gias_EF_Utility.CopyEntity(GiasContext, m.Macchina, macchina, username, data)
                        macchina.Piva = To_Piva
                        macchina.Mac_Cod = recode.To_Mac_Cod
                        GiasContext.Parco_Macchine.Attach(macchina)
                        GiasContext.Entry(macchina).State = EntityState.Modified

                        ' cancella costi macchina
                        Dim costi = (From c In GiasContext.Prodotti_Costi Where c.Piva = macchina.Piva And c.Mat_Cod = macchina.Mac_Cod And c.Elem_Cod = 1 And c.Id_Budget = 0 Select c).ToList()
                        For Each c As Prodotti_Costi In costi
                            GiasContext.Prodotti_Costi.Attach(c)
                            GiasContext.Prodotti_Costi.Remove(c)
                        Next

                        ' inserisce costi macchina
                        For Each c As Prodotti_Costi In m.Costi
                            Dim costo = Gias_EF_Utility.CopyEntity(GiasContext, c, Nothing, username, data)
                            costo.Piva = To_Piva
                            costo.Mat_Cod = macchina.Mac_Cod
                            GiasContext.Prodotti_Costi.Add(costo)
                        Next

                        G2GUtility.SaveChanges(GiasContext, index)

                    Next

                    GiasContext.SaveChanges()

                End If

                ' MACCHINE DA CANCELLARE
                If g2g.Recode.G2GRecodeParcoMacchineToDelete.Count > 0 Then

                    Dim index As Integer = 0

                    For Each r As G2G_Recode_Parco_Macchine In g2g.Recode.G2GRecodeParcoMacchineToDelete

                        Dim macchina = (From m In GiasContext.Parco_Macchine Where m.Mac_Cod = r.To_Mac_Cod).FirstOrDefault()

                        'cancella macchina privata azienda
                        If macchina IsNot Nothing AndAlso macchina.Piva = To_Piva AndAlso macchina.Sa_Cod <> -1 Then

                            'cancella costi macchina
                            Dim costi = (From c In GiasContext.Prodotti_Costi Where c.Piva = macchina.Piva And c.Mat_Cod = macchina.Mac_Cod And c.Elem_Cod = 1 And c.Id_Budget = 0 Select c).ToList()
                            For Each c As Prodotti_Costi In costi
                                GiasContext.Prodotti_Costi.Attach(c)
                                GiasContext.Prodotti_Costi.Remove(c)
                            Next

                            'cancella macchina
                            GiasContext.Parco_Macchine.Attach(macchina)
                            GiasContext.Parco_Macchine.Remove(macchina)

                            ' cancella recode macchina
                            Dim recode = (From rr In GiasContext.G2G_Recode_Parco_Macchine Where rr.From_PivaSuperUser = From_PivaSuperUser AndAlso rr.To_PivaSuperUser = To_PivaSuperUser AndAlso rr.From_Mac_Cod = r.From_Mac_Cod).FirstOrDefault()
                            If recode IsNot Nothing Then
                                GiasContext.G2G_Recode_Parco_Macchine.Attach(recode)
                                GiasContext.G2G_Recode_Parco_Macchine.Remove(recode)
                            End If

                        End If

                        G2GUtility.SaveChanges(GiasContext, index)

                    Next

                    GiasContext.SaveChanges()

                End If

                ' log importazione
                g2g.Recode.LogRecode = g2g.ParcoMacchineToInsert.Count & " nuovi, " & g2g.ParcoMacchineToUpdate.Count & " modificati, " & g2g.Recode.G2GRecodeParcoMacchineToDelete.Count & " cancellati"

            End Using

        Catch ex As Exception

            messaggioErrore = ex.Message
            g2g.Recode.MessaggioErrore = messaggioErrore
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)

        End Try

        Return g2g.Recode

    End Function

    Public Function Scrivi_Parco_MacchineReverse(ByRef g2g As G2G_Parco_Macchine_Reverse, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GParcoMacchine_W.Scrivi_Parco_MacchineReverse()"
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

                ' MACCHINE DA INSERIRE
                If g2g.ParcoMacchineToInsert.Count > 0 Then

                    Dim index As Integer = 0

                    For Each m As G2G_Macchina In g2g.ParcoMacchineToInsert

                        ' nuova macchina
                        Dim idSeq = G2GUtility.NuovoId_Tabella(GiasContext, "Parco_Macchine", 0, 2000000000, objParametri)
                        Dim macchina = Gias_EF_Utility.CopyEntity(GiasContext, m.Macchina, Nothing, username, data)
                        macchina.Piva = To_Piva
                        macchina.Mac_Cod = idSeq
                        GiasContext.Parco_Macchine.Add(macchina)

                        ' inserisce costi macchina
                        For Each c As Prodotti_Costi In m.Costi
                            Dim costo = Gias_EF_Utility.CopyEntity(GiasContext, c, Nothing, username, data)
                            costo.Piva = To_Piva
                            costo.Mat_Cod = macchina.Mac_Cod
                            GiasContext.Prodotti_Costi.Add(costo)
                        Next

                        'nuovo recode macchina
                        Dim recode =
                                    New G2G_Recode_Parco_Macchine With {
                                        .From_PivaSuperUser = To_PivaSuperUser,
                                        .To_PivaSuperUser = From_PivaSuperUser,
                                        .From_Mac_Cod = macchina.Mac_Cod,
                                        .To_Mac_Cod = m.Macchina.Mac_Cod,
                                        .Username_Creazione = username,
                                        .Username_Modifica = username,
                                        .Data_Creazione = Now(),
                                        .Data_Modifica = Now(),
                                        .Validita_Inizio = AGRODATAINIZIO,
                                        .Validita_Fine = AGRODATAFINE,
                                        .inviato = "0",
                                        .datainvio = Now()
                                    }
                        g2g.Recode.G2GRecodeParcoMacchineToInsert.Add(recode)
                        GiasContext.G2G_Recode_Parco_Macchine.Add(recode)

                        G2GUtility.SaveChanges(GiasContext, index)

                    Next

                    GiasContext.SaveChanges()

                End If

                ' MACCHINE DA MODIFICARE
                If g2g.ParcoMacchineToUpdate.Count > 0 Then

                    Dim index As Integer = 0

                    ' macchine da modificare
                    For Each m As G2G_Macchina In g2g.ParcoMacchineToUpdate

                        ' modifica recode macchina
                        Dim recode = (From rr In GiasContext.G2G_Recode_Parco_Macchine Where rr.To_PivaSuperUser = From_PivaSuperUser AndAlso rr.From_PivaSuperUser = To_PivaSuperUser AndAlso rr.To_Mac_Cod = m.Macchina.Mac_Cod).FirstOrDefault()
                        recode.Username_Modifica = username
                        recode.Data_Modifica = data
                        recode.datainvio = data
                        g2g.Recode.G2GRecodeParcoMacchineToUpdate.Add(recode)
                        GiasContext.G2G_Recode_Parco_Macchine.Attach(recode)
                        GiasContext.Entry(recode).State = EntityState.Modified

                        ' modifica macchina
                        Dim macchina = (From mm In GiasContext.Parco_Macchine Where mm.Mac_Cod = recode.From_Mac_Cod).FirstOrDefault()
                        macchina = Gias_EF_Utility.CopyEntity(GiasContext, m.Macchina, macchina, username, data)
                        macchina.Piva = To_Piva
                        macchina.Mac_Cod = recode.From_Mac_Cod
                        GiasContext.Parco_Macchine.Attach(macchina)
                        GiasContext.Entry(macchina).State = EntityState.Modified

                        ' cancella costi macchina
                        Dim costi = (From c In GiasContext.Prodotti_Costi Where c.Piva = macchina.Piva And c.Mat_Cod = macchina.Mac_Cod And c.Elem_Cod = 1 And c.Id_Budget = 0 Select c).ToList()
                        For Each c As Prodotti_Costi In costi
                            GiasContext.Prodotti_Costi.Attach(c)
                            GiasContext.Prodotti_Costi.Remove(c)
                        Next

                        ' inserisce costi macchina
                        For Each c As Prodotti_Costi In m.Costi
                            Dim costo = Gias_EF_Utility.CopyEntity(GiasContext, c, Nothing, username, data)
                            costo.Piva = To_Piva
                            costo.Mat_Cod = macchina.Mac_Cod
                            GiasContext.Prodotti_Costi.Add(costo)
                        Next

                        G2GUtility.SaveChanges(GiasContext, index)

                    Next

                    GiasContext.SaveChanges()

                End If

                ' MACCHINE DA CANCELLARE
                If g2g.Recode.G2GRecodeParcoMacchineToDelete.Count > 0 Then

                    Dim index As Integer = 0

                    For Each r As G2G_Recode_Parco_Macchine In g2g.Recode.G2GRecodeParcoMacchineToDelete

                        Dim macchina = (From m In GiasContext.Parco_Macchine Where m.Mac_Cod = r.From_Mac_Cod).FirstOrDefault()

                        'cancella macchina privata azienda
                        If macchina IsNot Nothing AndAlso macchina.Piva = To_Piva AndAlso macchina.Sa_Cod <> -1 Then

                            'cancella costi macchina
                            Dim costi = (From c In GiasContext.Prodotti_Costi Where c.Piva = macchina.Piva And c.Mat_Cod = macchina.Mac_Cod And c.Elem_Cod = 1 And c.Id_Budget = 0 Select c).ToList()
                            For Each c As Prodotti_Costi In costi
                                GiasContext.Prodotti_Costi.Attach(c)
                                GiasContext.Prodotti_Costi.Remove(c)
                            Next

                            'cancella macchina
                            GiasContext.Parco_Macchine.Attach(macchina)
                            GiasContext.Parco_Macchine.Remove(macchina)

                            ' cancella recode macchina
                            Dim recode = (From rr In GiasContext.G2G_Recode_Parco_Macchine Where rr.To_PivaSuperUser = From_PivaSuperUser AndAlso rr.From_PivaSuperUser = To_PivaSuperUser AndAlso rr.To_Mac_Cod = r.From_Mac_Cod).FirstOrDefault()
                            If recode IsNot Nothing Then
                                GiasContext.G2G_Recode_Parco_Macchine.Attach(recode)
                                GiasContext.G2G_Recode_Parco_Macchine.Remove(recode)
                            End If

                        End If

                        G2GUtility.SaveChanges(GiasContext, index)

                    Next

                    GiasContext.SaveChanges()

                End If

                ' log importazione
                g2g.Recode.LogRecode = g2g.ParcoMacchineToInsert.Count & " nuovi, " & g2g.ParcoMacchineToUpdate.Count & " modificati, " & g2g.Recode.G2GRecodeParcoMacchineToDelete.Count & " cancellati"

            End Using

        Catch ex As Exception

            messaggioErrore = ex.Message
            g2g.Recode.MessaggioErrore = messaggioErrore
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)

        End Try

        Return g2g.Recode

    End Function

End Class