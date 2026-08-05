Imports System.Linq
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework_POCO

Module LocalConstants
    Public ReadOnly CANCELLABILE As String = "Cancellabile"
End Module

Public Class GruppiMerce
    Inherits LogProvider


    Public objPServer As AgronicaCoreParametri
    Public objPUtenti As AgronicaCoreParametri

    Sub New(objPServer As AgronicaCoreParametri, objPUtenti As AgronicaCoreParametri)
        Me.objPServer = objPServer
        Me.objPUtenti = objPUtenti
    End Sub

    Public Function GetGruppiMerceConRagioneSociale(piva As String) As DataTable
        Try
            Dim dal As New Gruppi_Merce_R
            Dim dt As DataTable = dal.LeggiConRagioneSociale(piva, objPServer)

            Return dt
        Catch ex As Exception
            If objPServer IsNot Nothing Then
                Dim routine As String = Reflection.MethodBase.GetCurrentMethod().Name
                Scrivi_LOG(objPServer, routine, ex.Message)
            End If

            Throw ex
        End Try
    End Function

    Public Function GetGruppiMerce(piva As String) As DataTable
        Try
            Dim dal As New Gruppi_Merce_R
            Dim dt = dal.Leggi_con_Visibilita(piva, "", "", objPServer)

            Return dt
        Catch ex As Exception
            If objPServer IsNot Nothing Then
                Dim routine As String = Reflection.MethodBase.GetCurrentMethod().Name
                Scrivi_LOG(objPServer, routine, ex.Message)
            End If

            Throw ex
        End Try
    End Function


    Public Function GetGruppiMercePerAnagrafica(piva As String) As DataTable

        Try
            Dim dt As DataTable = GetGruppiMerceConRagioneSociale(piva) ' mi interessa tutti i gruppi merce visibili all'utente connesso
            AbilitaODisabilitaPulsanteCancella(dt)

            Return dt
        Catch ex As Exception
            If objPServer IsNot Nothing Then
                Dim routine As String = Reflection.MethodBase.GetCurrentMethod().Name
                Scrivi_LOG(objPServer, routine, ex.Message)
            End If

            Throw ex
        End Try
    End Function


    Private Sub AbilitaODisabilitaPulsanteCancella(ByRef gruppiMerceDT As DataTable)
        CreaColonnaCancellabile(gruppiMerceDT)

        VerificaVincoliProdottiExtra(gruppiMerceDT)
        VerificaVincoliGruppiMerceDefault(gruppiMerceDT)
    End Sub

    Private Sub CreaColonnaCancellabile(ByRef dt As DataTable)
        dt.Columns.Add(CANCELLABILE, GetType(Boolean))
    End Sub

    Private Sub VerificaVincoliProdottiExtra(ByRef gruppiMerceDT As DataTable)
        Dim dal As New Gruppi_Merce_R
        Dim prodottiExtra = dal.GetProdottiExtraPrivata(objPServer) _
            .GroupBy(Function(prodotto) prodotto.Id_Gruppo_Merce) _
            .Select(Function(prodotto) prodotto.First).ToList()

        For Each row As DataRow In gruppiMerceDT.Rows
            If Not IsDBNull(row(CANCELLABILE)) AndAlso row(CANCELLABILE) = False Then
                Continue For
            End If

            If VincolatoDaUnProdottoExtra(CInt(row("Id_Gruppo_Merce")), prodottiExtra) Then
                row(CANCELLABILE) = False
            Else
                row(CANCELLABILE) = True
            End If
        Next
    End Sub

    Private Sub VerificaVincoliGruppiMerceDefault(ByRef gruppiMerceDT As DataTable)
        Dim dal As New Gruppi_Merce_R
        Dim impreseImpostazioni = dal.GetListGruppiMerceDefault_MultiAzienda(Nothing, objPServer, True) _
                                     .GroupBy(Function(impImposta) impImposta.Id_Gruppo_Merce) _
                                     .Select(Function(gruppo) gruppo.First).ToList()

        For Each row As DataRow In gruppiMerceDT.Rows
            If Not IsDBNull(row(CANCELLABILE)) <> Nothing AndAlso row(CANCELLABILE) = False Then
                Continue For
            End If

            If EProdottoDiDefault(CInt(row("Id_Gruppo_Merce")), impreseImpostazioni) Then
                row(CANCELLABILE) = False
            Else
                row(CANCELLABILE) = True
            End If
        Next
    End Sub

    Private Function VincolatoDaUnProdottoExtra(Id_Gruppo_Merce As Integer, prodottiExtra As List(Of AgronicaCoreEntityFramework_POCO.Prodotti_Extra_Privata)) As Boolean

        Return prodottiExtra.Any(Function(prodotto) prodotto.Id_Gruppo_Merce = Id_Gruppo_Merce)
    End Function

    Private Function EProdottoDiDefault(Id_Gruppo_Merce As Integer, impresaImpostazioni As List(Of ImpostazioneDefault_GruppiMerce)) As Boolean
        Return impresaImpostazioni.Any(Function(impostazione) Id_Gruppo_Merce = impostazione.Id_Gruppo_Merce)
    End Function


    Public Function TentativoCancellazioneGruppoMerce(IdGruppoMerce As Integer, forzaCancellazione As Boolean) As RispostaTentativoCancellazioneGruppoMerce
        Try
            Dim risposta = CancellazioneGruppoMerce(IdGruppoMerce, forzaCancellazione) ' 

            Return risposta
        Catch ex As Exception
            If objPServer IsNot Nothing Then
                Dim routine As String = Reflection.MethodBase.GetCurrentMethod().Name
                Scrivi_LOG(objPServer, routine, ex.Message)
            End If

            Throw ex
        End Try
    End Function

    ''' <summary>
    ''' Se false allora cancellerà il gruppo merce se non esistono permessi collegati. Se esistono permessi 
    ''' collegati allora chiede all'utente di confermare la cancellazione.
    ''' Se true cancellerà il gruppo merce insieme coi permessi collegati senza chiedere la conferma.
    ''' </summary>
    ''' <param name="IdGruppoMerce">Gruppo merce da cancellare</param>
    ''' <param name="forzaCancellazione">Se false, chiede all'utente la conferma della cancellazione.</param>
    Private Function CancellazioneGruppoMerce(IdGruppoMerce As Integer, forzaCancellazione As Boolean) As RispostaTentativoCancellazioneGruppoMerce
        Dim risposta As New RispostaTentativoCancellazioneGruppoMerce

        ' Verifiche che richiedono l'attenzione dell'utente se non passano
        If Not forzaCancellazione Then
            Dim dal As New Gruppi_Merce_R()
            Dim numConflitti = dal.CaricaPermessiCollegatiAlGruppoMerce(IdGruppoMerce, objPServer).Length

            If numConflitti <> 0 Then
                risposta.NumberConflicts = numConflitti
                risposta.Risultato = RisultatoCancellazione.RICHIEDE_CONFERMA_CANCELLAZIONE
                Return risposta
            End If
        End If


        ' Verifiche che sono fatali se non passano
        Dim vincolatoDaUnProdottoExtra = VerificaVincoliProdottiExtra(IdGruppoMerce)
        Dim vincolatoDaUnGruppoMerceDefault = VerificaVincoliGruppiMerceDefault(IdGruppoMerce)

        If vincolatoDaUnProdottoExtra OrElse vincolatoDaUnGruppoMerceDefault Then
            risposta.Risultato = RisultatoCancellazione.FAILURE
            Return risposta
        End If


        ' Esegui la cancellazione
        AvviaTransazioneECancellaGruppoMerceInsiemeAiPermessi(IdGruppoMerce, forzaCancellazione)
        risposta.Risultato = RisultatoCancellazione.SUCCESS
        Return risposta
    End Function

    Private Sub AvviaTransazioneECancellaGruppoMerceInsiemeAiPermessi(idGruppoMerce As Integer, deveAncoraVerificarePermessi As Boolean)



        If objPServer.objConnessione Is Nothing Then
            objPServer.objConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objPServer.StringaConnessione)
            objPServer.objConnessione.Open()
        ElseIf objPServer.objConnessione.State = ConnectionState.Closed Then
            objPServer.objConnessione.ConnectionString = DataProviderFactory.Instance.AggiustaStringaDiConnessione(objPServer.StringaConnessione)
            objPServer.objConnessione.Open()
        End If

        Using transaction = objPServer.objConnessione.BeginTransaction
            Try
                Dim permessi As Gruppi_UtenteXGruppi_Merce() = New Gruppi_UtenteXGruppi_Merce() {}
                If deveAncoraVerificarePermessi Then
                    Dim dal As New Gruppi_Merce_R()
                    permessi = dal.CaricaPermessiCollegatiAlGruppoMerce(idGruppoMerce, objPServer)
                End If
                CancellaGruppoMerceInsiemeAiPermessi(idGruppoMerce, permessi)

                transaction.Commit()
            Catch ex As Exception
                transaction.Rollback()
                Throw ex
            End Try
        End Using

    End Sub

    Private Sub CancellaGruppoMerceInsiemeAiPermessi(idGruppoMerce As Integer, permessi As Gruppi_UtenteXGruppi_Merce())
        Dim dal As New Gruppi_Merce_W(objPServer, objPUtenti)
        For Each permesso In permessi
            dal.CancellaPermeeso(permesso)
        Next

        dal.CancellaGruppoMerce(idGruppoMerce)
    End Sub


    Private Function VerificaVincoliProdottiExtra(ByRef gruppoMerceId As Integer) As Boolean
        Dim dal As New Gruppi_Merce_R
        Dim prodottiExtra = dal.GetProdottiExtraPrivata(objPServer) _
            .GroupBy(Function(prodotto) prodotto.Id_Gruppo_Merce) _
            .Select(Function(prodotto) prodotto.First).ToList()

        Return VincolatoDaUnProdottoExtra(gruppoMerceId, prodottiExtra)
    End Function

    Private Function VerificaVincoliGruppiMerceDefault(ByRef gruppoMerceId As Integer) As Boolean
        Dim dal As New Gruppi_Merce_R
        Dim impreseImpostazioni = dal.GetListGruppiMerceDefault_MultiAzienda(Nothing, objPServer, True) _
                                     .GroupBy(Function(impImposta) impImposta.Id_Gruppo_Merce) _
                                     .Select(Function(gruppo) gruppo.First).ToList()

        Return EProdottoDiDefault(gruppoMerceId, impreseImpostazioni)
    End Function

    Public Sub AggiungiOModificaGruppoMerce(gruppoMerce As GruppoMerce)

        EseguiVerificheCorrettezzaDati(gruppoMerce)


        Dim dal As New Gruppi_Merce_W(objPServer, objPUtenti)
        If gruppoMerce.Id_Gruppo_Merce Is Nothing Then
            dal.AggiungiGruppoMerce(gruppoMerce.ToPoco())
        Else
            dal.ModificaGruppoMerce(gruppoMerce.ToPoco())
        End If
    End Sub

    Private Sub EseguiVerificheCorrettezzaDati(gruppo As GruppoMerce)
        VerificaUnivocitaCodice(gruppo)

        VerificaUtilizzoInProdottiExtra(gruppo)
        VerificaGruppoMerceDiDefault(gruppo)
    End Sub

    Private Sub VerificaUnivocitaCodice(gruppo As GruppoMerce)
        Dim dal As New Gruppi_Merce_R
        Dim dt As New DataTable

        If gruppo.Sa_Cod = Visibilita.PUBBLICO Then
            dt = dal.Leggi_con_Visibilita("", "", "", objPServer)
        ElseIf gruppo.Sa_Cod = Visibilita.PRIVATO Then
            dt = dal.Leggi_con_Visibilita(gruppo.Piva, "", "", objPServer)
        End If

        For Each row As DataRow In dt.Rows
            If gruppo.Id_Gruppo_Merce IsNot Nothing AndAlso CInt(row("Id_Gruppo_Merce")) = gruppo.Id_Gruppo_Merce Then
                Continue For
            End If

            If String.Compare(gruppo.Codice, CStr(row("Codice"))) = 0 Then
                Throw New CodiceDuplicatoEccezione
            End If
        Next
    End Sub

    ''' <summary>
    ''' Verifica se il gruppo merce che sta per essere aggiunto/aggiornato è utilizzato da un prodotto extra.
    ''' In quel caso sarà mostrato un messaggio all'utente tramite un'eccezione.
    ''' </summary>
    Private Sub VerificaUtilizzoInProdottiExtra(gruppo As GruppoMerce)

        ' TODO_RV aggiungere un flag che dice se lo stato precedente di questo oggetto è stato Pubblico
        If NuovoGruppo(gruppo) OrElse StatoAttuale(gruppo, Visibilita.PUBBLICO) Then
            Return
        End If

        Dim dal As New Gruppi_Merce_R
        Dim prodottiExtra = dal.GetProdottiExtraPrivata(objPServer) _
            .GroupBy(Function(prodotto) New With {prodotto.Id_Gruppo_Merce, prodotto.Piva}) _
            .Select(Function(prodotto) prodotto.First).ToList()

        ' TODO_RV Da chiedere se questa parte è sbagliata
        ' Dim utilizzatoDaUnProdotto = prodottiExtra.Where(Function(extra) String.Compare(extra.Piva, gruppo.Piva) = 0 AndAlso extra.Id_Gruppo_Merce = gruppo.Id_Gruppo_Merce).Any()
        Dim utilizzatoDaUnProdotto = prodottiExtra.Where(Function(extra) String.Compare(extra.Piva, gruppo.Piva) <> 0 AndAlso extra.Id_Gruppo_Merce = gruppo.Id_Gruppo_Merce).Any()
        If utilizzatoDaUnProdotto Then
            Throw New GruppoUtilizzatoDaUnProdottoExtraEccezione()
        End If
    End Sub

    Private Function NuovoGruppo(gruppo As GruppoMerce) As Boolean
        Return gruppo.Id_Gruppo_Merce Is Nothing
    End Function

    Private Function StatoAttuale(gruppo As GruppoMerce, visibilita As Visibilita) As Boolean
        Return gruppo.Sa_Cod = visibilita
    End Function

    Private Sub VerificaGruppoMerceDiDefault(gruppoMerce As GruppoMerce)

        ' TODO_RV aggiungere un flag che dice se lo stato precedente di questo oggetto è stato Pubblico
        If NuovoGruppo(gruppoMerce) OrElse StatoAttuale(gruppoMerce, Visibilita.PUBBLICO) Then
            Return
        End If

        Dim dal As New Gruppi_Merce_R
        Dim impreseImpostazioni = dal.GetListGruppiMerceDefault_MultiAzienda(Nothing, objPServer, True) _
                                     .GroupBy(Function(impImposta) impImposta.Id_Gruppo_Merce) _
                                     .Select(Function(gruppo) gruppo.First).ToList()

        For Each impostazione In impreseImpostazioni
            If String.Compare(impostazione.Piva, gruppoMerce.Piva) = 0 Then
                Continue For
            End If

            If impostazione.Id_Gruppo_Merce = gruppoMerce.Id_Gruppo_Merce Then
                Throw New UtilizzatoDaUnAltraImpresaComeGruppoMerceDefaultEccezione()
            End If
        Next
    End Sub
End Class
