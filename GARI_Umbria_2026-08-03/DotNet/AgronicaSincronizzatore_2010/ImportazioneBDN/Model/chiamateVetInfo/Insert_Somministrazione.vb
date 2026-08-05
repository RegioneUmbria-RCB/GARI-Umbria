
Public Class Insert_Somministrazione
	Public presrigaNumero As String
	Public somNote As String
	Public somDtEvento As String
	Public regscoScarichi As List(Of regSco_Scarico)
	Public prescapoRiduzioni As List(Of presCapoRiduzione)
	Public warnDisabled As Boolean

	Public Sub New(ByVal PresRiga_Numero As String,
				   ByVal Somm_Data As Date,
				   ByVal RegSco_Scarichi As List(Of regSco_Scarico),
				   Optional ByVal Note As String = "",
				   Optional ByVal Riduz_Capi As List(Of presCapoRiduzione) = Nothing,
				   Optional ByVal Warn_Disabled As Boolean = False)
		presrigaNumero = PresRiga_Numero
		somDtEvento = Somm_Data.ToString("dd-MM-yyyy")
		If Riduz_Capi IsNot Nothing Then
			prescapoRiduzioni = Riduz_Capi
		End If
		regscoScarichi = RegSco_Scarichi
		somNote = Note

		'If somNote = "" AndAlso Not IsNothing(prescapoRiduzioni) AndAlso prescapoRiduzioni.Count > 0 Then
		'	somNote = $"{If(prescapoRiduzioni.Count = 1, "Escluso capo", "Esclusi capi")} {String.Join(", ", Riduz_Capi)} dalla somministrazione."
		'End If

		warnDisabled = Warn_Disabled
	End Sub

	Public Sub New(ByVal PresRiga_Numero As String,
				   ByVal Somm_Data As Date,
				   ByVal RegSco_Scarichi As List(Of Tuple(Of String, Double)),
				   Optional ByVal Riduz_Capi As List(Of presCapoRiduzione) = Nothing,
				   Optional ByVal Note As String = "",
				   Optional ByVal Warn_Disabled As Boolean = False)
		Me.New(PresRiga_Numero, Somm_Data, RegSco_Scarichi.Select(Function(scarico) New regSco_Scarico(scarico.Item1, scarico.Item2)).ToList, Note, Riduz_Capi)
	End Sub

End Class

Public Class presCapoRiduzione
	Public prescapoNumero As String
	Public prescapoNumeroAnimali As Integer?

	''' <summary>
	''' Se non si tratta di un gruppo, e' richiesto solo l'identificativo del capo.
	''' </summary>
	''' <param name="identificativo"></param>
	Public Sub New(identificativo As String)
		prescapoNumero = identificativo
	End Sub

	Public Sub New(identificativo As String, numero As Integer)
		prescapoNumero = identificativo
		prescapoNumeroAnimali = numero
	End Sub

End Class

Public Class Insert_Somministrazione_Response
	Public tratNumero As String
	Public somNumero As String
	Public presrigaNumero As String
End Class
