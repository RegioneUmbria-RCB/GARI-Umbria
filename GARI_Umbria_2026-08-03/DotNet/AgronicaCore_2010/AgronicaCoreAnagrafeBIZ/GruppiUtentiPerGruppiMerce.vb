Imports System.Linq
Imports System.Transactions
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider

Public Class GruppiUtentiPerGruppiMerce
    Inherits LogProvider


    Public objPServer As AgronicaCoreParametri
    Public objPUtenti As AgronicaCoreParametri

    Sub New(objPServer As AgronicaCoreParametri, objPUtenti As AgronicaCoreParametri)
        Me.objPServer = objPServer
        Me.objPUtenti = objPUtenti
    End Sub

    Public Function ScriviGruppiUtentiPerGruppiMercePolicy(piva As String, righeSelezionate As RigheSelezionate) As ScriviDatiResult
        Try

            Dim _numElementiInseriti = VerificaValiditaDatiPoiScrivili(righeSelezionate, piva)

            Dim dal As New Gruppi_UtenteXGruppi_Merce_R(objPServer, objPUtenti)

            ' TODO_RV Da ragionare come gestire il caso in cui la piva non viene inoltrata.
            ' In questo caso dobbiamo per forza mostrare tutti i permessi di tutte le imprese (pive) che sono visibili
            ' all'utente connesso.

            Return New ScriviDatiResult With {
                .DT = LeggiPermessiVisibiliAllUtente(piva),
                .NumElementiInseriti = _numElementiInseriti
            }
        Catch ex As Exception
            If objPServer IsNot Nothing Then
                Dim routine As String = Reflection.MethodBase.GetCurrentMethod().Name
                Scrivi_LOG(objPServer, routine, ex.Message)
            End If

            Throw ex
        End Try
    End Function



    Private Function VerificaValiditaDatiPoiScrivili(righeSelezionate As RigheSelezionate, piva As String) As Integer

        If Not DatiSonoValidi(righeSelezionate) Then
            Return NO_IMPORTED_ELEMENTS
        End If

        Return ScriviProdottoCartesiano(righeSelezionate.Gruppi_Utente_codici, righeSelezionate.Ids_Gruppo_Merce, piva)
    End Function

    Public Function LeggiPermessiVisibiliAllUtente(piva As String) As DataTable
        ' Se una piva è inoltrata allora leggo soltanto i dati specifici a quella piva
        ' Se la piva è vuota, allora leggo tutti i dati disponibili per l'utente connesso

        Dim dal As New Gruppi_UtenteXGruppi_Merce_R(objPServer, objPUtenti)
        Return dal.LeggiPermessiVisibiliAllUtenteConnesso(piva)
    End Function


    Private Function ScriviProdottoCartesiano(Gruppi_Utente_codes As Integer(), Ids_Gruppo_Merce As Integer(), piva As String) As Integer
        Dim scopeOption As New TransactionScopeOption
        Dim transactionOptions As New TransactionOptions
        transactionOptions.IsolationLevel = IsolationLevel.ReadUncommitted
        Dim numElementiInseriti = 0

        Using scope As New TransactionScope(scopeOption, transactionOptions)
            Dim dal As New Gruppi_UtenteXGruppi_Merce_W(objPServer, objPUtenti)
            For Each utenteCod In Gruppi_Utente_codes
                For Each idMerce In Ids_Gruppo_Merce
                    If Not dal.RecordExists(utenteCod, idMerce, piva) Then
                        dal.Scrivi(utenteCod, idMerce, piva)
                        numElementiInseriti += 1
                    End If
                Next
            Next

            scope.Complete()
            scope.Dispose()
        End Using
        Return numElementiInseriti
    End Function

    Private Function DatiSonoValidi(righeSelezionate As RigheSelezionate) As Boolean
        If righeSelezionate.Gruppi_Utente_codici Is Nothing AndAlso righeSelezionate.Ids_Gruppo_Merce Is Nothing Then
            Return False
        End If

        If righeSelezionate.Gruppi_Utente_codici.Length = 0 AndAlso righeSelezionate.Ids_Gruppo_Merce.Length = 0 Then
            Return False
        End If

        If righeSelezionate.Gruppi_Utente_codici.Length = 0 OrElse righeSelezionate.Ids_Gruppo_Merce.Length = 0 Then
            Return False
        End If

        Return True
    End Function


    Public Function CancellaPermessiPolicy(permessi As ChiaveCancellaPermesso()) As Integer
        Try
            CancellaPermessi(permessi)
            Return SUCCESS
        Catch ex As Exception
            If objPServer IsNot Nothing Then
                Dim routine As String = Reflection.MethodBase.GetCurrentMethod().Name
                Scrivi_LOG(objPServer, routine, ex.Message)
            End If

            Throw ex
        End Try
    End Function

    Private Sub CancellaPermessi(permessi As ChiaveCancellaPermesso())
        Dim scopeOption As New TransactionScopeOption
        Dim transactionOptions As New TransactionOptions
        transactionOptions.IsolationLevel = IsolationLevel.ReadUncommitted

        Using scope As New TransactionScope(scopeOption, TransactionOptions)
            Dim dal As New Gruppi_UtenteXGruppi_Merce_W(objPServer, objPUtenti)

            For Each permesso In permessi
                dal.Cancella(permesso.Gruppi_Utente_cod, permesso.Id_Gruppo_Merce, permesso.Piva)
            Next

            scope.Complete()
            scope.Dispose()
        End Using
    End Sub
End Class
