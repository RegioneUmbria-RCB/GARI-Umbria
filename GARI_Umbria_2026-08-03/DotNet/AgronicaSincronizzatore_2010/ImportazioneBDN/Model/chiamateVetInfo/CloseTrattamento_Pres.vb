
Public Class CloseTrattamento_Pres
	Private Enum enum_StatoCodice
		UNDEFINED = 0
		Aperto = 1
		InCorso = 2
		Chiuso = 3
		Chiuso_Anomalia = 4
	End Enum

	Public tratNumero As String
	Public tratDtFine As String
	Public tratStatoCodice As String
	Public tratNote As String

	Private Function getStatoCodice(ByVal stato As Integer)
		Select Case stato
			Case enum_StatoCodice.Aperto
				Return "APERTO"
			Case enum_StatoCodice.InCorso
				Return "IN_CORSO"
			Case enum_StatoCodice.Chiuso
				Return "CHIUSO"
			Case enum_StatoCodice.Chiuso_Anomalia
				Return "CHIUSO_CON_ANOMALIA"
			Case Else
				Return ""
		End Select
	End Function

	Public Sub New(ByVal Tratt_Numero As String,
				   ByVal Data_Fine As Date,
				   ByVal Stato As Integer,
				   ByVal Note As String)
		tratNumero = Tratt_Numero
		tratDtFine = String.Format("dd-MM-yyyy", Data_Fine)
		tratStatoCodice = getStatoCodice(Stato)
		tratNote = Note
	End Sub

	Public Sub New(ByVal Tratt_Numero As String,
				   ByVal Data_Fine As String,
				   ByVal Stato As String,
				   ByVal Note As String)
		tratNumero = Tratt_Numero
		tratDtFine = Data_Fine
		tratStatoCodice = Stato
		tratNote = Note
	End Sub

End Class

Public Class CloseTrattamento_Pres_Response
	Public presrigaNumero As String
End Class

