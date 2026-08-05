
Imports System.Globalization
Imports System.Runtime.Serialization
Imports AgronicaCoreDataProvider.My.Resources

Namespace Agronomica30_PacchettoGrano

    <DataContract>
    Public Class Concimazione

        Public data As Date
        <DataMember(EmitDefaultValue:=False, Name:="data")>
        Public Property dataStr As String
            Get
                Return data.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
            End Get
            Set(value As String)
                data = Date.ParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture)
            End Set
        End Property

        <DataMember>
        Public qta_qha As Decimal 'Quantità (Quintali/ettaro) prodotto

        <DataMember>
        Public titoloN As Decimal 'Titolo azoto

        <DataMember>
        Public fracVol As Decimal 'Fraction volatilization

#If False Then

    NFERTI(j) = Product_amount * titolo_N / 10;
    VOLFI(j) = frac_vol;    %Fraction volatilization

    Se non indicato frac_vol = 1; 

    'UREA'
            titolo_N = 46;
            frac_vol = 15;

    'NITRATO AMMONICO'
            titolo_N = 27;
            frac_vol = 4;

    'NUTRIGAN TOP S'
            titolo_N = 10;

    'MICROLAN Zn'
            titolo_N = 5.5;

    'EMERN 35'
            titolo_N = 35;
            frac_vol = 8;

    'EMERTOP 30'
            titolo_N = 30;

    'SULFAN 24'
            titolo_N = 24;

    'NSZ 26'
            titolo_N = 26;

    'DECAFOL'
            titolo_N = 9;

    'KAPPA M'
            titolo_N = 21;
#End If

    End Class


    <DataContract>
    Public Class InputParams

        ''' <summary>
        ''' Latitudine in gradi decimali
        ''' </summary>
        <DataMember>
        Public latitude As Decimal

        ''' <summary>
        ''' 1 -> Grano Tenero 
        ''' 2 -> Grano Duro
        ''' </summary>
        <DataMember>
        Public varieta As Integer

        Public dataSemina As Date
        <DataMember(EmitDefaultValue:=False, Name:="dataSemina")>
        Public Property dataSeminaStr As String
            Get
                Return dataSemina.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
            End Get
            Set(value As String)
                dataSemina = Date.ParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture)
            End Set
        End Property

        ''' <summary>
        ''' 1 -> Sabbioso
        ''' 2 -> Franco sabbioso
        ''' 3 -> Sabbioso franco
        ''' 4 -> Franco
        ''' 5 -> Franco limoso argilloso
        ''' 6 -> Franco argilloso
        ''' 7 -> Franco limoso
        ''' 8 -> Argilloso sabbioso
        ''' 9 -> Franco sabbioso argilloso
        ''' 10 -> Argilloso limoso
        ''' 11 -> Argilloso
        ''' </summary>
        <DataMember>
        Public tipoSuolo As Integer 'DA VERIFICARE: Discrepanza elenco/indice per 2 e 3 in file Input_data.m

        <DataMember>
        Public concimazioni As List(Of Concimazione)

        ''' <summary>
        ''' 1 -> Altamente tollerante
        ''' 2 -> Tollerante
        ''' 3 -> Suscettibile
        ''' 4 -> Altamente suscettibile
        ''' </summary>
        <DataMember>
        Public cultivar As Integer

        ''' <summary>
        ''' 1 -> Colture annuali (pomodori, barbabietole, ecc..)
        ''' 2 -> Frumento - orzo
        ''' 3 -> Mais - sorgo
        ''' </summary>
        <DataMember>
        Public colturaPrec As Integer

        ''' <summary>
        ''' 1 -> Lavorazione convenzionale
        ''' 2 -> Minima lavorazione
        ''' 3 -> Nessuna lavorazione
        ''' </summary>
        <DataMember>
        Public lavTerreno As Integer

        'Public Sub New()
        '    concimazioni = New List(Of Concimazione)
        'End Sub
    End Class


    Public Class soil

        Public ReadOnly BDL(4) As Decimal       'Soil bulk density (g.cm-3)
        Public ReadOnly DLYER(4) As Decimal     'mm
        Public ReadOnly DRAINF(4) As Decimal
        Public ReadOnly DUL(4) As Decimal       'mm/mm
        Public ReadOnly EXTR(4) As Decimal      'mm/mm
        Public ReadOnly FG(4) As Decimal        'Fraction soil >2mm
        Public ReadOnly FMIN(4) As Decimal      'Fraction Org N avail. for mineralization
        Public ReadOnly MAI(4) As Decimal       '0=cll, 1=dul
        Public ReadOnly NH4(4) As Decimal       'ppm = mg.kg-1
        Public ReadOnly NLYER As Decimal
        Public ReadOnly NO3(4) As Decimal       'ppm = mg.kg-1
        Public ReadOnly NORG(4) As Decimal      'Organic N (%)
        Public ReadOnly SAT(4) As Decimal       'mm/mm

        Public Class _ferti
            Public data As Date
            Public nferti As Decimal    '1 fertilizer amount at the top-dressing (gN.m-2)
            Public volfi As Decimal     'Fraction volatilization
        End Class

        Private FERTI As List(Of _ferti)

        Public ReadOnly Property FN As Integer
            Get
                Return FERTI.Count
            End Get
        End Property

        Public Function getFerti(i As Integer) As _ferti
            If i < 0 Then
                Return Nothing
            End If
            If i >= FERTI.Count Then
                Return Nothing
            End If
            Return FERTI(i)
        End Function

        Public Sub New(tipoSuolo As Integer, clist As List(Of Concimazione))

            Select Case tipoSuolo

                Case 1  'Sand

                    BDL = {1.6, 1.6, 1.6, 1.6, 1.6}
                    DLYER = {150, 150, 300, 300, 300}
                    DRAINF = {0.5, 0.5, 0.5, 0.5, 0.5}
                    DUL = {0.1, 0.1, 0.1, 0.1, 0.1}
                    EXTR = {0.51, 0.051, 0.051, 0.051, 0.051}
                    FG = {0, 0, 0, 0, 0}
                    FMIN = {0.1, 0.1, 0.1, 0.1, 0.1}
                    MAI = {0.9, 0.9, 0.9, 0.9, 0.9}
                    NH4 = {2.2, 1.8, 1.6, 1.3, 1.1}
                    NLYER = 5
                    NO3 = {11, 8, 6, 5, 4}
                    NORG = {0.95, 0.095, 0.095, 0.05, 0.02}
                    SAT = {0.3, 0.297, 0.294, 0.2911, 0.2882}

                Case 2  'LoamySand

                    BDL = {1.5, 1.5, 1.5, 1.5, 1.5}
                    DLYER = {150, 150, 300, 300, 300}
                    DRAINF = {0.5, 0.5, 0.5, 0.5, 0.5}
                    DUL = {0.16, 0.1613, 0.1626, 0.1639, 0.1652}
                    EXTR = {0.816, 0.0823, 0.0829, 0.0836, 0.0842}
                    FG = {0, 0, 0, 0, 0}
                    FMIN = {0.1, 0.1, 0.1, 0.1, 0.1}
                    MAI = {0.9, 0.9, 0.9, 0.9, 0.9}
                    NH4 = {2.2, 1.8, 1.6, 1.3, 1.1}
                    NLYER = 5
                    NO3 = {11, 8, 6, 5, 4}
                    NORG = {0.95, 0.095, 0.095, 0.05, 0.03}
                    SAT = {0.35, 0.3518, 0.3535, 0.3553, 0.3571}

                Case 3  'SandyLoam

                    BDL = {1.5, 1.5, 1.5, 1.5, 1.5}
                    DLYER = {150, 150, 300, 300, 300}
                    DRAINF = {0.45, 0.45, 0.45, 0.45, 0.45}
                    DUL = {0.21, 0.2117, 0.2134, 0.2151, 0.2168}
                    EXTR = {0.1071, 0.108, 0.1088, 0.1097, 0.1106}
                    FG = {0, 0, 0, 0, 0}
                    FMIN = {0.1, 0.1, 0.1, 0.1, 0.1}
                    MAI = {0.9, 0.9, 0.9, 0.9, 0.9}
                    NH4 = {2.2, 1.8, 1.6, 1.3, 1.1}
                    NLYER = 5
                    NO3 = {11, 8, 6, 5, 4}
                    NORG = {0.95, 0.095, 0.095, 0.05, 0.05}
                    SAT = {0.42, 0.4221, 0.4242, 0.4263, 0.4285}

                Case 4  'Loam

                    BDL = {1.4, 1.4, 1.4, 1.4, 1.4}
                    DLYER = {150, 150, 300, 300, 300}
                    DRAINF = {0.45, 0.45, 0.45, 0.45, 0.45}
                    DUL = {0.27, 0.2722, 0.2743, 0.2765, 0.2787}
                    EXTR = {0.1377, 0.1388, 0.1399, 0.141, 0.1422}
                    FG = {0, 0, 0, 0, 0}
                    FMIN = {0.1, 0.1, 0.1, 0.1, 0.1}
                    MAI = {0.9, 0.9, 0.9, 0.9, 0.9}
                    NH4 = {2.2, 1.8, 1.6, 1.3, 1.1}
                    NLYER = 5
                    NO3 = {11, 8, 6, 5, 4}
                    NORG = {0.95, 0.095, 0.095, 0.07, 0.05}
                    SAT = {0.46, 0.4623, 0.4646, 0.4669, 0.4693}

                Case 5  'SiltyClayLoam

                    BDL = {1.4, 1.4, 1.4, 1.4, 1.4}
                    DLYER = {150, 150, 300, 300, 300}
                    DRAINF = {0.45, 0.45, 0.45, 0.45, 0.45}
                    DUL = {0.28, 0.2822, 0.2845, 0.2868, 0.2891}
                    EXTR = {0.1428, 0.1439, 0.1451, 0.1463, 0.1474}
                    FG = {0, 0, 0, 0, 0}
                    FMIN = {0.1, 0.1, 0.1, 0.1, 0.1}
                    MAI = {0.9, 0.9, 0.9, 0.9, 0.9}
                    NH4 = {2.2, 1.8, 1.6, 1.3, 1.1}
                    NLYER = 5
                    NO3 = {11, 8, 6, 5, 4}
                    NORG = {0.95, 0.095, 0.095, 0.08, 0.05}
                    SAT = {0.52, 0.5226, 0.5252, 0.5278, 0.5305}

                Case 6  'ClayLoam

                    BDL = {1.35, 1.35, 1.35, 1.35, 1.35}
                    DLYER = {150, 150, 300, 300, 300}
                    DRAINF = {0.45, 0.45, 0.45, 0.45, 0.45}
                    DUL = {0.29, 0.2923, 0.2947, 0.297, 0.2994}
                    EXTR = {0.1479, 0.1491, 0.1503, 0.1515, 0.1527}
                    FG = {0, 0, 0, 0, 0}
                    FMIN = {0.1, 0.1, 0.1, 0.1, 0.1}
                    MAI = {0.9, 0.9, 0.9, 0.9, 0.9}
                    NH4 = {2.2, 1.8, 1.6, 1.3, 0}
                    NLYER = 5
                    NO3 = {11, 8, 6, 5, 0}
                    NORG = {0.11, 0.11, 0.11, 0.1, 0.08}
                    SAT = {0.5331, 0.5358, 0.5385, 0.5412, 0.5439}

                Case 7  'SiltLoam

                    BDL = {1.32, 1.32, 1.32, 1.32, 1.32}
                    DLYER = {150, 150, 300, 300, 300}
                    DRAINF = {0.45, 0.45, 0.45, 0.45, 0.45}
                    DUL = {0.3, 0.3024, 0.3048, 0.3073, 0.3097}
                    EXTR = {0.153, 0.1542, 0.1555, 0.1567, 0.158}
                    FG = {0, 0, 0, 0, 0}
                    FMIN = {0.1, 0.1, 0.1, 0.1, 0.1}
                    MAI = {0.9, 0.9, 0.9, 0.9, 0.9}
                    NH4 = {2.2, 1.8, 1.6, 1.3, 1.1}
                    NLYER = 5
                    NO3 = {11, 8, 6, 5, 4}
                    NORG = {0.11, 0.11, 0.11, 0.1, 0.09}
                    SAT = {0.47, 0.4723, 0.4747, 0.4771, 0.4795}

                Case 8  'SandyClay

                    BDL = {1.3, 1.3, 1.3, 1.3, 1.3}
                    DLYER = {150, 150, 300, 300, 300}
                    DRAINF = {0.45, 0.45, 0.45, 0.45, 0.45}
                    DUL = {0.32, 0.3226, 0.3251, 0.3277, 0.3304}
                    EXTR = {0.1632, 0.1645, 0.1658, 0.1671, 0.1685}
                    FG = {0, 0, 0, 0, 0}
                    FMIN = {0.1, 0.1, 0.1, 0.1, 0.1}
                    MAI = {0.9, 0.9, 0.9, 0.9, 0.9}
                    NH4 = {2.2, 1.8, 1.6, 1.3, 1.1}
                    NLYER = 5
                    NO3 = {11, 8, 6, 5, 4}
                    NORG = {0.11, 0.11, 0.11, 0.1, 0.09}
                    SAT = {0.5, 0.5025, 0.505, 0.5075, 0.5101}

                Case 9  'SandyClayLoam

                    BDL = {1.2, 1.2, 1.2, 1.2, 1.2}
                    DLYER = {150, 150, 300, 300, 300}
                    DRAINF = {0.4, 0.4, 0.4, 0.4, 0.4}
                    DUL = {0.36, 0.3629, 0.3658, 0.3687, 0.3717}
                    EXTR = {0.1836, 0.1851, 0.1865, 0.188, 0.1895}
                    FG = {0, 0, 0, 0, 0}
                    FMIN = {0.1, 0.1, 0.1, 0.1, 0.1}
                    MAI = {0.9, 0.9, 0.9, 0.9, 0.9}
                    NH4 = {2.2, 1.8, 1.6, 1.3, 1.1}
                    NLYER = 5
                    NO3 = {11, 8, 6, 5, 4}
                    NORG = {0.11, 0.11, 0.11, 0.1, 0.09}
                    SAT = {0.52, 0.5226, 0.5252, 0.5278, 0.5305}

                Case 10 'SiltyClay

                    BDL = {1.2, 1.2, 1.2, 1.2, 1.2}
                    DLYER = {150, 150, 300, 300, 300}
                    DRAINF = {0.4, 0.4, 0.4, 0.4, 0.4}
                    DUL = {0.4, 0.4032, 0.4064, 0.4097, 0.413}
                    EXTR = {0.204, 0.2056, 0.2073, 0.2089, 0.2106}
                    FG = {0, 0, 0, 0, 0}
                    FMIN = {0.1, 0.1, 0.1, 0.1, 0.1}
                    MAI = {0.9, 0.9, 0.9, 0.9, 0.9}
                    NH4 = {2.2, 1.8, 1.6, 1.3, 1.1}
                    NLYER = 5
                    NO3 = {11, 8, 6, 5, 4}
                    NORG = {0.11, 0.11, 0.11, 0.1, 0.1}
                    SAT = {0.55, 0.5527, 0.5555, 0.5583, 0.5611}

                Case 11 'Clay

                    BDL = {1.1, 1.1, 1.1, 1.1, 1.1}
                    DLYER = {150, 150, 300, 300, 300}
                    DRAINF = {0.4, 0.4, 0.4, 0.4, 0.4}
                    DUL = {0.4, 0.4032, 0.4064, 0.4097, 0.413}
                    EXTR = {0.204, 0.2056, 0.2073, 0.2089, 0.2106}
                    FG = {0, 0, 0, 0, 0}
                    FMIN = {0.1, 0.1, 0.1, 0.1, 0.1}
                    MAI = {0.9, 0.9, 0.9, 0.9, 0.9}
                    NH4 = {2.2, 1.8, 1.6, 1.3, 1.1}
                    NLYER = 5
                    NO3 = {11, 8, 6, 5, 4}
                    NORG = {0.11, 0.11, 0.11, 0.09, 0.09}
                    SAT = {0.55, 0.5527, 0.5555, 0.5583, 0.5611}

                Case Else
                    Throw New NotImplementedException("Parametro tipoSuolo non valido ")
            End Select

            FERTI = New List(Of _ferti)

            For Each c In clist

                'DAPNF_day = x{1, j}{1, 1};   %Time of 1 application as top-dressing
                'DAPNF_month = x{1, j}{2, 1};   %Time of 1 application as top-dressing
                'DAPNF_year = x{1, j}{3, 1};   %Time of 1 application as top-dressing
                'DAPNF_date = strcat(DAPNF_month,'/',DAPNF_day,'/',DAPNF_year);
                'vv = datevec(DAPNF_date);
                'vv0 = vv;
                'vv0(:,2:3) = 1;
                'DAPNF(j) = 365 - Pdoy + datenum(vv) - datenum(vv0) + 1;

                FERTI.Add(New _ferti With {
                          .data = c.data,
                          .nferti = c.qta_qha * c.titoloN / 10.0,
                          .volfi = c.fracVol
                          })
            Next
        End Sub
    End Class


    Public Class crop

        Public ReadOnly varieta As Integer

        Public ReadOnly bdANTPM As Decimal
        Public ReadOnly bdBOTEAR As Decimal
        Public ReadOnly bdEARANT As Decimal
        Public ReadOnly bdEMRTIL As Decimal
        Public ReadOnly bdPMHM As Decimal
        Public ReadOnly bdSELBOT As Decimal
        Public ReadOnly bdSOWEMR As Decimal
        Public ReadOnly bdTILSEL As Decimal
        Public ReadOnly cpp As Decimal
        Public ReadOnly FLDKL As Decimal
        Public ReadOnly FLF1A As Decimal
        Public ReadOnly FLF1B As Decimal
        Public ReadOnly FLF2 As Decimal
        Public ReadOnly FRTRL As Decimal
        Public ReadOnly FRZLDR As Decimal
        Public ReadOnly GCC As Decimal
        Public ReadOnly GNC As Decimal
        Public ReadOnly GRTDP As Decimal
        Public ReadOnly iDEPORT As Decimal
        Public ReadOnly IRUE As Decimal
        Public ReadOnly KPAR As Decimal
        Public ReadOnly MEED As Decimal
        Public ReadOnly MXNUP As Decimal
        Public ReadOnly phyl As Decimal
        Public ReadOnly ppsen As Decimal
        Public ReadOnly PDHI As Decimal
        Public ReadOnly PLACON As Decimal
        Public ReadOnly PLAPOW As Decimal
        Public ReadOnly SLA As Decimal
        Public ReadOnly SLNG As Decimal
        Public ReadOnly SLNS As Decimal
        Public ReadOnly SNCG As Decimal
        Public ReadOnly SNCS As Decimal
        Public ReadOnly TBD As Decimal
        Public ReadOnly TBRUE As Decimal
        Public ReadOnly TBVER As Decimal
        Public ReadOnly TCD As Decimal
        Public ReadOnly TCRUE As Decimal
        Public ReadOnly TCVER As Decimal
        Public ReadOnly TEC As Decimal
        Public ReadOnly TKILL As Decimal
        Public ReadOnly TP1D As Decimal
        Public ReadOnly TP1RUE As Decimal
        Public ReadOnly TP1VER As Decimal
        Public ReadOnly TP2RUE As Decimal
        Public ReadOnly TP2VER As Decimal
        Public ReadOnly vsen As Decimal
        Public ReadOnly VDSAT As Decimal
        Public ReadOnly WDHI1 As Decimal
        Public ReadOnly WDHI2 As Decimal
        Public ReadOnly WDHI3 As Decimal
        Public ReadOnly WDHI4 As Decimal
        Public ReadOnly WSSD As Decimal
        Public ReadOnly WSSG As Decimal
        Public ReadOnly WSSL As Decimal
        Public ReadOnly WTOPL As Decimal

        Public ReadOnly PDEN As Decimal             'Densità di semina
        Public ReadOnly WGRN_media As Decimal       'Produzione standard

        Public ReadOnly Property bdEMR As Decimal
            Get
                Return bdSOWEMR
            End Get
        End Property

        Public ReadOnly Property bdTIL As Decimal
            Get
                Return bdEMR + bdEMRTIL + 1
            End Get
        End Property

        Public ReadOnly Property bdSEL As Decimal
            Get
                Return bdTIL + bdTILSEL - 3 'modificato il 13/08/2020
            End Get
        End Property

        Public ReadOnly Property bdBOT As Decimal
            Get

                Dim value As Decimal = bdSEL + bdSELBOT + 0.5

                If varieta = 1 Then

                    value += 1
                End If

                Return value
            End Get
        End Property

        Public ReadOnly Property bdEAR As Decimal
            Get
                Dim value As Decimal = bdBOT + bdBOTEAR - 1 ' + 5 modificato il 25/02/2020

                If varieta = 1 Then

                    value += 0.5
                End If

                Return value
            End Get
        End Property

        Public ReadOnly Property bdANT As Decimal
            Get
                Return bdEAR + bdEARANT - 3 ' - 4 modificato il 25/02/2020
            End Get
        End Property

        Public ReadOnly Property bdBSG As Decimal
            Get
                Return bdANT + 6.5  'BSG Or beginning linear HI 5 days after anthesis
            End Get
        End Property

        Public ReadOnly Property bdTSG As Decimal
            Get
                Return bdPM - 1.5   'TSG or termination of linear HI 1.5 days before physiological maturity
            End Get
        End Property

        Public ReadOnly Property bdPM As Decimal
            Get
                Return bdANT + bdANTPM + 1
            End Get
        End Property

        Public ReadOnly Property bdMAT As Decimal
            Get
                Return bdPM + bdPMHM - 4    ' modificato il 21/09/2020 - 3
            End Get
        End Property

        Public ReadOnly Property bdTLM As Decimal    'Leaf production on mainstem terminates at booting
            Get
                Return bdBOT
            End Get
        End Property

        Public Sub New(varieta As Integer)

            Me.varieta = varieta

            bdANTPM = 35
            bdBOTEAR = 3
            bdEARANT = 7
            bdEMRTIL = 5.5
            bdPMHM = 8
            bdSELBOT = 8
            bdSOWEMR = 4
            bdTILSEL = 10.05
            cpp = 21
            FLDKL = 20
            FLF1A = 0.6
            FLF1B = 0.3
            FLF2 = 0.1
            If varieta = 1 Then
                FRTRL = 0.4
            Else
                FRTRL = 0.22
            End If
            FRZLDR = 0.01
            GCC = 1
            GNC = 0.0213
            GRTDP = 30
            iDEPORT = 200
            IRUE = 2.2
            KPAR = 0.65
            MEED = 1000.0
            MXNUP = 0.25
            phyl = 95
            ppsen = 0.0026
            PDHI = 0.014
            PLACON = 1
            PLAPOW = 2.34
            SLA = 0.021
            SLNG = 1.5
            SLNS = 0.4
            SNCG = 0.015
            SNCS = 0.005
            TBD = 0
            TBRUE = 0
            TBVER = -1
            TCD = 40
            TCRUE = 35
            TCVER = 12
            TEC = 5.8
            TKILL = -5
            TP1D = 27.5
            TP1RUE = 15
            TP1VER = 0
            TP2RUE = 22
            TP2VER = 8
            vsen = 0
            VDSAT = 50
            WDHI1 = 0
            WDHI2 = 600
            WDHI3 = 1200.0
            WDHI4 = 3200.0
            WSSD = 0.4
            WSSG = 0.3
            WSSL = 0.4
            WTOPL = 160

            If varieta = 1 Then

                'Grano Tenero

                PDEN = 450
                WGRN_media = 700

            Else

                'Grano Duro

                PDEN = 400
                WGRN_media = 600

            End If

        End Sub

        Public Function RUE(Temp As Decimal) As Decimal

            'Adjustment of RUE (efficienza nell’uso della radiazione - Radiation Use Efficiency)

            Dim TCFRUE As Decimal = 0

            If Temp <= TBRUE OrElse Temp >= TCRUE Then

                TCFRUE = 0

            ElseIf TBRUE < Temp AndAlso Temp < TP1RUE Then

                TCFRUE = (Temp - TBRUE) / (TP1RUE - TBRUE)

            ElseIf TP2RUE < Temp AndAlso Temp < TCRUE Then

                TCFRUE = (TCRUE - Temp) / (TCRUE - TP2RUE)

            ElseIf TP1RUE <= Temp AndAlso Temp <= TP2RUE Then

                TCFRUE = 1

            End If

            Return IRUE * TCFRUE
        End Function
    End Class


    Public Class datoGG
        Public data As Date
        Public tmax As Decimal
        Public tmin As Decimal
        Public rain As Decimal
        Public lw As Decimal
        Public rh As Decimal
        Public srad As Decimal
        Public tmp As Decimal

        ''' <summary>
        ''' Main Stem Node Number
        ''' </summary>
        Public msnn As Decimal
        Public msnn_real As Decimal
        ''' <summary>
        ''' Biological day
        ''' </summary>
        Public bd As Decimal
        ''' <summary>
        ''' Cumulative biological day
        ''' </summary>
        Public cbd As Decimal
        ''' <summary>
        ''' Vernalization day
        ''' </summary>
        Public verday As Decimal
        ''' <summary>
        ''' Daily Temperature Unit
        ''' </summary>
        Public dtu As Decimal
        ''' <summary>
        ''' photoperiod
        ''' </summary>
        Public pp As Decimal

        Public lai As Decimal
        Public glai As Decimal
        Public dlai As Decimal
        ''' <summary>
        ''' lai at bsg
        ''' </summary>
        Public bsglai As Decimal

        ''' <summary>
        ''' Daily dry matter production
        ''' </summary>
        Public ddmp As Decimal

        Public Sub New(dm As MeteoDSSItem)

            data = dm.DataOra.Date
            tmax = dm.Temp
            tmin = dm.Temp
            rain = dm.Prec
            lw = dm.Bagn
            rh = dm.UmRel

            msnn = 1
            msnn_real = 1
            bd = 0
            cbd = 0

            lai = 0
            glai = 0
            dlai = 0
            bsglai = 0

        End Sub

        Public Sub addHour(dm As MeteoDSSItem)

            tmax = Math.Max(tmax, dm.Temp)
            tmin = Math.Min(tmin, dm.Temp)
            rain += dm.Prec
            lw += dm.Bagn
            rh += dm.UmRel

        End Sub

        Public Sub makeDay(ore As Integer, crop_data As crop, radiansLat As Decimal)

            rh /= ore
            tmp = (tmax - tmin) / 2

            'SRAD = compute_SRAD(LAT, DOY, TMAX, TMIN, str2num(data_semina_year), year_fine_stagione, mod(str2num(data_semina_year), 4) == 0, mod(year_fine_stagione, 4) == 0); % mod(2020, 4) == 0 bisestile
            'function Rs = compute_SRAD(LAT, DOY, TMAX, TMIN, anno1, anno2, anno1_bis_non_bis, anno2_bis_non_bis)

            Dim gsc As Decimal = 0.082
            Dim Kt As Decimal = 0.16

            Dim doy As Decimal = data.DayOfYear
            Dim alfa As Decimal = 2D * Math.PI * doy / 365D

            Dim ndec As Decimal = 0.409D * Math.Sin(alfa - 1.39D)
            Dim nws As Decimal = Math.Acos(-Math.Tan(radiansLat) * Math.Tan(ndec))
            Dim dfr As Decimal = 1 + 0.033 * Math.Cos(alfa)
            Dim Ra As Decimal = 24 * 60 / Math.PI * gsc * dfr * (nws * Math.Sin(radiansLat) * Math.Sin(ndec) + Math.Cos(radiansLat) * Math.Cos(ndec) * Math.Sin(nws))

            srad = Kt * Ra * tmp ^ 0.5

            Dim snow As Decimal = 0

            'Snow Cover and Melt
            If tmax <= 1 Then

                snow += rain

            ElseIf snow > 0 Then

                Dim snomlt As Decimal = tmax + rain * 0.4

                If snomlt > snow Then
                    snomlt = snow
                End If

                snow -= snomlt
                rain += snomlt
            End If

            'Crown Temperature
            Dim tmincr As Decimal = tmin
            Dim tmaxcr As Decimal = tmax
            Dim xs As Decimal = Math.Max(15, snow)

            If tmin < 0 AndAlso xs > 0 Then
                tmincr = 2 + tmin * (0.4 + 0.0018 * (xs - 15D) ^ 2D)
            End If

            If tmax < 0 AndAlso xs > 0 Then
                tmaxcr = 2 + tmax * (0.4 + 0.0018 * (xs - 15D) ^ 2D)
            End If

            Dim tmpcr As Decimal = (tmaxcr + tmincr) / 2D

            'Vernalization day per calendar day
            verday = 0

            If crop_data.TP1VER <= tmpcr AndAlso tmpcr <= crop_data.TP2VER Then

                ' 0 <= tmpcr <= 8 
                verday = 1

            Else

                If crop_data.TBVER < tmpcr AndAlso tmpcr < crop_data.TP1VER Then

                    '-1 < tmpcr < 0
                    verday = (tmpcr - crop_data.TBVER) / (crop_data.TP1VER - crop_data.TBVER)

                Else

                    If crop_data.TP2VER < tmpcr AndAlso tmpcr < crop_data.TCVER Then

                        '8 < tmpcr < 12
                        verday = (crop_data.TCVER - tmpcr) / (crop_data.TCVER - crop_data.TP2VER)

                    End If
                End If
            End If

            Dim SABH As Decimal = 6
            Dim RDN As Decimal = Math.PI / 180D
            Dim ALPHA As Decimal = 90D + SABH
            Dim SMA3_i As Decimal = 0.9856 * data.DayOfYear() - 3.251
            Dim LANDA As Decimal = SMA3_i + 1.916 * Math.Sin(SMA3_i * RDN) + 0.02 * Math.Sin(2 * SMA3_i * RDN) + 282.565
            Dim DEC_i As Decimal = 0.39779 * Math.Sin(LANDA * RDN)

            DEC_i = Math.Atan(DEC_i / Math.Sqrt(1 - DEC_i ^ 2))

            DEC_i /= RDN
            Dim TALSOC As Decimal = 1 / Math.Cos(radiansLat)
            Dim CEDSOC_i As Decimal = 1 / Math.Cos(DEC_i * RDN)
            Dim SOCRA_i As Decimal = (Math.Cos(ALPHA * RDN) * TALSOC * CEDSOC_i) - (Math.Tan(radiansLat) * Math.Tan(DEC_i * RDN))

            pp = Math.PI / 2D - Math.Atan(SOCRA_i / Math.Sqrt(1 - SOCRA_i ^ 2))
            pp /= RDN
            pp = 2D / 15D * pp

        End Sub

        Public Function ThermalTime() As Decimal

            Dim TMINtt As Decimal = Math.Max(0, tmin)
            Dim TMAXtt As Decimal = Math.Max(0, tmax)
            Dim Tt_cum As Decimal = 0
            Dim fr As Decimal
            Dim TH As Decimal

            For r = 1 To 8
                fr = 0.5D * (1D + Math.Cos(11.25D * (2D * r - 1D)))
                TH = TMINtt + fr * (TMAXtt - TMINtt)
                'Thermal Time
                Tt_cum += TH
            Next

            Return 0.125D * Tt_cum
        End Function

    End Class


    Public Class Agronomica30_ModelliGrano
        Inherits AbstractModello

        Private ReadOnly dataSemina As Date
        Private ReadOnly radiansLatitude As Decimal
        Private ReadOnly soil_data As soil
        Private ReadOnly crop_data As crop
        Private ReadOnly modello_fenologico As ModelloFenologico
        Private ReadOnly modello_avversita As ModelloAvversita
        Private ReadOnly condizioni_septoria As CondizioniSeptoria
        Private ReadOnly datiGG As List(Of datoGG)

        Private Class BiologicalDayToStage

            Public Class stage
                Public cbd As Decimal
                Public descr As String
            End Class

            Private ReadOnly qStages As Queue(Of stage)

            Public Sub New(crop_data As crop)
                qStages = New Queue(Of stage)(
                    {
                    New stage With {.cbd = crop_data.bdEMR, .descr = "Emergenza"},
                    New stage With {.cbd = crop_data.bdTIL, .descr = "Primo culmo di accestimento"},
                    New stage With {.cbd = crop_data.bdSEL, .descr = "Primo nodo"},
                    New stage With {.cbd = crop_data.bdBOT, .descr = "Botticella"},
                    New stage With {.cbd = crop_data.bdEAR, .descr = "Emergenza della spiga"},
                    New stage With {.cbd = crop_data.bdANT, .descr = "Antesi"},
                    New stage With {.cbd = crop_data.bdBSG, .descr = "Inizio formazione del seme"},
                    New stage With {.cbd = crop_data.bdTSG, .descr = "Fine formazione del seme"},
                    New stage With {.cbd = crop_data.bdPM, .descr = "Maturazione fisiologica"},
                    New stage With {.cbd = crop_data.bdMAT, .descr = "Maturazione di raccolta"}
                    })
            End Sub

            Public Function newStage(cbd As Decimal) As stage

                If cbd >= qStages.Peek.cbd Then

                    Return qStages.Dequeue()
                End If

                Return Nothing
            End Function
        End Class

        Public Sub New(datiMeteo As MeteoReadOnlyList, Mod_Cod As Integer, params As InputParams)
            MyBase.New(datiMeteo)

            dataSemina = params.dataSemina.Date

            radiansLatitude = params.latitude * Math.PI / 180D

            crop_data = New crop(params.varieta)    'crop = crop_data.Var1; in Crop_Data.mat
            soil_data = New soil(params.tipoSuolo, params.concimazioni) 'ex soil_type

            modello_fenologico = New ModelloFenologico(crop_data, soil_data)

            'Select Case Mod_Cod
            '    Case 
            'End Select

            modello_avversita = New Ruggine_Bruna(crop_data, modello_fenologico)
            modello_avversita = New Ruggine_Gialla_Dennis(crop_data, modello_fenologico)
            modello_avversita = New Fusariosi_Del_Ponte(crop_data, modello_fenologico, params)
            modello_avversita = New Oidio_Audsley(crop_data, modello_fenologico)
            condizioni_septoria = New CondizioniSeptoria
            modello_avversita = New Septoria_leaf_blotch(crop_data, modello_fenologico, condizioni_septoria)

            datiGG = New List(Of datoGG)
        End Sub

        Private Class laixfoglie
            Public data As Date
            Public lai As Decimal
        End Class

        Protected Overrides Function _elaboraModello() As cRisultatoModello

            If Not elabora() Then

                Return Nothing
            End If


            Dim nFoglie As New List(Of laixfoglie)
            Dim current_msnn As Integer = 0

            For Each dgg In datiGG

                If Math.Floor(dgg.msnn) <> current_msnn Then

                    current_msnn = Math.Floor(dgg.msnn)

                    nFoglie.Add(New laixfoglie With {.data = dgg.data, .lai = dgg.lai})

                    'Aggiungo una serie con X0 = datiGG.First.data e X1 = dgg.data ed Y = dgg.lai
                    'per avere nel grafico delle righe verticali tratteggiate che partono dall'asse Y ed arrivano al lai corrente al livello del lai corrente
                    'Indicano il valore del lai in cui si trovano 1, 2, 3, 4, 5, 6, 7, 8, 9 foglie
                End If

            Next


            Dim rismod As New cRisultatoModello

            Dim output As New OutputModello

            output.aggiungiColonna("Data", GetType(Date), "Data", "dd/MM/yyyy")
            output.aggiungiColonna("Rain", GetType(Decimal), Gias.Pioggia & " (mm)", "0.00")
            output.aggiungiColonna("Temp", GetType(Decimal), Gias.Temperatura & " (°C)", "0.00")
            output.aggiungiColonna("RH", GetType(Decimal), Gias.UmiditaRelativa & " (%)", "0.00")
            output.aggiungiColonna("LAI", GetType(Decimal), "LAI", "0.00")
            output.aggiungiColonna("NewStage", GetType(Boolean), "", "") '._hidden = True
            output.aggiungiColonna("StageDescr", GetType(String), "", "") '._hidden = True

            Dim nf As Integer = 1
            For Each f In nFoglie
                output.aggiungiColonna("msnn", GetType(Decimal), "Foglia #" & nf.ToString, "0.00") '._hidden = True
                nf += 1
            Next

            Dim stages As New BiologicalDayToStage(crop_data)
            Dim s As BiologicalDayToStage.stage
            Dim isFirst As Boolean = True

            For Each dgg In datiGG

                output.AddField(dgg.data)
                output.AddField(dgg.rain)
                output.AddField(dgg.tmp)
                output.AddField(dgg.rh)
                output.AddField(dgg.lai)

                s = stages.newStage(dgg.cbd)
                If s IsNot Nothing Then

                    'In grafico LAI  pallino rosso per indicare il nuovo stadio fenologico
                    output.AddField(True)
                    output.AddField(s.descr)

                Else

                    output.AddField(False)
                    output.AddField("")
                End If

                For Each f In nFoglie

                    If isFirst Then

                        output.AddField(f.lai)
                    Else

                        If f.data = dgg.data Then

                            output.AddField(f.lai)
                        Else

                            output.AddField(DBNull.Value)
                        End If
                    End If
                Next

                isFirst = False

                output.Commit()
            Next



            Dim produzione As Decimal = modello_fenologico.get_Produzione()
            Dim raggiunta_prod_media As Boolean = False

            If Math.Round(produzione / 100) * 100 = crop_data.WGRN_media Then

                'Raggiunte le condizioni per avere una produzione standard di granella.
                raggiunta_prod_media = True
            End If

            If Math.Floor(produzione) > crop_data.WGRN_media Then

                Dim percentuale As Decimal = Math.Round((produzione - crop_data.WGRN_media) / crop_data.WGRN_media * 100.0)

                If Not raggiunta_prod_media OrElse percentuale > 0 Then

                    'Raggiunte le condizioni perché si verifichi un aumento di produzione rispetto alla media (+' & percentuale & '%)'
                    raggiunta_prod_media = True
                End If
            End If

            If Not raggiunta_prod_media AndAlso datiGG.Last.cbd > crop_data.bdTSG Then

                Dim percentuale As Decimal = Math.Round((produzione - crop_data.WGRN_media) / crop_data.WGRN_media * 100.0)

                'Produzione media non raggiunta a fine formazione seme
                'è previsto un decremento di produzione rispetto alla media (-' & percentuale & '%)'
            End If



            rismod.Modello_Tabella1 = output.Output()
            rismod.Modello_Tabella2 = modello_avversita.generaOutput()

            Return rismod
        End Function

        Protected Overrides Function _elaboraIndicatore_vecchio(dataInizio As Date, dataFine As Date) As cRisultatoModelloIndicatori.Indicatore
            Throw New NotImplementedException()
        End Function

        Protected Overrides Function _elaboraIndicatore(risElab As RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione) As RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione
            Throw New NotImplementedException()
        End Function

        Private Function elabora() As Boolean

            Dim dgg As datoGG = Nothing
            Dim num_ore As Integer = 0
            Dim domaniSemina = dataSemina.AddDays(1)
            Dim dopoSemina As Boolean = False

            For Each dm In _datiMeteo

                If dopoSemina OrElse dm.DataOra >= domaniSemina Then

                    'se serve si può calcolare il VPD orario
                    'Dim vpd As Decimal = (1D - (dm.UmRel / 100D)) * (6.11D * Math.Exp((17.47D * dm.Temp) / 239D + dm.Temp))

                    condizioni_septoria.add(dm)

                    'Iniziamo dal giorno dopo la semina effettiva
                    dopoSemina = True

                    If dgg Is Nothing OrElse dgg.data.DayOfYear <> dm.DataOra.DayOfYear Then

                        If dgg IsNot Nothing Then

                            dgg.makeDay(num_ore, crop_data, radiansLatitude)

                            modello_fenologico.elabora(dgg)

                            modello_avversita.elabora(dgg)

                        End If

                        dgg = New datoGG(dm)
                        num_ore = 1

                        datiGG.Add(dgg)

                    Else

                        dgg.addHour(dm)
                        num_ore += 1
                    End If

                End If

            Next

            If dgg IsNot Nothing Then

                dgg.makeDay(num_ore, crop_data, radiansLatitude)

                modello_fenologico.elabora(dgg)

                modello_avversita.elabora(dgg)
            End If

            Return datiGG.Any
        End Function

    End Class



    Public Class ModelloFenologico

        Private crop_data As crop
        Private soil_data As soil
        Private prev_dgg As datoGG

        Private CUMVER As Decimal

        Private INLF As Decimal
        Private PLAPOW As Decimal
        Private PLA1 As Decimal
        Private WSFL As Decimal
        Private GLF As Decimal
        Private FXLF As Decimal

        Private NUP As Decimal  'Accumulo giornaliero di azoto
        Private INST As Decimal
        Private INGRN As Decimal
        Private XNLF As Decimal
        Private XNST As Decimal
        Private NLF As Decimal
        Private WSXF As Decimal
        Private WSFD As Decimal
        Private WLF As Decimal
        Private WST As Decimal
        Private GST As Decimal
        Private TRANSL As Decimal
        Private SGR As Decimal
        Private BSGDM As Decimal
        Private WSFG As Decimal
        Private WVEG As Decimal
        Private WGRN As Decimal
        Private SGR_WGRN As Decimal
        Private TRLDM_WGRN As Decimal
        Private HI As Decimal
        Private DHI As Decimal
        Private TRLDM As Decimal
        Private tuBSG As Decimal
        Private tuTSG As Decimal
        Private NST As Decimal
        Private CALB As Decimal
        Private KET As Decimal
        Private FTSWRZ As Decimal
        Private FLDUR As Decimal
        Private CNMIN As Decimal
        Private CTR As Decimal
        Private DYSE As Decimal
        Private DEPORT As Decimal
        Private L_NUMBER_SF9_round As Decimal
        Private Tt As Decimal       'thermal time
        'Private _nitrogen As Integer
        'Private _water As Integer
        'Private _semethod As Integer
        'Private _NATiT As Integer

        Private WGRN_ok As Decimal
        Private WTOP As Decimal

        Private SUM_Rain As Decimal

        Private FTSW As Decimal()
        Private MNORG As Decimal()
        Private NCON As Decimal()
        Private NSOL As Decimal()
        Private WLLL As Decimal()
        Private WLUL As Decimal()
        Private WL As Decimal()
        Private WLAD As Decimal()
        Private WLST As Decimal()
        Private FLOUT As Decimal()
        Private RT As Decimal()
        Private ATSW As Decimal()
        Private TTSW As Decimal()
        Private NAVL As Decimal()
        Private NDNIT As Decimal()
        Private IRGW As Decimal
        Private WSOL As Decimal
        Private WSAT As Decimal
        Private SOLDEP As Decimal
        Private RTLN As Integer
        Private SNAVL As Decimal
        Private CNSOL As Decimal
        Private AROOT As Decimal

        Public Function get_CUMVER() As Decimal
            Return CUMVER
        End Function

        Public Function get_L_NUMBER_SF9_round() As Decimal
            Return L_NUMBER_SF9_round
        End Function

        Public Function get_Produzione() As Decimal
            Return WGRN_ok
        End Function

        Public Sub New(crop_data As crop, soil_data As soil)

            Me.crop_data = crop_data
            Me.soil_data = soil_data
            prev_dgg = Nothing

            CUMVER = 0

            SUM_Rain = 0

            WSFD = 1
            Tt = 0

            '_nitrogen = 2
            '_water = 2 '%e.g.: 1, 2..
            'if (water == 2)
            '    disp('Rainfed')
            'end
            '_semethod = 1
            '_NATiT = 1     '1=DAP-based, 2=CBD-based

            L_NUMBER_SF9_round = 9

            '**************************************************************************************
            'FENOLOGIA
            '**************************************************************************************

            'TBD = 0         'crop.TBD
            'TCD = crop.TCD;
            'TBVER = -1      'crop.TBVER
            'TP1VER = 0      'crop.TP1VER
            'TP2VER = 8      'crop.TP2VER
            'TCVER = 12      'crop.TCVER
            'VDSAT = 50      'crop.VDSAT
            'vsen = 0        'crop.vsen
            'cpp = 21        'crop.cpp
            'ppsen = 0.0026  'crop.ppsen

            'bdSOWEMR = crop.bdSOWEMR;
            'bdEMRTIL = crop.bdEMRTIL;
            'bdTILSEL = crop.bdTILSEL;
            'bdSELBOT = crop.bdSELBOT;
            'bdBOTEAR = crop.bdBOTEAR;
            'bdEARANT = crop.bdEARANT;
            'bdANTPM = crop.bdANTPM;
            'bdPMHM = crop.bdPMHM;

            FTSWRZ = 0

            'SNOW = zeros(1, length_period);
            'CUMVER_vect = zeros(1, length_period);
            'CBD = zeros(1, length_period);
            'VERDAY = zeros(1, length_period);
            'SNOMLT = zeros(1, length_period);
            'LAI = 0;


            INLF = 0
            PLAPOW = crop_data.PLAPOW
            PLAPOW *= (-0.0006 * crop_data.PDEN + 1.1718)
            PLA1 = 0
            WSFL = 1
            GLF = 0


            'If nitrogen = 0

            'If iniLAI = 0 Then

            '    'PHYL = crop.phyl;
            '    'PLACON = crop.PLACON;
            '    'SLA = crop.SLA;
            '    'TKILL = crop.TKILL;
            '    'FRZLDR = crop.FRZLDR;

            '    'MSNN = ones(1, length_period);
            '    'PLA2 = 0;
            '    'LAI = zeros(1, length_period);
            '    'ANTLAI = 0;
            '    'MXLAI = 0;
            '    'WSFL = 1;
            '    iniLAI = 1
            '    'GLAI = zeros(1, length_period);
            '    'DLAI = zeros(1, length_period);
            '    'DLAIF = zeros(1, length_period);
            '    'BSGLAI = zeros(1, length_period);
            '    'INODE = zeros(1, length_period);

            'End If



            'else if nitrogen = 2

            'If iniLAI = 0 Then

            '        'PHYL = crop.phyl;
            '        'PLACON = crop.PLACON;
            'PLAPOW = crop_data.PLAPOW
            '        'SLA = crop.SLA;
            '        'TKILL = crop.TKILL;
            '        'FRZLDR = crop.FRZLDR;
            '        'SLNS = crop.SLNS;
            'PLAPOW = PLAPOW * (-0.0006 * PDEN + 1.1718)

            '        'PLA1 = 0;
            '        'PLA1_vect = zeros(1, length_period);

            '        'PLA2 = 0;
            '        'PLA2_vect = zeros(1, length_period);

            '        'LAI = 0;
            '        'LAI_vect = zeros(1, length_period);

            '        'ANTLAI = zeros(1, length_period);
            '        'MXLAI = 0;
            '        'WSFL = 1;
            '        'SLNG = 2;
            '        iniLAI = 1
            '        'MSNN = ones(1, length_period);
            '        'MSNN_real = ones(1, length_period);
            XNLF = 0
            '        'XNLF_vect = zeros(1, length_period);

            '        'INLF_vect = zeros(1, length_period);

            '        'GLAI = 0;
            '        'GLAI_vect = zeros(1, length_period);

            '        'DLAI = 0;
            '        'DLAI_vect = zeros(1, length_period);

            '        'GLF_vect = zeros(1, length_period);

            '        'BSGLAI = 0;
            '        'BSGLAI_vect = zeros(1, length_period);

            '        'INODE = zeros(1, length_period);

            '    End If


            ''DMProduction
            ''------------------------------- Parameters and Initials
            'If iniDMP = 0 Then
            '    'TBRUE = crop.TBRUE;
            '    'TP1RUE = crop.TP1RUE;
            '    'TP2RUE = crop.TP2RUE;
            '    'TCRUE = crop.TCRUE;
            '    'KPAR = crop.KPAR;
            '    'IRUE = crop.IRUE;

            WSFG = 1
            '    'WSFG_vect = ones(1, length_period);
            '    iniDMP = 1
            'End If


            ''DMDistribution: Parameters and Initials

            'If iniDMD = 0 Then
            '    'FLF1A = crop.FLF1A;
            '    'FLF1B = crop.FLF1B;
            '    'WTOPL = crop.WTOPL;
            '    'FLF2 = crop.FLF2;
            '    'GCC = crop.GCC;
            '    'PDHI = crop.PDHI;
            '    'WDHI1 = crop.WDHI1;
            '    'WDHI2 = crop.WDHI2;
            '    'WDHI3 = crop.WDHI3;
            '    'WDHI4 = crop.WDHI4;

            WLF = 0.5
            '    WLF_vect = 0.5*ones(1, length_period);
            WST = 0.5
            WVEG = WLF + WST
            '    WVEG_vect = WVEG*ones(1, length_period);
            '    'ANTDM = 0;
            WGRN = 0
            '    WGRN_vect = zeros(1, length_period); WGRN_ok_vect = zeros(1, length_period);
            '    'WGRN_concim_vect = zeros(1, length_period);

            SGR_WGRN = 0
            WGRN_ok = 0
            '    'WGRN_concim = 0;

            WTOP = 0
            '    WTOP_vect = zeros(1, length_period);

            HI = 0
            ' HI_vect = zeros(1, length_period);

            GST = 0
            '    GST_vect = zeros(1, length_period);

            TRANSL = 0
            'TRANSL_vect = zeros(1, length_period);
            SGR = 0
            '    SGR_vect = zeros(1, length_period);
            '    iniDMD = 1
            'End If

            tuBSG = 0
            tuTSG = 0
            BSGDM = 0
            TRLDM = 0
            TRLDM_WGRN = 0
            DHI = 0
            ''NonLegumPlantN: Parameters and Initials
            'If iniPNB = 0 Then
            '    'SLNG = crop.SLNG;
            '    'SLNS = crop.SLNS;
            '    'SNCG = crop.SNCG;
            '    'SNCS = crop.SNCS;
            '    'GNC = crop.GNC;
            '    'MXNUP = crop.MXNUP;
            NST = WST * crop_data.SNCG
            'End If

            WSXF = 1

            NUP = 0
            INST = 0
            INGRN = 0

            FXLF = 0

            'nitrogen = 2

            ''%NonLegumPlantN:
            'If iniPNB = 0 Then
            '    '    NLF = LAI * SLNG;
            '    '    NLF_vect = LAI_vect * SLNG;
            '    '    CNUP = NST + NLF;
            '    '    CNUP_vect = CNUP * ones(1, length_period);
            '    iniPNB = 1
            '    '    NUP = 0; NUP_vect = zeros(1, length_period);
            XNST = 0
            '    ; XNST_vect = zeros(1, length_period);
            '    '    INST = 0; INST_vect = zeros(1, length_period);   INGRN = 0; INGRN_vect = zeros(1, length_period);
            '    '    NGRN = 0; NGRN_vect = zeros(1, length_period);
            '    '    NSTDF = 0; NSTDF_vect = zeros(1, length_period);
            '    '    FTSWRZ_vect = zeros(1, length_period);
            SNAVL = 0
            '    ; SNAVL_vect = zeros(1, length_period);
            'End If

            CNSOL = 0

            'If water = 1 OrElse water = 2 OrElse water = 3 Then
            ''SoilWater:
            ''------------------------------- Parameters and Initials
            'If iniSW = 0 Then
            DEPORT = crop_data.iDEPORT   'mm
            '    '    MEED = crop.MEED;          %mm
            '    '    GRTDP = crop.GRTDP;        %mm/bd
            '    '    TEC = crop.TEC;
            '    '    WSSG = crop.WSSG;
            '    '    WSSL = crop.WSSL;
            '    '    WSSD = crop.WSSD;
            '    '    FLDKIL = crop.FLDKL;

            '    '    EOSMIN = 1.5;
            '    '    WETWAT = 10;
            KET = 0.5
            CALB = 0.23

            DYSE = 1
            CTR = 0
            '    '    CE = 0;
            '    '    CRAIN = 0;
            '    '    CRUNOF = 0;
            '    '    CIRGW = 0;
            '    '    IRGNO = 0;
            '    '    CDRAIN = 0;
            '    '    DPTOP = 0;
            RTLN = 0
            WSOL = 0
            '    '    WDUL = 0;
            '    '    WAST = 0;
            SOLDEP = 0
            '    '    ISOLWAT = 0;
            AROOT = 1
            FLDUR = 0

            'ATSWRZ = 0
            'TTSWRZ = 0

            WSAT = 0
            '    '    ETLAI = 0; ETLAI_vect = zeros(1, length_period);
            '    '    for L = 1 : NLYER
            '    '        CLL = DUL(L) - EXTR(L); %mm/mm
            '    '        ADRY = CLL / 3;   %mm/mm
            '    '        WLAD(L) = ADRY * DLYER(L);
            '    '        WLLL(L) = CLL * DLYER(L);
            '    '        WLUL(L) = DUL(L) * DLYER(L);
            '    '        WLST(L) = SAT(L) * DLYER(L);
            '    '        WL(L) = WLLL(L) + (WLUL(L) - WLLL(L)) * MAI(L); %SB: MAI è leggermente diverso nell'Excel
            '    '        ATSW(L) = WL(L) - WLLL(L);

            '    '        WSOL = WSOL + WL(L); % da controllare!! il valore viene leggermente diverso ma il DLYER in VBA è Empty
            '    '        WDUL = WDUL + WLUL(L);
            '    '        WSAT = WSAT + WLST(L);

            '    '        ISOLWAT = ISOLWAT + ATSW(L); % da controllare!! il valore viene leggermente diverso ma il DLYER in VBA è Empty
            '    '        SOLDEP = SOLDEP + DLYER(L);
            '    '    end
            '    '    SE2C = 3.5;     SE1MX = U;     DSR = 1;     SSE1 = U;     SSE = U + SE2C; %Ritchie 2-Stage sevp pars
            '    '    FLOUT = zeros(1,NLYER);
            '    iniSW = 1
            '    '    RT = zeros(1, NLYER);

            ReDim FTSW(soil_data.NLYER - 1)
            ReDim MNORG(soil_data.NLYER - 1)
            ReDim NCON(soil_data.NLYER - 1)
            ReDim NSOL(soil_data.NLYER - 1)
            ReDim WLLL(soil_data.NLYER - 1)
            ReDim WLUL(soil_data.NLYER - 1)
            ReDim WL(soil_data.NLYER - 1)
            ReDim WLAD(soil_data.NLYER - 1)
            ReDim WLST(soil_data.NLYER - 1)
            ReDim FLOUT(soil_data.NLYER - 1)
            ReDim RT(soil_data.NLYER - 1)
            ReDim ATSW(soil_data.NLYER - 1)
            ReDim TTSW(soil_data.NLYER - 1)
            ReDim NAVL(soil_data.NLYER - 1)
            ReDim NDNIT(soil_data.NLYER - 1)

            Dim CLL As Decimal
            Dim NORG As Decimal
            Dim SOILML As Decimal
            Dim ADRY As Decimal

            For L = 0 To soil_data.NLYER - 1

                CLL = soil_data.DUL(L) - soil_data.EXTR(L)      'mm/mm
                ADRY = CLL / 3                                  'mm/mm

                WLLL(L) = CLL * soil_data.DLYER(L)
                WLUL(L) = soil_data.DUL(L) * soil_data.DLYER(L)
                WLST(L) = soil_data.SAT(L) * soil_data.DLYER(L)

                WL(L) = WLLL(L) + (WLUL(L) - WLLL(L)) * soil_data.MAI(L)  'SB: MAI è leggermente diverso nell'Excel

                WLAD(L) = ADRY * soil_data.DLYER(L)

                WSOL += WL(L) 'da controllare!! il valore viene leggermente diverso ma il DLYER In VBA è Empty
                WSAT += WLST(L)

                SOLDEP += soil_data.DLYER(L)

                FTSW(L) = 0

                SOILML = soil_data.DLYER(L) * soil_data.BDL(L) * (1 - soil_data.FG(L)) * 1000   'g.m-2
                NORG = soil_data.NORG(L) * 0.01 * SOILML                                        'g.m-2

                MNORG(L) = NORG * soil_data.FMIN(L)                                             'gN.m-2 org. N avail. for mineralization

                'NSOL(L) = NO3L(L) + soil_data.NH4L(L)                                           'gN.m-2
                NCON(L) = NSOL(L) / (WL(L) * 1000)                                              'gN.g-1 H2O

                FLOUT(L) = 0
                RT(L) = 0

                ATSW(L) = WL(L) - WLLL(L)
                TTSW(L) = 0

                NAVL(L) = 0
                NDNIT(L) = 0

                CNSOL += NSOL(L)

            Next

            IRGW = 0
            'End If


            'If nitrogen = 2 Then


            ''SoilN:
            ''------------------------------- Parameters and initials
            'If iniSNB = 0 Then
            '    '    CNSOL = 0;
            '    '    CNSOL_vect(i) = CNSOL;
            '    '    for L = 1 : NLYER
            '    '        %                 FG = ThisWorkbook.Worksheets("Soil").Cells(SoilRowNo + 6 + L, 8)          %Fraction soil >2mm
            '    '        %                 BDL = ThisWorkbook.Worksheets("Soil").Cells(SoilRowNo + 6 + L, 9)         %Soil bulk density (g.cm-3)
            '    '        %                 NORGP = ThisWorkbook.Worksheets("Soil").Cells(SoilRowNo + 6 + L, 10)      %Organic N (%)
            '    '        %                 FMIN = ThisWorkbook.Worksheets("Soil").Cells(SoilRowNo + 6 + L, 11)       %Fraction Org N avail. for mineralization
            '    '        %                 NO3L = ThisWorkbook.Worksheets("Soil").Cells(SoilRowNo + 6 + L, 12)       %ppm = mg.kg-1
            '    '        %                 NH4L = ThisWorkbook.Worksheets("Soil").Cells(SoilRowNo + 6 + L, 13)       %ppm = mg.kg-1

            '    '        SOILML = DLYER(L) * BDL(L) * (1 - FG(L)) * 1000;    %g.m-2
            '    '        NORG = NORGP(L) * 0.01 * SOILML;                 %g.m-2
            '    '        MNORG(L) = NORG * FMIN(L);                       %gN.m-2 org. N avail. for mineralization

            '    '        NO3L(L) = NO3L(L) * (14 / 62) * 0.000001 * SOILML;  %from ppm to gN.m-2
            '    '        NH4L(L) = NH4L(L) * (14 / 18) * 0.000001 * SOILML ; %from ppm to gn.m-2

            '    '        NSOL(L) = (NO3L(L) + NH4L(L));                      %gN.m-2
            '    '        NCON(L) = NSOL(L) / (WL(L) * 1000);           %gN.g-1 H2O
            '    '        CNSOL = CNSOL + NSOL(L);
            '    '    end
            '    '    CNSOL_vect(i) = CNSOL;

            '    '    INSOL = CNSOL;  CNFERT = 0;   CNFERT_vect = zeros(1, length_period); CNVOL = 0; CNVOL_vect = zeros(1, length_period);
            '    '    CNLEACH = 0;    CNLEACH_vect = zeros(1, length_period);
            CNMIN = 0
            '    CNMIN_vect = zeros(1, length_period);
            '    '    CNDNIT = 0; CNDNIT_vect = zeros(1, length_period);
            '    '    NAVL = zeros(1,NLYER); NDNIT = zeros(1,NLYER);
            '    iniSNB = 1
            'End If

        End Sub

        Private Function tempfun(T As Decimal, T_opt As Decimal) As Decimal

            'beta function

            If crop_data.TBD < T AndAlso T < crop_data.TCD Then

                'Tb -> crop_data.TBD
                'Tc -> crop_data.TCD
                'To -> TP1D

                '***VERIFICARE***
                '𝑡𝑒𝑚𝑝𝑓𝑢𝑛 = [ (𝑇𝑐−𝑇) / (𝑇𝑐−𝑇𝑜) ] × [ (𝑇𝑐−𝑇) / (𝑇𝑜−𝑇𝑏) ] ^ [ (𝑇𝑜−𝑇𝑏) / (𝑇𝑐−𝑇𝑜) ]         if Tb < T < Tc

                Dim m1 As Decimal = (crop_data.TCD - T) / (crop_data.TCD - T_opt)
                Dim m2 As Decimal = (T - crop_data.TBD) / (T_opt - crop_data.TBD)  '???????????????????????    T - Tb   invece di   Tc - T  
                Dim expon As Decimal = (T_opt - crop_data.TBD) / (crop_data.TCD - T_opt)

                Return m1 * Math.Pow(m2, expon)
            End If

            Return 0
        End Function

        Private Function ppfun(pp As Decimal) As Decimal

            'Photoperiod function

            If prev_dgg IsNot Nothing Then

                Dim bdBRP As Decimal = crop_data.bdEMR
                Dim bdTRP As Decimal = crop_data.bdSEL

                If bdBRP <= prev_dgg.cbd AndAlso prev_dgg.cbd <= bdTRP Then

                    If pp < crop_data.cpp Then

                        Return Math.Max(0, 1D - crop_data.ppsen * (crop_data.cpp - pp) ^ 2D)
                    End If
                End If
            End If

            Return 1
        End Function

        Private Function verfun(cbd As Decimal) As Decimal

            Dim bdBRV As Decimal = crop_data.bdEMR
            Dim bdTRV As Decimal = crop_data.bdSEL

            If cbd >= bdBRV AndAlso cbd <= bdTRV Then

                Return Math.Min(1, Math.Max(0, 1 - crop_data.vsen * (crop_data.VDSAT - CUMVER)))
            End If

            Return 1
        End Function

        Private Sub LAI(dgg As datoGG)

            ' LAI initials and pars
            '   Leaf area development and senescence To simulate leaf area expansion,
            '   the first step is to determine on each day the increase in leaf number
            '   on the main stem using the phyllochron (temperature unit between emergence of successive leaves) concept.
            '   Phyllochron values between 90 and 112 oC were found for cultivars of this study.
            '   Plant leaf area is then computed as a function of main stem leaf number and water and nitrogen availability
            '   (Soltani et al., 2006b; Soltani and Sinclair, 2011; Soltani and Sinclair, 2012a).
            '   Leaf production on main stem terminates at the appearance of the ligule of the flag leaf.
            '   Therefore, photoperiod, temperature, vernalization and water availability determine
            '   the time available for leaf production. From emergence to flag leaf ligule appearance, potential plant leaf area each day (y)
            '   is predicted from main stem leaf number on that day (x) using a simple, power function
            '   The coefficient of the function (b) is adjusted for plant density .
            '   Increase in LAI is then computed from increase in plant leaf area between today and yesterday and plant density.
            '   This increase is further limited to the amount calculated based on nitrogen availability,
            '   which is nitrogen allocated to leaf growth (g N m-2 d-1) divided by specific leaf nitrogen (g N m-2 leaf) (also see below).
            '   From flag leaf ligule appearance until the beginning seed growth, leaf area development (due to leaf expansion on tillers)
            '   is calculated from dry matter allocated to leaf growth (10% of daily produced dry matter, see below) and specific leaf area.
            '   It is assumed that plant leaf area development stops at the beginning of seed growth. Under N-limited conditions,
            '   the amount of leaf area senesced each day (DLAI, m2 m-2 d-1) is computed from the amount of nitrogen mobilized
            '   from the leaves (XLN, g N m-2 ground) divided by the difference
            '   between specific leaf nitrogen of green (SPLNG, g N m-2 leaf) and senesced (SLNS, g N m-2 leaf) leaves

#Region "_nitrogen = 0"

            'If _nitrogen = 0 Then

            '    '------------------------------- Yesterday LAI to intercept PAR today
            '    If prev_dgg IsNot Nothing Then

            '        dgg.lai = Math.Max(0, prev_dgg.lai + prev_dgg.glai - prev_dgg.dlai)
            '        'if (LAI(i) > MXLAI)
            '        '    MXLAI = LAI(i);   %Saving maximum LAI
            '        'end

            '    End If

            '    '------------------------------- Daily increase and decrease in LAI today
            '    If dgg.cbd <= crop_data.bdEMR Then


            '    ElseIf dgg.cbd > crop_data.bdEMR AndAlso dgg.cbd <= crop_data.bdTLM Then

            '        INODE_i = dgg.dtu / crop_data.phyl
            '        INODE_i *= WSFL 'AGGIUNTO DA SB

            '        dgg.msnn += INODE_i
            '        Dim PLA2 As Decimal = crop_data.PLACON * dgg.msnn ^ PLAPOW
            '        dgg.glai = ((PLA2 - PLA1) * crop_data.PDEN / 10000) * WSFL
            '        PLA1 = PLA2
            '        'Cumulative biological days<= bdANT + 5 'BSG or beginning linear HI 5 days after anthesis

            '    ElseIf dgg.cbd > crop_data.bdTLM AndAlso dgg.cbd <= crop_data.bdBSG Then

            '        dgg.glai = GLF * crop_data.SLA
            '        dgg.bsglai = dgg.lai             'Saving LAI at BSG

            '    ElseIf dgg.cbd > crop_data.bdBSG Then 'solo dead lai

            '        'Cumulative biological days> bdANT + 5 'BSG fine LAI
            '        dgg.dlai = dgg.bd / (crop_data.bdMAT - crop_data.bdBSG) * dgg.bsglai 'Biological day per calindar day

            '    End If

            '    If dgg.cbd <= crop_data.bdANT Then
            '        'ANTLAI(i) = LAI(i);
            '    End If

            '    '------------------------------- Frost
            '    If dgg.cbd > crop_data.bdEMR AndAlso dgg.tmin < crop_data.TKILL Then

            '        Dim frstf As Decimal = Math.Min(1, Math.Max(0, Math.Abs(dgg.tmin - crop_data.TKILL) * crop_data.FRZLDR))
            '        DLAIF_i = dgg.lai * frstf
            '    End If

            '    If dgg.dlai < DLAIF_i Then
            '        dgg.dlai = DLAIF_i
            '    End If

            'End If

#End Region

            If True Then '_nitrogen = 2 Then

                '**********************************************************************************
                'Yesterday LAI to intercept PAR today
                '**********************************************************************************
                If prev_dgg IsNot Nothing AndAlso prev_dgg.glai > (INLF / crop_data.SLNG) Then

                    dgg.glai = INLF / crop_data.SLNG
                End If

                dgg.lai = Math.Max(0, dgg.lai + dgg.glai - dgg.dlai)
                'if (LAI > MXLAI)
                '    MXLAI = LAI;   %Saving maximum LAI
                'end


                '**********************************************************************************
                'Daily increase and decrease in LAI today
                '**********************************************************************************

                If dgg.cbd > crop_data.bdEMR Then

                    If dgg.cbd <= crop_data.bdTLM Then

                        Dim INODE_i As Decimal = dgg.dtu / crop_data.phyl

                        'if (floor(bd(i)) == 1)
                        '   return
                        'end
                        If prev_dgg IsNot Nothing Then

                            dgg.msnn = prev_dgg.msnn + INODE_i
                        Else

                            dgg.msnn = INODE_i
                        End If

                        If crop_data.bdTIL < dgg.cbd AndAlso dgg.cbd < crop_data.bdBOT Then

                            dgg.msnn_real = INODE_i

                            If prev_dgg IsNot Nothing Then

                                dgg.msnn_real += prev_dgg.msnn_real
                            End If

                        End If

                        Dim PLA2 As Decimal = crop_data.PLACON * dgg.msnn ^ PLAPOW

                        dgg.glai = ((PLA2 - PLA1) * crop_data.PDEN / 10000) * WSFL

                        PLA1 = PLA2

                    Else

                        If dgg.cbd <= crop_data.bdBSG Then

                            dgg.glai = GLF * crop_data.SLA

                            dgg.bsglai = dgg.lai        'Saving LAI at BSG

                            If prev_dgg IsNot Nothing Then

                                dgg.msnn = prev_dgg.msnn

                                If dgg.cbd > crop_data.bdTIL Then

                                    dgg.msnn_real = prev_dgg.msnn_real
                                End If
                            End If

                        Else

                            'crop_data.bdBSG < dgg.cbd

                            If prev_dgg IsNot Nothing Then

                                dgg.msnn = prev_dgg.msnn
                                dgg.msnn_real = prev_dgg.msnn_real
                            End If

                            dgg.dlai = dgg.bd / (crop_data.bdMAT - crop_data.bdBSG) * dgg.bsglai

                        End If
                    End If
                End If

                'If dgg.cbd <= crop_data.bdANT Then
                '    'ANTLAI(i) = LAI;
                'End If


                '**********************************************************************************
                'Frost
                '**********************************************************************************

                Dim DLAIF_i As Decimal = 0

                If crop_data.bdEMR < dgg.cbd AndAlso dgg.tmin < crop_data.TKILL Then

                    Dim frstf As Decimal = Math.Min(1, Math.Max(0, Math.Abs(dgg.tmin - crop_data.TKILL) * crop_data.FRZLDR))
                    DLAIF_i = dgg.lai * frstf

                End If

                If dgg.dlai < DLAIF_i Then

                    dgg.dlai = DLAIF_i
                End If
            End If
        End Sub

        Private Sub Production(dgg As datoGG)

            If True Then '_nitrogen = 2 Then

                '**********************************************************************************
                'DMProduction
                '**********************************************************************************

                Dim FINT As Decimal = 1 - Math.Exp(-crop_data.KPAR * dgg.lai) 'Frazione di radiazione intercettata

                dgg.ddmp = dgg.srad * 0.48 * FINT * crop_data.RUE(dgg.tmp) * WSFG



                '**********************************************************************************
                'DMDistribution:
                '**********************************************************************************

                'Mass partitioning and yield formation

                If dgg.cbd <= crop_data.bdEMR OrElse crop_data.bdTSG < dgg.cbd Then

                    dgg.ddmp = 0
                    GLF = 0
                    GST = 0
                    TRANSL = 0
                    SGR = 0
                    SGR_WGRN = 0

                Else

                    If crop_data.bdEMR < dgg.cbd AndAlso dgg.cbd <= crop_data.bdTLM Then

                        Dim FLF1 As Decimal = crop_data.FLF1B
                        If WTOP < crop_data.WTOPL Then
                            FLF1 = crop_data.FLF1A
                        End If

                        GLF = FLF1 * dgg.ddmp
                        GST = dgg.ddmp - GLF

                    Else

                        If dgg.cbd < crop_data.bdBSG Then

                            GLF = crop_data.FLF2 * dgg.ddmp
                            GST = dgg.ddmp - GLF

                            BSGDM = WTOP 'Saving WTOP at BSG

                            Dim DHIF As Decimal = 0
                            If BSGDM <= crop_data.WDHI1 OrElse BSGDM >= crop_data.WDHI4 Then

                                DHIF = 0
                            Else

                                If BSGDM < crop_data.WDHI2 Then

                                    DHIF = (BSGDM - crop_data.WDHI1) / (crop_data.WDHI2 - crop_data.WDHI1)
                                Else

                                    If BSGDM <= crop_data.WDHI3 Then

                                        DHIF = 1
                                    Else

                                        DHIF = (crop_data.WDHI4 - BSGDM) / (crop_data.WDHI4 - crop_data.WDHI3)
                                    End If
                                End If
                            End If

                            DHI = crop_data.PDHI * DHIF
                            TRLDM = BSGDM * crop_data.FRTRL

                            '%%%%% ADDED SB
                            TRLDM_WGRN = TRLDM
                            '%%%%% END SB

                        Else

                            If dgg.cbd <= crop_data.bdTSG Then

                                SGR = DHI * (WTOP + dgg.ddmp) + dgg.ddmp * HI

                                If dgg.lai = 0 AndAlso NST <= (WST * crop_data.SNCS) Then

                                    SGR = 0   'There is no N for seed filling
                                End If

                                If (SGR / crop_data.GCC) > dgg.ddmp Then

                                    TRANSL = (SGR / crop_data.GCC) - dgg.ddmp

                                    If TRANSL > TRLDM Then

                                        TRANSL = TRLDM
                                    Else

                                        If (SGR / crop_data.GCC) <= dgg.ddmp Then

                                            TRANSL = 0
                                        End If
                                    End If
                                End If

                                TRLDM -= TRANSL

                                If SGR > (dgg.ddmp + TRANSL) * crop_data.GCC Then

                                    SGR = (dgg.ddmp + TRANSL) * crop_data.GCC
                                End If

                                '%%%%% ADDED
                                Dim TRANSL_WGRN As Decimal = dgg.dtu / (tuTSG - tuBSG) * TRLDM_WGRN

                                If TRANSL_WGRN > TRLDM_WGRN Then

                                    TRANSL_WGRN = TRLDM_WGRN
                                End If

                                TRLDM_WGRN -= TRANSL_WGRN

                                SGR_WGRN = (dgg.ddmp + TRANSL_WGRN) * crop_data.GCC
                                '%%%%% END

                                If (SGR / crop_data.GCC) < dgg.ddmp Then

                                    GST = dgg.ddmp - (SGR / crop_data.GCC)
                                End If

                            End If
                        End If
                    End If
                End If
            End If

            WLF += GLF
            WST += GST

            WGRN += SGR

            WGRN_ok += SGR_WGRN

            WVEG += dgg.ddmp - (SGR / crop_data.GCC)

            WTOP = WVEG + WGRN
            HI = WGRN / WTOP

            'If dgg.cbd <= crop_data.bdANT Then
            '    'ANTDM = WTOP
            'End If
        End Sub

        Private Sub BilancioAzoto(dgg As datoGG)

            If True Then '_nitrogen = 2 Then

                If dgg.cbd <= crop_data.bdEMR OrElse dgg.cbd > crop_data.bdTSG Then

                    NUP = 0
                    XNLF = 0
                    XNST = 0
                    INLF = 0
                    INST = 0
                    INGRN = 0

                Else

                    If dgg.cbd < crop_data.bdBSG Then

                        Dim NSTDF As Decimal = Math.Max(0, (WST * crop_data.SNCG) - NST)
                        NUP = Math.Min(crop_data.MXNUP, Math.Max(0, (GST * crop_data.SNCG) + (dgg.glai * crop_data.SLNG) + NSTDF))

                        If FTSWRZ > 1 Then
                            NUP *= WSXF
                        End If

                        If dgg.ddmp = 0 Then
                            NUP = 0
                        End If
                        If NUP > SNAVL Then
                            NUP = SNAVL
                        End If

                        If NST <= WST * crop_data.SNCS Then

                            INST = WST * crop_data.SNCS - NST
                            XNST = 0

                            If INST >= NUP Then

                                INLF = 0
                                XNLF = INST - NUP

                            Else

                                INLF = dgg.glai * crop_data.SLNG
                                If INLF > (NUP - INST) Then
                                    INLF = NUP - INST
                                End If
                                INST = NUP - INLF
                                XNLF = 0
                            End If

                        Else

                            INLF = dgg.glai * crop_data.SLNG
                            XNLF = 0

                            If INLF >= NUP Then

                                INST = 0

                                XNST = INLF - NUP

                                If XNST > (NST - WST * crop_data.SNCS) Then

                                    XNST = NST - WST * crop_data.SNCS
                                End If

                                INLF = NUP + XNST

                            Else

                                INST = NUP - INLF
                                XNST = 0
                            End If
                        End If
                    Else

                        If dgg.cbd <= crop_data.bdTSG Then

                            INGRN = SGR * crop_data.GNC
                            NUP = INGRN

                            If FTSWRZ > 1 Then
                                NUP = 0
                            End If
                            If dgg.ddmp <= (SGR / crop_data.GCC) Then
                                NUP = 0
                            End If
                            If dgg.ddmp = 0 Then
                                NUP = 0
                            End If
                            If NUP > SNAVL Then
                                NUP = SNAVL
                            End If

                            If NUP > (SGR * crop_data.GNC) Then

                                'N is excess of seed needs
                                INLF = 0
                                INST = NUP - SGR * crop_data.GNC
                                XNLF = 0
                                XNST = 0

                            Else

                                'Need to transfer N from vegetative tissue
                                INLF = 0
                                INST = 0
                                XNLF = (SGR * crop_data.GNC - NUP) * FXLF
                                XNST = (SGR * crop_data.GNC - NUP) * (1 - FXLF)
                            End If
                        End If
                    End If
                End If

                NST += INST - XNST
                NLF += INLF - XNLF
                'NVEG = NLF + NST;
                'NGRN += INGRN;
                'CNUP += NUP;

                Dim TRLN As Decimal = dgg.lai * (crop_data.SLNG - crop_data.SLNS) + (NST - WST * crop_data.SNCS)
                FXLF = Math.Min(1, Math.Max(0, dgg.lai * (crop_data.SLNG - crop_data.SLNS) / (TRLN + 0.000000000001)))

            End If

            Dim RLYER(soil_data.NLYER - 1) As Decimal

            If True Then '_water = 1 OrElse _water = 2 OrElse _water = 3 Then

                '**********************************************************************************
                'SoilWater:
                '**********************************************************************************
                'Irrigation based on DAP, CBD or DOY
                'If _water = 1 Then
                '    '    if (FTSWRZ <= IRGLVL && CBD(i) < bdTSG)
                '    '        IRGW = (TTSWRZ - ATSWRZ);
                '    '        IRGNO = IRGNO + 1;
                '    '        IRGW_vect(i) = IRGW;
                '    '    else
                '    '        IRGW = 0;
                '    '        IRGW_vect(i) = IRGW;
                '    '    end
                'End If

                'CIRGW = CIRGW + IRGW;


                '**********************************************************************************
                'Drainage
                '**********************************************************************************

                'DRAIN = FLOUT(LDRAIN);
                'CDRAIN = CDRAIN + DRAIN;


                '**********************************************************************************
                'Root depth increase
                '**********************************************************************************

                Dim GRTD As Decimal = crop_data.GRTDP * dgg.bd
                Dim bdBRG As Decimal = crop_data.bdEMR
                Dim bdTRG As Decimal = crop_data.bdBSG

                If dgg.cbd < bdBRG Then
                    GRTD = 0
                End If
                If dgg.cbd > bdTRG Then
                    GRTD = 0
                End If
                If dgg.ddmp = 0 Then
                    GRTD = 0
                End If
                If DEPORT >= SOLDEP Then
                    GRTD = 0
                End If
                If DEPORT >= crop_data.MEED Then
                    GRTD = 0
                End If
                If ATSW(RTLN) = 0 Then
                    GRTD = 0
                End If

                DEPORT += GRTD
                RTLN = 0

                Dim DPTOP As Decimal = 0

                For L = 0 To soil_data.NLYER - 1    'Loop to find if there is roots in each layer

                    RLYER(L) = Math.Max(0, Math.Min(soil_data.DLYER(L), DEPORT - DPTOP))

                    If RLYER(L) > 0 Then
                        RTLN = L
                    End If

                    DPTOP += soil_data.DLYER(L)
                Next


                '**********************************************************************************
                'Runoff
                '**********************************************************************************

                Dim RUNOF As Decimal = 0
                If dgg.rain > 0.01 Then '_water = 2 AndAlso dgg.rain > 0.01 Then '

                    Dim CN As Decimal = 73
                    Dim S As Decimal = 254 * (100 / CN - 1)
                    Dim SWER As Decimal = Math.Max(0, 0.15 * (((WLST(0) - WL(0)) / (WLST(0) - WLLL(0))) * 0.5 + ((WLST(1) - WL(1)) / (WLST(1) - WLLL(1))) * 0.5))

                    If (dgg.rain - SWER * S) > 0 Then

                        RUNOF = Math.Pow(dgg.rain - SWER * S, 2) / (dgg.rain + (1 - SWER) * S)
                    Else

                        RUNOF = 0
                    End If
                End If

                If WSOL > WSAT Then
                    RUNOF += WSOL - WSAT
                End If

                'CRAIN = CRAIN + RAIN(i);
                'CRUNOF = CRUNOF + RUNOF;


                '**********************************************************************************
                'LAI for soil evaporation
                '**********************************************************************************

                Dim ETLAI As Decimal

                If dgg.cbd <= crop_data.bdBSG Then

                    ETLAI = dgg.lai
                Else

                    ETLAI = dgg.bsglai
                End If


                '**********************************************************************************
                'Potential ET
                '**********************************************************************************

                Dim SALB As Decimal = 0.13
                Dim TD_i As Decimal = 0.6 * dgg.tmax + 0.4 * dgg.tmin
                Dim ALBEDO As Decimal = CALB * (1 - Math.Exp(-KET * ETLAI)) + SALB * Math.Exp(-KET * ETLAI)
                Dim EEQ_i As Decimal = dgg.srad * (0.004876 - 0.004374 * ALBEDO) * (TD_i + 29)
                Dim PET_i As Decimal = EEQ_i * 1.1

                If dgg.tmax > 34 Then

                    PET_i = EEQ_i * ((dgg.tmax - 34) * 0.05 + 1.1)
                End If

                If dgg.tmax < 5 Then

                    PET_i = EEQ_i * 0.01 * Math.Exp(0.18 * (dgg.tmax + 20))
                End If


                '**********************************************************************************
                'Soil evaporation
                '**********************************************************************************

                Dim EOS As Decimal = PET_i * Math.Exp(-KET * ETLAI)
                Dim EOSMIN As Decimal = 1.5
                Dim WETWAT As Decimal = 10

                If PET_i > EOSMIN AndAlso EOS < EOSMIN Then
                    EOS = EOSMIN
                End If

                Dim SEVP As Decimal = EOS

                If (dgg.rain + IRGW) > WETWAT Then

                    DYSE = 1
                End If

                If DYSE > 1 OrElse FTSWRZ < 0.5 OrElse ATSW(0) <= 1 Then

                    SEVP = EOS * (Math.Pow(DYSE + 1, 0.5) - Math.Pow(DYSE, 0.5))
                    DYSE += 1
                End If

#Region "_semethod = 2"

                '------------------------------------------ A modified Ritchie 2-stage soil evp
                'If _semethod = 2 Then
                '    '    if ATSW(1) <= WLAD(1)
                '    '        SEVP = 0;
                '    '        SEVP_vect(i) =  SEVP;
                '    '    else
                '    '        if (SSE1 < SE1MX)
                '    '            %Stage I evaporation
                '    '            SEVP = EOS;
                '    '            SEVP_vect(i) =  SEVP;
                '    '            if (SEVP > (SE1MX - SSE1))
                '    '                SEVP = SE1MX - SSE1;
                '    '                SEVP_vect(i) =  SEVP;
                '    '            end
                '    '            SSE1 = SSE1 + SEVP;
                '    '            SSE = SSE + SEVP;

                '    '            if (SSE1 >= SE1MX)
                '    '                %Transition from Stage I to Stage II
                '    '                SEVP = SEVP + SE2C * (DSR ^ 0.5 - (DSR - 1) ^ 0.5) * (1 - (SEVP / EOS));
                '    '                SEVP_vect(i) =  SEVP;
                '    '                DSR = DSR + 1 - SEVP / EOS;
                '    '            end
                '    '        else
                '    '            %Stage II evaporation
                '    '            SEVP = SE2C * (DSR ^ 0.5 - (DSR - 1) ^ 0.5);
                '    '            SEVP_vect(i) =  SEVP;
                '    '            if (SEVP > EOS)
                '    '                SEVP = EOS;
                '    '                SEVP_vect(i) =  SEVP;
                '    '            end
                '    '            DSR = DSR + 1;
                '    '            SSE = SSE + SEVP;
                '    '        end

                '    '        FLUX1 = RAIN(i) + IRGW - RUNOF;
                '    '        if (FLUX1 >= SSE1)
                '    '            SSE = SSE - FLUX1;
                '    '            if (SSE < 0)
                '    '                SSE = 0;
                '    '            end
                '    '            SSE1 = 0;
                '    '            DSR = 1 + (SSE / SE2C) ^ 0.5;
                '    '        else
                '    '            SSE = SSE - FLUX1;
                '    '            SSE1 = SSE1 - FLUX1;
                '    '            DSR = 1 + (SSE / SE2C) ^ 0.5;
                '    '        end
                '    '    end
                'End If

#End Region

                '---------------------------------------------------------------------------------

                'CE = CE + SEVP;


                '**********************************************************************************
                'Plant transpiration
                '**********************************************************************************

                Dim VPTMIN_i As Decimal = 0.6108 * Math.Exp(17.27 * dgg.tmin / (dgg.tmin + 237.3))
                Dim VPTMAX_i As Decimal = 0.6108 * Math.Exp(17.27 * dgg.tmax / (dgg.tmax + 237.3))
                Dim VPDF As Decimal = 0.75
                Dim VPD As Decimal = VPDF * (VPTMAX_i - VPTMIN_i)
                Dim TR_i As Decimal = Math.Max(0, dgg.ddmp * VPD / crop_data.TEC)         'VPD in kPa, TEC in Pa

                CTR += TR_i


                '**********************************************************************************
                'Updating
                '**********************************************************************************

                Dim WU(soil_data.NLYER - 1) As Decimal
                Dim SE(soil_data.NLYER - 1) As Decimal

                Dim WUUR As Decimal = TR_i / AROOT
                Dim TSE As Decimal = SEVP

                For L = 0 To soil_data.NLYER - 1

                    WU(L) = RLYER(L) * RT(L) * WUUR

                    If WL(L) <= WLAD(L) Then

                        SE(L) = 0

                    Else

                        Dim SE_Bound As Decimal = (WL(L) - WLAD(L)) * soil_data.DRAINF(L)

                        SE(L) = Math.Min(SE_Bound, TSE)
                    End If

                    TSE = Math.Max(0, TSE - SE(L))
                Next

                Dim WRZ As Decimal = 0
                Dim WRZUL As Decimal = 0
                Dim WRZST As Decimal = 0
                Dim ATSWRZ As Decimal = 0
                Dim TTSWRZ As Decimal = 0
                WSOL = 0
                'ATSWSL = 0;
                'TTSWSL = 0;
                AROOT = 0

                Dim FLIN(soil_data.NLYER - 1) As Decimal

                For L = 0 To soil_data.NLYER - 1

                    If L = 0 Then

                        FLIN(L) = dgg.rain + IRGW - RUNOF   'today recharge

                    Else

                        FLIN(L) = FLOUT(L - 1)
                    End If

                    If FLIN(L) < 0 Then

                        FLIN(L) = 0
                    End If


                    WL(L) += FLIN(L) - WU(L) - SE(L)
                    FLOUT(L) = Math.Max(0, (WL(L) - WLUL(L)) * soil_data.DRAINF(L))
                    WL(L) -= FLOUT(L)
                    ATSW(L) = Math.Max(0, WL(L) - WLLL(L))

                    TTSW(L) = WLUL(L) - WLLL(L)
                    FTSW(L) = ATSW(L) / TTSW(L)

                    If SUM_Rain < 330 OrElse dgg.cbd <= crop_data.bdBSG Then '&& CBD(i) <= bdBSG % modifica 28/09/2020 vedi anche righe successive

                        If FTSW(L) > crop_data.WSSG Then

                            RT(L) = 1

                        Else

                            RT(L) = Math.Max(0, FTSW(L) / crop_data.WSSG)
                        End If
                    Else

                        RT(L) = 1
                    End If

                    AROOT += RLYER(L) * RT(L)
                    WRZ += WL(L) * (RLYER(L) / soil_data.DLYER(L))
                    WRZUL += WLUL(L) * (RLYER(L) / soil_data.DLYER(L))
                    WRZST += WLST(L) * (RLYER(L) / soil_data.DLYER(L))
                    ATSWRZ += ATSW(L) * (RLYER(L) / soil_data.DLYER(L))
                    TTSWRZ += TTSW(L) * (RLYER(L) / soil_data.DLYER(L))
                    FTSWRZ = ATSWRZ / TTSWRZ

                    WSOL += WL(L)
                    'ATSWSL += ATSW(L)

                Next


                '**********************************************************************************
                'Water-stress-effect factors
                '**********************************************************************************

                If FTSWRZ > crop_data.WSSL Then

                    WSFL = 1
                Else

                    WSFL = FTSWRZ / crop_data.WSSL
                End If

                If SUM_Rain < 330 OrElse dgg.cbd <= crop_data.bdBSG Then '&& CBD(i) <= bdBSG % modifica 28/09/2020 vedi anche righe precedenti

                    If dgg.cbd <= crop_data.bdBOT Then

                        If FTSWRZ > 0.8 Then 'WSSG 0.3 --> ora modificato, 0.1

                            WSFG = 1
                        Else

                            'WSFG = FTSWRZ / WSSG;
                            WSFG = FTSWRZ / 0.8 'modificato 0.03
                            'disp('Deficit di acqua.') % irrigazione di soccorso
                        End If
                    Else

                        If FTSWRZ > crop_data.WSSG Then

                            WSFG = 1
                        Else

                            'WSFG = FTSWRZ / WSSG;
                            WSFG = FTSWRZ / crop_data.WSSG 'modificato 0.03
                            'disp('Deficit di acqua.') % irrigazione di soccorso
                        End If

                    End If

                Else 'modifica 28/09/2020 vedi anche righe precedenti

                    WSFG = 1
                End If

                WSFD = (1 - WSFG) * crop_data.WSSD + 1

                If WRZ <= WRZUL Then
                    WSXF = 1
                Else
                    WSXF = (WRZST - WRZ) / (WRZST - WRZUL)
                End If

                If WRZ > (0.95 * WRZST) Then

                    WSFG = 0
                    WSFL = 0
                    FLDUR += 1

                Else

                    FLDUR = 0
                End If

                If FLDUR > crop_data.FLDKL Then

                    dgg.cbd = crop_data.bdTSG
                End If

            End If

            If True Then '_nitrogen = 2 Then

                'SoilN:
                'N net mineralization

                Dim TMPS_i As Decimal = Math.Min(35, dgg.tmp)
                Dim SNMIN As Decimal = 0
                Dim NMIN(soil_data.NLYER - 1) As Decimal

                For L = 0 To soil_data.NLYER - 1

                    Dim KN As Decimal = 24 * Math.Exp(17.753 - 6350.5 / (TMPS_i + 273)) / 168
                    Dim rn As Decimal
                    If FTSW(L) < 0.9 Then
                        rn = 1.111 * FTSW(L)
                    Else
                        rn = 10 - 10 * FTSW(L)
                    End If
                    rn = Math.Max(0, rn)

                    NMIN(L) = MNORG(L) * rn * (1 - Math.Exp(-KN))
                    NMIN(L) = Math.Max(0, NMIN(L) * (0.0002 - NCON(L)) / 0.0002)     'threshold = 200 mgN.L-1
                    MNORG(L) = MNORG(L) - NMIN(L)

                    SNMIN += NMIN(L)

                Next

                CNMIN += SNMIN


                '**********************************************************************************
                'N application & volatilization
                '**********************************************************************************

                Dim NFERT As Decimal = 0
                Dim NVOL As Decimal = 0
                Dim FN As Integer = soil_data.FN

                If True Then '_NATiT = 1 Then

                    'N appl. based on DAP
                    For N = 0 To FN - 1

                        Dim ferti = soil_data.getFerti(N)

                        If dgg.data = ferti.data Then

                            NVOL = (ferti.volfi / 100) * ferti.nferti     'gN.m-2
                        End If
                    Next

                    'ElseIf _NATiT = 2 Then

                    '    'N appl. based On CBD
                    '    For N = 0 To FN - 1
                    '        '        if (CBD(i) >= DAPNF(N) && NFA01(N) == 0)
                    '        '            NFERT = NFERTI(N);             %gN.m-2
                    '        '            VOLF = VOLFI(N) / 100;
                    '        '            NVOL = VOLF * NFERT;        %gN.m-2
                    '        '            NFA01(N) = 1;
                    '        '        end

                    '    Next
                End If

                'CNFERT += NFERT
                'CNVOL += NVOL


                '**********************************************************************************
                'N downward movement
                '**********************************************************************************

                Dim NOUT(soil_data.NLYER - 1) As Decimal

                For L = 0 To soil_data.NLYER - 1

                    NOUT(L) = NSOL(L) * (FLOUT(L) / (WL(L) + FLOUT(L))) 'gN.m-2

                    If NCON(L) <= 0.000001 Then
                        NOUT(L) = 0 'threshold = 1 mgN.L-1
                    End If
                Next

                'NLEACH = NOUT(NLYER);
                'CNLEACH = CNLEACH + NLEACH;


                '**********************************************************************************
                'N denitification
                '**********************************************************************************

                Dim SNDNIT As Decimal = 0

                For L = 0 To soil_data.NLYER - 1

                    If L = 0 OrElse L = 1 Then  'Dnit from layers 1 & 2

                        If FTSW(L) > 1 Then

                            Dim XNCON As Decimal = Math.Min(0.0004, NCON(L)) 'threshold = 400 mgN.L-1
                            Dim KDNIT As Decimal = 6 * Math.Exp(0.07735 * TMPS_i - 6.593) 'rate coef.

                            NDNIT(L) = XNCON * (1 - Math.Exp(-KDNIT))       'gN.g-1 H2O
                            NDNIT(L) *= WL(L) * 1000                        'gN.m-2

                        Else

                            NDNIT(L) = 0
                        End If
                    Else

                        NDNIT(L) = 0
                    End If

                    SNDNIT += NDNIT(L)
                Next

                'CNDNIT += SNDNIT


                '**********************************************************************************
                'Updating
                '**********************************************************************************

                Dim SAVE_SNAVL As Decimal = SNAVL

                SNAVL = 0
                CNSOL = 0

                For L = 0 To soil_data.NLYER - 1

                    'N removal by plant uptake
                    Dim NU_L As Decimal = NAVL(L) / (SAVE_SNAVL + 0.000001) * NUP

                    If L = 0 Then
                        NSOL(L) = NSOL(L) + NMIN(L) + NFERT - NVOL - NOUT(L) - NDNIT(L) - NU_L
                    Else
                        NSOL(L) = NSOL(L) + NOUT(L - 1) + NMIN(L) - NOUT(L) - NDNIT(L) - NU_L
                    End If

                    NCON(L) = NSOL(L) / (WL(L) * 1000)

                    NAVL(L) = Math.Max(0, (NCON(L) - 0.000001) * ATSW(L) * 1000 * (RLYER(L) / soil_data.DLYER(L))) 'threshold = 1 mgN.L-1

                    SNAVL += NAVL(L)
                    CNSOL += NSOL(L)

                Next

            End If

        End Sub

        Public Sub elabora(dgg As datoGG)

            'Cumulative vernalization day
            CUMVER += dgg.verday

            If CUMVER < 10 AndAlso dgg.tmax > 30 Then

                'de-vernalizzazione
                CUMVER -= 0.5D * (dgg.tmax - 30D)

            End If

            CUMVER = Math.Max(0, CUMVER)

            SUM_Rain += dgg.rain

            Dim TP1D As Decimal = 29    'crop_data.TP1D

            If prev_dgg IsNot Nothing Then

                If prev_dgg.cbd < crop_data.bdTIL Then

                    TP1D = 27.5
                Else

                    If prev_dgg.cbd <= crop_data.bdSEL Then

                        TP1D = 27.4
                    Else

                        If prev_dgg.cbd <= crop_data.bdBOT Then

                            TP1D = 29.5
                        Else

                            If prev_dgg.cbd <= crop_data.bdEAR Then

                                TP1D = 28
                            Else

                                If prev_dgg.cbd <= crop_data.bdANT Then

                                    TP1D = 29.1 '28.7
                                End If
                            End If
                        End If
                    End If
                End If
            End If

            Dim _verfun As Decimal = verfun(dgg.cbd)
            Dim _tempfun As Decimal = tempfun(dgg.tmp, TP1D)
            Dim _ppfun As Decimal = ppfun(dgg.pp)

            'Biological day

            dgg.dtu = (TP1D - crop_data.TBD) * _tempfun
            dgg.bd = _verfun * _tempfun * _ppfun

            If prev_dgg Is Nothing Then

                dgg.cbd = dgg.bd

            Else

                If prev_dgg.cbd > crop_data.bdEMR Then

                    dgg.dtu *= WSFD
                    dgg.bd *= WSFD

                End If

                dgg.cbd = prev_dgg.cbd + dgg.bd

            End If

            If dgg.cbd < crop_data.bdBSG Then

                tuBSG += dgg.dtu
            End If

            If dgg.cbd < crop_data.bdTSG Then

                tuTSG += dgg.dtu
            End If


            LAI(dgg)


            Production(dgg)


            BilancioAzoto(dgg)


            'PREDIZIONE NUMERO DI FOGLIE 

            If Math.Round(CUMVER) <= 50 Then

                Tt += dgg.ThermalTime()
            End If

            'CALCOLO DEL NUMERO DI FOGLIE L_NUMBER PER VARIETà INVERNALI
            If Math.Round(CUMVER) = 50 Then

                Dim L_NUMBER_SF9 As Decimal = 11.5 + 0.00502 * Tt - 0.42 * dgg.pp
                L_NUMBER_SF9_round = Math.Round(L_NUMBER_SF9)

            End If


            prev_dgg = dgg

        End Sub
    End Class



    Public MustInherit Class ModelloAvversita

        Public Enum Stato
            PreFase = -1
            InFase = 0
            PostFase = 1
        End Enum

        Protected ReadOnly crop_data As crop
        Protected ReadOnly fenologia As ModelloFenologico
        Protected _stato As Stato

        Protected Sub New(crop_data As crop, fenologia As ModelloFenologico)
            Me.crop_data = crop_data
            Me.fenologia = fenologia
            _stato = Stato.PreFase
        End Sub

        Public Function getStato() As Stato
            Return _stato
        End Function

        Public MustOverride Sub elabora(dgg As datoGG)
        Public MustOverride Function generaOutput() As String
    End Class


    Public Class Ruggine_Bruna
        Inherits ModelloAvversita

        Private Class eventoInf
            Public data As Date                 'Data evento infettivo
            Public periodoDiLatenza As Integer  'Periodo di latenza (In tabella output Data evento infettivo + periodo di latenza -> Data comparsa sintomi)
            Public sintomi As Boolean
            Public indiceInf As Decimal         'Indice infettività per indicatore
        End Class

        Private ReadOnly eventi As List(Of eventoInf)

        Public Sub New(crop_data As crop, fenologia As ModelloFenologico)

            MyBase.New(crop_data, fenologia)

            eventi = New List(Of eventoInf)

            'RLA = 0; RLA_vect = zeros(1, length_period); % Total rusted leaf area [cm2 cm-2]
            'DRLA = zeros(1, length_period); % Daily increase of RLA [cm2 cm-2]
            'ILA = zeros(1, length_period); % Infectious leaf area [cm2 cm-2]
            'INF_ruggine_bruna = zeros(1, length_period); % Infection efficiency of uredospores (0-1)
            'ALA = zeros(1, length_period); % Affectable leaf area [cm2 cm-2]
            'FAI = zeros(1, length_period); % Failure rate of latent infections (0-1)
            'CV = 0.6; % CARATTERIZZARE QUESTO PARAMETRO
            'INF_ruggine_bruna = zeros(1, length_period);
            'RB_diamond = NaN(1, length_period);

        End Sub

        Public Overrides Sub elabora(dgg As datoGG)

            If dgg.cbd < crop_data.bdSEL Then
                _stato = Stato.PreFase
                Return
            End If

            If crop_data.bdTSG < dgg.cbd Then
                _stato = Stato.PostFase
                Return
            End If

            _stato = Stato.InFase

            Dim LP As Decimal = 61.023 * dgg.tmp ^ (-0.7019) 'LP Is the time, expressed In days, that elapses between the arrival of uredospore And the eruption of uredia
            LP = Math.Round(LP)

            Dim INF_ruggine_bruna_i As Decimal = 0

            If dgg.lw >= 3 Then

                INF_ruggine_bruna_i = Math.Min(1, Math.Max(0, (-78.011 + 15.995 * dgg.tmp - 0.556 * dgg.tmp ^ 2 + 3.346 * dgg.lw) / 100D))
            End If

            'if(sum(dap >= dap_inf) >= 1) % sum(dap >= dap_inf) >= 1 --> almeno una dà sintomi
            '    disp(strcat('[RUGGINE BRUNA] Probabile presenza di sintomi il giorno', 32, num2str(dap)))
            '    [ date ] = from_DOY_to_date( dap, str2num(data_semina_day), str2num(data_semina_month), str2num(data_semina_year) );
            '    dap_inf(dap >= dap_inf) = realmax;
            'end

            Dim evento As New eventoInf With {
                .data = dgg.data,
                .periodoDiLatenza = LP,
                .sintomi = False,
                .indiceInf = INF_ruggine_bruna_i
            }

            'Indicatore
            '   indiceInf < 0.5 -> VERDE
            '   indiceInf < 0.8 -> GIALLO
            '   indiceInf >= 0.8 -> ROSSO
            '
            'In Tabella solo se sintomi = True

            eventi.Add(evento)

            Dim round_INF_ruggine_bruna As Decimal = Math.Round(INF_ruggine_bruna_i, 1)

            If dgg.cbd <= crop_data.bdBOT Then

                'primo periodo [stem elongation - booting]
                If round_INF_ruggine_bruna >= 0.8 Then

                    evento.sintomi = True

                    '    disp(strcat('[RUGGINE BRUNA] EVENTO INFETTIVO il giorno [stem elongation - booting]', 32, num2str(dap), 32, 'con PROBABILE COMPARSA SINTOMI il ', 32, num2str(dap + LP)))
                    '    [ date ] = from_DOY_to_date( dap, str2num(data_semina_day), str2num(data_semina_month), str2num(data_semina_year) );
                    '    [ date ] = from_DOY_to_date( dap + LP, str2num(data_semina_day), str2num(data_semina_month), str2num(data_semina_year) );
                    '    index_inf = index_inf + 1;
                    '    % dap_inf(index_inf) = dap + IP;
                    '    dap_inf(index_inf) = dap + LP;

                    '    RB_diamond(i) = 1.1;
                    '    w_rb = 3;
                    '    disp(num2str(round(INF_ruggine_bruna(i),1,'significant')))
                    '    disp(strcat('[RUGGINE BRUNA] INDICE di infettività', 32, num2str(INF_ruggine_bruna(i)), 32, 'ROSSO'))
                    '    disp(strcat('[RUGGINE BRUNA] LP:', 32, num2str(LP)))

                    '    %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
                    '    % Le parti commentate di seguito sono comunque utili per la costruzione
                    '    % del widget
                    '    %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
                    '    % elseif(CBD(i) >= bdSEL && CBD(i) <= bdBOT && round(INF_ruggine_bruna(i),1,'significant') >= 0.5 && round(INF_ruggine_bruna(i),1,'significant') < 0.8) % Non dà infezione
                    '    %     w_rb = 2;
                    '    %     disp(strcat('Infezione di ruggine bruna non andata a buon fine il', 32, num2str(dap)))
                    '    %     disp('GIALLO (RB)')
                    '    %     disp(strcat('Indice di infettività (Ruggine Bruna)', 32, num2str(INF_ruggine_bruna(i))))
                    '    % elseif(CBD(i) >= bdSEL && CBD(i) <= bdBOT && round(INF_ruggine_bruna(i),1,'significant') < 0.5) % Non dà infezione
                    '    %     w_rb = 1;
                    '    %     disp(strcat('Infezione di ruggine bruna non andata a buon fine il', 32, num2str(dap)))
                    '    %     disp('VERDE (RB)')
                    '    %     disp(strcat('Indice di infettività (Ruggine Bruna)', 32, num2str(INF_ruggine_bruna(i))))

                End If

            ElseIf dgg.cbd <= crop_data.bdBSG Then

                'secondo periodo [booting - beginning seed growth]
                If round_INF_ruggine_bruna >= 0.7 Then

                    evento.sintomi = True

                    'disp(strcat('[RUGGINE BRUNA] EVENTO INFETTIVO il giorno [booting - beginning seed growth]', 32, num2str(dap), 32, 'con probabile comparsa sintomi il ', 32, num2str(dap + LP)))
                    '[ date ] = from_DOY_to_date( dap, str2num(data_semina_day), str2num(data_semina_month), str2num(data_semina_year) );
                    '[ date ] = from_DOY_to_date( dap + LP, str2num(data_semina_day), str2num(data_semina_month), str2num(data_semina_year) );
                    'index_inf = index_inf + 1;

                    'RB_diamond(i) = 1.1;
                    'dap_inf(index_inf) = dap + LP;

                    'if(round(INF_ruggine_bruna(i),1,'significant') >= 0.8)
                    '    w_rb = 3;
                    '    disp(num2str(round(INF_ruggine_bruna(i),1,'significant')))
                    '    disp(strcat('[RUGGINE BRUNA] INDICE di infettività', 32, num2str(INF_ruggine_bruna(i)), 32, 'ROSSO'))
                    'else
                    '    w_rb = 2;
                    '    disp(num2str(round(INF_ruggine_bruna(i),1,'significant')))
                    '    disp(strcat('[RUGGINE BRUNA] INDICE di infettività', 32, num2str(INF_ruggine_bruna(i)), 32, 'GIALLO'))
                    'end

                    'disp(strcat('[RUGGINE BRUNA] LP:', 32, num2str(LP)))

                    '%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
                    '% Le parti commentate di seguito sono comunque utili per la costruzione
                    '% del widget
                    '%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
                    '% elseif(CBD(i) > bdBOT && CBD(i) <= bdBSG && round(INF_ruggine_bruna(i),1,'significant') < 0.7 && round(INF_ruggine_bruna(i),1,'significant') >= 0.5) % Non dà infezione
                    '%     w_rb = 2;
                    '%     disp(strcat('Infezione di ruggine bruna non andata a buon fine il', 32, num2str(dap)))
                    '%     disp('GIALLO (RB)')
                    '%     disp(strcat('Indice di infettività (Ruggine Bruna)', 32, num2str(INF_ruggine_bruna(i))))
                    '% elseif(CBD(i) > bdBOT && CBD(i) <= bdBSG && round(INF_ruggine_bruna(i),1,'significant') < 0.5) % Non dà infezione
                    '%     w_rb = 1;
                    '%     disp(strcat('Infezione di ruggine bruna non andata a buon fine il', 32, num2str(dap)))
                    '%     disp('VERDE (RB)')
                    '%     disp(strcat('Indice di infettività (Ruggine Bruna)', 32, num2str(INF_ruggine_bruna(i))))

                End If

            ElseIf dgg.cbd <= crop_data.bdTSG Then

                'terzo periodo [beginning seed growth - terminating seed growth]
                If round_INF_ruggine_bruna >= 0.8 Then

                    evento.sintomi = True

                    'disp(strcat('[RUGGINE BRUNA] EVENTO INFETTIVO il giorno [beginning seed growth - terminating seed growth]', 32, num2str(dap), 32, 'con probabile comparsa sintomi il ', 32, num2str(dap + LP)))
                    '[ date ] = from_DOY_to_date( dap, str2num(data_semina_day), str2num(data_semina_month), str2num(data_semina_year) );
                    '[ date ] = from_DOY_to_date( dap + LP, str2num(data_semina_day), str2num(data_semina_month), str2num(data_semina_year) );
                    'index_inf = index_inf + 1;
                    '% dap_inf(index_inf) = dap + IP;
                    'dap_inf(index_inf) = dap + LP;

                    'RB_diamond(i) = 1.1;
                    'w_rb = 3;
                    'disp(num2str(round(INF_ruggine_bruna(i),1,'significant')))
                    'disp(strcat('[RUGGINE BRUNA] INDICE di infettività', 32, num2str(INF_ruggine_bruna(i)), 32, 'ROSSO'))
                    'disp(strcat('[RUGGINE BRUNA] LP:', 32, num2str(LP)))

                    '%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
                    '% Le parti commentate di seguito sono comunque utili per la costruzione
                    '% del widget
                    '%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
                    '% elseif(CBD(i) > bdBSG && CBD(i) <= bdTSG && round(INF_ruggine_bruna(i),1,'significant') >= 0.5 && round(INF_ruggine_bruna(i),1,'significant') < 0.8) % Non dà infezione
                    '%     w_rb = 2;
                    '%     disp(strcat('Infezione di ruggine bruna non andata a buon fine il', 32, num2str(dap)))
                    '%     disp('GIALLO (RB)')
                    '%     disp(strcat('Indice di infettività (Ruggine Bruna)', 32, num2str(INF_ruggine_bruna(i))))
                    '% elseif(CBD(i) > bdBSG && CBD(i) <= bdTSG && round(INF_ruggine_bruna(i),1,'significant') < 0.5) % Non dà infezione
                    '%     w_rb = 1;
                    '%     disp(strcat('Infezione di ruggine bruna non andata a buon fine il', 32, num2str(dap)))
                    '%     disp('VERDE (RB)')
                    '%     disp(strcat('Indice di infettività (Ruggine Bruna)', 32, num2str(INF_ruggine_bruna(i))))

                End If
            End If
        End Sub

        Public Overrides Function generaOutput() As String

            Dim output As New OutputModello

            output.aggiungiColonna("Data", GetType(Date), "Data evento infettivo", "dd/MM/yyyy")
            output.aggiungiColonna("Latenza", GetType(Integer), "Periodo di latenza (gg)", "0.00")
            output.aggiungiColonna("DataSintomi", GetType(Date), "Data comparsa sintomi", "dd/MM/yyyy")
            output.aggiungiColonna("InfIndex", GetType(Decimal), "Indice di Infettività", "0.00")

            Dim indicator = output.aggiungiIndicatore("InfIndex_IND", {0.5, 0.8, Decimal.MaxValue}, OutputIndicator.enum_IndicatorType.Colors).setFieldVal("InfIndex")

            For Each ev In eventi
                output.AddField(ev.data)

                If ev.sintomi Then

                    output.AddField(ev.periodoDiLatenza)
                    output.AddField(ev.data.AddDays(ev.periodoDiLatenza))

                Else

                    output.AddField(DBNull.Value)
                    output.AddField(DBNull.Value)

                End If

                output.AddField(ev.indiceInf)
                output.AddField(indicator.colorForVal(ev.indiceInf))

                output.Commit()
            Next

            Return output.Output({indicator}.ToList())
        End Function

#If False Then

    Protected Overrides Function _elaboraIndicatore(ByVal risElab As RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione) As RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione

        If CalcolaPSA() Then

            risElab.Fill(_datiHH.Last().Risk_Index, 80, {20, 40})
        Else

            risElab.Errore(_errore)
        End If

        Return risElab
    End Function

#End If
    End Class


    Public Class Ruggine_Gialla_Dennis
        Inherits ModelloAvversita

        Private Class eventoInf
            Public data As Date
            Public dataComparsaSintomi As Date
            Public indiceInfettivita As Decimal
            Public risk As Decimal 'Per indicatore
        End Class

        Private ReadOnly eventiInf As List(Of eventoInf)

        Public Sub New(crop_data As crop, fenologia As ModelloFenologico)

            MyBase.New(crop_data, fenologia)

            'CS_RG = 0.9; % Coefficiente di suscettibilità varietale (0 - 1), in corso di validazione
            'INF_RG_Dennis = zeros(1, length_period); % Infection efficiency of uredospores (0-1)
            'Prob_Inf_RG_Dennis = zeros(1, length_period);
            'RG_diamond = NaN(1, length_period);
            'RISK_RG_DENNIS = zeros(1, length_period);

            eventiInf = New List(Of eventoInf)
        End Sub

        Public Overrides Sub elabora(dgg As datoGG)

            If dgg.cbd < crop_data.bdTIL Then
                _stato = Stato.PreFase
                Return
            End If

            If crop_data.bdBSG < dgg.cbd Then
                _stato = Stato.PostFase
                Return
            End If

            _stato = Stato.InFase

            'Calcolo giornaliero - RUGGINE GIALLA

            Dim INF_RG_Dennis_i As Decimal = 0

            If dgg.tmp <= 16 AndAlso dgg.lw > 3 Then

                Dim log_lw = Math.Log(dgg.lw)

                INF_RG_Dennis_i = (-355.14 + 0.99 * dgg.tmp + 1.64 * dgg.tmp ^ 2 - 0.11 * dgg.tmp ^ 3 + 292.99 * log_lw - 48.26 * log_lw ^ 2 - 1.55 * dgg.tmp * log_lw) / 100

            End If

            Dim CS_RG = 0.9 'Coefficiente di suscettibilità varietale (0 - 1), In corso di validazione
            Dim Prob_Inf_RG_Dennis_i As Decimal = Math.Min(1, Math.Max(0, INF_RG_Dennis_i * CS_RG))

            Dim LP_RG As Decimal = (1005 + 11.3 * dgg.tmp) / (2.5 + 5.65 * dgg.tmp) 'LP Is the time, expressed In days, that elapses between the arrival of uredospore and the eruption of uredia
            LP_RG = Math.Round(LP_RG)

            'Rischio ruggine gialla

            Dim RISK_RG_DENNIS_i As Decimal = INF_RG_Dennis_i

            If dgg.cbd < crop_data.bdBOT Then

                RISK_RG_DENNIS_i *= dgg.msnn / fenologia.get_L_NUMBER_SF9_round
            End If

            RISK_RG_DENNIS_i = Math.Min(1, Math.Max(0, RISK_RG_DENNIS_i))

            'Infezioni andate a buon fine
            If Prob_Inf_RG_Dennis_i >= 0.95 Then '% Dà infezione

                Dim ev As New eventoInf With {
                    .data = dgg.data,
                    .dataComparsaSintomi = dgg.data.AddDays(LP_RG),
                    .indiceInfettivita = Prob_Inf_RG_Dennis_i,
                    .risk = RISK_RG_DENNIS_i
                }

                eventiInf.Add(ev)

                'disp(strcat('[RUGGINE GIALLA] EVENTO INFETTIVO il giorno', 32, num2str(dap), 32, 'con PROBABILE COMPARSA SINTOMI il ', 32, num2str(dap + LP_RG)))
                '[ date ] = from_DOY_to_date( dap, str2num(data_semina_day), str2num(data_semina_month), str2num(data_semina_year));
                '[ date ] = from_DOY_to_date( dap + LP_RG, str2num(data_semina_day), str2num(data_semina_month), str2num(data_semina_year) );
                'index_inf = index_inf + 1;
                '% dap_inf(index_inf) = dap + IP;
                'dap_inf(index_inf) = dap + LP_RG;

                'RG_diamond(i) = 1.1;
                'disp(strcat('[RUGGINE GIALLA] INDICE di infettività', 32, num2str(Prob_Inf_RG_Dennis(i))))

                'If RISK_RG_DENNIS_i < 0.5 Then
                '    disp(strcat('[RUGGINE GIALLA] RISCHIO:', 32, num2str(RISK_RG_DENNIS(i)), 32, 'VERDE'))
                'ElseIf RISK_RG_DENNIS_i < 0.7 Then
                '    disp(strcat('[RUGGINE GIALLA] RISCHIO:', 32, num2str(RISK_RG_DENNIS(i)), 32, 'GIALLO'))
                'Else ' >= 0.7
                '    disp(strcat('[RUGGINE GIALLA] RISCHIO:', 32, num2str(RISK_RG_DENNIS(i)), 32, 'ROSSO'))
                'End If
            End If
        End Sub

        Public Overrides Function generaOutput() As String
            Throw New NotImplementedException()
        End Function

    End Class


    Public Class Fusariosi_Del_Ponte
        Inherits ModelloAvversita

        Private Class datoDelPonte
            Public data As Date
            Public ANText As Decimal    'Tasso di estrusione delle antere
            Public ANT As Decimal       'Proporzione di antere presenti ogni giorno
            Public ST As Decimal        'Tessuto suscettibile
            Public GZ As Decimal        'Fattore di inoculo
            Public INF As Decimal       'Infezioni calcolate su finestre di 2 giorni
            Public GIB4_cum As Decimal  'Rischio infettivo cumulato

            Public Calcolo_TOX_risk As Decimal  'Rischio micotossine
        End Class

        Private prev_dgg As datoGG

        Private X2 As Decimal   'sensibilità dell'areale di coltivazione dipande da LAT
        Private X3 As Decimal   'dato agronomico per rischio micotossine fusariosi
        Private X4 As Decimal   'suscettibilità varietale
        Private X5 As Decimal   'precessione colturale
        Private X6 As Decimal   'lavorazione del terreno
        Private inizio_discesa As Integer
        Private temp_st_1 As Decimal
        Private val_sat As Boolean
        Private inizio_rischio_FG As Integer
        Private mic_FG_cum As Decimal
        Private TOX_FG_CUM As Decimal
        Private Calcolo_TOX_risk_val_sat As Decimal
        Private GIB4_cum_vect_val_sat As Decimal

        Private ReadOnly datiFusariosi As List(Of datoDelPonte)

        Public Sub New(crop_data As crop, fenologia As ModelloFenologico, params As InputParams)

            MyBase.New(crop_data, fenologia)

            Dim coeff_acc As Decimal    'Coefficiente di accestimento per il calcolo del numero di culmi

            'Risk of the growing area
            If params.latitude < 41.5 Then

                'SUD Italia
                X2 = 0.519

            Else

                If params.latitude < 44 Then

                    'CENTRO Italia
                    X2 = 1.038

                Else 'params.latitude >= 44

                    'NORD Italia
                    X2 = 1.557
                End If
            End If

            If params.varieta = 1 Then

                'Grano Tenero
                coeff_acc = 1.2
                X3 = 1.31

            Else

                coeff_acc = 1.4
                X3 = 1.965

            End If

            Select Case params.cultivar
                Case 1
                    X4 = 0.342
                Case 2
                    X4 = 0.684
                Case 3
                    X4 = 1.026
                Case 4
                    X4 = 1.368
                Case Else
                    Throw New NotImplementedException("Parametro cultivar non valido ")
            End Select

            Select Case params.colturaPrec
                Case 1
                    X5 = 0.272
                Case 2
                    X5 = 0.544
                Case 3
                    X5 = 0.816
                Case Else
                    Throw New NotImplementedException("Parametro colturaPrec non valido ")
            End Select

            Select Case params.lavTerreno
                Case 1
                    X6 = 0.25
                Case 2
                    X6 = 0.686
                Case 3
                    X6 = 1.029
                Case Else
                    Throw New NotImplementedException("Parametro lavTerreno non valido ")
            End Select


            datiFusariosi = New List(Of datoDelPonte)

            'temp1_del_ponte = 0;
            'temp2_del_ponte = 0;
            temp_st_1 = 0
            inizio_discesa = 0
            'GIB1_cum = 0;
            'GIB2_cum = 0;
            'GIB3_cum = 0;
            'INF_del_ponte_cum = 0;
            'INF_del_ponte = zeros(1, length_period);
            'GIB4_cum_vect = zeros(1, length_period);
            val_sat = False
            'fg_cum = 0;
            TOX_FG_CUM = 0
            'TOX_FG_CUM_vect = zeros(1, length_period);
            mic_FG_cum = 0
            'mic_FG_cum_vect = zeros(1, length_period);
            inizio_rischio_FG = 0
            Calcolo_TOX_risk_val_sat = 0
            'Calcolo_TOX_risk = (-6.915 + 0.5 + X2 + X3 + X4 + X5 + X6) * ones(1, length_period);

            prev_dgg = Nothing
        End Sub

        Public Overrides Sub elabora(dgg As datoGG)

            _stato = Stato.InFase

            If crop_data.bdEAR < dgg.cbd Then 'INIZIO MODELLO DEL PONTE
                _stato = Stato.PreFase
            End If

            If temp_st_1 > 10 Then 'FINE MODELLO DEL PONTE
                _stato = Stato.PostFase
            End If

            If _stato = Stato.InFase Then
                _elabora(dgg)
            End If

            If temp_st_1 > 10 Then

                If Not val_sat Then 'valore in saturazione (fine stagione)

                    Calcolo_TOX_risk_val_sat = datiFusariosi.Last.Calcolo_TOX_risk
                    GIB4_cum_vect_val_sat = datiFusariosi.Last.GIB4_cum
                    val_sat = True

                    _rischio_sigmoide(datiFusariosi.Last)

                Else

                    'datiFusariosi.Last.Calcolo_TOX_risk = Calcolo_TOX_risk_val_sat
                    'datiFusariosi.Last.GIB4_cum = GIB4_cum_vect_val_sat;
                End If
            End If

            prev_dgg = dgg
        End Sub

        Private Sub _elabora(dgg As datoGG)

            'Daily rate of cumulative proportion of extruded anthers
            'a_del_ponte(i) = 0.255 - 0.029 * TMP(i) + 0.0009 * TMP(i) ^ 2;
            'b_del_ponte(i) = - 5.773 + 0.966 * TMP(i) - 0.0278 * TMP(i) ^ 2;

            Dim a_del_ponte As Decimal = 0.026
            Dim b_del_ponte As Decimal = 2 'con 2 al posto di 2.4261 consideriamo un periodo di tempo più lungo 
            '(e, conseguentemente, un maggior numero di infezioni) anche se è stato ridotto il numero 
            'di giorni dopo il raggiungimento della soglia (10 non più 14, vedi di seguito)

            'Simulazione T = 15.25
            'a_del_ponte = 0.0260;
            'b_del_ponte = 2.4261;

            'Simulazione T = 13.25
            'a_del_ponte = 0.0285;
            'b_del_ponte = 2.1622;

            Dim giorno_del_ponte As Decimal = datiFusariosi.Count + 1

            Dim ddp As New datoDelPonte With {
                .data = dgg.data,
                .ANText = 1D - Math.Exp(-a_del_ponte * giorno_del_ponte ^ b_del_ponte),
                .ANT = .ANText,
                .ST = .ANT,
                .INF = 0,
                .GZ = 0,
                .GIB4_cum = 0,
                .Calcolo_TOX_risk = 0
            }

            Dim prev_ddp As datoDelPonte = Nothing

            If datiFusariosi.Count > 0 Then
                prev_ddp = datiFusariosi(datiFusariosi.Count - 1)
                ddp.GIB4_cum = prev_ddp.GIB4_cum
            End If

            datiFusariosi.Add(ddp)

            If datiFusariosi.Count >= 5 Then ' giorno_del_ponte >= 5 Then

                ddp.ANT = ddp.ANText - datiFusariosi(datiFusariosi.Count - 5).ANText

                If ddp.ANT < 0.5 AndAlso dgg.cbd <= crop_data.bdANT AndAlso ddp.ANT < prev_ddp.ANT Then
                    ddp.ANT = 0.5
                End If

                'Susceptible tissue (ST)

                If ddp.ANT < 0.25 AndAlso ddp.ANT < prev_ddp.ANT AndAlso inizio_discesa = 0 Then
                    'picco non ancora raggiunto nonostante si stia scendendo
                    inizio_discesa = 1
                End If

                If inizio_discesa = 0 Then

                    'Salita
                    ddp.ST = ddp.ANT

                Else
                    'inizio_discesa = 1

                    If 0.01 <= ddp.ANT AndAlso ddp.ANT <= 0.25 AndAlso temp_st_1 = 0 Then

                        'Discesa
                        ddp.ST = 0.25

                    ElseIf prev_ddp.ST = 0.25 AndAlso 0.01 <= ddp.ANT AndAlso temp_st_1 = 0 Then

                        'In caso di ANT(i) > 0.25
                        ddp.ST = 0.25

                    ElseIf ddp.ANT < 0.01 AndAlso temp_st_1 = 0 Then

                        temp_st_1 += 1
                        ddp.ST = 0.25

                    ElseIf 1 <= temp_st_1 AndAlso temp_st_1 <= 7 Then

                        temp_st_1 += 1
                        ddp.ST = 0.25

                    ElseIf 7 < temp_st_1 AndAlso temp_st_1 <= 10 Then

                        'ridotto a 10 (non più 14 giorni) per compensare il maggior numero di giorni risultante dall'ANText (con b = 2)
                        temp_st_1 += 1
                        ddp.ST = 0.1

                    End If
                End If
            End If

            'Inocolum factor

            Dim CRD As Decimal = 0
            '**************************************************************************************
            '************************  CHIEDERE NON PUO' MAI RANGGIUNGERE L'ElseIf
            '**************************************************************************************
            'If (RAIN(i - 1) > 0) Then
            '    CRD = 1;
            'ElseIf (RAIN(i - 1) > 0 && RAIN(i - 2) > 0) Then
            '    CRD = 2;
            'ElseIf (RAIN(i - 1) > 0 && RAIN(i - 2) > 0 && RAIN(i - 3) > 0) Then
            '    CRD = 2.5;
            'ElseIf (RAIN(i - 1) > 0 && RAIN(i - 2) > 0 && RAIN(i - 3) > 0 && RAIN(i - 4) > 0) Then
            '    CRD = 0.3;
            'End If
            '**************************************************************************************
            '**************************************************************************************
            '**************************************************************************************

            ddp.GZ = (-0.6306 + 0.0152 * dgg.rh + 0.1076 * CRD) ^ 2

            'Environmental factor
            'INF_del_ponte: proportion of susceptible tisue likely to be infected at anytime

            'prev_dgg IsNot Nothing ?????????

            If dgg.rain > 0.3 AndAlso prev_dgg.rain > 0.3 AndAlso (dgg.rh + prev_dgg.rh) / 2 >= 78 Then

                ddp.INF = 0.001029 * Math.Exp(0.1957 * ((dgg.tmp + prev_dgg.tmp) / 2)) * 10

            ElseIf dgg.rain > 0.3 AndAlso dgg.rh >= 80 AndAlso prev_dgg.rh >= 85 Then

                ddp.INF = 0.001029 * Math.Exp(0.1957 * ((dgg.tmp + prev_dgg.tmp) / 2)) * 10

            ElseIf prev_dgg.rain > 0.3 AndAlso prev_dgg.rh >= 80 AndAlso dgg.rh >= 85 Then

                ddp.INF = 0.001029 * Math.Exp(0.1957 * ((dgg.tmp + prev_dgg.tmp) / 2)) * 10

            End If


            'prev_ggDelPonte IsNot Nothing ??????????

            'Daily and accumulated infection index
            '% GIB1(i) = ((ANT(i) + ANT(i - 1)) / 2) * INF_del_ponte(i);
            '% GIB2(i) = ((ANT(i) + ANT(i - 1)) / 2) * INF_del_ponte(i) * GZ_del_ponte(i);
            '% GIB3(i) = ((ST(i) + ST(i - 1)) / 2) * INF_del_ponte(i);
            Dim GIB4_i As Decimal = ((ddp.ST + prev_ddp.ST) / 2) * ddp.INF * ddp.GZ

            '% GIB%
            '% GIB1_cum = GIB1_cum + GIB1(i) * 100;
            '% GIB2_cum = GIB2_cum + GIB2(i) * 100;
            '% GIB3_cum = GIB3_cum + GIB3(i) * 100;
            ddp.GIB4_cum += GIB4_i * 100

            '% GIB1_cum_vect(i) = GIB1_cum;
            '% GIB2_cum_vect(i) = GIB2_cum;
            '% GIB3_cum_vect(i) = GIB3_cum;
            'GIB4_cum_vect(i) = GIB4_cum;

            'Tasso di produzione del Micelio - INV (Rossi et al.)

            Dim mic_FG_i As Decimal = Math.Min(1, (5.53 * (dgg.tmp / 38) ^ 1.55 * (1 - dgg.tmp / 38)) ^ 1.35)

            If GIB4_i > 0 AndAlso inizio_rischio_FG = 0 Then '% 0.1

                inizio_rischio_FG = 1

            End If

            If inizio_rischio_FG = 1 Then

                mic_FG_cum += mic_FG_i
                'mic_FG_cum_vect(i) = mic_FG_cum;
            End If

            Dim TOX_FG_i As Decimal = ddp.GIB4_cum * mic_FG_cum / 100  'GIB4_cum_vect(i) * mic_FG_cum_vect(i) / 100;%
            TOX_FG_CUM += TOX_FG_i
            'TOX_FG_CUM_vect(i) = TOX_FG_CUM;

            Dim X1 As Decimal
            'Calcolo TOX RISK (rischio micotossine associato a fattori agronomici)
            If TOX_FG_CUM <= 5 Then
                X1 = 0.921
            Else
                If TOX_FG_CUM <= 10 Then
                    X1 = 1.842
                Else
                    If TOX_FG_CUM <= 18 Then
                        X1 = 2.763
                    Else
                        If TOX_FG_CUM <= 25 Then
                            X1 = 3.684
                        Else
                            X1 = 4.605
                        End If
                    End If
                End If
            End If

            ddp.Calcolo_TOX_risk = -6.915 + X1 + X2 + X3 + X4 + X5 + X6
            '% if(Calcolo_TOX_risk(i) <= -2)
            '%     disp('Rischio molto basso')
            '%     disp('Analisi DON: Non necessaria')
            '% elseif(Calcolo_TOX_risk(i) > -2 && Calcolo_TOX_risk(i) <= 0) % -0.44
            '%     disp('Rischio basso')
            '%     disp('Analisi DON: Random')
            '% elseif(Calcolo_TOX_risk(i) > 0 && Calcolo_TOX_risk(i) <= 1.4) % -0.44 1.2
            '%     disp('Rischio intermedio')
            '%     disp('Analisi DON: Random')
            '% elseif(Calcolo_TOX_risk(i) > 1.4 && Calcolo_TOX_risk(i) <= 3)
            '%     disp('Rischio alto')
            '%     disp('Analisi DON: Sistematica')
            '% elseif(Calcolo_TOX_risk(i) > 3)
            '%     disp('Rischio molto alto')
            '%     disp('Analisi DON: Sistematica')
            '% end

        End Sub

        Private Sub _rischio_sigmoide(ddp As datoDelPonte)

            Dim valore_rischio As Decimal = Math.Round(ddp.Calcolo_TOX_risk * 1000) / 1000
            Dim tol As Decimal = 0.00000001

#If False Then

x_Risk = [-6:0.001:6]; Sig_prob = 1 - (1 ./ (1+exp(-2.2*(x_Risk + 0.44))));
x_Risk_2 = x_Risk + 1; Sig_prob_2 = 1 - (1 ./ (1+exp(-1.6*(x_Risk_2-1.2))));

% if(fig_sigmoide == 1)
asse_y = [0:.01:1];
x1 = -2 * ones(length(asse_y));
x2 = -0.44 * ones(length(asse_y));
x3 = 1.2 * ones(length(asse_y));
figure, plot(x_Risk, Sig_prob), xlim([-4,4]), hold on, plot(x_Risk_2, Sig_prob_2, 'r'),
plot(x1, asse_y, 'g'), plot(x2, asse_y, 'g'), plot(x3, asse_y, 'g'),
xlabel('Risk (R)'), ylabel('Probability')
txt = 'Assenza di DON';
text(-3.5,0.7,txt)
txt = 'DON<=500 ppb';
text(0.3,0.2,txt)
txt = 'DON>500 ppb';
text(1.8,0.5,txt)
% end

ind_rischio_tol = (abs(x_Risk - valore_rischio)) < tol;
ind_rischio_tol2 = (abs(x_Risk_2 - valore_rischio)) < tol;
plot(valore_rischio, Sig_prob(ind_rischio_tol), 'b*')
plot(valore_rischio, Sig_prob_2(ind_rischio_tol2), 'r*')


prima_prob = round(Sig_prob((abs(x_Risk - valore_rischio) < tol)) * 100, 1);
seconda_prob = round(Sig_prob_2((abs(x_Risk_2 - valore_rischio) < tol)) * 100, 1);
terza_prob = 100 - seconda_prob;

prob_DON = [prima_prob seconda_prob terza_prob];
[~, ind_prob_DON] = max(prob_DON);
if(ind_prob_DON == 2)
    disp(strcat('Probabilità DON <= 500 ppb:', 32, num2str(seconda_prob), '%'))
    if(prima_prob > 50)
        % disp(strcat('Probabilità assenza di DON:', 32, num2str(prima_prob), '%'))
        disp('Consigliata analisi randomica del DON')
    else
        disp('Necessaria analisi randomica del DON')
    end
elseif(ind_prob_DON == 3)
    disp(strcat('Probabilità DON > 500 ppb:', 32, num2str(terza_prob), '%'))
    disp('Necessaria analisi sistematica del DON')
end

#End If
        End Sub

        Public Overrides Function generaOutput() As String

            Dim output As New OutputModello

            output.aggiungiColonna("Data", GetType(Date), "Data evento infettivo", "dd/MM/yyyy")
            output.aggiungiColonna("INF", GetType(Decimal), "Infezione", "0.00")
            output.aggiungiColonna("GIB4_cum", GetType(Decimal), "Rischio infettivo", "0.00")
            output.aggiungiColonna("TOX_Risk", GetType(Decimal), "Rischio di micotossine", "0.00")

            Dim ind_GIB4_cum = output.aggiungiIndicatore("GIB4_cum_IND", {8, 13, 20}, OutputIndicator.enum_IndicatorType.Colors).setFieldVal("GIB4_cum")
            Dim ind_TOX_Risk = output.aggiungiIndicatore("TOX_Risk_IND", {0, 1.4, 4}, OutputIndicator.enum_IndicatorType.Colors).setFieldVal("TOX_Risk")

            For Each ddp In datiFusariosi

                'Informazioni per la tabella

                If ddp.INF > 0 Then
                    '    disp(strcat('[FUSARIOSI] EVENTO INFETTIVO il giorno', 32, num2str(dap)))
                    '    [ date ] = from_DOY_to_date( dap, str2num(data_semina_day), str2num(data_semina_month), str2num(data_semina_year) );
                    '    if(GIB4_cum_vect(i) < 8)
                    '        disp(strcat('[FUSARIOSI] RISCHIO infettivo (VERDE):', 32, num2str(GIB4_cum_vect(i))))
                    '    elseif(GIB4_cum_vect(i) >= 8 && GIB4_cum_vect(i) < 13)
                    '        disp(strcat('[FUSARIOSI] RISCHIO infettivo (GIALLO):', 32, num2str(GIB4_cum_vect(i))))
                    '    elseif(GIB4_cum_vect(i) >= 13)
                    '        disp(strcat('[FUSARIOSI] RISCHIO infettivo (ROSSO):', 32, num2str(GIB4_cum_vect(i))))
                    '    end

                    '    if(Calcolo_TOX_risk(i) < 0)
                    '        disp(strcat('[FUSARIOSI] RISCHIO di micotossine (VERDE):', 32, num2str(Calcolo_TOX_risk(i))))
                    '    elseif(Calcolo_TOX_risk(i) >= 0 && Calcolo_TOX_risk(i) < 1.4)
                    '        disp(strcat('[FUSARIOSI] RISCHIO di micotossine (GIALLO):', 32, num2str(Calcolo_TOX_risk(i))))
                    '    elseif(Calcolo_TOX_risk(i) >= 1.4)
                    '        disp(strcat('[FUSARIOSI] RISCHIO di micotossine (ROSSO):', 32, num2str(Calcolo_TOX_risk(i))))
                    '    end
                End If

                output.Commit()
            Next

            Return output.Output({ind_GIB4_cum, ind_TOX_Risk}.ToList())
        End Function
    End Class


    Public Class Oidio_Audsley
        Inherits ModelloAvversita

        Private Class mean_RH
            Private q As Queue(Of Decimal)
            Private c As Integer
            Public Sub New(_c As Integer)
                q = New Queue(Of Decimal)
                c = _c
            End Sub
            Public Function mean(rh As Decimal) As Decimal
                q.Enqueue(rh)
                If q.Count > c Then
                    q.Dequeue()
                End If
                Return q.Sum() / q.Count()
            End Function
        End Class

        Private Class datoOidio
            Public data As Date
            Public _y_oidio As Decimal
            Public U_oidio As Decimal
            Public Y_oidio As Decimal
            Public S_oidio As Decimal
            Public F_oidio As Decimal
            Public Lv_oidio As Decimal
            Public Lc_oidio As Decimal
            Public Rtemp_oidio As Decimal
            Public Rrain_oidio As Decimal
            Public Rrh_oidio As Decimal
        End Class

        Private prev_dgg As datoGG

        Private ReadOnly datiOidio As List(Of datoOidio)
        Private prev_oidio As datoOidio

        Private ReadOnly k_oidio As Decimal
        Private ReadOnly mean_RH_5gg As mean_RH
        'Private TMP_cum As Decimal

        Public Sub New(crop_data As crop, fenologia As ModelloFenologico)

            MyBase.New(crop_data, fenologia)

            prev_dgg = Nothing
            datiOidio = New List(Of datoOidio)
            prev_oidio = Nothing

            'TMP_cum = 0
            'TMP_cum_vect = zeros(1, length_period);
            k_oidio = 0.04
            'a_oidio = 7.15;
            'y_oidio = a_oidio * ones(1, length_period);
            'U_oidio = zeros(1, length_period);
            'F_oidio = zeros(1, length_period);
            '% I_oidio = zeros(length_period); % matrix
            'I_oidio = zeros(1, length_period); % solitamente non più di 10 foglie
            'Y_oidio = zeros(1, length_period);
            'Lv_oidio = zeros(1, length_period);
            'Lc_oidio = zeros(1, length_period);
            'Rcr_oidio = 0.8; % tolerant - resistant
            'Rtemp_oidio = zeros(1, length_period);
            'Rrain_oidio = zeros(1, length_period);
            'mean_RH_5gg = zeros(1, length_period);
            'Rrh_oidio = zeros(1, length_period);
            'Rage_oidio = zeros(1, length_period);
            'RN_oidio = zeros(1, length_period);
            'num_I_oidio = 0;
            'num_sintomi_oidio_inf = 1;
            'num_sintomi_oidio_sup = 1;
            'LP_oidio_vect_sup = zeros(1, length_period);
            'LP_oidio_vect_inf = zeros(1, length_period);

            mean_RH_5gg = New mean_RH(5)

        End Sub

        Public Overrides Sub elabora(dgg As datoGG)

            Dim d_oidio As New datoOidio With {
                .data = dgg.data
            }
            datiOidio.Add(d_oidio)

            If prev_dgg Is Nothing Then

                prev_dgg = dgg
                prev_oidio = d_oidio

                Return
            End If

            Dim leaf_oidio As Integer = Math.Round(dgg.msnn)
            'If dgg.tmp >= 0 Then
            '    TMP_cum += dgg.tmp
            '    TMP_cum_vect(i) = TMP_cum;
            'Else
            '    TMP_cum_vect(i) = TMP_cum;
            'End If

            If Math.Round(fenologia.get_CUMVER) < 50 OrElse dgg.cbd < crop_data.bdEMR Then
                _stato = Stato.PreFase
                Return
            End If

            Dim L_NUMBER_SF9_round_finale As Decimal = fenologia.get_L_NUMBER_SF9_round

            If dgg.cbd >= crop_data.bdBOT Then

                L_NUMBER_SF9_round_finale = Math.Round(dgg.msnn)
            End If

            If leaf_oidio < L_NUMBER_SF9_round_finale - 5 OrElse crop_data.bdTSG < dgg.cbd Then 'bdPM
                _stato = Stato.PostFase
                Return
            End If

            _stato = Stato.InFase

            'Lesion size (Logistic function)
            d_oidio._y_oidio = prev_oidio._y_oidio * (1 + k_oidio) - k_oidio * prev_oidio._y_oidio ^ 2

            'Potential infections
            d_oidio.U_oidio = compute_U(leaf_oidio, d_oidio.Y_oidio)

            'Self-infection term
            d_oidio.S_oidio = 1.1 * d_oidio.U_oidio

            'Number of infections reaching the target leaf l on day i
            Dim SUM_oidio As Decimal = 0
            For f = leaf_oidio + 1 To fenologia.get_L_NUMBER_SF9_round
                SUM_oidio += compute_U(f, d_oidio.Y_oidio)
            Next
            'for f = (leaf_oidio + 1) : L_NUMBER_SF9_round % 6 is the maximum number of diseased leaves in the model
            '    SUM_oidio = SUM_oidio + compute_U(i, f, Y_oidio, L_NUMBER_SF9_round);
            'end
            d_oidio.F_oidio = SUM_oidio + d_oidio.S_oidio

            'Number of successful infection events (I_oidio)
            'Attenuation factors (Lv_oidio * Lc_oidio * Rcr_oidio * Rtemp_oidio * Rrain_oidio * Rrh_oidio * RN_oidio)

            'Lv_oidio
            d_oidio.Lv_oidio = leaf_oidio / fenologia.get_L_NUMBER_SF9_round

            'Lc_oidio
            d_oidio.Lc_oidio = (fenologia.get_L_NUMBER_SF9_round - leaf_oidio) / 6

            'Rtemp_oidio
            Dim Delta_TMP As Decimal = dgg.tmp - prev_dgg.tmp
            If Delta_TMP >= 2 AndAlso Delta_TMP <= 25 Then
                d_oidio.Rtemp_oidio = 0.0005 * ((Delta_TMP * 2) ^ 2) * (25 - Delta_TMP)
            Else
                d_oidio.Rtemp_oidio = 0
            End If

            'Rrain_oidio
            If dgg.rain < 5 Then
                d_oidio.Rrain_oidio = ((0.75 + 0.05 * dgg.rain) * 0.9 / (1 + 0.001 * Math.Exp(2 * (dgg.rain - 5)))) + 0.1
            Else
                d_oidio.Rrain_oidio = (0.9 / (1 + 0.001 * Math.Exp(2 * (dgg.rain - 5)))) + 0.1
            End If

            Dim _mean_RH_5gg = mean_RH_5gg.mean(dgg.rh)

            'Rrh_oidio
            d_oidio.Rrh_oidio = -0.01 * _mean_RH_5gg ^ 2 + 1.2 * _mean_RH_5gg + 35
            d_oidio.Rrh_oidio = Math.Max(0, d_oidio.Rrh_oidio / 10D)

            'Rage_oidio
            'ind_leaf_dt = find(round(MSNN) == leaf_oidio);
            'leaf_dt = sum(TMP_cum_vect(ind_leaf_dt));
            'Rage_oidio(i) = (1 - 0.7)/(1 + 0.001 * exp(0.2 * leaf_dt)) + 0.7;

            '% RN_oidio
            'totN = sum(NFERTI(:)) * 10; % total amount of nitrogen [kgN/ha]
            'RN_oidio(i) = 1 / (1 + exp(-(0.1 * totN) + 6));

            '%I_oidio(i, leaf_oidio) = Lv_oidio(i) * Lc_oidio(i) * Rcr_oidio * Rtemp_oidio(i) * Rrain_oidio(i) * Rrh_oidio(i) * Rage_oidio(i) * RN_oidio(i) * F_oidio(i);
            'I_oidio(i) = Lv_oidio(i) * Lc_oidio(i) * Rcr_oidio * Rtemp_oidio(i) * Rrain_oidio(i) * Rrh_oidio(i) * Rage_oidio(i) * RN_oidio(i) * F_oidio(i); % leaf_oidio
            'temp_I_oidio = I_oidio(i);


            'prod_oidio = y_oidio' .* I_oidio(:);

            'Y_oidio(i + 1) = sum(prod_oidio);
            '%temp_Y_oidio = Y_oidio(i + 1, leaf_oidio);

            '% Informazioni da riportare in tabella
            'if(I_oidio(i) > 0)
            '    num_I_oidio = num_I_oidio + 1;
            '    disp(strcat('[OIDIO] EVENTO INFETTIVO numero', 32, num2str(num_I_oidio), 'il giorno:', 32, num2str(dap)))
            '    [ date ] = from_DOY_to_date( dap, str2num(data_semina_day), str2num(data_semina_month), str2num(data_semina_year) ); 
            '    disp(strcat('[OIDIO] NUMERO DI INFEZIONI pari a', 32, num2str(ceil(I_oidio(i))), 32, '(evento infettivo numero', 32, num2str(num_I_oidio), ')'))
            '    disp(strcat('[OIDIO] Severità', 32, num2str(ceil(round(Y_oidio(i + 1), 1)))))
            'end

            'if(num_I_oidio > 0)
            '    LP_oidio_vect_inf(num_sintomi_oidio_inf : num_I_oidio) = LP_oidio_vect_inf(num_sintomi_oidio_inf : num_I_oidio) + TMP(i);
            '    while(sum(LP_oidio_vect_inf(num_sintomi_oidio_inf : num_I_oidio) >= 105) >= 1)
            '        disp(strcat('[OIDIO] INIZIO sintomi (evento infettivo numero', 32, num2str(num_sintomi_oidio_inf), ')', 32, num2str(dap)))
            '        [ date ] = from_DOY_to_date( dap, str2num(data_semina_day), str2num(data_semina_month), str2num(data_semina_year) );        
            '        num_sintomi_oidio_inf = num_sintomi_oidio_inf + 1;
            '    end
            'end

            'if(num_I_oidio > 0)
            '    LP_oidio_vect_sup(num_sintomi_oidio_sup : num_I_oidio) = LP_oidio_vect_sup(num_sintomi_oidio_sup : num_I_oidio) + TMP(i);
            '    while(sum(LP_oidio_vect_sup(num_sintomi_oidio_sup : num_I_oidio) >= 234) >= 1)
            '        disp(strcat('[OIDIO] FINE sintomi (evento infettivo numero', 32, num2str(num_sintomi_oidio_sup), ')', 32, num2str(dap)))
            '        [ date ] = from_DOY_to_date( dap, str2num(data_semina_day), str2num(data_semina_month), str2num(data_semina_year) );        
            '        num_sintomi_oidio_sup = num_sintomi_oidio_sup + 1;
            '    end
            'end

        End Sub

        Private Function compute_U(leaf_oidio As Integer, Y_oidio As Decimal) As Decimal

            Dim num_leaf As Decimal = 0
            If leaf_oidio = fenologia.get_L_NUMBER_SF9_round - 5 Then
                num_leaf = 1
            End If

            Return 1 - Math.Exp(-2 * (Y_oidio + 0.1 * num_leaf))
        End Function


        Public Overrides Function generaOutput() As String
            Throw New NotImplementedException()
        End Function
    End Class


    Public Class CondizioniSeptoria

        Private Class meteoSeptoria
            Public dataOra As DateTime
            Public rain As Decimal
            Public rh As Decimal
            Public temp As Decimal

            Public Function GreaterEqual(other As meteoSeptoria) As Boolean
                Return rain >= other.rain AndAlso rh >= other.rh AndAlso temp >= other.temp
            End Function
        End Class

        Private ReadOnly meteo As List(Of meteoSeptoria)
        Private ReadOnly pattern_innesco() As meteoSeptoria
        Private needClean As Boolean

        Public Sub New()
            meteo = New List(Of meteoSeptoria)
            needClean = True

            pattern_innesco = {
                New meteoSeptoria With {.dataOra = Date.MinValue, .rain = 0.1, .rh = -99, .temp = -99},
                New meteoSeptoria With {.dataOra = Date.MinValue, .rain = 0.5, .rh = -99, .temp = -99},
                New meteoSeptoria With {.dataOra = Date.MinValue, .rain = -99, .rh = 60, .temp = 4},
                New meteoSeptoria With {.dataOra = Date.MinValue, .rain = -99, .rh = 60, .temp = 4},
                New meteoSeptoria With {.dataOra = Date.MinValue, .rain = -99, .rh = 60, .temp = 4},
                New meteoSeptoria With {.dataOra = Date.MinValue, .rain = -99, .rh = 60, .temp = 4},
                New meteoSeptoria With {.dataOra = Date.MinValue, .rain = -99, .rh = 60, .temp = 4},
                New meteoSeptoria With {.dataOra = Date.MinValue, .rain = -99, .rh = 60, .temp = 4},
                New meteoSeptoria With {.dataOra = Date.MinValue, .rain = -99, .rh = 60, .temp = 4},
                New meteoSeptoria With {.dataOra = Date.MinValue, .rain = -99, .rh = 60, .temp = 4},
                New meteoSeptoria With {.dataOra = Date.MinValue, .rain = -99, .rh = 60, .temp = 4},
                New meteoSeptoria With {.dataOra = Date.MinValue, .rain = -99, .rh = 60, .temp = 4},
                New meteoSeptoria With {.dataOra = Date.MinValue, .rain = -99, .rh = 60, .temp = 4},
                New meteoSeptoria With {.dataOra = Date.MinValue, .rain = -99, .rh = 60, .temp = 4},
                New meteoSeptoria With {.dataOra = Date.MinValue, .rain = -99, .rh = 60, .temp = 4},
                New meteoSeptoria With {.dataOra = Date.MinValue, .rain = -99, .rh = 60, .temp = 4},
                New meteoSeptoria With {.dataOra = Date.MinValue, .rain = -99, .rh = 60, .temp = 4},
                New meteoSeptoria With {.dataOra = Date.MinValue, .rain = -99, .rh = 60, .temp = 4},
                New meteoSeptoria With {.dataOra = Date.MinValue, .rain = -99, .rh = -99, .temp = 4},
                New meteoSeptoria With {.dataOra = Date.MinValue, .rain = -99, .rh = -99, .temp = 4},
                New meteoSeptoria With {.dataOra = Date.MinValue, .rain = -99, .rh = -99, .temp = 4},
                New meteoSeptoria With {.dataOra = Date.MinValue, .rain = -99, .rh = -99, .temp = 4},
                New meteoSeptoria With {.dataOra = Date.MinValue, .rain = -99, .rh = -99, .temp = 4},
                New meteoSeptoria With {.dataOra = Date.MinValue, .rain = -99, .rh = -99, .temp = 4},
                New meteoSeptoria With {.dataOra = Date.MinValue, .rain = -99, .rh = -99, .temp = 4},
                New meteoSeptoria With {.dataOra = Date.MinValue, .rain = -99, .rh = -99, .temp = 4}
            }

        End Sub

        Public Sub add(dm As MeteoDSSItem)

            meteo.Add(New meteoSeptoria With {
                      .dataOra = dm.DataOra,
                      .rain = dm.Prec,
                      .rh = dm.UmRel,
                      .temp = dm.Temp
                      })
        End Sub

        Public Class InfSeptoria
            Public data As Date
            Public possibili_infezioni As Integer
        End Class

        Private Sub aggiornaListaInfezioni(lista As List(Of InfSeptoria), data As Date)

            Dim found As Boolean = False
            Dim idx As Integer = 0

            While Not found AndAlso idx < lista.Count

                If lista(idx).data = data Then

                    lista(idx).possibili_infezioni += 1
                    found = True
                End If
            End While

            If Not found Then

                lista.Add(New InfSeptoria With {.data = data, .possibili_infezioni = 1})
            End If
        End Sub

        Public Function possibiliInfezioni(dt As Date) As List(Of InfSeptoria)

            If needClean Then

                While meteo.Any AndAlso meteo.First.dataOra < dt
                    meteo.RemoveAt(0)
                End While

                needClean = False
            End If

            Dim resultList As New List(Of InfSeptoria)

            While meteo.Count >= pattern_innesco.Length

                Dim cond As Boolean = True
                Dim idx As Integer = 0

                While cond AndAlso idx < pattern_innesco.Length

                    cond &= meteo(idx).GreaterEqual(pattern_innesco(idx))

                    idx += 1
                End While

                If cond Then

                    aggiornaListaInfezioni(resultList, meteo.First.dataOra.Date)
                End If

                meteo.RemoveAt(0)
            End While

            Return resultList





            'Dim result As Boolean = False

            'Dim tryNext As Boolean = True

            'Dim save_meteo As New Queue(Of meteoSeptoria)
            'Dim curr As meteoSeptoria

            'While tryNext

            '    Dim innesco As Boolean = False

            '    'Cerco una pioggia >= 0.1 mm 
            '    While Not innesco And meteo.Any

            '        curr = meteo.Dequeue()

            '        If curr.rain >= 0.1 Then

            '            save_meteo.Enqueue(curr)
            '            innesco = True
            '        End If
            '    End While

            '    If Not innesco Then

            '        'Queue vuota...
            '        tryNext = False
            '    Else

            '        If meteo.Peek.rain >= 0.5 Then

            '            'Seconda pioggia >= 0.5 mm
            '            save_meteo.Enqueue(meteo.Dequeue())

            '            'Sequenza di 16 ore consecutive com rh >= 60 e 24 ore con temp >= 4
            '            Dim cnt As Integer = 0

            '            While innesco AndAlso cnt < 24 AndAlso meteo.Any

            '                curr = meteo.Peek

            '                innesco = (cnt >= 16 OrElse curr.rh >= 60) AndAlso curr.temp >= 4

            '                If innesco Then

            '                    save_meteo.Enqueue(curr)

            '                    cnt += 1
            '                End If
            '            End While

            '            If innesco Then

            '                If cnt < 24 Then

            '                    'Non ho abbastanza dati a disposizione per verificare le condizioni

            '                    'Queue vuota...
            '                    tryNext = False

            '                    'La prossima volta dovrò ripartire da qui
            '                    While save_meteo.Any
            '                        meteo.Enqueue(save_meteo.Dequeue())
            '                    End While

            '                Else

            '                    tryNext = False
            '                    result = True
            '                End If
            '            End If
            '        End If
            '    End If

            'End While

            'Return result
        End Function
    End Class

    Public Class Septoria_leaf_blotch
        Inherits ModelloAvversita

        Private Class datoSeptoria
            Public data As Date
            Public INF As Decimal
            Public RISK As Decimal
            Public symptom_day As Decimal
            Public end_symptom_day As Decimal
            Public msnn As Decimal
            Public l_number_sf9_round_finale As Decimal
        End Class

        Private ReadOnly elencoSeptoria As List(Of datoSeptoria)
        Private ReadOnly _condizioniSeptoria As CondizioniSeptoria

        Public Sub New(crop_data As crop, fenologia As ModelloFenologico, _condizioniSeptoria As CondizioniSeptoria)

            MyBase.New(crop_data, fenologia)

            Me._condizioniSeptoria = _condizioniSeptoria

            elencoSeptoria = New List(Of datoSeptoria)
        End Sub

        Public Overrides Sub elabora(dgg As datoGG)

            If Math.Round(fenologia.get_CUMVER) < 50 AndAlso dgg.cbd < crop_data.bdEMR + 3 Then
                _stato = Stato.PreFase
                Return
            End If

            'Previsione foglie effettuata

            If crop_data.bdTSG < dgg.cbd Then
                _stato = Stato.PostFase
                Return
            End If

            _stato = Stato.InFase

            Dim L_NUMBER_SF9_round_finale = fenologia.get_L_NUMBER_SF9_round

            If dgg.cbd >= crop_data.bdBOT Then

                L_NUMBER_SF9_round_finale = Math.Round(dgg.msnn)
            End If

            Dim dsept As New datoSeptoria With {
                .data = dgg.data,
                .INF = 0,
                .RISK = dgg.msnn / L_NUMBER_SF9_round_finale,
                .symptom_day = 0,
                .end_symptom_day = 0,
                .msnn = dgg.msnn,
                .l_number_sf9_round_finale = L_NUMBER_SF9_round_finale
            }

            '**************************************************************************************
            'VERIFICARE ***************************************************************************
            '**************************************************************************************

            If dgg.tmp <> 0 Then

                If dgg.tmp < 26.4 Then

                    dsept.symptom_day = 1 / (0.0052 * dgg.tmp - 0.00000027 * dgg.tmp ^ 4)

                Else

                    dsept.symptom_day = -3.7677 * (dgg.tmp ^ 2) + 396.6034 * dgg.tmp - 7668.8
                End If

                dsept.end_symptom_day = 1 / (0.0049 * dgg.tmp - 0.00000041 * dgg.tmp ^ 4)
            End If

            '**************************************************************************************
            '**************************************************************************************
            '**************************************************************************************

            elencoSeptoria.Add(dsept)

            Dim possibiliInfezioni = _condizioniSeptoria.possibiliInfezioni(dgg.data)

            If possibiliInfezioni.Any() Then

                While possibiliInfezioni.Any

                    Dim inf = possibiliInfezioni.First
                    possibiliInfezioni.RemoveAt(0)

                    Dim idx As Integer = elencoSeptoria.Count - 1
                    Dim EndWhile As Boolean = False

                    While Not EndWhile AndAlso idx >= 0

                        If inf.data.Date = elencoSeptoria(idx).data.Date Then

                            elencoSeptoria(idx).INF += inf.possibili_infezioni
                            EndWhile = True
                        End If
                    End While

                End While
            End If

        End Sub

        Public Overrides Function generaOutput() As String

            Throw New NotImplementedException()

            For Each dsept In elencoSeptoria

                If dsept.INF > 0 Then

                    'Data evento infettivo          ->  dsept.data
                    'Numero infezioni               ->  dsept.INF
                    'Periodo di latenza             ->  dsept.symptom_day
                    'Data comparsa sintomi (Inizio) ->  dsept.data.AddDays(math.Floor(dsept.symptom_day))
                    'Data comparsa sintomi (Fine)   ->  dsept.data.AddDays(math.Floor(dsept.end_symptom_day))
                    'Severità                       ->  dsept.RISK


                End If
            Next



            'Il rischio dell’infezione (RISK_Septoria) [0 - 1], calcolato come il rapporto tra il valore di MSNN in un dato giorno (MSNN(i)) ed il numero di foglie previste (round(L_NUMBER_SF9)), è considerato:

            '    - grave (“WIDGET” ROSSO) se le infezioni si verificano su penultima ed ultima foglia;
            '    - medio (“WIDGET” GIALLO) se le infezioni si verificano sulle due foglie precedenti alla penultima;
            '    - basso (“WIDGET” VERDE) se le infezioni si verificano su tutte le altre (i.e., prime foglie).


            'La divisione del termometro nelle tre parti (rosso, giallo, verde) è funzione della previsione del numero di foglie totale (L_NUMBER_SF9):

            '    - parte ROSSA (lunga 1/round(L_NUMBER_SF9), ultime due foglie)
            '    - parte GIALLA (lunga 2/round(L_NUMBER_SF9), 2 foglie precedenti alla penultima)
            '    - parte VERDE (lunga (round(L_NUMBER_SF9)- 3)/ round(L_NUMBER_SF9), foglie rimanenti)

            'L'asticella del termometro rappresenta il valore di MSNN.



            '            disp(strcat('[SEPTORIA] EVENTO INFETTIVO il giorno', 32, num2str(dap)))
            '            INF_Septoria(i) = possibili_infezioni;
            '            [ date ] = from_DOY_to_date( dap, str2num(data_semina_day), str2num(data_semina_month), str2num(data_semina_year) );

            '            symptom_day(i) = latent_period(TMP(i));
            '            end_symptom_day(i) = end_latent_period(TMP(i));

            '            disp(strcat('[SEPTORIA] Inizio sintomi il giorno', 32, num2str(dap + floor(symptom_day(i)))))
            '            [ date ] = from_DOY_to_date( floor(dap + symptom_day(i)), str2num(data_semina_day), str2num(data_semina_month), str2num(data_semina_year) ); % prima round

            '            disp(strcat('[SEPTORIA] Massima espressione dei sintomi il giorno', 32, num2str(dap + floor(end_symptom_day(i)))))
            '            [ date ] = from_DOY_to_date( floor(dap + end_symptom_day(i)), str2num(data_semina_day), str2num(data_semina_month), str2num(data_semina_year) ); % prima round

            '            disp(strcat('[SEPTORIA] NUMERO di infezioni', 32, num2str(INF_Septoria(i))));
            '            if(MSNN(i) >= L_NUMBER_SF9_round_finale - 2) % -1
            '                disp(strcat('[SEPTORIA] INDICE DI RISCHIO:', 32, num2str(RISK_Septoria(i)), 32,'ROSSO'))
            '            elseif(MSNN(i) >= L_NUMBER_SF9_round_finale - 3)
            '                disp(strcat('[SEPTORIA] INDICE DI RISCHIO:', 32, num2str(RISK_Septoria(i)), 32,'GIALLO'))
            '            else
            '                disp(strcat('[SEPTORIA] INDICE DI RISCHIO:', 32, num2str(RISK_Septoria(i)), 32,'VERDE'))
            '            end

        End Function
    End Class

End Namespace


