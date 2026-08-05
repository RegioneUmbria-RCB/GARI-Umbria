Imports ImportazioneBDN

Namespace VetInfoResponseModel

	Public Class ErrorCodesList

		Private Const E_500 As String = "Errore imprevisto."
		Private Const E_400 As String = "Richiesta errata."
		Private Const E_403 As String = "Richiesta non autorizzata."
		Private Const E_404 As String = "Servizio non trovato."
		Private Const E_NO_DATA As String = "La richiesta è vuota."
		Private Const E_UNKNOWN As String = "Errore sconosciuto."
		Public Const E_BUSINESS As String = "Errore nella procedura."
		Private Const E_LIMIT As String = "Raggiunto il numero massimo di records ottenibili."
		Private Const E_MANDATORY As String = "Il campo deve essere valorizzato."
		Private Const E_INVALID As String = "Il campo non ha un valore valido."
		Private Const E_ACL As String = "Operazione non autorizzata."
		Private Const E_FORMAT As String = "Il campo non ha un formato valido."
		Private Const E_LIMIT_OUTPUT As String = "Raggiunto il numero massimo di records ottenibili."
		Private Const E_AMBIGUOUS As String = "La valorizzazione di alcuni campi genera ambiguità."
		Private Const E_CUSTOMIZED As String = "Errore con messaggio specializzato per l'errore."
		Private Const E_LENGTH As String = "Il campo non rispetta la lunghezza fissata."
		Private Const E_MAX_LENGTH As String = "Il campo ha superato la lunghezza massima consentita."
		Private Const E_MIN_LENGTH As String = "Il campo non ha la lunghezza minima consentita."
		Private Const E_DUPLICATES As String = "Il campo o la combinazione dei campi genera duplicati non ammessi."

		Public Shared ReadOnly ErrorsDict As New Dictionary(Of String, String) From {
			{NameOf(E_500), E_500},
			{NameOf(E_400), E_400},
			{NameOf(E_403), E_403},
			{NameOf(E_404), E_404},
			{NameOf(E_NO_DATA), E_NO_DATA},
			{NameOf(E_UNKNOWN), E_UNKNOWN},
			{NameOf(E_BUSINESS), E_BUSINESS},
			{NameOf(E_LIMIT), E_LIMIT},
			{NameOf(E_MANDATORY), E_MANDATORY},
			{NameOf(E_INVALID), E_INVALID},
			{NameOf(E_ACL), E_ACL},
			{NameOf(E_FORMAT), E_FORMAT},
			{NameOf(E_LIMIT_OUTPUT), E_LIMIT_OUTPUT},
			{NameOf(E_AMBIGUOUS), E_AMBIGUOUS},
			{NameOf(E_CUSTOMIZED), E_CUSTOMIZED},
			{NameOf(E_LENGTH), E_LENGTH},
			{NameOf(E_MAX_LENGTH), E_MAX_LENGTH},
			{NameOf(E_MIN_LENGTH), E_MIN_LENGTH},
			{NameOf(E_DUPLICATES), E_DUPLICATES}
		}

		Public Shared Function GetDesFromCod(ByVal cod As String)
			Dim desc As String = ""
			ErrorsDict.TryGetValue(cod, desc)
			Return desc
		End Function

	End Class

	Public Class WarningCodesList

		Private Const W_TRAT_DEL_SCORTA As String = "Attenzione: cancellando il trattamento cancellerai anche gli scarichi associati."
		Private Const W_TRAT_REOPEN As String = "Attenzione: aprendo il trattamento non tutti i residui dei farmaci saranno disponibili in quanto riutilizzati in altri trattamenti."
		Private Const W_TRATTAMENTORIGA_DEL_SCORTA As String = "Attenzione: cancellando la somministrazione cancellerai anche gli scarichi associati."
		Private Const W_SCORTA_MANUALE_UNIMIS As String = "Attenzione: sono stati selezionati prodotti e/o unità di misura differenti, il sistema non è in grado di garantire il controllo sulle quantità inserite."
		Private Const E_WARN_PROTOCOLLO_TRATTAMENTI As String = "Attenzione: sono presenti dei trattamenti collegati al protocollo."
		Private Const E_WARN_GIACENZA As String = "Attenzione: farmaco non in giacenza."
		Private Const E_WARN_PRESCAPO_IDENTIFICATIVO As String = "Attenzione: anomalia identificativo capo."
		Private Const E_WARN_PRESCAPO_NUMERO_ANIMALI As String = "Attenzione: numero animali indicati in eccedenza rispetto a quanto presente nella banca dati (avicoli)."
		Private Const E_WARN_PRESCAPO_SESSO As String = "Attenzione: sesso indicato discordante (avicoli)."

		Public Shared ReadOnly WarningsDict As New Dictionary(Of String, String) From {
			{NameOf(W_TRAT_DEL_SCORTA), W_TRAT_DEL_SCORTA},
			{NameOf(W_TRAT_REOPEN), W_TRAT_REOPEN},
			{NameOf(W_TRATTAMENTORIGA_DEL_SCORTA), W_TRATTAMENTORIGA_DEL_SCORTA},
			{NameOf(W_SCORTA_MANUALE_UNIMIS), W_SCORTA_MANUALE_UNIMIS},
			{NameOf(E_WARN_PROTOCOLLO_TRATTAMENTI), E_WARN_PROTOCOLLO_TRATTAMENTI},
			{NameOf(E_WARN_GIACENZA), E_WARN_GIACENZA},
			{NameOf(E_WARN_PRESCAPO_IDENTIFICATIVO), E_WARN_PRESCAPO_IDENTIFICATIVO},
			{NameOf(E_WARN_PRESCAPO_NUMERO_ANIMALI), E_WARN_PRESCAPO_NUMERO_ANIMALI},
			{NameOf(E_WARN_PRESCAPO_SESSO), E_WARN_PRESCAPO_SESSO}
		}

		Public Shared Function GetDesFromCod(ByVal cod As String)
			Dim desc As String = ""
			WarningsDict.TryGetValue(cod, desc)
			Return desc
		End Function

	End Class

	Public Class VetInfoResponse(Of T)
		Public errors As List(Of VetInfoError_Response)
		Public warnings As List(Of VetInfoWarning_Response)
		Public pagination As VetInfoPagination_Response
		Public data As List(Of T)
		Public success As Boolean

		Public Function GetErrorMessage()
			Dim [error] = errors.First

			If Not String.IsNullOrEmpty([error].field) AndAlso
			   Not String.IsNullOrEmpty([error].message) Then Return $"Il campo {[error].field} ha generato il seguente avviso: {[error].message}"

			If Not String.IsNullOrEmpty([error].code) Then
				If Not String.IsNullOrEmpty([error].message) Then
					Return $"{ErrorCodesList.GetDesFromCod([error].code)} {[error].message}"
				Else
					Return $"{ErrorCodesList.GetDesFromCod([error].code)}"
				End If
			End If

			Return String.Empty
		End Function

		Public Function GetWarningMessage()
			Dim warning = warnings.First

			If Not String.IsNullOrEmpty(warning.field) AndAlso
			   Not String.IsNullOrEmpty(warning.message) Then Return $"Il campo {warning.field} ha generato il seguente avviso: {warning.message}"

			If Not String.IsNullOrEmpty(warning.code) Then Return $"{WarningCodesList.GetDesFromCod(warning.code)}"

			Return String.Empty
		End Function

		Public Function GetWarningOrErrorMessage()
			If Not IsNothing(errors) Then Return GetErrorMessage()

			If Not IsNothing(warnings) Then Return GetWarningMessage()

			Return String.Empty
		End Function

	End Class

	Public Class VetInfoPagination_Response
		Inherits VetInfoModel.VetInfoPagination
		Public records As Integer
		Public totalRecords As Integer

	End Class

	Public Class VetInfoError_Response
		Public field As String
		Public code As String
		Public message As String
		Public index As Integer?
	End Class

	Public Class VetInfoWarning_Response
		Public code As String
		Public message As String
		Public field As String
		Public index As Integer?
	End Class

	Public Class VetInfoData_Response
		Public extra1 As String
		Public extra2 As String
		Public extra3 As String
		Public extra4 As String
		Public extraObject As String
	End Class

End Namespace

