
Public Class InsertTrattamento_ProtTera
	Public presNumero As String
	Public prescapoLista As List(Of presCapo)

	Public Sub New(ByVal Pres_Numero As String, ByVal Pres_Capi As List(Of String))
		presNumero = Pres_Numero
		prescapoLista = New List(Of presCapo)

		Pres_Capi.ForEach(Sub(identificativo) prescapoLista.Add(New presCapo(identificativo)))
	End Sub

	Public Sub New(ByVal Pres_Numero As String, ByVal Pres_Capi_Gruppo As List(Of presCapo))
		presNumero = Pres_Numero
		prescapoLista = Pres_Capi_Gruppo
	End Sub

End Class

Public Class presCapo
	Public prescapoIdentificativo As String
	Public prescapoNumeroAnimali As Integer?
	Public prescapoSesso As String

	'Campi al momento non utilizzati
	'Public prescapoSelezioneMassivaCodice As String
	'Public prescapoSelezioneMassivaOperatore As String
	'Public prescapoSelezioneMassivaValore1 As String
	'Public prescapoSelezioneMassivaValore2 As String

	''' <summary>
	''' Se non si tratta di un gruppo,e' richiesto solo l'identificativo del capo.
	''' </summary>
	''' <param name="identificativo"></param>
	Public Sub New(identificativo As String)
		prescapoIdentificativo = identificativo
	End Sub

	''' <summary>
	''' NON UTILIZZATA Utilizzato per gruppi di animali.
	''' </summary>
	''' <param name="identificativo"></param>
	''' <param name="sesso"></param>
	''' <param name="numAnimali"></param>
	Public Sub New(identificativo As String, sesso As String,
				   Optional numAnimali As Integer? = 0)
		prescapoIdentificativo = identificativo
		prescapoNumeroAnimali = numAnimali
		prescapoSesso = sesso
		'prescapoSelezioneMassivaCodice = ""
		'prescapoSelezioneMassivaOperatore = ""
		'prescapoSelezioneMassivaValore1 = ""
		'prescapoSelezioneMassivaValore2 = ""
	End Sub

End Class

Public Class InsertTrattamento_ProtTera_Response

	'Dati prescrizione
	Public presNumero As String
	Public presrigaNumeri As List(Of String)
	Public presrigaProdottiFamiglieAic As List(Of String)

End Class
