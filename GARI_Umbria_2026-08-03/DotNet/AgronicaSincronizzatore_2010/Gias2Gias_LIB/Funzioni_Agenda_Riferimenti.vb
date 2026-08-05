Imports System.Xml
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports System.Text
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreG2GLocalDal

Partial Public Class Funzioni

    Public Sub Elabora_RiferimentiAgenda_WS(
            ByVal piva As String,
            ByVal objOpzioni As clsOpzioni,
            ByRef Log_Import As StringBuilder,
            ByRef Log_Errori As StringBuilder,
            ByRef Log_Riepilogo As StringBuilder
        )

        Const nomeFunzione As String = "Elabora_RiferimentiAgenda_WS"

        Try

            Dim Str_Credenziali_WS As String = Get_Str_Credenziali_WS(objOpzioni)
            Dim ws As New WS_Importa_GIAS_2014.ImportaWS
            ws.Url = objOpzioni.wsimportaGiasURl
            ws.Timeout = _timeout_ws_Importa

            If ws.Aggiorna_Riferimenti_Agenda(Str_Credenziali_WS, objOpzioni.SuperUser_CodFiscale_ORIGINE, piva) Then
                G2GUtility.Log(Log_Import, "Si è verificato un errore durante l'aggiornamento dei riferimenti agenda")
            End If

        Catch ex As Exception

            Dim msg As String = CStr(Date.Now) + " - " + nomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            'Throw New Exception(msg)

        End Try

    End Sub

    Public Sub Elabora_RiferimentiAgenda_WSReverse(
            ByVal piva As String,
            ByVal objOpzioni As clsOpzioni,
            ByRef Log_Import As StringBuilder,
            ByRef Log_Errori As StringBuilder,
            ByRef Log_Riepilogo As StringBuilder
        )

        Const nomeFunzione As String = "Elabora_RiferimentiAgenda_WSReverse"

        Try

            Dim Str_Credenziali_WS As String = Get_Str_Credenziali_WS(objOpzioni)
            Dim ws As New WS_Importa_GIAS_2014.ImportaWS
            ws.Url = objOpzioni.wsimportaGiasURl
            ws.Timeout = _timeout_ws_Importa

            If ws.Aggiorna_Riferimenti_AgendaReverse(Str_Credenziali_WS, objOpzioni.SuperUser_CodFiscale_ORIGINE, piva) Then
                G2GUtility.Log(Log_Import, "Si è verificato un errore durante l'aggiornamento dei riferimenti agenda")
            End If

        Catch ex As Exception

            Dim msg As String = CStr(Date.Now) + " - " + nomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            'Throw New Exception(msg)

        End Try

    End Sub

    Public Sub Elabora_RiferimentiAgenda(
            ByVal piva As String,
            ByVal objOpzioni As clsOpzioni,
            ByRef Log_Import As StringBuilder,
            ByRef Log_Errori As StringBuilder,
            ByRef Log_Riepilogo As StringBuilder
        )

        Dim xMovimentiRifer As New AgronicaCoreContabDAL.Mov_Det_Riferimenti_W
        Dim llMov As List(Of G2G_Recode_Movimenti)
        Dim llMov_Dettagli As List(Of G2G_Recode_Mov_Dettagli)
        Dim righeAggiornate As Int32

        '  Marco Grilli, 28/08/2014 12:45:27: Oggetti per il G2G su WS
        Dim Str_Credenziali_WS As String = Nothing
        Dim ws As New WS_Importa_GIAS_2014.ImportaWS

        Dim idAgendaAggiornati As New List(Of Integer)

        Try

            '  Marco Grilli, 28/08/2014 12:49:23: estraggo la lista dei movimenti da scrivere
            If NuovaLogicaRecodeAG Then
                llMov = (
                    From mm In _efG2G.G2G_Recode_Movimenti
                    Join ii In _efG2G.G2G_Recode_Agenda On ii.FromId_Agenda Equals mm.FromId_Agenda _
                    And ii.FromPiva Equals mm.FromPiva
                    Where ii.RiferimentiElaborati = 0 _
                    And ii.FromPiva = piva
                    Select mm
            ).ToList
            Else
                llMov = (
                    From mm In _Movimenti
                    Join ii In _Agenda On ii.FromId_Agenda Equals mm.FromId_Agenda _
                    And ii.FromPiva Equals mm.FromPiva
                    Where ii.RiferimentiElaborati = 0 _
                    And ii.FromPiva = piva
                    Select mm
            ).ToList
            End If


            '  Marco Grilli, 28/08/2014 12:54:28: Se il G2G è locale
            If objOpzioni.isGias2Gias_local Then
                For Each lMov As G2G_Recode_Movimenti In llMov
                    xMovimentiRifer.G2G_ModificaCodici(
                                                        lMov.FromPiva,
                                                        0,
                                                        lMov.FromId_Agenda,
                                                        lMov.FromId_mov,
                                                        -1,
                                                        lMov.ToPiva,
                                                        0,
                                                        lMov.ToId_Agenda,
                                                        lMov.ToId_mov,
                                                        -1,
                                                        objOpzioni.objParametri_Server_GIAS_DESTINAZIONE,
                                                        righeAggiornate
                    )
                    '  Marco Grilli, 29/08/2014 16:30:51: 
                    If righeAggiornate > 0 Then
                        idAgendaAggiornati.Add(lMov.FromId_Agenda)
                    End If

                    xMovimentiRifer.G2G_ModificaCodici(
                                                       lMov.FromPiva,
                                                       lMov.FromSa_cod,
                                                       lMov.FromId_Agenda,
                                                       lMov.FromId_mov,
                                                       -1,
                                                       lMov.ToPiva,
                                                       lMov.ToSa_cod,
                                                       lMov.ToId_Agenda,
                                                       lMov.ToId_mov,
                                                       -1,
                                                       objOpzioni.objParametri_Server_GIAS_DESTINAZIONE,
                                                       righeAggiornate
                    )
                    If righeAggiornate > 0 Then
                        idAgendaAggiornati.Add(lMov.FromId_Agenda)
                    End If

                Next
                '  Marco Grilli, 28/08/2014 12:54:28: Se il G2G è su WS
            Else
                For Each lMov As G2G_Recode_Movimenti In llMov
                    ws.Scrivi_Mov_Det_Riferimenti(
                                                    Str_Credenziali_WS,
                                                    lMov.FromPiva,
                                                    0,
                                                    lMov.FromId_Agenda,
                                                    lMov.FromId_mov,
                                                    -1,
                                                    lMov.ToPiva,
                                                    0,
                                                    lMov.ToId_Agenda,
                                                    lMov.ToId_mov,
                                                    -1,
                                                    righeAggiornate
                    )
                    If righeAggiornate > 0 Then
                        idAgendaAggiornati.Add(lMov.FromId_Agenda)
                    End If

                    ws.Scrivi_Mov_Det_Riferimenti(
                                                    Str_Credenziali_WS,
                                                    lMov.FromPiva,
                                                    lMov.FromSa_cod,
                                                    lMov.FromId_Agenda,
                                                    lMov.FromId_mov,
                                                    -1,
                                                    lMov.ToPiva,
                                                    lMov.ToSa_cod,
                                                    lMov.ToId_Agenda,
                                                    lMov.ToId_mov,
                                                    -1,
                                                    righeAggiornate
                    )
                    If righeAggiornate > 0 Then
                        idAgendaAggiornati.Add(lMov.FromId_Agenda)
                    End If

                Next
            End If

            '  Marco Grilli, 28/08/2014 12:49:23: estraggo la lista dei movimenti_dettagli da scrivere
            If NuovaLogicaRecodeAG Then
                llMov_Dettagli = (
                    From mm In _efG2G.G2G_Recode_Mov_Dettagli
                    Join ii In _efG2G.G2G_Recode_Agenda On ii.FromId_Agenda Equals mm.FromId_Agenda _
                    And ii.FromPiva Equals mm.FromPiva
                    Where ii.RiferimentiElaborati = 0 _
                    And ii.FromPiva = piva
                    Select mm
                ).ToList
            Else
                llMov_Dettagli = (
                    From mm In _Mov_Dettagli
                    Join ii In _Agenda On ii.FromId_Agenda Equals mm.FromId_Agenda _
                    And ii.FromPiva Equals mm.FromPiva
                    Where ii.RiferimentiElaborati = 0 _
                    And ii.FromPiva = piva
                    Select mm
                ).ToList
            End If

            '  Marco Grilli, 28/08/2014 12:54:28: Se il G2G è locale
            If objOpzioni.isGias2Gias_local Then
                For Each lMov As G2G_Recode_Mov_Dettagli In llMov_Dettagli
                    xMovimentiRifer.G2G_ModificaCodici(
                                                    lMov.FromPiva,
                                                    0,
                                                    lMov.FromId_Agenda,
                                                    lMov.FromId_mov,
                                                    lMov.FromId_mov_det,
                                                    lMov.ToPiva,
                                                    0,
                                                    lMov.ToId_Agenda,
                                                    lMov.ToId_mov,
                                                    lMov.ToId_mov_det,
                                                    objOpzioni.objParametri_Server_GIAS_DESTINAZIONE,
                                                    righeAggiornate
                    )

                    If righeAggiornate > 0 Then
                        idAgendaAggiornati.Add(lMov.FromId_Agenda)
                    End If

                    xMovimentiRifer.G2G_ModificaCodici(
                                                    lMov.FromPiva,
                                                    lMov.FromSa_cod,
                                                    lMov.FromId_Agenda,
                                                    lMov.FromId_mov,
                                                    lMov.FromId_mov_det,
                                                    lMov.ToPiva,
                                                    lMov.ToSa_cod,
                                                    lMov.ToId_Agenda,
                                                    lMov.ToId_mov,
                                                    lMov.ToId_mov_det,
                                                    objOpzioni.objParametri_Server_GIAS_DESTINAZIONE,
                                                    righeAggiornate
                    )

                    If righeAggiornate > 0 Then
                        idAgendaAggiornati.Add(lMov.FromId_Agenda)
                    End If
                Next
                '  Marco Grilli, 28/08/2014 12:54:28: Se il G2G è su WS
            Else
                For Each lMov As G2G_Recode_Mov_Dettagli In llMov_Dettagli
                    ws.Scrivi_Mov_Det_Riferimenti(
                                                    Str_Credenziali_WS,
                                                    lMov.FromPiva,
                                                    0,
                                                    lMov.FromId_Agenda,
                                                    lMov.FromId_mov,
                                                    lMov.FromId_mov_det,
                                                    lMov.ToPiva,
                                                    0,
                                                    lMov.ToId_Agenda,
                                                    lMov.ToId_mov,
                                                    lMov.ToId_mov_det,
                                                    righeAggiornate
                    )
                    If righeAggiornate > 0 Then
                        idAgendaAggiornati.Add(lMov.FromId_Agenda)
                    End If

                    ws.Scrivi_Mov_Det_Riferimenti(
                                                    Str_Credenziali_WS,
                                                    lMov.FromPiva,
                                                    lMov.FromSa_cod,
                                                    lMov.FromId_Agenda,
                                                    lMov.FromId_mov,
                                                    lMov.FromId_mov_det,
                                                    lMov.ToPiva,
                                                    lMov.ToSa_cod,
                                                    lMov.ToId_Agenda,
                                                    lMov.ToId_mov,
                                                    lMov.ToId_mov_det,
                                                    righeAggiornate
                    )
                    If righeAggiornate > 0 Then
                        idAgendaAggiornati.Add(lMov.FromId_Agenda)
                    End If
                Next

            End If

            Dim agendaModificata = Nothing
            If NuovaLogicaRecodeAG Then
                agendaModificata =
                    From a In _efG2G.G2G_Recode_Agenda
                    Where idAgendaAggiornati.Contains(a.FromId_Agenda)
                    Select a
            Else
                agendaModificata =
                    From a In _Agenda
                    Where idAgendaAggiornati.Contains(a.FromId_Agenda)
                    Select a
            End If

            For Each a In agendaModificata
                a.RiferimentiElaborati = 1
            Next

        Catch ex As Exception

        End Try

    End Sub

    Public Sub Elabora_RiferimentiAgendaReverse(
            ByVal piva As String,
            ByVal objOpzioni As clsOpzioni,
            ByRef Log_Import As StringBuilder,
            ByRef Log_Errori As StringBuilder,
            ByRef Log_Riepilogo As StringBuilder
        )

        Dim xMovimentiRifer As New AgronicaCoreContabDAL.Mov_Det_Riferimenti_W
        Dim llMov As List(Of G2G_Recode_Movimenti)
        Dim llMov_Dettagli As List(Of G2G_Recode_Mov_Dettagli)
        Dim righeAggiornate As Int32

        '  Marco Grilli, 28/08/2014 12:45:27: Oggetti per il G2G su WS
        Dim Str_Credenziali_WS As String = Nothing
        Dim ws As New WS_Importa_GIAS_2014.ImportaWS

        Dim idAgendaAggiornati As New List(Of Integer)

        Try

            '  Marco Grilli, 28/08/2014 12:49:23: estraggo la lista dei movimenti da scrivere
            If NuovaLogicaRecodeAG Then
                llMov = (
                    From mm In _efG2G.G2G_Recode_Movimenti
                    Join ii In _efG2G.G2G_Recode_Agenda On ii.ToId_Agenda Equals mm.ToId_Agenda _
                    And ii.ToPiva Equals mm.ToPiva
                    Where ii.RiferimentiElaborati = 0 _
                    And ii.ToPiva = piva
                    Select mm
            ).ToList
            Else
                Throw New Exception("Vecchia logica recode non supportata")
            End If


            '  Marco Grilli, 28/08/2014 12:54:28: Se il G2G è locale
            If objOpzioni.isGias2Gias_local Then
                Throw New Exception("G2G Local non supportato")
                '  Marco Grilli, 28/08/2014 12:54:28: Se il G2G è su WS
            Else
                For Each lMov As G2G_Recode_Movimenti In llMov
                    ws.Scrivi_Mov_Det_Riferimenti(
                                                    Str_Credenziali_WS,
                                                    lMov.ToPiva,
                                                    0,
                                                    lMov.ToId_Agenda,
                                                    lMov.ToId_mov,
                                                    -1,
                                                    lMov.FromPiva,
                                                    0,
                                                    lMov.FromId_Agenda,
                                                    lMov.FromId_mov,
                                                    -1,
                                                    righeAggiornate
                    )
                    If righeAggiornate > 0 Then
                        idAgendaAggiornati.Add(lMov.FromId_Agenda)
                    End If

                    ws.Scrivi_Mov_Det_Riferimenti(
                                                    Str_Credenziali_WS,
                                                    lMov.ToPiva,
                                                    lMov.ToSa_cod,
                                                    lMov.ToId_Agenda,
                                                    lMov.ToId_mov,
                                                    -1,
                                                    lMov.FromPiva,
                                                    lMov.FromSa_cod,
                                                    lMov.FromId_Agenda,
                                                    lMov.FromId_mov,
                                                    -1,
                                                    righeAggiornate
                    )
                    If righeAggiornate > 0 Then
                        idAgendaAggiornati.Add(lMov.FromId_Agenda)
                    End If

                Next
            End If

            '  Marco Grilli, 28/08/2014 12:49:23: estraggo la lista dei movimenti_dettagli da scrivere
            If NuovaLogicaRecodeAG Then
                llMov_Dettagli = (
                    From mm In _efG2G.G2G_Recode_Mov_Dettagli
                    Join ii In _efG2G.G2G_Recode_Agenda On ii.ToId_Agenda Equals mm.ToId_Agenda _
                    And ii.ToPiva Equals mm.ToPiva
                    Where ii.RiferimentiElaborati = 0 _
                    And ii.ToPiva = piva
                    Select mm
                ).ToList
            Else
                Throw New Exception("Vecchia logica recode non supportata")
            End If

            '  Marco Grilli, 28/08/2014 12:54:28: Se il G2G è locale
            If objOpzioni.isGias2Gias_local Then
                Throw New Exception("Vecchia logica recode non supportata")
            Else
                For Each lMov As G2G_Recode_Mov_Dettagli In llMov_Dettagli
                    ws.Scrivi_Mov_Det_Riferimenti(
                                                    Str_Credenziali_WS,
                                                    lMov.ToPiva,
                                                    0,
                                                    lMov.ToId_Agenda,
                                                    lMov.ToId_mov,
                                                    lMov.ToId_mov_det,
                                                    lMov.FromPiva,
                                                    0,
                                                    lMov.FromId_Agenda,
                                                    lMov.FromId_mov,
                                                    lMov.FromId_mov_det,
                                                    righeAggiornate
                    )
                    If righeAggiornate > 0 Then
                        idAgendaAggiornati.Add(lMov.FromId_Agenda)
                    End If

                    ws.Scrivi_Mov_Det_Riferimenti(
                                                    Str_Credenziali_WS,
                                                    lMov.ToPiva,
                                                    lMov.ToSa_cod,
                                                    lMov.ToId_Agenda,
                                                    lMov.ToId_mov,
                                                    lMov.ToId_mov_det,
                                                    lMov.FromPiva,
                                                    lMov.FromSa_cod,
                                                    lMov.FromId_Agenda,
                                                    lMov.FromId_mov,
                                                    lMov.FromId_mov_det,
                                                    righeAggiornate
                    )
                    If righeAggiornate > 0 Then
                        idAgendaAggiornati.Add(lMov.FromId_Agenda)
                    End If
                Next

            End If

            Dim agendaModificata = Nothing
            If NuovaLogicaRecodeAG Then
                agendaModificata =
                    From a In _efG2G.G2G_Recode_Agenda
                    Where idAgendaAggiornati.Contains(a.ToId_Agenda)
                    Select a
            Else
                agendaModificata =
                    From a In _Agenda
                    Where idAgendaAggiornati.Contains(a.ToId_Agenda)
                    Select a
            End If

            For Each a In agendaModificata
                a.RiferimentiElaborati = 1
            Next

        Catch ex As Exception

        End Try

    End Sub

End Class
