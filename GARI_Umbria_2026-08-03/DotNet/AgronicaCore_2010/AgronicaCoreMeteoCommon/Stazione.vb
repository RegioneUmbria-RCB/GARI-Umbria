Imports Newtonsoft.Json.Linq

Public Class _key_stazione : Implements IEquatable(Of _key_stazione)

    Public tipo_sorgente As Integer
    Public stazione_cod As Integer

    Public Overrides Function Equals(other As Object) As Boolean
        If other.GetType Is GetType(_key_stazione) Then
            Return _equals(CType(other, _key_stazione))
        End If
        Return False
    End Function

    Public Function _equals(other As _key_stazione) As Boolean Implements IEquatable(Of _key_stazione).Equals
        If tipo_sorgente <> other.tipo_sorgente Then
            Return False
        End If
        If stazione_cod <> other.stazione_cod Then
            Return False
        End If
        Return True
    End Function

    Public Overrides Function GetHashCode() As Integer
        Return 0
    End Function

    Public Shared Function TryNew(ByVal f_tipo_sorgente As Object, ByVal f_stazione_cod As Object) As _key_stazione

        If IsDBNull(f_tipo_sorgente) Then
            Return Nothing
        End If
        If IsDBNull(f_stazione_cod) Then
            Return Nothing
        End If

        Return New _key_stazione With {.tipo_sorgente = CInt(f_tipo_sorgente), .stazione_cod = CInt(f_stazione_cod)}
    End Function
End Class

Public Class _stazione
    Public key_stazione As _key_stazione
    Public stazione_name As String
    Public rif_fornitore As String
    Public flag_reale As Boolean
    Public sensori_des As String
    Public Sub New(ByVal ks As _key_stazione)
        key_stazione = ks
        stazione_name = ""
        rif_fornitore = ""
        flag_reale = True
        sensori_des = ""
    End Sub
End Class

Public Class _elenco_stazioni

    Private _dict As Dictionary(Of _key_stazione, _stazione)
    Private _list As List(Of _stazione)

    Public Sub New()
        _dict = New Dictionary(Of _key_stazione, _stazione)
        _list = New List(Of _stazione)
    End Sub

    Public Function TryAdd(ByRef staz As _stazione) As _stazione
        If Not _dict.ContainsKey(staz.key_stazione) Then
            _dict.Add(staz.key_stazione, staz)
            _list.Add(staz)
        Else
            staz = _dict(staz.key_stazione)
        End If
        Return staz
    End Function
    Public Function GetList() As List(Of _stazione)
        Return _list
    End Function
    Public Function GetList(Of T)() As List(Of T)
        Return _list.OfType(Of T).ToList()
    End Function
    Public Function StazioneFromKey(ByVal ks As _key_stazione) As _stazione
        If _dict.ContainsKey(ks) Then
            Return _dict(ks)
        End If
        Return Nothing
    End Function
    Public Function DictOfTipoSorgente() As Dictionary(Of Integer, List(Of Integer))
        Dim stazXsorg As New Dictionary(Of Integer, List(Of Integer))
        For Each ks In _dict.Keys
            If stazXsorg.ContainsKey(ks.tipo_sorgente) Then
                stazXsorg(ks.tipo_sorgente).Add(ks.stazione_cod)
            Else
                stazXsorg.Add(ks.tipo_sorgente, New List(Of Integer) From {ks.stazione_cod})
            End If
        Next
        Return stazXsorg
    End Function
End Class

Public Class _centro
    Public sa_cod As Integer
    Public sa_nome As String
End Class

Public Class _stazione_x_centri
    Inherits _stazione

    Public centri As List(Of _centro)
    Public Sub New(ByVal ks As _key_stazione)
        MyBase.New(ks)
        centri = New List(Of _centro)
    End Sub
End Class


Public Class _stazione_x_monitor
    Inherits _stazione

    Public monitor_ore As Integer
    Public Sub New(ByVal ks As _key_stazione, ByVal _monitor_ore As Integer)
        MyBase.New(ks)
        monitor_ore = _monitor_ore
    End Sub
End Class


Public Class _stazione_x_controllo
    Inherits _stazione

    Private parametriCompleti As String
    Private parametriDaSerializzare As String

    Public ReadOnly Property parametri As Object
        Get
            Dim objres As JObject = Nothing

            Try

                Dim obj = JObject.Parse(parametriCompleti)

                objres = obj(parametriDaSerializzare)

            Catch ex As Exception

            End Try

            Return objres
        End Get
    End Property

    Public Sub New(ByVal ks As _key_stazione, ByVal parametri As String, ByVal gruppoParametri As String)
        MyBase.New(ks)

        parametriCompleti = parametri
        parametriDaSerializzare = gruppoParametri
    End Sub

End Class

Public Class _stazione_x_indicatori
    Inherits _stazione

    Public indicatori As List(Of OutputRisultatoIndicatori.OutputIndicatore)
    Public Sub New(ByVal ks As _key_stazione)
        MyBase.New(ks)
        indicatori = New List(Of OutputRisultatoIndicatori.OutputIndicatore)
    End Sub

End Class
