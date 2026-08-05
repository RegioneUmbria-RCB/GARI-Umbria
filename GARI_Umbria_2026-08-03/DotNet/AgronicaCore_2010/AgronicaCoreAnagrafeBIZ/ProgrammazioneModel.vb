
#Region "ClassiModel"

Public Class RibaltaAppezza
    Public Property Piva_SuperUser As String
    Public Property Programmazione_Cod As String
    Public Property Programmazione_Entita_Cod As String
    Public Property Operazione_Cod As String
    Public Property Piva As String
    Public Property Sa_Cod As String
    Public Property Campo_Cod As String
    Public Property Appezza As String
    Public Property Id_Reg As String
    Public Property Progetto_Cod As String
    Public Property Progetto_Nome As String
    Public Property Id_Cod As String
    Public Property Veg_Cod As String
    Public Property Cul_Cod As String
    Public Property Grfi_Cod As String
    Public Property Cop_Cod As String
    Public Property Resa As String
    Public Property TipoZona As String
    Public Property Veg_Cod_Prec As String
    Public Property Id_Mat_O As String
    Public Property Id_Fre As String
    Public Property N_Distribuito As String
    Public Property Validita_Inizio As String
    Public Property Validita_Fine As String
    Public Property Num_Piante As String
    Public Property Veg_Cod_Cliente As String
    Public Property Cul_Cod_Cliente As String
    Public Property Grva_Cod As String
    Public Property Gru_Cod As String
    Public Property Grva_Des As String
    Public Property Campo_Des As String
    Public Property Piva_old As String
    Public Property Sa_Cod_old As String
    Public Property Campo_Cod_old As String
    Public Property Appezza_old As String
    Public Property Id_Reg_old As String
    Public Property Progetto_cod_old As String
    Public Property N_fabbisogno As String
    Public Property Codice_Fiscale_Tecnico As String
    Public Property SupApp_Formattata As String
    Public Property Operazione As String
    Public Property Sa_Nome As String
    Public Property App_Nome As String
    Public Property Sup_App As String
    Public Property Veg_Des As String
    Public Property Cul_Des As String
    Public Property Grfi_Des As String
    Public Property DestinazioneUso_Des As String
    Public Property Validita_Inizio_Impianto As String
    Public Property Validita As String
    Public Property Validita_Originale As String
    Public Property Resa_Formattata As String
    Public Property Fase_Ciclo_Colturale_ID As Integer?
    Public Property Fase_Ciclo_Colturale_Des As String
    Public Property LimiteMaxN As String
    Public Property Unita_Vitata As String
    Public Property Catasto As String
    Public Property Ribaltato As String
    Public Property DateModificate As String

    'Public Property Selected As Boolean
    Public Property Imp_Cod As String
    Public Property Foral_Cod As Integer
    Public Property TRA_Fila As Double?
    Public Property SU_Fila As Double?
    Public Property MetodoProduzione_Cod As Integer?
    Public Property Disciplinare_Cod As Integer?
    Public Property Regolamento_Cod As Integer?
    Public Property Limite_N As String
    Public Property Limite_P As String
    Public Property Limite_K As String
    Public Property Veg_Cod_Prec2 As Integer?
    Public Property Veg_Cod_Prec3 As Integer?
    Public Property Veg_Cod_Prec4 As Integer?
    Public Property Piano_Semina As String
    Public Property Codice_Contratto As String
    Public Property Data_Semina As String
    Public Property Data_Raccolta As String
    Public Property Data_Fioritura_Prevista As String
    Public Property stato_ribaltamento As Integer?
    Public Property IAF As String
    Public Property Pratica_Cod As String
    Public Property Regolamento_Concimazione_Cod As Integer?
    Public Property Flag_PubblicoPrivato As Integer?
    Public Property Id_tr As Integer?
    Public Property DistBZ_CorpiIdrici As Double?
    Public Property DistBZ_AreeResPub As Double?
    Public Property DistBZ_Allevamenti As Double?
    Public Property DistBZ_VegNatNonColt As Double?
    Public Property SupBZ_Riduzione As Double?

    Public Property Riferimento_Alfanumerico_Appezzamento As String
    Public Property Isola As String
    Public Property CapitolatoPrivato As String
    Public Property Finalita_Concimazione_Impianto As Integer?
    Public Property Cod_Indirizzo As Integer?
    Public Property ind_des As String
    Public Property frz_des As String
    Public Property CAP As String
    Public Property com_des_indirizzo As String
    Public Property pro_cod_indirizzo As String
    Public Property stato_indirizzo As String
    Public Property stato_indirizzo_des As String
    Public Property note_indirizzo As String
    Public Property pro_cod_istat_indirizzo As String
    Public Property com_cod_istat_indirizzo As String
    Public Property KPIN As String
    Public Property Block_Name As String
    Public Property Data_Inizio_Portinnesto As String
    Public Property Mat_Cod As Integer


End Class

Public Class DatiRibalta
    Public Property RibaltaAppezza As RibaltaAppezza()
End Class

Public Class AppChiusi
    Public Property Piva As String
    Public Property Sa_Cod As String
    Public Property Campo_Cod As String
    Public Property Appezza As String
    Public Property ID_Reg As String
    Public Property Progetto_Cod As String
    Public Property Programmazione_Entita_Cod As String
    Public Property Operazione_Cod As String
    Public Property Operazione As String
    Public Property Sa_Nome As String
    Public Property Campo_Des As String
    Public Property App_Nome As String
    Public Property Sup_App As String
    Public Property Veg_Des As String
    Public Property Cul_Des As String
    Public Property Grfi_Des As String
    Public Property Progetto_Nome As String
    Public Property DestinazioneUso_Des As String
    Public Property Data_Chiusura As String
    Public Property UNID_APP_NEW As String
    Public Property Selected As Boolean
End Class

Public Class AppezzamentiChiusi
    Public Property AppChiusi As AppChiusi()
End Class

#End Region
