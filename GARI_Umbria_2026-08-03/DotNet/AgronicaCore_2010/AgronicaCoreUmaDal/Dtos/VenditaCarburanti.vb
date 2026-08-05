Imports System.Runtime.CompilerServices
Imports AgronicaCoreDataProvider

Public Class RigheVenditaCarburantiDaAggiornare
    Public Property RigheInserite As String
    Public Property RigheModificate As String
    Public Property RigheCancellate As String
End Class
Public Class FiltriCaricaRigheVenditaCarburanti
    Public Property Anno As Integer
    Public Property Piva As String
    Public Property NuovaVisibilita As Boolean
    Public Sub New(_anno As Integer, _cuaa As String, _nuovaVisibilita As Boolean)
        Anno = _anno
        Piva = _cuaa
        NuovaVisibilita = _nuovaVisibilita
    End Sub
End Class
Public Class FiltriPraticaRichiestaCarburante
    Public Property Anno As Integer
    Public Property Cuaa As String
End Class
Public Class OttienePivaCliente
    Public Property Anno As Integer
    Public Property Cuaa As String
End Class
Public Class PivaClienteERagioneSocialeResult
    Public Property Piva_Cliente As String
    Public Property Ragione_Sociale As String
    Public Property MessaggioSuccess As String
End Class
Public Class FiltriAggiornaLtAssegnati
    Public Property Anno As Integer
    Public Property PivaCliente As String
    Public Property Tipo_Carburante As Integer
    Public Property ContoProprioTerzi As Integer
    Public Sub New()
    End Sub
    Public Sub New(anno_cod As Integer, piva As String, tipoCarb As Integer, contoPriprioTerzi As Integer)
        Anno = anno_cod
        PivaCliente = piva
        Tipo_Carburante = tipoCarb
        ContoProprioTerzi = contoPriprioTerzi
    End Sub
End Class

Public Enum ContoProprioTerzi
    ContoProprio = 0
    ContoTerzi = -1
End Enum

Public Class FiltriAggiornaTotaleLtAcquistati
    Public Property Anno As Integer
    Public Property Cuaa As String
    Public Property TipoCarburante As Integer
    Public Property ContoProprioContoTerzi As Integer
    Public Sub New()
    End Sub
    Public Sub New(_anno As Integer, _cuaa As String, _tipoCarburante As Integer, contoProprioTerzi As Integer)
        Anno = _anno
        Cuaa = _cuaa
        TipoCarburante = _tipoCarburante
        ContoProprioContoTerzi = contoProprioTerzi
    End Sub
End Class
Public Class FiltriDettaglioLtAcquistabili
    Public Property Anno As Integer
    Public Property Cuaa As String
    Public Property Tipo_Carburante As Integer
    Public Property PivaCliente As String
    Public Sub New()
    End Sub
End Class
Public Class DettaglioLtAcquistabiliRisultati
    Public Property Conto_Proprio As Double
    Public Property Conto_Terzi As Double
End Class

Public Class VenditaCarburantiDto
    Public Property Id_Vendite As Integer
    Public Property PivaSuperUser As String
    Public Property PIVA_Venditore As String
    Public Property PIVA_Cliente As String
    'Public Property Anno As Integer
    'Public Property Conto_Proprio_Terzi As Integer
    'Public Property Tipo_Carburante As Integer
    Public Property Lt As Double
    Public Property Tipo_Documento_Cod As Integer
    Public Property Tipo_Documento_Des As String
    Public Property Data_Documento As Date?
    Public Property Nr_Documento As String
    Public Property inviato As Nullable(Of Short)
    Public Property datainvio As Nullable(Of Date)
    Public Property Data_Creazione As Nullable(Of Date)
    Public Property Data_Modifica As Nullable(Of Date)
    Public Property Username_Creazione As String
    Public Property Username_Modifica As String
    Public Property Validita_Inizio As Nullable(Of Date)
    Public Property Validita_Fine As Nullable(Of Date)
    Public Property Note_Rivenditore As String
    Public Property LtInPrecedenza As Double

    Public Property Azienda_Venditore_Cod As String
    Public Property Azienda_Venditore_Des As String
    Public Property Anno_Cod As Integer
    Public Property Anno_Des As String
    Public Property Cuaa As String
    Public Property Ragione_Sociale As String
    Public Property Conto_Proprio_Terzi_Cod As Integer
    Public Property Conto_Proprio_Terzi_Des As String
    Public Property Tipo_Carburante_Cod As Integer
    Public Property Tipo_Carburante_Des As String
    Public Property Lt_Assegnati As Integer?
    Public Property Totale_Lt_Acquistati As Integer?
    Public Property Lt_Acquistabili As Integer?
    'Public Property Lt As Integer
    'Public Property Tipo_Documento As String
    'Public Property Data_Documento As Date
    'Public Property Nr_Documento As Integer


End Class

Public Class Anno_DDL
    Public Anno_Cod As Integer
    Public Anno_Des As String

    Public Sub New(cod As Integer, des As String)
        Anno_Cod = cod
        Anno_Des = des
    End Sub
End Class
Public Class Conto_Proprio_Terzi_DDL
    Public Conto_Proprio_Terzi_Cod As Integer
    Public Conto_Proprio_Terzi_Des As String

    Public Sub New(cod As Integer, des As String)
        Conto_Proprio_Terzi_Cod = cod
        Conto_Proprio_Terzi_Des = des
    End Sub
End Class
Public Class Tipo_Carburante_DDL
    Public Tipo_Carburante_Cod As Integer
    Public Tipo_Carburante_Des As String

    Public Sub New(cod As Integer, des As String)
        Tipo_Carburante_Cod = cod
        Tipo_Carburante_Des = des
    End Sub
End Class
Public Class Tipo_Documento_DDL
    Public Tipo_Documento_Cod As Integer
    Public Tipo_Documento_Des As String

    Public Sub New(cod As Integer, des As String)
        Tipo_Documento_Cod = cod
        Tipo_Documento_Des = des
    End Sub
End Class

Module Extensions
    <Extension()>
    Function ToVenditaCarburantePOCO(ByVal input As VenditaCarburantiDto, context As AgronicaCoreParametri, Vendite_Id As Integer) As AgronicaCoreEntityFramework_POCO.UMA_Vendite
        Dim output As New AgronicaCoreEntityFramework_POCO.UMA_Vendite
        'output.Id_Vendite = input.Id_Vendite
        output.Id_Vendite = Vendite_Id
        output.PivaSuperUser = context.PivaSuperUser
        output.PIVA_Venditore = input.Azienda_Venditore_Cod
        output.PIVA_Cliente = input.PIVA_Cliente
        output.Anno = input.Anno_Cod
        output.Conto_Proprio_Terzi = input.Conto_Proprio_Terzi_Cod
        output.Tipo_Carburante = input.Tipo_Carburante_Cod
        output.Lt = input.Lt
        output.Tipo_Documento = input.Tipo_Documento_Cod
        output.Data_Documento = input.Data_Documento
        output.Nr_Documento = input.Nr_Documento
        output.Note_Rivenditore = input.Note_Rivenditore

        Dim now = Date.Now
        output.inviato = 0
        output.datainvio = now
        output.Data_Creazione = now
        output.Data_Modifica = now
        output.Username_Creazione = context.UtenteUsername
        output.Username_Modifica = context.UtenteUsername
        output.Validita_Inizio = New Date(1900, 1, 1)
        output.Validita_Fine = New Date(2100, 12, 31)
        Return output
    End Function
End Module
