
Imports AgronicaCoreDataProvider.My.Resources



Public Class Agronomica30_Peronospora
    Inherits AbstractModello

    Private Class TabellaModello

        Public Class DatoOrario
            Public ReadOnly Meteo As MeteoDSSItem
            Public ReadOnly BagnaturaEffettiva As Boolean
            Public ReadOnly Hth As Decimal
            Public ReadOnly Ht As Decimal
            Public ReadOnly DOR As Decimal
            Public ReadOnly BBCH As Decimal

            Sub New(_meteo As MeteoDSSItem, _bbch As Decimal, prev1 As DatoOrario, prev2 As DatoOrario)

                Meteo = _meteo
                BagnaturaEffettiva = False
                Hth = 0
                Ht = 0
                DOR = 0
                BBCH = _bbch

                Dim VPD As Decimal = Meteo.VPD()

                Dim sogliaUR As Decimal = 90

                If prev2 IsNot Nothing AndAlso (prev2.Meteo.Prec > 0 OrElse prev1.Meteo.Prec > 0) Then

                    sogliaUR = 85
                End If

                If VPD < 1 OrElse Meteo.BagnEffettiva(sogliaUR) > 0 Then

                    BagnaturaEffettiva = True
                End If

                If Meteo.Temp >= 0 Then

                    If Meteo.Prec > 0 OrElse Math.Round(VPD, 1) <= 4.5 Then

                        Hth = 1D / (1330.1D - Meteo.Temp * (116.19D - 2.6256D * Meteo.Temp))
                    End If
                End If

                If prev1 IsNot Nothing Then

                    Ht = prev1.Ht + Hth
                End If

                If Ht > 1.55 Then

                    DOR = Math.Exp(-15.891D * Math.Exp(-0.653D * (Ht + 1D))) - 0.0489D '0.0489 per partire con 0
                End If
            End Sub
        End Class

        Private ReadOnly _tabella As List(Of DatoOrario)

        Sub New()
            _tabella = New List(Of DatoOrario)
        End Sub

        Public Sub Add(d As DatoOrario)
            _tabella.Add(d)
        End Sub

        Default Public ReadOnly Property Row(idx As Integer) As DatoOrario
            Get
                Return _tabella(idx)
            End Get
        End Property

        Public ReadOnly Property Count As Integer
            Get
                Return _tabella.Count
            End Get
        End Property

        Public Function GetEnumerator() As IEnumerator(Of DatoOrario)
            Return _tabella.GetEnumerator()
        End Function

        Public Function FindFirstInSequence(_start As Integer, _stop As Integer, match As Predicate(Of DatoOrario)) As Integer

            Dim resIdx As Integer = -1
            Dim _step As Integer = 1

            If _start > _stop Then

                If _start >= _tabella.Count Then

                    Return resIdx
                End If

                _stop = Math.Max(_stop, 0)

                _step = -1
            Else

                _stop = Math.Min(_stop, _tabella.Count - 1)
                _start = Math.Min(Math.Max(_start, 0), _tabella.Count - 1)
            End If

            For idx = _start To _stop Step _step

                If match(_tabella(idx)) Then

                    resIdx = idx
                    Exit For
                End If
            Next

            Return resIdx
        End Function

        Public Class IndexItemPair
            Public ReadOnly Index As Integer
            Public ReadOnly Item As DatoOrario
            Public Sub New(idx As Integer, itm As DatoOrario)
                Index = idx
                Item = itm
            End Sub
        End Class

        Public Iterator Function Sequence(_start As Integer, _stop As Integer) As IEnumerable(Of IndexItemPair)

            Dim _step As Integer = 1

            If _start > _stop Then

                If _start >= _tabella.Count Then

                    Return
                End If

                _step = -1
            End If

            For num = _start To _stop Step _step

                Yield New IndexItemPair(num, _tabella(num))
            Next
        End Function

        Public Iterator Function SequenceWhile(_start As Integer, _stop As Integer, whileCondition As Predicate(Of DatoOrario)) As IEnumerable(Of DatoOrario)

            Dim _step As Integer = 1

            If _start > _stop Then
                _step = -1
            End If

            For num = _start To _stop Step _step

                Dim elem = _tabella(num)

                If whileCondition(elem) Then

                    Yield elem
                Else

                    Exit For
                End If
            Next
        End Function

    End Class

    Private Class PercOrDateTime

        Private Enum ProgressStatus
            isNull = -1
            isPerc = 0
            isFull = 1
        End Enum

        Private _perc As Decimal
        Private _dt As DateTime
        Private _status As ProgressStatus

        Public Sub New()
            _perc = 0
            _dt = Date.MinValue
            _status = ProgressStatus.isNull
        End Sub

        Public WriteOnly Property Perc As Decimal
            Set(value As Decimal)
                _status = ProgressStatus.isPerc
                _perc = value
            End Set
        End Property

        Public WriteOnly Property DateTime As DateTime
            Set(value As DateTime)
                _status = ProgressStatus.isFull
                _dt = value
            End Set
        End Property

        Public Function AsString(dt_format As String) As String
            If _status = ProgressStatus.isFull Then
                Return _dt.ToString(dt_format)
            End If
            If _status = ProgressStatus.isPerc Then
                Return _perc.ToString("0.0%")
            End If
            Return ""
        End Function

        Public Function HasDateTime(ByRef dt As DateTime) As Boolean
            dt = _dt
            Return _status = ProgressStatus.IsFull
        End Function
    End Class

    Private Class Coorte

        Public Enum EnumStato
            no_rilascio = 0
            morte_sporangio = 1
            morte_zoospore = 2
            no_infezione = 3
            possibile_infezione = 4
            infezione = 5
        End Enum

        Public ReadOnly data_pioggia_partenza_GER0 As DateTime
        Public ReadOnly PMO As Decimal
        Public ReadOnly GER1_mature As PercOrDateTime
        Public data_rilascio_zoospore As DateTime
        Public data_pioggia_dispersione_zoospore As DateTime
        Public ReadOnly incub1 As PercOrDateTime
        Public ReadOnly incub2 As PercOrDateTime
        Public stato As EnumStato
        Public ReadOnly progGER As List(Of Decimal)

        Sub New(ger0 As DateTime, _pmo As Decimal)

            data_pioggia_partenza_GER0 = ger0
            PMO = _pmo
            GER1_mature = New PercOrDateTime
            data_rilascio_zoospore = Date.MinValue
            data_pioggia_dispersione_zoospore = Date.MinValue
            incub1 = New PercOrDateTime
            incub2 = New PercOrDateTime
            stato = EnumStato.no_rilascio
            progGER = New List(Of Decimal)

        End Sub

        Public Function DestinoAsString() As String

            Dim destino As String = ""

            Select Case stato

                Case EnumStato.morte_sporangio
                    destino = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_Peronospora_morteSporangi

                Case EnumStato.morte_zoospore
                    destino = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_Peronospora_morteZoospore

                Case EnumStato.no_infezione
                    destino = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_Peronospora_noInfezione

                Case EnumStato.possibile_infezione
                    destino = "Possibile infezione in corso"

                Case EnumStato.infezione
                    destino = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_Peronospora_infezioneVediIncubazione

            End Select

            Return destino
        End Function
    End Class

    Private Class Peronospora

        Public Enum FaseInfezione
            Oospore_non_mature = -1         'Si considera progressHt (Ht < 1.55)
            Oospore_mature = 0              'Si considera progressHt al 100% (Ht >= 1.55 ma senza evento pioggia)
            Oospore_attive = 1              'Fase infettiva, generazione coorti
            Oospore_esaurite = 2            'Oospore esaurite (somma_coorti > 0.95)
            Coltura_non_suscettibile = 3    'Coltura non più suscettibile (BBCH > 91)
        End Enum

        Public fase As FaseInfezione
        Public progressHt As Decimal
        Public dataInizioCiclo As Date
        Public dataFineCiclo As Date
    End Class

    Private ReadOnly _tabella As TabellaModello
    Private ReadOnly _collezione As List(Of Coorte)


    Public Sub New(ByVal datiMeteo As MeteoReadOnlyList)
        MyBase.New(datiMeteo)

        _tabella = New TabellaModello
        _collezione = New List(Of Coorte)
    End Sub


    Protected Overrides Function _elaboraModello() As cRisultatoModello

        Dim pero = CalcolaPeronospora()

        If pero Is Nothing Then

            Return Nothing
        End If

        If pero.fase = Peronospora.FaseInfezione.Oospore_non_mature OrElse pero.fase = Peronospora.FaseInfezione.Oospore_mature Then

            _errore = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.NonRaggiuntaLaFaseFenologicaNecessariaAlloSviluppoDellaAvversita

            Return Nothing
        End If

        Dim msg As String = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_Peronospora_inizioCicloMalattiaOosporePronteDal_ & pero.dataInizioCiclo.ToShortDateString()

        Select Case pero.fase

            Case Peronospora.FaseInfezione.Oospore_esaurite
                'msg &= " - " & My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_Peronospora_fineCicloMalattiaOosporeEsauriteAl_
                msg &= " - Fine ciclo malattia, oospore esaurite"
                If pero.dataFineCiclo > Date.MinValue Then

                    msg &= " al " & pero.dataFineCiclo.ToShortDateString()
                End If

            Case Peronospora.FaseInfezione.Coltura_non_suscettibile
                msg &= " - Coltura non più suscettibile dal " & pero.dataFineCiclo.ToShortDateString()

        End Select

        Dim risModello As New cRisultatoModello With {
            .Modello_WarningMsg = "<div style='text-align: center; font-size: larger;'>" & msg & "</div>",
            .Modello_Tabella1 = OUT_coorti_time(),
            .Modello_Tabella2 = OUT_coorti(),
            .Modello_TabellaMeteo = OUT_meteo()
        }

        Return risModello
    End Function


    Protected Overrides Function _elaboraIndicatore_vecchio(dataInizio As Date, dataFine As Date) As cRisultatoModelloIndicatori.Indicatore

        Dim risIndic As New cRisultatoModelloIndicatori.Indicatore("Peronospora", dataInizio, dataFine)

        Dim pero = CalcolaPeronospora()

        If pero IsNot Nothing Then

            If pero.fase = Peronospora.FaseInfezione.Oospore_attive Then

                Dim doy As Integer = dataFine.DayOfYear
                Dim doyGER As Integer
                Dim indicatore As Decimal = -1
                Dim auxmsg As String = ""
                For Each _coorte In _collezione

                    doyGER = _coorte.data_pioggia_partenza_GER0.DayOfYear + _coorte.progGER.Count - 1

                    If doy = doyGER Then

                        Dim germinazione_oospore = _coorte.progGER.Last()
                        If germinazione_oospore >= 0.85 Then
                            Dim pmo = _coorte.PMO
                            If pmo >= 0.05 Then
                                'Attenzione: Rischio di infezione! Potenziale infettivo grave
                                auxmsg = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_Peronospora_potenzialeInfettivoGrave
                            ElseIf pmo >= 0.02 Then
                                'Attenzione: Rischio di infezione! Potenziale infettivo medio
                                auxmsg = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_Peronospora_potenzialeInfettivoMedio
                            Else
                                'Attenzione: Rischio di infezione! Potenziale infettivo leggero
                                auxmsg = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_Peronospora_potenzialeInfettivoLeggero
                            End If
                        ElseIf germinazione_oospore >= 0.71 Then
                            'Attenzione: seguire l’evoluzione del modello!
                        Else
                            'Basso rischio epidemico
                        End If

                        If indicatore < germinazione_oospore Then
                            indicatore = germinazione_oospore
                        End If
                    End If
                Next

                If indicatore < 0 Then
                    'non ci sono famiglie attive...
                    indicatore = 0
                    'Assenza di rischio epidemico
                End If

                risIndic.Fill(indicatore, 1, {0.71, 0.85}, dataFine, dataFine)
                risIndic.AuxMsg = auxmsg

            Else

                risIndic.Status = cRisultatoModelloIndicatori.Indicatore.enum_Status.Status_Error
                risIndic.StatusMsg = _errore
            End If
        Else

            risIndic.Status = cRisultatoModelloIndicatori.Indicatore.enum_Status.Status_Error
            risIndic.StatusMsg = _errore
        End If

        Return risIndic
    End Function


    Protected Overrides Function _elaboraIndicatore(ByVal risElab As RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione) As RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione

        Dim pero = CalcolaPeronospora()

        If pero Is Nothing Then

            risElab.Errore(_errore)

            Return risElab
        End If

        If pero.fase = Peronospora.FaseInfezione.Oospore_non_mature OrElse pero.fase = Peronospora.FaseInfezione.Oospore_mature Then

            risElab.InProgress(pero.progressHt, My.Resources.AgronicaCoreModelliPrevisionaliBIZ.NonRaggiuntaLaFaseFenologicaNecessariaAlloSviluppoDellaAvversita)

            Return risElab
        End If

        If pero.fase = Peronospora.FaseInfezione.Coltura_non_suscettibile Then

            _errore = "Coltura non più suscettibile dal " & pero.dataFineCiclo.ToShortDateString()
            '_errore = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_Peronospora_fineCicloMalattiaOosporeEsauriteAl_ & pero.dataFineCiclo.ToShortDateString()

            risElab.Errore(_errore)

            Return risElab
        End If

        'pero.fase = Peronospora.FaseInfezione.Oospore_esaurite ??? Devo comunque monitorare l'ultima eventuale coorte...

        Dim doy As Integer = _datiMeteo.Last().DataOra.DayOfYear 'dataFine.DayOfYear
        Dim indicatore As Decimal = -1
        Dim livPotenzialeInfettivo As Integer = -1

        For Each _coorte In _collezione

            Dim doyGER As Integer = _coorte.data_pioggia_partenza_GER0.DayOfYear + _coorte.progGER.Count - 1

            If doy = doyGER Then

                Dim germinazione_oospore = _coorte.progGER.Last()

                indicatore = Math.Max(indicatore, germinazione_oospore)

                If germinazione_oospore >= 0.85 Then

                    Dim pmo = _coorte.PMO

                    If pmo >= 0.05 Then

                        livPotenzialeInfettivo = Math.Max(livPotenzialeInfettivo, 2)

                    ElseIf pmo >= 0.02 Then

                        livPotenzialeInfettivo = Math.Max(livPotenzialeInfettivo, 1)

                    Else

                        livPotenzialeInfettivo = Math.Max(livPotenzialeInfettivo, 0)
                    End If

                ElseIf germinazione_oospore >= 0.71 Then

                    'Attenzione: seguire l’evoluzione del modello!

                Else

                    'Basso rischio epidemico
                End If

            End If
        Next

        If indicatore < 0 Then

            'non ci sono famiglie attive...

            indicatore = 0 'Assenza di rischio epidemico
        End If

        Dim auxmsg As String = ""

        Select Case livPotenzialeInfettivo

            Case 0
                'Attenzione: Rischio di infezione! Potenziale infettivo leggero
                auxmsg = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_Peronospora_potenzialeInfettivoLeggero

            Case 1
                'Attenzione: Rischio di infezione! Potenziale infettivo medio
                auxmsg = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_Peronospora_potenzialeInfettivoMedio

            Case 2
                'Attenzione: Rischio di infezione! Potenziale infettivo grave
                auxmsg = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_Peronospora_potenzialeInfettivoGrave

        End Select

        risElab.Fill(indicatore, 1, {0.71, 0.85})
        risElab.AuxMsg = auxmsg

        Return risElab
    End Function


    Private Function CalcolaPeronospora() As Peronospora

        Dim pero As New Peronospora With {
            .fase = Peronospora.FaseInfezione.Oospore_non_mature,
            .progressHt = 0,
            .dataInizioCiclo = Date.MinValue,
            .dataFineCiclo = Date.MinValue
        }

        Try

            'creo il modello e contemporaneamente cerco indice data pioggia dopo Ht >= 1.55

            Dim BBCH As New Agronomica30_BBCH(12, 33, 26, Agronomica30_BBCH.VarietaVite.CabernetSauvignon)

            Dim idxPioggiaDopoHt155 As Integer = -1

            Dim prev1 As TabellaModello.DatoOrario = Nothing
            Dim prev2 As TabellaModello.DatoOrario = Nothing

            For Each meteo In _datiMeteo

                BBCH.Calc(meteo.Temp)

                Dim elem As New TabellaModello.DatoOrario(meteo, BBCH.BBCH_rip, prev1, prev2)

                prev2 = prev1
                prev1 = elem

                _tabella.Add(elem)

                If pero.fase = Peronospora.FaseInfezione.Oospore_non_mature Then

                    Dim ht As Decimal = Math.Round(elem.Ht, 3)

                    If ht >= 1.55 Then

                        'da qui inizia la stagione pongo DOR=0 (DOR(Ht)-0.047 per intervallo PIS
                        'ora da qui cerco la pioggia che dà inizio alla prima coorte di oospore 

                        pero.fase = Peronospora.FaseInfezione.Oospore_mature
                        pero.progressHt = 100
                        pero.dataInizioCiclo = meteo.DataOra
                    Else

                        pero.progressHt = Math.Min(1.0, ht / 1.55) * 100D
                    End If
                End If

                If pero.fase = Peronospora.FaseInfezione.Oospore_mature AndAlso elem.Meteo.Prec > 0 Then

                    pero.fase = Peronospora.FaseInfezione.Oospore_attive

                    idxPioggiaDopoHt155 = _tabella.Count - 1
                End If

            Next



            If pero.fase = Peronospora.FaseInfezione.Oospore_attive Then

                'idxPioggiaDopoHt155 >= 0

                'dalla prima pioggia creo l'oggetto coorte
                'ogni pioggia origina una coorte    

                GeneraCoorti(idxPioggiaDopoHt155, pero)
            End If

        Catch ex As Exception

            pero = Nothing

            'uso questa funzione per ottenere il Messaggio..:
            _errore = Gias.ErroreDuranteOperazione_ & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return pero
    End Function


    Private Sub GeneraCoorti(idxPioggiaDopoHt155 As Integer, pero As Peronospora)

        Dim idx As Integer = idxPioggiaDopoHt155
        Dim precedentePIS As Decimal = 0
        Dim somma_coorti As Decimal = 0

        Dim elem As TabellaModello.DatoOrario = _tabella(idx)

        While elem IsNot Nothing AndAlso pero.fase = Peronospora.FaseInfezione.Oospore_attive

            If elem.BBCH > 91 Then

                pero.fase = Peronospora.FaseInfezione.Coltura_non_suscettibile
                pero.dataFineCiclo = elem.Meteo.DataOra

                'Torno indietro per impostare la datFineCiclo alla data in cui ho il primo BBCH > 91
                For Each d In _tabella.SequenceWhile(idx - 1, 0, Function(x) x.BBCH > 91)

                    pero.dataFineCiclo = d.Meteo.DataOra
                Next

            Else

                'assegno all'oggetto il range partente dalla pioggia
                'calcolo oospopre fisiologicamente mature della corteJ che parte con la pioggiaJ (PMO)
                'il PMO è l'integrale del DOR che a sua volta è funzione di Ht
                'quando Ht=1.55 Ger= 0.05 per sicurezza tolgo 0.47
                'cort.densita_coorte_GER0 = cort.densita_coorte_GER0 - 0.047 perPIS 95%

                Dim pCoorte As New Coorte(elem.Meteo.DataOra, elem.DOR - precedentePIS)

                _collezione.Add(pCoorte)

                precedentePIS = elem.DOR

                Dim idx_GER1_mature As Integer

                If EmissioneSporangio(idx, pCoorte, idx_GER1_mature) Then

                    'calcola evoluzione coorte dopo maturazione sporangi ed emissione sporangi GER1

                    Dim flag_avvenuto_rilascio As Integer
                    Dim idx_rilascio As Integer

                    If RilascioZoospore(idx_GER1_mature, pCoorte, idx_rilascio, flag_avvenuto_rilascio) Then

                        Dim idx_pioggia As Integer

                        If DispersioneZoospore(idx_rilascio, flag_avvenuto_rilascio, pCoorte, idx_pioggia) Then

                            Dim idx_infez As Integer

                            If Infezione(idx_pioggia, pCoorte, idx_infez) Then

                                IncubazioneSintomi(idx_infez, pCoorte)
                            End If
                        End If
                    End If
                End If

                somma_coorti += pCoorte.PMO

                If somma_coorti > 0.95 Then

                    pero.fase = Peronospora.FaseInfezione.Oospore_esaurite

                    'La data fine ciclo è la GER1_mature, ma solo se si è arrivati al 100%
                    Dim ger1 As DateTime
                    If pCoorte.GER1_mature.HasDateTime(ger1) Then

                        pero.dataFineCiclo = ger1
                    End If

                Else

                    elem = Nothing

                    'Prossima pioggia dopo bagnatura

                    'Cerco la fine del periodo bagnato
                    idx = _tabella.FindFirstInSequence(idx, _tabella.Count - 1, Function(x) Not x.BagnaturaEffettiva)

                    If idx > 0 Then

                        'Cerco prossima pioggia da fine bagnatura
                        idx = _tabella.FindFirstInSequence(idx, _tabella.Count - 1, Function(x) x.Meteo.Prec > 0)

                        If idx > 0 Then

                            elem = _tabella(idx)
                        End If
                    End If
                End If
            End If
        End While
    End Sub


    Private Function EmissioneSporangio(idx As Integer, pCoorte As Coorte, ByRef idx_GER1_mature As Integer) As Boolean
        'vedo se arrivo a GER1
        '(calcolo matrice con valori giornalieri di GER 0 1 per grafico coorte
        'dal set di dati calcolo dati giornalieri della corte faccio calcoli giornalieri confrontando la data
        'quando progressGER=1 ho dati gg su matrice)

        idx_GER1_mature = -1

        Dim progressGER As Decimal = 0
        Dim doy As Integer = -1

        For Each iip In _tabella.Sequence(idx, _tabella.Count - 1)

            progressGER += iip.Item.Hth

            Dim curr_doy = iip.Item.Meteo.DataOra.DayOfYear

            If curr_doy <> doy Then

                doy = curr_doy
                pCoorte.progGER.Add(0)
            End If

            If progressGER < 1 Then

                pCoorte.progGER(pCoorte.progGER.Count - 1) = progressGER
                pCoorte.GER1_mature.Perc = progressGER

            Else
                'emissione sporangi (progressGER >= 1)

                pCoorte.progGER(pCoorte.progGER.Count - 1) = 1
                pCoorte.GER1_mature.DateTime = iip.Item.Meteo.DataOra

                idx_GER1_mature = iip.Index

                Exit For
            End If
        Next

        Return idx_GER1_mature >= 0
    End Function


    Private Function RilascioZoospore(idx_GER1_mature As Integer, pCoorte As Coorte, ByRef idx_rilascio As Integer, ByRef flag_avvenuto_rilascio As Integer) As Boolean

        'Durata max degli sporangi vivi
        'Calcolo SUS incrementale fino a quando arriva a 1 (morte)

        Dim idx_morte_sporangi As Integer = _tabella.Count - 1
        Dim SUS As Decimal = 0

        For Each iip In _tabella.Sequence(idx_GER1_mature, _tabella.Count - 1)

            Dim VP As Decimal = iip.Item.Meteo.Temp * (1D - iip.Item.Meteo.UmRel / 100D)

            SUS += 1D / ((((0.01D * VP) - 0.47D) * VP + 5.67D) * 24D)

            If SUS >= 1 Then
                'Morte sporangi

                idx_morte_sporangi = iip.Index

                Exit For
            End If
        Next

        'cerco prima riga bagnatura nell'intervallo da ger1 a idx_norte_sporangi
        Dim idx_prima_bagn As Integer = _tabella.FindFirstInSequence(idx_GER1_mature + 1, idx_morte_sporangi, Function(x) x.BagnaturaEffettiva)

        'caso in cui non ci sia bagnatura fogliare per tutta la durata
        'se prima bagnatura si verifica nella riga GER=1 allora porre riga_prima_bagn=riga_prima_bagn+1

        idx_rilascio = 0
        flag_avvenuto_rilascio = 0

        Dim exitSub As Boolean = False

        While Not exitSub AndAlso idx_prima_bagn <= idx_morte_sporangi

            If idx_prima_bagn < 0 Then

                If idx_morte_sporangi < _tabella.Count - 1 Then

                    pCoorte.stato = Coorte.EnumStato.morte_sporangio
                End If

                exitSub = True
            Else

                'La lettiera è bagnata calcolo TWh (media temperatura ore bagnate)

                Dim ore_successive_bagnate As Integer = 0
                Dim ore_necessarie_rilascio_zoospore As Integer = 0
                Dim avvenutoRilascio As Boolean = False
                Dim sumT As Decimal = 0

                For Each d In _tabella.SequenceWhile(idx_prima_bagn, idx_morte_sporangi, Function(x) x.BagnaturaEffettiva)

                    ore_successive_bagnate += 1

                    If Not avvenutoRilascio Then

                        sumT += Math.Max(d.Meteo.Temp, 1)

                        ore_necessarie_rilascio_zoospore = Math.Round(Math.Exp(-1.022D + (19.634D / (sumT / ore_successive_bagnate))) + 0.5, 0)

                        avvenutoRilascio = ore_successive_bagnate >= ore_necessarie_rilascio_zoospore
                    End If
                Next

                'se le ore_in_succ_bagnate > ore_necess_rilascio_zoospore avviene rilascio
                flag_avvenuto_rilascio = ore_successive_bagnate - ore_necessarie_rilascio_zoospore

                If flag_avvenuto_rilascio < 0 Then
                    'cerco possibile intervallo bagnato in intervallo sopravvivenza sporangi

                    If idx_prima_bagn >= idx_morte_sporangi AndAlso idx_morte_sporangi < _tabella.Count Then

                        pCoorte.stato = Coorte.EnumStato.morte_sporangio
                        exitSub = True

                    Else

                        idx_prima_bagn += 1

                        If idx_prima_bagn <= idx_morte_sporangi Then

                            idx_prima_bagn = _tabella.FindFirstInSequence(idx_prima_bagn, idx_morte_sporangi, Function(x) x.BagnaturaEffettiva)

                        End If
                    End If
                Else

                    idx_rilascio = idx_prima_bagn + ore_necessarie_rilascio_zoospore

                    If idx_rilascio < _tabella.Count Then

                            pCoorte.data_rilascio_zoospore = _tabella(idx_rilascio).Meteo.DataOra

                        Else

                            pCoorte.data_rilascio_zoospore = _tabella(_tabella.Count - 1).Meteo.DataOra.AddHours(1)
                        End If

                        Exit While
                    End If
                End If
        End While

        If exitSub OrElse idx_rilascio = 0 OrElse idx_rilascio >= _tabella.Count Then

            Return False
        End If

        Return True
    End Function


    Private Function DispersioneZoospore(idx_rilascio As Integer, flag_avvenuto_rilascio As Integer, pCoorte As Coorte, ByRef idx_pioggia As Integer) As Boolean

        'ci sono ore bagnatura dopo il tempo di rilascio

        Dim fine_bagn_intervallo = _tabella.FindFirstInSequence(idx_rilascio, _tabella.Count - 1, Function(x) Not x.BagnaturaEffettiva)

        If fine_bagn_intervallo < 0 Then

            fine_bagn_intervallo = _tabella.Count - 1
        End If

        'da questa riga c'e rilascio vedo se piove nell intervallo per disperdere sulle foglie le zoospore

        idx_pioggia = _tabella.FindFirstInSequence(idx_rilascio, fine_bagn_intervallo, Function(x) x.Meteo.Prec > 0)

        If idx_pioggia < 0 Then

            'zoospore RELEASE SENZA PIOGGIA - morti
            If flag_avvenuto_rilascio >= 0 AndAlso fine_bagn_intervallo <= _tabella.Count Then

                pCoorte.stato = Coorte.EnumStato.morte_zoospore
            End If

            Return False
        End If

        If flag_avvenuto_rilascio < 0 Then

            Return False
        End If

        'ok pioggia dopo rilascio

        pCoorte.data_pioggia_dispersione_zoospore = _tabella(idx_pioggia).Meteo.DataOra

        Return True
    End Function


    Private Function Infezione(idx_pioggia As Integer, pCoorte As Coorte, ByRef idx_infez As Integer) As Boolean
        'L'infezione avviene se la sommatoria termica delle ore bagnate successive alla diffusione delle zoospore
        'risulta >= 55 (era 60) -> Correzione come da mail di Serena del 01/10/2020

        idx_infez = 0

        pCoorte.stato = Coorte.EnumStato.possibile_infezione

        Dim sumT As Decimal = 0

        For Each iip In _tabella.Sequence(idx_pioggia, _tabella.Count - 1)

            If Not iip.Item.BagnaturaEffettiva Then

                pCoorte.stato = Coorte.EnumStato.no_infezione

                Exit For
            End If

            sumT += iip.Item.Meteo.Temp

            If sumT >= 55 Then

                pCoorte.stato = Coorte.EnumStato.infezione

                idx_infez = iip.Index

                Exit For
            End If
        Next

        Return idx_infez > 0
    End Function


    Private Sub IncubazioneSintomi(idx_infez As Integer, pCoorte As Coorte)
        ' Purpose   = 1 / ((45.1 - 3.45 * temp + 0.07 * temp^2) * 24) UR alta
        '           = 1 / ((59.9 - 4.55 * temp + 0.095 * temp ^2) * 24) UR bassa
        'Le due colonne calcolano il periodo di incubazione.
        'La comparsa dei sintomi stimata dal modello è compresa tra il momento in cui le due sommatorie raggiungono il valore 1
        'calcolo incubazione INC in ore progressive dall'infezione
        'inc 1 e inc 2 rappresentano l'intervallo di confidenza entro cui si renderanno visibili i sintomi sulle foglie

        Dim incub_basso As Decimal = 0
        Dim incub_alto As Decimal = 0
        Dim limite_basso As Boolean = False
        Dim limite_alto As Boolean = False

        For Each iip In _tabella.Sequence(idx_infez + 1, _tabella.Count - 1)

            Dim t As Decimal = iip.Item.Meteo.Temp

            If Not limite_basso Then

                incub_basso += 1D / ((59.9D - 4.55D * t + 0.095D * Math.Pow(t, 2)) * 24D)

                If Math.Round(incub_basso, 2) >= 1 Then

                    limite_basso = True
                    pCoorte.incub1.DateTime = iip.Item.Meteo.DataOra

                Else

                    pCoorte.incub1.Perc = incub_basso
                End If
            End If

            If Not limite_alto Then

                incub_alto += 1D / ((45.1D - 3.45D * t + 0.07D * Math.Pow(t, 2)) * 24D)

                If Math.Round(incub_alto, 2) >= 1 Then

                    limite_alto = True
                    pCoorte.incub2.DateTime = iip.Item.Meteo.DataOra

                Else

                    pCoorte.incub2.Perc = incub_alto
                End If
            End If

            If limite_basso AndAlso limite_alto Then

                Exit For
            End If
        Next
    End Sub


    Private Function OUT_meteo()

        Dim output As New OutputModello
        'i18n
        output.aggiungiColonna("DataOra", GetType(DateTime), Gias.DataOra, "dd/MM/yyyy HH")
        output.aggiungiColonna("Temp", GetType(Decimal), "Temperatura", "0.00")
        output.aggiungiColonna("UR", GetType(Decimal), "Umidità relativa", "0.00")
        output.aggiungiColonna("Pioggia", GetType(Decimal), Gias.Pioggia, "0.00")
        output.aggiungiColonna("Bagn", GetType(Integer), "Bagnatura", "0")
        output.aggiungiColonna("BagnEff", GetType(Integer), "Bagnatura Effettiva", "0")

        For Each d_o In _tabella

            output.AddField(d_o.Meteo.DataOra)
            output.AddField(d_o.Meteo.Temp)
            output.AddField(d_o.Meteo.UmRel)
            output.AddField(d_o.Meteo.Prec)
            output.AddField(d_o.Meteo.Bagn)
            output.AddField(If(d_o.BagnaturaEffettiva, 1, 0))

            output.Commit()
        Next

        Return output.Output()
    End Function


    Private Function OUT_coorti() As String

        Dim output As New OutputModello

        'i18n
        output.aggiungiColonna("Num", GetType(Integer), My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_Peronospora_numeroCoorteOospore, "0")
        output.aggiungiColonna("PMO", GetType(Decimal), My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_Peronospora_intensitàCoorteOospore & " (%)", "0.00")
        Dim indicator = output.aggiungiIndicatore("PMO_IND", {0.02, 0.05, System.Decimal.MaxValue}, OutputIndicator.enum_IndicatorType.Numbers).setFieldVal("PMO")
        output.aggiungiColonna("GER0", GetType(DateTime), My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_Peronospora_dataInizioGerminazioneOospore, "dd/MM/yyyy HH") '..., "dd/MM/yyyy HH\""h\""")
        output.aggiungiColonna("GER1", "% " & My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_Peronospora_germinazioneDataEmissioneSporangi)
        output.aggiungiColonna("ZRE", My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_Peronospora_dataRilascioZoospore)
        output.aggiungiColonna("ZDI", My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_Peronospora_dataDispersioneZoosporePerPioggia)
        output.aggiungiColonna("info", My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_Peronospora_destinoCoorte)
        output.aggiungiColonna("incub2", Gias.Da)._gruppoColonne = "% " & My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_Peronospora_incubazioneDataComparsaSintomi
        output.aggiungiColonna("incub1", Gias.A)._gruppoColonne = "% " & My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Agronomica30_Peronospora_incubazioneDataComparsaSintomi

        Dim dtFormat As String = "dd/MM/yyyy HH" '"dd/MM/yyyy HH\h"

        Dim num As Integer = 0
        For Each c In _collezione

            num += 1

            output.AddField(num)
            output.AddField(c.PMO * 100D)
            output.AddField(indicator.colorForVal(c.PMO))
            output.AddField(c.data_pioggia_partenza_GER0)
            output.AddField(c.GER1_mature.AsString(dtFormat))
            output.AddField(If(c.data_rilascio_zoospore > Date.MinValue, c.data_rilascio_zoospore.ToString(dtFormat), ""))
            output.AddField(If(c.data_pioggia_dispersione_zoospore > Date.MinValue, c.data_pioggia_dispersione_zoospore.ToString(dtFormat), ""))
            output.AddField(c.DestinoAsString())
            output.AddField(c.incub2.AsString(dtFormat))
            output.AddField(c.incub1.AsString(dtFormat))

            output.Commit()
        Next

        Return output.Output({indicator}.ToList())
    End Function


    Private Function OUT_coorti_time() As String

        If _collezione.Count = 0 Then
            Return ""
        End If

        Dim output As New OutputModello
        output.aggiungiColonna("Data", GetType(DateTime), Gias.Data, "dd/MM/yyyy")

        Dim ic As Integer = 0
        Dim last_dt As DateTime = _collezione(0).data_pioggia_partenza_GER0
        Dim coorte_last_dt As DateTime
        Dim fname As String
        While ic < _collezione.Count

            fname = "CoorteVal" & CStr(ic + 1)
            output.aggiungiColonna(fname, GetType(Decimal), CStr(ic + 1), "0.00")

            fname = "CoorteClr" & CStr(ic + 1)
            output.aggiungiColonna(fname, "")._hidden = True

            fname = "CoorteBar" & CStr(ic + 1)
            output.aggiungiColonna(fname, GetType(Decimal), "", "0.00")._hidden = True

            coorte_last_dt = _collezione(ic).data_pioggia_partenza_GER0.AddDays(_collezione(ic).progGER.Count - 1)
            If coorte_last_dt > last_dt Then
                last_dt = coorte_last_dt
            End If

            ic += 1
        End While

        Dim current_dt As DateTime = _collezione(0).data_pioggia_partenza_GER0
        Dim doy As Integer = current_dt.DayOfYear
        Dim doy1, doy2 As Integer
        Dim indicator = OutputHelper.createIndicator("", {0, 0.71, 0.85, 1}, OutputIndicator.enum_IndicatorType.Colors)
        Dim val As Decimal
        While current_dt <= last_dt

            output.AddField(current_dt)

            ic = 0
            While ic < _collezione.Count

                doy1 = _collezione(ic).data_pioggia_partenza_GER0.DayOfYear
                doy2 = doy1 + _collezione(ic).progGER.Count - 1
                If doy1 <= doy Then

                    If doy <= doy2 Then

                        val = _collezione(ic).progGER(doy - doy1)
                        output.AddField(val)
                        output.AddField(indicator.gradientForVal(val))

                    Else

                        output.AddField(DBNull.Value)
                        output.AddField(DBNull.Value)
                    End If

                    output.AddField(_collezione(ic).PMO * 100D) 'output.AddField(1)

                Else

                    ic = _collezione.Count
                End If

                ic += 1
            End While

            output.Commit()

            current_dt = current_dt.AddDays(1)
            doy += 1
        End While

        Return output.Output()
    End Function

End Class

