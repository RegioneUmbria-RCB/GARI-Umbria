
'Helper per il calcolo del BBCH...
'per ora a disposizione parametri per varietà:
' - Chardonnay
' - CabernetSauvignon


Public Interface IAgronomica30_BBCH_Fun
    Function Calc_BBCH_rip(nhhsum As Decimal) As Decimal 'Reproductive scale
    Function Calc_BBCH_veg(nhhsum As Decimal) As Decimal 'Vegetative scale
End Interface



Public Class Agronomica30_BBCH_Chardonnay
    Implements IAgronomica30_BBCH_Fun

    Public Function Calc_BBCH_rip(nhhsum As Decimal) As Decimal Implements IAgronomica30_BBCH_Fun.Calc_BBCH_rip
        '12 25 33 BBCH = 8 E(-12) NHHsum4 – 2 E(-8) NHHsum3 – E(-5) NHHsum2 +0.0432 NHHsum + 43.736
        Return ((((0.000000000007D * nhhsum) - 0.00000002D) * nhhsum - 0.000001D) * nhhsum + 0.0432D) * nhhsum + 43.736D
    End Function

    Public Function Calc_BBCH_veg(nhhsum As Decimal) As Decimal Implements IAgronomica30_BBCH_Fun.Calc_BBCH_veg
        '12 25 33 BBCH = 12.133 Ln(NHHsum) - 48.537
        Return 12.133D * Math.Log(nhhsum) - 48.537D
    End Function
End Class

Public Class Agronomica30_BBCH_CabernetSauvignon
    Implements IAgronomica30_BBCH_Fun

    Public Function Calc_BBCH_rip(nhhsum As Decimal) As Decimal Implements IAgronomica30_BBCH_Fun.Calc_BBCH_rip
        '12 25 33 BBCH = 7 E(-12) NHHsum4 – 2 E(-8) NHHsum3 – 2 E(-6) NHHsum2 +0.0488 NHHsum + 42.642
        Return ((((0.000000000007D * nhhsum) - 0.00000002D) * nhhsum - 0.000002D) * nhhsum + 0.0488D) * nhhsum + 42.642D
    End Function

    Public Function Calc_BBCH_veg(nhhsum As Decimal) As Decimal Implements IAgronomica30_BBCH_Fun.Calc_BBCH_veg
        '12 25 33 BBCH = 12.667 Ln(NHHsum) - 53.051 (???)
        Return 12.667D * Math.Log(nhhsum) - 53.061D
    End Function
End Class



Public Class Agronomica30_BBCH

    'Ore Normali di Caldo (NHH)
    'normalizza le temperature per mezzo della curva di risposta (Wang & Engel, 1998, Weikai & Hunt, 1999)
    'rappresenta l’efficacia della temperatura per la fenologia delle piante

    Public Enum VarietaVite
        Chardonnay = 1
        CabernetSauvignon = 2
    End Enum

    Private ReadOnly _Tcmin As Decimal 'Temperatura cardinale minima – limite inferiore sotto al quale le temperature non sono efficaci
    Private ReadOnly _Tcmax As Decimal 'Temperatura cardinale massima – limite superiore oltre al quale le temperature non sono efficaci
    Private ReadOnly _Topt As Decimal 'Temperatura ottimale per il processo
    Private ReadOnly _alpha As Decimal
    Private ReadOnly _Functor As IAgronomica30_BBCH_Fun
    Private _NHHsum As Decimal
    Private _BBCH_rip As Decimal
    Private _BBCH_veg As Decimal

    Public ReadOnly Property NHHsum As Decimal
        Get
            Return _NHHsum
        End Get
    End Property
    Public ReadOnly Property BBCH_rip As Decimal
        Get
            Return _BBCH_rip
        End Get
    End Property
    Public ReadOnly Property BBCH_veg As Decimal
        Get
            Return _BBCH_veg
        End Get
    End Property

    Public Sub New(tcmin As Decimal, tcmax As Decimal, topt As Decimal, var As VarietaVite)

        _Tcmin = tcmin
        _Tcmax = tcmax
        _Topt = topt
        _alpha = Math.Log(2D / Math.Log((_Tcmax - _Tcmin) / (_Topt - _Tcmin)))

        _NHHsum = 0
        _BBCH_rip = 0
        _BBCH_veg = 0

        Select Case var

            Case VarietaVite.Chardonnay
                _Functor = New Agronomica30_BBCH_Chardonnay

            Case VarietaVite.CabernetSauvignon
                _Functor = New Agronomica30_BBCH_CabernetSauvignon

            Case Else
                Throw New Exception("Calcolo BBCH: parametro non corretto (varietà vite)")

        End Select
    End Sub

    Public Sub Calc(temp As Decimal)

        If temp < _Tcmin OrElse _Tcmax < temp Then

            'NHH non calcolato -> _NHHSum non cambia... Posso mantenere i valori calcolati precedentemente

            Return
        End If

        Dim NHH As Decimal = (2D * Math.Pow((temp - _Tcmin), _alpha) * Math.Pow((_Topt - _Tcmin), _alpha) - Math.Pow((temp - _Tcmin), (2D * _alpha))) / Math.Pow((_Topt - _Tcmin), (2D * _alpha))

        If NHH <= 0 Then

            '_NHHSum non cambia... Posso mantenere i valori calcolati precedentemente

            Return
        End If

        _NHHsum += NHH
        _BBCH_rip = Math.Round(_Functor.Calc_BBCH_rip(_NHHsum))
        _BBCH_veg = Math.Max(0D, _Functor.Calc_BBCH_veg(_NHHsum))
    End Sub
End Class

