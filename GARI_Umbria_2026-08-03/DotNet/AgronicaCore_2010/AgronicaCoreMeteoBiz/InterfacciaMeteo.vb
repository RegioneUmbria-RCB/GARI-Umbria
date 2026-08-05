

Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreUtility.VB6_Proxy
Imports AgronicaCoreUtility.DataOra
Imports System.Xml
Imports Newtonsoft.Json


Public Class InterfacciaMeteo
    Private Const PioggieNull As Integer = -999
    Private vOrari As List(Of clDatiOrari)
    Private vGiornalieri As List(Of clDatiGiornalieri)

    Private _ObjParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri

    Public Property ObjParametri_Server As AgronicaCoreParametri
        Get
            Return _ObjParametri_Server
        End Get
        Set(value As AgronicaCoreParametri)
            _ObjParametri_Server = value
        End Set
    End Property

    Public Property ModelliPrevisionali_InputData_Meteo As String
        Get
            Return _modelliPrevisionaliInputDataMeteo
        End Get
        Set(value As String)
            _modelliPrevisionaliInputDataMeteo = value
        End Set
    End Property

    Public Property DataInizio As Date
        Get
            Return _dataInizio
        End Get
        Set(value As Date)
            _dataInizio = value
        End Set
    End Property

    Public Property DataFine As Date
        Get
            Return _dataFine
        End Get
        Set(value As Date)
            _dataFine = value
        End Set
    End Property

    Public Property DatiOrari As List(Of clDatiOrari)
        Get
            Return vOrari
        End Get
        Set(value As List(Of clDatiOrari))
            vOrari = value
        End Set
    End Property

    Private _modelliPrevisionaliInputDataMeteo As String

    Private _dataInizio As Date

    Private _dataFine As Date

    Private _letturaDati As enum_MeteoLetturaDati


    Public vAnno As Integer  'Anno di riferimento dei dati

    'Private vOrari(0 To 365, 0 To 23) As clDatiOrari
    'Private vGiornalieri(0 To 365) As clDatiGiornalieri

    Public Sub New(ByVal letturaDati As enum_MeteoLetturaDati, ByVal dataInizio As Date, ByVal dataFine As Date, ByVal ObjParametri_Server As AgronicaCoreParametri, ByVal ModelliPrevisionali_InputData_Meteo As String, Optional ByVal RitardaLetturaDatiMeteo As Boolean = False)

        _letturaDati = letturaDati
        _dataInizio = dataInizio
        _dataFine = dataFine

        vAnno = Year(_dataInizio)

        _ObjParametri_Server = ObjParametri_Server
        _modelliPrevisionaliInputDataMeteo = ModelliPrevisionali_InputData_Meteo

        If Not RitardaLetturaDatiMeteo Then
            LeggiDatiMeteo()
        End If

    End Sub

    Public Function UltimaDataOrari() As Date

        If vOrari Is Nothing Then
            LeggiDatiMeteo()
        End If

        Dim rval As Date = (
            From oo In vOrari
            Order By oo.Data Descending
            Select oo.Data
         ).FirstOrDefault

        Return rval

    End Function


    Public Function UltimaDataGiornalieri() As Date

        If vGiornalieri Is Nothing Then
            LeggiDatiMeteo()
        End If

        Dim rval As Date = (
            From oo In vGiornalieri
            Order By oo.Data Descending
            Select oo.Data
         ).FirstOrDefault

        Return rval

    End Function

    ''' <summary>
    ''' Restituisce i dati per un giorno ed un ora specificati
    ''' </summary>
    ''' <param name="Indice_o_Data">Data (o indice)</param>
    ''' <returns></returns>
    Public Function Giornalieri(Indice_o_Data As Object) As clDatiGiornalieri

        Dim xData As Date

        If IsNumeric(Indice_o_Data) Then
            xData = AgronicaCoreUtility.DataOra.GiulianoToDate(Indice_o_Data, vAnno)

        Else
            xData = Indice_o_Data

        End If


        If vOrari Is Nothing Then
            LeggiDatiMeteo()
        End If

        Dim rval As clDatiGiornalieri = (
            From oo In vGiornalieri
            Where oo.Data = xData
        ).FirstOrDefault

        Return rval

    End Function

    ''' <summary>
    ''' Restituisce i dati per un giorno ed un ora specificati
    ''' </summary>
    ''' <param name="Indice_o_Data">Data (o indice)</param>
    ''' <param name="Ora">Ora del giorno, se non specificata vale zero.</param>
    ''' <returns></returns>
    Public Function Orari(Indice_o_Data As Object, Optional Ora As Integer = 0) As clDatiOrari

        Dim xData As Date

        If IsNumeric(Indice_o_Data) Then
            xData = AgronicaCoreUtility.DataOra.GiulianoToDate(Indice_o_Data, vAnno)

        Else
            xData = Indice_o_Data

        End If


        If vOrari Is Nothing Then
            LeggiDatiMeteo()
        End If

        Dim rval As clDatiOrari = (
            From oo In vOrari
            Where oo.Data = xData _
            And oo.ora = Ora
        ).FirstOrDefault

        Return rval

    End Function

    Public vMancanoOrari As Boolean
    Public vUltimaDataGiornalieri As Date
    Public vUltimaDataOrari As Date


    Private Function LeggiDatiMeteo() As String

        'leggo i dati meteo orari e/o giornalieri

        Dim d As Integer
        Dim i As Integer
        Dim J As Integer
        Dim Giorno As Integer

        Dim Flag_Dettagli As String = ""
        Dim Flag_Aggrega As String = ""
        Dim Flag_Spazio As String = ""
        Dim Soglia As Single = 0
        Dim OraRilievo As Integer = 23
        Dim ID_Quadrante As Integer = 0
        Dim ID_Stazione As Integer = 0
        Dim X As Integer = 0
        Dim Y As Integer = 0
        Dim Data1 As Date = #1/1/1900#
        Dim Data2 As Date = #12/31/2100#
        Dim flag_tabella_leggi As String = "H"
        Dim nomeStazione As String = ""
        Dim XmlRisultato As String = ""

        Dim xMeteoDAL As New AgronicaCoreMeteoDAL.Meteo_R

        Dim ErrCod As Integer
        Dim ErrMsg As String

        '-----------------------------------------------------------------------
        '--- Recupero i parametri
        '-----------------------------------------------------------------------

        Dim xmlParametri As String =
            xSicurezza.Decodifica(ModelliPrevisionali_InputData_Meteo, xCostanti.KeyEncoderDecoder, ErrCod, ErrMsg)

        Dim xMeteoBiz As New meteo
        xMeteoBiz.LeggiParametri(xmlParametri, Flag_Dettagli, Flag_Aggrega, Flag_Spazio, Soglia, OraRilievo, ID_Quadrante, ID_Stazione, X, Y, Data1, Data2, flag_tabella_leggi, nomeStazione)

        '-----------------------------------------------------------------------
        '--- Se ho ricevuto le coordinate ==> calcolo il quadrante
        '-----------------------------------------------------------------------

        If Flag_Spazio = "C" Then

            Dim DTQuadrante As New DataTable

            DTQuadrante = xMeteoDAL.Dati_Quadranti_R(X, Y, _ObjParametri_Server)

            If DTQuadrante.Rows.Count = 0 Then
                ErrCod = 736254
                ErrMsg = "Nessun quadrante corrisponde alle coordinate indicate"
                Return ErrMsg
            Else
                ID_Quadrante = CInt(DTQuadrante.Rows(0).Item("ID_Quadrante"))
            End If

        End If

        '-----------------------------------------------------------------------
        '--- Recupero l'identificatore corretto
        '-----------------------------------------------------------------------

        Dim ID_QuadranteStazione As Integer
        Dim Flag_Quadrante_Stazione As String

        Select Case Flag_Spazio
            Case "Q", "C"
                ID_QuadranteStazione = ID_Quadrante
                Flag_Quadrante_Stazione = "Q"
            Case "S"
                ID_QuadranteStazione = ID_Stazione
                Flag_Quadrante_Stazione = "S"
            Case Else
                ErrCod = 923773
                ErrMsg = "Parametro non corretto"
                Return ""
                Exit Function
        End Select

        If _letturaDati = enum_MeteoLetturaDati.Entrambi OrElse
            _letturaDati = enum_MeteoLetturaDati.Giornalieri Then

            Dim dtMeteo As DataTable

            Dim flagTabellaLeggiGiornalieri As String = "G"
            If flag_tabella_leggi = "M" Then
                flagTabellaLeggiGiornalieri = "N"
            End If

            dtMeteo = xMeteoDAL.Dati_Completi(
                ID_QuadranteStazione:=ID_QuadranteStazione,
                OraRilievo:=OraRilievo,
                DataInizio:=DataInizio,
                DataFine:=DataFine,
                Flag_Quadrante_Stazione:=Flag_Quadrante_Stazione,
                flag_tabella_leggi:=flagTabellaLeggiGiornalieri,
                NomeStazione:=nomeStazione,
                objParametri:=ObjParametri_Server
                )

            For Each rs As DataRow In dtMeteo.Rows

                Dim cGiornalieri As New clDatiGiornalieri

                cGiornalieri.Data = rs("TEMPO")
                cGiornalieri.Data_Agg = rs("Data_Agg")
                cGiornalieri.ET_H = IIf(IsDBNull(rs("Evapotraspirazione")), 0, Val(Valore(rs("Evapotraspirazione") & "")))
                cGiornalieri.ET_CLA = IIf(IsDBNull(rs("Evapotraspirazione")), 0, Val(Valore(rs("Evapotraspirazione") & "")))
                cGiornalieri.ET_PE = IIf(IsDBNull(rs("Evapotraspirazione")), 0, Val(Valore(rs("Evapotraspirazione") & "")))
                cGiornalieri.Vento = IIf(IsDBNull(rs("Vento_Int")), 0, Val(Valore(rs("Vento_Int") & "")))
                cGiornalieri.VD = IIf(IsDBNull(rs("Vento_Dir")), 0, Val(Valore(rs("Vento_Dir") & "")))
                cGiornalieri.Irragg = 0 '' VAnni: 29/3/2017: TODO: da verificare
                cGiornalieri.Ur = 0 '' VAnni: 29/3/2017: TODO: da verificare
                cGiornalieri.Temp_Min = IIf(IsDBNull(rs("Temp_Min")), 0, Val(Valore(rs("Temp_Min") & "")))
                cGiornalieri.Temp_Med = IIf(IsDBNull(rs("Temp_Media")), 0, Val(Valore(rs("Temp_Media") & "")))
                cGiornalieri.Temp_Max = IIf(IsDBNull(rs("Temp_Max")), 0, Val(Valore(rs("Temp_Max") & "")))

                cGiornalieri.Prec = 0

                ''Leggerò anche la pioggia?
                'If IsdbNull(rs("Prec_L")) Then
                '    cGiornalieri.Prec = Val(Valore(rs("PREC_SMR") & ""))
                'Else
                '    cGiornalieri.Prec = Val(Valore(rs("PREC_L") & ""))
                'End If
                cGiornalieri.Mancante = False

                If vGiornalieri Is Nothing Then
                    vGiornalieri = New List(Of clDatiGiornalieri)
                End If
                vGiornalieri.Add(cGiornalieri)

            Next

        End If

        If _letturaDati = enum_MeteoLetturaDati.Entrambi OrElse
            _letturaDati = enum_MeteoLetturaDati.Orari Then

            If Flag_Spazio = "S" Then
                Dim dt As DataTable = xMeteoDAL.LeggiQuadranteDaStazione(ID_Stazione, ObjParametri_Server)

                If dt.Rows.Count > 0 Then
                    ID_QuadranteStazione = dt.Rows(0)("Id_Quadrante")
                Else
                    ID_QuadranteStazione = ID_Stazione
                End If

                Flag_Quadrante_Stazione = "Q"
            End If


            vMancanoOrari = False
            vUltimaDataOrari = "31/12/" & (vAnno - 1)

            Dim dtMeteoOrari As DataTable

            ' VAnni: 29/3/2017: dati meteo per stazione
            dtMeteoOrari = xMeteoDAL.Dati_Completi(
                ID_QuadranteStazione:=ID_QuadranteStazione,
                OraRilievo:=OraRilievo,
                DataInizio:=DataInizio,
                DataFine:=DataFine,
                Flag_Quadrante_Stazione:=Flag_Quadrante_Stazione,
                flag_tabella_leggi:=flag_tabella_leggi,
                NomeStazione:=nomeStazione,
                objParametri:=ObjParametri_Server
                )


            Dim ApplicaSogliaBagnatura As Boolean = False
            If flag_tabella_leggi = "M" Then

                Dim dtf = xMeteoDAL.LeggiFornitoreDaStazione(nomeStazione, ObjParametri_Server)

                If dtf.Rows.Count > 0 Then
                    Dim fCod As Integer = dtf.Rows(0)("fornitore_cod")
                    If fCod = enum_Meteo_Agronica_Fornitori_Cod.Agricultural_Support OrElse fCod = enum_Meteo_Agronica_Fornitori_Cod.WiNet Then
                        ApplicaSogliaBagnatura = True
                    End If
                End If
            End If

            Dim pioggia As Single
            Dim h As Integer

            i = 0
            J = 0
            Giorno = 0
            pioggia = 0

            For Each rs As DataRow In dtMeteoOrari.Rows

                d = DateToGiuliano(rs("TEMPO"))
                h = Hour(rs("TEMPO"))
                If i <> d And J <> h Then
                    vMancanoOrari = True
                End If

                If rs("TEMPO") > vUltimaDataOrari Then
                    vUltimaDataOrari = rs("TEMPO")
                End If

                Dim cOrari As New clDatiOrari

                cOrari.Data = CType(rs("TEMPO"), Date).ToShortDateString
                cOrari.ora = Hour(rs("TEMPO"))
                cOrari.Data_Agg = rs("Data_Agg")
                cOrari.Temp = IIf(IsDBNull(rs("Temp_Media")), 0, Val(Valore(rs("Temp_Media") & "")))
                cOrari.Prec = IIf(IsDBNull(rs("Precipitazione")), 0, Val(Valore(rs("Precipitazione") & "")))
                cOrari.Ur = IIf(IsDBNull(rs("UmiditaRelativa")), 0, Val(Valore(rs("UmiditaRelativa") & "")))
                cOrari.Bagnat = IIf(IsDBNull(rs("Bagnatura")), 0, Val(Valore(rs("Bagnatura") & "")))
                If dtMeteoOrari.Columns.Contains("UmiditaSuolo") Then
                    cOrari.UmiditaSuolo = IIf(IsDBNull(rs("UmiditaSuolo")), 0, Val(Valore(rs("UmiditaSuolo") & "")))
                Else
                    cOrari.UmiditaSuolo = 0
                End If
                cOrari.Mancante = vMancanoOrari

                If ApplicaSogliaBagnatura Then
                    cOrari.Bagnat = If(cOrari.Bagnat > 15, 1, 0)
                End If

                ' VAnni: 3/4/2017: TODO: verificare la lettura di -999
                If cOrari.Prec = PioggieNull Then
                    cOrari.Prec = 0
                End If

                If vGiornalieri IsNot Nothing Then

                    If d = Giorno Then
                        pioggia = pioggia + cOrari.Prec
                    Else
                        vGiornalieri(Giorno).Prec = IIf(vGiornalieri(Giorno).Prec = 0, pioggia, vGiornalieri(Giorno).Prec)
                        pioggia = cOrari.Prec
                        Giorno = d
                    End If

                End If

                If vOrari Is Nothing Then
                    vOrari = New List(Of clDatiOrari)
                End If
                vOrari.Add(cOrari)

                i = i + 1
                J = (J + 1) Mod 24

            Next

        End If

        Return ""

    End Function

End Class



Public Class InterfacciaMeteoLight

    Private _ObjParametri_Server As AgronicaCoreParametri
    Private _ErrMsg As String

    Public ReadOnly Property ErrorMsg As String
        Get
            Return _ErrMsg
        End Get
    End Property

    Public Sub New(ByVal ObjParametri_Server As AgronicaCoreParametri)
        _ObjParametri_Server = ObjParametri_Server
        _ErrMsg = ""
    End Sub

    Public Function DatiStazione(ByVal TipoSorgente As Integer, ByVal Sorgente As Integer, ByVal NumOre As Integer) As AgronicaCoreMeteoBiz.MeteoStazione

        Dim xMeteoDAL As New AgronicaCoreMeteoDAL.Meteo_R

        Dim dtMeteoOrari As DataTable
        Dim dtStazioneCfg As New DataTable
        Dim StazioneDes As String = ""

        dtMeteoOrari = xMeteoDAL.UltimiDatiOrari(TipoSorgente, Sorgente, NumOre, _ObjParametri_Server, StazioneDes, dtStazioneCfg)

        If dtMeteoOrari Is Nothing Then
            Return Nothing
        End If

        Dim stazione As New AgronicaCoreMeteoBiz.MeteoStazione

        stazione.Stazione = StazioneDes

        If TipoSorgente = enum_Meteo_Tiposorgente.Gias_RER Or TipoSorgente = enum_Meteo_Tiposorgente.Gias_RER_Quadranti Then
            stazione.BaseUnit = "hours"
        Else
            stazione.BaseUnit = "minutes"
        End If

        For Each dr In dtStazioneCfg.Rows
            Dim mc As New AgronicaCoreMeteoBiz.MeteoStazione.MeteoConfigurazione
            mc.Colonna = dr("Colonna")
            mc.Descrizione = dr("Descrizione")
            mc.Simbolo = dr("Simbolo")
            mc.AggFun = dr("AggFun")
            mc.Colore = dr("Colore")
            mc.Grafico = dr("Grafico")
            mc.Gruppo = dr("Gruppo")
            mc.Ordine = dr("Ordine")

            stazione.Configurazione.Add(mc)
        Next

        For Each row In dtMeteoOrari.Rows

            Dim dm As New AgronicaCoreMeteoBiz.MeteoStazione.MeteoDato(row("DataOraRilievo"))

            For Each e In stazione.Configurazione
                Dim v = row(e.Colonna)
                'dm.Values.Add(IIf(IsDBNull(v), 0, Val(Valore(v & ""))))
                If IsDBNull(v) Then
                    dm.Values.Add("")
                Else
                    dm.Values.Add(CDec(v).ToString(Globalization.CultureInfo.InvariantCulture))
                End If
            Next

            stazione.Meteo.Add(dm)

        Next

        Return stazione
    End Function

    Public Function DatiOrari(ByVal TipoSorgente As Integer, ByVal ID_Stazione As Integer, ByVal DataInizio As Date, ByVal DataFine As Date, ByVal ModelliPrevisionali_InputData_Meteo As String) As List(Of clDatiOrari)

        '-----------------------------------------------------------------------
        'Recupero i parametri
        '-----------------------------------------------------------------------
        'Dim ErrCod As Integer
        'Dim ErrMsg As String = ""
        'Dim xmlParametri As String = xSicurezza.Decodifica(ModelliPrevisionali_InputData_Meteo, xCostanti.KeyEncoderDecoder, ErrCod, ErrMsg)
        'If String.IsNullOrEmpty(xmlParametri) Then
        '    _ErrMsg = ErrMsg
        '    Return Nothing
        'End If

        Dim xMeteoDAL As New AgronicaCoreMeteoDAL.Meteo_R

        Dim dtMeteoOrari As DataTable = xMeteoDAL.MeteoDatiOrari(TipoSorgente, ID_Stazione, DataInizio, DataFine, _ObjParametri_Server)

        Dim ApplicaSogliaBagnatura As Boolean = False

        If TipoSorgente = enum_Meteo_Tiposorgente.Aziendali OrElse TipoSorgente = enum_Meteo_Tiposorgente.RetiPartner Then

            Dim dtf = xMeteoDAL.LeggiFornitoreDaStazione(ID_Stazione, _ObjParametri_Server)

            If dtf.Rows.Count > 0 Then

                Dim fCod As Integer = dtf.Rows(0)("fornitore_cod")
                If fCod = enum_Meteo_Agronica_Fornitori_Cod.Agricultural_Support OrElse fCod = enum_Meteo_Agronica_Fornitori_Cod.WiNet Then
                    ApplicaSogliaBagnatura = True
                End If
            End If
        End If

        'Dim PioggieNull As Integer = -999

        Dim d_o As New List(Of clDatiOrari)

        For Each rs As DataRow In dtMeteoOrari.Rows

            'Dim cOrari As New clDatiOrari

            'cOrari.Data = CType(rs("TEMPO"), Date).ToShortDateString
            'cOrari.ora = Hour(rs("TEMPO"))
            'cOrari.Data_Agg = rs("Data_Agg")
            'cOrari.Temp = IIf(IsDBNull(rs("Temp_Media")), 0, Val(Valore(rs("Temp_Media") & "")))
            'cOrari.Prec = IIf(IsDBNull(rs("Precipitazione")), 0, Val(Valore(rs("Precipitazione") & "")))
            'cOrari.Ur = IIf(IsDBNull(rs("UmiditaRelativa")), 0, Val(Valore(rs("UmiditaRelativa") & "")))
            'cOrari.Bagnat = IIf(IsDBNull(rs("Bagnatura")), 0, Val(Valore(rs("Bagnatura") & "")))
            'If dtMeteoOrari.Columns.Contains("UmiditaSuolo") Then
            '    cOrari.UmiditaSuolo = IIf(IsDBNull(rs("UmiditaSuolo")), 0, Val(Valore(rs("UmiditaSuolo") & "")))
            'Else
            '    cOrari.UmiditaSuolo = 0
            'End If

            'If ApplicaSogliaBagnatura Then
            '    cOrari.Bagnat = If(cOrari.Bagnat > 15, 1, 0)
            'End If

            '' VAnni: 3/4/2017: TODO: verificare la lettura di -999
            'If cOrari.Prec = PioggieNull Then
            '    cOrari.Prec = 0
            'End If

            'd_o.Add(cOrari)

        Next

        Return d_o
    End Function

End Class

Public Class ParametriInterfacciaMeteo

    Public Property TipoSorgenteMeteo As enum_Meteo_Tiposorgente
    Public Property IdSorgenteMeteo As Integer
    Public Property DataInizio As Date
    Public Property DataFine As Date
    Public Property Frequenza As Char   'G -> Giornalieri, H -> Orari
    Public Property ParametriAggiuntivi As String
    Public Property InterfacciaModelli As ParametriInterfacciaModelli

    Private Function EncodeDecode(ByVal str_in As String, ByVal flagEncode As Boolean) As String

        Dim Chiave As String = xCostanti.KeyEncoderDecoder
        Dim ErrCod As Integer = 0
        Dim ErrMsg As String = ""
        Dim str_out As String = ""

        Dim i As Integer
        Dim A1 As Integer
        Dim A2 As Integer
        Dim KeyPos As Byte = 1

        Try

            If flagEncode Then

                str_in = str_in.Replace("<", "{")
                str_in = str_in.Replace(">", "}")

                For i = 1 To Len(str_in)

                    A1 = Asc(Mid(str_in, i, 1))
                    A2 = Asc(Mid(Chiave, KeyPos, 1))
                    str_out = str_out & Chr(A2 + A1)
                    KeyPos = KeyPos + 1
                    If KeyPos > Len(Chiave) Then KeyPos = 1
                Next

            Else

                For i = 1 To Len(str_in)
                    A1 = Asc(Mid(str_in, i, 1))
                    A2 = Asc(Mid(Chiave, KeyPos, 1))
                    str_out = str_out & Chr(A1 - A2)
                    KeyPos = KeyPos + 1
                    If KeyPos > Len(Chiave) Then KeyPos = 1
                Next

                str_out = str_out.Replace("{", "<")
                str_out = str_out.Replace("}", ">")

            End If

        Catch ex As Exception

            If flagEncode Then

                ErrCod = 452645
                ErrMsg = "[Codifica]:"

            Else

                ErrCod = 452644
                ErrMsg = "[Decodifica]:"

            End If
            ErrMsg &= ex.Message

            str_out = ""
        End Try

        Return str_out
    End Function

    Private Sub New()
        'solo per serializzazione
    End Sub

    Public Sub New(ByVal t_sorgente As enum_Meteo_Tiposorgente,
                   ByVal id_sorgente As Integer,
                   ByVal dtInizio As Date,
                   ByVal dtFine As Date,
                   Optional ByVal pAggiuntivi As String = "",
                   Optional ByVal iModelli As ParametriInterfacciaModelli = Nothing)
        TipoSorgenteMeteo = t_sorgente
        IdSorgenteMeteo = id_sorgente
        DataInizio = dtInizio
        DataFine = dtFine
        Frequenza = "G"c
        ParametriAggiuntivi = pAggiuntivi
        InterfacciaModelli = iModelli
    End Sub

    Public Sub New(ByVal xmlCodificato As String)

        Dim xmlParametri As String = EncodeDecode(xmlCodificato, False)

        Dim xD As XDocument = XDocument.Parse(xmlParametri)

        Dim jsonParametriAggiuntivi As String = ""
        Dim xPA = xD.Element("PARAMETRI").Element("ParametriAggiuntivi")
        If xPA IsNot Nothing Then
            If xPA.FirstNode() IsNot Nothing Then

                jsonParametriAggiuntivi = JsonConvert.SerializeXNode(xPA, Newtonsoft.Json.Formatting.None, True)
            End If

            xPA.Remove()
        End If

        Dim jsonInterfacciaModelli As String = ""
        Dim jsonParametriElaborazioneAggiuntivi As String = ""

        Dim xModelli = xD.Element("PARAMETRI").Element("InterfacciaModelli")
        If xModelli IsNot Nothing Then

            Dim xParAgg = xModelli.Element("ParametriElaborazioneAggiuntivi")
            If xParAgg IsNot Nothing Then

                If xParAgg.FirstNode() IsNot Nothing Then

                    jsonParametriElaborazioneAggiuntivi = JsonConvert.SerializeXNode(xParAgg, Newtonsoft.Json.Formatting.None, True)
                End If

                xParAgg.Remove()
            End If

            If xModelli.FirstNode() IsNot Nothing Then

                jsonInterfacciaModelli = JsonConvert.SerializeXNode(xModelli, Newtonsoft.Json.Formatting.None, True)
            End If

            xModelli.Remove()
        End If

        Dim json As String = JsonConvert.SerializeXNode(xD.Element("PARAMETRI"), Newtonsoft.Json.Formatting.None, True)

        Dim pim = JsonConvert.DeserializeObject(Of ParametriInterfacciaMeteo)(json)

        TipoSorgenteMeteo = pim.TipoSorgenteMeteo
        IdSorgenteMeteo = pim.IdSorgenteMeteo
        DataInizio = pim.DataInizio
        DataFine = pim.DataFine
        Frequenza = pim.Frequenza
        ParametriAggiuntivi = jsonParametriAggiuntivi
        InterfacciaModelli = Nothing

        If Not String.IsNullOrEmpty(jsonInterfacciaModelli) Then

            InterfacciaModelli = JsonConvert.DeserializeObject(Of ParametriInterfacciaModelli)(jsonInterfacciaModelli)
            InterfacciaModelli.ParametriElaborazioneAggiuntivi = ""
            If Not String.IsNullOrEmpty(jsonParametriElaborazioneAggiuntivi) Then

                InterfacciaModelli.ParametriElaborazioneAggiuntivi = jsonParametriElaborazioneAggiuntivi
            End If
        End If

    End Sub

    Public Function Encode(ByVal Username As String, ByVal Password As String) As String

        'crea xml x ws
        Dim Doorkey As String = "Y4h8u3B5w2"
        Dim Security_Token As String = ""

        Dim jsonMe As String = JsonConvert.SerializeObject(Me)
        Dim xD = JsonConvert.DeserializeXNode(jsonMe, "PARAMETRI")
        Dim Parametri As XElement = xD.Element("PARAMETRI")
        Parametri.SetAttributeValue("doorkey", Doorkey)
        Parametri.SetAttributeValue("usr", Username)
        Parametri.SetAttributeValue("pwd", Password)
        Parametri.SetAttributeValue("tkn", Security_Token)

        If Not String.IsNullOrEmpty(ParametriAggiuntivi) Then
            'Il contenuto è un JSON per cui lo devo traformare in XML a sua volta altrimenti la funzione di codifica fallisce
            Parametri.Element("ParametriAggiuntivi").Remove()
            Dim xD_pa = JsonConvert.DeserializeXNode(ParametriAggiuntivi, "ParametriAggiuntivi")
            Parametri.Add(xD_pa.FirstNode())
        End If

        If InterfacciaModelli IsNot Nothing Then
            If Not String.IsNullOrEmpty(InterfacciaModelli.ParametriElaborazioneAggiuntivi) Then
                'Il contenuto è un JSON per cui lo devo traformare in XML a sua volta altrimenti la funzione di codifica fallisce
                Parametri.Element("InterfacciaModelli").Element("ParametriElaborazioneAggiuntivi").Remove()
                Dim xD_pa = JsonConvert.DeserializeXNode(InterfacciaModelli.ParametriElaborazioneAggiuntivi, "ParametriElaborazioneAggiuntivi")
                Parametri.Element("InterfacciaModelli").Add(xD_pa.FirstNode())
            End If
        End If

        Return EncodeDecode(xD.ToString(), True)
    End Function

End Class

Public Class ParametriInterfacciaModelli
    Public Property IdModello As Integer
    Public Property Veg_Cod As Integer
    Public Property Av_Cod As Integer
    Public Property Algoritmo As Integer
    Public Property ParametriElaborazioneAggiuntivi As String
End Class

