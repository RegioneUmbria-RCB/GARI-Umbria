Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider

Namespace OperazioneAgenda_Temp

    Public Class Pagamento

        Sub New(ByVal pivaInput As String, ByVal dataPagamento As Date)
            Piva = pivaInput
            Sa_Cod = 0
            Id_Agenda = 0
            Id_Mov = 0
            Cod_Pagamento = 0

            Importo = 0
            Percentuale = 0
            Data_Pagamento = dataPagamento
            Note = ""
            Cau_Risorsa = ""
            Cod_Liquidita_Dare = 0
            Cod_Liquidita_Avere = 0
            Cau_Pagamento = 0
            Extra_Int = 0
            Extra_Str = ""
            Extra_Date = AGRODATAINIZIO

            Validita_Inizio = AGRODATAINIZIO
            Validita_Fine = AGRODATAFINE

            Cod_Conto_Dare = 0
            Cod_Conto_Avere = 0
            Cod_Conto_Pat_Dare = 0
            Cod_Conto_Pat_Avere = 0
            Tipo_Dare = 0
            Tipo_Avere = 0
            Tipo_Cod_Dare = 0
            Tipo_Cod_Avere = 0
            Anno = 0
            Ric_Cod = 0
            Ric_Cod_Pat = 0
            Previsto_Avvenuto = 0
            cbi_causale = 0
            ChkDataScadenza_Manuale = 0
            DataScadenza_Manuale = AGRODATAFINE

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
            Cod_Pagamento = 0

            Importo = 0
            Percentuale = 0
            Data_Pagamento = AGRODATAINIZIO
            Note = ""
            Cau_Risorsa = ""
            Cod_Liquidita_Dare = 0
            Cod_Liquidita_Avere = 0
            Cau_Pagamento = 0
            Extra_Int = 0
            Extra_Str = ""
            Extra_Date = AGRODATAINIZIO

            Validita_Inizio = AGRODATAINIZIO
            Validita_Fine = AGRODATAFINE

            Cod_Conto_Dare = 0
            Cod_Conto_Avere = 0
            Cod_Conto_Pat_Dare = 0
            Cod_Conto_Pat_Avere = 0
            Tipo_Dare = 0
            Tipo_Avere = 0
            Tipo_Cod_Dare = 0
            Tipo_Cod_Avere = 0
            Anno = 0
            Ric_Cod = 0
            Ric_Cod_Pat = 0
            Previsto_Avvenuto = 0
            cbi_causale = 0
            ChkDataScadenza_Manuale = 0
            DataScadenza_Manuale = AGRODATAFINE

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

        Public Property Cod_Pagamento As Integer

        Public Property Importo As Decimal

        Public Property Percentuale As Decimal

        Public Property Data_Pagamento As Date

        Public Property Note As String

        Public Property Cau_Risorsa As String

        Public Property Cod_Liquidita_Dare As Integer

        Public Property Cod_Liquidita_Avere As Integer

        Public Property Cau_Pagamento As String

        Public Property Extra_Int As Integer

        Public Property Extra_Str As String

        Public Property Extra_Date As Date

        Public Property Validita_Inizio As Date

        Public Property Validita_Fine As Date

        Public Property Cod_Conto_Dare As Integer

        Public Property Cod_Conto_Avere As Integer

        Public Property Cod_Conto_Pat_Dare As Integer

        Public Property Cod_Conto_Pat_Avere As Integer

        Public Property Tipo_Dare As Integer

        Public Property Tipo_Avere As Integer

        Public Property Tipo_Cod_Dare As Integer

        Public Property Tipo_Cod_Avere As Integer

        Public Property Anno As Integer

        Public Property Ric_Cod As Integer

        Public Property Ric_Cod_Pat As Integer

        Public Property Previsto_Avvenuto As Integer

        Public Property cbi_causale As Integer

        Public Property ChkDataScadenza_Manuale As Integer

        Public Property DataScadenza_Manuale As Date

        Public Property Data_Creazione As DateTime
        
        Public Property Data_Modifica As DateTime

        Public Property Username_Creazione As String

        Public Property Username_Modifica As String

        Public Property TopCode As Integer

        Public Property BaseCode As Integer

    End Class

    Public Class Agenda_Pagamenti_Helper

        Public Function Scrivi(ByVal Pagamento As Pagamento,
                               ByVal objParametri As AgronicaCoreParametri
                               ) As Boolean

            Dim flagConnessione As Boolean = False
            Dim flagTransazione As Boolean = False

            Dim codPagamento As Integer

            Try

                Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)

                '---------------------------
                'Movimento_Dettaglio_Tecnico
                '---------------------------

                codPagamento = Pagamento.Cod_Pagamento

                If codPagamento <= 0 Then

                    Dim objSequenze As New Agro_Sequenze

                    codPagamento = objSequenze.NuovoId_Tabella("Pagamento",
                                                               Pagamento.BaseCode,
                                                               Pagamento.TopCode,
                                                               objParametri)

                    objSequenze = Nothing

                End If

                Dim objPagamenti As New AgronicaCoreContabDAL.Pagamenti_W

                objPagamenti.Scrivi(Pagamento.Piva,
                                    Pagamento.Sa_Cod,
                                    Pagamento.Id_Agenda,
                                    Pagamento.Id_Mov,
                                    codPagamento,
                                    Pagamento.Importo,
                                    Pagamento.Percentuale,
                                    Pagamento.Data_Pagamento,
                                    Pagamento.Note,
                                    Pagamento.Cau_Risorsa,
                                    Pagamento.Cod_Liquidita_Dare,
                                    Pagamento.Cod_Liquidita_Avere,
                                    Pagamento.Cau_Pagamento,
                                    Pagamento.Extra_Str,
                                    Pagamento.Extra_Int,
                                    Pagamento.Extra_Date,
                                    Pagamento.Validita_Inizio,
                                    Pagamento.Validita_Fine,
                                    objParametri,
                                    Cod_Conto_Dare:=Pagamento.Cod_Conto_Dare,
                                    Cod_Conto_Avere:=Pagamento.Cod_Conto_Avere,
                                    Cod_Conto_Pat_Dare:=Pagamento.Cod_Conto_Pat_Dare,
                                    Cod_Conto_Pat_Avere:=Pagamento.Cod_Conto_Pat_Avere,
                                    Tipo_Dare:=Pagamento.Tipo_Dare,
                                    Tipo_Avere:=Pagamento.Tipo_Avere,
                                    Tipo_Cod_Dare:=Pagamento.Tipo_Cod_Dare,
                                    Tipo_Cod_Avere:=Pagamento.Tipo_Cod_Avere,
                                    Anno:=Pagamento.Anno,
                                    Ric_Cod:=Pagamento.Ric_Cod,
                                    Ric_Cod_Pat:=Pagamento.Ric_Cod_Pat,
                                    Previsto_Avvenuto:=Pagamento.Previsto_Avvenuto,
                                    cbi_causale:=Pagamento.cbi_causale,
                                    ChkDataScadenza_Manuale:=Pagamento.ChkDataScadenza_Manuale,
                                    DataScadenza_Manuale:=Pagamento.DataScadenza_Manuale,
                                    Data_creazione:=Pagamento.Data_Creazione,
                                    username_creazione:=Pagamento.Username_Creazione
                                    )

                objPagamenti = Nothing

                Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

            Catch ex As Exception

                Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)
                Throw New Exception("[ Agenda_Pagamenti_Helper.scrivi() ] : " & ex.Message)
            Finally

                Utility.VerificaChiudiConnessione(objParametri, flagConnessione)

            End Try

            Return True

        End Function

        Public Function Cancella(ByVal piva As String,
                                 ByVal saCod As Integer,
                                 ByVal idAgenda As Integer,
                                 ByVal idMov As Integer,
                                 ByVal codPagamento As Integer,
                                 ByVal objParametri As AgronicaCoreParametri
                                 ) As Boolean

            Dim flagConnessione As Boolean = False
            Dim flagTransazione As Boolean = False

            Try

                Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)

                Dim objPagamenti As New AgronicaCoreContabDAL.Pagamenti_W

                objPagamenti.Cancella(piva,
                                      saCod,
                                      idAgenda,
                                      idMov,
                                      codPagamento,
                                      "",
                                      objParametri)

                objPagamenti = Nothing

                Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

            Catch ex As Exception

                Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)
                Throw New Exception("[ Agenda_Pagamenti_Helper.cancella() ] : " & ex.Message)

            Finally

                Utility.VerificaChiudiConnessione(objParametri, flagConnessione)

            End Try

            Return True

        End Function

        Public Function Leggi(ByVal piva As String,
                              ByVal saCod As Integer,
                              ByVal idAgenda As Integer,
                              ByVal idMov As Integer,
                              ByVal codPagamento As Integer,
                              ByVal objParametri As AgronicaCoreParametri
                              ) As List(Of Pagamento)

            Dim flagConnessione As Boolean = False

            Dim listaPagamenti As New List(Of Pagamento)
            Dim Pagamento As Pagamento

            Try

                Utility.VerificaApriConnessione(objParametri, flagConnessione)


                Dim objPagamenti = New AgronicaCoreContabDAL.Pagamenti_R
                Dim Dt_Pagamenti As DataTable

                Dt_Pagamenti = objPagamenti.Leggi(CStr(piva),
                                                  CInt(saCod),
                                                  CInt(idAgenda),
                                                  idMov,
                                                  codPagamento,
                                                  enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                  "",
                                                  "",
                                                  objParametri)

                objPagamenti = Nothing

                If Dt_Pagamenti.Rows.Count > 0 Then

                    For j = 0 To Dt_Pagamenti.Rows.Count - 1

                        Pagamento = New Pagamento With {
                            .Piva = Dt_Pagamenti.Rows(j).Item("Piva"),
                            .Sa_Cod = Dt_Pagamenti.Rows(j).Item("Sa_Cod"),
                            .Id_Agenda = Dt_Pagamenti.Rows(j).Item("Id_Agenda"),
                            .Id_Mov = Dt_Pagamenti.Rows(j).Item("Id_Mov"),
                            .Cod_Pagamento = Dt_Pagamenti.Rows(j).Item("Cod_Pagamento"),
                            .Importo = Dt_Pagamenti.Rows(j).Item("Importo"),
                            .Percentuale = Dt_Pagamenti.Rows(j).Item("Percentuale"),
                            .Data_Pagamento = Dt_Pagamenti.Rows(j).Item("Data_Pagamento"),
                            .Note = Dt_Pagamenti.Rows(j).Item("Note"),
                            .Cau_Risorsa = Dt_Pagamenti.Rows(j).Item("Cau_Risorsa"),
                            .Cod_Liquidita_Dare = Dt_Pagamenti.Rows(j).Item("Cod_Liquidita_Dare"),
                            .Cod_Liquidita_Avere = Dt_Pagamenti.Rows(j).Item("Cod_Liquidita_Avere"),
                            .Cau_Pagamento = Dt_Pagamenti.Rows(j).Item("Cau_Pagamento"),
                            .Extra_Int = Dt_Pagamenti.Rows(j).Item("Extra_Int"),
                            .Extra_Str = Dt_Pagamenti.Rows(j).Item("Extra_Str"),
                            .Extra_Date = Dt_Pagamenti.Rows(j).Item("Extra_Date"),
                            .Validita_Inizio = Dt_Pagamenti.Rows(j).Item("Validita_Inizio"),
                            .Validita_Fine = Dt_Pagamenti.Rows(j).Item("Validita_Fine"),
                            .Data_Creazione = CDate(Dt_Pagamenti.Rows(j).Item("Data_Creazione")),
                            .Data_Modifica = CDate(Dt_Pagamenti.Rows(j).Item("Data_Modifica")),
                            .Username_Creazione = Dt_Pagamenti.Rows(j).Item("Username_Creazione"),
                            .Username_Modifica = Dt_Pagamenti.Rows(j).Item("Username_Modifica")
                        }

                        If Not IsDBNull(Dt_Pagamenti.Rows(j).Item("Cod_Conto_Dare")) Then
                            Pagamento.Cod_Conto_Dare = Dt_Pagamenti.Rows(j).Item("Cod_Conto_Dare")
                        End If

                        If Not IsDBNull(Dt_Pagamenti.Rows(j).Item("Cod_Conto_Avere")) Then
                            Pagamento.Cod_Conto_Avere = Dt_Pagamenti.Rows(j).Item("Cod_Conto_Avere")
                        End If

                        If Not IsDBNull(Dt_Pagamenti.Rows(j).Item("Cod_Conto_Pat_Dare")) Then
                            Pagamento.Cod_Conto_Pat_Dare = Dt_Pagamenti.Rows(j).Item("Cod_Conto_Pat_Dare")
                        End If

                        If Not IsDBNull(Dt_Pagamenti.Rows(j).Item("Cod_Conto_Pat_Avere")) Then
                            Pagamento.Cod_Conto_Pat_Avere = Dt_Pagamenti.Rows(j).Item("Cod_Conto_Pat_Avere")
                        End If

                        If Not IsDBNull(Dt_Pagamenti.Rows(j).Item("Tipo_Dare")) Then
                            Pagamento.Tipo_Dare = Dt_Pagamenti.Rows(j).Item("Tipo_Dare")
                        End If

                        If Not IsDBNull(Dt_Pagamenti.Rows(j).Item("Tipo_Avere")) Then
                            Pagamento.Tipo_Avere = Dt_Pagamenti.Rows(j).Item("Tipo_Avere")
                        End If

                        If Not IsDBNull(Dt_Pagamenti.Rows(j).Item("Tipo_Cod_Dare")) Then
                            Pagamento.Tipo_Cod_Dare = Dt_Pagamenti.Rows(j).Item("Tipo_Cod_Dare")
                        End If

                        If Not IsDBNull(Dt_Pagamenti.Rows(j).Item("Tipo_Cod_Avere")) Then
                            Pagamento.Tipo_Cod_Avere = Dt_Pagamenti.Rows(j).Item("Tipo_Cod_Avere")
                        End If

                        If Not IsDBNull(Dt_Pagamenti.Rows(j).Item("Anno")) Then
                            Pagamento.Anno = Dt_Pagamenti.Rows(j).Item("Anno")
                        End If

                        If Not IsDBNull(Dt_Pagamenti.Rows(j).Item("Ric_Cod")) Then
                            Pagamento.Ric_Cod = Dt_Pagamenti.Rows(j).Item("Ric_Cod")
                        End If

                        If Not IsDBNull(Dt_Pagamenti.Rows(j).Item("Ric_Cod_Pat")) Then
                            Pagamento.Ric_Cod_Pat = Dt_Pagamenti.Rows(j).Item("Ric_Cod_Pat")
                        End If

                        If Not IsDBNull(Dt_Pagamenti.Rows(j).Item("Previsto_Avvenuto")) Then
                            Pagamento.Previsto_Avvenuto = Dt_Pagamenti.Rows(j).Item("Previsto_Avvenuto")
                        End If

                        If Not IsDBNull(Dt_Pagamenti.Rows(j).Item("cbi_causale")) Then
                            Pagamento.cbi_causale = Dt_Pagamenti.Rows(j).Item("cbi_causale")
                        End If

                        If Not IsDBNull(Dt_Pagamenti.Rows(j).Item("ChkDataScadenza_Manuale")) Then
                            Pagamento.ChkDataScadenza_Manuale = Dt_Pagamenti.Rows(j).Item("ChkDataScadenza_Manuale")
                        End If

                        If Not IsDBNull(Dt_Pagamenti.Rows(j).Item("DataScadenza_Manuale")) Then
                            Pagamento.DataScadenza_Manuale = Dt_Pagamenti.Rows(j).Item("DataScadenza_Manuale")
                        End If


                        listaPagamenti.Add(Pagamento)

                    Next

                End If

            Catch ex As Exception

                Throw New Exception("[ Agenda_Pagamenti_Helper.leggi() ] : " & ex.Message)

            Finally

                Utility.VerificaChiudiConnessione(objParametri, flagConnessione)
            End Try

            Return listaPagamenti

        End Function


        Public Function Modifica(ByVal Pagamento As Pagamento,
                               ByVal objParametri As AgronicaCoreParametri
                               ) As Boolean

            Dim flagConnessione As Boolean = False
            Dim flagTransazione As Boolean = False

            Try

                Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)

                '---------------------------
                'Pagamento
                '---------------------------

                Dim objPagamenti As New AgronicaCoreContabDAL.Pagamenti_W

                objPagamenti.Modifica(objParametri,
                                      Pagamento.Piva,
                                      Pagamento.Sa_Cod,
                                      Pagamento.Id_Agenda,
                                      Pagamento.Id_Mov,
                                      Pagamento.Cod_Pagamento,
                                      Cau_Pagamento:=Pagamento.Cau_Pagamento)

                objPagamenti = Nothing

                Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

            Catch ex As Exception

                Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)
                Throw New Exception("[ Agenda_Pagamenti_Helper.scrivi() ] : " & ex.Message)
            Finally

                Utility.VerificaChiudiConnessione(objParametri, flagConnessione)

            End Try

            Return True

        End Function


    End Class

End Namespace
