Imports System.Net
Imports AgronicaCoreDataSTD
Imports AgronicaCoreDataProvider.My.Resources

Public Class Util

    Public Function CheckRequest(objRequest As AgronicaCoreDTOStd.InData.NewAgri.RequestNDistribuito, ByRef errorMessage As String) As Boolean

        errorMessage = ""

        If objRequest Is Nothing Then

            errorMessage = Gias.JsonNonValorizzato
            Return False

        Else

            If objRequest.CUAA = "" Then
                errorMessage = Gias.CuaaNonValorizzato
                Return False
            End If

            If IsNothing(objRequest.ElencoAppezzamenti) OrElse (objRequest.ElencoAppezzamenti IsNot Nothing AndAlso objRequest.ElencoAppezzamenti.Count = 0) Then

                errorMessage = Gias.DatiAppezzamentiNonValorizzati
                Return False
            Else
                For Each appezzamento In objRequest.ElencoAppezzamenti
                    If appezzamento.Chiave_Appezzamento = "" Then
                        errorMessage = String.Format(Gias.AppezzamentoIndexErrore_, objRequest.ElencoAppezzamenti.IndexOf(appezzamento))
                        Return False
                    End If
                Next
            End If
        End If

        Return True
    End Function

    Public Function CheckRequest(objRequest As AgronicaCoreDTOStd.InData.NewAgri.RequestUtente, ByRef errorMessage As String) As Boolean

        errorMessage = ""

        If objRequest Is Nothing Then

            errorMessage = Gias.JsonNonValorizzato
            Return False

        Else

            If objRequest.username = "" Then
                errorMessage = "Username non valorizzato"
                Return False
            End If

            If objRequest.mail = "" Then
                errorMessage = "Mail non valorizzata"
                Return False
            End If

            If objRequest.ruoli.Length = 0 Then
                errorMessage = "Nessun ruolo specificato"
                Return False
            End If

        End If

        Return True
    End Function

    Public Function CheckRequest(objRequest As AgronicaCoreDTOStd.InData.NewAgri.RequestImportVisibilita, ByRef errorMessage As String) As Boolean
        errorMessage = ""

        If objRequest Is Nothing Then
            errorMessage = Gias.JsonNonValorizzato
            Return False
        end If
        If objRequest.username = "" Then
            errorMessage = "Username non valorizzato"
            Return False
        End If
        Dim aziendeSenzaMandatoPresenti = True
        If IsNothing(objRequest.aziendeSenzaMandato) OrElse Not objRequest.aziendeSenzaMandato.Any() Then
            aziendeSenzaMandatoPresenti = False
        End If
        Dim aziendeConMandatoPresenti = True
        If IsNothing(objRequest.aziendeConMandato) OrElse Not objRequest.aziendeConMandato.Any() Then
            aziendeConMandatoPresenti = False
        End If

        If (aziendeConMandatoPresenti = False) AndAlso (aziendeSenzaMandatoPresenti = False) Then
            errorMessage = "Non è stato valorizzato almeno 1 dei seguenti campi: aziendeConMandato, aziendeSenzaMandato"
            Return False
        End If

        If IsNothing(objRequest.gruppiUtente) Then
            errorMessage = "Gruppo utente non valorizzato"
            Return False
        End If      

        Return True
    End Function

End Class
