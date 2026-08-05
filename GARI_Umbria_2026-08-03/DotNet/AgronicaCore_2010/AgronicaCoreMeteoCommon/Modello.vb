Public Class _key_modello : Implements IEquatable(Of _key_modello)

    Public mod_cod As Integer
    Public veg_cod As Integer
    Public avv_cod As Integer
    Public alg_cod As Integer
    Public params As String

    Public Overrides Function Equals(other As Object) As Boolean
        If other.GetType Is GetType(_key_modello) Then
            Return _equals(CType(other, _key_modello))
        End If
        Return False
    End Function

    Public Function _equals(other As _key_modello) As Boolean Implements IEquatable(Of _key_modello).Equals
        If mod_cod <> other.mod_cod Then
            Return False
        End If
        If veg_cod <> other.veg_cod Then
            Return False
        End If
        If avv_cod <> other.avv_cod Then
            Return False
        End If
        If alg_cod <> other.alg_cod Then
            Return False
        End If
        Return params.CompareTo(other.params) = 0
    End Function

    Public Overrides Function GetHashCode() As Integer
        Return 0
    End Function

    Public Shared Function TryNew(ByVal f_mod_cod As Object, ByVal f_veg_cod As Object, ByVal f_avv_cod As Object, ByVal f_alg_cod As Object, ByVal f_params As Object) As _key_modello

        If IsDBNull(f_mod_cod) Then
            Return Nothing
        End If
        If IsDBNull(f_veg_cod) Then
            Return Nothing
        End If
        If IsDBNull(f_avv_cod) Then
            Return Nothing
        End If
        If IsDBNull(f_alg_cod) Then
            Return Nothing
        End If

        Dim _params As String = ""
        If Not IsDBNull(f_params) Then
            _params = f_params.ToString
        End If

        Return New _key_modello With {
            .mod_cod = CInt(f_mod_cod),
            .veg_cod = CInt(f_veg_cod),
            .avv_cod = CInt(f_avv_cod),
            .alg_cod = CInt(f_alg_cod),
            .params = _params
        }
    End Function
End Class

Public Class _modello
    Public key_modello As _key_modello
    Public veg_des As String
    Public mod_name As String
    Public avv_name As String
    Public param_des As String
    Public periodo_des As String
    Public validita_des As String
    Public flag_completo As Boolean
End Class

Public Class _stazione_x_modelli
    Inherits _stazione

    Public modelli As List(Of _modello)
    Public Sub New(ByVal ks As _key_stazione)
        MyBase.New(ks)
        modelli = New List(Of _modello)
    End Sub
End Class
