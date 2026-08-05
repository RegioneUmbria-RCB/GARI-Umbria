


Public Class cRisultatoModello

    Public Property Modello_Titolo() As String
    Public Property Modello_Descrizione() As String
    Public Property Modello_Tabella1() As String
    Public Property Modello_Tabella2() As String
    Public Property Modello_WarningMsg() As String
    Public Property Modello_Disclaimer() As String
    Public Property Modello_TabellaMeteo() As String
    Public Property Modello_EngineId() As Integer
    Public Property Modello_CodiceEsterno() As String
End Class

'Public Delegate Function ResxFunction() As String

Public Class cRisultatoModelloIndicatori
    Public Class Indicatore

        Public Enum enum_Status
            Status_Invalid = -1
            Status_Valid = 0
            Status_Warning = 1
            Status_Error = 2
        End Enum
        Public Class Band
            Public Property Value As Decimal
            Public Property Color As String
        End Class
        Public Property Modello As String
        Public Property Centro As String
        Public Property Specie As String
        Public Property Stazione As String
        Public Property Scale_Min As Decimal
        Public Property Scale_Max As Decimal
        Public Property Bands As List(Of Band)
        Public Property Value As Decimal
        Public Property AuxMsg As String
        'Private Property _AuxMsg As Dictionary(Of String, String)
        Public Property Status As enum_Status
        Public Property StatusMsg As String
        'Private Property _StatusMsg As Dictionary(Of String, String)

        Public Class CalcParams
            Public Property DataInizio As String
            Public Property DataFine As String
            Public Property Tipo_Sorgente As Integer
            Public Property Stazione_Cod As Integer
            Public Property Veg_Cod As Integer
            Public Property Mod_Cod As Integer
            Public Sub New()
                DataInizio = ""
                DataFine = ""
                Tipo_Sorgente = 0
                Stazione_Cod = 0
                Veg_Cod = 0
                Mod_Cod = 0
            End Sub
        End Class

        Public Property Params As CalcParams

        'serve solo per la serializzazione
        Public Sub New()
            Modello = ""
            Centro = ""
            Specie = ""
            Stazione = ""
            Bands = New List(Of Band)
            AuxMsg = ""
            '_AuxMsg = New Dictionary(Of String, String)
            Status = enum_Status.Status_Valid
            StatusMsg = ""
            '_StatusMsg = New Dictionary(Of String, String)
            Params = New CalcParams
        End Sub
        Public Sub New(ByVal _modello As String, ByVal dataInizio As DateTime, ByVal dataFine As DateTime)
            Me.New()
            Modello = _modello
            Params.DataInizio = dataInizio.Date.ToString("u")
            Params.DataFine = dataFine.Date.ToString("u")
        End Sub
        Public Sub Fill(ByVal _value As Decimal, ByVal _scale_max As Decimal, ByVal _bands_values() As Decimal, ByVal elab_dt As DateTime, ByVal req_dt As DateTime)

            Value = _value
            Scale_Min = 0
            Scale_Max = Math.Max(_scale_max, _value * 1.1)

            Dim b As Integer = 0
            Dim iBand As Indicatore.Band
            While b < _bands_values.Length()
                iBand = New Indicatore.Band
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
            iBand = New Indicatore.Band With {.Value = Decimal.MaxValue, .Color = "#ff0000"}
            iBand.Value = Decimal.MaxValue
            Bands.Add(iBand)

            If DateDiff(DateInterval.Hour, elab_dt, req_dt) > 24 Then
                Status = enum_Status.Status_Warning
                'Dim msg As String = String.Format(My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Indicatore_DatiMeteoFinoAl, elab_dt.ToShortDateString())
                'StatusMsg = "Dati meteo fino al " & elab_dt.ToShortDateString()
                StatusMsg = String.Format(My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Indicatore_DatiMeteoFinoAl, elab_dt.ToShortDateString())

                'CallByName(AddressOf My.Resources.AgronicaCoreModelliPrevisionaliBIZ, Indicatore_DatiMeteoFinoAl, CallType.Get)
                'SetStatusMsg(AddressOf My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Indicatore_DatiMeteoFinoAl)
            End If

        End Sub
        'Public Sub SetStatusMsg(ByVal resF As ResxFunction)

        '    Dim resMan = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.ResourceManager

        '    Dim culture As New List(Of String) From {"it", "fr"}

        '    For Each c In culture
        '        Dim resCulture As New Global.System.Globalization.CultureInfo(c)
        '        resMan.GetString(resF(), resCulture)


        '    Next

        '    'If Not _StatusMsg.ContainsKey(CodiceISO_Lingua.ToLower) Then
        '    '    _StatusMsg.Add(CodiceISO_Lingua.ToLower, Text)
        '    'Else
        '    '    _StatusMsg(CodiceISO_Lingua.ToLower) = Text
        '    'End If
        'End Sub

        'Private Sub SetAuxMsg(ByVal resString As String)

        'End Sub

#If False Then
                'Dim tetstst = AddressOf My.Resources.AgronicaCoreModelliPrevisionaliBIZ.get_Agronomica30_TicchiolaturaMelo_raggiuntaFineCicloAvversita


                Dim testtsts = GetType(My.Resources.AgronicaCoreModelliPrevisionaliBIZ)

                Dim resString = "Agronomica30_TicchiolaturaMelo_raggiuntaFineCicloAvversita"

                Dim resMan = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.ResourceManager

                Dim culture As New List(Of String) From {"it", "en", "fr", "it-ch", "pt"}

                For Each c In culture
                    My.Resources.AgronicaCoreModelliPrevisionaliBIZ.Culture = New Global.System.Globalization.CultureInfo(c)

                    Dim resCulture As New Global.System.Globalization.CultureInfo(c)
                    Dim ttt As String = resMan.GetString(resString, resCulture)
                    ttt = ""

                Next
#End If

    End Class

    Public Enum enum_Stato

        NonDisponibili = -1
        InAttesa = 0
        Pronti = 1

    End Enum

    Public Property Stato As enum_Stato
    Public Property Indicatori As List(Of Indicatore)
End Class


