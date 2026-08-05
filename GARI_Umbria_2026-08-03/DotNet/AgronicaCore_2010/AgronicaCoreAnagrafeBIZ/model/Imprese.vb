Namespace AnagrafeNG

    Public Class Imprese
        Public Property Piva As String
        Public Property Rag_Soc As String
        Public Property Forma_Giuridica As Integer?
        Public Property Tipo_Impresa As Integer
        Public Property CUAA As String
        Public Property Indirizzo As Indirizzi
        Public Property Contatti As Impresa_Contatti()
        Public Property Padri As Gerarchia_Imprese_Padri()
        Public Property Tecnico_Referente As String
        Public Property Organismo_di_Controllo As Integer?
        Public Property Validita_Inizio As Date
        Public Property Validita_Fine As Date
        Public Property Codici As Codici()
    End Class

    Public Class Codici
        Public Property Descrizione As String
        Public Property Id_Cod As Integer
        Public Property Val_Cod As String
        Public Property Validita_Inizio As Date
        Public Property Validita_Fine As Date
    End Class

    Public Class Gerarchia_Imprese_Padri
        Public Property Piva As String
        Public Property Rag_Soc As String
    End Class

    Public Class Impresa_Contatti
        Public Property Cod_Rapporto As Integer
        Public Property Cod_Risum As Integer
        Public Property Settore_Des As String
        Public Property Attivita_Des As String
    End Class

End Namespace

