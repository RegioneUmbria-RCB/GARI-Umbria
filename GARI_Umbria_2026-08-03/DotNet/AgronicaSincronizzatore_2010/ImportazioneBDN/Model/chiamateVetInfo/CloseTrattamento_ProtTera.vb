
Public Class CloseTrattamento_ProtTera
	Private Enum enum_StatoCodice
		UNDEFINED = 0
		Aperto = 1
		InCorso = 2
		Chiuso = 3
		Chiuso_Anomalia = 4
	End Enum

	Public presNumero As String
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

	Public Sub New(ByVal Pres_Numero As String,
				   ByVal Data_Fine As String,
				   ByVal Stato As String,
				   ByVal Note As String)
		presNumero = Pres_Numero
		tratDtFine = Data_Fine
		tratStatoCodice = If(IsNumeric(Stato), getStatoCodice(Stato), Stato)
		tratNote = Note
	End Sub

End Class

Public Class CloseTrattamento_ProtTera_Response
	Public tratNumeri As List(Of String)
	Public presRigaNumeri As List(Of String)
End Class

