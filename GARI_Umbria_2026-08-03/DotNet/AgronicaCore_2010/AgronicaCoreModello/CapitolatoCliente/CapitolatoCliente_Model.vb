Imports Newtonsoft.Json
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate


Public Class TabellaRMAModel
	Public Property TabellaRMA_COD As Integer   '(int , not null)
	Public Property TabellaRMA_DES As String   '(nvarchar(4000) , not null)
	Public Property inviato As Integer?   '(smallint, null)
	Public Property datainvio As DateTime?   '(datetime, null)
	Public Property Data_Creazione As DateTime?   '(datetime, null)
	Public Property Data_Modifica As DateTime?   '(datetime, null)
	Public Property Username_Creazione As String   '(varchar(250), null)
	Public Property Username_Modifica As String   '(varchar(250), null)
	Public Property Validita_Inizio As DateTime?   '(datetime, null)
	Public Property Validita_Fine As DateTime?   '(datetime, null)

	Sub New()

	End Sub
End Class

Public Class modelCapitolatoCliente
	Public Property Capitolato_COD As Integer
	Public Property Capitolato_Codice As String
	Public Property Nome As String
	Public Property NomeBreve As String
	Public Property DPCampagnaPubPriv As String
	Public Property DPCampagnaDES As String
	Public Property N_Max_PA As Integer?
	Public Property N_Max_Tracce As Integer?
	Public Property Perc_Max_RMA As Decimal?
	Public Property Sum_Perc_Max_RMA As Decimal?
	Public Property Attivo As Boolean
	Public Property DPI_Impianto As Boolean
	Public Property GestioneVarieta As Boolean
	Public Property Data_Modifica As Date?
	Public Property Data_Creazione As Date?
	Public Property Piva_SuperUser As String
	Public Property Validita_Inizio As Date?
	Public Property Validita_Fine As Date?
	Public Property Flag_Privato_Pubblico_DPI As Integer?
	Public Property DPI_COD_REGOLAMENTO As Integer?

	Public Property veg_cod As Integer
	Public Property veg_des As String
	Public Property TabellaRMA_COD As Integer
	Public Property FlagBIO As Boolean
	Public Property GestionePartecipazioneAziende As Boolean
	Public Property AttivaVerificaFormulatoValido As Boolean

	Public Property inviato As Integer?

	Sub New()
		Capitolato_COD = 0
		Capitolato_Codice = ""
		Nome = ""
		NomeBreve = ""
		DPCampagnaPubPriv = ""
		DPCampagnaDES = ""
		N_Max_PA = Nothing
		N_Max_Tracce = Nothing
		Perc_Max_RMA = Nothing
		Sum_Perc_Max_RMA = Nothing
		Attivo = False
		DPI_Impianto = False
		GestioneVarieta = False
		Data_Modifica = Date.Today
		Validita_Inizio = AGRODATAINIZIO
		Validita_Fine = AGRODATAFINE
		Piva_SuperUser = ""
		Flag_Privato_Pubblico_DPI = 0
		DPI_COD_REGOLAMENTO = 0
		veg_cod = 0
		veg_des = ""
		TabellaRMA_COD = 0
		FlagBIO = False
		GestionePartecipazioneAziende = False
		AttivaVerificaFormulatoValido = True

		inviato = 0
	End Sub
End Class



'Public Class modelElencoAziende
'	Public Property Capitolato_COD As Integer
'	Public Property Piva As String
'	Public Property PivaSuperUser As String

'	Sub New()
'		Capitolato_COD = 0
'		Piva = ""
'		PivaSuperUser = ""
'	End Sub
'End Class





'Public Class modelElencoVarieta
'	Public Property CodiceVarieta As Integer
'	Public Property Descrizione As String
'	Sub New()
'		CodiceVarieta = 0
'		Descrizione = ""
'	End Sub
'End Class

Public Class CapitolatoCliente

	Public Property Capitolato_COD As Integer   '(int , not null)
	Public Property Capitolato_DES As String   '(varchar(255), null)
	Public Property DPI_COD_REGOLAMENTO As Integer?   '(int, null)
	Public Property Flag_Privato_Pubblico_DPI As Integer?   '(int, null)
	Public Property N_Max_PA As Integer?   '(int, null)
	Public Property N_Max_Tracce As Integer?   '(int, null)
	Public Property Perc_Max_RMA As Decimal?   '(float, null)
	Public Property Sum_Perc_Max_RMA As Decimal?   '(float, null)
	Public Property Piva_SuperUser As String   '(varchar(50), null)
	Public Property Attivo As Integer?   '(smallint, null)
	Public Property DPI_Impianto As Integer?   '(smallint, null)
	Public Property inviato As Integer?   '(smallint, null)
	Public Property datainvio As DateTime?   '(datetime, null)
	Public Property Data_Creazione As DateTime?   '(datetime, null)
	Public Property Data_Modifica As DateTime?   '(datetime, null)
	Public Property Username_Creazione As String   '(varchar(250), null)
	Public Property Username_Modifica As String   '(varchar(250), null)
	Public Property Validita_Inizio As DateTime?   '(datetime, null)
	Public Property Validita_Fine As DateTime?   '(datetime, null)
	Public Property GestioneVarieta As Integer?   '(smallint, null)
	Public Property veg_cod As Integer?   '(int, null)
	Public Property TabellaRMA_COD As Integer?   '(int, null)
	Public Property Capitolato_DES_Breve As String   '(varchar(4000), null)
	Public Property FlagBIO As Integer   '(int , not null)
	Public Property GestionePartecipazioneAziende As Integer?   '(smallint, null)
	Public Property AttivaVerificaFormulatoValido As Integer?   '(int, null)
	Public Property Capitolato_Codice As String   '(varchar(25), null)

	Sub New()
		Capitolato_Codice = ""
		FlagBIO = False
		GestionePartecipazioneAziende = 0
		AttivaVerificaFormulatoValido = 1
		inviato = 0

		Data_Creazione = DateAndTime.Now()
		Data_Modifica = DateAndTime.Now()
	End Sub
End Class

Public Class CapitolatoClienteXFamigliePrincipiAttiviRilevati
	Public Property Capitolato_COD As Integer   '(int , not null)
	Public Property FAM_COD As String   '(varchar(255) , not null)
	Public Property LMR As Double   '(float , not null)
	Public Property inviato As Integer?   '(smallint, null)
	Public Property datainvio As DateTime?   '(datetime, null)
	Public Property Data_Creazione As DateTime?   '(datetime, null)
	Public Property Data_Modifica As DateTime?   '(datetime, null)
	Public Property Username_Creazione As String   '(varchar(250), null)
	Public Property Username_Modifica As String   '(varchar(250), null)
	Public Property Validita_Inizio As DateTime?   '(datetime, null)
	Public Property Validita_Fine As DateTime?   '(datetime, null)
	Public Property PartecipaSommatoria As Integer?   '(int, null)

	Sub New()

	End Sub

End Class

Public Class CapitolatoClienteXCultivar
	Public Property Capitolato_COD As Integer   '(int , not null)
	Public Property Cul_COD As String   '(varchar(255) , not null)
	Public Property inviato As Integer?   '(smallint, null)
	Public Property datainvio As DateTime?   '(datetime, null)
	Public Property Data_Creazione As DateTime?   '(datetime, null)
	Public Property Data_Modifica As DateTime?   '(datetime, null)
	Public Property Username_Creazione As String   '(varchar(250), null)
	Public Property Username_Modifica As String   '(varchar(250), null)
	Public Property Validita_Inizio As DateTime?   '(datetime, null)
	Public Property Validita_Fine As DateTime?   '(datetime, null)

	Sub New()

	End Sub

End Class

'Public Class CapitolatoClienteXImprese
'	Public Property Piva As String   '(varchar(50) , not null)
'	Public Property Capitolato_Cod As Integer   '(int , not null)
'	Public Property PivaSuperUser As String   '(varchar(50) , not null)

'	Sub New()

'	End Sub

'End Class

Public Class CapitolatoClienteXPrincipiAttiviRilevati
	Public Property Capitolato_COD As Integer   '(int , not null)
	Public Property PA_COD As String   '(varchar(255) , not null)
	Public Property LMR As Double   '(float , not null)
	Public Property inviato As Integer?   '(smallint, null)
	Public Property datainvio As DateTime?   '(datetime, null)
	Public Property Data_Creazione As DateTime?   '(datetime, null)
	Public Property Data_Modifica As DateTime?   '(datetime, null)
	Public Property Username_Creazione As String   '(varchar(250), null)
	Public Property Username_Modifica As String   '(varchar(250), null)
	Public Property Validita_Inizio As DateTime?   '(datetime, null)
	Public Property Validita_Fine As DateTime?   '(datetime, null)
	Public Property PartecipaSommatoria As Integer?   '(int, null)

	Sub New()

	End Sub
End Class

'Public Class modelElencoSostanzeAttive
'	Public Property CodiceSostanzaAttiva As Integer
'	Public Property Descrizione As String
'	Public Property Tipo As Integer
'	Sub New()
'		CodiceSostanzaAttiva = 0
'		Descrizione = ""
'		Tipo = 0
'	End Sub
'End Class

'Public Class modelElencoLMR
'	Public Property CodiceSostanzaAttiva As Integer
'	Public Property Descrizione As String
'	Public Property Tipo As Integer
'	Public Property partecipaSommatoria As Integer?
'	Public Property maxLMR As Integer
'	Sub New()
'		CodiceSostanzaAttiva = 0
'		Descrizione = ""
'		Tipo = 0
'		partecipaSommatoria = Nothing
'		maxLMR = 0
'	End Sub
'End Class
