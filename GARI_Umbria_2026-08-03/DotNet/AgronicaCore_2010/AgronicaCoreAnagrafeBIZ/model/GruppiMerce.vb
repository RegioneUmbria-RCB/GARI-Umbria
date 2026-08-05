Imports System.Runtime.CompilerServices
Imports AgronicaCoreEntityFramework_POCO

Public Class GruppoMerce
    Public Property Piva_SuperUser As String
    Public Property Piva As String
    Public Property Sa_Cod As Integer
    Public Property Id_Gruppo_Merce As Nullable(Of Integer)
    Public Property Codice As String
    Public Property Descrizione As String
    Public Property inviato As Nullable(Of Short)
    Public Property datainvio As Nullable(Of Date)
    Public Property Data_Creazione As Nullable(Of Date)
    Public Property Data_Modifica As Nullable(Of Date)
    Public Property Username_Creazione As String
    Public Property Username_Modifica As String
    Public Property Validita_Inizio As Nullable(Of Date)
    Public Property Validita_Fine As Nullable(Of Date)

End Class

Module DtoExtensions
    <Extension()>
    Public Function ToPoco(ByVal gruppoMerce As GruppoMerce) As Gruppi_Merce
        Dim poco As New Gruppi_Merce
        poco.Piva_SuperUser = gruppoMerce.Piva_SuperUser
        poco.Piva = gruppoMerce.Piva
        poco.Sa_Cod = gruppoMerce.Sa_Cod
        If gruppoMerce.Id_Gruppo_Merce Is Nothing Then
            poco.Id_Gruppo_Merce = 0
        Else
            poco.Id_Gruppo_Merce = gruppoMerce.Id_Gruppo_Merce
        End If
        poco.Codice = gruppoMerce.Codice
        poco.Descrizione = gruppoMerce.Descrizione
        poco.inviato = gruppoMerce.inviato
        poco.datainvio = gruppoMerce.datainvio
        poco.Data_Creazione = gruppoMerce.Data_Creazione
        poco.Data_Modifica = gruppoMerce.Data_Modifica
        poco.Username_Creazione = gruppoMerce.Username_Creazione
        poco.Username_Modifica = gruppoMerce.Username_Modifica
        poco.Validita_Inizio = gruppoMerce.Validita_Inizio
        poco.Validita_Fine = gruppoMerce.Validita_Fine
        Return poco
    End Function
End Module

Public Class CodiceDuplicatoEccezione
    Inherits Exception

    Public Sub New()
        MyBase.New(InsertionErrors.CodiceDuplicato)
    End Sub
End Class

Public Class GruppoUtilizzatoDaUnProdottoExtraEccezione
    Inherits Exception

    Public Sub New()
        MyBase.New(InsertionErrors.GruppoUtilizzatoDaUnProdottoExtra)
    End Sub
End Class

Public Class UtilizzatoDaUnAltraImpresaComeGruppoMerceDefaultEccezione
    Inherits Exception

    Public Sub New()
        MyBase.New(InsertionErrors.UtilizzatoDaUnAltraImpresaComeGruppoMerceDefault)
    End Sub
End Class

Public Module ErrorsGruppoMerce
    Public Enum InsertionErrors
        CodiceDuplicato = 1
        GruppoUtilizzatoDaUnProdottoExtra = 2
        UtilizzatoDaUnAltraImpresaComeGruppoMerceDefault = 3
    End Enum

    Public Enum Visibilita
        PUBBLICO = -1
        PRIVATO = 0
    End Enum
End Module


