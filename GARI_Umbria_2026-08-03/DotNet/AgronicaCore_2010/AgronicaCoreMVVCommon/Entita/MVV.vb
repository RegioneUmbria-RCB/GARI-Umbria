Imports System.Text

Public Class MVV

    Public Speditore As MVV_Speditore
    Public Vettore As MVV_Vettore
    Public Destinatario As MVV_Destinatario
    Public DestinatarioDiverso As MVV_Destinatario_Diverso
    Public HaVenditore As Boolean
    Public Venditore As MVV_Venditore
    Public HaAcquirente As Boolean
    Public Acquirente As MVV_Acquirente
    Public Conducente_Nome As String
    Public Conducente_Cognome As String
    Public MezzoTrasoprto As MVV_MezzoTrasporto
    Public Documento As MVV_Documento
    Public Dettagli As List(Of MVV_Dettaglio)
    Public AutoritaComepetente As MVV_AutoritaCompetente
    Public Note As String
    Public HaDestinatarioDiverso As Boolean
    Public HaLuogoSpedizioneDIverso As Boolean
    Public HaLuogoConsegnaDiverso As Boolean
    Public LuogoConsegnaDiverso As String
    Public CodIndirizzoConsegnaDiverso As Integer
    Public IndirizzoConsegnaDiverso As MVV_Indirizzo

    Public Overrides Function ToString() As String

        Dim risultatoBase = String.Format("{0} / {1}",
                                 Documento.DocNumero,
                                 Convert.ToDateTime(Documento.DataDocumento).Year.ToString())

        If String.IsNullOrEmpty(Documento.CodiceICQRF) Then
            Return String.Format("MVV {0}", risultatoBase)
        Else
            Dim icqrfSenzaProv = Documento.CodiceICQRF.ToUpper().Replace(Speditore.Indirizzo.Provincia.ToUpper(), "")

            Return String.Format("MVV {0} / {1} / {2}",
                    Speditore.Indirizzo.Provincia.ToUpper(),
                    icqrfSenzaProv.ToUpper(),
                    risultatoBase
                    )

        End If

    End Function

End Class

Public Class MVV_Documento

    Public CausaleTrasporto As String
    Public DocNumeroSin As String
    Public DocNumero As String
    Public DocNumeroDes As String
    Public DataInizioTrasporto As Date
    Public OraInizioTrasporto As String
    Public OraTrasporto As Integer
    Public MinutiTrasporto As Integer
    Public LuogoSpedizione As String
    Public CodiceICQRF As String
    Public DataDocumento As String
    Public FlagArt29 As Integer

    Public Overrides Function ToString() As String
        Return String.Format("{0}{1}{2}", DocNumeroSin, DocNumero, DocNumeroDes)
    End Function

End Class

Public Class RegVinoAttributo

    Public Codice As String
    Public Descrizione As String

    Public Sub New(ByVal codice As String)
        Me.Codice = codice
    End Sub

End Class
Public Class RegistroVino

    Public CodStatoFisico As RegVinoAttributo
    Public CodClassificazione As RegVinoAttributo
    Public CodCategoria As RegVinoAttributo
    Public CodColore As RegVinoAttributo
    Public AttoCert As RegVinoAttributo
    Public Annata As RegVinoAttributo
    Public PercAnnata As String
    Public Biologico As RegVinoAttributo
    Public OrigineUve As RegVinoAttributo
    Public Provenienza As RegVinoAttributo
    Public Varieta As RegVinoAttributo
    Public AltreVarieta As String
    Public Menzioni As String
    Public CodZonaViticola As RegVinoAttributo
    Public DocIGP As RegVinoAttributo
    Public CodOperazioneVit As RegVinoAttributo
    Public Designazione As String
    Public CodEbacchus As RegVinoAttributo
    Public CodSottozona As RegVinoAttributo
    Public COdVigna As RegVinoAttributo
    Public CodPartita As String
    Public MassaVolumica As Decimal
    Public NumCertDOP As String
    Public DataCertDOP As DateTime
    Public PraticheEnologiche As String
    Public PaesiProvenienza As String
    Public CodTenoreZucchero As RegVinoAttributo


    Public Function IsEmpty() As Boolean

        If String.IsNullOrEmpty(CodStatoFisico.Descrizione) AndAlso String.IsNullOrEmpty(CodCategoria.Descrizione) _
            AndAlso String.IsNullOrEmpty(CodColore.Descrizione) AndAlso String.IsNullOrEmpty(AttoCert.Descrizione) _
            AndAlso String.IsNullOrEmpty(Annata.Descrizione) AndAlso String.IsNullOrEmpty(Biologico.Descrizione) _
            AndAlso String.IsNullOrEmpty(OrigineUve.Descrizione) AndAlso String.IsNullOrEmpty(Provenienza.Descrizione) _
            AndAlso String.IsNullOrEmpty(Varieta.Descrizione) AndAlso String.IsNullOrEmpty(CodZonaViticola.Descrizione) _
            AndAlso String.IsNullOrEmpty(DocIGP.Descrizione) Then
            Return True
        Else
            Return False
        End If


    End Function

End Class

Public Class MVV_AutoritaCompetente
    Public Indirizzo As MVV_Indirizzo
    Public ICQRF As String
End Class
Public Class MVV_Dettaglio

    Public CodiceNC As String
    Public CodCategoria As String
    Public CodiceZonaVit As String
    Public CodiceOperazioneVit As String
    Public DescrizioneGias As String
    Public TitoloAlcolTot As Decimal
    Public TitoloAlcolPot As Decimal
    Public TitoloAlcolEff As Decimal
    Public Densita As Decimal
    Public CapacitaImballo As Decimal
    Public CapacitaImballo_MateriaPrima As Decimal
    Public NumeroColli As Integer
    Public NumeroImballi As Integer
    Public DescrizioneColli As String
    Public DescrizioneImballi As String
    Public UnitaMisura As String
    Public Quantita As Decimal
    Public RegistroVino As RegistroVino
    Public UDMQTA As MVV_UDM_QTA
    Public UDMQTA_MAteriaPrima As MVV_UDM_QTA_MATERIA_PRIMA
    Public ID_Mov_Det As Integer
    Public Flag_Extra As Boolean
    Public CategoriaVinoCod As String
    Public UsaColli As Boolean
    Public Extra_Zona_Viticola As String
    Public Mat_Cod As Integer
    Public Lotto As String
    Public CodiceGenerazione As Integer
    Public VolNominale As Decimal

    Public MateriePrime_DescrizioneCommerciale As String
    Public MateriePrime_CodiceArticolo As String
    Public DescrizioneGiasAddizionale As String
    Public CodZonaViticola_Linea As String
    Public CodZonaViticola_Gen As String


    Public Overrides Function ToString() As String

        Dim descrizioneAggiuntiva As String = MateriePrime_DescrizioneCommerciale
        If Not String.IsNullOrEmpty(DescrizioneGiasAddizionale) Then
            descrizioneAggiuntiva = descrizioneAggiuntiva & " - " & Replace(DescrizioneGiasAddizionale, "§", " ")
        End If

        If Not String.IsNullOrEmpty(RegistroVino.Designazione) Then

            Dim sb = New StringBuilder()
            If String.IsNullOrEmpty(Lotto) Then
                sb.AppendLine(RegistroVino.Designazione)
            Else
                sb.AppendLine(String.Format("{0}, Lotto: {1}", RegistroVino.Designazione, Lotto))
            End If
            sb.AppendLine()
            sb.AppendLine(descrizioneAggiuntiva)

            Return sb.ToString()
        End If


        If Me.RegistroVino.IsEmpty() Then
            If String.IsNullOrEmpty(Lotto) Then
                Return descrizioneAggiuntiva
            Else
                Return String.Format("{0}, Lotto: {1}", descrizioneAggiuntiva, Lotto)
            End If

        Else
            Dim formatoDescrizione = "{0}, {1}, {2}, Descr. Atto: {3}, DOC: {4}, {5}, {6}, Paese orig. Uve: {7}, Paese prov: {8}, Varietà: {9}, Cap.imballo: {10}, Lotto: {11}"

            Dim descrRegVino = String.Format(formatoDescrizione,
                                 RegistroVino.CodStatoFisico.Descrizione,
                                 RegistroVino.CodCategoria.Descrizione,
                                 RegistroVino.CodColore.Descrizione,
                                 RegistroVino.AttoCert.Descrizione,
                                 RegistroVino.DocIGP.Descrizione,
                                 RegistroVino.Annata.Descrizione,
                                 RegistroVino.Biologico.Descrizione,
                                 RegistroVino.OrigineUve.Descrizione,
                                 RegistroVino.Provenienza.Descrizione,
                                 RegistroVino.Varieta.Descrizione, CapacitaImballo, Lotto)

            Dim sb = New StringBuilder(descrRegVino)
            sb.AppendLine()
            sb.AppendLine(descrizioneAggiuntiva)

            Return sb.ToString()

        End If


    End Function

End Class

Public Class MVV_Destinatario : Inherits MVV_Anagrafica
    Public CodiceAccisa As String
    Public CODICE_SOGGETTO As String
End Class

Public Class MVV_Destinatario_Diverso : Inherits MVV_Anagrafica
    Public CodiceAccisa As String
End Class


Public Class MVV_Venditore : Inherits MVV_Anagrafica
End Class

Public Class MVV_Acquirente : Inherits MVV_Anagrafica

End Class

Public Class MVV_Speditore : Inherits MVV_Anagrafica
    Public CodiceAccisa As String
End Class

Public Class MVV_Vettore : Inherits MVV_Anagrafica
    Public Mezzo As Integer
End Class
Public Class MVV_Anagrafica

    Public Denominazione As String
    Public Nome As String
    Public Cognome As String
    Public PIVA As String
    Public Indirizzo As MVV_Indirizzo
    Public ID_CF As Integer
    Public TipoSoggetto As String
    Public CODICE_SOGGETTO As String
    Public COD_CONTATTO As String
    Public SEED As String

End Class

Public Class MVV_Indirizzo
    Public Indirizzo As String
    Public CAP As String
    Public Comune_Provincia As String
    Public Stato As String
    Public Provincia As String
    Public Istat_Provincia As String
    Public Istat_Comune As String
End Class

Public Class MVV_MezzoTrasporto

    Public Tipo As String
    Public Targa As String
    Public TargaRimorchio As String
    Public NumeroAutorizzazione As String
    Public DataAutorizzazione As String
    Public Codice As Integer

End Class

Public Class MVV_UDM_QTA
    Public UDM_DES As String
    Public UDM_DES_EXTRA As String
    Public UDM_SIM As String
    Public UDM_SIM_EXTRA As String
    Public UDM_COD_EXTRA As Integer
    Public UDM_SIN_EXTRA2 As String
    Public QTA_EXtra_Tot As Decimal
    Public QTA As Decimal
    Public QTA_Extra As Decimal
End Class

Public Class MVV_UDM_QTA_MATERIA_PRIMA
    Public UDM_COD As Integer
    Public UDM_COD_EXTRA As Integer
    Public UDM_DES As String
    Public UDM_DES_EXTRA As String
    Public UDM_SIM As String
    Public UDM_SIM_EXTRA As String
End Class