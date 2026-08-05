Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.UtilityProvider
Imports System.Text
Imports AgronicaCoreDataProvider
Imports System.Dynamic
Imports CrystalDecisions.CrystalReports.Engine

Public Class Traduzioni_Stampe

    Public Property Fattura_TraduzioneValorizzata As Boolean

    'Public Property Fattura_Titolo As String
    Public Property Fattura_TitoloFatturaProforma As String
    Public Property Fattura_TitoloAutoFattura As String
    Public Property Fattura_TitoloFatturaIntra As String
    Public Property Fattura_TitoloFatturaAcconto As String
    Public Property Fattura_TitoloFatturaAccomp As String
    Public Property Fattura_TitoloFatturaDifferita As String
    Public Property Fattura_TitoloNDC As String
    Public Property Fattura_TitoloDDT As String
    Public Property Fattura_TitoloOrdineAcquisto As String
    Public Property Fattura_TitoloOrdineVendita As String
    Public Property Fattura_TitoloPreventivoVendita As String

    Public Property Fattura_FatturatoA As String
    Public Property Fattura_Destinatario As String
    Public Property Fattura_DataEmissione As String
    Public Property Fattura_Numero As String
    Public Property Fattura_Scadenza As String
    Public Property Fattura_Valuta As String
    Public Property Fattura_Pagina As String
    Public Property Fattura_ModalitaPagamento As String
    Public Property Fattura_Note As String
    Public Property Fattura_DataSpedizione As String
    Public Property Fattura_Colli As String
    Public Property Fattura_Peso As String
    'non gestito, lasciato LitriLabel1 con dicitura fissa LT
    'Public Property Fattura_Litri As String  
    Public Property Fattura_Aspetto As String
    Public Property Fattura_Causale As String
    Public Property Fattura_Trasporto As String
    Public Property Fattura_GestioneVettore As String
    Public Property Fattura_Vettore As String
    Public Property Fattura_DataConsegna As String
    Public Property Fattura_FirmaDestinatario As String
    Public Property Fattura_FirmaConducente As String
    Public Property Fattura_DescrizioneBeni As String
    Public Property Fattura_UM As String
    Public Property Fattura_Qta As String
    Public Property Fattura_Prezzo As String
    Public Property Fattura_Sconto As String
    Public Property Fattura_Importo As String
    Public Property Fattura_AliqIVA As String
    Public Property Fattura_TotImplordo As String
    Public Property Fattura_TotSconti As String
    Public Property Fattura_TotImpNetto As String
    Public Property Fattura_TotImposta As String
    Public Property Fattura_Tot As String 'TxtTotale

    'Public Property Fattura_TotDocumento 'TxtTotaleDaPagare non è gestita
    Public Property Fattura_TotaleFattura As String
    Public Property Fattura_TotaleFatturaProforma As String
    Public Property Fattura_TotaleNDC As String
    Public Property Fattura_TotaleDDT As String
    Public Property Fattura_TotaleOrdineAcquisto As String
    Public Property Fattura_TotaleOrdineVendita As String
    Public Property Fattura_TotalePreventivoVendita As String

    Public Property Fattura_CalcoloImposta As String
    Public Property Fattura_IVA_imponibile
    Public Property Fattura_IVA_aliquota
    Public Property Fattura_IVA_imposta
    Public Property Fattura_Articolo62 As String
    Public Property Fattura_Privacy As String
    Public Property Fattura_Conai As String
    Public Property Fattura_SEO As String



    Public Sub New(ByVal piva As String,
                          ByVal enum_CodificaStampe As enum_CodificaStampe,
                          ByVal Codice_Lingua As String,
                          ByVal TxtReport_Codice As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)
        'ByRef Flag_TraduzioneValorizzata As Boolean, _
        ' Flag_TraduzioneValorizzata = False


        'DEFAULT
        Fattura_TraduzioneValorizzata = False
        'Fattura_Titolo = ""
        Fattura_TitoloFatturaProforma = ""
        Fattura_TitoloAutoFattura = ""
        Fattura_TitoloFatturaIntra = ""
        Fattura_TitoloFatturaAcconto = ""
        Fattura_TitoloFatturaAccomp = ""
        Fattura_TitoloFatturaDifferita = ""
        Fattura_TitoloNDC = ""
        Fattura_TitoloDDT = ""
        Fattura_TitoloOrdineAcquisto = ""
        Fattura_TitoloOrdineVendita = ""
        Fattura_TitoloPreventivoVendita = ""
        Fattura_FatturatoA = ""
        Fattura_Destinatario = ""
        Fattura_DataEmissione = ""
        Fattura_Numero = ""
        Fattura_Scadenza = ""
        Fattura_Valuta = ""
        Fattura_Pagina = ""
        Fattura_ModalitaPagamento = ""
        Fattura_Note = ""
        Fattura_DataSpedizione = ""
        Fattura_Colli = ""
        Fattura_Peso = ""
        Fattura_Aspetto = ""
        Fattura_Causale = ""
        Fattura_Trasporto = ""
        Fattura_GestioneVettore = ""
        Fattura_Vettore = ""
        Fattura_DataConsegna = ""
        Fattura_FirmaDestinatario = ""
        Fattura_FirmaConducente = ""
        Fattura_DescrizioneBeni = ""
        Fattura_UM = ""
        Fattura_Qta = ""
        Fattura_Prezzo = ""
        Fattura_Sconto = ""
        Fattura_Importo = ""
        Fattura_AliqIVA = ""
        Fattura_TotImplordo = ""
        Fattura_TotSconti = ""
        Fattura_TotImpNetto = ""
        Fattura_TotImposta = ""
        Fattura_Tot = ""
        'Fattura_TotDocumento = ""
        Fattura_TotaleFattura = ""
        Fattura_TotaleFatturaProforma = ""
        Fattura_TotaleNDC = ""
        Fattura_TotaleDDT = ""
        Fattura_TotaleOrdineAcquisto = ""
        Fattura_TotaleOrdineVendita = ""
        Fattura_TotalePreventivoVendita = ""
        Fattura_CalcoloImposta = ""
        Fattura_IVA_imponibile = ""
        Fattura_IVA_aliquota = ""
        Fattura_IVA_imposta = ""
        Fattura_SEO = ""
        Fattura_Articolo62 = ""
        Fattura_Privacy = ""
        Fattura_Conai = ""


        Dim DT As DataTable
        Dim objTrad As New AgronicaCoreStampeDAL.Traduzioni_Stampe_R

        DT = objTrad.Leggi(piva,
                        enum_CodificaStampe,
                        Codice_Lingua,
                        0, -1, "", "",
                        objParametri)

        If Not IsNothing(DT) AndAlso DT.Rows.Count = 0 Then
            'non c'è personalizzazione sulla piva, allora leggo le traduzioni impostate sul superuser -> piva=""
            DT = objTrad.Leggi("",
                             enum_CodificaStampe,
                             Codice_Lingua,
                             0, -1, "", "",
                             objParametri)

        End If


        If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

            ' Flag_TraduzioneValorizzata = True

            For Each Dr In DT.Rows

                Select Case Dr("enum_CodificaStampe")

                    Case enum_CodificaStampe.Fatture

                        '------------------------------------------
                        '----------------- FATTURA ----------------
                        '------------------------------------------

                        Fattura_TraduzioneValorizzata = True

                        'Dim Flag_TitoloMultiRecord As Boolean = False
                        'Dim Dr_Titolo() As DataRow
                        'Dr_Titolo = DT.Select(" TxtReport_Codice = " & CStr(Dr("TxtReport_Codice")))

                        'If Not IsNothing(Dr_Titolo) AndAlso Dr_Titolo.Length > 0 Then
                        '    Flag_TitoloMultiRecord = True
                        'End If


                        Select Case Dr("TxtReport_Codice")


                            '--------------------------------------
                            '------ GESTIONE TITOLO -----------
                            Case enum_Fattura_TxtReport.Titolo

                                Select Case Dr("lav_cod")

                                    Case LAVCOD_FATTURA_EMESSA

                                        Select Case Dr("Tipo_Stampa")
                                            Case enum_TipoStampaFattura.Autofattura
                                                Fattura_TitoloAutoFattura = Dr("TxtReport_Descrizione")
                                            Case enum_TipoStampaFattura.Fattura_AcquistiIntracom
                                                Fattura_TitoloFatturaIntra = Dr("TxtReport_Descrizione")
                                            Case enum_TipoStampaFattura.Fattura_Acconto
                                                Fattura_TitoloFatturaAcconto = Dr("TxtReport_Descrizione")
                                            Case enum_TipoStampaFattura.Fattura_Acconto_Soci
                                                Fattura_TitoloFatturaAcconto = Dr("TxtReport_Descrizione")
                                            Case enum_TipoStampaFattura.Fattura_Immediata
                                                Fattura_TitoloFatturaAccomp = Dr("TxtReport_Descrizione")
                                            Case enum_TipoStampaFattura.Fattura_Differita
                                                Fattura_TitoloFatturaDifferita = Dr("TxtReport_Descrizione")
                                                'Case Else
                                                '    Fattura_Titolo = "NON GESTITO"
                                        End Select
                                        '----------- fatt emessa

                                    Case LAVCOD_NOTA_ACCREDITO_EMESSA
                                        Fattura_TitoloNDC = Dr("TxtReport_Descrizione")

                                    Case LAVCOD_BOLLA_EMESSA, LAVCOD_DDT_CONTABILIZZATO_EMESSO
                                        Fattura_TitoloDDT = Dr("TxtReport_Descrizione")

                                    Case LAVCOD_FATTURA_PROFORMA
                                        Fattura_TitoloFatturaProforma = Dr("TxtReport_Descrizione")

                                    Case LAVCOD_ORDINE_ACQUISTO
                                        Fattura_TitoloOrdineAcquisto = Dr("TxtReport_Descrizione")

                                    Case LAVCOD_ORDINE_VENDITA
                                        Fattura_TitoloOrdineVendita = Dr("TxtReport_Descrizione")

                                    Case LAVCOD_PREVENTIVO_VENDITA
                                        Fattura_TitoloPreventivoVendita = Dr("TxtReport_Descrizione")

                                        'Case Else
                                        '    Fattura_Titolo = "NON GESTITO"

                                End Select 'lav_cod


                                'If Flag_TitoloMultiRecord = False Then
                                '    Fattura_Titolo = Dr("TxtReport_Descrizione")
                                'Else
                                '    Fattura_Titolo = ""
                                'End If
                                '------ GESTIONE TITOLO -----------

                            Case enum_Fattura_TxtReport.FatturatoA
                                Fattura_FatturatoA = Dr("TxtReport_Descrizione")

                            Case enum_Fattura_TxtReport.Destinatario
                                Fattura_Destinatario = Dr("TxtReport_Descrizione")

                            Case enum_Fattura_TxtReport.DataEmissione
                                Fattura_DataEmissione = Dr("TxtReport_Descrizione")

                            Case enum_Fattura_TxtReport.Numero
                                Fattura_Numero = Dr("TxtReport_Descrizione")

                            Case enum_Fattura_TxtReport.Scadenza
                                Fattura_Scadenza = Dr("TxtReport_Descrizione")

                            Case enum_Fattura_TxtReport.Valuta
                                Fattura_Valuta = Dr("TxtReport_Descrizione")

                            Case enum_Fattura_TxtReport.Pagina
                                Fattura_Pagina = Dr("TxtReport_Descrizione")

                            Case enum_Fattura_TxtReport.ModalitaPagamento
                                Fattura_ModalitaPagamento = Dr("TxtReport_Descrizione")

                            Case enum_Fattura_TxtReport.Note
                                Fattura_Note = Dr("TxtReport_Descrizione")

                            Case enum_Fattura_TxtReport.DataSpedizione
                                Fattura_DataSpedizione = Dr("TxtReport_Descrizione")

                            Case enum_Fattura_TxtReport.Colli
                                Fattura_Colli = Dr("TxtReport_Descrizione")

                            Case enum_Fattura_TxtReport.Peso
                                Fattura_Peso = Dr("TxtReport_Descrizione")

                                'Case enum_Fattura_TxtReport.Litri
                                '    Fattura_Litri = Dr("TxtReport_Descrizione")

                            Case enum_Fattura_TxtReport.Aspetto
                                Fattura_Aspetto = Dr("TxtReport_Descrizione")

                            Case enum_Fattura_TxtReport.Causale
                                Fattura_Causale = Dr("TxtReport_Descrizione")

                            Case enum_Fattura_TxtReport.Trasporto
                                Fattura_Trasporto = Dr("TxtReport_Descrizione")

                            Case enum_Fattura_TxtReport.GestioneVettore
                                Fattura_GestioneVettore = Dr("TxtReport_Descrizione")

                            Case enum_Fattura_TxtReport.Vettore
                                Fattura_Vettore = Dr("TxtReport_Descrizione")

                            Case enum_Fattura_TxtReport.DataConsegna
                                Fattura_DataConsegna = Dr("TxtReport_Descrizione")

                            Case enum_Fattura_TxtReport.FirmaDestinatario
                                Fattura_FirmaDestinatario = Dr("TxtReport_Descrizione")

                            Case enum_Fattura_TxtReport.FirmaConducente
                                Fattura_FirmaConducente = Dr("TxtReport_Descrizione")

                            Case enum_Fattura_TxtReport.DescrizioneBeni
                                Fattura_DescrizioneBeni = Dr("TxtReport_Descrizione")

                            Case enum_Fattura_TxtReport.UnitaMisura
                                Fattura_UM = Dr("TxtReport_Descrizione")

                            Case enum_Fattura_TxtReport.Qta
                                Fattura_Qta = Dr("TxtReport_Descrizione")

                            Case enum_Fattura_TxtReport.Prezzo
                                Fattura_Prezzo = Dr("TxtReport_Descrizione")

                            Case enum_Fattura_TxtReport.Sconto
                                Fattura_Sconto = Dr("TxtReport_Descrizione")

                            Case enum_Fattura_TxtReport.Importo
                                Fattura_Importo = Dr("TxtReport_Descrizione")

                            Case enum_Fattura_TxtReport.AliqIVA
                                Fattura_AliqIVA = Dr("TxtReport_Descrizione")

                            Case enum_Fattura_TxtReport.TotImplordo
                                Fattura_TotImplordo = Dr("TxtReport_Descrizione")

                            Case enum_Fattura_TxtReport.TotSconti
                                Fattura_TotSconti = Dr("TxtReport_Descrizione")

                            Case enum_Fattura_TxtReport.TotImpNetto
                                Fattura_TotImpNetto = Dr("TxtReport_Descrizione")

                            Case enum_Fattura_TxtReport.TotImposta
                                Fattura_TotImposta = Dr("TxtReport_Descrizione")

                            Case enum_Fattura_TxtReport.Tot
                                Fattura_Tot = Dr("TxtReport_Descrizione")

                            Case enum_Fattura_TxtReport.TotDocumento
                                '------ TOTALE DOCUMENTO

                                Select Case Dr("lav_cod")

                                    Case LAVCOD_FATTURA_EMESSA
                                        Fattura_TotaleFattura = Dr("TxtReport_Descrizione")

                                    Case LAVCOD_NOTA_ACCREDITO_EMESSA
                                        Fattura_TotaleNDC = Dr("TxtReport_Descrizione")

                                    Case LAVCOD_BOLLA_EMESSA, LAVCOD_DDT_CONTABILIZZATO_EMESSO
                                        Fattura_TotaleDDT = Dr("TxtReport_Descrizione")

                                    Case LAVCOD_FATTURA_PROFORMA
                                        Fattura_TotaleFatturaProforma = Dr("TxtReport_Descrizione")

                                    Case LAVCOD_ORDINE_ACQUISTO
                                        Fattura_TotaleOrdineAcquisto = Dr("TxtReport_Descrizione")

                                    Case LAVCOD_ORDINE_VENDITA
                                        Fattura_TotaleOrdineVendita = Dr("TxtReport_Descrizione")

                                    Case LAVCOD_PREVENTIVO_VENDITA
                                        Fattura_TotalePreventivoVendita = Dr("TxtReport_Descrizione")

                                End Select 'lav_cod

                                'Fattura_TotDocumento = Dr("TxtReport_Descrizione")
                                '----- TOTALE DOCUMENTO

                            Case enum_Fattura_TxtReport.CalcoloImposta
                                Fattura_CalcoloImposta = Dr("TxtReport_Descrizione")

                            Case enum_Fattura_TxtReport.IVA_imponibile
                                Fattura_IVA_imponibile = Dr("TxtReport_Descrizione")

                            Case enum_Fattura_TxtReport.IVA_aliquota
                                Fattura_IVA_aliquota = Dr("TxtReport_Descrizione")

                            Case enum_Fattura_TxtReport.IVA_imposta
                                Fattura_IVA_imposta = Dr("TxtReport_Descrizione")

                            Case enum_Fattura_TxtReport.SEO
                                Fattura_SEO = Dr("TxtReport_Descrizione")

                            Case enum_Fattura_TxtReport.Articolo62
                                Fattura_Articolo62 = Dr("TxtReport_Descrizione")

                            Case enum_Fattura_TxtReport.Privacy
                                Fattura_Privacy = Dr("TxtReport_Descrizione")

                            Case enum_Fattura_TxtReport.Conai
                                Fattura_Conai = Dr("TxtReport_Descrizione")

                        End Select 'codici txtreport fattura

                        'FINE FATTURE 
                        '================================================

                End Select 'enum stampe

            Next 'dt

        End If

        'FatturatoA = CStr(dr.Item("FatturatoA"))
        'Destinatario = CStr(dr.Item("Destinatario"))
        'DataEmissione = CStr(dr.Item("DataEmissione"))
        'Numero = CStr(dr.Item("Numero"))
        'Valuta = CStr(dr.Item("Valuta"))
        'Pagina = CStr(dr.Item("Pagina"))
        'ModalitaPagamento = CStr(dr.Item("ModalitaPagamento"))
        'Note = CStr(dr.Item("Note"))
        'DataSpedizione = CStr(dr.Item("DataSpedizione"))
        'Colli = CStr(dr.Item("Colli"))
        'Peso = CStr(dr.Item("Peso"))
        'Litri = CStr(dr.Item("Litri"))
        'Aspetto = CStr(dr.Item("Aspetto"))
        'Causale = CStr(dr.Item("Causale"))
        'Trasporto = CStr(dr.Item("Trasporto"))
        'GestioneVettore = CStr(dr.Item("GestioneVettore"))
        'Vettore = CStr(dr.Item("Vettore"))
        'DataConsegna = CStr(dr.Item("DataConsegna"))
        'FirmaDestinatario = CStr(dr.Item("FirmaDestinatario"))
        'FirmaConducente = CStr(dr.Item("FirmaConducente"))
        'DescrizioneBeni = CStr(dr.Item("DescrizioneBeni"))
        'Qta = CStr(dr.Item("Qta"))
        'Prezzo = CStr(dr.Item("Prezzo"))
        'Sconto = CStr(dr.Item("Sconto"))
        'Importo = CStr(dr.Item("Importo"))
        'AliqIVA = CStr(dr.Item("AliqIVA"))
        'TotImplordo = CStr(dr.Item("TotImplordo"))
        'TotSconti = CStr(dr.Item("TotSconti"))
        'TotImpNetto = CStr(dr.Item("TotImpNetto"))
        'TotImposta = CStr(dr.Item("TotImposta"))
        'Tot = CStr(dr.Item("Tot"))
        'TotDocumento = CStr(dr.Item("TotDocumento"))
        ''SEO() = ""
        'CalcoloImposta = CStr(dr.Item("CalcoloImposta"))
        ''Sottoreport(Iva) = ""
        ''IVA_imponibile() = ""
        ''Sottoreport(Iva) = ""
        ''IVA_aliquota() = ""
        ''Sottoreport(Iva) = ""
        ''IVA_imposta() = ""
        'Articolo62 = CStr(dr.Item("Articolo62"))
        'Privacy = CStr(dr.Item("Privacy"))
        'Conai = CStr(dr.Item("T_Conai"))

    End Sub

    'Public Sub New(ByVal dr As DataRow)

    '    FatturatoA = CStr(dr.Item("FatturatoA"))
    '    Destinatario = CStr(dr.Item("Destinatario"))
    '    DataEmissione = CStr(dr.Item("DataEmissione"))
    '    Numero = CStr(dr.Item("Numero"))
    '    Valuta = CStr(dr.Item("Valuta"))
    '    Pagina = CStr(dr.Item("Pagina"))
    '    ModalitaPagamento = CStr(dr.Item("ModalitaPagamento"))
    '    Note = CStr(dr.Item("Note"))
    '    DataSpedizione = CStr(dr.Item("DataSpedizione"))
    '    Colli = CStr(dr.Item("Colli"))
    '    Peso = CStr(dr.Item("Peso"))
    '    Litri = CStr(dr.Item("Litri"))
    '    Aspetto = CStr(dr.Item("Aspetto"))
    '    Causale = CStr(dr.Item("Causale"))
    '    Trasporto = CStr(dr.Item("Trasporto"))
    '    GestioneVettore = CStr(dr.Item("GestioneVettore"))
    '    Vettore = CStr(dr.Item("Vettore"))
    '    DataConsegna = CStr(dr.Item("DataConsegna"))
    '    FirmaDestinatario = CStr(dr.Item("FirmaDestinatario"))
    '    FirmaConducente = CStr(dr.Item("FirmaConducente"))
    '    DescrizioneBeni = CStr(dr.Item("DescrizioneBeni"))
    '    Qta = CStr(dr.Item("Qta"))
    '    Prezzo = CStr(dr.Item("Prezzo"))
    '    Sconto = CStr(dr.Item("Sconto"))
    '    Importo = CStr(dr.Item("Importo"))
    '    AliqIVA = CStr(dr.Item("AliqIVA"))
    '    TotImplordo = CStr(dr.Item("TotImplordo"))
    '    TotSconti = CStr(dr.Item("TotSconti"))
    '    TotImpNetto = CStr(dr.Item("TotImpNetto"))
    '    TotImposta = CStr(dr.Item("TotImposta"))
    '    Tot = CStr(dr.Item("Tot"))
    '    TotDocumento = CStr(dr.Item("TotDocumento"))
    '    'SEO() = ""
    '    CalcoloImposta = CStr(dr.Item("CalcoloImposta"))
    '    'Sottoreport(Iva) = ""
    '    'IVA_imponibile() = ""
    '    'Sottoreport(Iva) = ""
    '    'IVA_aliquota() = ""
    '    'Sottoreport(Iva) = ""
    '    'IVA_imposta() = ""
    '    Articolo62 = CStr(dr.Item("Articolo62"))
    '    Privacy = CStr(dr.Item("Privacy"))
    '    Conai = CStr(dr.Item("T_Conai"))

    'End Sub

End Class


Public Class Traduzioni_Stampe_R
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function Leggi(ByVal piva As String,
                          ByVal enum_CodificaStampe As Integer,
                          ByVal Codice_Lingua As String,
                          ByVal TxtReport_Codice As Integer,
                          ByVal Tipo_Stampa As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreStampeDAL.Traduzioni_Stampe_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.Append(" SELECT * ")
            strSql.Append(" FROM  Traduzioni_Stampe ")
            strSql.Append(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            'non metto questo controllo perchè piva può essere = "" se la traduzione vale per tutte le aziende
            'If piva <> "" Then
            strSql.Append(" AND Piva ='" & Agro_SQL_SaveText(piva) & "' ")
            'End If

            If Codice_Lingua <> "" Then
                strSql.Append(" AND Codice_Lingua ='" & Agro_SQL_SaveText(Codice_Lingua) & "' ")
            End If

            If enum_CodificaStampe <> 0 Then
                strSql.Append(" AND enum_CodificaStampe = " & Agro_SQL_SaveNum(enum_CodificaStampe) & " ")
            End If

            If TxtReport_Codice <> 0 Then
                strSql.Append(" AND TxtReport_Codice = " & Agro_SQL_SaveNum(TxtReport_Codice) & " ")
            End If

            If Tipo_Stampa <> -1 Then
                strSql.Append(" AND Tipo_Stampa = " & Agro_SQL_SaveNum(Tipo_Stampa) & " ")
            End If
            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                'Else
                '    strSql.Append(" ORDER BY ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


End Class



Public Class Traduzione_Stampa_Base : Inherits LogProvider



    Public Property TraduzioneValorizzata As Boolean
    Public Property Report As ReportClass
        Get
            Return _report
        End Get
        Set(value As ReportClass)
            _report = value
        End Set
    End Property


    Public ReadOnly Property Piva As String
        Get
            Return _piva
        End Get
    End Property

    Public ReadOnly Property Enum_CodificaStampe As enum_CodificaStampe
        Get
            Return _enum_CodificaStampe
        End Get
    End Property

    Public ReadOnly Property Codice_Lingua As String
        Get
            Return _codice_Lingua
        End Get
    End Property

    Public ReadOnly Property Tipo_Stampa As Integer
        Get
            Return _tipo_Stampa
        End Get
    End Property

    Public ReadOnly Property ObjParametri As AgronicaCoreParametri
        Get
            Return _objParametri
        End Get
    End Property

    Public ReadOnly Property DtTraduzioni As DataTable
        Get
            Return _dtTraduzioni
        End Get
    End Property

    Public ReadOnly Property TRaduzioniComuni As Dictionary(Of String, String)
        Get
            Return _altreTraduzioni
        End Get
    End Property

    Private ReadOnly _piva As String = String.Empty
    Private ReadOnly _enum_CodificaStampe As enum_CodificaStampe
    Private ReadOnly _codice_Lingua As String = String.Empty
    Private ReadOnly _tipo_Stampa As Integer
    Private ReadOnly _objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    Private _report As ReportClass = Nothing
    Private _dtTraduzioni As DataTable = Nothing
    Private _altreTraduzioni As Dictionary(Of String, String) = Nothing


    Public Sub New(ByVal piva As String,
                          ByVal enum_CodificaStampe As enum_CodificaStampe,
                          ByVal Codice_Lingua As String,
                          ByVal tipo_Stampa As Integer,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        _piva = piva
        _enum_CodificaStampe = enum_CodificaStampe
        _codice_Lingua = Codice_Lingua
        _tipo_Stampa = tipo_Stampa
        _objParametri = objParametri

    End Sub

    Public Sub New(ByVal piva As String,
                    ByVal enum_CodificaStampe As enum_CodificaStampe,
                    ByVal Codice_Lingua As String,
                    ByVal tipo_Stampa As Integer,
                    ByVal Report As ReportClass,
                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Me.New(piva, enum_CodificaStampe, Codice_Lingua, tipo_Stampa, objParametri)
        _report = Report

    End Sub
    Public Overridable Sub Traduci(ByVal codice_Lingua As String)

    End Sub
    Public Overridable Sub Traduci(ByVal xFiltroAggiuntivo As String, ByVal xOrderBy As String)

        Dim objTrad As New AgronicaCoreStampeDAL.Traduzioni_Stampe_R

        _dtTraduzioni = objTrad.Leggi(_piva,
                        _enum_CodificaStampe,
                        _codice_Lingua,
                        0, _tipo_Stampa, xFiltroAggiuntivo, xOrderBy,
                        _objParametri)

        If Not IsNothing(_dtTraduzioni) AndAlso _dtTraduzioni.Rows.Count = 0 Then
            'non c'è personalizzazione sulla piva, allora leggo le traduzioni impostate sul superuser -> piva=""
            _dtTraduzioni = objTrad.Leggi("",
                             _enum_CodificaStampe,
                             _codice_Lingua,
                             0, _tipo_Stampa, "", "",
                             _objParametri)

        End If
    End Sub

    Public Overridable Sub Leggi(ByVal xFiltroAggiuntivo As String, ByVal xOrderBy As String)

        Dim objTrad As New AgronicaCoreStampeDAL.Traduzioni_Stampe_R

        _dtTraduzioni = objTrad.Leggi(_piva,
                        _enum_CodificaStampe,
                        _codice_Lingua,
                        0, _tipo_Stampa, xFiltroAggiuntivo, xOrderBy,
                        _objParametri)

        If Not IsNothing(_dtTraduzioni) AndAlso _dtTraduzioni.Rows.Count = 0 Then
            'non c'è personalizzazione sulla piva, allora leggo le traduzioni impostate sul superuser -> piva=""
            _dtTraduzioni = objTrad.Leggi("",
                             _enum_CodificaStampe,
                             _codice_Lingua,
                             0, _tipo_Stampa, "", "",
                             _objParametri)

        End If
    End Sub

    Public Sub Inizializza()

        _altreTraduzioni = New Dictionary(Of String, String) From
            {
                {"Cessionario:", "Cessionario:"},
                {"DOCUMENTO DI TRASPORTO (D.d.T.)", "DOCUMENTO DI TRASPORTO (D.d.T.)"},
                {"Partita Iva:", "Partita Iva:"},
                {"Codice Fiscale:", "Codice Fiscale:"},
                {"Prezzo", "Prezzo"},
                {"Cessionario: ", "Cessionario: "},
                {"AUTO - DOCUMENTO DI TRASPORTO (D.d.T.)", "AUTO - DOCUMENTO DI TRASPORTO (D.d.T.)"},
                {"CONTRIBUTO CONAI ASSOLTO OVE DOVUTO", "CONTRIBUTO CONAI ASSOLTO OVE DOVUTO"},
                {"Sconto Merce", "Sconto Merce"},
                {"Omaggio senza rivalsa IVA", "Omaggio senza rivalsa IVA"},
                {"Campione Omaggio senza rivalsa IVA", "Campione Omaggio senza rivalsa IVA"},
                {"Omaggio con rivalsa IVA", "Omaggio con rivalsa IVA"},
                {"Campione Omaggio con rivalsa IVA", "Campione Omaggio con rivalsa IVA"},
                {"Gratuito", "Gratuito"},
                {"Campione Gratuito", "Campione Gratuito"},
                {"U di Seme", "U di Seme"},
                {"Partenza: ", "Partenza: "},
                {"VAT: IT", "VAT: IT"},
                {"Partita IVA: ", "Partita IVA: "},
                {"   Codice Fiscale: ", "   Codice Fiscale: "},
                {"Codice VAT:", "Codice VAT:"},
                {"Tel: ", "Tel: "},
                {" Fax: ", " Fax: "},
                {" Cell: ", " Cell: "},
                {"Cell: ", "Cell: "},
                {"PEC: ", "PEC: "},
                {"Sede legale: ", "Sede legale: "},
                {"Sede operativa: ", "Sede operativa: "},
                {"Stabilimento: ", "Stabilimento: "},
                {"CEDENTE", "CEDENTE"},
                {"CESSIONARIO", "CESSIONARIO"},
                {"CONFERENTE", "CONFERENTE"},
                {"CONFERITORE", "CONFERITORE"},
                {"VETTORE", "VETTORE"},
                {"P. Iva: ", "P. Iva: "},
                {"Targa: ", "Targa: "},
                {"Albo: ", "Albo: "},
                {"N.Autorizz.Trasp.: ", "N.Autorizz.Trasp.: "},
                {"D.d.T. emesso dallo Stabilimento di ALFONSINE", "D.d.T. emesso dallo Stabilimento di ALFONSINE"},
                {"D.d.T. emesso dallo Stabilimento di LARINO", "D.d.T. emesso dallo Stabilimento di LARINO"},
                {"Peso lordo tot", "Peso lordo tot"},
                {"Peso netto tot", "Peso netto tot"},
                {"Kg Fattura", "Kg Fattura"},
                {"Agente: ", "Agente: "},
                {"Iscr. al n. ", "Iscr. al n. "},
                {" del Reg. Imprese di ", " del Reg. Imprese di "},
                {"R.E.A.: ", "R.E.A.: "},
                {"   Codice BNDOO: ", "   Codice BNDOO: "},
                {"   Cod. operatore Bio: ", "   Cod. operatore Bio: "},
                {"   Codice ISO: ", "   Codice ISO: "}
            }

        If _codice_Lingua.ToLower() <> "it" Then
            LeggiTraduzioniComuni()
        End If

    End Sub

    Public Function ValoreDizionarioTraduzioneComuni(ByVal chiave As String) As String

        If _altreTraduzioni.ContainsKey(chiave) Then
            Return _altreTraduzioni(chiave)
        End If

        Scrivi_LOG(ObjParametri, "Traduzioni_Stampe_Base.ValoreDizionarioTraduzioneComuni",
                   String.Format("Valore traduzione non trovato per la chiave < {0} >", chiave))

        Throw New Exception(String.Format("Valore traduzione non trovato per la chiave < {0} >", chiave))

    End Function

    Private Sub LeggiTraduzioniComuni()

        Dim objTrad As New AgronicaCoreStampeDAL.Traduzioni_Stampe_R

        Dim dtTraduzioniCOmuni = objTrad.Leggi("",
                        -1,
                        Codice_Lingua, 0, -1, "", "", ObjParametri)

        If Not dtTraduzioniCOmuni Is Nothing AndAlso dtTraduzioniCOmuni.Rows.Count > 0 Then
            For Each dr As DataRow In dtTraduzioniCOmuni.AsEnumerable
                Dim trad = dr.Item("TxtReport_Descrizione").ToString().Split("|")
                Dim chiave = trad(0).Replace("§", "")
                Dim valore = trad(1)

                If _altreTraduzioni.ContainsKey(chiave) Then
                    _altreTraduzioni(chiave) = valore
                Else
                    _altreTraduzioni.Add(chiave, valore)
                    Scrivi_LOG(ObjParametri, "Traduzioni_Stampe_Base.LeggiTraduzioniComuni",
                            String.Format("Aggiunto al di dizionario delle traduzioni comuni la chiave dal DB < {0} > ", chiave))
                End If

            Next
        End If

    End Sub

    Protected Overridable Function InizializzaOggettoTraduzioni() As ExpandoObject
        Return Nothing
    End Function
End Class


Public Class Traduzione_Stampa_Ricette : Inherits Traduzione_Stampa_Base


    Private _traduzioni As Ricette_Tipo_1

    Public ReadOnly Property Traduzioni As Ricette_Tipo_1
        Get
            Return _traduzioni
        End Get
    End Property

    Public Sub New(piva As String, enum_CodificaStampe As enum_CodificaStampe, Codice_Lingua As String, TxtReport_Codice As Integer, ByRef objParametri As AgronicaCoreParametri)
        MyBase.New(piva, enum_CodificaStampe, Codice_Lingua, TxtReport_Codice, objParametri)
    End Sub

    Public Overrides Sub Traduci(codice_Lingua As String)

        InizializzaTraduzioniVuote()


    End Sub


    Public Overrides Sub Traduci(xFiltroAggiuntivo As String, xOrderBy As String)

        InizializzaTraduzioniVuote()

        If MyBase.Codice_Lingua.ToLower() = "it" Then
            Traduzione_Italiano(_traduzioni)
        Else
            MyBase.Traduci(xFiltroAggiuntivo, xOrderBy)
            Traduzione_In_Lingua(_traduzioni, xFiltroAggiuntivo, xOrderBy)
            If Not TraduzioneValorizzata Then
                Traduzione_Italiano(_traduzioni)
            End If
        End If


    End Sub

    Private Sub InizializzaTraduzioniVuote()

        _traduzioni.Appezzamenti = ""
        _traduzioni.FaseFenologicaCorrente = ""
        _traduzioni.RptRicette.TitoloPrincipale = ""
        _traduzioni.RptRicette.Azienda = ""
        _traduzioni.RptRicette.Coltura = ""
        _traduzioni.RptRicette.Data = ""
        _traduzioni.RptRicette.DataVisita = ""
        _traduzioni.RptRicette.Firma_Agricoltore = ""
        _traduzioni.RptRicette.Firma_Responsabile_Tecnico = ""
        _traduzioni.RptRicette.Note = ""
        _traduzioni.RptRicette.Titolo_Report_1 = ""
        _traduzioni.RptRicette.Titolo_Report_2 = ""
        _traduzioni.RptRicette.Da = ""
        _traduzioni.RptRicette.A = ""
        _traduzioni.RptRicette.Numero = ""
        _traduzioni.Rpt_Ricetta_Appezzamenti.Cultivar = ""
        _traduzioni.Rpt_Ricetta_Appezzamenti.Numero_Appezzamento = ""
        _traduzioni.Rpt_Ricetta_Appezzamenti.Superficie_Totale = ""
        _traduzioni.Rpt_Ricetta_Appezzamenti_piano_distribuzione.Campo = ""
        _traduzioni.Rpt_Ricetta_Appezzamenti_piano_distribuzione.Centro = ""
        _traduzioni.Rpt_Ricetta_Appezzamenti_piano_distribuzione.Cultivar = ""
        _traduzioni.Rpt_Ricetta_Appezzamenti_piano_distribuzione.K_Max = ""
        _traduzioni.Rpt_Ricetta_Appezzamenti_piano_distribuzione.Numero_Appezzamento = ""
        _traduzioni.Rpt_Ricetta_Appezzamenti_piano_distribuzione.N_MAx = ""
        _traduzioni.Rpt_Ricetta_Appezzamenti_piano_distribuzione.P_Max = ""
        _traduzioni.Rpt_Ricetta_Appezzamenti_piano_distribuzione.Superficie_HA = ""
        _traduzioni.Rpt_Ricetta_Fertilizzazioni.Data_Cons = ""
        _traduzioni.Rpt_Ricetta_Fertilizzazioni.Dose = ""
        _traduzioni.Rpt_Ricetta_Fertilizzazioni.Intestazione = ""
        _traduzioni.Rpt_Ricetta_Fertilizzazioni.Note = ""
        _traduzioni.Rpt_Ricetta_Fertilizzazioni.Numero_appezzamento = ""
        _traduzioni.Rpt_Ricetta_Fertilizzazioni.Prodotto_Commerciale = ""
        _traduzioni.Rpt_Ricetta_Fertilizzazioni.Qta_Totale = ""
        _traduzioni.Rpt_Ricetta_Fertilizzazioni.Titolo_NPK = ""
        _traduzioni.Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.Data_Cons = ""
        _traduzioni.Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.Dose = ""
        _traduzioni.Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.Intestazione = ""
        _traduzioni.Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.K_Distribuzione = ""
        _traduzioni.Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.K_Resid = ""
        _traduzioni.Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.Note = ""
        _traduzioni.Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.Numero_appezzamento = ""
        _traduzioni.Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.N_Distribuzione = ""
        _traduzioni.Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.N_Resid = ""
        _traduzioni.Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.Prodotto_Commerciale = ""
        _traduzioni.Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.P_Distribuzione = ""
        _traduzioni.Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.P_Resid = ""
        _traduzioni.Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.Qta_Totale = ""
        _traduzioni.Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.Titolo_NPK = ""
        _traduzioni.Rpt_Ricetta_Irrigazioni.Data_Cons = ""
        _traduzioni.Rpt_Ricetta_Irrigazioni.Dose = ""
        _traduzioni.Rpt_Ricetta_Irrigazioni.Giorni = ""
        _traduzioni.Rpt_Ricetta_Irrigazioni.Intestazione = ""
        _traduzioni.Rpt_Ricetta_Irrigazioni.Numero_appezzamento = ""
        _traduzioni.Rpt_Ricetta_Irrigazioni.Ore = ""
        _traduzioni.Rpt_Ricetta_Irrigazioni.Periodo_Turno_Medio = ""
        _traduzioni.Rpt_Ricetta_Irrigazioni.Portata = ""
        _traduzioni.Rpt_Ricetta_Irrigazioni.Unita_Misura = ""
        _traduzioni.Rpt_Ricetta_Lavorazioni.Data_Cons = ""
        _traduzioni.Rpt_Ricetta_Lavorazioni.Intestazione = ""
        _traduzioni.Rpt_Ricetta_Lavorazioni.Note = ""
        _traduzioni.Rpt_Ricetta_Lavorazioni.Numero_appezzamento = ""
        _traduzioni.Rpt_Ricetta_Lavorazioni.Operazione = ""
        _traduzioni.Rpt_Ricetta_Trappole.Avversita = ""
        _traduzioni.Rpt_Ricetta_Trappole.Data_Cons = ""
        _traduzioni.Rpt_Ricetta_Trappole.Ditta = ""
        _traduzioni.Rpt_Ricetta_Trappole.Intestazione = ""
        _traduzioni.Rpt_Ricetta_Trappole.Lavorazione = ""
        _traduzioni.Rpt_Ricetta_Trappole.note = ""
        _traduzioni.Rpt_Ricetta_Trappole.Numero_appezzamento = ""
        _traduzioni.Rpt_Ricetta_Trappole.Qta = ""
        _traduzioni.Rpt_Ricetta_Trappole.Trappola = ""
        _traduzioni.Rpt_Ricetta_Trattamenti_2.Avversita = ""
        _traduzioni.Rpt_Ricetta_Trattamenti_2.Carenza = ""
        _traduzioni.Rpt_Ricetta_Trattamenti_2.Data_Cons = ""
        _traduzioni.Rpt_Ricetta_Trattamenti_2.Data_Utile_Prima_Raccolta = ""
        _traduzioni.Rpt_Ricetta_Trattamenti_2.Dose = ""
        _traduzioni.Rpt_Ricetta_Trattamenti_2.Intestazione = ""
        _traduzioni.Rpt_Ricetta_Trattamenti_2.Note = ""
        _traduzioni.Rpt_Ricetta_Trattamenti_2.Numero_appezzamento = ""
        _traduzioni.Rpt_Ricetta_Trattamenti_2.Principi_Attivi = ""
        _traduzioni.Rpt_Ricetta_Trattamenti_2.Prodotto_Commerciale = ""
        _traduzioni.Rpt_Ricetta_Trattamenti_2.Qta_Totale = ""
        _traduzioni.Rpt_Ricetta_Trattamenti_2.Tempo_Rientro = ""
        _traduzioni.Rpt_Ricetta_Trattamenti_2.Volume_H2O = ""

    End Sub
    Public Sub Traduzione_Italiano(ByRef traduzioni As Ricette_Tipo_1)

        traduzioni.FaseFenologicaCorrente = "Ultima Fase Fenologica Rilevata: "
        traduzioni.Appezzamenti = "Appezzamenti:"
        traduzioni.RptRicette.TitoloPrincipale = "RICETTA"
        traduzioni.RptRicette.Coltura = "Coltura:"
        traduzioni.RptRicette.Azienda = "Azienda:"
        traduzioni.RptRicette.Note = "Note:"
        traduzioni.RptRicette.Data = "Data:"
        traduzioni.RptRicette.DataVisita = "Data Visita: "
        traduzioni.RptRicette.Firma_Responsabile_Tecnico = "Firma Responsabile Tecnico:"
        traduzioni.RptRicette.Firma_Agricoltore = "Firma Agricoltore:"
        traduzioni.RptRicette.Titolo_Report_1 = "RICETTA "
        traduzioni.RptRicette.Titolo_Report_2 = "RICETTA Numero "
        traduzioni.RptRicette.Da = "Da "
        traduzioni.RptRicette.A = "A "
        traduzioni.RptRicette.Numero = "N° Ricetta:"

        traduzioni.Rpt_Ricetta_Appezzamenti.Cultivar = "Cultivar"
        traduzioni.Rpt_Ricetta_Appezzamenti.Numero_Appezzamento = "N. ro App. (Sup. Ha)"
        traduzioni.Rpt_Ricetta_Appezzamenti.Superficie_Totale = "Sup.Tot. Ha"

        traduzioni.Rpt_Ricetta_Appezzamenti_piano_distribuzione.Centro = "Centro"
        traduzioni.Rpt_Ricetta_Appezzamenti_piano_distribuzione.Campo = "Campo"
        traduzioni.Rpt_Ricetta_Appezzamenti_piano_distribuzione.Numero_Appezzamento = "N.App."
        traduzioni.Rpt_Ricetta_Appezzamenti_piano_distribuzione.Cultivar = "Cultivar"
        traduzioni.Rpt_Ricetta_Appezzamenti_piano_distribuzione.Superficie_HA = "Sup. Ha"
        traduzioni.Rpt_Ricetta_Appezzamenti_piano_distribuzione.N_MAx = "N Massimo [Kg/Ha]"
        traduzioni.Rpt_Ricetta_Appezzamenti_piano_distribuzione.P_Max = "P Massimo [Kg/Ha]"
        traduzioni.Rpt_Ricetta_Appezzamenti_piano_distribuzione.K_Max = "K Massimo [Kg/Ha]"

        traduzioni.Rpt_Ricetta_Trattamenti_2.Intestazione = "TRATTAMENTI INSETTICIDI, ACARICIDI, FUNGICIDI, ERBICIDI E FITOREGOLATORI"
        traduzioni.Rpt_Ricetta_Trattamenti_2.Data_Cons = "Data Cons."
        traduzioni.Rpt_Ricetta_Trattamenti_2.Numero_appezzamento = "N.ro App."
        traduzioni.Rpt_Ricetta_Trattamenti_2.Avversita = "Avversita' / Infestanti"
        traduzioni.Rpt_Ricetta_Trattamenti_2.Prodotto_Commerciale = "Prodotto Commerciale"
        traduzioni.Rpt_Ricetta_Trattamenti_2.Principi_Attivi = "Principi Attivi"
        traduzioni.Rpt_Ricetta_Trattamenti_2.Carenza = "gg. Carenza"
        traduzioni.Rpt_Ricetta_Trattamenti_2.Data_Utile_Prima_Raccolta = "Data utile 1a raccolta"
        traduzioni.Rpt_Ricetta_Trattamenti_2.Tempo_Rientro = "Tempo Rientro"
        traduzioni.Rpt_Ricetta_Trattamenti_2.Dose = "Dose"
        traduzioni.Rpt_Ricetta_Trattamenti_2.Qta_Totale = "Q.ta' Tot."
        traduzioni.Rpt_Ricetta_Trattamenti_2.Volume_H2O = "Vol. H2O"
        traduzioni.Rpt_Ricetta_Trattamenti_2.Note = "Rif. App. / Note / Giustif. Intervento"

        traduzioni.Rpt_Ricetta_Fertilizzazioni.Intestazione = "FERTILIZZAZIONI"
        traduzioni.Rpt_Ricetta_Fertilizzazioni.Data_Cons = "Data Cons."
        traduzioni.Rpt_Ricetta_Fertilizzazioni.Numero_appezzamento = "N.ro App."
        traduzioni.Rpt_Ricetta_Fertilizzazioni.Prodotto_Commerciale = "Prodotto Commerciale"
        traduzioni.Rpt_Ricetta_Fertilizzazioni.Titolo_NPK = "Titolo (N-P-K)"
        traduzioni.Rpt_Ricetta_Fertilizzazioni.Dose = "Dose/Ha"
        traduzioni.Rpt_Ricetta_Fertilizzazioni.Qta_Totale = "Q.ta' Tot."
        traduzioni.Rpt_Ricetta_Fertilizzazioni.Note = "Rif. App. / Note"

        traduzioni.Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.Intestazione = "FERTILIZZAZIONI"
        traduzioni.Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.Data_Cons = "Data Cons."
        traduzioni.Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.Numero_appezzamento = "N.ro App."
        traduzioni.Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.Prodotto_Commerciale = "Prodotto Commerciale"
        traduzioni.Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.Titolo_NPK = "Titolo (N-P-K)"
        traduzioni.Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.Dose = "Dose/Ha"
        traduzioni.Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.Qta_Totale = "Q.ta' Tot."
        traduzioni.Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.N_Distribuzione = "N Distrib.[Kg/Ha]"
        traduzioni.Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.N_Resid = "N Resid.[Kg/Ha]"
        traduzioni.Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.P_Distribuzione = "P Distrib.[Kg/Ha]"
        traduzioni.Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.P_Resid = "P Resid.[Kg/Ha]"
        traduzioni.Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.K_Distribuzione = "K Distrib.[Kg/Ha]"
        traduzioni.Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.K_Resid = "K Resid.[Kg/Ha]"
        traduzioni.Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.Note = "Note"

        traduzioni.Rpt_Ricetta_Irrigazioni.Intestazione = "IRRIGAZIONI"
        traduzioni.Rpt_Ricetta_Irrigazioni.Data_Cons = "Data Cons."
        traduzioni.Rpt_Ricetta_Irrigazioni.Numero_appezzamento = "N.ro App."
        traduzioni.Rpt_Ricetta_Irrigazioni.Dose = "Dose"
        traduzioni.Rpt_Ricetta_Irrigazioni.Unita_Misura = "Unita'di Misura"
        traduzioni.Rpt_Ricetta_Irrigazioni.Ore = "Ore"
        traduzioni.Rpt_Ricetta_Irrigazioni.Portata = "Portata [l/ora]"
        traduzioni.Rpt_Ricetta_Irrigazioni.Periodo_Turno_Medio = "Periodo Turno Medio"
        traduzioni.Rpt_Ricetta_Irrigazioni.Giorni = "Giorni"

        traduzioni.Rpt_Ricetta_Trappole.Intestazione = "INSTALLAZIONE TRAPPOLE / DISPENSER E RILIEVI IN CAMPO"
        traduzioni.Rpt_Ricetta_Trappole.Data_Cons = "Data Cons."
        traduzioni.Rpt_Ricetta_Trappole.Numero_appezzamento = "N.ro App."
        traduzioni.Rpt_Ricetta_Trappole.Lavorazione = "Lavorazione"
        traduzioni.Rpt_Ricetta_Trappole.Trappola = "Trappola"
        traduzioni.Rpt_Ricetta_Trappole.Ditta = "Ditta"
        traduzioni.Rpt_Ricetta_Trappole.Avversita = "Avversità"
        traduzioni.Rpt_Ricetta_Trappole.Qta = "N.ro"
        traduzioni.Rpt_Ricetta_Trappole.note = "Rif. App. / Note / Giustif. Intervento"

        traduzioni.Rpt_Ricetta_Lavorazioni.Intestazione = "ALTRE LAVORAZIONI"
        traduzioni.Rpt_Ricetta_Lavorazioni.Data_Cons = "Data Cons."
        traduzioni.Rpt_Ricetta_Lavorazioni.Numero_appezzamento = "N.ro App."
        traduzioni.Rpt_Ricetta_Lavorazioni.Operazione = "Operazione"
        traduzioni.Rpt_Ricetta_Lavorazioni.Note = "Rif. App. / Note / Giustif. Intervento"

    End Sub

    Private Sub Traduzione_In_Lingua(ByRef traduzioni As Ricette_Tipo_1, ByVal filtroAggiuntivo As String, ByVal orderBy As String)

        Dim DT As DataTable = MyBase.DtTraduzioni

        If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

            TraduzioneValorizzata = True

            For Each Dr As DataRow In DT.Rows

                Dim codiceCampo = CInt(Dr.Item("TxtReport_Codice"))
                Dim labelCampoCompleta = Dr.Item("TxtReport_Descrizione").ToString()
                Dim labeLCampo = labelCampoCompleta.Split("|")(1)

                Select Case codiceCampo
                    Case 1
                        traduzioni.RptRicette.Coltura = labeLCampo
                    Case 2
                        traduzioni.RptRicette.Azienda = labeLCampo
                    Case 3
                        traduzioni.RptRicette.Data = labeLCampo
                    Case 4
                        traduzioni.RptRicette.Note = labeLCampo
                    Case 5
                        traduzioni.RptRicette.Firma_Agricoltore = labeLCampo
                    Case 6
                        traduzioni.Rpt_Ricetta_Appezzamenti.Cultivar = labeLCampo
                    Case 7
                        traduzioni.Rpt_Ricetta_Appezzamenti.Numero_Appezzamento = labeLCampo
                    Case 8
                        traduzioni.Rpt_Ricetta_Appezzamenti.Superficie_Totale = labeLCampo
                    Case 9
                        traduzioni.Rpt_Ricetta_Appezzamenti_piano_distribuzione.Centro = labeLCampo
                    Case 10
                        traduzioni.Rpt_Ricetta_Appezzamenti_piano_distribuzione.Campo = labeLCampo
                    Case 11
                        traduzioni.Rpt_Ricetta_Appezzamenti_piano_distribuzione.Numero_Appezzamento = labeLCampo
                    Case 12
                        traduzioni.Rpt_Ricetta_Appezzamenti_piano_distribuzione.Cultivar = labeLCampo
                    Case 13
                        traduzioni.Rpt_Ricetta_Appezzamenti_piano_distribuzione.Superficie_HA = labeLCampo
                    Case 14
                        traduzioni.Rpt_Ricetta_Appezzamenti_piano_distribuzione.N_MAx = labeLCampo
                    Case 15
                        traduzioni.Rpt_Ricetta_Appezzamenti_piano_distribuzione.P_Max = labeLCampo
                    Case 16
                        traduzioni.Rpt_Ricetta_Appezzamenti_piano_distribuzione.K_Max = labeLCampo
                    Case 17
                        traduzioni.Rpt_Ricetta_Trattamenti_2.Intestazione = labeLCampo
                    Case 18
                        traduzioni.Rpt_Ricetta_Trattamenti_2.Data_Cons = labeLCampo
                    Case 19
                        traduzioni.Rpt_Ricetta_Trattamenti_2.Numero_appezzamento = labeLCampo
                    Case 20
                        traduzioni.Rpt_Ricetta_Trattamenti_2.Avversita = labeLCampo
                    Case 21
                        traduzioni.Rpt_Ricetta_Trattamenti_2.Prodotto_Commerciale = labeLCampo
                    Case 22
                        traduzioni.Rpt_Ricetta_Trattamenti_2.Principi_Attivi = labeLCampo
                    Case 23
                        traduzioni.Rpt_Ricetta_Trattamenti_2.Carenza = labeLCampo
                    Case 24
                        traduzioni.Rpt_Ricetta_Trattamenti_2.Data_Utile_Prima_Raccolta = labeLCampo
                    Case 25
                        traduzioni.Rpt_Ricetta_Trattamenti_2.Tempo_Rientro = labeLCampo
                    Case 26
                        traduzioni.Rpt_Ricetta_Trattamenti_2.Dose = labeLCampo
                    Case 27
                        traduzioni.Rpt_Ricetta_Trattamenti_2.Qta_Totale = labeLCampo
                    Case 28
                        traduzioni.Rpt_Ricetta_Trattamenti_2.Volume_H2O = labeLCampo
                    Case 29
                        traduzioni.Rpt_Ricetta_Trattamenti_2.Note = labeLCampo
                    Case 30
                        traduzioni.Rpt_Ricetta_Fertilizzazioni.Intestazione = labeLCampo
                    Case 31
                        traduzioni.Rpt_Ricetta_Fertilizzazioni.Data_Cons = labeLCampo
                    Case 32
                        traduzioni.Rpt_Ricetta_Fertilizzazioni.Numero_appezzamento = labeLCampo
                    Case 33
                        traduzioni.Rpt_Ricetta_Fertilizzazioni.Prodotto_Commerciale = labeLCampo
                    Case 34
                        traduzioni.Rpt_Ricetta_Fertilizzazioni.Titolo_NPK = labeLCampo
                    Case 35
                        traduzioni.Rpt_Ricetta_Fertilizzazioni.Dose = labeLCampo
                    Case 36
                        traduzioni.Rpt_Ricetta_Fertilizzazioni.Qta_Totale = labeLCampo
                    Case 37
                        traduzioni.Rpt_Ricetta_Fertilizzazioni.Note = labeLCampo
                    Case 38
                        traduzioni.Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.Intestazione = labeLCampo
                    Case 39
                        traduzioni.Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.Data_Cons = labeLCampo
                    Case 40
                        traduzioni.Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.Numero_appezzamento = labeLCampo
                    Case 41
                        traduzioni.Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.Prodotto_Commerciale = labeLCampo
                    Case 42
                        traduzioni.Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.Titolo_NPK = labeLCampo
                    Case 43
                        traduzioni.Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.Dose = labeLCampo
                    Case 44
                        traduzioni.Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.Qta_Totale = labeLCampo
                    Case 45
                        traduzioni.Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.N_Distribuzione = labeLCampo
                    Case 46
                        traduzioni.Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.N_Resid = labeLCampo
                    Case 47
                        traduzioni.Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.P_Distribuzione = labeLCampo
                    Case 48
                        traduzioni.Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.P_Resid = labeLCampo
                    Case 49
                        traduzioni.Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.K_Distribuzione = labeLCampo
                    Case 50
                        traduzioni.Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.K_Resid = labeLCampo
                    Case 51
                        traduzioni.Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.Note = labeLCampo
                    Case 52
                        traduzioni.Rpt_Ricetta_Irrigazioni.Intestazione = labeLCampo
                    Case 53
                        traduzioni.Rpt_Ricetta_Irrigazioni.Data_Cons = labeLCampo
                    Case 54
                        traduzioni.Rpt_Ricetta_Irrigazioni.Numero_appezzamento = labeLCampo
                    Case 55
                        traduzioni.Rpt_Ricetta_Irrigazioni.Dose = labeLCampo
                    Case 56
                        traduzioni.Rpt_Ricetta_Irrigazioni.Unita_Misura = labeLCampo
                    Case 57
                        traduzioni.Rpt_Ricetta_Irrigazioni.Ore = labeLCampo
                    Case 58
                        traduzioni.Rpt_Ricetta_Irrigazioni.Portata = labeLCampo
                    Case 59
                        traduzioni.Rpt_Ricetta_Irrigazioni.Periodo_Turno_Medio = labeLCampo
                    Case 60
                        traduzioni.Rpt_Ricetta_Irrigazioni.Giorni = labeLCampo
                    Case 61
                        traduzioni.Rpt_Ricetta_Trappole.Intestazione = labeLCampo
                    Case 62
                        traduzioni.Rpt_Ricetta_Trappole.Data_Cons = labeLCampo
                    Case 63
                        traduzioni.Rpt_Ricetta_Trappole.Numero_appezzamento = labeLCampo
                    Case 64
                        traduzioni.Rpt_Ricetta_Trappole.Lavorazione = labeLCampo
                    Case 65
                        traduzioni.Rpt_Ricetta_Trappole.Trappola = labeLCampo
                    Case 66
                        traduzioni.Rpt_Ricetta_Trappole.Ditta = labeLCampo
                    Case 67
                        traduzioni.Rpt_Ricetta_Trappole.Avversita = labeLCampo
                    Case 68
                        traduzioni.Rpt_Ricetta_Trappole.Qta = labeLCampo
                    Case 69
                        traduzioni.Rpt_Ricetta_Trappole.note = labeLCampo
                    Case 70
                        traduzioni.Rpt_Ricetta_Lavorazioni.Intestazione = labeLCampo
                    Case 71
                        traduzioni.Rpt_Ricetta_Lavorazioni.Data_Cons = labeLCampo
                    Case 72
                        traduzioni.Rpt_Ricetta_Lavorazioni.Numero_appezzamento = labeLCampo
                    Case 73
                        traduzioni.Rpt_Ricetta_Lavorazioni.Operazione = labeLCampo
                    Case 74
                        traduzioni.Rpt_Ricetta_Lavorazioni.Note = labeLCampo
                    Case 75
                        traduzioni.RptRicette.Firma_Responsabile_Tecnico = labeLCampo
                    Case 76
                        traduzioni.RptRicette.Titolo_Report_1 = labeLCampo
                    Case 77
                        traduzioni.RptRicette.Titolo_Report_2 = labeLCampo
                    Case 78
                        traduzioni.RptRicette.Da = labeLCampo
                    Case 79
                        traduzioni.RptRicette.A = labeLCampo
                    Case 80
                        traduzioni.RptRicette.TitoloPrincipale = labeLCampo
                    Case 81
                        traduzioni.Appezzamenti = labeLCampo
                    Case 82
                        traduzioni.FaseFenologicaCorrente = labeLCampo
                    Case 83
                        traduzioni.RptRicette.DataVisita = labeLCampo
                    Case 84
                        traduzioni.RptRicette.Numero = labeLCampo
                End Select


            Next

        End If

    End Sub

End Class

Public Structure Ricette_Tipo_1
    Public Appezzamenti As String
    Public FaseFenologicaCorrente As String
    Public RptRicette As RptRicette
    Public Rpt_Ricetta_Appezzamenti As Rpt_Ricetta_Appezzamenti
    Public Rpt_Ricetta_Appezzamenti_piano_distribuzione As Rpt_Ricetta_Appezzamenti_piano_distribuzione
    Public Rpt_Ricetta_Trattamenti_2 As Rpt_Ricetta_Trattamenti_2
    Public Rpt_Ricetta_Fertilizzazioni As Rpt_Ricetta_Fertilizzazioni
    Public Rpt_Ricetta_Fertilizzazioni_piano_distribuzione As Rpt_Ricetta_Fertilizzazioni_piano_distribuzione
    Public Rpt_Ricetta_Irrigazioni As Rpt_Ricetta_Irrigazioni
    Public Rpt_Ricetta_Trappole As Rpt_Ricetta_Trappole
    Public Rpt_Ricetta_Lavorazioni As Rpt_Ricetta_Lavorazioni
End Structure

Public Structure RptRicette
    Public Coltura As String
    Public Azienda As String
    Public Data As String
    Public Note As String
    Public DataVisita As String
    Public Firma_Agricoltore As String
    Public Firma_Responsabile_Tecnico As String
    Public Titolo_Report_1 As String
    Public Titolo_Report_2 As String
    Public Da As String
    Public A As String
    Public TitoloPrincipale As String
    Public Numero As String
End Structure

Public Structure Rpt_Ricetta_Appezzamenti
    Public Cultivar As String
    Public Numero_Appezzamento As String
    Public Superficie_Totale As String
End Structure

Public Structure Rpt_Ricetta_Appezzamenti_piano_distribuzione
    Public Centro As String
    Public Campo As String
    Public Numero_Appezzamento As String
    Public Cultivar As String
    Public Superficie_HA As String
    Public N_MAx As String
    Public P_Max As String
    Public K_Max As String
End Structure

Public Structure Rpt_Ricetta_Trattamenti_2
    Public Intestazione As String
    Public Data_Cons As String
    Public Numero_appezzamento As String
    Public Avversita As String
    Public Prodotto_Commerciale As String
    Public Principi_Attivi As String
    Public Carenza As String
    Public Data_Utile_Prima_Raccolta As String
    Public Tempo_Rientro As String
    Public Dose As String
    Public Qta_Totale As String
    Public Volume_H2O As String
    Public Note As String
End Structure

Public Structure Rpt_Ricetta_Fertilizzazioni
    Public Intestazione As String
    Public Data_Cons As String
    Public Numero_appezzamento As String
    Public Prodotto_Commerciale As String
    Public Titolo_NPK As String
    Public Dose As String
    Public Qta_Totale As String
    Public Note As String
End Structure

Public Structure Rpt_Ricetta_Fertilizzazioni_piano_distribuzione
    Public Intestazione As String
    Public Data_Cons As String
    Public Numero_appezzamento As String
    Public Prodotto_Commerciale As String
    Public Titolo_NPK As String
    Public Dose As String
    Public Qta_Totale As String
    Public N_Distribuzione As String
    Public N_Resid As String
    Public P_Distribuzione As String
    Public P_Resid As String
    Public K_Distribuzione As String
    Public K_Resid As String
    Public Note As String

End Structure

Public Structure Rpt_Ricetta_Irrigazioni
    Public Intestazione As String
    Public Data_Cons As String
    Public Numero_appezzamento As String
    Public Dose As String
    Public Unita_Misura As String
    Public Ore As String
    Public Portata As String
    Public Periodo_Turno_Medio As String
    Public Giorni As String
End Structure

Public Structure Rpt_Ricetta_Trappole
    Public Intestazione As String
    Public Data_Cons As String
    Public Numero_appezzamento As String
    Public Lavorazione As String
    Public Trappola As String
    Public Ditta As String
    Public Avversita As String
    Public Qta As String
    Public note As String
End Structure

Public Structure Rpt_Ricetta_Lavorazioni
    Public Intestazione As String
    Public Data_Cons As String
    Public Numero_appezzamento As String
    Public Operazione As String
    Public Note As String
End Structure
