Imports System.Reflection
Imports Microsoft.VisualStudio.TestTools.UnitTesting.Web
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports System.Text


Public Class UnitTestHelper



    Public Shared Sub LookLikeEachOther(a As Object, b As Object)
        Dim typeA = a.[GetType]()
        Dim typeB = b.[GetType]()

'        Assert.AreEqual(typeA, typeB, "I tipi non sono gli stessi.")
'
'        Dim myProperties = typeA.GetProperties(BindingFlags.DeclaredOnly Or BindingFlags.[Public] Or BindingFlags.Instance)
'        Dim myPropertiesB = typeB.GetProperties(BindingFlags.DeclaredOnly Or BindingFlags.[Public] Or BindingFlags.Instance)
'        Assert.AreEqual(myProperties.Length, myPropertiesB.Length, "I due oggetti hanno un numero di proprietà differente")
'
'        For Each myPropertyA In myProperties
'            Dim myPropertyB = typeB.GetProperty(myPropertyA.Name)
'            Assert.IsNotNull(myPropertyB, "Proprietà " & myPropertyA.Name & " non trovata.")
'
'            'controllo il tipo
'            Assert.AreEqual(Of Type)(myPropertyA.PropertyType, myPropertyB.PropertyType, "Tipi differenti, proprietà: " & myPropertyB.Name)
'
'            'controllo i valori
'            If (myPropertyA.PropertyType <> GetType(String)) And (myPropertyA.PropertyType <> GetType(Integer)) And (myPropertyA.PropertyType <> GetType(Boolean)) And (myPropertyA.PropertyType <> GetType(Date)) And (myPropertyA.PropertyType <> GetType(DateTime)) Then
'
'                'controllo se è lista
'                If myPropertyA.PropertyType.Namespace = "System.Collections.Generic" Then
'                    Dim i As Integer
'                    For i = 0 To myPropertyA.GetValue(a, Nothing).count - 1
'                        LookLikeEachOther(myPropertyA.GetValue(a, Nothing)(i), myPropertyB.GetValue(b, Nothing)(i))
'                    Next
'
'                End If
'
'                'UnitTestHelper.LookLikeEachOther(myPropertyA.GetValue(a, Nothing), myPropertyB.GetValue(b, Nothing))
'            Else
'                Assert.AreEqual(myPropertyA.GetValue(a, Nothing), myPropertyB.GetValue(b, Nothing), "Valori differenti " & myPropertyA.Name & ":" & myPropertyA.GetValue(a, Nothing) & " " & myPropertyB.Name & ":" & myPropertyB.GetValue(b, Nothing))
'
'            End If
'        Next
    End Sub
    Public Shared Function confrontaStringhe(ByVal s1 As String, ByVal s2 As String) As String

        Dim i As Integer = 0
        For Each c As Char In s1

            If c <> s2(i) Then
                Return finalizza(s1, s2, i)
            End If

            i += 1

        Next

        Return ""

    End Function


    Private Shared Function finalizza(ByVal s1 As String, ByVal s2 As String, ByVal i As Integer) As String

        Dim lens As String = "Nessuna differenza in lunghezza"
        If s1.Length <> s2.Length Then
            lens = "s1.Length = " & s1.Length & ",  s2.Length = " & s2.Length & " --- "
        End If

        If i = s1.Length - 1 Or i = s2.Length - 1 Then
            Return lens & "le stringhe sono diverse nell'ultimo carattere: s1 = " & Right(s1, 1) & " - s2 = " & Right(s2, 1)
        End If

        Dim r As New StringBuilder
        r.AppendLine(s1.Substring(i, s1.Length - i))
        r.AppendLine(s2.Substring(i, s2.Length - i))
        Return r.ToString
    End Function



    Public Shared Sub stampaStringhe(ByVal inputString As String, ByVal actual As String, Optional ByVal VarName_inputString As String = "", Optional ByVal VarName_Actual As String = "")
        Debug.Print("Input (" & VarName_inputString & ") = ")
        Debug.Print("§" & inputString & "§")

        Debug.Print("")

        Debug.Print("Actual (" & VarName_Actual & ") = ")
        Debug.Print("§" & actual.Replace(vbCrLf, "") & "§")
    End Sub

End Class
