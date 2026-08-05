Public Class Vertice
#Region "Members"
    Private nName As Integer
    Private nDescrizione As String
    Private nDescrizione_Latina As String

    'Private nLivello As Integer
    Private nLivelloDiStopDiscesa As Integer = 1

    Public nRank As Integer
    Public vRoot As Vertice

    Public listaSuccessori As New Hashtable

#End Region

#Region "Properties"
    Public ReadOnly Property Name() As Integer
        Get
            Return nName
        End Get
    End Property
    Public ReadOnly Property LivelloDiStopDiscesa() As Integer
        Get
            Return nLivelloDiStopDiscesa
        End Get
    End Property

    'Public Property Livello() As Integer
    '    Get
    '        Return nLivello
    '    End Get
    '    Set(ByVal value As Integer)
    '        nLivello = value
    '    End Set
    'End Property

    Public ReadOnly Property Descrizione() As String
        Get
            Return nDescrizione
        End Get
    End Property

    Public ReadOnly Property Descrizione_Latina() As String
        Get
            Return nDescrizione_Latina
        End Get
    End Property
#End Region

    Public Sub New(Name As Integer, Descrizione As String, Descrizione_Latina As String, LivelloDiStopDiscesa As Integer)

        Me.nName = Name
        Me.nDescrizione = Descrizione
        Me.nDescrizione_Latina = Descrizione_Latina
        'Me.nLivello = Livello
        Me.nLivelloDiStopDiscesa = LivelloDiStopDiscesa

        nRank = 0
        Me.vRoot = Me

    End Sub

#Region "Methods"
    Friend Function GetRoot() As Vertice
        If Me.vRoot IsNot Me Then
            ' am I my own root ? (am i the root ?)
            ' No? then get my root
            Me.vRoot = Me.vRoot.GetRoot()
        End If
        Return Me.vRoot
    End Function

    Friend Shared Sub Join(vRoot1 As Vertice, vRoot2 As Vertice)
        If vRoot2.nRank < vRoot1.nRank Then
            'is the rank of Root2 less than that of Root1 ?
            'yes! then make Root1 the root of Root2 (since it has the higher rank)
            vRoot2.vRoot = vRoot1
        Else
            'rank of Root2 is greater than or equal to that of Root1
            vRoot1.vRoot = vRoot2
            'make Root2 the root of Root1
            If vRoot1.nRank = vRoot2.nRank Then
                'both ranks are equal ?
                'increment one of them, we need to reach a single root for the whole tree
                vRoot1.nRank += 1
            End If
        End If
    End Sub
#End Region
End Class
