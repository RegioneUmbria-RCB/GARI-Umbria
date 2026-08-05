Public Class InsertTrattamento_Protocollo_Semplificato
	Public presNumero As String
	Public tratNote As String
	Public tratDtInizio As String
	Public tratDtFine As String
	Public prescapoLista As List(Of presCapo)
	Public regscoScarico As regSco_Scarico

	Public Sub New(ByVal Pres_Numero As String,
				   ByVal Tratt_Inizio As Date,
				   ByVal Tratt_Fine As Date,
				   ByVal Pres_Capi As List(Of String),
				   ByVal RegSco_Scarichi As regSco_Scarico,
				   Optional ByVal Note As String = "")
		presNumero = Pres_Numero
		tratNote = Note
		tratDtInizio = Tratt_Inizio.ToString("dd-MM-yyyy")
		tratDtFine = Tratt_Fine.ToString("dd-MM-yyyy")
		prescapoLista = New List(Of presCapo)
		regscoScarico = RegSco_Scarichi

		Pres_Capi.ForEach(Sub(identificativo) prescapoLista.Add(New presCapo(identificativo)))
	End Sub

	Public Sub New(ByVal Pres_Numero As String,
				   ByVal Tratt_Inizio As Date,
				   ByVal Tratt_Fine As Date,
				   ByVal Pres_Capi As List(Of presCapo),
				   ByVal RegSco_Scarichi As regSco_Scarico,
				   Optional ByVal Note As String = "")
		presNumero = Pres_Numero
		tratNote = Note
		tratDtInizio = Tratt_Inizio.ToString("dd-MM-yyyy")
		tratDtFine = Tratt_Fine.ToString("dd-MM-yyyy")
		prescapoLista = Pres_Capi
		regscoScarico = RegSco_Scarichi
	End Sub

	Public Sub New(ByVal PresRiga_Numero As String,
				   ByVal Tratt_Inizio As Date,
				   ByVal Tratt_Fine As Date,
				   ByVal Pres_Capi As List(Of String),
				   ByVal RegSco_Scarichi As Tuple(Of String, Double),
				   Optional ByVal Note As String = "")
		Me.New(PresRiga_Numero, Tratt_Inizio, Tratt_Fine, Pres_Capi,
			   New regSco_Scarico(RegSco_Scarichi.Item1, RegSco_Scarichi.Item2),
			   Note)
	End Sub

End Class

