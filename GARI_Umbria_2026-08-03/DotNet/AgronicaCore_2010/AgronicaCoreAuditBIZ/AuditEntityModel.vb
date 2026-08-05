Public Class AuditCampiModel
    Public Audit_Tipo As Integer
    Public Regolamento_Cod As Integer
    Public Campo_Cod As String
    Public Campo_Des As String
    Public Note As String
End Class

Public Class AuditCodiciModel
    Public Audit_Tipo As Integer
    Public Regolamento_Cod As Integer
    Public Disp_Cod As Integer
    Public Disp_Nome As String
    Public Disp_Des As String
    Public Sezione_Cod As Integer
    Public Sezione_Des As String
    Public Parte As Integer
    Public Punto_Numero As String
    Public Descrizione As String
    Public Allegato As String
    Public Nota As Integer
    Public Tipo As String
    Public Punteggio As Integer
    Public PropostaCorrettiva As Integer
    Public Criterio As String
    Public Valore As String
    Public Valore_2 As String
    Public Punto_Numero_Default As String
    Public Punto_Numero_Deroga As String
    Public FunCalcoloLivello As String
End Class

Public Class AuditDisposizioniModel
    Public Audit_Tipo As Integer
    Public Regolamento_Cod As Integer
    Public Disp_Cod As Integer
    Public Disp_Nome As String
    Public Descrizione As String
    Public Attivazione As String
    Public Campo As Integer
    Public Campo_Des As String
    Public Ordine As Integer
    Public Note As String
End Class

Public Class AuditDomandeDisposizioniModel
    Public Audit_Tipo As Integer
    Public Regolamento_Cod As Integer
    Public Disp_Cod As Integer
    Public Domanda_Cod As Integer
    Public Disp_Cod_Dominante As Integer
End Class

Public Class AuditDomandeIntervisteModel
    Public Audit_Tipo As Integer
    Public Regolamento_Cod As Integer
    Public Domanda_Cod As Integer
    Public Domanda_Des As String
    Public Attivazione As String
    Public Tipo As String
    Public Ordine As Integer
End Class

Public Class AuditPunteggiModel
    Public Audit_Tipo As Integer
    Public Regolamento_Cod As Integer
    Public Disp_Cod As Integer
    Public Disp_Nome As String
    Public Descrizione As String
    Public Campo As Integer
    Public Campo_Des As String
    Public Portata As Integer
    Public Gravita As Integer
    Public Durata As Integer
    Public Esito As Boolean
    Public Verifica As Boolean
    Public Intenzionalita As Boolean
    Public Reiterazione As Boolean
    Public Inadempienza As Boolean
    Public PunteggioPonderato As Double
End Class

Public Class AuditRegolamentiModel
    Public Audit_Tipo As Integer
    Public Regolamento_Cod As Integer
    Public Regolamento_Des As String
End Class

Public Class AuditSezioniModel
    Public Audit_Tipo As Integer
    Public Regolamento_Cod As Integer
    Public Sezione_Cod As Integer
    Public Sezione_Des As String
    Public CalcoloPunteggio As Integer
    Public Parte As Integer
    Public Ordine As Integer
    Public FunCalcoloLivello As String
End Class

Public Class AuditStatiModel
    Public Audit_Tipo As Integer
    Public Stato_Cod As String
    Public Stato_Des As String
    Public Ordine As Integer
End Class

Public Class AuditIntervisteModel
    Public Audit_Tipo As Integer
    Public Regolamento_Cod As Integer
    Public Regolamento_Des As String
    Public Intervista_Cod As Integer
    Public Intervista_Nome As String
    Public Intervista_SuperUser As String
    Public Piva As String
    Public PivaReale As String
    Public Piva_Url As String
    Public Rag_Soc As String
    Public Validita_Inizio As String
    Public Validita_Fine As String
End Class

Public Class AuditRisposteIntervisteModel
    Public Audit_Tipo As Integer
    Public Regolamento_Cod As Integer
    Public Domanda_Cod As String
    Public Domanda_Des As String
    Public Attivazione As String
    Public Tipo As String
    Public Ordine As Integer
    Public Intervista_Cod As Integer
    Public Valore As String
End Class

Public Class AuditModel
    Public Audit_Tipo As Integer
    Public Regolamento_Cod As Integer
    Public Audit_Cod As Integer
    Public Audit_SuperUser As String
    Public Piva As String
    Public PivaReale As String
    Public Piva_Url As String
    Public Rag_Soc As String
    Public Validita_Inizio As String
    Public Validita_Fine As String
    Public Audit_Stato As Integer
    Public Stato_Des As String
    Public Note As String
    Public Completa As Boolean
    Public Username_Modifica As String
    Public Riferimento As String
    Public Rintracciabilita As Integer
End Class

Public Class AuditRisposteModel
    Public Audit_Tipo As Integer
    Public Regolamento_Cod As Integer
    Public Audit_Cod As Integer
    Public Audit_SuperUser As String
    Public Disp_Cod As Integer
    Public Disp_Des As Integer
    Public Sezione_Cod As Integer
    Public Sezione_Des As String
    Public Parte As Integer
    Public FunCalcoloLivello As String
    Public CalcoloPunteggio As Integer
    Public Punto_Numero As String
    Public Descrizione As String
    Public Allegato As String
    Public Nota As Integer
    Public Tipo As String
    Public Punteggio As Integer
    Public Ordine As Integer
    Public Valore As String
    Public Valore_2 As String
    Public Criterio As String
    Public PropostaCorrettiva As Integer
    Public ValorePropostaCorrettiva As String
End Class

Public Class AuditFormModel
    Public name As String
    Public value As String
End Class

Public Class AuditExtraInfoBIO
    Public EnteCertificazione As String
    Public Valutatore As String
    Public MotivoSospesaEsclusa As String
    Public StabilimentoConferimento As String
    Public Tecnico As String
    Public ConferimentoChiuso As Boolean
    Public ConfiniRischio As Boolean
    Public DescrizioneConfiniRischio As String
    Public SuperficiConversione As Boolean
    Public DettaglioSuperficiConversione As String
    Public DettaglioSuperficiBio As String
    Public Geolocalizzazione As String
    Public SeminaAutunnale As String
    Public SbloccoCCPB As Boolean
    Public AnalisiMultiresidualeConforme As Boolean
    Public NumeroRapportoProva As String
    Public SAValore As String
    Public ComunicazioneAziendePositivita As Boolean
    Public RiscontroAzienda As Boolean
    Public DataInvioControCampione As String
    Public ControCampioneConforme As Boolean
    Public NumeroRapportoProvaControCampione As String
    Public ControCampioneSAValore As String
    Public ComunicazioneEntePositivita As Boolean
    Public SbloccoEntePositivita As Boolean
    Public MisuraPrecauzionalePrevista As String
    Public ModuloPrescrizioneRelazioneTecnica As Boolean
    Public Foto As Boolean
    Public Bio As Boolean
    Public Bio_Text As String
    Public FuoriBio As Boolean
    Public FuoriBio_Text As String
    Public Declassato As Boolean
    Public Declassato_Text As String
End Class

Public Class AuditExtraInfoSQNPI
    Public PrimoConferimento As String
    Public Valutatore As String
    Public NoteContatti As String
    Public ConferimentoChiuso As Boolean
    Public AnalisiMultiresiduale As Boolean
    Public Tecnico As String
    Public Regione As String
    Public Telefono As String
    Public Mail As String
    Public DelegatoDoc As String
    Public OperatoreUff As String
    Public ValutatoreUff As String
    Public AdesioneMisure As Boolean
    Public EnteCertificazione As String
    Public EttariVerificati As String
    Public EttariDomanda As String
    Public Geolocalizzazione As String
    'Public DataChiusuraPratica As String
    Public DataFatturazione As String
End Class


Public Class AuditExtraInfoTrasporti
    Public Categoria As Integer
    Public Categoria_Des As String
    Public CodiceContrattoTrasporto As String
    Public DittaTrasporto_Des As String
    Public Targa As String
    Public Conducente_Des As String
End Class


Public Class AuditExtraInfoEnqueteHevea
    Public Nom_de_Enqueteur As String
    Public Secteur As String
    Public Superficie_totale_hevea As String
End Class

Public Class AuditExtraInfoFornitoreEUDR
    Public CodFornitore As String
    Public Fornitore As String
    Public Indirizzo As String
    Public RischioPaese As String
    Public FornitoreEU As String
    Public DataValidazione As String
End Class