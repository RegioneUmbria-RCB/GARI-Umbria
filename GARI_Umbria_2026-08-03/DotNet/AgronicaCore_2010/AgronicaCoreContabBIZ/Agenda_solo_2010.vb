
Imports System.Data
Imports System.Xml
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider

Public Class Agenda_solo_2010_R
    Inherits AgronicaCoreDataProvider.LogProvider


End Class


'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§


Public Class Agenda_solo_2010_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Sposta_Magazzino(
            ByVal Sa_Cod As Integer,
            ByVal Fabbricato_Cod As Integer,
            ByVal listaID_Agenda As List(Of obj_ID_Agenda_ID_Mov_ID_Mov_Det),
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
            ByRef errore As String
        ) As Boolean

        '============================================================================
        'NUOVA versione della funzione che usa "objParametri"
        'Restituisce in uscita il codice del nuovo "ID_Agenda" appena creato : OUTPUT_ID_Agenda
        '============================================================================

        '----------------------------------------------------------------------
        Dim NomeRoutine As String = "AgronicaCoreContabBIZ.Agenda_W.Agenda_Scrivi()"
        '----------------------------------------------------------------------


        '------------------------------
        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False


        Dim xRisp As Boolean = True
        '------------------------------
        errore = ""


        Try
            '------------------------------
            'Se la connessione è chiusa la apro
            If objParametri.objConnessione Is Nothing Then
                'Richiedo una connessione
                objParametri.objConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
                objParametri.objConnessione.Open()
                FlagConnessioneLocale = True
            ElseIf objParametri.objConnessione.State = ConnectionState.Closed Then
                objParametri.objConnessione.ConnectionString = DataProviderFactory.Instance.AggiustaStringaDiConnessione(objParametri.StringaConnessione)
                objParametri.objConnessione.Open()
                FlagConnessioneLocale = True
            End If

            If objParametri.objTransazione Is Nothing Then
                'Inizializzo la transazione
                objParametri.objTransazione = objParametri.objConnessione.BeginTransaction
                FlagTransazioneLocale = True
            End If

            '------------------------------
            Dim str_ID_Agenda As String = ""
            Dim str_ID_Agenda_ID_mov_id_mov_det As String = ""
            Dim str_ID_Agenda_ID_Mov As String = ""
            For Each e In listaID_Agenda
                If (str_ID_Agenda_ID_mov_id_mov_det = "") Then
                Else
                    str_ID_Agenda_ID_mov_id_mov_det &= " OR "
                End If
                If (str_ID_Agenda = "") Then
                Else
                    str_ID_Agenda &= " OR "
                End If
                If (str_ID_Agenda_ID_Mov = "") Then
                Else
                    str_ID_Agenda_ID_Mov &= " OR "
                End If

                str_ID_Agenda &= e.toString_ID_Agenda
                str_ID_Agenda_ID_Mov &= e.toString_ID_Agenda_ID_Mov
                str_ID_Agenda_ID_mov_id_mov_det &= e.toString_ID_Agenda_ID_Mov_ID_Mov_Det
            Next



            Dim objMov_Dest As New AgronicaCoreContabDAL.Mov_Destinazioni_W
            Dim objMovimenti_Dettagli As New AgronicaCoreContabDAL.Movimenti_Dettagli_W
            Dim objMovimenti As New AgronicaCoreContabDAL.Movimenti_W
            Dim objAgenda As New AgronicaCoreContabDAL.Agenda_W

            'modifico mov_destinazioni  -- sa_cod - id_destinazioni
            objMov_Dest.Modifica_x_Trasferimento(Sa_Cod, Fabbricato_Cod, str_ID_Agenda_ID_mov_id_mov_det, objParametri)

            'modifico movimenti_dettagli  -- sa_cod 
            objMovimenti_Dettagli.Modifica_x_Trasferimento(Sa_Cod, str_ID_Agenda_ID_mov_id_mov_det, objParametri)

            'modifico movimenti -- sa_cod se sa_cod<>0 
            objMovimenti.Modifica_x_Trasferimento(Sa_Cod, str_ID_Agenda_ID_Mov, objParametri)

            'modifico agenda -- sa_cod se sa_cod<>0 
            objAgenda.Modifica_x_Trasferimento(Sa_Cod, str_ID_Agenda, objParametri)


            'Restituisco un valore Dummy
            xRisp = True

            'Se ho la transazione è stata avviata in questa routine faccio il commit
            If FlagTransazioneLocale = True Then
                objParametri.objTransazione.Commit()
            End If

            '----------------------------------------------------------------------------
        Catch ex As Exception

            'Restituisco un valore Dummy
            xRisp = False

            'Faccio il rollback della transazione
            If Not objParametri.objTransazione Is Nothing Then
                objParametri.objTransazione.Rollback()
                objParametri.objTransazione = Nothing
            End If

            errore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, errore)
            xRisp = False

        Finally

            'Chiudo la connessione se è stata aperta in questa routine
            If (FlagConnessioneLocale = True) AndAlso (Not objParametri.objConnessione Is Nothing) Then
                objParametri.objConnessione.Close()
            End If

        End Try

        Return xRisp

    End Function



End Class

