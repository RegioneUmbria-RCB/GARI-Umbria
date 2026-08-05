Imports System.Runtime.Serialization
Imports System.Security


<Serializable()>
Public Class RecoverableSPIDMultipleAccountLoginException
    Inherits System.Exception
    Public Risposta As UserLinkedAccountRisposta

    Public Sub New(risp As UserLinkedAccountRisposta)
        Risposta = risp
    End Sub
End Class

'Public Enum ErroreGias_Tipo
'    Generico = 0
'    LoginFallito = 1
'    ConflittoPermessi = 2
'    ParticellaConLegami = 3
'    NonGestito = 999
'End Enum

'Public Enum ErroreGias_Severity
'    Bloccante = 0 'Errore in basso a dx rosso, l'utente non può procedere (sia errori gestiti che exceptions)
'    Warning = 1 'Pop-up di n warning accodati con possibilità per l'utente di proseguire ("Si desidera proseguire?"   -   Annulla/Prosegui)
'    Info = 2
'    WarningBloccante = 3 'Pop-up di n errori accodati senza possibilità per l'utente di proseguire ("Non è possibile proseguire" - OK --> l'utente DEVE sistemare dei dati, NON può procedere altrimenti )
'End Enum

'Public Class ErroreGias
'    Public severity As ErroreGias_Severity
'    Public messaggio As String
'    Public ex As String
'    Public tipo As ErroreGias_Tipo
'End Class