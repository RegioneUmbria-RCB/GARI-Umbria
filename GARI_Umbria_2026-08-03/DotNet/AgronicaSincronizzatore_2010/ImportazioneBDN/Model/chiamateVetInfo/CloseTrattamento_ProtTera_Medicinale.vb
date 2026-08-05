
Public Class CloseTrattamento_ProtTera_Medicinale
	Private Enum enum_StatoCodice
		UNDEFINED = 0
		Chiuso = 1
		Chiuso_Anomalia = 2
	End Enum

	Private Function getStatoCodice(ByVal stato As Integer)
		Select Case stato
			Case enum_StatoCodice.Chiuso
				Return "CHIUSO"
			Case enum_StatoCodice.Chiuso_Anomalia
				Return "CHIUSO CON ANOMALIA"
			Case Else
				Return ""
		End Select
	End Function

	Public Class Trattamento

		Public tratNumero As String
		Public tratDtFine As String

		Public Sub New(ByVal Tratt_Numero As String,
					   ByVal Data_Fine As Date)
			tratNumero = Tratt_Numero
			tratDtFine = String.Format("dd-MM-yyyy", Data_Fine)
		End Sub

		Public Sub New(ByVal Tratt_Numero As String,
					   ByVal Data_Fine As String)
			tratNumero = Tratt_Numero
			tratDtFine = Data_Fine
		End Sub

	End Class

	Public presNumero As String
	Public tratNumeri As List(Of Trattamento)
	Public tratStatoCodice As String
	Public tratNote As String

	Public Sub New(ByVal Pres_Numero As String,
				   ByVal Trattamenti As List(Of Trattamento),
				   ByVal Stato As Integer,
				   ByVal Note As String)
		presNumero = Pres_Numero
		tratNumeri = Trattamenti
		tratStatoCodice = getStatoCodice(Stato)
		tratNote = Note
	End Sub

	Public Sub New(ByVal Pres_Numero As String,
				   ByVal Trattamenti As List(Of Trattamento),
				   ByVal Stato As String,
				   ByVal Note As String)
		presNumero = Pres_Numero
		tratNumeri = Trattamenti
		tratStatoCodice = Stato
		tratNote = Note
	End Sub

End Class

Public Class CloseTrattamento_ProtTera_Medicinale_Response
	Public tratNumeri As List(Of String)
	Public presRigaNumeri As List(Of String)
	Public tratDtfine As List(Of String)
End Class
