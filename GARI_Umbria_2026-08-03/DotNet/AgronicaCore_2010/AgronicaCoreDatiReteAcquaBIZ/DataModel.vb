Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class Risposta_ElencoGruppiConsegna
    Public Property elenco As List(Of GruppoConsegna)

    Public Sub New()
        elenco = New List(Of GruppoConsegna)
    End Sub
End Class

Public Class GruppoConsegna
    Public Property id As Integer = 0
    Public Property desc As String = ""
End Class

Public Class Risposta_DatiReteAcqua
    'Public Property IoTReteAcqua_Table() As String
    'Public Property IoTReteAcqua_Charts() As String
    'Public Property IoTReteAcqua_RiepilogoPeriodo() As String
    'Public Property IoTReteAcqua_RiepilogoSensori() As String

    'temporaneo fino a che non verrà portato tutto altrove.....
    Public Property Meteo_Table() As String
    Public Property Meteo_Charts() As String

    Public Property Chart_Type As String
    Public Property Meteo_RiepilogoPeriodo() As String
    Public Property Meteo_RiepilogoSensori() As String
End Class

Public Class Risposta_DatiReteAcquaCalcoloConfronto
    Public Property Table() As String
    Public Property Charts() As List(Of Risposta_DatiReteAcquaCalcoloConfronto_Charts)
End Class

Public Class Risposta_DatiReteAcquaCalcoloConfronto_Charts
    Public Property ChartID As String
    Public Property ChartData() As String
End Class

Public Class DatiReteAcqua_Chart(Of T)
    Public Property title As String
    Public Property xValuesDateType As Boolean
    Public Property xValues As List(Of T)     'sarà associato alle categorie dell'oggetto kendo
    Public Property yValues As List(Of Chart_LineWithValues)    'elenco delle linee con valori

    Public Sub New()
        xValuesDateType = False
        xValues = New List(Of T)
        yValues = New List(Of Chart_LineWithValues)
    End Sub
End Class

Public Class Chart_LineWithValues
    Public Property name As String
    Public Property color As String
    Public Property dashType As String
    Public Property data As List(Of Decimal)

    Public Sub New()
        dashType = "solid"
        data = New List(Of Decimal)
    End Sub
End Class

Public Class Risposta_DatiReteAcquaAnagrafe
    Public Property data As List(Of DatiReteAcquaCaratteristica)
    Public Property image As String

    Public Sub New()
        data = New List(Of DatiReteAcquaCaratteristica)
    End Sub
End Class

Public Class DatiReteAcquaCaratteristica
    Public Property Id As String
    Public Property Value As String
End Class

Public Class Risposta_DatiReteAcquaPrelieviStorici
    Public Property columns As String
    Public Property model As String
    Public Property data As String

End Class

Public Class Risposta_DatiReteAcquaParametri
    Public Property par_generali As Risposta_DatiReteAcquaPars
    Public Property coeffxspecie As Risposta_DatiReteAcquaPars

    Public Sub New()
        par_generali = New Risposta_DatiReteAcquaPars
        coeffxspecie = New Risposta_DatiReteAcquaPars
    End Sub

End Class

Public Class Risposta_DatiReteAcquaPars
    Public Property columns As String
    Public Property model As String
    Public Property data As String
End Class

Public Class Risposta_DatiReteAcquaPrelieviStoriciXColtura
    Public Property cul_cod As Integer
    Public Property dati As List(Of Double)

    Public Sub New()
        cul_cod = -1
        dati = New List(Of Double)
    End Sub
End Class

Public Class DatiReteAcqua_ColumnDefinition
    Public Property id As String
    Public Property desc As String
End Class

Public Class CoefficientiXSpecieVegetale
    Public Property Settimana As Integer
    Public Property Kc As Decimal      'coeff colturale per evapotraspirazione
    Public Property DFc As Decimal       'coeff colturale per deficit
End Class

Public Class TipiSensoreXStazione
    Public Property id_sensore As Integer
    Public Property id_tipoSensore As Integer
    Public Property tipo As String
    Public Property um As String
End Class

Public Class DatiReteAcqua_ParametriGenerali
    'Public Property Anno As Integer
    Public Property Settimana As Integer
    Public Property RadiazioneExtraterrestre As Decimal

End Class

Public Class ParametriSpecie
    Public Property Kc As Decimal      'coeff colturale per evapotraspirazione
    Public Property DFc As Decimal       'coeff colturale per deficit
End Class

Public Class ParametriXSpecie
    Public Property Veg_Cod As Integer
    Public Property superficie As Decimal   'superficie colturale
    Public Property pars As ParametriSpecie

    Public Sub New()
        pars = New ParametriSpecie
    End Sub

End Class

Public Class ParametriCalcolo
    Public Property Anno As Integer
    Public Property Settimana As Integer
    Public Property RadiazioneExtraterrestre As Decimal
    Public Property ParametriXSpecie As List(Of ParametriXSpecie)

    Public Sub New()
        ParametriXSpecie = New List(Of ParametriXSpecie)
    End Sub

    Public Function GetSumArea() As Decimal
        Dim ret As Decimal = 0
        For Each specie In ParametriXSpecie
            ret += specie.superficie
        Next
        Return ret
    End Function

End Class

Public Class DatiAcquaXCalcoloConfronto
    Public Property anno As Integer
    Public Property settimana As Integer
    Public Property CumulatoFornitore As List(Of Decimal)
    Public Property CumulatoFornitoreProgressivo As List(Of Decimal)

    Public Sub New()
        anno = -1
        settimana = -1
        CumulatoFornitore = New List(Of Decimal)
        CumulatoFornitoreProgressivo = New List(Of Decimal)
    End Sub

    Public Function GetCumulatoFornitore() As Decimal
        Return Math.Round(CumulatoFornitore.Sum() / CumulatoFornitore.Count, 2)
    End Function
    Public Function GetCumulatoFornitoreProgressivo() As Decimal
        Return Math.Round(CumulatoFornitoreProgressivo.Sum() / CumulatoFornitoreProgressivo.Count, 2)
    End Function
End Class

Public Class DatiMeteoXCalcoloConfronto
    Public Property anno As Integer
    Public Property settimana As Integer
    Public Property pioggia As List(Of Decimal)
    Public Property temp_min As List(Of Decimal)
    Public Property temp_media As List(Of Decimal)
    Public Property temp_max As List(Of Decimal)

    Public Sub New()
        anno = -1
        settimana = -1
        pioggia = New List(Of Decimal)
        temp_min = New List(Of Decimal)
        temp_media = New List(Of Decimal)
        temp_max = New List(Of Decimal)
    End Sub

    Public Function GetPioggia() As Decimal
        Return Math.Round(pioggia.Sum() / pioggia.Count, 1)
    End Function
    Public Function GetTemperaturaMinima() As Decimal
        Return Math.Round(temp_min.Sum() / temp_min.Count, 2)
    End Function
    Public Function GetTemperaturaMedia() As Decimal
        Return Math.Round(temp_media.Sum() / temp_media.Count, 2)
    End Function
    Public Function GetTemperaturaMassima() As Decimal
        Return Math.Round(temp_max.Sum() / temp_max.Count, 2)
    End Function

    Public Function GetET0_Rif(ByVal radiazione_extraterrestre As Decimal) As Decimal
        Return (0.881 * Math.Round(0.0023 * (GetTemperaturaMedia() + 17.8) * ((GetTemperaturaMassima() - GetTemperaturaMinima()) ^ 0.5) * radiazione_extraterrestre * 7, 1)) - 2.382
    End Function

End Class


Public Class DatiPrelieviCalcolo
    Public Property anno As Integer
    Public Property settimana As Integer
    Public Property pioggia As Decimal
    Public Property temp_max As Decimal
    Public Property temp_min As Decimal
    Public Property temp_media As Decimal
    Public Property radiazione_extra As Decimal
    Public Property evapotraspirazione_riferimento As Decimal
    Public Property DatiXSpecie As List(Of DatiPrelieviCalcoloXSpecie)
    Public Property DatiStoriciXSpecie As List(Of DatiPrelieviStoriciXSpecie)
    Public Property PrelievoIdricoCumulatoFornitore As Decimal
    Public Property PrelievoIdricoSettimanaleFornitore As Decimal
    Public Property PrelievoIdricoCumulatoUnitarioOsservato As Decimal
    Public Property FabbisognoIdricoCumulato_FULL As Decimal
    Public Property FabbisognoIdricoCumulato_DEFICIT As Decimal
    Public Property PrelievoIdricoCumulatoUnitarioPrevisto_FULL As Decimal
    Public Property PrelievoIdricoCumulatoUnitarioPrevisto_DEFICIT As Decimal
    Public Property PrelievoIdricoCumulatoUnitarioStorico_FULL As Decimal
    Public Property PrelievoIdricoCumulatoUnitarioStorico_DEFICIT As Decimal
    Public Property PrelievoIdricoCumulatoStorico_FULL As Decimal
    Public Property PrelievoIdricoCumulatoStorico_DEFICIT As Decimal

    Public Sub New()
        anno = -1
        settimana = -1
        temp_max = 0
        temp_media = 0
        temp_min = 0
        DatiXSpecie = New List(Of DatiPrelieviCalcoloXSpecie)
        DatiStoriciXSpecie = New List(Of DatiPrelieviStoriciXSpecie)
    End Sub

    Public Function GetSumArea() As Decimal
        Dim ret As Decimal = 0
        For Each specie In DatiXSpecie
            ret += specie.SuperficieTotaleColtura
        Next
        Return ret
    End Function

    'Public Function GetPrelievoStoricoDEFICIT() As Decimal
    '    Dim ret As Decimal = 0
    '    For Each specie In DatiStoriciXSpecie
    '        ret += specie.PrelievoStoricoDEFICIT
    '    Next
    '    Return ret
    'End Function

    'Public Function GetPrelievoStoricoFULL() As Decimal
    '    Dim ret As Decimal = 0
    '    For Each specie In DatiStoriciXSpecie
    '        ret += specie.PrelievoStoricoFULL
    '    Next
    '    Return ret
    'End Function

End Class

Public Class DatiPrelieviStoriciXSpecie
    Public Property veg_cod As Integer
    Private _coeffdeficit As Decimal
    Private _prelievostoricofull As Decimal
    Private _prelievostoricodeficit As Decimal
    Private _superficie As Decimal

    Public ReadOnly Property PrelievoStoricoFULL() As Decimal
        Get
            Return _prelievostoricofull
        End Get
    End Property

    Public ReadOnly Property PrelievoStoricoDEFICIT() As Decimal
        Get
            Return _prelievostoricodeficit
        End Get
    End Property

    Public ReadOnly Property SuperficieColturale() As Decimal
        Get
            Return _superficie
        End Get
    End Property

    Public Sub New(ByVal Specie As Integer, ByVal CoeffDeficit As Decimal, ByVal StoricoFull As Decimal, ByVal StoricoDeficit As Decimal, ByVal Superficie As Decimal)
        veg_cod = Specie
        _coeffdeficit = CoeffDeficit
        _prelievostoricofull = StoricoFull
        _prelievostoricodeficit = StoricoDeficit
        _superficie = Superficie
        If StoricoDeficit = 0 Then
            _prelievostoricodeficit = _prelievostoricofull * _coeffdeficit
        End If
    End Sub
End Class

Public Class DatiPrelieviCalcoloXSpecie
    Public Property veg_cod As Integer
    Public Property veg_des As String

    Private _coeffevapotraspirazione As Decimal
    Private _evapotraspirazione As Decimal
    Private _fabbisognoidrico As Decimal
    Private _previsioneFull As Decimal
    Private _previsioneDeficit As Decimal
    Private _superficie As Decimal
    Private _coeffDeficit As Decimal

    Public ReadOnly Property CoeffEvapotraspirazione() As Decimal
        Get
            Return _coeffevapotraspirazione
        End Get
    End Property

    Public ReadOnly Property CoefficienteDeficit() As Decimal
        Get
            Return _coeffDeficit
        End Get
    End Property

    Public ReadOnly Property EvapoTraspirazione() As Decimal
        Get
            Return _evapotraspirazione
        End Get
    End Property

    Public ReadOnly Property FabbisognoIdrico() As Decimal
        Get
            Return _fabbisognoidrico
        End Get
    End Property

    Public ReadOnly Property PrevisioneCumulataFullIrrigation() As Decimal
        Get
            Return _previsioneFull
        End Get
    End Property
    Public ReadOnly Property PrevisioneCumulataDeficitIrrigation() As Decimal
        Get
            Return _previsioneDeficit
        End Get
    End Property

    Public ReadOnly Property SuperficieTotaleColtura() As Decimal
        Get
            Return _superficie
        End Get
    End Property

    Public Sub New(ByVal Settimana As Integer,
                   ByVal Specie As Integer,
                   ByVal Et0 As Decimal, ByVal CoeffXSpecie As Decimal, ByVal Pioggia As Decimal,
                   ByVal CoeffDeficit As Decimal, ByVal Superficie As Decimal,
                   Optional ByVal PrevisionePrecedente As Decimal = 0
                   )
        veg_cod = Specie
        _superficie = Superficie
        _coeffevapotraspirazione = CoeffXSpecie
        _coeffDeficit = CoeffDeficit
        If Et0 < 0 Then
            _evapotraspirazione = 0
        Else
            If Specie = 298 Then
                If Settimana >= 23 And Settimana <= 40 Then
                    _evapotraspirazione = 20
                Else
                    _evapotraspirazione = 0
                End If
            Else
                _evapotraspirazione = Et0 * _coeffevapotraspirazione
            End If
        End If
        If Pioggia < 0 Then
            _fabbisognoidrico = -1
            _previsioneFull = -1
            _previsioneDeficit = -1
        Else
            _fabbisognoidrico = IIf(Pioggia < _evapotraspirazione, _evapotraspirazione - Pioggia, 0)
            _previsioneFull = _fabbisognoidrico * 10 / 0.7 + PrevisionePrecedente
            _previsioneDeficit = _previsioneFull * _coeffDeficit
        End If
    End Sub
End Class

'Public Class DatiPrelieviXSpecie
'    Public Property anno As Integer
'    Public Property settimana As Integer
'    Public Property veg_cod As Integer
'    Public ReadOnly Property EvapoTraspirazione() As Decimal
'        Get
'            Return _evapotraspirazione
'        End Get
'    End Property

'    Public ReadOnly Property FabbisognoIdrico() As Decimal
'        Get
'            Return _fabbisognoidrico
'        End Get
'    End Property

'    Public ReadOnly Property PrevisioneCumulataFullIrrigation() As Decimal
'        Get
'            Return _previsioneFull
'        End Get
'    End Property
'    Public ReadOnly Property PrevisioneCumulataDeficitIrrigation() As Decimal
'        Get
'            Return _previsioneDeficit
'        End Get
'    End Property

'    Public ReadOnly Property SuperficieTotaleColtura() As Decimal
'        Get
'            Return _superficie
'        End Get
'    End Property

'    Private _evapotraspirazione As Decimal
'    Private _fabbisognoidrico As Decimal
'    Private _previsioneFull As Decimal
'    Private _previsioneDeficit As Decimal
'    Private _superficie As Decimal

'    Public Sub New(ByVal Anno As Integer,
'                   ByVal Settimana As Integer,
'                   ByVal Specie As Integer,
'                   ByVal Et0 As Decimal, ByVal CoeffXSpecie As Decimal, ByVal Pioggia As Decimal,
'                   ByVal CoeffDeficit As Decimal, ByVal Superficie As Decimal,
'                   Optional ByVal PrevisionePrecedente As Decimal = 0
'                   )
'        Anno = Anno
'        Settimana = Settimana
'        veg_cod = Specie
'        _superficie = Superficie
'        If Et0 < 0 Then
'            _evapotraspirazione = 0
'        Else
'            If Specie = 298 Then
'                If Settimana >= 23 And Settimana <= 40 Then
'                    _evapotraspirazione = 20
'                Else
'                    _evapotraspirazione = 0
'                End If
'            Else
'                _evapotraspirazione = Et0 * CoeffXSpecie
'            End If
'        End If
'        If Pioggia < 0 Then
'            _fabbisognoidrico = -1
'            _previsioneFull = -1
'            _previsioneDeficit = -1
'        Else
'            _fabbisognoidrico = IIf(Pioggia < _evapotraspirazione, _evapotraspirazione - Pioggia, 0)
'            _previsioneFull = _fabbisognoidrico * 10 / 0.7 + PrevisionePrecedente
'            _previsioneDeficit = _previsioneFull * CoeffDeficit
'        End If
'    End Sub

'End Class

'Public Class DatiPrelieviCumulati
'    Public Property Anno As Integer
'    Public Property Settimana As Integer
'    Public Property PrelievoCumulatoFornitore As Decimal
'    Public Property PrelievoCumulatoProgressivo As Decimal
'    Public Property CumulatoUnitarioOsservato As Decimal        'cumulato progressivo / totale area colturale
'    Public Property PrelievoCumulatoFULL As Decimal
'    Public Property PrelievoCumulatoDEFICIT As Decimal
'    Public Property PrelievoCumulatoFULL_Tr10Anni As Decimal
'    Public Property PrelievoCumulatoDEFICIT_Tr10Anni As Decimal
'    Public Property PrelievoCumulatoUnitarioPrevisto_FULL As Decimal
'    Public Property PrelievoCumulatoUnitarioPrevisto_DEFICIT As Decimal

'End Class
