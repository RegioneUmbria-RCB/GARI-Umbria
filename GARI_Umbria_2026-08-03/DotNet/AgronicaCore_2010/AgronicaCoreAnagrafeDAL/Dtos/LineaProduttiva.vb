Imports AgronicaCoreEntityFramework_POCO

Public Class LineaProduttiva

    Public Property LineaPresente As Boolean

    Public Property Linee_Produzioni As Linee_Produzioni

    Public Property Linee_Produzioni_Mix As Linee_Produzioni_Mix

    Public Property RelazioneLineePreparazioni As List(Of RelazioneLineePreparazioni)

    Public Property RelazioneMateriePrime As List(Of RelazioneMateriePrime)

End Class

Public Class RelazioneLineePreparazioni
    Public Property Linee_Preparazioni As Linee_Preparazioni
    Public Property Linee_ProduzionixPreparazioni As Linee_ProduzionixPreparazioni
End Class

Public Class RelazioneMateriePrime
    Public Property OGenerazioni_Anagrafe_Log As OGenerazioni_Anagrafe_Log
    Public Property Materie_Prime As Materie_Prime
End Class
