Imports System.Runtime.CompilerServices
Imports AgronicaCoreModelsSTD.anagrafiche

Namespace AnagrafeNG

    Public Class Rubrica

        Public Property Rubrica_Cod As Integer
        Public Property Tipologia As String
        Public Property Valore As String
        'public property Descrizione As String

    End Class
End Namespace

Module StringExtensions
    <Extension()>
    Public Function ToRubrica(ByVal rub As RubricaVoci) As AnagrafeNG.Rubrica
        Dim r As New AnagrafeNG.Rubrica()
        r.Rubrica_Cod = rub.rubrica.codice
        r.Tipologia = rub.rubrica.tipologia
        r.Valore = rub.valore
        Return r
    End Function
End Module