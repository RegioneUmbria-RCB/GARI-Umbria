
Imports System.Data.OleDb

Public Class FF_Etichette_General



    Public Enum TipiLayout
        Confezione = 0
        Imballo = 1
        Pallet = 2
    End Enum

    Public Structure CodiceDescrizione
        Dim Codice As String
        Dim Descrizione As String
    End Structure

    Public Shared Function DBNullToNothing(ByVal valore As Object) As Object
        If System.Convert.IsDBNull(valore) Then
            DBNullToNothing = Nothing
        Else
            DBNullToNothing = valore
        End If
    End Function
    Public Shared Function NothingToDBNull(ByVal valore As Object) As Object
        If valore Is Nothing Then
            NothingToDBNull = DBNull.Value
        Else
            NothingToDBNull = valore
        End If
    End Function

    Public Shared Function SeparatoreCodiceDescrizione() As String
        Return "-"
    End Function
    Public Shared Function SeparaCodiceDescrizione(ByVal Valore As String) As CodiceDescrizione
        Dim cd As CodiceDescrizione
        Try
            Dim sep As String = SeparatoreCodiceDescrizione()
            If Valore.IndexOf(sep) > 0 Then
                cd.Codice = Valore.Substring(0, Valore.IndexOf(sep)).Trim()
                cd.Descrizione = Valore.Substring(Valore.IndexOf(sep) + sep.Length).Trim()
            ElseIf Valore.LastIndexOf(sep) > 0 Then
                cd.Codice = Valore.Substring(0, Valore.LastIndexOf(sep)).Trim()
                cd.Descrizione = Valore.Substring(Valore.LastIndexOf(sep) + sep.Length).Trim()
            End If
        Catch ex As Exception
            'DO NOTHING
        End Try
        Return cd
    End Function


    Public Shared Function DatiUscita(ByVal TipoLayout As TipiLayout) As DataTable
        Try

            'vanni, todo

            Dim dt As DataTable
            Return dt
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
    Public Shared Function DatiLayout(ByVal Layout As String) As DataTable
        Try

            'vanni, todo
            Return New DataTable
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Shared Function CodiceProgramma(ByVal CodiceImballo As String) As String
        Try

            'vanni, todo


            Return ""
        Catch ex As Exception
            Return String.Empty
        End Try

    End Function

    Public Shared Function ParametriObbligatori() As DataTable
        Try

            'vanni, todo

            Return New DataTable
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
    Public Shared Function TraduzioneParametro(ByVal ValoreParametroModelloDocumento As String, ByVal Lingua As String) As String
        Try

            'vanni , todo

        Catch ex As Exception
            Return String.Empty
        End Try
    End Function
    Public Shared Function TraduzioneLabel(ByVal CodiceLabel As String, ByVal Lingua As String) As String
        Try

            'vanni, todo

        Catch ex As Exception
            Return String.Empty
        End Try

    End Function

End Class
