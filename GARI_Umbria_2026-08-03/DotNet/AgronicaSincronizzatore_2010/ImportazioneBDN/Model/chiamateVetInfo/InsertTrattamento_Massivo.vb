
Public Class InsertTrattamento_Massivo
	Public presrigaNumero As String
	Public tratNote As String
	Public tratDtInizio As String
	Public tratDtFine As String
	Public regscoScarichi As List(Of regSco_Scarico)

	Public Sub New(ByVal PresRiga_Numero As String,
				   ByVal Tratt_Inizio As Date,
				   ByVal Tratt_Fine As Date,
				   ByVal RegSco_Scarichi As List(Of regSco_Scarico),
				   Optional ByVal Tratt_Note As String = "")
		presrigaNumero = PresRiga_Numero
		tratNote = Tratt_Note
		tratDtInizio = Tratt_Inizio.ToString("dd-MM-yyyy")
		tratDtFine = Tratt_Fine.ToString("dd-MM-yyyy")
		regscoScarichi = RegSco_Scarichi
	End Sub

	Public Sub New(ByVal PresRiga_Numero As String,
				   ByVal Tratt_Inizio As Date,
				   ByVal Tratt_Fine As Date,
				   ByVal RegSco_Scarichi As List(Of Tuple(Of String, Double)),
				   Optional ByVal Tratt_Note As String = "")
		Me.New(PresRiga_Numero, Tratt_Inizio, Tratt_Fine,
			   RegSco_Scarichi.Select(Function(scarico) New regSco_Scarico(scarico.Item1, scarico.Item2)).ToList,
			   Tratt_Note)
	End Sub

End Class

Public Class regSco_Scarico
	Public regscoNumero As String
	Public regscoQuantitativo As Double

	Public Sub New(ByVal RegSco_Numero As String,
				   ByVal RegSco_Qta As Double)
		regscoNumero = RegSco_Numero
		regscoQuantitativo = RegSco_Qta
	End Sub

End Class
