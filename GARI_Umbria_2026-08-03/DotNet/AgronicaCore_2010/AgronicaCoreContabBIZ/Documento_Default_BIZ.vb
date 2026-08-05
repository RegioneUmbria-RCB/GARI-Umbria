Imports AgronicaCoreContabDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class Documento_Default_BIZ
    Inherits AgronicaCoreDataProvider.LogProvider

#Region "Costruttori"

    'TODO i Conferimenti sono al momento mappati nel gruppo documenti emessi (V)

    Private ReadOnly _elencoCausali As List(Of CausaleDocumento) = New List(Of CausaleDocumento) From {
            New CausaleDocumento With {.TYPE = "A", .DOC_TYPE = "F", .LAV_COD = "1000", .LAV_DES = "Fattura ricevuta"},
            New CausaleDocumento With {.TYPE = "A", .DOC_TYPE = "F", .LAV_COD = "1002", .LAV_DES = "Nota di credito ricevuta"},
            New CausaleDocumento With {.TYPE = "V", .DOC_TYPE = "F", .LAV_COD = "1001", .LAV_DES = "Fattura emsessa"},
            New CausaleDocumento With {.TYPE = "V", .DOC_TYPE = "F", .LAV_COD = "1003", .LAV_DES = "Nota di credito emessa"},
            New CausaleDocumento With {.TYPE = "A", .DOC_TYPE = "C", .LAV_COD = "1025", .LAV_DES = "DDT ricevuto"},
            New CausaleDocumento With {.TYPE = "V", .DOC_TYPE = "C", .LAV_COD = "1031", .LAV_DES = "DDT emesso"},
            New CausaleDocumento With {.TYPE = "V", .DOC_TYPE = "C", .LAV_COD = "1069", .LAV_DES = "DDT corrispettivo"},
            New CausaleDocumento With {.TYPE = "V", .DOC_TYPE = "C", .LAV_COD = "1054", .LAV_DES = "Accettazione DDT ricevuto"},
            New CausaleDocumento With {.TYPE = "V", .DOC_TYPE = "C", .LAV_COD = "1051", .LAV_DES = "Accettazione"},
            New CausaleDocumento With {.TYPE = "V", .DOC_TYPE = "C", .LAV_COD = "1075", .LAV_DES = "Distinta di carico"},
            New CausaleDocumento With {.TYPE = "A", .DOC_TYPE = "C", .LAV_COD = "1076", .LAV_DES = "Accettazione distinta di carico"},
            New CausaleDocumento With {.TYPE = "A", .DOC_TYPE = "C", .LAV_COD = "1077", .LAV_DES = "Auto DDT emesso"},
            New CausaleDocumento With {.TYPE = "A", .DOC_TYPE = "C", .LAV_COD = "1078", .LAV_DES = "Accettazione Auto DDT emesso"},
            New CausaleDocumento With {.TYPE = "V", .DOC_TYPE = "O", .LAV_COD = "2002", .LAV_DES = "Ordine di vendita"},
            New CausaleDocumento With {.TYPE = "A", .DOC_TYPE = "O", .LAV_COD = "2004", .LAV_DES = "Ordine di acquisto"}
        }

    Public ReadOnly Property ElencoCausali As List(Of CausaleDocumento)
        Get
            Return _elencoCausali
        End Get
    End Property

    Public Sub New()

    End Sub

#End Region

#Region "Metodi Pubblici"

    Public Function NumeratoriEDefaults(ByVal data_Documento As Date,
                                        ByVal piva As String,
                                        ByVal sa_cod As Integer?,
                                        ByVal lav_cod As Integer?,
                                        ByVal fattura_accompagnatoria As Boolean,
                                        ByVal objParametri_Server As AgronicaCoreParametri
                                        ) As NumeratoriDefaults

        'Dim lav_cod_gruppo As Integer
        Dim lav_cod_gruppo = 0
        Dim gruppo = _elencoCausali.Where(Function(s) s.LAV_COD = lav_cod).FirstOrDefault()
        If Not gruppo Is Nothing Then
            lav_cod_gruppo = If(gruppo.TYPE = "A", -10000, -10001)
        End If

        Dim objDefault = New Documento_Default_R
        Dim dtNumeratori = objDefault.RicercaNumeratori(data_Documento, piva, sa_cod,
                                                   lav_cod, lav_cod_gruppo, fattura_accompagnatoria, Nothing, Nothing, Nothing,
                                                   Nothing, "", "", objParametri_Server)
        Dim dtDefault = objDefault.RicercaDefaults(piva, sa_cod,
                                                   lav_cod, lav_cod_gruppo, fattura_accompagnatoria, Nothing, Nothing, Nothing,
                                                   Nothing, "", "", objParametri_Server)

        ' -------------------------------------------------------------------------------------------------
        ' NUMERATORI E LORO DEFAULT
        ' -------------------------------------------------------------------------------------------------

        Dim numeratoriTrovati = (From d In dtNumeratori.AsEnumerable
                                 Select New Numeratore With {
                                    .Piva = d.Item("Piva").ToString(),
                                    .Numeratore_Tipo = CInt(d.Item("Numeratore_Tipo")),
                                    .Doc_Numero_Sin = d.Item("Doc_Numero_Sin").ToString,
                                    .Doc_Numero_Des = d.Item("Doc_Numero_Des").ToString,
                                    .Validita_Inizio = If(d.Item("Validita_Inizio") Is DBNull.Value, AGRODATAINIZIO, CDate(d.Item("Validita_Inizio"))),
                                    .Validita_Fine = If(d.Item("Validita_Fine") Is DBNull.Value, AGRODATAFINE, CDate(d.Item("Validita_Fine"))),
                                    .PrefissoSuffisso_Des = d.Item("PrefissoSuffisso_Des").ToString,
                                    .Lunghezza_Centro = CInt(d.Item("Lunghezza_Centro")),
                                    .CarattereFormattazione = d.Item("CarattereFormattazione"),
                                    .Sigla = d.Item("Sigla"),
                                    .NumeratoreTipo_Des = d.Item("NumeratoreTipo_Des"),
                                    .Lav_Cod = If(d.Item("Lav_Cod") Is DBNull.Value, Nothing, CType(d.Item("Lav_Cod"), Integer?)),
                                    .Sa_Cod = If(d.Item("Sa_Cod") Is DBNull.Value, Nothing, CType(d.Item("Sa_Cod"), Integer?)),
                                    .TipoFattura = If(d.Item("TipoFattura") Is DBNull.Value, "", d.Item("TipoFattura").ToString()),
                                    .NumTipo_Cod = If(d.Item("NumTipo_Cod") Is DBNull.Value, Nothing, CType(d.Item("NumTipo_Cod"), Integer?)),
                                    .Vincolante = If(d.Item("Vincolante") Is DBNull.Value, False, CBool(d.Item("Vincolante"))),
                                    .IsDefault = False
                                }).ToList

        Dim numeratoriRaggruppati = (From d In numeratoriTrovati
                                     Order By d.Numeratore_Tipo
                                     Group By chiave = d.Numeratore_Tipo
                                     Into nRagg = Group, Count())

        Dim Numeratori = New List(Of Numeratore)
        For Each nGroup In numeratoriRaggruppati
            Numeratori.AddRange(nGroup.nRagg.Take(1))
        Next

        Dim numeratoreDefault As Object = Nothing
        Dim numeratoriConDefault = numeratoriTrovati.Where(Function(s) Not s.NumTipo_Cod Is Nothing)
        If numeratoriConDefault.Any Then

            numeratoreDefault = EseguiCatenaRicerca(sa_cod, lav_cod, fattura_accompagnatoria, numeratoriConDefault)
            If Not numeratoreDefault Is Nothing Then
                Numeratori.FirstOrDefault(Function(s) s.Numeratore_Tipo = numeratoreDefault.Numeratore_Tipo).IsDefault = True
            End If

        End If

        ' -------------------------------------------------------------------------------------------------
        ' CAUSALE E SEZIONALE DEFAULT
        ' -------------------------------------------------------------------------------------------------

        Dim defaultsTrovati = From d In dtDefault.AsEnumerable
                              Select New With
                           {
                                .Lav_Cod = If(d.Item("Lav_Cod") Is DBNull.Value, Int32.MinValue, CInt(d.Item("Lav_Cod"))),
                                .Sa_Cod = If(d.Item("Sa_Cod") Is DBNull.Value, Int32.MinValue, CInt(d.Item("Sa_Cod"))),
                                .TipoFattura = If(d.Item("TipoFattura") Is DBNull.Value, "", d.Item("TipoFattura").ToString()),
                                .NumTipo_Cod = If(d.Item("NumTipo_Cod") Is DBNull.Value, Int32.MinValue, CInt(d.Item("NumTipo_Cod"))),
                                .Sezionale_Cod = If(d.Item("Sezionale_Cod") Is DBNull.Value, Nothing, CType(d.Item("Sezionale_Cod"), Integer?)),
                                .Causale_Cod = If(d.Item("Causale_Cod") Is DBNull.Value, Nothing, CType(d.Item("Causale_Cod"), Integer?))
                           }

        ' SEZIONALE
        Dim sezionaleDefault As Object = Nothing
        Dim defaultsConSezionale = defaultsTrovati.Where(Function(s) Not s.Sezionale_Cod Is Nothing)
        If defaultsConSezionale.Any Then
            sezionaleDefault = EseguiCatenaRicerca(sa_cod, lav_cod, fattura_accompagnatoria, defaultsConSezionale)
        End If

        ' CAUSALE
        Dim causaleDefault As Object = Nothing
        Dim defaultsConCausale = defaultsTrovati.Where(Function(s) Not s.Causale_Cod Is Nothing)
        If defaultsConCausale.Any Then
            causaleDefault = EseguiCatenaRicerca(sa_cod, lav_cod, fattura_accompagnatoria, defaultsConCausale)
        End If

        Return New NumeratoriDefaults With {
            .Numeratori = Numeratori,
            .Causale_Cod = If(Not causaleDefault Is Nothing, causaleDefault.Causale_Cod, Nothing),
            .Sezionale_Cod = If(Not sezionaleDefault Is Nothing, sezionaleDefault.Sezionale_Cod, Nothing)
        }

    End Function

    Private Function EseguiCatenaRicerca(ByVal sa_cod As Integer?,
                                         ByVal lav_cod As Integer?,
                                         ByVal fattura_accompagnatoria As Boolean,
                                         ByVal listaRicerca As IEnumerable(Of Object)) As Object

        Dim oggettoDefault As Object = Nothing

        Dim tipiFattura = New List(Of String)
        If fattura_accompagnatoria Then
            tipiFattura.AddRange(New List(Of String) From {"A", " "})
        Else
            tipiFattura.AddRange(New List(Of String) From {"D", " "})
        End If
        Dim gruppiLavCod As New List(Of Integer) From {-10000, -10001}

        Dim lav_cod_gruppo = 0
        Dim gruppo = _elencoCausali.Where(Function(s) s.LAV_COD = lav_cod).FirstOrDefault()
        If Not gruppo Is Nothing Then
            lav_cod_gruppo = If(gruppo.TYPE = "A", -10000, -10001)
        End If

        ' imposto quello di default
        If listaRicerca.Count = 1 Then
            oggettoDefault = listaRicerca.FirstOrDefault
        Else

            ' 1) prima cerco se c'è un default con corrispondenza esatta
            ' centro aziendale + coppia lav_cod, tipo fattura


            oggettoDefault = CercaNumeratoreODefault(sa_cod, lav_cod, tipiFattura, listaRicerca.ToList)
            If oggettoDefault Is Nothing Then
                '2) cerco per sa cod PUNTUALE e lav_cod gruppo
                oggettoDefault = CercaNumeratoreODefault(sa_cod, lav_cod_gruppo, tipiFattura, listaRicerca.ToList)
            End If

            If oggettoDefault Is Nothing Then
                '3) cerco per sa cod * e lav_cod puntuale
                oggettoDefault = CercaNumeratoreODefault(-1, lav_cod, tipiFattura, listaRicerca.ToList)
            End If

            If oggettoDefault Is Nothing Then
                '4) cerco per sa cod * e lav_cod gruppo
                oggettoDefault = CercaNumeratoreODefault(-1, lav_cod_gruppo, tipiFattura, listaRicerca.ToList)
            End If

            If oggettoDefault Is Nothing Then
                '5) cerco per sa cod puntuale e lav_cod *
                oggettoDefault = CercaNumeratoreODefault(sa_cod, 0, tipiFattura, listaRicerca.ToList)
            End If

            If oggettoDefault Is Nothing Then
                '6) cerco per sa cod * e lav_cod *
                oggettoDefault = CercaNumeratoreODefault(-1, 0, tipiFattura, listaRicerca.ToList)
            End If

        End If

        Return oggettoDefault

    End Function

    Private Function CercaNumeratoreODefault(ByVal sa_cod? As Integer,
                                     ByVal lav_cod? As Integer,
                                     ByVal tipoFattura As List(Of String),
                                     ByVal numeratori As IEnumerable(Of Object)) As Object

        Dim numDef = numeratori.Where(Function(s) tipoFattura.Contains(s.TipoFattura))
        If Not sa_cod Is Nothing Then
            numDef = numDef.Where(Function(s) s.Sa_Cod = sa_cod)
        End If
        If Not lav_cod Is Nothing Then
            numDef = numDef.Where(Function(s) s.Lav_Cod = lav_cod)
        End If

        'numDef = numeratori.FirstOrDefault(Function(s) s.Sa_Cod = sa_cod AndAlso s.Lav_Cod = lav_cod AndAlso tipoFattura.Contains(s.TipoFattura))

        Return numDef.FirstOrDefault()

    End Function

    Private Function CercaNumeratoreODefault(ByVal sa_cod As Integer,
                                     ByVal lav_cod As List(Of Integer),
                                     ByVal tipoFattura As List(Of String),
                                     ByVal numeratori As IEnumerable(Of Object)) As Object

        Dim numDef As Object = Nothing
        numDef = numeratori.FirstOrDefault(Function(s) s.Sa_Cod = sa_cod AndAlso lav_cod.Contains(s.Lav_Cod) AndAlso tipoFattura.Contains(s.TipoFattura))
        Return numDef

    End Function

#End Region

End Class

Public Class CausaleDocumento

    Public TYPE As String
    Public DOC_TYPE As String
    Public LAV_COD As String
    Public LAV_DES As String

End Class

Public Class NumeratoriDefaults
    Public Property Numeratori As List(Of Numeratore)
    Public Property Causale_Cod As Integer?
    Public Property Sezionale_Cod As Integer?
End Class

Public Class Numeratore
    Public Property Piva As String
    Public Property Numeratore_Tipo As Integer
    Public Property Doc_Numero_Sin As String
    Public Property Doc_Numero_Des As String
    Public Property Validita_Inizio As Date
    Public Property Validita_Fine As Date
    Public Property PrefissoSuffisso_Des As String
    Public Property Lunghezza_Centro As Integer
    Public Property CarattereFormattazione As String
    Public Property Sigla As String
    Public Property NumeratoreTipo_Des As String
    Public Property Lav_Cod As Integer?
    Public Property Sa_Cod As Integer?
    Public Property TipoFattura As String
    Public Property NumTipo_Cod As Integer?
    Public Property Vincolante As Boolean
    Public Property IsDefault As Boolean
End Class