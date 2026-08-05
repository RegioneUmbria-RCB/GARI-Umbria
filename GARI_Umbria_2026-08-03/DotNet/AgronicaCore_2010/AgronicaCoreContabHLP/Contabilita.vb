Imports System.Data
Imports AgronicaCoreDataProvider.Agro_Math
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.My.Resources

Public Class Contabilita

    Public Shared Function CausaliTrasportoXLavCod(ByVal tipoCausale As Integer, dtTipiDoc As DataTable) As Integer()

        Dim tipo As enum_Tipo_CausaliTrasporto = enum_Tipo_CausaliTrasporto.Non_Impostato
        If [Enum].IsDefined(GetType(enum_Tipo_CausaliTrasporto), tipoCausale) Then
            tipo = [Enum].Parse(GetType(enum_Tipo_CausaliTrasporto), tipoCausale)
        End If

        Select Case tipo
            Case enum_Tipo_CausaliTrasporto.Tutti_Doc_Contabili,
                 enum_Tipo_CausaliTrasporto.Doc_Contabili_Attivi,
                 enum_Tipo_CausaliTrasporto.Doc_Contabili_Passivi,
                 enum_Tipo_CausaliTrasporto.Accettazione_Beni
                Dim result As New List(Of Integer)
                result.AddRange({0, -10000, -10001})
                If Not IsNothing(dtTipiDoc) Then
                    result.AddRange(dtTipiDoc.Select($"Lav_Cod <> {LAVCOD_MVV_EMESSO}") _
                        .Select(Function(x) CInt(x("Lav_Cod"))))
                End If
                Return result.ToArray()

            Case enum_Tipo_CausaliTrasporto.MVV_Elettronico
                Return New Integer() {
                    LAVCOD_MVV_EMESSO
                }

            Case enum_Tipo_CausaliTrasporto.Contratti_Affitto
                Return New Integer() {
                    LAVCOD_CONTRATTO_AFFITTO
                }

            Case Else
                Return Nothing

        End Select

    End Function

    Public Shared Function Attivo_Passivo(ByVal lavCod As Integer) As String

        Select Case lavCod

            Case LAVCOD_FATTURA_EMESSA, LAVCOD_NOTA_ACCREDITO_RICEVUTA, LAVCOD_ALTRI_RICAVI,
                LAVCOD_BOLLA_EMESSA, LAVCOD_VENDITA, LAVCOD_AUTOCONSUMO,
                LAVCOD_RICEVUTA_EMESSA, LAVCOD_FATTURA_LIQ_CONF_EMESSA, LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA,
                LAVCOD_DOCO_EMESSO, LAVCOD_FATTURA_PROFORMA, LAVCOD_CORRISPETTIVO_VENDITA_SFUSO,
                LAVCOD_AUTOCONSUMO_VINO_SFUSO, LAVCOD_DDT_CONTABILIZZATO_EMESSO, LAVCOD_MVV_EMESSO,
                LAVCOD_ALTRI_RICAVI_NERO, LAVCOD_ORDINE_VENDITA, LAVCOD_PREVENTIVO_VENDITA

                Return "ATTIVO"

            Case LAVCOD_FATTURA_RICEVUTA, LAVCOD_NOTA_ACCREDITO_EMESSA, LAVCOD_REG_COMPENSI,
                LAVCOD_BOLLA_RICEVUTA, LAVCOD_ACQUISTO_BENI, LAVCOD_ALTRI_COSTI,
                LAVCOD_ACQUISTO, LAVCOD_FATTURA_LIQ_CONF_RICEVUTA, LAVCOD_AUTOFATTURA_LIQ_CONF_EMESSA,
                LAVCOD_DOCO_RICEVUTO, LAVCOD_FATTURA_PROFESSIONISTI, LAVCOD_MVV_RICEVUTO,
                LAVCOD_ALTRI_COSTI_NERO, LAVCOD_ORDINE_ACQUISTO,
                LAVCOD_ACCETTAZIONE_DIVERSI, LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE, LAVCOD_DISTINTA_CARICO_ACCETTAZIONE

                Return "PASSIVO"

            Case LAVCOD_MOV_FINANZIARIO     'Partita Doppia
                Return ""
            Case Else
                Return ""
        End Select

    End Function

    '####################################################################################################################
    Public Function IVA_Credito(ByVal lavCod As Integer, ByVal iva As Decimal) As Decimal

        'Acquisto
        Select Case lavCod

            Case LAVCOD_FATTURA_RICEVUTA, LAVCOD_REG_COMPENSI, LAVCOD_ACQUISTO_BENI, LAVCOD_ALTRI_COSTI,
                LAVCOD_ALTRI_COSTI_NERO, LAVCOD_ACQUISTO, LAVCOD_NOTA_ACCREDITO_EMESSA,
                LAVCOD_FATTURA_LIQ_CONF_RICEVUTA, LAVCOD_AUTOFATTURA_LIQ_CONF_EMESSA, LAVCOD_FATTURA_PROFESSIONISTI

                Return iva

            Case Else

                Return 0

        End Select


    End Function
    
    '####################################################################################################################
    Public Function IVA_Debito(ByVal lavCod As Integer, ByVal iva As Decimal) As Decimal

        'Vendita
        Select Case lavCod

            Case LAVCOD_FATTURA_EMESSA, LAVCOD_NOTA_ACCREDITO_RICEVUTA, LAVCOD_ALTRI_RICAVI, LAVCOD_ALTRI_RICAVI_NERO,
                LAVCOD_VENDITA, LAVCOD_AUTOCONSUMO, LAVCOD_RICEVUTA_EMESSA, LAVCOD_FATTURA_LIQ_CONF_EMESSA,
                LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA, LAVCOD_CORRISPETTIVO_VENDITA_SFUSO,
                LAVCOD_AUTOCONSUMO_VINO_SFUSO, LAVCOD_DDT_CONTABILIZZATO_EMESSO

                Return iva

            Case Else

                Return 0

        End Select


    End Function

    '####################################################################################################################
    Public Function Leggi_Imponibile_PositivoNegativo(ByVal lavCod As Integer, ByVal valore As Decimal) As Decimal

        Select Case lavCod

            Case LAVCOD_BOLLA_EMESSA, LAVCOD_DDT_CONTABILIZZATO_EMESSO, LAVCOD_FATTURA_EMESSA,
                LAVCOD_FATTURA_PROFORMA, LAVCOD_RICEVUTA_EMESSA,
                LAVCOD_FATTURA_LIQ_CONF_EMESSA,
                LAVCOD_AUTOFATTURA_LIQ_CONF_EMESSA, LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA,
                LAVCOD_ALTRI_RICAVI, LAVCOD_VENDITA, LAVCOD_CORRISPETTIVO_VENDITA_SFUSO,
                LAVCOD_NOTA_ACCREDITO_RICEVUTA, LAVCOD_ORDINE_VENDITA, LAVCOD_PREVENTIVO_VENDITA,
                LAVCOD_CONFERIMENTO_DIVERSI,
                LAVCOD_AUTOCONSUMO, LAVCOD_AUTOCONSUMO_VINO_SFUSO, LAVCOD_DOCO_EMESSO, LAVCOD_DAA_EMESSO

                Return valore

            Case LAVCOD_BOLLA_RICEVUTA, LAVCOD_FATTURA_RICEVUTA,
                LAVCOD_FATTURA_PROFESSIONISTI,
                LAVCOD_FATTURA_LIQ_CONF_RICEVUTA,
                LAVCOD_ALTRI_COSTI, LAVCOD_ACQUISTO,
                LAVCOD_NOTA_ACCREDITO_EMESSA,
                LAVCOD_ORDINE_ACQUISTO,
                LAVCOD_CONFERIMENTO, LAVCOD_DOCO_RICEVUTO,
                LAVCOD_ACCETTAZIONE_DIVERSI, LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE, LAVCOD_DISTINTA_CARICO_ACCETTAZIONE

                Return -1 * valore

            Case Else

                Return valore

        End Select

    End Function

    '####################################################################################################################
    ''' <summary>
    ''' a differenza dell'altra visualizza le note di accredito col segno -
    ''' </summary>
    Public Function Leggi_Imponibile_PositivoNegativo2(ByVal lavCod As Integer, ByVal valore As Decimal) As Decimal

        Select Case lavCod

            Case LAVCOD_BOLLA_EMESSA, LAVCOD_DDT_CONTABILIZZATO_EMESSO, LAVCOD_FATTURA_EMESSA,
                LAVCOD_FATTURA_PROFORMA, LAVCOD_RICEVUTA_EMESSA,
                LAVCOD_FATTURA_LIQ_CONF_EMESSA,
                LAVCOD_AUTOFATTURA_LIQ_CONF_EMESSA, LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA,
                LAVCOD_ALTRI_RICAVI, LAVCOD_VENDITA, LAVCOD_CORRISPETTIVO_VENDITA_SFUSO,
                LAVCOD_CONFERIMENTO_DIVERSI,
                LAVCOD_AUTOCONSUMO, LAVCOD_AUTOCONSUMO_VINO_SFUSO, LAVCOD_DOCO_EMESSO, LAVCOD_DAA_EMESSO,
                LAVCOD_NOTA_ACCREDITO_EMESSA, LAVCOD_ORDINE_VENDITA, LAVCOD_PREVENTIVO_VENDITA

                Return valore

            Case LAVCOD_BOLLA_RICEVUTA, LAVCOD_FATTURA_RICEVUTA,
                LAVCOD_FATTURA_PROFESSIONISTI,
                LAVCOD_FATTURA_LIQ_CONF_RICEVUTA,
                LAVCOD_ALTRI_COSTI, LAVCOD_ACQUISTO,
                LAVCOD_CONFERIMENTO, LAVCOD_DOCO_RICEVUTO,
                LAVCOD_ORDINE_ACQUISTO,
                LAVCOD_NOTA_ACCREDITO_RICEVUTA,
                LAVCOD_ACCETTAZIONE_DIVERSI, LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE, LAVCOD_DISTINTA_CARICO_ACCETTAZIONE

                Return -1 * valore

            Case Else

                Return valore

        End Select


    End Function

    '####################################################################################################################
    Public Function Leggi_Imponibile_PositivoNegativo_RegCorrispettivi(ByVal valore As Decimal) As Decimal

        'poiché nel registro corrispettivi ci vanno:
        '- ricevuti fiscali emesse
        '-corrispettivi vendita
        'ddt contabilizzati emessi
        'non ho bisogno del lav_cod

        Return valore

    End Function


    '####################################################################################################################
    Public Function Leggi_IVA_PositivaNegativa(ByVal lavCod As Integer, ByVal valore As Decimal) As Decimal

        Select Case lavCod

            Case LAVCOD_BOLLA_EMESSA, LAVCOD_DDT_CONTABILIZZATO_EMESSO, LAVCOD_FATTURA_EMESSA,
                LAVCOD_FATTURA_PROFORMA, LAVCOD_RICEVUTA_EMESSA,
                LAVCOD_FATTURA_LIQ_CONF_EMESSA,
                LAVCOD_AUTOFATTURA_LIQ_CONF_EMESSA, LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA,
                LAVCOD_ALTRI_RICAVI, LAVCOD_VENDITA, LAVCOD_CORRISPETTIVO_VENDITA_SFUSO,
                LAVCOD_NOTA_ACCREDITO_RICEVUTA, LAVCOD_ORDINE_VENDITA, LAVCOD_PREVENTIVO_VENDITA,
                LAVCOD_CONFERIMENTO_DIVERSI,
                LAVCOD_AUTOCONSUMO, LAVCOD_AUTOCONSUMO_VINO_SFUSO, LAVCOD_DOCO_EMESSO, LAVCOD_DAA_EMESSO

                Return -1 * valore

            Case LAVCOD_BOLLA_RICEVUTA, LAVCOD_FATTURA_RICEVUTA,
                LAVCOD_FATTURA_PROFESSIONISTI,
                LAVCOD_FATTURA_LIQ_CONF_RICEVUTA,
                LAVCOD_ALTRI_COSTI, LAVCOD_ACQUISTO,
                LAVCOD_NOTA_ACCREDITO_EMESSA,
                LAVCOD_ORDINE_ACQUISTO,
                LAVCOD_CONFERIMENTO, LAVCOD_DOCO_RICEVUTO,
                LAVCOD_ACCETTAZIONE_DIVERSI, LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE, LAVCOD_DISTINTA_CARICO_ACCETTAZIONE

                Return valore

            Case Else

                Return valore

        End Select


    End Function

    '####################################################################################################################
    ''' <summary>
    ''' a differenza dell'altra visualizza le note di accredito col segno -
    ''' </summary>
    Public Function Leggi_IVA_PositivaNegativa2(ByVal lavCod As Integer, ByVal valore As Decimal) As Decimal

        Select Case lavCod

            Case LAVCOD_BOLLA_EMESSA, LAVCOD_DDT_CONTABILIZZATO_EMESSO, LAVCOD_FATTURA_EMESSA,
                LAVCOD_FATTURA_PROFORMA, LAVCOD_RICEVUTA_EMESSA,
                LAVCOD_FATTURA_LIQ_CONF_EMESSA,
                LAVCOD_AUTOFATTURA_LIQ_CONF_EMESSA, LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA,
                LAVCOD_ALTRI_RICAVI, LAVCOD_VENDITA, LAVCOD_CORRISPETTIVO_VENDITA_SFUSO,
                LAVCOD_NOTA_ACCREDITO_EMESSA, LAVCOD_ORDINE_VENDITA, LAVCOD_PREVENTIVO_VENDITA,
                LAVCOD_CONFERIMENTO_DIVERSI,
                LAVCOD_AUTOCONSUMO, LAVCOD_AUTOCONSUMO_VINO_SFUSO, LAVCOD_DOCO_EMESSO, LAVCOD_DAA_EMESSO

                Return -1 * valore

            Case LAVCOD_BOLLA_RICEVUTA, LAVCOD_FATTURA_RICEVUTA,
                LAVCOD_FATTURA_LIQ_CONF_RICEVUTA,
                LAVCOD_FATTURA_PROFESSIONISTI,
                LAVCOD_ALTRI_COSTI, LAVCOD_ACQUISTO,
                LAVCOD_NOTA_ACCREDITO_RICEVUTA,
                LAVCOD_ORDINE_ACQUISTO,
                LAVCOD_CONFERIMENTO, LAVCOD_DOCO_RICEVUTO,
                LAVCOD_ACCETTAZIONE_DIVERSI, LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE, LAVCOD_DISTINTA_CARICO_ACCETTAZIONE

                Return valore

            Case Else

                Return valore

        End Select


    End Function

    '####################################################################################################################
    'gestione della visualizzazione dell'iva indetraibile senza il segno negativo
    'WIP
    Public Function Leggi_IVAIndet_PositivaNegativa(ByVal lavCod As Integer, ByVal valore As Decimal) As Decimal

        Select Case lavCod

            Case LAVCOD_BOLLA_RICEVUTA, LAVCOD_FATTURA_RICEVUTA,
                LAVCOD_NOTA_ACCREDITO_EMESSA,
                LAVCOD_FATTURA_LIQ_CONF_RICEVUTA,
                LAVCOD_FATTURA_PROFESSIONISTI,
                LAVCOD_ALTRI_COSTI, LAVCOD_ACQUISTO,
                LAVCOD_ORDINE_ACQUISTO,
                LAVCOD_CONFERIMENTO, LAVCOD_DOCO_RICEVUTO,
                LAVCOD_ACCETTAZIONE_DIVERSI, LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE, LAVCOD_DISTINTA_CARICO_ACCETTAZIONE

                Return valore * -1

            Case LAVCOD_BOLLA_EMESSA, LAVCOD_DDT_CONTABILIZZATO_EMESSO, LAVCOD_FATTURA_EMESSA,
                LAVCOD_FATTURA_PROFORMA, LAVCOD_RICEVUTA_EMESSA, LAVCOD_NOTA_ACCREDITO_RICEVUTA,
                LAVCOD_FATTURA_LIQ_CONF_EMESSA,
                LAVCOD_AUTOFATTURA_LIQ_CONF_EMESSA, LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA,
                LAVCOD_ALTRI_RICAVI, LAVCOD_VENDITA, LAVCOD_CORRISPETTIVO_VENDITA_SFUSO,
                LAVCOD_ORDINE_VENDITA, LAVCOD_PREVENTIVO_VENDITA,
                LAVCOD_CONFERIMENTO_DIVERSI,
                LAVCOD_AUTOCONSUMO, LAVCOD_AUTOCONSUMO_VINO_SFUSO, LAVCOD_DOCO_EMESSO, LAVCOD_DAA_EMESSO

                Return valore

            Case Else

                Return valore

        End Select


    End Function


    '####################################################################################################################
    ''a differenza dell'altra visualizza le note di accredito col segno -
    Public Function Leggi_IVAIndet_PositivaNegativa2(ByVal lavCod As Integer, ByVal valore As Decimal) As Decimal

        Select Case lavCod

            Case LAVCOD_BOLLA_RICEVUTA, LAVCOD_FATTURA_RICEVUTA, LAVCOD_NOTA_ACCREDITO_RICEVUTA,
                LAVCOD_FATTURA_LIQ_CONF_RICEVUTA,
                LAVCOD_FATTURA_PROFESSIONISTI,
                LAVCOD_ALTRI_COSTI, LAVCOD_ACQUISTO,
                LAVCOD_ORDINE_ACQUISTO,
                LAVCOD_CONFERIMENTO, LAVCOD_DOCO_RICEVUTO,
                LAVCOD_ACCETTAZIONE_DIVERSI, LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE, LAVCOD_DISTINTA_CARICO_ACCETTAZIONE

                Return valore * -1

            Case LAVCOD_BOLLA_EMESSA, LAVCOD_DDT_CONTABILIZZATO_EMESSO, LAVCOD_FATTURA_EMESSA,
                LAVCOD_FATTURA_PROFORMA, LAVCOD_RICEVUTA_EMESSA,
                LAVCOD_FATTURA_LIQ_CONF_EMESSA,
                LAVCOD_AUTOFATTURA_LIQ_CONF_EMESSA, LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA,
                LAVCOD_ALTRI_RICAVI, LAVCOD_VENDITA, LAVCOD_CORRISPETTIVO_VENDITA_SFUSO,
                LAVCOD_ORDINE_VENDITA, LAVCOD_PREVENTIVO_VENDITA,
                LAVCOD_CONFERIMENTO_DIVERSI,
                LAVCOD_AUTOCONSUMO, LAVCOD_AUTOCONSUMO_VINO_SFUSO, LAVCOD_DOCO_EMESSO, LAVCOD_DAA_EMESSO,
                LAVCOD_NOTA_ACCREDITO_EMESSA

                Return valore

            Case Else

                Return valore

        End Select


    End Function

    '####################################################################################################################
    Public Function Leggi_IVA_PositivaNegativa_RegCorrispettivi(ByVal valore As Decimal) As Decimal

        'poiché nel registro corrispettivi ci vanno:
        '- ricevuti fiscali emesse
        '-corrispettivi vendita
        'ddt contabilizzati emessi
        'non ho bisogno del lav_cod

        Return -1 * valore

    End Function



    '####################################################################################################################
    'gli importi sono tutti positivi, questa funzione visualizza col - gli importi delle note
    Public Function Leggi_Importo_PositivoNegativo(ByVal lavCod As Integer, ByVal valore As Decimal) As Decimal

        Select Case lavCod

            Case LAVCOD_NOTA_ACCREDITO_RICEVUTA, LAVCOD_NOTA_ACCREDITO_EMESSA

                Return -1 * valore

            Case Else

                Return valore

        End Select


    End Function


    '####################################################################################################################
    ' i conti dare del conto economico sono salvati sul db con valore negativo
    ' a video non li devo fare vedere negativi
    'non va bene usare il math.abs perché altrimenti tutto a priori diventa positivo
    Public Function Leggi_SaldoContoCE_PositivoNegativo(ByVal Dare_Avere As String, ByVal valore As Decimal) As Decimal

        Select Case Dare_Avere

            Case "A"
                Return valore

            Case "D"

                Return -1 * valore

            Case Else
                'errore
                Return valore

        End Select


    End Function

    '####################################################################################################################
    Public Function LiquidazioneIVA_Desc_from_Cod(ByVal cod As enum_LiquidazioneIva) As String

        Dim liquidazioneIva As String = ""

        Select Case cod
            Case enum_LiquidazioneIva.NonImpostato
                liquidazioneIva = "Non Impostato"
            Case enum_LiquidazioneIva.Mensile
                liquidazioneIva = "Mensile"
            Case enum_LiquidazioneIva.Trimestrale
                liquidazioneIva = "Trimestrale"
        End Select

        Return liquidazioneIva

    End Function


    '####################################################################################################################
    Public Function EsigibilitaIVA_Desc_from_Cod(ByVal cod As enum_EsigibilitaIva) As String

        Dim EsigibilitaIva As String = ""

        Select Case cod
            Case enum_EsigibilitaIva.Non_Specificata
                EsigibilitaIva = "Non Specificata"
            Case enum_EsigibilitaIva.Immediata
                EsigibilitaIva = "Immediata"
            Case enum_EsigibilitaIva.Differita
                EsigibilitaIva = "Differita"
            Case enum_EsigibilitaIva.Scissione_Pagamenti
                EsigibilitaIva = "Scissione Pagamenti"
            Case enum_EsigibilitaIva.Inversione_Contabile
                EsigibilitaIva = "Inversione Contabile"
        End Select

        Return EsigibilitaIva

    End Function


    '#####################################################################################################
    Public Function Ricava_NumeroDocumento_Con_Sequenza(ByVal Doc_Numero_Sin As String,
                                                        ByVal Doc_Numero As Double,
                                                        ByVal Doc_Numero_Des As String,
                                                        ByVal Lunghezza_Sin As Integer,
                                                        ByVal Lunghezza_Centro As Integer,
                                                        ByVal Lunghezza_Des As Integer,
                                                        ByVal CarattereFormattazione As String
                                                        ) As String

        Dim j As Integer
        Dim Stringa_Vuota_Sin As String = ""
        Dim Stringa_Vuota_Centro As String = ""
        Dim Stringa_Vuota_Des As String = ""
        Dim Numero_Completo As String
        Dim Format_Numero_Sin As String
        Dim Format_Numero_Centro As String
        Dim Format_Numero_Des As String

        For j = 0 To CInt(Lunghezza_Sin) - 1
            Stringa_Vuota_Sin &= CStr(CarattereFormattazione)
        Next

        For j = 0 To CInt(Lunghezza_Centro) - 1
            Stringa_Vuota_Centro &= CStr(CarattereFormattazione)
        Next

        For j = 0 To CInt(Lunghezza_Des) - 1
            Stringa_Vuota_Des &= CStr(CarattereFormattazione)
        Next

        Format_Numero_Sin = Right(Stringa_Vuota_Sin & Doc_Numero_Sin, Lunghezza_Sin)
        Format_Numero_Centro = Right(Stringa_Vuota_Centro & CStr(Doc_Numero), Lunghezza_Centro)
        Format_Numero_Des = Right(Stringa_Vuota_Des & Doc_Numero_Des, Lunghezza_Des)

        Numero_Completo = CStr(Format_Numero_Sin & Format_Numero_Centro & Format_Numero_Des)

        Return Numero_Completo

    End Function

    '#####################################################################################################
    Public Function Ricava_NumeroDocumento_Senza_Sequenza(ByVal Doc_Numero_Sin As String,
                                                          ByVal Doc_Numero As Double,
                                                          ByVal Doc_Numero_Des As String
                                                          ) As String


        Dim numeroCompleto As String

        numeroCompleto = Doc_Numero_Sin & CStr(Doc_Numero) & Doc_Numero_Des

        Return numeroCompleto


    End Function

    '#####################################################################################################
    Public Function Calcola_PesoLordo(ByVal tipoPeso As Integer,
                                      ByVal peso As Decimal,
                                      ByVal taraImballi As Decimal
                                      ) As Decimal

        Dim pesoLordo As Decimal

        Select Case tipoPeso
            Case 0 'lordo
                pesoLordo = peso
            Case 1 'netto
                pesoLordo = peso + taraImballi
        End Select

        Return pesoLordo

    End Function

    Public Function Calcola_PesoLordo_NewConf(ByVal Peso_Totale As Decimal, ByVal Tara_Veicolo As Decimal) As Decimal

        Dim Peso_Lordo As Decimal

        Peso_Lordo = Peso_Totale - Tara_Veicolo

        Return Peso_Lordo

    End Function


    '#####################################################################################################
    Public Function Calcola_PesoNetto(ByVal tipoPeso As Integer,
                                      ByVal peso As Decimal,
                                      ByVal taraImballi As Decimal
                                      ) As Decimal

        Dim pesoNetto As Decimal

        Select Case tipoPeso
            Case 0 'lordo
                pesoNetto = peso - taraImballi
            Case 1 'netto
                pesoNetto = peso
        End Select

        Return pesoNetto

    End Function

    '####################################################################################################################
    'Se si modifica questa funzione, bisogna modificare anche la sua gemella, nel GiasOnline
    Public Function Aliquota_from_CodIVA(ByVal codIva As Integer) As String

        Select Case codIva
            Case IVA_4
                Return "4%"
            Case IVA_10
                Return "10%"
            Case IVA_12
                Return "12%"
            Case IVA_20
                Return "20%"
            Case IVA_21
                Return "21%"
            Case FCI
                Return "FCI"
            Case NON_IVABILE
                Return "Non Ivabile"
            Case EsclArt15
                'Return "Escl. Art.15"
                Return "Escl. Art.15 DPR 633" 'sconto merce
            Case EsArt7
                Return "Es. Art.7"
            Case Art74LettC
                Return "Art.74 Lett.C."
            Case Art74LettE
                Return "Art.74 Lett.E"
            Case NonImpArt9
                Return "Non Imp. Art.9"
            Case NonImpArt8
                Return "Non Imp. Art.8"
            Case NonImpArt40
                Return "Non Imp. Art.40"
            Case EsenteArt10
                'Return "Esente Art.10"
                Return "Es. Art.10 DPR 331" 'esenzione per addebito canoni di affitto
            Case EsArt14L537
                Return "Es. Art.14 L.537"
            Case Art44comma
                Return "Art.4 - 4° comma"
            Case Art74Ter
                Return "Art.74 Ter"
            Case NoNImpArt72
                Return "Non Imp. Art.72"
            Case NonImpArt26
                Return "Non Imp. Art.26"
            Case EsclusoArt5
                Return "Escluso Art.5"
            Case Art3SubA
                Return "Art.3 Sub a."
            Case EsclArt134
                Return "Escl. Art.1-3-4"
            Case Art4DL331
                'Return "Art.41 D.L.331"
                Return "Es. Art.41 DPR 331" 'merce estero esenzione CEE
            Case Art18DPR633
                Return "Es. Art.18 DPR 633" 'campione gratuito
            Case Art71DPR331
                Return "Es. Art.71 DPR 331" 'esenzioni canoni in affitto
            Case Art10_16DPR633_72
                Return "Es.Art.10/16 DPR633/72"
            Case Art2DPR633_72
                Return "Es.Art.2 DPR633/72" ' CAMPIONI PER DEGUSTAZIONE FUORI CAMPO I.V.A. 
            Case NonImpArt8C1LetB
                Return "Non Imp. Art.8; c.1; lett.B DPR 633/72" ' cessioni ad esportatori non residenti (quando il bene viene esportato a cura del cliente)
            Case NonImpArt8C1LetC
                Return "Non Imp. Art.8; c.1; lett.C DPR 633/72" ' esportrazioni indirette - dichiarazione d'intenti (quando chi compra il bene
                'ha emesso una dichiarazione di intenti essendo esportatore abituale e vuole quindi la fattura senza IVA)
            Case NonImpArt8DPR633_72
                Return "Non Imp. Art.8 DPR 633/72"

        End Select
        Return ""
    End Function

    Public Function RegimeIVA_Desc_from_Cod(ByVal cod As enum_RegimeIva) As String

        Dim regimeIVA As String = ""

        Select Case cod
            Case enum_RegimeIva.NonImpostato
                regimeIVA = "Non Impostato"
            Case enum_RegimeIva.Ordinario
                regimeIVA = "Ordinario"
            Case enum_RegimeIva.Speciale
                regimeIVA = "Speciale"
            Case enum_RegimeIva.EsenzioneIva
                regimeIVA = "Esenzione Iva"
        End Select

        Return regimeIVA

    End Function


    '####################################################################################################################
    'in base all'elem_cod della categoria di magazzino sensibile, ritorna il tipo di lotto
    Public Function TipoLotto_from_ElemCod(ByVal elemCod As Integer) As String

        Select Case elemCod
            Case SEMENTI
                Return "Lotto del Seme"
            Case SEMILAVORATI_VEGETALI
                Return "Lotto Accettazione"
            Case TRASFORMATI_VEGETALI
                Return "Lotto di Produzione"
            Case SEMILAVORATI_ANIMALI
                Return "Lotto di Macellazione"
            Case TRASFORMATI_ANIMALI
                Return "Lotto di Produzione"
        End Select
        Return ""
    End Function


    '####################################################################################################################
    Public Function GestioneVettore_from_Id_Gestione_Vettore(ByVal idGestioneVettore As enum_GestioneVettore) As String

        Select Case idGestioneVettore
            Case enum_GestioneVettore.Nessuno
                Return ""
            Case enum_GestioneVettore.FrancoArrivo
                Return "Franco Arrivo"
            Case enum_GestioneVettore.FrancoPartenza
                Return "Franco Partenza"
            Case enum_GestioneVettore.FrancoSpedizioniere
                Return "Franco Spedizioniere"
            Case enum_GestioneVettore.FrancoLungoBordo
                Return "Franco Lungo Bordo"
            Case enum_GestioneVettore.FrancoABordo
                Return "Franco a Bordo"
            Case enum_GestioneVettore.CostoENolo
                Return "Costo e Nolo"
            Case enum_GestioneVettore.CostoAssicurazioneENolo
                Return "Costo Assicurazione e Nolo"
            Case enum_GestioneVettore.TrasportoPagatoFinoA
                Return "Trasporto Pagato Fino a"
            Case enum_GestioneVettore.TrasportoEAssicurazionePagatiFinoA
                Return "Trasporto e Assicurazione Pagati Fino a"
            Case enum_GestioneVettore.ResoFrontiera
                Return "Reso Frontiera"
            Case enum_GestioneVettore.ResoExShip
                Return "Reso Ex Ship"
            Case enum_GestioneVettore.ResoBanchina
                Return "Reso Banchina"
            Case enum_GestioneVettore.ResoNonSdoganato
                Return "Reso Non Sdoganato"
            Case enum_GestioneVettore.ResoSdoganato
                Return "Reso Sdoganato"
            Case Else
                Return ""
        End Select

    End Function

    '####################################################################################################################
    Public Sub DataInizioFineMese_from_Data(ByRef dataInizio As Date, ByRef dataFine As Date, ByVal data As Date)

        Dim mese As Integer = data.Month
        Dim anno As Integer = data.Year
        Dim strMese As String = Right("00" & CStr(mese), 2)
        Dim strGiorno As String

        Select Case mese
            Case 11, 4, 6, 9
                strGiorno = "30"
            Case 2
                Dim myCal As New Globalization.TaiwanCalendar
                If myCal.IsLeapYear(anno, Globalization.TaiwanCalendar.CurrentEra) = True Then
                    strGiorno = "29"
                Else
                    strGiorno = "28"
                End If
                strGiorno = "28"
            Case Else
                strGiorno = "31"
        End Select

        dataInizio = CDate("01/" & strMese & "/" & CStr(anno))

        dataFine = CDate(strGiorno & "/" & strMese & "/" & CStr(anno))

    End Sub



    '################################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Spostata dal modulo Contabilità.
    ''' </summary>
    ''' <param name="CodFisc"></param>
    ''' <param name="Flag_Iniziale_Completo"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	19/04/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function Sesso_from_CodFisc(ByVal CodFisc As String,
                                       ByVal Flag_Iniziale_Completo As Integer
                                       ) As String

        Dim sesso As String = ""

        If CodFisc <> "" AndAlso Not IsNumeric(CodFisc) Then

            Select Case Flag_Iniziale_Completo

                Case 1
                    If CodFisc.Substring(9, 1) < "4" Then
                        sesso = "M"
                    Else
                        sesso = "F"
                    End If

                Case 2
                    If CodFisc.Substring(9, 1) >= "4" Then
                        sesso = "Maschio"
                    Else
                        sesso = "Femmina"
                    End If

            End Select

        End If

        'Restituisco il risultato
        Return sesso

    End Function


    '################################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Spostata dal modulo Contabilità.
    ''' </summary>
    ''' <param name="CodFisc"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	19/04/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function DataNascita_from_CodFisc(ByVal CodFisc As String) As String

        Dim dataNascita As String = AGRODATAINIZIO.ToShortDateString

        If CodFisc <> "" AndAlso Not IsNumeric(CodFisc) Then

            If VerificaEspressioneRegolare(CodFisc, "", enum_EspressioniRegolari.RegExp_CodiceFiscale) Then

                CodFisc = UCase(CodFisc)

                Dim giorno As String = ""
                Dim mese As String = ""
                Dim anno As String = ""

                anno = "19" & CodFisc.Substring(6, 2)

                Select Case CodFisc.Substring(8, 1)
                    Case "A"
                        mese = "01"
                    Case "B"
                        mese = "02"
                    Case "C"
                        mese = "03"
                    Case "D"
                        mese = "04"
                    Case "E"
                        mese = "05"
                    Case "H"
                        mese = "06"
                    Case "L"
                        mese = "07"
                    Case "M"
                        mese = "08"
                    Case "P"
                        mese = "09"
                    Case "R"
                        mese = "10"
                    Case "S"
                        mese = "11"
                    Case "T"
                        mese = "12"
                End Select

                Select Case Sesso_from_CodFisc(CodFisc, 1)
                    Case "M"
                        giorno = CodFisc.Substring(9, 2)
                    Case "F"
                        giorno = CStr(CInt(CodFisc.Substring(9, 2)) - 40)
                End Select

                dataNascita = giorno & "/" & mese & "/" & anno

                If IsDate(dataNascita) Then
                    dataNascita = Format(CDate(dataNascita), "dd/MM/yyyy")
                Else
                    dataNascita = ""
                End If

            End If

        End If

        Return dataNascita


    End Function

    '##########################################################################
    'Tipo_Persona_str = "" non valorizzato
    Public Shared Function Ricava_IdCF_ChkFittizio_Contatto(ByVal Cod_Contatto As String,
                                                         ByVal Tipo_Persona_str As String,
                                                         ByVal Flag_Estero As Boolean,
                                                         ByVal chkFittizioEstero As Integer,
                                                         ByRef Flag_Associazione As Boolean,
                                                         ByRef ChkFittizio As Integer
                                                         ) As Integer

        Dim ID_CF As Integer
        Flag_Associazione = False

        ChkFittizio = 0

        If Flag_Estero = True Then
            ID_CF = CONTATTO_ESTERO
            ChkFittizio = chkFittizioEstero
        Else
            'italiano
            If IsNumeric(Cod_Contatto) AndAlso CDbl(Cod_Contatto) < 0 Then
                'codice fiscale sconosciuto modalità giaslan
                '28/10/2019: i contatti fittizi ora vengono importati nella modalità giasonline (con F davanti)
                ID_CF = PERSONA_FISICA
                ChkFittizio = 1
            ElseIf IsNumeric(Cod_Contatto) AndAlso Cod_Contatto.Length = 11 Then

                '14/01/2019: aggiornamento, le associazioni vanno importate come persone giuridiche
                'lo spesometro non esiste più
                'in ogni caso quello e l'invio al sian riconoscono questi cod_contatto e l'inviano come vanno inviati per non dare errore
                If Cod_Contatto.StartsWith("8") OrElse Cod_Contatto.StartsWith("9") Then
                    'sono associazioni
                    Flag_Associazione = True

                    ''NOTA X SPESOMETRO:
                    '' il Giaslan non ritiene valida la combinazione persona fisica con codice fiscale delle associazioni
                    ''(tali contatti si riescono a inserire solo come P.G.)
                    ''ma per lo spesometro vanno impostate come PF
                    'ID_CF = PERSONA_FISICA
                    ID_CF = PERSONA_GIURIDICA
                Else
                    'p.g. con piva
                    ID_CF = PERSONA_GIURIDICA
                End If 'associazioni

            ElseIf IsNumeric(Cod_Contatto) AndAlso Cod_Contatto.Length <> 11 Then
                'piva con numero sbagliato di caratteri
                ID_CF = PERSONA_GIURIDICA
                ChkFittizio = 1

            ElseIf Cod_Contatto.Length = 11 AndAlso Cod_Contatto.StartsWith("F") AndAlso
                   (IsNumeric(Mid(Cod_Contatto, 2, Cod_Contatto.Length - 1))) Then
                'codice fiscale sconosciuto modalità giasonline (con F davanti)
                ChkFittizio = 1
                Select Case Tipo_Persona_str
                    Case ""
                        'come default lascio persona fisica
                        ID_CF = PERSONA_FISICA
                    Case "PF"
                        ID_CF = PERSONA_FISICA
                    Case "PG"
                        ID_CF = PERSONA_GIURIDICA
                    Case Else
                        'valori sbagliati
                        'come default lascio persona fisica
                        ID_CF = PERSONA_FISICA
                End Select
            Else
                'tutti gli altri casi:
                'codice fiscale
                ID_CF = PERSONA_FISICA
            End If 'piva


        End If 'estero

        Return ID_CF

    End Function

    '###########################################################################
    Public Function Bypass_Imputazione_Conto(ByVal lavCod As Long) As Boolean

        Select Case lavCod

            Case LAVCOD_BOLLA_RICEVUTA, LAVCOD_DDT_CONTABILIZZATO_EMESSO, LAVCOD_BOLLA_EMESSA,
                LAVCOD_CARICO, LAVCOD_SCARICO, LAVCOD_ALTRI_RICAVI_NERO, LAVCOD_ALTRI_COSTI_NERO,
                LAVCOD_TRASFERIMENTO, LAVCOD_CONFERIMENTO, LAVCOD_CONFERIMENTO_DIVERSI,
                LAVCOD_DOCO_EMESSO, LAVCOD_DOCO_RICEVUTO, LAVCOD_DAA_EMESSO, LAVCOD_FATTURA_PROFORMA,
                LAVCOD_ORDINE_VENDITA, LAVCOD_ORDINE_ACQUISTO, LAVCOD_PREVENTIVO_VENDITA,
                LAVCOD_MVV_EMESSO, LAVCOD_MVV_RICEVUTO
                'Bolla, Carico/Scarico, Altri Costi e Ricavi, Conferimento, Trasferimento, Doco, DAA, Fattura ProForma,
                'Ordini, Preventivo Vendita, MMV

                Bypass_Imputazione_Conto = True 'Conto Non Imputato

            Case Else

                Bypass_Imputazione_Conto = False

        End Select


    End Function



    '##############################################################
    'da usare quando si ha il dt restituito da AgronicaCoreContabDAL.Pagamenti_Causali_R.Leggi()
    Public Function CauPagamentoSigla_from_CauPagamento(ByVal dt As DataTable,
                                                        ByVal cauPagamento As Integer
                                                        ) As String

        Dim des As String = ""

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then

            Dim dr() As DataRow

            dr = dt.Select(" Cau_Pagamento = " & Agro_SQL_SaveNum(cauPagamento))

            If Not IsNothing(dr) AndAlso dr.Length > 0 Then
                des = CStr(dr(0).Item("Cau_Pagamento_Sigla"))
            End If

        End If

        Return des

    End Function

    '##############################################################
    Public Function Des_from_Cod(ByVal dt As DataTable,
                                 ByVal nomeCampoCod As String,
                                 ByVal nomeCampoDes As String,
                                 ByVal codice As Integer
                                 ) As String

        Dim des As String = ""

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then

            Dim dr() As DataRow

            dr = dt.Select(" " & nomeCampoCod & " = " & Agro_SQL_SaveNum(codice))

            If Not IsNothing(dr) AndAlso dr.Length > 0 Then
                des = CStr(dr(0).Item(nomeCampoDes))
            End If

        End If

        Return des

    End Function

    '##############################################################
    Public Function Des_from_CodStr(ByVal dt As DataTable,
                                    ByVal nomeCampoCod As String,
                                    ByVal nomeCampoDes As String,
                                    ByVal strCodice As String) As String

        Dim des As String = ""

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then

            Dim dr() As DataRow

            dr = dt.Select(" " & nomeCampoCod & " = '" & Agro_SQL_SaveText(strCodice) & "' ")

            If Not IsNothing(dr) AndAlso dr.Length > 0 Then
                des = CStr(dr(0).Item(nomeCampoDes))
            End If

        End If

        Return des

    End Function

    '##############################################################
    Public Function Cod_from_Cod(ByVal dt As DataTable,
                                 ByVal nomeCampoCodFiltro As String,
                                 ByVal nomeCampoCodRecupero As String,
                                 ByVal codice As Integer
                                 ) As Integer

        Dim cod As Integer = 0

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then

            Dim dr() As DataRow

            dr = dt.Select(" " & nomeCampoCodFiltro & " = " & Agro_SQL_SaveNum(codice))

            If Not IsNothing(dr) AndAlso dr.Length > 0 Then
                cod = CInt(dr(0).Item(nomeCampoCodRecupero))
            End If

        End If

        Return cod

    End Function
    '##############################################################
    Public Function ValueDbl_from_Cod(ByVal dt As DataTable,
                                      ByVal nomeCampoCod As String,
                                      ByVal nomeCampoDbl As String,
                                      ByVal codice As Integer
                                      ) As Decimal

        Dim value As Decimal = 0

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then

            Dim dr() As DataRow

            dr = dt.Select(" " & nomeCampoCod & " = " & Agro_SQL_SaveNum(codice))

            If Not IsNothing(dr) AndAlso dr.Length > 0 Then
                value = CDec(dr(0).Item(nomeCampoDbl))
            End If

        End If

        Return value

    End Function

    '##############################################################
    Public Function ValueDbl_from_CodStr(ByVal dt As DataTable,
                                         ByVal nomeCampoCod As String,
                                         ByVal nomeCampoDbl As String,
                                         ByVal codice As String
                                         ) As Decimal

        Dim value As Decimal = 0

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then

            Dim dr() As DataRow

            dr = dt.Select(" " & nomeCampoCod & " = '" & Agro_SQL_SaveText(codice) & "' ")

            If Not IsNothing(dr) AndAlso dr.Length > 0 Then
                value = CDec(dr(0).Item(nomeCampoDbl))
            End If

        End If

        Return value

    End Function

    '##############################################################
    Public Function ValueInt_from_CodStr(ByVal dt As DataTable,
                                         ByVal nomeCampoFiltro As String,
                                        ByVal nomeCampoRecupero As String,
                                         ByVal codice As String
                                         ) As Decimal

        Dim value As Integer = 0

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then

            Dim dr() As DataRow

            dr = dt.Select(" " & nomeCampoFiltro & " = '" & Agro_SQL_SaveText(codice) & "' ")

            If Not IsNothing(dr) AndAlso dr.Length > 0 Then
                value = CInt(dr(0).Item(nomeCampoRecupero))
            End If

        End If

        Return value

    End Function


    Public Sub Recupera_Cod_Des_ContiEconomici(ByVal dtCodifiche As DataTable,
                                               ByRef Id_Riclassificazione_RicavixIVAincompensazione As String,
                                               ByRef Cod_Conto_RicavixIVAincompensazione As Integer,
                                               ByRef Conto_Descr_RicavixIVAincompensazione As String,
                                               ByRef Id_Riclassificazione_OmaggiAllaClientela As String,
                                               ByRef Cod_Conto_OmaggiAllaClientela As Integer,
                                               ByRef Conto_Descr_OmaggiAllaClientela As String,
                                               ByRef Id_Riclassificazione_RicavixIVAincompensazioneEstero As String,
                                               ByRef Cod_Conto_RicavixIVAincompensazioneEstero As Integer,
                                               ByRef Conto_Descr_RicavixIVAincompensazioneEstero As String)


        'ByRef Id_Riclassificazione_CostixIVAindetraibile As String,
        'ByRef Conto_Descr_CostixIVAindetraibile As String,
        'ByRef Cod_Conto_CostixIVAindetraibile As Integer

        Id_Riclassificazione_RicavixIVAincompensazione = Des_from_Cod(dtCodifiche,
                                                                      "Codifica_Conto",
                                                                      "Id_Riclassificazione",
                                                                      enum_Conti_Economici.RicavixIVAincompensazione)

        Conto_Descr_RicavixIVAincompensazione = Des_from_Cod(dtCodifiche,
                                                             "Codifica_Conto",
                                                             "Conto_Descr",
                                                             enum_Conti_Economici.RicavixIVAincompensazione)

        Cod_Conto_RicavixIVAincompensazione = Cod_from_Cod(dtCodifiche,
                                                           "Codifica_Conto",
                                                           "Cod_Conto",
                                                           enum_Conti_Economici.RicavixIVAincompensazione)

        '----

        Id_Riclassificazione_RicavixIVAincompensazioneEstero = Des_from_Cod(dtCodifiche,
                                                                            "Codifica_Conto",
                                                                            "Id_Riclassificazione",
                                                                            enum_Conti_Economici.RicavixIVAincompensazioneEstero)

        Conto_Descr_RicavixIVAincompensazioneEstero = Des_from_Cod(dtCodifiche,
                                                                   "Codifica_Conto",
                                                                   "Conto_Descr",
                                                                   enum_Conti_Economici.RicavixIVAincompensazioneEstero)

        Cod_Conto_RicavixIVAincompensazioneEstero = Cod_from_Cod(dtCodifiche,
                                                                 "Codifica_Conto",
                                                                 "Cod_Conto",
                                                                 enum_Conti_Economici.RicavixIVAincompensazioneEstero)

        '----

        Id_Riclassificazione_OmaggiAllaClientela = Des_from_Cod(dtCodifiche,
                                                                "Codifica_Conto",
                                                                "Id_Riclassificazione",
                                                                enum_Conti_Economici.OmaggiAllaClientela)

        Conto_Descr_OmaggiAllaClientela = Des_from_Cod(dtCodifiche,
                                                       "Codifica_Conto",
                                                       "Conto_Descr",
                                                       enum_Conti_Economici.OmaggiAllaClientela)

        Cod_Conto_OmaggiAllaClientela = Cod_from_Cod(dtCodifiche,
                                                     "Codifica_Conto",
                                                     "Cod_Conto",
                                                     enum_Conti_Economici.OmaggiAllaClientela)

        '----

        'Id_Riclassificazione_CostixIVAindetraibile = Des_from_Cod(DT_Codifiche,
        '                                                     "Codifica_Conto",
        '                                                     "Id_Riclassificazione",
        '                                                     enum_Conti_Economici.CostixIVAindetraibile)

        'Conto_Descr_CostixIVAindetraibile = Des_from_Cod(DT_Codifiche,
        '                                                   "Codifica_Conto",
        '                                                   "Conto_Descr",
        '                                                   enum_Conti_Economici.CostixIVAindetraibile)

        'Cod_Conto_CostixIVAindetraibile = Cod_from_Cod(DT_Codifiche,
        '                                           "Codifica_Conto",
        '                                           "Cod_Conto",
        '                                           enum_Conti_Economici.CostixIVAindetraibile)

        '----

    End Sub

    '###############################################################################
    'Primi 4 parametri (crediti/debiti/banche/cassa) seguenti servono per gestire il collegamento alle anagrafiche
    'Successivi 4 parametri (le iva): il loro codice lo deve dedurre la query perché non è salvato sull'operazione
    Public Sub Recupera_Cod_Des_ContiPatrimoniali(ByVal dtCodifiche As DataTable,
                                                  ByRef Id_Riclassificazione_CreditiVersoClienti As String,
                                                  ByRef Id_Riclassificazione_DebitiVersoFornitori As String,
                                                  ByRef Id_Riclassificazione_DepositiBancariPostali As String,
                                                  ByRef Id_Riclassificazione_DenaroValoriInCassa As String,
                                                  ByRef Id_Riclassificazione_IvaACredito As String,
                                                  ByRef Id_Riclassificazione_IvaACredito_AcqIntra As String,
                                                  ByRef Id_Riclassificazione_IvaADebito As String,
                                                  ByRef Id_Riclassificazione_IvaADebito_AcqIntra As String,
                                                  ByRef Conto_Pat_Descr_CreditiVersoClienti As String,
                                                  ByRef Conto_Pat_Descr_DebitiVersoFornitori As String,
                                                  ByRef Conto_Pat_Descr_DepositiBancariPostali As String,
                                                  ByRef Conto_Pat_Descr_DenaroValoriInCassa As String,
                                                  ByRef Conto_Pat_Descr_IvaACredito As String,
                                                  ByRef Conto_Pat_Descr_IvaACredito_AcqIntra As String,
                                                  ByRef Conto_Pat_Descr_IvaADebito As String,
                                                  ByRef Conto_Pat_Descr_IvaADebito_AcqIntra As String,
                                                  ByRef Cod_Conto_Pat_CreditiVersoClienti As Integer,
                                                  ByRef Cod_Conto_Pat_DebitiVersoFornitori As Integer,
                                                  ByRef Cod_Conto_Pat_DepositiBancariPostali As Integer,
                                                  ByRef Cod_Conto_Pat_DenaroValoriInCassa As Integer,
                                                  ByRef Cod_Conto_Pat_IvaACredito As Integer,
                                                  ByRef Cod_Conto_Pat_IvaACredito_AcqIntra As Integer,
                                                  ByRef Cod_Conto_Pat_IvaADebito As Integer,
                                                  ByRef Cod_Conto_Pat_IvaADebito_AcqIntra As Integer)

        'ByRef Id_Riclassificazione_ErarioRitenuteLavoroAutonomo As String,
        'ByRef Id_Riclassificazione_DebitiVsEnasarco As String,
        'ByRef Conto_Pat_Descr_ErarioRitenuteLavoroAutonomo As String,
        ' ByRef Conto_Pat_Descr_DebitiVsEnasarco As String,
        ' ByRef Cod_Conto_Pat_ErarioRitenuteLavoroAutonomo As Integer,
        ' ByRef Cod_Conto_Pat_DebitiVsEnasarco As Integer

        Id_Riclassificazione_CreditiVersoClienti = Des_from_Cod(dtCodifiche,
                                                             "Codifica_Conto_Pat",
                                                             "Id_Riclassificazione",
                                                             enum_Conti_Patrimoniali.CreditiVersoClienti)

        Conto_Pat_Descr_CreditiVersoClienti = Des_from_Cod(dtCodifiche,
                                                           "Codifica_Conto_Pat",
                                                           "Conto_Pat_Descr",
                                                           enum_Conti_Patrimoniali.CreditiVersoClienti)

        Cod_Conto_Pat_CreditiVersoClienti = Cod_from_Cod(dtCodifiche,
                                                      "Codifica_Conto_Pat",
                                                      "Cod_Conto_Pat",
                                                      enum_Conti_Patrimoniali.CreditiVersoClienti)

        '----

        Id_Riclassificazione_DebitiVersoFornitori = Des_from_Cod(dtCodifiche,
                                                             "Codifica_Conto_Pat",
                                                             "Id_Riclassificazione",
                                                             enum_Conti_Patrimoniali.DebitiVersoFornitori)

        Conto_Pat_Descr_DebitiVersoFornitori = Des_from_Cod(dtCodifiche,
                                                           "Codifica_Conto_Pat",
                                                           "Conto_Pat_Descr",
                                                           enum_Conti_Patrimoniali.DebitiVersoFornitori)

        Cod_Conto_Pat_DebitiVersoFornitori = Cod_from_Cod(dtCodifiche,
                                                   "Codifica_Conto_Pat",
                                                   "Cod_Conto_Pat",
                                                   enum_Conti_Patrimoniali.DebitiVersoFornitori)

        '----

        Id_Riclassificazione_DepositiBancariPostali = Des_from_Cod(dtCodifiche,
                                                             "Codifica_Conto_Pat",
                                                             "Id_Riclassificazione",
                                                             enum_Conti_Patrimoniali.DepositiBancariPostali)

        Conto_Pat_Descr_DepositiBancariPostali = Des_from_Cod(dtCodifiche,
                                                              "Codifica_Conto_Pat",
                                                              "Conto_Pat_Descr",
                                                              enum_Conti_Patrimoniali.DepositiBancariPostali)

        Cod_Conto_Pat_DepositiBancariPostali = Cod_from_Cod(dtCodifiche,
                                                  "Codifica_Conto_Pat",
                                                  "Cod_Conto_Pat",
                                                  enum_Conti_Patrimoniali.DepositiBancariPostali)

        '----

        Id_Riclassificazione_DenaroValoriInCassa = Des_from_Cod(dtCodifiche,
                                                             "Codifica_Conto_Pat",
                                                             "Id_Riclassificazione",
                                                             enum_Conti_Patrimoniali.DenaroValoriInCassa)

        Conto_Pat_Descr_DenaroValoriInCassa = Des_from_Cod(dtCodifiche,
                                                           "Codifica_Conto_Pat",
                                                           "Conto_Pat_Descr",
                                                           enum_Conti_Patrimoniali.DenaroValoriInCassa)

        Cod_Conto_Pat_DenaroValoriInCassa = Cod_from_Cod(dtCodifiche,
                                                         "Codifica_Conto_Pat",
                                                         "Cod_Conto_Pat",
                                                         enum_Conti_Patrimoniali.DenaroValoriInCassa)

        '----

        Id_Riclassificazione_IvaACredito = Des_from_Cod(dtCodifiche,
                                                        "Codifica_Conto_Pat",
                                                        "Id_Riclassificazione",
                                                        enum_Conti_Patrimoniali.IvaACredito)

        Conto_Pat_Descr_IvaACredito = Des_from_Cod(dtCodifiche,
                                                   "Codifica_Conto_Pat",
                                                   "Conto_Pat_Descr",
                                                   enum_Conti_Patrimoniali.IvaACredito)

        Cod_Conto_Pat_IvaACredito = Cod_from_Cod(dtCodifiche,
                                                 "Codifica_Conto_Pat",
                                                 "Cod_Conto_Pat",
                                                 enum_Conti_Patrimoniali.IvaACredito)

        '----


        Id_Riclassificazione_IvaACredito_AcqIntra = Des_from_Cod(dtCodifiche,
                                                                 "Codifica_Conto_Pat",
                                                                 "Id_Riclassificazione",
                                                                 enum_Conti_Patrimoniali.IvaACreditoAcqIntra)

        Conto_Pat_Descr_IvaACredito_AcqIntra = Des_from_Cod(dtCodifiche,
                                                            "Codifica_Conto_Pat",
                                                            "Conto_Pat_Descr",
                                                            enum_Conti_Patrimoniali.IvaACreditoAcqIntra)

        Cod_Conto_Pat_IvaACredito_AcqIntra = Cod_from_Cod(dtCodifiche,
                                                          "Codifica_Conto_Pat",
                                                          "Cod_Conto_Pat",
                                                          enum_Conti_Patrimoniali.IvaACreditoAcqIntra)

        '----


        Id_Riclassificazione_IvaADebito = Des_from_Cod(dtCodifiche,
                                                       "Codifica_Conto_Pat",
                                                       "Id_Riclassificazione",
                                                       enum_Conti_Patrimoniali.IvaADebito)

        Conto_Pat_Descr_IvaADebito = Des_from_Cod(dtCodifiche,
                                                  "Codifica_Conto_Pat",
                                                  "Conto_Pat_Descr",
                                                  enum_Conti_Patrimoniali.IvaADebito)

        Cod_Conto_Pat_IvaADebito = Cod_from_Cod(dtCodifiche,
                                                "Codifica_Conto_Pat",
                                                "Cod_Conto_Pat",
                                                enum_Conti_Patrimoniali.IvaADebito)

        '----

        Id_Riclassificazione_IvaADebito_AcqIntra = Des_from_Cod(dtCodifiche,
                                                                "Codifica_Conto_Pat",
                                                                "Id_Riclassificazione",
                                                                enum_Conti_Patrimoniali.IvaADebitoAcqIntra)

        Conto_Pat_Descr_IvaADebito_AcqIntra = Des_from_Cod(dtCodifiche,
                                                           "Codifica_Conto_Pat",
                                                           "Conto_Pat_Descr",
                                                           enum_Conti_Patrimoniali.IvaADebitoAcqIntra)

        Cod_Conto_Pat_IvaADebito_AcqIntra = Cod_from_Cod(dtCodifiche,
                                                         "Codifica_Conto_Pat",
                                                         "Cod_Conto_Pat",
                                                         enum_Conti_Patrimoniali.IvaADebitoAcqIntra)

        '----

    End Sub


    Public Sub Recupera_IdRiclassificazione_ContiEconomici(ByVal dtCodifiche As DataTable,
                                                           ByRef Id_Riclassificazione_RicavixIVAincompensazione As String,
                                                           ByRef Id_Riclassificazione_OmaggiAllaClientela As String)

        ' ByRef Id_Riclassificazione_CostixIVAindetraibile As String
        Id_Riclassificazione_RicavixIVAincompensazione = Des_from_Cod(dtCodifiche,
                                                                      "Codifica_Conto",
                                                                      "Id_Riclassificazione",
                                                                      enum_Conti_Economici.RicavixIVAincompensazione)

        '----

        Id_Riclassificazione_OmaggiAllaClientela = Des_from_Cod(dtCodifiche,
                                                                "Codifica_Conto",
                                                                "Id_Riclassificazione",
                                                                enum_Conti_Economici.OmaggiAllaClientela)
        '----

    End Sub

    Public Sub Recupera_CodContoPat_EnasarcoRitAcconto(ByVal dtCodifiche As DataTable,
                                                       ByRef Cod_Conto_Pat_DebitiVsEnasarco As Integer,
                                                       ByRef Cod_Conto_Pat_ErarioRitenuteLavoroAutonomo As Integer)


        Cod_Conto_Pat_DebitiVsEnasarco = Cod_from_Cod(dtCodifiche,
                                                      "Codifica_Conto_Pat",
                                                      "Cod_Conto_Pat",
                                                      enum_Conti_Patrimoniali.DebitiVsEnasarco)

        '----

        Cod_Conto_Pat_ErarioRitenuteLavoroAutonomo = Cod_from_Cod(dtCodifiche,
                                                                  "Codifica_Conto_Pat",
                                                                  "Cod_Conto_Pat",
                                                                  enum_Conti_Patrimoniali.ErarioRitenuteLavoroAutonomo)


    End Sub


    Public Sub Recupera_CodContoPat_ContiPatrimoniali(ByVal dtCodifiche As DataTable,
                                                      ByRef Cod_Conto_Pat_CreditiVersoClienti As Integer,
                                                      ByRef Cod_Conto_Pat_DebitiVersoFornitori As Integer,
                                                      ByRef Cod_Conto_Pat_DepositiBancariPostali As Integer,
                                                      ByRef Cod_Conto_Pat_DenaroValoriInCassa As Integer,
                                                      ByRef Cod_Conto_Pat_IvaACredito As Integer,
                                                      ByRef Cod_Conto_Pat_IvaACredito_AcqIntra As Integer,
                                                      ByRef Cod_Conto_Pat_IvaADebito As Integer,
                                                      ByRef Cod_Conto_Pat_IvaADebito_AcqIntra As Integer)


        Cod_Conto_Pat_CreditiVersoClienti = Cod_from_Cod(dtCodifiche,
                                                         "Codifica_Conto_Pat",
                                                         "Cod_Conto_Pat",
                                                         enum_Conti_Patrimoniali.CreditiVersoClienti)

        '----

        Cod_Conto_Pat_DebitiVersoFornitori = Cod_from_Cod(dtCodifiche,
                                                          "Codifica_Conto_Pat",
                                                          "Cod_Conto_Pat",
                                                          enum_Conti_Patrimoniali.DebitiVersoFornitori)

        '----

        Cod_Conto_Pat_DepositiBancariPostali = Cod_from_Cod(dtCodifiche,
                                                            "Codifica_Conto_Pat",
                                                            "Cod_Conto_Pat",
                                                            enum_Conti_Patrimoniali.DepositiBancariPostali)

        '----

        Cod_Conto_Pat_DenaroValoriInCassa = Cod_from_Cod(dtCodifiche,
                                                         "Codifica_Conto_Pat",
                                                         "Cod_Conto_Pat",
                                                         enum_Conti_Patrimoniali.DenaroValoriInCassa)

        '----


        Cod_Conto_Pat_IvaACredito = Cod_from_Cod(dtCodifiche,
                                                 "Codifica_Conto_Pat",
                                                 "Cod_Conto_Pat",
                                                 enum_Conti_Patrimoniali.IvaACredito)

        '----


        Cod_Conto_Pat_IvaACredito_AcqIntra = Cod_from_Cod(dtCodifiche,
                                                          "Codifica_Conto_Pat",
                                                          "Cod_Conto_Pat",
                                                          enum_Conti_Patrimoniali.IvaACreditoAcqIntra)

        '----

        Cod_Conto_Pat_IvaADebito = Cod_from_Cod(dtCodifiche,
                                                "Codifica_Conto_Pat",
                                                "Cod_Conto_Pat",
                                                enum_Conti_Patrimoniali.IvaADebito)

        '----

        Cod_Conto_Pat_IvaADebito_AcqIntra = Cod_from_Cod(dtCodifiche,
                                                         "Codifica_Conto_Pat",
                                                         "Cod_Conto_Pat",
                                                         enum_Conti_Patrimoniali.IvaADebitoAcqIntra)

        '----

    End Sub

    Public Sub Recupera_CodConto_ContiEconomici(ByVal dtCodifiche As DataTable,
                                                ByRef Cod_Conto_RicavixIVAincompensazione As Integer,
                                                ByRef Cod_Conto_OmaggiAllaClientela As Integer)

        ' ByRef Cod_Conto_CostixIVAindetraibile As Integer

        Cod_Conto_RicavixIVAincompensazione = Cod_from_Cod(dtCodifiche,
                                                           "Codifica_Conto",
                                                           "Cod_Conto",
                                                           enum_Conti_Economici.RicavixIVAincompensazione)

        '----

        Cod_Conto_OmaggiAllaClientela = Cod_from_Cod(dtCodifiche,
                                                     "Codifica_Conto",
                                                     "Cod_Conto",
                                                     enum_Conti_Economici.OmaggiAllaClientela)

        '----

        'Cod_Conto_CostixIVAindetraibile = Cod_from_Cod(dtCodifiche,
        '                                           "Codifica_Conto",
        '                                           "Cod_Conto",
        '                                           enum_Conti_Economici.CostixIVAindetraibile)

        '----

    End Sub


    Public Sub Recupera_IdRiclassificazione_ContiPatrimoniali(ByVal dtCodifiche As DataTable,
                                                              ByRef Id_Riclassificazione_CreditiVersoClienti As String,
                                                              ByRef Id_Riclassificazione_DebitiVersoFornitori As String,
                                                              ByRef Id_Riclassificazione_DepositiBancariPostali As String,
                                                              ByRef Id_Riclassificazione_DenaroValoriInCassa As String,
                                                              ByRef Id_Riclassificazione_IvaACredito As String,
                                                              ByRef Id_Riclassificazione_IvaACredito_AcqIntra As String,
                                                              ByRef Id_Riclassificazione_IvaADebito As String,
                                                              ByRef Id_Riclassificazione_IvaADebito_AcqIntra As String)


        Id_Riclassificazione_CreditiVersoClienti = Des_from_Cod(dtCodifiche,
                                                                "Codifica_Conto_Pat",
                                                                "Id_Riclassificazione",
                                                                enum_Conti_Patrimoniali.CreditiVersoClienti)

        '----

        Id_Riclassificazione_DebitiVersoFornitori = Des_from_Cod(dtCodifiche,
                                                                 "Codifica_Conto_Pat",
                                                                 "Id_Riclassificazione",
                                                                 enum_Conti_Patrimoniali.DebitiVersoFornitori)

        '----

        Id_Riclassificazione_DepositiBancariPostali = Des_from_Cod(dtCodifiche,
                                                                   "Codifica_Conto_Pat",
                                                                   "Id_Riclassificazione",
                                                                   enum_Conti_Patrimoniali.DepositiBancariPostali)
        '----

        Id_Riclassificazione_DenaroValoriInCassa = Des_from_Cod(dtCodifiche,
                                                                "Codifica_Conto_Pat",
                                                                "Id_Riclassificazione",
                                                                enum_Conti_Patrimoniali.DenaroValoriInCassa)

        '----

        Id_Riclassificazione_IvaACredito = Des_from_Cod(dtCodifiche,
                                                        "Codifica_Conto_Pat",
                                                        "Id_Riclassificazione",
                                                        enum_Conti_Patrimoniali.IvaACredito)

        '----


        Id_Riclassificazione_IvaACredito_AcqIntra = Des_from_Cod(dtCodifiche,
                                                                 "Codifica_Conto_Pat",
                                                                 "Id_Riclassificazione",
                                                                 enum_Conti_Patrimoniali.IvaACreditoAcqIntra)
        '----


        Id_Riclassificazione_IvaADebito = Des_from_Cod(dtCodifiche,
                                                       "Codifica_Conto_Pat",
                                                       "Id_Riclassificazione",
                                                       enum_Conti_Patrimoniali.IvaADebito)

        '----

        Id_Riclassificazione_IvaADebito_AcqIntra = Des_from_Cod(dtCodifiche,
                                                                "Codifica_Conto_Pat",
                                                                "Id_Riclassificazione",
                                                                enum_Conti_Patrimoniali.IvaADebitoAcqIntra)

    End Sub


    Public Shared Function CalcolaSaldoDareAvere(ByVal tipologiaECO_PAT As String, ByVal importoAvere As Decimal, ByVal importoDare As Decimal) As Decimal

        Dim saldo As Decimal = 0

        Select Case tipologiaECO_PAT
            Case CONTO_ECONOMICO
                saldo = importoAvere - importoDare
            Case CONTO_PATRIMONIALE
                saldo = importoDare - importoAvere
        End Select

        Return saldo

    End Function

    Public Shared Function Costruisci_IBAN(ByVal nazione As String, ByVal cifreControllo As String, ByVal cin As String,
                                           ByVal abi As String, ByVal cab As String, ByVal numeroConto As String,
                                           Optional ByVal flagUsaSpazi As Boolean = False
                                           ) As String

        Select Case flagUsaSpazi
            Case True
                Return nazione & " " & cifreControllo & " " & cin & " " & abi & " " & cab & " " & numeroConto
            Case Else
                Return nazione & cifreControllo & cin & abi & cab & numeroConto
        End Select

    End Function

    Public Shared Function CreaDesLib(ByVal lavCod As Integer,
                                      ByVal docNumSin As String,
                                      ByVal docNum As Integer?,
                                      ByVal docNumDes As String,
                                      ByVal ragSoc As String,
                                      Optional ByVal accompagnatoria As enum_FatturaTipo = enum_FatturaTipo.Differita,
                                      Optional ByVal numeroDocFormattato As String = "",
                                      Optional ByVal progrProtocollo As Integer = 0,
                                      Optional ByVal note As String = "",
                                      Optional ByVal descrizioneAggiuntiva As String = ""
                                      ) As String

        Dim stb As New Text.StringBuilder

        Select Case lavCod
            Case LAVCOD_FATTURA_RICEVUTA
                If accompagnatoria = enum_FatturaTipo.Immediata Then
                    stb.Append(Gias.FatturaAccompagnatoriaRicevuta)
                Else
                    stb.Append(Gias.FatturaRicevuta)
                End If
            Case LAVCOD_FATTURA_EMESSA
                If accompagnatoria = enum_FatturaTipo.Immediata Then
                    stb.Append(Gias.FatturaAccompagnatoriaEmessa)
                Else
                    stb.Append(Gias.FatturaEmessa)
                End If
            Case LAVCOD_NOTA_ACCREDITO_RICEVUTA
                stb.Append(Gias.NotaAccreditoFornitoreRicevuta)
            Case LAVCOD_NOTA_ACCREDITO_EMESSA
                stb.Append(Gias.NotaAccreditoClienteEmessa)
            Case LAVCOD_VENDITA
                stb.Append(Gias.CorrispettivoVendita)
            Case LAVCOD_BOLLA_RICEVUTA
                stb.Append(Gias.DDTRicevuto)
            Case LAVCOD_BOLLA_EMESSA
                stb.Append(Gias.DDTEmesso)
            Case LAVCOD_ACCETTAZIONE_DIVERSI
                stb.Append(Gias.AccettazioneDDTRicevuto)
            Case LAVCOD_DISTINTA_CARICO
                stb.Append(Gias.DistintaDiCarico)
            Case LAVCOD_DISTINTA_CARICO_ACCETTAZIONE
                stb.Append(Gias.AccettazioneDistintaDiCarico)
            Case LAVCOD_AUTO_DDT_EMESSO
                stb.Append(Gias.AutoDDTEmesso)
            Case LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE
                stb.Append(Gias.AccettazioneAutoDDTEmesso)
            Case LAVCOD_ORDINE_VENDITA
                stb.Append(Gias.OrdineVenditaEmesso)
            Case LAVCOD_ORDINE_ACQUISTO
                stb.Append(Gias.OrdineAcquistoEmesso)
            Case LAVCOD_CARICO
                stb.Append(Gias.CaricoDiMagazzino)
            Case LAVCOD_SCARICO
                stb.Append(Gias.ScaricoDiMagazzino)
            Case LAVCOD_CONTRATTO_AFFITTO
                stb.Append(Gias.ContrattoDiAffitto)
            Case Else
                stb.Append("LAV_COD=" & lavCod)
        End Select

        Dim stbAgg As New Text.StringBuilder

        If Not IsNothing(docNum) AndAlso docNum <> 0 Then
            If Not String.IsNullOrEmpty(numeroDocFormattato) Then
                'Se il chiamante si è già occupato di predispormi in numero formattato
                stbAgg.Append(Gias.NumeroAbbr & ". " & numeroDocFormattato)
            Else
                stbAgg.Append(Gias.NumeroAbbr & ". " & docNumSin & " " & CStr(docNum) & " " & docNumDes)
            End If
        End If

        If lavCod = LAVCOD_CARICO OrElse lavCod = LAVCOD_SCARICO Then
            If progrProtocollo > 0 Then
                SeAccodaSpazio(stbAgg)
                stbAgg.Append(Gias.NumeroAbbr & ". " & progrProtocollo.ToString())
            End If
            If Not String.IsNullOrEmpty(note) Then
                SeAccodaSpazio(stbAgg)
                stbAgg.Append(Gias.Note & ": " & Trim(note.Replace(vbCrLf, "").Replace(vbLf, "")))
            End If
        End If

        If Not IsNothing(ragSoc) AndAlso Not String.IsNullOrEmpty(ragSoc) Then
            SeAccodaSpazio(stbAgg)
            stbAgg.Append(Gias.RiferimentoAbbr & ": " & ragSoc.Replace(vbCrLf, "").Replace(vbLf, ""))
        End If

        If Not String.IsNullOrEmpty(stbAgg.ToString()) Then
            stb.Append(" (")
            stb.Append(stbAgg.ToString())
            stb.Append(")")

            If Not IsNothing(descrizioneAggiuntiva) AndAlso Not String.IsNullOrEmpty(descrizioneAggiuntiva) Then
                SeAccodaSpazio(stb)
                stb.Append(descrizioneAggiuntiva.Replace(vbCrLf, "").Replace(vbLf, ""))
            End If

        End If

        Return stb.ToString

    End Function

    Private Shared Sub SeAccodaSpazio(ByRef stbAgg As Text.StringBuilder)
        If Not String.IsNullOrEmpty(stbAgg.ToString()) Then
            stbAgg.Append(" ")
        End If
    End Sub

#Region "Sconti"

    Public Shared Function CalcolaScontoComplessivo(ByVal ParamArray sconti() As Decimal?) As Decimal
        Dim scontoTotale As Decimal = 0D
        If sconti.Length <= 0 Then 
            Return scontoTotale
        End If

        For i As Integer = 0 To UBound(sconti, 1)
            If sconti(i) IsNot Nothing Then
                scontoTotale += ((100 - scontoTotale) * sconti(i)) / 100 
            End If
        Next i

        Return scontoTotale
    End Function

    Public Shared Function ComponiScontoAddizionaleTesto(ByVal ParamArray sconti() As Decimal?) As String
        Dim scontoTesto As String = ""
        If sconti.Length <= 0 Then 
            Return scontoTesto
        End If

        For i As Integer = 0 To UBound(sconti, 1)
            If sconti(i) IsNot Nothing AndAlso sconti(i) <> 0 Then
                scontoTesto &= If(Trim(scontoTesto) = "", "", "-") & CStr(sconti(i))
            End If
        Next i

        Return scontoTesto
    End Function

#End Region

#Region "Layout"

    Public Shared Function GetLayoutFormatiStampa(ByVal ChkLayOut_Peso As Integer,
                                                  ByVal ChkLayOut_Prezzo As Integer,
                                                  ByVal ChkLayOut_Riscontrato As Integer
                                                  ) As enum_LayoutFormatiStampaDoc
        If ChkLayOut_Riscontrato = 2 Then
            Return enum_LayoutFormatiStampaDoc.RiscontratoTotale
        Elseif ChkLayOut_Riscontrato = 1 Then
            Return enum_LayoutFormatiStampaDoc.Riscontrato
        Elseif ChkLayOut_Prezzo = 1 Then
            Return enum_LayoutFormatiStampaDoc.Prezzo
        Elseif ChkLayOut_Peso = 1 Then
            Return enum_LayoutFormatiStampaDoc.Peso
        Else
            Return enum_LayoutFormatiStampaDoc.Standard
        End If
    End Function

    Public Shared Sub SetLayoutFormatiStampa(ByRef ChkLayOut_Peso As Integer,
                                             ByRef ChkLayOut_Prezzo As Integer,
                                             ByRef ChkLayOut_Riscontrato As Integer,
                                             ByVal layout As enum_LayoutFormatiStampaDoc)
        Select Case layout
            Case enum_LayoutFormatiStampaDoc.Standard
                ChkLayOut_Peso = 0
                ChkLayOut_Prezzo = 0
                ChkLayOut_Riscontrato = 0
            Case enum_LayoutFormatiStampaDoc.Peso
                ChkLayOut_Peso = 1
                ChkLayOut_Prezzo = 0
                ChkLayOut_Riscontrato = 0
            Case enum_LayoutFormatiStampaDoc.RiscontratoTotale
                ChkLayOut_Peso = 0
                ChkLayOut_Prezzo = 0
                ChkLayOut_Riscontrato = 2
            Case enum_LayoutFormatiStampaDoc.Riscontrato
                ChkLayOut_Peso = 0
                ChkLayOut_Prezzo = 0
                ChkLayOut_Riscontrato = 1
            Case enum_LayoutFormatiStampaDoc.Prezzo
                ChkLayOut_Peso = 0
                ChkLayOut_Prezzo = 1
                ChkLayOut_Riscontrato = 0
            Case Else
                ChkLayOut_Peso = 0
                ChkLayOut_Prezzo = 0
                ChkLayOut_Riscontrato = 0
        End Select

    End Sub

#End Region

#Region "Scadenza Ordine"

    Public Shared Sub GetScadenzaOrdine(ByRef ChkScadenza As Boolean,
                                        ByRef ChkTassativa As Boolean,
                                        ByVal extraInt As Integer)

        Select Case extraInt

            Case enum_ModalitaScadenzaOrdine.SCAGLIONATA
                ChkScadenza = False
                ChkTassativa = False

            Case enum_ModalitaScadenzaOrdine.SCAGLIONATA_TASSATIVA
                ChkScadenza = False
                ChkTassativa = True

            Case enum_ModalitaScadenzaOrdine.UNICA
                ChkScadenza = True
                ChkTassativa = False

            Case enum_ModalitaScadenzaOrdine.UNICA_TASSATIVA
                ChkScadenza = True
                ChkTassativa = True

        End Select

    End Sub

    Public Shared Function SetScadenzaOrdine(ByVal ChkScadenza As Boolean,
                                             ByVal ChkTassativa As Boolean
                                             ) As enum_ModalitaScadenzaOrdine

        If ChkScadenza = True AndAlso ChkTassativa = False Then
            Return enum_ModalitaScadenzaOrdine.UNICA

        ElseIf ChkScadenza = True AndAlso ChkTassativa = True Then
            Return enum_ModalitaScadenzaOrdine.UNICA_TASSATIVA

        ElseIf ChkScadenza = False AndAlso ChkTassativa = False Then
            Return enum_ModalitaScadenzaOrdine.SCAGLIONATA

        Else
            Return enum_ModalitaScadenzaOrdine.UNICA_TASSATIVA
        End If

    End Function

#End Region

#Region "Stato Ordine"

    Public Shared Function GetStatoOrdineDocumento_Cod(ByVal numRigheProdottoDocumento As Integer,
                                                       Optional ByVal numRigheNonPronte As Integer = 0,
                                                       Optional ByVal numRigheEvase As Integer = 0,
                                                       Optional ByVal numRigheEvaseForzate As Integer = 0,
                                                       Optional ByVal numRigheInevase As Integer = 0,
                                                       Optional ByVal numRigheParzEvase As Integer = 0
                                                       ) As enum_StatoOrdine

        Dim statoOrdine As enum_StatoOrdine

        If numRigheProdottoDocumento > 0 Then
            If numRigheNonPronte > 0 Then
                statoOrdine = enum_StatoOrdine.NON_PRONTO

            ElseIf numRigheEvase = numRigheProdottoDocumento Then
                statoOrdine = enum_StatoOrdine.EVASO

            ElseIf (numRigheEvase + numRigheEvaseForzate) = numRigheProdottoDocumento Then
                statoOrdine = enum_StatoOrdine.EVASO_FORZATAMENTE

            ElseIf numRigheInevase = numRigheProdottoDocumento Then
                statoOrdine = enum_StatoOrdine.INEVASO

            ElseIf numRigheEvase > 0 OrElse numRigheEvaseForzate > 0 OrElse numRigheParzEvase > 0 Then
                statoOrdine = enum_StatoOrdine.PARZIALMENTE_EVASO

            Else
                statoOrdine = enum_StatoOrdine.INDEFINITO
            End If
        Else 
            statoOrdine = enum_StatoOrdine.INDEFINITO
        End If

        Return statoOrdine

    End Function

    Public Shared Function GetStatoOrdineDettaglio_Cod(ByVal elemCod As Integer,
                                                       ByVal contabilizzato As Integer,
                                                       ByVal qtaRichiesta As Decimal,
                                                       ByVal qtaEvasa As Decimal
                                                       ) As enum_StatoOrdine

        Dim statoOrdineDettaglio As enum_StatoOrdine = enum_StatoOrdine.INDEFINITO

        If elemCod <> RIGA_DESCRIZIONE_LIBERA Then

            If contabilizzato = CONTABILE_EVASO_FORZATAMENTE Then 'Ho segnato la riga come forzatamente evasa
                statoOrdineDettaglio = enum_StatoOrdine.EVASO_FORZATAMENTE
            Else

                If qtaEvasa = 0D Then
                    statoOrdineDettaglio = enum_StatoOrdine.INEVASO
                ElseIf qtaEvasa < qtaRichiesta Then
                    statoOrdineDettaglio = enum_StatoOrdine.PARZIALMENTE_EVASO
                Else
                    'Esattamente/Iper Evaso
                    statoOrdineDettaglio = enum_StatoOrdine.EVASO
                End If

            End If

        End If

        Return statoOrdineDettaglio

    End Function

    Public Shared Function GetStatoOrdine_Des(ByVal statoOrdine As enum_StatoOrdine) As String

        Select Case statoOrdine
            Case enum_StatoOrdine.INEVASO
                Return "Non Evaso"
            Case enum_StatoOrdine.EVASO
                Return "Evaso"
            Case enum_StatoOrdine.PARZIALMENTE_EVASO
                Return "Evaso Parzialmente"
            Case enum_StatoOrdine.NON_PRONTO
                Return "Non Pronto"
            Case enum_StatoOrdine.EVASO_FORZATAMENTE
                Return "Evaso Forzatamente"
            Case Else
                Return ""
        End Select

    End Function

#End Region

End Class

'//////////////////////////////////////////////////////////////////////
'//////////////////////////////////////////////////////////////////////
'//////////////////////////////////////////////////////////////////////
' A T T E N Z I O N E 
'modificare questa classe solo in accordo con Marco Lucchi
'GiasLan e stampe devono funzionare alla stessa maniera
'//////////////////////////////////////////////////////////////////////
'//////////////////////////////////////////////////////////////////////
Public Class GiasLan_Round
    Inherits AgronicaCoreDataProvider.DataProvider

    Const DEFAULT_INDICE_RIGA_DUPLICATO As Integer = -1

    Const D_ID_MOV_DET As Integer = 0
    Const D_MODALITA_SCONTO As Integer = 1
    Const D_SCONTO As Integer = 2
    Const D_SCONTO_LISTINO As Integer = 3
    Const D_QTA As Integer = 4
    Const D_PREZZO As Integer = 5
    Const D_PREZZO_NETTO As Integer = 6
    Const D_IMPONIBILE_LORDO As Integer = 7
    Const D_IMPONIBILE_NETTO As Integer = 8
    Const D_COD_IVA As Integer = 9
    Const D_IVA As Integer = 10
    Const D_COD_ALIQUOTA_IVA As Integer = 11
    Const D_DES_IVA As Integer = 12
    Const D_COSTOCOMPLESSIVO As Integer = 13
    Const D_Iva_Indetraibile_Perc As Integer = 14
    Const D_Iva_Indetraibile As Integer = 15
    Const D_modalita_doc As Integer = 16

    Const I_IMPONIBILE_LORDO As Integer = 0
    Const I_IMPONIBILE_NETTO As Integer = 1
    Const I_COD_IVA As Integer = 2
    Const I_IVA As Integer = 3
    Const I_COD_ALIQUOTA_IVA As Integer = 4
    Const I_DES_IVA As Integer = 5
    Const I_CHK_IVA_MANUALE As Integer = 6
    Const I_PREZZO As Integer = 7
    Const I_IMPOSTA_OMAGGIO As Integer = 8
    Const I_iva_indet_perc As Integer = 9
    Const I_iva_indet As Integer = 10
    Const I_modalita_doc As Integer = 11

#Region "Funzioni pubbliche"

    '###################################################################
    'mTipo_Sconto viene passato dal data entry, in base al punto di partenza di immissione dei dati
    'nelle stampe non c'è, metto un default
    '-----
    'ritorna il dt del riepilogo iva
    'il lan è una sub, perché riempie direttamente la griglia
    Public Function FormAggiornaImporto(ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef DtDettagli As DataTable,
                                        ByRef Riepilogo_ImponibileLordo As Decimal,
                                        ByRef Riepilogo_Variazioni As Decimal,
                                        ByRef Riepilogo_ImponibileNetto As Decimal,
                                        ByRef Riepilogo_Imposta As Decimal,
                                        ByRef Riepilogo_Importo As Decimal,
                                        ByVal Edit_Importo As enum_EditImporto,
                                        Optional ByVal Flag_LiqIva As Boolean = False,
                                        Optional ByVal Flag_Vendita As Boolean = False
                                        ) As DataTable

        '26/03/2015: sostituito tipo_sconto con edit_importo
        ' Optional ByVal mTipo_Sconto As enum_TipoSconto = enum_TipoSconto.PrezzoUnitario,

        ' ByVal mTipo_Sconto As enum_TipoSconto,
        ' ByVal Flag_RegistriIva As Boolean,
        ' ByVal Flag_Vendita As Boolean
        'As DataTable
        'Optional ByVal mTipo_Sconto As enum_TipoSconto = enum_TipoSconto.PrezzoUnitario,
        'Optional ByVal Flag_RegistriIva As Boolean = False,
        'Optional ByVal Flag_0Vendite_1Acquisti As Integer = -1
        'As DataTable

        'ByRef DtIva As DataTable, _
        Dim nomeRoutine As String = "AgronicaCoreContabHLP.Contabilita.FormAggiornaImporto()"
        Dim messaggioErrore As String = ""
        Dim DtIva As DataTable

        Try

            If IsNothing(DtDettagli) Then
                Throw New Exception("DtDettagli nothing!")
            End If

            'modifica del 26/09/2014:
            ' il Flag_LiqIva viene passato a true per liquidazione iva e registri iva
            'in questi report va gestita l'iva indetraibile/iva in compensazione
            'negli altri casi (stampa di un documento) non è da fare

            ''If IsNothing(DtIva) Then
            ''    DtIva = CaricaGriglia_DtIva()
            ''End If
            'DtIva = CaricaGriglia_DtIva()

            If Flag_LiqIva = False Then
                DtIva = CaricaGriglia_DtIva()
            Else
                DtIva = CaricaGriglia_DtIva_LiqIVA()
            End If

            Dim i As Integer
            Dim Imponibile_Lordo As Decimal
            Dim Imponibile_Netto As Decimal
            Dim Imposta As Decimal
            Dim Importo As Decimal
            Dim Sconto As Decimal
            Dim IndiceRigaIVA As Integer
            Dim Imposta_Campione_Omaggio As Decimal
            Dim Importo_Campione_Omaggio As Decimal
            Dim Totale_Importo_Omaggi As Decimal
            Dim Totale_Imposta_Omaggi As Decimal


            'azzero ad ogni giro i riepiloghi, li conteggio ogni volta sui dettagli attuali
            Riepilogo_ImponibileLordo = Format2(0)
            Riepilogo_Variazioni = Format2(0)
            Riepilogo_ImponibileNetto = Format2(0)
            Riepilogo_Imposta = Format2(0)
            Riepilogo_Importo = Format2(0)
            'TxtProvvigione = Format2(0)
            'TxtProvvigione_Importo = Format2(0)

            'NOTA: LA QTA VA IN math.abs PER GESTIRE I RESI!!!
            For i = 0 To DtDettagli.Rows.Count - 1

                If IsNumeric(DtDettagli.Rows(i).Item(D_QTA)) AndAlso
                   IsNumeric(DtDettagli.Rows(i).Item(D_PREZZO)) Then

                    ' Select Case Edit_Importo  'mTipo_Sconto

                    ''Case enum_TipoSconto.PrezzoUnitario, enum_TipoSconto.Imponibile, enum_TipoSconto.Totale
                    'Case enum_EditImporto.PrezzoUnitario, enum_EditImporto.Imponibile, enum_EditImporto.Importo

                    Select Case CInt(DtDettagli.Rows(i).Item(D_MODALITA_SCONTO))

                        Case enModalitaSconto.Percentuale
                            'Sottraggo lo sconto cliente
                            DtDettagli.Rows(i).Item(D_PREZZO_NETTO) = Format(CDbl(DtDettagli.Rows(i).Item(D_PREZZO)) + (CDbl(DtDettagli.Rows(i).Item(D_PREZZO) / 100) * CDbl(DtDettagli.Rows(i).Item(D_SCONTO))), "##,###,###.00######")

                            'Sottraggo lo sconto listino
                            If CDbl(DtDettagli.Rows(i).Item(D_SCONTO_LISTINO)) < 0 Then
                                DtDettagli.Rows(i).Item(D_SCONTO_LISTINO) = -1 * CDbl(DtDettagli.Rows(i).Item(D_SCONTO_LISTINO))
                            End If
                            DtDettagli.Rows(i).Item(D_PREZZO_NETTO) = Format(CDbl(DtDettagli.Rows(i).Item(D_PREZZO_NETTO)) - (CDbl(DtDettagli.Rows(i).Item(D_PREZZO_NETTO) / 100) * CDbl(DtDettagli.Rows(i).Item(D_SCONTO_LISTINO))), "##,###,###.00######")

                            Imponibile_Lordo = CDec(DtDettagli.Rows(i).Item(D_PREZZO)) * Math.Abs(CDec(DtDettagli.Rows(i).Item(D_QTA)))
                            Imponibile_Netto = CDec(DtDettagli.Rows(i).Item(D_PREZZO_NETTO)) * Math.Abs(CDec(DtDettagli.Rows(i).Item(D_QTA)))

                            DtDettagli.Rows(i).Item(D_IMPONIBILE_LORDO) = FormatEsteso(Imponibile_Lordo)
                            DtDettagli.Rows(i).Item(D_IMPONIBILE_NETTO) = FormatEsteso(Imponibile_Netto)

                        Case enModalitaSconto.Omaggio_SenzaRivalsaIva, enModalitaSconto.Omaggio_ConRivalsaIva
                            'DtDettagli.Rows(i).Item(D_PREZZO_NETTO) = Format(CDbl(DtDettagli.Rows(i).Item(D_PREZZO)) + (CDbl(DtDettagli.Rows(i).Item(D_PREZZO) / 100) * CDbl(DtDettagli.Rows(i).Item(D_SCONTO))), "##,###,###.00######")
                            DtDettagli.Rows(i).Item(D_PREZZO_NETTO) = Format(CDbl(DtDettagli.Rows(i).Item(D_PREZZO)), "##,###,###.00######")
                            Imponibile_Lordo = CDec(DtDettagli.Rows(i).Item(D_PREZZO)) * Math.Abs(CDec(DtDettagli.Rows(i).Item(D_QTA)))
                            Imponibile_Netto = CDec(DtDettagli.Rows(i).Item(D_PREZZO)) * Math.Abs(CDec(DtDettagli.Rows(i).Item(D_QTA)))

                            DtDettagli.Rows(i).Item(D_IMPONIBILE_NETTO) = FormatEsteso(Imponibile_Netto)
                            DtDettagli.Rows(i).Item(D_IMPONIBILE_LORDO) = Format(Imponibile_Lordo)

                        Case enModalitaSconto.Sconto_Merce, enModalitaSconto.Campioni_Gratuiti
                            DtDettagli.Rows(i).Item(D_PREZZO_NETTO) = Format(0, "##,###,###.00######")

                            Imponibile_Lordo = 0
                            Imponibile_Netto = 0

                            DtDettagli.Rows(i).Item(D_IMPONIBILE_LORDO) = FormatEsteso(Imponibile_Lordo)
                            DtDettagli.Rows(i).Item(D_IMPONIBILE_NETTO) = FormatEsteso(Imponibile_Netto)

                            'Case enModalitaSconto.Omaggio_ConRivalsaIva

                            '    DtDettagli.Rows(i).Item(D_PREZZO_NETTO) = Format(CDbl(DtDettagli.Rows(i).Item(D_PREZZO)), "##,###,###.00######")
                            '    Imponibile_Lordo = CDbl(DtDettagli.Rows(i).Item(D_PREZZO)) * Math.Abs(DtDettagli.Rows(i).Item(D_QTA))

                            '    Imponibile_Netto = CDbl(DtDettagli.Rows(i).Item(D_PREZZO)) * Math.Abs(DtDettagli.Rows(i).Item(D_QTA))

                            '    DtDettagli.Rows(i).Item(D_IMPONIBILE_NETTO) = FormatEsteso(Imponibile_Netto)
                            '    DtDettagli.Rows(i).Item(D_IMPONIBILE_LORDO) = Format(Imponibile_Lordo)
                    End Select

                    DtDettagli.Rows(i).Item(D_PREZZO) = Format(DtDettagli.Rows(i).Item(D_PREZZO), "##,###,###.00######")
                    Sconto = Imponibile_Lordo - Imponibile_Netto

                    '    Case Else
                    'Throw New Exception("Case enum_EditImporto non gestito")
                    'End Select

                    If Flag_LiqIva = False Then
                        'Cerco se l'aliquota Iva è già presente in griglia
                        IndiceRigaIVA = ChkDuplicatiGriglia(DtIva, CLng(DtDettagli.Rows(i).Item(D_COD_IVA)), I_COD_IVA)

                        If IndiceRigaIVA = DEFAULT_INDICE_RIGA_DUPLICATO Then

                            'l'indice è -1, non è stato trovato nulla
                            'inserisco la riga e mi salvo in IndiceRigaIVA l'indice del record che vado ad inserire

                            InserisciRiga_DtIva(DtIva,
                                                Format2(0),
                                                Format2(0),
                                                CInt(DtDettagli.Rows(i).Item(D_COD_IVA)),
                                                Format2(0),
                                                CDec(DtDettagli.Rows(i).Item(D_COD_ALIQUOTA_IVA)),
                                                CStr(DtDettagli.Rows(i).Item(D_DES_IVA)),
                                                0,
                                                Format2(0),
                                                Format2(0),
                                                Format2(0))

                            IndiceRigaIVA = DtIva.Rows.Count - 1
                        End If
                    Else

                        'Cerco se l'aliquota Iva è già presente in griglia
                        IndiceRigaIVA = ChkDuplicatiGriglia2(DtIva,
                                                             CInt(DtDettagli.Rows(i).Item(D_COD_IVA)), I_COD_IVA,
                                                             CDec(DtDettagli.Rows(i).Item(D_Iva_Indetraibile_Perc)), I_iva_indet_perc,
                                                             CInt(DtDettagli.Rows(i).Item(D_modalita_doc)), I_modalita_doc)

                        If IndiceRigaIVA = DEFAULT_INDICE_RIGA_DUPLICATO Then

                            'l'indice è -1, non è stato trovato nulla
                            'inserisco la riga e mi salvo in IndiceRigaIVA l'indice del record che vado ad inserire

                            InserisciRiga_DtIva_LiqIva(DtIva,
                                                       Format2(0),
                                                       Format2(0),
                                                       CInt(DtDettagli.Rows(i).Item(D_COD_IVA)),
                                                       Format2(0),
                                                       CDec(DtDettagli.Rows(i).Item(D_COD_ALIQUOTA_IVA)),
                                                       CStr(DtDettagli.Rows(i).Item(D_DES_IVA)),
                                                       0,
                                                       Format2(0),
                                                       Format2(0),
                                                       CDec(DtDettagli.Rows(i).Item(D_Iva_Indetraibile_Perc)),
                                                       CDec(DtDettagli.Rows(i).Item(D_Iva_Indetraibile)),
                                                       CInt(DtDettagli.Rows(i).Item(D_modalita_doc)),
                                                       0)

                            IndiceRigaIVA = DtIva.Rows.Count - 1
                        End If

                    End If

                    Sconto = Format2(Sconto)

                    Imposta = (Format2(Imponibile_Netto) / 100) * CDec(DtIva.Rows(IndiceRigaIVA).Item("aliquota_iva"))

                    'modifica del 23/09/2014
                    'era stato fatto a novembre 2013 (in fretta e furia per la demo del giorno dopo)
                    'non va bene, nel registro iva l'iva deve essere stampata come in fattura, non deve essere decrementata
                    'nel riepilogo si avrà una voce in più per vedere il riepilogo dell'iva indetraibile
                    'Imposta = Gestione_Iva_IndetraibileCompensazione(Flag_RegistriIva,
                    '                                                Flag_Vendita,
                    '                                                Imposta,
                    '                                                DtDettagli.Rows(i))

                    Select Case CInt(DtDettagli.Rows(i).Item(D_MODALITA_SCONTO))

                        Case enModalitaSconto.Percentuale
                            DtIva.Rows(IndiceRigaIVA).Item(I_IMPONIBILE_NETTO) = CDec(DtIva.Rows(IndiceRigaIVA).Item(I_IMPONIBILE_NETTO)) + Imponibile_Netto
                            DtIva.Rows(IndiceRigaIVA).Item(I_IMPONIBILE_LORDO) = CDec(DtIva.Rows(IndiceRigaIVA).Item(I_IMPONIBILE_LORDO)) + Imponibile_Lordo

                            DtIva.Rows(IndiceRigaIVA).Item(I_IVA) = CDec(DtIva.Rows(IndiceRigaIVA).Item(I_IVA)) + Imposta

                            '=======================================================================================================
                            'Importo dettaglio
                            '-------------------------------------------------------------------------------------------------------
                            FormImpostaArrotondamenti(Imponibile_Netto, Imponibile_Lordo, Imposta, Edit_Importo)

                            Importo = Format2(Imponibile_Netto + Imposta)
                            Riepilogo_Variazioni = Riepilogo_Variazioni - Sconto
                            '=======================================================================================================

                        Case enModalitaSconto.Omaggio_SenzaRivalsaIva

                            DtIva.Rows(IndiceRigaIVA).Item(I_IMPONIBILE_NETTO) = CDec(DtIva.Rows(IndiceRigaIVA).Item(I_IMPONIBILE_NETTO)) + Imponibile_Netto
                            DtIva.Rows(IndiceRigaIVA).Item(I_IMPONIBILE_LORDO) = CDec(DtIva.Rows(IndiceRigaIVA).Item(I_IMPONIBILE_LORDO)) + Imponibile_Lordo

                            DtIva.Rows(IndiceRigaIVA).Item(I_IVA) = CDec(DtIva.Rows(IndiceRigaIVA).Item(I_IVA)) + Imposta

                            'Indico che l'imponibile non verrà considerato nel calcolo importo
                            DtIva.Rows(IndiceRigaIVA).Item(I_PREZZO) = CDec(DtIva.Rows(IndiceRigaIVA).Item(I_PREZZO)) + Imponibile_Netto

                            Importo = 0

                        Case enModalitaSconto.Omaggio_ConRivalsaIva

                            DtIva.Rows(IndiceRigaIVA).Item(I_IMPONIBILE_NETTO) = CDec(DtIva.Rows(IndiceRigaIVA).Item(I_IMPONIBILE_NETTO)) + Imponibile_Netto
                            DtIva.Rows(IndiceRigaIVA).Item(I_IMPONIBILE_LORDO) = CDec(DtIva.Rows(IndiceRigaIVA).Item(I_IMPONIBILE_LORDO)) + Imponibile_Lordo

                            DtIva.Rows(IndiceRigaIVA).Item(I_IVA) = CDec(DtIva.Rows(IndiceRigaIVA).Item(I_IVA)) + Imposta

                            DtIva.Rows(IndiceRigaIVA).Item(I_PREZZO) = 0

                            Importo = Imposta

                        Case Else
                            DtIva.Rows(IndiceRigaIVA).Item(I_IMPONIBILE_NETTO) = CDec(DtIva.Rows(IndiceRigaIVA).Item(I_IMPONIBILE_NETTO)) + Imponibile_Netto
                            DtIva.Rows(IndiceRigaIVA).Item(I_IMPONIBILE_LORDO) = CDec(DtIva.Rows(IndiceRigaIVA).Item(I_IMPONIBILE_LORDO)) + Imponibile_Lordo
                            Importo = 0
                    End Select

                    DtDettagli.Rows(i).Item(D_COSTOCOMPLESSIVO) = Format2(Importo)

                    '            '=================================================================================================================================
                    '            'Impostazione Provvigione
                    '            '---------------------------------------------------------------------------------------------------------------------------------
                    '            If Not IsNumeric(DtDettagli.rows(i).item( COL_PROVVIGIONE)) Then
                    '
                    '               DtDettagli.rows(i).item( COL_PROVVIGIONE) = 0
                    '
                    '            End If
                    '
                    '            TxtProvvigione_Importo = Format2(TxtProvvigione_Importo.Text + CDbl(DtDettagli.rows(i).item( COL_PROVVIGIONE)))
                    '
                    '            '=================================================================================================================================

                End If

            Next 'dettagli

            Importo = 0


            For i = 0 To DtIva.Rows.Count - 1

                Imponibile_Netto = CDec(DtIva.Rows(i).Item(I_IMPONIBILE_NETTO))
                Imponibile_Lordo = CDec(DtIva.Rows(i).Item(I_IMPONIBILE_LORDO))
                Imposta = CDec(DtIva.Rows(i).Item(I_IVA))

                'Select Case CInt(DtDettagli.Rows(i).Item(D_MODALITA_SCONTO))
                '    Case enModalitaSconto.Omaggio_SenzaRivalsaIva
                '        'Detraggo dall'imposta l'eventuale campione omaggio
                '        Imposta_Campione_Omaggio = Imposta_Campione_Omaggio + Format2((CDbl(DtIva.Rows(i).Item(I_PREZZO) / 100) * DtIva.Rows(i).Item(I_COD_ALIQUOTA_IVA)))
                '        'Detraggo Importo
                '        Importo_Campione_Omaggio = Importo_Campione_Omaggio + Imponibile_Netto
                'End Select

                'imposta campione omaggio da detrarre
                ' Imposta_Campione_Omaggio = Imposta_Campione_Omaggio + (CDbl(DtIva.Rows(i).Item(I_PREZZO) / 100) * DtIva.Rows(i).Item(I_COD_ALIQUOTA_IVA))
                Imposta_Campione_Omaggio = (CDbl(DtIva.Rows(i).Item(I_PREZZO) / 100) * DtIva.Rows(i).Item(I_COD_ALIQUOTA_IVA))

                'Importo da detrarre
                ' Importo_Campione_Omaggio = Importo_Campione_Omaggio + DtIva.Rows(i).Item(I_PREZZO)
                Importo_Campione_Omaggio = CDec(DtIva.Rows(i).Item(I_PREZZO))

                'Impostazioni arrotondamenti per aliquota
                FormImpostaArrotondamenti(Imponibile_Netto, Imponibile_Lordo, Imposta, Edit_Importo)

                FormImpostaArrotondamenti(Importo_Campione_Omaggio, 0, Imposta_Campione_Omaggio, Edit_Importo)

                Totale_Importo_Omaggi += Importo_Campione_Omaggio
                Totale_Imposta_Omaggi += Imposta_Campione_Omaggio

                'Correzione Arrotondamenti
                DtIva.Rows(i).Item(I_IMPONIBILE_NETTO) = Format2(Imponibile_Netto)
                DtIva.Rows(i).Item(I_IMPONIBILE_LORDO) = Format2(Imponibile_Lordo)
                DtIva.Rows(i).Item(I_IVA) = Format2(Imposta)

                If Flag_LiqIva = True Then
                    '  Giulia, 14/11/2016 15.15.17: La compensazione va calcolata sull'imponibile netto, l'indetraibilità sull'imposta
                    'DtIva.Rows(i).Item(I_iva_indet) = Format2(CDbl(Format2(Imposta)) * DtIva.Rows(i).Item(I_iva_indet_perc) / 100)
                    If Flag_Vendita = True Then
                        DtIva.Rows(i).Item(I_iva_indet) = Format2(CDbl(Format2(Imponibile_Netto)) * CDec(DtIva.Rows(i).Item(I_iva_indet_perc)) / 100)
                    Else
                        DtIva.Rows(i).Item(I_iva_indet) = Format2(CDbl(Format2(Imposta)) * CDec(DtIva.Rows(i).Item(I_iva_indet_perc)) / 100)
                    End If
                End If

                'Aggiornamento Imposta
                Riepilogo_Imposta += CDec(DtIva.Rows(i).Item(I_IVA))

                'Aggiornamento Imponibili
                Riepilogo_ImponibileLordo += CDec(DtIva.Rows(i).Item(I_IMPONIBILE_LORDO))
                Riepilogo_ImponibileNetto += CDec(DtIva.Rows(i).Item(I_IMPONIBILE_NETTO))

            Next 'iva

            'Formattazione a 2 decimali
            For i = 0 To DtDettagli.Rows.Count - 1

                DtDettagli.Rows(i).Item(D_IMPONIBILE_LORDO) = Format2(DtDettagli.Rows(i).Item(D_IMPONIBILE_LORDO))
                DtDettagli.Rows(i).Item(D_IMPONIBILE_NETTO) = Format2(DtDettagli.Rows(i).Item(D_IMPONIBILE_NETTO))

                'Sistemazioni finali rivalsa iva
                If CInt(DtDettagli.Rows(i).Item(D_MODALITA_SCONTO)) = enModalitaSconto.Omaggio_ConRivalsaIva Then
                    'Scalo l'importo totale in caso di rivalsa iva
                    Riepilogo_Importo = Riepilogo_Importo - CDec(DtDettagli.Rows(i).Item(D_IMPONIBILE_NETTO))
                End If

            Next

            'Formattazione a 2 decimali
            Riepilogo_ImponibileLordo = Format2(Riepilogo_ImponibileLordo)
            Riepilogo_Variazioni = Format2(Riepilogo_Variazioni)
            Riepilogo_ImponibileNetto = Format2(Riepilogo_ImponibileNetto)
            Riepilogo_Imposta = Format2(Riepilogo_Imposta)

            '  TxtImporto.Text = Format2(CDbl(TxtImporto.Text) + CDbl(TxtImponibileNetto) + CDbl(TxtImposta) - Totale_Importo_Omaggi - Totale_Imposta_Omaggi)
            Riepilogo_Importo = Format2(Riepilogo_Importo + Riepilogo_ImponibileNetto + Riepilogo_Imposta - Totale_Importo_Omaggi - Totale_Imposta_Omaggi)

            ''Impostazione Provvigione
            ''    If CDbl(Riepilogo_ImponibileNetto) > 0 Then
            ''       TxtProvvigione.Text = Format2((TxtProvvigione_Importo * 100) / Riepilogo_ImponibileNetto)
            ''    Else
            ''       TxtProvvigione.Text = Format2(0)
            ''    End If
            'CmdPagamenti.enabled = IIf(CDbl(TxtImporto.Text) > 0, True, False)
            ''    ColoraCelleIva
            ''Ordinamento Griglia Iva
            ''    FormOrdinamentoIva
            ''Aggiornamento Provvigione Agente
            ''    FormImpostaProvvigioneImporto


        Catch ex As Exception
            DtIva = Nothing
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return DtIva

    End Function

    Public Function FormAggiornaImportoNEW(ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                           ByRef DtDettagli As DataTable,
                                           ByRef Riepilogo_ImponibileLordo As Decimal,
                                           ByRef Riepilogo_Variazioni As Decimal,
                                           ByRef Riepilogo_ImponibileNetto As Decimal,
                                           ByRef Riepilogo_Imposta As Decimal,
                                           ByRef Riepilogo_Importo As Decimal,
                                           ByVal Edit_Importo As enum_EditImporto,
                                           ByVal Flag_LiqIva As Boolean,
                                           ByVal Flag_Vendita As Boolean, _
                                           ByVal EsigibilitaIva As enum_EsigibilitaIva _
                                           ) As DataTable

        'ByVal Flag_LiqIva As Boolean = False,
        'ByVal Flag_Vendita As Boolean = False, _

        '26/03/2015: sostituito tipo_sconto con edit_importo
        ' Optional ByVal mTipo_Sconto As enum_TipoSconto = enum_TipoSconto.PrezzoUnitario,

        ' ByVal mTipo_Sconto As enum_TipoSconto,
        ' ByVal Flag_RegistriIva As Boolean,
        ' ByVal Flag_Vendita As Boolean
        'As DataTable
        'Optional ByVal mTipo_Sconto As enum_TipoSconto = enum_TipoSconto.PrezzoUnitario,
        'Optional ByVal Flag_RegistriIva As Boolean = False,
        'Optional ByVal Flag_0Vendite_1Acquisti As Integer = -1
        'As DataTable

        'ByRef DtIva As DataTable,
        Dim nomeRoutine As String = "AgronicaCoreContabHLP.Contabilita.FormAggiornaImportoNEW()"
        Dim messaggioErrore As String = ""
        Dim DtIva As DataTable

        Try

            If IsNothing(DtDettagli) Then
                Throw New Exception("DtDettagli nothing!")
            End If

            'modifica del 26/09/2014:
            ' il Flag_LiqIva viene passato a true per liquidazione iva e registri iva
            'in questi report va gestita l'iva indetraibile/iva in compensazione
            'negli altri casi (stampa di un documento) non è da fare

            ''If IsNothing(DtIva) Then
            ''    DtIva = CaricaGriglia_DtIva()
            ''End If
            'DtIva = CaricaGriglia_DtIva()

            If Flag_LiqIva = False Then
                DtIva = CaricaGriglia_DtIva()
            Else
                DtIva = CaricaGriglia_DtIva_LiqIVA()
            End If

            Dim i As Integer
            Dim Imponibile_Lordo As Decimal
            Dim Imponibile_Netto As Decimal
            Dim Imposta As Decimal
            Dim Iva_Indetraibile As Decimal
            Dim Importo As Decimal
            Dim Sconto As Decimal
            Dim IndiceRigaIVA As Integer
            Dim Imposta_Campione_Omaggio As Decimal
            Dim Importo_Campione_Omaggio As Decimal
            Dim Totale_Importo_Omaggi As Decimal
            Dim Totale_Imposta_Omaggi As Decimal
            Dim Iva_Split As Decimal


            'azzero ad ogni giro i riepiloghi, li conteggio ogni volta sui dettagli attuali
            Riepilogo_ImponibileLordo = 0
            Riepilogo_Variazioni = 0
            Riepilogo_ImponibileNetto = 0
            Riepilogo_Imposta = 0
            Riepilogo_Importo = 0
            'TxtProvvigione = Format2(0)
            'TxtProvvigione_Importo = Format2(0)

            'NOTA: LA QTA VA IN math.abs PER GESTIRE I RESI!!!
            For i = 0 To DtDettagli.Rows.Count - 1

                If IsNumeric(DtDettagli.Rows(i).Item("qta")) AndAlso
                   IsNumeric(DtDettagli.Rows(i).Item("prezzo_unitario")) Then

                    ' Select Case Edit_Importo  'mTipo_Sconto

                    ''Case enum_TipoSconto.PrezzoUnitario, enum_TipoSconto.Imponibile, enum_TipoSconto.Totale
                    'Case enum_EditImporto.PrezzoUnitario, enum_EditImporto.Imponibile, enum_EditImporto.Importo

                    Select Case CInt(DtDettagli.Rows(i).Item("modalita_sconto"))

                        Case enModalitaSconto.Percentuale

                            DtDettagli.Rows(i).Item("prezzo_unitario_netto") = Format6(ArrotondaVal_6(CDec(DtDettagli.Rows(i).Item("prezzo_unitario_netto"))))

                            Imponibile_Lordo = ArrotondaVal_2(CDec(DtDettagli.Rows(i).Item("imponibile")))
                            Imponibile_Netto = ArrotondaVal_2(CDec(DtDettagli.Rows(i).Item("imponibile_netto")))

                            DtDettagli.Rows(i).Item("imponibile") = Format2(Imponibile_Lordo)
                            DtDettagli.Rows(i).Item("imponibile_netto") = Format2(Imponibile_Netto)

                        Case enModalitaSconto.Omaggio_SenzaRivalsaIva, enModalitaSconto.Omaggio_ConRivalsaIva

                            DtDettagli.Rows(i).Item("prezzo_unitario_netto") = Format6(ArrotondaVal_6(CDec(DtDettagli.Rows(i).Item("prezzo_unitario"))))

                            Imponibile_Lordo = ArrotondaVal_2(CDec(DtDettagli.Rows(i).Item("imponibile")))
                            Imponibile_Netto = ArrotondaVal_2(CDec(DtDettagli.Rows(i).Item("imponibile")))

                            DtDettagli.Rows(i).Item("imponibile_netto") = Format2(Imponibile_Netto)
                            DtDettagli.Rows(i).Item("imponibile") = Format2(Imponibile_Lordo)

                        Case enModalitaSconto.Sconto_Merce, enModalitaSconto.Campioni_Gratuiti

                            DtDettagli.Rows(i).Item("prezzo_unitario_netto") = Format6(0)

                            Imponibile_Lordo = 0
                            Imponibile_Netto = 0

                            DtDettagli.Rows(i).Item("imponibile") = Format2(Imponibile_Lordo)
                            DtDettagli.Rows(i).Item("imponibile_netto") = Format2(Imponibile_Netto)

                            'Case enModalitaSconto.Omaggio_ConRivalsaIva

                            '    DtDettagli.Rows(i).Item("prezzo_unitario_netto") = Format(CDec(DtDettagli.Rows(i).Item("prezzo_unitario")), "##,###,###.00######")
                            '    Imponibile_Lordo = CDec(DtDettagli.Rows(i).Item("prezzo_unitario")) * Math.Abs(DtDettagli.Rows(i).Item("qta"))

                            '    Imponibile_Netto = CDec(DtDettagli.Rows(i).Item("prezzo_unitario")) * Math.Abs(DtDettagli.Rows(i).Item("qta"))

                            '    DtDettagli.Rows(i).Item("imponibile_netto") = FormatEsteso(Imponibile_Netto)
                            '    DtDettagli.Rows(i).Item("imponibile") = Format(Imponibile_Lordo)
                    End Select

                    DtDettagli.Rows(i).Item("prezzo_unitario") = Format6(ArrotondaVal_6(CDec(DtDettagli.Rows(i).Item("prezzo_unitario"))))

                    '    Case Else
                    'Throw New Exception("Case enum_EditImporto non gestito")
                    'End Select

                    If Flag_LiqIva = False Then
                        'Cerco se l'aliquota Iva è già presente in griglia
                        IndiceRigaIVA = ChkDuplicatiGriglia(DtIva, CLng(DtDettagli.Rows(i).Item("cod_iva")), I_COD_IVA)

                        If IndiceRigaIVA = DEFAULT_INDICE_RIGA_DUPLICATO Then

                            'l'indice è -1, non è stato trovato nulla
                            'inserisco la riga e mi salvo in IndiceRigaIVA l'indice del record che vado ad inserire

                            InserisciRiga_DtIva(DtIva,
                                                0,
                                                0,
                                                CInt(DtDettagli.Rows(i).Item("cod_iva")),
                                                0,
                                                CDec(DtDettagli.Rows(i).Item("aliquota_iva")),
                                                CStr(DtDettagli.Rows(i).Item("aliquota_des")),
                                                0,
                                                0,
                                                0,
                                                0)

                            IndiceRigaIVA = DtIva.Rows.Count - 1
                        End If
                    Else

                        'Cerco se l'aliquota Iva è già presente in griglia
                        IndiceRigaIVA = ChkDuplicatiGriglia2(DtIva,
                                                             CInt(DtDettagli.Rows(i).Item("cod_iva")), I_COD_IVA,
                                                             CDec(DtDettagli.Rows(i).Item("Iva_Indetraibile_Perc")), I_iva_indet_perc,
                                                             CInt(DtDettagli.Rows(i).Item("modalita_doc")), I_modalita_doc)

                        If IndiceRigaIVA = DEFAULT_INDICE_RIGA_DUPLICATO Then

                            'l'indice è -1, non è stato trovato nulla
                            'inserisco la riga e mi salvo in IndiceRigaIVA l'indice del record che vado ad inserire

                            InserisciRiga_DtIva_LiqIva(DtIva,
                                                       0,
                                                       0,
                                                       CInt(DtDettagli.Rows(i).Item("cod_iva")),
                                                       0,
                                                       CDec(DtDettagli.Rows(i).Item("aliquota_iva")),
                                                       CStr(DtDettagli.Rows(i).Item("aliquota_des")),
                                                       0,
                                                       0,
                                                       0,
                                                       CDec(DtDettagli.Rows(i).Item("Iva_Indetraibile_Perc")),
                                                       0,
                                                       CInt(DtDettagli.Rows(i).Item("modalita_doc")),
                                                       0)

                            IndiceRigaIVA = DtIva.Rows.Count - 1
                        End If

                    End If

                    Sconto = ArrotondaVal_2(Imponibile_Lordo - Imponibile_Netto)

                    'TODO: togli ricalcolo di imposta ==> DONE
                    'Imposta = (Imponibile_Netto / 100) * DtIva.Rows(IndiceRigaIVA).Item("aliquota_iva")
                    Imposta = CDec(DtDettagli.Rows(i).Item("iva"))
                    Iva_Indetraibile = CDec(DtDettagli.Rows(i).Item("Iva_Indetraibile"))

                    'modifica del 23/09/2014
                    'era stato fatto a novembre 2013 (in fretta e furia per la demo del giorno dopo)
                    'non va bene, nel registro iva l'iva deve essere stampata come in fattura, non deve essere decrementata
                    'nel riepilogo si avrà una voce in più per vedere il riepilogo dell'iva indetraibile
                    'Imposta = Gestione_Iva_IndetraibileCompensazione(Flag_RegistriIva,
                    '                                                Flag_Vendita,
                    '                                                Imposta,
                    '                                                DtDettagli.Rows(i))

                    DtIva.Rows(IndiceRigaIVA).Item("imponibile_netto") += Imponibile_Netto
                    DtIva.Rows(IndiceRigaIVA).Item("imponibile") += Imponibile_Lordo


                    '09/02/2018 by maga: aggiunta somma dell'iva indetraibile, 
                    'altrimenti nel caso di documento con più righe con la stessa iva e stessa % di indetraibile. veniva considerata solo l'iva_indet della prima riga
                    '(una volta c'era il calcolo dell'iva indetraibile alla fine, vedi commento più avanti)
                    Select Case CInt(DtDettagli.Rows(i).Item("modalita_sconto"))

                        Case enModalitaSconto.Percentuale

                            'Controllo Split Payment
                            If EsigibilitaIva = enum_EsigibilitaIva.Scissione_Pagamenti Then
                                Iva_Split += Imposta
                            End If

                            DtIva.Rows(IndiceRigaIVA).Item("iva") += Imposta
                            DtIva.Rows(IndiceRigaIVA).Item("iva_split") += Iva_Split

                            If Flag_LiqIva = True Then
                                DtIva.Rows(IndiceRigaIVA).Item("iva_indetraibile") += Iva_Indetraibile
                            End If

                            '=======================================================================================================
                            'Importo dettaglio
                            '-------------------------------------------------------------------------------------------------------
                            'FormImpostaArrotondamenti(Imponibile_Netto, Imponibile_Lordo, Imposta, Edit_Importo)

                            Importo = ArrotondaVal_2(Imponibile_Netto + Imposta)
                            Riepilogo_Variazioni = CDec(Riepilogo_Variazioni) - Sconto
                            '=======================================================================================================

                        Case enModalitaSconto.Omaggio_SenzaRivalsaIva

                            DtIva.Rows(IndiceRigaIVA).Item("iva") += Imposta
                            If Flag_LiqIva = True Then
                                DtIva.Rows(IndiceRigaIVA).Item("iva_indetraibile") += Iva_Indetraibile
                            End If

                            'Indico che l'imponibile non verrà considerato nel calcolo importo
                            DtIva.Rows(IndiceRigaIVA).Item("prezzo") += Imponibile_Netto

                            DtIva.Rows(IndiceRigaIVA).Item("imposta_omaggio") += Imposta

                            Importo = 0

                        Case enModalitaSconto.Omaggio_ConRivalsaIva

                            DtIva.Rows(IndiceRigaIVA).Item("iva") += Imposta
                            If Flag_LiqIva = True Then
                                DtIva.Rows(IndiceRigaIVA).Item("iva_indetraibile") += Iva_Indetraibile
                            End If

                            DtIva.Rows(IndiceRigaIVA).Item("prezzo") += 0D

                            Importo = Imposta

                        Case Else

                            Importo = 0

                    End Select

                    DtDettagli.Rows(i).Item("importo_totale") = Format2(ArrotondaVal_2(Importo))

                    '            '=================================================================================================================================
                    '            'Impostazione Provvigione
                    '            '---------------------------------------------------------------------------------------------------------------------------------
                    '            If Not IsNumeric(DtDettagli.rows(i).item( COL_PROVVIGIONE)) Then
                    '
                    '               DtDettagli.rows(i).item( COL_PROVVIGIONE) = 0
                    '
                    '            End If
                    '
                    '            TxtProvvigione_Importo = Format2(TxtProvvigione_Importo.Text + CDbl(DtDettagli.rows(i).item( COL_PROVVIGIONE)))
                    '
                    '            '=================================================================================================================================

                End If

            Next 'dettagli

            Importo = 0


            For Each drIva As DataRow In DtIva.Rows
                'For i = 0 To DtIva.Rows.Count - 1

                Imponibile_Netto = CDec(drIva.Item("imponibile_netto"))
                Imponibile_Lordo = CDec(drIva.Item("imponibile"))
                Imposta = CDec(drIva.Item("iva"))

                '11/02/2019: aggiunto arrotondamento a 2 sull'iva_indetraibile
                If Flag_LiqIva = True Then
                    Iva_Indetraibile = CDec(drIva.Item("iva_indetraibile"))
                End If

                'Select Case CInt(DtDettagli.Rows(i).Item("modalita_sconto"))
                '    Case enModalitaSconto.Omaggio_SenzaRivalsaIva
                '        'Detraggo dall'imposta l'eventuale campione omaggio
                '        Imposta_Campione_Omaggio = Imposta_Campione_Omaggio + Format2((CDec(drIva.Item("prezzo") / 100) * drIva.Item("aliquota_iva")))
                '        'Detraggo Importo
                '        Importo_Campione_Omaggio = Importo_Campione_Omaggio + Imponibile_Netto
                'End Select

                'imposta campione omaggio da detrarre
                'TODO: elimina calcolo ==> DONE
                ' Imposta_Campione_Omaggio = Imposta_Campione_Omaggio + (CDec(drIva.Item("prezzo") / 100) * drIva.Item("aliquota_iva"))
                'Imposta_Campione_Omaggio = ArrotondaVal_2(CDec(drIva.Item("prezzo") / 100) * CDec(drIva.Item("aliquota_iva")))
                Imposta_Campione_Omaggio = ArrotondaVal_2(CDec(drIva.Item("imposta_omaggio")))

                'Importo da detrarre
                ' Importo_Campione_Omaggio = Importo_Campione_Omaggio + DtIva.Rows(i).Item("prezzo")
                Importo_Campione_Omaggio = ArrotondaVal_2(CDec(drIva.Item("prezzo")))

                'Impostazioni arrotondamenti per aliquota
                'FormImpostaArrotondamenti(Imponibile_Netto, Imponibile_Lordo, Imposta, Edit_Importo)

                'FormImpostaArrotondamenti(Importo_Campione_Omaggio, 0, Imposta_Campione_Omaggio, Edit_Importo)

                Totale_Importo_Omaggi += Importo_Campione_Omaggio
                Totale_Imposta_Omaggi += Imposta_Campione_Omaggio

                'Correzione Arrotondamenti
                drIva.Item("imponibile_netto") = Format2(Imponibile_Netto)
                drIva.Item("imponibile") = Format2(Imponibile_Lordo)
                drIva.Item("iva") = Format2(ArrotondaVal_2(Imposta))

                '11/02/2019: aggiunto arrotondamento a 2 sull'iva_indetraibile
                If Flag_LiqIva = True Then
                    drIva.Item("iva_indetraibile") = Format2(ArrotondaVal_2(Iva_Indetraibile))
                End If

                'MAGA 24/01/2018: eliminato calcolo, legge dato salvato da GiasLan
                'visto che il dt_iva è già stato valorizzato dai dettagli
                'non occorre fare nulla, iva indetraibile già calcolata sopra come sommatoria
                'If Flag_LiqIva = True Then
                '    '  Giulia, 14/11/2016 15.15.17: La compensazione va calcolata sull'imponibile netto, l'indetraibilità sull'imposta
                '    'drIva.Item(I_iva_indet) = Format2(CDbl(Format2(Imposta)) * drIva.Item(I_iva_indet_perc) / 100)
                '    If Flag_Vendita = True Then
                '        drIva.Item(I_iva_indet) = Format2(CDbl(Format2(Imponibile_Netto)) * drIva.Item(I_iva_indet_perc) / 100)
                '    Else
                '        drIva.Item(I_iva_indet) = Format2(CDbl(Format2(Imposta)) * drIva.Item(I_iva_indet_perc) / 100)
                '    End If
                'End If

                'Aggiornamento Imposta
                Riepilogo_Imposta += CDec(drIva.Item("iva"))

                'Aggiornamento Imponibili
                Riepilogo_ImponibileLordo += CDec(drIva.Item("imponibile"))
                Riepilogo_ImponibileNetto += CDec(drIva.Item("imponibile_netto"))

            Next 'iva

            'Sistemazioni finali rivalsa iva
            For i = 0 To DtDettagli.Rows.Count - 1

                If CInt(DtDettagli.Rows(i).Item("modalita_sconto")) = enModalitaSconto.Omaggio_ConRivalsaIva Then
                    'Scalo l'importo totale in caso di rivalsa iva
                    Riepilogo_Importo = Riepilogo_Importo - CDec(DtDettagli.Rows(i).Item("imponibile_netto"))
                End If

            Next

            'Formattazione a 2 decimali
            Riepilogo_ImponibileLordo = Format2(Riepilogo_ImponibileLordo)
            Riepilogo_Variazioni = Format2(Riepilogo_Variazioni)
            Riepilogo_ImponibileNetto = Format2(Riepilogo_ImponibileNetto)
            Riepilogo_Imposta = Format2(Riepilogo_Imposta)

            Riepilogo_Importo = Format2(Riepilogo_Importo + CDec(Riepilogo_ImponibileNetto) + CDec(Riepilogo_Imposta) - Totale_Importo_Omaggi - Totale_Imposta_Omaggi - Iva_Split)

        Catch ex As Exception
            DtIva = Nothing
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return DtIva

    End Function


    ''###################################################################
    'Private Function Gestione_Iva_IndetraibileCompensazione(ByVal Flag_RegistriIva As Boolean,
    '                                                        ByVal Flag_Vendita As Boolean,
    '                                                        ByVal Imposta As Decimal,
    '                                                        ByVal Dr_Dettagli As DataRow) As Decimal

    '    Dim NuovaImposta As Decimal

    '    NuovaImposta = Imposta

    '    'SE SONO IN STAMPA REGISTRI IVA, DEVO GESTIRE L'IVA INDETRAIBILE / IVA IN COMPENSAZIONE
    '    If Flag_RegistriIva = True Then

    '        Dim Iva_Indetraibile_Perc As Decimal
    '        Dim Iva_Indetraibile As Decimal

    '        Iva_Indetraibile_Perc = Dr_Dettagli.Item("Iva_Indetraibile_Perc")
    '        Iva_Indetraibile = Dr_Dettagli.Item("Iva_Indetraibile")

    '        Select Case Flag_Vendita

    '            Case True
    '                'VENDITE
    '                'iva in compensazione
    '                If Iva_Indetraibile_Perc <> 0 Then
    '                    'imposta segno +
    '                    'Iva_Indetraibile segno +
    '                    NuovaImposta = Imposta - Iva_Indetraibile
    '                End If
    '            Case False
    '                'ACQUISTI
    '                'iva indetraibile
    '                If Iva_Indetraibile_Perc <> 0 Then
    '                    'imposta segno +
    '                    'Iva_Indetraibile segno -
    '                    NuovaImposta = Imposta + Iva_Indetraibile
    '                End If
    '                'Case Else
    '                '    NuovaImposta = Imposta
    '                '    Throw New Exception("Flag_0Vendite_1Acquisti: valore non valido")
    '        End Select
    '        'Else
    '        '    'caso standard, ad esempio stampa di una fattura
    '        '    'lascio l'imposta già calcolata
    '        '    NuovaImposta = Imposta
    '    End If

    '    Return NuovaImposta


    'End Function


    '###################################################################
    Public Shared Function Format2(ByVal importo) As String
        Return Format(importo, "##,###,###,###.00")
    End Function

    Public Shared Function Format6(ByVal importo) As String
        Return Format(importo, "##,###,###.00####")
    End Function

    '###################################################################
    Public Shared Function FormatEsteso(ByVal importo) As String
        Return Format(importo, "##,###,###,###.0000000")
    End Function

    '########################################################################################
    'se si aggiungono colonne occorre aggiornare le costanti con gli index!!!!!!!!!!!!!!!!!!
    Public Function CaricaGriglia_DtIva() As DataTable

        Dim dtIva As New DataTable

        dtIva.Columns.Add(New DataColumn("imponibile", GetType(Decimal)))
        dtIva.Columns.Add(New DataColumn("imponibile_netto", GetType(Decimal)))
        dtIva.Columns.Add(New DataColumn("cod_iva", GetType(Integer)))
        dtIva.Columns.Add(New DataColumn("iva", GetType(Decimal)))
        dtIva.Columns.Add(New DataColumn("aliquota_iva", GetType(Integer)))
        dtIva.Columns.Add(New DataColumn("aliquota_des", GetType(String)))
        dtIva.Columns.Add(New DataColumn("chkiva_manuale", GetType(Integer)))
        dtIva.Columns.Add(New DataColumn("prezzo", GetType(Decimal))) 'Imponibile Campioni Omaggio da Detrarre
        dtIva.Columns.Add(New DataColumn("imposta_omaggio", GetType(Decimal))) 'Imposta Campioni Omaggio da Detrarre
        dtIva.Columns.Add(New DataColumn("iva_split", GetType(Decimal))) 'enum_EsigibilitaIva.Scissione_Pagamenti

        Return dtIva

    End Function

    '########################################################################################
    'se si aggiungono colonne occorre aggiornare le costanti con gli index!!!!!!!!!!!!!!!!!!
    Public Function CaricaGriglia_DtDettagli() As DataTable

        Dim dt As New DataTable

        dt.Columns.Add(New DataColumn("Id_Mov_Det", GetType(Integer)))
        dt.Columns.Add(New DataColumn("modalita_sconto", GetType(Integer)))
        dt.Columns.Add(New DataColumn("sconto", GetType(Decimal)))
        dt.Columns.Add(New DataColumn("sconto_listino", GetType(Decimal)))
        dt.Columns.Add(New DataColumn("qta", GetType(Decimal)))
        dt.Columns.Add(New DataColumn("prezzo_unitario", GetType(Decimal)))
        dt.Columns.Add(New DataColumn("prezzo_unitario_netto", GetType(Decimal)))
        dt.Columns.Add(New DataColumn("imponibile", GetType(Decimal)))
        dt.Columns.Add(New DataColumn("imponibile_netto", GetType(Decimal)))
        dt.Columns.Add(New DataColumn("cod_iva", GetType(Integer)))
        dt.Columns.Add(New DataColumn("iva", GetType(Decimal)))
        dt.Columns.Add(New DataColumn("aliquota_iva", GetType(Decimal)))
        dt.Columns.Add(New DataColumn("aliquota_des", GetType(String)))
        dt.Columns.Add(New DataColumn("importo_totale", GetType(Decimal)))
        dt.Columns.Add(New DataColumn("Iva_Indetraibile_Perc", GetType(Decimal)))
        dt.Columns.Add(New DataColumn("Iva_Indetraibile", GetType(Decimal)))
        dt.Columns.Add(New DataColumn("modalita_doc", GetType(Integer))) 'tipologia di fattura, ad esempio fattura x acquisti intra

        Return dt

    End Function

    '########################################################################################
    'se si aggiungono colonne occorre aggiornare le costanti con gli index!!!!!!!!!!!!!!!!!!
    Public Function OLD_CaricaGriglia_DtIvaIndetCompens() As DataTable

        Dim dtIvaIndetComp As New DataTable

        dtIvaIndetComp.Columns.Add(New DataColumn("id_agenda", GetType(Integer)))
        dtIvaIndetComp.Columns.Add(New DataColumn("imponibile", GetType(Decimal)))
        dtIvaIndetComp.Columns.Add(New DataColumn("imponibile_netto", GetType(Decimal)))
        dtIvaIndetComp.Columns.Add(New DataColumn("cod_iva", GetType(Integer)))
        dtIvaIndetComp.Columns.Add(New DataColumn("iva", GetType(Decimal)))
        dtIvaIndetComp.Columns.Add(New DataColumn("Iva_Indetraibile", GetType(Decimal)))
        dtIvaIndetComp.Columns.Add(New DataColumn("Iva_Indetraibile_Perc", GetType(Decimal)))
        dtIvaIndetComp.Columns.Add(New DataColumn("aliquota_iva", GetType(Integer)))
        dtIvaIndetComp.Columns.Add(New DataColumn("aliquota_des", GetType(String)))

        Return dtIvaIndetComp

    End Function

    '########################################################################################
    'se si aggiungono colonne occorre aggiornare le costanti con gli index!!!!!!!!!!!!!!!!!!
    Public Function CaricaGriglia_DtIva_LiqIVA() As DataTable

        Dim dtIva As New DataTable

        dtIva.Columns.Add(New DataColumn("imponibile", GetType(Decimal)))
        dtIva.Columns.Add(New DataColumn("imponibile_netto", GetType(Decimal)))
        dtIva.Columns.Add(New DataColumn("cod_iva", GetType(Integer)))
        dtIva.Columns.Add(New DataColumn("iva", GetType(Decimal)))
        dtIva.Columns.Add(New DataColumn("aliquota_iva", GetType(Integer)))
        dtIva.Columns.Add(New DataColumn("aliquota_des", GetType(String)))
        dtIva.Columns.Add(New DataColumn("chkiva_manuale", GetType(Integer)))
        dtIva.Columns.Add(New DataColumn("prezzo", GetType(Decimal))) 'Imponibile Campioni Omaggio da Detrarre
        dtIva.Columns.Add(New DataColumn("imposta_omaggio", GetType(Decimal))) 'Imposta Campioni Omaggio da Detrarre
        dtIva.Columns.Add(New DataColumn("iva_indetraibile_perc", GetType(Decimal)))
        dtIva.Columns.Add(New DataColumn("Iva_Indetraibile", GetType(Decimal)))
        dtIva.Columns.Add(New DataColumn("modalita_doc", GetType(Integer))) 'tipologia di fattura, ad esempio fattura x acquisti intra
        dtIva.Columns.Add(New DataColumn("iva_split", GetType(Decimal))) 'enum_EsigibilitaIva.Scissione_Pagamenti

        Return dtIva

    End Function

    '########################################################################################
    Public Sub OLD_InserisciRiga_DtIvaIndetCompens(ByRef DtIvaIndetcomp As DataTable,
                                                   ByVal id_agenda As Integer,
                                                   ByVal imponibile As Decimal,
                                                   ByVal imponibile_netto As Decimal,
                                                   ByVal cod_iva As Integer,
                                                   ByVal iva As Decimal,
                                                   ByVal aliquota_iva As Decimal,
                                                   ByVal aliquota_des As String,
                                                   ByVal Iva_Indetraibile As Decimal,
                                                   ByVal Iva_Indetraibile_Perc As Decimal)

        Dim dr As DataRow

        'Creo una nuova riga
        dr = DtIvaIndetcomp.NewRow

        dr.Item("id_agenda") = id_agenda
        dr.Item("imponibile") = imponibile
        dr.Item("imponibile_netto") = imponibile_netto
        dr.Item("cod_iva") = cod_iva
        dr.Item("iva") = iva
        dr.Item("aliquota_iva") = aliquota_iva
        dr.Item("aliquota_des") = aliquota_des
        dr.Item("Iva_Indetraibile") = Iva_Indetraibile
        dr.Item("Iva_Indetraibile_Perc") = Iva_Indetraibile_Perc

        'Associo alla tabella la nuova riga creata
        DtIvaIndetcomp.Rows.Add(dr)

    End Sub

    '########################################################################################
    <Obsolete("Da NON Usare")>
    Public Function OLD_Elabora_NoteIva_IndetComp(ByVal DtIvaIndetcomp As DataTable,
                                                  ByVal id_agenda As Integer,
                                                  ByVal lav_cod As Integer,
                                                  ByVal cod_iva As Integer
                                                  ) As String

        Dim noteIva As String = ""

        If Not IsNothing(DtIvaIndetcomp) AndAlso DtIvaIndetcomp.Rows.Count > 0 Then

            Dim dr() As DataRow
            Dim ht As New Hashtable
            Dim i As Integer
            Dim chiaveHT As String
            'Dim imp, imp_tot As Decimal
            Dim iva, ivaTot As Decimal
            Dim ivaIndetraibilePerc As Decimal
            Dim objcontab As New AgronicaCoreContabHLP.Contabilita

            'Dr = DtIvaIndetcomp.Select(" Id_agenda = " & Agro_SQL_SaveNum(id_agenda) & " " &
            '                           " AND Iva_Indetraibile_Perc <> 0 " &
            '                           " AND cod_iva = " & Agro_SQL_SaveNum(cod_iva) & " ")

            dr = DtIvaIndetcomp.Select(" Id_agenda = " & Agro_SQL_SaveNum(id_agenda, False) & " " &
                                       " AND cod_iva = " & Agro_SQL_SaveNum(cod_iva, False) & " ")

            If Not IsNothing(dr) AndAlso dr.Length > 0 Then

                Select Case dr.Length

                    Case 1
                        'c'è solo un dettaglio con quel cod_iva
                        'quindi scrivo direttamente la % di iva indetraibile
                        ivaIndetraibilePerc = CDec(dr(0).Item("Iva_Indetraibile_Perc"))
                        If ivaIndetraibilePerc <> 0 Then
                            noteIva = CStr(dr(0).Item("Iva_Indetraibile_Perc")) & "%"
                        End If

                    Case Else
                        'sul cod_iva ci sono più dettagli (% di iva indetraibile e/o nessuna)
                        'le visualizzo per ogni imponibile tot

                        For i = 0 To dr.Length - 1

                            ' chiaveHT = CStr(Dr(i).Item("cod_iva")) & "|" & CStr(Dr(i).Item("Iva_Indetraibile_Perc"))
                            chiaveHT = CStr(dr(i).Item("Iva_Indetraibile_Perc"))

                            If chiaveHT <> 0 Then

                                'imp = CStr(Dr(i).Item("imponibile_netto"))
                                iva = objcontab.Leggi_IVA_PositivaNegativa(lav_cod, CDec(dr(i).Item("iva")))

                                If Not ht.Contains(chiaveHT) Then
                                    ht.Add(chiaveHT, iva)
                                Else
                                    ivaTot = ht(chiaveHT)
                                    ivaTot += iva
                                    ht(chiaveHT) = ivaTot
                                End If


                                'If Not chiaveHT.Contains(chiaveHT) Then
                                '    HT.Add(chiaveHT, imp)
                                'Else
                                '    imp_tot = HT(chiaveHT)
                                '    imp_tot += imp
                                '    HT(chiaveHT) = imp_tot
                                'End If

                            End If

                        Next

                        Dim key As Object
                        For Each key In ht.Keys
                            noteIva += CStr(key) & "% su iva " & CStr(ht(key)) & "€  "
                        Next

                End Select

            End If 'select su dt

        End If 'dt vuoto 


        Return noteIva

    End Function

    '########################################################################################
    'se si aggiungono colonne occorre aggiornare le costanti con gli index!!!!!!!!!!!!!!!!!!
    Public Sub InserisciRiga_DtDettagli(ByRef Dt As DataTable,
                                        ByVal Id_Mov_Det As Integer,
                                        ByVal ChkLayOut_Hide As Integer,
                                        ByVal modalita_sconto As Integer,
                                        ByVal sconto As Decimal,
                                        ByVal sconto_listino As Decimal,
                                        ByVal qta As Decimal,
                                        ByVal prezzo_unitario As Decimal,
                                        ByVal prezzo_unitario_netto As Decimal,
                                        ByVal imponibile As Decimal,
                                        ByVal imponibile_netto As Decimal,
                                        ByVal cod_iva As Integer,
                                        ByVal iva As Decimal,
                                        ByVal aliquota_iva As Integer,
                                        ByVal aliquota_des As String,
                                        ByVal importo_totale As Decimal,
                                        ByVal Iva_Indetraibile As Decimal,
                                        ByVal Iva_Indetraibile_Perc As Decimal,
                                        ByVal modalita_doc As Integer)

        If Not IsNothing(Dt) Then

            If ChkLayOut_Hide <> 1 Then

                Dim dr As DataRow

                'Creo una nuova riga
                dr = Dt.NewRow

                dr.Item("Id_Mov_Det") = Id_Mov_Det
                dr.Item("modalita_sconto") = modalita_sconto
                dr.Item("sconto") = sconto
                dr.Item("sconto_listino") = sconto_listino
                dr.Item("qta") = qta
                dr.Item("prezzo_unitario") = prezzo_unitario
                dr.Item("prezzo_unitario_netto") = prezzo_unitario_netto
                dr.Item("imponibile") = imponibile
                dr.Item("imponibile_netto") = imponibile_netto
                dr.Item("cod_iva") = cod_iva
                dr.Item("iva") = iva
                dr.Item("aliquota_iva") = aliquota_iva
                dr.Item("aliquota_des") = aliquota_des
                dr.Item("importo_totale") = importo_totale
                dr.Item("Iva_Indetraibile") = Iva_Indetraibile
                dr.Item("Iva_Indetraibile_Perc") = Iva_Indetraibile_Perc
                dr.Item("modalita_doc") = modalita_doc

                'Associo alla tabella la nuova riga creata
                Dt.Rows.Add(dr)

            End If

        End If

    End Sub


#End Region


#Region "Funzioni private"


    '###################################################################
    Private Sub FormImpostaArrotondamenti(ByRef Imponibile_Netto As Decimal,
                                          ByRef Imponibile_Lordo As Decimal,
                                          ByRef Imposta As Decimal,
                                          ByVal Edit_Importo As enum_EditImporto)

        Dim Delta_Imponibile As Decimal
        Dim Delta_Imposta As Decimal

        Delta_Imponibile = Imponibile_Netto - Format2(Imponibile_Netto)

        Delta_Imposta = Imposta - Format2(Imposta)


        If (Math.Abs(Delta_Imponibile + Delta_Imposta) >= CDbl("0,01")) OrElse
           (Math.Abs(Delta_Imponibile + Delta_Imposta) < CDbl("0,005")) Then

            'Arrotondamento estremo: entrambi per eccesso o per difetto (Naturale)
            Imponibile_Netto = Format2(Imponibile_Netto)
            Imponibile_Lordo = Format2(Imponibile_Lordo)

            Imposta = Format2(Imposta)


        ElseIf Delta_Imponibile > 0 AndAlso Delta_Imposta > 0 Then

            'Impedisco l'arrotondamento per eccesso dell'imponibile ed arrotondo l'iva per eccesso (no sanzioni)
            Imponibile_Netto = Format2(Imponibile_Netto)
            Imponibile_Lordo = Format2(Imponibile_Lordo)

            Imposta = Format2(Imposta) + CDbl("0,01")

            '24/03/15: ritolgo commento x segnalazione su ricevute fiscali
            '/////////
            '06/02/2015: 
            'commentato per risolvere segnalazione della Fiorini (mail di mercoledì 17/12/2014 09:44)
            'su arrotondamento nota di accredito
            '////////
        ElseIf Delta_Imponibile < 0 AndAlso Delta_Imposta > 0 Then

            'Grazie all'italia che è una repubblica fondata sul furto è meglio pagare un cent in +.
            'arrotondamento per difetto imponibile e per eccesso imposta (no sanzioni)
            Imponibile_Netto = Format2(Imponibile_Netto) - CDbl("0,01")
            Imponibile_Lordo = Format2(Imponibile_Lordo) - CDbl("0,01")

            Imposta = Format2(Imposta) + CDbl("0,01")

        Else

            ''24/03/15: ritolgo commento x segnalazione su ricevute fiscali
            ''/////////
            ''06/02/2015:
            ''commentato per risolvere segnalazione della Fiorini (mail di mercoledì 17/12/2014 09:44)
            ''su arrotondamento nota di accredito
            ''Arrotondamento x difetto
            ''////////////
            'Imponibile_Netto = Format2(Imponibile_Netto) - CDbl("0,01")
            'Imponibile_Lordo = Format2(Imponibile_Lordo) - CDbl("0,01")
            ''  Imponibile_Netto = Format2(Imponibile_Netto) '- CDbl("0,01")
            ''  Imponibile_Lordo = Format2(Imponibile_Lordo) '- CDbl("0,01")

            'nuova modifica del 26/03/2015:
            'gestito salvataggio sul documento della modalità di inserimento prezzi (a partire dal prezzo unitario, dal totale di riga...)
            Select Case Edit_Importo

                Case enum_EditImporto.PrezzoUnitario
                    Imponibile_Netto = Format2(Imponibile_Netto)
                    Imponibile_Lordo = Format2(Imponibile_Lordo)

                Case Else
                    'Arrotondamento x difetto
                    Imponibile_Netto = Format2(Imponibile_Netto) - CDbl("0,01")
                    Imponibile_Lordo = Format2(Imponibile_Lordo) - CDbl("0,01")

            End Select

            Imposta = Format2(Imposta)

        End If

    End Sub

    '########################################################################
    Private Function ChkDuplicatiGriglia(ByVal dt As DataTable, ByVal codice As Long, ByVal colonna As Integer) As Integer

        Dim indiceRigaDuplicato As Integer = DEFAULT_INDICE_RIGA_DUPLICATO

        'Verifica Duplicati in Griglia
        For i As Integer = 0 To dt.Rows.Count - 1
            If codice = CLng(dt.Rows(i).Item(colonna)) Then
                indiceRigaDuplicato = i
                Exit For
            End If
        Next

        Return indiceRigaDuplicato

    End Function

    '########################################################################
    Private Function ChkDuplicatiGriglia2(ByVal dt As DataTable,
                                          ByVal codice1 As Integer, ByVal colonna1 As Integer,
                                          ByVal codice2 As Decimal, ByVal colonna2 As Integer,
                                          ByVal codice3 As Integer, ByVal colonna3 As Integer
                                          ) As Integer

        Dim indiceRigaDuplicato As Integer = DEFAULT_INDICE_RIGA_DUPLICATO

        'Verifica Duplicati in Griglia
        For i As Integer = 0 To dt.Rows.Count - 1
            If codice1 = CInt(dt.Rows(i).Item(colonna1)) AndAlso
               codice2 = CDec(dt.Rows(i).Item(colonna2)) AndAlso
               codice3 = CInt(dt.Rows(i).Item(colonna3)) Then
                indiceRigaDuplicato = i
                Exit For
            End If
        Next

        Return indiceRigaDuplicato

    End Function


    '########################################################################################
    'se si aggiungono colonne occorre aggiornare le costanti con gli index!!!!!!!!!!!!!!!!!!
    Private Sub InserisciRiga_DtIva(ByRef DtIva As DataTable,
                                    ByVal imponibile As Decimal,
                                    ByVal imponibile_netto As Decimal,
                                    ByVal cod_iva As Integer,
                                    ByVal iva As Decimal,
                                    ByVal aliquota_iva As Decimal,
                                    ByVal aliquota_des As String,
                                    ByVal chkiva_manuale As Integer,
                                    ByVal prezzo As Decimal,
                                    ByVal imposta_omaggio As Decimal,
                                    ByVal iva_split As Decimal)

        Dim dr As DataRow
        dr = DtIva.NewRow

        dr.Item("imponibile") = imponibile
        dr.Item("imponibile_netto") = imponibile_netto
        dr.Item("cod_iva") = cod_iva
        dr.Item("iva") = iva
        dr.Item("aliquota_iva") = aliquota_iva
        dr.Item("aliquota_des") = aliquota_des
        dr.Item("chkiva_manuale") = chkiva_manuale
        dr.Item("prezzo") = prezzo
        dr.Item("imposta_omaggio") = imposta_omaggio
        dr.Item("iva_split") = iva_split

        DtIva.Rows.Add(dr)

    End Sub

    '########################################################################################
    Private Sub InserisciRiga_DtIva_LiqIva(ByRef DtIva As DataTable,
                                           ByVal imponibile As Decimal,
                                           ByVal imponibile_netto As Decimal,
                                           ByVal cod_iva As Integer,
                                           ByVal iva As Decimal,
                                           ByVal aliquota_iva As Decimal,
                                           ByVal aliquota_des As String,
                                           ByVal chkiva_manuale As Integer,
                                           ByVal prezzo As Decimal,
                                           ByVal imposta_omaggio As Decimal,
                                           ByVal iva_indetraibile_perc As Decimal,
                                           ByVal Iva_Indetraibile As Decimal,
                                           ByVal modalita_doc As Integer,
                                           ByVal iva_split As Decimal)

        Dim dr As DataRow
        dr = DtIva.NewRow

        dr.Item("imponibile") = imponibile
        dr.Item("imponibile_netto") = imponibile_netto
        dr.Item("cod_iva") = cod_iva
        dr.Item("iva") = iva
        dr.Item("aliquota_iva") = aliquota_iva
        dr.Item("aliquota_des") = aliquota_des
        dr.Item("chkiva_manuale") = chkiva_manuale
        dr.Item("prezzo") = prezzo
        dr.Item("imposta_omaggio") = imposta_omaggio
        dr.Item("iva_indetraibile_perc") = iva_indetraibile_perc
        dr.Item("Iva_Indetraibile") = Iva_Indetraibile
        dr.Item("modalita_doc") = modalita_doc
        dr.Item("iva_split") = iva_split

        DtIva.Rows.Add(dr)

    End Sub

#End Region

End Class