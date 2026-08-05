Public Class CaricaModelli4_Response

	Public prenotazioneId As String
	Public documentoId As String
	Public tipoDestinazione As String
	Public codAzienda_Prov As String
	Public idFiscaleAzienda_Prov As String
	Public codAzienda_Dest As String
	Public idFiscaleAzienda_Dest As String
	Public targa As String
	Public targa_rimorchio As String
	Public regione_codice As String
	Public numAutorizzazione As String
	Public durataViaggio As String
	Public flagMezzoProprio As String
	Public codAsl_Trasp As String
	Public oraPartenza As String
	Public giorniValidita As Integer
	Public numModello As String
	Public dataPrenotazione As DateTime
	Public dataIngresso As DateTime
	Public dataUscita As DateTime
	Public dataDocumento As DateTime

	Public listaMatricoleCapi As List(Of String)

End Class
