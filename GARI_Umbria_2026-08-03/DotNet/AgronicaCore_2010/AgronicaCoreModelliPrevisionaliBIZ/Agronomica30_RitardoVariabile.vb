
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreVarieBIZ
Imports System.Globalization
Imports System.IO
Imports System.Threading
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreModelliPrevisionaliBIZ



Public Class Agronomica30_RitardoVariabile
    Inherits AbstractModello

    Private ReadOnly mModCod As enum_ModelliPrevisionali
    Private ReadOnly mModello As Agronomica30_MRV

    Public Sub New(datiMeteo As MeteoReadOnlyList, Mod_Cod As Integer, lat As Decimal, lng As Decimal, zoneId As String)
        MyBase.New(datiMeteo)

        mModCod = Mod_Cod

        Select Case mModCod

            Case enum_ModelliPrevisionali.Agronomica30_MRV_Eulia
                'mModello = New Agronomica30_MRV_Eulia___OLD
                mModello = New Agronomica30_MRV_Eulia(lat, lng, zoneId)

            Case enum_ModelliPrevisionali.Agronomica30_MRV_CydiaMolesta
                'mModello = New Agronomica30_MRV_CydiaMolesta___OLD
                mModello = New Agronomica30_MRV_CydiaMolesta(lat, lng, zoneId)

            Case enum_ModelliPrevisionali.Agronomica30_MRV_Carpocapsa
                'mModello = New Agronomica30_MRV_Carpocapsa___OLD
                mModello = New Agronomica30_MRV_Carpocapsa(lat, lng, zoneId)

            Case enum_ModelliPrevisionali.Agronomica30_MRV_Helicoverpa
                'mModello = New Agronomica30_MRV_Helicoverpa___OLD
                mModello = New Agronomica30_MRV_Helicoverpa(lat, lng, zoneId)

            Case enum_ModelliPrevisionali.Agronomica30_MRV_Tignoletta
                'mModello = New Agronomica30_MRV_Tignoletta___OLD
                mModello = New Agronomica30_MRV_Tignoletta(lat, lng, zoneId)

            Case enum_ModelliPrevisionali.MRV_PiralideMais
                mModello = New Agronomica30_MRV_Piralide(lat, lng, zoneId)

            Case enum_ModelliPrevisionali.MRV_NottuaMais
                mModello = New Agronomica30_MRV_Nottua(lat, lng, zoneId)

            Case Else
                mModello = Nothing

        End Select

    End Sub

    Protected Overrides Function _elaboraModello() As cRisultatoModello

        If mModello Is Nothing Then

            Return Nothing
        End If

        Dim risModello As cRisultatoModello = Nothing

        Try

            Dim risElab = mModello.Elabora(_datiMeteo)

            If Not risElab Then

                _errore = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_RitardoVariabile_erroreDuranteApplicazioneModello
            Else

                risModello = mModello.CreaOutput()
            End If

        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            _errore = Gias.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

            risModello = Nothing
        End Try

        Return risModello
    End Function

    Protected Overrides Function _elaboraIndicatore_vecchio(dataInizio As Date, dataFine As Date) As cRisultatoModelloIndicatori.Indicatore

        Dim risIndic As New cRisultatoModelloIndicatori.Indicatore("...", dataInizio, dataFine)

        Dim flagOK As Boolean = False

        Try

            If mModello IsNot Nothing Then

                Dim risElab = mModello.Elabora(_datiMeteo)

                If risElab Then

                    Dim indic = mModello.ElaboraIndicatore()

                    If indic IsNot Nothing Then

                        Dim iStadio As Integer = -1
                        Dim offset As Decimal = 0
                        If indic.Stadi.Count > 1 AndAlso indic.Stadi(1).Valore > 0 Then
                            'Presenza Larve > 1% -> Rosso
                            iStadio = 1
                            offset = 200
                        Else
                            If indic.Stadi.Count > 0 AndAlso indic.Stadi(0).Valore > 0 Then
                                'Presenza Uova > 1% -> Giallo
                                iStadio = 0
                                offset = 100
                            Else
                                If indic.Stadi.Count > 3 AndAlso indic.Stadi(3).Valore > 0 Then
                                    iStadio = 3
                                End If
                            End If
                        End If

                        If iStadio >= 0 Then

                            flagOK = True
                            risIndic.Fill(offset + Math.Min(indic.Stadi(iStadio).Valore, 100), 300, {100, 200}, indic.Giorno, dataFine)
                            risIndic.AuxMsg = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_RitardoVariabile_presenzaDelloStadio_ & indic.Stadi(iStadio).Nome
                        Else

                            flagOK = True
                            risIndic.Fill(0, 300, {100, 200}, indic.Giorno, dataFine)
                            risIndic.AuxMsg = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_RitardoVariabile_nonPresentiIndividuiInStadioSignificativo

                            '_errore = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_RitardoVariabile_nonPresentiIndividuiInStadioSignificativo
                        End If
                    Else

                        _errore = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_RitardoVariabile_erroreDuranteApplicazioneModello
                    End If
                Else

                    _errore = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_RitardoVariabile_erroreDuranteApplicazioneModello
                End If
            End If

        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            _errore = Gias.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        If Not flagOK Then

            risIndic.Status = cRisultatoModelloIndicatori.Indicatore.enum_Status.Status_Error
            risIndic.StatusMsg = _errore
        End If

        Return risIndic
    End Function

    Protected Overrides Function _elaboraIndicatore(ByVal risElab As RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione) As RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione

        If mModello Is Nothing Then

            _errore = "Modello sconosciuto"
            risElab.Errore(_errore)
            Return risElab
        End If

        Try

            If Not mModello.Elabora(_datiMeteo) Then

                _errore = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_RitardoVariabile_erroreDuranteApplicazioneModello
                risElab.Errore(_errore)
                Return risElab
            End If

            Dim indic = mModello.ElaboraIndicatore()

            If indic Is Nothing Then

                _errore = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_RitardoVariabile_StadioSvernante
                risElab.Errore(_errore)
                Return risElab
            End If

            Dim offset As Decimal = 0
            Dim auxmsg As String = ""
            Dim iStadio As Integer = mModello.ValutaIndicatore(indic, offset, auxmsg)

            If iStadio < 0 Then

                '_errore = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_RitardoVariabile_nonPresentiIndividuiInStadioSignificativo
                'risElab.Errore(_errore)
                'Return risElab

                risElab.Fill(0, 300, {100, 200})
                risElab.AuxMsg = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_RitardoVariabile_nonPresentiIndividuiInStadioSignificativo
            Else

                risElab.Fill(offset + Math.Min(indic.Stadi(iStadio).Valore, 100), 300, {100, 200})
                risElab.AuxMsg = auxmsg
            End If


        Catch ex As Exception

            _errore = Gias.ErroreDuranteOperazione_ & vbCrLf &
                Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

            risElab.Errore(_errore)
        End Try

        Return risElab
    End Function


    'Public Function Calcola(ByVal datiMeteo As List(Of AgronicaCoreMeteoBiz.InterfacciaMeteoNT.MeteoDSS)) As rispostaStandard(Of cRisultatoModello)

    '    Dim r As New rispostaStandard(Of cRisultatoModello)
    '    r.RispostaOK = False
    '    r.Errore = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_RitardoVariabile_modelloPrevisionaleSconosciuto

    '    If mModello IsNot Nothing Then

    '        Try

    '            Dim risElab = mModello.Elabora(datiMeteo)

    '            If Not risElab Then

    '                r.Errore = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_RitardoVariabile_erroreDuranteApplicazioneModello

    '            Else

    '                r.RispostaOK = True
    '                r.RispostaStringa = mModello.CreaOutput()

    '            End If

    '        Catch ex As Exception

    '            'uso questa funzione per ottenere il Messaggio..:
    '            r.Errore = Gias.ErroreDuranteOperazione_ & vbCrLf &
    '                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

    '        End Try

    '    End If

    '    Return r
    'End Function

    'Public Function Calcola_Indicatore(ByVal datiMeteo As List(Of AgronicaCoreMeteoBiz.InterfacciaMeteoNT.MeteoDSS), ByVal DataInizio As DateTime, ByVal DataFine As DateTime) As rispostaStandard(Of cRisultatoModelloIndicatori.Indicatore)

    '    Dim r As New rispostaStandard(Of cRisultatoModelloIndicatori.Indicatore)
    '    r.RispostaStringa = New cRisultatoModelloIndicatori.Indicatore("...", DataInizio, DataFine)
    '    r.RispostaOK = False
    '    r.Errore = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_RitardoVariabile_modelloPrevisionaleSconosciuto

    '    Try

    '        If mModello IsNot Nothing Then

    '            Dim risElab = mModello.Elabora(datiMeteo)

    '            If risElab Then

    '                Dim indic = mModello.ElaboraIndicatore()

    '                If indic IsNot Nothing Then

    '                    Dim iStadio As Integer = -1
    '                    Dim offset As Decimal = 0
    '                    If indic.Stadi.Count > 1 AndAlso indic.Stadi(1).Valore > 0 Then
    '                        'Presenza Larve > 1% -> Rosso
    '                        iStadio = 1
    '                        offset = 200
    '                    Else
    '                        If indic.Stadi.Count > 0 AndAlso indic.Stadi(0).Valore > 0 Then
    '                            'Presenza Uova > 1% -> Giallo
    '                            iStadio = 0
    '                            offset = 100
    '                        Else
    '                            If indic.Stadi.Count > 3 AndAlso indic.Stadi(3).Valore > 0 Then
    '                                iStadio = 3
    '                            End If
    '                        End If
    '                    End If

    '                    If iStadio >= 0 Then

    '                        r.RispostaOK = True
    '                        r.RispostaStringa.Fill(offset + Math.Min(indic.Stadi(iStadio).Valore, 100), 300, {100, 200}, indic.Giorno, DataFine)
    '                        r.RispostaStringa.AuxMsg = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_RitardoVariabile_presenzaDelloStadio_ & indic.Stadi(iStadio).Nome
    '                    Else

    '                        r.Errore = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_RitardoVariabile_nonPresentiIndividuiInStadioSignificativo
    '                    End If
    '                Else

    '                    r.Errore = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_RitardoVariabile_erroreDuranteApplicazioneModello
    '                End If
    '            Else

    '                r.Errore = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_RitardoVariabile_erroreDuranteApplicazioneModello
    '            End If
    '        End If

    '    Catch ex As Exception

    '        'uso questa funzione per ottenere il Messaggio..:
    '        r.Errore = Gias.ErroreDuranteOperazione_ & vbCrLf &
    '                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

    '    End Try

    '    If Not r.RispostaOK Then

    '        r.RispostaStringa.Status = cRisultatoModelloIndicatori.Indicatore.enum_Status.Status_Error
    '        r.RispostaStringa.StatusMsg = r.Errore
    '    End If

    '    Return r
    'End Function

End Class



'**************************************************************************************************
'**************************************************************************************************
'**************************************************************************************************


Public MustInherit Class Agronomica30_MRV

    Private Class DatoGG
        Public Giorno As DateTime
        Public GenXStadi(,) As Decimal?

        Public Sub New(dt As DateTime, n_gen As Integer, n_stadi As Integer)
            Giorno = dt.Date
            ReDim GenXStadi(n_gen - 1, n_stadi - 1)
        End Sub
    End Class

    Private ReadOnly _id_algoritmo As Integer
    Private ReadOnly _dati_gg As List(Of DatoGG)
    Private _nGenerazioni As Integer
    Private _nStadi As Integer
    Protected _solcalc As SolarCalculator

    Public Sub New(id_alg As Integer, lat As Decimal, lng As Decimal, zoneId As String)
        _id_algoritmo = id_alg
        _dati_gg = New List(Of DatoGG)
        _nGenerazioni = 0
        _nStadi = 0
        _solcalc = New SolarCalculator(lat, lng, zoneId)
    End Sub


    Private Class ControlloPersistenza

        Private Class Controllo
            Public Inizio As DateTime
            Public Persistenza_gg As Integer
            Public Status As Integer
        End Class

        Private ReadOnly ListaXControllo As New List(Of List(Of Controllo))

        Public Sub New(n_gen As Integer, limiti As Integer())

            ListaXControllo = New List(Of List(Of Controllo))

            For i_gen = 0 To n_gen - 1

                Dim listXgen = New List(Of Controllo)

                ListaXControllo.Add(listXgen)

                For Each l In limiti

                    listXgen.Add(New Controllo With {.Inizio = Date.MinValue, .Persistenza_gg = l, .Status = -1})
                Next
            Next
        End Sub

        Public Function Persistenza(dt As DateTime, i_gen As Integer, i_stadio As Integer) As Boolean

            Dim ctrl = ListaXControllo(i_gen)(i_stadio)

            If ctrl.Status = 1 Then
                'Massima persistenza raggiunta
                Return False
            End If

            If ctrl.Status = 0 Then
                'Calcolo se deve persistere

                If DateDiff(DateInterval.Day, ctrl.Inizio, dt) < ctrl.Persistenza_gg Then

                    Return True
                End If

                ctrl.Status = 1
                Return False
            End If

            'Inizio a calcolare la persistenza
            ctrl.Status = 0
            ctrl.Inizio = dt

            Return True
        End Function
    End Class

    Public Function Elabora(ByVal datiMeteo As MeteoReadOnlyList) As Boolean

        '******************************************************************************************
        'ELABORAZIONE CON APPLICAZIONE ESTERNA (COMPILATO FORTRAN)
        '******************************************************************************************

        Dim exe_name As String = Configuration.ConfigurationManager.AppSettings("CartellaAgronomica30RitardoVariabileExe")
        exe_name = Path.Combine(exe_name, "Agronomica30_RitardoVariabile.exe")

        Dim target As String = Path.GetDirectoryName(exe_name) & "\Process"
        If Not Directory.Exists(target) Then
            Directory.CreateDirectory(target)
        End If
        Dim fname As String = "Agronomica30_RitardoVariabile_" & Path.GetRandomFileName()
        Dim input_file As String = Path.Combine(target, Path.ChangeExtension(fname, ".input"))
        Dim output_file As String = Path.Combine(target, Path.ChangeExtension(fname, ".output"))

        Dim in_file As New FileStream(input_file, FileMode.Create, FileAccess.Write)
        Dim writer As New StreamWriter(in_file)

        writeInputStream(datiMeteo, writer)

        writer.Close()

        'Start External Application...

        Dim myProcess As New Process() With {
            .EnableRaisingEvents = False
        }
        myProcess.StartInfo.UseShellExecute = False
        myProcess.StartInfo.RedirectStandardOutput = True
        myProcess.StartInfo.FileName = exe_name
        myProcess.StartInfo.Arguments = _id_algoritmo.ToString & " " & input_file & " " & output_file
        myProcess.Start()

        'output dell'applicazione esterna...
        Dim myProcess_output As String = myProcess.StandardOutput.ReadToEnd()

        'External Application Ended

        If myProcess.ExitCode <> 0 Then
            File.Delete(input_file)
            Return False
        End If

        Dim tries As Integer = 0
        Dim out_file As FileStream = Nothing

        'provo 10 volte
        While out_file Is Nothing AndAlso tries < 10

            If tries > 0 Then
                Thread.Sleep(500)
            End If

            Try

                out_file = New FileStream(output_file, FileMode.Open, FileAccess.Read, FileShare.None)

            Catch ex As Exception

                out_file = Nothing

            End Try

            tries += 1
        End While

        If out_file Is Nothing Then
            File.Delete(input_file)
            Return False
        End If

        Dim reader As New StreamReader(out_file)

        Dim line As String

        line = reader.ReadLine()
        _nGenerazioni = Integer.Parse(line.Trim, CultureInfo.InvariantCulture)

        line = reader.ReadLine()
        _nStadi = Integer.Parse(line.Trim, CultureInfo.InvariantCulture)

        line = reader.ReadLine()
        Dim stadioSvernante As Integer = Integer.Parse(line.Trim, CultureInfo.InvariantCulture) - 1

        Dim curr_t As Integer = 0
        Dim curr_dgg As New DatoGG(datiMeteo(curr_t).DataOra, _nGenerazioni, _nStadi)
        Dim first_dgg_valid As Boolean = False

        Dim limiti = (From l In Enumerable.Range(0, _nStadi)
                      Select PersistenzaStadio(l)).ToArray

        Dim _controlloPersistenza As New ControlloPersistenza(_nGenerazioni, limiti)

        While reader.Peek() >= 0

            line = reader.ReadLine()

            If curr_t < datiMeteo.Count Then

                Dim mdo = datiMeteo(curr_t)

                curr_t += 1

                If curr_dgg.Giorno.DayOfYear <> mdo.DataOra.DayOfYear Then

                    If first_dgg_valid Then

                        _dati_gg.Add(curr_dgg)
                    End If

                    curr_dgg = New DatoGG(mdo.DataOra.Date, _nGenerazioni, _nStadi)
                End If

                Dim aValues As Decimal() = Array.ConvertAll(line.Split({" "c}, StringSplitOptions.RemoveEmptyEntries), Function(s) Decimal.Parse(s.Trim(), CultureInfo.InvariantCulture))

                Dim i_v As Integer = 0

                For i_gen = 0 To _nGenerazioni - 1

                    For i_stadio = 0 To _nStadi - 1

                        'Per la prima genereazione considero solo gli stadi successivi a quello svernante
                        If i_gen > 0 OrElse i_stadio > stadioSvernante Then

                            Dim val As Decimal = Math.Round(aValues(i_v) * 10D) / 10D

                            If val > 0 Then

                                If val > 99.99 Then

                                    If _controlloPersistenza.Persistenza(curr_dgg.Giorno, i_gen, i_stadio) Then

                                        curr_dgg.GenXStadi(i_gen, i_stadio) = aValues(i_v)
                                        first_dgg_valid = True
                                    End If
                                Else

                                    curr_dgg.GenXStadi(i_gen, i_stadio) = aValues(i_v)
                                    first_dgg_valid = True
                                End If
                            End If
                        End If

                        i_v += 1
                    Next
                Next
            End If
        End While

        If first_dgg_valid Then

            _dati_gg.Add(curr_dgg)
        End If

        reader.Close()

        File.Delete(input_file)
        File.Delete(output_file)

        Return True
    End Function

    Public Function CreaOutput() As cRisultatoModello

        Dim output As New OutputModello

        If _dati_gg.Count > 0 Then

            Dim str_stadi As String() = StrStadi()

            output.aggiungiColonna("DataOra", GetType(DateTime), Gias.Data, "dd/MM/yyyy")

            For i_g = 1 To _nGenerazioni

                For i_s = 1 To _nStadi

                    output.aggiungiColonna("Gen_" + CStr(i_g) + "_Stadio_" + CStr(i_s),
                                           GetType(Decimal),
                                           str_stadi(i_s - 1),
                                           "0.0")._gruppoColonne = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_MRV_generazione & " " + If(i_g > 1, CStr(i_g - 1), My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_MRV_svernante)
                Next
            Next

            For Each dgg In _dati_gg

                output.AddField(dgg.Giorno)

                For i_g = 0 To _nGenerazioni - 1

                    For i_s = 0 To _nStadi - 1

                        If dgg.GenXStadi(i_g, i_s).HasValue Then

                            output.AddField(dgg.GenXStadi(i_g, i_s).Value)
                        Else

                            output.AddField(DBNull.Value)
                        End If
                    Next
                Next

                output.Commit()
            Next


            '    output.aggiungiColonna("DataOra", GetType(DateTime), "Data : ora", "dd/MM/yyyy : HH")
            '    For i_gen = 1 To ngen
            '        For i_s = 0 To mStadi.Count - 1
            '            Dim nome_colonna As String = "Gen_" + CStr(i_gen) + "_Stadio_" + CStr(i_s + 1)
            '            'Uova / Larve / Pupe / Adulti -> Raggruppo per Generazione, per Stadio o nessuno?
            '            Dim colonna As String = ""
            '            output.aggiungiColonna(nome_colonna, GetType(Decimal), colonna, "0.00")
            '        Next
            '    Next

            '    Dim sb_debug As New Text.StringBuilder
            '    For i_t = 0 To nt - 1

            '        Dim mdo = mDatiMeteo(i_t)

            '        output.AddField(mdo.DataOra)

            '        sb_debug.Append(mdo.DataOra.ToString("dd/MM/yyyy : HH"))

            '        For i_gen = 0 To ngen - 1

            '            For Each oStadio In mStadi

            '                output.AddField(oStadio.gen(i_gen).zf(i_t))

            '                Dim val As Decimal = oStadio.gen(i_gen).zf(i_t)
            '                val = Math.Round(val * 100000D) / 100000D
            '                Dim str As String = "         " + String.Format(Globalization.CultureInfo.InvariantCulture, "{0:0.00000}", val)
            '                sb_debug.Append(Right(str, 15))

            '            Next
            '        Next

            '        output.Commit()

            '        sb_debug.Append(vbCrLf)
            '    Next
        End If

        Dim risultatoModello As New cRisultatoModello
        risultatoModello.Modello_Tabella1 = output.Output()

        Return risultatoModello
    End Function

    Public Class Indicatore
        Public Giorno As DateTime
        Public Class Stadio
            Public Nome As String
            Public Valore As Decimal
        End Class
        Public Stadi As List(Of Stadio)
        Public Sub New()
            Stadi = New List(Of Stadio)
        End Sub
    End Class

    Public Function ElaboraIndicatore() As Indicatore

        If _dati_gg Is Nothing OrElse _dati_gg.Count = 0 Then
            Return Nothing
        End If

        Dim dgg As DatoGG = _dati_gg.Last

        Dim indic As New Indicatore With {
            .Giorno = dgg.Giorno
        }

        Dim stadi = StrStadi()
        For Each s In stadi
            indic.Stadi.Add(New Indicatore.Stadio With {
                            .Nome = s,
                            .Valore = 0
                            })
        Next

        For i_g = 0 To _nGenerazioni - 1

            For i_s = 0 To stadi.Length - 1

                If dgg.GenXStadi(i_g, i_s).HasValue Then

                    indic.Stadi(i_s).Valore += dgg.GenXStadi(i_g, i_s).Value
                End If
            Next
        Next

        Return indic
    End Function

    Public Overridable Function ValutaIndicatore(indic As Indicatore, ByRef offset As Decimal, ByRef auxmsg As String) As Integer

        Dim iStadio As Integer = -1
        offset = 0
        auxmsg = ""

        If indic.Stadi.Count > 1 AndAlso indic.Stadi(1).Valore > 0 Then
            'Presenza Larve > 1% -> Rosso
            iStadio = 1
            offset = 200
        Else
            If indic.Stadi.Count > 0 AndAlso indic.Stadi(0).Valore > 0 Then
                'Presenza Uova > 1% -> Giallo
                iStadio = 0
                offset = 100
            Else
                If indic.Stadi.Count > 3 AndAlso indic.Stadi(3).Valore > 0 Then
                    iStadio = 3
                End If
            End If
        End If

        If iStadio >= 0 Then

            auxmsg = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_RitardoVariabile_presenzaDelloStadio_ & indic.Stadi(iStadio).Nome
        End If

        Return iStadio
    End Function

    Protected Overridable Sub writeInputStream(datiMeteo As MeteoReadOnlyList, sw As StreamWriter)

        For Each dm In datiMeteo
            sw.WriteLine(dm.Temp.ToString("0.00000", CultureInfo.InvariantCulture))
        Next
    End Sub

    Protected Overridable Function StrStadi() As String()

        Dim defStadi As New List(Of String) From {
            My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_MRV_uova,
            My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_MRV_larve,
            My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_MRV_pupe,
            My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_MRV_adulti
        }

        For i_s = defStadi.Count To _nStadi - 1
            defStadi.Add("Stadio sconosciuto")
        Next

        Return defStadi.ToArray
    End Function

    Protected Overridable Function PersistenzaStadio(i_s As Integer) As Integer
        Return 7
    End Function

End Class


Public Class Agronomica30_MRV_Eulia
    Inherits Agronomica30_MRV

    Sub New(lat As Decimal, lng As Decimal, zoneId As String)
        MyBase.New(1, lat, lng, zoneId)
        'Argyrotaenia Pulchellana (Eulia)
    End Sub

End Class


Public Class Agronomica30_MRV_CydiaMolesta
    Inherits Agronomica30_MRV

    Sub New(lat As Decimal, lng As Decimal, zoneId As String)
        MyBase.New(2, lat, lng, zoneId)
        'Cydia Molesta
    End Sub

End Class


Public Class Agronomica30_MRV_Carpocapsa
    Inherits Agronomica30_MRV

    Sub New(lat As Decimal, lng As Decimal, zoneId As String)
        MyBase.New(3, lat, lng, zoneId)
        'Cydia Pomonella - Carpocapsa del Melo
    End Sub

    Protected Overrides Sub writeInputStream(datiMeteo As MeteoReadOnlyList, sw As StreamWriter)

        'scrivo i valori di temperatura, pioggia, umidità, bagnatura e ora tramonto

        Dim line As String
        Dim doy As Integer = -1
        Dim tramonto As Decimal = 0

        For Each dm In datiMeteo
            line = dm.Temp.ToString("0.00000", CultureInfo.InvariantCulture)
            line &= " " & dm.Prec.ToString("0.00000", CultureInfo.InvariantCulture)
            line &= " " & dm.UmRel.ToString("0.00000", CultureInfo.InvariantCulture)
            line &= " " & dm.Bagn.ToString("0.00000", CultureInfo.InvariantCulture)

            If dm.DataOra.DayOfYear <> doy Then
                doy = dm.DataOra.DayOfYear
                _solcalc.Calc(dm.DataOra.Date)
                tramonto = Math.Ceiling(_solcalc.Sunset)
            End If

            line &= " " & tramonto.ToString("0.00000", CultureInfo.InvariantCulture)

            sw.WriteLine(line)
        Next

    End Sub

End Class


Public Class Agronomica30_MRV_Helicoverpa
    Inherits Agronomica30_MRV

    Sub New(lat As Decimal, lng As Decimal, zoneId As String)
        MyBase.New(4, lat, lng, zoneId)
        'Helicoverpa Armigera - Nottula del pomodoro
    End Sub

End Class


Public Class Agronomica30_MRV_Tignoletta
    Inherits Agronomica30_MRV

    Sub New(lat As Decimal, lng As Decimal, zoneId As String)
        MyBase.New(5, lat, lng, zoneId)
        'Lobesia botrana (Tignoletta)
    End Sub

    Protected Overrides Sub writeInputStream(datiMeteo As MeteoReadOnlyList, sw As StreamWriter)

        'scrivo i valori di temperatura, pioggia, umidità, bagnatura, ora tramonto e durata del giorno

        Dim line As String
        Dim doy As Integer = -1
        Dim tramonto As Decimal = 0
        Dim dl As Decimal = 0

        For Each dm In datiMeteo
            line = dm.Temp.ToString("0.00000", CultureInfo.InvariantCulture)
            line &= " " & dm.Prec.ToString("0.00000", CultureInfo.InvariantCulture)
            line &= " " & dm.UmRel.ToString("0.00000", CultureInfo.InvariantCulture)
            line &= " " & dm.Bagn.ToString("0.00000", CultureInfo.InvariantCulture)


            If dm.DataOra.DayOfYear <> doy Then
                doy = dm.DataOra.DayOfYear
                _solcalc.Calc(dm.DataOra.Date)
                tramonto = Math.Ceiling(_solcalc.Sunset)
                dl = _solcalc.DayLength
            End If

            line &= " " & tramonto.ToString("0.00000", CultureInfo.InvariantCulture)
            line &= " " & dl.ToString("0.00000", CultureInfo.InvariantCulture)

            sw.WriteLine(line)
        Next

    End Sub

End Class


Public Class Agronomica30_MRV_Piralide
    Inherits Agronomica30_MRV

    Sub New(lat As Decimal, lng As Decimal, zoneId As String)
        MyBase.New(6, lat, lng, zoneId)
    End Sub

    Public Overrides Function ValutaIndicatore(indic As Indicatore, ByRef offset As Decimal, ByRef auxmsg As String) As Integer

        If indic.Stadi.Count > 1 AndAlso indic.Stadi(1).Valore > 10 Then

            'Larve X > 10% allerta rossa con avviso di inizio schiusa uova 

            offset = 200
            auxmsg = "Inizio schiusa uova"

            Return 1
        End If

        If indic.Stadi.Count > 0 AndAlso indic.Stadi(0).Valore > 10 Then

            'Uova X > 10% allerta rossa con avviso di ovodeposizione.

            offset = 200
            auxmsg = "Ovodeposizione"

            Return 0
        End If

        If indic.Stadi.Count > 3 AndAlso indic.Stadi(3).Valore > 95 Then

            'Adulti X > 95% allerta gialla con avviso di volo imminente
            offset = 100
            auxmsg = "Volo imminente"

            Return 3
        End If

        If indic.Stadi.Count > 1 AndAlso indic.Stadi(1).Valore > 50 Then

            'Larve X > 50% allerta gialla con avviso di monitoraggio presenza larve

            offset = 100
            auxmsg = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_RitardoVariabile_presenzaDelloStadio_ & indic.Stadi(1).Nome

            Return 1
        End If

        Return -1
    End Function
End Class


Public Class Agronomica30_MRV_Nottua
    Inherits Agronomica30_MRV

    Sub New(lat As Decimal, lng As Decimal, zoneId As String)
        MyBase.New(7, lat, lng, zoneId)
    End Sub

    Public Overrides Function ValutaIndicatore(indic As Indicatore, ByRef offset As Decimal, ByRef auxmsg As String) As Integer

        If indic.Stadi.Count > 1 AndAlso indic.Stadi(1).Valore > 50 Then

            'Larve X > 50% allerta rossa con avviso di presenza larve

            offset = 200
            auxmsg = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_RitardoVariabile_presenzaDelloStadio_ & indic.Stadi(1).Nome

            Return 1
        End If

        If indic.Stadi.Count > 1 AndAlso indic.Stadi(1).Valore > 1 Then

            'Larve X > 1% allerta rossa con avviso di inizio schiusa uova 

            offset = 200
            auxmsg = "Inizio schiusa uova"

            Return 1
        End If

        If indic.Stadi.Count > 3 AndAlso indic.Stadi(3).Valore > 50 Then

            'Adulti X > 50% allerta gialla con avviso per monitoraggio volo

            offset = 100
            auxmsg = "Monitoraggio volo"

            Return 3
        End If

        If indic.Stadi.Count > 0 AndAlso indic.Stadi(0).Valore > 10 Then

            'Uova X > 10% allerta gialla avviso di ovodeposizione.

            offset = 100
            auxmsg = "Ovodeposizione"

            Return 0
        End If

        Return -1
    End Function
End Class



'**************************************************************************************************
'**************************************************************************************************
'**************************************************************************************************



'Public Class testdll
'    <Runtime.InteropServices.DllImport("Agronomica30_RitardoVariabile.dll", CallingConvention:=Runtime.InteropServices.CallingConvention.Cdecl, CharSet:=Runtime.InteropServices.CharSet.Ansi)>
'    Public Shared Sub __dll_MOD_calcola(ByRef ns As Integer,
'                                             ByRef nws As Integer,
'                                             ByRef ngen As Integer,
'                                             ByRef nkmax As Integer,
'                                             ByRef nt As Integer,
'                                             ByRef v As Double,
'                                             ByRef zfi_ws As Double,
'                                             ByRef zf As Double)
'    End Sub

'End Class


Public MustInherit Class Agronomica30_MRV___OLD

    Private Class TriDSystemSolver
        'Struttura per soluzione sistema tridiagonale
        '   i vettori per le diagonali a(), b() e c() 
        '   hanno elementi a(i), b(i) e c(i) (con i da 2 a n-1) sempre uguali per cui è inutile un vettore per memorizzare tutti i valori
        '   servono variabili solo per il primo elemento, per l'elemento intermedio e per l'ultimo elemento
        Public a_0, a_i, a_n As Decimal 'sub-diagonal
        Public b_0, b_i, b_n As Decimal 'main diagonal
        Public c_0, c_i, c_n As Decimal 'sup-diagonal
        Public d_0, d_i, d_n As Decimal 'right part
        Private c1() As Decimal         'c-prime
        Private d1() As Decimal         'd-prime
        Public Sub New(ByVal n As Integer)
            ReDim c1(n - 1)
            ReDim d1(n - 1)
        End Sub
        Public Sub Solve(ByRef x() As Decimal, ByVal n As Integer)

            Dim m As Decimal

            'solve the linear system (Tridiagonal)
            c1(0) = c_0 / b_0
            d1(0) = d_0 / b_0

            For i = 1 To n - 2

                m = 1D / (b_i - a_i * c1(i - 1))
                d_i = x(i)
                c1(i) = c_i * m
                d1(i) = (d_i - a_i * d1(i - 1)) * m

            Next

            m = 1D / (b_n - a_n * c1(n - 2))
            c1(n - 1) = c_n * m
            d1(n - 1) = (d_n - a_n * d1(n - 2)) * m

            x(n - 1) = d1(n - 1)

            For i = n - 2 To 0 Step -1

                x(i) = d1(i) - c1(i) * x(i + 1)

            Next

        End Sub
    End Class

    Protected Class Generazione
        Public zfint() As Decimal
        Public zfi() As Decimal
        Public zf() As Decimal
        Public zfen As Decimal
        Public Sub New(ByVal nt As Integer, ByVal nkmax As Integer)

            ReDim zfint(nt - 1)
            ReDim zfi(nkmax - 1)
            ReDim zf(nt - 1)

            'inizializzo il vettore dei flussi zfi e le abbondanze zfen

            For i_t = 0 To nt - 1
                zfint(i_t) = 0
            Next

            zf(0) = 0
            zfen = 0

            For i_k = 0 To nkmax - 1
                zfi(i_k) = 0
            Next

        End Sub
    End Class

    Protected Class Stadio
        Public nt As Integer
        Public ngen As Integer
        Public nk As Integer
        Public gen() As Generazione
        Public deltx As Decimal
        Public deltx2 As Decimal
        Public zsig As Decimal
        Public v() As Decimal
        Public zm() As Decimal
        Public Sub New(ByVal nt_ As Integer, ByVal ngen_ As Integer, ByVal nk_ As Integer)

            nt = nt_
            ngen = ngen_
            nk = nk_

            ReDim gen(ngen)
            For i_gen = 0 To ngen
                gen(i_gen) = New Generazione(nt, nk)
            Next

            ReDim v(nt - 1)
            ReDim zm(nt - 1)

            'discretizzazione età fisiologica
            deltx = 1D / (nk - 0.5D)
            deltx2 = deltx * deltx
            zsig = 0.0001

        End Sub
    End Class

    Protected MustInherit Class FunSviluppo
        Public MustOverride Function Apply(ByVal temp As Decimal) As Decimal
    End Class
    Protected Class Briere_1
        Inherits FunSviluppo

        Private bcost As Decimal
        Private Tmin As Decimal
        Private Tmax As Decimal
        Public Sub New(ByVal _bcost As Decimal, ByVal _Tmin As Decimal, ByVal _Tmax As Decimal)
            bcost = _bcost
            Tmin = _Tmin
            Tmax = _Tmax
        End Sub
        Public Overrides Function Apply(ByVal temp As Decimal) As Decimal
            If Tmin <= temp And temp <= Tmax Then
                Return bcost * temp * (temp - Tmin) * Math.Sqrt(Tmax - temp)
            End If
            Return 0
        End Function
    End Class
    Protected Class Bieri_1
        Inherits FunSviluppo

        Private a As Decimal
        Private b As Decimal
        Private c As Decimal
        Private Tc As Decimal
        Public Sub New(ByVal _a As Decimal, ByVal _b As Decimal, ByVal _c As Decimal, ByVal _Tc As Decimal)
            a = _a
            b = _b
            c = _c
            Tc = _Tc
        End Sub
        Public Overrides Function Apply(ByVal temp As Decimal) As Decimal
            Dim result As Decimal = a * (temp - Tc) - Math.Pow(b, (temp - Tc - c))
            Return Math.Max(0D, result)
        End Function
    End Class
    Protected Class Lactin
        Inherits FunSviluppo

        Private rho As Decimal
        Private Tmax As Decimal
        Private Delta As Decimal
        Public Sub New(ByVal _rho As Decimal, ByVal _Tmax As Decimal, ByVal _Delta As Decimal)
            rho = _rho
            Tmax = _Tmax
            Delta = _Delta
        End Sub
        Public Overrides Function Apply(ByVal temp As Decimal) As Decimal
            If temp <= Tmax Then
                Return Math.Exp(rho * temp) - Math.Exp(rho * Tmax - (Tmax - temp) / Delta)
            End If
            Return 0
        End Function
    End Class

    Private Class DatoGG
        Public Giorno As DateTime
        Public GenXStadi(,) As Decimal?
        Public Sub New(ByVal dt As DateTime, ByVal n_gen As Integer, ByVal n_stadi As Integer)
            Giorno = dt.Date
            ReDim GenXStadi(n_gen - 1, n_stadi - 1)
        End Sub
    End Class

    Private mDatiGG As List(Of DatoGG)

    Public Sub New()
        mDatiGG = New List(Of DatoGG)
    End Sub

    Protected MustOverride Function ImpostaStadioSvernante(ByVal Stadi As List(Of Stadio)) As Stadio
    Protected MustOverride Function FV(ByVal iStadio As Integer) As FunSviluppo
    Protected Overridable Function NStadi() As Integer
        Return 4 'numero di stadi (uova, larve, pupe, adulti)
    End Function
    Protected Overridable Function StrStadi() As String()
        Return {
            My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_MRV_uova,
            My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_MRV_larve,
            My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_MRV_pupe,
            My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_MRV_adulti
        }
    End Function
    Protected Overridable Function PersistenzaStadio(i_s As Integer) As Integer
        Return 7
    End Function
    Protected Overridable Function NGenerazioni() As Integer
        Return 5 'numero di generazioni
    End Function
    Protected Overridable Function NSottostadi(ByVal i_s As Integer) As Integer
        Return 800 'costante per tutti gli stadi...
    End Function

    Public Function Elabora(ByVal datiMeteo As MeteoReadOnlyList) As Boolean

        Dim Stadi As New List(Of Stadio)

        Dim nt As Integer = datiMeteo.Count
        Dim ns As Integer = NStadi()
        Dim ngen As Integer = NGenerazioni()

        Dim fun As FunSviluppo

        For i_s = 0 To ns - 1

            Dim oStadio As New Stadio(nt, ngen, NSottostadi(i_s))

            Stadi.Add(oStadio)

            fun = FV(i_s)

            oStadio.v(0) = 0
            oStadio.zm(0) = 0
            For i_t = 1 To nt - 1
                oStadio.v(i_t) = fun.Apply(datiMeteo(i_t - 1).Temp)
                oStadio.zm(i_t) = 0
            Next

        Next

        Dim StadioSvernante As Stadio = ImpostaStadioSvernante(Stadi)

        Dim Exe_Fortran As Boolean = True
        If Exe_Fortran Then

            If Not ElaboraModello_FortranExe(Stadi, nt, StadioSvernante) Then
                Return False
            End If

        Else

            StadioSvernante.gen(0).zf(0) = 100D
            StadioSvernante.gen(0).zfint(0) = 0
            For i_k = 0 To StadioSvernante.nk - 1
                StadioSvernante.gen(0).zfint(0) += StadioSvernante.gen(0).zfi(i_k)
            Next
            StadioSvernante.gen(0).zfint(0) *= StadioSvernante.deltx

            'dati ogni ora
            Dim dt As Decimal = 1D / 24D

            If Not ElaboraModello(Stadi, nt, dt) Then
                Return False
            End If

        End If



        'Per lo stadio svernante imposto a 0 i valori di zf per la prima generazione per una "pulizia" della tabella risultato
        For i_t = 0 To nt - 1
            StadioSvernante.gen(0).zf(i_t) = 0
        Next

        Dim doy As Integer = datiMeteo(0).DataOra.DayOfYear

        Dim persist_gg As Integer(,)
        ReDim persist_gg(ngen - 1, Stadi.Count - 1)

        For i_t = 1 To nt - 1

            If i_t = nt - 1 OrElse doy <> datiMeteo(i_t + 1).DataOra.DayOfYear Then

                If i_t < nt - 1 Then

                    doy = datiMeteo(i_t + 1).DataOra.DayOfYear
                End If

                Dim mdo = datiMeteo(i_t)

                Dim dgg As New DatoGG(mdo.DataOra, ngen, Stadi.Count)

                Dim needAdd As Boolean = False

                For i_gen = 0 To ngen - 1
                    For i_s = 0 To Stadi.Count - 1

                        Dim val As Decimal = Math.Round(Stadi(i_s).gen(i_gen).zf(i_t) * 10D) / 10D
                        If val > 0 Then

                            If (val > 99.99) Then

                                If persist_gg(i_gen, i_s) < PersistenzaStadio(i_s) Then

                                    persist_gg(i_gen, i_s) += 1

                                    dgg.GenXStadi(i_gen, i_s) = Stadi(i_s).gen(i_gen).zf(i_t)
                                    needAdd = True
                                End If
                            Else

                                dgg.GenXStadi(i_gen, i_s) = Stadi(i_s).gen(i_gen).zf(i_t)
                                needAdd = True
                            End If
                        End If
                    Next
                Next

                If needAdd Then

                    mDatiGG.Add(dgg)
                End If
            End If
        Next

        'Dim iDbg As Integer = mDatiGG.Count - 1
        'While iDbg > 0

        '    Dim dgg_prec = mDatiGG(iDbg - 1)
        '    Dim dgg = mDatiGG(iDbg)

        '    For i_gen = 0 To ngen - 1
        '        For i_s = 0 To Stadi.Count - 1

        '            dgg.GenXStadi(i_gen, i_s) -= dgg_prec.GenXStadi(i_gen, i_s)

        '        Next
        '    Next

        '    iDbg -= 1
        'End While

        Return True
    End Function

    Private Function ElaboraModello(ByRef Stadi As List(Of Stadio), ByVal nt As Integer, ByVal dt As Decimal) As Boolean

        Dim ngen As Integer = NGenerazioni()

        Dim firstStadio As Stadio = Stadi.First()
        Dim lastStadio As Stadio = Stadi.Last()
        Dim prevStadio As Stadio

        Dim solver As New TriDSystemSolver(NSottostadi(-1))
        Dim sexr, pegg, zfi0, a, b, c As Decimal

        'ciclo sulle generazioni
        For i_gen = 0 To ngen - 1

            'ciclo sul tempo
            For i_t = 1 To nt - 1

                If (i_gen > 0) Then
                    If (firstStadio.gen(i_gen).zfint(i_t) > 0) Then
                        firstStadio.gen(i_gen).zfi(0) += firstStadio.gen(i_gen).zfint(i_t) / firstStadio.deltx
                    End If
                End If

                'ciclo sullo stadio
                prevStadio = Nothing
                For Each oStadio In Stadi

                    zfi0 = 0
                    If prevStadio Is Nothing Then

                        'produzione uova
                        sexr = 1 'sex-ratio
                        pegg = 0
                        For i_k = 0 To lastStadio.nk - 1
                            pegg += lastStadio.gen(i_gen).zfi(i_k)
                        Next
                        pegg *= (lastStadio.v(i_t) * lastStadio.deltx * sexr * dt)
                        oStadio.gen(i_gen + 1).zfen = pegg
                        oStadio.gen(i_gen + 1).zfint(i_t) += pegg

                    Else

                        'flusso da uno stadio al successivo
                        zfi0 = prevStadio.v(i_t) * prevStadio.gen(i_gen).zfi(prevStadio.nk - 1) * dt
                        oStadio.gen(i_gen).zfen = zfi0
                        zfi0 /= oStadio.deltx

                    End If

                    'soluzione del sistema di equazioni discretizzato

                    a = -(0.5D * oStadio.v(i_t) / oStadio.deltx + oStadio.zsig / oStadio.deltx2)
                    b = 2D * oStadio.zsig / oStadio.deltx2 + oStadio.zm(i_t)
                    c = -(-0.5D * oStadio.v(i_t) / oStadio.deltx + oStadio.zsig / oStadio.deltx2)

                    solver.a_0 = 0
                    solver.b_0 = 1D + (-a + oStadio.zm(i_t)) * dt
                    solver.c_0 = c * dt
                    solver.d_0 = oStadio.gen(i_gen).zfi(0) + zfi0
                    solver.a_i = a * dt
                    solver.b_i = 1D + b * dt
                    solver.c_i = c * dt
                    'solver.d_i = ...
                    solver.a_n = 2D * a * dt
                    solver.b_n = 1D + (b + oStadio.v(i_t) / oStadio.deltx) * dt
                    solver.c_n = 0
                    solver.d_n = oStadio.gen(i_gen).zfi(oStadio.nk - 1)

                    solver.Solve(oStadio.gen(i_gen).zfi, oStadio.nk)

                    prevStadio = oStadio
                Next

                prevStadio = Nothing
                For Each oStadio In Stadi

                    'calcolo le abbondanze in ogni stadio ad ogni tempo
                    oStadio.gen(i_gen).zfint(i_t) = 0
                    For i_k = 1 To oStadio.nk - 1
                        oStadio.gen(i_gen).zfint(i_t) += oStadio.gen(i_gen).zfi(i_k)
                    Next
                    oStadio.gen(i_gen).zfint(i_t) *= oStadio.deltx

                    'calcolo le cumulate in ogni stadio ad ogni tempo
                    If prevStadio Is Nothing Then 'primo stadio
                        oStadio.gen(i_gen + 1).zf(i_t) = oStadio.gen(i_gen + 1).zf(i_t - 1) + oStadio.gen(i_gen + 1).zfen
                    Else
                        'i_t parte da 1...
                        oStadio.gen(i_gen).zf(i_t) = oStadio.gen(i_gen).zf(i_t - 1) + oStadio.gen(i_gen).zfen
                        'If i_t = 0 Then
                        '    oStadio.gen(i_gen).zf(i_t) = oStadio.gen(i_gen).zfen
                        'Else
                        '    oStadio.gen(i_gen).zf(i_t) = oStadio.gen(i_gen).zf(i_t - 1) + oStadio.gen(i_gen).zfen
                        'End If
                    End If

                    prevStadio = oStadio
                Next

            Next
        Next

        For Each oStadio In Stadi

            'normalizzo a 100 le percentuali di individi negli stadi
            For i_gen = 0 To ngen - 1

                If oStadio.gen(i_gen).zf(nt - 1) > 95 Then 'se il flusso in uno stadio è superiore a 95

                    Dim norm100 As Decimal = 100D / oStadio.gen(i_gen).zf(nt - 1)

                    For i_t = 0 To nt - 1
                        oStadio.gen(i_gen).zf(i_t) *= norm100 'normalizzo a 100
                    Next

                End If

            Next
        Next

        Return True

    End Function

    Private Function ElaboraModello_FortranExe(ByRef Stadi As List(Of Stadio), ByVal nt As Integer, ByRef StadioSvernante As Stadio) As Boolean

        '******************************************************************************************
        'ELABORAZIONE CON APPLICAZIONE ESTERNA (COMPILATO FORTRAN)
        '******************************************************************************************

        Dim ns As Integer = Stadi.Count
        Dim nws As Integer = Stadi.IndexOf(StadioSvernante) + 1
        Dim ngen As Integer = NGenerazioni()
        Dim nkmax As Integer = NSottostadi(-1)



        ''TEST CON DLL COMPILATA IN FORTRAN
        ''I vettori passati alla dll sviluppata in fortran non possono essere di tipo Decimal 
        ''Le matrici hanno gli indici inverti 
        'Dim v(ns - 1, nt - 1) As Double
        'Dim zfi_ws(StadioSvernante.nk - 1) As Double
        'Dim zf((ngen * ns) - 1, nt - 1) As Double

        'For i_t = 0 To nt - 1
        '    For i_s = 0 To Stadi.Count - 1
        '        v(i_s, i_t) = Stadi(i_s).v(i_t)
        '    Next
        'Next
        'For i_k = 0 To StadioSvernante.nk - 1
        '    zfi_ws(i_k) = StadioSvernante.gen(0).zfi(i_k)
        'Next

        'testdll.__dll_MOD_calcola(ns, nws, ngen, nkmax, nt, v(0, 0), zfi_ws(0), zf(0, 0))



        Dim exe_name As String = Configuration.ConfigurationManager.AppSettings("CartellaAgronomica30RitardoVariabileExe")
        exe_name = Path.Combine(exe_name, "Agronomica30_RitardoVariabile.exe")

        Dim target As String = Path.GetDirectoryName(exe_name) & "\Process"
        If Not Directory.Exists(target) Then
            Directory.CreateDirectory(target)
        End If
        Dim fname As String = "Agronomica30_RitardoVariabile_" & Path.GetRandomFileName()
        Dim input_file As String = Path.Combine(target, Path.ChangeExtension(fname, ".input"))
        Dim output_file As String = Path.Combine(target, Path.ChangeExtension(fname, ".output"))

        Dim in_file As New FileStream(input_file, FileMode.Create, FileAccess.Write)
        Dim writer As New StreamWriter(in_file)
        writer.WriteLine(ns.ToString())
        writer.WriteLine(nws.ToString())
        writer.WriteLine(ngen.ToString())
        writer.WriteLine(nkmax.ToString())
        writer.WriteLine(nt.ToString())

        Dim sv As String

        For i_t = 0 To nt - 1
            sv = ""
            For Each oStadio In Stadi
                sv &= " " & oStadio.v(i_t).ToString("0.000000000000000", CultureInfo.InvariantCulture)
            Next
            writer.WriteLine(sv)
        Next

        For i_k = 0 To StadioSvernante.nk - 1
            writer.WriteLine(StadioSvernante.gen(0).zfi(i_k).ToString("0.000000000000000", CultureInfo.InvariantCulture))
        Next

        writer.Close()

        'Start External Application...

        Dim myProcess As Process = New Process()
        myProcess.EnableRaisingEvents = False
        myProcess.StartInfo.UseShellExecute = False
        myProcess.StartInfo.FileName = exe_name
        myProcess.StartInfo.Arguments = input_file & " " & output_file
        myProcess.StartInfo.RedirectStandardOutput = True
        myProcess.Start()

        'output dell'applicazione esterna...
        Dim myProcess_output As String = myProcess.StandardOutput.ReadToEnd()

        'External Application Ended

        If myProcess.ExitCode <> 0 Then
            File.Delete(input_file)
            Return False
        End If

        Dim tries As Integer = 0
        Dim out_file As FileStream = Nothing

        'provo 10 volte
        While out_file Is Nothing AndAlso tries < 10

            If tries > 0 Then
                Thread.Sleep(500)
            End If

            Try

                out_file = New FileStream(output_file, FileMode.Open, FileAccess.Read, FileShare.None)

            Catch ex As Exception

                out_file = Nothing

            End Try

            tries += 1
        End While

        If out_file Is Nothing Then
            File.Delete(input_file)
            Return False
        End If

        Dim reader As New StreamReader(out_file)

        Dim line As String
        Dim i_tt As Integer = 0
        While reader.Peek() >= 0
            line = reader.ReadLine()
            Dim values As Decimal() = Array.ConvertAll(line.Split({" "c}, StringSplitOptions.RemoveEmptyEntries), Function(s) Decimal.Parse(s.Trim(), CultureInfo.InvariantCulture))
            Dim i_v As Integer = 0
            For i_gen = 0 To ngen - 1
                For Each oStadio In Stadi
                    oStadio.gen(i_gen).zf(i_tt) = values(i_v)
                    i_v += 1
                Next
            Next
            i_tt += 1
        End While
        reader.Close()

        File.Delete(input_file)
        File.Delete(output_file)



        'Dim counter As Integer = 0
        'For i_gen = 0 To ngen - 1
        '    For i_s = 0 To ns - 1
        '        For i_t = 0 To nt - 1
        '            Dim diff = Stadi(i_s).gen(i_gen).zf(i_t) - CDec(zf(i_s + (i_gen * ns), i_t))
        '            If Math.Abs(diff) > 0.000001 Then
        '                counter += 1
        '            End If
        '            'Stadi(i_s).gen(i_gen).zf(i_t) = CDec(zf(i_t, i_s, i_gen))
        '        Next
        '    Next
        'Next



        Return True
    End Function

    Public Function CreaOutput() As cRisultatoModello

        Dim output As New OutputModello

        If mDatiGG.Count > 0 Then

            Dim str_stadi As String() = StrStadi()
            Dim ngen As Integer = NGenerazioni()
            Dim nstadi As Integer = StrStadi.Count

            output.aggiungiColonna("DataOra", GetType(DateTime), Gias.Data, "dd/MM/yyyy")

            For i_g = 1 To ngen
                For i_s = 1 To nstadi
                    output.aggiungiColonna("Gen_" + CStr(i_g) + "_Stadio_" + CStr(i_s),
                                       GetType(Decimal),
                                       str_stadi(i_s - 1),
                                       "0.0")._gruppoColonne = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_MRV_generazione & " " + If(i_g > 1, CStr(i_g - 1), My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_MRV_svernante)
                Next
            Next

            For Each dgg In mDatiGG

                output.AddField(dgg.Giorno)

                For i_g = 0 To ngen - 1
                    For i_s = 0 To nstadi - 1

                        If (dgg.GenXStadi(i_g, i_s).HasValue) Then

                            'output.AddField(Math.Round(dgg.GenXStadi(i_g, i_s).Value * 10D) / 10D)
                            output.AddField(dgg.GenXStadi(i_g, i_s).Value)
                        Else

                            output.AddField(DBNull.Value)
                        End If
                    Next
                Next

                output.Commit()
            Next


            '    output.aggiungiColonna("DataOra", GetType(DateTime), "Data : ora", "dd/MM/yyyy : HH")
            '    For i_gen = 1 To ngen
            '        For i_s = 0 To mStadi.Count - 1
            '            Dim nome_colonna As String = "Gen_" + CStr(i_gen) + "_Stadio_" + CStr(i_s + 1)
            '            'Uova / Larve / Pupe / Adulti -> Raggruppo per Generazione, per Stadio o nessuno?
            '            Dim colonna As String = ""
            '            output.aggiungiColonna(nome_colonna, GetType(Decimal), colonna, "0.00")
            '        Next
            '    Next

            '    Dim sb_debug As New Text.StringBuilder
            '    For i_t = 0 To nt - 1

            '        Dim mdo = mDatiMeteo(i_t)

            '        output.AddField(mdo.DataOra)

            '        sb_debug.Append(mdo.DataOra.ToString("dd/MM/yyyy : HH"))

            '        For i_gen = 0 To ngen - 1

            '            For Each oStadio In mStadi

            '                output.AddField(oStadio.gen(i_gen).zf(i_t))

            '                Dim val As Decimal = oStadio.gen(i_gen).zf(i_t)
            '                val = Math.Round(val * 100000D) / 100000D
            '                Dim str As String = "         " + String.Format(Globalization.CultureInfo.InvariantCulture, "{0:0.00000}", val)
            '                sb_debug.Append(Right(str, 15))

            '            Next
            '        Next

            '        output.Commit()

            '        sb_debug.Append(vbCrLf)
            '    Next

        End If

        Dim risultatoModello As New cRisultatoModello
        risultatoModello.Modello_Tabella1 = output.Output()

        Return risultatoModello
    End Function



    Public Class Indicatore
        Public Giorno As DateTime
        Public Class Stadio
            Public Nome As String
            Public Valore As Decimal
        End Class
        Public Stadi As List(Of Stadio)
        Public Sub New()
            Stadi = New List(Of Stadio)
        End Sub
    End Class

    Public Function ElaboraIndicatore() As Indicatore

        If mDatiGG Is Nothing OrElse mDatiGG.Count = 0 Then
            Return Nothing
        End If

        Dim idx_gg As Integer = mDatiGG.Count - 1
        Dim dgg As DatoGG = mDatiGG(idx_gg)

        Dim indic As New Indicatore With {
            .Giorno = dgg.Giorno
        }

        Dim ngen As Integer = NGenerazioni()
        Dim stadi = StrStadi()
        For Each s In stadi
            indic.Stadi.Add(New Indicatore.Stadio With {
                            .Nome = s,
                            .Valore = 0
                            })
        Next

        For i_g = 0 To ngen - 1

            For i_s = 0 To stadi.Length - 1

                If dgg.GenXStadi(i_g, i_s).HasValue Then

                    indic.Stadi(i_s).Valore += dgg.GenXStadi(i_g, i_s).Value
                End If
            Next
        Next

        Return indic
    End Function

End Class



Public Class Agronomica30_MRV_Eulia___OLD
    Inherits Agronomica30_MRV___OLD
    Protected Overrides Function ImpostaStadioSvernante(ByVal Stadi As List(Of Stadio)) As Stadio

        'sverna come pupa nws = 2
        Dim sv As Stadio = Stadi(2)

        'condizione iniziale distribuita sul primo quarto dell'età fisiologica per lo stadio svernante
        Dim nk As Integer = sv.nk / 4 - 1
        Dim val As Decimal = 100D / (0.25D * sv.deltx * sv.nk)
        For ik = 0 To nk
            sv.gen(0).zfi(ik) = val
        Next

        Return sv
    End Function
    Protected Overrides Function FV(ByVal iStadio As Integer) As FunSviluppo
        If iStadio = 0 Then
            Return New Briere_1(0.00010558, 7.4209, 34.9515)
        ElseIf iStadio = 1 Then
            Return New Briere_1(0.000057592, 9.4864, 31.8667)
        ElseIf iStadio = 2 Then
            Return New Briere_1(0.00010856, 7.7434, 33.7707)
        Else
            Return New Briere_1(0.00006, 7.6604, 35.0)
        End If
        Return Nothing
    End Function
End Class

Public Class Agronomica30_MRV_CydiaMolesta___OLD
    Inherits Agronomica30_MRV___OLD
    Protected Overrides Function ImpostaStadioSvernante(ByVal Stadi As List(Of Stadio)) As Stadio

        'stadio svernante: termine dello stadio larvale (winter stage: nws=1)
        Dim sv As Stadio = Stadi(1)

        ''condizione iniziale distribuita sulla prima metà dell'età fisiologica
        'Dim nk As Integer = sv.nk / 2 - 1
        'Dim val As Decimal = 100D / (sv.deltx * 0.5D * sv.nk)
        'For ik = 0 To nk
        '    sv.gen(0).zfi(ik) = val
        'Next

        'condizione iniziale distribuita sulla seconda metà dell'età fisiologica
        Dim ik0 As Integer = sv.nk / 2 + 1
        Dim ik1 As Integer = sv.nk - 1
        Dim val As Decimal = 100D / (sv.deltx * 0.5D * (sv.nk - 1))
        For ik = ik0 To ik1
            sv.gen(0).zfi(ik) = val
        Next

        Return sv
    End Function
    Protected Overrides Function FV(ByVal iStadio As Integer) As FunSviluppo
        If iStadio = 0 Then
            Return New Bieri_1(0.0125215958, 1.80300636, 32.644203, 4.33535249)
        ElseIf iStadio = 1 Then
            Return New Bieri_1(0.006204089255957, 1.61323956092378, 30.114552877400751, 9.1847652486544487)
        ElseIf iStadio = 2 Then
            'Return New Briere_1(0.00008316238252249, 11.51148033, 36.777898943)
            Return New Briere_1(0.00004530461, 3.2223305, 39.9388)
        Else
            'usiamo quello delle pupe perchè abbiamo solo due dati per gli adulti
            'Return New Briere_1(0.00008316238252249, 11.51148033, 36.777898943)
            Return New Briere_1(0.00004530461, 3.2223305, 39.9388)
        End If
        Return Nothing
    End Function
    Protected Overrides Function NGenerazioni() As Integer
        Return 6
    End Function
End Class

Public Class Agronomica30_MRV_Carpocapsa___OLD
    Inherits Agronomica30_MRV___OLD
    Protected Overrides Function ImpostaStadioSvernante(ByVal Stadi As List(Of Stadio)) As Stadio

        'sverna come larva (winter stage: nws=1)
        Dim sv As Stadio = Stadi(1)

        'condizione iniziale distribuita sulla seconda metà dell'età fisiologica
        Dim ik0 As Integer = sv.nk / 2 + 1
        Dim ik1 As Integer = sv.nk - 1
        Dim val As Decimal = 100D / (sv.deltx * 0.5D * (sv.nk - 1))
        For ik = ik0 To ik1
            sv.gen(0).zfi(ik) = val
        Next

        Return sv
    End Function
    Protected Overrides Function FV(ByVal iStadio As Integer) As FunSviluppo
        If iStadio = 0 Then
            Return New Lactin(0.173077918452635, 36.7585060697056, 5.7708754366849044)
        ElseIf iStadio = 1 Then
            Return New Lactin(0.15076632355712, 37.094239251128556, 6.6284462491589782)
        ElseIf iStadio = 2 Then
            Return New Lactin(0.1602228711576, 37.762802446158751, 6.2381095071158361)
        Else
            Return New Lactin(0.1406, 40.0, 7.1)
        End If
        Return Nothing
    End Function
End Class

Public Class Agronomica30_MRV_Helicoverpa___OLD
    Inherits Agronomica30_MRV___OLD
    Protected Overrides Function ImpostaStadioSvernante(ByVal Stadi As List(Of Stadio)) As Stadio

        'sverna come pupa (winter stage: nws=2)
        Dim sv As Stadio = Stadi(2)

        'condizione iniziale con tutti gli individui aventi età fisiologica zero
        sv.gen(0).zfi(0) = 100D / sv.deltx

        Return sv
    End Function
    Protected Overrides Function FV(ByVal iStadio As Integer) As FunSviluppo
        If iStadio = 0 Then
            Return New Briere_1(0.00026909, 11.4393, 39.9997)
        ElseIf iStadio = 1 Then
            Return New Briere_1(0.0000454668, 9.80214, 39.88807)
        ElseIf iStadio = 2 Then
            Return New Briere_1(0.0000614663, 10.6805, 39.9997)
        Else
            'Return New Briere_1(0.00049473, 11.71109, 36.44138)
            Return New Briere_1(0.00004, 13, 38)
        End If
        Return Nothing
    End Function
End Class

Public Class Agronomica30_MRV_Tignoletta___OLD
    Inherits Agronomica30_MRV___OLD
    Protected Overrides Function ImpostaStadioSvernante(ByVal Stadi As List(Of Stadio)) As Stadio

        'sverna come pupa
        Dim sv As Stadio = Stadi(2)

        'condizione iniziale distribuita sul primo quarto dell'età fisiologica
        Dim nk As Integer = sv.nk / 4 - 1
        Dim val As Decimal = 100D / (0.25D * sv.deltx * sv.nk)
        For ik = 0 To nk
            sv.gen(0).zfi(ik) = val
        Next

        Return sv
    End Function
    Protected Overrides Function FV(ByVal iStadio As Integer) As FunSviluppo
        If iStadio = 0 Then
            Return New Briere_1(0.0001824, 9.21, 36.66)
        ElseIf iStadio = 1 Then
            Return New Briere_1(0.00004444, 7.77, 38.76)
        ElseIf iStadio = 2 Then
            Return New Briere_1(0.00009641, 11.095, 34.71)
        Else
            Return New Briere_1(0.00009641, 11.095, 34.71)
        End If
        Return Nothing
    End Function
    Protected Overrides Function NSottostadi(ByVal i_s As Integer) As Integer
        Return 801
    End Function
End Class





'**************************************************************************************************
'**************************************************************************************************
'**************************************************************************************************





Public Class SolarCalculator

    '**********************************************************************************************
    'https://gml.noaa.gov/grad/solcalc/
    '**********************************************************************************************

    Private _latitude As Decimal
    Private _longitude As Decimal
    Private _zoneId As String
    Private _sunrise As Decimal
    Private _sunset As Decimal

    Public ReadOnly Property Sunrise As Decimal
        Get
            Return HourDec(_sunrise)
        End Get
    End Property

    Public ReadOnly Property Sunset As Decimal
        Get
            Return HourDec(_sunset)
        End Get
    End Property

    Public ReadOnly Property DayLength As Decimal
        Get
            Return HourDec(_sunset - _sunrise)
        End Get
    End Property

    Private Function Deg2Rad(deg As Decimal) As Decimal
        Return Math.PI * deg / 180.0
    End Function

    Private Function Rad2Deg(rad As Decimal) As Decimal
        Return 180.0 * rad / Math.PI
    End Function

    Private Function HourDec(minutes As Decimal) As Decimal

        Dim hours As Decimal = Math.Floor(minutes / 60.0)
        Dim seconds As Decimal = Math.Floor((minutes - Math.Floor(minutes)) * 60.0)
        minutes = Math.Floor(minutes - (hours * 60.0))

        Return hours + (minutes / 60.0) + (seconds / 3600.0)
    End Function

    Public Sub New(lat As Decimal, lng As Decimal, zoneId As String)
        _latitude = lat
        _longitude = lng
        _zoneId = zoneId
    End Sub

    Public Sub Calc(dt As Date)

        Dim year As Integer = dt.Year
        Dim month As Integer = dt.Month
        Dim day As Integer = dt.Day

        If month <= 2 Then

            year -= 1
            month += 12
        End If

        Dim A As Decimal = Math.Floor(year / 100.0)
        Dim B As Decimal = 2 - A + Math.Floor(A / 4)

        'Number is returned for start of day. Fractional days should be added later. 
        Dim julian As Decimal = Math.Floor(365.25 * (year + 4716)) + Math.Floor(30.6001 * (month + 1)) + day + B - 1524.5

        'Julian Day to centuries since J2000.0
        Dim t As Decimal = (julian - 2451545.0) / 36525.0

        Dim seconds As Decimal = 21.448 - t * (46.815 + t * (0.00059 - t * (0.001813)))

        'calculate the mean obliquity of the ecliptic (degrees)   
        Dim e0 As Decimal = 23.0 + (26.0 + (seconds / 60.0)) / 60.0

        'corrected obliquity of the ecliptic (degrees)
        Dim epsilon As Decimal = e0 + 0.00256 * Math.Cos(Deg2Rad(125.04 - 1934.136 * t))

        'Geometric Mean Longitude of the Sun (degrees)
        Dim l0 As Decimal = 280.46646 + t * (36000.76983 + 0.0003032 * t)
        While l0 > 360.0
            l0 -= 360.0
        End While
        While l0 < 0.0
            l0 += 360.0
        End While

        Dim l0rad As Decimal = Deg2Rad(l0)

        'calculate the eccentricity of earth's orbit (unitless)
        Dim e As Decimal = 0.016708634 - t * (0.000042037 + 0.0000001267 * t)

        'Geometric Mean Anomaly of the Sun in radians
        Dim mrad As Double = Deg2Rad(357.52911 + t * (35999.05029 - 0.0001537 * t))

        Dim y As Decimal = Math.Tan(Deg2Rad(epsilon) / 2.0)
        y *= y
        Dim sin2l0 As Decimal = Math.Sin(2.0 * l0rad)
        Dim sinm As Decimal = Math.Sin(mrad)
        Dim cos2l0 As Decimal = Math.Cos(2.0 * l0rad)
        Dim sin4l0 As Decimal = Math.Sin(4.0 * l0rad)
        Dim sin2m As Decimal = Math.Sin(2.0 * mrad)
        Dim sin3m As Decimal = Math.Sin(3.0 * mrad)

        Dim Etime As Decimal = y * sin2l0 - 2.0 * e * sinm + 4.0 * e * y * sinm * cos2l0 - 0.5 * y * y * sin4l0 - 1.25 * e * e * sin2m
        'difference between true solar time and mean solar time (equation of time in minutes of time)
        Dim eqTime As Decimal = Rad2Deg(Etime) * 4.0

        'equation of center for the sun (degrees)
        Dim c As Decimal = sinm * (1.914602 - t * (0.004817 + 0.000014 * t)) + sin2m * (0.019993 - 0.000101 * t) + sin3m * 0.000289

        'true longitude of the sun (degrees)
        Dim o As Decimal = l0 + c

        'sun's apparent longitude (degrees)
        Dim lambda As Decimal = o - 0.00569 - 0.00478 * Math.Sin(Deg2Rad(125.04 - 1934.136 * t))

        Dim sint As Double = Math.Sin(Deg2Rad(epsilon)) * Math.Sin(Deg2Rad(lambda))
        'sun's declination in degrees 
        Dim solarDec As Decimal = Rad2Deg(Math.Asin(sint))

        ' calculate the hour angle of the sun at sunrise for the latitude   
        Dim latRad As Decimal = Deg2Rad(_latitude)
        Dim sdRad As Decimal = Deg2Rad(solarDec)
        Dim HAarg As Decimal = (Math.Cos(Deg2Rad(90.833)) / (Math.Cos(latRad) * Math.Cos(sdRad)) - Math.Tan(latRad) * Math.Tan(sdRad))
        'hour angle of sunrise (radians)
        Dim hourAngle As Decimal = (Math.Acos(Math.Cos(Deg2Rad(90.833)) / (Math.Cos(latRad) * Math.Cos(sdRad)) - Math.Tan(latRad) * Math.Tan(sdRad)))

        ' calculate the Universal Coordinated Time (UTC) of sunrise for the given day at the given location on earth   
        Dim delta As Decimal = _longitude + Rad2Deg(hourAngle)
        Dim sunriseUTC As Decimal = 720 - (4.0 * delta) - eqTime 'time in minutes from zero Z  

        ' calculate the Universal Coordinated Time (UTC) of sunset for the given day at the given location on earth   
        hourAngle = -hourAngle
        delta = _longitude + Rad2Deg(hourAngle)
        Dim sunsetUTC As Decimal = 720 - (4.0 * delta) - eqTime ' time in minutes from zero Z  

        Dim zone As TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(_zoneId)
        Dim tspan As TimeSpan = zone.GetUtcOffset(dt)

        _sunrise = sunriseUTC + tspan.TotalMinutes
        _sunset = sunsetUTC + tspan.TotalMinutes

    End Sub
End Class
