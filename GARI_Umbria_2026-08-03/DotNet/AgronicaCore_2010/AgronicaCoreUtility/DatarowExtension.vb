Imports System.Runtime.CompilerServices
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Module DatarowExtension
    ''' <summary>
    ''' Converte i dati in un formato stringa
    ''' </summary>
    ''' <param name="value"></param>
    ''' <param name="campo"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function ToStringa(value As DataRow, campo As String) As String

        If IsDBNull(value(campo)) Then
            Return Nothing
        End If

        If IsNothing(value(campo)) Then
            Return Nothing 'String.Empty
        End If

        Return value(campo).ToString()
    End Function
    ''' <summary>
    ''' Converte i dati in un formato intero
    ''' </summary>
    ''' <param name="value"></param>
    ''' <param name="campo"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function ToIntero(value As DataRow, campo As String) As Integer?

        If IsDBNull(value(campo)) Then
            Return Nothing
        End If

        If IsNothing(value(campo)) Then
            Return 0 'Nothing
        End If
        If Not IsNumeric(value(campo)) Then
            Return 0 'Nothing
        End If
        Return Convert.ToInt32(value(campo))
    End Function
    ''' <summary>
    ''' Converte i dati in un formato decimal
    ''' </summary>
    ''' <param name="value"></param>
    ''' <param name="campo"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function ToDecimale(value As DataRow, campo As String) As Decimal?

        If IsDBNull(value(campo)) Then
            Return Nothing
        End If

        If IsNothing(value(campo)) Then
            Return 0 'Nothing
        End If
        If Not IsNumeric(value(campo)) Then
            Return 0 'Nothing
        End If
        Return Convert.ToDecimal(value(campo))
    End Function
    ''' <summary>
    ''' Converte i dati in un formato date. Se la data non è valida 
    ''' forza la costante AGRODATAINIZIO
    ''' </summary>
    ''' <param name="value"></param>
    ''' <param name="campo"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function ToDataInizio(value As DataRow, campo As String) As Date

        Dim ret As Object
        If IsNothing(value(campo)) Then
            Return AGRODATAINIZIO
        End If

        If value(campo).Equals(DBNull.Value) Then
            ret = Nothing
        Else
            If Not IsDate(value(campo)) Then
                ret = AGRODATAINIZIO
            End If
            ret = Convert.ToDateTime(value(campo))
        End If

        If value(campo).Equals(AGRODATAINIZIO) Then
            ret = Nothing
        End If

        Return ret
    End Function

    ''' <summary>
    ''' Converte i dati in un formato date. Se la data non è valida 
    ''' forza la costante AGRODATAFINE
    ''' </summary>
    ''' <param name="value"></param>
    ''' <param name="campo"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function ToDataFine(value As DataRow, campo As String) As Date

        Dim ret As Object
        If IsNothing(value(campo)) Then
            Return AGRODATAFINE
        End If

        If value(campo).Equals(DBNull.Value) Then
            ret = Nothing
        Else
            If Not IsDate(value(campo)) Then
                ret = AGRODATAFINE
            End If
            ret = Convert.ToDateTime(value(campo))
        End If

        If value(campo).Equals(AGRODATAFINE) Then
            ret = Nothing
        End If

        Return ret
    End Function
    ''' <summary>
    ''' Converte i dati in un formato booleano. Se il dato non è valido forza false di default
    ''' </summary>
    ''' <param name="value"></param>
    ''' <param name="campo"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function ToBooleano(value As DataRow, campo As String) As Boolean

        If IsDBNull(value(campo)) Then
            Return False
        End If

        If IsNothing(value(campo)) Then
            Return False
        End If

        Return Convert.ToBoolean(value(campo))
    End Function
    ''' <summary>
    ''' Converte i dati in un formato Single
    ''' </summary>
    ''' <param name="value"></param>
    ''' <param name="campo"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function ToSingolo(value As DataRow, campo As String) As Single

        If IsNothing(value(campo)) Then
            Return 0 'Nothing
        End If

        If Not IsNumeric(value(campo)) Then value(campo) = 0
        Return Convert.ToSingle(value(campo))
    End Function

    ''' <summary>
    ''' Converte i dati in un formato Short (Intero 16 bit)
    ''' </summary>
    ''' <param name="value"></param>
    ''' <param name="campo"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function ToShort(value As DataRow, campo As String) As Short

        If IsNothing(value(campo)) Then
            Return 0 'Nothing
        End If

        If Not IsNumeric(value(campo)) Then value(campo) = 0
        Return Convert.ToInt16(value(campo))
    End Function

End Module
