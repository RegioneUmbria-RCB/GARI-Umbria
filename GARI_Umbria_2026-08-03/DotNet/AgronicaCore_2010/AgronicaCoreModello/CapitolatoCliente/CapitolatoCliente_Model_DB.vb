Public Class CapitolatoCliente_DatiGlobale
	Public Property DatiTestata As SalvaCapitolatoClienteModel
	Public Property DatiAziende As List(Of modelElencoAziende)
	Public Property DatiVarieta As List(Of modelElencoVarieta)
	Public Property DatiSAVietate As List(Of modelElencoSostanzeAttive)
	Public Property DatiLMREccezioni As List(Of modelElencoLMR)
	Public Property DatiLMRUlteriori As List(Of modelElencoLMR)
End Class


Public Class SalvaCapitolatoClienteModel
	Public Property Capitolato_COD
	Public Property Capitolato_Codice
	Public Property Capitolato_DES As String
	Public Property Capitolato_DES_Breve As String
	Public Property N_Max_PA As Integer?
	Public Property Perc_Max_RMA As Decimal?
	Public Property Sum_Perc_Max_RMA As Decimal?
	Public Property Validita_Inizio As DateTime?
	Public Property Validita_Fine As DateTime?
	Public Property Attivo
	Public Property DPI_Impianto
	Public Property FlagBIO
	Public Property GestionePartecipazioneAziende
	Public Property AttivaVerificaFormulatoValido
	Public Property GestioneVarieta
	Public Property veg_cod
	Public Property DPI_COD_REGOLAMENTO
	Public Property Flag_Privato_Pubblico_DPI
	Public Property TabellaRMA_COD
	Sub New()

	End Sub
End Class

Public Class modelDPIRegolamento
	Public Property DPI_COD_Regolamento As Integer
	Public Property NomeEsteso As String
End Class

Public Class modelFlagDPIPrivatoPubblico
	Public Property Flag_DPI_COD As Integer
	Public Property Descrizione As String
End Class

Public Class modelElencoDisciplinari
	Public Property DPI_COD_REGOLAMENTO As Integer
	Public Property NomeEsteso As String

	Sub New()
		DPI_COD_REGOLAMENTO = 0
		NomeEsteso = ""
	End Sub
End Class

Public Class modelElencoSpecieVegetali
	Public Property CodiceSpecie As Integer
	Public Property Descrizione As String
	Sub New()
		CodiceSpecie = 0
		Descrizione = ""
	End Sub
End Class

Public Class modelElencoAziende
	Public Property Capitolato_COD As Integer
	Public Property Piva As String
	Public Property PivaSuperUser As String
	Sub New()
	End Sub
End Class

Public Class modelElencoVarieta
	Public Property Varieta As String
	Public Property Capitolato_COD As Integer
	Public Property Selected As Boolean?

	Public Property CodiceVarieta As Integer
	Public Property Descrizione As String

	Sub New()
		CodiceVarieta = 0
		Descrizione = ""

		Varieta = ""
		Capitolato_COD = 0
		Selected = False
	End Sub
End Class


Public Class modelElencoSostanzeAttive
	Public Property CodiceSostanzaAttiva As Integer
	Public Property Descrizione As String
	Public Property Tipo As Integer
	Sub New()
		CodiceSostanzaAttiva = 0
		Descrizione = ""
		Tipo = 0
	End Sub
End Class

Public Class modelElencoLMR
	Public Property CodiceSostanzaAttiva As Integer
	Public Property Descrizione As String
	Public Property Tipo As Integer
	Public Property maxLMR As Integer
	Public Property partecipaSommatoria As Integer?
	Sub New()
		CodiceSostanzaAttiva = 0
		Descrizione = ""
		Tipo = 0
		partecipaSommatoria = Nothing
		maxLMR = 0
	End Sub
End Class

