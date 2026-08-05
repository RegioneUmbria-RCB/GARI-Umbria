Imports System.Xml
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports System.Text
Imports System.Data.EntityClient
Imports AgronicaCoreEntityFramework_POCO
Imports System.Data.Entity

Partial Public Class Funzioni

    Private _CentriAziendali As New List(Of G2G_Recode_Imprese)
    Private _CentriAziendaliEdit As New List(Of G2G_Recode_Imprese)
    Private _Campi As New List(Of G2G_Recode_Campo)
    Private _CampiEdit As New List(Of G2G_Recode_Campo)
    Private _AppezzamentiMappati As New List(Of G2G_Recode_Appezzamenti)
    Private _AppezzamentiMappatiEdit As New List(Of G2G_Recode_Appezzamenti)
    Private _ImpiantiMappati As New List(Of G2G_Recode_Impianti)
    Private _ImpiantiMappatiEdit As New List(Of G2G_Recode_Impianti)
    Private _Distinta As New List(Of G2G_Recode_Distinta)
    Private _DistintaEdit As New List(Of G2G_Recode_Distinta)
    Private _Fabbricati As New List(Of G2G_Recode_Fabbricati)
    Private _FabbricatiEdit As New List(Of G2G_Recode_Fabbricati)

    Private _Agenda As New List(Of G2G_Recode_Agenda)
    Private _AgendaDelete As New List(Of G2G_Recode_Agenda)
    Private _Raccoglitore As New List(Of G2G_Recode_Raccoglitore)
    Private _Movimenti As New List(Of G2G_Recode_Movimenti)
    Private _Mov_Dettagli As New List(Of G2G_Recode_Mov_Dettagli)

    'Private _Contatti As New List(Of G2G_Recode_Contatti)
    'Private _ContattiEdit As New List(Of G2G_Recode_Contatti)
    Private _AreeOmogenee As New List(Of recode_areeOmogenee)

    Private _funzioniGLOBAL As FunzioniGLOBAL
    Private _objP_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
    Private _efG2G As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities
    Private _nuovaLogicaRecode As Boolean
    Private _nuovaLogicaRecodePC As Boolean
    Private _nuovaLogicaRecodeAG As Boolean

    Private _timeout_ws_Importa As Integer = Integer.MaxValue '1800000

    Private Shared Sub FinestraTemporaleRecupera(objOpzioni As clsOpzioni, ByRef FinestraTemporaleInizioPrecedente As Date, ByRef FinestraTemporaleFinePrecedente As Date)
        FinestraTemporaleInizioPrecedente =
            objOpzioni.objParametri_Server_GIAS_ORIGINE.FinestraTemporaleInizio
        FinestraTemporaleFinePrecedente =
            objOpzioni.objParametri_Server_GIAS_ORIGINE.FinestraTemporaleFine
    End Sub

    Private Shared Sub FinestraTemporaleImpostaValori(objOpzioni As clsOpzioni, Inizio As Date, Fine As Date)
        objOpzioni.objParametri_Server_GIAS_ORIGINE.FinestraTemporaleInizio = Inizio
        objOpzioni.objParametri_Server_GIAS_ORIGINE.FinestraTemporaleFine = Fine
    End Sub

    Public Sub New(ByVal objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri, Timeout_ws_Importa As Integer)
        _objP_Server = objParametri_server
        _efG2G = Nothing
        _nuovaLogicaRecode = False
        _nuovaLogicaRecodePC = False
        _nuovaLogicaRecodeAG = False
        _timeout_ws_Importa = Timeout_ws_Importa

        If Timeout_ws_Importa <> 0 Then
            _timeout_ws_Importa = Timeout_ws_Importa
        End If

    End Sub

    Public Sub New(ByVal objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri, ByVal efG2G As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities, ByVal nuovaLogicaRecode As Boolean, ByVal nuovaLogicaRecodePC As Boolean, ByVal nuovaLogicaRecodeAG As Boolean, Timeout_ws_Importa As Integer)
        _objP_Server = objParametri_server
        _efG2G = efG2G
        _nuovaLogicaRecode = nuovaLogicaRecode
        _nuovaLogicaRecodePC = nuovaLogicaRecodePC
        _nuovaLogicaRecodeAG = nuovaLogicaRecodeAG

        If Timeout_ws_Importa <> 0 Then
            _timeout_ws_Importa = Timeout_ws_Importa
        End If

    End Sub

    Public Sub AgendaADD(ByVal i As G2G_Recode_Agenda)
        If Not _Agenda.Contains(i) Then
            i.Username_Creazione = "1"
            i.Username_Modifica = _objP_Server.UtenteCodFiscale
            i.Data_Creazione = Now()
            i.Data_Modifica = Now()
            i.Validita_Inizio = AGRODATAINIZIO
            i.Validita_Fine = AGRODATAFINE
            i.inviato = "0"
            i.datainvio = Now()
            _Agenda.Add(i)
        End If
    End Sub

    Public Sub AgendaDelete(ByVal i As G2G_Recode_Agenda)
        If Not _AgendaDelete.Contains(i) Then
            i.Username_Creazione = "1"
            i.Data_Creazione = Now()
            i.inviato = "0"
            i.datainvio = Now()
            _AgendaDelete.Add(i)
        End If
    End Sub

    Public Sub AppezzamentiMappatiADD(ByVal i As G2G_Recode_Appezzamenti)
        If Not _AppezzamentiMappati.Contains(i) Then
            i.Username_Creazione = "1"
            i.Username_Modifica = _objP_Server.UtenteCodFiscale
            i.Data_Creazione = Now()
            i.Data_Modifica = Now()
            i.Validita_Inizio = AGRODATAINIZIO
            i.Validita_Fine = AGRODATAFINE
            i.inviato = "0"
            i.datainvio = Now()
            _AppezzamentiMappati.Add(i)
        End If
    End Sub

    Public Sub AreeOmogeneeADD(ByVal i As recode_areeOmogenee)
        If Not _AreeOmogenee.Contains(i) Then
            'i.Username_Creazione = "1"
            'i.Data_Creazione = Now()
            _AreeOmogenee.Add(i)
        End If
    End Sub

    Public Sub CentriAziendaliADD(ByVal i As G2G_Recode_Imprese)
        If Not _CentriAziendali.Contains(i) Then
            i.Username_Creazione = "1"
            i.Username_Modifica = _objP_Server.UtenteCodFiscale
            i.Data_Creazione = Now()
            i.Data_Modifica = Now()
            i.Validita_Inizio = AGRODATAINIZIO
            i.Validita_Fine = AGRODATAFINE
            i.inviato = "0"
            i.datainvio = Now()
            _CentriAziendali.Add(i)
        End If
    End Sub

    Public Sub CampiADD(ByVal i As G2G_Recode_Campo)
        If Not _Campi.Contains(i) Then
            i.Username_Creazione = "1"
            i.Username_Modifica = _objP_Server.UtenteCodFiscale
            i.Data_Creazione = Now()
            i.Data_Modifica = Now()
            i.Validita_Inizio = AGRODATAINIZIO
            i.Validita_Fine = AGRODATAFINE
            i.inviato = "0"
            i.datainvio = Now()
            _Campi.Add(i)
        End If
    End Sub

    Public Sub ImpiantiMappatiADD(ByVal i As G2G_Recode_Impianti)
        If Not _ImpiantiMappati.Contains(i) Then
            i.Username_Creazione = "1"
            i.Username_Modifica = _objP_Server.UtenteCodFiscale
            i.Data_Creazione = Now()
            i.Data_Modifica = Now()
            i.Validita_Inizio = AGRODATAINIZIO
            i.Validita_Fine = AGRODATAFINE
            i.inviato = "0"
            i.datainvio = Now()
            _ImpiantiMappati.Add(i)
        End If
    End Sub

    Public Sub FabbricatiADD(ByVal i As G2G_Recode_Fabbricati)
        If Not _Fabbricati.Contains(i) Then
            i.Username_Creazione = "1"
            i.Username_Modifica = _objP_Server.UtenteCodFiscale
            i.Data_Creazione = Now()
            i.Data_Modifica = Now()
            i.Validita_Inizio = AGRODATAINIZIO
            i.Validita_Fine = AGRODATAFINE
            i.inviato = "0"
            i.datainvio = Now()
            _Fabbricati.Add(i)
        End If
    End Sub

    'Public Sub ContattiADD(ByVal i As G2G_Recode_Contatti)
    '    If Not _Contatti.Contains(i) Then
    '        i.Username_Creazione = "1"
    '        i.Username_Modifica = _objP_Server.UtenteCodFiscale
    '        i.Data_Creazione = Now()
    '        i.Data_Modifica = Now()
    '        i.Validita_Inizio = AGRODATAINIZIO
    '        i.Validita_Fine = AGRODATAFINE
    '        i.inviato = "0"
    '        i.datainvio = Now()
    '        _Contatti.Add(i)
    '    End If
    'End Sub

    Public Sub DistintaADD(ByVal i As G2G_Recode_Distinta)
        If Not _Distinta.Contains(i) Then
            i.Username_Creazione = "1"
            i.Username_Modifica = _objP_Server.UtenteCodFiscale
            i.Data_Creazione = Now()
            i.Data_Modifica = Now()
            i.Validita_Inizio = AGRODATAINIZIO
            i.Validita_Fine = AGRODATAFINE
            i.inviato = "0"
            i.datainvio = Now()
            _Distinta.Add(i)
        End If
    End Sub

    Public Sub MovimentiADD(ByVal i As G2G_Recode_Movimenti)
        If Not _Movimenti.Contains(i) Then
            i.Username_Creazione = "1"
            i.Username_Modifica = _objP_Server.UtenteCodFiscale
            i.Data_Creazione = Now()
            i.Data_Modifica = Now()
            i.Validita_Inizio = AGRODATAINIZIO
            i.Validita_Fine = AGRODATAFINE
            i.inviato = "0"
            i.datainvio = Now()
            _Movimenti.Add(i)
        End If
    End Sub

    Public Sub Mov_DettagliADD(ByVal i As G2G_Recode_Mov_Dettagli)
        If Not _Mov_Dettagli.Contains(i) Then
            i.Username_Creazione = "1"
            i.Username_Modifica = _objP_Server.UtenteCodFiscale
            i.Data_Creazione = Now()
            i.Data_Modifica = Now()
            i.Validita_Inizio = AGRODATAINIZIO
            i.Validita_Fine = AGRODATAFINE
            i.inviato = "0"
            i.datainvio = Now()
            _Mov_Dettagli.Add(i)
        End If
    End Sub

    Public Sub RaccoglitoreADD(ByVal i As G2G_Recode_Raccoglitore)

        'Controllo se non esista già tra quelli da inserire o quelli su db
        If Not _Raccoglitore.Contains(i) AndAlso IsNothing(
            (From x In _Raccoglitore Where x.From_PivaSuperUser = i.From_PivaSuperUser AndAlso x.To_PivaSuperUser = i.To_PivaSuperUser AndAlso x.From_Piva = i.From_Piva AndAlso x.From_Raccoglitore_Cod = i.From_Raccoglitore_Cod AndAlso
                 x.To_PivaSuperUser = i.To_PivaSuperUser AndAlso x.To_Piva = i.To_Piva AndAlso x.To_Raccoglitore_Cod = i.To_Raccoglitore_Cod).FirstOrDefault()) AndAlso IsNothing(
            (From x In _efG2G.G2G_Recode_Raccoglitore Where x.From_PivaSuperUser = i.From_PivaSuperUser AndAlso x.To_PivaSuperUser = i.To_PivaSuperUser AndAlso x.From_Piva = i.From_Piva AndAlso x.From_Raccoglitore_Cod = i.From_Raccoglitore_Cod AndAlso
                 x.To_PivaSuperUser = i.To_PivaSuperUser AndAlso x.To_Piva = i.To_Piva AndAlso x.To_Raccoglitore_Cod = i.To_Raccoglitore_Cod).FirstOrDefault()) Then

            i.Username_Creazione = "1"
            i.Username_Modifica = _objP_Server.UtenteCodFiscale
            i.Data_Creazione = Now()
            i.Data_Modifica = Now()
            i.Validita_Inizio = AGRODATAINIZIO
            i.Validita_Fine = AGRODATAFINE
            i.inviato = "0"
            i.datainvio = Now()
            _Raccoglitore.Add(i)
        End If
    End Sub

    Public Property FunzioniGLOBAL() As FunzioniGLOBAL
        Get
            Return _funzioniGLOBAL
        End Get
        Set(value As FunzioniGLOBAL)
            _funzioniGLOBAL = value
        End Set
    End Property

    Public ReadOnly Property NuovaLogicaRecode() As Boolean
        Get
            Return _nuovaLogicaRecode
        End Get
    End Property

    Public ReadOnly Property NuovaLogicaRecodePC() As Boolean
        Get
            Return _nuovaLogicaRecodePC
        End Get
    End Property

    Public ReadOnly Property NuovaLogicaRecodeAG() As Boolean
        Get
            Return _nuovaLogicaRecodeAG
        End Get
    End Property

    Public Sub Recode_Salvataggio_Agenda(efG2G As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities, ByRef Log_G2G As StringBuilder, ByRef Log_Errori As StringBuilder, ByRef Log_Riepilogo As StringBuilder, ByRef ErrFLAG As Integer)

        Dim NomeRoutine As String = "Funzioni.Recode_Salvataggio_Agenda"

        Try
            For Each elemEdit In (From i In _AgendaDelete Where i.Username_Creazione = "1")

                'elimino l'operazione
                Dim xUpdate As G2G_Recode_Agenda =
                    (From g In efG2G.G2G_Recode_Agenda
                     Where g.FromId_Agenda = elemEdit.FromId_Agenda AndAlso
                           g.ToId_Agenda = elemEdit.ToId_Agenda
                         ).FirstOrDefault

                If xUpdate IsNot Nothing Then
                    efG2G.G2G_Recode_Agenda.Remove(xUpdate)


                    Dim xMovimenti As List(Of G2G_Recode_Movimenti) = (
                        From m In efG2G.G2G_Recode_Movimenti
                        Where m.FromId_Agenda = elemEdit.FromId_Agenda AndAlso
                              m.ToId_Agenda = elemEdit.ToId_Agenda
                             ).ToList()

                    For Each xmov In xMovimenti
                        efG2G.G2G_Recode_Movimenti.Remove(xmov)
                    Next

                    Dim xMovimentiDettagli As List(Of G2G_Recode_Mov_Dettagli) = (
                        From m In efG2G.G2G_Recode_Mov_Dettagli
                        Where m.FromId_Agenda = elemEdit.FromId_Agenda AndAlso
                              m.ToId_Agenda = elemEdit.ToId_Agenda
                             ).ToList()

                    For Each xmovDettagli As G2G_Recode_Mov_Dettagli In xMovimentiDettagli
                        efG2G.G2G_Recode_Mov_Dettagli.Remove(xmovDettagli)
                    Next

                    Dim xMovimentiDettaglioTecnico As List(Of G2G_Recode_Mov_DettaglioTecnico) = (
                        From m In efG2G.G2G_Recode_Mov_DettaglioTecnico
                        Where m.FromId_Agenda = elemEdit.FromId_Agenda AndAlso
                              m.ToId_Agenda = elemEdit.ToId_Agenda
                             ).ToList()

                    For Each xmovDettagliotecnico As G2G_Recode_Mov_DettaglioTecnico In xMovimentiDettaglioTecnico
                        efG2G.G2G_Recode_Mov_DettaglioTecnico.Remove(xmovDettagliotecnico)
                    Next

                End If
            Next

            efG2G.SaveChanges()

            For Each elemNew In (From i In _Agenda Where i.Username_Creazione = "1")
                elemNew.Username_Creazione = _objP_Server.UtenteCodFiscale
                efG2G.G2G_Recode_Agenda.Add(elemNew)
            Next
            efG2G.SaveChanges()

            For Each elemNew In (From i In _Movimenti Where i.Username_Creazione = "1")
                elemNew.Username_Creazione = _objP_Server.UtenteCodFiscale
                efG2G.G2G_Recode_Movimenti.Add(elemNew)
            Next
            efG2G.SaveChanges()

            For Each elemNew In (From i In _Mov_Dettagli Where i.Username_Creazione = "1")
                elemNew.Username_Creazione = _objP_Server.UtenteCodFiscale
                efG2G.G2G_Recode_Mov_Dettagli.Add(elemNew)
            Next
            efG2G.SaveChanges()

            For Each elemNew In (From i In _Raccoglitore Where i.Username_Creazione = "1")
                elemNew.Username_Creazione = _objP_Server.UtenteCodFiscale
                efG2G.G2G_Recode_Raccoglitore.Add(elemNew)
            Next
            efG2G.SaveChanges()

            ErrFLAG = 0

        Catch ex As Exception
            Log_G2G.Append(" [" & NomeRoutine & "] : " & ex.Message.ToString & vbCrLf)
            Log_Errori.Append(" [" & NomeRoutine & "] : " & ex.Message.ToString & vbCrLf)
            ErrFLAG = 1
        End Try

    End Sub

    Public Sub Recode_Salvataggio(efG2G As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities, ByRef Log_G2G As StringBuilder, ByRef Log_Errori As StringBuilder, ByRef Log_Riepilogo As StringBuilder, ByRef ErrFLAG As Integer)

        Dim NomeRoutine As String = "Funzioni.Recode_Salvataggio"

        Try

            'modifiche ..
            For Each elemEdit In (From i In _CentriAziendaliEdit)
                efG2G.G2G_Recode_Imprese.Attach(elemEdit)
                efG2G.Entry(elemEdit).State = EntityState.Modified
            Next
            efG2G.SaveChanges()

            For Each elemEdit In (From i In _CampiEdit)
                efG2G.G2G_Recode_Campo.Attach(elemEdit)
                efG2G.Entry(elemEdit).State = EntityState.Modified
            Next
            efG2G.SaveChanges()

            For Each elemEdit In (From i In _AppezzamentiMappatiEdit)
                efG2G.G2G_Recode_Appezzamenti.Attach(elemEdit)
                efG2G.Entry(elemEdit).State = EntityState.Modified
            Next
            efG2G.SaveChanges()

            For Each elemEdit In (From i In _ImpiantiMappatiEdit)
                efG2G.G2G_Recode_Impianti.Attach(elemEdit)
                efG2G.Entry(elemEdit).State = EntityState.Modified
            Next
            efG2G.SaveChanges()

            For Each elemEdit In (From i In _DistintaEdit)
                efG2G.G2G_Recode_Distinta.Attach(elemEdit)
                efG2G.Entry(elemEdit).State = EntityState.Modified
            Next
            efG2G.SaveChanges()

            For Each elemEdit In (From i In _FabbricatiEdit)
                efG2G.G2G_Recode_Fabbricati.Attach(elemEdit)
                efG2G.Entry(elemEdit).State = EntityState.Modified
            Next
            efG2G.SaveChanges()

            'For Each elemEdit In (From i In _ContattiEdit)
            '    efG2G.G2G_Recode_Contatti.Attach(elemEdit)
            '    efG2G.Entry(elemEdit).State = EntityState.Modified
            'Next
            'efG2G.SaveChanges()

            'aggiunte...
            For Each elemNew In (From i In _CentriAziendali Where i.Username_Creazione = "1")
                elemNew.Username_Creazione = _objP_Server.UtenteCodFiscale
                efG2G.G2G_Recode_Imprese.Add(elemNew)
            Next
            efG2G.SaveChanges()

            For Each elemNew In (From i In _Campi Where i.Username_Creazione = "1")
                elemNew.Username_Creazione = _objP_Server.UtenteCodFiscale
                efG2G.G2G_Recode_Campo.Add(elemNew)
            Next
            efG2G.SaveChanges()

            For Each elemNew In (From i In _AppezzamentiMappati Where i.Username_Creazione = "1")
                elemNew.Username_Creazione = _objP_Server.UtenteCodFiscale
                efG2G.G2G_Recode_Appezzamenti.Add(elemNew)
            Next
            efG2G.SaveChanges()

            For Each elemNew In (From i In _ImpiantiMappati Where i.Username_Creazione = "1")
                elemNew.Username_Creazione = _objP_Server.UtenteCodFiscale
                efG2G.G2G_Recode_Impianti.Add(elemNew)
            Next
            efG2G.SaveChanges()

            For Each elemNew In (From i In _Distinta Where i.Username_Creazione = "1")
                elemNew.Username_Creazione = _objP_Server.UtenteCodFiscale
                efG2G.G2G_Recode_Distinta.Add(elemNew)
            Next
            efG2G.SaveChanges()

            For Each elemNew In (From i In _Fabbricati Where i.Username_Creazione = "1")
                elemNew.Username_Creazione = _objP_Server.UtenteCodFiscale
                efG2G.G2G_Recode_Fabbricati.Add(elemNew)
            Next
            efG2G.SaveChanges()

            'For Each elemNew In (From i In _Contatti Where i.Username_Creazione = "1")
            '    elemNew.Username_Creazione = _objP_Server.UtenteCodFiscale
            '    efG2G.G2G_Recode_Contatti.Add(elemNew)
            'Next
            'efG2G.SaveChanges()

            ErrFLAG = 0

        Catch ex As Exception
            Log_G2G.Append(" [" & NomeRoutine & "] : " & ex.Message.ToString & vbCrLf)
            Log_Errori.Append(" [" & NomeRoutine & "] : " & ex.Message.ToString & vbCrLf)
            ErrFLAG = 1
        End Try

    End Sub

    Public Sub CentriAziendaliEDIT(ByVal i As G2G_Recode_Imprese)
        If Not _CentriAziendaliEdit.Contains(i) Then
            i.Username_Modifica = _objP_Server.UtenteCodFiscale
            i.Data_Modifica = Now()
            i.datainvio = Now()
            _CentriAziendaliEdit.Add(i)
        End If
    End Sub

    Public Sub CampiEDIT(ByVal i As G2G_Recode_Campo)
        If Not _CampiEdit.Contains(i) Then
            i.Username_Modifica = _objP_Server.UtenteCodFiscale
            i.Data_Modifica = Now()
            i.datainvio = Now()
            _CampiEdit.Add(i)
        End If
    End Sub

    Public Sub AppezzamentiMappatiEDIT(ByVal i As G2G_Recode_Appezzamenti)
        If Not _AppezzamentiMappatiEdit.Contains(i) Then
            i.Username_Modifica = _objP_Server.UtenteCodFiscale
            i.Data_Modifica = Now()
            i.datainvio = Now()
            _AppezzamentiMappatiEdit.Add(i)
        End If
    End Sub

    Public Sub ImpiantiMappatiEDIT(ByVal i As G2G_Recode_Impianti)
        If Not _ImpiantiMappatiEdit.Contains(i) Then
            i.Username_Modifica = _objP_Server.UtenteCodFiscale
            i.Data_Modifica = Now()
            i.datainvio = Now()
            _ImpiantiMappatiEdit.Add(i)
        End If
    End Sub

    Public Sub DistintaEDIT(ByVal i As G2G_Recode_Distinta)
        If Not _DistintaEdit.Contains(i) Then
            i.Username_Modifica = _objP_Server.UtenteCodFiscale
            i.Data_Modifica = Now()
            i.datainvio = Now()
            _DistintaEdit.Add(i)
        End If
    End Sub

    Public Sub FabbricatiEDIT(ByVal i As G2G_Recode_Fabbricati)
        If Not _FabbricatiEdit.Contains(i) Then
            i.Username_Modifica = _objP_Server.UtenteCodFiscale
            i.Data_Modifica = Now()
            i.datainvio = Now()
            _FabbricatiEdit.Add(i)
        End If
    End Sub

    'Public Sub ContattiEDIT(ByVal i As G2G_Recode_Contatti)
    '    If Not _ContattiEdit.Contains(i) Then
    '        i.Username_Modifica = _objP_Server.UtenteCodFiscale
    '        i.Data_Modifica = Now()
    '        i.datainvio = Now()
    '        _ContattiEdit.Add(i)
    '    End If
    'End Sub

    Public Function Recode_ImpreseCentri(ByVal objOpzioni As clsOpzioni, ByVal Piva As String, ByVal Sa_Cod As Integer) As G2G_Recode_Imprese
        If NuovaLogicaRecode Then
            Return (From x In _efG2G.G2G_Recode_Imprese
                    Where x.FROM_Piva = Piva AndAlso x.FROM_SaCod = Sa_Cod AndAlso
                          x.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser AndAlso
                          x.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser Select x).FirstOrDefault
        Else
            Return (From x In _CentriAziendali
                    Where x.FROM_Piva = Piva AndAlso x.FROM_SaCod = Sa_Cod AndAlso
                          x.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser AndAlso
                          x.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser Select x).FirstOrDefault
        End If
    End Function

    Public Function Recode_ImpreseCentriReverse(ByVal objOpzioni As clsOpzioni, ByVal Piva As String, ByVal Sa_Cod As Integer) As G2G_Recode_Imprese
        If NuovaLogicaRecode Then
            Return (From x In _efG2G.G2G_Recode_Imprese
                    Where x.FROM_Piva = Piva AndAlso x.TO_SaCod = Sa_Cod AndAlso
                          x.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser AndAlso
                          x.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser Select x).FirstOrDefault
        Else
            Throw New Exception("Vecchia gestione Recode non supportata")
        End If
    End Function

    Public Function Recode_Fabbricati(ByVal objOpzioni As clsOpzioni, ByVal Piva As String, ByVal Sa_Cod As Integer, ByVal Fabbricato_Cod As Integer) As G2G_Recode_Fabbricati
        If NuovaLogicaRecode Then
            Return (From x In _efG2G.G2G_Recode_Fabbricati.ToList
                    Where x.FromPiva = Piva AndAlso x.FromSa_cod = Sa_Cod AndAlso x.From_FabbricatoCod = Fabbricato_Cod AndAlso
                          x.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser AndAlso
                          x.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser Select x).FirstOrDefault
        Else
            Return (From x In _Fabbricati
                    Where x.FromPiva = Piva AndAlso x.FromSa_cod = Sa_Cod AndAlso x.From_FabbricatoCod = Fabbricato_Cod AndAlso
                          x.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser AndAlso
                          x.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser Select x).FirstOrDefault
        End If
    End Function

    Public Function Recode_Campi(ByVal objOpzioni As clsOpzioni, ByVal Piva As String, ByVal Sa_Cod As Integer, ByVal Campo_Cod As Integer) As G2G_Recode_Campo
        If NuovaLogicaRecodePC Then
            Return (From x In _efG2G.G2G_Recode_Campo
                    Where x.From_Piva = Piva AndAlso x.From_Sa_Cod = Sa_Cod AndAlso x.From_Campo_cod = Campo_Cod AndAlso
                          x.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser AndAlso
                          x.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser Select x).FirstOrDefault
        Else
            Return (From x In _Campi
                    Where x.From_Piva = Piva AndAlso x.From_Sa_Cod = Sa_Cod AndAlso x.From_Campo_cod = Campo_Cod AndAlso
                          x.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser AndAlso
                          x.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser Select x).FirstOrDefault
        End If

    End Function

    Public Function Recode_Appezzamenti(ByVal objOpzioni As clsOpzioni, ByVal Piva As String, ByVal Sa_Cod As Integer, ByVal Appezza As Integer) As G2G_Recode_Appezzamenti
        If NuovaLogicaRecodePC Then
            Return (From x In _efG2G.G2G_Recode_Appezzamenti
                    Where x.From_Piva = Piva AndAlso x.From_Sa_Cod = Sa_Cod AndAlso x.From_Appezza = Appezza AndAlso
                          x.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser AndAlso
                          x.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser Select x).FirstOrDefault
        Else
            Return (From x In _AppezzamentiMappati
                    Where x.From_Piva = Piva AndAlso x.From_Sa_Cod = Sa_Cod AndAlso x.From_Appezza = Appezza AndAlso
                          x.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser AndAlso
                          x.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser Select x).FirstOrDefault
        End If

    End Function

    Public Function Recode_Impianti(ByVal objOpzioni As clsOpzioni, ByVal Piva As String, ByVal Sa_Cod As Integer, ByVal Appezza As Integer, ByVal Id_Reg As Integer) As G2G_Recode_Impianti
        If NuovaLogicaRecodePC Then
            Return (From x In _efG2G.G2G_Recode_Impianti
                    Where x.From_Piva = Piva AndAlso x.From_Sa_Cod = Sa_Cod AndAlso x.From_Appezza = Appezza AndAlso x.From_Id_Reg = Id_Reg AndAlso
                          x.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser AndAlso
                          x.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser Select x).FirstOrDefault
        Else
            Return (From x In _ImpiantiMappati
                    Where x.From_Piva = Piva AndAlso x.From_Sa_Cod = Sa_Cod AndAlso x.From_Appezza = Appezza AndAlso x.From_Id_Reg = Id_Reg AndAlso
                          x.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser AndAlso
                          x.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser Select x).FirstOrDefault
        End If

    End Function

    Public Function Recode_Distinte(ByVal objOpzioni As clsOpzioni, ByVal Piva As String, ByVal Progetto_Cod As Integer) As G2G_Recode_Distinta
        If NuovaLogicaRecodePC Then
            Return (From x In _efG2G.G2G_Recode_Distinta
                    Where x.From_Piva = Piva AndAlso x.From_Progetto_cod = Progetto_Cod AndAlso
                          x.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser AndAlso
                          x.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser Select x).FirstOrDefault
        Else
            Return (From x In _Distinta
                    Where x.From_Piva = Piva AndAlso x.From_Progetto_cod = Progetto_Cod AndAlso
                          x.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser AndAlso
                          x.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser Select x).FirstOrDefault
        End If

    End Function

    Public Function Recode_Caricamento(ByVal efG2G As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities) As Boolean

        If Not NuovaLogicaRecode Then
            _CentriAziendali = (From ii In efG2G.G2G_Recode_Imprese).ToList
            _Fabbricati = (From ii In efG2G.G2G_Recode_Fabbricati).ToList
        End If

        If Not NuovaLogicaRecodePC Then
            _Campi = (From ii In efG2G.G2G_Recode_Campo).ToList
            _AppezzamentiMappati = (From ii In efG2G.G2G_Recode_Appezzamenti).ToList
            _ImpiantiMappati = (From ii In efG2G.G2G_Recode_Impianti).ToList
            _Distinta = (From ii In efG2G.G2G_Recode_Distinta).ToList
        End If

        If Not NuovaLogicaRecodeAG Then
            _Agenda = (From ii In efG2G.G2G_Recode_Agenda).ToList
            _Movimenti = (From ii In efG2G.G2G_Recode_Movimenti).ToList
            _Mov_Dettagli = (From ii In efG2G.G2G_Recode_Mov_Dettagli).ToList
        End If

        '_Contatti = (From ii In efG2G.G2G_Recode_Contatti).ToList

        Return True

    End Function



#Region "Area Omogenea"
    Public Sub Elabora_XML_AreaOmogenea_Salva( _
                                ByVal objOpzioni As clsOpzioni, _
                                ByRef Log_Import As StringBuilder, _
                                ByRef Log_Errori As StringBuilder, _
                                ByRef Log_Riepilogo As StringBuilder, _
                                ByVal Piva_Origine As String, _
                                ByVal Piva_Destinazione As String _
                        )

        Const nomeFunzione As String = "Elabora_XML_AreaOmogenea_Salva"


        Try

            Dim leggiAreaOmogenee As New AgronicaCoreAnagrafeDAL.Area_Omogenea_R
            Dim ScriviAreaOmogenea As New AgronicaCoreAnagrafeDAL.Area_Omogenea_W

            Dim dtleggiAreaOmogenee As DataTable = _
                leggiAreaOmogenee.Leggi( _
                    Piva_Origine, _
                    0, _
                    "", _
                    "", _
                    objOpzioni.objParametri_Server_GIAS_ORIGINE _
                )


            Dim area_cod As Integer
            Dim ObjSequenze As New AgronicaCoreDataProvider.Agro_Sequenze
            Dim dtAreaTmp As DataTable


            If dtleggiAreaOmogenee IsNot Nothing Then
                For Each rowleggiAreaOmogenee In dtleggiAreaOmogenee.Rows

                    'area_cod = ObjSequenze.NuovoId_Tabella( _
                    '    "AREEOMOGENEE", _
                    '    objOpzioni.BaseCode_DESTINAZIONE, _
                    '    objOpzioni.TopCode_DESTINAZIONE,
                    '    objOpzioni.objParametri_Server_GIAS_DESTINAZIONE _
                    ')


                    ScriviAreaOmogenea.Scrivi( _
                        Piva_Destinazione, _
                        area_cod, _
                        CType(rowleggiAreaOmogenee("Area_Des"), String), _
                        CType(rowleggiAreaOmogenee("Tessitura_Cod"), Integer), _
                        CType(rowleggiAreaOmogenee("Altimetria"), String), _
                        CType(rowleggiAreaOmogenee("so"), Double), _
                        DBNullToNothing(rowleggiAreaOmogenee("TipoZona")), _
                        CType(rowleggiAreaOmogenee("Area_Validita_Inizio"), Date), _
                        CType(rowleggiAreaOmogenee("Area_Validita_Fine"), Date), _
                        objOpzioni.objParametri_Server_GIAS_DESTINAZIONE, _
                        CType(rowleggiAreaOmogenee("Area_data_creazione"), Date), _
                        CType(rowleggiAreaOmogenee("Area_Data_modifica"), Date), _
                        CType(rowleggiAreaOmogenee("Area_Username_creazione"), String), _
                        CType(rowleggiAreaOmogenee("Area_Username_modifica"), String) _
                        )

                    dtAreaTmp = ScriviAreaOmogenea.EseguiQuery_Lettura(objOpzioni.objParametri_Server_GIAS_DESTINAZIONE, "SELECT SCOPE_IDENTITY() ", "")

                    If dtAreaTmp.Rows(0)(0) Is DBNull.Value Then
                        Throw New Exception(nomeFunzione & ": Mancata lettura dell'Identity per il campo area_Cod")
                    End If
                    area_cod = dtAreaTmp.Rows(0)(0)


                    _AreeOmogenee.Add( _
                        New recode_areeOmogenee With { _
                            .From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser, _
                            .To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser, _
                            .From_Area_Omogenea_cod = CType(rowleggiAreaOmogenee("Area_Cod"), Integer), _
                            .To_Area_Omogenea_cod = area_cod _
                        })
                Next
            End If

        Catch ex As Exception

            Dim msg As String
            msg = CStr(Date.Now) & " - " & nomeFunzione & " Si è verificato il seguente errore: " & ex.Message & vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)
        End Try


    End Sub


#End Region




    Public Shared Function Get_Str_Credenziali_WS(ByVal objOpzioni As clsOpzioni) As String

        Dim oUtenti As New AgronicaCoreUtentiDAL.Utenti_Read
        Dim SU_piva As String = ""
        Dim SU_username As String = ""
        Dim SU_password As String = ""

        '  Marco Grilli, 18/08/2014 11:29:11: Creo l'oggetto di collegamento col web service
        Dim ws As New WS_Importa_GIAS_2014.ImportaWS()
        ws.Url = objOpzioni.wsimportaGiasURl
        ws.Timeout = 600000

        '  Marco Grilli, 18/08/2014 11:29:33: ERR: non ha senso che siano fatti in locale dato che è un G2G con WS!
        'oUtenti.Leggi_SuperUserPiva(SU_piva, objOpzioni.objParametri_Utenti_GIAS_DESTINAZIONE.StringaConnessione)
        'oUtenti.Leggi_SuperUser(SU_piva, SU_username, "", SU_password, objOpzioni.objParametri_Utenti_GIAS_DESTINAZIONE.StringaConnessione)

        '  Marco Grilli, 18/08/2014 12:04:21: Leggo i dati degli utenti dal Web Service
        ws.Leggi_SuperUserPiva(SU_piva, objOpzioni.objParametri_Utenti_GIAS_DESTINAZIONE.StringaConnessione)
        ws.Leggi_SuperUser(SU_piva, SU_username, "", SU_password, objOpzioni.objParametri_Utenti_GIAS_DESTINAZIONE.StringaConnessione)

        Dim xCredenziali As New AgronicaCoreXML.XML_WS_Importa_Gias

        Return _
            xCredenziali.Genera_Stringa_Credenziali(
                True,
                Nothing,
                SU_username,
                SU_password,
                objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser,
                False, "", "", "", "", "", "",
                objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.StringaConnessione,
                objOpzioni.objParametri_Utenti_GIAS_DESTINAZIONE.StringaConnessione)

    End Function





End Class
