Imports System.Data.Entity
Imports System.Transactions
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModello

Public Class G2GImprese_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Nuovo_Imprese_G2G(ByVal From_PivaSuperUser As String, ByVal To_PivaSuperUser As String, ByVal From_Piva As String, ByVal To_Piva As String, ByVal To_Piva_Padre As String) As G2G_Imprese

        Dim g2g As New G2G_Imprese
        With g2g
            .From_PivaSuperUser = From_PivaSuperUser
            .To_PivaSuperUser = To_PivaSuperUser
            .From_Piva = From_Piva
            .To_Piva = To_Piva
            .To_Piva_Padre = To_Piva_Padre
            .ImpreseToInsert = New List(Of G2G_Impresa)
            .ImpreseToUpdate = New List(Of G2G_Impresa)
            .ImpreseToDelete = New List(Of G2G_Impresa)
            .Recode = G2GUtility.Nuovo_G2G_Recode()
        End With

        Return g2g

    End Function

    Public Function Nuovo_Imprese_G2G_Reverse(ByVal From_PivaSuperUser As String, ByVal To_PivaSuperUser As String, ByVal From_Piva As String, ByVal To_Piva As String, ByVal To_Piva_Padre As String) As G2G_Imprese_Reverse

        Dim g2g As New G2G_Imprese_Reverse
        With g2g
            .From_PivaSuperUser = From_PivaSuperUser
            .To_PivaSuperUser = To_PivaSuperUser
            .From_Piva = From_Piva
            .To_Piva = To_Piva
            .To_Piva_Padre = To_Piva_Padre
            .ImpreseToInsert = New List(Of G2G_Impresa)
            .ImpreseToUpdate = New List(Of G2G_Impresa)
            .ImpreseToDelete = New List(Of G2G_Impresa)
            .Recode = G2GUtility.Nuovo_G2G_Recode()
        End With

        Return g2g

    End Function

    Public Function Leggi_Imprese_G2G(ByVal Forza_Update As Boolean, ByRef g2g As G2G_Imprese, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GImprese_R.Leggi_Imprese_G2G()"
        Dim From_Piva = g2g.From_Piva
        Dim From_PivaSuperUser = g2g.From_PivaSuperUser
        Dim To_PivaSuperUser = g2g.To_PivaSuperUser

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
            GiasContext.Database.CommandTimeout = 3600
            Dim listaImpreseInsert = (
                From i In GiasContext.Imprese
                Where i.PIVA = From_Piva AndAlso
                    Not GiasContext.G2G_Recode_Imprese.Any(Function(g) g.From_PivaSuperUser = From_PivaSuperUser AndAlso
                        g.To_PivaSuperUser = To_PivaSuperUser AndAlso g.FROM_Piva = i.PIVA AndAlso g.FROM_SaCod = 0)
                Select i).ToList()

            For Each impresa In listaImpreseInsert
                Dim utente = (From ui In GiasContext.UtentiXImprese Where ui.PIVA = impresa.PIVA Select ui).FirstOrDefault()
                Dim gerarchia = (From gi In GiasContext.GerarchiaImprese Where gi.Figlio = impresa.PIVA Select gi).FirstOrDefault()
                Dim codici = (From c In GiasContext.Imprese_Codici Where c.PIVA = impresa.PIVA Select c).ToList()
                Dim indirizzi = (From ii In GiasContext.ImpresexIndirizzi Join i In GiasContext.Indirizzi On ii.cod_indirizzo Equals i.cod_indirizzo Where ii.PIVA = impresa.PIVA Select New G2G_Indirizzo With {.Indirizzo = i, .Tipo_Indirizzo = ii.Tipo_Indirizzo}).ToList()
                Dim contatto = (From c In GiasContext.Contatti Where c.Piva = From_PivaSuperUser AndAlso c.Cod_Contatto = impresa.PIVA Select c).FirstOrDefault()
                Dim risorse = (From ru In GiasContext.Risorse_Umane Where ru.Piva = From_PivaSuperUser AndAlso ru.Cod_Contatto = impresa.PIVA Select ru).ToList()
                g2g.ImpreseToInsert.Add(
                    New G2G_Impresa With {
                        .Impresa = impresa,
                        .Utente = utente,
                        .Gerarchia = gerarchia,
                        .Codici = codici,
                        .Indirizzi = indirizzi,
                        .ContattoImpresa = New G2G_Contatto With {.Contatto = contatto, .RisorseUmane = risorse, .Indirizzi = indirizzi}
                    })
            Next

            ' forzo la modifica impresa se manca il recode dell'indirizzo
            Dim Forza_Modifica As Boolean = False
            If Forza_Update AndAlso listaImpreseInsert.Count = 0 Then
                Forza_Modifica = (
                    From i In GiasContext.ImpresexIndirizzi Where i.PIVA = From_Piva AndAlso
                    GiasContext.G2G_Recode_Indirizzi.Any(Function(g) g.From_PivaSuperUser = From_PivaSuperUser AndAlso
                    g.To_PivaSuperUser = To_PivaSuperUser AndAlso g.From_Cod_Indirizzo = i.cod_indirizzo)
                    Select i).Count = 0
            End If

            Dim listaImpreseUpdate = (
                From i In GiasContext.Imprese
                Where i.PIVA = From_Piva AndAlso
                    GiasContext.G2G_Recode_Imprese.Any(Function(g) g.From_PivaSuperUser = From_PivaSuperUser AndAlso
                        g.To_PivaSuperUser = To_PivaSuperUser AndAlso g.FROM_Piva = i.PIVA AndAlso g.FROM_SaCod = 0 AndAlso (Forza_Modifica OrElse g.datainvio < i.Data_Modifica))
                Select i).ToList()

            For Each impresa In listaImpreseUpdate
                Dim utente = (From ui In GiasContext.UtentiXImprese Where ui.PIVA = impresa.PIVA Select ui).FirstOrDefault()
                Dim gerarchia = (From gi In GiasContext.GerarchiaImprese Where gi.Figlio = impresa.PIVA Select gi).FirstOrDefault()
                Dim codici = (From c In GiasContext.Imprese_Codici Where c.PIVA = impresa.PIVA Select c).ToList()
                Dim indirizzi = (From ii In GiasContext.ImpresexIndirizzi Join i In GiasContext.Indirizzi On ii.cod_indirizzo Equals i.cod_indirizzo Where ii.PIVA = impresa.PIVA Select New G2G_Indirizzo With {.Indirizzo = i, .Tipo_Indirizzo = ii.Tipo_Indirizzo}).ToList()
                Dim contatto = (From c In GiasContext.Contatti Where c.Piva = From_PivaSuperUser AndAlso c.Cod_Contatto = impresa.PIVA Select c).FirstOrDefault()
                Dim risorse = (From ru In GiasContext.Risorse_Umane Where ru.Piva = From_PivaSuperUser AndAlso ru.Cod_Contatto = impresa.PIVA Select ru).ToList()
                g2g.ImpreseToUpdate.Add(
                    New G2G_Impresa With {
                        .Impresa = impresa,
                        .Utente = utente,
                        .Gerarchia = gerarchia,
                        .Codici = codici,
                        .Indirizzi = indirizzi,
                        .ContattoImpresa = New G2G_Contatto With {.Contatto = contatto, .RisorseUmane = risorse, .Indirizzi = indirizzi}
                    })
            Next

            'g2g.Recode.G2GRecodeImpreseCentriToDelete = (
            '    From g In GiasContext.G2G_Recode_Imprese
            '    Where g.From_PivaSuperUser = From_PivaSuperUser AndAlso g.To_PivaSuperUser = To_PivaSuperUser _
            '         AndAlso g.FROM_Piva = From_Piva AndAlso g.FROM_SaCod = 0 AndAlso Not GiasContext.Imprese.Any(Function(i) i.PIVA = g.FROM_Piva)
            '    Select g).ToList()

        End Using

        Return g2g.ImpreseToInsert.Count > 0 OrElse g2g.ImpreseToUpdate.Count > 0

    End Function

    Public Function Leggi_Imprese_G2G_Reverse(ByVal Forza_Update As Boolean, ByRef g2g As G2G_Imprese_Reverse, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GImprese_R.Leggi_Imprese_G2G_Reverse()"
        Dim From_Piva = g2g.From_Piva
        Dim From_PivaSuperUser = g2g.From_PivaSuperUser
        Dim To_PivaSuperUser = g2g.To_PivaSuperUser

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
            GiasContext.Database.CommandTimeout = 3600
            Dim listaImpreseInsert = (
                From i In GiasContext.Imprese
                Where i.PIVA = From_Piva AndAlso
                    Not GiasContext.G2G_Recode_Imprese.Any(Function(g) g.To_PivaSuperUser = From_PivaSuperUser AndAlso
                        g.From_PivaSuperUser = To_PivaSuperUser AndAlso g.FROM_Piva = i.PIVA AndAlso g.FROM_SaCod = 0)
                Select i).ToList()

            For Each impresa In listaImpreseInsert
                Dim utente = (From ui In GiasContext.UtentiXImprese Where ui.PIVA = impresa.PIVA Select ui).FirstOrDefault()
                Dim gerarchia = (From gi In GiasContext.GerarchiaImprese Where gi.Figlio = impresa.PIVA Select gi).FirstOrDefault()
                Dim codici = (From c In GiasContext.Imprese_Codici Where c.PIVA = impresa.PIVA Select c).ToList()
                Dim indirizzi = (From ii In GiasContext.ImpresexIndirizzi Join i In GiasContext.Indirizzi On ii.cod_indirizzo Equals i.cod_indirizzo Where ii.PIVA = impresa.PIVA Select New G2G_Indirizzo With {.Indirizzo = i, .Tipo_Indirizzo = ii.Tipo_Indirizzo}).ToList()
                Dim contatto = (From c In GiasContext.Contatti Where c.Piva = From_PivaSuperUser AndAlso c.Cod_Contatto = impresa.PIVA Select c).FirstOrDefault()
                Dim risorse = (From ru In GiasContext.Risorse_Umane Where ru.Piva = From_PivaSuperUser AndAlso ru.Cod_Contatto = impresa.PIVA Select ru).ToList()
                g2g.ImpreseToInsert.Add(
                    New G2G_Impresa With {
                        .Impresa = impresa,
                        .Utente = utente,
                        .Gerarchia = gerarchia,
                        .Codici = codici,
                        .Indirizzi = indirizzi,
                        .ContattoImpresa = New G2G_Contatto With {.Contatto = contatto, .RisorseUmane = risorse, .Indirizzi = indirizzi}
                    })
            Next

            ' forzo la modifica impresa se manca il recode dell'indirizzo
            Dim Forza_Modifica As Boolean = False
            If Forza_Update AndAlso listaImpreseInsert.Count = 0 Then
                Forza_Modifica = (
                    From i In GiasContext.ImpresexIndirizzi Where i.PIVA = From_Piva AndAlso
                    GiasContext.G2G_Recode_Indirizzi.Any(Function(g) g.To_PivaSuperUser = From_PivaSuperUser AndAlso
                    g.From_PivaSuperUser = To_PivaSuperUser AndAlso g.From_Cod_Indirizzo = i.cod_indirizzo)
                    Select i).Count = 0
            End If

            Dim listaImpreseUpdate = (
                From i In GiasContext.Imprese
                Where i.PIVA = From_Piva AndAlso
                    GiasContext.G2G_Recode_Imprese.Any(Function(g) g.From_PivaSuperUser = From_PivaSuperUser AndAlso
                        g.To_PivaSuperUser = To_PivaSuperUser AndAlso g.FROM_Piva = i.PIVA AndAlso g.FROM_SaCod = 0 AndAlso (Forza_Modifica OrElse g.datainvio < i.Data_Modifica))
                Select i).ToList()

            For Each impresa In listaImpreseUpdate
                Dim utente = (From ui In GiasContext.UtentiXImprese Where ui.PIVA = impresa.PIVA Select ui).FirstOrDefault()
                Dim gerarchia = (From gi In GiasContext.GerarchiaImprese Where gi.Figlio = impresa.PIVA Select gi).FirstOrDefault()
                Dim codici = (From c In GiasContext.Imprese_Codici Where c.PIVA = impresa.PIVA Select c).ToList()
                Dim indirizzi = (From ii In GiasContext.ImpresexIndirizzi Join i In GiasContext.Indirizzi On ii.cod_indirizzo Equals i.cod_indirizzo Where ii.PIVA = impresa.PIVA Select New G2G_Indirizzo With {.Indirizzo = i, .Tipo_Indirizzo = ii.Tipo_Indirizzo}).ToList()
                Dim contatto = (From c In GiasContext.Contatti Where c.Piva = From_PivaSuperUser AndAlso c.Cod_Contatto = impresa.PIVA Select c).FirstOrDefault()
                Dim risorse = (From ru In GiasContext.Risorse_Umane Where ru.Piva = From_PivaSuperUser AndAlso ru.Cod_Contatto = impresa.PIVA Select ru).ToList()
                g2g.ImpreseToUpdate.Add(
                    New G2G_Impresa With {
                        .Impresa = impresa,
                        .Utente = utente,
                        .Gerarchia = gerarchia,
                        .Codici = codici,
                        .Indirizzi = indirizzi,
                        .ContattoImpresa = New G2G_Contatto With {.Contatto = contatto, .RisorseUmane = risorse, .Indirizzi = indirizzi}
                    })
            Next

            'g2g.Recode.G2GRecodeImpreseCentriToDelete = (
            '    From g In GiasContext.G2G_Recode_Imprese
            '    Where g.From_PivaSuperUser = From_PivaSuperUser AndAlso g.To_PivaSuperUser = To_PivaSuperUser _
            '         AndAlso g.FROM_Piva = From_Piva AndAlso g.FROM_SaCod = 0 AndAlso Not GiasContext.Imprese.Any(Function(i) i.PIVA = g.FROM_Piva)
            '    Select g).ToList()

        End Using

        Dim PivaSuperUser_Appoggio = ""
        PivaSuperUser_Appoggio = g2g.From_PivaSuperUser
        g2g.From_PivaSuperUser = g2g.To_PivaSuperUser
        g2g.To_PivaSuperUser = PivaSuperUser_Appoggio

        Return g2g.ImpreseToInsert.Count > 0 OrElse g2g.ImpreseToUpdate.Count > 0

    End Function

End Class

Public Class G2GImprese_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi_Imprese_G2G(ByRef g2g As G2G_Imprese, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GImprese_W.Scrivi_Imprese_G2G()"
        Dim messaggioErrore As String = ""

        If G2GUtility.transactionScope Then

            Dim transactionOptions As New TransactionOptions()
            transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            transactionOptions.Timeout = G2GUtility.GetTransactionTimeout(objParametri)
            Dim g2gRecode As G2G_Recode = g2g.Recode

            Try

                Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)
                    g2gRecode = Scrivi_Imprese(g2g, objParametri)
                    scope.Complete()
                End Using

            Catch ex As Exception

                messaggioErrore = ex.Message
                g2gRecode.MessaggioErrore = messaggioErrore
                Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)

            End Try

            Return g2gRecode

        End If

        Return Scrivi_Imprese(g2g, objParametri)

    End Function


    Public Function Scrivi_Imprese_G2GReverse(ByRef g2g As G2G_Imprese_Reverse, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GImprese_W.Scrivi_Imprese_G2GReverse()"
        Dim messaggioErrore As String = ""

        If G2GUtility.transactionScope Then

            Dim transactionOptions As New TransactionOptions()
            transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            transactionOptions.Timeout = G2GUtility.GetTransactionTimeout(objParametri)
            Dim g2gRecode As G2G_Recode = g2g.Recode

            Try

                Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)
                    g2gRecode = Scrivi_ImpreseReverse(g2g, objParametri)
                    scope.Complete()
                End Using

            Catch ex As Exception

                messaggioErrore = ex.Message
                g2gRecode.MessaggioErrore = messaggioErrore
                Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)

            End Try

            Return g2gRecode

        End If

        Return Scrivi_ImpreseReverse(g2g, objParametri)

    End Function

    Public Function Scrivi_Imprese(ByRef g2g As G2G_Imprese, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GImprese_W.Scrivi_Imprese()"
        Dim messaggioErrore As String = ""

        Dim From_PivaSuperUser = g2g.From_PivaSuperUser
        Dim To_PivaSuperUser = g2g.To_PivaSuperUser
        Dim From_Piva = g2g.From_Piva
        Dim To_Piva = g2g.To_Piva
        Dim To_Piva_Padre = g2g.To_Piva_Padre

        Dim username = objParametri.UsernameOperazione
        Dim data = Date.Now

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try

            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                ' IMPRESE DA INSERIRE
                If g2g.ImpreseToInsert.Count > 0 Then

                    Dim index As Integer = 0

                    For Each i As G2G_Impresa In g2g.ImpreseToInsert

                        ' verifico se l'impresa è già presente in destinazione
                        Dim impresa = (From ii In GiasContext.Imprese Where ii.PIVA = To_Piva).FirstOrDefault()

                        If impresa Is Nothing Then

                            ' nuova impresa
                            impresa = Gias_EF_Utility.CopyEntity(GiasContext, i.Impresa, Nothing, username, data)
                            impresa.PIVA = To_Piva
                            GiasContext.Imprese.Add(impresa)

                            ' inserisce gerarchia impresa
                            Dim gerarchia = Gias_EF_Utility.CopyEntity(GiasContext, i.Gerarchia, Nothing, username, data)
                            gerarchia.Padre = To_Piva_Padre
                            gerarchia.Figlio = To_Piva
                            GiasContext.GerarchiaImprese.Add(gerarchia)

                            ' inserisce utente impresa
                            Dim utente = Gias_EF_Utility.CopyEntity(GiasContext, i.Utente, Nothing, username, data)
                            utente.USER = To_PivaSuperUser
                            utente.PIVA = To_Piva
                            GiasContext.UtentiXImprese.Add(utente)

                            ' inserisce codici impresa
                            For Each ic As Imprese_Codici In i.Codici
                                Dim codice = Gias_EF_Utility.CopyEntity(GiasContext, ic, Nothing, username, data)
                                If codice.id_cod = enum_CodiciAnagrafe.Organismo_di_Controllo Then
                                    Dim recodeRisum = (From ris In GiasContext.G2G_Recode_Contatti Where ris.From_PivaSuperUser = From_PivaSuperUser And
                                                                                                  ris.To_PivaSuperUser = To_PivaSuperUser And
                                                                                                  ris.From_Cod_RisUm = ic.val_cod).FirstOrDefault
                                    If recodeRisum IsNot Nothing Then
                                        codice.val_cod = recodeRisum.To_Cod_Risum
                                    Else
                                        Throw New Exception("Codice Organismo di Controllo azienda non mappato")
                                    End If
                                End If
                                codice.PIVA = To_Piva
                                GiasContext.Imprese_Codici.Add(codice)
                            Next

                            ' inserisco indirizzi impresa
                            For Each ii As G2G_Indirizzo In i.Indirizzi

                                ' nuovo indirizzo
                                Dim idSeq = G2GUtility.NuovoId_Tabella(GiasContext, "Indirizzi", 0, 2000000000, objParametri)
                                Dim indirizzo = Gias_EF_Utility.CopyEntity(GiasContext, ii.Indirizzo, Nothing, username, data)
                                indirizzo.cod_indirizzo = idSeq
                                GiasContext.Indirizzi.Add(indirizzo)

                                ' nuovo indirizzo contatto
                                Dim ixi = New ImpresexIndirizzi With {
                                    .PIVA = impresa.PIVA,
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
                                GiasContext.ImpresexIndirizzi.Add(ixi)

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

                            If i.ContattoImpresa IsNot Nothing Then

                                ' nuovo contatto impresa

                                Dim contatto = Gias_EF_Utility.CopyEntity(GiasContext, i.ContattoImpresa.Contatto, Nothing, username, data)
                                contatto.Piva = To_PivaSuperUser
                                contatto.Cod_Contatto = impresa.PIVA

                                Dim esisteContatto As Boolean = False
                                If (From c In GiasContext.Contatti Where c.Cod_Contatto = impresa.PIVA AndAlso c.Piva = To_PivaSuperUser).FirstOrDefault IsNot Nothing Then
                                    esisteContatto = True
                                End If

                                If Not esisteContatto Then
                                    GiasContext.Contatti.Add(contatto)
                                End If


                                ' inserisco risorse umane contatto impresa
                                For Each r As Risorse_Umane In i.ContattoImpresa.RisorseUmane

                                    ' nuova risorsa numana
                                    Dim esisteRisorsa As Boolean = False
                                    Dim r_check As Risorse_Umane
                                    Dim To_Cod_Risum As Integer
                                    If esisteContatto Then

                                        r_check = (From rr In GiasContext.Risorse_Umane Where rr.Piva = To_PivaSuperUser _
                                                                                               AndAlso rr.Cod_Contatto = impresa.PIVA _
                                                                                               AndAlso rr.Cod_Rapporto = r.Cod_Rapporto).FirstOrDefault

                                        If r_check IsNot Nothing Then
                                            esisteRisorsa = True
                                        End If

                                    End If

                                    If Not esisteRisorsa Then
                                        Dim idSeq = G2GUtility.NuovoId_Tabella(GiasContext, "Risorse_Umane", 0, 2000000000, objParametri)
                                        Dim risorsa = Gias_EF_Utility.CopyEntity(GiasContext, r, Nothing, username, data)
                                        To_Cod_Risum = idSeq
                                        risorsa.Piva = To_PivaSuperUser
                                        risorsa.Cod_Contatto = impresa.PIVA
                                        risorsa.Cod_RisUm = To_Cod_Risum
                                        GiasContext.Risorse_Umane.Add(risorsa)
                                    Else
                                        To_Cod_Risum = r_check.Cod_RisUm
                                    End If

                                    Dim esisteRecodeRisorsa As Boolean = False
                                    If esisteRisorsa Then

                                        Dim check_recode = (From rec In GiasContext.G2G_Recode_Contatti Where rec.From_PivaSuperUser = From_PivaSuperUser _
                                                                                                         AndAlso rec.To_PivaSuperUser = To_PivaSuperUser _
                                                                                                         AndAlso rec.From_Cod_RisUm = r.Cod_RisUm _
                                                                                                         AndAlso rec.To_Cod_Risum = To_Cod_Risum).FirstOrDefault

                                        If check_recode IsNot Nothing Then
                                            esisteRecodeRisorsa = True
                                        End If

                                    End If

                                    If Not esisteRecodeRisorsa Then
                                        ' nuovo recode contatto
                                        Dim recode_contatto =
                                            New G2G_Recode_Contatti With {
                                                .From_PivaSuperUser = From_PivaSuperUser,
                                                .To_PivaSuperUser = To_PivaSuperUser,
                                                .From_Cod_RisUm = r.Cod_RisUm,
                                                .To_Cod_Risum = To_Cod_Risum,
                                                .Username_Creazione = username,
                                                .Username_Modifica = username,
                                                .Data_Creazione = Now(),
                                                .Data_Modifica = Now(),
                                                .Validita_Inizio = AGRODATAINIZIO,
                                                .Validita_Fine = AGRODATAFINE,
                                                .inviato = 0,
                                                .datainvio = Now()
                                            }
                                        g2g.Recode.G2GRecodeContattiToInsert.Add(recode_contatto)
                                        GiasContext.G2G_Recode_Contatti.Add(recode_contatto)
                                    End If

                                Next

                            End If

                        End If

                        'nuovo recode impresa
                        Dim recode =
                            New G2G_Recode_Imprese With {
                                .From_PivaSuperUser = From_PivaSuperUser,
                                .To_PivaSuperUser = To_PivaSuperUser,
                                .FROM_Piva = From_Piva,
                                .To_Piva = To_Piva,
                                .FROM_SaCod = 0,
                                .TO_SaCod = 0,
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

                    Next

                    GiasContext.SaveChanges()

                End If

                ' IMPRESE DA MODIFICARE
                If g2g.ImpreseToUpdate.Count > 0 Then

                    Dim index As Integer = 0

                    ' impresa da modificare
                    For Each i As G2G_Impresa In g2g.ImpreseToUpdate

                        ' modifica recode impresa, se non è presente lo crea
                        Dim recode = (From rr In GiasContext.G2G_Recode_Imprese Where rr.From_PivaSuperUser = From_PivaSuperUser AndAlso rr.To_PivaSuperUser = To_PivaSuperUser AndAlso rr.FROM_Piva = i.Impresa.PIVA AndAlso rr.FROM_SaCod = 0).FirstOrDefault()
                        If recode Is Nothing Then
                            recode = New G2G_Recode_Imprese With {
                                .From_PivaSuperUser = From_PivaSuperUser,
                                .To_PivaSuperUser = To_PivaSuperUser,
                                .FROM_Piva = From_Piva,
                                .To_Piva = To_Piva,
                                .FROM_SaCod = 0,
                                .TO_SaCod = 0,
                                .Username_Creazione = username,
                                .Username_Modifica = username,
                                .Data_Creazione = Now(),
                                .Data_Modifica = Now(),
                                .Validita_Inizio = AGRODATAINIZIO,
                                .Validita_Fine = AGRODATAFINE,
                                .inviato = 0,
                                .datainvio = Now()
                            }
                            GiasContext.G2G_Recode_Imprese.Add(recode)
                            g2g.Recode.G2GRecodeImpreseCentriToUpdate.Add(recode)
                        Else
                            recode.Username_Modifica = username
                            recode.Data_Modifica = data
                            recode.datainvio = data
                            g2g.Recode.G2GRecodeImpreseCentriToUpdate.Add(recode)
                            GiasContext.G2G_Recode_Imprese.Attach(recode)
                            GiasContext.Entry(recode).State = EntityState.Modified
                        End If

                        ' modifica impresa
                        Dim impresa = (From ii In GiasContext.Imprese Where ii.PIVA = recode.To_Piva).FirstOrDefault()
                        impresa = Gias_EF_Utility.CopyEntity(GiasContext, i.Impresa, impresa, username, data)
                        impresa.PIVA = recode.To_Piva
                        GiasContext.Imprese.Attach(impresa)
                        GiasContext.Entry(impresa).State = EntityState.Modified

                        ' TODO: valutare se modificare gerarchia e utente impresa

                        ' cancella codici impresa
                        Dim codici = (From c In GiasContext.Imprese_Codici Where c.PIVA = impresa.PIVA Select c).ToList()
                        For Each c As Imprese_Codici In codici
                            GiasContext.Imprese_Codici.Attach(c)
                            GiasContext.Imprese_Codici.Remove(c)
                        Next

                        ' inserisce codici impresa
                        For Each ic As Imprese_Codici In i.Codici
                            Dim codice = Gias_EF_Utility.CopyEntity(GiasContext, ic, Nothing, username, data)
                            If codice.id_cod = enum_CodiciAnagrafe.Organismo_di_Controllo Then
                                Dim recodeRisum = (From ris In GiasContext.G2G_Recode_Contatti Where ris.From_PivaSuperUser = From_PivaSuperUser And
                                                                                                  ris.To_PivaSuperUser = To_PivaSuperUser And
                                                                                                  ris.From_Cod_RisUm = ic.val_cod).FirstOrDefault
                                If recodeRisum IsNot Nothing Then
                                    codice.val_cod = recodeRisum.To_Cod_Risum
                                Else
                                    Throw New Exception("Codice Organismo di Controllo azienda non mappato")
                                End If
                            End If
                            codice.PIVA = impresa.PIVA
                            GiasContext.Imprese_Codici.Add(codice)
                        Next

                        ' cancello indirizzi impresa
                        Dim impresexindirizzi = (From ii In GiasContext.ImpresexIndirizzi Where ii.PIVA = impresa.PIVA Select ii).ToList()
                        For Each ixi As ImpresexIndirizzi In impresexindirizzi
                            GiasContext.ImpresexIndirizzi.Attach(ixi)
                            GiasContext.ImpresexIndirizzi.Remove(ixi)
                        Next

                        ' inserisco / modifico indirizzi impresa
                        For Each ii As G2G_Indirizzo In i.Indirizzi

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

                            ' nuovo indirizzo impresa
                            Dim ixi = New ImpresexIndirizzi With {
                                .PIVA = impresa.PIVA,
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
                            GiasContext.ImpresexIndirizzi.Add(ixi)

                        Next

                        G2GUtility.SaveChanges(GiasContext, index)

                    Next

                    GiasContext.SaveChanges()

                End If


                ' log importazione
                g2g.Recode.LogRecode = g2g.ImpreseToInsert.Count & " nuovi, " & g2g.ImpreseToUpdate.Count & " modificati, " & g2g.Recode.G2GRecodeImpreseCentriToDelete.Count & " cancellati"

            End Using

        Catch ex As Exception

            messaggioErrore = ex.Message
            g2g.Recode.MessaggioErrore = messaggioErrore
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)

        End Try

        Return g2g.Recode

    End Function

    Public Function Scrivi_ImpreseReverse(ByRef g2g As G2G_Imprese_Reverse, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GImprese_W.Scrivi_ImpreseReverse()"
        Dim messaggioErrore As String = ""

        Dim From_PivaSuperUser = g2g.From_PivaSuperUser
        Dim To_PivaSuperUser = g2g.To_PivaSuperUser
        Dim From_Piva = g2g.From_Piva
        Dim To_Piva = g2g.To_Piva
        Dim To_Piva_Padre = g2g.To_Piva_Padre

        Dim username = objParametri.UsernameOperazione
        Dim data = Date.Now

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try

            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                ' IMPRESE DA INSERIRE
                If g2g.ImpreseToInsert.Count > 0 Then

                    Dim index As Integer = 0

                    For Each i As G2G_Impresa In g2g.ImpreseToInsert

                        ' verifico se l'impresa è già presente in destinazione
                        Dim impresa = (From ii In GiasContext.Imprese Where ii.PIVA = To_Piva).FirstOrDefault()

                        If impresa Is Nothing Then

                            ' nuova impresa
                            impresa = Gias_EF_Utility.CopyEntity(GiasContext, i.Impresa, Nothing, username, data)
                            impresa.PIVA = To_Piva
                            GiasContext.Imprese.Add(impresa)

                            ' inserisce gerarchia impresa
                            Dim gerarchia = Gias_EF_Utility.CopyEntity(GiasContext, i.Gerarchia, Nothing, username, data)
                            gerarchia.Padre = To_Piva_Padre
                            gerarchia.Figlio = To_Piva
                            GiasContext.GerarchiaImprese.Add(gerarchia)

                            ' inserisce utente impresa
                            Dim utente = Gias_EF_Utility.CopyEntity(GiasContext, i.Utente, Nothing, username, data)
                            utente.USER = From_PivaSuperUser
                            utente.PIVA = To_Piva
                            GiasContext.UtentiXImprese.Add(utente)

                            ' inserisce codici impresa
                            For Each ic As Imprese_Codici In i.Codici
                                Dim codice = Gias_EF_Utility.CopyEntity(GiasContext, ic, Nothing, username, data)
                                If codice.id_cod = enum_CodiciAnagrafe.Organismo_di_Controllo Then
                                    Dim recodeRisum = (From ris In GiasContext.G2G_Recode_Contatti Where ris.From_PivaSuperUser = From_PivaSuperUser And
                                                                                                  ris.To_PivaSuperUser = To_PivaSuperUser And
                                                                                                  ris.From_Cod_RisUm = ic.val_cod).FirstOrDefault
                                    If recodeRisum IsNot Nothing Then
                                        codice.val_cod = recodeRisum.To_Cod_Risum
                                    Else
                                        'Throw New Exception("Codice Organismo di Controllo azienda non mappato")
                                    End If
                                End If
                                codice.PIVA = To_Piva
                                GiasContext.Imprese_Codici.Add(codice)
                            Next

                            ' inserisco indirizzi impresa
                            For Each ii As G2G_Indirizzo In i.Indirizzi

                                ' nuovo indirizzo
                                Dim idSeq = G2GUtility.NuovoId_Tabella(GiasContext, "Indirizzi", 0, 2000000000, objParametri)
                                Dim indirizzo = Gias_EF_Utility.CopyEntity(GiasContext, ii.Indirizzo, Nothing, username, data)
                                indirizzo.cod_indirizzo = idSeq
                                GiasContext.Indirizzi.Add(indirizzo)

                                ' nuovo indirizzo contatto
                                Dim ixi = New ImpresexIndirizzi With {
                                    .PIVA = impresa.PIVA,
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
                                GiasContext.ImpresexIndirizzi.Add(ixi)

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

                            If i.ContattoImpresa IsNot Nothing Then

                                ' nuovo contatto impresa
                                Dim contatto = Gias_EF_Utility.CopyEntity(GiasContext, i.ContattoImpresa.Contatto, Nothing, username, data)
                                contatto.Piva = From_PivaSuperUser
                                contatto.Cod_Contatto = impresa.PIVA
                                GiasContext.Contatti.Add(contatto)

                                ' inserisco risorse umane contatto impresa
                                For Each r As Risorse_Umane In i.ContattoImpresa.RisorseUmane

                                    ' nuova risorsa numana
                                    Dim idSeq = G2GUtility.NuovoId_Tabella(GiasContext, "Risorse_Umane", 0, 2000000000, objParametri)
                                    Dim risorsa = Gias_EF_Utility.CopyEntity(GiasContext, r, Nothing, username, data)
                                    risorsa.Piva = From_PivaSuperUser
                                    risorsa.Cod_Contatto = impresa.PIVA
                                    risorsa.Cod_RisUm = idSeq
                                    GiasContext.Risorse_Umane.Add(risorsa)

                                    ' nuovo recode contatto
                                    Dim recode_contatto =
                                        New G2G_Recode_Contatti With {
                                            .From_PivaSuperUser = From_PivaSuperUser,
                                            .To_PivaSuperUser = To_PivaSuperUser,
                                            .From_Cod_RisUm = risorsa.Cod_RisUm,
                                            .To_Cod_Risum = r.Cod_RisUm,
                                            .Username_Creazione = username,
                                            .Username_Modifica = username,
                                            .Data_Creazione = Now(),
                                            .Data_Modifica = Now(),
                                            .Validita_Inizio = AGRODATAINIZIO,
                                            .Validita_Fine = AGRODATAFINE,
                                            .inviato = 0,
                                            .datainvio = Now()
                                        }
                                    g2g.Recode.G2GRecodeContattiToInsert.Add(recode_contatto)
                                    GiasContext.G2G_Recode_Contatti.Add(recode_contatto)

                                Next

                            End If

                        End If

                        'nuovo recode impresa
                        Dim recode =
                            New G2G_Recode_Imprese With {
                                .From_PivaSuperUser = From_PivaSuperUser,
                                .To_PivaSuperUser = To_PivaSuperUser,
                                .FROM_Piva = From_Piva,
                                .To_Piva = To_Piva,
                                .FROM_SaCod = 0,
                                .TO_SaCod = 0,
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

                    Next

                    GiasContext.SaveChanges()

                End If

                ' IMPRESE DA MODIFICARE
                If g2g.ImpreseToUpdate.Count > 0 Then

                    Dim index As Integer = 0

                    ' impresa da modificare
                    For Each i As G2G_Impresa In g2g.ImpreseToUpdate

                        ' modifica recode impresa, se non è presente lo crea
                        Dim recode = (From rr In GiasContext.G2G_Recode_Imprese Where rr.From_PivaSuperUser = From_PivaSuperUser AndAlso rr.To_PivaSuperUser = To_PivaSuperUser AndAlso rr.FROM_Piva = i.Impresa.PIVA AndAlso rr.FROM_SaCod = 0).FirstOrDefault()
                        If recode Is Nothing Then
                            recode = New G2G_Recode_Imprese With {
                                .From_PivaSuperUser = From_PivaSuperUser,
                                .To_PivaSuperUser = To_PivaSuperUser,
                                .FROM_Piva = From_Piva,
                                .To_Piva = To_Piva,
                                .FROM_SaCod = 0,
                                .TO_SaCod = 0,
                                .Username_Creazione = username,
                                .Username_Modifica = username,
                                .Data_Creazione = Now(),
                                .Data_Modifica = Now(),
                                .Validita_Inizio = AGRODATAINIZIO,
                                .Validita_Fine = AGRODATAFINE,
                                .inviato = 0,
                                .datainvio = Now()
                            }
                            GiasContext.G2G_Recode_Imprese.Add(recode)
                            g2g.Recode.G2GRecodeImpreseCentriToUpdate.Add(recode)
                        Else
                            recode.Username_Modifica = username
                            recode.Data_Modifica = data
                            recode.datainvio = data
                            g2g.Recode.G2GRecodeImpreseCentriToUpdate.Add(recode)
                            GiasContext.G2G_Recode_Imprese.Attach(recode)
                            GiasContext.Entry(recode).State = EntityState.Modified
                        End If

                        ' modifica impresa
                        Dim impresa = (From ii In GiasContext.Imprese Where ii.PIVA = recode.To_Piva).FirstOrDefault()
                        impresa = Gias_EF_Utility.CopyEntity(GiasContext, i.Impresa, impresa, username, data)
                        impresa.PIVA = recode.To_Piva
                        GiasContext.Imprese.Attach(impresa)
                        GiasContext.Entry(impresa).State = EntityState.Modified

                        ' TODO: valutare se modificare gerarchia e utente impresa

                        ' cancella codici impresa
                        Dim codici = (From c In GiasContext.Imprese_Codici Where c.PIVA = impresa.PIVA Select c).ToList()
                        For Each c As Imprese_Codici In codici
                            GiasContext.Imprese_Codici.Attach(c)
                            GiasContext.Imprese_Codici.Remove(c)
                        Next

                        ' inserisce codici impresa
                        For Each ic As Imprese_Codici In i.Codici
                            Dim codice = Gias_EF_Utility.CopyEntity(GiasContext, ic, Nothing, username, data)
                            If codice.id_cod = enum_CodiciAnagrafe.Organismo_di_Controllo Then
                                Dim recodeRisum = (From ris In GiasContext.G2G_Recode_Contatti Where ris.From_PivaSuperUser = From_PivaSuperUser And
                                                                                                  ris.To_PivaSuperUser = To_PivaSuperUser And
                                                                                                  ris.From_Cod_RisUm = ic.val_cod).FirstOrDefault
                                If recodeRisum IsNot Nothing Then
                                    codice.val_cod = recodeRisum.To_Cod_Risum
                                Else
                                    Throw New Exception("Codice Organismo di Controllo azienda non mappato")
                                End If
                            End If
                            codice.PIVA = impresa.PIVA
                            GiasContext.Imprese_Codici.Add(codice)
                        Next

                        ' cancello indirizzi impresa
                        Dim impresexindirizzi = (From ii In GiasContext.ImpresexIndirizzi Where ii.PIVA = impresa.PIVA Select ii).ToList()
                        For Each ixi As ImpresexIndirizzi In impresexindirizzi
                            GiasContext.ImpresexIndirizzi.Attach(ixi)
                            GiasContext.ImpresexIndirizzi.Remove(ixi)
                        Next

                        ' inserisco / modifico indirizzi impresa
                        For Each ii As G2G_Indirizzo In i.Indirizzi

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

                            ' nuovo indirizzo impresa
                            Dim ixi = New ImpresexIndirizzi With {
                                .PIVA = impresa.PIVA,
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
                            GiasContext.ImpresexIndirizzi.Add(ixi)

                        Next

                        G2GUtility.SaveChanges(GiasContext, index)

                    Next

                    GiasContext.SaveChanges()

                End If


                ' log importazione
                g2g.Recode.LogRecode = g2g.ImpreseToInsert.Count & " nuovi, " & g2g.ImpreseToUpdate.Count & " modificati, " & g2g.Recode.G2GRecodeImpreseCentriToDelete.Count & " cancellati"

            End Using

        Catch ex As Exception

            messaggioErrore = ex.Message
            g2g.Recode.MessaggioErrore = messaggioErrore
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)

        End Try

        Return g2g.Recode

    End Function

End Class