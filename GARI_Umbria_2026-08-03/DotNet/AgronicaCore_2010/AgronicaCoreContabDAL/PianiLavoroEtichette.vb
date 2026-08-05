Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreEntityFramework
Imports Newtonsoft.Json

Public Class PianiLavoroEtichette_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function LeggiElencoPianiLavoro(ByVal piva As String,
                                           ByVal descrizione As String,
                                           ByVal note As String,
                                           ByVal lotto As String,
                                           ByRef dtPianiLavoro As DataTable,
                                           ByRef objParametri As AgronicaCoreParametri,
                                           Optional ByVal dataCreazioneDa As Date = AGRODATAINIZIO,
                                           Optional ByVal dataCreazioneA As Date = AGRODATAFINE
                                           ) As String
        
        Const nomeRoutine = "AgronicaCoreContabDAL.PianiLavoroEtichette_R.LeggiPianiLavoro()"
        Dim messaggioErrore As String = ""
        Dim risposta As String = ""

        Try
            Dim gefutils As New Gias_EF_Utility

            Dim efConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

            Using giasContext As New Gias_DeveloperServer_Entities(efConnString)

                giasContext.Configuration.LazyLoadingEnabled = False
         
                Dim elenco = From ag In giasContext.Agenda
                             Join mv In giasContext.Movimenti On ag.PIVA Equals mv.PIVA And ag.Id_Agenda Equals mv.Id_Agenda
                             Where ag.PIVA = piva AndAlso
                                   ag.Lav_Cod = LAVCOD_MONITORAGGIO_TEMPI_RIENTRO AndAlso
                                   mv.Cau_Mov = CAU_CARICO AndAlso
                                   ag.Data_Creazione >= dataCreazioneDa AndAlso ag.Data_Creazione <= dataCreazioneA AndAlso
                                   (descrizione = "" OrElse ag.des_lib.Contains(descrizione)) AndAlso 
                                   (note = "" OrElse mv.Mov_Desc.Contains(note)) AndAlso
                                   (lotto = "" OrElse mv.Extra_Str.Contains(lotto))
                             Select New With {
                                .Chiave = "",
                                .Piva = ag.PIVA,
                                .IdAgenda = ag.Id_Agenda,
                                .IdMov = mv.Id_Mov,
                                .DataCreazione = ag.Data_Creazione,
                                .Descrizione = ag.Des_lib,
                                .Note = mv.Mov_Desc,
                                .LottoDefault = mv.Extra_Str,
                                .SovrascriviLotto = False,
                                .DataOraUltimaLettura = Now
                             }

                Dim list = elenco.ToList()

                For Each item in list
                    item.Chiave = item.Piva & "_" & item.IdAgenda & "_" & item.IdMov
                Next
                
                Dim ut As New Gias_EF_Utility
                dtPianiLavoro = ut.ObjectQueryToDataTable(list)

                Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
                risposta = JsonConvert.SerializeObject(list, Formatting.None, serializerSettings)

            End Using

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return risposta

    End Function

    Public Function LeggiDettagliPianoLavoro(ByVal piva As String,
                                             ByVal idAgenda As Integer,
                                             ByRef dtDettagliPianoLavoro As DataTable,
                                             ByRef objParametri As AgronicaCoreParametri
                                             ) As String
        
        Const nomeRoutine = "AgronicaCoreContabDAL.PianiLavoroEtichette_R.LeggiPianiLavoro()"
        Dim messaggioErrore As String = ""
        Dim risposta As String = ""

        Try
            Dim gefutils As New Gias_EF_Utility

            Dim efConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

            Using giasContext As New Gias_DeveloperServer_Entities(efConnString)

                giasContext.Configuration.LazyLoadingEnabled = False
            
                Dim elenco = From md In giasContext.Movimenti_dettagli
                             Join mv In giasContext.Movimenti On md.PIVA Equals mv.PIVA And md.Id_Agenda Equals mv.Id_Agenda
                             Where md.PIVA = piva AndAlso
                                   md.Id_Agenda = idAgenda AndAlso
                                   mv.Cau_Mov = CAU_CARICO
                             Select md

                Dim list = elenco.ToList()
                
                Dim ut As New Gias_EF_Utility
                dtDettagliPianoLavoro = ut.ObjectQueryToDataTable(list)

                Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
                risposta = JsonConvert.SerializeObject(list, Formatting.None, serializerSettings)

            End Using

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return risposta

    End Function
    
End Class
