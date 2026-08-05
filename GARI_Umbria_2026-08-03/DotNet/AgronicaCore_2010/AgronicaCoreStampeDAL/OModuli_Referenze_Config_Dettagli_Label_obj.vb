Public Class OModuli_Referenze_Config_Dettagli_Label_obj

    Private _iFF_Etichette_tipo As Integer
    Private _NomeCampoAlias As String

    Public Property NomeCampoAlias As String
        Get
            Return _NomeCampoAlias
        End Get
        Set(ByVal value As String)
            _NomeCampoAlias = value
        End Set
    End Property



    Public Property IFF_Etichette_tipo As Integer
        Get
            Return _iFF_Etichette_tipo
        End Get
        Set(ByVal value As Integer)
            _iFF_Etichette_tipo = value
        End Set
    End Property


End Class
