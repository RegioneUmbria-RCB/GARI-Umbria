Imports AgronicaCoreAnagrafeStdDAL
Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports Microsoft.EntityFrameworkCore

Public Class Reg_Impianti
    Inherits BaseBiz

    Public Sub New(dbContext As GiasDbContext)
        MyBase.New(dbContext)
    End Sub

    Public Function EstraiListaCentriAziendali(Piva As String) As List(Of AgronicaCoreModelloSTD.Centri_Aziendali)

        Dim xLettura As New Reg_Impianti_R()
        Return xLettura.EstraiListaCentriAziendali(dbContext, Piva)

    End Function


    ''' <summary>
    ''' Legge gli impianti
    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <param name="cod_SpecieOppureDestinazioneUso">veg_cod oppure 0/id_cod</param>
    ''' <param name="DataRiferimento"></param>
    ''' <returns></returns>
    Public Function LeggiImpianti(Piva As String, ByVal Sa_Cod As Integer, ByVal cod_SpecieOppureDestinazioneUso As String, ByVal DataRiferimento As DateTime) As List(Of APP_Reg_Impianti)

        Dim xSplit As String() = cod_SpecieOppureDestinazioneUso.Split(CChar("/"))

        Dim Veg_Cod As Integer = 0
        Dim id_cod As Integer = 0

        If xSplit.Length = 1 Then
            Veg_Cod = CInt(xSplit(0))
        Else
            id_cod = CInt(xSplit(1))
        End If

        Dim xLettura = New Reg_Impianti_R()
        Return xLettura.Leggi(dbContext, Piva, Sa_Cod, Veg_Cod, id_cod, DataRiferimento)

    End Function

    Public Function LeggiImpianti(listaImpiantiFiltro As List(Of AgronicaCoreModelloSTD.Reg_Impianti)) As List(Of APP_Reg_Impianti)

        Dim xLettura = New Reg_Impianti_R()
        Return xLettura.Leggi(dbContext, listaImpiantiFiltro)

    End Function

    Public Async Function LeggiImpiantiAsync() As Task(Of IEnumerable(Of APP_Reg_Impianti))
        Try
            Dim Impianti = Await dbContext.APP_Reg_Impianti.ToListAsync()
            Return Impianti
        Catch e As Exception
            Return Nothing
        End Try
    End Function

    Public Function LeggiImpiantiCodiceAnagrafe(Codice As String) As APP_Reg_Impianti

        Dim xLettura = New Reg_Impianti_R()
        Return xLettura.LeggiDatoCodiceAnagrafe(dbContext, Codice)

    End Function

    Public Sub ScriviImpianti(listImpianti As List(Of APP_Reg_Impianti), piva As String, cancella As Boolean)

        Dim count As Integer = 0
        Dim commit As Boolean = False
        Dim numImpianti As Integer = listImpianti.Count
        Dim commitCount As Integer = 100
        Dim xScrittura = New Reg_Impianti_W()

        If cancella Then
            xScrittura.Cancella(dbContext, Nothing, piva)
        End If

        For Each impianto In listImpianti
            count += 1
            commit = count Mod commitCount = 0 OrElse count = numImpianti
            xScrittura.Scrivi(dbContext, impianto, commit)
        Next

    End Sub

    Public Sub CancellaImpianto(impianto As APP_Reg_Impianti)

        Dim xScrittura = New Reg_Impianti_W()
        xScrittura.Cancella(dbContext, impianto, Nothing)

    End Sub

    Public Sub CancellaImpianti(piva As String)

        Dim xScrittura = New Reg_Impianti_W()
        xScrittura.Cancella(dbContext, Nothing, piva)

    End Sub

End Class
