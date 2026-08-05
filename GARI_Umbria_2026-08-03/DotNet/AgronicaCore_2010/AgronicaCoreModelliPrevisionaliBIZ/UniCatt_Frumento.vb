Imports AgronicaCoreDataProvider.My.Resources

Public Class UniCatt_Frumento
    Inherits AbstractModello

    Private Class DatoGiorno
        Public Data As Date
        Public HOST As Decimal
        Public T As Decimal
        Public RH As Decimal
        Public R As Decimal
        Public LW As Decimal
        Public RintMax_P As Decimal 'Valore max di R nelle 24h precedenti
        Public RH80 As Decimal      'Sommatoria giornaliere degli eventi in cui RH > 80
        Public Rtot_P As Decimal    'Sommatoria della pioggia nelle 24, riferita al giorno precedente
        Public W As Decimal
        Public aW As Decimal
        Public Fg As Decimal
        Public Fc As Decimal
        Public TOX_tot As Decimal

        Public Sub SetW(perW As Integer)
            Select Case perW
                Case 0
                    W = 0
                Case 1
                    W = 1.1
                Case 2
                    W = 2.5
                Case 3
                    W = 1.2
                Case Else
                    W = 0.8
            End Select
        End Sub

        Public ReadOnly Property Teq As Decimal
            Get
                Return (T - 5D) / 30D
            End Get
        End Property

        Public ReadOnly Property SPOR_Fg As Decimal
            Get
                Return SPOR(25.97625, 8.592385, 0.2414387)
            End Get
        End Property

        Public ReadOnly Property SPOR_Fc As Decimal
            Get
                Return SPOR(20.92, 8.12, 0.32)
            End Get
        End Property

        Public ReadOnly Property SPOR_Fa As Decimal
            Get
                Return SPOR(12.43, 4.27, 0.49)
            End Get
        End Property

        Public ReadOnly Property SPOR_Mn As Decimal
            Get
                Return SPOR(6.64, 1.99, 2.55)
            End Get
        End Property

        Private Function SPOR(m As Decimal, e1 As Decimal, e2 As Decimal) As Decimal
            If T <= 0 Then
                Return 0
            End If
            Dim tt As Decimal = T / 35.1D
            Return (m * (tt ^ e1) * (1 - tt)) ^ e2
        End Function

        Public ReadOnly Property DISP As Decimal
            Get
                Dim DIS2 As Decimal

                If R > 0 Then
                    DIS2 = (-839.7D + 4.083195D * T * T + 410.275692D * W + 115.448323 * RintMax_P) / 3307D
                Else
                    DIS2 = ((-682.280911D + 45.681859D * T + 21.497537D * RH80 + 107.012811D * Rtot_P) / 10D) / 4550D
                End If

                If DIS2 > 0 Then

                    Return DIS2
                End If

                Return 0
            End Get
        End Property

        Public ReadOnly Property INF_Fg As Decimal
            Get
                Return Math.Max(0D, (((((-0.0000024D * T) + 0.000199D) * T - 0.00591D) * T + 0.07808D) * T - 0.363D) * LW)
            End Get
        End Property

        Public ReadOnly Property INF_Fc As Decimal
            Get
                Return Math.Max(0D, ((((-0.000017D * T) + 0.00101D) * T - 0.01746D) * T + 0.09049D) * LW)
            End Get
        End Property

        Public ReadOnly Property INF_Fa As Decimal
            Get
                Return Math.Max(0D, ((((-0.000000828D * T) + 0.000041D) * T) - 0.00042D) * T * T * LW)
            End Get
        End Property

        Public ReadOnly Property INF_Mn As Decimal
            Get
                Return Math.Max(0D, (((((0.000013D * T) - 0.00109D) * T) + 0.02649D) * T - 0.1703D) * LW)
            End Get
        End Property

        Public ReadOnly Property Risk_Fg As Decimal
            Get
                If INF_Fg > 0 Then
                    Return SPOR_Fg * DISP * INF_Fg * HOST
                End If
                Return 0
            End Get
        End Property

        Public ReadOnly Property Risk_Fc As Decimal
            Get
                If INF_Fc > 0 Then
                    Return SPOR_Fc * DISP * INF_Fc * HOST * If(RH < 80, 10D, 1D)
                End If
                Return 0
            End Get
        End Property

        Public ReadOnly Property Risk_Fa As Decimal
            Get
                If INF_Fa > 0 Then
                    Return SPOR_Fa * DISP * INF_Fa * HOST
                End If
                Return 0
            End Get
        End Property

        Public ReadOnly Property Risk_Mn As Decimal
            Get
                If INF_Mn > 0 Then
                    Return SPOR_Mn * DISP * INF_Mn * HOST
                End If
                Return 0
            End Get
        End Property

        Public ReadOnly Property MIC_Fg As Decimal
            Get
                If T <= 0 Then
                    Return 0
                End If
                Dim tt As Decimal = T / 38D
                Return Math.Min(1, (5.53D * tt ^ 1.55D * (1D - tt)) ^ 1.35D)
            End Get
        End Property

        Public ReadOnly Property MIC_Fc As Decimal
            Get
                If T <= 2 Then
                    Return 0
                End If
                Dim tt As Decimal = (T - 2D) / 34D
                Return Math.Min(1, (5.83D * tt ^ 1.54D * (1D - tt)) ^ 1.35D)
            End Get
        End Property

        Public ReadOnly Property DON_T_Fg As Decimal
            Get
                Dim t_eq As Decimal = Teq
                Return (((((((-30.365D * t_eq + 49.283D) * t_eq) - 21.277D) * t_eq) + 1.7363D) * t_eq) + 0.611D) * t_eq
            End Get
        End Property

        Public ReadOnly Property DON_T_Fc As Decimal
            Get
                Dim t_eq As Decimal = Teq
                If t_eq < 0 Then
                    Return 0
                End If
                Return Math.Min(1, (3.974D * t_eq ^ 0.972D * (1D - t_eq)) ^ 2.34D)
            End Get
        End Property

        Public ReadOnly Property Tox_Fg As Decimal
            Get
                Return MIC_Fg * DON_aW_both * DON_T_Fg * Fg
            End Get
        End Property

        Public ReadOnly Property Tox_Fc As Decimal
            Get
                Return MIC_Fc * DON_aW_both * DON_T_Fc * Fc
            End Get
        End Property

        Private ReadOnly Property DON_aW_both As Decimal
            Get
                Return (1D / (1D + Math.Exp(98.72D - 101.24D * aW))) / 0.9255D
            End Get
        End Property

    End Class


    Private ReadOnly _dtSpigatura As Date


    Public Sub New(datiMeteo As MeteoReadOnlyList, parametriAggiuntivi As String)
        MyBase.New(datiMeteo)

        Dim lettore As New LettoreParametri(parametriAggiuntivi)

        Dim dtSpigatura As Date = lettore.DateOrDefault("DataSpigatura", Date.MinValue)
        Dim dtSpigatura_gg As Integer = lettore.IntOrDefault("DataSpigatura_gg", 0)

        _dtSpigatura = _data_from_doy(dtSpigatura, dtSpigatura_gg, 5, 8) '8 maggio
    End Sub


    Protected Overrides Function _elaboraModello() As cRisultatoModello

        Dim risModello As cRisultatoModello = Nothing

        Try

            Dim datiModello = Elabora()

            If datiModello IsNot Nothing Then

                Dim rismod As New cRisultatoModello

                Dim output As New OutputModello

                output.aggiungiColonna("Data", GetType(Date), "Data", "dd/MM/yyyy")
                'output.aggiungiColonna("HOST", GetType(Decimal), "HOST", "0.00")
                output.aggiungiColonna("T", GetType(Decimal), Gias.Temperatura & " (°C)", "0.0")
                output.aggiungiColonna("UR", GetType(Decimal), Gias.UmiditaRelativa & " (%)", "0.0")
                output.aggiungiColonna("R", GetType(Decimal), Gias.Pioggia & " (mm)", "0.0")
                output.aggiungiColonna("LW", GetType(Decimal), Gias.BagnaturaFogliare, "0")
                'output.aggiungiColonna("W", GetType(Decimal), "W", "0.0")
                'output.aggiungiColonna("Teq", GetType(Decimal), "Teq", "0.000")
                'output.aggiungiColonna("aW", GetType(Decimal), "aW", "0.000")
                'output.aggiungiColonna("SPOR_Fg", GetType(Decimal), "SPOR_Fg", "0.000")
                'output.aggiungiColonna("SPOR_Fc", GetType(Decimal), "SPOR_Fc", "0.000")
                'output.aggiungiColonna("SPOR_Fa", GetType(Decimal), "SPOR_Fa", "0.000")
                'output.aggiungiColonna("SPOR_Mn", GetType(Decimal), "SPOR_Mn", "0.000")
                'output.aggiungiColonna("DISP", GetType(Decimal), "DISP", "0.000")
                'output.aggiungiColonna("INF_Fg", GetType(Decimal), "INF_Fg", "0.000")
                'output.aggiungiColonna("INF_Fc", GetType(Decimal), "INF_Fc", "0.000")
                'output.aggiungiColonna("INF_Fa", GetType(Decimal), "INF_Fa", "0.000")
                'output.aggiungiColonna("INF_Mn", GetType(Decimal), "INF_Mn", "0.000")
                'output.aggiungiColonna("Risk_Fg", GetType(Decimal), "Risk_Fg", "0.000")
                'output.aggiungiColonna("Risk_Fc", GetType(Decimal), "Risk_Fc", "0.000")
                'output.aggiungiColonna("Risk_Fa", GetType(Decimal), "Risk_Fa", "0.000")
                'output.aggiungiColonna("Risk_Mn", GetType(Decimal), "Risk_Mn", "0.000")
                'output.aggiungiColonna("MIC_Fg", GetType(Decimal), "MIC_Fg", "0.000")
                'output.aggiungiColonna("MIC_Fc", GetType(Decimal), "MIC_Fc", "0.000")
                'output.aggiungiColonna("DON_T_Fg", GetType(Decimal), "DON_T_Fg", "0.00")
                'output.aggiungiColonna("DON_T_Fc", GetType(Decimal), "DON_T_Fc", "0.00")
                'output.aggiungiColonna("Fg", GetType(Decimal), "Fg", "0.000")
                'output.aggiungiColonna("Fc", GetType(Decimal), "Fc", "0.000")
                'output.aggiungiColonna("Tox_Fg", GetType(Decimal), "Tox Fg", "0.00")
                'output.aggiungiColonna("Tox_Fc", GetType(Decimal), "Tox Fc", "0.00")
                output.aggiungiColonna("TOX_Fg", GetType(Decimal), "TOX Fg", "0.00")
                output.aggiungiColonna("TOX_Fc", GetType(Decimal), "TOX Fc", "0.00")
                output.aggiungiColonna("TOX_Tot", GetType(Decimal), "TOX tot", "0.000")

                Dim TOX_Fg As Decimal = 0
                Dim TOX_Fc As Decimal = 0

                For Each dm In datiModello

                    output.AddField(dm.Data)
                    'output.AddField(dm.HOST)
                    output.AddField(dm.T)
                    output.AddField(dm.RH)
                    output.AddField(dm.R)
                    output.AddField(dm.LW)
                    'output.AddField(dm.W)
                    'output.AddField(dm.Teq)
                    'output.AddField(dm.aW)
                    'output.AddField(dm.SPOR_Fg)
                    'output.AddField(dm.SPOR_Fc)
                    'output.AddField(dm.SPOR_Fa)
                    'output.AddField(dm.SPOR_Mn)
                    'output.AddField(dm.DISP)
                    'output.AddField(dm.INF_Fg)
                    'output.AddField(dm.INF_Fc)
                    'output.AddField(dm.INF_Fa)
                    'output.AddField(dm.INF_Mn)
                    'output.AddField(dm.Risk_Fg)
                    'output.AddField(dm.Risk_Fc)
                    'output.AddField(dm.Risk_Fa)
                    'output.AddField(dm.Risk_Mn)
                    'output.AddField(dm.MIC_Fg)
                    'output.AddField(dm.MIC_Fc)
                    'output.AddField(dm.DON_T_Fg)
                    'output.AddField(dm.DON_T_Fc)
                    'output.AddField(dm.Fg)
                    'output.AddField(dm.Fc)
                    'output.AddField(dm.Tox_Fg)
                    'output.AddField(dm.Tox_Fc)
                    TOX_Fg += dm.Tox_Fg
                    TOX_Fc += dm.Tox_Fc

                    output.AddField(TOX_Fg)
                    output.AddField(TOX_Fc)
                    output.AddField(dm.TOX_tot)

                    output.Commit()
                Next

                risModello = New cRisultatoModello With {
                    .Modello_Tabella1 = output.Output()
                }
            End If

        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            _errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

            risModello = Nothing
        End Try

        Return risModello
    End Function

    Protected Overrides Function _elaboraIndicatore_vecchio(dataInizio As Date, dataFine As Date) As cRisultatoModelloIndicatori.Indicatore

        Throw New NotImplementedException()

        'Dim risIndic As New cRisultatoModelloIndicatori.Indicatore("", dataInizio, dataFine)

        'Dim flagOK As Boolean = False

        'Try

        '    Dim modello = CreaModello()

        '    If modello IsNot Nothing Then

        '        modello.GeneraIndicatore_Vecchio(risIndic, dataFine)

        '        flagOK = True
        '    End If

        'Catch ex As Exception
        '    'uso questa funzione per ottenere il Messaggio..:
        '    _errore = "Errore durante l'operazione: " & vbCrLf &
        '        AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        'End Try

        'If Not flagOK Then

        '    risIndic.Status = cRisultatoModelloIndicatori.Indicatore.enum_Status.Status_Error
        '    risIndic.StatusMsg = _errore
        'End If

        'Return risIndic
    End Function

    Protected Overrides Function _elaboraIndicatore(risElab As RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione) As RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione

        'Dim flagOK As Boolean = False

        'Try

        '    Dim modello = CreaModello()

        '    If modello IsNot Nothing Then

        '        modello.GeneraIndicatore(risElab)

        '        flagOK = True
        '    End If

        'Catch ex As Exception
        '    'uso questa funzione per ottenere il Messaggio..:
        '    _errore = "Errore durante l'operazione: " & vbCrLf &
        '        AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        'End Try

        'If Not flagOK Then

        '    risElab.Errore(_errore)
        'End If

        risElab.Errore("Indicatore per il modello non disponibile")

        Return risElab
    End Function


    Private Function Elabora() As List(Of DatoGiorno)

        If Not _datiMeteo.Any Then
            Return Nothing
        End If

        Dim GiorniHost As New Dictionary(Of Integer, Decimal) From {
            {-10, 0.15}, {-9, 0.3}, {-8, 0.3}, {-7, 0.3}, {-6, 0.3}, {-5, 0.5}, {-4, 1}, {-3, 1.25}, {-2, 1.5}, {-1, 2.5},
            {1, 3}, {2, 3}, {3, 3}, {4, 3}, {5, 3}, {6, 3}, {7, 2}, {8, 1}, {9, 0.8}, {10, 0.5}, {11, 0.5}, {12, 0.5},
            {13, 0.5}, {14, 0.5}, {15, 0.5}, {16, 0.5}, {17, 0.2}, {18, 0.2}, {19, 0.2}, {20, 0.2}, {21, 0.2}, {22, 0.2}, {23, 0.2}, {24, 0.2},
            {25, 0.2}, {26, 0.2}, {27, 0.1}, {28, 0.1}, {29, 0.1}, {30, 0.1}, {31, 0.1}, {32, 0.1}, {33, 0.1}, {34, 0.1}, {35, 0.1}, {36, 0.1}
        }

        Dim datiModello As New List(Of DatoGiorno)

        Dim R_sum As Decimal = 0
        Dim R_max As Decimal = 0
        Dim prevDT As Date = Date.MinValue
        Dim hh As Decimal
        Dim dg As DatoGiorno = Nothing

        Dim perW As Integer = 0
        Dim last_Fg As Decimal = 0
        Dim Risk_Fg As Decimal
        Dim last_Fc As Decimal = 0
        Dim Risk_Fc As Decimal
        Dim cum_Tox_Fg As Decimal = 0
        Dim cum_Tox_Fc As Decimal = 0

        For Each dm In _datiMeteo

            If dm.DataOra.Date <> prevDT Then

                If dg IsNot Nothing Then

                    dg.T /= hh
                    dg.RH /= hh

                    If dg.R > 0 Then
                        perW += 1
                    Else
                        perW = 0
                    End If

                    dg.SetW(perW)

                    Risk_Fg = dg.Risk_Fg
                    dg.Fg = last_Fg + If(Risk_Fg > 0.001D, Risk_Fg, 0D)
                    last_Fg = dg.Fg

                    Risk_Fc = dg.Risk_Fc
                    dg.Fc = last_Fc + If(Risk_Fc > 0.001D, Risk_Fc, 0D)
                    last_Fc = dg.Fc

                    cum_Tox_Fg += dg.Tox_Fg
                    cum_Tox_Fc += dg.Tox_Fc

                    dg.TOX_tot = cum_Tox_Fg + cum_Tox_Fc

                    datiModello.Add(dg)
                End If

                dg = New DatoGiorno With {
                    .Data = dm.DataOra.Date,
                    .HOST = 0,
                    .T = dm.Temp,
                    .RH = dm.UmRel,
                    .R = dm.Prec,
                    .LW = dm.Bagn,
                    .RintMax_P = R_max,
                    .RH80 = IIf(dm.UmRel > 80, 1, 0),
                    .Rtot_P = R_sum
                }

                Dim giorni As Decimal = Math.Max(0, 1 + DateDiff(DateInterval.Day, _dtSpigatura, dg.Data))
                If giorni <= 46 Then

                    If giorni > 0 Then

                        giorni -= 11

                        If giorni >= 0 Then

                            giorni += 1
                        End If
                    End If

                    If giorni <> 0 Then

                        dg.HOST = GiorniHost(giorni)
                    End If
                End If

                dg.aW = 1D - Math.Exp(-27.6258D * Math.Exp(-0.069515D * giorni))

                R_max = dm.Prec
                R_sum = dm.Prec
                hh = 1
                prevDT = dm.DataOra.Date

            Else

                dg.T += dm.Temp
                dg.RH += dm.UmRel
                dg.R += dm.Prec
                dg.LW += dm.Bagn
                dg.RH80 += IIf(dm.UmRel > 80, 1, 0)

                If dm.Prec > R_max Then
                    R_max = dm.Prec
                End If
                R_sum += dm.Prec
                hh += 1
            End If
        Next

        dg.T /= hh
        dg.RH /= hh

        If dg.R > 0 Then
            perW += 1
        Else
            perW = 0
        End If

        dg.SetW(perW)

        Risk_Fg = dg.Risk_Fg
        dg.Fg = last_Fg + If(Risk_Fg > 0.001D, Risk_Fg, 0D)

        Risk_Fc = dg.Risk_Fc
        dg.Fc = last_Fc + If(Risk_Fc > 0.001D, Risk_Fc, 0D)

        cum_Tox_Fg += dg.Tox_Fg
        cum_Tox_Fc += dg.Tox_Fc

        dg.TOX_tot = cum_Tox_Fg + cum_Tox_Fc

        datiModello.Add(dg)

        Return datiModello
    End Function


    Public Shared Function EsponiParams(ByVal params As String) As String

        Dim lettore As New LettoreParametri(params)

        Dim str_dt As String = lettore.OutputFromDate("DataSpigatura", "Data spigatura: ")

        If String.IsNullOrEmpty(str_dt) Then

            str_dt = lettore.OutputFromDOY("DataSpigatura_gg", "Data spigatura: ")
        End If

        Return str_dt
    End Function

End Class
