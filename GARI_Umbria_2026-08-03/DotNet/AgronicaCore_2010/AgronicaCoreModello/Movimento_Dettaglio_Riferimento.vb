Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider

Namespace OperazioneAgenda_Temp

    Public Class Movimento_Dettaglio_Riferimento

        Sub New(ByVal pivaInput As String, ByVal lavCod As Integer)

            Piva = pivaInput
            Sa_Cod = 0
            Id_Agenda = 0
            Id_Mov = 0
            Id_Mov_Det = 0
            Lav_Cod = lavCod
            Cau_Mov = ""

            Piva_Rif = ""
            Sa_Cod_Rif = 0
            Id_Agenda_Rif = 0
            Id_Mov_Rif = 0
            Id_Mov_Det_Rif = 0
            Lav_Cod_Rif = 0
            Cau_Mov_Rif = ""

            Qta = 0

            Preserva_Legame = 0

            Tipo_Associazione = 0

            'valori che poi il DAL in fase di scrittura sostituirà con quelli reali
            Data_Creazione = #2/1/1900#
            Data_Modifica = #2/1/1900#
            Username_Creazione = ""
            Username_Modifica = ""

            BaseCode = 0
            TopCode = 200000000

        End Sub

        Sub New()

            Piva = ""
            Sa_Cod = 0
            Id_Agenda = 0
            Id_Mov = 0
            Id_Mov_Det = 0
            Lav_Cod = 0
            Cau_Mov = ""

            Piva_Rif = ""
            Sa_Cod_Rif = 0
            Id_Agenda_Rif = 0
            Id_Mov_Rif = 0
            Id_Mov_Det_Rif = 0
            Lav_Cod_Rif = 0
            Cau_Mov_Rif = ""

            Qta = 0

            Preserva_Legame = 0

            Tipo_Associazione = 0

            'valori che poi il DAL in fase di scrittura sostituirà con quelli reali
            Data_Creazione = #2/1/1900#
            Data_Modifica = #2/1/1900#
            Username_Creazione = ""
            Username_Modifica = ""

            BaseCode = 0
            TopCode = 200000000

        End Sub

        Public Property Piva As String

        Public Property Sa_Cod As Integer

        Public Property Id_Agenda As Integer

        Public Property Id_Mov As Integer

        Public Property Id_Mov_Det As Integer

        Public Property Lav_Cod As Integer

        Public Property Cau_Mov As String

        Public Property Piva_Rif As String

        Public Property Sa_Cod_Rif As Integer

        Public Property Id_Agenda_Rif As Integer

        Public Property Id_Mov_Rif As Integer

        Public Property Id_Mov_Det_Rif As Integer

        Public Property Lav_Cod_Rif As Integer

        Public Property Cau_Mov_Rif As String

        Public Property Qta As Decimal

        Public Property Data_Creazione As DateTime
        
        Public Property Data_Modifica As DateTime

        Public Property Username_Creazione As String

        Public Property Username_Modifica As String

        Public Property TopCode As Integer

        Public Property BaseCode As Integer

        Public Property Preserva_Legame As Integer

        Public Property Tipo_Associazione As Integer

    End Class

    Public Class Agenda_Movimenti_Dettagli_Riferimenti_Helper

        Public Function Scrivi(ByVal Movimento_Dettaglio_Riferimento As Movimento_Dettaglio_Riferimento,
                               ByVal objParametri As AgronicaCoreParametri
                               ) As Boolean


            Dim flagConnessione As Boolean = False
            Dim flagTransazione As Boolean = False

            Try

                Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)

                Dim objMovimentiDettagliRif As New AgronicaCoreContabDAL.Mov_Det_Riferimenti_W

                objMovimentiDettagliRif.Scrivi(Movimento_Dettaglio_Riferimento.Piva,
                                               Movimento_Dettaglio_Riferimento.Sa_Cod,
                                               Movimento_Dettaglio_Riferimento.Id_Agenda,
                                               Movimento_Dettaglio_Riferimento.Id_Mov,
                                               Movimento_Dettaglio_Riferimento.Id_Mov_Det,
                                               Movimento_Dettaglio_Riferimento.Lav_Cod,
                                               Movimento_Dettaglio_Riferimento.Cau_Mov,
                                               Movimento_Dettaglio_Riferimento.Piva_Rif,
                                               Movimento_Dettaglio_Riferimento.Sa_Cod_Rif,
                                               Movimento_Dettaglio_Riferimento.Id_Agenda_Rif,
                                               Movimento_Dettaglio_Riferimento.Id_Mov_Rif,
                                               Movimento_Dettaglio_Riferimento.Id_Mov_Det_Rif,
                                               Movimento_Dettaglio_Riferimento.Lav_Cod_Rif,
                                               Movimento_Dettaglio_Riferimento.Cau_Mov_Rif,
                                               Movimento_Dettaglio_Riferimento.Qta,
                                               AGRODATAINIZIO,
                                               AGRODATAFINE,
                                               objParametri,
                                               Data_creazione:=Movimento_Dettaglio_Riferimento.Data_Creazione,
                                               username_creazione:=Movimento_Dettaglio_Riferimento.Username_Creazione,
                                               Preserva_Legame:=Movimento_Dettaglio_Riferimento.Preserva_Legame,
                                               Tipo_Associazione:=Movimento_Dettaglio_Riferimento.Tipo_Associazione)

                objMovimentiDettagliRif = Nothing

                Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

            Catch ex As Exception

                Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)
                Throw New Exception("[ Agenda_Movimenti_Dettagli_Riferimenti_Helper.scrivi() ] : " & ex.Message)
            Finally

                Utility.VerificaChiudiConnessione(objParametri, flagConnessione)

            End Try

            Return True

        End Function

        Public Function Cancella(ByVal piva As String,
                                 ByVal saCod As Integer,
                                 ByVal idAgenda As Integer,
                                 ByVal idMov As Integer,
                                 ByVal idMovDet As Integer,
                                 ByVal objParametri As AgronicaCoreParametri
                                 ) As Boolean

            Dim flagConnessione As Boolean = False
            Dim flagTransazione As Boolean = False

            Try

                Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)


                Dim objMovimentiDettagliRif As New AgronicaCoreContabDAL.Mov_Det_Riferimenti_W

                objMovimentiDettagliRif.Cancella(piva,
                                                 saCod,
                                                 idAgenda,
                                                 idMov,
                                                 idMovDet,
                                                 "",
                                                 objParametri)

                objMovimentiDettagliRif = Nothing

                Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

            Catch ex As Exception

                Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)
                Throw New Exception("[ Agenda_Movimenti_Dettagli_Riferimenti_Helper.cancella() ] : " & ex.Message)

            Finally

                Utility.VerificaChiudiConnessione(objParametri, flagConnessione)

            End Try

            Return True

        End Function

        Public Function Leggi(ByVal piva As String,
                              ByVal saCod As Integer,
                              ByVal idAgenda As Integer,
                              ByVal idMov As Integer,
                              ByVal idMovDet As Integer,
                              ByVal lavCod As Integer,
                              ByVal cauMov As String,
                              ByVal objParametri As AgronicaCoreParametri,
                              Optional ByVal xFiltroAggiuntivo As String = ""
                              ) As List(Of Movimento_Dettaglio_Riferimento)

            Dim flagConnessione As Boolean = False

            Dim listaMovimentiDettagliRif As New List(Of Movimento_Dettaglio_Riferimento)
            Dim Movimento_Dettaglio_Riferimento As Movimento_Dettaglio_Riferimento

            Try

                Utility.VerificaApriConnessione(objParametri, flagConnessione)

                '--------------------------------------------------------
                '-------- MOVIMENTI_DETTAGLI_RIFERIMENTI ----------------
                '--------------------------------------------------------
                Dim objMovDettagliRif = New AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R
                Dim dtMovDettagliRif As DataTable

                dtMovDettagliRif = objMovDettagliRif.Leggi(CStr(piva),
                                                           CInt(saCod),
                                                           CInt(idAgenda),
                                                           idMov,
                                                           idMovDet,
                                                           lavCod,
                                                           cauMov,
                                                           xFiltroAggiuntivo,
                                                           enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                           objParametri)

                objMovDettagliRif = Nothing

                If dtMovDettagliRif.Rows.Count > 0 Then

                    For j = 0 To dtMovDettagliRif.Rows.Count - 1

                        Movimento_Dettaglio_Riferimento = New Movimento_Dettaglio_Riferimento With {
                            .Piva = dtMovDettagliRif.Rows(j).Item("Piva"),
                            .Sa_Cod = dtMovDettagliRif.Rows(j).Item("Sa_Cod"),
                            .Id_Agenda = dtMovDettagliRif.Rows(j).Item("Id_Agenda"),
                            .Id_Mov = dtMovDettagliRif.Rows(j).Item("Id_Mov"),
                            .Id_Mov_Det = dtMovDettagliRif.Rows(j).Item("Id_Mov_Det"),
                            .Lav_Cod = dtMovDettagliRif.Rows(j).Item("Lav_Cod"),
                            .Cau_Mov = dtMovDettagliRif.Rows(j).Item("Cau_Mov"),
                            .Piva_Rif = dtMovDettagliRif.Rows(j).Item("Piva_Rif"),
                            .Sa_Cod_Rif = dtMovDettagliRif.Rows(j).Item("Sa_Cod_Rif"),
                            .Id_Agenda_Rif = dtMovDettagliRif.Rows(j).Item("Id_Agenda_Rif"),
                            .Id_Mov_Rif = dtMovDettagliRif.Rows(j).Item("Id_Mov_Rif"),
                            .Id_Mov_Det_Rif = dtMovDettagliRif.Rows(j).Item("Id_Mov_Det_Rif"),
                            .Lav_Cod_Rif = dtMovDettagliRif.Rows(j).Item("Lav_Cod_Rif"),
                            .Cau_Mov_Rif = dtMovDettagliRif.Rows(j).Item("Cau_Mov_Rif"),
                            .Qta = dtMovDettagliRif.Rows(j).Item("Qta"),
                            .Data_Creazione = CDate(dtMovDettagliRif.Rows(j).Item("Data_Creazione")),
                            .Data_Modifica = CDate(dtMovDettagliRif.Rows(j).Item("Data_Modifica")),
                            .Username_Creazione = dtMovDettagliRif.Rows(j).Item("Username_Creazione"),
                            .Username_Modifica = dtMovDettagliRif.Rows(j).Item("Username_Modifica"),
                            .Preserva_Legame = If(Not IsDBNull(dtMovDettagliRif.Rows(j).Item("Preserva_Legame")), dtMovDettagliRif.Rows(j).Item("Preserva_Legame"), 0),
                            .Tipo_Associazione = dtMovDettagliRif.Rows(j).Item("Tipo_Associazione")
                        }

                        listaMovimentiDettagliRif.Add(Movimento_Dettaglio_Riferimento)

                    Next

                End If

            Catch ex As Exception

                Throw New Exception("[ Agenda_Movimenti_Dettagli_Riferimenti_Helper.leggi() ] : " & ex.Message)

            Finally

                Utility.VerificaChiudiConnessione(objParametri, flagConnessione)
            End Try

            Return listaMovimentiDettagliRif

        End Function

        Public Function LeggiRiferimentiAgenda(ByVal piva As String,
                      ByVal saCod As Integer,
                      ByVal idAgenda As Integer,
                      ByVal lavCod As Integer,
                      ByVal cauMov As String,
                      ByVal objParametri As AgronicaCoreParametri
                      ) As List(Of Movimento_Dettaglio_Riferimento)

            Dim flagConnessione As Boolean = False

            Dim listaMovimentiDettagliRif As New List(Of Movimento_Dettaglio_Riferimento)
            Dim Movimento_Dettaglio_Riferimento As Movimento_Dettaglio_Riferimento

            Try

                Utility.VerificaApriConnessione(objParametri, flagConnessione)

                '--------------------------------------------------------
                '-------- MOVIMENTI_DETTAGLI_RIFERIMENTI ----------------
                '--------------------------------------------------------
                Dim objMovDettagliRif = New AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R
                Dim dtMovDettagliRif As DataTable

                dtMovDettagliRif = objMovDettagliRif.LeggiPerAgenda(CStr(piva),
                                                           CInt(saCod),
                                                           CInt(idAgenda),
                                                           lavCod,
                                                           cauMov,
                                                           "",
                                                           enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                           objParametri)

                objMovDettagliRif = Nothing

                If dtMovDettagliRif.Rows.Count > 0 Then

                    For j = 0 To dtMovDettagliRif.Rows.Count - 1

                        Movimento_Dettaglio_Riferimento = New Movimento_Dettaglio_Riferimento With {
                            .Piva = dtMovDettagliRif.Rows(j).Item("Piva"),
                            .Sa_Cod = dtMovDettagliRif.Rows(j).Item("Sa_Cod"),
                            .Id_Agenda = dtMovDettagliRif.Rows(j).Item("Id_Agenda"),
                            .Id_Mov = dtMovDettagliRif.Rows(j).Item("Id_Mov"),
                            .Id_Mov_Det = dtMovDettagliRif.Rows(j).Item("Id_Mov_Det"),
                            .Lav_Cod = dtMovDettagliRif.Rows(j).Item("Lav_Cod"),
                            .Cau_Mov = dtMovDettagliRif.Rows(j).Item("Cau_Mov"),
                            .Piva_Rif = dtMovDettagliRif.Rows(j).Item("Piva_Rif"),
                            .Sa_Cod_Rif = dtMovDettagliRif.Rows(j).Item("Sa_Cod_Rif"),
                            .Id_Agenda_Rif = dtMovDettagliRif.Rows(j).Item("Id_Agenda_Rif"),
                            .Id_Mov_Rif = dtMovDettagliRif.Rows(j).Item("Id_Mov_Rif"),
                            .Id_Mov_Det_Rif = dtMovDettagliRif.Rows(j).Item("Id_Mov_Det_Rif"),
                            .Lav_Cod_Rif = dtMovDettagliRif.Rows(j).Item("Lav_Cod_Rif"),
                            .Cau_Mov_Rif = dtMovDettagliRif.Rows(j).Item("Cau_Mov_Rif"),
                            .Qta = dtMovDettagliRif.Rows(j).Item("Qta"),
                            .Data_Creazione = CDate(dtMovDettagliRif.Rows(j).Item("Data_Creazione")),
                            .Data_Modifica = CDate(dtMovDettagliRif.Rows(j).Item("Data_Modifica")),
                            .Username_Creazione = dtMovDettagliRif.Rows(j).Item("Username_Creazione"),
                            .Username_Modifica = dtMovDettagliRif.Rows(j).Item("Username_Modifica"),
                            .Preserva_Legame = If(Not IsDBNull(dtMovDettagliRif.Rows(j).Item("Preserva_Legame")), dtMovDettagliRif.Rows(j).Item("Preserva_Legame"), 0)
                        }

                        listaMovimentiDettagliRif.Add(Movimento_Dettaglio_Riferimento)

                    Next

                End If

            Catch ex As Exception

                Throw New Exception("[ Agenda_Movimenti_Dettagli_Riferimenti_Helper.leggiRiferimentiAgenda() ] : " & ex.Message)

            Finally

                Utility.VerificaChiudiConnessione(objParametri, flagConnessione)
            End Try

            Return listaMovimentiDettagliRif

        End Function

    End Class

End Namespace
