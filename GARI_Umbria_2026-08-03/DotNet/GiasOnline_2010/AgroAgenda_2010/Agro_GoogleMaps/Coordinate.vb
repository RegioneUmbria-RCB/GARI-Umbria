
Public Class Coordinate


    Const a_int1924 As Double = 6378388.0
    Const a_WGS84 As Double = 6378137.0
    Const a_clarke1866 = 6378206.4
    Const f_int1924 As Double = 1 / 297
    Const ft_int1924 As Double = 297
    Const f_WGS84 As Double = 1 / 298.257
    Const ft_WGS84 As Double = 298.257
    Const f_clarke1866 As Double = 1 / 294.98
    Const ft_clarke1866 As Double = 294.98
    Const UTM32 As Integer = 32
    Const UTM33 As Integer = 33
    Const lat0_UTM32 As Double = 0.0
    Const lat0_UTM33 As Double = 0.0
    Const lon0_UTM32 As Double = 9.0
    Const lon0_UTM33 As Double = 15.0

    Private a As Double = a_int1924
    Private f As Double = f_int1924
    Private lat0 As Double = lat0_UTM32
    Private lon0 As Double
    Private mFuso As Integer
    Private mdeltaE As Double
    Private mdeltaN As Double

    Public Sub New(ByVal Fuso As Integer, ByVal deltaE As Double, ByVal deltaN As Double)

        a = a_int1924
        f = f_int1924

        Me.Fuso = Fuso
        Me.deltaE = deltaE
        Me.deltaN = deltaN

    End Sub

    Public Property Fuso() As Integer
        Get
            Return mFuso
        End Get
        Set(ByVal Value As Integer)
            mFuso = Value
        End Set
    End Property

    Public Property deltaE() As Double
        Get
            Return mdeltaE
        End Get
        Set(ByVal Value As Double)
            mdeltaE = Value
        End Set
    End Property

    Public Property deltaN() As Double
        Get
            Return mdeltaN
        End Get
        Set(ByVal Value As Double)
            mdeltaN = Value
        End Set
    End Property


    '############################################################################################################
    Public Function gradmin2graddec(ByVal lat As String) As Double 'Trasforma in gradi sessadecimali
        Dim gradec As String = "ERR"
        Dim pvirg As Integer
        Dim sdec As String
        'Dim dec As Integer
        Dim dec As Double

        Dim sgrad As String
        Dim grad As Double

        pvirg = lat.IndexOf(",")

        If pvirg <> -1 Then

            'lat = lat.Replace(".", ",")
            'sdec = lat.Substring(pvirg - 2, lat.Length - pvirg + 2)

            'dec = CDbl(sdec) / 60
            ' ''sdec1 = lat.Substring(pvirg + 1, lat.Length - pvirg - 1)
            ''If lat.Length - pvirg = 4 Then
            ''    dec = CDbl(sdec) / 60000
            ''Else : dec = CDbl(sdec) / 600000
            ''End If
            ' ''dec = dec * 100 / 60
            'sgrad = lat.Substring(0, pvirg - 1)
            'grad = CDbl(sgrad) + dec
            'Return grad

            lat = lat.Replace(".", ",")
            sdec = "0," + lat.Substring(pvirg + 1, lat.Length - pvirg - 1)

            dec = CDbl(sdec) / 60
            ''sdec1 = lat.Substring(pvirg + 1, lat.Length - pvirg - 1)
            'If lat.Length - pvirg = 4 Then
            '    dec = CDbl(sdec) / 60000
            'Else : dec = CDbl(sdec) / 600000
            'End If
            ''dec = dec * 100 / 60
            sgrad = lat.Substring(0, pvirg)
            grad = CDbl(sgrad) + dec
            Return grad


        End If


    End Function

    '############################################################################################################
    Public Function ProiezioneUTM(ByVal lat As Double, ByVal lon As Double, ByVal lat0 As Double, ByVal lon0 As Double, ByVal a As Double, ByVal f As Double) As Double()

        'Const a = 6378388
        'Const a = 6378206.4

        'Const e2 = 0.00676866
        Const k0 As Double = 0.9996
        'Const f = 1 / 297
        'Const f = 1 / 294.98
        'Const e2 = 2 * f - (f ^ 2)
        Dim e2 As Double = f * (2 - f)
        Dim e12 As Double = e2 / (1 - e2)
        'Const lat0 = 0
        'Const lon0 = 9
        'Const lon0 = 75
        Dim lon0r As Double = lon0 / 180 * Math.PI
        Dim lat0r As Double = lat0 / 180 * Math.PI

        Dim latr As Double = lat / 180 * Math.PI
        Dim lonr As Double = lon / 180 * Math.PI


        Dim ris(2) As Double

        'Dim AA As Double = Math.Cos(latr) * (-lon - (-lon0)) * (Math.PI) / 180
        Dim AA As Double
        Dim C As Double
        Dim T As Double
        Dim N As Double
        Dim x As Double
        Dim y As Double
        Dim M As Double
        Dim M0 As Double

        Dim mint As Double

        AA = Math.Cos(latr) * (lonr - lon0r) '* (Math.PI) / 180
        'AA = Math.Cos(latr) * (-lonr + lon0r) '* (Math.PI) / 180
        C = e12 * (Math.Cos(latr)) ^ 2
        T = (Math.Tan(latr)) ^ 2
        N = a / ((1 - e2 * (Math.Sin(latr) ^ 2)) ^ (0.5))

        'M = 111132.0894 * (lat) - 16216.94 * Math.Sin(2 * latr) + 17.21 * Math.Sin(4 * latr) - 0.02 * Math.Sin(6 * latr)
        M0 = 111132.0894 * (lat0) - 16216.94 * Math.Sin(2 * lat0r) + 17.21 * Math.Sin(4 * lat0r) - 0.02 * Math.Sin(6 * lat0r)

        mint = latr * (1 - e2 / 4 - 3 * e2 * e2 / 64 - 5 * e2 * e2 * e2 / 256)
        mint = mint - Math.Sin(2 * latr) * (3 * e2 / 8 + 3 * e2 * e2 / 32 + 45 * e2 * e2 * e2 / 1024)
        mint = mint + Math.Sin(4 * latr) * e2 * e2 * (15 / 256 + 45 * e2 / 1024)
        M = 1 * a * (mint - Math.Sin(6 * latr) * 35 * e2 * e2 * e2 / 3072)


        x = k0 * N * (AA + (1 - T + C) * ((AA ^ 3) / 6) + (5 - 18 * T + T ^ 2 + 72 * C - 58 * e12) * ((AA ^ 5) / 120))

        y = k0 * (M - M0 + N * Math.Tan(latr) * ((AA ^ 2) / 2 + (5 - T + 9 * C + 4 * (C ^ 2)) * (AA ^ 4) / 24 + (61 - 58 * T + T ^ 2 + 600 * C - 330 * e12) * (AA ^ 6) / 720))

        ris(0) = x
        ris(1) = y

        Return ris

    End Function


    Public Sub CorreggiUTM(ByVal X As Double, ByVal Y As Double, ByRef X2 As Double, ByRef Y2 As Double, ByVal deltaE As Double, ByVal deltaN As Double, Optional ByVal FalsoEst As Double = 0.0)
        X2 = X + FalsoEst + deltaE  'Latitudine(coordinata Est UTM)
        Y2 = Y + deltaN
    End Sub

    Public Sub FromGradiToUTM(ByVal latitudine As Double, ByVal longitudine As Double, ByRef X As Double, ByRef Y As Double)

        Dim la As Double
        Dim lo As Double
        Dim latlon(1) As Double


        Try

            If Fuso = UTM32 Then
                lat0 = lat0_UTM32
                lon0 = lon0_UTM32
            ElseIf Fuso = UTM33 Then
                lat0 = lat0_UTM33
                lon0 = lon0_UTM33
            End If

            'la = gradmin2graddec(latitudine)
            'lo = gradmin2graddec(longitudine)

            la = latitudine
            lo = longitudine

            'Proiezione da coordinate geometriche a coordinate piane
            latlon = ProiezioneUTM(la, lo, lat0, lon0, a, f)

            CorreggiUTM(latlon(0), latlon(1), X, Y, deltaE, deltaN, 500000.0)
            'x = latlon(0) + 500000 + CDbl(ObjGiasIni.GPSConvParamDeltaE)  'Latitudine(coordinata Est UTM)
            'y = latlon(1) + CDbl(ObjGiasIni.GPSConvParamDeltaN)

        Catch ex As Exception

            X = -1.0
            Y = -1.0

        End Try



    End Sub
End Class

