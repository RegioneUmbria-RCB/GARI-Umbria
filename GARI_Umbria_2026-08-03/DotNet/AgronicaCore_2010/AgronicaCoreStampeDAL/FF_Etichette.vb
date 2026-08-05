Imports System.IO
Imports System.Text
Imports System.Web.Script.Serialization
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports ZXing
Imports AgronicaCoreDataProvider.DataProviderExtensions

Public Class FF_Etichette_R
    Inherits AgronicaCoreDataProvider.DataProvider
    Private Const Movimenti_Alias_mProd As String = "mProd"
    Private Const Movimenti_Alias_mTestata As String = "mTest"
    Private Const Movimenti_Alias_mBolla As String = "mBoll"
    Private Const Movimenti_Alias_mImballi As String = "mImball"
    Private Const SeparatoreCodiceQRY As String = "-"

    Public Function LeggiPickingList_Full(
                ByVal id_agenda As Integer,
                ByVal StartCodeChar As String,
                ByVal EndCodeChar As String,
                ByVal StartAIChar As String,
                ByVal lSetCode128 As String,
                ByVal nRigheBarcode As Integer,
                ByVal xFiltroAggiuntivo As String,
                ByVal xOrderBy As String,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
            ) As DataSet


        Dim dt As DataSet = LeggiPickingList(id_agenda, xFiltroAggiuntivo, xOrderBy, objParametri)


        Dim bcc As BarCodeCreator = New BarCodeCreator(
            StartCodeChar,
            EndCodeChar,
            StartAIChar,
            lSetCode128
        )


        For Each r In dt.Tables(1).Rows

            bcc.DataBlockCollection.IndiceDivisioneCodice = 3
            bcc.DataBlockCollection.Add(New DataBlock(bcc.DataBlockCollection, 1, "", bcc.CreateCode(r("barcodeRigaORdine")), 1))


            r("barcodeRigaORdine") = bcc.Code_Top

        Next


        Return dt

    End Function

    Private Function GetPiva(ByVal id_Agenda As Integer, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, ByVal NomeRoutine As String, ByVal stb As System.Text.StringBuilder) As String
        stb.Length = 0
        stb.Append("SELECT piva from agenda where id_agenda = " & id_Agenda & vbCrLf)

        Dim dtAg As DataTable =
            EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)

        Dim piva As String = dtAg.Rows(0)("piva")

        stb.Length = 0
        Return piva
    End Function

    Private Function GetLav_cod(ByVal id_Agenda As Integer, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, ByVal NomeRoutine As String, ByVal stb As System.Text.StringBuilder) As Integer
        stb.Length = 0
        stb.Append("SELECT lav_cod from agenda where id_agenda = " & id_Agenda & vbCrLf)

        Dim dtAg As DataTable =
            EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)

        Dim lav_cod As Integer = dtAg.Rows(0)("lav_cod")

        stb.Length = 0
        Return lav_cod
    End Function


    Private Function GetModulo(ByVal id_Agenda As Integer, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, ByVal NomeRoutine As String, ByVal stb As System.Text.StringBuilder) As Integer
        stb.Length = 0
        stb.Append("SELECT modulo from agenda where id_agenda = " & id_Agenda & vbCrLf)

        Dim dtAg As DataTable =
            EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)

        Dim lav_cod As Integer = dtAg.Rows(0)("modulo")

        stb.Length = 0
        Return lav_cod
    End Function

    Private Function LeggiPickingList(
          ByVal id_Agenda As Integer,
          ByVal xFiltroAggiuntivo As String,
          ByVal xOrderBy As String,
          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
      ) As DataSet





        '----- Descrizione
        Dim NomeRoutine As String = "FF_Etichette_R.LeggiPickingList()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As New DataSet

        Try
            Dim lav_Cod As Integer = GetLav_cod(id_Agenda, objParametri, NomeRoutine, stb)


            Dim Movimenti As String
            Dim MovimentiDettagli As String
            Dim cau_mov As String

            If lav_Cod = LAVCOD_ORDINE_VENDITA Then
                Movimenti = Movimenti_Alias_mTestata
                MovimentiDettagli = "detProd"
                cau_mov = CAU_SCARICO
            Else
                Movimenti = "mRiff"
                MovimentiDettagli = "detRiff"
                cau_mov = CAU_SCARICO
            End If

            stb.Append("SELECT " & vbCrLf)

            stb.Append("   coalesce(" & Movimenti & ".Doc_Numero_Sin, '') as Serie " & vbCrLf)
            stb.Append(" , coalesce(" & Movimenti & ".Doc_Numero, '') as Numero " & vbCrLf)
            stb.Append(" , cCli.piva as CodCliente " & vbCrLf)
            stb.Append(" , cCli.Rag_Soc  as RSCliente " & vbCrLf)
            stb.Append(" , '' as Indirizzo " & vbCrLf)
            stb.Append(" , '' as CAP " & vbCrLf)
            stb.Append(" , '' as Citta " & vbCrLf)
            stb.Append(" , '' as Provincia " & vbCrLf)
            stb.Append(" , " & Movimenti & ".Data_Movimento as DataOrdine " & vbCrLf)
            stb.Append(" , '' as RifCliente " & vbCrLf)
            stb.Append(" , '' as Note " & vbCrLf)

            LeggiDatiEtichetta_From(cau_mov, "4000", True, True, False, stb)

            stb.Append(" WHERE A.ID_Agenda = " & id_Agenda & vbCrLf)

            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.Append(" AND   A.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.Append(" AND   A.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine, DT, "AAA_FAP_StampaOrdineDiVendita_tmp1")
            '--------------------------------------------------------------------------


            stb.Length = 0

            stb.Append("SELECT " & vbCrLf)
            stb.Append("   coalesce(cast(" & MovimentiDettagli & ".id_mov_det as varchar(100)), '') AS[barcodeRigaORdine]   " & vbCrLf)
            stb.Append(" , coalesce(dense_Rank() over(order by " & MovimentiDettagli & ".id_mov_det), '') as Riga " & vbCrLf)
            stb.Append(" , coalesce(veg.Veg_Des , '') as Specie " & vbCrLf)
            stb.Append(" , '' as CodSpecie " & vbCrLf)
            stb.Append(" , coalesce(cul.cul_des , '')  as Varieta " & vbCrLf)
            stb.Append(" , '' as CodVarieta " & vbCrLf)
            stb.Append(" , otpCalibro.sigla as CodCalibro " & vbCrLf)
            stb.Append(" , otpCalibro.Descrizione as Calibro " & vbCrLf)
            stb.Append(" , otpImballaggio.sigla as CodImballo " & vbCrLf)
            stb.Append(" , otpImballaggio.Descrizione as DescrImballo " & vbCrLf)
            stb.Append(" , cast(" & MovimentiDettagli & ".Qta_Dettaglio1 as integer) as NumColli " & vbCrLf)
            stb.Append(" , otpConfezione.Sigla as CodConfezione " & vbCrLf)
            stb.Append(" , otpConfezione.Descrizione as Confezione " & vbCrLf)
            stb.Append(" , '' as PesoXRiga " & vbCrLf)
            stb.Append(" , '' as Note")


            LeggiDatiEtichetta_From(cau_mov, "4000", True, True, False, stb)

            leggiParametriOmniFF_FROM("Calibro", stb)
            leggiParametriOmniFF_FROM("Qualità", stb)
            leggiParametriOmniFF_FROM("Imballaggio", stb)
            leggiParametriOmniFF_FROM("Confezione", stb)

            stb.Append(" WHERE A.ID_Agenda = " & id_Agenda & vbCrLf)


            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.Append(" AND   A.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.Append(" AND   A.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine, DT, "AAA_FAP_StampaOrdineDiVendita_tmp2")
            '--------------------------------------------------------------------------


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT




    End Function



    Public Function LeggiLinguEtichette(
                ByVal xFiltroAggiuntivo As String,
                ByVal xOrderBy As String,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
            ) As DataTable


        '----- Descrizione
        Dim NomeRoutine As String = "FF_Etichette_R.LeggiLinguEtichette()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0

            stb.Append("SELECT LE.Lingua_Cod, LE1.Nome as Lingua_DES " & vbCrLf)
            stb.Append("FROM FF_LinguaEtichette LE ")
            stb.Append("   inner join Lingue LE1 ")
            stb.Append("   on  LE1.Lingua_Cod = LE.Lingua_Cod ")

            stb.Append("WHERE 1=1 ")


            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.Append(" AND   LE.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.Append(" AND   LE.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception


            'uso questa funzione per ottenere il Messaggio..:
            MessaggioErrore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing

            'passo l'eccezione corrente al throw come inner exception ..:
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)



        End Try

        Return DT




    End Function


    Public Function LeggiLayoutEtichette(
                ByVal xFiltroAggiuntivo As String,
                ByVal xOrderBy As String,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
            ) As DataTable


        '----- Descrizione
        Dim NomeRoutine As String = "FF_Etichette_R.LeggiLayoutEtichette()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0

            stb.Append("SELECT *, ff_LayoutEtichette_Des + ' ('+ A.Tipo_etichetta + ')' as DescrizioneEstesa " & vbCrLf)
            stb.Append("FROM FF_LayoutEtichette A ")
            stb.Append("WHERE 1=1 ")
            stb.Append("AND ( piva_SuperUser is Null OR (piva_SuperUser is not null and piva_SuperUser = '" & objParametri.PivaSuperUser & "') ) ")


            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.Append(" AND   A.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.Append(" AND   A.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception


            'uso questa funzione per ottenere il Messaggio..:
            MessaggioErrore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing

            'passo l'eccezione corrente al throw come inner exception ..:
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)



        End Try

        Return DT




    End Function

    Public Function LeggiEtichetta_PalletFull(
                ByVal id_agenda As Integer,
                ByVal StartCodeChar As String,
                ByVal EndCodeChar As String,
                ByVal StartAIChar As String,
                ByVal lSetCode128 As String,
                ByVal nRigheBarcode As Integer,
                ByVal infoCopie As String,
                ByVal xFiltroAggiuntivo As String,
                ByVal xOrderBy As String,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
            ) As DataTable



        Dim dt As DataTable = LeggiEtichetta_Pallet(id_agenda, infoCopie, xFiltroAggiuntivo, xOrderBy, objParametri)


        Dim bcc As BarCodeCreator = New BarCodeCreator(
            StartCodeChar,
            EndCodeChar,
            StartAIChar,
            lSetCode128
        )


        For Each r In dt.Rows
            bcc.DataBlockCollection.IndiceDivisioneCodice = 3

            If nRigheBarcode > 2 Then
                bcc.DataBlockCollection.Add(New DataBlock(bcc.DataBlockCollection, 1, "02", bcc.CreateCode(14, "0", r("codiceean")), 1))
                bcc.DataBlockCollection.Add(New DataBlock(bcc.DataBlockCollection, 3, "37", bcc.CreateCode(r("numeroImballi")), 1))
                'bcc.DataBlockCollection.Add(New DataBlock(bcc.DataBlockCollection, bcc.DataBlockCollection.Count, "400", bcc.CreateCode(r.lottoEsternoImballo)))

                bcc.DataBlockCollection.Add(New DataBlock(bcc.DataBlockCollection, 13, "412", bcc.CreateCode(r("note")), 2))
                'bcc.DataBlockCollection.Add(New DataBlock(bcc.DataBlockCollection, 2, "15", bcc.CreateCode(r("lottoInterno")), 1))
                bcc.DataBlockCollection.Add(New DataBlock(bcc.DataBlockCollection, 2, "15", "", 1))
                bcc.DataBlockCollection.Add(New DataBlock(bcc.DataBlockCollection, 15, "10", bcc.CreateCode(r("lottoEsternoConfezione")), 2))

                bcc.DataBlockCollection.Add(New DataBlock(bcc.DataBlockCollection, 26, "00", r("SSCC"), 3))
                'bcc.DataBlockCollection.Add(New DataBlock(bcc.DataBlockCollection, 27, "400", bcc.CreateCode(r("lottoEsternoImballo")), 3))
                bcc.DataBlockCollection.Add(New DataBlock(bcc.DataBlockCollection, 27, "400", bcc.CreateCode(r("BarcodeOrdine")), 3))

            Else
                bcc.DataBlockCollection.Add(New DataBlock(bcc.DataBlockCollection, 1, "02", bcc.CreateCode(14, "0", r("codiceean")), 1))
                bcc.DataBlockCollection.Add(New DataBlock(bcc.DataBlockCollection, 2, "10", bcc.CreateCode(r("lottoEsternoImballo")), 1))
                bcc.DataBlockCollection.Add(New DataBlock(bcc.DataBlockCollection, 40, "37", bcc.CreateCode(r("numeroImballi")), 3))
                bcc.DataBlockCollection.Add(New DataBlock(bcc.DataBlockCollection, 30, "00", r("SSCC"), 3))

            End If

            r("EAN128_TOP") = bcc.Code_Top
            r("EAN128_TOP_HR") = bcc.HumanReadable_Top
            r("EAN128_MID") = bcc.Code_Mid
            r("EAN128_MID_HR") = bcc.HumanReadable_MID
            r("EAN128_BOTTOM") = bcc.Code_Bottom
            r("EAN128_BOTTOM_HR") = bcc.HumanReadable_Bottom
        Next

        Return dt

    End Function

    Private Function FormattaData(ByVal data As DateTime) As String
        Return data.Day.ToString.PadLeft(2, "0") & data.Month.ToString.PadLeft(2, "0") & ((data.Year) - 2000).ToString.PadLeft(2, "0")
    End Function

    Private Function PDF417Data(ByVal r As Object) As String

        '  Vanni, 02/11/2015 18:13:04: CODICE A POSTO (personalizzato su COFRUTA, HardCoded nei sorgenti per adesso)

        ' codice fornitore, in aggiunta spazi bianchi a destra fino a raggiungere 10 caratteri
        ' codice articolo, in aggiunta spazi bianchi a destra fino a raggiungere 12 caratteri

        ' accoppiata di codice lotto e numero bolla, in aggiunta Zero a sinistra fino a raggiungere 14 caratteri

        ' Due spazi bianchi

        ' data in formato DDMMYY

        'Return _
        '          r("CodFornitore").ToString.Replace("C", "").PadRight(10, " ") & _
        '           r("Cod_Articolo").PadRight(12, " ") & _
        '           (r("Lotto").ToString & _
        '            r("Bolla_Numero") & "." & _
        '            r("Bolla_Riga") _
        '            ).ToString.PadLeft(14, "0") & _
        '           "  " & FormattaData(r("Data"))

        Dim PostFisso As String = "C"
        Dim vPostFisso As String()
        vPostFisso = r("Bolla_Numero").split("/")

        If vPostFisso.Count > 1 Then
            PostFisso = vPostFisso(1)
        End If

        ' Stefano 11/5/17
        'Nel caso di utilizzo delle linee produttive viene utilizzato solo il codice linea compensato con 0
        ' Il codice deve essere lungo 3 e senza _

        'Return _
        '    r("CodFornitore").ToString.Replace("C", "").PadRight(10, " ") & _
        '    r("Cod_Articolo").PadRight(12, " ") & _
        '    ( _
        '        r("Lotto").padleft(10, "0").ToString & _
        '        Right(r("Bolla_Numero").ToString, 5).PadLeft(5, "0") & _
        '        Right(r("Bolla_Riga"), 1) _
        '    ).ToString.PadRight(16, " ") & _
        '    FormattaData(r("Data"))
        Dim articolo As String = r("Cod_Articolo")
        Dim indUnderscore = r("Cod_Articolo").IndexOf("_")
        If indUnderscore > 0 Then
            articolo = r("Cod_Articolo").Replace(r("Cod_Articolo").ToString().Substring((r("Cod_Articolo").IndexOf("_"))), "").PadLeft(3, "0")
        End If
        Return _
           r("CodFornitore").ToString.Replace("C", "").PadRight(10, " ") &
           articolo.PadRight(12, " ") &
           (
               r("Lotto").padleft(10, "0").ToString &
               Right(r("Bolla_Numero").ToString, 5).PadLeft(5, "0") &
               Right(r("Bolla_Riga"), 1)
           ).ToString.PadRight(16, " ") &
           FormattaData(r("Data"))


    End Function

    Private Sub CP_ParametriCrystal(ByVal lingua_cod As Integer, ByRef rpt As CrystalDecisions.CrystalReports.Engine.ReportDocument, ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim sTmp As String
        Dim pvCustPattern As CrystalDecisions.Shared.ParameterValues
        Dim pdvCustPattern As CrystalDecisions.Shared.ParameterDiscreteValue

        For Each pfd As CrystalDecisions.CrystalReports.Engine.ParameterFieldDefinition In rpt.DataDefinition.ParameterFields
            If pfd.Name.Contains("CP_") Then
                sTmp = pfd.Name.Replace("CP_", "")
                If IsNumeric(sTmp) Then

                    pvCustPattern = New CrystalDecisions.Shared.ParameterValues
                    pdvCustPattern = New CrystalDecisions.Shared.ParameterDiscreteValue

                    pdvCustPattern.Value = get_label(lingua_cod, CType(sTmp, Long), objParametri_Server)
                    pvCustPattern.Add(pdvCustPattern)
                    pfd.ApplyCurrentValues(pvCustPattern)
                End If
            End If

        Next



    End Sub


    Private Function get_label(ByVal lingua_Cod As Integer, ByVal FF_A_CrystalReports_CP_Label_COD As Integer, ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim etich As New FF_A_CrystalReports_CP_TraduzioniLabel_R
        Dim dtE As DataTable = etich.Leggi(
            FF_A_CrystalReports_CP_Label_COD,
            lingua_Cod,
            "",
            "",
            objParametri_Server
        )

        If dtE.Rows.Count > 0 Then
            Return dtE.Rows(0)("Traduzione")
        Else
            Return ""
        End If

    End Function

    Public Function LeggiEtichetta_Confezione_Full(
            ByVal id_agenda As Integer,
            ByVal StartCodeChar As String,
            ByVal EndCodeChar As String,
            ByVal StartAIChar As String,
            ByVal lSetCode128 As String,
            ByVal nRigheBarcode As Integer,
            ByVal infoCopie As String,
            ByVal barcodeType As String,
            ByVal lingua_cod As Integer,
            ByVal OModuli_Referenze_Config_Testata As Integer,
            ByVal iFF_Etichette_Tipo As FF_Etichette_tipo,
            ByVal xFiltroAggiuntivo As String,
            ByVal xOrderBy As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
            ByRef rpt As CrystalDecisions.CrystalReports.Engine.ReportDocument,
            ByRef DataSourceImpostato As Boolean
            ) As DataTable


        Dim dt As DataTable = LeggiEtichetta_Confezione(
            id_agenda, infoCopie, OModuli_Referenze_Config_Testata, iFF_Etichette_Tipo, barcodeType, xFiltroAggiuntivo, xOrderBy, objParametri)

        BarcodeCommon(barcodeType, lingua_cod, StartCodeChar, EndCodeChar, StartAIChar, lSetCode128, dt, rpt, DataSourceImpostato, objParametri)

        Return dt

    End Function

    Public Sub QRCodeCommom(ByRef dt As DataTable, Optional qrBlobColumnName As String = "QrBlob", Optional ByVal CampoHR As String = "CodBinHr", Optional ByVal cfg As FF_BarcodeType_Barcode = Nothing)

        'Dim qrGen As ZXing.IBarcodeWriter = New BarcodeWriter() With {.Format = BarcodeFormat.QR_CODE}
        Dim qrGen As New BarcodeWriter() With {.Format = BarcodeFormat.QR_CODE}

        dt.Columns.Add(New DataColumn(qrBlobColumnName, GetType(Byte())))
        For Each r As DataRow In dt.Rows

            Dim qrTest As String
            If cfg Is Nothing Then
                qrTest = If(IsDBNull(r.Item(CampoHR)), "", r.Item(CampoHR).ToString)
            Else
                qrTest = QRCodeGeneraStringa(r, cfg)
            End If

            If Not String.IsNullOrEmpty(qrTest) Then
                Dim qrCode As Drawing.Bitmap = qrGen.Write(qrTest)

                Dim memoryStream = New IO.MemoryStream()
                qrCode.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg)
                memoryStream.Flush()
                r(qrBlobColumnName) = memoryStream.ToArray

                'Dim File = New FileStream("C:\qrcode\test.jpg", FileMode.Create, FileAccess.Write)
                'memoryStream.WriteTo(File)
                'File.Close()
                memoryStream.Close()
            End If

        Next


    End Sub

    Public Function QRCodeGeneraStringa(ByVal r As DataRow, cfg As FF_BarcodeType_Barcode) As String

        Dim rval As String = ""
        Dim rval1 As New List(Of String)
        For Each c In cfg.ListaMappaDescrizioni
            rval1.Add(c.NomeCampoDescrizione & cfg.SeparatoreChiaveVaore & If(IsDBNull(r.Item(c.NomeCampoDt)), "", r.Item(c.NomeCampoDt).ToString))
        Next
        rval = String.Join(cfg.SeparatoreCampo, rval1)
        Return rval

    End Function

    Private Sub BarcodeCommon(
            ByVal BarcodeType As String,
            ByVal lingua_cod As Integer,
            ByVal StartCodeChar As String,
            ByVal EndCodeChar As String,
            ByVal StartAIChar As String,
            ByVal lSetCode128 As String,
            ByRef dt As DataTable,
            ByRef rpt As CrystalDecisions.CrystalReports.Engine.ReportDocument,
            ByRef dataSourceImpostato As Boolean,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        )


        If BarcodeType.Contains("{") Then


            Dim cfg As FF_BarcodeType =
            jSonToBarcodeType(BarcodeType)

            BarCodeCommonCode128(StartCodeChar, EndCodeChar, StartAIChar, lSetCode128, dt, cfg)

            BarcodeCommonPDF417(dt, cfg)

            QRCodeCommom(dt)

            If cfg.CP_ParametriCrystal Then
                rpt.SetDataSource(dt)
                dataSourceImpostato = True
                CP_ParametriCrystal(lingua_cod, rpt, objParametri)
            End If

        End If

    End Sub

    Private Sub BarcodeCommonPDF417(dt As DataTable, cfg As FF_BarcodeType)
        Dim bccPDF417 As Barcode_PDF417
        If cfg.PDF417 IsNot Nothing Then

            bccPDF417 = New Barcode_PDF417
            For Each cur_Cfg In cfg.PDF417

                dt.Columns.Add(New DataColumn(cur_Cfg.CampoA, GetType(Byte())))

                For Each r As DataRow In dt.Rows
                    Dim fileContents() As Byte = bccPDF417.CreaBarcode(PDF417Data(r), 1, 1)
                    r(cur_Cfg.CampoA) = fileContents
                Next
            Next

        End If
    End Sub

    Public Sub BarCodeCommonCode128(StartCodeChar As String, EndCodeChar As String, StartAIChar As String, lSetCode128 As String, dt As DataTable, cfg As FF_BarcodeType)
        Dim bcc As BarCodeCreator

        If cfg.Code128 IsNot Nothing Then

            bcc = New BarCodeCreator(StartCodeChar,
                                     EndCodeChar,
                                     StartAIChar,
                                     lSetCode128)

            For Each cur_Cfg In cfg.Code128

                For Each r As DataRow In dt.Rows


                    bcc.DataBlockCollection.IndiceDivisioneCodice = 3
                    bcc.DataBlockCollection.Add(New DataBlock(bcc.DataBlockCollection, cur_Cfg.Posizione, cur_Cfg.AI, bcc.CreateCode(cur_Cfg.Lunghezza, cur_Cfg.PadChar, {r(cur_Cfg.CampoDa)}), 1))

                    r(cur_Cfg.CampoA) = bcc.Code_Top

                Next
            Next
        End If

    End Sub

    Public Function LeggiEtichetta_Conferimento_Full(
        ByVal id_agenda As Integer,
        ByVal StartCodeChar As String,
        ByVal EndCodeChar As String,
        ByVal StartAIChar As String,
        ByVal lSetCode128 As String,
        ByVal nRigheBarcode As Integer,
        ByVal infoCopie As String,
        ByVal barcodeType As String,
        ByVal lingua_cod As Integer,
        ByVal OModuli_Referenze_Config_Testata As Integer,
        ByVal iFF_Etichette_Tipo As FF_Etichette_tipo,
        ByRef dataSourceImpostato As Boolean,
        ByVal numero_copie As Integer,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
        ByRef rpt As CrystalDecisions.CrystalReports.Engine.ReportDocument
        ) As DataTable




        Dim dt As DataTable = LeggiEtichetta_Conferimento(
            id_agenda,
            infoCopie,
            OModuli_Referenze_Config_Testata,
            iFF_Etichette_Tipo,
            barcodeType,
            numero_copie,
            xFiltroAggiuntivo,
            xOrderBy,
            objParametri
        )


        'Dim bcc As BarCodeCreator = New BarCodeCreator( _
        '    StartCodeChar, _
        '    EndCodeChar, _
        '    StartAIChar, _
        '    lSetCode128 _
        ')

        BarcodeCommon(
            barcodeType,
            lingua_cod,
            StartCodeChar,
            EndCodeChar,
            StartAIChar,
            lSetCode128,
            dt,
            rpt,
            dataSourceImpostato,
            objParametri
        )


        'Dim bccPDF417 As Barcode_PDF417

        'If barcodeType.Contains("PDF417") Then
        '    dt.Columns.Add(New DataColumn("Code2Dblob", GetType(Byte())))
        '    bccPDF417 = New Barcode_PDF417
        'End If

        'If barcodeType.Contains("CP_ParametriCrystal") Then
        '    CP_ParametriCrystal(lingua_cod, rpt, objParametri)
        'End If

        'For Each r In dt.Rows

        '    bcc.DataBlockCollection.IndiceDivisioneCodice = 3
        '    bcc.DataBlockCollection.Add(New DataBlock(bcc.DataBlockCollection, 1, "", bcc.CreateCode(20, r("codBIN")), 1))

        '    If barcodeType.Contains("PDF417") Then
        '        Dim fileContents() As Byte = bccPDF417.CreaBarcode(PDF417Data(r))
        '        r("Code2Dblob") = fileContents

        '    End If

        '    r("codBIN") = bcc.Code_Top

        'Next


        Return dt

    End Function

    Private Function jSonToBarcodeType(ByVal jSonCFG As String) As FF_BarcodeType

        Dim serializer1 As New JavaScriptSerializer()
        Dim rval As FF_BarcodeType
        rval = serializer1.Deserialize(Of FF_BarcodeType)(jSonCFG)

        Dim t As New FF_BarcodeType
        t.Code128 = New List(Of FF_BarcodeType_Barcode)
        t.Code128.Add(New FF_BarcodeType_Barcode With {.CampoDa = "A", .CampoA = "B"})
        t.Code128.Add(New FF_BarcodeType_Barcode With {.CampoDa = "D", .CampoA = "E"})

        t.CP_ParametriCrystal = True

        Dim aaa As String = serializer1.Serialize(t)

        'rval = ( _
        '    From c In ojCFG("BarcodeConfig") _
        '    Select New FF_BarcodeType With { _
        '        .Code128 = ( _
        '            From c128 In c("Code128: BarCode") _
        '            Select New FF_BarcodeType_Barcode With { _
        '                .CampoDa = c128("CampoDa"), _
        '                .CampoA = c128("CampoA") _
        '            }).ToList _
        '        , .PDF417 = ( _
        '            From cPDF417 In c("PDF417: BarCode") _
        '            Select New FF_BarcodeType_Barcode With { _
        '                .CampoDa = cPDF417("CampoDa"), _
        '                .CampoA = cPDF417("CampoA") _
        '            }).ToList _
        '        , .CP_ParametriCrystal = CBool(c("CP_ParametriCrystal")) _
        '    }).FirstOrDefault

        Return rval


    End Function

    Public Function LeggiParametriQualitativi(
                ByVal id_agenda As Integer,
                ByVal OModuli_Referenze_Config_Testata As Integer,
                ByVal iFF_Etichette_Tipo As FF_Etichette_tipo,
                ByVal caumov_det As String,
                ByVal xFiltroAggiuntivo As String,
                ByVal xOrderBy As String,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
            ) As DataTable

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim NomeRoutine As String = "LeggiParametriQualitativi"

        Try

            stb.Length = 0

            Dim piva As String = GetPiva(id_agenda, objParametri, NomeRoutine, stb)
            Dim lav_cod As Integer = GetLav_cod(id_agenda, objParametri, NomeRoutine, stb)
            Dim modulo As Integer = GetModulo(id_agenda, objParametri, NomeRoutine, stb)

            Dim caumov_tes As String = GetCaumov_tes_datoLav_Cod(lav_cod)
            ' Dim caumov_det As String = GetCaumov_det_datoLav_Cod(lav_cod)

            Dim joinRisum As Boolean = True

            If lav_cod = LAVCOD_TRASFORMAZIONI OrElse
               lav_cod = LAVCOD_DISTINTA_CARICO OrElse
               lav_cod = LAVCOD_DISTINTA_CARICO_ACCETTAZIONE Then

                joinRisum = False

            End If

            Dim oCFG As OModuli_Referenze_Config_Testata_obj
            oCFG = leggiParametriOmniFF_DatoOModuli_Referenze_Config_Dettagli(piva,
                OModuli_Referenze_Config_Testata,
                objParametri
            )


            stb.Append("SELECT  " & vbCrLf)
            stb.Append("   case when  detProd.Mov_Det_Des = '' then veg.veg_des else  detProd.Mov_Det_Des + ' (' + coalesce(veg.veg_des, '') + ')' end as Specie" & vbCrLf)
            stb.Append(" , cul.Cul_Des as Varieta" & vbCrLf)


            stb.Append(" , matProd.Mat_Des as Referenza" & vbCrLf)
            stb.Append(" , mTest.Data_Movimento as Data" & vbCrLf)
            stb.Append(" , mTest.Ora as Ora" & vbCrLf)
            stb.Append(" , detProd.Qta * detProd.Qta_Extra as QtaKg" & vbCrLf)


            'inizio parametri Omni

            leggiParametriOmniFF_SELECT_DatoOModuli_Referenze_Config_Dettagli(
               iFF_Etichette_Tipo, True, oCFG, False, stb, False, True
            )

            '  Vanni, 30/10/2015 16:21:59: quando si tratta di accettazione allora il numero di bolla deve essere letto dal movimento secondario
            Dim Accettazione_MovimentoSecondario As Boolean = False
            If modulo = 2 Then
                Accettazione_MovimentoSecondario = True
            End If


            LeggiDatiEtichetta_From(caumov_det, caumov_tes, False, joinRisum, Accettazione_MovimentoSecondario, stb)

            leggiParametriOmniFF_FROM_DatoOModuli_Referenze_Config_Dettagli(oCFG, stb)


            stb.Append(" WHERE  A.id_Agenda  = " & Agro_SQL_SaveNum(id_agenda) & vbCrLf)


            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.Append(" AND   A.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.Append(" AND   A.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception


            'uso questa funzione per ottenere il Messaggio..:
            MessaggioErrore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing

            'passo l'eccezione corrente al throw come inner exception ..:
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)



        End Try

        Return DT

    End Function

    Public Function LeggiEtichetta_Conferimento(
                ByVal id_agenda As Integer,
                ByVal infoCopie As String,
                ByVal OModuli_Referenze_Config_Testata As Integer,
                ByVal iFF_Etichette_Tipo As FF_Etichette_tipo,
                ByVal BarcodeType As String,
                ByVal numero_copie As Integer,
                ByVal xFiltroAggiuntivo As String,
                ByVal xOrderBy As String,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
            ) As DataTable


        'per lettura config di estrazione dati...
        Dim cfg As FF_BarcodeType =
           jSonToBarcodeType(BarcodeType)



        '----- Descrizione
        Dim NomeRoutine As String = "FF_Etichette_R.LeggiEtichetta_Pallet()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Dim piva As String = GetPiva(id_agenda, objParametri, NomeRoutine, stb)
            Dim lav_cod As Integer = GetLav_cod(id_agenda, objParametri, NomeRoutine, stb)
            Dim modulo As Integer = GetModulo(id_agenda, objParametri, NomeRoutine, stb)

            Dim caumov_det As String = GetCaumov_det_datoModulo(modulo)

            Dim joinRisum As Boolean = True

            If lav_cod = LAVCOD_DISTINTA_CARICO OrElse lav_cod = LAVCOD_DISTINTA_CARICO_ACCETTAZIONE OrElse lav_cod = LAVCOD_TESTATE_ORDINE_LAVORAZIONE Then
                joinRisum = False
            End If

            Dim oCFG As OModuli_Referenze_Config_Testata_obj
            oCFG = leggiParametriOmniFF_DatoOModuli_Referenze_Config_Dettagli(
                piva, OModuli_Referenze_Config_Testata,
                objParametri
            )

            stb.Length = 0

            stb.Append("SELECT  " & vbCrLf)

            Select Case lav_cod
                Case LAVCOD_TRASFORMAZIONI, LAVCOD_TESTATE_ORDINE_LAVORAZIONE
                    stb.Append("   contattifornitore.Rag_Soc as RSFornitore " & vbCrLf)
                Case Else
                    stb.Append("   cCli.Rag_Soc as RSFornitore" & vbCrLf)
            End Select
            stb.Append(" , case when  detProd.Mov_Det_Des = '' then veg.veg_des else  detProd.Mov_Det_Des + ' (' + coalesce(veg.veg_des, '') + ')' end as Specie" & vbCrLf)
            stb.Append(" , cul.Cul_Des as Varieta" & vbCrLf)



            Select Case cfg.Code128.FirstOrDefault.Algoritmo

                Case FF_Barcode_Algoritmo_Dati.Nessuno
                    stb.Append(" , '' as CodBin" & vbCrLf)

                Case FF_Barcode_Algoritmo_Dati.RiportaLotto
                    stb.Append(" , detProd.lotto as CodBin" & vbCrLf)

                Case FF_Barcode_Algoritmo_Dati.Cofruta
                    ' Stefano 11/5/17 
                    '  questo aveva senso con la vecchia logica dei lotti
                    '  stb.Append(" , 'C' + detProd.lotto + '000001A' as CodBin" & vbCrLf)
                    stb.Append(" , detProd.lotto as CodBin" & vbCrLf)

                Case FF_Barcode_Algoritmo_Dati.MiniFrutta
                    stb.Append(" , detProd.lotto as CodBin" & vbCrLf)

            End Select


            stb.Append(" , matProd.Mat_Des as Referenza" & vbCrLf)
            stb.Append(" , mTest.Data_Movimento as Data" & vbCrLf)
            stb.Append(" , mTest.Ora as Ora" & vbCrLf)
            stb.Append(" , '' as DaAnalizzare" & vbCrLf)
            stb.Append(" , coalesce(cCliCodSocio.Val_Cod, risumCli.settore_des) as CodFornitore" & vbCrLf)
            stb.Append(" , '' as CodAppezzamento" & vbCrLf)
            stb.Append(" , '' as Disciplinare" & vbCrLf)
            stb.Append(" , '' as PesoNettoXPallet" & vbCrLf)

            stb.Append("  , coalesce( cast(detProd.Qta_Dettaglio2 as integer) , 1 ) AS Numero_Pedane " & vbCrLf)
            stb.Append("  , coalesce( cast(detProd.Qta_Dettaglio1 as integer), 1)  AS Numero_Imballi " & vbCrLf)

            stb.Append(" , coalesce(detProd.Extra_Str, '00') as Bolla_Riga " & vbCrLf)


            'inizio parametri Omni

            leggiParametriOmniFF_SELECT_DatoOModuli_Referenze_Config_Dettagli(
               iFF_Etichette_Tipo, True, oCFG, False, stb
            )

            'leggiParametriOmniFF_Select("Calibro", stb)
            'leggiParametriOmniFF_Select("Qualità", stb, "qualita")
            'leggiParametriOmniFF_Select("imballaggio", stb, "Imballo_Descrizione")
            'Fine Parametri Omni

            '  Giulia, 06/10/2016 17.19.08: le note vengo già tirate su dalla funzione poche righe più su
            'stb.Append(" , '' as Note" & vbCrLf)


            Select Case cfg.Code128.FirstOrDefault.Algoritmo

                Case FF_Barcode_Algoritmo_Dati.Nessuno
                    stb.Append(" , '' as CodBinHR" & vbCrLf)

                Case FF_Barcode_Algoritmo_Dati.RiportaLotto
                    stb.Append(" , detProd.lotto as CodBinHR" & vbCrLf)

                Case FF_Barcode_Algoritmo_Dati.Cofruta
                    ' Stefano 11/5/17 
                    '  questo aveva senso con la vecchia logica dei lotti
                    '  stb.Append(" , 'C' + detProd.lotto + '000001A' as CodBinHR" & vbCrLf)
                    stb.Append(" , detProd.lotto as CodBinHR" & vbCrLf)

                Case FF_Barcode_Algoritmo_Dati.MiniFrutta
                    stb.Append(" , detProd.lotto as CodBinHR" & vbCrLf)

            End Select



            stb.Append(" , detProd.lotto as Lotto  " & vbCrLf)
            stb.Append(" , matProd.Cod_Articolo  " & vbCrLf)

            stb.Append(" , detProd.Cal_Cod " & vbCrLf)

            stb.Append(vbCrLf)


            '  Vanni, 30/10/2015 16:21:59: quando si tratta di accettazione allora il numero di bolla deve essere letto dal movimento secondario
            Dim Accettazione_MovimentoSecondario As Boolean = False
            If modulo = 2 Then
                Accettazione_MovimentoSecondario = True
            End If

            leggiParametriOmniFF_SelectNDOC(Movimenti_Alias_mTestata, "Bolla_Numero", stb)

            If Accettazione_MovimentoSecondario Then
                leggiParametriOmniFF_SelectNDOC(Movimenti_Alias_mBolla, "DDT_Numero", stb)
            End If

            Dim cauMovTestata As Integer = 0
            Select Case lav_cod
                Case LAVCOD_TRASFORMAZIONI, LAVCOD_TESTATE_ORDINE_LAVORAZIONE
                    cauMovTestata = CAU_LINEA_PRODUZIONE
                Case Else
                    cauMovTestata = CAU_REGISTRAZIONI

            End Select

            LeggiDatiEtichetta_From(caumov_det, cauMovTestata.ToString, False, joinRisum, Accettazione_MovimentoSecondario, stb)

            leggiParametriOmniFF_FROM_DatoOModuli_Referenze_Config_Dettagli(oCFG, stb)

            stb.Append(getJoinCopie(infoCopie))

            stb.Append(" WHERE  A.id_Agenda  = " & Agro_SQL_SaveNum(id_agenda) & vbCrLf)




            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.Append(" AND   A.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.Append(" AND   A.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception


            'uso questa funzione per ottenere il Messaggio..:
            MessaggioErrore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing

            'passo l'eccezione corrente al throw come inner exception ..:
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)



        End Try

        ' Personalizzazione per Mini: viene forzato come numero imballi quelli digitati sulla riga
        Select Case cfg.Code128.FirstOrDefault.Algoritmo
            Case FF_Barcode_Algoritmo_Dati.MiniFrutta
                For Each dr In DT.Rows
                    dr(DT.Columns("Numero_Pedane")) = numero_copie
                Next
        End Select


        Return DT

    End Function


    Private Shared Sub LeggiDatiEtichetta_From_Movimenti(ByVal cau_mov As String, ByVal AliasTab As String, ByVal stb As System.Text.StringBuilder)

        stb.Append("  " & vbCrLf)
        stb.Append("    inner join Movimenti  " & AliasTab & vbCrLf)
        stb.Append("        on a.Id_Agenda = " & AliasTab & ".Id_Agenda  " & vbCrLf)
        stb.Append("            and a.PIVA = " & AliasTab & ".piva  " & vbCrLf)

        If AliasTab = Movimenti_Alias_mProd Then
            stb.Append("            and " & AliasTab & ".cau_mov IN (")
            Dim causaliMov = cau_mov.Split(",").ToList()
            For Each c As String In causaliMov
                stb.Append(String.Format("'{0}',", c))
            Next
            stb.Remove(stb.Length - 1, 1)
            stb.Append(" ) " & vbCrLf)
        Else
            stb.Append("            and " & AliasTab & ".cau_mov = '" & cau_mov & "' " & vbCrLf)
        End If

        stb.Append("  " & vbCrLf)

    End Sub


    Private Shared Sub LeggiDatiEtichetta_xWaTable_v2_GetQuery(ByVal id_agenda As Integer, ByRef stb As System.Text.StringBuilder)

        stb.Append(" select " & vbCrLf)
        stb.Append("   cast(PRNs.FF_Stampanti_cod as varchar(100)) + '" & SeparatoreCodiceQRY & "'+ coalesce(cast(CFGstampa.Id_Mov_Det as varchar(100)), '0') as Codice" & vbCrLf)
        stb.Append(" , Descrizione " & vbCrLf)
        stb.Append(" , PRNs.Nome_Per_Stampa " & vbCrLf)
        stb.Append(" , CFGstampa.Id_Mov_Det" & vbCrLf)
        stb.Append(" , coalesce(CFGStampa.NumeroEtichettePedana, 1) as NumeroEtichettePedana " & vbCrLf)
        stb.Append(" , coalesce(CFGStampa.NumeroEtichetteImballi, 1) as NumeroEtichetteImballi " & vbCrLf)
        stb.Append(" , coalesce(CFGStampa.FF_Stampa_Dettagli_COD, 0) as FF_Stampa_Dettagli_COD " & vbCrLf)
        stb.Append(" , PRNs.FF_Stampanti_cod " & vbCrLf)
        stb.Append(" , 0 as Inviato " & vbCrLf)
        stb.Append(" , cast(PRNs.FF_Stampanti_cod as varchar(100)) + '" & SeparatoreCodiceQRY & "'+ coalesce(cast(CFGStampa.FF_Stampa_Dettagli_COD as varchar(100)), '0') as FF_Stampanti_cod_FF_Stampa_Dettagli_COD " & vbCrLf)

        stb.Append(" from FF_stampanti PRNs " & vbCrLf)

        stb.Append(" left join ( " & vbCrLf)

        stb.Append("    select CFGs.FF_Stampanti_Cod " & vbCrLf)
        stb.Append("         , DD.Id_Mov_Det " & vbCrLf)
        stb.Append("         , CFGs.FF_Stampa_Dettagli_COD " & vbCrLf)
        stb.Append("         , coalesce( cast(DD.Qta_Dettaglio2 as integer) , 1 ) AS NumeroEtichettePedana" & vbCrLf)
        stb.Append("         , coalesce( cast(DD.Qta_Dettaglio1 as integer), 1)  AS NumeroEtichetteImballi" & vbCrLf)

        stb.Append("    from FF_ConfigurazioniDiStampa_Dettagli CFGs         " & vbCrLf)
        stb.Append("    inner join movimenti_dettagli DD " & vbCrLf)
        stb.Append("        on DD.ID_Attivita = CFGs.FF_Stampa_Dettagli_Cod " & vbCrLf)

        stb.Append("    inner join Agenda a " & vbCrLf)
        stb.Append("        on a.Id_Agenda = dd.id_agenda " & vbCrLf)

        stb.Append("    where a.id_agenda = " & id_agenda & vbCrLf)


        stb.Append(" ) CFGstampa " & vbCrLf)
        stb.Append("    on CFGstampa.FF_Stampanti_Cod = PRNs.FF_Stampanti_Cod " & vbCrLf)
        stb.Append(" ")


    End Sub

    Private Shared Sub LeggiDatiEtichetta_From(ByVal cau_mov_Prodotti As String, ByVal cau_mov_testata As String, ByVal JoinOrdine As Boolean, ByVal joinRisorseUmane As Boolean, ByVal Accettazione_MovimentoSecondario As Boolean, ByRef stb As System.Text.StringBuilder)

        Dim joinRisum As String
        If joinRisorseUmane Then
            joinRisum = "INNER JOIN "
        Else
            joinRisum = "LEFT JOIN "
        End If

        stb.Append("  " & vbCrLf)
        stb.Append("  FROM Agenda A  " & vbCrLf)

        LeggiDatiEtichetta_From_Movimenti(cau_mov_Prodotti, Movimenti_Alias_mProd, stb)

        If cau_mov_testata <> "" Then
            LeggiDatiEtichetta_From_Movimenti(cau_mov_testata, Movimenti_Alias_mTestata, stb)
        End If

        If Accettazione_MovimentoSecondario Then
            LeggiDatiEtichetta_From_Movimenti(CAU_REGISTRAZIONE_SECONDARIA, Movimenti_Alias_mBolla, stb)
        End If

        'LeggiDatiEtichetta_From_Movimenti(CAU_CARICO, Movimenti_Alias_mImballi, stb)

        stb.Append("    inner join Imprese i " & vbCrLf)
        stb.Append("        on i.piva = a.PIVA  " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("    left join impresexindirizzi id " & vbCrLf)
        stb.Append("        on id.piva = i.piva " & vbCrLf)
        stb.Append("        and id.Tipo_Indirizzo = 1 " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("    left join Indirizzi ind " & vbCrLf)
        stb.Append("        on id.cod_indirizzo = ind.cod_indirizzo      " & vbCrLf)
        stb.Append(" ")

        If cau_mov_testata <> "" Then
            stb.Append("    " & joinRisum & " Risorse_Umane risumCli " & vbCrLf)
            stb.Append("        on risumCli.Cod_RisUm = mTest.Cod_RisUm  " & vbCrLf)
            stb.Append("  " & vbCrLf)

            stb.Append("    " & joinRisum & " Contatti cCli " & vbCrLf)
            stb.Append("        on cCli.Cod_Contatto = risumCli.Cod_Contatto  " & vbCrLf)
            stb.Append("        and cCli.Piva = risumCli.Piva  " & vbCrLf)
            stb.Append("  " & vbCrLf)

            stb.Append("        left join imprese iCli " & vbCrLf)
            stb.Append("        on iCli.piva = cCli.Cod_Contatto " & vbCrLf)
            stb.Append("  " & vbCrLf)

            stb.Append("     left join Imprese_codici cCliCodSocio  " & vbCrLf)
            stb.Append("         on iCli.piva = cCliCodSocio.PIVA        " & vbCrLf)
            stb.Append("         and cCliCodSocio.id_cod = " & enum_CodiciAnagrafe.Codice_Socio & vbCrLf)

        End If





        stb.Append("    inner join Movimenti_dettagli detProd " & vbCrLf)
        stb.Append("        on detProd.PIVA = mProd.piva  " & vbCrLf)
        stb.Append("        and detProd.Id_Agenda = mProd.Id_Agenda  " & vbCrLf)
        stb.Append("        and detProd.Id_Mov = mProd.Id_Mov  " & vbCrLf)
        stb.Append("  " & vbCrLf)

        'stb.Append("    inner join Movimenti_dettagli detImball " & vbCrLf)
        'stb.Append("        on detImball.PIVA = mImball.piva  " & vbCrLf)
        'stb.Append("        and detImball.Id_Agenda = mImball.Id_Agenda  " & vbCrLf)
        'stb.Append("        and detImball.Id_Mov = mImball.Id_Mov  " & vbCrLf)
        'stb.Append("  " & vbCrLf)

        stb.Append("    left join mov_dettaglio_tecnico_extra extraProd " & vbCrLf)
        stb.Append("        on extraProd.PIVA = detProd.piva  " & vbCrLf)
        stb.Append("        and extraProd.Id_Agenda = detProd.Id_Agenda  " & vbCrLf)
        stb.Append("        and extraProd.Id_Mov = detProd.Id_Mov  " & vbCrLf)
        stb.Append("        and extraProd.Id_Mov_det = detProd.Id_Mov_det " & vbCrLf)
        stb.Append("  " & vbCrLf)

        stb.Append("    inner join materie_Prime matProd " & vbCrLf)
        stb.Append("        on matProd.Mat_Cod = detProd.Mat_Cod  " & vbCrLf)
        stb.Append("        and matProd.Elem_Cod = detProd.Elem_Cod  " & vbCrLf)
        stb.Append("        and matProd.Piva = mprod.piva  " & vbCrLf)
        stb.Append("  " & vbCrLf)

        'stb.Append("    inner join materie_Prime matImball " & vbCrLf)
        'stb.Append("        on matImball.Mat_Cod = detImball.Mat_Cod  " & vbCrLf)
        'stb.Append("        and matImball.Elem_Cod = detImball.Elem_Cod  " & vbCrLf)
        'stb.Append("        and matImball.Piva = detImball.piva  " & vbCrLf)
        'stb.Append("  " & vbCrLf)

        stb.Append("    left join Materie_Prime_Campionature cc " & vbCrLf)
        stb.Append("        on cc.Progressivo = detProd.Cal_Cod  " & vbCrLf)
        stb.Append("        and cc.Tipo = 'OFornitore' " & vbCrLf)

        stb.Append("    left join Cultivar cul " & vbCrLf)
        stb.Append("        on cul.Cul_Cod = matProd.Cul_Cod  " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("    left join SpecieVegetali veg " & vbCrLf)
        stb.Append("        on veg.Veg_Cod = matProd.Veg_Cod  " & vbCrLf)
        stb.Append(" ")

        If cau_mov_testata = "" Then
            leggiParametriOmniFF_FROM("Cliente", stb)

            stb.Append("    " & joinRisum & " Risorse_Umane risumCli " & vbCrLf)
            stb.Append("        on risumCli.Cod_RisUm = cCliente.Tipo_Cod  " & vbCrLf)
            stb.Append("  " & vbCrLf)

            stb.Append("    " & joinRisum & " Contatti cCli " & vbCrLf)
            stb.Append("        on cCli.Cod_Contatto = risumCli.Cod_Contatto  " & vbCrLf)
            stb.Append("        and cCli.Piva = risumCli.Piva  " & vbCrLf)
            stb.Append("  " & vbCrLf)
        End If



        If JoinOrdine Then

            stb.Append("    left join Mov_Dettagli_Riferimenti riff " & vbCrLf)
            stb.Append("        on riff.Piva = A.PIVA  " & vbCrLf)
            stb.Append("        and riff.Sa_Cod = a.Sa_Cod  " & vbCrLf)
            stb.Append("        and riff.Id_Mov = detProd.Id_Mov  " & vbCrLf)
            stb.Append("        and riff.Id_Mov_Det = detProd.Id_Mov_Det  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("     left join Movimenti_dettagli detRiff " & vbCrLf)
            stb.Append("      on riff.Piva_Rif  = detRiff.PIVA  " & vbCrLf)
            stb.Append("        and riff.id_agenda_rif = detRiff.id_agenda " & vbCrLf)
            stb.Append("        and riff.Sa_Cod_Rif  = detRiff.Sa_Cod  " & vbCrLf)
            stb.Append("        and riff.Id_Mov_Rif  = detRiff.Id_Mov  " & vbCrLf)
            stb.Append("        and riff.Id_Mov_Det_Rif = detRiff.Id_Mov_Det  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("    left join movimenti mRiff " & vbCrLf)
            stb.Append("        on mriff.Piva  = detRiff.PIVA " & vbCrLf)
            stb.Append("        and mriff.Id_Agenda = detRiff.Id_Agenda   " & vbCrLf)
            stb.Append("        and mriff.Sa_Cod = detRiff.Sa_Cod        " & vbCrLf)
            stb.Append("        and mriff.cau_mov= '4000'")

        End If


    End Sub

    Public Sub leggiParametriOmniFF_SelectNDOC(ByVal AliasTab As String, ByVal NomeCampo As String, ByRef stb As StringBuilder)

        'stb.Append(" , " & AliasTab & ".Doc_Numero_Sin + case when " & AliasTab & ".Doc_Numero_Sin <> '' then '/' else '' end + cast(" & AliasTab & ".Doc_Numero as varchar(100)) + case when " & AliasTab & ".Doc_Numero_Des <> '' then '/' else '' end + " & AliasTab & ".Doc_Numero_DES as Bolla_Numero " & vbCrLf)

        stb.Append(" , replace(" & AliasTab & ".Doc_Numero_Sin, '/', '') +  cast(" & AliasTab & ".Doc_Numero as varchar(100)) + replace(" & AliasTab & ".Doc_Numero_DES, '/','') as " & NomeCampo & vbCrLf)

    End Sub

    'Public Shared Sub leggiParametriOmniFF_Select_CatSTR(ByVal parametro As String, ByRef stb As StringBuilder, Optional ByVal lst As String = "+", Optional ByVal Descrizione_Oppure_Sigla As String = "Descrizione")
    '    stb.Append(" coalesce(otp" & leggiParametriOmniFF_FROM_tab(parametro) & "." & Descrizione_Oppure_Sigla & ", '') + '-' " & lst & vbCrLf)
    'End Sub

    Public Sub leggiParametriOmniFF_Select_Codice(ByVal iFF_Etichette_tipo As FF_Etichette_tipo, ByVal cfg As OModuli_Referenze_Config_Dettagli_obj, ByRef stb As StringBuilder, ByVal CatStr As Boolean, Optional ByVal lst As String = "+")

        Dim Prefisso As String = "otp"

        Dim startStr As String = " , "
        Dim endCatStr As String = " as "

        stb.Append(startStr & Prefisso & leggiParametriOmniFF_FROM_tab(cfg.Tabella_Key) & ".Tabella_Par_Cod" & endCatStr & cfg.Tabella_Key & "_Codice")

    End Sub

    Public Sub leggiParametriOmniFF_Select(ByVal iFF_Etichette_tipo As FF_Etichette_tipo,
                                           ByVal cfg As OModuli_Referenze_Config_Dettagli_obj,
                                           ByRef stb As StringBuilder,
                                           ByVal CatStr As Boolean,
                                           Optional ByVal Descrizione_Oppure_Sigla As String = "Descrizione",
                                           Optional ByVal lst As String = "+")

        Dim Prefisso As String = "c"
        If cfg.Tipo <> 2 Then
            Prefisso = "otp"
        End If

        Dim startStr As String = " , "
        Dim endCatStr As String = ", '') + '-' " & lst & vbCrLf
        If CatStr Then
            startStr = " coalesce ("
        Else
            endCatStr = " as "
        End If

        Dim DettaglioEtichettaCampi As OModuli_Referenze_Config_Dettagli_Label_obj = (
            From eTi In cfg.CFG_Etichette
            Where eTi.IFF_Etichette_tipo = iFF_Etichette_tipo
            Select eTi
        ).FirstOrDefault

        stb.Append(startStr & Prefisso & leggiParametriOmniFF_FROM_tab(cfg.Tabella_Key) & "." & Descrizione_Oppure_Sigla & endCatStr)

        Dim AliasCol As String = ""
        If DettaglioEtichettaCampi IsNot Nothing Then
            AliasCol = DettaglioEtichettaCampi.NomeCampoAlias
        Else
            If Descrizione_Oppure_Sigla = "Sigla" Then
                AliasCol = cfg.Tabella_Key & "_Sigla"
            End If
        End If

        If Not CatStr Then
            If Not String.IsNullOrEmpty(AliasCol) Then
                stb.Append(AliasCol & vbCrLf)
            Else
                stb.Append(cfg.Tabella_Key & vbCrLf)
            End If
        End If





    End Sub

    'Public Sub leggiParametriOmniFF_Select(ByVal parametro As String, ByRef stb As StringBuilder, Optional ByVal AliasCol As String = "", Optional ByVal Descrizione_Oppure_Sigla As String = "Descrizione")

    '    stb.Append(" , otp" & leggiParametriOmniFF_FROM_tab(parametro) & "." & Descrizione_Oppure_Sigla & " as ")

    '    If Not String.IsNullOrEmpty(AliasCol) Then
    '        stb.Append(AliasCol & vbCrLf)
    '    Else
    '        stb.Append(parametro & vbCrLf)
    '    End If


    'End Sub

    Public Shared Function leggiParametriOmniFF_FROM_tab(ByVal Parametro As String) As String
        Return Replace(Parametro, " ", "_")
    End Function

    Public Shared Sub leggiParametriOmniFF_FROM(ByVal Parametro As String, ByRef stb As StringBuilder, Optional ByVal Param_Cli_For As String = "")

        If String.IsNullOrEmpty(Param_Cli_For) Then
            'OModuli_Referenze_Config_Testata (OFiltro_veg_Cod (elenco di specie con separatore) )
            'OModuli_Referenze_Config_Dettagli OChkEtichetta = 1 --> va stampata in etichetta

            stb.Append(vbCrLf)
            stb.Append(" left join Materie_Prime_Campionature c" & leggiParametriOmniFF_FROM_tab(Parametro) & " " & vbCrLf)
            stb.Append("         on c" & leggiParametriOmniFF_FROM_tab(Parametro) & ".Progressivo = detProd.Cal_Cod   " & vbCrLf)
            stb.Append("         and c" & leggiParametriOmniFF_FROM_tab(Parametro) & ".Tipo = 'O" & Parametro & "'  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("    left join OTabelle ot" & leggiParametriOmniFF_FROM_tab(Parametro) & " " & vbCrLf)
            stb.Append("        on 'O' + ot" & leggiParametriOmniFF_FROM_tab(Parametro) & ".Tabella_Des = c" & leggiParametriOmniFF_FROM_tab(Parametro) & ".Tipo  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("    left join OTabelle_Parametri otp" & leggiParametriOmniFF_FROM_tab(Parametro) & " " & vbCrLf)

            '  Vanni, 30/10/2015 15:09:04: su indicazione di Lucchi commento la parte che manda in join la piva
            'stb.Append("        on otp" & Parametro & ".Piva = ot" & Parametro & ".Piva  " & vbCrLf)
            stb.Append("        on otp" & leggiParametriOmniFF_FROM_tab(Parametro) & ".Tabella_Par_Cod = c" & leggiParametriOmniFF_FROM_tab(Parametro) & ".tipo_cod" & vbCrLf)

            ' VAnni: 6/3/2020: su indicazione di Giulia aggiungo il join sulla OTabella altrimenti se ci sono prodotti sullo stesso tipo_cod duplica le letture.
            stb.Append("  and otp" & leggiParametriOmniFF_FROM_tab(Parametro) & ".Tabella_Cod =  ot" & leggiParametriOmniFF_FROM_tab(Parametro) & ".Tabella_Cod " & vbCrLf)
        Else

            If Param_Cli_For = "F" Then
                stb.AppendLine(" left join Materie_Prime_Campionature c" & leggiParametriOmniFF_FROM_tab(Parametro) & " ")
                stb.AppendLine("         on c" & leggiParametriOmniFF_FROM_tab(Parametro) & ".Progressivo = detProd.Cal_Cod   ")
                stb.AppendLine("         and c" & leggiParametriOmniFF_FROM_tab(Parametro) & ".Tipo = 'O" & Parametro & "'  ")
                stb.AppendLine("    left join risorse_umane r_u" & leggiParametriOmniFF_FROM_tab(Parametro) & " ")
                stb.AppendLine("        on r_u" & leggiParametriOmniFF_FROM_tab(Parametro) & ".Cod_RisUm = c" & leggiParametriOmniFF_FROM_tab(Parametro) & ".Tipo_Cod  ")
                stb.AppendLine("    left join contatti contatti" & leggiParametriOmniFF_FROM_tab(Parametro) & " ")
                stb.AppendLine("        on contatti" & leggiParametriOmniFF_FROM_tab(Parametro) & ".Cod_Contatto = r_u" & leggiParametriOmniFF_FROM_tab(Parametro) & ".Cod_Contatto  ")
            End If
        End If
    End Sub

    Private Sub LeggiEtichetta_Sintetica_getQuery(
        ByVal piva As String,
        ByVal id_agenda As Integer,
        ByVal cau_mov_Prodotti As String,
        ByVal cau_mov_testata As String,
        ByVal OModuli_Referenze_Config_Testata As Integer,
        ByVal iFF_Etichette_Tipo As FF_Etichette_tipo,
        ByVal joinRisum As Boolean,
        ByRef stb As System.Text.StringBuilder,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    )

        Dim oCFG As OModuli_Referenze_Config_Testata_obj
        oCFG = leggiParametriOmniFF_DatoOModuli_Referenze_Config_Dettagli(piva,
            OModuli_Referenze_Config_Testata,
            objParametri
        )

        stb.Length = 0

        stb.Append("SELECT  " & vbCrLf)
        stb.Append("     detProd.id_mov_det AS codice" & vbCrLf)

        stb.Append("   , matProd.Mat_Des  AS [descrizioneConfezione] " & vbCrLf)

        stb.Append("   , '' AS[codiceSpecie]  " & vbCrLf)
        stb.Append("   , coalesce(veg.Veg_Des , '') AS[specie]  " & vbCrLf)
        stb.Append("   , '' AS[codiceVarieta]  " & vbCrLf)
        stb.Append("   , coalesce(cul.cul_des , '') AS[varieta] ")


        'stb.Append("  , coalesce( extraProd.num_contenitori , 1 ) AS NumeroEtichettePedana" & vbCrLf)
        'stb.Append("  , coalesce( extraProd.num_colli, 1)  AS NumeroEtichetteImballi" & vbCrLf)


        stb.Append("  , coalesce( cast(detProd.Qta_Dettaglio2 as integer) , 1 ) AS NumeroEtichettePedana" & vbCrLf)
        stb.Append("  , coalesce( cast(detProd.Qta_Dettaglio1 as integer), 1)  AS NumeroEtichetteImballi" & vbCrLf)

        stb.Append(" , coalesce(detProd.ID_Attivita, 0) as FF_Stampa_Dettagli_Cod " & vbCrLf)

        stb.Append(" , matProd.Mat_Des + '-' + " & vbCrLf)
        stb.Append("   coalesce(cCli.Rag_Soc, '') + '-' + " & vbCrLf)

        'Inizio Parametri Omni
        leggiParametriOmniFF_SELECT_DatoOModuli_Referenze_Config_Dettagli(
            iFF_Etichette_Tipo, False, oCFG, True, stb
        )

        stb.Append("   As DescrizioneXwaTable " & vbCrLf)

        stb.Append("  " & vbCrLf)
        LeggiDatiEtichetta_From(cau_mov_Prodotti, cau_mov_testata, False, joinRisum, False, stb)

        leggiParametriOmniFF_FROM_DatoOModuli_Referenze_Config_Dettagli(oCFG, stb)

        stb.Append(" WHERE  A.id_Agenda  = " & Agro_SQL_SaveNum(id_agenda) & " AND detProd.Ordine_Det <> 1000 " & vbCrLf)

        If cau_mov_testata = CAU_LINEA_PRODUZIONE Then
            stb.Append(" AND detProd.Extra_Str = '' AND detprod.Extra_Int = 0")
        End If

    End Sub
    Private Shared Function GetCaumov_tes_datoLav_Cod(ByVal lav_cod As Integer) As String
        Dim caumov_det As String

        Select Case lav_cod
            Case LAVCOD_TRASFORMAZIONI, LAVCOD_TESTATE_ORDINE_LAVORAZIONE
                caumov_det = "10001"

            Case 5001
                caumov_det = ""

            Case Else
                caumov_det = "4000"

        End Select
        Return caumov_det
    End Function

    Private Shared Function GetCaumov_det_datoLav_Cod(ByVal lav_cod As Integer) As String
        Dim caumov_det As String

        Select Case lav_cod
            Case LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE, LAVCOD_ACCETTAZIONE_DIVERSI, LAVCOD_DISTINTA_CARICO_ACCETTAZIONE
                'caumov_det = "7920"
                caumov_det = "4070"

            Case LAVCOD_TRASFORMAZIONI
                caumov_det = "7350"

            Case Else
                caumov_det = "7300"

        End Select
        Return caumov_det
    End Function
    Private Shared Function GetCaumov_det_datoModulo(ByVal modulo As Integer) As String
        Dim caumov_det As String

        If modulo = 2 Then
            caumov_det = "4070,7300"
        Else
            caumov_det = "7300"
        End If


        Return caumov_det
    End Function
    Public Function LeggiEtichetta_ConferimentoSintetica(
        ByVal id_agenda As Integer,
        ByVal OModuli_Referenze_Config_Testata As Integer,
        ByVal iFF_Etichette_Tipo As FF_Etichette_tipo,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "FF_Etichette_R.LeggiEtichetta_ConferimentoSintetica()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            Dim piva As String = GetPiva(id_agenda, objParametri, NomeRoutine, stb)

            Dim lav_cod As Integer = GetLav_cod(id_agenda, objParametri, NomeRoutine, stb)
            Dim modulo As Integer = GetModulo(id_agenda, objParametri, NomeRoutine, stb)

            Dim caumov_tes As String = GetCaumov_tes_datoLav_Cod(lav_cod)
            'Dim caumov_det As String = GetCaumov_det_datoModulo(modulo)
            Dim caumov_det As String = "4070,7300"

            Dim joinRisum As Boolean = True

            If lav_cod = LAVCOD_DISTINTA_CARICO OrElse
                lav_cod = LAVCOD_DISTINTA_CARICO_ACCETTAZIONE OrElse
                lav_cod = LAVCOD_MONITORAGGIO_TEMPI_RIENTRO OrElse LAVCOD_TESTATE_ORDINE_LAVORAZIONE Then
                joinRisum = False
            End If


            LeggiEtichetta_Sintetica_getQuery(piva, id_agenda, caumov_det, caumov_tes, OModuli_Referenze_Config_Testata, iFF_Etichette_Tipo, joinRisum, stb, objParametri)




            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.Append(" AND   A.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.Append(" AND   A.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception


            'uso questa funzione per ottenere il Messaggio..:
            MessaggioErrore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing

            'passo l'eccezione corrente al throw come inner exception ..:
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)



        End Try

        Return DT





    End Function


    Public Function LeggiEtichetta_xWaTable_v2(
            ByVal id_agenda As Integer,
            ByVal xFiltroAggiuntivo As String,
            ByVal xOrderBy As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "FF_Etichette_R.LeggiEtichetta_xWaTable_v2()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            LeggiDatiEtichetta_xWaTable_v2_GetQuery(id_agenda, stb)

            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.Append(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.Append(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception


            'uso questa funzione per ottenere il Messaggio..:
            MessaggioErrore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing

            'passo l'eccezione corrente al throw come inner exception ..:
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)



        End Try

        Return DT





    End Function


    Public Function LeggiEtichetta_PalletSintetica(
            ByVal id_agenda As Integer,
            ByVal OModuli_Referenze_Config_Testata As Integer,
            ByVal iFF_Etichette_Tipo As FF_Etichette_tipo,
            ByVal xFiltroAggiuntivo As String,
            ByVal xOrderBy As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "FF_Etichette_R.LeggiEtichetta_Pallet()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            Dim piva As String = GetPiva(id_agenda, objParametri, NomeRoutine, stb)

            LeggiEtichetta_Sintetica_getQuery(piva, id_agenda, "7350", "4000", OModuli_Referenze_Config_Testata, iFF_Etichette_Tipo, True, stb, objParametri)

            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.Append(" AND   A.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.Append(" AND   A.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            MessaggioErrore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing

            'passo l'eccezione corrente al throw come inner exception ..:
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)



        End Try

        Return DT





    End Function


    Private Function getJoinCopie(ByVal s As String) As String

        '  Vanni, 04/11/2015 11:24:58: se non viene richiesto un "numero di copie" qui allora non mando in join quindi non è necessario questa parte.
        If s.Contains("-") Then
            Return ""
        End If

        Dim rval As String = vbCrLf & " inner join ( " & vbCrLf

        Dim s1 As String() = s.TrimEnd("|").Split("|")

        For i As Integer = 0 To s1.Length - 1

            Dim s2 As String() = s1(i).Split(",")
            Dim tt As String = ""
            For j As Integer = 0 To s2(1) - 1 Step 1
                tt &= "1,"
            Next
            tt = tt.TrimEnd(",")

            If tt = "" Then
                tt = "1"
            End If

            rval &= " " & vbCrLf &
                "select " & s2(0) & " as id " & vbCrLf &
                "from dbo.fSplit('" & tt & "', ',')" & vbCrLf
            If i < s1.Length - 1 Then
                rval &= "union all "
            End If



        Next

        Return rval & vbCrLf &
            ") aa on aa.id =  detProd.Id_Mov_Det "


    End Function


    Private Function leggiParametriOmniFF_DatoOModuli_Referenze_Config_Dettagli(
            ByVal piva As String, ByVal OModuli_Referenze_Config_Testata_ID As Integer,
            ByVal objparametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As OModuli_Referenze_Config_Testata_obj

        Dim oLeggiCFG As New OModuli_Referenze_Config
        Dim oCFG As OModuli_Referenze_Config_Testata_obj = Nothing

        oCFG = oLeggiCFG.LeggiConfigurazione(piva,
            OModuli_Referenze_Config_Testata_ID,
            False,
            "",
            "",
            objparametri_Server
        )


        Return oCFG

    End Function

    Private Sub leggiParametriOmniFF_SELECT_DatoOModuli_Referenze_Config_Dettagli(
        ByVal iFF_Etichette_Tipo As FF_Etichette_tipo,
        ByVal LeggiAncheSigla As Boolean,
        ByVal oCFG As OModuli_Referenze_Config_Testata_obj,
        ByVal catStr As Boolean,
        ByRef stb As StringBuilder,
        Optional ByVal leggiMixMaxCalibro As Boolean = False,
        Optional ByVal leggiCodice As Boolean = False
    )

        Dim iCicla As Integer = 0
        Dim iConteggioCFG As Integer = oCFG.Configurazioni.Count
        Dim iLst As String = "+"

        For Each curCfg In oCFG.Configurazioni
            If iCicla = iConteggioCFG - 1 Then
                iLst = ""
            End If
            If Not ({"cliente"}).Contains(curCfg.Tabella_Key.ToString.ToLower) Then
                If Not ({"fornitore"}).Contains(curCfg.Tabella_Key.ToString.ToLower) Then
                    ' Il codice viene letto solo se non si sta componendo la stringa
                    If Not catStr Then
                        leggiParametriOmniFF_Select_Codice(iFF_Etichette_Tipo, curCfg, stb, catStr, lst:=iLst)
                    End If
                    If LeggiAncheSigla AndAlso curCfg.Tipo = 1 Then
                        leggiParametriOmniFF_Select(iFF_Etichette_Tipo, curCfg, stb, catStr, Descrizione_Oppure_Sigla:="Sigla", lst:=iLst)
                    End If
                    leggiParametriOmniFF_Select(iFF_Etichette_Tipo, curCfg, stb, catStr, Descrizione_Oppure_Sigla:="Descrizione", lst:=iLst)
                Else
                    If Not catStr Then
                        stb.AppendLine("   , r_u" & curCfg.Tabella_Key & ".Cod_RisUm  AS fornitore_Codice ")
                        If LeggiAncheSigla Then
                            stb.AppendLine("   , contatti" & curCfg.Tabella_Key & ".Rag_Soc  AS fornitore_Sigla ")
                        End If
                        stb.AppendLine("   , contatti" & curCfg.Tabella_Key & ".Rag_Soc  AS fornitore ")
                    End If
                End If
            End If

            ' Per il calibro aggiungo il valore min e max che contengono i mm del prodotto per quel calibro
            If leggiMixMaxCalibro AndAlso Not catStr AndAlso ({"calibro"}).Contains(curCfg.Tabella_Key.ToString.ToLower) Then
                'calibro mm da / a
                stb.AppendLine("   , CAST(coalesce(otpCalibro.Valore_Min, 0) AS INT) AS Calibro_Valore_Min ")
                stb.AppendLine("   , CAST(coalesce(otpCalibro.Valore_Max, 0) AS INT) AS Calibro_Valore_Max ")
            End If

            iCicla += 1

        Next

        Dim qryString = stb.ToString().Trim()
        Dim lastChar = qryString(qryString.Length - 1)
        If lastChar = "+" Then
            stb.Clear()
            stb.Append(qryString.Remove(qryString.Length - 1))
        End If


    End Sub

    Private Sub leggiParametriOmniFF_FROM_DatoOModuli_Referenze_Config_Dettagli(
            ByVal oCFG As OModuli_Referenze_Config_Testata_obj,
            ByRef stb As StringBuilder
        )

        For Each curCfg In oCFG.Configurazioni
            If Not ({"cliente"}).Contains(curCfg.Tabella_Key.ToString.ToLower) AndAlso
               Not ({"fornitore"}).Contains(curCfg.Tabella_Key.ToString.ToLower) Then
                leggiParametriOmniFF_FROM(curCfg.Tabella_Key, stb)
            End If
            If ({"fornitore"}).Contains(curCfg.Tabella_Key.ToString.ToLower) Then
                leggiParametriOmniFF_FROM(curCfg.Tabella_Key, stb, "F")
            End If
        Next

    End Sub



    Public Function LeggiEtichetta_Confezione(
                ByVal id_agenda As Integer,
                ByVal infoCopie As String,
                ByVal OModuli_Referenze_Config_Testata As Integer,
                ByVal iFF_Etichette_Tipo As FF_Etichette_tipo,
                ByVal barcodeType As String,
                ByVal xFiltroAggiuntivo As String,
                ByVal xOrderBy As String,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
            ) As DataTable


        'per lettura config di estrazione dati...
        Dim cfg As FF_BarcodeType =
           jSonToBarcodeType(barcodeType)

        Dim stb As New System.Text.StringBuilder
        Dim lav_cod As Integer = GetLav_cod(id_agenda, objParametri, "", stb)
        Dim modulo As Integer = GetModulo(id_agenda, objParametri, "", stb)

        Dim caumov_det As String = GetCaumov_det_datoModulo(modulo)


        '----- Descrizione
        Dim NomeRoutine As String = "FF_Etichette_R.LeggiEtichetta_Pallet()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim DT As DataTable

        Try

            Dim caumov_Testata As String
            caumov_Testata = GetCaumov_tes_datoLav_Cod(lav_cod)
            Dim piva As String = GetPiva(id_agenda, objParametri, NomeRoutine, stb)

            Dim oCFG As OModuli_Referenze_Config_Testata_obj
            oCFG = leggiParametriOmniFF_DatoOModuli_Referenze_Config_Dettagli(
                piva, OModuli_Referenze_Config_Testata,
                objParametri
            )

            stb.Length = 0

            stb.Append(" select  " & vbCrLf)
            stb.Append("      coalesce(veg.Veg_Des , '') AS Specie " & vbCrLf)
            stb.Append("    , coalesce(cul.cul_des , '') AS Varieta " & vbCrLf)
            stb.Append("    , detProd.lotto AS Lotto " & vbCrLf)
            stb.Append(" , matProd.Mat_Des as Referenza" & vbCrLf)
            stb.Append(" , mProd.Data_Movimento as Data_Movimento" & vbCrLf)
            'Inizio Parametri Omni
            leggiParametriOmniFF_SELECT_DatoOModuli_Referenze_Config_Dettagli(
                iFF_Etichette_Tipo, True, oCFG, False, stb, True, False
            )

            'stb.Append("    , otpProvenienza.Descrizione as Provenienza " & vbCrLf)
            'stb.Append("    , otpCalibro.Descrizione  as CalibroEsteso " & vbCrLf)
            'stb.Append("    , otpQualità.Descrizione  as Qualita " & vbCrLf)
            'stb.Append("    , cDescrizione.Descrizione as Descrizione " & vbCrLf)
            'stb.Append("    , otpCalibro.Descrizione  as CalibroBreve " & vbCrLf)
            'stb.Append("    , cLotto_Cliente.Descrizione as LottoCliente " & vbCrLf)
            'stb.Append("    , cEan.Descrizione as CodiceEAN " & vbCrLf)
            'stb.Append("    , cGGN.Descrizione  as GlobalGapNumber " & vbCrLf)
            'Fine parametri Omni

            stb.Append("    , A.Des_Lib  AS Descrizione " & vbCrLf)
            stb.Append("    , cCli.Rag_Soc  AS Cliente " & vbCrLf)

            stb.Append("    , case when detProd.udm_cod_Extra = 2 then detProd.qta_extra * 1000 else detProd.qta_extra end as PesoNetto " & vbCrLf) 'todo: su etich. confezione, peso netto 

            stb.Append("    , '1000' as CodiceProgramma " & vbCrLf) 'todo: su etich. confezione, Codice Programma
            stb.Append("    , 0 as NumeroColli " & vbCrLf) 'todo: su etich. confezione, Numero Colli
            stb.Append("    , 0 as TipoCodiceEAN " & vbCrLf) 'todo: su etich. confezione, Tipo codice EAN
            stb.Append("    , 0 as PesoNettoTotale " & vbCrLf) 'todo: su etich. confezione, Peso Netto Totale
            'stb.Append("    , '02' as NumUscita " & vbCrLf) 'todo, letto da omni
            stb.Append("    , '' as EAN8 " & vbCrLf)
            stb.Append("    , '' as EAN13 " & vbCrLf)
            stb.Append("    , '' as EAN14 " & vbCrLf)


            Select Case cfg.Code128.FirstOrDefault.Algoritmo

                Case FF_Barcode_Algoritmo_Dati.Cofruta
                    'stb.Append(" , detProd.lotto as CODE128" & vbCrLf)
                    stb.Append(" , A.Des_Lib as CODE128" & vbCrLf)

                Case Else
                    stb.Append(" , '' as CODE128" & vbCrLf)

            End Select

            stb.Append("    , ''  as CodPro " & vbCrLf)
            stb.Append("    , ''  as NumUsc " & vbCrLf)
            stb.Append("    , ''  as Code1028BC " & vbCrLf)
            stb.Append("    , ''  as CodiceEANBC " & vbCrLf)

            LeggiDatiEtichetta_From(caumov_det, caumov_Testata, True, False, False, stb)

            leggiParametriOmniFF_FROM_DatoOModuli_Referenze_Config_Dettagli(oCFG, stb)

            stb.Append(getJoinCopie(infoCopie))


            stb.Append("    left join Centri_Aziendali_Codici cac_bioNCert " & vbCrLf)
            stb.Append("        on cac_bioNCert.PIVA = a.piva  " & vbCrLf)
            stb.Append("        and cac_bioNCert.id_cod = 1003 " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("    left join Centri_Aziendali_Codici cac_bioEnte " & vbCrLf)
            stb.Append("        on cac_bioEnte.PIVA = a.piva  " & vbCrLf)
            stb.Append("        and cac_bioEnte.id_cod = 1025 " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("    left join imprese_codici ic_bndoo " & vbCrLf)
            stb.Append("        on ic_bndoo.piva = i.piva  " & vbCrLf)
            stb.Append("        and ic_bndoo.id_cod = 1277 " & vbCrLf)

            stb.Append(" WHERE  A.id_Agenda  = " & Agro_SQL_SaveNum(id_agenda) & vbCrLf)

            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.Append(" AND   A.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.Append(" AND   A.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception


            'uso questa funzione per ottenere il Messaggio..:
            MessaggioErrore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing

            'passo l'eccezione corrente al throw come inner exception ..:
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)



        End Try

        Return DT


    End Function


    Public Function LeggiEtichetta_Imballo_Full(
            ByVal id_agenda As Integer,
            ByVal StartCodeChar As String,
            ByVal EndCodeChar As String,
            ByVal StartAIChar As String,
            ByVal lSetCode128 As String,
            ByVal nRigheBarcode As Integer,
            ByVal infoCopie As String,
            ByVal barcodeType As String,
            ByVal lingua_cod As Integer,
            ByVal OModuli_Referenze_Config_Testata As Integer,
            ByVal iFF_Etichette_Tipo As FF_Etichette_tipo,
            ByVal xFiltroAggiuntivo As String,
            ByVal xOrderBy As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
            ByRef rpt As CrystalDecisions.CrystalReports.Engine.ReportDocument,
            ByRef DataSourceImpostato As Boolean
            ) As DataTable


        Dim dt As DataTable = LeggiEtichetta_Imballo(id_agenda, infoCopie, OModuli_Referenze_Config_Testata, iFF_Etichette_Tipo, xFiltroAggiuntivo, xOrderBy, objParametri)

        BarcodeCommon(barcodeType, lingua_cod, StartCodeChar, EndCodeChar, StartAIChar, lSetCode128, dt, rpt, DataSourceImpostato, objParametri)

        Return dt

    End Function

    Public Function LeggiEtichetta_Imballo(
                ByVal id_agenda As Integer,
                ByVal infoCopie As String,
                ByVal OModuli_Referenze_Config_Testata As Integer,
                ByVal iFF_Etichette_Tipo As FF_Etichette_tipo,
                ByVal xFiltroAggiuntivo As String,
                ByVal xOrderBy As String,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
            ) As DataTable


        '----- Descrizione
        Dim NomeRoutine As String = "FF_Etichette_R.LeggiEtichetta_Pallet()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim piva As String = GetPiva(id_agenda, objParametri, NomeRoutine, stb)

        Dim oCFG As OModuli_Referenze_Config_Testata_obj
        oCFG = leggiParametriOmniFF_DatoOModuli_Referenze_Config_Dettagli(
            piva, OModuli_Referenze_Config_Testata,
            objParametri
        )


        Try

            stb.Length = 0

            stb.Append("select " & vbCrLf)

            stb.Append("   --dati ordine " & vbCrLf)
            stb.Append("      coalesce(cast(year(mRiff.Data_Movimento) as varchar(100)) + '-' + mRiff.Doc_Numero_Sin + '-' + cast(  mRiff.Doc_Numero as varchar(100)) + '-' + cast( dense_Rank() over(order by mRiff.id_mov) as varchar(100)) , '') AS[barcodeORdine]   " & vbCrLf)
            stb.Append("    , coalesce(year(mRiff.Data_Movimento), '') AS[annoORdine]   " & vbCrLf)
            stb.Append("    , coalesce(mRiff.Doc_Numero_Sin, '') AS[serieORdine]   " & vbCrLf)
            stb.Append("    , coalesce(mRiff.Doc_Numero, '') AS[numeroORdine]   " & vbCrLf)
            stb.Append("    , coalesce(dense_Rank() over(order by mRiff.id_mov), '') AS[rigaORdine] ")


            stb.Append(" , '' as CodiceCalibroRiga " & vbCrLf)
            stb.Append(" , '' as CodiceCalibroTestata " & vbCrLf)

            stb.Append("   , cCli.piva AS[codiceCliente]  " & vbCrLf)
            stb.Append("   , cCli.Rag_Soc           AS[ragioneSocialeCliente]  " & vbCrLf)

            stb.Append("   , '' AS[codiceSpecie]  " & vbCrLf)
            stb.Append("   , coalesce(veg.Veg_Des , '') AS[specie]  " & vbCrLf)

            stb.Append("   , '' AS[codiceVarieta]  " & vbCrLf)
            stb.Append("   , coalesce(cul.cul_des , '') AS[varieta]  " & vbCrLf)

            'configurazione omni

            'calibro Riga
            leggiParametriOmniFF_SELECT_DatoOModuli_Referenze_Config_Dettagli(
                iFF_Etichette_Tipo, False, oCFG, False, stb
            )

            'stb.Append("   , otpCalibro.sigla AS[codiceCalibroRiga]   " & vbCrLf)
            'stb.Append("   , otpCalibro.Descrizione   AS[descrizioneCalibroRiga]  ")


            ''calibro Testata
            'stb.Append("   , otpCalibro.sigla AS[codiceCalibroTestata]   " & vbCrLf)
            'stb.Append("   , otpCalibro.Descrizione   AS[descrizioneCalibroTestata]  ")


            ''pedana
            'stb.Append("   , '' AS[codicePedanaBin]  " & vbCrLf)
            'stb.Append("   , '' AS[descrizionePedanaBin]  " & vbCrLf)


            ''imballo
            'stb.Append("   , otpImballaggio.sigla AS[codiceImballo]   " & vbCrLf)
            'stb.Append("   , otpImballaggio.Descrizione   AS[descrizioneImballo]  ")
            'stb.Append("   , '' AS[numeroImballi]  " & vbCrLf)

            ''confezione
            'stb.Append("   , otpConfezione.sigla AS[codiceConfezione]   " & vbCrLf)
            'stb.Append("   , otpConfezione.Descrizione   AS[descrizioneConfezione]  ")

            'fine parametriOmni


            stb.Append(" , '' as CodiceDisciplinare " & vbCrLf)
            stb.Append(" , '' as CodiceEAN " & vbCrLf)
            stb.Append(" , '' as CodiceLinea " & vbCrLf)
            stb.Append(" , '' as CodiceProgramma " & vbCrLf)
            stb.Append(" , getdate() as DataConfezionamento " & vbCrLf)
            stb.Append(" , '' as DescrizioneDisciplinare " & vbCrLf)
            stb.Append(" , '' as LayoutConfezione " & vbCrLf)
            stb.Append(" , '' as LayoutImballo " & vbCrLf)
            stb.Append(" , '' as Lingua " & vbCrLf)
            stb.Append(" , '' as LottoEsternoConfezione " & vbCrLf)
            stb.Append(" , '' as LottoEsternoImballo " & vbCrLf)
            stb.Append("   , detProd.lotto AS [lottoInterno]  " & vbCrLf)
            stb.Append(" , '' as NumeroConfezioniImballo " & vbCrLf)
            stb.Append(" , '' as NumeroCopieConfezione " & vbCrLf)
            stb.Append(" , '' as NumeroCopieImballo " & vbCrLf)
            stb.Append(" , '' as NumeroImballi " & vbCrLf)
            stb.Append(" , '' as Operatore " & vbCrLf)
            stb.Append(" , '' as PesoLordoPedanaBin " & vbCrLf)
            stb.Append(" , '' as PesoNettoConfezione " & vbCrLf)
            stb.Append(" , '' as PesoNettoImballo " & vbCrLf)
            stb.Append(" , '' as PesoNettoPedanaBin " & vbCrLf)
            stb.Append(" , '' as PesoNettoStimatoConfezione " & vbCrLf)
            stb.Append(" , '' as PesoNettoStimatoImballo " & vbCrLf)
            stb.Append(" , '' as PesoNettoStimatoPedanaBin " & vbCrLf)
            stb.Append("  , '' as TestoLibero1  " & vbCrLf)

            stb.Append("  , case when matProd.regolamento = " & enum_Cod_Regolamento.Regolamento_bio & " then coalesce( " & vbCrLf)
            stb.Append("             'DA AGRICOLTURA BIOLOGICA <br>' +  " & vbCrLf)
            stb.Append("             'ORGANISMO DI CONTR. AUT. DL MIPAAF IT BIO ' + cac_bioEnte.val_cod + '<br>' +  " & vbCrLf)
            stb.Append("             'OPERATORE CONTROLLATO ' + cac_bioNCert.val_cod + '' " & vbCrLf)
            stb.Append("  , '' " & vbCrLf)
            stb.Append("   ) else '' end as TestoLibero2 " & vbCrLf)


            stb.Append(" , '' as TestoLibero3 " & vbCrLf)
            stb.Append(" , '' as Uscita " & vbCrLf)
            stb.Append(" , i.rag_soc as RagioneSocialeFornitore  " & vbCrLf)

            stb.Append(" , ind.ind_des + ' ' + ind.CAP + ' ' + ind.com_des + ' (' + ind.pro_cod  + ')' as IndirizzoFornitore  " & vbCrLf)

            stb.Append(" , matProd.mat_des as NomeReferenza ")

            stb.Append(" , coalesce(ic_bndoo.val_cod , '') as BNDOO  " & vbCrLf)

            LeggiDatiEtichetta_From("7350", "4000", True, True, False, stb)



            leggiParametriOmniFF_FROM_DatoOModuli_Referenze_Config_Dettagli(oCFG, stb)


            stb.Append(getJoinCopie(infoCopie))


            stb.Append("    left join Centri_Aziendali_Codici cac_bioNCert " & vbCrLf)
            stb.Append("        on cac_bioNCert.PIVA = a.piva  " & vbCrLf)
            stb.Append("        and cac_bioNCert.id_cod = 1003 " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("    left join Centri_Aziendali_Codici cac_bioEnte " & vbCrLf)
            stb.Append("        on cac_bioEnte.PIVA = a.piva  " & vbCrLf)
            stb.Append("        and cac_bioEnte.id_cod = 1025 " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("    left join imprese_codici ic_bndoo " & vbCrLf)
            stb.Append("        on ic_bndoo.piva = i.piva  " & vbCrLf)
            stb.Append("        and ic_bndoo.id_cod = 1277 " & vbCrLf)


            stb.Append(" WHERE  A.id_Agenda  = " & Agro_SQL_SaveNum(id_agenda) & vbCrLf)

            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.Append(" AND   A.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.Append(" AND   A.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception


            'uso questa funzione per ottenere il Messaggio..:
            MessaggioErrore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing

            'passo l'eccezione corrente al throw come inner exception ..:
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)



        End Try

        Return DT



    End Function



    Private Function LeggiEtichetta_Pallet(
                ByVal id_agenda As Integer,
                ByVal infoCopie As String,
                ByVal xFiltroAggiuntivo As String,
                ByVal xOrderBy As String,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
            ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "FF_Etichette_R.LeggiEtichetta_Pallet()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0

            stb.Append("SELECT  " & vbCrLf)
            stb.Append("     '' AS[originePedana]  " & vbCrLf)
            stb.Append("   , '' AS[linea]  " & vbCrLf)
            stb.Append("   , '' AS[codiceUscita]  " & vbCrLf)
            stb.Append("   , '' AS[uscita]  " & vbCrLf)
            stb.Append("   ,  CONVERT(DateTime,'1900/01/01',120)  AS[dataOraUscita]  " & vbCrLf)
            stb.Append("   , '' AS[lingua]  " & vbCrLf)
            stb.Append("   , '' AS[layout]  " & vbCrLf)


            stb.Append("   --dati ordine " & vbCrLf)
            stb.Append("    , coalesce(cast(year(mRiff.Data_Movimento) as varchar(100)) + '-' + mRiff.Doc_Numero_Sin + '-' + cast(  mRiff.Doc_Numero as varchar(100)) + '-' + cast( dense_Rank() over(order by mRiff.id_mov) as varchar(100)) , '') AS[barcodeORdine]   " & vbCrLf)
            stb.Append("    , coalesce(year(mRiff.Data_Movimento), '') AS[annoORdine]   " & vbCrLf)
            stb.Append("    , coalesce(mRiff.Doc_Numero_Sin, '') AS[serieORdine]   " & vbCrLf)
            stb.Append("    , coalesce(mRiff.Doc_Numero, '') AS[numeroORdine]   " & vbCrLf)
            stb.Append("    , coalesce(dense_Rank() over(order by mRiff.id_mov), '') AS[rigaORdine] ")

            stb.Append("   --dati bolla " & vbCrLf)
            stb.Append("   , cast(year(mTest.Data_Movimento) as varchar(100)) + '-' + mTest.Doc_Numero_Sin + '-' + cast(  mTest.Doc_Numero as varchar(100)) + cast( dense_Rank() over(order by mProd.id_mov) as varchar(100)) AS[barcodeBolla]  " & vbCrLf)
            stb.Append("   , year(mTest.Data_Movimento) AS[annoBolla]  " & vbCrLf)
            stb.Append("   , mTest.Doc_Numero_Sin AS[serieBolla]  " & vbCrLf)
            stb.Append("   , mTest.Doc_Numero AS[numeroBolla]  " & vbCrLf)
            stb.Append("   , dense_Rank() over(order by mProd.id_mov) AS[rigaBolla]  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("   , '' AS[dataOrdine]  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("   , cCli.piva AS[codiceCliente]  " & vbCrLf)
            stb.Append("   , cCli.Rag_Soc           AS[ragioneSocialeCliente]  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("   , '' AS[codiceSpecie]  " & vbCrLf)
            stb.Append("   , coalesce(veg.Veg_Des , '') AS[specie]  " & vbCrLf)
            stb.Append("   , '' AS[codiceVarieta]  " & vbCrLf)
            stb.Append("   , coalesce(cul.cul_des , '') AS[varieta]  " & vbCrLf)

            'calibro
            stb.Append("   , otpCalibro.sigla AS[codiceCalibro]   " & vbCrLf)
            stb.Append("   , otpCalibro.Descrizione   AS[descrizioneCalibro]  ")

            'pedana
            stb.Append("   , '' AS[codicePedana]  " & vbCrLf)
            stb.Append("   , '' AS[descrizionePedana]  " & vbCrLf)
            stb.Append("   , '' AS[numeroPedane]  " & vbCrLf)

            'imballo
            stb.Append("   , otpImballaggio.sigla AS[codiceImballo]   " & vbCrLf)
            stb.Append("   , otpImballaggio.Descrizione   AS[descrizioneImballo]  ")
            stb.Append("   , '' AS[numeroImballi]  " & vbCrLf)

            'confezione
            stb.Append("   , otpConfezione.sigla AS[codiceConfezione]   " & vbCrLf)
            stb.Append("   , otpConfezione.Descrizione   AS[descrizioneConfezione]  ")
            stb.Append("   , '' AS[numeroConfezioni]  " & vbCrLf)

            stb.Append("  " & vbCrLf)
            stb.Append("   , 0 AS[pesoStimatoComplessivo]  " & vbCrLf)
            stb.Append("   , 0 AS[pesoLordo]  " & vbCrLf)
            stb.Append("   , 0 AS[pesoNettoComplessivo]  " & vbCrLf)
            stb.Append("   , '' AS[note]  " & vbCrLf)
            stb.Append("   ,  CONVERT(DateTime,'1900/01/01',120)  AS[dataConfezionamento]  " & vbCrLf)


            stb.Append("   , otpQualità.Descrizione   AS[qualita]  ")


            stb.Append("   , '' AS[provenienza]  " & vbCrLf)
            stb.Append("   , '' AS[lottoEsternoImballo]  " & vbCrLf)
            stb.Append("   , '' AS[lottoEsternoConfezione]  " & vbCrLf)
            stb.Append("   , detProd.lotto AS[lottoInterno]  " & vbCrLf)
            stb.Append("   , '' AS[codiceean]  " & vbCrLf)
            stb.Append("   , '' AS[pesoStimatoPerPedana]  " & vbCrLf)
            stb.Append("   , '' AS[pesoNettoPerPedana]  " & vbCrLf)
            stb.Append("   , '' AS[nPedana]  " & vbCrLf)
            stb.Append("   , '' AS[SSCC]  " & vbCrLf)
            stb.Append("   , '' AS[stampata]  " & vbCrLf)
            stb.Append("   , '' AS[EAN128_TOP]  " & vbCrLf)
            stb.Append("   , '' AS[EAN128_TOP_HR]  " & vbCrLf)
            stb.Append("   , '' AS[EAN128_BOTTOM]  " & vbCrLf)
            stb.Append("   , '' AS[EAN128_BOTTOM_HR]  " & vbCrLf)
            stb.Append("   , '' AS[EAN128_MID]  " & vbCrLf)
            stb.Append("   , '' AS[EAN128_MID_HR]  " & vbCrLf)
            stb.Append("  " & vbCrLf)

            LeggiDatiEtichetta_From("7350", "4000", True, True, False, stb)


            leggiParametriOmniFF_FROM("Calibro", stb)
            leggiParametriOmniFF_FROM("Qualità", stb)
            leggiParametriOmniFF_FROM("Imballaggio", stb)
            leggiParametriOmniFF_FROM("Confezione", stb)

            stb.Append(getJoinCopie(infoCopie))

            stb.Append(" WHERE  A.id_Agenda  = " & Agro_SQL_SaveNum(id_agenda) & vbCrLf)




            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.Append(" AND   A.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.Append(" AND   A.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception


            'uso questa funzione per ottenere il Messaggio..:
            MessaggioErrore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing

            'passo l'eccezione corrente al throw come inner exception ..:
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)



        End Try

        Return DT





    End Function


End Class



'#################################################################
'#################################################################
'#################################################################

Public Class FF_Etichette_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '##############################################################################################
    Public Function ImpostaCodiceABarreSuELI_UPDT(
                  ByVal Progressivo As Integer,
                  ByVal Tipo As String,
                  ByVal Tipo_Cod As Integer,
                  ByVal udm_cod As Integer,
                  ByVal Valore As String,
                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                ) As Boolean


        Dim NomeRoutine As String = "Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try


            '---------------------------------------------
            Stb.Length = 0
            Stb.AppendLine(" UPDATE Materie_Prime_Campionature ")

            Stb.Append(" set Descrizione =  '" & Agro_SQL_SaveText(Valore) & "' ")

            Stb.Append(" where  Progressivo  =  " & Agro_SQL_SaveNum(Progressivo))
            Stb.Append(" and tipo  =  '" & Agro_SQL_SaveText(Tipo) & "'")
            Stb.Append(" and tipo_cod  =  " & Agro_SQL_SaveNum(Tipo_Cod) & "")
            Stb.Append(" and udm_cod  =  " & Agro_SQL_SaveNum(udm_cod) & "")

            '--------------------------------------------------------------------------
            ' Asteriscato da Stefano Scattolin in data 26/4/2017 perchè ora questo record viene scritto nel Lan al momento del salvataggio
            ' xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function







End Class







