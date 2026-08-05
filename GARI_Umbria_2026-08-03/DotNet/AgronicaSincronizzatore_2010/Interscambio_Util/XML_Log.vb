Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreUtility
Imports Interscambio_Util.Util

Public Class XML_Universal_Import_Log_Helper

    Public Sub New()
        Piva = ""
        NomeFileOrig = ""
        PathFileOrig = ""
        NomeFileWork = ""
        PathFileWork = ""
        IdAgenda = 0
        LavCod = 0
        TipoXml = ""
        Sezione = ""
        Errore = ""
        SospesoFlag = False
        SospesoTipo = enum_XmlLog_SospesoTipo.NESSUNO
        SospesoValore = ""
    End Sub

    Public Property Piva As String
    Public Property NomeFileOrig As String
    Public Property PathFileOrig As String
    Public Property NomeFileWork As String
    Public Property PathFileWork As String
    Public Property IdAgenda As Integer
    Public Property LavCod As Integer
    Public Property TipoXml As String
    Public Property Sezione As String
    Public Property Errore As String
    Public Property SospesoFlag As Boolean
    Public Property SospesoTipo As enum_XmlLog_SospesoTipo
    Public Property SospesoValore As String

End Class

Public Class XML_Universal_Log_Helper

    Public Sub New()
        Tipo = ""
        Stato = enum_WFlow_XML_Log_Universale.Nessuno
        NomeFile = ""
        DataOperazione = AGRODATAINIZIO
        Chiave = ""
        Piva = ""
        IdAgenda = 0
        SaCod = 0
        Appezza = 0
        IdReg = 0
        ProgettoCod = 0
        CodAnimale = 0
        CodAnimaleDistinta = 0
        ImputazioneCod = 0
        ElemCod = 0
        ProdottoCod = 0
        CodContatto = ""
        CodRisUm = 0
        CodiceSecondario = ""
        Note = ""
        TipoXml = enum_TipoXml.NON_SPECIFICATO
        Sezione = ""
        ChiaveSistemaEsterno = ""
        UltimaOperazioneDB = enum_TipoOperazioneDB.Scrittura
    End Sub

    Public Property Tipo As String
    Public Property Stato As enum_WFlow_XML_Log_Universale
    Public Property NomeFile As String
    Public Property DataOperazione As DateTime
    Public Property Chiave As String
    Public Property Piva As String
    Public Property IdAgenda As Integer
    Public Property SaCod As Integer
    Public Property Appezza As Integer
    Public Property IdReg As Integer
    Public Property ProgettoCod As Integer
    Public Property CodAnimale As Integer
    Public Property CodAnimaleDistinta As Integer
    Public Property ImputazioneCod As Integer
    Public Property ElemCod As Integer
    Public Property ProdottoCod As Integer
    Public Property CodContatto As String
    Public Property CodRisUm As Integer
    Public Property CodiceSecondario As String
    Public Property Note As String
    Public Property TipoXml As enum_TipoXml
    Public Property Sezione As String
    Public Property ChiaveSistemaEsterno As String
    Public Property UltimaOperazioneDB As enum_TipoOperazioneDB

    Public Function Scrivi(ByRef objParametriServer As AgronicaCoreParametri,
                           Optional ByRef idLogScritto As Integer = 0
                           ) As Boolean
        'ByRef idLogScritto As Integer

        Const nomeRoutine = "XML_Log.Scrivi"
        Dim xRisp As Boolean = False

        Try

            Dim gEfUtils As New AgronicaCoreEntityFramework.Gias_EF_Utility
            Dim efConnString As String = gEfUtils.GetEntityConnectionString(objParametriServer.StringaConnessione)

            Dim xmlLog As New AgronicaCoreEntityFramework_POCO.XML_Log With {
                .Tipo = Tipo,
                .Stato = CInt(Stato),
                .NomeFile = NomeFile,
                .Data_Operazione = DataOperazione,
                .Chiave = Chiave,
                .Piva = Piva,
                .Id_Agenda = IdAgenda,
                .Sa_Cod = SaCod,
                .Appezza = Appezza,
                .Id_Reg = IdReg,
                .Progetto_Cod = ProgettoCod,
                .Cod_Animale = CodAnimale,
                .Cod_Animale_Distinta = CodAnimaleDistinta,
                .Imputazione_Cod = ImputazioneCod,
                .Elem_Cod = ElemCod,
                .Prodotto_Cod = ProdottoCod,
                .Cod_Contatto = CodContatto,
                .Cod_RisUm = CodRisUm,
                .CodiceSecondario = CodiceSecondario,
                .Note = Stringhe.TroncaStringa(Note, 4000),
                .Data_Creazione = DateTime.Now,
                .Data_Modifica = DateTime.Now,
                .Username_Creazione = objParametriServer.UsernameOperazione,
                .Username_Modifica = objParametriServer.UsernameOperazione
            }

            Using dal As New AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities(efConnString)

                dal.XML_Log.Add(xmlLog)
                dal.SaveChanges()

                idLogScritto = xmlLog.Id_Log

            End Using

            xRisp = True

        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return xRisp

    End Function

    Public Sub ScriviConListLog(ByRef objParametriServer As AgronicaCoreParametri,
                                Optional ByRef listLogScritti As List(Of Integer) = Nothing)

        Dim idLogScritto As Integer = 0
        Dim flagLog As Boolean = Scrivi(objParametriServer, idLogScritto)
        If listLogScritti IsNot Nothing AndAlso idLogScritto <> 0 Then
            listLogScritti.Add(idLogScritto)
        End If
    End Sub

End Class