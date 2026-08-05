Imports System



''' <summary>
''' Questa classe serve per convertire i webcontrol in valori da salvare
''' </summary>
''' <remarks></remarks>
Public Class SalvataggioWebControl
    Private Structure MessaggiErrore
        Public Messaggio As String
    End Structure
    Private _messaggi As MessaggiErrore

    Public Property Messaggi() As String
        Get
            Return _messaggi.Messaggio
        End Get
        Set(ByVal value As String)
        End Set
    End Property


    Sub New()
        _messaggi.Messaggio = ""
    End Sub


    ''' <summary>
    ''' Aggiunge alla proprietà messaggio dell'oggetto l'errore in formato stringa passato
    ''' </summary>
    ''' <param name="mess">messaggio errore da concatenare</param>
    Public Sub AggiungiMessaggioErrore(mess As String)
        _messaggi.Messaggio += mess.Trim + vbCrLf
    End Sub

    Public Sub Controlla(controllo As HtmlInputHidden, ByRef ritorno As Object, defaultValue As Object)
        If controllo.Value <> "" Then
            Try
                ritorno = Converti(controllo.Value, ritorno)
            Catch ex As Exception
                _messaggi.Messaggio += ex.Message + vbCrLf
            End Try
        Else
            ritorno = defaultValue
        End If
    End Sub

    Public Sub Controlla(controllo As TextBox, ByRef ritorno As Object, defaultValue As Object)
        If controllo.Text <> "" Then
            Try
                ritorno = Converti(controllo.Text, ritorno)
            Catch ex As Exception
                _messaggi.Messaggio += ex.Message + vbCrLf
            End Try
        Else
            ritorno = defaultValue
        End If
    End Sub

    Public Sub Controlla(controllo As DropDownList, ByRef ritorno As Object, defaultValue As Object)
        If controllo.Text <> "" Then
            Try
                ritorno = Converti(controllo.SelectedValue, ritorno)
            Catch ex As Exception
                _messaggi.Messaggio += ex.Message + vbCrLf
            End Try
        Else
            ritorno = defaultValue
        End If
    End Sub

    '  Galassi, 03/07/2017 12.17.38: Non sono da usare perchè non rilevano le modifiche lato client in asp
    'Public Sub Controlla(controllo As Label, ByRef ritorno As Object, defaultValue As Object)
    '    If controllo.Text <> "" Then
    '        Try
    '            ritorno = converti(controllo.Text, ritorno)
    '        Catch ex As Exception
    '            _messaggi.Messaggio += ex.Message + vbCrLf
    '        End Try
    '    Else
    '        ritorno = defaultValue
    '    End If
    'End Sub



    Private Function Converti(ritorno As String, tipo As Object) As Object
        Select Case tipo.GetType()
            Case System.Type.GetType("System.String")
                Return CType(ritorno, String)
            Case System.Type.GetType("System.Integer")
                Return CType(ritorno, Integer)
            Case System.Type.GetType("System.Int32")
                Return CType(ritorno, System.Int32)
            Case System.Type.GetType("System.Int64")
                Return CType(ritorno, System.Int64)
            Case System.Type.GetType("System.Int16")
                Return CType(ritorno, System.Int16)
            Case System.Type.GetType("System.Long")
                Return CType(ritorno, Long)
            Case System.Type.GetType("System.Short")
                Return CType(ritorno, Short)
            Case System.Type.GetType("System.Single")
                Return CType(ritorno, Single)
            Case System.Type.GetType("System.Decimal")
                Return CType(ritorno, Decimal)
            Case System.Type.GetType("System.Double")
                Return CType(ritorno, Double)
            Case System.Type.GetType("System.Boolean")
                Return CType(ritorno, Boolean)
            Case System.Type.GetType("System.Date")
                Return CType(ritorno, Date)
            Case System.Type.GetType("System.DateTime")
                Return CType(ritorno, DateTime)
            Case Else
                Return Nothing
        End Select
    End Function
End Class
