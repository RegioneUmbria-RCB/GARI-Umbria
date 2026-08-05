Public Class AgronicaCoreParametri_NG
    Public PivaSuperUser As String
    Public SuperUserUsername As String
    Public UsernameOperazione As String
    Public UtenteUsername As String
    Public UtenteCodFiscale As String
    Public FinestraTemporaleInizio As Date
    Public FinestraTemporaleFine As Date
    Public Lingua_Cod As Integer
End Class

Public Class AgronicaLink_NG
    Public linkGiasBase As String
    Public linkAgronicaCoreAPI As String
End Class
Public Class VariabiliInSessione_NG
    Public collegamento_dpi As String
    Public collegamento_fito As String
    Public id_servizio As String
    Public pathfileini As String
    Public cn_server As String
    Public cn_utenti As String
    Public cn_logaccessi As String
    Public stringa_cn_server As String
    Public stringa_cn_utenti As String
    Public progressivo_gias As String
    Public utente_usr As String
    Public utente_pwd As String
    Public utente_codfiscale As String
    Public utente_usr_crypt As String
    Public utente_pwd_crypt As String
    Public superuser_usr As String
    Public superuser_pwd As String
    Public superuser_piva As String
    Public superuser_usr_crypt As String
    Public superuser_pwd_crypt As String
    Public finestratemporale_inizio As String
    Public finestratemporale_fine As String
    Public agronicacore_flag_cancellazionelogica As String
    Public agronicacore_flag_visibilita As String
    Public agronicacore_directorylog As String
    Public agronicacore_nomefilelog As String
End Class

Public Class Imprese_Impostazioni
    Public Piva As String
    Public Sa_Cod As Integer
    Public Impostazione_Cod As Integer
    Public Valore As String
End Class

Public Class ImpiantiAgendaNG
    Public Piva As String
    Public Sa_Cod As Integer
    Public Appezza As Integer
    Public Id_Reg As Integer
    Public Progetto_Cod As Integer
    Public Veg_Cod As Integer
    Public Id_Cod As Integer
    Public Sup_Imp As Decimal
    Public Sup_Imp_help As Decimal
    Public Sup_Riduzione_BufferZone As Decimal
    Public Perc_Riduzione_Deriva As Decimal
End Class