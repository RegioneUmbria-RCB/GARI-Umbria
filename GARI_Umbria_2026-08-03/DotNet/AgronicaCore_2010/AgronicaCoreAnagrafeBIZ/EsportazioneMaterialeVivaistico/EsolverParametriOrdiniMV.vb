Imports Newtonsoft.Json

Public Class EsolverParametriOrdiniMV
    Public Property BaseUrl As String
    Public Property AuthEndpoint As String
    Public Property ExportEndpoint As String
    Public Property StatoEndpoint As String
    Public Property PasswordHash As String
End Class

<JsonObject()>
Public Class EsolverModelOrdiniMV
    
    Public Property Codice As EsolverTipiParametriOrdiniMV

    Public Sub New(ByVal codiceDaAssegnare As EsolverTipiParametriOrdiniMV)
        Codice = codiceDaAssegnare
    End Sub

End Class

Public Class EsolverModelOrdiniMV_Numerico : Inherits EsolverModelOrdiniMV
    
    <JsonProperty(PropertyName:="ValoreNumerico")>
    Public Property Valore As Integer
    
    Public Sub New(ByVal codiceDaAssegnare As EsolverTipiParametriOrdiniMV, ByVal numero As Integer)
        MyBase.New(codiceDaAssegnare)
        Valore = numero
    End Sub

End Class

Public Class EsolverModelOrdiniMV_AlfaNumerico : Inherits EsolverModelOrdiniMV
    
    <JsonProperty(PropertyName:="ValoreAlfaNumerico")>
    Public Property Valore As String
    
    Public Sub New(ByVal codiceDaAssegnare As EsolverTipiParametriOrdiniMV, ByVal str As String)
        MyBase.New(codiceDaAssegnare)
        Valore = str
    End Sub

End Class

Public Class EsolverModelOrdiniMV_Data : Inherits EsolverModelOrdiniMV
    
    <JsonProperty(PropertyName:="ValoreData")>
    Public Property Valore As Date

    Public Sub New(ByVal codiceDaAssegnare As EsolverTipiParametriOrdiniMV, ByVal dataDaAssegnare As Date)
        MyBase.New(codiceDaAssegnare)
        Valore = dataDaAssegnare
    End Sub

End Class

Public Enum EsolverTipiParametriOrdiniMV
    TipoRiga = 1
    DataOrdineGias = 20
    NumeroOrdineGias = 30
    OrdineCancellato = 160
    CodiceArticolo = 260
    QuantitaSemi = 300
    DataConsegnaPrevista = 320
    QuantitaPiante = 521
    DataTrapiantoPrevista = 522
    PivaAziendaAgricola = 523
    PivaVivaio = 524
    Omaggio = 529
End Enum

Public Class EsolverRispostaExportOrdiniMV
    ''' <summary>
    ''' 1 = ok, 0 = errore
    ''' </summary>
    ''' <returns></returns>
    Public Property Esito As Integer
    Public Property Errori As List(Of String)
    Public Property GuidOperazione As String
End Class

Public Class EsolverRispostaGetStatoOrdiniMV
    Public Property Count As Integer
    Public Property Result As List(Of EsolverRispostaGetStatoOrdiniMV_Singolo)

End Class


Public Class EsolverRispostaGetStatoOrdiniMV_Singolo
    ''' <summary>
    ''' Id in formato: dd-MM-YYYY-x ove x è il numero intero del documento
    ''' </summary>
    ''' <returns></returns>
    Public Property RDADataNum As String
    ''' <summary>
    ''' 0 = ordine su esolver non ancora generato
    ''' 1 = [omissis]
    ''' </summary>
    ''' <returns></returns>
    Public Property RigaOrdinata As Integer
    ''' <summary>
    ''' Numero di semi consegnati (ovvero evasi)
    ''' </summary>
    ''' <returns></returns>
    Public Property QtaConsegnata As Double
    ''' <summary>
    ''' 0 = La quantità consegnata non è esaustiva della qta richiesta
    ''' 1 = Al contrario, oppure indica evasione forzata
    ''' </summary>
    ''' <returns></returns>
    Public Property RigaSaldata As Integer
    ''' <summary>
    ''' Partita Iva della ditta sementiera verso la quale è stato finalizzato l'ordine
    ''' </summary>
    ''' <returns></returns>
    Public Property PIVADittaSeme As String
    ''' <summary>
    ''' IGNORARE numero riga dell’output
    ''' </summary>
    ''' <returns></returns>
    Public Property RowNumberBc As Integer
    ''' <summary>
    ''' IGNORARE numero tot righe dell’output
    ''' </summary>
    ''' <returns></returns>
    Public Property BCRowsTot As Integer

End Class