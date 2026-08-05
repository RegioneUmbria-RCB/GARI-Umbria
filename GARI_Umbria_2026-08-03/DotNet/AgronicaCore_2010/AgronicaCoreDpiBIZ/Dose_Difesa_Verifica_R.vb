
Imports System.Data
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports System.Web
Imports AgronicaCoreDataProvider
Imports System.Data.Common

Public Class Dose_Difesa_Verifica_R

    Inherits AgronicaCoreDataProvider.LogProvider

    '============================================================================
    Public Sub Dose_Etichetta(ByRef Udm_Cod As Int32,
                          ByRef Dose_min As Decimal,
                          ByRef Dose_Max As Decimal,
                          ByRef Descrizione As String,
                          ByVal Pro_Cod As Int32,
                          ByVal TipoTestata As Int32,
                          ByVal Veg_Cod As Int32,
                          ByVal Av_Cod As Int32,
                          ByVal Av_Gru As Int32,
                          ByVal Epoca_Cod As Int32,
                          ByVal grfi_cod As Integer,
                          ByVal bCoadiuvante As Boolean,
                            ByVal Data As Date,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          )

        Dim NomeRoutine As String = "DpiBIZ.Dose_Difesa_Verifica_R.Dose_Etichetta()"

        Dim MessaggioErrore As String = ""

        Dim FlagConnessioneLocale As Boolean = False
        Dim xConnessione As DbConnection

        'Dim RisultatoFunzione As String = String.Empty

        Dim xConnectionState As ConnectionState = ConnectionState.Closed

        '======

        Dim Setting As Boolean
        'Dim Coadiuvante As Boolean

        Try

            '------------------------------
            'Verifico se e' stata impostata una connessione
            If IsNothing(objParametri.objConnessione) Then
                'Flag
                FlagConnessioneLocale = True
                'Creo la connessione localmente
                xConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
            Else
                'Utilizzo quella passata come parametro
                xConnessione = objParametri.objConnessione
                xConnectionState = objParametri.objConnessione.State
            End If
            '------------------------------


            'Verifica se il prodotto è 1 coadiuvante

            'Dim ObjClassificazione As New AgronicaCoreMetaSchemaDAL.FormulatixClassifica_R
            'Dim DtClassificazione As DataTable
            'DtClassificazione = ObjClassificazione.Leggi(Pro_Cod, 0, AGRODATAINIZIO, AGRODATAFINE, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)

            'If DtClassificazione.Rows.Count > 0 Then

            '    Select Case CLng(DtClassificazione.Rows(0).Item("class_cod"))

            '        Case Is < 500, Is > 506

            '            Coadiuvante = False

            '        Case Else

            '            Coadiuvante = True

            '    End Select

            'Else

            '    'Eccezione
            '    Coadiuvante = False

            'End If


            '============================================================================================================================
            'Lettura della dose consigliata
            '----------------------------------------------------------------------------------------------------------------------------
            Setting = False

            'La dose consigliata si trova nella tabella FormulatixSpeciexAvversitaxDosi
            Dim DtDose As DataTable

            Dim ObjDose As New AgronicaCoreDpiDAL.Fitofarmaci

            Select Case bCoadiuvante

                Case True

                    If TipoTestata = 1 And Av_Cod = -3 And Av_Gru = -3 Then

                        ObjDose.AgroWS_InfoRead_Formulati_SpecieVegetali_Infestanti_Dosi(
                                objParametri,
                                DtDose,
                                MessaggioErrore,
                                Pro_Cod,
                                Veg_Cod,
                                Av_Cod,
                                Av_Gru,
                                grfi_cod,
                                "0",
                                Data)

                    Else

                        ObjDose.AgroWS_InfoRead_Formulati_SpecieVegetali_Avversita_Dosi(
                                objParametri,
                                DtDose,
                                MessaggioErrore,
                                Pro_Cod,
                                Veg_Cod,
                                Av_Cod,
                                Av_Gru,
                                grfi_cod,
                                "0",
                                Data)

                    End If

                Case Else

                    Select Case TipoTestata

                        Case 0 'Difesa

                            ObjDose.AgroWS_InfoRead_Formulati_SpecieVegetali_Avversita_Dosi(
                                                objParametri,
                                                DtDose,
                                                MessaggioErrore,
                                                Pro_Cod,
                                                Veg_Cod,
                                                Av_Cod,
                                                Av_Gru,
                                                grfi_cod,
                                                "0",
                                                Data)

                        Case 1 'Diserbo

                            ObjDose.AgroWS_InfoRead_Formulati_SpecieVegetali_Infestanti_Dosi(
                                                objParametri,
                                                DtDose,
                                                MessaggioErrore,
                                                Pro_Cod,
                                                Veg_Cod,
                                                Av_Cod,
                                                Av_Gru,
                                                grfi_cod,
                                                "0",
                                                Data)
                    End Select

            End Select

            If DtDose.Rows.Count <> 0 Then

                Setting = True

            End If


            ''Select Case TipoTestata

            ''    Case 0 'Difesa

            ''        'Dim ObjDose As New AgronicaCoreMetaSchemaDAL.FormulatixSpeciexAvvxDosi_R

            ''        Select Case Coadiuvante

            ''            Case True

            ''                'Dim objDPILeggi As New AgronicaCoreDpiBIZ.Fitofarmaci_Leggi
            ''                'DtDose = objDPILeggi.Formulati_SpecieVegetali_Avversita_Dosi( _
            ''                '                    CInt(Pro_Cod), _
            ''                '                    CInt(Veg_Cod), _
            ''                '                    CInt(0), _
            ''                '                    CInt(0), _
            ''                '                    HttpContext.Current.Session)

            ''                'DtDose = ObjDose.Leggi2(0, Pro_Cod, 0, 0, "", AGRODATAINIZIO, AGRODATAFINE, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)

            ''            Case False

            ''                DtDose = objDPILeggi.Formulati_SpecieVegetali_Avversita_Dosi( _
            ''                                    CInt(Pro_Cod), _
            ''                                    CInt(Veg_Cod), _
            ''                                    CInt(Av_Gru), _
            ''                                    CInt(Av_Cod), _
            ''                                    HttpContext.Current.Session)

            ''                'DtDose = ObjDose.Leggi_New(0, Pro_Cod, 0, 0, "", AGRODATAINIZIO, AGRODATAFINE, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)

            ''                'If RsDose.State = 0 And Av_Gru <> 0 Then
            ''                'If DtDose.Rows.Count = 0 And Av_Gru <> 0 Then
            ''                '    'Lettura della dose impostata sul gruppo inesistente --> dose sulle singole avversità

            ''                '    Dim ObjAvversitaxGruppoAvversita As New AgronicaCoreMetaSchemaDAL.AvversitaxGruppoAvversita_R
            ''                '    Dim DtAvversita As DataTable

            ''                '    Dim strAvversita As String

            ''                '    DtAvversita = ObjAvversitaxGruppoAvversita.Leggi(0, 0, Av_Gru, AGRODATAINIZIO, AGRODATAFINE, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)

            ''                '    If DtAvversita.Rows.Count <> 0 Then

            ''                '        Dim iAvv As Integer

            ''                '        For iAvv = 0 To DtAvversita.Rows.Count - 1

            ''                '            strAvversita = strAvversita & DtAvversita.Rows(iAvv).Item("Av_Cod") & ", "

            ''                '        Next

            ''                '    End If

            ''                '    If Trim(strAvversita) <> "" Then
            ''                '        strAvversita = "Av_Cod IN ( " & Left(strAvversita, Len(strAvversita) - 2) & " ) "

            ''                '        DtDose = ObjDose.Leggi_New(Veg_Cod, Pro_Cod, 0, 0, strAvversita, AGRODATAINIZIO, AGRODATAFINE, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)

            ''                '    End If

            ''                '    ObjAvversitaxGruppoAvversita = Nothing

            ''                'End If


            ''        End Select

            ''        If DtDose.Rows.Count <> 0 Then

            ''            Setting = True

            ''        End If


            ''    Case 1 'Diserbo

            ''        'Inserimento Dose Etichetta

            ''        Select Case Coadiuvante

            ''            Case True

            ''                ''In caso di Coadiuvante la dose consigliata si trova nella stessa tabella utilizzata
            ''                ''in caso di difesa: 'FormulatixSpeciexAvvxDosi_R'. NO COMMENT!!!!!
            ''                'Dim ObjDose As New AgronicaCoreMetaSchemaDAL.FormulatixSpeciexAvvxDosi_R
            ''                'DtDose = ObjDose.Leggi_New(0, Pro_Cod, 0, 0, "", AGRODATAINIZIO, AGRODATAFINE, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)

            ''                DtDose = objDPILeggi.Formulati_SpecieVegetali_Avversita_Dosi( _
            ''                                    CInt(Pro_Cod), _
            ''                                    CInt(Veg_Cod), _
            ''                                    CInt(0), _
            ''                                    CInt(0), _
            ''                                    HttpContext.Current.Session)


            ''            Case False

            ''                'Dim ObjDose As New AgronicaCoreMetaSchemaDAL.FormulatixSpeciexInfxDosi_R
            ''                DtDose = objDPILeggi.Formulati_SpecieVegetali_Infestanti_Dosi( _
            ''                                    CInt(Pro_Cod), _
            ''                                    CInt(Veg_Cod), _
            ''                                    CInt(Av_Gru), _
            ''                                    CInt(Av_Cod), _
            ''                                    HttpContext.Current.Session)

            ''                ' DtDose = ObjDose.Leggi_New(Veg_Cod, Pro_Cod, Av_Gru, Av_Cod, "", Epoca_Cod, AGRODATAINIZIO, AGRODATAFINE, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)

            ''                'If DtDose.Rows.Count = 0 And Av_Gru <> 0 Then

            ''                '    '-------------------
            ''                '    'Lettura della dose impostata sul gruppo inesistente --> dose sulle singole avversità
            ''                '    Dim ObjInfestantixGruppoInfestanti As New AgronicaCoreMetaSchemaDAL.InfestantiAttive_R
            ''                '    Dim DtInfestanti As DataTable
            ''                '    Dim strInfestanti As String

            ''                '    ObjInfestantixGruppoInfestanti = CreateObject("AgronicaCoreMetaSchemaDAL.InfestantiAttive_R")
            ''                '    DtInfestanti = ObjInfestantixGruppoInfestanti.Leggi(0, Av_Gru, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)

            ''                '    If DtInfestanti.Rows.Count <> 0 Then

            ''                '        Dim iInf As Integer
            ''                '        For iInf = 0 To DtInfestanti.Rows.Count - 1

            ''                '            strInfestanti = strInfestanti & DtInfestanti.Rows(iInf).Item("Av_Cod") & ", "

            ''                '        Next

            ''                '    End If

            ''                '    If Trim(strInfestanti) <> "" Then

            ''                '        strInfestanti = "Av_Cod IN ( " & Left(strInfestanti, Len(strInfestanti) - 2) & " ) "

            ''                '        DtDose = ObjDose.Leggi_New(Veg_Cod, Pro_Cod, 0, 0, strInfestanti, 0, AGRODATAINIZIO, AGRODATAFINE, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)

            ''                '    End If

            ''                '    ObjInfestantixGruppoInfestanti = Nothing

            ''                '    '------------------------

            ''                'End If

            ''                'End If


            ''        End Select

            ''        If DtDose.Rows.Count <> 0 Then

            ''            Setting = True

            ''        End If

            ''End Select


            If Setting Then

                'Dose consigliata impostata
                If IsNumeric(DtDose.Rows(0).Item("Dose_Max")) Then
                    Dose_Max = DtDose.Rows(0).Item("Dose_Max")
                Else 'Eccezione
                    Dose_Max = 0
                End If

                If IsNumeric(DtDose.Rows(0).Item("Dose_min")) Then
                    Dose_min = DtDose.Rows(0).Item("Dose_min")
                Else 'Eccezione
                    Dose_min = 0
                End If


                Select Case CLng(DtDose.Rows(0).Item("Udm_cod"))

                    Case 23 'g/hl
                        Udm_Cod = CLng(DtDose.Rows(0).Item("Udm_Cod"))
                        Descrizione = "g/hl " & Dose_min & "-" & Dose_Max
                    Case 22 'l/ha
                        Udm_Cod = CLng(DtDose.Rows(0).Item("Udm_Cod"))
                        Descrizione = "l/ha " & Dose_min & "-" & Dose_Max
                    Case 170 'ml/pianta
                        Udm_Cod = -1 'Non Gestito
                        Descrizione = "ml/pianta " & Dose_min & "-" & Dose_Max
                    Case 300 'g/m
                        Udm_Cod = CLng(DtDose.Rows(0).Item("Udm_Cod"))
                        Descrizione = "g/m " & Dose_min & "-" & Dose_Max
                    Case 29 'l
                        Udm_Cod = CLng(DtDose.Rows(0).Item("Udm_Cod"))
                        Descrizione = "l " & Dose_min & "-" & Dose_Max
                    Case 174 'g/q
                        Udm_Cod = CLng(DtDose.Rows(0).Item("Udm_Cod"))
                        Descrizione = "g/q " & Dose_min & "-" & Dose_Max
                    Case 175 'Kg/hl
                        Udm_Cod = CLng(DtDose.Rows(0).Item("Udm_Cod"))
                        Descrizione = "kg/hl " & Dose_min & "-" & Dose_Max
                    Case 21 'cc/hl
                        Udm_Cod = CLng(DtDose.Rows(0).Item("Udm_Cod"))
                        Descrizione = "cc/hl " & Dose_min & "-" & Dose_Max
                    Case 20 'g/ha
                        Udm_Cod = CLng(DtDose.Rows(0).Item("Udm_Cod"))
                        Descrizione = "g/ha " & Dose_min & "-" & Dose_Max
                    Case 21 'cc/hl
                        Udm_Cod = CLng(DtDose.Rows(0).Item("Udm_Cod"))
                        Descrizione = "cc/hl " & Dose_min & "-" & Dose_Max
                    Case 302 'l/mq
                        Udm_Cod = CLng(DtDose.Rows(0).Item("Udm_Cod"))
                        Descrizione = "l/mq " & Dose_min & "-" & Dose_Max
                    Case 165 'ml/q
                        Udm_Cod = CLng(DtDose.Rows(0).Item("Udm_Cod"))
                        Descrizione = "ml/q " & Dose_min & "-" & Dose_Max
                    Case 173 'l/hl
                        Udm_Cod = CLng(DtDose.Rows(0).Item("Udm_Cod"))
                        Descrizione = "l/hl " & Dose_min & "-" & Dose_Max
                    Case 303 'l/q
                        Udm_Cod = CLng(DtDose.Rows(0).Item("Udm_Cod"))
                        Descrizione = "l/q " & Dose_min & "-" & Dose_Max
                    Case 3 'g
                        Udm_Cod = CLng(DtDose.Rows(0).Item("Udm_Cod"))
                        Descrizione = "g " & Dose_min & "-" & Dose_Max
                    Case 88 'Kg/ha
                        Udm_Cod = CLng(DtDose.Rows(0).Item("Udm_Cod"))
                        Descrizione = "kg/ha " & Dose_min & "-" & Dose_Max
                    Case 2 'Kg
                        Udm_Cod = CLng(DtDose.Rows(0).Item("Udm_Cod"))
                        Descrizione = "kg " & Dose_min & "-" & Dose_Max
                    Case 163 'ml/ha
                        Udm_Cod = CLng(DtDose.Rows(0).Item("Udm_Cod"))
                        Descrizione = "ml/ha " & Dose_min & " - " & Dose_Max
                    Case 171 'g/mq
                        Udm_Cod = CLng(DtDose.Rows(0).Item("Udm_Cod"))
                        Descrizione = "g/mq " & Dose_min & "-" & Dose_Max
                    Case 101 'ml
                        Udm_Cod = CLng(DtDose.Rows(0).Item("Udm_Cod"))
                        Descrizione = "ml " & Dose_min & "-" & Dose_Max
                    Case 164 'ml/hl
                        Udm_Cod = CLng(DtDose.Rows(0).Item("Udm_Cod"))
                        Descrizione = "ml/hl " & Dose_min & "-" & Dose_Max
                    Case 172 'ml/mq
                        Udm_Cod = CLng(DtDose.Rows(0).Item("Udm_Cod"))
                        Descrizione = "ml/mq " & Dose_min & "-" & Dose_Max
                    Case Else
                        Udm_Cod = -1 'Non Gestito
                        Descrizione = "Non Disponibile"
                        Dose_min = 0
                        Dose_Max = 0
                End Select

            Else

                Udm_Cod = -2 'Non Presente
                Descrizione = "Non Disponibile"
                Dose_min = 0
                Dose_Max = 0

            End If

        Catch ex As Exception

            'RisultatoFunzione = ""
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

            Udm_Cod = -1
            Descrizione = "Non Disponibile"
            Dose_min = 0
            Dose_Max = 0

        Finally

            If FlagConnessioneLocale = True Then
                If Not IsNothing(xConnessione) Then
                    xConnessione.Close()
                    xConnessione.Dispose()
                End If
            Else
                If xConnectionState = ConnectionState.Closed Then
                    xConnessione.Close()
                End If
            End If

        End Try


    End Sub

    '#############################################################################################################
    '#############################################################################################################
    '#############################################################################################################

    'Routine che verifica se la dose utilizzata nella lavorazione identificata dalla quaterna:
    '  1.Mezzo_Lavorazione
    '  2.Udm_Cod_Lavorazione
    '  3.Dose_Lavorazione
    '  4.H2O_Lavorazione
    'è corretta. In caso contrario ritorna un errore parametrico.


    'Public Function Dose_Verifica(ByRef Err_Code As Int32, _
    '                                  ByVal Lav_Cod As Int32, _
    '                                ByVal Veg_Cod As Int32, _
    '                                ByVal Fr_Cod As Int32, _
    '                                ByVal Fr_Des As String, _
    '                                ByVal Mezzo_Lavorazione As Int32, _
    '                                ByVal Udm_Cod_Lavorazione As Int32, _
    '                                ByVal Dose_Lavorazione As Decimal, _
    '                                ByVal H2O_Lavorazione As Decimal, _
    '                                ByVal Superficie_Lavorazione As Decimal, _
    '                                ByVal Av_Gru As Int32, _
    '                                ByVal Av_Cod As Int32, _
    '                                ByVal grfi_cod As Integer, _
    '                                ByVal strAvversita As String, _
    '                                ByVal Data As Date, _
    '                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                              ) As String

    '    Dim NomeRoutine As String = "DpiBIZ.Dose_Difesa_Verifica_R.Dose_Verifica()"

    '    Dim MessaggioErrore As String = ""

    '    Dim FlagConnessioneLocale As Boolean = False
    '    Dim xConnessione As DbConnection
    '    Dim xConnectionState As ConnectionState = ConnectionState.Closed

    '    Dim RisultatoFunzione As String = String.Empty

    '    '======

    '    Dim Udm_Cod_Etichetta As Long
    '    Dim Udm_Des_Etichetta As String
    '    Dim Coefficiente_Etichetta As Decimal
    '    Dim Qta_Etichetta As Decimal

    '    Dim Qta_Etichetta_Max_Ha As Decimal
    '    Dim Qta_Etichetta_Max_Hl As Decimal

    '    Dim Qta_Utilizzata As Decimal
    '    Dim Udm_Cod_Utilizzato As Long
    '    Dim Udm_Des_Utilizzato As String

    '    Dim Dose_Max As Decimal
    '    Dim Mezzo_Etichetta As Integer
    '    Dim Mezzo_Lavorazione_Des As String


    '    Dim VerificaDose As Boolean

    '    Try

    '        '------------------------------
    '        'Verifico se e' stata impostata una connessione
    '        If IsNothing(objParametri.objConnessione) Then
    '            'Flag
    '            FlagConnessioneLocale = True
    '            'Creo la connessione localmente
    '            xConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
    '        Else
    '            'Utilizzo quella passata come parametro
    '            xConnessione = objParametri.objConnessione
    '            xConnectionState = objParametri.objConnessione.State
    '        End If

    '        '------------------------------
    '        'Lettura della dose consigliata da etichetta
    '        Dim DtDose As DataTable

    '        Dim ObjDose As New AgronicaCoreDpiDAL.Fitofarmaci
    '        ObjDose.AgroWS_InfoRead_Formulati_SpecieVegetali_Avversita_Dosi( _
    '                                        objParametri, _
    '                                        DtDose, _
    '                                        MessaggioErrore, _
    '                                        Fr_Cod, _
    '                                        Veg_Cod, _
    '                                        Av_Cod, _
    '                                        Av_Gru, _
    '                                        grfi_cod, _
    '                                        "0", _
    '                                        Data)

    '        Dim i As Integer

    '        If DtDose.Rows.Count <> 0 Then

    '            Dim For_Veg_Av_Dos_Cod As New Hashtable

    '            'Verifico le n-dosi
    '            For i = 0 To DtDose.Rows.Count - 1

    '                'evito le doppie dosi (dovute alla gerarchia dei gruppi avversità)
    '                If Not For_Veg_Av_Dos_Cod.ContainsKey(CInt(DtDose.Rows(i).Item("For_Veg_Av_Dos_Cod"))) Then

    '                    VerificaDose = False

    '                    For_Veg_Av_Dos_Cod.Add(CInt(DtDose.Rows(i).Item("For_Veg_Av_Dos_Cod")), "")

    '                    'Impostazione Dose Max da Etichetta
    '                    If IsNumeric(DtDose.Rows(i).Item("Dose_Max")) Then
    '                        Dose_Max = DtDose.Rows(i).Item("Dose_Max")
    '                    Else
    '                        'Eccezione
    '                        Dose_Max = 0
    '                    End If


    '                    '##############################################################################################
    '                    '######## CALCOLO DELLA QUANTITA' TOTALE DISTRIBUIBILE DA ETICHETTA FORMULATO #################
    '                    '##############################################################################################

    '                    'Impostazione dell'unità di misura

    '                    'Utilizzo la seguente codifica per il mezzo_lavorazione
    '                    'Ettolitro = 0
    '                    'Ettaro = 1

    '                    Select Case Agro_SQL_SaveNum(DtDose.Rows(i).Item("Udm_Cod"))

    '                        Case 23 'g/hl
    '                            Udm_Cod_Etichetta = 2
    '                            Udm_Des_Etichetta = "grammi/Hl"
    '                            Coefficiente_Etichetta = 1 / 1000
    '                            Mezzo_Etichetta = 0 'Distribuzione per Ettolitro

    '                        Case 22 'l/ha
    '                            Udm_Cod_Etichetta = 29
    '                            Udm_Des_Etichetta = "Litri/Ha"
    '                            Coefficiente_Etichetta = 1
    '                            Mezzo_Etichetta = 1 'Distribuzione per Ettaro


    '                        Case 170 'ml/pianta
    '                            Udm_Cod_Etichetta = 29
    '                            Udm_Des_Etichetta = "ml/pianta"
    '                            Coefficiente_Etichetta = -1 'Indefinita
    '                            Mezzo_Etichetta = -1

    '                        Case 175 'Kg/hl
    '                            Udm_Cod_Etichetta = 2
    '                            Udm_Des_Etichetta = "Kg/Hl"
    '                            Coefficiente_Etichetta = 1
    '                            Mezzo_Etichetta = 0 'Distribuzione per Ettolitro

    '                        Case 21 'cc/hl
    '                            Udm_Cod_Etichetta = 29
    '                            Udm_Des_Etichetta = "cc/Hl"
    '                            Coefficiente_Etichetta = 1 / 1000
    '                            Mezzo_Etichetta = 0 'Distribuzione per Ettolitro

    '                        Case 20 'g/ha
    '                            Udm_Cod_Etichetta = 2
    '                            Udm_Des_Etichetta = "Grammi/Ha"
    '                            Coefficiente_Etichetta = 1 / 1000
    '                            Mezzo_Etichetta = 1 'Distribuzione per Ettaro

    '                        Case 302 'l/mq
    '                            Udm_Cod_Etichetta = 29
    '                            Udm_Des_Etichetta = "Litri/mq"
    '                            Coefficiente_Etichetta = 10000
    '                            Mezzo_Etichetta = 1 'Distribuzione per Ettaro


    '                        Case 173 'l/hl
    '                            Udm_Cod_Etichetta = 29
    '                            Udm_Des_Etichetta = "Litri/Hl"
    '                            Coefficiente_Etichetta = 1
    '                            Mezzo_Etichetta = 0 'Distribuzione per Ettolitro

    '                        Case 88 'Kg/ha
    '                            Udm_Cod_Etichetta = 2
    '                            Udm_Des_Etichetta = "Kg/Ha"
    '                            Coefficiente_Etichetta = 1
    '                            Mezzo_Etichetta = 1 'Distribuzione per Ettaro

    '                        Case 163 'ml/ha
    '                            Udm_Cod_Etichetta = 29
    '                            Udm_Des_Etichetta = "ml/Ha"
    '                            Coefficiente_Etichetta = 1 / 1000
    '                            Mezzo_Etichetta = 1 'Distribuzione per Ettaro

    '                        Case 171 'g/mq
    '                            Udm_Cod_Etichetta = 29
    '                            Udm_Des_Etichetta = "Grammi/mq"
    '                            Coefficiente_Etichetta = 10
    '                            Mezzo_Etichetta = 1 'Distribuzione per Ettaro

    '                        Case 164, 101 'ml/hl
    '                            Udm_Cod_Etichetta = 29
    '                            Udm_Des_Etichetta = "ml/Hl"
    '                            Coefficiente_Etichetta = 1 / 1000
    '                            Mezzo_Etichetta = 0 'Distribuzione per Ettolitro

    '                        Case 172 'ml/mq
    '                            Udm_Cod_Etichetta = 29
    '                            Udm_Des_Etichetta = "ml/mq"
    '                            Coefficiente_Etichetta = 10
    '                            Mezzo_Etichetta = 1 'Distribuzione per Ettaro

    '                        Case Else
    '                            'Mappatura non esistente
    '                            Coefficiente_Etichetta = -1

    '                    End Select


    '                    '=========================================================================
    '                    'Verifica
    '                    '-------------------------------------------------------------------------
    '                    'Codifica Errori
    '                    '-1.  Errore durante l'esecuzione della routine
    '                    ' 0.  Dose Ok
    '                    ' 100.  Dose Consigliata Non Disponibile
    '                    ' 101. Unità di Misura Non Compatibile
    '                    ' 102. Dose Eccessiva
    '                    ' 103. La qta di H2O non è corretta.

    '                    'Calcolo dose totale da etichetta
    '                    Select Case Mezzo_Etichetta
    '                        Case 0 'Distribuzione per Hl
    '                            Qta_Etichetta = Dose_Max * H2O_Lavorazione * Coefficiente_Etichetta
    '                        Case 1 'Distribuzione per Ha
    '                            Qta_Etichetta = Dose_Max * Superficie_Lavorazione * Coefficiente_Etichetta
    '                        Case Else 'Eccezione
    '                            Qta_Etichetta = 0
    '                    End Select

    '                    'Determino la qta di formulato distribuito espresso in macro udm
    '                    Qta_Formulato(Qta_Utilizzata, Udm_Cod_Utilizzato, Udm_Des_Utilizzato, Mezzo_Lavorazione_Des, Mezzo_Lavorazione, Udm_Cod_Lavorazione, Dose_Lavorazione, H2O_Lavorazione, Superficie_Lavorazione)

    '                    'Per le dosi doppie con lo stesso mezzo verifico SOLO le Massime 
    '                    'potrebbero esserci dosi diverse in base all'epoca (controllo al momento non gestito) 

    '                    Select Case Mezzo_Etichetta
    '                        Case 0 'Hl
    '                            If Qta_Etichetta_Max_Hl < Qta_Etichetta Then
    '                                Qta_Etichetta_Max_Hl = Qta_Etichetta
    '                                VerificaDose = True
    '                            End If
    '                        Case 1 'Ha
    '                            If Qta_Etichetta_Max_Ha < Qta_Etichetta Then
    '                                Qta_Etichetta_Max_Ha = Qta_Etichetta
    '                                VerificaDose = True
    '                            End If
    '                    End Select

    '                    If VerificaDose = True Then

    '                        If Udm_Cod_Utilizzato <> Udm_Cod_Etichetta Then
    '                            'Unità di Misura Non Compatibili
    '                            Err_Code = 101
    '                            Dose_Verifica = "L'unità di misura impiegata (" & Udm_Des_Utilizzato & Mezzo_Lavorazione_Des & ") del prodotto commerciale " & Fr_Des & " non è conforme all'etichetta (" & Udm_Des_Etichetta & ")."
    '                        Else

    '                            Select Case Lav_Cod
    '                                Case LAVCOD_GEODISINFESTAZIONE, LAVCOD_CONCIA_SEME
    '                                    'Controllo le quantità
    '                                    'Essendo Kg o Litri arrotondo al terzo decimale ...
    '                                    If Math.Round(Qta_Utilizzata, 3) > Math.Round(Qta_Etichetta, 3) Then
    '                                        'If Qta_Utilizzata > Qta_Etichetta Then
    '                                        'Dose Eccessiva
    '                                        Err_Code = 102
    '                                        Dose_Verifica = "La quantità impiegata di " & Fr_Des & " è eccessiva e non conforme all'etichetta del prodotto. "
    '                                        Dose_Verifica = Dose_Verifica & "La quantità distribuita è di " & Udm_Des_Utilizzato & " " & Qta_Utilizzata & " mentre quella distribuibile per l'intervento risulta essere non superiore a " & Udm_Des_Utilizzato & " " & Qta_Etichetta & "."
    '                                        'Exit For
    '                                    Else
    '                                        'OK
    '                                        Err_Code = 0
    '                                        Dose_Verifica = ""
    '                                    End If

    '                                Case Else
    '                                    If H2O_Lavorazione = 0 Then
    '                                        'La qta di H2O non è corretta
    '                                        Err_Code = 103
    '                                        Dose_Verifica = "Quantità di H2O non è corretta."
    '                                        'Exit For
    '                                    Else
    '                                        'Controllo le quantità
    '                                        'Essendo Kg o Litri arrotondo al terzo decimale ...
    '                                        If Math.Round(Qta_Utilizzata, 3) > Math.Round(Qta_Etichetta, 3) Then
    '                                            'If Qta_Utilizzata > Qta_Etichetta Then
    '                                            'Dose Eccessiva
    '                                            Err_Code = 102
    '                                            Dose_Verifica = "La quantità impiegata di " & Fr_Des & " è eccessiva e non conforme all'etichetta del prodotto. "
    '                                            Dose_Verifica = Dose_Verifica & "La quantità distribuita è di " & Udm_Des_Utilizzato & " " & Qta_Utilizzata & " mentre quella distribuibile per l'intervento risulta essere non superiore a " & Udm_Des_Utilizzato & " " & Qta_Etichetta & "."
    '                                            'Exit For
    '                                        Else
    '                                            'OK
    '                                            Err_Code = 0
    '                                            Dose_Verifica = ""
    '                                        End If

    '                                    End If
    '                            End Select
    '                            'Ok
    '                        End If

    '                    End If

    '                End If



    '            Next

    '        Else

    '            Coefficiente_Etichetta = -1 'Dose etichetta non disponibile

    '            'Mappatura non possbile
    '            Err_Code = 100
    '            Dose_Verifica = "Dose consigliata di " & Fr_Des & " non disponibile"

    '        End If


    '        '=========================================================================

    '        'Distruggo gli Oggetti
    '        DtDose = Nothing
    '        ObjDose = Nothing


    '    Catch ex As Exception

    '        RisultatoFunzione = ""
    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '        Err_Code = -1
    '        Dose_Verifica = ""

    '    Finally

    '        If FlagConnessioneLocale = True Then
    '            If Not IsNothing(xConnessione) Then
    '                xConnessione.Close()
    '                xConnessione.Dispose()
    '            End If
    '        Else
    '            If xConnectionState = ConnectionState.Closed Then
    '                xConnessione.Close()
    '            End If
    '        End If

    '    End Try


    'End Function
    'aggiunti controlli acqua, num trattamenti e intevallo trattamenti
    'restituisce + errori e + warning
    Public Sub Dose_Verifica_2(ByRef Err_Code() As Int32,
                               ByRef Err_Des() As String,
                               ByRef Warning_Code() As Int32,
                               ByRef Warning_Des() As String,
                               ByVal Lav_Cod As Int32,
                               ByVal Veg_Cod As Int32,
                               ByVal Fr_Cod As Int32,
                               ByVal Fr_Des As String,
                               ByVal Mezzo_Lavorazione As Int32,
                               ByVal Mezzo_Dettaglio_Lavorazione As Int32,
                               ByVal Udm_Cod_Lavorazione As Int32,
                               ByVal Dose_Lavorazione_Ha As Decimal,
                               ByVal Dose_Lavorazione_Hl As Decimal,
                               ByVal H2O_Ha As Decimal,
                               ByVal H2O_Lavorazione As Decimal,
                               ByVal Superficie_Lavorazione As Decimal,
                               ByVal Av_Gru As Int32,
                               ByVal Av_Cod As Int32,
                               ByVal grfi_cod As Integer,
                               ByVal strAvversita As String,
                               ByRef LimiteInterventi() As Integer,
                               ByRef UDM_COD_Limite() As Integer,
                               ByRef IntervalloTrattamenti_Min() As Integer,
                               ByRef IntervalloTrattamenti_Max() As Integer,
                               ByVal For_Veg_Av_Dos_Cod As String,
                               ByVal Data As Date,
                               ByVal FormulatiXAllegatiNormative_IDRiga As Integer,
                               ByVal TipoTestata As Integer, ByVal Gruppo_Dosaggi As String,
                               ByRef Num_Max_Interventi_Globali() As Integer,
                               ByRef Array_For_Veg_Av_Dos_Cod() As Integer, ByRef Array_Gruppo_Dosaggi() As Integer,
                               ByRef Dose_Max_Etichetta() As Decimal,
                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                              )

        Dim NomeRoutine As String = "DpiBIZ.Dose_Difesa_Verifica_R.Dose_Verifica()"

        Dim MessaggioErrore As String = ""

        Dim FlagConnessioneLocale As Boolean = False
        Dim xConnessione As DbConnection
        Dim xConnectionState As ConnectionState = ConnectionState.Closed

        Dim RisultatoFunzione As String = String.Empty

        '======

        Dim Udm_Cod_Etichetta As Long
        Dim Udm_Des_Etichetta As String
        Dim Coefficiente_Etichetta As Decimal
        Dim Qta_EtichettaMax As Decimal
        Dim Qta_EtichettaMin As Decimal

        Dim Qta_Etichetta_Max_Ha As Decimal
        Dim Qta_Etichetta_Max_Hl As Decimal
        Dim Qta_Etichetta_Min_Ha As Decimal
        Dim Qta_Etichetta_Min_Hl As Decimal

        Dim Qta_Utilizzata As Decimal
        Dim Udm_Cod_Utilizzato As Long
        Dim Udm_Des_Utilizzato As String

        Dim Dose_Max As Decimal
        Dim Dose_Min As Decimal
        Dim Mezzo_Etichetta As Integer
        Dim Mezzo_Lavorazione_Des As String

        Dim DoseMaxAnno As Decimal
        Dim DoseMaxAnno_UDM As Decimal
        Dim DoseMaxAnno_Veg_Cod As Decimal
        Dim DoseMaxAnno_UDM_Veg_Cod As Decimal


        Dim LimiteInterventi_d As Integer
        Dim UDM_COD_Des_d As String
        Dim UDM_COD_Limite_d As Integer
        Dim IntervalloTrattamenti_Min_d, IntervalloTrattamenti_Max_d As Integer
        Dim Acqua_Min, Acqua_Max As Decimal
        Dim Acqua_Udm As Integer
        Dim Num_Max_Interventi_Globali_d As Integer
        Dim Epoca_Cod As Integer

        Dim N_Err As Integer = 0
        Dim N_war As Integer = 0
        Dim VerificaDose As Boolean

        Try

            '------------------------------
            'Verifico se e' stata impostata una connessione
            If IsNothing(objParametri.objConnessione) Then
                'Flag
                FlagConnessioneLocale = True
                'Creo la connessione localmente
                xConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
            Else
                'Utilizzo quella passata come parametro
                xConnessione = objParametri.objConnessione
                xConnectionState = objParametri.objConnessione.State
            End If

            '------------------------------
            'Lettura della dose consigliata da etichetta
            Dim DtDose As DataTable

            Dim ObjDose As New AgronicaCoreDpiDAL.Fitofarmaci

            Select Case TipoTestata

                Case 1

                    'coadiuvanti,fisiofarmaci nel diserbo
                    If (Av_Cod = -1 And Av_Gru = -1) Or (Av_Cod = -2 And Av_Gru = -2) Then

                        ObjDose.AgroWS_InfoRead_Formulati_SpecieVegetali_Avversita_Dosi(
                               objParametri,
                               DtDose,
                               MessaggioErrore,
                               Fr_Cod,
                               Veg_Cod,
                               Av_Cod,
                               Av_Gru,
                               grfi_cod,
                               For_Veg_Av_Dos_Cod,
                               Data,
                               FormulatiXAllegatiNormative_IDRiga)
                    Else
                        'diserbanti, disseccanti
                        ObjDose.AgroWS_InfoRead_Formulati_SpecieVegetali_Infestanti_Dosi(
                                objParametri,
                                DtDose,
                                MessaggioErrore,
                                Fr_Cod,
                                Veg_Cod,
                                Av_Cod,
                                Av_Gru,
                                grfi_cod,
                                For_Veg_Av_Dos_Cod,
                                Data,
                                FormulatiXAllegatiNormative_IDRiga)
                    End If


                Case Else

                    ObjDose.AgroWS_InfoRead_Formulati_SpecieVegetali_Avversita_Dosi(
                                 objParametri,
                                 DtDose,
                                 MessaggioErrore,
                                 Fr_Cod,
                                 Veg_Cod,
                                 Av_Cod,
                                 Av_Gru,
                                 grfi_cod,
                                 For_Veg_Av_Dos_Cod,
                                 Data,
                                 FormulatiXAllegatiNormative_IDRiga)

            End Select


            Dim i As Integer

            Dim Verifica_Dosi_Gruppo As Boolean = False

            If DtDose.Rows.Count <> 0 Then

                Dim Hash_For_Veg_Av_Dos_Cod As New Hashtable

                'Verifico le n-dosi
                For i = 0 To DtDose.Rows.Count - 1

                    'evito le doppie dosi (dovute alla gerarchia dei gruppi avversità)
                    If Not Hash_For_Veg_Av_Dos_Cod.ContainsKey(CInt(DtDose.Rows(i).Item("For_Veg_Av_Dos_Cod"))) Then

                        VerificaDose = False

                        Hash_For_Veg_Av_Dos_Cod.Add(CInt(DtDose.Rows(i).Item("For_Veg_Av_Dos_Cod")), "")

                        If Not IsDBNull(DtDose.Rows(i).Item("gruppo_dosaggi")) AndAlso IsNumeric(DtDose.Rows(i).Item("gruppo_dosaggi")) AndAlso CInt(DtDose.Rows(i).Item("gruppo_dosaggi")) <> 0 Then
                            Gruppo_Dosaggi = CInt(DtDose.Rows(i).Item("gruppo_dosaggi"))
                            Verifica_Dosi_Gruppo = True
                        End If

                        'blocco fioritura
                        If IsNumeric(DtDose.Rows(i).Item("epoca_cod")) Then
                            Epoca_Cod = DtDose.Rows(i).Item("epoca_cod")
                        Else
                            Epoca_Cod = 0
                        End If



                        'Impostazione Dose Max da Etichetta
                        If IsNumeric(DtDose.Rows(i).Item("Dose_Max")) Then
                            Dose_Max = DtDose.Rows(i).Item("Dose_Max")
                        Else
                            'Eccezione
                            Dose_Max = 0
                        End If

                        'Impostazione Dose Max da Etichetta
                        If IsNumeric(DtDose.Rows(i).Item("Dose_Min")) Then
                            Dose_Min = DtDose.Rows(i).Item("Dose_Min")
                        Else
                            'Eccezione
                            Dose_Min = 0
                        End If

                        'Impostazione Udm Acqua
                        If IsNumeric(DtDose.Rows(i).Item("Acqua_UDM_COD")) Then
                            Acqua_Udm = DtDose.Rows(i).Item("Acqua_UDM_COD")
                        Else
                            Acqua_Udm = 0
                        End If

                        'Impostazione Dose Max Acqua
                        If IsNumeric(DtDose.Rows(i).Item("Acqua_max")) Then
                            Acqua_Max = DtDose.Rows(i).Item("Acqua_max")
                            'trasformo in HL
                            Select Case Acqua_Udm
                                Case 22, 29 'l/ha, l
                                    Acqua_Max = Acqua_Max / 100
                            End Select
                        Else
                            Acqua_Max = 0
                        End If

                        'Impostazione Dose Min Acqua
                        If IsNumeric(DtDose.Rows(i).Item("Acqua_min")) Then
                            Acqua_Min = DtDose.Rows(i).Item("Acqua_min")
                            'trasformo in HL
                            Select Case Acqua_Udm
                                Case 22, 29 'l/ha, l
                                    Acqua_Min = Acqua_Min / 100
                            End Select
                        Else
                            Acqua_Min = 0
                        End If

                        'LimiteInterventi
                        If IsNumeric(DtDose.Rows(i).Item("LimiteInterventi")) Then
                            LimiteInterventi_d = DtDose.Rows(i).Item("LimiteInterventi")
                        Else
                            LimiteInterventi_d = 0
                        End If
                        If IsNumeric(DtDose.Rows(i).Item("Num_Max_Interventi_Globali")) Then
                            Num_Max_Interventi_Globali_d = DtDose.Rows(i).Item("Num_Max_Interventi_Globali")
                        Else
                            Num_Max_Interventi_Globali_d = 0
                        End If

                        'Impostazione Udm Limite
                        If IsNumeric(DtDose.Rows(i).Item("UDM_COD_Limite")) Then
                            UDM_COD_Limite_d = DtDose.Rows(i).Item("UDM_COD_Limite")
                            Select Case UDM_COD_Limite_d
                                Case enum_UnitaMisura.Anno
                                    UDM_COD_Des_d = " all'anno "
                                Case enum_UnitaMisura.CicloColturale
                                    UDM_COD_Des_d = " per ciclo colturale "
                            End Select
                        Else
                            UDM_COD_Limite_d = 0
                        End If

                        If LimiteInterventi_d <> 0 Then
                            ReDim Preserve Warning_Code(N_war)
                            ReDim Preserve Warning_Des(N_war)
                            ReDim Preserve LimiteInterventi(N_war)
                            ReDim Preserve UDM_COD_Limite(N_war)
                            ReDim Preserve IntervalloTrattamenti_Max(N_war)
                            ReDim Preserve IntervalloTrattamenti_Min(N_war)
                            ReDim Preserve Num_Max_Interventi_Globali(N_war)
                            ReDim Preserve Array_For_Veg_Av_Dos_Cod(N_war)
                            ReDim Preserve Array_Gruppo_Dosaggi(N_war)
                            ReDim Preserve Dose_Max_Etichetta(N_war)

                            Warning_Code(N_war) = enTipoWarning_Verifica.Numero_Interventi_xProdotto
                            Warning_Des(N_war) = "Il numero di interventi effettuati con " & Fr_Des & " non può superare il numero massimo consentito da etichetta (" & LimiteInterventi_d & UDM_COD_Des_d & ")."
                            LimiteInterventi(N_war) = LimiteInterventi_d
                            UDM_COD_Limite(N_war) = UDM_COD_Limite_d
                            IntervalloTrattamenti_Max(N_war) = 0
                            IntervalloTrattamenti_Min(N_war) = 0
                            Num_Max_Interventi_Globali(N_war) = Num_Max_Interventi_Globali_d
                            Array_For_Veg_Av_Dos_Cod(N_war) = CInt(DtDose.Rows(i).Item("For_Veg_Av_Dos_Cod"))
                            Array_Gruppo_Dosaggi(N_war) = Gruppo_Dosaggi
                            Dose_Max_Etichetta(N_war) = 0
                            N_war += 1

                        End If



                        'Impostazione IntervalloTrattamenti_Min
                        If IsNumeric(DtDose.Rows(i).Item("IntervalloTrattamenti_Min")) Then
                            IntervalloTrattamenti_Min_d = DtDose.Rows(i).Item("IntervalloTrattamenti_Min")
                        Else
                            IntervalloTrattamenti_Min_d = 0
                        End If

                        'Impostazione IntervalloTrattamenti_Min
                        If IsNumeric(DtDose.Rows(i).Item("IntervalloTrattamenti_Max")) Then
                            IntervalloTrattamenti_Max_d = DtDose.Rows(i).Item("IntervalloTrattamenti_Max")
                        Else
                            IntervalloTrattamenti_Max_d = 0
                        End If

                        If Not (IntervalloTrattamenti_Min_d = 0 And IntervalloTrattamenti_Max_d = 0) Then
                            ReDim Preserve Warning_Code(N_war)
                            ReDim Preserve Warning_Des(N_war)
                            ReDim Preserve IntervalloTrattamenti_Max(N_war)
                            ReDim Preserve IntervalloTrattamenti_Min(N_war)
                            ReDim Preserve LimiteInterventi(N_war)
                            ReDim Preserve UDM_COD_Limite(N_war)
                            ReDim Preserve Num_Max_Interventi_Globali(N_war)
                            ReDim Preserve Array_For_Veg_Av_Dos_Cod(N_war)
                            ReDim Preserve Array_Gruppo_Dosaggi(N_war)
                            ReDim Preserve Dose_Max_Etichetta(N_war)

                            Warning_Code(N_war) = enTipoWarning_Verifica.Intervallo_Trattamenti_xProdotto
                            Warning_Des(N_war) = "L'intervallo di giorni tra gli interventi effettuati con " & Fr_Des & " va da " & IntervalloTrattamenti_Min_d & " a " & IntervalloTrattamenti_Max_d & "."
                            IntervalloTrattamenti_Max(N_war) = IntervalloTrattamenti_Max_d
                            IntervalloTrattamenti_Min(N_war) = IntervalloTrattamenti_Min_d
                            LimiteInterventi(N_war) = 0
                            UDM_COD_Limite(N_war) = 0
                            Num_Max_Interventi_Globali(N_war) = 0
                            Array_For_Veg_Av_Dos_Cod(N_war) = CInt(DtDose.Rows(i).Item("For_Veg_Av_Dos_Cod"))
                            Array_Gruppo_Dosaggi(N_war) = Gruppo_Dosaggi
                            Dose_Max_Etichetta(N_war) = 0
                            N_war += 1
                        End If





                        '##############################################################################################
                        '######## CALCOLO DELLA QUANTITA' TOTALE DISTRIBUIBILE DA ETICHETTA FORMULATO #################
                        '##############################################################################################

                        'Impostazione dell'unità di misura

                        'Utilizzo la seguente codifica per il mezzo_lavorazione
                        'Ettolitro = 0
                        'Ettaro = 1

                        Select Case Agro_SQL_SaveNum(DtDose.Rows(i).Item("Udm_Cod"))

                            Case 23 'g/hl
                                Udm_Cod_Etichetta = 2
                                Udm_Des_Etichetta = "grammi/Hl"
                                Coefficiente_Etichetta = 1 / 1000
                                Mezzo_Etichetta = 0 'Distribuzione per Ettolitro

                            Case 22 'l/ha
                                Udm_Cod_Etichetta = 29
                                Udm_Des_Etichetta = "Litri/Ha"
                                Coefficiente_Etichetta = 1
                                Mezzo_Etichetta = 1 'Distribuzione per Ettaro


                            Case 170 'ml/pianta
                                Udm_Cod_Etichetta = 29
                                Udm_Des_Etichetta = "ml/pianta"
                                Coefficiente_Etichetta = -1 'Indefinita
                                Mezzo_Etichetta = -1

                            Case 175 'Kg/hl
                                Udm_Cod_Etichetta = 2
                                Udm_Des_Etichetta = "Kg/Hl"
                                Coefficiente_Etichetta = 1
                                Mezzo_Etichetta = 0 'Distribuzione per Ettolitro

                            Case 21 'cc/hl
                                Udm_Cod_Etichetta = 29
                                Udm_Des_Etichetta = "cc/Hl"
                                Coefficiente_Etichetta = 1 / 1000
                                Mezzo_Etichetta = 0 'Distribuzione per Ettolitro

                            Case 20 'g/ha
                                Udm_Cod_Etichetta = 2
                                Udm_Des_Etichetta = "Grammi/Ha"
                                Coefficiente_Etichetta = 1 / 1000
                                Mezzo_Etichetta = 1 'Distribuzione per Ettaro

                            Case 302 'l/mq
                                Udm_Cod_Etichetta = 29
                                Udm_Des_Etichetta = "Litri/mq"
                                Coefficiente_Etichetta = 10000
                                Mezzo_Etichetta = 1 'Distribuzione per Ettaro


                            Case 173 'l/hl
                                Udm_Cod_Etichetta = 29
                                Udm_Des_Etichetta = "Litri/Hl"
                                Coefficiente_Etichetta = 1
                                Mezzo_Etichetta = 0 'Distribuzione per Ettolitro

                            Case 88 'Kg/ha
                                Udm_Cod_Etichetta = 2
                                Udm_Des_Etichetta = "Kg/Ha"
                                Coefficiente_Etichetta = 1
                                Mezzo_Etichetta = 1 'Distribuzione per Ettaro

                            Case 163 'ml/ha
                                Udm_Cod_Etichetta = 29
                                Udm_Des_Etichetta = "ml/Ha"
                                Coefficiente_Etichetta = 1 / 1000
                                Mezzo_Etichetta = 1 'Distribuzione per Ettaro

                            Case 171 'g/mq
                                Udm_Cod_Etichetta = 29
                                Udm_Des_Etichetta = "Grammi/mq"
                                Coefficiente_Etichetta = 10
                                Mezzo_Etichetta = 1 'Distribuzione per Ettaro

                            Case 164, 101 'ml/hl
                                Udm_Cod_Etichetta = 29
                                Udm_Des_Etichetta = "ml/Hl"
                                Coefficiente_Etichetta = 1 / 1000
                                Mezzo_Etichetta = 0 'Distribuzione per Ettolitro

                            Case 172 'ml/mq
                                Udm_Cod_Etichetta = 29
                                Udm_Des_Etichetta = "ml/mq"
                                Coefficiente_Etichetta = 10
                                Mezzo_Etichetta = 1 'Distribuzione per Ettaro

                            Case 2016 'ml/l
                                Udm_Cod_Etichetta = 29
                                Udm_Des_Etichetta = "ml/l"
                                Coefficiente_Etichetta = 1 / 1000 * 100
                                Mezzo_Etichetta = 0 'Distribuzione per Ettolitro

                            Case 2003 'g/l
                                Udm_Cod_Etichetta = 2
                                Udm_Des_Etichetta = "g/l"
                                Coefficiente_Etichetta = 1 / 1000 * 100
                                Mezzo_Etichetta = 0 'Distribuzione per Ettolitro

                            Case enum_UnitaMisura.Numero_Diffusori_HA 'n. diffusori/ha
                                Udm_Cod_Etichetta = enum_UnitaMisura.Numero_Diffusori
                                Udm_Des_Etichetta = "n. diffusori/ha"
                                Coefficiente_Etichetta = 1
                                Mezzo_Etichetta = 1 'Distribuzione per Ettaro

                            Case enum_UnitaMisura.UNITA__HA 'unita/ha
                                Udm_Cod_Etichetta = enum_UnitaMisura.UNITA
                                Udm_Des_Etichetta = "unita'/ha"
                                Coefficiente_Etichetta = 1
                                Mezzo_Etichetta = 1 'Distribuzione per Ettaro

                            Case Else
                                'Mappatura non esistente
                                Coefficiente_Etichetta = -1

                        End Select


                        '=========================================================================
                        'Verifica
                        '-------------------------------------------------------------------------
                        'Codifica Errori
                        '-1.  Errore durante l'esecuzione della routine
                        ' 0.  Dose Ok
                        ' 100. Dose Consigliata Non Disponibile
                        ' 101. Unità di Misura Non Compatibile
                        ' 102. Dose Eccessiva
                        ' 103. La qta di H2O non è corretta (=0; < Acqua_min;> Acqua_max).

                        'Calcolo dose totale da etichetta
                        Select Case Mezzo_Etichetta
                            Case 0 'Distribuzione per Hl
                                Qta_EtichettaMax = Dose_Max * H2O_Lavorazione * Coefficiente_Etichetta
                                Qta_EtichettaMin = Dose_Min * H2O_Lavorazione * Coefficiente_Etichetta
                            Case 1 'Distribuzione per Ha
                                Qta_EtichettaMax = Dose_Max * Superficie_Lavorazione * Coefficiente_Etichetta
                                Qta_EtichettaMin = Dose_Min * Superficie_Lavorazione * Coefficiente_Etichetta
                            Case Else 'Eccezione
                                Qta_EtichettaMax = 0
                                Qta_EtichettaMin = 0
                        End Select

                        'Determino la qta di formulato distribuito espresso in macro udm
                        Qta_Formulato(Qta_Utilizzata, Udm_Cod_Utilizzato, Udm_Des_Utilizzato, Mezzo_Lavorazione_Des, Mezzo_Lavorazione, Mezzo_Dettaglio_Lavorazione, Udm_Cod_Lavorazione, Dose_Lavorazione_Ha, Dose_Lavorazione_Hl, H2O_Lavorazione, Superficie_Lavorazione)

                        'Per le dosi doppie con lo stesso mezzo verifico SOLO le Massime 
                        'potrebbero esserci dosi diverse in base all'epoca (controllo al momento non gestito) 

                        Select Case Mezzo_Etichetta
                            Case 0 'Hl
                                If Qta_Etichetta_Max_Hl < Qta_EtichettaMax Then
                                    Qta_Etichetta_Max_Hl = Qta_EtichettaMax
                                    VerificaDose = True
                                End If
                            Case 1 'Ha
                                If Qta_Etichetta_Max_Ha < Qta_EtichettaMax Then
                                    Qta_Etichetta_Max_Ha = Qta_EtichettaMax
                                    VerificaDose = True
                                End If
                        End Select

                        Select Case Mezzo_Etichetta
                            Case 0 'Hl
                                If Qta_Etichetta_Min_Hl > Qta_EtichettaMin Then
                                    Qta_Etichetta_Min_Hl = Qta_EtichettaMin
                                    VerificaDose = True
                                End If
                            Case 1 'Ha
                                If Qta_Etichetta_Min_Ha > Qta_EtichettaMin Then
                                    Qta_Etichetta_Min_Ha = Qta_EtichettaMin
                                    VerificaDose = True
                                End If
                        End Select

                        If VerificaDose = True Then

                            If Udm_Cod_Utilizzato <> Udm_Cod_Etichetta Then
                                'Unità di Misura Non Compatibili
                                ReDim Preserve Err_Code(N_Err)
                                ReDim Preserve Err_Des(N_Err)
                                Err_Code(N_Err) = enTipoErrCode_Verifica.UnitaMisuraNonCompatibile
                                Err_Des(N_Err) = "L'unità di misura impiegata (" & Udm_Des_Utilizzato & Mezzo_Lavorazione_Des & ") del prodotto commerciale " & Fr_Des & " non è conforme all'etichetta (" & Udm_Des_Etichetta & ")."
                                N_Err += 1
                            End If

                            'Controllo le quantità
                            'Essendo Kg o Litri arrotondo al terzo decimale ...
                            If Math.Round(Qta_Utilizzata, 3) > Math.Round(Qta_EtichettaMax, 3) Then
                                'If Qta_Utilizzata > Qta_Etichetta Then
                                'Dose Eccessiva
                                ReDim Preserve Err_Code(N_Err)
                                ReDim Preserve Err_Des(N_Err)
                                Err_Code(N_Err) = enTipoErrCode_Verifica.DoseEccessiva
                                Err_Des(N_Err) = "La quantità impiegata di " & Fr_Des & " è eccessiva e non conforme all'etichetta del prodotto. " &
                                                     "La quantità distribuita è di " & Udm_Des_Utilizzato & " " & Math.Round(Qta_Utilizzata, 3) & " mentre quella distribuibile per l'intervento risulta essere non superiore a " & Udm_Des_Utilizzato & " " & Math.Round(Qta_EtichettaMax, 3) & "."
                                N_Err += 1
                            Else
                                ''OK
                                'Err_Code = 0
                                'Dose_Verifica = ""
                            End If

                            '(04/10/2017 fede) commento temporaneamente perchè l'agenda lo da sempre....
                            '(23/10/2019 fede) decommento perchè l'agenda ora è allineata
                            'se è indicata una dose mninima verifico che la qta non sia inferiore
                            If Qta_EtichettaMin <> 0 Then
                                If Math.Round(Qta_Utilizzata, 3) < Math.Round(Qta_EtichettaMin, 3) Then
                                    'If Qta_Utilizzata > Qta_Etichetta Then
                                    'Dose Eccessiva
                                    ReDim Preserve Err_Code(N_Err)
                                    ReDim Preserve Err_Des(N_Err)
                                    Err_Code(N_Err) = enTipoErrCode_Verifica.DoseInsufficiente
                                    Err_Des(N_Err) = "La quantità impiegata di " & Fr_Des & " è insufficiente e non conforme all'etichetta del prodotto. " &
                                                 "La quantità distribuita è di " & Udm_Des_Utilizzato & " " & Math.Round(Qta_Utilizzata, 3) & " mentre quella distribuibile per l'intervento risulta essere non inferiore a " & Udm_Des_Utilizzato & " " & Math.Round(Qta_EtichettaMin, 3) & "."
                                    N_Err += 1
                                Else
                                    ''OK
                                    'Err_Code = 0
                                    'Dose_Verifica = ""
                                End If
                            End If

                            Select Case Lav_Cod
                                Case LAVCOD_GEODISINFESTAZIONE, LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, LAVCOD_CONFUSIONE_DISORIENTAMENTO_SESSUALE, LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA, LAVCOD_REINNESCO_TRAPPOLE
                                Case Else
                                    If H2O_Lavorazione = 0 And H2O_Ha = 0 Then
                                        'La qta di H2O non è corretta
                                        ReDim Preserve Err_Code(N_Err)
                                        ReDim Preserve Err_Des(N_Err)
                                        Err_Code(N_Err) = enTipoErrCode_Verifica.AcquaNonCorretta
                                        Err_Des(N_Err) = "Quantità di H2O non è corretta."
                                        N_Err += 1
                                    End If
                            End Select

                            'controllo acqua
                            If H2O_Ha = 0 And H2O_Lavorazione <> 0 Then
                                H2O_Ha = H2O_Lavorazione / Superficie_Lavorazione
                            End If
                            If Acqua_Min <> 0 Then
                                If H2O_Ha < Acqua_Min Then
                                    ReDim Preserve Err_Code(N_Err)
                                    ReDim Preserve Err_Des(N_Err)
                                    Err_Code(N_Err) = enTipoErrCode_Verifica.AcquaNonCorretta
                                    Err_Des(N_Err) = "La quantità di acqua impiegata (" & Math.Round(H2O_Ha, 3) & " hl/ha) è inferiore a quella minima dell'etichetta del prodotto " & Fr_Des & " (" & Acqua_Min & " hl/ha)."
                                    N_Err += 1
                                End If
                            End If

                            'controllo acqua
                            If Acqua_Max <> 0 Then
                                If H2O_Ha > Acqua_Max Then
                                    ReDim Preserve Err_Code(N_Err)
                                    ReDim Preserve Err_Des(N_Err)
                                    Err_Code(N_Err) = enTipoErrCode_Verifica.AcquaNonCorretta
                                    Err_Des(N_Err) = "La quantità di acqua impiegata (" & Math.Round(H2O_Ha, 3) & " hl/ha) è superiore a quella massima dell'etichetta del prodotto " & Fr_Des & " (" & Acqua_Max & " hl/ha)."
                                    N_Err += 1
                                End If
                            End If

                            'controllo blocco fioritura
                            If Epoca_Cod <> 0 Then

                            End If

                        End If




                        '=============================================================================================================================
                        'Marco: Dose Max Annuale x Specie Vegetale
                        '-----------------------------------------------------------------------------------------------------------------------------
                        If IsNumeric(DtDose.Rows(i).Item("DoseMaxAnno_Veg_Cod")) And IsNumeric(DtDose.Rows(i).Item("DoseMaxAnno_UDM_Veg_Cod")) Then
                            DoseMaxAnno_Veg_Cod = DtDose.Rows(i).Item("DoseMaxAnno_Veg_Cod")
                            DoseMaxAnno_UDM_Veg_Cod = DtDose.Rows(i).Item("DoseMaxAnno_UDM_Veg_Cod")
                        Else
                            DoseMaxAnno_Veg_Cod = 0
                            DoseMaxAnno_UDM_Veg_Cod = 0
                        End If

                        If DoseMaxAnno_Veg_Cod <> 0 And DoseMaxAnno_UDM_Veg_Cod <> 0 Then

                            'Impostazione dell'unità di misura
                            Select Case DoseMaxAnno_UDM_Veg_Cod

                                Case 23 'g/hl
                                    Udm_Cod_Etichetta = 2
                                    Udm_Des_Etichetta = "grammi/Hl"
                                    Coefficiente_Etichetta = 1 / 1000
                                    Mezzo_Etichetta = 0 'Distribuzione per Ettolitro

                                Case 22 'l/ha
                                    Udm_Cod_Etichetta = 29
                                    Udm_Des_Etichetta = "Litri/Ha"
                                    Coefficiente_Etichetta = 1
                                    Mezzo_Etichetta = 1 'Distribuzione per Ettaro


                                Case 170 'ml/pianta
                                    Udm_Cod_Etichetta = 29
                                    Udm_Des_Etichetta = "ml/pianta"
                                    Coefficiente_Etichetta = -1 'Indefinita
                                    Mezzo_Etichetta = -1

                                Case 175 'Kg/hl
                                    Udm_Cod_Etichetta = 2
                                    Udm_Des_Etichetta = "Kg/Hl"
                                    Coefficiente_Etichetta = 1
                                    Mezzo_Etichetta = 0 'Distribuzione per Ettolitro

                                Case 21 'cc/hl
                                    Udm_Cod_Etichetta = 29
                                    Udm_Des_Etichetta = "cc/Hl"
                                    Coefficiente_Etichetta = 1 / 1000
                                    Mezzo_Etichetta = 0 'Distribuzione per Ettolitro

                                Case 20 'g/ha
                                    Udm_Cod_Etichetta = 2
                                    Udm_Des_Etichetta = "Grammi/Ha"
                                    Coefficiente_Etichetta = 1 / 1000
                                    Mezzo_Etichetta = 1 'Distribuzione per Ettaro

                                Case 302 'l/mq
                                    Udm_Cod_Etichetta = 29
                                    Udm_Des_Etichetta = "Litri/mq"
                                    Coefficiente_Etichetta = 10000
                                    Mezzo_Etichetta = 1 'Distribuzione per Ettaro


                                Case 173 'l/hl
                                    Udm_Cod_Etichetta = 29
                                    Udm_Des_Etichetta = "Litri/Hl"
                                    Coefficiente_Etichetta = 1
                                    Mezzo_Etichetta = 0 'Distribuzione per Ettolitro

                                Case 88 'Kg/ha
                                    Udm_Cod_Etichetta = 2
                                    Udm_Des_Etichetta = "Kg/Ha"
                                    Coefficiente_Etichetta = 1
                                    Mezzo_Etichetta = 1 'Distribuzione per Ettaro

                                Case 163 'ml/ha
                                    Udm_Cod_Etichetta = 29
                                    Udm_Des_Etichetta = "ml/Ha"
                                    Coefficiente_Etichetta = 1 / 1000
                                    Mezzo_Etichetta = 1 'Distribuzione per Ettaro

                                Case 171 'g/mq
                                    Udm_Cod_Etichetta = 29
                                    Udm_Des_Etichetta = "Grammi/mq"
                                    Coefficiente_Etichetta = 10
                                    Mezzo_Etichetta = 1 'Distribuzione per Ettaro

                                Case 164, 101 'ml/hl
                                    Udm_Cod_Etichetta = 29
                                    Udm_Des_Etichetta = "ml/Hl"
                                    Coefficiente_Etichetta = 1 / 1000
                                    Mezzo_Etichetta = 0 'Distribuzione per Ettolitro

                                Case 172 'ml/mq
                                    Udm_Cod_Etichetta = 29
                                    Udm_Des_Etichetta = "ml/mq"
                                    Coefficiente_Etichetta = 10
                                    Mezzo_Etichetta = 1 'Distribuzione per Ettaro

                                Case 2016 'ml/l
                                    Udm_Cod_Etichetta = 29
                                    Udm_Des_Etichetta = "ml/l"
                                    Coefficiente_Etichetta = 1 / 1000 * 100
                                    Mezzo_Etichetta = 0 'Distribuzione per Ettolitro

                                Case 2003 'g/l
                                    Udm_Cod_Etichetta = 2
                                    Udm_Des_Etichetta = "g/l"
                                    Coefficiente_Etichetta = 1 / 1000 * 100
                                    Mezzo_Etichetta = 0 'Distribuzione per Ettolitro

                                Case enum_UnitaMisura.Numero_Diffusori_HA 'n. diffusori/ha
                                    Udm_Cod_Etichetta = enum_UnitaMisura.Numero_Diffusori
                                    Udm_Des_Etichetta = "n. diffusori'/ha"
                                    Coefficiente_Etichetta = 1
                                    Mezzo_Etichetta = 1 'Distribuzione per Ettaro

                                Case enum_UnitaMisura.UNITA__HA 'unita/ha
                                    Udm_Cod_Etichetta = enum_UnitaMisura.UNITA
                                    Udm_Des_Etichetta = "unita'/ha"
                                    Coefficiente_Etichetta = 1
                                    Mezzo_Etichetta = 1 'Distribuzione per Ettaro

                                Case Else
                                    'Mappatura non esistente
                                    Coefficiente_Etichetta = -1

                            End Select




                            ReDim Preserve Warning_Code(N_war)
                            ReDim Preserve Warning_Des(N_war)
                            ReDim Preserve LimiteInterventi(N_war)
                            ReDim Preserve UDM_COD_Limite(N_war)
                            ReDim Preserve IntervalloTrattamenti_Max(N_war)
                            ReDim Preserve IntervalloTrattamenti_Min(N_war)
                            ReDim Preserve Num_Max_Interventi_Globali(N_war)
                            ReDim Preserve Array_For_Veg_Av_Dos_Cod(N_war)
                            ReDim Preserve Array_Gruppo_Dosaggi(N_war)
                            ReDim Preserve Dose_Max_Etichetta(N_war)

                            Warning_Code(N_war) = enTipoWarning_Verifica.Dose_MaxAnno
                            Warning_Des(N_war) = " La dose massima all'anno del prodotto " & Fr_Des & " non può superare il numero massimo consentito da etichetta (" & DoseMaxAnno_Veg_Cod & " " & Udm_Des_Etichetta & ")."
                            LimiteInterventi(N_war) = 0
                            UDM_COD_Limite(N_war) = DoseMaxAnno_UDM_Veg_Cod
                            IntervalloTrattamenti_Max(N_war) = 0
                            IntervalloTrattamenti_Min(N_war) = 0
                            Num_Max_Interventi_Globali(N_war) = 0
                            Array_For_Veg_Av_Dos_Cod(N_war) = 0
                            Array_Gruppo_Dosaggi(N_war) = 0
                            Dose_Max_Etichetta(N_war) = DoseMaxAnno_Veg_Cod
                            N_war += 1

                        End If


                        '=============================================================================================================================
                        'Marco: Dose Max Annuale x Avversità/Infestante
                        '-----------------------------------------------------------------------------------------------------------------------------

                        'DA TESTARE PER CASISTINCA MANCANTE DFA BANCHE DATI!!!

                        If IsNumeric(DtDose.Rows(i).Item("DoseMaxAnno")) And IsNumeric(DtDose.Rows(i).Item("DoseMaxAnno_UDM")) Then
                            DoseMaxAnno = DtDose.Rows(i).Item("DoseMaxAnno")
                            DoseMaxAnno_UDM = DtDose.Rows(i).Item("DoseMaxAnno_UDM")
                        Else
                            DoseMaxAnno = 0
                            DoseMaxAnno_UDM = 0
                        End If

                        If DoseMaxAnno <> 0 And DoseMaxAnno_UDM <> 0 Then

                            'Impostazione dell'udm
                            Select Case DoseMaxAnno_UDM

                                Case 23 'g/hl
                                    Udm_Cod_Etichetta = 2
                                    Udm_Des_Etichetta = "grammi/Hl"
                                    Coefficiente_Etichetta = 1 / 1000
                                    Mezzo_Etichetta = 0 'Distribuzione per Ettolitro

                                Case 22 'l/ha
                                    Udm_Cod_Etichetta = 29
                                    Udm_Des_Etichetta = "Litri/Ha"
                                    Coefficiente_Etichetta = 1
                                    Mezzo_Etichetta = 1 'Distribuzione per Ettaro


                                Case 170 'ml/pianta
                                    Udm_Cod_Etichetta = 29
                                    Udm_Des_Etichetta = "ml/pianta"
                                    Coefficiente_Etichetta = -1 'Indefinita
                                    Mezzo_Etichetta = -1

                                Case 175 'Kg/hl
                                    Udm_Cod_Etichetta = 2
                                    Udm_Des_Etichetta = "Kg/Hl"
                                    Coefficiente_Etichetta = 1
                                    Mezzo_Etichetta = 0 'Distribuzione per Ettolitro

                                Case 21 'cc/hl
                                    Udm_Cod_Etichetta = 29
                                    Udm_Des_Etichetta = "cc/Hl"
                                    Coefficiente_Etichetta = 1 / 1000
                                    Mezzo_Etichetta = 0 'Distribuzione per Ettolitro

                                Case 20 'g/ha
                                    Udm_Cod_Etichetta = 2
                                    Udm_Des_Etichetta = "Grammi/Ha"
                                    Coefficiente_Etichetta = 1 / 1000
                                    Mezzo_Etichetta = 1 'Distribuzione per Ettaro

                                Case 302 'l/mq
                                    Udm_Cod_Etichetta = 29
                                    Udm_Des_Etichetta = "Litri/mq"
                                    Coefficiente_Etichetta = 10000
                                    Mezzo_Etichetta = 1 'Distribuzione per Ettaro


                                Case 173 'l/hl
                                    Udm_Cod_Etichetta = 29
                                    Udm_Des_Etichetta = "Litri/Hl"
                                    Coefficiente_Etichetta = 1
                                    Mezzo_Etichetta = 0 'Distribuzione per Ettolitro

                                Case 88 'Kg/ha
                                    Udm_Cod_Etichetta = 2
                                    Udm_Des_Etichetta = "Kg/Ha"
                                    Coefficiente_Etichetta = 1
                                    Mezzo_Etichetta = 1 'Distribuzione per Ettaro

                                Case 163 'ml/ha
                                    Udm_Cod_Etichetta = 29
                                    Udm_Des_Etichetta = "ml/Ha"
                                    Coefficiente_Etichetta = 1 / 1000
                                    Mezzo_Etichetta = 1 'Distribuzione per Ettaro

                                Case 171 'g/mq
                                    Udm_Cod_Etichetta = 29
                                    Udm_Des_Etichetta = "Grammi/mq"
                                    Coefficiente_Etichetta = 10
                                    Mezzo_Etichetta = 1 'Distribuzione per Ettaro

                                Case 164, 101 'ml/hl
                                    Udm_Cod_Etichetta = 29
                                    Udm_Des_Etichetta = "ml/Hl"
                                    Coefficiente_Etichetta = 1 / 1000
                                    Mezzo_Etichetta = 0 'Distribuzione per Ettolitro

                                Case 172 'ml/mq
                                    Udm_Cod_Etichetta = 29
                                    Udm_Des_Etichetta = "ml/mq"
                                    Coefficiente_Etichetta = 10
                                    Mezzo_Etichetta = 1 'Distribuzione per Ettaro

                                Case 2016 'ml/l
                                    Udm_Cod_Etichetta = 29
                                    Udm_Des_Etichetta = "ml/l"
                                    Coefficiente_Etichetta = 1 / 1000 * 100
                                    Mezzo_Etichetta = 0 'Distribuzione per Ettolitro

                                Case 2003 'g/l
                                    Udm_Cod_Etichetta = 2
                                    Udm_Des_Etichetta = "g/l"
                                    Coefficiente_Etichetta = 1 / 1000 * 100
                                    Mezzo_Etichetta = 0 'Distribuzione per Ettolitro

                                Case enum_UnitaMisura.Numero_Diffusori_HA 'n. diffusori/ha
                                    Udm_Cod_Etichetta = enum_UnitaMisura.Numero_Diffusori
                                    Udm_Des_Etichetta = "n. diffusori'/ha"
                                    Coefficiente_Etichetta = 1
                                    Mezzo_Etichetta = 1 'Distribuzione per Ettaro

                                Case enum_UnitaMisura.UNITA__HA 'unita/ha
                                    Udm_Cod_Etichetta = enum_UnitaMisura.UNITA
                                    Udm_Des_Etichetta = "unita'/ha"
                                    Coefficiente_Etichetta = 1
                                    Mezzo_Etichetta = 1 'Distribuzione per Ettaro

                                Case Else
                                    'Mappatura non esistente
                                    Coefficiente_Etichetta = -1

                            End Select

                            ReDim Preserve Warning_Code(N_war)
                            ReDim Preserve Warning_Des(N_war)
                            ReDim Preserve LimiteInterventi(N_war)
                            ReDim Preserve UDM_COD_Limite(N_war)
                            ReDim Preserve IntervalloTrattamenti_Max(N_war)
                            ReDim Preserve IntervalloTrattamenti_Min(N_war)
                            ReDim Preserve Num_Max_Interventi_Globali(N_war)
                            ReDim Preserve Array_For_Veg_Av_Dos_Cod(N_war)
                            ReDim Preserve Array_Gruppo_Dosaggi(N_war)
                            ReDim Preserve Dose_Max_Etichetta(N_war)

                            Warning_Code(N_war) = enTipoWarning_Verifica.Dose_MaxAnno_xAvversita_Infestante
                            Warning_Des(N_war) = " La dose massima all'anno del prodotto " & Fr_Des & " non può superare il numero massimo consentito da etichetta (" & DoseMaxAnno & " " & Udm_Des_Etichetta & ")."
                            LimiteInterventi(N_war) = 0
                            UDM_COD_Limite(N_war) = DoseMaxAnno_UDM
                            IntervalloTrattamenti_Max(N_war) = 0
                            IntervalloTrattamenti_Min(N_war) = 0
                            Num_Max_Interventi_Globali(N_war) = 0
                            Array_For_Veg_Av_Dos_Cod(N_war) = CInt(DtDose.Rows(i).Item("For_Veg_Av_Dos_Cod"))
                            Array_Gruppo_Dosaggi(N_war) = 0
                            Dose_Max_Etichetta(N_war) = DoseMaxAnno
                            N_war += 1

                        End If

                    End If

                Next

            Else

                Coefficiente_Etichetta = -1 'Dose etichetta non disponibile

                'Mappatura non possbile
                ReDim Preserve Err_Code(N_Err)
                ReDim Preserve Err_Des(N_Err)
                Err_Code(N_Err) = enTipoErrCode_Verifica.DoseNonDisponibile
                'Err_Des(N_Err) = "Dose consigliata di " & Fr_Des & " non disponibile"
                Err_Des(N_Err) = "Attenzione! Il prodotto " & Fr_Des & " è stato revisionato. E' consigliato verificare l'intervento con la nuova etichetta."
                N_Err += 1

            End If

            ''(06/08/2018 fede) se il dosaggio appartiene ad un gruppo

            'If Verifica_Dosi_Gruppo = True Then

            '    Select Case TipoTestata

            '        Case 1

            '            'coadiuvanti,fisiofarmaci nel diserbo
            '            If (Av_Cod = -1 And Av_Gru = -1) Or (Av_Cod = -2 And Av_Gru = -2) Then

            '                ObjDose.AgroWS_InfoRead_Formulati_SpecieVegetali_Avversita_Dosi(
            '                       objParametri,
            '                       DtDose,
            '                       MessaggioErrore,
            '                       Fr_Cod,
            '                       Veg_Cod,
            '                       Av_Cod,
            '                       Av_Gru,
            '                       grfi_cod,
            '                       "0",
            '                       Data,
            '                       FormulatiXAllegatiNormative_IDRiga, Gruppo_Dosaggi:=Gruppo_Dosaggi)
            '            Else
            '                'diserbanti, disseccanti
            '                ObjDose.AgroWS_InfoRead_Formulati_SpecieVegetali_Infestanti_Dosi(
            '                        objParametri,
            '                        DtDose,
            '                        MessaggioErrore,
            '                        Fr_Cod,
            '                        Veg_Cod,
            '                        Av_Cod,
            '                        Av_Gru,
            '                        grfi_cod,
            '                        "0",
            '                        Data,
            '                        FormulatiXAllegatiNormative_IDRiga, Gruppo_Dosaggi:=Gruppo_Dosaggi)
            '            End If


            '        Case Else

            '            ObjDose.AgroWS_InfoRead_Formulati_SpecieVegetali_Avversita_Dosi(
            '                         objParametri,
            '                         DtDose,
            '                         MessaggioErrore,
            '                         Fr_Cod,
            '                         Veg_Cod,
            '                         Av_Cod,
            '                         Av_Gru,
            '                         grfi_cod,
            '                         "0",
            '                         Data,
            '                         FormulatiXAllegatiNormative_IDRiga, Gruppo_Dosaggi:=Gruppo_Dosaggi)

            '    End Select


            '    If DtDose.Rows.Count <> 0 Then

            '        Dim Hash_For_Veg_Av_Dos_Cod As New Hashtable

            '        'Verifico le n-dosi
            '        For i = 0 To DtDose.Rows.Count - 1

            '            'escludo quella controllata sopra
            '            If For_Veg_Av_Dos_Cod <> "" AndAlso For_Veg_Av_Dos_Cod <> "0" AndAlso For_Veg_Av_Dos_Cod <> DtDose.Rows(i).Item("For_Veg_Av_Dos_Cod") Then

            '                'evito le doppie dosi (dovute alla gerarchia dei gruppi avversità)
            '                If Not Hash_For_Veg_Av_Dos_Cod.ContainsKey(CInt(DtDose.Rows(i).Item("For_Veg_Av_Dos_Cod"))) Then

            '                    VerificaDose = False

            '                    Hash_For_Veg_Av_Dos_Cod.Add(CInt(DtDose.Rows(i).Item("For_Veg_Av_Dos_Cod")), "")

            '                    If Not IsDBNull(DtDose.Rows(i).Item("gruppo_dosaggi")) AndAlso IsNumeric(DtDose.Rows(i).Item("gruppo_dosaggi")) AndAlso CInt(DtDose.Rows(i).Item("gruppo_dosaggi")) <> 0 Then
            '                        Gruppo_Dosaggi = CInt(DtDose.Rows(i).Item("gruppo_dosaggi"))
            '                        Verifica_Dosi_Gruppo = True
            '                    End If

            '                    'blocco fioritura
            '                    If IsNumeric(DtDose.Rows(i).Item("epoca_cod")) Then
            '                        Epoca_Cod = DtDose.Rows(i).Item("epoca_cod")
            '                    Else
            '                        Epoca_Cod = 0
            '                    End If



            '                    'Impostazione Dose Max da Etichetta
            '                    If IsNumeric(DtDose.Rows(i).Item("Dose_Max")) Then
            '                        Dose_Max = DtDose.Rows(i).Item("Dose_Max")
            '                    Else
            '                        'Eccezione
            '                        Dose_Max = 0
            '                    End If

            '                    'Impostazione Dose Max da Etichetta
            '                    If IsNumeric(DtDose.Rows(i).Item("Dose_Min")) Then
            '                        Dose_Min = DtDose.Rows(i).Item("Dose_Min")
            '                    Else
            '                        'Eccezione
            '                        Dose_Min = 0
            '                    End If

            '                    'Impostazione Udm Acqua
            '                    If IsNumeric(DtDose.Rows(i).Item("Acqua_UDM_COD")) Then
            '                        Acqua_Udm = DtDose.Rows(i).Item("Acqua_UDM_COD")
            '                    Else
            '                        Acqua_Udm = 0
            '                    End If

            '                    'Impostazione Dose Max Acqua
            '                    If IsNumeric(DtDose.Rows(i).Item("Acqua_max")) Then
            '                        Acqua_Max = DtDose.Rows(i).Item("Acqua_max")
            '                        'trasformo in HL
            '                        Select Case Acqua_Udm
            '                            Case 22, 29 'l/ha, l
            '                                Acqua_Max = Acqua_Max / 100
            '                        End Select
            '                    Else
            '                        Acqua_Max = 0
            '                    End If

            '                    'Impostazione Dose Min Acqua
            '                    If IsNumeric(DtDose.Rows(i).Item("Acqua_min")) Then
            '                        Acqua_Min = DtDose.Rows(i).Item("Acqua_min")
            '                        'trasformo in HL
            '                        Select Case Acqua_Udm
            '                            Case 22, 29 'l/ha, l
            '                                Acqua_Min = Acqua_Min / 100
            '                        End Select
            '                    Else
            '                        Acqua_Min = 0
            '                    End If

            '                    'LimiteInterventi
            '                    If IsNumeric(DtDose.Rows(i).Item("LimiteInterventi")) Then
            '                        LimiteInterventi_d = DtDose.Rows(i).Item("LimiteInterventi")
            '                    Else
            '                        LimiteInterventi_d = 0
            '                    End If
            '                    If IsNumeric(DtDose.Rows(i).Item("Num_Max_Interventi_Globali")) Then
            '                        Num_Max_Interventi_Globali_d = DtDose.Rows(i).Item("Num_Max_Interventi_Globali")
            '                    Else
            '                        Num_Max_Interventi_Globali_d = 0
            '                    End If

            '                    'Impostazione Udm Limite
            '                    If IsNumeric(DtDose.Rows(i).Item("UDM_COD_Limite")) Then
            '                        UDM_COD_Limite_d = DtDose.Rows(i).Item("UDM_COD_Limite")
            '                        Select Case UDM_COD_Limite_d
            '                            Case enum_UnitaMisura.Anno
            '                                UDM_COD_Des_d = " all'anno "
            '                            Case enum_UnitaMisura.CicloColturale
            '                                UDM_COD_Des_d = " per ciclo colturale "
            '                        End Select
            '                    Else
            '                        UDM_COD_Limite_d = 0
            '                    End If

            '                    If LimiteInterventi_d <> 0 Then
            '                        ReDim Preserve Warning_Code(N_war)
            '                        ReDim Preserve Warning_Des(N_war)
            '                        ReDim Preserve LimiteInterventi(N_war)
            '                        ReDim Preserve UDM_COD_Limite(N_war)
            '                        ReDim Preserve IntervalloTrattamenti_Max(N_war)
            '                        ReDim Preserve IntervalloTrattamenti_Min(N_war)
            '                        ReDim Preserve Num_Max_Interventi_Globali(N_war)
            '                        ReDim Preserve Array_For_Veg_Av_Dos_Cod(N_war)
            '                        ReDim Preserve Array_Gruppo_Dosaggi(N_war)

            '                        Warning_Code(N_war) = enTipoWarning_Verifica.Numero_Interventi_xProdotto
            '                        Warning_Des(N_war) = "Il numero di interventi effettuati con " & Fr_Des & " non può superare il numero massimo consentito da etichetta (" & LimiteInterventi_d & UDM_COD_Des_d & ")."
            '                        LimiteInterventi(N_war) = LimiteInterventi_d
            '                        UDM_COD_Limite(N_war) = UDM_COD_Limite_d
            '                        IntervalloTrattamenti_Max(N_war) = 0
            '                        IntervalloTrattamenti_Min(N_war) = 0
            '                        Num_Max_Interventi_Globali(N_war) = Num_Max_Interventi_Globali_d
            '                        Array_For_Veg_Av_Dos_Cod(N_war) = CInt(DtDose.Rows(i).Item("For_Veg_Av_Dos_Cod"))
            '                        Array_Gruppo_Dosaggi(N_war) = Gruppo_Dosaggi
            '                        N_war += 1
            '                    End If



            '                    'Impostazione IntervalloTrattamenti_Min
            '                    If IsNumeric(DtDose.Rows(i).Item("IntervalloTrattamenti_Min")) Then
            '                        IntervalloTrattamenti_Min_d = DtDose.Rows(i).Item("IntervalloTrattamenti_Min")
            '                    Else
            '                        IntervalloTrattamenti_Min_d = 0
            '                    End If

            '                    'Impostazione IntervalloTrattamenti_Min
            '                    If IsNumeric(DtDose.Rows(i).Item("IntervalloTrattamenti_Max")) Then
            '                        IntervalloTrattamenti_Max_d = DtDose.Rows(i).Item("IntervalloTrattamenti_Max")
            '                    Else
            '                        IntervalloTrattamenti_Max_d = 0
            '                    End If

            '                    If Not (IntervalloTrattamenti_Min_d = 0 And IntervalloTrattamenti_Max_d = 0) Then
            '                        ReDim Preserve Warning_Code(N_war)
            '                        ReDim Preserve Warning_Des(N_war)
            '                        ReDim Preserve IntervalloTrattamenti_Max(N_war)
            '                        ReDim Preserve IntervalloTrattamenti_Min(N_war)
            '                        ReDim Preserve LimiteInterventi(N_war)
            '                        ReDim Preserve UDM_COD_Limite(N_war)
            '                        ReDim Preserve Num_Max_Interventi_Globali(N_war)
            '                        ReDim Preserve Array_For_Veg_Av_Dos_Cod(N_war)
            '                        ReDim Preserve Array_Gruppo_Dosaggi(N_war)

            '                        Warning_Code(N_war) = enTipoWarning_Verifica.Intervallo_Trattamenti_xProdotto
            '                        Warning_Des(N_war) = "L'intervallo di giorni tra gli interventi effettuati con " & Fr_Des & " va da " & IntervalloTrattamenti_Min_d & " a " & IntervalloTrattamenti_Max_d & "."
            '                        IntervalloTrattamenti_Max(N_war) = IntervalloTrattamenti_Max_d
            '                        IntervalloTrattamenti_Min(N_war) = IntervalloTrattamenti_Min_d
            '                        LimiteInterventi(N_war) = 0
            '                        UDM_COD_Limite(N_war) = 0
            '                        Num_Max_Interventi_Globali(N_war) = Num_Max_Interventi_Globali_d
            '                        Array_For_Veg_Av_Dos_Cod(N_war) = CInt(DtDose.Rows(i).Item("For_Veg_Av_Dos_Cod"))
            '                        Array_Gruppo_Dosaggi(N_war) = Gruppo_Dosaggi
            '                        N_war += 1
            '                    End If



            '                    '##############################################################################################
            '                    '######## CALCOLO DELLA QUANTITA' TOTALE DISTRIBUIBILE DA ETICHETTA FORMULATO #################
            '                    '##############################################################################################

            '                    'Impostazione dell'unità di misura

            '                    'Utilizzo la seguente codifica per il mezzo_lavorazione
            '                    'Ettolitro = 0
            '                    'Ettaro = 1

            '                    Select Case Agro_SQL_SaveNum(DtDose.Rows(i).Item("Udm_Cod"))

            '                        Case 23 'g/hl
            '                            Udm_Cod_Etichetta = 2
            '                            Udm_Des_Etichetta = "grammi/Hl"
            '                            Coefficiente_Etichetta = 1 / 1000
            '                            Mezzo_Etichetta = 0 'Distribuzione per Ettolitro

            '                        Case 22 'l/ha
            '                            Udm_Cod_Etichetta = 29
            '                            Udm_Des_Etichetta = "Litri/Ha"
            '                            Coefficiente_Etichetta = 1
            '                            Mezzo_Etichetta = 1 'Distribuzione per Ettaro


            '                        Case 170 'ml/pianta
            '                            Udm_Cod_Etichetta = 29
            '                            Udm_Des_Etichetta = "ml/pianta"
            '                            Coefficiente_Etichetta = -1 'Indefinita
            '                            Mezzo_Etichetta = -1

            '                        Case 175 'Kg/hl
            '                            Udm_Cod_Etichetta = 2
            '                            Udm_Des_Etichetta = "Kg/Hl"
            '                            Coefficiente_Etichetta = 1
            '                            Mezzo_Etichetta = 0 'Distribuzione per Ettolitro

            '                        Case 21 'cc/hl
            '                            Udm_Cod_Etichetta = 29
            '                            Udm_Des_Etichetta = "cc/Hl"
            '                            Coefficiente_Etichetta = 1 / 1000
            '                            Mezzo_Etichetta = 0 'Distribuzione per Ettolitro

            '                        Case 20 'g/ha
            '                            Udm_Cod_Etichetta = 2
            '                            Udm_Des_Etichetta = "Grammi/Ha"
            '                            Coefficiente_Etichetta = 1 / 1000
            '                            Mezzo_Etichetta = 1 'Distribuzione per Ettaro

            '                        Case 302 'l/mq
            '                            Udm_Cod_Etichetta = 29
            '                            Udm_Des_Etichetta = "Litri/mq"
            '                            Coefficiente_Etichetta = 10000
            '                            Mezzo_Etichetta = 1 'Distribuzione per Ettaro


            '                        Case 173 'l/hl
            '                            Udm_Cod_Etichetta = 29
            '                            Udm_Des_Etichetta = "Litri/Hl"
            '                            Coefficiente_Etichetta = 1
            '                            Mezzo_Etichetta = 0 'Distribuzione per Ettolitro

            '                        Case 88 'Kg/ha
            '                            Udm_Cod_Etichetta = 2
            '                            Udm_Des_Etichetta = "Kg/Ha"
            '                            Coefficiente_Etichetta = 1
            '                            Mezzo_Etichetta = 1 'Distribuzione per Ettaro

            '                        Case 163 'ml/ha
            '                            Udm_Cod_Etichetta = 29
            '                            Udm_Des_Etichetta = "ml/Ha"
            '                            Coefficiente_Etichetta = 1 / 1000
            '                            Mezzo_Etichetta = 1 'Distribuzione per Ettaro

            '                        Case 171 'g/mq
            '                            Udm_Cod_Etichetta = 29
            '                            Udm_Des_Etichetta = "Grammi/mq"
            '                            Coefficiente_Etichetta = 10
            '                            Mezzo_Etichetta = 1 'Distribuzione per Ettaro

            '                        Case 164, 101 'ml/hl
            '                            Udm_Cod_Etichetta = 29
            '                            Udm_Des_Etichetta = "ml/Hl"
            '                            Coefficiente_Etichetta = 1 / 1000
            '                            Mezzo_Etichetta = 0 'Distribuzione per Ettolitro

            '                        Case 172 'ml/mq
            '                            Udm_Cod_Etichetta = 29
            '                            Udm_Des_Etichetta = "ml/mq"
            '                            Coefficiente_Etichetta = 10
            '                            Mezzo_Etichetta = 1 'Distribuzione per Ettaro

            '                        Case Else
            '                            'Mappatura non esistente
            '                            Coefficiente_Etichetta = -1

            '                    End Select


            '                    '=========================================================================
            '                    'Verifica
            '                    '-------------------------------------------------------------------------
            '                    'Codifica Errori
            '                    '-1.  Errore durante l'esecuzione della routine
            '                    ' 0.  Dose Ok
            '                    ' 100. Dose Consigliata Non Disponibile
            '                    ' 101. Unità di Misura Non Compatibile
            '                    ' 102. Dose Eccessiva
            '                    ' 103. La qta di H2O non è corretta (=0; < Acqua_min;> Acqua_max).

            '                    'Calcolo dose totale da etichetta
            '                    Select Case Mezzo_Etichetta
            '                        Case 0 'Distribuzione per Hl
            '                            Qta_EtichettaMax = Dose_Max * H2O_Lavorazione * Coefficiente_Etichetta
            '                            Qta_EtichettaMin = Dose_Min * H2O_Lavorazione * Coefficiente_Etichetta
            '                        Case 1 'Distribuzione per Ha
            '                            Qta_EtichettaMax = Dose_Max * Superficie_Lavorazione * Coefficiente_Etichetta
            '                            Qta_EtichettaMin = Dose_Min * Superficie_Lavorazione * Coefficiente_Etichetta
            '                        Case Else 'Eccezione
            '                            Qta_EtichettaMax = 0
            '                            Qta_EtichettaMin = 0
            '                    End Select

            '                    'Determino la qta di formulato distribuito espresso in macro udm
            '                    Qta_Formulato(Qta_Utilizzata, Udm_Cod_Utilizzato, Udm_Des_Utilizzato, Mezzo_Lavorazione_Des, Mezzo_Lavorazione, Udm_Cod_Lavorazione, Dose_Lavorazione, H2O_Lavorazione, Superficie_Lavorazione)

            '                    'Per le dosi doppie con lo stesso mezzo verifico SOLO le Massime 
            '                    'potrebbero esserci dosi diverse in base all'epoca (controllo al momento non gestito) 

            '                    Select Case Mezzo_Etichetta
            '                        Case 0 'Hl
            '                            If Qta_Etichetta_Max_Hl < Qta_EtichettaMax Then
            '                                Qta_Etichetta_Max_Hl = Qta_EtichettaMax
            '                                VerificaDose = True
            '                            End If
            '                        Case 1 'Ha
            '                            If Qta_Etichetta_Max_Ha < Qta_EtichettaMax Then
            '                                Qta_Etichetta_Max_Ha = Qta_EtichettaMax
            '                                VerificaDose = True
            '                            End If
            '                    End Select

            '                    Select Case Mezzo_Etichetta
            '                        Case 0 'Hl
            '                            If Qta_Etichetta_Min_Hl > Qta_EtichettaMin Then
            '                                Qta_Etichetta_Min_Hl = Qta_EtichettaMin
            '                                VerificaDose = True
            '                            End If
            '                        Case 1 'Ha
            '                            If Qta_Etichetta_Min_Ha > Qta_EtichettaMin Then
            '                                Qta_Etichetta_Min_Ha = Qta_EtichettaMin
            '                                VerificaDose = True
            '                            End If
            '                    End Select

            '                    If VerificaDose = True Then

            '                        If Udm_Cod_Utilizzato <> Udm_Cod_Etichetta Then
            '                            'Unità di Misura Non Compatibili
            '                            ReDim Preserve Err_Code(N_Err)
            '                            ReDim Preserve Err_Des(N_Err)
            '                            Err_Code(N_Err) = enTipoErrCode_Verifica.UnitaMisuraNonCompatibile
            '                            Err_Des(N_Err) = "L'unità di misura impiegata (" & Udm_Des_Utilizzato & Mezzo_Lavorazione_Des & ") del prodotto commerciale " & Fr_Des & " non è conforme all'etichetta (" & Udm_Des_Etichetta & ")."
            '                            N_Err += 1
            '                        End If

            '                        'Controllo le quantità
            '                        'Essendo Kg o Litri arrotondo al terzo decimale ...
            '                        If Math.Round(Qta_Utilizzata, 3) > Math.Round(Qta_EtichettaMax, 3) Then
            '                            'If Qta_Utilizzata > Qta_Etichetta Then
            '                            'Dose Eccessiva
            '                            ReDim Preserve Err_Code(N_Err)
            '                            ReDim Preserve Err_Des(N_Err)
            '                            Err_Code(N_Err) = enTipoErrCode_Verifica.DoseEccessiva
            '                            Err_Des(N_Err) = "La quantità impiegata di " & Fr_Des & " è eccessiva e non conforme all'etichetta del prodotto. " &
            '                                             "La quantità distribuita è di " & Udm_Des_Utilizzato & " " & Math.Round(Qta_Utilizzata, 3) & " mentre quella distribuibile per l'intervento risulta essere non superiore a " & Udm_Des_Utilizzato & " " & Math.Round(Qta_EtichettaMax, 3) & "."
            '                            N_Err += 1
            '                        Else
            '                            ''OK
            '                            'Err_Code = 0
            '                            'Dose_Verifica = ""
            '                        End If

            '                        '(04/10/2017 fede) commento temporaneamente perchè l'agenda lo da sempre....
            '                        'se è indicata una dose mninima verifico che la qta non sia inferiore
            '                        'If Qta_EtichettaMin <> 0 Then
            '                        '    If Math.Round(Qta_Utilizzata, 3) < Math.Round(Qta_EtichettaMin, 3) Then
            '                        '        'If Qta_Utilizzata > Qta_Etichetta Then
            '                        '        'Dose Eccessiva
            '                        '        ReDim Preserve Err_Code(N_Err)
            '                        '        ReDim Preserve Err_Des(N_Err)
            '                        '        Err_Code(N_Err) = enTipoErrCode_Verifica.DoseInsufficiente
            '                        '        Err_Des(N_Err) = "La quantità impiegata di " & Fr_Des & " è insufficiente e non conforme all'etichetta del prodotto. " &
            '                        '                     "La quantità distribuita è di " & Udm_Des_Utilizzato & " " & Math.Round(Qta_Utilizzata, 3) & " mentre quella distribuibile per l'intervento risulta essere non inferiore a " & Udm_Des_Utilizzato & " " & Math.Round(Qta_EtichettaMin, 3) & "."
            '                        '        N_Err += 1
            '                        '    Else
            '                        '        ''OK
            '                        '        'Err_Code = 0
            '                        '        'Dose_Verifica = ""
            '                        '    End If
            '                        'End If

            '                        Select Case Lav_Cod
            '                            Case LAVCOD_GEODISINFESTAZIONE, LAVCOD_CONCIA_SEME, LAVCOD_TRATTAMENTO_ANTIPARASSITARIO
            '                            Case Else
            '                                If H2O_Lavorazione = 0 And H2O_Ha = 0 Then
            '                                    'La qta di H2O non è corretta
            '                                    ReDim Preserve Err_Code(N_Err)
            '                                    ReDim Preserve Err_Des(N_Err)
            '                                    Err_Code(N_Err) = enTipoErrCode_Verifica.AcquaNonCorretta
            '                                    Err_Des(N_Err) = "Quantità di H2O non è corretta."
            '                                    N_Err += 1
            '                                End If
            '                        End Select

            '                        'controllo acqua
            '                        If H2O_Ha = 0 And H2O_Lavorazione <> 0 Then
            '                            H2O_Ha = H2O_Lavorazione / Superficie_Lavorazione
            '                        End If
            '                        If Acqua_Min <> 0 Then
            '                            If H2O_Ha < Acqua_Min Then
            '                                ReDim Preserve Err_Code(N_Err)
            '                                ReDim Preserve Err_Des(N_Err)
            '                                Err_Code(N_Err) = enTipoErrCode_Verifica.AcquaNonCorretta
            '                                Err_Des(N_Err) = "La quantità di acqua impiegata (" & Math.Round(H2O_Ha, 3) & " hl/ha) è inferiore a quella minima dell'etichetta del prodotto " & Fr_Des & " (" & Acqua_Min & " hl/ha)."
            '                                N_Err += 1
            '                            End If
            '                        End If

            '                        'controllo acqua
            '                        If Acqua_Max <> 0 Then
            '                            If H2O_Ha > Acqua_Max Then
            '                                ReDim Preserve Err_Code(N_Err)
            '                                ReDim Preserve Err_Des(N_Err)
            '                                Err_Code(N_Err) = enTipoErrCode_Verifica.AcquaNonCorretta
            '                                Err_Des(N_Err) = "La quantità di acqua impiegata (" & Math.Round(H2O_Ha, 3) & " hl/ha) è superiore a quella massima dell'etichetta del prodotto " & Fr_Des & " (" & Acqua_Max & " hl/ha)."
            '                                N_Err += 1
            '                            End If
            '                        End If

            '                        'controllo blocco fioritura
            '                        If Epoca_Cod <> 0 Then

            '                        End If

            '                    End If

            '                End If

            '            End If

            '        Next

            '    Else

            '        Coefficiente_Etichetta = -1 'Dose etichetta non disponibile

            '        'Mappatura non possbile
            '        ReDim Preserve Err_Code(N_Err)
            '        ReDim Preserve Err_Des(N_Err)
            '        Err_Code(N_Err) = 100
            '        'Err_Des(N_Err) = "Dose consigliata di " & Fr_Des & " non disponibile"
            '        Err_Des(N_Err) = "Attenzione! Il prodotto " & Fr_Des & " è stato revisionato. E' consigliato verificare l'intervento con la nuova etichetta."
            '        N_Err += 1

            '    End If

            'End If



            '=========================================================================

            'Distruggo gli Oggetti
            DtDose = Nothing
            ObjDose = Nothing


        Catch ex As Exception

            RisultatoFunzione = ""
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

            'Err_Code = -1
            'Dose_Verifica = ""

        Finally

            If FlagConnessioneLocale = True Then
                If Not IsNothing(xConnessione) Then
                    xConnessione.Close()
                    xConnessione.Dispose()
                End If
            Else
                If xConnectionState = ConnectionState.Closed Then
                    xConnessione.Close()
                End If
            End If

        End Try


    End Sub

    ''#############################################################################################################
    ''#############################################################################################################
    ''#############################################################################################################

    Public Sub Qta_Formulato(ByRef Qta_Distribuita As Decimal,
                             ByRef Udm_Cod_Utilizzato As Int32,
                             ByRef Udm_Des_Utilizzato As String,
                             ByRef Mezzo_Lavorazione_Des As String,
                             ByVal Mezzo_Lavorazione As Int32,
                             ByVal Mezzo_Dettaglio_Lavorazione As Int32,
                             ByVal Udm_Cod_Lavorazione As Int32,
                             ByVal Dose_Lavorazione_Ha As Decimal,
                             ByVal Dose_Lavorazione_Hl As Decimal,
                             ByVal H2O_Lavorazione As Decimal,
                             ByVal Superficie_Lavorazione As Decimal)



        Dim Coefficiente_Utilizzato As Decimal


        Try

            '------------------------------

            'Conversione Unità di misura in Macro_Udm
            Select Case Udm_Cod_Lavorazione

                Case enum_UnitaMisura.Litri
                    Udm_Cod_Utilizzato = 29
                    Udm_Des_Utilizzato = "Litri"
                    Coefficiente_Utilizzato = 1

                Case enum_UnitaMisura.CentimetriCubi
                    Udm_Cod_Utilizzato = 29
                    Udm_Des_Utilizzato = "Litri"
                    Coefficiente_Utilizzato = 1 / 1000

                Case enum_UnitaMisura.Millilitri
                    Udm_Cod_Utilizzato = 29
                    Udm_Des_Utilizzato = "Litri"
                    Coefficiente_Utilizzato = 1 / 1000

                Case enum_UnitaMisura.KG
                    Udm_Cod_Utilizzato = 2
                    Udm_Des_Utilizzato = "Kg"
                    Coefficiente_Utilizzato = 1

                Case enum_UnitaMisura.Grammi
                    Udm_Cod_Utilizzato = 2
                    Udm_Des_Utilizzato = "Kg"
                    Coefficiente_Utilizzato = 1 / 1000

                Case enum_UnitaMisura.Quintali
                    Udm_Cod_Utilizzato = 2
                    Udm_Des_Utilizzato = "Quintali"
                    Coefficiente_Utilizzato = 100

                Case enum_UnitaMisura.Tonnellate
                    Udm_Cod_Utilizzato = 2 'Kilogrammi
                    Udm_Des_Utilizzato = "Tonnellate"
                    Coefficiente_Utilizzato = 1000

                Case enum_UnitaMisura.Numero_Diffusori
                    Udm_Cod_Utilizzato = enum_UnitaMisura.Numero_Diffusori
                    Udm_Des_Utilizzato = "n. diffusori"
                    Coefficiente_Utilizzato = 1

                Case enum_UnitaMisura.UNITA
                    Udm_Cod_Utilizzato = enum_UnitaMisura.UNITA
                    Udm_Des_Utilizzato = "unita'"
                    Coefficiente_Utilizzato = 1

                Case Else
                    Udm_Cod_Utilizzato = -1 'Indefinita
                    Coefficiente_Utilizzato = 0

            End Select

            'Calcolo Qta Totale Utilizzata nella Lavorazione
            Select Case Mezzo_Dettaglio_Lavorazione
                Case enum_MezzoLavorazione.Distribuzione_Hl 'Distribuzione per Hl
                    Qta_Distribuita = Dose_Lavorazione_Hl * H2O_Lavorazione * Coefficiente_Utilizzato
                    Mezzo_Lavorazione_Des = "/Ettolitro"
                Case enum_MezzoLavorazione.Distribuzione_Ha 'Distribuzione per Ha
                    Qta_Distribuita = Dose_Lavorazione_Ha * Superficie_Lavorazione * Coefficiente_Utilizzato
                    Mezzo_Lavorazione_Des = "/Ettaro"
                Case Else 'Eccezione
                    Qta_Distribuita = 0
                    Mezzo_Lavorazione_Des = ""
            End Select

        Catch ex As Exception
            Udm_Cod_Utilizzato = -1 'Indefinita
            Coefficiente_Utilizzato = 0
            Qta_Distribuita = -1
            Mezzo_Lavorazione_Des = ""

        Finally

        End Try


    End Sub



End Class
