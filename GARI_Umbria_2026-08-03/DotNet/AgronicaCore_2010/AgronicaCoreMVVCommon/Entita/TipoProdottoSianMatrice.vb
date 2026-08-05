Public Class TipoProdottoSianMatrice
    Public Codice As String
    Public Descrizione As String
    Public CodiceGenerazione As List(Of Integer)
    Public ElencoCategorie As List(Of Integer)
    Public ElencoClassificazione As List(Of String)
End Class


Public Enum enumAttributiSian
    AttoCertificato
    ClassificazioneProdotto
    CodiceDopIGP
    PercIGP
    CodiceEbaccus
    Provenienza
    PaeseProvenienza
    OrigineUve
    ZonaViticola
    Varieta_N
    PercVarieta_N
    Varieta_Non_SIAN
    Sottozona
    Vigna
    Colore
    Menzione_N
    Biologico
    TenoreZucchero
    PraticaEnologica_N
    Annata
    PercAnnata
    AlcoolPotenziale
    AlcoolEffettivo
    AlcoolTotale
    MassaVol
    StatoFisico
    RecipienteCaricoScarico
    Qta
    VolumeNominale
    DataCertDOP
    NumCertDOP
    NumeroCertificazione_N
    Lotto
    Note
    GradoAcidita
    MonteGradi
    Partita
    Trattamento
    MetodoPE
    GiorniInvecchiamento
    QtaPersa
End Enum