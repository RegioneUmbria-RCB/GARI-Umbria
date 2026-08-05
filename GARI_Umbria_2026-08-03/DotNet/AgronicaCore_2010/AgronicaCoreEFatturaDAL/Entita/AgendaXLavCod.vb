Public Class GenerazioneFatture

    Public DaGenerare As List(Of AgendaXLavCod)
    Public Eliminate As List(Of AgendaXLavCod)
    Public Modificate As List(Of AgendaXLavCod)

End Class

Public Class ControlloPreliminare
    Public PresentiInSdiLogMaEliminate As List(Of AgendaXLavCod) = New List(Of AgendaXLavCod)
    Public Generate As List(Of AgendaXLavCod) = New List(Of AgendaXLavCod)
    Public InviatoSDI_KO As List(Of AgendaXLavCod) = New List(Of AgendaXLavCod)
End Class


Public Class AgendaXLavCod

    Public PIVA As String
    Public IdAgenda As Integer
    Public Lav_cod As String
    Public Des_lib As String
    Public Cod_RisUm As String
    Public Blocco_Flag As Integer
    Public StatoGias As StatoFattura_Gias?
    Public StatoSDI As StatoFattura_SDI?
    Public ValiditaInizio As DateTime
    Public DataMovimento As DateTime?
    Public Doc_numero As Double
    Public Doc_NUmero_Sin As String
    Public Doc_NUmero_Des As String
    Public CodiceRaggruppamento As String
    Public DataGenerazioneXML As DateTime
    Public DataModificaEliminazione As DateTime
    Public ID_LOg As Integer
    Public NomeFIleXml As String
    Public Anno As String

    Public Sub ImpostaCodiceRaggruppamento()

        Dim annoDocumento As String = "1900"

        If Not DataMovimento Is Nothing AndAlso DataMovimento.HasValue Then
            annoDocumento = DataMovimento.Value.Year.ToString
        End If
        Anno = annoDocumento

        CodiceRaggruppamento = String.Format("{0}{1}{2}", Doc_NUmero_Sin, Doc_NUmero_Des, Anno)

    End Sub

    Public Function NumeroDocumento() As String
        Return String.Format("{0}{1}{2}", Doc_NUmero_Sin, Doc_numero, Doc_NUmero_Des)
    End Function

    Public Overrides Function ToString() As String
        If Not DataMovimento Is Nothing AndAlso DataMovimento.HasValue AndAlso DataMovimento <> DateTime.MinValue Then
            Return String.Format("Id_Agenda {0} - Numero Documento {1} del {2} - Nome file XML {3}", Me.IdAgenda, NumeroDocumento(), Me.DataMovimento, Me.NomeFIleXml)
        Else
            Return String.Format("Id_Agenda {0} - Numero Documento {1} - Nome file XML {2}", Me.IdAgenda, NumeroDocumento(), Me.NomeFIleXml)
        End If

    End Function

End Class
