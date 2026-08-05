Imports System.Text.RegularExpressions

Public Class NomenclatoreParametri

    Private Const PARTE_FISSA_PARAM_STRING As String = "@AGSP"
    Private Const PARTE_FISSA_PARAM_DATE As String = "@AGDP"
    Private Const PARTE_FISSA_PARAM_NUMERIC As String = "@AGNP"

    Private Const PREFISSO_PARAM_STRING As String = "<§SPB_@§@§@§>"
    Private Const SUFFISSO_PARAM_STRING As String = "<§SPE_@§@§@§>"
    Private Const PREFISSO_PARAM_DATE As String = "<§DPB_@§@§@§>"
    Private Const SUFFISSO_PARAM_DATE As String = "<§DPE_@§@§@§>"
    Private Const PREFISSO_PARAM_NUMERIC As String = "<§NPB_@§@§@§>"
    Private Const SUFFISSO_PARAM_NUMERIC As String = "<§NPE_@§@§@§>"

    Private _mappaturaMetadati As Dictionary(Of Type, Tuple(Of String, String)) = New Dictionary(Of Type, Tuple(Of String, String))
    Private _mappaturaParametri As Dictionary(Of Type, String) = New Dictionary(Of Type, String)

    Private _objParametri As AgronicaCoreParametri = Nothing

    Sub New()

        _mappaturaMetadati.Add(GetType(String), New Tuple(Of String, String)(PREFISSO_PARAM_STRING, SUFFISSO_PARAM_STRING))
        _mappaturaMetadati.Add(GetType(Date), New Tuple(Of String, String)(PREFISSO_PARAM_DATE, SUFFISSO_PARAM_DATE))
        _mappaturaMetadati.Add(GetType(Decimal), New Tuple(Of String, String)(PREFISSO_PARAM_NUMERIC, SUFFISSO_PARAM_NUMERIC))

        _mappaturaParametri.Add(GetType(String), PARTE_FISSA_PARAM_STRING)
        _mappaturaParametri.Add(GetType(Date), PARTE_FISSA_PARAM_DATE)
        _mappaturaParametri.Add(GetType(Decimal), PARTE_FISSA_PARAM_NUMERIC)

    End Sub

    Sub New(ByRef objParametri As AgronicaCoreParametri)
        Me.New()
        _objParametri = objParametri
    End Sub

    Public Function OttieniTipoParametro(ByVal nomeParametro As String) As Type

        For Each kvp As KeyValuePair(Of Type, String) In _mappaturaParametri
            If nomeParametro.StartsWith(kvp.Value) Then
                Return kvp.Key
            End If
        Next

        Throw New Exception(String.Format("Tipo Parametro non riconosciuto: {0}", nomeParametro))

    End Function

    Public Function OttieniParteFissaNomeParametro(ByVal tipoParametro As Type) As String

        If Not _mappaturaParametri.ContainsKey(tipoParametro) Then
            Throw New Exception("Tipo Parametro non supportato")
        End If

        Return _mappaturaParametri(tipoParametro)

    End Function

    Public Function OttieniNomenclatura(ByVal tipoParametro As Type) As Tuple(Of String, String)

        If Not _mappaturaMetadati.ContainsKey(tipoParametro) Then
            Throw New Exception("Tipo Parametro non supportato")
        End If

        Return _mappaturaMetadati(tipoParametro)

    End Function

    Public Function RimuoviNomenclature(ByVal stringaSql As String) As String

        Dim origin As String = stringaSql

        For Each kvp As KeyValuePair(Of Type, Tuple(Of String, String)) In _mappaturaMetadati
            origin = Regex.Replace(origin, kvp.Value.Item1, "", RegexOptions.IgnoreCase, TimeSpan.FromSeconds(3))
            origin = Regex.Replace(origin, kvp.Value.Item2, "", RegexOptions.IgnoreCase, TimeSpan.FromSeconds(3))
        Next

        Return origin

    End Function

    Public Function ContieneNomenclatura(ByVal stringaSql As String) As Boolean

        For Each kvp As KeyValuePair(Of Type, Tuple(Of String, String)) In _mappaturaMetadati
            If stringaSql.Contains(kvp.Value.Item1) OrElse stringaSql.Contains(kvp.Value.Item2) Then
                Return True
            End If

        Next
        Return False

    End Function

End Class
