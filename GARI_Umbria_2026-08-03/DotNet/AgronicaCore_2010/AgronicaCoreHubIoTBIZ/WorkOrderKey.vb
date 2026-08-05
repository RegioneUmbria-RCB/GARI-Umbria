Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDTOStd.SmartTractors_HubIoT

Public Class WorkOrderKey_R
    Inherits LogProvider

    Public Function LeggiElencoWorkOrderDaInviare(ByVal Piva As String,
                                                  ByVal VIN As String,
                                                  ByVal IdDocumento As Integer,
                                                  ByVal IdOperazioneDocumento As Integer,
                                                  ByRef ObjParametri As AgronicaCoreParametri) As List(Of WorkOrderKey)

        Dim NomeRoutine As String = "AgronicaCoreHubIoTBIZ.WorkOrderKey_R.LeggiElencoWorkOrderDaInviare()"
        Dim MessaggioErrore As String = ""

        Dim ret As New List(Of WorkOrderKey)
        Try
            Dim reader As New AgronicaCoreHubIoTDAL.HubIoT_WorkOrderKey_R

            Dim dt = reader.Leggi(ObjParametri.PivaSuperUser, Piva, 0, 0, 0, IdDocumento, IdOperazioneDocumento, 0, VIN, 0, AGRODATAINIZIO, "", "", ObjParametri)
            For Each row In dt.Rows
                ret.Add(New WorkOrderKey() With {
                            .Entita_Origine = CType(row("Entita_Origine"), Integer),
                            .PivaSuperUser = CType(row("PivaSuperUser"), String),
                            .Piva = CType(row("Piva"), String),
                            .sa_cod = CType(row("sa_cod"), Integer),
                            .appezza = CType(row("appezza"), Integer),
                            .id_reg = CType(row("id_reg"), Integer),
                            .Id_documento = CType(row("id_documento"), Integer),
                            .id_operazione_documento = CType(row("id_documento_operazione"), Integer),
                            .Mac_Cod = CType(row("Mac_Cod"), Integer),
                            .Data_Registrazione = CType(row("Data_Registrazione"), DateTime),
                            .Stato = CType(row("Stato"), Integer),
                            .workerOrderId = New Guid(row("WorkOrderId").ToString()).ToString().Replace("-", ""),
                            .RegolaElaborazione = CType(row("Id_RegolaElaborazione"), Integer)
                        })
            Next


        Catch ex As Exception
            ret.Clear()
            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri, NomeRoutine, MessaggioErrore)
        End Try

        Return ret
    End Function

End Class

Public Class WorkOrderKey_W
    Inherits LogProvider

    Public Function ScriviWorkOrderKeyDaRicetteOperazioni(ByVal Ricetta_Cod As Integer,
                                                          ByVal Ricetta_Operazione_Cod As Integer,
                                                          ByVal Mac_Cod As Integer,
                                                          ByRef objParametri As AgronicaCoreParametri
                                                          ) As Boolean
        Dim xRisp As Boolean = False
        'apro la transazione 
        Dim FlagTransazioneLocale As Boolean
        Dim FlagConnessioneLocale As Boolean

        '------------------------------
        'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
        AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri)
        Try
            Dim reader As New AgronicaCoreHubIoTDAL.HubIoT_WorkOrderData_R
            Dim writer As New AgronicaCoreHubIoTDAL.HubIoT_WorkOrderKey_W

            Dim dt = reader.LeggiElencoWorkerOrderKeyDaRicetteOperazioniXHubIoT(objParametri.PivaSuperUser, Ricetta_Cod, Ricetta_Operazione_Cod, Mac_Cod, "", "", objParametri)
            For Each row In dt.Rows
                If (row("mac_cod") IsNot DBNull.Value) Then
                    writer.Scrivi(row("Entita_Origine"), row("Ricetta_SuperUser"), row("Piva"), row("sa_cod"), row("appezza"), row("id_reg"), row("ricetta_Cod"), row("ricetta_operazione_cod"),
                              row("mac_cod"), row("Regola_Elaborazione"), row("Validita_inizio"), Guid.NewGuid.ToString(), objParametri)
                End If
            Next

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)

            xRisp = True
        Catch ex As Exception
            AgronicaCoreDataProvider.ConnessioniTransazioni.RollBackTransazione(objParametri)
            xRisp = False
        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                      objParametri)
        End Try

        Return xRisp

    End Function

    Public Function ScriviWorkOrderKeyDaOperazioniPianificate(ByVal id_agenda As Integer,
                                                              ByVal piva As String,
                                                              ByVal sa_cod As Integer,
                                                              ByVal appezza As Integer,
                                                              ByVal id_reg As Integer,
                                                              ByRef objParametri As AgronicaCoreParametri
                                                              ) As Boolean
        Dim xRisp As Boolean = False
        'apro la transazione 
        Dim FlagTransazioneLocale As Boolean
        Dim FlagConnessioneLocale As Boolean

        '------------------------------
        'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
        AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri)
        Try
            Dim reader As New AgronicaCoreHubIoTDAL.HubIoT_WorkOrderData_R
            Dim writer As New AgronicaCoreHubIoTDAL.HubIoT_WorkOrderKey_W

            Dim dt = reader.LeggiElencoWorkerOrderKeyDaOperazioniPianificateXHubIoT(objParametri.PivaSuperUser, id_agenda, piva, sa_cod, appezza, id_reg, "", "", objParametri)
            For Each row In dt.Rows
                If (row("mac_cod") IsNot DBNull.Value) Then
                    writer.Scrivi(row("Entita_Origine"), row("PivaSuperUser"), row("Piva"), row("sa_cod"), row("appezza"), row("id_reg"), row("id_agenda"), row("id_mov_det"),
                              row("mac_cod"), row("Id_RegolaElaborazione"), row("Validita_inizio"), Guid.NewGuid.ToString(), objParametri)
                End If
            Next

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)

            xRisp = True
        Catch ex As Exception
            AgronicaCoreDataProvider.ConnessioniTransazioni.RollBackTransazione(objParametri)
            xRisp = False
        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                      objParametri)
        End Try

        Return xRisp

    End Function

    Public Function AggiornaStatoWorkOrderKey(ByVal keyList As List(Of String),
                                              ByVal Stato As Integer,
                                              ByRef objParametri As AgronicaCoreParametri) As Boolean
        Dim xRisp As Boolean = False
        Try
            Dim writer As New AgronicaCoreHubIoTDAL.HubIoT_WorkOrderKey_W

            For Each key In keyList
                writer.Modifica(0, 0, "", "", 0, 0, 0, 0, 0, 0, 0, key, "", Stato, objParametri)
            Next
            xRisp = True
        Catch ex As Exception
            xRisp = False
        End Try
        Return xRisp
    End Function

End Class
