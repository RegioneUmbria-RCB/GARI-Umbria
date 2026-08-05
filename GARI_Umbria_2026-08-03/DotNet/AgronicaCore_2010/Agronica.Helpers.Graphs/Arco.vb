Public Class Arco
    Implements IComparable

#Region "Members"
    Private m_v1 As Vertice, m_v2 As Vertice
    Private nCost As Integer

#End Region

#Region "Properties"
    Public ReadOnly Property V1() As Vertice
        Get
            Return m_v1
        End Get
    End Property
    Public ReadOnly Property V2() As Vertice
        Get
            Return m_v2
        End Get
    End Property
    Public ReadOnly Property Cost() As Integer
        Get
            Return nCost
        End Get
    End Property

#End Region

    Public Sub New(v1 As Vertice, v2 As Vertice, nCost As Integer)
        Me.m_v1 = v1
        Me.m_v2 = v2
        Me.nCost = nCost

    End Sub

#Region "IComparable Members"

    Public Function CompareTo(obj As Object) As Integer
        Dim e As Arco = DirectCast(obj, Arco)
        Return Me.nCost.CompareTo(e.nCost)
    End Function

#End Region

    Friend Shared Sub QuickSort(m_lstEdgesInitial As List(Of Arco), nLeft As Integer, nRight As Integer)
        Dim i As Integer, j As Integer, x As Integer
        i = nLeft
        j = nRight
        x = m_lstEdgesInitial((nLeft + nRight) \ 2).Cost

        Do
            While (m_lstEdgesInitial(i).Cost < x) AndAlso (i < nRight)
                i += 1
            End While
            While (x < m_lstEdgesInitial(j).Cost) AndAlso (j > nLeft)
                j -= 1
            End While

            If i <= j Then
                Dim y As Arco = m_lstEdgesInitial(i)
                m_lstEdgesInitial(i) = m_lstEdgesInitial(j)
                m_lstEdgesInitial(j) = y
                i += 1
                j -= 1
            End If
        Loop While i <= j

        If nLeft < j Then
            QuickSort(m_lstEdgesInitial, nLeft, j)
        End If
        If i < nRight Then
            QuickSort(m_lstEdgesInitial, i, nRight)
        End If
    End Sub

    Public Function CompareTo1(obj As Object) As Integer Implements System.IComparable.CompareTo

    End Function
End Class