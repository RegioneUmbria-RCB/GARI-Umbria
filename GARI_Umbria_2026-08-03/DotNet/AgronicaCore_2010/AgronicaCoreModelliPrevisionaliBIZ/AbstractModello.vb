Imports System.Runtime.Serialization
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json.Linq

Public Class RisultatoElaborazioneIndicatori

    Public Class Indicatore
        Public Class ParametriElaborazione
            Public Tipo_Sorgente As Integer
            Public Stazione_Cod As Integer
            Public Mod_Cod As Integer
            Public Veg_Cod As Integer
            Public Avv_Cod As Integer
            Public Alg_Cod As Integer
            Public ParametriElaborazione As String
        End Class

        Public Class RisultatoElaborazione

            Public Enum Stato_Elaborazione
                _OutOfRange = -1
                _Valid = 0
                _Warning = 1
                _Error = 2
                _ProgressBefore = 3
            End Enum

            Public Class Band
                Public Value As Decimal
                Public Color As String
            End Class

            Public DataInizio As String
            Public DataFine As String
            Public Scale_Min As Decimal
            Public Scale_Max As Decimal
            Public Bands As List(Of Band)
            Public Value As Decimal

            Public Status As Stato_Elaborazione
            Public StatusMsg As String
            Public AuxMsg As String

            Public MeteoStatus As MeteoDSS_Helper.Enum_MeteoStatus
            Public MeteoMsg As String

            <DataContract>
            Public Class OutputGridValue
                <DataMember>
                Public Property field As String
                <DataMember>
                Public Property title As String
                <DataMember>
                Public Property valueType As String
                <DataMember>
                Public Property value As Object
                <DataMember>
                Public Property outputFormat As String
                Public Sub New()
                    field = ""
                    title = ""
                    valueType = ""
                    value = Nothing
                End Sub
                Public Sub New(f As String, t As String)
                    field = f
                    title = t
                    valueType = ""
                    value = Nothing
                End Sub
                Public Sub SetValue(Of T)(v As T, Optional f As String = "")
                    valueType = GetType(T).Name
                    value = v
                    outputFormat = f
                End Sub
            End Class

            Public OutputGridValues As List(Of OutputGridValue)

            Public Class EventoPioggia
                Public InizioEvento As String
                Public Cumulato_mm As Decimal
                Public Durata_hh As Integer
            End Class

            Public EventiPioggia As List(Of EventoPioggia)

            Public Sub New()
                Bands = New List(Of Band)
                Status = Stato_Elaborazione._Valid
                StatusMsg = ""
                AuxMsg = ""
                OutputGridValues = New List(Of OutputGridValue)
                EventiPioggia = New List(Of EventoPioggia)
            End Sub

            Public Sub Fill(_value As Decimal, _scale_max As Decimal, _bands_values() As Decimal)

                Value = _value
                Scale_Min = 0
                Scale_Max = Math.Max(_scale_max, _value * 1.1)

                Dim b As Integer = 0
                Dim iBand As Band
                While b < _bands_values.Length()
                    iBand = New Band
                    iBand.Value = _bands_values(b)
                    Select Case b
                        Case 0
                            iBand.Color = "#008000" 'verde
                        Case 1
                            iBand.Color = "#fff000" 'giallo
                        Case 2
                            iBand.Color = "#ff8000" 'arancio
                        Case Else
                            iBand.Color = "#f00000" 'rosso
                    End Select
                    Bands.Add(iBand)
                    b += 1
                End While
                iBand = New Band With {.Value = Decimal.MaxValue, .Color = "#f00000"}
                iBand.Value = Decimal.MaxValue
                Bands.Add(iBand)
            End Sub

            Public Sub FillCustom(_value As Decimal, _scale_max As Decimal, _bands As List(Of Band))

                Value = _value
                Scale_Min = 0
                Scale_Max = Math.Max(_scale_max, _value * 1.1)

                Bands.Clear()
                For Each b In _bands
                    Bands.Add(b)
                Next
            End Sub

            Public Sub Errore(strerr As String)
                Status = Stato_Elaborazione._Error
                StatusMsg = strerr
            End Sub

            Public Sub InProgress(perc As Decimal, msg As String)
                Status = Stato_Elaborazione._ProgressBefore
                StatusMsg = msg
                Value = perc
            End Sub

        End Class

        Public Parametri As ParametriElaborazione
        Public Risultato As RisultatoElaborazione

        Public Sub New()
            Parametri = New ParametriElaborazione
            Risultato = New RisultatoElaborazione
        End Sub
    End Class

    Public Enum Stato_Elaborazione
        NonDisponibili = -1
        InAttesa = 0
        Pronti = 1
    End Enum

    Public Stato As Stato_Elaborazione
    Public Indicatori As List(Of Indicatore)
End Class



Public Class IndicatoreXSorgente
    Public TipoSorgente As Integer
    Public StazioneSorgente As Integer
    Public RisultatoElaborazione As RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione
End Class

Public MustInherit Class AbstractModello

    Protected Class LettoreParametri
        Private ReadOnly _objParams As JObject
        Public Sub New(ByVal strParam As String)
            If Not String.IsNullOrEmpty(strParam) Then
                _objParams = JObject.Parse(strParam)
            Else
                _objParams = Nothing
            End If
        End Sub
        Public Function DateOrDefault(ByVal prop_name As String, ByVal def_dt As Date) As Date
            Dim res_dt As Date = def_dt
            If _objParams IsNot Nothing AndAlso _objParams(prop_name) IsNot Nothing Then
                res_dt = Convert.ToDateTime(_objParams(prop_name)).ToLocalTime
            End If
            Return res_dt
        End Function
        Public Function IntOrDefault(ByVal prop_name As String, ByVal def_val As Integer) As Integer
            Dim res_val As Integer = def_val
            If _objParams IsNot Nothing AndAlso _objParams(prop_name) IsNot Nothing Then
                res_val = CInt(_objParams(prop_name))
            End If
            Return res_val
        End Function
        Public Function GetObject(prop_name As String) As JObject
            If _objParams IsNot Nothing AndAlso _objParams(prop_name) IsNot Nothing Then
                Return _objParams(prop_name)
            End If
            Return Nothing
        End Function
        Public Function StringOrDefault(ByVal prop_name As String, ByVal def_val As String) As String
            Dim res_val As String = def_val
            If _objParams IsNot Nothing AndAlso _objParams(prop_name) IsNot Nothing Then
                res_val = _objParams(prop_name).ToString()
            End If
            Return res_val
        End Function
        Public Function OutputFromDate(ByVal prop_name As String, ByVal str_out As String) As String
            Dim str_dt As String = ""
            Dim dt As Date = DateOrDefault(prop_name, Date.MinValue)
            If dt > Date.MinValue Then
                str_dt = str_out & dt.ToString("d")
            End If
            Return str_dt
        End Function
        Public Function OutputFromDOY(ByVal prop_name As String, ByVal str_out As String) As String
            Dim str_dt As String = ""
            Dim doy As Integer = IntOrDefault(prop_name, 0)
            If doy > 0 Then

                Dim dt = New Date(1999, 1, 1) '1999 Anno non bisestile
                dt = dt.AddDays(doy - 1)

                str_dt = str_out & dt.ToString("m")
            End If
            Return str_dt
        End Function
    End Class

    Protected _datiMeteo As MeteoReadOnlyList
    Protected _errore As String

    Public Sub New(datiMeteo As MeteoReadOnlyList)
        _datiMeteo = datiMeteo
        _errore = ""
    End Sub

    Public Function ElaboraModello() As rispostaStandard(Of cRisultatoModello)

        Dim risp As New rispostaStandard(Of cRisultatoModello) With {
            .RispostaStringa = Nothing,
            .RispostaOK = False,
            .Errore = ""
        }

        Dim risModello = _elaboraModello()

        If risModello Is Nothing Then

            risModello = New cRisultatoModello
            risp.Errore = _errore
        Else

            risp.RispostaOK = True
        End If

        risp.RispostaStringa = risModello

        Return risp
    End Function

    Public Function ElaboraIndicatore_Vecchio(ByVal dataInizio As DateTime, ByVal dataFine As DateTime) As rispostaStandard(Of cRisultatoModelloIndicatori.Indicatore)

        Dim risp As New rispostaStandard(Of cRisultatoModelloIndicatori.Indicatore) With {
            .RispostaStringa = Nothing,
            .RispostaOK = False,
            .Errore = ""
        }

        Dim risIndicatore = _elaboraIndicatore_vecchio(dataInizio, dataFine)

        If risIndicatore.Status = cRisultatoModelloIndicatori.Indicatore.enum_Status.Status_Error Then

            risp.Errore = _errore
        Else

            risp.RispostaOK = True
        End If

        risp.RispostaStringa = risIndicatore

        Return risp
    End Function

    Public Function ElaboraIndicatore(meteoHelper As MeteoDSS_Helper) As RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione

        Dim risElab As New RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione With {
            .DataInizio = meteoHelper.DatiMeteo.First.DataOra.Date.ToString("u"),
            .DataFine = meteoHelper.DatiMeteo.Last.DataOra.Date.ToString("u"),
            .MeteoStatus = meteoHelper.Status,
            .MeteoMsg = meteoHelper.StatusMsg
        }

        Return _elaboraIndicatore(risElab)
    End Function

    Protected MustOverride Function _elaboraModello() As cRisultatoModello
    Protected MustOverride Function _elaboraIndicatore_vecchio(dataInizio As DateTime, dataFine As DateTime) As cRisultatoModelloIndicatori.Indicatore
    Protected MustOverride Function _elaboraIndicatore(risElab As RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione) As RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione

    Protected Function _data_from_doy(ByVal dt As Date, ByVal doy As Integer, ByVal def_month As Integer, ByVal def_day As Integer) As Date

        Dim def_year As Integer = Now().Year

        If _datiMeteo.Any() Then

            def_year = _datiMeteo.First.DataOra.Year
        End If

        Dim res_dt As Date = New Date(def_year, def_month, def_day)

        If _datiMeteo.Any() Then

            If dt > Date.MinValue Then

                res_dt = dt
            Else

                If doy > 0 Then

                    res_dt = New Date(1999, 1, 1) '1999 Anno non bisestile
                    res_dt = res_dt.AddDays(doy - 1)
                    Dim month As Integer = res_dt.Month
                    Dim day As Integer = res_dt.Day
                    res_dt = New Date(def_year, month, day)
                End If
            End If

            If res_dt < _datiMeteo.First.DataOra.Date Then

                res_dt = res_dt.AddYears(1)
            End If
        End If

        Return res_dt.Date
    End Function
End Class

