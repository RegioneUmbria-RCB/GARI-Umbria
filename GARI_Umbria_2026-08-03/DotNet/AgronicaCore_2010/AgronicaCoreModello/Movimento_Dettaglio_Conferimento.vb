Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Namespace OperazioneAgenda_Temp

    Public Class Movimento_Dettaglio_Conferimento
        Sub New()
            Piva = ""

            Sa_Cod = 0
            Id_Agenda = 0
            Id_Mov = 0
            Id_Mov_Det = 0
            Id_Reg_Dettaglio = 0

            Tagliando_Pesa = ""
            Premio_Complessivo = 0D
            Prezzo_Unitario_Finale = 0D
            Cod_Varieta = 0
            Desc_Appezzamenti = ""

            Validita_Inizio = AGRODATAINIZIO
            Validita_Fine = AGRODATAFINE

            'valori che poi il DAL in fase di scrittura sostituirà con quelli reali
            Data_Creazione = #2/1/1900#
            Data_Modifica = #2/1/1900#
            Username_Creazione = ""
            Username_Modifica = ""

            BaseCode = 0
            TopCode = 200000000

        End Sub

        Sub New(ByVal pivaInput As String)
            Piva = pivaInput

            Sa_Cod = 0
            Id_Agenda = 0
            Id_Mov = 0
            Id_Mov_Det = 0
            Id_Reg_Dettaglio = 0

            Tagliando_Pesa = ""
            Premio_Complessivo = 0D
            Prezzo_Unitario_Finale = 0D
            Cod_Varieta = 0
            Desc_Appezzamenti = ""

            Validita_Inizio = AGRODATAINIZIO
            validita_fine = AGRODATAFINE

            'valori che poi il DAL in fase di scrittura sostituirà con quelli reali
            Data_Creazione = #2/1/1900#
            Data_Modifica = #2/1/1900#
            Username_Creazione = ""
            Username_Modifica = ""

            BaseCode = 0
            TopCode = 200000000

        End Sub

        Public Property TopCode As Integer
        Public Property BaseCode As Integer

        Public Property Piva As String
        Public Property Sa_Cod As Integer
        Public Property Id_Agenda As Integer
        Public Property Id_Mov As Integer
        Public Property Id_Mov_Det As Integer
        Public Property Id_Reg_Dettaglio As Integer

        Public Property Tagliando_Pesa As String
        Public Property Premio_Complessivo As Decimal
        Public Property Prezzo_Unitario_Finale As Decimal
        Public Property Cod_Varieta As Integer
        Public Property Desc_Appezzamenti As String

        Public Property Validita_Inizio As DateTime
        Public Property Validita_Fine As DateTime

        Public Property Data_Creazione As DateTime
        Public Property Data_Modifica As DateTime

        Public Property Username_Creazione As String
        Public Property Username_Modifica As String

    End Class

    Public Class Agenda_Movimenti_Dettagli_Conferimento_Helper

        Public Function Leggi(ByVal piva As String,
                              ByVal saCod As Integer,
                              ByVal idAgenda As Integer,
                              ByVal idMov As Integer,
                              ByVal idMovDet As Integer,
                              ByVal idRegDettaglio As Integer,
                              ByVal objParametri As AgronicaCoreParametri
                              ) As List(Of Movimento_Dettaglio_Conferimento)

            Dim flagConnessione As Boolean = False

            Dim listaMovimentiDettagliConferimento As New List(Of Movimento_Dettaglio_Conferimento)
            Dim objMovDetConferimento As Movimento_Dettaglio_Conferimento

            Try

                Utility.VerificaApriConnessione(objParametri, flagConnessione)

                '--------------------------------------------------------
                '-------- MOVIMENTI_DETTAGLI_CONFERIMENTO ---------------
                '--------------------------------------------------------
                Dim objMovDetConfR As New AgronicaCoreContabDAL.Mov_Dett_Conferimento_R
                Dim dtMovDetConf As DataTable

                dtMovDetConf = objMovDetConfR.Leggi(CStr(piva), CInt(saCod), CInt(idAgenda),
                                                    idMov, idMovDet, idRegDettaglio,
                                                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                    "", "", objParametri)

                objMovDetConfR = Nothing

                If dtMovDetConf.Rows.Count > 0 Then

                    For Each conferimento As DataRow In dtMovDetConf.Rows

                        objMovDetConferimento = New Movimento_Dettaglio_Conferimento With {
                            .Piva = conferimento.Item("Piva"),
                            .Sa_Cod = conferimento.Item("Sa_Cod"),
                            .Id_Agenda = conferimento.Item("Id_Agenda"),
                            .Id_Mov = conferimento.Item("Id_Mov"),
                            .Id_Mov_Det = conferimento.Item("Id_Mov_Det"),
                            .Id_Reg_Dettaglio = conferimento.Item("Id_Reg_Dettaglio"),
                            .Tagliando_Pesa = conferimento.Item("Tagliando_Pesa"),
                            .Premio_Complessivo = conferimento.Item("Premio_Complessivo"),
                            .Prezzo_Unitario_Finale = conferimento.Item("Prezzo_Unitario_Finale"),
                            .Cod_Varieta = conferimento.Item("Cod_Varieta"),
                            .Desc_Appezzamenti = conferimento.Item("Desc_Appezzamenti"),
                            .Data_Creazione = CDate(conferimento.Item("Data_Creazione")),
                            .Data_Modifica = CDate(conferimento.Item("Data_Modifica")),
                            .Username_Creazione = conferimento.Item("Username_Creazione"),
                            .Username_Modifica = conferimento.Item("Username_Modifica")
                        }

                        If Not IsNothing(conferimento.Item("validita_inizio")) AndAlso
                            IsDate(conferimento.Item("validita_inizio")) Then
                            objMovDetConferimento.Validita_Inizio = CDate(conferimento.Item("validita_inizio"))
                        End If

                        If Not IsNothing(conferimento.Item("validita_fine")) AndAlso
                            IsDate(conferimento.Item("validita_fine")) Then
                            objMovDetConferimento.Validita_fine = CDate(conferimento.Item("validita_fine"))
                        End If

                        listaMovimentiDettagliConferimento.Add(objMovDetConferimento)

                    Next

                End If

            Catch ex As Exception
                Throw New Exception("[ Agenda_Movimenti_Dettagli_Conferimento_Helper.Leggi() ] : " & ex.Message)
            Finally
                Utility.VerificaChiudiConnessione(objParametri, flagConnessione)
            End Try

            Return listaMovimentiDettagliConferimento

        End Function

        Public Function Scrivi(ByVal objMovDetConferimento As Movimento_Dettaglio_Conferimento,
                               ByVal objParametri As AgronicaCoreParametri
                               ) As Boolean

            Dim flagConnessione As Boolean = False
            Dim flagTransazione As Boolean = False

            Dim idRegDettaglio As Integer

            Try

                Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)

                '---------------------------
                'Movimento_Dettaglio_Conferimento
                '---------------------------

                idRegDettaglio = objMovDetConferimento.Id_Reg_Dettaglio

                If idRegDettaglio <= 0 Then

                    Dim objSequenze As New Agro_Sequenze

                    idRegDettaglio = objSequenze.NuovoId_Tabella("MOVIMENTI_DETTAGLI_CONFERIMENTO",
                                                                 objMovDetConferimento.BaseCode,
                                                                 objMovDetConferimento.TopCode,
                                                                 objParametri)

                    objSequenze = Nothing

                End If

                Dim objMovimentiDettagliConferimento As New AgronicaCoreContabDAL.Mov_Dett_Conferimento_W

                objMovimentiDettagliConferimento.Scrivi(Piva:=objMovDetConferimento.Piva,
                                                        Sa_Cod:=objMovDetConferimento.Sa_Cod,
                                                        Id_Agenda:=objMovDetConferimento.Id_Agenda,
                                                        Id_Mov:=objMovDetConferimento.Id_Mov,
                                                        Id_Mov_Det:=objMovDetConferimento.Id_Mov_Det,
                                                        Id_Reg_Dettaglio:=idRegDettaglio,
                                                        Tagliando_Pesa:=objMovDetConferimento.Tagliando_Pesa,
                                                        Premio_Complessivo:=objMovDetConferimento.Premio_Complessivo,
                                                        Prezzo_Unitario_Finale:=objMovDetConferimento.Prezzo_Unitario_Finale,
                                                        Cod_Varieta:=objMovDetConferimento.Cod_Varieta,
                                                        Desc_Appezzamenti:=objMovDetConferimento.Desc_Appezzamenti,
                                                        Validita_Inizio:=objMovDetConferimento.Validita_Inizio,
                                                        Validita_Fine:=objMovDetConferimento.Validita_Fine,
                                                        objParametri:=objParametri,
                                                        Data_creazione:=objMovDetConferimento.Data_Creazione,
                                                        username_creazione:=objMovDetConferimento.Username_Creazione)

                objMovimentiDettagliConferimento = Nothing

                Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

            Catch ex As Exception
                Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)
                Throw New Exception("[ Agenda_Movimenti_Dettagli_Conferimento_Helper.Scrivi() ] : " & ex.Message)
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
                                 ByVal idRegDettaglio As Integer,
                                 ByVal objParametri As AgronicaCoreParametri
                                 ) As Boolean

            Dim flagConnessione As Boolean = False
            Dim flagTransazione As Boolean = False

            Try

                Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)

                '---------------------------
                'Movimento_Dettaglio_Conferimento
                '---------------------------

                Dim objMovimentiDettagliConferimento As New AgronicaCoreContabDAL.Mov_Dett_Conferimento_W

                objMovimentiDettagliConferimento.Cancella(piva, saCod, idAgenda,
                                                          idMov, idMovDet, idRegDettaglio,
                                                          "", objParametri)

                objMovimentiDettagliConferimento = Nothing

                Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

            Catch ex As Exception
                Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)
                Throw New Exception("[ Agenda_Movimenti_Dettagli_Conferimento_Helper.Cancella() ] : " & ex.Message)
            Finally
                Utility.VerificaChiudiConnessione(objParametri, flagConnessione)
            End Try

            Return True

        End Function

    End Class

End Namespace
