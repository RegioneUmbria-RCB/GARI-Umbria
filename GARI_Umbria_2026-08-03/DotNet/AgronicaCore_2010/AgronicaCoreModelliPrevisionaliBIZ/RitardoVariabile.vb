
Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility.DataOra

Imports System.Math
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreUtility.VisualStudioHelper
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModelliPrevisionaliBIZ

Public Class RitardoVariabile
    Inherits AbstractModello

    Private Class Modello
        ' parametri curva di Logan
        Private Structure ParametriLogan
            Public P1 As Double
            Public P2 As Double
            Public P3 As Double
            Public TempMaxA As Double
            Public TempMinA As Double
            Public TempMax As Double
            Public TempMin As Double
            Public Sub SetVal(ByVal idx As Integer, ByVal val As Double)
                Select Case idx
                    Case 0
                        P1 = val
                    Case 1
                        P2 = val
                    Case 2
                        P3 = val
                    Case 3
                        TempMaxA = val
                    Case 4
                        TempMinA = val
                    Case 5
                        TempMax = val
                    Case 6
                        TempMin = val
                End Select
            End Sub
            Public Function Calcola(ByVal temp As Double) As Double
                Dim result As Double = 0
                If TempMin < temp And temp < TempMax Then
                    result = P1 * (Exp(P2 * (temp - TempMin)) - Exp(P2 * (TempMax - TempMin) - P3 * (TempMax - temp)))
                End If
                Return result
            End Function
            Public Function CalcolaA(ByVal temp As Double, ByVal val As Double) As Double
                Dim result As Double = 0
                If TempMinA <= temp And temp <= TempMaxA Then
                    result = P1 * temp + P2
                Else
                    If temp > TempMaxA Then
                        result = val
                    End If
                End If
                Return result
            End Function
        End Structure

        Private Class Stadio
            Public Flusso() As Double       ' Flusso (i) e' il valore del flusso che esce dal compartimento i al tempo t
            Public MassaQ() As Double       ' MassaQ (i) e' il valore del contenuto del compartimento i al tempo t
            Public Delay As Double          ' Delay e' il valore di Delay del passo di tempo DeltaT attuale
            Public TotStad As Double        ' TotStad: N. di individui di uno stadio che restano nel processo
            Public CumStad As Double        ' CumStad: N. di individui che hanno raggiunto uno stadio (compresi quelli che ne sono gia' usciti)
            Public Ingresso As Double       ' Ingresso e' il valore dell'ingresso nel processo per unita' di tempo
            Public Uscita As Double         ' Uscita e' il valore dell'uscita dal processo per unita' di tempo
            Public GioStad As Double
            Public Logan As ParametriLogan  ' parametri curva di Logan
            Public k As Integer
            Public Sub New(ByVal kmax As Integer)
                ReDim Flusso(kmax)
                ReDim MassaQ(kmax)
                For j = 0 To kmax
                    Flusso(j) = 0
                    MassaQ(j) = 0
                Next
                Delay = 0
                TotStad = 0
                CumStad = 0
                Ingresso = 0
                Uscita = 0
                GioStad = 0
            End Sub
            Public Sub CalcolaMassaQ()

                TotStad = 0
                For j = 0 To k
                    MassaQ(j) = Flusso(j) * (Delay / k)
                    TotStad += MassaQ(j)
                Next

            End Sub
        End Class

        Private StIni, SotStIni As Integer  ' Stadio e Sottostadio di partenza
        Private Logan_Deposito As ParametriLogan
        Private k_Deposito As Integer
        Private rOptAd As Double            ' tasso ottimale sviluppo adulti
        Private UovaFem As Double           ' numero totale di uova deposte da una femmina
        Private Bieri(2) As Double          ' parametri curva di Bieri

        Private NStadi As Integer
        Private DeltaT As Double            ' DeltaT e' la lunghezza in giorni del passo DeltaT
        Private PrimoDeltaT As Boolean
        Private GenerazioneIbernante As Boolean
        Private Stadi As List(Of Stadio)

        Private Function ParametroDB(ByVal pNome As String, ByVal DT As DataTable, Optional ByVal vDefault As String = "") As String

            Dim rVal As String = (From dd In DT.AsEnumerable
                                  Where dd("ParametroNome") = pNome
                                  Select CStr(dd("ParametroValore"))).DefaultIfEmpty(vDefault).First()

            rVal = rVal.Replace(".", Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator)

            Return rVal
        End Function

        Public Sub New(ByRef DTParams As DataTable)

            NStadi = 4 '4 Stadi (Uova, Larve, Pupe, Adulti)

            If DTParams.Rows.Count = 0 Then
                Throw New Exception("Errore: Non ci sono parametri!")
            End If

            ' VAnni: 3/4/2017: HACK: gli indici sono tutti a base zero in .net, su db sono a base 1

            StIni = ParametroDB("partenzaStadio", DTParams)
            StIni -= 1

            SotStIni = ParametroDB("partenzaSottostadio", DTParams)
            SotStIni -= 1

            rOptAd = ParametroDB("tuttitasso sviluppo", DTParams, "0")
            UovaFem = ParametroDB("uovauova deposte", DTParams, "0")

            For n = 0 To 2
                Bieri(n) = ParametroDB("tutticurva di Bieri" & (n + 1).ToString(), DTParams, "0")
            Next n

            Dim tmp_k(4) As Integer
            Dim kmax As Integer = 0
            For s = 0 To 4
                tmp_k(s) = ParametroDB("tuttivalori di k" & (s + 1).ToString(), DTParams, "0")
                If kmax < tmp_k(s) Then
                    kmax = tmp_k(s)
                End If
            Next

            Dim Tipi() As String = {"uovaLogan", "larveLogan", "pupeLogan", "adultiLogan"}
            Stadi = New List(Of Stadio)
            For iStadio = 0 To NStadi - 1

                Dim stad As New Stadio(kmax)

                For n = 0 To 6
                    stad.Logan.SetVal(n, ParametroDB(Tipi(iStadio) & (n + 1).ToString(), DTParams, "0"))
                Next
                stad.k = tmp_k(iStadio)

                Stadi.Add(stad)
            Next

            'Deposito dei valori dei parametri Logan e k dello stadio ibernante
            Logan_Deposito = Stadi(StIni).Logan
            k_Deposito = Stadi(StIni).k

            'Sostituzione dei parametri normali con quelli dello stadio ibernante
            For n = 0 To 6
                Stadi(StIni).Logan.SetVal(n, ParametroDB("svernantiLogan" & (n + 1).ToString(), DTParams, "0"))
            Next
            Stadi(StIni).k = tmp_k(4)

            DeltaT = 1 / 24

            GenerazioneIbernante = True
            PrimoDeltaT = True

            Stadi(StIni).Flusso(SotStIni) = 1 / DeltaT

        End Sub


        Public Function GetNStadi() As Integer
            Return NStadi
        End Function
        Public Function StrStadi() As String()
            Return {"Uova", "Larve", "Pupe", "Adulti"}
        End Function

        Public Sub Calcola(ByVal Temp As Decimal)

            Dim delayPrec, fLogan, a, V, Deld, dr As Double
            For Each stad In Stadi

                delayPrec = stad.Delay

                If stad Is Stadi.Last Then
                    ' Per gli adulti si usa una retta
                    fLogan = stad.Logan.CalcolaA(Temp, rOptAd)
                Else
                    'Per uova, larve e pupe, si usa la funzione di Logan
                    fLogan = stad.Logan.Calcola(Temp)
                End If
                fLogan = Math.Max(0.001, fLogan)

                stad.Delay = 1 / fLogan

                a = stad.k * (DeltaT / stad.Delay) 'If a >= 1 -> Abbassare delta T o k
                V = stad.Ingresso
                Deld = (stad.Delay - delayPrec) / (DeltaT * stad.k)

                For i = 0 To stad.k

                    dr = stad.Flusso(i)
                    stad.Flusso(i) = dr + a * (V - dr * (1 + Deld))
                    V = dr

                Next

                stad.Uscita = stad.Flusso(stad.k)
            Next

            Dim lastStadio As Stadio = Stadi(NStadi - 1)
            lastStadio.CalcolaMassaQ()
            Dim TotUova As Double = 0
            Dim DelOpt As Double = 1 / rOptAd
            Dim Eta, FertOpt, UovaDep As Double
            For i = 0 To lastStadio.k
                Eta = (i - 0.5) * DelOpt / lastStadio.k
                FertOpt = (Bieri(0) * (Eta - Bieri(1))) / (Exp(Log(Bieri(2)) * (Eta - Bieri(1))))
                FertOpt = Math.Max(0, FertOpt)
                UovaDep = lastStadio.MassaQ(i) / UovaFem * FertOpt * (DelOpt / lastStadio.Delay) * DeltaT
                TotUova += UovaDep
            Next i

            Dim prevStadio As Stadio = Nothing
            For Each currStadio In Stadi
                If prevStadio Is Nothing Then
                    currStadio.Ingresso = TotUova / DeltaT
                    currStadio.CumStad += TotUova
                    currStadio.GioStad += TotUova
                Else
                    currStadio.Ingresso = prevStadio.Uscita
                    currStadio.CumStad += prevStadio.Uscita * DeltaT
                    currStadio.GioStad += prevStadio.Uscita * DeltaT
                End If
                prevStadio = currStadio
            Next

            If PrimoDeltaT Then
                'azzeramento dell'input iniziale
                Stadi(StIni).Flusso(SotStIni) = 0
                PrimoDeltaT = False
            End If

        End Sub

        Public Sub Finalizza(ByRef dgg As DatoGG)

            For Each stad In Stadi
                stad.CalcolaMassaQ()
            Next

            'Controllo fine stadio ibernante ed eventuale ripristino parametri Logan e k.  
            'La fine è indicata dalla nuova produzione di individui dello stadio ibernante
            If GenerazioneIbernante And (Stadi(StIni).GioStad > 0.0009) Then
                'Ripristino Logan
                Stadi(StIni).Logan = Logan_Deposito
                'Ripristino k
                Stadi(StIni).k = k_Deposito
                GenerazioneIbernante = False
            End If

            For iStadio = 0 To NStadi - 1

                Dim stad As Stadio = Stadi(iStadio)

                ' calcolo della generazione, del cumulo e della presenza
                dgg.Generazione(iStadio) = Int(stad.CumStad)
                dgg.Cumulo(iStadio) = 100 * (stad.CumStad - dgg.Generazione(iStadio))
                dgg.Presenza(iStadio) = Math.Min(100D, 100 * stad.TotStad)

                If dgg.Cumulo(iStadio) < 1 Then
                    dgg.Cumulo(iStadio) = 0
                End If
                If dgg.Presenza(iStadio) < 1 Then
                    dgg.Presenza(iStadio) = 0
                End If

                If iStadio = 0 Or
                    (0 < iStadio And iStadio < NStadi - 1 And StIni >= iStadio) Or
                    (iStadio = NStadi - 1 And StIni = 0) Then
                    dgg.Generazione(iStadio) += 1
                End If

                If dgg.Cumulo(iStadio) = 0 Then
                    dgg.Generazione(iStadio) = Math.Max(0, dgg.Generazione(iStadio) - 1)
                End If
                If iStadio = NStadi - 1 And StIni = NStadi - 1 And Stadi(NStadi - 1).CumStad > 0 Then
                    dgg.Generazione(iStadio) += 1
                End If

                stad.GioStad = 0

            Next

        End Sub

    End Class

    Private Class DatoGG
        Public Data As DateTime
        Public Pioggia As Decimal
        Public Temp_h_18 As Decimal
        Public Temp_h_19 As Decimal
        Public Temp_h_20 As Decimal
        Public Generazione() As Integer
        Public Presenza() As Decimal
        Public Cumulo() As Decimal
        Public Sub New(ByVal dt As DateTime, ByVal nstadi As Integer)
            Data = dt
            Pioggia = 0
            Temp_h_18 = -1000
            Temp_h_19 = -1000
            Temp_h_20 = -1000
            ReDim Generazione(nstadi - 1)
            ReDim Presenza(nstadi - 1)
            ReDim Cumulo(nstadi - 1)
            For istadio = 0 To nstadi - 1
                Generazione(istadio) = 0
                Presenza(istadio) = 0
                Cumulo(istadio) = 0
            Next
        End Sub
    End Class

    Private ReadOnly _av_cod As Integer
    Private ReadOnly _datiGG As List(Of DatoGG)
    Private ReadOnly _mdl As Modello

    Public Sub New(ByVal datiMeteo As MeteoReadOnlyList, Mod_Cod As Integer, Veg_Cod As Long, Av_Cod As Long, Tipo_Cod As Integer, objParametri_Server As AgronicaCoreParametri)
        MyBase.New(datiMeteo)

        _av_cod = Av_Cod
        _datiGG = New List(Of DatoGG)

        Dim xLeggi As New AgronicaCoreModelliPrevisionaliDAL.ParametriModelli_R
        ' VAnni: 30/3/2017: passo zero come da versione in VB6
        Dim DTParams As DataTable = xLeggi.Leggi(Av_Cod, 0, Mod_Cod, "", "", objParametri_Server)

        _mdl = New Modello(DTParams)
    End Sub

    Protected Overrides Function _elaboraModello() As cRisultatoModello

        Dim risModello As cRisultatoModello = Nothing

        Try

            Dim NStadi As Integer = _mdl.GetNStadi()

            Dim dgg As DatoGG = Nothing

            For Each d In _datiMeteo

                If dgg Is Nothing OrElse d.DataOra.DayOfYear <> dgg.Data.DayOfYear Then

                    If dgg IsNot Nothing Then
                        _mdl.Finalizza(dgg)
                        _datiGG.Add(dgg)
                    End If

                    dgg = New DatoGG(d.DataOra, NStadi)
                End If

                dgg.Pioggia += d.Prec
                Select Case d.DataOra.Hour
                    Case 18
                        dgg.Temp_h_18 = d.Temp
                    Case 19
                        dgg.Temp_h_19 = d.Temp
                    Case 20
                        dgg.Temp_h_20 = d.Temp
                End Select
                _mdl.Calcola(d.Temp)
            Next

            If dgg IsNot Nothing Then
                _mdl.Finalizza(dgg)
                _datiGG.Add(dgg)
            End If

            'Dim Seq As Integer
            'Dim idx As Integer
            'For iStadio = 0 To NStadi - 1
            '    idx = 0
            '    Seq = 0
            '    While idx < DatiGG.Count

            '        dgg = DatiGG(idx)

            '        If Seq < 2 And dgg.Cumulo(iStadio) > 98.5 Then
            '            Seq = Seq + 1
            '        ElseIf Seq = 2 And dgg.Cumulo(iStadio) > 98.5 Then
            '            Seq = 0
            '            dgg.Cumulo(iStadio) = 100
            '        End If

            '        If dgg.Cumulo(iStadio) > 99.5 Then
            '            'elimina la sequenza di 99 0 100
            '            idx += 1
            '            While idx < DatiGG.Count AndAlso DatiGG(idx).Cumulo(iStadio) > 98.5
            '                DatiGG(idx).Cumulo(iStadio) = 0
            '                idx += 1
            '            End While
            '        End If
            '        idx += 1
            '    End While
            'Next

            risModello = New cRisultatoModello With {
                .Modello_Tabella1 = OutputModello(),
                .Modello_Descrizione = DescrizioneModello()
            }

        Catch ex As Exception

            risModello = Nothing

            'uso questa funzione per ottenere il Messaggio..:
            _errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return risModello
    End Function

    Protected Overrides Function _elaboraIndicatore_vecchio(dataInizio As Date, dataFine As Date) As cRisultatoModelloIndicatori.Indicatore
        Throw New NotImplementedException()
    End Function

    Protected Overrides Function _elaboraIndicatore(ByVal risElab As RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione) As RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione
        Throw New NotImplementedException()
    End Function


    'Public Function CalcolaRitardoVariabile(ByVal datiMeteo As List(Of AgronicaCoreMeteoBiz.InterfacciaMeteoNT.MeteoDSS), Av_Cod As Long, Veg_Cod As Long, Classe As String, Tipo_Cod As Integer, objParametri_Server As AgronicaCoreParametri) As AgronicaCoreVarieBIZ.rispostaStandard(Of cRisultatoModello)

    '    Dim rval As New rispostaStandard(Of cRisultatoModello)
    '    rval.RispostaStringa = New cRisultatoModello()

    '    Try

    '        Dim xLeggi As New AgronicaCoreModelliPrevisionaliDAL.ParametriModelli_R
    '        ' VAnni: 30/3/2017: passo zero come da versione in VB6
    '        Dim DTParams As DataTable = xLeggi.Leggi(Av_Cod, 0, Classe, "", "", objParametri_Server)

    '        Mdl = New Modello(DTParams)

    '        Dim NStadi As Integer = Mdl.GetNStadi()

    '        Dim dgg As DatoGG = Nothing
    '        For Each d In datiMeteo

    '            If dgg Is Nothing OrElse d.DataOra.DayOfYear <> dgg.Data.DayOfYear Then

    '                If dgg IsNot Nothing Then
    '                    Mdl.Finalizza(dgg)
    '                    DatiGG.Add(dgg)
    '                End If

    '                dgg = New DatoGG(d.DataOra, NStadi)
    '            End If

    '            dgg.Pioggia += d.Prec
    '            Select Case d.DataOra.Hour
    '                Case 18
    '                    dgg.Temp_h_18 = d.Temp
    '                Case 19
    '                    dgg.Temp_h_19 = d.Temp
    '                Case 20
    '                    dgg.Temp_h_20 = d.Temp
    '            End Select
    '            Mdl.Calcola(d.Temp)
    '        Next

    '        If dgg IsNot Nothing Then
    '            Mdl.Finalizza(dgg)
    '            DatiGG.Add(dgg)
    '        End If

    '        'Dim Seq As Integer
    '        'Dim idx As Integer
    '        'For iStadio = 0 To NStadi - 1
    '        '    idx = 0
    '        '    Seq = 0
    '        '    While idx < DatiGG.Count

    '        '        dgg = DatiGG(idx)

    '        '        If Seq < 2 And dgg.Cumulo(iStadio) > 98.5 Then
    '        '            Seq = Seq + 1
    '        '        ElseIf Seq = 2 And dgg.Cumulo(iStadio) > 98.5 Then
    '        '            Seq = 0
    '        '            dgg.Cumulo(iStadio) = 100
    '        '        End If

    '        '        If dgg.Cumulo(iStadio) > 99.5 Then
    '        '            'elimina la sequenza di 99 0 100
    '        '            idx += 1
    '        '            While idx < DatiGG.Count AndAlso DatiGG(idx).Cumulo(iStadio) > 98.5
    '        '                DatiGG(idx).Cumulo(iStadio) = 0
    '        '                idx += 1
    '        '            End While
    '        '        End If
    '        '        idx += 1
    '        '    End While
    '        'Next

    '        rval.RispostaOK = True
    '        rval.RispostaStringa.Modello_Tabella1 = OutputModello(Av_Cod)
    '        rval.RispostaStringa.Modello_Descrizione = DescrizioneModello(Av_Cod)

    '    Catch ex As Exception

    '        rval.RispostaOK = False

    '        'uso questa funzione per ottenere il Messaggio..:
    '        rval.Errore = "Errore durante l'operazione: " & vbCrLf &
    '            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

    '    End Try

    '    Return rval

    'End Function

    Private Function OutputModello() As String

        Dim NStadi As Integer = _mdl.GetNStadi()

        Dim StrStadi() As String = _mdl.StrStadi()
        Dim StrGen() As String = {"SV", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X"}

        Dim numGenMax As Integer = 0
        For Each dgg In _datiGG
            For iStadio = 0 To NStadi - 1
                If dgg.Generazione(iStadio) > numGenMax Then
                    numGenMax = dgg.Generazione(iStadio)
                End If
            Next
        Next

        Dim numGenMax_x_Av As Integer = 5 'Al massimo considero 5 generazioni in output (Tabella troppo grande???) 
        If _av_cod = 87 Or _av_cod = 94 Then
            numGenMax_x_Av = 2
        End If
        numGenMax = Math.Min(numGenMax_x_Av, numGenMax + 1)

        Dim output As New OutputModello

        output.aggiungiColonna("Data", GetType(DateTime), "Data", "dd/MM/yyyy")
        For iGen = 0 To numGenMax - 1
            For iStadio = 0 To NStadi - 1
                output.aggiungiColonna("Gen_" + StrGen(iGen) + "_Stadio_" + StrStadi(iStadio) + "_Cumulo",
                                           GetType(Decimal),
                                           "Cumulo",
                                           "0")._gruppoColonne = "Generazione " + StrGen(iGen) + "|" + StrStadi(iStadio)

                output.aggiungiColonna("Gen_" + StrGen(iGen) + "_Stadio_" + StrStadi(iStadio) + "_Presenza",
                                           GetType(Decimal),
                                           "Presenza",
                                           "0")._gruppoColonne = "Generazione " + StrGen(iGen) + "|" + StrStadi(iStadio)
            Next
        Next
        output.aggiungiColonna("Pioggia", GetType(Decimal), "Pioggia", "0.00")
        If _av_cod = 87 Or _av_cod = 94 Then
            output.aggiungiColonna("T_h18", GetType(Decimal), "Temp 18h", "0.00", True)
            output.aggiungiColonna("T_h19", GetType(Decimal), "Temp 19h", "0.00", True)
            output.aggiungiColonna("T_h20", GetType(Decimal), "Temp 20h", "0.00", True)
        End If

        For Each riga In _datiGG

            output.AddField(riga.Data)

            For iGen = 0 To numGenMax - 1
                For iStadio = 0 To NStadi - 1
                    If riga.Generazione(iStadio) = iGen Then
                        output.AddField(riga.Cumulo(iStadio))
                        output.AddField(riga.Presenza(iStadio))
                    Else
                        output.AddField(0)
                        output.AddField(0)
                    End If
                Next
            Next

            output.AddField(riga.Pioggia)

            If _av_cod = 87 Or _av_cod = 94 Then

                If riga.Temp_h_18 > -1000 Then
                    output.AddField(riga.Temp_h_18)
                Else
                    output.AddField(DBNull.Value)
                End If
                If riga.Temp_h_19 > -1000 Then
                    output.AddField(riga.Temp_h_19)
                Else
                    output.AddField(DBNull.Value)
                End If
                If riga.Temp_h_20 > -1000 Then
                    output.AddField(riga.Temp_h_20)
                Else
                    output.AddField(DBNull.Value)
                End If

            End If

            output.Commit()

        Next

        Return output.Output()
    End Function
    Private Function DescrizioneModello() As String

        Dim StrStadi() As String = _mdl.StrStadi()

        Dim str As String = ""
        Select Case _av_cod
            Case 87 'Verme delle pere e delle mele - Carpocapsa- Cydia Pomonella (*) Sverna come larva
                str = "Il modello è valido solo per le prime due generazioni del fitofago"
            Case 427 'Ricamatrice delle Pomacee - Pandemis Cerasana (*) Sverna come larva
                str = "Il modello è valido per tutte le generazioni del fitofago"
            Case 107 'Eulia - Argyrotaenia Pulchellana (*) Sverna come pupa
                str = "Il modello è valido per tutte le generazioni del fitofago"
            Case 138 'Tignoletta della Vite - Lobesia Botrana (*) Sverna come pupa
                str = "Il modello è valido per tutte le generazioni del fitofago"
            Case 94 'Tignola Orientale del pesco - Cydia Molesta (*) Sverna come larva
                str = "Il modello è valido solo per le prime due generazioni del fitofago"
            Case 96 'Cidia del susino - Cydia Funebrana (*) Sverna come larva
                str = "Il modello è valido per tutte le generazioni del fitofago"
        End Select
        '(*) Considerando i parametri in tabella

        str &= "<br><br>"

        Dim NStadi As Integer = StrStadi.Length()

        Dim table(NStadi, 2) As String
        table(0, 0) = ""
        table(0, 1) = "Cumulo"
        table(0, 2) = "Presenza"
        For iStadio = 0 To NStadi - 1
            table(iStadio + 1, 0) = StrStadi(iStadio)
            Select Case iStadio
                Case 0 'UOVA
                    table(iStadio + 1, 1) = "Percentuale cumulativa di uova deposte"
                    table(iStadio + 1, 2) = "Percentuale di uova deposte e non ancora schiuse"
                Case 1 'LARVE
                    table(iStadio + 1, 1) = "Percentuale cumulativa di larve nate"
                    table(iStadio + 1, 2) = "Percentuale di larve nate e non ancora incrisalidate"
                Case 2 'PUPE
                    table(iStadio + 1, 1) = "Percentuale cumulativa di pupe formate"
                    table(iStadio + 1, 2) = "Percentuale di pupe formate e non ancora sfarfallate"
                Case 3 'ADULTI
                    table(iStadio + 1, 1) &= "Percentuale cumulativa di adulti sfarfallati"
                    table(iStadio + 1, 2) &= "Percentuale di adulti sfarfallati non ancora morti"
            End Select
        Next

        str &= "<table>"
        For r = 0 To NStadi
            str &= "<tr>"
            For c = 0 To 2
                str &= "<td style='" & If(c < 2, "padding:5px 10px 5px 5px;", "padding:5px;") & If(c = 0, " text-align:right;", "") & If(c = 0 Or r = 0, " font-weight:bold;", "") & "'>"
                str &= table(r, c)
                str &= "</td>"
            Next
            str &= "</tr>"
        Next
        str &= "</table>"

        Return str
    End Function
End Class





#If False Then


Public Class RitardoVariabile_

    'versione precedente a base 1
    'Private Const S_UOVA As Integer = 1        
    'Private Const S_ADULTI As Integer = 4    
    'Private Const S_DEPOSITO As Integer = 6
    'Private Const Sottostadi As Integer = 99
    'Private Const NESSUN_LIMITE = 101
    'Private Const V_CUMULATI = 1
    'Private Const V_PRESENZE = 2

    Private Const S_UOVA As Integer = 0
    Private Const S_LARVE As Integer = 1
    Private Const S_PUPE As Integer = 2
    Private Const S_ADULTI As Integer = 3
    Private Const S_IBERNANTE As Integer = 5
    Private Const S_DEPOSITO As Integer = 5
    Private Const Sottostadi As Integer = 98

    ' a Ritardo Variabile
    Private SotStIni, StIni As Integer           ' Sottostadio e Stadio di partenza
    Private ParLogan(0 To 6, 0 To S_DEPOSITO) As Double ' parametri curva di Logan
    Private k(0 To S_DEPOSITO) As Integer               ' valori di K per ogni stadio
    Private Bieri(0 To 2) As Double              ' parametri curva di Bieri
    Private rOptAd As Double                     ' tasso ottimale sviluppo adulti
    Private UovaFem As Double                    ' numero totale di uova deposte da una femmina
    Private Flusso(0 To Sottostadi, 0 To S_ADULTI)
    Private MassaQ(0 To Sottostadi, 0 To S_ADULTI)
    Private Delay(0 To S_ADULTI) As Double
    Private DelayPrec(0 To S_ADULTI) As Double
    Private Attrito(0 To S_ADULTI) As Double
    Private TotStad(0 To S_ADULTI) As Double
    Private Ingresso(0 To S_ADULTI) As Double
    Private Uscita(0 To S_ADULTI) As Double
    Private GioStad(0 To S_ADULTI) As Double
    Private CumStad(0 To S_ADULTI) As Double
    Private TotUovaGior As Double
    Private TotUovaDeltaT As Double
    Private TotUova As Double
    Private ValoreIngresso As Double
    Private DeltaT As Double
    Private PrimoDeltaT As Boolean
    Private GenerazioneIbernante As Boolean
    ' a ritardo variabile
    Public Cumulo(0 To S_ADULTI, 0 To 370) As Double                'Public solo per DEBUG
    Public Presenza(0 To S_ADULTI, 0 To 370) As Double              'Public solo per DEBUG
    Public Generazione(0 To S_ADULTI, 0 To 370) As Integer          'Public solo per DEBUG

    Private InizioPeriodo As Date
    Private FinePeriodo As Date


    Private Meteo As AgronicaCoreMeteoBiz.InterfacciaMeteo


    Private _TipoDebug As AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoDebug

    Public Sub New()

        TipoDebug = TipiEnumerativi.enum_TipoDebug.Off

    End Sub

    Public Property TipoDebug As TipiEnumerativi.enum_TipoDebug
        Get
            Return _TipoDebug
        End Get
        Set
            _TipoDebug = Value
        End Set
    End Property

    Public Function CalcolaRitardoVariabile(DataInizio As Date, DataFine As Date, ByVal ModelliPrevisionali_InputData_Meteo As String, Av_Cod As Long, Veg_Cod As Long, Classe As String, Tipo_Cod As Integer, objParametri_Server As AgronicaCoreParametri) As AgronicaCoreVarieBIZ.rispostaStandard(Of cRisultatoModello)

        '  Flusso (i) e' il valore del flusso che esce dal compartimento i al tempo t
        '  MassaQ (i) e' il valore del contenuto del compartimento i al tempo t
        '  k (s) e' il numero di compartimenti (sottostadi) dello stadio s
        '  DeltaT e' la lunghezza in giorni del passo DeltaT
        '  Delay e' il valore di Del (Delay) del passo di tempo DeltaT attuale
        '  DelayPrec e' il valore di Del del passo di tempo DeltaT precedente
        '  Attrito e' il valore momentaneo dell'attrito
        '  TotStad: N. di individui di uno stadio che restano nel processo
        '  CumStad: N. di individui che hanno raggiunto uno stadio (compresi quelli che ne sono gia' usciti)
        '  ValoreIngresso sono i valori dell'input durante il primo DeltaT
        '  Ingresso e' il valore dell'ingresso nel processo per unita' di tempo
        '  Uscita e' il valore dell'uscita dal processo per unita' di tempo
        '  xg e' il numero del giorno in cui si svolge la simulazione

        Dim xg As Integer

        'clessidra durante l'attesa


        'Open "c:\mode\mete\gdebug" For Output Access Write As #1


        ' VAnni: 3/4/2017: qualsiasi periodo imposto la partenza sui inizio anno.
        InizioPeriodo = DateSerial(Year(DataInizio), 1, 1)
        DataInizio = InizioPeriodo

        Dim rval As New rispostaStandard(Of cRisultatoModello)
        rval.RispostaStringa = New cRisultatoModello()

        Meteo = New AgronicaCoreMeteoBiz.InterfacciaMeteo(TipiEnumerativi.enum_MeteoLetturaDati.Entrambi, DataInizio, DataFine, objParametri_Server, ModelliPrevisionali_InputData_Meteo)

        ParametriRitardoVariabile(Av_Cod, Veg_Cod, Classe, Tipo_Cod, objParametri_Server, rval)

        FinePeriodo = Meteo.UltimaDataOrari

        For xg = DateToGiuliano(InizioPeriodo) To DateToGiuliano(FinePeriodo)
            GiornoRitardoVariabile(xg)
        Next


        'vModelli.ToolBar.Tools("ID_RitardoVariabile").Visible = True
        'vModelli.ToolBar.Tools("ID_Uova").State = ssChecked

        'GraficoRitardoVariabile Av_Cod, StIni
        'TabellaRitardoVariabile Av_Cod
        'Close #1

        'GraficoRitardoVariabile Av_Cod, S_UOVA

        Dim DT As DataTable = TabellaRitardoVariabile(Av_Cod)

        rval.RispostaOK = True
        rval.RispostaStringa.Modello_Tabella1 = JSON_RitardoVariabile(DT, Av_Cod)
        rval.RispostaStringa.Modello_Descrizione = RisultatiRitardoVariabile(Av_Cod, S_UOVA)

        Return rval

    End Function

    Private Sub GiornoRitardoVariabile(xg As Integer)
        Dim Funzione As Double
        Dim Temp As Double
        Dim Stadio As Integer
        Dim i As Integer
        Dim passo, yr As Integer
        Dim d_o As AgronicaCoreMeteoBiz.clDatiOrari


        TotUovaGior = 0
        GioStad(S_UOVA) = 0
        GioStad(S_LARVE) = 0
        GioStad(S_PUPE) = 0
        GioStad(S_ADULTI) = 0

        For passo = 0 To 23
            ' passo biorario
            '  yr = passo * 2
            '  Temp = 0.5 * (Meteo.Orari(xg, yr).Temp + Meteo.Orari(xg, yr+1).Temp)
            yr = passo
            d_o = Meteo.Orari(xg, yr)
            If d_o Is Nothing Then 'GABRIELE
                Exit For
            End If
            Temp = d_o.Temp

            'DEBUG_CRPV
            'Print #1, GiulianoToDate(xg) & ":" & Format(passo, "00 ") & TEMP_C_ORARIA(xg, passo) & "°C"

            For Stadio = S_UOVA To S_ADULTI
                Funzione = Logan(Temp, Stadio)

                If Funzione <= 0 Then Funzione = 0.001
                Delay(Stadio) = 1 / Funzione
                'Debug_CPRV
                'Print #FileNo, "   Del " & Stadio & " " & Delay(Stadio)

                Call Ritardo(Stadio)
                If Stadio = S_ADULTI Then
                    Call CalcolaMassaQ(Stadio)
                    Call NumUova(Stadio, Temp)
                    TotUovaGior = TotUovaGior + TotUovaDeltaT
                    TotUova = TotUova + TotUovaDeltaT
                End If ' Stadio

            Next Stadio

            If PrimoDeltaT Then   ' azzeramento dell'input iniziale
                Flusso(SotStIni, StIni) = 0
                PrimoDeltaT = False
            End If


            Ingresso(S_UOVA) = TotUovaDeltaT * 1 / DeltaT
            Ingresso(S_LARVE) = Uscita(S_UOVA)
            Ingresso(S_PUPE) = Uscita(S_LARVE)
            Ingresso(S_ADULTI) = Uscita(S_PUPE)

            CumStad(S_UOVA) = CumStad(S_UOVA) + Ingresso(S_UOVA) * DeltaT
            CumStad(S_LARVE) = CumStad(S_LARVE) + Uscita(S_UOVA) * DeltaT
            CumStad(S_PUPE) = CumStad(S_PUPE) + Uscita(S_LARVE) * DeltaT
            CumStad(S_ADULTI) = CumStad(S_ADULTI) + Uscita(S_PUPE) * DeltaT

            GioStad(S_UOVA) = GioStad(S_UOVA) + Ingresso(S_UOVA) * DeltaT
            GioStad(S_LARVE) = GioStad(S_LARVE) + Uscita(S_UOVA) * DeltaT
            GioStad(S_PUPE) = GioStad(S_PUPE) + Uscita(S_LARVE) * DeltaT
            GioStad(S_ADULTI) = GioStad(S_ADULTI) + Uscita(S_PUPE) * DeltaT

            'DEBUG_CRPV
            'Print #FileNo, "   Ingresso ", Ingresso(1), Ingresso(2), Ingresso(3), Ingresso(4)
            'Print #FileNo, "   Uscita ", Uscita(1), Uscita(2), Uscita(3), Uscita(4)
        Next passo

        For Stadio = S_UOVA To S_ADULTI
            Call CalcolaMassaQ(Stadio)
        Next Stadio


        ' Controllo fine stadio ibernante  ed eventuale ripristino
        ' parametri  Logan e k.  La fine  e' indicata  dalla  nuova
        ' produzione di individui dello stadio ibernante

        If ((GenerazioneIbernante = True) And (GioStad(StIni) > 0.0009)) Then
            For i = 0 To 6
                ' Ripristino Logan
                ParLogan(i, StIni) = ParLogan(i, S_DEPOSITO)
            Next i
            ' Ripristino k
            k(StIni) = k(S_DEPOSITO)
            GenerazioneIbernante = False
        End If

        For Stadio = S_UOVA To S_ADULTI

            ' calcolo della generazione e del cumulo
            Generazione(Stadio, xg) = Int(CumStad(Stadio))
            Cumulo(Stadio, xg) = 100 * (CumStad(Stadio) - Generazione(Stadio, xg))

            If Cumulo(Stadio, xg) < 1 Then
                Cumulo(Stadio, xg) = 0
            End If

            If Stadio = S_UOVA Then
                Generazione(Stadio, xg) = Generazione(Stadio, xg) + 1
            End If
            If ((Stadio = S_LARVE) Or (Stadio = S_PUPE)) And (StIni >= Stadio) Then
                Generazione(Stadio, xg) = Generazione(Stadio, xg) + 1
            End If
            If (Stadio = S_ADULTI) And (StIni = S_UOVA) Then
                Generazione(Stadio, xg) = Generazione(Stadio, xg) + 1
            End If
            If Cumulo(Stadio, xg) = 0 Then
                Generazione(Stadio, xg) = Generazione(Stadio, xg) - 1
            End If
            If Generazione(Stadio, xg) < 0 Then
                Generazione(Stadio, xg) = 0
            End If
            If (Stadio = S_ADULTI) And (StIni = S_ADULTI) And (CumStad(S_ADULTI) > 0) Then
                Generazione(Stadio, xg) = Generazione(Stadio, xg) + 1
            End If

            ' calcolo della presenza
            Presenza(Stadio, xg) = 100 * TotStad(Stadio)
            If Presenza(Stadio, xg) > 100 Then
                Presenza(Stadio, xg) = 100
            End If
            If Presenza(Stadio, xg) < 1 Then
                Presenza(Stadio, xg) = 0
            End If

        Next Stadio

    End Sub
    Private Function Logan(Temp As Double, Stadio As Integer) As Double
        Dim rAdulti As Double

        If Stadio = S_ADULTI Then   ' Per gli adulti si usa una retta
            If (Temp >= ParLogan(5, Stadio)) And (Temp <= ParLogan(4, Stadio)) Then
                rAdulti = ParLogan(1, Stadio) * Temp + ParLogan(2, Stadio)
            End If
            If Temp > ParLogan(4, Stadio) Then
                rAdulti = rOptAd
            End If
            If Temp < ParLogan(5, Stadio) Then
                rAdulti = 0.0001
            End If
            ' Quasi zero, ad evitare errori
            Return rAdulti

        Else ' Per uova, larve e pupe, si usa la funzione di Logan
            If (Temp > ParLogan(6, Stadio)) And (Temp < ParLogan(5, Stadio)) Then
                Return ParLogan(0, Stadio) *
                    (Exp(ParLogan(1, Stadio) * (Temp - ParLogan(4, Stadio))) _
                    - Exp(ParLogan(1, Stadio) * (ParLogan(3, Stadio) - ParLogan(4, Stadio)) _
                    - ParLogan(2, Stadio) * (ParLogan(3, Stadio) - Temp)))
            Else
                Return 0.0001
            End If
        End If

    End Function

    Private Sub NumUova(Stadio As Integer, Temp As Double)
        Dim i As Integer
        Dim DelOpt As Double
        Dim UovaDep As Double
        Dim Eta As Double

        DelOpt = 1 / rOptAd
        TotUovaDeltaT = 0

        ' VAnni: 3/4/2017: hack: riporto a base zero il ciclo.
        For i = 0 To k(Stadio)
            Eta = (i - 0.5) * DelOpt / k(Stadio)
            UovaDep = MassaQ(i, Stadio) / UovaFem * FertOpt(Eta) _
                   * (DelOpt / Delay(Stadio)) * DeltaT
            TotUovaDeltaT = TotUovaDeltaT + UovaDep

        Next i

    End Sub
    Private Function FertOpt(Eta As Double) As Double
        Dim Fertil As Double

        Fertil = (Bieri(0) * (Eta - Bieri(1))) _
                / (Exp(Log(Bieri(2)) * (Eta - Bieri(1))))

        If (Fertil < 0) Then
            Fertil = 0
        End If

        Return Fertil
        'Print #1, "Eta - Fert " & Eta & " " & FertOpt

    End Function
    Private Sub Ritardo(Stadio As Integer)
        Dim a, V, Deld, dr, deltadr As Double
        Dim i As Integer

        a = k(Stadio) * (DeltaT / Delay(Stadio))
        If a >= 1 Then
            '''MsgBox ("Stop: a e' > 1. Abbassare delta T o k")
            'Print #1, " k di ", Stadio, " - ", k(Stadio), " del ", Delay(Stadio)
        End If

        V = Ingresso(Stadio)
        Deld = ((Delay(Stadio) - DelayPrec(Stadio)) /
          (DeltaT * k(Stadio))) + Attrito(Stadio) * Delay(Stadio) / k(Stadio)

        DelayPrec(Stadio) = Delay(Stadio)

        For i = 0 To k(Stadio)
            dr = Flusso(i, Stadio)

            deltadr = dr + a * (V - dr * (1 + Deld))

            'if deltadr = Double.NaN Then
            '    deltadr = 0 
            'End If

            Flusso(i, Stadio) = deltadr
            If (deltadr > 0) Then
                '''Print #1, " dr > 0 ", dr, "(", i, ")"
            End If
            V = dr

            'Ritardo_Debug(i, Stadio, a, V, Deld, dr, deltadr)

        Next i

        Uscita(Stadio) = Flusso(k(Stadio), Stadio)

    End Sub

    Private Sub Ritardo_Debug(ByVal i As Integer, ByVal Stadio As Integer, ByVal a As Double, ByVal V As Double, ByVal Deld As Double, ByVal dr As Double, ByVal deltadr As Double)

        If TipoDebug = TipiEnumerativi.enum_TipoDebug.Verbose Then

            Debug_Write_Double(i, hyp:=vbTab)
            Debug_Write_Double(Stadio, hyp:=vbTab)
            Debug_Write_Double(dr, hyp:=vbTab)
            Debug_Write_Double(a, hyp:=vbTab)
            Debug_Write_Double(V, hyp:=vbTab)
            Debug_Write_Double(Deld, hyp:=vbTab)
            Debug_Write_Double(deltadr, hyp:="", aCapo:=vbCrLf)

        End If

    End Sub

    Private Sub CalcolaMassaQ(Stadio As Integer)
        Dim j As Integer

        TotStad(Stadio) = 0

        For j = 0 To k(Stadio)
            MassaQ(j, Stadio) = Flusso(j, Stadio) * (Delay(Stadio) / k(Stadio))
            TotStad(Stadio) = TotStad(Stadio) + MassaQ(j, Stadio)
        Next j

    End Sub

    Private Function ParametroGetList(ByVal CosaCerco As String, ByVal DT As DataTable, Optional ByVal ValoreDefault As String = "") As String

        Dim rVal As String = (
            From dd In DT.AsEnumerable
            Where dd("ParametroNome") = CosaCerco
            Select CStr(dd("ParametroValore"))
        ).FirstOrDefault()

        If String.IsNullOrEmpty(rVal) Then
            rVal = ValoreDefault
        Else
            rVal = rVal.Replace(".", Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator)
        End If

        Return rVal
    End Function

    Private Sub ParametriRitardoVariabile(Av_Cod As Long, Veg_Cod As Long, Classe As String, Tipo_Cod As Integer, ByVal objParametri_Server As AgronicaCoreParametri, ByRef rval As AgronicaCoreVarieBIZ.rispostaStandard(Of cRisultatoModello))

        Dim dtParametri As DataTable
        Dim xLeggi As New AgronicaCoreModelliPrevisionaliDAL.ParametriModelli_R

        ' VAnni: 30/3/2017: passo zero come da versione in VB6
        dtParametri = xLeggi.Leggi(Av_Cod, 0, Classe, "", "", objParametri_Server)

        Dim Tipo As String
        Dim TempMassimaPeriodo As Double = 0D

        If dtParametri.Rows.Count > 0 Then

            'Rs.FindFirst "Parametro ='partenzaSottostadio'"
            SotStIni = ParametroGetList("partenzaSottostadio", dtParametri)
            SotStIni -= 1 ' VAnni: 3/4/2017: HACK: gli indici sono tutti a base zero in .net

            'Rs.FindFirst "Parametro ='partenzaStadio'"
            StIni = ParametroGetList("partenzaStadio", dtParametri)
            StIni -= 1 ' VAnni: 3/4/2017: HACK: gli indici sono tutti a base zero in .net

            For s = 0 To 4
                Select Case s
                    Case 0
                        Tipo = "uovaLogan"
                    Case 1
                        Tipo = "larveLogan"
                    Case 2
                        Tipo = "pupeLogan"
                    Case 3
                        Tipo = "adultiLogan"
                    Case 4
                        Tipo = "svernantiLogan"
                End Select

                For n = 0 To 6

                    'Rs.FindFirst "Parametro ='" & Tipo & n & "'"
                    'ParLogan(n, s) = IIf(IsNull(Rs("Valore")), 0, Val(Valore(Rs("Valore"))))

                    ' VAnni: 3/4/2017: inidice = n + 1 perchè in .net sono a base zero, su db sono a base 1
                    ParLogan(n, s) = ParametroGetList(Tipo & (n + 1).ToString(), dtParametri, "0")

                    If TipoDebug = TipiEnumerativi.enum_TipoDebug.Verbose Then
                        Debug_Write_Double(ParLogan(n, s))
                    End If

                Next

            Next

            For n = 0 To 2
                Tipo = "tutticurva di Bieri" & (n + 1).ToString()
                'Rs.FindFirst "Parametro ='" & Tipo & "'"
                'Bieri(n) = IIf(IsNull(Rs("Valore")), 0, Val(Valore(Rs("Valore"))))

                Bieri(n) = ParametroGetList(Tipo, dtParametri, "0")

            Next n

            For n = 0 To 4
                Tipo = "tuttivalori di k" & (n + 1).ToString()

                'Rs.FindFirst "Parametro ='" & Tipo & "'"
                'k(n) = IIf(IsNull(Rs("Valore")), 0, Val(Valore(Rs("Valore"))))

                k(n) = ParametroGetList(Tipo, dtParametri, "0")

            Next n

            Tipo = "tuttitasso sviluppo"
            'Rs.FindFirst "Parametro ='" & Tipo & "'"
            'rOptAd = IIf(IsNull(Rs("Valore")), 0, Val(Valore(Rs("Valore"))))
            rOptAd = ParametroGetList(Tipo, dtParametri, "0")

            Tipo = "uovauova deposte"
            'Rs.FindFirst "Parametro ='" & Tipo & "'"
            'UovaFem = IIf(IsNull(Rs("Valore")), 0, Val(Valore(Rs("Valore"))))
            UovaFem = ParametroGetList(Tipo, dtParametri, "0")


            'GABRIELE lo fa dopo...
            '' Deposito dei valori dei parametri Logan e k dello stadio ibernante


            'For i = 0 To 6
            '    ParLogan(i, S_DEPOSITO) = ParLogan(i, StIni)  ' Deposito
            'Next i

            'k(S_DEPOSITO) = k(StIni)  ' Deposito

            '' Inizializzazione delle variabili

            'For i = S_UOVA To S_ADULTI

            '    For j = 0 To Sottostadi
            '        Flusso(j, i) = 0
            '        MassaQ(j, i) = 0
            '    Next j

            '    Ingresso(i) = 0
            '    Uscita(i) = 0
            '    Attrito(i) = 0
            '    Delay(i) = 0
            '    DelayPrec(i) = 0
            '    CumStad(i) = 0
            '    TotStad(i) = 0
            'Next i

            'TotUovaGior = 0
            'TotUova = 0
            'TotUovaDeltaT = 0

            'DeltaT = 1 / 24
            'ValoreIngresso = 1

            'GenerazioneIbernante = True
            'Flusso(SotStIni, StIni) = ValoreIngresso * 1 / DeltaT
            'PrimoDeltaT = True
            'TempMassimaPeriodo = 0


            '' Sostituzione dei parametri normali con quelli dello stadio ibernante

            'For i = 0 To 6
            '    ParLogan(i, StIni) = ParLogan(i, S_IBERNANTE)  ' Sostituzione
            'Next i
            'k(StIni) = k(S_IBERNANTE)  ' Sostituzione

        Else

            rval.Errore &= "Errore:Non ci sono parametri!"
            rval.RispostaOK = False


        End If





        '*****************************


        ' Deposito dei valori dei parametri Logan e k dello stadio ibernante

        For i = 0 To 6
            ParLogan(i, S_DEPOSITO) = ParLogan(i, StIni)  ' Deposito
        Next i

        k(S_DEPOSITO) = k(StIni)  ' Deposito

        ' Inizializzazione delle variabili

        For i = S_UOVA To S_ADULTI

            For j = 0 To Sottostadi
                Flusso(j, i) = 0
                MassaQ(j, i) = 0
            Next j

            Ingresso(i) = 0
            Uscita(i) = 0
            Attrito(i) = 0
            Delay(i) = 0
            DelayPrec(i) = 0
            CumStad(i) = 0
            TotStad(i) = 0
        Next i

        TotUovaGior = 0
        TotUova = 0
        TotUovaDeltaT = 0

        DeltaT = 1 / 24
        ValoreIngresso = 1

        GenerazioneIbernante = True
        Flusso(SotStIni, StIni) = ValoreIngresso * 1 / DeltaT
        PrimoDeltaT = True
        TempMassimaPeriodo = 0

        ' Sostituzione dei parametri normali con quelli dello stadio ibernante

        For i = 0 To 6
            ParLogan(i, StIni) = ParLogan(i, S_IBERNANTE)  ' Sostituzione
        Next i
        k(StIni) = k(S_IBERNANTE)  ' Sostituzione


    End Sub


    Private Sub TabellaRitardoVariabile_generaDT(ByVal DT As DataTable)

        DT.Columns.Add(New DataColumn("kendoKey", GetType(String)))

        DT.Columns.Add(New DataColumn("DataInfezione", GetType(Date)))

        DT.Columns.Add(New DataColumn("Uova_Generazione", GetType(String)))
        DT.Columns.Add(New DataColumn("Uova_Cumulo", GetType(Decimal)))
        DT.Columns.Add(New DataColumn("Uova_Presenza", GetType(Decimal)))

        DT.Columns.Add(New DataColumn("Larve_Generazione", GetType(String)))
        DT.Columns.Add(New DataColumn("Larve_Cumulo", GetType(Decimal)))
        DT.Columns.Add(New DataColumn("Larve_Presenza", GetType(Decimal)))

        DT.Columns.Add(New DataColumn("Pupe_Generazione", GetType(String)))
        DT.Columns.Add(New DataColumn("Pupe_Cumulo", GetType(Decimal)))
        DT.Columns.Add(New DataColumn("Pupe_Presenza", GetType(Decimal)))

        DT.Columns.Add(New DataColumn("Adulti_Generazione", GetType(String)))
        DT.Columns.Add(New DataColumn("Adulti_Cumulo", GetType(Decimal)))
        DT.Columns.Add(New DataColumn("Adulti_Presenza", GetType(Decimal)))

        DT.Columns.Add(New DataColumn("Pioggia", GetType(Decimal)))

        DT.Columns.Add(New DataColumn("Temperatura_Ore_15", GetType(Decimal)))
        DT.Columns.Add(New DataColumn("Temperatura_Ore_16", GetType(Decimal)))
        DT.Columns.Add(New DataColumn("Temperatura_Ore_18", GetType(Decimal)))
        DT.Columns.Add(New DataColumn("Temperatura_Ore_19", GetType(Decimal)))
        DT.Columns.Add(New DataColumn("Temperatura_Ore_20", GetType(Decimal)))

    End Sub

    Private Function TabellaRitardoVariabile(Av_Cod As Long) As DataTable

        Dim DT As New DataTable
        TabellaRitardoVariabile_generaDT(DT)

        Dim i As Long
        Dim Stadio As Long
        Dim Riga As Long
        Dim Seq As Integer


        Riga = 1

        For Stadio = 0 To 3
            i = DateToGiuliano(InizioPeriodo)
            Seq = 0
            Do
                If Seq < 2 And Format(Cumulo(Stadio, i), "##0") = 99 Then
                    Seq = Seq + 1
                ElseIf Seq = 2 And Format(Cumulo(Stadio, i), "##0") = 99 Then
                    Seq = 0
                    Cumulo(Stadio, i) = 100
                End If

                If Format(Cumulo(Stadio, i), "##0") = 100 Then
                    'elimina la sequenza di 99 0 100
                    i = i + 1
                    Do While i < DateToGiuliano(Meteo.UltimaDataOrari) And (Format(Cumulo(Stadio, i), "##0") = 100 Or Format(Cumulo(Stadio, i), "##0") = 99)
                        Cumulo(Stadio, i) = 0
                        i = i + 1
                    Loop
                End If
                i = i + 1

            Loop Until i > DateToGiuliano(Meteo.UltimaDataOrari) 'GABRIELE DateToGiuliano(Meteo.UltimaDataGiornalieri)
        Next

        Dim DataInfezione As DateTime = InizioPeriodo 'GABRIELE
        For i = DateToGiuliano(InizioPeriodo) To DateToGiuliano(Meteo.UltimaDataOrari)

            Riga = Riga + 1

            Dim Dr As DataRow = DT.NewRow

            Dr("kendoKey") = Riga

            Dr("DataInfezione") = DataInfezione.ToShortDateString  'GABRIELE CType (Meteo.Giornalieri(i).Data, datetime).ToShortDateString
            DataInfezione = DataInfezione.AddDays(1)

            Dr("Uova_Generazione") = StrGenerazione(Generazione(S_UOVA, i), S_UOVA)
            Dr("Uova_Cumulo") = Format(Cumulo(S_UOVA, i), "##0")
            Dr("Uova_Presenza") = Format(Presenza(S_UOVA, i), "##0")

            Dr("Larve_Generazione") = StrGenerazione(Generazione(S_LARVE, i), S_LARVE)
            Dr("Larve_Cumulo") = Format(Cumulo(S_LARVE, i), "##0")
            Dr("Larve_Presenza") = Format(Presenza(S_LARVE, i), "##0")

            Dr("Pupe_Generazione") = StrGenerazione(Generazione(S_PUPE, i), S_PUPE)
            Dr("Pupe_Cumulo") = Format(Cumulo(S_PUPE, i), "##0")
            Dr("Pupe_Presenza") = Format(Presenza(S_PUPE, i), "##0")

            Dr("Adulti_Generazione") = StrGenerazione(Generazione(S_ADULTI, i), S_ADULTI)
            Dr("Adulti_Cumulo") = Format(Cumulo(S_ADULTI, i), "##0")
            Dr("Adulti_Presenza") = Format(Presenza(S_ADULTI, i), "##0")

            Dim Pioggia As Decimal = 0
            For h = 0 To 23
                Dim d_o As AgronicaCoreMeteoBiz.clDatiOrari = Meteo.Orari(i, h)
                If d_o Is Nothing Then
                    Exit For
                End If
                Pioggia += d_o.Prec

                If Av_Cod = 87 Or Av_Cod = 94 Then

                    If h = 18 Then
                        Dr("Temperatura_Ore_18") = Format(Meteo.Orari(i, 18).Temp, "##0.0")
                    ElseIf h = 19 Then
                        Dr("Temperatura_Ore_19") = Format(Meteo.Orari(i, 19).Temp, "##0.0")
                    ElseIf h = 20 Then
                        Dr("Temperatura_Ore_20") = Format(Meteo.Orari(i, 20).Temp, "##0.0")
                    End If

                End If

            Next
            Dr("Pioggia") = Format(Pioggia, "##0")

            'GABRIELE Dr("Pioggia") = Format(Meteo.Giornalieri(i).Prec, "##0")
            'GABRIELE
            'If Av_Cod = 87 Or Av_Cod = 94 Then
            '    Dr("Temperatura_Ore_18") = Format(Meteo.Orari(i, 18).Temp, "##0.0")
            '    Dr("Temperatura_Ore_19") = Format(Meteo.Orari(i, 19).Temp, "##0.0")
            '    Dr("Temperatura_Ore_20") = Format(Meteo.Orari(i, 20).Temp, "##0.0")
            'End If

            DT.Rows.Add(Dr)

        Next

        Return DT

    End Function

    Private Function JSON_RitardoVariabile(ByVal DT As DataTable, av_Cod As Integer) As String

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)
        Dim c As ColonneNome

        ' VAnni: 15/2/2017: todo: verificare tutte le chiavi commentate (es: tariffa_cod per costi SBTF..)

        c = New ColonneNome("DataInfezione", "Data Infezione", "date")
        c._Filtrabile = True
        l.Add(c)

        c = New ColonneNome("Uova_Generazione", "Uova Gen", "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        l.Add(c)

        c = New ColonneNome("Uova_Cumulo", "Uova Cum", "number")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        l.Add(c)

        c = New ColonneNome("Uova_Presenza", "Uova Pre", "number")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        l.Add(c)



        c = New ColonneNome("Larve_Generazione", "Larve Gen", "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        l.Add(c)

        c = New ColonneNome("Larve_Cumulo", "Larve Cum", "number")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        l.Add(c)

        c = New ColonneNome("Larve_Presenza", "Larve Pre", "number")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        l.Add(c)



        c = New ColonneNome("Pupe_Generazione", "Pupe Gen", "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        l.Add(c)

        c = New ColonneNome("Pupe_Cumulo", "Pupe Cum", "number")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        l.Add(c)

        c = New ColonneNome("Pupe_Presenza", "Pupe Pre", "number")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        l.Add(c)



        c = New ColonneNome("Adulti_Generazione", "Adulti Gen", "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        l.Add(c)

        c = New ColonneNome("Adulti_Cumulo", "Adulti Cum", "number")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        l.Add(c)

        c = New ColonneNome("Adulti_Presenza", "Adulti Pre", "number")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        l.Add(c)


        c = New ColonneNome("Pioggia", "Pioggia", "number")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        l.Add(c)


        If av_Cod = 87 Or av_Cod = 94 Then

            c = New ColonneNome("Temperatura_Ore_18", "Temp Ore 18", "number")
            l.Add(c)
            c = New ColonneNome("Temperatura_Ore_19", "Temp Ore 19", "number")
            l.Add(c)
            c = New ColonneNome("Temperatura_Ore_20", "Temp Ore 20", "number")
            l.Add(c)

        End If


        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        js.Editabile_Deafault = False
        Dim risp As String = js.JSON_DataTable_Kendo(DT, l, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True, tipoFiltroKendo:=TipiEnumerativi.TipoFiltroKendo_colonne.CasellaTesto)


        Return risp


    End Function

    'Private Sub TabellaRitardoVariabile(Av_Cod As Long)
    '    Dim i As Long
    '    Dim Stadio As Long
    '    Dim Riga As Long
    '    Dim Seq As Integer




    '    If Av_Cod = 87 Or Av_Cod = 94 Then
    '        vModelli.MSFlexGrid1.Cols = 17
    '    Else
    '        vModelli.MSFlexGrid1.Cols = 14
    '    End If
    '    vModelli.MSFlexGrid1.Rows = 3
    '    vModelli.MSFlexGrid1.FixedRows = 2
    '    'Inserisco i titoli
    '    vModelli.MSFlexGrid1.MergeCells = flexMergeRestrictAll
    '    vModelli.MSFlexGrid1.MergeRow(0) = True
    '    vModelli.MSFlexGrid1.MergeCol(1) = True
    '    vModelli.MSFlexGrid1.MergeCol(2) = True
    '    vModelli.MSFlexGrid1.MergeCol(3) = True
    '    vModelli.MSFlexGrid1.MergeCol(4) = True
    '    vModelli.MSFlexGrid1.MergeCol(5) = True
    '    vModelli.MSFlexGrid1.MergeCol(6) = True
    '    vModelli.MSFlexGrid1.MergeCol(7) = True
    '    vModelli.MSFlexGrid1.MergeCol(8) = True
    '    vModelli.MSFlexGrid1.MergeCol(9) = True
    '    vModelli.MSFlexGrid1.MergeCol(10) = True
    '    vModelli.MSFlexGrid1.MergeCol(11) = True
    '    vModelli.MSFlexGrid1.MergeCol(12) = True
    '    vModelli.MSFlexGrid1.Row = 0
    '    vModelli.MSFlexGrid1.Col = 1
    '    vModelli.MSFlexGrid1.Text = "UOVA"
    '    vModelli.MSFlexGrid1.Col = 2
    '    vModelli.MSFlexGrid1.Text = "UOVA"
    '    vModelli.MSFlexGrid1.Col = 3
    '    vModelli.MSFlexGrid1.Text = "UOVA"
    '    vModelli.MSFlexGrid1.Col = 4
    '    vModelli.MSFlexGrid1.Text = "LARVE"
    '    vModelli.MSFlexGrid1.Col = 5
    '    vModelli.MSFlexGrid1.Text = "LARVE"
    '    vModelli.MSFlexGrid1.Col = 6
    '    vModelli.MSFlexGrid1.Text = "LARVE"
    '    vModelli.MSFlexGrid1.Col = 7
    '    vModelli.MSFlexGrid1.Text = "PUPE"
    '    vModelli.MSFlexGrid1.Col = 8
    '    vModelli.MSFlexGrid1.Text = "PUPE"
    '    vModelli.MSFlexGrid1.Col = 9
    '    vModelli.MSFlexGrid1.Text = "PUPE"
    '    vModelli.MSFlexGrid1.Col = 10
    '    vModelli.MSFlexGrid1.Text = "ADULTI"
    '    vModelli.MSFlexGrid1.Col = 11
    '    vModelli.MSFlexGrid1.Text = "ADULTI"
    '    vModelli.MSFlexGrid1.Col = 12
    '    vModelli.MSFlexGrid1.Text = "ADULTI"
    '    vModelli.MSFlexGrid1.Col = 13
    '    vModelli.MSFlexGrid1.Text = "METEO"
    '    If Av_Cod = 87 Or Av_Cod = 94 Then
    '        vModelli.MSFlexGrid1.Col = 14
    '        vModelli.MSFlexGrid1.Text = "METEO"
    '        vModelli.MSFlexGrid1.Col = 15
    '        vModelli.MSFlexGrid1.Text = "METEO"
    '        vModelli.MSFlexGrid1.Col = 16
    '        vModelli.MSFlexGrid1.Text = "METEO"
    '    End If

    '    vModelli.MSFlexGrid1.Row = 1
    '    For i = 0 To 16

    '        If Av_Cod = 87 Or Av_Cod = 94 Then
    '            vModelli.MSFlexGrid1.Col = i
    '        ElseIf i < 14 Then
    '            vModelli.MSFlexGrid1.Col = i
    '        End If

    '        Select Case i
    '            Case 0
    '                vModelli.MSFlexGrid1.ColWidth(i) = 980
    '                vModelli.MSFlexGrid1.Text = "Data"
    '            Case 1
    '                vModelli.MSFlexGrid1.ColWidth(i) = 450
    '                vModelli.MSFlexGrid1.Text = "Gen."
    '            Case 2
    '                vModelli.MSFlexGrid1.ColWidth(i) = 450
    '                vModelli.MSFlexGrid1.Text = "Cum."
    '            Case 3
    '                vModelli.MSFlexGrid1.ColWidth(i) = 450
    '                vModelli.MSFlexGrid1.Text = "Pre."
    '            Case 4
    '                vModelli.MSFlexGrid1.ColWidth(i) = 450
    '                vModelli.MSFlexGrid1.Text = "Gen."
    '            Case 5
    '                vModelli.MSFlexGrid1.ColWidth(i) = 450
    '                vModelli.MSFlexGrid1.Text = "Cum."
    '            Case 6
    '                vModelli.MSFlexGrid1.ColWidth(i) = 450
    '                vModelli.MSFlexGrid1.Text = "Pre."
    '            Case 7
    '                vModelli.MSFlexGrid1.ColWidth(i) = 450
    '                vModelli.MSFlexGrid1.Text = "Gen."
    '            Case 8
    '                vModelli.MSFlexGrid1.ColWidth(i) = 450
    '                vModelli.MSFlexGrid1.Text = "Cum."
    '            Case 9
    '                vModelli.MSFlexGrid1.ColWidth(i) = 450
    '                vModelli.MSFlexGrid1.Text = "Pre."
    '            Case 10
    '                vModelli.MSFlexGrid1.ColWidth(i) = 450
    '                vModelli.MSFlexGrid1.Text = "Gen."
    '            Case 11
    '                vModelli.MSFlexGrid1.ColWidth(i) = 450
    '                vModelli.MSFlexGrid1.Text = "Cum."
    '            Case 12
    '                vModelli.MSFlexGrid1.ColWidth(i) = 450
    '                vModelli.MSFlexGrid1.Text = "Pre."
    '            Case 13
    '                vModelli.MSFlexGrid1.ColWidth(i) = 650
    '                vModelli.MSFlexGrid1.Text = "Pioggia"
    '            Case 14

    '                If Av_Cod = 87 Or Av_Cod = 94 Then
    '                    vModelli.MSFlexGrid1.ColWidth(i) = 1050
    '                    vModelli.MSFlexGrid1.Text = "Temp.Ore 18"
    '                End If
    '            Case 15
    '                If Av_Cod = 87 Or Av_Cod = 94 Then
    '                    vModelli.MSFlexGrid1.ColWidth(i) = 1050
    '                    vModelli.MSFlexGrid1.Text = "Temp.Ore 19"
    '                End If
    '            Case 16
    '                If Av_Cod = 87 Or Av_Cod = 94 Then
    '                    vModelli.MSFlexGrid1.ColWidth(i) = 1050
    '                    vModelli.MSFlexGrid1.Text = "Temp.Ore 20"
    '                End If
    '        End Select
    '    Next

    '    vModelli.MSFlexGrid1.Rows = 1 + 2 + IIf(DateToGiuliano(Meteo.UltimaDataOrari) - DateToGiuliano(InizioPeriodo) < 0, 0, DateToGiuliano(Meteo.UltimaDataOrari) - DateToGiuliano(InizioPeriodo))


    '    Riga = 1

    '    For Stadio = 1 To 4
    '        i = DateToGiuliano(InizioPeriodo)
    '        Seq = 0
    '        Do
    '            If Seq < 3 And Format(Cumulo(Stadio, i), "##0") = 99 Then
    '                Seq = Seq + 1
    '            ElseIf Seq = 3 And Format(Cumulo(Stadio, i), "##0") = 99 Then
    '                Seq = 0
    '                Cumulo(Stadio, i) = 100
    '            End If

    '            If Format(Cumulo(Stadio, i), "##0") = 100 Then
    '                'elimina la sequenza di 99 0 100
    '                i = i + 1
    '                Do While i < DateToGiuliano(Meteo.UltimaDataOrari) And (Format(Cumulo(Stadio, i), "##0") = 100 Or Format(Cumulo(Stadio, i), "##0") = 99)
    '                    Cumulo(Stadio, i) = 0
    '                    i = i + 1
    '                Loop
    '            End If
    '            i = i + 1

    '        Loop Until i > DateToGiuliano(Meteo.UltimaDataGiornalieri)
    '    Next

    '    For i = DateToGiuliano(InizioPeriodo) To DateToGiuliano(Meteo.UltimaDataOrari)
    '        Riga = Riga + 1
    '        vModelli.MSFlexGrid1.Row = Riga
    '        vModelli.MSFlexGrid1.Col = 0
    '        vModelli.MSFlexGrid1.Text = Format(Meteo.Giornalieri(i).Data, "dd/mm/yyyy")
    '        vModelli.MSFlexGrid1.Col = 1
    '        vModelli.MSFlexGrid1.Text = StrGenerazione(Generazione(S_UOVA, i), S_UOVA)
    '        vModelli.MSFlexGrid1.Col = 2
    '        vModelli.MSFlexGrid1.Text = Format(Cumulo(S_UOVA, i), "##0")
    '        vModelli.MSFlexGrid1.Col = 3
    '        vModelli.MSFlexGrid1.Text = Format(Presenza(S_UOVA, i), "##0")
    '        vModelli.MSFlexGrid1.Col = 4
    '        vModelli.MSFlexGrid1.Text = StrGenerazione(Generazione(S_LARVE, i), S_LARVE)
    '        vModelli.MSFlexGrid1.Col = 5
    '        vModelli.MSFlexGrid1.Text = Format(Cumulo(S_LARVE, i), "##0")
    '        vModelli.MSFlexGrid1.Col = 6
    '        vModelli.MSFlexGrid1.Text = Format(Presenza(S_LARVE, i), "##0")
    '        vModelli.MSFlexGrid1.Col = 7
    '        vModelli.MSFlexGrid1.Text = StrGenerazione(Generazione(S_PUPE, i), S_PUPE)
    '        vModelli.MSFlexGrid1.Col = 8
    '        vModelli.MSFlexGrid1.Text = Format(Cumulo(S_PUPE, i), "##0")
    '        vModelli.MSFlexGrid1.Col = 9
    '        vModelli.MSFlexGrid1.Text = Format(Presenza(S_PUPE, i), "##0")
    '        vModelli.MSFlexGrid1.Col = 10
    '        vModelli.MSFlexGrid1.Text = StrGenerazione(Generazione(S_ADULTI, i), S_ADULTI)
    '        vModelli.MSFlexGrid1.Col = 11
    '        vModelli.MSFlexGrid1.Text = Format(Cumulo(S_ADULTI, i), "##0")
    '        vModelli.MSFlexGrid1.Col = 12
    '        vModelli.MSFlexGrid1.Text = Format(Presenza(S_ADULTI, i), "##0")
    '        vModelli.MSFlexGrid1.Col = 13
    '        vModelli.MSFlexGrid1.Text = Format(Meteo.Giornalieri(i).Prec, "##0")
    '        If Av_Cod = 87 Or Av_Cod = 94 Then
    '            vModelli.MSFlexGrid1.Col = 14
    '            vModelli.MSFlexGrid1.Text = Format(Meteo.Orari(i, 18).Temp, "##0.0")
    '            vModelli.MSFlexGrid1.Col = 15
    '            vModelli.MSFlexGrid1.Text = Format(Meteo.Orari(i, 19).Temp, "##0.0")
    '            vModelli.MSFlexGrid1.Col = 16
    '            vModelli.MSFlexGrid1.Text = Format(Meteo.Orari(i, 20).Temp, "##0.0")
    '        End If
    '    Next


    'End Sub

    Private Function StrGenerazione(ByVal Valore As String, ByVal Stadio As Integer) As String

        Dim rval As String = ""

        Select Case CInt(Valore)
            Case 0
                rval = "SV"
            Case 1
                rval = "I"
            Case 2
                rval = "II"
            Case 3
                rval = "III"
            Case 4
                rval = "IV"
            Case 5
                rval = "V"
            Case 6
                rval = "VI"
            Case 7
                rval = "VII"
            Case 8
                rval = "VIII"
            Case 9
                rval = "IX"
            Case 10
                rval = "X"
        End Select

        Return rval
    End Function

    'Public Sub GraficoRitardoVariabile(Av_Cod, Stadio)
    '    Dim Titolo As String
    '    Dim Fonte As String
    '    Dim StadioDes As String
    '    Dim Interrompi As Boolean

    '    Titolo = Disease
    '    Select Case Meteo.TipoSorgente
    '        Case objmeteo.meteoStazioni
    '            Fonte = "Stazione:"
    '        Case objmeteo.meteoQuadranti
    '            Fonte = "Quadrante:"
    '        Case objmeteo.meteoCapannine
    '            Fonte = "Capannina:"
    '    End Select
    '    Select Case Stadio
    '        Case S_UOVA
    '            StadioDes = "Uova"
    '        Case S_LARVE
    '            StadioDes = "Larve"
    '        Case S_PUPE
    '            StadioDes = "Pupe"
    '        Case S_ADULTI
    '            StadioDes = "Adulti"
    '    End Select

    '    Titolo = Titolo & " - " & StadioDes
    '    vModelli.TchGrafico.RemoveAllSeries

    '    vModelli.TchGrafico.Header.Text.Clear

    '    vModelli.TchGrafico.Axis.Bottom.Title.Caption = ""
    '    vModelli.TchGrafico.Axis.Left.Title.Caption = ""
    '    vModelli.TchGrafico.Legend.Visible = True
    '    vModelli.TchGrafico.Legend.TextStyle = ltsPlain
    '    vModelli.TchGrafico.Aspect.View3D = False

    '    vModelli.TchGrafico.Axis.Left.Maximum = 100
    '    vModelli.TchGrafico.Axis.Left.Minimum = 0
    '    vModelli.TchGrafico.Axis.Left.Increment = 10
    '    vModelli.TchGrafico.Axis.Bottom.Maximum = DateToGiuliano(Meteo.UltimaDataOrari)
    '    vModelli.TchGrafico.Axis.Bottom.Minimum = DateToGiuliano(InizioPeriodo)

    '    vModelli.TchGrafico.Axis.Bottom.Labels.Angle = 270

    '    vModelli.TchGrafico.Axis.Left.Title.Caption = "%"
    '    vModelli.TchGrafico.Header.Text.Add "Modello Ritardo Variabile su " & Fonte & Meteo.Quadrante & " - " & Meteo.NomeFonte
    'vModelli.TchGrafico.Header.Text.Add Titolo


    'Select Case Av_Cod
    '        Case 87 'Verme delle pere e delle mele - Carpocapsa- Cydia Pomonella
    '            Select Case Stadio
    '                Case S_PUPE
    '                    vModelli.TchGrafico.AddSeries scLine
    '                vModelli.TchGrafico.Series(0).Marks.Visible = False
    '                    vModelli.TchGrafico.Series(0).VerticalAxis = aLeftAxis
    '                    GraficoSerie 0, Stadio, V_CUMULATI, 2, 10, vbBlue

    '            Case S_ADULTI
    '                    vModelli.TchGrafico.AddSeries scLine
    '                vModelli.TchGrafico.Series(0).Marks.Visible = False
    '                    vModelli.TchGrafico.Series(0).VerticalAxis = aLeftAxis
    '                    GraficoSerie 0, Stadio, V_CUMULATI, 2, 10, vbBlue
    '                vModelli.TchGrafico.AddSeries scVolume
    '                vModelli.TchGrafico.Series(1).Marks.Visible = False
    '                    vModelli.TchGrafico.Series(1).VerticalAxis = aLeftAxis
    '                    GraficoSerie 1, Stadio, V_PRESENZE, 2, 10, vbRed
    '            Case S_UOVA
    '                    vModelli.TchGrafico.AddSeries scLine
    '                vModelli.TchGrafico.Series(0).Marks.Visible = False
    '                    vModelli.TchGrafico.Series(0).VerticalAxis = aLeftAxis
    '                    GraficoSerie 0, Stadio, V_CUMULATI, 3, 10, vbBlue
    '                vModelli.TchGrafico.AddSeries scVolume
    '                vModelli.TchGrafico.Series(1).Marks.Visible = False
    '                    vModelli.TchGrafico.Series(1).VerticalAxis = aLeftAxis
    '                    GraficoSerie 1, Stadio, V_PRESENZE, 3, 10, vbRed
    '            Case S_LARVE
    '                    vModelli.TchGrafico.AddSeries scLine
    '                vModelli.TchGrafico.Series(0).Marks.Visible = False
    '                    vModelli.TchGrafico.Series(0).VerticalAxis = aLeftAxis
    '                    GraficoSerie 0, Stadio, V_CUMULATI, 3, 10, vbBlue
    '        End Select

    '        Case 427 'Ricamatrice delle Pomacee - Pandemis Cerasana
    '            Select Case Stadio
    '                Case S_PUPE
    '                    vModelli.TchGrafico.AddSeries scLine
    '                vModelli.TchGrafico.Series(0).Marks.Visible = False
    '                    vModelli.TchGrafico.Series(0).VerticalAxis = aLeftAxis
    '                    GraficoSerie 0, Stadio, V_CUMULATI, 1, NESSUN_LIMITE, vbBlue
    '            Case S_ADULTI
    '                    vModelli.TchGrafico.AddSeries scLine
    '                vModelli.TchGrafico.Series(0).Marks.Visible = False
    '                    vModelli.TchGrafico.Series(0).VerticalAxis = aLeftAxis
    '                    GraficoSerie 0, Stadio, V_CUMULATI, 1, NESSUN_LIMITE, vbBlue
    '                vModelli.TchGrafico.AddSeries scVolume
    '                vModelli.TchGrafico.Series(1).Marks.Visible = False
    '                    vModelli.TchGrafico.Series(1).VerticalAxis = aLeftAxis
    '                    GraficoSerie 1, Stadio, V_PRESENZE, 1, NESSUN_LIMITE, vbRed
    '            Case S_UOVA
    '                    vModelli.TchGrafico.AddSeries scLine
    '                vModelli.TchGrafico.Series(0).Marks.Visible = False
    '                    vModelli.TchGrafico.Series(0).VerticalAxis = aLeftAxis
    '                    GraficoSerie 0, Stadio, V_CUMULATI, 2, NESSUN_LIMITE, vbBlue
    '                vModelli.TchGrafico.AddSeries scVolume
    '                vModelli.TchGrafico.Series(1).Marks.Visible = False
    '                    vModelli.TchGrafico.Series(1).VerticalAxis = aLeftAxis
    '                    GraficoSerie 1, Stadio, 2, V_PRESENZE, NESSUN_LIMITE, vbRed
    '            Case S_LARVE
    '                    vModelli.TchGrafico.AddSeries scLine
    '                vModelli.TchGrafico.Series(0).Marks.Visible = False
    '                    vModelli.TchGrafico.Series(0).VerticalAxis = aLeftAxis
    '                    GraficoSerie 0, Stadio, V_CUMULATI, 2, NESSUN_LIMITE, vbBlue
    '        End Select
    '        Case 107 'Eulia - Argyrotaenia Pulchellana
    '            Select Case Stadio
    '                Case S_PUPE
    '                    vModelli.TchGrafico.AddSeries scLine
    '                vModelli.TchGrafico.Series(0).Marks.Visible = False
    '                    vModelli.TchGrafico.Series(0).VerticalAxis = aLeftAxis
    '                    GraficoSerie 0, Stadio, V_CUMULATI, 3, NESSUN_LIMITE, vbBlue
    '            Case S_ADULTI
    '                    vModelli.TchGrafico.AddSeries scLine
    '                vModelli.TchGrafico.Series(0).Marks.Visible = False
    '                    vModelli.TchGrafico.Series(0).VerticalAxis = aLeftAxis
    '                    GraficoSerie 0, Stadio, V_CUMULATI, 2, NESSUN_LIMITE, vbBlue
    '                vModelli.TchGrafico.AddSeries scVolume
    '                vModelli.TchGrafico.Series(1).Marks.Visible = False
    '                    vModelli.TchGrafico.Series(1).VerticalAxis = aLeftAxis
    '                    GraficoSerie 1, Stadio, V_PRESENZE, 2, NESSUN_LIMITE, vbRed
    '            Case S_UOVA
    '                    vModelli.TchGrafico.AddSeries scLine
    '                vModelli.TchGrafico.Series(0).Marks.Visible = False
    '                    vModelli.TchGrafico.Series(0).VerticalAxis = aLeftAxis
    '                    GraficoSerie 0, Stadio, V_CUMULATI, 3, NESSUN_LIMITE, vbBlue
    '                vModelli.TchGrafico.AddSeries scVolume
    '                vModelli.TchGrafico.Series(1).Marks.Visible = False
    '                    vModelli.TchGrafico.Series(1).VerticalAxis = aLeftAxis
    '                    GraficoSerie 1, Stadio, V_PRESENZE, 3, NESSUN_LIMITE, vbRed
    '            Case S_LARVE
    '                    vModelli.TchGrafico.AddSeries scLine
    '                vModelli.TchGrafico.Series(0).Marks.Visible = False
    '                    vModelli.TchGrafico.Series(0).VerticalAxis = aLeftAxis
    '                    GraficoSerie 0, Stadio, V_CUMULATI, 3, NESSUN_LIMITE, vbBlue
    '                vModelli.TchGrafico.AddSeries scVolume
    '                vModelli.TchGrafico.Series(1).Marks.Visible = False
    '                    vModelli.TchGrafico.Series(1).VerticalAxis = aLeftAxis
    '                    GraficoSerie 1, Stadio, V_PRESENZE, 3, NESSUN_LIMITE, vbRed
    '        End Select
    '        Case 138 'Tignoletta della Vite - Lobesia Botrana
    '            Select Case Stadio
    '                Case S_PUPE
    '                    vModelli.TchGrafico.AddSeries scLine
    '                vModelli.TchGrafico.Series(0).Marks.Visible = False
    '                    vModelli.TchGrafico.Series(0).VerticalAxis = aLeftAxis
    '                    GraficoSerie 0, Stadio, V_CUMULATI, 3, NESSUN_LIMITE, vbBlue
    '            Case S_ADULTI
    '                    vModelli.TchGrafico.AddSeries scLine
    '                vModelli.TchGrafico.Series(0).Marks.Visible = False
    '                    vModelli.TchGrafico.Series(0).VerticalAxis = aLeftAxis
    '                    GraficoSerie 0, Stadio, V_CUMULATI, 2, NESSUN_LIMITE, vbBlue
    '                vModelli.TchGrafico.AddSeries scVolume
    '                vModelli.TchGrafico.Series(1).Marks.Visible = False
    '                    vModelli.TchGrafico.Series(1).VerticalAxis = aLeftAxis
    '                    GraficoSerie 1, Stadio, V_PRESENZE, 2, NESSUN_LIMITE, vbRed
    '            Case S_UOVA
    '                    vModelli.TchGrafico.AddSeries scLine
    '                vModelli.TchGrafico.Series(0).Marks.Visible = False
    '                    vModelli.TchGrafico.Series(0).VerticalAxis = aLeftAxis
    '                    GraficoSerie 0, Stadio, V_CUMULATI, 3, NESSUN_LIMITE, vbBlue
    '                vModelli.TchGrafico.AddSeries scVolume
    '                vModelli.TchGrafico.Series(1).Marks.Visible = False
    '                    vModelli.TchGrafico.Series(1).VerticalAxis = aLeftAxis
    '                    GraficoSerie 1, Stadio, V_PRESENZE, 3, NESSUN_LIMITE, vbRed
    '            Case S_LARVE
    '                    vModelli.TchGrafico.AddSeries scLine
    '                vModelli.TchGrafico.Series(0).Marks.Visible = False
    '                    vModelli.TchGrafico.Series(0).VerticalAxis = aLeftAxis
    '                    GraficoSerie 0, Stadio, V_CUMULATI, 3, NESSUN_LIMITE, vbBlue
    '                vModelli.TchGrafico.AddSeries scVolume
    '                vModelli.TchGrafico.Series(1).Marks.Visible = False
    '                    vModelli.TchGrafico.Series(1).VerticalAxis = aLeftAxis
    '                    GraficoSerie 1, Stadio, V_PRESENZE, 3, NESSUN_LIMITE, vbRed
    '        End Select
    '        Case 94 'Tignola Orientale del pesco - Cydia Molesta
    '            Select Case Stadio
    '                Case S_PUPE
    '                    vModelli.TchGrafico.AddSeries scLine
    '                vModelli.TchGrafico.Series(0).Marks.Visible = False
    '                    vModelli.TchGrafico.Series(0).VerticalAxis = aLeftAxis
    '                    GraficoSerie 0, Stadio, V_CUMULATI, 2, 10, vbBlue
    '            Case S_ADULTI
    '                    vModelli.TchGrafico.AddSeries scLine
    '                vModelli.TchGrafico.Series(0).Marks.Visible = False
    '                    vModelli.TchGrafico.Series(0).VerticalAxis = aLeftAxis
    '                    GraficoSerie 0, Stadio, V_CUMULATI, 2, 10, vbBlue
    '                vModelli.TchGrafico.AddSeries scVolume
    '                vModelli.TchGrafico.Series(1).Marks.Visible = False
    '                    vModelli.TchGrafico.Series(1).VerticalAxis = aLeftAxis
    '                    GraficoSerie 1, Stadio, V_PRESENZE, 2, 10, vbRed
    '            Case S_UOVA
    '                    vModelli.TchGrafico.AddSeries scLine
    '                vModelli.TchGrafico.Series(0).Marks.Visible = False
    '                    vModelli.TchGrafico.Series(0).VerticalAxis = aLeftAxis
    '                    GraficoSerie 0, Stadio, V_CUMULATI, 3, 10, vbBlue
    '                vModelli.TchGrafico.AddSeries scVolume
    '                vModelli.TchGrafico.Series(1).Marks.Visible = False
    '                    vModelli.TchGrafico.Series(1).VerticalAxis = aLeftAxis
    '                    GraficoSerie 1, Stadio, V_PRESENZE, 3, 10, vbRed
    '            Case S_LARVE
    '                    vModelli.TchGrafico.AddSeries scLine
    '                vModelli.TchGrafico.Series(0).Marks.Visible = False
    '                    vModelli.TchGrafico.Series(0).VerticalAxis = aLeftAxis
    '                    GraficoSerie 0, Stadio, V_CUMULATI, 3, 10, vbBlue
    '        End Select
    '        Case 96 'Cidia del susino - Cydia Funebrana
    '            Select Case Stadio
    '                Case S_PUPE
    '                    vModelli.TchGrafico.AddSeries scLine
    '                vModelli.TchGrafico.Series(0).Marks.Visible = False
    '                    vModelli.TchGrafico.Series(0).VerticalAxis = aLeftAxis
    '                    GraficoSerie 0, Stadio, V_CUMULATI, 2, NESSUN_LIMITE, vbBlue
    '            Case S_ADULTI
    '                    vModelli.TchGrafico.AddSeries scLine
    '                vModelli.TchGrafico.Series(0).Marks.Visible = False
    '                    vModelli.TchGrafico.Series(0).VerticalAxis = aLeftAxis
    '                    GraficoSerie 0, Stadio, V_CUMULATI, 2, NESSUN_LIMITE, vbBlue
    '                vModelli.TchGrafico.AddSeries scVolume
    '                vModelli.TchGrafico.Series(1).Marks.Visible = False
    '                    vModelli.TchGrafico.Series(1).VerticalAxis = aLeftAxis
    '                    GraficoSerie 1, Stadio, V_PRESENZE, 2, NESSUN_LIMITE, vbRed
    '            Case S_UOVA
    '                    vModelli.TchGrafico.AddSeries scLine
    '                vModelli.TchGrafico.Series(0).Marks.Visible = False
    '                    vModelli.TchGrafico.Series(0).VerticalAxis = aLeftAxis
    '                    GraficoSerie 0, Stadio, V_CUMULATI, 3, NESSUN_LIMITE, vbBlue
    '                vModelli.TchGrafico.AddSeries scVolume
    '                vModelli.TchGrafico.Series(1).Marks.Visible = False
    '                    vModelli.TchGrafico.Series(1).VerticalAxis = aLeftAxis
    '                    GraficoSerie 1, Stadio, V_PRESENZE, 3, NESSUN_LIMITE, vbRed
    '            Case S_LARVE
    '                    vModelli.TchGrafico.AddSeries scLine
    '                vModelli.TchGrafico.Series(0).Marks.Visible = False
    '                    vModelli.TchGrafico.Series(0).VerticalAxis = aLeftAxis
    '                    GraficoSerie 0, Stadio, V_CUMULATI, 3, NESSUN_LIMITE, vbBlue
    '        End Select
    '    End Select




    'End Sub

    'Private Sub GraficoSerie(Serie As Byte, Stadio As Variant, Valori As Byte, GenerazioneLimite As Byte, Limite As Double, Colore As Long)
    '    Dim Interrompi As Boolean
    '    Dim Dato As Double
    '    Dim DatoLimite As Double
    '    Dim Delta As Double

    '    vModelli.TchGrafico.Series(Serie).Color = Colore
    '    Select Case Stadio
    '        Case S_UOVA
    '            Select Case Serie
    '                Case 0
    '                    vModelli.TchGrafico.Series(Serie).Title = "Deposizione"
    '                Case 1
    '                    vModelli.TchGrafico.Series(Serie).Title = "Presenza"
    '            End Select
    '        Case S_LARVE
    '            Select Case Serie
    '                Case 0
    '                    vModelli.TchGrafico.Series(Serie).Title = "Nascita"
    '                Case 1
    '                    vModelli.TchGrafico.Series(Serie).Title = "Presenza"
    '            End Select
    '        Case S_PUPE
    '            Select Case Serie
    '                Case 0
    '                    vModelli.TchGrafico.Series(Serie).Title = "Impupamento"
    '                Case 1
    '                    vModelli.TchGrafico.Series(Serie).Title = "Presenza"
    '            End Select
    '        Case S_ADULTI
    '            Select Case Serie
    '                Case 0
    '                    vModelli.TchGrafico.Series(Serie).Title = "Sfarfallamento"
    '                Case 1
    '                    vModelli.TchGrafico.Series(Serie).Title = "Presenza"
    '            End Select
    '    End Select

    '    Interrompi = False
    '    For i = DateToGiuliano(InizioPeriodo) To DateToGiuliano(Meteo.UltimaDataOrari)
    '        DatoLimite = Cumulo(Stadio, i)
    '        Select Case Valori
    '            Case 1 'Cumulo
    '                Dato = Cumulo(Stadio, i)
    '                If i > DateToGiuliano(InizioPeriodo) Then
    '                    Delta = Abs(Cumulo(Stadio, i) - Cumulo(Stadio, i - 1))
    '                Else
    '                    Delta = 0
    '                End If
    '            Case 2 'Presenza
    '                Dato = Presenza(Stadio, i)
    '                If i > DateToGiuliano(InizioPeriodo) Then
    '                    Delta = Abs(Presenza(Stadio, i) - Presenza(Stadio, i - 1))
    '                Else
    '                    Delta = 0
    '                End If

    '        End Select

    '        If Dato > 0.1 And Generazione(Stadio, i) <= GenerazioneLimite And Delta < 80 Then

    '            If Generazione(Stadio, i) = GenerazioneLimite And DatoLimite > Limite Then
    '                Interrompi = True
    '            End If
    '            If Interrompi Then
    '                vModelli.TchGrafico.Series(Serie).AddNull Format(GiulianoToDate(i), "dd/mm")
    '        Else
    '                vModelli.TchGrafico.Series(Serie).Add Dato, Format(GiulianoToDate(i), "dd/mm"), Colore
    '        End If
    '        Else
    '            vModelli.TchGrafico.Series(Serie).AddNull Format(GiulianoToDate(i), "dd/mm")
    '    End If
    '    Next

    '    vModelli.TchGrafico.Legend.TextStyle = ltsPlain
    '    vModelli.TchGrafico.Legend.LegendStyle = lsSeries

    'End Sub

    Public Function RisultatiRitardoVariabile(Av_Cod, Stadio) As String
        Dim Testo As String

        Testo = "<DIV STYLE=""" & "color: red" & """>"
        Select Case Av_Cod
            Case 87 'Verme delle pere e delle mele - Carpocapsa- Cydia Pomonella
                Testo = "Il modello è valido solo per le prime due generazioni del fitofago <br>"
            Case 427 'Ricamatrice delle Pomacee - Pandemis Cerasana
                Testo = "Il modello è valido per tutte le generazioni del fitofago <br>"
            Case 107 'Eulia - Argyrotaenia Pulchellana
                Testo = "Il modello è valido per tutte le generazioni del fitofago <br>"
            Case 138 'Tignoletta della Vite - Lobesia Botrana
                Testo = "Il modello è valido per tutte le generazioni del fitofago <br>"
            Case 94 'Tignola Orientale del pesco - Cydia Molesta
                Testo = "Il modello è valido solo per le prime due generazioni del fitofago <br>"
            Case 96 'Cidia del susino - Cydia Funebrana
                Testo = "Il modello è valido per tutte le generazioni del fitofago <br>"
        End Select
        Testo = Testo & "<ul>"
        Select Case Stadio
            Case S_PUPE
                Testo = Testo & "<li>Cum = percentuale Cumulativa delle pupe formate </li>"
                Testo = Testo & "<li>Pre = percentuale pupe formate e non ancora sfarfallate</li>"
                Testo = Testo & "<li>Gen = Generazione in cui si trovano le pupe in quel momento (si riferisce a Cum)</li>"
            Case S_ADULTI
                Testo = Testo & "<li>Cum = percentuale Cumulativa degli adulti sfarfallati</li>"
                Testo = Testo & "<li>Pre = percentuale degli adulti sfarfallati non ancora morti</li>"
                Testo = Testo & "<li>Gen = Generazione in cui si trovano le uova in quel momento (si riferisce a Cum)</li>"
            Case S_UOVA
                Testo = Testo & "<li>Cum = percentuale Cumulativa delle uova deposte</li>"
                Testo = Testo & "<li>Pre = percentuale di uova deposte e non ancora schiuse</li>"
                Testo = Testo & "<li>Gen = Generazione in cui si trovano le uova in quel momento (si riferisce a Cum)</li>"
            Case S_LARVE
                Testo = Testo & "<li>Cum = percentuale Cumulativa delle larve nate</li>"
                Testo = Testo & "<li>Pre = percentuale delle larve nate e non ancora incrisalidate</li>"
                Testo = Testo & "<li>Gen = Generazione in cui si trovano le larve in quel momento (si riferisce a Cum)</li>"
        End Select
        Testo = Testo & "</ul></DIV>"
        'Evito l'errore bloccante. Il metodo non e' supportato da explorer 5


        Return Testo

    End Function



End Class

#End If